using System.Globalization;
using System.Reflection;
using System.Resources;
using AllOfTheHackathon.Contracts;
using CsvHelper;
using CsvHelper.Configuration;

namespace AllOfTheHackathon.Repository;

public class EmployeeCsvRepository
{
    private IList<Employee> _employees = new List<Employee>();

    public void Load(string xmlResourcePath)
    {
        Stream? resourceStream;
        try
        {
            resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlResourcePath);
        }
        catch (Exception e)
        {
            throw new MissingManifestResourceException($"Resource {xmlResourcePath} is not found", e);
        }

        if (resourceStream == null)
        {
            throw new MissingManifestResourceException($"resource {xmlResourcePath} is not found");
        }

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        using var reader = new StreamReader(resourceStream);
        using var csvReader = new CsvReader(reader, csvConfiguration);

        csvReader.Read();
        csvReader.ReadHeader();

        _employees = csvReader.GetRecords<Employee>().ToList();
    }

    public IList<Employee> Get()
    {
        return _employees;
    }
}