using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Geçici görev yolluk belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class PSLGeciciGorevYolluk
    {
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(VeriErisim ve2, Kullanici kullanan, GeciciGorevYollukBelge sb, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            //sonuc.islemSonuc = OMKYetkiKontrol.YazmaYetkisiVarMi(kullanan, sb.muhasebeKod, sb.harcamaKod, sb.ambarKod);
            //if (!sonuc.islemSonuc)
            //{
            //    sonuc.hataStr = "Belirtilen muhasebe, harcama, ambar kodları için yazma yetkiniz yoktur.";
            //    return sonuc;
            //}

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = sb.yil;
            string surecNo = sb.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            sonuc = Kontrol(sb);

            if (sonuc.hataStr != String.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(ve2, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk toGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();

            sonuc = toGeciciGorevYolluk.Kaydet(sb);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Bu belgeye ait satırları sil

            GeciciGorevYollukDetay gdSil = new GeciciGorevYollukDetay();

            gdSil.yil = sb.yil;
            gdSil.muhasebeKod = sb.muhasebeKod;
            gdSil.birimKod = sb.birimKod;
            gdSil.surecNo = sb.surecNo;
            gdSil.belgeTur = sb.belgeTur;
            gdSil.tcKimlikNoBelge = sb.tcKimlikNo;
            gdSil.kayitSiraNo = sb.kayitSiraNo;

            sonuc = SilDetay(ve, kullanan, gdSil);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            //// Hesaplama Kısmı
            ////1. 

            //decimal gundelikDeger = 0;
            //decimal gundelikDegerOdemeOrani = 0;
            //decimal gundelikDegerArttirimli = 0;
            //decimal yurtDisiGundelikDeger = 0;
            //decimal yurtDisiGundelikDegerOdemeOrani = 0;
            //decimal pay = 0;
            //decimal payda = 0;
            //if (sb.belgeTur ==  ENUMYollukGundelikTuru.SUREKLIGECICI || sb.belgeTur == ENUMYollukGundelikTuru.DENETIMELEMANI
            //    || sb.belgeTur == ENUMYollukGundelikTuru.TEDAVIYOLLUGU || sb.belgeTur == ENUMYollukGundelikTuru.KOVUSTURMA
            //    || sb.belgeTur == ENUMYollukGundelikTuru.ANKETOR)
            //{
            //    YollukGundelikTanim gundelikTanimKriter = new YollukGundelikTanim();
            //    gundelikTanimKriter.yil = sb.gundelikYil;
            //    gundelikTanimList = (new PSLSurekliGorevGundelikTanim()).Listele(kullanan, gundelikTanimKriter);
            //    gundelikDeger = GundelikTanimYevmiyeDegeri(sb.gundelikKod);

            //    //Ödeme Oranı varsa
            //    if (!string.IsNullOrEmpty(sb.odemeOrani))
            //    {
            //        int koordinat = sb.odemeOrani.IndexOf("/");
            //        if (koordinat != -1)
            //        {
            //            string[] kesir = sb.odemeOrani.Split('/');
            //            int odemeOraniPay = OrtakFonksiyonlar.ConvertToInt(kesir[0], 0);
            //            int odemeOraniPayda = OrtakFonksiyonlar.ConvertToInt(kesir[1], 0);

            //            if (odemeOraniPay != 0 && odemeOraniPayda != 0)
            //            {
            //                gundelikDeger = Math.Round((gundelikDeger * odemeOraniPay) / odemeOraniPayda, 2);
            //            }
            //        }
            //    }
            //    gundelikDegerArttirimli = gundelikDeger * (decimal)1.5;
            //}

            //pay = 1;
            //payda = 1;
            //int kullanilanArttirimliGunSayisi = 0;
            //int kalanArttirimliGunSayisi = arttirimligunSiniri;
            //int yevmiyeKurDovizKod = 0;
            //decimal toplamTutar = decimal.Zero;

            //bool arttirimliHesapla = sb.arttirimliHesapla;

            //foreach (GeciciGorevYollukDetay yolculuk in sb.detay.objeler)
            //{
            //    //Konaklama ile ilgili hesapların yapıldığı yer
            //    yolculuk.yolculukGunSayi = OrtakServis.KesirDeger(yolculuk.yolculukGunSayiStr, out pay, out payda);

            //    decimal icGundelikDeger = gundelikDeger;

            //    //switch (sb.belgeTur)
            //    //{
            //    //    case ENUMYollukGundelikTuru.SUREKLIGECICI:
            //    //    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
            //    //    case ENUMYollukGundelikTuru.KOVUSTURMA:
            //    //    case ENUMYollukGundelikTuru.ANKETOR:
            //    //        yolculuk.toplamTutar += yolculuk.ikamettenTerminaleHarcanan;
            //    //        if (yolculuk.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GOREV)
            //    //            yolculuk.toplamTutar += (pay * (gundelikDeger / payda));
            //    //        else
            //    //            icGundelikDeger = (pay * (gundelikDeger / payda));
            //    //        yolculuk.toplamTutar += yolculuk.yolculuktaHarcanan;
            //    //        yolculuk.toplamTutar += yolculuk.terminaldenIkameteHarcanan;

            //    //        decimal gorevYevmiyeToplami = 0;
            //    //        if (yolculuk.gorevGunSayisi > gunSiniri)
            //    //        {
            //    //            int kalanGun = yolculuk.gorevGunSayisi - gunSiniri;
            //    //            decimal yeniGundelikDeger = Math.Round((icGundelikDeger / 3) * 2, 2);
            //    //            gorevYevmiyeToplami += gunSiniri * icGundelikDeger;
            //    //            gorevYevmiyeToplami += kalanGun * yeniGundelikDeger;
            //    //        }
            //    //        else
            //    //            gorevYevmiyeToplami += yolculuk.gorevGunSayisi * icGundelikDeger;

            //    //        yolculuk.gorevYevmiyeToplami = Math.Round(gorevYevmiyeToplami, 2);

            //    //        yolculuk.toplamTutar += yolculuk.gorevYevmiyeToplami;
            //    //        yolculuk.toplamTutar += yolculuk.hamalBagaj;

            //    //        decimal konaklananGunSayisi = yolculuk.konaklananGunSayi;
            //    //        if (yolculuk.konaklananGunSayi > arttirimligunSiniri)
            //    //            konaklananGunSayisi = arttirimligunSiniri;

            //    //        decimal toplamKonaklamaOdenen = 0;
            //    //        if (yolculuk.konaklamaGunlukFatura > gundelikDegerArttirimli)
            //    //            toplamKonaklamaOdenen = konaklananGunSayisi * gundelikDegerArttirimli;
            //    //        else
            //    //            toplamKonaklamaOdenen = konaklananGunSayisi * yolculuk.konaklamaGunlukFatura;
            //    //        yolculuk.toplamKonaklamaOdenen = toplamKonaklamaOdenen;

            //    //        break;

            //    //    case ENUMYollukGundelikTuru.DENETIMELEMANI:
            //    //        yolculuk.toplamTutar += yolculuk.ikamettenTerminaleHarcanan;
            //    //        if (yolculuk.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GOREV)
            //    //            yolculuk.toplamTutar += (pay * (gundelikDeger / payda));
            //    //        else
            //    //            icGundelikDeger = (pay * (gundelikDeger / payda));

            //    //        yolculuk.toplamTutar += yolculuk.yolculuktaHarcanan;
            //    //        yolculuk.toplamTutar += yolculuk.terminaldenIkameteHarcanan;
            //    //        yolculuk.gorevYevmiyeToplami = yolculuk.gorevGunSayisi * icGundelikDeger;
            //    //        yolculuk.gorevYevmiyeToplami = Math.Round(yolculuk.gorevYevmiyeToplami, 2);
            //    //        yolculuk.toplamTutar += yolculuk.gorevYevmiyeToplami;
            //    //        yolculuk.toplamTutar += yolculuk.hamalBagaj;

            //    //        if (yolculuk.konaklamaGunlukFatura > gundelikDegerArttirimli)
            //    //            yolculuk.toplamKonaklamaOdenen = yolculuk.konaklananGunSayi * gundelikDegerArttirimli;
            //    //        else
            //    //            yolculuk.toplamKonaklamaOdenen = yolculuk.konaklananGunSayi * yolculuk.konaklamaGunlukFatura;

            //    //        yolculuk.toplamKonaklamaOdenen = Math.Round(yolculuk.toplamKonaklamaOdenen, 2);

            //    //        break;
            //    //    case ENUMYollukGundelikTuru.GECICIYURTDISI:
            //    //        gunSiniri = 180;

            //    //        KurBilgisi kur = new KurBilgisi();
            //    //        kur.yil = sb.yil;
            //    //        kur.muhasebe = sb.muhasebeKod;
            //    //        kur.birim = sb.birimKod;
            //    //        kur.surecNo = sb.surecNo;
            //    //        kur.tckimlikno = sb.tcKimlikNo;

            //    //        gundelikDeger = yolculuk.yevmiyeTutar;
            //    //        gundelikDegerOdemeOrani = yolculuk.yevmiyeTutar;
            //    //        yevmiyeKurDovizKod = yolculuk.yevmiyeParaBirimi.kod;
            //    //        if (arttirimliHesapla)
            //    //        {
            //    //            yurtDisiGundelikDeger = Math.Round(yolculuk.yevmiyeTutar * (decimal)1.5, 2);// KurBilgisiAl(kullanan, kur, yolculuk.yevmiyeParaBirimi.kod);
            //    //        }
            //    //        else
            //    //        {
            //    //            yurtDisiGundelikDeger = yolculuk.yevmiyeTutar;
            //    //        }
            //    //        yurtDisiGundelikDegerOdemeOrani = yurtDisiGundelikDeger;

            //    //        if (sb.odemeOrani != "")
            //    //        {
            //    //            if (sb.odemeOrani.IndexOf("/") != -1)
            //    //            {
            //    //                decimal ppay = OrtakFonksiyonlar.ConvertToDecimal(sb.odemeOrani.Split('/')[0], 1);
            //    //                decimal ppayda = OrtakFonksiyonlar.ConvertToDecimal(sb.odemeOrani.Split('/')[1], 1);
            //    //                gundelikDegerOdemeOrani = Math.Round((ppay * yolculuk.yevmiyeTutar) / ppayda, 2);
            //    //                yurtDisiGundelikDegerOdemeOrani = Math.Round((ppay * yurtDisiGundelikDeger) / ppayda, 2);
            //    //            }
            //    //        }

            //    //        yolculuk.toplamTutar += Math.Round(yolculuk.ikamettenTerminaleHarcanan * KurBilgisiAl(ve, kullanan, kur, yolculuk.ikamettenTerminaleParaBirimi.kod), 2);
            //    //        //yolculuk gün sayısını kontrol et

            //    //        decimal icGorevGunSayisi = yolculuk.gorevGunSayisi;
            //    //        if (yolculuk.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GOREV)
            //    //        {
            //    //            if (yolculuk.yolculukGunSayi > 0)
            //    //            {
            //    //                kalanArttirimliGunSayisi--;
            //    //                //if (arttirimligunSiniri - 1 - kullanilanArttirimliGunSayisi >= 0)
            //    //                if (kalanArttirimliGunSayisi >= 0)
            //    //                {
            //    //                    yolculuk.toplamTutar += Math.Round(yurtDisiGundelikDeger * yolculuk.yolculukGunSayi * KurBilgisiAl(ve, kullanan, kur, yolculuk.yevmiyeParaBirimi.kod), 2);
            //    //                    kullanilanArttirimliGunSayisi++;
            //    //                }
            //    //                else
            //    //                    yolculuk.toplamTutar += Math.Round(gundelikDegerOdemeOrani * yolculuk.yolculukGunSayi * KurBilgisiAl(ve, kullanan, kur, yolculuk.yevmiyeParaBirimi.kod), 2);
            //    //            }

            //    //        }
            //    //        else
            //    //            icGorevGunSayisi = icGorevGunSayisi * yolculuk.yolculukGunSayi;




            //    //        yolculuk.toplamTutar += Math.Round(yolculuk.yolculuktaHarcanan * KurBilgisiAl(ve, kullanan, kur, yolculuk.yolculuktaParaBirimi.kod), 2);
            //    //        yolculuk.toplamTutar += Math.Round(yolculuk.terminaldenIkameteHarcanan * KurBilgisiAl(ve, kullanan, kur, yolculuk.terminaldenIkameteParaBirimi.kod), 2);
            //    //        if (arttirimliHesapla)
            //    //        {
            //    //            if (kalanArttirimliGunSayisi < 0)
            //    //                kalanArttirimliGunSayisi = 0;

            //    //            if (yolculuk.gorevGunSayisi > kalanArttirimliGunSayisi) // (arttirimligunSiniri - 1 - kullanilanArttirimliGunSayisi))
            //    //            {
            //    //                //1 gununu yolda kullandı
            //    //                yolculuk.gorevYevmiyeToplami += kalanArttirimliGunSayisi * yurtDisiGundelikDegerOdemeOrani;

            //    //                if (yolculuk.gorevGunSayisi > gunSiniri)
            //    //                {
            //    //                    decimal yurtDisiGundelikDegerOdemeOraniKismi = Math.Round((yurtDisiGundelikDegerOdemeOrani / 3), 2);
            //    //                    yolculuk.gorevYevmiyeToplami += (gunSiniri - arttirimligunSiniri) * gundelikDegerOdemeOrani;
            //    //                    yolculuk.gorevYevmiyeToplami += (yolculuk.gorevGunSayisi - gunSiniri) * yurtDisiGundelikDegerOdemeOraniKismi;
            //    //                }
            //    //                else
            //    //                    yolculuk.gorevYevmiyeToplami += (yolculuk.gorevGunSayisi - kalanArttirimliGunSayisi) * gundelikDegerOdemeOrani;//arttirimli gun sınırı ustu normal tarife çalışır

            //    //                arttirimliHesapla = false;
            //    //            }
            //    //            else
            //    //            {
            //    //                yolculuk.gorevYevmiyeToplami = icGorevGunSayisi * yurtDisiGundelikDegerOdemeOrani;
            //    //                kalanArttirimliGunSayisi -= yolculuk.gorevGunSayisi;
            //    //            }
            //    //        }
            //    //        else if (yolculuk.gorevGunSayisi != 0)
            //    //            yolculuk.gorevYevmiyeToplami = Math.Round(icGorevGunSayisi * gundelikDegerOdemeOrani, 2);

            //    //        yolculuk.gorevYevmiyeToplami = Math.Round(yolculuk.gorevYevmiyeToplami, 2);
            //    //        yolculuk.toplamTutar += Math.Round(yolculuk.gorevYevmiyeToplami * KurBilgisiAl(ve, kullanan, kur, yolculuk.yevmiyeParaBirimi.kod), 2);
            //    //        yolculuk.toplamTutar += Math.Round(yolculuk.hamalBagaj * KurBilgisiAl(ve, kullanan, kur, yolculuk.hamalBagajParaBirimi.kod), 2);

            //    //        //konaklama ücreti hesaplanan bölüm
            //    //        decimal konaklamaGunlukFatura = yolculuk.konaklamaGunlukFatura; //*KurBilgisiAl(kullanan, kur, yolculuk.konaklamaGunlukFaturaParaBirimi.kod);
            //    //        if (!sb.konaklamaTamamiOdenecekMi)
            //    //        {
            //    //            //konaklamaGunlukFatura, %50 arttırımlı gündeliğin %40ından fazla ise; fazla olan kısmın %70i ödenir.
            //    //            decimal yuzde40i = yurtDisiGundelikDegerOdemeOrani * (decimal)(0.4);
            //    //            decimal odenecekTutar = 0;
            //    //            if (konaklamaGunlukFatura > yuzde40i)
            //    //            {
            //    //                decimal asanKisim = konaklamaGunlukFatura - yuzde40i;
            //    //                odenecekTutar = Math.Round(asanKisim * (decimal)(0.7), 2);
            //    //            }
            //    //            else
            //    //                odenecekTutar = konaklamaGunlukFatura * 0; //fazla değilse 0

            //    //            if (yolculuk.konaklananGunSayi > arttirimligunSiniri)
            //    //                yolculuk.toplamKonaklamaOdenen = Math.Round(arttirimligunSiniri * odenecekTutar * KurBilgisiAl(ve, kullanan, kur, yolculuk.konaklamaGunlukFaturaParaBirimi.kod), 2);
            //    //            else
            //    //                yolculuk.toplamKonaklamaOdenen = Math.Round(yolculuk.konaklananGunSayi * odenecekTutar * KurBilgisiAl(ve, kullanan, kur, yolculuk.konaklamaGunlukFaturaParaBirimi.kod), 2);
            //    //        }
            //    //        else
            //    //        {
            //    //            //KmL 11.04.2012 
            //    //            //25 Mart 2012 tarihli 28244 sayılı resmi gazetedeki yazı sebebiyle değiştirildi.
            //    //            //konaklamaGunlukFatura, %50 arttırımlı gündeliğin %40ından fazla ise; fazla olan kısım ödenir.
            //    //            decimal yuzde40i = yurtDisiGundelikDegerOdemeOrani * (decimal)(0.4);
            //    //            decimal odenecekTutar = 0;
            //    //            if (konaklamaGunlukFatura > yuzde40i)
            //    //            {
            //    //                decimal asanKisim = konaklamaGunlukFatura - yuzde40i;
            //    //                odenecekTutar = asanKisim;
            //    //            }
            //    //            yolculuk.toplamKonaklamaOdenen = Math.Round(yolculuk.konaklananGunSayi * odenecekTutar * KurBilgisiAl(ve, kullanan, kur, yolculuk.konaklamaGunlukFaturaParaBirimi.kod), 2);
            //    //        }
            //    //        break;
            //    //}

            //    //yolculuk.toplamKonaklamaOdenen = Math.Round(yolculuk.toplamKonaklamaOdenen, 2);
            //    //yolculuk.toplamTutar += yolculuk.toplamKonaklamaOdenen;

            //    //if (sb.dovizKuru != 0)
            //    //{
            //    //    yolculuk.toplamKonaklamaOdenen = Math.Round(yolculuk.toplamKonaklamaOdenen * sb.dovizKuru, 2);
            //    //    yolculuk.toplamTutarDoviz = yolculuk.toplamTutar;
            //    //    yolculuk.toplamTutar = Math.Round(yolculuk.toplamTutar * sb.dovizKuru, 2);
            //    //    yolculuk.toplamTutarDoviz = Math.Round(yolculuk.toplamTutarDoviz, 2);
            //    //}
            //    //yolculuk.toplamTutar = Math.Round(yolculuk.toplamTutar, 2);
            //    toplamTutar += yolculuk.toplamTutar;
            //}

            //if (sb.avansAlindiMi)
            //{
            //    OnayForm of = new OnayForm();
            //    of.yil = sb.yil;
            //    of.surecNo = sb.surecNo;
            //    of.avansVarmi = (int)ENUMEvetHayir.EVET;
            //    of.hrcTalimati = (int)ENUMEvetHayir.EVET;
            //    ObjectArray onayFormBilgi = new ObjectArray();
            //    onayFormBilgi = new HRCOnayForm().Listele(kullanan, of);
            //    if (onayFormBilgi.objeler.Count > 0)
            //    {
            //        of = (OnayForm)onayFormBilgi.objeler[0];
            //        if (of.yaklasikMaliyet > 0.0)
            //        {
            //            decimal tutarFark = toplamTutar - OrtakFonksiyonlar.ConvertToDecimal(of.yaklasikMaliyet);

            //            KurBilgisi kur = new KurBilgisi();
            //            kur.yil = sb.yil;
            //            kur.muhasebe = sb.muhasebeKod;
            //            kur.birim = sb.birimKod;
            //            kur.surecNo = sb.surecNo;
            //            kur.tckimlikno = sb.tcKimlikNo;

            //            decimal avansKur = KurBilgisiAl(ve2, kullanan, kur, yevmiyeKurDovizKod);
            //            decimal bildirimKur = MerkezBankasiDovizKurGetir(sb.bildirimTarihi, yevmiyeKurDovizKod);
            //            decimal kurFark = bildirimKur - avansKur;

            //            decimal hesaplananKurFarki = Math.Round(tutarFark * kurFark, 2);

            //            string kurFarkiStr = " Kur Farkı";
            //            if (hesaplananKurFarki > decimal.Zero)
            //            {
            //                int k = sb.eklenecekAciklama.IndexOf(kurFarkiStr);
            //                if (k > -1)
            //                {
            //                    int fiyatKoordinatBaslangic = sb.eklenecekAciklama.IndexOf('(', k);
            //                    int fiyatKoordinatBitis = sb.eklenecekAciklama.IndexOf(')', k);
            //                    decimal fiyat = OrtakFonksiyonlar.ConvertToDecimal(sb.eklenecekAciklama.Substring(fiyatKoordinatBaslangic + 1, fiyatKoordinatBitis - fiyatKoordinatBaslangic - 4), decimal.Zero);


            //                    sb.eklenecekAciklama = sb.eklenecekAciklama.Substring(0, k);

            //                    sb.eklenecekTutar += hesaplananKurFarki - fiyat;
            //                    sb.eklenecekAciklama += " Kur Farkı (" + hesaplananKurFarki.ToString("N2") + " TL)";

            //                }
            //            }
            //            else if (hesaplananKurFarki < decimal.Zero)
            //            {
            //                int k = sb.cikarilacakAciklama.IndexOf(kurFarkiStr);
            //                if (k > -1)
            //                {
            //                    int fiyatKoordinatBaslangic = sb.cikarilacakAciklama.IndexOf('(', k);
            //                    int fiyatKoordinatBitis = sb.cikarilacakAciklama.IndexOf(')', k);
            //                    decimal fiyat = OrtakFonksiyonlar.ConvertToDecimal(sb.cikarilacakAciklama.Substring(fiyatKoordinatBaslangic + 1, fiyatKoordinatBitis - fiyatKoordinatBaslangic - 4), decimal.Zero);

            //                    sb.cikarilacakAciklama = sb.cikarilacakAciklama.Substring(0, k);

            //                    sb.cikarilacakTutar += Math.Abs(hesaplananKurFarki) - Math.Abs(fiyat);
            //                    sb.cikarilacakAciklama += " Kur Farkı (" + hesaplananKurFarki.ToString("N2") + " TL)";
            //                }
            //            }

            //            sonuc = toGeciciGorevYolluk.Kaydet(sb);
            //        }
            //    }
            //}

            //Kaydetme Kısmı
            int sira = 0;
            foreach (GeciciGorevYollukDetay sDetay in sb.detay.objeler)
            {
                sira++;
                sDetay.yil = sb.yil;
                sDetay.muhasebeKod = sb.muhasebeKod;
                sDetay.birimKod = sb.birimKod;
                sDetay.surecNo = sb.surecNo;
                sDetay.belgeTur = sb.belgeTur;
                sDetay.siraNo = sira;
                sDetay.kayitSiraNo = sb.kayitSiraNo;
                sonuc = KaydetDetay(ve, kullanan, sDetay);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //Süreç devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "" && surecNo != "-1")
                sonuc = new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
            ///////////////////////////////////////////////////////////////////////////

            //Parca Adım devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "" && surecNo != "-1")
                sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = sb.surecNo;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisini GeciciGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public GeciciGorevYollukBelge Listele(Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            GeciciGorevYollukBelge bilgi = null;
            if (string.IsNullOrEmpty(sb.surecNo)) return bilgi;
            if (sb.surecNo == "-1" && string.IsNullOrEmpty(sb.tcKimlikNo)) return bilgi;
            if (sb.surecNo != "-1") sb.tcKimlikNo = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);
            IDataReader dr = TABGeciciGorevYolluk.Listele(sb);

            if (TABGeciciGorevYolluk.Hata() == "")
            {
                if (dr.Read())
                {
                    bilgi = DRDenOku(dr);
                }
                dr.Close();
                if (bilgi != null)
                    bilgi.detay = DetayListele(ve, kullanan, sb);
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        private GeciciGorevYollukBelge DRDenOku(IDataReader dr)
        {
            GeciciGorevYollukBelge bilgi = new GeciciGorevYollukBelge();
            bilgi.yil = VeriErisim.DegerAl(dr, 0, 0);
            bilgi.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
            bilgi.birimKod = VeriErisim.DegerAl(dr, 2, "");
            bilgi.surecNo = VeriErisim.DegerAl(dr, 3, "");
            bilgi.belgeTur = (ENUMYollukGundelikTuru)VeriErisim.DegerAl(dr, 4, 0);
            bilgi.tcKimlikNo = VeriErisim.DegerAl(dr, 5, "");
            bilgi.kadroEk = VeriErisim.DegerAl(dr, 6, "");
            bilgi.gundelikYil = VeriErisim.DegerAl(dr, 7, 0);
            bilgi.gundelikKod = VeriErisim.DegerAl(dr, 8, "");
            bilgi.dovizKodu = VeriErisim.DegerAl(dr, 9, 0);
            bilgi.dovizKuru = (decimal)VeriErisim.DegerAl(dr, 10, 0.0);
            bilgi.unvanKodu = VeriErisim.DegerAl(dr, 11, 0);
            bilgi.bildirimTarihi = VeriErisim.DegerAl(dr, 12);
            bilgi.odemeEmriNo = VeriErisim.DegerAl(dr, 13, "");
            bilgi.eklenecekAciklama = VeriErisim.DegerAl(dr, 14, "");
            bilgi.eklenecekTutar = (decimal)VeriErisim.DegerAl(dr, 15, 0.0);
            bilgi.eklenecekTutarParaBirimi.kod = VeriErisim.DegerAl(dr, 16, 0);
            bilgi.cikarilacakAciklama = VeriErisim.DegerAl(dr, 17, "");
            bilgi.cikarilacakTutar = (decimal)VeriErisim.DegerAl(dr, 18, 0.0);
            bilgi.cikarilacakTutarParaBirimi.kod = VeriErisim.DegerAl(dr, 19, 0);
            bilgi.arttirimliHesapla = Convert.ToBoolean(VeriErisim.DegerAl(dr, 20, 0));
            bilgi.kayitSiraNo = VeriErisim.DegerAl(dr, 21, 0);
            bilgi.odemeOrani = VeriErisim.DegerAl(dr, 22, "");
            bilgi.emanetOrani = VeriErisim.DegerAl(dr, 23, 0.0);
            bilgi.avansAlindiMi = Convert.ToBoolean(VeriErisim.DegerAl(dr, 24, 0));
            bilgi.kurTarihi = VeriErisim.DegerAl(dr, 25);
            bilgi.konaklamaTamamiOdenecekMi = Convert.ToBoolean(VeriErisim.DegerAl(dr, 26, 0));
            bilgi.yurtIciKonaklamaTuru = VeriErisim.DegerAl(dr, 27, 0);
            bilgi.adSoyad = VeriErisim.DegerAl(dr, 28, "") + "  " + VeriErisim.DegerAl(dr, 29, "");
            bilgi.unvanAdi = VeriErisim.DegerAl(dr, 30, "");
            bilgi.gundelikDeger = (decimal)VeriErisim.DegerAl(dr, 31, 0.0);
            return bilgi;
        }

        public ObjectArray ListeleAyrintili(Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);
            IDataReader dr = TABGeciciGorevYolluk.Listele(sb);

            if (TABGeciciGorevYolluk.Hata() == "")
            {
                while (dr.Read())
                {
                    GeciciGorevYollukBelge bilgi = DRDenOku(dr);
                    liste.Ekle(bilgi);
                }
            }

            dr.Close();
            ve.VeriErisimKapat();

            foreach (GeciciGorevYollukBelge b in liste.objeler)
            {
                b.detay = DetayListele(null, kullanan, b);
            }

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TumListe(Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.TumListe(sb);

            while (dr.Read())
            {
                GeciciGorevYollukBelge bilgi = new GeciciGorevYollukBelge();
                bilgi.yil = VeriErisim.DegerAl(dr, 0, 0);
                bilgi.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                bilgi.birimKod = VeriErisim.DegerAl(dr, 2, "");
                bilgi.surecNo = VeriErisim.DegerAl(dr, 3, "");
                bilgi.odemeEmriNo = VeriErisim.DegerAl(dr, 4, "");
                bilgi.bildirimTarihi = VeriErisim.DegerAl(dr, 5);

                liste.Ekle(bilgi);
            }
            dr.Close();

            if (liste.ObjeSayisi == 0)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00286;
            }
            else
                liste.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, toplu geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri GeciciGorevYollukToplu nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetToplu(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            GeciciGorevYollukToplu sb = ((GeciciGorevYollukToplu)liste.objeler[0]);
            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = sb.yil;
            string surecNo = sb.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.hataStr != String.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk toGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            ve.BeginTransaction();

            //Kaydetme Kısmı
            foreach (GeciciGorevYollukToplu yolluk in liste.objeler)
            {
                if (yolluk.sil)
                    sonuc = toGeciciGorevYolluk.SilToplu(yolluk);
                else
                    sonuc = toGeciciGorevYolluk.KaydetToplu(yolluk);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //Süreç devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "")
                sonuc = new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
            ///////////////////////////////////////////////////////////////////////////

            //Parca Adım devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "")
                sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = sb.surecNo;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KaydetTopluBelge(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = new Sonuc();
            foreach (GeciciGorevYollukBelge belge in liste.objeler)
            {
                sonuc = Kaydet(null, kullanan, belge, islemKod, parcaKod);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleToplu(Kullanici kullanan, GeciciGorevYollukToplu sb)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk toGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = toGeciciGorevYolluk.ListeleToplu(sb);

            while (dr.Read())
            {
                GeciciGorevYollukToplu sd = new GeciciGorevYollukToplu();

                sd.yil = VeriErisim.DegerAl(dr, 0, 0);
                sd.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                sd.birimKod = VeriErisim.DegerAl(dr, 2, "");
                sd.surecNo = VeriErisim.DegerAl(dr, 3, "");
                sd.tcKimlikNo = VeriErisim.DegerAl(dr, 4, "");
                sd.belgeTur = VeriErisim.DegerAl(dr, 5, 0);
                sd.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                sd.cektigiAvans = (decimal)VeriErisim.DegerAl(dr, 7, 0.0);
                sd.yollukTutari = (decimal)VeriErisim.DegerAl(dr, 8, 0.0);
                sd.ptt = (decimal)VeriErisim.DegerAl(dr, 9, 0.0);
                sd.damgaVergisi = (decimal)VeriErisim.DegerAl(dr, 10, 0.0);
                sd.eleGecen = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sd.mahsupEdilen = (decimal)VeriErisim.DegerAl(dr, 12, 0.0);
                sd.odenecekTutar = (decimal)VeriErisim.DegerAl(dr, 13, 0.0);
                sd.aciklama = VeriErisim.DegerAl(dr, 14, "");
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 15, "");
                sd.gelirVergisi = (decimal)VeriErisim.DegerAl(dr, 16, 0.0);
                sd.adSoyad = VeriErisim.DegerAl(dr, 17, "");
                sd.unvan = VeriErisim.DegerAl(dr, 18, "");
                sd.banka = VeriErisim.DegerAl(dr, 19, "");
                sd.sube = VeriErisim.DegerAl(dr, 20, "");
                sd.hesapNo = VeriErisim.DegerAl(dr, 21, "");

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge bilgisine ait banka listesini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleTopluBankaListesi(Kullanici kullanan, GeciciGorevYollukToplu sb)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk toGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = toGeciciGorevYolluk.ListeleTopluBankaListesi(sb);

            while (dr.Read())
            {
                GeciciGorevYollukToplu sd = new GeciciGorevYollukToplu();
                sd.tcKimlikNo = VeriErisim.DegerAl(dr, 0, "");
                sd.odenecekTutar = (decimal)VeriErisim.DegerAl(dr, 1, 0.0);
                sd.adSoyad = VeriErisim.DegerAl(dr, 2, "");
                sd.unvan = VeriErisim.DegerAl(dr, 3, "");
                sd.banka = VeriErisim.DegerAl(dr, 4, "");
                sd.sube = VeriErisim.DegerAl(dr, 5, "");
                sd.hesapNo = VeriErisim.DegerAl(dr, 6, "");

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            ve.BeginTransaction();

            sonuc = to.OdemeEmriNoKaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, toplu geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukToplu kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            ve.BeginTransaction();

            sonuc = to.OdemeEmriNoKaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belgesini, geçici görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HavuzSil(Kullanici islemYapan, GeciciGorevYollukBelge sb)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemYapan));

            TABGeciciGorevYolluk toGorevYolluk = new TABGeciciGorevYolluk(ve);
            TABGenel tabGenel = new TABGenel(ve);

            sb.muhasebeKod = -1;
            sb.birimKod = "-1";
            sb.surecNo = "-1";

            ve.BeginTransaction();
            Sonuc sonuc = toGorevYolluk.HavuzSil(sb);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleIstatistik(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray taksiListe = new ObjectArray();
            ObjectArray taksiDListe = new ObjectArray();
            ObjectArray yolListe = new ObjectArray();
            ObjectArray yolDListe = new ObjectArray();
            ObjectArray otelListe = new ObjectArray();

            PSLYollukIstatistikSinif ps = new PSLYollukIstatistikSinif();
            ps.yil = 2009;

            taksiDListe = (new PSLGeciciGorevYolluk()).ListeleIstatistikTaksiKurlu(kullanan, ps);

            taksiListe = (new PSLGeciciGorevYolluk()).ListeleIstatistikTaksiKursuz(kullanan, ps);

            TaksiGercekDegerler(taksiDListe, taksiListe);


            yolDListe = (new PSLGeciciGorevYolluk()).ListeleIstatistikYolKurlu(kullanan, ps);

            yolListe = (new PSLGeciciGorevYolluk()).ListeleIstatistikYolKursuz(kullanan, ps);

            YolGercekDegerler(yolDListe, yolListe);

            otelListe = (new PSLGeciciGorevYolluk()).ListeleIstatistikOtelKursuz(kullanan, ps);

            otelListe.objeler.AddRange(taksiListe.objeler);

            otelListe.objeler.AddRange(yolListe.objeler);

            return otelListe;

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="sb">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini GeciciGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public GeciciGorevYollukBelge OncedenKayitliMi(Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            GeciciGorevYollukBelge bilgi = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.OncedenKayitliMi(sb);

            while (dr.Read())
            {
                bilgi = new GeciciGorevYollukBelge();
                bilgi.yil = VeriErisim.DegerAl(dr, 0, 0);
                bilgi.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                bilgi.birimKod = VeriErisim.DegerAl(dr, 2, "");
                bilgi.surecNo = VeriErisim.DegerAl(dr, 3, "");
                bilgi.belgeTur = (ENUMYollukGundelikTuru)VeriErisim.DegerAl(dr, 4, 0);
                bilgi.tcKimlikNo = VeriErisim.DegerAl(dr, 5, "");
                GeciciGorevYollukDetay detay = new GeciciGorevYollukDetay();
                detay.hareketTarihi = VeriErisim.DegerAl(dr, 6);

                bilgi.detay.objeler.Add(detay);
                break;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yolculuk gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yolculuk gün sayısı bilgisini double olarak döner</returns>
        public double ToplamYolculukGunSayisiGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            double deger = to.ToplamYolculukGunSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam görev gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam görev gün sayısı bilgisini double olarak döner</returns>
        public double ToplamGorevGunSayisiGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            double deger = to.ToplamGorevGunSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam gün sayısı bilgisini double olarak döner</returns>
        public double ToplamGunSayisiGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            double toplamGorevGunSayisi = to.ToplamGorevGunSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            double toplamYolculukGunSayisi = to.ToplamYolculukGunSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            double toplam = toplamGorevGunSayisi + toplamYolculukGunSayisi;
            return toplam;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        public double ToplamYevmiyeTutarGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            double deger = to.ToplamYevmiyeTutar(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        public double ToplamAdamSayisiGetir(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            double deger = to.ToplamAdamSayisi(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            return deger;
        }

        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Ülke Adı, Görev Sayısı, Toplam Görev Gün Sayısı ve Toplam Tutar bilgileri bulunur.</returns>
        public ObjectArray ToplamGerceklesmeBilgileriListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            IDataReader dr = to.ToplamGerceklesmeListele(yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            ObjectArray liste;
            while (dr.Read())
            {
                liste = new ObjectArray();
                liste.objeler.Add(VeriErisim.DegerAl(dr, 0, ""));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 1, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 2, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 3, 0.0));
                donenListe.objeler.Add(liste);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini veritabanından okur 
        /// ve (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini, 
        /// (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak tutan nesne</returns>
        public ObjectArray ToplamGerceklesmeDenetimBilgileriListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, bool sadeceIl)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk to = new TABGeciciGorevYolluk(ve);
            IDataReader dr = to.ToplamGerceklesmeDenetimListele(yil, muhasebeKod, birimKod, sadeceIl);
            ObjectArray liste;
            while (dr.Read())
            {
                liste = new ObjectArray();
                liste.objeler.Add(VeriErisim.DegerAl(dr, 0, ""));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 1, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 2, 0));
                liste.objeler.Add(VeriErisim.DegerAl(dr, 3, 0.0));
                donenListe.objeler.Add(liste);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YIGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YIGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYIGGYIstatistik(int yil, ENUMYIGGYTur tur)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleYIGGYIstatistik(yil, tur);

            YIGGYIstatistik yollukIst = new YIGGYIstatistik();

            while (dr.Read())
            {
                yollukIst = new YIGGYIstatistik();
                yollukIst.yil = yil;
                yollukIst.tur = (int)tur;
                yollukIst.gorevSayisi = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.toplamGunSayisi = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.toplamTutar = VeriErisim.DegerAl(dr, 2, 0);

                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı geçici görev yolluk istatistik bilgisi listesini YDGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYDGGYIstatistik(int yil, ENUMYDGGYTur tur)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleYDGGYIstatistik(yil, tur);

            YDGGYIstatistik yollukIst = new YDGGYIstatistik();

            while (dr.Read())
            {
                yollukIst = new YDGGYIstatistik();
                yollukIst.yil = yil;
                yollukIst.tur = (int)tur;
                yollukIst.gorevSayisi = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.toplamGunSayisi = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.toplamTutar = VeriErisim.DegerAl(dr, 2, 0);

                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YISGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YISGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYISGYIstatistik(int yil, ENUMYISGYTur tur)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            if (tur == ENUMYISGYTur.HEPSI)
            {
                IDataReader dr1 = TABGeciciGorevYolluk.ListeleYISGYIstatistik(yil, ENUMYISGYTur.EMEKLI);

                YISGYIstatistik yollukIst = new YISGYIstatistik();

                while (dr1.Read())
                {
                    yollukIst = new YISGYIstatistik();
                    yollukIst.yil = yil;
                    yollukIst.tur = (int)ENUMYISGYTur.YERDEGISTIRME;
                    yollukIst.atamaSayisi = VeriErisim.DegerAl(dr1, 0, 0);
                    yollukIst.toplamTutar = VeriErisim.DegerAl(dr1, 1, 0);

                    donenListe.objeler.Add(yollukIst);
                }
                dr1.Close();

                IDataReader dr2 = TABGeciciGorevYolluk.ListeleYISGYIstatistik(yil, ENUMYISGYTur.YERDEGISTIRME);
                while (dr2.Read())
                {
                    yollukIst = new YISGYIstatistik();
                    yollukIst.yil = yil;
                    yollukIst.tur = (int)ENUMYISGYTur.YERDEGISTIRME;
                    yollukIst.atamaSayisi = VeriErisim.DegerAl(dr2, 0, 0);
                    yollukIst.toplamTutar = VeriErisim.DegerAl(dr2, 1, 0);

                    donenListe.objeler.Add(yollukIst);
                }
                dr2.Close();
            }
            else
            {
                IDataReader dr = TABGeciciGorevYolluk.ListeleYISGYIstatistik(yil, tur);

                YISGYIstatistik yollukIst = new YISGYIstatistik();

                while (dr.Read())
                {
                    yollukIst = new YISGYIstatistik();
                    yollukIst.yil = yil;
                    yollukIst.tur = (int)tur;
                    yollukIst.atamaSayisi = VeriErisim.DegerAl(dr, 0, 0);
                    yollukIst.toplamTutar = VeriErisim.DegerAl(dr, 1, 0);

                    donenListe.objeler.Add(yollukIst);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDSGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı sürekli görev yolluk istatistik bilgisi listesini YDSGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYDSGYIstatistik(int yil, ENUMYDSGYTur tur)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleYDSGYIstatistik(yil, tur);

            YDSGYIstatistik yollukIst = new YDSGYIstatistik();

            while (dr.Read())
            {
                yollukIst = new YDSGYIstatistik();
                yollukIst.yil = yil;
                yollukIst.tur = (int)tur;
                yollukIst.atamaSayisi = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.toplamTutar = VeriErisim.DegerAl(dr, 1, 0);

                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return donenListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve PSLSosyalYardimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belge numarası bilgisi listesini PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABGeciciGorevYolluk toGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = toGeciciGorevYolluk.ListeleOdemeEmri(yil, surecNo);

            while (dr.Read())
            {
                GeciciGorevYollukBelge sd = new GeciciGorevYollukBelge();
                sd.yil = VeriErisim.DegerAl(dr, 0, 0);
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 1, "");

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen nesnenin özelliklerinin uygunluğunu kontrol eder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="sb">Kontrolü yapılacak nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kontrol(GeciciGorevYollukBelge sb)
        {
            Sonuc sonuc = new Sonuc();

            //Belge Kontrol
            if (sb.yil == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (sb.muhasebeKod == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (sb.birimKod == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (sb.tcKimlikNo == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00236 + "<br>";
            if (sb.belgeTur == 0)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00288 + "<br>";
            if (sb.kadroEk == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00289 + "<br>";
            if (sb.belgeTur != ENUMYollukGundelikTuru.GECICIYURTDISI)
            {
                if (sb.gundelikKod == "")
                    sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00290 + "<br>";
            }
            if (sb.bildirimTarihi.isNull)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00291 + "<br>";
            if (sb.bildirimTarihi.Oku() > DateTime.Now)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00292 + "<br>";


            return sonuc;
        }

        private ObjectArray DetayListele(VeriErisim veriErisim, Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYollukDetay = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYollukDetay.ListeleDetay(sb);

            while (dr.Read())
            {
                GeciciGorevYollukDetay sd = new GeciciGorevYollukDetay();

                sd.yil = VeriErisim.DegerAl(dr, 0, 0);
                sd.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                sd.birimKod = VeriErisim.DegerAl(dr, 2, "");
                sd.surecNo = VeriErisim.DegerAl(dr, 3, "");
                sd.belgeTur = (ENUMYollukGundelikTuru)VeriErisim.DegerAl(dr, 4, 0);
                sd.tcKimlikNoBelge = VeriErisim.DegerAl(dr, 5, "");
                sd.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                sd.yolculukTipi = VeriErisim.DegerAl(dr, 7, 0);
                sd.yolculukGunSayi = (decimal)VeriErisim.DegerAl(dr, 8, 0.0);
                sd.yolculukGunSayiStr = VeriErisim.DegerAl(dr, 9, "");
                decimal pay;
                decimal payda;
                sd.yolculukGunSayi = OrtakServis.KesirDeger(sd.yolculukGunSayiStr, out pay, out payda);

                sd.nereden = VeriErisim.DegerAl(dr, 10, "");
                sd.nereye = VeriErisim.DegerAl(dr, 11, "");
                sd.hareketTarihi = VeriErisim.DegerAl(dr, 12);
                sd.hareketSaati = VeriErisim.DegerAl(dr, 13, "");
                sd.yevmiyeTutar = (decimal)VeriErisim.DegerAl(dr, 14, 0.0);
                sd.yevmiyeParaBirimi.kod = VeriErisim.DegerAl(dr, 15, 0);
                sd.ikamettenTerminaleAracTipi = VeriErisim.DegerAl(dr, 16, 0);
                sd.ikamettenTerminaleHarcanan = (decimal)VeriErisim.DegerAl(dr, 17, 0.0);
                sd.ikamettenTerminaleParaBirimi.kod = VeriErisim.DegerAl(dr, 18, 0);
                sd.yolculukAracTipi = VeriErisim.DegerAl(dr, 19, 0);
                sd.yolculuktaHarcanan = (decimal)VeriErisim.DegerAl(dr, 20, 0.0);
                sd.yolculuktaParaBirimi.kod = VeriErisim.DegerAl(dr, 21, 0);
                sd.terminaldenIkameteAracTipi = VeriErisim.DegerAl(dr, 22, 0);
                sd.terminaldenIkameteHarcanan = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                sd.terminaldenIkameteParaBirimi.kod = VeriErisim.DegerAl(dr, 24, 0);
                sd.gorevGunSayisi = VeriErisim.DegerAl(dr, 25, 0);
                sd.gorevYevmiyeToplami = (decimal)VeriErisim.DegerAl(dr, 26, 0.0);
                sd.konaklananGunSayi = (decimal)VeriErisim.DegerAl(dr, 27, 0.0);
                sd.konaklamaGunlukFatura = (decimal)VeriErisim.DegerAl(dr, 28, 0.0);
                sd.konaklamaGunlukFaturaParaBirimi.kod = VeriErisim.DegerAl(dr, 29, 0);
                sd.hamalBagaj = (decimal)VeriErisim.DegerAl(dr, 30, 0.0);
                sd.hamalBagajParaBirimi.kod = VeriErisim.DegerAl(dr, 31, 0);
                sd.toplamKonaklamaOdenen = (decimal)VeriErisim.DegerAl(dr, 32, 0.0);
                sd.toplamTutarDoviz = (decimal)VeriErisim.DegerAl(dr, 33, 0.0);
                sd.toplamTutar = (decimal)VeriErisim.DegerAl(dr, 34, 0.0);
                sd.kayitSiraNo = VeriErisim.DegerAl(dr, 35, 0);

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        private void TaksiGercekDegerler(ObjectArray kurluList, ObjectArray kursuzList)
        {

            foreach (PSLYollukIstatistikSinif kursuz in kursuzList.objeler)
            {
                foreach (PSLYollukIstatistikSinif kurlu in kurluList.objeler)
                {
                    if (kursuz.il == kurlu.il && kursuz.yollukTip == kurlu.yollukTip)
                    {
                        kursuz.taksi -= kurlu.taksi;
                        kursuz.taksi += kurlu.taksiDoviz;
                    }
                }

            }
        }

        private void YolGercekDegerler(ObjectArray kurluList, ObjectArray kursuzList)
        {

            foreach (PSLYollukIstatistikSinif kursuz in kursuzList.objeler)
            {
                foreach (PSLYollukIstatistikSinif kurlu in kurluList.objeler)
                {
                    if (kursuz.il == kurlu.il && kursuz.yollukTip == kurlu.yollukTip)
                    {
                        kursuz.yol -= kurlu.yol;
                        kursuz.yol += kurlu.yolDoviz;
                    }
                }

            }
        }

        private ObjectArray ListeleIstatistikTaksiKurlu(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleTaksiIstatistikKurlu(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.taksiDoviz = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.taksi = VeriErisim.DegerAl(dr, 4, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 5, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 6, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }
        
        private ObjectArray ListeleIstatistikTaksiKursuz(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleTaksiIstatistikKursuz(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.taksi = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 4, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 5, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }
        
        private ObjectArray ListeleIstatistikYolKurlu(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleYolIstatistikKurlu(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.yolDoviz = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.yol = VeriErisim.DegerAl(dr, 4, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 5, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 6, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }
        
        private ObjectArray ListeleIstatistikYolKursuz(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleYolIstatistikKursuz(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.yol = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 4, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 5, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }
        
        private ObjectArray ListeleIstatistikOtelKursuz(Kullanici kullanan, PSLYollukIstatistikSinif sb)
        {
            ObjectArray donenListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk TABGeciciGorevYolluk = new TABGeciciGorevYolluk(ve);

            IDataReader dr = TABGeciciGorevYolluk.ListeleOtelIstatistikKursuz(sb);

            PSLYollukIstatistikSinif yollukIst = new PSLYollukIstatistikSinif();

            while (dr.Read())
            {
                yollukIst = new PSLYollukIstatistikSinif();
                yollukIst.il = VeriErisim.DegerAl(dr, 0, 0);
                yollukIst.yollukTip = VeriErisim.DegerAl(dr, 1, 0);
                yollukIst.perSayi = VeriErisim.DegerAl(dr, 2, 0);
                yollukIst.konaklama = VeriErisim.DegerAl(dr, 3, 0.0);
                yollukIst.islemYapanSayi = VeriErisim.DegerAl(dr, 4, 0);
                yollukIst.islemSayisi = VeriErisim.DegerAl(dr, 5, 0);
                donenListe.objeler.Add(yollukIst);
            }
            dr.Close();


            ve.VeriErisimKapat();
            return donenListe;
        }

        //private decimal MerkezBankasiDovizKurGetir(TNSDateTime kurTarih, int paraBirimKod)
        //{
        //    decimal donusDeger = decimal.Zero;
        //    MerkezBankasiKurSinif sinif = new MerkezBankasiKurSinif();
        //    sinif.kurTarih = kurTarih;

        //    sinif = HRCMerkezBankKur.Oku(null, null, sinif);

        //    if (sinif != null && sinif.satirlar != null && sinif.satirlar.Length > 0)
        //    {
        //        for (int i = 0; i < sinif.satirlar.Length; i++)
        //        {
        //            if (sinif.satirlar[i].paraBirim.kod == paraBirimKod)
        //            {
        //                donusDeger = sinif.satirlar[i].dovizSatis;
        //            }
        //        }
        //    }
        //    return donusDeger;
        //}

        //private decimal KurBilgisiAl(VeriErisim veriErisim, Kullanici kullanan, KurBilgisi kur, int kod)
        //{
        //    //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    //TABKurBilgisi tab = new TABKurBilgisi(ve);

        //    if (kod == 1) //TL
        //        return 1;

        //    kur.paraBirimi.kod = kod;
        //    ObjectArray donusDeger = new HRCKurBilgisi().Listele(veriErisim, kullanan, kur);

        //    if (donusDeger.objeler.Count > 0)
        //        return ((KurBilgisi)donusDeger.objeler[0]).kur;
        //    else
        //        return 1;
        //}

        //private decimal GundelikTanimYevmiyeDegeri(string kod)
        //{
        //    foreach (YollukGundelikTanim yevmiye in gundelikTanimList.objeler)
        //    {
        //        if (yevmiye.kod == kod)
        //            return (decimal)yevmiye.deger;
        //    }
        //    return decimal.Zero;
        //}

        private Sonuc SilDetay(VeriErisim ve, Kullanici kullanan, GeciciGorevYollukDetay sDetay)
        {
            Sonuc sonuc = new Sonuc();

            TABGeciciGorevYolluk tabDetaySil = new TABGeciciGorevYolluk(ve);

            sonuc = tabDetaySil.SilDetay(sDetay);

            return sonuc;
        }

        private Sonuc KaydetDetay(VeriErisim ve, Kullanici kullanan, GeciciGorevYollukDetay sDetay)
        {
            Sonuc sonuc = new Sonuc();

            TABGeciciGorevYolluk tabDetay = new TABGeciciGorevYolluk(ve);

            sonuc = tabDetay.KaydetDetay(sDetay);

            return sonuc;
        }

        internal static Sonuc YollukBelgeSil(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciGorevYolluk tabDetaySil = new TABGeciciGorevYolluk(ve);

            ve.BeginTransaction();

            Sonuc sonuc = tabDetaySil.SilDetay(yil, surecNo);

            if (sonuc.islemSonuc)
                sonuc = tabDetaySil.SilAna(yil, surecNo);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
