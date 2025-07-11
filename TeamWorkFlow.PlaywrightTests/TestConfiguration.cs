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
        Email = GetSecureValue("TestUsers:AdminUser:Email", "TEST_ADMIN_EMAIL", "fake.admin@test.local"),
        Password = GetSecureValue("TestUsers:AdminUser:Password", "TEST_ADMIN_PASSWORD", "FakeAdminPass123!"),
        FirstName = _configuration["TestUsers:AdminUser:FirstName"] ?? "Fake",
        LastName = _configuration["TestUsers:AdminUser:LastName"] ?? "Admin"
    };

    public TestUser OperatorUser => new()
    {
        Email = GetSecureValue("TestUsers:OperatorUser:Email", "TEST_OPERATOR_EMAIL", "fake.operator@test.local"),
        Password = GetSecureValue("TestUsers:OperatorUser:Password", "TEST_OPERATOR_PASSWORD", "FakeOperatorPass456!"),
        FirstName = _configuration["TestUsers:OperatorUser:FirstName"] ?? "Fake",
        LastName = _configuration["TestUsers:OperatorUser:LastName"] ?? "Operator"
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

    /// <summary>
    /// Gets a secure value from environment variables first, then configuration, then fallback
    /// </summary>
    /// <param name="configKey">Configuration key</param>
    /// <param name="envVarName">Environment variable name</param>
    /// <param name="fallback">Fallback value</param>
    /// <returns>The secure value</returns>
    private string GetSecureValue(string configKey, string envVarName, string fallback)
    {
        // First try environment variable
        var envValue = Environment.GetEnvironmentVariable(envVarName);
        if (!string.IsNullOrEmpty(envValue))
        {
            return envValue;
        }

        // Then try configuration
        var configValue = _configuration[configKey];
        if (!string.IsNullOrEmpty(configValue) &&
            !configValue.StartsWith("PLACEHOLDER_", StringComparison.OrdinalIgnoreCase))
        {
            return configValue;
        }

        // Finally use fallback
        return fallback;
    }
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
