using GeneralServices.DB;
using Microsoft.EntityFrameworkCore;
using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddEnvironmentVariables("twitch_");

builder.Services.AddWatchDogServices(opt =>
{
    opt.SetExternalDbConnString = builder.Configuration.GetConnectionString("DefaultConnection");
    opt.DbDriverOption = WatchDogDbDriverEnum.PostgreSql;
});

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDBContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Configuration.GetSection("redis").Bind(UtilsLib.Config.redis);
app.Configuration.GetSection("watchdog").Bind(UtilsLib.Config.watchdog);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWatchDogExceptionLogger();

app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = UtilsLib.Config.watchdog.username;
    opt.WatchPagePassword = UtilsLib.Config.watchdog.password;
});

app.Run();
