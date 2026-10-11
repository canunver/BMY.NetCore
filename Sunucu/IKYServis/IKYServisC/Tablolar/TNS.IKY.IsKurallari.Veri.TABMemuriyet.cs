using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    //public class TABMemuriyet : TABOrtak
    //{
    //    public TABMemuriyet(VeriErisim veriErisim) : base(veriErisim) { }

    //    ///// <summary>
    //    ///// Tablo adı
    //    ///// </summary>
    //    //static string TABLOAD = "IKYPERSONELMEMURIYET";
    //    ///// <summary>
    //    ///// Tablo alanları indisi
    //    ///// </summary>
    //    //public enum ENUMAlanlar : int
    //    //{
    //    //    /// <summary>
    //    //    /// KOD 
    //    //    /// </summary>
    //    //    KOD = 0,
    //    //    /// <summary>
    //    //    /// PERSONEL KOD
    //    //    /// </summary>
    //    //    PKOD = 1,
    //    //    /// <summary>
    //    //    /// MEMURİYET TÜRÜ
    //    //    /// </summary>
    //    //    MEMURIYETTURU = 2,            
    //    //    /// <summary>
    //    //    /// Birim Kodu
    //    //    /// </summary>
    //    //    BIRIMKODU = 3,
    //    //           /// <summary>
    //    //    /// Birim Adı
    //    //    /// </summary>
    //    //    BIRIMAD = 4,
    //    //    /// <summary>
    //    //    /// UNVANKOD
    //    //    /// </summary>
    //    //    UNVANKOD = 5,
    //    //    /// <summary>
    //    //    /// SINIF KODU
    //    //    /// </summary>
    //    //    SINIFKOD = 6,
    //    //    /// <summary>
    //    //    /// Açıklama
    //    //    /// </summary>
    //    //    ACIKLAMA = 7,
    //    //    /// <summary>
    //    //    /// GÖREV YERİ
    //    //    /// </summary>
    //    //    GOREVYERI = 8,
    //    //    /// <summary>
    //    //    /// GÖREV TÜRÜ
    //    //    /// </summary>
    //    //    GOREVTURU = 9,
    //    //    /// <summary>
    //    //    /// ATAMA TARİHİ
    //    //    /// </summary>
    //    //    ATAMATARIHI = 10,
    //    //    /// <summary>
    //    //    /// Başlama Tarihi
    //    //    /// </summary>
    //    //    BASLAMATARIHI = 11,
    //    //    /// <summary>
    //    //    /// Başlama Nedeni
    //    //    /// </summary>
    //    //    BASLAMANEDENI = 12,
    //    //    BASLAMABELGETURU=13,
    //    //    BASLAMABELGENO=14,
    //    //    BASLAMABELGETARIHI=15,
    //    //    /// <summary>
    //    //    /// Ayrılma Tarihi
    //    //    /// </summary>
    //    //    AYRILISTARIHI = 16,            
    //    //    /// <summary>
    //    //    /// Ayrılma Nedeni
    //    //    /// </summary>
    //    //    AYRILISNEDENI = 17,
    //    //    AYRILISBELGETURU = 18,
    //    //    AYRILISBELGENO = 19,
    //    //    AYRILISBELGETARIHI = 20,
    //    //    HIZMETSURESI=21,
    //    //    KADRODERECE = 22,
    //    //    ODEMEYEESASDERECE = 23,
    //    //    ODEMEYEESASKADEME = 24,
    //    //    ODEMEYEESASTARIH = 25,
    //    //    ODEMEYEESASGOSTERGE = 26,
    //    //    ODEMEYEESASEKGOSTERGE = 27,
    //    //    KAZANILMISHAKDERECE = 28,
    //    //    KAZANILMISHAKKADEME = 29,
    //    //    KAZANILMISHAKTARIH = 30,
    //    //    KAZANILMISGOSTERGE = 31,
    //    //    KAZANILMISEKGOSTERGE = 32,
    //    //    EMEKLIYEESASDERECE = 33,
    //    //    EMEKLIYEESASKADEME = 34,
    //    //    EMEKLIYEESASTARIH = 35,  
    //    //    EMEKLIYEESASGOSTERGE = 36,
    //    //    EMEKLIYEESASEKGOSTERGE = 37
    //    //}
    //    ///// <summary>
    //    ///// Tablo alanları
    //    ///// </summary>
    //    //static string[] ALANLAR = { "TCKIMLIKNO", "SIRANO", "ORGANIZASYONKOD", "MUHASEBEKODU", "BIRIMKODU", "KURUM", "BIRIMAD", "GOREV", "ISTIHDAMSEKLI", "ACIKLAMA", "BASLAMATARIHI", "AYRILMATARIHI", "BASLAMANEDENI", "AYRILMANEDENI", "EVRAKNO", "YASALDAYANAK", "ONAYTARIH", "KADRODERECE", "KADROTARIH", "ODEMEYEESASDERECE", "ODEMEYEESASKADEME", "ODEMEYEESASTARIH", "KAZANILMISHAKDERECE", "KAZANILMISHAKKADEME", "KAZANILMISHAKTARIH", "EMEKLIYEESASDERECE", "EMEKLIYEESASKADEME", "EMEKLIYEESASTARIH", "ODEMEYEESASGOSTERGE", "ODEMEYEESASEKGOSTERGE", "KAZANILMISGOSTERGE", "KAZANILMISEKGOSTERGE", "EMEKLIYEESASGOSTERGE", "EMEKLIYEESASEKGOSTERGE", };

    //    //public Sonuc KaydetMemuriyet(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "";
    //    //    //string sayacKosul = ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
    //    //    //p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], sayacKosul);

    //    //    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
    //    //    sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
    //    //    sql += VeriErisim.DegerYap(p.siraNo) + ",";
    //    //    sql += VeriErisim.DegerYap(p.organizasyonKodu) + ",";
    //    //    sql += VeriErisim.DegerYap(p.muhasebeKodu) + ",";
    //    //    sql += VeriErisim.DegerYap(p.birimKodu) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kurum.Replace(".", "")) + ",";
    //    //    sql += VeriErisim.DegerYap(p.birimAd) + ",";
    //    //    sql += VeriErisim.DegerYap(p.gorev) + ",";
    //    //    sql += VeriErisim.DegerYap(p.istihdamSekli) + ",";
    //    //    sql += VeriErisim.DegerYap(p.aciklama) + ",";
    //    //    sql += VeriErisim.DegerYap(p.baslamaTarihi) + ",";
    //    //    sql += VeriErisim.DegerYap(p.ayrilmaTarihi) + ",";
    //    //    sql += VeriErisim.DegerYap(p.baslamaNedeni) + ",";
    //    //    sql += VeriErisim.DegerYap(p.ayrilmaNedeni) + ",";
    //    //    sql += VeriErisim.DegerYap(p.evrakNo) + ",";
    //    //    sql += VeriErisim.DegerYap(p.yasalDayanak) + ",";
    //    //    sql += VeriErisim.DegerYap(p.onayTarih) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kadroTarih) + ",";
    //    //    sql += VeriErisim.DegerYap(p.odemeyeEsasDerece) + ",";
    //    //    sql += VeriErisim.DegerYap(p.odemeyeEsasKademe) + ",";
    //    //    sql += VeriErisim.DegerYap(p.odemeyeEsasTarih) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kazanilmisHakDerece) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kazanilmisHakKademe) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kazanilmisHakTarih) + ",";
    //    //    sql += VeriErisim.DegerYap(p.emekliyeEsasDerece) + ",";
    //    //    sql += VeriErisim.DegerYap(p.emekliyeEsasKademe) + ",";
    //    //    sql += VeriErisim.DegerYap(p.emekliyeEsasTarih) + ",";
    //    //    sql += VeriErisim.DegerYap(p.odemeyeEsasGosterge) + ",";
    //    //    sql += VeriErisim.DegerYap(p.odemeyeEsasEkGosterge) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kazanilmisGosterge) + ",";
    //    //    sql += VeriErisim.DegerYap(p.kazanilmisEkGosterge) + ",";
    //    //    sql += VeriErisim.DegerYap(p.emekliyeEsasGosterge) + ",";
    //    //    sql += VeriErisim.DegerYap(p.emekliyeEsasEkGosterge) + ")";


    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public IDataReader ListeleMemuriyet(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "";
    //    //    string kosul = "";

    //    //    sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",IKYNEDENBASLAMA.AD, IKYNEDENAYRILMA.AD";
    //    //    sql += " FROM IKYNEDEN IKYNEDENAYRILMA ";
    //    //    sql += "RIGHT OUTER JOIN IKYPERSONELMEMURIYET ON IKYNEDENAYRILMA.KOD = IKYPERSONELMEMURIYET.AYRILMANEDENI  ";
    //    //    sql += "LEFT OUTER JOIN IKYNEDEN IKYNEDENBASLAMA ON IKYNEDENBASLAMA.KOD=IKYPERSONELMEMURIYET.BASLAMANEDENI ";

    //    //    //if (!string.IsNullOrEmpty(p.tcKimlikNo))
    //    //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

    //    //    //if (p.siraNo > 0)
    //    //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

    //    //    if (!string.IsNullOrEmpty(p.aciklama))
    //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

    //    //    //TODO: Diğer alanlar da kontrol edilip sorguya eklenecek

    //    //    if (kosul != "")
    //    //        sql += " WHERE " + kosul;

    //    //    sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI] + " ASC";

    //    //    return veriErisim.DataReaderDondur(sql);
    //    //}

    //    //public IDataReader ListeleMemuriyetTarihleri(string tcKimlikNo)
    //    //{
    //    //    throw new Exception("Hazır Değil");
    //    //    //string sql = "";// "SELECT " + ALANLAR[(int)ENUMAlanlar.MEMURIYETEBASLAMATARIHI] + "," + ALANLAR[(int)ENUMAlanlar.SONBIRIMBASLAMATARIHI] + "," + ALANLAR[(int)ENUMAlanlar.SONGOREVBASLAMATARIHI] + " FROM " + TABLOAD;
    //    //    ////sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));
    //    //    //return veriErisim.DataReaderDondur(sql);
    //    //}

    //    //public Sonuc GuncelleMemuriyet(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "";

    //    //    sql = "UPDATE " + TABLOAD + " SET ";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], p.birimKodu) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMAD], p.birimAd) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], p.aciklama) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], p.baslamaTarihi) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYRILISTARIHI], p.ayrilmaTarihi) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMANEDENI], p.baslamaNedeni) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYRILISNEDENI], p.ayrilmaNedeni) + ",";

    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.kod));

    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public Sonuc SilMemuriyet(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "";

    //    //    sql = "DELETE FROM " + TABLOAD;
    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public Sonuc MemuriyetPersonelBirimGuncelle(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "UPDATE " + TABLOAD + " SET ";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI], p.istihdamSekli);
    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public Sonuc MemuriyetPersonelAyrilmaGuncelle(string tcKimlikNo)
    //    //{
    //    //    string sql = "UPDATE " + TABLOAD + " SET ";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "") + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROMUHASEBEKODU], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROBIRIMKODU], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROORGANIZASYONKOD], 0) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROKURUM], "");
    //    //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", KOD));

    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public Sonuc MemuriyetTarihleriGuncelle(string tcKimlikNo, TNSDateTime memuriyetBaslamaTarihi, TNSDateTime sonBirimeBaslamaTarihi, TNSDateTime sonGoreveBaslamaTarihi)
    //    //{
    //    //    string sql = "UPDATE " + TABLOAD + " SET ";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEMURIYETEBASLAMATARIHI], memuriyetBaslamaTarihi) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONBIRIMBASLAMATARIHI], sonBirimeBaslamaTarihi) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONGOREVBASLAMATARIHI], sonGoreveBaslamaTarihi);
    //    //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

    //    //    return KomutCalistir(sql, 1);
    //    //}

    //    //public Sonuc MemuriyetKadroBilgileriniGuncelle(PersonelMemuriyet p)
    //    //{
    //    //    string sql = "UPDATE " + TABLOAD + " SET ";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADRODERECE], p.kadroDerece) + ",";
    //    //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROTARIH], p.kadroTarih) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYEESASDERECE], p.odemeyeEsasDerece) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYEESASKADEME], p.odemeyeEsasKademe) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYEESASTARIH], p.odemeyeEsasTarih) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIYEESASDERECE], p.emekliyeEsasDerece) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIYEESASKADEME], p.emekliyeEsasKademe) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIYEESASTARIH], p.emekliyeEsasTarih) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYEESASGOSTERGE], p.odemeyeEsasGosterge) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYEESASEKGOSTERGE], p.odemeyeEsasEkGosterge) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISGOSTERGE], p.kazanilmisGosterge) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISEKGOSTERGE], p.kazanilmisEkGosterge) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIYEESASGOSTERGE], p.emekliyeEsasGosterge) + ",";
    //    //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIYEESASEKGOSTERGE], p.emekliyeEsasEkGosterge);

    //    //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
    //    //    return KomutCalistir(sql, 1);
    //    //}        

    //    //public TNSDateTime MemuriyetTarihiAl(string tcKimlikNo, int tarihTip)
    //    //{
    //    //    string sql = "";
    //    //    if (tarihTip == 1)//memuriyet başlama tarihi
    //    //        sql = "SELECT BASLAMATARIHI FROM IKYPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MIN(BASLAMATARIHI) FROM IKYPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";
    //    //    else if (tarihTip == 2)//Göreve başlama tarihi
    //    //        sql = "SELECT BASLAMATARIHI FROM IKYPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MAX(BASLAMATARIHI) FROM IKYPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";
    //    //    else if (tarihTip == 3)//Birime başlama tarihi
    //    //        sql = "SELECT BASLAMATARIHI FROM IKYPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MAX(BASLAMATARIHI) FROM IKYPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";

    //    //    return veriErisim.TekKolonSQLDate(sql);
    //    //}

    //}
}
