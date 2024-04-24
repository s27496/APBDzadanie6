using Zadanie6.Dto;
using Zadanie6.Repositories;

namespace Zadanie6.Services;

public interface IWarehouseService
{
    public Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto);
}

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto)
    {
        Result result = await _warehouseRepository.RegisterProductInWarehouseAsync(
            idWarehouse: dto.IdWarehouse!.Value,
            idProduct: dto.IdProduct!.Value,
            amount: dto.Amount!.Value,
            createdAt: dto.CreatedAt!.Value);

        if (result.ErrorMessage != "") throw new Exception("Error: " + result.ErrorMessage);
        return result.IdProductWarehouse;
    }
}