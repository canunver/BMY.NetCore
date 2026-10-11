using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System.Security.Cryptography;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCDBIkrazlar formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class DBIkrazlar
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";


        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        private static bool YetkisiVarMi(Kullanici kullanan, TNS.HRC.ENUMOGY ogy, string kurKod)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI, (int)ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI))
            {
                string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Dünya Bankası İşlem Yetkisi");
                string[] yetkiTipiYetkiDegerleri = yetkiTipiYetkiDegeri.Split(';');
                bool ilkYetki = false;
                foreach (string item in yetkiTipiYetkiDegerleri)
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(item.Trim(), -1);

                    if (ogy == ENUMOGY.Gorme && yetkiTipiYetkiDegeriInt > 0)
                        ilkYetki = true;
                    if (ogy == ENUMOGY.Yazma && yetkiTipiYetkiDegeriInt > 1)
                        ilkYetki = true;
                    if (ogy == ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                        ilkYetki = true;
                    if (ilkYetki) break;
                }
                if (ilkYetki)
                {
                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI)) return true;

                    if (string.IsNullOrWhiteSpace(kurKod)) return false;

                    yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Dünya Bankası Proje Birimi");
                    ObjectArray d = new ObjectArray();
                    int parcaSay = OrtakFonksiyonlar.StringiParcala(yetkiTipiYetkiDegeri, ";", d);
                    foreach (string yetkiTipiDegeriParca in d.objeler)
                    {
                        if (!string.IsNullOrWhiteSpace(yetkiTipiDegeriParca))
                            if (kurKod.Replace(".", "").StartsWith(yetkiTipiDegeriParca.Replace(".", "")))
                                return true;
                    }

                    return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int altBilesenID)
        {
            return YetkisiVarMi(kullanan, TNS.HRC.ENUMOGY.Yazma, BirimKodBul(kullanan, altBilesenID));
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="altBilesenID">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        internal static bool GormeYetkisiVarMi(Kullanici kullanan, int altBilesenID)
        {
            return YetkisiVarMi(kullanan, TNS.HRC.ENUMOGY.Gorme, BirimKodBul(kullanan, altBilesenID));
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="altBilesenID">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        internal static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int altBilesenID)
        {
            return YetkisiVarMi(kullanan, TNS.HRC.ENUMOGY.Onaylama, BirimKodBul(kullanan, altBilesenID));
        }

        private static Dictionary<int, string> bilesenKurum = new Dictionary<int, string>();

        private static string BirimKodBul(Kullanici kullanan, int altBilesenID)
        {
            if (altBilesenID <= 0) return "";
            if (bilesenKurum.TryGetValue(altBilesenID, out string kurumKod))
            {
                return kurumKod;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            kurumKod = ve.TekKolonSQLStr($"SELECT ILGILIBIRIMBUTCEKODU FROM DBALTBILESEN WHERE ALTBILESENID = {altBilesenID}");
            bilesenKurum[altBilesenID] = kurumKod;

            return kurumKod;
        }

        /// <summary>
        /// DBIkraz nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetDBIkraz(VeriErisim veriErisim, Kullanici kullanan, DBIkraz form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta("İkraz bilgisi değiştirme yetkiniz bulunmamaktadır!");
                return s;
            }

            form.Kontrol(s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBIKRAZ = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.ikrazID == 0)
                {
                    form.ikrazID = tabHRCDBIKRAZ.EkleIkraz(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = tabHRCDBIKRAZ.GuncelleIkraz(form);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.ikrazID.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// DBIkraz nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuDBIkraz(VeriErisim veriErisim, Kullanici kullanan, DBIkraz form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleDBIkrazlar(veriErisim, kullanan, form, true);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// DBIkraz listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleDBIkrazlar(VeriErisim veriErisim, Kullanici kullanan, DBIkraz form, bool detayOku)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = tabHRCDBIKRAZ.ListeleIkrazlar(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                DBIkraz detay = new DBIkraz();
                detay.ikrazID = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.IKRAZID, 0);
                detay.anlasmaAdiTr = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAADITR, "");
                detay.anlasmaAdiEn = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAADIEN, "");
                detay.anlasmaKodu = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAKODU, "");
                detay.baslangicYili = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.BASLANGICYILI, 0);
                detay.bitisYili = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.BITISYILI, 0);
                detay.paraBirimi = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.PARABIRIMI, "");
                detay.donusumKuru = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.DONUSUMKURU, 0.0);
                detay.ikrazNo = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.IKRAZNO, "");
                detay.mbHesapNo = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.MBHESAPNO, "");
                oa.Ekle(detay);
            }
            dr.Close();
            if (detayOku)
            {
                foreach (DBIkraz item in oa.objeler)
                {
                    item.bilesenler = ListeleDBBilesenler(ve, kullanan, new DBBilesen() { ikrazID = item.ikrazID }).objeler;
                    item.altBilesenler = ListeleDBAltBilesenler(ve, kullanan, new DBAltBilesen() { ikrazID = item.ikrazID }).objeler;
                }
            }

            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// DBBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetDBBilesen(VeriErisim veriErisim, Kullanici kullanan, DBBilesen form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta("Yetkiniz bulunmamaktadır!");
                return s;
            }

            form.Kontrol(s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBBILESEN = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.bilesenID == 0)
                {
                    form.bilesenID = tabHRCDBBILESEN.EkleBilesen(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = tabHRCDBBILESEN.GuncelleBilesen(form);
            }

            if (s.islemSonuc)
            {
                s.anahtar = form.bilesenID.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// DBBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuDBBilesen(VeriErisim veriErisim, Kullanici kullanan, DBBilesen form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleDBBilesenler(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// DBBilesen listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleDBBilesenler(VeriErisim veriErisim, Kullanici kullanan, DBBilesen form)
        {
            ObjectArray oa = new ObjectArray();
            if (form.ikrazID == 0 && form.bilesenID == 0)
            {
                oa.sonuc.DegerAta("Listele için ikraz id veya bileşen id veriniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBBILESEN = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = tabHRCDBBILESEN.ListeleBilesenler(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                DBBilesen detay = new DBBilesen();

                detay.ikraz.ikrazID = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.IKRAZID, 0);
                detay.ikraz.anlasmaAdiTr = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAADITR, "");
                detay.ikraz.anlasmaAdiEn = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAADIEN, "");
                detay.ikraz.anlasmaKodu = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.ANLASMAKODU, "");
                detay.ikraz.baslangicYili = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.BASLANGICYILI, 0);
                detay.ikraz.bitisYili = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.BITISYILI, 0);
                detay.ikraz.paraBirimi = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.PARABIRIMI, "");
                detay.ikraz.donusumKuru = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.DONUSUMKURU, 0.0);
                detay.ikraz.ikrazNo = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.IKRAZNO, "");
                detay.ikraz.mbHesapNo = VeriErisim.DegerAl(dr, (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.MBHESAPNO, "");

                int yer = (int)TABDBIKRAZ.ENUMAlanlarIKRAZ.MBHESAPNO + 1;
                detay.bilesenID = VeriErisim.DegerAl(dr, yer + (int)TABDBIKRAZ.ENUMAlanlarBILESEN.BILESENID, 0);
                detay.bilesenNo = VeriErisim.DegerAl(dr, yer + (int)TABDBIKRAZ.ENUMAlanlarBILESEN.BILESENNO, "");
                detay.bilesenAd = VeriErisim.DegerAl(dr, yer + (int)TABDBIKRAZ.ENUMAlanlarBILESEN.BILESENAD, "");
                detay.toplamButce = VeriErisim.DegerAl(dr, yer + (int)TABDBIKRAZ.ENUMAlanlarBILESEN.BILESENAD + 1, 0.0);
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetDBAltBilesen(VeriErisim veriErisim, Kullanici kullanan, DBAltBilesen form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta("Kaydetme yetkiniz bulunmamaktadır!");
                return s;
            }

            form.Kontrol(s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.altBilesenID == 0)
                {
                    form.altBilesenID = TABDBIKRAZ.EkleAltBilesen(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = TABDBIKRAZ.GuncelleAltBilesen(form);
            }
            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.altBilesenID.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuDBAltBilesen(VeriErisim veriErisim, Kullanici kullanan, DBAltBilesen form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleDBAltBilesenler(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// DBAltBilesen listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleDBAltBilesenler(VeriErisim veriErisim, Kullanici kullanan, DBAltBilesen form)
        {
            ObjectArray oa = new ObjectArray();
            if (form.ikrazID == 0 && form.altBilesenID == 0)
            {
                oa.sonuc.DegerAta("Listele için ikraz id veya alt bileşen id veriniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.ListeleAltBilesenler(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBAltBilesen detay = new DBAltBilesen();
                int kol = 0;
                detay.bilesen.ikraz.ikrazID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesen.ikraz.anlasmaAdiTr = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.ikraz.anlasmaAdiEn = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.ikraz.anlasmaKodu = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.ikraz.baslangicYili = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesen.ikraz.bitisYili = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesen.ikraz.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.ikraz.donusumKuru = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.bilesen.ikraz.ikrazNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.ikraz.mbHesapNo = VeriErisim.DegerAl(dr, kol++, "");

                detay.bilesen.bilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                kol++; //ikraz id okumaya gerek yok
                detay.bilesen.bilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesen.bilesenAd = VeriErisim.DegerAl(dr, kol++, "");

                detay.altBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliBirimNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliBirimAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliBirimButceKodu = VeriErisim.DegerAl(dr, kol++, "");
                detay.toplamButce = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.bagYil = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ilgiliPEBKod = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliBirimButceAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliPEBAd = VeriErisim.DegerAl(dr, kol++, "");

                detay.ilgiliBirimButceKodu = OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(detay.bagYil, 1, false), detay.ilgiliBirimButceKodu);

                detay.ilgiliPEBKod = OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(detay.bagYil, 14, false), detay.ilgiliPEBKod);

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc KaydetButceRef(VeriErisim veriErisim, Kullanici kullanan, DBButceRef form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form.altBilesenID))
            {
                s.DegerAta("Alt bileşen verilemeden kayıt yapılamaz veya alt bileşenle ilgili işlem yapmak yetkiniz bulunmamaktadır!");
                return s;
            }

            List<int> silinecekler = new List<int>();
            for (int i = form.lotlar.Count - 1; i >= 0; i--)
            {
                DBButceRefLot simdikiLot = (DBButceRefLot)form.lotlar[i];
                if (string.IsNullOrWhiteSpace(simdikiLot.aciklama))
                {
                    if (simdikiLot.butceRefLotID > 0) silinecekler.Add(simdikiLot.butceRefLotID);
                    form.lotlar.RemoveAt(i);
                }
            }

            int buyukLot = form.lotlar.Count;
            for (int i = 0; i < form.lotlar.Count; i++)
            {
                DBButceRefLot simdikiLot = (DBButceRefLot)form.lotlar[i];
                if (simdikiLot.lotNo <= 0) s.DegerAta($"Gelen lot numarası ({simdikiLot.lotNo}) sıfırdan büyük olmalı!");
                else if (simdikiLot.lotNo > buyukLot) s.DegerAta($"Gelen lot numarası ({simdikiLot.lotNo}) en fazla {buyukLot} olabilir!");
                for (int j = i + 1; j < form.lotlar.Count; j++)
                {
                    DBButceRefLot kontrolLot = (DBButceRefLot)form.lotlar[j];
                    if (simdikiLot.lotNo == kontrolLot.lotNo) s.DegerAta($"Gelen lot numarası ({simdikiLot.lotNo}) iki kez kullanılmış!");
                }
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBBUTCEREF = new TABDBIKRAZ(ve);
            DBIkraz eskiIkraz = null;
            DBAltBilesen eskiAltbilesen = null;
            DBButceRef eskiForm = null;

            if (form.butceRefID > 0)
            {
                ObjectVAN ov = OkuButceRef(ve, kullanan, new DBButceRef() { butceRefID = form.butceRefID });
                if (ov == null)
                    s.DegerAta("Eski form okunamadı!");
                else if (!ov.sonuc.islemSonuc)
                    s.DegerAta("Eski form okunamadı:" + ov.sonuc.hataStr);
                else if (ov.o == null)
                    s.DegerAta("Eski form boş geldi:");
                else
                {
                    eskiIkraz = (DBIkraz)ov.o;
                    eskiAltbilesen = eskiIkraz.AltBilesenBul();
                    eskiForm = eskiAltbilesen.ButceRefBul();
                    if (eskiForm.durum > 1)
                        s.DegerAta("Yalnızca yeni durumdaki formlar üzerinde değişiklik yapılabilir!");
                    if (!YazmaYetkisiVarMi(kullanan, eskiAltbilesen.altBilesenID))
                        s.DegerAta("Bu birimle ilgili kayıt yapma yetkiniz bulunmamaktadır!");
                }
            }
            else
                eskiIkraz = null;

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (eskiForm == null)
            {
                int yeniId = tabHRCDBBUTCEREF.EkleButceRef(form);
                if (!string.IsNullOrWhiteSpace(ve.hata))
                {
                    s.DegerAta("Kayıt yapılamadı:" + ve.hata);
                }
                else
                    s.anahtar = yeniId.ToString();
                form.butceRefID = yeniId;
            }
            else
            {
                s = tabHRCDBBUTCEREF.GuncelleButceRef(form);
                s.anahtar = eskiForm.butceRefID.ToString();
            }
            if (s.islemSonuc)
            {
                foreach (int item in silinecekler)
                {
                    s = tabHRCDBBUTCEREF.SilButceRefLot(item);
                    if (!s.islemSonuc)
                        break;
                }
            }
            if (s.islemSonuc)
            {
                foreach (DBButceRefLot item in form.lotlar)
                {
                    item.butceRefID = form.butceRefID;
                    if (item.butceRefLotID == 0)
                    {
                        int yeniId = tabHRCDBBUTCEREF.EkleButceRefLot(item);
                        if (!string.IsNullOrWhiteSpace(ve.hata))
                        {
                            s.DegerAta("Kayıt yapılamadı:" + ve.hata);
                        }
                        else
                            item.butceRefLotID = yeniId;
                    }
                    else
                        s = tabHRCDBBUTCEREF.GuncelleButceRefLot(item);
                    if (!s.islemSonuc)
                        break;
                }
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc SilDBButceRef(VeriErisim veriErisim, Kullanici kullanan, int butceRefID)
        {
            Sonuc s = new Sonuc();
            if (butceRefID <= 0)
            {
                s.DegerAta("Numara alacak sözleşme bilgisi hatalı!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            DBButceRef butce = new DBButceRef();
            butce.butceRefID = butceRefID;

            ObjectVAN ov = OkuButceRef(ve, kullanan, butce);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            if (ov.o == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Step Verisi bulunamadı!");
            }

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;

            //SÖZLEŞME KONTROLÜ
            dr = TABDBIKRAZ.ButceSozlesmeBul(butceRefID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc(ve.hata);
            }

            int sozlesmeSayisi = 0;
            if (dr.Read())
            {
                sozlesmeSayisi = VeriErisim.DegerAl(dr, 0, 0);
            }
            dr.Close();

            if (sozlesmeSayisi > 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Bu bütçeye bağlı sözleşmeler bulunmaktadır. Önce sözleşmeleri silmelisiniz!");
            }

            //HAKEDİŞ KONTROLÜ
            dr = TABDBIKRAZ.ButceHakedisBul(butceRefID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc(ve.hata);
            }

            int hakedisSayisi = 0;
            if (dr.Read())
            {
                hakedisSayisi = VeriErisim.DegerAl(dr, 0, 0);
            }
            dr.Close();

            if (hakedisSayisi > 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Bu bütçeye bağlı hakedişler bulunmaktadır. Önce hakedişleri silmelisiniz!");
            }

            try
            {
                ve.BeginTransaction();

                // Önce bağlı lotları sil
                s = TABDBIKRAZ.SilButceRefLot(butceRefID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }

                // Sonra bütçeyi sil
                s = TABDBIKRAZ.SilButceRef(butceRefID);

                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                s.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return s;
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        public static ObjectVAN OkuButceRef(VeriErisim veriErisim, Kullanici kullanan, DBButceRef form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleButceRef(veriErisim, kullanan, form, true);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        /// <summary>
        /// DBButceRef listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleButceRef(VeriErisim veriErisim, Kullanici kullanan, DBButceRef form, bool lotOku)
        {
            ObjectArray oa = new ObjectArray();

            if (form.butceRefID == 0)
            {
                if (!GormeYetkisiVarMi(kullanan, form.altBilesenID))
                {
                    oa.sonuc.DegerAta("Listeleyebilmek için alt bileşen seçiniz!");
                    return oa;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBBUTCEREF = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = tabHRCDBBUTCEREF.ListeleButceRef(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            DBIkraz oncDBIkraz = null;
            DBBilesen oncBilesen = null;
            DBAltBilesen oncAltBilesen = null;
            DBSatinAlma oncSatinAlma = null;
            TNSCollection refler = new TNSCollection();
            while (dr.Read())
            {
                int kol = 0;
                int icIkrazID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncDBIkraz == null || oncDBIkraz.ikrazID != icIkrazID)
                {
                    oncDBIkraz = new DBIkraz();
                    oncDBIkraz.ikrazID = icIkrazID;
                    oncDBIkraz.anlasmaAdiTr = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.anlasmaAdiEn = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.anlasmaKodu = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.baslangicYili = VeriErisim.DegerAl(dr, kol++, 0);
                    oncDBIkraz.bitisYili = VeriErisim.DegerAl(dr, kol++, 0);
                    oncDBIkraz.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.donusumKuru = VeriErisim.DegerAl(dr, kol++, 0.0);
                    oncDBIkraz.ikrazNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.mbHesapNo = VeriErisim.DegerAl(dr, kol++, "");
                    oa.Ekle(oncDBIkraz);
                    oncBilesen = null;
                }
                else
                    kol += 9;

                int icbilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncBilesen == null || oncBilesen.bilesenID != icbilesenID)
                {
                    oncBilesen = new DBBilesen();
                    oncBilesen.bilesenID = icbilesenID;
                    kol++;//ikrazid'ye gerek yok
                    oncBilesen.bilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncBilesen.bilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.bilesenler.Add(oncBilesen);
                    oncAltBilesen = null;
                }
                else
                    kol += 3;

                int icaltBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncAltBilesen == null || oncAltBilesen.altBilesenID != icaltBilesenID)
                {
                    oncAltBilesen = new DBAltBilesen();
                    oncAltBilesen.altBilesenID = icaltBilesenID;
                    kol++; //bileşen id'ye gerek yok
                    oncAltBilesen.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.altBilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimAdi = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimButceKodu = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.toplamButce = VeriErisim.DegerAl(dr, kol++, 0.0);
                    oncAltBilesen.bagYil = VeriErisim.DegerAl(dr, kol++, 0);
                    oncAltBilesen.ilgiliPEBKod = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimButceAdi = VeriErisim.DegerAl(dr, kol++, "");
                    oncBilesen.altBilesenler.Add(oncAltBilesen);

                    if (form.butceRefID > 0)
                    {
                        if (!GormeYetkisiVarMi(kullanan, oncAltBilesen.altBilesenID))
                        {
                            oa.sonuc.DegerAta("Bu alt bileşeni görme yetkiniz bulunmamaktadır!");
                            break;
                        }
                    }
                }
                else
                    kol += 10;

                int icSatinAlmaID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncSatinAlma == null || oncSatinAlma.satinAlmaID != icSatinAlmaID)
                {
                    oncSatinAlma = new DBSatinAlma();
                    oncSatinAlma.satinAlmaID = icSatinAlmaID;
                    kol++; //altBilesenID'ye gerek yok
                    oncSatinAlma.satinAlmaNo = VeriErisim.DegerAl(dr, kol++, 0);
                    oncSatinAlma.satinAlmaGrupNo = VeriErisim.DegerAl(dr, kol++, 0);
                    oncSatinAlma.aciklamaTr = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.aciklamaEn = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.referansNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.referansGrupEk = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.referansEk = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.butceKategori = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.satinAlmaYontemi = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.secimYontemi = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.onceSonra = VeriErisim.DegerAl(dr, kol++, 0);
                    oncSatinAlma.tahminiDavetTarihi = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.planlananImzaTarihi = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.planlananBitisTarihi = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.altBirimAd = VeriErisim.DegerAl(dr, kol++, "");
                    oncSatinAlma.odenek = 0;
                    SatinAlmaDoldur(oncSatinAlma, VeriErisim.DegerAl(dr, kol++, ""), oncDBIkraz.donusumKuru);
                    oncAltBilesen.satinAlmaPlani.Add(oncSatinAlma);
                }
                else
                    kol += 17;

                DBButceRef detay = new DBButceRef();
                detay.butceRefID = VeriErisim.DegerAl(dr, kol++, 0);
                kol++; //satinAlmaID'ye gerek yok
                detay.referansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.aciklamaTr = VeriErisim.DegerAl(dr, kol++, "");
                detay.aciklamaEn = VeriErisim.DegerAl(dr, kol++, "");
                detay.odenek = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.durum = VeriErisim.DegerAl(dr, kol++, 0);
                oncSatinAlma.butceRefler.Add(detay);
                refler.Add(detay);
            }
            dr.Close();
            foreach (DBButceRef item in refler)
            {
                item.lotlar.AddRange(ListeleButceRefLot(ve, kullanan, item.butceRefID).objeler);
            }

            ve.VeriErisimKapat();
            return oa;
        }

        /// <summary>
        /// DBButceRef listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        public static ObjectArray ListeleButceRefLot(VeriErisim veriErisim, Kullanici kullanan, int butceRefID)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBBUTCEREF = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = tabHRCDBBUTCEREF.ListeleButceRefLot(butceRefID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBButceRefLot l = new DBButceRefLot();
                int kol = 0;
                l.butceRefLotID = VeriErisim.DegerAl(dr, kol++, 0);
                l.butceRefID = VeriErisim.DegerAl(dr, kol++, 0);
                l.lotNo = VeriErisim.DegerAl(dr, kol++, 0);
                l.aciklama = VeriErisim.DegerAl(dr, kol++, "");
                oa.objeler.Add(l);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleSatinAlma(VeriErisim veriErisim, Kullanici kullanan, DBSatinAlma satinAlma, int siralama = 1)
        {
            ObjectArray oa = new ObjectArray();

            if (satinAlma.satinAlmaID == 0)
            {
                if (!GormeYetkisiVarMi(kullanan, satinAlma.altBilesenID))
                {
                    oa.sonuc.DegerAta("Listeleyebilmek için alt bileşen seçiniz!");
                    return oa;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tab = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = tab.ListeleSatinAlma(satinAlma, siralama);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            DBIkraz oncDBIkraz = null;
            DBBilesen oncBilesen = null;
            DBAltBilesen oncAltBilesen = null;
            while (dr.Read())
            {
                int kol = 0;
                int icIkrazID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncDBIkraz == null || oncDBIkraz.ikrazID != icIkrazID)
                {
                    oncDBIkraz = new DBIkraz();
                    oncDBIkraz.ikrazID = icIkrazID;
                    oncDBIkraz.anlasmaAdiTr = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.anlasmaAdiEn = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.anlasmaKodu = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.baslangicYili = VeriErisim.DegerAl(dr, kol++, 0);
                    oncDBIkraz.bitisYili = VeriErisim.DegerAl(dr, kol++, 0);
                    oncDBIkraz.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.donusumKuru = VeriErisim.DegerAl(dr, kol++, 0.0);
                    oncDBIkraz.ikrazNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.mbHesapNo = VeriErisim.DegerAl(dr, kol++, "");

                    oa.Ekle(oncDBIkraz);
                    oncBilesen = null;
                }
                else
                    kol += 9;

                int icbilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncBilesen == null || oncBilesen.bilesenID != icbilesenID)
                {
                    oncBilesen = new DBBilesen();
                    oncBilesen.bilesenID = icbilesenID;
                    kol++;//ikrazid'ye gerek yok
                    oncBilesen.bilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncBilesen.bilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                    oncDBIkraz.bilesenler.Add(oncBilesen);
                    oncAltBilesen = null;
                }
                else
                    kol += 3;

                int icaltBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                if (oncAltBilesen == null || oncAltBilesen.altBilesenID != icaltBilesenID)
                {
                    oncAltBilesen = new DBAltBilesen();
                    oncAltBilesen.altBilesenID = icaltBilesenID;
                    kol++; //bileşen id'ye gerek yok
                    oncAltBilesen.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.altBilesenAd = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimNo = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimAdi = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.ilgiliBirimButceKodu = VeriErisim.DegerAl(dr, kol++, "");
                    oncAltBilesen.toplamButce = VeriErisim.DegerAl(dr, kol++, 0.0);
                    oncAltBilesen.bagYil = VeriErisim.DegerAl(dr, kol++, 0);
                    oncAltBilesen.ilgiliPEBKod = VeriErisim.DegerAl(dr, kol++, "");
                    oncBilesen.altBilesenler.Add(oncAltBilesen);
                }
                else
                    kol += 9;
                DBSatinAlma detay = new DBSatinAlma();
                detay.satinAlmaID = VeriErisim.DegerAl(dr, kol++, 0);
                kol++; //altBilesenID'ye gerek yok
                detay.satinAlmaNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.satinAlmaGrupNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.aciklamaTr = VeriErisim.DegerAl(dr, kol++, "");
                detay.aciklamaEn = VeriErisim.DegerAl(dr, kol++, "");
                detay.referansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.referansGrupEk = VeriErisim.DegerAl(dr, kol++, "");
                detay.referansEk = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceKategori = VeriErisim.DegerAl(dr, kol++, "");

                detay.satinAlmaYontemi = VeriErisim.DegerAl(dr, kol++, "");
                detay.secimYontemi = VeriErisim.DegerAl(dr, kol++, "");
                detay.onceSonra = VeriErisim.DegerAl(dr, kol++, 0);


                detay.tahminiDavetTarihi = VeriErisim.DegerAl(dr, kol++, "");
                detay.planlananImzaTarihi = VeriErisim.DegerAl(dr, kol++, "");
                detay.planlananBitisTarihi = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBirimAd = VeriErisim.DegerAl(dr, kol++, "");
                SatinAlmaDoldur(detay, VeriErisim.DegerAl(dr, kol++, ""), oncDBIkraz.donusumKuru);

                if (satinAlma.satinAlmaID > 0)
                {
                    if (!GormeYetkisiVarMi(kullanan, oncAltBilesen.altBilesenID))
                    {
                        oa.sonuc.DegerAta("Listeleyebilmek için alt bileşen seçiniz!");
                        return oa;
                    }
                }

                oncAltBilesen.satinAlmaPlani.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        private static void SatinAlmaDoldur(DBSatinAlma detay, string xml, double donusumKuru)
        {
            double toplam = 0;

            XMLStringDataReader r = new XMLStringDataReader(xml);

            if (r != null)
            {
                DBSatinAlmaGerceklesmePlan gercPlan = null;
                while (r.Read())
                {
                    int kolIc = 0;
                    int dagilimId = VeriErisim.DegerAl(r, kolIc++, 0);
                    if (gercPlan == null || gercPlan.dagilimID != dagilimId)
                    {
                        gercPlan = new DBSatinAlmaGerceklesmePlan();
                        gercPlan.dagilimID = dagilimId;
                        gercPlan.planTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                        gercPlan.durum = VeriErisim.DegerAl(r, kolIc++, 0);
                        gercPlan.aciklama = VeriErisim.DegerAl(r, kolIc++, "");
                        detay.gerceklesmePlanlar.Add(gercPlan);
                    }
                    else
                        kolIc += 3;
                    DBOdemePlaniDetay dty = new DBOdemePlaniDetay(dagilimId,
                        VeriErisim.DegerAl(r, kolIc++, 0),
                        VeriErisim.DegerAl(r, kolIc++, 0),
                        VeriErisim.DegerAl(r, kolIc++, 0.0));

                    dty.odenecekTutarUSD = dty.odenecekTutar;
                    dty.odenecekTutarEUR = TNS.HRC.Arac.ParaCevir(dty.odenecekTutar, donusumKuru, false);

                    gercPlan.toplam += dty.odenecekTutar;
                    gercPlan.detaylar.Add(dty);
                }
                if (gercPlan != null)
                {
                    detay.odenek = gercPlan.toplam;
                    detay.odenekEUR = TNS.HRC.Arac.ParaCevir(detay.odenek, donusumKuru, false);
                }
                r.Close();
            }
        }

        internal static ObjectVAN OkuSatinAlma(VeriErisim veriErisim, Kullanici kullanan, DBSatinAlma satinAlma)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = ListeleSatinAlma(veriErisim, kullanan, satinAlma);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        internal static Sonuc KaydetSatinAlma(VeriErisim veriErisim, Kullanici kullanan, DBSatinAlma satinAlma)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, satinAlma.altBilesenID))
            {
                s.DegerAta("Verilen alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (satinAlma.satinAlmaID == 0)
                {
                    satinAlma.satinAlmaID = TABDBIKRAZ.EkleSatinAlma(satinAlma);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = TABDBIKRAZ.GuncelleSatinAlma(satinAlma);
            }
            if (s.islemSonuc)
                s = tabTarihce.Ekle("DBSAP", satinAlma.satinAlmaID.ToString(), kullanan.kullaniciKodu, "Satın Alma Planı Kaydedildi", "");

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = satinAlma.altBilesenID.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc KaydetDBSatinAlmaGerceklesmePlani(VeriErisim veriErisim, Kullanici kullanan, DBSatinAlmaGerceklesmePlan gercPlan)
        {
            Sonuc s = new Sonuc();

            if (gercPlan == null || gercPlan.satinAlmaID <= 0)
            {
                s.DegerAta("Satın Alma verisi verilmelidir.");
            }
            else
            {
                if (TNSDateTime.IsNull(gercPlan.planTarihi)) s.DegerAta("Tarih alanı zorunludur!");
                if (string.IsNullOrWhiteSpace(gercPlan.aciklama)) s.DegerAta("Açıklama alanı zorunludur!");
                int satir = 1;
                foreach (DBOdemePlaniDetay det in gercPlan.detaylar)
                {
                    if (det.BosMu()) continue;
                    if (det.yil <= 0 || det.ay <= 0 || det.odenecekTutar < 0.001)
                    {
                        s.DegerAta($"{satir}. satırda yıl, ay veya ödenecek tutar hatalı girilmiş!");
                    }
                    satir++;
                }
            }

            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuSatinAlma(ve, kullanan, new DBSatinAlma() { satinAlmaID = gercPlan.satinAlmaID });

            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            DBSatinAlma satinAlma = null;
            DBAltBilesen altBilesen = null;
            try
            {
                DBIkraz ikraz = (DBIkraz)ov.o;
                altBilesen = (DBAltBilesen)((DBBilesen)ikraz.bilesenler[0]).altBilesenler[0];
                satinAlma = (DBSatinAlma)altBilesen.satinAlmaPlani[0];

                if (gercPlan.dagilimID > 0)
                {
                    DBSatinAlmaGerceklesmePlan gerceklesmePlan = satinAlma.GerceklesmePlanBul(gercPlan.dagilimID);
                    if (gerceklesmePlan == null)
                        s.DegerAta("Satın alma planı harcama dağılımı bulunamadı!");
                    else if (gerceklesmePlan.durum != 1)
                        s.DegerAta("Satın alma planı yeni durumunda olmadığından değiştirilemez!");
                }
            }
            catch
            {
                s.DegerAta("Satın alma planı okunamadı!");
            }

            if (s.islemSonuc && !YazmaYetkisiVarMi(kullanan, altBilesen.altBilesenID))
            {
                s.DegerAta("Verilen alt bileşenle ilgili işlem yapma yetkiniz bulunmamaktadır!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (gercPlan.dagilimID == 0)
                {
                    gercPlan.dagilimID = TABDBIKRAZ.EkleSatinAlmaGerceklesmePlani(gercPlan);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                {
                    s = TABDBIKRAZ.GuncelleSatinAlmaGerceklesmePlani(gercPlan);
                    if (s.islemSonuc)
                    {
                        s = TABDBIKRAZ.SilSatinAlmaGerceklesmePlaniDetay(gercPlan);
                    }
                }
            }

            if (s.islemSonuc)
            {
                foreach (DBOdemePlaniDetay detay in gercPlan.detaylarTanimli)
                {
                    if (!s.islemSonuc) break;

                    detay.odemePlanID = gercPlan.dagilimID;
                    if (detay.odenecekTutar > 0)
                        s = TABDBIKRAZ.EkleSatinAlmaGerceklesmePlaniDetay(detay);
                }
            }
            if (s.islemSonuc)
                s = tabTarihce.Ekle("DBSAP", gercPlan.satinAlmaID.ToString(), kullanan.kullaniciKodu, "Gerçekleşme Planı Kaydedildi", "");

            if (s.islemSonuc)
            {
                ve.Commit();
                s.ekNesne = gercPlan;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }


        internal static Sonuc DurumDegistirDBSatinAlmaGerceklesmePlani(VeriErisim veriErisim, Kullanici kullanan, int satinAlmaID, int dagilimID, int durumKod)
        {
            Sonuc s = new Sonuc();

            if (satinAlmaID <= 0)
            {
                s.DegerAta("Satın Alma verisi verilmelidir.");
            }
            if (dagilimID <= 0)
            {
                s.DegerAta("Dağılım kodu verilmelidir.");
            }

            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN ov = OkuSatinAlma(ve, kullanan, new DBSatinAlma() { satinAlmaID = satinAlmaID });

            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            DBSatinAlma satinAlma = null;
            DBAltBilesen altBilesen = null;
            try
            {
                DBIkraz ikraz = (DBIkraz)ov.o;
                altBilesen = (DBAltBilesen)((DBBilesen)ikraz.bilesenler[0]).altBilesenler[0];
                satinAlma = (DBSatinAlma)altBilesen.satinAlmaPlani[0];
                DBSatinAlmaGerceklesmePlan gerceklesmePlan = satinAlma.GerceklesmePlanBul(dagilimID);
                if (gerceklesmePlan == null)
                    s.DegerAta("Satın alma planı harcama dağılımı bulunamadı!");
            }
            catch
            {
                s.DegerAta("Satın alma planı okunamadı!");
            }

            if (!s.islemSonuc)
            {
                int yetkiBirimKodu = altBilesen.altBilesenID;
                if (durumKod != 9 && durumKod != 2)
                    yetkiBirimKodu = 0;
                if (s.islemSonuc && !OnaylamaYetkisiVarMi(kullanan, altBilesen.altBilesenID))
                {
                    s.DegerAta("Verilen alt bileşenle ilgili onaylama yetkiniz bulunmamaktadır!");
                }
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            string islemAd;
            if (durumKod == 1)
                islemAd = "İade";
            else if (durumKod == 2)
                islemAd = "Onaya Gönderildi";
            else if (durumKod == 5)
                islemAd = "Onaylandı";
            else
                islemAd = "İptal Edildi";

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                s = TABDBIKRAZ.GuncelleSatinAlmaGerceklesmePlani(dagilimID, durumKod);
                if (s.islemSonuc)
                {
                    s = tabTarihce.Ekle("DBSAP", satinAlmaID.ToString(), kullanan.kullaniciKodu, islemAd, "");
                }
            }

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc SilDBSatinAlmaPlani(VeriErisim veriErisim, Kullanici kullanan, int satinAlmaID)
        {
            Sonuc s = new Sonuc();
            if (satinAlmaID <= 0)
            {
                s.DegerAta("Geçersiz Satın Alma Planı!");
                return s;
            }

            ObjectVAN ov = OkuButceRef(veriErisim, kullanan, new DBButceRef() { satinAlmaID = satinAlmaID });

            if (!ov.sonuc.islemSonuc)
            {
                return ov.sonuc;
            }

            if (ov.o != null)
            {
                return new Sonuc("İlk önce bu satın alma planına bağlı step verilerini silmelisiniz!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);
            try
            {
                ve.BeginTransaction();
                s = TABDBIKRAZ.SilSatinAlmaPlaniDagilimDetay(satinAlmaID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }
                s = TABDBIKRAZ.SilSatinAlmaPlaniDagilim(satinAlmaID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }
                s = TABDBIKRAZ.SilSatinAlmaPlani(satinAlmaID);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                s.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static ObjectArray ListeleDBDosyaBilgileri(VeriErisim veriErisim, Kullanici kullanan, int kod, string modulKod)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.ListeleDBDosyaBilgileri(kod, modulKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }


            while (dr.Read())
            {
                DBYuklenebilecekDosyalar detay = new DBYuklenebilecekDosyalar();
                int kol = 0;
                detay.kod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.dosyaAd = VeriErisim.DegerAl(dr, kol++, "");
                detay.modulKod = VeriErisim.DegerAl(dr, kol++, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc AlanlarGecerliMiDosya(Kullanici kullanan, DBYuklenebilecekDosyalar form, Sonuc s)
        {
            form.Kontrol(s);
            return s;
        }

        internal static Sonuc KaydetDBDosyaBilgisi(VeriErisim veriErisim, Kullanici kullanan, DBYuklenebilecekDosyalar form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta("İkraz bilgisi değiştirme yetkiniz bulunmamaktadır!");
                return s;
            }

            s = AlanlarGecerliMiDosya(kullanan, form, s);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ tabHRCDBIKRAZ = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.kod == 0)
                {
                    form.kod = tabHRCDBIKRAZ.EkleDosyaBilgi(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = tabHRCDBIKRAZ.GuncelleDosyaBilgi(form);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.kod.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray ListeleDBSozlesmeler(VeriErisim veriErisim, Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1)
        {
            ObjectArray oa = new ObjectArray();

            if (sozlesme.sozlesmeID <= 0 && !GormeYetkisiVarMi(kullanan, sozlesme.altBilesenID))
            {
                oa.sonuc.DegerAta("Verilen alt bileşeni görme yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MerkezBankasiKurSinif mbForm = new MerkezBankasiKurSinif() { yakinGunBul = true };
            if (TNSDateTime.IsNull(sozlesme.kayitTarih))
                mbForm.kurTarih = new TNSDateTime(DateTime.Now.Date);
            else
                mbForm.kurTarih = sozlesme.kayitTarih;
            mbForm.satirlar = new HRC.MerkezBankasiKurDetaySinif[2];
            mbForm.satirlar[0] = MerkezBankasiKurDetaySinif.Olustur("EUR");
            mbForm.satirlar[1] = MerkezBankasiKurDetaySinif.Olustur("USD");
            MerkezBankasiKurSinif sonDegerler = HRCMerkezBankKur.Oku(null, kullanan, mbForm);

            decimal kurUSD = sonDegerler.KurBul("USD", "DOVIZSATIS", 0);
            decimal kurEUR = sonDegerler.KurBul("EUR", "DOVIZSATIS", 0);

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.ListeleDBSozlesmeler(sozlesme, siralama);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBSozlesme detay = new DBSozlesme();
                int kol = 0;
                detay.sozlesmeID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.butceRefID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.sozlesmeNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.konu = VeriErisim.DegerAl(dr, kol++, "");
                detay.aciklama = VeriErisim.DegerAl(dr, kol++, "");
                detay.ilgiliKod = VeriErisim.DegerAl(dr, kol++, 0);
                detay.sozlesmeTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.avansHukmu = VeriErisim.DegerAl(dr, kol++, 0);
                detay.avansOrani = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.avansMahsubu = VeriErisim.DegerAl(dr, kol++, 0);
                detay.avansMahsubuOrani = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.avansTeminatMektubu = VeriErisim.DegerAl(dr, kol++, "");
                detay.kesinTeminatMektubu = VeriErisim.DegerAl(dr, kol++, "");
                detay.ihtiyatKesintisiIceriyorMu = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                detay.sarfMalzemesiVarMi = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                detay.demirbasVarMi = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                detay.durum = VeriErisim.DegerAl(dr, kol++, 0);
                detay.butceRefLotID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.tahminiBaslamaTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.avansParaBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.sozlesmeNoAlmaTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));

                detay.firmaVNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.firmaAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.firmaIBAN = VeriErisim.DegerAl(dr, kol++, "");

                detay.kurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);

                detay.ikrazID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.anlasmaKodu = VeriErisim.DegerAl(dr, kol++, "");

                detay.bilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesenAdi = VeriErisim.DegerAl(dr, kol++, "");

                detay.altBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBilesenAdi = VeriErisim.DegerAl(dr, kol++, "");

                detay.satKategori = VeriErisim.DegerAl(dr, kol++, "");
                detay.satID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.satReferansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.satAcikTR = VeriErisim.DegerAl(dr, kol++, "");
                detay.satAcikEN = VeriErisim.DegerAl(dr, kol++, "");
                detay.satGrupEk = VeriErisim.DegerAl(dr, kol++, "");

                detay.butceReferansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceReferansAciklama = VeriErisim.DegerAl(dr, kol++, "");

                detay.butceRefLotNo = VeriErisim.DegerAl(dr, kol++, 0);
                detay.butceRefLotAciklama = VeriErisim.DegerAl(dr, kol++, "");

                string odemePlaniXML = VeriErisim.DegerAl(dr, kol++, "");
                string hakedislerXML = VeriErisim.DegerAl(dr, kol++, "");
                detay.odemePlani = new TNSCollection();
                detay.hakedisler = new TNSCollection();

                if (!string.IsNullOrWhiteSpace(odemePlaniXML))
                {
                    XMLStringDataReader r = new XMLStringDataReader(odemePlaniXML);
                    if (r != null)
                    {
                        DBOdemePlani dBOdemePlani = null;
                        while (r.Read())
                        {
                            int kolIc = 0;
                            int ODEMEPLANID = VeriErisim.DegerAl(r, kolIc++, 0);
                            if (dBOdemePlani == null || dBOdemePlani.odemePlanID != ODEMEPLANID)
                            {
                                if (dBOdemePlani != null) dBOdemePlani.RakamlariHesapla();
                                dBOdemePlani = new DBOdemePlani();
                                dBOdemePlani.odemePlanID = ODEMEPLANID;
                                dBOdemePlani.sozlesmeID = VeriErisim.DegerAl(r, kolIc++, 0);
                                dBOdemePlani.tahminiSozlesmeBitisi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                                dBOdemePlani.odemePlaniTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                                dBOdemePlani.durum = VeriErisim.DegerAl(r, kolIc++, 0);
                                dBOdemePlani.aciklama = VeriErisim.DegerAl(r, kolIc++, "");
                                dBOdemePlani.odemePlaniTipi = VeriErisim.DegerAl(r, kolIc++, 0);
                                dBOdemePlani.kurUSD = VeriErisim.DegerAl(r, kolIc++, 0.0);
                                dBOdemePlani.kurEUR = VeriErisim.DegerAl(r, kolIc++, 0.0);
                                dBOdemePlani.paraBirimi = detay.paraBirimi;
                                detay.odemePlani.Add(dBOdemePlani);
                            }
                            else
                                kolIc += 8;
                            DBOdemePlaniDetay dty = new DBOdemePlaniDetay(ODEMEPLANID,
                                VeriErisim.DegerAl(r, kolIc++, 0),
                                VeriErisim.DegerAl(r, kolIc++, 0),
                                VeriErisim.DegerAl(r, kolIc++, 0.0));
                            dty.RakamlariHesapla(dBOdemePlani.paraBirimi, dBOdemePlani.kurUSD, dBOdemePlani.kurEUR);
                            dBOdemePlani.odenecekTutarEUR += dty.odenecekTutarEUR;
                            dBOdemePlani.odenecekTutarUSD += dty.odenecekTutarUSD;
                            dBOdemePlani.odenecekTutarTRY += dty.odenecekTutarTRY;

                            dBOdemePlani.detaylar.Add(dty);
                        }
                        r.Close();
                        if (dBOdemePlani != null) dBOdemePlani.RakamlariHesapla();
                    }
                }

                if (!string.IsNullOrWhiteSpace(hakedislerXML))
                {
                    XMLStringDataReader r = new XMLStringDataReader(hakedislerXML);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            int kolIc = 0;
                            DBHakedisOzeti hakedisOzeti = new DBHakedisOzeti();
                            hakedisOzeti.hakedisID = VeriErisim.DegerAl(r, kolIc++, 0);
                            hakedisOzeti.belgeTurKod = VeriErisim.DegerAl(r, kolIc++, 0);
                            hakedisOzeti.belgeTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                            hakedisOzeti.belgeNo = VeriErisim.DegerAl(r, kolIc++, "");
                            hakedisOzeti.paraBirimi = VeriErisim.DegerAl(r, kolIc++, "");
                            hakedisOzeti.kurTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                            hakedisOzeti.kurUSD = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.kurEUR = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.odenen = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.trKarsilik = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.eurKarsilik = VeriErisim.DegerAl(r, kolIc++, 0.0);

                            detay.hakedisler.Add(hakedisOzeti);
                        }
                        r.Close();
                    }
                }

                detay.SozlesmeTutarHesapla(Convert.ToDouble(kurUSD), Convert.ToDouble(kurEUR));

                if (sozlesme.sozTutarBasla > 0)
                {
                    if (detay.sozlesmeTutariSon < sozlesme.sozTutarBasla) continue;
                }

                if (sozlesme.sozTutarBit > 0)
                {
                    if (detay.sozlesmeTutariSon > sozlesme.sozTutarBit) continue;
                }

                if (sozlesme.sozlesmeID > 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, detay.altBilesenID))
                {
                    oa.sonuc.DegerAta($"${detay.altBilesenID} kodlu alt bileşenle ilgili işlem yapma yetkiniz  bulunmamaktadır!");
                    break;
                }
                else
                    oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleSozlesmesizOdemeler(VeriErisim veriErisim, Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1)
        {
            ObjectArray oa = new ObjectArray();

            if (!GormeYetkisiVarMi(kullanan, sozlesme.altBilesenID))
            {
                oa.sonuc.DegerAta("Verilen alt bileşeni görme yetkiniz bulunmamaktadır!");
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            MerkezBankasiKurSinif mbForm = new MerkezBankasiKurSinif() { yakinGunBul = true };
            if (TNSDateTime.IsNull(sozlesme.kayitTarih))
                mbForm.kurTarih = new TNSDateTime(DateTime.Now.Date);
            else
                mbForm.kurTarih = sozlesme.kayitTarih;
            mbForm.satirlar = new HRC.MerkezBankasiKurDetaySinif[2];
            mbForm.satirlar[0] = MerkezBankasiKurDetaySinif.Olustur("EUR");
            mbForm.satirlar[1] = MerkezBankasiKurDetaySinif.Olustur("USD");
            MerkezBankasiKurSinif sonDegerler = HRCMerkezBankKur.Oku(null, kullanan, mbForm);

            decimal kurUSD = sonDegerler.KurBul("USD", "DOVIZSATIS", 0);
            decimal kurEUR = sonDegerler.KurBul("EUR", "DOVIZSATIS", 0);

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.ListeleSozlesmesizOdemeler(sozlesme, siralama);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBSozlesme detay = new DBSozlesme();
                int kol = 0;
                detay.sozlesmeID = -1;
                detay.butceRefID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.sozlesmeNo = -1;
                detay.konu = "";
                detay.aciklama = "";
                detay.ilgiliKod = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.sozlesmeTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                //detay.avansHukmu = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.avansOrani = VeriErisim.DegerAl(dr, kol++, 0.0);
                //detay.avansMahsubu = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.avansMahsubuOrani = VeriErisim.DegerAl(dr, kol++, 0.0);
                //detay.avansTeminatMektubu = VeriErisim.DegerAl(dr, kol++, "");
                //detay.kesinTeminatMektubu = VeriErisim.DegerAl(dr, kol++, "");
                //detay.ihtiyatKesintisiIceriyorMu = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                //detay.sarfMalzemesiVarMi = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                //detay.demirbasVarMi = VeriErisim.DegerAl(dr, kol++, 0) == 1;
                //detay.durum = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.butceRefLotID = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.tahminiBaslamaTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                //detay.avansParaBirimi = VeriErisim.DegerAl(dr, kol++, "");
                //detay.sozlesmeNoAlmaTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));

                detay.firmaVNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.firmaAdi = VeriErisim.DegerAl(dr, kol++, "");
                detay.firmaIBAN = VeriErisim.DegerAl(dr, kol++, "");

                detay.kurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);

                detay.ikrazID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.anlasmaKodu = VeriErisim.DegerAl(dr, kol++, "");

                detay.bilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.bilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.bilesenAdi = VeriErisim.DegerAl(dr, kol++, "");

                detay.altBilesenID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.altBilesenNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.altBilesenAdi = VeriErisim.DegerAl(dr, kol++, "");

                detay.satKategori = VeriErisim.DegerAl(dr, kol++, "");
                detay.satID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.satReferansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.satAcikTR = VeriErisim.DegerAl(dr, kol++, "");
                detay.satAcikEN = VeriErisim.DegerAl(dr, kol++, "");
                detay.satGrupEk = VeriErisim.DegerAl(dr, kol++, "");

                detay.butceReferansNo = VeriErisim.DegerAl(dr, kol++, "");
                detay.butceReferansAciklama = VeriErisim.DegerAl(dr, kol++, "");

                //detay.butceRefLotNo = VeriErisim.DegerAl(dr, kol++, 0);
                //detay.butceRefLotAciklama = VeriErisim.DegerAl(dr, kol++, "");

                //string odemePlaniXML = VeriErisim.DegerAl(dr, kol++, "");
                string hakedislerXML = VeriErisim.DegerAl(dr, kol++, "");
                detay.odemePlani = new TNSCollection();
                detay.hakedisler = new TNSCollection();
                if (!string.IsNullOrWhiteSpace(hakedislerXML))
                {
                    XMLStringDataReader r = new XMLStringDataReader(hakedislerXML);
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            int kolIc = 0;
                            DBHakedisOzeti hakedisOzeti = new DBHakedisOzeti();
                            hakedisOzeti.hakedisID = VeriErisim.DegerAl(r, kolIc++, 0);
                            hakedisOzeti.belgeTurKod = VeriErisim.DegerAl(r, kolIc++, 0);
                            hakedisOzeti.belgeTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                            hakedisOzeti.belgeNo = VeriErisim.DegerAl(r, kolIc++, "");
                            hakedisOzeti.paraBirimi = VeriErisim.DegerAl(r, kolIc++, "");
                            hakedisOzeti.kurTarihi.Yaz(VeriErisim.DegerAl(r, kolIc++));
                            hakedisOzeti.kurUSD = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.kurEUR = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.odenen = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.trKarsilik = VeriErisim.DegerAl(r, kolIc++, 0.0);
                            hakedisOzeti.eurKarsilik = VeriErisim.DegerAl(r, kolIc++, 0.0);

                            detay.hakedisler.Add(hakedisOzeti);
                        }
                        r.Close();
                    }
                }

                detay.SozlesmeTutarHesapla(Convert.ToDouble(kurUSD), Convert.ToDouble(kurEUR), true);

                //if (sozlesme.sozTutarBasla > 0)
                //{
                //    if (detay.sozlesmeTutariSon < sozlesme.sozTutarBasla) continue;
                //}

                //if (sozlesme.sozTutarBit > 0)
                //{
                //    if (detay.sozlesmeTutariSon > sozlesme.sozTutarBit) continue;
                //}

                //if (sozlesme.sozlesmeID > 0 && !DBIkrazlar.GormeYetkisiVarMi(kullanan, detay.altBilesenID))
                //{
                //    oa.sonuc.DegerAta($"${detay.altBilesenID} kodlu alt bileşenle ilgili işlem yapma yetkiniz  bulunmamaktadır!");
                //    break;
                //}
                //else
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectVAN OkuDBSozlesme(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray oa = ListeleDBSozlesmeler(veriErisim, kullanan, new DBSozlesme() { sozlesmeID = sozlesmeID });
            if (!oa.sonuc.islemSonuc)
            {
                ov.sonuc = oa.sonuc;
                return ov;
            }
            if (oa.ObjeSayisi == 0)
            {
                ov.sonuc.DegerAta("Aranan sözleşme bulunamadı!");
            }
            else if (oa.ObjeSayisi > 1)
            {
                ov.sonuc.DegerAta("Beklenenden fazle sayıda sözleşme bulundu!");
            }
            else
                ov.o = oa.Al(0);
            return ov;
        }

        internal static Sonuc KaydetDBSozlesme(VeriErisim veriErisim, Kullanici kullanan, DBSozlesme form)
        {
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            if (form.sozlesmeID > 0)
            {
                ObjectVAN ov = OkuDBSozlesme(ve, kullanan, form.sozlesmeID);

                if (ov.sonuc.islemSonuc)
                {
                    DBSozlesme okunan = (DBSozlesme)ov.o;

                    if (!YazmaYetkisiVarMi(kullanan, okunan.altBilesenID))
                    {
                        s.DegerAta("Yetkiniz yok!");
                        return s;
                    }

                    if (okunan.durum >= 5 && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI))
                        ov.sonuc.DegerAta("Durumu kapanmış sözleşmeler değiştirilemez!");
                }
                if (!ov.sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return ov.sonuc;
                }
            }
            else
            {
                ObjectVAN ov = OkuButceRef(ve, kullanan, new DBButceRef() { butceRefID = form.butceRefID });
                if (ov.sonuc.islemSonuc)
                {
                    DBIkraz okunanIkraz = (DBIkraz)ov.o;
                    if (okunanIkraz != null && okunanIkraz.bilesenler.Count > 0)
                    {
                        DBBilesen okunanBilesen = (DBBilesen)okunanIkraz.bilesenler[0];
                        if (okunanBilesen != null && okunanBilesen.altBilesenler.Count > 0)
                        {
                            DBAltBilesen okunanAltBilesen = (DBAltBilesen)okunanBilesen.altBilesenler[0];
                            if (okunanAltBilesen != null)
                            {
                                if (!YazmaYetkisiVarMi(kullanan, okunanAltBilesen.altBilesenID))
                                {
                                    s.DegerAta("Yetkiniz yok!");
                                }
                            }
                            else
                                s.DegerAta("Alt bileşen bulunamadı!");
                        }
                        else
                            s.DegerAta("Bileşen bulunamadı!");
                    }
                    else
                    {
                        s.DegerAta("İkraz okunamadı!");
                    }
                }
            }
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (form.sozlesmeID == 0)
                {
                    form.sozlesmeID = TABDBIKRAZ.EkleSozlesme(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                    s = TABDBIKRAZ.GuncelleSozlesme(form);

                if (s.islemSonuc)
                    s = tabTarihce.Ekle("DBSOZ", form.sozlesmeID.ToString(), kullanan.kullaniciKodu, "Sözleşme Kaydedildi", "");

                if (s.islemSonuc)
                {
                    ve.Commit();
                    s.anahtar = form.sozlesmeID.ToString();
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectVAN DBSozlesmeNoAl(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID)
        {
            ObjectVAN ov = new ObjectVAN();
            if (sozlesmeID <= 0)
            {
                ov.sonuc.DegerAta("Numara alacak sözleşme bilgisi hatalı!");
                return ov;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ov = OkuDBSozlesme(ve, kullanan, sozlesmeID);
            DBSozlesme okunan = (DBSozlesme)ov.o;

            if (ov.sonuc.islemSonuc)
            {
                if (okunan.durum >= 5)
                    ov.sonuc.DegerAta("Durumu kapanmış sözleşmeler değiştirilemez!");
                if (okunan.sozlesmeNo > 0)
                    ov.sonuc.DegerAta("Sözleşme numarası alınmış, tekrar alınamaz!");
            }
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov;
            }

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            ve.BeginTransaction();


            int yeniSozlesmeNo = TABDBIKRAZ.SozlesmeNoBul(okunan.ikrazID);
            if (!string.IsNullOrWhiteSpace(ve.hata))
                ov.sonuc.DegerAta("Sözleşme numarası alınamadı!");
            if (ov.sonuc.islemSonuc)
            {
                okunan.sozlesmeNo = yeniSozlesmeNo;
                okunan.sozlesmeNoAlmaTarihi.Yaz(DateTime.Now.Date);
                ov.sonuc = TABDBIKRAZ.SozlesmeNoYaz(okunan);
            }
            if (ov.sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return ov;
        }

        internal static ObjectVAN DBSozlesmeKapat(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID, int durum, string gerekce)
        {
            ObjectVAN ov = new ObjectVAN();
            if (sozlesmeID <= 0)
            {
                ov.sonuc.DegerAta("Numara alacak sözleşme bilgisi hatalı!");
                return ov;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ov = OkuDBSozlesme(ve, kullanan, sozlesmeID);
            DBSozlesme okunan = (DBSozlesme)ov.o;

            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov;
            }

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            okunan.durum = durum;
            okunan.gerekce = gerekce;
            ov.sonuc = TABDBIKRAZ.SozlesmeKapat(okunan);

            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);

            if (ov.sonuc.islemSonuc)
                ov.sonuc = tabTarihce.Ekle("DBSOZ", okunan.sozlesmeID.ToString(), kullanan.kullaniciKodu, "Durum değiştirildi (" + durum + ")", gerekce);

            if (ov.sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return ov;
        }

        internal static Sonuc DBSilSozlesme(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID)
        {
            Sonuc s = new Sonuc();
            if (sozlesmeID <= 0)
            {
                s.DegerAta("Numara alacak sözleşme bilgisi hatalı!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ov = OkuDBSozlesme(ve, kullanan, sozlesmeID);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }

            if (ov.o == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Sözleşme bulunamadı!");
            }

            DBSozlesme okunan = (DBSozlesme)ov.o;
            if (okunan.hakedisler.Count > 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Kendisiyle ilgili hakediş bulunan sözleşmeler silinemez!");
            }

            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);
            try
            {
                ve.BeginTransaction();
                s = TABDBIKRAZ.SilOdemePlaniDetay(sozlesmeID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }
                s = TABDBIKRAZ.SilOdemePlani(sozlesmeID);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    return s;
                }
                s = TABDBIKRAZ.SilSozlesme(sozlesmeID);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                s.DegerAta(ex.Message);
            }
            finally
            {
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static ObjectVAN DBSozlesmeOdemePlaniOku(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID, int odemePlanID)
        {
            ObjectVAN ov = new ObjectVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.SozlemeOdemePlaniListele(odemePlanID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                ov.sonuc.DegerAta(ve.hata);
                return ov;
            }

            DBOdemePlani detay = new DBOdemePlani();
            if (dr.Read())
            {
                int kol = 0;

                detay.odemePlanID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.tahminiSozlesmeBitisi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.odemePlaniTarihi.Yaz(VeriErisim.DegerAl(dr, kol++));
                detay.durum = VeriErisim.DegerAl(dr, kol++, 0);
                detay.aciklama = VeriErisim.DegerAl(dr, kol++, "");
                detay.odemePlaniTipi = VeriErisim.DegerAl(dr, kol++, 0);
                detay.paraBirimi = VeriErisim.DegerAl(dr, kol++, "");
                detay.kurUSD = VeriErisim.DegerAl(dr, kol++, 0.0);
                detay.kurEUR = VeriErisim.DegerAl(dr, kol++, 0.0);
            }
            ov.o = detay;
            dr.Close();

            ObjectArray oa = DBSozlesmeOdemePlaniDetayListele(ve, kullanan, odemePlanID);
            if (!oa.sonuc.islemSonuc)
                ov.sonuc = oa.sonuc;
            else
            {
                detay.detaylar = oa.objeler;
                detay.RakamlariHesapla();
            }
            ve.VeriErisimKapat();
            return ov;
        }

        internal static ObjectArray DBSozlesmeOdemePlaniDetayListele(VeriErisim veriErisim, Kullanici kullanan, int odemePlanID)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.SozlemeOdemePlaniDetayListele(odemePlanID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBOdemePlaniDetay detay = new DBOdemePlaniDetay();
                int kol = 0;

                detay.odemePlanID = VeriErisim.DegerAl(dr, kol++, 0);
                detay.yil = VeriErisim.DegerAl(dr, kol++, 0);
                detay.ay = VeriErisim.DegerAl(dr, kol++, 0);
                detay.odenecekTutar = VeriErisim.DegerAl(dr, kol++, 0.0);


                oa.Ekle(detay);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ListeleOdemePlani(VeriErisim veriErisim, Kullanici kullanan, int sozlesmeID)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TABDBIKRAZ = new TABDBIKRAZ(ve);

            IDataReader dr;
            dr = TABDBIKRAZ.ListeleOdemePlani(sozlesmeID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                DBOdemePlani detay = new DBOdemePlani();
                int kol = 0;

                detay.odemePlanID = VeriErisim.DegerAl(dr, kol++, 0);

                oa.Ekle(detay);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc KaydetDBSozlesmeOdemePlani(VeriErisim veriErisim, Kullanici kullanan, DBOdemePlani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.sozlesmeID <= 0)
            {
                s.DegerAta("Geçersiz sözleşme numarası!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray oa = ListeleDBSozlesmeler(ve, kullanan, new DBSozlesme() { sozlesmeID = form.sozlesmeID });
            if (!oa.sonuc.islemSonuc)
            {
                s = oa.sonuc;
            }

            if (oa.ObjeSayisi != 1)
            {
                s.DegerAta("Verilen sözleşme bulunamadı!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            DBSozlesme okunan = (DBSozlesme)oa.Al(0);

            if (form.odemePlanID == 0)
            {
                foreach (DBOdemePlani item in okunan.odemePlani)
                {
                    if (item.durum == 1)
                    {
                        s.DegerAta("Zaten durumu yeni olan bir plan varken, yeni plan açılamaz!");
                        break;
                    }
                }
            }
            else
            {
                foreach (DBOdemePlani item in okunan.odemePlani)
                {
                    if (item.odemePlanID == form.odemePlanID)
                    {
                        if (item.durum != 1)
                        {
                            s.DegerAta("Yeni durumundaki ödeme planları değiştirilebilir.!");
                        }
                        break;
                    }
                }
            }

            int satir = 1;
            foreach (DBOdemePlaniDetay det in form.detaylar)
            {
                if (det.BosMu()) continue;
                if (det.yil <= 0 || det.ay <= 0 || det.odenecekTutar < 0.001)
                {
                    s.DegerAta($"{satir}. satırda yıl, ay veya ödenecek tutar hatalı girilmiş!");
                }
                satir++;
            }
            if (s.islemSonuc)
            {
                double toplam = 0;
                for (int i = 0; i < form.detaylar.Count; i++)
                {
                    DBOdemePlaniDetay detDis = (DBOdemePlaniDetay)form.detaylar[i];
                    if (detDis.BosMu()) continue;
                    toplam += detDis.odenecekTutar;
                    for (int j = i + 1; j < form.detaylar.Count; j++)
                    {
                        DBOdemePlaniDetay detIc = (DBOdemePlaniDetay)form.detaylar[j];
                        if (detIc.BosMu()) continue;
                        if (detIc.yil == detDis.yil && detIc.ay == detDis.ay)
                        {
                            s.DegerAta($"{i + 1}. satır ile {j + 1}. satırın  yıl, ay ({detIc.yil}, {detIc.ay}) değerleri aynı!");
                        }
                    }
                }
                if (s.islemSonuc && form.odemePlaniTipi == 2)
                {
                    if (!TNS.OrtakFonksiyonlar.EsitMi(okunan.sozlesmeTutariSon, toplam))
                    {
                        s.DegerAta($"Ödeme planı değişikliğinde girilen tutarların toplamı ({toplam.ToString("#,##0,00")}), sözleşme tutarına ({okunan.sozlesmeTutariSon.ToString("#,##0,00")}) eşit olmalıdır! ");
                    }
                }
            }
            if (!YazmaYetkisiVarMi(kullanan, okunan.altBilesenID))
            {
                s.DegerAta("Kaydetme yetkiniz bulunmamaktadır!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABDBIKRAZ tabDBIKRAZ = new TABDBIKRAZ(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);
            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (form.odemePlanID == 0)
                {
                    form.odemePlanID = tabDBIKRAZ.EkleOdemePlani(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }
                else
                {
                    s = tabDBIKRAZ.GuncelleOdemePlani(form);
                    if (s.islemSonuc)
                    {
                        s = tabDBIKRAZ.SilOdemePlaniDetay(form);
                    }
                }
            }

            foreach (DBOdemePlaniDetay detay in form.detaylarTanimli)
            {
                if (!s.islemSonuc) break;

                detay.odemePlanID = form.odemePlanID;
                if (detay.odenecekTutar > 0)
                    s = tabDBIKRAZ.EkleOdemePlaniDetay(detay);
            }

            if (s.islemSonuc)
                s = tabTarihce.Ekle("DBSOZ", form.sozlesmeID.ToString(), kullanan.kullaniciKodu, "Sözleşme Ödeme Planı Kaydedildi", "");

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.odemePlanID.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc DurumDegistirDBSozlesmeOdemePlani(VeriErisim veriErisim, Kullanici kullanan, DBOdemePlani form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.sozlesmeID <= 0)
            {
                s.DegerAta("Geçersiz sözleşme numarası!");
                return s;
            }
            if (form.odemePlanID <= 0)
            {
                s.DegerAta("Geçersiz ödeme planı numarası!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray oa = ListeleDBSozlesmeler(ve, kullanan, new DBSozlesme() { sozlesmeID = form.sozlesmeID });
            if (!oa.sonuc.islemSonuc)
            {
                s = oa.sonuc;
            }

            if (oa.ObjeSayisi != 1)
            {
                s.DegerAta("Verilen sözleşme bulunamadı!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oa.sonuc;
            }

            DBSozlesme okunan = (DBSozlesme)oa.Al(0);

            foreach (DBOdemePlani item in okunan.odemePlani)
            {
                if (item.odemePlanID == form.odemePlanID)
                {
                    if (item.durum != 1)
                    {
                        //s.DegerAta("Yeni durumundaki ödeme planları değiştirilebilir.!");
                    }
                    break;
                }
            }

            if (!YazmaYetkisiVarMi(kullanan, okunan.altBilesenID))
            {
                s.DegerAta("Kaydetme yetkiniz bulunmamaktadır!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABDBIKRAZ tabDBIKRAZ = new TABDBIKRAZ(ve);
            TABDBTARIHCE tabTarihce = new TABDBTARIHCE(ve);
            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                s = tabDBIKRAZ.DurumDegistirSozlesmeOdemePlani(form);
            }

            if (s.islemSonuc)
            {
                s = tabTarihce.Ekle("DBSOZ", form.sozlesmeID.ToString(), kullanan.kullaniciKodu, "Ödeme Alma Durum Değiştirildi (" + form.durum + ")", "");
            }

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        public static ObjectArray ListeleDBEkstreler(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, int ikrazID)
        {

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABDBIKRAZ TAB = new TABDBIKRAZ(ve);
            ObjectArray oa = new ObjectArray();

            double bakiye;
            if (!TNSDateTime.IsNull(baslangicTarih))
            {
                bakiye = TAB.BakiyeBul(new TNSDateTime(baslangicTarih), ikrazID);
                DBEkstre detayB = new DBEkstre();
                detayB.BalanceEur = bakiye;
                detayB.EksDate.Yaz(new TNSDateTime(baslangicTarih));
                oa.Ekle(detayB);
            }
            else
                bakiye = 0;

            IDataReader dr = TAB.ListeleEkstreler(baslangicTarih, bitisTarih, ikrazID);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                int kolIc = 0;

                DBEkstre detay = new DBEkstre();

                detay.Numarator = VeriErisim.DegerAl(dr, kolIc++, "");
                detay.IkrazID = VeriErisim.DegerAl(dr, kolIc++, 0);

                detay.EksDate.Yaz(
                    VeriErisim.DegerAl(dr, kolIc++)
                );
                detay.EksDateSiraNo = VeriErisim.DegerAl(dr, kolIc++, 0);

                detay.CatNo = VeriErisim.DegerAl(dr, kolIc++, "");
                detay.Explanation = VeriErisim.DegerAl(dr, kolIc++, "");
                detay.Cur = VeriErisim.DegerAl(dr, kolIc++, "");
                detay.AmountPaid = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.ExchRate = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.TryEquiv = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.EurEquiv = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.DepositEur = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.DepositTry = VeriErisim.DegerAl(dr, kolIc++, 0.0);
                detay.SGBNumarator = VeriErisim.DegerAl(dr, kolIc++, 0);
                detay.HakedisVar = detay.SGBNumarator > 0 && !dr.IsDBNull(kolIc);
                detay.HakedisTutari = VeriErisim.DegerAl(dr, kolIc, 0.0);
                bakiye += detay.DepositEur - detay.EurEquiv;
                detay.BalanceEur = bakiye;
                oa.Ekle(detay);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return oa;
        }

        public static DBNakitPozisyonu OkuDBNakitPozisyonu(VeriErisim veriErisim, Kullanici kullanan, int ikrazID)
        {
            DBNakitPozisyonu cevap = new DBNakitPozisyonu();
            if (ikrazID <= 0)
            {
                cevap.sonuc.DegerAta("Lütfen ikraz seçiniz!");
                return cevap;
            }

            ObjectArray ekstreler = ListeleDBEkstreler(veriErisim, kullanan, null, null, ikrazID);
            if (!ekstreler.sonuc.islemSonuc)
            {
                cevap.sonuc = ekstreler.sonuc;
                return cevap;
            }
            if (ekstreler.ObjeSayisi == 0)
            {
                cevap.sonuc.DegerAta("Seçilen ikraza ait banka ekstresi bulunamadı!");
                return cevap;
            }

            DBEkstre sonEkstre = (DBEkstre)ekstreler.objeler[ekstreler.ObjeSayisi - 1];
            cevap.sonEkstreTarihi = sonEkstre.EksDateDT;
            cevap.bankaBakiyesiEUR = sonEkstre.BalanceEur;

            MerkezBankasiKurSinif kurKriter = new MerkezBankasiKurSinif
            {
                // Nakit durumu raporunun alındığı güne ait, yoksa o güne en yakın kur kullanılır.
                kurTarih = new TNSDateTime(DateTime.Now.Date),
                yakinGunBul = true
            };
            MerkezBankasiKurSinif kurlar = HRCMerkezBankKur.Oku(null, kullanan, kurKriter);
            if (!kurlar.sonuc.islemSonuc)
            {
                cevap.sonuc = kurlar.sonuc;
                return cevap;
            }

            cevap.kurTarihi = kurlar.kurTarih.Oku();
            if (kurlar.satirlar != null)
            {
                foreach (MerkezBankasiKurDetaySinif kur in kurlar.satirlar)
                {
                    if (kur.paraBirim.kisaltma == "USD") cevap.usdKuru = (double)kur.dovizSatis;
                    if (kur.paraBirim.kisaltma == "EUR") cevap.eurKuru = (double)kur.dovizSatis;
                }
            }
            if (cevap.usdKuru <= 0) cevap.usdKuru = 1;
            if (cevap.eurKuru <= 0) cevap.eurKuru = 1;

            ObjectArray hakedisler = DBHakedisIslemleri.ListeleDBHakedis(
                null, kullanan, new DBHakedis { ikrazID = ikrazID, durumKod = 90 }, true, 0);
            if (!hakedisler.sonuc.islemSonuc)
            {
                cevap.sonuc = hakedisler.sonuc;
                return cevap;
            }

            ObjectArray altBilesenler = ListeleDBAltBilesenler(
                null, kullanan, new DBAltBilesen { ikrazID = ikrazID });
            if (!altBilesenler.sonuc.islemSonuc)
            {
                cevap.sonuc = altBilesenler.sonuc;
                return cevap;
            }

            Dictionary<string, DBNakitPozisyonGrup> gruplar = new Dictionary<string, DBNakitPozisyonGrup>();
            string[] paraBirimleri = { "TRY", "USD", "EUR" };
            foreach (bool numarali in new bool[] { true, false })
                foreach (string paraBirimi in paraBirimleri)
                    gruplar.Add(NakitGrupAnahtari(numarali, paraBirimi),
                        new DBNakitPozisyonGrup { numarali = numarali, paraBirimi = paraBirimi });
            Dictionary<string, DBNakitPozisyonBilesen> bilesenler = new Dictionary<string, DBNakitPozisyonBilesen>();
            foreach (DBAltBilesen altBilesen in altBilesenler.objeler)
            {
                string anahtar = altBilesen.altBilesenID + "|" + (altBilesen.altBilesenNo ?? "");
                if (!bilesenler.ContainsKey(anahtar))
                {
                    bilesenler.Add(anahtar, new DBNakitPozisyonBilesen
                    {
                        altBilesenID = altBilesen.altBilesenID,
                        altBilesenNo = altBilesen.altBilesenNo,
                        altBilesenAd = altBilesen.altBilesenAd
                    });
                }
            }

            foreach (DBHakedis hakedis in hakedisler.objeler)
            {
                string paraBirimi = (hakedis.paraBirimi ?? "").Trim().ToUpperInvariant();
                if (paraBirimi == "TL") paraBirimi = "TRY";
                if (!gruplar.ContainsKey(NakitGrupAnahtari(true, paraBirimi)) || hakedis.hakedisSahipleri == null) continue;

                foreach (DBHakedisSahibi hakSahibi in hakedis.hakedisSahipleri)
                {
                    if (!TNSDateTime.IsNull(hakSahibi.odemeTarihi)) continue;

                    bool numarali = hakSahibi.numarator > 0;
                    double eurKarsiligi = NakitEuroyaCevir(
                        hakSahibi.kesintiTutari, paraBirimi, cevap.usdKuru, cevap.eurKuru);

                    string bilesenAnahtari = hakedis.altBilesenID + "|" + (hakedis.altBilesenNo ?? "");
                    if (!bilesenler.ContainsKey(bilesenAnahtari))
                    {
                        bilesenler.Add(bilesenAnahtari, new DBNakitPozisyonBilesen
                        {
                            altBilesenID = hakedis.altBilesenID,
                            altBilesenNo = hakedis.altBilesenNo,
                            altBilesenAd = hakedis.altBilesenAd
                        });
                    }
                    DBNakitPozisyonBilesen bilesen = bilesenler[bilesenAnahtari];
                    if (numarali) bilesen.numaraliToplamEUR += eurKarsiligi;
                    else bilesen.numarasizToplamEUR += eurKarsiligi;

                    DBNakitPozisyonGrup grup = gruplar[NakitGrupAnahtari(numarali, paraBirimi)];
                    grup.tutar += hakSahibi.kesintiTutari;
                    grup.eurKarsiligi += eurKarsiligi;
                    grup.kayitSayisi++;
                    cevap.detaylar.Add(new DBNakitPozisyonDetay
                    {
                        hakedisID = hakedis.hakedisID,
                        numarator = hakSahibi.numarator,
                        ilgiliAd = hakSahibi.ilgiliAd,
                        paraBirimi = paraBirimi,
                        tutar = hakSahibi.kesintiTutari,
                        eurKarsiligi = eurKarsiligi,
                        numarali = numarali
                    });
                }
            }

            foreach (bool numarali in new bool[] { true, false })
                foreach (string paraBirimi in paraBirimleri)
                {
                    DBNakitPozisyonGrup grup = gruplar[NakitGrupAnahtari(numarali, paraBirimi)];
                    cevap.gruplar.Add(grup);
                    cevap.toplamOdemeEUR += grup.eurKarsiligi;
                    if (numarali) cevap.numaraliToplamEUR += grup.eurKarsiligi;
                    else cevap.numarasizToplamEUR += grup.eurKarsiligi;
                }

            List<DBNakitPozisyonBilesen> bilesenListesi = new List<DBNakitPozisyonBilesen>(bilesenler.Values);
            bilesenListesi.Sort(delegate(DBNakitPozisyonBilesen sol, DBNakitPozisyonBilesen sag)
            {
                return NakitBilesenNoKarsilastir(sol.altBilesenNo, sag.altBilesenNo);
            });
            cevap.bilesenler.AddRange(bilesenListesi.ToArray());
            cevap.netPozisyonEUR = cevap.bankaBakiyesiEUR - cevap.toplamOdemeEUR;
            cevap.finansmanIhtiyaciEUR = cevap.netPozisyonEUR < 0 ? -cevap.netPozisyonEUR : 0;
            return cevap;
        }

        private static string NakitGrupAnahtari(bool numarali, string paraBirimi)
        {
            return (numarali ? "1" : "0") + paraBirimi;
        }

        private static double NakitEuroyaCevir(double tutar, string paraBirimi, double usdKuru, double eurKuru)
        {
            if (paraBirimi == "EUR") return tutar;
            if (paraBirimi == "USD") return tutar * usdKuru / eurKuru;
            return tutar / eurKuru;
        }

        private static int NakitBilesenNoKarsilastir(string sol, string sag)
        {
            string[] solParcalar = (sol ?? "").Split('.');
            string[] sagParcalar = (sag ?? "").Split('.');
            int uzunluk = Math.Max(solParcalar.Length, sagParcalar.Length);
            for (int i = 0; i < uzunluk; i++)
            {
                int solSayi = 0;
                int sagSayi = 0;
                bool solSayisal = i < solParcalar.Length && int.TryParse(solParcalar[i], out solSayi);
                bool sagSayisal = i < sagParcalar.Length && int.TryParse(sagParcalar[i], out sagSayi);
                if (solSayisal && sagSayisal && solSayi != sagSayi) return solSayi.CompareTo(sagSayi);
                if (i >= solParcalar.Length) return -1;
                if (i >= sagParcalar.Length) return 1;
                int metinSonucu = StringComparer.CurrentCultureIgnoreCase.Compare(solParcalar[i], sagParcalar[i]);
                if (metinSonucu != 0) return metinSonucu;
            }
            return 0;
        }

        public static Sonuc DBEkstrelerVeriEkle(VeriErisim veriErisim, Kullanici kullanan, DBEkstre form)
        {
            Sonuc s = new Sonuc(true, "", "");

            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta("Ekstre bilgisi değiştirme yetkiniz bulunmamaktadır!");
                return s;
            }

            form.Kontrol(s);
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(
                veriErisim,
                VERIERISIMAD,
                TNS.KYM.Arac.KimlikBul(kullanan)
            );

            TABDBIKRAZ TAB = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            try
            {
                if (form.Numarator == null || form.Numarator == "")
                {
                    s.DegerAta("Numaratör boş olamaz!");
                }
                else
                {
                    s = TAB.EkleEkstre(form);
                    if (!string.IsNullOrWhiteSpace(ve.hata))
                        s.DegerAta(ve.hata);
                }

                if (s.islemSonuc)
                {
                    ve.Commit();
                    s.anahtar = form.Numarator.ToString();
                }
                else
                {
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                s.DegerAta(ex.Message);
            }

            ve.VeriErisimKapat();

            return s;
        }

        internal static Sonuc KaydetDBSGBNumarator(VeriErisim veriErisim, Kullanici kullanan, DBEkstre[] liste)
        {
            Sonuc s = new Sonuc();

            if (liste == null || liste.Length == 0)
            {
                s.DegerAta("Kaydedilecek veri bulunamadı.");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABDBIKRAZ TAB = new TABDBIKRAZ(ve);

            ve.BeginTransaction();

            foreach (var item in liste)
            {
                if (!s.islemSonuc) break;
                item.EksDate = new TNSDateTime(item.EksDateDT);
                s = TAB.GuncelleSGBNumarator(item);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return s;
        }
    }
}
