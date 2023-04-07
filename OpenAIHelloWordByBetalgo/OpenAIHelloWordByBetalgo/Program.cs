using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;

namespace OpenAIHelloWordByBetalgo
{
    /// <summary>
    /// 第一次使用 C# 來呼叫 Azure OpenAI API
    /// 使用套件 : Betalgo.OpenAI.GPT3
    /// 
    /// 建立 OpenAI Key 永久性的環境變數
    /// setx OpenAIKey "Azure OpenAI 儀表板看到的 Key" /M
    /// 刪除系統變數
    /// reg delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v OpenAIKey /f
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region 建立 OpenAiOptions 物件，用來標明此次呼叫 API 的類型與授權資訊
            // 這邊使用 Environment.GetEnvironmentVariable() 來取得環境變數，也可以直接使用字串
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            var gpt3 = new OpenAIService(new OpenAiOptions()
            {
                ProviderType = ProviderType.Azure,
                ApiKey = apiKey,
                DeploymentId = "text-davinci-003",
                ResourceName = "vulcan-openai"
            });
            #endregion

            #region 使用 Completions 物件，呼叫 OpenAI API 並取得回傳結果
            string prompt = "OpenAI, 恭喜我，我會使用 C# 呼叫 OpenAI API 了\n";
            await Console.Out.WriteLineAsync(prompt);
            var completionResult = await gpt3.Completions
                .CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = prompt,
                Model = Models.TextDavinciV3,
                Temperature = 0.5F,
                MaxTokens = 100,
                N = 3
            });
            #endregion

            #region 判斷回傳結果是否成功，並將結果印出
            if (completionResult.Successful)
            {
                await Console.Out.WriteLineAsync($"底下是 OpenAI 回覆的內容:");
                foreach (var choice in completionResult.Choices)
                {
                    Console.WriteLine(choice.Text);
                }
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}