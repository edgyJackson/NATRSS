using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpillTracker.Utilities
{
    public class RegexParserUtilities
    {
        //refactoring code to make it testable
        public static double RegexDensityParse(string input)
        {
            double density;
            string densitytest = Regex.Match(input, @"^\d*\.*\d*\s").Value;

            if (densitytest == "")
            {
                density = double.Parse(Regex.Match(input, @"^(\d*\.*\d*)-(\d*\.*\d*)").Groups[2].Value);
            }
            else
            {
                density = double.Parse(densitytest);
            }
            
            return density;
        }

        public static double RegexVaporParse(string vaporPressureString)
        {
            vaporPressureString = vaporPressureString.Replace("X10-", "e-");
            return (double)Decimal.Parse(Regex.Match(vaporPressureString, @"^\d*\.*\d*[e,E]*-*\d*").Value, NumberStyles.Float);
        }
    }
}
