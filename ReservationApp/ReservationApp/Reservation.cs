using System;

public class Reservation
{
    public string CustomerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ReservationStatus Status { get; set; }

    public Reservation(string customerName, DateTime startTime, DateTime endTime)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Имя клиента не может быть пустым");

        if (startTime >= endTime)
            throw new ArgumentException("Время начала должно быть раньше времени окончания");

        CustomerName = customerName;
        StartTime = startTime;
        EndTime = endTime;
        Status = ReservationStatus.Активно;
    }
    public bool IsActive()
    {
        return Status == ReservationStatus.Активно;
    }

    public TimeSpan GetDuration()
    {
        return EndTime - StartTime;
    }

    public override string ToString()
    {
        return $"{CustomerName} - {StartTime:yyyy-MM-dd HH:mm} - {Status}";
    }

    public void UpdateStatus(ReservationStatus newStatus)
    {
        Status = newStatus;
    }
}