using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReservationManager
{
    public List<Reservation> Reservations { get; private set; }

    public ReservationManager()
    {
        Reservations = new List<Reservation>();
        LoadReservations();
    }

    public void AddReservation(Reservation reservation)
    {
        if (Reservations.Any(r => HasOverlap(r, reservation)))
            throw new ArgumentException("Время резервирования пересекается");

        Reservations.Add(reservation);
    }

    public void RemoveReservation(Reservation reservation)
    {
        if (!Reservations.Contains(reservation))
            throw new ArgumentException("Резервирование не найдено");

        Reservations.Remove(reservation);
    }

    public void UpdateReservationStatus(Reservation reservation, ReservationStatus newStatus)
    {
        if (reservation == null)
        {
            throw new ArgumentNullException(nameof(reservation));
        }
        reservation.UpdateStatus(newStatus);
        SaveReservations();
    }

    private bool HasOverlap(Reservation existing, Reservation newReservation)
    {
        return existing.StartTime < newReservation.EndTime &&
               newReservation.StartTime < existing.EndTime;
    }

    private void SaveReservations()
    {
        // var lines = Reservations.Select(r =>
        //    $"{r.CustomerName}|{r.StartTime:yyyy-MM-dd HH:mm}|{r.EndTime:yyyy-MM-dd HH:mm}|{(int)r.Status}");
        // File.WriteAllLines("reservations.txt", lines);
        File.WriteAllLines("reservations.txt", Reservations.Select(r =>
        $"{r.CustomerName}|{r.StartTime.ToString("yyyy-MM-dd HH: mm")}|{r.EndTime.ToString("yyyy - MM - dd HH: mm")}|{(int)r.Status}"));
    }

    private void LoadReservations()
    {
        // if (!File.Exists("reservations.txt")) return;

        // var lines = File.ReadAllLines("reservations.txt");
        // foreach (var line in lines)
        // {
        //    var parts = line.Split('|');
        //    if (parts.Length == 4)
        //    {
        //        if (DateTime.TryParse(parts[1], out DateTime startTime) &&
        //            DateTime.TryParse(parts[2], out DateTime endTime) &&
        //            Enum.TryParse<ReservationStatus>(parts[3], out ReservationStatus status))
        //        {
        //            var reservation = new Reservation(parts[0], startTime, endTime);
        //            reservation.Status = status;
        //            Reservations.Add(reservation);
        //        }
        //    }
        // }

        if (File.Exists("reservations.txt"))
        {
            var lines = File.ReadAllLines("reservations.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 4)
                {
                    DateTime startTime;
                    DateTime endTime;
                    ReservationStatus status;
                    if (DateTime.TryParse(parts[1], out startTime) && DateTime.TryParse(parts[2],
                    out endTime) && Enum.TryParse<ReservationStatus>(parts[3], out status))
                    {
                        Reservations.Add(new Reservation(parts[0], startTime, endTime));
                        Reservations.Last().Status = status;
                    }
                }
            }
        }
    }

    public List<Reservation> FindByCustomerName(string customerName)
    {
        return Reservations.Where(r => r.CustomerName == customerName).ToList();
    }
}