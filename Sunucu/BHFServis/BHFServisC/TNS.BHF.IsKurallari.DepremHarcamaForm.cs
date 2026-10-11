using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
using TNS.BHF.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Collections.Generic;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// BHFDepremHarcamaForm formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHFDepremHarcamaForm
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
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, DepremHarcamaAna form, Sonuc sonuc)
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
        public static bool AlanlarGecerliMi(Kullanici kullanan, DepremHarcamaAna form, Sonuc sonuc)
        {
            if (AnahtarAlanlarGecerliMi(kullanan, form, sonuc))
            {
                form.RakamlarGecerliMi(sonuc);
            }
            return sonuc.islemSonuc;
        }

        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DepremHarcamaAna form)
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
            TABBHFDEPREMHARCAMADETAY tab = new TABBHFDEPREMHARCAMADETAY(ve);

            if (!OncekiAydanBuyukMu(ve, kullanan, form, s))
            {
                ve.VeriErisimKapat();
                return s;
            }

            DepremHarcamaAna okunanForm = FormKimlikOku(tab, form, s);
            if (s.islemSonuc && okunanForm != null)
            {
                if (okunanForm.durum >= 5)
                    s.DegerAta("Form onaylı olduğundan değiştiremezsiniz!");
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            ve.BeginTransaction();

            if (okunanForm != null)
            {
                if (s.islemSonuc)
                    s = tab.Sil(form);
            }
            if (s.islemSonuc)
            {
                if (okunanForm == null)
                    s = tab.EkleAna(form, kullanan.kullaniciKodu);
                else
                    s = tab.GuncelleAna(form, kullanan.kullaniciKodu);
            }
            for (int i = 0; i < form.depremHarcamaDetay.Count && s.islemSonuc; i++)
            {
                DepremHarcamaDetay det = (DepremHarcamaDetay)form.depremHarcamaDetay[i];
                if (det.ekoKod.Length > 2 && det.RakamVarMi())
                    s = tab.Ekle(form, det);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static bool OncekiAydanBuyukMu(VeriErisim ve, Kullanici kullanan, DepremHarcamaAna bAyF, Sonuc s)
        {
            if (bAyF.ay <= 1) return s.islemSonuc;
            bAyF.ay = bAyF.ay - 1;
            ObjectVAN oAy = Oku(ve, kullanan, bAyF);
            bAyF.ay = bAyF.ay + 1;
            if (!oAy.sonuc.islemSonuc)
            {
                s.DegerAta(oAy.sonuc.hataStr);
                return s.islemSonuc;
            }
            DepremHarcamaAna oAyF = (DepremHarcamaAna)oAy.o;

            if (bAyF.depremHarcamaDetay.Count != oAyF.depremHarcamaDetay.Count)
            {
                s.DegerAta("Önceki ay ile bu ay arasındaki kayıt sayıları tutarsız olduğundan, önceki ay harcamalarını kapsıyor mu kontrolü yapılamadı");
            }
            else
            {
                for (int i = 0; i < bAyF.depremHarcamaDetay.Count; i++)
                {
                    DepremHarcamaDetay bAyD = (DepremHarcamaDetay)bAyF.depremHarcamaDetay[i];
                    if (string.IsNullOrWhiteSpace(bAyD.ekoKod) || bAyD.ekoKod.Length <= 2) continue;

                    DepremHarcamaDetay oAyD = (DepremHarcamaDetay)oAyF.depremHarcamaDetay[i];
                    if (bAyD.ekoKod != oAyD.ekoKod)
                    {
                        s.DegerAta($"Önceki ay kodu ({oAyD.ekoKod}-{oAyD.ad}) ile bu ay kodu ({bAyD.ekoKod}-{bAyD.ad}) farklı olduğundan, önceki ay harcamalarını kapsıyor mu kontrolü yapılamadı");
                        break;
                    }
                    else
                    {
                        List<string> farkliAlanlar = new List<string>();
                        if (bAyD.butceIci < oAyD.butceIci) farkliAlanlar.Add("Bütçe içi");
                        if (bAyD.hT1 < oAyD.hT1) farkliAlanlar.Add("Özel Hesap");
                        if (bAyD.hT2 < oAyD.hT2) farkliAlanlar.Add("Döner Sermaye");
                        if (bAyD.hT3 < oAyD.hT3) farkliAlanlar.Add("Fon");
                        if (bAyD.hT4 < oAyD.hT4) farkliAlanlar.Add("Sosyal Tesis");
                        if (bAyD.hT5 < oAyD.hT5) farkliAlanlar.Add("Diğer");
                        if (farkliAlanlar.Count != 0)
                        {
                            s.DegerAta($"{oAyD.ad.Trim()} harcaması, önceki ay değerinden, '{String.Join(", ", farkliAlanlar)}' {(farkliAlanlar.Count == 1 ? "alanında" : "alanlarında")} küçük olduğu için kaydedilemez. Harcamalar kümülatif olarak girilmelidir!");
                        }
                    }
                }
            }
            return s.islemSonuc;
        }

        private static DepremHarcamaAna FormKimlikOku(TABBHFDEPREMHARCAMADETAY tab, DepremHarcamaAna sorguForm, Sonuc sonuc)
        {
            IDataReader dr = tab.OkuAna(sorguForm);
            if (dr == null)
            {
                sonuc.DegerAta(tab.Hata());
                return null;
            }
            DepremHarcamaAna formSimd = null;
            if (dr.Read())
            {
                formSimd = new DepremHarcamaAna();

                int kol = 0;
                formSimd.yil = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.ay = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.muhasebeKod = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                formSimd.durum = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.kaydeden = VeriErisim.DegerAl(dr, kol++, "");
                formSimd.kayitTarihi = VeriErisim.DegerAl(dr, kol++);
                formSimd.onaylanan = VeriErisim.DegerAl(dr, kol++, "");
                formSimd.onayTarihi = VeriErisim.DegerAl(dr, kol++);
            }
            dr.Close();
            return formSimd;
        }

        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        public static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, DepremHarcamaAna form)
        {
            ObjectVAN ov = new ObjectVAN();
            if (AnahtarAlanlarGecerliMi(kullanan, form, ov.sonuc))
            {
                ObjectArray s = Listele(veriErisim, kullanan, form);
                ov.sonuc = s.sonuc;
                if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            }
            return ov;
        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, DepremHarcamaAna form, Sonuc sonuc)
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
        public static bool AnahtarAlanlarGecerliMi(Kullanici kullanan, DepremHarcamaAna form, Sonuc sonuc)
        {
            if (form.yil <= 0)
                sonuc.DegerAta("Lütfen yıl giriniz!");

            if (form.ay <= 0 || form.ay > 12)
                sonuc.DegerAta("Lütfen ay giriniz!");

            if (form.muhasebeKod <= 0)
                sonuc.DegerAta("Lütfen muhasebe giriniz!");

            if (string.IsNullOrWhiteSpace(form.birimKod) || form.birimKod.Length != 3)
                sonuc.DegerAta("Lütfen birim giriniz!");

            if (form.birimKod == "250" || form.birimKod == "000")
                sonuc.DegerAta("Bu form, ilçe müdürlükleri tarafından kullanılmayacaktır!");

            if (form.ilKod <= 0)
                sonuc.DegerAta("Lütfen harcamanın hangi il için yapıldığını seçiniz!");

            return sonuc.islemSonuc;
        }

        /// <summary>
        /// DepremHarcamaAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sorguForm">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DepremHarcamaAna sorguForm)
        {
            ObjectArray oa = new ObjectArray();
            if (!GormeYetkisiVarMi(kullanan, sorguForm, oa.sonuc))
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFDEPREMHARCAMADETAY tabBHFDEPREMHARCAMADETAY = new TABBHFDEPREMHARCAMADETAY(ve);
            IDataReader dr = tabBHFDEPREMHARCAMADETAY.ListeleDepremKodlari();
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            List<StrKodStrAd> depKodlar = new List<StrKodStrAd>();
            depKodlar.Add(new StrKodStrAd("", "Toplam"));
            while (dr.Read())
            {
                string kod = VeriErisim.DegerAl(dr, 0, "");
                depKodlar.Add(new StrKodStrAd(kod, "".PadLeft(kod.Length, (char)160) + VeriErisim.DegerAl(dr, 1, "")));
            }
            dr.Close();
            dr = tabBHFDEPREMHARCAMADETAY.Listele(sorguForm);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            TNSCollection al = new TNSCollection();
            DepremHarcamaAna formAktif = null;
            while (dr.Read())
            {
                DepremHarcamaAna formSimd = new DepremHarcamaAna();

                int kol = 0;

                formSimd.yil = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.ay = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.muhasebeKod = VeriErisim.DegerAl(dr, kol++, 0);
                formSimd.birimKod = VeriErisim.DegerAl(dr, kol++, "");
                formSimd.ilKod = VeriErisim.DegerAl(dr, kol++, 0);

                if (!formSimd.Equals(formAktif))
                {
                    formAktif = formSimd;
                    formAktif.KodlariYaz(depKodlar);
                    oa.objeler.Add(formAktif);
                }
                formAktif.durum = VeriErisim.DegerAl(dr, kol++, 0);
                formAktif.kaydeden = VeriErisim.DegerAl(dr, kol++, "");
                formAktif.kayitTarihi = VeriErisim.DegerAl(dr, kol++);
                formAktif.onaylanan = VeriErisim.DegerAl(dr, kol++, "");
                formAktif.onayTarihi = VeriErisim.DegerAl(dr, kol++);
                if (formAktif.ilKod > 0)
                {

                    DepremHarcamaDetay detay = new DepremHarcamaDetay();
                    detay.ekoKod = VeriErisim.DegerAl(dr, kol++, "");
                    detay.butceIci = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.butceIciAciklama = VeriErisim.DegerAl(dr, kol++, "");
                    detay.hT1 = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.hT2 = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.hT3 = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.hT4 = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.hT5 = VeriErisim.DegerAl(dr, kol++, 0.0);
                    detay.butceDisiAciklama = VeriErisim.DegerAl(dr, kol++, "");
                    formAktif.DepremHarcamaDetayBulEkle(detay);
                }
                else
                {
                    formAktif.ilKod = sorguForm.ilKod;
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (oa.objeler.Count == 0 && oa.sonuc.islemSonuc)
            {
                DepremHarcamaAna formSimd = new DepremHarcamaAna();
                formSimd.KodlariYaz(depKodlar);
                oa.objeler.Add(formSimd);
            }
            return oa;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, DepremHarcamaAna form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (form.durum == 1)
            {
                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                    return s;
                }
            }
            else if (form.durum == 5)
            {
                if (!YazmaYetkisiVarMi(kullanan, form, s))
                {
                    return s;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFDEPREMHARCAMADETAY tab = new TABBHFDEPREMHARCAMADETAY(ve);

            DepremHarcamaAna okunanForm = FormKimlikOku(tab, form, s);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (okunanForm == null)
                s = tab.OnayEkle(form, kullanan.kullaniciKodu);
            else
                s = tab.OnayGuncelle(form, kullanan.kullaniciKodu);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OncekiAydanKopyala(VeriErisim veriErisim, Kullanici kullanan, DepremHarcamaAna form)
        {
            Sonuc s = new Sonuc(true, "", "");

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHFDEPREMHARCAMADETAY tab = new TABBHFDEPREMHARCAMADETAY(ve);

            ve.BeginTransaction();

            s = tab.VeriKopyalaForm(form.yil, form.ay - 1, form.yil, form.ay);
            if(s.islemSonuc)
                s = tab.VeriKopyalaDetay(form.yil, form.ay - 1, form.yil, form.ay);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }
    }
}
