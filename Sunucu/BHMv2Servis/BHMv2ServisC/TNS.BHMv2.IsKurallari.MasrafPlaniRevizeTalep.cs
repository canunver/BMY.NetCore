using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using System.Collections;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    public class MasrafPlaniRevizeTalep
    {
        static string VERIERISIMAD = "BaglantiSatiriBUM";
        private static bool YazmaYetkisiVarMiForm(Kullanici kullanan, int yil, KurumsalYapi kurumsalYapi)
        {
            return IsKurallari.MasrafPlani.YazmaYetkisiVarMi(kullanan, yil, kurumsalYapi, ENUMMasrafPlanAsama.TALEP, "");
        }

        internal static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MPRevizeTalep form, bool elleGiris)
        {
            Sonuc s = new Sonuc();
            if (form.satirlar.Length == 0)
            {
                s.DegerAta("Kaydedilecek satır bulunamadığından, kayıt yapılamadı!");
            }
            form.kurumsalYapi = BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, form.kurumsalYapi.kod);
            if (!YazmaYetkisiVarMiForm(kullanan, form.yil, form.kurumsalYapi))
            {
                s.DegerAta("Forma Kaydetme yetkiniz olmadığından kayıt yapılamadı!");
            }

            if (!form.AlanlarGecerliMi(s))
            {
                return s;
            }

            if (elleGiris)
            {
                foreach (MPRevizeSatir d in form.satirlar)
                {
                    if (TNS.DEG.Arac.OdenekGondermeYasakliMi(d.tertip.kodlar[3]))
                    {
                        s.DegerAta(string.Format("Verilen ekonomik kod ({0}) için, talep işlemini harcama yönetiminden yapmalısınız!", d.tertip.kodlar[3]));
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            if (MasrafPlani.OnayDurumBul(ve, form.yil, "", ENUMMasrafPlanAsama.AHP, "000") != 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Masraf planı onaylı olmadığından, revize talep işlemi yapılamaz!");
                return s;
            }

            TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);
            MPRevizeTalep eskiForm = null;
            if (form.revizeTalepNo > 0)
            {
                try
                {
                    eskiForm = Oku(ve, kullanan, form.yil, form.kurumsalYapi.kod, form.revizeTalepNo);
                    if (eskiForm == null)
                    {
                        ve.VeriErisimKapat();
                        s.DegerAta("Eski form bulunamadı!");
                        return s;
                    }
                }
                catch (Exception ex)
                {
                    ve.VeriErisimKapat();
                    s.DegerAta("Eski form okunamadı:" + ex.Message);
                    return s;
                }
                if (!eskiForm.DegistirilebilirMi())
                {
                    ve.VeriErisimKapat();
                    s.DegerAta("Onaylanmış yada İptal edilmiş formların durumunu değiştiremezsiniz!");
                    return s;
                }
            }
            if (s.islemSonuc)
                MasrafPlaniRevize.TekrarliSatirVarMi(form.satirlar, s);

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            ve.BeginTransaction();
            if (eskiForm != null)
            {
                s = tab.RevizeTalepDetaySil(form.yil, form.kurumsalYapi.kod, form.revizeTalepNo);
                if (s.islemSonuc)
                    s = tab.RevizeTalepSil(form.yil, form.kurumsalYapi.kod, form.revizeTalepNo);
            }
            else
            {
                form.revizeTalepNo = tab.RevizeTalepSiraNoBul(form.yil, form.kurumsalYapi.kod);
                if (form.revizeTalepNo <= 0)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    s.DegerAta("Sıra numarası alınamadı!");
                    return s;
                }
            }
            TNS.UZY.IUZYServis servisUZY = UZY.Arac.Tanimla();
            s = tab.RevizeTalepEkle(form, kullanan.kullaniciKodu);
            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                if (!form.satirlar[i].tertip.TamMi())
                    s.DegerAta("Hatalı Tertip:" + form.satirlar[i].tertip.TertipStrOl());
                form.satirlar[i].tertip.AdlariBul(servisUZY);
                string kodAd;
                string kod;
                if (!form.satirlar[i].tertip.IsimlerTamMi(out kodAd, out kod))
                    s.DegerAta("Hatalı Tertip, tanımsız kısım: (" + kodAd + ", " + kod + ")");

                for (int ay = 0; ay < 12 && s.islemSonuc; ay++)
                {
                    s = tab.RevizeTalepDetayEkle(form, form.satirlar[i], ay, true);
                }
            }

            if (s.islemSonuc)
            {
                s.anahtar = form.revizeTalepNo.ToString();
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static MPRevizeTalep Oku(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalYapiKod, int revizeTalepNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            MPRevizeTalep form = new MPRevizeTalep();
            form.yil = yil;
            form.kurumsalYapi = new KurumsalYapi();
            form.kurumsalYapi.kod = kurumsalYapiKod;
            form.revizeTalepNo = revizeTalepNo;
            MPRevizeTalep[] formlar = null;
            try
            {
                formlar = Listele(ve, kullanan, form);
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                throw ex;
            }
            if (formlar == null || formlar.Length != 1)
            {
                ve.VeriErisimKapat();
                throw new Exception("Form bulunamadı!");
            }
            try
            {
                formlar[0].satirlar = SatirlariOku(ve, formlar[0], true);
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                throw ex;
            }
            return formlar[0];
        }

        private static MPRevizeSatir[] SatirlariOku(VeriErisim veriErisim, MPRevizeTalep form, bool sadeceTalepler)
        {
            ArrayList al = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);
            TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);
            IDataReader dr = tab.TalepDetayOku(form.yil, form.kurumsalYapi.kod, form.revizeTalepNo, sadeceTalepler);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Okuma Hatası: " + ve.hata);
            }
            string eskiTertipStr = "";
            MPRevizeSatir satir = null;
            while (dr.Read())
            {
                Tertip icTertip = new Tertip(form.yil, ENUMTabloKod.TABMasrafPlani);
                icTertip.TertipAta(
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.KURKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.FONKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.FINKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.EKOKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.PEBKOD, ""),
                    VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.KYKOD, "")
                );
                string yeniTertipStr = icTertip.TertipStrOl();
                int talep = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.TALEPMI, 0);
                int ay = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.AY, 0);
                if (yeniTertipStr != eskiTertipStr)
                {
                    satir = new MPRevizeSatir();
                    satir.tertip = icTertip;
                    satir.tertip.Noktala();
                    if (!sadeceTalepler)
                        satir.karsilananOdenek = new double[12];
                    al.Add(satir);
                    eskiTertipStr = yeniTertipStr;
                }
                double odenek = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEPTERTIP.ODENEK, 0.0);
                if (talep == 1)
                    satir.odenek[ay - 1] = odenek;
                else
                    satir.karsilananOdenek[ay - 1] = odenek;
            }
            dr.Close();
            ve.VeriErisimKapat();

            return (MPRevizeSatir[])al.ToArray(typeof(MPRevizeSatir));
        }

        internal static MPRevizeTalep[] Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, MPRevizeTalep kistasForm)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);

            ArrayList al = new ArrayList();
            IDataReader dr = tab.RevizeTalepListele(kistasForm);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            while (dr.Read())
            {
                MPRevizeTalep f = new MPRevizeTalep();
                f.yil = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.YIL, 0);
                f.kurumsalYapi = new KurumsalYapi();
                f.kurumsalYapi.kod = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.KYKOD, "");
                f.revizeTalepNo = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.REVIZETALEPNO, 0);
                f.evrakTarih = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.EVRAKTARIH);
                f.evrakNo = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.EVRAKNO, "");
                f.evrakIlgi = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.EVRAKILGI, "");
                f.talepAciklama = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.TALEPACIKLAMA, "");
                f.cevapAciklama = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.CEVAPACIKLAMA, "");
                f.formDurum = (ENUMRevizeFormDurum)VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.FORMDURUM, 0);
                f.kaydedenKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.KAYDEDEN, "");
                f.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.KAYITTARIH);
                f.onaylayanKullaniciKodu = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.ONAYLAYAN, "");
                f.onaylamaTarihi = VeriErisim.DegerAl(dr, (int)TABMPGREVIZETALEP.ENUMAlanlarREVIZETALEP.ONAYTARIH);
                f.talepToplam = VeriErisim.DegerAl(dr, 13, 0.0);
                f.karsilananToplam = VeriErisim.DegerAl(dr, 14, 0.0);
                al.Add(f);
            }
            dr.Close();
            if (kistasForm.detayGetir)
            {
                foreach (MPRevizeTalep f in al)
                {
                    f.satirlar = SatirlariOku(ve, f, false);
                }
            }
            ve.VeriErisimKapat();
            return (MPRevizeTalep[])al.ToArray(typeof(MPRevizeTalep));
        }

        internal static Sonuc DurumAta(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalYapiKod, int revizeTalepNo, ENUMRevizeFormDurum durum)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            try
            {
                MPRevizeTalep form = Oku(veriErisim, kullanan, yil, kurumsalYapiKod, revizeTalepNo);
                if (form.DegistirilebilirMi())
                {
                    TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);
                    ve.BeginTransaction();
                    s = tab.RevizeTalepDurumDegistir(yil, kurumsalYapiKod, revizeTalepNo, durum);
                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }
                else
                    s.DegerAta("Formun durumu değiştirilemez!");
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc[] MPRevizeVeyaTalepYap(VeriErisim veriErisim, Kullanici kullanan, MPRevizeTalep form, int eskiTalepNo, int eskiRevizeNo)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            MPRevizeForm revizeForm = null;
            if (eskiRevizeNo > 0)
            {
                try
                {
                    revizeForm = IsKurallari.MasrafPlaniRevize.MPRevizeFormOku(ve, kullanan, form.yil, eskiRevizeNo);
                    if (!revizeForm.DegistirilebilirMi())
                        revizeForm.revizeNo = 0;
                }
                catch { }
            }

            if (revizeForm == null)
            {
                revizeForm = new MPRevizeForm();
                revizeForm.revizeFormTip = ENUMRevizeFormTip.MasrafPlaniRevize;
                revizeForm.yil = form.yil;
                revizeForm.revizeNo = 0;
            }

            MPRevizeTalep talepForm = null;
            if (eskiTalepNo > 0)
            {
                try
                {
                    talepForm = IsKurallari.MasrafPlaniRevizeTalep.Oku(ve, kullanan, form.yil, form.kurumsalYapi.kod, eskiTalepNo);
                    if (!talepForm.DegistirilebilirMi())
                        talepForm.revizeTalepNo = 0;
                }
                catch { }
            }

            if (talepForm == null)
            {
                talepForm = new MPRevizeTalep();
                talepForm.yil = form.yil;
                talepForm.kurumsalYapi = new TNS.BHMv2.KurumsalYapi();
                talepForm.revizeTalepNo = 0;
                talepForm.kurumsalYapi.kod = form.kurumsalYapi.kod;
                talepForm.evrakNo = string.Empty;
            }

            talepForm.evrakTarih = form.evrakTarih;
            talepForm.talepAciklama = form.talepAciklama;
            talepForm.formDurum = ENUMRevizeFormDurum.Iletildi;
            revizeForm.aciklama = form.talepAciklama;
            revizeForm.evrakTarih = form.evrakTarih;
            revizeForm.formDurum = ENUMRevizeFormDurum.Onayli;

            int eklenenSatir = -1;
            ArrayList revizeSatirlar = new ArrayList();
            ArrayList talepSatirlar = new ArrayList();
            while (true)
            {
                eklenenSatir = MPRevizeSatir.EklenenSatirBul(eklenenSatir, form.satirlar);
                if (eklenenSatir < 0)
                    break;
                int dusulenSatir = -1;
                while (true)
                {
                    dusulenSatir = MPRevizeSatir.DusulenUyumluSatirBul(dusulenSatir, form.satirlar, form.satirlar[eklenenSatir]);
                    if (dusulenSatir < 0)
                        break;
                    if (IsKurallari.TertipTanim.TertipDegisimIzniVarMi(ve, kullanan, form.satirlar[eklenenSatir].tertip, form.satirlar[dusulenSatir].tertip))
                    {
                        DetayaEkle(revizeSatirlar, form.satirlar[eklenenSatir], form.satirlar[dusulenSatir]);
                    }
                    else
                    {
                        DetayaEkle(talepSatirlar, form.satirlar[eklenenSatir], form.satirlar[dusulenSatir]);
                    }
                }
                double kalanSatir = form.satirlar[eklenenSatir].NetToplam();
                if (kalanSatir > 0)
                    DetayaEkle(talepSatirlar, form.satirlar[eklenenSatir], null);
            }
            Sonuc[] sler = new Sonuc[2];
            sler[0] = null;
            sler[1] = null;
            if (revizeSatirlar.Count > 0)
            {
                revizeForm.satirlar = (MPRevizeSatir[])revizeSatirlar.ToArray(typeof(MPRevizeSatir));
                sler[0] = SatirlariAyarla(ve, revizeForm.satirlar, true, form.kurumsalYapi.kod);
                if (sler[0].islemSonuc)
                    sler[0] = IsKurallari.MasrafPlaniRevize.MPRevizeFormKaydet(ve, kullanan, revizeForm, true, false);
                else
                {
                    ve.VeriErisimKapat();
                    return sler;
                }

            }
            if (talepSatirlar.Count > 0)
            {
                talepForm.satirlar = (MPRevizeSatir[])talepSatirlar.ToArray(typeof(MPRevizeSatir));
                sler[1] = SatirlariAyarla(ve, talepForm.satirlar, false, form.kurumsalYapi.kod);
                if (sler[1].islemSonuc)
                    sler[1] = Kaydet(ve, kullanan, talepForm, false);
            }
            ve.VeriErisimKapat();
            return sler;
        }

        private static Sonuc SatirlariAyarla(VeriErisim ve, MPRevizeSatir[] satirlar, bool revizeMi, string kurumsalYapiKod)
        {
            Sonuc s = new Sonuc(true, "", "");
            for (int i = 0; i < satirlar.Length; i++)
            {
                if (string.IsNullOrEmpty(satirlar[i].tertip.kodlar[5]))
                    satirlar[i].tertip.kodlar[5] = kurumsalYapiKod;
                if (satirlar[i].NetToplam() < 0)
                {
                    double[] mpBilgi = IsKurallari.MasrafPlani.MasrafPlaniOdenekBilgiBul(ve, satirlar[i].tertip, false, 0);
                    if (!AylardanHarcananlariDus(mpBilgi))
                    {
                        s.DegerAta(string.Format("Masraf planı ayarlaması yapılırken, {0} tertibinde, harcama ({1}) ve bloke ({2}) rakamları, masraf planından ({3}) çıkartılamadı!", satirlar[i].tertip.TertipStrOl(), mpBilgi[2].ToString("#,##0"), mpBilgi[3].ToString("#,##0"), mpBilgi[0].ToString("#,##0")));
                        return s;
                    }

                    for (int ay = 0; ay < 12; ay++)
                    {
                        int mpAy = ay + 6;
                        double fark = mpBilgi[mpAy] + satirlar[i].odenek[ay];
                        if (fark < 0)
                        {
                            satirlar[i].odenek[ay] = 0 - mpBilgi[mpAy];
                            int eklenenSatir = -1;
                            double tutFark = fark;
                            while (tutFark < 0)
                            {
                                eklenenSatir = MPRevizeSatir.AyaEklenenUyumluSatirBul(eklenenSatir, satirlar, satirlar[i], ay);
                                if (eklenenSatir < 0)
                                {
                                    s.DegerAta(string.Format("Masraf planı ayarlaması yapılırken, {0} tertibinde, düşülen tertibin değeri ile uyumlu eklenen satır bulunamadı!", satirlar[i].tertip.TertipStrOl()));
                                    return s;
                                }
                                double azalan = 0;
                                if (satirlar[eklenenSatir].odenek[ay] + tutFark > 0)
                                {
                                    azalan = 0 - tutFark;
                                    satirlar[eklenenSatir].odenek[ay] += tutFark;
                                    tutFark = 0;
                                }
                                else
                                {
                                    tutFark += satirlar[eklenenSatir].odenek[ay];
                                    azalan = satirlar[eklenenSatir].odenek[ay];
                                    satirlar[eklenenSatir].odenek[ay] = 0;
                                }
                                for (int icAy = 0; icAy < ay && azalan > 0; icAy++)
                                {
                                    int azalanAy = icAy;
                                    if (azalanAy > 11) azalanAy -= 12;
                                    int azalanMpAy = azalanAy + 6;

                                    double dusulebilir = mpBilgi[azalanMpAy] + satirlar[i].odenek[azalanAy];
                                    if (dusulebilir > 0)
                                    {
                                        double dus = 0;
                                        if (dusulebilir > azalan)
                                        {
                                            dus = azalan;
                                            azalan = 0;
                                        }
                                        else
                                        {
                                            dus = dusulebilir;
                                            azalan -= dusulebilir;
                                        }

                                        satirlar[i].odenek[azalanAy] -= dus;
                                        satirlar[eklenenSatir].odenek[azalanAy] += dus;
                                    }

                                }
                                if (azalan > 0)
                                {
                                    s.DegerAta(string.Format("Masraf planı ayarlaması yapılırken, {0} tertibinde, düşülen tertibin değeri ({1}) diğer aylara aktarılamadı!", satirlar[i].tertip.TertipStrOl(), satirlar[eklenenSatir].odenek[ay].ToString("#,##0")));
                                    return s;
                                }
                            }
                        }
                    }
                }
            }
            return s;
        }

        private static bool AylardanHarcananlariDus(double[] mpBilgi)
        {
            double dusulecek = mpBilgi[2] + mpBilgi[3];
            for (int i = 6; i < mpBilgi.Length && dusulecek > 0; i++)
            {
                if (dusulecek > mpBilgi[i])
                {
                    dusulecek = dusulecek - mpBilgi[i];
                    mpBilgi[i] = 0;
                }
                else
                {
                    mpBilgi[i] = mpBilgi[i] - dusulecek;
                    dusulecek = 0;
                }
            }
            if (dusulecek != 0)
                return false;
            else
                return true;
        }

        private static void DetayaEkle(ArrayList hedefSatirlar, MPRevizeSatir eklenenSatir, MPRevizeSatir dusulenSatir)
        {
            double[] eklenenAylar = new double[12];
            if (dusulenSatir == null)
            {
                DetayaEkle(hedefSatirlar, eklenenSatir, 0, eklenenAylar);
            }
            else
            {
                double eklenen = eklenenSatir.NetToplam();
                double dusulen = 0 - dusulenSatir.NetToplam();
                double ekleDusMiktar = Math.Min(eklenen, dusulen);
                if (ekleDusMiktar != 0)
                {
                    DetayaEkle(hedefSatirlar, eklenenSatir, ekleDusMiktar, eklenenAylar);
                    MPRevizeSatir yeniDusulen = DetayDusmeAyarla(dusulenSatir, ekleDusMiktar, eklenenAylar);
                    DetayaEkle(hedefSatirlar, yeniDusulen, 0 - ekleDusMiktar, eklenenAylar);
                }
            }
        }

        private static MPRevizeSatir DetayDusmeAyarla(MPRevizeSatir dusulenSatir, double dusMiktar, double[] dusulecekAylar)
        {
            MPRevizeSatir yeniSatir = new MPRevizeSatir();
            yeniSatir.tertip = dusulenSatir.tertip;
            while (true)
            {
                int ilkDusulenAy = -1;
                double dusulenMiktar = 0;
                for (int ay = 0; ay < 12; ay++)
                {
                    dusulenMiktar = dusulenSatir.odenek[ay];
                    if (dusulenMiktar < 0)
                    {
                        ilkDusulenAy = ay;
                        break;
                    }
                }
                if (ilkDusulenAy == -1) return yeniSatir;
                int ilkEklenenAy = -1;
                double eklenenMiktar = 0;
                for (int ay = 0; ay < 12; ay++)
                {
                    eklenenMiktar = dusulecekAylar[ay];
                    if (eklenenMiktar > 0)
                    {
                        ilkEklenenAy = ay;
                        break;
                    }
                }
                if (ilkEklenenAy == -1) return yeniSatir;

                double islemMiktar = Math.Min(eklenenMiktar, 0 - dusulenMiktar);
                islemMiktar = Math.Min(islemMiktar, dusMiktar);

                yeniSatir.odenek[ilkEklenenAy] -= islemMiktar;
                dusulenSatir.odenek[ilkDusulenAy] += islemMiktar;
                dusulecekAylar[ilkEklenenAy] -= islemMiktar;
                dusMiktar -= islemMiktar;
                if (dusMiktar < 0) return yeniSatir;
            }
        }

        private static void DetayaEkle(ArrayList hedefSatirlar, MPRevizeSatir satir, double ekleDusMiktar, double[] eklenenAylar)
        {
            MPRevizeSatir yeniSatir = SatirBul(hedefSatirlar, satir);
            if (yeniSatir == null)
            {
                yeniSatir = new MPRevizeSatir();
                yeniSatir.tertip = new Tertip(satir.tertip.yil, ENUMTabloKod.TABMasrafPlani);
                yeniSatir.tertip.TertipAta(satir.tertip);
                hedefSatirlar.Add(yeniSatir);
            }
            for (int i = 0; i < 12; i++)
            {
                if (ekleDusMiktar == 0)
                {
                    yeniSatir.odenek[i] += satir.odenek[i];
                    eklenenAylar[i] = satir.odenek[i];
                    satir.odenek[i] = 0;
                }
                else
                {
                    if (ekleDusMiktar > 0 && satir.odenek[i] <= 0) continue;
                    if (ekleDusMiktar < 0 && satir.odenek[i] >= 0) continue;
                    if (satir.odenek[i] >= ekleDusMiktar)
                    {
                        satir.odenek[i] -= ekleDusMiktar;
                        yeniSatir.odenek[i] += ekleDusMiktar;
                        eklenenAylar[i] = ekleDusMiktar;
                        ekleDusMiktar = 0;
                        return;
                    }
                    else
                    {
                        yeniSatir.odenek[i] += satir.odenek[i];
                        ekleDusMiktar -= satir.odenek[i];
                        eklenenAylar[i] = satir.odenek[i];
                        satir.odenek[i] = 0;
                    }
                }
            }
        }

        private static MPRevizeSatir SatirBul(ArrayList hedefSatirlar, MPRevizeSatir arananSatir)
        {
            foreach (MPRevizeSatir satir in hedefSatirlar)
            {
                if (satir.tertip.TertipStrOl().Equals(arananSatir.tertip.TertipStrOl()))
                    return satir;
            }
            return null;
        }

        internal static Sonuc MasrafPlaniKarsilananlariYaz(VeriErisim veriErisim, Kullanici kullanan, MPRevizeTalep[] revizeTalepler, int revizeNo, bool bosSatirKontroluYap)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (revizeTalepler == null || revizeTalepler.Length == 0)
            {
                s.DegerAta("İşlem için seçilmiş revize talep bilgisi gönderilmemiş!");
                return s;
            }
            int yil = revizeTalepler[0].yil;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            ArrayList tertipler = new ArrayList();
            MPRevizeForm revizeForm = null;
            if (revizeNo > 0)
            {
                try
                {
                    revizeForm = IsKurallari.MasrafPlaniRevize.MPRevizeFormOku(ve, kullanan, yil, revizeNo);
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
                if (s.islemSonuc)
                {
                    if (!revizeForm.DegistirilebilirMi())
                    {
                        s.DegerAta("Verilen revize numarası onaylı olduğu için değiştirilemez!");
                    }
                    else if (revizeForm.kaydedenKullaniciKodu != kullanan.kullaniciKodu)
                    {
                        s.DegerAta("Başka bir kullanıcının revize işlemini değiştiremezsiniz!");
                    }
                }
                if (!s.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return s;
                }
                for (int i = 0; i < revizeForm.satirlar.Length; i++)
                {
                    tertipler.Add(revizeForm.satirlar[i]);
                }
            }
            else
            {
                revizeForm = new MPRevizeForm();
                revizeForm.yil = yil;
                revizeForm.revizeNo = 0;
                revizeForm.revizeFormTip = ENUMRevizeFormTip.MasrafPlaniRevize;
            }

            TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);
            foreach (MPRevizeTalep talep in revizeTalepler)
            {
                ENUMRevizeFormDurum durum = tab.RevizeTalepDurumBul(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo);
                if (durum != ENUMRevizeFormDurum.Iletildi)
                    s.DegerAta(string.Format("{0} kodlu birim için yapılan işlemde, talebin durumu iletildi olmadığından işleme devam edilmedi!", talep.kurumsalYapi.kod));
                if ((talep.satirlar == null || talep.satirlar.Length == 0) && bosSatirKontroluYap)
                    s.DegerAta("0} kodlu birim için yapılan işlemde, birim seçilmiş fakat satırlar girilmemiş!");
                for (int i = 0; i < talep.satirlar.Length; i++)
                {
                    TalepSatiriEkle(tertipler, talep.satirlar[i]);
                }
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            ve.BeginTransaction();
            revizeForm.satirlar = (MPRevizeSatir[])tertipler.ToArray(typeof(MPRevizeSatir));


            s = IsKurallari.MasrafPlaniRevize.MPRevizeFormKaydet(ve, kullanan, revizeForm, true, false);
            string kayitRevizeNo = s.anahtar;
            foreach (MPRevizeTalep talep in revizeTalepler)
            {
                if (!s.islemSonuc)
                    break;
                if (string.IsNullOrEmpty(talep.cevapAciklama))
                    talep.cevapAciklama = "Talebiniz karşılanmıştır.";
                s = tab.RevizeTalepDurumDegistir(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo, ENUMRevizeFormDurum.Onayli);
                if (!s.islemSonuc)
                    break;
                s = tab.RevizeTalepKarsilananSil(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo);
                if (!s.islemSonuc)
                    break;

                for (int i = 0; i < talep.satirlar.Length; i++)
                {
                    for (int ay = 0; ay < 12; ay++)
                    {
                        s = tab.RevizeTalepDetayEkle(talep, talep.satirlar[i], ay, false);
                        if (!s.islemSonuc)
                            break;
                    }
                    if (!s.islemSonuc)
                        break;
                }
            }
            if (!s.islemSonuc)
            {
                ve.Rollback();
            }
            else
            {
                s.anahtar = kayitRevizeNo;
                ve.Commit();
            }
            ve.VeriErisimKapat();
            return s;
        }

        private static void TalepSatiriEkle(ArrayList tertipler, MPRevizeSatir gelenRS)
        {
            foreach (MPRevizeSatir rs in tertipler)
            {
                if (rs.tertip.Equals(gelenRS.tertip))
                {
                    for (int ay = 0; ay < 12; ay++)
                    {
                        rs.odenek[ay] += gelenRS.karsilananOdenek[ay];
                    }
                    return;
                }
            }
            for (int ay = 0; ay < 12; ay++)
            {
                gelenRS.odenek[ay] = gelenRS.karsilananOdenek[ay];
            }
            tertipler.Add(gelenRS);
            return;
        }

        internal static Sonuc TalepleriIptalEt(VeriErisim veriErisim, Kullanici kullanan, MPRevizeTalep[] revizeTalepler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (revizeTalepler == null || revizeTalepler.Length == 0)
            {
                s.DegerAta("İşlem için seçilmiş revize talep bilgisi gönderilmemiş!");
                return s;
            }
            int yil = revizeTalepler[0].yil;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            ArrayList tertipler = new ArrayList();
            TABMPGREVIZETALEP tab = new TABMPGREVIZETALEP(ve);
            foreach (MPRevizeTalep talep in revizeTalepler)
            {
                ENUMRevizeFormDurum durum = tab.RevizeTalepDurumBul(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo);
                if (durum != ENUMRevizeFormDurum.Iletildi)
                    s.DegerAta(string.Format("{0} kodlu birim için yapılan işlemde, talebin durumu iletildi olmadığından işleme devam edilmedi!", talep.kurumsalYapi.kod));
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            ve.BeginTransaction();
            foreach (MPRevizeTalep talep in revizeTalepler)
            {
                if (!s.islemSonuc)
                    break;
                if (string.IsNullOrEmpty(talep.cevapAciklama))
                    talep.cevapAciklama = "Talebiniz iptal edilmiştir!";
                s = tab.RevizeTalepDurumDegistir(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo, ENUMRevizeFormDurum.Iptal);
                if (!s.islemSonuc)
                    break;
                s = tab.RevizeTalepKarsilananSil(talep.yil, talep.kurumsalYapi.kod, talep.revizeTalepNo);
                if (!s.islemSonuc)
                    break;
            }
            if (!s.islemSonuc)
            {
                ve.Rollback();
            }
            else
            {
                ve.Commit();
            }
            ve.VeriErisimKapat();
            return s;
        }
    }
}
