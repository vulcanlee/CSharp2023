using System;
using HtmlAgilityPack;

namespace HtmlToText
{
    class Program
    {
        static void Main(string[] args)
        {
            string htmlContent = File.ReadAllText("【筆記】範本.html");

            // 將 HTML 內容轉換為純文本
            var textContent = ConvertHtmlToPlainText(htmlContent);

            // 將純文本內容顯示在螢幕上
            Console.WriteLine(textContent);
        }

        static string ConvertHtmlToPlainText(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // 刪除所有 script 和 style 節點
            RemoveNodeByTag(htmlDoc, "script");
            RemoveNodeByTag(htmlDoc, "style");

            // 取得網頁的純文本內容
            string plainText = htmlDoc.DocumentNode.InnerText;

            // 刪除多餘的空格和換行符
            plainText = System.Text.RegularExpressions.Regex.Replace(plainText, @"\s+", " ").Trim();

            return plainText;
        }

        static void RemoveNodeByTag(HtmlDocument htmlDoc, string tagName)
        {
            var nodes = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }
        }
    }
}
