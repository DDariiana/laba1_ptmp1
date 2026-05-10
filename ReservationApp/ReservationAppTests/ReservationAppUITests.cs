using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
/*using System.Windows.Forms; */// 🔥 Добавлено для SendKeys

namespace ReservationApp.Tests
{
    [TestClass]
    public class ReservationAppUITests
    {
        private FlaUI.Core.Application _app;
        private UIA3Automation _automation;
        private FlaUI.Core.AutomationElements.Window _mainWindow;

        private readonly string _reservationsFilePath = Path.Combine(
            Environment.CurrentDirectory,
            "reservations.txt");

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(_reservationsFilePath))
                File.Delete(_reservationsFilePath);

            _app = FlaUI.Core.Application.Launch(@"D:\Новая папка\ReservationApp\ReservationApp\bin\Debug\ReservationApp.exe");
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);

            Thread.Sleep(1000);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                _app?.Close();
                Thread.Sleep(500);

                if (File.Exists(_reservationsFilePath))
                    File.Delete(_reservationsFilePath);
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                try { if (File.Exists(_reservationsFilePath)) File.Delete(_reservationsFilePath); }
                catch { }
            }
            catch { }
            finally
            {
                _automation?.Dispose();
                _app = null;
            }
        }

        #region Вспомогательные методы

        private TextBox FindTextBox(string automationId) =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsTextBox()
            ?? throw new Exception($"TextBox '{automationId}' не найден");

        private ComboBox FindComboBox(string automationId) =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsComboBox()
            ?? throw new Exception($"ComboBox '{automationId}' не найден");

        private Button FindButton(string automationId) =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
            ?? throw new Exception($"Button '{automationId}' не найден");

        private ListBox FindListBox(string automationId) =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsListBox()
            ?? throw new Exception($"ListBox '{automationId}' не найден");

        private DateTimePicker FindDatePicker(string automationId) =>
            _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsDateTimePicker()
            ?? throw new Exception($"DateTimePicker '{automationId}' не найден");

        private void SetDatePickerValue(DateTimePicker picker, DateTime dateTime)
        {
            picker.Focus();
            Thread.Sleep(300);

            System.Windows.Forms.SendKeys.SendWait("^a");

            Thread.Sleep(100);

            System.Windows.Forms.SendKeys.SendWait(dateTime.ToString("dd.MM.yyyy"));
            Thread.Sleep(200);

            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            Thread.Sleep(300);
        }

        private void SelectComboBoxItem(ComboBox comboBox, string itemText)
        {
            if (comboBox.Patterns.Value.IsSupported)
                comboBox.Patterns.Value.Pattern.SetValue(itemText);
            Thread.Sleep(300);
        }

        private void CloseMessageBox()
        {
            var modal = _mainWindow.ModalWindows.FirstOrDefault();
            if (modal != null)
            {
                var okButton = modal.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button));
                okButton?.Click();
                Thread.Sleep(300);
            }
        }

        private void ClearReservationsList()
        {
            try
            {
                var listBox = FindListBox("reservationsListBox");
                var removeButton = FindButton("removeReservationButton");

                while (listBox.Items.Length > 0)
                {
                    listBox.Items[0].Click();
                    Thread.Sleep(200);
                    removeButton.Click();
                    Thread.Sleep(500);
                    CloseMessageBox();
                }
            }
            catch { }
        }

        private void AddTestReservation(string name, DateTime startTime, DateTime endTime, string status = "Активно")
        {
            var nameBox = FindTextBox("customerNameTextBox");
            var startPicker = FindDatePicker("startTimePicker");
            var endPicker = FindDatePicker("endTimePicker");
            var statusBox = FindComboBox("statusComboBox");
            var addButton = FindButton("addReservationButton");

            nameBox.Text = name;
            SetDatePickerValue(startPicker, startTime);
            SetDatePickerValue(endPicker, endTime);
            SelectComboBoxItem(statusBox, status);

            addButton.Click();
            Thread.Sleep(500);
            CloseMessageBox();
            Thread.Sleep(200);
        }

        #endregion

        #region TC-01: Добавление резервирования с корректными данными
        [TestMethod]
        public void TC01_AddReservation_ValidData()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0), "Активно");
            var listBox = FindListBox("reservationsListBox");
            Assert.AreEqual(1, listBox.Items.Length);
            StringAssert.Contains(listBox.Items[0].Text, "Иван");
            StringAssert.Contains(listBox.Items[0].Text, "Активно");
        }
        #endregion

        #region TC-02: Добавление с пустым именем
        [TestMethod]
        public void TC02_AddReservation_EmptyCustomerName()
        {
            FindTextBox("customerNameTextBox").Text = "";
            SetDatePickerValue(FindDatePicker("startTimePicker"), new DateTime(2026, 6, 7, 10, 0, 0));
            SetDatePickerValue(FindDatePicker("endTimePicker"), new DateTime(2026, 6, 10, 12, 0, 0));
            SelectComboBoxItem(FindComboBox("statusComboBox"), "Активно");
            FindButton("addReservationButton").Click();
            Thread.Sleep(500); CloseMessageBox();
            Assert.AreEqual(0, FindListBox("reservationsListBox").Items.Length);
        }
        #endregion

        #region TC-03: Добавление без выбора статуса
        [TestMethod]
        public void TC03_AddReservation_WithoutStatusSelection()
        {
            FindTextBox("customerNameTextBox").Text = "Иван";
            SetDatePickerValue(FindDatePicker("startTimePicker"), new DateTime(2026, 6, 7, 16, 6, 0));
            SetDatePickerValue(FindDatePicker("endTimePicker"), new DateTime(2026, 6, 10, 16, 6, 0));
            FindButton("addReservationButton").Click();
            Thread.Sleep(500); CloseMessageBox();
            var listBox = FindListBox("reservationsListBox");
            Assert.AreEqual(1, listBox.Items.Length);
            StringAssert.Contains(listBox.Items[0].Text, "Активно");
        }
        #endregion

        #region TC-04: Удаление резервирования
        [TestMethod]
        public void TC04_DeleteReservation()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));
            var listBox = FindListBox("reservationsListBox");
            listBox.Items[0].Click(); Thread.Sleep(200);
            FindButton("removeReservationButton").Click(); Thread.Sleep(500); CloseMessageBox();
            Assert.AreEqual(0, listBox.Items.Length);
        }
        #endregion

        #region TC-05: Обновление статуса
        [TestMethod]
        public void TC05_UpdateReservationStatus()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));
            var listBox = FindListBox("reservationsListBox");
            listBox.Items[0].Click(); Thread.Sleep(200);
            SelectComboBoxItem(FindComboBox("statusComboBox"), "Отменено");
            FindButton("updateStatusButton").Click(); Thread.Sleep(500); CloseMessageBox();
            StringAssert.Contains(listBox.Items[0].Text, "Отменено");
        }
        #endregion

        #region TC-06: Удаление без выбора
        [TestMethod]
        public void TC06_DeleteWithoutSelection()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));
            FindButton("removeReservationButton").Click(); Thread.Sleep(500); CloseMessageBox();
            Assert.AreEqual(1, FindListBox("reservationsListBox").Items.Length);
        }
        #endregion

        #region TC-07: Обновление статуса без выбора
        [TestMethod]
        public void TC07_UpdateStatusWithoutSelection()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));
            SelectComboBoxItem(FindComboBox("statusComboBox"), "Отменено");
            FindButton("updateStatusButton").Click(); Thread.Sleep(500); CloseMessageBox();
            StringAssert.Contains(FindListBox("reservationsListBox").Items[0].Text, "Активно");
        }
        #endregion

        #region TC-08: Некорректные даты (начало позже окончания)
        [TestMethod]
        public void TC08_AddReservation_InvalidDates()
        {
            var nameBox = FindTextBox("customerNameTextBox");
            var startPicker = FindDatePicker("startTimePicker");
            var endPicker = FindDatePicker("endTimePicker");
            var statusBox = FindComboBox("statusComboBox");
            var addButton = FindButton("addReservationButton");

            nameBox.Text = "Иван";

            // 🔥 Начало ПОЗЖЕ окончания
            SetDatePickerValue(startPicker, new DateTime(2026, 6, 10, 10, 0, 0));
            SetDatePickerValue(endPicker, new DateTime(2026, 6, 7, 12, 0, 0));

            SelectComboBoxItem(statusBox, "Активно");
            Thread.Sleep(300);
            addButton.Click();
            Thread.Sleep(1000);

            CloseMessageBox();
            Thread.Sleep(200);

            var reservationsListBox = FindListBox("reservationsListBox");
            Assert.AreEqual(0, reservationsListBox.Items.Length, "Запись с некорректными датами не должна добавиться");
        }
        #endregion

        #region TC-09: Пересекающиеся даты
        [TestMethod]
        public void TC09_AddReservation_OverlappingDates()
        {
            AddTestReservation("Иван", new DateTime(2026, 6, 7, 10, 0, 0), new DateTime(2026, 6, 10, 12, 0, 0));

            var nameBox = FindTextBox("customerNameTextBox");
            nameBox.Text = "Артем";
            SetDatePickerValue(FindDatePicker("startTimePicker"), new DateTime(2026, 6, 7, 9, 0, 0));
            SetDatePickerValue(FindDatePicker("endTimePicker"), new DateTime(2026, 6, 10, 13, 0, 0));
            SelectComboBoxItem(FindComboBox("statusComboBox"), "Активно");
            FindButton("addReservationButton").Click();
            Thread.Sleep(500); CloseMessageBox();

            Assert.AreEqual(1, FindListBox("reservationsListBox").Items.Length);
        }
        #endregion

        #region TC-10: Просмотр свободных дат
        [TestMethod]
        public void TC10_CheckAvailableDates()
        {
            var checkButton = FindButton("checkAvailabilityButton");
            var availableSlotsList = FindListBox("availableSlotsListBox");
            var reservationsList = FindListBox("reservationsListBox");

            checkButton.Click(); Thread.Sleep(500);
            int datesBefore = availableSlotsList.Items.Length;
            Assert.IsTrue(datesBefore > 0);

            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));
            Assert.AreEqual(1, reservationsList.Items.Length);

            checkButton.Click(); Thread.Sleep(500);
            int datesAfter = availableSlotsList.Items.Length;
            Assert.IsTrue(datesAfter < datesBefore);
        }
        #endregion

        #region TC-11: Свободные даты после удаления
        [TestMethod]
        public void TC11_CheckAvailableDatesAfterDelete()
        {
            var checkButton = FindButton("checkAvailabilityButton");
            var availableSlotsList = FindListBox("availableSlotsListBox");
            var reservationsList = FindListBox("reservationsListBox");
            var removeButton = FindButton("removeReservationButton");

            AddTestReservation("Иван", new DateTime(2026, 6, 7, 16, 6, 0), new DateTime(2026, 6, 10, 16, 6, 0));

            checkButton.Click(); Thread.Sleep(500);
            int datesBeforeDelete = availableSlotsList.Items.Length;

            reservationsList.Items[0].Click(); Thread.Sleep(200);
            removeButton.Click(); Thread.Sleep(500); CloseMessageBox();

            checkButton.Click(); Thread.Sleep(500);
            int datesAfterDelete = availableSlotsList.Items.Length;

            Assert.AreEqual(0, reservationsList.Items.Length);
            Assert.IsTrue(datesAfterDelete > datesBeforeDelete);
        }
        #endregion
    }
}