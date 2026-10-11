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
    /// Ek ders düzenlenen personel bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCEkDersPersonel
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders düzenlenen personel bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islem yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders düzenlenen personel bilgisini EkDers nesnesi olarak tutan nesne</returns>
        public EkDers Listele(Kullanici kullanici, EkDers f)
        {
            EkDers fm = new EkDers();
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

            TABEkDersPersonel tab = new TABEkDersPersonel(ve);

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

                EkDersPersonel fp = new EkDersPersonel();
                fp.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");
                fp.puantaj.kod = VeriErisim.DegerAl(r, 7, 0);
                fp.gelirVergisiOrani = VeriErisim.DegerAl(r, 8, 0);
                fp.adSoyad = VeriErisim.DegerAl(r, 9, "");
                if (fp.adSoyad == "")
                    fp.adSoyad = VeriErisim.DegerAl(r, 11, "");
                fp.puantaj.aciklama = VeriErisim.DegerAl(r, 10, "");

                fm.personel.Add(fp);
            }

            fm.sonuc.islemSonuc = true;

            if (fm.yil == 0)
                fm.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fm;
        }

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
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABEkDersPersonel tab = new TABEkDersPersonel(ve);

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
            pVAN.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            return pVAN;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, EkDers f, int islemKod, int parcaKod)
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

            TABEkDersPersonel tab = new TABEkDersPersonel(ve);

            sonuc = Sil(ve, kullanan, f, islemKod, parcaKod);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (EkDersPersonel fp in f.personel)
            {
                sonuc = tab.Kaydet(f, fp);
                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

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

        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Sil(VeriErisim ve, Kullanici kullanici, EkDers f, int islemKod, int parcaKod)
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

            TABEkDersPersonel tab = new TABEkDersPersonel(ve);

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

        private string KontrolUstBilgi(EkDers f)
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
                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe);
                }

                ad = "BURADA";// OMKServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim);
                }
            }

            return hata;
        }

        private string Kontrol(EkDers f)
        {
            string hata = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            //TABEkDersBordro tb = new TABEkDersBordro(ve);
            //EkDers oncekiKayit = tb.Varmi(f);
            //ve.VeriErisimKapat();

            //if (varMi > 0)
            //{
            //    hata = "Kayıt edilmek istenilen birime ait Ek Ders Bordrosu hazırlanmıştır. Bu nedenle kayıt işlemi <b>yapılamadı</b>. Eğer kayıt yapılması isteniyorsa <b>Bordro İptali</b> işlemi yapılması gereklidir.";
            //    return hata;
            //}

            int satir = 0;
            foreach (EkDersPersonel fp in f.personel)
            {
                satir++;
                if (fp.tcKimlikNo.Length != 11)
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00080, satir, fp.tcKimlikNo);
                }
                else
                {
                    //OgrenciEgitmen oe = new OgrenciEgitmen();
                    //oe.muhasebe = f.muhasebe;
                    //oe.birim = f.birim;
                    //oe.tcKimlikNo = fp.tcKimlikNo;
                    //oe.tip = 2;
                    //oe.durum = 1;

                    //ObjectArray egtBilgi = new HRCOgrenciEgitmen().Listele(null, oe);

                    PersonelBilgi perBilgi = new HRCPersonel().ListelePersonelOgrenciEgitmen(null, fp.tcKimlikNo, 0, "");

                    //if (egtBilgi.objeler.Count != 1)
                    if (perBilgi == null || perBilgi.tcKimlikNo == "")
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
