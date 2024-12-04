using AllOfTheHackathon.Contracts;

namespace AllOfTheHackathon.Repository;

public interface IEmployeeCsvRepository
{
    void Load(string xmlResourcePath);
    IList<Employee> Get();
}