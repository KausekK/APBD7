using System.Data.SqlClient;
using lab7.Models.DTO;
using lab7.Services;
using Microsoft.AspNetCore.Mvc;

namespace lab7.Controllers;
[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : ControllerBase
{
    
    private readonly WarehouseService _warehouseService;

    public WarehouseController(WarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost("AddProductToWarehouse")]
    public IActionResult AddProductToWarehouse([FromBody] ProductWarehouseDTO productWarehouse)
    {
        if (productWarehouse.IdProduct <= 0 || productWarehouse.IdWarehouse <= 0 || productWarehouse.Amount <= 0)
        {
            return BadRequest("IdProduct, IdWarehouse, and Amount should be greater than 0");
        }
        if (productWarehouse.CreatedAt >= DateTime.Now)
        {
            return BadRequest("CreatedAt should be earlier than the current date and time");
        }

        bool productExists = _warehouseService.CheckIfProductExists(productWarehouse.IdProduct);
        bool warehouseExists = _warehouseService.CheckIfWarehouseExists(productWarehouse.IdWarehouse);

        if (!productExists)
        {
            return NotFound("Product with the provided Id does not exist");
        }
        if (!warehouseExists)
        {
            return NotFound("Warehouse with the provided Id does not exist");
        }

      
        
        if (_warehouseService.CheckIfOrderExists(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt))
        {
            return BadRequest("Order does not exist");
        }
        
        if (_warehouseService.GetOrderId(productWarehouse.IdProduct, productWarehouse.Amount,
                productWarehouse.CreatedAt) == -1)
        {
            return BadRequest("Order does not exist");

        }
        var orderId = _warehouseService.GetOrderId(productWarehouse.IdProduct, productWarehouse.Amount,
            productWarehouse.CreatedAt);
       
        if (_warehouseService.IsIdOrderInProduct_Warehouse(orderId))
        {
            return BadRequest("The order has already been fulfilled");
        }
        _warehouseService.UpdateOrderFulfilledAt(orderId);

        double productPrice = _warehouseService.GetProductPrice(productWarehouse.IdProduct);
        double totalPrice = productPrice * productWarehouse.Amount;

        _warehouseService.InsertProductWarehouseRecord(orderId, productWarehouse.IdProduct, productWarehouse.IdWarehouse, productWarehouse.Amount, totalPrice);

        return Ok("Product added to warehouse");
    }
}