using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik amaç bilgilerini tutan STRGENELHAVUZ veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABSpGenelHavuz : TABOrtak
    {
        public TABSpGenelHavuz(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRGENELHAVUZKIMLIK";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            TUR = 1,
            DURUM = 2,
            BASLANGICTARIH = 3,
            BITISTARIH = 4,
            HIZMETMEVZUAT = 5,
            HIZMETFAALIYET = 6,
            PROJETIP = 7,
            SORUNKATEGORI = 8,
            SORUNTIP = 9,
            USTBELGEKATEGORI = 10,
            ONERILENALTMETIN = 11,
            KURKOD = 12,
            FONKOD = 13,
            FINKOD = 14,
            EKOKOD = 15,
            PEBKOD = 16
        }
        public static string[] ALANLAR = { "KOD", "TUR", "DURUM", "BASLANGICTARIH", "BITISTARIH", "HIZMETMEVZUAT", "HIZMETFAALIYET", "PROJETIP", "SORUNKATEGORI", "SORUNTIP", "USTBELGEKATEGORI", "ONERILENALTMETIN",
                                           "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD" };

        public static string TABLOADORG = "STRGENELHAVUZORG";
        public enum ENUMAlanlarOrg : int
        {
            KOD = 0,
            TUR = 1,
            ORGBIRIMTUR = 2,
        }
        public static string[] ALANLARORG = { "KOD", "TUR", "ORGBIRIMTUR" };

        public static string TABLOADBAG = "STRGENELHAVUZBAG";
        public enum ENUMAlanlarBag : int
        {
            KOD = 0,
            BAGLIKOD = 1,
        }
        public static string[] ALANLARBAG = { "KOD", "BAGLIKOD" };

        public Sonuc KaydetOrganizasyon(string kod, string tur, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(kod) || string.IsNullOrEmpty(tur))
                return sonuc;

            foreach (SpGenelHavuzOrganizasyon organizasyon in liste.objeler)
            {
                if (string.IsNullOrEmpty(organizasyon.orgKod))
                    continue;

                string sql = "INSERT INTO " + TABLOADORG + " (" + VeriErisim.TumAlanlar("", ALANLARORG) + ") VALUES (";
                sql += VeriErisim.DegerYap(kod) + ",";
                sql += VeriErisim.DegerYap(tur) + ",";
                sql += VeriErisim.DegerYap(organizasyon.orgKod) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        public Sonuc KaydetBag(string kod, string bagliKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "INSERT INTO " + TABLOADBAG + " (" + VeriErisim.TumAlanlar("", ALANLARBAG) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(bagliKod) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc SilBag(string kod)
        {
            string sql = "DELETE FROM " + TABLOADBAG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBAG[(int)ENUMAlanlarBag.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilOrganizasyon(SpGenelHavuzOrganizasyon organizasyon)
        {
            string sql = "DELETE FROM " + TABLOADORG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARORG[(int)ENUMAlanlarOrg.KOD], "=", organizasyon.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARORG[(int)ENUMAlanlarOrg.TUR], "=", organizasyon.tur));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilOrganizasyon(string kod, string tur)
        {
            string sql = "DELETE FROM " + TABLOADORG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARORG[(int)ENUMAlanlarOrg.KOD], "=", kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARORG[(int)ENUMAlanlarOrg.TUR], "=", tur));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleOrganizasyon(SpGenelHavuzOrganizasyon organizasyon)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADORG, ALANLARORG);
            sql += " FROM " + TABLOADORG;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(organizasyon.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADORG, ALANLARORG[(int)ENUMAlanlarOrg.KOD], "=", organizasyon.kod));
            if (!string.IsNullOrEmpty(organizasyon.tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADORG, ALANLARORG[(int)ENUMAlanlarOrg.TUR], "=", organizasyon.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " where " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOrganizasyon(string kod, string tur)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADORG, ALANLARORG);
            sql += " FROM " + TABLOADORG;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADORG, ALANLARORG[(int)ENUMAlanlarOrg.KOD], "=", kod));
            if (!string.IsNullOrEmpty(tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADORG, ALANLARORG[(int)ENUMAlanlarOrg.TUR], "=", tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " where " + kosul;


            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleBag(string kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBAG, ALANLARBAG);
            sql += " FROM " + TABLOADBAG;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBAG, ALANLARBAG[(int)ENUMAlanlarBag.KOD], "=", kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(SpGenelHavuz sp)
        {
            sp.kod = HavuzKodAl(sp.tur).ToString().PadLeft(8, '0');
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sp.kod) + ",";
            sql += VeriErisim.DegerYap(sp.tur) + ",";
            //sql += VeriErisim.DegerYap(sp.ad) + ",";
            //sql += VeriErisim.DegerYap(sp.aciklama) + ",";
            sql += VeriErisim.DegerYap(sp.durum) + ",";
            sql += VeriErisim.DegerYap(sp.baslangicTarih) + ",";
            sql += VeriErisim.DegerYap(sp.bitisTarih) + ",";
            sql += VeriErisim.DegerYap(sp.hizmetMevzuat) + ",";
            sql += VeriErisim.DegerYap(sp.hizmetFaaliyet) + ",";
            sql += VeriErisim.DegerYap(sp.projeTip) + ",";
            sql += VeriErisim.DegerYap(sp.sorunKategori) + ",";
            sql += VeriErisim.DegerYap(sp.sorunTip) + ",";
            sql += VeriErisim.DegerYap(sp.ustBelgeKategori) + ",";
            sql += VeriErisim.DegerYap(sp.onerilenAltMetin) + ",";
            sql += VeriErisim.DegerYap(sp.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sp.fonKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sp.finKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sp.ekoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sp.pebKod.Replace(".", "")) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sp.kod.ToString();
            return sonuc;
        }

        public Sonuc Guncelle(SpGenelHavuz sp)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], sp.bitisTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], sp.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIZMETMEVZUAT], sp.hizmetMevzuat) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIZMETFAALIYET], sp.hizmetFaaliyet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJETIP], sp.projeTip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUNKATEGORI], sp.sorunKategori) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUNTIP], sp.sorunTip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTBELGEKATEGORI], sp.ustBelgeKategori) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONERILENALTMETIN], sp.onerilenAltMetin) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], sp.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], sp.fonKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], sp.finKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], sp.ekoKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], sp.pebKod.Replace(".", ""));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", sp.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", sp.tur));
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(SpGenelHavuz sp)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", sp.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", sp.tur));
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(SpGenelHavuz sp)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(sp.kod))
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", sp.kod);
            if (!string.IsNullOrEmpty(sp.tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", sp.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public bool HavuzVarMi(string kod, string tur)
        {
            return KayitVarmi(TABLOAD, kod, tur);
        }

        private bool KayitVarmi(string tablo, string kod, string tur)
        {

            string[] stringAlan = { ALANLAR[(int)ENUMAlanlar.KOD], ALANLAR[(int)ENUMAlanlar.TUR] };
            string[] stringDeger = { kod, tur };

            return TABGenel.KayitVarmi(veriErisim, tablo, stringAlan, stringDeger, new string[0], new int[0]);
        }

        private string HavuzKodAl(string tur)
        {
            string donusDeger = string.Empty;
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.TUR] };
            string[] strDeger = { tur };

            int deger = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], strAlan, strDeger, new string[0], new int[0]);

            if (deger == 1)
                donusDeger = tur + deger.ToString().PadLeft(6, '0');
            else
                donusDeger = deger.ToString().PadLeft(8, '0');

            return donusDeger;
        }

        public IDataReader Listele(int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, params string[] birimTur)
        {
            return Listele(donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod, durum, null, null, birimTur);
        }

        private IDataReader Listele(int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, TNSDateTime basTarih, TNSDateTime bitTarih, params string[] birimTur)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", spGenelHavuzKod.Replace(".", "") + "%"));

            if (durum != EnumSPGenelHavuzDurum.Hepsi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)durum));

            if (birimTur != null && birimTur.Length > 0)
            {
                string birimTurKosul = string.Empty;
                foreach (string bt in birimTur)
                    if (!string.IsNullOrEmpty(bt))
                        birimTurKosul = VeriErisim.KosulEkle(birimTurKosul, "OR", VeriErisim.KosulOl(TABLOADORG, ALANLARORG[(int)ENUMAlanlarOrg.ORGBIRIMTUR], "LIKE", bt.Replace(".", "") + "%"));

                if (!string.IsNullOrEmpty(birimTurKosul))
                {
                    //Birim türü uyan kataloglar ve birim türü olmayan kataloglar gelsin diye
                    string icSQL = "SELECT 1 FROM " + TABLOADORG;
                    icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADORG + "." + ALANLARORG[(int)ENUMAlanlarOrg.KOD];
                    sql = VeriErisim.KosulEkle(sql, "AND", "(NOT EXISTS (" + icSQL + ")");

                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", "(" + birimTurKosul + ")");
                    sql = VeriErisim.KosulEkle(sql, "OR", "EXISTS (" + icSQL + "))");
                }
            }

            if ((int)(EnumSPGenelHavuzIliski)System.Enum.Parse(typeof(EnumSPGenelHavuzIliski), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(spGenelHavuzKod.Substring(0, 2), 0))) > 0)
            {
                string icSQL = "SELECT 1 FROM " + TABLOADBAG;
                if (donemNo > 0 && orgBirimKod > 0)
                    icSQL += ", " + TABBirimGenelHavuz.TABLOAD;
                icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.KOD];
                if (donemNo > 0 && orgBirimKod > 0)
                {
                    icSQL += " AND " + TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " = " + TABBirimGenelHavuz.TABLOAD + "." + TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.GENELHAVUZKOD];
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABBirimGenelHavuz.TABLOAD, TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.DONEMNO], "=", donemNo));
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABBirimGenelHavuz.TABLOAD, TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
                }

                if (ustSpGenelHavuzKod != null && ustSpGenelHavuzKod.Length > 0)
                {
                    string ustSpKodKosul = string.Empty;
                    foreach (string ustSpKod in ustSpGenelHavuzKod)
                    {
                        if (!string.IsNullOrEmpty(ustSpKodKosul))
                            ustSpKodKosul += ", ";
                        if (!string.IsNullOrEmpty(ustSpKod))
                            ustSpKodKosul += VeriErisim.DegerYap(ustSpKod.Replace(".", ""));
                    }
                    if (!string.IsNullOrEmpty(ustSpKodKosul))
                        icSQL = VeriErisim.KosulEkle(icSQL, "AND", TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " IN (" + ustSpKodKosul + ")");
                }

                sql = VeriErisim.KosulEkle(sql, "AND", "EXISTS (" + icSQL + ")");
            }

            //Tarih kontrolünden vazgeçildi, belki ilerde tekrar açılır.
            //sql = VeriErisim.KosulEkle(sql, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH] + " IS NULL");
            //if (basTarih != null && !basTarih.isNull)
            //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<=", basTarih));
            //sql += ")";

            //sql = VeriErisim.KosulEkle(sql, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");
            //if (bitTarih != null && !bitTarih.isNull)
            //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", bitTarih));
            //sql += ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimdeKullanilmislarinBaglariniListele(int donemNo, int orgBirimKod, string spGenelHavuzKod, params string[] ustSpGenelHavuzKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBAG, ALANLARBAG);
            sql += " FROM " + TABLOADBAG;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(spGenelHavuzKod))
                kosul = VeriErisim.KosulOl(TABLOADBAG, ALANLARBAG[(int)ENUMAlanlarBag.KOD], "LIKE", spGenelHavuzKod.Replace(".", "") + "%");

            if (ustSpGenelHavuzKod != null && ustSpGenelHavuzKod.Length > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " IN (");
                foreach (string ustKod in ustSpGenelHavuzKod)
                    kosul += " " + VeriErisim.DegerYap(ustKod.Replace(".", "")) + ",";
                kosul = kosul.Substring(0, kosul.Length - 1) + ")";
            }

            if (donemNo > 0 && orgBirimKod > 0)
            {
                string icSQL = "SELECT 1 FROM " + TABBirimGenelHavuz.TABLOAD;
                icSQL += " WHERE " + TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.KOD] + " = " + TABBirimGenelHavuz.TABLOAD + "." + TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.GENELHAVUZKOD];
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABBirimGenelHavuz.TABLOAD, TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.DONEMNO], "=", donemNo));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABBirimGenelHavuz.TABLOAD, TABBirimGenelHavuz.ALANLAR[(int)TABBirimGenelHavuz.ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", "EXISTS (" + icSQL + ")");
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public bool AyniAdVarmi(string kod, string ad, int dilNo, string tur)
        {
            if (string.IsNullOrEmpty(ad))
                return false;

            string tabloAdHavuz = "STRGENELHAVUZ";
            string sql = "SELECT COUNT(*) FROM " + tabloAdHavuz;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kod))
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod);
            if (!string.IsNullOrEmpty(tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", tur + "%"));
            if (!string.IsNullOrEmpty(ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdHavuz, "AD" + ((dilNo > 1) ? dilNo.ToString() : string.Empty), "=", ad));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        public bool BagVarMi(string spGenelHavuzKod, params string[] ustSpGenelHavuzKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOADBAG;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(spGenelHavuzKod))
                kosul = VeriErisim.KosulOl(TABLOADBAG, ALANLARBAG[(int)ENUMAlanlarBag.KOD], "LIKE", spGenelHavuzKod.Replace(".", "") + "%");

            if (ustSpGenelHavuzKod != null && ustSpGenelHavuzKod.Length > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " IN (");
                foreach (string ustKod in ustSpGenelHavuzKod)
                    kosul += " " + VeriErisim.DegerYap(ustKod.Replace(".", "")) + ",";
                kosul = kosul.Substring(0, kosul.Length - 1) + ")";
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }
    }
}