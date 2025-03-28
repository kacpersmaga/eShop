using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace eShop.Shared.Configuration;

public static class EnvironmentHelpers
{
    public const string TestEnvironmentName = "Test";
    
    public static bool IsTestEnvironment(IWebHostEnvironment env)
    {
        return env.IsEnvironment(TestEnvironmentName);
    }
    
    public static bool IsProductionOrStaging(IWebHostEnvironment env)
    {
        return env.IsProduction() || env.IsStaging();
    }
}