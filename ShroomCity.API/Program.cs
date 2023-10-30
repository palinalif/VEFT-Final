using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Repositories;
using ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// TODO: Register all services
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IMushroomRepository, MushroomRepository>();
builder.Services.AddTransient<IResearcherRepository, ResearcherRepository>();
builder.Services.AddTransient<ITokenRepository, TokenRepository>();

builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IExternalMushroomService, ExternalMushroomService>();
builder.Services.AddTransient<IMushroomService, MushroomService>();
builder.Services.AddTransient<IResearcherService, ResearcherService>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddDbContext<ShroomCityDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ShroomCityConnectionString"), b => b.MigrationsAssembly("ShroomCity.API"));
}
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key") ?? ""))
    };
});

builder.Services.AddAuthorization(options => 
{
    // read mushrooms policy
    options.AddPolicy("read:mushrooms", policy => policy.RequireClaim("permissions", "read:mushrooms"));
    // write mushrooms policy
    options.AddPolicy("write:mushrooms", policy => policy.RequireClaim("permissions", "write:mushrooms"));
    // read researchers policy
    options.AddPolicy("read:researchers", policy => policy.RequireClaim("permissions", "read:mushrooms"));
    // write researchers policy
    options.AddPolicy("write:researchers", policy => policy.RequireClaim("permissions", "read:researchers"));
});

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
