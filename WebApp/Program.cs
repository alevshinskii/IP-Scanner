using System.Text.Json;
using LoggerLib;
using ScannerLib;
using ILogger = LoggerLib.ILogger;

StartScanService();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void StartScanService()
{
    var scanner = new NetScanner(new List<ILogger>(){new LocalLogger(new KuznechikCypher())});
    try
    {
        using (FileStream fs = new FileStream("scansettings.json", FileMode.OpenOrCreate))
        {
            scanner.Settings = JsonSerializer.Deserialize<ScannerSettings>(fs);
        }
    }
    catch
    {
        //settings not found
    }

    if (scanner.Settings != null)
        Task.Run(() => { autoscanStart(scanner); });
}

void autoscanStart(NetScanner scanner)
{
    
}
