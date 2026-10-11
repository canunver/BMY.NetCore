using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonel : TABOrtak
    {
        public TABPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONEL";

        //"PERSONELTUR", "CINSIYET", "ILKSOYAD", "DOGUMTARIHI", "DURUM", 

        public enum ENUMAlanlar : int
        {
            KOD = 0, TCKIMLIKNO = 1, AD = 2, SOYAD = 3, KURUMSICILNO = 4, SAKATLIKDURUMU = 5,
            EMEKLISICILNO = 6, SSKSICILNO = 7, BAGKURSICILNO = 8, ILKISEBASLAMATARIHI = 9,
            ASALETTARIHI = 10, KURUMABASLAMATARIHI = 11, ACIKLAMA = 12, SAKATLIKDERECESI = 13, TERORMAGDURU = 14, HUKUMLU = 15,
            KORUNMAYAMUHTAC = 16, SOSYALGUVENCE = 17, BANKAIBANNO = 18, VERGIDAIRESI = 19, AKTIFMI = 20,
            GELDIGIKURUM = 21, ARSIVNO = 22, GELDIGIKURUMSICILI = 23, RUMUZ = 24, TITR = 25
        }
        public static string[] ALANLAR = {  "KOD", "TCKIMLIKNO", "AD", "SOYAD", "KURUMSICILNO", "SAKATLIKDURUMU",
                                            "EMEKLISICILNO", "SSKSICILNO", "BAGKURSICILNO", "MEMURIYETEBASLAMATARIHI",
                                            "ASALETTARIHI", "KURUMABASLAMATARIHI", "ACIKLAMA", "SAKATLIKDERECESI", "TERORMAGDURU", "HUKUMLU",
                                            "KORUNMAYAMUHTAC", "SOSYALGUVENCE", "BANKAIBANNO", "VERGIDAIRESI", "AKTIFMI", "GELDIGIKURUM", "ARSIVNO", "GELDIGIKURUMSICILI", "RUMUZ", "TITR" };
        public static string TABLOADNufus = "IKYPERSONELNUFUS";
        public enum ENUMAlanlarNUFUS : int
        {
            KOD = 0,
            DOGUMYERI = 1,
            MEDENIHALI = 2,
            ANAAD = 3,
            BABAAD = 4,
            KANGRUBU = 5,
            CUZDANSERINO = 6,
            CUZDANVERILISNEDENI = 7,
            CUZDANVERILISTARIHI = 8,
            CUZDANVERILDIGIYER = 9,
            CUZDANKAYITNO = 10,
            NUFUSILKOD = 11,
            NUFUSILCEKOD = 12,
            NUFUSMAHALLEKOY = 13,
            NUFUSCILTNO = 14,
            NUFUSSIRANO = 15,
            NUFUSAILESIRANO = 16,
            ILKSOYAD = 17,
            DOGUMTARIHI = 18,
            NUFUSDINI = 19,
            CINSIYET = 20,
            PASAPORTNO = 21,
            PASAPORTVERILISTARIHI = 22,
            PASAPORTVERILDIGIYER = 23,
            PASAPORTVERENMAKAM = 24,
            PASAPORTSURESI = 25
        }
        public static string[] ALANLARNUFUS = { "KOD", "DOGUMYERI", "MEDENIHALI", "ANAAD", "BABAAD", "KANGRUBU", "CUZDANSERINO", "CUZDANVERILISNEDENI",
                                                "CUZDANVERILISTARIHI", "CUZDANVERILDIGIYER", "CUZDANKAYITNO", "NUFUSILKOD", "NUFUSILCEKOD",
                                                "NUFUSMAHALLEKOY", "NUFUSCILTNO", "NUFUSSIRANO", "NUFUSAILESIRANO", "ILKSOYAD", "DOGUMTARIHI","NUFUSDINI", "CINSIYET",
                                                "PASAPORTNO","PASAPORTVERILISTARIHI","PASAPORTVERILDIGIYER","PASAPORTVERENMAKAM","PASAPORTSURESI" };

        public static string TABLOADIletisim = "IKYPERSONELILETISIM";
        public enum ENUMAlanlarIletisim : int
        {
            KOD = 0,
            ISTELEFON = 1,
            CEPTELEFON = 2,
            FAKS = 3,
            EPOSTA = 4,
            ADRES = 5,
            EVTELEFON = 6,
            URL = 7,
            MOBILIMZATELEFON = 8,
            CEPTELEFON2 = 9,
            TARABYATEL = 10,
            LOJMANTEL = 11,
            TELSIZ = 12,
            GORUNTULU = 13,
            REHBERDEGOZUKSUN = 14,
            EPOSTA2 = 15
        }
        public static string[] ALANLARILETISIM = { "KOD", "ISTELEFON", "CEPTELEFON", "FAKS", "EPOSTA", "ADRES", "EVTELEFON", "URL", "MOBILIMZATELEFON", "CEPTELEFON2", "TARABYATEL", "LOJMANTEL", "TELSIZ", "GORUNTULU", "REHBERDEGOZUKSUN", "EPOSTA2" };

        static string TABLOADMALBILDIRIMI = "IKYPERSONELMALBILDIRIMI";
        public enum ENUMAlanlarMalBildirimi : int
        {
            KOD = 0,
            PKOD = 1,
            TARIH = 2,
            ACIKLAMA = 3
        }

        static string[] ALANLARMALBILDIRIMI = { "KOD", "PKOD", "TARIH", "ACIKLAMA" };

        static string TABLOADCEZAODUL = "IKYPERSONELCEZAODUL";
        public enum ENUMAlanlarCezaOdul : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            TIP = 2,
            CEZAODULTIP = 3,
            VERILISNEDENI = 4,
            TARIH = 5,
            BELGENO = 6,
            DAYANAK = 7,
            BELGETARIHI = 8
        }
        static string[] ALANLARCEZAODUL = { "TCKIMLIKNO", "SIRANO", "TIP", "CEZAODULTIP", "VERILISNEDENI", "TARIH", "BELGENO", "DAYANAK", "BELGETARIHI" };

        static string TABLOADOGRENIM = "IKYPERSONELOGRENIM";
        public enum ENUMAlanlarOgrenim : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            OGRENIMKOD = 2,
            OKULADI = 3,
            FAKULTEBOLUM = 4,
            BELGETUR = 5,
            BELGETARIHI = 6,
            BELGENO = 7,
            BASLANGICTARIHI = 8,
            BITIRMETARIHI = 9,
            DERECE = 10,
            GECERLI = 11
        }
        static string[] ALANLAROGRENIM = { "TCKIMLIKNO", "SIRANO", "OGRENIMKOD", "OKULADI", "FAKULTEBOLUM", "BELGETUR", "BELGETARIHI", "BELGENO", "BASLANGICTARIHI", "BITIRMETARIHI", "DERECE", "GECERLI" };

        static string TABLOADDIL = "IKYPERSONELDIL";
        public enum ENUMAlanlarDil : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            DILKODU = 2,
            DUZEY = 3,
            PUAN = 4,
            DONEMAY = 5,
            DONEMYIL = 6,
            BELGETUR = 7,
            BELGETARIHI = 8,
            BELGENO = 9,
            BORDROYAESAS = 10
        }
        static string[] ALANLARDIL = { "TCKIMLIKNO", "SIRANO", "DILKODU", "DUZEY", "PUAN", "DONEMAY", "DONEMYIL", "BELGETUR", "BELGETARIHI", "BELGENO", "BORDROYAESAS" };

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOADKURS = "IKYPERSONELKURS";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        enum ENUMAlanlarKurs : int
        {
            /// <summary>
            /// T.C. Kimlik Numarası
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// Sıra Numarası
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Eğitim(Kurs)
            /// </summary>
            KURSKODU = 2,
            /// <summary>
            /// Aldığı Yer
            /// </summary>
            ALDIGIYER = 3,
            /// <summary>
            /// Başlama Tarihi
            /// </summary>
            BASLAMATARIHI = 4,
            /// <summary>
            /// Bitirme Tarihi
            /// </summary>
            BITIRMETARIHI = 5,
            /// <summary>
            /// Derece
            /// </summary>
            DERECE = 6,
            /// <summary>
            /// Kurs Adı
            /// </summary>
            KURSADI = 7,
            /// <summary>
            /// Ders Saati
            /// </summary>
            DERSSAATI = 8,
            /// <summary>
            /// Eğitim Yeri
            /// </summary>
            EGITIMYERI = 9,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 10,
            /// <summary>
            /// Dönem
            /// </summary>
            DONEM = 11,
            /// <summary>
            /// Yıl
            /// </summary>
            YIL = 12,
            /// <summary>
            /// Dosya Numarası
            /// </summary>
            DOSYANO = 13,
            /// <summary>
            /// Gün
            /// </summary>
            GUN = 14,
        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLARKURS = { "TCKIMLIKNO", "SIRANO", "KURSKODU", "ALDIGIYER", "BASLAMATARIHI", "BITIRMETARIHI", "DERECE", "KURSADI", "DERSSAATI", "EGITIMYERI", "ACIKLAMA", "DONEM", "YIL", "DOSYANO", "GUN" };

        static string TABLOADDILSOZLUK = "IKYDIL";
        enum ENUMAlanlarDilSozluk : int
        {
            KOD = 0,
            AD = 1,
        }
        static string[] ALANLARDILSOZLUK = { "KOD", "AD" };

        static string TABLOADKURSSOZLUK = "IKYKURS";
        enum ENUMAlanlarKursSozluk : int
        {
            KOD = 0,
            AD = 1,
        }
        static string[] ALANLARKURSSOZLUK = { "KOD", "AD" };

        //MAAŞ
        //*****************************************************************
        static string TABLOADSOZLESMELIBORDROTANIM = "IKYPERSONELSOZBORDROTANIM";
        public enum ENUMAlanlarSozlesmeliBordro : int
        {
            TCKIMLIKNO = 0,
            YIL = 1,
            AY = 2,
            KUMULATIFVERGIMATRAHI = 3,
        }
        static string[] ALANLARSOZLESMELIBORDROTANIM = { "TCKIMLIKNO", "YIL", "AY", "KUMULATIFVERGIMATRAHI" };

        static string TABLOADIZIN = "IKYPERSONELIZIN";
        public enum ENUMAlanlarIzin : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            TIP = 2,
            ACIKLAMA = 3,
            BASLANGICTARIHI = 4,
            BITISTARIHI = 5,
            TELEFON = 6,
            KULLANILANGUN = 7,
            IZINNOTU = 8,
            ONAY = 9,
            EVREVRAKNO = 10,
            PREVRAKNO = 11,
            EVRAKALAN1 = 12,
            EVRAKNOT1 = 13
        }
        static string[] ALANLARIZIN = { "TCKIMLIKNO", "SIRANO", "TIP", "ACIKLAMA", "BASLANGICTARIHI", "BITISTARIHI", "TELEFON", "KULLANILANGUN", "IZINNOTU", "ONAY", "EVREVRAKNO", "PREVRAKNO", "EVRAKALAN1", "EVRAKNOT1" };

        static string TABLOADIZINDEVIR = "IKYPERSONELIZINDEVIR";
        public enum ENUMAlanlarIzinDevir : int
        {
            TCKIMLIKNO = 0,
            YIL = 1,
            DEVREDEN = 2,
            YILICI = 3,
        }
        static string[] ALANLARIZINDEVIR = { "TCKIMLIKNO", "YIL", "DEVREDEN", "YILICI" };

        public static string TABLOADTARIHCE = "IKYPERSONELTARIHCE";
        public enum ENUMAlanlarTarihce : int
        {
            KOD = 0,
            PKOD = 1,
            KULLANICI = 2,
            TARIH = 3,
            DEGISIKLIK = 4,
            KULLANICIAD = 5
        }
        public static string[] ALANLARTARIHCE = { "KOD", "PKOD", "KULLANICI", "TARIH", "DEGISIKLIK", "KULLANICIAD" };

        //*****************************************************************

        public IDataReader PersonelLojmanliListele(Personel p, string kopruKod = null)
        {
            string kosul = string.Empty;
            string sql = @"select p.KOD, pl.KOD, p.AD+' '+p.SOYAD, s.SITEADI,b.BlokNo,d.DaireNo
                        from IKYPERSONEL p
                        left outer join IKYPERSONELLOJMAN pl on pl.PKOD = p.KOD and pl.BitTarihi is null
                        left outer join IKYLOJMANDAIRE d on d.KOD = pl.DKOD
                        left outer join IKYLOJMANBLOK b on b.KOD = d.BKOD
                        left outer join IKYLOJMANSITE s on s.KOD = b.SKOD
                        LEFT OUTER JOIN IKYPERSONELKADRO ON IKYPERSONELKADRO.PKOD = p.KOD AND KADRODURUMU in (106)
                        LEFT OUTER JOIN IKYUNVAN ON IKYUNVAN.KOD = UNVANKOD
                        LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM OB ON OB.KOD = IKYPERSONELKADRO.ORGANIZASYONKOD
                        left outer join IKYPERSONELNUFUS n on p.KOD = n.KOD
                        LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C ON C.KOD = n.NUFUSILKOD";

            if (!string.IsNullOrWhiteSpace(p.genelSorgu))
            {
                string disSorgu = "";
                foreach (var itemDis in p.genelSorgu.Split(';'))
                {
                    string sorDis = itemDis.Trim();

                    string icSorgu = "";
                    foreach (var item in sorDis.Split(','))
                    {
                        string sor = item.Trim();
                        if (!string.IsNullOrWhiteSpace(sor))
                        {
                            icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("", "p.AD + ' ' + p.SOYAD", "LIKE", "%" + sor + "%"));
                            icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("IKYUNVAN", "AD", "LIKE", "%" + sor + "%"));
                            icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("C", "AD", "LIKE", "%" + sor + "%"));
                            icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.KURUMSICILNO], "=", sor));
                            icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sor));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(icSorgu))
                    {
                        disSorgu = VeriErisim.KosulEkle(disSorgu, "AND", "(" + icSorgu + ")");
                    }
                }
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + disSorgu + ")");
            }

            if (!string.IsNullOrWhiteSpace(kopruKod))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "OB.KOPRUKOD LIKE " + VeriErisim.DegerYap(kopruKod + "%"));
            }
            if (p.kadro.unvan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD], "=", p.kadro.unvan));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(Personel p, string kopruKod, string kopruKodlar, TNSDateTime gorevBitisBasla = null, TNSDateTime gorevBitisSon = null, TNSDateTime zorunluEmeklilikTarihi = null)
        {
            string resimSorgu, egitimSorgu, lojmanSorgu, kadroSorgu, maasParamSorgu, matrahSorgu, aileSorgu, dilSorgu, hizmetSuresiSorgu, izinSorgu, gorevSorgu, giderSorgu, cezaSorgu, sendikaSorgu;

            //, , , , , , , , , 
            //, bool resim, bool egitimler, bool lojmanlar, bool kadrolar, bool maasParamlar, bool matrahlar, bool aileler, bool diller, bool hizmetSuresi
            if (p.resimOlsun) resimSorgu = ", r.RESIM";
            else resimSorgu = ", NULL";

            if (p.ogrenimListesiOlsun) egitimSorgu = "\r\n, (" + TABPersonelOgrenim.ListeleUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else egitimSorgu = ", NULL";

            if (p.lojmanListesiOlsun) lojmanSorgu = "\r\n, (" + TABPersonelLojman.ListeleUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], null, false) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else lojmanSorgu = ", NULL";

            if (p.kadrolarOlsun) kadroSorgu = "\r\n, (" + TABPersonelKadro.ListeleKadroUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], true) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else kadroSorgu = ", NULL";

            if (p.maasParamlarOlsun) maasParamSorgu = "\r\n, (" + PERPERSONELMAASPARAM.ListeleParamUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else maasParamSorgu = ", NULL";

            if (!TNSDateTime.IsNull(p.matrahTarih)) matrahSorgu = "\r\n, (" + PERPERSONELMAASPARAM.ListeleMatrahUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], p.matrahTarih.Yil) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else matrahSorgu = ", NULL";

            if (p.ailelerOlsun) aileSorgu = "\r\n, (" + TABAile.ListeleUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else aileSorgu = ", NULL";

            if (p.dillerOlsun) dilSorgu = "\r\n, (" + TABPersonelDil.ListeleUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else dilSorgu = ", NULL";

            if (p.hizmetSuresiOlsun)
            {
                hizmetSuresiSorgu = "\r\n, (" + TABPersonelKadro.HizmetSuresiGetirUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], p.aktifTarih) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
                if (TNSDateTime.IsNull(p.aktifTarih))
                    hizmetSuresiSorgu += ", NULL";
                else
                {
                    TNSDateTime tt = new TNSDateTime(p.aktifTarih.Oku());
                    tt.AddMonths(-1);
                    hizmetSuresiSorgu += "\r\n, (" + TABPersonelKadro.HizmetSuresiGetirUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], tt) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
                }
            }
            else
                hizmetSuresiSorgu = ", NULL, NULL";

            //İzin Sorgu
            if (!TNSDateTime.IsNull(p.izinDonemBas)) izinSorgu = "\r\n, (" + TABPersonelIzin.ListeleUstTablo(p, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else izinSorgu = ", NULL";

            //Görev Sorgu
            if (p.gorevYili != 0 || (!TNSDateTime.IsNull(gorevBitisBasla) && !TNSDateTime.IsNull(gorevBitisSon))) gorevSorgu = "\r\n, (" + TABIKYGOREVLER.ListeleUstTablo(p.gorevYili, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], gorevBitisBasla, gorevBitisSon) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else gorevSorgu = ", NULL";

            //Gider Sorgu
            if (!TNS.TNSDateTime.IsNull(p.giderBasla) || !TNS.TNSDateTime.IsNull(p.giderBitis))
                giderSorgu = "\r\n, (" + TABPERSONELGIDER.ListeleUstTablo(p.giderBasla, p.giderBitis, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else giderSorgu = ", NULL";

            if (p.cezalarOlsun > 0)
                cezaSorgu = "\r\n, (" + TABCezaOdul.ListeleUstTablo(p.cezalarOlsun, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else cezaSorgu = ", NULL";

            if (p.sendikaOlsun)
                sendikaSorgu = "\r\n, (" + TABIKYSENDIKABILGI.ListeleUstTablo(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            else sendikaSorgu = ", NULL";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " + VeriErisim.TumAlanlar(TABLOADNufus, ALANLARNUFUS) + ", " + VeriErisim.TumAlanlar(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR) + ", " + VeriErisim.TumAlanlar("i", ALANLARILETISIM) +
                         ", IKYUNVAN.AD, IKYPERSONELTIPI.AD, SOZLUK4.KAYITAD, C.AD, C2.AD, OB.ADI, OB.KOPRUKOD, IKYUNVAN.SIRANO, u2.AD, OBG.ADI, IKYUNVAN.SINIFKOD, OB.TURKOD, OBG.TURKOD, MUH.MUHASEBEAD, MUHBIR.BIRIMAD " +
                         resimSorgu + egitimSorgu + lojmanSorgu + kadroSorgu + maasParamSorgu + matrahSorgu + aileSorgu + dilSorgu + hizmetSuresiSorgu + izinSorgu + gorevSorgu + ", OBGIT.KOPRUKOD, HSA.SEBEPADI" + giderSorgu + cezaSorgu + sendikaSorgu;
            sql += "\r\n";
            sql += PersonelKosul(p, kopruKod, kopruKodlar, zorunluEmeklilikTarihi);
            string siralama = "";
            if (p.listeSiralama == ENUMPersonelSira.Unvan)
            {
                siralama = "case when IKYUNVAN.SIRANO IS NULL then 999999999 when IKYUNVAN.SIRANO=0 then 999999998 else IKYUNVAN.SIRANO end, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMSICILNO] + ", ";
            }
            else if (p.listeSiralama == ENUMPersonelSira.Birim)
            {
                siralama = "OB.KOPRUKOD," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMSICILNO] + ", ";
            }
            else if (p.listeSiralama == ENUMPersonelSira.UnvanAd)
            {
                siralama = "IKYUNVAN.AD, ";
            }
            else if (p.listeSiralama == ENUMPersonelSira.SicilNo)
            {
                siralama = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMSICILNO] + ", ";
            }
            siralama += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            sql += " ORDER BY " + siralama;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKod(TNSCollection tcKimlikNoList, TNSCollection tcSicilNoList)
        {
            string sql = $"SELECT {ALANLAR[(int)ENUMAlanlar.KOD]}, {ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO]}, {ALANLAR[(int)ENUMAlanlar.KURUMSICILNO]} FROM {TABLOAD}";
            string kosul = "";
            if (tcKimlikNoList != null)
                kosul = VeriErisim.KosulEkle(kosul, "OR", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " IN (" + VirgulleBirlestir(tcKimlikNoList) + ")");
            if (tcSicilNoList != null)
                kosul = VeriErisim.KosulEkle(kosul, "OR", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMSICILNO] + " IN (" + VirgulleBirlestir(tcSicilNoList) + ")");
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }

        public string PersonelKosul(Personel p, string kopruKod, string kopruKodlar, TNSDateTime zorunluEmeklilikTarihi = null)
        {
            string sql = " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADNufus + " ON " + TABLOAD + ".KOD = " + TABLOADNufus + ".KOD";
            if (TNSDateTime.IsNull(p.aktifTarih) || p.aktifTarihTur != 10)
                sql += " LEFT OUTER JOIN IKYPERSONELKADRO ON IKYPERSONELKADRO.PKOD = IKYPERSONEL.KOD AND " + TABGenel.KadroDurumuAktifler("IKYPERSONELKADRO.KADRODURUMU");
            else
            {
                string tarihSorgu;
                string maxMin;
                if (p.aktifTarihSonHareket)
                    maxMin = "MAX";
                else
                    maxMin = "MIN";
                if (TNSDateTime.IsNull(p.aktifTarihSon))
                    tarihSorgu = "dbo.GunTarihAraligindaMi(" + VeriErisim.DegerYap(p.aktifTarih) + ", IC.AtamaTarihi, IC.AYRILMATARIHI)=1";
                else
                    tarihSorgu = "dbo.DonemlerKesisiyorMu(" + VeriErisim.DegerYap(p.aktifTarih) + ", " + VeriErisim.DegerYap(p.aktifTarihSon) + ", IC.AtamaTarihi, IC.AYRILMATARIHI)=1";
                string arananTipler = PersonelTipKosul("AND", "IC." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.PERSONELTUR], p, false);

                sql += " LEFT OUTER JOIN IKYPERSONELKADRO ON IKYPERSONELKADRO.KOD = (SELECT " + maxMin + "(IC.KOD) FROM IKYPERSONELKADRO IC INNER JOIN IKYSOZLUKHIZMETSEBEBI SHS ON SHS.KOD=IC.HIZMETSEBEBIKOD WHERE SHS.TIP IN (1, 5, 6, 8, 9) AND IC.PKOD = IKYPERSONEL.KOD AND " + tarihSorgu + " AND " + TABGenel.KadroDurumuIptalHaric("IC.KADRODURUMU") + " " + arananTipler + ")";
            }
            sql += " left outer join IKYPERSONELILETISIM i on i.kod = " + TABLOAD + ".KOD";
            sql += " LEFT OUTER JOIN IKYUNVAN ON IKYUNVAN.KOD = UNVANKOD";
            sql += " LEFT OUTER JOIN IKYPERSONELTIPI ON IKYPERSONELKADRO.PERSONELTUR = IKYPERSONELTIPI.KOD";
            sql += " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK4 ON MEDENIHALI = SOZLUK4.KOD";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C ON C.KOD = " + TABLOADNufus + "." + ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILKOD];
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C2 ON C2.KOD = " + TABLOADNufus + "." + ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILCEKOD];
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM OB ON OB.KOD = IKYPERSONELKADRO.ORGANIZASYONKOD";
            sql += " LEFT OUTER JOIN IKYUNVAN as u2 ON u2.KOD = GorunenUnvanKodu";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM OBG ON OBG.KOD = IKYPERSONELKADRO.GOREVORGANIZASYONKOD";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM MUHBIR ON MUHBIR.BIRIMKOD = IKYPERSONELKADRO.MuhasebeBirimi AND MUHBIR.MUHASEBEKOD = IKYPERSONELKADRO.MuhasebeKodu AND MUHBIR.KURKOD=IKYPERSONELKADRO.KurumsalKod";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH ON MUH.MUHASEBEKOD = IKYPERSONELKADRO.MuhasebeKodu";
            sql += " LEFT OUTER JOIN IKYPERSONELRESIM r on r.KOD = IKYPERSONEL.KOD ";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM OBGIT ON OBGIT.KOD = IKYPERSONELKADRO.TAYINOLACAGIBIRIMKOD";

            sql += " LEFT OUTER JOIN IKYSOZLUKHIZMETSEBEBI HSA ON HSA.KOD = IKYPERSONELKADRO.AYRILMAGEREKCESI";

            if (p.personelSiteKod > 0 || p.personelSiteKod == -1)
            {
                sql += " INNER JOIN IKYPERSONELLOJMAN l on l.PKOD = IKYPERSONEL.KOD INNER JOIN IKYLOJMANDAIRE d on d.KOD = l.DKOD INNER JOIN IKYLOJMANBLOK b on b.KOD = d.BKOD INNER JOIN IKYLOJMANSITE s on s.KOD = b.SKOD ";
            }

            string kosul = string.Empty;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            else if (!string.IsNullOrWhiteSpace(p.kodlar))
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + p.kodlar + ")");
            else if (p.tcList != null && p.tcList.Count > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " IN (" + VirgulleBirlestir(p.tcList) + ")");
            }
            else
            {
                if (p.aktifTarih == null || p.aktifTarih.isNull || p.aktifTarihTur != 10)
                {
                    if (p.aktifDurum > -1 && string.IsNullOrWhiteSpace(p.aktifDurumlar))
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIFMI], "=", p.aktifDurum));
                    else if (!string.IsNullOrWhiteSpace(p.aktifDurumlar))
                        kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AKTIFMI] + " IN (" + p.aktifDurumlar + ")");
                    if (p.aktifTarih != null && !p.aktifTarih.isNull && p.aktifTarihTur >= 20)
                    {
                        string islec = "";
                        string alan = "";
                        if (p.aktifTarihTur == 20 || p.aktifTarihTur == 30)
                            islec = ">=";
                        else
                            islec = "<=";

                        if (p.aktifTarihTur >= 20 && p.aktifTarihTur < 30)
                            alan = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMABASLAMATARIHI];
                        else
                            alan = "IKYPERSONELKADRO.AYRILMATARIHI";
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", alan, islec, p.aktifTarih));
                    }
                }

                if (p.sicilNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSICILNO], "=", p.sicilNo));

                if (p.ad != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", p.ad + "%"));

                if (p.soyad != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", p.soyad + "%"));
                if (!string.IsNullOrWhiteSpace(p.genelSorgu))
                {
                    string disSorgu = "";
                    foreach (var itemDis in p.genelSorgu.Split(';'))
                    {
                        string sorDis = itemDis.Trim();

                        string icSorgu = "";
                        foreach (var item in sorDis.Split(','))
                        {
                            string sor = item.Trim();
                            if (!string.IsNullOrWhiteSpace(sor))
                            {
                                if (!string.IsNullOrWhiteSpace(kopruKodlar))
                                {
                                    icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.OR_KosulOl("OB.KOPRUKOD", kopruKodlar, true, false, 2));
                                    kopruKodlar = "";
                                }
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("", "IKYPERSONEL.AD + ' ' + IKYPERSONEL.SOYAD", "LIKE", "%" + sor + "%"));
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("IKYUNVAN", "AD", "LIKE", "%" + sor + "%"));
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("C", "AD", "LIKE", "%" + sor + "%"));
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSICILNO], "=", sor));
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sor));
                                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELDIGIKURUMSICILI], "=", sor));
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(icSorgu))
                        {
                            disSorgu = VeriErisim.KosulEkle(disSorgu, "AND", "(" + icSorgu + ")");
                        }
                    }
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + disSorgu + ")");
                }
                if (p.dogumTarihi.ToString() != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.DOGUMTARIHI], "=", p.dogumTarihi));

                if (zorunluEmeklilikTarihi != null)
                {
                    string icKosul = $"(DOGUMTARIHI IS NULL OR DATEDIFF(year, DOGUMTARIHI, {VeriErisim.DegerYap(zorunluEmeklilikTarihi)})>= case when IKYPERSONELKADRO.PERSONELTUR IN (17, 37, 18, 36, 81, 82, 83, 100) then 65 else 70 end)";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }

                if ((int)p.cinsiyet > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CINSIYET], "=", (int)p.cinsiyet));

                if (p.nufus.pasaportNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTNO], "=", p.nufus.pasaportNo));
                if (p.nufus.pasaportVerilisTarihi.ToString() != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERILISTARIHI], "=", p.nufus.pasaportVerilisTarihi));
                if (p.nufus.pasaportVerildigiYer != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERILDIGIYER], "=", p.nufus.pasaportVerildigiYer));
                if (p.nufus.pasaportVerenMakam != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERENMAKAM], "=", p.nufus.pasaportVerenMakam));
                if (p.nufus.pasaportSuresi > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTSURESI], "=", p.nufus.pasaportSuresi));


                if (p.tcKimlikNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

                if (p.sakatlikDurumu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAKATLIKDURUMU], "=", p.sakatlikDurumu));

                if (p.kadro.muhasebeKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.MuhasebeKodu], "=", p.kadro.muhasebeKodu));

                if (!string.IsNullOrWhiteSpace(p.kadro.muhasebeBirimi) && p.kadro.muhasebeBirimi != "000")
                {
                    string tut = VeriErisim.OR_KosulOl(TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.MuhasebeBirimi], p.kadro.muhasebeBirimi, true, false, 2);
                    kosul = VeriErisim.KosulEkle(kosul, "AND", tut);
                }

                if (!string.IsNullOrWhiteSpace(kopruKod))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "OB.KOPRUKOD LIKE " + VeriErisim.DegerYap(kopruKod + "%"));
                }
                if (p.kadro.unvan > 0)
                {
                    if (p.kadro.vekilUnvanKod > 0)
                    {
                        string icSorgu = "";
                        icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD], "=", p.kadro.unvan));
                        icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.GorunenUnvanKodu], "=", p.kadro.vekilUnvanKod));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icSorgu + ")");
                    }
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD], "=", p.kadro.unvan));
                }

                if (p.kadro.kadroDerece > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.KADRODERECE], "=", p.kadro.kadroDerece));

                kosul += PersonelTipKosul(BaglacDondur(kosul), TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.PERSONELTUR], p, true);

                if (p.personelSiteKod > 0 || p.personelSiteKod == -1)
                {
                    if (p.personelSiteKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("s", "KOD", "=", p.personelSiteKod));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "l.BitTarihi is null");
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            return sql;
        }

        private string PersonelTipKosul(string baglac, string alanAd, Personel p, bool tipGrupOlsun)
        {
            //TABPersonelKadro.TABLOAD
            if (p.personelTip > 0)
                return "AND" + VeriErisim.KosulOl("", alanAd, "=", p.personelTip);
            else if (!string.IsNullOrWhiteSpace(p.personelTipleri))
                return TABRapor.IN_KosulOl(baglac, alanAd, p.personelTipleri, false);
            else if (p.tipGruplar != null && p.tipGruplar.Length > 0 && tipGrupOlsun)
                return TABRapor.IN_KosulOl(baglac, TABIKYPERSONELTIPI.TABLOAD + "." + TABIKYPERSONELTIPI.ALANLAR[(int)TABIKYPERSONELTIPI.ENUMAlanlar.GRUPAD], p.tipGruplar, true);
            return "";
        }

        private static string BaglacDondur(string kosul)
        {
            if (!string.IsNullOrEmpty(kosul))
                return " and ";
            else
                return " ";
        }

        private string VirgulleBirlestir(TNSCollection liste)
        {
            string retVal = "";
            foreach (string item in liste)
            {
                if (retVal != "") retVal += ",";
                retVal += VeriErisim.DegerYap(item);
            }
            return retVal;
        }

        //public IDataReader PersonelDetayliBilgiGetir(int kod, string kimlikNo, int kadroDurumu, int eskiKadroKod)
        //{
        //    //En son kadro tarihli olanı getirsin
        //    string sql = "SELECT top 1 IKYPERSONEL.TCKIMLIKNO, IKYPERSONEL.KURUMSICILNO, IKYPERSONEL.AD, IKYPERSONEL.SOYAD,IKYPERSONEL.DOGUMTARIHI, IKYPERSONELNUFUS.DOGUMYERI, "
        //             + " SOZLUK1.KAYITAD, SOZLUK2.KAYITAD, SOZLUK3.KAYITAD, SOZLUK4.KAYITAD, SOZLUK5.KAYITAD, IKYPERSONELOGRENIM.OKULADI, IKYPERSONEL.KOD, IKYUNVAN.SINIFKOD, "
        //             + " IKYPERSONEL.PERSONELTUR, IKYPERSONELKADRO.* "
        //             + " FROM IKYPERSONEL "
        //             + " INNER JOIN IKYPERSONELNUFUS ON IKYPERSONELNUFUS.KOD = IKYPERSONEL.KOD "
        //             + " LEFT OUTER JOIN IKYPERSONELASKERLIK ON IKYPERSONEL.KOD = IKYPERSONELASKERLIK.KOD "
        //             + " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK1 ON SOZLUK1.KOD = IKYPERSONELASKERLIK.ASKERLIKDURUMU "
        //             + " LEFT OUTER JOIN IKYPERSONELOGRENIM ON IKYPERSONELOGRENIM.PKOD = IKYPERSONEL.KOD AND GECERLI = '1' "
        //             + " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK2 ON SOZLUK2.KOD = IKYPERSONELOGRENIM.OGRENIMKOD "
        //             + " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK3 ON SOZLUK3.KOD = IKYPERSONELOGRENIM.UNIVERSITEKOD "
        //             + " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK4 ON SOZLUK4.KOD = IKYPERSONELOGRENIM.FAKULTEKOD "
        //             + " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK5 ON SOZLUK5.KOD = IKYPERSONELOGRENIM.BOLUMKOD "
        //             + " LEFT OUTER JOIN IKYPERSONELKADRO ON IKYPERSONELKADRO.PKOD = IKYPERSONEL.KOD ";

        //    if (eskiKadroKod > 0)
        //        sql += " AND IKYPERSONELKADRO.KOD = '" + eskiKadroKod + "'";
        //    else
        //        sql += " AND " + TABGenel.KadroDurumuAktifler("IKYPERSONELKADRO.KADRODURUMU");

        //    sql += " LEFT OUTER JOIN IKYUNVAN ON IKYUNVAN.KOD = IKYPERSONELKADRO.UNVANKOD";

        //    if (kod > 0)
        //        sql += " WHERE IKYPERSONEL.KOD = '" + kod + "'";
        //    else
        //        sql += " WHERE IKYPERSONEL.TCKIMLIKNO = '" + kimlikNo + "'";

        //    sql += " ORDER BY IKYPERSONELKADRO.KADROTARIH DESC";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader ListeleIletisim(int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIletisim, ALANLARILETISIM) + ", AD + ' ' + SOYAD";
            sql += " FROM " + TABLOADIletisim;
            sql += " inner join " + TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADIletisim + "." + ALANLARILETISIM[(int)ENUMAlanlarIletisim.KOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIletisim, ALANLARILETISIM[(int)ENUMAlanlarIletisim.KOD], "=", kod));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleNufus(int kod)
        {
            string sql = "select " + VeriErisim.TumAlanlar(TABLOADNufus, ALANLARNUFUS) + ", SOZLUK.KAYITAD medeniHal, C.AD il, C2.Ad ilce";
            sql += " FROM " + TABLOADNufus;
            sql += " LEFT OUTER JOIN IKYSOZLUK AS SOZLUK ON MEDENIHALI = SOZLUK.KOD";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C ON C.KOD = " + TABLOADNufus + "." + ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILKOD];
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C2 ON C2.KOD = " + TABLOADNufus + "." + ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILCEKOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADNufus, ALANLARNUFUS[(int)ENUMAlanlarNUFUS.KOD], "=", kod));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListelePersonelRapor(Personel p)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT p.TCKIMLIKNO, p.AD, p.SOYAD, pu.AD, p.CINSIYET, p.DOGUMTARIHI, ppo1.OKULADI, po1.TIP, ppo1.BITIRMETARIHI, pd.AD, ppd.PUAN, ppd.DUZEY, ppd.DONEMYIL, ppd.DONEMAY, ppd.BELGETUR FROM PERPERSONEL p LEFT OUTER JOIN IKYUNVAN pu ON p.UNVANKODU = pu.KOD LEFT OUTER JOIN PERPERSONELOGRENIM ppo1 ON p.TCKIMLIKNO = ppo1.TCKIMLIKNO LEFT OUTER JOIN IKYPERSONELDIL ppd ON p.TCKIMLIKNO = ppd.TCKIMLIKNO LEFT OUTER JOIN PERDIL pd ON ppd.DILKODU = pd.KOD LEFT OUTER JOIN PEROGRENIM po1 ON ppo1.OGRENIMKOD = po1.KOD";

            //if (p.muhasebeKodu > 0)
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
            //    if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
            //    {
            //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
            //        kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
            //    }
            //    else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
            //    {
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
            //    }
            //}
            //else if (p.organizasyonKodu > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "case when pu.SIRANO IS NULL then 999999999 when pu.SIRANO=0 then 999999998 else pu.SIRANO end, p." + ALANLAR[(int)ENUMAlanlar.AD] + ", p." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListeleUnvanKadro(Personel p)
        {
            string sql = "";

            sql = "SELECT IKYUNVAN.AD, CINSIYET, COUNT(CINSIYET) FROM " + TABLOAD + " LEFT OUTER JOIN IKYUNVAN ON " + TABLOAD + ".UNVANKODU = IKYUNVAN.KOD ";

            //if (p.organizasyonKodu > 0)
            //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            //sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + ", IKYUNVAN.AD, " + ALANLAR[(int)ENUMAlanlar.CINSIYET];
            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.UNVANKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMalBildirimi(PersonelMalBildirimi p, bool detayli)
        {
            string kimlikListesi;
            string tasinmazListesi;
            string kooperatifListesi;
            string tasitListesi;
            string digerTasinirListesi;
            string bankaMenkulListesi;
            string altinMucevherListesi;
            string borcAlacakListesi;
            string digerServetListesi;
            if (detayli)
            {
                kimlikListesi = TABIKYMALBILDIRIMI.TABLOADALTINMUCEVHER;
                tasinmazListesi = TABIKYMALBILDIRIMI.ListeleTasinmaz($"{TABLOADMALBILDIRIMI}.KOD"); ;
                kooperatifListesi = TABIKYMALBILDIRIMI.ListeleKooperatif($"{TABLOADMALBILDIRIMI}.KOD"); ;
                tasitListesi = TABIKYMALBILDIRIMI.ListeleTasitSql($"{TABLOADMALBILDIRIMI}.KOD");
                digerTasinirListesi = "";
                bankaMenkulListesi = "";
                altinMucevherListesi = "";
                borcAlacakListesi = "";
                digerServetListesi = "";
            }
            else
            {
                kimlikListesi = "NULL";
                tasinmazListesi = "NULL";
                kooperatifListesi = "NULL";
                tasitListesi = "NULL";
                digerTasinirListesi = "NULL";
                bankaMenkulListesi = "NULL";
                altinMucevherListesi = "NULL";
                borcAlacakListesi = "NULL";
                digerServetListesi = "NULL";
            }

            string sql = $"SELECT {VeriErisim.TumAlanlar(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI)}, {kimlikListesi}, {tasinmazListesi}, {kooperatifListesi}, {tasitListesi}, {digerTasinirListesi}, {bankaMenkulListesi}, {altinMucevherListesi}, {borcAlacakListesi}, {digerServetListesi} FROM {TABLOADMALBILDIRIMI}";

            string kosul = "";
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.KOD], "=", p.kod));
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.PKOD], "=", p.personelKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TARIH];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleEgitimRapor(Personel p, PersonelKurs pk)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", "
                + "IKYUNVAN.AD, "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI] + ", "
                + "PERKURS.AD, "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.BASLAMATARIHI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.BITIRMETARIHI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DERSSAATI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.ACIKLAMA] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DONEM] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.YIL] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DOSYANO]
                + " FROM " + TABLOAD
                + " INNER JOIN " + TABLOADKURS + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO];
            //+ " LEFT OUTER JOIN IKYUNVAN ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = IKYUNVAN.KOD"
            //+ " LEFT OUTER JOIN PERKURS ON " + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU] + " = PERKURS.KOD";

            //if (p.muhasebeKodu > 0)
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
            //    if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
            //    {
            //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
            //        kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
            //    }
            //    else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
            //    {
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
            //    }
            //}
            //else if (p.organizasyonKodu > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (!string.IsNullOrEmpty(pk.kursAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], "LIKE", pk.kursAdi + "%"));
            if (pk.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.YIL], "=", pk.yil));
            if (!string.IsNullOrEmpty(pk.donem))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], "=", pk.donem));
            if (!string.IsNullOrEmpty(pk.aldigiYer))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER], "LIKE", pk.aldigiYer + "%"));
            if (pk.egitimYeri > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], "LIKE", pk.egitimYeri + "%"));



            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "" + TABLOADKURS + ".BASLAMATARIHI, PERKURS.AD, IKYUNVAN.KOD, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + " " + VeriErisim.IslecBul("+") + " " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListeleAdres(int kod)
        {
            string sql = "SELECT ";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADRES] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEBLIGATADRES] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MULKDURUM] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TASINMATARIHI] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISTELEFON] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EVTELEFON] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.CEPTELEFON] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FAKS] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EPOSTA] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.URL] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MOBILIMZATELEFON] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACILKISI] + ",";
            //sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACILKISINUMARA];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader ListeleIzinRapor(Personel p, PersonelIzin pk)
        //{
        //    string sql = "";
        //    string kosul = "";
        //    sql = "SELECT "
        //        + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", "
        //        + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", "
        //        + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", "
        //        + "IKYUNVAN.AD, "
        //        + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMSICILNO] + ", "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI] + ", "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI] + ", "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ", "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + ", "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.ACIKLAMA]
        //        + " FROM " + TABLOAD
        //        + " INNER JOIN " + TABLOADIZIN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO];
        //    //+ " LEFT OUTER JOIN IKYUNVAN ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = IKYUNVAN.KOD";

        //    //if (p.muhasebeKodu > 0)
        //    //{
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
        //    //    if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
        //    //    {
        //    //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
        //    //        kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
        //    //    }
        //    //    else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
        //    //    {
        //    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
        //    //    }
        //    //}
        //    //else if (p.organizasyonKodu > 0)
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

        //    if (!string.IsNullOrEmpty(pk.tcKimlikNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", pk.tcKimlikNo));
        //    if (pk.baslangicTarihi.Yil > 1)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", pk.baslangicTarihi));
        //    if (pk.tip > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", pk.tip));
        //    if (!String.IsNullOrEmpty(pk.donem))
        //    {
        //        if (pk.donem == "I. 6 Aylık") //1.6ay
        //        {
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + pk.baslangicTarihi.Yil)));
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("30.06." + pk.baslangicTarihi.Yil)));
        //        }
        //        else if (pk.donem == "II. 6 Aylık") //2.6ay
        //        {
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.07." + pk.baslangicTarihi.Yil)));
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + pk.baslangicTarihi.Yil)));
        //        }

        //    }
        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    sql += " ORDER BY " + "" + " IKYUNVAN.KOD, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " , " + TABLOADIZIN + ".BASLANGICTARIHI";
        //    IDataReader r = null;
        //    if (kosul != "")
        //        r = veriErisim.DataReaderDondur(sql);

        //    return r;
        //}

        //public IDataReader ListeleIzinDurum(PersonelIzinDurum pk)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT "
        //        + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + ", "
        //        + "IKYIZINTUR.AD" + ", "
        //        + "IKYIZINTUR.SURE" + ", "
        //        + "SUM(" + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ")"
        //        + " FROM " + TABLOADIZIN
        //        + " INNER JOIN IKYIZINTUR ON " + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + " = IKYIZINTUR.KOD ";

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", pk.tcKimlikNo));
        //    if (pk.listelemeYili > 0)
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", pk.listelemeYili.ToString("d4") + "-01-01 00:00:00"));
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<", (pk.listelemeYili + 1).ToString("d4") + "-01-01 00:00:00"));
        //    }

        //    sql += " WHERE " + kosul;
        //    sql += " GROUP BY " + TABLOADIZIN + ".TIP" + ", IKYIZINTUR.AD" + ", IKYIZINTUR.SURE";
        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader ListeleIzin(PersonelIzin p)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIZIN, ALANLARIZIN) + " FROM " + TABLOADIZIN;

        //    if (!string.IsNullOrEmpty(p.tcKimlikNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    if (p.sirano > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));
        //    if (!p.baslangicTarihi.isNull)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", p.baslangicTarihi));
        //    if (!p.bitisTarihi.isNull)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", p.bitisTarihi));
        //    if (!string.IsNullOrEmpty(p.prEvrakNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], "=", p.prEvrakNo));
        //    if (!string.IsNullOrEmpty(p.evrEvrakNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], "=", p.evrEvrakNo));

        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    sql += " ORDER BY " + ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI] + " DESC";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader ListeleIzinDevir(PersonelIzinDevir p)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIZINDEVIR, ALANLARIZINDEVIR) + " FROM " + TABLOADIZINDEVIR;

        //    if (!string.IsNullOrEmpty(p.tcKimlikNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    if (p.yil > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public int ListeleKullanilanIzin(string tcKimlikNo, int yil, int izinSiraNo)
        //{
        //    string sql = "";

        //    sql = "SELECT SUM(" + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ") FROM " + TABLOADIZIN;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + yil)));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + yil)));
        //    if (izinSiraNo > 0)//Verilen sirano ile kayıtlı izin hariç kullanılan izin süresi hesaplamak için. Önce den kayıt edilen izin düzeltilerek kayot edilemek istendiğinde süre kontrolü yapmak için
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", izinSiraNo));

        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", 1));//1->yıllık izin
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.BELIRSIZ));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.MAZERET));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.REFAKATLI));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.SIHHI));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.UCRETLI));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.UCRETSIZ));

        //    return veriErisim.TekKolonSQLInt(sql);
        //}

        public IDataReader ListeleTarihce(PersonelTarihce p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;
            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.PKOD], "=", p.personelKod));
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD], "=", p.kod));
            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Personel p, int aktif = 0)
        {
            string sql;
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(veriErisim, TABLOAD);
                if (!string.IsNullOrWhiteSpace(veriErisim.hata)) return new Sonuc(veriErisim.hata);
            }
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap(p.sicilNo) + ",";
            if (p.sakatlikDurumu > 0)
                sql += VeriErisim.DegerYap(p.sakatlikDurumu) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.emekliSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.sskSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.bagkurSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.ilkIseBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.asaletTarihi) + ",";
            sql += VeriErisim.DegerYap(p.kurumaBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.sakatlikDerecesi) + ",";
            sql += VeriErisim.DegerYap(p.terorMagduru) + ",";
            sql += VeriErisim.DegerYap(p.hukumlu) + ",";
            sql += VeriErisim.DegerYap(p.korunmayaMuhtac) + ",";
            if (p.sosyalGuvence > 0)
                sql += VeriErisim.DegerYap(p.sosyalGuvence) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.bankaIbanNo) + ",";
            sql += VeriErisim.DegerYap(p.vergiDairesi) + ",";
            sql += VeriErisim.DegerYap(aktif);
            sql += "," + VeriErisim.DegerYap(p.geldigiKurum);
            sql += "," + VeriErisim.DegerYap(p.arsivNo);
            sql += "," + VeriErisim.DegerYap(p.geldigiKurumSicili);
            sql += "," + VeriErisim.DegerYap(p.rumuz);
            sql += "," + VeriErisim.DegerYap(p.titr);
            sql += ")";

            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        public Sonuc KaydetNufus(PersonelNufus p, bool ekle)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            if (!ekle)
            {
                sql = "UPDATE " + TABLOADNufus + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.DOGUMYERI], p.dogumYeri) + ",";
                if (p.medeniHali > 0)
                    sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.MEDENIHALI], p.medeniHali) + ",";
                else
                    sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.MEDENIHALI], (string)null) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.ANAAD], p.anaAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.BABAAD], p.babaAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.KANGRUBU], p.kanGrubu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CUZDANSERINO], p.cuzdanSeriNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CUZDANVERILISNEDENI], p.cuzdanVerilisNedeni) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CUZDANVERILISTARIHI], HataliTarihDuzelt(p.cuzdanVerilisTarihi)) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CUZDANVERILDIGIYER], p.cuzdanVerildigiYer) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CUZDANKAYITNO], p.cuzdanKayitNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILKOD], p.nufusIlKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSILCEKOD], p.nufusIlceKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSMAHALLEKOY], p.nufusMahalleKoy) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSCILTNO], p.nufusCiltNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSSIRANO], p.nufusSiraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSAILESIRANO], p.nufusAileSiraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.ILKSOYAD], p.ilkSoyad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.DOGUMTARIHI], HataliTarihDuzelt(p.dogumTarihi)) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.NUFUSDINI], p.dini) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.CINSIYET], (int)p.cinsiyeti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTNO], p.pasaportNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERILISTARIHI], p.pasaportVerilisTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERILDIGIYER], p.pasaportVerildigiYer) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTVERENMAKAM], p.pasaportVerenMakam) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARNUFUS[(int)ENUMAlanlarNUFUS.PASAPORTSURESI], p.pasaportSuresi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            if (!sonuc.islemSonuc || ekle)
            {
                sql = "INSERT INTO " + TABLOADNufus + " (" + VeriErisim.TumAlanlar("", ALANLARNUFUS) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.dogumYeri) + ",";
                if (p.medeniHali > 0)
                    sql += VeriErisim.DegerYap(p.medeniHali) + ",";
                else
                    sql += VeriErisim.DegerYap((string)null) + ",";
                sql += VeriErisim.DegerYap(p.anaAd) + ",";
                sql += VeriErisim.DegerYap(p.babaAd) + ",";
                sql += VeriErisim.DegerYap(p.kanGrubu) + ",";
                sql += VeriErisim.DegerYap(p.cuzdanSeriNo) + ",";
                sql += VeriErisim.DegerYap(p.cuzdanVerilisNedeni) + ",";
                sql += VeriErisim.DegerYap(HataliTarihDuzelt(p.cuzdanVerilisTarihi)) + ",";
                sql += VeriErisim.DegerYap(p.cuzdanVerildigiYer) + ",";
                sql += VeriErisim.DegerYap(p.cuzdanKayitNo) + ",";
                sql += VeriErisim.DegerYap(p.nufusIlKod) + ",";
                sql += VeriErisim.DegerYap(p.nufusIlceKod) + ",";
                sql += VeriErisim.DegerYap(p.nufusMahalleKoy) + ",";
                sql += VeriErisim.DegerYap(p.nufusCiltNo) + ",";
                sql += VeriErisim.DegerYap(p.nufusSiraNo) + ",";
                sql += VeriErisim.DegerYap(p.nufusAileSiraNo) + ",";
                sql += VeriErisim.DegerYap(p.ilkSoyad) + ",";
                sql += VeriErisim.DegerYap(HataliTarihDuzelt(p.dogumTarihi)) + ",";
                sql += VeriErisim.DegerYap(p.dini) + ",";
                sql += VeriErisim.DegerYap((int)p.cinsiyeti) + ",";
                sql += VeriErisim.DegerYap(p.pasaportNo) + ",";
                sql += VeriErisim.DegerYap(p.pasaportVerilisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.pasaportVerildigiYer) + ",";
                sql += VeriErisim.DegerYap(p.pasaportVerenMakam) + ",";
                sql += VeriErisim.DegerYap(p.pasaportSuresi) + ")";
            }

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        private TNSDateTime HataliTarihDuzelt(TNSDateTime gelenTarih)
        {
            if (gelenTarih == null || gelenTarih.isNull) return null;
            if (gelenTarih.Yil < 1753 || gelenTarih.Yil > 9999) return null;
            return gelenTarih;
        }

        public Sonuc KaydetIletisim(PersonelIletisim p, bool ekle)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            if (!ekle)
            {
                sql = "UPDATE " + TABLOADIletisim + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.ISTELEFON], p.isTelefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.CEPTELEFON], p.cepTelefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.FAKS], p.faks) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.EPOSTA], p.eposta) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.EPOSTA2], p.eposta2) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.ADRES], p.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.EVTELEFON], p.evTelefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.URL], p.url) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.MOBILIMZATELEFON], p.mobilImzaTelefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.CEPTELEFON2], p.cepTelefon2) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.TARABYATEL], p.tarabyaTel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.LOJMANTEL], p.lojmanTel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.TELSIZ], p.telsiz) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.GORUNTULU], p.goruntulu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.REHBERDEGOZUKSUN], p.rehberdeGozuksun);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILETISIM[(int)ENUMAlanlarIletisim.KOD], "=", p.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            if (ekle || !sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOADIletisim + " (" + VeriErisim.TumAlanlar("", ALANLARILETISIM) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.isTelefon) + ",";
                sql += VeriErisim.DegerYap(p.cepTelefon) + ",";
                sql += VeriErisim.DegerYap(p.faks) + ",";
                sql += VeriErisim.DegerYap(p.eposta) + ",";
                sql += VeriErisim.DegerYap(p.adres) + ",";
                sql += VeriErisim.DegerYap(p.evTelefon) + ",";
                sql += VeriErisim.DegerYap(p.url) + ",";
                sql += VeriErisim.DegerYap(p.mobilImzaTelefon) + ",";
                sql += VeriErisim.DegerYap(p.cepTelefon2) + ",";
                sql += VeriErisim.DegerYap(p.tarabyaTel) + ",";
                sql += VeriErisim.DegerYap(p.lojmanTel) + ",";
                sql += VeriErisim.DegerYap(p.telsiz) + ",";
                sql += VeriErisim.DegerYap(p.goruntulu) + ",";
                sql += VeriErisim.DegerYap(p.rehberdeGozuksun) + ",";
                sql += VeriErisim.DegerYap(p.eposta2) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        public bool PersonelVarMi(string tcKimlikNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(TCKIMLIKNO) FROM " + TABLOAD;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public Sonuc KaydetBirim(Personel p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], p.muhasebeKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], p.birimKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], p.kurum.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";
            //string sayacKosul = ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            //p.sirano = TABGenel.SayacDegerAl(veriErisim, TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADMALBILDIRIMI + " (" + VeriErisim.TumAlanlar("", ALANLARMALBILDIRIMI) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        //public Sonuc KaydetKurs(PersonelKurs p)
        //{
        //    string sql = "";
        //    p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO] + "=" + p.tcKimlikNo);

        //    sql = "INSERT INTO " + TABLOADKURS + " (" + VeriErisim.TumAlanlar("", ALANLARKURS) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
        //    sql += VeriErisim.DegerYap(p.siraNo) + ",";
        //    sql += VeriErisim.DegerYap(p.kursKod) + ",";
        //    sql += VeriErisim.DegerYap(p.aldigiYer) + ",";
        //    sql += VeriErisim.DegerYap(p.baslamaTarihi) + ",";
        //    sql += VeriErisim.DegerYap(p.bitirmeTarihi) + ",";
        //    sql += VeriErisim.DegerYap(p.derece) + ",";
        //    sql += VeriErisim.DegerYap(p.kursAdi) + ",";
        //    sql += VeriErisim.DegerYap(p.dersSaati) + ",";
        //    sql += VeriErisim.DegerYap(p.egitimYeri) + ",";
        //    sql += VeriErisim.DegerYap(p.aciklama) + ",";
        //    sql += VeriErisim.DegerYap(p.donem) + ",";
        //    sql += VeriErisim.DegerYap(p.yil) + ",";
        //    sql += VeriErisim.DegerYap(p.dosyaNo) + ",";
        //    sql += VeriErisim.DegerYap(p.gun) + ")";

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KaydetIzin(PersonelIzin p)
        //{
        //    string sql = "";

        //    string sayacKosul = ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
        //    p.sirano = TABGenel.SayacDegerAl(veriErisim, TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], sayacKosul);

        //    sql = "INSERT INTO " + TABLOADIZIN + " (" + VeriErisim.TumAlanlar("", ALANLARIZIN) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
        //    sql += VeriErisim.DegerYap(p.sirano) + ",";
        //    sql += VeriErisim.DegerYap(p.tip) + ",";
        //    sql += VeriErisim.DegerYap(p.aciklama) + ",";
        //    sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
        //    sql += VeriErisim.DegerYap(p.bitisTarihi) + ",";
        //    sql += VeriErisim.DegerYap(p.telefon) + ",";
        //    sql += VeriErisim.DegerYap(p.kullanilanGun) + ",";
        //    sql += VeriErisim.DegerYap(p.izinNotu) + ",";
        //    sql += VeriErisim.DegerYap(p.onay) + ",";
        //    sql += VeriErisim.DegerYap(p.evrEvrakNo) + ",";
        //    sql += VeriErisim.DegerYap(p.prEvrakNo) + ",";
        //    sql += VeriErisim.DegerYap(p.alan1) + ",";
        //    sql += VeriErisim.DegerYap(p.not) + ")";


        //    return KomutCalistir(sql, 1);
        //}

        //public int KaydetIzinUygunMu(PersonelIzin p)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT COUNT(*) FROM " + TABLOADIZIN;

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], "<=", p.bitisTarihi));
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], ">=", p.baslangicTarihi));
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", p.sirano));

        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    return veriErisim.TekKolonSQLInt(sql);
        //}

        //public Sonuc KaydetOnayIzin(PersonelIzin p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADIZIN + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ONAY], p.onay);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KaydetIzinAlan(PersonelIzin p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADIZIN + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKALAN1], p.alan1) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKNOT1], p.not);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KaydetIzinEvrakBilgi(PersonelIzin p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADIZIN + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], p.evrEvrakNo) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], p.prEvrakNo);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KaydetIzinDevir(PersonelIzinDevir p)
        //{
        //    string sql = "";
        //    sql = "INSERT INTO " + TABLOADIZINDEVIR + " (" + VeriErisim.TumAlanlar("", ALANLARIZINDEVIR) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
        //    sql += VeriErisim.DegerYap(p.yil) + ",";
        //    sql += VeriErisim.DegerYap(p.devreden) + ",";
        //    sql += VeriErisim.DegerYap(p.yilici) + ")";

        //    return KomutCalistir(sql, 1);
        //}

        public Sonuc KaydetTarihce(PersonelTarihce p)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            if (p.personelKod == 0)
                sql += VeriErisim.DegerYap((string)null) + ",";
            else
                sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.kullanici) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.degisiklik) + ",";
            sql += VeriErisim.DegerYap(p.kullaniciAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "INSERT INTO " + TABLOADSOZLESMELIBORDROTANIM + " (" + VeriErisim.TumAlanlar("", ALANLARSOZLESMELIBORDROTANIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.ay) + ",";
            sql += VeriErisim.DegerYap(p.kumulatifVergiMatrahi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Personel p)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], p.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMSICILNO], p.sicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYAD], p.soyad) + ",";

            if (p.sakatlikDurumu > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKATLIKDURUMU], p.sakatlikDurumu) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKATLIKDURUMU], (string)null) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLISICILNO], p.emekliSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SSKSICILNO], p.sskSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BAGKURSICILNO], p.bagkurSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKISEBASLAMATARIHI], p.ilkIseBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASALETTARIHI], p.asaletTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMABASLAMATARIHI], p.kurumaBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKATLIKDERECESI], p.sakatlikDerecesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERORMAGDURU], p.terorMagduru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HUKUMLU], p.hukumlu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KORUNMAYAMUHTAC], p.korunmayaMuhtac) + ",";
            if (p.sosyalGuvence > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOSYALGUVENCE], p.sosyalGuvence) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOSYALGUVENCE], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKAIBANNO], p.bankaIbanNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRESI], p.vergiDairesi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELDIGIKURUM], p.geldigiKurum);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELDIGIKURUMSICILI], p.geldigiKurumSicili);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARSIVNO], p.arsivNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RUMUZ], p.rumuz);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TITR], p.titr);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAktiflik(int kod, int aktiflik)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIFMI], aktiflik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADMALBILDIRIMI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TARIH], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.ACIKLAMA], p.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        //public Sonuc GuncelleKurs(PersonelKurs p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADKURS + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU], p.kursKod) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER], p.aldigiYer) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.BASLAMATARIHI], p.baslamaTarihi) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.BITIRMETARIHI], p.bitirmeTarihi) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DERECE], p.derece) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], p.kursAdi) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DERSSAATI], p.dersSaati) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], p.egitimYeri) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.ACIKLAMA], p.aciklama) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], p.donem) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.YIL], p.yil) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DOSYANO], p.dosyaNo) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.GUN], p.gun);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], "=", p.siraNo));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc GuncelleIzin(PersonelIzin p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADIZIN + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], p.tcKimlikNo) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], p.tip) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ACIKLAMA], p.aciklama) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], p.baslangicTarihi) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], p.bitisTarihi) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TELEFON], p.telefon) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN], p.kullanilanGun) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.IZINNOTU], p.izinNotu) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ONAY], p.onay) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], p.evrEvrakNo) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], p.prEvrakNo) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKALAN1], p.alan1) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKNOT1], p.not);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc GuncelleIzinDevir(PersonelIzinDevir p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADIZINDEVIR + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.DEVREDEN], p.devreden) + ",";
        //    sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YILICI], p.yilici);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

        //    return base.KomutCalistir(sql, 1);
        //}

        public Sonuc GuncelleSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "UPDATE " + TABLOADSOZLESMELIBORDROTANIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.KUMULATIFVERGIMATRAHI], p.kumulatifVergiMatrahi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.YIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.AY], p.ay);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Personel p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSOZLESMELIBORDROTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        //public Sonuc SilKurs(PersonelKurs p)
        //{
        //    string sql = "";

        //    sql = "DELETE FROM " + TABLOADKURS;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], "=", p.siraNo));

        //    return KomutCalistir(sql, 0);
        //}

        public Sonuc SilMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "DELETE FROM " + TABLOADMALBILDIRIMI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.KOD], "=", p.kod));
            return KomutCalistir(sql, 1);
        }

        //public void GuncelleOgrenimGecerli(PersonelOgrenim p)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOADOGRENIM + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], 0);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    KomutCalistir(sql, 0);


        //    sql = "UPDATE " + TABLOADOGRENIM + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], 1);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], "=", p.siraNo));
        //    KomutCalistir(sql, 1);


        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMKODU], p.ogrenimKod);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    KomutCalistir(sql, 1);
        //}

        //public void GuncelleDilBordroyaEsas(PersonelDil p)
        //{
        //    //string sql = "";

        //    //sql = "UPDATE " + TABLOADDIL + " SET ";
        //    //sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BORDROYAESAS], 0);
        //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    //KomutCalistir(sql, 0);


        //    //sql = "UPDATE " + TABLOADDIL + " SET ";
        //    //sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BORDROYAESAS], 1);
        //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], "=", p.siraNo));
        //    //KomutCalistir(sql, 1);
        //}

        //public IDataReader MemuriyeteUygunKadroBilgileriniAl(string tcKimlikNo, TNSDateTime ayrilisTarihi, int tip)
        //{
        //    string sql = "";
        //    if (tip == 1)
        //    {
        //        sql = "SELECT TOP (1) KADRODERECE, KADROTARIH FROM IKYPERSONELKADRODERECE";
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KADROTARIH], "<=", ayrilisTarihi));
        //        sql += " ORDER BY KADROTARIH DESC";
        //    }
        //    else if (tip == 2)
        //    {
        //        sql = "SELECT TOP (1) ODEMEYEESASDERECE, ODEMEYEESASKADEME, ODEMEYEESASTARIH, ODEMEYEESASGOSTERGE, ODEMEYEESASEKGOSTERGE FROM IKYPERSONELKADRODERECE";
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASTARIH], "<=", ayrilisTarihi));
        //        sql += " ORDER BY ODEMEYEESASTARIH DESC";
        //    }
        //    else if (tip == 3)
        //    {
        //        sql = "SELECT TOP (1) KAZANILMISHAKDERECE, KAZANILMISHAKKADEME, KAZANILMISHAKTARIH, KAZANILMISGOSTERGE, KAZANILMISEKGOSTERGE FROM IKYPERSONELKADRODERECE";
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISHAKTARIH], "<=", ayrilisTarihi));
        //        sql += " ORDER BY KAZANILMISHAKTARIH DESC";
        //    }
        //    else if (tip == 4)
        //    {
        //        sql = "SELECT TOP (1) EMEKLIYEESASDERECE, EMEKLIYEESASKADEME, EMEKLIYEESASTARIH, EMEKLIYEESASGOSTERGE, EMEKLIYEESASEKGOSTERGE FROM IKYPERSONELKADRODERECE";
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASTARIH], "<=", ayrilisTarihi));
        //        sql += " ORDER BY EMEKLIYEESASTARIH DESC";
        //    }
        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public Sonuc KadroBilgisiPersonelGuncelle(Personel p)
        //{
        //    string sql = "UPDATE " + TABLOAD + " SET ";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROORGANIZASYONKOD], p.kadroOrganizasyonKodu) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROMUHASEBEKODU], p.kadroMuhasebeKodu) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROBIRIMKODU], p.kadroBirimKodu) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROKURUM], p.kadroKurum) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADRODERECE], p.kadroDerece) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROTARIH], p.kadroTarih) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASDERECE], p.odemeEsasDerece) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASKADEME], p.odemeEsasKademe) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASGOSTERGE], p.odemeEsasGosterge) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASTARIH], p.odemeEsasTarih) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASDERECE], p.emekliEsasDerece) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASKADEME], p.emekliEsasKademe) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASGOSTERGE], p.emekliEsasGosterge) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASTARIH], p.emekliEsasTarih) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], p.unvanKodu) + ",";
        //    //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFKODU], p.sinifKodu);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

        //    return KomutCalistir(sql, 1);
        //}

        //public IDataReader KadroBilgisiAl(string tcKimlikNo, int tip)
        //{
        //    string sql = "";
        //    if (tip == 1)//kadro bilgisi
        //        sql = "SELECT ORGANIZASYONKOD, MUHASEBEKODU, BIRIMKODU, KURUM, UNVANKOD, SINIFKOD, KADRODERECE, KADROTARIH FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND KADROTARIH = (SELECT MAX(KADROTARIH) FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
        //    else if (tip == 2)//Ödeme Esas
        //        sql = "SELECT ODEMEYEESASDERECE, ODEMEYEESASKADEME, ODEMEYEESASGOSTERGE, ODEMEYEESASTARIH FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND ODEMEYEESASTARIH = (SELECT MAX(ODEMEYEESASTARIH) FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
        //    else if (tip == 3)//Kazanılmış hak
        //        sql = "SELECT KAZANILMISHAKDERECE, KAZANILMISHAKKADEME, KAZANILMISHAKTARIH FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND KAZANILMISHAKTARIH = (SELECT MAX(KAZANILMISHAKTARIH) FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
        //    else if (tip == 4)//Emekliye Esas
        //        sql = "SELECT EMEKLIYEESASDERECE, EMEKLIYEESASKADEME, EMEKLIYEESASGOSTERGE, EMEKLIYEESASTARIH FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND EMEKLIYEESASTARIH = (SELECT MAX(EMEKLIYEESASTARIH) FROM IKYPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public bool IzinKayitKontrol(PersonelIzin p)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT SUM(" + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ")"
        //        + " FROM " + TABLOADIZIN
        //        + " INNER JOIN IKYIZINTUR ON " + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + " = IKYIZINTUR.KOD";

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", p.tip));
        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
        //    if (p.sirano > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", p.sirano));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + DateTime.Now.Year.ToString("d4"))));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + DateTime.Now.Year.ToString("d4"))));

        //    sql += " WHERE " + kosul;
        //    sql += " GROUP BY IKYIZINTUR.KOD";
        //    int dahaOnceKullandigiToplamIzin = veriErisim.TekKolonSQLInt(sql);
        //    sql = "SELECT IKYIZINTUR.SURE FROM IKYIZINTUR WHERE IKYIZINTUR.KOD=" + p.tip;
        //    int kullanabilecegiToplamIzin = veriErisim.TekKolonSQLInt(sql);
        //    if (kullanabilecegiToplamIzin == 0)
        //        return true;
        //    if ((kullanabilecegiToplamIzin - dahaOnceKullandigiToplamIzin) >= p.kullanilanGun)
        //        return true;
        //    else
        //        return false;
        //}

        public IDataReader PersonelBulOncekiSonraki(string sicilNo, bool onceki)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD] + " FROM " + TABLOAD;

            string alanAd = ALANLAR[(int)ENUMAlanlar.KURUMSICILNO];
            sql = VeriErisim.KosulEkle(sql, "WHERE", alanAd + "= (SELECT " + MaxMin(onceki) + "(" + alanAd + ") FROM " + TABLOAD + " IC WHERE IC." + alanAd + KucukBuyuk(onceki) + VeriErisim.DegerYap(sicilNo) + ")");

            return veriErisim.DataReaderDondur(sql);
        }

        private string KucukBuyuk(bool onceki)
        {
            if (onceki) return "<";
            else return ">";
        }

        private string MaxMin(bool onceki)
        {
            if (onceki) return "MAX";
            else return "MIN";
        }

        public Sonuc GuncelleKullaniciMail(string tcKimlikNo, string eposta)
        {
            string sql = "UPDATE Kullanici.dbo.KULLANICI SET EMAIL = " + VeriErisim.DegerYap(eposta);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERNIS", "=", tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader IstatistikListeleUnvanTipGrupTip(Personel p, string kopruKod, string kopruKodlar)
        {
            string sql = "SELECT CONVERT(VARCHAR, IKYUNVAN.KOD), IKYUNVAN.AD, IKYPERSONELTIPI.GRUPAD, IKYPERSONELTIPI.GRUPAD, CONVERT(VARCHAR,IKYPERSONELTIPI.SIRA), IKYPERSONELTIPI.AD, COUNT(*), case when IKYUNVAN.SIRANO IS NULL then 999999999 when IKYUNVAN.SIRANO=0 then 999999998 else IKYUNVAN.SIRANO end " + PersonelKosul(p, kopruKod, kopruKodlar);
            sql += " GROUP BY IKYUNVAN.KOD, IKYUNVAN.AD, IKYPERSONELTIPI.GRUPAD, IKYPERSONELTIPI.GRUPAD, IKYPERSONELTIPI.SIRA, IKYPERSONELTIPI.AD, case when IKYUNVAN.SIRANO IS NULL then 999999999 when IKYUNVAN.SIRANO=0 then 999999998 else IKYUNVAN.SIRANO end";
            sql += " ORDER BY case when IKYUNVAN.SIRANO IS NULL then 999999999 when IKYUNVAN.SIRANO=0 then 999999998 else IKYUNVAN.SIRANO end, IKYUNVAN.AD, IKYPERSONELTIPI.GRUPAD, IKYPERSONELTIPI.SIRA, IKYPERSONELTIPI.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IstatistikListeleHarcBirimPTip(Personel p, string kopruKod, string kopruKodlar)
        {
            string sql = "SELECT CONVERT(VARCHAR, MUHBIR.MUHASEBEKOD) + '-' + MUHBIR.BIRIMKOD, MUHBIR.BIRIMAD, CONVERT(VARCHAR,IKYPERSONELTIPI.SIRA), IKYPERSONELTIPI.AD, COUNT(*) " + PersonelKosul(p, kopruKod, kopruKodlar);
            sql += " GROUP BY CONVERT(VARCHAR, MUHBIR.MUHASEBEKOD) + '-' + MUHBIR.BIRIMKOD, MUHBIR.BIRIMAD, CONVERT(VARCHAR,IKYPERSONELTIPI.SIRA), IKYPERSONELTIPI.AD";
            sql += " ORDER BY 1, 3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TCKimlikNoGuncelle(string eskiTC, string yeniTC)
        {
            Sonuc sonuc = new Sonuc(true, "", "TC Test Değişti");

            veriErisim.komutTuru = CommandType.StoredProcedure;
            veriErisim.param.Add(new VeriPara("@eskiTC", DbType.String, eskiTC));
            veriErisim.param.Add(new VeriPara("@yeniTC", DbType.String, yeniTC));

            sonuc = KomutCalistir("TCKIMLIKDEGISTIR", 0);

            int i = veriErisim.ExecuteNonquery("TCKIMLIKDEGISTIR");
            if (i > 0)
                sonuc.islemSonuc = true;

            return sonuc;
        }
    }
}
