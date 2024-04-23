using System.Data.SqlClient;

namespace lab7.Services;

public class WarehouseService
{
    private readonly IConfiguration _configuration;

    public WarehouseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool CheckIfProductExists(int IdProduct)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct";
                command.Parameters.AddWithValue("@IdProduct", IdProduct);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
    public bool CheckIfWarehouseExists(int IdWarehouse)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
                command.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }

    public bool CheckIfOrderExists(int IdProduct, int Amount, DateTime CreatedAt)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText =
                    "SELECT COUNT(*) FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
                command.Parameters.AddWithValue("@IdProduct", IdProduct);
                command.Parameters.AddWithValue("@Amount", Amount);
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }

    public bool IsIdOrderInProduct_Warehouse(int IdOrder)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
                command.Parameters.AddWithValue("@IdOrder", IdOrder);

                return (int)command.ExecuteScalar() != 0;
            }
        }
    }

    public int GetOrderId(int IdProduct, int Amount, DateTime CreatedAt)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText =
                    "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt > @CreatedAt ORDER BY CreatedAt DESC";
                command.Parameters.AddWithValue("@IdProduct", IdProduct);
                command.Parameters.AddWithValue("@Amount", Amount);
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);

                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return (int)result; 
                }

                return -1; 
            }
        }
    }

    public void UpdateOrderFulfilledAt(int IdOrder)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder";
                command.Parameters.AddWithValue("@IdOrder", IdOrder);
                command.ExecuteNonQuery();
            }
        }
    }

    public int InsertProductWarehouseRecord(int IdOrder, int IdProduct, int IdWarehouse, int Amount, double totalPrice)
    {
        int productWarehouseId = -1;

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "INSERT INTO Product_Warehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
                command.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);
                command.Parameters.AddWithValue("@IdProduct", IdProduct);
                command.Parameters.AddWithValue("@IdOrder", IdOrder);
                command.Parameters.AddWithValue("@Amount", Amount);
                command.Parameters.AddWithValue("@Price", totalPrice);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now); 

                command.ExecuteNonQuery();

                command.CommandText = "SELECT MAX(IdProductWarehouse) FROM Product_Warehouse";
                var result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    productWarehouseId = Convert.ToInt32(result);
                }
            }
        }

        return productWarehouseId;
    }
    public double GetProductPrice(int IdProduct)
    {
        double productPrice = 0;

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
                command.Parameters.AddWithValue("@IdProduct", IdProduct);

                var result = command.ExecuteScalar();
                if (result != null && double.TryParse(result.ToString(), out double price))
                {
                    productPrice = price;
                }
            }
        }

        return productPrice;
    }
    
}
