using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// KYMYoneticiBildirimFormu formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class KYMYoneticiBildirimFormu
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool HasWriteAccess(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            TNS.MUH.IMUHServis servisMuh = TNS.MUH.Arac.Tanimla();
            bool yetkiVar = servisMuh.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlik.ToString(), form.birim, MUH.ENUMOGY.Yazma);
            if (yetkiVar) return yetkiVar;

            TNS.ORG.IORGServis servisOrg = TNS.ORG.Arac.Tanimla();
            yetkiVar = servisOrg.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlik.ToString(), form.birim, ORG.ENUMOGY.Yazma);
            return yetkiVar;
        }

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc IsFieldsOK(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = "";

            if (form.saymanlik <= 0)
                hata = "Geçersiz saymanlık kodu girdiniz!<br>";

            if (string.IsNullOrWhiteSpace(form.birim))
                hata = "Geçersiz birim kodu girdiniz!<br>";

            if (string.IsNullOrWhiteSpace(form.yoneticiTCKimlikNo))
                hata += "Kaydetmek için yönetici olmasını istediğiniz kişinin kimlik numarasını giriniz!<br>";

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            string birimAd = uzyServis.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", form.birim + "-" + form.saymanlik, true);
            if (string.IsNullOrEmpty(birimAd))
                hata += "Muhasebe birim kodu bulunamadı!<br>";

            if (hata != "") sonuc.DegerAta(hata);
            return sonuc;
        }

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Save(VeriErisim veriErisim, Kullanici kullanan, YoneticiBildirimFormu[] formlar, ENUMYoneticiBildirimFormuDurum durum)
        {
            Sonuc s = new Sonuc();
            TNS.PER.IPERServis perServis = TNS.PER.Arac.Tanimla();
            for (int i = 0; i < formlar.Length; i++)
            {
                s = IsFieldsOK(kullanan, formlar[i]);
                if (!s.islemSonuc)
                {
                    return s;
                }
                if (!HasWriteAccess(kullanan, formlar[i]))
                {
                    s.DegerAta(KullaniciResI.YoneticiBildirimFormu001);
                    return s;
                }

                PER.Personel p = new PER.Personel();
                p.tcKimlikNo = formlar[i].yoneticiTCKimlikNo;
                ObjectArray oa = perServis.PersonelListele(kullanan, p);

                if (!oa.sonuc.islemSonuc) return oa.sonuc;
                if (oa.ObjeSayisi == 0) { s.DegerAta("Aranan personel bulunamadı!"); return s; }
                if (oa.ObjeSayisi > 1) { s.DegerAta("Aynı TC numarasına sahip birden fazla personel bulundu!"); return s; }

                p = (PER.Personel)oa.objeler[0];

                TNS.PER.PersonelAdres perAdres = new TNS.PER.PersonelAdres();
                perAdres.tcKimlikNo = p.tcKimlikNo;

                ObjectArray adresList = perServis.PersonelAdresListele(kullanan, perAdres);

                if (!adresList.sonuc.islemSonuc) return adresList.sonuc;
                if (adresList.objeler.Count >= 1)
                {
                    perAdres = (TNS.PER.PersonelAdres)adresList.objeler[0];
                    if (string.IsNullOrEmpty(perAdres.eposta)) { s.DegerAta("Kişiye ait email adresi girilmemiş!"); return s; }
                }
                else { s.DegerAta("Kişiye ait adres bilgileri bulunamadı!"); return s; }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            for (int i = 0; i < formlar.Length; i++)
            {
                YoneticiBildirimFormu eskiForm = null;
                if (formlar[i].talepNo != 0)
                {
                    eskiForm = Get(ve, kullanan, formlar[i]);
                    if (eskiForm == null)
                    {
                        s.DegerAta("Değiştirmek için verdiğiniz numaralı form bulunamadı!");
                    }
                    if ((int)eskiForm.durum > 1)
                        s.DegerAta("Değiştirmek istediğiniz form yeni olmadığı için değiştirilemez!");
                }

            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABKYMYONETICIBILDIRIMFORMU tabKYMYONETICIBILDIRIMFORMU = new TABKYMYONETICIBILDIRIMFORMU(ve);

            ve.BeginTransaction();
            for (int i = 0; i < formlar.Length; i++)
            {
                if (formlar[i].talepNo > 0)
                    s = tabKYMYONETICIBILDIRIMFORMU.Delete(formlar[i]);
                else
                {
                    formlar[i].talepNo = tabKYMYONETICIBILDIRIMFORMU.MaxTalepNoBul(formlar[i]) + 1;
                    if (!string.IsNullOrWhiteSpace(ve.hata)) s.DegerAta(ve.hata);
                }

                if (s.islemSonuc)
                    s = tabKYMYONETICIBILDIRIMFORMU.Insert(formlar[i], kullanan.kullaniciKodu);
                if (durum != ENUMYoneticiBildirimFormuDurum.Tanimsiz && s.islemSonuc)
                {
                    formlar[i].durum = durum;
                    s = tabKYMYONETICIBILDIRIMFORMU.DurumGuncelle(formlar[i], kullanan.kullaniciKodu);
                }
                if (!s.islemSonuc)
                    break;
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

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, YoneticiBildirimFormu[] formlar, ENUMYoneticiBildirimFormuDurum durum, int genelGorevKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            string hata = "";
            for (int i = 0; i < formlar.Length; i++)
            {
                YoneticiBildirimFormu form = formlar[i];
                if (!HasWriteAccess(kullanan, form))
                {
                    hata += KullaniciResI.YoneticiBildirimFormu001;
                    return sonuc;
                }
                formlar[i] = Get(ve, kullanan, form);
                if (formlar[i] == null)
                    hata += "Onaylanmak istenen form bulunamadı";

                if (formlar[i].durum == ENUMYoneticiBildirimFormuDurum.IptalEdildi || formlar[i].durum == ENUMYoneticiBildirimFormuDurum.Onaylandi || formlar[i].durum == ENUMYoneticiBildirimFormuDurum.Reddedildi)
                    hata += "Daha önce onaylanmış/reddedilmiş formu tekrar onaylayamazsınız!";
            }


            if (hata != "")
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (durum == ENUMYoneticiBildirimFormuDurum.Onaylandi || durum == ENUMYoneticiBildirimFormuDurum.Reddedildi)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    if (kullanan.YetkiTipiDegeriBul(101) != "1")
                    {
                        ve.VeriErisimKapat();
                        sonuc.DegerAta("Formu onaylama yetkiniz bulunmamaktadır!");
                        return sonuc;
                    }
                }
            }

            if (durum == ENUMYoneticiBildirimFormuDurum.Onaylandi)
            {
                TNS.ORG.IORGServis servisORG = TNS.ORG.Arac.Tanimla();
                for (int i = 0; i < formlar.Length; i++)
                {
                    YoneticiBildirimFormu form = formlar[i];

                    TNS.ORG.Personel[] personeller = new TNS.ORG.Personel[1];
                    personeller[0] = new TNS.ORG.Personel();
                    personeller[0].tcKimlikNo = formlar[i].yoneticiTCKimlikNo;
                    TNS.ORG.OrganizasyonBirim ob = new ORG.OrganizasyonBirim();

                    ob.muhasebekodu = form.saymanlik.ToString();
                    ob.muhasebebirim = form.birim;
                    ob.harcamaMi = 1;

                    ObjectArray oa = servisORG.OrganizasyonBirimListele(ob, false, 0, false);
                    if (!oa.sonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return oa.sonuc;
                    }

                    if (oa.ObjeSayisi == 0)
                    {
                        ve.VeriErisimKapat();
                        oa.sonuc.DegerAta("Saymanlık birim organizyon şemasında bulunamadı!");
                        return oa.sonuc;
                    }
                    TNS.ORG.BirimGenelGorev bgg = new ORG.BirimGenelGorev();
                    bgg.OrgKod = 0;
                    bgg.siraNo = genelGorevKod;
                    Sonuc gorevAtaSonuc = servisORG.GenelGorevKisileriEkle(kullanan, (TNS.ORG.OrganizasyonBirim)oa.objeler[0], bgg, personeller);
                    if (!gorevAtaSonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return gorevAtaSonuc;
                    }
                }
            }

            TABKYMYONETICIBILDIRIMFORMU tab = new TABKYMYONETICIBILDIRIMFORMU(ve);

            ve.BeginTransaction();
            for (int i = 0; i < formlar.Length; i++)
            {
                formlar[i].durum = durum;
                sonuc = tab.DurumGuncelle(formlar[i], kullanan.kullaniciKodu);
                if (!sonuc.islemSonuc) break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        public static YoneticiBildirimFormu Get(VeriErisim veriErisim, Kullanici kullanan, YoneticiBildirimFormu form)
        {
            Sonuc sonuc = AreKeyFieldsOK(kullanan, form);
            if (!sonuc.islemSonuc)
            {
                throw new Exception(sonuc.hataStr);
            }


            YoneticiBildirimFormu[] s = List(veriErisim, kullanan, form);
            if (s == null || s.Length == 0) return null;
            return s[0];
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool HasReadAccess(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            TNS.MUH.IMUHServis servisMuh = TNS.MUH.Arac.Tanimla();
            bool yetkiVar = servisMuh.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlik.ToString(), form.birim, MUH.ENUMOGY.Gorme);
            if (yetkiVar) return yetkiVar;

            TNS.ORG.IORGServis servisOrg = TNS.ORG.Arac.Tanimla();
            yetkiVar = servisOrg.MuhasebeBirimYetkiVarMi(kullanan, form.saymanlik.ToString(), form.birim, ORG.ENUMOGY.Gorme);
            return yetkiVar;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AreKeyFieldsOK(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = "";

            if (form.saymanlik <= 0)
                hata = "Geçersiz saymanlık kodu girdiniz!<br>";

            if (string.IsNullOrWhiteSpace(form.birim))
                hata = "Geçersiz birim kodu girdiniz!<br>";

            if (form.talepNo <= 0)
                hata += "Geçersiz talep numarası girdiniz!<br>";
            if (hata != "")
                sonuc.DegerAta(hata);
            return sonuc;
        }

        /// <summary>
        /// YoneticiBildirimFormu listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        public static YoneticiBildirimFormu[] List(VeriErisim veriErisim, Kullanici kullanan, YoneticiBildirimFormu form)
        {
            if (!HasReadAccess(kullanan, form))
            {
                throw new Exception(KullaniciResI.YoneticiBildirimFormu002);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABKYMYONETICIBILDIRIMFORMU tabKYMYONETICIBILDIRIMFORMU = new TABKYMYONETICIBILDIRIMFORMU(ve);

            IDataReader dr = tabKYMYONETICIBILDIRIMFORMU.List(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList alAna = new ArrayList();
            while (dr.Read())
            {
                YoneticiBildirimFormu detay = new YoneticiBildirimFormu();
                detay.saymanlik = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.SAYMANLIK, 0);
                detay.birim = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.BIRIM, "");
                detay.talepNo = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.TALEPNO, 0);
                detay.talepTarihi = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.TALEPTARIHI);
                detay.yoneticiTCKimlikNo = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.YONETICITCKIMLIKNO, "");
                detay.durum = (ENUMYoneticiBildirimFormuDurum)VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.DURUM, 0);
                detay.islemYapan = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.ISLEMYAPAN, "");
                detay.islemOnaylayan = VeriErisim.DegerAl(dr, (int)TABKYMYONETICIBILDIRIMFORMU.ENUMAlanlar.ISLEMONAYLAYAN, "");
                string ad = VeriErisim.DegerAl(dr, 8, "");
                string soyad = VeriErisim.DegerAl(dr, 9, "");
                detay.yoneticiAdSoyad = ad + ' ' + soyad;
                alAna.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (YoneticiBildirimFormu[])alAna.ToArray(typeof(YoneticiBildirimFormu));
        }
    }
}
