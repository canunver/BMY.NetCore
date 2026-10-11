using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUHASEBE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMuhasebe : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebe(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHMUHASEBE";

        public enum ENUMAlanlar
        {
            MUHASEBEKOD = 0,
            MUHASEBEAD = 1,
            ILGILI1 = 2,
            ILGILI2 = 3,
            ADRES = 4,
            TELEFON = 5,
            VERGINO = 6,
            FAKS = 7,
            EPOSTA = 8,
            ILGILI1UNVAN = 9,
            ILGILI2UNVAN = 10,
        }
        static string[] ALANLAR = { "MUHASEBEKOD", "MUHASEBEAD", "ILGILI1", "ILGILI2", "ADRES", "TELEFON", "VERGINO", "FAKS", "EPOSTA", "ILGILI1UNVAN", "ILGILI2UNVAN" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MuhasebeBilgisi muhasebe)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(muhasebe.kod) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ad) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ilgili1) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ilgili2) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.adres) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.telefon) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.vergiNo) + ",";
            sql += VeriErisim.DegerYap(muhasebe.faks) + ",";
            sql += VeriErisim.DegerYap(muhasebe.ePosta) + ",";
            sql += VeriErisim.DegerYap(muhasebe.ilgili1Unvan) + ",";
            sql += VeriErisim.DegerYap(muhasebe.ilgili2Unvan) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(MuhasebeBilgisi muhasebe)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + ALANLAR[(int)ENUMAlanlar.MUHASEBEAD] + "=" + VeriErisim.DegerYap(muhasebe.ad);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ILGILI1] + "=" + VeriErisim.DegerYap(muhasebe.ilgili1);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ILGILI2] + "=" + VeriErisim.DegerYap(muhasebe.ilgili2);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ADRES] + "=" + VeriErisim.DegerYap(muhasebe.adres);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.TELEFON] + "=" + VeriErisim.DegerYap(muhasebe.telefon);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.VERGINO] + "=" + VeriErisim.DegerYap(muhasebe.vergiNo);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FAKS] + "=" + VeriErisim.DegerYap(muhasebe.faks);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.EPOSTA] + "=" + VeriErisim.DegerYap(muhasebe.ePosta);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ILGILI1UNVAN] + "=" + VeriErisim.DegerYap(muhasebe.ilgili1Unvan);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ILGILI2UNVAN] + "=" + VeriErisim.DegerYap(muhasebe.ilgili2Unvan);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe.kod);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MuhasebeBilgisi muhasebe)
        {
            string sql = "DELETE FROM " + TABLOAD + " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe.kod);
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tablodaki bütün kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHepsini()
        {
            return KomutCalistir("DELETE FROM " + TABLOAD, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(MuhasebeBilgisi muhasebe)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = "";
            if (muhasebe.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe.kod));

            if (muhasebe.ad != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEAD], "LIKE", "%" + muhasebe.ad + "%"));
            }

            if (!string.IsNullOrWhiteSpace(muhasebe.kistasKodAd))
            {
                string icSsql = "";
                int kistasKod = OrtakFonksiyonlar.ConvertToInt(muhasebe.kistasKodAd, 0);
                if (kistasKod > 0)
                {
                    int carpan = 1;
                    if (kistasKod < 100)
                    {
                        carpan = 1000;
                    }
                    else if (kistasKod < 1000)
                    {
                        carpan = 100;
                    }
                    icSsql = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", kistasKod * carpan);
                    icSsql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (kistasKod + 1) * carpan);
                    if (carpan != 1)
                    {
                        icSsql = "((" + icSsql + ") OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kistasKod) + ")";
                    }
                }
                else
                    icSsql = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEAD], " LIKE ", "%" + muhasebe.kistasKodAd + "%");
                kosul = VeriErisim.KosulEkle(kosul, "AND", icSsql);
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        private Sonuc CokluSQLCalistir(string[] sqller)
        {
            Sonuc s = new Sonuc();
            foreach (var sql in sqller)
            {
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    Sonuc sIc = KomutCalistir(sql, 0);
                    if (sIc.islemSonuc)
                        s.kayitSay += sIc.kayitSay;
                    else
                        return sIc;
                }
            }
            return s;
        }

        public Sonuc OdemeEmriTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string sql = $"UPDATE MUHMIFFORM SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            return KomutCalistir(sql, 0);
        }

        public Sonuc AbonelikTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[2];
            sqller[0] = $"UPDATE HarcamaYonetimi.dbo.HRCABONENO SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE HarcamaYonetimi.dbo.HRCABONEFATURA SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc PersonelTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[4];

            sqller[0] = $"UPDATE Personel.dbo.PERPERSONEL SET MUHASEBEKODU={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKODU={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKODU={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKODU={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE Personel.dbo.PERPERSONELKADRODERECE SET MUHASEBEKODU={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKODU={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKODU={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKODU={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[2] = $"UPDATE Personel.dbo.PERPERSONELMEMURIYET SET MUHASEBEKODU={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKODU={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKODU={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKODU={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[3] = $"UPDATE Personel.dbo.PERGOREV SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc BirimButceTeklifiTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[3];

            sqller[0] = $"UPDATE ButceHazirlikv2.dbo.BHMBIRIMBUTCEHAZIRLIK SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE ButceHazirlikv2.dbo.BHMBIRIMONAY SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[2] = $"UPDATE ButceHazirlikv2.dbo.BHMBIRIMONAYTARIHCE SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc BirimHizmetMaliyetiTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[1];
            sqller[0] = $"UPDATE ButceHazirlik.dbo.BHFHIZMETMALIYETTESPITTASRA SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc DepremHarcamalariTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[2];

            sqller[0] = $"UPDATE ButceHazirlik.dbo.BHFDEPREMHARCAMAANA SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE ButceHazirlik.dbo.BHFDEPREMHARCAMADETAY SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc AracKiralamaPlaniTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[2];

            sqller[0] = $"UPDATE ButceHazirlik.dbo.BHFARACKIRALAMAPLANANA SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE ButceHazirlik.dbo.BHFARACKIRALAMAPLANDETAY SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc SurecTanimlariTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[1];
            sqller[0] = $"UPDATE HarcamaYonetimi.dbo.HRCSUREC SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc DogrudanTeminTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[1];
            sqller[0] = $"UPDATE HarcamaYonetimi.dbo.HRCDOGRUDANTEMINPLAN SET SAYMANLIK={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIM={VeriErisim.DegerYap(yeniBirimKod)} WHERE SAYMANLIK={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIM={VeriErisim.DegerYap(eskiBirimKod)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc SeyyarGorevPlanlamaTasi(string eskiHarcamaBirimi, string yeniHarcamaBirimi)
        {
            string[] sqller = new string[4];
            sqller[0] = $"UPDATE HarcamaYonetimi.dbo.HRCSEYYARGOREVPLANLANAN SET HARCAMABIRIMI={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE HARCAMABIRIMI={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            sqller[1] = $"UPDATE HarcamaYonetimi.dbo.HRCSEYYARGOREVGERCEKLESEN SET HARCAMABIRIMI={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE HARCAMABIRIMI={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            sqller[2] = $"UPDATE HarcamaYonetimi.dbo.HRCSEYYARGOREVONAY SET HARCAMABIRIMI={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE HARCAMABIRIMI={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            sqller[3] = $"UPDATE HarcamaYonetimi.dbo.HRCSEYYARGOREVUNVANBILGISI SET HARCAMABIRIMI={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE HARCAMABIRIMI={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc MevcutTasitVeKiralikSozlesmeTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[3];

            sqller[0] = $"UPDATE ButceHazirlik.dbo.BHFTASITBILGIDETAY SET BULUNDUGUSAYMANLIK={VeriErisim.DegerYap(yeniSaymanlikKod)}, BULUNDUGUBIRIM={VeriErisim.DegerYap(yeniBirimKod)} WHERE BULUNDUGUSAYMANLIK={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BULUNDUGUBIRIM={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE ButceHazirlik.dbo.BHFTASITBILGIDETAY SET AITOLDUGUSAYMANLIK={VeriErisim.DegerYap(yeniSaymanlikKod)}, AITOLDUGUBIRIM={VeriErisim.DegerYap(yeniBirimKod)} WHERE AITOLDUGUSAYMANLIK={VeriErisim.DegerYap(eskiSaymanlikKod)} AND AITOLDUGUBIRIM={VeriErisim.DegerYap(eskiBirimKod)}";
            sqller[1] = $"UPDATE ButceHazirlik.dbo.BHFKIRALIKARACSOZLESMESI SET SAYMANLIKNO={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE SAYMANLIKNO={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc CesitliOdemelerTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[1];

            sqller[0] = $"UPDATE HarcamaYonetimi.dbo.PSLCESITLIODEMEBORDRO SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc OdenekGondermeleriTasi(int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod)
        {
            string[] sqller = new string[1];

            sqller[0] = $"UPDATE ButceIslemleri.dbo.MUHGONDERMETENKISFORM SET MUHASEBEKOD={VeriErisim.DegerYap(yeniSaymanlikKod)}, BIRIMKOD={VeriErisim.DegerYap(yeniBirimKod)} WHERE MUHASEBEKOD={VeriErisim.DegerYap(eskiSaymanlikKod)} AND BIRIMKOD={VeriErisim.DegerYap(eskiBirimKod)}";

            return CokluSQLCalistir(sqller);
        }

        public Sonuc MasrafPlaniTasi(string eskiHarcamaBirimi, string yeniHarcamaBirimi)
        {
            string[] sqller = new string[3];
            sqller[0] = $"UPDATE ButceIslemleri.dbo.MPGMASRAFPLANIDURUM SET KURUMSALYAPIKOD={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE KURUMSALYAPIKOD={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            sqller[1] = $"UPDATE ButceIslemleri.dbo.MPGMASRAFPLANITARIHCE SET KURUMSALYAPIKOD={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE KURUMSALYAPIKOD={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            sqller[2] = $"UPDATE ButceIslemleri.dbo.MPGMASRAFPLANITERTIP  SET KURUMSALYAPIKOD={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE KURUMSALYAPIKOD={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            return CokluSQLCalistir(sqller);
        }

        public Sonuc MasrafPlaniTavanTasi(string eskiHarcamaBirimi, string yeniHarcamaBirimi)
        {
            string[] sqller = new string[1];
            sqller[0] = $"UPDATE ButceHazirlikV2.dbo.MPTAVAN SET KYKOD={VeriErisim.DegerYap(yeniHarcamaBirimi)} WHERE KYKOD={VeriErisim.DegerYap(eskiHarcamaBirimi)}";
            return CokluSQLCalistir(sqller);
        }
    }
}
