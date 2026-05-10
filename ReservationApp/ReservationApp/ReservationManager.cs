using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReservationApp
{
    public class ReservationManager
    {
        // ОСТАВЛЯЕМ ОДНО поле — публичное свойство
        public List<Reservation> Reservations { get; private set; }
        private readonly string _filePath = "reservations.txt";

        public ReservationManager()
        {
            Reservations = new List<Reservation>();
            LoadReservations();
        }

        public bool IsTimeAvailable(DateTime start, DateTime end)
        {
            return !Reservations.Any(r =>
                r.Status == ReservationStatus.Активно &&
                start < r.EndTime &&
                end > r.StartTime);
        }

        public List<string> GetAvailableSlots(DateTime startDate, int daysToCheck = 30)
        {
            var availableDates = new List<string>();

            for (int i = 0; i < daysToCheck; i++)
            {
                DateTime checkDate = startDate.AddDays(i);
                bool isDateAvailable = true;

                foreach (var r in Reservations)
                {
                    if (r.Status == ReservationStatus.Активно)
                    {
                        if ((checkDate.Date >= r.StartTime.Date && checkDate.Date <= r.EndTime.Date) ||
                            (r.StartTime.Date <= checkDate.Date && r.EndTime.Date >= checkDate.Date))
                        {
                            isDateAvailable = false;
                            break;
                        }
                    }
                }

                if (isDateAvailable)
                {
                    availableDates.Add(checkDate.ToString("dd.MM.yyyy (dddd)"));
                }
            }

            return availableDates;
        }

        // 🔧 ИСПРАВЛЕНО: используем Reservations вместо _reservations
        public void AddReservation(Reservation reservation)
        {
            foreach (var existing in Reservations) // ← было _reservations
            {
                if (existing.Status == ReservationStatus.Активно &&
                    reservation.StartTime < existing.EndTime &&
                    reservation.EndTime > existing.StartTime)
                {
                    throw new ArgumentException(
                        $"Время с {reservation.StartTime} по {reservation.EndTime} уже занято резервированием для {existing.CustomerName}");
                }
            }

            Reservations.Add(reservation); // ← было _reservations
            SaveReservations(); // ← не забываем сохранить!
        }

        public void RemoveReservationById(Guid id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                throw new ArgumentException("Резервирование не найдено.");

            Reservations.Remove(reservation);
            SaveReservations();
        }

        public void UpdateReservationStatusById(Guid id, ReservationStatus newStatus)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                throw new ArgumentException("Резервирование не найдено.");

            reservation.Status = newStatus;
            SaveReservations();
        }

        private void SaveReservations()
        {
            var lines = Reservations.Select(r =>
                $"{r.Id}|{r.CustomerName}|{r.StartTime:yyyy-MM-dd HH:mm}|{r.EndTime:yyyy-MM-dd HH:mm}|{(int)r.Status}");
            File.WriteAllLines(_filePath, lines);
        }

        private void LoadReservations()
        {
            if (!File.Exists(_filePath)) return;

            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 5 &&
                    Guid.TryParse(parts[0], out Guid id) &&
                    DateTime.TryParse(parts[2], out DateTime start) &&
                    DateTime.TryParse(parts[3], out DateTime end) &&
                    int.TryParse(parts[4], out int statusInt))
                {
                    if (Enum.IsDefined(typeof(ReservationStatus), statusInt))
                    {
                        Reservations.Add(new Reservation(id, parts[1], start, end, (ReservationStatus)statusInt));
                    }
                }
            }
        }

        public void RemoveReservation(Reservation reservation)
        {
            RemoveReservationById(reservation.Id);
        }

        public void UpdateReservationStatus(Reservation reservation, ReservationStatus status)
        {
            UpdateReservationStatusById(reservation.Id, status);
        }
    }
}