using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// Sürece bağlı ihale dosyası içerik bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleDosyasiIcerikListe
    {
        /// <summary>
        /// Arguman olarak verilen, sürece bağlı ihale dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleDosyasiIcerikListe liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";

            if (liste.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (liste.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (liste.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (liste.surecNo == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            if (liste.liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (IhaleDosyasiIcerik bilgi in liste.liste.objeler)
                {
                    if (bilgi.isTuru == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00146 + "<br>";
                        break;
                    }
                    if (bilgi.ihaleUsul == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00147 + "<br>";
                        break;
                    }
                    if (bilgi.adi == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00148, bilgi.sira) + "<br>";
                }
            }


            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////

            bool tamamlandi = true;
            int yil = liste.yil;
            string surecNo = liste.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleDosyasiIcerikListe to = new TABIhaleDosyasiIcerikListe(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////
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
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece bağlı ihale dosyası içerik bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece bağlı ihale dosyası içerik bilgisini IhaleDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        public IhaleDosyasiIcerikListe Listele(Kullanici kullanan, IhaleDosyasiIcerikListe kriter)
        {
            IhaleDosyasiIcerikListe bilgi = new IhaleDosyasiIcerikListe();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleDosyasiIcerikListe to = new TABIhaleDosyasiIcerikListe(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    if (bilgi.yil == 0)
                    {
                        bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    }

                    IhaleDosyasiIcerik idi = new IhaleDosyasiIcerik();

                    idi.isTuru = VeriErisim.DegerAl(r, 4, 0);
                    idi.ihaleUsul = VeriErisim.DegerAl(r, 5, 0);
                    idi.sira = VeriErisim.DegerAl(r, 6, 0);
                    idi.adi = VeriErisim.DegerAl(r, 7, "");
                    idi.dosyaAdi = VeriErisim.DegerAl(r, 8, "");

                    bilgi.liste.objeler.Add(idi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }

    /// <summary>
    /// İhale dosyası satın alma bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleDosyasiAlan
    {
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası satın alma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray list, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");
            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}


            string hataStr = "";
            int satir = 0;
            ObjectArray silinecekler = new ObjectArray();
            ObjectArray kaydedilecekler = new ObjectArray();
            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());
                foreach (IhaleTeklifDosyasi ida in list.objeler)
                {
                    satir++;
                    if (ida.sil == true)
                    {
                        IhaleTeklifDosyasi silinecek = new IhaleTeklifDosyasi();
                        silinecek.yil = ida.yil;
                        silinecek.muhasebe = ida.muhasebe;
                        silinecek.birim = ida.birim;
                        silinecek.surecNo = ida.surecNo;
                        silinecek.firmaKodu = ida.firmaKodu;
                        silinecekler.objeler.Add(silinecek);
                        //list.objeler.Remove(silinecek);
                        //list.objeler.RemoveAt(satir-1);
                    }
                    else
                    {
                        if (ida.yil == 0)
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                            break;
                        }
                        if (ida.muhasebe == 0)
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                            break;
                        }
                        if (ida.birim.Trim() == "")
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                            break;
                        }
                        if (ida.surecNo == "")
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                            break;
                        }

                        if (ida.firmaKodu == 0)
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00157, satir) + "<br>";
                        if (ida.firmaAdi.Trim() == "")
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00158, satir) + "<br>";
                        if (ida.alindiBelgeTarihi.isNull)
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00159, satir) + "<br>";

                        if (ida.alindiBelgeTarihi.Oku() > bugunTarihi.Oku())
                            hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //else if ((ida.alindiBelgeTarihi.Yil == bugunTarihi.Yil) && (ida.alindiBelgeTarihi.YilinGunu > bugunTarihi.YilinGunu))
                        //    hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //else if (ida.alindiBelgeTarihi.YilinGunu > bugunTarihi.YilinGunu)
                        //{
                        //    if (!YillaraSariIslemMi(kullanan, ida.yil, ida.surecNo))
                        //        hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //}

                        if (ida.alindiBelgeSaati == "")
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00161, satir) + "<br>";
                        try
                        {
                            //string gunSaat = ida.alindiBelgeSaati;
                            //int koordinat = gunSaat.IndexOf(":");
                            //int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                            //int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);
                            /*   if ((ida.alindiBelgeTarihi.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == ida.alindiBelgeTarihi.YilinGunu))
                               {
                                   if (saat > bugunTarihi.Saat)
                                       hataStr += satir + ". satırda Alındı Belgesi Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                                   else if ((saat == bugunTarihi.Saat) && (dakika > bugunTarihi.Dakika))
                                       hataStr += satir + ". satırda Alındı Belgesi Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                               }*/
                        }
                        catch
                        {
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00162, satir) + "<br>";
                        }
                        if (ida.satisBelgeTarihi.isNull)
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00163, satir) + "<br>";

                        if (ida.satisBelgeTarihi.Oku() > bugunTarihi.Oku())
                            hataStr += satir + ". satır Satış Belgesi Tarihi bu günün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //else if ((ida.satisBelgeTarihi.Yil == bugunTarihi.Yil) && (ida.satisBelgeTarihi.YilinGunu > bugunTarihi.YilinGunu))
                        //    hataStr += satir + ". satır Satış Belgesi Tarihi bu günün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //else if (ida.satisBelgeTarihi.YilinGunu > bugunTarihi.YilinGunu)
                        //{
                        //    if (!YillaraSariIslemMi(kullanan, ida.yil, ida.surecNo))
                        //        hataStr += satir + ". satır Satış Belgesi Tarihi bu günün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                        //}

                        //if (ida.satisBelgeNo == 0)
                        //    hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00165, satir) + "<br>";
                        if (ida.satinAlanKisi == "")
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00166, satir) + "<br>";

                        kaydedilecekler.objeler.Add(ida);
                    }
                }//end foreach
            }

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            IhaleTeklifDosyasi dosya = (IhaleTeklifDosyasi)list.objeler[0];
            int yil = dosya.yil;
            string surecNo = dosya.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleDosyasiAlan to = new TABIhaleDosyasiAlan(ve);
            ve.BeginTransaction();

            if (silinecekler.objeler.Count > 0)
                sonuc = to.Sil(silinecekler);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(kaydedilecekler);

            //Kullanıcı Dosya Alanlardan herhangi bir firmayı silerse, bu firmanın Zarf Kontrolündeki bilgileri de silinir.
            TABTeklifZarfKontrolu to2 = new TABTeklifZarfKontrolu(ve);
            if (sonuc.islemSonuc)
                to2.SilIhaleTeklifDosyasiOlmayan(dosya);

            //Kullanıcı Dosya Alanlardan herhangi bir firmayı silerse, bu firmanın Teklif Edilen Fiyat bilgileri de silinir.
            TABTeklifEdilenFiyat to3 = new TABTeklifEdilenFiyat(ve);
            if (sonuc.islemSonuc)
                to3.SilIhaleTeklifDosyasiOlmayan(dosya);


            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

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

        private bool YillaraSariIslemMi(Kullanici kullanan, int surecYil, string surecNo)
        {
            Surec surec = new Surec();
            surec.yil = surecYil;
            surec.surecNo = surecNo;

            ObjectArray surecListe = HRCSurec.Listele(null, kullanan, surec);
            if (surecListe.ObjeSayisi > 0)
            {
                surec = (Surec)surecListe.objeler[0];

                if (surec.yillaraSariIslemMi > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası satın alma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası satın alma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleDosyasiAlan to = new TABIhaleDosyasiAlan(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleTeklifDosyasi bilgi = new IhaleTeklifDosyasi();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.alindiSiraNo = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.firmaKodu = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.alindiBelgeTarihi = VeriErisim.DegerAl(r, 6);
                    bilgi.alindiBelgeSaati = VeriErisim.DegerAl(r, 7, "");
                    bilgi.satisBelgeTarihi = VeriErisim.DegerAl(r, 8);
                    bilgi.satisBelgeNo = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.teslimSiraNo = VeriErisim.DegerAl(r, 10, 0);
                    bilgi.teslimTarihi = VeriErisim.DegerAl(r, 11);
                    bilgi.teslimSaati = VeriErisim.DegerAl(r, 12, "");
                    bilgi.gelisSekli = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.postadaGeciktimi = VeriErisim.DegerAl(r, 14, 0);
                    bilgi.teklifDosyasiVerdimi = VeriErisim.DegerAl(r, 15, 0);
                    bilgi.satinAlanKisi = VeriErisim.DegerAl(r, 16, "");
                    bilgi.firmaDevam = VeriErisim.DegerAl(r, 17, 0);
                    bilgi.firmaYeterli = VeriErisim.DegerAl(r, 18, 0);
                    bilgi.gerekce = VeriErisim.DegerAl(r, 19, "");
                    bilgi.ihaleAsamasi = VeriErisim.DegerAl(r, 20, 0);
                    bilgi.belgeGidisSekli = VeriErisim.DegerAl(r, 21, 0);
                    bilgi.belgeGidisTarihi = VeriErisim.DegerAl(r, 22);
                    bilgi.firmaAdi = VeriErisim.DegerAl(r, 23, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }

    /// <summary>
    /// İhale dosyası teslim etme bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifDosyasiVeren
    {
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası teslim etme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray list, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());

            string hataStr = "";
            int satir = 0;
            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (IhaleTeklifDosyasi ida in list.objeler)
                {
                    satir++;
                    if (ida.yil == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                        break;
                    }
                    if (ida.muhasebe == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                        break;
                    }
                    if (ida.birim.Trim() == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                        break;
                    }
                    if (ida.surecNo == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                        break;
                    }
                    if (ida.firmaKodu == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00157, satir) + "<br>";
                    if (ida.firmaAdi.Trim() == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00158, satir) + "<br>";
                    if (ida.teslimTarihi.isNull)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00167, satir) + "<br>";
                    //if ((ida.teslimTarihi.Yil > bugunTarihi.Yil) || (ida.teslimTarihi.YilinGunu > bugunTarihi.YilinGunu))
                    //    hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00168, satir) + "<br>";

                    if (ida.teslimTarihi.Oku() > bugunTarihi.Oku())
                        hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                    //else if ((ida.teslimTarihi.Yil == bugunTarihi.Yil) && (ida.teslimTarihi.YilinGunu > bugunTarihi.YilinGunu))
                    //    hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                    //else if (ida.teslimTarihi.YilinGunu > bugunTarihi.YilinGunu)
                    //{
                    //    if (!YillaraSariIslemMi(kullanan, ida.yil, ida.surecNo))
                    //        hataStr += satir + ". satırda Alındı Belgesi Tarihi bugünün tarihinden sonraki bir tarih olarak seçilmiştir.<br>";
                    //}

                    try
                    {
                        //string gunSaat = ida.teslimSaati;
                        //int koordinat = gunSaat.IndexOf(":");
                        //int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                        //int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);
                        /*if ((ida.teslimTarihi.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == ida.teslimTarihi.YilinGunu))
                        {
                            if (saat > bugunTarihi.Saat)
                                hataStr += satir + ". satırda Alındı Belgesi Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                            else if ((saat == bugunTarihi.Saat) && (dakika > bugunTarihi.Dakika))
                                hataStr += satir + ". satırda Alındı Belgesi Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                        }*/
                    }
                    catch
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00162, satir) + "<br>";
                    }
                    if (ida.gelisSekli == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00169, satir) + "<br>";
                    else
                    {
                        if (ida.gelisSekli == (int)ENUMTeklifGelisTuru.ELDEN && ida.postadaGeciktimi == 1)
                            hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00170, satir) + "<br>";
                    }

                }
            }
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            IhaleTeklifDosyasi dosya = (IhaleTeklifDosyasi)list.objeler[0];
            int yil = dosya.yil;
            string surecNo = dosya.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDosyasiVeren to = new TABTeklifDosyasiVeren(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(list);

            //Kullanıcı Dosya Teslim Edenlerden herhangi bir firmanın teslim türünü Postada Gecikti veya Gelmedi olarak değiştirirse, bu firmanın Zarf Kontrolündeki bilgileri de silinir.
            TABTeklifZarfKontrolu to2 = new TABTeklifZarfKontrolu(ve);
            if (sonuc.islemSonuc)
                to2.SilIhaleTeklifDosyasiOlmayan(dosya);

            //Kullanıcı Dosya Teslim Edenlerden herhangi bir firmanın teslim türünü Postada Gecikti veya Gelmedi olarak değiştirirse, bu firmanın Teklif Edilen Fiyat bilgileri de silinir.
            TABTeklifEdilenFiyat to3 = new TABTeklifEdilenFiyat(ve);
            if (sonuc.islemSonuc)
                to3.SilIhaleTeklifDosyasiOlmayan(dosya);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

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

        private bool YillaraSariIslemMi(Kullanici kullanan, int surecYil, string surecNo)
        {
            Surec surec = new Surec();
            surec.yil = surecYil;
            surec.surecNo = surecNo;

            ObjectArray surecListe = HRCSurec.Listele(null, kullanan, surec);
            if (surecListe.ObjeSayisi > 0)
            {
                surec = (Surec)surecListe.objeler[0];

                if (surec.yillaraSariIslemMi > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Arguman olarak verilen, firmanın ihale devam edip etmeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Guncelle(Kullanici kullanan, ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());

            string hataStr = "";
            int satir = 0;
            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (IhaleTeklifDosyasi ida in list.objeler)
                {
                    satir++;
                    if (ida.yil == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                        break;
                    }
                    if (ida.muhasebe == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                        break;
                    }
                    if (ida.birim.Trim() == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                        break;
                    }
                    if (ida.surecNo == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                        break;
                    }
                    if (ida.firmaKodu == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00171, satir) + "<br>";
                        break;
                    }

                }
            }
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifUygunOlmayanBelge toBelge = new TABTeklifUygunOlmayanBelge(ve);
            TABTeklifDosyasiVeren toFirma = new TABTeklifDosyasiVeren(ve);
            ve.BeginTransaction();

            sonuc = toFirma.GuncelleFirmaSurecBilgisi(list);

            if (sonuc.islemSonuc)
            {
                foreach (IhaleTeklifDosyasi firma in list.objeler)
                {
                    if (firma.firmaDevam == (int)ENUMEvetHayir.EVET)
                    {
                        TeklifUygunOlmayanBelge firmaUygunOlmayan = new TeklifUygunOlmayanBelge();
                        firmaUygunOlmayan.yil = firma.yil;
                        firmaUygunOlmayan.muhasebe = firma.muhasebe;
                        firmaUygunOlmayan.birim = firma.birim;
                        firmaUygunOlmayan.surecNo = firma.surecNo;
                        firmaUygunOlmayan.firmaKod = firma.firmaKodu;
                        sonuc = toBelge.Sil(firmaUygunOlmayan);
                        if (!sonuc.islemSonuc)
                            break;
                    }
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası teslim etme bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası teslim etme bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDosyasiVeren to = new TABTeklifDosyasiVeren(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleTeklifDosyasi bilgi = new IhaleTeklifDosyasi();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.alindiSiraNo = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.firmaKodu = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.alindiBelgeTarihi = VeriErisim.DegerAl(r, 6);
                    bilgi.alindiBelgeSaati = VeriErisim.DegerAl(r, 7, "");
                    bilgi.satisBelgeTarihi = VeriErisim.DegerAl(r, 8);
                    bilgi.satisBelgeNo = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.teslimSiraNo = VeriErisim.DegerAl(r, 10, 0);
                    bilgi.teslimTarihi = VeriErisim.DegerAl(r, 11);
                    bilgi.teslimSaati = VeriErisim.DegerAl(r, 12, "");
                    bilgi.gelisSekli = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.postadaGeciktimi = VeriErisim.DegerAl(r, 14, 0);
                    bilgi.teklifDosyasiVerdimi = VeriErisim.DegerAl(r, 15, 0);
                    bilgi.satinAlanKisi = VeriErisim.DegerAl(r, 16, "");
                    bilgi.firmaDevam = VeriErisim.DegerAl(r, 17, 0);
                    bilgi.firmaYeterli = VeriErisim.DegerAl(r, 18, 0);
                    bilgi.gerekce = VeriErisim.DegerAl(r, 19, "");
                    bilgi.ihaleAsamasi = VeriErisim.DegerAl(r, 20, 0);
                    bilgi.belgeGidisSekli = VeriErisim.DegerAl(r, 21, 0);
                    bilgi.belgeGidisTarihi = VeriErisim.DegerAl(r, 22);
                    bilgi.firmaAdi = VeriErisim.DegerAl(r, 23, "");

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye devem eden firma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye devam eden firma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleDevamEdenFirma(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDosyasiVeren to = new TABTeklifDosyasiVeren(ve);
            IDataReader r = to.ListeleDevamEdenFirma(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleTeklifDosyasi bilgi = new IhaleTeklifDosyasi();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.alindiSiraNo = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.firmaKodu = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.alindiBelgeTarihi = VeriErisim.DegerAl(r, 6);
                    bilgi.alindiBelgeSaati = VeriErisim.DegerAl(r, 7, "");
                    bilgi.satisBelgeTarihi = VeriErisim.DegerAl(r, 8);
                    bilgi.satisBelgeNo = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.teslimSiraNo = VeriErisim.DegerAl(r, 10, 0);
                    bilgi.teslimTarihi = VeriErisim.DegerAl(r, 11);
                    bilgi.teslimSaati = VeriErisim.DegerAl(r, 12, "");
                    bilgi.gelisSekli = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.postadaGeciktimi = VeriErisim.DegerAl(r, 14, 0);
                    bilgi.teklifDosyasiVerdimi = VeriErisim.DegerAl(r, 15, 0);
                    bilgi.satinAlanKisi = VeriErisim.DegerAl(r, 16, "");
                    bilgi.firmaDevam = VeriErisim.DegerAl(r, 17, 0);
                    bilgi.firmaYeterli = VeriErisim.DegerAl(r, 18, 0);
                    bilgi.gerekce = VeriErisim.DegerAl(r, 19, "");
                    bilgi.ihaleAsamasi = VeriErisim.DegerAl(r, 20, 0);
                    bilgi.belgeGidisSekli = VeriErisim.DegerAl(r, 21, 0);
                    bilgi.belgeGidisTarihi = VeriErisim.DegerAl(r, 22);
                    bilgi.firmaAdi = VeriErisim.DegerAl(r, 23, "");


                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }

    /// <summary>
    /// İhale teklif dosyası komisyon tutanağı bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifDosyasiKomisyonTutanagi
    {
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası komisyon tutanağı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}
            string hataStr = "";

            if (bilgi.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (bilgi.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (bilgi.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (bilgi.surecNo.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            if (bilgi.tutanakTarihi.isNull)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00149 + "<br>";
            if (bilgi.tutanakSaati == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00150 + "<br>";
            if (bilgi.teslimEden == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00151 + "<br>";
            if (bilgi.teslimEdenUnvan == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00152 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = bilgi.yil;
            string surecNo = bilgi.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDosyasiKomisyonTutanagi to = new TABTeklifDosyasiKomisyonTutanagi(ve);

            ve.BeginTransaction();
            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası komisyon tutanağı bilgisini veritabanından okur 
        /// ve TeklifDosyasiKomisyonTutanagi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası komisyon tutanağı bilgisini TeklifDosyasiKomisyonTutanagi nesnesi olarak tutan nesne</returns>
        public TeklifDosyasiKomisyonTutanagi Listele(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi kriter)
        {
            TeklifDosyasiKomisyonTutanagi donusDegeri = new TeklifDosyasiKomisyonTutanagi();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDosyasiKomisyonTutanagi to = new TABTeklifDosyasiKomisyonTutanagi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.tutanakTarihi = VeriErisim.DegerAl(r, 4);
                    donusDegeri.tutanakSaati = VeriErisim.DegerAl(r, 5, "");
                    donusDegeri.teslimEden = VeriErisim.DegerAl(r, 6, "");
                    donusDegeri.teslimEdenUnvan = VeriErisim.DegerAl(r, 7, "");

                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}
