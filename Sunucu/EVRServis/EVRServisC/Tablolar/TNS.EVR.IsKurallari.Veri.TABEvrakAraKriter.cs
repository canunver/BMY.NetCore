using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{


    public class TABEvrakAraKriter : TABOrtak
    {
        public TABEvrakAraKriter(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKARAKRITER";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            ISLEMYAPAN = 2,
            ISLEMTARIH = 3,
            EVRAKBIRIMKODU = 4,
            EVRAKBIRIMKODUALT = 5,
            YIL = 6,
            EVRAKTIPI = 7,
            KAYITTARIHI1 = 8,
            KAYITTARIHI2 = 9,
            EVRAKNO1 = 10,
            EVRAKNO2 = 11,
            KONU = 12,
            GELDIGIYER = 13,
            GELDIGIYERTUR = 14,
            GELENEVRAKNO = 15,
            GELENEVRAKTARIH = 16,
            ICERIK = 17,
            YAZISEKLI = 18,
            GELISSEKLI = 19,
            GIZLILIK = 20,
            YAZIDURUM = 21,
            TASFIYEKOD = 22,
            SAKLAMASURESI = 23,
            SAKLAMAKRITERIKOD = 24,
            SAKLAMABASLANGICKOD = 25,
            SORGULAMATURU = 26,
            DOSYANO=27,
            EVRAKICERIK=28,
        }
        public static string[] ALANLAR = {  "KOD"
                                            ,"AD"
                                            ,"ISLEMYAPAN"
                                            ,"ISLEMTARIH"
                                            ,"EVRAKBIRIMKODU"
                                            ,"EVRAKBIRIMKODUALT"
                                            ,"YIL"
                                            ,"EVRAKTIPI"
                                            ,"KAYITTARIHI1"
                                            ,"KAYITTARIHI2"
                                            ,"EVRAKNO1"
                                            ,"EVRAKNO2"
                                            ,"KONU"
                                            ,"GELDIGIYER"
                                            ,"GELDIGIYERTUR"
                                            ,"GELENEVRAKNO"
                                            ,"GELENEVRAKTARIH"
                                            ,"ICERIK"
                                            ,"YAZISEKLI"
                                            ,"GELISSEKLI"
                                            ,"GIZLILIK"
                                            ,"YAZIDURUM"
                                            ,"TASFIYEKOD"
                                            ,"SAKLAMASURESI"
                                            ,"SAKLAMAKRITERIKOD"
                                            ,"SAKLAMABASLANGICKOD"
                                            ,"SORGULAMATURU"
                                            ,"DOSYANO"
                                            ,"EVRAKICERIK"
                                        };

        public Sonuc Kaydet(EvrakAraKriter form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.islemyapan) + ",";
            sql += VeriErisim.DegerYap(form.islemtarih) + ",";
            sql += VeriErisim.DegerYap(form.evrakbirimkodu) + ",";
            sql += VeriErisim.DegerYap(form.evrakbirimkodualt) + ",";
            sql += VeriErisim.DegerYap(form.yil) + ",";
            sql += VeriErisim.DegerYap(form.evraktipi) + ",";
            sql += VeriErisim.DegerYap(form.kayittarihi1) + ",";
            sql += VeriErisim.DegerYap(form.kayittarihi2) + ",";
            sql += VeriErisim.DegerYap(form.evrakno1) + ",";
            sql += VeriErisim.DegerYap(form.evrakno2) + ",";
            sql += VeriErisim.DegerYap(form.konu) + ",";
            sql += VeriErisim.DegerYap(form.geldigiyer) + ",";
            sql += VeriErisim.DegerYap(form.geldigiyertur) + ",";
            sql += VeriErisim.DegerYap(form.gelenevrakno) + ",";
            sql += VeriErisim.DegerYap(form.gelenevraktarih) + ",";
            sql += VeriErisim.DegerYap(form.icerik) + ",";
            sql += VeriErisim.DegerYap(form.yazisekli) + ",";
            sql += VeriErisim.DegerYap(form.gelissekli) + ",";
            sql += VeriErisim.DegerYap(form.gizlilik) + ",";
            sql += VeriErisim.DegerYap(form.yazidurum) + ",";
            sql += VeriErisim.DegerYap(form.tasfiyeKod) + ",";
            sql += VeriErisim.DegerYap(form.saklamaSuresi) + ",";
            sql += VeriErisim.DegerYap(form.saklamaKriteriKod) + ",";
            sql += VeriErisim.DegerYap(form.saklamaBaslangicKod) + ",";
            sql += VeriErisim.DegerYap(form.sorguTuru) + ",";
            sql += VeriErisim.DegerYap(form.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakIcerigi) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakAraKriter form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kriter = "";
            if (form.kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemyapan));

            if (form.sorguTuru == 2)//ORNEKLE SORGULA tabında kayıtlanmışları getir
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SORGULAMATURU], "=", form.sorguTuru));

            else if (form.sorguTuru != 0)//ORNEKLE SORGULA tabında kayıt edilmişler hariç hepsini getir.
            {
                kriter += " AND (SORGULAMATURU!=2 OR SORGULAMATURU IS NULL)";
            }

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY ISLEMTARIH DESC";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
