using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using TNS;
using System.Xml;
using System.Collections;
namespace TNS.DEG
{
    [Serializable()]
    public class DEGServis : MarshalByRefObject, TNS.DEG.IDEGServis
    {
        static System.Xml.XmlDocument xmlDocument;
        static System.Xml.XmlElement degiskenler;
        static Hashtable degiskenVarsayilanlar = null;
        static string dosyaAd = "";
        static DEGServis()
        {
            //OrtakFonksiyonlar.HataDosyaYaz("DegiskenTest.txt", TNS.DEG.Arac.MS() + ": Statik yükleme", true);
            xmlDocument = new System.Xml.XmlDocument();
            dosyaAd = System.Configuration.ConfigurationManager.AppSettings.Get("DegiskenXML");

            try
            {
                System.IO.FileAttributes fa = System.IO.File.GetAttributes(dosyaAd);
                System.IO.File.SetAttributes(dosyaAd, System.IO.FileAttributes.Normal);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("Degisken.xml dosyasına ulaşım sırasında hata olustu:" + e.Message);
            }

            try
            {
                xmlDocument.Load(dosyaAd);
                degiskenler = xmlDocument["DEGISKENLER"];
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz("Degisken.xml dosyasını yükleme sırasında hata olustu:" + e.Message);
                degiskenler = xmlDocument.CreateElement("DEGISKENLER");
            }

            try
            {
                degiskenVarsayilanlar = new Hashtable();

                degiskenVarsayilanlar.Add("/KURUMKOD", "BOŞ");

                degiskenVarsayilanlar.Add("/OrgSema", "0");
                degiskenVarsayilanlar.Add("/KurKaynak", "tcmb");
                degiskenVarsayilanlar.Add("/OFMBAGLANTI", "0");

                degiskenVarsayilanlar.Add("/ButceFisiKesinHesap", "1");
                degiskenVarsayilanlar.Add("/ButceFisiMalzeme", "1");
                degiskenVarsayilanlar.Add("/ButceFisiHarcama", "1");

                degiskenVarsayilanlar.Add("/TasnifSablon1Ad", "Ekonomik 1");
                degiskenVarsayilanlar.Add("/TasnifSablon1Anahtar", "4,1");

                degiskenVarsayilanlar.Add("/TasnifSablon2Ad", "Birim");
                degiskenVarsayilanlar.Add("/TasnifSablon2Anahtar", "1,4");

                degiskenVarsayilanlar.Add("/TasnifSablon3Ad", "Birim; Ekonomik 1");
                degiskenVarsayilanlar.Add("/TasnifSablon3Anahtar", "1,4;4,1");

                degiskenVarsayilanlar.Add("/TasnifSablon4Ad", "Birim; Ekonomik 1; Ekonomik 2");
                degiskenVarsayilanlar.Add("/TasnifSablon4Anahtar", "1,4;4,1;4,2");

                degiskenVarsayilanlar.Add("/TasnifSablon5Ad", "Alt Faaliyetler");
                degiskenVarsayilanlar.Add("/TasnifSablon5Anahtar", "14,1;14,2;4,1");

                degiskenVarsayilanlar.Add("/TasnifSablon6Ad", "Projeler");
                degiskenVarsayilanlar.Add("/TasnifSablon6Anahtar", "P");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD1", "KURUMSAL");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD1/KODYAPI", "99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD1/VTALANADI", "KURAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD1/VTALANKODU", "KURKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD1/VTTABLOADI", "ABSKUR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD2", "FONKSİYONEL");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD2/KODYAPI", "99.9.9.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD2/VTALANADI", "FONAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD2/VTALANKODU", "FONKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD2/VTTABLOADI", "ABSFON");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD3", "FİNANS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD3/KODYAPI", "9");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD3/VTALANADI", "FINAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD3/VTALANKODU", "FINKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD3/VTTABLOADI", "ABSFIN");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD4", "EKONOMİK");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD4/KODYAPI", "99.9.9.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD4/KODYAPIKISA", "99.9");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD4/VTALANADI", "EKOAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD4/VTALANKODU", "EKOKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD4/VTTABLOADI", "ABSEKO");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD5", "GELİR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD5/KODYAPI", "99.9.9.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD5/VTALANADI", "EKOAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD5/VTALANKODU", "EKOKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD5/VTTABLOADI", "ABSGELIRKOD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD6", "FİNANSMAN");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD6/KODYAPI", "9.9.9.9");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD6/VTALANADI", "FINAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD6/VTALANKODU", "FINKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD6/VTTABLOADI", "ABSFINANSMANKOD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD7", "SEKTÖR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD7/KODYAPI", "99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD7/VTALANADI", "SEKTORAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD7/VTALANKODU", "SEKTORKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD7/VTTABLOADI", "ABSSEKTOR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD8", "HESAPKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD8/KODYAPI", "999.99.99.99.99.99.99.99.99.99.99.99.99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD8/VTALANADI", "ACIKLAMA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD8/VTALANKODU", "HESAPKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD8/VTTABLOADI", "ButceIslemleri.dbo.MUHHESAPPLANI");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD9", "EGOSTERGEKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD9/KODYAPI", "99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD9/VTALANADI", "ACIKLAMA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD9/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD9/VTTABLOADI", "EGMKOD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD10", "TASHARCAMABIRIM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD10/KODYAPI", "99.99.99.99.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD10/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD10/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD10/VTTABLOADI", "TASHARCAMABIRIM");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD11", "TASAMBAR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD11/KODYAPI", "99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD11/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD11/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD11/VTTABLOADI", "TASAMBAR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD12", "TASMUHASEBE");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD12/KODYAPI", "99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD12/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD12/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD12/VTTABLOADI", "TASMUHASEBE");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD13", "TASHESAPKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD13/KODYAPI", "999.99.99.99.99.99.99.99.99.99.99.99.99.99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD13/VTALANADI", "ACIKLAMA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD13/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD13/VTTABLOADI", "TASHESAPPLAN");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD14", "PEBFONKSİYONEL");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD14/KODYAPI", "99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD14/VTALANADI", "PEBAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD14/VTALANKODU", "PEBKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD14/VTTABLOADI", "ABSPEBKOD");
                //degiskenVarsayilanlar.Add("/KODYAPI/KOD14/EKSAYFA", "../ButceHazirlikv2/AltFaaliyetTanimlaDetay.aspx");
                //degiskenVarsayilanlar.Add("/KODYAPI/KOD14/EKSAYFASEVIYE", "99.99");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD15", "KURUMSALYAPI");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/KODYAPI", "9.99.999.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/VTTABLOADI", "BHMKURUMSALYAPI");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/VTBAGLANTIADI", "BaglantiSatiriBUM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD15/YILYOK", "1");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD16", "BUTCESENARYO");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD16/KODYAPI", "99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD16/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD16/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD16/VTTABLOADI", "BHMBUTCESENARYO");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD17", "KURUMSALYAPIVIEW");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/KODYAPI", "9.99.999.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/VTTABLOADI", "BHMKURUMSALYAPI");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/VTBAGLANTIADI", "BaglantiSatiriBUM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD17/YILYOK", "1");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD18", "EVRAKBIRIMKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD18/KODYAPI", "9.99.9.999.9.99.9.99.9.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD18/VTALANADI", "AD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD19", "EVRAKDOSYAKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD19/KODYAPI", "999.99.99.99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD19/VTALANADI", "AD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20", "COGRAFYA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/KODYAPI", "99.99.999.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/VTTABLOADI", "ORGCOGRAFYA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/VTBAGLANTIADI", "BaglantiSatiriKYM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/EKALANLAR", "KISAAD,PLAKAKOD,NUFUS,MUHASEBE,MERNISKOD");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/KISAAD/SEVIYE", "99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/KISAAD/MASKE", "S");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/PLAKAKOD/SEVIYE", "99.99.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/PLAKAKOD/MASKE", "S");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/NUFUS/SEVIYE", "99.99.999.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/NUFUS/MASKE", "N");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/MUHASEBE/SEVIYE", "99.99.999/99.99.999.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/MUHASEBE/MASKE", "N");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/MERNISKOD/SEVIYE", "99.99.999/99.99.999.999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD20/MERNISKOD/MASKE", "N");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD21", "ORGBIRIMTUR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/KODYAPI", "99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/VTTABLOADI", "ORGBIRIMTUR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/VTBAGLANTIADI", "BaglantiSatiriKYM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD21/YILYOK", "1");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD22", "STRGENELHAVUZ");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/KODYAPI", "99.999999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/VTTABLOADI", "STRGENELHAVUZ");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/VTBAGLANTIADI", "BaglantiSatiriSTR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD22/YILYOK", "1");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD23", "TAJIKBS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD23/KODYAPI", "999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD23/VTALANADI", "BSAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD23/VTALANKODU", "BS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD23/VTTABLOADI", "ABSBS");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD24", "TAJIKTypeExp");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD24/KODYAPI", "9");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD24/VTALANADI", "TYPEEXPAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD24/VTALANKODU", "TYPEEXP");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD24/VTTABLOADI", "ABSTYPEEXP");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD25", "TAJIKTerr");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD25/KODYAPI", "99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD25/VTALANADI", "TERRAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD25/VTALANKODU", "TERR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD25/VTTABLOADI", "ABSTERR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD26", "TAJIKBudgetParam"); /**HT*/
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/KODYAPI", "99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/VTALANADI", "BPAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/VTALANKODU", "BP");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/VTTABLOADI", "ABSBUDGETPARAMS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/EKALANLAR", "ISLEAF");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/ISLEAF/SEVIYE", "99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD26/ISLEAF/MASKE", "N");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD27", "TAJIKRevAdmins");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD27/KODYAPI", "99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD27/VTALANADI", "ADMAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD27/VTALANKODU", "ADMKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD27/VTTABLOADI", "ABSADMREV");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD28", "TASINMAZCINS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/KODYAPI", "99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/VTTABLOADI", "GELTASINMAZCINS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD28/VTBAGLANTIADI", "BaglantiSatiriGEL");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD29", "EKONIMIKGOSTERGEKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/KODYAPI", "99.99.99.99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/VTALANADI", "ACIKLAMA");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/VTTABLOADI", "EKGKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/EKALANLAR", "BIRIMKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/VTBAGLANTIADI", "BaglantiSatiriEKG");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/BIRIMKOD/SEVIYE", "99/99.99.99.99.99.99.99.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/BIRIMKOD/MASKE", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/BIRIMKOD/ORTAKALAN", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD29/BIRIMKOD/LISTE", "ISTBirim");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD30", "DTVT");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD30/KODYAPI", "99999999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD30/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD30/VTALANKODU", "IDAREKIMLIKKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD30/VTTABLOADI", "EVRDTVT");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD30/VTBAGLANTIADI", "BaglantiSatiriEVR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD31", "ISYERITEHLIKESINIFI");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/KODYAPI", "99.9.9.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/VTALANADI", "TSAD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/VTALANKODU", "TSKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/VTTABLOADI", "TEHLIKESINIF");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/EKALANLAR", "TEHLIKESINIFI");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/TEHLIKESINIFI/SEVIYE", "99.9.9.99");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD31/TEHLIKESINIFI/MASKE", "S");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD32", "ATMBORCKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/KODYAPI", "S");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/VTTABLOADI", "ATMBORCKODU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/VTBAGLANTIADI", "BaglantiSatiriATM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/EKALANLAR", "BORCTURU");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/BORCTURU/MASKE", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD32/BORCTURU/SEVIYE", "/999999999");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD33", "PROGRAM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD33/KODYAPI", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD33/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD33/VTALANKODU", "PRGKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD33/VTTABLOADI", "PRGPROGRAM");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD34", "ALTPROGRAM");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD34/KODYAPI", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD34/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD34/VTALANKODU", "APRGKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD34/VTTABLOADI", "PRGALTPROGRAM");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD35", "FAALIYET");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD35/KODYAPI", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD35/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD35/VTALANKODU", "FAALKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD35/VTTABLOADI", "PRGFAALIYET");
                
                degiskenVarsayilanlar.Add("/KODYAPI/KOD36", "ALTFAALIYET");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD36/KODYAPI", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD36/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD36/VTALANKODU", "AFAALKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD36/VTTABLOADI", "PRGALTFAALIYET");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD37", "PROGRAMTUR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD37/KODYAPI", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD37/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD37/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD37/VTALANKODU", "PTKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD37/VTTABLOADI", "PRGPROGRAMTUR");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD38", "PROGRAMTUR");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/KODYAPI", "9.99.9.9.9.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/YILYOK", "1");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/VTALANADI", "AD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/VTALANKODU", "KOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/VTTABLOADI", "HRCOKAS");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/VTBAGLANTIADI", "BaglantiSatiriHRC");

                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/EKALANLAR", "KODLEVEL,KALEMTURU,OKASKOD");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/KODLEVEL/SEVIYE", "9/9.99.9.9.9.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/KODLEVEL/MASKE", "N");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/KALEMTURU/SEVIYE", "9/9.99.9.9.9.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/KALEMTURU/MASKE", "S");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/OKASKOD/SEVIYE", "9/9.99.9.9.9.99999");
                degiskenVarsayilanlar.Add("/KODYAPI/KOD38/OKASKOD/MASKE", "S");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP0/YAPI", "1,2,3,4");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP0/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP0/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP1/YAPI", "1,2,3,4");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP1/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP1/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP2/YAPI", "1,2,3,4,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP2/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP2/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP3/YAPI", "1,5");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP3/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP3/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP4/YAPI", "1,5,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP4/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP4/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP5/YAPI", "1,5");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP5/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP5/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP6/YAPI", "1,6");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP6/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP6/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP7/YAPI", "1,6");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP7/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP7/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP8/YAPI", "1,6");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP8/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP8/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP9/YAPI", "1,2,3,4,7");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP9/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP9/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP10/YAPI", "1,2,3,4");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP10/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP10/KISAEKO", "1");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP11/YAPI", "8");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP11/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP11/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP12/YAPI", "1,4");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP12/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP12/KISAEKO", "1");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP13/YAPI", "13");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP13/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP13/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP14/YAPI", "1,2,3,4,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP14/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP14/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP15/YAPI", "1,2,3,4,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP15/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP15/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP16/YAPI", "1,2,3,4,14,17");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP16/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP16/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP17/YAPI", "1,2,3,4,14,17");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP17/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP17/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP18/YAPI", "1,5,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP18/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP18/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP19/YAPI", "1,5,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP19/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP19/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP20/YAPI", "1,2,3,4,14,17");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP20/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP20/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP24/YAPI", "1,2,3,4,14,17");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP24/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP24/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP25/YAPI", "1,2,3,14,23,24,25");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP25/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP25/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP26/YAPI", "1,2,3,4,14,23,24,25");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP26/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP26/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP32/YAPI", "1,2,3,4,14,17");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP32/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP32/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP33/YAPI", "1,2,3,4,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP33/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP33/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP34/YAPI", "1,5,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP34/KURUMMU", "1");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP34/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP35/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP35/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP35/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP36/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP36/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP36/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP37/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP37/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP37/KISAEKO", "0");
                
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP38/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP38/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP38/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP39/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP39/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP39/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP40/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP40/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP40/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP41/YAPI", "1,2,3,4,14");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP41/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP41/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP42/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP42/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP42/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP43/YAPI", "37,33,34,35,36,1,3,4,14,2");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP43/KURUMMU", "0");
                degiskenVarsayilanlar.Add("/TERTIPYAPI/TERTIP43/KISAEKO", "0");

                degiskenVarsayilanlar.Add("/MODUL/ORG", "1");
                degiskenVarsayilanlar.Add("/MODUL/BHM", "1");
                degiskenVarsayilanlar.Add("/MODUL/BUM", "1");
                degiskenVarsayilanlar.Add("/MODUL/MUH", "1");
                degiskenVarsayilanlar.Add("/MODUL/HRC", "1");
                degiskenVarsayilanlar.Add("/MODUL/TMM", "1");
                degiskenVarsayilanlar.Add("/MODUL/PER", "1");
                degiskenVarsayilanlar.Add("/MODUL/EVR", "0");
                degiskenVarsayilanlar.Add("/MODUL/KTP", "0");
                degiskenVarsayilanlar.Add("/MODUL/YAT", "1");
                degiskenVarsayilanlar.Add("/MODUL/ATM", "1");
                degiskenVarsayilanlar.Add("/MODUL/KHM", "1");
                degiskenVarsayilanlar.Add("/MODUL/PEB", "1");
                degiskenVarsayilanlar.Add("/MODUL/STRKur", "1");
                degiskenVarsayilanlar.Add("/MODUL/STRBir", "0");
                degiskenVarsayilanlar.Add("/MODUL/GEL", "1");
                degiskenVarsayilanlar.Add("/MODUL/KPS", "1");
                degiskenVarsayilanlar.Add("/MODUL/BHMTjk", "0");
                degiskenVarsayilanlar.Add("/MODUL/MVZ", "0");
                degiskenVarsayilanlar.Add("/MODUL/EKG", "0");
                degiskenVarsayilanlar.Add("/MODUL/TRM", "0");
                degiskenVarsayilanlar.Add("/MODUL/SB", "0");
                degiskenVarsayilanlar.Add("/MODUL/TFT", "0");
                degiskenVarsayilanlar.Add("/MODUL/TMZ", "0");//**ht:Taşınmaz
                degiskenVarsayilanlar.Add("/PerformansLP", "c:/inetpub/wwwroot/sgb.net/PerformansLP");

                degiskenVarsayilanlar.Add("/Servisler", "1,2,3,4,5,6");
                degiskenVarsayilanlar.Add("/Servisler/1/DLLAd", "AktarimServis");
                degiskenVarsayilanlar.Add("/Servisler/2/DLLAd", "POPServis");
                degiskenVarsayilanlar.Add("/Servisler/3/DLLAd", "OCRServis");
                degiskenVarsayilanlar.Add("/Servisler/4/DLLAd", "ChatServis");
                degiskenVarsayilanlar.Add("/Servisler/5/DLLAd", "WorkflowServis");
                degiskenVarsayilanlar.Add("/Servisler/6/DLLAd", "TJWindowServices");
            }
            catch { }
        }

        private static XmlNode YolEkle(XmlNode ustEleman, string yolTur, string yolDeger, string ozellikAd)
        {
            XmlNode xn = xmlDocument.CreateNode(XmlNodeType.Element, yolTur, "");
            XmlAttribute attr = xmlDocument.CreateAttribute(ozellikAd);
            attr.Value = yolDeger;
            xn.Attributes.Append(attr);
            ustEleman.AppendChild(xn);
            return xn;
        }

        static XmlNode YilBul(int yil, bool yoksaAc)
        {
            if (degiskenler == null) return null;
            XmlNode cocuk = degiskenler.FirstChild;
            while (cocuk != null)
            {
                XmlAttribute ozellik = cocuk.Attributes["DEGER"];
                if (ozellik != null)
                {
                    string iText = ozellik.InnerText;
                    if (iText.Equals(yil.ToString())) return cocuk;
                }
                cocuk = cocuk.NextSibling;
            }
            if (yoksaAc)
            {
                return YolEkle(degiskenler, "YIL", yil.ToString(), "DEGER");
            }
            else return null;
        }

        static XmlNode IcYolBul(XmlNode yilNode, string yol, bool yoksaAc)
        {
            if (yilNode == null) return null;
            if (yol == "") return yilNode;
            XmlNode cocuk = yilNode.FirstChild;
            while (cocuk != null)
            {
                if (cocuk.Name.Equals("YOL"))
                {
                    XmlAttribute ozellik = cocuk.Attributes["DEGER"];
                    if (ozellik != null && ozellik.Value.Equals(yol)) return cocuk;
                }
                cocuk = cocuk.NextSibling;
            }
            if (yoksaAc)
            {
                return YolEkle(yilNode, "YOL", yol, "DEGER");
            }
            else return null;
        }

        static XmlNode YolBul(XmlNode yilNode, string yol, bool yoksaAc)
        {
            if (yol == "") return yilNode;
            string[] yolBolumler = yol.Split('\\', '/');
            XmlNode icYol = yilNode;
            for (int i = 0; i < yolBolumler.Length; i++)
            {
                icYol = IcYolBul(icYol, yolBolumler[i], yoksaAc);
            }
            return icYol;
        }

        public static XmlNode DegiskenBul(XmlNode yolNode, string degisken)
        {
            XmlNode cocuk = yolNode.FirstChild;
            while (cocuk != null)
            {
                if (cocuk.Name.Equals("DEGISKEN"))
                {
                    XmlAttribute ozellik = cocuk.Attributes["AD"];
                    if (ozellik != null && ozellik.Value.Equals(degisken)) return cocuk;
                }
                cocuk = cocuk.NextSibling;
            }
            return null;
        }

        static XmlNode DegiskenNodeBulIc(int yil, string degiskenYol, out XmlNode yolNode, out string degisken)
        {
            yolNode = null;
            degisken = null;
            XmlNode yilNode = YilBul(yil, true);
            if (yilNode == null) return null;
            string yol = System.IO.Path.GetDirectoryName(degiskenYol);
            degisken = System.IO.Path.GetFileName(degiskenYol);
            yolNode = YolBul(yilNode, yol, true);
            if (yolNode == null) return null;
            XmlNode degiskenNode = DegiskenBul(yolNode, degisken);
            return degiskenNode;
        }

        static string DegiskenDegerBulIc(int yil, string degiskenYol)
        {
            XmlNode yilNode = YilBul(yil, false);
            if (yilNode == null) return null;
            string yol = System.IO.Path.GetDirectoryName(degiskenYol);
            string degisken = System.IO.Path.GetFileName(degiskenYol);
            XmlNode yolNode = YolBul(yilNode, yol, false);
            if (yolNode == null) return null;
            XmlNode degiskenNode = DegiskenBul(yolNode, degisken);
            if (degiskenNode == null) return null;
            XmlAttribute ozellik = degiskenNode.Attributes["DEGER"];
            if (ozellik != null)
            {
                return ozellik.Value;
            }
            return null;
        }

        public string DegiskenDegerBul(int yil, string degiskenYol)
        {
            if (string.IsNullOrWhiteSpace(degiskenYol)) return "";

            //CACHE**************************
            object cachedObjectData = TNS.Onbellek.Get("DEG_" + yil + "_" + degiskenYol);
            if (cachedObjectData != null)
            {
                //OrtakFonksiyonlar.HataDosyaYaz("cacheDEG.txt", "Cache den Okundu:" + degiskenYol, false);
                return (string)(cachedObjectData + "");
            }
            //*******************************

            string sonuc = null;
            if (yil > 0)
                sonuc = DegiskenDegerBulIc(yil, degiskenYol);
            if (String.IsNullOrEmpty(sonuc)) sonuc = DegiskenDegerBulIc(0, degiskenYol);
            if (String.IsNullOrEmpty(sonuc))
            {
                if (!degiskenYol.StartsWith("/")) degiskenYol = "/" + degiskenYol;
                try
                {
                    sonuc = (string)degiskenVarsayilanlar[degiskenYol];
                }
                catch
                {
                    sonuc = null;
                }
            }

            TNS.Onbellek.Remove("DEG_" + yil + "_" + degiskenYol);
            TNS.Onbellek.Insert("DEG_" + yil + "_" + degiskenYol, sonuc + "");
            //OrtakFonksiyonlar.HataDosyaYaz("cacheDEG.txt", "Cache e yazıldı:" + degiskenYol + "*" + sonuc + "" + "*", false);
            return sonuc;
        }

        public void DegiskenDegerAta(int yil, string degiskenYol, string deger)
        {
            XmlNode ustNode;
            string degiskenAd;
            XmlNode node = DegiskenNodeBulIc(yil, degiskenYol, out ustNode, out degiskenAd);
            if (ustNode == null) return;
            if (node == null)
            {
                node = YolEkle(ustNode, "DEGISKEN", degiskenAd, "AD");
                if (node == null) return;
            }
            XmlAttribute attr = node.Attributes["DEGER"];
            if (attr == null)
            {
                attr = xmlDocument.CreateAttribute("DEGER");
                node.Attributes.Append(attr);
            }
            attr.Value = deger;
            xmlDocument.Save(dosyaAd);
            TNS.Onbellek.Remove("DEG_" + yil + "_" + degiskenYol);
        }
    }
}