using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReservationApp
{
    public class ReservationManager
    {
        public List<Reservation> Reservations { get; private set; }
        private readonly string _filePath = "reservations.txt";

        public ReservationManager()
        {
            Reservations = new List<Reservation>();
            LoadReservations();
        }

        public void AddReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            // Проверка пересечения только с активными резервациями
            bool hasOverlap = Reservations.Any(r =>
                r.Status == ReservationStatus.Активно &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);

            if (hasOverlap)
                throw new InvalidOperationException(
                    "Период резервирования пересекается с существующей активной записью.");

            Reservations.Add(reservation);
            SaveReservations();
        }

        public void RemoveReservationById(Guid id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                throw new ArgumentException("Резервирование не найдено.");

            Reservations.Remove(reservation);
            SaveReservations();
        }

        // Перегрузка для совместимости
        public void RemoveReservation(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));
            RemoveReservationById(reservation.Id);
        }

        public void UpdateReservationStatusById(Guid id, ReservationStatus newStatus)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                throw new ArgumentException("Резервирование не найдено.");

            reservation.UpdateStatus(newStatus);
            SaveReservations();
        }

        // Перегрузка для совместимости
        public void UpdateReservationStatus(Reservation reservation, ReservationStatus newStatus)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));
            UpdateReservationStatusById(reservation.Id, newStatus);
        }

        public Reservation GetReservationById(Guid id)
        {
            return Reservations.FirstOrDefault(r => r.Id == id);
        }

        public List<Reservation> GetActiveReservations()
        {
            return Reservations.Where(r => r.Status == ReservationStatus.Активно).ToList();
        }

        public List<Reservation> FindByCustomerName(string customerName)
        {
            return Reservations.Where(r => r.CustomerName.Contains(customerName)).ToList();
        }

        private void SaveReservations()
        {
            var lines = Reservations.Select(r =>
                $"{r.Id}|{r.CustomerName}|{r.StartTime:yyyy-MM-dd HH:mm}|{r.EndTime:yyyy-MM-dd HH:mm}|{(int)r.Status}");
            File.WriteAllLines(_filePath, lines);
        }

        private void LoadReservations()
        {
            if (!File.Exists(_filePath))
                return;

            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 5 &&
                    Guid.TryParse(parts[0], out Guid id) &&
                    DateTime.TryParse(parts[2], out DateTime startTime) &&
                    DateTime.TryParse(parts[3], out DateTime endTime) &&
                    int.TryParse(parts[4], out int statusInt) &&
                    Enum.IsDefined(typeof(ReservationStatus), statusInt))
                {
                    var reservation = new Reservation(id, parts[1], startTime, endTime, (ReservationStatus)statusInt);
                    Reservations.Add(reservation);
                }
            }
        }
    }
}