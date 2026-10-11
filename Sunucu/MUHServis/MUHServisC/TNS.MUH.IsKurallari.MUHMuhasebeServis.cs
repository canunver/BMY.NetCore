using System;
using System.Collections.Generic;
using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Muhasebe işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMuhasebeServis
    {
        /// <summary>
        /// Muhasebe bilgilerini kaydet
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static Sonuc Kaydet(ObjectArray muhasebeBilgileri)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABMuhasebe to = new TABMuhasebe(ve);
            TABGenel tg = new TABGenel(ve);

            foreach (MuhasebeBilgisi muhasebe in muhasebeBilgileri.objeler)
            {
                if (muhasebe.kod > 0)
                {
                    MuhasebeBilgisi okunan = Oku(ve, muhasebe.kod);
                    if (okunan == null)
                        sonuc = to.Ekle(muhasebe);
                    else
                        sonuc = to.Guncelle(muhasebe);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH128;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe bilgisini sil
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static Sonuc Sil(ObjectArray muhasebeBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABMuhasebe to = new TABMuhasebe(ve);

            foreach (MuhasebeBilgisi muhasebe in muhasebeBilgileri.objeler)
            {
                if (muhasebe.kod > 0)
                    sonuc = to.Sil(muhasebe);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH129;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe bilgisi listele
        /// </summary>
        /// <param name="pMuhasebe">Muhasebe kodu</param>
        /// <returns></returns>
        public static MuhasebeBilgisi Oku(VeriErisim veriErisim, int muhKod)
        {
            if (muhKod <= 0) return null;
            MuhasebeBilgisi muhBil = new MuhasebeBilgisi();
            muhBil.kod = muhKod;
            ObjectArray iVAN = Listele(veriErisim, muhBil);

            if (iVAN.ObjeSayisi == 1)
                return (MuhasebeBilgisi)iVAN[0];
            return null;
        }

        /// <summary>
        /// Muhasebe bilgisi listele
        /// </summary>
        /// <param name="pMuhasebe">Muhasebe kodu</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, MuhasebeBilgisi pMuhasebe)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebe to = new TABMuhasebe(ve);
            IDataReader r = to.Listele(pMuhasebe);

            if (r != null)
            {
                while (r.Read())
                {
                    MuhasebeBilgisi muhasebe = new MuhasebeBilgisi();

                    muhasebe.kod = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.MUHASEBEKOD, 0);
                    muhasebe.ad = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.MUHASEBEAD, "");
                    muhasebe.ilgili1 = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.ILGILI1, "");
                    muhasebe.ilgili2 = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.ILGILI2, "");
                    muhasebe.adres = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.ADRES, "");
                    muhasebe.telefon = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.TELEFON, "");
                    muhasebe.vergiNo = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.VERGINO, "");
                    muhasebe.faks = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.FAKS, "");
                    muhasebe.ePosta = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.EPOSTA, "");
                    muhasebe.ilgili1Unvan = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.ILGILI1UNVAN, "");
                    muhasebe.ilgili2Unvan = VeriErisim.DegerAl(r, (int)TABMuhasebe.ENUMAlanlar.ILGILI2UNVAN, "");

                    iVAN.objeler.Add(muhasebe);
                }
                r.Close();
            }
            else
                iVAN.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();

            return iVAN;
        }

        ///// <summary>
        ///// Muhasebe listesini ebutceden aktar.
        ///// </summary>
        ///// <param name="kod">Muhasebe Kodu</param>
        ///// <param name="ad">Muhasebe Adı</param>
        ///// <returns></returns>
        //public static Sonuc EbutcedenAktar(string kod, string ad)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    //System.Net.ServicePointManager.CertificatePolicy = new CertificatePolicy();
        //    MUHServisC.ebutce.Muhasebe.Muhasebe mEbutce = new MUHServisC.ebutce.Muhasebe.Muhasebe();
        //    mEbutce.Proxy = OrtakFonksiyonlar.ProxyAyarla();

        //    ObjectArray muhlar = new ObjectArray();
        //    MuhasebeBilgileriW mW = mEbutce.MuhasebeListele(kod, ad);

        //    for (int i = 0; i < mW.muhsebeBilgisi.Length; i++)
        //    {
        //        MuhasebeBilgisi muh = new MuhasebeBilgisi();
        //        muh.ad = mW.muhsebeBilgisi[i].ad;
        //        muh.kod = mW.muhsebeBilgisi[i].kod;

        //        if (muh.kod < 87000)//Deneme amaçlı tanımlanmış olan bilgiler gelmesin
        //        {
        //            bool bulundu = false;
        //            foreach (MuhasebeBilgisi mb in muhlar.objeler)
        //            {
        //                if (mb.kod == muh.kod)
        //                {
        //                    bulundu = true;
        //                    break;
        //                }
        //            }
        //            if (!bulundu)
        //                muhlar.objeler.Add(muh);
        //        }
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

        //    TABMuhasebe to = new TABMuhasebe(ve);
        //    if (kod == "" && ad == "" && muhlar.objeler.Count > 0)
        //        sonuc = to.SilHepsini();
        //    else
        //        sonuc.islemSonuc = true;

        //    if (sonuc.islemSonuc && muhlar.objeler.Count > 0)
        //        sonuc = Kaydet(muhlar);
        //    else
        //        sonuc.islemSonuc = false;

        //    if (sonuc.islemSonuc)
        //        sonuc.bilgiStr = ButceMuhasebeResI.MUH126;
        //    else
        //    {
        //        sonuc.islemSonuc = false;

        //        sonuc.hataStr = ButceMuhasebeResI.MUH127;

        //        if (muhlar.objeler.Count == 0)
        //            sonuc.hataStr += "Muhasebe listesi boş geldi.";
        //    }
        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        internal static Sonuc MuhasebeBirimiTasi(VeriErisim veriErisim, Kullanici kullanan, int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod, TNSCollection islemListesi)
        {
            Sonuc sonuc = new Sonuc();

            TNS.HRC.IHRCServis srv = TNS.HRC.Arac.Tanimla();

            HRC.IdareBilgi kriter = new HRC.IdareBilgi();
            kriter.muhasebe = eskiSaymanlikKod;
            kriter.birim = eskiBirimKod;
            HRC.IdareBilgi eskiBirim = srv.IdareBilgiListele(kullanan, kriter);


            if (eskiBirim == null)
                sonuc.DegerAta($"{eskiSaymanlikKod}-{eskiBirimKod} kodlu birim, idare tanımında bulunamadı!<br />");
            else if (!string.IsNullOrEmpty(eskiBirim.vNo))
                sonuc.DegerAta($"{eskiSaymanlikKod}-{eskiBirimKod} kodlu birimin idare tanımında vergi numarası olmamalı!<br />");
            if (sonuc.islemSonuc)
            {
                kriter.muhasebe = yeniSaymanlikKod;
                kriter.birim = yeniBirimKod;
                HRC.IdareBilgi yeniBirim = srv.IdareBilgiListele(kullanan, kriter);
                if (yeniBirim == null)
                    sonuc.DegerAta($"{yeniSaymanlikKod}-{yeniBirimKod} kodlu birim, idare tanımında bulunamadı!<br />");
                else if (string.IsNullOrEmpty(yeniBirim.vNo))
                    sonuc.DegerAta($"{yeniSaymanlikKod}-{yeniBirimKod} kodlu birimin idare tanımında vergi numarası olmalı!<br />");
            }
            if (!sonuc.islemSonuc)
                return sonuc;


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABMuhasebe to = new TABMuhasebe(ve);
            string yeniHarcamaBirimi = null;
            string eskiHarcamaBirimi = null;
            foreach (int item in islemListesi)
            {
                if (yeniHarcamaBirimi == null && (
                         (ENUMSaymanlikTasiIslem)item == ENUMSaymanlikTasiIslem.SEYYARGOREVPLANLAMA ||
                         (ENUMSaymanlikTasiIslem)item == ENUMSaymanlikTasiIslem.MASRAFPLANI) ||
                         (ENUMSaymanlikTasiIslem)item == ENUMSaymanlikTasiIslem.MASRAFPLANITAVAN
                   )
                {
                    TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                    yeniHarcamaBirimi = HarcamaBirimiBul(servis, ve, kullanan, yeniSaymanlikKod, yeniBirimKod);
                    if (!string.IsNullOrWhiteSpace(yeniHarcamaBirimi))
                    {
                        eskiHarcamaBirimi = HarcamaBirimiBul(servis, ve, kullanan, eskiSaymanlikKod, eskiBirimKod);
                        if (string.IsNullOrWhiteSpace(yeniHarcamaBirimi))
                            sonuc.DegerAta($"{eskiSaymanlikKod} - {eskiBirimKod} kodunun kurumsal yapı karşılığı bulunamadı!");
                    }
                    else
                        sonuc.DegerAta($"{yeniSaymanlikKod} - {yeniBirimKod} kodunun kurumsal yapı karşılığı bulunamadı!");
                    if (!sonuc.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }

                string itemAd = TNS.MUH.Arac.SaymanlikTasiIslemAdBul((ENUMSaymanlikTasiIslem)item);
                Sonuc s = null;
                ve.BeginTransaction();
                switch ((ENUMSaymanlikTasiIslem)item)
                {
                    case ENUMSaymanlikTasiIslem.ODEMEEMRI:
                        s = to.OdemeEmriTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.ABONELIKLER:
                        s = to.AbonelikTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.PERSONEL:
                        s = to.PersonelTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;

                    case ENUMSaymanlikTasiIslem.BIRIMBUTCETEKLIFI:
                        s = to.BirimButceTeklifiTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.BIRIMHIZMETMALIYETI:
                        s = to.BirimHizmetMaliyetiTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.DEPREMHARCAMALARI:
                        s = to.DepremHarcamalariTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.ARACKIRALAMAPLANI:
                        s = to.AracKiralamaPlaniTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.SURECTANIMLARI:
                        s = to.SurecTanimlariTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.DOGRUDANTEMIN:
                        s = to.DogrudanTeminTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.SEYYARGOREVPLANLAMA:
                        if (!string.IsNullOrWhiteSpace(eskiHarcamaBirimi) && !string.IsNullOrWhiteSpace(yeniHarcamaBirimi))
                            s = to.SeyyarGorevPlanlamaTasi(eskiHarcamaBirimi, yeniHarcamaBirimi);
                        else
                            s = new Sonuc("Saymanlık - birim kodu, harcama birimine çevrilmemiş!");
                        break;
                    case ENUMSaymanlikTasiIslem.MEVCUTTASITLAR:
                        s = to.MevcutTasitVeKiralikSozlesmeTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.CESITLIODEMELER:
                        s = to.CesitliOdemelerTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.ODENEKGONDERME:
                        s = to.OdenekGondermeleriTasi(eskiSaymanlikKod, eskiBirimKod, yeniSaymanlikKod, yeniBirimKod);
                        break;
                    case ENUMSaymanlikTasiIslem.MASRAFPLANI:
                        if (!string.IsNullOrWhiteSpace(eskiHarcamaBirimi) && !string.IsNullOrWhiteSpace(yeniHarcamaBirimi))
                            s = to.MasrafPlaniTasi(eskiHarcamaBirimi, yeniHarcamaBirimi);
                        else
                            s = new Sonuc("Saymanlık - birim kodu, harcama birimine çevrilmemiş!");
                        break;
                    case ENUMSaymanlikTasiIslem.MASRAFPLANITAVAN:
                        if (!string.IsNullOrWhiteSpace(eskiHarcamaBirimi) && !string.IsNullOrWhiteSpace(yeniHarcamaBirimi))
                            s = to.MasrafPlaniTavanTasi(eskiHarcamaBirimi, yeniHarcamaBirimi);
                        else
                            s = new Sonuc("Saymanlık - birim kodu, harcama birimine çevrilmemiş!");
                        break;

                    default:
                        break;
                }
                if (s != null && s.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += $"{itemAd} için {s.kayitSay} adet kayıt aktarıldı!<br />";
                }
                else
                {
                    ve.Rollback();
                    if (s == null)
                        sonuc.bilgiStr += $"{itemAd} için aktarım henüz yazılmamış!<br />";
                    else
                        sonuc.bilgiStr += $"{itemAd} aktarılamadı: {s.hataStr}!<br />";
                }
            }
            return sonuc;
        }

        private static string HarcamaBirimiBul(TNS.BHMv2.IBHMv2Servis servis, VeriErisim ve, Kullanici kullanan, int saymanlikKod, string birimKod)
        {
            BHMv2.KurumsalYapi ky = new BHMv2.KurumsalYapi();
            ky.saymanlikKod = saymanlikKod;
            ky.birimKod = birimKod;
            BHMv2.KurumsalYapi[] kyler = servis.KurumsalYapiListele(kullanan, ky, false, false);
            if (kyler.Length == 1)
                return kyler[0].kod.Replace(".", "");
            return null;
        }
    }
}
