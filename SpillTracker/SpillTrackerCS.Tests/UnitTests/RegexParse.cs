using NUnit.Framework;
using SpillTracker.Controllers;
using SpillTracker.Utilities;
namespace SpillTrackerCS.Tests.UnitTests
{
    public class  RegexParseUtility
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void RegexDensityParse_RegexParsesDoubleOutOfDensityString_Success()
        {   //arrange
            string input = "1.51 at 68 °F (NTP, 1992)";
            //act
            double output = RegexParserUtilities.RegexDensityParse(input);
            //assert
            Assert.AreEqual(1.51, output);
        }

        [Test]
        public void RegexDensityParse_RegexParsesHigherDensityOutOfDensityStringWithARange_Success()
        {   //arrange
            string input = "1.51-3.51 at 68 °F (NTP, 1992)";
            //act
            double output = RegexParserUtilities.RegexDensityParse(input);
            //assert
            Assert.AreEqual(3.51, output);
        }

        [Test]
        public void RegexVaporParse_RegexParsesVaporPressureFromVaporString_Success()
        {   //arrange
            string input = "10 mm Hg at 66.7 °F ; 20 mm Hg at 89.8° F (NTP, 1992)";
            //act
            double output = RegexParserUtilities.RegexVaporParse(input);
            //assert
            Assert.That(10, Is.EqualTo(output));
        }

        [Test]
        public void RegexVaporParse_RegexConvertsX10ScientificNotationToEformat_Success()
        {   //arrange
            string input = "1.25X10-10 mm Hg at 25 °C (est)";
            //act
            double output = RegexParserUtilities.RegexVaporParse(input);
            //assert
            Assert.AreEqual(1.25e-10, output);
        }

        [Test]
        public void RegexVaporParse_RegexCatchesObscureFormatAndParsesInCorrectUnits_Success()
        {   //arrange
            string input = "Vapor pressure (solid): 2.6 Pa at 117 °C; 10 Pa at 146 °C; 100 Pa at 181 °C; 1 kPa at 222 °C; 10 kPa at 272 °C; 100 kPa at 336 °C";
            //act
            double output = RegexParserUtilities.RegexVaporParse(input);
            //assert
            Assert.AreEqual(0.0195, output);
        }
    }
}