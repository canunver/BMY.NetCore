using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.STR.IsKurallari.Veri;

namespace TNS.STR.IsKurallari
{
    /// <summary>
    /// Stratejik amaçlarla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRAmac
    {
        /// <summary>
        /// Stratejik amaç varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Amaç nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, StratejikAmac amac)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (amac.donemBYil <= 0)
                hata = StratejikPlanResI.STR003 + "<br>";
            if (amac.dil.kod <= 0)
                hata += StratejikPlanResI.STR004 + "<br>";
            if (amac.tema.temaKod <= 0)
                hata += StratejikPlanResI.STR005 + "<br>";
            if (string.IsNullOrEmpty(amac.strAmacKisa))
                hata += StratejikPlanResI.STR006 + "<br>";
            if (string.IsNullOrEmpty(amac.strAmac))
                hata += StratejikPlanResI.STR007 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABAmac tAmac = new TABAmac(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            Tema tema = new Tema(amac.donemBYil, amac.tema.temaKod, "", "", "", "", amac.dil);
            if (!STRGenel.TemaVarMi(ve, tema))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = StratejikPlanResI.STR008 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (tAmac.AmacVarMi(amac.donemBYil, amac.strAmacKod))
                sonuc = tAmac.Guncelle(amac);
            else if (amac.strAmacKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = StratejikPlanResI.STR009 + "<br>";
            }
            else
            {
                sonuc = tAmac.Kaydet(amac);
                amac.strAmacKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            if (sonuc.islemSonuc)
            {
                StratejikYazi yazi = new StratejikYazi(amac);
                if (tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Guncelle(yazi);
                else
                    sonuc = tYazi.Kaydet(yazi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = amac.strAmacKod.ToString();
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
        /// Verilen kriterlere uygun olan stratejik amaç bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, StratejikAmac amac)
        {
            ObjectArray amacVAN = new ObjectArray();
            amacVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!amacVAN.sonuc.islemSonuc)
                return amacVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABAmac tAmac = new TABAmac(ve);

            IDataReader dr = tAmac.Listele(amac);
            while (dr.Read())
            {
                StratejikAmac gelen = new StratejikAmac();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strAmacKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.dil.ad = STRServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                gelen.strAmac = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.tema.temaKod = VeriErisim.DegerAl(dr, 8, 0);
                gelen.tema.tema = STRServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.Tema).ToString() + "-" + gelen.tema.temaKod.ToString() + "-" + gelen.dil.kod, true, "");
                gelen.tema.ebutceKod = STRServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.Tema).ToString() + "-" + gelen.tema.temaKod.ToString() + "-" + gelen.dil.kod, true, "EBUTCEKOD");
                gelen.strAmacKisa = VeriErisim.DegerAl(dr, 9, string.Empty);

                amacVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (amacVAN.objeler.Count == 0)
                amacVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

            ve.VeriErisimKapat();
            return amacVAN;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik amaç özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        public static ObjectArray AmacKisaListele(VeriErisim veriErisim, Kullanici kullanan, int donemBYil)
        {
            ObjectArray amacVAN = new ObjectArray();
            amacVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!amacVAN.sonuc.islemSonuc)
                return amacVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABAmac tAmac = new TABAmac(ve);

            IDataReader dr = tAmac.AmacKisaListele(donemBYil);
            while (dr.Read())
            {
                StratejikAmac gelen = new StratejikAmac();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strAmacKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.tema.temaKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.strAmacKisa = VeriErisim.DegerAl(dr, 3, string.Empty);

                amacVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (amacVAN.objeler.Count == 0)
                amacVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

            ve.VeriErisimKapat();
            return amacVAN;
        }

        /// <summary>
        /// Stratejik amaç, stratejik hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Amaç nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amaclar">Stratejik Amaç nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection amaclar)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABAmac tAmac = new TABAmac(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            foreach (StratejikAmac a in amaclar)
            {
                if (tAmac.AmacKullanilmisMi(a.donemBYil, a.strAmacKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(StratejikPlanResI.STR013, a.strAmacKod.ToString());
                    break;
                }

                //Amacın yazı değerleri siliniyor.
                StratejikYazi yazi = new StratejikYazi(a);
                sonuc = tYazi.Sil(yazi);
                if (!sonuc.islemSonuc)
                    break;

                //Amacın kendisi siliniyor.
                sonuc = tAmac.Sil(a);
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