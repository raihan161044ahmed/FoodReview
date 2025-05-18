using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Microsoft.AspNetCore.Http;
using FoodReview.Repository.Implementation;
using FoodReview.Repository.Interface;
using FoodReview.Service.BusinessLogic;
using FoodReview.Service.Interface;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using FoodReview.Service.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//  Add Cookie Authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "FoodReviewAuth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/User/login";
        options.LogoutPath = "/api/User/logout";
        options.AccessDeniedPath = new PathString("/api/User/Forbidden");
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.SaveTokens = true;

        // Optional: Map specific claims from Google
        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");

        // Optional: Configure the scopes
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });
builder.Services.AddAuthorization();

// Register the repositories with Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>();
builder.Services.AddScoped<ISocialUserRepository, SocialUserRepository>();

// Register the services with Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); 

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
