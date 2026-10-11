using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Collections.Generic;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMProgramOnayForm formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ProgramOnayForm
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, ProgramOnay form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProgramOnay form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(ButceHazirlikResI.ProgramOnayForm001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBHMPROGRAMONAY tabBHMPROGRAMONAY = new TABBHMPROGRAMONAY(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMPROGRAMONAY.Sil(form);
            if (s.islemSonuc)
                s = tabBHMPROGRAMONAY.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        public static ProgramOnay Oku(VeriErisim veriErisim, Kullanici kullanan, ProgramOnay form)
        {
            ProgramOnay[] s = Listele(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, ProgramOnay form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, ProgramOnay form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// ProgramOnayAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        public static ProgramOnay[] Listele(VeriErisim veriErisim, Kullanici kullanan, ProgramOnay form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception(ButceHazirlikResI.ProgramOnayForm002);
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                throw new Exception(s.hataStr);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABBHMPROGRAMONAY tabOnay = new TABBHMPROGRAMONAY(ve);

            IDataReader dr = tabOnay.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ArrayList alAna = new ArrayList();

            while (dr.Read())
            {
                ProgramOnay detay = new ProgramOnay();
                detay.programKodu = VeriErisim.DegerAl(dr, 0, "");
                detay.programAdi = VeriErisim.DegerAl(dr, 1, "");
                detay.durum = VeriErisim.DegerAl(dr, 2, 0);
                alAna.Add(detay);
            }
            ve.VeriErisimKapat();
            return (ProgramOnay[])alAna.ToArray(typeof(ProgramOnay));
        }

        public static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string programKod)
        {
            Sonuc s = new Sonuc();
            ButceTasnifOdenekleriFormu tasnifOdenekleriFormu = new ButceTasnifOdenekleriFormu();
            tasnifOdenekleriFormu.yil = yil;
            tasnifOdenekleriFormu.tasnif = new ButceTasnif();
            tasnifOdenekleriFormu.tasnif.siraNo = 15;
            EkDegerParam[] ekDegerParamlar = null;
            if (yil < 2000) s.DegerAta("Lütfen yıl bilgisi giriniz!");
            if (string.IsNullOrWhiteSpace(senaryoKod)) s.DegerAta("Lütfen senaryo seçiniz!");
            if (string.IsNullOrWhiteSpace(programKod)) s.DegerAta("Lütfen faaliyet seçiniz!");
            else if (programKod.Length != 2) s.DegerAta("Lütfen faaliyet kodunun birinci kırılımını seçiniz!");

            if (s.islemSonuc)
            {
                try
                {
                    ButceTasnifOdenekleriFormu f = ButceTasnifOdenekGiris.Oku(veriErisim, kullanan, tasnifOdenekleriFormu, ekDegerParamlar, senaryoKod, "", "30----" + programKod);

                    if (f.butceTasnifSatirOdenekleri == null || f.butceTasnifSatirOdenekleri.Length == 0) s.DegerAta("Verilen bilgilere uyan bilgi bulunamadığından onaylama yapılamaz!");
                    else
                    {
                        string hata = "";
                        for (int i = 0; i < f.butceTasnifSatirOdenekleri.Length; i++)
                        {
                            ButceTasnifSatirOdenegi so = f.butceTasnifSatirOdenekleri[i];
                            if (so.ekDegerler[0] != so.ekDegerler[1])
                            {
                                hata += string.Format("{0} ödeneklerinde tavan {1} ile detay toplamı {2} eşit değil!<br>", so.hiyerarsi.ad, so.ekDegerler[0], so.ekDegerler[1]);
                            }
                        }
                        s.DegerAta(hata);
                    }
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
            }
            if (!s.islemSonuc)
                return s;

            ProgramOnay form = new ProgramOnay();
            form.butceYili = yil;
            form.senaryoKod = senaryoKod;
            form.programKodu = programKod;
            form.durum = 5;
            return Kaydet(veriErisim, kullanan, form);
        }

        internal static OdenekSenaryoDurum[] ButceOdenekDurumListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            List<OdenekSenaryoDurum> l = new List<OdenekSenaryoDurum>();
            try
            {
                TABBHMODENEKONAY tabOnay = new TABBHMODENEKONAY(ve);

                ButceSenaryo[] Listele = BHMv2ButceSenaryo.Listele(null, kullanan, null);
                for (int i = 0; i < Listele.Length; i++)
                {
                    OdenekSenaryoDurum osd = new OdenekSenaryoDurum();
                    osd.senaryo = Listele[i];
                    osd.onayDurum = tabOnay.OnayKod(yil, kurumsalKod, osd.senaryoKod);
                    l.Add(osd);
                }
            }
            catch { }
            ve.VeriErisimKapat();
            return l.ToArray();
        }

        internal static Sonuc ButceOdenekDurumKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string birim, OdenekSenaryoDurum[] onaylar)
        {
            if (onaylar == null || onaylar.Length == 0) return new Sonuc(false, "Kaydedilmek istenen liste boş geldi!", "");
            if (!YazmaYetkisiVarMi(kullanan)) return new Sonuc(false, "Bu işlemi yapmaya yetkiniz bulunmamaktadır!", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMODENEKONAY tabOnay = new TABBHMODENEKONAY(ve);
            ve.BeginTransaction();

            Sonuc s = tabOnay.OnayKodSil(yil, birim, null);
            for (int i = 0; i < onaylar.Length && s.islemSonuc; i++)
            {
                s = tabOnay.OnayKodYaz(yil, birim, onaylar[i].senaryoKod, onaylar[i].onayDurum);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return s;
        }

        internal static string OnayliAsamaBul(VeriErisim veriErisim, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), null);
            TABBHMODENEKONAY tabOnay = new TABBHMODENEKONAY(ve);
            string senaryo = tabOnay.SenaryoBul(yil, TNS.DEG.Arac.VarsayilanKurumBul(), 3);
            ve.VeriErisimKapat();
            if (!string.IsNullOrEmpty(senaryo)) senaryo = OrtakFonksiyonlar.KodNoktaKoy("99.99", senaryo);
            return senaryo;
        }
    }
}
