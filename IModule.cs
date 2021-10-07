
public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}

public static class ModuleExtensions
{
    // this could also be added into the DI container
    static readonly List<IModule> registeredModules = new List<IModule>();

    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.RegisterModule(builder.Services);
            registeredModules.Add(module);
        }

        return builder;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in registeredModules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
