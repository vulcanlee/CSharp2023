using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System.Diagnostics;
using Elastic.Clients.Elasticsearch.Aggregations;

namespace csElasticsearchNETWithEmr;

#region DataModel
public class Blog
{
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
}

public class SbuUserId
{
    public long est_type { get; set; }
    public DateTime sbr_enddate { get; set; }
    public DateTime sbr_startdate { get; set; }
    public string sbru_userid { get; set; }
    public string sbu_userid { get; set; }
}

public class EmrReportCustHeader
{
    public string erch_reportcode { get; set; }
    public DateTime erch_createdate { get; set; }
    public long erch_istrash { get; set; }
    public DateTime erch_reportdate { get; set; }
    public string erch_reportdoctorid { get; set; }
    public long erch_id { get; set; }



    //public string erch_caseno { get; set; }
    //public string erch_eecclaimcheck { get; set; }
    ////public long erch_eecsend { get; set; }
    ////public long erch_eecsign { get; set; }
    //public string erch_extension1 { get; set; }
    //public string erch_extension2 { get; set; }
    //public string erch_extension3 { get; set; }
    //public DateTime erch_firstsigndate { get; set; }
    ////public long erch_isbehalfsign { get; set; }
    ////public long erch_isfinalsign { get; set; }
    ////public long erch_istrash { get; set; }
    //public DateTime erch_lastsigndate { get; set; }
    //public string erch_orderno { get; set; }
    //public string erch_orgid { get; set; }
    //public DateTime erch_patbirth { get; set; }
    //public string erch_patidnumber { get; set; }
    //public string erch_patname { get; set; }
    //public string erch_patno { get; set; }
    //public string erch_referno { get; set; }
    //public DateTime erch_reportdate { get; set; }
    //public string erch_reportdepid { get; set; }
    //public string erch_reportdepname { get; set; }
    //public string erch_reportdoctorname { get; set; }
    ////public long erch_reportversion { get; set; }
    //public DateTime erch_requestdate { get; set; }
    //public string erch_requestdepid { get; set; }
    //public string erch_requestdepname { get; set; }
    //public string erch_requestdoctorid { get; set; }
    //public string erch_requestdoctorname { get; set; }
    ////public long erch_secretlv { get; set; }
    //public string erch_sex { get; set; }
    ////public long erch_sortno { get; set; }
    //public string erch_source { get; set; }
    //public DateTime erch_trashdate { get; set; }
    //public string erch_trashuserid { get; set; }
    ////public long erch_validateflag { get; set; }
    ////public long erch_version { get; set; }
    //public string erch_xmlfile { get; set; }
}
#endregion

internal class Program
{
    static async Task Main(string[] args)
    {
        var settings = new ElasticsearchClientSettings(new Uri("http://10.1.1.231:9200/"))
            .Authentication(new BasicAuthentication("elastic", "elastic"))
            .DisableDirectStreaming()
            ;
        var client = new ElasticsearchClient(settings);

        string indexSbuName = "sbu_userid".ToLower();
        string indexReportCustHeaderName = "emr_report_cust_header".ToLower();
        string unique_sbu_userid_name = "unique_sbu_userid";
        List<string> unique_sbu_userid = new List<string>();

        #region 指定的固定參數
        string assignUserId = "admin";
        DateTime SYSDATE = DateTime.Parse("2023/11/23 00:00:00");
        DateTime startDate = SYSDATE;
        DateTime endDate = DateTime.Parse("2018/11/23 00:00:00");
        int estType = 3;
        #endregion

        #region 嘗試第一次初始連線
        var searchResponse = client.Search<EmrReportCustHeader>(s => s
            .Index("emr_report_cust_header")
            .Size(1)
            );

        if (searchResponse.IsValidResponse)
        {
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync(searchResponse.DebugInformation);
            await Console.Out.WriteLineAsync();
        }
        #endregion

        Stopwatch stopwatch = new Stopwatch();

        #region 對 sbu_userid 做 DISTINCT  查詢
        await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync("對 sbu_userid 做 DISTINCT  查詢");
        stopwatch.Restart();

        var response6 = await client.SearchAsync<SbuUserId>(s => s
        .Index(indexSbuName)
        .From(0)
        .Size(6000)
        .Query(q => q
            .Bool(b => b
            .Must(mt => mt
                .Range(r => r
                    .DateRange(dr => dr
                        .Field(f => f.sbr_startdate)
                        .Lte(startDate))),
                mte => mte
                .Range(r => r
                    .DateRange(dr => dr
                        .Field(f=>f.sbr_enddate)
                        .Gte(endDate))),
                et => et
                .Term(t => t.est_type, estType),
                uid => uid
                .Term(t => t.sbru_userid, assignUserId))
                )
            )
        .Aggregations(agg => agg
            .Terms(unique_sbu_userid_name, te => te
                .Field(f => f.sbu_userid).Size(8000))
            )
         );

        stopwatch.Stop();

        if (response6.IsValidResponse)
        {
            await Console.Out.WriteLineAsync();
            //await Console.Out.WriteLineAsync(response6.DebugInformation);
            var debugInformation = response6.DebugInformation.Split("\n");
            for (int i = 0; i < debugInformation.Length; i++)
            {
                var line = debugInformation[i];
                if (line.Contains("# Request:") == true)
                {
                    line = debugInformation[i + 1];
                    await Console.Out.WriteLineAsync(line);
                }
            }
            await Console.Out.WriteLineAsync();

            var document = response6.Documents;

            if (response6.Aggregations.ContainsKey(unique_sbu_userid_name))
            {
                var termsAggregation = response6.Aggregations[unique_sbu_userid_name] as StringTermsAggregate;
                Console.WriteLine($"搜尋筆數 : {termsAggregation.Buckets.Count}");
                foreach (var bucket in termsAggregation.Buckets)
                {
                    //Console.WriteLine($"Key: {bucket.Key}, Count: {bucket.DocCount}");
                    unique_sbu_userid.Add(bucket.Key.ToString());
                }
            }

        }

        Console.WriteLine($"Distinct 查詢筆數 : {unique_sbu_userid.Count}");
        // 顯示需要耗費時間
        Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
        #endregion

    }
}
