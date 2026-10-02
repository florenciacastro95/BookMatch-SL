using  Microsoft.Extensions.Configuration;
namespace Bookmatch_SL.Repositories;

public abstract class RepositorioBase
{
    protected readonly string connectionString;

    public RepositorioBase(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration["ConnectionStrings:DefaultConnection"]!;
    }
}