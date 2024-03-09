namespace SchoolLab.Models;

public class ComputerDto
{
    public string Description { get; }
    public ComputerStatus Status { get; }
    public string LabAssigned { get; }

    public ComputerDto(string description, ComputerStatus status, string labAssigned)
    {
        Description = description;
        Status = status;
        LabAssigned = labAssigned;
    }
}