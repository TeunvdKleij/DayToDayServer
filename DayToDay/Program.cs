using System.Text;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var MyAllowSpecificOrigins = "myAllowSpecificOrigins";

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder => {
            builder
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://127.0.0.1:3000",
                    "http://10.0.0.92:3000",
                    "http://192.168.1.241:3000",
                    "http://192.168.1.241:3001",
                    "http://172.20.10.6:3000",
                    "http://172.20.10.6:3001"
                )
                .WithMethods("POST", "GET", "PUT", "OPTIONS", "DELETE")
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    
    .AddJwtBearer(options => {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters() {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ditisdeencryptie"))
        };
    });

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication();
builder.Services.AddDataProtection();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/AccessLog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
    .CreateLogger();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dbContext.Database.Migrate();
    if (!dbContext.Group.Any())
    {
        dbContext.Group.Add(new GroupModel() { Name = "Home"});
        dbContext.SaveChanges();
    }
    if (File.Exists("./Dbbackups/" + DateTime.Now.ToString("ddMMyyyy") + "app.db"))
    {
        Log.Information("Db backed up already");
    }
    else
    {
        string[] dbFiles = Directory.GetFiles("./Dbbackups/", "*.db");
        if (dbFiles.Length > 0) 
            foreach (string dbFile in dbFiles) File.Delete(dbFile);
        string fileToCopy = "./database/app.db";
        string destinationDirectory = "./Dbbackups/"+ DateTime.Now.ToString("ddMMyyyy") +"app.db";
        if (File.Exists(fileToCopy)) File.Copy(fileToCopy, destinationDirectory);
    }
}

//app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/Acceslog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger(); 

app.Run();