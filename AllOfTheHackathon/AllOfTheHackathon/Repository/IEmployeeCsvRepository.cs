using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Repository;

public interface IEmployeeCsvRepository
{
    IList<Employee> LoadFromAssembly(string csvResourcePath);
    IList<Employee> LoadFromExecutingDirectory(string csvResourcePath);
    [Obsolete("This method was used in combination with 2 upper")]
    IList<Employee> Get();
}