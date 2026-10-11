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
    /// HRCPSLGeciciGorevUcak formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCPSLGeciciGorevUcak
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            Sonuc s = new Sonuc(true, "", ""); ;
            string hata = "";
            if (form.surecYil <= 2000)
                hata += "Süreç yılını giriniz!<br>";
            if (string.IsNullOrEmpty(form.surecNo))
                hata += "Süreç numarasını giriniz!<br>";
            if (string.IsNullOrEmpty(form.firma.vergiNo))
                hata += "Firma vergi numarasını giriniz!<br>";
            if (form.faturalar == null || form.faturalar.Length == 0)
                hata += "En az bir fatura giriniz!<br>";

            for (int i = 0; i < form.faturalar.Length; i++)
            {
                PSLGeciciGorevUcakFatura fat = form.faturalar[i];
                if (fat.tarih == null || fat.tarih.isNull)
                    hata += string.Format("{0}. satırda fatura tarihi boş!<br>", i + 1);
                if (string.IsNullOrEmpty(fat.faturaNo))
                    hata += string.Format("{0}. satırda fatura numarası boş!<br>", i + 1);
                if (fat.tutar <= 0)
                    hata += string.Format("{0}. satırda tutar girilmemiş!<br>", i + 1);
            }

            if (hata != "")
                s.DegerAta(hata);
            return s;
        }


        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma yetkiniz yok!");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCPSLGECICIGOREVUCAK tabHRCPSLGECICIGOREVUCAK = new TABHRCPSLGECICIGOREVUCAK(ve);
            TABHRCPSLGECICIGOREVUCAKFATURA tabHRCPSLGECICIGOREVUCAKFATURA = new TABHRCPSLGECICIGOREVUCAKFATURA(ve);

            for (int i = 0; i < form.faturalar.Length && s.islemSonuc; i++)
            {
                PSLGeciciGorevUcakFatura anaFat = form.faturalar[i];
                int eskiSurecYil;
                string eskiSurecNo;
                if (tabHRCPSLGECICIGOREVUCAKFATURA.FaturaVarMi(form.surecYil, form.surecNo, form.firma.vergiNo, anaFat.faturaNo, out eskiSurecYil, out eskiSurecNo))
                {
                    s.DegerAta($"{anaFat.faturaNo} fatura numarası {eskiSurecYil}-{eskiSurecNo} sürecinde de kullanıldığından, bu süreçte kullanamazsınız!");
                }
                else
                    for (int j = i + 1; j < form.faturalar.Length && s.islemSonuc; j++)
                    {
                        PSLGeciciGorevUcakFatura simFat = form.faturalar[j];
                        if (simFat.faturaNo == anaFat.faturaNo)
                        {
                            s.DegerAta($"{simFat.faturaNo} fatura numarasını iki kez kullanmışsınız, lütfen bir kez giriniz!");
                        }
                    }
            }

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCPSLGECICIGOREVUCAKFATURA.Sil(form);
            if (s.islemSonuc)
                s = tabHRCPSLGECICIGOREVUCAK.Sil(form);

            if (s.islemSonuc)
                s = tabHRCPSLGECICIGOREVUCAK.Ekle(form);

            for (int i = 0; i < form.faturalar.Length && s.islemSonuc; i++)
            {
                s = tabHRCPSLGECICIGOREVUCAKFATURA.Ekle(form, i);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        public static PSLGeciciGorevUcak Oku(VeriErisim veriErisim, Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            PSLGeciciGorevUcak[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// PSLGeciciGorevUcak listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        public static PSLGeciciGorevUcak[] Listele(VeriErisim veriErisim, Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCPSLGECICIGOREVUCAK tabHRCPSLGECICIGOREVUCAK = new TABHRCPSLGECICIGOREVUCAK(ve);
            TABHRCPSLGECICIGOREVUCAKFATURA tabHRCPSLGECICIGOREVUCAKFATURA = new TABHRCPSLGECICIGOREVUCAKFATURA(ve);

            ArrayList alAna;
            IDataReader dr;
            dr = tabHRCPSLGECICIGOREVUCAK.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            alAna = new ArrayList();
            while (dr.Read())
            {
                PSLGeciciGorevUcak detay = new PSLGeciciGorevUcak();
                detay.surecYil = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAK.ENUMAlanlar.SURECYIL, 0);
                detay.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAK.ENUMAlanlar.SURECNO, "");
                detay.firma.vergiNo = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAK.ENUMAlanlar.TCKIMLIKVEYAVERGINO, "");
                detay.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAK.ENUMAlanlar.ODEMEEMRINO, "");
                detay.odemeEmriYil = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAK.ENUMAlanlar.ODEMEEMRIYIL, 0);
                alAna.Add(detay);
            }
            dr.Close();
            ArrayList al;
            foreach (PSLGeciciGorevUcak formIc in alAna)
            {
                FirmaBilgisi fbkriter = new FirmaBilgisi();
                fbkriter.muhasebe = form.firma.muhasebe;
                fbkriter.vergiNo = formIc.firma.vergiNo;
                ObjectArray oa = new HRCFirmaBilgisi().Listele(kullanan, ve, fbkriter);
                if (!oa.sonuc.islemSonuc || oa.ObjeSayisi != 1)
                {
                    ve.VeriErisimKapat();
                    string hata;
                    if (!oa.sonuc.islemSonuc) hata = oa.sonuc.hataStr;
                    else if (oa.ObjeSayisi == 0) hata = formIc.firma.vergiNo + " vergi numaralı firma bulunamadı!";
                    else hata = formIc.firma.vergiNo + " vergi numaralı birden fazla firma bulundu!";
                    throw new Exception(hata);
                }
                formIc.firma = (FirmaBilgisi)oa.objeler[0];
                dr = tabHRCPSLGECICIGOREVUCAKFATURA.Oku(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                al = new ArrayList();
                while (dr.Read())
                {
                    PSLGeciciGorevUcakFatura detay = new PSLGeciciGorevUcakFatura();
                    detay.tarih = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAKFATURA.ENUMAlanlar.TARIH);
                    detay.faturaNo = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAKFATURA.ENUMAlanlar.FATURANO, "");
                    detay.tutar = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAKFATURA.ENUMAlanlar.TUTAR, 0.0);
                    detay.komisyonMu = VeriErisim.DegerAl(dr, (int)TABHRCPSLGECICIGOREVUCAKFATURA.ENUMAlanlar.KOMISYONMU, 0);
                    al.Add(detay);
                }
                formIc.faturalar = (PSLGeciciGorevUcakFatura[])al.ToArray(typeof(PSLGeciciGorevUcakFatura));
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (PSLGeciciGorevUcak[])alAna.ToArray(typeof(PSLGeciciGorevUcak));
        }
    }
}
