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
    class HRCFazlaMesaiBordroUnvan
    {
        public ObjectArray Listele(Kullanici islemiYapan, FazlaMesaiBordroUnvan f, bool bankaListesi)
        {
            ObjectArray liste = new ObjectArray();

            //hata = Kontrol(f);
            //if (hata != "")
            //{
            //    fler.sonuc.hataStr = hata;
            //    fler.sonuc.islemSonuc = false;
            //    return fler;
            //}
            /*
            if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(islemiYapan, f.muhasebe, f.birim))
            {
                fler.sonuc.hataStr = hata;
                fler.sonuc.islemSonuc = false;
                return fler;
            }
            */

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABFazlaMesaiBordroUnvan tab = new TABFazlaMesaiBordroUnvan(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(f, bankaListesi);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                //return fler;
            }

            while (r.Read())
            {
                FazlaMesaiBordroUnvan fb = new FazlaMesaiBordroUnvan();

                fb.yil = VeriErisim.DegerAl(r, 0, 0);
                fb.surecNo = VeriErisim.DegerAl(r, 1, string.Empty);
                fb.ilgiliYil = VeriErisim.DegerAl(r, 2, 0);
                fb.ilgiliAy = VeriErisim.DegerAl(r, 3, 0);
                fb.tcKimlikNo = VeriErisim.DegerAl(r, 4, string.Empty);
                fb.unvanKod = VeriErisim.DegerAl(r, 5, 0);
                fb.toplamSaat = VeriErisim.DegerAl(r, 6, 0.0);
                fb.kanuniIzin = VeriErisim.DegerAl(r, 7, 0.0);
                fb.yurtDisiGorev = VeriErisim.DegerAl(r, 8, 0.0);
                fb.resmiDiniGun = VeriErisim.DegerAl(r, 9, 0.0);
                fb.kesilenSaat = VeriErisim.DegerAl(r, 10, 0.0);
                fb.toplamKesilenSaat = VeriErisim.DegerAl(r, 11, 0.0);
                fb.hesaplanacakSaat = VeriErisim.DegerAl(r, 12, 0.0);
                fb.saatUcreti = VeriErisim.DegerAl(r, 13, 0.0);
                fb.toplamTutar = VeriErisim.DegerAl(r, 14, 0.0);
                fb.fazlaMesaiSaat = VeriErisim.DegerAl(r, 15, 0.0);
                fb.genelToplamSaat = VeriErisim.DegerAl(r, 16, 0.0);
                fb.ad = VeriErisim.DegerAl(r, 17, string.Empty);
                fb.soyad = VeriErisim.DegerAl(r, 18, string.Empty);
                fb.unvanAd = VeriErisim.DegerAl(r, 19, string.Empty);
                fb.damgaVergisi = VeriErisim.DegerAl(r, 20, 0.0);
                fb.sgk195 = VeriErisim.DegerAl(r, 21, 0.0);
                fb.sgk14 = VeriErisim.DegerAl(r, 22, 0.0);
                fb.kesinti = VeriErisim.DegerAl(r, 23, 0.0);
                fb.kesintiToplam = VeriErisim.DegerAl(r, 24, 0.0);
                fb.netOdenecek = VeriErisim.DegerAl(r, 25, 0.0);

                liste.objeler.Add(fb);
            }

            liste.sonuc.islemSonuc = true;

            if (liste.ObjeSayisi == 0)
                liste.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            r.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //string hata = "";
            //hata = Kontrol((FazlaMesaiBordro)f.liste[0]);
            //if (hata != "")
            //{
            //    sonuc.hataStr = hata;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}
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

            TABFazlaMesaiBordroUnvan tab = new TABFazlaMesaiBordroUnvan(ve);

            sonuc = tab.Sil((FazlaMesaiBordroUnvan)liste.objeler[0]);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (FazlaMesaiBordroUnvan fpu in liste.objeler)
            {
                fpu.fazlaMesaiSaat = PersonelFazlaMesaiSaatGetir(ve, fpu);
                fpu.toplamKesilenSaat = Math.Round((fpu.kanuniIzin + fpu.yurtDisiGorev + fpu.resmiDiniGun) * fpu.kesilenSaat, 2);
                if (fpu.toplamKesilenSaat < 0)
                    fpu.toplamKesilenSaat = 0;
                fpu.hesaplanacakSaat = fpu.toplamSaat - fpu.toplamKesilenSaat;
                fpu.genelToplamSaat = fpu.hesaplanacakSaat + fpu.fazlaMesaiSaat;
                fpu.toplamTutar = Math.Round(fpu.genelToplamSaat * fpu.saatUcreti, 2);
                fpu.damgaVergisi = Math.Round(fpu.toplamTutar * 0.00759, 2);
                if (fpu.sozlesmeliMi)
                {
                    fpu.sgk195 = Math.Round(fpu.toplamTutar * 0.195, 2);
                    fpu.sgk14 = Math.Round(fpu.toplamTutar * 0.14, 2);
                }
                fpu.kesintiToplam = fpu.damgaVergisi + fpu.sgk195 + fpu.sgk14 + fpu.kesinti;
                fpu.netOdenecek = fpu.toplamTutar - fpu.kesintiToplam;

                sonuc = tab.Kaydet(fpu);

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

        private static double PersonelFazlaMesaiSaatGetir(VeriErisim ve, FazlaMesaiBordroUnvan fpu)
        {
            TABFazlaMesai to = new TABFazlaMesai(ve);
            IDataReader r = to.ListelePersonelFazlaMesai(fpu.yil, fpu.surecNo, fpu.tcKimlikNo);

            double toplamFazlaMesaiSaat = 0.0;
            while (r.Read())
            {
                toplamFazlaMesaiSaat += VeriErisim.DegerAl(r, 8, 0.0);
            }
            r.Close();
            return toplamFazlaMesaiSaat;
        }

        public Sonuc Sil(Kullanici kullanici, FazlaMesaiBordroUnvan f, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            //hata = Kontrol(f);
            //if (hata != "")
            //{
            //    sonuc.hataStr = hata;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}
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

            TABFazlaMesaiBordroUnvan tab = new TABFazlaMesaiBordroUnvan(ve);

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

    }
}
