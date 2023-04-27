using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace PptxToTextOpenXml
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = "Blazor.pptx";

            string textContent = ExtractTextFromPptx(inputFilePath);

            Console.WriteLine("PowerPoint 檔案已成功轉換為文字內容！");
            Console.WriteLine(textContent);
        }

        static string ExtractTextFromPptx(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            using (PresentationDocument presentationDocument = PresentationDocument.Open(filePath, false))
            {
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                if (presentationPart != null)
                {
                    foreach (SlideId slideId in presentationPart.Presentation.SlideIdList)
                    {
                        SlidePart slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId);

                        ExtractTextFromSlide(slidePart, sb);
                    }
                }
            }

            return sb.ToString();
        }

        static void ExtractTextFromSlide(SlidePart slidePart, StringBuilder sb)
        {
            if (slidePart == null)
            {
                return;
            }

            CommonSlideData slideData = slidePart.Slide.CommonSlideData;

            foreach (var shape in slideData.ShapeTree)
            {
                if (shape is Shape && ((Shape)shape).TextBody != null)
                {
                    foreach (var paragraph in ((Shape)shape).TextBody.Descendants<A.Paragraph>())
                    {
                        foreach (var text in paragraph.Descendants<A.Text>())
                        {
                            sb.AppendLine(text.Text);
                        }
                    }
                }
            }
        }
    }
}
