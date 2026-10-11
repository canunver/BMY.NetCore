using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.STR.IsKurallari.Veri;

namespace TNS.STR.IsKurallari
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
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (tema.donemBYil <= 0)
                hata = StratejikPlanResI.STR003 + "<br>";
            if (tema.dil.kod <= 0)
                hata += StratejikPlanResI.STR004 + "<br>";
            if (string.IsNullOrEmpty(tema.temaKisa))
                hata += StratejikPlanResI.STR026 + "<br>";
            if (string.IsNullOrEmpty(tema.tema))
                hata += StratejikPlanResI.STR005 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABTema tTema = new TABTema(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            Vizyon vizyon = new Vizyon(tema.donemBYil, tema.dil, "", "", "");
            if (!STRGenel.VizyonVarMi(ve, vizyon))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = StratejikPlanResI.STR027 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (tTema.TemaVarMi(tema.donemBYil, tema.temaKod))
                sonuc = tTema.Guncelle(tema);
            else if (tema.temaKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = StratejikPlanResI.STR028 + "<br>";
            }
            else
            {
                sonuc = tTema.Kaydet(tema);
                tema.temaKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
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
                sonuc.bilgiStr = StratejikPlanResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = StratejikPlanResI.STR011 + "<br>" + sonuc.hataStr;
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
            temaVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!temaVAN.sonuc.islemSonuc)
                return temaVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABTema tTema = new TABTema(ve);

            IDataReader dr = tTema.Listele(tema);
            while (dr.Read())
            {
                Tema gelen = new Tema();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.temaKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.dil.ad = STRServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                gelen.tema = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.temaKisa = VeriErisim.DegerAl(dr, 8, string.Empty);

                temaVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (temaVAN.objeler.Count == 0)
                temaVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

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
        public static ObjectArray TemaKisaListele(VeriErisim veriErisim, Kullanici kullanan, int donemBYil)
        {
            ObjectArray temaVAN = new ObjectArray();
            temaVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!temaVAN.sonuc.islemSonuc)
                return temaVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABTema tTema = new TABTema(ve);

            IDataReader dr = tTema.TemaKisaListele(donemBYil);
            while (dr.Read())
            {
                Tema gelen = new Tema();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.temaKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.temaKisa = VeriErisim.DegerAl(dr, 2, string.Empty);

                temaVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (temaVAN.objeler.Count == 0)
                temaVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

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
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection temalar)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABTema tTema = new TABTema(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            foreach (Tema t in temalar)
            {
                if (tTema.TemaKullanilmisMi(t.donemBYil, t.temaKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(StratejikPlanResI.STR029, t.temaKod.ToString());
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
                sonuc.bilgiStr = StratejikPlanResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = StratejikPlanResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}