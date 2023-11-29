using SelectPdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        //string url = "http://chimeiconsentforms.posly.cc/Account/LoginAuto?userName=exentric&Password=70400845&ReturnUrl=/Forms/CustomForm?FormCode%3dMR-02-5100-001%26viewMode%3dForPrint";
        //string url = "http://chimeiconsentforms.posly.cc/Account/Loginauto?username=admin&ReturnUrl=%2FForms%2FCustomForm%3FFormCode%3DMR-02-5100-001%26Patno%3D12345%26AccessionNo%3D23243291%26AccessionKey%3DMR-02-5100-00112345202309121813%26PatientConsentId%3D98%26readonly%3Dreadonly%26Head_Name%3D%E6%9E%97%E5%A5%BD%E5%A5%BD%26Head_Birthday%3D1992%2F04%2F01%26Head_Bed%3D23-01%26navbar%3Dhide%26viewMode%3dForPrint";
        string url = "http://localhost:51332/Account/Loginauto?username=admin&ReturnUrl=%2FForms%2FCustomForm%3FFormCode%3DMR-02-5100-001%26Patno%3D12345%26AccessionNo%3D23243291%26AccessionKey%3DMR-02-5100-00112345202309121813%26PatientConsentId%3D98%26readonly%3Dreadonly%26Head_Name%3D%E6%9E%97%E5%A5%BD%E5%A5%BD%26Head_Birthday%3D1992%2F04%2F01%26Head_Bed%3D23-01%26navbar%3Dhide%26viewMode%3dForPrint";
        //string url = "http://chimeiconsentforms.posly.cc/Account/Loginauto?username=admin&ReturnUrl=%2FForms%2FCustomForm%3FformId%3DA218EDC7-4909-4A4F-A4C7-B66E6F83C748%26navbar%3Dhide%26ViewMode%3DPrint";

        string[] urls
        {
            get
            {
                return new string[] {
                    url+"%26Number%3d=1",
                    url+"%26Number%3d=2",
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url,
                    //url
                };
            }
            set { }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Parallel()
        {
            HtmlToPdf converter = getClient();

            var doc = new PdfDocument();

            var result = new Converter().ParallelConvert(converter, urls);

            doc.Append(result);

            addFooter(doc);

            byte[] bytes = doc.Save();

            return new FileStreamResult(new MemoryStream(bytes), "Application/pdf");

            //return View("Index");
        }

        public async Task<ActionResult> ParallelAsync()
        {
            HtmlToPdf converter = getClient();

            await new Converter().ParallelConvertAsync(converter, urls);

            return View("Index");
        }

        public ActionResult NewTask()
        {
            HtmlToPdf converter = getClient();

            var sw = Stopwatch.StartNew();

            var taskList = new List<Task>();

            foreach (var u in urls)
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                var t = Task.Factory.StartNew(() =>
                {
                    new Converter().Convert(converter, u);

                    Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                });

                taskList.Add(t);
            }

            taskList.ForEach(x => x.Wait());

            return View("Index");
        }

        public ActionResult NewTaskAsync()
        {
            HtmlToPdf converter = getClient();

            var sw = Stopwatch.StartNew();

            var taskList = new List<Task>();

            foreach (var u in urls)
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                var t = Task.Factory.StartNew(async () =>
                {
                    await new Converter().ConvertAsync(converter, u);

                    Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                });

                taskList.Add(t);
            }

            taskList.ForEach(x => x.Wait());

            return View("Index");
        }

        public ActionResult ForEach()
        {
            HtmlToPdf converter = getClient();

            var sw = Stopwatch.StartNew();

            foreach (var u in urls)
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                new Converter().Convert(converter, u);

                Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

            }

            return View("Index");
        } 
        
        public async Task<ActionResult> ForEachAsync()
        {
            HtmlToPdf converter = getClient();

            var sw = Stopwatch.StartNew();

            foreach (var u in urls)
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                await new Converter().ConvertAsync(converter, u);

                Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

            }

            return View("Index");
        }


        private HtmlToPdf getClient()
        {
            HtmlToPdf converter = new HtmlToPdf();
            SelectPdf.GlobalProperties.LicenseKey = "TmV/bnx7f255eW5/d2B+bn1/YH98YHd3d3c=";
            converter.Options.MaxPageLoadTime = 120;
            converter.Options.MinPageLoadTime = 2;
            converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            converter.Options.DrawBackground = false;
            converter.Options.DisplayFooter = true;
            converter.Options.MarginTop = 50;
            converter.Options.MarginLeft = 1;
            converter.Options.MarginRight = 1;

            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            converter.Footer.Height = 50;
            return converter;
        }

        private void addFooter(PdfDocument doc)
        {
            PdfHtmlElement footerElement = new PdfHtmlElement($@"
                            <div style=""font-size:20px;font-family:標楷體;width:85%;margin-left:auto;margin-right:auto"">
                                <div style=""border-top:1px solid rgba(0,0,0,0.5);padding-bottom:5px;""></div>     
                                <div style=""display:flex;position:relative"">
                                    <div class=""pisElement"">完成日期:</div>
                                    <div class=""pisElement"">2023/09/22 14:27:00</div>
                                    <div style=""display:flex;position: absolute; right:3%;"">
                                        <div class=""pisElement"">人員:</div>
                                        <div class=""pisElement"">詹ＯＯ</div>
                                    </div>
                                </div>
                            </div>", "");

            SelectPdf.PdfFont font2 = doc.AddFont(PdfStandardFont.Helvetica);
            font2.Size = 12;

            PdfTextElement footerPageElement = new PdfTextElement(470, 25, "Page: {page_number} of {total_pages}", font2);
            footerPageElement.HorizontalAlign = PdfTextHorizontalAlign.Left;

            doc.Footer = doc.AddTemplate(doc.Pages[0].ClientRectangle.Width, 50);
            doc.Footer.Add(footerElement);
            doc.Footer.Add(footerPageElement);
            doc.Footer.DisplayOnEvenPages = true;
        }
    }

    public class Converter
    {
        public async Task<PdfDocument> ParallelConvertAsync(HtmlToPdf client, string[] urls)
        {
            SelectPdf.PdfDocument doc = new SelectPdf.PdfDocument();

            var sw = Stopwatch.StartNew();

            Parallel.ForEach(urls, async x =>
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                var detailDoc = await ConvertAsync(client, x);

                doc.Append(detailDoc);

                Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

            });

            Debug.WriteLine($"ParalleConvert 作業完成,總共花了 {sw.Elapsed.TotalSeconds} 秒");

            return doc;
        }

        public PdfDocument ParallelConvert(HtmlToPdf client, string[] urls)
        {
            Dictionary<string, SelectPdf.PdfDocument> taskUrlDic = new Dictionary<string, SelectPdf.PdfDocument>();

            foreach(var url in urls)
            {
                taskUrlDic.Add(url, null);
            }

            SelectPdf.PdfDocument doc = new SelectPdf.PdfDocument();

            var sw = Stopwatch.StartNew();

            Parallel.ForEach(urls, x =>
            {
                var g = Guid.NewGuid().ToString();
                Debug.WriteLine($"{g} 開始列印,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

                var detailDoc = Convert(client, x);

                taskUrlDic[x] = detailDoc;

                Debug.WriteLine($"{g} 列印產生完成,Thread:{Thread.CurrentThread.ManagedThreadId},Seconds:{sw.Elapsed.TotalSeconds}");

            });

            foreach (var task in taskUrlDic)
            {
                doc.Append(task.Value);
            }


            Debug.WriteLine($"ParalleConvert 作業完成,總共花了 {sw.Elapsed.TotalSeconds} 秒");

            return doc;
        }

        public async Task<PdfDocument> ConvertAsync(HtmlToPdf client, string url)
        {
            return await Task.Run(() => { return client.ConvertUrl(url); });
        }

        public PdfDocument Convert(HtmlToPdf client, string url)
        {
            return client.ConvertUrl(url);
        }
    }

}