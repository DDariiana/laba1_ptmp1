using System;

namespace ReservationApp
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }

        // Конструктор для создания новой записи
        public Reservation(string customerName, DateTime startTime, DateTime endTime)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Имя клиента не может быть пустым.");
            if (startTime >= endTime)
                throw new ArgumentException("Время начала должно быть раньше времени окончания.");

            Id = Guid.NewGuid();
            CustomerName = customerName;
            StartTime = startTime;
            EndTime = endTime;
            Status = ReservationStatus.Активно;
        }

        // Конструктор для загрузки из файла
        public Reservation(Guid id, string customerName, DateTime startTime, DateTime endTime, ReservationStatus status)
        {
            Id = id;
            CustomerName = customerName;
            StartTime = startTime;
            EndTime = endTime;
            Status = status;
        }

        public override string ToString()
        {
            return $"{CustomerName} - {StartTime:yyyy-MM-dd HH:mm} - {EndTime:yyyy-MM-dd HH:mm} - {Status}";
        }
    }
}