using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SpillTracker.Models;
using GetStarted.Tests.Utilites;

namespace SpillTrackerCS.Tests.UnitTests
{
    [TestFixture]
    class ContactInfo_Verify
    {

        private static ContactInfo MakeValidContactInfo()
        {
            return new ContactInfo
            {
                AgencyName = "Mcdonalds",
                PhoneNumber = "555-555-5555",
                State = "Oregon"

            };
        }

        [Test]
        public void ContactInfo_DefaultIs_NotValid()
        {
            //arrange
            ContactInfo ci = new ContactInfo();

            //act
            ModelValidator mv = new ModelValidator(ci);

            //assert
            /*Assert.That(ci.AgencyName, Is.Not.Null);*/
            Assert.That(mv.Valid, Is.False);
        }

        [Test]
        public void ContactInfo_RequiredNullablePropertiesAreNullMeansIs_NotValid()
        {
            // Arrange
            ContactInfo ci = new ContactInfo
            {
                AgencyName = null,
                PhoneNumber = null,
                State = null
            };
            // Act
            ModelValidator mv = new ModelValidator(ci);
            // Assert
            // Was one reason why it failed that the Title property didn't validate?
            Assert.That(mv.ContainsFailureFor("AgencyName"), Is.True);
            Assert.That(mv.ContainsFailureFor("PhoneNumber"), Is.True);
            Assert.That(mv.ContainsFailureFor("State"), Is.True);
            // Did it fail? (this is a more general test so should go last)
            Assert.That(mv.Valid, Is.False);
        }

        [Test]
        public void ContactInfo_AllRequiredPropertiesAreNonNullAndValidValuesIs_Valid()
        {
            // Arrange
            ContactInfo ci = MakeValidContactInfo();
            // Act
            ModelValidator mv = new ModelValidator(ci);
            // Assert
            Assert.That(mv.Valid, Is.True);
        }

        //****************Phone Number String Length****************
        [Test]
        public void ContactInfo_PhoneNumberExceeding20CharsIs_NotValid()
        {
            // Arrange
            ContactInfo ci = MakeValidContactInfo();
            ci.PhoneNumber = "555-555-5555-5555-5555-5555";
            // Act
            ModelValidator mv = new ModelValidator(ci);
            // Assert
            Assert.That(mv.ContainsFailureFor("PhoneNumber"), Is.True);
            Assert.That(mv.Valid, Is.False);           
        }

        [TestCase("555-555-5555")]
        [TestCase("555-555-")]
        [TestCase("555-55")]
        [TestCase("55")]
        [TestCase("5")]
        public void ContactInfo_PhoneNumberLesserThan20CharsIs_Valid(string pn)
        {
            // Arrange
            ContactInfo ci = MakeValidContactInfo();
            ci.PhoneNumber = pn;

            // Act
            ModelValidator mv = new ModelValidator(ci);
            // Assert
            Assert.That(mv.ContainsFailureFor("PhoneNumber"), Is.False);
            Assert.That(mv.Valid, Is.True);
        }
    }
}
