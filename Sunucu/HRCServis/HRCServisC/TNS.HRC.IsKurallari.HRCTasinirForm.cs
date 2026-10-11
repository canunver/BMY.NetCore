using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.TMM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Taşınır form bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTasinirForm
    {
        /// <summary>
        /// Taşınır Mal Modul servisi
        /// </summary>
        public static ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!servisTMM.YazmaYetkisiVarMi(kullanan, kriter.muhasebe.ToString().PadLeft(5, '0'), kriter.harcamaBirimKod, kriter.ambarKod))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (kriter.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (kriter.birim.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (kriter.surecNo.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";
            if (kriter.prFaturaNo == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00270 + "<br>";
            if (kriter.harcamaBirimKod.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (kriter.ambarKod.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00271 + "<br>";
            if (kriter.tasinirTarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00272 + "<br>";

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

            if (kriter.prTasinirNo == 0)
            {
                //Firma kod alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "HRCTASINIRNO";

                kriter.prTasinirNo = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                //**********************************************************
            }

            TABTasinirForm to = new TABTasinirForm(ve);
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
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.anahtar = kriter.prTasinirNo.ToString();
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
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır form bilgisini veritabanından okur 
        /// ve TasinirForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, taşınır form listesini TasinirForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TasinirForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHBirim = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirForm to = new TABTasinirForm(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TasinirForm f = new TasinirForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.prTasinirNo = VeriErisim.DegerAl(r, 4, 0);
                    f.prFaturaNo = VeriErisim.DegerAl(r, 5, 0);
                    f.tasinirNo = VeriErisim.DegerAl(r, 6, "");
                    f.tasinirTarih = VeriErisim.DegerAl(r, 7);
                    f.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHBirim, VeriErisim.DegerAl(r, 8, ""));
                    f.ambarKod = VeriErisim.DegerAl(r, 9, "");
                    f.aciklama = VeriErisim.DegerAl(r, 10, "");
                    f.dovizKur = VeriErisim.DegerAl(r, 11, 0.0);
                    f.tasinirYil = f.tasinirTarih.Oku().Year;

                    f.detay = ListeleDetay(kullanan, f);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır işlem belgesinin durum bilgisi iptal olarak veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">İptal edilecek bilgiyi içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TasinirIslemFisIptal(Kullanici kullanan, TasinirForm kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            TasinirForm tmmform = new TasinirForm();

            ObjectArray tmmFormList = new HRCTasinirForm().Listele(kullanan, kriter);

            foreach (TasinirForm surecTmmForm in tmmFormList.objeler)
            {
                TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();
                tf.yil = surecTmmForm.yil;
                tf.muhasebeKod = surecTmmForm.muhasebe.ToString().PadLeft(5, '0');
                tf.harcamaKod = surecTmmForm.harcamaBirimKod;
                tf.fisNo = surecTmmForm.tasinirNo;

                int fisDurum = servisTMM.TasinirIslemFisiDurumGetir(kullanan, tf);
                if (fisDurum != (int)TNS.TMM.ENUMBelgeDurumu.IPTAL)
                {
                    sonuc = servisTMM.TasinirIslemFisiDurumDegistir(kullanan, tf, "İptal");
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00268, "");


            if (!servisTMM.YazmaYetkisiVarMi(kullanan, kriter.muhasebe.ToString().PadLeft(5, '0'), kriter.harcamaBirimKod, kriter.ambarKod))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (kriter.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (kriter.birim.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (kriter.surecNo.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";
            if (kriter.prTasinirNo == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00273 + "<br>";

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
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00233;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirForm to = new TABTasinirForm(ve);
            ve.BeginTransaction();

            sonuc = to.SilDetay(kriter);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(kriter);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır harcama bilgisini veritabanından okur 
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
        /// <returns>İstenilen kriterdeki, taşınır harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMTeslimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABTasinirForm to = new TABTasinirForm(ve);
            IDataReader r = to.ListeleButceHazirlikTeslimHarcama(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

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

        private ObjectArray ListeleDetay(Kullanici kullanan, TasinirForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHesap = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirForm to = new TABTasinirForm(ve);
            IDataReader r = to.ListeleDetay(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TasinirDetay t = new TasinirDetay();

                    t.adi = VeriErisim.DegerAl(r, 6, "");
                    t.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 7, ""));
                    t.miktar = VeriErisim.DegerAl(r, 8, 0.0);
                    t.olcuBirimKod = VeriErisim.DegerAl(r, 9, 0);
                    t.kdvOran = VeriErisim.DegerAl(r, 10, 0);
                    t.birimFiyat = VeriErisim.DegerAl(r, 11, 0.0);
                    t.olusturuldu = VeriErisim.DegerAl(r, 12, 0);
                    t.faturaDetaySiraNo = VeriErisim.DegerAl(r, 13, 0);
                    t.hesapPlanAd = HRCServis.UzayYaziDegerGetir("TASHESAPPLAN", t.hesapPlanKod, true, "ACIKLAMA");

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
