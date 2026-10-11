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
    class HRCCesitliOdemeBordro
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

            CesitliOdeme ilkKayit = (CesitliOdeme)coler[0];
            sonuc = Kontrol(ilkKayit, parcaKod, kullanan);

            if (!sonuc.islemSonuc)
                return sonuc;

            //Seyyar görev için Çeşitli ödeme kullanıldığında vize cetveli aşımı için, toplam alma eklendi.
            int isciToplam = 0;
            int memurToplam = 0;
            int sozlesmeliToplam = 0;
            int digerToplam = 0;
            for (int i = 0; i < coler.ObjeSayisi; i++)
            {
                CesitliOdeme oi = (CesitliOdeme)coler[i];

                if (oi.odemeTur == 4 || oi.odemeTur == 5)
                    isciToplam += Convert.ToInt32(oi.carpan);
                else if (oi.odemeTur == 2)
                    sozlesmeliToplam += Convert.ToInt32(oi.carpan);
                else if (oi.odemeTur == 1)
                    memurToplam += Convert.ToInt32(oi.carpan);
                else
                    digerToplam += Convert.ToInt32(oi.carpan);

                for (int j = i + 1; j < coler.ObjeSayisi; j++)
                {
                    CesitliOdeme oj = (CesitliOdeme)coler[j];
                    if (oi.tcKimlikNo == oj.tcKimlikNo)
                    {
                        sonuc.DegerAta(string.Format("{0} satırındaki {2} numaralı tckimlik no {1} satırındaki {3} tc kimlik numarası ile aynı", i, j, oi.tcKimlikNo, oj.tcKimlikNo));
                        return sonuc;
                    }
                }
            }

            if (digerToplam > 0 && islemKod == TNS.HRC.Arac.SEYYARGOREVISLEM)
            {
                sonuc.DegerAta(string.Format("Personel türü (memur, işçi, sözleşmeli) belirlenemeyen kişilere {0} gün girilmiştir. Bu kişilerin türlerini düzeltiniz!", digerToplam));
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            if (islemKod == TNS.HRC.Arac.SEYYARGOREVISLEM) //Eğer seyyar görev ise vize aşılıyor mu kontrolü.
            {
                SeyyarGorevDurumAna sgda = SeyyarGorevC.SeyyarGorevDurumBul(ve, kullanan, ilkKayit.hakedisYil, ilkKayit.muhasebeKod, ilkKayit.birimKod, "", islemKod, ilkKayit.surecYil, ilkKayit.surecNo, "");
                SeyyarGorevDurum sgd;
                if (memurToplam > 0)
                {
                    bool hata = false;
                    sgd = sgda.MemurBul();
                    if (sgd == null)
                        hata = true;
                    else
                        hata = sgd.AsiyorMu(memurToplam);
                    if (hata)
                        sonuc.DegerAta("Girdiğiniz memur toplamı ile vizeli gün sayınızı aşıyorsunuz!");
                }
                if (isciToplam > 0)
                {
                    bool hata = false;
                    sgd = sgda.IsciBul();
                    if (sgd == null)
                        hata = true;
                    else
                        hata = sgd.AsiyorMu(isciToplam);
                    if (hata)
                        sonuc.DegerAta("Girdiğiniz işçi toplamı ile vizeli gün sayınızı aşıyorsunuz!");
                }
                if (sozlesmeliToplam > 0)
                {
                    bool hata = false;
                    sgd = sgda.SozlesmeliBul();
                    if (sgd == null)
                        hata = true;
                    else
                        hata = sgd.AsiyorMu(sozlesmeliToplam);
                    if (hata)
                        sonuc.DegerAta("Girdiğiniz sözleşmeli toplamı ile vizeli gün sayınızı aşıyorsunuz!");
                }
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABCesitliOdemeBordro tabCOB = new TABCesitliOdemeBordro(ve);

            ve.BeginTransaction();

            sonuc = tabCOB.Sil((CesitliOdeme)coler.objeler[0]);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019;
                return sonuc;
            }

            foreach (CesitliOdeme c in coler.objeler)
            {
                sonuc = tabCOB.Kaydet(c);

                if (!sonuc.islemSonuc)
                {
                    sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00011 + "(" + c.tcKimlikNo + ")" + ": " + ve.hata);
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //Süreç devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.islemSonuc)
                sonuc = new HRCSurec().SurecDurumKaydet(ve, kullanan, ((CesitliOdeme)coler[0]).surecYil, ((CesitliOdeme)coler[0]).surecNo, ENUMSurecDurum.DEVAMEDIYOR);
            ///////////////////////////////////////////////////////////////////////////

            if (sonuc.islemSonuc)
                sonuc = new HRCSurec().ParcaDurumKaydet(ve, kullanan, ((CesitliOdeme)coler[0]).surecYil, ((CesitliOdeme)coler[0]).surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011;
                return sonuc;
            }

            sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;

            ve.Commit();
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemiYapan">The kullanan.</param>
        /// <param name="co">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        public static ObjectArray Listele(Kullanici islemiYapan, CesitliOdeme co)
        {
            ObjectArray coler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemiYapan));

            TABCesitliOdemeBordro tabCOB = new TABCesitliOdemeBordro(ve);

            IDataReader dr = tabCOB.Listele(co);
            if (dr != null)
            {
                while (dr.Read())
                {
                    CesitliOdeme coGelen = new CesitliOdeme();

                    coGelen.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                    coGelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, 0);
                    coGelen.birimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    coGelen.surecNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                    coGelen.tcKimlikNo = VeriErisim.DegerAl(dr, 4, string.Empty);
                    coGelen.hakedisYil = VeriErisim.DegerAl(dr, 5, 0);
                    coGelen.hakedisAy = VeriErisim.DegerAl(dr, 6, 0);

                    coGelen.odemeTur = VeriErisim.DegerAl(dr, 7, 0);

                    coGelen.siraNo = VeriErisim.DegerAl(dr, 8, 0);

                    coGelen.sabitTutar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                    coGelen.carpan = VeriErisim.DegerAl(dr, 10, (decimal)0);
                    coGelen.ekTutar = VeriErisim.DegerAl(dr, 11, (decimal)0);
                    //coGelen.toplamTutar = VeriErisim.DegerAl(dr, 12, (decimal)0);

                    coGelen.gelirVergi = VeriErisim.DegerAl(dr, 13, (decimal)0);
                    coGelen.damgaVergi = VeriErisim.DegerAl(dr, 14, (decimal)0);
                    coGelen.digerVergi = VeriErisim.DegerAl(dr, 15, (decimal)0);
                    //coGelen.toplamVergi = VeriErisim.DegerAl(dr, 16, (decimal)0);
                    //coGelen.odenecekTutar = VeriErisim.DegerAl(dr, 17, (decimal)0);

                    coGelen.aciklama = VeriErisim.DegerAl(dr, 18, string.Empty);
                    coGelen.odemeEmriNo = VeriErisim.DegerAl(dr, 19, string.Empty);
                    coGelen.paraBirimi = VeriErisim.DegerAl(dr, 20, 0);
                    coGelen.avansTutar = VeriErisim.DegerAl(dr, 21, (decimal)0);
                    coGelen.avansOdemeEmriNo = VeriErisim.DegerAl(dr, 22, string.Empty);

                    coGelen.blGun = VeriErisim.DegerAl(dr, 23);
                    coGelen.blSiraNo = VeriErisim.DegerAl(dr, 24, (int)0);
                    coGelen.avansOdemeGun = VeriErisim.DegerAl(dr, 25);
                    coGelen.avansOdemeSiraNo = VeriErisim.DegerAl(dr, 26, (int)0);
                    coler.objeler.Add(coGelen);
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            TNS.MUH.IMUHServis servisMuh = TNS.MUH.Arac.Tanimla();
            foreach (CesitliOdeme coGelen in coler.objeler)
            {
                MUH.IlgiliBilgi ib = servisMuh.IlgiliBilgiBul(islemiYapan, coGelen.tcKimlikNo, co.muhasebeKod);
                if (ib != null)
                {
                    coGelen.adSoyad = ib.ilgili;
                    coGelen.unvan = "";
                    coGelen.ibanNo = ib.ilgIBAN;
                }
                if (string.IsNullOrWhiteSpace(coGelen.adSoyad))
                {
                    dr = tabCOB.Listele1(coGelen.tcKimlikNo);

                    if (dr != null)
                    {
                        if (dr.Read())
                        {
                            coGelen.adSoyad = VeriErisim.DegerAl(dr, 1, string.Empty);
                            coGelen.unvan = VeriErisim.DegerAl(dr, 2, string.Empty);
                            coGelen.ibanNo = VeriErisim.DegerAl(dr, 5, string.Empty);
                        }
                        dr.Close();
                    }
                }
            }

            ve.VeriErisimKapat();
            return coler;
        }

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABCesitliOdemeBordro to = new TABCesitliOdemeBordro(ve);

            ObjectArray coler = Listele(kullanan, bilgi);
            ve.BeginTransaction();
            foreach (CesitliOdeme codeme in coler.objeler)
            {
                codeme.odemeEmriNo = bilgi.odemeEmriNo;
                sonuc = to.OdemeEmriNoKaydet(codeme);
                if (!sonuc.islemSonuc) break;
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


        private static Sonuc Kontrol(CesitliOdeme co, int parcaKod, Kullanici kullanan)
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

        public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABCesitliOdemeBordro toCesitliOdemeBordro = new TABCesitliOdemeBordro(ve);

            IDataReader dr = toCesitliOdemeBordro.ListeleOdemeEmri(yil, surecNo);

            while (dr.Read())
            {
                CesitliOdeme sd = new CesitliOdeme();
                sd.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 1, "");

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00071;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        internal Sonuc CesitliOdemeBankaListesiKaydet(Kullanici kullanan, CesitliOdeme bilgi, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABCesitliOdemeBordro to = new TABCesitliOdemeBordro(ve);
            ve.BeginTransaction();

            ObjectArray coler = Listele(kullanan, bilgi);
            foreach (CesitliOdeme codeme in coler.objeler)
            {
                codeme.odemeEmriNo = bilgi.odemeEmriNo;
                sonuc = to.BankaListesiKaydet(codeme, bankaOdemeGun, bankaOdemeSiraNo);
                if (!sonuc.islemSonuc) break;
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

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AvansOdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABCesitliOdemeBordro to = new TABCesitliOdemeBordro(ve);
            ve.BeginTransaction();

            ObjectArray coler = Listele(kullanan, bilgi);
            foreach (CesitliOdeme codeme in coler.objeler)
            {
                codeme.avansOdemeEmriNo = bilgi.avansOdemeEmriNo;
                sonuc = to.AvansOdemeEmriNoKaydet(codeme);
                //if (sonuc.islemSonuc) break;
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


        internal Sonuc CesitliOdemeAvansBankaListesiKaydet(Kullanici kullanan, CesitliOdeme co, TNSDateTime avansBankaOdemeGun, int avansBankaOdemeSiraNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABCesitliOdemeBordro to = new TABCesitliOdemeBordro(ve);
            ve.BeginTransaction();

            ObjectArray coler = Listele(kullanan, co);
            foreach (CesitliOdeme codeme in coler.objeler)
            {
                codeme.avansOdemeEmriNo = co.avansOdemeEmriNo;
                sonuc = to.AvansBankaListesiKaydet(codeme, avansBankaOdemeGun, avansBankaOdemeSiraNo);
                if (!sonuc.islemSonuc) break;
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
    }
}
