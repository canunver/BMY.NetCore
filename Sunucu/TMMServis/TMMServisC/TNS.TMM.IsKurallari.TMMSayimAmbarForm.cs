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
    public class TMMSayimAmbarForm
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc SayimBaslat(VeriErisim ve, Kullanici kullanan, SayimAmbarForm form)
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
            foreach (SayimAmbarForm kontrol in kontrolBilgiler.objeler)
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

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
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
        public Sonuc SayimSonlandir(VeriErisim ve, Kullanici kullanan, SayimAmbarForm form)
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
                foreach (SayimAmbarForm kontrol in kontrolBilgiler.objeler)
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

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            ve.BeginTransaction();

            sonuc = to.SayimSonlandir(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray SayimListele(VeriErisim veriErisim, Kullanici kullanan, SayimAmbarForm form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            IDataReader dr = to.SayimListele(form);
            while (dr.Read())
            {
                SayimAmbarForm syk = new SayimAmbarForm();
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

        public ObjectArray SayimAmbarListele(VeriErisim veriErisim, Kullanici kullanan, Sayimambar form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            IDataReader dr = to.AmbarListele(form);
            while (dr.Read())
            {
                Sayimambar sk = new Sayimambar();
                sk.kod = VeriErisim.DegerAl(dr, 0, "");
                sk.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.ambarKod = VeriErisim.DegerAl(dr, 2, "");
                sk.ambarAd = VeriErisim.DegerAl(dr, 3, "");
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

        public Sonuc YeniAmbarSay(VeriErisim ve, Kullanici kullanan, Sayimambar sk)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sk.sayimKod))
                hata += "Sayım Kodu boş bırakılamaz" + "<br>";
            if (string.IsNullOrEmpty(sk.ambarKod))
                hata += "Ambar bilgisi boş bırakılamaz" + "<br>";

            SayimAmbarForm sayim = new SayimAmbarForm();

            if (sk.ambarKod != "" && sk.sayimKod != "")
            {
                //Sayimi bul ve muhasebe ve hb bilgisini al --alt bölümde kullanılacak
                sayim.kod = sk.sayimKod;
                ObjectArray saListe = SayimListele(ve, kullanan, sayim);
                if (saListe.objeler.Count == 1)
                    sayim = (SayimAmbarForm)saListe.objeler[0];
                else
                    hata += "Verilen sayım bilgisi bulunamadı";

                ObjectArray kontrolBilgiler = SayimAmbarListele(ve, kullanan, sk);
                foreach (Sayimambar kontrol in kontrolBilgiler.objeler)
                {
                    hata += "Sayım yapılmak istenen ambar için sayım işlemi daha önce başlatılmıştır. Listeden ilgili ambarı seçerek sayım bilgilerini görebilirsiniz.." + "<br>";
                    break;
                }

                if (hata == "" && string.IsNullOrEmpty(sk.ambarAd))
                {
                    Ambar ambar = new Ambar();
                    ambar.kod = sk.ambarKod;
                    ambar.muhasebeAd = sayim.muhasebe;
                    ambar.harcamaBirimKod = sayim.harcamaBirimi;
                    ObjectArray ambarlar = TMMAmbar.Listele(ve, kullanan, ambar);
                    foreach (Ambar item in ambarlar.objeler)
                    {
                        sk.ambarAd = item.ad;
                    }

                    if (sk.ambarAd == "") hata += sk.ambarKod + " nolu Ambar bulunamadı";
                }
            }

            ObjectArray bilgiler = new ObjectArray();
            if (hata == "")
            {
                TNS.TMM.SicilNoHareket kriter = new SicilNoHareket();
                kriter.muhasebeKod = sayim.muhasebe;
                kriter.harcamaBirimKod = sayim.harcamaBirimi;
                kriter.ambarKod = sk.ambarKod;
                kriter.nereyeGitti = sk.odaKod;
                kriter.yil = DateTime.Now.Year;
                bilgiler = TMMSicilNoHareket.BarkodSicilNolar(ve, kullanan, kriter, new Sayfalama());

                if (!bilgiler.sonuc.islemSonuc)
                    hata += bilgiler.sonuc.hataStr;

                if (bilgiler.objeler.Count == 0)
                    hata += sk.ambarAd + " ambarında malzeme bulunmamaktadır.";
            }

            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            ve.BeginTransaction();

            sk.kod = Guid.NewGuid().ToString();
            sonuc = to.AmbarKaydet(sk);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                //demirbaşlar yukarıda listelendi şimdi sayıma kayıt edilecek

                //Ambardaki malzemelerin kaydı yapılıyor.
                ObjectArray listeAmbar = new ObjectArray();
                foreach (SicilNoHareket item in bilgiler.objeler)
                {
                    if (item.islemTipKod == (int)ENUMZimmetIslemTipi.ZFVERME || item.islemTipKod == (int)ENUMZimmetIslemTipi.DTLVERME) continue;
                    if (item.kimeGitti == "" || item.odaKod != "")
                    {
                        SayimAmbarDetay kd = new SayimAmbarDetay();
                        kd.kod = "";
                        kd.sayimKod = sk.sayimKod;
                        kd.ambarKod = sk.ambarKod;
                        kd.sicilNo = item.sicilNo;
                        kd.prSicilNo = item.prSicilNo;
                        kd.malzemeAd = item.hesapPlanAd;

                        kd.sayimDetayKod = sk.kod;

                        listeAmbar.objeler.Add(kd);
                    }
                }

                if (listeAmbar.objeler.Count > 0)
                    sonuc = AmbarDetayKaydet(ve, kullanan, listeAmbar);

                sonuc.anahtar = sk.kod;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc AmbarDetayKaydet(VeriErisim ve, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            foreach (SayimAmbarDetay skd in liste.objeler)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(skd.sayimKod))
                    hata += "Sayım Kodu boş bırakılamaz" + "<br>";
                if (string.IsNullOrEmpty(skd.ambarKod + skd.sayimDetayKod))
                    hata += "Ambar/Sayım Detay bilgisi boş bırakılamaz" + "<br>";
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

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            ve.BeginTransaction();

            foreach (SayimAmbarDetay skd in liste.objeler)
            {
                skd.kod = Guid.NewGuid().ToString();
                sonuc = to.AmbarDetayKaydet(skd);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc AmbarDetayOkundu(VeriErisim ve, Kullanici kullanan, SayimAmbarDetay sayim)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sayim.sayimKod))
                hata += "Sayım Kodu boş bırakılamaz" + "<br>";
            if (string.IsNullOrEmpty(sayim.ambarKod + sayim.sayimDetayKod))
                hata += "Ambar/Sayım Detay bilgisi boş bırakılamaz" + "<br>";
            if (sayim.prSicilNo == 0)
                hata += "Malzeme bilgisi boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(sayim.kod))
            {
                SayimAmbarDetay kriter = new SayimAmbarDetay();
                kriter.sayimKod = sayim.sayimKod;
                kriter.ambarKod = sayim.ambarKod;
                kriter.prSicilNo = sayim.prSicilNo;
                kriter.sicilNo = sayim.sicilNo;
                ObjectArray sdListe = SayimAmbarDetayListele(ve, kullanan, kriter);

                foreach (SayimAmbarDetay sd in sdListe.objeler)
                {
                    sayim.kod = sd.kod;
                }
            }

            if (!string.IsNullOrEmpty(sayim.kod))
                sonuc = to.AmbarDetayOkundu(sayim);
            else
            {
                sayim.kod = Guid.NewGuid().ToString();
                sonuc = to.AmbarDetayKaydet(sayim);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray SayimAmbarDetayListele(VeriErisim veriErisim, Kullanici kullanan, SayimAmbarDetay form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            IDataReader dr = to.AmbarDetayListele(form);
            while (dr.Read())
            {
                SayimAmbarDetay sk = new SayimAmbarDetay();
                sk.kod = VeriErisim.DegerAl(dr, 0, "");
                sk.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sk.ambarKod = VeriErisim.DegerAl(dr, 2, "");
                sk.prSicilNo = VeriErisim.DegerAl(dr, 3, 0);
                sk.sicilNo = VeriErisim.DegerAl(dr, 4, "");
                sk.malzemeAd = VeriErisim.DegerAl(dr, 5, "");
                sk.okundu = VeriErisim.DegerAl(dr, 6, 0);
                sk.fazla = VeriErisim.DegerAl(dr, 7, 0);
                sk.fazlaAmbarKod = VeriErisim.DegerAl(dr, 8, "");
                sk.fazlaAmbarAd = VeriErisim.DegerAl(dr, 9, "");
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

        public Sonuc FazlaSil(VeriErisim veriErisim, Kullanici kullanan, SayimAmbarDetay skd)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);

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

        public ObjectArray Rapor(VeriErisim veriErisim, Kullanici kullanan, Sayimambar sayim)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            IDataReader dr = to.Rapor(sayim);
            while (dr.Read())
            {
                Sayimambar sa = new Sayimambar();
                sa.kod = VeriErisim.DegerAl(dr, 0, "");
                sa.sayimKod = VeriErisim.DegerAl(dr, 1, "");
                sa.ambarKod = VeriErisim.DegerAl(dr, 2, "");
                sa.ambarAd = VeriErisim.DegerAl(dr, 3, "");
                sa.odaKod = VeriErisim.DegerAl(dr, 4, "");
                sa.odaAd = VeriErisim.DegerAl(dr, 5, "");
                sa.islemYapanAd = VeriErisim.DegerAl(dr, 6, "");
                sa.islemYapanKod = VeriErisim.DegerAl(dr, 7, "");
                sa.islemTarihi = VeriErisim.DegerAl(dr, 8);
                sa.sicilNo = VeriErisim.DegerAl(dr, 9, "");
                sa.malzemeAd = VeriErisim.DegerAl(dr, 10, "");
                sa.okundu = VeriErisim.DegerAl(dr, 11, 0);
                sa.fazla = VeriErisim.DegerAl(dr, 12, 0);
                sa.fazlaAmbarKod = VeriErisim.DegerAl(dr, 13, "");
                sa.fazlaAmbarAd = VeriErisim.DegerAl(dr, 14, "");
                sa.ambarAd = VeriErisim.DegerAl(dr, 15, "");
                sa.baslamaTarihi = VeriErisim.DegerAl(dr, 16);
                sa.bitisTarihi = VeriErisim.DegerAl(dr, 17);
                sa.sayimAd = VeriErisim.DegerAl(dr, 18, "");

                liste.objeler.Add(sa);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc SayimSil(VeriErisim ve, Kullanici kullanan, SayimAmbarForm form)
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
            TABSayimAmbarForm to = new TABSayimAmbarForm(ve);
            ve.BeginTransaction();

            sonuc = to.SayimAmbarDetaySil(form.kod);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SayimAmbarSil(form.kod);
                if (sonuc.islemSonuc)
                {
                    sonuc = to.SayimAmbarFormSil(form.kod);
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
