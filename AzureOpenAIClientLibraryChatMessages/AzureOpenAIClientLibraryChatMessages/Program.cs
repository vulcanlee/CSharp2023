using Azure.AI.OpenAI;
using Azure;

namespace AzureOpenAIClientLibraryChatMessages
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string endpoint = "https://vulcan-openai.openai.azure.com/";
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            #region 準備使用 OpenAI GPT 的 聊天記憶功能，並且呼叫 GPT API 來生成內容
            while (true)
            {
                Console.WriteLine("請輸入手頭上可用的食材名稱");
                string input = Console.ReadLine();
                Console.WriteLine("請稍後，主廚正在思考中...");
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, "你是一個全方位主廚，擅長世界各地美食製作與烹飪，" +
                        "將會根據輸入食材，建議要做出的菜名與建議作法(並不一定只能夠使用指定食材來設計這道菜)，最後給予這道菜一個完美說明與介紹；" +
                        "若輸入內容沒有食材，請回應:這難倒我了，我不是食神"),
                        new ChatMessage(ChatRole.User, "食材：雞胸肉、檸檬、迷迭香"),
                        new ChatMessage(ChatRole.Assistant, "建議：檸檬迷迭香烤雞胸"),
                        new ChatMessage(ChatRole.User, "食材：番茄、義大利麵、大蒜"),
                        new ChatMessage(ChatRole.Assistant, "建議：番茄大蒜義大利麵"),
                        new ChatMessage(ChatRole.User, $"食材：{input}"),
                    },
                    Temperature = (float)0.9,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                string deploymentName = "gpt-4";
                Response<ChatCompletions> response = await client.GetChatCompletionsAsync(
                    deploymentOrModelName: deploymentName,
                    chatCompletionsOptions);
                var result = response.Value;

                foreach (var message in result.Choices)
                {
                    Console.WriteLine(message.Message.Content);
                }
                await Console.Out.WriteLineAsync();
                await Console.Out.WriteLineAsync();
                #endregion
            }

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}