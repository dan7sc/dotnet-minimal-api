using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infra.Database;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login feito com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administrators");
#endregion

#region Vehicles
ValidationError validationsDTO(VehicleDTO vehicleDTO)
{
    var validations = new ValidationError
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))
    {
        validations.Messages.Add("Name cannot be empty");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Make))
    {
        validations.Messages.Add("Make cannot be empty");
    }

    if (vehicleDTO.Year < 1950)
    {
        validations.Messages.Add("Year should be bigger than 1950");
    }

    return validations;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validations = validationsDTO(vehicleDTO);
    if (validations.Messages.Count > 0)
    {
        return Results.BadRequest(validations.Messages);
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Make = vehicleDTO.Make,
        Year = vehicleDTO.Year,
    };
    vehicleService.Insert(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.All(page: page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    var validations = validationsDTO(vehicleDTO);
    if (validations.Messages.Count > 0)
    {
        return Results.BadRequest(validations.Messages);
    }

    vehicle.Name = vehicleDTO.Name;
    vehicle.Make = vehicleDTO.Make;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Remove(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion