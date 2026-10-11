using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// İş/görev bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBIsGorev
    {
        /// <summary>
        /// İş/görev varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IsGorev nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de ilgili yordamlar çağırılarak kayıt edilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMiOrgKod(kullanan, ig.orgKod);
            if (!sonuc.islemSonuc)
                return sonuc;
            object eski = null, yeni = null; //Tarihçe için
            string hata = string.Empty;
            
            ENUMTARIHCEISLEMTIP islemTip= new ENUMTARIHCEISLEMTIP();

            if (ig.orgKod <= 0)
                hata += PerformansButceResI.PEB005 + "<br>";
            //else if (!PEBGenel.KurKodVarMi(ig.yil, ig.orgKod)) ???
            //    hata += PerformansButceResI.PEB006 + "<br>";
            if (ig.af.afKod <= 0)
                hata += PerformansButceResI.PEB018 + "<br>";
            if (string.IsNullOrEmpty(ig.isGorevAd))
                hata += PerformansButceResI.PEB078 + "<br>";
            if (string.IsNullOrEmpty(ig.hedef))
                hata += PerformansButceResI.PEB139 + "<br>";
            if (string.IsNullOrEmpty(ig.aciklama))
                hata += PerformansButceResI.PEB138 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }          


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            eski = PEBIsGorev.ListeleIsGorev(ve,kullanan,ig)[0];
            ig.af.yil = ig.yil;
            ig.af.orgKod = ig.orgKod;
            
            if (!PEBGenel.AltFaaliyetVarMi(ve, ig.af))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB079 + "<br>";
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABIsGorev tIG = new TABIsGorev(ve);
            ve.BeginTransaction();

            if (tIG.IsGorevVarMi(ig))
            { sonuc = tIG.Guncelle(ig);
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
            }
                
            else if (ig.isGorevKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB080 + "<br>";
            }
            else
            {
                sonuc = tIG.Kaydet(ig);
                ig.isGorevKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
            }

            yeni = ig;
            Sonuc sonucTarihce = new Sonuc();

            if (sonuc.islemSonuc)
                sonucTarihce = STROperasyonelPlanTarihce.Kaydet(ve, kullanan, yeni, eski, ENUMTARIHCE.ISGOREVDETAY, islemTip,ig);

            if (sonuc.islemSonuc)
                sonuc = PEBIsGorevZamanlama.Kaydet(ve, kullanan, ig);
            if(sonuc.islemSonuc)
            {
                //eski=
            }
            if (sonuc.islemSonuc)
                sonuc = PEBIsGorevSorumlu.Kaydet(ve, kullanan, ig);

            if (sonuc.islemSonuc)
                sonuc = PEBIsGorevGosterge.Kaydet(ve, kullanan, ig);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = ig.isGorevKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
                yeni = ig;
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig, bool detayVer)
        {
            return Listele(veriErisim, kullanan, ig, string.Empty, detayVer, false);
        }

        /// <summary>
        /// Parametre olarak verilen personelin iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tcKimlikNo">İş/görev bilgileri istenen personelin T.C. kimlik numarası</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public static ObjectArray PersonelIsGorevListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string tcKimlikNo, bool detayVer)
        {
            IsGorev ig = new IsGorev();
            ig.yil = yil;

            return Listele(veriErisim, kullanan, ig, tcKimlikNo, detayVer, false);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public static ObjectArray Yazdir(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            return Listele(veriErisim, kullanan, ig, string.Empty, true, true);
        }


        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="tcKimlikNo">Varsa iş/görev bilgileri istenen personelin T.C. kimlik numarası</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <param name="yazdir">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        private static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig, string tcKimlikNo, bool detayVer, bool yazdir)
        {
            ObjectArray igVAN = new ObjectArray();
            igVAN.sonuc = Yetki.OkumaHakkiVarMiOrgKod(kullanan, ig.orgKod);
            if (!igVAN.sonuc.islemSonuc)
                return igVAN;

            string hata = string.Empty;
            if (yazdir)
            {
                if (ig.orgKod <= 0)
                    hata += PerformansButceResI.PEB005 + "<br>";
                //if (ig.af.daire.daireKod <= 0)
                //    hata += PerformansButceResI.PEB007 + "<br>";
            }
            else if (detayVer)
            {
                if (ig.orgKod <= 0)
                    hata += PerformansButceResI.PEB005 + "<br>";
                if (ig.isGorevKod <= 0)
                    hata += PerformansButceResI.PEB081 + "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                igVAN.sonuc.islemSonuc = false;
                igVAN.sonuc.hataStr = hata;
                return igVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorev tIG = new TABIsGorev(ve);

            IDataReader dr = tIG.Listele(ig, tcKimlikNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    IsGorev gelen = new IsGorev();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.orgKod = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, 1, string.Empty), 0);
                    //gelen.orgKod = PEBGenel.KurKodNoktala(gelen.orgKod);
                    //gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.orgKod, true, "");
                    gelen.isGorevKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.af.afKod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.isGorevAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.hedef = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.tekrarSikligi = VeriErisim.DegerAl(dr, 7, 0);

                    gelen.af.afAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                    gelen.af.fp.fpKod = VeriErisim.DegerAl(dr, 9, 0);
                    gelen.af.fp.fpAd = VeriErisim.DegerAl(dr, 10, string.Empty);
                    gelen.af.sm.smKod = VeriErisim.DegerAl(dr, 11, 0);
                    gelen.af.sm.smAd = VeriErisim.DegerAl(dr, 12, string.Empty);
                    gelen.af.fp.kurKod = VeriErisim.DegerAl(dr, 13, string.Empty);
                    gelen.af.sm.kurKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                    //gelen.af.daire.daireKod = VeriErisim.DegerAl(dr, 11, 0);
                    //gelen.af.daire.daireAd = VeriErisim.DegerAl(dr, 12, string.Empty);

                    if (detayVer)
                    {
                        ObjectArray igZamanlama = PEBIsGorevZamanlama.Listele(ve2, kullanan, gelen);
                        if (!igZamanlama.sonuc.islemSonuc)
                        {
                            igVAN.sonuc = igZamanlama.sonuc;
                            break;
                        }
                        else
                            gelen.zamanlamalar = igZamanlama.objeler;

                        ObjectArray igSorumlu = PEBIsGorevSorumlu.Listele(ve2, kullanan, gelen);
                        if (!igSorumlu.sonuc.islemSonuc)
                        {
                            igVAN.sonuc = igSorumlu.sonuc;
                            break;
                        }
                        else
                            gelen.sorumlular = igSorumlu.objeler;

                        ObjectArray igGosterge = PEBIsGorevGosterge.Listele(ve2, kullanan, gelen);
                        if (!igGosterge.sonuc.islemSonuc)
                        {
                            igVAN.sonuc = igGosterge.sonuc;
                            break;
                        }
                        else
                            gelen.gostergeler = igGosterge.objeler;
                    }

                    igVAN.objeler.Add(gelen);
                }
                dr.Close();
            }
            if (igVAN.objeler.Count == 0)
                igVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return igVAN;
        }

        /// <summary>
        /// İş/görev bilgilerini, zaman planını, sorumlu personellerini ve performans göstergelerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="igler">IsGorev nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection igler)
        {
            Sonuc sonuc = new Sonuc(false, PerformansButceResI.PEB040, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorev tIG = new TABIsGorev(ve);
            TABIsGorevZamanlama tZamanlama = new TABIsGorevZamanlama(ve);
            TABIsGorevSorumlu tSorumlu = new TABIsGorevSorumlu(ve);
            TABIsGorevGosterge tGosterge = new TABIsGorevGosterge(ve);
            ve.BeginTransaction();

            foreach (IsGorev ig in igler)
            {
                sonuc = Yetki.YazmaHakkiVarMiOrgKod(kullanan, ig.orgKod);
                if (!sonuc.islemSonuc)
                    break;

                //if (tIG.IsGorevKullanilmisMi(ig))
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = "Daha önce kullanıldığı için " + ig.kurKod + " kodlu birimin " + ig.isGorevKod.ToString() + " kodlu İş/Görevi silinemez!";
                //    break;
                //}

                //İş/Görevin zamanlamaları siliniyor.
                sonuc = tZamanlama.Sil(ig);
                if (!sonuc.islemSonuc)
                    break;

                //İş/Görevin sorumluları siliniyor.
                sonuc = tSorumlu.Sil(ig);
                if (!sonuc.islemSonuc)
                    break;

                //İş/Görevin göstergeleri siliniyor.
                sonuc = tGosterge.Sil(ig);
                if (!sonuc.islemSonuc)
                    break;

                //İş/Görevin kendisi siliniyor.
                sonuc = tIG.Sil(ig);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.PEB020;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr;
            }


            ve.VeriErisimKapat();
            return sonuc;
        }

        private static ObjectArray ListeleIsGorev(VeriErisim veriErisim, Kullanici kullanan, IsGorev ig)
        {
            ObjectArray igVAN = new ObjectArray();
            igVAN.sonuc = Yetki.OkumaHakkiVarMiOrgKod(kullanan, ig.orgKod);
            if (!igVAN.sonuc.islemSonuc)
                return igVAN;

            string hata = string.Empty;

            if (!string.IsNullOrEmpty(hata))
            {
                igVAN.sonuc.islemSonuc = false;
                igVAN.sonuc.hataStr = hata;
                return igVAN;            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsGorev tIG = new TABIsGorev(ve);

            IDataReader dr = tIG.ListeleIsGorev(ig);
            if (dr != null)
            {
                while (dr.Read())
                {
                    IsGorev gelen = new IsGorev();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.orgKod = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, 1, string.Empty), 0);                 
                    gelen.isGorevKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.af.afKod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.isGorevAd = VeriErisim.DegerAl(dr,4, string.Empty);
                    gelen.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.hedef = VeriErisim.DegerAl(dr, 6, string.Empty);
                    gelen.tekrarSikligi = VeriErisim.DegerAl(dr, 7, 0);     

                    igVAN.objeler.Add(gelen);
                }
                dr.Close();
            }
            if (igVAN.objeler.Count == 0)
                igVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;
            ve.VeriErisimKapat();
            return igVAN;
        }
    }
}