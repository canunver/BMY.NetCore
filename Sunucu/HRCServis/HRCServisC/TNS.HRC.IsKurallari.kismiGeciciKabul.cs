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
    /// HRCkismiGeciciKabul formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCkismiGeciciKabul
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, KismiGeciciKabul form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, KismiGeciciKabul form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KismiGeciciKabul form)
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
            TeklifKazananFirma kazananFirma = new HRCTeklif().KazananGetir(ve, kullanan, form.surecYil, form.surecNo, form.malzemeSiraNo);
            if (kazananFirma == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Kazananı belli olmadığından devam edemezsiniz!");
            }
            TABUAKISMIGECICIKABUL tabUAKISMIGECICIKABUL = new TABUAKISMIGECICIKABUL(ve);
            TABUAKISMIGECICIIMZA tabUAKISMIGECICIIMZA = new TABUAKISMIGECICIIMZA(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabUAKISMIGECICIIMZA.Sil(form);
            if (s.islemSonuc)
                s = tabUAKISMIGECICIKABUL.Sil(form);

            if (s.islemSonuc)
                s = tabUAKISMIGECICIKABUL.Ekle(form);

            for (int i = 0; i < form.kismiGeciciKabulImza.Length && s.islemSonuc; i++)
            {
                s = tabUAKISMIGECICIIMZA.Ekle(form, i);

            }

            s = new HRCUAStf().KaydetTeklif(kullanan, form.surecYil, form.surecNo, kazananFirma.firmaNo, 4, form.teklifListe.objeler);

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
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, KismiGeciciKabul form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, KismiGeciciKabul form)
        {
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        public static KismiGeciciKabul Oku(VeriErisim veriErisim, Kullanici kullanan, KismiGeciciKabul form)
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
            TeklifKazananFirma kazananFirma = new HRCTeklif().KazananGetir(ve, kullanan, form.surecYil, form.surecNo, form.malzemeSiraNo);
            if (kazananFirma == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Kazananı belli olmadığından devam edemezsiniz!");
            }

            TABUAKISMIGECICIKABUL tabUAKISMIGECICIKABUL = new TABUAKISMIGECICIKABUL(ve);
            TABUAKISMIGECICIIMZA tabUAKISMIGECICIIMZA = new TABUAKISMIGECICIIMZA(ve);

            KismiGeciciKabul formIc = new KismiGeciciKabul();
            IDataReader dr;
            dr = tabUAKISMIGECICIKABUL.Oku(form);
            if (dr != null)
            {
                if (dr.Read())
                {
                    formIc.surecYil = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIKABUL.ENUMAlanlar.SURECYIL, 0);
                    formIc.surecNo = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIKABUL.ENUMAlanlar.SURECNO, "");
                    formIc.malzemeSiraNo = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIKABUL.ENUMAlanlar.MALSIRANO, 0);
                    formIc.yaziTarihi = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIKABUL.ENUMAlanlar.YAZITARIH);
                    formIc.aciklama = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIKABUL.ENUMAlanlar.ACIKLAMA, "");
                }
                dr.Close();

                ArrayList al;
                dr = tabUAKISMIGECICIIMZA.Oku(form);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                al = new ArrayList();
                while (dr.Read())
                {
                    KismiGeciciKabulImza detay = new KismiGeciciKabulImza();
                    detay.TcKimlikNo = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIIMZA.ENUMAlanlar.TCKIMLIKNO, "");
                    detay.Ad = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIIMZA.ENUMAlanlar.AD, "");
                    detay.Soyad = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIIMZA.ENUMAlanlar.SOYAD, "");
                    detay.Unvan = VeriErisim.DegerAl(dr, (int)TABUAKISMIGECICIIMZA.ENUMAlanlar.UNVAN, "");
                    al.Add(detay);
                }
                formIc.kismiGeciciKabulImza = (KismiGeciciKabulImza[])al.ToArray(typeof(KismiGeciciKabulImza));
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ve.VeriErisimKapat();

            formIc.teklifListe = new HRCUAStf().ListeleTeklif(kullanan, form.surecYil, form.surecNo, kazananFirma.firmaNo, 3, form.malzemeSiraNo);
            ObjectArray oa4 = new HRCUAStf().ListeleTeklif(kullanan, form.surecYil, form.surecNo, kazananFirma.firmaNo, 4, form.malzemeSiraNo);
            foreach (SatinAlmaTalepIhtiyacTeklif baslik in oa4.objeler)
            {
                SatinAlmaTalepIhtiyacTeklif bulunan = formIc.TeklifBul(baslik);
                if (bulunan != null)
                {
                    bulunan.GerceklesenMiktar1 = baslik.miktar1;
                    bulunan.GerceklesenMiktar2 = baslik.miktar2;
                    bulunan.harcamaBirimKod = baslik.harcamaBirimKod;
                }
            }
            return formIc;
        }

    }
}
