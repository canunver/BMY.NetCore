using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Saklı Raporlar (eski AlacakTakip/SakliRaporlar.aspx): her rapor grubu (SakliRaporGruplari.Gruplar) bir sekme; sekmede ATMAB
    /// modülünün o gruba ekli dosyaları (../Duyuru/EkliDosyalarForm) çerçevede açılır. Çerçeve sekme ilk gösterilince yüklenir
    /// (wwwroot/js/sakliraporlar.js). Yetki: sayfanın menü seçeneği (bkz. SakliRaporGruplari.Yetkili).
    /// </summary>
    public class SakliRaporlarModel : AlacakTakipSayfaModel
    {
        public SakliRaporlarModel(IWebHostEnvironment ortam) : base(ortam) { }

        public SakliRaporGrup[] Gruplar => SakliRaporGruplari.Gruplar;

        public string Modul => SakliRaporGruplari.Modul;

        protected override bool YetkiVar(Kullanici kullanan, string klasor) =>
            SakliRaporGruplari.Yetkili(kullanan, klasor);
    }
}
