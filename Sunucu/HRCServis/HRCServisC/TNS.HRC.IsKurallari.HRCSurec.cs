using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using System.Collections;
using static TNS.HRC.BordroYapi;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Süreç bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCSurec
    {
        internal Sonuc SurecYetkisiVarMi(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, ENUMKullaniciYetkiTipi yetkiTipi)
        {
            return HRCYetkiKontrol.SurecYetkisiVarMi(veriErisim, kullanan, SurecOku(veriErisim, kullanan, surecYil, surecNo), yetkiTipi);
        }

        /// <summary>
        /// Arguman olarak verilen, süreç bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Surec bilgi, SurecParca spBilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            double kontrolMuhasebe = 0;
            string kontrolBirim = "";

            string hata = "";

            if (bilgi.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            else
            {
                kontrolMuhasebe = HRCServis.UzaySayiDegerGetir("MUHMUHASEBE", bilgi.muhasebe.ToString(), true, "MUHASEBEKOD");
                if (Convert.ToInt32(kontrolMuhasebe) != bilgi.muhasebe)
                    hata += "<br>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00234, bilgi.muhasebe);
            }

            if (bilgi.birim.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            else
            {
                kontrolBirim = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", bilgi.birim + "-" + bilgi.muhasebe.ToString(), false, "BIRIMKOD");
                if (kontrolBirim != bilgi.birim.Trim())
                    hata += "<br>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00235, bilgi.birim) + " kodlu harcama birimi bulunamadı.";
            }

            if (!HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00256 + "<br>";

            if (bilgi.surecTarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00257 + "<br>";

            if (bilgi.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";

            if (bilgi.islem.kod == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00258 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            bool numaraAldik = false;
            if (bilgi.surecNo == "")
            {
                numaraAldik = true;
                bilgi.surecDurum = (int)ENUMSurecDurum.YENI;

                //Belgeye numara alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = bilgi.yil.ToString();
                sayacKod = "HRCSUREC" + sayacKod;

                bilgi.surecNo = tg.BelgeNoAl(sayacKod, "000000");
                //**********************************************************

                //Eğer belgeno alındı ise aşağıda başına kurum kodu konuluyor
                //sonuc.Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                //**********************************************************
                if (OrtakFonksiyonlar.ConvertToInt(bilgi.surecNo, 0) == 0)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00259;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
                //**********************************************************
            }

            if (OrtakFonksiyonlar.ConvertToDouble(bilgi.surecNo, 0) > 0)
            {
                ve.BeginTransaction();
                TABSurec to = new TABSurec(ve);

                sonuc = to.Kaydet(bilgi);
                if (sonuc.islemSonuc && spBilgi != null)
                {
                    spBilgi.surecNo = bilgi.surecNo;
                    new HRCSurecParca().Kaydet(ve, kullanan, spBilgi);
                }

                if (sonuc.islemSonuc && numaraAldik)
                {
                    ParcaDurumKaydet(ve, kullanan, bilgi.yil, bilgi.surecNo, bilgi.islem.kod, 0, ENUMSurecParcaDurum.BASLADI, kullanan.kullaniciKodu);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    if (bilgi.surecDurum == (int)ENUMSurecDurum.YENI)
                    {
                        int parcaKod = IsParcaKoduVer(kullanan, bilgi, 0, -1);
                        ParcaDurumKaydet(ve, kullanan, bilgi.yil, bilgi.surecNo, bilgi.islem.kod, parcaKod, ENUMSurecParcaDurum.BASLADI, kullanan.kullaniciKodu);
                    }

                    sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00260, bilgi.surecNo) + "<br>";
                    sonuc.anahtar = bilgi.surecNo;
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, sürecin açıklama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetAciklama(Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan) ||
                !HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            if (OrtakFonksiyonlar.ConvertToDouble(bilgi.surecNo, 0) > 0)
            {
                ve.BeginTransaction();
                TABSurec to = new TABSurec(ve);

                bilgi.surecDurum = (int)ENUMSurecDurum.DEVAMEDIYOR;

                sonuc = to.KaydetAciklama(bilgi);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00261 + "<br>";
                    sonuc.anahtar = bilgi.surecNo;
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, sürecin durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc DurumDegistir(Kullanici kullanan, Surec bilgi)
        {
            if (string.IsNullOrWhiteSpace(bilgi.surecNo) || OrtakFonksiyonlar.ConvertToDouble(bilgi.surecNo, 0) <= 0)
            {
                return new Sonuc(false, "Lütfen süreç no giriniz!", "");
            }

            if (bilgi.yil <= 0)
            {
                return new Sonuc(false, "Lütfen süreç yılını giriniz!", "");
            }
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //bilgi içinde muhasebe ve birim bilgileri olmadığı için o bilgileri alınması gerekli
            Surec eskiSurec = new Surec();
            eskiSurec.yil = bilgi.yil;
            eskiSurec.surecNo = bilgi.surecNo;
            ObjectArray liste = Listele(null, kullanan, eskiSurec);
            if (!liste.sonuc.islemSonuc) return liste.sonuc;

            if (liste.ObjeSayisi == 0)
            {
                return new Sonuc(false, "Süreç bulunamadı giriniz!", "");
            }
            else if (liste.ObjeSayisi > 1)
            {
                return new Sonuc(false, "Birden fazla süreç bulundu!", "");
            }

            eskiSurec = (Surec)liste.objeler[0];
            bilgi.muhasebe = eskiSurec.muhasebe;
            bilgi.birim = eskiSurec.birim;

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan) ||
                !HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurec to = new TABSurec(ve);

            sonuc = to.DurumDegistir(bilgi);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }
            ve.Commit();

            string ekHataStr = "";
            OdemeEmriMIFForm mifForm = new OdemeEmriMIFForm();
            mifForm.surecNo = bilgi.surecNo;
            mifForm.yil = bilgi.yil;

            //Sürece bağlı belgelerin iptali
            if (bilgi.surecDurum == (int)ENUMSurecDurum.IPTAL)
            {
                //Sürece bağlı ödeme emirleri iptali
                HRCOdemeEmriForm hrcOdemeForm = new HRCOdemeEmriForm();

                sonuc = hrcOdemeForm.OdemeEmriIptal(kullanan, mifForm);

                //Sürece bağlı taşınır işlem fişleri iptali
                if (sonuc.islemSonuc)
                {
                    HRCTasinirForm hrcTasinirForm = new HRCTasinirForm();
                    TasinirForm tasinirForm = new TasinirForm();
                    tasinirForm.surecNo = bilgi.surecNo;
                    tasinirForm.yil = bilgi.yil;
                    sonuc = hrcTasinirForm.TasinirIslemFisIptal(kullanan, tasinirForm);
                    if (!sonuc.islemSonuc)
                        ekHataStr = TNS.HRC.HarcamaSurecleriResI.HRC00262;
                    else
                    {
                        //Onay formdaki kayıtlı bloke tutarlarını 0 yaparak kaydet
                        OnayFormTertip oft = new OnayFormTertip();
                        oft.yil = bilgi.yil;
                        oft.surecNo = bilgi.surecNo;
                        HRCOnayForm of = new HRCOnayForm();
                        sonuc = of.BlokeKaldirTertip(kullanan, oft);
                    }
                }
                else
                {
                    ekHataStr = TNS.HRC.HarcamaSurecleriResI.HRC00263;
                }
            }
            else if (bilgi.surecDurum == (int)ENUMSurecDurum.ARSIV)
            {
                //Blokeleri kaldır
                //OnayFormTertip oft = new OnayFormTertip();
                //oft.yil = bilgi.yil;
                //oft.surecNo = bilgi.surecNo;
                //HRCOnayForm of = new HRCOnayForm();
                //sonuc = of.BlokeKaldirTertip(kullanan, oft);
                sonuc = (new HRCOdemeEmriForm()).IptalOdenekGondermeTalebi(kullanan, mifForm);
                if (!sonuc.islemSonuc)
                    ekHataStr = "Ödeme Gönderme Talebi";
            }

            if (sonuc.islemSonuc)
            {
                string durum = "";
                if (bilgi.surecDurum == (int)ENUMSurecDurum.ARSIV)
                    durum = "<b>" + TNS.HRC.HarcamaSurecleriResI.HRC00264 + "</b>";
                else if (bilgi.surecDurum == (int)ENUMSurecDurum.IPTAL)
                    durum = "<b>" + TNS.HRC.HarcamaSurecleriResI.HRC00265 + "</b>";

                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00266, bilgi.surecNo, durum) + "<br>";
                sonuc.anahtar = bilgi.surecNo;
            }
            else
            {
                sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00267, bilgi.surecNo, ekHataStr);

                ve.BeginTransaction();
                to.DurumDegistir(eskiSurec);
                ve.Commit();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc IslemDegistir(Kullanici kullanan, Surec bilgi)
        {
            if (string.IsNullOrWhiteSpace(bilgi.surecNo) || OrtakFonksiyonlar.ConvertToDouble(bilgi.surecNo, 0) <= 0)
            {
                return new Sonuc(false, "Lütfen süreç no giriniz!", "");
            }

            if (bilgi.yil <= 0)
            {
                return new Sonuc(false, "Lütfen süreç yılını giriniz!", "");
            }
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //bilgi içinde muhasebe ve birim bilgileri olmadığı için o bilgileri alınması gerekli
            Surec eskiSurec = new Surec();
            eskiSurec.yil = bilgi.yil;
            eskiSurec.surecNo = bilgi.surecNo;
            ObjectArray liste = Listele(null, kullanan, eskiSurec);
            if (!liste.sonuc.islemSonuc) return liste.sonuc;

            if (liste.ObjeSayisi == 0)
            {
                return new Sonuc(false, "Süreç bulunamadı giriniz!", "");
            }
            else if (liste.ObjeSayisi > 1)
            {
                return new Sonuc(false, "Birden fazla süreç bulundu!", "");
            }

            eskiSurec = (Surec)liste.objeler[0];
            bilgi.muhasebe = eskiSurec.muhasebe;
            bilgi.birim = eskiSurec.birim;

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan) ||
                !HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurec to = new TABSurec(ve);

            sonuc = to.IslemDegistir(bilgi);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                return sonuc;
            }
            ve.Commit();

            ve.VeriErisimKapat();

            return sonuc;
        }


        public static Surec SurecOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo)
        {
            Surec surec = new Surec();
            surec.yil = yil;
            surec.surecNo = surecNo;

            ObjectArray sler = Listele(veriErisim, kullanan, surec);
            if (sler.ObjeSayisi == 1) return (Surec)sler[0];
            return null;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç için geçerli olan ekonomik kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç için geçerli olan ekonomik kod bilgisini string olarak döner</returns>
        public string SurecEkonomikKodAl(Kullanici kullanan, int yil, string surecNo)
        {
            string donusDeger = "";

            Surec surec = new Surec();
            surec.yil = yil;
            surec.surecNo = surecNo;

            ObjectArray sler = Listele(null, kullanan, surec);
            foreach (Surec s in sler.objeler)
            {
                if (donusDeger != "") donusDeger += ";";
                donusDeger += s.islem.ekoKod;
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Surec kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " surec başlıyor");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *1");

            TABSurec to = new TABSurec(ve);
            KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref kriter.muhasebe, ref kriter.birim);

            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *2" + kriter.muhasebe + kriter.birim);

            string kisiKosul = null;
            string yetkiTipiDegeri = null;  //OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Kişisel İşler");
            //if (!string.IsNullOrWhiteSpace(yetkiTipiDegeri))
            //{
            //    if (yetkiTipiDegeri.EndsWith(";")) yetkiTipiDegeri = yetkiTipiDegeri.Substring(0, yetkiTipiDegeri.Length - 1);
            //    kisiKosul = kullanan.kullaniciKodu;
            //}

            IDataReader r = to.Listele(kriter, kisiKosul, yetkiTipiDegeri);

            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *3");

            if (r != null)
            {
                while (r.Read())
                {
                    //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *4");

                    Surec sr = new Surec();

                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    sr.birim = VeriErisim.DegerAl(r, 3, "");
                    sr.islem.kod = VeriErisim.DegerAl(r, 4, 0);
                    sr.surecTarih = VeriErisim.DegerAl(r, 5);
                    sr.surecDurum = VeriErisim.DegerAl(r, 6, 0);
                    sr.aciklama = VeriErisim.DegerAl(r, 7, "");
                    sr.isinAdi = VeriErisim.DegerAl(r, 8, "");
                    sr.yillaraSariIslemMi = VeriErisim.DegerAl(r, 9, 0);
                    //9.kolon islem.kod yukarıda tanımlandı
                    sr.islem.ad = VeriErisim.DegerAl(r, 11, "");
                    sr.islem.ekoKod = VeriErisim.DegerAl(r, 12, "");
                    sr.islem.tur = VeriErisim.DegerAl(r, 13, 0);
                    sr.islem.odemeHesapKod = VeriErisim.DegerAl(r, 14, "");
                    sr.muhasebeAd = VeriErisim.DegerAl(r, 15, "");
                    sr.birimAd = VeriErisim.DegerAl(r, 16, "");

                    //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *5");

                    try
                    {
                        if (HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, sr.muhasebe.ToString(), sr.birim))
                        {
                            //sr.muhasebeAd = HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBE", sr.muhasebe.ToString(), true, "");
                            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *5a");

                            //sr.birimAd = HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", sr.birim + "-" + sr.muhasebe, false, "");
                            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *5b");

                            bilgi.objeler.Add(sr);
                        }
                    }
                    catch (Exception e)
                    {
                        OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *Hata verdi*" + sr.muhasebe.ToString() + " - " + sr.birim + "*" + e.Message + " " + e.StackTrace);
                        OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *Hata verdi*" + kullanan.kullaniciKodu);

                        bilgi.objeler = new TNSCollection();

                        bilgi.sonuc.hataStr = "Yetki hatası: " + e.Message;
                        bilgi.sonuc.islemSonuc = false;
                        break;
                    }

                    //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *6");

                }
                r.Close();
            }
            else
            {
                bilgi.sonuc.hataStr = to.Hata();
                bilgi.sonuc.islemSonuc = false;
            }

            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *30");

            foreach (Surec item in bilgi.objeler)
            {
                //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *31");

                IDataReader drIC = to.SurecBaslatanBul(item.yil, item.surecNo);
                if (drIC != null)
                {
                    //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *32");

                    if (drIC.Read())
                    {
                        //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *33");

                        item.kullaniciAdi = VeriErisim.DegerAl(drIC, 1, "");
                        item.surecTarih = VeriErisim.DegerAl(drIC, 0);

                    }
                    drIC.Close();
                }
            }
            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " *40");

            ve.VeriErisimKapat();

            //OrtakFonksiyonlar.HataStrYaz(kriter.surecNo + " * süreç bitiyor");

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleSurecMuhMifBaglanti(Kullanici kullanan, SurecMuhMifBaglanti kriter, Tertip tertip)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);
            KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref kriter.muhasebe, ref kriter.birim);
            IDataReader r = to.ListeleSurecMuhMifBaglanti(kriter, tertip);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SurecMuhMifBaglanti sr = new SurecMuhMifBaglanti();

                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    sr.birim = VeriErisim.DegerAl(r, 3, "");
                    sr.odemeEmriNo = VeriErisim.DegerAl(r, 4, "");
                    sr.borc = VeriErisim.DegerAl(r, 10, decimal.Zero);

                    bilgi.objeler.Add(sr);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Sorgula(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            int kriter_muhasebe = 0;
            string kriter_birim = "";
            KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref kriter_muhasebe, ref kriter_birim);
            if (kriter.muhasebe == 0) kriter.muhasebe = kriter_muhasebe;
            if (string.IsNullOrWhiteSpace(kriter.birim)) kriter.birim = kriter_birim;

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebe, kriter.birim))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);

            string kisiKosul = null;
            string yetkiTipiDegeri = "";
            string kisiselIsler = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Kişisel İşler");
            if (!kriter.alimIsleri)
            {
                yetkiTipiDegeri = kisiselIsler;
                if (!string.IsNullOrWhiteSpace(yetkiTipiDegeri))
                {
                    if (yetkiTipiDegeri.EndsWith(";")) yetkiTipiDegeri = yetkiTipiDegeri.Substring(0, yetkiTipiDegeri.Length - 1);
                    kisiKosul = kullanan.kullaniciKodu;
                }
            }

            IDataReader r = to.Sorgula(kriter, kisiKosul, yetkiTipiDegeri);
            string eskiSurecNo = "";
            Hashtable htYetki = new Hashtable();
            Hashtable htBirimAd = new Hashtable();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Surec sr = new Surec();
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    sr.birim = VeriErisim.DegerAl(r, 3, "");
                    sr.islem.kod = VeriErisim.DegerAl(r, 4, 0);
                    sr.surecTarih = VeriErisim.DegerAl(r, 5);
                    sr.surecDurum = VeriErisim.DegerAl(r, 6, 0);
                    sr.aciklama = VeriErisim.DegerAl(r, 7, "");
                    sr.isinAdi = VeriErisim.DegerAl(r, 8, "");
                    sr.yillaraSariIslemMi = VeriErisim.DegerAl(r, 9, 0);
                    //9.kolon islem.kod yukarıda tanımlandı
                    sr.islem.ad = VeriErisim.DegerAl(r, 11, "");
                    sr.islem.ekoKod = VeriErisim.DegerAl(r, 12, "");
                    sr.islem.tur = VeriErisim.DegerAl(r, 13, 0);
                    sr.islem.odemeHesapKod = VeriErisim.DegerAl(r, 14, "");
                    sr.muhasebeAd = VeriErisim.DegerAl(r, 15, "");
                    //sonradan, süreç sorgu ekranında liste yazdır bölümü için eklendi.
                    sr.parca.kod = VeriErisim.DegerAl(r, 16, 0);
                    //sr.parca.ad = VeriErisim.DegerAl(r, 16, "");
                    sr.parcaDurum.tarih = VeriErisim.DegerAl(r, 17);
                    sr.parcaDurum.kullaniciAd = VeriErisim.DegerAl(r, 18, "");
                    //sr.odemeEmriNo = VeriErisim.DegerAl(r, 19, "");
                    if (string.IsNullOrWhiteSpace(kisiselIsler))
                    {
                        sr.onayTutari = VeriErisim.DegerAl(r, 19, 0.0);
                        sr.yaklasikMaliyetTutari = VeriErisim.DegerAl(r, 20, 0.0);
                        sr.piyasaFiyatTutari = VeriErisim.DegerAl(r, 21, 0.0);
                        sr.sozlesmeTutari = VeriErisim.DegerAl(r, 22, 0.0);
                    }
                    int odemeEmriVar = VeriErisim.DegerAl(r, 23, 0);
                    if (odemeEmriVar > 0) sr.odemeEmriNo = "1";

                    int sifreVar = VeriErisim.DegerAl(r, 24, 0);
                    if (sifreVar > 0)
                    {
                        sr.onayTutari = -1;
                        sr.yaklasikMaliyetTutari = -1;
                        sr.piyasaFiyatTutari = -1;
                        sr.sozlesmeTutari = -1;
                    }

                    string yetkiKey = sr.muhasebe + "-" + sr.birim;
                    string birimAdKey = sr.birim + "-" + sr.muhasebe;

                    if (!htYetki.Contains(yetkiKey))
                        htYetki.Add(yetkiKey, HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, sr.muhasebe.ToString(), sr.birim));

                    if ((bool)htYetki[yetkiKey])
                    {
                        if (!htBirimAd.Contains(birimAdKey))
                            htBirimAd.Add(birimAdKey, HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", sr.birim + "-" + sr.muhasebe, false, ""));

                        sr.birimAd = (string)htBirimAd[birimAdKey];

                        if (eskiSurecNo != sr.surecNo)
                            bilgi.objeler.Add(sr);
                    }
                    eskiSurecNo = sr.surecNo;
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, devam eden süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, devam eden süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray DevamEdenler(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebe, kriter.birim))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);

            string kisiKosul = null;
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Kişisel İşler");
            if (!string.IsNullOrWhiteSpace(yetkiTipiDegeri))
            {
                if (yetkiTipiDegeri.EndsWith(";")) yetkiTipiDegeri = yetkiTipiDegeri.Substring(0, yetkiTipiDegeri.Length - 1);
                kisiKosul = kullanan.kullaniciKodu;
            }

            IDataReader r = to.DevamEdenler(kriter, kisiKosul, yetkiTipiDegeri);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Surec sr = new Surec();
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    sr.birim = VeriErisim.DegerAl(r, 3, "");
                    sr.islem.kod = VeriErisim.DegerAl(r, 4, 0);
                    sr.surecTarih = VeriErisim.DegerAl(r, 5);
                    sr.surecDurum = VeriErisim.DegerAl(r, 6, 0);
                    sr.aciklama = VeriErisim.DegerAl(r, 7, "");
                    sr.isinAdi = VeriErisim.DegerAl(r, 8, "");
                    sr.yillaraSariIslemMi = VeriErisim.DegerAl(r, 9, 0);
                    sr.islem.ad = VeriErisim.DegerAl(r, 11, "");

                    bilgi.objeler.Add(sr);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece ait parçaların durum bilgisini veritabanından okur 
        /// ve SurecParcaDurum nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece ait parça durum listesini SurecParcaDurum nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ParcaDurumListele(Kullanici kullanan, SurecParcaDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);
            IDataReader r = to.ParcaDurumListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SurecParcaDurum pp = new SurecParcaDurum();

                    pp.yil = VeriErisim.DegerAl(r, 0, 0);
                    pp.surecNo = VeriErisim.DegerAl(r, 1, "");
                    pp.islemKod = VeriErisim.DegerAl(r, 2, 0);
                    pp.parcaKod = VeriErisim.DegerAl(r, 3, 0);
                    pp.tarih = VeriErisim.DegerAl(r, 4);
                    pp.kullaniciAd = VeriErisim.DegerAl(r, 5, "");
                    pp.durum = VeriErisim.DegerAl(r, 6, 0);
                    pp.parcaAd = VeriErisim.DegerAl(r, 7, "");

                    bilgi.objeler.Add(pp);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini onaylandı olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParcaDurumOnayVer(Kullanici kullanan, SurecParcaDurum kriter)
        {

            Sonuc sonuc = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            string kontrolKod = TNS.DEG.Arac.Tanimla().DegiskenDegerBul(0, "/Surec" + kriter.parcaKod.ToString() + "KontrolKod");
            if (kontrolKod == "1")
            {
                sonuc = OnayBelgesiOdenekKontrol(ve, kullanan, kriter.yil, kriter.surecNo);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();

            TABSurec to = new TABSurec(ve);
            //Parçayı "tamamlandı" olarak işaretle
            sonuc = ParcaDurumKaydet(ve, kullanan, kriter.yil, kriter.surecNo, kriter.islemKod, kriter.parcaKod, ENUMSurecParcaDurum.TAMAMLANDI, kullanan.kullaniciKodu);

            if (sonuc.islemSonuc)
            {
                //Surec surec = new Surec();
                //surec.yil = kriter.yil;
                //surec.surecNo = kriter.surecNo;
                //surec.islem.kod = kriter.islemKod;
                //int parcaKodu = IsParcaKoduVer(kullanan, surec, 3, kriter.parcaKod);

                SurecParca surec = new SurecParca();
                surec.yil = kriter.yil;
                surec.surecNo = kriter.surecNo;
                surec.isKod = kriter.islemKod;
                int parcaKodu = IsParcaKoduVer(kullanan, surec, 3, kriter.parcaKod); //Sonraki parça kodunu al

                if (parcaKodu > 0)//Sonraki parça kodunu "başladı" olarak işaretle
                    sonuc = ParcaDurumKaydet(ve, kullanan, kriter.yil, kriter.surecNo, kriter.islemKod, parcaKodu, ENUMSurecParcaDurum.BASLADI, kullanan.kullaniciKodu);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private Sonuc OnayBelgesiOdenekKontrol(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo)
        {
            return new Sonuc(true, "", "");
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini devam ediyor ya da tanımsız olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParcaDurumOnayKaldir(Kullanici kullanan, SurecParcaDurum kriter)
        {
            Sonuc sonuc = new Sonuc();

            SurecParcaDurum kontrol = new SurecParcaDurum();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABSurec to = new TABSurec(ve);

            bool kilitsiz = true;

            //Surec surec = new Surec();
            //surec.yil = kriter.yil;
            //surec.surecNo = kriter.surecNo;
            //surec.islem.kod = kriter.islemKod;

            SurecParca surec = new SurecParca();
            surec.yil = kriter.yil;
            surec.surecNo = kriter.surecNo;
            surec.isKod = kriter.islemKod;
            surec.zorunlu = 1;
            int parcaKodu = IsParcaKoduVer(kullanan, surec, 3, kriter.parcaKod);//Kiliti kaldırılmak istenen parçadan bir sonraki parçayı al
            if (parcaKodu > 0)
            {

                //Kilit kalkması için sonraki parçanın kilitsiz olması gerek
                kontrol.yil = kriter.yil;
                kontrol.surecNo = kriter.surecNo;
                kontrol.islemKod = kriter.islemKod;
                kontrol.parcaKod = parcaKodu;

                ObjectArray kontrolParcalar = ParcaDurumListele(kullanan, kontrol);

                if (kontrolParcalar.objeler.Count > 0)
                {
                    kontrol = new SurecParcaDurum();
                    kontrol = (SurecParcaDurum)kontrolParcalar.objeler[0];
                    if (kontrol.durum == (int)ENUMSurecParcaDurum.TAMAMLANDI)
                        kilitsiz = false;
                }

                //KmL
                //"tamamlandı" parçasını "devam ediyor" olarak işaretle
                if (kilitsiz)
                {
                    sonuc = ParcaDurumKaydet(ve, kullanan, kriter.yil, kriter.surecNo, kriter.islemKod, kriter.parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                    //sonraki adımı tanımsız olarak kaydet
                    sonuc = ParcaDurumKaydet(ve, kullanan, kriter.yil, kriter.surecNo, kriter.islemKod, parcaKodu, ENUMSurecParcaDurum.TANIMSIZ, kullanan.kullaniciKodu);
                }
                else
                {
                    //sonuc.islemSonuc = false;
                    //sonuc.hataStr = "Kiliti kaldırmak istediğiniz parçadan sonraki adım kilitli. Lütfen ilk önce onun kilidini kaldırın";
                    //Eğer adım adım kilit kaldırılmak isteniyorsa alt kısımmı açıklama satırı yap, yukarıdakileri aç
                    Sonuc araSonuc = ParcaDurumOnayKaldir(kullanan, kontrol);
                    if (araSonuc.islemSonuc)
                        ParcaDurumOnayKaldir(kullanan, kriter);
                }

            }
            else
            {
                ObjectArray liste = new HRCSurecParca().ListeleCok(new Kullanici(), surec);
                if (kriter.parcaKod == ((SurecParca)liste.objeler[liste.ObjeSayisi - 1]).parcaKod)
                {
                    sonuc = ParcaDurumKaydet(ve, kullanan, kriter.yil, kriter.surecNo, kriter.islemKod, kriter.parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, parça durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi.</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi.</param>
        /// <param name="islemKod">İşlem yapılacak işlem kod bilgisi.</param>
        /// <param name="parcaKod">İşlem yapılacak parça kod bilgisi.</param>
        /// <param name="durum">İşlem yapılacak parça durum bilgisi.</param>
        /// <param name="kullaniciKod">İşlem yapan kullanıcı kod bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParcaDurumKaydet(VeriErisim ve, Kullanici kullanan, int yil, string surecNo, int islemKod, int parcaKod, ENUMSurecParcaDurum durum, string kullaniciKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve2 = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ve2.BeginTransaction();

            TABSurec to = new TABSurec(ve2);

                sonuc = to.ParcaDurumKaydet(yil, surecNo, islemKod, parcaKod, durum, kullaniciKod);

            if (durum > 0)
                sonuc = to.ParcaTarihceKaydet(yil, surecNo, islemKod, parcaKod, durum, kullaniciKod);

            if (sonuc.islemSonuc)
                ve2.Commit();
            else
                ve2.Rollback();

            ve2.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi.</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi.</param>
        /// <param name="durum">İşlem yapılacak süreç durum bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecDurumKaydet(VeriErisim ve, Kullanici kullanan, int yil, string surecNo, ENUMSurecDurum durum)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve2 = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve2.BeginTransaction();

            TABSurec to = new TABSurec(ve2);
            sonuc = to.SurecDurumKaydet(yil, surecNo, durum);

            if (sonuc.islemSonuc)
                ve2.Commit();
            else
                ve2.Rollback();

            ve2.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece ait parça durum bilgisini veritabanından okur 
        /// ve bool olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi.</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi.</param>
        /// <param name="parcaKod">The parca kod.</param>
        /// <returns>İşlem sonucunu bool olarak döner</returns>
        public bool ParcaTamamlandiMi(Kullanici kullanan, int yil, string surecNo, int parcaKod)
        {
            SurecParcaDurum kriter = new SurecParcaDurum();
            kriter.yil = yil;
            kriter.surecNo = surecNo;
            kriter.parcaKod = parcaKod;

            ObjectArray o = ParcaDurumListele(kullanan, kriter);
            foreach (SurecParcaDurum s in o.objeler)
            {
                if (s.durum == (int)ENUMSurecParcaDurum.TAMAMLANDI)
                    return true;
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürecin yıllara sari olup olmadığı bilgisini veritabanından okur 
        /// ve bool olarak döner
        /// </summary>
        /// <param name="surecYil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, sürecin yıllara sari olup olmadığı bilgisini bool olarak döner</returns>
        public bool YillaraSariIslemMi(int surecYil, string surecNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABSurec to = new TABSurec(ve);
            int yillaraSariIslemMi = to.YillaraSariIslemMi(surecYil, surecNo);

            ve.VeriErisimKapat();
            return Convert.ToBoolean(yillaraSariIslemMi);
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç hatırlatma bilgisini veritabanından okur 
        /// ve string (kod|aciklama) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, süreç hatırlatma bilgisi listesini string (kod|aciklama) nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HatirlatmaSorgula(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);
            IDataReader r = to.HatirlatmaSorgula(kullanan.kullaniciKodu);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    int kod = VeriErisim.DegerAl(r, 0, 0);
                    string aciklama = VeriErisim.DegerAl(r, 2, "");

                    bilgi.objeler.Add(kod + "|" + aciklama);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç hatırlatma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">İşlem yapılacak hatırlatma kodu bilgisi</param>
        /// <param name="aciklama">İşlem yapılacak hatırlatma açıklama bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HatirlatmaKaydet(Kullanici kullanan, int kod, string aciklama)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurec to = new TABSurec(ve);

            if (kod == 0)
            {
                //Belgeye numara alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "HRCHATIRLATMA";

                kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000000"), 0);
                //**********************************************************
            }

            if (aciklama != "")
                sonuc = to.HatirlatmaKaydet(kullanan.kullaniciKodu, kod, aciklama);
            else
                sonuc = to.HatirlatmaSil(kullanan.kullaniciKodu, kod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00261 + "<br>";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç tertip bilgisini veritabanından okur 
        /// ve string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"> The kullanan.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu yıl bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç tertip bilgisi listesini string nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TertipSorgula(Kullanici kullanan, int muhasebe, string birim)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);
            //Tertiplerin listesi
            IDataReader r = to.TertipSorgula(kullanan.kullaniciKodu);
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    string tertip = VeriErisim.DegerAl(r, 0, "");
                    tertip = TertipNoktala(tertip);
                    bilgi.objeler.Add(tertip);
                }
            }
            r.Close();
            //*******************************
            ve.VeriErisimKapat();

            //Harcama Bilgisini Çekmek İçin
            TNSCollection uzaySorgu = new TNSCollection();
            foreach (string s in bilgi.objeler)
            {
                TNS.UZY.UzaySorguBilgi uBilgi = new TNS.UZY.UzaySorguBilgi();
                uBilgi.uzayAd = "ISLHARCAMAMUHASEBE";
                uBilgi.esitle = false;
                uBilgi.kosul = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + muhasebe + "-" + birim + "-" + s;
                uBilgi.tur = "N";

                uzaySorgu.Add(uBilgi);
            }

            ObjectArray harcama = new ObjectArray();
            harcama = HRCServis.servisUZY.UzayDegeriToplu(kullanan, uzaySorgu);
            //*******************************

            //Ödenek Bilgisini Çekmek İçin
            uzaySorgu = new TNSCollection();
            foreach (string s in bilgi.objeler)
            {
                TNS.UZY.UzaySorguBilgi uBilgi = new TNS.UZY.UzaySorguBilgi();
                uBilgi.uzayAd = "MUHGONDERMEBELGESITENKISBIRIM";
                uBilgi.esitle = false;
                uBilgi.kosul = DateTime.Now.Year + "-" + "" + "-" + muhasebe + "-" + birim + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + s;
                uBilgi.tur = "N";

                uzaySorgu.Add(uBilgi);
            }
            ObjectArray odenek = new ObjectArray();
            odenek = HRCServis.servisUZY.UzayDegeriToplu(kullanan, uzaySorgu);

            //Bloke Bilgisini Çekmek İçin
            uzaySorgu = new TNSCollection();
            foreach (string s in bilgi.objeler)
            {
                TNS.UZY.UzaySorguBilgi uBilgi = new TNS.UZY.UzaySorguBilgi();
                uBilgi.uzayAd = "MUHBLOKEODENEK";
                uBilgi.esitle = false;
                uBilgi.kosul = DateTime.Now.Year + "-" + muhasebe + "-" + birim + "-" + s;
                uBilgi.tur = "N";

                uzaySorgu.Add(uBilgi);
            }
            ObjectArray devamEden = new ObjectArray();
            devamEden = HRCServis.servisUZY.UzayDegeriToplu(kullanan, uzaySorgu);

            //Avans Bilgisini Çekmek İçin
            uzaySorgu = new TNSCollection();
            foreach (string s in bilgi.objeler)
            {
                TNS.UZY.UzaySorguBilgi uBilgi = new TNS.UZY.UzaySorguBilgi();
                uBilgi.uzayAd = "MUHAVANS";
                uBilgi.esitle = false;
                uBilgi.kosul = DateTime.Now.Year + "-" + muhasebe + "-" + birim + "-" + s + "-";
                uBilgi.tur = "N";

                uzaySorgu.Add(uBilgi);
            }
            ObjectArray avans = new ObjectArray();
            avans = HRCServis.servisUZY.UzayDegeriToplu(kullanan, uzaySorgu);
            //*******************************

            for (int i = 0; i < bilgi.objeler.Count; i++)
                bilgi.objeler[i] = ((string)bilgi.objeler[i]) + "|" + harcama.objeler[i].ToString();

            for (int i = 0; i < bilgi.objeler.Count; i++)
                bilgi.objeler[i] = ((string)bilgi.objeler[i]) + "|" + odenek.objeler[i].ToString();

            for (int i = 0; i < bilgi.objeler.Count; i++)
                bilgi.objeler[i] = ((string)bilgi.objeler[i]) + "|" + devamEden.objeler[i].ToString();

            for (int i = 0; i < bilgi.objeler.Count; i++)
                bilgi.objeler[i] = ((string)bilgi.objeler[i]) + "|" + avans.objeler[i].ToString();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TertipKaydet(Kullanici kullanan, string tertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (tertip.Trim() == "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurec to = new TABSurec(ve);

            sonuc = to.TertipKaydet(kullanan.kullaniciKodu, tertip.Replace(".", ""));

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00261 + "<br>";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TertipSil(Kullanici kullanan, string tertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00268, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABSurec to = new TABSurec(ve);

            sonuc = to.TertipSil(kullanan.kullaniciKodu, tertip.Replace(".", ""));

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018 + "<br>";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray SurecDurumListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSurec to = new TABSurec(ve);

            IDataReader r = to.SurecDurumListele(kriter.yil, kriter.surecNoBasla);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Surec sr = new Surec();
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.islem.kod = VeriErisim.DegerAl(r, 2, 0);
                    sr.parca.kod = VeriErisim.DegerAl(r, 3, 0);
                    sr.surecTarih = VeriErisim.DegerAl(r, 4);
                    sr.kullaniciAdi = VeriErisim.DegerAl(r, 5, "");
                    sr.surecDurum = VeriErisim.DegerAl(r, 6, 0);
                    sr.parca.ad = VeriErisim.DegerAl(r, 7, "");
                    sr.islem.ad = VeriErisim.DegerAl(r, 8, "");
                    sr.aciklama = VeriErisim.DegerAl(r, 9, "");
                    sr.aciklama += " " + VeriErisim.DegerAl(r, 10, "");

                    bilgi.Ekle(sr);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        //Verilen işlem içindeki yer parametresine göre parçanın kodunu verir
        //Yer 1 ise ilk parça
        //2 ise aktifParcaKod dan önceki
        //3 ise aktifParcaKod dan sonraki
        private int IsParcaKoduVer(Kullanici kullanan, Surec surec, int yeri, int aktifParcaKod)
        {
            int parcaKod = 0;
            IsParca ip = new IsParca();
            ip = new HRCIsParca().Listele(new Kullanici(), surec.islem.kod);

            //İhaleden gelen parcalar
            //******************************
            IhaleParca ihp = new IhaleParca();
            ihp = IhaleParcaListesi(kullanan, surec.yil, surec.surecNo);
            foreach (Parca ii in ihp.parcalar.objeler)
                ip.parcalar.objeler.Add(ii);
            //******************************

            int sayac = 0;
            foreach (Parca p in ip.parcalar.objeler)
            {
                if (yeri == 0)//İlk parça
                {
                    for (int i = 0; i < ip.parcalar.objeler.Count; i++)
                    {
                        Parca pO = new Parca();
                        pO = (Parca)ip.parcalar.objeler[i];
                        if (pO.sayfa != "" && pO.zorunlu > 0)
                        {
                            parcaKod = pO.kod;
                            break;
                        }
                    }
                }
                else if (p.kod == aktifParcaKod)
                {
                    if (yeri == 2)//Önceki Parca
                    {
                        for (int i = sayac - 1; i > 0; i--)
                        {
                            Parca pO = new Parca();
                            pO = (Parca)ip.parcalar.objeler[i];
                            if (pO.sayfa != "" && pO.zorunlu > 0)
                            {
                                parcaKod = pO.kod;
                                break;
                            }
                        }
                    }
                    else if (yeri == 3)//Sonraki parça
                    {
                        for (int i = sayac + 1; i < ip.parcalar.objeler.Count; i++)
                        {
                            Parca pO = new Parca();
                            pO = (Parca)ip.parcalar.objeler[i];
                            if (pO.sayfa != "" && pO.zorunlu > 0)
                            {
                                parcaKod = pO.kod;
                                break;
                            }
                        }
                    }
                }

                if (parcaKod > 0)
                    break;
                sayac++;
            }

            return parcaKod;
        }

        private string TertipNoktala(string tertip)
        {
            string[] tParca = tertip.Split('-');

            TNS.BKD.Tertip t = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABHarcama);

            t.kodlar[0] = tParca[0].Replace(".", "");
            t.kodlar[1] = tParca[1].Replace(".", "");
            t.kodlar[2] = tParca[2].Replace(".", "");
            t.kodlar[3] = tParca[3].Replace(".", "");
            t.Noktala();

            return t.TertipStrOl();
        }

        private static void KullanicininYetkiliOlduguMuhasebeBirim(Kullanici kullanan, ref int muhasebe, ref string gelenBirim)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return;

            int calistigiMuhasebe = OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0);

            //Kullanıcı yöneticisi ise sadece muhasebe set edilicek
            //Milli emlak gazi bey kullanıcı yöneticisi ve harcama süreçlerinde taşra birimlerinin süreçlerini görmek istiyor
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            //    return;

            string birimListe = "";
            string muhasebeListe = "";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
            if (yetkiTipiDegeri != "")
            {
                ObjectArray d = new ObjectArray();
                int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                foreach (string yetkiTipiDegeriParca in d.objeler)
                {
                    string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                    if (muhasebeBirim.Length == 2)
                    {
                        if (muhasebeBirim[0].Trim() != "*")
                        {
                            if (muhasebeListe != "")
                                muhasebeListe += ";";
                            muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                        }

                        if (muhasebeBirim[1].Trim() != "*")
                        {
                            if (string.IsNullOrWhiteSpace(gelenBirim))
                            {
                                //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                if (birimListe != "") birimListe += ";";
                                birimListe += muhasebeBirim[1];
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(gelenBirim) && gelenBirim != "000")
                            {
                                if (birimListe != "") birimListe += ";";
                                birimListe += gelenBirim.Trim();
                            }
                        }
                    }
                }
            }
            else if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Taşınır Muhasebe Harcama Birim Yetkisi");
                if (yetkiTipiDegeri != "")
                {
                    ObjectArray d = new ObjectArray();
                    int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                    foreach (string yetkiTipiDegeriParca in d.objeler)
                    {
                        string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                        if (muhasebeBirim.Length == 3)
                        {
                            if (muhasebeBirim[0].Trim() != "*")
                            {
                                if (muhasebeListe != "")
                                    muhasebeListe += ";";
                                muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                            }

                            if (muhasebeBirim[1].Trim() != "*")
                            {
                                if (gelenBirim.Trim() == "")
                                {
                                    //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                    //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                    if (birimListe != "") birimListe += ";";
                                    if (muhasebeBirim[1].Length > 10)
                                        birimListe += muhasebeBirim[1].Replace(".", "").Substring(8, 3);//Son 3 hanesi
                                    else
                                        birimListe += "999";
                                }
                            }
                            else
                            {
                                if (gelenBirim.Trim() != "")
                                {
                                    if (birimListe != "") birimListe += ";";
                                    birimListe += gelenBirim;
                                }
                            }
                        }
                    }
                }
            }
            if (birimListe != "")
                gelenBirim = birimListe;
            if (muhasebeListe != "")
                muhasebe = calistigiMuhasebe;
        }

        private int IsParcaKoduVer(Kullanici kullanan, SurecParca surecParca, int yeri, int aktifParcaKod)
        {
            int parcaKod = 0;
            ObjectArray liste = new HRCSurecParca().ListeleCok(new Kullanici(), surecParca);

            int sayac = 0;
            foreach (SurecParca p in liste.objeler)
            {
                if (yeri == 0)//İlk parça
                {
                    for (int i = 0; i < liste.objeler.Count; i++)
                    {
                        SurecParca pO = new SurecParca();
                        pO = (SurecParca)liste.objeler[i];
                        //if (pO.sayfa != "" && pO.zorunlu > 0)
                        if (pO.zorunlu > 0)
                        {
                            parcaKod = pO.parcaKod;
                            break;
                        }
                    }
                }
                else if (p.parcaKod == aktifParcaKod)
                {
                    if (yeri == 2)//Önceki Parca
                    {
                        if (sayac == 1)
                        {
                            parcaKod = ((SurecParca)liste.objeler[0]).parcaKod;
                            break;
                        }
                        for (int i = sayac - 1; i > 0; i--)
                        {
                            SurecParca pO = new SurecParca();
                            pO = (SurecParca)liste.objeler[i];
                            //if (pO.sayfa != "" && pO.zorunlu > 0)
                            if (pO.zorunlu > 0)
                            {
                                parcaKod = pO.parcaKod;
                                break;
                            }
                        }
                    }
                    else if (yeri == 3)//Sonraki parça
                    {
                        for (int i = sayac + 1; i < liste.objeler.Count; i++)
                        {
                            SurecParca pO = new SurecParca();
                            pO = (SurecParca)liste.objeler[i];
                            //if (pO.sayfa != "" && pO.zorunlu > 0)
                            if (pO.zorunlu > 0)
                            {
                                parcaKod = pO.parcaKod;
                                break;
                            }
                        }
                    }
                }
                if (parcaKod > 0)
                    break;
                sayac++;
            }
            return parcaKod;
        }

        private int ParcaKodVer(SurecParca sp, bool sonrakiParcaIste)
        {
            int parcaKod = 0;
            sp.zorunlu = 1;
            sp.sonrakiParcaIste = sonrakiParcaIste;
            ObjectArray spListe = new HRCSurecParca().ListeleCok(new Kullanici(), sp);
            if (spListe.ObjeSayisi > 0)
            {
                if (sonrakiParcaIste)
                    parcaKod = ((SurecParca)spListe.objeler[spListe.ObjeSayisi - 1]).parcaKod;
                else
                    parcaKod = ((SurecParca)spListe.objeler[0]).parcaKod;
            }
            return parcaKod;
        }

        private IhaleParca IhaleParcaListesi(Kullanici kullanan, int yil, string surecNo)
        {
            IhaleParca ihp = new IhaleParca();

            int isTur = 0;
            Surec surec = new Surec();
            surec.yil = yil;
            surec.surecNo = surecNo;
            ObjectArray bilgi = new ObjectArray();
            bilgi = HRCSurec.Listele(null, kullanan, surec);
            if (bilgi.objeler.Count > 0)
            {
                isTur = ((Surec)bilgi.objeler[0]).islem.tur;
            }

            if (isTur == (int)ENUMIhaleTur.MAL ||
                 isTur == (int)ENUMIhaleTur.HIZMET ||
                 isTur == (int)ENUMIhaleTur.YAPIM)
            {

                OnayForm of = new OnayForm();
                of.yil = yil;
                of.surecNo = surecNo;

                bilgi = new ObjectArray();
                bilgi = new HRCOnayForm().Listele(kullanan, of);
                int ihaleUsul = 0;
                int ihaleMadde = 0;
                int komisyon = 0;
                if (bilgi.objeler.Count > 0)
                {
                    ihaleUsul = ((OnayForm)bilgi.objeler[0]).ihaleUsulu;
                    ihaleMadde = ((OnayForm)bilgi.objeler[0]).ihaleMaddesi;
                    komisyon = ((OnayForm)bilgi.objeler[0]).komisyonVarmi;

                    ihp = new HRCIhaleParca().Listele(kullanan, ihaleUsul, ihaleMadde, komisyon, isTur);
                }
            }

            return ihp;
        }

        internal static SurecOzetBilgi SurecOzetBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo)
        {
            SurecOzetBilgi bilgi = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSurec tabSurec = new TABSurec(ve);
            IDataReader r = tabSurec.OzetBilgiBul(surecYil, surecNo);
            if (r != null)
            {
                if (r.Read())
                {
                    bilgi = new SurecOzetBilgi();

                    bilgi.SURECYIL = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.SURECNO = VeriErisim.DegerAl(r, 1, "");
                    bilgi.MUHASEBEKOD = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.BIRIMKOD = VeriErisim.DegerAl(r, 3, "");
                    bilgi.ISLEMKOD = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.SURECTARIH = VeriErisim.DegerAl(r, 5);
                    bilgi.SURECDURUM = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.ACIKLAMA = VeriErisim.DegerAl(r, 7, "");
                    bilgi.ISINADI = VeriErisim.DegerAl(r, 8, "");
                    bilgi.YILLARASARIISLEMMI = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.HRCIS_AD = VeriErisim.DegerAl(r, 10, "");
                    bilgi.HRCIS_TUR = VeriErisim.DegerAl(r, 11, 0);
                    bilgi.ONAY_ISTANIM = VeriErisim.DegerAl(r, 12, "");
                    bilgi.ONAY_ISNITELIK = VeriErisim.DegerAl(r, 13, "");
                    bilgi.ONAY_ISMIKTAR = VeriErisim.DegerAl(r, 14, "");
                    bilgi.ONAY_IHALEUSULU = VeriErisim.DegerAl(r, 15, 0);
                    bilgi.ONAY_IHALEMADDESI = VeriErisim.DegerAl(r, 16, 0);
                    bilgi.MADDE_ADI = VeriErisim.DegerAl(r, 17, "");
                    bilgi.MADDE_ACIKLAMA = VeriErisim.DegerAl(r, 18, "");
                }
            }
            return bilgi;
        }
    }
}
