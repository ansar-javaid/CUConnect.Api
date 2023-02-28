using CUConnect.Database;
using CUConnect.Database.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Sql Dependency Injection
#region Database Contexts
builder.Services.AddDbContext<IdentityContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("CUConnect.Api")));
builder.Services.AddDbContext<CUConnectDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("CUConnect.Api")));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();
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
        Description = $"API's Documentaion with Swagger \r\n\r\n © Copyright {DateTime.Now.Year}. All rights reserved."
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
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
#endregion

builder.Services.AddHealthChecks();

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
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("health");
    endpoints.MapControllers();

    /* Uncomment line below to get the list of config variables resolved at runtime */
    //endpoints.MapGet("/dump-config", async ctx =>
    //{
    //    var configInfor = (Configuration as IConfigurationRoot).GetDebugView();
    //    await ctx.Response.WriteAsync(configInfor);
    //});
});

app.Run();
