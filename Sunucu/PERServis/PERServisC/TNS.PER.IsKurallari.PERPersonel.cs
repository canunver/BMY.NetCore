using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERPersonel
    {
        public static IKYMServis servisKYM = null;

        public ObjectArray eskiTmp; //Tarihçe için

        public PERPersonel()
        {
            if (servisKYM == null)
                servisKYM = TNS.Api.ServisIstemci.Olustur<IKYMServis>(System.Configuration.ConfigurationManager.AppSettings.Get("KYMServisURL"));
        }

        public ObjectArray Listele(Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();

            //Şifresiz girişten kullanıcı listelenebiliyor
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu, true))
                {
                    if (!PER.Yetki.YetkisiVarMiKopru(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                    {
                        pVAN.sonuc.islemSonuc = false;
                        pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                        return pVAN;
                    }
                }
            }
            else if (TNS.PER.Arac.OrgSemaPersonelKullaniyorMu())
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu) && p.organizasyonKodu > 0)
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
                else if (p.yetkiKontroluIleListele && p.organizasyonKodu == 0)
                {
                    string[] yetkiler = PER.Yetki.YetkiliOlduguBirimler(kullanan);
                    p.yetkiliOlduguBirimler = new int[yetkiler.Length];
                    for (int i = 0; i < yetkiler.Length; i++)
                    {
                        p.yetkiliOlduguBirimler[i] = OrtakFonksiyonlar.ConvertToInt(yetkiler[i], 0);
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = tab.Listele(p, null);
            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            while (r.Read())
            {
                Personel pp = new Personel();

                pp.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TCKIMLIKNO, "");
                pp.sicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SICILNO, "");
                pp.ad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.AD, "");
                pp.soyad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SOYAD, "");
                pp.birimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BIRIMKODU, "");
                pp.organizasyonKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ORGANIZASYONKOD, 0);
                pp.muhasebeKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.MUHASEBEKODU, 0);
                pp.kurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KURUM, "");

                int personelAlanlarSonIndex = (Enum.GetNames(typeof(TABPersonel.ENUMAlanlar))).Length;
                pp.unvanAd = VeriErisim.DegerAl(r, personelAlanlarSonIndex + (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarSozlesmeliBordro))).Length, "");
                pp.unvanAdKisa = VeriErisim.DegerAl(r, personelAlanlarSonIndex + (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarSozlesmeliBordro))).Length + 1, "");
                pp.eposta = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EPOSTA, "");
                pp.aktifMi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.AKTIFMI, 1);
                pp.kod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KOD, 0);

                if (!string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
                {
                    pp.babaAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BABAAD, "");
                    pp.anaAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ANAAD, "");
                    pp.dogumTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.DOGUMTARIHI);
                    pp.cinsiyet = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CINSIYET, 0);
                    pp.medeniHali = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.MEDENIHALI, 0);
                    pp.kanGrubu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KANGRUBU, "");
                    pp.isTelefon = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ISTELEFON, "");
                    pp.cepTelefon = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CEPTELEFON, "");
                    pp.faks = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.FAKS, "");
                    pp.askerlikKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ASKERLIKKODU, 0);
                    pp.ogrenimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.OGRENIMKODU, 0);
                    pp.esDurumuKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ESDURUMUKODU, 0);
                    pp.saglikKarneNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SAGLIKKARNENO, "");
                    pp.durum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.DURUM, 0);
                    pp.emekliSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLISICILNO, "");
                    pp.guncellenmeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.GUNCELLENMETARIHI);
                    pp.guncelleyenKisi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.GUNCELLEYENKISI, "");

                    pp.bankaKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BANKAKODU, "");
                    pp.bankaSubeKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BANKASUBEKODU, "");
                    pp.bankaHesapNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BANKAHESAPNO, "");

                    pp.teskilatKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TESKILATKODU, 0);
                    pp.unvanKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.UNVANKODU, 0);
                    pp.memurBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.MEMURBASLAMATARIHI);
                    pp.iseBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ISEBASLAMATARIHI);
                    pp.istenCikisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ISTENCIKISTARIHI);

                    pp.sinifKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SINIFKODU, 0);
                    pp.kidemAy = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KIDEMAY, 0);
                    pp.kidemYil = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KIDEMYIL, 0);
                    pp.kadroDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADRODERECE, 0);
                    pp.kadroTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADROTARIH);
                    pp.odemeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODEMEESASDERECE, 0);
                    pp.odemeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODEMEESASKADEME, 0);
                    pp.odemeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODEMEESASGOSTERGE, 0);
                    pp.odemeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODEMEESASTARIH);
                    pp.emekliEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLIESASDERECE, 0);
                    pp.emekliEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLIESASKADEME, 0);
                    pp.emekliEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLIESASGOSTERGE, 0);
                    pp.emekliEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLIESASTARIH);
                    pp.kazanilmisHakDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KAZANILMISHAKDERECE, 0);
                    pp.kazanilmisHakKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KAZANILMISHAKKADEME, 0);
                    pp.kazanilmisHakTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KAZANILMISHAKTARIH);
                    pp.istihdamSekli = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ISTIHDAMSEKLI, 0);

                    pp.adres = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ADRES, "");
                    pp.evTelefon = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EVTELEFON, "");
                    pp.vergiDairesi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.VERGIDAIRESI, "");
                    pp.ustBirimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.USTBIRIMKODU, 0);

                    pp.evrakBirimKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EVRAKBIRIMKOD, "");

                    pp.odaKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODA, "");
                    pp.ilkSoyad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ILKSOYAD, "");
                    pp.dogumYeri = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.DOGUMYERI, "");
                    pp.nufusIlKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSILKOD, "");
                    pp.nufusIlceKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSILCEKOD, "");
                    pp.cuzdanSeriNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CUZDANSERINO, "");
                    pp.cuzdanKayitNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CUZDANKAYITNO, "");
                    pp.nufusMahalleKoy = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSMAHALLEKOY, "");
                    pp.nufusCiltNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSCILTNO, "");
                    pp.nufusSiraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSSIRANO, "");
                    pp.nufusAileSiraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.NUFUSAILESIRANO, "");
                    pp.cuzdanVerilisNedeni = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CUZDANVERILISNEDENI, "");
                    pp.cuzdanVerildigiYer = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CUZDANVERILDIGIYER, "");
                    pp.cuzdanVerilisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.CUZDANVERILISTARIHI);
                    pp.bakanlikKartNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BAKANLIKKARTNO, "");
                    pp.sskSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SSKSICILNO, "");
                    pp.bagkurSicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.BAGKURSICILNO, "");
                    pp.surucuBelgeNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SURUCUBELGENO, "");
                    pp.surucuBelgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SURUCUBELGETARIHI);
                    pp.yasTashihTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.YASTASHIHTARIHI);
                    pp.url = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.URL, "");
                    pp.mobilImzaTelNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.MOBILIMZATELEFON, "");

                    pp.tecilTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TECILTARIHI);
                    pp.askerlikMuafNeden = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ASKERLIKMUAFNEDEN, "");
                    pp.askerlikBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ASKERLIKBASLAMATARIHI);
                    pp.terhisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TERHISTARIHI);
                    pp.askerlikCuzdanNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ASKERLIKCUZDANNO, "");

                    pp.mulkDurumu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.MULKDURUM, 0);
                    pp.tasinmaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TASINMATARIHI);
                    pp.oturmaSuresi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.OTURMASURESI, 0);
                    pp.sonGoreveBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SONGOREVBASLAMATARIHI);
                    pp.sonBirimeBaslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.SONBIRIMBASLAMATARIHI);

                    pp.kadroOrganizasyonKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADROORGANIZASYONKOD, 0);
                    pp.kadroMuhasebeKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADROMUHASEBEKODU, 0);
                    pp.kadroBirimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADROBIRIMKODU, "");
                    pp.kadroKurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KADROKURUM, "");
                    pp.tebligatAdresi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.TEBLIGATADRES, "");
                    pp.acilKisi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ACILKISI, "");
                    pp.acilKisiNumara = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ACILKISINUMARA, "");

                    pp.kazanilmisHakGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KAZANILMISHAKGOSTERGE, 0);
                    pp.odemeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.ODEMEESASEKGOSTERGE, 0);
                    pp.emekliEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.EMEKLIESASEKGOSTERGE, 0);
                    pp.kazanilmisHakEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.KAZANILMISHAKEKGOSTERGE, 0);

                    pp.sozlesmeliBordroTanim.yil = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarSozlesmeliBordro.YIL + personelAlanlarSonIndex, 0);
                    pp.sozlesmeliBordroTanim.ay = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarSozlesmeliBordro.AY + personelAlanlarSonIndex, 0);
                    pp.sozlesmeliBordroTanim.kumulatifVergiMatrahi = (decimal)VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarSozlesmeliBordro.KUMULATIFVERGIMATRAHI + personelAlanlarSonIndex, 0.0);

                    pp.guncelleyenKisi = PERServis.UzayYaziDegerGetir("KULLANICI", pp.guncelleyenKisi, true, "AD" + VeriErisim.IslecBul("+") + "' '" + VeriErisim.IslecBul("+") + "SOYAD");
                }

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleBelgeler(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelBelgeler d = new PersonelBelgeler();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.TCKIMLIKNO, "");
                d.tip = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.TIP, 0);
                d.kayitSiraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.KAYITSIRANO, "");
                d.belgeSiraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.BELGESIRANO, 0);
                d.belgeAdi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.BELGEADI, "");

                #region geriye dönük uyumluluk için...
                string dosyaAdi = d.belgeAdi;
                if (dosyaAdi.LastIndexOf('/') > 0)
                    dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('/')).TrimStart('/');
                if (dosyaAdi.LastIndexOf('\\') > 0)
                    dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('\\')).TrimStart('\\');
                d.belgeAdi = dosyaAdi;
                #endregion

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray BirimBazliPersonelSonMemuriyetListele(Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.BirimBazliPersonelSonMemuriyetListele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                Personel d = new Personel();
                d.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                d.ad = VeriErisim.DegerAl(r, 1, "");
                d.soyad = VeriErisim.DegerAl(r, 2, "");
                d.unvanAd = VeriErisim.DegerAl(r, 3, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc ListeleBelgeGetir(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();
            if (p.belgeSiraNo == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.bilgiStr = "Verilen kriterlere uygun belge <b>bulunamadı</b>";
                return sonuc;
            }

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;
            try
            {
                r = tab.ListeleBelgeler(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;

                return sonuc;
            }

            sonuc.islemSonuc = false;
            sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";
            while (r.Read())
            {
                string belgeAdi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarBelgeler.BELGEADI, "");
                byte[] belge = (byte[])(r.GetValue((int)TABPersonel.ENUMAlanlarBelgeler.BELGE));

                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "";

                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                System.IO.FileStream dosya = new System.IO.FileStream(sonucDosyaAd, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                dosya.Write(belge, 0, belge.Length);
                dosya.Close();

                string uzanti = "xls";
                if (belgeAdi.Contains("."))
                    uzanti = belgeAdi.Substring(belgeAdi.LastIndexOf('.') + 1);

                sonuc.anahtar = sonucDosyaAd + "\n" + belgeAdi + "\n" + uzanti;
                //OrtakClass.DosyaIslem.DosyaGonderX(sonucDosyaAd, belgeAdi, true, uzanti);
            }
            r.Close();
            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray ListeleUnvanKadro(Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleUnvanKadro(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelUnvanKadro pp = new PersonelUnvanKadro();

                pp.unvanAd = VeriErisim.DegerAl(r, 0, "");
                pp.cinsiyet = VeriErisim.DegerAl(r, 1, 0);
                pp.cinsiyetAdet = VeriErisim.DegerAl(r, 2, 0);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleOzet(Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p, null);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                Personel pp = new Personel();

                 pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                pp.sicilNo = VeriErisim.DegerAl(r, 1, "");
                pp.ad = VeriErisim.DegerAl(r, 2, "");
                pp.soyad = VeriErisim.DegerAl(r, 3, "");

                pp.teskilatKodu = VeriErisim.DegerAl(r, 25, 0);
                pp.unvanKodu = VeriErisim.DegerAl(r, 26, 0);
                pp.muhasebeKodu = VeriErisim.DegerAl(r, 27, 0);
                pp.birimKodu = VeriErisim.DegerAl(r, 28, "");
                pp.unvanAd = VeriErisim.DegerAl(r, 58, "");
                pp.unvanAdKisa = VeriErisim.DegerAl(r, 59, "");

                pp.aktifMi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlar.AKTIFMI, 1);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleAdres(Kullanici kullanan, PersonelAdres p)
        {
            ObjectArray pVAN = new ObjectArray();

            //Şİfresiz girişten kullanıcı listelenebiliyor
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleAdres(p.tcKimlikNo);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelAdres d = new PersonelAdres();
                if (!string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
                {
                    d.tcKimlikNo = p.tcKimlikNo;
                    d.adres = VeriErisim.DegerAl(r, 0, "");
                    d.tebligatAdres = VeriErisim.DegerAl(r, 1, "");
                    d.mulkDurumu = VeriErisim.DegerAl(r, 2, 0);
                    d.tasinmaTarihi = VeriErisim.DegerAl(r, 3);
                }
                d.isTel = VeriErisim.DegerAl(r, 4, "");
                d.evTel = VeriErisim.DegerAl(r, 5, "");
                d.cepTel = VeriErisim.DegerAl(r, 6, "");
                d.faks = VeriErisim.DegerAl(r, 7, "");
                d.eposta = VeriErisim.DegerAl(r, 8, "");
                d.url = VeriErisim.DegerAl(r, 9, "");
                d.mobilImzaTel = VeriErisim.DegerAl(r, 10, "");
                d.acilKisi = VeriErisim.DegerAl(r, 11, "");
                d.acilKisiNumara = VeriErisim.DegerAl(r, 12, "");


                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleKurs(Kullanici kullanan, PersonelKurs p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleKurs(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelKurs d = new PersonelKurs();
                d.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                d.siraNo = VeriErisim.DegerAl(r, 1, 0);
                d.kursKod = VeriErisim.DegerAl(r, 2, 0);
                d.aldigiYer = VeriErisim.DegerAl(r, 3, "");
                d.baslamaTarihi = VeriErisim.DegerAl(r, 4);
                d.bitirmeTarihi = VeriErisim.DegerAl(r, 5);
                d.derece = VeriErisim.DegerAl(r, 6, "");
                d.kursAdi = VeriErisim.DegerAl(r, 7, "");
                d.dersSaati = VeriErisim.DegerAl(r, 8, 0);
                d.egitimYeri = VeriErisim.DegerAl(r, 9, "");
                d.aciklama = VeriErisim.DegerAl(r, 10, "");
                d.donem = VeriErisim.DegerAl(r, 11, 0);
                d.yil = VeriErisim.DegerAl(r, 12, 0);
                d.dosyaNo = VeriErisim.DegerAl(r, 13, "");
                d.gun = VeriErisim.DegerAl(r, 14, 0);
				d.katilimTipi = VeriErisim.DegerAl(r, 15, 0);
                d.kursNiteligi = VeriErisim.DegerAl(r, 16, "");
                d.yapildigiIl = VeriErisim.DegerAl(r, 17, "");
                d.egitimSaati = VeriErisim.DegerAl(r, 18, 0);
                d.egitimTurKodu = VeriErisim.DegerAl(r, 19, 0);


                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }
		
		 public ObjectArray ListeleEgitimTurleri(Kullanici kullanan, PersonelEgitimTurleri p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);
            IDataReader r = null;
            try
            {
                r = tab.ListeleEgitimTurleri(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelEgitimTurleri d = new PersonelEgitimTurleri();
                d.kod = VeriErisim.DegerAl(r, 0, 0);
                d.ataKod = VeriErisim.DegerAl(r, 1, 0);
                d.ad = VeriErisim.DegerAl(r, 2, "");
                d.seviye = VeriErisim.DegerAl(r, 3, 0);
                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";
            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleAile(Kullanici kullanan, PersonelAile p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleAile(p.personelTcKimlikNo, p.tcKimlikNo);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelAile d = new PersonelAile();
                d.personelTcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.PERSONELTCKIMLIKNO, "");
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.TCKIMLIKNO, "");
                d.ad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.AD, "");
                d.soyad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.SOYAD, "");
                d.dogumTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.DOGUMTARIHI);
                d.cinsiyet = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.CINSIYET, 0);
                d.yakinlik = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.YAKINLIKDERECESI, 0);
                d.durum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.DURUM, 0);
                d.saglikKarneNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.SAGLIKKARNENO, "");
                d.evlenmeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.EVLENMETARIHI);
                d.aileCuzdaniSeriNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.AILECUZDANISERINO, "");
				d.esininCalistigiKurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.ESININCALISTIGIKURUM, "");
                d.esininGorevYeri = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.ESININGOREVYERI, "");
                d.esininUnvani = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.ESININUNVANI, "");
                d.cocugununOkulununAdi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarAile.COCUGUNUNOKULUNUNADI, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleMalBildirimi(Kullanici kullanan, PersonelMalBildirimi p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleMalBildirimi(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelMalBildirimi d = new PersonelMalBildirimi();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMalBildirimi.TCKIMLIKNO, "");
                d.sirano = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMalBildirimi.SIRANO, 0);
                d.tarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMalBildirimi.TARIH);
                d.aciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMalBildirimi.ACIKLAMA, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleCezaOdul(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelCezaOdul d = new PersonelCezaOdul();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.TCKIMLIKNO, "");
                d.sirano = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.SIRANO, 0);
                d.tip = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.TIP, 0);
                d.cezaodultip = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.CEZAODULTIP, 0);
                d.verilisNedeni = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.VERILISNEDENI, "");
                d.tarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.TARIH);
                d.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.BELGENO, "");
                d.dayanak = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.DAYANAK, "");
                d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarCezaOdul.BELGETARIHI);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleOgrenim(Kullanici kullanan, PersonelOgrenim p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleOgrenim(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelOgrenim d = new PersonelOgrenim();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.TCKIMLIKNO, "");
                d.siraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.SIRANO, 0);
                d.ogrenimKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.OGRENIMKOD, 0);
                d.okulAdi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.OKULADI, "");
                d.fakulteBolum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.FAKULTEBOLUM, "");
                d.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.BELGETUR, 0);
                d.bitirmeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.BITIRMETARIHI);
                d.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.BELGENO, "");
                d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.BELGETARIHI);
                d.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.BASLANGICTARIHI);
                d.derece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.DERECE, 0);
                d.gecerli = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarOgrenim.GECERLI, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleDil(Kullanici kullanan, PersonelDil p)
        {
            ObjectArray pVAN = new ObjectArray();

            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleDil(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelDil d = new PersonelDil();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.TCKIMLIKNO, "");
                d.siraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.SIRANO, 0);
                d.dilKod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.DILKODU, 0);
                d.duzey = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.DUZEY, "");
                d.puan = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.PUAN, 0.0);
                d.donemAy = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.DONEMAY, 0);
                d.donemYil = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.DONEMYIL, 0);
                d.belgeTur = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.BELGETUR, 0);
                d.belgeTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.BELGETARIHI);
                d.belgeNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.BELGENO, "");
                d.bordroyaEsas = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDil.BORDROYAESAS, 0);
                d.dilAd = VeriErisim.DegerAl(r, ((int)TABPersonel.ENUMAlanlarDil.BORDROYAESAS) + 2, "");


                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        {
            ObjectArray pVAN = new ObjectArray();

            if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            {
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleMemuriyet(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }
			
			int sonIndex = (Enum.GetNames(typeof(TABPersonel.ENUMAlanlarMemuriyet))).Length;
            while (r.Read())
            {
                PersonelMemuriyet d = new PersonelMemuriyet();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.TCKIMLIKNO, "");
                d.siraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.SIRANO, 0);
                d.organizasyonKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ORGANIZASYONKOD, 0);
                d.muhasebeKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.MUHASEBEKODU, 0);
                d.birimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.BIRIMKODU, "");
                d.kurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KURUM, "");
                d.birimAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.BIRIMAD, "");
                d.gorev = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.GOREV, "");
                d.istihdamSekli = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ISTIHDAMSEKLI, 0);
                d.aciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ACIKLAMA, "");
                d.baslamaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.BASLAMATARIHI);
                d.ayrilmaTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.AYRILMATARIHI);
                d.baslamaNedeni = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.BASLAMANEDENI, 0);
                d.ayrilmaNedeni = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.AYRILMANEDENI, 0);
                d.evrakNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EVRAKNO, "");
                d.yasalDayanak = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.YASALDAYANAK, "");
                d.onayTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ONAYTARIH);
                d.kadroDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KADRODERECE, 0);
                d.kadroTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KADROTARIH);
                d.odemeyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ODEMEYEESASDERECE, 0);
                d.odemeyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ODEMEYEESASKADEME, 0);
                d.odemeyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ODEMEYEESASTARIH);
                d.kazanilmisHakDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KAZANILMISHAKDERECE, 0);
                d.kazanilmisHakKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KAZANILMISHAKKADEME, 0);
                d.kazanilmisHakTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KAZANILMISHAKTARIH);
                d.emekliyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EMEKLIYEESASDERECE, 0);
                d.emekliyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EMEKLIYEESASKADEME, 0);
                d.emekliyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EMEKLIYEESASTARIH);
                d.odemeyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ODEMEYEESASGOSTERGE, 0);
                d.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.ODEMEYEESASEKGOSTERGE, 0);
                d.kazanilmisGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KAZANILMISGOSTERGE, 0);
                d.kazanilmisEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.KAZANILMISEKGOSTERGE, 0);
                d.emekliyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EMEKLIYEESASGOSTERGE, 0);
                d.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.EMEKLIYEESASEKGOSTERGE, 0);
			    d.malul = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.MALUL, 0);
                d.daireKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarMemuriyet.DAIREKODU, 0);

                d.baslamaNedeniAd = VeriErisim.DegerAl(r, sonIndex, "");
                d.ayrilmaNedeniAd = VeriErisim.DegerAl(r, sonIndex+1, "");
                d.daireAdi = VeriErisim.DegerAl(r, sonIndex + 2, "");
					 
                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleKadro(Kullanici kullanan, PersonelKadro p)
        {
            ObjectArray pVAN = new ObjectArray();

            if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            {
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

                return pVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleKadro(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelKadro d = new PersonelKadro();
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.TCKIMLIKNO, "");
                d.siraNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.SIRANO, 0);
                d.organizasyonKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ORGANIZASYONKOD, 0);
                d.muhasebeKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.MUHASEBEKODU, 0);
                d.birimKodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.BIRIMKODU, "");
                d.kurum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KURUM, "");
                d.birimAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.BIRIMAD, "");
                d.unvan = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.UNVANKOD, 0);
                d.sinifi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.SINIFKOD, 0);
                d.evrakNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EVRAKNO, "");
                d.yasalDayanak = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.YASALDAYANAK, "");
                d.onayTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ONAYTARIH);
                d.aciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ACIKLAMA, "");
                d.kadroDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KADRODERECE, 0);
                d.kadroTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KADROTARIH);
                d.odemeyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ODEMEYEESASDERECE, 0);
                d.odemeyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ODEMEYEESASKADEME, 0);
                d.odemeyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ODEMEYEESASTARIH);
                d.kazanilmisHakDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KAZANILMISHAKDERECE, 0);
                d.kazanilmisHakKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KAZANILMISHAKKADEME, 0);
                d.kazanilmisHakTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KAZANILMISHAKTARIH);
                d.emekliyeEsasDerece = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASDERECE, 0);
                d.emekliyeEsasKademe = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASKADEME, 0);
                d.emekliyeEsasTarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASTARIH);
                d.odemeyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ODEMEYEESASGOSTERGE, 0);
                d.odemeyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.ODEMEYEESASEKGOSTERGE, 0);
                d.kazanilmisGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KAZANILMISGOSTERGE, 0);
                d.kazanilmisEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.KAZANILMISEKGOSTERGE, 0);
                d.emekliyeEsasGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASGOSTERGE, 0);
                d.emekliyeEsasEkGosterge = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASEKGOSTERGE, 0);

                d.unvanAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarKadro.EMEKLIYEESASEKGOSTERGE + 1, "");

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleIzin(Kullanici kullanan, PersonelIzin p)
        {
            ObjectArray pVAN = new ObjectArray();

            //Şİfresiz girişten kullanıcı listelenebiliyor
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleIzin(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelIzin pi = new PersonelIzin();
                pi.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.TCKIMLIKNO, "");
                pi.sirano = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.SIRANO, 0);
                pi.tip = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.TIP, 0);
                pi.aciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.ACIKLAMA, "");
                pi.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.BASLANGICTARIHI);
                pi.bitisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.BITISTARIHI);
                pi.telefon = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.TELEFON, "");
                pi.kullanilanGun = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.KULLANILANGUN, 0);
                pi.izinNotu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.IZINNOTU, "");
                pi.onay = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.ONAY, 0);
                pi.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.EVREVRAKNO, string.Empty);
                pi.prEvrakNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.PREVRAKNO, string.Empty);
                pi.alan1 = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.EVRAKALAN1, string.Empty);
                pi.not = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.EVRAKNOT1, string.Empty);
                pi.raporTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.RAPORTARIHI);
                pi.raporVeren = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.RAPORVEREN, string.Empty);
                pi.raporNumara = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzin.RAPORNUMARA, string.Empty);

                pVAN.objeler.Add(pi);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();


            return pVAN;
        }

        public ObjectArray ListeleIzinDevir(VeriErisim ve, Kullanici kullanan, PersonelIzinDevir p)
        {
            ObjectArray pVAN = new ObjectArray();

            //Şİfresiz girişten kullanıcı listelenebiliyor
            //if (!PER.Yetki.YetkiVarMiOkumaya(kullanan))
            //{
            //    pVAN.sonuc.islemSonuc = false;
            //    pVAN.sonuc.hataStr = "Bilgileri görme yetkiniz yok.";

            //    return pVAN;
            //}

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleIzinDevir(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelIzinDevir pi = new PersonelIzinDevir();
                pi.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.TCKIMLIKNO, "");
                pi.yil = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.YIL, 0);
                pi.devreden = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.DEVREDEN, 0);
                pi.yilici = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.YILICI, 0);
                pi.ad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.YILICI + 1, "");
                pi.soyad = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.YILICI + 2, "");
                pi.sicilNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinDevir.YILICI + 3, "");


                pVAN.objeler.Add(pi);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleTarihce(Kullanici kullanan, PersonelTarihce p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleTarihce(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelTarihce pT = new PersonelTarihce();
                pT.kullanici = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.KULLANICI, "");
                pT.personel = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.PERSONEL, "");
                pT.tarih = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.TARIH);
                pT.degisiklik = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.DEGISIKLIK, "");
                pT.kullaniciAd = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarTarihce.KULLANICIAD, "");

                pVAN.objeler.Add(pT);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Personel ListeleMemuriyetTarihleri(string tcKimlikNo)
        {
            Personel p = new Personel();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleMemuriyetTarihleri(tcKimlikNo);
            }
            catch
            {
                ve.VeriErisimKapat();
                return p;
            }

            while (r.Read())
            {
                p.memurBaslamaTarihi = VeriErisim.DegerAl(r, 0);
                p.sonBirimeBaslamaTarihi = VeriErisim.DegerAl(r, 1);
                p.sonGoreveBaslamaTarihi = VeriErisim.DegerAl(r, 2);
            }
            r.Close();

            ve.VeriErisimKapat();

            return p;
        }

        public Sonuc Kaydet(Kullanici kullanan, Personel p)
        {
            object eski = null, yeni = null; //Tarihçe için

            Sonuc sonuc = new Sonuc();

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (kullanan.mernis != p.tcKimlikNo && !PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Verdiğiniz birim ile bilgileri kaydetme yetkiniz yok.";

                    return sonuc;
                }
            }

            string hata = "";
            hata = Kontrol(kullanan, p);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            ObjectArray pVAN = new ObjectArray();
            Personel pp = new Personel();
            pp.tcKimlikNo = p.tcKimlikNo;
            pp.muhasebeKodu = -1;
            pp.birimKodu = "0";
            pVAN = Listele(kullanan, pp);
            if (pVAN.sonuc.islemSonuc)
            {
                foreach (Personel o in pVAN.objeler)
                {
                    if ((o.birimKodu != p.birimKodu || o.muhasebeKodu != p.muhasebeKodu) && (OrtakFonksiyonlar.ConvertToInt(o.birimKodu, 0) != 0 && o.muhasebeKodu != 0))
                        hata += "Bu personel, " + o.muhasebeKodu + "/" + o.birimKodu + " biriminde kayıtlıdır. Bu personeli kendinize kayıt edebilmeniz için ilgili birim tarafından durum bilgisi nakil olarak değiştirilmelidir.";
                }
            }

            if (hata != "")
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            this.eskiTmp = this.Listele(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new Personel();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            ve.BeginTransaction();

            p.guncelleyenKisi = kullanan.kullaniciKodu;
            if (p.durum > 5)
            {
                p.muhasebeKodu = 0;
                p.birimKodu = "0";
            }

            ENUMTARIHCEISLEMTIP islemTip;
            if (tab.PersonelVarMi(p))
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.Guncelle(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                p.istihdamSekli = 1;//Personel ilk kayıt ediliken Memur olarak kayıt edilsin. Ahmet İlhan 
                sonuc = tab.Kaydet(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                if (p.durum > 5)
                    sonuc.bilgiStr = "Durum bilgisi sebebiyle Muhasebe ve birim kodu serbest bırakıldı. Eğer bu personeli kendi biriminize dahil etmek istiyorsanız Durum alanını uygun bir bilgi ile değiştirip tekrar kayıt edin.";

                if (islemTip == ENUMTARIHCEISLEMTIP.KAYDET)
                {
                    PersonelMemuriyet pm = new PersonelMemuriyet();
                    pm.tcKimlikNo = p.tcKimlikNo;
                    pm.birimAd = p.birimAd;
                    pm.organizasyonKodu = p.organizasyonKodu;
                    pm.muhasebeKodu = p.muhasebeKodu;
                    pm.birimKodu = p.birimKodu;
                    pm.kurum = p.kurum;
                    pm.gorev = "-";
                    pm.baslamaTarihi = new TNSDateTime(DateTime.Now.ToString());
                    pm.istihdamSekli = 0;

                    KaydetMemuriyet(kullanan, pm);
                }

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.KISISEL, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);
            sonuc = tab.KaydetBelgeler(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetBirim(Kullanici kullanan, Personel p)
        {
            object eski = null, yeni = null; //Tarihçe için
            Sonuc sonuc = new Sonuc();

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Verdiğiniz birim ile bilgileri kaydetme yetkiniz yok.";

                    return sonuc;
                }
            }

            //Tarihçe ********************
            this.eskiTmp = this.Listele(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new Personel();
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            ve.BeginTransaction();

            p.guncelleyenKisi = kullanan.kullaniciKodu;

            sonuc = tab.KaydetBirim(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                PersonelMemuriyet pm = new PersonelMemuriyet();
                pm.tcKimlikNo = p.tcKimlikNo;
                pm.birimAd = p.birimAd;
                pm.organizasyonKodu = p.organizasyonKodu;
                pm.muhasebeKodu = p.muhasebeKodu;
                pm.birimKodu = p.birimKodu;
                pm.kurum = p.kurum;
                pm.gorev = "-";
                pm.baslamaTarihi = new TNSDateTime(DateTime.Now.ToString());
                pm.istihdamSekli = tab.IstihdamSekliniVer(p.tcKimlikNo);

                KaydetMemuriyet(kullanan, pm);

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.KISISEL, p.tcKimlikNo, ENUMTARIHCEISLEMTIP.GUNCELLE);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetSozlesmeliBordroTanim(Kullanici kullanan, SozlesmeliBordroTanim p)
        {
            Sonuc sonuc = new Sonuc();

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";

            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            //BARIS: islemSonuc olmadan kaydet ve güncelleye karar vermem lazım
            sonuc = tab.GuncelleSozlesmeliBordroTanim(p);
            if (!sonuc.islemSonuc)
                sonuc = tab.KaydetSozlesmeliBordroTanim(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetAdres(Kullanici kullanan, PersonelAdres p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(p.eposta))
            {
                hata += "Personel ePosta adresi boş bırakılamaz.<br>";
            }
            try
            {
                System.Net.Mail.MailAddress ma = new System.Net.Mail.MailAddress(p.eposta);
            }
            catch
            {
                hata += "hatalı ePosta adresi girilmiş.<br>";
            }

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            this.eskiTmp = this.ListeleAdres(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)
                eski = new PersonelAdres();
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.GuncelleAdres(p);
            if (sonuc.islemSonuc)
            {
                sonuc = tab.GuncelleKullaniciMail(p);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.ADRES, p.tcKimlikNo, ENUMTARIHCEISLEMTIP.GUNCELLE);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetKurs(Kullanici kullanan, PersonelKurs p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.egitimTurKodu == 0)
                hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleKurs(kullanan, new PersonelKurs() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKurs();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.siraNo == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetKurs(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleKurs(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }










        public Sonuc KaydetPersonelEgitimTanim(Kullanici kullanan, PersonelEgitimTurleri p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, kullanan.mernis))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            //if (p.ataKod == 0)
            //    hata = "Eğitim grubu ya da türü boş bırakılamaz.<br>";
            if (p.ad.Trim() == "")
                hata += "Eğitimin adı boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.kod > 0)
            {
                this.eskiTmp = this.ListeleEgitimTurleri(kullanan, new PersonelEgitimTurleri() { kod=p.kod });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelEgitimTurleri();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.kod == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetEgitimTanim(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleEgitimTanim(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                //Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, kullanan.mernis, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

















        public Sonuc KaydetAile(Kullanici kullanan, PersonelAile p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.personelTcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Aile Bireyinin T.C Numarası boş bırakılamaz.<br>";
            if (p.personelTcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";
            if (p.ad.Trim() == "")
                hata += "Aile Bireyinin adı boş bırakılamaz.<br>";
            if (p.soyad.Trim() == "")
                hata += "Aile Bireyinin soyadı boş bırakılamaz.<br>";
            if (!TCKimlikNoKontrolu(kullanan, p.tcKimlikNo, p.dogumTarihi))
                hata += "T.C Kimlik numarası " + p.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor.";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            this.eskiTmp = this.ListeleAile(kullanan, p);
            if (eskiTmp.objeler.Count == 1)
                eski = eskiTmp[0];

            if (eski == null)//BARIS:silinmeli mi?
                eski = new PersonelAile();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (tab.PersonelAileVarMi(p))
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleAile(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetAile(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.AILE, p.personelTcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetMalBildirimi(Kullanici kullanan, PersonelMalBildirimi p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.sirano > 0)
            {
                this.eskiTmp = this.ListeleMalBildirimi(kullanan, new PersonelMalBildirimi() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelMalBildirimi();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.sirano == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetMalBildirimi(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleMalBildirimi(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.MALBILDIRIMI, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.sirano > 0)
            {
                this.eskiTmp = this.ListeleCezaOdul(kullanan, new PersonelCezaOdul() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelCezaOdul();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.sirano == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetCezaOdul(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleCezaOdul(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.CEZAODUL, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetOgrenim(Kullanici kullanan, PersonelOgrenim p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleOgrenim(kullanan, new PersonelOgrenim() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelOgrenim();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.siraNo == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetOgrenim(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleOgrenim(p);
            }

            if (sonuc.islemSonuc)
            {
                if (p.gecerli > 0)//Sadece tek bir kayıtta gecerli işaretli olması için
                    tab.GuncelleOgrenimGecerli(p);

                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.OGRENIM, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetDil(Kullanici kullanan, PersonelDil p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.dilKod == 0)
                hata = "Dil bilgisi boş bırakılamaz.<br>";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleDil(kullanan, new PersonelDil() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelDil();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.siraNo == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetDil(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleDil(p);
            }

            if (sonuc.islemSonuc)
            {
                if (p.bordroyaEsas > 0)//Sadece tek bir kayıtta gecerli işaretli olması için
                    tab.GuncelleDilBordroyaEsas(p);

                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.YABANCIDIL, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";
            if (p.baslamaTarihi.isNull)
                hata += "Birime başlama tarihi boş bırakılamaz.<br>";
            if (!p.ayrilmaTarihi.isNull)
            {
                if (!p.baslamaTarihi.isNull && p.ayrilmaTarihi.Oku() < p.baslamaTarihi.Oku())
                    hata += "Ayrılma tarihi, başlama tarihinden küçük olamaz.<br>";
            }
            else
            {
                ObjectArray memuriyetListe = ListeleMemuriyet(kullanan, p);
                int sayac = 0;
                foreach (PersonelMemuriyet memuriyet in memuriyetListe.objeler)
                {
                    if (memuriyet.ayrilmaTarihi.isNull)
                        sayac++;
                }
                //if (sayac > 1)
                //{
                //    hata += "Birden fazla memuriyet bilgisi için Ayrılma tarihi boş bırakılamaz. Lütfen personelin önceki bilgileri için Ayrılma tarihi bilgisini kaydediniz.<br>";
                //}
            }



            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleMemuriyet(kullanan, new PersonelMemuriyet() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelMemuriyet();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.siraNo == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetMemuriyet(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleMemuriyet(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                MemuriyetTarihleriGuncelle(p.tcKimlikNo);
                MemuriyetKadroBilgileriniGuncelle(kullanan, p.tcKimlikNo);
                if (!p.ayrilmaTarihi.isNull)
                    MemuriyetPersonelAyrilmaGuncelle(kullanan, p);
                else
                    MemuriyetPersonelBirimGuncelle(p);

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.MEMURIYET, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetKadro(Kullanici kullanan, PersonelKadro p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleKadro(kullanan, new PersonelKadro() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKadro();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            ENUMTARIHCEISLEMTIP islemTip;
            if (p.siraNo == 0)
            {
                islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                sonuc = tab.KaydetKadro(p);
            }
            else
            {
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleKadro(p);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
                KadroBilgisiPersonelGuncelle(p.tcKimlikNo);
                MemuriyetKadroBilgileriniGuncelle(kullanan, p.tcKimlikNo);

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.KADRO, p.tcKimlikNo, islemTip);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIzin(Kullanici kullanan, PersonelIzin p, bool OnayIslemi)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            //if (!PER.Yetki.YetkiVarMiYazmaya(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
            if (!p.baslangicTarihi.isNull && !p.bitisTarihi.isNull)
            {
                if (p.baslangicTarihi.Oku() > p.bitisTarihi.Oku())
                    hata += "İzin başlangıç tarihi bitiş tarihinden geç olamaz<br>";
                else
                {
                    VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
                    TABPersonel tab2 = new TABPersonel(ve2);
                    if (tab2.KaydetIzinUygunMu(p) > 0 && p.tip == (int)ENUMIZINTIP.YILLIK)//Kişi izinde iken sıhhı veya mazaret izni alabilir. Hastalık Raporu veya yakını öldüğü için Melih 09.04.2016 Melih
                        hata += "Belirtilen tarih aralığında başka bir izinle çakışıyor<br>";
                }
            }

            if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
            {
                if (p.onay > 0)
                    hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
            }

            if (!OnayIslemi)
            {
                if (p.tip == 0)
                    hata += "Personel izin türü boş bırakılamaz<br>";
                if (p.baslangicTarihi.isNull)
                    hata += "Personel izin başlama tarihi boş bırakılamaz<br>";
                if (p.bitisTarihi.isNull)
                    hata += "Personel izin bitiş tarihi boş bırakılamaz<br>";
            }
            else
            {
                if (p.sirano == 0)
                    hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
                else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
                    hata += "Onaylama işlemi yapamazsınız<br>";
            }

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, p.tcKimlikNo, true))
                p.onay = 0;
            else
                p.onay = 1;

            //Tarihçe ********************
            if (p.sirano > 0)
            {
                this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];

                foreach (PersonelIzin pi in eskiTmp.objeler)
                {
                    if (pi.onay > 0)
                    {
                        if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, pi.tcKimlikNo, true))
                            hata += "Onaylı izni değiştirme yetkiniz yok.";
                    }
                }
            }
            if (eski == null)
                eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            if (!tab.IzinKayitKontrol(p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Seçilen izin türü için izin hakkınız bulunmamaktadır.";
                ve.Rollback();
            }
            else
            {
                ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.KAYDET;//BARIS?
                if (OnayIslemi)
                    sonuc = tab.KaydetOnayIzin(p);
                else
                {
                    //BARIS
                    if (p.sirano == 0)
                    {
                        islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                        sonuc = tab.KaydetIzin(p);
                    }
                    else
                    {
                        islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                        sonuc = tab.GuncelleIzin(p);
                    }
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = p.sirano.ToString();
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;

                    //Tarihçe Kaydet ********************
                    yeni = p;
                    Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.tcKimlikNo, islemTip);
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIzinEvrakBilgi(Kullanici kullanan, PersonelIzin p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            //if (!PER.Yetki.YetkiVarMiYazmaya(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
            if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
            {
                if (p.onay > 0)
                    hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
            }


            if (p.sirano == 0)
                hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
            else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
                hata += "Onaylama işlemi yapamazsınız<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.sirano > 0)
            {
                this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            if (!tab.IzinKayitKontrol(p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Verilen tarih aralığı, kayıtlı izin tarihlerine denk gelmektedir. Lütfen izin tarihlerini kontrol ediniz.";
                ve.Rollback();
            }
            else
            {
                ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

                sonuc = tab.KaydetIzinEvrakBilgi(p);

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = p.sirano.ToString();
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;

                    //Tarihçe Kaydet ********************
                    yeni = p;
                    Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.tcKimlikNo, islemTip);
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIzinAlan(Kullanici kullanan, PersonelIzin p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            //if (!PER.Yetki.YetkiVarMiYazmaya(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Personel T.C Kimlik Numarası boş bırakılamaz<br>";
            if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı listeniyor
            {
                if (p.onay > 0)
                    hata += "Sadece onaysız Personel izin bilgisi kaydı yapılabilirsiniz<br>";
            }


            if (p.sirano == 0)
                hata += "Daha önce kayıt edilmiş izin bilgisi seçiniz<br>";
            else if (string.IsNullOrEmpty(kullanan.kullaniciKodu))//Şifresiz girişten kullanıcı kayıt yapması
                hata += "Onaylama işlemi yapamazsınız<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.sirano > 0)
            {
                this.eskiTmp = this.ListeleIzin(kullanan, new PersonelIzin() { tcKimlikNo = p.tcKimlikNo, sirano = p.sirano });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelIzin();// { tcKimlikNo = "-1" }; // -1 : "Yeni Kayıt"
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            if (!tab.IzinKayitKontrol(p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Verilen tarih aralığı, kayıtlı izin tarihlerine denk gelmektedir. Lütfen izin tarihlerini kontrol ediniz.";
                ve.Rollback();
            }
            else
            {
                ENUMTARIHCEISLEMTIP islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;

                sonuc = tab.KaydetIzinAlan(p);

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = p.sirano.ToString();
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;

                    //Tarihçe Kaydet ********************
                    yeni = p;
                    Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINHAREKET, p.tcKimlikNo, islemTip);
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIzinDevir(Kullanici kullanan, PersonelIzinDevir[] pList)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            if (pList == null || pList.Length == 0)
            {
                return sonuc;
            }

            PersonelIzinDevir pIlk = pList[0];

            if (!PER.Yetki.YetkiVarMiYazmaya(kullanan, pIlk.tcKimlikNo))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PersonelResI.PER011;

                return sonuc;
            }

            string hata = "";
            foreach (PersonelIzinDevir p in pList)
            {
                if (p.tcKimlikNo.Trim() == "")
                    hata = "Personel T.C Kimlik Numarası boş bırakılamaz.<br>";
                if (p.yil == 0)
                    hata += "Personel yıl bilgisi boş bırakılamaz.<br>";
                //if (!TCKimlikNoKontrolu(p.tcKimlikNo))
                //    hata += "T.C Kimlik numarası " + p.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor.";

                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABPersonel tab = new TABPersonel(ve);

            foreach (PersonelIzinDevir p in pList)
            {
                ObjectArray eskiListe = ListeleIzinDevir(ve, kullanan, p);
                foreach (PersonelIzinDevir e in eskiListe.objeler)
                {
                    eski = e;
                    break;
                }

                //BARIS: islemSonuc olmadan kaydet ve güncelleye karar vermem lazım
                ENUMTARIHCEISLEMTIP islemTip;
                islemTip = ENUMTARIHCEISLEMTIP.GUNCELLE;
                sonuc = tab.GuncelleIzinDevir(p);
                if (!sonuc.islemSonuc)
                {
                    islemTip = ENUMTARIHCEISLEMTIP.KAYDET;
                    sonuc = tab.KaydetIzinDevir(p);
                }

                if (sonuc.islemSonuc)
                {
                    //Tarihçe Kaydet ********************
                    if (eski == null) eski = new PersonelIzinDevir();
                    yeni = p;
                    Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.IZINDEVIR, p.tcKimlikNo, islemTip);

                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetIzinTalep(Kullanici kullanan, PersonelIzinTalep[] pList)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiYazmaya(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            foreach (PersonelIzinTalep p in pList)
            {
                if (p.tcKimlikNo.Trim() == "")
                    hata = "Personel T.C Kimlik Numarası boş bırakılamaz.<br>";
                if (p.yil == 0)
                    hata += "Personel yıl bilgisi boş bırakılamaz.<br>";

                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABPersonel tab = new TABPersonel(ve);

            foreach (PersonelIzinTalep p in pList)
            {
                //BARIS: islemSonuc olmadan kaydet ve güncelleye karar vermem lazım
                sonuc = tab.GuncelleIzinTalep(p);
                if (!sonuc.islemSonuc)
                    sonuc = tab.KaydetIzinTalep(p);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = PersonelResI.PER007;
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, Personel p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            if (p.sicilNo == "")
            {
                sonuc.hataStr = "Silmek istenilen <b>Personelin Sicil Numarası</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.Sil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilBelge(Kullanici kullanan, PersonelBelgeler p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";
            if (p.tip <= 0)
                hata += "Tip hatalı.<br>";
            if (p.kayitSiraNo.Trim() == "")
                hata += "Belgenin silineceği kişi/birim belirtilmemiş.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);
            sonuc = tab.SilBelgeler(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilKurs(Kullanici kullanan, PersonelKurs p)
        {
            Sonuc sonuc = new Sonuc();
            object eski = null, yeni = null; //Tarihçe için

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            //if (p.kursKodu == 0)
            //    hata = "Kurs bilgisi boş bırakılamaz.<br>";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Tarihçe ********************
            if (p.siraNo > 0)
            {
                this.eskiTmp = this.ListeleKurs(kullanan, new PersonelKurs() { tcKimlikNo = p.tcKimlikNo, siraNo = p.siraNo });
                if (eskiTmp.objeler.Count == 1)
                    eski = eskiTmp[0];
            }
            if (eski == null)
                eski = new PersonelKurs();
            //****************************

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilKurs(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";

                //Tarihçe Kaydet ********************
                yeni = p;
                Sonuc sonucTarihce = PERPersonelTarihce.Kaydet(kullanan, yeni, eski, ENUMTARIHCE.EGITIM, p.tcKimlikNo, ENUMTARIHCEISLEMTIP.SIL);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }





        public Sonuc SilEgitimTanim(Kullanici kullanan, PersonelEgitimTurleri p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            if(!tab.EgitimTanimKontrol(p))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Seçilen eğitime bağlı kayıtlar olduğu için silme işlemi gerçekleştirilemedi.";
                ve.Rollback();
            }

            else
            {
                sonuc = tab.SilEgitimTanim(p);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
                }
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }





        public Sonuc SilAile(Kullanici kullanan, PersonelAile p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Aile Bireyinin T.C Numarası boş bırakılamaz.<br>";
            if (p.personelTcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilAile(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilMalBildirimi(Kullanici kullanan, PersonelMalBildirimi p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilMalBildirimi(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilCezaOdul(Kullanici kullanan, PersonelCezaOdul p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilCezaOdul(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilOgrenim(Kullanici kullanan, PersonelOgrenim p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilOgrenim(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilDil(Kullanici kullanan, PersonelDil p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.dilKod == 0)
                hata = "Dil bilgisi boş bırakılamaz.<br>";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilDil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilMemuriyet(Kullanici kullanan, PersonelMemuriyet p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilMemuriyet(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
                MemuriyetTarihleriGuncelle(p.tcKimlikNo);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilKadro(Kullanici kullanan, PersonelKadro p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilKadro(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                MemuriyetKadroBilgileriniGuncelle(kullanan, p.tcKimlikNo);
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilIzin(Kullanici kullanan, PersonelIzin p)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Personel T.C Numarası boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilIzin(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilIzinDevir(Kullanici kullanan, PersonelIzinDevir p)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            if (p.tcKimlikNo.Trim() == "")
                hata = "Personel T.C Numarası boş bırakılamaz.<br>";
            if (p.yil == 0)
                hata += "Personel yıl bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilIzinDevir(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc SilSozlesmeliBordroTanim(Kullanici kullanan, SozlesmeliBordroTanim p)
        {
            Sonuc sonuc = new Sonuc();

            //if (!PER.Yetki.YetkiVarMiSilmeye(kullanan))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = PersonelResI.PER011;

            //    return sonuc;
            //}

            string hata = "";

            if (p.tcKimlikNo.Trim() == "")
                hata += "Personel bilgisi boş bırakılamaz.<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPersonel tab = new TABPersonel(ve);

            sonuc = tab.SilSozlesmeliBordroTanim(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string Kontrol(Kullanici kullanan, Personel p)
        {
            string hata = "";
            bool kontrolYap = !TNS.PER.Arac.MerkezBankasiKullaniyor();


            if (!TCKimlikNoKontrolu(kullanan, p.tcKimlikNo, p.dogumTarihi))
                hata = "T.C Kimlik numarası " + p.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor.";
            if (p.muhasebeKodu == 0)
            {
                if (hata != "") hata += "<br>";
                hata += "Muhasebe bilgisi boş bırakılmış";
            }
            if (p.birimKodu == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Birim bilgisi boş bırakılmış";
            }
            //if (hata == "")
            //{
            //    string ad = "";
            //    if (p.muhasebeKodu > 0)
            //    {
            //        ad = PERServis.UzayYaziDegerGetir("MUHMUHASEBE", p.muhasebeKodu.ToString(), true, "");
            //        if (ad == "")
            //        {
            //            if (hata != "") hata += "<br>";
            //            hata += p.muhasebeKodu + " olarak verilen Muhasebe Kodu hatalı";
            //        }
            //    }

            //    if (p.birimKodu.Trim() != "0")
            //    {
            //        ad = PERServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", p.birimKodu.Trim() + "-" + p.muhasebeKodu, false, "");
            //        if (ad == "")
            //        {
            //            if (hata != "") hata += "<br>";
            //            hata += p.birimKodu + " olarak verilen Muhasebe Birim kodu hatalı";
            //        }
            //    }
            //}
            //if (p.sicilNo.Trim() == "")
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Sicil Numarası boş bırakılamaz";
            //}
            if (p.ad.Trim() == "" || p.soyad.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Ad ve Soyadı boş bırakılamaz";
            }
            if (kontrolYap && p.babaAd.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Baba Adı boş bırakılamaz";
            }
            if (kontrolYap && p.anaAd.Trim() == "")
            {
                if (hata != "") hata += "<br>";
                hata += "Ana Adı boş bırakılamaz";
            }
            //if (p.dogumTarihi.isNull)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Doğum Tarihi boş bırakılamaz";
            //}
            //if (p.cinsiyet == 0)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Cinsiyeti boş bırakılamaz";
            //}
            //if (p.istihdamSekli == 0)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "İstihdam Şekli boş bırakılamaz";
            //}
            //if (p.teskilatKodu == 0)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Teşkilat Bilgisi boş bırakılamaz";
            //}
            //if (p.unvanKodu == 0)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Ünvan Kodu boş bırakılamaz";
            //}
            //if (p.memurBaslamaTarihi.isNull)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "Memuriyete Başlama Tarihi boş bırakılamaz";
            //}
            //if (p.iseBaslamaTarihi.isNull)
            //{
            //    if (hata != "") hata += "<br>";
            //    hata += "İşe Giriş Tarihi boş bırakılamaz";
            //}
            if (kontrolYap && (p.bankaKodu.Trim() == "" || p.bankaSubeKodu.Trim() == "" || p.bankaHesapNo.Trim() == ""))
            {
                if (hata != "") hata += "<br>";
                hata += "Banka Bilgileri boş bırakılamaz";
            }
            //else
            //{
            //    string kod = PERServis.UzayYaziDegerGetir("PERBANKASUBE", p.bankaSubeKodu + "-" + p.bankaKodu, true, "KOD");
            //    if (kod != p.bankaSubeKodu)
            //    {
            //        if (hata != "") hata += "<br>";
            //        hata += "Banka bilgilerinde hata var.";
            //    }
            //}

            return hata;
        }

        /*
        Kayıt edilmek istenilen personelin TC Kimlik No kontrol eder. 
        Doğru ise true, yoksa false döndürür.
        */
        public bool TCKimlikNoKontrolu(Kullanici kullanan, string mernisNo, TNSDateTime dogumTarih)
        {
            bool mernisKontrolYapma = TNS.KYM.Arac.Tanimla().MernisKontroluYapma();
            if (mernisKontrolYapma)
                return true;

            KullaniciVAN kMernisVAN = servisKYM.MernisBilgiOku(kullanan, mernisNo, dogumTarih);
            if (kMernisVAN.sonuc.islemSonuc)
            {
                if (kMernisVAN.kullanici.mernis != mernisNo)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public int KullanilanIzinHesapla(string tcKimlikNo, int yil, TNSDateTime baslangicTarih, int izinTip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            int izin = tab.ListeleKullanilanIzin(tcKimlikNo, yil, baslangicTarih, izinTip);
            ve.VeriErisimKapat();

            return izin;
        }

        public void MemuriyetTarihleriGuncelle(string tcKimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 1);
            TNSDateTime sonGoreveBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 2);
            TNSDateTime sonBirimeBaslamaTarihi = tab.MemuriyetTarihiAl(tcKimlikNo, 3);
            tab.MemuriyetTarihleriGuncelle(tcKimlikNo, memuriyetBaslamaTarihi, sonBirimeBaslamaTarihi, sonGoreveBaslamaTarihi);
            ve.VeriErisimKapat();
            //sonBirimeBaslamaTarihi eksik hesaplanıyor. 
            //Son birim başlama tarihi update et- PERPERSONEL tablosunda SONBIRIMBASLAMATARIHI alanı
        }

        public void MemuriyetPersonelBirimGuncelle(PersonelMemuriyet p)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(p.tcKimlikNo, 3);
            if (memuriyetBaslamaTarihi.Oku() == p.baslamaTarihi.Oku() && p.ayrilmaTarihi.isNull)
                if (p.muhasebeKodu > 0 || p.organizasyonKodu > 0)
                    tab.MemuriyetPersonelBirimGuncelle(p);
            ve.VeriErisimKapat();
        }

        public void MemuriyetPersonelAyrilmaGuncelle(Kullanici kullanan, PersonelMemuriyet p)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            TNSDateTime memuriyetBaslamaTarihi = tab.MemuriyetTarihiAl(p.tcKimlikNo, 3);
            bool ayrildi = false;
            if (memuriyetBaslamaTarihi.Oku() == p.baslamaTarihi.Oku() && p.ayrilmaTarihi != null && !p.ayrilmaTarihi.isNull)
            {
                Sonuc s = tab.MemuriyetPersonelAyrilmaGuncelle(p.tcKimlikNo);
                if (s.islemSonuc)
                    ayrildi = true;
            }
            ve.VeriErisimKapat();
            if (ayrildi)
            {
                TNS.ORG.Arac.Tanimla().GenelGorevKisiTemizle(kullanan, p.tcKimlikNo);
            }
        }

        public void MemuriyetKadroBilgileriniGuncelle(Kullanici kullanan, string tcKimlikNo)
        {
            PersonelMemuriyet p = new PersonelMemuriyet();
            p.tcKimlikNo = tcKimlikNo;
            //Personelin memuriyet listesi alınıyor
            ObjectArray pler = ListeleMemuriyet(kullanan, p);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            foreach (PersonelMemuriyet pp in pler.objeler)
            {
                p = new PersonelMemuriyet();
                p.tcKimlikNo = tcKimlikNo;
                p.siraNo = pp.siraNo;
                TNSDateTime ayrilisTarihi = new TNSDateTime();
                if (pp.ayrilmaTarihi.isNull)
                    ayrilisTarihi = new TNSDateTime(DateTime.Now.ToString());
                else
                    ayrilisTarihi = pp.ayrilmaTarihi;

                IDataReader dr = tab.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 1);
                if (dr.Read())
                {
                    p.kadroDerece = VeriErisim.DegerAl(dr, 0, 0);
                    p.kadroTarih = VeriErisim.DegerAl(dr, 1);
                }
                dr.Close();

                dr = tab.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 2);
                if (dr.Read())
                {
                    p.odemeyeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
                    p.odemeyeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
                    p.odemeyeEsasTarih = VeriErisim.DegerAl(dr, 2);
                    p.odemeyeEsasGosterge = VeriErisim.DegerAl(dr, 3, 0);
                    p.odemeyeEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
                }
                dr.Close();

                dr = tab.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 3);
                if (dr.Read())
                {
                    p.kazanilmisHakDerece = VeriErisim.DegerAl(dr, 0, 0);
                    p.kazanilmisHakKademe = VeriErisim.DegerAl(dr, 1, 0);
                    p.kazanilmisHakTarih = VeriErisim.DegerAl(dr, 2);
                    p.kazanilmisGosterge = VeriErisim.DegerAl(dr, 3, 0);
                    p.kazanilmisEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
                }
                dr.Close();

                dr = tab.MemuriyeteUygunKadroBilgileriniAl(p.tcKimlikNo, ayrilisTarihi, 4);
                if (dr.Read())
                {
                    p.emekliyeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
                    p.emekliyeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
                    p.emekliyeEsasTarih = VeriErisim.DegerAl(dr, 2);
                    p.emekliyeEsasGosterge = VeriErisim.DegerAl(dr, 3, 0);
                    p.emekliyeEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
                }
                dr.Close();

                tab.MemuriyetKadroBilgileriniGuncelle(p);
            }

            ve.VeriErisimKapat();
        }

        public void KadroBilgisiPersonelGuncelle(string tcKimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            Personel p = new Personel();
            p.tcKimlikNo = tcKimlikNo;

            IDataReader dr;

            dr = tab.KadroBilgisiAl(p.tcKimlikNo, 1);
            if (dr.Read())
            {
                p.kadroOrganizasyonKodu = VeriErisim.DegerAl(dr, 0, 0);
                p.kadroMuhasebeKodu = VeriErisim.DegerAl(dr, 1, 0);
                p.kadroBirimKodu = VeriErisim.DegerAl(dr, 2, "");
                p.kadroKurum = VeriErisim.DegerAl(dr, 3, "");
                p.unvanKodu = VeriErisim.DegerAl(dr, 4, 0);
                p.sinifKodu = VeriErisim.DegerAl(dr, 5, 0);
                p.kadroDerece = VeriErisim.DegerAl(dr, 6, 0);
                p.kadroTarih = VeriErisim.DegerAl(dr, 7);
            }
            dr.Close();

            dr = tab.KadroBilgisiAl(p.tcKimlikNo, 2);
            if (dr.Read())
            {
                p.odemeEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
                p.odemeEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
                p.odemeEsasGosterge = VeriErisim.DegerAl(dr, 2, 0);
                p.odemeEsasTarih = VeriErisim.DegerAl(dr, 3);
                p.odemeEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
            }
            dr.Close();

            dr = tab.KadroBilgisiAl(p.tcKimlikNo, 3);
            if (dr.Read())
            {
                p.kazanilmisHakDerece = VeriErisim.DegerAl(dr, 0, 0);
                p.kazanilmisHakKademe = VeriErisim.DegerAl(dr, 1, 0);
                p.kazanilmisHakTarih = VeriErisim.DegerAl(dr, 2);
                p.kazanilmisHakGosterge = VeriErisim.DegerAl(dr, 3, 0);
                p.kazanilmisHakEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
            }
            dr.Close();

            dr = tab.KadroBilgisiAl(p.tcKimlikNo, 4);
            if (dr.Read())
            {
                p.emekliEsasDerece = VeriErisim.DegerAl(dr, 0, 0);
                p.emekliEsasKademe = VeriErisim.DegerAl(dr, 1, 0);
                p.emekliEsasGosterge = VeriErisim.DegerAl(dr, 2, 0);
                p.emekliEsasTarih = VeriErisim.DegerAl(dr, 3);
                p.emekliEsasEkGosterge = VeriErisim.DegerAl(dr, 4, 0);
            }
            dr.Close();

            tab.KadroBilgisiPersonelGuncelle(p);
            ve.VeriErisimKapat();
        }

        //Raporlar
        public ObjectArray ListelePersonelRapor(Kullanici kullanan, Personel p)
        {
            ObjectArray pVAN = new ObjectArray();

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListelePersonelRapor(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelPersonelRapor pp = new PersonelPersonelRapor();

                pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.soyad = VeriErisim.DegerAl(r, 2, "");
                pp.unvan = VeriErisim.DegerAl(r, 3, "");
                pp.sinif = VeriErisim.DegerAl(r, 4, "");
                pp.cinsiyet = VeriErisim.DegerAl(r, 5, 0);
                pp.dogumTarihi = VeriErisim.DegerAl(r, 6);
                pp.okulAdi = VeriErisim.DegerAl(r, 7, "");
                pp.seviyesi = VeriErisim.DegerAl(r, 8, 0);
                pp.bitirmeTarihi = VeriErisim.DegerAl(r, 9);
                pp.dil = VeriErisim.DegerAl(r, 10, "");
                pp.notuDil = VeriErisim.DegerAl(r, 11, 0);
                pp.duzeyDil = VeriErisim.DegerAl(r, 12, "");
                pp.yilDil = VeriErisim.DegerAl(r, 13, 0);
                pp.ayDil = VeriErisim.DegerAl(r, 14, 0);
                pp.belgeTurDil = VeriErisim.DegerAl(r, 15, 0);
                pp.gecerliOkul = VeriErisim.DegerAl(r, 16, 0);
                pp.fakulteBolum = VeriErisim.DegerAl(r, 17, "");

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleEgitimRapor(Kullanici kullanan, Personel p, PersonelKurs pk)
        {
            ObjectArray pVAN = new ObjectArray();

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleEgitimRapor(p, pk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelEgitimRapor pp = new PersonelEgitimRapor();

                pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.soyad = VeriErisim.DegerAl(r, 2, "");
                pp.unvan = VeriErisim.DegerAl(r, 3, "");
                pp.egitiminAdi = VeriErisim.DegerAl(r, 4, "");
                pp.egitiminTipi = VeriErisim.DegerAl(r, 5, "");
                pp.baslangicTarihi = VeriErisim.DegerAl(r, 6);
                pp.bitisTarihi = VeriErisim.DegerAl(r, 7);
                pp.dersSaati = VeriErisim.DegerAl(r, 8, 0);
                pp.egitimiVerenKurulus = VeriErisim.DegerAl(r, 9, "");
                pp.egitimYeri = VeriErisim.DegerAl(r, 10, "");
                pp.aciklama = VeriErisim.DegerAl(r, 11, "");
                pp.donemi = VeriErisim.DegerAl(r, 12, 0);
                pp.yili = VeriErisim.DegerAl(r, 13, 0);
                pp.dosyaNo = VeriErisim.DegerAl(r, 14, "");
				pp.egitimTurKodu = VeriErisim.DegerAl(r, 15, 0);
                pp.katilimTipi = VeriErisim.DegerAl(r, 16, 0);
				 

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }
		
		public ObjectArray DonemlikEgitimFaaliyetRaporu(Kullanici kullanan, Personel p, PersonelKurs pk)
        {
            ObjectArray pVAN = new ObjectArray();

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.DonemlikEgitimFaaliyetRaporu(p, pk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Aranan kriterlere uygun kayıt bulunamadı.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelEgitimRapor pp = new PersonelEgitimRapor();

                pp.egitimiVerenKurulus = VeriErisim.DegerAl(r, 0, "");
                pp.egitimTurKodu = VeriErisim.DegerAl(r, 1, 0);
                pp.egitiminAdi = VeriErisim.DegerAl(r, 2, "");
                pp.baslangicTarihi = VeriErisim.DegerAl(r, 3);
                pp.bitisTarihi = VeriErisim.DegerAl(r, 4);
                pp.gun = VeriErisim.DegerAl(r, 5,0);
                pp.dersSaati = VeriErisim.DegerAl(r, 6, 0);
                pp.unvan = VeriErisim.DegerAl(r, 7, "");
                pp.katilimciSayisi = VeriErisim.DegerAl(r, 8, 0);
                pp.egitimYeri = VeriErisim.DegerAl(r, 9, "");
                if(string.IsNullOrEmpty(pp.egitimYeri))
                    pp.egitimYeri = VeriErisim.DegerAl(r, 10, "");

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleIzinRapor(Kullanici kullanan, Personel p, PersonelIzin pk)
        {
            ObjectArray pVAN = new ObjectArray();

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleIzinRapor(p, pk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelIzinRapor pp = new PersonelIzinRapor();

                pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.soyad = VeriErisim.DegerAl(r, 2, "");
                pp.unvan = VeriErisim.DegerAl(r, 3, "");
                pp.sicilNumarasi = VeriErisim.DegerAl(r, 4, "");
                pp.baslangicTarihi = VeriErisim.DegerAl(r, 5);
                pp.bitisTarihi = VeriErisim.DegerAl(r, 6);
                pp.yillikIzin = VeriErisim.DegerAl(r, 7, 0);
                pp.tip = VeriErisim.DegerAl(r, 8, 0);
                pp.aciklama = VeriErisim.DegerAl(r, 9, "");

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public ObjectArray ListeleIzinDurum(Kullanici kullanan, PersonelIzinDurum pk)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleIzinDurum(pk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;
                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Hata oluştu. Kriterlerinizi kontrol ediniz.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelIzinDurum pp = new PersonelIzinDurum();

                pp.tip = VeriErisim.DegerAl(r, 0, 0);
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.toplamGun = VeriErisim.DegerAl(r, 2, 0);
                pp.kullanilanGun = VeriErisim.DegerAl(r, 3, 0);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc IzinEvrakNoKaydet(Kullanici kullanan, string tcKimlikNo, int siraNo, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABPersonel per = new TABPersonel(ve);

            sonuc = per.IzinEvrakNoKaydet(tcKimlikNo, siraNo, prEvrakNo);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        internal string IzinEvrakNoOku(string tcKimlikNo, int siraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABPersonel per = new TABPersonel(ve);

            string evrakNo = per.IzinEvrakNoOku(tcKimlikNo, siraNo);

            ve.VeriErisimKapat();

            return evrakNo;
        }

        //
        // Personel Yıllık İzin Taleplerinin alınması işlemleri
        //

        public ObjectArray ListeleIzinTalep(Kullanici kullanan, PersonelIzinTalep p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleIzinTalep(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                PersonelIzinTalep pi = new PersonelIzinTalep();
                pi.donemId = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMID, 0);
                pi.yil = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMYIL, 0);
                pi.donemBaslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMBASLANGICTARIHI);
                pi.donemBitisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMBITISTARIHI);
                pi.donemAciklama = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMACIKLAMA, "");
                pi.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMACIKLAMA + 1 + (int)TABPersonel.ENUMAlanlarIzinTalep.TCKIMLIKNO, "");
                pi.talepBaslangicTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMACIKLAMA + 2);
                pi.talepBitisTarihi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarPerDonem.DONEMACIKLAMA + 3);
                pi.durum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarIzinTalep.DURUMID + 1, 0);

                pVAN.objeler.Add(pi);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        internal static Sonuc BirimKoduDegistir(VeriErisim veriErisim, Kullanici kullanan, int eskiSaymanlik, string eskiBirim, int yeniSaymanlik, string yeniBirim)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                s.DegerAta("Bu işlemi yapamazsınız!");

            if (eskiSaymanlik <= 0)
                s.DegerAta("Eski saymanlik boş olamaz!");
            if (yeniSaymanlik <= 0)
                s.DegerAta("Yeni saymanlik boş olamaz!");
            if (string.IsNullOrWhiteSpace(yeniBirim))
                s.DegerAta("Yeni birim boş olamaz!");
            else if (yeniBirim.Trim().Length != 3)
                s.DegerAta("Yeni birim kodu 3 basamak olmalı!");
            if (string.IsNullOrWhiteSpace(eskiBirim))
                s.DegerAta("Eski birim boş olamaz!");
            else if (eskiBirim.Trim().Length != 3)
                s.DegerAta("Eski birim kodu 3 basamak olmalı!");
            if (!s.islemSonuc) return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
                s = tab.PersonelBirimKoduDegistir(eskiSaymanlik, eskiBirim, yeniSaymanlik, yeniBirim);
            if (s.islemSonuc)
                s = tab.MemuriyetBirimKoduDegistir(eskiSaymanlik, eskiBirim, yeniSaymanlik, yeniBirim);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
		 public ObjectArray ListeleBirimDaire(BirimDaireBilgileri p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleBirimDaireBilgileri(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                BirimDaireBilgileri d = new BirimDaireBilgileri();
                d.kod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDaire.KOD, 0);
                d.anakod = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDaire.ANAKOD, 0);
                d.birimkodu = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDaire.BIRIMKODU, "");
                d.adi = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDaire.ADI, "");
                d.durum = VeriErisim.DegerAl(r, (int)TABPersonel.ENUMAlanlarDaire.DURUM,0 );

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek belge <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }




        public ObjectArray ListeleDaireRapor(Kullanici kullanan, Personel p, PersonelMemuriyet pk)
        {
            ObjectArray pVAN = new ObjectArray();

            if ((p.muhasebeKodu > 0 && p.birimKodu.Trim().Length == 3))
            {
                if (!PER.Yetki.YetkisiVarMiBirime(kullanan, p.muhasebeKodu.ToString(), p.birimKodu, p.organizasyonKodu))
                {
                    pVAN.sonuc.islemSonuc = false;
                    pVAN.sonuc.hataStr = "Verdiğiniz birim bilgilerini görme yetkiniz yok.";

                    return pVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleDaireRapor(p, pk);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = true;
                pVAN.sonuc.bilgiStr = "Arama yapılabilmesi için bir kriter bilgisi girilmesi gerekmektedir.";
                return pVAN;
            }

            while (r.Read())
            {
                PersonelDaireBilgileriRapor pp = new PersonelDaireBilgileriRapor();

                pp.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                pp.ad = VeriErisim.DegerAl(r, 1, "");
                pp.soyad = VeriErisim.DegerAl(r, 2, "");
                pp.unvan = VeriErisim.DegerAl(r, 3, "");
                pp.daireAdi = VeriErisim.DegerAl(r, 4, "");
                pp.iseBaslamaTarihi = VeriErisim.DegerAl(r, 5);
                pp.istenCikisTarihi = VeriErisim.DegerAl(r, 6);
               
                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public string ListeleEgitimTurleriAdlari(Kullanici kullanan, int kod)
        {
            string egitimAdi = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleEgitimTurleriRecursive(kod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();

                return e.Message;
            }

            ObjectArray arr=new ObjectArray();
            while (r.Read())
            {
                arr.Ekle(VeriErisim.DegerAl(r, 1, ""));          
            }
            r.Close();

            for (int i = 0; i < arr.ObjeSayisi; i++)
            {
                egitimAdi += arr[i];
                if (i != arr.ObjeSayisi - 1)
                    egitimAdi += "-";
            }
           
            ve.VeriErisimKapat();

            return egitimAdi;

        }
        public ObjectArray ListeleEgitimTurBilgileriRecursive(Kullanici kullanan, int kod)
        {
            ObjectArray arr = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonel tab = new TABPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.ListeleEgitimTurleriRecursive(kod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                arr.sonuc.islemSonuc = false;
                arr.sonuc.hataStr = e.Message;

                return arr;
            }

            while (r.Read())
            {
                PersonelEgitimTurleri d = new PersonelEgitimTurleri();
                d.kod = VeriErisim.DegerAl(r, 0, 0);
                d.ad = VeriErisim.DegerAl(r, 1, "");
                d.ataKod = VeriErisim.DegerAl(r, 2, 0);
                d.seviye = VeriErisim.DegerAl(r, 3, 0);
                arr.objeler.Add(d);
            }
            r.Close();
            ve.VeriErisimKapat();
            return arr;

        }											  
    }
}
