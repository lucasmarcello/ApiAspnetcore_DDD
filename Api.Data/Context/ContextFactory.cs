using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Data.Context
{
    public class ContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {
            //TODO alterar para melhorar seguranca
            //MySQL
            //var connectionString = "Server=localhost;Port=3306;Database=dbAPI;Uid=root;Pwd=mudar@123";
            //SQLServer
            var connectionString = "Server=.\\SQLEXPRESS2019;Database=dbAPI;user Id=sa;Password=mudar@123";
            var optionsBuilder = new DbContextOptionsBuilder<MyContext> ();
            //optionsBuilder.UseMySql (connectionString);
            optionsBuilder.UseSqlServer (connectionString);
            return new MyContext (optionsBuilder.Options);
        }
    }
}
