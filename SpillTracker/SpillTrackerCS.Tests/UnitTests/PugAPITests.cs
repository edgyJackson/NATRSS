using Moq;
using NUnit.Framework;
using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using SpillTracker.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SpillTrackerCS.Tests.UnitTests
{
    class PugAPITests
    {
        JsonStrings theJsons = new JsonStrings();
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void PugAPI_GitCIDAndMolWeightFromPugRest_RetrievesCIDandMolWeightWhenURLUsesRegularCASFormat()
        {
            // Mock the API Call interface
            Mock<IAPICall> mockAPICall = new Mock<IAPICall>();
            mockAPICall.Setup(m => m.DoAPICall(It.IsAny<string>())).Returns(theJsons.MolecularweightJson);
            //Arrange
            PugAPI pAPI = new PugAPI(mockAPICall.Object);
            //Act
            ExtraChemData result = pAPI.GitCIDAndMolWeightFromPugRest("theURL");

            //Assert
            Assert.That(result.CID, Is.EqualTo(7847));
            Assert.That(result.MolecularWeight, Is.EqualTo(56.06));
        }

        [Test]
        public void PugAPI_GitCIDAndMolWeightFromPugRest_RetrievesCIDandMolWeightWhenURLUsesCASDashFormat()
        {
            // Mock the API Call interface          
            Mock<IAPICall> mockAPICall = new Mock<IAPICall>();
            mockAPICall.Setup(m => m.DoAPICall(It.Is<string>(s =>!s.Contains("CAS-")))).Throws(new Exception()); 
            mockAPICall.Setup(m => m.DoAPICall(It.Is<string>(s=>s.Contains("CAS-")))).Returns(theJsons.MolecularweightJson);

            //Arrange
            PugAPI pAPI = new PugAPI(mockAPICall.Object);

            //Act
            ExtraChemData result = pAPI.GitCIDAndMolWeightFromPugRest("https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/10026-11-6/property/MolecularWeight/json");
 
            //Assert
            Assert.That(result.CID, Is.EqualTo(7847));
            Assert.That(result.MolecularWeight, Is.EqualTo(56.06));
        }

        [Test]
        public void PugAPI_GetDensVapPresFromPUGView_RetrievesDensityAndVaporPressureWhenInStandardFormat()
        {
            // Mock the API Call interface          
            Mock<IAPICall> mockAPICall = new Mock<IAPICall>();
            mockAPICall.Setup(m => m.DoAPICall(It.Is<string>(s => s.Contains("Density")))).Returns(theJsons.DensityJson);
            mockAPICall.Setup(m => m.DoAPICall(It.Is<string>(s => s.Contains("Vapor")))).Returns(theJsons.VaporPressureJson);

            //Arrange
            PugAPI pAPI = new PugAPI(mockAPICall.Object);
            ExtraChemData p = new ExtraChemData();
            //Act
            ExtraChemData result = pAPI.GetDensVapPresFromPUGView(p);

            //Assert
            Assert.That(result.Density, Is.EqualTo(0.6818));
            Assert.That(result.VaporPressure, Is.EqualTo(400));
        }

     
    }     

} 
