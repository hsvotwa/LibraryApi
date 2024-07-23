using LibraryApi.Data;
using LibraryApi.Services.Implementations;
using LibraryApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IBookTransactionService, BookTransactionService>();
builder.Services.AddTransient<IBookStatusService, BookStatusService>();
builder.Services.AddTransient<INotificationService, NotificationService>();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
