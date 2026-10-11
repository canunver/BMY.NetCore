using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Collections.Generic;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BKDBirimButceHazirlik formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BKDBirimButceHazirlik
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        static TNS.MUH.IMUHServis _servisMuh = null;
        static TNS.MUH.IMUHServis servisMuh { get { if (_servisMuh == null) _servisMuh = TNS.MUH.Arac.Tanimla(); return _servisMuh; } }
        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="saymanlik">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool HasWriteAccess(Kullanici kullanan, int saymanlik, string birim)
        {
            return servisMuh.MuhasebeBirimYetkiVarMi(kullanan, saymanlik.ToString(), birim, MUH.ENUMOGY.Yazma);
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="saymanlik">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool HasReadAccess(Kullanici kullanan, int saymanlik, string birim)
        {
            bool yetkiVar = servisMuh.MuhasebeBirimYetkiVarMi(kullanan, saymanlik.ToString(), birim, MUH.ENUMOGY.Gorme);
            if (yetkiVar) return yetkiVar;
            if (birim == "250")
            {
                return IlKullanicisiMi(kullanan, saymanlik / 1000);
            }
            return yetkiVar;
        }

        public static bool IlKullanicisiMi(Kullanici kullanan, int il)
        {
            bool yetkiVar = servisMuh.MuhasebeBirimYetkiVarMi(kullanan, (il * 1000 + 100).ToString(), "260", MUH.ENUMOGY.Gorme);
            if (yetkiVar) return yetkiVar;
            return servisMuh.MuhasebeBirimYetkiVarMi(kullanan, (il * 1000 + 150).ToString(), "260", MUH.ENUMOGY.Gorme);
        }

        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Sakla(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, TertipOdenek[] tertipler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (saymanlik <= 0 || string.IsNullOrWhiteSpace(birim) || birim.Length != 3)
            {
                s.DegerAta("Lütfen saymanlık ve birim seçiniz");
                return s;
            }
            if (senaryoKod.Replace(".", "") == "0000")
            {
                if (!HasWriteAccess(kullanan, saymanlik, birim))
                {
                    s.DegerAta(ButceKodResI.BirimButceHazirlik001);
                    return s;
                }
            }
            else
            {
                for (int i = 0; i < tertipler.Length; i++)
                {
                    if (!YetkiServis.OGYYetkisiVarMi(kullanan, "", tertipler[i].tertip, ENUMOGY.Yazma))
                    {
                        s.DegerAta(String.Format("Verdiğiniz tertibi ({0}) değiştirme yetkiniz bulunmamaktadır!", tertipler[i].tertip.TertipStrOl()));
                        return s;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tab = new TABBKDBIRIMBUTCEHAZIRLIK(ve);

            int onayKod = tab.OnaykodBul(ENUMBirimOnayTur.BIRIMBUTCEHAZIRLIK, yil, senaryoKod, saymanlik, birim);
            if (onayKod == 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Biriminiz bütçe teklif verileri onaylı olduğundan değişiklik yapamazsınız. Üst birimizle iletişime geçerek onay kaldırılabilir.");
            }

            for (int i = 0; i < tertipler.Length; i++)
            {
                if (tertipler[i].odenek[0] < 0)
                    s.DegerAta(string.Format("{0} tertibinin miktarı negatif girilmiş.", tertipler[i].tertip.TertipStrOl()));
                if (senaryoKod == "00.00")
                {
                    if (tertipler[i].odenek[0] > 0 && string.IsNullOrWhiteSpace(tertipler[i].tertip.tertipAdi))
                        s.DegerAta(string.Format("{0} tertibi için lütfen gerekçe yazınız!", tertipler[i].tertip.TertipStrOl()));
                }
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();

                for (int i = 0; i < tertipler.Length; i++)
                {
                    if (s.islemSonuc)
                        s = tab.Delete(yil, saymanlik, birim, senaryoKod, tertipler[i].tertip);

                    if (s.islemSonuc && tertipler[i].odenek[0] > 0)
                        s = tab.Insert(yil, saymanlik, birim, senaryoKod, tertipler[i].tertip, tertipler[i].odenek[0], tertipler[i].tertip.tertipAdi);
                }
                if (s.islemSonuc)
                    s = tab.OnaykodTarihceYaz(ENUMBirimOnayTur.BIRIMBUTCEHAZIRLIK, yil, senaryoKod, saymanlik, birim, -1, kullanan.kullaniciKodu);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static int OnayKodOkuTasra(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tab = new TABBKDBIRIMBUTCEHAZIRLIK(ve);

            int onayKod = tab.OnaykodBul(onayTur, yil, senaryoKod, saymanlik, birim);
            ve.VeriErisimKapat();
            return onayKod;
        }


        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static ObjectArray OnayKodListeleTasra(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur)
        {
            ObjectArray oa = new ObjectArray();
            if (!HasReadAccess(kullanan, saymanlik, birim))
            {
                oa.sonuc.DegerAta(ButceKodResI.BirimButceHazirlik001);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tab = new TABBKDBIRIMBUTCEHAZIRLIK(ve);

            int onayKod = tab.OnaykodBul(onayTur, yil, senaryoKod, saymanlik, birim);

            if (!string.IsNullOrWhiteSpace(ve.hata))
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }
            oa.sonuc.anahtar = onayKod.ToString();
            IDataReader dr = tab.OnaykodTarihceListele(onayTur, yil, senaryoKod, saymanlik, birim);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }

            while (dr.Read())
            {
                IslemTarihce it = new IslemTarihce();
                it.YIL = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.YIL, 0);
                it.SENARYOKOD = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.SENARYOKOD, "");
                it.MUHASEBEKOD = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.MUHASEBEKOD, 0);
                it.BIRIMKOD = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.BIRIMKOD, "");
                it.ONAYTUR = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.ONAYTUR, 0);
                it.ISLEMTARIH = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.ISLEMTARIH);
                it.ONAYKOD = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.ONAYKOD, 0);
                it.ISLEMIYAPAN = VeriErisim.DegerAl(dr, (int)TABBKDBIRIMBUTCEHAZIRLIK.ENUMFieldsONAYTARIHCE.ISLEMIYAPAN, "");
                oa.Ekle(it);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static ObjectArray ListeleTekBirim(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int saymanlik, string birim, string senaryo)
        {
            int eko2Uz = tertip.KodUzunlukBul(3, 2, true);

            ObjectArray oa = new ObjectArray();
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            Tertip t = new Tertip(tertip.yil, tertip.tabloKod);
            string kurKod = "";
            if (saymanlik < 1000 && birim == "000")
            {
                kurKod = tertip.kodlar[0];
                if (string.IsNullOrWhiteSpace(kurKod))
                    kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            }
            else
                kurKod = TNS.MUH.Arac.Tanimla().SaymanlikBirimKurkodBul(saymanlik, birim);
            tertip.kodlar[0] = kurKod;

            string[] sorgular = new string[4];
            string[] baglantilar = new string[4];
            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", saymanlik.ToString(), birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[0] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[0] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", saymanlik.ToString(), birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            string oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), saymanlik.ToString(), birim, "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[2] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), true, "SUM(BORC-ALACAK)", "H.", "", eko2Uz);
            baglantilar[2] = "BaglantiSatiriBUM";

            sorgular[3] += $"SELECT DISTINCT BHMTERTIPLISTESIGIDER.KURKOD, BHMTERTIPLISTESIGIDER.FONKOD, BHMTERTIPLISTESIGIDER.FINKOD, SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) AS Expr1, BHMTERTIPLISTESIGIDER.PEBKOD, ";
            sorgular[3] += "ButceHazirlik.dbo.ABSEKO.EKOAD, ButceHazirlik.dbo.ABSPEBKOD.PEBAD ";
            sorgular[3] += "FROM BHMTERTIPLISTESIGIDER INNER JOIN ";
            sorgular[3] += $"ButceHazirlik.dbo.ABSEKO ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sorgular[3] += "ButceHazirlik.dbo.ABSPEBKOD ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSPEBKOD.YIL AND BHMTERTIPLISTESIGIDER.PEBKOD = ButceHazirlik.dbo.ABSPEBKOD.PEBKOD ";
            sorgular[3] += "WHERE (BHMTERTIPLISTESIGIDER.YIL = " + tertip.yil.ToString() + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.KURKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FONKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FINKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.EKOKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.PEBKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "") + "%") + ") ";

            sorgular[3] += "order by 1,2,3,4,5";
            baglantilar[3] = "BaglantiSatiriBHMv2";


            //Birim grupla
            bool sifirlarVar = false;
            if (!string.IsNullOrWhiteSpace(tertip.kodlar[4])) sifirlarVar = true;

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.saymanlik = saymanlik;
                hk.birim = birim;
                hk.tertipTam = "";
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + alAnahtar.StrAl(3) + "-" + alAnahtar.StrAl(4));
                hk.planlanan = alVeri.DblAl(0);
                hk.talepEdilen = alVeri.DblAl(1);
                hk.harcama = alVeri.DblAl(2);
                hk.ekoad = alVeri.StrAl(3);
                hk.pebad = alVeri.StrAl(4);
                if (sifirlarVar || (hk.planlanan != 0 || hk.talepEdilen != 0 || hk.harcama != 0))
                {
                    if (string.IsNullOrWhiteSpace(hk.ekoad)) hk.ekoad = EkoAdBul(kullanan, servis, htEko, tertip.yil, alAnahtar.StrAl(3));
                    if (string.IsNullOrWhiteSpace(hk.pebad)) hk.pebad = PebAdBul(kullanan, servis, htPeb, tertip.yil, alAnahtar.StrAl(4));
                    oa.Ekle(hk);
                }
            }

            sorgular = new string[3];
            baglantilar = new string[3];

            sorgular[0] = TertipKosulOlBBH(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), "");
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), false, "SUM(BORC-ALACAK)", "H.", "", eko2Uz);
            baglantilar[1] = "BaglantiSatiriBUM";

            sorgular[2] += "SELECT DISTINCT BHMTERTIPLISTESIGIDER.KURKOD, BHMTERTIPLISTESIGIDER.FONKOD, BHMTERTIPLISTESIGIDER.FINKOD,  BHMTERTIPLISTESIGIDER.EKOKOD, BHMTERTIPLISTESIGIDER.PEBKOD, ";
            sorgular[2] += "ButceHazirlik.dbo.ABSEKO.EKOAD, ButceHazirlik.dbo.ABSPEBKOD.PEBAD ";
            sorgular[2] += "FROM BHMTERTIPLISTESIGIDER INNER JOIN ";
            sorgular[2] += "ButceHazirlik.dbo.ABSEKO ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND BHMTERTIPLISTESIGIDER.EKOKOD = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sorgular[2] += "ButceHazirlik.dbo.ABSPEBKOD ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSPEBKOD.YIL AND BHMTERTIPLISTESIGIDER.PEBKOD = ButceHazirlik.dbo.ABSPEBKOD.PEBKOD ";
            sorgular[2] += "WHERE (BHMTERTIPLISTESIGIDER.YIL = " + tertip.yil.ToString() + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.KURKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.FONKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.FINKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.EKOKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.PEBKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "") + "%") + ") ";

            sorgular[2] += "order by 1,2,3,4,5";
            baglantilar[2] = "BaglantiSatiriBHMv2";

            ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.saymanlik = saymanlik;
                hk.birim = birim;

                string eko4 = alAnahtar.StrAl(3);
                string eko2 = eko4.Substring(0, eko2Uz);
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + eko2 + "-" + alAnahtar.StrAl(4));
                hk.tertipTam = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + eko4 + "-" + alAnahtar.StrAl(4));
                hk.planlanan = 0;
                hk.talepEdilen = alVeri.DblAl(0);
                hk.aciklama = alVeri.StrAl(1);
                hk.harcama = alVeri.DblAl(2);
                hk.ekoad = alVeri.StrAl(3);
                hk.pebad = alVeri.StrAl(4);
                if (sifirlarVar || (hk.planlanan != 0 || hk.talepEdilen != 0 || hk.harcama != 0))
                {
                    if (string.IsNullOrWhiteSpace(hk.ekoad)) hk.ekoad = EkoAdBul(kullanan, servis, htEko, tertip.yil, eko4);
                    if (string.IsNullOrWhiteSpace(hk.pebad)) hk.pebad = PebAdBul(kullanan, servis, htPeb, tertip.yil, alAnahtar.StrAl(4));
                    oa.Ekle(hk);
                }
            }

            oa.objeler.Sort(new BirimButceHazirlikKarsilanan());
            return oa;
        }

        public static string PebAdBul(Kullanici kullanan, IUZYServis servis, Hashtable htPeb, int yil, string pebKod)
        {
            return KodBul(kullanan, servis, htPeb, yil, pebKod, "ABSPEBKOD");
        }

        public static string EkoAdBul(Kullanici kullanan, IUZYServis servis, Hashtable htEko, int yil, string ekoKod)
        {
            return KodBul(kullanan, servis, htEko, yil, ekoKod, "ABSEKO");
        }

        public static string FinAdBul(Kullanici kullanan, IUZYServis servis, Hashtable htFin, int yil, string finKod)
        {
            return KodBul(kullanan, servis, htFin, yil, finKod, "ABSFIN");
        }

        public static string SektorAdBul(Kullanici kullanan, IUZYServis servis, Hashtable htSektor, int yil, string sektorKod)
        {
            return KodBul(kullanan, servis, htSektor, yil, sektorKod, "ABSSEKTOR");
        }

        private static string KodBul(Kullanici kullanan, IUZYServis servis, Hashtable ht, int yil, string kod, string uzayAd)
        {
            string donDeger = null;
            try
            {
                donDeger = (string)ht[kod];
            }
            catch { donDeger = null; }
            if (!string.IsNullOrWhiteSpace(donDeger))
                return donDeger;
            donDeger = servis.UzayDegeriStr(kullanan, uzayAd, yil.ToString() + "-" + kod, true);
            ht[kod] = donDeger;
            return donDeger;
        }

        public static ObjectArray ListeleGrup(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int saymanlik, string birim, string senaryo, bool eko2Grup)
        {
            ObjectArray oa = new ObjectArray();
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            Tertip t = new Tertip(tertip.yil, tertip.tabloKod);
            int eko2Uz = t.KodUzunlukBul(3, 2, true);
            string kurKod = "";
            string saymanlikStr = saymanlik.ToString();
            if (saymanlik < 1000 && birim == "000")
            {
                kurKod = tertip.kodlar[0];
                if (string.IsNullOrWhiteSpace(kurKod))
                    kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            }
            else
            {
                kurKod = TNS.MUH.Arac.Tanimla().SaymanlikBirimKurkodBul(saymanlik, birim);
                if (saymanlik > 1000 && birim == "260")
                {
                    int ilKod = saymanlik / 1000;
                    saymanlikStr = (ilKod * 1000).ToString() + "/" + (ilKod * 1000 + 999).ToString();
                    birim = "250,260";
                }
            }
            tertip.kodlar[0] = kurKod;

            string[] sorgular = new string[3];
            string[] baglantilar = new string[3];
            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", saymanlikStr, birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[0] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), true, "SUM(ODENEK)", "", "SAYMANLIKKOD, BIRIMKOD, ", eko2Uz, eko2Grup);
            baglantilar[0] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", saymanlikStr, birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), true, "SUM(ODENEK)", "", "MUHASEBEKOD, BIRIMKOD, ", eko2Uz, eko2Grup);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            string oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), saymanlikStr, birim, "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[2] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), true, "SUM(BORC-ALACAK)", "H.", "M.MUHASEBEKOD, M.BIRIMKOD, ", eko2Uz, eko2Grup);
            baglantilar[2] = "BaglantiSatiriBUM";
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            int[] ortakAlanlar;
            if (!eko2Grup)
                ortakAlanlar = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            else
                ortakAlanlar = new int[] { 0, 1, 2, 3, 4, 5 };

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, ortakAlanlar);
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.saymanlik = alAnahtar.IntAl(0);
                hk.birim = alAnahtar.StrAl(1);
                hk.tertipTam = "";
                string peb = "";
                if (!eko2Grup)
                    peb = "-" + alAnahtar.StrAl(6);
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(2) + "-" + alAnahtar.StrAl(3) + "-" + alAnahtar.StrAl(4) + "-" + alAnahtar.StrAl(5) + peb);
                hk.planlanan = alVeri.DblAl(0);
                hk.talepEdilen = alVeri.DblAl(1);
                hk.harcama = alVeri.DblAl(2);
                if ((hk.planlanan != 0 || hk.talepEdilen != 0 || hk.harcama != 0))
                {
                    hk.ekoad = EkoAdBul(kullanan, servis, htEko, tertip.yil, alAnahtar.StrAl(5));
                    if (!eko2Grup)
                        hk.pebad = PebAdBul(kullanan, servis, htPeb, tertip.yil, alAnahtar.StrAl(6));
                    oa.Ekle(hk);
                }
            }

            if (!eko2Grup)
            {
                sorgular = new string[2];
                baglantilar = new string[2];

                sorgular[0] = TertipKosulOlBBH(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), "MUHASEBEKOD, BIRIMKOD, ");
                baglantilar[0] = "BaglantiSatiriBHMv2";

                sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), false, "SUM(BORC-ALACAK)", "H.", "M.MUHASEBEKOD, M.BIRIMKOD, ", eko2Uz);
                baglantilar[1] = "BaglantiSatiriBUM";

                ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6 });
                foreach (DictionaryEntry de in ht)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                    hk.saymanlik = alAnahtar.IntAl(0);
                    hk.birim = alAnahtar.StrAl(1);
                    string eko4 = alAnahtar.StrAl(5);
                    string peb = "";
                    peb = alAnahtar.StrAl(6);
                    string eko2 = eko4.Substring(0, eko2Uz);
                    string on = alAnahtar.StrAl(2) + "-" + alAnahtar.StrAl(3) + "-" + alAnahtar.StrAl(4);
                    hk.tertipEko2 = Noktala(t, on + "-" + eko2 + "-" + peb);
                    hk.tertipTam = Noktala(t, on + "-" + eko4 + "-" + peb);
                    hk.planlanan = 0;
                    hk.talepEdilen = alVeri.DblAl(0);
                    hk.aciklama = alVeri.StrAl(1);
                    hk.harcama = alVeri.DblAl(2);
                    if ((hk.planlanan != 0 || hk.talepEdilen != 0 || hk.harcama != 0))
                    {
                        hk.ekoad = EkoAdBul(kullanan, servis, htEko, tertip.yil, eko4);
                        hk.pebad = PebAdBul(kullanan, servis, htPeb, tertip.yil, peb);
                        oa.Ekle(hk);
                    }
                }

                oa.objeler.Sort(new BirimButceHazirlikKarsilanan(true));
            }
            return oa;
        }

        /// <summary>
        /// BirimButceHazirlik listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int saymanlik, string birim, string senaryo, bool birimGrupla, bool eko2)
        {
            if (!HasReadAccess(kullanan, saymanlik, birim))
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(ButceKodResI.BirimButceHazirlik001);
                return oa;
            }

            if (birimGrupla)
                return ListeleGrup(veriErisim, kullanan, tertip, saymanlik, birim, senaryo, eko2);
            else
                return ListeleTekBirim(veriErisim, kullanan, tertip, saymanlik, birim, senaryo);

        }

        internal static ObjectArray ListeleBirimButceKarsilamaGroupByIl(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli)
        {
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            string[] sorgular = null;
            string[] baglantilar = null;
            ObjectArray oa = new ObjectArray();
            sorgular = new string[5];
            baglantilar = new string[5];
            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[0] = IlKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), "SAYMANLIKKOD");
            baglantilar[0] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[1] = IlKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), "MUHASEBEKOD");
            baglantilar[1] = "BaglantiSatiriBHMv2";

            string karsilananKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "01.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[2] = IlKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", karsilananKosul, false, ""), "MUHASEBEKOD");
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] = "SELECT CONVERT(INT, PLAKAKOD), AD FROM ORGCOGRAFYA WHERE LEN(KOD) = 7 ORDER BY 1";
            baglantilar[3] = "BaglantiSatiriBUM";

            string oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), "", "", "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[4] = IlKosulOlHRC(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), "M.MUHASEBEKOD", "SUM(BORC-ALACAK)");
            baglantilar[4] = "BaglantiSatiriBUM";

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.il = alAnahtar.IntAl(0);
                hk.planlanan = alVeri.DblAl(0);
                hk.talepEdilen = alVeri.DblAl(1);
                hk.karsilanan = alVeri.DblAl(2);
                hk.ad = alVeri.StrAl(3);
                hk.harcama = alVeri.DblAl(4);
                if (hk.il == 0) hk.ad = "Merkez";
                if (!(sifirlarGizli && hk.harcama == 0 && hk.planlanan == 0 && hk.talepEdilen == 0 && hk.karsilanan == 0))
                    oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleBirimButceKarsilamaEko2Eko4Karisik(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli)
        {
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            string[] sorgular = null;
            string[] baglantilar = null;
            ObjectArray oa = new ObjectArray();

            Tertip t = new Tertip(tertip.yil, tertip.tabloKod);
            int eko2Uz = t.KodUzunlukBul(3, 2, true);

            string kurKod = "";
            if (saymanlik < 1000 && birim == "000")
            {
                kurKod = tertip.kodlar[0];
                if (string.IsNullOrWhiteSpace(kurKod))
                    kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            }
            else
                kurKod = TNS.MUH.Arac.Tanimla().SaymanlikBirimKurkodBul(saymanlik, birim);
            tertip.kodlar[0] = kurKod;

            sorgular = new string[5];
            baglantilar = new string[5];
            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", saymanlik.ToString(), birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[0] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[0] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", saymanlik.ToString(), birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            string karsilananKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "01.00", saymanlik.ToString(), birim, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[2] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", karsilananKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] += $"SELECT DISTINCT BHMTERTIPLISTESIGIDER.KURKOD, BHMTERTIPLISTESIGIDER.FONKOD, BHMTERTIPLISTESIGIDER.FINKOD, SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) AS Expr1, BHMTERTIPLISTESIGIDER.PEBKOD, ";
            sorgular[3] += "ButceHazirlik.dbo.ABSEKO.EKOAD ";
            sorgular[3] += "FROM BHMTERTIPLISTESIGIDER INNER JOIN ";
            sorgular[3] += $"ButceHazirlik.dbo.ABSEKO ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sorgular[3] += "ButceHazirlik.dbo.ABSPEBKOD ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSPEBKOD.YIL AND BHMTERTIPLISTESIGIDER.PEBKOD = ButceHazirlik.dbo.ABSPEBKOD.PEBKOD ";
            sorgular[3] += "WHERE (BHMTERTIPLISTESIGIDER.YIL = " + tertip.yil.ToString() + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.KURKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FONKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FINKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.EKOKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.PEBKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "") + "%") + ") ";
            sorgular[3] += "order by 1,2,3,4,5";
            baglantilar[3] = "BaglantiSatiriBHMv2";

            string oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), saymanlik.ToString(), birim, "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[4] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), true, "SUM(BORC-ALACAK)", "H.", "", eko2Uz);
            baglantilar[4] = "BaglantiSatiriBUM";

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.il = il;
                hk.saymanlik = saymanlik;
                hk.birim = birim;
                hk.tertipTam = "";
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + alAnahtar.StrAl(3) + "-" + alAnahtar.StrAl(4));
                hk.planlanan = alVeri.DblAl(0);
                hk.talepEdilen = alVeri.DblAl(1);
                hk.karsilanan = alVeri.DblAl(2);
                hk.ekoad = alVeri.StrAl(3);
                hk.harcama = alVeri.DblAl(4);
                oa.Ekle(hk);
            }

            sorgular = new string[4];
            baglantilar = new string[4];

            sorgular[0] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), false, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", karsilananKosul, false, ""), false, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] += "SELECT DISTINCT BHMTERTIPLISTESIGIDER.KURKOD, BHMTERTIPLISTESIGIDER.FONKOD, BHMTERTIPLISTESIGIDER.FINKOD,  BHMTERTIPLISTESIGIDER.EKOKOD, BHMTERTIPLISTESIGIDER.PEBKOD, ";
            sorgular[2] += "ButceHazirlik.dbo.ABSEKO.EKOAD ";
            sorgular[2] += "FROM BHMTERTIPLISTESIGIDER INNER JOIN ";
            sorgular[2] += "ButceHazirlik.dbo.ABSEKO ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND BHMTERTIPLISTESIGIDER.EKOKOD = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sorgular[2] += "ButceHazirlik.dbo.ABSPEBKOD ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSPEBKOD.YIL AND BHMTERTIPLISTESIGIDER.PEBKOD = ButceHazirlik.dbo.ABSPEBKOD.PEBKOD ";
            sorgular[2] += "WHERE (BHMTERTIPLISTESIGIDER.YIL = " + tertip.yil.ToString() + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.KURKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.FONKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.FINKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.EKOKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "") + "%") + ") ";
            sorgular[2] += "AND (BHMTERTIPLISTESIGIDER.PEBKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "") + "%") + ") ";
            sorgular[2] += "order by 1,2,3,4,5";
            baglantilar[2] = "BaglantiSatiriBHMv2";

            oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), saymanlik.ToString(), birim, "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[3] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), false, "SUM(BORC-ALACAK)", "H.", "", eko2Uz);
            baglantilar[3] = "BaglantiSatiriBUM";

            ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.il = il;
                hk.saymanlik = saymanlik;
                hk.birim = birim;

                string eko4 = alAnahtar.StrAl(3);
                string eko2 = eko4.Substring(0, eko2Uz);
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + eko2 + "-" + alAnahtar.StrAl(4));
                hk.tertipTam = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + eko4 + "-" + alAnahtar.StrAl(4));
                hk.planlanan = 0;
                hk.talepEdilen = alVeri.DblAl(0);
                hk.karsilanan = alVeri.DblAl(1);
                hk.ekoad = alVeri.StrAl(2);
                hk.harcama = alVeri.DblAl(3);
                oa.Ekle(hk);
            }

            oa.objeler.Sort(new BirimButceHazirlikKarsilanan());
            return oa;
        }

        internal static ObjectArray ListeleBirimButceKarsilamaEko2(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip)
        {
            int eko2Uz = tertip.KodUzunlukBul(3, 2, true);

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            string[] sorgular = null;
            string[] baglantilar = null;
            ObjectArray oa = new ObjectArray();
            if (string.IsNullOrWhiteSpace(tertip.kodlar[0])) tertip.kodlar[0] = TNS.DEG.Arac.VarsayilanKurumBul();
            Hashtable htEko = new Hashtable();
            Hashtable htPeb = new Hashtable();

            Tertip t = new Tertip(tertip.yil, tertip.tabloKod);

            sorgular = new string[4];
            baglantilar = new string[4];
            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[0] = TertipKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[0] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[1] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[1] = "BaglantiSatiriBHMv2";

            string karsilananKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "01.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[2] = TertipKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", karsilananKosul, false, ""), true, "SUM(ODENEK)", "", "", eko2Uz);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            sorgular[3] += $"SELECT DISTINCT BHMTERTIPLISTESIGIDER.KURKOD, BHMTERTIPLISTESIGIDER.FONKOD, BHMTERTIPLISTESIGIDER.FINKOD, SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) AS Expr1, BHMTERTIPLISTESIGIDER.PEBKOD, ";
            sorgular[3] += "ButceHazirlik.dbo.ABSEKO.EKOAD, ButceHazirlik.dbo.ABSPEBKOD.PEBAD ";
            sorgular[3] += "FROM BHMTERTIPLISTESIGIDER INNER JOIN ";
            sorgular[3] += $"ButceHazirlik.dbo.ABSEKO ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND SUBSTRING(BHMTERTIPLISTESIGIDER.EKOKOD, 1, {eko2Uz}) = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sorgular[3] += "ButceHazirlik.dbo.ABSPEBKOD ON BHMTERTIPLISTESIGIDER.YIL = ButceHazirlik.dbo.ABSPEBKOD.YIL AND BHMTERTIPLISTESIGIDER.PEBKOD = ButceHazirlik.dbo.ABSPEBKOD.PEBKOD ";
            sorgular[3] += "WHERE (BHMTERTIPLISTESIGIDER.YIL = " + tertip.yil.ToString() + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.KURKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FONKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.FINKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.EKOKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "") + "%") + ") ";
            sorgular[3] += "AND (BHMTERTIPLISTESIGIDER.PEBKOD LIKE " + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "") + "%") + ") ";
            sorgular[3] += "order by 1,2,3,4,5";
            baglantilar[3] = "BaglantiSatiriBHMv2";

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.tertipTam = "";
                hk.tertipEko2 = Noktala(t, alAnahtar.StrAl(0) + "-" + alAnahtar.StrAl(1) + "-" + alAnahtar.StrAl(2) + "-" + alAnahtar.StrAl(3) + "-" + alAnahtar.StrAl(4));
                hk.planlanan = alVeri.DblAl(0);
                hk.talepEdilen = alVeri.DblAl(1);
                hk.karsilanan = alVeri.DblAl(2);
                hk.ekoad = alVeri.StrAl(3);
                hk.pebad = alVeri.StrAl(4);
                if (string.IsNullOrWhiteSpace(hk.ekoad)) hk.ekoad = EkoAdBul(kullanan, servis, htEko, tertip.yil, alAnahtar.StrAl(3));
                if (string.IsNullOrWhiteSpace(hk.pebad)) hk.pebad = PebAdBul(kullanan, servis, htPeb, tertip.yil, alAnahtar.StrAl(4));
                oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleBirimButceKarsilamaGroupBySaymanlikBirim(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli)
        {
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            string[] sorgular = null;
            string[] baglantilar = null;
            ObjectArray oa = new ObjectArray();

            sorgular = new string[5];
            baglantilar = new string[5];

            string oncekiHRCKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 2).ToString(), "", "", "830", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], "", tertip.kodlar[4]);
            sorgular[0] = SaymanlikKosulOlHRC(servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", oncekiHRCKosul, false, ""), il);
            baglantilar[0] = "BaglantiSatiriBUM";


            string oncekiMPKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil - 1).ToString(), "5", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], "", "", "5", "0");
            sorgular[1] = SaymanlikKosulOl(servis.UzayKosulStr(kullanan, "MPGMASRAFPLANIBIRIM", oncekiMPKosul, false, ""), "SAYMANLIKKOD", il);
            baglantilar[1] = "BaglantiSatiriBUM";

            string talepKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "00.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[2] = SaymanlikKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", talepKosul, false, ""), "MUHASEBEKOD", il);
            baglantilar[2] = "BaglantiSatiriBHMv2";

            string karsilananKosul = TNS.UZY.Arac.UzayKosulOl((tertip.yil).ToString(), "01.00", "", "", tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
            sorgular[3] = SaymanlikKosulOl(servis.UzayKosulStr(kullanan, "BHMBIRIMBUTCEHAZIRLIK", karsilananKosul, false, ""), "MUHASEBEKOD", il);
            baglantilar[3] = "BaglantiSatiriBHMv2";

            sorgular[4] = "SELECT MUHMUHASEBE.MUHASEBEKOD, MUHMUHASEBEBIRIM.BIRIMKOD, MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD FROM MUHMUHASEBE INNER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBE.MUHASEBEKOD = MUHMUHASEBEBIRIM.MUHASEBEKOD where MUHMUHASEBE.MUHASEBEKOD/1000=" + il.ToString() + " AND BIRIMKOD<>'000' order by 1,2";
            baglantilar[4] = "BaglantiSatiriBUM";

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                BirimButceHazirlikKarsilanan hk = new BirimButceHazirlikKarsilanan();
                hk.il = il;
                hk.saymanlik = alAnahtar.IntAl(0);
                hk.birim = alAnahtar.StrAl(1);
                hk.harcama = alVeri.DblAl(0);
                hk.planlanan = alVeri.DblAl(1);
                hk.talepEdilen = alVeri.DblAl(2);
                hk.karsilanan = alVeri.DblAl(3);
                hk.ad = TNS.BHMv2.Arac.BirimAdOl(alVeri.StrAl(4), alVeri.StrAl(5));
                if (!(sifirlarGizli && hk.planlanan == 0 && hk.talepEdilen == 0 && hk.karsilanan == 0))
                    oa.Ekle(hk);
            }
            return oa;
        }

        internal static ObjectArray ListeleBirimButceKarsilama(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli, int tur)
        {
            if (!YetkiServis.OGYYetkisiVarMi(kullanan, "", tertip, ENUMOGY.Gorme))
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(String.Format("Verdiğiniz tertip kıstasını ({0}) görme yetkiniz bulunmamaktadır. Lüften yetkiniz olan kodları seçiniz!", tertip.TertipStrOl()));
                return oa;
            }
            if (tur == 1)
                return ListeleBirimButceKarsilamaGroupByIl(veriErisim, kullanan, tertip, il, saymanlik, birim, sifirlarGizli);
            else if (tur == 2)
                return ListeleBirimButceKarsilamaGroupBySaymanlikBirim(veriErisim, kullanan, tertip, il, saymanlik, birim, sifirlarGizli);
            else if (tur == 3)
                return ListeleBirimButceKarsilamaEko2Eko4Karisik(veriErisim, kullanan, tertip, il, saymanlik, birim, sifirlarGizli);
            else if (tur == 4)
                return ListeleBirimButceKarsilamaEko2(veriErisim, kullanan, tertip);
            else
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(String.Format("Desteklenmeyen tür kodu geldi!", tertip.TertipStrOl()));
                return oa;
            }
        }

        private static string Noktala(Tertip t, string tKod)
        {
            t.TertipAta(tKod);
            t.Noktala();
            return t.TertipStrOl();
        }

        private static string TertipKosulOl(string uzayKosul, bool eko2, string sumAlan, string onAd, string onAlan, int eko2uz, bool eko2grup = false)
        {
            string eko2alan;
            string peb = "";
            if (!eko2grup)
            {
                peb = ", " + onAd + "PEBKOD";
            }
            if (eko2)
                eko2alan = "SUBSTRING(" + onAd + $"EKOKOD, 1, {eko2uz})";
            else
                eko2alan = onAd + "EKOKOD";
            string alanlar = onAlan + onAd + "KURKOD, " + onAd + "FONKOD, " + onAd + "FINKOD, " + eko2alan + peb;
            string arka = FromdanSonrasi(uzayKosul);
            return "SELECT " + alanlar + ", " + sumAlan + " FROM " + arka + " GROUP BY " + alanlar + " ORDER BY " + alanlar;
        }

        //private static string TertipKosulOl(string uzayKosul, bool eko2, int eko2Uz)
        //{
        //    string eko2alan;
        //    string onAd = "M.";
        //    string sumAlan = "SUM(BORC-ALACAK)";
        //    if (eko2)
        //        eko2alan = $"SUBSTRING(" + onAd + "EKOKOD, 1, {eko2Uz})";
        //    else
        //        eko2alan = onAd + "EKOKOD";
        //    string alanlar = onAd + "KURKOD, " + onAd + "FONKOD, " + onAd + "FINKOD, " + eko2alan + ", " + onAd + "PEBKOD";
        //    string arka = FromdanSonrasi(uzayKosul);
        //    return "SELECT " + alanlar + ", " + sumAlan + " FROM " + arka + " GROUP BY " + alanlar + " ORDER BY " + alanlar;
        //}

        private static string TertipKosulOlBBH(string uzayKosul, string onAlan)
        {
            string alanlar = onAlan + "KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD";
            string arka = FromdanSonrasi(uzayKosul);
            return "SELECT " + alanlar + ", ODENEK, ACIKLAMA" + " FROM " + arka + " ORDER BY " + alanlar;
        }

        private static string IlKosulOl(string uzayKosul, string alanAd)
        {
            string arka = FromdanSonrasi(uzayKosul);
            return "SELECT " + alanAd + "/1000, SUM(ODENEK) FROM " + arka + " GROUP BY " + alanAd + "/1000 ORDER BY 1";
        }

        private static string IlKosulOlHRC(string uzayKosul, string alanAd, string sumAlanAd)
        {
            string arka = FromdanSonrasi(uzayKosul);
            return "SELECT " + alanAd + "/1000, " + sumAlanAd + " FROM " + arka + " GROUP BY " + alanAd + "/1000 ORDER BY 1";
        }

        private static string SaymanlikKosulOlHRC(string uzayKosul, int il)
        {
            string alanAd = "M.MUHASEBEKOD"; //, "SUM(BORC-ALACAK)", 
            string arka = FromdanSonrasi(uzayKosul);
            string ilKodAlan = alanAd + "/1000";
            return "SELECT " + alanAd + ", M.BIRIMKOD, SUM(BORC-ALACAK) FROM " + arka + " AND " + ilKodAlan + "=" + il.ToString() + " GROUP BY " + alanAd + ", M.BIRIMKOD ORDER BY 1, 2";
        }

        private static string SaymanlikKosulOl(string uzayKosul, string alanAd, int il)
        {
            string arka = FromdanSonrasi(uzayKosul);
            string ilKodAlan = alanAd + "/1000";
            return "SELECT " + alanAd + ", BIRIMKOD, SUM(ODENEK) FROM " + arka + " AND " + ilKodAlan + "=" + il.ToString() + " GROUP BY " + alanAd + ", BIRIMKOD ORDER BY 1, 2";
        }

        private static string FromdanSonrasi(string uzayKosul)
        {
            string on = "";
            string arka = "";
            OrtakFonksiyonlar.StringiIkiyeBol(uzayKosul, " FROM ", ref on, ref arka);
            return arka;
        }

        internal static Sonuc BirimButceDetayOranla(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, double karsilanan, int dagitimYontem)
        {
            Sonuc s = new Sonuc();
            if (!YetkiServis.OGYYetkisiVarMi(kullanan, "", tertip, ENUMOGY.Yazma))
            {
                s.DegerAta(String.Format("Verdiğiniz tertip kıstasını ({0}) değiştirme yetkiniz bulunmamaktadır. Lüften yetkiniz olan kodları seçiniz!", tertip.TertipStrOl()));
            }
            //int onayKod = tab.OnaykodBul(1, tertip.yil, "01.00", saymanlik, birim);
            //if (onayKod == 5)
            //{
            //    ve.VeriErisimKapat();
            //    s.DegerAta("Biriminiz bütçe teklif verileri onaylı olduğundan değişiklik yapamazsınız. Üst birimizle iletişime geçerek onay kaldırılabilir.");
            //}
            if (!s.islemSonuc)
                return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tab = new TABBKDBIRIMBUTCEHAZIRLIK(ve);
            double toplam = 0;
            if (dagitimYontem == 1)
                toplam = tab.HarcamaToplamBul(tertip, il, saymanlik, birim);
            else if (dagitimYontem == 2)
                toplam = tab.TalepToplamBul(tertip, il, saymanlik, birim, "00.00");
            if (toplam == 0)
            {
                s.DegerAta("Kaynak veri bulunamadığından dağıtım işlemi yapılamadı!");
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            s = tab.TertipleriSil(tertip, "01.00", il, saymanlik, birim);

            if (s.islemSonuc)
                s = tab.TertipleriKopyala(tertip, il, saymanlik, birim, dagitimYontem, karsilanan, toplam);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();

            return s;
        }

        internal static Sonuc OnayKodAtaTasra(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, ENUMTasraHazirlikOnayDurum onayDurum)
        {
            Sonuc s = new Sonuc(true, "", "");
            ENUMBirimTur bt = TNS.BHMv2.Arac.BirimTurBul(saymanlik, birim);
            if (bt == ENUMBirimTur.Bilinmiyor)
            {
                s.DegerAta("Bilinmeyen birim tür, onay işlemi iptal edildi!");
                return s;
            }

            bool hakkiVar;

            if (onayDurum == ENUMTasraHazirlikOnayDurum.IlceOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.IlOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.KurumOnayla)
            {
                hakkiVar = HasWriteAccess(kullanan, saymanlik, birim);
            }
            else if (onayDurum == ENUMTasraHazirlikOnayDurum.IlceOnayKaldir)
            {
                hakkiVar = (birim == "250" && IlKullanicisiMi(kullanan, saymanlik / 1000));
            }
            else if (onayDurum == ENUMTasraHazirlikOnayDurum.IlOnayKaldir || onayDurum == ENUMTasraHazirlikOnayDurum.KurumOnayKaldir)
            {
                hakkiVar = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
            }
            else
                hakkiVar = false;

            if (!hakkiVar)
            {
                s.DegerAta(string.Format("Bu {0} yetkiniz yoktur!", onayDurum == ENUMTasraHazirlikOnayDurum.IlceOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.IlOnayla ? "birimi onaylama" : "birimin onayını kaldırma"));
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tabTasHaz = new TABBKDBIRIMBUTCEHAZIRLIK(ve);
            int onayKod;
            if (onayDurum == ENUMTasraHazirlikOnayDurum.IlceOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.IlOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.KurumOnayla)
            {
                onayKod = 5;
                if (bt == ENUMBirimTur.Tasra_Il)
                {
                    TNSCollection coll = OnaysizBirimListele(tabTasHaz, onayTur, asama, yil, saymanlik / 1000, "250");
                    if (coll == null)
                    {
                        s.DegerAta(ve.hata);
                        ve.VeriErisimKapat();
                        return s;
                    }
                    if (coll.Count != 0)
                    {
                        s.DegerAta("Onaysız alt birimler olduğundan onaylama işlemi yapılamaz!" + StringeCevir(coll));
                        ve.VeriErisimKapat();
                        return s;
                    }
                }
            }
            else
            {
                if (onayDurum == ENUMTasraHazirlikOnayDurum.KurumOnayKaldir)
                    onayKod = 1;
                else
                    onayKod = 0;
                if (bt == ENUMBirimTur.Tasra_Ilce)
                {
                    TNSCollection coll = OnayliBirimListele(tabTasHaz, onayTur, asama, yil, saymanlik / 1000, "260");
                    if (coll == null)
                    {
                        s.DegerAta(ve.hata);
                        ve.VeriErisimKapat();
                        return s;
                    }
                    if (coll.Count != 0)
                    {
                        s.DegerAta("Üst birim onaylı olduğundan onay kaldırma işlemi yapılamaz!" + StringeCevir(coll));
                        ve.VeriErisimKapat();
                        return s;
                    }
                }
            }
            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabTasHaz.OnaykodSil(onayTur, yil, asama, saymanlik, birim);

            if (s.islemSonuc)
                s = tabTasHaz.OnaykodAta(onayTur, yil, asama, saymanlik, birim, onayKod);

            if (s.islemSonuc)
                s = tabTasHaz.OnaykodTarihceYaz(onayTur, yil, asama, saymanlik, birim, onayKod, kullanan.kullaniciKodu);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OnayKodAtaToplu(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, int onayKod)
        {
            Sonuc s = new Sonuc();
            bool hakkiVar = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
            if (!hakkiVar && saymanlik > 0 && !string.IsNullOrWhiteSpace(birim))
            {
                hakkiVar = HasWriteAccess(kullanan, saymanlik, birim);
            }
            if (!hakkiVar)
            {
                s.DegerAta(string.Format("Bu işleme yapmaya yetkiniz bulunmamaktadır!"));
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBKDBIRIMBUTCEHAZIRLIK tabTasHaz = new TABBKDBIRIMBUTCEHAZIRLIK(ve);

            MUH.MuhasebeBirim muhasebeBirim = new MUH.MuhasebeBirim();
            muhasebeBirim.muhasebe = saymanlik;
            muhasebeBirim.birimKod = birim;
            ObjectArray muhBirimler = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(muhasebeBirim);

            ve.BeginTransaction();
            foreach (MUH.MuhasebeBirim item in muhBirimler.objeler)
            {
                if (!s.islemSonuc) break;
                if (s.islemSonuc)
                    s = tabTasHaz.OnaykodSil(onayTur, yil, asama, item.muhasebe, item.birimKod);

                if (s.islemSonuc)
                    s = tabTasHaz.OnaykodAta(onayTur, yil, asama, item.muhasebe, item.birimKod, onayKod);
            }

            if (s.islemSonuc)
                s = tabTasHaz.OnaykodTarihceYaz(onayTur, yil, asama, saymanlik, birim, onayKod, kullanan.kullaniciKodu);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }


        private static string StringeCevir(TNSCollection coll)
        {
            string retVal = "";
            if (coll != null)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    if (retVal != "") retVal += ", ";
                    retVal += coll[i].ToString();
                }
            }
            return retVal;
        }

        private static TNSCollection OnayliBirimListele(TABBKDBIRIMBUTCEHAZIRLIK tabTasHaz, ENUMBirimOnayTur onayTur, string asama, int yil, int ilKod, string birimKod)
        {
            IDataReader dr = tabTasHaz.OnayliBirimListele(onayTur, asama, yil, ilKod, "260");
            if (dr == null)
            {
                return null;
            }
            TNSCollection coll = new TNSCollection();
            while (dr.Read())
            {
                TNS.MUH.MuhasebeBirim mb = new MUH.MuhasebeBirim();
                mb.muhasebe = VeriErisim.DegerAl(dr, 0, 0);
                mb.birimKod = VeriErisim.DegerAl(dr, 1, "");
                mb.birimAd = VeriErisim.DegerAl(dr, 3, "");
                mb.muhasebeAd = VeriErisim.DegerAl(dr, 4, "");
                coll.Add(mb);
            }
            dr.Close();
            return coll;
        }

        private static TNSCollection OnaysizBirimListele(TABBKDBIRIMBUTCEHAZIRLIK tabTasHaz, ENUMBirimOnayTur onayTur, string asama, int yil, int ilKod, string birimKod)
        {
            IDataReader dr = tabTasHaz.OnaysizBirimListele(onayTur, asama, yil, ilKod, "250");
            if (dr == null)
            {
                return null;
            }
            TNSCollection coll = new TNSCollection();
            while (dr.Read())
            {
                TNS.MUH.MuhasebeBirim mb = new MUH.MuhasebeBirim();
                mb.muhasebe = VeriErisim.DegerAl(dr, 0, 0);
                mb.birimKod = VeriErisim.DegerAl(dr, 1, "");
                mb.birimAd = VeriErisim.DegerAl(dr, 3, "");
                mb.muhasebeAd = VeriErisim.DegerAl(dr, 4, "");
                coll.Add(mb);
            }
            dr.Close();
            return coll;
        }

        internal static ObjectArray BirimButceHazirlikOnayDurum(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod)
        {
            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            string[] sorgular = null;
            string[] baglantilar = null;
            ObjectArray oa = new ObjectArray();
            sorgular = new string[3];
            baglantilar = new string[3];
            sorgular[0] = "SELECT MUHMUHASEBE.MUHASEBEKOD, MUHMUHASEBEBIRIM.BIRIMKOD, MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD FROM MUHMUHASEBEBIRIM INNER JOIN MUHMUHASEBE ON MUHMUHASEBEBIRIM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD WHERE BIRIMKOD<>'000' AND BIRIMKOD <> '251' AND 1 NOT IN (SELECT 1 FROM MUHMUHASEBEBIRIMKAPALI IC WHERE IC.BIRIMKOD=MUHMUHASEBEBIRIM.BIRIMKOD AND IC.MUHASEBEKOD=MUHMUHASEBEBIRIM.MUHASEBEKOD AND IC.KURKOD=MUHMUHASEBEBIRIM.KURKOD) order by 1,2";
            baglantilar[0] = "BaglantiSatiriBUM";

            sorgular[1] = "SELECT MUHASEBEKOD, BIRIMKOD, ONAYKOD FROM BHMBIRIMONAY WHERE YIL=" + VeriErisim.DegerYap(yil) + " AND SENARYOKOD=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND ONAYTUR=1 AND BIRIMKOD<>'000' AND BIRIMKOD <> '251' order by 1,2";
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] = "SELECT MUHASEBEKOD, BIRIMKOD, SUM(ODENEK) FROM BHMBIRIMBUTCEHAZIRLIK where YIL=" + VeriErisim.DegerYap(yil) + " AND SENARYOKOD=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND BIRIMKOD<>'000' AND BIRIMKOD <> '251' group by MUHASEBEKOD, BIRIMKOD order by 1,2";
            baglantilar[2] = "BaglantiSatiriBHMv2";
            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                IslemTarihce it = new IslemTarihce();

                it.MUHASEBEKOD = alAnahtar.IntAl(0);
                it.BIRIMKOD = alAnahtar.StrAl(1);
                it.MUHASEBEAD = alVeri.StrAl(0);
                it.BIRIMAD = alVeri.StrAl(1);
                it.ONAYKOD = alVeri.IntAl(2);
                it.GIRILEN = alVeri.DblAl(3);
                oa.Ekle(it);
            }
            return oa;

            throw new NotImplementedException();
        }
    }
}
