namespace HrDirectorRabbitService.Services;

public class StatusService
{
    public readonly int HackathonTimes;

    public StatusService()
    {
        var hackathonTimesStr = Environment.GetEnvironmentVariable("HACKATHON_TIMES");
        if (hackathonTimesStr == null)
        {
            Console.Error.WriteLine("No HACKATHON_TIMES variable");
            throw new InvalidOperationException("No HACKATHON_TIMES variable");
        }

        var success = int.TryParse(hackathonTimesStr, out var hackathonTimes);
        if (!success)
        {
            Console.Error.WriteLine("HACKATHON_TIMES does not contains number");
            throw new InvalidOperationException("HACKATHON_TIMES does not contains number");
        }

        if (hackathonTimes < 1)
        {
            Console.Error.WriteLine("HACKATHON_TIMES contains number less than 1");
            throw new InvalidOperationException("HACKATHON_TIMES contains number less than 1");
        }

        HackathonTimes = hackathonTimes;
    }
}