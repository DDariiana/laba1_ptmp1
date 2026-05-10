using System;
using System.Windows.Forms;
using System.Drawing;

namespace ReservationApp
{
    public class ReservationForm : Form
    {
        private ReservationManager reservationManager;
        private TextBox customerNameTextBox;
        private DateTimePicker startTimePicker;
        private DateTimePicker endTimePicker;
        private ComboBox statusComboBox;
        private Button addReservationButton;
        private Button removeReservationButton;
        private Button updateStatusButton;
        private Button checkAvailabilityButton;
        private ListBox reservationsListBox;
        private ListBox availableSlotsListBox;

        public ReservationForm()
        {
            this.Text = "Управление резервированием";
            this.Width = 850;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ВЕРХНЯЯ ПАНЕЛЬ: ВВОД ДАННЫХ

            var nameLabel = new Label
            {
                Location = new Point(20, 20),
                Text = "Имя клиента:",
                AutoSize = true
            };

            customerNameTextBox = new TextBox
            {
                Location = new Point(110, 17),
                Width = 180,
                Name = "customerNameTextBox"
            };

            var startLabel = new Label
            {
                Location = new Point(320, 20),
                Text = "Дата начала:",
                AutoSize = true
            };

            startTimePicker = new DateTimePicker
            {
                Location = new Point(410, 17),
                Width = 140,
                Format = DateTimePickerFormat.Long,
                ShowUpDown = false,
                Name = "startTimePicker"
            };

            var endLabel = new Label
            {
                Location = new Point(570, 20),
                Text = "Дата окончания:",
                AutoSize = true
            };

            endTimePicker = new DateTimePicker
            {
                Location = new Point(690, 17),
                Width = 140,
                Format = DateTimePickerFormat.Long,
                ShowUpDown = false,
                Name = "endTimePicker"
            };

            var statusLabel = new Label
            {
                Location = new Point(20, 55),
                Text = "Статус:",
                AutoSize = true
            };

            statusComboBox = new ComboBox
            {
                Location = new Point(80, 52),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "statusComboBox"
            };
            statusComboBox.Items.AddRange(new object[] { "Активно", "Отменено", "Завершено" });
            statusComboBox.SelectedIndex = 0;

            // ПАНЕЛЬ КНОПОК
            var buttonPanel = new Panel
            {
                Location = new Point(20, 90),
                Width = 800,
                Height = 45,
                Name = "buttonPanel"
            };

            addReservationButton = new Button
            {
                Location = new Point(10, 10),
                Text = "Добавить",
                Width = 110,
                Name = "addReservationButton"
            };
            addReservationButton.Click += AddReservationButton_Click;

            removeReservationButton = new Button
            {
                Location = new Point(130, 10),
                Text = "Удалить",
                Width = 110,
                Name = "removeReservationButton"
            };
            removeReservationButton.Click += RemoveReservationButton_Click;

            updateStatusButton = new Button
            {
                Location = new Point(250, 10),
                Text = "Обновить статус",
                Width = 130,
                Name = "updateStatusButton"
            };
            updateStatusButton.Click += UpdateStatusButton_Click;

            checkAvailabilityButton = new Button
            {
                Location = new Point(390, 10),
                Text = "Проверить доступность",
                Width = 160,
                Name = "checkAvailabilityButton"
            };
            checkAvailabilityButton.Click += CheckAvailabilityButton_Click;

            buttonPanel.Controls.AddRange(new Control[] {
                addReservationButton,
                removeReservationButton,
                updateStatusButton,
                checkAvailabilityButton
            });

            // СПИСОК РЕЗЕРВОВ
            var resLabel = new Label
            {
                Location = new Point(20, 150),
                Text = "Список резервов:",
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                AutoSize = true
            };

            reservationsListBox = new ListBox
            {
                Location = new Point(20, 175),
                Width = 800,
                Height = 180,
                Name = "reservationsListBox"
            };

            // СПИСОК СВОБОДНЫХ СЛОТОВ
            var slotsLabel = new Label
            {
                Location = new Point(20, 370),
                Text = "Свободные даты:",
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                AutoSize = true
            };

            availableSlotsListBox = new ListBox
            {
                Location = new Point(20, 395),
                Width = 800,
                Height = 180,
                Name = "availableSlotsListBox"
            };

            // ДОБАВЛЕНИЕ ЭЛЕМЕНТОВ НА ФОРМУ
            this.Controls.AddRange(new Control[] {
                nameLabel,
                customerNameTextBox,
                startLabel,
                startTimePicker,
                endLabel,
                endTimePicker,
                statusLabel,
                statusComboBox,
                buttonPanel,
                resLabel,
                reservationsListBox,
                slotsLabel,
                availableSlotsListBox
            });

            reservationManager = new ReservationManager();
            UpdateReservationsList();
        }

        private void UpdateReservationsList()
        {
            reservationsListBox.Items.Clear();
            foreach (var res in reservationManager.Reservations)
            {
                reservationsListBox.Items.Add(res);
            }
        }

        // ОБРАБОТЧИКИ СОБЫТИЙ
        private void RefreshAvailableDates()
        {
            try
            {
                DateTime checkDate = startTimePicker.Value.Date;
                var availableDates = reservationManager.GetAvailableSlots(checkDate);

                availableSlotsListBox.Items.Clear();

                if (availableDates.Count == 0)
                {
                    availableSlotsListBox.Items.Add("Нет свободных дат.");
                }
                else
                {
                    foreach (var date in availableDates)
                    {
                        availableSlotsListBox.Items.Add(date);
                    }
                }
            }
            catch (Exception){}
        }
        private void AddReservationButton_Click(object sender, EventArgs e)
        {
            try
            {
                var res = new Reservation(customerNameTextBox.Text, startTimePicker.Value, endTimePicker.Value);
                reservationManager.AddReservation(res);

                UpdateReservationsList();
                RefreshAvailableDates();

                MessageBox.Show("Резервирование добавлено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveReservationButton_Click(object sender, EventArgs e)
        {
            if (reservationsListBox.SelectedItem is Reservation selected)
            {
                try
                {
                    reservationManager.RemoveReservationById(selected.Id);
                    UpdateReservationsList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateStatusButton_Click(object sender, EventArgs e)
        {
            if (reservationsListBox.SelectedItem is Reservation selected)
            {
                try
                {
                    var newStatus = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), statusComboBox.SelectedItem.ToString());
                    reservationManager.UpdateReservationStatusById(selected.Id, newStatus);
                    UpdateReservationsList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CheckAvailabilityButton_Click(object sender, EventArgs e)
        {
            RefreshAvailableDates();
        }
    }
}