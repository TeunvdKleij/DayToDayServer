using DayToDay.Data;

namespace DayToDay.Services;

public class NoteService
{
    private readonly DataContext _dataContext;
    private readonly LogService _logService;

    public NoteService(DataContext dataContext, LogService logService)
    {
        _dataContext = dataContext;
        _logService = logService;
    }

}