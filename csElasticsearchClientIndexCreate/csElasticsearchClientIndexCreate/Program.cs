using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Newtonsoft.Json;

namespace csElasticsearchClientIndexCreate
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Create an index
            const string IndexName = "gpt_embedding";
            Uri HostUri = new Uri("http://192.168.82.7:9200");

            // Create a client settings object
            var settings = new ElasticsearchClientSettings(HostUri);
            // Create a client object from settings
            var client = new ElasticsearchClient(settings);

            #region 刪除索引
            var deleteResponse = await client.Indices.DeleteAsync(IndexName);
            #endregion

            // Check if the index exists
            var existsResponse = await client.Indices.ExistsAsync(IndexName);

            #region 檢查與建立索引
            if (!existsResponse.Exists)
            {
                await Console.Out.WriteLineAsync($"沒有找到指定索引，現在開始建立");
                var newIndexResponse = await client.Indices.CreateAsync<DocumentationChunk>(IndexName,
                    i => i
                    .Mappings(m => m
                    .Properties(p => p
                    .Text(n => n.FileName)
                    .Text(n => n.ChunkNumber)
                    .Keyword(n => n.Extension)
                    .DenseVector(m => m.EmbeddingVector, m =>
                    {
                        m.Dims(1536);
                        m.Similarity("dot_product");
                        m.Index(true);
                    })
                    )));

                if (!newIndexResponse.IsValidResponse || newIndexResponse.Acknowledged is false)
                {
                    throw new Exception("Oh no!");
                }
            }
            #endregion

            #region 新增資料
            var bulkAll = client.BulkAll(GetEmbeddingData(), r => r
            .Index(IndexName)
            .BackOffRetries(20)
            .BackOffTime(TimeSpan.FromSeconds(10)));

            bulkAll.Wait(TimeSpan.FromMinutes(10), r=>Console.WriteLine("data indexed"));
            #endregion

            var sq = new ScriptQuery
            {
                QueryName = "name_query",
                Boost = 1.1f,
                Script = new InlineScript
            };
            #region 使用 Consine 進行相似性搜尋
            var cosineResponse = await client.SearchAsync<DocumentationChunk>(s => s
            .Index(IndexName)
            );
            #endregion
        }

        static List<DocumentationChunk> GetEmbeddingData()
        {
            List<DocumentationChunk> DocumentationChunks = new();
            string directory = @"C:\DataLake\Snapshot\第一個記事本";
            var files = Directory.GetFiles(directory, "*.EmbeddingJson");
            foreach (var file in files)
            {
                string embeddingText = File.ReadAllText(file);
                var embedding = JsonConvert.DeserializeObject<float[]>(embeddingText)!;
                string filename = file;
                string chunkNumber = "";
                string extension = Path.GetExtension(filename);

                DocumentationChunks.Add(new DocumentationChunk
                {
                    ChunkNumber = chunkNumber,
                    Extension = extension,
                    FileName = filename,
                    EmbeddingVector = embedding,
                });
            }
            return DocumentationChunks;
        }
    }
}