using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== SignalR Client ===");

var protocol = Prompt("Protocol (http/https)", "https", s => s is "http" or "https");
var host = Prompt("Server host", "localhost");
var portStr = Prompt("Port", protocol == "https" ? "5001" : "5000", s => int.TryParse(s, out var p) && p > 0 && p < 65536);
var hubPath = Prompt("Hub path", "/discount", s => s.StartsWith("/"));
var ignoreSsl = protocol == "https" && PromptYesNo("Ignore dev SSL certificate?", false);

var url = $"{protocol}://{host}:{portStr}{hubPath}";
Console.WriteLine($"Connecting to: {url}");

var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .AddMessagePackProtocol()
    .WithAutomaticReconnect()
    .Build();

connection.Reconnecting += error =>
{
    Console.WriteLine($"-- Reconnecting... ({error?.Message})");
    return Task.CompletedTask;
};
connection.Reconnected += connectionId =>
{
    Console.WriteLine($"-- Reconnected. ConnectionId={connectionId}");
    return Task.CompletedTask;
};
connection.Closed += error =>
{
    Console.WriteLine($"-- Connection closed. ({error?.Message})");
    return Task.CompletedTask;
};


try
{
    Console.WriteLine("Connecting to SignalR Hub...");
    await connection.StartAsync();
    Console.WriteLine("✅ Connected.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Failed to connect: {ex.Message}");
    return;
}
while (true)
{
    Console.WriteLine();
    Console.WriteLine("Choose an action:");
    Console.WriteLine("  1) Generate codes");
    Console.WriteLine("  2) Use code");
    Console.WriteLine("  q) Quit");
    Console.Write("> ");

    var choice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
    if (choice is "q" or "quit" or "exit") break;

    try
    {
        switch (choice)
        {
            case "1":
            case "g":
            case "generate":
                {
                    var count = PromptUShort("Count (1..2000)", 5, 1, 2000);
                    var length = PromptByte("Length (7..8)", 7, 7, 8);

                    Console.WriteLine($"Invoking Generate(count={count}, length={length})...");
                    var ok = await connection.InvokeAsync<bool>("Generate", count, length, CancellationToken.None);
                    Console.WriteLine($"=> Generate result: {ok}");
                    break;
                }
            case "2":
            case "u":
            case "use":
                {
                    Console.Write("Code (7-8 chars): ");
                    var code = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

                    Console.WriteLine($"Invoking UseCode(code={code})...");
                    var result = await connection.InvokeAsync<byte>("UseCode", code, CancellationToken.None);
                    Console.WriteLine($"=> UseCode result: {result}  (0=Success,1=NotFound,2=AlreadyUsed,3=Invalid,4=UnknownError)");
                    break;
                }
            default:
                Console.WriteLine("Unknown choice.");
                break;
        }
    }
    catch (HubException hx)
    {
        Console.WriteLine($"Hub error: {hx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

try
{
    await connection.StopAsync();
}
finally
{
    await connection.DisposeAsync();
}
Console.WriteLine("Bye.");

static string Prompt(string label, string @default, Func<string, bool>? validate = null)
{
    while (true)
    {
        Console.Write($"{label} [{@default}]: ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return @default;
        if (validate == null || validate(input)) return input;
        Console.WriteLine("Invalid value. Try again.");
    }
}

static bool PromptYesNo(string label, bool @default = false)
{
    var defTxt = @default ? "Y/n" : "y/N";
    while (true)
    {
        Console.Write($"{label} ({defTxt}): ");
        var input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(input)) return @default;
        if (input is "y" or "yes") return true;
        if (input is "n" or "no") return false;
        Console.WriteLine("Please answer y or n.");
    }
}

static ushort PromptUShort(string label, ushort @default, ushort min, ushort max)
{
    while (true)
    {
        Console.Write($"{label} [{@default}]: ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return @default;
        if (ushort.TryParse(input, out var val) && val >= min && val <= max) return val;
        Console.WriteLine($"Enter a number between {min} and {max}.");
    }
}

static byte PromptByte(string label, byte @default, byte min, byte max)
{
    while (true)
    {
        Console.Write($"{label} [{@default}]: ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return @default;
        if (byte.TryParse(input, out var val) && val >= min && val <= max) return val;
        Console.WriteLine($"Enter a number between {min} and {max}.");
    }
}
