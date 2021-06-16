﻿using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SpillTracker.Controllers;
using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using SpillTracker.Models.Repositories;
using SpillTrackerCS.Tests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpillTrackerCS.Tests.UnitTests
{

    public class ChemicalsControllerTests
    {
        private readonly List<Chemical> data = MockTest.GenerateChemsList();
        private readonly Mock<SpillTrackerDbContext> mockContext = new Mock<SpillTrackerDbContext>();
        private readonly Mock<ISpillTrackerChemicalRepository> mockChemicalsRepo = new Mock<ISpillTrackerChemicalRepository>();

        Mock<IPugAPI> mockPugAPI = new Mock<IPugAPI>();
        [SetUp]
        public void Setup()
        {
            //Mock the Chemical Repository         
            mockChemicalsRepo.Setup(m => m.GetChemByCAS(It.IsAny<string>())).Returns(new Chemical() { CasNum = "10026-11-6" });
            mockChemicalsRepo.Setup(m => m.AddOrUpdateAsync(It.IsAny<Chemical>())).Verifiable();

            // Mock the pugAPI
            mockPugAPI.Setup(m => m.GitCIDAndMolWeightFromPugRest(It.IsAny<string>())).Returns(new ExtraChemData()
            {
                CAS = "10026-11-6",
                CID = 24817,
                MolecularWeight = 233.0
            });
            mockPugAPI.Setup(m => m.GetDensVapPresFromPUGView(It.IsAny<ExtraChemData>())).Returns(new ExtraChemData()
            {
                CAS = "10026-11-6",
                CID = 24817,
                MolecularWeight = 233.0,
                Density = 2.083,
                VaporPressure = .0075
            });
        }

        [Test]
        public async Task ChemnicalController_GetChemicalPropertiesFromPUGAPIAsyncUpdatesDBifPropertiesAreNull()
        {
            mockChemicalsRepo.Setup(m => m.TheCIDIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockChemicalsRepo.Setup(m => m.TheMolecularWeightIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockChemicalsRepo.Setup(m => m.TheDensityIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockChemicalsRepo.Setup(m => m.TheVaporPressureIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            //Arrange
            ChemicalsController chemicalsController = new ChemicalsController(mockChemicalsRepo.Object, mockPugAPI.Object);

            //Act
            ExtraChemData result = await chemicalsController.GetChemicalPropertiesFromPUGAPIAsync("10026-11-6");

            //Assert
            mockChemicalsRepo.Verify(x => x.AddOrUpdateAsync(It.IsAny<Chemical>()), Times.Exactly(4));
            Assert.That(result.CID, Is.EqualTo(24817));
            Assert.That(result.MolecularWeight, Is.EqualTo(233.0));
            Assert.That(result.Density, Is.EqualTo(2.083));
            Assert.That(result.VaporPressure, Is.EqualTo(.0075));
        }

        [Test]
        public async Task ChemnicalController_GetChemicalPropertiesFromPUGAPIAsyncUpdatesOnlyNonNullPropertiesInDB()
        {
            mockChemicalsRepo.Setup(m => m.TheCIDIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockChemicalsRepo.Setup(m => m.TheMolecularWeightIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
            mockChemicalsRepo.Setup(m => m.TheDensityIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
            mockChemicalsRepo.Setup(m => m.TheVaporPressureIsNullAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
            //Arrange
            ChemicalsController chemicalsController = new ChemicalsController(mockChemicalsRepo.Object, mockPugAPI.Object);

            //Act
            ExtraChemData result = await chemicalsController.GetChemicalPropertiesFromPUGAPIAsync("10026-11-6");

            //Assert
            mockChemicalsRepo.Verify(x => x.AddOrUpdateAsync(It.IsAny<Chemical>()), Times.Exactly(2));         
        }

        [Test]
        public void ChemnicalRepo_OrdersByNameandReturnsChemicalsThatHaveNumbersinTheirName()
        {

            Mock<DbSet<Chemical>> mockChemicalsDbSet = MockTest.GetMockDbSet(data.AsQueryable());
            mockContext.Setup(ctx => ctx.Chemicals).Returns(mockChemicalsDbSet.Object);


            //Arrange
            ISpillTrackerChemicalRepository chemRepo = new SpillTrackerChemicalRepository(mockContext.Object);

            //Act

            var chemsOrderedByName = chemRepo.OrderByName();
            var ChemsWithNumbersinName = chemRepo.getHashTag();


            //Assert
            Assert.That(chemsOrderedByName[3].Name, Is.EqualTo("Bocho"));
            Assert.That(ChemsWithNumbersinName.Count(), Is.EqualTo(1));

        }

    }
}
