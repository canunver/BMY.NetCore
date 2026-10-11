using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Misyonla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRMisyon
    {
        /// <summary>
        /// Misyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Misyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Misyon misyon)
        {
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, misyon.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (misyon.donemBYil <= 0)
                sonuc.DegerAta(PerformansButceResI.STR003 + "<br>");
            if (misyon.donemBYil < misyon.aralikBasiYil || misyon.donemBYil > misyon.aralikSonuYil)
                sonuc.DegerAta("Plan yılı dönem aralığı içinde olmalıdır" + "<br>");
            if (misyon.dil.kod <= 0)
                sonuc.DegerAta(PerformansButceResI.STR004 + "<br>");
            if (string.IsNullOrEmpty(misyon.misyon))
                sonuc.DegerAta(PerformansButceResI.STR025 + "<br>");
            if (string.IsNullOrEmpty(misyon.kurumKod))
                sonuc.DegerAta("Kurumsal kod girilmelidir!" + "<br>");

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMisyon tMisyon = new TABMisyon(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            if (!tMisyon.MisyonVarMi(misyon.kurumKod, misyon.donemBYil))
                sonuc = tMisyon.Kaydet(misyon);
            else
                sonuc = tMisyon.Guncelle(misyon);
            if (sonuc.islemSonuc)
            {
                StratejikYazi yazi = new StratejikYazi(misyon);
                if (tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Guncelle(yazi);
                else
                    sonuc = tYazi.Kaydet(yazi);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                sonuc.DegerAta(PerformansButceResI.STR011 + "<br>" + sonuc.hataStr);
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan misyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında misyon nesnesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Misyon misyon)
        {
            ObjectArray misyonVAN = new ObjectArray();
            misyonVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!misyonVAN.sonuc.islemSonuc)
                return misyonVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMisyon tMisyon = new TABMisyon(ve);

            IDataReader dr = tMisyon.Listele(misyon);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Misyon gelen = new Misyon();
                    gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.dil.ad = PEBServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                    gelen.misyon = VeriErisim.DegerAl(dr, 4, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                    gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 8, string.Empty));
                    gelen.aralikBasiYil = VeriErisim.DegerAl(dr, 9, 0);
                    gelen.aralikSonuYil = VeriErisim.DegerAl(dr, 10, 0);
                    gelen.kurumAd = VeriErisim.DegerAl(dr, 11, "");

                    misyonVAN.objeler.Add(gelen);
                }
                dr.Close();
                if (misyonVAN.objeler.Count == 0)
                    misyonVAN.sonuc.bilgiStr = PerformansButceResI.STR012;
            }
            else
                misyonVAN.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();
            return misyonVAN;
        }

        internal static Sonuc Kopyala(VeriErisim veriErisim, Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc s = new Sonuc();
            if (kaynakYil == 0) s.DegerAta("Kaynak yılı giriniz!");
            if (hedefYil == 0) s.DegerAta("Hedef yılı giriniz!");
            if (hedefYil == kaynakYil) s.DegerAta("Hedef ve Kaynak yıl farklı olmalı!");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) s.DegerAta("Bu işlemi sistem yöneticileri yapabilir!");
            if (!s.islemSonuc) return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMisyon tMisyon = new TABMisyon(ve);
            string bilgiStr = "";
            string adim = "Misyon";
            ve.BeginTransaction();
            s = tMisyon.Kopyala(kaynakYil, hedefYil);
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Misyon kopyalandı<br />";
                TABVizyon tVizyon = new TABVizyon(ve);
                adim = "Vizyon";
                s = tVizyon.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Vizyon kopyalandı<br />";
                TABTema tTema = new TABTema(ve);
                adim = "Tema";
                s = tTema.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Tema kopyalandı<br />";
                TABAmac tAmac = new TABAmac(ve);
                adim = "Amaç";
                s = tAmac.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Amaç kopyalandı<br />";
                TABHedef tHedef = new TABHedef(ve);
                adim = "Hedef";
                s = tHedef.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Hedef kopyalandı<br />";
                TABSTRHEDEFACIKLAMA tHedefAciklama = new TABSTRHEDEFACIKLAMA(ve);
                adim = "Açıklama";
                s = tHedefAciklama.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Açıklama kopyalandı<br />";
                TABSTRHEDEFBIRIM tHedefBirim = new TABSTRHEDEFBIRIM(ve);
                adim = "Birim";
                s = tHedefBirim.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet birim kopyalandı<br />";
                TABSTRHEDEFUSTPOLITIKABELGESI tHedefPol = new TABSTRHEDEFUSTPOLITIKABELGESI(ve);
                adim = "Politika";
                s = tHedefPol.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Politika Belgesi kopyalandı<br />";
                TABSTRHEDEFGOSTERGE tHedefGos = new TABSTRHEDEFGOSTERGE(ve);
                adim = "Hedef gösterge";
                s = tHedefGos.Kopyala(kaynakYil, hedefYil);
            }
            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Gösterge kopyalandı<br />";
                TABYazi tYazi = new TABYazi(ve);
                adim = "Yazı";
                s = tYazi.Kopyala(kaynakYil, hedefYil);
            }

            if (s.islemSonuc)
            {
                bilgiStr += $"{s.kayitSay} adet Açıklama kopyalandı<br />";
                ve.Commit();
                s.bilgiStr = bilgiStr;
            }
            else
            {
                s.hataStr = s.hataStr + " (" + adim + ")";
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}