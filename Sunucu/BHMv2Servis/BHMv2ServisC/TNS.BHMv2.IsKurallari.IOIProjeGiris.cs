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
    /// BHMv2IOIProjeGiris formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2IOIProjeGiris
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, IOIProje form, bool harcama)
        {
            TNS.MUH.IMUHServis servis = TNS.MUH.Arac.Tanimla();
            if (harcama)
                return servis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, MUH.ENUMOGY.Yazma);
            else
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            return false;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, IOIProje form)
        {
            return new Sonuc(form.Kontrol());
        }

        /// <summary>
        /// IOIProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IOIProje form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form, false))
            {
                s.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
                return s;
            }
            s = AlanlarGecerliMi(kullanan, form);

            //if (BirimOnayliMi(kullanan, form))
            //    s.DegerAta("Biriminiz onaylı olduğundan değişiklik yapamazsınız!");
            //else
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            ve.BeginTransaction();
            if (form.siraNo > 0)
                s = tabBHMV2IOIPROJE.Guncelle(form);
            else
            {
                s = tabBHMV2IOIPROJE.Ekle(form);
                form.siraNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
                if (form.siraNo <= 0) s.DegerAta("Kayıt sırasında anahtar alınamadı!");
            }

            //if (s.islemSonuc)
            //{
            //    s = tabBHMV2IOIPROJE.SilPebKodlariSil(form.siraNo);
            //}

            //foreach (IOIProjeBag item in form.pebKodlar)
            //{
            //    if (!s.islemSonuc) break;
            //    s = tabBHMV2IOIPROJE.EklePebKod(form.siraNo, item.yil, item.pebKod);
            //}

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static bool BirimOnayliMi(Kullanici kullanan, IOIProje form)
        {
            BKD.TavanGenel tg = new BKD.TavanGenel();
            tg.kod1 = form.muhasebeKod.ToString();
            tg.kod2 = form.birimKod;
            tg = TavanGenelIslemler.BHMTavanGenelOnayOku(null, kullanan, 0, "00.00", BKD.ENUMTavanTur.tavanTurIOIProje, tg);
            return tg.durum == 5;
        }

        /// <summary>
        /// IOIProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, IOIProje form)
        {
            ObjectVAN ov = new ObjectVAN();

            ObjectArray s = Listele(veriErisim, kullanan, form, false, null, 0);
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
        public static bool GormeYetkisiVarMi(Kullanici kullanan, IOIProje form)
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
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, IOIProje form)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// IOIProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IOIProje form, bool detaylarOlsun, List<int> isKodListesi, int butceYil)
        {
            ObjectArray alAna = new ObjectArray();
            if (form.siraNo == 0)
            {
                if (!GormeYetkisiVarMi(kullanan, form))
                {
                    alAna.sonuc.DegerAta(ButceHazirlikResI.IOIProjeGiris002);
                    return alAna;
                }
            }

            alAna.sonuc = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!alAna.sonuc.islemSonuc)
            {
                return alAna;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            IDataReader dr = tabBHMV2IOIPROJE.Listele(form, isKodListesi, butceYil);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                alAna.sonuc.DegerAta(ve.hata);
                return alAna;
            }

            string pebKodYapi = TNS.BKD.TertipYapi.KodYapiBul(DateTime.Now.Year, 14, false);
            while (dr.Read())
            {
                IOIProje detay = new IOIProje();
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.SIRANO, 0);
                detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.MUHASEBEKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.BIRIMKOD, "");
                //detay.altProjeNo = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.ALTPROJENO, 0);
                detay.ioiProjeAdi = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.IOIPROJEADI, "");
                detay.baslaYil = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.BASLAYIL, 0);
                detay.bitisYil = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.BITISYIL, 0);
                detay.projeTutari = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.PROJETUTARI, 0.0);
                detay.ilKod = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.ILKOD, 0);
                detay.isTur = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.ISTUR, 0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABBHMV2IOIPROJE.ENUMAlanlar.ACIKLAMA, "");

                int baslaKol = ((int)TABBHMV2IOIPROJE.ENUMAlanlar.ACIKLAMA) + 1;
                string muhAd = VeriErisim.DegerAl(dr, baslaKol++, "");
                string birAd = VeriErisim.DegerAl(dr, baslaKol++, "");

                detay.ilAdi = VeriErisim.DegerAl(dr, baslaKol++, "");

                detay.aktarilanTutar = VeriErisim.DegerAl(dr, baslaKol++, 0.0);
                detay.islemleEklenen = VeriErisim.DegerAl(dr, baslaKol++, 0.0);
                detay.iOIHarc = VeriErisim.DegerAl(dr, baslaKol++, 0.0);
                detay.gonderilen = VeriErisim.DegerAl(dr, baslaKol++, 0.0);
                detay.pebKodlar = new TNSCollection();

                string xmlPebKodlar = VeriErisim.DegerAl(dr, baslaKol++, "");
                if (!string.IsNullOrEmpty(xmlPebKodlar))
                {
                    XMLStringDataReader r = new XMLStringDataReader(xmlPebKodlar);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            detay.pebKodlar.Add(ProjeBagOku(r, pebKodYapi));
                        }
                        r.Close();
                    }
                }

                detay.muhasebeBirimAd = TNS.MUH.Arac.BirimAdOl(muhAd, birAd);

                alAna.Ekle(detay);
            }
            dr.Close();

            if (form.siraNo > 0 && alAna.objeler.Count == 1)
            {
                IOIProje detay = (IOIProje)alAna.Al(0);
                if (!GormeYetkisiVarMi(kullanan, detay))
                {
                    alAna.objeler.Clear();
                    alAna.sonuc.DegerAta(ButceHazirlikResI.IOIProjeGiris002);
                }
            }
            if (alAna.sonuc.islemSonuc && detaylarOlsun)
            {
                foreach (IOIProje item in alAna.objeler)
                {
                    item.detaylar = new TNSCollection();
                    item.DetayEkle(IOIProjeAktarimListele(ve, kullanan, item.siraNo, true), true);
                    item.DetayEkle(IOIProjeHarcamaListele(ve, kullanan, item.siraNo, true), false);
                    item.detaylar.Sort();
                }
            }

            ve.VeriErisimKapat();
            return alAna;
        }

        private static IOIProjeBag ProjeBagOku(IDataReader r, string kodYapi)
        {
            IOIProjeBag pb = new IOIProjeBag();
            int kol = 0;
            pb.yil = VeriErisim.DegerAl(r, kol++, 0);
            pb.pebKod = VeriErisim.DegerAl(r, kol++, "");
            pb.pebAd = VeriErisim.DegerAl(r, kol++, "");
            pb.pebAd2 = VeriErisim.DegerAl(r, kol++, "");
            pb.projeNo = VeriErisim.DegerAl(r, kol++, 0);
            pb.DPTProjeNo = VeriErisim.DegerAl(r, kol++, "");
            pb.projeAd = VeriErisim.DegerAl(r, kol++, "");
            pb.altProjeNo = VeriErisim.DegerAl(r, kol++, 0);
            pb.altProjeAd = VeriErisim.DegerAl(r, kol++, "");
            pb.pebKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapi, pb.pebKod);

            return pb;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IOIProje form)
        {
            ////if (BirimOnayliMi(kullanan, form))
            ////    s.DegerAta("Biriminiz onaylı olduğundan değişiklik yapamazsınız!");

            //if (!s.islemSonuc)
            //{
            //    return s;
            //}

            Sonuc s = new Sonuc(true, "", "");
            if (form.siraNo <= 0)
            {
                s.DegerAta("Lütfen silinecek projeyi seçiniz!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = Oku(ve, kullanan, form);
            if (!ov.sonuc.islemSonuc)
                s = ov.sonuc;
            else if (ov.o == null)
                s.DegerAta("Verilen proje bulunamadı!");

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            IOIProje okunan = (IOIProje)ov.o;

            if (!YazmaYetkisiVarMi(kullanan, okunan, false))
                s.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
            if (okunan.aktarilanTutar > 0 || okunan.iOIHarc > 0)
                s.DegerAta("Silinecek kayıtların aktarması veya harcaması olmamalıdır!");

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabBHMV2IOIPROJE.Sil(form);

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

        internal static ObjectArray IOIProjeHarcamaListele(VeriErisim veriErisim, Kullanici kullanan, int ioiSiraNo, bool kontrolEtme)
        {
            ObjectArray oa = new ObjectArray();
            if (ioiSiraNo <= 0)
            {
                oa.sonuc.DegerAta("Lütfen proje numarasını veriniz!");
                return oa;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (!kontrolEtme)
            {
                ObjectVAN ov = Oku(ve, kullanan, new IOIProje() { siraNo = ioiSiraNo });
                if (!ov.sonuc.islemSonuc)
                {
                    oa.sonuc.DegerAta(ov.sonuc.hataStr);
                    ve.VeriErisimKapat();
                    return oa;
                }
            }
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            IDataReader dr = tabBHMV2IOIPROJE.ListeleHarcama(ioiSiraNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                IOIProjeHarcama detay = new IOIProjeHarcama();

                detay.harcamaNo = VeriErisim.DegerAl(dr, 0, 0);
                detay.harcamaTarihi = VeriErisim.DegerAl(dr, 1);
                detay.harcamaTutari = VeriErisim.DegerAl(dr, 2, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, 3, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray IOIProjeAktarimListele(VeriErisim veriErisim, Kullanici kullanan, int ioiSiraNo, bool kontrolEtme)
        {
            ObjectArray oa = new ObjectArray();
            if (ioiSiraNo <= 0)
            {
                oa.sonuc.DegerAta("Lütfen proje numarasını veriniz!");
                return oa;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            if (!kontrolEtme)
            {
                ObjectVAN ov = Oku(ve, kullanan, new IOIProje() { siraNo = ioiSiraNo });
                if (!ov.sonuc.islemSonuc)
                {
                    oa.sonuc.DegerAta(ov.sonuc.hataStr);
                    ve.VeriErisimKapat();
                    return oa;
                }
            }
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            IDataReader dr = tabBHMV2IOIPROJE.ListeleAktarilan(ioiSiraNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                IOIProjeAktarilan detay = new IOIProjeAktarilan();

                detay.aktarmaNo = VeriErisim.DegerAl(dr, 0, 0);
                detay.aktarmaTarihi = VeriErisim.DegerAl(dr, 1);
                detay.aktarilanTutar = VeriErisim.DegerAl(dr, 2, 0.0);
                detay.aciklama = VeriErisim.DegerAl(dr, 3, "");
                detay.tur = VeriErisim.DegerAl(dr, 4, 0);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc IOIProjeHarcamaKaydet(VeriErisim veriErisim, Kullanici kullanan, int ioiSiraNo, IOIProjeHarcama[] liste)
        {
            Sonuc sonuc = new Sonuc();
            if (ioiSiraNo <= 0)
            {
                sonuc.DegerAta("Lütfen proje numarasını veriniz!");
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = Oku(ve, kullanan, new IOIProje() { siraNo = ioiSiraNo });
            if (!ov.sonuc.islemSonuc)
            {
                sonuc.DegerAta(ov.sonuc.hataStr);
                ve.VeriErisimKapat();
                return sonuc;
            }
            if (!YazmaYetkisiVarMi(kullanan, (IOIProje)ov.o, true))
            {
                sonuc.DegerAta("Bu işlemi yapma yetkiniz bulunmamaktadır!");
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);

            int say = 1;
            foreach (IOIProjeHarcama item in liste)
            {
                item.Kontrol(say++, sonuc);
            }

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                foreach (IOIProjeHarcama item in liste)
                {
                    if (item.harcamaNo == 0)
                    {
                        if (item.Dolu())
                            sonuc = tabBHMV2IOIPROJE.HarcamaEkle(ioiSiraNo, item);
                    }
                    else
                    {
                        if (item.Dolu())
                            sonuc = tabBHMV2IOIPROJE.HarcamaGuncelle(item);
                        else
                            sonuc = tabBHMV2IOIPROJE.HarcamaSil(item);
                    }
                    if (!sonuc.islemSonuc) break;
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc IOIProjeAktarimKaydet(VeriErisim veriErisim, Kullanici kullanan, int ioiSiraNo, IOIProjeAktarilan[] liste)
        {
            Sonuc sonuc = new Sonuc();
            if (ioiSiraNo <= 0)
            {
                sonuc.DegerAta("Lütfen proje numarasını veriniz!");
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = Oku(ve, kullanan, new IOIProje() { siraNo = ioiSiraNo });
            if (!ov.sonuc.islemSonuc)
            {
                sonuc.DegerAta(ov.sonuc.hataStr);
                ve.VeriErisimKapat();
                return sonuc;
            }
            if (!YazmaYetkisiVarMi(kullanan, (IOIProje)ov.o, false))
            {
                sonuc.DegerAta("Bu işlemi yapma yetkiniz bulunmamaktadır!");
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);
            int say = 1;
            foreach (IOIProjeAktarilan item in liste)
            {
                item.Kontrol(say++, sonuc);
            }

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                foreach (IOIProjeAktarilan item in liste)
                {
                    if (item.aktarmaNo == 0)
                    {
                        if (item.Dolu())
                            sonuc = tabBHMV2IOIPROJE.AktarmaEkle(ioiSiraNo, item);
                    }
                    else
                    {
                        if (item.Dolu())
                            sonuc = tabBHMV2IOIPROJE.AktarmaGuncelle(item);
                        else
                            sonuc = tabBHMV2IOIPROJE.AktarmaSil(item);
                    }
                    if (!sonuc.islemSonuc) break;
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static ObjectVAN PEBBilgi(VeriErisim veriErisim, Kullanici kullanan, int yil, string pebKod)
        {
            ObjectVAN ov = new ObjectVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2IOIPROJE tabBHMV2IOIPROJE = new TABBHMV2IOIPROJE(ve);
            string pebKodYapi = TNS.BKD.TertipYapi.KodYapiBul(yil, 14, false);

            ov.o = null;
            IDataReader r = tabBHMV2IOIPROJE.ListelePeb(yil, pebKod);
            if (r != null)
            {
                if (r.Read())
                {
                    ov.o = ProjeBagOku(r, pebKodYapi);
                }
                r.Close();
                if (ov.o == null)
                    ov.sonuc.DegerAta("Verilen kodun proje bağlantısı bulunamadı!");
            }
            else
                ov.sonuc.DegerAta("Veri tabanı hatası:" + ve.hata);
            ve.VeriErisimKapat();
            return ov;
        }
    }
}
