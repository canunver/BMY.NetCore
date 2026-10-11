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
    /// HRCDogrudanTeminPlan formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCDogrudanTeminPlan
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";
        static TNS.MUH.IMUHServis _servisMuh = null;
        static TNS.MUH.IMUHServis servisMuh { get { if (_servisMuh == null) _servisMuh = TNS.MUH.Arac.Tanimla(); return _servisMuh; } }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="saymanlik">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int saymanlik, string birim)
        {
            bool yetkiVar = servisMuh.MuhasebeBirimYetkiVarMi(kullanan, saymanlik.ToString(), birim, MUH.ENUMOGY.Gorme);
            if (yetkiVar) return yetkiVar;
            //if (birim == "250")
            //{
            //    return IlKullanicisiMi(kullanan, saymanlik / 1000);
            //}
            return yetkiVar;
        }

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int saymanlik, string birim)
        {
            return servisMuh.MuhasebeBirimYetkiVarMi(kullanan, saymanlik.ToString(), birim, MUH.ENUMOGY.Yazma);
        }

        /// <summary>
        /// DogrudanTeminPlan nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int saymanlik, string birim, string mhy, DogrudanTeminPlan[] formlar)
        {
            Sonuc s = new Sonuc(true, "", "");
            //if (formlar == null || formlar.Length == 0)
            //{
            //    s.DegerAta("Lütfen veri girdikten sonra kaydet tuşuna basınız!");
            //    return s;
            //}

            if (saymanlik <= 0)
            {
                s.DegerAta("Lütfen saymanlık seçiniz!");
                return s;
            }

            if (string.IsNullOrWhiteSpace(birim))
            {
                s.DegerAta("Lütfen birim seçiniz!");
                return s;
            }

            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                int durum = bhmv2Servis.OnayKodOkuTasra(kullanan, yil, 0, "000", senaryoKod, BHMv2.ENUMBirimOnayTur.KIKYUZDE10);
                if (durum >= 4)
                {
                    s.DegerAta("KİK doğrudan temin planı girilen yıl için veri girişine kapalı olduğundan değişiklik yapamazsınız!");
                    return s;
                }
                if (durum != 1)
                {
                    s.DegerAta("KİK doğrudan temin planı girilen yıl için veri girişine açılmadığından değişiklik yapamazsınız!");
                    return s;
                }
            }
            IdareBilgi ib = new IdareBilgi();
            ib.muhasebe = saymanlik;
            ib.birim = birim;

            ib = new HRCIdareBilgi().Listele(veriErisim, kullanan, ib);
            double sinir = 0;
            string yerKod = "";
            if (ib.buyuksehirMi == 1)
            {
                yerKod = "Büyükşehir";
                sinir = OrtakFonksiyonlar.ConvertToDbl(TNS.DEG.Arac.DegiskenDegerBul(yil, "/BuyukSehirSinir"));
            }
            else
            {
                yerKod = "Küçükşehir";
                sinir = OrtakFonksiyonlar.ConvertToDbl(TNS.DEG.Arac.DegiskenDegerBul(yil, "/KucukSehirSinir"));
            }
            if (ib.argeMi == 1)
                sinir *= 5;
            double pazSinir = OrtakFonksiyonlar.ConvertToDbl(TNS.DEG.Arac.DegiskenDegerBul(yil, "/PazarlikUstSinir"));

            for (int i = 0; i < formlar.Length; i++)
            {
                DogrudanTeminPlan form = formlar[i];
                if (saymanlik != form.saymanlik || birim != form.birim)
                    s.DegerAta("Kaydedilecek veriler hepsi aynı birime ait olmalıdır!");

                if (string.IsNullOrWhiteSpace(form.tasHesapKod))
                    s.DegerAta(string.Format("{0}. satırda OKAS kodunu giriniz!", i + 1));
                else
                {
                    char ilkChar = form.tasHesapKod[0];
                    if (!(ilkChar == 'M' || ilkChar == 'H' || ilkChar == 'Y'))
                        s.DegerAta(string.Format("{0}. satırda OKAS kodunu seçiniz!", i + 1));
                }
                if (string.IsNullOrWhiteSpace(form.tasHesapAd))
                    s.DegerAta(string.Format("{0}. satırda alınacak mal adını giriniz!", i + 1));
                if (form.dtHaric < 0)
                    s.DegerAta(string.Format("{0}. satırda doğrudan temin hariç tutar negatif olamaz!", i + 1));

                if (form.dtHaric == 0)
                {
                    if (form.miktar <= 0)
                        s.DegerAta(string.Format("{0}. satırda alınacak malın miktarını giriniz!", i + 1));

                    if (form.birimFiyat <= 0)
                        s.DegerAta(string.Format("{0}. satırda alınacak malın birim fiyatını giriniz!", i + 1));

                    if (string.IsNullOrWhiteSpace(form.birimAd))
                        s.DegerAta(string.Format("{0}. satırda alınacak malın birimini giriniz!", i + 1));
                }
                else if (form.dtHaric > 0)
                {
                    if (form.miktar < 0)
                        s.DegerAta(string.Format("{0}. satırda alınacak malın miktarını negatif olamaz!", i + 1));

                    if (form.birimFiyat < 0)
                        s.DegerAta(string.Format("{0}. satırda alınacak malın birim fiyatı negatif olamaz!", i + 1));
                }
                if (string.IsNullOrWhiteSpace(form.harcamaTuru))
                    s.DegerAta(string.Format("{0}. satırda harcama türünü giriniz!", i + 1));

                if (string.IsNullOrWhiteSpace(form.kanunMaddesi))
                    s.DegerAta(string.Format("{0}. satırda kanun maddesini giriniz!", i + 1));

                double satToplam = form.miktar * form.birimFiyat;
                if (form.kanunMaddesi == "22d")
                {
                    if (satToplam > sinir)
                        s.DegerAta(string.Format("{0}. satırda girilen miktar {1} sınırı olan {2} liranın üzerindedir!", i + 1, yerKod, sinir.ToString("#,##0")));
                }
                else
                {
                    if (satToplam > pazSinir)
                        s.DegerAta(string.Format("{0}. satırda girilen miktar {1} sınırı olan {2} liranın üzerindedir!", i + 1, "Pazarlık", pazSinir.ToString("#,##0")));
                }
                for (int j = i + 1; j < formlar.Length; j++)
                {
                    DogrudanTeminPlan form2 = formlar[j];
                    if (form2.tasHesapKod.Replace(".", "") == form.tasHesapKod.Replace(".", ""))
                    {
                        s.DegerAta(string.Format("{0}. satır ile {1}. satırın OKAS kodları aynı!", i + 1, j + 1));
                        break;
                    }
                }

                if (!s.islemSonuc)
                    return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, saymanlik, birim))
            {
                s.DegerAta(HarcamaSurecleriResI.DogrudanTeminPlan001);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCDOGRUDANTEMINPLAN tabHRCDOGRUDANTEMINPLAN = new TABHRCDOGRUDANTEMINPLAN(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCDOGRUDANTEMINPLAN.Sil(yil, senaryoKod, saymanlik, birim, mhy);

            for (int i = 0; i < formlar.Length && s.islemSonuc; i++)
            {
                s = tabHRCDOGRUDANTEMINPLAN.Ekle(formlar[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, DogrudanTeminPlan form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// DogrudanTeminPlan listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DogrudanTeminPlan form)
        {
            ObjectArray alAna = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, form.saymanlik, form.birim))
            {
                alAna.sonuc.DegerAta(HarcamaSurecleriResI.DogrudanTeminPlan002);
                return alAna;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                alAna.sonuc.DegerAta(s.hataStr);
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCDOGRUDANTEMINPLAN tabHRCDOGRUDANTEMINPLAN = new TABHRCDOGRUDANTEMINPLAN(ve);

            IDataReader dr = tabHRCDOGRUDANTEMINPLAN.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }
            while (dr.Read())
            {
                DogrudanTeminPlan detay = new DogrudanTeminPlan();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.YIL, 0);
                detay.saymanlik = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.SAYMANLIK, 0);
                detay.birim = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.BIRIM, "");
                detay.tasHesapKod = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.TASHESAPKOD, "");
                detay.miktar = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.MIKTAR, 0.0);
                detay.birimFiyat = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.BIRIMFIYAT, 0.0);
                detay.kanunMaddesi = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.KANUNMADDESI, "");
                detay.harcamaTuru = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.HARCAMATURU, "");

                detay.tasHesapAd = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.TASHESAPAD, "");
                detay.birimAd = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.BIRIMAD, "");
                detay.dtHaric = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.DTHARIC, 0.0);
                detay.onceAlim = VeriErisim.DegerAl(dr, (int)TABHRCDOGRUDANTEMINPLAN.ENUMAlanlar.ONCEALIM, 0.0);

                int kolonBas = 13;
                detay.muhasebeAd = VeriErisim.DegerAl(dr, kolonBas++, "");
                detay.muhasebeBirimAd = VeriErisim.DegerAl(dr, kolonBas++, "");
                detay.okasKod = VeriErisim.DegerAl(dr, kolonBas++, "");
                detay.okasAd = VeriErisim.DegerAl(dr, kolonBas++, "");

                alAna.objeler.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }

        public static ObjectArray IstatistikUret(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray alAna = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, 0, ""))
            {
                alAna.sonuc.DegerAta(HarcamaSurecleriResI.DogrudanTeminPlan002);
                return alAna;
            }

            string[] sorgular = new string[7];
            string[] baglantilar = new string[7];
            senaryoKod = senaryoKod.Replace(".", "");
            string sql = "select SAYMANLIK, BIRIM, SUM(MIKTAR*BIRIMFIYAT) from HRCDOGRUDANTEMINPLAN where YIL={0} and HarcamaTuru='{1}' and KANUNMADDESI='{2}' and SENARYOKOD='{3}' GROUP BY SAYMANLIK, BIRIM";

            sorgular[0] = "select M.MUHASEBEKOD, B.BIRIMKOD, MUHASEBEAD, BIRIMAD from MUHMUHASEBE M INNER JOIN MUHMUHASEBEBIRIM B ON M.MUHASEBEKOD=B.MUHASEBEKOD where BIRIMKOD<>'000' AND 1 NOT IN (SELECT 1 FROM MUHMUHASEBEBIRIMKAPALI K where K.MUHASEBEKOD=B.MUHASEBEKOD AND K.BIRIMKOD=B.BIRIMKOD AND K.KURKOD=B.KURKOD) ORDER BY 1,2";
            sorgular[1] = String.Format(sql, yil.ToString(), "Mal", "21f", senaryoKod);
            sorgular[2] = String.Format(sql, yil.ToString(), "Mal", "22d", senaryoKod);
            sorgular[3] = String.Format(sql, yil.ToString(), "Hizmet", "21f", senaryoKod);
            sorgular[4] = String.Format(sql, yil.ToString(), "Hizmet", "22d", senaryoKod);
            sorgular[5] = String.Format(sql, yil.ToString(), "Yapım", "21f", senaryoKod);
            sorgular[6] = String.Format(sql, yil.ToString(), "Yapım", "22d", senaryoKod);

            baglantilar[0] = "BaglantiSatiriBUM";
            baglantilar[1] = "BaglantiSatiriHRC";
            baglantilar[2] = "BaglantiSatiriHRC";
            baglantilar[3] = "BaglantiSatiriHRC";
            baglantilar[4] = "BaglantiSatiriHRC";
            baglantilar[5] = "BaglantiSatiriHRC";
            baglantilar[6] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1 });

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                object[] oDizi = new object[9];
                oDizi[0] = alAnahtar.IntAl(0);
                oDizi[1] = alAnahtar.StrAl(1);
                oDizi[2] = TNS.MUH.Arac.BirimAdOl(alVeri.StrAl(0), alVeri.StrAl(1));
                oDizi[3] = alVeri.DblAl(2);
                oDizi[4] = alVeri.DblAl(3);
                oDizi[5] = alVeri.DblAl(4);
                oDizi[6] = alVeri.DblAl(5);
                oDizi[7] = alVeri.DblAl(6);
                oDizi[8] = alVeri.DblAl(7);

                alAna.Ekle(oDizi);
            }

            return alAna;
        }
    }
}
