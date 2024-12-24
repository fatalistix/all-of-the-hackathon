using AllOfTheHackathon.Contracts;

namespace HrDirectorRabbitService.Services;

public class CollectingService(HrDirectorService service)
{
    private volatile IList<Team>? _teams;
    private volatile IList<Wishlist>? _teamLeadsWishlists;
    private volatile IList<Wishlist>? _juniorsWishlists;
    private readonly Mutex _mutex = new();
    
    public void AddTeams(IList<Team> teams)
    {
        _teams = teams;
        CheckAndHandle();
    }

    public void AddWishlists(IList<Wishlist> teamLeadsWishlists, IList<Wishlist> juniorWishlists)
    {
        _teamLeadsWishlists = teamLeadsWishlists;
        _juniorsWishlists = juniorWishlists;
        CheckAndHandle();
    }

    private void CheckAndHandle()
    {
        _mutex.WaitOne();
        if (_teams != null && _teamLeadsWishlists != null && _juniorsWishlists != null)
        {
            service.DoWork(_teams, _teamLeadsWishlists, _juniorsWishlists);
            _teams = null;
            _teamLeadsWishlists = null;
            _juniorsWishlists = null;
        }
        _mutex.ReleaseMutex();
    }
}