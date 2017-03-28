using System.Text.RegularExpressions;

namespace kuujinbo.DataTables.Utils
{
    public class RegexUtils
    {
        static Regex pascalRegex = new Regex(
        @"	# lookbehind/lookahead match on **boundaries**
            # positive lookbehind
            (?<=			# start
                [A-Za-z]	# SINGLE upper OR lower
            )         		# end
            
            # positive lookahead
            (?=				# start
                [A-Z][a-z]	# upper FOLLOWED by lower
            )         		# end
            ",
             RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
        );

        public static string PascalCaseSplit(string input)
        {
            return input != null ? pascalRegex.Replace(input, " ") : input;
        }
    }
}