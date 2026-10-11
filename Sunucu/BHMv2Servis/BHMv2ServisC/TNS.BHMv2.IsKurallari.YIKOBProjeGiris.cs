using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2YIKOBProjeGiris formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class YIKOBProjeGiris
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, YIKOBProje form)
        {
            TNS.MUH.IMUHServis servis = TNS.MUH.Arac.Tanimla();
            return servis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, MUH.ENUMOGY.Yazma);
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, YIKOBProje form)
        {
            return new Sonuc(form.Kontrol());
        }

        /// <summary>
        /// YIKOBProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YIKOBProje yapısında YIKOBProje:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, YIKOBProje form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
                return s;
            }

            if (BirimOnayliMi(kullanan, form))
                s.DegerAta("Biriminiz onaylı olduğundan değişiklik yapamazsınız!");
            else
                s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2YIKOBPROJE tabBHMV2YIKOBPROJE = new TABBHMV2YIKOBPROJE(ve);
            TABBHMV2YIKOBPROJEBUTCE tabBHMV2YIKOBPROJEBUTCE = new TABBHMV2YIKOBPROJEBUTCE(ve);

            ve.BeginTransaction();
            if (form.siraNo == 0)
            {
                form.siraNo = tabBHMV2YIKOBPROJE.SiraNoBul(form);
            }
            if (s.islemSonuc)
                s = tabBHMV2YIKOBPROJEBUTCE.Sil(form);
            if (s.islemSonuc)
                s = tabBHMV2YIKOBPROJE.Sil(form);

            if (s.islemSonuc)
                s = tabBHMV2YIKOBPROJE.Ekle(form);

            for (int i = 0; i < form.projeButce.Count && s.islemSonuc; i++)
            {
                YIKOBProjeButce ypb = (YIKOBProjeButce)form.projeButce[i];
                if (ypb.Dolu())
                    s = tabBHMV2YIKOBPROJEBUTCE.Ekle(form, i);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.siraNo.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static bool BirimOnayliMi(Kullanici kullanan, YIKOBProje form)
        {
            BKD.TavanGenel tg = new BKD.TavanGenel();
            tg.kod1 = form.muhasebeKod.ToString();
            tg.kod2 = form.birimKod;
            tg.kod3 = form.yil.ToString();
            tg = TavanGenelIslemler.BHMTavanGenelOnayOku(null, kullanan, 0, "00.00", BKD.ENUMTavanTur.tavanTurYIKOBProje, tg);
            return tg.durum == 5;
        }

        /// <summary>
        /// YIKOBProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YIKOBProje yapısında YIKOBProje:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, YIKOBProje form)
        {
            ObjectVAN ov = new ObjectVAN();

            ObjectArray s = Listele(veriErisim, kullanan, form);
            if (s == null || s.ObjeSayisi == 0) ov.sonuc.DegerAta("Kayıt bulunamadı");
            ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, YIKOBProje form)
        {
            TNS.MUH.IMUHServis servis = TNS.MUH.Arac.Tanimla();
            return servis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, MUH.ENUMOGY.Gorme);
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, YIKOBProje form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// YIKOBProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YIKOBProje yapısında YIKOBProje:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, YIKOBProje form)
        {
            ObjectArray alAna = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                alAna.sonuc.DegerAta(ButceHazirlikResI.IOIProjeGiris002);
                return alAna;
            }

            alAna.sonuc = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!alAna.sonuc.islemSonuc)
            {
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2YIKOBPROJE tabBHMV2YIKOBPROJE = new TABBHMV2YIKOBPROJE(ve);
            TABBHMV2YIKOBPROJEBUTCE tabBHMV2YIKOBPROJEBUTCE = new TABBHMV2YIKOBPROJEBUTCE(ve);

            IDataReader dr = tabBHMV2YIKOBPROJE.Listele(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }

            while (dr.Read())
            {
                YIKOBProje detay = new YIKOBProje();
                detay.yil = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YIL, 0);
                detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.MUHASEBEKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.BIRIMKOD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.SIRANO, 0);
                detay.projeAdi = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.PROJEADI, "");
                detay.baslaYil = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.BASLAYIL, 0);
                detay.bitisYil = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.BITISYIL, 0);
                detay.toplamMaliyet = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.TOPLAMMALIYET, 0.0);
                detay.uygulamaYeri = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.UYGULAMAYERI, "");
                detay.projeninAmaci = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.PROJENINAMACI, "");
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.ACIKLAMA, "");
                detay.yatirimYil = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMYIL, 0);
                detay.yatirimYil = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMYIL, 0);
                detay.yatirimProjeAd = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJEAD, "");
                detay.yatirimProjeNo = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO, "");


                string muhAd = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 1, "");
                string birAd = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 2, "");
                detay.muhasebeBirimAd = TNS.MUH.Arac.BirimAdOl(muhAd, birAd);

                detay.yIKOBButce = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 3, 0.0);
                detay.bakButce = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 4, 0.0);
                detay.yIKOBHarc = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 5, 0.0);
                detay.bakHarc = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 6, 0.0);
                //detay.yIKOBDevir = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 7, 0.0);
                //detay.bakDevir = VeriErisim.DegerAl(dr, ((int)TABBHMV2YIKOBPROJE.ENUMAlanlar.YATIRIMPROJENO) + 8, 0.0);

                alAna.Ekle(detay);
            }
            dr.Close();
            TNS.BKD.ABSKodlarVAN oa = YIKOBProje.EkonomikListele(form.yil);
            foreach (YIKOBProje formIc in alAna.objeler)
            {
                formIc.EkoDoldur(oa);
                dr = tabBHMV2YIKOBPROJEBUTCE.Oku(formIc);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                while (dr.Read())
                {
                    string ekoKod = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.EKOKOD, "");

                    YIKOBProjeButce detay = formIc.ProjeButceBul(ekoKod);
                    if (detay != null)
                    {
                        detay.yIKOBButce = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.YIKOBBUTCE, 0.0);
                        detay.yIKOBHarc = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.YIKOBHARC, 0.0);
                        //detay.yIKOBDevir = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.YIKOBDEVIR, 0.0);
                        detay.bakButce = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.BAKBUTCE, 0.0);
                        detay.bakHarc = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.BAKHARC, 0.0);
                        //detay.bakDevir = VeriErisim.DegerAl(dr, (int)TABBHMV2YIKOBPROJEBUTCE.ENUMAlanlar.BAKDEVIR, 0.0);
                    }
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return alAna;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, YIKOBProje form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
                return s;
            }

            if (BirimOnayliMi(kullanan, form))
                s.DegerAta("Biriminiz onaylı olduğundan değişiklik yapamazsınız!");

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2YIKOBPROJE tabBHMV2YIKOBPROJE = new TABBHMV2YIKOBPROJE(ve);
            TABBHMV2YIKOBPROJEBUTCE tabBHMV2YIKOBPROJEBUTCE = new TABBHMV2YIKOBPROJEBUTCE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMV2YIKOBPROJEBUTCE.Sil(form);
            if (s.islemSonuc)
                s = tabBHMV2YIKOBPROJE.Sil(form);

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = "0";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
    }
}
