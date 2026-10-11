using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeYatirimYonetim : TABOrtak
    {
        public TABHibeYatirimYonetim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEYATIRIMYONETIM";
        public static string TABLOADGIRIS = "TRMHIBEYATIRIMYONETIMGIRIS";

        public enum ENUMAlanlar : int
        {
            ETAPNO = 0,
            GERCEK = 1,
            TUZEL = 2,
            TARIH1 = 3,
            TARIH2 = 4,
            AKTIF = 5,
            GIRIS = 6,
            TEBLIGNO = 7,
            ONLINEKAPATMATARIHI = 8,
            RESMIGAZETENO = 9,
            DEGERLENDIRMEKAPATMATARIHI = 10,
            EKSIKBELGEBASTARIH = 11,
            EKSIKBELGEBITTARIH = 12,
            TEBLIGNOALTYAPI = 13,
            RESMIGAZETENOALTYAPI = 14,
            TARIH1ALTYAPI = 15,
            TARIH2ALTYAPI = 16,
            ONLINEKAPATMATARIHIALTYAPI = 17,
            DEGERLENDIRMEKAPATMATARIHIALTYAPI = 18,
            GIRISALTYAPI = 19,
            EKSIKBELGEBASTARIHALTYAPI = 20,
            EKSIKBELGEBITTARIHALTYAPI = 21,
            IZLEMESURESI = 22,
            IZLEMESURESIALTYAPI = 23,
        }

        public enum ENUMAlanlarGiris : int
        {
            ETAPNO = 0,
            ILKOD = 1,
            TARIH = 2
        }

        public static string[] ALANLAR = { "ETAPNO", "GERCEK", "TUZEL", "TARIH1", "TARIH2", "AKTIF", "GIRIS", "TEBLIGNO", "ONLINEKAPATMATARIHI", "RESMIGAZETENO", "DEGERLENDIRMEKAPATMATARIHI", "EKSIKBELGEBASTARIH", "EKSIKBELGEBITTARIH", "TEBLIGNOALTYAPI", "RESMIGAZETENOALTYAPI", "TARIH1ALTYAPI", "TARIH2ALTYAPI", "ONLINEKAPATMATARIHIALTYAPI", "DEGERLENDIRMEKAPATMATARIHIALTYAPI", "GIRISALTYAPI", "EKSIKBELGEBASTARIHALTYAPI", "EKSIKBELGEBITTARIHALTYAPI", "IZLEMESURESI", "IZLEMESURESIALTYAPI" };

        public static string[] ALANLARGIRIS = { "ETAPNO", "ILKOD", "TARIH" };

        public Sonuc Kaydet(Yonetim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (form.aktif > 0)
            {
                sonuc = this.HepsiniPasifYap();
                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.etapNo) + ",";
                sql += VeriErisim.DegerYap(form.gercek) + ",";
                sql += VeriErisim.DegerYap(form.tuzel) + ",";
                sql += VeriErisim.DegerYap(form.tarih1) + ",";
                sql += VeriErisim.DegerYap(form.tarih2) + ",";
                sql += VeriErisim.DegerYap(form.aktif) + ",";
                sql += VeriErisim.DegerYap(form.giris) + ",";
                sql += VeriErisim.DegerYap(form.tebligNo) + ",";
                sql += VeriErisim.DegerYap(form.onlineGirisiKapatmaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.resmiGazeteNo) + ",";
                sql += VeriErisim.DegerYap(form.degerlendirmeKapatmaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.eksikBelgeBasTarihi) + ",";
                sql += VeriErisim.DegerYap(form.eksikBelgeBitTarihi) + ",";

                sql += VeriErisim.DegerYap(form.tebligNoAltyapi) + ",";
                sql += VeriErisim.DegerYap(form.resmiGazeteNoAltyapi) + ",";
                sql += VeriErisim.DegerYap(form.tarih1Altyapi) + ",";
                sql += VeriErisim.DegerYap(form.tarih2Altyapi) + ",";
                sql += VeriErisim.DegerYap(form.onlineGirisiKapatmaTarihiAltyapi) + ",";
                sql += VeriErisim.DegerYap(form.degerlendirmeKapatmaTarihiAltyapi) + ",";
                sql += VeriErisim.DegerYap(form.girisAltyapi) + ",";
                sql += VeriErisim.DegerYap(form.eksikBelgeBasTarihiAltYapi) + ",";
                sql += VeriErisim.DegerYap(form.eksikBelgeBitTarihiAltYapi) + ",";
                sql += VeriErisim.DegerYap(form.izlemeSuresi) + ",";
                sql += VeriErisim.DegerYap(form.izlemeSuresiAltYapi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Yonetim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEK], form.gercek) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUZEL], form.tuzel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], form.tarih1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], form.tarih2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], form.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRIS], form.giris) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEBLIGNO], form.tebligNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONLINEKAPATMATARIHI], form.onlineGirisiKapatmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RESMIGAZETENO], form.resmiGazeteNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEKAPATMATARIHI], form.degerlendirmeKapatmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKSIKBELGEBASTARIH], form.eksikBelgeBasTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKSIKBELGEBITTARIH], form.eksikBelgeBitTarihi) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEBLIGNOALTYAPI], form.tebligNoAltyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RESMIGAZETENOALTYAPI], form.resmiGazeteNoAltyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH1ALTYAPI], form.tarih1Altyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH2ALTYAPI], form.tarih2Altyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONLINEKAPATMATARIHIALTYAPI], form.onlineGirisiKapatmaTarihiAltyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEKAPATMATARIHIALTYAPI], form.degerlendirmeKapatmaTarihiAltyapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKSIKBELGEBASTARIHALTYAPI], form.eksikBelgeBasTarihiAltYapi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKSIKBELGEBITTARIHALTYAPI], form.eksikBelgeBitTarihiAltYapi) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZLEMESURESI], form.izlemeSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZLEMESURESIALTYAPI], form.izlemeSuresiAltYapi) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRISALTYAPI], form.girisAltyapi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(YonetimGiris form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADGIRIS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.TARIH], form.tarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADGIRIS + " (" + VeriErisim.TumAlanlar("", ALANLARGIRIS) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.etapNo) + ",";
                sql += VeriErisim.DegerYap(form.ilKod) + ",";
                sql += VeriErisim.DegerYap(form.tarih) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader Listele(Yonetim form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            if (form.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", form.aktif));

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY 1 DESC";

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader Listele(YonetimGiris form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGIRIS) + " FROM " + TABLOADGIRIS;
            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGIRIS, ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            if (!string.IsNullOrEmpty(form.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGIRIS, ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(Yonetim form)
        {
            Sonuc sonuc = this.Sil(new YonetimGiris() { etapNo = form.etapNo });
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Sil(YonetimGiris form)
        {
            string sql = "DELETE FROM " + TABLOADGIRIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            if (!string.IsNullOrEmpty(form.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            return KomutCalistir(sql, 0);
        }

        private Sonuc HepsiniPasifYap()
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], 0);
            return KomutCalistir(sql, 0);
        }
        private int EtapNoAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.ETAPNO] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public string IlKoduVer(string basvuruNo)
        {
            string sql = "SELECT ILKOD  FROM TRMHIBEYATIRIM WHERE BASVURUNO='" + basvuruNo + "'";
            return veriErisim.TekKolonSQLStr(sql);
        }
    }
}