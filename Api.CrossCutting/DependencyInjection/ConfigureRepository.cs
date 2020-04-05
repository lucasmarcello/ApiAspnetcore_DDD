using Api.Data.Context;
using Api.Data.Implementations;
using Api.Data.Repository;
using Api.Domain.Interfaces;
using Api.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.CrossCutting.DependencyInjection
{
    public class ConfigureRepository
    {
        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            serviceCollection.AddScoped<IUserRepository, UserImplementation> ();

            //TODO Melhorar
            serviceCollection.AddDbContext<MyContext>(
                // options => options.UseMySql("Server=localhost;Port=3306;Database=dbAPI;Uid=root;Pwd=mudar@123")
                options => options.UseSqlServer("Server=.\\SQLEXPRESS2019;Database=dbAPI;user Id=sa;Password=mudar@123")
            );

        }
    }
}
