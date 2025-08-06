// SignalR_DbMonitor_ConsoleApp (All-in-one Program.cs Style)
#region Using Direktifleri
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
#endregion

#region Konfigürasyon ve DI Servisleri
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => { builder.Configuration.GetSection("Kestrel").Bind(options); });

var oracleConnectionString = builder.Configuration.GetConnectionString("OracleDb") ??
    throw new InvalidOperationException("Oracle connection string not configured.");

builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<PatientQueueDbContext>(options =>
    options.UseOracle(oracleConnectionString).LogTo(Console.WriteLine, LogLevel.Error)
);
builder.Services.AddSingleton<ServerSentEventsClientManager>();
builder.Services.AddHostedService<DatabaseChangeMonitorService>();
builder.Services.AddLogging();
builder.Services.Configure<StartupOptions>(builder.Configuration.GetSection("Startup"));
#endregion

#region Uygulama Oluşturma ve Middleware
var app = builder.Build();
app.UseStaticFiles();
#endregion

#region Konfigürasyon Erişimi ve Başlangıç İşlemleri
var startupOptions = app.Services.GetRequiredService<IOptions<StartupOptions>>().Value;
if (startupOptions.RunRawDbCheck)
    await VerifyRawOracleConnectionAsync();
#endregion

#region Endpoint Tanımlamaları
app.MapHub<QueueHub>(startupOptions.SignalRHub);
app.MapGet("/health", async (IServiceProvider sp) =>
{
    var health = new Dictionary<string, object>();
    // Veritabanı kontrolü
    try
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PatientQueueDbContext>();
        var any = await db.QueueStatuses.AnyAsync();
        health["Database"] = any ? "Healthy" : "No Data";
    }
    catch (Exception ex)
    {
        health["Database"] = $"Unhealthy: {ex.Message}";
    }

    // SignalR kontrolü (örnek: hub servisi var mı)
    try
    {
        var hub = sp.GetService<QueueHub>();
        health["SignalR"] = hub != null ? "Healthy" : "Not Registered";
    }
    catch (Exception ex)
    {
        health["SignalR"] = $"Unhealthy: {ex.Message}";
    }

    // SSE kontrolü (örnek: client manager var mı)
    try
    {
        var sse = sp.GetService<ServerSentEventsClientManager>();
        health["SSE"] = sse != null ? "Healthy" : "Not Registered";
    }
    catch (Exception ex)
    {
        health["SSE"] = $"Unhealthy: {ex.Message}";
    }

    return Results.Json(health);
});

app.MapGet(startupOptions.SseEvents, async (
    HttpContext context,
    ServerSentEventsClientManager clientManager,
    ILogger<Program> logger) =>
{
    logger.LogInformation("New SSE connection received at {Time}", DateTime.Now);
    context.Response.Headers.TryAdd("Content-Type", "text/event-stream");
    context.Response.Headers.TryAdd("Cache-Control", "no-cache");
    context.Response.Headers.TryAdd("Connection", "keep-alive");
    var response = context.Response;
    var clientId = Guid.NewGuid();

    var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(startupOptions.ChannelCapacity)
    {
        SingleReader = true,
        SingleWriter = true,
        AllowSynchronousContinuations = true
    });

    await clientManager.AddClientAsync(clientId, channel);

    try
    {
        await foreach (var message in channel.Reader.ReadAllAsync(context.RequestAborted))
        {
            await response.WriteAsync($"data: {message}\n\n");
            await response.Body.FlushAsync();
        }
    }
    catch (OperationCanceledException)
    {
        logger.LogInformation("SSE client {ClientId} disconnected.", clientId);
    }
    finally
    {
        await clientManager.RemoveClientAsync(clientId);
        logger.LogInformation("SSE client {ClientId} removed.", clientId);
    }
});

app.Logger.LogInformation("SignalR is running at http://localhost:1771/hub");
app.Logger.LogInformation("SSE endpoint is running at http://localhost:1771/events");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
#endregion

#region Ana Fonksiyonlar
async Task VerifyRawOracleConnectionAsync()
{
    try
    {
        await using var connection = new OracleConnection(oracleConnectionString);
        await connection.OpenAsync();

        const string sql =
            "select UNITNAME,RESPONSIBLESTAFF,LASTCALLEDTICKETNUMBER,LASTCALLEDPATIENTNAME, LASTCALLEDTICKEDTIME from HBYS.VW_LATEST_QUEUE";
        await using var command = new OracleCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[RAW DB TEST] ✅ SYSDATE sonucu: {result}");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[RAW DB TEST] ❌ Bağlantı hatası: {ex.Message}");
    }
    finally
    {
        Console.ResetColor();
    }
}
#endregion

