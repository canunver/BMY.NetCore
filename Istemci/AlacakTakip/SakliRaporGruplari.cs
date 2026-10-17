using System;
using Istemci.OrtakSayfa;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    /// <summary>Saklı rapor grubu (eski SakliRaporlar.DosyaGrupBilgi: dosyaGrupKod, dosyaGrupAd).</summary>
    public sealed class SakliRaporGrup
    {
        /// <summary>Ekli dosya form kodu (EkliDosyalar.form; modül "ATMAB").</summary>
        public string kod { get; }
        /// <summary>Sekme adı.</summary>
        public string ad { get; }

        public SakliRaporGrup(string kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }
    }

    /// <summary>
    /// Saklı Raporlar (eski AlacakTakip/SakliRaporlar.aspx) grupları ve sayfa yetkisi. Her grup bir sekmedir; sekmede ekli dosyalar
    /// bileşeni (../Duyuru/EkliDosyalarForm?mdl=ATMAB&amp;frm=kod) açılır. AB Takip İşlemleri ekranının "Sakla" raporu üretilen raporu
    /// Gruplar[0] (Borçlu Defterleri, G_BDR) grubuna ATMAB modülüyle saklar (eski SakliRaporlar.dgb[0].dosyaGrupKod).
    /// </summary>
    public static class SakliRaporGruplari
    {
        /// <summary>Ekli dosyalardaki modül kodu.</summary>
        public const string Modul = "ATMAB";

        public const string SayfaAdresi = "../AlacakTakip/SakliRaporlar.aspx";
        public const string ABSayfaAdresi = "../AlacakTakip/AlacakTakipListeAB.aspx";

        /// <summary>Eski SakliRaporlar.dgb (sıra aynı; 0. grup "Sakla" raporunun yeridir).</summary>
        public static readonly SakliRaporGrup[] Gruplar =
        {
            new SakliRaporGrup("G_BDR", "Borçlu Defterleri"),
            new SakliRaporGrup("G_MR", "Mutabakat Raporları"),
            new SakliRaporGrup("G_YBR", "Yıllık Borç Raporları"),
            new SakliRaporGrup("G_DR", "Diğer Raporlar"),
        };

        /// <summary>"Sakla" raporunun saklandığı grubun kodu (Gruplar[0].kod = "G_BDR").</summary>
        public static string SaklaGrupKod => Gruplar[0].kod;

        /// <summary>
        /// Sayfanın kendi menü seçeneği (../AlacakTakip/SakliRaporlar.aspx). Seçenek menu.xml'de Gizli ise MenuOkuyucu onu kimseye
        /// göstermez (yalnızca sistem yöneticisi girer); bu durumda AB Takip İşlemleri menü yetkisi + TNS.ATM.Yetki.SayfayaGirisYapabilirMi
        /// olan kullanıcılar da girer (eski sayfada hiç yetki denetimi yoktu; raporlar AB ekranından saklanır).
        /// </summary>
        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi)) return true;
            if (!SayfaYetki.Yetkili(kullanan, klasor, ABSayfaAdresi)) return false;
            try
            {
                return TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
