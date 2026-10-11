using System;
using System.Data;
using TNS.BKD;
using TNS.HRC;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Ödenek gönderme talep işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTalepServis
    {
        /// <summary>
        /// Talep kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, TalepBelge belge)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            //if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            //{
            //    sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            ve.BeginTransaction();

            if (belge.talepNo > 0)
                sonuc = to.Sil(belge);
            if (sonuc.islemSonuc)
            {
                if (belge.talepNo == 0)
                    belge.talepNo = TalepNoAl(ve, belge.yil);

                sonuc = to.Kaydet(belge);
                if (sonuc.islemSonuc)
                {
                    //Tarihçe Kaydet
                    //sonuc = TarihceKaydet(belge);

                    if (sonuc.islemSonuc)
                    {
                        sonuc.anahtar = belge.talepNo.ToString();
                        sonuc.bilgiStr = ButceMuhasebeResI.MUH190;
                        ve.Commit();
                    }
                    else
                    {
                        sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                        ve.Rollback();
                    }
                }
                else
                {
                    sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Talep Ödenek gönderme belgesi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="talepDetay">talep detay</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        public Sonuc KaydetOdenekGonderme(Kullanici kullanan, ObjectArray talepDetay, int yil, int talepNo)
        {
            //Odenek Gonderme Belgesi olustur
            TalepBelge belge = ListeleTekBelge(kullanan, yil, talepNo);
            Sonuc sonuc = OdenekGondermeTalepOlustur(kullanan, belge);
            if (sonuc.islemSonuc)
            {
                //Karsilanan tutar aciklamasini kaydet
                sonuc = KaydetDetay(kullanan, talepDetay);

                if (string.IsNullOrEmpty(belge.odenekGondermeBelgeNo))
                {
                    //Talep tablosuna Odenek Gonderme Belge No kaydet
                    if (sonuc.islemSonuc)
                        sonuc = KaydetOdenekGondermeBelgeNo(yil, talepNo, sonuc.anahtar);
                }
            }

            return sonuc;
        }

        ///// <summary>
        ///// Talep onay belgesi kaydet.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="tertipListe">tertip liste</param>
        ///// <param name="talepYil">talep yil</param>
        ///// <param name="talepNo">talep no</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="birim">Birim kodu</param>
        ///// <param name="surecNo">Sürec no</param>
        ///// <returns></returns>
        //public Sonuc KaydetOnayBelgesiTalep(Kullanici kullanan, ObjectArray tertipListe, int talepYil, int talepNo, int muhasebe, string birim, string surecNo)
        //{
        //    TalepBelge belge = new TalepBelge();
        //    if (talepYil > 0 && talepNo > 0)
        //        belge = ListeleTekBelge(kullanan, talepYil, talepNo);
        //    else if (talepYil > 0 && !string.IsNullOrEmpty(surecNo))
        //        belge = ListeleTekBelge(kullanan, talepYil, surecNo);

        //    if (belge.talepNo == 0)
        //    {
        //        belge.yil = talepYil;
        //        belge.muhasebeKod = muhasebe;
        //        belge.birimKod = birim;
        //        belge.surecNo = surecNo;
        //        belge.surecYil = talepYil;
        //        belge.talepKullaniciKod = kullanan.kullaniciKodu;
        //        belge.talepTarih = new TNSDateTime(DateTime.Now);
        //        belge.formDurum = (int)ENUMTalepFormDurum.YENI;
        //        belge.formDurumTarih = new TNSDateTime(DateTime.Now);
        //    }

        //    string tertip = string.Empty;
        //    foreach (OnayFormTertip trtp in tertipListe.objeler)
        //    {
        //        TalepDetay detay = new TalepDetay();
        //        detay.yil = trtp.yil;
        //        detay.kurKod = trtp.kurKod;
        //        detay.fonKod = trtp.fonKod;
        //        detay.finKod = trtp.finKod;
        //        detay.ekoKod = trtp.ekoKod;
        //        detay.pebKod = trtp.pebKod;
        //        detay.talepNo = belge.talepNo;
        //        tertip = TertipStrOl(trtp.kurKod, trtp.fonKod, trtp.finKod, trtp.ekoKod, trtp.pebKod);
        //        detay.talepTutar = OrtakFonksiyonlar.ConvertToDecimal(OdenekIhtiyacHesapla(kullanan, belge.yil, belge.muhasebeKod, belge.birimKod, tertip, trtp.kullanilacakTutar));

        //        if (!ListedeVarMi(detay, belge.detay))
        //            belge.detay.objeler.Add(detay);
        //    }

        //    Sonuc sonuc = Kaydet(kullanan, belge);
        //    return sonuc;
        //}

        //private bool ListedeVarMi(TalepDetay detay, ObjectArray liste)
        //{
        //    foreach (TalepDetay d in liste.objeler)
        //    {
        //        if (d.kurKod.Replace(".", "") == detay.kurKod.Replace(".", "") &&
        //            d.fonKod.Replace(".", "") == detay.fonKod.Replace(".", "") &&
        //            d.finKod.Replace(".", "") == detay.finKod.Replace(".", "") &&
        //            d.ekoKod.Replace(".", "") == detay.ekoKod.Replace(".", "") &&
        //            d.pebKod.Replace(".", "") == detay.pebKod.Replace(".", ""))
        //            return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Talep tarihçe kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihce">tarihce</param>
        /// <returns></returns>
        public Sonuc KaydetTarihce(Kullanici kullanan, TalepTarihce tarihce)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetTarihce(tarihce);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceMuhasebeResI.MUH190;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Talep durum değiştir.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <param name="talepFormDurum">talep form durum</param>
        /// <returns></returns>
        public Sonuc DurumDegistir(Kullanici kullanan, int yil, int talepNo, int talepFormDurum)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            ve.BeginTransaction();

            sonuc = to.DurumDegistir(yil, talepNo, talepFormDurum);

            if (sonuc.islemSonuc)
            {
                //TarihceKaydet

                sonuc.bilgiStr = ButceMuhasebeResI.MUH192;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH193 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Talep listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, TalepBelge belge)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriMUH", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTalep to = new TABTalep(ve);
            IDataReader r = to.ListeleBelge(belge);

            if (to.Hata() == "")
            {
                try
                {
                    while (r.Read())
                    {
                        TalepBelge b = new TalepBelge();
                        TalepBelgeNesneDoldur(kullanan, ve, r, b);
                        bilgi.objeler.Add(b);
                    }
                    r.Close();
                }
                catch (Exception e)
                {
                    bilgi.sonuc.islemSonuc = false;
                    bilgi.sonuc.hataStr = e.Message;
                }
            }


            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Talep tek belge listele .
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        public TalepBelge ListeleTekBelge(Kullanici kullanan, int yil, int talepNo)
        {
            TalepBelge b = new TalepBelge();
            b.yil = yil;
            b.talepNo = talepNo;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            IDataReader r = to.ListeleBelge(b);

            if (to.Hata() == "")
            {
                try
                {
                    while (r.Read())
                    {
                        TalepBelgeNesneDoldur(kullanan, ve, r, b);
                    }
                    r.Close();
                }
                catch
                {

                }
            }

            ve.VeriErisimKapat();

            return b;
        }

        TalepBelge ListeleTekBelge(Kullanici kullanan, int surecYil, string surecNo)
        {
            TalepBelge b = new TalepBelge();
            b.surecYil = surecYil;
            b.surecNo = surecNo.PadLeft(6, '0');

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            IDataReader r = to.ListeleBelge(b);

            if (to.Hata() == "")
            {
                try
                {
                    while (r.Read())
                    {
                        TalepBelgeNesneDoldur(kullanan, ve, r, b);
                    }
                    r.Close();
                }
                catch
                {

                }
            }

            ve.VeriErisimKapat();

            return b;
        }

        ObjectArray ListeleDetay(Kullanici kullanan, TalepDetay detay, string odenekGondermeBelgeNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTalep to = new TABTalep(ve);
            IDataReader r = to.ListeleDetay(detay);

            if (to.Hata() == "")
            {
                try
                {
                    while (r.Read())
                    {
                        TalepDetay b = new TalepDetay();
                        TalepDetayNesneDoldur(odenekGondermeBelgeNo, ve, r, b);
                        bilgi.objeler.Add(b);
                    }
                    r.Close();
                }
                catch (Exception e)
                {
                    bilgi.sonuc.islemSonuc = false;
                    bilgi.sonuc.hataStr = e.Message;
                }
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        private Sonuc KaydetOdenekGondermeBelgeNo(int yil, int talepNo, string odenekGondermeBelgeNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            //if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            //{
            //    sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABTalep to = new TABTalep(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetOdenekGondermeBelgeNo(yil, talepNo, odenekGondermeBelgeNo);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceMuhasebeResI.MUH190;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        
        private Sonuc KaydetDetay(Kullanici kullanan, ObjectArray detay)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalep to = new TABTalep(ve);
            ve.BeginTransaction();

            foreach (TalepDetay dty in detay.objeler)
            {
                sonuc = to.KaydetDetay(dty);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceMuhasebeResI.MUH190;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH191 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        
        private Sonuc KaydetTarihce(TalepBelge belge)
        {
            Sonuc sonuc = new Sonuc(false, "not implemented", "");
            return sonuc;
        }

        //private double OdenekGetir(Kullanici kullanan, int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        //{
        //    return new MUHServis().OdenekMiktarGetir(kullanan, muhasebe, birim, yil, kur, fon, fin, eko, peb);
        //    //double odenek = 0;
        //    //string odenekUstuKontrol = System.Configuration.ConfigurationManager.AppSettings.Get("OdenekUstuHarcamaKontrol");
        //    //if (odenekUstuKontrol.ToUpper() == "ODENEK")// veya ODENEKGONDERME
        //    //    odenek = MUHServis.UzaySayiDegerGetir("ABSBUTCEGIDER", yil.ToString() + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "");
        //    //else
        //    //    odenek = MUHServis.UzaySayiDegerGetir("MUHGONDERMEBELGESITENKIS", yil.ToString() + "----" + muhasebe + "-" + birim + "--5-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "");
        //    //return odenek;
        //}

        //private bool EkoHesapKodKontrol(string eko, string hesapKod)
        //{
        //    return (eko.StartsWith("01") || eko.StartsWith("02") ||

        //        (hesapKod.StartsWith("16") && (eko.Replace(".", "").StartsWith("034"))) || //Avans ise 034 kontrol

        //        (!hesapKod.StartsWith("16") && (eko.Replace(".", "").StartsWith("034201") || eko.Replace(".", "").StartsWith("034204"))) ||  // Avans değilse detayda kontrol
        //            eko.Replace(".", "").StartsWith("039") || eko.Replace(".", "").StartsWith("033201")); //|| eko.Replace(".", "").StartsWith("035202"));
        //}
        
        //private string SurecBilgiGetir(Kullanici kullanan, int surecYil, string surecNo, int muhasebe, string birim)
        //{
        //    Surec surec = new Surec();
        //    surec.yil = surecYil;
        //    surec.surecNo = surecNo;
        //    surec.muhasebe = muhasebe;
        //    surec.birim = birim;

        //    IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
        //    ObjectArray liste = servisHRC.SurecListele(kullanan, surec);
        //    string donenDeger = string.Empty;
        //    if (liste.ObjeSayisi > 0)
        //    {
        //        surec = (Surec)liste.objeler[0];
        //        donenDeger += "Süreç No: " + surec.yil + "/" + surec.surecNo;
        //        if (!string.IsNullOrEmpty(surec.isinAdi))
        //            donenDeger += "- İşin Adı: " + surec.isinAdi;
        //        if (!string.IsNullOrEmpty(surec.aciklama))
        //            donenDeger += "- Açıklama: " + surec.aciklama;
        //    }
        //    else
        //        donenDeger = "Ödenek İhtiyacı";
        //    return donenDeger;
        //}

        //Sonuc OdenekTalepIslemleri(Kullanici kullanan, int belgeYil, string belgeNo, int surecYil, string surecNo, int muhasebe, string birim, string tertip, double borc)
        //{
        //    Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH165, "");

        //    if (!string.IsNullOrEmpty(tertip))
        //    {
        //        string[] tertipler = tertip.Split('-');
        //        if (tertipler.Length > 0)
        //        {
        //            string kur = tertipler[0];
        //            string fon = tertipler[1];
        //            string fin = tertipler[2];
        //            string eko = tertipler[3];
        //            string peb = "";
        //            if (tertipler.Length > 4)
        //                peb = tertipler[4];


        //            if (kur != "" && fon != "" && fin != "" && eko != "")
        //            {
        //                Tertip t = new Tertip(surecYil, ENUMTabloKod.TABButce);

        //                string kontrolEko = eko.Replace(".", "").Substring(0, 3);
        //                double odenek = OdenekGetir(kullanan, surecYil, muhasebe, birim, kur, fon, fin, kontrolEko, "");
        //                string hesapKod = string.Empty;

        //                if (!EkoHesapKodKontrol(eko, hesapKod))
        //                {
        //                    double harcama = MUHServis.UzaySayiDegerGetir("MUHODEMEEMRIMIF", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-830,834-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko + "-1/8--", false, "");
        //                    double avans = MUHServis.UzaySayiDegerGetir("MUHODEMEEMRIMIF", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-16-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko + "-1/8--", false, "");
        //                    double blokeEdilen = MUHServis.UzaySayiDegerGetir("MUHBLOKEODENEK", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");
        //                    string blokesql = MUHServis.servisUZY.UzayKosulStr(kullanan, "MUHBLOKEODENEK", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");

        //                    double borcToplam = harcama + avans + blokeEdilen;
        //                    double kalan = odenek - borcToplam;
        //                    if (kalan < 0)
        //                        kalan = 0;

        //                    string odenekUstuIzni = System.Configuration.ConfigurationManager.AppSettings.Get("OdenekUstuHarcamaIzni");
        //                    if (borc > kalan && borc > avans)
        //                    {
        //                        if (OrtakFonksiyonlar.ConvertToInt(odenekUstuIzni, 0) == 0)
        //                        {
        //                            double odenekTalep = MUHServis.UzaySayiDegerGetir("MUHGONDERMEBELGESITENKIS", surecYil.ToString() + "----" + muhasebe + "-" + birim + "--3/5-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");
        //                            double ihtiyac = borc - kalan - odenekTalep;
        //                            ihtiyac = Math.Round(ihtiyac, 2);
        //                            if (ihtiyac > 0)
        //                            {
        //                                if (odenek < 10000)
        //                                {
        //                                    if (ihtiyac > 10000)
        //                                        ihtiyac = 10000;
        //                                }
        //                                else
        //                                {
        //                                    if (ihtiyac > odenek)
        //                                        ihtiyac = 2 * odenek;
        //                                }
        //                                sonuc.hataStr += "<br>" + string.Format(ButceMuhasebeResI.MUH166, odenek.ToString("N2"), harcama.ToString("N2"), avans.ToString("N2"), blokeEdilen.ToString("N2"), kalan.ToString("N2"));
        //                                if (sonuc.islemSonuc)
        //                                {

        //                                    sonuc.hataStr += "<br>" + string.Format(ButceMuhasebeResI.MUH161, sonuc.anahtar, ihtiyac.ToString());
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            sonuc.islemSonuc = true;
        //                            sonuc.bilgiStr = ButceMuhasebeResI.MUH167;
        //                            sonuc.hataStr = string.Empty;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sonuc.islemSonuc = true;
        //                        if (OrtakFonksiyonlar.ConvertToInt(odenekUstuIzni, 0) == 0)
        //                            sonuc.bilgiStr = ButceMuhasebeResI.MUH168;
        //                        sonuc.hataStr = string.Empty;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return sonuc;
        //}

        //private string TertipStrOl(string kurKod, string fonKod, string finKod, string ekoKod, string pebKod)
        //{
        //    string tertip = kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod;
        //    if (!string.IsNullOrEmpty(pebKod))
        //        tertip += "-" + pebKod;
        //    return tertip.Replace(".", "");
        //}

        //private double OdenekIhtiyacHesapla(Kullanici kullanan, int surecYil, int muhasebe, string birim, string tertip, double borc)
        //{
        //    double ihtiyac = 0.0;
        //    if (!string.IsNullOrEmpty(tertip))
        //    {
        //        string[] tertipler = tertip.Split('-');
        //        if (tertipler.Length > 0)
        //        {
        //            string kur = tertipler[0];
        //            string fon = tertipler[1];
        //            string fin = tertipler[2];
        //            string eko = tertipler[3];
        //            string peb = "";
        //            if (tertipler.Length > 4)
        //                peb = tertipler[4];


        //            if (kur != "" && fon != "" && fin != "" && eko != "")
        //            {
        //                Tertip t = new Tertip(surecYil, ENUMTabloKod.TABButce);

        //                string kontrolEko = eko.Replace(".", "").Substring(0, 3);
        //                double odenek = OdenekGetir(surecYil, muhasebe, birim, kur, fon, fin, kontrolEko);
        //                string hesapKod = string.Empty;

        //                if (!EkoHesapKodKontrol(eko, hesapKod))
        //                {
        //                    double harcama = MUHServis.UzaySayiDegerGetir("MUHODEMEEMRIMIF", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-830,834-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko + "-1/8--", false, "");
        //                    double avans = MUHServis.UzaySayiDegerGetir("MUHODEMEEMRIMIF", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-16-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko + "-1/8--", false, "");
        //                    double blokeEdilen = MUHServis.UzaySayiDegerGetir("MUHBLOKEODENEK", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");
        //                    //string blokesql = MUHServis.servisUZY.UzayKosulStr(kullanan, "MUHBLOKEODENEK", surecYil.ToString() + "-" + muhasebe + "-" + birim + "-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");

        //                    double borcToplam = harcama + avans + blokeEdilen;
        //                    double kalan = odenek - borcToplam;
        //                    if (kalan < 0)
        //                        kalan = 0;

        //                    string odenekUstuIzni = System.Configuration.ConfigurationManager.AppSettings.Get("OdenekUstuHarcamaIzni");
        //                    if (borc > kalan && borc > avans)
        //                    {
        //                        if (OrtakFonksiyonlar.ConvertToInt(odenekUstuIzni, 0) == 0)
        //                        {
        //                            double odenekTalep = MUHServis.UzaySayiDegerGetir("MUHGONDERMEBELGESITENKIS", surecYil.ToString() + "----" + muhasebe + "-" + birim + "--3/5-" + kur + "-" + fon + "-" + fin + "-" + kontrolEko, false, "");
        //                            ihtiyac = borc - kalan - odenekTalep;
        //                            ihtiyac = Math.Round(ihtiyac, 2);
        //                            if (ihtiyac > 0)
        //                            {
        //                                if (odenek < 10000)
        //                                {
        //                                    if (ihtiyac > 10000)
        //                                        ihtiyac = 10000;
        //                                }
        //                                else
        //                                {
        //                                    if (ihtiyac > odenek)
        //                                        ihtiyac = 2 * odenek;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                        }
        //                    }
        //                    else
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return ihtiyac;
        //}

        private Sonuc OdenekGondermeTalepOlustur(Kullanici kullanan, TalepBelge belge) //int yil, int muhasebe, string birim, ObjectArray talepDetay, string odenekAciklama, int surecYil, string surecNo, string odenekGondermeBelgeNo)
        {
            decimal ihtiyac = decimal.Zero;
            string tertip = string.Empty;

            Sonuc sonuc = new Sonuc();
            GondermeTenkisForm form = new GondermeTenkisForm();
            form.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            form.yil = belge.yil;
            form.kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            form.muhasebe = belge.muhasebeKod;
            form.birim = belge.birimKod;
            form.surecYil = belge.surecYil;
            form.surecNo = belge.surecNo;
            form.belgeNo = belge.odenekGondermeBelgeNo;

            foreach (TalepDetay dty in belge.detay.objeler)
            {
                GondermeTenkisDetay detay = new GondermeTenkisDetay();
                tertip = dty.kurKod + "-" + dty.fonKod + "-" + dty.finKod + "-" + dty.ekoKod;
                if (!string.IsNullOrEmpty(dty.pebKod))
                    tertip += "-" + dty.pebKod;
                detay.tertip.TertipAta(tertip);
                detay.miktar = (double)dty.talepTutar;
                ihtiyac += dty.talepTutar;
                form.detay.objeler.Add(detay);
            }

            form.belgeToplami = (double)ihtiyac;

            string talepBilgi = string.Empty;
            ObjectArray formListe = new MUHServis().GondermeTenkisListele(kullanan, form, 0);
            if (formListe.objeler.Count > 0)
            {
                for (int i = 0; i < formListe.objeler.Count; i++)
                {
                    GondermeTenkisForm oncekiForm = (GondermeTenkisForm)formListe.objeler[i];
                    if (oncekiForm.formDurum == (int)ENUMBelgeDurumu.YENITALEP || oncekiForm.formDurum == (int)ENUMBelgeDurumu.TALEP)
                    {
                        form.belgeNo = oncekiForm.belgeNo;

                        if (form.belgeToplami == oncekiForm.belgeToplami)
                        {
                            talepBilgi += string.Format(ButceMuhasebeResI.MUH169, oncekiForm.belgeNo, oncekiForm.belgeToplami.ToString()) + "<br>";
                        }
                        else if (ihtiyac == 0)
                            talepBilgi += string.Format(ButceMuhasebeResI.MUH170, oncekiForm.belgeNo, oncekiForm.belgeToplami.ToString()) + "<br>";
                    }
                    else if (oncekiForm.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                    {
                        continue;
                    }
                    else
                    {
                        talepBilgi += string.Format(ButceMuhasebeResI.MUH170, oncekiForm.belgeNo, oncekiForm.belgeToplami.ToString()) + "<br>";
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(talepBilgi))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceMuhasebeResI.MUH171 + "<br>" + talepBilgi;
                return sonuc;
            }

            //---------------
            form.talep = "1";
            form.formDurum = (int)ENUMBelgeDurumu.YENITALEP;
            form.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            form.aciklama = belge.aciklama;
            form.islemYapan = kullanan.kullaniciKodu;
            form.yaziNo = "";
            form.yaziTarih = new TNSDateTime();
            form.istekYaziNo = "";
            form.istekYaziTarih = new TNSDateTime();
            if (form.belgeNo == "")
                form.icmalNo = 0;

            sonuc = new MUHServis().GondermeTenkisKaydet(kullanan, form, true, false, false);
            return sonuc;
        }

        private void TalepBelgeNesneDoldur(Kullanici kullanan, VeriErisim ve, IDataReader r, TalepBelge b)
        {
            b.yil = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.YIL, 0);
            b.talepNo = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.TALEPNO, 0);
            b.muhasebeKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.MUHASEBEKOD, 0);
            b.birimKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.BIRIMKOD, string.Empty);
            b.aciklama = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.ACIKLAMA, string.Empty);
            b.talepTarih = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.TALEPTARIH);
            b.talepKullaniciKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.TALEPKULLANICIKOD, string.Empty);
            b.formDurum = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.FORMDURUM, 0);
            b.formDurumTarih = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.FORMDURUMTARIH);
            b.odenekGondermeBelgeNo = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.ODENEKGONDERMEBELGENO, string.Empty);
            b.surecYil = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.SURECYIL, 0);
            b.surecNo = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarBelge.SURECNO, string.Empty);

            b.karsilananTarih = KarsilananTarihGetir(ve, b.odenekGondermeBelgeNo);
            b.karsilayanKullaniciKod = KarsilayanKullaniciKodGetir(ve, b.odenekGondermeBelgeNo);

            b.detay = ListeleDetay(kullanan, new TalepDetay(b.yil, b.talepNo), b.odenekGondermeBelgeNo);
            //b.tarihce =
        }
        
        private string KarsilayanKullaniciKodGetir(VeriErisim ve, string p)
        {
            return string.Empty;
        }
        
        private TNSDateTime KarsilananTarihGetir(VeriErisim ve, string odenekGondermeBelgeNo)
        {
            return null;
        }
        
        private void TalepDetayNesneDoldur(string odenekGondermeBelgeNo, VeriErisim ve, IDataReader r, TalepDetay b)
        {
            b.yil = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.YIL, 0);
            b.talepNo = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.TALEPNO, 0);
            b.siraNo = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.SIRANO, 0);
            b.kurKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.KURKOD, string.Empty);
            b.fonKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.FONKOD, string.Empty);
            b.finKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.FINKOD, string.Empty);
            b.ekoKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.EKOKOD, string.Empty);
            b.pebKod = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.PEBKOD, string.Empty);
            b.talepTutar = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.TALEPTUTAR, decimal.Zero);
            b.talepAciklama = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.TALEPACIKLAMA, string.Empty);
            b.karsilananAciklama = VeriErisim.DegerAl(r, (int)TABTalep.ENUMAlanlarDetay.KARSILANANACIKLAMA, string.Empty);
            if (!string.IsNullOrEmpty(odenekGondermeBelgeNo))
            {
                string tertip = b.kurKod + "-" + b.fonKod + "-" + b.finKod + "-" + b.ekoKod;
                if (!string.IsNullOrEmpty(b.pebKod))
                    tertip += "-" + b.pebKod;
                b.karsilananTutar = KarsilananTutarGetir(ve, b.yil, odenekGondermeBelgeNo, tertip);
            }
        }
        
        private int TalepNoAl(VeriErisim ve, int yil)
        {
            TABGenel tg = new TABGenel(ve);
            int talepNo = 0;
            string sayacKod = "TALEP" + yil.ToString();
            talepNo = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "000000"), 0);
            return talepNo;
        }
        
        private decimal KarsilananTutarGetir(VeriErisim ve, int yil, string odenekGondermeBelgeNo, string tertip)
        {
            GondermeTenkisDetay detay = new MUHServis().GondermeTenkisTekBelgeTekDetayListele(null, yil, odenekGondermeBelgeNo, 4, tertip);
            return OrtakFonksiyonlar.ConvertToDecimal(detay.miktar);
        }
    }
}
