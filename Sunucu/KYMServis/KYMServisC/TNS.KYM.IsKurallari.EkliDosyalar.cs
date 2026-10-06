using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;
using System.Collections.Generic;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// KYMEkliDosyalar formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KYMEkliDosyalar
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, EkliDosyalar form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (string.IsNullOrWhiteSpace(form.modul)) s.DegerAta("Lütfen modül kodunu giriniz!");

            if (string.IsNullOrWhiteSpace(form.form)) s.DegerAta("Lütfen önce formu kaydediniz!");
            else if (form.form == "-1") s.DegerAta("Lütfen önce formu kaydediniz!");

            if (string.IsNullOrWhiteSpace(form.dosyaAdi)) s.DegerAta("Lütfen dosya adını giriniz!");
            if (form.ekliDosyaIcerik == null || form.ekliDosyaIcerik.Length == 0) s.DegerAta("Lütfen dosyayı gönderiniz!");
            return s;
        }

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form)
        {
            Sonuc s = new Sonuc(true, "", "");

            s = AlanlarGecerliMi(kullanan, form);
            if (s.islemSonuc)
            {
                YetkiKontrol(s, kullanan, null, form, ENUMIslemTuru.KAYIT);
            }
            if (!s.islemSonuc)
            {
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMEKLIDOSYALAR tabKYMEKLIDOSYALAR = new TABKYMEKLIDOSYALAR(ve);

            if (form.dosyaNo > 0)
            {
                s = Sil(ve, kullanan, form, true);
                if (!s.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return s;
                }
            }

            ve.BeginTransaction();
            form.dosyaNo = tabKYMEKLIDOSYALAR.DosyaNoBul(form);
            if (!string.IsNullOrWhiteSpace(ve.hata))
                s.DegerAta(ve.hata);
            if (s.islemSonuc)
            {
                form.buyukluk = form.ekliDosyaIcerik.Length;
                form.yuklemeTarihiDT = DateTime.Now;
                form.yukleyen = kullanan.kullaniciKodu;
                s = tabKYMEKLIDOSYALAR.Ekle(form);
            }
            if (s.islemSonuc)
                s = tabKYMEKLIDOSYALAR.EkleParca(form, 0, form.ekliDosyaIcerik);

            //int yazilan = 0;
            //int parcaNo = 0;
            //int boy = 4096;
            //while (true)
            //{
            //    int icBoy = form.buyukluk - yazilan;
            //    if (icBoy > boy) icBoy = boy;
            //    if (icBoy > 0)
            //    parcaNo++;
            //    yazilan += icBoy;
            //    if (yazilan >= form.buyukluk) break;
            //}

            if (s.islemSonuc)
            {
                s.anahtar = form.dosyaNo.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static void YetkiKontrol(Sonuc s, Kullanici kullanan, string oncekiKaydeden, EkliDosyalar form, ENUMIslemTuru islemTur)
        {
            if (kullanan == null)
                s.DegerAta("Dosya işlemleri için giriş yapmış olmalısınız!");
            else
            {
                string dllAd, nesneAd;
                NesneAdBul(form.modul, out dllAd, out nesneAd);
                if (dllAd != null)
                {
                    try
                    {
                        System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(dllAd, nesneAd);
                        if (oh != null)
                        {
                            TNS.KYM.IYetki rr = (TNS.KYM.IYetki)oh.Unwrap();
                            Sonuc ss = rr.YetkiKontrolu(kullanan, islemTur, oncekiKaydeden, 0, "", form.modul, form.form);
                            if (!ss.islemSonuc)
                                s.DegerAta(ss.hataStr);
                        }
                    }
                    catch
                    {
                        s.DegerAta("Yetki kontrolü yapılamadı!");
                    }
                }
                else if (islemTur == ENUMIslemTuru.SILME || islemTur == ENUMIslemTuru.ONAYKALDIRMA || islemTur == ENUMIslemTuru.ONAYLAMA)
                {
                    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                        s.DegerAta("Bu işlemi yapmak için yetkiniz yok!");
                }
            }
        }

        private static void NesneAdBul(string modul, out string dllAd, out string nesneAd)
        {
            if (modul.Equals("ATMAB"))
            {
                dllAd = "ATMServisC";
                nesneAd = "TNS.ATM.IsKurallari.ATMYetki";
            }
            else if (modul.StartsWith("DB"))
            {
                dllAd = "HRCServisC";
                nesneAd = "TNS.HRC.IsKurallari.HRCYetki";
            }
            else
            {
                dllAd = null;
                nesneAd = null;
            }
        }

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        public static ObjectVAN OkuDosyaIcerik(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form)
        {
            ObjectVAN ov = new ObjectVAN();
            EkliDosyalar ed = new EkliDosyalar();
            ov.o = null;
            ed.ekliDosyaIcerik = null;
            
            YetkiKontrol(ov.sonuc, kullanan, null, form, ENUMIslemTuru.GORME);
            if (!ov.sonuc.islemSonuc) return ov;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));
            TABKYMEKLIDOSYALAR to = new TABKYMEKLIDOSYALAR(ve);
            IDataReader dr = to.ListeleParca(form);

            if (dr != null)
            {
                if (dr.Read())
                {
                    ov.o = ed;
                    ed.dosyaAdi = VeriErisim.DegerAl(dr, 1, "");
                    ed.buyukluk = VeriErisim.DegerAl(dr, 2, 0);
                    if (!dr.IsDBNull(0))
                    {
                        byte[] okunanParca = (byte[])dr.GetValue(0);
                        if (ed.ekliDosyaIcerik == null)
                            ed.ekliDosyaIcerik = okunanParca;
                        else
                        {
                            int tut = ed.ekliDosyaIcerik.Length;
                            System.Array.Resize<byte>(ref ed.ekliDosyaIcerik, tut + okunanParca.Length);
                            okunanParca.CopyTo(ed.ekliDosyaIcerik, tut);
                        }
                    }
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return ov;
        }

        /// <summary>
        /// EkliDosyalar listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form, bool like, bool birimAdOlsun)
        {
            ObjectArray oa = new ObjectArray();
            Dictionary<string, MUH.MuhasebeBirim> birimler = new Dictionary<string, MUH.MuhasebeBirim>();
            if (birimAdOlsun)
            {
                TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                ObjectArray oaBirimler = null;
                oaBirimler = muhServis.MuhasebeBirimListele(new MUH.MuhasebeBirim());
                if (oaBirimler.objeler != null)
                {
                    foreach (MUH.MuhasebeBirim item in oaBirimler.objeler)
                    {
                        birimler[item.MuhasebeBirimKurum] = item;
                    }
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABKYMEKLIDOSYALAR tabKYMEKLIDOSYALAR = new TABKYMEKLIDOSYALAR(ve);

            IDataReader dr = tabKYMEKLIDOSYALAR.Listele(form, like);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                EkliDosyalar detay = new EkliDosyalar();
                detay.modul = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.MODUL, "");
                detay.form = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.FORM, "");
                detay.dosyaNo = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.DOSYANO, 0);
                detay.dosyaAdi = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.DOSYAADI, "");
                detay.buyukluk = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.BUYUKLUK, 0);
                detay.yuklemeTarihi = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.YUKLEMETARIHI);
                detay.yukleyen = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.YUKLEYEN, "");
                detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.ONAYLAYAN, "");
                detay.onayZaman = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.ONAYZAMAN);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABKYMEKLIDOSYALAR.ENUMAlanlar.ACIKLAMA, "");
                if (birimAdOlsun)
                {
                    string[] alanlar = detay.form.Split('_');

                    if (alanlar.Length >= 3 && alanlar[2].StartsWith("B"))
                    {
                        if (birimler.TryGetValue(alanlar[2].Substring(1), out MUH.MuhasebeBirim mb))
                        {
                            detay.birimAd = TNS.MUH.Arac.BirimAdOl(mb.muhasebeAd, mb.birimAd);
                        }
                    }
                }
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static EkliDosyalar Oku(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form)
        {
            ObjectArray oa = Listele(veriErisim, kullanan, form, false, false);
            if (!oa.sonuc.islemSonuc)
                return null;
            if (oa.ObjeSayisi == 1)
                return (EkliDosyalar)oa.objeler[0];
            return null;
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form, bool varsaSil)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));

            EkliDosyalar okunanForm = Oku(veriErisim, kullanan, form);
            if (okunanForm == null)
            {
                if (!varsaSil)
                    s.DegerAta("Silinecek kayıt bulunamadı!");
                else
                    return s;
            }
            else if (!string.IsNullOrWhiteSpace(okunanForm.onaylayan))
            {
                s.DegerAta("Onaylanmış kayıtlar silinemez!");
            }
            else
            {
                TABKYMEKLIDOSYALAR to = new TABKYMEKLIDOSYALAR(ve);
                YetkiKontrol(s, kullanan, okunanForm.yukleyen, okunanForm, ENUMIslemTuru.SILME);
                if (s.islemSonuc)
                {
                    ve.BeginTransaction();
                    s = to.Sil(form);
                    if (s.islemSonuc)
                        s = to.SilParca(form);

                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OnaylaOnayKaldir(VeriErisim veriErisim, Kullanici kullanan, EkliDosyalar form, bool onayla)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(null));

            EkliDosyalar okunanForm = Oku(veriErisim, kullanan, form);
            if (okunanForm == null)
            {
                s.DegerAta("Silinecek kayıt bulunamadı!");
            }
            else
            {
                TABKYMEKLIDOSYALAR to = new TABKYMEKLIDOSYALAR(ve);
                ENUMIslemTuru it;
                if (onayla)
                    it = ENUMIslemTuru.ONAYLAMA;
                else
                    it = ENUMIslemTuru.ONAYKALDIRMA;
                YetkiKontrol(s, kullanan, okunanForm.yukleyen, okunanForm, it);
                if (s.islemSonuc)
                {
                    ve.BeginTransaction();
                    TNSDateTime islemZaman = null;
                    String onaylayan = null;
                    if (onayla)
                    {
                        islemZaman = new TNSDateTime(DateTime.Now);
                        onaylayan = kullanan.kullaniciKodu;
                    }
                    s = to.OnayIslem(form, onaylayan, islemZaman);

                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}
