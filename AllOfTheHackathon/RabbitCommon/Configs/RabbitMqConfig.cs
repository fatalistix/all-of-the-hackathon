namespace EmployeeRabbitService.Configs;

public class RabbitMqConfig
{
    private const string HostVariable = "RABBIT_MQ_HOST";
    private const string UsernameVariable = "RABBIT_MQ_USERNAME";
    private const string PasswordVariable = "RABBIT_MQ_PASSWORD";

    public readonly string Host = Environment.GetEnvironmentVariable(HostVariable) ?? throw new InvalidOperationException($"No {HostVariable} variable");

    public readonly string Username = Environment.GetEnvironmentVariable(UsernameVariable) ??
                                              throw new InvalidOperationException(
                                                  $"No {UsernameVariable} variable");

    public readonly string Password = Environment.GetEnvironmentVariable(PasswordVariable) ?? throw new InvalidOperationException($"No {PasswordVariable} variable");
}