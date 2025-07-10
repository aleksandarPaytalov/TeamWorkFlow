using Microsoft.Extensions.Configuration;

namespace TeamWorkFlow.PlaywrightTests;

public class TestConfiguration
{
    private static TestConfiguration? _instance;
    private readonly IConfiguration _configuration;

    private TestConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    public static TestConfiguration Instance => _instance ??= new TestConfiguration();

    public string BaseUrl => _configuration["TestSettings:BaseUrl"] ?? "https://localhost:7015";
    public int Timeout => int.Parse(_configuration["TestSettings:Timeout"] ?? "30000");
    public int DefaultWaitTime => int.Parse(_configuration["TestSettings:DefaultWaitTime"] ?? "5000");
    public bool ScreenshotOnFailure => bool.Parse(_configuration["TestSettings:ScreenshotOnFailure"] ?? "true");
    public bool VideoOnFailure => bool.Parse(_configuration["TestSettings:VideoOnFailure"] ?? "true");
    public int SlowMo => int.Parse(_configuration["TestSettings:SlowMo"] ?? "0");

    public TestUser AdminUser => new()
    {
        Email = _configuration["TestUsers:AdminUser:Email"] ?? "admin@teamworkflow.com",
        Password = _configuration["TestUsers:AdminUser:Password"] ?? "Admin123!",
        FirstName = _configuration["TestUsers:AdminUser:FirstName"] ?? "Admin",
        LastName = _configuration["TestUsers:AdminUser:LastName"] ?? "User"
    };

    public TestUser OperatorUser => new()
    {
        Email = _configuration["TestUsers:OperatorUser:Email"] ?? "operator@teamworkflow.com",
        Password = _configuration["TestUsers:OperatorUser:Password"] ?? "Operator123!",
        FirstName = _configuration["TestUsers:OperatorUser:FirstName"] ?? "Operator",
        LastName = _configuration["TestUsers:OperatorUser:LastName"] ?? "User"
    };

    public TestData SampleTask => new()
    {
        Name = _configuration["TestData:SampleTask:Name"] ?? "Test Task - Automated",
        Description = _configuration["TestData:SampleTask:Description"] ?? "This is a test task created by automated tests",
        ProjectNumber = _configuration["TestData:SampleTask:ProjectNumber"] ?? "TEST001"
    };

    public TestData SampleProject => new()
    {
        Name = _configuration["TestData:SampleProject:Name"] ?? "Test Project - Automated",
        ProjectNumber = _configuration["TestData:SampleProject:ProjectNumber"] ?? "PROJ001",
        TotalHoursSpent = _configuration["TestData:SampleProject:TotalHoursSpent"] ?? "40"
    };

    public TestData SampleMachine => new()
    {
        Name = _configuration["TestData:SampleMachine:Name"] ?? "Test Machine - Automated",
        Capacity = _configuration["TestData:SampleMachine:Capacity"] ?? "100",
        ImageUrl = _configuration["TestData:SampleMachine:ImageUrl"] ?? "https://example.com/test-machine.jpg"
    };
}

public class TestUser
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class TestData
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public string TotalHoursSpent { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
