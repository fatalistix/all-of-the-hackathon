using System.Text.Json.Serialization;
using AllOfTheHackathon.Contracts;

namespace HrDirectorHttpService.Requests;

public record TeamRequest(
    [property: JsonPropertyName("teams")] IList<Team> Teams,
    [property: JsonPropertyName("team_leads_wishlists")] IList<Wishlist> TeamLeadsWishlists,
    [property: JsonPropertyName("juniors_wishlists")] IList<Wishlist> JuniorsWishlists);