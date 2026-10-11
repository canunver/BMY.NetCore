using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABGenel : TABOrtak
    {
        public TABGenel(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string kosul)
        {
            string sql = "";

            sql = "SELECT MAX(" + alan + ") FROM " + tablo;
            if (kosul != "")
                sql += " WHERE " + kosul;
            double deger = ve.TekKolonSQLDbl(sql);
            return OrtakFonksiyonlar.ConvertToInt((deger + 1).ToString(), 0);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; yeni belge numarası oluşturan ve, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam. 
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <param name="sayacFormat">Sayaç formatını içeren nese</param>
        /// <returns>Sayaç numarası string olarak döndürülür.</returns>
        public string SayacDegerAl(string sayacKod, string sayacFormat)
        {
            string sayacDeger = "";
            string sql = "";

            lock (sayacKod)
            {
                veriErisim.BeginTransaction();
                sql = "SELECT SAYAC FROM SAYAC WITH(XLOCK,ROWLOCK) WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
                IDataReader r = veriErisim.DataReaderDondur(sql);

                if (veriErisim.hata == "")
                {
                    if (r.Read())
                        sayacDeger = VeriErisim.DegerAl(r, 0, "");
                    r.Close();

                    if (sayacDeger == "")
                        sayacDeger = "0";

                    sayacDeger = (Convert.ToInt32(sayacDeger) + 1).ToString();
                    sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);

                    if (Convert.ToInt32(sayacDeger) == 1)
                        sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
                    else
                        sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);

                    if (KomutCalistir(sql, 0).islemSonuc)
                    {
                        veriErisim.Commit();
                        return sayacDeger;
                    }
                }

                veriErisim.Rollback();
            }
            return "";
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; yeni belge numarası oluşturan ve, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam. 
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <param name="sayacFormat">Sayaç formatını içeren nese</param>
        /// <returns>Sayaç numarası string olarak döndürülür.</returns>
        public int SayacDegerAlInt(string sayacKod, string sayacFormat)
        {
            return OrtakFonksiyonlar.ConvertToInt(SayacDegerAl(sayacKod, sayacFormat), 0);
        }

        public static int SayacDegerAlInt(VeriErisim ve, string tabloAd)
        {
            TABGenel tg = new TABGenel(ve);
            return tg.SayacDegerAlInt(tabloAd, "000000000");
        }

        public static string KadroDurumuAktifler(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFYENI)
                             + ")";
        }

        public static string KadroDurumuIptalHaric(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.YENI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AYRILDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AYRILDIHESAPLAMA)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFYENI)
                             + ")";
        }

        public static string KadroDurumuHesaplananlar(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AYRILDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                             + ")";
        }

        public static string KadroDurumuOnaylilar(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AYRILDI)
                             + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                             + ")";
        }

        internal static string KadroDurumuAktifAciklar(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.YENI)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFYENI)
                + ")";
        }

        internal static string KadroDurumuAktifAciklarYenisiz(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                + ")";
        }

        internal static string KadroDurumuTerfiler(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAYIT)
                + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                + ")";
        }

        internal static string KadroDurumuOnaysizlar(string alanAd)
        {
            return alanAd + " IN (" + VeriErisim.DegerYap((int)ENUMKadroDurumu.YENI)
                            + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANIYOR)
                            + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFUCRETSIZIZIN)
                            + ", " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFYENI)
                            + ")";
        }
    }
}
