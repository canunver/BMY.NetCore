using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır demirbaşlarının özellik bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMSicilNoOzellik
    {
        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgileri varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="sicilNoOzellik">Demirbaşlara ait özellik bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, ObjectArray sicilNoOzellik)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, muhasebe, harcamaBirimi, ambar);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);
            ve.BeginTransaction();

            foreach (SicilNoOzellik s in sicilNoOzellik.objeler)
            {
                sonuc = to.Kaydet(s);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşa ait özellik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşın özellik bilgileri döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik tabSicilO = new TABSicilNoOzellik(ve);
            IDataReader dr = null;

            try
            {
                dr = tabSicilO.Listele(prSicilNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                SicilNoOzellik gelen = new SicilNoOzellik();
                gelen.prSicilno = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.PRSICILNO, 0);

                gelen.markaKod = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.MARKAKOD, 0);
                if (gelen.markaKod > 0)
                    gelen.markaAd = TMMServis.UzayYaziDegerGetir("TASMARKA", gelen.markaKod.ToString(), true, "");

                gelen.modelKod = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.MODELKOD, 0);
                if (gelen.modelKod > 0)
                    gelen.modelAd = TMMServis.UzayYaziDegerGetir("TASMODEL", gelen.markaKod.ToString() + "-" + gelen.modelKod.ToString(), true, "");

                gelen.saseNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.SASENO, string.Empty);
                gelen.motorNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.MOTORNO, string.Empty);
                gelen.plaka = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.PLAKA, string.Empty);
                gelen.ciltNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.CILTNO, string.Empty);
                gelen.dil = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.DIL, string.Empty);
                gelen.yazarAdi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.YAZARADI, string.Empty);
                gelen.adi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.ADI, string.Empty);
                gelen.yayinYeri = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.YAYINYERI, string.Empty);
                gelen.yayinTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.YAYINTARIHI, string.Empty);
                gelen.neredenGeldi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.NEREDENGELDI, string.Empty);
                gelen.boyutlari = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.BOYUTLARI, string.Empty);
                gelen.satirSayisi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.SATIRSAYISI, string.Empty);
                gelen.yaprakSayisi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.YAPRAKSAYISI, string.Empty);
                gelen.sayfaSayisi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.SAYFASAYISI, string.Empty);
                gelen.ciltTuru = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.CILTTURU, string.Empty);
                gelen.cesidi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.CESIDI, string.Empty);
                gelen.yeriKonusu = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.YERIKONUSU, string.Empty);
                gelen.gelisTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.GELISTARIHI, string.Empty);
                gelen.neredeBulundu = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.NEREDEBULUNDU, string.Empty);
                gelen.cagi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.CAGI, string.Empty);
                gelen.durumuMaddesi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.DURUMUMADDESI, string.Empty);
                gelen.onYuz = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.ONYUZ, string.Empty);
                gelen.arkaYuz = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.ARKAYUZ, string.Empty);
                gelen.fotograf = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.FOTOGRAF, string.Empty);
                gelen.puan = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.PUAN, string.Empty);
                gelen.disSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.DISSICILNO, string.Empty);
                gelen.disSicilNo2 = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.DISSICILNO2, string.Empty);
                gelen.bisNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.BISNO, string.Empty);
                gelen.eskiBisNo1 = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.ESKIBISNO1, string.Empty);
                gelen.eskiBisNo2 = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.ESKIBISNO2, string.Empty);
                gelen.rfidEtiketTurKod = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.RFIDETIKETTURKOD, 0);
                gelen.garantiBitisTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.GARANTIBITISTARIHI);
                gelen.giai = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.GIAI, "");
                gelen.ekNo = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.EKNO, "");
                gelen.hesapTarih = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.HESAPTARIHI, "");
                gelen.butceKodu = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.BUTCEKODU, "");
                gelen.blokajKodu = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.BLOKAJKODU, "");
                gelen.geldigiSube = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.GELDIGISUBE, "");
                gelen.amortismanYil = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.AMORTISMANYIL, 0);
                gelen.amortismanBitti = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.AMORTISMANBITTI, 0);
                gelen.bulunduguYer = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.BULUNDUGUYER, "");
                gelen.rfidEtiketTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoOzellik.ENUMAlanlar.RFIDETIKETTARIHI);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgilerini silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="prSicilNo">Demirbaş listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, int[] prSicilNo)
        {
            Sonuc sonuc = new Sonuc();

            if (prSicilNo.Length == 0)
                sonuc.hataStr = TasinirMalResI.TMM074 + "";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);

            ve.BeginTransaction();
            for (int i = 0; i < prSicilNo.Length; i++)
            {
                sonuc = to.Sil(prSicilNo[i]);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc BisKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, string sicilNo, int prSicilNo, SicilNoOzellik[] formListe, bool silmeIslemi = true)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            string hata = "";
            if (muhasebe.Trim() == "")
                hata = TasinirMalResI.TMM013 + "<br>";
            if (harcamaBirimi.Trim() == "")
                hata += TasinirMalResI.TMM014 + "<br>";
            if (sicilNo.Trim() == "" && prSicilNo == 0)
                hata += TasinirMalResI.TMM020 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }


            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, muhasebe, harcamaBirimi, ambar);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);

            if (prSicilNo == 0)
                prSicilNo = to.PrSicilNoGetir(sicilNo);

            if (prSicilNo == 0)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = sicilNo + " sicil nolu kaydın programatik sicil numarası bulunamadı.<br>";
                return sonuc;
            }


            //Kayıt kontrol
            ObjectArray kayitListe = new ObjectArray();

            if (formListe != null && formListe.Length > 0)
            {
                foreach (SicilNoOzellik form in formListe)
                {
                    if (prSicilNo == form.prSicilno)
                    {
                        hata += sicilNo + " sicil nolu kayıt seçilen referans malzeme ile aynı!<br/>";
                        continue;
                    }

                    string bisNo = to.BisNoGetir(form.prSicilno);
                    if (bisNo != "" && bisNo != prSicilNo + "")
                    {
                        string sicilNoHedef = to.SicilNoGetir(form.prSicilno + "");
                        string sicilNoKaynak = to.SicilNoGetir(bisNo);

                        hata += sicilNoHedef + " sicil nolu malzeme <b>" + sicilNoKaynak + "</b> sicil nolu malzeme üzerine kayıtlı!<br/>";
                        continue;
                    }

                    kayitListe.objeler.Add(form);
                }


            }
            //***************************************************************


            ve.BeginTransaction();
            if (silmeIslemi)
                sonuc = to.BisSil(new SicilNoOzellik() { bisNo = prSicilNo + "" });

            if (sonuc.islemSonuc)
            {
                if (kayitListe.objeler.Count > 0)
                {
                    foreach (SicilNoOzellik form in kayitListe.objeler)
                    {
                        form.bisNo = prSicilNo + "";

                        sonuc = to.BisKaydet(form);
                        if (!sonuc.islemSonuc)
                            hata += sonuc.hataStr + "<br>";
                    }
                }
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr + "<br>";
                ve.Rollback();
            }


            ve.VeriErisimKapat();

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr += hata;
            }

            return sonuc;
        }

        public static Sonuc BisKayitDus(VeriErisim veriErisim, Kullanici kullanan, ObjectArray sicilNolar)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            System.Collections.ArrayList listeSorgu = new System.Collections.ArrayList();
            foreach (SicilNoHareket hareket in sicilNolar.objeler)
                listeSorgu.Add(hareket.prSicilNo);
            int[] prSicilNoListe = (int[])listeSorgu.ToArray(typeof(int));

            //ObjectArray iliskiliMalzemeler = TMMSicilNoHareket.IliskiliMalzemeListele(veriErisim, kullanan, prListeSorgu);

            //System.Collections.ArrayList listeIliskiliMalzemeler = new System.Collections.ArrayList();
            //foreach (SicilNoHareket sicil in iliskiliMalzemeler.objeler)
            //    listeIliskiliMalzemeler.Add(sicil);
            //int[] prSicilNoListe = (int[])listeIliskiliMalzemeler.ToArray(typeof(int));

            if (prSicilNoListe != null && prSicilNoListe.Length > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABSicilNoOzellik to = new TABSicilNoOzellik(ve);

                ve.BeginTransaction();

                sonuc = to.BisDus(prSicilNoListe);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = TasinirMalResI.TMM118;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr + "<br>";
                    ve.Rollback();
                }

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        public static Sonuc AmortismanSureKaydet(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo, int sure)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);

            //Kayıtlı bilgi var mı? kontrol ediliyor. Eğer kayıt yok ise güncellemek için bakılıyor.
            int amortismanSuresi = to.AmortsimanSureGetir(prSicilNo);
            if (amortismanSuresi == 0)
            {
                ve.BeginTransaction();

                sonuc = to.AmortsimanSureKaydet(prSicilNo, sure);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = TasinirMalResI.TMM118;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr + "<br>";
                    ve.Rollback();
                }
            }
            else
                sonuc.islemSonuc = true;

            ve.VeriErisimKapat();


            return sonuc;
        }

        public Sonuc HesapPlanKodDegistir(Kullanici kullanan, string muhasebe, string harcamaBirimi, string ambar, int prSicilNo, HesapPlaniSatir form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, muhasebe, harcamaBirimi, ambar);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);
            ve.BeginTransaction();

            sonuc = to.HesapPlanKodDegistir(prSicilNo, form.hesapKod, kullanan.kullaniciKodu);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc RFIDEtiketTarihiGuncelle(Kullanici kullanan, int prSicilNo, TNSDateTime tarih)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);
            ve.BeginTransaction();

            sonuc = to.RFIDEtiketTarihiGuncelle(prSicilNo, tarih);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static int PrSicilNoGetir(VeriErisim veriErisim, Kullanici kullanan, string sicilNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik to = new TABSicilNoOzellik(ve);

            int prSicilNo = to.PrSicilNoGetir(sicilNo);

            ve.VeriErisimKapat();

            return prSicilNo;
        }

        public static ObjectArray PrSicilNoListele(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket form)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoOzellik tabSicilO = new TABSicilNoOzellik(ve);
            IDataReader dr = null;

            try
            {
                dr = tabSicilO.PrSicilNoListele(form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                SicilNoHareket gelen = new SicilNoHareket();
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.sicilNo = VeriErisim.DegerAl(dr, 1, "");
                gelen.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 2, "");
                gelen.ozellik.bisNo = VeriErisim.DegerAl(dr, 3, "");
                gelen.ozellik.eskiBisNo1 = VeriErisim.DegerAl(dr, 4, "");


                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }


    }
}
