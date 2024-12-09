namespace AllOfTheHackathon.Service.Transient;

public class PrintAverageSatisfactionScenario(AverageSatisfactionCalculator averageSatisfactionCalculator)
{
    public void Perform()
    {
        var (average, total) = averageSatisfactionCalculator.Calculate();
        Console.WriteLine($"Средний уровень удовлетворенности по {total} хакатонам равен {average}");
        Console.WriteLine();
    }
}