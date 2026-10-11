using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class TMMSayimKisiForm
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc SayimBaslat(VeriErisim ve, Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.adi))
                hata += "Sayım adı boş bırakılamaz." + "<br>";
            if (form.muhasebe == "")
                hata += "Muhasebe bilgisi boş bırakılamaz." + "<br>";
            if (form.harcamaBirimi == "")
                hata += "Harcama Birimi bilgisi boş bırakılamaz." + "<br>";

            ObjectArray kontrolBilgiler = SayimListele(ve, kullanan, form);
            foreach (SayimKisiForm kontrol in kontrolBilgiler.objeler)
            {
                if (kontrol.bitisTarih.isNull)
                {
                    hata += "Devam eden bir sayım bulunmaktadır. Bu sayımı sonlandırdıktan sonra yeni sayım başlatabilirsiniz." + "<br>";
                    break;
                }
            }

            if (hata == "")
            {
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, form.muhasebe.ToString(), form.harcamaBirimi);
                if (!sonuc.islemSonuc)
                    hata += TasinirMalResI.TMM024;
            }

            if (!string.IsNullOrEmpty(hata))
            {
                hata = "Sayım başlatma işlemi yapılamadı<br><br>" + hata;
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            form.kod = Guid.NewGuid().ToString();
            sonuc = to.SayimBaslat(form);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = form.kod;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Guncelles the specified ve.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc SayimSonlandir(VeriErisim ve, Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.kod))
                hata += "Sayım seçilmemiş." + "<br>";

            if (form.kod != "")
            {
                ObjectArray kontrolBilgiler = SayimListele(ve, kullanan, form);
                bool bulundu = false;
                foreach (SayimKisiForm kontrol in kontrolBilgiler.objeler)
                {
                    if (form.kod == kontrol.kod && kontrol.bitisTarih.isNull)
                    {
                        sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, kontrol.muhasebe, kontrol.harcamaBirimi);
                        if (!sonuc.islemSonuc)
                            hata += TasinirMalResI.TMM024;

                        bulundu = true;
                        break;
                    }
                }

                if (!bulundu)
                {
                    hata += "İşlem yapılmak istenen sayım sonlandırılamadı." + "<br>";
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                hata = "Sayım sonlandırma işlemi yapılamadı<br><br>" + hata;
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            sonuc = to.SayimSonlandir(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray SayimListele(VeriErisim veriErisim, Kullanici kullanan, SayimKisiForm form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            IDataReader dr = to.SayimListele(form);
            while (dr.Read())
            {
                SayimKisiForm syk = new SayimKisiForm();
                syk.kod = VeriErisim.DegerAl(dr, 0, "");
                syk.muhasebe = VeriErisim.DegerAl(dr, 1, "");
                syk.harcamaBirimi = VeriErisim.DegerAl(dr, 2, "");
                syk.baslamaTarih = VeriErisim.DegerAl(dr, 3);
                syk.bitisTarih = VeriErisim.DegerAl(dr, 4);
                syk.adi = VeriErisim.DegerAl(dr, 5, "");
                syk.islemYapanAd = VeriErisim.DegerAl(dr, 6, "");
                syk.islemYapanKod = VeriErisim.DegerAl(dr, 7, "");
                syk.islemTarihi = VeriErisim.DegerAl(dr, 8);
                liste.objeler.Add(syk);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public ObjectArray SayimKisiListele(VeriErisim veriErisim, Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            IDataReader dr = to.KisiListele(form);
            while (dr.Read())
            {
                Sayimkisi sk = new Sayimkisi();
                sk.kod = VeriErisim.DegerAl(dr, 0, "");
                sk.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.kisiKod = VeriErisim.DegerAl(dr, 2, "");
                sk.kisiAd = VeriErisim.DegerAl(dr, 3, "");
                sk.odaKod = VeriErisim.DegerAl(dr, 4, "");
                sk.odaAd = VeriErisim.DegerAl(dr, 5, "");
                sk.islemYapanAd = VeriErisim.DegerAl(dr, 6, "");
                sk.islemYapanKod = VeriErisim.DegerAl(dr, 7, "");
                sk.islemTarihi = VeriErisim.DegerAl(dr, 8);
                sk.zimmetli = VeriErisim.DegerAl(dr, 9, 0);
                sk.sayilan = VeriErisim.DegerAl(dr, 10, 0);
                sk.fazla = VeriErisim.DegerAl(dr, 11, 0);
                liste.objeler.Add(sk);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc YeniKisiSay(VeriErisim ve, Kullanici kullanan, Sayimkisi sk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sk.sayimKod))
                hata += "Sayım Kodu boş bırakılamaz" + "<br>";
            if (string.IsNullOrEmpty(sk.kisiKod) && string.IsNullOrEmpty(sk.odaKod))
                hata += "Kişi/Oda bilgisi boş bırakılamaz" + "<br>";

            SayimKisiForm sayim = new SayimKisiForm();

            if ((sk.kisiKod != "" || sk.odaKod != "") && sk.sayimKod != "")
            {
                //Sayimi bul ve muhasebe ve hb bilgisini al --alt bölümde kullanılacak
                sayim.kod = sk.sayimKod;
                ObjectArray saListe = SayimListele(ve, kullanan, sayim);
                if (saListe.objeler.Count == 1)
                    sayim = (SayimKisiForm)saListe.objeler[0];
                else
                    hata += "Verilen sayım bilgisi bulunamadı";

                ObjectArray kontrolBilgiler = SayimKisiListele(ve, kullanan, sk);
                foreach (Sayimkisi kontrol in kontrolBilgiler.objeler)
                {
                    hata += "Sayım yapılmak istenen kişi/oda için sayım işlemi daha önce başlatılmıştır. Listeden ilgili kişiyi seçerek sayım bilgilerini görebilirsiniz.." + "<br>";
                    break;
                }

                if (hata == "" && string.IsNullOrEmpty(sk.kisiAd) && sk.kisiKod != "")
                {
                    Personel personel = new Personel();
                    personel.kod = sk.kisiKod;
                    ObjectArray personeller = TMMPersonel.Listele(kullanan, personel);
                    foreach (Personel item in personeller.objeler)
                    {
                        sk.kisiAd = item.ad;
                    }

                    if (sk.kisiAd == "") hata += sk.kisiKod + " nolu Kişi bulunamadı";
                }
                if (hata == "" && string.IsNullOrEmpty(sk.odaAd) && sk.odaKod != "")
                {
                    Oda oda = new Oda();
                    oda.kod = sk.odaKod;
                    ObjectArray odalar = TMMOda.Listele(kullanan, oda);
                    foreach (Oda item in odalar.objeler)
                    {
                        sk.odaAd = item.ad;
                    }
                    sk.kisiAd = "Oda sayımı";
                    if (sk.odaAd == "") hata += sk.odaKod + " nolu Oda bulunamadı";
                }
            }


            ObjectArray bilgiler = new ObjectArray();
            if (hata == "")
            {
                if (Arac.MerkezBankasiKullaniyor())
                {
                    TNS.TMM.SicilNoHareket kriter = new SicilNoHareket();
                    kriter.muhasebeKod = sayim.muhasebe;
                    kriter.harcamaBirimKod = sayim.harcamaBirimi;
                    kriter.ambarKod = "01";
                    kriter.kimeGitti = sk.kisiKod;
                    kriter.yil = DateTime.Now.Year;
                    var bilgiler2 = TMMSicilNoHareket.BarkodSicilNolar(ve, kullanan, kriter, new Sayfalama());

                    if (bilgiler2.objeler.Count > 0)
                    {
                        var item = new ZimmetOrtakAlanVeKisi();

                        foreach (TNS.TMM.SicilNoHareket item2 in bilgiler2.objeler)
                        {
                            var itemd = new ZimmetOrtakAlanVeKisiDetay();
                            itemd.gorSicilNo = item2.sicilNo;
                            itemd.prSicilNo = item2.prSicilNo;
                            itemd.sicilAd = item2.hesapPlanAd;

                            item.detay.Add(itemd);
                        }

                        bilgiler.objeler.Add(item);

                    }

                }
                else
                {
                    TNS.TMM.ZimmetOrtakAlanVeKisi kriter = new ZimmetOrtakAlanVeKisi();
                    kriter.tcKimlikNo = sk.kisiKod;
                    kriter.odaKod = sk.odaKod;
                    kriter.muhasebeKod = sayim.muhasebe;
                    kriter.harcamaKod = sayim.harcamaBirimi;
                    kriter.yil = DateTime.Now.Year;

                    if (kriter.tcKimlikNo == "") kriter.tcKimlikNo = "||";

                    bilgiler = TMMRaporlar.ZimmetKisi(ve, kullanan, kriter, true);
                }

                if (!bilgiler.sonuc.islemSonuc)
                    hata += bilgiler.sonuc.hataStr;

                if (bilgiler.objeler.Count == 0)
                {
                    if (sk.kisiAd != "")
                        hata += sk.kisiAd + " isimli personel üzerinde zimmetli malzeme bulunmamaktadır.";
                    else if (sk.odaAd != "")
                        hata += sk.odaAd + " isimli oda üzerinde zimmetli malzeme bulunmamaktadır.";
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            sk.kod = Guid.NewGuid().ToString();
            sonuc = to.KisiKaydet(sk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                //demirbaşlar yukarıda listelendi şimdi sayıma kayıt edilecek

                //Kişi üzerindeki malzemelerin kaydı yapılıyor.
                ObjectArray listeKisi = new ObjectArray();
                foreach (ZimmetOrtakAlanVeKisi item in bilgiler.objeler)
                {
                    foreach (ZimmetOrtakAlanVeKisiDetay itemd in item.detay)
                    {
                        SayimKisiDetay kd = new SayimKisiDetay();
                        kd.kod = "";
                        kd.sayimKod = sk.sayimKod;
                        if (sk.kisiKod != "")
                            kd.kisiKod = sk.kisiKod;
                        else
                            kd.kisiKod = sk.odaKod;
                        kd.sicilNo = itemd.gorSicilNo;
                        kd.prSicilNo = itemd.prSicilNo;
                        kd.malzemeAd = itemd.sicilAd;

                        kd.sayimDetayKod = sk.kod;

                        listeKisi.objeler.Add(kd);
                    }
                }

                if (listeKisi.objeler.Count > 0)
                    sonuc = KisiDetayKaydet(ve, kullanan, listeKisi);
            }
            else
                ve.Rollback();

            if (sonuc.islemSonuc)
                sonuc.anahtar = sk.kod;

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KisiDetayKaydet(VeriErisim ve, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            foreach (SayimKisiDetay skd in liste.objeler)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(skd.sayimKod))
                    hata += "Sayım Kodu boş bırakılamaz" + "<br>";
                if (string.IsNullOrEmpty(skd.kisiKod + skd.sayimDetayKod))
                    hata += "Kişi/Sayım Detay Kod bilgisi boş bırakılamaz" + "<br>";
                if (skd.prSicilNo == 0)
                    hata += "Malzeme bilgisi boş bırakılamaz" + "<br>";

                if (!string.IsNullOrEmpty(hata))
                {
                    ve.VeriErisimKapat();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            foreach (SayimKisiDetay skd in liste.objeler)
            {
                skd.kod = Guid.NewGuid().ToString();
                sonuc = to.KisiDetayKaydet(skd);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KisiDetayOkundu(VeriErisim ve, Kullanici kullanan, SayimKisiDetay sayim)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sayim.sayimKod))
                hata += "Sayım Kodu boş bırakılamaz" + "<br>";
            if (string.IsNullOrEmpty(sayim.kisiKod + sayim.sayimDetayKod))
                hata += "Kişi/Sayım Detay Kod bilgisi boş bırakılamaz" + "<br>";
            if (sayim.prSicilNo == 0)
                hata += "Malzeme bilgisi boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(sayim.kod))
            {
                SayimKisiDetay kriter = new SayimKisiDetay();
                kriter.sayimKod = sayim.sayimKod;
                kriter.kisiKod = sayim.kisiKod;
                kriter.prSicilNo = sayim.prSicilNo;
                kriter.sicilNo = sayim.sicilNo;
                ObjectArray sdListe = SayimKisiDetayListele(ve, kullanan, kriter);

                foreach (SayimKisiDetay sd in sdListe.objeler)
                {
                    sayim.kod = sd.kod;
                }
            }

            if (!string.IsNullOrEmpty(sayim.kod))
                sonuc = to.KisiDetayOkundu(sayim);
            else
            {
                sayim.kod = Guid.NewGuid().ToString();
                sonuc = to.KisiDetayKaydet(sayim);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray SayimKisiDetayListele(VeriErisim veriErisim, Kullanici kullanan, SayimKisiDetay form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            IDataReader dr = to.KisiDetayListele(form);
            while (dr.Read())
            {
                SayimKisiDetay sk = new SayimKisiDetay();
                sk.kod = VeriErisim.DegerAl(dr, 0, "");
                sk.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.kisiKod = VeriErisim.DegerAl(dr, 2, "");
                sk.prSicilNo = VeriErisim.DegerAl(dr, 3, 0);
                sk.sicilNo = VeriErisim.DegerAl(dr, 4, "");
                sk.malzemeAd = VeriErisim.DegerAl(dr, 5, "");
                sk.okundu = VeriErisim.DegerAl(dr, 6, 0);
                sk.fazla = VeriErisim.DegerAl(dr, 7, 0);
                sk.fazlaKisiKod = VeriErisim.DegerAl(dr, 8, "");
                sk.fazlaKisiAd = VeriErisim.DegerAl(dr, 9, "");
                sk.islemYapanAd = VeriErisim.DegerAl(dr, 10, "");
                sk.islemYapanKod = VeriErisim.DegerAl(dr, 11, "");
                sk.islemTarihi = VeriErisim.DegerAl(dr, 12);
                sk.sayimDetayKod = VeriErisim.DegerAl(dr, 13, "");
                sk.epc = VeriErisim.DegerAl(dr, 14, "");
                liste.objeler.Add(sk);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc FazlaSil(VeriErisim veriErisim, Kullanici kullanan, SayimKisiDetay skd)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);

            ve.BeginTransaction();

            sonuc = to.FazlaSil(skd);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM162 + "</b>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM163 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Rapor(VeriErisim veriErisim, Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            IDataReader dr = to.Rapor(form);
            while (dr.Read())
            {
                Sayimkisi sk = new Sayimkisi();
                sk.kod = VeriErisim.DegerAl(dr, 0, "");
                sk.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.kisiKod = VeriErisim.DegerAl(dr, 2, "");
                sk.kisiAd = VeriErisim.DegerAl(dr, 3, "");
                sk.odaKod = VeriErisim.DegerAl(dr, 4, "");
                sk.odaAd = VeriErisim.DegerAl(dr, 5, "");
                sk.islemYapanAd = VeriErisim.DegerAl(dr, 6, "");
                sk.islemYapanKod = VeriErisim.DegerAl(dr, 7, "");
                sk.islemTarihi = VeriErisim.DegerAl(dr, 8);
                sk.sicilNo = VeriErisim.DegerAl(dr, 9, "");
                sk.malzemeAd = VeriErisim.DegerAl(dr, 10, "");
                sk.okundu = VeriErisim.DegerAl(dr, 11, 0);
                sk.fazla = VeriErisim.DegerAl(dr, 12, 0);
                sk.fazlaKisiKod = VeriErisim.DegerAl(dr, 13, "");
                sk.fazlaKisiAd = VeriErisim.DegerAl(dr, 14, "");
                sk.baslamaTarihi = VeriErisim.DegerAl(dr, 15);
                sk.bitisTarihi = VeriErisim.DegerAl(dr, 16);
                sk.sayimAd = VeriErisim.DegerAl(dr, 17, "");

                liste.objeler.Add(sk);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public ObjectArray KisiRapor(VeriErisim veriErisim, Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            IDataReader dr = to.KisiRapor(form);
            while (dr.Read())
            {
                Sayimkisi sk = new Sayimkisi();
                sk.muhasebeKod = VeriErisim.DegerAl(dr, 0, "");
                sk.harcamabirimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.kimeGitti = VeriErisim.DegerAl(dr, 2, "");
                sk.kisiAd = VeriErisim.DegerAl(dr, 3, "");
                liste.objeler.Add(sk);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc SayimSil(VeriErisim ve, Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.kod))
                hata += "Sayım kodu boş olamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimKisiForm to = new TABSayimKisiForm(ve);
            ve.BeginTransaction();

            sonuc = to.SayimKisiDetaySil(form.kod);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SayimKisiSil(form.kod);
                if (sonuc.islemSonuc)
                {
                    sonuc = to.SayimKisiFormSil(form.kod);
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
