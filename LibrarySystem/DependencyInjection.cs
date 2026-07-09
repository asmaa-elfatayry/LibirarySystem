namespace LibrarySystem.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

        return services;
    }
}
