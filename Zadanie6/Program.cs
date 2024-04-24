namespace Zadanie6;

using System.Data.SqlClient;
using Zadanie6.Repositories;
using Zadanie6.Services;

public class Program
{
    public static readonly string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                                                     throw new Exception(
                                                         "Environment variable DB_CONNECTION_STRING not set\nExiting...");

    public static void Main(string[] args)
    {
        SetupDb();
        SetupApi(args);
    }

    private static void SetupApi(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    private static void SetupDb()
    {
        // setup bazy danych
        var connection = new SqlConnection(ConnectionString);
        connection.Open();

        // dropowanie tabel
        var dropScript = File.ReadAllText("drop.sql").Replace("GO", ";");
        var dropTableCommand = new SqlCommand(dropScript, connection);
        dropTableCommand.ExecuteNonQuery();
        Console.WriteLine("tables dropped");

        // utworzenie tabel ze skryptu create.sql
        var createScript = File.ReadAllText("create.sql").Replace("GO", ";");
        var createTableCommand = new SqlCommand(createScript, connection);
        createTableCommand.ExecuteNonQuery();
        Console.WriteLine("tables created");

        connection.Close();
    }
}