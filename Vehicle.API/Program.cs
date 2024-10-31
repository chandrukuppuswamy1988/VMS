using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using Vehicle.API.Prrofile;
using Vehicle.API.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(o =>
    o.Filters.Add(new AuthorizeFilter("fullaccess")));

builder.Services.AddControllers();

builder.Services.AddTransient<IBusesRepository, BusesRepository>();

builder.Services.AddDbContext<VehicleDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleDB"));
});

builder.Services.AddAutoMapper(typeof(BusProfile));


builder.Services.AddSwaggerGen(o =>
{

    //Addes Title and Description for the API Swagger
    o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Vehicle API",
        Description = "Through this API you can access Vehicle."
    });


    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    /// addes the comments to swagger methods.also check output of xml in properties setting of the projects
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    o.IncludeXmlComments(xmlCommentsFullPath);
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = "https://localhost:5001";
        o.Audience = "globoapi";
        o.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("fullaccess", p =>
        p.RequireClaim(JwtClaimTypes.Scope, "globoapi_fullaccess"));


});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

// This middleware serves the Swagger documentation UI
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle.API");


});


app.Run();
