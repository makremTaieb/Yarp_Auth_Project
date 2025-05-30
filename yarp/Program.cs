using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

var app = builder.Build();


// Login Endpoint 

app.MapPost("/login", (string username, string password) =>
{
    if (username == "admin" && password == "admin")
    {
        return Results.SignIn(new System.Security.Claims.ClaimsPrincipal(
            new ClaimsIdentity(
                [
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("username", username),
                new Claim("ts", DateTime.UtcNow.ToShortDateString()),
                new Claim("sub", Guid.NewGuid().ToString())
                ], BearerTokenDefaults.AuthenticationScheme)),
                authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
    }


        return Results.Unauthorized();


});



app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.Run();
