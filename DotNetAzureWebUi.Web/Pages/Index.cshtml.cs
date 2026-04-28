using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetAzureWebUi.Web.Pages;

public class IndexModel : PageModel
{
    public string AppName { get; } = "DotNet Azure Web UI";
    public string EnvironmentName { get; private set; } = "Unknown";

    public List<FeatureCard> Features { get; } =
    [
        new FeatureCard(
            "Azure DevOps Ready",
            "This project includes a clean structure that can be pushed into Azure Repos and built with Azure Pipelines.",
            "CI/CD"),

        new FeatureCard(
            "Web UI Visible",
            "The Razor Pages frontend gives you a real browser-based user interface instead of only an API.",
            "Frontend"),

        new FeatureCard(
            "Cloud Deployable",
            "You can deploy this same application to Azure App Service and access it from a public URL.",
            "Azure"),

        new FeatureCard(
            "SRE Friendly",
            "The /api/health endpoint can be used for smoke testing, uptime checks, and pipeline validation.",
            "Health Check")
    ];

    public void OnGet()
    {
        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    }
}

public record FeatureCard(string Title, string Description, string Badge);
