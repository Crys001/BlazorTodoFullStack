using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAppGemini;
using Blazored.LocalStorage;
using Microsoft.Extensions.Http; // Assicurati di avere questo

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Registra il LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 2. Registra il nostro Handler (il casellante del Token)
builder.Services.AddTransient<JwtHandler>();

// 3. Configura l'HttpClient NOMINATO "ServerAPI" 
// ATTENZIONE: Qui mettiamo l'indirizzo dell'API (7287)
builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7287/");
})
.AddHttpMessageHandler<JwtHandler>();

// 4. Imposta questo client come quello predefinito per tutta l'app
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("ServerAPI"));

await builder.Build().RunAsync();