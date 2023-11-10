using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.UnitOfWork;
using AdopPixAPI.Hubs;
using AdopPixAPI.Services.UnitOfWork;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
builder.Services.AddScoped<IEntityUnitOfWork, EntityUnitOfWork>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("TokenKey:Access").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServerConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost:4200", "http://127.0.0.1:5173" ,"http://localhost:5173", "https://tester.adoppix.com" ,"https://adoppix.com" , "https://react.adoppix.com")
                                                      .AllowAnyMethod()
                                                      .AllowAnyHeader()
                                                      .AllowCredentials());
});
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddDbContext<SqlServerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AuctionBidHub>("api/auction-bid");
app.MapHub<NotificationHub>("api/notification");

app.Run();
