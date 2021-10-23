using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.Services;

namespace MongoDB
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      ConfigureMongoDb(services);

      services.AddControllers()
        .AddNewtonsoftJson(options => options.UseMemberCasing());
    }

    private void ConfigureMongoDb(IServiceCollection services)
    {
      var settings = GetMongoDbSettings();
      var db = CreateMongoDatabase(settings);

      AddMongoDbService<PostsService, Post>(settings.PostsCollectionName);
      AddMongoDbService<UsersService, User>(settings.UsersCollectionName);

      void AddMongoDbService<TService, TModel>(string collectionName)
      {
        services.AddSingleton(db.GetCollection<TModel>(collectionName));
        services.AddSingleton(typeof(TService));
      }
    }

    private SocialDatabaseSettings GetMongoDbSettings() =>
        Configuration.GetSection(nameof(SocialDatabaseSettings)).Get<SocialDatabaseSettings>();

    private IMongoDatabase CreateMongoDatabase(SocialDatabaseSettings settings)
    {
      var client = new MongoClient(settings.ConnectionString);
      return client.GetDatabase(settings.DatabaseName);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
