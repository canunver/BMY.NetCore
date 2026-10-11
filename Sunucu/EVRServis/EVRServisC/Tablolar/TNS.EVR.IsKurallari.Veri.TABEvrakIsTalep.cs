using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakIsTalep : TABOrtak
    {
        public TABEvrakIsTalep(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKISTALEP";

        public enum ENUMAlanlar : int
        {
            KOD = 0, PREVRAKNO = 1, SERVISKAYITKOD = 2, ATANANKISI = 3, ATANANKISIAD = 4, ISLEMYAPAN = 5, ISLEMYAPANAD = 6, ISLEMTARIH = 7, ISLEM = 8, ISLEMACIKLAMA = 9, DETAY = 10,
        }

        public static string[] ALANLAR = { "KOD", "PREVRAKNO", "SERVISKAYITKOD", "ATANANKISI", "ATANANKISIAD", "ISLEMYAPAN", "ISLEMYAPANAD", "ISLEMTARIH", "ISLEM", "ISLEMACIKLAMA" , "DETAY" };

        public Sonuc Kaydet(EvrakIsTalep form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.servisKayitKod) + ",";
                sql += VeriErisim.DegerYap(form.atananKisi) + ",";
                sql += VeriErisim.DegerYap(form.atananKisiAd) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.islemTarihi) + ",";
                sql += VeriErisim.DegerYap((int)form.islem) + ",";
                sql += VeriErisim.DegerYap(form.islemAciklama) + ",";
                sql += VeriErisim.DegerYap(form.detay) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc Guncelle(EvrakIsTalep form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (form.islem != ENUMEvrakIsTalepIslem.TANIMSIZ)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], (int)form.islem) + ",";
            if (form.servisKayitKod != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SERVISKAYITKOD], form.servisKayitKod) + ",";
            if (form.islemAciklama != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMACIKLAMA], form.islemAciklama) + ",";

            sql = sql.TrimEnd(',');

            if (form.kod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                return KomutCalistir(sql, 1);
            }
            else if (form.servisKayitKod != "")
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SERVISKAYITKOD], "=", form.servisKayitKod));
                return KomutCalistir(sql, 1);
            }
            return new Sonuc("EvrakIsTalep bilgi güncellme işlemi için kod veya servisKayitKod bilgisi bulunamadı.");
        }

        public IDataReader Listele(EvrakIsTalep form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string ekKosul = "";

            if (form.kod > 0)
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.prEvrakNo != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            if (form.servisKayitKod != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SERVISKAYITKOD], "=", form.servisKayitKod));
            if (form.islemTarihi.Yil > 1)
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", form.islemTarihi));
            if (form.islemYapan != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], ">=", form.islemYapan));
            if (form.islemYapanAd != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], ">=", form.islemYapanAd));
            if (form.atananKisi != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATANANKISI], ">=", form.atananKisi));
            if (form.atananKisiAd != "")
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATANANKISIAD], ">=", form.atananKisiAd));
            if (form.islem != ENUMEvrakIsTalepIslem.TANIMSIZ)
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)form.islem));

            sql += " WHERE " + ekKosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(EvrakIsTalep form)
        {
            form.islem = ENUMEvrakIsTalepIslem.IPTAL;
            return Guncelle(form);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
