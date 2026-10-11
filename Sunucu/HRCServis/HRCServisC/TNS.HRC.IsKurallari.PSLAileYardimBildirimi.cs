using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Aile yardım bildirim belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAileYardimBildirimi
    {
        /// <summary>
        /// Personel modülü servisine ulaşmak için kullanılan değişken
        /// </summary>
        public static IPERServis PSLservis = TNS.PER.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, aile yardım bildirim belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            //if (kriter.cocuklar.ObjeSayisi > 0)
            //{
            if (kriter.TCKimlikNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00284 + "<br>";
            else
            {
                int satir = 0;
                //ILERDE BU KISIM ISTEMCI TARAFINDAN DOLDURULUNCA KALKACAK.
                //--->KALKACAK YERİN BAŞLANGICI 
                //Personel Unvanini ekle 
                TNS.PER.Personel personel = new TNS.PER.Personel();
                personel.birimKodu = kriter.birim;
                personel.muhasebeKodu = kriter.muhasebe;
                personel.tcKimlikNo = kriter.TCKimlikNo;

                ObjectArray ObjectArray = PSLservis.PersonelListele(kullanan, personel);

                if (ObjectArray.sonuc.islemSonuc)
                {

                    if (ObjectArray.objeler.Count > 0)
                    {
                        personel = (TNS.PER.Personel)ObjectArray.objeler[0];
                    }
                }
                else
                {
                    hata = ObjectArray.sonuc.hataStr;
                }
                //Hata var ise geri dön uyarı ver
                kriter.unvanKodu = personel.unvanKodu;

                PersonelAile perAileKriter = new PersonelAile();
                //perAileKriter.personelTcKimlikNo = kriter.TCKimlikNo;
                //.:KmL:. 22.07.08
                //Personel Aile Listeleme bölümünde kriter olarak tcKimlikNo belirlenmiş,
                //personelTcKimlik No belirlenmemiş düzeltilmesi lazım 
                perAileKriter.tcKimlikNo = kriter.TCKimlikNo;

                ObjectArray perBilgiVan = PSLservis.PersonelAileListele(kullanan, perAileKriter);

                foreach (PersonelAile aileFerdi in perBilgiVan.objeler)
                {
                    if (aileFerdi.yakinlik == 1) //Eşi
                    {
                        kriter.esAdSoyad = aileFerdi.ad + " " + aileFerdi.soyad;
                        kriter.esIsDurumu = aileFerdi.durum;
                        kriter.aileCuzdaniSeriNo = aileFerdi.aileCuzdaniSeriNo;
                        kriter.evlenmeTarihi = aileFerdi.evlenmeTarihi;
                        kriter.medeniHali = 2; //Evli
                    }
                    else
                    {
                        if (kriter.cocuklar.objeler.Count > 0)
                        {
                            foreach (PSLAileYardimBildirimiSatir t in kriter.cocuklar.objeler)
                            {
                                if (t.cocukTCKimlikNo == "")
                                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00285, satir) + "<br>";
                                else
                                {
                                    if (aileFerdi.tcKimlikNo == t.cocukTCKimlikNo.Trim())
                                    {
                                        t.cocukYakinligi = (int)ENUMCOCUKDURUM.OZ;// aileFerdi.cocukYakinligi;
                                        t.dogumTarihi = aileFerdi.dogumTarihi;
                                        t.babaAd = "";// aileFerdi.babaAdi;
                                        t.anaAd = "";// aileFerdi.anneAdi;
                                        t.cinsiyet = aileFerdi.cinsiyet;
                                    }
                                }
                            }
                        }
                    }
                }


            }


            if (hata != "")
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
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));


            TABAileYardimBildirimi to = new TABAileYardimBildirimi(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc = to.SilSatir(kriter);

                if (sonuc.islemSonuc)
                    sonuc = to.KaydetSatir(kriter);
            }

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
        /// Arguman olarak verilen kriterlere bağlı olarak, aile yardım bildirim belge bilgisini veritabanından okur 
        /// ve PSLAileYardimBildirimiBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, aile yardım bildirim belge bilgisini PSLAileYardimBildirimiBaslik nesnesi olarak tutan nesne</returns>
        public PSLAileYardimBildirimiBaslik Listele(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter)
        {
            PSLAileYardimBildirimiBaslik bilgi = new PSLAileYardimBildirimiBaslik();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAileYardimBildirimi to = new TABAileYardimBildirimi(ve);
            IDataReader r = to.ListeleBaslik(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.TCKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    bilgi.perAdSoyad = VeriErisim.DegerAl(r, 5, "");
                    bilgi.unvanKodu = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.medeniHali = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.ilgiliAy = VeriErisim.DegerAl(r, 8, 0);
                    bilgi.ilgiliYil = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.esAdSoyad = VeriErisim.DegerAl(r, 10, "");
                    bilgi.evlenmeTarihi = VeriErisim.DegerAl(r, 11);
                    bilgi.aileCuzdaniSeriNo = VeriErisim.DegerAl(r, 12, "");
                    bilgi.esIsDurumu = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.esAciklama = VeriErisim.DegerAl(r, 14, "");
                    bilgi.gorevi = VeriErisim.DegerAl(r, 15, "");
                    bilgi.muhasebeAd = VeriErisim.DegerAl(r, 16, "");
                    bilgi.birimAd = VeriErisim.DegerAl(r, 17, "");

                    bilgi.cocuklar = ListeleSatir(kullanan, kriter);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }


        private ObjectArray ListeleSatir(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAileYardimBildirimi to = new TABAileYardimBildirimi(ve);
            IDataReader r = to.ListeleSatir(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    PSLAileYardimBildirimiSatir satir = new PSLAileYardimBildirimiSatir();

                    satir.perTCKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    satir.cocukTCKimlikNo = VeriErisim.DegerAl(r, 5, "");
                    satir.cocukAdSoyad = VeriErisim.DegerAl(r, 6, "");
                    satir.dogumTarihi = VeriErisim.DegerAl(r, 7);
                    satir.cinsiyet = VeriErisim.DegerAl(r, 8, 0);
                    satir.babaAd = VeriErisim.DegerAl(r, 9, "");
                    satir.anaAd = VeriErisim.DegerAl(r, 10, "");
                    satir.cocukYakinligi = VeriErisim.DegerAl(r, 11, 0);
                    satir.yuksOgrKayitTarihi = VeriErisim.DegerAl(r, 12);
                    satir.yuksOgrOkulAdi = VeriErisim.DegerAl(r, 13, "");
                    satir.sinif = VeriErisim.DegerAl(r, 14, 0);
                    satir.bursDurumu = VeriErisim.DegerAl(r, 15, 0);
                    satir.aciklama = VeriErisim.DegerAl(r, 16, "");

                    bilgi.objeler.Add(satir);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
