using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OwlStream.Application.Services;
using OwlStream.Domain.Configs;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;
using OwlStream.Domain.Services.Infra;
using OwlStream.Infra.Repositories;
using OwlStream.Infra.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Get configurations
var connectionString = builder.Configuration.GetValue<string>("DatabaseConnection");
var jwtConfigSection = builder.Configuration.GetSection("JwtConfig");
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOriginPolicy", builder =>
    {
        builder
            .WithOrigins(allowedOrigins)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers().ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });

// Application Services
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IMoviesService, MoviesService>();
builder.Services.AddScoped<IGenresService, GenresService>();
builder.Services.AddScoped<IMovieRolesService, MovieRolesService>();
builder.Services.AddScoped<ICinelistsService, CinelistsService>();
builder.Services.AddScoped<ICastService, CastService>();

// Repositories
builder.Services.AddScoped<IPeopleRepository, PeopleRepository>(_ => new PeopleRepository(connectionString));
builder.Services.AddScoped<IUsersRepository, UsersRepository>(_ => new UsersRepository(connectionString, new PeopleRepository(connectionString)));
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>(_ => new MoviesRepository(connectionString));
builder.Services.AddScoped<IGenresRepository, GenresRepository>(_ => new GenresRepository(connectionString));
builder.Services.AddScoped<IMovieRolesRepository, MovieRolesRepository>(_ => new MovieRolesRepository(connectionString));
builder.Services.AddScoped<ICinelistsRepository, CinelistsRepository>(_ => new CinelistsRepository(connectionString));
builder.Services.AddScoped<ICastRepository, CastRepository>(_ => new CastRepository(connectionString, new PeopleRepository(connectionString)));

// Infra Services
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IFilesValidationService, FilesValidationService>();

// File limit size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2147483648;
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OwlStream.API",
        Version = "v1",
        Description = "API to **manage** and **access** OWL Stream features.<br>Here we have 3 different access levels:<br><br>1. **[Public]** Method allowed for everyone.<br>2. **[Private]** Method allowed only for logged users.<br>3. **[Admin]** Method allowed only for Administrators."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                   \r\n\r\n Enter 'Bearer'[space] and then your token in the text input below.
                    \r\n\r\nExample: 'Bearer 12345abcdef\'",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.EnableAnnotations();
});

// JWT authentication
builder.Services.Configure<JwtConfig>(jwtConfigSection);
var jwtConfig = jwtConfigSection.Get<JwtConfig>();
var key = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(x =>
    {
        x.Cookie.Name = jwtConfig.Name;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[jwtConfig.Name];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.UseCors("AnyOriginPolicy");

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.DefaultModelsExpandDepth(0);
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OwlStream.API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
