using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Dosya Durum Tarihçe Sınıfı
    /// </summary>
    public class DosyaDurumTarihceClass
    {
        /// <summary>
        /// Alacak Takip Dosya Durum Tarihçe bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="durumTarihce">Durum Tarihçe bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection durumTarihce)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosyaDurumTarihce tabDurumTarihce = new TABATMDosyaDurumTarihce(ve);
            ve.BeginTransaction();

            try
            {
                foreach (DosyaDurumTarihce dt in durumTarihce)
                {
                    if (!Genel.DosyaVarMi(ve, dt.yil, dt.dosyaNo))
                    {
                        donusDeger.hataStr = AlacakTakipResI.ATA021;
                        donusDeger.islemSonuc = false;
                        break;
                    }

                    if (!Genel.DurumVarMi(ve, dt.durum.kod))
                    {
                        donusDeger.hataStr = AlacakTakipResI.ATA022;
                        donusDeger.islemSonuc = false;
                        break;
                    }

                    if (dt.durum.kod == (int)ENUMDurum.OdemeYapildi)
                    {
                        if (!Genel.TaksitOdendiMi(ve, dt.yil, dt.dosyaNo, 0))
                        {
                            donusDeger.hataStr = AlacakTakipResI.ATA023;
                            donusDeger.islemSonuc = false;
                            break;
                        }
                    }

                    if (dt.tarih.isNull)
                    {
                        donusDeger.hataStr = AlacakTakipResI.ATA024;
                        donusDeger.islemSonuc = false;
                        break;
                    }

                    if (dt.kullaniciKodu == "")
                    {
                        donusDeger.hataStr = AlacakTakipResI.ATA025;
                        donusDeger.islemSonuc = false;
                        break;
                    }

                    int sonDurum = Genel.SonDurumDondur(ve, dt.yil, dt.dosyaNo);

                    donusDeger = tabDurumTarihce.Kaydet(dt);

                    if (donusDeger.islemSonuc)
                        donusDeger = DosyaClass.DurumGuncelle(ve, dt.yil, dt.dosyaNo, dt.durum.kod, dt.tarih);
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }


        /// <summary>
        /// Yıl ve Dosya numarasına göre Alacak Takip Dosya Durum Tarihçe bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, int dosyaNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
            {
                donusDeger.sonuc.hataStr = AlacakTakipResI.ATA026;
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            if (dosyaNo <= 0)
            {
                donusDeger.sonuc.hataStr = AlacakTakipResI.ATA027;
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosyaDurumTarihce tabDurumT = new TABATMDosyaDurumTarihce(ve);
            IDataReader dr = null;

            try
            {
                dr = tabDurumT.Listele(yil, dosyaNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                DosyaDurumTarihce gelen = new DosyaDurumTarihce();
                gelen.yil = VeriErisim.DegerAl(dr, 0, -1);
                gelen.dosyaNo = VeriErisim.DegerAl(dr, 1, -1);
                gelen.siraNo = VeriErisim.DegerAl(dr, 2, -1);
                gelen.durum.kod = VeriErisim.DegerAl(dr, 3, -1);
                gelen.durum.ad = Genel.DurumAdiDondur(gelen.durum.kod);
                gelen.tarih = VeriErisim.DegerAl(dr, 4);
                gelen.aciklama = VeriErisim.DegerAl(dr, 5, "");
                gelen.kullaniciKodu = VeriErisim.DegerAl(dr, 6, "");
                Kullanici kul = new Kullanici();
                kul.kullaniciKodu = gelen.kullaniciKodu;

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA028;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
