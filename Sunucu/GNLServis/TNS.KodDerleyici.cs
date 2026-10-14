using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace TNS
{
    /// <summary>
    /// Çalışma anında C# kaynak kodunu derler (dinamik bordro: Calculator, PEB gösterge formülleri).
    /// .NET 8'de eski CSharpCodeProvider (CodeDom) derleme yapamaz (PlatformNotSupportedException); yerine .NET'in kendi
    /// derleyicisi Roslyn (Microsoft.CodeAnalysis.CSharp) kullanılır. Derlenen kod, çalışan uygulamanın bütün kitaplıklarını
    /// (çekirdek kitaplıklar, GNLServis, HRCServisI ...) görür; ayrıca dosya yolu verilen kitaplıklar eklenebilir.
    /// </summary>
    public static class KodDerleyici
    {
        private static readonly object kilit = new object();
        private static List<MetadataReference> temelReferanslar;

        /// <summary>Kaynak kodu derler. Hata varsa null döner ve hatalar <paramref name="hata"/>'ya yazılır (eski CodeDom biçimine yakın).</summary>
        public static byte[] Derle(string kaynak, string derlemeAdi, IEnumerable<string> ekReferanslar, out string hata)
        {
            SyntaxTree agac = CSharpSyntaxTree.ParseText(kaynak ?? "", new CSharpParseOptions(LanguageVersion.Latest));
            CSharpCompilation derleme = CSharpCompilation.Create(
                derlemeAdi ?? ("Dinamik_" + Guid.NewGuid().ToString("N")),
                new[] { agac },
                Referanslar(ekReferanslar),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult sonuc = derleme.Emit(ms);
                if (!sonuc.Success)
                {
                    hata = string.Join("\r\n", sonuc.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()));
                    if (hata == "") hata = "Verilen kod derlenemedi!";
                    return null;
                }
                hata = "";
                return ms.ToArray();
            }
        }

        /// <summary>Derlenmiş kodu bellekte, toplanabilir ayrı bir yükleme bağlamına yükler (kullanılmayınca bellekten atılabilir).</summary>
        public static Assembly Yukle(byte[] ikili, string ad)
        {
            AssemblyLoadContext baglam = new AssemblyLoadContext(ad ?? "Dinamik", true);
            using (MemoryStream ms = new MemoryStream(ikili))
                return baglam.LoadFromStream(ms);
        }

        private static List<MetadataReference> Referanslar(IEnumerable<string> ekReferanslar)
        {
            List<MetadataReference> liste = new List<MetadataReference>(TemelReferanslar());
            if (ekReferanslar != null)
            {
                HashSet<string> olanlar = new HashSet<string>(liste.OfType<PortableExecutableReference>()
                    .Select(r => Path.GetFileName(r.FilePath)), StringComparer.OrdinalIgnoreCase);
                foreach (string yol in ekReferanslar)
                {
                    string dosya = DosyaBul(yol);
                    if (dosya != null && olanlar.Add(Path.GetFileName(dosya)))
                        liste.Add(MetadataReference.CreateFromFile(dosya));
                }
            }
            return liste;
        }

        /// <summary>Verilen yol yoksa (eski "bin\\X.dll" yolları) uygulama klasöründe aynı adlı dosya aranır.</summary>
        private static string DosyaBul(string yol)
        {
            if (string.IsNullOrWhiteSpace(yol)) return null;
            if (File.Exists(yol)) return yol;
            string ad = Path.GetFileName(yol.Replace('\\', Path.DirectorySeparatorChar));
            string uygulamada = Path.Combine(AppContext.BaseDirectory, ad);
            return File.Exists(uygulamada) ? uygulamada : null;
        }

        private static List<MetadataReference> TemelReferanslar()
        {
            lock (kilit)
            {
                if (temelReferanslar != null) return temelReferanslar;
                Dictionary<string, string> dosyalar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                // Çalışma zamanının ve uygulamanın bütün kitaplıkları (System.Runtime, System.Private.CoreLib, GNLServis ...)
                string tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
                if (!string.IsNullOrEmpty(tpa))
                    foreach (string yol in tpa.Split(Path.PathSeparator))
                        if (yol.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && File.Exists(yol))
                            dosyalar[Path.GetFileName(yol)] = yol;
                // Sonradan yüklenmiş kitaplıklar (dinamik olmayanlar)
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.IsDynamic || string.IsNullOrEmpty(a.Location) || !File.Exists(a.Location)) continue;
                    string ad = Path.GetFileName(a.Location);
                    if (!dosyalar.ContainsKey(ad)) dosyalar[ad] = a.Location;
                }
                List<MetadataReference> liste = new List<MetadataReference>();
                foreach (string yol in dosyalar.Values)
                {
                    try { liste.Add(MetadataReference.CreateFromFile(yol)); }
                    catch (Exception) { /* .NET kitaplığı olmayan yerel dll'ler atlanır */ }
                }
                temelReferanslar = liste;
                return liste;
            }
        }
    }
}
