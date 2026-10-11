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
    /// Ek ders bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCEkDers
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders bilgisini EkDers nesnesi olarak tutan nesne</returns>
        public EkDers Listele(Kullanici kullanici, EkDers f)
        {
            EkDers fm = new EkDers();
            string hata = "";

            int geceMi = f.gecemi;

            //hata = Kontrol(f);
            if (hata != "")
            {
                fm.sonuc.hataStr = hata;
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            /*
            if (!((new TNS.OMK.IOMKServis()).OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim)))
            {
                fm.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                fm.sonuc.islemSonuc = false;
                return fm;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABEkDers tab = new TABEkDers(ve);

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
            EkDersPersonel fp = new EkDersPersonel();

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
                    fm.gecemi = geceMi;

                    ilk = false;
                }

                sicilNo = VeriErisim.DegerAl(r, 6, "");
                if (sicilNo != eskiSicilNo)
                {
                    fp = new EkDersPersonel();
                    fp.tcKimlikNo = VeriErisim.DegerAl(r, 6, "");

                    if (fp.tcKimlikNo != "")
                        fm.personel.Add(fp);
                    //fp.unvan = VeriErisim.DegerAl(r, 12, "");
                    fp.puantaj.yil = VeriErisim.DegerAl(r, 10, 0);
                    fp.puantaj.ay = VeriErisim.DegerAl(r, 11, 0);
                    fp.puantaj.kod = VeriErisim.DegerAl(r, 12, 0);
                    fp.puantaj.aciklama = VeriErisim.DegerAl(r, 13, "");
                    fp.puantaj.yillikSaat = VeriErisim.DegerAl(r, 14, 0.0);
                    fp.puantaj.haftalikSaat = VeriErisim.DegerAl(r, 15, 0.0);
                    fp.puantaj.haftaiciUcret = VeriErisim.DegerAl(r, 16, 0.0);
                    fp.puantaj.haftasonuUcret = VeriErisim.DegerAl(r, 17, 0.0);
                    fp.puantaj.varsayilan = VeriErisim.DegerAl(r, 18, 0);
                    fp.puantaj.durum = VeriErisim.DegerAl(r, 19, 0);

                    fp.adSoyad = VeriErisim.DegerAl(r, 20, "");
                    if (string.IsNullOrEmpty(fp.adSoyad))
                        fp.adSoyad = VeriErisim.DegerAl(r, 21, "");

                    fp.unvan = VeriErisim.DegerAl(r, 22, "");
                    if (string.IsNullOrEmpty(fp.unvan))
                        fp.unvan = VeriErisim.DegerAl(r, 23, "");
                }

                EkDersPersonelSaat fs = new EkDersPersonelSaat();
                fs.gun = VeriErisim.DegerAl(r, 7, 0);
                fs.saat = VeriErisim.DegerAl(r, 8, 0.0);
                fp.gunSaat.Add(fs);

                eskiSicilNo = sicilNo;

            }
            r.Close();
            //Sonradan Eklenen Personel İçin 

            TABEkDers to2 = new TABEkDers(ve);
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
                fp = new EkDersPersonel();
                fp.tcKimlikNo = VeriErisim.DegerAl(r1, 0, "");
                fp.puantaj.yil = VeriErisim.DegerAl(r1, 2, 0);
                fp.puantaj.ay = VeriErisim.DegerAl(r1, 3, 0);
                fp.puantaj.kod = VeriErisim.DegerAl(r1, 4, 0);
                fp.puantaj.aciklama = VeriErisim.DegerAl(r1, 5, "");
                fp.puantaj.yillikSaat = VeriErisim.DegerAl(r1, 6, 0.0);
                fp.puantaj.haftalikSaat = VeriErisim.DegerAl(r1, 7, 0.0);
                fp.puantaj.haftaiciUcret = VeriErisim.DegerAl(r1, 8, 0.0);
                fp.puantaj.haftasonuUcret = VeriErisim.DegerAl(r1, 9, 0.0);
                fp.puantaj.varsayilan = VeriErisim.DegerAl(r1, 10, 0);
                fp.puantaj.durum = VeriErisim.DegerAl(r1, 11, 0);
                fp.adSoyad = VeriErisim.DegerAl(r1, 12, "");

                if (string.IsNullOrEmpty(fp.adSoyad))
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders listesini EkDers nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Sorgula(Kullanici kullanici, EkDers f)
        {
            ObjectArray fm = new ObjectArray();
            //string hata = "";
            /*
                        if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
                        {
                            fm.sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                            fm.sonuc.islemSonuc = false;
                            return fm;
                        }
                        */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABEkDers tab = new TABEkDers(ve);

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
                EkDers fg = new EkDers();
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
        /// Arguman olarak verilen, ek ders bilgisini veritabanına kaydeder 
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

            TABEkDers tab = new TABEkDers(ve);

            sonuc = tab.Sil(f);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }

            EkDersBordro bordro = new EkDersBordro();
            bordro.yil = f.yil;
            bordro.surecNo = f.surecNo;
            bordro.muhasebe = f.muhasebe;
            bordro.birim = f.birim;
            TABEkDersBordro tabBordro = new TABEkDersBordro(ve);

            Sonuc bordroSonuc = tabBordro.Sil(bordro);

            if (bordroSonuc.islemSonuc)
            {
                EkDers fSorgu = new EkDers();
                fSorgu.yil = f.yil;
                fSorgu.surecNo = f.surecNo;
                fSorgu.muhasebe = f.muhasebe;
                fSorgu.birim = f.birim;
                fSorgu.ilgiliYil = f.ilgiliYil;
                fSorgu.ilgiliAy = f.ilgiliAy;

                int siraNo = 0;
                foreach (EkDersPersonel fp in f.personel)
                {
                    hata = KontrolMesaiSaat(ve, fSorgu, fp);
                    if (hata != "")
                        sonuc.hataStr += hata;
                    else
                    {
                        kayitSonuc = tab.Kaydet(f, fp, siraNo);

                        if (!kayitSonuc.islemSonuc)
                            sonuc.hataStr += kayitSonuc.hataStr;
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
        /// Arguman olarak verilen, ek ders bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanici, EkDers f, int islemKod, int parcaKod)
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
                        }*/

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABEkDersBordro tb = new TABEkDersBordro(ve);
            if (tb.Varmi(f) > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00073;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            TABEkDers tab = new TABEkDers(ve);

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

        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisi için gerekli hesaplamaları yapar, ek ders bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BordroHazirla(Kullanici kullanici, EkDers f, int islemKod, int parcaKod)
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
            HRCEkDers omkF = new HRCEkDers();
            f = omkF.Listele(kullanici, f);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABEkDersBordro tab = new TABEkDersBordro(ve);

            EkDersBordro fbSil = new EkDersBordro();
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

            foreach (EkDersPersonel fp in f.personel)
            {
                double haftaIciSaat = 0;
                double haftaSonuSaat = 0;

                EkDersBordro fb = new EkDersBordro();
                fb.yil = f.yil;
                fb.muhasebe = f.muhasebe;
                fb.birim = f.birim;
                fb.surecNo = f.surecNo;
                fb.ilgiliYil = f.ilgiliYil;
                fb.ilgiliAy = f.ilgiliAy;
                fb.tcKimlikNo = fp.tcKimlikNo;

                EkDersPuantaj fpu = new EkDersPuantaj();
                TABEkDersPersonelPuantaj tpp = new TABEkDersPersonelPuantaj(ve);
                IDataReader r = tpp.Listele(f, fp);
                if (r.Read())
                {
                    fpu.haftaiciUcret = VeriErisim.DegerAl(r, 6, 0.0);
                    fpu.haftasonuUcret = VeriErisim.DegerAl(r, 7, 0.0);
                    fp.gelirVergisiOrani = VeriErisim.DegerAl(r, 10, 0);
                }
                r.Close();

                foreach (EkDersPersonelSaat fs in fp.gunSaat)
                {
                    DateTime d = new DateTime(f.ilgiliYil, f.ilgiliAy, fs.gun);

                    if (d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday)
                        haftaSonuSaat += fs.saat;
                    else
                        haftaIciSaat += fs.saat;
                }


                double haftaiciUcret = 0;
                double haftasonuUcret = 0;
                if (f.gecemi == (int)ENUMEvetHayir.EVET)
                {
                    haftaSonuSaat = haftaSonuSaat + haftaIciSaat;
                    haftaIciSaat = 0;

                    haftaiciUcret = haftaIciSaat * fpu.haftasonuUcret;
                    haftasonuUcret = haftaSonuSaat * fpu.haftasonuUcret;

                    fb.haftaiciUcret = haftaiciUcret;
                    fb.haftasonuUcret = haftasonuUcret;
                }
                else
                {
                    haftaiciUcret = haftaIciSaat * fpu.haftaiciUcret;
                    haftasonuUcret = haftaSonuSaat * fpu.haftasonuUcret;
                }

                double damgaVergisi = (haftasonuUcret + haftaiciUcret) * .759 / 100; //HT:.66 / 100;
                double gelirVergisi = (haftasonuUcret + haftaiciUcret) * (Double)(fp.gelirVergisiOrani / 100);

                fb.haftaiciUcret = Math.Round(haftaiciUcret, 2);
                fb.haftasonuUcret = Math.Round(haftasonuUcret, 2);
                fb.haftaiciSaat = haftaIciSaat;
                fb.haftasonuSaat = haftaSonuSaat;
                fb.damgaVergisi = Math.Round(damgaVergisi, 2);
                fb.gelirVergisi = Math.Round(gelirVergisi, 2);
                fb.gelirVergisiOran = OrtakFonksiyonlar.ConvertToDbl(fp.gelirVergisiOrani);
                fb.kesinti = 0;

                if (haftaiciUcret + haftasonuUcret > 0)
                {
                    sonuc = tab.Kaydet(fb);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

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
        /// Arguman olarak verilen, ek ders bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoKaydet(Kullanici kullanici, EkDersBordro f, string belgeNo)
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

            TABEkDers tab = new TABEkDers(ve);

            sonuc = tab.MIFNoKaydet(f, belgeNo);

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

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoListele(Kullanici kullanici, EkDersBordro f)
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
            if (!OMKYetkiServis.OdemeEmriGormeYetkisiVarMi(kullanici, f.muhasebe, f.birim))
            {
                sonuc.hataStr = "Verilen muhasebe veya birim kodunda işlem yapma yetkiniz yok.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            */
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));

            TABEkDers tab = new TABEkDers(ve);

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

                ve.VeriErisimKapat();

                return sonuc;
            }

            if (r.Read())
            {
                sonuc.anahtar = VeriErisim.DegerAl(r, 4, "");
                sonuc.islemSonuc = true;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string Kontrol(EkDersBordro f)
        {
            EkDers fm = new EkDers();
            fm.yil = f.yil;
            fm.muhasebe = f.muhasebe;
            fm.birim = f.birim;
            fm.surecNo = f.surecNo;

            return Kontrol(fm);
        }

        private string Kontrol(EkDers f)
        {
            string hata = "";

            if (f.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;

            if (f.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;

            if (f.birim == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005;

            if (f.surecNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067;

            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00068, f.muhasebe) + "<br>";

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00069, f.birim) + "<br>";
            }

            return hata;
        }

        private string KontrolMesaiSaat(VeriErisim ve, EkDers f, EkDersPersonel fp)
        {
            string hata = "";

            EkDersPuantaj fpu = new EkDersPuantaj();
            TABEkDersPersonelPuantaj tpp = new TABEkDersPersonelPuantaj(ve);
            IDataReader r = tpp.Listele(f, fp);

            if (r.Read())
            {
                fpu.yillikSaat = VeriErisim.DegerAl(r, 4, 0.0);
                fpu.haftalikSaat = VeriErisim.DegerAl(r, 5, 0.0);
            }
            else
                hata = "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00074, fp.tcKimlikNo) + "</li>";

            r.Close();

            if (hata == "")
            {
                //haftalik toplam saat kontrolü

                int gunSira = 0;
                double[,] weeks = new double[6, 2];
                if (fp.gunSaat.Count > 0)
                {

                    DateTime d = new DateTime(f.yil, f.ilgiliAy, 1);
                    switch (d.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            gunSira = 1;
                            break;
                        case DayOfWeek.Tuesday:
                            gunSira = 2;
                            break;
                        case DayOfWeek.Wednesday:
                            gunSira = 3;
                            break;
                        case DayOfWeek.Thursday:
                            gunSira = 4;
                            break;
                        case DayOfWeek.Friday:
                            gunSira = 5;
                            break;
                        case DayOfWeek.Saturday:
                            gunSira = 6;
                            break;
                        case DayOfWeek.Sunday:
                            gunSira = 7;
                            break;
                    }
                }

                weeks[0, 0] = (7 - gunSira) + 2;
                weeks[1, 0] = weeks[0, 0] + 7;
                weeks[2, 0] = weeks[1, 0] + 7;
                weeks[3, 0] = weeks[2, 0] + 7;
                weeks[4, 0] = weeks[3, 0] + 7;
                weeks[5, 0] = weeks[4, 0] + 7;
                //int haftaSira = 1;
                foreach (EkDersPersonelSaat fs in fp.gunSaat)
                {

                    if (fs.gun < weeks[0, 0])
                        weeks[0, 1] += fs.saat;
                    else if (weeks[0, 0] <= fs.gun && fs.gun < weeks[1, 0])
                        weeks[1, 1] += fs.saat;
                    else if (weeks[1, 0] <= fs.gun && fs.gun < weeks[2, 0])
                        weeks[2, 1] += fs.saat;
                    else if (weeks[2, 0] <= fs.gun && fs.gun < weeks[3, 0])
                        weeks[3, 1] += fs.saat;
                    else if (weeks[3, 0] <= fs.gun && fs.gun < weeks[4, 0])
                        weeks[4, 1] += fs.saat;
                    else if (weeks[4, 0] <= fs.gun && fs.gun < weeks[5, 0])
                        weeks[5, 1] += fs.saat;
                }
                for (int i = 0; i < 6; i++)
                {

                    if (weeks[i, 1] > fpu.haftalikSaat)
                    {
                        hata += "<li>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00075, fp.tcKimlikNo, (i + 1), weeks[i, 1], fpu.haftalikSaat) + "</li>";

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
    }
}
