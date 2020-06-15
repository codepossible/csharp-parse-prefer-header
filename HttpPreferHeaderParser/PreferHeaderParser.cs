using System.Collections.Generic;
using System.Linq;

namespace HttpPreferHeaderParser
{

    /// <summary>
    /// Class to parse the HTTP Prefer header into a list of Preference Objects based on specifications provided in RFC 7240 (https://tools.ietf.org/html/rfc7240).
    /// 
    /// This code is a port to C# from NodeJS. The NodeJS code is written by Partick Paskaris. 
    /// NodeJs version of the code is available here - https://github.com/ppaskaris/node-parse-prefer-header
    /// 
    /// </summary>
    public class PreferHeaderParser
    {
        /// <summary>
        /// Parse a list of string array containing multiple Prefer headers
        /// </summary>
        /// <param name="headerValues">List of string containing contents of multiple HTTP Prefer headers</param>
        /// <returns>List of Preferences parsed from the header values</returns>
        public List<Preference> Parse(string[] headerValues)
        {

            return headerValues?.Length > 0 ?
                    Parse(string.Join(",", headerValues)) :
                    null;

        }

        /// <summary>
        /// Parses the string containing multiple Prefer values and parameters
        /// </summary>
        /// <param name="headerValue">String containing the contents of HTTP Prefer header</param>
        /// <returns>List of Preferences parsed from the header value</returns>
        public List<Preference> Parse(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                return null;
            }


            var preferences = new List<Preference>();


            int start = 0;
            bool isQuote = false;

            for (var i = 0; i <= headerValue.Length; i++)
            {

                var c = i < headerValue.Length ? headerValue[i] : ',';
                if (c == '"')
                {
                    isQuote = !isQuote;
                }
                else if (c == ',' && !isQuote)
                {
                    var headerPreference = headerValue.Substring(start, (i - start));
                    start = i + 1;
                    Preference preference = ParsePreference(headerPreference);
                    if (!preferences.Any(p => p.Name.Equals(preference.Name)))
                    {
                        preferences.Add(preference);
                    }
                }
            }
            return preferences;
        }


        /// <summary>
        /// Helper method to split preference into name, value and parameter and parameter value.
        /// </summary>
        /// <param name="headerPreference">String containing the Preference</param>
        /// <returns>Preference object containing the preference, values and parameters</returns>
        private Preference ParsePreference(string headerPreference)
        {

            if (string.IsNullOrEmpty(headerPreference))
            {
                return null;
            }

            Preference preference = null;

            int start = 0;
            bool isQuote = false;

            for (var i = 0; i <= headerPreference.Length; i++)
            {

                char c = i < headerPreference.Length ? headerPreference[i] : ';';
                if (c == '"')
                {
                    isQuote = !isQuote;
                }
                else if (c == ';' && !isQuote)
                {

                    var segment = headerPreference.Substring(start, (i - start));
                    var index = segment.IndexOf('=');

                    string token;
                    string value;

                    if (index < 0)
                    {
                        token = segment.Trim().ToLower();
                        value = "true";
                    }
                    else
                    {
                        token = segment.Substring(0, index).Trim().ToLower();
                        value = segment.Substring(index + 1).Trim();                     

                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Replace("\"", "");
                        }

                        value = string.IsNullOrEmpty(value) ? "true" : value;
                    }

                    token = token.Replace(" ", "");

                    if (preference == null)
                    {
                        preference = new Preference() { Name = token, Value = value };
                    }
                    else
                    {
                        preference.Parameters.Add(token, value);
                    }

                    start = i + 1;
                }
            }

            return preference;
        }
    }
}
