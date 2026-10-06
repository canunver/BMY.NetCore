using Istemci;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

// Bu dosya bilerek boş tutulur: uygulamanın kurulumu Istemci.dll içindeki IstemciBaslangic sınıfındadır.
// Yeni sayfa, uç veya servis eklemek için bu projede değişiklik gerekmez.

IstemciBaslangic.OrtamiHazirla();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Istemci.dll ile gelen wwwroot dosyaları (css, js, resim) derleme çıktısından çalışırken de sunulsun.
builder.WebHost.UseStaticWebAssets();
IstemciBaslangic.ServisleriEkle(builder);

WebApplication app = builder.Build();
IstemciBaslangic.Kullan(app);
app.Run();
