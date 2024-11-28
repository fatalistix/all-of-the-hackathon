namespace AllOfTheHackathon.Database.Entity;

public class HackathonEntity
{
    public Guid Id { get; set; }
    public IList<TeamLeadEntity> TeamLeads { get; set; } = new List<TeamLeadEntity>();
    public IList<JuniorEntity> Juniors { get; set; } = new List<JuniorEntity>();
    public IList<TeamLeadWishlistEntity> TeamLeadWishlists { get; set; } = new List<TeamLeadWishlistEntity>();
    public IList<JuniorWishlistEntity> JuniorWishlists { get; set; } = new List<JuniorWishlistEntity>();
    public IList<TeamEntity> Teams { get; set; } = new List<TeamEntity>();
    public double AverageSatisfaction { get; set; }
}
