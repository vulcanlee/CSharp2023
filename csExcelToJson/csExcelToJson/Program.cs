
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3;

namespace csExcelToJson;

internal class Program
{
    static async Task Main(string[] args)
    {
        List<string> jsons = new List<string>();
        string filename = Path.Combine(Directory.GetCurrentDirectory(), "Embedding.json");
        bool generateEmbeddingDataFile = true;

        if (File.Exists(filename)) generateEmbeddingDataFile = false;
        else generateEmbeddingDataFile = true;

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

        Dictionary<string, List<double>> allStorageEmbedding = new();
        Dictionary<string, Vector<double>> allDocumentsEmbedding = new();

        if (generateEmbeddingDataFile)
        {
            jsons = ExcelToJsons();
            #region 建立文件庫文字的 Embedding
            int index = 2;
            allDocumentsEmbedding.Clear();
            foreach (var library in jsons)
            {
                var docEmbedding = await GetEmbedding(openAITextEmbedding, library);
                allDocumentsEmbedding.Add(index.ToString(), docEmbedding);
                allStorageEmbedding.Add(index.ToString(), docEmbedding.ToList());
                await Console.Out.WriteAsync($"{index} ");
                index++;
            }

            #region 準備將 Embedding Vector 系列化成為 JSON，並且寫入到檔案內
            string json = JsonConvert.SerializeObject(allStorageEmbedding);
            await File.WriteAllTextAsync(filename, json);
            #endregion
        }
        else
        {
            string json = await File.ReadAllTextAsync(filename);
            var embeddedObject = JsonConvert.DeserializeObject<Dictionary<string, List<double>>>(json);
            foreach (var item in embeddedObject)
            {
                allDocumentsEmbedding.Add(item.Key, Vector<double>.Build.DenseOfArray(item.Value.ToArray()));
            }
        }
        #endregion

        while (true)
        {
            #region 建立 查詢問題文字的 Embedding
            await Console.Out.WriteLineAsync($"提出你的問題 (quit 代表結束):");
            string question = Console.ReadLine();

            if (question.ToLower().Trim() == "quit") break;
            if (question.ToLower().Trim() == "") continue;

            var questionEmbedding = await GetEmbedding(openAITextEmbedding, question);
            #endregion

            #region 問題與文件的內嵌 Cosine Similarity 計算
            Dictionary<string, double> allDocumentsCosineSimilarity = new();
            foreach (var item in allDocumentsEmbedding)
            {
                // calculate cosine similarity
                var v2 = item.Value;
                var v1 = questionEmbedding;
                var cosineSimilarity = v1.DotProduct(v2) / (v1.L2Norm() * v2.L2Norm());
                allDocumentsCosineSimilarity.Add(item.Key, cosineSimilarity);
            }
            var items = allDocumentsCosineSimilarity.OrderByDescending(x => x.Value).Take(5);
            foreach (var item in items)
            {
                Console.WriteLine($"Cosine similarity: {item.Key} > {item.Value}");
            }
            await Console.Out.WriteLineAsync(); await Console.Out.WriteLineAsync();
        }
        #endregion
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

    private static List<string> ExcelToJsons()
    {
        string currentPath = System.IO.Directory.GetCurrentDirectory();
        //string filename = "sample.xlsx";
        string filename = "20230407 falcon.xlsx";
        string filePath = System.IO.Path.Combine(currentPath, filename);
        string sheetName = "Grid Results";
        var jsons = ConvertExcelToJson(filePath, sheetName);
        return jsons;
    }

    public static List<string> ConvertExcelToJson(string filePath, string sheetName)
    {
        List<string> jsons = new List<string>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

            // Get the number of rows and columns
            int rows = worksheet.Dimension.Rows;
            int columns = worksheet.Dimension.Columns;

            // Create a list to store the data
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

            // Loop through each row and column and add the data to the list
            bool isFirst = true;
            for (int row = 1; row <= rows; row++)
            {
                Dictionary<string, object> rowData = new Dictionary<string, object>();
                for (int column = 1; column <= columns; column++)
                {
                    // Get the cell value and add it to the dictionary
                    object cellValue = worksheet.Cells[row, column].Value;
                    string columnName = worksheet.Cells[1, column].Value.ToString();
                    rowData.Add(columnName, cellValue);
                }
                if (isFirst == true)
                {
                    isFirst = false;
                    continue;
                }
                data.Add(rowData);
                // Convert the list to JSON string
                string json = JsonConvert.SerializeObject(rowData, Newtonsoft.Json.Formatting.Indented);
                jsons.Add(json);
            }

            return jsons;
        }
    }
}