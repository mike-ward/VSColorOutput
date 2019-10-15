using System;
using System.Text.RegularExpressions;

namespace VSColorOutput.Output.ColorClassifier
{
    public class RegExClassification
    {
        private string _regExPattern;

				public RegExClassification() => _regExPattern = ".*";

				public string RegExPattern
				{
					get => _regExPattern;
					set
					{
						ValidatePattern(value);
						_regExPattern = value;
					}
				}

		public ClassificationTypes ClassificationType { get; set; }
        public bool IgnoreCase { get; set; }

        public override string ToString()
        {
            return $"\"{RegExPattern ?? "null"}\",{ClassificationType},{IgnoreCase}";
        }

        private static void ValidatePattern(string regex)
        {
            new Regex(regex);
        }

        public static Regex RegExFactory(RegExClassification pattern)
        {
            return new Regex(
                pattern.RegExPattern, 
                pattern.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 
                TimeSpan.FromMilliseconds(250));
        }
    }
}