using AI.Dev.OpenAI.GPT;

namespace csGptTokenizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = "Microsoft MVP | 提供 .NET C# / Blazor / MAUI 教育訓練與顧問服務";
            List<int> tokens = GPT3Tokenizer.Encode(text);
            Console.WriteLine(text);
            Console.WriteLine($"字串長度:{text.Length}, Token數量:{tokens.Count}");
            text = "Azure OpenAI is powered by models with different capabilities and price points";
            tokens = GPT3Tokenizer.Encode(text);
            Console.WriteLine(text);
            Console.WriteLine($"字串長度:{text.Length}, Token數量:{tokens.Count}");
        }
    }
}