using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABOrtakDosyalar : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrak sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABOrtakDosyalar(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRORTAKDOSYALAR";
        public static string[] ALANLAR = { "KOD", "ANAKOD", "ADI", "TURU", "BOYUT", "FIZIKSELDOSYAID", "OLUSTURAN", "OLUSTURANAD", "OLUSTURMATARIHI", "DEGISTIREN", "DEGISTIRENAD", "DEGISTIRMETARIHI", "BAGLANTIADRESI", "YOLKOD", "YOLADI" };
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ANAKOD = 1,
            ADI = 2,
            TURU = 3,
            BOYUT = 4,
            FIZIKSELDOSYAID = 5,
            OLUSTURAN = 6,
            OLUSTURANAD = 7,
            OLUSTURMATARIHI = 8,
            DEGISTIREN = 9,
            DEGISTIRENAD = 10,
            DEGISTIRMETARIHI = 11,
            BAGLANTIADRESI = 12,
            YOLKOD = 13,
            YOLADI = 14
        }

        public static string TABLOADYETKI = "EVRORTAKDOSYALARYETKI";
        public static string[] ALANLARYETKI = { "KOD", "DOSYAKOD", "YETKIVERILEN", "YETKIVEREN", "YETKIVERILENAD", "YETKIVERENAD", "GORME", "EKLE", "EPOSTAGONDER", "ADDEGISTIR", "INDIR", "KOPYALA", "TASIMA", "SILME", "SURUM", "YETKI" };
        public enum ENUMAlanlarYetki : int
        {
            KOD = 0,
            DOSYAKOD = 1,
            YETKIVERILEN = 2,
            YETKIVEREN = 3,
            YETKIVERILENAD = 4,
            YETKIVERENAD = 5,
            GORME = 6,
            EKLE = 7,
            EPOSTAGONDER = 8,
            ADDEGISTIR = 9,
            INDIR = 10,
            KOPYALA = 11,
            TASIMA = 12,
            SILME = 13,
            SURUM = 14,
            YETKI = 15
        }

        public static string TABLOADBIRIM = "EVRORTAKDOSYALARBIRIM";
        public static string[] ALANLARBIRIM = { "ANAKOD", "BIRIMKOD" };
        public enum ENUMAlanlarBirim : int
        {
            ANAKOD = 0,
            BIRIMKOD = 1,
        }

        public static string TABLOADNOT = "EVRORTAKDOSYALARNOTLAR";
        public static string[] ALANLARNOT = { "KOD", "DOSYAKOD", "EKLEYENKISI", "EKLEYENKISIAD", "EKLEMETARIHI", "NOTBILGISI" };
        public enum ENUMAlanlarNot : int
        {
            KOD = 0,
            DOSYAKOD = 1,
            EKLEYENKISI = 2,
            EKLEYENKISIAD = 3,
            EKLEMETARIHI = 4,
            NOTBILGISI = 5
        }

        public static string TABLOADSURUM = "EVRORTAKDOSYALARSURUM";
        public static string[] ALANLARSURUM = { "KOD", "DOSYAKOD", "EKLEYENKISI", "EKLEYENKISIAD", "EKLEMETARIHI", "BOYUT", "FIZIKSELDOSYAID", "ADI", "ACIKLAMA" };
        public enum ENUMAlanlarSurum : int
        {
            KOD = 0,
            DOSYAKOD = 1,
            EKLEYENKISI = 2,
            EKLEYENKISIAD = 3,
            EKLEMETARIHI = 4,
            BOYUT = 5,
            FIZIKSELDOSYAID = 6,
            ADI = 7,
            ACIKLAMA = 8
        }

        public static string TABLOADTARIHCE = "EVRORTAKDOSYALARTARIHCE";
        public static string[] ALANLARTARIHCE = { "DOSYAKOD", "ISLEMKOD", "ISLEMAD", "ISLEMTARIHI", "ISLEMIYAPAN", "ISLEMIYAPANAD", "ACIKLAMA" };
        public enum ENUMAlanlarTarihce : int
        {
            //KOD alanı autoincrement tipinde olduğu için buraya konulmuyor
            DOSYAKOD = 0,
            ISLEMKOD = 1,
            ISLEMAD = 2,
            ISLEMTARIHI = 3,
            ISLEMIYAPAN = 4,
            ISLEMIYAPANAD = 5,
            ACIKLAMA = 6
        }

        public Sonuc Kaydet(OrtakDosyalar form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.anaKod) + ",";
            sql += VeriErisim.DegerYap(form.adi) + ",";
            sql += VeriErisim.DegerYap(form.turu) + ",";
            sql += VeriErisim.DegerYap(form.boyut) + ",";
            sql += VeriErisim.DegerYap(form.fizikselDosyaID) + ",";
            sql += VeriErisim.DegerYap(form.olusturan) + ",";
            sql += VeriErisim.DegerYap(form.olusturanAd) + ",";
            sql += VeriErisim.DegerYap(form.olusturmaTarihi) + ",";
            sql += VeriErisim.DegerYap(form.degistiren) + ",";
            sql += VeriErisim.DegerYap(form.degistirenAd) + ",";
            sql += VeriErisim.DegerYap(form.degistirmeTarihi) + ",";
            sql += VeriErisim.DegerYap(form.baglantiAdresi) + ",";
            sql += VeriErisim.DegerYap(form.yolKod) + ",";
            sql += VeriErisim.DegerYap(form.yolAdi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Guncelle(OrtakDosyalar form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], form.anaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], form.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURU], form.turu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOYUT], form.boyut) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIZIKSELDOSYAID], form.fizikselDosyaID) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLUSTURAN], form.olusturan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLUSTURANAD], form.olusturanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLUSTURMATARIHI], form.olusturmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIREN], form.degistiren) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRENAD], form.degistirenAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRMETARIHI], form.degistirmeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAd(OrtakDosyalar form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], form.adi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAnaKod(OrtakDosyalar form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], form.anaKod);
            if (form.yolKod != "")
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YOLKOD], form.yolKod);
            if (form.yolAdi != "")
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YOLADI], form.yolAdi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSurumIcin(OrtakDosyalar form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], form.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURU], form.turu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOYUT], form.boyut) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIZIKSELDOSYAID], form.fizikselDosyaID) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIREN], form.degistiren) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRENAD], form.degistirenAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRMETARIHI], form.degistirmeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public bool KayitliMi(OrtakDosyalar form)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", form.anaKod));

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public IDataReader Listele(OrtakDosyalar form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += ",(SELECT COUNT(*) FROM EVRORTAKDOSYALARYETKI Y WHERE Y.DOSYAKOD=C.KOD)";
            sql += ",(SELECT COUNT(*) FROM EVRORTAKDOSYALARNOTLAR N WHERE N.DOSYAKOD=C.KOD)";

            string kosul = "";
            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            string icKosul = "";
            //kodlar arrayi gönderildi ise
            foreach (int kod in form.kodlar)
            {
                if (kod > 0)
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            }
            if (icKosul != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            //*****************************************************************************

            if (form.anaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", form.anaKod));
            if (form.aramaAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", "%" + form.aramaAdi + "%"));

            //Eğer Paylaşılan ise
            if (form.anaKod == 0 && form.gosterilecekDosyaTuru == (int)ENUMOrtakDosyalarGosterilecekDosyalar.PAYLASILAN && form.olusturan != "")
            {
                kosul = "";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLUSTURAN], "<>", form.olusturan));

                string ekKosul = "KOD IN (SELECT DOSYAKOD FROM EVRORTAKDOSYALARYETKI ";
                ekKosul += " WHERE YETKIVERILEN='*' ";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", "YETKIVERILEN", "=", form.olusturan));

                foreach (string yy in form.yetkiliOlduguBirimler)
                {
                    if (string.IsNullOrEmpty(yy)) continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", "YETKIVERILEN", "LIKE", yy + "%"));
                }
                ekKosul += ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);

                //sql += ",(SELECT YOLU FROM EVRORTAKDOSYALARYOL A WHERE A.KOD=C.KOD AND UZUNYOL=(SELECT MAX(UZUNYOL) FROM EVRORTAKDOSYALARYOL AS B WHERE A.KOD=B.KOD)) AS Yolu";
            }
            else//Kendi dosyalarını görecek ise
            {
                sql += ",ADI AS Yolu";
            }

            sql += " FROM " + TABLOAD + " AS C";
            if (kosul != "") sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TURU] + "," + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public int ListeleBaglantiAdresindenKodBul(string adres)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BAGLANTIADRESI], "=", adres));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public int ListeleAdVarMi(string adi, int anaKod, int dosyaKod)
        {
            string sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "=", adi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", anaKod));
            if (dosyaKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", dosyaKod));
            }

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader ListeleYetki(int[] kodlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYETKI, ALANLARYETKI);
            sql += " FROM " + TABLOADYETKI;

            string kosul = "";
            foreach (int kod in kodlar)
            {
                if (kod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.DOSYAKOD], "=", kod));
            }

            if (kosul != "")
                sql += " WHERE (" + kosul + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            //sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleTarihce(int dosyaKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.DOSYAKOD], "=", dosyaKod));
            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ISLEMTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetTarihce(OrtakDosyalarTarihce form)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.dosyaKod) + ",";
            sql += VeriErisim.DegerYap(form.islemKod) + ",";
            sql += VeriErisim.DegerYap(form.islemAd) + ",";
            sql += VeriErisim.DegerYap(form.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(form.islemiYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemiYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetYetki(OrtakDosyalarYetki form)
        {
            string sql = "INSERT INTO " + TABLOADYETKI + " (" + VeriErisim.TumAlanlar("", ALANLARYETKI) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.dosyaKod) + ",";
            sql += VeriErisim.DegerYap(form.yetkiVerilen) + ",";
            sql += VeriErisim.DegerYap(form.yetkiVeren) + ",";
            sql += VeriErisim.DegerYap(form.yetkiVerilenAd) + ",";
            sql += VeriErisim.DegerYap(form.yetkiVerenAd) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.gorme, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.ekle, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.epostaGonder, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.adDegistir, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.indir, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.kopyala, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.tasima, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.silme, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.surum, 0)) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(form.yetki, 0)) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilYetki(int dosyaKod)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.DOSYAKOD], "=", dosyaKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader DosyaYoluVerUst(int kod)
        {
            string sql = "";
            //1
            //Verilen kodun üst klasörlerini listeler
            sql = " WITH YOL AS (SELECT KOD, ADI, ANAKOD, TURU, BOYUT, FIZIKSELDOSYAID, OLUSTURAN, OLUSTURANAD, OLUSTURMATARIHI, DEGISTIREN, DEGISTIRENAD, DEGISTIRMETARIHI ";
            sql += " ,CAST(ADI AS VARCHAR(2000)) AS YOL ";
            sql += " ,CAST(CONVERT(varchar(2000),kod) AS VARCHAR(2000)) AS KODLAR ";
            sql += " ,CAST(KOD - KOD AS INT) AS SEVIYE ";
            sql += " FROM dbo.EVRORTAKDOSYALAR ";
            sql += " WHERE KOD=" + kod.ToString();
            sql += " UNION ALL ";
            sql += " SELECT T.KOD, T.ADI, T.ANAKOD, T.TURU, T.BOYUT, T.FIZIKSELDOSYAID, T.OLUSTURAN, T.OLUSTURANAD, T.OLUSTURMATARIHI, T.DEGISTIREN, T.DEGISTIRENAD, T.DEGISTIRMETARIHI ";
            sql += " , CAST(T.ADI+ '/' + A.YOL AS VARCHAR(2000)) AS YOL ";
            sql += " , CAST(CONVERT(varchar(2000),T.KOD) + '/' + CONVERT(varchar(2000),A.KODLAR) AS VARCHAR(2000)) AS KODLAR ";
            sql += " , CAST(SEVIYE + 1 AS INT) AS SEVIYE ";
            sql += " FROM dbo.EVRORTAKDOSYALAR AS T INNER JOIN YOL AS A ON A.ANAKOD = T.KOD AND A.KOD <> T.ANAKOD) ";
            sql += " SELECT TOP (100) PERCENT KOD, ADI, ANAKOD, TURU, BOYUT, FIZIKSELDOSYAID, OLUSTURAN, OLUSTURANAD, OLUSTURMATARIHI, DEGISTIREN, DEGISTIRENAD, DEGISTIRMETARIHI, YOL, SEVIYE, KODLAR FROM YOL AS YOL_1 ";
            sql += " WHERE (SEVIYE =(SELECT MAX(SEVIYE) FROM YOL AS B WHERE (KOD = YOL_1.KOD))) ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DosyaYoluVerAlt(int kod)
        {
            string sql = "";
            //2
            //Verilen kodun üst klasörlerini listeler
            sql = " WITH YOL AS (SELECT KOD, ADI, ANAKOD, TURU, BOYUT, FIZIKSELDOSYAID, OLUSTURAN, OLUSTURANAD, OLUSTURMATARIHI, DEGISTIREN, DEGISTIRENAD, DEGISTIRMETARIHI ";
            sql += " ,CAST(ADI AS VARCHAR(2000)) AS YOL ";
            sql += " ,CAST(CONVERT(varchar(2000),kod) AS VARCHAR(2000)) AS KODLAR ";
            sql += " ,CAST(KOD - KOD AS INT) AS SEVIYE ";
            sql += " FROM dbo.EVRORTAKDOSYALAR ";
            sql += " WHERE KOD=" + kod.ToString();
            sql += " UNION ALL ";
            sql += " SELECT T.KOD, T.ADI, T.ANAKOD, T.TURU, T.BOYUT, T.FIZIKSELDOSYAID, T.OLUSTURAN, T.OLUSTURANAD, T.OLUSTURMATARIHI, T.DEGISTIREN, T.DEGISTIRENAD, T.DEGISTIRMETARIHI ";
            sql += " , CAST(T.ADI+ '/' + A.YOL AS VARCHAR(2000)) AS YOL ";
            sql += " , CAST(CONVERT(varchar(2000),T.KOD) + '/' + CONVERT(varchar(2000),A.KODLAR) AS VARCHAR(2000)) AS KODLAR ";
            sql += " , CAST(SEVIYE + 1 AS INT) AS SEVIYE ";
            sql += " FROM dbo.EVRORTAKDOSYALAR AS T INNER JOIN YOL AS A ON A.KOD = T.ANAKOD AND A.ANAKOD <> T.KOD) ";
            sql += " SELECT TOP (100) PERCENT KOD, ADI, ANAKOD, TURU, BOYUT, FIZIKSELDOSYAID, OLUSTURAN, OLUSTURANAD, OLUSTURMATARIHI, DEGISTIREN, DEGISTIRENAD, DEGISTIRMETARIHI, YOL, SEVIYE, KODLAR FROM YOL AS YOL_1 ";
            sql += " WHERE (SEVIYE =(SELECT MAX(SEVIYE) FROM YOL AS B WHERE (KOD = YOL_1.KOD))) ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BirimKaydet(OrtakDosyalarBirim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADBIRIM + " (" + VeriErisim.TumAlanlar("", ALANLARBIRIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.birimKod) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader BirimKayitliMi(OrtakDosyalarBirim form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBIRIM) + " FROM " + TABLOADBIRIM + " WITH(XLOCK,ROWLOCK)";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBirim.BIRIMKOD], "=", form.birimKod));
            return veriErisim.DataReaderDondur(sql);
        }

        #region "Not Bilgileri"
        public Sonuc KaydetNot(OrtakDosyalarNot form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADNOT + " (" + VeriErisim.TumAlanlar("", ALANLARNOT) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.dosyaKod) + ",";
            sql += VeriErisim.DegerYap(form.ekleyenKisi) + ",";
            sql += VeriErisim.DegerYap(form.ekleyenKisiAd) + ",";
            sql += VeriErisim.DegerYap(form.eklemeTarihi) + ",";
            sql += VeriErisim.DegerYap(form.notBilgisi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleNot(OrtakDosyalarNot form)
        {
            string sql = "UPDATE " + TABLOADNOT + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARNOT[(int)ENUMAlanlarNot.DOSYAKOD], form.dosyaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARNOT[(int)ENUMAlanlarNot.EKLEYENKISI], form.ekleyenKisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARNOT[(int)ENUMAlanlarNot.EKLEYENKISIAD], form.ekleyenKisiAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARNOT[(int)ENUMAlanlarNot.EKLEMETARIHI], form.eklemeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARNOT[(int)ENUMAlanlarNot.NOTBILGISI], form.notBilgisi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARNOT[(int)ENUMAlanlarNot.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleNot(int dosyaKod, int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARNOT) + " FROM " + TABLOADNOT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARNOT[(int)ENUMAlanlarNot.DOSYAKOD], "=", dosyaKod));
            if (kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARNOT[(int)ENUMAlanlarNot.KOD], "=", kod));

            sql += " ORDER BY " + ALANLARNOT[(int)ENUMAlanlarNot.EKLEMETARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilNot(int kod)
        {
            string sql = "DELETE FROM " + TABLOADNOT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARNOT[(int)ENUMAlanlarNot.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }
        #endregion

        #region "Surumler"
        public Sonuc KaydetSurum(OrtakDosyalarSurumler form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADSURUM + " (" + VeriErisim.TumAlanlar("", ALANLARSURUM) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.dosyaKod) + ",";
            sql += VeriErisim.DegerYap(form.ekleyenKisi) + ",";
            sql += VeriErisim.DegerYap(form.ekleyenKisiAd) + ",";
            sql += VeriErisim.DegerYap(form.eklemeTarihi) + ",";
            sql += VeriErisim.DegerYap(form.boyut) + ",";
            sql += VeriErisim.DegerYap(form.fizikselDosyaID) + ",";
            sql += VeriErisim.DegerYap(form.adi) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader ListeleSurum(int dosyaKod, int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSURUM) + " FROM " + TABLOADSURUM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURUM[(int)ENUMAlanlarSurum.DOSYAKOD], "=", dosyaKod));
            if (kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURUM[(int)ENUMAlanlarSurum.KOD], "=", kod));

            sql += " ORDER BY " + ALANLARSURUM[(int)ENUMAlanlarSurum.EKLEMETARIHI];

            return veriErisim.DataReaderDondur(sql);
        }


        #endregion

        public double ListeleKullanimBilgisi(string olusturan)
        {
            string sql = "SELECT SUM(BOYUT) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLUSTURAN], "=", olusturan));

            return veriErisim.TekKolonSQLDbl(sql);
        }

    }
}
