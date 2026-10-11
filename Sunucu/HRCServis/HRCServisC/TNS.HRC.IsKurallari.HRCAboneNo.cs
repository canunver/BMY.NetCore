using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Abone no bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAboneNo
    {
        /// <summary>
        /// Arguman olarak verilen, abone no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, AboneNo kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, kriter.muhasebe, kriter.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (kriter.birim == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (kriter.aboneNo == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00047 + "<br>";
            if (kriter.aboneAdi == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00048 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            if (kriter.kod == 0)
            {
                TABAboneNo to = new TABAboneNo(ve);
                IDataReader r = to.AboneNoKayitliMi(kriter);
                if (to.Hata() == "")
                {
                    if (r.Read())
                    {
                        AboneNo aboneNo = new AboneNo();
                        aboneNo.kod = VeriErisim.DegerAl(r, 0, 0);
                        aboneNo.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        aboneNo.birim = VeriErisim.DegerAl(r, 2, "");
                        aboneNo.aboneNo = VeriErisim.DegerAl(r, 3, "");
                        aboneNo.aboneAdi = VeriErisim.DegerAl(r, 4, "");
                        aboneNo.durum = VeriErisim.DegerAl(r, 5, 0);
                        aboneNo.ilKodu = VeriErisim.DegerAl(r, 6, "");
                        aboneNo.aboneTip = VeriErisim.DegerAl(r, 7, 0);
                        aboneNo.firma.kod = VeriErisim.DegerAl(r, 8, 0);
                        aboneNo.kayitTarihi = VeriErisim.DegerAl(r, 9);
                        aboneNo.kayitEden = VeriErisim.DegerAl(r, 10, "");
                        aboneNo.isletmeKodu = VeriErisim.DegerAl(r, 11, "");
                        r.Close();

                        sonuc.islemSonuc = false;
                        sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00049, aboneNo.aboneNo, aboneNo.muhasebe, aboneNo.birim);

                        return sonuc;
                    }
                    else
                    {
                        r.Close();

                        //AboneNo kod alınıyor
                        //**********************************************************
                        TABGenel tg = new TABGenel(ve);

                        string sayacKod = "ABONENOBILGI";

                        kriter.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                        //**********************************************************
                    }
                }
            }

            TABAboneNo toAboneNo = new TABAboneNo(ve);
            ve.BeginTransaction();

            if (kriter.aboneTip == (int)ENUMAboneTip.SU)
                kriter.isletmeKodu = kriter.ilKodu;

            sonuc = toAboneNo.Kaydet(kriter);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, abone no bilgisini veritabanından okur 
        /// ve AboneNo nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, abone no listesini AboneNo nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, AboneNo kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneNo to = new TABAboneNo(ve);
            IDataReader r = null;

            if (kriter.sadeceIlgili)
            {
                r = to.ListeleIlgili(kriter);
                if (r != null)
                {
                    while (r.Read())
                    {
                        AboneNo aboneNo = new AboneNo();
                        aboneNo = AboneIlgiliDondur(kullanan, r);
                        bilgi.objeler.Add(aboneNo);
                    }
                    r.Close();
                }
                else
                    throw new Exception(to.Hata());
            }
            else
            {
                r = to.Listele(kriter);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        AboneNo aboneNo = new AboneNo();
                        aboneNo = AboneNoDoldurDondur(kullanan, r);
                        bilgi.objeler.Add(aboneNo);
                    }
                    r.Close();
                }
            }
            ve.VeriErisimKapat();

            return bilgi;
        }

        private AboneNo AboneIlgiliDondur(Kullanici kullanan, IDataReader r)
        {
            AboneNo aboneNo = new AboneNo();

            aboneNo.firma.kod = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.KOD, 0);
            aboneNo.firma.ad = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.AD, "");
            aboneNo.firma.banka = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.BANKA, "");
            aboneNo.firma.hesapNo = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.HESAPNO, "");
            aboneNo.firma.vergiDairesi = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.VERGIDAIRESI, "");
            aboneNo.firma.vergiNo = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.VERGINO, "");
            aboneNo.firma.adres = VeriErisim.DegerAl(r, (int)TABFirmaBilgisi.ENUMAlanlar.ADRES, "");
            return aboneNo;
        }

        private AboneNo AboneNoDoldurDondur(Kullanici kullanan, IDataReader r)
        {
            AboneNo aboneNo = new AboneNo();

            aboneNo.kod = VeriErisim.DegerAl(r, 0, 0);
            aboneNo.muhasebe = VeriErisim.DegerAl(r, 1, 0);
            aboneNo.birim = VeriErisim.DegerAl(r, 2, "");
            aboneNo.aboneNo = VeriErisim.DegerAl(r, 3, "");
            aboneNo.aboneAdi = VeriErisim.DegerAl(r, 4, "");
            aboneNo.durum = VeriErisim.DegerAl(r, 5, 0);
            aboneNo.ilKodu = VeriErisim.DegerAl(r, 6, "");
            aboneNo.aboneTip = VeriErisim.DegerAl(r, 7, 0);
            aboneNo.firma.kod = VeriErisim.DegerAl(r, 8, 0);
            aboneNo.kayitTarihi = VeriErisim.DegerAl(r, 9);
            aboneNo.kayitEden = VeriErisim.DegerAl(r, 10, "");
            aboneNo.isletmeKodu = VeriErisim.DegerAl(r, 11, "");
            aboneNo.muhasebeAd = VeriErisim.DegerAl(r, 12, "");

            if (aboneNo.birim != "")
                aboneNo.birimAd = HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", aboneNo.birim + "-" + aboneNo.muhasebe, false, "");
            else
                aboneNo.birimAd = "";
            aboneNo.firma.ad = VeriErisim.DegerAl(r, 13, "");

            return aboneNo;
        }
    }
}
