using LibraryApi.Data;
using LibraryApi.Entities;
using LibraryApi.Services.Implementations;
using LibraryApi.Services.Interfaces;
using LibraryApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IBookTransactionService, BookTransactionService>();
builder.Services.AddTransient<IBookStatusService, BookStatusService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IAccountService, AccountService>();

string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'ConnectionString' was not found.");
}

builder.Services.AddDbContext<LibraryContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName)
    );
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<LibraryContext>()
    .AddDefaultTokenProviders();

byte[] key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAutoMapper(typeof(LibraryApiMappingProfile).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
