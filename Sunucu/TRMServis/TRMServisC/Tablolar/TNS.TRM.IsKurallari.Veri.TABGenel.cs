using System;
using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string BaglantiSatiri = "BaglantiSatiriTRM";

        /// <summary>
        ///  Parametre olarak verilen veriErisim nesnesine göre, TABGenel sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; yeni belge numarası oluşturan ve, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam. 
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <param name="sayacFormat">Sayaç formatını içeren nese</param>
        /// <returns>Belge numarası string olarak döndürülür.</returns>
        public string BelgeNoAl(string sayacKod, string sayacFormat)
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
        /// <returns>Belge numarası string olarak döndürülür.</returns>
        public int BelgeNoAlInt(string sayacKod, string sayacFormat)
        {
            return OrtakFonksiyonlar.ConvertToInt(BelgeNoAl(sayacKod, sayacFormat), 0);
        }

        public string IlPlakaKodBul(string ilKod)
        {
            string sql = "SELECT PLAKAKOD FROM ORGCOGRAFYA";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, "KOD", "=", ilKod.Replace(".", "")));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public string IlKodBul(string plakaKod)
        {
            string sql = "SELECT KOD FROM ORGCOGRAFYA";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, "MERNISKOD", "=", OrtakFonksiyonlar.ConvertToInt(plakaKod, 0)));
            return veriErisim.TekKolonSQLStr(sql);
        }
    }
}
