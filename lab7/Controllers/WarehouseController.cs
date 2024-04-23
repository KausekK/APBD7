using System.Data.SqlClient;
using lab7.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace lab7.Controllers;
[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : ControllerBase
{
    
    private readonly IConfiguration _configuration;
    public WarehouseController(IConfiguration configuration)
{
    _configuration = configuration;
}


    [HttpGet("{IdProduct}")]
    public IEnumerable<ProductDTO> isProductExist(int IdProduct)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        var reader = command.ExecuteReader();
        List<ProductDTO> products = new List<ProductDTO>();
        while (reader.Read())
        {
            var product = new ProductDTO
            {
                IdProduct = (int)reader["IdProduct"],
                Name  = reader["Name"].ToString(),
                Description =  reader["Description"].ToString(),
                Price =  (double) reader["Price"]
            };
            products.Add(product);
        }

        if (IdProduct )
        {
            
        }
        connection.Close();
        return products;
    }



    [HttpPost]
        public IActionResult AddProductToWarehouse(ProductWarehouseDTO productWarehouseDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
                connection.Open();

                using SqlCommand command = new SqlCommand();
                command.Connection = connection;
                

                    return Ok("Product added to warehouse successfully");
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
}