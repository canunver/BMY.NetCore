using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    //public class TABPersonelAtama : TABOrtak
    //{
    //    public TABPersonelAtama(VeriErisim veriErisim) : base(veriErisim) { }

    //    public static string TABLOAD = "IKYPERSONELATAMA";

    //    public enum ENUMAlanlar : int
    //    {
    //        KOD = 0,
    //        PKKOD = 1,
    //        ATAMATURU = 2,
    //        ATAMASEKLI = 3,
    //        ATAMANEDENI = 4,
    //        ATAMATARIHI = 5,
    //        ONAYTARIHI = 6,
    //        ONAYLAYAN = 7,
    //        HAREKETDURUMU = 8
    //    }
    //    public static string[] ALANLAR = { "KOD", "PKKOD", "ATAMATURU", "ATAMASEKLI", "ATAMANEDENI", "ATAMATARIHI", "ONAYTARIHI", "ONAYLAYAN", "HAREKETDURUMU" };

    //    //public Sonuc Kaydet(PersonelAtama atama)
    //    //{
    //    //    string sql = "";
    //    //    Sonuc sonuc = new Sonuc();

    //    //    veriErisim.hata = "";

    //    //    sql = "UPDATE " + TABLOAD + " SET ";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAMATURU], atama.atamaTuru) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAMASEKLI], atama.atamaSekli) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAMANEDENI], atama.atamaNedeni) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAMATARIHI], atama.atamaTarihi);
    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", atama.kod));

    //    //    sonuc = KomutCalistir(sql, 1);

    //    //    if (!sonuc.islemSonuc)
    //    //    {
    //    //        sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
    //    //        sql += VeriErisim.DegerYap(atama.kod) + ",";
    //    //        sql += VeriErisim.DegerYap(atama.pkkod) + ",";

    //    //        if (atama.atamaTuru > 0)
    //    //            sql += VeriErisim.DegerYap(atama.atamaTuru) + ",";
    //    //        else
    //    //            sql += VeriErisim.DegerYap((string)null) + ",";

    //    //        if (atama.atamaSekli > 0)
    //    //            sql += VeriErisim.DegerYap(atama.atamaSekli) + ",";
    //    //        else
    //    //            sql += VeriErisim.DegerYap((string)null) + ",";

    //    //        sql += VeriErisim.DegerYap(atama.atamaNedeni) + ",";
    //    //        sql += VeriErisim.DegerYap(atama.atamaTarihi) + ",";
    //    //        sql += VeriErisim.DegerYap(atama.onayTarihi) + ",";
    //    //        sql += VeriErisim.DegerYap(atama.onaylayan) + ",";

    //    //        if (atama.hareketDurumu > 0)
    //    //            sql += VeriErisim.DegerYap(atama.hareketDurumu) + ")";
    //    //        else
    //    //            sql += VeriErisim.DegerYap((string)null) + ")";

    //    //        sonuc = KomutCalistir(sql, 1);
    //    //    }

    //    //    return sonuc;
    //    //}

    //    //public Sonuc GuncelleOnayBilgileri(PersonelAtama atama)
    //    //{
    //    //    string sql = "";

    //    //    sql = "UPDATE " + TABLOAD + " SET ";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIHI], atama.onayTarihi) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], atama.onaylayan) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAREKETDURUMU], atama.hareketDurumu);
    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", atama.kod));
   
    //    //    return KomutCalistir(sql, 1);         
    //    //}

    //    //public IDataReader ListeleAtama(PersonelKadro p)
    //    //{
    //    //    string sql = "";
    //    //    string kosul = "";

    //    //    sql = "SELECT " + VeriErisim.TumAlanlar(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR) + ", " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
    //    //          " IKYPERSONEL.TCKIMLIKNO, IKYPERSONEL.AD, IKYPERSONEL.SOYAD, IKYPERSONELTIPI.AD, SOZLUK1.KAYITAD, SOZLUK2.KAYITAD, IKYUNVAN.AD, SOZLUK3.KAYITAD, SOZLUK5.KAYITAD" +
    //    //          " , SOZLUK5.KAYITKOD FROM IKYPERSONELATAMA" +
    //    //          " LEFT OUTER JOIN IKYPERSONELKADRO ON IKYPERSONELATAMA.PKKOD = IKYPERSONELKADRO.KOD" +
    //    //          " LEFT OUTER JOIN IKYPERSONEL ON IKYPERSONEL.KOD = IKYPERSONELKADRO.PKOD" +
    //    //          " LEFT OUTER JOIN IKYUNVAN ON IKYUNVAN.KOD = IKYPERSONELKADRO.UNVANKOD" +
    //    //          " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK1 ON SOZLUK1.KOD = IKYPERSONELATAMA.ATAMATURU" +
    //    //          " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK2 ON SOZLUK2.KOD = IKYPERSONELATAMA.ATAMASEKLI" +
    //    //          " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK3 ON SOZLUK3.KOD = IKYUNVAN.SINIFKOD" +
    //    //          " LEFT OUTER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD = IKYPERSONEL.PERSONELTUR" +
    //    //          " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK5 ON SOZLUK5.KOD = IKYPERSONELATAMA.HAREKETDURUMU";

    //    //    if (p.atama.kod > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.atama.kod));

    //    //    if (p.atama.hareketDurumu > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HAREKETDURUMU], "=", p.atama.hareketDurumu));

    //    //    if (p.atama.atamaTuru > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAMATURU], "=", p.atama.atamaTuru));

    //    //    if (p.atama.atamaSekli > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAMASEKLI], "=", p.atama.atamaSekli));

    //    //    if (p.personelTCKimlikNo != "")
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.TCKIMLIKNO], "=", p.personelTCKimlikNo));

    //    //    //if (p.personelTur > 0)
    //    //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.PERSONELTUR], "=", p.personelTur));

    //    //    if (p.organizasyonKodu > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD], "=", p.organizasyonKodu));

    //    //    if (p.unvan > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD], "=", p.unvan));

    //    //    if (p.sinifi > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABUnvan.TABLOAD, TABUnvan.ALANLAR[(int)TABUnvan.ENUMAlanlar.SINIFKOD], "=", p.sinifi));

    //    //    if (p.kadroDerece > 0)
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.KADRODERECE], "=", p.kadroDerece));

    //    //    //TODO: Diğer alanlar da kontrol edilip sorguya eklenecek, sıralama şartı güncellenecek

    //    //    if (kosul != "")
    //    //        sql += " WHERE " + kosul;

    //    //    sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";

    //    //    return veriErisim.DataReaderDondur(sql);
    //    //}
    //}
}
