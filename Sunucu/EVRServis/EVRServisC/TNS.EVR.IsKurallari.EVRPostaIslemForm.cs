using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Posta işlemleriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRPostaIslemForm
    {
        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlemini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, PostaIslem form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI138, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABPostaIslemForm to = new TABPostaIslemForm(ve);
            TABGenel tabGenel = new TABGenel(ve);
            TABEvrak tabEvrak = new TABEvrak(ve);

            ve.BeginTransaction();
            try
            {
                to.Form = form;
                if (form.belgeNo == 0)
                {
                    string sayacKod = "EVRPOSTAISLEM" + form.yil.ToString() + EVRBirim.EvrakKokBirimGetir(ve, form.evrakBirimKodu);
                    form.belgeNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "000000"), 0);
                    to.YeniKayit = true;
                }

                sonuc = to.Kaydet();
                if (sonuc.islemSonuc)
                {
                    ArrayList listEvrEvrakNo = new ArrayList();

                    int siraNo = 1;
                    foreach (PostaIslemDetay detay in form.detay)
                    {
                        detay.siraNo = siraNo++;

                        sonuc = to.KaydetDetay(detay);

                        if (!sonuc.islemSonuc)
                            break;

                        if (!listEvrEvrakNo.Contains(detay.evrEvrakNo))
                            listEvrEvrakNo.Add(detay.evrEvrakNo);
                    }

                    //İletim tarihi güncelle
                    if (sonuc.islemSonuc && listEvrEvrakNo.Count > 0)
                    {
                        foreach (string evrEvrakNo in listEvrEvrakNo)
                        {
                            sonuc = tabEvrak.Guncelle(new EvrakSinif() { evrEvrakNo = evrEvrakNo, iletimTarihi = form.belgeTarih });
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit(); sonuc.bilgiStr += EvrakI.EVRI139;
                    sonuc.anahtar = form.belgeNo.ToString();
                }
                else
                    ve.Rollback(); sonuc.hataStr = EvrakI.EVRI140 + "<br>" + sonuc.hataStr;
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D40081D0-C531-4BD3-B58E-8D0318781BFA} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlem listesini PostaIslem nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>PostaIslem nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, PostaIslem form, bool detay)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                //#############
                TABPostaIslemForm to = new TABPostaIslemForm(ve);
                to.Form = form;
                IDataReader r = to.Listele(detay);

                if (to.Hata() == "")
                {
                    PostaIslem bilgi = new PostaIslem();
                    string eskiEvrakBirimKod = "";
                    int eskiYil = 0;
                    int eskiBelgeNo = 0;
                    int index = ((int)TABPostaIslemForm.ENUMAlanlar.KULLANICIADI) + 1 - 3;
                    int detayIndex = (int)TABPostaIslemForm.ENUMAlanlarDetay.ZARFNO;

                    while (r.Read())
                    {
                        string evrakBirimKod = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.EVRAKBIRIMKODU, "");
                        int yil = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.YIL, 0);
                        int belgeNo = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.BELGENO, 0);

                        if (eskiEvrakBirimKod != evrakBirimKod || eskiYil != yil || eskiBelgeNo != belgeNo)
                        {
                            if (eskiYil > 0)
                                hVAN.objeler.Add(bilgi);

                            bilgi = new PostaIslem();
                            bilgi.evrakBirimKodu = evrakBirimKod;
                            bilgi.yil = yil;
                            bilgi.belgeNo = belgeNo;
                            bilgi.belgeTarih = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.BELGETARIH);
                            bilgi.kullaniciKodu = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.KULLANICIKODU, "");
                            bilgi.kayitTarih = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.KAYITTARIH);
                            bilgi.kullaniciAdi = VeriErisim.DegerAl(r, (int)TABPostaIslemForm.ENUMAlanlar.KULLANICIADI, "");
                        }
                        if (detay)
                        {
                            PostaIslemDetay bilgi2 = new PostaIslemDetay();
                            bilgi2.evrakBirimKodu = evrakBirimKod;
                            bilgi2.yil = yil;
                            bilgi2.belgeNo = belgeNo;
                            bilgi2.evrEvrakNo = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.EVREVRAKNO, "");
                            bilgi2.evrakNo = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.EVRAKNO, 0);
                            bilgi2.gidecegiYer = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.GIDECEGIYER, "");
                            bilgi2.postaTur = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.POSTATUR, 0);
                            bilgi2.fiyatNormal = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.FIYATNORMAL, (decimal)0.00);
                            bilgi2.fiyatPostane = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.FIYATPOSTANE, (decimal)0.00);
                            bilgi2.barkod = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.BARKOD, "");
                            bilgi2.sayfaNo = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.SAYFANO, 0);
                            bilgi2.zarfNo = VeriErisim.DegerAl(r, index + (int)TABPostaIslemForm.ENUMAlanlarDetay.ZARFNO, 0);

                            bilgi2.konu = VeriErisim.DegerAl(r, index + detayIndex + 1, "");
                            bilgi2.hazirlayanBirimKodu = VeriErisim.DegerAl(r, index + detayIndex + 2, "");
                            bilgi2.hazirlayanBirimAdi = VeriErisim.DegerAl(r, index + detayIndex + 3, "");
                            bilgi2.gidenEvrakTarih = VeriErisim.DegerAl(r, index + detayIndex + 4);
                            bilgi2.dagitimBirimKodu = VeriErisim.DegerAl(r, index + detayIndex + 5, "");
                            bilgi2.dagitimBirimAdi = VeriErisim.DegerAl(r, index + detayIndex + 6, "");
                            bilgi2.baskanlikEvrakNo = VeriErisim.DegerAl(r, index + detayIndex + 7, "");
                            bilgi2.bakanlikEvrakNo = VeriErisim.DegerAl(r, index + detayIndex + 8, "");
                            bilgi2.ekBilgisi = VeriErisim.DegerAl(r, index + detayIndex + 9, "");
                            bilgi2.aciklama = VeriErisim.DegerAl(r, index + detayIndex  + 10, "");

                            bilgi.detay.Add(bilgi2);
                        }
                        eskiEvrakBirimKod = bilgi.evrakBirimKodu;
                        eskiYil = bilgi.yil;
                        eskiBelgeNo = bilgi.belgeNo;

                    }
                    r.Close();
                    if (eskiYil > 0)
                        hVAN.objeler.Add(bilgi);
                }
                //#############

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{0BB8869A-B603-44F2-BCB4-E889A8BAC712} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta işlem bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI141, "");

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen PostaIslem nesnesine göre, evrak bilgilerini PostaIslemDetay nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>PostaIslemDetay nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray ListeleEvrak(VeriErisim veriErisim, PostaIslem form)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                //#############
                TABPostaIslemForm to = new TABPostaIslemForm(ve);
                to.Form = form;
                IDataReader r = to.ListeleEvrak();



                PostaIslemDetay detay = new PostaIslemDetay();
                string evrEvrakNo = "", evrEvrakNoEski = "", dagitimAd = "", dagitimAd2 = "";

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        evrEvrakNo = VeriErisim.DegerAl(r, 0, "");
                        dagitimAd = VeriErisim.DegerAl(r, 4, "");
                        dagitimAd2 = VeriErisim.DegerAl(r, 7, "");

                        if (evrEvrakNo == evrEvrakNoEski)
                        {
                            if (!string.IsNullOrEmpty(dagitimAd))
                            {
                                if (!string.IsNullOrEmpty(dagitimAd2))
                                    detay.dagitimListesi.objeler.Add(dagitimAd2);
                                else
                                    detay.dagitimListesi.objeler.Add(dagitimAd);
                            }
                            continue;
                        }

                        detay = new PostaIslemDetay();
                        detay.evrEvrakNo = evrEvrakNo;
                        detay.evrakNo = VeriErisim.DegerAl(r, 1, 0);
                        detay.gidecegiYer = VeriErisim.DegerAl(r, 2, "");
                        detay.ekBilgisi = VeriErisim.DegerAl(r, 3, "");
                        detay.gizlilik = VeriErisim.DegerAl(r, 5, 0);
                        detay.gizlilikAd = VeriErisim.DegerAl(r, 6, "");

                        if (!string.IsNullOrEmpty(dagitimAd))
                        {
                            if (!string.IsNullOrEmpty(dagitimAd2))
                                detay.dagitimListesi.objeler.Add(dagitimAd2);
                            else
                                detay.dagitimListesi.objeler.Add(dagitimAd);
                        }

                        hVAN.objeler.Add(detay);
                        evrEvrakNoEski = evrEvrakNo;

                    }
                    r.Close();
                }
                //#############

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{96061E83-2975-4E3B-B774-DE9F62DF6EC4} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return hVAN;
        }
    }
}
