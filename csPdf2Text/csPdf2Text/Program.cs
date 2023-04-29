using System;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;

namespace csPdf2Text
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StringBuilder result = new StringBuilder();

            using (PdfReader pdfReader = new PdfReader("synapse_pacs.pdf"))
            {
                using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                {
                    int numberOfPages = pdfDoc.GetNumberOfPages();

                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                        result.AppendLine(pageContent);
                    }
                }
            }

            Console.WriteLine(result.ToString());
        }
    }
}
