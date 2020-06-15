using System.Collections.Generic;

namespace HttpPreferHeaderParser {
     public class Preference {
        public string Name {get; set;}
        public string Value { get; set; }
        public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
    }
}