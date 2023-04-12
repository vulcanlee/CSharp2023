using Azure.AI.OpenAI;
using Azure;

namespace AzureOpenAIClientLibrary;

/// <summary>
/// 使用微軟官方 Azure OpenAI client library for .NET 套件來呼叫相關 API
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
        string endpoint = "https://vulcan-openai.openai.azure.com/";
        var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        #endregion

        #region 準備使用 OpenAI GPT 的 Prompt / Completion 模式呼叫 API
        string prompt = "OpenAI, 恭喜我，我會使用 C# 呼叫 OpenAI API 了\n";
        await Console.Out.WriteLineAsync(prompt);

        var completionsOptions = new CompletionsOptions()
        {
            Prompts = { prompt },
            MaxTokens = 100,
            Temperature = 0.5f,
        };

        string deploymentName = "text-davinci-003";

        Response<Completions> completionsResponse = await client
            .GetCompletionsAsync(deploymentName, completionsOptions);
        if (completionsResponse != null)
        {
            string completion = completionsResponse.Value.Choices[0].Text;
            await Console.Out.WriteLineAsync($"底下是 OpenAI 回覆的內容:");
            Console.WriteLine($"{completion}");
        }
        #endregion

        Console.WriteLine("Press any key for continuing...");
        Console.ReadKey();
    }
}