using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
using TNS.BHF.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// BHFTasitBilgi formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFTasitBilgi
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHF";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int saymanlikKod, string birimKod)
        {
            TNS.MUH.IMUHServis ser = TNS.MUH.Arac.Tanimla();
            return ser.MuhasebeBirimYetkiVarMi(kullanan, saymanlikKod.ToString(), birimKod, MUH.ENUMOGY.Yazma);
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int saymanlikKod, string birimKod)
        {
            TNS.MUH.IMUHServis ser = TNS.MUH.Arac.Tanimla();
            return ser.MuhasebeBirimYetkiVarMi(kullanan, saymanlikKod.ToString(), birimKod, MUH.ENUMOGY.Gorme);
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int saymanlikKod, string birimKod, MUH.ENUMOGY onayKod)
        {
            TNS.MUH.IMUHServis ser = TNS.MUH.Arac.Tanimla();
            return ser.MuhasebeBirimYetkiVarMi(kullanan, saymanlikKod.ToString(), birimKod, onayKod);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, TasitBilgi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.tipKodu <= 0)
                s.DegerAta("Araç türünü seçmelisiniz!");
            int motorsuz = 0;
            if (form.cinsKodu <= 0)
                s.DegerAta("Araç cinsini seçmelisiniz!");
            else
            {
                TasitCins tc = new TasitCins { kod = form.cinsKodu };
                ObjectArray oa = BHFTasitCins.Listele(null, kullanan, tc);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                {
                    tc = (TasitCins)oa.objeler[0];
                    motorsuz = tc.motorsuz;
                }
            }
            if (string.IsNullOrWhiteSpace(form.saseNo))
            {
                if (motorsuz == 1) s.DegerAta("Şase numarası alanına, motorsuz taşıtlarda Taşınır Kayıt sicil numarasını girmelisiniz!");
                else s.DegerAta("Şase numarasını girmelisiniz!");
            }
            if (motorsuz != 1 && string.IsNullOrWhiteSpace(form.motorNo))
                s.DegerAta("Motor numarasını girmelisiniz!");
            if (form.bulunduguSaymanlik <= 0)
                s.DegerAta("Bulunduğu saymanlığı girmelisiniz!");
            if (string.IsNullOrWhiteSpace(form.bulunduguBirim))
                s.DegerAta("Bulunduğu birimi girmelisiniz!");
            if (s.islemSonuc)
            {
                if (form.aitOlduguSaymanlik == 0 && string.IsNullOrWhiteSpace(form.aitOlduguBirim))
                {
                    form.aitOlduguSaymanlik = form.bulunduguSaymanlik;
                    form.aitOlduguBirim = form.bulunduguBirim;
                }
                if (form.aitOlduguSaymanlik <= 0)
                    s.DegerAta("Ait olduğu saymanlığı girmelisiniz!");
                if (string.IsNullOrWhiteSpace(form.aitOlduguBirim))
                    s.DegerAta("Ait olduğu birimi girmelisiniz!");
            }

            if (motorsuz != 1)
            {
                if (string.IsNullOrWhiteSpace(form.plakaNo))
                    s.DegerAta("Plaka numarasını girmelisiniz!");
                else
                {
                    int plakaUz = form.plakaNo.Replace(" ", "").Length;
                    if (plakaUz < 7 || plakaUz > 9)
                        s.DegerAta("Plaka hanesi hatalı girilmiş!");
                }
            }

            if (form.modelKodu <= 0)
                s.DegerAta("Araç modelini seçmelisiniz!");
            if (form.modelYili < 1950)
                s.DegerAta("Araçın model yılı 1950 yılından küçük olamaz!");
            if (form.modelYili > DateTime.Now.Year + 1)
                s.DegerAta("Araçın model yılı " + (DateTime.Now.Year + 1).ToString() + " yılından büyük olamaz!");
            if (form.koltukSayisi < 0)
                s.DegerAta("Koltuk sayısını sıfır yada sıfırdan büyük girmelisiniz!");
            if (motorsuz != 1 && (int)form.yakitTuru < 1)
                s.DegerAta("Yakıt türünü seçiniz!");
            if ((int)form.aitOlduguButce < 1)
                s.DegerAta("Ait olduğu bütçeyi seçiniz!");
            if ((int)form.kullanimDurumu < 1)
                s.DegerAta("Kullanım durumunu seçiniz!");
            //else if (form.kullanimDurumu == ENUMKullanimDurumu.Ayrildi)
            //{
            //    if (form.cikisTarih == null || form.cikisTarih.isNull)
            //        s.DegerAta("Birimden çıkışı yapılan araçlar için çıkış tarihini giriniz!");
            //    if (string.IsNullOrWhiteSpace(form.cikisEvrakSayisi))
            //        s.DegerAta("Birimden çıkışı yapılan araçlar için çıkış yazı numarasını giriniz!");
            //}
            //else
            //{
            //    if (!(form.cikisTarih == null || form.cikisTarih.isNull))
            //        s.DegerAta("Çıkışı yapılmamış araçlar için ayrılış tarihini girmeyiniz!");
            //    if (!string.IsNullOrWhiteSpace(form.cikisEvrakSayisi))
            //        s.DegerAta("Çıkışı yapılmamış araçlar için ayrılış yazı numarasını girmeyiniz!");
            //}
            return s;
        }

        /// <summary>
        /// TasitBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasitBilgi form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form.bulunduguSaymanlik, form.bulunduguBirim))
            {
                s.DegerAta(ButceFormResI.TasitBilgi001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITBILGI tabBHFTASITBILGI = new TABBHFTASITBILGI(ve);

            ObjectVAN ov = Oku(ve, kullanan, form);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            TasitBilgi okunan = (TasitBilgi)ov.o;
            if (okunan != null)
            {
                if (okunan.onayKodu == 5)
                    s.DegerAta("Onaylı formu değiştiremezsiniz!");
                if (okunan.bulunduguBirim != form.bulunduguBirim || okunan.bulunduguSaymanlik != form.bulunduguSaymanlik)
                {
                    if (!YazmaYetkisiVarMi(kullanan, okunan.bulunduguSaymanlik, okunan.bulunduguBirim))
                    {
                        s.DegerAta("Bu aracın daha önce kaydedildiği birime yazma hakkınız olmadığından araç bilgilerini değiştiremezsiniz");
                    }
                }
            }

            if (s.islemSonuc && (!string.IsNullOrWhiteSpace(form.motorNo)  || !string.IsNullOrWhiteSpace(form.plakaNo)))
            {
                ObjectArray oa = Listele(ve, kullanan, form, true, null, false);
                if (!oa.sonuc.islemSonuc)
                {
                    s.DegerAta(oa.sonuc.hataStr);
                }
                else if (oa.ObjeSayisi > 0)
                {
                    s.DegerAta("Aynı plaka ya da motor numarasına sahip bir kayıt bulunduğundan kayıt yapamazsınız");
                }
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (okunan == null)
                {
                    s = tabBHFTASITBILGI.Ekle(form);
                    if (s.islemSonuc) s = tabBHFTASITBILGI.DetayEkle(form);
                }
                else
                {
                    s = tabBHFTASITBILGI.Guncelle(form);
                    if (s.islemSonuc) s = tabBHFTASITBILGI.DetayGuncelle(form);
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasitBilgi form)
        {
            Sonuc s = new Sonuc();
            if (form == null || string.IsNullOrWhiteSpace(form.saseNo))
            {
                s.DegerAta("Silmek istediğiniz aracın şase numarasını giriniz!");
            }
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITBILGI tabBHFTASITBILGI = new TABBHFTASITBILGI(ve);
            ObjectVAN ov = Oku(ve, kullanan, form);
            TasitBilgi okunan = (TasitBilgi)ov.o;
            if (!ov.sonuc.islemSonuc)
                s = ov.sonuc;
            else
            {
                if (okunan != null)
                {
                    if (okunan.onayKodu > 1)
                        s.DegerAta("Onaylı veya değiştirilmiş formları değiştiremezsiniz!");
                    if (!YazmaYetkisiVarMi(kullanan, okunan.bulunduguSaymanlik, okunan.bulunduguBirim))
                        s.DegerAta("Bu aracın daha önce kaydedildiği birime yazma hakkınız olmadığından araç bilgilerini değiştiremezsiniz!");
                }
                else
                    s.DegerAta("Silinmek istenen araç bulunamadı!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            s = tabBHFTASITBILGI.DetaySil(form);
            if (s.islemSonuc)
                s = tabBHFTASITBILGI.Sil(form);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// TasitBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, TasitBilgi form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form, false, null, true);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, TasitBilgi form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// TasitBilgi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasitBilgi form, bool ayniKayitlar, TNSCollection formlar, bool resimGelsin)
        {
            ObjectArray oa = new ObjectArray();
            if (form != null)
            {
                if (string.IsNullOrWhiteSpace(form.saseNo))
                {
                    if (!GormeYetkisiVarMi(kullanan, form.bulunduguSaymanlik, form.bulunduguBirim))
                    {
                        oa.sonuc.DegerAta(ButceFormResI.TasitBilgi002);
                        return oa;
                    }
                }

                Sonuc s = AnahtarAlanlarGecerliMi(kullanan, form);
                if (!s.islemSonuc)
                {
                    oa.sonuc.DegerAta(s.hataStr);
                    return oa;
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITBILGI tabBHFTASITBILGI = new TABBHFTASITBILGI(ve);

            IDataReader dr = tabBHFTASITBILGI.Listele(form, ayniKayitlar, formlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            TasitBilgi detay = null;
            while (dr.Read())
            {
                string gelenSaseNo = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.SASENO, "");

                int sonAlan = 0;
                if (detay == null || gelenSaseNo != detay.saseNo)
                {
                    detay = new TasitBilgi();
                    detay.saseNo = gelenSaseNo;
                    detay.modelKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.MODELKODU, 0);
                    detay.modelYili = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.MODELYILI, 0);
                    detay.cinsKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.CINSKODU, 0);
                    detay.motorNo = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.MOTORNO, "");
                    detay.koltukSayisi = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.KOLTUKSAYISI, 0);
                    detay.yakitTuru = (ENUMYakitTuru)VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.YAKITTURU, 0);
                    detay.aitOlduguButce = (ENUMAitOlduguButce)VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.AITOLDUGUBUTCE, 0);
                    detay.kullanimDurumu = (ENUMKullanimDurumu)VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.KULLANIMDURUMU, 0);
                    detay.onayKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.ONAYKODU, 0);
                    detay.aracSinifi = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.ARACSINIFI, "");
                    detay.ruhsatResimAd = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.RUHSATRESIMAD, "");
                    detay.modelGirilen = VeriErisim.DegerAl(dr, (int)TABBHFTASITBILGI.ENUMAlanlar.MODELGIRILEN, "");
                    byte[] ruhRes;

                    if (!dr.IsDBNull((int)TABBHFTASITBILGI.ENUMAlanlar.RUHSATRESIM))
                        ruhRes = (byte[])dr.GetValue((int)TABBHFTASITBILGI.ENUMAlanlar.RUHSATRESIM);
                    else
                        ruhRes = null;

                    if (resimGelsin)
                        detay.ruhsatResim = ruhRes;
                    else
                        detay.ruhsatResim = null;

                    if (ruhRes == null || ruhRes.Length == 0)
                        detay.resimVar = false;
                    else
                        detay.resimVar = true;

                    sonAlan = ((int)TABBHFTASITBILGI.ENUMAlanlar.MODELGIRILEN) + 1;
                    detay.markasi = VeriErisim.DegerAl(dr, sonAlan++, "");
                    detay.modeli = VeriErisim.DegerAl(dr, sonAlan++, "");
                    detay.cinsi = VeriErisim.DegerAl(dr, sonAlan++, "");
                    detay.tipKodu = VeriErisim.DegerAl(dr, sonAlan++, 0);
                    detay.tasitCetveliKodu = VeriErisim.DegerAl(dr, sonAlan++, "");
                    detay.tipi = VeriErisim.DegerAl(dr, sonAlan++, "");
                    detay.kesin = VeriErisim.DegerAl(dr, sonAlan++, 0);
                    detay.motorsuz = VeriErisim.DegerAl(dr, sonAlan++, 0);
                    oa.Ekle(detay);
                }
                else
                    sonAlan = ((int)TABBHFTASITBILGI.ENUMAlanlar.MODELGIRILEN) + 9;
                TasitBilgiDetay tbd = new TasitBilgiDetay();

                tbd.siraNo = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.SIRANO, 0);
                tbd.bulunduguSaymanlik = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.BULUNDUGUSAYMANLIK, 0);
                tbd.bulunduguBirim = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.BULUNDUGUBIRIM, "");
                tbd.aitOlduguSaymanlik = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.AITOLDUGUSAYMANLIK, 0);
                tbd.aitOlduguBirim = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.AITOLDUGUBIRIM, "");
                tbd.plakaNo = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.PLAKANO, "");
                tbd.girisTarih = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.GIRISTARIH);
                tbd.cikisTarih = VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.CIKISTARIH);
                tbd.cikisSebebi = (ENUMAracCikisSebebi)VeriErisim.DegerAl(dr, sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.CIKISSEBEBI, 0);
                int sonAlan2 = sonAlan + (int)TABBHFTASITBILGI.ENUMAlanlarDetay.CIKISSEBEBI + 1;
                tbd.aitOlduguSaymanlikAd = VeriErisim.DegerAl(dr, sonAlan2++, "");
                tbd.aitOlduguBirimAd = VeriErisim.DegerAl(dr, sonAlan2++, "");
                tbd.bulunduguSaymanlikAd = VeriErisim.DegerAl(dr, sonAlan2++, "");
                tbd.bulunduguBirimAd = VeriErisim.DegerAl(dr, sonAlan2++, "");
                detay.HareketEkle(tbd);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, TNSCollection formlar, int islemKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITBILGI tabBHFTASITBILGI = new TABBHFTASITBILGI(ve);
            if (formlar.Count == 0)
            {
                sonuc.DegerAta("Lütfen onaylanacak kayıtları seçiniz");
                return sonuc;
            }

            ObjectArray oa = Listele(ve, kullanan, null, false, formlar, false);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }
            if (oa.ObjeSayisi == 0)
            {
                sonuc.DegerAta("Lütfen onaylanacak kayıtları seçiniz");
                return sonuc;
            }

            TasitBilgi form = (TasitBilgi)oa.objeler[0];
            foreach (TasitBilgi item in oa.objeler)
            {
                if (item.bulunduguBirim != form.bulunduguBirim || item.bulunduguSaymanlik != form.bulunduguSaymanlik)
                    sonuc.DegerAta("Onaylanacak formlar listesinde birden fazla birime ait kayıt bulundu. Lütfen aynı birime ait kayıtları seçiniz!");

                string hata = DurumDegisirMi(form.onayKodu, OnayKodBul(islemKod));
                if (!string.IsNullOrWhiteSpace(hata))
                    sonuc.DegerAta(hata);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            if (islemKod == 3)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                    sonuc.DegerAta(ButceFormResI.TasitBilgi002);
            }
            else
            {
                MUH.ENUMOGY ogy;
                if (islemKod == 1)
                    ogy = MUH.ENUMOGY.Onaylama;
                else
                    ogy = MUH.ENUMOGY.OnayKaldir;
                if (!OnaylamaYetkisiVarMi(kullanan, form.bulunduguSaymanlik, form.bulunduguBirim, ogy))
                    sonuc.DegerAta(ButceFormResI.TasitBilgi002);
            }

            sonuc = tabBHFTASITBILGI.OnayKodAta(formlar, OnayKodBul(islemKod));

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static int OnayKodBul(int islemKod)
        {
            return islemKod == 1 ? 5 : (islemKod == 2 ? 2 : 1);
        }

        private static string DurumDegisirMi(int formOnayKod, int yeniOnayKod)
        {
            if (yeniOnayKod == 1)
            {
                if (formOnayKod == 1) return "Zaten yeni olan formu yeni yapamazsınız!";
            }
            else if (yeniOnayKod == 2)
            {
                if (formOnayKod != 5) return "Onay kaldırmak için, öncelikle onaylamanız gerekmektedir!";
            }
            else if (yeniOnayKod == 5)
            {
                if (formOnayKod == 5) return "Onaylı formu onaylayamazsınız!";
            }
            return null;
        }

        internal static ObjectVAN TasitIslemTalepOkuBekleyen(VeriErisim veriErisim, Kullanici kullanan, string saseNo, int talepNo = 0)
        {
            ObjectArray oa = TasitIslemTalepListele(veriErisim, kullanan, saseNo, talepNo, true);
            ObjectVAN ov = new ObjectVAN();
            ov.sonuc = oa.sonuc;
            if (oa.ObjeSayisi > 0) ov.o = oa.objeler[0];
            return ov;
        }

        public static ObjectArray TasitIslemTalepListele(VeriErisim veriErisim, Kullanici kullanan, string saseNo, int talepNo, bool bekleyen)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFTASITBILGI tabBHFTASITBILGI = new TABBHFTASITBILGI(ve);
            ObjectArray oa = new ObjectArray();
            bool yetkili = false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                yetkili = true;
            }

            if (string.IsNullOrWhiteSpace(saseNo) && !yetkili)
                oa.sonuc.DegerAta("Bu işlemi sistem yöneticileri yapabilir!");

            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa;
            }

            TABBHFTASITISLEMTALEP tabBHFTASITISLEMTALEP = new TABBHFTASITISLEMTALEP(ve);

            TasitIslemTalep talepForm = new TasitIslemTalep();
            talepForm.saseNo = saseNo;
            talepForm.talepNo = talepNo;
            if (bekleyen)
                talepForm.onayKodu = 1;

            IDataReader dr = tabBHFTASITISLEMTALEP.Listele(talepForm);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                TasitIslemTalep detay = new TasitIslemTalep();
                detay.saseNo = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.SASENO, "");
                detay.talepNo = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.TALEPNO, 0);
                detay.talepTur = (ENUMTasitIslemTur)VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.TALEPTUR, 0);
                detay.gidecegiSaymanlik = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.GIDECEGISAYMANLIK, 0);
                detay.gidecegiBirim = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.GIDECEGIBIRIM, "");
                detay.sahiplikDegismesin = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.SAHIPLIKDEGISMESIN, 0);
                detay.ayrildigiTarih = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.AYRILDIGITARIH);
                detay.ayrildigiEvrakSayi = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.AYRILDIGIEVRAKSAYI, "");
                detay.onayKodu = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ONAYKODU, 0);
                detay.talepYapan = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.TALEPYAPAN, "");
                detay.islemYapan = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN, "");

                detay.gidecegiSaymanlikAd = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 1, "");
                detay.gidecegiBirimAd = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 2, "");

                detay.talepSaymanlik = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 3, 0);
                detay.talepBirim = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 4, "");
                detay.talepSaymanlikAd = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 5, "");
                detay.talepBirimAd = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 6, "");
                detay.plakaNo = VeriErisim.DegerAl(dr, (int)TABBHFTASITISLEMTALEP.ENUMAlanlar.ISLEMYAPAN + 7, "");
                if (!yetkili && !GormeYetkisiVarMi(kullanan, detay.talepSaymanlik, detay.talepBirim))
                {
                    oa.sonuc.DegerAta("Bu aracın kayıtlı olduğu birimle ilgili işlem yetkiniz olmadığından işlem yapamazsınız!");
                    break;
                }

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc TasitIslemTalepKaydet(VeriErisim veriErisim, Kullanici kullanan, TasitIslemTalep form)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TasitBilgi tbForm = new TasitBilgi();
            tbForm.saseNo = form.saseNo;
            ObjectArray oa = Listele(ve, kullanan, tbForm, false, null, false);
            if (oa.sonuc.islemSonuc)
            {
                if (oa.ObjeSayisi == 0)
                {
                    oa.sonuc.DegerAta("Şase numarasını verdiğiniz araç bulunamadı");
                }
                else
                {
                    tbForm = (TasitBilgi)oa.objeler[0];
                    if (!YazmaYetkisiVarMi(kullanan, tbForm.bulunduguSaymanlik, tbForm.bulunduguBirim))
                    {
                        oa.sonuc.DegerAta("Bu aracın kayıtlı olduğu birimle ilgili yazma hakkınız olmadığından işlem yapamazsınız!");
                    }
                    if (tbForm.onayKodu != 5)
                    {
                        oa.sonuc.DegerAta("Onaylanmamış araçlar için talep yapılamaz!");
                    }
                }
            }
            oa.objeler.Clear();

            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            s = IslemAlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            TABBHFTASITISLEMTALEP tabBHFTASITISLEMTALEP = new TABBHFTASITISLEMTALEP(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
            {
                if (form.talepNo == 0)
                {
                    form.talepNo = tabBHFTASITISLEMTALEP.TalepNoBul(form.saseNo);
                }
                else
                    s = tabBHFTASITISLEMTALEP.Sil(form);
            }
            form.talepYapan = kullanan.kullaniciKodu;
            if (s.islemSonuc)
                s = tabBHFTASITISLEMTALEP.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            s.anahtar = form.talepNo.ToString();
            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc IslemAlanlarGecerliMi(Kullanici kullanan, TasitIslemTalep form)
        {
            return new Sonuc();
        }

        internal static Sonuc TasitIslemTalepOnayla(VeriErisim veriErisim, Kullanici kullanan, TNSCollection talepListesi, int onayKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc s = null;
            foreach (TasitIslemTalep tit in talepListesi)
            {
                s = TasitIslemTalepOnayla(ve, kullanan, tit, onayKod);
                if (!s.islemSonuc)
                {
                    break;
                }
            }
            ve.VeriErisimKapat();
            if (s == null)
                s = new Sonuc();
            return s;
        }

        private static Sonuc TasitIslemTalepOnayla(VeriErisim ve, Kullanici kullanan, TasitIslemTalep tit, int onayKod)
        {
            Sonuc s = new Sonuc();
            ObjectVAN ov = TasitIslemTalepOkuBekleyen(ve, kullanan, tit.saseNo, tit.talepNo);
            if (!ov.sonuc.islemSonuc)
                return ov.sonuc;

            if (ov.o == null)
                s.DegerAta("Değiştirilecek kayıt bulunamadı");
            TasitIslemTalep okunan = (TasitIslemTalep)ov.o;

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                s.DegerAta("Bu işlemi sistem yöneticileri yapabilir!");

            if (!s.islemSonuc)
                return s;

            ve.BeginTransaction();
            TABBHFTASITISLEMTALEP tab = new TABBHFTASITISLEMTALEP(ve);
            TABBHFTASITBILGI tabTasit = new TABBHFTASITBILGI(ve);

            s = tab.OnayKodAta(tit, onayKod, kullanan.kullaniciKodu);
            if (onayKod == 5 && s.islemSonuc)
            {
                if (s.islemSonuc)
                {
                    TasitBilgi tasitBilgi = new TasitBilgi();
                    int siraNo;
                    if (okunan.talepTur == ENUMTasitIslemTur.KurumIciDevir)
                    {
                        s = tabTasit.DetayArttir(tit.saseNo);
                        tasitBilgi.saseNo = tit.saseNo;
                        tasitBilgi.bulunduguSaymanlik = okunan.gidecegiSaymanlik;
                        tasitBilgi.bulunduguBirim = okunan.gidecegiBirim;
                        if (okunan.sahiplikDegismesin == 1)
                        {
                            tasitBilgi.aitOlduguSaymanlik = okunan.talepSaymanlik;
                            tasitBilgi.aitOlduguBirim = okunan.talepBirim;
                        }
                        else
                        {
                            tasitBilgi.aitOlduguSaymanlik = okunan.gidecegiSaymanlik;
                            tasitBilgi.aitOlduguBirim = okunan.gidecegiBirim;
                        }
                        tasitBilgi.plakaNo = okunan.plakaNo;
                        tasitBilgi.girisTarih = okunan.ayrildigiTarih;
                        tasitBilgi.cikisTarih = null;
                        tasitBilgi.cikisEvrakSayisi = null;
                        tasitBilgi.cikisSebebi = ENUMAracCikisSebebi.Bos;
                        s = tabTasit.DetayEkle(tasitBilgi);
                        siraNo = 1;
                    }
                    else
                        siraNo = 0;
                    if (s.islemSonuc)
                    {
                        ENUMAracCikisSebebi cikSebep = ENUMAracCikisSebebi.DIGER;
                        if (okunan.talepTur == ENUMTasitIslemTur.KurumIciDevir)
                            cikSebep = ENUMAracCikisSebebi.KURUMICIDEVIR;
                        else if (okunan.talepTur == ENUMTasitIslemTur.KurumDisiDevir)
                            cikSebep = ENUMAracCikisSebebi.KURUMDISIDEVIR;
                        else if (okunan.talepTur == ENUMTasitIslemTur.Terkin)
                            cikSebep = ENUMAracCikisSebebi.TERKIN;
                        else if (okunan.talepTur == ENUMTasitIslemTur.Hurda)
                            cikSebep = ENUMAracCikisSebebi.HURDA;
                        s = tabTasit.DetayGuncelle(tit.saseNo, siraNo, okunan.ayrildigiTarih, okunan.ayrildigiEvrakSayi, cikSebep);
                    }
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return s;
        }

        internal static Sonuc TasitRuhsatResimKaydet(VeriErisim veriErisim, Kullanici kullanan, string saseNo, string resimAd, byte[] resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABBHFTASITBILGI to = new TABBHFTASITBILGI(ve);

            sonuc = to.ResimGuncelle(saseNo, resimAd, resim);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
