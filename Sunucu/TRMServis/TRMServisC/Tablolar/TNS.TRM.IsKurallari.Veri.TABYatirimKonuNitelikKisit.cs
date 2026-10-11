using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimKonuNitelikKisit : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimKonuNitelikKisit(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEYATIRIMKONUNITELIKKISIT";

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            ETAPNO = 1,
            ILKOD = 2,
            ILCEKOD = 3,
            KONUKOD = 4,
            ALTKONUKOD = 5,
            NITELIKKOD = 6,
        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ETAPNO", "ILKOD", "ILCEKOD", "KONUKOD", "ALTKONUKOD", "NITELIKKOD" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(YatirimKonuNitelikKisit bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], bilgi.etapNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], bilgi.ilKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], bilgi.ilceKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONUKOD], bilgi.konuKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTKONUKOD], bilgi.altKonuKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NITELIKKOD], bilgi.nitelikKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilceKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.konuKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.altKonuKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.nitelikKod) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YatirimKonuNitelikKisit bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(YatirimKonuNitelikKisit bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", ORGCOGRAFYA.AD AS IlAdi";
            sql += ", ORGCOGRAFYA_1.AD AS IlceAdi ";
            sql += ", TRMYATIRIMKONU.KISAADI + ' - ' + TRMYATIRIMKONU.ADI AS KonuAdi";
            sql += ", TRMYATIRIMKONU_1.KISAADI + ' - ' + TRMYATIRIMKONU_1.ADI AS AltKonuAdi";
            sql += ", TRMYATIRIMNITELIK.ADI AS NitelikAdi ";
            sql += " FROM TRMYATIRIMKONU ";
            sql += " RIGHT OUTER JOIN TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ";
            sql += " RIGHT OUTER JOIN TRMHIBEYATIRIMKONUNITELIKKISIT ";
            sql += " LEFT OUTER JOIN TRMYATIRIMNITELIK ON TRMHIBEYATIRIMKONUNITELIKKISIT.NITELIKKOD = TRMYATIRIMNITELIK.KOD";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIMKONUNITELIKKISIT.ILCEKOD = ORGCOGRAFYA_1.KOD ";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ";
            sql += " ON TRMHIBEYATIRIMKONUNITELIKKISIT.ILKOD = ORGCOGRAFYA.KOD ";
            sql += " ON TRMYATIRIMKONU_1.KOD = TRMHIBEYATIRIMKONUNITELIKKISIT.ALTKONUKOD ";
            sql += " ON TRMYATIRIMKONU.KOD = TRMHIBEYATIRIMKONUNITELIKKISIT.KONUKOD ";

            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));

            if (bilgi.ilceKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", bilgi.ilceKod));

            if (bilgi.konuKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONUKOD], "=", bilgi.konuKod));

            if (bilgi.altKonuKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTKONUKOD], "=", bilgi.altKonuKod));

            if (bilgi.nitelikKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NITELIKKOD], "=", bilgi.nitelikKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY ETAPNO,IlAdi,IlceAdi,KonuAdi,AltKonuAdi,NITELIKKOD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
