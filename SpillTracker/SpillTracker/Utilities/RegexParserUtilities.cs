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
            double vp=0.0;
            //ensure that the value is in scientific e notation
            vaporPressureString = vaporPressureString.Replace("X10-", "e-");
            try
            {
                vp = (double)Decimal.Parse(Regex.Match(vaporPressureString, @"^\d*\.*\d*[e,E]*-*\d*").Value, NumberStyles.Float);
            }
            catch (Exception)
            {
                //Irregular format or no vapor pressure found
            }

            //Check for units that are in pascals and convert them to mm hg
            if (Regex.IsMatch(vaporPressureString, @"^\d*\.*\d*[e,E]*-*\d*\s[P,p][A,a]"))
            {
                vp = vp * .0075;
            }

            //Check obscure api return format incase it uses it
            if (Regex.IsMatch(vaporPressureString, @"^[V,v]apor\s*[P,p]ressure\s\([S,s]olid\):\s(\d*\.*\d*[e,E]*-*\d*)\s[P,p][A,a]"))
            {
                vp = (double)Decimal.Parse(Regex.Match(vaporPressureString, @"^[V,v]apor\s*[P,p]ressure\s\([S,s]olid\):\s(\d*\.*\d*[e,E]*-*\d*)\s[P,p][A,a]").Groups[1].Value, NumberStyles.Float) * .0075;
            }

            return vp;

        }
    }
}
