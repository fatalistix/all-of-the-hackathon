using AllOfTheHackathon.Database.Context;

namespace AllOfTheHackathon.Service.Transient;

public class AverageSatisfactionCalculator(
    HackathonContext hackathonContext)
{
    public (double averageSatisfaction, int totalHackathons) Calculate()
    {
        var averageSatisfaction = hackathonContext.Hackathons.Sum(h => h.AverageSatisfaction);
        var totalHackathons = hackathonContext.Hackathons.Count();
        return (averageSatisfaction, totalHackathons);
    }
}