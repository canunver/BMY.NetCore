using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using TNS.HRC;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHEntegrasyonMIF
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            //sonuc = KontrolDurum(ve, kullanan, bilgi);
            //if (!sonuc.islemSonuc)
            //    return sonuc;

            ve.BeginTransaction();

            TABEntegrasyonMIF to = new TABEntegrasyonMIF(ve);

            if (string.IsNullOrWhiteSpace(bilgi.belgeNo))
            {

                //Belgeye numara alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = bilgi.yil + bilgi.kurum.Replace(".", "");
                sayacKod = "ENTGMIF" + sayacKod;

                bilgi.belgeNo = tg.BelgeNoAl(sayacKod, "000000");
                //**********************************************************

                //Eğer belgeno alındı ise aşağıda başına kurum kodu konuluyor
                //sonuc.Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                //**********************************************************
                if (OrtakFonksiyonlar.ConvertToInt(bilgi.belgeNo, 0) == 0)
                {
                    ve.VeriErisimKapat();
                    sonuc.DegerAta(ButceMuhasebeResI.MUH007);
                    return sonuc;
                }
                else
                    bilgi.belgeNo = bilgi.kurum.Replace(".", "").PadRight(4, '0') + bilgi.belgeNo;
                //**********************************************************
            }

            if (!string.IsNullOrWhiteSpace(bilgi.belgeNo))
                sonuc = to.Sil(bilgi.yil, bilgi.belgeNo);

            //if (!string.IsNullOrEmpty(bilgi.belgeNo))
            //    sonuc = to.Guncelle(bilgi);
            //else
            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH008, bilgi.belgeNo);
                sonuc.anahtar = bilgi.belgeNo.ToString();

                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Listeles the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm bilgi, bool getirDetay)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEntegrasyonMIF to = new TABEntegrasyonMIF(ve);
            IDataReader dr = null;

            try
            {
                dr = to.Listele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }


            int sonKolon = (int)TABEntegrasyonMIF.ENUMAlanlar.YEVMIYENO2;
            while (dr.Read())
            {
                OdemeEmriMIFForm b = new OdemeEmriMIFForm();

                b.yil = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.YIL, 0);
                b.belgeNo = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.BELGENO, "");
                b.kurum = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.KURUMKOD, "");
                b.muhasebe = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.MUHASEBEKOD, 0);
                b.birim = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.BIRIMKOD, "");
                b.aciklama = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.ACIKLAMA, "");
                b.islemTarih = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.ISLEMTARIH);
                b.formDurum = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.FORMDURUM, 0);
                b.islemYapan = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.ISLEMYAPAN, "");
                b.fisTurYap((ENUMFISTUR)VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.FISTUR, 0));
                b.kurKod = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.KURKOD, "");
                b.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.BASLAMATARIHI);
                b.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.BITISTARIHI);
                b.yevmiyeNo1 = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.YEVMIYENO1, 0);
                b.yevmiyeNo2 = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlar.YEVMIYENO2, 0);

                b.borcToplam = VeriErisim.DegerAl(dr, sonKolon + 1, 0.0);
                b.alacakToplam = VeriErisim.DegerAl(dr, sonKolon + 2, 0.0);
                b.ettn = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                b.muhasebeAd = VeriErisim.DegerAl(dr, sonKolon + 4, "");

                liste.Ekle(b);
            }
            dr.Close();

            if (getirDetay)
            {
                //foreach (SatinAlma b in liste.objeler)
                //{
                //    SatinAlmaDetay sd = new SatinAlmaDetay();
                //    sd.satinAlmaKod = b.kod;
                //    b.satirlar = ListeleDetay(ve, sd);
                //}

                foreach (OdemeEmriMIFForm item in liste.objeler)
                {
                    item.detay = ListeleDetay(ve, kullanan, item);
                }
            }

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABEntegrasyonMIF to = new TABEntegrasyonMIF(ve);

            if (!string.IsNullOrWhiteSpace(bilgi.belgeNo))
            {
                sonuc = to.Sil(bilgi.yil, bilgi.belgeNo);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleDurum(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = KontrolDurum(ve, kullanan, bilgi, "GUNCELLEDURUM");
            if (!sonuc.islemSonuc)
                return sonuc;

            bilgi.islemYapan = kullanan.ADSOYAD;

            ve.BeginTransaction();

            TABEntegrasyonMIF to = new TABEntegrasyonMIF(ve);

            if (!string.IsNullOrWhiteSpace(bilgi.belgeNo))
            {
                sonuc = to.GuncelleDurum(bilgi);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc KontrolDurum(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm bilgi, string islemTur)
        {
            Sonuc sonuc = new Sonuc();

            ObjectArray liste = Listele(ve, kullanan, bilgi, false);

            string hata = "";

            foreach (OdemeEmriMIFForm item in liste.objeler)
            {
                if (islemTur == "GUNCELLEDURUM")
                {
                    if (item.formDurum == (int)ENUMBelgeDurumu.ONAYLI && bilgi.formDurum == (int)ENUMBelgeDurumu.IPTAL)
                        hata = "<br>" + string.Format(ButceMuhasebeResI.MUH149, item.belgeNo.ToString()) + " Lütfen önce belgenin Onayını kaldırın.";
                }
                else if (islemTur == "KAYDET")
                {
                    if (item.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                        hata = "<br>" + string.Format(ButceMuhasebeResI.MUH149, item.belgeNo.ToString()) + " Lütfen önce belgenin Onayını kaldırın.";
                    if (item.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                        hata = "<br>" + string.Format(ButceMuhasebeResI.MUH021, item.belgeNo.ToString());
                }



            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }

            return sonuc;
        }


        /// <summary>
        /// Listeles the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, OdemeEmriMIFForm bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABEntegrasyonMIF to = new TABEntegrasyonMIF(ve);
            IDataReader dr = null;

            try
            {
                dr = to.ListeleDetay(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            Tertip tt = new Tertip(bilgi.yil, ENUMTabloKod.TABOdenek);
            Tertip th = new Tertip(bilgi.yil, ENUMTabloKod.TABOdemeEmri);

            int sonKolon = (int)TABEntegrasyonMIF.ENUMAlanlarDetay.TERTIP;
            while (dr.Read())
            {
                OdemeEmriMIFDetay b = new OdemeEmriMIFDetay(bilgi.yil);

                b.yil = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.YIL, 0);
                b.belgeNo = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.BELGENO, "");
                b.satirNo = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.SIRA, 0);
                b.hesapKod = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.HESAPKOD, "");
                b.kur = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.KURKOD, "");
                b.fon = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.FONKOD, "");
                b.fin = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.FINKOD, "");
                b.eko = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.EKOKOD, "");
                b.borc = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.BORC, 0.0);
                b.alacak = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.ALACAK, 0.0);
                b.altHesapKod = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.ALTHESAPKOD, "");
                b.aciklama = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.ACIKLAMA, "");
                b.detayBirim = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.DETAYBIRIM, "");
                b.sorguTertip = VeriErisim.DegerAl(dr, (int)TABEntegrasyonMIF.ENUMAlanlarDetay.TERTIP, "");

                if (b.kur != "")
                {
                    b.kur = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), b.kur);
                    b.fon = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), b.fon);
                    b.fin = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), b.fin);
                    b.eko = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), b.eko);
                }
                b.detayBirim = b.detayBirim + "." + b.kur;

                b.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)th.Kirilimlar(0), b.hesapKod);

                b.aciklama = VeriErisim.DegerAl(dr, sonKolon + 1, "");

                liste.Ekle(b);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Kontrols the specified tip.
        /// </summary>
        /// <param name="tip">The tip.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        private static Sonuc Kontrol(string tip, Kullanici kullanan, OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(bilgi.belgeNo))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (!string.IsNullOrEmpty(bilgi.belgeNo))
                {
                    OdemeEmriMIFForm kriter = new OdemeEmriMIFForm();
                    kriter.belgeNo = bilgi.belgeNo;
                    kriter.yil = bilgi.yil;

                    sonuc = KontrolDurum(null, kullanan, kriter, tip);
                }
                if (!sonuc.islemSonuc)
                    return sonuc;

                if (bilgi.muhasebe == 0 || string.IsNullOrEmpty(bilgi.birim))
                    sonuc.hataStr = "Birim bilgisi boş bırakılamaz." + "<br>";
            }

            if (tip == "IPTAL")
            {
                if (bilgi.belgeNo == "")
                    sonuc.hataStr = "İlk önce kayıtlı EntegrasyonMIF seçiniz." + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}
