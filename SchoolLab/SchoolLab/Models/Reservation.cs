using Newtonsoft.Json;

namespace SchoolLab.Models;
public class Reservation
{
    public string? Id { get; private set; }
    public string UserId { get; private set; }
    public string LabId { get; private set; }
    public string ComputerId { get; private set; }
    public DateTime Date { get; private set; }
    public int TimeSlot { get; private set; }

    [JsonConstructor]
    public Reservation(string id, string userId, string labId, string computerId, DateTime date, int timeSlot)
    {
        Id = id;
        UserId = userId;
        LabId = labId;
        ComputerId = computerId;
        Date = date;
        TimeSlot = timeSlot;
    }

    public Reservation(string userId, string labId, string computerId, DateTime date, int timeSlot)
    {
        UserId = userId;
        LabId = labId;
        ComputerId = computerId;
        Date = date;
        TimeSlot = timeSlot;
    }

    public void ResetReservation()
    {
        Id = "";
        UserId = "";
        LabId = "";
        ComputerId = "";
        TimeSlot = 0;
    }

    public void GenerateRandomId()
    {
        Id = Guid.NewGuid().ToString("N")[..10];
    }

    public void AssignInNewLab(string newLabId)
    {
        if (LabId != newLabId)
        {
            LabId = newLabId;
        }
    }
}