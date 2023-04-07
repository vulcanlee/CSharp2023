using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3;
using System.Reflection.Metadata;
using MathNet.Numerics.LinearAlgebra;

namespace OpenAIHelloEmbedding;

/// <summary>
/// 使用 OpenAI Embedding 技術，進行文字內容搜尋
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        #region 建立 OpenAiOptions 物件，用來標明此次呼叫 API 的類型與授權資訊
        // 這邊使用 Environment.GetEnvironmentVariable() 來取得環境變數，也可以直接使用字串
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
        var openAITextEmbedding = new OpenAIService(new OpenAiOptions()
        {
            ProviderType = ProviderType.Azure,
            ApiKey = apiKey,
            DeploymentId = "text-embedding-ada-002",
            ResourceName = "vulcan-openai"
        });
        #endregion

        #region 建立 查詢問題文字的 Embedding
        string question = "哪句話是關於動物的？";
        //string question = "哪句話是關於健康的？";
        //string question = "哪句話是關於哲學的？";
        //string question = "What is the animal that jumps over the dog?";
        var questionEmbedding = await GetEmbedding(openAITextEmbedding, question);
        #endregion

        #region 建立文件庫文字的 Embedding
        List<string> allLibrary = new List<string>()
        {
            "敏捷的棕色狐狸跳過了懶狗",
            "一天一蘋果，醫生遠離我",
            "存在還是不存在，這是個問題",
            //"The quick brown fox jumps over the lazy dog.",
            //"Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            //"The cat in the hat."
        };
        Dictionary<string, Vector<double>> allDocumentsEmbedding = new();
        foreach (var library in allLibrary)
        {
            var docEmbedding = await GetEmbedding(openAITextEmbedding, library);
            allDocumentsEmbedding.Add(library, docEmbedding);
        }
        #endregion

        #region 問題與文件的內嵌 Cosine Similarity 計算
        foreach (var item in allDocumentsEmbedding)
        {
            // calculate cosine similarity
            var v2 = item.Value;
            var v1 = questionEmbedding;
            var cosineSimilarity = v1.DotProduct(v2) / (v1.L2Norm() * v2.L2Norm());

            Console.WriteLine($"Cosine similarity: {cosineSimilarity}");
        }
        #endregion

        Console.WriteLine("Press any key for continuing...");
        Console.ReadKey();
    }

    static async Task<Vector<double>> GetEmbedding(OpenAIService openAITextEmbedding, string doc)
    {
        var embeddingResult = await openAITextEmbedding.Embeddings
            .CreateEmbedding(new EmbeddingCreateRequest()
            {
                Input = doc,
                Model = Models.TextEmbeddingAdaV2,
            });

        if (embeddingResult.Successful)
        {
            Vector<double> theEmbedding;

            var embeddingResponse = embeddingResult.Data.FirstOrDefault();
            var allValues = embeddingResponse.Embedding.ToArray();
            theEmbedding = Vector<double>.Build.DenseOfArray(allValues);
            return theEmbedding;
        }
        else
        {
            if (embeddingResult.Error == null)
            {
                throw new Exception("Unknown Error");
            }
            Console.WriteLine($"{embeddingResult.Error.Code}: {embeddingResult.Error.Message}");
            return null;
        }

    }
}