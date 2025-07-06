using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace SenzuraPOS_DAPP
{
    public class PdfGenerator
    {
        public void GenerateInvoice(ObservableCollection<CartItem> cartItems, decimal totalAmount)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

            int yPoint = 50;

            gfx.DrawString("Invoice", new XFont("Verdana", 20, XFontStyle.Bold), XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopCenter);
            yPoint += 40;

            gfx.DrawString($"Date: {System.DateTime.Now.ToShortDateString()}", font, XBrushes.Black, new XPoint(50, yPoint));
            yPoint += 20;

            gfx.DrawString("----------------------------------------------------------------------------------------------------", font, XBrushes.Black, new XPoint(50, yPoint));
            yPoint += 20;

            gfx.DrawString("Item		Brand		Model		Qty		Price		Total", font, XBrushes.Black, new XPoint(50, yPoint));
            yPoint += 20;

            gfx.DrawString("----------------------------------------------------------------------------------------------------", font, XBrushes.Black, new XPoint(50, yPoint));
            yPoint += 20;

            foreach (var item in cartItems)
            {
                gfx.DrawString($"{item.Name}		{item.Brand}		{item.ModelNumber}		{item.Quantity}		{item.Price:N2}		{item.Total:N2}", font, XBrushes.Black, new XPoint(50, yPoint));
                yPoint += 20;
            }

            gfx.DrawString("----------------------------------------------------------------------------------------------------", font, XBrushes.Black, new XPoint(50, yPoint));
            yPoint += 20;

            gfx.DrawString($"Total Amount: Rs. {totalAmount:N2}", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(50, yPoint));

            string filename = $"Invoice_{System.DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";
            string filePath = Path.Combine(Path.GetTempPath(), filename);
            document.Save(filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
