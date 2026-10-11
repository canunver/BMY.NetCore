using System;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.STR.IsKurallari.Veri;

namespace TNS.STR.IsKurallari
{
    /// <summary>
    /// Stratejik plan modülü iş kuralları sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class STRGenel
    {
        /// <summary>
        /// Degisken.xml dosyasındaki değişkenlere ulaşan servis
        /// </summary>
        public static IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();

        /// <summary>
        /// Verilen misyonun sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Misyon varsa true, yoksa false döndürülür.</returns>
        public static bool MisyonVarMi(VeriErisim ve, Misyon misyon)
        {
            StratejikYazi yazi = new StratejikYazi(misyon);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen vizyonun sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Vizyon varsa true, yoksa false döndürülür.</returns>
        public static bool VizyonVarMi(VeriErisim ve, Vizyon vizyon)
        {
            StratejikYazi yazi = new StratejikYazi(vizyon);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen temanın sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Tema varsa true, yoksa false döndürülür.</returns>
        public static bool TemaVarMi(VeriErisim ve, Tema tema)
        {
            StratejikYazi yazi = new StratejikYazi(tema);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen stratejik amacın sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Stratejik amaç varsa true, yoksa false döndürülür.</returns>
        public static bool StratejikAmacVarMi(VeriErisim ve, StratejikAmac amac)
        {
            StratejikYazi yazi = new StratejikYazi(amac);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }
    }
}