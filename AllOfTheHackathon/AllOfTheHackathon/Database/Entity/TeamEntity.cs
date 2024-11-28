namespace AllOfTheHackathon.Database.Entity;

public class TeamEntity
{
    public int Id { get; set; }
    public TeamLeadEntity TeamLead { get; set; }
    public JuniorEntity Junior { get; set; }
}