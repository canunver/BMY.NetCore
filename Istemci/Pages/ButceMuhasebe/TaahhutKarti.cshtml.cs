using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using OrtakClass;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Sözleşme ve Taahhüt Kartı (eski ButceMuhasebe/TaahhutKarti.aspx; ?tur=2 Personel Taahhütleri). İki sekme: Belge Arama ve
    /// Belge Düzenleme. Teminat / avans / borç / ödeme emri, açılış tahakkuku, değer artışı ve sözleşme üretimi sayfanın kendi
    /// pencereleridir; Taahhüt Planı, Sözleşme Koşulları, Bakım Tarihçesi, Belge Ekle ve Ödeme Emri Kayıt alt sayfa olarak açılır.
    /// İşlemler js/taahhutkarti.js ile /api/taahhutkarti/* uçlarından (Istemci/Muhasebe/TaahhutKartiUclari.cs).
    /// </summary>
    public class TaahhutKartiModel : MuhTanimSayfa
    {
        public TaahhutKartiModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        /// <summary>1 Taahhüt Kartı, 2 Personel Taahhütleri.</summary>
        public int Tur { get; private set; } = 1;

        /// <summary>Sistem yöneticisi ya da Ödeme Emri/MİF kullanıcısı: ödeme ile ilgili tuşlar görünür (eski Page_Load).</summary>
        public bool OdemeYetkili { get; private set; }

        public bool Doviz { get; private set; }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            Tur = Request.Query["tur"] == "2" ? 2 : 1;
            OdemeYetkili = TaahhutKartiUclari.OdemeYetkili(kullanan);
            Doviz = TNS.MUH.Arac.MuhasebeDovizVarMi();
            return TaahhutKartiUclari.Yetkili(kullanan, Klasor, Tur);
        }
    }
}
