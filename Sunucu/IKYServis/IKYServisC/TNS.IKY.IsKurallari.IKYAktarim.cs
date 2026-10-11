using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.IKY.IsKurallari.Veri;
using System.Data;
using System.Collections;

namespace TNS.IKY.IsKurallari
{
    public class IKYAktarim
    {
        public static Sonuc TumVeriyiAktar(string kullaniciKodu)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";
            for (int i = 4; i < 29; i++)
            {
                if (i == 21 || i == 22 || i == 24)
                    continue;
                sonuc = CBVeriAktar(i, kullaniciKodu);
                if (!sonuc.islemSonuc)
                    hata += "\n" + sonuc.hataStr;
            }

            sonuc = SiteGiderTablosuAktar(kullaniciKodu);
            if (!sonuc.islemSonuc)
                hata += "\n" + sonuc.hataStr;
            //sonuc = PersonelSiteKesintisiAktar(kullaniciKodu);
            //if (!sonuc.islemSonuc)
            //    hata += "\n" + sonuc.hataStr;

            if (!string.IsNullOrWhiteSpace(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }
            return sonuc;
        }

        public static Sonuc LojmanTanimVerileriniAktar(string kullaniciKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", kullaniciKodu);

            Sonuc sonuc = new Sonuc();
            string sql = "";

            sql = LojmanSiteAktar();

            try
            {
                Sonuc s = Calistir(ve, sql);
                ve.hata = "";
                if (s.islemSonuc)
                {
                    sql = LojmanSiteBlokAktar();
                    s = Calistir(ve, sql);
                    ve.hata = "";
                    if (!s.islemSonuc)
                    {
                        s.DegerAta("Birinci işlem tamamlandı fakat ikinci işlemde hata oluştu:" + s.hataStr);
                    }
                    else
                    {
                        sql = LojmanSiteBlokDaireAktar();
                        s = Calistir(ve, sql);
                        ve.hata = "";
                        if (!s.islemSonuc)
                        {
                            s.DegerAta("1. ve 2. işlem tamamlandı fakat 3. işlemde hata oluştu:" + s.hataStr);
                        }
                    }
                }
                return s;
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.bilgiStr = ex.Message;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc CBVeriAktar(int tip, string kullaniciKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", kullaniciKodu);

            Sonuc sonuc = new Sonuc();
            string[] sqller = null;
            string sql = "";
            string sql2 = "";
            Sonuc s = new Sonuc();
            switch (tip)
            {
                case 1:
                    sql = ORGBirimSorgu();
                    break;
                case 2:
                    sql = ORGBirimTurSorgu();
                    break;
                case 3:
                    sql = IKYUnvanSorgu();
                    break;
                case 4:
                    sql = IKYPersonelSorgu();
                    break;
                case 5:
                    sql = IKYPersonelNufusSorgu();
                    break;
                case 6:
                    sql = IKYPersonelAileSorgu();
                    break;
                case 7:
                    sql = IKYPersonelAskerlikSorgu();
                    break;
                case 8:
                    sql = IKYPersonelOgrenimSorgu();
                    break;
                case 9:
                    sql = IKYPersonelDilSorgu();
                    break;
                case 10:
                    sql = IKYPersonelKursSorgu();
                    break;
                case 11:
                    sql = IKYPersonelKadroSorgu();
                    sql2 = IKYPersonelKadroKontenjanSorgu();
                    break;
                case 12:
                    sql = IKYPersonelIletisimSorgu();
                    break;
                case 13:
                    sql = IKYPersonelCezaOdulSorgu();
                    break;
                case 14:
                    sql = IKYPersonelKartSorgu();
                    break;
                case 15:
                    return new Sonuc();//sql = IKYHizmetSebebiSorgu();
                //break;
                case 16:
                //sql = IKYSozlukEskiHizmetSorgu();
                //break;
                case 17:
                    //sql = IKYEskiHizmetSorgu();
                    return new Sonuc();
                case 18:
                    sql = IKYPersonelResimSorgu();
                    break;
                case 19:
                    sql = IKYPersonelIzinSorgu();
                    break;
                case 20:
                    sql = IKYPersonelIzinDevirSorgu();
                    break;
                case 23:
                    sql = LojmanPersonelAktar();
                    break;
                case 25:
                    sql = PersonelSiteKesintisiSorgu();
                    break;
                case 26:
                    sqller = MaasBilgileriAktarSql();
                    break;
                case 27:
                    sql = MaasMatrahAktarSql();
                    break;
                case 28:
                    sql = YemekAktarSql();
                    break;
                case 29:
                    sql = DosyadanAktar(ve, s);
                    break;
            }

            if (sqller == null)
            {
                sqller = new string[2];
                sqller[0] = sql;
                sqller[1] = sql2;
            }

            try
            {
                foreach (string sqlIc in sqller)
                {
                    if (string.IsNullOrWhiteSpace(sqlIc)) continue;
                    s = Calistir(ve, sqlIc);
                    if (!s.islemSonuc) break;
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            ve.VeriErisimKapat();
            return s;
        }

        private static string DosyadanAktar(VeriErisim ve, Sonuc sonuc)
        {
            /*
Aşağıdaki alanlar aktarım sistemi tarafından tanınmaktadır. 
Bu alanlar aralarına boşluk konularak ve türkçe karakterlere uygun şekilde büyük harflerle de yazılabilir.
Bu alanların yanısına bordro parameterleri # karakteri ile başlatılarak aktarım dosyasına konulabilir:
             Not 1:Birim adı birimin veri tabanındaki idsi olabilir.
             Not 2:id alanı yoksa sistem otomatik sıradan id üretir.
             Not 3: Adres ile Adres1 ve Adres2 alanları birlikte çalışmaz. Öncelik Adres alanındadır.
             Not 4: Cinsiyet E ile ve K ile başlamalı yada 2(E) veya 3(K) olmalıdır
birimadı 
id 
kimlikno 
sicilno 
ad 
soyad 
geldigikurumsicili 
vergidairesi 
ssksicilno 
akademikunvanı 
ıban 
cinsiyet 
babaad 
anaad 
doğumyeri 
doğumtarihi 
kangrubu 
cüzdankayıtno 
cüzdanveriliştarihi 
ilksoyad 
cüzdanverildigiyer 
cüzdanverilişnedeni 
nüfusilçekod 
nüfusilkod 
nüfusmahalleköy 
nüfusciltNo 
nüfusailesırano 
nüfussırano 
giriştarihi 
çıkıştarihi 
sonatamatarihi 
adres 
adres1
adres2 
eposta 
iştelefon 
ceptelefon  
              
personeltip
unvan
kadroDerece
vekilunvankod
             
             * */
            string sql1 = @"TRUNCATE TABLE IKYPERSONELAILE
                            TRUNCATE TABLE IKYPERSONELASKERLIK
                            TRUNCATE TABLE IKYPERSONELBELGELER
                            TRUNCATE TABLE IKYPERSONELDIL
                            TRUNCATE TABLE IKYPERSONELOGRENIM
                            TRUNCATE TABLE IKYPERSONELNUFUS
                            TRUNCATE TABLE IKYPERSONELKURS
                            TRUNCATE TABLE IKYPERSONELILETISIM
                            TRUNCATE TABLE IKYPERSONELIZIN
                            TRUNCATE TABLE IKYPERSONELIZINDEVIR
                            TRUNCATE TABLE IKYTERFI
                            TRUNCATE TABLE PERPERSONELMAASPARAM
                            DELETE IKYPERSONELKADRO
                            TRUNCATE TABLE IKYPERSONELKART
                            TRUNCATE TABLE IKYPERSONELCEZAODUL
                            TRUNCATE TABLE IKYPERSONELRESIM
                            TRUNCATE TABLE IKYPERSONELKESINTI
                            TRUNCATE TABLE IKYPERSONELTARIHCE
                            DELETE IKYPERSONELLOJMAN
                            DELETE IKYPERSONEL;";
            ve.ExecuteNonquery(sql1);
            if (!string.IsNullOrWhiteSpace(ve.hata))
            {
                sonuc.DegerAta(ve.hata);
                return "";
            }

            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("DegiskenXML");
            yazYol = System.IO.Path.GetDirectoryName(yazYol);//Degisken xml yolu


            Hashtable ht = new Hashtable();
            TNS.TxtDosya kaynakDosya = null;
            try
            {
                kaynakDosya = new TxtDosya(yazYol + "\\PersonelListe.txt", '\t', ht);
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
                return "";
            }

            TxtDosyaSatir alanS = kaynakDosya.SatirOku();
            if (alanS == null)
            {
                sonuc.DegerAta("Dosyanın ilk satırı alan isimleri olmalıdır.");
                return "";
            }
            List<string> maasParam = new List<string>();
            for (int i = 0; i < alanS.parcalar.Length; i++)
            {
                string alanAd = alanS.parcalar[i];
                if (alanAd.StartsWith("#"))
                    maasParam.Add(alanAd);
                else
                    alanAd = alanAd.Replace(" ", "").ToLower();
                ht[alanAd] = TxtDosyaAlanTanimOl(i);
            }

            TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();
            TABPersonel tabPer = new TABPersonel(ve);
            TABPersonelKadro tabKad = new TABPersonelKadro(ve);
            PERPERSONELMAASPARAM tabPar = new PERPERSONELMAASPARAM(ve);
            Sonuc s = new Sonuc();
            ORG.OrganizasyonBirim aktifBirim = null;
            int say = 0;
            Hashtable birimHT = new Hashtable();
            int yer = -5;
            try
            {
                while (true)
                {
                    yer = -5;
                    TxtDosyaSatir simdS = kaynakDosya.SatirOku();
                    if (simdS == null) break;
                    string yeniBirimAd = simdS.ParcaStr("birimadı");
                    if (string.IsNullOrWhiteSpace(yeniBirimAd))
                    {
                        yeniBirimAd = "TANIMSIZ";
                    }
                    aktifBirim = BirimBul(yeniBirimAd, s, birimHT, orgServis);

                    Personel p = new Personel();
                    p.kod = simdS.ParcaInt("id");
                    if (p.kod == 0)
                        p.kod = ++say;
                    p.TCKIMLIKNO = simdS.ParcaStr("kimlikno");
                    if (string.IsNullOrWhiteSpace(p.TCKIMLIKNO))
                        p.TCKIMLIKNO = p.kod.ToString();
                    p.sicilNo = simdS.ParcaStr("sicilno");
                    p.AD = simdS.ParcaStr("ad");
                    p.SOYAD = simdS.ParcaStr("soyad");
                    p.geldigiKurumSicili = simdS.ParcaStr("geldigikurumsicili");
                    p.vergiDairesi = simdS.ParcaStr("vergidairesi");
                    p.sskSicilNo = simdS.ParcaStr("ssksicilno");
                    p.titr = simdS.ParcaStr("akademikunvanı");
                    p.bankaIbanNo = simdS.ParcaStr("ıban");

                    string cinsiyet = simdS.ParcaStr("cinsiyet");
                    if (cinsiyet.StartsWith("K")) cinsiyet = "3";
                    else if (cinsiyet.StartsWith("E")) cinsiyet = "2";
                    p.nufus.kod = p.kod;
                    p.nufus.cinsiyeti = (ENUMCINSIYET)OrtakFonksiyonlar.ConvertToInt(cinsiyet, 0);
                    p.nufus.babaAd = simdS.ParcaStr("babaad");
                    p.nufus.anaAd = simdS.ParcaStr("anaad");
                    p.nufus.dogumYeri = simdS.ParcaStr("doğumyeri");
                    p.nufus.dogumTarihi = simdS.ParcaDT("doğumtarihi");
                    p.nufus.kanGrubu = simdS.ParcaStr("kangrubu");
                    p.nufus.cuzdanKayitNo = simdS.ParcaStr("cüzdankayıtno");
                    p.nufus.cuzdanVerilisTarihi = simdS.ParcaDT("cüzdanveriliştarihi");
                    p.nufus.ilkSoyad = simdS.ParcaStr("ilksoyad");
                    p.nufus.cuzdanVerildigiYer = simdS.ParcaStr("cüzdanverildigiyer");
                    p.nufus.cuzdanVerilisNedeni = simdS.ParcaStr("cüzdanverilişnedeni");
                    p.nufus.nufusIlceKod = simdS.ParcaStr("nüfusilçekod");
                    p.nufus.nufusIlKod = simdS.ParcaStr("nüfusilkod");
                    p.nufus.nufusMahalleKoy = simdS.ParcaStr("nüfusmahalleköy");
                    p.nufus.nufusCiltNo = simdS.ParcaStr("nüfusciltNo");
                    p.nufus.nufusAileSiraNo = simdS.ParcaStr("nüfusailesırano");
                    p.nufus.nufusSiraNo = simdS.ParcaStr("nüfussırano");

                    TNSDateTime dtGiris = BinDokuzyuzNull(simdS.ParcaDT("giriştarihi"));
                    TNSDateTime dtCikis = BinDokuzyuzNull(simdS.ParcaDT("çıkıştarihi"));
                    TNSDateTime dtSonAtama = BinDokuzyuzNull(simdS.ParcaDT("sonatamatarihi"));

                    List<PersonelKadro> perKad = new List<PersonelKadro>();

                    if (dtGiris == null || dtGiris.isNull || dtGiris.Yil < 1950)
                    {
                        dtGiris = new TNSDateTime("01.01.2015");
                    }

                    if (dtSonAtama != null && !dtSonAtama.isNull && dtSonAtama.Oku() > dtGiris.Oku())
                    {
                        KadroyaEkle(simdS, perKad, dtGiris, dtSonAtama, false, aktifBirim, p.kod);
                        dtGiris = dtSonAtama;
                    }
                    KadroyaEkle(simdS, perKad, dtGiris, dtCikis, true, aktifBirim, p.kod);

                    p.iletisim.adres = simdS.ParcaStr("adres");
                    if (string.IsNullOrWhiteSpace(p.iletisim.adres))
                    {
                        p.iletisim.adres = simdS.ParcaStr("adres1") + " " + simdS.ParcaStr("adres2");
                        p.iletisim.adres = p.iletisim.adres.Trim();
                    }
                    p.iletisim.eposta = simdS.ParcaStr("eposta");
                    p.iletisim.isTelefon = simdS.ParcaStr("iştelefon");
                    p.iletisim.cepTelefon = simdS.ParcaStr("ceptelefon");
                    if (s.islemSonuc)
                    {
                        yer = 1;
                        s = tabPer.Kaydet(p, (dtCikis == null || dtCikis.isNull) ? 1 : 0);
                    }
                    if (s.islemSonuc)
                    {
                        yer = 2;
                        s = tabPer.KaydetNufus(p.nufus, true);
                    }
                    foreach (PersonelKadro item in perKad)
                    {
                        if (s.islemSonuc)
                        {
                            yer = 3;
                            s = tabKad.KaydetKadro(item);
                        }
                    }
                    MaasParam mpItem = new MaasParam();
                    foreach (string item in maasParam)
                    {
                        mpItem.degiskenAd = item.Substring(1);
                        mpItem.degerDbl = OrtakFonksiyonlar.ConvertToDbl(simdS.ParcaStr(item));
                        if (s.islemSonuc && mpItem.degerDbl > 0)
                        {
                            yer = 4;
                            s = tabPar.EkleParam(p.tcKimlikNo, mpItem);
                        }
                    }
                    if (!s.islemSonuc)
                        break;
                    yer = -5;
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            if (!s.islemSonuc)
                sonuc.DegerAta(yer.ToString() + ". noktada hata oldu: " + s.hataStr);
            kaynakDosya.Kapat();
            return "";
        }

        private static TNSDateTime BinDokuzyuzNull(TNSDateTime dt)
        {
            if (dt == null || dt.isNull) return dt;
            if (dt.Yil < 1901)
            {
                dt.isNull = true;
            }
            return dt;
        }

        private static object TxtDosyaAlanTanimOl(int i)
        {
            TxtDosyaAlanTanim at = new TxtDosyaAlanTanim();
            at.alanYer = i;
            at.defDeger = "";
            return at;
        }

        private static ORG.OrganizasyonBirim BirimBul(string yeniBirimAd, Sonuc sonuc, Hashtable birimHT, ORG.IORGServis orgServis)
        {
            ORG.OrganizasyonBirim ob = (ORG.OrganizasyonBirim)birimHT[yeniBirimAd];
            if (ob != null) return ob;
            string oncekiBirimAd = yeniBirimAd;
            bool birimAddan = true;
            if (OrtakFonksiyonlar.ConvertToInt(oncekiBirimAd, 0) > 0)
                birimAddan = false;

            ORG.OrganizasyonBirim bulBirim = null;
            ob = new ORG.OrganizasyonBirim();
            if (birimAddan)
                ob.adi = oncekiBirimAd;
            else
                ob.kod = OrtakFonksiyonlar.ConvertToInt(oncekiBirimAd, 0);

            if (!string.IsNullOrWhiteSpace(oncekiBirimAd))
            {
                ObjectArray oa = orgServis.OrganizasyonBirimListele(ob, false, false);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                    bulBirim = (ORG.OrganizasyonBirim)oa.objeler[0];
                else
                    sonuc.bilgiStr += oncekiBirimAd + " bulunamadı!";
            }

            if (bulBirim == null)
            {
                bulBirim = new ORG.OrganizasyonBirim();
                bulBirim.kod = 1;
                bulBirim.muhasebekodu = "1";
                bulBirim.muhasebebirim = "000";
            }
            birimHT[yeniBirimAd] = bulBirim;
            return bulBirim;
        }

        private static void KadroyaEkle(TxtDosyaSatir simdS, List<PersonelKadro> perKad, TNSDateTime dtGiris, TNSDateTime dtCikis, bool aktifMi, ORG.OrganizasyonBirim aktifBirim, int perKod)
        {
            PersonelKadro p_KADRO = new PersonelKadro();
            p_KADRO.personelTip = OrtakFonksiyonlar.ConvertToInt(simdS.ParcaStr("personeltip"), 0);
            if (p_KADRO.personelTip == 0) p_KADRO.personelTip = 100;

            p_KADRO.unvan = OrtakFonksiyonlar.ConvertToInt(simdS.ParcaStr("unvan"), 0);
            p_KADRO.kadroDerece = OrtakFonksiyonlar.ConvertToInt(simdS.ParcaStr("kadroDerece"), 0);
            p_KADRO.vekilUnvanKod = OrtakFonksiyonlar.ConvertToInt(simdS.ParcaStr("vekilunvankod"), 0);

            if (aktifMi)
            {
                if (dtCikis == null || dtCikis.isNull)
                    p_KADRO.kadroDurum = ENUMKadroDurumu.AKTIFKAYIT;
                else
                    p_KADRO.kadroDurum = ENUMKadroDurumu.AKTIFKAPANDI;
            }
            else
                p_KADRO.kadroDurum = ENUMKadroDurumu.AYRILDI;
            p_KADRO.atamaTarihi = dtGiris;
            p_KADRO.ayrilmaTarihi = dtCikis;
            p_KADRO.personelKod = perKod;
            p_KADRO.kod = perKod * 2;
            if (aktifMi)
                p_KADRO.kod++;
            p_KADRO.organizasyonKodu = aktifBirim.kod;
            p_KADRO.muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(aktifBirim.muhasebekodu, 0);
            p_KADRO.muhasebeBirimi = aktifBirim.muhasebebirim;
            perKad.Add(p_KADRO);
        }

        private static string MaasMatrahAktarSql()
        {
            return @"DELETE FROM PERPERSONELMAASMATRAH where YIL=2016 AND SURECNO='000000000';
                     INSERT INTO  PERPERSONELMAASMATRAH(TCKIMLIKNO, YIL, SURECNO, MATRAHTUR, TARIH, DEGER, TABLOAD, SATIRNO) SELECT p.personelTcNo, 2016, '000000000', 1, '2016-08-16', MAX(bordroYillikMatrah - bordroAylikMatrah), '', 0 FROM CBPersonel2.dbo.bordro a inner join CBPersonel2.dbo.personel p on p.pkPersonel = a.bordroPersonelPk where len(p.personelTcNo)>1 and p.personelAktif=1 and bordroYil=2016 and bordroAy=9 group by p.personelTcNo";
        }

        private static string[] MaasBilgileriAktarSql()
        {
            string[] sqller = new string[18];
            sqller[0] = "delete from PERPERSONELMAASPARAM where DEGISKEN IN ('ucretGostergesi', 'tazminatGostergesi', 'ozelHizmetTazminatiOrani', 'ekOdemeOrani', 'emsalOzelHizmetTazminatiOrani', 'isGuclugu', 'ilaveEkOdemeYok', 'ayliktanKesmeCezasi', 'sahisBorcu', 'icra', 'nafakaKesintisi', 'sakatlikOrani', 'sozlesmeUcreti', 'kefaletBaslamaAyi', 'kefaletBaslamaYili', 'bordroDisi', 'KHK666')";

            sqller[1] = MaasParametreAktarimOl("maasAyarAylikGosterge", "ucretGostergesi", "");
            sqller[2] = MaasParametreAktarimOl("maasAyarOzelHizmetOdemeOrani", "tazminatGostergesi", "maasAyarOzelHizmetOdemeOrani>1000");
            sqller[3] = MaasParametreAktarimOl("maasAyarOzelHizmetOdemeOrani*100", "ozelHizmetTazminatiOrani", "maasAyarOzelHizmetOdemeOrani<1000");
            sqller[4] = MaasParametreAktarimOl("maasAyarEkOdemeOrani*100", "ekOdemeOrani", "");
            sqller[5] = MaasParametreAktarimOl("maasAyarEmsalOzelHizmetOrani", "emsalOzelHizmetTazminatiOrani", "");
            sqller[6] = MaasParametreAktarimOl("maasAyarYanOdemeOrani", "isGuclugu", "");
            sqller[7] = MaasParametreAktarimOl("maasAyarIlaveEkOdemeOrani*-1", "ilaveEkOdemeYok", "");
            sqller[8] = MaasParametreAktarimOl("maasAyarAyliktanKesmeCezasiOrani", "ayliktanKesmeCezasi", "");
            sqller[9] = MaasParametreAktarimOl("maasAyarKisiBorcu", "sahisBorcu", "");
            sqller[10] = MaasParametreAktarimOl("maasAyarIcra", "icra", "");
            sqller[11] = MaasParametreAktarimOl("maasAyarNafaka", "nafakaKesintisi", "");
            sqller[12] = MaasParametreAktarimOl("maasAyarSakatlikOrani", "sakatlikOrani", "");
            sqller[13] = MaasParametreAktarimOl("maasAyarSozlesmeUcretiOdemeOrani", "sozlesmeUcreti", "");
            sqller[14] = MaasParametreAktarimOl("DATEPART(month,  maasAyarKefaletBaslamaTarihi)", "kefaletBaslamaAyi", "");
            sqller[15] = MaasParametreAktarimOl("DATEPART(year,  maasAyarKefaletBaslamaTarihi)", "kefaletBaslamaYili", "");
            sqller[16] = MaasParametreAktarimOl("maasAyarBordroDisi", "bordroDisi", "");
            sqller[17] = "INSERT INTO PERPERSONELMAASPARAM SELECT distinct p.personelTcNo, 'KHK666', 1 FROM CBPersonel2.dbo.personel p where len(p.personelTcNo)>1 and p.personelKurumBaslamaTarihi>='2012-01-15'";
            //sqller[18] = "INSERT INTO PERPERSONELMAASPARAM SELECT distinct p.personelTcNo, 'KHK666', 1 FROM CBPersonel2.dbo.personel p where len(p.personelTcNo)>1 and p.personelIlkIseBasTarihi>='2012-01-15' AND personelKodPersonelTipiPk = 8 AND personelKodPersonelTipiPk <> 8";
            return sqller;
        }

        private static string MaasParametreAktarimOl(string alan, string yeniAlan, string kosul)
        {
            string sql = "INSERT INTO PERPERSONELMAASPARAM SELECT p.personelTcNo, '" + yeniAlan + "', " + alan + " FROM CBPersonel2.dbo.maasAyar a inner join CBPersonel2.dbo.personel p on p.pkPersonel = a.maasAyarPersonelPk where len(p.personelTcNo)>1 and p.personelAktif=1";
            if (!string.IsNullOrWhiteSpace(kosul)) sql += " AND " + kosul;
            return sql;
        }

        private static Sonuc Calistir(VeriErisim ve, string sql)
        {
            Sonuc s = new Sonuc();
            ve.BeginTransaction();
            ve.ExecuteNonquery(sql);
            if (string.IsNullOrEmpty(ve.hata))
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
                s.DegerAta("Aktarım başarısız. " + ve.hata); ;
            }
            return s;
        }

        private static string ORGBirimSorgu()
        {
            return @"truncate table Kullanici.dbo.ORGBIRIM;
                        insert into Kullanici.dbo.ORGBIRIM (KOD, ADI, KURUMKOD, TURKOD, ANABIRIMKOD, PERSONELMI)
                        select [pkKodBirimler]
                            ,[kodBirimlerEtiket]
                            ,[kodKurumPk]   
                            ,[kodBirimTipiPk]
                            ,[kodCbBirimParent]  
                            ,1
                        from [CBPersonel2].dbo.kodCbBirim 
                        where kodCbBirimAktif = 1;

                        update Kullanici.[dbo].[ORGBIRIM]
                        set [ENTEGREKOD] = [KOD];

                        update Kullanici.dbo.ORGBIRIM
                        set SABLONMU = 0;

                        Declare @varkod varchar(3)
                        SET @varkod = (SELECT kod FROM Kullanici.dbo.ORGBIRIM where ANABIRIMKOD is null);
                        update Kullanici.dbo.ORGBIRIM
                        set KOPRUKOD = @varkod where ANABIRIMKOD is null
                        ;with CTE 
                        as
                        (
                        SELECT ANABIRIMKOD, kod, convert(varchar(15), ANABIRIMKOD) + right('000'+convert(varchar(3), [KOD]),3)  AS kopruKod, 
                            0 as level
                        FROM Kullanici.dbo.ORGBIRIM 
                        WHERE ANABIRIMKOD = @varkod     
                        UNION ALL
                        SELECT c.ANABIRIMKOD, c.kod, convert(varchar(15), CTE.kopruKod) + right('000'+convert(varchar(3), c.[KOD]),3) AS kopruKod,       
                            CTE.level + 1 as level
                        FROM Kullanici.DBO.ORGBIRIM c
                        JOIN CTE
                            ON CTE.kod = c.ANABIRIMKOD    
                        )
                        UPDATE Kullanici.dbo.ORGBIRIM
                        SET KOPRUKOD = CTE.kopruKod
                        FROM Kullanici.dbo.ORGBIRIM AS OB
                        JOIN CTE ON OB.KOD = CTE.KOD";
        }

        private static string ORGBirimTurSorgu()
        {
            return @"truncate table [Kullanici].[dbo].ORGBIRIMTUR;
insert into [Kullanici].[dbo].[ORGBIRIMTUR]
([KOD], AD)
select  [pkLkpDetay], [lkpDetayAdi]
from [CBPersonel2].[dbo].[lkpDetay]
where [lkpDetayParentPk] = '279'";
        }

        private static string IKYUnvanSorgu()
        {
            return @"truncate table [InsanKaynaklari].dbo.IKYUNVAN;
insert into [InsanKaynaklari].[dbo].[IKYUNVAN]
([KOD], [AD], [ADKISA], [SIRANO], [YONETICI], [AKTIF], [HITAPKOD])
select [pkKodUnvan]
      ,[kodUnvanEtiket]
      ,[kodUnvanTipi]
      ,[kodUnvanSira]      
      ,[kodUnvanBirimYonetici], kodUnvanAktifPasif, kodUnvanKodu
from [CBPersonel2].[dbo].[kodUnvan];
UPDATE [InsanKaynaklari].dbo.IKYUNVAN 
SET SINIFKOD = (select KOD from IKYSOZLUK WHERE ADKISA = KISAAD and KAYITKOD LIKE '22.%')";
        }

        private static string IKYPersonelSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELAILE]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELASKERLIK]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELBELGELER]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELDIL]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELOGRENIM]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELNUFUS]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELKURS]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELILETISIM]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELIZIN]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELIZINDEVIR]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYTERFI]
