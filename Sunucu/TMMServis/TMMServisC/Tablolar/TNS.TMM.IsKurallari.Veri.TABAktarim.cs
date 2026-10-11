using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABAktarim : TABOrtak
    {
        public TABAktarim(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASAKTARIM";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            AMBARKOD = 3,
            BELGETARIH = 4,
            ISLEMTIPIKOD = 5,
            BELGETUR = 6,
            NEREDENGELDI = 7,
            ACIKLAMA = 8,
            ISLEMTARIH = 9,
            ISLEMYAPANKOD = 10,
            ISLEMYAPANAD = 11,
            DURUM = 12,
            TOPLAMKAYIT = 13,
            ISLENENKAYIT = 14,
            BELGENO = 15
        }
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "BELGETARIH", "ISLEMTIPIKOD", "BELGETUR", "NEREDENGELDI", "ACIKLAMA", "ISLEMTARIH", "ISLEMYAPANKOD", "ISLEMYAPANAD", "DURUM", "TOPLAMKAYIT", "ISLENENKAYIT", "BELGENO" };

        static string TABLOADDETAY = "TASAKTARIMDETAY";
        public enum ENUMAlanlarDetay : int
        {
            KOD = 0,
            KODANA = 1,
            SIRANO = 2,
            HESAPPLANKOD = 3,
            MIKTAR = 4,
            KDV = 5,
            BIRIMFIYAT = 6,
            ACIKLAMA = 7,
            ESERADI = 8,
            YAZARADI = 9,
            SICILNO = 10,
            DURUM = 11,
            BELGENO = 12
        }
        static string[] ALANLARDETAY = { "KOD", "KODANA", "SIRANO", "HESAPPLANKOD", "MIKTAR", "KDV", "BIRIMFIYAT", "ACIKLAMA", "ESERADI", "YAZARADI", "SICILNO", "DURUM", "BELGENO" };

        static string TABLOADTARIHCE = "TASAKTARIMTARIHCE";
        public enum ENUMAlanlarTarihce : int
        {
            KOD = 0,
            ILGIKOD = 1,
            ISLEMTARIH = 2,
            ACIKLAMA = 3,
        }
        static string[] ALANLARTARIHCE = { "KOD", "ILGIKOD", "ISLEMTARIH", "ACIKLAMA" };

        public Sonuc Kaydet(Aktarim tk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.kod) + ",";
            sql += VeriErisim.DegerYap(tk.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.ambar.kod) + ",";
            sql += VeriErisim.DegerYap(tk.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(tk.islemTipiKod) + ",";
            sql += VeriErisim.DegerYap(tk.belgeTur) + ",";
            sql += VeriErisim.DegerYap(tk.neredenGeldi) + ",";
            sql += VeriErisim.DegerYap(tk.aciklama) + ",";
            sql += VeriErisim.DegerYap(tk.islemTarih) + ",";
            sql += VeriErisim.DegerYap(tk.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(tk.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.toplamKayit) + ",";
            sql += VeriErisim.DegerYap(tk.islenenKayit) + ",";
            sql += VeriErisim.DegerYap(tk.olusanBelgeNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDetay(AktarimDetay tk)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.kod) + ",";
            sql += VeriErisim.DegerYap(tk.kodAna) + ",";
            sql += VeriErisim.DegerYap(tk.siraNo) + ",";
            sql += VeriErisim.DegerYap(tk.hesapPlanKod) + ",";
            sql += VeriErisim.DegerYap(tk.miktar) + ",";
            sql += VeriErisim.DegerYap(tk.kdv) + ",";
            sql += VeriErisim.DegerYap(tk.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(tk.aciklama) + ",";
            sql += VeriErisim.DegerYap(tk.eserAdi) + ",";
            sql += VeriErisim.DegerYap(tk.yazarAdi) + ",";
            sql += VeriErisim.DegerYap(tk.sicilNo) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.belgeNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Aktarim tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], tk.muhasebe.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], tk.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], tk.ambar.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIH], tk.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], tk.islemTipiKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], tk.belgeTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], tk.neredenGeldi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], tk.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], tk.islemTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], tk.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], tk.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMKAYIT], tk.toplamKayit) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLENENKAYIT], tk.islenenKayit) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], tk.olusanBelgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tk.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSayilar(Aktarim tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (tk.toplamKayit > -1)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMKAYIT], tk.toplamKayit) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLENENKAYIT], tk.islenenKayit) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], tk.olusanBelgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tk.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(string kod, int durumKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelleDetay(string kod, int durumKod)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DURUM], durumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(string kodAna)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KODANA], "=", kodAna));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Aktarim tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM TASAKTARIM LEFT OUTER JOIN ";
            sql += " TASMUHASEBE ON TASAKTARIM.MUHASEBEKOD = TASMUHASEBE.KOD";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(tk.muhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));

            if (!string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", tk.harcamaBirimi.kod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tk.ambar.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", tk.ambar.kod));

            if (tk.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", tk.durum));

            if (tk.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", tk.kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY MUHASEBEKOD, HARCAMABIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDetay(AktarimDetay tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;

            if (tk.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", tk.kod));

            if (tk.kodAna != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KODANA], "=", tk.kodAna));

            if (tk.durum > 0)
            {
                if (tk.durum == 2) tk.durum = 0;
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DURUM], "=", tk.durum));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetTarihce(string ilgiKod, string aciklama)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(System.Guid.NewGuid().ToString()) + ",";
            sql += VeriErisim.DegerYap(ilgiKod) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ",";
            sql += VeriErisim.DegerYap(aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilTarihce(string ilgiKod)
        {
            string sql = "DELETE FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ILGIKOD], "=", ilgiKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleTarihce(string ilgiKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE);
            sql += " FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ILGIKOD], "=", ilgiKod));

            sql += " ORDER BY ISLEMTARIH DESC";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}