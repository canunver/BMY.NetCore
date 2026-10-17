using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Dosya Hesap Hareket Formu (eski AlacakTakip/HesapHareket.aspx): Takip İşlemleri (AlacakTakipListe) ekranından
    /// Sgb.pencere('../AlacakTakip/HesapHareket?yil=2026&amp;dosyaNo=12', 'Hesap Hareket') ile pencerede açılır.
    /// Üstte seçilen işlemin hesap satırları (Tutar girilir), altta dosyanın kayıtlı hareketleri; Kaydet, Sil, Yeni Kayıt, Kapat.
    /// Menüde yoktur; yetkisi HesapHareketUclari.UstSayfalar'dan gelir. İşlemler wwwroot/js/hesaphareket.js ile yapılır.
    /// </summary>
    public class HesapHareketModel : AlacakTakipSayfaModel
    {
        public HesapHareketModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => HesapHareketUclari.Yetkili(kullanan, klasor);
    }
}
