using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Çeşitli ödemeler bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCMahkemeHarc
    {
        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="coler">Kaydedilecek bilgileri CesitliOdeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc Kaydet(Kullanici kullanan, ObjectArray coler, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (coler.ObjeSayisi <= 0 || coler.objeler[0] == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00066;

                return sonuc;
            }

            MahkemeHarc ilkKayit = (MahkemeHarc)coler[0];
            sonuc = Kontrol(ilkKayit, parcaKod, kullanan);

            if (!sonuc.islemSonuc)
                return sonuc;

            for (int i = 0; i < coler.ObjeSayisi; i++)
            {
                MahkemeHarc oi = (MahkemeHarc)coler[i];
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMahkemeHarc tabCOB = new TABMahkemeHarc(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tabCOB.Sil(ilkKayit);

                for (int i = 0; i < coler.ObjeSayisi; i++)
                {
                    if (!sonuc.islemSonuc) break;
                    sonuc = tabCOB.Kaydet(ilkKayit.surecYil, ilkKayit.muhasebeKod, ilkKayit.birimKod, ilkKayit.surecNo, i, (MahkemeHarc)coler[i]);
                }

                //Süreç devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                if (sonuc.islemSonuc)
                    sonuc = new HRCSurec().SurecDurumKaydet(ve, kullanan, ilkKayit.surecYil, ilkKayit.surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                ///////////////////////////////////////////////////////////////////////////

                if (sonuc.islemSonuc)
                    sonuc = new HRCSurec().ParcaDurumKaydet(ve, kullanan, ilkKayit.surecYil, ilkKayit.surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
                ve.Commit();
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemiYapan">The kullanan.</param>
        /// <param name="mahkemeHarc">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        public static ObjectArray Listele(Kullanici islemiYapan, MahkemeHarc mahkemeHarc)
        {
            ObjectArray mhler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABMahkemeHarc tabMH = new TABMahkemeHarc(ve);

            IDataReader dr = tabMH.Listele(mahkemeHarc);
            if (dr != null)
            {
                while (dr.Read())
                {
                    MahkemeHarc mhYeni = new MahkemeHarc();

                    mhYeni.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                    mhYeni.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                    mhYeni.birimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    mhYeni.surecNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                    mhYeni.birimAciklama = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.BIRIMACIKLAMA, string.Empty);
                    mhYeni.davaci = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.DAVACI, "");
                    mhYeni.davaKonusu = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.DAVAKONUSU, "");
                    mhYeni.mahkemeKararNo = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.MAHKEMEKARARNO, "");
                    mhYeni.icraTakipDurumu = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.ICRATAKIPDURUMU, "");

                    mhYeni.yargitayDanistayKarari = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.YARGITAYDANISTAYKARARI, "");
                    mhYeni.kesinlesmisMahkemeKarari = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.KESINLESMISMAHKEMEKARARI, "");
                    mhYeni.kesinlestiMi = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.KESINLESTIMI, "");

                    mhYeni.miktar = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.MIKTAR, 0.0);
                    mhYeni.odemeEmriNo = VeriErisim.DegerAl(dr, (int)TABMahkemeHarc.ENUMAlanlar.ODEMEEMRINO, "");
                    mhler.objeler.Add(mhYeni);
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ve.VeriErisimKapat();
            return mhler;
        }

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="mahkemeHarc">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, MahkemeHarc mahkemeHarc)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMahkemeHarc to = new TABMahkemeHarc(ve);

            ObjectArray coler = Listele(kullanan, mahkemeHarc);

            ve.BeginTransaction();
            if (coler.objeler.Count > 0)
            {
                MahkemeHarc mh = (MahkemeHarc)coler.objeler[0];
                mh.odemeEmriNo = mahkemeHarc.odemeEmriNo;
                sonuc = to.OdemeEmriNoKaydet(mh);
            }

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

        private static Sonuc Kontrol(MahkemeHarc co, int parcaKod, Kullanici kullanan)
        {
            string hata = string.Empty;

            Sonuc sonuc = new Sonuc();

            if (co.surecYil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }

            if (co.muhasebeKod == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;
            }
            if (co.birimKod == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005;
            }
            if (co.surecNo == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067;
            }
            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", co.muhasebeKod.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, co.muhasebeKod);
                }

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", co.birimKod.Trim() + "-" + co.muhasebeKod, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, co.birimKod);
                }
            }

            bool tamamlandi = true;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, co.surecYil, co.surecNo, parcaKod);

            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }

            return sonuc;
        }

        //public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
        //    TABCesitliOdemeBordro toCesitliOdemeBordro = new TABCesitliOdemeBordro(ve);

        //    IDataReader dr = toCesitliOdemeBordro.ListeleOdemeEmri(yil, surecNo);

        //    while (dr.Read())
        //    {
        //        CesitliOdeme sd = new CesitliOdeme();
        //        sd.surecYil = VeriErisim.DegerAl(dr, 0, 0);
        //        sd.odemeEmriNo = VeriErisim.DegerAl(dr, 1, "");

        //        bilgi.Ekle(sd);
        //    }
        //    dr.Close();

        //    if (bilgi.ObjeSayisi == 0)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00071;
        //    }
        //    else
        //        bilgi.sonuc.islemSonuc = true;

        //    ve.VeriErisimKapat();
        //    return bilgi;
        //}
    }
}
