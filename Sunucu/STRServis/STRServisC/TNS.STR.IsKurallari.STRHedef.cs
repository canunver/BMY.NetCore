using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.STR.IsKurallari.Veri;
using TNS.PEB;

namespace TNS.STR.IsKurallari
{
    /// <summary>
    /// Stratejik hedeflerle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRHedef
    {
        /// <summary>
        /// Stratejik hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Hedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (hedef.donemBYil <= 0)
                hata = StratejikPlanResI.STR003 + "<br>";
            if (hedef.dil.kod <= 0)
                hata += StratejikPlanResI.STR004 + "<br>";
            if (hedef.strAmac.strAmacKod <= 0)
                hata += StratejikPlanResI.STR007 + "<br>";
            if (string.IsNullOrEmpty(hedef.strHedefKisa))
                hata += StratejikPlanResI.STR020 + "<br>";
            if (string.IsNullOrEmpty(hedef.strHedef))
                hata += StratejikPlanResI.STR021 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABHedef tHedef = new TABHedef(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            StratejikAmac amac = new StratejikAmac(hedef.donemBYil, hedef.strAmac.strAmacKod, "", "", "", "", null, hedef.dil);
            if (!STRGenel.StratejikAmacVarMi(ve, amac))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = StratejikPlanResI.STR022 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (tHedef.HedefVarMi(hedef.donemBYil, hedef.strHedefKod))
                sonuc = tHedef.Guncelle(hedef);
            else if (hedef.strHedefKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = StratejikPlanResI.STR023 + "<br>";
            }
            else
            {
                sonuc = tHedef.Kaydet(hedef);
                hedef.strHedefKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            if (sonuc.islemSonuc)
            {
                StratejikYazi yazi = new StratejikYazi(hedef);
                if (tYazi.YaziVarMi(yazi))
                    sonuc = tYazi.Guncelle(yazi);
                else
                    sonuc = tYazi.Kaydet(yazi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = hedef.strHedefKod.ToString();
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
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, StratejikHedef hedef)
        {
            ObjectArray hedefVAN = new ObjectArray();
            hedefVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!hedefVAN.sonuc.islemSonuc)
                return hedefVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABHedef tHedef = new TABHedef(ve);

            IDataReader dr = tHedef.Listele(hedef);
            while (dr.Read())
            {
                StratejikHedef gelen = new StratejikHedef();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.dil.kod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.dil.ad = STRServis.UzayYaziDegerGetir("STRDIL", gelen.dil.kod.ToString(), true, "");
                gelen.strHedef = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.dosya = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.strAmac.strAmacKod = VeriErisim.DegerAl(dr, 8, 0);
                gelen.strAmac.strAmac = STRServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + gelen.strAmac.strAmacKod.ToString() + "-" + gelen.dil.kod, true, "");
                gelen.strAmac.ebutceKod = STRServis.UzayYaziDegerGetir("STRYAZI", gelen.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + gelen.strAmac.strAmacKod.ToString() + "-" + gelen.dil.kod, true, "EBUTCEKOD");
                gelen.strHedefKisa = VeriErisim.DegerAl(dr, 9, string.Empty);

                hedefVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (hedefVAN.objeler.Count == 0)
                hedefVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

            ve.VeriErisimKapat();
            return hedefVAN;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public static ObjectArray HedefKisaListele(VeriErisim veriErisim, Kullanici kullanan, int donemBYil)
        {
            ObjectArray hedefVAN = new ObjectArray();
            hedefVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!hedefVAN.sonuc.islemSonuc)
                return hedefVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABHedef tHedef = new TABHedef(ve);

            IDataReader dr = tHedef.HedefKisaListele(donemBYil);
            while (dr.Read())
            {
                StratejikHedef gelen = new StratejikHedef();
                gelen.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.strAmac.strAmacKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.strHedefKisa = VeriErisim.DegerAl(dr, 3, string.Empty);

                hedefVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (hedefVAN.objeler.Count == 0)
                hedefVAN.sonuc.bilgiStr = StratejikPlanResI.STR012;

            ve.VeriErisimKapat();
            return hedefVAN;
        }

        /// <summary>
        /// Stratejik hedef, performans hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Hedef nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedefler">Stratejik Hedef nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection hedefler)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR");
            TABHedef tHedef = new TABHedef(ve);
            TABYazi tYazi = new TABYazi(ve);
            ve.BeginTransaction();

            foreach (StratejikHedef h in hedefler)
            {
                //Hedef; Performans Hedefte kullanılmış mı kontrolü
                //*************************************************
                IPEBServis servisPEB = PEB.Arac.Tanimla();
                ObjectArray phler = servisPEB.PerformansHedefListele(kullanan, new PerformansHedef(), false, h.donemBYil, h.strHedefKod, true);
                if (!phler.sonuc.islemSonuc)
                {
                    sonuc = phler.sonuc;
                    break;
                }
                if (phler.objeler.Count > 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(StratejikPlanResI.STR024, h.strHedefKod.ToString());
                    break;
                }
                //*************************************************

                //Hedefin yazı değerleri siliniyor.
                StratejikYazi yazi = new StratejikYazi(h);
                sonuc = tYazi.Sil(yazi);
                if (!sonuc.islemSonuc)
                    break;

                //Hedefin kendisi siliniyor.
                sonuc = tHedef.Sil(h);
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