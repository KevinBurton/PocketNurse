using Microsoft.AspNetCore.Http;
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
        Mock<IPatientRepository> _patientRepository;
        Mock<IPatientAllergyRepository> _patientAllergyRepository;
        Mock<IMedicationOrderRepository> _medicationOrderRepository;
        Mock<INotInFormularyRepository> _notInFormularyRepository;
        ImportController _controller;
        Mock<IFormFile> _file;
        long _fileSize;
        [TestInitialize]
        public void Setup()
        {
            _patientRepository = new Mock<IPatientRepository>();
            _patientRepository.Setup(m => m.GetAll()).Returns(GetTestPatients());
            _patientRepository.Setup(m => m.Find(It.IsAny<string>())).Returns(GetTestPatients().First);
            _patientAllergyRepository = new Mock<IPatientAllergyRepository>();
            _medicationOrderRepository = new Mock<IMedicationOrderRepository>();
            _notInFormularyRepository = new Mock<INotInFormularyRepository>();
            _controller = new ImportController(_patientRepository.Object,
                                               _patientAllergyRepository.Object,
                                               _medicationOrderRepository.Object,
                                               _notInFormularyRepository.Object);
            var filePath = "../../../SampleRequest.xlsx";
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
        public void ImportIndexTest()
        {
            var response = _controller.Upload(_file.Object);
            Assert.IsInstanceOfType(response, typeof(IActionResult));
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            var valueResponse = (OkObjectResult)response;
            Debug.WriteLine($"Size {valueResponse.Value.GetType().GetProperty("size").GetValue(valueResponse.Value, null)} Count {valueResponse.Value.GetType().GetProperty("wsCount").GetValue(valueResponse.Value, null)} Worksheets {valueResponse.Value.GetType().GetProperty("worksheets").GetValue(valueResponse.Value, null)}");
            Assert.IsTrue((long)valueResponse.Value.GetType().GetProperty("size").GetValue(valueResponse.Value, null) == _fileSize);
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
