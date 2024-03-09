using Newtonsoft.Json;

namespace SchoolLab.Models;

public class ComputerCalendar
{
    [JsonProperty]
    public Dictionary<DateTime, Dictionary<int, Reservation>> Reservations { get; set; }

    public ComputerCalendar()
    {
        Reservations = new Dictionary<DateTime, Dictionary<int, Reservation>>();

        for (int i = 0; i < 10; i++)
        {
            DateTime day = DateTime.Today.AddDays(i);
            Reservations[day] = new Dictionary<int, Reservation>();

            for (int timeSlot = 9; timeSlot < 19; timeSlot++)
            {
                Reservation initialReservation = new("", "", "", "", DateTime.Today, 0);
                Reservations[day][timeSlot] = initialReservation;
            }
        }
    }

    public bool IsSlotAvailable(DateTime date, int slot)
    {
        if (!IsAllowedDay(date))
        {
            throw new Exception("Day not allowed for booking");
        }

        if (!IsAllowedTimeSlot(slot))
        {
            throw new Exception("Time not allowed for booking");
        }

        if (Reservations.TryGetValue(date, out var dayReservation) && dayReservation.TryGetValue(slot, out var reservation))
        {
            return reservation.UserId == "";
        }
        return true;
    }

    public Reservation ReserveSlot(DateTime date, int slot, string userId, string labId, string computerId)
    {
        if (!IsAllowedDay(date))
        {
            throw new Exception("Day not allowed for booking");
        }

        if (!IsAllowedTimeSlot(slot))
        {
            throw new Exception("Time not allowed for booking");
        }

        if (Reservations.TryGetValue(date, out var dayReservation))
        {
            Reservation newReservation = new(userId, labId, computerId, date, slot);
            newReservation.GenerateRandomId();
            dayReservation[slot] = newReservation;
            return newReservation;
        }
        throw new Exception("no slot available for the selected day");
    }

    public bool CancelReservation(string reservationId)
    {
        Reservation? reservationToDelete = Reservations.Values
            .SelectMany(slotReservation => slotReservation.Values)
            .FirstOrDefault(r => r.Id == reservationId);

        if (reservationToDelete is null)
        {
            return false;
        }

        reservationToDelete.ResetReservation();
        return true;
    }

    private bool IsAllowedDay (DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }

    private bool IsAllowedTimeSlot(int slot)
    {
        return slot >= 9 && slot <= 18;
    }
}