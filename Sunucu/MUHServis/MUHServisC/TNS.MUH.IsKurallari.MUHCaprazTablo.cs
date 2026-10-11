using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TNS.HRC;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Capraz Tablo işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHCaprazTablo
    {
        /// <summary>
        /// Capraz Tablo kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, CaprazTablo bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            //if (!Yetki.SistemYoneticisimi(kullanan))
            //{
            //	sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //	sonuc.islemSonuc = false;
            //	return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABCaprazTablo to = new TABCaprazTablo(ve);
            TABGenel tg = new TABGenel(ve);

            bilgi.kayitEden = kullanan.KullaniciKodu;
            bilgi.kayitTarihi = new TNSDateTime(DateTime.Now);

            if (bilgi.kod <= 0)
            {

                bilgi.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("MUHCAPRAZTABLO", "00000"), 0);
                sonuc = to.Kaydet(bilgi);
            }
            else
                sonuc = to.Guncelle(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgi.kod.ToString();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH132;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Capraz tablo sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">CaprazTablo nesnesi</param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, CaprazTablo bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            //if (!Yetki.SistemYoneticisimi(kullanan))
            //{
            //	sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //	sonuc.islemSonuc = false;
            //	return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABCaprazTablo to = new TABCaprazTablo(ve);

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH207;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Capraz Tablo listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Capraz Tablo</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, CaprazTablo bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABCaprazTablo to = new TABCaprazTablo(ve);

            IDataReader dr = to.Listele(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    CaprazTablo mt = new CaprazTablo();
                    mt.kod = VeriErisim.DegerAl(dr, 0, 0);
                    mt.adi = VeriErisim.DegerAl(dr, 1, string.Empty);
                    mt.json = VeriErisim.DegerAl(dr, 2, string.Empty);
                    mt.kayitEden = VeriErisim.DegerAl(dr, 3, string.Empty);
                    mt.kayitTarihi = VeriErisim.DegerAl(dr, 4);
                    mt.sutunTur = VeriErisim.DegerAl(dr, 5, 0);

                    liste.objeler.Add(mt);
                }
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray ListeleTablo(Kullanici kullanan, CaprazTablo bilgi)
        {
            ObjectArray liste = new ObjectArray();

            string kodYapisiHesap = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABCaprazTablo to = new TABCaprazTablo(ve);

            IDataReader dr = to.ListeleTablo(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    OdemeEmriMIFDetay t = new OdemeEmriMIFDetay(bilgi.sorguYil);

                    t.yil = VeriErisim.DegerAl(dr, 0, 0);
                    t.belgeNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                    t.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(dr, 2, string.Empty));
                    t.kur = VeriErisim.DegerAl(dr, 3, string.Empty);
                    t.fon = VeriErisim.DegerAl(dr, 4, string.Empty);
                    t.fin = VeriErisim.DegerAl(dr, 5, "");
                    t.eko = VeriErisim.DegerAl(dr, 6, string.Empty);
                    t.peb = VeriErisim.DegerAl(dr, 7, string.Empty);
                    t.borc = VeriErisim.DegerAl(dr, 8, 0.0);
                    t.alacak = VeriErisim.DegerAl(dr, 9, 0.0);
                    t.detayBirim = VeriErisim.DegerAl(dr, 10, string.Empty);
                    t.altHesapKod = VeriErisim.DegerAl(dr, 11, string.Empty);

                    liste.objeler.Add(t);
                }
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray ListeleHesapKodTablo(Kullanici kullanan, CaprazTablo bilgi)
        {
            ObjectArray liste = new ObjectArray();
            Hashtable ht = new Hashtable();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABCaprazTablo to = new TABCaprazTablo(ve);

            IDataReader dr = to.ListeleTablo(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    string belgeNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                    string hesapKod = VeriErisim.DegerAl(dr, 2, string.Empty);

                    if (ht.ContainsKey(belgeNo))
                    {
                        if (ht[belgeNo].ToString().Contains(hesapKod))
                            continue;

                        string val = ht[belgeNo].ToString() + "|" + hesapKod;
                        ht[belgeNo] = val;
                    }
                    else
                        ht.Add(belgeNo, hesapKod);
                }

                liste.Ekle(ht);
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static ObjectArray ListeleHesapKodTablo2(Kullanici kullanan, CaprazTablo bilgi)
        {
            ObjectArray liste = new ObjectArray();
            Hashtable htSatir = new Hashtable();
            Hashtable htSutun = new Hashtable();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABCaprazTablo to = new TABCaprazTablo(ve);

            IDataReader dr = to.ListeleHesapKodTablo(bilgi);
            List<CaprazTabloIcerik> listeIcerik = new List<CaprazTabloIcerik>();
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    CaprazTabloIcerik icerik = new CaprazTabloIcerik();

                    icerik.belgeNo = VeriErisim.DegerAl(dr, 0, string.Empty);
                    icerik.belgeTarih = VeriErisim.DegerAl(dr, 1);
                    icerik.hesapKodu = VeriErisim.DegerAl(dr, 2, string.Empty);
                    icerik.yevmiyeNo = VeriErisim.DegerAl(dr, 3, 0);
                    icerik.yevmiyeTarih = VeriErisim.DegerAl(dr, 4);
                    icerik.bakiye = VeriErisim.DegerAl(dr, 5, 0.0);
                    icerik.ilgiliAdi = VeriErisim.DegerAl(dr, 6, "");
                    icerik.ilgiliNo = VeriErisim.DegerAl(dr, 7, "");
                    icerik.tur = VeriErisim.DegerAl(dr, 8, string.Empty);

                    listeIcerik.Add(icerik);
                }

                foreach (CaprazTabloIcerik icerik in listeIcerik)
                {
                    string anahtar = icerik.belgeNo + "_" + icerik.hesapKodu;
                    if (icerik.tur == "SATIR")
                    {
                        if (!htSatir.ContainsKey(anahtar))
                            htSatir[anahtar] = icerik;
                        else
                        {
                            CaprazTabloIcerik gecici = (CaprazTabloIcerik)htSatir[anahtar];
                            gecici.bakiye += icerik.bakiye;
                            htSatir[anahtar] = gecici;
                        }
                    }
                }

                foreach (CaprazTabloIcerik icerik in listeIcerik)
                {
                    string anahtar = icerik.belgeNo + "_" + icerik.hesapKodu;

                    if (icerik.tur == "SUTUN")
                    {
                        //satırda olmayan belgeler alınmasın
                        //************************************
                        bool bulundu = false;
                        foreach (DictionaryEntry item in htSatir)
                        {
                            string satirAnahtar = (string)item.Key;
                            if (satirAnahtar.StartsWith(icerik.belgeNo))
                            {
                                bulundu = true;
                                break;
                            }
                        }
                        if (!bulundu) continue;
                        //************************************

                        if (!htSutun.ContainsKey(anahtar))
                            htSutun[anahtar] = icerik;
                        else
                        {
                            CaprazTabloIcerik gecici = (CaprazTabloIcerik)htSutun[anahtar];
                            gecici.bakiye += icerik.bakiye;
                            htSutun[anahtar] = gecici;
                        }
                    }
                }

                liste.Ekle(htSatir);
                liste.Ekle(htSutun);
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }
    }
}