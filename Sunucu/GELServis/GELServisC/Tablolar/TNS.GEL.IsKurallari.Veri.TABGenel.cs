using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABGenel : TABOrtak
    {
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADPARABIRIMI = "HRCPARABIRIMI";
        enum ENUMAlanlarParaBirimi
        {
            KOD = 0,
            ADI = 1,
            KISALTMA = 2,
        }
        static string[] ALANLARPARABIRIMI = { "KOD", "ADI", "KISALTMA" };
        public IDataReader ParaBirimiListele(TNS.BKD.ParaBirimi parabirimi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARPARABIRIMI) + " FROM " + TABLOADPARABIRIMI;

            if (parabirimi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD], "=", parabirimi.kod));

            if (parabirimi.adi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI], " LIKE ", "%" + parabirimi.adi + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD];
            return veriErisim.DataReaderDondur(sql);
        }


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

        public static int FormDurumNoVer(string yeniDurum)
        {
            return yeniDurum == "Onay" ? (int)TNS.HRC.ENUMBelgeDurumu.ONAYLI :
            yeniDurum == "İptal" ? (int)TNS.HRC.ENUMBelgeDurumu.IPTAL :
            yeniDurum == "OnayKaldir" ? (int)TNS.HRC.ENUMBelgeDurumu.ONAYKALDIR :
            (int)TNS.HRC.ENUMBelgeDurumu.TANIMSIZ;
        }


       


    }
}
