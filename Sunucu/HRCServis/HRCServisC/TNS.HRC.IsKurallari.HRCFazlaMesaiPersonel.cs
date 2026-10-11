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
    /// Fazla mesai düzenlenen personel bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCFazlaMesaiPersonel
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai düzenlenen personel bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islem yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai düzenlenen personel bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        public FazlaMesai Listele(Kullanici kullanici, FazlaMesai f)
        {
            FazlaMesai fm = new FazlaMesai();
            string hata = "";

            hata = KontrolUstBilgi(f);
            if (hata != "")
            {
                fm.sonuc.hataStr = hata;
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            /*if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                fm.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABFazlaMesaiPersonel tab = new TABFazlaMesaiPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                fm.sonuc.islemSonuc = false;
                fm.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fm;
            }

            bool ilk = true;

            while (r.Read())
            {
                if (ilk)
                {
                    fm.yil = VeriErisim.DegerAl(r, 0, 0);
                    fm.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    fm.birim = VeriErisim.DegerAl(r, 2, "");
                    fm.surecNo = VeriErisim.DegerAl(r, 3, "");
                    fm.ilgiliYil = VeriErisim.DegerAl(r, 4, 0);
                    fm.ilgiliAy = VeriErisim.DegerAl(r, 5, 0);
                    ilk = false;
                }

                FazlaMesaiPersonel fp = new FazlaMesaiPersonel();
                fp.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");
                fp.puantaj.kod = VeriErisim.DegerAl(r, 7, 0);
                fp.gelirVergisiOrani = VeriErisim.DegerAl(r, 9, 0);
                fp.adSoyad = VeriErisim.DegerAl(r, 11, "");

                fm.personel.Add(fp);
            }
            r.Close();

            fm.sonuc.islemSonuc = true;

            if (fm.yil == 0)
                fm.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fm;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel bilgisini veritabanından okur 
        /// ve ObjectArray nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini ObjectArray nesnesi olarak tutan nesne</returns>
        public ObjectArray ListelePersonel(Kullanici islemiYapan, int muhasebe, string birim)
        {
            ObjectArray pVAN = new ObjectArray();

            if (muhasebe == 0 || birim.Trim() == "")
            {
                pVAN.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00079;
                pVAN.sonuc.islemSonuc = false;
                return pVAN;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(islemiYapan, muhasebe, birim))
            {
                pVAN.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                pVAN.sonuc.islemSonuc = false;
                return pVAN;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABFazlaMesaiPersonel tab = new TABFazlaMesaiPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListelePersonel(muhasebe, birim);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();

                return pVAN;
            }

            while (r.Read())
            {
                Personel p = new Personel();
                p.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                // p.adSoyad = VeriErisim.DegerAl(r, 1, "");
                // p.unvan = VeriErisim.DegerAl(r, 2, "");

                pVAN.objeler.Add(p);
            }
            r.Close();
            pVAN.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            return pVAN;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, FazlaMesai f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            hata = KontrolUstBilgi(f);
            if (hata == "")
                hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */


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

            ve.BeginTransaction();

            TABFazlaMesaiPersonel tab = new TABFazlaMesaiPersonel(ve);

            sonuc = Sil(ve, kullanan, f, islemKod, parcaKod);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            int siraNo = 0;
            foreach (FazlaMesaiPersonel fp in f.personel)
            {
                sonuc = PersonelinBaskaKaydiYok(tab, f.ilgiliYil, f.ilgiliAy, f.surecNo, fp.tcKimlikNo);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }

                sonuc = tab.Kaydet(f, fp, siraNo);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
                siraNo++;


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
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private Sonuc PersonelinBaskaKaydiYok(TABFazlaMesaiPersonel tab, int yil, int ay, string surecNo, string TCKimlikNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            //IDataReader r = null;
            //r = tab.PersonelinBaskaKaydiVarMi(yil, ay, surecNo, TCKimlikNo);

            //if (r != null && r.Read())
            //{
            //    string tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
            //    int surecYil = VeriErisim.DegerAl(r, 1, 0);
            //    string bulunanSurecNo = VeriErisim.DegerAl(r, 2, "");
            //    r.Close();

            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = tcKimlikNo + " T.C. Kimlik Numaralı personelin " + yil + "/" + ay + " dönemine ait " + surecYil + "/" + bulunanSurecNo + " sürecinde fazla mesai kaydı bulunmaktadır. Bir ayda bir personele ait yalnızca bir fazla mesai hazırlanabilir.";
            //    return sonuc;
            //}
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Sil(VeriErisim ve, Kullanici kullanici, FazlaMesai f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            //hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            bool veriErisimGeldi = false;
            if (ve == null)
                ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));
            else
                veriErisimGeldi = true;


            ve.BeginTransaction();

            TABFazlaMesaiPersonel tab = new TABFazlaMesaiPersonel(ve);

            sonuc = tab.Sil(f);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
            }
            else
                ve.Rollback();

            if (!veriErisimGeldi)
                ve.VeriErisimKapat();

            return sonuc;
        }

        private string KontrolUstBilgi(FazlaMesai f)
        {
            string hata = "";
            string ad = "";

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
                ad = "bURAYI DEĞİŞTİR ";// OMKServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe);
                }

                ad = "BURAIDA";// OMKServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim);
                }
            }

            return hata;
        }

        private string Kontrol(FazlaMesai f)
        {
            string hata = "";
            string ad = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            int satir = 0;
            foreach (FazlaMesaiPersonel fp in f.personel)
            {
                satir++;
                if (fp.tcKimlikNo.Length != 11)
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00080, satir, fp.tcKimlikNo);
                }
                else
                {
                    ad = HRCServis.UzayYaziDegerGetir("PERPERSONEL", fp.tcKimlikNo, true, "");
                    if (ad == "")
                    {
                        if (hata != "") hata += "<br>";
                        hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00081, satir, fp.tcKimlikNo);
                    }
                }
            }

            return hata;
        }
    }
}
