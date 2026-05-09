using System;
using System.Windows.Forms;
using System.Drawing;

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
        this.Width = 600;
        this.Height = 500;
        customerNameTextBox = new TextBox
        {
            Location = new System.Drawing.Point(10, 10),
            Width = 150,
            Text = "Имя клиента"
        };
        startTimePicker = new DateTimePicker
        {
            Location = new System.Drawing.Point(170, 10),
            Width = 150
        };
        endTimePicker = new DateTimePicker
        {
            Location = new System.Drawing.Point(330, 10),
            Width = 150
        };
        statusComboBox = new ComboBox
        {
            Location = new System.Drawing.Point(10, 40),
            Width = 100,
            Text = "Статус",
            Items = { "Активно", "Отменено", "Завершено" }
        };
        addReservationButton = new Button
        {
            Location = new System.Drawing.Point(10, 70),
            Text = "Добавить",
            Width = 100
        };
        addReservationButton.Click += AddReservationButton_Click;
        removeReservationButton = new Button
        {
            Location = new System.Drawing.Point(120, 70),
            Text = "Удалить",
            Width = 100
        };
        removeReservationButton.Click += RemoveReservationButton_Click;
        updateStatusButton = new Button
        {
            Location = new System.Drawing.Point(220, 70),
            Text = "Обновить статус",
            Width = 120
        };
        updateStatusButton.Click += UpdateStatusButton_Click;
        reservationsListBox = new ListBox
        {
            Location = new System.Drawing.Point(10, 100),
            Width = 560,
            Height = 250
        };
        this.Controls.Add(customerNameTextBox);
        this.Controls.Add(startTimePicker);
        this.Controls.Add(endTimePicker);
        this.Controls.Add(statusComboBox);
        this.Controls.Add(addReservationButton);
        this.Controls.Add(removeReservationButton);
        this.Controls.Add(updateStatusButton);
        this.Controls.Add(reservationsListBox);
        reservationManager = new ReservationManager();
        UpdateReservationsList();
    }

    private void UpdateReservationsList()
    {
        reservationsListBox.Items.Clear();
        foreach (var reservation in reservationManager.Reservations)
        {
            reservationsListBox.Items.Add($"{reservation.CustomerName} - {reservation.StartTime.ToString("yyyy-MM-dd HH:mm")} - {reservation.Status}");
        }
    }

    private void AddReservationButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(customerNameTextBox.Text))
        {
            MessageBox.Show("Введите имя клиента!");
            return;
        }
        DateTime startTime = startTimePicker.Value;
        DateTime endTime = endTimePicker.Value;
        if (startTime >= endTime)
        {
            MessageBox.Show("Время начала должно быть раньше времени окончания!");
            return;
        }
        Reservation newReservation = new Reservation(customerNameTextBox.Text,
        startTime, endTime);
        try
        {
            reservationManager.AddReservation(newReservation);
            customerNameTextBox.Clear();
            UpdateReservationsList();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void RemoveReservationButton_Click(object sender, EventArgs e)
    {
        if (reservationsListBox.SelectedIndex == -1)
        {
            MessageBox.Show("Выберите резервирование для удаления!");
            return;
        }
        string selectedItem = reservationsListBox.SelectedItem.ToString();
        string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
        if (parts.Length >= 3)
        {
            string customerName = parts[0].Trim();
            DateTime startTime;
            if (DateTime.TryParse(parts[1].Trim(), out startTime))
            {
                var reservationToRemove = reservationManager.Reservations.Find(r =>
                r.CustomerName == customerName && r.StartTime == startTime);
                if (reservationToRemove != null)
                {
                    try
                    {
                        reservationManager.RemoveReservation(reservationToRemove);
                        UpdateReservationsList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }

    private void UpdateStatusButton_Click(object sender, EventArgs e)
    {
        if (reservationsListBox.SelectedIndex == -1)
        {
            MessageBox.Show("Выберите резервирование для обновления статуса!");
            return;
        }
        string selectedItem = reservationsListBox.SelectedItem.ToString();
        string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
        if (parts.Length >= 3)
        {
            string customerName = parts[0].Trim();
            DateTime startTime;
            if (DateTime.TryParse(parts[1].Split(' ')[0], out startTime))
            {
                var reservationToUpdate = reservationManager.Reservations.Find(r =>
                r.CustomerName == customerName && r.StartTime == startTime);
                if (reservationToUpdate != null)
                {
                    ReservationStatus newStatus =
                    (ReservationStatus)Enum.Parse(typeof(ReservationStatus),
                    statusComboBox.SelectedItem.ToString());
                    try
                    {
                        reservationManager.UpdateReservationStatus(reservationToUpdate,
                        newStatus);
                        UpdateReservationsList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}