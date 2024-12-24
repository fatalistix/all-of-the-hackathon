using AllOfTheHackathon.Contracts;

namespace RabbitCommon.Messages;

public record WishlistsMessage(IList<Wishlist> TeamLeadsWishlists, IList<Wishlist> JuniorsWishlists);