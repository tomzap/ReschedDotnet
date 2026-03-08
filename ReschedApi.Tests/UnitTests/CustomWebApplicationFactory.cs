using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
             // Remove existing DbContext registration
            services.RemoveAll<DbContextOptions<TodoDb>>();

            // Add InMemory database
            services.AddDbContext<TodoDb>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDb>();

            db.Todos.Add(new Todo { Name = "Test task" });
            db.SaveChanges();
        });

        builder.UseEnvironment("Development");
    }
}