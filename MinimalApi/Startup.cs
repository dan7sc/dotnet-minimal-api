using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infra.Database;


namespace MinimalApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").GetSection("Key").ToString()
            ?? throw new ArgumentException("Key cannot be null");
    }

    private readonly string key;

    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert your JWT token here"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
        },
            });
        });

        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administrators
            string GenerateJwtToken(Administrator administrator)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Profile", administrator.Profile),
        new Claim(ClaimTypes.Role, administrator.Profile),
    };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
            {
                var administrator = administratorService.Login(loginDTO);
                if (administrator != null)
                {
                    string token = GenerateJwtToken(administrator);
                    return Results.Ok(new LoggedAdministrator
                    {
                        Email = administrator.Email,
                        Profile = administrator.Profile,
                        Token = token,
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administrators");

            endpoints.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
            {
                var administrators = new List<AdministratorModelView>();
                var allAdministrators = administratorService.All(page);
                foreach (var adm in allAdministrators)
                {
                    administrators.Add(new AdministratorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Profile = adm.Profile,
                    });
                }
                return Results.Ok(administrators);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            endpoints.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
            {
                var administrator = administratorService.FindById(id);

                if (administrator == null) return Results.NotFound();
                return Results.Ok(new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile,
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            endpoints.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
            {
                var validations = new ValidationError
                {
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(administratorDTO.Email))
                {
                    validations.Messages.Add("Email cannot be empty");
                }

                if (string.IsNullOrEmpty(administratorDTO.Password))
                {
                    validations.Messages.Add("Password cannot be empty");
                }

                if (administratorDTO.Profile == null)
                {
                    validations.Messages.Add("Profile cannot be empty");
                }

                if (validations.Messages.Count > 0)
                {
                    return Results.BadRequest(validations.Messages);
                }

                var administrator = new Administrator
                {
                    Email = administratorDTO.Email,
                    Password = administratorDTO.Password,
                    Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString(),
                };
                administratorService.Insert(administrator);

                return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile,
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

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

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.All(page: page);

                return Results.Ok(vehicles);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);

                if (vehicle == null) return Results.NotFound();
                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.FindById(id);
                if (vehicle == null) return Results.NotFound();

                vehicleService.Remove(vehicle);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");
            #endregion
        });
    }
}