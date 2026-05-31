using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using ToDoBackEnd.Data;
using ToDoBackEnd.Interfaces;
using ToDoBackEnd.Services;
using ToDoBackEnd.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region 🔹 CONTROLLERS + VALIDATION

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Custom validation response (instead of default ModelState errors)
        options.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest
            };

            return new BadRequestObjectResult(problem);
        };
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

#endregion

#region 🔹 SWAGGER / API DOCS

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // JWT support in Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region 🔹 DATABASE

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

#endregion

#region 🔹 DEPENDENCY INJECTION (SERVICES)
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<JwtService>();

#endregion

#region 🔹 AUTHENTICATION (JWT)

var jwtKey = builder.Configuration["Jwt:Key"];

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

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            )
        };
    });

builder.Services.AddAuthorization();

#endregion

#region 🔹 CORS (Angular FRONTEND)

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region 🔹 AUTOMAPPER

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

#region 🔹 PIPELINE

// Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

// IMPORTANT ORDER: Auth first, then Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();