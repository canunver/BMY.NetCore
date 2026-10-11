using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCBordroRaporSablon formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCBordroRaporSablon
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// BordroRaporSablon nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroRaporSablon bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BordroRaporSablon form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDRORAPORSABLON to = new TABHRCBORDRORAPORSABLON(ve);
            ve.BeginTransaction();

            if (form.kod == 0)
            {
                //Kod alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "BORDRORAPORSABLON";

                form.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                //**********************************************************
            }
            else
                s = to.Sil(new BordroRaporSablon() { kod = form.kod });

            if (s.islemSonuc)
                s = to.Kaydet(form);

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.kod + "";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return s;
        }

        /// <summary>
        /// BordroRaporSablon listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroRaporSablon yapısında bilgi tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, BordroRaporSablon form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDRORAPORSABLON tab = new TABHRCBORDRORAPORSABLON(ve);

            IDataReader dr = tab.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                BordroRaporSablon detay = new BordroRaporSablon();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABHRCBORDRORAPORSABLON.ENUMAlanlar.KOD, 0);
                detay.kullaniciKod = VeriErisim.DegerAl(dr, (int)TABHRCBORDRORAPORSABLON.ENUMAlanlar.KULLANICIKODU, "");
                detay.ad = VeriErisim.DegerAl(dr, (int)TABHRCBORDRORAPORSABLON.ENUMAlanlar.AD, "");
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABHRCBORDRORAPORSABLON.ENUMAlanlar.ACIKLAMA, "");
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return oa;
        }

        internal static ObjectArray PersonelMaliyetHesapla(VeriErisim veriErisim, Kullanici kullanan, string tcKimlikNo, TNSDateTime basTarih, TNSDateTime bitTarih, int ekParam)
        {
            ObjectArray donusDeger = new ObjectArray();
            TNSCollection toplamIslemler = new TNSCollection();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TNS.HRC.MerkezBankasiKurSinif kriterKB = new TNS.HRC.MerkezBankasiKurSinif();

            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.tcList = new TNSCollection();
            p.tcList.Add(tcKimlikNo);
            p.aktifTarihSonHareket = false;
            p.aktifTarihTur = 10;
            //p.kadro.muhasebeKodu = muhasebe;
            //p.kadro.muhasebeBirimi = BirimBul(birim);
            p.bordroDisiDahil = false;
            p.ogrenimListesiOlsun = true;
            p.lojmanListesiOlsun = true;
            p.kadrolarOlsun = true;
            p.maasParamlarOlsun = true;
            p.ailelerOlsun = true;
            p.dillerOlsun = true;
            p.hizmetSuresiOlsun = true;

            CalisanDinamikBordroDonem bordroDonem = new CalisanDinamikBordroDonem(0, "");
            DateTime dt = new DateTime(basTarih.Yil, basTarih.Ay, 1);
            bordroDonem.bordroDonemBasTarih = new TNSDateTime(dt);
            bordroDonem.hazirlamaTarih = new TNSDateTime(dt);
            TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

            kriterKB.kurTarih = bordroDonem.hazirlamaTarih;
            TNS.HRC.MerkezBankasiKurSinif gunlukKur = HRCMerkezBankKur.Oku(null, kullanan, kriterKB);
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();

            int perTip = -1;
            TNS.Calculator c = null;
            BordroYapi by = null;
            bool isGunu = true;
            string hata = "";
            p.aktifTarih = new TNSDateTime();
            p.matrahTarih = new TNSDateTime();
            IKY.Personel okunanPer = null;
            for (int i = 0; bordroDonem.bordroDonemBasTarih.Oku() < bitTarih.Oku(); i++)
            {
                p.aktifTarih.Yaz(bordroDonem.bordroDonemBasTarih);
                p.aktifTarihSon = bordroDonem.sonGun;
                p.matrahTarih.Yaz(bordroDonem.bordroDonemBasTarih);
                p.aktifTarihSon.AddDays(1);
                p.izinDonemBas = new TNSDateTime(bordroDonem.bordroDonemBasTarih.Oku());
                if (TNS.DEG.Arac.VarsayilanKurumBul() != "13.99")
                    p.izinDonemBas.AddDays(-90);
                else
                    p.izinDonemBas.AddMonths(-12 * 50);
                p.izinDonemSon = bordroDonem.sonGun;
                p.izinDonemSon.AddDays(1);
                p.gorevYili = p.aktifTarih.Yil;
                DateTime dtGiderBasla = new DateTime(bordroDonem.hazirlamaTarih.Yil, bordroDonem.hazirlamaTarih.Ay, 1);
                dtGiderBasla = dtGiderBasla.AddMonths(-1);
                DateTime dtGiderBitis = dtGiderBasla.AddMonths(1);
                p.giderBasla = new TNSDateTime(dtGiderBasla);
                p.giderBitis = new TNSDateTime(dtGiderBitis);
                ObjectArray perler = servisIKY.PersonelListele(kullanan, p, true, false);
                if (perler.sonuc.islemSonuc && perler.ObjeSayisi == 1)
                {
                    okunanPer = ((TNS.IKY.Personel)perler[0]);
                    if (c == null || perTip != okunanPer.personelTip)
                    {
                        perTip = okunanPer.personelTip;
                        by = BordroYapiBul(ve, kullanan, okunanPer.personelTip, 1);
                        if (by.hepsi == null)
                        {
                            donusDeger.sonuc.DegerAta($"{perTip} kodlu personel tipi için, bordro hesaplama yöntemi bulunamadı!");
                            ve.VeriErisimKapat();
                            return donusDeger;
                        }
                        else
                        {
                            isGunu = false;
                            hata = "";
                            c = HRCServisI.PSLDinBordroHesaplayici.BordroDerleyicisiOl(by, out hata, HRCServisI.PSLDinBordroHesaplayici.OrtakFonksiyonlarOku(kullanan));
                            if (!string.IsNullOrEmpty(hata))
                            {
                                donusDeger.sonuc.DegerAta(hata);
                                ve.VeriErisimKapat();
                                return donusDeger;
                            }
                        }
                    }

                    ArrayList satirlar = new ArrayList();
                    int[] personelTipDiziInt = null;

                    HRCServisI.PSLDinBordroHesaplayici.KisiEkle(okunanPer, null, satirlar, isGunu, bordroDonem, by, "", personelTipDiziInt, ref hata);
                    CalisanDinamikBordro cb = new CalisanDinamikBordro();
                    cb.kisiDegerler = satirlar;
                    int ind = by.YerBul("dersSaati", 0);
                    if (ind > -1)
                        ((KisiSatir)cb.kisiDegerler[0]).degerler[ind + 1] = ekParam;
                    HRCServisI.PSLDinBordroHesaplayici.BordroVerileriHesaplaGenelCompiler(c, kullanan, servisHRC, by, bordroDonem, cb, "", isGunu);
                    ObjectArray oa = new ObjectArray();
                    BordroYapiIslem.MuhasebeHesaplariEkle(kullanan, muhServis, gunlukKur, oa, cb, by, 0, bordroDonem, 2020, "", 3, 0, donusDeger.sonuc);
                    if (!donusDeger.sonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return donusDeger;
                    }
                    HesaplariEkle(toplamIslemler, i, oa, "830.01", "800.01");
                }
                else if (!perler.sonuc.islemSonuc)
                {
                    //donusDeger.sonuc.DegerAta(perler.sonuc.hataStr);
                    //ve.VeriErisimKapat();
                    //return donusDeger;
                }
                bordroDonem.bordroDonemBasTarih.AddMonths(1);
            }
            ve.VeriErisimKapat();
            if (okunanPer == null)
            {
                donusDeger.sonuc.DegerAta($"{tcKimlikNo} kodlu personele ait bordro hesabı yapılamadı!");
            }
            else
            {
                donusDeger.Ekle(okunanPer);
                donusDeger.Ekle(toplamIslemler);
            }
            return donusDeger;
        }

        private static void Debug(TNSCollection hesaplar)
        {
            foreach (BordroMuhasebe item in hesaplar)
            {
                OrtakFonksiyonlar.HataDosyaYaz("Taah.txt", $"{item.hesapKod}::{item.altHesapKod}::{item.birimKod}::{item.toplam.borc}", false);
            }
        }

        private static void HataDosyaYaz(string mesaj, bool yeni)
        {
            //OrtakFonksiyonlar.HataDosyaYaz("BordroTah.txt", mesaj, yeni);
        }

        private static void HesaplariEkle(TNSCollection toplamIslemler, int ayIndis, ObjectArray oa, params string[] hesapKodlar)
        {
            TNSCollection sonuc = new TNSCollection();
            foreach (BordroMuhasebe item in oa.objeler)
            {
                if (HesapVarMi(hesapKodlar, item.hesapKod))
                {
                    BordroMuhasebe bulunan = HesapBulYoksaEkle(toplamIslemler, item);
                    bulunan.dizi[ayIndis].borc += item.dizi[0].borc;
                    bulunan.dizi[ayIndis].borcDoviz += item.dizi[0].borcDoviz;
                }
            }
            oa.objeler = sonuc;
        }

        private static bool HesapVarMi(string[] hesapKodlar, string hesapKod)
        {
            foreach (string item in hesapKodlar)
            {
                if (hesapKod.StartsWith(item)) return true;
            }
            return false;
        }

        private static BordroMuhasebe HesapBulYoksaEkle(TNSCollection toplamIslemler, BordroMuhasebe gelenHesap)
        {
            foreach (BordroMuhasebe item in toplamIslemler)
            {
                if (item.hesapKod == gelenHesap.hesapKod)
                    return item;
            }
            BordroMuhasebe yeni = new BordroMuhasebe();
            yeni.hesapSatir = gelenHesap.hesapSatir;
            yeni.birimKod = gelenHesap.birimKod;
            toplamIslemler.Add(yeni);
            return yeni;
        }

        private static BordroYapi BordroYapiBul(VeriErisim ve, Kullanici kullanan, int personelTip, int bordroTur)
        {
            BordroYapi by = BordroYapiIslem.BordroYapiOku(ve, kullanan, personelTip, bordroTur, true, null, null, true);

            return by;
        }

        internal static ObjectArray BordroKisiBorcuBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int personelTip)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.BordroKisiBorcuBilgiListele(personelTip);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int kol = 0;
                    oa.Ekle(new BordroBilgi(
                        VeriErisim.DegerAl(dr, kol++, 0),
                        VeriErisim.DegerAl(dr, kol++, 0),
                        VeriErisim.DegerAl(dr, kol++, ""),
                        VeriErisim.DegerAl(dr, kol++, ""),
                        (BordroBilgi.KesintiTur)VeriErisim.DegerAl(dr, kol++, 0),
                        VeriErisim.DegerAl(dr, kol++, 0) == 1
                        ));
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
