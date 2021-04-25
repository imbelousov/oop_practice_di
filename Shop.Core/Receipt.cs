using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Shop.Core
{
    public class Receipt
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public static int Create(Receipt receipt)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    insert into Receipts (Date) values ($date);
                    select last_insert_rowid();
                ";
                command.Parameters.AddWithValue("$date", receipt.Date.ToBinary());
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public static void AddItem(int receiptId, ReceiptItem item)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    insert into ReceiptsProducts (ReceiptId, ProductId, Price) values ($receiptId, $productId, $price);
                ";
                command.Parameters.AddWithValue("$receiptId", receiptId);
                command.Parameters.AddWithValue("$productId", item.ProductId);
                command.Parameters.AddWithValue("$price", item.Price);
                command.ExecuteNonQuery();
            }
        }

        public static List<Receipt> FindAll()
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    select Id, Date from Receipts;
                ";
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Receipt>();
                    while (reader.Read())
                        result.Add(Read(reader));
                    return result;
                }
            }
        }

        public static Receipt TryGetById(int id)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    select Id, Date from Receipts where Id = $id;
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

        public static List<ReceiptItem> FindItems(int receiptId)
        {
            using (var connection = DbHelper.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    select ProductId, Price from ReceiptsProducts where ReceiptId = $receiptId;
                ";
                command.Parameters.AddWithValue("$receiptId", receiptId);
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ReceiptItem>();
                    while (reader.Read())
                        result.Add(new ReceiptItem
                        {
                            ProductId = reader.GetInt32(0),
                            Price = reader.GetDecimal(1)
                        });
                    return result;
                }
            }
        }

        private static Receipt Read(SqliteDataReader reader)
        {
            return new Receipt
            {
                Id = reader.GetInt32(0),
                Date = DateTime.FromBinary(reader.GetInt64(1))
            };
        }
    }

    public class ReceiptItem
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
