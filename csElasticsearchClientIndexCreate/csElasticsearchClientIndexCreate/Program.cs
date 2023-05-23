//using Elastic.Clients.Elasticsearch;
using Nest;
using Newtonsoft.Json;
using System.Diagnostics;

namespace csElasticsearchClientIndexCreate
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Create an index
            const string IndexName = "gpt_embedding";
            Uri HostUri = new Uri("http://192.168.82.7:9200");
            bool RecreateIndex = false;

            // Create a client settings object
            var settings = new ConnectionSettings(HostUri)
                        .DefaultIndex(IndexName)  // 你的索引名稱
                        .DisableDirectStreaming(); // 關閉直接串流

            // Create a client object from settings
            var client = new ElasticClient(settings);

            var allEmbeddingData = GetEmbeddingData();

            if (RecreateIndex)
            {
                #region 刪除索引
                var deleteResponse = await client.Indices.DeleteAsync(IndexName);
                #endregion

                // Check if the index exists
                var existsResponse = await client.Indices.ExistsAsync(IndexName);

                #region 檢查與建立索引
                if (!existsResponse.Exists)
                {
                    await Console.Out.WriteLineAsync($"沒有找到指定索引，現在開始建立");
                    var newIndexResponse = await client.Indices.CreateAsync(IndexName,
                        i => i
                        .Map<DocumentationChunk>(m => m
                        .Properties(p => p
                        .Text(n => n.Name(s => s.FileName))
                        .Text(n => n.Name(s => s.ChunkNumber))
                        .Keyword(n => n.Name(s => s.Extension))
                        .DenseVector(n => n.Name(s => s.EmbeddingVector).Dimensions(1536)))
                        ));

                    if (!newIndexResponse.IsValid || newIndexResponse.Acknowledged is false)
                    {
                        throw new Exception("Oh no!");
                    }
                }
                #endregion

                #region 新增資料

                foreach (var itemEmbedding in allEmbeddingData)
                {
                    var insertResponse = client.Index(itemEmbedding, i => i.Index(IndexName));
                }

                #endregion
            }

            #region 使用 Consine 進行相似性搜尋
            var settingsEmbeddingSearch = settings;
            var clientEmbeddingSearch = client;

            // 當你搜索向量時，你需要提供一個向量
            var inputVector = allEmbeddingData[10].EmbeddingVector;  // 你的輸入向量

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var searchResponse = clientEmbeddingSearch.Search<DocumentationChunk>(s => s
                .Index(IndexName)
                .From(0)
                .Size(30)
                .Query(q => q
                    .ScriptScore(ss => ss
                        .Query(_ => _.MatchAll())
                        .Script(sc => sc
                        //.Source("cosineSimilarity(params.queryVector, 'embeddingVector') + 1.0")
                        .Source("cosineSimilarity(params.queryVector, 'embeddingVector') + 1.0")                         // 使用cosineSimilarity函數進行比較
                            .Params(p => p
                                .Add("queryVector", inputVector)
                            )
                        )
                    )
                )
            );
            sw.Stop();
            await Console.Out.WriteLineAsync($"查詢耗費時間 : {sw.ElapsedMilliseconds} ms");
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
                if (embedding.Length != 1536)
                {
                    throw new Exception("Embedding length is not 1536");
                }
                string filename = file;
                string chunkNumber = "A";
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