using System.Text.Json.Serialization;
using AllOfTheHackathon.Contracts;

namespace HttpCommon.Requests;

public record TeamsAndWishlistsRequest(
    [property: JsonPropertyName("teams")] IList<Team> Teams,
    [property: JsonPropertyName("team_leads_wishlists")] IList<Wishlist> TeamLeadsWishlists,
    [property: JsonPropertyName("juniors_wishlists")] IList<Wishlist> JuniorsWishlists);