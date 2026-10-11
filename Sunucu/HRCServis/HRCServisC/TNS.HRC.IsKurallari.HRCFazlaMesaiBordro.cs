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
    /// Fazla mesai bordro bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCFazlaMesaiBordro
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro bilgisini veritabanından okur 
        /// ve FazlaMesaiBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bordro bilgisini FazlaMesaiBordrolar nesnesi olarak tutan nesne</returns>
        public FazlaMesaiBordrolar Listele(Kullanici islemiYapan, FazlaMesaiBordro f, bool bankaListesi)
        {
            FazlaMesaiBordrolar fler = new FazlaMesaiBordrolar();
            string hata = "";

            hata = Kontrol(f);
            if (hata != "")
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(islemiYapan, f.muhasebe, f.birim))
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }
            */

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABFazlaMesaiBordro tab = new TABFazlaMesaiBordro(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(f, bankaListesi);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                fler.sonuc.islemSonuc = false;
                fler.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fler;
            }

            while (r.Read())
            {
                FazlaMesaiBordro fb = new FazlaMesaiBordro();

                fb.yil = VeriErisim.DegerAl(r, 0, 0);
                fb.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                fb.birim = VeriErisim.DegerAl(r, 2, "");
                fb.surecNo = VeriErisim.DegerAl(r, 3, "");
                fb.ilgiliYil = VeriErisim.DegerAl(r, 4, 0);
                fb.ilgiliAy = VeriErisim.DegerAl(r, 5, 0);
                fb.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");
                fb.haftaIciSaat = VeriErisim.DegerAl(r, 7, 0.0);
                fb.haftaSonuSaat = VeriErisim.DegerAl(r, 8, 0.0);
                fb.haftaIciUcret = VeriErisim.DegerAl(r, 9, 0.0);
                fb.haftaSonuUcret = VeriErisim.DegerAl(r, 10, 0.0);
                fb.damgaVergisi = VeriErisim.DegerAl(r, 11, 0.0);
                fb.kesinti = VeriErisim.DegerAl(r, 12, 0.0);
                fb.gelirVergisi = VeriErisim.DegerAl(r, 13, 0.0);
                fb.gelirVergisiOran = VeriErisim.DegerAl(r, 14, 0.0);
                fb.sgkIsveren = VeriErisim.DegerAl(r, 15, 0.0);
                fb.sgkIsci = VeriErisim.DegerAl(r, 16, 0.0);
                fb.adSoyad = VeriErisim.DegerAl(r, 18, "");
                fb.unvan = VeriErisim.DegerAl(r, 19, "");
                fb.banka = VeriErisim.DegerAl(r, 20, "");
                fb.sube = VeriErisim.DegerAl(r, 21, "");
                fb.hesapNo = VeriErisim.DegerAl(r, 22, "");
                fb.istihdamSekli = VeriErisim.DegerAl(r, 25, 0);

                fler.liste.Add(fb);
            }
            r.Close();

            fler.sonuc.islemSonuc = true;

            if (fler.liste.Count == 0)
                fler.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fler;
        }

        public Sonuc Kaydet(Kullanici kullanan, FazlaMesaiBordrolar f)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            hata = Kontrol((FazlaMesaiBordro)f.liste[0]);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, ((FazlaMesaiBordro)f.liste[0]).muhasebe, ((FazlaMesaiBordro)f.liste[0]).birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABFazlaMesaiBordro tab = new TABFazlaMesaiBordro(ve);

            sonuc = tab.Sil((FazlaMesaiBordro)f.liste[0]);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (FazlaMesaiBordro fp in f.liste)
            {
                sonuc = tab.Kaydet(fp);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KesintiKaydet(Kullanici kullanici, FazlaMesaiBordrolar f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (f.liste.Count == 0)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00066;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            hata = Kontrol((FazlaMesaiBordro)f.liste[0]);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, ((FazlaMesaiBordro)f.liste[0]).muhasebe, ((FazlaMesaiBordro)f.liste[0]).birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABFazlaMesaiBordro tab = new TABFazlaMesaiBordro(ve);

            foreach (FazlaMesaiBordro fp in f.liste)
            {
                sonuc = tab.KesintiKaydet(fp);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, FazlaMesaiBordro f, int islemKod, int parcaKod)
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
            if (!OMKYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABFazlaMesaiBordro tab = new TABFazlaMesaiBordro(ve);

            sonuc = tab.Sil(f);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string Kontrol(FazlaMesaiBordro f)
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
    }
}
