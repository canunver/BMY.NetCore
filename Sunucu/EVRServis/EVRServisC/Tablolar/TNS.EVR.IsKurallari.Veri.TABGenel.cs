using System;
using System.Web;
using TNS.EVR.IsKurallari;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak veritabanı ile ilgili genel işlemlerin yapıldığı yordamlarını tutan sınıf.    
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string BaglantiSatiri = "BaglantiSatiriEVR";
        public static string BaglantiSatiriDosyaIcerik = "BaglantiSatiriEVRICERIK";

        /// <summary>
        ///  Parametre olarak verilen veriErisim nesnesine göre, TABGenel sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak hareketinin son durum bilgilerini içeren veri tabanı view bilgisinin adını tutan değişken.
        /// </summary>
        public static string VIEWHAREKETSONDURUM = "EVREVRAKHAREKETSONDURUM";

        /// <summary>
        /// Birim ile ilgili evrak hareketinin son durum bilgilerini içeren veri tabanı view bilgisinin adını tutan değişken.
        /// </summary>
        public static string VIEWHAREKETSONDURUMBIRIM = "EVREVRAKHAREKETSONDURUMBIRIM";

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; yeni belge numarası oluşturan ve, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam. 
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <param name="sayacFormat">Sayaç formatını içeren nese</param>
        /// <returns>Belge numarası string olarak döndürülür.</returns>
        /// 
        public string BelgeNoAl(string sayacKod, string sayacFormat)
        {
            string sayacDeger = "";
            string sql = "";

            lock (sayacKod)
            {
                sql = "SELECT SAYAC FROM SAYAC WITH(XLOCK,ROWLOCK) WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
                IDataReader r = veriErisim.DataReaderDondur(sql);

                if (r != null)
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

                    veriErisim.BeginTransaction();
                    if (KomutCalistir(sql, 0).islemSonuc)
                    {
                        veriErisim.Commit();
                        return sayacDeger;
                    }
                    else
                        veriErisim.Rollback();
                }

            }
            return "";
        }

        //public string BelgeNoAl(string sayacKod, string sayacFormat)
        //{
        //    string sayacDeger = "";
        //    string sql = "";
        //    bool dosyaYok = true;


        //    //if (sayacKod.StartsWith("EVRGIDEN"))
        //    //    OrtakFonksiyonlar.HataStrYaz("no almaya geldi:" + sayacKod);

        //    DateTime t = DateTime.Now;
        //    while (dosyaYok)
        //    {
        //        try
        //        {
        //            dosyaYok = SayacDosyaYarat(sayacKod);
        //            if (t.AddSeconds(5) < DateTime.Now)//5 saniye dönerse çıksın
        //                dosyaYok = false;
        //        }
        //        catch { }
        //    }

        //    lock (sayacKod)
        //    {
        //        sql = "SELECT SAYAC FROM SAYAC WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
        //        IDataReader r = veriErisim.DataReaderDondur(sql);

        //        if (veriErisim.hata == "")
        //        {
        //            if (r.Read())
        //                sayacDeger = VeriErisim.DegerAl(r, 0, "");

        //            r.Close();

        //            if (sayacDeger == "")
        //                sayacDeger = "0";

        //            sayacDeger = (Convert.ToInt32(sayacDeger) + 1).ToString();
        //            sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);

        //            if (Convert.ToInt32(sayacDeger) == 1)
        //                sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
        //            else
        //                sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);

        //            //if (sayacKod.StartsWith("EVRGIDEN"))
        //            //    OrtakFonksiyonlar.HataStrYaz("no almaya alıyor:" + sql);

        //            if (!KomutCalistir(sql, 0).islemSonuc)
        //            {
        //                //if (sayacKod.StartsWith("EVRGIDEN"))
        //                //    OrtakFonksiyonlar.HataStrYaz("no almaya BAŞARISIZ:" + sql + veriErisim.hata);

        //                SayacDosyaSil(sayacKod);
        //                return "";
        //            }
        //        }
        //        else
        //        {
        //            SayacDosyaSil(sayacKod);
        //            return "";
        //        }

        //        SayacDosyaSil(sayacKod);

        //        //if (sayacKod.StartsWith("EVRGIDEN"))
        //        //    OrtakFonksiyonlar.HataStrYaz("no almaya BAŞARILI:" + sayacDeger);

        //        return sayacDeger;
        //    }
        //}

        /// <summary>
        /// Parametre olarak verilen sayaç koduna göre; dosya oluşturan yordam.
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <returns>Dosya daha önce oluşturulmuş ise doğru, yoksa yanlış döndürülür.</returns>
        //bool SayacDosyaYarat(string sayacKod)
        //{
        //    if (System.IO.File.Exists(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod))
        //        return true;

        //    try
        //    {
        //        System.IO.FileStream fs = System.IO.File.Create(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
        //        fs.Close();
        //        return false;
        //    }
        //    catch
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// Parametre olarak verilen sayaç koduna göre; daha önce oluşturulmuş dosyayı silen yordam.
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <returns>Dosya silme işlemi gerçekleşirse doğru, yoksa yanlış döndürülür.</returns>
        //bool SayacDosyaSil(string sayacKod)
        //{
        //    try
        //    {
        //        System.IO.File.Delete(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; yeni paraf numarası oluşturan ve, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam. 
        /// </summary>
        /// <param name="sayacKod">Sayaç kod bilgisi içeren nesne</param>
        /// <param name="sayacFormat">Sayaç formatını içeren nese</param>
        /// <returns>Paraf id numarası int olarak döndürülür.</returns>
        //public int ParafIdAl(string sayacKod, string sayacFormat)
        //{
        //    string sayacDeger = "";
        //    string sql = "";
        //    bool dosyaYok = true;

        //    while (dosyaYok)
        //    {
        //        try
        //        {
        //            dosyaYok = SayacDosyaYarat(sayacKod);
        //        }
        //        catch { }
        //    }

        //    lock (this)
        //    {
        //        sql = "SELECT SAYAC FROM SAYAC WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
        //        IDataReader r = veriErisim.DataReaderDondur(sql);

        //        if (veriErisim.hata == "")
        //        {
        //            if (r.Read())
        //                sayacDeger = VeriErisim.DegerAl(r, 0, "");

        //            r.Close();

        //            if (sayacDeger == "")
        //                sayacDeger = "0";

        //            sayacDeger = (Convert.ToInt32(sayacDeger) + 1).ToString();
        //            sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);

        //            if (Convert.ToInt32(sayacDeger) == 1)
        //                sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
        //            else
        //                sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);

        //            if (!KomutCalistir(sql, 0).islemSonuc)
        //            {
        //                SayacDosyaSil(sayacKod);
        //                return 0;
        //            }
        //        }
        //        else
        //        {
        //            SayacDosyaSil(sayacKod);
        //            return 0;
        //        }

        //        SayacDosyaSil(sayacKod);
        //        return OrtakFonksiyonlar.ConvertToInt(sayacDeger, 0);
        //    }
        //}

        public string DosyaPlaniAdGetir(string dosyaKodu)
        {
            string sql = "SELECT AD FROM EVRDOSYAPLANI ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", dosyaKodu));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public string KullaniciAdGetir(string tcKimlikNo)
        {
            string sql = "SELECT AD+' '+SOYAD FROM Kullanici.dbo.KULLANICI ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERNIS", "=", tcKimlikNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public bool KullaniciAktifMi(string tcKimlikNo)
        {
            string sql = "SELECT AKTIFMI FROM Kullanici.dbo.KULLANICI ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERNIS", "=", tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "AKTIFMI", "=", 1));
            
            return veriErisim.TekKolonSQLInt(sql) == 1 ? true : false;
        }

        public string EvrakBirimAdGetir(string evrakBirimKod)
        {
            string sql = "SELECT AD FROM EVRBIRIM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", evrakBirimKod));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public string EvrakBirimAdGetir(int idariKimlikKod)
        {
            string sql = "SELECT AD FROM EVRBIRIM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "IDAREKIMLIKKODU", "=", idariKimlikKod));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public string EvrakBirimKisaAdGetir(string evrakBirimKod)
        {
            string sql = "SELECT KISAAD FROM EVRBIRIM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", evrakBirimKod));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public string EvrakGelisAdGetir(string kod)
        {
            string sql = "SELECT AD FROM EVRGELISSEKLI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", kod));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public int EvrakTipGetir(string evrEvrakNo)
        {
            string sql = "SELECT EVRAKTIPI FROM EVREVRAK";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVREVRAKNO", "=", evrEvrakNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Max olarak kullanılan yerlerin max değerlerinin sayac'a aktarılması için
        /// </summary>
        public void BelgeNoYaz(string sayacKod, string sayacDeger)
        {
            string sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);
            lock (sayacKod)
            {
                veriErisim.BeginTransaction();
                if (KomutCalistir(sql, 0).islemSonuc)
                    veriErisim.Commit();
                else
                    veriErisim.Rollback();
            }
        }

        public IDataReader SayacListele(string kod)
        {
            string sql = "SELECT * FROM SAYAC ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "LIKE", "%" + kod + "%"));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SayacKaydet(string sayacKod, string sayacDeger)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE SAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);
            lock (sayacKod) { sonuc = KomutCalistir(sql, 1); }
            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO SAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
                lock (sayacKod) { sonuc = KomutCalistir(sql, 1); }
            }

            return sonuc;
        }
    }
}
