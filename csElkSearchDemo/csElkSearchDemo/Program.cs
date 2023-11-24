﻿//using Elastic.Clients.Elasticsearch;
//using Elastic.Clients.Elasticsearch.Aggregations;
//using Elastic.Transport;
using Nest;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace csElkSearchDemo
{
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

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new ConnectionSettings(new Uri("http://10.1.1.231:9200/"))
                .DisableDirectStreaming()
                .BasicAuthentication("elastic", "elastic");

            var client = new ElasticClient(settings);

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

            #region 變更欄位屬性的 Fielddata = true
            //var resultMap = await client.MapAsync<SbuUserId>(m => m
            //    .Index(indexSbuName)
            //    .Properties( p=>p
            //        .Text(t=>t
            //            .Name(n=>n.sbu_userid)
            //            .Fielddata(true)
            //        )
            //    )
            //);
            #endregion

            #region 測試存在 Array 內的程式碼
            //unique_sbu_userid = new List<string>
            //{
            //    "admin", "q6501", "uhd", "d4107", "d4209",
            //    "d4265", "d4806", "d4173", "d4231", "d4243"
            //};

            //var searchResponse = client.Search<EmrReportCustHeader>(s => s
            //    .Index("emr_report_cust_header")
            //    .Query(q => q
            //        .Terms(t => t
            //            .Field(f => f.erch_reportdoctorid)
            //            .Terms(unique_sbu_userid)
            //            )
            //        )
            //    );

            //if (searchResponse.IsValid)
            //{
            //    await Console.Out.WriteLineAsync();
            //    //await Console.Out.WriteLineAsync(searchResponse.DebugInformation);
            //    await Console.Out.WriteLineAsync();
            //}
            #endregion

            #region 嘗試第一次初始連線，
            var searchResponse = client.Search<EmrReportCustHeader>(s => s
                .Index("emr_report_cust_header")
                .Size(1)
                );

            if (searchResponse.IsValid)
            {
                await Console.Out.WriteLineAsync();
                //await Console.Out.WriteLineAsync(searchResponse.DebugInformation);
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
                    .DateRange(dr => dr
                        .Field(f => f.sbr_startdate)
                        .LessThanOrEquals(startDate)),
                    mte => mte
                    .DateRange(dr => dr
                        .Field(f => f.sbr_enddate)
                        .GreaterThanOrEquals(endDate)),
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

            if (response6.IsValid)
            {
                await Console.Out.WriteLineAsync();
                //await Console.Out.WriteLineAsync(response6.DebugInformation);
                var debugInformation = response6.DebugInformation.Split("\n");
                for (int i = 0; i < debugInformation.Length; i++)
                {
                    var line = debugInformation[i];
                    if(line.Contains("# Request:") == true)
                    {
                        line = debugInformation[i + 1];
                        await Console.Out.WriteLineAsync(line);
                    }
                }
                await Console.Out.WriteLineAsync();

                var document = response6.Documents;

                if (response6.Aggregations.ContainsKey(unique_sbu_userid_name))
                {
                    var termsAggregation = response6.Aggregations.Terms(unique_sbu_userid_name);
                    //Console.WriteLine($"搜尋筆數 : {termsAggregation.Buckets.Count}");
                    foreach (var bucket in termsAggregation.Buckets)
                    {
                        //Console.WriteLine($"Key: {bucket.Key}, Count: {bucket.DocCount}");
                        unique_sbu_userid.Add(bucket.Key);
                    }
                }

            }

            Console.WriteLine($"Distinct 查詢筆數 : {unique_sbu_userid.Count}");
            // 顯示需要耗費時間
            Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion

            #region 使用上面 Aggregation 結果，對 emr_report_cust_header 做查詢
            //unique_sbu_userid
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("使用上面 Aggregation 結果，對 emr_report_cust_header 做查詢");
            stopwatch.Restart();




            //unique_sbu_userid = new List<string>
            //{
            //    "admin", "q6501", "uhd", "d4107", "d4209",
            //    "d4265", "d4806", "d4173", "d4231", "d4243"
            //};




            var response7 = await client.SearchAsync<EmrReportCustHeader>(s => s
            .Index(indexReportCustHeaderName)
            .From(0)
            .Size(200)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sd => sd
                            .Term(t => t.erch_reportdoctorid, assignUserId)
                        )
                    .Must(
                        mt => mt
                            .DateRange(dr => dr
                                .Field(f => f.erch_reportdate)
                             .LessThanOrEquals(SYSDATE)),
                        et => et
                            .Term(t => t.erch_istrash, 0),
                        rdi => rdi
                            .Terms(t => t
                                .Field("erch_reportdoctorid").Terms(unique_sbu_userid))
                        )
                    )
                )
            .Sort(s => s
                .Ascending($"erch_id")
                .Descending(d => d.erch_createdate)
                )
            );

            stopwatch.Stop();

            if (response7.IsValid)
            {
                await Console.Out.WriteLineAsync();
                var debugInformation = response7.DebugInformation.Split("\n");
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

                var documents = response7.Documents;

                foreach (var doc in documents)
                {
                    //Console.WriteLine($"erch_id : {doc.erch_id} / erch_reportcode: {doc.erch_reportcode}");
                }
            }

            #region 統計文件數量
            var response8 = await client.CountAsync<EmrReportCustHeader>(s => s
                .Index(indexReportCustHeaderName)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            sd => sd
                                .Term(t => t.erch_reportdoctorid, assignUserId)
                            )
                        .Must(
                            mt => mt
                                .DateRange(dr => dr
                                    .Field(f => f.erch_reportdate)
                                 .LessThanOrEquals(SYSDATE)),
                            et => et
                                .Term(t => t.erch_istrash, 0),
                            rdi => rdi
                                .Terms(t => t
                                    .Field("erch_reportdoctorid").Terms(unique_sbu_userid))
                            )
                        )
                    )
                );

            #endregion
            // 顯示需要耗費時間
            Console.WriteLine($"查詢筆數 : {response8.Count}");
            Console.WriteLine($"查詢文件需要 {stopwatch.ElapsedMilliseconds} ms");
            #endregion
        }
    }
}
