using ApiGateway.Presentation.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using eCommerce.SharedLibrary.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", false, true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
JwtAuthenticationScheme.AddJwtAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();
app.Run();