delete [InsanKaynaklari].dbo.[IKYPERSONELKADRO]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELKART]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELCEZAODUL]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELRESIM]
TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELKESINTI]
delete [InsanKaynaklari].dbo.[IKYPERSONELLOJMAN]
DELETE [InsanKaynaklari].dbo.IKYPERSONEL;

insert into [InsanKaynaklari].[dbo].[IKYPERSONEL]
([KOD]
      ,[TCKIMLIKNO]
      ,[AD]
      ,[SOYAD]  
      ,[KURUMSICILNO] 
	  ,[PERSONELTUR]  
      ,[MEMURIYETEBASLAMATARIHI]          
      ,[BANKAIBANNO]
      ,[EMEKLISICILNO]
      ,[SSKSICILNO]
      ,[BAGKURSICILNO] 
      ,[VERGIDAIRESI]
      ,[KURUMABASLAMATARIHI]
      ,[AKTIFMI]
      ,[GELDIGIKURUM]
      ,[RUMUZ]
      ,[TITR])
select [pkPersonel]
      ,[personelTcNo]
      ,[personelAdi]
      ,[personelSoyadi]
      ,[personelSicili]
	  ,[personelKodPersonelTipiPk]   
      ,[personelIlkIseBasTarihi]
      ,[personelBankaIbanNo]
      ,[personelEmekliNo]
      ,[personelSskNo]
      ,[personelBagkurNo]
      ,[personelVergiDairesi]
      ,[personelBaslamaTarihi]
      ,[personelAktif]
      ,[personelGeldigiKurum]
      ,[personelRumuz]
      ,[personelTitle]
