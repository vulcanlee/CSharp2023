using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace Firsts_Smantic_Kernel
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 透過環境變數取得 Azure OpenAI 服務需要用到的授權金鑰
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

            // 建立 Semantic Kernel Builder 建置器物件，準備進行相關運行參數設定
            var builder = new KernelBuilder();

            // 設定使用 Azure OpenAI 服務需要用到的參數
            builder.WithAzureTextCompletionService(
                     "text-davinci-003",                  // Azure OpenAI 服務中所佈署的模型名稱
                     "https://openailabtw.openai.azure.com/", // Azure OpenAI 服務端點
                     apiKey);      // Azure OpenAI 服務需要用到的授權金鑰

            // 建立 Semantic Kernel 物件
            var kernel = builder.Build();

            // 宣告一個要提問文字的樣板，其中 input 為使用者要提問的文字
            var prompt = @"{{$input}}";

            // 宣告要使用該模型的相關參數
            var myPromptConfig = new PromptTemplateConfig
            {
                Description = "透過指定的 GPT 模型，進行各種問題的文字生成.",
                Completion =
                {
                    MaxTokens = 300,
                    Temperature = 0.2,
                    TopP = 0.5,
                }
            };

            // 宣告一個提示樣板，傳入提示文字，使用該提示規劃物件與 Semantic Kernel 物件
            var myPromptTemplate = new PromptTemplate(
                prompt,
                myPromptConfig,
                kernel
            );

            // 宣告一個語意函數規劃物件，傳入提示規劃物件與提示樣板
            var myFunctionConfig = new SemanticFunctionConfig(myPromptConfig, myPromptTemplate);

            // 註冊語意函數，傳入語意函數名稱、語意函數類型、語意函數規劃物件
            // 注意：A skill name can contain only ASCII letters, digits, and underscores
            var myFunction = kernel.RegisterSemanticFunction(
                "Test_Prompt_Completion_Function",
                "TestPromptCompletion",
                myFunctionConfig);

            string inputContent = "如何才能提高統一發票中獎機率";

            // 執行語意函數，傳入使用者要提問的文字
            var myOutput = await kernel.RunAsync(inputContent, myFunction);

            // 將 GPT 生成內容寫入到螢幕
            Console.WriteLine(myOutput);
        }
    }
}