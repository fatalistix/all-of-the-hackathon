using AllOfTheHackathon.Service.Transient;

namespace AllOfTheHackathon.Calculator;

public class HarmonicMeanCalculator : ICalculator
{
    
    public double Calculate(IList<int> values)
    {
        var sum = values.Sum(v => 1.0 / v);
        return values.Count / sum;
    }
}