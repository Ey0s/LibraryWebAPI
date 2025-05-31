using LibraryWebAPI.Data;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Ensure this is present
using Microsoft.AspNetCore.Diagnostics; // ADD THIS USING DIRECTIVE FOR IExceptionHandlerPathFeature

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>(); // AuthService will now get ILogger injected

// --- CORS Services Configuration ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // This is where your React app typically runs
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});
// --- END CORS Services Configuration ---

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryWebAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add DbContext configuration (using SqlServer as per your appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // === START GLOBAL ERROR HANDLING ENHANCEMENT (PRODUCTION ONLY) ===
    // Use a global exception handler in production
    app.UseExceptionHandler("/error"); // Redirects to a custom /error endpoint
    app.UseHsts(); // Recommended for production HTTP Strict Transport Security
    // === END GLOBAL ERROR HANDLING ENHANCEMENT ===
}

app.UseHttpsRedirection();

// --- CORS Middleware Configuration ---
// IMPORTANT: Place this BEFORE UseAuthentication() and UseAuthorization()
app.UseCors();
// --- END CORS Middleware Configuration ---

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// === START GLOBAL ERROR HANDLING ENDPOINT ===
// Define the /error endpoint that UseExceptionHandler redirects to
app.Map("/error", async (HttpContext context) =>
{
    // Retrieve the exception details
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;

    // Get the logger (Program is used here as a general logger, or you could create a specific one)
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Log the exception details
    if (exception != null)
    {
        logger.LogError(exception, "An unhandled exception occurred at: {Path}", context.Request.Path);
    }
    else
    {
        logger.LogError("An unhandled exception occurred, but no exception details were found.");
    }

    // Return a generic error response to the client
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new
    {
        Title = "An unexpected error occurred.",
        Status = StatusCodes.Status500InternalServerError,
        Detail = app.Environment.IsDevelopment() ? exception?.Message : "Please try again later."

    });
});


app.Run();