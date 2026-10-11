using System;
using System.Data;
using TNS.HRC;
using TNS.KYM;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Fatura bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCFaturaForm
    {
        /// <summary>
        /// Arguman olarak verilen, fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, FaturaForm kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan) && !kriter.firmaKayitYapiyor)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            if (kriter.detay.ObjeSayisi > 0)
            {
                if (kriter.firmaNo == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00084 + "<br>";
                if (kriter.faturaNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00085 + "<br>";
                if (kriter.faturaTarih.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00086 + "<br>";

                int satir = 0;

                foreach (FaturaDetay t in kriter.detay.objeler)
                {
                    satir++;
                    if (t.adi == "")
                        hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00087, satir) + "<br>";
                    if (t.miktar == 0)
                        hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00088, satir) + "<br>";

                }
            }
            else
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00089 + "<br>";

            if (kriter.faturaNo.Length > 255)
            {
                hata += "Fatura Numarası 255 karakterden uzun olamaz.<br>";
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = kriter.yil;
            string surecNo = kriter.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi && !kriter.firmaKayitYapiyor)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            Hesapla(kriter);
            sonuc = FaturaOnaydanBuyukMu(kriter, kullanan);

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            if (kriter.prFaturaNo == 0)
            {
                //Fatura kod alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "HRCFATURANO";

                kriter.prFaturaNo = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                //**********************************************************
            }

            TABFaturaForm to = new TABFaturaForm(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(kriter);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilDetay(kriter);

                if (sonuc.islemSonuc)
                    sonuc = to.KaydetDetay(kriter);
            }

            if (sonuc.islemSonuc)
            {
                if (!kriter.firmaKayitYapiyor)
                {
                    //Parca Adım devam ediyor olarak işaretle.
                    ///////////////////////////////////////////////////////////////////////////
                    new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                    ///////////////////////////////////////////////////////////////////////////
                }

                sonuc.anahtar = kriter.prFaturaNo.ToString();
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
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura bilgisini veritabanından okur 
        /// ve FaturaForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fatura listesini FaturaForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, FaturaForm kriter, bool malzemeListesineBagla)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FaturaForm f = new FaturaForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.prFaturaNo = VeriErisim.DegerAl(r, 4, 0);
                    f.firmaNo = VeriErisim.DegerAl(r, 5, 0);
                    f.faturaNo = VeriErisim.DegerAl(r, 6, "");
                    f.faturaTarih = VeriErisim.DegerAl(r, 7);
                    f.aciklama = VeriErisim.DegerAl(r, 8, "");
                    f.tasinirNo = VeriErisim.DegerAl(r, 9, "");
                    f.odemeEmriNo = VeriErisim.DegerAl(r, 10, "");
                    f.personelAmiri = VeriErisim.DegerAl(r, 11, "");
                    f.personelAmiriUnvan = VeriErisim.DegerAl(r, 12, "");
                    f.genelToplam = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    f.oiv = (decimal)VeriErisim.DegerAl(r, 14, 0.0);
                    f.kdvTevkifati = (decimal)VeriErisim.DegerAl(r, 15, 0.0);
                    f.durum = VeriErisim.DegerAl(r, 16, 0);
                    f.iskonto = (decimal)VeriErisim.DegerAl(r, 17, 0.0);
                    f.iskontoOran = (decimal)VeriErisim.DegerAl(r, 18, 0.0);
                    f.odemeEmriYil = VeriErisim.DegerAl(r, 19, 0);
                    f.paraBirimi = VeriErisim.DegerAl(r, 20, 0);

                    if (f.odemeEmriYil == 0)
                        f.odemeEmriYil = f.yil;
                    if (f.tasinirYil == 0)
                        f.tasinirYil = f.yil;

                    if (kriter.detayTip == 0)
                    {
                        f.firma.kod = VeriErisim.DegerAl(r, 21, 0);
                        f.firma.muhasebe = VeriErisim.DegerAl(r, 22, 0);
                        f.firma.birim = VeriErisim.DegerAl(r, 23, "");
                        f.firma.ad = VeriErisim.DegerAl(r, 24, "");
                        f.firma.vergiNo = VeriErisim.DegerAl(r, 25, "");
                        f.firma.vergiDairesi = VeriErisim.DegerAl(r, 26, "");
                        f.firma.banka = VeriErisim.DegerAl(r, 27, "");
                        f.firma.hesapNo = VeriErisim.DegerAl(r, 28, "");
                        f.firma.eMail = VeriErisim.DegerAl(r, 29, "");
                        f.firma.firmaYetkilisi = VeriErisim.DegerAl(r, 30, "");
                        f.firma.telefon = VeriErisim.DegerAl(r, 31, "");
                        f.firma.faks = VeriErisim.DegerAl(r, 32, "");
                        f.firma.adres = VeriErisim.DegerAl(r, 33, "");
                        f.firma.uyruk = VeriErisim.DegerAl(r, 34, 0);
                        f.firma.gorulmesin = VeriErisim.DegerAl(r, 35, 0);
                        f.firma.yasakli = VeriErisim.DegerAl(r, 36, 0);
                        f.firma.cepTel = VeriErisim.DegerAl(r, 37, "");
                        f.firma.web = VeriErisim.DegerAl(r, 38, "");
                    }
                    bilgi.objeler.Add(f);
                }
                r.Close();

                foreach (FaturaForm f in bilgi.objeler)
                {
                    f.detay = ListeleDetay(ve, kullanan, f, malzemeListesineBagla);
                    if (!f.detay.sonuc.islemSonuc)
                    {
                        bilgi.sonuc.DegerAta(f.detay.sonuc.hataStr);
                        break;
                    }
                    else
                        Hesapla(f);
                }
            }
            else
            {
                bilgi.sonuc.DegerAta(ve.hata);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura form kontrol bilgisini veritabanından okur 
        /// ve gerekli kontrolleri yaptıktan sonra işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ListeleKontrol(Kullanici kullanan, FaturaForm kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (kriter.iskontoOran > 0 && kriter.iskonto > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00090;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.ListeleKontrol(kriter);

            ObjectArray bilgi = new ObjectArray();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FaturaForm f = new FaturaForm();
                    f.prFaturaNo = VeriErisim.DegerAl(r, 4, 0);
                    f.faturaNo = VeriErisim.DegerAl(r, 6, "");
                    bilgi.objeler.Add(f);
                }
            }

            if (bilgi.objeler.Count > 0)
            {
                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00091, kriter.faturaNo, kriter.faturaTarih, kriter.firma.ad);
                sonuc.islemSonuc = false;
            }

            r.Close();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, faturaya ait ödeme emri belge numarasını veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, FaturaForm kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm to = new TABFaturaForm(ve);
            ve.BeginTransaction();

            if (kriter.odemeEmriYil == 0)
                kriter.odemeEmriYil = kriter.yil;

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
        /// Arguman olarak verilen, faturanın durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc DurumDegistir(Kullanici kullanan, FaturaForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            if (bilgi.prFaturaNo > 0)
            {
                ObjectArray liste = Listele(kullanan, bilgi, false);
                foreach (FaturaForm f in liste.objeler)
                {
                    bilgi = f;
                    break;
                }

                ve.BeginTransaction();

                TABFaturaForm to = new TABFaturaForm(ve);
                sonuc = to.DurumDegistir(bilgi);

                string ekHataStr = "";
                if (sonuc.islemSonuc)
                {
                    //Sürece bağlı belgelerin iptali
                    if (bilgi.durum == 0)
                    {
                        //Sürece bağlı ödeme emirleri iptali
                        HRCOdemeEmriForm hrcOdemeForm = new HRCOdemeEmriForm();
                        OdemeEmriMIFForm mifForm = new OdemeEmriMIFForm();
                        mifForm.surecNo = bilgi.surecNo;
                        mifForm.yil = bilgi.odemeEmriYil;
                        mifForm.prFaturaNo = bilgi.prFaturaNo;
                        sonuc = hrcOdemeForm.OdemeEmriIptal(kullanan, mifForm, false);

                        //Sürece bağlı taşınır işlem fişleri iptali
                        if (sonuc.islemSonuc)
                        {
                            HRCTasinirForm hrcTasinirForm = new HRCTasinirForm();
                            TasinirForm tasinirForm = new TasinirForm();
                            tasinirForm.surecNo = bilgi.surecNo;
                            tasinirForm.yil = bilgi.tasinirYil;
                            tasinirForm.prFaturaNo = bilgi.prFaturaNo;
                            sonuc = hrcTasinirForm.TasinirIslemFisIptal(kullanan, tasinirForm);
                            if (!sonuc.islemSonuc)
                                ekHataStr = TNS.HRC.HarcamaSurecleriResI.HRC00092;
                        }
                        else
                            ekHataStr = TNS.HRC.HarcamaSurecleriResI.HRC00093;
                    }
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00095, bilgi.faturaNo);
                    }
                    else
                        sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00094, bilgi.surecNo, ekHataStr, bilgi.faturaNo);
                }

                if (!sonuc.islemSonuc)
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, harcama pusulası bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc DetaySil(Kullanici kullanan, FaturaForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            bilgi.durum = (int)ENUMAktiflik.AKTIF;
            bilgi.prFaturaNo = PrFaturaNoAl(kullanan, bilgi, ve);

            if (bilgi.prFaturaNo > 0)
            {
                ve.BeginTransaction();


                TABFaturaForm to = new TABFaturaForm(ve);

                sonuc = to.SilDetay(bilgi);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00096, bilgi.detaySiraNo) + "<br>";
                }
                else
                    sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00097, bilgi.surecNo, bilgi.detaySiraNo);

                if (!sonuc.islemSonuc)
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, fatura harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMFaturaHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.ListeleButceHazirlikFaturaHarcama(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BHMHarcama bilgi = new BHMHarcama();

                    bilgi.islemYil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.islemAy = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.surecYil = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 5, "");
                    bilgi.kur = VeriErisim.DegerAl(r, 6, "");
                    bilgi.fon = VeriErisim.DegerAl(r, 7, "");
                    bilgi.fin = VeriErisim.DegerAl(r, 8, "");
                    bilgi.eko = VeriErisim.DegerAl(r, 9, "");
                    bilgi.peb = VeriErisim.DegerAl(r, 10, "");
                    bilgi.toplamTutar = VeriErisim.DegerAl(r, 11, (decimal)0);
                    bilgi.yillaraSariIslemMi = Convert.ToBoolean(VeriErisim.DegerAl(r, 12, 0));

                    if (bilgi.yillaraSariIslemMi)
                        bilgi.yil = bilgi.surecYil + 1;
                    else
                        bilgi.yil = bilgi.surecYil;

                    liste.objeler.Add(bilgi);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }


        /// <summary>
        /// Test fonksiyonu
        /// </summary>
        public decimal KdvsizToplamHesapla(ObjectArray list, bool indirimliMi)
        {
            decimal kdvsizToplam = 0;
            decimal satirToplam = 0;
            foreach (FaturaDetay detay in list.objeler)
            {
                satirToplam = 0;
                if (indirimliMi)
                    satirToplam = Math.Round(detay.birimFiyatIndirimli * detay.miktar, 2);
                else
                    satirToplam = Math.Round(detay.birimFiyat * detay.miktar, 2);
                //kdvsizToplam += Math.Round(satirToplam, 2, MidpointRounding.AwayFromZero);2009-166413 nolu süreçte kadv hatalı hesaplanıyordu.Math.Round foreach dışına konuldu. Melih.05-03-2010
                kdvsizToplam += satirToplam;
            }
            kdvsizToplam = Math.Round(kdvsizToplam, 2, MidpointRounding.AwayFromZero);
            return kdvsizToplam;
        }

        private Sonuc FaturaOnaydanBuyukMu(FaturaForm f, Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc();

            //Önceki faturaların kdvsiz toplamlarını al
            //************************************************************
            decimal oncekiFaturaToplamlari = 0;
            TNS.HRC.FaturaForm kriter = new TNS.HRC.FaturaForm();
            kriter.yil = f.yil;
            kriter.muhasebe = f.muhasebe;
            kriter.birim = f.birim;
            kriter.surecNo = f.surecNo;
            kriter.durum = (int)ENUMAktiflik.AKTIF;

            ObjectArray faturaBilgi = new ObjectArray();
            //faturaBilgi = servisHRC.FaturaFormListele(kullanan, kriter);
            faturaBilgi = new HRCFaturaForm().Listele(kullanan, kriter, false);

            foreach (TNS.HRC.FaturaForm ff in faturaBilgi.objeler)
            {
                //Önceden kesilmiş fatura üzerinde değişiklik yapılıyorsa eski fiyatı dikkate alma
                if (ff.prFaturaNo != f.prFaturaNo)
                {
                    foreach (TNS.HRC.FaturaDetay fd in ff.detay.objeler)
                        oncekiFaturaToplamlari += fd.satirToplam;//KDVsiz toplam almak için
                }
            }
            //************************************************************

            //Şu anda kayıt edilmek istenilen fatura
            //************************************************************
            foreach (TNS.HRC.FaturaDetay fd in f.detay.objeler)
                oncekiFaturaToplamlari += fd.satirToplam;//KDVsiz toplam almak için
            //************************************************************

            OnayForm onayForm = new OnayForm();
            onayForm.yil = f.yil;
            onayForm.muhasebe = f.muhasebe;
            onayForm.birim = f.birim;
            onayForm.surecNo = f.surecNo;
            ObjectArray onayFormBilgi = new ObjectArray();
            //onayFormBilgi = servisHRC.OnayFormListele(kullanan, onayForm);
            onayFormBilgi = new HRCOnayForm().Listele(kullanan, onayForm);
            if (onayFormBilgi.objeler.Count > 0)
            {
                double onayTutari = ((OnayForm)onayFormBilgi.objeler[0]).yaklasikMaliyet;

                double limit = 60;
                double limitTutar = onayTutari * ((limit / 100) + 1);

                if (limitTutar < OrtakFonksiyonlar.ConvertToDbl(oncekiFaturaToplamlari))
                    sonuc.hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00098, onayTutari.ToString("#,###.00"), oncekiFaturaToplamlari.ToString("#,###.00"), limit);
            }
            else
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00099;
            if (sonuc.hataStr != "")
                sonuc.islemSonuc = false;

            return sonuc;
        }

        private ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, FaturaForm kriter, bool malzemeListesineBagla)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.ListeleDetay(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FaturaDetay t = new FaturaDetay();
                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    t.birim = VeriErisim.DegerAl(r, 2, "");
                    t.surecNo = VeriErisim.DegerAl(r, 3, "");
                    t.prFaturaNo = VeriErisim.DegerAl(r, 4, 0);
                    t.detaySiraNo = VeriErisim.DegerAl(r, 5, 0);
                    t.adi = VeriErisim.DegerAl(r, 6, "");
                    t.miktar = (decimal)VeriErisim.DegerAl(r, 7, 0.0);
                    t.kdvOran = VeriErisim.DegerAl(r, 8, 0);
                    t.birimFiyat = (decimal)VeriErisim.DegerAl(r, 9, 0.0);
                    t.belgeSahibi = VeriErisim.DegerAl(r, 10, "");
                    t.belgeTarihi = VeriErisim.DegerAl(r, 11);
                    t.belgeNo = VeriErisim.DegerAl(r, 12, "");
                    t.aciklama = VeriErisim.DegerAl(r, 13, "");
                    t.detayTip = VeriErisim.DegerAl(r, 14, 0);
                    t.satirToplam = (decimal)VeriErisim.DegerAl(r, 15, 0.0);
                    t.belgeSahibiTCNo = VeriErisim.DegerAl(r, 16, "");
                    t.kdvTutari = (decimal)VeriErisim.DegerAl(r, 17, 0.0);
                    t.malzemeNo = VeriErisim.DegerAl(r, 18, 0);

                    t.belgeyiAlan = VeriErisim.DegerAl(r, 19, "");
                    t.belgeyiAlanTCNo = VeriErisim.DegerAl(r, 20, "");
                    t.birimFiyatIndirimli = (decimal)VeriErisim.DegerAl(r, 21, 0.0);

                    t.malzemeKod = VeriErisim.DegerAl(r, 22, "");
                    t.olcuBirimKod = VeriErisim.DegerAl(r, 23, 0);
                    t.olcuBirimAd = VeriErisim.DegerAl(r, 24, "");
                    bool ekle = true;
                    if (string.IsNullOrWhiteSpace(t.adi))
                    {
                        if (malzemeListesineBagla)
                        {
                            t.adi = VeriErisim.DegerAl(r, 25, "");
                            t.miktar = VeriErisim.DegerAl(r, 26, (decimal)0);
                            t.malzemeNo = VeriErisim.DegerAl(r, 27, 0);
                            t.malzemeKod = VeriErisim.DegerAl(r, 28, "");
                        }
                        else
                            ekle = false;
                    }
                    if (ekle)
                        bilgi.objeler.Add(t);
                }
                r.Close();
            }
            else
                bilgi.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();

            return bilgi;
        }

        private int PrFaturaNoAl(Kullanici kullanan, FaturaForm bilgi, VeriErisim ve)
        {
            int prFaturaNo = 0;
            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.Listele(bilgi);
            if (to.Hata() == "")
            {
                if (r.Read())
                    prFaturaNo = VeriErisim.DegerAl(r, 4, 0);
            }
            r.Close();
            return prFaturaNo;
        }

        private void Hesapla(FaturaForm f)
        {
            decimal kdv1 = 0;
            decimal kdv8 = 0;
            decimal kdv10 = 0;
            decimal kdv12 = 0;
            decimal kdv18 = 0;
            decimal kdv20 = 0;
            decimal kdvTutar = 0;
            decimal kdvToplam = 0;
            f.kdvsizToplam = this.KdvsizToplamHesapla(f.detay, false);
            decimal iskonto = 0;
            decimal iskontoOran = 0;

            if (f.iskonto > 0)
            {
                iskonto = f.iskonto;
                iskontoOran = iskonto / f.kdvsizToplam;
            }
            else if (f.iskontoOran > 0)
            {
                iskonto = Math.Round(f.kdvsizToplam * f.iskontoOran / 100, 2, MidpointRounding.AwayFromZero);
                iskontoOran = Math.Round(f.iskontoOran / 100, 2);
                f.iskonto = iskonto;
            }

            bool kdvHesapla = false;
            if (iskonto > 0)
            {
                foreach (TNS.HRC.FaturaDetay dty in f.detay.objeler)
                {
                    if (decimal.Equals(dty.birimFiyat, dty.birimFiyatIndirimli))
                        dty.birimFiyatIndirimli *= (1 - iskontoOran);
                }

                decimal indKdvsizToplam = this.KdvsizToplamHesapla(f.detay, true);
                decimal hesaplananInd = f.kdvsizToplam - indKdvsizToplam;

                int i = 0;
                while (iskonto != hesaplananInd)
                {
                    if (i > 10)
                        break;

                    decimal fark = hesaplananInd - iskonto;

                    //Listedeki ilk objenin birim fiyatına aradaki fark kadar ekleme yap
                    TNS.HRC.FaturaDetay ilkDetay = new TNS.HRC.FaturaDetay();
                    ilkDetay = (TNS.HRC.FaturaDetay)f.detay.objeler[0];
                    ilkDetay.birimFiyatIndirimli += (fark / ilkDetay.miktar);
                    ilkDetay.satirToplam = ilkDetay.birimFiyat * ilkDetay.miktar;

                    indKdvsizToplam = this.KdvsizToplamHesapla(f.detay, true);
                    hesaplananInd = f.kdvsizToplam - indKdvsizToplam;
                    i++;
                }

                kdvHesapla = true;
            }
            else
                kdvHesapla = true;

            if (kdvHesapla)
            {
                foreach (TNS.HRC.FaturaDetay dty in f.detay.objeler)
                {
                    //05.03.2010 166413(265579-265581) nolu süreç kdv hata veriyordu
                    //kdvTutar = Math.Round(dty.birimFiyatIndirimli * dty.miktar * dty.kdvOran / 100, 2, MidpointRounding.AwayFromZero); 
                    kdvTutar = dty.birimFiyatIndirimli * dty.miktar * dty.kdvOran / 100;
                    kdvToplam += kdvTutar;

                    if (dty.kdvOran == 1)
                        kdv1 += kdvTutar;
                    else if (dty.kdvOran == 8)
                        kdv8 += kdvTutar;
                    else if (dty.kdvOran == 10)
                        kdv10 += kdvTutar;
                    else if (dty.kdvOran == 12)
                        kdv12 += kdvTutar;
                    else if (dty.kdvOran == 18)
                        kdv18 += kdvTutar;
                    else if (dty.kdvOran == 20)
                        kdv20 += kdvTutar;
                }

                f.kdv1 = kdv1;
                f.kdv8 = kdv8;
                f.kdv10 = kdv10;
                f.kdv12 = kdv12;
                f.kdv18 = kdv18;
                f.kdv20 = kdv20;
            }
            f.kdv1 = Math.Round(f.kdv1, 2, MidpointRounding.AwayFromZero);
            f.kdv8 = Math.Round(f.kdv8, 2, MidpointRounding.AwayFromZero);
            f.kdv10 = Math.Round(f.kdv10, 2, MidpointRounding.AwayFromZero);
            f.kdv12 = Math.Round(f.kdv12, 2, MidpointRounding.AwayFromZero);
            f.kdv18 = Math.Round(f.kdv18, 2, MidpointRounding.AwayFromZero);
            f.kdv20 = Math.Round(f.kdv20, 2, MidpointRounding.AwayFromZero);

            f.genelToplam = Math.Round(f.kdvsizToplam + kdvToplam + f.oiv - iskonto, 2);
        }

        public ObjectArray RaporIhaleFirmaListele(RaporIhaleFirma raporIhaleFirma)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.ListeleRaporIhaleFirma(raporIhaleFirma);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    RaporIhaleFirma t = new RaporIhaleFirma();
                    t.kurKod = VeriErisim.DegerAl(r, 0, "");
                    t.ihaleUsulu = VeriErisim.DegerAl(r, 1, 0);
                    t.toplam = VeriErisim.DegerAl(r, 2, decimal.Zero);
                    t.firmaVergiNo = VeriErisim.DegerAl(r, 3, "");
                    t.firmaAd = VeriErisim.DegerAl(r, 4, "");

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray ListeleServis(Kullanici kullanan, FaturaForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABFaturaForm to = new TABFaturaForm(ve);
            IDataReader r = to.FaturaListeleServis(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FaturaForm f = new FaturaForm();
                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.surecNo = VeriErisim.DegerAl(r, 1, "");
                    f.firmaNo = VeriErisim.DegerAl(r, 2, 0);
                    f.firma.ad = VeriErisim.DegerAl(r, 3, "");
                    f.faturaTarih = VeriErisim.DegerAl(r, 4);
                    f.tMalzeme.birimFiyat = VeriErisim.DegerAl(r, 5, 0.0);
                    f.tMalzeme.malzemeKod = VeriErisim.DegerAl(r, 6, "");
                    f.tMalzeme.adi = VeriErisim.DegerAl(r, 7, "");

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }
            else
            {
                bilgi.sonuc.DegerAta(ve.hata);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
