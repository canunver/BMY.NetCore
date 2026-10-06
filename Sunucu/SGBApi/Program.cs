using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SGBApi;
using TNS.Api;

// Eski kod ayarlarını ConfigurationManager.AppSettings ile okuyor; ayarlar App.config dosyasından (SGBApi.dll.config) gelir.
AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(AppContext.BaseDirectory, "SGBApi.dll.config"));

// Eski web.config'deki <globalization culture="tr-TR"> karşılığı
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
CultureInfo kultur = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = kultur;
CultureInfo.DefaultThreadCurrentUICulture = kultur;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

// Servis kayıtları SGBServisler kitaplığındadır (tek katmanlı kullanımla ortak).
ServisKayit.Varsayilanlar();

app.MapGet("/api", () => ServisDagitici.ServisListesi());
app.MapGet("/api/{servis}", (string servis) => ServisDagitici.MetotListesi(servis));
app.MapPost("/api/{servis}/{metot}", (HttpContext baglam, string servis, string metot) => ServisDagitici.Calistir(baglam, servis, metot));

app.Run();
