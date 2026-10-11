using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Fazla mesai bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCFazlaMesai
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        public FazlaMesai Listele(Kullanici kullanici, FazlaMesai f)
        {
            FazlaMesai fm = new FazlaMesai();
            string hata = "";

            //hata = Kontrol(f);
            if (hata != "")
            {
                fm.sonuc.hataStr = hata;
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            /*
            if (!((new TNS.HRC.IHRCServis()).OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim)))
            {
                fm.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABFazlaMesai tab = new TABFazlaMesai(ve);

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
            string sicilNo = "";
            string eskiSicilNo = "";
            FazlaMesaiPersonel fp = new FazlaMesaiPersonel();

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

                sicilNo = VeriErisim.DegerAl(r, 6, "");
                if (sicilNo != eskiSicilNo)
                {
                    fp = new FazlaMesaiPersonel();
                    fp.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");

                    if (fp.tcKimlikNo != "")
                        fm.personel.Add(fp);

                    fp.adSoyad = VeriErisim.DegerAl(r, 11, "");
                    fp.unvan = VeriErisim.DegerAl(r, 12, "");

                    fp.puantaj.yil = VeriErisim.DegerAl(r, 16, 0);
                    fp.puantaj.kod = VeriErisim.DegerAl(r, 17, 0);
                    fp.puantaj.aciklama = VeriErisim.DegerAl(r, 18, "");
                    fp.puantaj.aylikSaat = VeriErisim.DegerAl(r, 19, 0.0);
                    fp.puantaj.haftaIciSaat = VeriErisim.DegerAl(r, 20, 0.0);
                    fp.puantaj.haftaIciUcret = VeriErisim.DegerAl(r, 21, 0.0);
                    fp.puantaj.cumartesiSaat = VeriErisim.DegerAl(r, 22, 0.0);
                    fp.puantaj.cumartesiUcret = VeriErisim.DegerAl(r, 23, 0.0);
                    fp.puantaj.pazarSaat = VeriErisim.DegerAl(r, 24, 0.0);
                    fp.puantaj.pazarUcret = VeriErisim.DegerAl(r, 25, 0.0);
                    fp.puantaj.varsayilan = VeriErisim.DegerAl(r, 26, 0);


                }

                FazlaMesaiPersonelSaat fs = new FazlaMesaiPersonelSaat();
                fs.gun = VeriErisim.DegerAl(r, 7, 0);
                fs.saat = VeriErisim.DegerAl(r, 8, 0.0);
                fp.gunSaat.Add(fs);

                eskiSicilNo = sicilNo;
            }

            r.Close();
            //Sonradan Eklenen Personel İçin 

            TABFazlaMesai to2 = new TABFazlaMesai(ve);
            IDataReader r1 = null;
            try
            {
                r1 = to2.ListeleIcmalGirisiYapilmayanlar(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                fm.sonuc.islemSonuc = false;
                fm.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fm;
            }

            while (r1.Read())
            {
                fp = new FazlaMesaiPersonel();
                fp.tcKimlikNo = VeriErisim.DegerAl(r1, 0, "");
                fp.puantaj.yil = VeriErisim.DegerAl(r1, 2, 0);
                fp.puantaj.kod = VeriErisim.DegerAl(r1, 3, 0);
                fp.puantaj.aciklama = VeriErisim.DegerAl(r1, 4, "");
                fp.puantaj.aylikSaat = VeriErisim.DegerAl(r1, 5, 0.0);
                fp.puantaj.haftaIciSaat = VeriErisim.DegerAl(r1, 6, 0.0);
                fp.puantaj.haftaIciUcret = VeriErisim.DegerAl(r1, 7, 0.0);
                fp.puantaj.cumartesiSaat = VeriErisim.DegerAl(r1, 8, 0.0);
                fp.puantaj.cumartesiUcret = VeriErisim.DegerAl(r1, 9, 0.0);
                fp.puantaj.pazarSaat = VeriErisim.DegerAl(r1, 10, 0.0);
                fp.puantaj.pazarUcret = VeriErisim.DegerAl(r1, 11, 0.0);
                fp.puantaj.varsayilan = VeriErisim.DegerAl(r1, 12, 0);
                fp.adSoyad = VeriErisim.DegerAl(r1, 13, "");

                if (fp.tcKimlikNo != "")
                    fm.personel.Add(fp);
            }

            r1.Close();
            fm.sonuc.islemSonuc = true;

            if (fm.yil == 0)
                fm.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fm;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisi listesini FazlaMesai nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Sorgula(Kullanici kullanici, FazlaMesai f)
        {
            ObjectArray fm = new ObjectArray();
            //string hata = "";
            /*
                        if (!HRCYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
                        {
                            fm.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                            fm.sonuc.islemSonuc = false;
                            return fm;
                        }
                        */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABFazlaMesai tab = new TABFazlaMesai(ve);

            IDataReader r = null;

            try
            {
                r = tab.Sorgulama(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                fm.sonuc.islemSonuc = false;
                fm.sonuc.hataStr = e.Message;

                ve.VeriErisimKapat();

                return fm;
            }

            while (r.Read())
            {
                FazlaMesai fg = new FazlaMesai();
                fg.yil = VeriErisim.DegerAl(r, 0, 0);
                fg.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                fg.birim = VeriErisim.DegerAl(r, 2, "");
                fg.surecNo = VeriErisim.DegerAl(r, 3, "");
                fg.ilgiliYil = VeriErisim.DegerAl(r, 4, 0);
                fg.ilgiliAy = VeriErisim.DegerAl(r, 5, 0);

                fm.objeler.Add(fg);
            }

            fm.sonuc.islemSonuc = true;

            if (fm.objeler.Count == 0)
                fm.sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00072;

            ve.VeriErisimKapat();

            return fm;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
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

            Sonuc kayitSonuc = new Sonuc();

            string hata = "";
            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!HRCYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
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

            TABFazlaMesai tab = new TABFazlaMesai(ve);

            sonuc = tab.Sil(f);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            FazlaMesaiBordro bordro = new FazlaMesaiBordro();
            bordro.yil = f.yil;
            bordro.surecNo = f.surecNo;
            bordro.muhasebe = f.muhasebe;
            bordro.birim = f.birim;
            TABFazlaMesaiBordro tabBordro = new TABFazlaMesaiBordro(ve);

            Sonuc bordroSonuc = tabBordro.Sil(bordro);

            if (bordroSonuc.islemSonuc)
            {
                FazlaMesai fSorgu = new FazlaMesai();
                fSorgu.yil = f.yil;
                fSorgu.surecNo = f.surecNo;
                fSorgu.muhasebe = f.muhasebe;
                fSorgu.birim = f.birim;
                fSorgu.ilgiliYil = f.ilgiliYil;
                fSorgu.ilgiliAy = f.ilgiliAy;

                int siraNo = 0;
                foreach (FazlaMesaiPersonel fp in f.personel)
                {
                    hata = KontrolMesaiSaat(ve, fSorgu, fp);
                    if (hata != "")
                        sonuc.hataStr += hata;
                    else
                    {
                        kayitSonuc = tab.Kaydet(f, fp, siraNo);

                        if (!kayitSonuc.islemSonuc)
                        {
                            sonuc.hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00100, fp.tcKimlikNo);
                            break;
                        }
                        siraNo++;
                    }
                }

                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                if (sonuc.hataStr == "")
                    sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                if (sonuc.hataStr == "")
                {
                    ve.Commit();
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                    sonuc.islemSonuc = false;
                    ve.Rollback();
                }
                ve.VeriErisimKapat();

                return sonuc;
            }
            else
            {
                bordroSonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                ve.Rollback();
                ve.VeriErisimKapat();
                return bordroSonuc;
            }
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, FazlaMesai f, int islemKod, int parcaKod)
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
                        if (!HRCYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
                        {
                            sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }*/

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABFazlaMesaiBordro tb = new TABFazlaMesaiBordro(ve);
            if (tb.Varmi(f) > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00101;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            TABFazlaMesai tab = new TABFazlaMesai(ve);

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
            FazlaMesai fm = new FazlaMesai();
            fm.yil = f.yil;
            fm.muhasebe = f.muhasebe;
            fm.birim = f.birim;
            fm.surecNo = f.surecNo;

            return Kontrol(fm);
        }

        private string Kontrol(FazlaMesai f)
        {
            string hata = "";

            if (f.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";

            if (f.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";

            if (f.birim == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";

            if (f.surecNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe);

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim);
            }

            return hata;
        }

        private string KontrolMesaiSaat(VeriErisim ve, FazlaMesai f, FazlaMesaiPersonel fp)
        {
            string hata = "";

            FazlaMesaiPuantaj fpu = new FazlaMesaiPuantaj();
            TABFazlaMesaiPersonelPuantaj tpp = new TABFazlaMesaiPersonelPuantaj(ve);
            IDataReader r = tpp.Listele(f, fp);

            if (r.Read())
            {
                fpu.aylikSaat = VeriErisim.DegerAl(r, 3, 0.0);
                fpu.haftaIciSaat = VeriErisim.DegerAl(r, 4, 0.0);
                fpu.cumartesiSaat = VeriErisim.DegerAl(r, 6, 0.0);
                fpu.pazarSaat = VeriErisim.DegerAl(r, 8, 0.0);
            }
            else
                hata = "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00074, fp.tcKimlikNo) + "</li>";

            r.Close();

            if (hata == "")
            {
                //Aylık toplam saat kontrolü
                double saatToplam = 0;
                foreach (FazlaMesaiPersonelSaat fs in fp.gunSaat)
                    saatToplam += fs.saat;

                if (saatToplam > fpu.aylikSaat)
                    hata += "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00102, fp.tcKimlikNo, saatToplam, fpu.aylikSaat) + "</li>";

                //Günlük (Haftaiçi, cumartesi, pazar) saat kontrolü
                foreach (FazlaMesaiPersonelSaat fs in fp.gunSaat)
                {
                    DateTime d = new DateTime(f.ilgiliYil, f.ilgiliAy, fs.gun);

                    if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (fs.saat > fpu.pazarSaat) //fs.saat=fpu.pazarSaat;
                            hata += "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00103, fs.gun, fs.saat, fpu.pazarSaat) + "</li>";
                    }
                    else if (d.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (fs.saat > fpu.cumartesiSaat) //fs.saat=fpu.cumartesiSaat;
                            hata += "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00104, fs.gun, fs.saat, fpu.cumartesiSaat) + "</li>";
                    }
                    else
                    {
                        if (fs.saat > fpu.haftaIciSaat) //fs.saat=fpu.haftaIciSaat;
                            hata += "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00105, fs.gun, fs.saat, fpu.haftaIciSaat) + "</li>";
                    }
                }
            }

            if (hata != "")
            {
                if (hata != "")
                    hata += "<br>";
                hata = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00076, fp.tcKimlikNo, hata);
            }
            return hata;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisi için gerekli hesaplamaları yapar, fazla mesai bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BordroHazirla(Kullanici kullanici, FazlaMesai f, int islemKod, int parcaKod)
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
            if (!HRCYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
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
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanici, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            HRCFazlaMesai hrcF = new HRCFazlaMesai();
            f = hrcF.Listele(kullanici, f);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABFazlaMesaiBordro tab = new TABFazlaMesaiBordro(ve);

            FazlaMesaiBordro fbSil = new FazlaMesaiBordro();
            fbSil.yil = f.yil;
            fbSil.muhasebe = f.muhasebe;
            fbSil.birim = f.birim;
            fbSil.surecNo = f.surecNo;

            sonuc = tab.Sil(fbSil);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            foreach (FazlaMesaiPersonel fp in f.personel)
            {
                double haftaIciSaat = 0;
                double cumartesiSaat = 0;
                double pazarSaat = 0;

                FazlaMesaiBordro fb = new FazlaMesaiBordro();
                fb.yil = f.yil;
                fb.muhasebe = f.muhasebe;
                fb.birim = f.birim;
                fb.surecNo = f.surecNo;
                fb.ilgiliYil = f.ilgiliYil;
                fb.ilgiliAy = f.ilgiliAy;
                fb.tcKimlikNo = fp.tcKimlikNo;

                FazlaMesaiPuantaj fpu = new FazlaMesaiPuantaj();
                TABFazlaMesaiPersonelPuantaj tpp = new TABFazlaMesaiPersonelPuantaj(ve);
                IDataReader r = tpp.Listele(f, fp);
                if (r.Read())
                {
                    fpu.haftaIciUcret = VeriErisim.DegerAl(r, 5, 0.0);
                    fpu.cumartesiUcret = VeriErisim.DegerAl(r, 7, 0.0);
                    fpu.pazarUcret = VeriErisim.DegerAl(r, 9, 0.0);

                    //KmL
                    fp.gelirVergisiOrani = VeriErisim.DegerAl(r, 14, 0);
                }
                r.Close();

                foreach (FazlaMesaiPersonelSaat fs in fp.gunSaat)
                {
                    DateTime d = new DateTime(f.ilgiliYil, f.ilgiliAy, fs.gun);

                    if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        pazarSaat += fs.saat;
                    }
                    else if (d.DayOfWeek == DayOfWeek.Saturday)
                        cumartesiSaat += fs.saat;
                    else
                        haftaIciSaat += fs.saat;
                }

                fb.haftaIciSaat = haftaIciSaat;
                fb.haftaIciUcret = OrtakFonksiyonlar.ConvertToDouble((haftaIciSaat * fpu.haftaIciUcret).ToString("#.##"), 0);
                fb.haftaSonuSaat = cumartesiSaat + pazarSaat;
                fb.haftaSonuUcret = OrtakFonksiyonlar.ConvertToDouble(((cumartesiSaat * fpu.cumartesiUcret) + (pazarSaat * fpu.pazarUcret)).ToString("#.##"), 0);
                fb.damgaVergisi = OrtakFonksiyonlar.ConvertToDouble((((fb.haftaIciUcret + fb.haftaSonuUcret) * .759) / 100).ToString("#.##"), 0);
                fb.gelirVergisi = OrtakFonksiyonlar.ConvertToDouble(((fb.haftaSonuUcret + fb.haftaIciUcret) * (Double)(fp.gelirVergisiOrani / 100)).ToString("#.##"), 0);
                fb.gelirVergisiOran = OrtakFonksiyonlar.ConvertToDbl(fp.gelirVergisiOrani);
                fb.kesinti = 0;

                if (fb.haftaIciUcret + fb.haftaSonuUcret > 0)
                {
                    sonuc = tab.Kaydet(fb);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            //Parca Adım devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr != "")
                sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanici, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanici.kullaniciKodu);
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00077;
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00078 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoKaydet(Kullanici kullanici, FazlaMesaiBordro f, string belgeNo)
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
            if (!HRCYetkiServis.OdemeEmriYazmaYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABFazlaMesai tab = new TABFazlaMesai(ve);

            sonuc = tab.MIFNoKaydet(f, belgeNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011+"<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoListele(Kullanici kullanici, FazlaMesaiBordro f)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = false;

            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            /*
            if (!HRCYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABFazlaMesai tab = new TABFazlaMesai(ve);

            IDataReader r = null;

            try
            {
                r = tab.MIFNoListele(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
                r.Close();
                ve.VeriErisimKapat();

                return sonuc;
            }

            if (r.Read())
            {
                sonuc.anahtar = VeriErisim.DegerAl(r, 4, "");
                sonuc.islemSonuc = true;
            }
            r.Close();
            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
