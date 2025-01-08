using System.Globalization;
using System.Reflection;
using System.Resources;
using AllOfTheHackathon.Contracts;
using CsvHelper;
using CsvHelper.Configuration;

namespace AllOfTheHackathon.Repository;

public class EmployeeCsvRepository : IEmployeeCsvRepository
{
    private IList<Employee> _employees = new List<Employee>();
    private readonly CsvConfiguration _csvConfiguration = new(CultureInfo.InvariantCulture) 
    {
        Delimiter = ";"
    };

    public IList<Employee> LoadFromAssembly(string csvResourcePath)
    {
        Stream? resourceStream;
        try
        {
            resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(csvResourcePath);
        }
        catch (Exception e)
        {
            throw new MissingManifestResourceException($"Resource {csvResourcePath} is not found", e);
        }

        if (resourceStream == null)
        {
            throw new MissingManifestResourceException($"resource {csvResourcePath} is not found");
        }
        
        using var reader = new StreamReader(resourceStream);
        using var csvReader = new CsvReader(reader, _csvConfiguration);

        csvReader.Read();
        csvReader.ReadHeader();

        _employees = csvReader.GetRecords<Employee>().ToList();
        return _employees;
    }

    public IList<Employee> LoadFromExecutingDirectory(string csvResourcePath)
    {
        if (!File.Exists(csvResourcePath))
        {
            throw new FileNotFoundException("file with csv is not found", csvResourcePath);
        }
        using var streamReader = new StreamReader(csvResourcePath);
        using var csvReader = new CsvReader(streamReader, _csvConfiguration);

        csvReader.Read();
        csvReader.ReadHeader();

        _employees = csvReader.GetRecords<Employee>().ToList();
        return _employees;
    }

    public IList<Employee> Get()
    {
        return _employees;
    }
}