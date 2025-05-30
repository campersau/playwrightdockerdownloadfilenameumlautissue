using Microsoft.Net.Http.Headers;
using Microsoft.Playwright;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("""<a href="/download2">Download</a>""");
});

app.MapGet("/download2", async context =>
{
    var contentDisposition = new ContentDispositionHeaderValue("attachment"); // { FileNameStar = "testäöü" };
    contentDisposition.SetHttpFileName("testäöü.txt");

    context.Response.ContentType = "text/plain";
    context.Response.GetTypedHeaders().ContentDisposition = contentDisposition;

    await context.Response.WriteAsync("Hello World!");
});

var task = app.RunAsync();


var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync();
var browserContext = await browser.NewContextAsync(new() { BaseURL = "http://localhost:5000" });
var page = await browserContext.NewPageAsync();

{
    var response = await page.GotoAsync("/");
    var body = await response!.BodyAsync();

    Console.WriteLine("Goto HTML: " + Encoding.UTF8.GetString(body));
}
{
    var download = await page.RunAndWaitForDownloadAsync(async () =>
    {
        var response = await page.RunAndWaitForResponseAsync(async () =>
        {
            await page.Locator("a").ClickAsync();
        }, response => response.Url.Contains("/download2"));

        foreach (var header in response!.Headers)
        {
            Console.WriteLine("Download Header: " + header.Key + " = " + header.Value);
        }
    });

    Console.WriteLine("Download URL: " + download.Url);
    Console.WriteLine("Download SuggestedFilename: " + download.SuggestedFilename);
}

await task;