#region Domain Sınıfları ve Record’lar
internal record QueueStatus(
    string UnitName,
    string ResponsibleStaffGender,
    string ResponsibleStaff,
    string LastCalledTicketNumber,
    string LastCalledPatientName,
    DateTime LastCalledTickedTime);

internal record QueueUpdateDto(
    string UnitName,
    string? LastCalledTicketNumber,
    string? LastCalledPatientName,
    DateTime LastCalledTickedTime);
#endregion

#region Servis Sınıfları
internal class PatientQueueDbContext(DbContextOptions<PatientQueueDbContext> options) : DbContext(options)
{
    public DbSet<QueueStatus> QueueStatuses => Set<QueueStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QueueStatus>(entity =>
        {
            entity.HasKey(e => new { e.UnitName });
            entity.ToView("VW_LATEST_QUEUE", "HBYS");
            entity.Property(e => e.UnitName).HasColumnName("UNITNAME");
            entity.Property(e => e.ResponsibleStaffGender).HasColumnName("RESPONSIBLESTAFFGENDER");
            entity.Property(e => e.ResponsibleStaff).HasColumnName("RESPONSIBLESTAFF");
            entity.Property(e => e.LastCalledTicketNumber).HasColumnName("LASTCALLEDTICKETNUMBER");
            entity.Property(e => e.LastCalledPatientName).HasColumnName("LASTCALLEDPATIENTNAME");
            entity.Property(e => e.LastCalledTickedTime).HasColumnName("LASTCALLEDTICKEDTIME");
        });
    }
}

internal class QueueHub(IServiceProvider provider) : Hub
{
    public async Task<List<QueueStatus>> GetInitialState()
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PatientQueueDbContext>();
        return await db.QueueStatuses.AsNoTracking().ToListAsync();
    }

    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }
}

internal class ServerSentEventsClientManager
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _clients = new();

    public async Task AddClientAsync(Guid clientId, Channel<string> channel)
    {
        await Task.Run(() => _clients.TryAdd(clientId, channel));
    }

    public async Task RemoveClientAsync(Guid clientId)
    {
        await Task.Run(() => _clients.TryRemove(clientId, out _));
    }

    public async Task BroadcastMessageAsync(object payload)
    {
        var serializedPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            //Türkçe Karakterlerin json mesajı içerisinde orijinal karakter ile gösterilmesi için
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        var broadcastTasks = _clients.Values.Select(async channel =>
        {
            try
            {
                await channel.Writer.WriteAsync(serializedPayload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Broadcast failed: {ex.Message}");
            }
        });

        await Task.WhenAll(broadcastTasks);
    }
}

internal class DatabaseChangeMonitorService(
    IServiceProvider serviceProvider,
    IHubContext<QueueHub> hubContext,
    ServerSentEventsClientManager clientManager,
    ILogger<DatabaseChangeMonitorService> logger,
    IOptions<StartupOptions> startupOptions) : BackgroundService
{
    private readonly int _pollingInterval = startupOptions.Value.PollingInterval;
    
    private DateTime _lastPollTime = DateTime.MinValue;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"logging interval :{_pollingInterval}");
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PatientQueueDbContext>();

            var latestEntries = await dbContext.QueueStatuses
                .Where(x => x.LastCalledTickedTime > _lastPollTime)
                .AsNoTracking()
                .ToListAsync(stoppingToken);

            foreach (var entry in latestEntries)
            {
                var updateDto = new QueueUpdateDto(
                    entry.UnitName,
                    entry.LastCalledTicketNumber,
                    entry.LastCalledPatientName,
                    entry.LastCalledTickedTime);

                logger.LogInformation("Change detected in {Unit} at {Time}.", entry.UnitName, DateTime.Now);

                await hubContext.Clients.All.SendAsync("QueueUpdate", updateDto, stoppingToken);
                await clientManager.BroadcastMessageAsync(updateDto);
            }

            if (latestEntries.Any()) _lastPollTime = latestEntries.Max(x => x.LastCalledTickedTime);

            await Task.Delay(TimeSpan.FromSeconds(_pollingInterval), stoppingToken);
        }
    }
}

internal class StartupOptions
{
    public bool RunRawDbCheck { get; set; } = true;
    public int PollingInterval { get; set; } = 10;
    public int ChannelCapacity { get; set; } = 17;
    public string SignalRHub { get; set; } = "/hub";
    public string SseEvents { get; set; } = "/events";
}
#endregion

#region Enums
internal enum StaffGender
{
    Male, //E
    Female //K
}

internal enum HealthStatus
{
    Healthy,
    Unhealthy,
    NoData
}
#endregion