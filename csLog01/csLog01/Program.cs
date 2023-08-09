namespace csLog01
{
    /// <summary>
    /// 不要發明輪子，自己做 Log 機制
    /// 在底下的做法，你看到了甚麼問題?
    /// </summary>
    internal class Program
    {
        public static Logger logger = new Logger();
        static void Main(string[] args)
        {
            logger.Trace("我是追蹤:Trace");
        }
    }

    public class Logger
    {
        public void Trace(string message)
        {
            #region 想要將日誌訊息寫入螢幕
            Console.WriteLine(message);
            #endregion

            #region 想要將日誌訊息寫入到檔案內
            string filePath = "example.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.Write(message + Environment.NewLine);
            }
            #endregion
        }
    }
}