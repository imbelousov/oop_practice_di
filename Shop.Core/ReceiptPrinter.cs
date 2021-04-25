using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Shop.Core
{
    public static class ReceiptPrinter
    {
        public static byte[] PrintDocx(int receiptId)
        {
            var receipt = Receipt.TryGetById(receiptId);
            var receiptItems = Receipt.FindItems(receiptId);
            var products = receiptItems.Select(x => x.ProductId).Distinct().ToDictionary(x => x, Product.TryGetById);
            var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                new Document(new Body()).Save(mainPart);

                var title = new Paragraph(new Run(new Text($"Кассовый чек от {receipt.Date:g}")));

                var table = new Table();
                var tableHeader = new TableRow();
                tableHeader.AppendChild(new TableCell(new TableCellProperties {TableCellWidth = new TableCellWidth {Type = TableWidthUnitValues.Pct, Width = "70"}}, new Paragraph(new Run(new Text("Товар")))));
                tableHeader.AppendChild(new TableCell(new TableCellProperties {TableCellWidth = new TableCellWidth {Type = TableWidthUnitValues.Pct, Width = "30"}}, new Paragraph(new Run(new Text("Стоимость")))));
                table.AppendChild(tableHeader);
                
                foreach (var (product, price) in receiptItems.Select(x => (products[x.ProductId], x.Price)))
                {
                    var tableRow = new TableRow();
                    tableRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(product.Title)))));
                    tableRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(price.ToString("F2"))))));
                    table.AppendChild(tableRow);
                }

                mainPart.Document.Body.AppendChild(title);
                mainPart.Document.Body.AppendChild(table);

                mainPart.Document.Save();
            }
            return stream.ToArray();
        }
    }
}
