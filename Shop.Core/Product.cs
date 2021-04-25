using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Shop.Core
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ProductType Type { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public static int Create(Product product)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    insert into Products (Title, Type, Price, IsDeleted) values ($title, $type, $price, $isDeleted);
                    select last_insert_rowid();
                ";
                command.Parameters.AddWithValue("$title", product.Title);
                command.Parameters.AddWithValue("$type", (int) product.Type);
                command.Parameters.AddWithValue("$price", product.Price);
                command.Parameters.AddWithValue("$isDeleted", product.IsDeleted);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public static List<Product> FindAll()
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    select Id, Title, Type, Price, IsDeleted from Products;
                ";
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Product>();
                    while (reader.Read())
                        result.Add(Read(reader));
                    return result;
                }
            }
        }

        public static Product TryGetById(int id)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    select Id, Title, Type, Price, IsDeleted from Products where Id = $id;
                ";
                command.Parameters.AddWithValue("$id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;
                    return Read(reader);
                }
            }
        }

        public static bool TryUpdate(Product product)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    update Products set Title = $title, Type = $type, Price = $price, IsDeleted = $isDeleted where Id = $id;
                    select changes();
                ";
                command.Parameters.AddWithValue("$id", product.Id);
                command.Parameters.AddWithValue("$title", product.Title);
                command.Parameters.AddWithValue("$type", (int) product.Type);
                command.Parameters.AddWithValue("$price", product.Price);
                command.Parameters.AddWithValue("$isDeleted", product.IsDeleted);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static Product Read(SqliteDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Type = (ProductType) reader.GetInt32(2),
                Price = reader.GetDecimal(3),
                IsDeleted = reader.GetBoolean(4)
            };
        }
    }

    public enum ProductType
    {
        Other,
        Food,
        HouseholdChemicals,
        Electronics,
        Toys
    }
}
