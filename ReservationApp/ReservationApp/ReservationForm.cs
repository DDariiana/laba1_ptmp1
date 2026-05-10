using System;
using System.Windows.Forms;

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
        private ListBox reservationsListBox;

        public ReservationForm()
        {
            this.Text = "Управление резервированием";
            this.Width = 780;
            this.Height = 550;

            // Метки
            var nameLabel = new Label { Location = new System.Drawing.Point(10, 14), Text = "Имя клиента:", AutoSize = true };
            var startTimeLabel = new Label { Location = new System.Drawing.Point(255, 14), Text = "Дата начала:", AutoSize = true };
            var endTimeLabel = new Label { Location = new System.Drawing.Point(500, 14), Text = "Дата окончания:", AutoSize = true };
            var statusLabel = new Label { Location = new System.Drawing.Point(10, 49), Text = "Статус:", AutoSize = true };

            // Поля ввода
            customerNameTextBox = new TextBox { Location = new System.Drawing.Point(95, 10), Width = 150 };
            startTimePicker = new DateTimePicker { Location = new System.Drawing.Point(340, 10), Width = 150 };
            endTimePicker = new DateTimePicker { Location = new System.Drawing.Point(600, 10), Width = 150 };
            statusComboBox = new ComboBox { Location = new System.Drawing.Point(70, 45), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            statusComboBox.Items.AddRange(new object[] { "Активно", "Отменено", "Завершено" });
            statusComboBox.SelectedIndex = 0;

            // Кнопки
            addReservationButton = new Button { Location = new System.Drawing.Point(10, 78), Text = "Добавить", Width = 100 };
            addReservationButton.Click += AddReservationButton_Click;

            removeReservationButton = new Button { Location = new System.Drawing.Point(120, 78), Text = "Удалить", Width = 100 };
            removeReservationButton.Click += RemoveReservationButton_Click;

            updateStatusButton = new Button { Location = new System.Drawing.Point(230, 78), Text = "Обновить статус", Width = 130 };
            updateStatusButton.Click += UpdateStatusButton_Click;

            // Список резерваций
            reservationsListBox = new ListBox { Location = new System.Drawing.Point(10, 115), Width = 740, Height = 380 };

            // Добавление элементов на форму
            this.Controls.AddRange(new Control[] {
                nameLabel, customerNameTextBox, startTimeLabel, startTimePicker,
                endTimeLabel, endTimePicker, statusLabel, statusComboBox,
                addReservationButton, removeReservationButton, updateStatusButton,
                reservationsListBox
            });

            reservationManager = new ReservationManager();
            UpdateReservationsList();
        }

        private void UpdateReservationsList()
        {
            reservationsListBox.Items.Clear();
            foreach (var reservation in reservationManager.Reservations)
            {
                reservationsListBox.Items.Add(reservation); // Добавляем объект, а не строку
            }
            reservationsListBox.DisplayMember = "ToString";
        }

        private void AddReservationButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(customerNameTextBox.Text))
            {
                MessageBox.Show("Введите имя клиента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newReservation = new Reservation(
                    customerNameTextBox.Text,
                    startTimePicker.Value,
                    endTimePicker.Value);

                reservationManager.AddReservation(newReservation);
                customerNameTextBox.Clear();
                UpdateReservationsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveReservationButton_Click(object sender, EventArgs e)
        {
            if (reservationsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите резервирование для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var reservation = reservationsListBox.SelectedItem as Reservation;
            if (reservation == null)
            {
                MessageBox.Show("Не удалось найти выбранное резервирование.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                reservationManager.RemoveReservationById(reservation.Id);
                UpdateReservationsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateStatusButton_Click(object sender, EventArgs e)
        {
            if (reservationsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите резервирование для обновления статуса!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var reservation = reservationsListBox.SelectedItem as Reservation;
            if (reservation == null)
            {
                MessageBox.Show("Не удалось найти выбранное резервирование.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var newStatus = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), statusComboBox.SelectedItem.ToString());
                reservationManager.UpdateReservationStatusById(reservation.Id, newStatus);
                UpdateReservationsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}