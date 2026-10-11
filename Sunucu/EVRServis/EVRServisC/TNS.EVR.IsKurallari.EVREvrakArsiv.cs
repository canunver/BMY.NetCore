using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;
using EVRServisC;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakArsiv
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(form.prEvrakNo))
                {
                    string sayacKod = "EVRARSIV" + DateTime.Now.Year;
                    form.prEvrakNo = DateTime.Now.Year + tabGenel.BelgeNoAl(sayacKod, "00000000");
                }
                else
                    sonuc = tab.Sil(form);

                sonuc = tab.Kaydet(form);
                if (sonuc.islemSonuc)
                {
                    //Evrak Arşiv Meta Kaydet
                    sonuc = tab.SilMeta(form.prEvrakNo);
                    if (sonuc.islemSonuc && form.meta != null)
                    {
                        foreach (EvrakArsivMeta meta in form.meta)
                        {
                            meta.prEvrakNo = form.prEvrakNo;
                            sonuc = tab.Kaydet(meta);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }

                    //Evrak Arsiv Dosya Kaydet
                    sonuc = tab.SilDosya(form.prEvrakNo);
                    if (sonuc.islemSonuc && form.dosya != null)
                    {
                        form.dosya.prEvrakNo = form.prEvrakNo;
                        form.dosya.arsivKod = form.arsiv.kod;
                        sonuc = tab.Kaydet(form.dosya);
                    }

                    if (sonuc.islemSonuc)
                    {
                        EvrakArsivTarihce formTarihce = new EvrakArsivTarihce();
                        formTarihce.islem = (int)ENUMEvrakArsivTarihceDurum.YENI;
                        formTarihce.prevrakNo = form.prEvrakNo;
                        sonuc = EVREvrakArsiv.KaydetTarihce(ve, kullanan, formTarihce);
                    }
                }

                //if (tab.KayitKontrol(form.prEvrakNo, form.arsiv.kod) == 0)
                //    sonuc = tab.Kaydet(form);
                //else
                //    sonuc.hataStr = "Seçilen evrak ikinci kez eklenemez!" + "<br>" + sonuc.hataStr;

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.prEvrakNo;
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F98D36C3-7DCE-46E4-B28B-B4F9E71A53A2} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakArsiv[] formList)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                //Silme İşlemi için
                {
                    Hashtable prEvrakNo = new Hashtable();
                    foreach (EvrakArsiv form in formList)
                    {
                        if (prEvrakNo.ContainsKey(form.prEvrakNo))
                            continue;
                        prEvrakNo.Add(form.prEvrakNo, "");

                    }
                    foreach (DictionaryEntry prNo in prEvrakNo)
                    {
                        sonuc = tab.IptalEt(new EvrakArsiv() { prEvrakNo = (string)prNo.Key });
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc)
                {
                    foreach (EvrakArsiv form in formList)
                    {
                        sonuc = tab.Kaydet(form);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }


                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{2615CD23-530A-4C8E-B40D-C86F6C8262EE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Guncelle(VeriErisim veriErisim, Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            if (string.IsNullOrEmpty(form.prEvrakNo))
            {
                sonuc.hataStr = "prEvrakNo boş bırakılamaz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            ve.BeginTransaction();
            try
            {
                if (form.meta != null)
                {
                    //Evrak Arşiv Meta Güncelleme
                    sonuc = tab.SilMeta(form.prEvrakNo);
                    if (sonuc.islemSonuc)
                    {
                        foreach (EvrakArsivMeta meta in form.meta)
                        {
                            meta.prEvrakNo = form.prEvrakNo;
                            sonuc = tab.Kaydet(meta);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        EvrakArsivTarihce formTarihce = new EvrakArsivTarihce();
                        formTarihce.islem = (int)ENUMEvrakArsivTarihceDurum.DEGISIKLIKMETA;
                        formTarihce.prevrakNo = form.prEvrakNo;
                        sonuc = EVREvrakArsiv.KaydetTarihce(ve, kullanan, formTarihce);
                    }
                }


                if (form.dosya != null && !string.IsNullOrEmpty(form.dosya.dosyaYol) && !string.IsNullOrEmpty(form.prEvrakNo))
                {
                    form.dosya.prEvrakNo = form.prEvrakNo;
                    form.dosya.arsivKod = form.arsiv.kod;

                    //Evrak Arşiv Dosya Bilgisi Güncelleme
                    sonuc = tab.SilDosya(form.prEvrakNo);
                    if (sonuc.islemSonuc)
                        sonuc = tab.Kaydet(form.dosya);

                    if (sonuc.islemSonuc)
                    {
                        EvrakArsivTarihce formTarihce = new EvrakArsivTarihce();
                        formTarihce.islem = (int)ENUMEvrakArsivTarihceDurum.DEGISIKLIKDOSYA;
                        formTarihce.prevrakNo = form.prEvrakNo;
                        sonuc = EVREvrakArsiv.KaydetTarihce(ve, kullanan, formTarihce);
                    }
                }


                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{724B0FD9-9608-4F54-9F35-634CA3F12D88} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakArsiv form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (string.IsNullOrEmpty(form.prEvrakNo))
            {
                sonuc.hataStr = "prEvrakNo boş bırakılamaz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            ve.BeginTransaction();
            try
            {
                //Silme İşlemi Kapatıldı. 29.05.2013 Hsyn

                //sonuc = tab.SilMeta(form.prEvrakNo);
                //if (sonuc.islemSonuc)
                //    sonuc = tab.SilDosya(form.prEvrakNo);
                //if (sonuc.islemSonuc)
                //    sonuc = tab.Sil(form);

                sonuc = tab.IptalEt(form);
                if (sonuc.islemSonuc)
                {
                    EvrakArsivTarihce formTarihce = new EvrakArsivTarihce();
                    formTarihce.islem = (int)ENUMEvrakArsivTarihceDurum.IPTAL;
                    formTarihce.prevrakNo = form.prEvrakNo;
                    sonuc = EVREvrakArsiv.KaydetTarihce(ve, kullanan, formTarihce);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{330F34EE-92B9-4C3A-83EC-3B10381EDBC6} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakSinif[] Listele(VeriErisim veriErisim, Kullanici kullanan, Arsiv form)
        {
            ObjectArray liste = EVREvrak.Listele(veriErisim, null, new EvrakSinif() { arsiv = form });

            return (EvrakSinif[])liste.objeler.ToArray(typeof(EvrakSinif));
        }

        public static EvrakArsiv[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakArsiv form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakArsiv();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.PREVRAKNO, "");
                    form.arsiv.kod = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.ARSIVKOD, 0);
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.ACIKLAMA, "");
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.ISLEMTARIH);
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlar.ISLEMYAPANAD, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakArsiv[])liste.objeler.ToArray(typeof(EvrakArsiv));
        }

        public static EvrakArsivMeta[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakArsivMeta form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int index = (int)TABEvrakArsiv.ENUMAlanlarMeta.METADEGER2;
                while (dr.Read())
                {
                    form = new EvrakArsivMeta();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarMeta.PREVRAKNO, "");
                    form.meta.kod = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarMeta.METAKOD, 0);
                    form.metaDeger = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarMeta.METADEGER, "");
                    form.metaDeger2 = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarMeta.METADEGER2, "");
                    form.meta.ad = VeriErisim.DegerAl(dr, index + 1, "");
                    form.meta.tip = VeriErisim.DegerAl(dr, index + 2, 0);
                    form.meta.zorunlu = VeriErisim.DegerAl(dr, index + 3, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakArsivMeta[])liste.objeler.ToArray(typeof(EvrakArsivMeta));
        }

        public static EvrakArsivDosya[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakArsivDosya form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakArsivDosya();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.PREVRAKNO, "");
                    form.arsivKod = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.ARSIVKOD, 0);
                    form.dosyaAd = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.DOSYAAD, "");
                    form.dosyaYol = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.DOSYAYOL, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakArsivDosya[])liste.objeler.ToArray(typeof(EvrakArsivDosya));
        }

        public static EvrakArsivDosya[] Listele(VeriErisim veriErisim, Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList)
        {
            ObjectArray liste = new ObjectArray();

            if (formList != null && formList.Length > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABEvrakArsiv tab = new TABEvrakArsiv(ve);

                IDataReader dr = tab.Listele(arsivKod, formList);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        EvrakArsivDosya form = new EvrakArsivDosya();
                        form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.PREVRAKNO, "");
                        form.arsivKod = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.ARSIVKOD, 0);
                        form.dosyaAd = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.DOSYAAD, "");
                        form.dosyaYol = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.DOSYAYOL, "");
                        form.dosyaAdet = VeriErisim.DegerAl(dr, (int)TABEvrakArsiv.ENUMAlanlarDosya.DOSYAADET, 0);

                        liste.objeler.Add(form);
                    }
                    dr.Close();
                }

                ve.VeriErisimKapat();
            }

            return (EvrakArsivDosya[])liste.objeler.ToArray(typeof(EvrakArsivDosya));
        }

        public static EvrakArsiv[] Ara(VeriErisim veriErisim, Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList)
        {
            ObjectArray liste = new ObjectArray();

            if (formList != null && formList.Length > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABEvrakArsiv tab = new TABEvrakArsiv(ve);

                IDataReader dr = tab.Ara(arsivKod, formList);
                if (dr != null)
                {
                    EvrakArsiv evrakArsiv = new EvrakArsiv();
                    ArrayList metaList = new ArrayList();

                    while (dr.Read())
                    {
                        string prEvrakNo = VeriErisim.DegerAl(dr, 0, "");
                        if (prEvrakNo != evrakArsiv.prEvrakNo)
                        {
                            if (!string.IsNullOrEmpty(evrakArsiv.prEvrakNo))
                            {
                                evrakArsiv.meta = (EvrakArsivMeta[])metaList.ToArray(typeof(EvrakArsivMeta));
                                liste.objeler.Add(evrakArsiv);
                            }

                            evrakArsiv = new EvrakArsiv();
                            evrakArsiv.dosya.arsivKod = arsivKod;
                            evrakArsiv.prEvrakNo = prEvrakNo;
                            evrakArsiv.dosya.dosyaAd = VeriErisim.DegerAl(dr, 1, "");
                            evrakArsiv.dosya.dosyaYol = VeriErisim.DegerAl(dr, 2, "");
                            evrakArsiv.dosya.dosyaAdet = VeriErisim.DegerAl(dr, 3, 0);
                            evrakArsiv.islemYapan = VeriErisim.DegerAl(dr, 4, "");
                            evrakArsiv.islemTarih = VeriErisim.DegerAl(dr, 5);
                            evrakArsiv.islemYapanAd = VeriErisim.DegerAl(dr, 6, "");
                            evrakArsiv.iptal = VeriErisim.DegerAl(dr, 7, 0);

                            metaList = new ArrayList();
                        }

                        EvrakArsivMeta evrakMeta = new EvrakArsivMeta();
                        evrakMeta.meta.kod = VeriErisim.DegerAl(dr, 8, 0);
                        evrakMeta.meta.ad = VeriErisim.DegerAl(dr, 9, "");
                        evrakMeta.metaDeger = VeriErisim.DegerAl(dr, 10, "");
                        evrakMeta.metaDeger2 = VeriErisim.DegerAl(dr, 11, "");
                        metaList.Add(evrakMeta);

                    }
                    dr.Close();


                    if (!string.IsNullOrEmpty(evrakArsiv.prEvrakNo))
                    {
                        evrakArsiv.meta = (EvrakArsivMeta[])metaList.ToArray(typeof(EvrakArsivMeta));
                        liste.objeler.Add(evrakArsiv);
                    }

                }

                ve.VeriErisimKapat();
            }

            return (EvrakArsiv[])liste.objeler.ToArray(typeof(EvrakArsiv));
        }

        public static Sonuc KaydetTarihce(VeriErisim veriErisim, Kullanici kullanan, EvrakArsivTarihce form)
        {
            Sonuc sonuc = new Sonuc(false, "İşlem sırasında bilinmeyen bir hata oluştu", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            ve.BeginTransaction();
            try
            {
                form.islemTarih = new TNSDateTime(DateTime.Now);
                form.islemYapan = kullanan.mernis;
                form.islemYapanAd = kullanan.ad + " " + kullanan.soyad;

                sonuc = tab.Kaydet(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{8D1E2EAC-2B74-4E08-B750-F3F7D79F75BC} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakArsivRaporEvrakSayisi[] EvrakArsivRaporEvrakSayisiListele(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, string islemYapan, int arsivKod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakArsiv tab = new TABEvrakArsiv(ve);

            IDataReader dr = tab.ListeleEvrakSayisi(baslangicTarih, bitisTarih, islemYapan, arsivKod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    EvrakArsivRaporEvrakSayisi form = new EvrakArsivRaporEvrakSayisi();
                    form.islemYapanAd = VeriErisim.DegerAl(dr, 0, "");
                    form.evrakSayisi = VeriErisim.DegerAl(dr, 1, 0);
                    form.sayfaSayisi = VeriErisim.DegerAl(dr, 2, 0);
                    form.islemDurum = VeriErisim.DegerAl(dr, 3, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakArsivRaporEvrakSayisi[])liste.objeler.ToArray(typeof(EvrakArsivRaporEvrakSayisi));
        }

    }
}
