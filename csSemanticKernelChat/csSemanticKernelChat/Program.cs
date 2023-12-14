using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

string endPoint = "https://gpt4tw.openai.azure.com/";
string apiKey = "";
string deploymentId = "gpt-4";
string serviceId = "gpt-4";
string modelId = "gpt-4";
string systemRoleMessage = "你是一個專案 DBA ，精通各類資料庫，並且也是這方面的專業顧問與講師";

// 從環境變數內讀取 ApiKey 的值
apiKey = Environment.GetEnvironmentVariable("AOAILabKey");

IKernelBuilder builder = new KernelBuilder();
builder.Services.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Trace).AddDebug());
builder.Services.AddAzureOpenAIChatCompletion(deploymentId, modelId,
    endPoint, apiKey, serviceId);

Kernel kernel = builder.Build();

const string skPrompt = @"
{{$history}}

{{$userInput}}";

var executionSettings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 2000,
    Temperature = 0.7,
    TopP = 0.5
};

var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings);

var history = $"System > {systemRoleMessage}";
var arguments = new KernelArguments()
{
    ["history"] = history
};

while (true)
{
    Console.Write("User > ");
    var userInput = Console.ReadLine();
    if (userInput == "exit")
    {
        break;
    }
    arguments["userInput"] = userInput;

    //var assistant_answer = await chatFunction.InvokeAsync(kernel, arguments);
    //Console.WriteLine($"Assistant > {assistant_answer}\n");

    var result = chatFunction
        .InvokeStreamingAsync<StreamingChatMessageContent>(kernel, arguments);
    ChatMessageContent? chatMessageContent = null;
    await foreach (var content in result)
    {
        System.Console.Write(content);
        if (chatMessageContent == null)
        {
            System.Console.Write("Assistant > ");
            chatMessageContent = new ChatMessageContent(
                content.Role ?? AuthorRole.Assistant,
                content.ModelId!,
                content.Content!,
                content.InnerContent,
                content.Encoding,
                content.Metadata);
        }
        else
        {
            chatMessageContent.Content += content;
        }
    }

    System.Console.WriteLine(chatMessageContent);

    history += $"\nUser > {userInput}\nAssistant > {chatMessageContent}\n";
    arguments["history"] = history;
}








var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();

ChatHistory chatHistory = new ChatHistory(systemRoleMessage);

while (true)
{
    Console.Write("User > ");
    string input = Console.ReadLine();
    if (input == "exit")
    {
        break;
    }

    chatHistory.AddUserMessage(input);

    var result = chatCompletionService.GetChatMessageContentAsync(chatHistory);

    //Console.WriteLine($"Bot: {response}");
    //chatHistory.Add(input, response);
}

/*
var chatHistory = new ChatHistory();


chatHistory.AddSystemMessage("你是一個專案 DBA ，精通各類資料庫，並且也是這方面的專業顧問與講師");

while (true)
{
    Console.Write("User > ");
    string input = Console.ReadLine();
    if (input == "exit")
    {
        break;
    }

    chatHistory.AddUserMessage(input);

    var result = kernel
        .InvokeStreamingAsync<StreamingChatMessageContent>(input, chatHistory);

    ChatMessageContent chatMessageContent = null;

    string response = await kernel.ChatCompletion(input, chatHistory);
    Console.WriteLine($"Bot: {response}");
    chatHistory.Add(input, response);
}
*/




