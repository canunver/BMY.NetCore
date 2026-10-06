using System;
using System.Data;
using TNS.KYM;
using TNS.KYM.IsKurallari.Veri;
using TNS.Veri;
using TNS.UZY;

namespace TNS.KYM.IsKurallari
{
    public class YerleskeOdaServis
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriKYM";

        internal static YerleskeOda Oku(VeriErisim veriErisim, Kullanici kullanan, int odaKod)
        {
            if (odaKod == 0) return null;

            YerleskeOda oda = new YerleskeOda();
            if (odaKod < 0)
            {
                oda.kod = -1;
            }
            else
            {
                oda.kod = odaKod;
                ObjectArray oa = Listele(veriErisim, kullanan, oda);
                if (!oa.sonuc.islemSonuc) return null;
                if (oa.ObjeSayisi == 0) return null;
                oda = (YerleskeOda)oa.objeler[0];
            }
            return oda;
        }

        internal static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, YerleskeOda form)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYERLESKEODA tabYerleskeOda = new TABYERLESKEODA(ve);

            var merkezBankasi = TMM.Arac.MerkezBankasiKullaniyor();

            IDataReader dr = tabYerleskeOda.Listele(form, merkezBankasi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            int siraNo = (int)TABYERLESKEODA.ENUMAlanlar.SIRANO;
            while (dr.Read())
            {
                YerleskeOda detay = new YerleskeOda();
                detay.kod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.KOD, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.AD, "");
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    detay.gorKodKisa = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.GORKOD, "");
                else
                    detay.gorKodKisa = YerleskeOda.SonUc(VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.GORKOD, ""));

                detay.tarif = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.TARIF, "");
                detay.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.SAYMANLIKKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.BIRIMKOD, "");
                detay.altDalVar = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.ALTDALVAR, 0) == 1;
                detay.ustKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.USTKOD, 0);
                detay.yerTurKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.YERTURKOD, 0);
                detay.odaTurKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.ODATURKOD, 0);
                detay.ilKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.ILKOD, "");
                detay.orgKod = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.ORGKOD, 0);
                detay.durum = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.DURUM, 0);
                detay.maliyetMerkezi = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.MALIYETMERKEZI, 0);
                detay.gorKodEski = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.GORKODESKI, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABYERLESKEODA.ENUMAlanlar.SIRANO, 0);

                detay.ustAd = VeriErisim.DegerAl(dr, siraNo + 1, "");
                detay.ustGorKod = VeriErisim.DegerAl(dr, siraNo + 2, "");
                detay.ustTarif = VeriErisim.DegerAl(dr, siraNo + 3, "");
                detay.ilAd = VeriErisim.DegerAl(dr, siraNo + 4, "");
                detay.orgAd = VeriErisim.DegerAl(dr, siraNo + 5, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, YerleskeOda form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma hakkınız yok");
                return s;
            }

            s = AlanlarGecerliMi(kullanan, form);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYERLESKEODA tabYerleskeOda = new TABYERLESKEODA(ve);

            ve.BeginTransaction();

            if (form.kod == 0)
            {
                form.kod = tabYerleskeOda.EnYuksekKodBul();
                form.altDalVar = false;
                if (!string.IsNullOrWhiteSpace(ve.hata)) s.DegerAta(ve.hata);
                if (s.islemSonuc)
                    s = tabYerleskeOda.Ekle(form);
                if (s.islemSonuc && form.ustKod > 0)
                    s = tabYerleskeOda.AltDalliYap(form.ustKod);
            }
            else if (form.durum == -2)//PASİF
            {
                s = tabYerleskeOda.PasifYap(form);
            }
            else
            {
                s = tabYerleskeOda.Guncelle(form);
            }

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                s = tabYerleskeOda.SeviyeGuncelle(form.kod);

            if (s.islemSonuc)
            {
                s.anahtar = form.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc GuncelleUstKod(VeriErisim veriErisim, Kullanici kullanan, YerleskeOda form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Yazma hakkınız yok");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABYERLESKEODA tabYerleskeOda = new TABYERLESKEODA(ve);


            bool hedefAltDalVar = tabYerleskeOda.AltDalVarMi(form.ustKod);

            ve.BeginTransaction();

            if (form.kod != 0)
            {
                s = tabYerleskeOda.GuncelleUstKod(form);

                if (s.islemSonuc && !hedefAltDalVar && form.ustKod > 0)
                    s = tabYerleskeOda.AltDalliYap(form.ustKod);
            }
            if (s.islemSonuc)
            {
                s.anahtar = form.kod.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc AlanlarGecerliMi(Kullanici kullanan, YerleskeOda form)
        {
            Sonuc s = new Sonuc();
            if (string.IsNullOrWhiteSpace(form.gorKodKisa))
                s.DegerAta("Oda kodu boş bırakılamaz!");
            else if (!TNS.TMM.Arac.MerkezBankasiKullaniyor() && form.gorKodKisa.Length != 3)
                s.DegerAta("Oda kodu 3 hane olmalıdır!");
            else if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && form.gorKodKisa.Length > 50)
                s.DegerAta("Oda kodu en fazla 50 hane olmalıdır!");

            if (string.IsNullOrWhiteSpace(form.ad))
                s.DegerAta("Oda adı boş bırakılamaz!");
            if (form.kod <= 0 && form.ustKod == 0)
                s.DegerAta("Hatalı çağrı yapıldı, kod ve üst kod aynı anda boş olamaz!");
            if (form.yerTurKod <= 0)
                s.DegerAta("Tür alanını seçiniz!");
            else if (form.yerTurKod == 1)
            {
                if (string.IsNullOrWhiteSpace(form.ilKod))
                    s.DegerAta("Yerleşke için il seçiniz!");
            }
            else if (form.yerTurKod == 5)
            {
                if (form.odaTurKod == 0)
                    s.DegerAta("Oda için oda türünü seçiniz!");
                if (form.orgKod == 0)
                    s.DegerAta("Oda için odanın ait olduğu birimi seçiniz!");
            }
            return s;
        }

        private static bool YazmaYetkisiVarMi(Kullanici kullanan, YerleskeOda form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKAYDI)) return true;
            return false;
        }
    }
}
