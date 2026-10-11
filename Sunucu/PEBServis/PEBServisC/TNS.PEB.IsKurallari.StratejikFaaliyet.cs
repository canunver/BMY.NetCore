using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
using TNS.PEB.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// PEBStratejikFaaliyet formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class PEBStratejikFaaliyet
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriSTR";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, STRFaaliyet form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, STRFaaliyet form)
        {
            Sonuc s = new Sonuc();
            if (form.dONEMBYIL == 0) s.DegerAta("Stratejik plan dönemini seçiniz!");
            if (form.hEDEFKOD == 0) s.DegerAta("Hedefi seçiniz!");
            if (string.IsNullOrWhiteSpace(form.kURUMKOD)) s.DegerAta("Kurumu seçiniz!");
            if (form.yil == 0) s.DegerAta("Eylem planı yılını seçiniz!");
            if (string.IsNullOrWhiteSpace(form.fAALKODU)) s.DegerAta("Faaliyet kodunu giriniz!");
            if (string.IsNullOrWhiteSpace(form.fAALADI)) s.DegerAta("Faaliyet adını giriniz!");

            if (form.sPALTFAALIYET == null || form.sPALTFAALIYET.Count == 0) s.DegerAta("Alt faaliyet giriniz!");
            else
            {
                foreach (STRAltFaaliyet item in form.sPALTFAALIYET)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrWhiteSpace(item.aLTFAALKODU)) s.DegerAta("Alt faaliyet kodunu giriniz!");
                        if (string.IsNullOrWhiteSpace(item.aLTFAALADI)) s.DegerAta("Alt faaliyet adını giriniz!");
                        if (item.sPALTFAALIYETBIRIM == null || item.sPALTFAALIYETBIRIM.Count == 0) s.DegerAta("En an bir birim giriniz!");
                        else
                        {
                            if (item.sPALTFAALIYETBIRIM[0] == null) s.DegerAta("En an bir birim giriniz!");
                            foreach (TNS.BKD.ABSKod k in item.sPALTFAALIYETBIRIM)
                            {
                                if (k != null)
                                {
                                    if (string.IsNullOrWhiteSpace(k.kod)) s.DegerAta("İlgili sorumlu biri seçiniz!");
                                }
                            }
                        }

                        if (item.baslaTarihDT == DateTime.MinValue) s.DegerAta("Başlangıç tarihini giriniz!");
                        if (item.bitisTarihDT == DateTime.MinValue) s.DegerAta("Bitiş tarihini giriniz!");
                        else if (item.baslaTarihDT != DateTime.MinValue)
                        {
                            if (item.baslaTarihDT > item.bitisTarihDT) s.DegerAta("Bitiş tarihi, başlangıç tarihinden büyük olmalı!");
                        }
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// STRFaaliyet nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">STRFaaliyet yapısında STRFaaliyet: Stratejik Faaliyet bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, STRFaaliyet form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(PerformansButceResI.StratejikFaaliyet001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRFAALIYET tabPEBSPFAALIYET = new TABSTRFAALIYET(ve);
            TABSTRALTFAALIYET tabPEBSPALTFAALIYET = new TABSTRALTFAALIYET(ve);
            TABSTRALTFAALIYETBIRIM tabPEBSPALTFAALIYETBIRIM = new TABSTRALTFAALIYETBIRIM(ve);

            ve.BeginTransaction();

            if (form.fAALNO > 0)
            {
                if (s.islemSonuc)
                    s = tabPEBSPALTFAALIYETBIRIM.Sil(form);
                if (s.islemSonuc)
                    s = tabPEBSPALTFAALIYET.Sil(form);
                if (s.islemSonuc)
                    s = tabPEBSPFAALIYET.Sil(form);
            }
            else
                form.fAALNO = tabPEBSPFAALIYET.FaalNoBul(form);

            if (s.islemSonuc)
                s = tabPEBSPFAALIYET.Ekle(form);

            for (int i = 0; i < form.sPALTFAALIYET.Count && s.islemSonuc; i++)
            {
                STRAltFaaliyet aForm = (STRAltFaaliyet)form.sPALTFAALIYET[i];
                aForm.aLTFAALNO = i + 1;
                s = tabPEBSPALTFAALIYET.Ekle(form, aForm);
                for (int j = 0; j < aForm.sPALTFAALIYETBIRIM.Count && s.islemSonuc; j++)
                {
                    TNS.BKD.ABSKod k = (TNS.BKD.ABSKod)aForm.sPALTFAALIYETBIRIM[j];
                    if (k != null)
                        s = tabPEBSPALTFAALIYETBIRIM.Ekle(form, aForm.aLTFAALNO, k.kod, j);
                }
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.fAALNO.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// STRFaaliyet nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">STRFaaliyet yapısında STRFaaliyet: Stratejik Faaliyet bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, STRFaaliyet form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form, true);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, STRFaaliyet form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, STRFaaliyet form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// STRFaaliyet listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">STRFaaliyet yapısında STRFaaliyet: Stratejik Faaliyet bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, STRFaaliyet form, bool detayli)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                oa.sonuc.DegerAta(PerformansButceResI.StratejikFaaliyet002);
                return oa;
            }

            Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                oa.sonuc.DegerAta(s.hataStr);
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRFAALIYET tabPEBSPFAALIYET = new TABSTRFAALIYET(ve);
            TABSTRALTFAALIYET tabPEBSPALTFAALIYET = new TABSTRALTFAALIYET(ve);
            TABSTRALTFAALIYETBIRIM tabPEBSPALTFAALIYETBIRIM = new TABSTRALTFAALIYETBIRIM(ve);

            IDataReader dr;
            dr = tabPEBSPFAALIYET.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                STRFaaliyet detay = new STRFaaliyet();
                detay.dONEMBYIL = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.DONEMBYIL, 0);
                detay.hEDEFKOD = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.HEDEFKOD, 0);
                detay.kURUMKOD = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.KURUMKOD, "");
                detay.fAALNO = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.FAALNO, 0);
                detay.fAALKODU = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.FAALKODU, "");
                detay.fAALADI = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.FAALADI, "");
                detay.yil = VeriErisim.DegerAl(dr, (int)TABSTRFAALIYET.ENUMAlanlar.YIL, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            if (detayli)
            {
                TNSCollection al;
                foreach (STRFaaliyet formIc in oa.objeler)
                {
                    dr = tabPEBSPALTFAALIYET.Oku(formIc);
                    if (dr == null)
                    {
                        ve.VeriErisimKapat();
                        oa.sonuc.DegerAta(ve.hata);
                        return oa;
                    }
                    al = new TNSCollection();
                    while (dr.Read())
                    {
                        STRAltFaaliyet detay = new STRAltFaaliyet();
                        detay.aLTFAALNO = VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYET.ENUMAlanlar.ALTFAALNO, 0);
                        detay.aLTFAALKODU = VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYET.ENUMAlanlar.ALTFAALKODU, "");
                        detay.aLTFAALADI = VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYET.ENUMAlanlar.ALTFAALADI, "");
                        detay.baslaTarih = VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYET.ENUMAlanlar.BASLATARIH);
                        detay.bitisTarih = VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYET.ENUMAlanlar.BITISTARIH);
                        al.Add(detay);
                    }
                    formIc.sPALTFAALIYET = al;
                    dr.Close();

                    foreach (STRAltFaaliyet item in al)
                    {
                        dr = tabPEBSPALTFAALIYETBIRIM.Oku(formIc, item.aLTFAALNO);
                        if (dr == null)
                        {
                            ve.VeriErisimKapat();
                            oa.sonuc.DegerAta(ve.hata);
                            return oa;
                        }
                        al = new TNSCollection();
                        while (dr.Read())
                        {
                            TNS.BKD.ABSKod detay = new TNS.BKD.ABSKod();
                            
                            detay.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99", VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYETBIRIM.ENUMAlanlar.KURKOD, ""));
                            detay.AdAta(VeriErisim.DegerAl(dr, (int)TABSTRALTFAALIYETBIRIM.ENUMAlanlar.SIRANO + 1, ""));
                            al.Add(detay);
                        }
                        item.sPALTFAALIYETBIRIM = al;
                        dr.Close();
                    }
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }
    }
}
