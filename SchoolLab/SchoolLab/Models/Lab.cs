using Newtonsoft.Json;
using SchoolLab.Models;

public class Lab
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    [JsonProperty("Computers")]
    public List<Computer> Computers { get; set; }

    [JsonConstructor]
    public Lab(string id, string name, string description, List<Computer> computer)
    {
        Id = id;
        Name = name;
        Description = description;
        Computers = computer;
    }

    public Computer GetComputerById(string computerId)
    {
        return Computers.First(c => c.Id == computerId);
    }

    public List<Computer> GetComputers()
    {
        return Computers;
    }

    public Computer GetFirstAvailableComputer(DateTime date, int slot)
    {
        try
        {
            Computer? availableComputer = Computers.FirstOrDefault(c => c.IsAvailable(date, slot));

            return availableComputer;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool AddComputer(Computer computer)
    {
        if (computer is not null)
        {
            Computers.Add(computer);
            return true;
        }
        return false;
    }

    public bool RemoveComputer(Computer computer)
    {
        return Computers.Remove(computer);
    }

    public bool ContainsComputer(Computer computer)
    {
        if (Computers.Contains(computer))
        {
            return true;
        }
        return false;
    }
}