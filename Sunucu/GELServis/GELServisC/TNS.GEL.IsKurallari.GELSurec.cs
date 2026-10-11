using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;
using System.Data;
using System.Collections;

namespace TNS.GEL.IsKurallari
{
    public class GELSurec
    {

        public static string SurecSifreKontrol(SurecSifre bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            int sayac = 1;
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.satirlar != null)
                {
                    foreach (Parca item in bilgi.satirlar)
                    {
                        if (item.kod <= 0)
                            hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + sayac + TNS.GEL.GelirSurecleriResI.GEL00113 + "<br/>");
                        sayac++;
                    }
                }
            }
            return hatalar.ToString();
        }


        public static string SurecKayitKontrol(Surec bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil == 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");

            if (islemturu == ENUMIslemTuru.LISTELEME || islemturu == ENUMIslemTuru.KAYITKONTROL || islemturu == ENUMIslemTuru.GUNCELLEME)
            {
                if (string.IsNullOrEmpty(bilgi.surecNo))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
                if (islemturu == ENUMIslemTuru.GUNCELLEME)
                {
                    if (bilgi.surecDurum == (int)TNS.HRC.ENUMSurecDurum.TANIMSIZ)
                    {
                        hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00112 + "<br/>");
                    }
                }
            }
            else if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (string.IsNullOrEmpty(bilgi.birim))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
                if (bilgi.muhasebe == 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = SurecKayitKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            bool kayitVar = tabSurec.SurecVarmi(bilgi);

            TABGenel tg = new TABGenel(ve);
            string sayacKod = bilgi.yil.ToString();
            sayacKod = "GELSUREC" + sayacKod;
            bilgi.surecNo = tg.BelgeNoAl(sayacKod, "000000");
            if (!string.IsNullOrEmpty(bilgi.surecNo))
            {
                sonuc = tabSurec.SurecKaydet(bilgi);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.anahtar = bilgi.surecNo.Trim();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;

                    ///Süreç Yeni bir süreç ise 
                    ///o sürecin iskoduna ait parcalar
                    ///süreçle ilişkilendiriliyot

                    Sonuc surecParcaSonuc = ParcaKaydet(veriErisim, kullanan, bilgi);
                    if (!surecParcaSonuc.islemSonuc)
                    {
                        sonuc.bilgiStr += "<br/>" + TNS.GEL.GelirSurecleriResI.GEL00105;
                        sonuc.bilgiStr += "<br/>" + TNS.GEL.GelirSurecleriResI.GEL00086 + surecParcaSonuc.hataStr;
                    }
                    SurecParcaDurum spd = new SurecParcaDurum();
                    spd.yil = bilgi.yil;
                    spd.surecNo = bilgi.surecNo;
                    spd.islemKod = bilgi.islem.kod;

                    Sonuc surecParcaDurumSonuc = ParcaDurumKaydet(veriErisim, kullanan, bilgi);
                    if (!surecParcaDurumSonuc.islemSonuc)
                    {
                        sonuc.bilgiStr += "<br/>" + TNS.GEL.GelirSurecleriResI.GEL00106;
                        sonuc.bilgiStr += "<br/>" + TNS.GEL.GelirSurecleriResI.GEL00086 + surecParcaSonuc.hataStr;
                    }

                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00026;
                ve.Rollback();
            }


            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            sonuc = tabSurec.SurecGuncelle(bilgi);
            if (sonuc.islemSonuc)
            {
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                bilgi.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                sonuc = DurumGuncelle(ve, kullanan, bilgi);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00111;
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;

        }

        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = SurecKayitKontrol(bilgi, ENUMIslemTuru.GUNCELLEME);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            sonuc = tabSurec.SurecDurumGuncelle(bilgi);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00111;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ObjectArray surecListesi = new ObjectArray();
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            IDataReader dr = tabSurec.SurecListele(bilgi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Surec src = new Surec();
                    src.yil = VeriErisim.DegerAl(dr, 0, 0);
                    src.surecTarih = VeriErisim.DegerAl(dr, 1);
                    src.surecNo = VeriErisim.DegerAl(dr, 2, "");
                    src.muhasebe = VeriErisim.DegerAl(dr, 3, 0);
                    src.birim = VeriErisim.DegerAl(dr, 4, "");
                    src.islem.kod = VeriErisim.DegerAl(dr, 5, 0);
                    src.isinAdi = VeriErisim.DegerAl(dr, 6, "");
                    src.aciklama = VeriErisim.DegerAl(dr, 7, "");
                    src.surecDurum = VeriErisim.DegerAl(dr, 8, 0);
                    //src.teklifturu = VeriErisim.DegerAl(dr, 9, 0);
                    surecListesi.objeler.Add(src);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            foreach (Surec item in surecListesi.objeler)
            {
                Is iss = new Is();
                iss.kod = item.islem.kod;
                ObjectArray isler = GELIs.Listele(veriErisim, kullanan, iss);
                if (isler.objeler.Count > 0)
                {
                    iss = (Is)isler.objeler[0];
                    item.islem.ad = iss.ad;
                }
            }
            return surecListesi;
        }

        public static Sonuc TeklifTipiGetir(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string hatalar = SurecKayitKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hatalar.ToString().Trim() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar;
                sonuc.anahtar = string.Empty;
                return sonuc;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABSurec tabSurec = new TABSurec(ve);
                ve.BeginTransaction();
                sonuc.anahtar = tabSurec.TipGetir(bilgi).ToString();
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// Süreç ilk başlatıldığında sürecin iş koduna ait parçalar
        /// SURECPARCA tablosuna kaydediliyor
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="bilgi"></param>
        /// <returns></returns>
        protected static Sonuc ParcaKaydet(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ve.BeginTransaction();
            IsParca isparca = IsParcaDondur(ve, bilgi.islem.kod);
            int sirano = 1;
            TABSurec tabSurec = new TABSurec(ve);
            foreach (Parca item in isparca.parcalar.objeler)
            {
                bilgi.parca = item;
                sonuc = tabSurec.ParcaKaydet(bilgi, sirano, 0, 0);
                if (!sonuc.islemSonuc) break;
                sirano++;
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.islemSonuc = true;
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Bu metod arayüzden süreç parçalarının ikonlarına tıklandığı zaman
        /// çağrılır(kilit aç,kilit kapat ikonları)
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="bilgi"></param>
        /// <returns></returns>
        public static Sonuc ParcaDurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, SurecParcaDurum bilgi)
        {
            Sonuc sonuc = new Sonuc(true, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            bool spdKaydiVar = tabSurec.ParcaDurumVarmi(bilgi);
            if (!spdKaydiVar)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Parça Durumuna Ait Kayıt Bulunamadı";
            }
            else
            {
                sonuc = tabSurec.ParcaDurumGuncelle(bilgi);
                if (sonuc.islemSonuc)
                {

                    //***Üzerinde işlem yapılan parca nın durumuna göre bir önceki
                    //***veya bir sonraki parçanın durumu değiştirilmeli
                    ObjectArray surecparcadurumlari = SurecDurumlariDondur(ve, bilgi.yil, bilgi.surecNo, 0, 0);
                    foreach (SurecParcaDurum item in surecparcadurumlari.objeler)
                    {

                        if (item != bilgi)
                        {
                            //*****gelen(bilgi) parça durumu devam ediyor olarak belirlendi ise
                            //*****bu parçadan sonraki parçaların durumları tanımsız=0 olmalıdır
                            if (item.parcaKod > bilgi.parcaKod && bilgi.durum == (int)TNS.GEL.ENUMSurecParcaDurum.DEVAMEDIYOR)
                            {
                                item.durum = (int)TNS.GEL.ENUMSurecParcaDurum.TANIMSIZ;
                                sonuc = tabSurec.ParcaDurumGuncelle(item);
                            }
                            //***gelen parça durumu tamalandı olarak belirlendi ise
                            //***bir sonraki parçanın durumu başladı=1 olarak güncellenmelidir
                            else if (item.parcaKod == bilgi.parcaKod + 1 && bilgi.durum == (int)TNS.GEL.ENUMSurecParcaDurum.TAMAMLANDI)
                            {
                                item.durum = (int)TNS.GEL.ENUMSurecParcaDurum.BASLADI;
                                sonuc = tabSurec.ParcaDurumGuncelle(item);
                                break;
                            }
                        }

                    }
                    if (!sonuc.islemSonuc)
                    {
                        ve.Rollback();
                    }
                    else
                    {
                        ve.Commit();
                    }

                }
                else
                {
                    ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Süreç İlk başladığında sürecin işkoduna ait olan parçaların durumları kaydediliyor.
        /// İlk parçanın durumu 1= başladı olarak belirleniyor
        /// diğer parçaların durumları ise belirsiz=0 olarak belirleniyor
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="bilgi"></param>
        /// <returns></returns>
        public static Sonuc ParcaDurumKaydet(VeriErisim veriErisim, Kullanici kullanan, Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(true, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABSurec tabSurec = new TABSurec(ve);
            ve.BeginTransaction();
            IsParca ip = IsParcaDondur(ve, bilgi.islem.kod);
            int sayac = 1;

            SurecParcaDurum spd = new SurecParcaDurum();
            foreach (Parca parca in ip.parcalar.objeler)
            {
                spd.yil = bilgi.yil;
                spd.surecNo = bilgi.surecNo;
                spd.islemKod = bilgi.islem.kod;
                spd.parcaKod = parca.kod;

                if (!sonuc.islemSonuc) break;
                if (sayac == 1)
                    spd.durum = (int)TNS.HRC.ENUMSurecParcaDurum.BASLADI;
                else
                    spd.durum = (int)TNS.HRC.ENUMSurecParcaDurum.TANIMSIZ;
                sonuc = tabSurec.ParcaDurumKaydet(spd);

                sayac++;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgi.surecNo.Trim();
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Sürece ait olan parçalar listelenir
        /// SURECPARCA tablosundan
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="yil"></param>
        /// <param name="surecno"></param>
        /// <returns></returns>
        public static ObjectArray ParcaListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecno)
        {
            ObjectArray surecParcalari = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (string.IsNullOrEmpty(surecno))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (hatalar.ToString().Trim() != "")
            {
                surecParcalari.sonuc.islemSonuc = false;
                surecParcalari.sonuc.hataStr = hatalar.ToString();
                return surecParcalari;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABSurec tabSurec = new TABSurec(ve);
                IDataReader dr = tabSurec.ParcaListele(yil, surecno);
                while (dr.Read())
                {
                    SurecParca sp = new SurecParca();
                    sp.yil = VeriErisim.DegerAl(dr, 0, 0);
                    sp.surecNo = VeriErisim.DegerAl(dr, 1, "");
                    sp.isKod = VeriErisim.DegerAl(dr, 2, 0);
                    sp.parcaKod = VeriErisim.DegerAl(dr, 3, 0);
                    sp.siraNo = VeriErisim.DegerAl(dr, 4, 0);
                    sp.ihaleUsulu = VeriErisim.DegerAl(dr, 5, 0);
                    sp.ihaleMaddesi = VeriErisim.DegerAl(dr, 6, 0);
                    sp.duzey = VeriErisim.DegerAl(dr, 7, 0);
                    sp.zorunlu = VeriErisim.DegerAl(dr, 8, 0);

                    Parca parca = new Parca();
                    parca.kod = VeriErisim.DegerAl(dr, 9, 0);
                    parca.ad = VeriErisim.DegerAl(dr, 10, "");
                    parca.sayfa = VeriErisim.DegerAl(dr, 11, "");
                    parca.yukseklik = VeriErisim.DegerAl(dr, 12, 0);
                    parca.genislik = VeriErisim.DegerAl(dr, 13, 0);
                    parca.duzey = VeriErisim.DegerAl(dr, 7, 0);
                    parca.zorunlu = VeriErisim.DegerAl(dr, 8, 0);

                    sp.parcalar.objeler.Add(parca);
                    surecParcalari.objeler.Add(sp);
                }
                ve.VeriErisimKapat();
            }
            return surecParcalari;
        }

        /// <summary>
        /// SURECDURUM tablosundan sürece ait olan parçaların durumları listelenir
        /// (parça durumu :tanımsız,devamediyor:tamamlandır gibi)
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="yil"></param>
        /// <param name="surecno"></param>
        /// <returns></returns>
        public static ObjectArray ParcaDurumListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecno, int islemkod, int parcakod)
        {
            ObjectArray parcadurumlari = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (string.IsNullOrEmpty(surecno))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (hatalar.ToString().Trim() != "")
            {
                parcadurumlari.sonuc.islemSonuc = false;
                parcadurumlari.sonuc.hataStr = hatalar.ToString();
                return parcadurumlari;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                parcadurumlari = SurecDurumlariDondur(ve, yil, surecno, islemkod, parcakod);
                ve.VeriErisimKapat();
            }
            return parcadurumlari;
        }

        protected static ObjectArray SurecDurumlariDondur(VeriErisim ve, int yil, string surecno, int islemkod, int parcakod)
        {
            ObjectArray parcadurumlari = new ObjectArray();
            TABSurec tabSurec = new TABSurec(ve);
            IDataReader dr = tabSurec.ParcaDurumListele(yil, surecno, islemkod, parcakod);
            while (dr.Read())
            {
                SurecParcaDurum spd = new SurecParcaDurum();
                spd.yil = VeriErisim.DegerAl(dr, 0, 0);
                spd.surecNo = VeriErisim.DegerAl(dr, 1, "");
                spd.islemKod = VeriErisim.DegerAl(dr, 2, 0);
                spd.parcaKod = VeriErisim.DegerAl(dr, 3, 0);
                spd.tarih = VeriErisim.DegerAl(dr, 4);
                spd.kullaniciAd = VeriErisim.DegerAl(dr, 5, "");
                spd.durum = VeriErisim.DegerAl(dr, 6, 0);
                parcadurumlari.objeler.Add(spd);
            }
            dr.Close();
            return parcadurumlari;
        }

        protected static IsParca IsParcaDondur(VeriErisim ve, int iskod)
        {
            TABIsParca tabIP = new TABIsParca(ve);
            IsParca isparca = new IsParca();
            IDataReader dr = tabIP.Listele(iskod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    Parca pBilgi = new Parca();
                    pBilgi.kod = VeriErisim.DegerAl(dr, 1, 0);
                    //2=sırano
                    pBilgi.duzey = VeriErisim.DegerAl(dr, 3, 0);
                    pBilgi.zorunlu = VeriErisim.DegerAl(dr, 4, 0);
                    pBilgi.ad = VeriErisim.DegerAl(dr, 5, "");
                    pBilgi.sayfa = VeriErisim.DegerAl(dr, 6, "");
                    pBilgi.yukseklik = VeriErisim.DegerAl(dr, 7, 0);
                    pBilgi.genislik = VeriErisim.DegerAl(dr, 8, 0);
                    isparca.parcalar.objeler.Add(pBilgi);
                }
                dr.Close();

            }
            return isparca;
        }

        public static Sonuc SifreKaydet(VeriErisim veriErisim, Kullanici kullanan, SurecSifre bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = SurecSifreKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABSurec tabSurec = new TABSurec(ve);
                ve.BeginTransaction();
                tabSurec.SurecSifreSil(bilgi.yil, bilgi.surecNo);
                if (ve.hata.Trim() == "")
                {
                    foreach (Parca item in bilgi.satirlar)
                    {
                        if (!sonuc.islemSonuc) break;
                        sonuc = tabSurec.SurecSifreKaydet(bilgi, item);
                    }
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();

            }

            return sonuc;
        }


        public static Sonuc SifreKaldir(VeriErisim veriErisim, Kullanici kullanan, SurecSifre bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = SurecSifreKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABSurec tabSurec = new TABSurec(ve);
                ve.BeginTransaction();
                tabSurec.SurecSifreSil(bilgi.yil, bilgi.surecNo);
                if (ve.hata.Trim() == "")
                {
                    ve.Commit();
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static ObjectArray SifreListele(VeriErisim veriErisim, Kullanici kullanan, SurecSifre bilgi)
        {
            ObjectArray sifreler = new ObjectArray();
            Sonuc sonuc = new Sonuc(false, "", "");
            string hata = SurecSifreKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                sifreler.sonuc.islemSonuc = false;
                sifreler.sonuc.hataStr = hata;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABSurec tabSurec = new TABSurec(ve);
                ve.BeginTransaction();
                int parcakod = 0;
                string sifre = string.Empty;
                if (bilgi.satirlar != null && bilgi.satirlar.Length > 0)
                {
                    parcakod = bilgi.satirlar[0].kod;
                }
                IDataReader dr = tabSurec.SurecSifreListele(bilgi.yil, bilgi.surecNo, parcakod, bilgi.sifre);
                SurecSifre sSifre = new SurecSifre();
                int sayac = 0;
                ArrayList listParcalar = new ArrayList();
                while (dr.Read())
                {
                    if (sayac == 0)
                    {
                        sSifre.yil = VeriErisim.DegerAl(dr, 0, 0);
                        sSifre.surecNo = VeriErisim.DegerAl(dr, 1, "");
                        sSifre.sifre = VeriErisim.DegerAl(dr, 3, "");
                        sSifre.TCKimlik = VeriErisim.DegerAl(dr, 4, "");
                        sSifre.tarih = VeriErisim.DegerAl(dr, 5);
                    }
                    Parca p = new Parca();
                    p.kod = VeriErisim.DegerAl(dr, 2, 0);
                    listParcalar.Add(p);
                    sayac++;
                }
                sSifre.satirlar = (Parca[])listParcalar.ToArray(typeof(Parca));
                sifreler.objeler.Add(sSifre);
                ve.VeriErisimKapat();
            }

            return sifreler;
        }

        public static bool ParcaTamamlandiMi(Kullanici kullanan, int yil, string surecno, int islemkod, int parcakod)
        {
            SurecParcaDurum kriter = new SurecParcaDurum();
            kriter.yil = yil;
            kriter.surecNo = surecno;
            kriter.parcaKod = parcakod;

            ObjectArray o = ParcaDurumListele(null, kullanan, yil, surecno, islemkod, parcakod);
            foreach (SurecParcaDurum s in o.objeler)
            {
                if (s.durum == (int)ENUMSurecParcaDurum.TAMAMLANDI)
                    return true;
                else
                    return false;
            }

            return false;
        }
    }


}
