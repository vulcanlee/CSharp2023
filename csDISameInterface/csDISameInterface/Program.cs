using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace csDISameInterface
{
    public interface IService
    {
        void Get();
    }
    public class ServiceA : IService
    {
        public void Get()
        {
            Console.WriteLine("ServiceA");
        }
    }

    public class ServiceB : IService
    {
        public void Get()
        {
            Console.WriteLine("ServiceB");
        }
    }

    public class ServiceC : IService
    {
        public void Get()
        {
            Console.WriteLine("ServiceC");
        }
    }
    internal class Program
    {
        delegate IService ServiceResolver(string key);
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ServiceA>();
                    services.AddTransient<ServiceB>();
                    services.AddTransient<ServiceC>();
                    services.AddTransient<Func<string, IService>>(serviceProvider => key =>
                    {
                        return key switch
                        {
                            "A" => serviceProvider.GetService<ServiceA>(),
                            "B" => serviceProvider.GetService<ServiceB>(),
                            "C" => serviceProvider.GetService<ServiceC>(),
                            _ => throw new KeyNotFoundException()
                        };
                    });

                    services.AddTransient<ServiceResolver>(serviceProvider => key =>
                    {
                        switch (key)
                        {
                            case "A":
                                return serviceProvider.GetService<ServiceA>();
                            case "B":
                                return serviceProvider.GetService<ServiceB>();
                            case "C":
                                return serviceProvider.GetService<ServiceC>();
                            default:
                                throw new KeyNotFoundException();
                        }
                    });
                }).Build();

            var serviceAccessor1 = host.Services.GetRequiredService<Func<string, IService>>();
            var serviceAccessor2 = host.Services.GetRequiredService<ServiceResolver>();
            IService service = serviceAccessor1("A");
            service.Get();
            service = serviceAccessor2("C");
            service.Get();
        }
    }
}
