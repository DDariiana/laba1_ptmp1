using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ReservationApp;

namespace ReservationApp.Tests
{
    [TestClass]
    public class ReservationTests
    {
        [TestMethod]
        public void Constructor_WhenValidData_CreatesReservation()
        {
            // Arrange
            string customerName = "Иван Иванов";
            DateTime start = new DateTime(2026, 5, 7, 10, 0, 0);
            DateTime end = new DateTime(2026, 5, 7, 12, 0, 0);

            // Act
            var reservation = new Reservation(customerName, start, end);

            // Assert
            Assert.AreEqual(customerName, reservation.CustomerName);
            Assert.AreEqual(start, reservation.StartTime);
            Assert.AreEqual(end, reservation.EndTime);
            Assert.AreEqual(ReservationStatus.Активно, reservation.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WhenNameIsEmpty_ThrowsException()
        {
            new Reservation("", DateTime.Now, DateTime.Now.AddHours(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WhenNameIsNull_ThrowsException()
        {
            new Reservation(null, DateTime.Now, DateTime.Now.AddHours(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WhenStartTimeGreaterOrEqualEndTime_ThrowsException()
        {
            DateTime time = DateTime.Now;
            new Reservation("Клиент", time, time);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WhenStartTimeAfterEndTime_ThrowsException()
        {
            DateTime start = DateTime.Now.AddHours(2);
            DateTime end = DateTime.Now;
            new Reservation("Клиент", start, end);
        }

        [TestMethod]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var reservation = new Reservation("Иван",
                new DateTime(2026, 5, 7, 10, 0, 0),
                new DateTime(2026, 5, 7, 11, 0, 0));

            // Act
            string result = reservation.ToString();

            // Assert - проверяем то, что реально есть в строке
            StringAssert.Contains(result, "Иван");
            StringAssert.Contains(result, "2026");
            StringAssert.Contains(result, "10:00");
        }

        [TestMethod]
        public void IsActive_WhenStatusActive_ReturnsTrue()
        {
            // Arrange
            var reservation = new Reservation("Иван", DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));

            // Act & Assert
            Assert.IsTrue(reservation.IsActive());
        }

        [TestMethod]
        public void IsActive_WhenStatusCancelled_ReturnsFalse()
        {
            // Arrange
            var reservation = new Reservation("Иван", DateTime.Now.AddHours(1), DateTime.Now.AddHours(2))
            {
                Status = ReservationStatus.Отменено
            };

            // Act & Assert
            Assert.IsFalse(reservation.IsActive());
        }

        [TestMethod]
        public void GetDuration_ReturnsCorrectHours()
        {
            // Arrange
            DateTime start = new DateTime(2026, 5, 7, 10, 0, 0);
            DateTime end = new DateTime(2026, 5, 7, 12, 30, 0);
            var reservation = new Reservation("Иван", start, end);

            // Act
            TimeSpan duration = reservation.GetDuration();

            // Assert
            Assert.AreEqual(2.5, duration.TotalHours, 0.01);
        }
    }
}