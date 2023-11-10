
using Microsoft.Extensions.DependencyInjection;

namespace csGenericDI;

#region 只有一個泛型型別的介面與具體實作
public interface IRepository<T>
{
    void Show(T item);
}

public class Repository<T> : IRepository<T> 
{
    public void Show(T item)
    {
        Console.WriteLine($"Adding {item}");
    }
}
#endregion

internal class Program
{
    static void Main(string[] args)
    {
       var serviceProvider = new ServiceCollection()
            .AddScoped(typeof(IRepository<>), typeof(Repository<>))
            .AddScoped(typeof(Repository<>))
            .AddScoped(typeof(IRepositoryWithMoreGenericType<,>), typeof(RepositoryWithMoreGenericType<,>))
            .BuildServiceProvider();

        #region 使用介面來注入實作物件
        var repositoryString = serviceProvider.GetService<IRepository<string>>();
        repositoryString.Show("Hello World");

        var repositoryInt = serviceProvider.GetService<IRepository<int>>();
        repositoryInt.Show(123);
        #endregion

        #region 使用實作類別來注入實作物件
        var repositoryString2 = serviceProvider.GetService<Repository<string>>();
        repositoryString2.Show("Hello Not Interface");
        #endregion

        #region 使用多個泛型來注入實作物件
        var repositoryWithMoreGenericType = serviceProvider
            .GetService<IRepositoryWithMoreGenericType<string, int>>();
        repositoryWithMoreGenericType.Show("Hello", 123);
        #endregion
    }
}

#region 具有多個泛型型別的介面與具體實作
public interface IRepositoryWithMoreGenericType<T1,T2>
{
    void Show(T1 item1, T2 item2);
}

public class RepositoryWithMoreGenericType<T1, T2> : IRepositoryWithMoreGenericType<T1, T2>
{
    public void Show(T1 item1, T2 item2)
    {
        Console.WriteLine($"Adding {item1} / {item2} ");
    }
}
#endregion
