using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using Newtonsoft.Json;



var token = File.ReadAllText("DISCORD_TOKEN.txt");

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway(opt =>
    {
        opt.Token = token;
        opt.Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent;
    })
    .AddGatewayHandlers(typeof(Program).Assembly)
    .AddApplicationCommands();
var host = builder.Build();

// // Add commands using minimal APIs
host.AddSlashCommand("login_screen_enable_for", "Enable login screen for a spesifc user.", (User usr) => $"login enable {usr.Id}");
host.AddSlashCommand("login_screen_disable_for", "Disable login screen for a spesifc user.", (User usr) => $"login disable {usr.Id}");
host.AddSlashCommand("login_screen_set_timeout", "after <when> time, user will need to re-type to gain access to the server.", (User usr, TimeSpan when, Guild guild) => $"login enable {usr.Id}");



await host.RunAsync();


return;

[JsonObject(MemberSerialization.OptIn)]
class PerUserState
{
    [JsonProperty] public bool IsLoginEnabled;
    [JsonProperty] public DateTime SessionLoginTime;
    [JsonProperty] public TimeSpan TimeoutAfter;
}

[JsonObject(MemberSerialization.OptIn)]
class PerServerState
{
    [JsonProperty] public Dictionary<ulong, PerUserState> UserStates = new();
}


[JsonObject(MemberSerialization.OptIn)]
class State
{
    [JsonProperty] public Dictionary<ulong /*guild id*/, PerServerState> ServerStates = new();
}


