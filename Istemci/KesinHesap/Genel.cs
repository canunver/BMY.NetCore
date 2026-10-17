using TNS.KHM;
using TNS.KYM;

namespace KesinHesap
{
    /// <summary>
    /// Genel sınıfı. Eski Istemci/KesinHesap/Genel.cs'ten alındı; değişenler: eski Resources.KesinHesap.FRM017 metni
    /// (App_GlobalResources/KesinHesap.resx) Core'da kaynak dosyası olmadığından FRM017 sabiti olarak eklendi.
    /// </summary>
    public class Genel
    {
        /// <summary>
        /// Eski Resources.KesinHesap.FRM017: yetki yoksa raporların verdiği ileti.
        /// </summary>
        public const string FRM017 = "Verilen kriterler ile bu raporu alma yetkiniz yok.";

        /// <summary>
        /// Görme yetkisini kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı.</param>
        /// <param name="birim">Harcama birimi.</param>
        /// <returns></returns>
        public static bool GormeYetkisiVarMiKesinHesap(Kullanici kullanan, string birim)
        {
            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            return servisKHM.GormeYetkisiVarMi(kullanan, birim);
        }
    }
}
