using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using TNS;
using TNS.Api;

namespace SGBApi
{
    /// <summary>
    /// POST /api/{servis}/{metot} isteklerini, servis arayüzündeki ilgili metoda yönlendirir.
    /// Her servis metodu için ayrı controller yazmak yerine arayüz yansıma (reflection) ile okunur;
    /// arayüze eklenen yeni metot kendiliğinden API'de görünür.
    /// </summary>
    public static class ServisDagitici
    {
        public static IResult ServisListesi()
        {
            return Results.Content(JsonSerializer.Serialize(ServisKayit.Adlar()), "application/json", Encoding.UTF8);
        }

        public static IResult MetotListesi(string servis)
        {
            Type arayuz;
            Func<object> fabrika;
            if (!ServisKayit.Bul(servis, out arayuz, out fabrika))
                return Results.NotFound();

            JsonArray liste = new JsonArray();
            foreach (MethodInfo m in ServisProtokol.Metotlar(arayuz).OrderBy(x => x.Name))
            {
                JsonObject o = new JsonObject();
                o["metot"] = m.Name;
                o["imza"] = ServisProtokol.Imza(m);
                o["parametreler"] = new JsonArray(m.GetParameters().Select(p => (JsonNode)p.Name).ToArray());
                o["donus"] = m.ReturnType.ToString();
                liste.Add(o);
            }
            return Results.Content(liste.ToJsonString(), "application/json", Encoding.UTF8);
        }

        public static async Task Calistir(HttpContext baglam, string servis, string metot)
        {
            baglam.Response.ContentType = "application/json; charset=utf-8";
            try
            {
                Type arayuz;
                Func<object> fabrika;
                if (!ServisKayit.Bul(servis, out arayuz, out fabrika))
                {
                    await HataYaz(baglam, 404, "'" + servis + "' adlı servis yok.", null);
                    return;
                }

                string govdeMetni;
                using (StreamReader okuyucu = new StreamReader(baglam.Request.Body, Encoding.UTF8))
                    govdeMetni = await okuyucu.ReadToEndAsync();
                JsonObject govde = ApiJson.NesneAyristir(govdeMetni);

                string imza = baglam.Request.Query["imza"];
                MethodInfo m;
                try
                {
                    m = ServisProtokol.MetotBul(arayuz, metot, imza, govde.Select(p => p.Key).ToList());
                }
                catch (MissingMethodException e)
                {
                    await HataYaz(baglam, 404, e.Message, null);
                    return;
                }
                catch (AmbiguousMatchException e)
                {
                    await HataYaz(baglam, 400, e.Message, null);
                    return;
                }

                object[] args = ServisProtokol.GovdeOku(m, govde);
                object hedef = fabrika();
                object donus;
                try
                {
                    donus = m.Invoke(hedef, args);
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    throw;
                }

                await baglam.Response.WriteAsync(ServisProtokol.SonucYaz(m, donus, args), Encoding.UTF8);
            }
            catch (TNSIslemHatasi e)
            {
                await HataYaz(baglam, 500, e.Message, e.sonuc);
            }
            catch (Exception e)
            {
                await HataYaz(baglam, 500, e.Message, null);
            }
        }

        private static Task HataYaz(HttpContext baglam, int durumKodu, string hata, Sonuc sonuc)
        {
            if (baglam.Response.HasStarted) return Task.CompletedTask;
            baglam.Response.StatusCode = durumKodu;
            JsonObject o = new JsonObject();
            o["hata"] = hata;
            if (sonuc != null) o["sonuc"] = ApiJson.TokenYaz(sonuc, typeof(Sonuc));
            return baglam.Response.WriteAsync(o.ToJsonString(ApiJson.Ayarlar), Encoding.UTF8);
        }
    }
}
