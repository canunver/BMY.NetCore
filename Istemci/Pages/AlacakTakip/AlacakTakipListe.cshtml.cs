using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Takip İşlemleri (eski AlacakTakip/AlacakTakipListe.aspx): solda sorgu alanları, ortada dosya listesi, sağda seçilen dosyanın
    /// bilgileri. Liste Yazdır (Kısa / Uzun Liste), Durum Değiştir, Dosya Kapat, Yeni Dosya, Kaydet, Hesap Hareket, Ödeme Bilgileri,
    /// Toplu Ödeme Emri ve Muavin Defteri tuşları. İşlemler wwwroot/js/alacaktakip.js ile /api/alacaktakip/takip uçlarından yapılır.
    /// </summary>
    public class AlacakTakipListeModel : AlacakTakipSayfaModel
    {
        public AlacakTakipListeModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => AlacakTakipListeUclari.Yetkili(kullanan, klasor);
    }
}
