using SchoolLab.Models;

namespace SchoolLab.Service;
public class LabAdminService : BaseLabService
{
    public LabAdminService() : base() { }

    public List<Computer> GetComputers(string labId)
    {
        Lab? targetLab = GetLabById(labId);

        if (targetLab is not null)
        {
            return targetLab.GetComputers();
        } 
        else throw new Exception("Laboratory not found");
    }

    public async Task<Computer> CreateComputer(string labId, ComputerDto computer)
    {
        Lab? targetLab = GetLabById(labId);

        if (targetLab is not null)
        {
            Computer newComputer = new(computer.Description, computer.Status, targetLab.Id);
            targetLab.AddComputer(newComputer);
            await SaveLabs();
            return newComputer;
        }
        throw new Exception("Lab not found");
    }

    public async Task<Computer> UpdateComputer(string labId, string computerId, ComputerDto computerUpdated)
    {
        Lab? targetLab = GetLabById(labId);

        if (targetLab is not null)
        {
            Computer? existingComputer = targetLab.GetComputerById(computerId);
            // Computer? existingComputer = targetLab.Computers.FirstOrDefault(c => c.Id == computerId);

            if (existingComputer is not null)
            {
                if (existingComputer.LabAssigned != computerUpdated.LabAssigned)
                {
                    if (!AssignComputerToLab(existingComputer.LabAssigned, existingComputer, computerUpdated.LabAssigned))
                    {
                        throw new Exception("Failed to assign computer to the new lab.");
                    }
                }

                existingComputer.Description = computerUpdated.Description;
                existingComputer.Status = computerUpdated.Status;
                existingComputer.LabAssigned = computerUpdated.LabAssigned;

                await SaveLabs();

                return existingComputer;
            }
            throw new Exception("the computer was not found inside the lab");
        }
        throw new Exception("Lab not found");
    }

    public async Task<bool> DeleteComputer(string labId, string computerId)
    {
        Lab? targetLab = GetLabById(labId);

        if (targetLab is not null)
        {
            Computer? computerToDelete = targetLab.GetComputerById(computerId);
            // Computer? computerToDelete = targetLab.Computers.FirstOrDefault(c => c.Id == computerId);

            if (computerToDelete is not null)
            {
                targetLab.RemoveComputer(computerToDelete);

                Lab? lab = GetLabById(computerToDelete.LabAssigned);

                if (lab is not null)
                {
                    lab.RemoveComputer(computerToDelete);
                    await SaveLabs();
                    return true;
                }
                throw new Exception("Failed to find computer in lab.");
            }
        }
        return false;
    }

    private bool AssignComputerToLab(string labid, Computer computer, string newLabAssigned)
    {
        Lab? oldLab = GetLabById(labid);
        Lab? newLab = GetLabById(newLabAssigned);

        if (oldLab is not null && newLab is not null)
        {
            oldLab.RemoveComputer(computer);

            if (!newLab.ContainsComputer(computer))
            {
                newLab.AddComputer(computer);

                foreach (var res in _userReservations.Where(r => r.ComputerId == computer.Id))
                {
                    res.AssignInNewLab(newLab.Id);
                }

                SaveReservations().Wait();
                return true;
            }
            throw new Exception("Computer is already assigned to the new lab.");
        }
        throw new Exception("Computer not found or lab assignment unchanged.");
    }
} 