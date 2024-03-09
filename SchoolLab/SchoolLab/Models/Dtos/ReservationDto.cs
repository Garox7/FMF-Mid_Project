namespace SchoolLab.Models;
public class ReservationDto
{
    public string LabId { get; }
    public DateTime Date { get; }
    public int TimeSlot { get; }
    public string UserId { get; }

    public ReservationDto(string labId, DateTime date, int timeSlot, string userId)
    {
        LabId = labId;
        Date = date;
        TimeSlot = timeSlot;
        UserId = userId;
    }
}