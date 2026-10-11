using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    /// <summary>
    /// IKYLojmanSite formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class IKYKomisyon
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        /// <summary>
        /// IKYKomisyon nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYKomisyon yapısında IKYKomisyon:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetKomisyon(VeriErisim veriErisim, Kullanici kullanan, Komisyon form)
        {
            Sonuc s = new Sonuc(true, "", "");

            //string hata = "";

            //if (!string.IsNullOrWhiteSpace(hata))
            //{
            //    s.islemSonuc = false;
            //    s.hataStr = hata;
            //    return s;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYKOMISYON TAB = new TABIKYKOMISYON(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod > 0)
                    s = TAB.GuncelleKomisyon(form);
                else
                {
                    form.kod = TABGenel.SayacDegerAlInt(ve, TABIKYKOMISYON.TABLOAD);
                    for (int i = 0; i < form.imzalar.Length; i++)
                        form.imzalar[i].komisyonKod = form.kod;
                    s = TAB.KaydetKomisyon(form);
                }
            }

            if (s.islemSonuc)
                s = ImzalariKaydet(ve, kullanan, form.imzalar);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Komisyon listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Komisyon yapısında Komisyon:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleKomisyon(VeriErisim veriErisim, Kullanici kullanan, Komisyon form)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYKOMISYON TAB = new TABIKYKOMISYON(ve);

            IDataReader dr;
            dr = TAB.OkuKomisyon(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYKOMISYON.ENUMAlanlar))).Length;
            while (dr.Read())
            {
                Komisyon detay = new Komisyon();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlar.KOD, 0);
                detay.kararTarihi = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlar.KARARTARIHI);
                detay.kararSayisi = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlar.KARARSAYISI, "");
                detay.toplantiTarihi = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlar.TOPLANTITARIHI);
                detay.yonerge = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlar.YONERGE, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// Komisyon nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Komisyon yapısında Komisyon:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuKomisyon(VeriErisim veriErisim, Kullanici kullanan, Komisyon form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleKomisyon(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0)
            {
                Komisyon komisyon = (Komisyon)s[0];

                int cnt = 0;

                ObjectArray liste = ListeleImza(veriErisim, kullanan, new IKYImza { komisyonKod = komisyon.kod });
                komisyon.imzalar = new IKYImza[liste.ObjeSayisi];

                foreach (IKYImza i in liste.objeler)
                {
                    komisyon.imzalar[cnt] = i;
                    cnt++;
                }

                ov.o = komisyon;
            }

            return ov;
        }

        public static Sonuc SilKomisyon(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";

            if (kod < 0)
                hata += "Komisyon seçilmemiş.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYKOMISYON tab = new TABIKYKOMISYON(ve);

            ve.BeginTransaction();

            sonuc = tab.SilImza(new IKYImza { komisyonKod = kod });

            if (sonuc.islemSonuc)
                sonuc = tab.SilKomisyon(kod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// IKYImza listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IKYImza yapısında IKYImza:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleImza(VeriErisim veriErisim, Kullanici kullanan, IKYImza form)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYKOMISYON TAB = new TABIKYKOMISYON(ve);

            IDataReader dr;
            dr = TAB.OkuImza(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            int AlanlarSonIndex = (Enum.GetNames(typeof(TABIKYKOMISYON.ENUMAlanlarIMZA))).Length;
            while (dr.Read())
            {
                IKYImza detay = new IKYImza();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.KOD, 0);
                detay.komisyonKod = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.KOMISYONKOD, 0);
                detay.tip = (ENUMKomisyonTip)VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.TIP, 0);
                detay.unvan = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.UNVAN, "");
                detay.isim = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.ISIM, "");
                detay.imzaTur = (ENUMImzaTur)VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.IMZATUR, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABIKYKOMISYON.ENUMAlanlarIMZA.SIRANO, 0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc SilImza(Kullanici kullanan, IKYImza sk)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
            {
                TABIKYKOMISYON tab = new TABIKYKOMISYON(ve);
                sonuc = tab.SilImza(sk);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc ImzalariKaydet(VeriErisim veriErisim, Kullanici kullanan, IKYImza[] imzalar)
        {
            Sonuc sonuc = new Sonuc();
            if (imzalar.Length == 0)
                return sonuc;

            //if (!string.IsNullOrEmpty(sonuc.hataStr))
            //{
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABIKYKOMISYON tabKesinti = new TABIKYKOMISYON(ve);
            ve.BeginTransaction();


            if (imzalar[0].komisyonKod > 0)
                sonuc = tabKesinti.SilImza(imzalar[0]);

            if (sonuc.islemSonuc)
            {
                foreach (IKYImza i in imzalar)
                {
                    i.kod = TABGenel.SayacDegerAlInt(ve, TABIKYKOMISYON.TABLOADIMZA);
                    sonuc = tabKesinti.KaydetImza(i);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
