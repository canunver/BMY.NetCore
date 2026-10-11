using System;
using System.Web;
using TNS.HRC.IsKurallari;
using TNS.Veri;
using System.Data;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Tüm veritabanı tablolarında kullanılan ortak fonksiyonları tutan nesne
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen kriterlere göre belge numarası oluşturur ve string olarak döner
        /// </summary>
        /// <param name="sayacKod">Kıstas olarak kullanılacak sayaç kodu parametresi</param>
        /// <param name="sayacFormat">Kıstas olarak kullanılacak sayaç format parametresi</param>
        /// <returns>Belge numarası oluşturur ve string olarak döner</returns>
        public string BelgeNoAl(string sayacKod, string sayacFormat)
        {
            string sayacDeger = "";
            string sql = "";
            bool dosyaYok = true;

            DateTime t = DateTime.Now;
            while (dosyaYok)
            {
                try
                {
                    dosyaYok = SayacDosyaYarat(sayacKod);
                    if (t.AddSeconds(5) < DateTime.Now)//5 saniye dönerse çıksın
                        dosyaYok = false;
                }
                catch { }
            }

            lock (sayacKod)
            {
                sql = "SELECT SAYAC FROM SAYAC WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
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

                    if (!KomutCalistir(sql, 0).islemSonuc)
                    {
                        SayacDosyaSil(sayacKod);
                        return "";
                    }
                }
                else
                {
                    SayacDosyaSil(sayacKod);
                    return "";
                }

                SayacDosyaSil(sayacKod);
                return sayacDeger;
            }
        }

        /// <summary>
        /// Forma ait durum bilgisini int olarak döner
        /// </summary>
        /// <param name="yeniDurum">Form durumu</param>
        /// <returns>Form durumunu int olrak döner</returns>
        public static int FormDurumNoVer(string yeniDurum)
        {
            return yeniDurum == "Onay" ? (int)ENUMBelgeDurumu.ONAYLI :
            yeniDurum == "İptal" ? (int)ENUMBelgeDurumu.IPTAL :
            yeniDurum == "OnayKaldir" ? (int)ENUMBelgeDurumu.ONAYKALDIR :
            (int)ENUMBelgeDurumu.TANIMSIZ;
        }

        bool SayacDosyaYarat(string sayacKod)
        {
            if (System.IO.File.Exists(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod))
                return true;

            try
            {
                System.IO.FileStream fs = System.IO.File.Create(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
                fs.Close();
                return false;
            }
            catch
            {
                return true;
            }
        }

        bool SayacDosyaSil(string sayacKod)
        {
            try
            {
                System.IO.File.Delete(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