from [CBPersonel2].[dbo].[personel];

update IKYPERSONEL set TCKIMLIKNO = TCKIMLIKNO + '_' where KOD IN (55, 61, 223, 1561, 3356, 3679, 3704, 4314, 4342, 4344, 4425, 200012, 200019, 200032, 
200045, 200046, 200058, 200073, 200126, 200171, 200189, 200198, 200199, 200200, 200207, 300009, 300047, 300054, 300069, 300091, 300097, 300113, 300167, 
300168, 300184, 300204, 300205, 300229, 300262, 300263, 300282, 300303, 300771, 301073, 301075, 301089, 301259, 301445, 301499, 301804, 302251, 302277, 
302280, 302295, 302315, 302330, 302336, 302350, 302360, 302373, 302376, 302377, 302378, 302380, 302381, 302393, 302398, 302413, 302431, 302438, 302446, 
302466, 302475, 302494, 302500, 302504, 302547, 302573, 302580, 302583, 302584, 302585, 302587, 302588, 302589, 302592, 302644, 302649, 302728, 302739, 
302763, 302806, 302938, 302959, 302961, 302984, 302985, 302986, 302987, 302988, 302990, 303152, 303177, 303398, 303405, 303501, 303666, 304262, 304361);

update [dbo].[SAYAC]
set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONEL])
where [KOD] = 'IKYPERSONEL'";
        }

        private static string IKYPersonelNufusSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.[IKYPERSONELNUFUS];
set dateformat dmy;
insert into [InsanKaynaklari].dbo.[IKYPERSONELNUFUS]
([KOD], [CINSIYET], [CUZDANSERINO], [ILKSOYAD], [BABAAD], [ANAAD], [DOGUMYERI], [DOGUMTARIHI], [MEDENIHALI], [NUFUSDINI], [KANGRUBU],
[NUFUSILKOD], [NUFUSILCEKOD], [NUFUSMAHALLEKOY], [NUFUSCILTNO], [NUFUSAILESIRANO], [NUFUSSIRANO], [CUZDANVERILDIGIYER], [CUZDANVERILISNEDENI], 
[CUZDANKAYITNO], [CUZDANVERILISTARIHI])
SELECT nufusPersonelPk, case  when N.nufusCinsiyeti = 'Kadın' then 3 when N.nufusCinsiyeti = 'Erkek' then 2 else 0 end, 
nufusCuzdanSeri, nufusKizlikSoyadi, nufusBabaAdi, nufusAnaAdi, nufusDogumYeri, nufusDogumTarihi, 
(select kod from [InsanKaynaklari].dbo.IKYSOZLUK  where KAYITKOD like '04.%' and KAYITAD = CONVERT(varchar(20),N.nufusMedeniHali)) as nufusMedeniHali, 
nufusDini, nufusKanGrubu, C.KOD ili, C2.KOD ilcesi, nufusMahKoyu, nufusCiltNo, nufusAileSiraNo, 
nufusBireySiraNo, nufusVerildigiYer, nufusVerilisNedeni, nufusKayitNo, case when ISDATE(nufusVerilisTarihi) = 1 then convert(datetime, nufusVerilisTarihi) else null end as nufusVerilisTarihi
FROM   [CBPersonel2].dbo.nufus N
LEFT OUTER JOIN [Kullanici].[dbo].[ORGCOGRAFYA] C ON N.nufusIli = C.MERNISKOD
LEFT OUTER JOIN [Kullanici].[dbo].[ORGCOGRAFYA] C2 ON N.nufusIlcesi = C2.MERNISKOD";
        }

        private static string IKYPersonelAileSorgu()
        {
            return @"set dateformat dmy;
TRUNCATE TABLE IKYPERSONELAILE;
insert into IKYPERSONELAILE
([KOD], [PKOD], [TCKIMLIKNO], [AD], [SOYAD], [DOGUMTARIHI], [BAKMAKLAYUKUMLU], [OGRENIMTIPI], [CINSIYET], [YAKINLIKDERECESI], [MESLEK], [AGI])
select [pkAile], [ailePersonelPk], [aileTcNo], [aileAdi], [aileSoyadi], case when ISDATE(aileDogumTarihi) = 1 then convert(datetime, aileDogumTarihi) else null end as aileDogumTarihi, [aileBakmaklaYukumlu],
(select kod from [InsanKaynaklari].dbo.IKYSOZLUK  where KAYITKOD like '28.%' and ESKIID = CONVERT(varchar(20),[aileOgrenimTipi])) as OgrenimTipi,
(case 
    when aileCinsiyeti='Bilinmiyor' then 0
    when aileCinsiyeti='' then 0
    when aileCinsiyeti is null then 0
    when substring(aileCinsiyeti,1,1)='E' then 2
    else 3
end) as Cinsiyeti,
aileYakinligi  as yakinlik,
[aileEsMeslek],
case when [aileAgiOdenmesin]=1 then 0 else 1 end
from [CBPersonel2].[dbo].[aile];
update [dbo].[SAYAC]
set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELAILE])
where [KOD] = 'IKYPERSONELAILE';

UPDATE IKYPERSONELAILE SET DURUM=(SELECT KOD FROM IKYSOZLUK where KAYITKOD='05.00001') 
WHERE 1 IN (SELECT 1 FROM CBPersonel2.dbo.aile ca where  ca.pkAile =IKYPERSONELAILE.KOD and ca.aileOgrenimTipi IN (96,97));

UPDATE IKYPERSONELAILE SET DURUM=(SELECT KOD FROM IKYSOZLUK where KAYITKOD='05.00002') 
WHERE 1 IN (SELECT 1 FROM CBPersonel2.dbo.aile ca where  ca.pkAile =IKYPERSONELAILE.KOD and ca.aileOgrenimTipi IN (98));

UPDATE IKYPERSONELAILE SET DURUM=(SELECT KOD FROM IKYSOZLUK where KAYITKOD='05.00006') 
WHERE 1 IN (SELECT 1 FROM CBPersonel2.dbo.aile ca where  ca.pkAile =IKYPERSONELAILE.KOD and ca.aileEmeklimi=1);
";
        }

        private static string IKYPersonelAskerlikSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELASKERLIK;
