using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Firmalar tarafından malzemelere yapılan teklif bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklif
    {
        /// <summary>
        /// Arguman olarak verilen, firmalar tarafından malzemelere yapılan teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Teklif nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            Teklif teklifKriter = (Teklif)liste.objeler[0];
            int yil = teklifKriter.yil;
            string surecNo = teklifKriter.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = true;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklif to = new TABTeklif(ve);
            ve.BeginTransaction();

            sonuc = to.Sil((Teklif)liste.objeler[0], false);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            decimal toplamTutar = 0;
            //Her teklifin tutari genel toplama eklenip ortak tabloya yazılsın
            if (sonuc.islemSonuc)
            {
                foreach (Teklif var in liste.objeler)
                {
                    if (var.kazanan == (int)ENUMEvetHayir.EVET)
                        toplamTutar += var.toplamTutar;
                }
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
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalara ait malzeme teklif bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firmalara ait malzeme teklif bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Teklif kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklif to = new TABTeklif(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    t.birim = VeriErisim.DegerAl(r, 2, "");
                    t.surecNo = VeriErisim.DegerAl(r, 3, "");
                    t.malzeme.malzemeNo = VeriErisim.DegerAl(r, 4, 0);
                    t.firma.firmaNo = VeriErisim.DegerAl(r, 5, 0);
                    t.donem.donemNo = VeriErisim.DegerAl(r, 6, 0);
                    t.tip = VeriErisim.DegerAl(r, 7, 0);
                    t.birimFiyat = VeriErisim.DegerAl(r, 8, 0.0);
                    t.kazanan = VeriErisim.DegerAl(r, 9, 0);
                    t.malzeme.malzemeKod = VeriErisim.DegerAl(r, 10, "");
                    t.aciklama = VeriErisim.DegerAl(r, 11, "");
                    t.yMParaBirimi = VeriErisim.DegerAl(r, 12, 0);
                    t.donem.tarih = VeriErisim.DegerAl(r, 13);
                    t.malzeme.adi = VeriErisim.DegerAl(r, 14, "");
                    t.malzeme.miktar = VeriErisim.DegerAl(r, 15, 0.0);
                    t.malzeme.olcuBirim.kod = VeriErisim.DegerAl(r, 16, 0);
                    t.malzeme.olcuBirim.ad = VeriErisim.DegerAl(r, 17, "");
                    t.malzeme.malzemeKod = VeriErisim.DegerAl(r, 18, "");

                    if ((t.tip == (int)ENUMTeklifTuru.YAKLASIKMALIYETFIRMA) || (t.tip == (int)ENUMTeklifTuru.PIYASAFIYATARASTIRMASI)
                        || (t.tip == (int)ENUMTeklifTuru.KISMITEKLIF))
                    {
                        t.firma.firmaBilgisi.ad = VeriErisim.DegerAl(r, 19, "");
                        t.firma.firmaBilgisi.adres = VeriErisim.DegerAl(r, 20, "");
                        t.firma.firmaBilgisi.kod = VeriErisim.DegerAl(r, 21, 0);
                    }
                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYMOrtBirimFiyat(Kullanici kullanan, Teklif kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklif to = new TABTeklif(ve);
            IDataReader r = to.ListeleYMOrtBirimFiyat(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.malzeme.malzemeNo = VeriErisim.DegerAl(r, 0, 0);
                    t.toplamTutar = VeriErisim.DegerAl(r, 3, (decimal)0);

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Kısmi Teklif ile ihale kazanan 1. ve 2. firma bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki kısmi teklif kazanan firma bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleKismiTeklifKazanan(Kullanici kullanan, Teklif kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklif to = new TABTeklif(ve);
            IDataReader r = to.ListeleKismiTeklifKazanan(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.firma.firmaBilgisi.kod = VeriErisim.DegerAl(r, 0, 0);
                    t.firma.firmaBilgisi.ad = VeriErisim.DegerAl(r, 1, "");
                    t.firma.firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 2, "");
                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, alım harcama bilgisini veritabanından okur 
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
        /// <returns>İstenilen kriterdeki, alım harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMAlimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABTeklif to = new TABTeklif(ve);
            IDataReader r = to.ListeleButceHazirlikAlimHarcama(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

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

        public TeklifKazananFirma KazananGetir(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, int malzemeNo)
        {
            ObjectArray oa = KazananGetirListe(veriErisim, kullanan, yil, surecNo, malzemeNo);
            if (oa.ObjeSayisi > 0)
                return (TeklifKazananFirma)oa.objeler[0];
            return null;
        }

        internal ObjectArray KazananGetirListe(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, int malzemeNo)
        {
            ObjectArray bilgi = new ObjectArray();

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            //TABTeklif to = new TABTeklif(ve);
            //IDataReader reader = to.KazananGetir(yil, surecNo, malzemeNo);
            //TeklifKazananFirma teklif = null;
            //if (to.Hata() == "")
            //{
            //    if (reader.Read())
            //    {
            //        teklif = new TeklifKazananFirma();

            //        teklif.firmaNo = VeriErisim.DegerAl(reader, 0, 0);
            //        teklif.tip = VeriErisim.DegerAl(reader, 1, 0);
            //        teklif.muhasebeKod = VeriErisim.DegerAl(reader, 2, 0);
            //        teklif.vergiNo = VeriErisim.DegerAl(reader, 3, "");
            //        bilgi.objeler.Add(teklif);
            //    }
            //    reader.Close();
            //}
            //ve.VeriErisimKapat();

            Teklif kriter = new Teklif();
            kriter.yil = surecYil;
            kriter.surecNo = surecNo;
            kriter.malzeme.malzemeNo = malzemeNo;
            ObjectArray oa = ListeleKismiTeklifKazanan(kullanan, kriter);

            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                foreach (Teklif t in oa.objeler)
                {
                    TeklifKazananFirma tkf = new TeklifKazananFirma();
                    tkf.vergiNo = t.firma.firmaBilgisi.vergiNo;
                    tkf.ad = t.firma.firmaBilgisi.ad;
                    bilgi.Ekle(tkf);
                }
            }
            else
            {
                if (!oa.sonuc.islemSonuc)
                {
                    bilgi.sonuc = oa.sonuc;
                }
                else
                {
                    KazananFirma kFirma = new HRCKazananFirma().Listele(kullanan, new KazananFirma(surecYil, 0, "", surecNo));
                    if (kFirma.firmaVergiNo1 != "")
                    {
                        TeklifKazananFirma tkf = new TeklifKazananFirma();
                        tkf.vergiNo = kFirma.firmaVergiNo1;
                        tkf.ad = kFirma.firmaAd1;
                        bilgi.Ekle(tkf);
                    }
                    else
                    {
                        bilgi.sonuc.DegerAta("Kazanan firmalar bulunamadı. Lütfen önceki aşamalardan kazanan firmaları işaretleyiniz!");
                    }
                }
            }
            return bilgi;
        }
    }
}
