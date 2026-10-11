using System;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.MUH;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMAmortismanIslemForm
    {
        /// <summary>
        /// Amortisman entegrasyonunun olup olmadığını tutan değişken
        /// </summary>
        private static string amortismanDeger = System.Configuration.ConfigurationManager.AppSettings.Get("Amortisman");

        private static Sonuc Kontrol(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form)
        {
            Sonuc sonuc = new Sonuc();
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (form.yil == 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (form.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (form.harcamaKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (form.ambarKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

            //if (form.muhasebeKod != String.Empty)
            //{
            //    if (TMMServis.UzayYaziDegerGetir("TASMUHASEBE", form.muhasebeKod, true, "") == String.Empty)
            //        sonuc.hataStr += string.Format(TasinirMalResI.TMM096, form.muhasebeKod) + "<br>";
            //}
            //if (form.muhasebeKod != String.Empty && form.harcamaKod != String.Empty)
            //{
            //    if (TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", form.muhasebeKod + "-" + form.harcamaKod, true, "") == String.Empty)
            //        sonuc.hataStr += string.Format(TasinirMalResI.TMM097, form.muhasebeKod, form.harcamaKod) + "<br>";
            //}
            //if (form.muhasebeKod != String.Empty && form.harcamaKod != String.Empty && form.ambarKod != String.Empty && form.tip != "DEGERARTIS")
            //{
            //    if (TMMServis.UzayYaziDegerGetir("TASAMBAR", form.muhasebeKod + "-" + form.harcamaKod + "-" + form.ambarKod, true, "") == String.Empty)
            //        sonuc.hataStr += string.Format(TasinirMalResI.TMM098, form.muhasebeKod, form.harcamaKod, form.ambarKod) + "<br>";
            //}

            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            int durum = tabAmortismanIslemForm.DurumGetirTekKolon(form);
            if (durum == (int)ENUMAmortismanFormDurumu.ONAYLI)
            {
                if (!string.IsNullOrEmpty(form.fisNo))
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM271, form.fisNo) + "<br>";
                else
                {
                    sonuc.DegerAta(form.yil + "/" + form.donem + " dönemine ait amortisman kayıtları onaylandığı için işlem yapılamaz");
                }
            }
            else if (durum == 0)
                form.durum = (int)ENUMBelgeDurumu.YENI;
            else
                form.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            //B-A Onay Kontrolü
            if (Arac.MerkezBankasiKullaniyor())
            {
                ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabAmortismanIslemForm.OnayDurumGetirTekKolon(form);

                string belgeNo = "";
                if (form.fisNo != "")
                    belgeNo = form.fisNo + " no\'lu belge ";

                if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                    sonuc.hataStr += string.Format("{0} B Onayınını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                    sonuc.hataStr += string.Format("{0} A Onayını beklediği için değişiklik yapılamaz.", belgeNo) + "<br>";
            }

            if (sonuc.hataStr != String.Empty)
                sonuc.islemSonuc = false;

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = Kontrol(ve, kullanan, form);
            if (sonuc.hataStr != String.Empty)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);
            TABGenel tabGenel = new TABGenel(ve2);

            ve.BeginTransaction();

            if (string.IsNullOrEmpty(form.kod))
                form.kod = Genel.KodAl();

            if (string.IsNullOrEmpty(form.fisNo))
            {
                //string sayacKod = "AMOR" + form.yil.ToString() + form.muhasebeKod + form.harcamaKod.Replace(".", "");

                //string fisNoEkAlan = "A";
                //if (form.tip == "DEGERARTIS")
                //    fisNoEkAlan = "B";
                //else if (form.tip == "ENFLASYONDUZELTMESI")
                //    fisNoEkAlan = "E";

                //form.fisNo = fisNoEkAlan + tabGenel.BelgeNoAl(sayacKod, "000000");

                string fisNoEkAlan = "";
                string sayacKod = "AMOR" + form.yil.ToString() + form.muhasebeKod;

                if (form.tip == "ENFLASYONDUZELTMESI")
                {
                    fisNoEkAlan = "E";
                    form.fisNo = fisNoEkAlan + tabGenel.BelgeNoAl(fisNoEkAlan + sayacKod, "000000");
                }
                else if (form.tip == "DEGERARTIS")
                {
                    fisNoEkAlan = "B";
                    form.fisNo = fisNoEkAlan + tabGenel.BelgeNoAl(fisNoEkAlan + sayacKod, "000000");
                }
                else
                {
                    fisNoEkAlan = "A";
                    form.fisNo = fisNoEkAlan + tabGenel.BelgeNoAl(fisNoEkAlan + sayacKod, "000000");
                }

                form.durum = (int)ENUMBelgeDurumu.YENI;

                if (form.fisNo == string.Empty)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM083;
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    return sonuc;
                }

                form.fisTarih = new TNSDateTime(DateTime.Now.Date);
            }
            else
                form.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            form.islemTarih = new TNSDateTime(DateTime.Now);
            form.islemYapan = kullanan.kullaniciKodu;
            //form.ipAdresi = kullanan.telefon;

            sonuc = tabAmortismanIslemForm.Kaydet(form);

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM200 + "" + sonuc.hataStr;
                ve.Rollback();
                sonuc.vtHatasi = true;
            }
            else if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.fisNo + "-" + form.kod;
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM199, form.fisNo);
                ve.Commit();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Oku(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form, bool tekil)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            ObjectArray bilgi = new ObjectArray();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (form.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                if (form.muhasebeKod == string.Empty)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";

                if (tekil)
                {
                    if (form.harcamaKod == string.Empty)
                        bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                }

                if (bilgi.sonuc.hataStr != String.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            IDataReader dr = tabAmortismanIslemForm.Oku(form, tekil);

            int sonSiraNo = (int)TABAmortismanIslemForm.ENUMAlanlar.FISACIKLAMA;
            while (dr.Read())
            {
                AmortismanIslemForm af = new AmortismanIslemForm();
                af.kod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.KOD, string.Empty);
                af.yil = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.YIL, 0);
                af.donem = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DONEM, 0);
                af.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                af.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty));
                af.fisNo = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISNO, string.Empty);
                af.fisTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISTARIH);
                af.ambarKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.AMBARKOD, string.Empty);
                af.durum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DURUM, 0);
                af.islemTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMTARIH);
                af.islemYapan = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMYAPAN, string.Empty);
                af.onayDurum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                af.fisAciklama = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISACIKLAMA, "");

                af.muhasebeAd = VeriErisim.DegerAl(dr, sonSiraNo + 1, string.Empty);
                af.harcamaAd = VeriErisim.DegerAl(dr, sonSiraNo + 2, string.Empty);
                af.ambarAd = VeriErisim.DegerAl(dr, sonSiraNo + 3, string.Empty);

                //form.detay = TMMTasinirIslemDetay.Listele(form, siraNoSirali);

                bilgi.Ekle(af);
            }
            dr.Close();
            ve.VeriErisimKapat();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM086;
            }
            else if (bilgi.ObjeSayisi > 1 && tekil) //Buraya hiç bir zaman girmezsen sevinirim ;)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM272;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.objeler.Clear();
                return bilgi;
            }

            return bilgi;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            ObjectArray bilgi = new ObjectArray();

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, form.muhasebeKod, form.harcamaKod, form.ambarKod);
            //KUllanıcı biriminde yetkili olan kişi

            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (form.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";

                if (bilgi.sonuc.hataStr != string.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            IDataReader dr = tabAmortismanIslemForm.Listele(form);

            if (ve.hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr += ve.hata;
                bilgi.sonuc.vtHatasi = true;

                if (dr != null)
                    dr.Close();

                ve.VeriErisimKapat();

                return bilgi;
            }

            int sonSiraNo = (int)TABAmortismanIslemForm.ENUMAlanlar.FISACIKLAMA;
            while (dr.Read())
            {
                AmortismanIslemForm aForm = new AmortismanIslemForm();

                aForm.kod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.KOD, string.Empty);
                aForm.yil = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.YIL, 0);
                aForm.donem = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DONEM, 0);
                aForm.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                aForm.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty));
                aForm.fisNo = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISNO, string.Empty);
                aForm.fisTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISTARIH);
                aForm.ambarKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.AMBARKOD, string.Empty);
                aForm.durum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DURUM, 0);
                aForm.islemTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMTARIH);
                aForm.islemYapan = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMYAPAN, string.Empty);
                aForm.onayDurum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                aForm.fisAciklama = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISACIKLAMA, "");

                aForm.muhasebeAd = VeriErisim.DegerAl(dr, sonSiraNo + 1, "");
                aForm.harcamaAd = VeriErisim.DegerAl(dr, sonSiraNo + 2, "");
                aForm.ambarAd = VeriErisim.DegerAl(dr, sonSiraNo + 3, "");

                bilgi.Ekle(aForm);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static int BelgeDurumGetir(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form)
        {
            if (form.yil == 0)
                return (int)ENUMBelgeDurumu.TANIMSIZ;
            if (string.IsNullOrEmpty(form.muhasebeKod))
                return (int)ENUMBelgeDurumu.TANIMSIZ;
            if (string.IsNullOrEmpty(form.harcamaKod))
                return (int)ENUMBelgeDurumu.TANIMSIZ;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm t = new TABAmortismanIslemForm(ve);
            return t.DurumGetirTekKolon(form);
        }

        public static Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form, ENUMAmortismanFormDurumu yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            form.islemYapan = kullanan.kullaniciKodu;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            ObjectArray listeFis = Oku(ve, kullanan, form, true);
            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM276;
                ve.VeriErisimKapat();
                return sonuc;
            }

            form = (AmortismanIslemForm)listeFis[0];

            //sonuc = Kontrol(ve, kullanan, form);
            //if (sonuc.hataStr != "")
            //{
            //    sonuc.islemSonuc = false;
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            if (form.durum == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM086;
                ve.VeriErisimKapat();
                return sonuc;
            }


            if (form.durum == (int)ENUMAmortismanFormDurumu.ONAYLI && yeniDurum == ENUMAmortismanFormDurumu.IPTAL)
                sonuc.hataStr = string.Format(TasinirMalResI.TMM273, form.fisNo) + "<br>"; //Test amaçlı onaylı belge iptal edilebilsin.
            else if (form.durum == (int)ENUMAmortismanFormDurumu.ONAYLI && yeniDurum == ENUMAmortismanFormDurumu.ONAYLI)
                sonuc.hataStr = string.Format(TasinirMalResI.TMM274, form.fisNo) + "<br>";
            else if (form.durum != (int)ENUMAmortismanFormDurumu.ONAYLI && yeniDurum == ENUMAmortismanFormDurumu.ONAYKALDIR)
                sonuc.hataStr = string.Format(TasinirMalResI.TMM274, form.fisNo) + "<br>";

            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //B-A onay kontrolü
            if (Arac.MerkezBankasiKullaniyor())
            {
                if (form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB || form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                {
                    if (yeniDurum == ENUMAmortismanFormDurumu.IPTAL)
                        sonuc.hataStr = "Belgenin durumu <b>İptal</b> için uygun değil!<br><br><b>Not:</b> Belge B/A Onayına gönderildi. İptal etmek için B/A onay yetklisi tarafından geri gönderilmesi gerekir.";

                    if (sonuc.hataStr != "")
                    {
                        sonuc.islemSonuc = false;
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }
            }

            ve.BeginTransaction();

            if (sonuc.hataStr == "" && sonuc.islemSonuc)
            {
                form.islemYapan = kullanan.kullaniciKodu;
                sonuc = tabAmortismanIslemForm.DurumDegistir(form, yeniDurum);

                if (sonuc.islemSonuc)
                {
                    string tarihceAciklama = AmortismanIslemForm.FormDurumAciklama(yeniDurum);
                    //form.ipAdresi = kullanan.telefon;
                    tabAmortismanIslemForm.KaydetTarihce(form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo, kullanan.kullaniciKodu, tarihceAciklama, form.kod, "");
                    sonuc.bilgiStr = string.Format(TasinirMalResI.TMM288, form.fisNo) + "<br>";

                    sonuc.anahtar = form.fisNo;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc OnayDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form, ENUMTasinirIslemFormOnayDurumu yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            form.islemYapan = kullanan.kullaniciKodu;
            string fisAciklama = form.fisAciklama;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            ObjectArray listeFis = Oku(ve, kullanan, form, true);
            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM276;
                ve.VeriErisimKapat();
                return sonuc;
            }

            form = (AmortismanIslemForm)listeFis[0];

            if (form.durum == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM086;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //**************Onay Durum Kontrolü****************************************

            string bilgi = "";

            if (form.durum == (int)ENUMAmortismanFormDurumu.ONAYLI)
                sonuc.DegerAta("Belge onaylı olduğu için onaya gönderilemez!<br>");
            else if (form.durum == (int)ENUMAmortismanFormDurumu.IPTAL)
                sonuc.DegerAta("Belge iptal edildiği için onaya gönderilemez!<br>");
            else if (form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA && yeniDurum != ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
            {
                //B onayına yapan kişi A onayını yapamaz. Kontrolü eklenecek
                string bOnayiVerenKisi = "";
                ObjectArray listeTarihce = new TMMTarihce().IslemFormListele(kullanan, form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo);
                foreach (TarihceBilgisi tarihce in listeTarihce.objeler)
                {
                    if (tarihce.islem == "A Onayına Gönderildi")
                        bOnayiVerenKisi = tarihce.islemYapan;
                }

                if (bOnayiVerenKisi == kullanan.kullaniciKodu)
                    sonuc.DegerAta("B onayını veren kullanıcı A onayını veremez!<br>");
            }

            if (sonuc.hataStr == "")
            {
                if ((form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ || form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI) && yeniDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)//B-Onayına gönderildi
                    bilgi = "B Onayına Gönderildi";
                else if (form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIB && yeniDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)//A-Onayına gönderildi
                    bilgi = "A Onayına Gönderildi";
                else if (form.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA && yeniDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)//A-Onaylandı
                    bilgi = TasinirMalResI.TMM009;
                else if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
                    bilgi = "B-A Onayından Geri Gonderildi";
                else
                    sonuc.DegerAta("Onay durumu uygun değil!<br>");
                //******************************************************
            }

            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            bool muhasebeKaydiOlustur = false;

            if (sonuc.hataStr == "" && sonuc.islemSonuc)
            {
                ve.BeginTransaction();

                if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                {
                    if (form.fisNo.StartsWith("A"))//AMORTİSMAN
                    {
                        sonuc = DurumDegistir(ve, kullanan, form, ENUMAmortismanFormDurumu.ONAYLI);
                    }
                    else if (form.fisNo.StartsWith("E"))//ENFLASYON DÜZELTMESİ
                    {
                        sonuc = DurumDegistir(ve, kullanan, form, ENUMAmortismanFormDurumu.ONAYLI);
                    }
                    else if (form.fisNo.StartsWith("B"))//DEĞERARTIŞI
                    {
                        SicilNoDegerArtis dbilgi = new SicilNoDegerArtis
                        {
                            muhasebeKod = form.muhasebeKod,
                            harcamaKod = form.harcamaKod,
                            belgeNo = form.donem.ToString("000000")
                        };

                        VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

                        ObjectArray dListe = TMMSicilNoDegerArtis.Listele(ve2, kullanan, dbilgi);
                        dbilgi = (SicilNoDegerArtis)dListe[0];

                        ve2.VeriErisimKapat();


                        sonuc = TMMSicilNoDegerArtis.GuncelleDurum(kullanan, dbilgi, "Onay");

                        //Değer artış onay sonucunda Tarihçe tablosuna onaylandı bilgisi yaz.
                        if (sonuc.islemSonuc)
                        {
                            tabAmortismanIslemForm.KaydetTarihce(form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo, kullanan.kullaniciKodu, bilgi, form.kod, "");
                            sonuc.bilgiStr = string.Format("{0} belgenin onay durumu değiştirildi.", form.fisNo) + "<br>";
                        }
                        //*********************************************************************


                    }
                    //*************************************************************************************

                    //Merkez Bankası Muhasebe bilgi kaydı oluştur.
                    if (sonuc.islemSonuc)
                        muhasebeKaydiOlustur = true;

                }

                if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
                {
                    if (form.fisNo.StartsWith("A"))//AMORTİSMAN
                    {
                        //sonuc = DurumDegistir(ve, kullanan, form, ENUMAmortismanFormDurumu.GER);
                    }
                    else if (form.fisNo.StartsWith("E"))//ENFLASYON DÜZELTMESİ
                    {
                        //sonuc = DurumDegistir(ve, kullanan, form, ENUMAmortismanFormDurumu.GER);
                    }
                    else if (form.fisNo.StartsWith("B"))//DEĞERARTIŞI
                    {
                        SicilNoDegerArtis dbilgi = new SicilNoDegerArtis();
                        dbilgi.muhasebeKod = form.muhasebeKod;
                        dbilgi.harcamaKod = form.harcamaKod;
                        dbilgi.belgeNo = form.donem.ToString("000000");

                        ObjectArray dListe = TMMSicilNoDegerArtis.Listele(null, kullanan, dbilgi);
                        dbilgi = (SicilNoDegerArtis)dListe[0];

                        sonuc = TMMSicilNoDegerArtis.GuncelleDurum(kullanan, dbilgi, "GeriGonderildi");
                    }
                }

                if (sonuc.islemSonuc)
                {
                    //ve.BeginTransaction();
                    form.islemYapan = kullanan.kullaniciKodu;
                    form.fisAciklama = yeniDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB ? fisAciklama : ""; //Amortisman form onaya gönderilirken açıklama giriliyor.

                    sonuc = tabAmortismanIslemForm.OnayDurumDegistir(form, yeniDurum, fisAciklama);
                }

                if (sonuc.islemSonuc && yeniDurum != ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                {
                    //form.ipAdresi = kullanan.telefon;
                    tabAmortismanIslemForm.KaydetTarihce(form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo, kullanan.kullaniciKodu, bilgi, form.kod, "");
                    sonuc.bilgiStr = string.Format("{0} belgenin onay durumu değiştirildi.", form.fisNo) + "<br>";

                    sonuc.anahtar = form.fisNo;
                }
            }

            bool commit = false;

            if (sonuc.islemSonuc)
            {
                if (Arac.MerkezBankasiKullaniyor())
                {
                    if (muhasebeKaydiOlustur)
                    {
                        bool muhasebeKaydiZorunlu = false;

                        AyarlarMB ayarlar = new AyarlarMB
                        {
                            muhasebeKod = form.muhasebeKod,
                            harcamaKod = form.harcamaKod,
                            ambarKod = form.ambarKod
                        };

                        ObjectArray liste = TMMAyarlarMB.Listele(ve, kullanan, ayarlar);
                        if (liste.sonuc.islemSonuc && liste.objeler.Count == 1)
                        {
                            AyarlarMB a = (AyarlarMB)liste.objeler[0];
                            muhasebeKaydiZorunlu = a.aOnayiMuhasebeKontrolu == 1 ? true : false;
                        }

                        if (!muhasebeKaydiZorunlu)
                            commit = true;


                        //Merkez Bankası Muhasebe bilgi kaydı oluştur.
                        ENUMMuhasebeIslemTur muhasebeIslemTur = ENUMMuhasebeIslemTur.AMORTISMAN;
                        if (form.fisNo.StartsWith("B"))
                            muhasebeIslemTur = ENUMMuhasebeIslemTur.DEGERARTIS;
                        else if (form.fisNo.StartsWith("E"))
                            muhasebeIslemTur = ENUMMuhasebeIslemTur.ENFLASYONDUZELTMESI;


                        MuhasebeIslemiKriter mk = new MuhasebeIslemiKriter()
                        {
                            yil = form.yil,
                            muhasebeKod = form.muhasebeKod,
                            harcamaKod = form.harcamaKod,
                            ambarKod = form.ambarKod,
                            fisNo = form.fisNo,
                            donem = form.donem,
                            muhasebeIslemTur = muhasebeIslemTur
                        };

                        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
                        mk.mutemmim = servisTMM.MutemmimAktifMi(kullanan, new AyarlarMB { muhasebeKod = form.muhasebeKod, harcamaKod = form.harcamaKod, ambarKod = form.ambarKod }) ? 1 : 0;

                        Sonuc sonuc2 = TMMMuhasebeBilgi.Kaydet(ve, kullanan, mk);
                        if (!sonuc2.islemSonuc)
                            TMMMuhasebeBilgi.HataDosyaYaz("ENUMMuhasebeIslemTur:" + mk.muhasebeIslemTur + " - ", sonuc.hataStr);
                        else if (muhasebeKaydiZorunlu)
                            commit = true;

                    }
                    else
                        commit = true;
                }
                else
                    commit = true;
            }

            if (commit)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();


            if (commit && Arac.MerkezBankasiKullaniyor())
            {
                if (bilgi == "B Onayına Gönderildi" || bilgi == "A Onayına Gönderildi")
                {
                    TasinirIslemForm tfIslem = new TasinirIslemForm();
                    tfIslem.fisNo = form.fisNo;
                    tfIslem.fisTarih = form.fisTarih;
                    tfIslem.harcamaAd = form.harcamaAd;

                    if (form.fisNo.StartsWith("A"))
                        tfIslem.islemTipAd = "Amortisman";
                    else if (form.fisNo.StartsWith("B"))
                        tfIslem.islemTipAd = "Değer Artış";
                    else if (form.fisNo.StartsWith("E"))
                        tfIslem.islemTipAd = "Enflasyon Düzeltmesi";

                    tfIslem.muhasebeKod = form.muhasebeKod;
                    tfIslem.harcamaKod = form.harcamaKod;

                    TMMTasinirIslemForm.BAOnayinaGonderildiMailAt(kullanan, tfIslem, bilgi);
                }
            }

            return sonuc;
        }

        public static bool AmortismanYapilmisMi(VeriErisim veriErisim, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);
            bool sonuc = tabAmortismanIslemForm.AmortismanYapilmisMi(ak);
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static AmortismanIslemForm SonAmortisman(VeriErisim veriErisim, Kullanici kullanan, AmortismanIslemForm form)
        {
            AmortismanIslemForm aForm = new AmortismanIslemForm();

            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanIslemForm tabAmortismanIslemForm = new TABAmortismanIslemForm(ve);

            IDataReader dr = tabAmortismanIslemForm.SonAmortisman(form);

            if (dr != null)
            {
                while (dr.Read())
                {
                    aForm.kod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.KOD, string.Empty);
                    aForm.yil = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.YIL, 0);
                    aForm.donem = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DONEM, 0);
                    aForm.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                    aForm.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty));
                    aForm.fisNo = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISNO, string.Empty);
                    aForm.fisTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISTARIH);
                    aForm.ambarKod = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.AMBARKOD, string.Empty);
                    aForm.durum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.DURUM, 0);
                    aForm.islemTarih = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMTARIH);
                    aForm.islemYapan = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ISLEMYAPAN, string.Empty);
                    aForm.onayDurum = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                    aForm.fisAciklama = VeriErisim.DegerAl(dr, (int)TABAmortismanIslemForm.ENUMAlanlar.FISACIKLAMA, "");
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return aForm;
        }

    }
}