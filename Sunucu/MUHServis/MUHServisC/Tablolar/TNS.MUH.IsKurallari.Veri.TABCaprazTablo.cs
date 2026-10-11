using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHCaprazTablo veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABCaprazTablo : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABCaprazTablo(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// create table MUHCaprazTablo
        /// (
        /// 	KOD INT, 
        /// 	JSON varchar(MAX),
        /// 	KAYITEDEN varchar(50),
        /// 	KAYITTARIHI DATETIME,
        /// 	primary key (KOD)
        /// )
        /// </summary>
        static string TABLOAD = "MUHCAPRAZTABLO";

        enum ENUMAlanlar
        {
            KOD = 0,
            ADI = 1,
            JSON = 2,
            KAYITEDEN = 3,
            KAYITTARIHI = 4,
            SUTUNTUR = 5
        }
        static string[] ALANLAR = { "KOD", "ADI", "JSON", "KAYITEDEN", "KAYITTARIHI", "SUTUNTUR" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(CaprazTablo bilgi)
        {
            bilgi.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.adi) + ", ";
            sql += VeriErisim.DegerYap(bilgi.json) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ", ";
            sql += VeriErisim.DegerYap(bilgi.sutunTur) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(CaprazTablo bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.JSON], bilgi.json) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUTUNTUR], bilgi.sutunTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(CaprazTablo bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(CaprazTablo bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTablo(CaprazTablo bilgi)
        {
            string kosul = "";
            string sql = "SELECT YIL, BELGENO, HESAPKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, BORC, ALACAK, BIRIMKOD, ALTHESAPKOD FROM MUHODEMEEMRI";

            //WHERE YIL = 2019 AND-- - ek koşullar
            //HESAPKOD IN('1500302', '1500301', '1500304')--SATIR VEYA SÜTUN İÇİN VERİLMİŞ BÜTÜN HESAPKODLARI

            kosul = KosulEkle(bilgi, true);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader ListeleHesapKodTablo(CaprazTablo bilgi)
        {
            string kosul = "";
            string satirSQL = "";
            string sql = "SELECT BELGENO, ISLEMTARIH, HESAPKOD, YEVMIYENO, YEVMIYETARIH, BORC-ALACAK AS BAKIYE, IADSOYAD AS ILGILIADI, IKIMLIKNO AS ILGILINO, 'SATIR'";
            sql += " FROM MUHODEMEEMRI ";
            kosul = KosulEkle(bilgi, true);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            satirSQL = sql;
            satirSQL = "SELECT BELGENO " + satirSQL.Substring(satirSQL.IndexOf("FROM MUHODEMEEMRI"));

            sql += " UNION";
            sql += " SELECT BELGENO, ISLEMTARIH, HESAPKOD, YEVMIYENO, YEVMIYETARIH, BORC-ALACAK AS BAKIYE,'' AS ILGILIADI, '' AS ILGILINO, 'SUTUN'";
            sql += " FROM MUHODEMEEMRI ";
            kosul = KosulEkle(bilgi, false);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            sql += " AND BELGENO IN (" + satirSQL + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        private string KosulEkle(CaprazTablo bilgi, bool satir)
        {
            string kosul = "";
            if (bilgi.sorguYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "YIL", "=", bilgi.sorguYil));

            if (bilgi.sorguDurum > 0)
            {
                if (bilgi.sorguDurum == 22)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "FORMDURUM", "<>", 9));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "FORMDURUM", "=", bilgi.sorguDurum));
            }

            if (bilgi.sorguMuhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "MUHASEBEKOD", "=", bilgi.sorguMuhasebeKod));

            if (bilgi.sorguBirimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "BIRIMKOD", "=", bilgi.sorguBirimKod));

            if (!bilgi.sorguTarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "ISLEMTARIH", ">=", bilgi.sorguTarih1));

            if (!bilgi.sorguTarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "ISLEMTARIH", "<=", bilgi.sorguTarih2));

            if (!bilgi.sorguYevmiyeTarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "YEVMIYETARIH", ">=", bilgi.sorguYevmiyeTarih1));

            if (!bilgi.sorguYevmiyeTarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "YEVMIYETARIH", "<=", bilgi.sorguYevmiyeTarih2));

            if (bilgi.sorguAltHesapKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "ALTHESAPKOD", "=", bilgi.sorguAltHesapKod));

            if (bilgi.sorguIlgiliKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "IKIMLIKNO", "=", bilgi.sorguIlgiliKod));

            if (bilgi.sorguKurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "KURKOD", "=", bilgi.sorguKurKod));

            if (bilgi.sorguFonkKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "FONKOD", "=", bilgi.sorguFonkKod));

            if (bilgi.sorguFinansKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "FINKOD", "=", bilgi.sorguFinansKod));

            if (bilgi.sorguEkoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "EKOKOD", "=", bilgi.sorguEkoKod));

            if (bilgi.sorguFaaliyetKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "PEBKOD", "=", bilgi.sorguFaaliyetKod));

            if (satir)
            {
                if (bilgi.sorguSatir != "")
                {
                    string[] kk = bilgi.sorguSatir.Split(',');
                    string icKosul = "";
                    foreach (string item in kk)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", "HESAPKOD", "LIKE", item.Replace("'", "").Trim() + "%"));
                    }

                    if (icKosul != "")
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }
            }
            else if (bilgi.sorguSutun != "")
            {
                string[] kk = bilgi.sorguSutun.Split(',');
                string icKosul = "";
                foreach (string item in kk)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", "HESAPKOD", "LIKE", item.Replace("'", "").Trim() + "%"));
                }

                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }

            return kosul;
        }
    }
}