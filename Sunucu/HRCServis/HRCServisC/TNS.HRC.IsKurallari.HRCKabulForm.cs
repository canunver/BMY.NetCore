using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Collections.Generic;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCkabulForm formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCkabulForm
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, IhaleKabul form)
        {
            return true;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, IhaleKabul form)
        {
            Sonuc s = new Sonuc(true, "", "");
            string hataStr = "";
            if (form == null)
            {
                s.DegerAta("Boş form gönderildi!");
            }
            else
            {
                if (string.IsNullOrEmpty(form.vergiNo))
                    hataStr += "Lütfen firma seçiniz!<br>";
                if (form.firmaMuhasebeKod < 1)
                    hataStr += "Lütfen bir muhasebe seçiniz!<br>";
                if (form.komUyeleri != null)
                {
                    if (form.komUyeleri.Length < 1)
                        hataStr += "Lütfen en az 1 komisyon üyesi seçiniz!<br>";
                    for (int i = 0; i < form.komUyeleri.Length; i++)
                    {
                        string simdTC = form.komUyeleri[i].tcKimlik;
                        if (string.IsNullOrEmpty(form.komUyeleri[i].gorev))
                            hataStr += string.Format("{0} numaralı üyenin görevi belirtilmemiş!<br>", simdTC);
                        for (int j = i + 1; j < form.komUyeleri.Length; j++)
                        {
                            if (simdTC == form.komUyeleri[j].tcKimlik)
                                hataStr += string.Format("{0} numaralı üye iki kez yazılmış!<br>", simdTC);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(hataStr))
                    s.DegerAta(hataStr);
            }
            return s;
        }

        /// <summary>
        /// IhaleKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IhaleKabul form)
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
            TABHRCIHLKABUL tabTutanak = new TABHRCIHLKABUL(ve);
            TABHRCIHLKABULKOMISYON tabKomisyon = new TABHRCIHLKABULKOMISYON(ve);

            ve.BeginTransaction();
            if (form.tutanakNo == 0)
            {
                form.tutanakNo = tabTutanak.EnBuyukNo(form) + 1;
            }
            else
            {
                if (s.islemSonuc)
                    s = tabKomisyon.Sil(form);
                if (s.islemSonuc)
                    s = tabTutanak.Sil(form);
            }

            if (s.islemSonuc)
                s = tabTutanak.Ekle(form);

            if (form.komUyeleri != null)
            {
                for (int i = 0; i < form.komUyeleri.Length && s.islemSonuc; i++)
                {
                    s = tabKomisyon.Ekle(form, i);
                }
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.tutanakNo.ToString();
            }
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, IhaleKabul form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, IhaleKabul form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// IhaleKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        public static IhaleKabul Oku(VeriErisim veriErisim, Kullanici kullanan, IhaleKabul form)
        {
            IhaleKabul[] k = Listele(veriErisim, kullanan, form, true);
            if (k == null) return null;
            if (k.Length == 0) return null;
            return k[0];
        }

        internal static IhaleKabul[] Listele(VeriErisim veriErisim, Kullanici kullanan, IhaleKabul form, bool detayVar)
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
            TABHRCIHLKABUL tabTutanak = new TABHRCIHLKABUL(ve);

            IDataReader dr = tabTutanak.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            List<IhaleKabul> al = new List<IhaleKabul>();
            while (dr.Read())
            {
                IhaleKabul formIc = new IhaleKabul();
                formIc.surecYil = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SURECYIL, 0);
                formIc.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SURECNO, "");
                formIc.tutanakNo = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.TUTANAKNO, 0);
                formIc.tutanakTur = (ENUMIhaleKabulTutanakTur)VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.TUTANAKTUR, 0);
                formIc.vergiNo = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.FIRMANO, "");
                formIc.firmaMuhasebeKod = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.FIRMAMUHASEBEKOD, 0);
                formIc.sozlesmeTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SOZLESMETARIHI);
                formIc.sozlesmeBedeli = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SOZLESMEBEDELI, 0.0);
                formIc.faturaTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.FATURATARIHI);
                formIc.faturaNo = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.FATURANO, "");
                formIc.faturaTutari = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.FATURATUTARI, 0.0);
                formIc.isinSuresi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.ISINSURESI, "");
                formIc.sozBitisTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SOZBITISTARIHI);
                formIc.sureUzatim = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SUREUZATIM, "");
                formIc.sonBitisTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.SONBITISTARIHI);
                formIc.onayTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.ONAYTARIHI);
                formIc.tutanakTarihi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.TUTANAKTARIHI);
                formIc.malSiraNo = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABUL.ENUMAlanlar.MALSIRANO, 0);
                formIc.firmaAd = VeriErisim.DegerAl(dr, 18, "");

                al.Add(formIc);
            }

            dr.Close();
            if (detayVar)
            {
                TABHRCIHLKABULKOMISYON tabKomisyon = new TABHRCIHLKABULKOMISYON(ve);
                foreach (IhaleKabul formIc in al)
                {
                    ArrayList alIc = new ArrayList();
                    dr = tabKomisyon.Listele(formIc);
                    if (dr == null)
                    {
                        ve.VeriErisimKapat();
                        throw new Exception(ve.hata);
                    }

                    while (dr.Read())
                    {
                        IhaleKomisyonUye detay = new IhaleKomisyonUye();
                        detay.tcKimlik = VeriErisim.DegerAl(dr, 1, "");
                        detay.gorev = VeriErisim.DegerAl(dr, 2, "");
                        detay.adSoyad = VeriErisim.DegerAl(dr, 3, "");
                        alIc.Add(detay);
                    }
                    formIc.komUyeleri = (IhaleKomisyonUye[])alIc.ToArray(typeof(IhaleKomisyonUye));
                    dr.Close();
                }
            }

            ve.VeriErisimKapat();
            return al.ToArray();
        }

        internal static Sonuc MalzemeKaydet(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, int tutanakNo, IhaleKabulMalzeme[] malListesi)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABHRCIHLKABULMALZEME tabHRCIHLKABULMALZEME = new TABHRCIHLKABULMALZEME(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tabHRCIHLKABULMALZEME.Sil(surecYil, surecNo, tutanakNo);

            for (int i = 0; i < malListesi.Length && s.islemSonuc; i++)
            {
                s = tabHRCIHLKABULMALZEME.Ekle(surecYil, surecNo, tutanakNo, i, malListesi[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static IhaleKabulMalzeme[] MalzemeListele(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, int tutanakNo)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCIHLKABULMALZEME tabHRCIHLKABULMALZEME = new TABHRCIHLKABULMALZEME(ve);

            IDataReader dr = tabHRCIHLKABULMALZEME.Listele(surecYil, surecNo, tutanakNo);

            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            List<IhaleKabulMalzeme> al = new List<IhaleKabulMalzeme>();
            while (dr.Read())
            {
                IhaleKabulMalzeme detay = new IhaleKabulMalzeme();
                detay.malKodu = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABULMALZEME.ENUMAlanlar.MALKOD, "");
                detay.malAdi = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABULMALZEME.ENUMAlanlar.MALAD, "");
                detay.miktar = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABULMALZEME.ENUMAlanlar.MIKTAR, 0.0);
                detay.birimFiyat = VeriErisim.DegerAl(dr, (int)TABHRCIHLKABULMALZEME.ENUMAlanlar.BIRIMFIYAT, 0.0);
                al.Add(detay);
            }
            ve.VeriErisimKapat();
            return al.ToArray();

        }
    }
}
