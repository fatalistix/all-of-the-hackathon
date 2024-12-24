using System.Text.Json.Serialization;
using AllOfTheHackathon.Contracts;

namespace RabbitCommon.Requests;

public record TeamRequest(
    [property: JsonPropertyName("teams")] IList<Team> Teams);
