using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Veritabanı tablo isimlerini tutan nesne
    /// </summary>
    public class DBDegiskeni
    {
        /// <summary>
        /// 
        /// </summary>
        public DBDegiskeni() { }

        //View lar
        /// <summary>
        /// HRCPERUNVANVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADUNVAN = "HRCPERUNVANVIEW";   
        /// <summary>
        /// HRCPERBANKASUBEVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADBANKASUBE = "HRCPERBANKASUBEVIEW";
        /// <summary>
        /// HRCPERBANKAVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADBANKA = "HRCPERBANKAVIEW";
        /// <summary>
        /// HRCMUHASEBEBIRIMVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADMUHASEBEBIRIM = "HRCMUHASEBEBIRIMVIEW";
        /// <summary>
        /// HRCMUHASEBEVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADMUHASEBE = "HRCMUHASEBEVIEW";
        /// <summary>
        /// HRCSENELIKIZINTOPLAMIVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADSENELIKTOPLAMIZIN = "HRCSENELIKIZINTOPLAMIVIEW";
        /// <summary>
        /// HRCPERKUMULATIFVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADKUMILATIFVERGI = "HRCPERKUMULATIFVIEW";
        /// <summary>
        /// HRCPERPERSONELVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADPERSONEL = "HRCPERPERSONELVIEW";
        /// <summary>
        /// HRCSENELIKEKDERSTOPLAMIVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADSENELIKTOPLAMEKDERS = "HRCSENELIKEKDERSTOPLAMIVIEW";
        
        //ihale sureclerinin ekonmikkod kurumsal kod bilgilerini veririr
        /// <summary>
        /// HRCONAYFORMVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADONAYFORMVIEW = "HRCONAYFORMVIEW";
        /// <summary>
        /// HRCFATURADETAYSURECTOPLAMVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADFATURADETAYSURECTOPLAMVIEW = "HRCFATURADETAYSURECTOPLAMVIEW";
        /// <summary>
        /// HRCGRUPLUFATURADETAYVIEW tablo adı değişkeni
        /// </summary>
        public static string GRUPLUFATURADETAYVIEW = "HRCGRUPLUFATURADETAYVIEW";
        /// <summary>
        /// HRCODEMEEMRITAHAKKUKTOPLAMVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADODEMEEMRITAHAKKUKTOPLAMVIEW = "HRCODEMEEMRITAHAKKUKTOPLAMVIEW";
        /// <summary>
        /// HRCGRUPLUODEMEEMRIVIEW tablo adı değişkeni
        /// </summary>
        public static string GRUPLUODEMEEMRIVIEW = "HRCGRUPLUODEMEEMRIVIEW";
        /// <summary>
        /// HRCFAGECICIGOREVDETAYVIEW tablo adı değişkeni
        /// </summary>
        public static string GRUPLUGECICIGOREVYOLLUKVIEW = "HRCFAGECICIGOREVDETAYVIEW";
        /// <summary>
        /// HRCFASUREKLIGOREVDETAYVIEW tablo adı değişkeni
        /// </summary>
        public static string GRUPLUSUREKLIGOREVYOLLUKVIEW = "HRCFASUREKLIGOREVDETAYVIEW";
        /// <summary>
        /// HRCGRUPLUURUNVIEW tablo adı değişkeni
        /// </summary>
        public static string GRUPLUURUNVIEW = "HRCGRUPLUURUNVIEW";
        /// <summary>
        /// HRCTASINIRHESAPKOD tablo adı değişkeni
        /// </summary>
        public static string TABLOADTASINIRHESAPKODVIEW = "HRCTASINIRHESAPKOD";
        /// <summary>
        /// PERILDADIGILIMIVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADPERILDADIGILIMIVIEW = "PERILDADIGILIMIVIEW";
        /// <summary>
        /// PERBIRIMDAGILIMIVIEW tablo adı değişkeni
        /// </summary>
        public static string TABLOADPERBIRIMDAGILIMIVIEW = "PERBIRIMDAGILIMIVIEW";
        /// <summary>
        /// HRCPERSONEL tablo adı değişkeni
        /// </summary>
        public static string PERSONEL = "HRCPERSONEL";
        /// <summary>
        /// ButceIslemleri.MUHMIFFORM viewi adı değişkeni
        /// </summary>
        public static string MUHMIFFORM = "MUHMIFFORM";
        /// <summary>
        /// ButceIslemleri.MUHMIF viewi adı değişkeni
        /// </summary>
        public static string MUHMIF = "MUHMIF";
        /// <summary>
        /// HRCMHYALIMVIEW tablo adı değişkeni
        /// </summary>
        public static string VIEWMHYALIM= "HRCMHYALIMVIEW";
        /// <summary>
        /// HRCSURECMUHMIFBAG tablo adı değişkeni
        /// </summary>
        public static string VIEWSURECMUHMIFBAG = "HRCSURECMUHMIFBAG";

    }
}