insert into [InsanKaynaklari].[dbo].IKYPERSONELASKERLIK
( [KOD]
      ,[ASKERLIKDURUMU]
      ,[DUHULTARIHI]
      ,[TERHISTARIHI]
      ,[MUAFIYETNEDENI]
      ,[SAYILMAYANGUNSAYISI] 
      ,[TERHISBELGENO]
      ,[TERHISBELGETARIHI]
      ,[ASKERLIKSUBESI]
      ,[YEDEKSUBAYOKULUGIRISTARIHI]
      ,[ASTEGMENNASPTARIHI]
      ,[TEGMENNASPTARIHI]
      ,[SINIFDONEMSICILNO]
      ,[ERYEDEKSUBAYGOREVYERI]
      ,[ERYEDEKSUBAYGECISTARIHI]
      ,[ERYEDEKSUBAYKITAHIZMETIBASLAMATARIHI]
      ,[ERYEDEKSUBAYKITAHIZMETIBITISTARIHI]
      ,[IBRAZINTIKALTARIHI]
      ,[ACIKLAMA]
      ,[SINIFI])
SELECT  [askerlikPersonelPk]
        ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '07.%' and ESKIID = CONVERT(varchar(20), [askerlikTipiLkpDetayPk]))
		,[askerlikBaslamaTarihi]
		,[askerlikBitisTarihi]
		,[askerlikMuafNedeni]
		,[askerlikSayilmayanGun]   
		,[askerlikBelgeNo]
		,[askerlikBelgeTarihi]
		,[askerlikSubesi]  
		,[askerlikYsOkulGirisTarihi]  
		,[askerlikAstegmenNasbi]  
		,[askerlikTegmenkikNaspTarihi]
		,[askerlikSinifDonemSicilNo]
		,[askerlikGorevYeri]
		,[askerlikErliktenYedekSubayTarihi]
		,[askerlikKitaHizmetiBaslamaTarihi]
		,[askerlikKitaHizmetibitisTarihi] 
		,[akerlikIntikalTarihi]
		,[askerlikAciklama]
        ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '14.%' and KAYITAD = CONVERT(varchar(20), [askerlikSinifi]))
FROM [CBPersonel2].[dbo].[askerlik]";
        }

        private static string IKYPersonelOgrenimSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELOGRENIM;
insert into [InsanKaynaklari].dbo.IKYPERSONELOGRENIM
(      [KOD]
      ,[PKOD]
      ,[OGRENIMKOD]     
      ,[OKULADI]     
      ,[BELGETARIHI]
      ,[BELGENO]    
      ,[OGRENIMYERI]
      ,[DENKLIKTARIHI]
      ,[DENKOKUL]
      ,[DENKBOLUM]
      ,[MEMURIYETEGIRISOKULU]
      ,[OGRENIMSURESI]
      ,[BASLANGICTARIHI])
SELECT [pkOgrenim]
      ,[ogrenimPersonelPk]
	  ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '08.%' and ESKIID =  CONVERT(varchar(20), [ogrenimTipi]))
      ,[ogrenimOkulAdi]
      ,[ogrenimBelgeTarihi]
      ,[ogrenimBelgeNo]
      ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '55.%' and ESKIID =  CONVERT(varchar(20), [ogrenimYeri]))
      ,[ogrenimDenklikTarihi]
      ,[ogrenimDenklikOkulAdi]
      ,[ogrenimDenklikBolum]  
      ,[ogrenimMemuriyetGirisOkulu]
      ,[ogrenimSuresi]
      ,[ogrenimMezuniyetTarihi]
  FROM [CBPersonel2].[dbo].[ogrenim]";
        }

        private static string IKYPersonelDilSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELDIL;
insert into [InsanKaynaklari].dbo.IKYPERSONELDIL
([KOD]
      ,[PKOD]
      ,[PUAN]
      ,[TARIH]
      ,[DUZEY]
      ,[BELGETUR]    
      ,[BORDROYAESAS]
      ,[DILSEVIYESI]
      ,[DILKOD])
SELECT [pkDil]
      ,[dilPersonelPk]
      ,[dilKpdsPuan]
      ,[dilKpdsBaslamaTarihi]
	  ,[dilKpdsDuzey]
	  ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '16.%' and ESKIID = CONVERT(varchar(20), [dilSinavTipi]) )
      ,[dilKpdsAktif]
      ,[dilSeviye]
      ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '15.%' and KAYITAD =  [dilAdi] )
  FROM [CBPersonel2].[dbo].[dil];
update [dbo].[SAYAC]
set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELDIL])
where [KOD] = 'IKYPERSONELDIL'";
        }

        private static string IKYPersonelKursSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELKURS;
insert into [InsanKaynaklari].dbo.IKYPERSONELKURS
([KOD]
      ,[PKOD]     
      ,[ALDIGIYER]
      ,[BASLAMATARIHI]
      ,[BITIRMETARIHI]   
      ,[KURSADI])
SELECT [pkKurs]
      ,[kursPersonelPk]
      ,[kursAdresi]
      ,[kursBaslamaTarihi]
      ,[kursBitisTarihi]
      ,[kursAdi]
  FROM [CBPersonel2].[dbo].[kurs];
