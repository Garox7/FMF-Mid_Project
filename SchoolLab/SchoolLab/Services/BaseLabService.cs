using FileManager;
using SchoolLab.Models;

namespace SchoolLab.Service;
public class BaseLabService
{
    protected List<Lab> _labs = new();
    protected List<Reservation> _userReservations = new();
    private readonly string _fileLabsPath = "./Data/Labs.json";
    private readonly string _fileReservationsPath = "./Data/Reservations.json";
    private readonly DataManager<List<Lab>> _labsDataManager;
    private readonly DataManager<List<Reservation>> _reservationsDataManager;


    protected BaseLabService()
    {
        _labsDataManager = new DataManager<List<Lab>>(_fileLabsPath);
        _reservationsDataManager = new DataManager<List<Reservation>>(_fileReservationsPath);
        LoadLabs().Wait();
        LoadReservations().Wait();
    }

    protected async Task LoadLabs()
    {
        _labs = await _labsDataManager.LoadData<List<Lab>>();
    }

    protected async Task SaveLabs()
    {
        await _labsDataManager.SaveData(_labs);
    }

    protected async Task LoadReservations()
    {
        try
        {
            _userReservations = await _reservationsDataManager.LoadData<List<Reservation>>();

            // check if expired reservations exist
            List<Reservation> expiredReservations = _userReservations.Where(r => r.Date < DateTime.Now).ToList();

            if (expiredReservations is not null)
            {
                _userReservations.RemoveAll(r => r.Date < DateTime.Now);
                SaveReservations().Wait();
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Errore durante il caricamento dei dati: {e}");
        }
    }

    protected async Task SaveReservations()
    {
        try
        {
            await _reservationsDataManager.SaveData(_userReservations);
        }
        catch (Exception e)
        {
            throw new Exception($"Errore durante il salvataggio dei dati: {e}");
        }
    }

    public List<Lab> GetAllLabs()
    {
        return _labs;
    }

    public Lab? GetLabById(string labId)
    {
        Lab? lab = _labs.Find(lab => lab.Id == labId);
        return lab;
    }
}