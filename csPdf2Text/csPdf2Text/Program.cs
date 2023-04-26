using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Reflection.PortableExecutable;

namespace csPdf2Text
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // creating a pdf reader object
            PdfReader reader = new PdfReader("synapse_pacs.pdf");
            // printing number of pages in pdf file
            Console.WriteLine("Number of pages: " + reader.NumberOfPages);

            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                Console.WriteLine($"------------------------------- Page:{i}");
                // getting a specific page from the pdf file
                PdfDictionary page = reader.GetPageN(i);

                // extracting text from page
                string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                Console.WriteLine(text);
            }

            // closing the reader
            reader.Close();
        }
    }
}
