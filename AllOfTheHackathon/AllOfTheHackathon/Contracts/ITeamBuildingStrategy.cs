namespace AllOfTheHackathon.Contracts;

public interface ITeamBuildingStrategy
{
    /// <summary>
    /// Распределяет тиилидов и джунов по командам
    /// </summary>
    /// <param name="teamLeads">Тимлиды</param>
    /// <param name="juniors">Джуны</param>
    /// <returns>Список команд</returns>
    /// <exception cref="ArgumentException">Если размер входящих данных не совпадает</exception>
    IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, 
        IEnumerable<Employee> juniors, 
        IEnumerable<Wishlist> teamLeadsWishlists, 
        IEnumerable<Wishlist> juniorsWishlists);
}