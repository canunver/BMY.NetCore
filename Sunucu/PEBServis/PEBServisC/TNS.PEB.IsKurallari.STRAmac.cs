using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
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
            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, amac.kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (amac.donemBYil <= 0)
                hata = PerformansButceResI.STR003 + "<br>";
            if (amac.dil.kod <= 0)
                hata += PerformansButceResI.STR004 + "<br>";
            //if (amac.tema.temaKod <= 0)
            //    hata += PerformansButceResI.STR005 + "<br>";
            if (string.IsNullOrWhiteSpace(amac.strAmacKisa))
                hata += PerformansButceResI.STR006 + "<br>";
            if (string.IsNullOrWhiteSpace(amac.strAmac))
                hata += PerformansButceResI.STR007 + "<br>";
            if (string.IsNullOrWhiteSpace(amac.kurumKod))
                hata += "Kurum kodunu giriniz!" + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmac tAmac = new TABAmac(ve);
            TABYazi tYazi = new TABYazi(ve);

            Vizyon vizyon = new Vizyon(amac.donemBYil, amac.dil, "", "", "", amac.kurumKod);
            if (!STRGenel.VizyonVarMi(ve, vizyon))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.STR008 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();
            if (tAmac.AmacVarMi(amac.kurumKod, amac.donemBYil, amac.strAmacKod))
                sonuc = tAmac.Guncelle(amac);
            else  if (amac.strAmacKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR009 + "<br>";
            }
            else
            {
                amac.strAmacKod = tAmac.AmacKodAl(amac.kurumKod, amac.donemBYil);
                sonuc = tAmac.Kaydet(amac);
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
            amacVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!amacVAN.sonuc.islemSonuc)
                return amacVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmac tAmac = new TABAmac(ve);

            IDataReader dr = tAmac.Listele(amac);
            if (dr == null)
            {
                amacVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return amacVAN;
            }
            while (dr.Read())
            {
                StratejikAmac gelen = new StratejikAmac();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strAmacKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.strAmac = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 8, string.Empty));

                gelen.tema.temaKod = VeriErisim.DegerAl(dr, 9, 0);
                gelen.strAmacKisa = VeriErisim.DegerAl(dr, 10, string.Empty);

                //gelen.dil.ad = PEBServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                //gelen.tema.tema = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.Tema).ToString() + "-" + gelen.tema.temaKod.ToString() + "-" + gelen.dil.kod, true, "");
                //gelen.tema.ebutceKod = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.Tema).ToString() + "-" + gelen.tema.temaKod.ToString() + "-" + gelen.dil.kod, true, "EBUTCEKOD");
                amacVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (amacVAN.objeler.Count == 0)
                amacVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

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
        public static ObjectArray AmacKisaListele(VeriErisim veriErisim, Kullanici kullanan, string kurumKod, int donemBYil)
        {
            ObjectArray amacVAN = new ObjectArray();
            amacVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!amacVAN.sonuc.islemSonuc)
                return amacVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmac tAmac = new TABAmac(ve);

            IDataReader dr = tAmac.AmacKisaListele(kurumKod, donemBYil);
            while (dr.Read())
            {
                StratejikAmac gelen = new StratejikAmac();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strAmacKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.tema.temaKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.strAmacKisa = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.kurumKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, 4, string.Empty));

                amacVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (amacVAN.objeler.Count == 0)
                amacVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

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
            if (amaclar == null || amaclar.Count == 0)
            {
                return new Sonuc("Silinecek kayıtları gönderiniz!");
            }

            Sonuc sonuc = YetkiSTR.OGYHakkiVarMi(kullanan, ((StratejikAmac)amaclar[0]).kurumKod, BHMv2.ENUMOGY.Yazma);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmac tAmac = new TABAmac(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            foreach (StratejikAmac a in amaclar)
            {
                if (tAmac.AmacKullanilmisMi(a.kurumKod, a.donemBYil, a.strAmacKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.STR013, a.strAmacKod.ToString());
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