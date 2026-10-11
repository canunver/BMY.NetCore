using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;
using System.Data;
using TNS.BHMv2;
using TNS.PER;
using System.Collections;
using TNS.UZY;

namespace TNS.HRC.IsKurallari
{
    public class SeyyarGorevC
    {
        private static string HarcamaBirimiKodYapisi()
        {
            string kodyapi = TNS.DEG.Arac.DegiskenDegerBul(0, "KODYAPI/KOD17/KODYAPI");
            return kodyapi;
        }

        private static int DegiskenKivrimSayisi()
        {
            string kodyapi = TNS.DEG.Arac.DegiskenDegerBul(0, "KODYAPI/KOD17/KODYAPI");
            if (string.IsNullOrEmpty(kodyapi)) return 0;
            return kodyapi.Split('.').Length;
        }

        public static Sonuc VeriKaydedilebilirMi(SeyyarGorevI sg, VeriErisim ve, TABSeyyarGorev tabsg, VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc();
            if (sg.asama == 0)
            {
                sonuc.DegerAta("Seyyar Görev Aşama Bilgisi Boş Olmamalıdır!");
            }
            if (sg.yil == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.DegerAta("Seyyar Görev Yıl Bilgisi Boş Olmamalıdır");
            }
            StringBuilder strBHBkivrim = new StringBuilder();
            int hbOkunanDegiskenXmlKivrimSayisi = DegiskenKivrimSayisi();
            int hbKivrimSayisi = sg.harcamaBirimiKodu.Split('.').Length;
            if (hbKivrimSayisi != hbOkunanDegiskenXmlKivrimSayisi)
            {
                sonuc.DegerAta("Harcama Birimi Kıvrım Sayısı " + hbOkunanDegiskenXmlKivrimSayisi.ToString() + " Olmalıdır!");
            }

            string[] harcamaBirimiKivrimlari = sg.harcamaBirimiKodu.Split('.');
            for (int i = 0; i < harcamaBirimiKivrimlari.Length + 1 && sonuc.islemSonuc; i++)
            {
                int onayliSayisi;
                if (i < harcamaBirimiKivrimlari.Length)
                {
                    strBHBkivrim.Append(harcamaBirimiKivrimlari[i]);
                    onayliSayisi = tabsg.SeyyarGorevOnayliVarMi(ve, sg.yil, strBHBkivrim.ToString(), sg.asama);
                }
                else
                {
                    strBHBkivrim.Clear();
                    onayliSayisi = tabsg.SeyyarGorevOnayliVarMi(ve, sg.yil, "", sg.asama);
                }
                if (!string.IsNullOrWhiteSpace(ve.hata))
                {
                    sonuc.DegerAta("Beklenmeye hata:" + ve.hata);
                }
                else if (onayliSayisi > 0)
                {
                    string bilgi = strBHBkivrim.ToString().Trim() != "" ? strBHBkivrim.ToString() : " Tüm Harcama Birimleri ";
                    sonuc.DegerAta(sg.harcamaBirimiKodu + " İçinde " + bilgi + " Onaylanmıştır Kaydedilemez!");
                }
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, SeyyarGorevI[] sgler)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);

