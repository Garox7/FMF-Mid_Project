using SchoolLab.Models;
using FileManager;
using System.Globalization;

namespace SchoolLab.Service;
public class LabUserService : BaseLabService
{
    public LabUserService() : base() { }

    public List<Reservation> GetReservationsByUserId(string userId)
    {
        List<Reservation> userReservations = _userReservations.FindAll(r => r.UserId == userId);

        if (userReservations.Count is not 0)
        {
            return userReservations;
        }
        throw new Exception("No reservation found for this user");
    }

    public async Task<Reservation> BookComputer(string labId, DateTime date, int slot, string userId)
    {
        Lab labSelected = GetLabById(labId);

        if (labSelected is not null)
        {
            Computer? firstAvailableComputer = labSelected.GetFirstAvailableComputer(date, slot);

            if (firstAvailableComputer is not null && firstAvailableComputer.Status is ComputerStatus.Available)
            {
                Reservation? newReservation = firstAvailableComputer.ReserveSlotCalendar(date, slot, userId, labId, firstAvailableComputer.Id);

                _userReservations.Add(newReservation);
                await SaveReservations();
                await SaveLabs();
                return newReservation;
            }
            throw new Exception("No computers available");
        }
        throw new Exception("Lab not found");
    }
    
    public async Task<bool> CancelBooking(string reservationId, string userId)
    {
        Reservation? reservationToDelete = _userReservations.Find(r => r.Id == reservationId && r.UserId == userId);

        if (reservationToDelete is null)
        {
            throw new Exception("no booking found");
        }

        Lab? lab = GetLabById(reservationToDelete.LabId);
        Computer? computer = lab.GetComputerById(reservationToDelete.ComputerId);

        if (computer is null)
        {
            throw new Exception("Computer not found");
        }

        if (!computer.UpdateCalendarReservation(reservationToDelete.Id))
        {
            throw new Exception("Failed to update computer calendar");
        }

        _userReservations.Remove(reservationToDelete);

        await SaveReservations();
        await SaveLabs();
        return true;
    }
}