using AllOfTheHackathon.Database.Context;

namespace AllOfTheHackathon.Service.Transient;

public class HackathonScenario(HackathonEvent hackathonEvent, HackathonContext hackathonContext)
{
    public void Perform()
    {
        var id = hackathonEvent.Handle();
        hackathonContext.SaveChanges();
        Console.WriteLine($"Хакатон проведен. Идентификатор: {id}");
        Console.WriteLine();
    }
}