using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// Çeşitli veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kayıt edilen form bilgilerine numara verme işlemini yapan yordam.
        /// </summary>
        /// <param name="sayacKod">Sayaç kod.</param>
        /// <param name="sayacFormat">Sayaç format.</param>
        /// <returns></returns>
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
        /// Parametre olrak verilen kod un sayaç değerini veren yordam
        /// </summary>
        /// <param name="sayacKod">Sayaç kod.</param>
        /// <returns></returns>
        public string SayacNoOgren(string sayacKod)
        {
            string sayacDeger = "";
            string sql = "SELECT SAYAC FROM SAYAC WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (veriErisim.hata == "")
            {
                if (r.Read())
                    sayacDeger = VeriErisim.DegerAl(r, 0, "");

                r.Close();

                if (sayacDeger == "")
                    sayacDeger = "0";
            }

            return sayacDeger;
        }

        /// <summary>
        /// Parametre olarak veilen kod un değerini güncelleyen yordam.
        /// </summary>
        /// <param name="sayacKod">Sayaç kod.</param>
        /// <param name="sayacDeger">Sayaç deger.</param>
        /// <param name="sayacFormat">Sayaç format.</param>
        /// <returns></returns>
        public Sonuc SayacNoGuncelle(string sayacKod, string sayacDeger, string sayacFormat)
        {
            string sql = "";
            sayacDeger = (Convert.ToInt32(sayacDeger)).ToString();
            sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);


            sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
            Sonuc sonuc = KomutCalistir(sql, 0);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);
                sonuc = KomutCalistir(sql, 0);
            }
            sonuc.anahtar = sayacDeger;
            return sonuc;
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
