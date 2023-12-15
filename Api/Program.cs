using Api.Auth;
using Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Cors config
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        x =>
        {
            x.WithOrigins("https://localhost:5000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
});

var domain = $"https://{config["Auth0:Domain"]}/";

// Auth0 config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = config["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

//builder.Services
//    .AddAuthorization(options =>
//    {
//        options.AddPolicy(
//            "read:messages",
//            policy => policy.Requirements.Add(
//                new HasScopeRequirement("read:messages", domain)
//            )
//        );
//    });

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("read:messages", policy => policy.Requirements.Add(
                new HasScopeRequirement("read:messages", domain)
            )
);


builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddSingleton<IConfiguration>(config);

builder.Services.AddControllers();//.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
//builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();