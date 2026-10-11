using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Temalarla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRTema
    {
        /// <summary>
        /// Tema varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tema nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Tema tema)
        {
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, tema.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (tema.donemBYil <= 0)
                hata = PerformansButceResI.STR003 + "<br>";
            if (tema.dil.kod <= 0)
                hata += PerformansButceResI.STR004 + "<br>";
            if (string.IsNullOrEmpty(tema.temaKisa))
                hata += PerformansButceResI.STR026 + "<br>";
            if (string.IsNullOrEmpty(tema.tema))
                hata += PerformansButceResI.STR005 + "<br>";
            if (string.IsNullOrEmpty(tema.kurumKod))
                hata += "Kurumsal kod girilmelidir!" + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTema tTema = new TABTema(ve);
            TABYazi tYazi = new TABYazi(ve);

            if (!STRGenel.MisyonVarMi(ve, tema.donemBYil, tema.dil.kod, tema.kurumKod))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.STR031 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();

            if (tTema.TemaVarMi(tema.kurumKod, tema.donemBYil, tema.temaKod))
                sonuc = tTema.Guncelle(tema);
            else if (tema.temaKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR028 + "<br>";
            }
            else
            {
                tema.temaKod = tTema.TemaKodAl(tema.kurumKod, tema.donemBYil);
                if (string.IsNullOrWhiteSpace(ve.hata))
                    sonuc = tTema.Kaydet(tema);
                else
                    sonuc.DegerAta(ve.hata);
            }

            if (sonuc.islemSonuc)
            {
                StratejikYazi yazi = new StratejikYazi(tema);
                if (tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Guncelle(yazi);
                else
                    sonuc = tYazi.Kaydet(yazi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = tema.temaKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tema bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tema tema)
        {
            ObjectArray temaVAN = new ObjectArray();
            temaVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!temaVAN.sonuc.islemSonuc)
                return temaVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTema tTema = new TABTema(ve);

            IDataReader dr = tTema.Listele(tema);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Tema gelen = new Tema();
                    gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.temaKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.dil.ad = PEBServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                    gelen.tema = VeriErisim.DegerAl(dr, 4, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                    gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 8, string.Empty));
                    gelen.temaKisa = VeriErisim.DegerAl(dr, 9, string.Empty);

                    temaVAN.objeler.Add(gelen);
                }
                dr.Close();
            }
            else
                temaVAN.sonuc.DegerAta(ve.hata);
            if (temaVAN.objeler.Count == 0)
                temaVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

            ve.VeriErisimKapat();
            return temaVAN;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait tema özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        public static ObjectArray TemaKisaListele(VeriErisim veriErisim, Kullanici kullanan, string kurumKod, int donemBYil)
        {
            ObjectArray temaVAN = new ObjectArray();
            temaVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!temaVAN.sonuc.islemSonuc)
                return temaVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTema tTema = new TABTema(ve);

            IDataReader dr = tTema.TemaKisaListele(kurumKod, donemBYil);
            while (dr.Read())
            {
                Tema gelen = new Tema();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.temaKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.temaKisa = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 3, string.Empty));
                temaVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (temaVAN.objeler.Count == 0)
                temaVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

            ve.VeriErisimKapat();
            return temaVAN;
        }

        /// <summary>
        /// Tema, stratejik amaçta kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Tema nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="temalar">Tema nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kurumKod, TNSCollection temalar)
        {
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTema tTema = new TABTema(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            foreach (Tema t in temalar)
            {
                t.kurumKod = kurumKod;
                if (tTema.TemaKullanilmisMi(t.kurumKod, t.donemBYil, t.temaKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.STR029, t.temaKod.ToString());
                    break;
                }

                //Temanın yazı değerleri siliniyor.
                StratejikYazi yazi = new StratejikYazi(t);
                sonuc = tYazi.Sil(yazi);
                if (!sonuc.islemSonuc)
                    break;

                //Temanın kendisi siliniyor.
                sonuc = tTema.Sil(t);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}