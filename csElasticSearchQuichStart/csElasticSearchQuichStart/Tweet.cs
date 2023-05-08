namespace csElasticSearchQuichStart;

/// <summary>
/// Tweet 物件
/// </summary>
public class Tweet
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// 發文日期
    /// </summary>
    public DateTime PostDate { get; set; }
    /// <summary>
    /// 發文內容
    /// </summary>
    public string Message { get; set; }
}