            foreach (var sg in sgler)
            {
                sg.yil = yil;
                sg.asama = asama;
                if (string.IsNullOrWhiteSpace(sg.harcamaBirimiKodu))
                {
                    sonuc.DegerAta("Harcama birim kodu boş olamaz!");
                    return sonuc;
                }

                KurumsalYapi ky = KurumsalYapiGetir(sg.harcamaBirimiKodu);

                if (string.IsNullOrWhiteSpace(ky.kod))
                {
                    sonuc.DegerAta(sg.harcamaBirimiKodu + " bulunamadı!");
                    return sonuc;
                }

                if (!Yetki.YazmaYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
                {
                    sonuc.DegerAta("Seyyar Görev Kayıt İşlemi İçin Yetkiniz Yoktur");
                    return sonuc;
                }
                // bool onayliHarcamaBirimiVar = false;

                //**********************************************************

                sonuc = VeriKaydedilebilirMi(sg, ve, tabsg, veriErisim);
                if (!sonuc.islemSonuc)//veri kaydedilemiyorsa
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();
            try
            {
                KaydetIc(tabsg, sgler, yil, asama, kullanan, sonuc);
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            if (sonuc.islemSonuc) //ünvan bilgisi sorunsuz kaydedilirse
            {
                ve.Commit();
                sonuc.bilgiStr = "Seyyar Görev Bilgisi ve Seyyar Görev Ünvan Bilgisi Kaydedildi";
            }
            else
            {
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void KaydetIc(TABSeyyarGorev tabsg, SeyyarGorevI[] sgler, int yil, int asama, Kullanici kullanan, Sonuc sonuc)
        {
            foreach (var sg in sgler)
            {
                sg.yil = yil;
                sg.asama = asama;
                sonuc = tabsg.SeyyarGorevPlanSil(sg);
                if (sonuc.islemSonuc && sg.memur > 0) // seyyar görev gerçekleşen silerken sorun yoksa memur kaydediliyor
                {
                    sonuc = tabsg.SeyyarGorevPlanlananKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.MEMUR, sg.memur);
                }
                if (sonuc.islemSonuc && sg.isci > 0) // seyyar görev gerçekleşen silerken sorun yoksa işçi kaydediliyor
                {
                    sonuc = tabsg.SeyyarGorevPlanlananKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.ISCI, sg.isci);
                }
                if (sonuc.islemSonuc && sg.sozlesmeli > 0) // seyyar görev gerçekleşen silerken sorun yoksa işçi kaydediliyor
                {
                    sonuc = tabsg.SeyyarGorevPlanlananKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.SOZLESMELI, sg.sozlesmeli);
                }
                if (asama == 1)
                {
                    if (sonuc.islemSonuc)
                        sonuc = tabsg.SeyyarGorevGerceklesenSil(sg);

                    if (sonuc.islemSonuc && sg.oncKullanilanMemur > 0) // seyyar görev gerçekleşen silerken sorun yoksa memur kaydediliyor
                    {
                        sonuc = tabsg.SeyyarGorevGerceklesenKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.MEMUR, sg.oncKullanilanMemur);
                    }

                    if (sonuc.islemSonuc && sg.oncKullanilanIsci > 0) // seyyar görev gerçekleşen silerken sorun yoksa işçi kaydediliyor
                    {
                        sonuc = tabsg.SeyyarGorevGerceklesenKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.ISCI, sg.oncKullanilanIsci);
                    }

                    if (sonuc.islemSonuc && sg.oncKullanilanSozlesmeli > 0) // seyyar görev gerçekleşen silerken sorun yoksa işçi kaydediliyor
                    {
                        sonuc = tabsg.SeyyarGorevGerceklesenKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.SOZLESMELI, sg.oncKullanilanSozlesmeli);
                    }

                    if (sonuc.islemSonuc && sg.unvanlar != null)//Seyyar görev kaydederken hata oluşmazsa
                    {
                        sonuc = tabsg.UnvanBilgisiSil(sg.yil, sg.harcamaBirimiKodu, sg.asama, ENUMPersonelIstihdamSekli.TOPLAM);
                        foreach (SeyyarGorevUnvanBilgi sgub in sg.unvanlar) // seyyar görev unvan bilgisi kaydet
                        {
                            if (!sonuc.islemSonuc) break;
                            sonuc = tabsg.UnvanBilgisiKaydet(sg, sgub);
                        }
                    }
                }
            }
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string harcamaBirimi, int asama)
        {
            return SeyyarGorevAdamGunTespitListele(veriErisim, kullanan, yil, harcamaBirimi, 1, true, true, true, true);
        }

        private static KurumsalYapi KurumsalYapiGetir(string harcamaBirimi)
        {
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            KurumsalYapi ky = new KurumsalYapi();
            ky = servis.KurumsalYapiDondur(System.Threading.Thread.CurrentThread.CurrentCulture.Name, harcamaBirimi);
            return ky;
        }

        public static Sonuc OnaySil(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc sonuc = new Sonuc();
            // if (!Yetki.YazmaYetkisiVarMi(kullanan))
            if (!Yetki.SistemYoneticisiMi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Onay Silme Yetkiniz Bulunmamaktadır";
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            ve.BeginTransaction();
            sonuc = tabsg.SeyyarGorevOnaySil(ve, sg.yil, sg.harcamaBirimiKodu, sg.asama);
            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
            }
            else
            {
                sonuc.bilgiStr = "Onay Kaldırıldı";
                ve.Commit();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc OnayKaydet(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc sonuc = new Sonuc();
            KurumsalYapi ky = KurumsalYapiGetir(sg.harcamaBirimiKodu);
            string[] harcamaBirimiKivrimlari = sg.harcamaBirimiKodu.Split('.');
            if (harcamaBirimiKivrimlari.Length != 4)
            {
                if (!Yetki.SistemYoneticisiMi(kullanan))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Onay Kaydetme Yetkiniz Bulunmamaktadır";
                    return sonuc;
                }
            }
            if (!Yetki.YazmaYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Onay Kayıt İşlemi İçin Yetkiniz Yoktur";
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            ve.BeginTransaction();
            tabsg.SeyyarGorevOnaySil(ve, sg.yil, sg.harcamaBirimiKodu, sg.asama); // önce harcama birimi kıvrımını onaylılar tablosundan sil
            if (ve.hata.Trim() == "")
            {
                sonuc = tabsg.SeyyarGorevOnayKaydet(kullanan.kullaniciKodu, sg.yil, sg.harcamaBirimiKodu, sg.asama); // kıvrımı onaylılar tablosuna ekle
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
            }
            else
            {
                ve.Commit();
                sonuc.bilgiStr = "Onay Kaydedildi";
            }
            ve.VeriErisimKapat();
            return sonuc;

        }

        public static ObjectArray PersonelListele(VeriErisim veriErisim, Kullanici kullanan, string harcamaBirimi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray personelListesi = new ObjectArray();
            if (harcamaBirimi.Trim() == "")
            {
                personelListesi.sonuc.islemSonuc = false;
                personelListesi.sonuc.hataStr = "Harcamabirimi Boş Olmamalıdır";
                return personelListesi;
            }
            KurumsalYapi ky = KurumsalYapiGetir(harcamaBirimi);
            HarcamaBirimiPersoneli personel;
            if (!Yetki.GormeYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                personelListesi.sonuc.islemSonuc = false;
                personelListesi.sonuc.hataStr = "Listeleme İşlemi İçin Yetkiniz Yoktur";
                return personelListesi;
            }

            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            ve.BeginTransaction();
            //IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            //KurumsalYapi ky = new KurumsalYapi();
            //ky = servis.KurumsalYapiDondur(System.Threading.Thread.CurrentThread.CurrentCulture.Name, harcamaBirimi);
            IDataReader dr = tabsg.PersonelBilgisiListele(ky.birimKod, OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, 0));
            while (dr.Read())
            {
                personel = new HarcamaBirimiPersoneli();
                personel.adi = VeriErisim.DegerAl(dr, 0, "");
                personel.soyadi = VeriErisim.DegerAl(dr, 1, "");
                personel.istihdamSekli = VeriErisim.DegerAl(dr, 2, 0);
                personel.unvanKodu = VeriErisim.DegerAl(dr, 3, 0);
                personel.unvanAdi = VeriErisim.DegerAl(dr, 4, "");
                personel.odemeyeEsasDerece = VeriErisim.DegerAl(dr, 5, 0);
                personel.odemeyeEsasKademe = VeriErisim.DegerAl(dr, 6, 0);
                personelListesi.objeler.Add(personel);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return personelListesi;
        }

        public static ObjectArray SeyyarGorevAdamGunTespitListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string harcamaBirimi, int asama, bool harcamaOlsun, bool perSayOlsun, bool unvanOlsun, bool detay)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray sgAdamGunTespitListesi = new ObjectArray();

            if (detay && harcamaBirimi.Replace(".", "").Length != 11)
            {
                sgAdamGunTespitListesi.sonuc.DegerAta("Kurumsal yapı, birim seviyesinde olmalıdır!");
                return sgAdamGunTespitListesi;
            }

            KurumsalYapi ky = KurumsalYapiGetir(harcamaBirimi);
            if (!Yetki.GormeYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                sgAdamGunTespitListesi.sonuc.DegerAta("Listeleme İşlemi İçin Yetkiniz Yoktur");
                return sgAdamGunTespitListesi;
            }

            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            SeyyarGorevAdamGunTespitListeleIc(tabsg, sgAdamGunTespitListesi, yil, harcamaBirimi, asama);

            VeriErisim vePER = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veUnvan = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsgUnvan = new TABSeyyarGorev(veUnvan);
            TABSeyyarGorev tabsgPer = new TABSeyyarGorev(vePER);

            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            IDataReader dr;
            foreach (SeyyarGorevI sgItem in sgAdamGunTespitListesi.objeler)
            {
                if (perSayOlsun)
                {
                    sgItem.personelSayisiMemur = tabsgPer.PersonelSayisiGetir((int)ENUMPersonelIstihdamSekli.MEMUR, sgItem.birimKod, sgItem.saymanlikKod);
                    sgItem.personelSayisiIsci = tabsgPer.PersonelSayisiGetir((int)ENUMPersonelIstihdamSekli.ISCI, sgItem.birimKod, sgItem.saymanlikKod);
                    sgItem.personelSayisiSozlesmeli = tabsgPer.PersonelSayisiGetir((int)TNS.PER.ENUMPersonelIstihdamSekli.Sozlesmeli, sgItem.birimKod, sgItem.saymanlikKod);
                }
                if (harcamaOlsun)
                {
                    sgItem.oncekiYilKullanilanTutar = servisUZY.UzayDegeriDbl(kullanan, "MUHASEBAT", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), "12", sgItem.saymanlikKod.ToString(), sgItem.birimKod, "830", sgItem.kurKod, "", "", "03.3.5.01,06.9.2.05,03.03.50.01,06.09.20.05"), false);
                }
                if (unvanOlsun)
                {
                    dr = tabsgUnvan.UnvanListele(sgItem.birimKod, sgItem.saymanlikKod);
                    if (dr != null)
                    {
                        TNSCollection colUnvan = new TNSCollection();
                        while (dr.Read())
                        {
                            SeyyarGorevUnvanBilgi sgub = new SeyyarGorevUnvanBilgi();

                            sgub.istihdamSekliKod = (ENUMPersonelIstihdamSekli)VeriErisim.DegerAl(dr, 0, 0);
                            sgub.unvanKod = VeriErisim.DegerAl(dr, 1, 0);
                            sgub.unvanAd = VeriErisim.DegerAl(dr, 2, "");
                            sgub.personelSayisi = VeriErisim.DegerAl(dr, 3, 0);
                            colUnvan.Add(sgub);
                        }
                        dr.Close();
                        sgItem.unvanlar = (SeyyarGorevUnvanBilgi[])colUnvan.ToArray(typeof(SeyyarGorevUnvanBilgi));
                    }

                    //**********************unvanların vize cetveli bilgisi ve şimdiki yıl seyyar görev almalı mı? bilgisi okunuyor
                    for (int i = 0; i < sgItem.unvanlar.Length; i++)
                    {
                        dr = tabsg.UnvanBilgiGetir(ve, yil, harcamaBirimi, asama, sgItem.unvanlar[i].istihdamSekliKod, sgItem.unvanlar[i].unvanKod);
                        while (dr.Read())
                        {
                            sgItem.unvanlar[i].vizeCetvelindeOlmaliMi = VeriErisim.DegerAl(dr, 1, 0) == 1;
                        }
                        dr.Close();
                        dr = tabsg.UnvanBilgiGetir(ve, yil - 1, harcamaBirimi, asama, sgItem.unvanlar[i].istihdamSekliKod, sgItem.unvanlar[i].unvanKod);
                        while (dr.Read())
                        {
                            sgItem.unvanlar[i].oncekiVizeCetvelindeVarMi = VeriErisim.DegerAl(dr, 1, 0) == 1;
                        }
                        dr.Close();
                    }
                }
            }

            veUnvan.VeriErisimKapat();
            vePER.VeriErisimKapat();
            ve.VeriErisimKapat();

            return sgAdamGunTespitListesi;
        }

        private static void SeyyarGorevAdamGunTespitListeleIc(TABSeyyarGorev tabsg, ObjectArray sgAdamGunTespitListesi, int yil, string harcamaBirimi, int asama)
        {
            IDataReader dr = tabsg.SeyyarGorevPlanlananListeleDetayli(yil, harcamaBirimi, asama);

            if (dr != null)
            {
                while (dr.Read())
                {
                    SeyyarGorevI sg = new SeyyarGorevI();
                    sg.harcamaBirimiKodu = OrtakFonksiyonlar.KodNoktaKoy("9.99.999.99999", VeriErisim.DegerAl(dr, 0, ""));
                    sg.harcamaBirimiAdi = VeriErisim.DegerAl(dr, 1, "");
                    sg.saymanlikKod = VeriErisim.DegerAl(dr, 2, 0);
                    sg.birimKod = VeriErisim.DegerAl(dr, 3, "");
                    sg.kurKod = VeriErisim.DegerAl(dr, 4, "");

                    sg.memur = VeriErisim.DegerAl(dr, 5, 0);
                    sg.isci = VeriErisim.DegerAl(dr, 6, 0);

                    sg.oncAsamaMemur = VeriErisim.DegerAl(dr, 7, 0);
                    sg.oncAsamaIsci = VeriErisim.DegerAl(dr, 8, 0);

                    sg.oncVizeliMemur = VeriErisim.DegerAl(dr, 9, 0);
                    sg.oncVizeliIsci = VeriErisim.DegerAl(dr, 10, 0);

                    sg.oncKullanilanMemur = VeriErisim.DegerAl(dr, 11, 0);
                    sg.oncKullanilanIsci = VeriErisim.DegerAl(dr, 12, 0);

                    sg.sozlesmeli = VeriErisim.DegerAl(dr, 13, 0);
                    sg.oncAsamaSozlesmeli = VeriErisim.DegerAl(dr, 14, 0);
                    sg.oncVizeliSozlesmeli = VeriErisim.DegerAl(dr, 15, 0);
                    sg.oncKullanilanSozlesmeli = VeriErisim.DegerAl(dr, 16, 0);

                    sgAdamGunTespitListesi.objeler.Add(sg);
                }
                dr.Close();
            }
            else
                sgAdamGunTespitListesi.sonuc.DegerAta(tabsg.Hata());
        }

        public static Sonuc SeyyarGorevAdamGunTespitKaydet(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc sonuc = new Sonuc();
            KurumsalYapi ky = KurumsalYapiGetir(sg.harcamaBirimiKodu);
            if (string.IsNullOrWhiteSpace(ky.kod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = sg.harcamaBirimiKodu + " bulunamadı!";
                return sonuc;
            }

            if (!Yetki.YazmaYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Adam Gün Tespit Kayıt İşlemi İçin Yetkiniz Yoktur";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            sonuc = VeriKaydedilebilirMi(sg, ve, tabsg, veriErisim);
            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();

                sonuc = tabsg.SeyyarGorevPlanSil(sg);//adamgüntespit bilgisi sil
                if (sonuc.islemSonuc && sg.memur > 0)
                    sonuc = tabsg.SeyyarGorevPlanlananKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.MEMUR, sg.memur);

                if (sonuc.islemSonuc && sg.isci > 0)
                    sonuc = tabsg.SeyyarGorevPlanlananKaydet(kullanan.kullaniciKodu, sg, ENUMPersonelIstihdamSekli.MEMUR, sg.isci);


                if (sonuc.islemSonuc)//seyyar görev gün bilgileri kaydederken sorun çıkmamışsa
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Seyyar Görev Adam Gün Tespit Bilgisi Kaydedildi...";
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.hataStr = ve.hata;
                sonuc.islemSonuc = false;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SeyyarGorevUnvanTespitKaydet(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc sonuc = new Sonuc();
            KurumsalYapi ky = KurumsalYapiGetir(sg.harcamaBirimiKodu);
            if (!Yetki.YazmaYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Seyyar Görev Kayıt İşlemi İçin Yetkiniz Yoktur";
                return sonuc;
            }
            StringBuilder sbHatalar = new StringBuilder();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            ve.BeginTransaction();

            int onayliSayisi = tabsg.SeyyarGorevOnayliVarMi(ve, sg.yil, "", sg.asama);
            if (onayliSayisi > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Ünvan Tespit Bilgisi Onaylanmıştır Kaydedilemez...";
                return sonuc;
            }
            int sayac = 0;
            foreach (SeyyarGorevUnvanBilgi sgub in sg.unvanlar)
            {
                tabsg.UnvanBilgisiSil(sg.yil, sg.harcamaBirimiKodu, sg.asama, sgub.istihdamSekliKod, sgub.unvanKod);
                if (ve.hata.Trim() == "")//unvan bilgisi silinirken hata oluşmazsa
                {
                    sonuc = tabsg.UnvanBilgisiKaydet(sg, sgub);
                    if (sonuc.islemSonuc)
                    {
                        sonuc.bilgiStr = "Seyyar Görev Ünvan Tespit Bilgisi Kaydedildi...";
                        ve.Commit();
                    }
                    else
                    {
                        sbHatalar.Append("Satır=" + sayac.ToString() + " Hata: " + sonuc.hataStr);
                        ve.Rollback();
                    }
                }
                else
                {
                    sbHatalar.Append("Satır=" + sayac.ToString() + " Hata: " + ve.hata.Trim());
                    ve.Rollback();
                }
                sayac++;
            }
            ve.VeriErisimKapat();
            sonuc.hataStr = sbHatalar.ToString();
            return sonuc;
        }

        public static ObjectArray SeyyarGorevUnvanTespitListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int istihdamSekli, int unvanKodu, int asama, string harcamaBirimi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray sgUnvanTespitListesi = new ObjectArray();
            KurumsalYapi ky = KurumsalYapiGetir(harcamaBirimi);

            if (!Yetki.GormeYetkisiVarMi(kullanan, ky.saymanlikKod.ToString(), ky.birimKod))
            {
                sgUnvanTespitListesi.sonuc.islemSonuc = false;
                sgUnvanTespitListesi.sonuc.hataStr = "Listeleme İşlemi İçin Yetkiniz Yoktur";
                return sgUnvanTespitListesi;
            }

            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            VeriErisim veUnvan = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsgUnvan = new TABSeyyarGorev(veUnvan);
            //**************şimdiki yıl vize cetveli ünvan bilgisi(yiliistihdamsekli,unvankodu,asama=1,harcamabirimi <>'')
            //IDataReader dr = tabsg.UnvanTespitListele(yil, asama, "=''", istihdamSekli, unvanKodu);//istihamdamsekli,ünvankod,gunsayisi
            IDataReader dr = tabsgUnvan.UnvanListele(istihdamSekli, unvanKodu);
            TNSCollection colUnvan;
            SeyyarGorevI sgSimdikiYil = new SeyyarGorevI();
            if (dr != null)
            {
                colUnvan = new TNSCollection();
                while (dr.Read())
                {
                    SeyyarGorevUnvanBilgi sgub = new SeyyarGorevUnvanBilgi();
                    sgub.istihdamSekliKod = (ENUMPersonelIstihdamSekli)VeriErisim.DegerAl(dr, 0, 0);
                    sgub.unvanKod = VeriErisim.DegerAl(dr, 1, 0);
                    sgub.unvanAd = VeriErisim.DegerAl(dr, 2, "");
                    sgub.personelSayisi = 0;
                    sgub.vizeCetvelindeOlmaliMi = false;
                    sgub.oncekiVizeCetvelindeVarMi = false;
                    colUnvan.Add(sgub);
                }
                sgSimdikiYil.unvanlar = (SeyyarGorevUnvanBilgi[])colUnvan.ToArray(typeof(SeyyarGorevUnvanBilgi));
                dr.Close();
            }
            ///********önceki yıl vize cetveli durumu
            for (int i = 0; i < sgSimdikiYil.unvanlar.Length; i++)
            {
                dr = tabsg.UnvanTespitListele(yil - 1, 3, "=''", sgSimdikiYil.unvanlar[i].istihdamSekliKod, sgSimdikiYil.unvanlar[i].unvanKod);
                while (dr.Read())
                {
                    sgSimdikiYil.unvanlar[i].oncekiVizeCetvelindeVarMi = VeriErisim.DegerAl(dr, 0, 0) == 1;
                }
                dr.Close();
                sgUnvanTespitListesi.objeler.Add(sgSimdikiYil.unvanlar[i]);
            }
            ///************talep sayıları
            for (int i = 0; i < sgSimdikiYil.unvanlar.Length; i++)
            {
                dr = tabsg.UnvanTespitTalepSayisiListele(yil, 1, "<>''", sgSimdikiYil.unvanlar[i].unvanKod, sgSimdikiYil.unvanlar[i].istihdamSekliKod);
                while (dr.Read())
                {
                    sgSimdikiYil.unvanlar[i].personelSayisi = VeriErisim.DegerAl(dr, 0, 0);
                }
                dr.Close();
            }
            ///*******şimdiki yıl vize cetvelinde olmalımı
            for (int i = 0; i < sgSimdikiYil.unvanlar.Length; i++)
            {
                dr = tabsg.UnvanTespitListele(yil, asama, "=''", sgSimdikiYil.unvanlar[i].istihdamSekliKod, sgSimdikiYil.unvanlar[i].unvanKod);
                while (dr.Read())
                {
                    sgSimdikiYil.unvanlar[i].vizeCetvelindeOlmaliMi = VeriErisim.DegerAl(dr, 0, 0) == 1;
                }
                dr.Close();
            }

            veUnvan.VeriErisimKapat();
            ve.VeriErisimKapat();
            return sgUnvanTespitListesi;
        }

        protected static int UnvanVarmi(SeyyarGorevI sgAranacak, int unvanKod, ENUMPersonelIstihdamSekli istihdamSekli)
        {
            for (int i = 0; i < sgAranacak.unvanlar.Length; i++)
            {
                SeyyarGorevUnvanBilgi sgub = sgAranacak.unvanlar[i];
                if (sgub.istihdamSekliKod == istihdamSekli && sgub.unvanKod == unvanKod)
                {
                    return i;
                }
            }
            return -1;
        }

        public static ObjectArray PersonelHarcamaBirimiListele(VeriErisim veriErisim, Kullanici kullanan, int istihdamSekli, int unvanKodu)
        {
            ObjectArray sgListesi = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            IDataReader dr = tabsg.HarcamaBirimiVePersonelAdetListele(istihdamSekli, unvanKodu);
            while (dr.Read())
            {
                SeyyarGorevI sg = new SeyyarGorevI();
                sg.harcamaBirimiKodu = VeriErisim.DegerAl(dr, 0, "");
                sg.harcamaBirimiAdi = "";
                sg.unvanlar = new SeyyarGorevUnvanBilgi[1];
                sg.unvanlar[0] = new SeyyarGorevUnvanBilgi();
                sg.unvanlar[0].personelSayisi = VeriErisim.DegerAl(dr, 1, 0);
                sgListesi.objeler.Add(sg);
            }
            dr.Close();

            VeriErisim veBUM = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            veBUM.BeginTransaction();
            TABSeyyarGorev tabsgBUM = new TABSeyyarGorev(veBUM);
            foreach (SeyyarGorevI sgItem in sgListesi.objeler)
            {
                dr = tabsgBUM.HarcamaBirimiGetir(sgItem.harcamaBirimiKodu);
                while (dr.Read())
                {
                    sgItem.harcamaBirimiAdi = VeriErisim.DegerAl(dr, 1, "");
                }
                dr.Close();
            }
            veBUM.VeriErisimKapat();
            ve.VeriErisimKapat();
            return sgListesi;
        }

        public static SeyyarGorevDurumAna SeyyarGorevDurumBul(VeriErisim veriErisim, Kullanici kullanan, int yil, int saymanlik, string birim, string kurKod, int islemKod, int surecYil, string surecNo, string oeNo)
        {
            SeyyarGorevDurumAna sgdAna = new SeyyarGorevDurumAna();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);

            IDataReader dr = tabsg.VizeliDurumBul(yil, saymanlik, birim, kurKod);

            if (dr != null)
            {
                while (dr.Read())
                {
                    SeyyarGorevDurum sgd = new SeyyarGorevDurum();
                    sgd.istihdamSekli = VeriErisim.DegerAl(dr, 0, 0);
                    sgd.vizeEdilen = VeriErisim.DegerAl(dr, 1, 0);
                    sgdAna.istihdamSekilleri.Add(sgd);
                }
                dr.Close();
            }
            else
                sgdAna.sonuc.DegerAta("Veriye erişilirken hata ile karşılaşıldı:" + ve.hata);

            foreach (SeyyarGorevDurum item in sgdAna.istihdamSekilleri)
            {
                int k;
                double t;
                tabsg.SurecKullanilanBul(yil, saymanlik, birim, kurKod, item.istihdamSekli, islemKod, 0, "", out k, out t);
                item.kullanilan = k;
                item.tutar = t;
                if (!string.IsNullOrWhiteSpace(surecNo))
                {
                    tabsg.SurecKullanilanBul(yil, saymanlik, birim, kurKod, item.istihdamSekli, islemKod, surecYil, surecNo, out k, out t);
                    item.surecKullanilan = k;
                    item.surecTutar = t;
                }
            }

            TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            string uzayKosul = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), saymanlik.ToString(), birim, "830", "", "", "", "03.03", "", "23.00.00.02", "", "", "", "", "");
            sgdAna.harcanan = uzy.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false);

            if (!string.IsNullOrWhiteSpace(surecNo))
            {
                uzayKosul = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), saymanlik.ToString(), birim, "830", "", "", "", "03.03", "", "23.00.00.02", "", "", surecNo, "", "");
                sgdAna.surecHarcama = uzy.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false);
            }
            else
                sgdAna.surecHarcama = 0;

            if (!string.IsNullOrWhiteSpace(oeNo))
            {
                uzayKosul = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), saymanlik.ToString(), birim, "830", "", "", "", "03.03", "", "23.00.00.02", "", "", "", oeNo, "");
                sgdAna.oeHarcama = uzy.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false);
            }
            else
                sgdAna.oeHarcama = 0;

            sgdAna.gonderilen = 0;
            uzayKosul = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), "", "", "", saymanlik.ToString(), birim, "", "", "", "", "", "03.03", "23.00.00.02", "");
            sgdAna.gonderilen = uzy.UzayDegeriDbl(kullanan, "MUHGONDERMEBELGESITENKIS", uzayKosul, false);

            return sgdAna;
        }

        //internal static ObjectArray ListeleSeyyarGorevDOSE(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, TavanGenel kistas)
        //{
        //    string[] sorgular = new string[4];
        //    string[] baglantilar = new string[4];

        //    int saymanlik = OrtakFonksiyonlar.ConvertToInt(kistas.kod1, 0);
        //    int ilKod = saymanlik / 1000;


        //    sorgular[0] = TABSeyyarGorev.BirimTavanBul(yil, senaryoKod, ENUMTavanTur.tavanTurSeyyarDOSE, ilKod, kistas.kod2, kistas.kod3);
        //    baglantilar[0] = "BaglantiSatiriBHMv2";

        //    sorgular[1] = TABSeyyarGorev.BirimPlanlananBul(yil, 2, 1, ilKod, kistas.kod2, kistas.kod3, false, null, 0, false, false, false);
        //    baglantilar[1] = "BaglantiSatiriHRC";
        //    sorgular[2] = TABSeyyarGorev.BirimPlanlananBul(yil, 2, 4, ilKod, kistas.kod2, kistas.kod3, false, null, 0, false, false, false);
        //    baglantilar[2] = "BaglantiSatiriHRC";

        //    MuhasebeBirim mb = new MuhasebeBirim();
        //    mb.plakaKod = ilKod;
        //    mb.birimKod = kistas.kod2;
        //    mb.kurKod = kistas.kod3;
        //    sorgular[3] = TABRaporlar.BirimListeleStrMuhSql(mb, true, null);
        //    baglantilar[3] = "BaglantiSatiriHRC";

        //    TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

        //    SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2 });

        //    ObjectArray oa = new ObjectArray();
        //    foreach (DictionaryEntry de in ht)
        //    {
        //        TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
        //        TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

        //        TavanGenel g = new TavanGenel();
        //        g.kod1 = alAnahtar.StrAl(0);
        //        g.kod2 = alAnahtar.StrAl(1);
        //        g.kod3 = alAnahtar.StrAl(2);

        //        g.yil0 = alVeri.DblAl(0);
        //        g.yil1 = alVeri.DblAl(1);
        //        g.ekOdenek = alVeri.DblAl(2);
        //        g.ekOdenek1 = alVeri.DblAl(3);

        //        g.ad1 = alVeri.StrAl(4);
        //        g.ad2 = alVeri.StrAl(5);
        //        g.ad2 = TNS.BHMv2.Arac.BirimAdOl(g.ad1, g.ad2);

        //        oa.objeler.Add(g);
        //    }
        //    return oa;
        //}

        internal static ObjectArray ListeleSeyyarIlTavan(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, TNSCollection ilKodlar, bool ilkTeklifTavan, bool ilkTeklifDetay)
        {
            string[] sorgular = new string[5];
            string[] baglantilar = new string[5];

            string ilListe = ListeOl(ilKodlar);

            sorgular[0] = TABSeyyarGorev.IlTavanBul(yil, senaryoKod, BKD.ENUMTavanTur.tavanTurSeyyarIlTavan, ilkTeklifTavan, ilListe);
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = TABSeyyarGorev.IlPlanlananBul(yil, 3, 1, ilListe, 0, false);
            baglantilar[1] = "BaglantiSatiriHRC";

            sorgular[2] = TABSeyyarGorev.IlPlanlananBul(yil, 3, 4, ilListe, 0, false);
            baglantilar[2] = "BaglantiSatiriHRC";

            sorgular[3] = TABSeyyarGorev.IlPlanlananBul(yil, 3, 2, ilListe, 0, false);
            baglantilar[3] = "BaglantiSatiriHRC";

            sorgular[4] = TABSeyyarGorev.IlListeleSql(ilListe);
            baglantilar[4] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            ObjectArray oa = new ObjectArray();
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                BKD.TavanGenel g = new BKD.TavanGenel();
                g.kod1 = alAnahtar.StrAl(0);
                g.kod2 = "000000";

                g.yil0 = alVeri.DblAl(0);
                g.yil1 = alVeri.DblAl(1);
                g.yil2 = alVeri.DblAl(2);
                g.ekOdenek = alVeri.DblAl(3);
                g.ekOdenek1 = alVeri.DblAl(4);
                g.ekOdenek2 = alVeri.DblAl(5);

                g.sgb0 = alVeri.DblAl(6);
                g.sgb1 = alVeri.DblAl(7);
                g.sgb2 = alVeri.DblAl(8);

                g.ad1 = alVeri.StrAl(9);

                oa.objeler.Add(g);
            }
            return oa;
        }

        private static string ListeOl(TNSCollection ilKodlar)
        {
            string donusDeger = "";
            if (ilKodlar != null)
            {
                foreach (string item in ilKodlar)
                {
                    if (!string.IsNullOrWhiteSpace(donusDeger)) donusDeger += ",";
                    donusDeger += VeriErisim.DegerYap(item);
                }
            }
            return donusDeger;
        }

        private static TNSCollection BirimKodlariOlustur(TABHRCSEYYARGOREVREVIZEFORM tab, int yil, int belgeNo)
        {
            if (belgeNo <= 0) return null;
            TNSCollection birimler = new TNSCollection();

            IDataReader dr = tab.FormBirimleriListele(yil, belgeNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    birimler.Add(VeriErisim.DegerAl(dr, 0, ""));
                }
                dr.Close();
            }

            return birimler;
        }

        private static TNSCollection IlKodlariOlustur(TABHRCSEYYARGOREVREVIZEFORM tab, int yil, string belgeNo)
        {
            TNSCollection birimler = new TNSCollection();

            IDataReader dr = tab.FormIlleriListele(yil, belgeNo, BKD.ENUMTavanTur.tavanTurSeyyarIlTavan);
            if (dr != null)
            {
                while (dr.Read())
                {
                    birimler.Add(VeriErisim.DegerAl(dr, 0, ""));
                }
                dr.Close();
            }

            return birimler;
        }

        internal static SeyyarGorevRevize OkuSeyyarGorevRevizeForm(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMRevizeFormBelgeTur belgeTur, int belgeNo, bool detayOku, bool detayBilgiOku, bool tumIl, bool onaylilar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCSEYYARGOREVREVIZEFORM tab = new TABHRCSEYYARGOREVREVIZEFORM(ve);

            SeyyarGorevRevize form = new SeyyarGorevRevize();
            form.yil = yil;
            form.belgeTur = belgeTur;
            form.belgeNo = belgeNo;
            IDataReader dr = tab.Oku(form);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                form.sonuc.DegerAta(ve.hata);
                return form;
            }
            if (dr.Read())
            {
                form.yil = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.YIL, 0);
                form.belgeTur = (ENUMRevizeFormBelgeTur)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.BELGETUR, 0);
                form.belgeNo = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.BELGENO, 0);
                form.aciklama = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ACIKLAMA, "");
                form.tarih = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.TARIH);
                form.altIslemTur = (ENUMSeyyarGorevRevizeAltIslemTur)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ISLEMTUR, 0);
                form.durum = (ENUMRevizeFormDurum)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.DURUM, 0);

                form.onayaSunanAdSoyad = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYASUNANAD, "");
                form.onayaSunanUnvan = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYASUNANUNVAN, "");
                form.onaylayanAdSoyad = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYANAD, "");
                form.onaylayanUnvan = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYANUNVAN, "");

                //form.talepDurum = 0;
                form.kaydedenKullaniciKod = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.KAYDEDEN, "");
                form.onaylayanKullaniciKod = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYAN, "");
                form.icmalNo = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ICMALNO, 0);
            }
            else
                form = null;

            dr.Close();
            if (form != null && detayOku)
            {
                TNS.BHMv2.IBHMv2Servis servisBhmv2 = TNS.BHMv2.Arac.Tanimla();

                ObjectArray oa;
                TNSCollection c = BirimKodlariOlustur(tab, yil, belgeNo);
                if (c.Count > 0)
                    oa = ListeleSeyyarBirim(veriErisim, kullanan, yil, c, belgeNo, onaylilar);
                else
                    oa = new ObjectArray();
                form.birimDegisimler = oa.objeler;

                if (tumIl && form.birimDegisimler.Count > 0)
                {
                    BKD.TavanGenel ilkSatir = (BKD.TavanGenel)form.birimDegisimler[0];
                    TNSCollection tc = new TNSCollection();
                    tc.Add("_." + ilkSatir.kod1.Substring(1, 2));

                    oa = ListeleSeyyarBirim(ve, kullanan, yil, tc, belgeNo, onaylilar);
                    form.birimDegisimler = oa.objeler;
                }
            }
            ve.VeriErisimKapat();
            return form;
        }

        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, SeyyarGorevRevize form)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.ODEMEEMRI_MIF))
                return true;
            return false;
        }

        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(VeriErisim ve, Kullanici kullanan, SeyyarGorevRevize form)
        {
            Sonuc s = new Sonuc();
            if (form == null)
            {
                s.DegerAta("Lütfen kaydedilecek formu gönderiniz!");
                return s;
            }

            if (form.yil < 2000) s.DegerAta("Lütfen formu yılı alanını doldurunuz!");
            if (form.tarih == null || form.tarih.isNull) s.DegerAta("Lütfen tarih alanını doldurunuz!");
            if (string.IsNullOrWhiteSpace(form.aciklama)) s.DegerAta("Lütfen açıklama alanını doldurunuz!");

            if (form.belgeNo > 0)
            {
                SeyyarGorevRevize eskiForm = OkuSeyyarGorevRevizeForm(null, kullanan, form.yil, form.belgeTur, form.belgeNo, false, false, false, false);
                if (eskiForm == null) s.DegerAta("Eski form okunamadı!");
                else
                {
                    if (eskiForm.durum != ENUMRevizeFormDurum.Yeni && form.durum == ENUMRevizeFormDurum.Yeni
                        || eskiForm.durum != ENUMRevizeFormDurum.YeniTalep && form.durum == ENUMRevizeFormDurum.YeniTalep)
                        s.DegerAta("Eski form onaylı veya iptal edildiğinden değiştirilemez!");
                }
            }
            else //Yeni belge kaydedeiliyor, zaten yeni durumunda olan belge var mı kontrol edilsin?
            {
                TNSCollection birimListesi;
                if (form.durum == ENUMRevizeFormDurum.YeniTalep) //1 birimin bir tane talebi olabilir
                {
                    birimListesi = new TNSCollection();
                    BirimListesiOl(form, birimListesi);
                    if (birimListesi.Count == 0)
                    {
                        s.DegerAta("Kaydedilecek veri bulunamadı!");
                    }
                }
                else
                    birimListesi = null;
                if (s.islemSonuc)
                {
                    int yeniKayitSay = YeniKayitBul(ve, form, birimListesi);
                    if (yeniKayitSay > 0)
                        s.DegerAta("Birime ait yeni durumunda olan kayıt olduğundan, yeni belge üretemezsiniz, durumu yeni olan belgeyi, düzeltiniz!");
                }
            }

            TNSCollection liste = form.birimDegisimler;
            int satirSay = 0;
            if (liste != null)
            {

                for (int l = 0; l < liste.Count;)
                {
                    BKD.TavanGenel tg = (BKD.TavanGenel)liste[l];
                    if (tg.yil0 == 0 && tg.yil1 == 0 && tg.yil2 == 0)
                        liste.RemoveAt(l);
                    else
                        l++;
                }

                satirSay = SatirKontroluYap(ve, form, s);
            }
            if (satirSay == 0) s.DegerAta("Lütfen satırları doldurunuz!");
            return s;
        }

        private static int SatirKontroluYap(VeriErisim ve, SeyyarGorevRevize form, Sonuc s)
        {
            TNSCollection liste = form.birimDegisimler;
            int satirSay = 0;
            TABHRCSEYYARGOREVREVIZEFORM tab = new TABHRCSEYYARGOREVREVIZEFORM(ve);
            double tyil0 = 0;
            double tyil1 = 0;
            double tyil2 = 0;
            foreach (BKD.TavanGenel tg in liste)
            {
                if (!string.IsNullOrWhiteSpace(tg.kod1))
                {
                    satirSay++;
                    if (tg.yil0 == 0 && tg.yil1 == 0 && tg.yil2 == 0)
                        s.DegerAta(string.Format("{0} kodlu il/birim için eklenen veya düşülen değerlerinden en az birini giriniz!", tg.kod1));
                    if ((form.altIslemTur == ENUMSeyyarGorevRevizeAltIslemTur.IlEkleme || form.altIslemTur == ENUMSeyyarGorevRevizeAltIslemTur.DetayEkleme) && (tg.yil0 < 0 || tg.yil1 < 0 || tg.yil2 < 0))
                        s.DegerAta(string.Format("{0} kodlu il/birim için ekleme işleminde düşülen alanlarına değer girmeyiniz!", tg.kod1));
                    if ((form.altIslemTur == ENUMSeyyarGorevRevizeAltIslemTur.DetayIade) && (tg.yil0 > 0 || tg.yil1 > 0 || tg.yil2 > 0))
                        s.DegerAta(string.Format("{0} kodlu il/birim için tenkis işleminde eklenen alanlarına değer girmeyiniz!", tg.kod1));
                    if (tg.yil0 < 0 || tg.yil1 < 0 || tg.yil2 < 0)
                    {
                        double kalanMemur = 0;
                        double kalanIsci = 0;
                        double kalanSozlesmeli = 0;
                        SatirDurumBul(tab, form, tg, out kalanMemur, out kalanIsci, out kalanSozlesmeli);
                        if (tg.yil0 + kalanMemur < 0)
                            s.DegerAta(string.Format("{0} kodlu il/birim için kullanılabilir MEMUR miktarından daha fazla düşüm yapıyorsunuz!", tg.kod1));
                        if (tg.yil1 + kalanIsci < 0)
                            s.DegerAta(string.Format("{0} kodlu il/birim için kullanılabilir İŞÇİ miktarından daha fazla düşüm yapıyorsunuz!", tg.kod1));
                        if (tg.yil2 + kalanSozlesmeli < 0)
                            s.DegerAta(string.Format("{0} kodlu il/birim için kullanılabilir SÖZLEŞMELİ miktarından daha fazla düşüm yapıyorsunuz!", tg.kod1));
                    }

                    tyil0 += tg.yil0;
                    tyil1 += tg.yil1;
                    tyil2 += tg.yil2;
                }
            }
            if ((form.altIslemTur == ENUMSeyyarGorevRevizeAltIslemTur.IlRevize || form.altIslemTur == ENUMSeyyarGorevRevizeAltIslemTur.DetayRevize) && (tyil0 != 0 || tyil1 != 0 || tyil2 != 0))
                s.DegerAta("Revize işleminde eklenen düşülen toplamları eşit olmalıdır!");
            return satirSay;
        }

        private static int YeniKayitBul(VeriErisim ve, SeyyarGorevRevize form, TNSCollection birimListesi)
        {
            TABSeyyarGorev tsg = new TABSeyyarGorev(ve);
            return tsg.FormSayiBul(form, birimListesi);
        }

        private static void BirimListesiOl(SeyyarGorevRevize form, TNSCollection birimListesi)
        {
            foreach (BKD.TavanGenel tg in form.birimDegisimler)
            {
                if (!string.IsNullOrEmpty(tg.kod1))
                {
                    birimListesi.Add(tg.kod1);
                }
            }
        }

        private static void SatirDurumBul(TABHRCSEYYARGOREVREVIZEFORM tab, SeyyarGorevRevize form, BKD.TavanGenel tg, out double kalanMemur, out double kalanIsci, out double kalanSozlesmeli)
        {
            tab.SeyyarKalanBul(tg, form, out kalanMemur, out kalanIsci, out kalanSozlesmeli);
        }

        /// <summary>
        /// SeyyarGorevRevize nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SeyyarGorevRevize yapısında SeyyarGorevRevize:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        internal static Sonuc KaydetSeyyarGorevRevize(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevRevize form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz yoktur!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            s = AlanlarGecerliMi(ve, kullanan, form);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            s = KaydetSeyyarGorevRevizeIc(ve, kullanan, form, s);
            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = form.belgeNo.ToString();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc KaydetSeyyarGorevRevizeIc(VeriErisim ve, Kullanici kullanan, SeyyarGorevRevize form, Sonuc s)
        {
            TABHRCSEYYARGOREVREVIZEFORM tab = new TABHRCSEYYARGOREVREVIZEFORM(ve);
            if (form.belgeNo == 0)
            {
                form.belgeNo = tab.YeniBelgeNoBul(form);
                if (!string.IsNullOrWhiteSpace(ve.hata))
                    s.DegerAta(ve.hata);
                if (form.durum == ENUMRevizeFormDurum.Yeni)
                    form.icmalNo = tab.YeniIcmalNoBul(form);
            }
            else
            {
                if (s.islemSonuc)
                    s = tab.Sil(form);

                if (s.islemSonuc)
                    s = tab.SilDetaySeyyarGorev(form);
            }
            if (s.islemSonuc)
                s = tab.Ekle(form, kullanan.kullaniciKodu);

            foreach (BKD.TavanGenel tg in form.birimDegisimler)
            {
                if (s.islemSonuc)
                    if (!string.IsNullOrWhiteSpace(tg.kod1))
                    {
                        s = tab.EkleDetaySeyyarGorev(form, tg.kod1, 1, Convert.ToInt32(tg.yil0));
                        if (s.islemSonuc)
                            s = tab.EkleDetaySeyyarGorev(form, tg.kod1, 4, Convert.ToInt32(tg.yil1));
                        if (s.islemSonuc)
                            s = tab.EkleDetaySeyyarGorev(form, tg.kod1, 2, Convert.ToInt32(tg.yil2));
                    }
                    else
                        continue;
                else
                    break;
            }

            return s;
        }

        internal static BKD.TavanGenel SeyyarGorevBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string ilKod, int haricBelgeNoDagitilan)
        {
            TNSCollection ilKodlar = new TNSCollection();
            ilKodlar.Add("_" + ilKod);
            ObjectArray oa = SeyyarGorevBilgiBul(veriErisim, kullanan, yil, ilKodlar, "", haricBelgeNoDagitilan);
            if (oa.ObjeSayisi >= 1)
                return (BKD.TavanGenel)oa[0];
            return new BKD.TavanGenel();
        }

        internal static ObjectArray SeyyarGorevBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection ilKodlar, string belgeNo, int haricBelgeNoDagitilan)
        {
            string ilListe = ListeOl(ilKodlar);

            string[] sorgular = new string[3];
            string[] baglantilar = new string[3];
            string senaryoKod = "0300";
            if (belgeNo == "000000") belgeNo = "";
            //sorgular[0] = TABSeyyarGorev.IlTavanBulForm(yil, senaryoKod, ENUMTavanTur.tavanTurSeyyarIlTavan, false, 1, ilListe, belgeNo, true);
            //baglantilar[0] = "BaglantiSatiriHRC";

            //sorgular[1] = TABSeyyarGorev.IlTavanBulForm(yil, senaryoKod, ENUMTavanTur.tavanTurSeyyarIlTavan, false, 4, ilListe, belgeNo, true);
            //baglantilar[1] = "BaglantiSatiriHRC";

            //sorgular[2] = TABSeyyarGorev.IlTavanBulForm(yil, senaryoKod, ENUMTavanTur.tavanTurSeyyarIlTavan, false, 1, ilListe, belgeNo, false);
            //baglantilar[2] = "BaglantiSatiriHRC";

            //sorgular[3] = TABSeyyarGorev.IlTavanBulForm(yil, senaryoKod, ENUMTavanTur.tavanTurSeyyarIlTavan, false, 4, ilListe, belgeNo, false);
            //baglantilar[3] = "BaglantiSatiriHRC";

            sorgular[0] = TABSeyyarGorev.IlPlanlananBul(yil, 3, 1, ilListe, haricBelgeNoDagitilan, true);
            baglantilar[0] = "BaglantiSatiriHRC";

            sorgular[1] = TABSeyyarGorev.IlPlanlananBul(yil, 3, 4, ilListe, haricBelgeNoDagitilan, true);
            baglantilar[1] = "BaglantiSatiriHRC";

            sorgular[2] = TABSeyyarGorev.IlListeleSql(ilListe);
            baglantilar[2] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            ObjectArray oa = new ObjectArray();
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                BKD.TavanGenel g = new BKD.TavanGenel();
                g.kod1 = alAnahtar.StrAl(0);
                g.kod2 = belgeNo;

                //g.tavanMemur = alVeri.DblAl(0);
                //g.tavanIsci = alVeri.DblAl(1);

                //g.yil0 = alVeri.DblAl(2);
                //g.yil1 = alVeri.DblAl(3);

                g.dagitilanMemur = alVeri.DblAl(4);
                g.dagitilanIsci = alVeri.DblAl(5);
                g.ad1 = alVeri.StrAl(6);
                oa.objeler.Add(g);
            }
            return oa;
        }

        internal static ObjectArray SeyyarGorevIslemListele(Kullanici kullanan, SeyyarGorevRevize kistasForm, bool onaylilar)
        {
            string[] sorgular = new string[7];
            string[] baglantilar = new string[7];
            sorgular[0] = TABRaporlar.BirimListeleStrKYSql(null);
            baglantilar[0] = "BaglantiSatiriHRC";

            sorgular[1] = TABHRCSEYYARGOREVREVIZEFORM.ListeleDetaySql(kistasForm, 1);
            baglantilar[1] = "BaglantiSatiriHRC";

            sorgular[2] = TABHRCSEYYARGOREVREVIZEFORM.ListeleDetaySql(kistasForm, 4);
            baglantilar[2] = "BaglantiSatiriHRC";

            sorgular[3] = TABHRCSEYYARGOREVREVIZEFORM.ListeleDetaySql(kistasForm, 2);
            baglantilar[3] = "BaglantiSatiriHRC";

            sorgular[4] = TABSeyyarGorev.BirimPlanlananBul(kistasForm.yil, 3, 1, false, null, 0, true, onaylilar);
            baglantilar[4] = "BaglantiSatiriHRC";

            sorgular[5] = TABSeyyarGorev.BirimPlanlananBul(kistasForm.yil, 3, 4, false, null, 0, true, onaylilar);
            baglantilar[5] = "BaglantiSatiriHRC";

            sorgular[6] = TABSeyyarGorev.BirimPlanlananBul(kistasForm.yil, 3, 2, false, null, 0, true, onaylilar);
            baglantilar[6] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            ObjectArray oa = new ObjectArray();
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                BKD.TavanGenel g = new BKD.TavanGenel();
                g.kod1 = alAnahtar.StrAl(0);
                g.ad1 = alVeri.StrAl(0);

                g.yil0 = alVeri.DblAl(4);
                g.yil1 = alVeri.DblAl(5);
                g.yil2 = alVeri.DblAl(6);

                g.dagitilanMemur = alVeri.DblAl(7);
                g.dagitilanIsci = alVeri.DblAl(8);
                g.dagitilanSozlesmeli = alVeri.DblAl(9);
                oa.objeler.Add(g);
            }
            oa.objeler.Sort(new Siralama());
            return oa;
        }

        public class Siralama : IComparer
        {
            public int Compare(object x, object y)
            {
                BKD.TavanGenel g1 = null;
                BKD.TavanGenel g2 = null;

                try
                {
                    g1 = (BKD.TavanGenel)x;
                    g2 = (BKD.TavanGenel)y;
                }
                catch
                {
                }

                if (g1 == null) return -1;
                if (g2 == null) return 1;
                int kiyas;
                if ((kiyas = KurumsalYapi.IlkodBul(g1.kod1).CompareTo(KurumsalYapi.IlkodBul(g2.kod1))) != 0) return kiyas;
                if ((kiyas = KurumsalYapi.BirimTurBul(g1.kod1).CompareTo(KurumsalYapi.BirimTurBul(g2.kod1))) != 0) return kiyas;
                return g1.kod1.CompareTo(g2.kod1);
            }
        }

        internal static Sonuc DurumAtaSeyyarGorevRevize(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevRevize form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan, form))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz yoktur!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            SeyyarGorevRevize eklenecekBelge = null;
            SeyyarGorevRevize eskiForm = OkuSeyyarGorevRevizeForm(ve, kullanan, form.yil, form.belgeTur, form.belgeNo, form.durum == ENUMRevizeFormDurum.KarsilananTalep, false, false, false);
            if (eskiForm == null) s.DegerAta(string.Format("{0} numaralı form okunamadı!", form.belgeNo));
            else
            {
                if (form.durum == ENUMRevizeFormDurum.Iptal || form.durum == ENUMRevizeFormDurum.IptalTalep)
                {
                    if (eskiForm.durum == ENUMRevizeFormDurum.Yeni)
                        form.durum = ENUMRevizeFormDurum.Iptal;
                    else if (eskiForm.durum == ENUMRevizeFormDurum.YeniTalep)
                        form.durum = ENUMRevizeFormDurum.IptalTalep;
                    else
                    {
                        s.DegerAta("Yeni belgeler iptal edilebilir!");
                    }
                }
                else if (form.durum == ENUMRevizeFormDurum.Yeni || form.durum == ENUMRevizeFormDurum.YeniTalep)
                {
                    if (!(eskiForm.durum == ENUMRevizeFormDurum.OMK && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                    {
                        s.DegerAta("Sadece ön mali kontrole giden belgelerin durumu yeni yapılabilir!");
                    }
                }
                else if (form.durum == ENUMRevizeFormDurum.Onayli)
                {
                    if (!(eskiForm.durum == ENUMRevizeFormDurum.OMK && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                    {
                        s.DegerAta("Sadece ön mali kontrole giden belgelerin durumu onaylanalabilir!");
                    }
                }
                else if (form.durum == ENUMRevizeFormDurum.OMK)
                {
                    if (!(eskiForm.durum == ENUMRevizeFormDurum.Yeni && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                    {
                        s.DegerAta("Sadece yeni belgeler ön mali kontrole gönderilebilir!");
                    }
                }
                else if (form.durum == ENUMRevizeFormDurum.KarsilananTalep)
                {
                    if (!(eskiForm.durum == ENUMRevizeFormDurum.YeniTalep && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                    {
                        s.DegerAta("Sadece yeni belgeler, işleme çevrilebilir!");
                    }
                    else
                    {
                        eklenecekBelge = YeniBelgeBul(ve, kullanan, form.yil, s);
                        if (eklenecekBelge == null && s.islemSonuc)
                        {
                            eklenecekBelge = new SeyyarGorevRevize();
                            eklenecekBelge.yil = form.yil;
                            eklenecekBelge.belgeTur = ENUMRevizeFormBelgeTur.DetayRevize;
                            eklenecekBelge.aciklama = "Talep karşılama";
                            eklenecekBelge.belgeNo = 0;
                            eklenecekBelge.altIslemTur = ENUMSeyyarGorevRevizeAltIslemTur.DetayRevize;
                            eklenecekBelge.durum = ENUMRevizeFormDurum.Yeni;
                            eklenecekBelge.icmalNo = 0;
                            eklenecekBelge.tarih = new TNSDateTime(DateTime.Now.Date);
                        }
                        if (eklenecekBelge != null)
                        {
                            eklenecekBelge.SatirlariEkle(eskiForm);
                            eklenecekBelge.YedekAyarla();
                            SatirKontroluYap(ve, eklenecekBelge, s);
                            if (!s.islemSonuc && eklenecekBelge.belgeNo > 0)
                                s.DegerAta($"{eklenecekBelge.belgeNo} numaralı belge değiştirilemedi!");
                        }
                        else if (s.islemSonuc)
                            s.DegerAta("Karşılayacak form oluşturulamadı!");
                    }
                }
            }
            if (!s.islemSonuc) return s;
            TABHRCSEYYARGOREVREVIZEFORM tab = new TABHRCSEYYARGOREVREVIZEFORM(ve);

            ve.BeginTransaction();

            if (eklenecekBelge != null)
            {
                KaydetSeyyarGorevRevizeIc(ve, kullanan, eklenecekBelge, s);
            }

            if (s.islemSonuc)
                s = tab.BelgeOnayla(form, kullanan.kullaniciKodu, eklenecekBelge == null ? 0 : eklenecekBelge.icmalNo);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        private static SeyyarGorevRevize YeniBelgeBul(VeriErisim ve, Kullanici kullanan, int yil, Sonuc s)
        {
            TABHRCSEYYARGOREVREVIZEFORM tabsg = new TABHRCSEYYARGOREVREVIZEFORM(ve);
            SeyyarGorevRevize kistas = new SeyyarGorevRevize();
            kistas.yil = yil;
            kistas.durum = ENUMRevizeFormDurum.Yeni;
            ObjectArray oa = ListeleSeyyarGorevRevize(ve, kullanan, kistas);
            if (!oa.sonuc.islemSonuc)
            {
                s.DegerAta(oa.sonuc.hataStr);
            }
            else if (oa.ObjeSayisi > 0)
            {
                SeyyarGorevRevize okunan = (SeyyarGorevRevize)oa.objeler[0];
                return OkuSeyyarGorevRevizeForm(ve, kullanan, okunan.yil, okunan.belgeTur, okunan.belgeNo, true, false, false, false);
            }
            return null;
        }

        internal static ObjectArray ListeleSeyyarBirim(VeriErisim veriErisim, Kullanici kullanan, int yil, TNSCollection kyKodListesi, int belgeNo, bool onaylilar)
        {
            string[] sorgular = new string[10];
            string[] baglantilar = new string[10];

            sorgular[0] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 1, false, kyKodListesi, belgeNo, true, onaylilar);
            baglantilar[0] = "BaglantiSatiriHRC";

            sorgular[1] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 4, false, kyKodListesi, belgeNo, true, onaylilar);
            baglantilar[1] = "BaglantiSatiriHRC";

            sorgular[2] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 2, false, kyKodListesi, belgeNo, true, onaylilar);
            baglantilar[2] = "BaglantiSatiriHRC";


            sorgular[3] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 1, false, kyKodListesi, belgeNo, false, onaylilar);
            baglantilar[3] = "BaglantiSatiriHRC";

            sorgular[4] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 4, false, kyKodListesi, belgeNo, false, onaylilar);
            baglantilar[4] = "BaglantiSatiriHRC";

            sorgular[5] = TABSeyyarGorev.BirimPlanlananBul(yil, 3, 2, false, kyKodListesi, belgeNo, false, onaylilar);
            baglantilar[5] = "BaglantiSatiriHRC";


            sorgular[6] = TABSeyyarGorev.SurecKullanilanBulSql(yil, 1, TNS.HRC.Arac.SEYYARGOREVISLEM, 0, null, kyKodListesi);
            baglantilar[6] = "BaglantiSatiriHRC";

            sorgular[7] = TABSeyyarGorev.SurecKullanilanBulSql(yil, 4, TNS.HRC.Arac.SEYYARGOREVISLEM, 0, null, kyKodListesi);
            baglantilar[7] = "BaglantiSatiriHRC";

            sorgular[8] = TABSeyyarGorev.SurecKullanilanBulSql(yil, 2, TNS.HRC.Arac.SEYYARGOREVISLEM, 0, null, kyKodListesi);
            baglantilar[8] = "BaglantiSatiriHRC";

            sorgular[9] = TABRaporlar.BirimListeleStrKYSql(kyKodListesi);
            baglantilar[9] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            ObjectArray oa = new ObjectArray();
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                BKD.TavanGenel g = new BKD.TavanGenel();
                g.kod1 = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.99.999.99999", alAnahtar.StrAl(0));
                g.dagitilanMemur = alVeri.DblAl(0); //sorgular[0]
                g.dagitilanIsci = alVeri.DblAl(1);  //sorgular[1]
                g.dagitilanSozlesmeli = alVeri.DblAl(2); //sorgular[2]

                if (belgeNo > 0)
                {
                    g.yil0 = alVeri.DblAl(3); //sorgular[3]
                    g.yil1 = alVeri.DblAl(4); //sorgular[4]
                    g.yil2 = alVeri.DblAl(5); //sorgular[5]
                }
                else
                {
                    g.yil0 = 0;
                    g.yil1 = 0;
                    g.yil2 = 0;
                }
                g.kullanilanMemur = alVeri.DblAl(6); //sorgular[6]. ilk alan
                g.kullanilanIsci = alVeri.DblAl(8); //sorgular[7]. ilk alan
                g.kullanilanSozlesmeli = alVeri.DblAl(10); //sorgular[8] ilk alan

                g.ad1 = alVeri.StrAl(12);
                g.ad2 = "";
                //g.ad1 = TNS.MUH.Arac.BirimAdOl(g.ad1, g.ad2);
                oa.objeler.Add(g);
            }
            return oa;
        }

        //internal static ObjectArray HarcamaBirimiListele(VeriErisim veriErisim, Kullanici kullanan, MuhasebeBirim muhasebeBirim, bool kurumsalYapili)
        //{
        //    ObjectArray oa = new ObjectArray();
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABRaporlar t = new TABRaporlar(ve);
        //    IDataReader dr = t.HarcamaBirimiListele(muhasebeBirim, kurumsalYapili);
        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            MuhasebeBirim oku = new MuhasebeBirim();
        //            oku.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, 0, ""), 0);
        //            oku.birimKod = VeriErisim.DegerAl(dr, 1, "");
        //            oku.kurKod = VeriErisim.DegerAl(dr, 2, "");
        //            oku.muhasebeAd = VeriErisim.DegerAl(dr, 3, "");
        //            oku.birimAd = VeriErisim.DegerAl(dr, 4, "");
        //            oku.kyKod = VeriErisim.DegerAl(dr, 5, "");
        //            oku.kyAd = VeriErisim.DegerAl(dr, 6, "");
        //            oa.Ekle(oku);
        //        }
        //        dr.Close();
        //    }
        //    else
        //        oa.sonuc.DegerAta(ve.hata);
        //    return oa;
        //}

        internal static ObjectArray ListeleSeyyarGorevRevize(VeriErisim veriErisim, Kullanici kullanan, SeyyarGorevRevize kistasForm)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCSEYYARGOREVREVIZEFORM tab = new TABHRCSEYYARGOREVREVIZEFORM(ve);
            ObjectArray oa = new ObjectArray();
            IDataReader dr = tab.Listele(kistasForm);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                SeyyarGorevRevize form = new SeyyarGorevRevize();
                form.yil = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.YIL, 0);
                form.belgeTur = (ENUMRevizeFormBelgeTur)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.BELGETUR, 0);
                form.belgeNo = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.BELGENO, 0);
                form.aciklama = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ACIKLAMA, "");
                form.tarih = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.TARIH);
                form.altIslemTur = (ENUMSeyyarGorevRevizeAltIslemTur)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ISLEMTUR, 0);
                form.durum = (ENUMRevizeFormDurum)VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.DURUM, 0);

                form.onayaSunanAdSoyad = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYASUNANAD, "");
                form.onayaSunanUnvan = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYASUNANUNVAN, "");
                form.onaylayanAdSoyad = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYANAD, "");
                form.onaylayanUnvan = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYANUNVAN, "");
                //form.talepDurum = 0; // VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.TALEPDURUM, 0);
                form.kaydedenKullaniciKod = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.KAYDEDEN, "");
                form.onaylayanKullaniciKod = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ONAYLAYAN, "");
                form.icmalNo = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ICMALNO, 0);


                form.memurToplam = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ICMALNO + 1, 0);
                form.isciToplam = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ICMALNO + 2, 0);
                form.sozlesmeliToplam = VeriErisim.DegerAl(dr, (int)TABHRCSEYYARGOREVREVIZEFORM.ENUMAlanlar.ICMALNO + 3, 0);

                oa.Ekle(form);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc DurumDegistirSeyyarGorevRevize(VeriErisim veriErisim, Kullanici kullanan, TNSCollection belgeler)
        {
            Sonuc s = new Sonuc();
            if (belgeler == null || belgeler.Count == 0)
            {
                s.DegerAta("İşlem yapılacak belgeleri seçiniz!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            for (int i = 0; i < belgeler.Count; i++)
            {
                s = DurumAtaSeyyarGorevRevize(ve, kullanan, (SeyyarGorevRevize)belgeler[i]);
                if (!s.islemSonuc) break;
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc OncekiAsamadanAl(VeriErisim veriErisim, Kullanici kullanan, int hedefYil, int hedefAsama)
        {
            Sonuc s = new Sonuc();
            int kaynakAsama = hedefAsama - 1;
            int kaynakYil;
            if (kaynakAsama == 0)
            {
                kaynakAsama = 3;
                kaynakYil = hedefYil - 1;
            }
            else
                kaynakYil = hedefYil;

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("İşlemi yapma yetkiniz bulunmamaktadır!");
            }

            if (hedefAsama < 1 || hedefAsama > 3)
            {
                s.DegerAta("Hatalı aşama: 1, 2 veya 3 seçiniz!");
            }

            if (s.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);

                Sonuc sVeri = tabsg.OncekiAsamadanAl(kaynakYil, kaynakAsama, hedefYil, hedefAsama);
                if (!sVeri.islemSonuc)
                    s.DegerAta("Kopyalama işlemi tamamlanamadı!");
                ve.VeriErisimKapat();
            }
            return s;
        }

        internal static Sonuc DetayHesapla(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("İşlemi yapma yetkiniz bulunmamaktadır!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray liste = ListeleSeyyarIlTavan(ve, kullanan, yil, "0300", null, true, true);

            if (liste.sonuc.islemSonuc)
            {
                double isciTop = 0;
                double memurTop = 0;
                double sozTop = 0;
                foreach (BKD.TavanGenel item in liste.objeler)
                {
                    memurTop += item.ekOdenek;
                    isciTop += item.ekOdenek1;
                    sozTop += item.ekOdenek2;
                }
                if (s.islemSonuc)
                    s = DetayHesapla(ve, kullanan, yil, asama, liste.objeler, memurTop, isciTop, sozTop);
            }
            else
                s = liste.sonuc;

            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc DetayHesapla(VeriErisim ve, Kullanici kullanan, int yil, int asama, TNSCollection liste, double memurTop, double isciTop, double sozTop)
        {
            Sonuc s = new Sonuc();
            TABSeyyarGorev tabsg = new TABSeyyarGorev(ve);
            int onayliSayisi = tabsg.SeyyarGorevOnayliVarMi(ve, yil, "", asama, true);
            if (onayliSayisi > 0)
            {
                s.DegerAta("Onaylı kayıtlar olduğundan dağıtım işlemi yapılamaz!");
                return s;
            }

            ve.BeginTransaction();
            try
            {
                foreach (BKD.TavanGenel item in liste)
                {
                    if (item.ekOdenek != item.sgb0 || item.ekOdenek1 != item.sgb1 || item.ekOdenek2 != item.sgb2)
                    {
                        IlIslem(tabsg, kullanan, yil, asama, item, s);
                        if (!s.islemSonuc) break;
                    }
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        private static void IlIslem(TABSeyyarGorev tabsg, Kullanici kullanan, int yil, int asama, BKD.TavanGenel item, Sonuc s)
        {
            ObjectArray adamGunListesi = new ObjectArray();

            string detayKod;
            if (item.kod1 == "99")
                detayKod = "900";
            else
                detayKod = "_" + item.kod1;

            SeyyarGorevAdamGunTespitListeleIc(tabsg, adamGunListesi, yil, detayKod, asama);
            if (!adamGunListesi.sonuc.islemSonuc)
            {
                s.DegerAta(adamGunListesi.sonuc.hataStr);
                return;
            }
            if (item.ekOdenek != item.sgb0)
                RakamHesapla(adamGunListesi, ENUMPersonelIstihdamSekli.MEMUR, (int)item.ekOdenek, (int)item.sgb0, s, 10);
            if (item.ekOdenek1 != item.sgb1 && s.islemSonuc)
                RakamHesapla(adamGunListesi, ENUMPersonelIstihdamSekli.ISCI, (int)item.ekOdenek1, (int)item.sgb1, s, 10);
            if (item.ekOdenek2 != item.sgb2 && s.islemSonuc)
                RakamHesapla(adamGunListesi, ENUMPersonelIstihdamSekli.SOZLESMELI, (int)item.ekOdenek1, (int)item.sgb2, s, 10);
            if (s.islemSonuc)
            {
                KaydetIc(tabsg, (SeyyarGorevI[])adamGunListesi.objeler.ToArray(typeof(SeyyarGorevI)), yil, asama, kullanan, s);
            }
        }

        private static void RakamHesapla(ObjectArray adamGunListesi, ENUMPersonelIstihdamSekli isSek, int olacak, int olan, Sonuc s, int bolen)
        {
            try
            {
                olacak = olacak / bolen;
                olan = olan / bolen;
                if (olan < 0 || olacak < 0)
                    throw new Exception("Negatif rakamlar bulundu, işlem iptal edildi.");
                int rakamSay = 0;
                int toplam = 0;
                bool sifirDahil;
                if (olan > 0)
                {
                    double carpan = olacak * 1.0 / olan;
                    foreach (SeyyarGorevI item in adamGunListesi.objeler)
                    {
                        int rakam = item.GunBul(isSek) / bolen;
                        if (rakam < 0)
                            throw new Exception("Negatif rakamlar bulundu (detay), işlem iptal edildi.");
                        if (rakam > 1 || carpan > 1)
                        {
                            rakamSay++;
                            rakam = Convert.ToInt32(OrtakFonksiyonlar.Yuvarla(rakam * carpan, 0));
                            item.GunYaz(isSek, rakam);
                        }
                        toplam += rakam;
                    }
                    sifirDahil = false;
                }
                else
                {
                    rakamSay = adamGunListesi.ObjeSayisi;
                    sifirDahil = true;
                }

                int kalan = olacak - toplam;
                if (rakamSay > 0)
                {
                    while (kalan != 0)
                    {
                        int adim = (int)OrtakFonksiyonlar.YuvarlaYukari(kalan * 1.0 / rakamSay);
                        if (adim == 0)
                        {
                            if (kalan < 0) adim = -1;
                            else adim = 1;
                        }
                        foreach (SeyyarGorevI item in adamGunListesi.objeler)
                        {
                            int rakam = item.GunBul(isSek);
                            if (sifirDahil || rakam > 0)
                            {
                                int icAdim = adim;

                                if (kalan < 0)
                                {
                                    if (kalan > icAdim)
                                        icAdim = kalan;
                                }
                                else if (kalan > 0)
                                {
                                    if (kalan < icAdim)
                                        icAdim = kalan;
                                }
                                else
                                    break;
                                rakam += icAdim;
                                kalan -= icAdim;
                                item.GunYaz(isSek, rakam);
                                if (kalan == 0) break;
                            }
                        }
                    }
                    if (bolen > 1)
                    {
                        foreach (SeyyarGorevI item in adamGunListesi.objeler)
                        {
                            item.GunYaz(isSek, item.GunBul(isSek) * bolen);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
            }
        }

        internal ObjectArray IlListele(Kullanici kullanan, string ilkod)
        {
            string[] sorgular = new string[1];
            string[] baglantilar = new string[1];

            sorgular[0] = TABSeyyarGorev.IlListeleSql(ilkod);
            baglantilar[0] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            ObjectArray oa = new ObjectArray();
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                Il f = new Il();

                f.kod = alAnahtar.StrAl(0);
                f.ad = alVeri.StrAl(0);

                oa.objeler.Add(f);
            }
            return oa;
        }
    }
}