update [dbo].[SAYAC]
set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELKURS])
where [KOD] = 'IKYPERSONELKURS'";
        }

        private static string IKYPersonelKadroSorgu()
        {
            return @"delete [InsanKaynaklari].dbo.IKYPERSONELKADROAYAR;

insert into IKYPERSONELKADROAYAR(PKOD
      ,EMEKLITERFIAYI
      ,KAZANILMISTERFIAYI
      ,KIDEMTERFIAYI
      ,MAXIMUMDERECE
      ,EMEKLITERFITARIHI
      ,KAZANILMISTERFITARIHI
      ,KADRODERECEAKTIF)
SELECT [hizmetAyarPersonelPk]
      ,[hizmetAyarEmekliTerfiAyi]
      ,[hizmetAyarKazanilmisTerfiAyi]
      ,[hizmetAyarKidemTerfiAyi]
      ,[hizmetAyarMaximumDerece]
      ,[hizmetAyarEmekliTerfiTarihi]
      ,[hizmetAyarKazanilmisTerfiTarihi]
      ,[hizmetAyarKadroDereceAktif]
  FROM [CBPersonel2].[dbo].[hizmetAyar];

delete [InsanKaynaklari].dbo.IKYPERSONELKADRO;
insert into [InsanKaynaklari].dbo.IKYPERSONELKADRO
    ([KOD]
    ,[PKOD]
    ,[ORGANIZASYONKOD]
    ,[UNVANKOD]
    ,[KADRODERECE]
    ,[KADROTARIH] 
    ,[ONAYTARIH]    
    ,[ODEMEYEESASDERECE]  
    ,[ODEMEYEESASKADEME]  
    ,[KAZANILMISHAKDERECE]
    ,[KAZANILMISHAKKADEME]   
    ,[EMEKLIYEESASDERECE]
    ,[EMEKLIYEESASKADEME]
    ,[ODEMEYEESASEKGOSTERGE]
    ,[KAZANILMISEKGOSTERGE]
    ,[EMEKLIYEESASEKGOSTERGE]
    ,[KADRODURUMU]
    ,[GOREVYERI]
    ,[HIZMETSEBEBI]
    ,[AYRILMAGEREKCESI]
    ,[BELGECINSI]
    ,[BELGENO]
    ,[BELGETARIHI]
    ,[AYRILMATARIHI]
	,[gorunenUnvanKodu]
	,[EskiHizmetFlag]
	,[KurumDisi]
	,[AtamaTarihi]
    ,[PERSONELTUR]
    ,MuhasebeKodu
    ,MuhasebeBirimi
    ,GOREVORGANIZASYONKOD)
    SELECT [pkHizmet]
    ,[hizmetPersonelPk]
    ,(case when hizmetYeniBirimPk is null then hizmetEskiBirimPk else hizmetYeniBirimPk end)
    ,(case when hizmetYeniUnvanPk is null then hizmetEskiUnvanPk else hizmetYeniUnvanPk end)
    ,isnull([hizmetKadroDerece],1)
    ,[hizmetBaslamaTarihi]
    ,[hizmetKurumOnayTarihi]
    ,isnull([hizmetKadroDerece],1)
    ,[hizmetOdemeKademe]
    ,[hizmetKazanilmisDerece]
    ,[hizmetKazanilmisKademe]
    ,[hizmetEmekliDerece]
    ,[hizmetEmekliKademe]
    ,[hizmetEkGosterge]
    ,(case when [hizmetKazanilmisEkGosterge] is null or [hizmetKazanilmisEkGosterge] = 0 then hizmetEkGosterge else hizmetKazanilmisEkGosterge end)
    ,(case when [hizmetEmekliEkGosterge] is null or [hizmetEmekliEkGosterge] = 0 then hizmetEkGosterge else hizmetEmekliEkGosterge end)
	, (case when [hizmetBitisTarihi] is null then 106 else 201 end)
    ,[hizmetMemuriyeti]
    ,[hizmetSebebi]
    ,[hizmetKodSgkHizmetSebepPk]
    ,[hizmetBelgeCinsi]
    ,[hizmetBelgeNo]
    ,[hizmetBelgeTarihi]
    ,[hizmetBitisTarihi]  
	,[personelKodVekaletPk]
	,0
	,0
	,[hizmetBaslamaTarihi]
    ,p.personelKodPersonelTipiPk
    ,200
    ,'965'
    ,personelGorevYeriPk
    FROM [CBPersonel2].[dbo].[hizmet] h
    join [CBPersonel2].[dbo].personel p
    on h.hizmetPersonelPk = p.pkPersonel
    where [pkHizmet] not  in
	(select [pkHizmet] from [CBPersonel2].[dbo].[hizmet] where [hizmetBitisTarihi] is null and [hizmetBaslamaTarihi] is null);

INSERT INTO IKYPERSONELKADRO (KOD,PKOD,ORGANIZASYONKOD, UNVANKOD, KADRODERECE, KADRODURUMU, PERSONELTUR, AtamaTarihi, EskiHizmetFlag, MuhasebeKodu, MuhasebeBirimi)
    select 400000+ pkPersonel AS kod, pkPersonel, personelKodCbBirimPk,personelkodUnvanPk, 1 AS KADRODERECE, 106 KADRODURUMU, personelKodPersonelTipiPk,personelBaslamaTarihi, 0,200, '965' from CBPersonel2.dbo.personel p
    where 1 not in (select 1 from CBPersonel2.dbo.hizmet h where h.hizmetPersonelPk = p.pkPersonel) and personelAktif=1; 

update [InsanKaynaklari].dbo.IKYPERSONELKADRO 
    set ORGANIZASYONKOD = (select personelKodCbBirimPk from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod);
    --where ORGANIZASYONKOD is null or ORGANIZASYONKOD=0;

update IKYPERSONELKADRO 
    set UNVANKOD = (select personelkodUnvanPk from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod)
    where UNVANKOD=0 or UNVANKOD IS NULL;

update IKYPERSONELKADRO 
    set GorunenUnvanKodu = (select personelKodVekaletPk from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod)
    where GorunenUnvanKodu is null or GorunenUnvanKodu=0;

update [InsanKaynaklari].dbo.IKYPERSONELKADRO
    set [HizmetGunSayisi] = dbo.gunDondur([AtamaTarihi], AYRILMATARIHI, 1, null, 0)
where DatePart(yy, AYRILMATARIHI)<2020 and DatePart(yy, AYRILMATARIHI)>1800;

update IKYPERSONELKADRO
    set AYRILMATARIHI = 
	    (
		    SELECT MIN(CASE WHEN AtamaTarihi IS NULL THEN AYRILMATARIHI  ELSE AtamaTarihi END) 
		    FROM IKYPERSONELKADRO IC WHERE IC.PKOD=IKYPERSONELKADRO.PKOD AND 
		    (IC.AtamaTarihi > IKYPERSONELKADRO.AtamaTarihi OR IC.AYRILMATARIHI > IKYPERSONELKADRO.AtamaTarihi) 
	    )
    where AYRILMATARIHI IS NULL;

update IKYPERSONELKADRO
    set KADRODURUMU = 
    case when AYRILMATARIHI IS NULL then 106
    else 201 end;

UPDATE IKYPERSONELKADRO 
    SET KADRODURUMU= 201 
    WHERE PKOD IN (SELECT  PKOD
    FROM IKYPERSONELKADRO IC
    WHERE KADRODURUMU>100 AND KADRODURUMU<200 AND IC.PKOD=IKYPERSONELKADRO.PKOD
    group by PKOD
    having COUNT(*)>1)
    AND KADRODURUMU>100 AND KADRODURUMU<200
    AND KOD <> (SELECT SONKOD FROM IKYPERSONELKADROSONKOD WHERE IKYPERSONELKADROSONKOD.PKOD = IKYPERSONELKADRO.PKOD);

UPDATE IKYPERSONELKADRO 
    SET KADRODURUMU=126
    WHERE KOD IN (SELECT SONKOD
    FROM IKYPERSONEL ICP INNER JOIN IKYPERSONELKADROSONKOD ON IKYPERSONELKADROSONKOD.PKOD=ICP.KOD 
    LEFT OUTER JOIN IKYPERSONELKADRO PK ON ICP.KOD=PK.PKOD AND PK.KADRODURUMU>100 AND PK.KADRODURUMU<200 
    WHERE PK.KADRODURUMU IS NULL AND ICP.KOD = IKYPERSONELKADRO.PKOD)

update [InsanKaynaklari].dbo.IKYPERSONELKADRO 
    set PERSONELTUR = NULL
    where KADRODURUMU <> 106 and KADRODURUMU <> 126;

UPDATE IKYPERSONELKADRO 
    SET PERSONELTUR = 100
    WHERE AtamaTarihi >= (SELECT personelkurumbaslamatarihi FROM CBPersonel2.dbo.personel p where p.pkpersonel = IKYPERSONELKADRO.pkod);

UPDATE IKYPERSONELKADRO 
    SET PERSONELTUR = (SELECT personelKodPersonelTipiPk FROM CBPersonel2.dbo.personel p where p.pkpersonel = IKYPERSONELKADRO.pkod)
    WHERE AtamaTarihi >= (SELECT PERSONELBASLAMATARihi FROM CBPersonel2.dbo.personel p where p.pkpersonel = IKYPERSONELKADRO.pkod);

UPDATE IKYPERSONELKADRO 
    SET PERSONELTUR = 10001
    WHERE (PERSONELTUR = 0 OR PERSONELTUR IS NULL);
-- or (KADRODURUMU = 106 or KADRODURUMU = 116 or KADRODURUMU = 126 or KADRODURUMU = 136)

UPDATE IKYPERSONELKADRO 
    SET KADRODURUMU = (case when KADRODURUMU=106 then 207 else 208 end)
    WHERE AtamaTarihi < (SELECT personelkurumbaslamatarihi FROM CBPersonel2.dbo.personel p where p.pkpersonel = IKYPERSONELKADRO.pkod);

INSERT INTO IKYPERSONELKADRO (KOD,PKOD,KADRODURUMU, PERSONELTUR, AtamaTarihi, AYRILMATARIHI, HizmetGunSayisi, EskiHizmetFlag)
    select  800000+pkEskiHizmet, eskiHizmetPersonelPk, 201, 10000 + eskiHizmetTipi,  
    DateAdd(d, -1*isnull(eskiHizmetGun,0), DateAdd(m, -1*isnull(eskiHizmetAy, 0),  DateAdd(yy, -1*isnull(eskiHizmetYil,0), '1900-01-01'))), 
    '1900-01-01', 
    isnull(eskiHizmetYil,0)*360+isnull(eskiHizmetAy,0)*30+isnull(eskiHizmetGun,0),1  from CBPersonel2.dbo.eskiHizmet h INNER JOIN CBPersonel2.dbo.personel p ON h.eskiHizmetPersonelPk=p.pkPersonel
    where eskiHizmetTipi<=11 and personelBaslamaTarihi is not null
    and isnull(eskiHizmetYil,0)<50;

update IKYPERSONELKADRO 
    set 
        UNVANKOD = (select personelkodUnvanPk from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod),
        GorunenUnvanKodu = (select personelKodVekaletPk from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod)
    where KADRODURUMU IN (106,116,126,136,207);

--pasif kişilerinin kadro durumlarını aktif kapandı yap
update IKYPERSONELKADRO 
    set KADRODURUMU = (CASE when KADRODURUMU=106 then 126 else 207 end)
where KADRODURUMU IN(106,207) and 1 IN (select 1 from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod and (p.personelAktif is null or p.personelAktif <> 1));

--Ayrılma tarihi hatalı olan kayıtların durumunu iptal yap.
update IKYPERSONELKADRO set KADRODURUMU = 209 where AYRILMATARIHI > '2018-01-01';

--aktifse ve son kayıtta AYRILMATARIHI dolu ise, bu kişileri ücretsiz izin aktif yap
update IKYPERSONELKADRO set KADRODURUMU = 136
where KOD = (SELECT SONKOD FROM IKYPERSONELKADROSONKOD WHERE IKYPERSONELKADROSONKOD.PKOD = IKYPERSONELKADRO.PKOD)
and 1 IN (select 1 from CBPersonel2.dbo.personel p where p.pkPersonel = IKYPERSONELKADRO.pkod and p.personelAktif = 1)
AND AYRILMATARIHI IS NOT NULL;

--Ödemeye esas kademesi olmayanlara değer ata
update IKYPERSONELKADRO set ODEMEYEESASKADEME = case when ODEMEYEESASDERECE<KAZANILMISHAKDERECE then 1 else KAZANILMISHAKKADEME end
where ODEMEYEESASKADEME = 0 or ODEMEYEESASKADEME is null;

update SAYAC
    set  SAYAC = (select right('00000000'+convert(varchar(8), max(KOD)), 8)  from IKYPERSONELKADRO)
    where KOD = 'IKYPERSONELKADRO';
";
        }

        private static string IKYPersonelKadroKontenjanSorgu()
        {
            return @"truncate table IKYKADROKONTENJANDETAY;
truncate table IKYKADROKONTENJAN;
INSERT INTO IKYKADROKONTENJAN (KOD, ORGANIZASYONKOD, ISLEMNO, ISLEMTUR, ISLEMTARIH, ACIKLAMA, DAYANAKBELGETARIH, DAYANAKBELGENO, PERSONELTUR)
    VALUES (1, 303, 1, 1, '2016-01-01', 'Açılış', '2016-01-01', '', 1);

INSERT INTO IKYKADROKONTENJANDETAY (KOD, KKOD, UNVANKOD, KADRODERECE, KONTENJAN)
select 1 KOD, 1 KKOD, UNVANKOD, KADRODERECE, SUM(KONTENJAN) KONTENJAN FROM
(select  UNVANKOD, KADRODERECE, COUNT(*) KONTENJAN 
from IKYPERSONELKADRO INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD=IKYPERSONELKADRO.PERSONELTUR  
where KADRODURUMU IN (106,136) and IKYPERSONELTIPI.KADROLU=1
group by UNVANKOD, KADRODERECE
UNION ALL
select  bosKadroKodUnvanPk UNVANKOD, bosKadroDerece KADRODERECE, count(*) KONTENJAN 
from CBPersonel2.dbo.bosKadro
group by bosKadroKodUnvanPk, bosKadroDerece) TABLO
group by UNVANKOD, KADRODERECE;
";
        }

        private static string IKYPersonelIletisimSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELILETISIM;
insert into [InsanKaynaklari].dbo.IKYPERSONELILETISIM
([KOD]
      ,[ISTELEFON]
      ,[CEPTELEFON]
      ,[FAKS]
      ,[LOJMANTEL]
      ,[TELSIZ]
      ,[GORUNTULU]
      ,[TARABYATEL]
      ,[REHBERDEGOZUKSUN]
      ,[CEPTELEFON2]
      ,[EPOSTA])
SELECT [telefonPersonelPk], [telefonHariciTel] + ' - ' + [telefonDahiliTel] AS isTel,
[telefonCepTel1], [telefonFax], [telefonLojmanTel], [telefonTelsiz], [telefonGoruntulu],
[telefonTHarici] + ' - ' + [telefonTDahili]  AS tarabyaTel, [telefonRehberdeGozuksun], [telefonCepTel2],
P.personelEmail
FROM [CBPersonel2].[dbo].[telefon] T
JOIN [CBPersonel2].[dbo].[personel] P ON T.[telefonPersonelPk]= P.pkPersonel";
        }

        private static string IKYPersonelCezaOdulSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELCEZAODUL;
insert into [InsanKaynaklari].dbo.IKYPERSONELCEZAODUL
([KOD]
      ,[PKOD]
      ,[VerenKurum]
      ,[VerenAdi]
      ,[VerenUnvani]
      ,[AlanUnvani]
      ,[Cinsi]
      ,[Sebebi]
      ,[BelgeCinsi]
      ,[BelgeNo]
      ,[BelgeTarihi]
      ,[CezaOdulTipi]
      ,Tarih)
SELECT [pkTakdirCeza]
      ,[takdirCezaPersonelPk]
      ,[takdirCezaVerenKurum]
      ,[takdirCezaVerenAdi]
      ,[takdirCezaVerenUnvani]
      ,[takdirCezaAlanUnvani]
      ,[takdirCezaCinsi]
      ,[takdirCezaSebebi]
      ,[takdirCezaBelgeCinsi]
      ,[takdirCezaBelgeNo]
      ,[takdirCezaBelgeTarihi]
	  ,(select KOD from InsanKaynaklari.[dbo].[IKYSOZLUK] where KAYITKOD LIKE '30.%' and HITAPID = CONVERT(varchar(20),[takdirCezaTipi]))
      ,[takdirCezaBelgeTarihi]
  FROM [CBPersonel2].[dbo].[takdirCeza];
update [dbo].[SAYAC]
set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELCEZAODUL])
where [KOD] = 'IKYPERSONELCEZAODUL'";
        }

        private static string IKYPersonelKartSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELKART;
insert into [InsanKaynaklari].dbo.IKYPERSONELKART
([KOD]
      ,[PKOD]
      ,[ZimmetTarihi]
      ,[IadeTarihi]
      ,[kartTipi]
      ,[kartNo]
      ,[Yakinligi]
      ,[Plaka]
      ,[AdiSoyadi]
      ,[TagCihazNo]
      ,[ZimmetEdenPersonel]
      ,[IadeAlanPersonel]
      ,[Aciklama]
      ,[AktifMi]
      ,[KayipTarihi])
SELECT [pkKart]
      ,[kartPersonelPk]
      ,[kartZimmetTarihi]
      ,[kartIadeTarihi]
	  ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '31.%' and KAYITAD = [kartTipi])
      ,[kartNo]
      ,(select ESKIID from InsanKaynaklari.[dbo].[IKYSOZLUK] where KAYITKOD LIKE '06.%' and KAYITAD = [kartYakinligi])
      ,[kartPlaka]
      ,[kartAdiSoyadi]
      ,[kartTagCihazNo]
      ,[kartZimmetEdenPersonelPk]
      ,[kartIadeAlanPersonelPlk]
      ,[kartAciklama]
      ,[kartIsActive]
      ,[kartKayipTarihi]
  FROM [CBPersonel2].[dbo].[kart];

