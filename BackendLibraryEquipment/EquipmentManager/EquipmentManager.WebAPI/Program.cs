using EquipmentManager.Application.Contracts;
using EquipmentManager.Application.Services;
using EquipmentManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Dependency injection
// Repositories
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<ILocationsRepository, LocationsRepository>();
builder.Services.AddScoped<IMaintenanceRecordsRepository, MaintenanceRecordsRepository>();
builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
builder.Services.AddScoped<ITechniciansRepository, TechniciansRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

// Services
builder.Services.AddScoped<CategoriesService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<LocationsService>();
builder.Services.AddScoped<MaintenanceRecordsService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<TechniciansService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<UserSettingsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:8443")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("VueClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
