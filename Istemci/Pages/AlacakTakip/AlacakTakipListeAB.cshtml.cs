using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// AB (IPARD) Takip İşlemleri (eski AlacakTakip/AlacakTakipListeAB.aspx): solda sorgu alanları ve dosya listesi, sağda seçilen dosyanın
    /// sekmeleri (Borçlu, Ön Borç, Geri Alım Kararı, Geri Ödeme, Yorum / Süreç Durum, Taksitlendirme, İşlem Listesi, Dosyalar, Tarihçe) ve
    /// borç hareketleri. İşlem pencereleri (İşlem Girişi, Borç Bilgisi, Ön Borç, Geri Alım Kararı, Süreç Durumu, Taksit, İmzalar, Dönem Faizi)
    /// sayfadadır. İşlemler wwwroot/js/atmab.js ve atmabpencere.js ile /api/alacaktakip/ab uçlarından yapılır.
    /// </summary>
    public class AlacakTakipListeABModel : AlacakTakipSayfaModel
    {
        public AlacakTakipListeABModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => AlacakTakipListeABUclari.Yetkili(kullanan, klasor);
    }
}
