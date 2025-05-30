using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddAuthorization(options => {
    // default policy
    options.AddPolicy("access", policy => policy.RequireAuthenticatedUser());

    //user policy 
    options.AddPolicy("user-access", policy => policy.RequireAuthenticatedUser()
    .RequireClaim("role","user"));
    
    //admin policy 
    options.AddPolicy("admin-access", policy => policy.RequireAuthenticatedUser()
    .RequireClaim("role","admin"));


});

var app = builder.Build();


// Login Endpoint 

app.MapPost("/login", (string username, string password , string role = "user") =>
{
    if (username == "admin" && password == "pwd")
    {
        return Results.SignIn(new System.Security.Claims.ClaimsPrincipal(
            new ClaimsIdentity(
                [
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("username", username),
                new Claim("ts", DateTime.UtcNow.ToShortDateString()),
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("role", "admin")
                ], BearerTokenDefaults.AuthenticationScheme)),
                authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
    }
    else if (username == "user" && password == "pwd")
    {
        return Results.SignIn(new System.Security.Claims.ClaimsPrincipal(
            new ClaimsIdentity(
                [
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("username", username),
                new Claim("ts", DateTime.UtcNow.ToShortDateString()),
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("role", "user")
                ], BearerTokenDefaults.AuthenticationScheme)),
                authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
    }

    return Results.Unauthorized();


});



app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.Run();
