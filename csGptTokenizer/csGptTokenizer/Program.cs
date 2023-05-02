using AI.Dev.OpenAI.GPT;

namespace csGptTokenizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = "Microsoft MVP | 提供 .NET C# / Blazor / MAUI 教育訓練與顧問服務";
            List<int> tokensChinese = GPT3Tokenizer.Encode(text);
            Console.WriteLine(text);
            Console.WriteLine($"中文字串長度:{text.Length}, Token數量:{tokensChinese.Count}");
            text = "Microsoft MVP | Providing .NET C# / Blazor / MAUI Education, Training and Consulting Services";
            tokensChinese = GPT3Tokenizer.Encode(text);
            Console.WriteLine(text);
            Console.WriteLine($"英文字串長度:{text.Length}, Token數量:{tokensChinese.Count}");
            Console.WriteLine($"上述 Token 計算結果，可以從 https://platform.openai.com/tokenizer 來進行比對");
        }
    }
}