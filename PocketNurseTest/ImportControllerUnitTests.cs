using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PocketNurse.Controllers;
using PocketNurse.Models;
using PocketNurse.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PocketNurseTest
{
    [TestClass]
    public class ImportControllerUnitTests
    {
        Mock<IPocketNurseRepository> _pocketNurseRepository;
        Mock<UserManager<ApplicationUser>> _userManager;
        ImportController _controller;
        Mock<IFormFile> _file;
        long _fileSize;
        [TestInitialize]
        public void Setup()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>();
            _pocketNurseRepository = new Mock<IPocketNurseRepository>();
            _pocketNurseRepository.Setup(m => m.GetAllPatients()).Returns(GetTestPatients());
            _pocketNurseRepository.Setup(m => m.FindPatient(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult<Patient>(GetTestPatients().First()));
            _controller = new ImportController(_userManager.Object, _pocketNurseRepository.Object);
        }
        private void SetupFile(string path)
        {
            var filePath = path;
            var testFileStream = ReadTestData(filePath);
            var fileInfo = new FileInfo(filePath);
            _fileSize = fileInfo.Length;
            //Arrange
            _file = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            _file.Setup(_ => _.OpenReadStream()).Returns(testFileStream);
            _file.Setup(_ => _.FileName).Returns("SampleRequest.xlsx");
            _file.Setup(_ => _.Length).Returns(_fileSize);
            _file.Setup(m => m.CopyToAsync(It.IsAny<Stream>(), default(CancellationToken))).Returns(Task.CompletedTask);

        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void ImportUploadTest()
        {
            SetupFile("../../../SampleRequest1.xlsx");
            var response = _controller.Upload(_file.Object).Result;
            Assert.IsInstanceOfType(response, typeof(IActionResult));
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)response).Model, typeof(OmnicellCabinetViewModel));
            var modelResponse = ((ViewResult)response).Model as OmnicellCabinetViewModel;
            Debug.WriteLine($"Patients {modelResponse.Patients.Count} NotInFormulary {modelResponse.NotInFormulary.Count}");
            Assert.IsTrue(modelResponse.Patients.Count == 2);
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void ImportUploadErrorTest()
        {
            SetupFile("../../../ErrorRequest1.xlsx");
            var response = _controller.Upload(_file.Object).Result;
            Assert.IsInstanceOfType(response, typeof(IActionResult));
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)response).Model, typeof(OmnicellCabinetViewModel));
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState.Keys.Count() == 2);
            Assert.IsTrue(_controller.ModelState["patient"].Errors.Count == 1);
            Assert.IsTrue(_controller.ModelState["patient"].Errors.Select(e => e.ErrorMessage).First() == "This doesn't appear to be a patient worksheet");
            Assert.IsTrue(_controller.ModelState["medication-order"].Errors.Count == 6);
        }
        private FileStream ReadTestData(string path)
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException($"The file at {path} could not be found. Current working directory {Directory.GetCurrentDirectory()}");
            }
            return File.OpenRead(path);
        }
        private IEnumerable<Patient> GetTestPatients()
        {
            var result = new List<Patient>()
            {
                new Patient()
                {
                    PatientId = "1",
                    MRN = "1",
                    FullName = "Luke Skywalker",
                    First = "Luke",
                    Last = "Skywalker",
                    DOB = DateTime.Now.Date
                },
                new Patient()
                {
                    PatientId = "2",
                    MRN = "2",
                    FullName = "Hans Solo",
                    First = "Hans",
                    Last = "Solo",
                    DOB = DateTime.Now.Date
                },
                new Patient()
                {
                    PatientId = "3",
                    MRN = "3",
                    FullName = "Leia Organa",
                    First = "Leia",
                    Last = "Organa",
                    DOB = DateTime.Now.Date
                },
                new Patient()
                {
                    PatientId = "4",
                    MRN = "4",
                    FullName = "Obi-Wan Kenobi",
                    First = "Obi-Wan",
                    Last = "Kenobi",
                    DOB = DateTime.Now.Date
                },
                new Patient()
                {
                    PatientId = "5",
                    MRN = "5",
                    FullName = "Padmé Amidala",
                    First = "Padmé",
                    Last = "Amidala",
                    DOB = DateTime.Now.Date
                }
            };
            return result;
        }
    }
}
