using Newtonsoft.Json;

namespace SchoolLab.Models;
public enum ComputerStatus
{
    Available,
    Maintenance,
    OutOfOrder,
    Removed,
    Reserved
}

public class Computer
{
    public string? Id { get; set; }
    public string Description { get; set; }
    public ComputerStatus Status { get; set; }
    public string LabAssigned { get; set; }
    private DateTime? CreationDate { get; set; }
    public ComputerCalendar Calendar { get; set; }

    [JsonConstructor]
    public Computer(string id, string description, ComputerStatus status, string labAssigned, DateTime date, ComputerCalendar calendar)
    {
        Id = id;
        Description = description;
        Status = status;
        LabAssigned = labAssigned;
        CreationDate = date;
        Calendar = calendar;
    }

    public Computer(string description, ComputerStatus status, string labAssigned)
    {
        Id = Guid.NewGuid().ToString("N")[..10];
        Description = description;
        Status = status;
        LabAssigned = labAssigned;
        CreationDate = DateTime.Today;
        Calendar = new ComputerCalendar();
    }

    public bool IsAvailable(DateTime date, int slot)
    {
        return Calendar.IsSlotAvailable(date, slot);
    }

    public Reservation ReserveSlotCalendar(DateTime date, int slot, string userId, string labId, string computerId)
    {
        Reservation reservation = Calendar.ReserveSlot(date, slot, userId, labId, computerId);
        return reservation;
    }

    public bool UpdateCalendarReservation(string reservationId)
    {
        return Calendar.CancelReservation(reservationId);
    }
}