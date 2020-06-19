using System.Collections.Generic;
using Xunit;

namespace HttpPreferHeaderParser.Test
{
    public class ConverterTest
    {

        #region Data Generators

        public static IEnumerable<object[]> GetSinglePreferences()
        {
            var data = new List<object[]>
            {
                new object[] { new Preference { Name = "foo", Value = "true" }, "foo" },
                new object[] { new Preference { Name = "foo", Value = "42" }, "foo=42" },
                new object[] { new Preference { Name = "foo", Value = "bar, baz; qux" }, "foo=\"bar, baz; qux\""},
                new object[] { new Preference { Name = "response-cache-headers", Value="etag, last-modified"}, "response-cache-headers=\"etag, last-modified\"" }

            };

            return data;
        }

        public static IEnumerable<object[]> GetPreferencesWithParameters()
        {
            var data = new List<object[]>
            {
                new object[] {
                    new Preference { Name = "foo", Value = "true" },
                    new Dictionary<string, string> { {"bar", "true" } },
                    "foo; bar" },
                new object[] {
                    new Preference { Name = "respond-async", Value = "true" },
                    new Dictionary<string, string> { {"wait", "100" } },
                    "respond-async; wait=100" },
                new object[] {
                    new Preference { Name = "handling", Value = "strict" },
                    new Dictionary<string, string> { { "abort-early", "true" }, { "path-format", "json-pointer" } },
                    "handling=strict; abort-early; path-format=json-pointer" }
            };
           
            return data;
        }
        
        #endregion

        #region Unit Tests

        [Fact]
        public void Test001_ConvertNullPreference()
        {
            var objectUnderTest = new PreferHeaderParser();

            Preference preference = null;
            var headerValue = objectUnderTest.ConvertToHeaderValue(preference);

            Assert.Null(headerValue);
       
        }

        [Fact]
        public void Test002_ConvertNullListOfPreferences()
        {
            var objectUnderTest = new PreferHeaderParser();

            List<Preference> preferenceList = null;
            var headerValues = objectUnderTest.ConvertToHeaderValue(preferenceList);

            Assert.Null(headerValues);

        }

        [Theory]
        [MemberData(nameof(GetSinglePreferences))]
        public void Test003_ConvertPreference(Preference preference, string expectedHeaderValue)
        {
            var objectUnderTest = new PreferHeaderParser();

            var headerValue = objectUnderTest.ConvertToHeaderValue(preference);

            Assert.NotNull(headerValue);
            Assert.Equal(expectedHeaderValue, headerValue);
        }
        
        [Theory]
        [MemberData(nameof(GetPreferencesWithParameters))]
        public void Test004_ConvertPreferenceWithParameters(Preference preference, Dictionary<string, string> parametersToAdd, string expectedHeaderValue)
        {
            var objectUnderTest = new PreferHeaderParser();

            if (parametersToAdd != null && parametersToAdd.Keys.Count > 0)
            {
                foreach (var key in parametersToAdd.Keys)
                {
                    preference.Parameters.Add(key, parametersToAdd[key]);
                }
            }

            var headerValue = objectUnderTest.ConvertToHeaderValue(preference);

            Assert.NotNull(headerValue);
            Assert.Equal(expectedHeaderValue, headerValue);
        }

        #endregion

    }
}
