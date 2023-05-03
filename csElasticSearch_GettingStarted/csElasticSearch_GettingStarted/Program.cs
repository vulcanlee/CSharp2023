namespace csElasticSearch_GettingStarted
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await FirstQuickLaunch();
        }

        /// <summary>
        /// 使用 HttpClient 呼叫 ElasticSearch 的 REST API
        /// </summary>
        /// <returns></returns>
        private static async Task FirstQuickLaunch()
        {
            string url = "http://192.168.82.7:9200/";
            string response = await new HttpClient().GetStringAsync(url);
            Console.WriteLine(response);
        }
    }
}