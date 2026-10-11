using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

using System.Collections;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Sözleşmeli maaş bordro bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCSozlesmeliMaasBordro
    {
        private ObjectArray gelirVergisiList;
        private ObjectArray perMaasTanimList;
        private ObjectArray perOncekiMaasTanimList;
        private ObjectArray perGecenAyMaasList;


        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="maasHesaplalist">Kaydedilecek bilgileri PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray maasHesaplalist, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = Kontrol(maasHesaplalist);
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }


            //**************** ******* ** *** MAAŞ HESAPLAMA ************ ** * * * * * ** **********
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TNS.HRC.PSLSozlesmeliMaasBordro bordro = (TNS.HRC.PSLSozlesmeliMaasBordro)maasHesaplalist.objeler[0];

            //1. ADIM Sozlesmeli maasları için genel tanımlamaları al
            PSLMaasTemelBilgisi maasTemelBilgisi = MaasTemelBilgisiAl(kullanan, bordro.maasYil, bordro.maasAy);
            PSLSosyalYardimTemelBilgisi sosyalYardimTemelBilgisi = SosyalYardimTemelBilgisiAl(kullanan, bordro.maasYil, bordro.maasAy);

            //2. ADIM Gelir Vergisi Bilgisi
            gelirVergisiList = GelirVergisiListesiAl(kullanan, bordro.maasYil);

            //3. ADIM Maas Bordrosu Yapılacak Personel Listesinin tanım bilgilerini Al
            //--H--
            perMaasTanimList = PersonelMaasTanimListesiAl(kullanan, bordro.muhasebe, bordro.birim, bordro.maasYil, bordro.maasAy);

            //Hata var ise geri dön uyarı ver
            hataStr = KontrolHesaplama(maasTemelBilgisi, gelirVergisiList, perMaasTanimList);
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            if (bordro.bordroTur == (int)ENUMBordroTur.FARK)
            {
                int perMaasAy = bordro.maasAy - 1;
                int perMaasYil = bordro.maasYil;
                if (perMaasAy == 0)
                {
                    perMaasAy = 12;
                    perMaasYil = bordro.maasYil - 1;
                }
                PSLMaasTemelBilgisi oncekiMaasTemelBilgisi = MaasTemelBilgisiAl(kullanan, perMaasYil, perMaasAy);
                maasTemelBilgisi.aylikKatsayi -= oncekiMaasTemelBilgisi.aylikKatsayi;
            }

            //4. ADIM Bu ay maaşı hesaplanacakların kumilatif vergi matrahları için gecen ay maas listesini alalım
            //--H--
            perGecenAyMaasList = PersonelGecenAyMaasListesiAl(kullanan, bordro.muhasebe, bordro.birim, bordro.maasYil, bordro.maasAy);


            //5. ADIM Listeden Gelen Personellerin Maas Hesabını yapalım
            SozlesmeliBordroTanim perMaasTanim = null;
            PSLSozlesmeliMaasBordro perEskiMaas = null;
            IPERServis servisPER = TNS.PER.Arac.Tanimla();
            foreach (TNS.HRC.PSLSozlesmeliMaasBordro perMaas in maasHesaplalist.objeler)
            {
                //Risk! Personelin maaş tanımının girilmediği durum
                //Risk! Personelin onceki ay maaşının gelmediği bir durum

                perMaasTanim = PersonelMaasTanimBilgisiAl(perMaas.tCKimlikNo.Trim(), perMaasTanimList);

                if (KontrolPersonelMaasTanimBilgisi(perMaasTanim))
                {
                    perEskiMaas = PersonelEskiMaasBilgisiAl(perMaas.tCKimlikNo.Trim());

                    //A// ->girilen ücret->
                    if (perMaas.bordroTur == (int)ENUMBordroTur.FARK)
                    {
                        int perMaasAy = bordro.maasAy - 1;
                        int perMaasYil = bordro.maasYil;
                        if (perMaasAy == 0)
                        {
                            perMaasAy = 12;
                            perMaasYil = bordro.maasYil - 1;
                        }
                        if (perOncekiMaasTanimList == null)
                        {
                            perOncekiMaasTanimList = PersonelMaasTanimListesiAl(kullanan, bordro.muhasebe, bordro.birim, perMaasYil, perMaasAy);

                            if (perOncekiMaasTanimList.objeler.Count == 0)
                            {
                                sonuc.islemSonuc = false;
                                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00297, perMaas.tCKimlikNo);
                                return sonuc;
                            }
                        }
                        SozlesmeliBordroTanim oncekiMaasTanim = PersonelMaasTanimBilgisiAl(perMaas.tCKimlikNo, perOncekiMaasTanimList);
                        perMaasTanim.sozlesmeUcret -= oncekiMaasTanim.sozlesmeUcret;
                        perMaas.gun = 14;
                    }

                    perMaas.ucret = HesaplaUcret(perMaasTanim.sozlesmeUcret, perMaas.gun);

                    if (perMaas.bordroTur != (int)ENUMBordroTur.FARK)
                    {
                        if (perMaas.bordroTip == (int)ENUMBordroTip.SOZLESMELI4B)
                            perMaas.aileYardimi = HesaplaAileYardimi(servisPER, kullanan, maasTemelBilgisi, sosyalYardimTemelBilgisi, perMaas.tCKimlikNo);
                        else if (perMaas.bordroTip == (int)ENUMBordroTip.SOZLESMELI4C)
                            perMaas.aileYardimi = perMaas.ekTazminatOran;
                    }
                    if (perMaas.bordroTip == (int)ENUMBordroTip.SOZLESMELI4C)
                        perMaas.ekTazminatOran = 0;

                    //B//->ek tazminat
                    perMaas.ekTazminat = HesaplaEkTazminat(maasTemelBilgisi.enYuksMemurMaasGost, maasTemelBilgisi.aylikKatsayi, perMaasTanim.ekTazminatOrani, perMaas.gun);
                    perMaas.maxMaasGostergesi = maasTemelBilgisi.enYuksMemurMaasGost;
                    perMaas.aylikKatsayi = maasTemelBilgisi.aylikKatsayi;
                    perMaas.ekTazminatOran = perMaasTanim.ekTazminatOrani;

                    //C//->isveren payı->(A+B)* SSK işveren payi orani yüzde olarak(-genel tanım-)
                    //Eğer personel için tanımlı SSK işveren payi oranı yoksa genel tanımdaki değer alınarak işlem yapılır.
                    decimal sskIsverenPayiOran = perMaasTanim.sskIsverenPayiOrani;
                    if (sskIsverenPayiOran == 0)
                        sskIsverenPayiOran = maasTemelBilgisi.sskIsverenPayiOran;
                    perMaas.sskIsverenPayi = HesaplaSskIsverenPayi(0, perMaas.ucret, sskIsverenPayiOran);
                    perMaas.sskIsverenPayiOran = sskIsverenPayiOran;

                    //.:KmL:. 04.03.2009 Tahakkuk toplamına sendika devlet yardımı eklendi.
                    //D//->toplam tahakkuk-> A+B+C
                    perMaas.tahakkukToplam = HesaplaTahakkukToplam(perMaas.bordroTur, perMaas.ucret, perMaas.ekTazminat, perMaas.sskIsverenPayi, perMaasTanim.sendikaDevletYardimi, perMaas.aileYardimi);

                    //I//->SSK işçi payı oranı (A+B)*ssk işçi payı(-genel tanım-)
                    perMaas.sskIsciPayi = HesaplaSskIsciPayi(0, perMaas.ucret, maasTemelBilgisi.sskIsciPayiOran);
                    perMaas.sskIsciPayiOran = maasTemelBilgisi.sskIsciPayiOran;

                    //P//vergi matrahı A-I
                    //.:KmL:. 04.03.2009 Vergi Matrahından sakatlik indirimi ve ozel sigorta çıkartıldı.
                    perMaas.vergiMatrahi = HesaplaVergiMatrahi(perMaas.ucret, perMaas.sskIsciPayi, perMaasTanim.ozelSigorta, perMaasTanim.sakatlikIndirimi, perMaasTanim.sendikaAidati); //perMaas.ucret - perMaas.sskIsciPayi - perMaasTanim.ozelSigorta - perMaasTanim.sakatlikIndirimi - perMaasTanim.sendikaAidati

                    //O// Kumülatif vergi matrahı->Onceki ayın P lerinin toplamı
                    perMaas.kumVergiMatrahi = HesaplaKumulatifVergiMatrahi(perMaas, bordro, perEskiMaas, perMaasTanim);

                    //E//->asıl gelir vergisi-> [(A)-(I+Q+R)]*(gelir vergisi oranı -tanım-)
                    /***************************    ASIL GELIR VERGISI HESAPLA  ********************/
                    perMaas.asilGelirVergisi = HesaplaGelirVergisi(perMaas.kumVergiMatrahi, perMaas.vergiMatrahi, gelirVergisiList);
                    perMaas.sakatlikIndirim = perMaasTanim.sakatlikIndirimi;

                    //F//->asgari geçim indirimi->?
                    //asgari geçim indirimnde gelir vergisi dilimi en son girilen 
                    if (perMaas.bordroTur != (int)ENUMBordroTur.FARK)
                    {
                        perMaas.ozelSigorta = perMaasTanim.ozelSigorta;
                        perMaas.sendikaAidati = perMaasTanim.sendikaAidati;
                        perMaas.sendikaDevletYardimi = perMaasTanim.sendikaDevletYardimi;

                        PSLGelirVergisi gelirVergisi = gelirVergisiDilimi(0);
                        if (perMaasTanim.asgariGecimIndirim > 0)
                        {
                            perMaas.asgGecimIndirimi = HesaplaAsgariGecimIndirimi(maasTemelBilgisi.brutAsgariUcret, perMaasTanim.asgariGecimIndirim, gelirVergisi.yuzde);

                            if (perMaas.asgGecimIndirimi > perMaas.asilGelirVergisi)
                                perMaas.asgGecimIndirimi = perMaas.asilGelirVergisi;
                        }
                        else
                            perMaas.asgGecimIndirimi = 0;

                        gelirVergisi = gelirVergisiDilimi(perMaas.kumVergiMatrahi);
                        perMaas.gelirVergisiOran = gelirVergisi.yuzde;
                    }
                    else
                        perMaas.asgGecimIndirimi = 0;

                    perMaas.brutAsgariUcret = maasTemelBilgisi.brutAsgariUcret;
                    perMaas.asgGecimIndKatsayi = perMaasTanim.asgariGecimIndirim;
                    //G//->kesilen gelir vergisi->E-F
                    perMaas.gelirVergisi = perMaas.asilGelirVergisi - perMaas.asgGecimIndirimi;

                    //.:KmL:. 04.03.2009 Damga Vergisi Hesabına Sendika Devlet Yardımı eklendi.
                    //H//->damga vergisi->(A+B)*damga vergisi oranı(-tanım-) 
                    perMaas.damgaVergisi = HesaplaDamgaVergisi(perMaas.ucret, perMaas.ekTazminat, perMaasTanim.sendikaDevletYardimi, maasTemelBilgisi.damgaVergisiOran);
                    perMaas.damgaVergisiOran = maasTemelBilgisi.damgaVergisiOran;

                    //J// giriş lojman
                    //K// giriş icra
                    //L// giriş diğer kesinti
                    perMaas.lojman = perMaasTanim.lojman;
                    perMaas.icra = perMaasTanim.icra;
                    perMaas.digerKesinti = perMaasTanim.digerKesinti;
                    perMaas.nafaka = perMaasTanim.nafaka;

                    //.:KmL:. 04.03.2009 Kesinti toplamına nafaka ve sendika aidatı eklendi.
                    //M// kesinti toplamı G+H+I+J+K+L+C
                    perMaas.kesintiToplam = HesaplaKesintiToplam(perMaas, perMaasTanim);
                    //N// Ödenmesi gereken miktar D-M
                    perMaas.odenmesiGereken = perMaas.tahakkukToplam - perMaas.kesintiToplam;
                }
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            int yil = bordro.yil;
            string surecNo = bordro.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////


            TABSozlesmeliMaasBordro to = new TABSozlesmeliMaasBordro(ve);
            ve.BeginTransaction();

            if (bordro.bordroTur == (int)ENUMBordroTur.AYLIK)
                sonuc = to.Sil(bordro);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(maasHesaplalist);

            if (sonuc.islemSonuc)
            {
                //Süreç devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                ///////////////////////////////////////////////////////////////////////////

                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

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

        private decimal HesaplaKesintiToplam(PSLSozlesmeliMaasBordro perMaas, SozlesmeliBordroTanim perMaasTanim)
        {
            decimal kesintiToplam = decimal.Zero;

            kesintiToplam += perMaas.gelirVergisi +
                             perMaas.damgaVergisi +
                             perMaas.sskIsciPayi +
                             perMaasTanim.lojman +
                             perMaasTanim.icra +
                             perMaasTanim.nafaka +
                             perMaasTanim.sendikaAidati +
                             perMaasTanim.digerKesinti +
                             perMaas.sskIsverenPayi;

            return Math.Round(kesintiToplam, 2);
        }

        private decimal HesaplaDamgaVergisi(decimal perMaasUcret, decimal ekTazminat, decimal sendikaDevletYardimi, decimal damgaVergisiOran)
        {
            return Math.Round((perMaasUcret + ekTazminat + sendikaDevletYardimi) * (damgaVergisiOran / 100), 2);
        }

        private decimal HesaplaAsgariGecimIndirimi(decimal brutAsgariUcret, decimal asgariGecimIndirim, decimal gelirVergisiYuzde)
        {
            return Math.Round(brutAsgariUcret * (asgariGecimIndirim / 100) * (gelirVergisiYuzde / 100), 2);
        }

        private decimal HesaplaKumulatifVergiMatrahi(PSLSozlesmeliMaasBordro perMaas, PSLSozlesmeliMaasBordro bordro, PSLSozlesmeliMaasBordro perEskiMaas, SozlesmeliBordroTanim perMaasTanim)
        {
            decimal kumVergiMatrahi = decimal.Zero;

            if (bordro.maasAy == 1)
                kumVergiMatrahi = perMaas.vergiMatrahi;
            else if (perEskiMaas.maasAy == perMaasTanim.maasAyBaslangic &&
                     perEskiMaas.maasYil == perMaasTanim.maasYilBaslangic &&
                     perMaasTanim.kumulatifVergiMatrahiBaslangic > 0)
                kumVergiMatrahi = perMaasTanim.kumulatifVergiMatrahiBaslangic + perMaas.vergiMatrahi;
            else
                kumVergiMatrahi = perEskiMaas.kumVergiMatrahi + perMaas.vergiMatrahi;

            return Math.Round(kumVergiMatrahi, 2);
        }

        private decimal HesaplaVergiMatrahi(decimal perMaasUcret, decimal sskIsciPayi, decimal ozelSigorta, decimal sakatlikIndirimi, decimal sendikaAidati)
        {
            return Math.Round(perMaasUcret - sskIsciPayi - ozelSigorta - sakatlikIndirimi - sendikaAidati, 2);
        }

        private decimal HesaplaSskIsciPayi(decimal ekTazminat, decimal perMaasUcret, decimal sskIsciPayiOran)
        {
            return Math.Round((ekTazminat + perMaasUcret) * (sskIsciPayiOran / 100), 2, MidpointRounding.AwayFromZero);
        }

        private decimal HesaplaTahakkukToplam(int bordroTur, decimal perMaasUcret, decimal ekTazminat, decimal sskIsverenPayi, decimal sendikaDevletYardimi, decimal aileYardimi)
        {
            decimal toplam = Math.Round(perMaasUcret + ekTazminat + sskIsverenPayi + sendikaDevletYardimi + aileYardimi, 2);
            if (bordroTur == (int)ENUMBordroTur.FARK)
                toplam = Math.Round(perMaasUcret + ekTazminat + sskIsverenPayi, 2);
            return toplam;
        }

        private decimal HesaplaSskIsverenPayi(decimal ekTazminat, decimal perMaasUcret, decimal sskIsverenPayiOran)
        {
            return Math.Round((ekTazminat + perMaasUcret) * (sskIsverenPayiOran / 100), 2);
        }

        private decimal HesaplaUcret(decimal sozlesmeUcret, int perMaasGun)
        {
            return Math.Round((sozlesmeUcret * perMaasGun) / 30, 2);
        }

        private decimal HesaplaEkTazminat(decimal enYuksMemurMaasGost, decimal aylikKatsayi, decimal ekTazminatOrani, int perMaasGun)
        {
            return Math.Round((enYuksMemurMaasGost * aylikKatsayi * (ekTazminatOrani / 100)) * perMaasGun / 30, 2);
        }

        private decimal HesaplaAileYardimi(IPERServis servisPER, Kullanici kullanan, PSLMaasTemelBilgisi maasTemelBilgisi, PSLSosyalYardimTemelBilgisi sosyalYardimTemelBilgisi, string personelTCKimlikNo)
        {
            PersonelAile perAileKriter = new PersonelAile();
            //perAileKriter.personelTcKimlikNo = kriter.TCKimlikNo;
            //.:KmL:. 22.07.08
            //Personel Aile Listeleme bölümünde kriter olarak tcKimlikNo belirlenmiş,
            //personelTcKimlik No belirlenmemiş düzeltilmesi lazım 
            perAileKriter.personelTcKimlikNo = personelTCKimlikNo;
            //perAileKriter.tcKimlikNo = personelTCKimlikNo;
            ObjectArray perBilgiVan = servisPER.PersonelAileListele(kullanan, perAileKriter);

            decimal toplam = decimal.Zero;
            decimal aylikKatsayi = maasTemelBilgisi.aylikKatsayi;
            decimal cocukKatsayi = decimal.Zero;

            foreach (PersonelAile aileFerdi in perBilgiVan.objeler)
            {
                if (aileFerdi.yakinlik == 1 && aileFerdi.durum == 5) //Eşi && durumu çalışmıyor 
                    toplam += Math.Round(aylikKatsayi * sosyalYardimTemelBilgisi.aileYardimGost, 2);
                else if (aileFerdi.yakinlik == 2) //Çocuk
                {
                    cocukKatsayi = CocukSosyalYardimKatsayiGetir(sosyalYardimTemelBilgisi, aileFerdi.dogumTarihi);
                    toplam += Math.Round(aylikKatsayi * cocukKatsayi, 2);
                }
            }
            return Math.Round(toplam, 2);
        }

        private decimal HesaplaAsilGelirVergisini(decimal gecenAyKumilatifVergiMatrahi, decimal buAyEklenenVergiMatrahi)
        {
            decimal toplamVergi = 0;

            PSLGelirVergisi ilkGelirVergisi = gelirVergisiDilimi(gecenAyKumilatifVergiMatrahi);
            PSLGelirVergisi ikinciGelirVergisi = gelirVergisiDilimi(gecenAyKumilatifVergiMatrahi + buAyEklenenVergiMatrahi);

            if (ilkGelirVergisi.yuzde == ikinciGelirVergisi.yuzde)
            {
                toplamVergi = (ilkGelirVergisi.yuzde * buAyEklenenVergiMatrahi) / 100;
            }
            else
            {
                decimal ilkPay = (ilkGelirVergisi.miktar - gecenAyKumilatifVergiMatrahi);
                toplamVergi = ilkPay * ilkGelirVergisi.yuzde / 100;
                decimal ikinciPay = buAyEklenenVergiMatrahi - ilkPay;
                toplamVergi += ikinciPay * ikinciGelirVergisi.yuzde / 100;
            }
            return Math.Round(toplamVergi, 2);
        }

        private decimal HesaplaGelirVergisi(decimal persKumilatifVergi, decimal vergiMatrahi, ObjectArray gelirVergisiList)
        {
            for (int i = 0; i < gelirVergisiList.objeler.Count; i++)
            {
                PSLGelirVergisi vergiDilimi = (PSLGelirVergisi)gelirVergisiList.objeler[i];
                if (persKumilatifVergi < vergiDilimi.miktar)
                {
                    if (i > 0)
                    {
                        PSLGelirVergisi oncekiDilim = (PSLGelirVergisi)gelirVergisiList.objeler[i - 1];
                        if (persKumilatifVergi - oncekiDilim.miktar < vergiMatrahi)
                        {

                            decimal oncekiDilimMiktar = persKumilatifVergi - oncekiDilim.miktar;
                            decimal oncekiDilimVergi = Math.Round(oncekiDilimMiktar * vergiDilimi.yuzde / 100, 2);

                            decimal artanKisimMiktar = vergiMatrahi - oncekiDilimMiktar;
                            decimal artanKisimVergi = Math.Round(artanKisimMiktar * oncekiDilim.yuzde / 100, 2);

                            return oncekiDilimVergi + artanKisimVergi;

                        }
                        else
                            return Math.Round(vergiMatrahi * vergiDilimi.yuzde / 100, 2);
                    }
                    else
                    {
                        return Math.Round(vergiMatrahi * vergiDilimi.yuzde / 100, 2);
                    }
                }
            }
            return 0;
        }

        private string Kontrol(ObjectArray maasHesaplalist)
        {
            string hataStr = "";
            int satir = 0;

            if (maasHesaplalist.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (TNS.HRC.PSLSozlesmeliMaasBordro ida in maasHesaplalist.objeler)
                {
                    satir++;

                    if (ida.yil == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                        break;
                    }
                    if (ida.muhasebe == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                        break;
                    }
                    if (ida.birim.Trim() == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                        break;
                    }
                    if (ida.surecNo == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                        break;
                    }
                    if (ida.tCKimlikNo == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00214, satir) + "<br>";
                    if (ida.maasYil == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00215, satir) + "<br>";
                    if (ida.maasAy == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00216, satir) + "<br>";

                }
            }
            return hataStr;
        }

        private string KontrolHesaplama(PSLMaasTemelBilgisi maasTemelBilgisi, ObjectArray gelirVergisiList, ObjectArray perMaasTanimList)
        {
            string hataStr = string.Empty;
            if (maasTemelBilgisi == null)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00218;
            if (gelirVergisiList.ObjeSayisi == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00219 + "<br>";
            if (perMaasTanimList.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00220;

            return hataStr;
        }

        private decimal CocukSosyalYardimKatsayiGetir(PSLSosyalYardimTemelBilgisi sosyalYardimTemelBilgisi, TNSDateTime dogumTarihi)
        {
            if (DateTime.Now.Year - dogumTarihi.Yil > 6)
                return sosyalYardimTemelBilgisi.cocukYardimGost6_X;
            if (DateTime.Now.Year - dogumTarihi.Yil < 6)
                return sosyalYardimTemelBilgisi.cocukYardimGost0_6;
            else if ((DateTime.Now.Year - dogumTarihi.Yil == 6) && (DateTime.Now.Month == dogumTarihi.Ay))
            {
                if (DateTime.Now.Day >= dogumTarihi.Gun)
                    return sosyalYardimTemelBilgisi.cocukYardimGost6_X;
                else if (DateTime.Now.Day < dogumTarihi.Gun)
                    return sosyalYardimTemelBilgisi.cocukYardimGost0_6;
            }
            else if ((DateTime.Now.Year - dogumTarihi.Yil == 6) && (DateTime.Now.Month <= dogumTarihi.Ay))
                return sosyalYardimTemelBilgisi.cocukYardimGost0_6;
            else if ((DateTime.Now.Year - dogumTarihi.Yil == 6) && (DateTime.Now.Month > dogumTarihi.Ay))
                return sosyalYardimTemelBilgisi.cocukYardimGost6_X;

            return sosyalYardimTemelBilgisi.cocukYardimGost0_6;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="f">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, PSLSozlesmeliMaasBordro f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = false;

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            int yil = f.yil;
            string surecNo = f.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSozlesmeliMaasBordro to = new TABSozlesmeliMaasBordro(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(f);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
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
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini veritabanından okur 
        /// ve boolean olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini boolean olarak döner</returns>
        public bool FarkBordrosuVarMi(Kullanici kullanan, PSLSozlesmeliMaasBordro kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSozlesmeliMaasBordro to = new TABSozlesmeliMaasBordro(ve);
            bool donusDeger = false;
            if (to.FarkBordrosuVarmi(kriter) > 0)
            {
                donusDeger = true;
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSozlesmeliMaasBordro to = new TABSozlesmeliMaasBordro(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.HRC.PSLSozlesmeliMaasBordro bilgi = new TNS.HRC.PSLSozlesmeliMaasBordro();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tCKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    bilgi.maasYil = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.maasAy = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.siraNo = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.gun = VeriErisim.DegerAl(r, 8, 0);
                    bilgi.ucret = (decimal)VeriErisim.DegerAl(r, 9, 0.0);
                    bilgi.ekTazminat = (decimal)VeriErisim.DegerAl(r, 10, 0.0);
                    bilgi.asgGecimIndirimi = (decimal)VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.ozelSigorta = (decimal)VeriErisim.DegerAl(r, 12, 0.0);
                    bilgi.sakatlikIndirim = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    bilgi.gelirVergisiOran = (decimal)VeriErisim.DegerAl(r, 14, 0.0);
                    bilgi.gelirVergisi = (decimal)VeriErisim.DegerAl(r, 15, 0.0);
                    bilgi.damgaVergisiOran = (decimal)VeriErisim.DegerAl(r, 16, 0.0);
                    bilgi.damgaVergisi = (decimal)VeriErisim.DegerAl(r, 17, 0.0);
                    bilgi.asilGelirVergisi = (decimal)VeriErisim.DegerAl(r, 18, 0.0);
                    bilgi.sskIsverenPayiOran = (decimal)VeriErisim.DegerAl(r, 19, 0.0);
                    bilgi.sskIsverenPayi = (decimal)VeriErisim.DegerAl(r, 20, 0.0);
                    bilgi.sskIsciPayiOran = (decimal)VeriErisim.DegerAl(r, 21, 0.0);
                    bilgi.sskIsciPayi = (decimal)VeriErisim.DegerAl(r, 22, 0.0);
                    bilgi.lojman = (decimal)VeriErisim.DegerAl(r, 23, 0.0);
                    bilgi.icra = (decimal)VeriErisim.DegerAl(r, 24, 0.0);
                    bilgi.digerKesinti = (decimal)VeriErisim.DegerAl(r, 25, 0.0);
                    bilgi.kesintiToplam = (decimal)VeriErisim.DegerAl(r, 26, 0.0);
                    bilgi.odenmesiGereken = (decimal)VeriErisim.DegerAl(r, 27, 0.0);
                    bilgi.kumVergiMatrahi = (decimal)VeriErisim.DegerAl(r, 28, 0.0);
                    bilgi.vergiMatrahi = (decimal)VeriErisim.DegerAl(r, 29, 0.0);
                    bilgi.tahakkukToplam = (decimal)VeriErisim.DegerAl(r, 30, 0.0);
                    bilgi.maxMaasGostergesi = (decimal)VeriErisim.DegerAl(r, 31, 0.0);
                    bilgi.aylikKatsayi = (decimal)VeriErisim.DegerAl(r, 32, 0.0);
                    bilgi.brutAsgariUcret = (decimal)VeriErisim.DegerAl(r, 33, 0.0);
                    bilgi.issizSigPrimOran = (decimal)VeriErisim.DegerAl(r, 34, 0.0);
                    bilgi.ekTazminatOran = (decimal)VeriErisim.DegerAl(r, 35, 0.0);
                    bilgi.asgGecimIndKatsayi = (decimal)VeriErisim.DegerAl(r, 36, 0.0);
                    bilgi.bankaKodu = VeriErisim.DegerAl(r, 37, "");
                    bilgi.bankaSubeKodu = VeriErisim.DegerAl(r, 38, "");
                    bilgi.bankaHesapNo = VeriErisim.DegerAl(r, 39, "");
                    bilgi.unvanKodu = VeriErisim.DegerAl(r, 40, 0);
                    bilgi.medeniDurumu = VeriErisim.DegerAl(r, 41, 0);
                    bilgi.sendikaDevletYardimi = (decimal)VeriErisim.DegerAl(r, 42, 0.0);
                    bilgi.sendikaAidati = (decimal)VeriErisim.DegerAl(r, 43, 0.0);
                    bilgi.nafaka = (decimal)VeriErisim.DegerAl(r, 44, 0.0);
                    bilgi.bordroTur = VeriErisim.DegerAl(r, 45, 0);
                    bilgi.aileYardimi = (decimal)VeriErisim.DegerAl(r, 46, 0.0);

                    bilgi.bankaAdi = VeriErisim.DegerAl(r, 47, "");
                    bilgi.bankaSubeAdi = VeriErisim.DegerAl(r, 48, "");
                    bilgi.unvanAdi = VeriErisim.DegerAl(r, 49, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 50, "");
                    bilgi.soyad = VeriErisim.DegerAl(r, 51, "");
                    bilgi.sicilNo = VeriErisim.DegerAl(r, 52, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli personel asgari geçim bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli personel asgari geçim bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleAsgariGecimBordro(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSozlesmeliMaasBordro to = new TABSozlesmeliMaasBordro(ve);
            IDataReader r = to.ListeleAsgariGecimBordro(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.HRC.PSLSozlesmeliMaasBordro bilgi = new TNS.HRC.PSLSozlesmeliMaasBordro();

                    bilgi.tCKimlikNo = VeriErisim.DegerAl(r, 0, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");
                    bilgi.maasYil = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.maasAy = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.asgGecimIndKatsayi = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                    bilgi.asgGecimIndirimi = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                    bilgi.brutAsgariUcret = (decimal)VeriErisim.DegerAl(r, 6, 0.0);

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoKaydet(Kullanici kullanici, PSLSozlesmeliMaasBordro f, string belgeNo)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!HRCYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABSozlesmeliMaasBordro tab = new TABSozlesmeliMaasBordro(ve);

            sonuc = tab.MIFNoKaydet(f, belgeNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoListele(Kullanici kullanici, PSLSozlesmeliMaasBordro f)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = false;

            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!HRCYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABSozlesmeliMaasBordro tab = new TABSozlesmeliMaasBordro(ve);

            IDataReader r = null;

            try
            {
                r = tab.MIFNoListele(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return sonuc;
            }

            if (r.Read())
            {
                sonuc.anahtar = VeriErisim.DegerAl(r, 4, "");
                sonuc.islemSonuc = true;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }



        private string Kontrol(PSLSozlesmeliMaasBordro f)
        {
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }

            if (f.muhasebe == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;
            }
            if (f.birim == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005;
            }
            if (f.surecNo == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067;
            }
            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe);
                }

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim);
                }
            }

            return hata;
        }

        private ObjectArray PersonelGecenAyMaasListesiAl(Kullanici kullanan, int muhasebe, string birim, int maasYil, int maasAy)
        {
            PSLSozlesmeliMaasBordro sozlesmeliKriter = new PSLSozlesmeliMaasBordro();
            sozlesmeliKriter.birim = birim;
            sozlesmeliKriter.muhasebe = muhasebe;
            if (maasAy == 1)
            {
                sozlesmeliKriter.maasAy = 12;
                sozlesmeliKriter.maasYil = maasYil - 1;
            }
            else
            {
                sozlesmeliKriter.maasAy = maasAy - 1;
                sozlesmeliKriter.maasYil = maasYil;
            }

            return Listele(kullanan, sozlesmeliKriter);
        }

        private ObjectArray PersonelMaasTanimListesiAl(Kullanici kullanan, int muhasebe, string birim, int maasYil, int maasAy)
        {
            SozlesmeliBordroTanim personelMaasTanimKriteri = new SozlesmeliBordroTanim();
            personelMaasTanimKriteri.muhasebe = muhasebe;
            personelMaasTanimKriteri.birim = birim;
            personelMaasTanimKriteri.yil = maasYil;
            personelMaasTanimKriteri.ay = maasAy;

            return (new HRCSozlesmeliMaasBordroTanim()).Listele(kullanan, personelMaasTanimKriteri);
        }

        private ObjectArray GelirVergisiListesiAl(Kullanici kullanan, int maasYil)
        {
            PSLGelirVergisi kriterGelirVergisi = new PSLGelirVergisi();
            kriterGelirVergisi.yil = maasYil;
            return (new HRCGelirVergisi()).Listele(kullanan, kriterGelirVergisi);
        }

        private static PSLMaasTemelBilgisi MaasTemelBilgisiAl(Kullanici kullanan, int maasYil, int maasAy)
        {
            TNS.HRC.PSLMaasTemelBilgisi kriter = new PSLMaasTemelBilgisi();
            kriter.ay = maasAy;
            kriter.yil = maasYil;

            ObjectArray maasTemelBilgiList = (new HRCMaasTemelBilgisi()).Listele(kullanan, kriter);
            PSLMaasTemelBilgisi maasTemelBilgisi = null;

            if (maasTemelBilgiList.ObjeSayisi > 0)
                maasTemelBilgisi = (PSLMaasTemelBilgisi)maasTemelBilgiList[0];

            return maasTemelBilgisi;
        }

        private static PSLSosyalYardimTemelBilgisi SosyalYardimTemelBilgisiAl(Kullanici kullanan, int maasYil, int maasAy)
        {
            TNS.HRC.PSLSosyalYardimTemelBilgisi kriter = new PSLSosyalYardimTemelBilgisi();
            kriter.ay = maasAy;
            kriter.yil = maasYil;

            ObjectArray sosyalYardimBilgiList = (new HRCSosYardTemelBilgisi()).Listele(kullanan, kriter);
            PSLSosyalYardimTemelBilgisi sosyalYardimBilgisi = null;

            if (sosyalYardimBilgiList.ObjeSayisi > 0)
                sosyalYardimBilgisi = (PSLSosyalYardimTemelBilgisi)sosyalYardimBilgiList[0];

            return sosyalYardimBilgisi;
        }

        private bool KontrolPersonelMaasTanimBilgisi(SozlesmeliBordroTanim perTanim)
        {
            if (perTanim.sozlesmeUcret == 0)
                return false;

            return true;
        }

        private SozlesmeliBordroTanim PersonelMaasTanimBilgisiAl(string tcKimlik, ObjectArray maasTanimList)
        {
            SozlesmeliBordroTanim donusDegeri = new SozlesmeliBordroTanim();
            foreach (SozlesmeliBordroTanim varPer in maasTanimList.objeler)
            {
                if (varPer.tcKimlikNo.Trim() == tcKimlik)
                {
                    donusDegeri = varPer;
                }
            }
            return donusDegeri;
        }

        private PSLSozlesmeliMaasBordro PersonelEskiMaasBilgisiAl(string tcKimlik)
        {
            foreach (PSLSozlesmeliMaasBordro var in perGecenAyMaasList.objeler)
            {
                if (var.tCKimlikNo.Trim() == tcKimlik)
                    return var;
            }
            return new PSLSozlesmeliMaasBordro();

        }

        private PSLGelirVergisi gelirVergisiDilimi(decimal persKumilatifVergi)
        {

            foreach (PSLGelirVergisi vergiDilimi in gelirVergisiList.objeler)
            {

                if (persKumilatifVergi < vergiDilimi.miktar)
                {
                    return vergiDilimi;
                }

            }
            return new PSLGelirVergisi();
        }
    }

}
