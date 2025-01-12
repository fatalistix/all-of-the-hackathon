using AllOfTheHackathon.Contracts;

namespace HrDirectorRabbitService.Services;

public class CollectingService(HrDirectorService service)
{
    private volatile IList<Team>? _teams;
    private volatile IList<Wishlist>? _teamLeadsWishlists;
    private volatile IList<Wishlist>? _juniorsWishlists;
    private readonly Mutex _mutex = new();
    
    public void AddTeams(IList<Team> teams, Guid hackathonId)
    {
        _teams = teams;
        CheckAndHandle(hackathonId);
    }

    public void AddWishlists(IList<Wishlist> teamLeadsWishlists, IList<Wishlist> juniorWishlists, Guid hackathonId)
    {
        _teamLeadsWishlists = teamLeadsWishlists;
        _juniorsWishlists = juniorWishlists;
        CheckAndHandle(hackathonId);
    }

    private void CheckAndHandle(Guid hackathonId)
    {
        _mutex.WaitOne();
        if (_teams != null && _teamLeadsWishlists != null && _juniorsWishlists != null)
        {
            service.DoWork(_teams, _teamLeadsWishlists, _juniorsWishlists, hackathonId);
            _teams = null;
            _teamLeadsWishlists = null;
            _juniorsWishlists = null;
        }
        _mutex.ReleaseMutex();
    }
}