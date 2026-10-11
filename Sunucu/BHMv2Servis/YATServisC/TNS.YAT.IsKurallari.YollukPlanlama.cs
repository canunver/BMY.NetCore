using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// YATYollukPlanlama formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class YATYollukPlanlama
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriYAT";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, Tertip tertip)
        {
            TNS.BHMv2.IBHMv2Servis s = TNS.BHMv2.Arac.Tanimla();
            return s.OGYYetkisiVarMi(kullanan, null, tertip, BHMv2.ENUMOGY.Yazma);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static void AlanlarGecerliMi(Sonuc s, Kullanici kullanan, YollukPlan form, int satNo)
        {
            string on = satNo.ToString() + ". satırda ";
            if (string.IsNullOrWhiteSpace(form.dairesi))
                s.DegerAta(on + "Lütfen daire giriniz!");
            if (string.IsNullOrWhiteSpace(form.konusu))
                s.DegerAta(on + "Lütfen konu giriniz!");
            if (form.muhtelifMi)
            {
                form.suresi = 0;
                form.kisiSayisi = 0;
                form.yapilacagiAy = "Muhtelif";
                form.yeri = "Muhtelif";
            }
            else
            {
                if (form.suresi <= 0) s.DegerAta(on + "Lütfen süresi alını giriniz!");
                if (form.kisiSayisi <= 0) s.DegerAta(on + "Lütfen kişi sayısını giriniz!");
                if (string.IsNullOrWhiteSpace(form.yapilacagiAy)) s.DegerAta(on + "Lütfen yapılacağı ayı giriniz!");
                if (string.IsNullOrWhiteSpace(form.yeri)) s.DegerAta(on + "Lütfen yer alanını giriniz!");
            }
            if (form.mALIYET <= 0)
                s.DegerAta(on + "Lütfen maliyet giriniz!");
        }

        /// <summary>
        /// YollukPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlan yapısında liste</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, TNSCollection formlar)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (tertip == null || tertip.kodlar == null || tertip.kodlar.Length != 5 || !tertip.TamMi())
            {
                s.DegerAta("Lütfen tertip seçiniz!");
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, tertip))
            {
                s.DegerAta("Yazma Yetkiniz bulunmamaktadır");
                return s;
            }
            int sat = 0;
            double topMal = 0;
            foreach (YollukPlan item in formlar)
            {
                topMal += item.mALIYET;
                AlanlarGecerliMi(s, kullanan, item, ++sat);
            }
            if (!s.islemSonuc)
            {
                return s;
            }
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            topMal = Math.Round(topMal, 2);
            double topOdenek = Math.Round(uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryoKod + "-0-" + tertip.TertipStrOl(), true), 2);
            if (topMal != topOdenek)
            {
                s.DegerAta(tertip.TertipStrOl() + " tertibinin ödeneği " + topOdenek.ToString("#,##0.00") + " iken, girilen maliyet toplamı " + topMal.ToString("#,##0.00") + " olduğundan kaydedemezsiniz. Ödenek kadar planlama yapınız.");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATYOLLUKPLAN tab = new TABYATYOLLUKPLAN(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tab.Sil(yil, senaryoKod, tertip);

            for (int i = 0; i < formlar.Count && s.islemSonuc; i++)
            {
                s = tab.Ekle(yil, senaryoKod, tertip, i + 1, (YollukPlan)formlar[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, Tertip tertip, string pebKodListe)
        {
            TNS.BHMv2.IBHMv2Servis s = TNS.BHMv2.Arac.Tanimla();
            if (tertip == null)
            {
                tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
                tertip.kodlar[0] = "*";
                tertip.kodlar[4] = pebKodListe;
            }
            return s.OGYYetkisiVarMi(kullanan, null, tertip, BHMv2.ENUMOGY.Yazma);
        }

        /// <summary>
        /// YollukPlanAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlanAna yapısında YollukPlanAna: Yıl ve Faaliyet Seçimi bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, string pebKodListe)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, tertip, pebKodListe))
            {
                oa.sonuc.DegerAta("Görme yetkiniz yok!");
                return oa;
            }
            TNS.BKD.Tertip t = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATYOLLUKPLAN tab = new TABYATYOLLUKPLAN(ve);

            IDataReader dr = tab.Listele(yil, senaryoKod, tertip, pebKodListe);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                YollukPlan detay = new YollukPlan();
                detay.sIRANO = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.SIRANO, 0);
                detay.dairesi = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.DAIRESI, "");
                detay.konusu = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.KONUSU, "");
                detay.muhtelifMi = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MUHTELIFMI, 0) == 1;
                detay.suresi = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.SURESI, 0);
                detay.kisiSayisi = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.KISISAYISI, 0);
                detay.yapilacagiAy = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.YAPILACAGIAY, "");
                detay.yeri = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.YERI, "");
                detay.kURKOD = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.KURKOD, ""));
                detay.fONKOD = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.FONKOD, ""));
                detay.fINKOD = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.FINKOD, ""));
                detay.eKOKOD = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.EKOKOD, ""));
                detay.pebKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.PEBKOD, ""));
                detay.mALIYET = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MALIYET, 0.0);

                detay.altProjeAdi = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MALIYET + 1, "");
                detay.yurutenBirim = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MALIYET + 2, "");
                detay.faaliyetAd = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MALIYET + 3, "");
                detay.faaliyetBirimAd = VeriErisim.DegerAl(dr, (int)TABYATYOLLUKPLAN.ENUMAlanlar.MALIYET + 4, "");
                oa.Ekle(detay);
            }
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
