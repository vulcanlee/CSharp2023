using NPOI.XWPF.Extractor;
using NPOI.XWPF.UserModel;
using System;
using System.IO;

namespace csWord2Text
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 讀取 Word 檔案
            XWPFDocument document = new XWPFDocument(File.OpenRead(@"UserGuide_Android.docx"));

            // 讀取所有內容
            XWPFWordExtractor extractor = new XWPFWordExtractor(document);
            string text = extractor.Text;

            // 輸出所有內容
            Console.WriteLine(text);

            // 關閉 C# 檔案
            document.Close();
        }
    }
}