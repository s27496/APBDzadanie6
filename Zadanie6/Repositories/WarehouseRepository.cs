using System.Data;
using System.Data.SqlClient;

namespace Zadanie6.Repositories;

public interface IWarehouseRepository
{
    public Task<Result> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int amount, DateTime createdAt);
    public Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt);
}

public record Result
{
    public required int IdProductWarehouse { get; init; }
    public required string ErrorMessage { get; init; }
}

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Result> RegisterProductInWarehouseAsync(int idWarehouse, int idProduct, int amount,
        DateTime createdAt)
    {
        await using var connection = new SqlConnection(Program.ConnectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var command = new SqlCommand("", connection);
        command.Transaction = (SqlTransaction)transaction;

        try
        {
            // part 1 - check if IdWarehouse and IdProduct are valid
            var price = CheckParameters(command, idWarehouse, idProduct).Result;

            // part 2 and 3 - check if order exists
            const string queryGetOrderId2 =
                "SELECT TOP 1 o.IdOrder FROM \"Order\" o LEFT JOIN Product_Warehouse pw ON o.IdOrder = pw.IdOrder WHERE o.IdProduct = @IdProduct AND o.Amount = @Amount AND pw.IdProductWarehouse IS NULL AND o.CreatedAt < @CreatedAt;";
            command.CommandText = queryGetOrderId2;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            var result = await command.ExecuteScalarAsync();

            if (result == null) throw new Exception("There is no order to fulfill");
            var idOrder = (int)result;

            // part 4 - update order fulfilled time
            const string queryUpdateOrder = "UPDATE \"Order\" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            command.CommandText = queryUpdateOrder;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();

            // part 5 and 6 - insert into Product_Warehouse
            command.CommandText = @"
                      INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                      OUTPUT Inserted.IdProductWarehouse
                      VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Cost, @CreatedAt);";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Cost", amount * price);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            result = await command.ExecuteScalarAsync();
            if (result == null) throw new Exception("Failed to insert into Product_Warehouse");

            await transaction.CommitAsync();
            return new Result
            {
                IdProductWarehouse = (int)result,
                ErrorMessage = ""
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new Result
            {
                IdProductWarehouse = -1,
                ErrorMessage = e.Message
            };
        }
    }

    private async Task<decimal> CheckParameters(SqlCommand command, int idWarehouse,
        int idProduct)
    {
        // check if product with given id exists
        const string queryCheckProductId = "SELECT Product.Price FROM Product WHERE IdProduct = @IdProduct";
        command.CommandText = queryCheckProductId;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        var result = await command.ExecuteScalarAsync();

        if (result == null) throw new Exception("Product with given id does not exist");
        var price = (decimal)result;

        // check if warehouse with given id exists
        const string queryCheckWarehouseId = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        command.CommandText = queryCheckWarehouseId;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        result = await command.ExecuteScalarAsync();

        if (result == null) throw new Exception("CheckWarehouseId query failed");
        if ((int)result == 0) throw new Exception("Warehouse with given id does not exist");

        return price;
    }

    public async Task RegisterProductInWarehouseByProcedureAsync(int idWarehouse, int idProduct, DateTime createdAt)
    {
        await using var connection = new SqlConnection(Program.ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("AddProductToWarehouse", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("IdWarehouse", idWarehouse);
        command.Parameters.AddWithValue("Amount", 0);
        command.Parameters.AddWithValue("CreatedAt", createdAt);
        await command.ExecuteNonQueryAsync();
    }
}