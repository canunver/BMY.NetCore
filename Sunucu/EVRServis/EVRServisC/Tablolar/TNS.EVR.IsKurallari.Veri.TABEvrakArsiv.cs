using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakArsiv : TABOrtak
    {
        public TABEvrakArsiv(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKARSIV";
        public enum ENUMAlanlar : int { PREVRAKNO, ARSIVKOD, ACIKLAMA, ISLEMYAPAN, ISLEMTARIH, ISLEMYAPANAD, IPTAL }
        public static string[] ALANLAR = { "PREVRAKNO", "ARSIVKOD", "ACIKLAMA", "ISLEMYAPAN", "ISLEMTARIH", "ISLEMYAPANAD", "IPTAL" };

        public static string TABLOADMETA = "EVREVRAKARSIVMETA";
        public enum ENUMAlanlarMeta : int { PREVRAKNO, METAKOD, METADEGER, METADEGER2 }
        public static string[] ALANLARMETA = { "PREVRAKNO", "METAKOD", "METADEGER", "METADEGER2" };

        public static string TABLOADDOSYA = "EVREVRAKARSIVDOSYA";
        public enum ENUMAlanlarDosya : int { PREVRAKNO, ARSIVKOD, DOSYAAD, DOSYAYOL, DOSYAADET }
        public static string[] ALANLARDOSYA = { "PREVRAKNO", "ARSIVKOD", "DOSYAAD", "DOSYAYOL", "DOSYAADET" };

        public static string TABLOADTARIHCE = "EVREVRAKARSIVTARIHCE";
        public enum ENUMAlanlarTarihce : int { ISLEMTARIH, ISLEM, ISLEMYAPAN, ISLEMYAPANAD, PREVRAKNO }
        public static string[] ALANLARTARIHCE = { "ISLEMTARIH", "ISLEM", "ISLEMYAPAN", "ISLEMYAPANAD", "PREVRAKNO" };

        //public IDataReader Listele(Arsiv form)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
        //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
        //    //if (form.arsiv.kod > 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ARSIVKOD], "=", form.arsiv.kod));

        //    return veriErisim.DataReaderDondur(sql);
        //}
        public IDataReader Listele(EvrakArsiv form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ARSIVKOD], "=", form.arsiv.kod));
            if (string.IsNullOrEmpty(form.prEvrakNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(EvrakArsiv form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.arsiv.kod) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += 0 + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc IptalEt(EvrakArsiv form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IPTAL], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return KomutCalistir(sql, 1);
            //string sql = "DELETE FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            //return KomutCalistir(sql, 0);
        }
        public Sonuc Sil(EvrakArsiv form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return KomutCalistir(sql, 1);
        }
        public int KayitKontrol(string prEvrakNo, int arsivKod)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ARSIVKOD], "=", arsivKod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader Listele(EvrakArsivMeta form)
        {
            string sql = "SELECT EVREVRAKARSIVMETA.PREVRAKNO, EVREVRAKARSIVMETA.METAKOD, EVREVRAKARSIVMETA.METADEGER, EVREVRAKARSIVMETA.METADEGER2, EVRMETA.AD, EVRMETA.TIP, EVRMETA.ZORUNLU ";
            sql += "FROM EVREVRAKARSIVMETA INNER JOIN ";
            sql += "EVRMETA ON EVREVRAKARSIVMETA.METAKOD = EVRMETA.KOD ";
            sql += "WHERE (EVREVRAKARSIVMETA.PREVRAKNO = " + VeriErisim.DegerYap(form.prEvrakNo) + ")";

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(EvrakArsivMeta form)
        {
            string sql = "INSERT INTO " + TABLOADMETA + " (" + VeriErisim.TumAlanlar("", ALANLARMETA) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.meta.kod) + ",";
            sql += VeriErisim.DegerYap(form.metaDeger) + ",";
            sql += VeriErisim.DegerYap(form.metaDeger2) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc SilMeta(string prEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOADMETA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMETA[(int)ENUMAlanlarMeta.PREVRAKNO], "=", prEvrakNo));
            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(EvrakArsivDosya form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDOSYA) + " FROM " + TABLOADDOSYA;
            if (!string.IsNullOrEmpty(form.prEvrakNo))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDOSYA[(int)ENUMAlanlarDosya.PREVRAKNO], "=", form.prEvrakNo));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDOSYA[(int)ENUMAlanlarDosya.ARSIVKOD], "=", form.arsivKod));

            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDOSYA, ALANLARDOSYA) + " FROM " + TABLOADDOSYA;
            //sql += " INNER JOIN EVREVRAKARSIV ON EVREVRAKARSIVDOSYA.PREVRAKNO = EVREVRAKARSIV.PREVRAKNO ";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVREVRAKARSIV", ALANLARDOSYA[(int)ENUMAlanlarDosya.ARSIVKOD], "=", form.arsivKod));

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader Listele(int arsivKod, EvrakArsivMeta[] formList)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDOSYA, ALANLARDOSYA) + " FROM " + TABLOADDOSYA;

            sql += " INNER JOIN ";
            sql += " EVREVRAKARSIVMETA ON EVREVRAKARSIVDOSYA.PREVRAKNO = EVREVRAKARSIVMETA.PREVRAKNO ";
            sql += " WHERE ";

            int index = 0;
            foreach (EvrakArsivMeta item in formList)
            {
                index++;

                sql += "(EVREVRAKARSIVDOSYA.ARSIVKOD = " + VeriErisim.DegerYap(arsivKod) + ") ";
                if (item.meta.kod == -100) //Dosya Adına Göre Ara
                    sql += " AND (EVREVRAKARSIVDOSYA.DOSYAAD LIKE " + VeriErisim.DegerYap("%" + item.metaDeger + "%") + ") ";
                else
                    sql += " AND (EVREVRAKARSIVMETA.METADEGER LIKE " + VeriErisim.DegerYap("%" + item.metaDeger + "%") + ") AND (EVREVRAKARSIVMETA.METAKOD = " + VeriErisim.DegerYap(item.meta.kod) + ") ";
                if (index != formList.Length)
                    sql += " OR ";
            }

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader Ara(int arsivKod, EvrakArsivMeta[] formList)
        {
            string sql = "SELECT DISTINCT " +
                        "EAD.PREVRAKNO, EAD.DOSYAAD, EAD.DOSYAYOL, EAD.DOSYAADET, EVREVRAKARSIV.ISLEMYAPAN, EVREVRAKARSIV.ISLEMTARIH, EVREVRAKARSIV.ISLEMYAPANAD, EVREVRAKARSIV.IPTAL, EVRMETA.KOD AS METAKOD, " +
                        "EVRMETA.AD AS METAAD, EMETA.METADEGER, EMETA.METADEGER2 " +
                        "FROM EVREVRAKARSIVDOSYA AS EAD INNER JOIN " +
                        "EVREVRAKARSIVMETA AS EAM ON EAD.PREVRAKNO = EAM.PREVRAKNO INNER JOIN " +
                        "EVREVRAKARSIV ON EAD.PREVRAKNO = EVREVRAKARSIV.PREVRAKNO INNER JOIN " +
                        "EVREVRAKARSIVMETA AS EMETA ON EVREVRAKARSIV.PREVRAKNO = EMETA.PREVRAKNO INNER JOIN " +
                        "EVRMETA ON EMETA.METAKOD = EVRMETA.KOD ";

            string kosul = "";
            string join = "";
            int joinIndex = 0;
            int index = 0;

            string prEvrakNoKriter = "";
            bool iptalEdilenlerdeListelensin = false;

            foreach (EvrakArsivMeta item in formList)
            {
                index++;

                if (item.meta.kod == -100)      //Dosya Adına Göre Ara
                    kosul += " AND (EAD.DOSYAAD LIKE " + VeriErisim.DegerYap("%" + item.metaDeger + "%") + ")";
                else if (item.meta.kod == -200) //Dosya Sayısına Göre Ara
                    kosul += " AND (EAD.DOSYAADET = " + VeriErisim.DegerYap(item.metaDeger) + ")";
                else if (item.meta.kod == -300) //PrEvrakNo bilgisine Göre Ara
                {
                    prEvrakNoKriter = " AND (EVREVRAKARSIV.PREVRAKNO = " + VeriErisim.DegerYap(item.metaDeger) + ")";
                }
                else if (item.meta.kod == -400) //İptal edilen evraklarıda listele
                {
                    iptalEdilenlerdeListelensin = true;
                }
                else
                {
                    if (index > 1)
                    {
                        joinIndex++;
                        string joinTable = "EAM" + joinIndex;
                        join += " INNER JOIN EVREVRAKARSIVMETA AS " + joinTable + " ON EAM.PREVRAKNO = " + joinTable + ".PREVRAKNO ";
                        kosul += " AND (" + joinTable + ".METADEGER LIKE " + VeriErisim.DegerYap("%" + item.metaDeger + "%") + ") AND (" + joinTable + ".METAKOD = " + VeriErisim.DegerYap(item.meta.kod) + ") ";
                    }
                    else
                        kosul += " AND (EAM.METADEGER LIKE " + VeriErisim.DegerYap("%" + item.metaDeger + "%") + ") AND (EAM.METAKOD = " + VeriErisim.DegerYap(item.meta.kod) + ") ";
                }
            }

            sql += join;
            sql += " WHERE (EAD.ARSIVKOD = " + VeriErisim.DegerYap(arsivKod) + ") ";

            if (!iptalEdilenlerdeListelensin)
                sql += " AND (EVREVRAKARSIV.IPTAL = 0 OR EVREVRAKARSIV.IPTAL IS NULL) ";

            if (prEvrakNoKriter != "")
                kosul = prEvrakNoKriter;

            sql += kosul;
            sql += "ORDER BY EAD.PREVRAKNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(EvrakArsivDosya form)
        {
            string sql = "INSERT INTO " + TABLOADDOSYA + " (" + VeriErisim.TumAlanlar("", ALANLARDOSYA) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.arsivKod) + ",";
            sql += VeriErisim.DegerYap(form.dosyaAd) + ",";
            sql += VeriErisim.DegerYap(form.dosyaYol) + ",";
            sql += VeriErisim.DegerYap(form.dosyaAdet) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc SilDosya(string prEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOADDOSYA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDOSYA[(int)ENUMAlanlarDosya.PREVRAKNO], "=", prEvrakNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc Kaydet(EvrakArsivTarihce form)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islem) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.prevrakNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleEvrakSayisi(TNSDateTime baslangicTarih, TNSDateTime bitisTarih, string islemYapan, int arsivKod)
        {
            string sql = "SELECT EVREVRAKARSIV.ISLEMYAPANAD, COUNT(*) AS [Evrak Sayısı], SUM(EVREVRAKARSIVDOSYA.DOSYAADET) AS [Dosya Sayısı], EVREVRAKARSIVTARIHCE.ISLEM";
            sql += " FROM EVREVRAKARSIV INNER JOIN";
            sql += " EVREVRAKARSIVDOSYA ON EVREVRAKARSIV.PREVRAKNO = EVREVRAKARSIVDOSYA.PREVRAKNO INNER JOIN";
            sql += " EVREVRAKARSIVTARIHCE ON EVREVRAKARSIV.PREVRAKNO = EVREVRAKARSIVTARIHCE.PREVRAKNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVREVRAKARSIVTARIHCE", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">", baslangicTarih));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKARSIVTARIHCE", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<", new TNSDateTime(bitisTarih.Oku().AddDays(1))));
            if (!string.IsNullOrEmpty(islemYapan))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKARSIV", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", islemYapan));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKARSIV", ALANLAR[(int)ENUMAlanlar.ARSIVKOD], "=", arsivKod));

            sql += " GROUP BY EVREVRAKARSIV.ISLEMYAPANAD, EVREVRAKARSIVTARIHCE.ISLEM";
            sql += " ORDER BY EVREVRAKARSIV.ISLEMYAPANAD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
