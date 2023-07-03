using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Logic;
using CUConnect.Logic.Notifications;
using CUConnect.Models.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
//Sql Dependency Injection
#region Database Contexts

// We are using two DB Contexts one for Identity other for our Designed databse.
// Follow the steps one by one.
// 1 For Migration run this: Add-Migration step1 -context IdentityContext
// 1.1 For Migration run this: Add-Migration step1.1 -context CUConnectDBContext
// 2  For Database Update run this:Update-database -context CUConnectDBContext
builder.Services.AddDbContext<IdentityContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("CUConnect.Api")));
builder.Services.AddDbContext<CUConnectDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("CUConnect.Api")));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();
#endregion


#region Repositiry Scops
builder.Services.AddScoped<IAuthenticationREPO, AuthenticationLogic>();
builder.Services.AddScoped<IProfileREPO, ProfileLogic>();
builder.Services.AddScoped<IPostREPO, PostsLogic>();
builder.Services.AddScoped<ISubscriptionREPO, SubscriptionLogic>();
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#region Swager UI
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CU Connect API's",
        Description = $"API's Documentaion with Swagger \r\n\r\n � Copyright {DateTime.Now.Year}. All rights reserved."
    });
    #region XMl Documentation  
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    swagger.IncludeXmlComments(xmlPath);
    #endregion
});
#endregion


#region COROS
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed((hosts) => true);
    });
});
#endregion



builder.Services.AddHealthChecks();
builder.Services.AddDirectoryBrowser();

//JWT Configuration
#region JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JsonWebTokenKeys:ValidIssuer"],
                    ValidAudience = builder.Configuration["JsonWebTokenKeys:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JsonWebTokenKeys:IssuerSigningKey"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/api/notifications")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("EnableCORS");

app.UseRouting();
/*app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "Document")),
    RequestPath = "/files"
});*/

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("health");
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/api/notifications");

    /* Uncomment line below to get the list of config variables resolved at runtime */
    //endpoints.MapGet("/dump-config", async ctx =>
    //{
    //    var configInfor = (Configuration as IConfigurationRoot).GetDebugView();
    //    await ctx.Response.WriteAsync(configInfor);
    //});
});

app.Run();
