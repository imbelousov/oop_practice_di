using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Shop.Core
{
    [TestFixture]
    internal class Tests
    {
        [Test]
        public void CreateSingleProduct()
        {
            var id = Product.Create(new TestProduct());
            Assert.AreEqual(1, id);
        }

        [Test]
        public void ListProducts()
        {
            Product.Create(new TestProduct {Price = 1000});
            Product.Create(new TestProduct {Type = ProductType.HouseholdChemicals});
            Product.Create(new TestProduct {Title = "Soap"});
            var products = Product.FindAll();
            Assert.AreEqual(3, products.Count);
        }

        [Test]
        public void CheckProductPropertiesInList()
        {
            var expected = new TestProduct();
            expected.Id = Product.Create(expected);
            var actual = Product.FindAll().Single();
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Title, actual.Title);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.IsDeleted, actual.IsDeleted);
        }

        [Test]
        public void GetProductById()
        {
            var expected = new TestProduct {IsDeleted = true};
            expected.Id = Product.Create(expected);
            var actual = Product.TryGetById(expected.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Title, actual.Title);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.IsDeleted, actual.IsDeleted);
        }

        [Test]
        public void GetNotExistingProductById()
        {
            var actual = Product.TryGetById(1);
            Assert.IsNull(actual);
        }

        [Test]
        public void UpdateExistingProduct()
        {
            var id = Product.Create(new TestProduct());
            var expected = new TestProduct
            {
                Id = id,
                Title = "Soap",
                Price = 200,
                Type = ProductType.HouseholdChemicals,
                IsDeleted = true
            };
            var result = Product.TryUpdate(expected);
            Assert.IsTrue(result);
            var actual = Product.TryGetById(id);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Title, actual.Title);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.IsDeleted, actual.IsDeleted);
        }

        [Test]
        public void UpdateNotExistingProduct()
        {
            var id = Product.Create(new TestProduct());
            var expected = new TestProduct
            {
                Id = id + 1,
                Title = "Soap",
                Price = 200,
                Type = ProductType.HouseholdChemicals,
                IsDeleted = true
            };
            var result = Product.TryUpdate(expected);
            Assert.IsFalse(result);
        }

        [Test]
        public void CreateReceipt()
        {
            var id = Receipt.Create(new TestReceipt());
            Assert.AreEqual(1, id);
        }

        [Test]
        public void ListReceipts()
        {
            Receipt.Create(new TestReceipt());
            Receipt.Create(new TestReceipt());
            Receipt.Create(new TestReceipt());
            Receipt.Create(new TestReceipt());
            var receipts = Receipt.FindAll();
            Assert.AreEqual(4, receipts.Count);
        }

        [Test]
        public void CheckReceiptPropertiesInList()
        {
            var expected = new TestReceipt();
            expected.Id = Receipt.Create(expected);
            var actual = Receipt.FindAll().Single();
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Date, actual.Date);
        }

        [Test]
        public void GetReceiptById()
        {
            var expected = new TestReceipt();
            expected.Id = Receipt.Create(expected);
            var actual = Receipt.TryGetById(expected.Id);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Date, actual.Date);
        }

        [Test]
        public void GetNotExistingReceiptById()
        {
            var actual = Receipt.TryGetById(1);
            Assert.IsNull(actual);
        }

        [Test]
        public void FillReceiptWithItems()
        {
            var productsIds = new[]
            {
                Product.Create(new TestProduct()),
                Product.Create(new TestProduct()),
                Product.Create(new TestProduct())
            };
            var receiptId = Receipt.Create(new TestReceipt());
            Receipt.AddItem(receiptId, new TestReceiptItem(productsIds[0]));
            Receipt.AddItem(receiptId, new TestReceiptItem(productsIds[2]));
            var actual = Receipt.FindItems(receiptId);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public void CheckReceiptItemsProperties()
        {
            var productId = Product.Create(new TestProduct());
            var receiptId = Receipt.Create(new TestReceipt());
            var expected = new TestReceiptItem(productId);
            Receipt.AddItem(receiptId, expected);
            var actual = Receipt.FindItems(receiptId).Single();
            Assert.AreEqual(expected.ProductId, actual.ProductId);
            Assert.AreEqual(expected.Price, actual.Price);
        }

        [Test]
        public void ReceiptItemsDuplicates()
        {
            var productId = Product.Create(new TestProduct());
            var receiptId = Receipt.Create(new TestReceipt());
            var item = new TestReceiptItem(productId);
            Receipt.AddItem(receiptId, item);
            Receipt.AddItem(receiptId, item);
            var actual = Receipt.FindItems(receiptId);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public void PrintReceiptDocx()
        {
            var productsIds = new[]
            {
                Product.Create(new TestProduct {Title = "Product 1"}),
                Product.Create(new TestProduct {Title = "Product 2"})
            };
            var receiptId = Receipt.Create(new TestReceipt());
            Receipt.AddItem(receiptId, new TestReceiptItem(productsIds[0]) {Price = 30});
            Receipt.AddItem(receiptId, new TestReceiptItem(productsIds[1]) {Price = 40});
            Receipt.AddItem(receiptId, new TestReceiptItem(productsIds[1]) {Price = 40});
            var bytes = ReceiptPrinter.PrintDocx(receiptId);
            File.WriteAllBytes("receipt.docx", bytes);
        }

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(EmptyDbPath))
            {
                File.Delete(DbPath);
                File.Copy(EmptyDbPath, DbPath);
            }
            else
                File.Copy(DbPath, EmptyDbPath);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            if (File.Exists(EmptyDbPath))
                File.Delete(EmptyDbPath);
        }

        private const string DbPath = "shop.db";
        private const string EmptyDbPath = "empty." + DbPath;
    }

    internal class TestProduct : Product
    {
        public TestProduct()
        {
            Title = "Test product";
            Type = ProductType.Food;
            Price = 15.7m;
        }
    }

    internal class TestReceipt : Receipt
    {
        public TestReceipt()
        {
            Date = new DateTime(2021, 02, 15, 18, 30, 0, DateTimeKind.Utc);
        }
    }

    internal class TestReceiptItem : ReceiptItem
    {
        public TestReceiptItem(int productId)
        {
            ProductId = productId;
            Price = 15.5m;
        }
    }
}
