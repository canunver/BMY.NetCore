using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCBORDROTANIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCBORDROTANIM : TABOrtak
    {
        /// <summary>
        /// TABHRCBORDROTANIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCBORDROTANIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }


        /*
CREATE TABLE HRCBORDROTANIM
    (BORDROTANIM INT
    , BORDROISLEM INT
    , DEGISKENFORMULAD VARCHAR(80)
    , DEGISKENTIP INT
    , DEGISKENTUR INT
    , FORMUL VARCHAR(4000)
    , ALACAKHESAP VARCHAR(64)
    , BORCHESAP VARCHAR(64)
    , KISAETIKET VARCHAR(15)
    , UZUNETIKET VARCHAR(100)
    , SIRANO VARCHAR(10)
    , GENISLIK INT
    PRIMARY KEY (BORDROTANIM, DEGISKENFORMULAD, BORDROISLEM)
)
         
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORDROTANIM";
        public static string TABLOADSABLON = "HRCBORDROSABLON";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANA = "HRCBORDROTANIMANA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// BORDROTANIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORDROTANIM = 0,

            /// <summary>
            /// BORDROISLEM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORDROISLEM = 1,

            /// <summary>
            /// DEGISKENFORMULAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENFORMULAD = 2,

            /// <summary>
            /// DEGISKENTIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENTIP = 3,

            /// <summary>
            /// DEGISKENTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENTUR = 4,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMUL = 5,
            ALACAKHESAP = 6,
            BORCHESAP = 7,
            KISAETIKET = 8,
            UZUNETIKET = 9,
            SIRANO = 10,
            GENISLIK = 11,
            ORTAK = 12,
            NOKTASAY = 13,
            ILKDEGER = 14
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANA : int
        {
            /// <summary>
            /// BORDROTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORDROTUR = 0,

            /// <summary>
            /// BORDROISLEMTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORDROISLEMTUR = 1,

            /// <summary>
            /// BORDROAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORDROAD = 2,

            /// <summary>
            /// KAYNAKBORDROTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYNAKBORDROTUR = 3,

            /// <summary>
            /// KAYNAKBORDROISLEMTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYNAKBORDROISLEMTUR = 4,

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "BORDROTANIM"
            , "BORDROISLEM"
            , "DEGISKENFORMULAD"
            , "DEGISKENTIP"
            , "DEGISKENTUR"
            , "FORMUL"
            , "ALACAKHESAP"
            , "BORCHESAP"
            , "KISAETIKET"
            , "UZUNETIKET"
            , "SIRANO"
            , "GENISLIK"
            , "ORTAK"
            , "NOKTASAY"
            , "ILKDEGER"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANA = {
            "BORDROTUR"
            , "BORDROISLEMTUR"
            , "BORDROAD"
            , "KAYNAKBORDROTUR"
            , "KAYNAKBORDROISLEMTUR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroTanim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int bordroTur, int bordroIslemTur)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROTANIM], "=", bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROISLEM], "=", (int)bordroIslemTur));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BordroTanim yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int bordroTur, int islemTur, DegiskenFormul form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(bordroTur);
            sql += "," + VeriErisim.DegerYap((int)islemTur);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap((int)form.tip);
            sql += "," + VeriErisim.DegerYap((int)form.giris);
            sql += "," + VeriErisim.DegerYap(form.formula);
            sql += "," + VeriErisim.DegerYap(form.alacakHesap);
            sql += "," + VeriErisim.DegerYap(form.borcHesap);
            sql += "," + VeriErisim.DegerYap(form.kisaEtiket);
            sql += "," + VeriErisim.DegerYap(form.uzunEtiket);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.genislik);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(form.noktaSay);
            sql += "," + VeriErisim.DegerYap(form.ilkDeger);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bordroTur">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroTanim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int bordroTur, int bordroIslemTur, string formulAd, string bordroYapiTurler, string bordroIslemTurler, bool kendisindeVeyaAdinda)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADANA, ALANLARANA) + " FROM " + TABLOAD;
            sql += $" LEFT OUTER JOIN {TABLOADANA} ON {TABLOADANA}.{ALANLARANA[(int)ENUMAlanlarANA.BORDROTUR]}={TABLOAD}.{ALANLAR[(int)ENUMAlanlar.BORDROTANIM]} AND {TABLOADANA}.{ALANLARANA[(int)ENUMAlanlarANA.BORDROISLEMTUR]}={TABLOAD}.{ALANLAR[(int)ENUMAlanlar.BORDROISLEM]} ";
            string kosul = "";
            if (bordroTur > 0)
            {
                string icKosul = "";
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORDROTANIM], "=", bordroTur));
                if (kendisindeVeyaAdinda)
                {
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarANA.BORDROAD], "LIKE", bordroTur.ToString() + ",%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarANA.BORDROAD], "LIKE", "%," + bordroTur.ToString() + ",%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarANA.BORDROAD], "LIKE", "%," + bordroTur.ToString()));
                }
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }
            else if (!string.IsNullOrWhiteSpace(bordroYapiTurler))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IN_KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BORDROTANIM], bordroYapiTurler, false, false));

            if (bordroIslemTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORDROISLEM], "=", bordroIslemTur));
            else if (!string.IsNullOrWhiteSpace(bordroIslemTurler))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IN_KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BORDROISLEM], bordroIslemTurler, false, false));

            if (!string.IsNullOrWhiteSpace(formulAd))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DEGISKENFORMULAD], "=", formulAd));
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            if (!string.IsNullOrWhiteSpace(formulAd))
                sql += $" ORDER BY {TABLOAD}.BORDROTANIM, {TABLOAD}.BORDROISLEM";
            else
                sql += $" ORDER BY {TABLOAD}.BORDROTANIM, {TABLOAD}.BORDROISLEM, {TABLOAD}.SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bordroTur">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroTanim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleAdlar(int bordroIslemTur)
        {
            string sql = "SELECT DISTINCT DEGISKENFORMULAD FROM " + TABLOAD;
            if (bordroIslemTur > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROISLEM], "=", bordroIslemTur));
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BordroKisiListele(BordroYapi by, string siralama, BordroKistas bordroKistas, string kurumKod)
        {
            string sql = "SELECT " + ToplamYap(by.VTTabloAd, "TCKIMLIKNO", VariableType.String, by.toplamAl);
            for (int i = 0; i < by.hepsi.Length; i++)
            {
                sql += ", " + ToplamYap(by.VTTabloAd, by.hepsi[i].ad, by.hepsi[i].tip, by.toplamAl);
            }
            sql += ", " + ToplamYap(by.VTTabloAd, "SATIRNO", VariableType.Integer, by.toplamAl);

            if (!by.toplamAl)
            {
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.ODEMETARIH];
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.ONCBORDROTARIH];
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.YIL];
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.SURECNO];
                sql += ", HRCSUREC.MUHASEBEKOD, HRCSUREC.BIRIMKOD, HRCMUHASEBEVIEW.MUHASEBEAD, HRCMUHASEBEBIRIMVIEW.BIRIMAD";
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.HAZIRLAYAN];
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.ONAYASUNAN];
                sql += ", " + "HRCBORDRODONEM." + ALANLARDonem[(int)ENUMAlanlarDonem.ONAYLAYAN];
            }

            sql += " FROM " + by.VTTabloAd + " LEFT OUTER JOIN HRCBORDRODONEM ON " + VeriErisim.Join("HRCBORDRODONEM", by.VTTabloAd, new string[] { "YIL", "SURECNO" });
            sql += " LEFT OUTER JOIN HRCSUREC ON " + VeriErisim.Join("HRCSUREC", by.VTTabloAd, new string[] { "YIL", "SURECNO" });
            sql += " INNER JOIN HRCMUHASEBEVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEVIEW.MUHASEBEKOD";
            sql += " INNER JOIN HRCMUHASEBEBIRIMVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD AND HRCSUREC.BIRIMKOD = HRCMUHASEBEBIRIMVIEW.BIRIMKOD AND HRCMUHASEBEBIRIMVIEW.KURKOD LIKE " + VeriErisim.DegerYap(kurumKod.Replace(".", "") + "%");

            bool surecOnayEkle = false;
            string kosul = "";
            if (bordroKistas.yil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(by.VTTabloAd, "YIL", "=", bordroKistas.yil));
                if (!string.IsNullOrWhiteSpace(bordroKistas.surecNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(by.VTTabloAd, "SURECNO", "=", bordroKistas.surecNo));
                else
                    surecOnayEkle = true;
            }
            else
                surecOnayEkle = true;
            if (surecOnayEkle)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<>", 9));

            if (bordroKistas.tcler != null)
            {
                string tcSorgu = "";
                foreach (object item in bordroKistas.tcler)
                {
                    int satirNo = 0;
                    string tc = "";
                    if (item is IntKodStrAd)
                    {
                        satirNo = ((IntKodStrAd)item).kod;
                        tc = ((IntKodStrAd)item).ad;
                    }
                    else if (item is string)
                        tc = (string)item;
                    if (!string.IsNullOrWhiteSpace(tc))
                    {
                        string icSql = VeriErisim.KosulOl(by.VTTabloAd, "TCKIMLIKNO", "=", tc);
                        if (satirNo > 0)
                            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(by.VTTabloAd, "SATIRNO", "=", satirNo));
                        if (tcSorgu != "") tcSorgu = " OR ";
                        tcSorgu += "(" + icSql + ")";
                    }
                }
                if (!string.IsNullOrWhiteSpace(tcSorgu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + tcSorgu + ")");
            }
            else if (by.filter != null)
            {
                foreach (StrKodStrAd kod in by.filter)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(by.VTTabloAd, kod.kod, "LIKE", "%" + kod.ad + "%"));
                }
            }

            if (!TNSDateTime.IsNull(bordroKistas.verilenTarih))
            {
                string alanAd;

                if (by.bordroIslemTur == 2)
                    alanAd = ALANLARDonem[(int)ENUMAlanlarDonem.HAZIRLAMATARIH];
                else
                    alanAd = ALANLARDonem[(int)ENUMAlanlarDonem.ONCBORDROTARIH];

                string icIslec;
                if (TNSDateTime.IsNull(bordroKistas.verilenBitisTarih))
                    icIslec = "=";
                else
                {
                    icIslec = ">=";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDonem, alanAd, "<", bordroKistas.verilenBitisTarih));
                }
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDonem, alanAd, icIslec, bordroKistas.verilenTarih));

            }

            if (bordroKistas.odemeYil > 0 && bordroKistas.odemeAy > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "DatePart(yy, " + TABLOADDonem + "." + ALANLARDonem[(int)ENUMAlanlarDonem.ODEMETARIH] + ")", "=", bordroKistas.odemeYil));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "DatePart(m, " + TABLOADDonem + "." + ALANLARDonem[(int)ENUMAlanlarDonem.ODEMETARIH] + ")", "=", bordroKistas.odemeAy));
            }

            if (bordroKistas.donemYil > 0 && bordroKistas.donemAy > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "DatePart(yy, " + TABLOADDonem + "." + ALANLARDonem[(int)ENUMAlanlarDonem.ONCBORDROTARIH] + ")", "=", bordroKistas.donemYil));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "DatePart(m, " + TABLOADDonem + "." + ALANLARDonem[(int)ENUMAlanlarDonem.ONCBORDROTARIH] + ")", "=", bordroKistas.donemAy));
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            if (!by.toplamAl)
            {
                sql += " ORDER BY " + TABLOADDonem + "." + ALANLARDonem[(int)ENUMAlanlarDonem.ODEMETARIH] + ", HRCSUREC.BIRIMKOD, ";
                if (!string.IsNullOrWhiteSpace(siralama))
                    sql += siralama;
                else
                    sql += by.VTTabloAd + ".TCKIMLIKNO, " + by.VTTabloAd + ".SATIRNO";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BordroKisiBorcuBilgiListele(int personelTip)
        {
            string sql = $"SELECT ID, PERSONELTIP, ALANAD, DEGISKENAD, TUR, HEP30 FROM HRCBORDROKISIBORCUTANIM WHERE PERSONELTIP={VeriErisim.DegerYap(personelTip)}";
            return veriErisim.DataReaderDondur(sql);
        }

        private string ToplamYap(string tabloAd, string alanAd, VariableType variableType, bool toplamAl)
        {
            if (!string.IsNullOrWhiteSpace(tabloAd)) alanAd = tabloAd + "." + alanAd;
            if (!toplamAl) return alanAd;
            if (variableType == VariableType.Money || variableType == VariableType.Integer)
            {
                return "SUM(" + alanAd + ")";
            }
            return "null";
        }

        public Sonuc BordroKaydet(int yil, string surecNo, int satirNo, BordroYapi bordroYapi, object[] degerler, double[] matrahlar)
        {
            if (bordroYapi.hepsi.Length + 1 <= degerler.Length)
            {
                string sql = "";
                for (int i = 0; i < matrahlar.Length; i++)
                {
                    matrahlar[i] = 0;
                }
                try
                {
                    string alanlar = "INSERT INTO " + bordroYapi.VTTabloAd + "(YIL, SURECNO, SATIRNO, TCKIMLIKNO";
                    string values = VeriErisim.DegerYap(yil) + "," + VeriErisim.DegerYap(surecNo) + "," + VeriErisim.DegerYap(satirNo) + "," + VeriErisim.DegerYap((string)degerler[0]);
                    for (int i = 1; i < bordroYapi.hepsi.Length + 1; i++)
                    {
                        DegiskenFormul tut = bordroYapi.hepsi[i - 1];
                        alanlar += ", " + tut.ad;
                        values += ", ";
                        int mNo = -1;

                        for (int j = 0; j < matrahlar.Length; j++)
                        {
                            if (tut.siraNo.IndexOf($"[M{j + 1}]") >= 0) { mNo = j; break; }
                        }

                        if (mNo >= 0) matrahlar[mNo] += (double)degerler[i];

                        if (degerler[i] == null) values += "null";
                        else if (tut.tip == VariableType.Integer) values += VeriErisim.DegerYap((int)degerler[i]);
                        else if (tut.tip == VariableType.Money) values += VeriErisim.DegerYap((double)degerler[i]);
                        else if (tut.tip == VariableType.Date) values += VeriErisim.DegerYap(new TNSDateTime((DateTime)degerler[i]));
                        else values += VeriErisim.DegerYap((string)degerler[i]);
                        if (tut.ad == "hata")
                        {
                            string icHata = (string)degerler[i];
                            if (!string.IsNullOrWhiteSpace(icHata))
                            {
                                return new Sonuc(icHata);
                            }
                        }
                    }
                    sql = alanlar + ") VALUES (" + values + ")";
                    return this.KomutCalistir(sql, 1);
                }
                catch (Exception ex)
                {
                    return new Sonuc(false, string.Format("Kayıt yapılırken verilen komutta ({0}) hata oluştu: {1}!", sql, ex.Message), "");
                }
            }
            return new Sonuc(false, string.Format("Tanımlı bordro {0} alandan oluşurken gelen veriler {1} adet.", bordroYapi.hepsi.Length + 1, degerler.Length), "");
        }

        public Sonuc BordroSil(int yil, string surecNo, string VTTabloAd, string tc = null, int satirNo = 0)
        {
            string sql = "DELETE FROM " + VTTabloAd;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            if (!string.IsNullOrWhiteSpace(tc))
            {
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SATIRNO", "=", satirNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", tc));
            }
            return KomutCalistir(sql, 0);
        }

        public int BordroSiraBul(int yil, string surecNo, string VTTabloAd)
        {
            string sql = "SELECT MAX(SATIRNO) FROM " + VTTabloAd;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            int tut = (int)this.veriErisim.TekKolonSQLDbl(sql);
            return tut + 1;
        }

        public Sonuc SablonSil(BordroSablonDosya bsd)
        {
            string sql = "DELETE FROM " + TABLOADSABLON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BORDROTUR", "=", bsd.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTUR", "=", bsd.islemTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DOSYAKOD", "=", bsd.sablonKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SablonGuncelle(BordroSablonDosya bsd)
        {
            string sql = "UPDATE " + TABLOADSABLON;
            sql += " SET " + VeriErisim.AtamaOl("", "SABLONAD", bsd.sablonAd);
            sql += ", " + VeriErisim.AtamaOl("", "SIRANO", bsd.siraNo);

            sql += "," + VeriErisim.AtamaOl("", "TEKKISILIK", bsd.tekKisilik ? 1 : 0);
            sql += "," + VeriErisim.AtamaOl("", "GECERLIKODLAR", bsd.gecerliKodlar);
            sql += "," + VeriErisim.AtamaOl("", "SAYFA", bsd.sayfa ? 1 : 0);
            sql += "," + VeriErisim.AtamaOl("", "ORTAKRAPOR ", bsd.ortakRapor ? 1 : 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BORDROTUR", "=", bsd.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTUR", "=", bsd.islemTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DOSYAKOD", "=", bsd.sablonKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SablonEkle(BordroSablonDosya bsd)
        {
            string sql = "INSERT INTO " + TABLOADSABLON + "(BORDROTUR, ISLEMTUR, DOSYAKOD, SABLONAD, SIRANO, TEKKISILIK, GECERLIKODLAR, SAYFA, ORTAKRAPOR ) VALUES";
            sql += "(" + VeriErisim.DegerYap(bsd.bordroTur);
            sql += "," + VeriErisim.DegerYap(bsd.islemTur);
            sql += "," + VeriErisim.DegerYap(bsd.sablonKod);
            sql += "," + VeriErisim.DegerYap(bsd.sablonAd);

            sql += "," + VeriErisim.DegerYap(bsd.siraNo);
            sql += "," + VeriErisim.DegerYap(bsd.tekKisilik ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(bsd.gecerliKodlar);
            sql += "," + VeriErisim.DegerYap(bsd.sayfa ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(bsd.ortakRapor ? 1 : 0);
            sql += ")";
            return KomutCalistir(sql, 0);
        }

        public IDataReader SablonListele(bool tekKayit, int bordroTur, int islemTur, int dosyaKod)
        {
            string sql = "SELECT BORDROTUR, ISLEMTUR, DOSYAKOD, SABLONAD, SIRANO, TEKKISILIK, GECERLIKODLAR, SAYFA, ORTAKRAPOR FROM " + TABLOADSABLON;
            if (tekKayit)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BORDROTUR", "=", bordroTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTUR", "=", islemTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DOSYAKOD", "=", dosyaKod));
            }
            else
                sql += " ORDER BY SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDonem : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// HAZIRLAMATARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAZIRLAMATARIH = 2,

            /// <summary>
            /// ODEMETARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMETARIH = 3,

            /// <summary>
            /// HAZIRLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAZIRLAYAN = 4,

            /// <summary>
            /// ONAYASUNAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYASUNAN = 5,

            /// <summary>
            /// ONAYLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYLAYAN = 6,

            /// <summary>
            /// ONCBORDROTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONCBORDROTARIH = 7,
            BORDROTUR = 8,
            BORDROISLEM = 9
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDonem = "HRCBORDRODONEM";

        /// <summary>
        /// Tablo ALANLARDonemının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDonem = {
            "YIL"
            , "SURECNO"
            , "HAZIRLAMATARIH"
            , "ODEMETARIH"
            , "HAZIRLAYAN"
            , "ONAYASUNAN"
            , "ONAYLAYAN"
            , "ONCBORDROTARIH"
            , "BORDROTUR"
            , "BORDROISLEM"
        };

        public Sonuc DonemSil(CalisanDinamikBordroDonem form)
        {
            string sql = "DELETE FROM " + TABLOADDonem;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDonem[(int)ENUMAlanlarDonem.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDonem[(int)ENUMAlanlarDonem.SURECNO], "=", form.surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran ALANLARDonemı veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, CalisanDinamikBordroDonem yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DonemEkle(CalisanDinamikBordroDonem form)
        {
            string sql = "INSERT INTO " + TABLOADDonem + "(" + VeriErisim.TumAlanlar("", ALANLARDonem) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.hazirlamaTarih);
            sql += "," + VeriErisim.DegerYap(form.odemeTarih);
            sql += "," + VeriErisim.DegerYap(form.hazirlayan);
            sql += "," + VeriErisim.DegerYap(form.onayaSunan);
            sql += "," + VeriErisim.DegerYap(form.onaylayan);
            sql += "," + VeriErisim.DegerYap(form.bordroDonemBasTarih);
            sql += "," + VeriErisim.DegerYap(form.bordroTur);
            sql += "," + VeriErisim.DegerYap(form.bordroIslem);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader DonemOku(CalisanDinamikBordroDonem form, TNSDateTime tarihKisit)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDonem, ALANLARDonem) + ", HRCSUREC.MUHASEBEKOD, HRCSUREC.BIRIMKOD, HRCMUHASEBEVIEW.MUHASEBEAD, HRCMUHASEBEBIRIMVIEW.BIRIMAD FROM " + TABLOADDonem;
            sql += " INNER JOIN HRCSUREC ON HRCBORDRODONEM.YIL = HRCSUREC.YIL AND HRCBORDRODONEM.SURECNO = HRCSUREC.SURECNO";
            sql += " INNER JOIN HRCMUHASEBEVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEVIEW.MUHASEBEKOD";
            sql += " INNER JOIN HRCMUHASEBEBIRIMVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD AND HRCSUREC.BIRIMKOD = HRCMUHASEBEBIRIMVIEW.BIRIMKOD";
            if (TNSDateTime.IsNull(tarihKisit))
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.SURECNO], "=", form.surecNo));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HRCSUREC", "MUHASEBEKOD", "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "BIRIMKOD", "=", form.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.ONCBORDROTARIH], "=", form.bordroDonemBasTarih));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.BORDROTUR], "=", form.bordroTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.BORDROISLEM], "=", form.bordroIslem));
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.YIL], "<>", form.yil));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOADDonem, ALANLARDonem[(int)ENUMAlanlarDonem.SURECNO], "<>", form.surecNo)) + ")";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BordroMailiGonderEkle(int surecYil, string surecNo, int bordroTur, int bordroIslemTur, string tc, string ePosta, string kullaniciKodu)
        {
            string sql = "INSERT INTO HRCBORDROEPOSTAGONDER(SURECYIL, SURECNO, BORDROTUR, BORDROISLEMTUR, TCKIMLIKNO, EPOSTA, EKLEMEZAMANI, EKLEYEN, SONUCZAMANI, SONUCSTR, ISLEMEALINDI) VALUES";
            sql += "(" + VeriErisim.DegerYap(surecYil);
            sql += "," + VeriErisim.DegerYap(surecNo);
            sql += "," + VeriErisim.DegerYap(bordroTur);
            sql += "," + VeriErisim.DegerYap(bordroIslemTur);
            sql += "," + VeriErisim.DegerYap(tc);
            sql += "," + VeriErisim.DegerYap(ePosta);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader BordroMailBekleyenler()
        {
            //TNSDateTime dt = new TNSDateTime( DateTime.Now.AddMinutes(-20));
            string sql = "SELECT G.SURECYIL, G.SURECNO, G.BORDROTUR, G.BORDROISLEMTUR, G.TCKIMLIKNO, G.EPOSTA, G.EKLEMEZAMANI, P.AD, P.SOYAD, S.ISINADI ";
            sql += "FROM HRCBORDROEPOSTAGONDER G ";
            sql += "INNER JOIN InsanKaynaklari.dbo.IKYPERSONEL P ON G.TCKIMLIKNO = P.TCKIMLIKNO ";
            sql += "INNER JOIN HRCSUREC S ON S.YIL = G.SURECYIL AND S.SURECNO = G.SURECNO ";
            sql += "WHERE G.SONUCZAMANI IS NULL AND(G.ISLEMEALINDI IS NULL) ";
            sql += "ORDER BY G.EKLEMEZAMANI ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc IslemeAlindi(BordroMail bm)
        {
            string sql = "UPDATE HRCBORDROEPOSTAGONDER SET ISLEMEALINDI = " + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.AtamaOl("", "SURECYIL", bm.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "SURECNO", bm.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "BORDROTUR", bm.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "BORDROISLEMTUR", bm.bordroIslemTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "TCKIMLIKNO", bm.tc));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "EKLEMEZAMANI", bm.eklemeZamani));
            return KomutCalistir(sql, 1);
        }

        public Sonuc BordroMailiKapat(BordroMail bm)
        {
            string sql = "UPDATE HRCBORDROEPOSTAGONDER SET ISLEMEALINDI = NULL";
            sql += ", " + VeriErisim.AtamaOl("", "SONUCZAMANI", bm.sonucZamani);
            sql += ", " + VeriErisim.AtamaOl("", "SONUCSTR", bm.sonucStr);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.AtamaOl("", "SURECYIL", bm.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "SURECNO", bm.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "BORDROTUR", bm.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "BORDROISLEMTUR", bm.bordroIslemTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "TCKIMLIKNO", bm.tc));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.AtamaOl("", "EKLEMEZAMANI", bm.eklemeZamani));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroYapi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleAna(BordroYapi form)
        {
            string sql = "UPDATE " + TABLOADANA;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarANA.BORDROAD], form.bordroAd);
            sql += ", " + ALANLARANA[(int)ENUMAlanlarANA.KAYNAKBORDROTUR] + "=" + ((form.kaynakBordroTur <= 0) ? "NULL" : form.kaynakBordroTur.ToString());
            sql += ", " + ALANLARANA[(int)ENUMAlanlarANA.KAYNAKBORDROISLEMTUR] + "=" + ((form.kaynakBordroIslemTur <= 0) ? "NULL" : form.kaynakBordroIslemTur.ToString());
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.BORDROTUR], "=", form.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.BORDROISLEMTUR], "=", form.bordroIslemTur));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BordroYapi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleAna(BordroYapi form)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.bordroTur);
            sql += "," + VeriErisim.DegerYap(form.bordroIslemTur);
            sql += "," + VeriErisim.DegerYap(form.bordroAd);
            sql += "," + ((form.kaynakBordroTur <= 0) ? "NULL" : VeriErisim.DegerYap(form.kaynakBordroTur));
            sql += "," + ((form.kaynakBordroIslemTur <= 0) ? "NULL" : VeriErisim.DegerYap(form.kaynakBordroIslemTur));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroYapi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuAna(BordroYapi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADANA, ALANLARANA) + " FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.BORDROTUR], "=", form.bordroTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.BORDROISLEMTUR], "=", form.bordroIslemTur));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OncekiDonemBul(int surecYil, string surecNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDonem, ALANLARDonem) + ", HRCSUREC.MUHASEBEKOD, HRCSUREC.BIRIMKOD, HRCMUHASEBEVIEW.MUHASEBEAD, HRCMUHASEBEBIRIMVIEW.BIRIMAD FROM " + TABLOADDonem;
            sql += $" INNER JOIN {TABLOADDonem} S ON S.YIL = {VeriErisim.DegerYap(surecYil)} AND S.SURECNO = {VeriErisim.DegerYap(surecNo)}";
            sql += " INNER JOIN HRCSUREC ON HRCBORDRODONEM.YIL = HRCSUREC.YIL AND HRCBORDRODONEM.SURECNO = HRCSUREC.SURECNO AND HRCSUREC.SURECDURUM < 6";
            sql += " INNER JOIN HRCMUHASEBEVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEVIEW.MUHASEBEKOD";
            sql += " INNER JOIN HRCMUHASEBEBIRIMVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD AND HRCSUREC.BIRIMKOD = HRCMUHASEBEBIRIMVIEW.BIRIMKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", $"{TABLOADDonem}.BORDROTUR = S.BORDROTUR");
            sql = VeriErisim.KosulEkle(sql, "AND", $"{TABLOADDonem}.BORDROISLEM = S.BORDROISLEM");
            sql = VeriErisim.KosulEkle(sql, "AND", $"{TABLOADDonem}.ONCBORDROTARIH  = (SELECT MAX(ONCBORDROTARIH) FROM HRCBORDRODONEM IC INNER JOIN HRCSUREC ICS ON ICS.YIL=IC.YIL AND ICS.SURECNO=IC.SURECNO AND ICS.SURECDURUM<6 WHERE IC.ONCBORDROTARIH  < S.ONCBORDROTARIH) ");
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
