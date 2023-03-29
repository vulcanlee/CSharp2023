using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace EFCoreUsingDbContextPoolSize;

/// <summary>
/// 模擬 DbContext 的 Connection Pool 超出使用量，造成例外異常的情況
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        // 設定資料庫連線的最大連線數量
        int MaxPoolSize = 4;
        // 設定資料庫連線的最大等待時間
        int ConnectTimeout = 5;
        bool UsingEFCoreQuery = false;

        #region 準備進行資料庫連線或者建立資料庫
        // 連線字串
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestPool;" +
            $"Max Pool Size={MaxPoolSize};Connect Timeout={ConnectTimeout};MultipleActiveResultSets=True";
        // 建立資料庫連線
        DbContextOptions<SchoolContext> options =
            new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlServer(connectionString)
            .Options;

        using (var context = new SchoolContext(options))
        {
            // 檢查資料庫是否存在
            if (context.Database.CanConnect() == false)
            {
                // 建立資料庫
                await context.Database.EnsureCreatedAsync();

                context.Database.GetDbConnection();
                var connection = context.Database.GetDbConnection();
            }
        }
        #endregion

        #region 模擬建立多個 DbContext 來進行資料庫存取，故意造成 Connection Pool 被耗盡的情況
        //#region 建立 10 個 DbContext 來進行資料庫存取

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            // 建立 10 個 DbContext 來進行資料庫存取
            int idx = i;
            Task task = Task.Run(async () =>
            {
                using (var context = new SchoolContext(options))
                {
                    if (UsingEFCoreQuery)
                    {
                        #region 使用 EF Core 的 SqlQuery 來進行查詢
                        Console.WriteLine($"第{idx}工作已經開始 " + DateTime.Now.ToString("mm:ss"));

                        var result = context.Database
                        .SqlQuery<string>($"SELECT GETDATE() AS D");
                        Console.WriteLine($"第{idx}工作準備休息 " + DateTime.Now.ToString("mm:ss"));
                        // 這裡休息多久，都不會有例外異常發生，因為，執行完 SQL 命令，連線就立即關閉了
                        Thread.Sleep(5000);
                        Console.WriteLine($"第{idx}工作正要結束 " + DateTime.Now.ToString("mm:ss"));

                        //var std = new Student()
                        //{
                        //    Name = "Bill"
                        //};
                        //context.Students.Add(std);
                        //context.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        #region 使用 DbConnection 來存取資料庫，但是故意延遲關閉連線時間
                        using (var cn = context.Database.GetDbConnection())
                        {
                            Console.WriteLine($"第{idx}工作開啟連線 " + DateTime.Now.ToString("mm:ss"));
                            cn.Open();
                            // 關閉前休息3秒，並註解底下四行，同樣會造成例外異常
                            var cmd = cn.CreateCommand();
                            cmd.CommandText = "SELECT GETDATE() AS D";
                            var dr = cmd.ExecuteReader();
                            dr.Read();
                            Console.WriteLine($"第{idx}工作準備休息 " + DateTime.Now.ToString("mm:ss"));

                            // 這裡若將休息時間改為 3000 (3秒)，將會造成底下錯誤
                            // -----------------------------------------------------
                            // System.InvalidOperationException: 'Timeout expired.
                            // The timeout period elapsed prior to obtaining a connection
                            // from the pool.  This may have occurred because
                            // all pooled connections were in use and max pool size was reached.'
                            Thread.Sleep(3000);
                            // 這裡換成 await Task.Delay 同樣無效，會造成例外異常
                            //await Task.Delay(3000);

                            Console.WriteLine($"第{idx}工作正要結束 " + DateTime.Now.ToString("mm:ss"));
                            cn.Close();
                        }
                        #endregion
                    }
                }
            });
            tasks.Add(task);
        }
        #endregion

        await Task.WhenAll(tasks);

        Console.WriteLine("Press any key for continuing...");
        Console.ReadKey();
    }
}