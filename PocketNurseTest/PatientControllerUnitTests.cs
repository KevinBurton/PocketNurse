using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PocketNurse.Controllers;
using PocketNurse.Models;
using PocketNurse.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace PocketNurseTest
{
    [TestClass]
    public class PatientControllerUnitTests
    {
        Mock<IPocketNurseRepository> _repository;
        PatientController _controller;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IPocketNurseRepository>();
            _repository.Setup(m => m.GetAllPatients()).Returns(GetTestPatients());
            _repository.Setup(m => m.FindPatient(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult<Patient>(GetTestPatients().First()));
            _controller = new PatientController(_repository.Object);
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void PatientIndexTest()
        {
            var response = _controller.Index();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            Assert.IsInstanceOfType((response as ViewResult).Model, typeof(IEnumerable<Patient>));
            var list = (response as ViewResult).Model as IEnumerable<Patient>;
            Assert.IsTrue(list.Count() == 5);
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void DetailsTest()
        {
            var response = _controller.Details("1", 1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            Assert.IsInstanceOfType((response as ViewResult).Model, typeof(Patient));
            var patient = (response as ViewResult).Model as Patient;
            Assert.IsTrue(patient.FullName == GetTestPatients().First().FullName);
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void CreateTest()
        {
            var response = _controller.Create();
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            response = _controller.Create(GetTestPatients().First());
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(RedirectToActionResult));
            _repository.Verify(mock => mock.Add(It.IsAny<Patient>()), Times.Once());
            _repository.Verify(mock => mock.Save(), Times.Once());
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void EditTest()
        {
            var response = _controller.Edit("1", 1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            _repository.Verify(mock => mock.FindPatient(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            var patient = GetTestPatients().First();
            response = _controller.Edit(patient.PatientId, patient);
            _repository.Verify(mock => mock.Update(It.IsAny<Patient>()), Times.Once());
            _repository.Verify(mock => mock.Save(), Times.Once());
            Assert.IsInstanceOfType(response, typeof(RedirectToActionResult));
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void DeleteTest()
        {
            var response = _controller.Delete("1", 1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            _repository.Verify(mock => mock.FindPatient(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }
        [TestMethod]
        [Owner("Kevin Burton")]
        public void DeleteConfirmedTest()
        {
            var response = _controller.DeleteConfirmed("1", 1);
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(RedirectToActionResult));
            _repository.Verify(mock => mock.Delete(It.IsAny<Patient>()), Times.Once());
            _repository.Verify(mock => mock.Save(), Times.Once());
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
