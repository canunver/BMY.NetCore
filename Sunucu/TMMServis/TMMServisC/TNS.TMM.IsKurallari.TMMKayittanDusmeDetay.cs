using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Kayıttan düşme teklif ve onay tutanağının detay bilgileriyle ilgili, iş kuralları
    /// kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMKayittanDusmeDetay
    {
        /// <summary>
        /// Kayıttan düşme teklif ve onay tutanağının detay bilgilerini kaydeden yordam
        /// Kayıttan düşme teklif ve onay tutanağının detay bilgileri ile ilgili bir takım kontroller
        /// yapılmakta, sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kd">Kayıttan düşme teklif ve onay tutanağının detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection kd)
        {
            Sonuc sonuc = new Sonuc();

            if (kd.Count <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM157;
                return sonuc;
            }

            KayittanDusmeDetay d = (KayittanDusmeDetay)kd[0];

            string hata = string.Empty;

            if (d.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(d.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(d.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(d.tutanakNo))
                hata += TasinirMalResI.TMM158 + "<br>";

            hata += Genel.KontrolMuhasebeHarcamaAmbar(d.muhasebeKod, d.harcamaKod, "");

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, d.muhasebeKod, d.harcamaKod, "");
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            int i = 0;
            string kod = string.Empty;

            foreach (KayittanDusmeDetay detay in kd)
            {
                hata = string.Empty;

                i++;

                //Aynı belge sadece tüketim veya demirbaşlar bulunsun
                if (i == 1)
                    kod = detay.hesapPlanKod.Substring(0, 1);
                else
                {
                    if (kod != detay.hesapPlanKod.Substring(0, 1))
                        hata += TasinirMalResI.TMM159 + "<br>";
                }

                if (string.IsNullOrEmpty(detay.hesapPlanKod))
                    hata += string.Format(TasinirMalResI.TMM144, i.ToString()) + "<br>";

                hata += Genel.KontrolHesapPlan(kullanan, detay.hesapPlanKod, i, false, true);
                hata += Genel.KontrolSicilNo(detay.yil, detay.muhasebeKod, detay.harcamaKod, detay.gorSicilNo);

                if (!string.IsNullOrEmpty(detay.gorSicilNo))
                {
                    if (!detay.gorSicilNo.Contains(detay.hesapPlanKod.Replace(".", "")))
                        hata += string.Format(TasinirMalResI.TMM160, i.ToString(), detay.gorSicilNo, detay.hesapPlanKod) + "<br>";

                    if (detay.miktar > 1)
                        hata += string.Format(TasinirMalResI.TMM161, i.ToString(), detay.gorSicilNo) + "<br>";
                }

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr += hata;
                }
            }

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKayittanDusmeDetay tabKD = new TABKayittanDusmeDetay(ve);
            ve.BeginTransaction();

            try
            {
                for (int j = 0; j < kd.Count; j++)
                {
                    if (!String.IsNullOrEmpty(((KayittanDusmeDetay)kd[j]).gorSicilNo))
                    {
                        SicilNoHareket sc = new SicilNoHareket();
                        sc.muhasebeKod = d.muhasebeKod;
                        sc.harcamaBirimKod = d.harcamaKod.Replace(".", "");
                        sc.sicilNo = ((KayittanDusmeDetay)kd[j]).gorSicilNo;
                        sc.fiyat = ((KayittanDusmeDetay)kd[j]).birimFiyat;

                        ObjectArray liste = TMMSicilNoHareket.AmbardakiSicilNolar(ve, kullanan, sc);

                        foreach (SicilNoHareket ssc in liste.objeler)
                            ((KayittanDusmeDetay)kd[j]).prSicilNo = ssc.prSicilNo;

                        //((KayittanDusmeDetay)kd[j]).prSicilNo = (int)TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", d.muhasebeKod + "-" + d.harcamaKod + "-" + ((KayittanDusmeDetay)kd[j]).gorSicilNo, true, "");
                    }
                }

                tabKD.Sil((KayittanDusmeDetay)kd[0]);
                foreach (KayittanDusmeDetay detay in kd)
                {
                    if (detay.miktar > 0)
                    {
                        if (kod == "2" && String.IsNullOrEmpty(detay.gorSicilNo))
                            continue;
                        sonuc = tabKD.Kaydet(detay);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kayıttan düşme teklif ve onay tutanağının detay bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeDetay nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kd">Kayıttan düşme teklif ve onay tutanağının kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kayıttan düşme teklif ve onay tutanağına ait detay bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KayittanDusmeDetay kd)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, kd.muhasebeKod, kd.harcamaKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kd.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKayittanDusmeDetay tabKD = new TABKayittanDusmeDetay(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKD.Listele(kd);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                KayittanDusmeDetay gelen = new KayittanDusmeDetay();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);

                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.tutanakNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                gelen.hesapPlanKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlanKod);
                gelen.hesapPlanAd = Genel.AdGetir(isimler, gelen.hesapPlanKod, "TASHESAPPLAN");

                gelen.prSicilNo = VeriErisim.DegerAl(dr, 5, 0);
                gelen.gorSicilNo = Genel.GorSicilNoGetir(gelen.muhasebeKod, gelen.harcamaKod, gelen.prSicilNo);
                gelen.miktar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                gelen.birimFiyat = VeriErisim.DegerAl(dr, 7, (decimal)0);
                gelen.siraNo = VeriErisim.DegerAl(dr, 8, 0);
                gelen.kdvOran = VeriErisim.DegerAl(dr, 9, 0);
                gelen.tutar = (decimal)gelen.miktar * gelen.birimFiyat * (((decimal)gelen.kdvOran / 100) + 1);
                gelen.olcuBirimKod = VeriErisim.DegerAl(dr, 10, 0);
                gelen.olcuBirimAd = VeriErisim.DegerAl(dr, 11, string.Empty);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}