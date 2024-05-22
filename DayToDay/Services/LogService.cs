using DayToDay.Data;
using Serilog;

namespace DayToDay.Services;

public class LogService
{
    public LogService(DataContext dataContext) { }

    public void InformationLog(string controllerName, string methodName, string notice)
    {
        Log.Information("Controller: " + controllerName + " | " + methodName + " | " +notice);
    }
    public void ErrorLog(string controllerName, string methodName, string notice)
    {
        Log.Error("Controller: " + controllerName + " | " + methodName + " | " +notice);
    }
    public void WarningLog(string controllerName, string methodName, string notice)
    {
        Log.Warning("Controller: " + controllerName + " | " + methodName + " | " +notice);
    }
}