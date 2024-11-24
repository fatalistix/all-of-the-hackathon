using AllOfTheHackathon.Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllOfTheHackathonTest.Calculator;

[TestClass]
public class HarmonicMeanCalculatorTest
{
    private readonly HarmonicMeanCalculator _calculator = new();

    [TestMethod]
    public void GivenSpecifiedNumbers_WhenCalculate_ThenExpectedValue()
    {
        var testCases = new List<TestCase>([
            new TestCase([3, 3, 3, 3], 3.0),
            new TestCase([2, 6], 3.0),
            new TestCase([3, 1, 3, 1], 1.5)
        ]);

        foreach (var tc in testCases)
        {
            Assert.AreEqual(tc.Result, _calculator.Calculate(tc.Values));
        }
    }
}

internal readonly record struct TestCase(IList<int> Values, double Result);