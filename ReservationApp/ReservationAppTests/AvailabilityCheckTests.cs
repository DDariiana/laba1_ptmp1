using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ReservationApp;

namespace ReservationApp.Tests
{
    [TestClass]
    public class AvailabilityCheckTests
    {
        private ReservationManager _manager;

        [TestInitialize]
        public void SetUp()
        {
            _manager = new ReservationManager();
            _manager.Reservations.Clear();
        }

        [TestCleanup]
        public void TearDown()
        {
            _manager = null;
        }

        #region Тесты для метода GetAvailableSlots

        [TestMethod]
        public void GetAvailableSlots_WhenNoReservations_ReturnsAllDates()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);
            int daysToCheck = 30;

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, daysToCheck);

            // Assert
            Assert.IsNotNull(availableDates);
            Assert.AreEqual(daysToCheck, availableDates.Count);
        }

        [TestMethod]
        public void GetAvailableSlots_WithExistingReservations_ExcludesOccupiedDates()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);
            var reservation = new Reservation("Иван",
                new DateTime(2026, 5, 5, 10, 0, 0),
                new DateTime(2026, 5, 7, 12, 0, 0));
            _manager.Reservations.Add(reservation);

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, 10);

            // Assert
            Assert.IsNotNull(availableDates);
            // Проверяем, что занятые даты исключены
            foreach (var date in availableDates)
            {
                Assert.IsFalse(date.Contains("05.05.2026"), "Дата 05.05.2026 не должна быть в списке");
                Assert.IsFalse(date.Contains("06.05.2026"), "Дата 06.05.2026 не должна быть в списке");
                Assert.IsFalse(date.Contains("07.05.2026"), "Дата 07.05.2026 не должна быть в списке");
            }
        }

        [TestMethod]
        public void GetAvailableSlots_OnlyExcludesActiveReservations()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);
            var cancelledReservation = new Reservation("Иван",
                new DateTime(2026, 5, 5, 10, 0, 0),
                new DateTime(2026, 5, 5, 12, 0, 0))
            {
                Status = ReservationStatus.Отменено
            };
            _manager.Reservations.Add(cancelledReservation);

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, 10);

            // Assert
            Assert.IsNotNull(availableDates);
            // Отмененное резервирование не должно блокировать дату
            bool foundMay5 = false;
            foreach (var date in availableDates)
            {
                if (date.Contains("05.05.2026"))
                {
                    foundMay5 = true;
                    break;
                }
            }
            Assert.IsTrue(foundMay5, "Дата 05.05.2026 должна быть доступна (резерв отменен)");
        }

        [TestMethod]
        public void GetAvailableSlots_ReturnsCorrectDateFormat()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, 5);

            // Assert
            Assert.IsNotNull(availableDates);
            Assert.IsTrue(availableDates.Count > 0);
            // Проверяем формат: "dd.MM.yyyy (dddd)"
            StringAssert.Contains(availableDates[0], ".");
            StringAssert.Contains(availableDates[0], "(");
            StringAssert.Contains(availableDates[0], ")");
        }

        #endregion

        #region Тесты для метода IsTimeAvailable

        [TestMethod]
        public void IsTimeAvailable_WhenNoReservations_ReturnsTrue()
        {
            // Arrange
            DateTime start = new DateTime(2026, 5, 10, 10, 0, 0);
            DateTime end = new DateTime(2026, 5, 10, 12, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(start, end);

            // Assert
            Assert.IsTrue(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_WhenTimeIsOccupied_ReturnsFalse()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 11, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 13, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsFalse(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_WhenTimeIsFree_ReturnsTrue()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 14, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 16, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsTrue(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_DoesNotCheckCancelledReservations()
        {
            // Arrange
            var cancelledReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0))
            {
                Status = ReservationStatus.Отменено
            };
            _manager.Reservations.Add(cancelledReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 10, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 11, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsTrue(isAvailable, "Отмененное резервирование не должно блокировать время");
        }

        [TestMethod]
        public void IsTimeAvailable_DetectsExactOverlap()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 10, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 12, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsFalse(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_DetectsPartialOverlap_Start()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 9, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 11, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsFalse(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_DetectsPartialOverlap_End()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 11, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 13, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsFalse(isAvailable);
        }

        [TestMethod]
        public void IsTimeAvailable_AllowsAdjacentTime()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            DateTime newStart = new DateTime(2026, 5, 10, 12, 0, 0);
            DateTime newEnd = new DateTime(2026, 5, 10, 14, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(newStart, newEnd);

            // Assert
            Assert.IsTrue(isAvailable, "Смежное время должно быть доступно");
        }

        #endregion

        #region Тесты для интеграции с AddReservation

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReservation_WhenTimeIsOccupied_ThrowsException()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            var newReservation = new Reservation("Петр",
                new DateTime(2026, 5, 10, 11, 0, 0),
                new DateTime(2026, 5, 10, 13, 0, 0));

            // Act
            _manager.AddReservation(newReservation);
        }

        [TestMethod]
        public void AddReservation_WhenTimeIsFree_AddsSuccessfully()
        {
            // Arrange
            var existingReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 10, 0, 0),
                new DateTime(2026, 5, 10, 12, 0, 0));
            _manager.Reservations.Add(existingReservation);

            var newReservation = new Reservation("Петр",
                new DateTime(2026, 5, 10, 14, 0, 0),
                new DateTime(2026, 5, 10, 16, 0, 0));

            // Act
            _manager.AddReservation(newReservation);

            // Assert
            Assert.AreEqual(2, _manager.Reservations.Count);
        }

        #endregion

        #region Тесты граничных значений

        [TestMethod]
        public void GetAvailableSlots_ChecksExactlySpecifiedDays()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);
            int daysToCheck = 7;

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, daysToCheck);

            // Assert
            Assert.AreEqual(daysToCheck, availableDates.Count);
            // Проверяем, что последняя дата - 7 мая
            Assert.IsTrue(availableDates[availableDates.Count - 1].Contains("07.05.2026"));
        }

        [TestMethod]
        public void GetAvailableSlots_HandlesMonthBoundary()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 25);
            int daysToCheck = 10;

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, daysToCheck);

            // Assert
            Assert.IsNotNull(availableDates);
            Assert.AreEqual(daysToCheck, availableDates.Count);
            // Проверяем, что даты июня тоже есть
            bool hasJuneDate = false;
            foreach (var date in availableDates)
            {
                if (date.Contains("06."))
                {
                    hasJuneDate = true;
                    break;
                }
            }
            Assert.IsTrue(hasJuneDate, "Должны быть даты июня при переходе через границу месяца");
        }

        [TestMethod]
        public void IsTimeAvailable_HandlesSameDayDifferentTimes()
        {
            // Arrange
            var morningReservation = new Reservation("Иван",
                new DateTime(2026, 5, 10, 9, 0, 0),
                new DateTime(2026, 5, 10, 11, 0, 0));
            _manager.Reservations.Add(morningReservation);

            DateTime eveningStart = new DateTime(2026, 5, 10, 18, 0, 0);
            DateTime eveningEnd = new DateTime(2026, 5, 10, 20, 0, 0);

            // Act
            bool isAvailable = _manager.IsTimeAvailable(eveningStart, eveningEnd);

            // Assert
            Assert.IsTrue(isAvailable, "Вечернее время должно быть свободно");
        }

        [TestMethod]
        public void GetAvailableSlots_ExcludesMultiDayReservation()
        {
            // Arrange
            DateTime startDate = new DateTime(2026, 5, 1);
            var multiDayReservation = new Reservation("Иван",
                new DateTime(2026, 5, 5, 10, 0, 0),
                new DateTime(2026, 5, 8, 12, 0, 0));
            _manager.Reservations.Add(multiDayReservation);

            // Act
            var availableDates = _manager.GetAvailableSlots(startDate, 10);

            // Assert
            Assert.IsNotNull(availableDates);
            // Проверяем, что все дни резервации исключены
            foreach (var date in availableDates)
            {
                Assert.IsFalse(date.Contains("05.05.2026"), "Дата 05.05.2026 не должна быть доступна");
                Assert.IsFalse(date.Contains("06.05.2026"), "Дата 06.05.2026 не должна быть доступна");
                Assert.IsFalse(date.Contains("07.05.2026"), "Дата 07.05.2026 не должна быть доступна");
                Assert.IsFalse(date.Contains("08.05.2026"), "Дата 08.05.2026 не должна быть доступна");
            }
        }

        #endregion
    }
}