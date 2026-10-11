using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İşçi maaş bordro bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIsciMaasBordro
    {
        private ObjectArray gelirVergisiList;
        private TNSCollection perMaasTanimList;
        private ObjectArray perGecenAyMaasList;
        private const int tediyeGunCarpani = 13;
        private const int aylikGunSabiti = 30;

        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="maasHesaplalist">Kaydedilecek bilgileri IsciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
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

            string hataStr = "";
            int satir = 0;

            if (maasHesaplalist.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (IsciMaasBordro ida in maasHesaplalist.objeler)
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
                    if (ida.bordroTip == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00217, satir) + "<br>";

                }
            }
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }



            IsciMaasBordro bordro = (IsciMaasBordro)maasHesaplalist.objeler[0];


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


            //**************** ******* ** *** MAAŞ HESAPLAMA ************ ** * * * * * ** **********

            TNS.HRC.PSLMaasTemelBilgisi kriter = new TNS.HRC.PSLMaasTemelBilgisi();
            kriter.ay = bordro.maasAy;
            kriter.yil = bordro.maasYil;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            //1. ADIM Sozlesmeli maasları için genel tanımlamaları al
            ObjectArray maasTemelBilgiList = (new HRCMaasTemelBilgisi()).Listele(kullanan, kriter);

            if (maasTemelBilgiList.ObjeSayisi == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00218 + "<br>";
            PSLMaasTemelBilgisi maasTemelBilgisi = null;

            if (hataStr == "")
                maasTemelBilgisi = (PSLMaasTemelBilgisi)maasTemelBilgiList[0];

            PSLGelirVergisi kriterGelirVergisi = new PSLGelirVergisi();
            kriterGelirVergisi.yil = bordro.yil;




            //2. ADIM Gelir Vergisi Bilgisi
            gelirVergisiList = (new HRCGelirVergisi()).Listele(kullanan, kriterGelirVergisi);

            if (gelirVergisiList.ObjeSayisi == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00219 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //3. ADIM Maas Bordrosu Yapılacak Personel Listesinin tanım bilgilerini al
            //--H--
            IsciBordroTanim personelMaasTanimKriteri = new IsciBordroTanim();
            personelMaasTanimKriteri.birim = bordro.birim;
            personelMaasTanimKriteri.muhasebe = bordro.muhasebe;
            personelMaasTanimKriteri.ay = bordro.maasAy;
            personelMaasTanimKriteri.yil = bordro.maasYil;

            perMaasTanimList = (new HRCIsciMaasBordroTanim()).Listele(kullanan, personelMaasTanimKriteri).objeler;

            if (perMaasTanimList.Count == 0)
            {
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00220 + "<br>";
            }
            //Hata var ise geri dön uyarı ver
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //4. ADIM Bu ay maaşı hesaplanacakların kumilatif vergi matrahları için gecen ay maas listesini alalım
            //--H--
            IsciMaasBordro sozlesmeliKriter = new IsciMaasBordro();
            sozlesmeliKriter.birim = bordro.birim;
            sozlesmeliKriter.muhasebe = bordro.muhasebe;
            sozlesmeliKriter.yil = bordro.yil;
            sozlesmeliKriter.bordroTip = (int)ENUMIsciMaasBordroTip.MAAS;
            /*İşçi maaşlarında fark bordrosu hesaplanırken daha önceden maaşın hesaplandığını düşünüyoruz
             bu sebepten bu ay için her personelin kumilatifi daha önceden hesaplanması gerekli*/
            if (bordro.farkmi == (int)ENUMEvetHayir.HAYIR)
            {
                sozlesmeliKriter.farkmi = (int)ENUMEvetHayir.HAYIR;
                if (bordro.maasAy == 1)
                {
                    sozlesmeliKriter.maasAy = 12;
                    sozlesmeliKriter.maasYil = bordro.yil - 1;
                }
                else
                {
                    sozlesmeliKriter.maasAy = bordro.maasAy - 1;
                    sozlesmeliKriter.maasYil = bordro.yil;
                }
            }
            else
            {
                sozlesmeliKriter.farkmi = (int)ENUMEvetHayir.EVET;
                sozlesmeliKriter.maasAy = bordro.maasAy;
                sozlesmeliKriter.maasYil = bordro.yil;

            }
            perGecenAyMaasList = Listele(kullanan, sozlesmeliKriter);


            //5. ADIM Listeden Gelen Personellerin Maas Hesabını yapalım
            IsciBordroTanim perMaasTanim = null;

            foreach (IsciMaasBordro perMaas in maasHesaplalist.objeler)
            {
                perMaasTanim = PersonelMaasTanimBilgisiAl(perMaas.tCKimlikNo.Trim());
                if (KontrolPersonelMaasTanimBilgisi(perMaasTanim))
                {
                    if (bordro.farkmi == (int)ENUMEvetHayir.HAYIR)
                    {
                        switch (perMaas.bordroTip)
                        {
                            case (int)ENUMIsciMaasBordroTip.MAAS:
                                IsciMaasiHesapla(perMaas, maasTemelBilgisi, perMaasTanim);
                                break;
                            case (int)ENUMIsciMaasBordroTip.SENDIKALIKRAMIYE:
                                IsciSendikalIkramiyeHesapla(perMaas, maasTemelBilgisi, perMaasTanim);
                                break;
                            case (int)ENUMIsciMaasBordroTip.ILAVETEDIYE:
                                IsciIlaveTediyeHesapla(perMaas, maasTemelBilgisi, perMaasTanim);
                                break;
                        }
                    }
                    else
                    {
                        //Fark Bordrolarını Hazırla
                        foreach (IsciMaasBordro perFarkMaas in maasHesaplalist.objeler)
                        {
                            IsciMaasiFarkiHesapla(perFarkMaas, maasTemelBilgisi, perMaasTanim);

                        }


                    }

                }
            }

            TABIsciMaasBordro to = new TABIsciMaasBordro(ve);
            ve.BeginTransaction();

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
        
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro bilgisini veritabanından okur 
        /// ve IsciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro bilgisi listesini IsciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IsciMaasBordro kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIsciMaasBordro to = new TABIsciMaasBordro(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IsciMaasBordro bilgi = new IsciMaasBordro();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tCKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    bilgi.bordroTip = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.farkmi = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.maasYil = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.maasAy = VeriErisim.DegerAl(r, 8, 0);

                    bilgi.siraNo = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.gun = VeriErisim.DegerAl(r, 10, 0);
                    bilgi.yevmiye = (decimal)VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.ucret = (decimal)VeriErisim.DegerAl(r, 12, 0.0);
                    bilgi.sakatlikIndirimi = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    bilgi.ozelSigorta = (decimal)VeriErisim.DegerAl(r, 14, 0.0);
                    bilgi.prim = (decimal)VeriErisim.DegerAl(r, 15, 0.0);
                    bilgi.tasitGunu = VeriErisim.DegerAl(r, 16, 0);
                    bilgi.tasitYardimi = (decimal)VeriErisim.DegerAl(r, 17, 0.0);
                    bilgi.tasitYardimKatsayi = (decimal)VeriErisim.DegerAl(r, 18, 0.0);
                    bilgi.yemekGunu = VeriErisim.DegerAl(r, 19, 0);
                    bilgi.yemekYardimi = (decimal)VeriErisim.DegerAl(r, 20, 0.0);
                    bilgi.yemekYardimKatsayi = (decimal)VeriErisim.DegerAl(r, 21, 0.0);
                    bilgi.hakSskIsverenPayi = (decimal)VeriErisim.DegerAl(r, 22, 0.0);
                    bilgi.hakSskPrimineEsasKazanc = (decimal)VeriErisim.DegerAl(r, 23, 0.0);
                    bilgi.hakIssizlikSigIsverenPayi = (decimal)VeriErisim.DegerAl(r, 24, 0.0);
                    bilgi.hakToplamKazanc = (decimal)VeriErisim.DegerAl(r, 25, 0.0);
                    bilgi.kesDamgaVergisiMatrah = (decimal)VeriErisim.DegerAl(r, 26, 0.0);
                    bilgi.kesGelirVergisiMatrah = (decimal)VeriErisim.DegerAl(r, 27, 0.0);
                    bilgi.kesGelirVergisi = (decimal)VeriErisim.DegerAl(r, 28, 0.0);
                    bilgi.gelirVergisiOrani = (decimal)VeriErisim.DegerAl(r, 29, 0.0);
                    bilgi.kesDamgaVergisi = (decimal)VeriErisim.DegerAl(r, 30, 0.0);
                    bilgi.damgaVergisiOrani = (decimal)VeriErisim.DegerAl(r, 31, 0.0);
                    bilgi.kesSskPrimIsverenPayi = (decimal)VeriErisim.DegerAl(r, 32, 0.0);
                    bilgi.sskIsverenOran = (decimal)VeriErisim.DegerAl(r, 33, 0.0);
                    bilgi.kesSskPrimIsciPayi = (decimal)VeriErisim.DegerAl(r, 34, 0.0);
                    bilgi.sskIsciOran = (decimal)VeriErisim.DegerAl(r, 35, 0.0);
                    bilgi.kesIssizSigIsverenPayi = (decimal)VeriErisim.DegerAl(r, 36, 0.0);
                    bilgi.issizlikSigIsverenOran = (decimal)VeriErisim.DegerAl(r, 37, 0.0);
                    bilgi.kesIssizSigIsciPayi = (decimal)VeriErisim.DegerAl(r, 38, 0.0);
                    bilgi.issizSigIsciOran = (decimal)VeriErisim.DegerAl(r, 39, 0.0);
                    bilgi.sendikaAidat = (decimal)VeriErisim.DegerAl(r, 40, 0.0);
                    bilgi.brutAsgariUcret = (decimal)VeriErisim.DegerAl(r, 41, 0.0);
                    bilgi.asgariGecimIndirim = (decimal)VeriErisim.DegerAl(r, 42, 0.0);
                    bilgi.asgGecimIndKatsayi = (decimal)VeriErisim.DegerAl(r, 43, 0.0);
                    bilgi.toplamKesinti = (decimal)VeriErisim.DegerAl(r, 44, 0.0);
                    bilgi.netOdenen = (decimal)VeriErisim.DegerAl(r, 45, 0.0);
                    bilgi.kumulatifVergiMatrahi = (decimal)VeriErisim.DegerAl(r, 46, 0.0);
                    bilgi.bankaKodu = VeriErisim.DegerAl(r, 47, "");
                    bilgi.bankaSubeKodu = VeriErisim.DegerAl(r, 48, "");
                    bilgi.bankaHesapNo = VeriErisim.DegerAl(r, 49, "");
                    bilgi.unvanKodu = VeriErisim.DegerAl(r, 50, 0);
                    bilgi.medeniDurumu = VeriErisim.DegerAl(r, 51, 0);
                    bilgi.digerKesinti = (decimal)VeriErisim.DegerAl(r, 52, 0.0);
                    bilgi.sosyalYardim = (decimal)VeriErisim.DegerAl(r, 53, 0.0);
                    bilgi.bankaAdi = VeriErisim.DegerAl(r, 54, "");
                    bilgi.bankaSubeAdi = VeriErisim.DegerAl(r, 55, "");
                    bilgi.unvanAdi = VeriErisim.DegerAl(r, 56, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 57, "");
                    bilgi.soyad = VeriErisim.DegerAl(r, 58, "");
                    bilgi.sicilNo = VeriErisim.DegerAl(r, 59, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, IsciMaasBordro kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIsciMaasBordro to = new TABIsciMaasBordro(ve);


            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            int yil = kriter.yil;
            string surecNo = kriter.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            try
            {
                sonuc = to.Sil(kriter);
            }
            catch (Exception e)
            {
                sonuc.hataStr = e.Message;

            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        private bool KontrolPersonelMaasTanimBilgisi(IsciBordroTanim perTanim)
        {

            if (perTanim.asgariGecimIndirim == 0 || perTanim.yevmiye == 0)
                return false;

            return true;

        }

        private void IsciMaasiHesapla(IsciMaasBordro perMaas, PSLMaasTemelBilgisi maasTemelBilgisi, IsciBordroTanim perMaasTanim)
        {
            IsciMaasBordro perEskiMaas = PersonelEskiMaasBilgisiAl(perMaas.tCKimlikNo.Trim());

            perMaas.ucret = perMaasTanim.yevmiye * perMaas.gun;
            perMaas.ucret = Math.Round(perMaas.ucret, 2);
            //yuvarla

            perMaas.yevmiye = perMaasTanim.yevmiye;

            perMaas.prim = perMaasTanim.prim;
            perMaas.sosyalYardim = perMaasTanim.sosyalYardim;

            perMaas.tasitYardimi = (perMaas.tasitGunu / 30) * perMaasTanim.tasitYardimi;
            perMaas.tasitYardimi = Math.Round(perMaas.tasitYardimi, 2);
            perMaas.tasitYardimKatsayi = perMaasTanim.tasitYardimi;

            perMaas.yemekYardimi = perMaas.yemekGunu * perMaasTanim.yemekYardimi;
            perMaas.yemekYardimi = Math.Round(perMaas.yemekYardimi, 2);
            perMaas.yemekYardimKatsayi = perMaasTanim.yemekYardimi;

            //SSK Primine Esas Kazanç

            perMaas.hakSskPrimineEsasKazanc = perMaas.ucret + perMaas.prim + perMaas.sosyalYardim + perMaas.tasitYardimi + perMaas.yemekYardimi;

            perMaas.hakSskPrimineEsasKazanc = Math.Round(perMaas.hakSskPrimineEsasKazanc, 2);


            perMaas.hakSskIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.hakSskIsverenPayi = Math.Round(perMaas.hakSskIsverenPayi, 2);

            perMaas.hakIssizlikSigIsverenPayi = perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran / 100;
            perMaas.hakIssizlikSigIsverenPayi = Math.Round(perMaas.hakIssizlikSigIsverenPayi, 2);

            //Toplam Kazanç Kazanç

            perMaas.hakToplamKazanc = perMaas.hakSskPrimineEsasKazanc + perMaas.hakSskIsverenPayi + perMaas.hakIssizlikSigIsverenPayi;

            perMaas.hakToplamKazanc = Math.Round(perMaas.hakToplamKazanc, 2);

            perMaas.kesDamgaVergisiMatrah = perMaas.hakSskPrimineEsasKazanc;

            perMaas.kesDamgaVergisi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.damgaVergisiOran) / 100;
            perMaas.kesDamgaVergisi = Math.Round(perMaas.kesDamgaVergisi, 2);
            perMaas.damgaVergisiOrani = maasTemelBilgisi.damgaVergisiOran;


            perMaas.kesSskPrimIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.kesSskPrimIsverenPayi = Math.Round(perMaas.kesSskPrimIsverenPayi, 2);
            perMaas.sskIsverenOran = maasTemelBilgisi.sskIsverenPayiOran;

            perMaas.kesSskPrimIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsciPayiOran) / 100;
            perMaas.kesSskPrimIsciPayi = Math.Round(perMaas.kesSskPrimIsciPayi, 2);
            perMaas.sskIsciOran = maasTemelBilgisi.sskIsciPayiOran;

            perMaas.kesIssizSigIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran) / 100;
            perMaas.kesIssizSigIsverenPayi = Math.Round(perMaas.kesIssizSigIsverenPayi, 2);
            perMaas.issizlikSigIsverenOran = maasTemelBilgisi.sskIssizSigIsVerenPayiOran;

            perMaas.kesIssizSigIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsciPayiOran) / 100;
            perMaas.kesIssizSigIsciPayi = Math.Round(perMaas.kesIssizSigIsciPayi, 2);
            perMaas.issizSigIsciOran = maasTemelBilgisi.sskIssizSigIsciPayiOran;

            perMaas.asgariGecimIndirim = (maasTemelBilgisi.brutAsgariUcret * (perMaasTanim.asgariGecimIndirim / 100) * ((PSLGelirVergisi)gelirVergisiList.objeler[0]).yuzde) / 100;
            perMaas.asgariGecimIndirim = Math.Round(perMaas.asgariGecimIndirim, 2);
            perMaas.asgGecimIndKatsayi = perMaasTanim.asgariGecimIndirim;
            perMaas.brutAsgariUcret = maasTemelBilgisi.brutAsgariUcret;

            perMaas.sendikaAidat = perMaasTanim.sendikaAidati;
            perMaas.kesGelirVergisiMatrah = perMaas.hakSskPrimineEsasKazanc - (perMaas.kesIssizSigIsciPayi + perMaas.kesSskPrimIsciPayi + perMaas.sendikaAidat);
            perMaas.kesGelirVergisiMatrah = Math.Round(perMaas.kesGelirVergisiMatrah, 2);

            //kumilatif vergi matrahı hesabı
            perMaas.kumulatifVergiMatrahi = perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic + perMaas.kesGelirVergisiMatrah;
            perMaas.kumulatifVergiMatrahi = Math.Round(perMaas.kumulatifVergiMatrahi, 2);

            //Bu eklenenle birlikte gelir vergisinde girdiği yüzdelik dilim
            perMaas.gelirVergisiOrani = gelirVergisiDilimi(perMaas.kumulatifVergiMatrahi).yuzde;

            perMaas.kesGelirVergisi = asilGelirVergisiniHesapla(perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic, perMaas.kesGelirVergisiMatrah);
            perMaas.kesGelirVergisi = Math.Round(perMaas.kesGelirVergisi, 2);

            perMaas.sendikaAidat = perMaasTanim.sendikaAidati;
            perMaas.toplamKesinti = perMaas.kesGelirVergisi + perMaas.kesDamgaVergisi + perMaas.kesIssizSigIsciPayi + perMaas.kesIssizSigIsverenPayi + perMaas.kesSskPrimIsciPayi + perMaas.kesSskPrimIsverenPayi + perMaas.sendikaAidat + perMaas.digerKesinti;
            perMaas.toplamKesinti = Math.Round(perMaas.toplamKesinti, 2);

            perMaas.netOdenen = perMaas.hakToplamKazanc - perMaas.toplamKesinti + perMaas.asgariGecimIndirim;
            perMaas.netOdenen = Math.Round(perMaas.netOdenen, 2);


        }
      
        private void IsciSendikalIkramiyeHesapla(IsciMaasBordro perMaas, PSLMaasTemelBilgisi maasTemelBilgisi, IsciBordroTanim perMaasTanim)
        {
            IsciMaasBordro perEskiMaas = PersonelEskiMaasBilgisiAl(perMaas.tCKimlikNo.Trim());

            perMaas.ucret = perMaasTanim.yevmiye * aylikGunSabiti;
            perMaas.ucret = Math.Round(perMaas.ucret, 2);
            //yuvarla

            perMaas.yevmiye = perMaasTanim.yevmiye;

            perMaas.tasitYardimi = (perMaas.tasitGunu / 30) * perMaasTanim.tasitYardimi;
            perMaas.tasitYardimi = Math.Round(perMaas.tasitYardimi, 2);
            perMaas.tasitYardimKatsayi = perMaasTanim.tasitYardimi;

            perMaas.yemekYardimi = perMaas.yemekGunu * perMaasTanim.yemekYardimi;
            perMaas.yemekYardimi = Math.Round(perMaas.yemekYardimi, 2);
            perMaas.yemekYardimKatsayi = perMaasTanim.yemekYardimi;
            //SSK Primine Esas Kazanç
            //Vergi Dilimi İçin
            decimal aylikUcreti = perMaasTanim.yevmiye * perMaas.gun;
            aylikUcreti = Math.Round(aylikUcreti, 2);
            decimal sskPrimineEsasKazanc = aylikUcreti + perMaasTanim.prim + perMaasTanim.sosyalYardim + perMaas.tasitYardimi + perMaas.yemekYardimi;


            perMaas.hakSskPrimineEsasKazanc = perMaas.ucret;
            perMaas.hakSskPrimineEsasKazanc = Math.Round(perMaas.hakSskPrimineEsasKazanc, 2);

            perMaas.hakSskIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.hakSskIsverenPayi = Math.Round(perMaas.hakSskIsverenPayi, 2);

            perMaas.hakIssizlikSigIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran) / 100;
            perMaas.hakIssizlikSigIsverenPayi = Math.Round(perMaas.hakIssizlikSigIsverenPayi, 2);


            //Toplam Kazanç Kazanç

            perMaas.hakToplamKazanc = perMaas.ucret + perMaas.hakIssizlikSigIsverenPayi + perMaas.hakSskIsverenPayi;
            perMaas.hakToplamKazanc = Math.Round(perMaas.hakToplamKazanc, 2);

            perMaas.kesDamgaVergisiMatrah = perMaas.hakSskPrimineEsasKazanc;

            perMaas.kesDamgaVergisi = (perMaas.kesDamgaVergisiMatrah * maasTemelBilgisi.damgaVergisiOran) / 100;
            perMaas.kesDamgaVergisi = Math.Round(perMaas.kesDamgaVergisi, 2);
            perMaas.damgaVergisiOrani = maasTemelBilgisi.damgaVergisiOran;


            perMaas.kesSskPrimIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.kesSskPrimIsverenPayi = Math.Round(perMaas.kesSskPrimIsverenPayi, 2);
            perMaas.sskIsverenOran = maasTemelBilgisi.sskIsverenPayiOran;

            perMaas.kesSskPrimIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsciPayiOran) / 100;
            perMaas.kesSskPrimIsciPayi = Math.Round(perMaas.kesSskPrimIsciPayi, 2);
            perMaas.sskIsciOran = maasTemelBilgisi.sskIsciPayiOran;

            decimal sskPrimIsciPayi = (sskPrimineEsasKazanc * maasTemelBilgisi.sskIsciPayiOran) / 100;
            sskPrimIsciPayi = Math.Round(sskPrimIsciPayi, 2);

            perMaas.kesIssizSigIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran) / 100;
            perMaas.kesIssizSigIsverenPayi = Math.Round(perMaas.kesIssizSigIsverenPayi, 2);
            perMaas.issizlikSigIsverenOran = maasTemelBilgisi.sskIssizSigIsVerenPayiOran;

            perMaas.kesIssizSigIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsciPayiOran) / 100;
            perMaas.kesIssizSigIsciPayi = Math.Round(perMaas.kesIssizSigIsciPayi, 2);
            perMaas.issizSigIsciOran = maasTemelBilgisi.sskIssizSigIsciPayiOran;

            decimal issizSigIsciPayi = (sskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsciPayiOran) / 100;
            issizSigIsciPayi = Math.Round(issizSigIsciPayi, 2);

            perMaas.kesGelirVergisiMatrah = perMaas.hakSskPrimineEsasKazanc - (perMaas.kesIssizSigIsciPayi + perMaas.kesSskPrimIsciPayi);
            perMaas.kesGelirVergisiMatrah = Math.Round(perMaas.kesGelirVergisiMatrah, 2);

            decimal gercekKesVergiMatrahi = sskPrimineEsasKazanc - (issizSigIsciPayi + sskPrimIsciPayi + perMaasTanim.sendikaAidati);
            //kumilatif vergi matrahı hesabı
            perMaas.kumulatifVergiMatrahi = perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic + perMaas.kesGelirVergisiMatrah;
            perMaas.kumulatifVergiMatrahi = Math.Round(perMaas.kumulatifVergiMatrahi, 2);

            //Bu eklenenle birlikte gelir vergisinde girdiği yüzdelik dilim
            perMaas.gelirVergisiOrani = gelirVergisiDilimi(perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic + gercekKesVergiMatrahi).yuzde;

            perMaas.kesGelirVergisi = (perMaas.kesGelirVergisiMatrah * perMaas.gelirVergisiOrani) / 100;
            perMaas.kesGelirVergisi = Math.Round(perMaas.kesGelirVergisi, 2);


            perMaas.toplamKesinti = perMaas.kesGelirVergisi + perMaas.kesDamgaVergisi + perMaas.kesIssizSigIsciPayi + perMaas.kesIssizSigIsverenPayi + perMaas.kesSskPrimIsciPayi + perMaas.kesSskPrimIsverenPayi;
            perMaas.toplamKesinti = Math.Round(perMaas.toplamKesinti, 2);

            perMaas.netOdenen = perMaas.hakToplamKazanc - perMaas.toplamKesinti + perMaas.asgariGecimIndirim;
            perMaas.netOdenen = Math.Round(perMaas.netOdenen, 2);


        }
       
        private void IsciIlaveTediyeHesapla(IsciMaasBordro perMaas, PSLMaasTemelBilgisi maasTemelBilgisi, IsciBordroTanim perMaasTanim)
        {
            IsciMaasBordro perEskiMaas = PersonelEskiMaasBilgisiAl(perMaas.tCKimlikNo.Trim());

            perMaas.ucret = perMaasTanim.yevmiye * tediyeGunCarpani;//su an 13 ile carpılıyor
            perMaas.ucret = Math.Round(perMaas.ucret, 2);
            //yuvarla
            perMaas.yevmiye = perMaasTanim.yevmiye;

            perMaas.tasitYardimi = (perMaas.tasitGunu / 30) * perMaasTanim.tasitYardimi;
            perMaas.tasitYardimi = Math.Round(perMaas.tasitYardimi, 2);
            perMaas.tasitYardimKatsayi = perMaasTanim.tasitYardimi;

            perMaas.yemekYardimi = perMaas.yemekGunu * perMaasTanim.yemekYardimi;
            perMaas.yemekYardimi = Math.Round(perMaas.yemekYardimi, 2);
            perMaas.yemekYardimKatsayi = perMaasTanim.yemekYardimi;
            //SSK Primine Esas Kazanç
            //Vergi Dilimi İçin
            decimal aylikUcreti = perMaasTanim.yevmiye * perMaas.gun;
            aylikUcreti = Math.Round(aylikUcreti, 2);
            decimal sskPrimineEsasKazanc = aylikUcreti + perMaasTanim.prim + perMaasTanim.sosyalYardim + perMaas.tasitYardimi + perMaas.yemekYardimi;

            decimal sskPrimIsciPayi = (sskPrimineEsasKazanc * maasTemelBilgisi.sskIsciPayiOran) / 100;
            sskPrimIsciPayi = Math.Round(sskPrimIsciPayi, 2);

            decimal issizSigIsciPayi = (sskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsciPayiOran) / 100;
            issizSigIsciPayi = Math.Round(issizSigIsciPayi, 2);

            decimal gercekKesVergiMatrahi = sskPrimineEsasKazanc - (issizSigIsciPayi + sskPrimIsciPayi + perMaasTanim.sendikaAidati);


            //Toplam Kazanç Kazanç
            perMaas.hakToplamKazanc = perMaas.ucret;
            perMaas.kesDamgaVergisiMatrah = perMaas.ucret;

            perMaas.kesDamgaVergisi = (perMaas.kesDamgaVergisiMatrah * maasTemelBilgisi.damgaVergisiOran) / 100;
            perMaas.kesDamgaVergisi = Math.Round(perMaas.kesDamgaVergisi, 2);
            perMaas.damgaVergisiOrani = maasTemelBilgisi.damgaVergisiOran;

            perMaas.kesGelirVergisiMatrah = perMaas.ucret;
            perMaas.kesGelirVergisiMatrah = Math.Round(perMaas.kesGelirVergisiMatrah, 2);

            //kumilatif vergi matrahı hesabı
            perMaas.kumulatifVergiMatrahi = perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic + perMaas.kesGelirVergisiMatrah;
            perMaas.kumulatifVergiMatrahi = Math.Round(perMaas.kumulatifVergiMatrahi, 2);

            //Bu eklenenle birlikte gelir vergisinde girdiği yüzdelik dilim
            perMaas.gelirVergisiOrani = gelirVergisiDilimi(perEskiMaas.kumulatifVergiMatrahi + perMaasTanim.kumulatifVergiMatrahiBaslangic + gercekKesVergiMatrahi).yuzde;

            perMaas.kesGelirVergisi = (perMaas.kesGelirVergisiMatrah * perMaas.gelirVergisiOrani) / 100;
            perMaas.kesGelirVergisi = Math.Round(perMaas.kesGelirVergisi, 2);


            perMaas.toplamKesinti = perMaas.kesGelirVergisi + perMaas.kesDamgaVergisi + perMaas.kesIssizSigIsciPayi;
            perMaas.toplamKesinti = Math.Round(perMaas.toplamKesinti, 2);

            perMaas.netOdenen = perMaas.hakToplamKazanc - perMaas.toplamKesinti + perMaas.asgariGecimIndirim;
            perMaas.netOdenen = Math.Round(perMaas.netOdenen, 2);

        }
        
        private void IsciMaasiFarkiHesapla(IsciMaasBordro perMaas, PSLMaasTemelBilgisi maasTemelBilgisi, IsciBordroTanim perMaasTanim)
        {
            IsciMaasBordro perEskiMaas = PersonelEskiMaasBilgisiAl(perMaas.tCKimlikNo.Trim());

            perMaas.ucret = perMaas.yevmiye * perMaas.gun;
            perMaas.ucret = Math.Round(perMaas.ucret, 2);
            //yuvarla

            perMaas.tasitYardimi = (perMaas.tasitGunu / 30) * perMaas.tasitYardimKatsayi;
            perMaas.tasitYardimi = Math.Round(perMaas.tasitYardimi, 2);

            perMaas.yemekYardimi = perMaas.yemekGunu * perMaas.yemekYardimKatsayi;
            perMaas.yemekYardimi = Math.Round(perMaas.yemekYardimi, 2);

            //SSK Primine Esas Kazanç

            perMaas.hakSskPrimineEsasKazanc = perMaas.ucret + perMaas.prim + perMaas.sosyalYardim + perMaas.tasitYardimi + perMaas.yemekYardimi;

            perMaas.hakSskPrimineEsasKazanc = Math.Round(perMaas.hakSskPrimineEsasKazanc, 2);


            perMaas.hakSskIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.hakSskIsverenPayi = Math.Round(perMaas.hakSskIsverenPayi, 2);

            perMaas.hakIssizlikSigIsverenPayi = perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran / 100;
            perMaas.hakIssizlikSigIsverenPayi = Math.Round(perMaas.hakIssizlikSigIsverenPayi, 2);

            //Toplam Kazanç Kazanç

            perMaas.hakToplamKazanc = perMaas.hakSskPrimineEsasKazanc + perMaas.hakSskIsverenPayi + perMaas.hakIssizlikSigIsverenPayi;

            perMaas.hakToplamKazanc = Math.Round(perMaas.hakToplamKazanc, 2);

            perMaas.kesDamgaVergisiMatrah = perMaas.hakSskPrimineEsasKazanc;

            perMaas.kesDamgaVergisi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.damgaVergisiOran) / 100;
            perMaas.kesDamgaVergisi = Math.Round(perMaas.kesDamgaVergisi, 2);
            perMaas.damgaVergisiOrani = maasTemelBilgisi.damgaVergisiOran;


            perMaas.kesSskPrimIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsverenPayiOran) / 100;
            perMaas.kesSskPrimIsverenPayi = Math.Round(perMaas.kesSskPrimIsverenPayi, 2);
            perMaas.sskIsverenOran = maasTemelBilgisi.sskIsverenPayiOran;

            perMaas.kesSskPrimIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIsciPayiOran) / 100;
            perMaas.kesSskPrimIsciPayi = Math.Round(perMaas.kesSskPrimIsciPayi, 2);
            perMaas.sskIsciOran = maasTemelBilgisi.sskIsciPayiOran;

            perMaas.kesIssizSigIsverenPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsVerenPayiOran) / 100;
            perMaas.kesIssizSigIsverenPayi = Math.Round(perMaas.kesIssizSigIsverenPayi, 2);
            perMaas.issizlikSigIsverenOran = maasTemelBilgisi.sskIssizSigIsVerenPayiOran;

            perMaas.kesIssizSigIsciPayi = (perMaas.hakSskPrimineEsasKazanc * maasTemelBilgisi.sskIssizSigIsciPayiOran) / 100;
            perMaas.kesIssizSigIsciPayi = Math.Round(perMaas.kesIssizSigIsciPayi, 2);
            perMaas.issizSigIsciOran = maasTemelBilgisi.sskIssizSigIsciPayiOran;

            perMaas.asgariGecimIndirim = (maasTemelBilgisi.brutAsgariUcret * (perMaasTanim.asgariGecimIndirim / 100) * ((PSLGelirVergisi)gelirVergisiList.objeler[0]).yuzde) / 100;
            perMaas.asgariGecimIndirim = Math.Round(perMaas.asgariGecimIndirim, 2);
            perMaas.asgGecimIndKatsayi = perMaasTanim.asgariGecimIndirim;
            perMaas.brutAsgariUcret = maasTemelBilgisi.brutAsgariUcret;

            perMaas.kesGelirVergisiMatrah = perMaas.hakSskPrimineEsasKazanc - (perMaas.kesIssizSigIsciPayi + perMaas.kesSskPrimIsciPayi + perMaas.sendikaAidat);
            perMaas.kesGelirVergisiMatrah = Math.Round(perMaas.kesGelirVergisiMatrah, 2);

            //kumilatif vergi matrahı hesabı
            perMaas.kumulatifVergiMatrahi = perEskiMaas.kumulatifVergiMatrahi + perMaas.kesGelirVergisiMatrah;
            perMaas.kumulatifVergiMatrahi = Math.Round(perMaas.kumulatifVergiMatrahi, 2);

            //Bu eklenenle birlikte gelir vergisinde girdiği yüzdelik dilim
            perMaas.gelirVergisiOrani = gelirVergisiDilimi(perMaas.kumulatifVergiMatrahi).yuzde;

            perMaas.kesGelirVergisi = asilGelirVergisiniHesapla(perEskiMaas.kumulatifVergiMatrahi, perMaas.kesGelirVergisiMatrah);
            perMaas.kesGelirVergisi = Math.Round(perMaas.kesGelirVergisi, 2);

            perMaas.toplamKesinti = perMaas.kesGelirVergisi + perMaas.kesDamgaVergisi + perMaas.kesIssizSigIsciPayi + perMaas.kesIssizSigIsverenPayi + perMaas.kesSskPrimIsciPayi + perMaas.kesSskPrimIsverenPayi + perMaas.sendikaAidat + perMaas.digerKesinti;
            perMaas.toplamKesinti = Math.Round(perMaas.toplamKesinti, 2);

            perMaas.netOdenen = perMaas.hakToplamKazanc - perMaas.toplamKesinti + perMaas.asgariGecimIndirim;
            perMaas.netOdenen = Math.Round(perMaas.netOdenen, 2);


        }
        
        private IsciBordroTanim PersonelMaasTanimBilgisiAl(string tcKimlik)
        {
            IsciBordroTanim donusDegeri = new IsciBordroTanim();
            foreach (IsciBordroTanim varPer in perMaasTanimList)
            {
                if (varPer.tcKimlikNo.Trim() == tcKimlik)
                {
                    donusDegeri = varPer;

                }
            }
            return donusDegeri;

        }
        
        private IsciMaasBordro PersonelEskiMaasBilgisiAl(string tcKimlik)
        {
            foreach (IsciMaasBordro var in perGecenAyMaasList.objeler)
            {
                if (var.tCKimlikNo.Trim() == tcKimlik)
                    return var;
            }
            return new IsciMaasBordro();

        }
        
        //Asagıdaki Fonksiyon Kümülatif Toplamın Şu Ana Kadar Ödenen Vergisini Çıkarır
        private decimal kumilatifVergisiniHesapla(decimal persKumilatifVergi)
        {

            decimal toplamVergi = 0;
            decimal oncekiMiktar = 0;
            decimal fark = 0;
            foreach (PSLGelirVergisi vergiDilimi in gelirVergisiList.objeler)
            {

                if ((vergiDilimi.miktar == 0) || (persKumilatifVergi < vergiDilimi.miktar))
                {
                    fark = persKumilatifVergi - oncekiMiktar;
                    toplamVergi += (fark * vergiDilimi.yuzde) / 100;
                    break;
                }
                else
                {
                    toplamVergi = vergiDilimi.vergisi;
                    oncekiMiktar = vergiDilimi.miktar;
                }

            }
            return toplamVergi;
        }
        
        //Bu ayki vergi matrahının gelir vergisini hesaplar
        private decimal asilGelirVergisiniHesapla(decimal gecenAyKumilatifVergiMatrahi, decimal buAyEklenenVergiMatrahi)
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