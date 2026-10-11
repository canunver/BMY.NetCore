using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;
using System.Collections.Generic;
using System.Security.Cryptography;
using TNS.BUM;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe tertiplerinde, faaliyet ve yatırım projeleriyle ilişkiyi kuran
    /// Alt Faaliyet boyutu ile ilgili iş kuralları yordamlarını tutan sınıf.
    /// </summary>
    public class MPDagitimServis
    {
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        internal static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MPDagitimAna form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YetkisiVarMi(kullanan, form, ENUMOGY.Yazma))
            {
                s.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
                return s;
            }
            AlanlarGecerliMi(kullanan, form, s);

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2MPDAGITIM tab = new TABBHMV2MPDAGITIM(ve);

            ve.BeginTransaction();
            if (form.id > 0)
                s = tab.Guncelle(form);
            else
            {
                s = tab.Ekle(form);
                form.id = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
                if (form.id <= 0) s.DegerAta("Kayıt sırasında anahtar alınamadı!");
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, MPDagitimAna form)
        {
            ObjectArray oa = new ObjectArray();
            if (!YetkisiVarMi(kullanan, form, ENUMOGY.Yazma))
            {
                oa.sonuc.DegerAta(ButceHazirlikResI.IOIProjeGiris001);
                return oa;
            }
            KriterlerGecerliMi(kullanan, form, oa.sonuc);

            if (!oa.sonuc.islemSonuc)
            {
                return oa;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMV2MPDAGITIM tab = new TABBHMV2MPDAGITIM(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    MPDagitimAna mp = new MPDagitimAna();
                    int kol = 0;
                    mp.id = VeriErisim.DegerAl(dr, kol++, 0);
                    mp.yil = VeriErisim.DegerAl(dr, kol++, 0);
                    mp.kur = VeriErisim.DegerAl(dr, kol++, "");
                    mp.fon = VeriErisim.DegerAl(dr, kol++, "");
                    mp.fin = VeriErisim.DegerAl(dr, kol++, "");
                    mp.eko = VeriErisim.DegerAl(dr, kol++, "");
                    mp.peb = VeriErisim.DegerAl(dr, kol++, "");
                    mp.aciklama = VeriErisim.DegerAl(dr, kol++, "");
                    mp.miktar = VeriErisim.DegerAl(dr, kol++, 0.0);
                    mp.kurAd = VeriErisim.DegerAl(dr, kol++, "");
                    mp.fonAd = VeriErisim.DegerAl(dr, kol++, "");
                    mp.finAd = VeriErisim.DegerAl(dr, kol++, "");
                    mp.ekoAd = VeriErisim.DegerAl(dr, kol++, "");
                    mp.pebAd = VeriErisim.DegerAl(dr, kol++, "");
                    oa.Ekle(mp);
                }
                dr.Close();
                if (form.id > 0 && oa.ObjeSayisi == 1)
                {
                    MPDagitimAna tek = (MPDagitimAna)oa.Al(0);
                    dr = tab.ListeleDetay(tek.id);
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            MPDagitimDetay dty = new MPDagitimDetay();
                            int kol = 1;
                            dty.birimKod = OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(tek.yil, 15, false), VeriErisim.DegerAl(dr, kol++, ""));
                            dty.miktar = VeriErisim.DegerAl(dr, kol++, 0.0);
                            dty.buYilDeger = VeriErisim.DegerAl(dr, kol++, 0.0);
                            dty.oncYilDeger = VeriErisim.DegerAl(dr, kol++, 0.0);
                            dty.birimAd = VeriErisim.DegerAl(dr, kol++, "");

                            tek.detay.Add(dty);
                        }
                        dr.Close();
                    }
                }
            }
            else
                oa.sonuc.DegerAta("Veri okunamadı:" + tab.Hata());

            ve.VeriErisimKapat();
            return oa;
        }

        private static void KriterlerGecerliMi(Kullanici kullanan, MPDagitimAna form, Sonuc sonuc)
        {
            if (form.yil < 2000 && form.id <= 0) sonuc.DegerAta("Listeme için form kodu veya yıl giriniz!");
        }

        internal static ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, int id)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray oa = Listele(veriErisim, kullanan, new MPDagitimAna() { id = id });
            if (!oa.sonuc.islemSonuc)
                ov.sonuc = oa.sonuc;
            else if (oa.ObjeSayisi == 1)
            {
                ov.o = oa.Al(0);
            }
            else
                ov.sonuc.DegerAta("Aranan kayıt bulunamadı!");
            return ov;
        }

        private static void AlanlarGecerliMi(Kullanici kullanan, MPDagitimAna form, Sonuc s)
        {
            if (form.yil < 2000) s.DegerAta("Geçerli bir yıl giriniz!");
            if (string.IsNullOrWhiteSpace(form.aciklama)) s.DegerAta("Açıklama giriniz!");
            if (form.miktar < 1) s.DegerAta("Geçerli miktar giriniz!");
        }

        private static bool YetkisiVarMi(Kullanici kullanan, MPDagitimAna form, ENUMOGY yazma)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            return false;
        }

        internal static ObjectVAN BirimEkle(VeriErisim veriErisim, Kullanici kullanan, int id, string birimKod, double tutar)
        {
            ObjectVAN ov = new ObjectVAN();
            if (id <= 0)
                ov.sonuc.DegerAta("Lütfen formu önce kaydedeniz!");
            if (string.IsNullOrEmpty(birimKod))
                ov.sonuc.DegerAta("Lütfen birim seçiniz!");
            if (!YetkisiVarMi(kullanan, null, ENUMOGY.Yazma))
                ov.sonuc.DegerAta("Bu işlemi yapma yetkiniz bulunmamaktadır!");
            if (!ov.sonuc.islemSonuc)
                return ov;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ovOkunan = Oku(null, kullanan, id);
            if (!ovOkunan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ovOkunan;
            }

            MPDagitimAna mpAna = (MPDagitimAna)ovOkunan.o;

            Tertip kistas = new Tertip(mpAna.yil, ENUMTabloKod.TABTasraOdenek, mpAna.kur, mpAna.fon, mpAna.fin, mpAna.eko, mpAna.peb, birimKod);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(5), kistas.KodNoKirilimSay(5)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[2];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 1, 0, "5");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");

            OdenekBilgi[] obler = BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(ve, kullanan, kistas, kodBilgileri, ekBilgiler, true, false, true, -1);

            MPDagitimDetay detay;
            detay = new MPDagitimDetay();
            detay.miktar = tutar;
            detay.birimKod = birimKod;
            if (obler != null && obler.Length > 0)
            {
                detay.birimAd = obler[0].tertip.adlar[5];
                detay.oncYilDeger = obler[0].EkRakam(0);
                detay.buYilDeger = obler[0].EkRakam(1);
            }
            else if (tutar > 0)
            {
                detay.birimAd = "";
                detay.oncYilDeger = 0;
                detay.buYilDeger = 0;
            }
            else
            {
                detay = null;
                ve.VeriErisimKapat();
                ov.sonuc.DegerAta("Birime ait veri bulunamadı!");
                return ov;
            }

            TABBHMV2MPDAGITIM tab = new TABBHMV2MPDAGITIM(ve);
            ve.BeginTransaction();
            ov.sonuc = tab.EkleDetay(id, detay);
            if (ov.sonuc.islemSonuc)
            {
                ve.Commit();
                ov.o = detay;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return ov;
        }

        internal static Sonuc RevizeUret(VeriErisim veriErisim, Kullanici kullanan, int id, int ay)
        {
            Sonuc s = new Sonuc();
            ObjectVAN ov = new ObjectVAN();
            if (id <= 0)
                s.DegerAta("Lütfen formu önce kaydedeniz!");
            if (!YetkisiVarMi(kullanan, null, ENUMOGY.Yazma))
                s.DegerAta("Bu işlemi yapma yetkiniz bulunmamaktadır!");
            if (!s.islemSonuc)
                return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectVAN ovOkunan = Oku(null, kullanan, id);
            if (!ovOkunan.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ovOkunan.sonuc;
            }
            TertipOdenek merkezMerkez = null;
            MPDagitimAna mpAna = (MPDagitimAna)ovOkunan.o;
            if (mpAna.detay.Count == 0)
                s.DegerAta("Formun detayına birim ekleyiniz");
            else
            {
                TertipOdenek[] merkezMerkezler = MasrafPlani.MPTertipListeleAy(null, kullanan, new Tertip(mpAna.yil, ENUMTabloKod.TABPEBOdenek, mpAna.kur, mpAna.fon, mpAna.fin, mpAna.eko, mpAna.peb), "90000000000", false, false);
                if (merkezMerkezler.Length == 0)
                    s.DegerAta("Merkez merkez tertibi bulunamadı!");
                else if (merkezMerkezler.Length > 1)
                    s.DegerAta("Birden fazla merkez merkez tertibi bulundu!");
                else
                    merkezMerkez = merkezMerkezler[0];
            }

            if (s.islemSonuc)
            {
                double toplam = 0;
                double[] dizi = new double[12];
                int sonIndeks = -1;
                for (int i = 0; i < 12; i++)
                {
                    if (i < ay)
                        dizi[i] = 0;
                    else
                    {
                        dizi[i] = merkezMerkez.odenek[i];
                        toplam += dizi[i];
                        if (dizi[i] > 0) sonIndeks = i;
                    }
                }
                MPRevizeForm form = new MPRevizeForm();
                form.yil= mpAna.yil;
                form.aciklama = mpAna.aciklama;
                form.evrakTarih = new TNSDateTime(DateTime.Now.Date);
                form.revizeFormTip = ENUMRevizeFormTip.MasrafPlaniRevize;
                form.formDurum = ENUMRevizeFormDurum.Musvette;
                form.altIslemKod = "02.02.04";
                List<MPRevizeSatir> satirlar = new List<MPRevizeSatir>();
                MPRevizeSatir merkezSatir = new MPRevizeSatir();
                merkezSatir.tertip = new Tertip(mpAna.yil, ENUMTabloKod.TABMasrafPlani);
                merkezSatir.tertip.TertipAta(mpAna.tertip + "-" + "90000000000");
                for (int i = 0; i < merkezSatir.odenek.Length; i++)
                {
                    merkezSatir.odenek[i] = 0;
                }
                satirlar.Add(merkezSatir);
                foreach (MPDagitimDetay det in mpAna.detay)
                {
                    MPRevizeSatir satir = new MPRevizeSatir();
                    satir.tertip = new Tertip(mpAna.yil, ENUMTabloKod.TABMasrafPlani);
                    satir.tertip.TertipAta(mpAna.tertip + "-" + det.birimKod);
                    satir.odenek = OrtakFonksiyonlar.OranliDagit(dizi, sonIndeks, toplam, det.miktar, 0);
                    for (int i = 0; i < satir.odenek.Length; i++)
                    {
                        merkezSatir.odenek[i] += satir.odenek[i];
                    }
                    satir.IOIISKOD = 0;
                    satirlar.Add(satir);
                }

                for (int i = 0; i < merkezSatir.odenek.Length; i++)
                {
                    if (merkezSatir.odenek[i] > dizi[i])
                        s.DegerAta($"{i + 1}. ay için dağıtılan ({merkezSatir.odenek[i].ToString("#,##0")}), merkez ödeneğinden ({dizi[i].ToString("#,##0")}) fazla oldu!");
                    merkezSatir.odenek[i] = 0 - merkezSatir.odenek[i];
                }

                form.satirlar = satirlar.ToArray();

                s = MasrafPlaniRevize.MPRevizeFormKaydet(null, kullanan, form, false, false);
            }

            ve.VeriErisimKapat();
            return s;
        }
    }
}