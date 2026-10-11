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
    /// BHFAracKiralamaPlani formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFAracKiralamaPlani
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, AracKiralamaPlanAna form, Sonuc sonuc)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, MUH.ENUMOGY.Yazma))
            {
                sonuc.DegerAta("Bu birimle (muhasebe:" + form.muhasebeKod + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
            }
            return sonuc.islemSonuc;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static bool AlanlarGecerliMi(Kullanici kullanan, AracKiralamaPlanAna form, Sonuc sonuc)
        {
            if (AnahtarAlanlarGecerliMi(kullanan, form, sonuc))
            {
                form.RakamlarGecerliMi(sonuc);
            }
            return sonuc.islemSonuc;
        }


        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AracKiralamaPlanAna form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form, s))
            {
                return s;
            }

            if (!AlanlarGecerliMi(kullanan, form, s))
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = Oku(ve, kullanan, form);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            if (ov.o != null)
            {
                AracKiralamaPlanAna okunan = (AracKiralamaPlanAna)ov.o;
                if (okunan.durum == 5)
                {
                    s.DegerAta("Onaylı formlar değiştirilemez!");
                    ve.VeriErisimKapat();
                    return s;
                }
            }

            TABBHFARACKIRALAMAPLAN tabBHFARACKIRALAMAPLAN = new TABBHFARACKIRALAMAPLAN(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHFARACKIRALAMAPLAN.SilDetay(form);
            if (s.islemSonuc)
                s = tabBHFARACKIRALAMAPLAN.SilAna(form);

            if (s.islemSonuc)
                s = tabBHFARACKIRALAMAPLAN.EkleAna(form, kullanan.kullaniciKodu);

            for (int i = 0; i < form.aracKiralamaPlanDetay.Count && s.islemSonuc; i++)
            {
                s = tabBHFARACKIRALAMAPLAN.EkleDetay(form, i);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, AracKiralamaPlanAna form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = Listele(veriErisim, kullanan, form);
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, AracKiralamaPlanAna form, Sonuc sonuc)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, MUH.ENUMOGY.Gorme))
            {
                sonuc.DegerAta("Bu birimle (muhasebe:" + form.muhasebeKod + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
            }
            return sonuc.islemSonuc;

        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static bool AnahtarAlanlarGecerliMi(Kullanici kullanan, AracKiralamaPlanAna form, Sonuc sonuc)
        {
            if (form.yil <= 0)
                sonuc.DegerAta("Lütfen yıl giriniz!");

            if (form.muhasebeKod <= 0)
                sonuc.DegerAta("Lütfen muhasebe giriniz!");

            if (string.IsNullOrWhiteSpace(form.birimKod) || form.birimKod.Length != 3)
                sonuc.DegerAta("Lütfen birim giriniz!");

            if (form.birimKod == "250" || form.birimKod == "000")
                sonuc.DegerAta("Bu form, ilçe müdürlükleri tarafından kullanılmayacaktır!");

            return sonuc.islemSonuc;
        }


        /// <summary>
        /// AracKiralamaPlanAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AracKiralamaPlanAna form)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form, oa.sonuc))
            {
                return oa;
            }

            if (!AnahtarAlanlarGecerliMi(kullanan, form, oa.sonuc))
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFARACKIRALAMAPLAN tab = new TABBHFARACKIRALAMAPLAN(ve);

            IDataReader dr;
            dr = tab.OkuAna(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                AracKiralamaPlanAna ana = new AracKiralamaPlanAna();
                ana.yil = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.YIL, 0);
                ana.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.MUHASEBEKOD, 0);
                ana.birimKod = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.BIRIMKOD, "");
                ana.durum = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.DURUM, 0);
                ana.kaydeden = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.KAYDEDEN, "");
                ana.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.KAYITTARIHI);
                ana.onaylayan = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.ONAYLAYAN, "");
                ana.onayTarihi = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarAna.ONAYTARIHI);
                oa.Ekle(ana);
            }
            dr.Close();
            TNSCollection al;
            foreach (AracKiralamaPlanAna formIc in oa.objeler)
            {
                dr = tab.OkuDetay(formIc, form.siralaYontem);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    oa.sonuc.DegerAta(ve.hata);
                    return oa;
                }
                al = new TNSCollection();
                while (dr.Read())
                {
                    AracKiralamaPlanDetay detay = new AracKiralamaPlanDetay();
                    detay.tasitTipi = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.TASITTIPI, 0);
                    detay.tasitKodu = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.TASITKODU, "");
                    detay.butceKaynagi = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.BUTCEKAYNAGI, 0);
                    detay.buYilKiralanan = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.BUYILKIRALANAN, 0);
                    detay.buYilKiralananSure = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.BUYILKIRALANANSURE, 0);
                    detay.gelecekYilKiralanacak = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.GELECEKYILKIRALANACAK, 0);
                    detay.gelecekYilKiralanacakSure = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.GELECEKYILKIRALANACAKSURE, 0);

                    detay.buYilFiyat = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.BUYILFIYAT, 0.0);
                    detay.gelecekYilFiyat = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.GELECEKYILFIYAT, 0.0);
                    detay.oncYilKiralanan = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.ONCYILKIRALANAN, 0);
                    detay.oncYilKiralananSure = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.ONCYILKIRALANANSURE, 0);
                    detay.oncYilFiyat = VeriErisim.DegerAl(dr, (int)TABBHFARACKIRALAMAPLAN.ENUMAlanlarDetay.ONCYILFIYAT, 0.0);

                    al.Add(detay);
                }
                formIc.aracKiralamaPlanDetay = al;
                dr.Close();
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, AracKiralamaPlanAna form, bool kayitYoksaEkle)
        {
            Sonuc s;
            if (form.durum != 5 && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return new Sonuc("Bu işlemi yapmaya yetkiniz bulunmuyor!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = Oku(ve, kullanan, form);
            s = ov.sonuc;
            if (ov.sonuc.islemSonuc && (ov.o != null || kayitYoksaEkle))
            {
                if (YazmaYetkisiVarMi(kullanan, form, s))
                {
                    TABBHFARACKIRALAMAPLAN tab = new TABBHFARACKIRALAMAPLAN(ve);
                    ve.BeginTransaction();

                    if (ov.o == null)
                        s = tab.OnayEkle(form, kullanan.kullaniciKodu);
                    else
                        s = tab.OnayGuncelle(form, kullanan.kullaniciKodu);

                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
            }
            else if (ov.sonuc.islemSonuc && ov.o == null)
            {
                s.DegerAta("Onaylamadan önce kaydetmelisiniz!");
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}
