using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ReservationApp;

namespace ReservationApp.Tests
{
    [TestClass]
    public class ReservationManagerTests
    {
        private ReservationManager _manager;

        [TestInitialize]
        public void SetUp()
        {
            _manager = new ReservationManager();
        }

        [TestCleanup]
        public void TearDown()
        {
            _manager = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReservation_WhenTimeOverlaps_ThrowsException()
        {
            // Arrange
            DateTime baseTime = DateTime.Now;
            var reservation1 = new Reservation("Иван", baseTime, baseTime.AddHours(2));
            var reservation2 = new Reservation("Петр", baseTime.AddHours(1), baseTime.AddHours(3));

            _manager.AddReservation(reservation1);

            // Act
            _manager.AddReservation(reservation2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveReservation_WhenNotExists_ThrowsException()
        {
            // Arrange
            var reservation = new Reservation("Иван", DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));

            // Act
            _manager.RemoveReservation(reservation);
        }

        [TestMethod]
        public void UpdateReservationStatus_WhenValid_UpdatesSuccessfully()
        {
            // Arrange
            var reservation = new Reservation("Иван", DateTime.Now.AddHours(40), DateTime.Now.AddHours(41));
            _manager.AddReservation(reservation);

            // Act
            _manager.UpdateReservationStatus(reservation, ReservationStatus.Отменено);

            // Assert
            Assert.AreEqual(ReservationStatus.Отменено, reservation.Status);
        }
    }
}