update [dbo].[SAYAC] set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELKART])
where [KOD] = 'IKYPERSONELKART'";
        }

        private static string IKYHizmetSebebiSorgu()
        {
            return @"truncate table [InsanKaynaklari].dbo.IKYSOZLUKHIZMETSEBEBI;
INSERT INTO [InsanKaynaklari].[dbo].[IKYSOZLUKHIZMETSEBEBI]
           ([KOD]
           ,[SEBEPADI]
           ,[TIPADI]
           ,[TIP])
SELECT [pkKodHizmetSebep]
      ,[kodHizmetSebepAdi]
      ,[kodHizmetSebepTipAdi]
      ,[kodHizmetSebepTipi]
  FROM [CBPersonel2].[dbo].[kodSgkHizmetSebep]";
        }

        private static string IKYSozlukEskiHizmetSorgu()
        {
            return @"truncate table [InsanKaynaklari].dbo.IKYSOZLUKESKIHIZMET;
INSERT INTO [InsanKaynaklari].[dbo].[IKYSOZLUKESKIHIZMET]
           ([KOD]
           ,[HizmetAdi]
           ,[Toplam]
           ,[Aylik]
           ,[Emekli]
           ,[Izin]
           ,[Cb])
SELECT  [pkKodEskiHizmet]
        ,[kodEskiHizmetAdi]
        ,[kodEskiHizmetToplam]
        ,[kodEskiHizmetAylik]
        ,[kodEskiHizmetEmekli]
        ,[kodEskiHizmetIzin]
        ,[kodEskiHizmetCb]
  FROM [CBPersonel2].[dbo].[kodEskiHizmet]";
        }

        //        private static string IKYEskiHizmetSorgu()
        //        {
        //            return @"DELETE [InsanKaynaklari].dbo.IKYPERSONELKADRO WHERE EskiHizmetFlag = 1;
        //insert into [InsanKaynaklari].dbo.IKYPERSONELKADRO
        //([KOD]
        //      ,[PKOD]
        //      ,[EskiHizmetTipi]
        //	  ,[HizmetGunSayisi]
        //	  ,[EskiHizmetFlag]
        //	  ,[KurumDisi]
        //      ,[KADRODURUMU])
        //SELECT CONCAT('-',[pkEskiHizmet])
        //      ,[eskiHizmetPersonelPk]
        //      ,[eskiHizmetTipi]
        //	  ,(([eskiHizmetYil]*360) + ([eskiHizmetAy]*30) + [eskiHizmetGun]) hizmetGunSayisi
        //	  ,1
        //	  ,(case when [eskiHizmetTipi] != 2 then 1 else 0 end) kurumDisi
        //      ,106
        //  FROM [CBPersonel2].[dbo].[eskiHizmet];
        // ";
        //        }

        private static string IKYPersonelResimSorgu()
        {
            return @"TRUNCATE TABLE [InsanKaynaklari].dbo.IKYPERSONELRESIM;
INSERT INTO [InsanKaynaklari].[dbo].[IKYPERSONELRESIM]
           ([KOD]
           ,[RESIM])
SELECT [resimPersonelPk]
      ,[resimFotograf]
  FROM [CBPersonel2].[dbo].[resim]";
        }

        private static string IKYPersonelIzinSorgu()
        {
            return @"truncate table [InsanKaynaklari].dbo.IKYPERSONELIZIN;
                    INSERT INTO [dbo].[IKYPERSONELIZIN]
                    ([KOD]
                    ,[PKOD]
                    ,[IZINTUR]
                    ,[ACIKLAMA]
                    ,[BASLANGICTARIHI]
                    ,[BITISTARIHI]           
                    ,[BelgeNo]
                    ,[BelgeTarihi]
                    ,[IZINNOTU]
	                ,[TELEFON]
                    ,[ImzalayanPersonel]
                    ,[KayitTarihi]
                    ,[Memuriyeti]
                    ,[TeklifEdenPersonel]
                    ,[YerineGorevliPersonel]
                    ,[SaglikKurulusu]
                    ,[KULLANILANGUN])
                    SELECT [pkIzin]
                        ,[izinPersonelPk]
                        ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUKIZINTUR] where ESKIID = [izinTipi])
                        ,[izinSebebi]
                        ,[izinBaslamaTarihi]
                        ,[izinBitisTarihi]
                        ,[izinBelgeNo]
                        ,[izinBelgeTarihi]
                        ,[izinAdresi]
                        ,[izinTelefon]
                        ,[izinImzaPersonelPk]
                        ,[izinKayitTarihi]
                        ,[izinMemuriyeti]    
                        ,[izinImzaTeklifEdenPk]
                        ,[izinYerineGorevliPersonelPk]
                        ,[izinSaglikKurulusu]
                        ,DATEDIFF(day,[izinBaslamaTarihi],[izinBitisTarihi])
                    FROM [CBPersonel2].[dbo].[izin];
                    update [dbo].[SAYAC]
                    set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELIZIN])
                    where [KOD] = 'IKYPERSONELIZIN'";
        }

        private static string YemekAktarSql()
        {
            return @"truncate table [InsanKaynaklari].dbo.[yemekHareket];

                    INSERT INTO [dbo].[yemekHareket]
                                ([pkYemekHareket]
                                ,[yemekHareketPersonelPk]
                                ,[yemekHareketTarih]
                                ,[yemekHareketYerinePersonelPk]
                                ,[yemekHareketKodYemekhanePk]
                                ,[yemekHareketManuel]
                                ,[yemekHareketUcretsiz]
                                ,[yemekHareketUserPk]
                                ,[yemekHareketCreateDate]
                                ,[yemekHareketIsActive]
                                ,[yemekHareketCreatedByUserPk]
                                ,[yemekHareketLastEditByUserPk]
                                ,[yemekHareketDeletedByUserPk]
                                ,[yemekHareketMailGonderildi]
                                ,[yemekHareketProgramatikAktarim])
                        SELECT [pkYemekHareket]
                            ,[yemekHareketPersonelPk]
                            ,[yemekHareketTarih]
                            ,[yemekHareketYerinePersonelPk]
                            ,[yemekHareketKodYemekhanePk]
                            ,[yemekHareketManuel]
                            ,[yemekHareketUcretsiz]
                            ,[yemekHareketUserPk]
                            ,[yemekHareketCreateDate]
                            ,[yemekHareketIsActive]
                            ,[yemekHareketCreatedByUserPk]
                            ,[yemekHareketLastEditByUserPk]
                            ,[yemekHareketDeletedByUserPk]
                            ,[yemekHareketMailGonderildi]
                            ,[yemekHareketProgramatikAktarim]
                        FROM [CBPersonel2].[dbo].[yemekHareket];

                        update [dbo].[SAYAC]
                        set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(pkYemekHareket)), 9)  from [dbo].[yemekHareket])
                        where [KOD] = 'yemekHareket'";
        }

        private static string IKYPersonelIzinDevirSorgu()
        {
            return @"truncate table [InsanKaynaklari].dbo.[IKYPERSONELIZINDEVIR];
                     INSERT INTO [InsanKaynaklari].[dbo].[IKYPERSONELIZINDEVIR]
                    ([KOD]
                    ,[PKOD]
                    ,[YIL]
                    ,[DEVREDEN]
                    ,[YILICI])
                SELECT [pkIzinDetay]
                ,[izinDetayPersonelPk]
                ,[izinDetayYil]+1
                ,[izinDetayKalan]
                ,[izinDetayKullanilan]
                FROM [CBPersonel2].[dbo].[izinDetay];
                update [dbo].[SAYAC]
                set  [SAYAC] = (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELIZINDEVIR])
                where [KOD] = 'IKYPERSONELIZINDEVIR'";
        }

        private static string LojmanSiteAktar()
        {
            return @"delete [InsanKaynaklari].[dbo].IKYPERSONELSITEKESINTISI;
delete [InsanKaynaklari].[dbo].IKYPERSONELSITEKESINTITOPLAM;
delete [InsanKaynaklari].[dbo].[IKYPERSONELKESINTI];
delete [InsanKaynaklari].[dbo].[IKYPERSONELLOJMAN];
delete [InsanKaynaklari].[dbo].[IKYSITEGIDER];
delete [InsanKaynaklari].dbo.IKYLOJMANDAIRE;
                    delete [InsanKaynaklari].dbo.IKYLOJMANBLOK;
                    delete [InsanKaynaklari].dbo.IKYLOJMANSITE;
                    INSERT INTO [InsanKaynaklari].[dbo].[IKYLOJMANSITE]
                               ([KOD]
                               ,[SITEADI]
                               ,[MAHALLE]
                               ,[CADDE]
                               ,[SOKAK]
                               ,[SEMT]
                               ,[ILCE]
                               ,[IL]
                               ,[YAKITKESILECEK]
                               ,[KESINTITIPI]
                               ,[KESINTICEVREGIDERI]
                               ,[KESINTISUGIDERI]
                               ,[KESINTIDIGER]
                               ,[KESINTIDIGERACIKLAMA]
                               ,[KESINTIORTAKGIDER])
                    SELECT [pkKodLojman]
                          ,[kodLojmanSiteAdi]
                          ,[kodLojmanMahalle]
                          ,[kodLojmanCadde]
                          ,[kodLojmanSokak]
                          ,[kodLojmanSemt]
                          ,[kodLojmanIlce]
                          ,[kodLojmanIl]
                          ,[kodLojmanYakitKesilecek]
                          ,[kodLojmanKesintiTipi]
                          ,[kodLojmanKesintiCevreGideri]
                          ,[kodLojmanKesintiSuGideri]
                          ,[kodLojmanKesintiDiger]
                          ,[kodLojmanKesintiDigerAciklama]
                          ,[kodLojmanKesintiOrtakGider]
                      FROM [CBPersonel2].[dbo].[kodLojman];

                    delete [dbo].[SAYAC]
                    where [KOD] = 'IKYLOJMANSITE';

                      INSERT INTO [dbo].[SAYAC]
                                ([KOD]
                                ,[SAYAC])
                            VALUES
                                ('IKYLOJMANSITE',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYLOJMANSITE])
                                )";

        }

        private static string LojmanSiteBlokAktar()
        {
            return @"delete [InsanKaynaklari].dbo.IKYLOJMANDAIRE;
                    delete [InsanKaynaklari].dbo.IKYLOJMANBLOK;
                    INSERT INTO [InsanKaynaklari].[dbo].[IKYLOJMANBLOK]
                        ([KOD]
                        ,[SKOD]
                        ,[BlokAdi]
                        ,[BlokNo]
                        ,[BlokDaireSayisi]
                        ,[BlokSiteGideriKesilsinmi])
                    SELECT [pkKodLojmanBlok]
                        ,[kodBlokKodLojmanPk]
                        ,[kodBlokAdi]
                        ,[kodBlokNo]
                        ,[kodBlokDaireSayisi]
                        ,[kodBlokSiteGideriKesilsinmi]
                    FROM [CBPersonel2].[dbo].[kodLojmanBlok];

                    delete [dbo].[SAYAC]
                    where [KOD] = 'IKYLOJMANBLOK';

                    INSERT INTO [dbo].[SAYAC]
                                ([KOD]
                                ,[SAYAC])
                            VALUES
                                ('IKYLOJMANBLOK',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYLOJMANBLOK])
                                )";
        }

        private static string LojmanSiteBlokDaireAktar()
        {
            return @"delete [InsanKaynaklari].dbo.IKYLOJMANDAIRE;
                    INSERT INTO [InsanKaynaklari].[dbo].[IKYLOJMANDAIRE]
                            ([KOD]
                            ,[BKOD]
                            ,[DaireNo]
                            ,[DaireTipi]
                            ,[DaireMetreKare]
                            ,[DaireElektrikNo]
                            ,[DaireSuNo]
                            ,[DaireDogalGazNo]
                            ,[DaireCevreTemizlik]
                            ,[DaireTelefon]
                            ,[DaireTuru]
                            ,[DaireKatNo]
                            ,[DaireAciklama])
                    SELECT [pkKodLojmanDaire]
                            ,[kodDaireKodBlokPk]
                            ,[kodDaireNo]
                            ,[kodDaireTipi]
                            ,[kodDaireMetreKare]
                            ,[kodDaireElektrikNo]
                            ,[kodDaireSuNo]
                            ,[kodDaireDogalGazNo]
                            ,[kodDaireCevreTemizlik]
                            ,[kodDaireTelefon]
                            ,(select KOD from [InsanKaynaklari].[dbo].[IKYSOZLUK] where KAYITKOD LIKE '33.%' and ESKIID = CONVERT(varchar(20), [kodDaireTuru]) )
                            ,[kodLojmanDaireKatNo]
                            ,[kodLojmanDaireAciklama]
                    FROM [CBPersonel2].[dbo].[kodLojmanDaire];

                    delete [dbo].[SAYAC]
                    where [KOD] = 'IKYLOJMANDAIRE';

                    INSERT INTO [dbo].[SAYAC]
                                ([KOD]
                                ,[SAYAC])
                            VALUES
                                ('IKYLOJMANDAIRE',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYLOJMANDAIRE])
                                )";
        }

        private static string LojmanPersonelAktar()
        {
            return @"delete [InsanKaynaklari].dbo.IKYPERSONELLOJMAN;

                    INSERT INTO [dbo].[IKYPERSONELLOJMAN]
                               ([KOD]
                               ,[PKOD]
                               ,[DKOD]
                               ,[LojmanTipi]
                               ,[BasTarihi]
                               ,[BitTarihi]
                               ,[Adres]
                               ,[KesintiVar]
                               ,[TazminatiVar]
                               ,[SuParasiKesiliyor]
                               ,[KonutYardimTarihi]
                               ,[OzelTarih]
                               ,[AdresTipi]
                               ,[IsTahakkuk]
                               ,[TahsisTuru]
                               ,[TahsisTip])

                    SELECT [pkAdres]
                          ,[adresPersonelPk]
                          ,[adresKodLojmanDairePk]    
                          ,(select kod from IKYSOZLUKLOJMANTIPI tab where tab.ETIKET = adr.adresLojmanTahsisiTuru)  
                          ,[adresLojmanBasTarihi]
                          ,[adresLojmanBitTarihi]
                          ,[adresAdresi]
                          ,[adresLojmanKesintiVar]
                          ,[adresLojmanTazminatiVar]
                          ,[adresSuParasiKesiliyor]
                          ,[adresKonutYardimTarihi]
                          ,[adresOzelTarih]
                          ,[adresAdresTipi]
                          ,[adresIsTahakkuk]
                          ,[adresLojmanTahsisiTuru]   
                          ," + (int)ENUMTahsisTipi.Tahsis +
                      @" FROM CBPersonel2.dbo.adres adr;

                    delete [dbo].[SAYAC]
                    where [KOD] = 'IKYPERSONELLOJMAN';

                    INSERT INTO [dbo].[SAYAC]
                                ([KOD]
                                ,[SAYAC])
                            VALUES
                                ('IKYPERSONELLOJMAN',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from [dbo].[IKYPERSONELLOJMAN])
                                )";
        }

        public static Sonuc SiteGiderTablosuAktar(string kullaniciKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", kullaniciKodu);
            string siteGiderleriSql = @"SELECT 
	                                           pkLojmanGider
	                                          ,lojmanGiderSitePk
                                              ,lojmanGiderYil
                                              ,lojmanGiderAy
                                              ,lojmanGiderPersonelGideri
                                              ,lojmanGiderElektrikGideri
                                              ,lojmanGiderGazGideri
                                              ,lojmanGiderCevreGideri
                                              ,lojmanGiderDigerGiderler      
                                              ,lojmanGiderSuFiyati
                                              ,lojmanGiderAsansorBakim
                                              ,lojmanGiderAciklama
                                              ,lojmanGiderElektrikOtomatik
                                              ,lojmanGiderSuOtomatik
                                              ,lojmanGiderGazOtomatik
                                        FROM CBPersonel2.lojman.lojmanGider order by pkLojmanGider";

            IDataReader r = ve.DataReaderDondur(siteGiderleriSql);
            ObjectArray oa = new ObjectArray();
            if (r == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa.sonuc;
            }

            while (r.Read())
            {
                SiteGider sg = new SiteGider();
                sg.eskiKod = (int)r.GetInt64(0);
                sg.siteKod = VeriErisim.DegerAl(r, 1, 0);
                sg.yil = VeriErisim.DegerAl(r, 2, 0);
                sg.ay = VeriErisim.DegerAl(r, 3, 0);
                sg.giderler = new SozlukGider[7];
                sg.giderler[0] = new SozlukGider();
                sg.giderler[0].kod = 1;
                sg.giderler[0].miktar = r.GetDouble(4);
                sg.giderler[1] = new SozlukGider();
                sg.giderler[1].kod = 2;
                sg.giderler[1].miktar = r.GetDouble(5);
                sg.giderler[1].otomatik = r.IsDBNull(12) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(12), 0);
                sg.giderler[2] = new SozlukGider();
                sg.giderler[2].kod = 3;
                sg.giderler[2].miktar = r.GetDouble(6);
                sg.giderler[2].otomatik = r.IsDBNull(14) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(14), 0);
                sg.giderler[3] = new SozlukGider();
                sg.giderler[3].kod = 4;
                sg.giderler[3].miktar = r.GetDouble(7);
                sg.giderler[4] = new SozlukGider();
                sg.giderler[4].kod = 5;
                sg.giderler[4].miktar = r.GetDouble(9);
                sg.giderler[4].otomatik = r.IsDBNull(13) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(13), 0);
                sg.giderler[5] = new SozlukGider();
                sg.giderler[5].kod = 6;
                sg.giderler[5].miktar = r.GetDouble(8);
                sg.giderler[6] = new SozlukGider();
                sg.giderler[6].kod = 7;
                sg.giderler[6].miktar = r.GetDouble(10);
                sg.aciklama = VeriErisim.DegerAl(r, 11, "");
                oa.Ekle(sg);
            }

            r.Dispose();
            int sayac = 1;
            string insertSql = "truncate table IKYSITEGIDER; ";
            Sonuc sonuc = new Sonuc();

            foreach (SiteGider s in oa.objeler)
            {
                for (int i = 0; i < s.giderler.Length; i++)
                {
                    if (s.giderler[i].miktar > 0)
                    {
                        insertSql += @"INSERT INTO IKYSITEGIDER
                                           (KOD
                                           ,SITEKOD
                                           ,YIL
                                           ,AY
                                           ,GIDERKOD
                                           ,OTOMATIK
                                           ,MIKTAR
                                           ,ACIKLAMA
                                           ,ESKIKOD)
                                     VALUES
                                           (" + sayac
                                           + ", " + s.siteKod
                                           + ", " + s.yil
                                           + ", " + s.ay
                                           + ", " + s.giderler[i].kod
                                           + ", " + s.giderler[i].otomatik
                                           + ", '" + s.giderler[i].miktar + "'"
                                           + ", '" + s.aciklama + "'"
                                           + ", " + s.eskiKod + ") ; ";
                    }
                    sayac++;
                }
            }

            insertSql += @"delete SAYAC
                           where KOD = 'IKYSITEGIDER';

                           INSERT INTO SAYAC
                                (KOD
                                ,SAYAC)
                           VALUES
                                ('IKYSITEGIDER',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from IKYSITEGIDER)
                                )";

            ve.BeginTransaction();

            int ss = ve.ExecuteNonquery(insertSql);

            if (ss > 0)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                sonuc.islemSonuc = true;
            }
            else
            {
                ve.Rollback();
                sonuc.DegerAta(ve.hata);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static string PersonelSiteKesintisiSorgu()
        {
            return null;
            //            string sql = @"truncate table IKYPERSONELSITEKESINTISI; 
            //
            //DELETE IKYPERSONELSITEKESINTITOPLAM;
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTITOPLAM]
            //           ([KOD]
            //           ,[PLOJMANKOD]
            //           ,[YIL]
            //           ,[AY]
            //           ,[TOPLAMTUTAR])
            //SELECT   [pkLojmanAylikKesinti]
            //,[lojmanAylikKesintiAdresPk]
            //,[lojmanAylikKesintiYil]
            //,[lojmanAylikKesintiAy]
            //,[lojmanAylikKesintiToplamGider]
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 1000000
            //,[pkLojmanAylikKesinti]
            //,1
            //,[lojmanAylikKesintiPersonelGiderAktif]
            //,0
            //,[lojmanAylikKesintiPersonelGider]
            //,0
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5010
            //,[pkLojmanAylikKesinti]
            //,2
            //,[lojmanAylikKesintiElektrikGiderAktif]
            //,[lojmanAylikKesintiElektrikEndex]
            //,[lojmanAylikKesintiElektrikGider]
            //,[lojmanAylikKesintiElektrikGiderElden]
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5020
            //,[pkLojmanAylikKesinti]
            //,3
            //,[lojmanAylikKesintiGazGiderAktif]
            //,0
            //,[lojmanAylikKesintiGazGider]
            //,[lojmanAylikKesintiGazGiderElden]
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5030
            //,[pkLojmanAylikKesinti]
            //,4
            //,[lojmanAylikKesintiCevreGiderAktif]
            //,0
            //,[lojmanAylikKesintiCevreGider]
            //,0
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5040
            //,[pkLojmanAylikKesinti]
            //,5
            //,[lojmanAylikKesintiSuGiderAktif]
            //,[lojmanAylikKesintiSuEndex]
            //,[lojmanAylikKesintiSuGider]
            //,[lojmanAylikKesintiSuGiderElden]
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5050
            //,[pkLojmanAylikKesinti]
            //,6
            //,[lojmanAylikKesintiDigerGiderAktif]
            //,0
            //,[lojmanAylikKesintiDigerGider]
            //,[lojmanAylikKesintiDigerGiderElden]
            //,[lojmanAylikKesintiDigerGiderAciklama]
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
            //           (KOD
            //           ,[SKTKOD]
            //           ,[GIDERKOD]
            //           ,[AKTIF]
            //           ,[ENDEX]
            //           ,[TUTAR]
            //           ,[ELDEN]
            //           ,[ACIKLAMA])
            //SELECT  pkLojmanAylikKesinti + 5060
            //,[pkLojmanAylikKesinti]
            //,7
            //,[lojmanAylikKesintiAsansorGiderAktif]
            //,0
            //,[lojmanAylikKesintiAsansorGider]
            //,0
            //,''
            //FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]
            //
            //delete SAYAC
            //                           where KOD = 'IKYPERSONELSITEKESINTISI';
            //
            //                           INSERT INTO SAYAC
            //                                (KOD
            //                                ,SAYAC)
            //                           VALUES
            //                                ('IKYPERSONELSITEKESINTISI',
            //		                        (select right('000000000'+convert(varchar(9), 
            //
            //max(KOD)), 9)  from IKYPERSONELSITEKESINTISI)
            //                                );
            //
            //delete SAYAC
            //                           where KOD = 'IKYPERSONELSITEKESINTITOPLAM';
            //
            //                           INSERT INTO SAYAC
            //                                (KOD
            //                                ,SAYAC)
            //                           VALUES
            //                                ('IKYPERSONELSITEKESINTITOPLAM',
            //		                        (select right('000000000'+convert(varchar(9), 
            //
            //max(KOD)), 9)  from IKYPERSONELSITEKESINTITOPLAM)
            //                                );";

            string sql = @"    truncate table IKYPERSONELKESINTI; 

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 1
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiPersonelGiderAktif]
                        ,0
                        ,[lojmanAylikKesintiPersonelGider]
                        ,0
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 2
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiElektrikGiderAktif]
                        ,[lojmanAylikKesintiElektrikEndex]
                        ,[lojmanAylikKesintiElektrikGider]
                        ,[lojmanAylikKesintiElektrikGiderElden]
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 3
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiGazGiderAktif]
                        ,0
                        ,[lojmanAylikKesintiGazGider]
                        ,[lojmanAylikKesintiGazGiderElden]
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 4
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiCevreGiderAktif]
                        ,0
                        ,[lojmanAylikKesintiCevreGider]
                        ,0
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 5
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiSuGiderAktif]
                        ,[lojmanAylikKesintiSuEndex]
                        ,[lojmanAylikKesintiSuGider]
                        ,[lojmanAylikKesintiSuGiderElden]
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 6
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiDigerGiderAktif]
                        ,0
                        ,[lojmanAylikKesintiDigerGider]
                        ,[lojmanAylikKesintiDigerGiderElden]
                        ,[lojmanAylikKesintiDigerGiderAciklama]
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]

                        INSERT INTO [dbo].[IKYPERSONELKESINTI]
                                   ([PKOD]
                                   ,[YIL]
                                   ,[AY]
                                   ,[GIDERKOD]
                                   ,[PLOJMANKOD]
                                   ,[AKTIF]
                                   ,[ENDEX]
                                   ,[TUTAR]
                                   ,[ELDEN]
                                   ,[ACIKLAMA])
                        SELECT lojmanAylikKesintiPersonelPk
                        , lojmanAylikKesintiYil
                        , lojmanAylikKesintiAy
                        , 7
                        , lojmanAylikKesintiAdresPk
                        ,[lojmanAylikKesintiAsansorGiderAktif]
                        ,0
                        ,[lojmanAylikKesintiAsansorGider]
                        ,0
                        ,''
                        FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]";

            return sql;
        }

        public static Sonuc PersonelSiteKesintisiAktar(string kullaniciKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", kullaniciKodu);
            string siteGiderleriSql = @"SELECT   [pkLojmanAylikKesinti]
                                                ,[lojmanAylikKesintiYil]
                                                ,[lojmanAylikKesintiAy]
                                                ,[lojmanAylikKesintiAktif]
                                                ,[lojmanAylikKesintiAdresPk]
                                                ,[lojmanAylikKesintiPersonelGider]
                                                ,[lojmanAylikKesintiPersonelGiderAktif]
                                                ,[lojmanAylikKesintiElektrikGider]
                                                ,[lojmanAylikKesintiElektrikGiderAktif]
                                                ,[lojmanAylikKesintiElektrikGiderElden]
                                                ,[lojmanAylikKesintiElektrikEndex]
                                                ,[lojmanAylikKesintiGazGider]
                                                ,[lojmanAylikKesintiGazGiderAktif]
                                                ,[lojmanAylikKesintiGazGiderElden]
                                                ,[lojmanAylikKesintiCevreGider]
                                                ,[lojmanAylikKesintiCevreGiderAktif]
                                                ,[lojmanAylikKesintiSuGider]
                                                ,[lojmanAylikKesintiSuGiderAktif]
                                                ,[lojmanAylikKesintiSuGiderElden]
                                                ,[lojmanAylikKesintiSuEndex]
                                                ,[lojmanAylikKesintiDigerGider]
                                                ,[lojmanAylikKesintiDigerGiderAktif]
                                                ,[lojmanAylikKesintiDigerGiderElden]
                                                ,[lojmanAylikKesintiDigerGiderAciklama]
                                                ,[lojmanAylikKesintiAsansorGider]
                                                ,[lojmanAylikKesintiAsansorGiderAktif]
                                                ,[lojmanAylikKesintiToplamGider]
                                                FROM [CBPersonel2].[lojman].[lojmanAylikKesinti]";

            IDataReader r = ve.DataReaderDondur(siteGiderleriSql);
            ObjectArray oa = new ObjectArray();
            if (r == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa.sonuc;
            }

            while (r.Read())
            {
                PersonelSiteKesintiToplam sg = new PersonelSiteKesintiToplam();
                sg.kod = (int)r.GetInt64(0);
                sg.yil = VeriErisim.DegerAl(r, 1, 0);
                sg.ay = VeriErisim.DegerAl(r, 2, 0);
                //sg.aktif = VeriErisim.DegerAl(r, 3, 0);
                sg.pLojmanKod = VeriErisim.DegerAl(r, 4, 0);
                sg.toplamTutar = VeriErisim.DegerAl(r, 26, 0);
                sg.kesintiler = new PersonelSiteKesinti[7];
                sg.kesintiler[0] = new PersonelSiteKesinti();
                sg.kesintiler[0].giderKod = 1;
                sg.kesintiler[0].tutar = r.IsDBNull(5) ? 0 : r.GetDouble(5);
                sg.kesintiler[0].aktif = r.IsDBNull(6) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(6), 0);
                sg.kesintiler[1] = new PersonelSiteKesinti();
                sg.kesintiler[1].giderKod = 2;
                sg.kesintiler[1].tutar = r.IsDBNull(7) ? 0 : r.GetDouble(7);
                sg.kesintiler[1].aktif = r.IsDBNull(8) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(8), 0);
                sg.kesintiler[1].elden = r.IsDBNull(9) ? 0 : r.GetDouble(9);
                sg.kesintiler[1].endex = r.IsDBNull(10) ? 0 : r.GetDouble(10);
                sg.kesintiler[2] = new PersonelSiteKesinti();
                sg.kesintiler[2].giderKod = 3;
                sg.kesintiler[2].tutar = r.IsDBNull(11) ? 0 : r.GetDouble(11);
                sg.kesintiler[2].aktif = r.IsDBNull(12) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(12), 0);
                sg.kesintiler[2].elden = r.IsDBNull(13) ? 0 : r.GetDouble(13);
                sg.kesintiler[3] = new PersonelSiteKesinti();
                sg.kesintiler[3].giderKod = 4;
                sg.kesintiler[3].tutar = r.IsDBNull(14) ? 0 : r.GetDouble(14);
                sg.kesintiler[3].aktif = r.IsDBNull(15) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(15), 0);
                sg.kesintiler[4] = new PersonelSiteKesinti();
                sg.kesintiler[4].giderKod = 5;
                sg.kesintiler[4].tutar = r.IsDBNull(16) ? 0 : r.GetDouble(16);
                sg.kesintiler[4].aktif = r.IsDBNull(17) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(17), 0);
                sg.kesintiler[4].elden = r.IsDBNull(18) ? 0 : r.GetDouble(18);
                sg.kesintiler[4].endex = r.IsDBNull(19) ? 0 : r.GetDouble(19);
                sg.kesintiler[5] = new PersonelSiteKesinti();
                sg.kesintiler[5].giderKod = 6;
                sg.kesintiler[5].tutar = r.IsDBNull(20) ? 0 : r.GetDouble(20);
                sg.kesintiler[5].aktif = r.IsDBNull(21) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(21), 0);
                sg.kesintiler[5].elden = r.IsDBNull(22) ? 0 : r.GetDouble(22);
                sg.kesintiler[5].aciklama = VeriErisim.DegerAl(r, 23, "");
                sg.kesintiler[6] = new PersonelSiteKesinti();
                sg.kesintiler[6].giderKod = 7;
                sg.kesintiler[6].tutar = r.IsDBNull(24) ? 0 : r.GetDouble(24);
                sg.kesintiler[6].aktif = r.IsDBNull(25) ? 0 : OrtakFonksiyonlar.ConvertToInt(r.GetBoolean(25), 0);
                oa.Ekle(sg);
            }

            r.Dispose();
            int sayac = 1;
            string insertSql = "truncate table IKYPERSONELSITEKESINTISI; DELETE IKYPERSONELSITEKESINTITOPLAM;";
            Sonuc sonuc = new Sonuc();

            foreach (PersonelSiteKesintiToplam s in oa.objeler)
            {
                insertSql += @"INSERT INTO [dbo].[IKYPERSONELSITEKESINTITOPLAM]
                                           ([KOD]
                                           ,[PLOJMANKOD]
                                           ,[YIL]
                                           ,[AY]
                                           ,[TOPLAMTUTAR])
                                     VALUES
                                           (" + s.kod
                                           + ", " + s.pLojmanKod
                                           + ", " + s.yil
                                           + ", " + s.ay
                                           + ", " + s.toplamTutar + "); ";

                for (int i = 0; i < s.kesintiler.Length; i++)
                {
                    //if (s.giderler[i].miktar > 0)
                    //{
                    insertSql += @"INSERT INTO [dbo].[IKYPERSONELSITEKESINTISI]
                                               ([KOD]
                                               ,[SKTKOD]
                                               ,[GIDERKOD]
                                               ,[AKTIF]
                                               ,[ENDEX]
                                               ,[TUTAR]                                               
                                               ,[ELDEN]
                                               ,[ACIKLAMA])
                                     VALUES
                                           (" + sayac
                                           + ", " + s.kod
                                           + ", " + s.kesintiler[i].giderKod
                                           + ", " + s.kesintiler[i].aktif
                                           + ", '" + s.kesintiler[i].endex + "'"
                                           + ", '" + s.kesintiler[i].tutar + "'"
                                           + ", '" + s.kesintiler[i].elden + "'"
                                           + ", '" + s.kesintiler[i].aciklama + "') ; ";
                    //}
                    sayac++;
                }
            }

            insertSql += @"delete SAYAC
                           where KOD = 'IKYPERSONELSITEKESINTISI';

                           INSERT INTO SAYAC
                                (KOD
                                ,SAYAC)
                           VALUES
                                ('IKYPERSONELSITEKESINTISI',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from IKYPERSONELSITEKESINTISI)
                                ); ";

            insertSql += @"delete SAYAC
                           where KOD = 'IKYPERSONELSITEKESINTITOPLAM';

                           INSERT INTO SAYAC
                                (KOD
                                ,SAYAC)
                           VALUES
                                ('IKYPERSONELSITEKESINTITOPLAM',
		                        (select right('000000000'+convert(varchar(9), max(KOD)), 9)  from IKYPERSONELSITEKESINTITOPLAM)
                                )";

            ve.BeginTransaction();

            int ss = ve.ExecuteNonquery(insertSql);

            if (ss > 0)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                sonuc.islemSonuc = true;
            }
            else
            {
                ve.Rollback();
                sonuc.DegerAta(ve.hata);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        //        public static Sonuc ORGBirimAktar(VeriErisim ve)
        //        {
        //            Sonuc sonuc = new Sonuc();
        //            ve.BeginTransaction();
        //            string sql = @"truncate table Kullanici.dbo.ORGBIRIM;
        //insert into Kullanici.dbo.ORGBIRIM (KOD, ADI, KURUMKOD, TURKOD, ANABIRIMKOD)
        //select [pkKodBirimler]
        //      ,[kodBirimlerEtiket]
        //      ,[kodKurumPk]   
        //      ,[kodBirimTipiPk]
        //      ,[kodCbBirimParent]  
        //from [CBPersonel].dbo.kodCbBirim 
        //where kodCbBirimAktif = 1;
        //
        //update Kullanici.[dbo].[ORGBIRIM]
        //set [ENTEGREKOD] = [KOD];
        //
        //update Kullanici.dbo.ORGBIRIM
        //set SABLONMU = 0;
        //
        //Declare @varkod varchar(3)
        //SET @varkod = (SELECT kod FROM Kullanici.dbo.ORGBIRIM where ANABIRIMKOD is null);
        //update Kullanici.dbo.ORGBIRIM
        //set KOPRUKOD = @varkod where ANABIRIMKOD is null
        //;with CTE 
        //as
        //(
        //    SELECT ANABIRIMKOD, kod, convert(varchar(15), ANABIRIMKOD) + right('000'+convert(varchar(3), [KOD]),3)  AS kopruKod, 
        //      0 as level
        //    FROM Kullanici.dbo.ORGBIRIM 
        //    WHERE ANABIRIMKOD = @varkod     
        //    UNION ALL
        //    SELECT c.ANABIRIMKOD, c.kod, convert(varchar(15), CTE.kopruKod) + right('000'+convert(varchar(3), c.[KOD]),3) AS kopruKod,       
        //      CTE.level + 1 as level
        //    FROM Kullanici.DBO.ORGBIRIM c
        //    JOIN CTE
        //      ON CTE.kod = c.ANABIRIMKOD    
        //)
        //UPDATE Kullanici.dbo.ORGBIRIM
        //SET KOPRUKOD = CTE.kopruKod
        //FROM Kullanici.dbo.ORGBIRIM AS OB
        //JOIN CTE ON OB.KOD = CTE.KOD";

        //            try
        //            {
        //                ve.ExecuteNonquery(sql);
        //                if (string.IsNullOrEmpty(ve.hata))
        //                {
        //                    ve.Commit();
        //                    sonuc.islemSonuc = true;
        //                    sonuc.bilgiStr = "Aktarım başarı ile gerçekleşti.";
        //                }
        //                else
        //                {
        //                    ve.Rollback();
        //                    sonuc.islemSonuc = false;
        //                    sonuc.hataStr = "Aktarım başarısız. " + ve.hata;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                sonuc.islemSonuc = false;
        //                sonuc.bilgiStr = ex.Message;
        //            }

        //            ve.VeriErisimKapat();
        //            return sonuc;
        //        }

    }
}
