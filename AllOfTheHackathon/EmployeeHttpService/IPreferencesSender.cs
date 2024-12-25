using HttpCommon.Requests;
using Refit;

namespace EmployeeHttpService;

public interface IPreferencesSender
{
    [Post("/")]
    Task SendPreferences(EmployeeRequest request);
}