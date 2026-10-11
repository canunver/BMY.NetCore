using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHDAMGAVERGISIAKTARIMITANIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABVergiDairesiAktarimTurTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABVergiDairesiAktarimTurTanim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHDAMGAVERGISIAKTARIMITANIM";

        enum ENUMAlanlar
        {
            KOD = 0,
            TURKOD = 1,
            ADI = 2,
            HESAPKOD = 3,
            ORANI = 4,
            ISLEMYAPANAD = 5,
            ISLEMYAPANKOD = 6,
            ISLEMTARIHI = 7,
            AKTARIMTURKOD = 8,
        }
        static string[] ALANLAR = { "KOD", "TURKOD", "ADI", "HESAPKOD", "ORANI", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI", "AKTARIMTURKOD" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tanim">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(VergiDairesiAktarimTurTanim tanim)
        {
            tanim.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tanim.kod) + ", ";
            sql += VeriErisim.DegerYap(tanim.turKod) + ", ";
            sql += VeriErisim.DegerYap(tanim.adi) + ", ";
            sql += VeriErisim.DegerYap(tanim.hesapKod) + ", ";
            sql += VeriErisim.DegerYap(tanim.orani) + ", ";
            sql += VeriErisim.DegerYap(tanim.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(tanim.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(tanim.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(tanim.aktarimTurKod) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tanim.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tanim">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(VergiDairesiAktarimTurTanim tanim)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], tanim.turKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], tanim.adi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], tanim.hesapKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORANI], tanim.orani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], tanim.islemYapanAd);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], tanim.islemYapanKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], tanim.islemTarihi);
            if (tanim.kod != 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tanim.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", tanim.hesapKod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tanim.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tanim">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(VergiDairesiAktarimTurTanim tanim)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tanim.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tanim">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(VergiDairesiAktarimTurTanim tanim)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (tanim.kod > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tanim.kod);
            if (tanim.aktarimTurKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKTARIMTURKOD], "=", tanim.aktarimTurKod));
            if (tanim.turKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], "=", tanim.turKod));
            if (!string.IsNullOrEmpty(tanim.adi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", "%" + tanim.adi + "%"));
            if (tanim.orani > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORANI], "=", tanim.orani));
            if (!string.IsNullOrEmpty(tanim.hesapKod))
            {
                if (kosul != "")
                    kosul += " AND ";
                kosul += " (" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPKOD] + " IN (" + Ayir(tanim.hesapKod) + ")) ";
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        private string Ayir(string hesapKodlari)
        {
            string donDeger = "";
            if (string.IsNullOrWhiteSpace(hesapKodlari)) return donDeger;
            string[] birimDizi = hesapKodlari.Split(';');
            for (int i = 0; i < birimDizi.Length; i++)
            {
                if (i > 0) donDeger += ",";
                donDeger += VeriErisim.DegerYap(birimDizi[i]);
            }

            return donDeger;
        }

        public bool HesapKoduVarmi(string hesapKodu)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hesapKodu));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
    }
}