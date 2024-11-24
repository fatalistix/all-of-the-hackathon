using AllOfTheHackathon.Contracts;
using AllOfTheHackathon.Internal.Extension;

namespace AllOfTheHackathon.Service.Transient;

public class Hackathon
{
    public (IList<Wishlist> teamLeadsWishlists, IList<Wishlist> juniorsWhishLists) Hold(in IList<Employee> teamLeads, in IList<Employee> juniors)
    {
        var teamLeadsWishlists = new List<Wishlist>();
        var juniorsWishLists = new List<Wishlist>();
        
        var juniorsCopy = juniors.ToList();
        foreach (var t in teamLeads)
        {
            juniorsCopy.Shuffle();
            teamLeadsWishlists.Add(new Wishlist(t.Id, juniorsCopy.Select(j => j.Id).ToArray()));
        }

        var teamLeadsCopy = teamLeads.ToList();
        foreach (var j in juniors)
        {
            teamLeadsCopy.Shuffle();
            juniorsWishLists.Add(new Wishlist(j.Id, teamLeadsCopy.Select(t => t.Id).ToArray()));
        }

        return (teamLeadsWishlists, juniorsWishLists);
    }

    private void Validate(in IList<Employee> teamLeads, in IList<Employee> juniors)
    {
        if (teamLeads.Count != juniors.Count)
        {
            throw new ArgumentException("Number of team leads is not equal to number of juniors");
        } 
    }
}