using System.Text.Json.Serialization;

namespace EmployeeHttpService;

public record Request(
    [property: JsonPropertyName("id")] int Id, 
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("desired_employees")] IList<int> DesiredEmployees, 
    [property: JsonPropertyName("type")] string Type);