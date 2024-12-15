using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Repository;

public interface IEmployeeCsvRepository
{
    void LoadFromAssembly(string csvResourcePath);
    void LoadFromExecutingDirectory(string csvResourcePath);
    IList<Employee> Get();
}