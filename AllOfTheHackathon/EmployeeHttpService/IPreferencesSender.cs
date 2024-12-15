using Refit;

namespace EmployeeHttpService;

public interface IPreferencesSender
{
    [Post("/")]
    Task SendPreferences(Request request);
}