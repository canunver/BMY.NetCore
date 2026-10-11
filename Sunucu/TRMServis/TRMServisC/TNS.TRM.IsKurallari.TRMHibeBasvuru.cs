using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeBasvuru
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, Basvuru form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (form.hibeKod == 0)
                hata += "Hibe Kod boş bırakılamaz." + "<br>";
            if (form.turKod == 0)
                hata += "Tür Kod boş bırakılamaz." + "<br>";
            if (form.bolumKod == "")
                hata += "Bölüm Kod boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru tGrup = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            if (form.turKod == (int)ENUMHibeTur.EKONOMIKYATIRIM)
            {
                int durum = new TRMHibeYatirim().BasvuruDurumBilgisi(form.hibeKod.ToString());
                if (durum == (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI)
                {
                    sonuc = tGrup.BasvuruKopyasiAl(form.hibeKod);
                    ve.hata = "";
                }
            }

            sonuc = tGrup.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);

                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = form.hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Başvuru formu kaydetme. Bölüm:" + form.bolumKod;
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı." + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, Basvuru form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (form.hibeKod == 0)
                hata += "Hibe Kod boş bırakılamaz." + "<br>";
            if (form.turKod == 0)
                hata += "Tür Kod boş bırakılamaz." + "<br>";
            if (form.bolumKod == "")
                hata += "Bölüm Kod boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Basvuru[] Listele(VeriErisim ve, Kullanici kullanan, Basvuru form)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            IDataReader r = to.Listele(form);

            ObjectArray bler = new ObjectArray();
            while (r.Read())
            {
                Basvuru b = new Basvuru();
                b.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlar.HIBEKOD, 0);
                b.turKod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlar.TURKOD, 0);
                b.bolumKod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlar.BOLUMKOD, "");
                b.metin = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlar.METIN, "");

                bler.objeler.Add(b);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (Basvuru[])bler.objeler.ToArray(typeof(Basvuru));
        }

        public static Sonuc KaydetSablon(VeriErisim ve, Kullanici kullanan, BasvuruSablon form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))))
                return sonuc;

            if (form.turKod == 0)
                hata += "Tür Kod boş bırakılamaz." + "<br>";
            if (form.bolumKod == "")
                hata += "Bölüm Kod boş bırakılamaz." + "<br>";
            if (form.bolumAd == "")
                hata += "Bölüm Ad boş bırakılamaz." + "<br>";
            if (form.etapNo == 0)
                hata += "Etap No boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            if (form.kod == 0)
                form.kod = to.EnBuyukKodSablon();

            sonuc = to.KaydetSablon(form);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı." + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SilSablon(VeriErisim ve, Kullanici kullanan, BasvuruSablon form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))))
                return sonuc;

            if (form.kod == 0)
                hata += "Kod boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.SilSablon(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static BasvuruSablon[] ListeleSablon(VeriErisim ve, Kullanici kullanan, BasvuruSablon form)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            IDataReader r = to.ListeleSablon(form);

            ObjectArray bler = new ObjectArray();
            while (r.Read())
            {
                BasvuruSablon b = new BasvuruSablon();
                b.kod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.KOD, 0);
                b.turKod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.TURKOD, 0);
                b.etapNo = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.ETAPNO, 0);
                b.bolumKod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.BOLUMKOD, "");
                b.bolumAd = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.BOLUMAD, "");
                b.aciklama = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.ACIKLAMA, "");
                b.sablon = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.SABLON, "");
                b.tabloGirisSekli = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.TABLOGIRISSEKLI, 0);
                b.altYapiA = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.ALTYAPIA, 0);
                b.altYapiB = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarSablon.ALTYAPIB, 0);

                bler.objeler.Add(b);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (BasvuruSablon[])bler.objeler.ToArray(typeof(BasvuruSablon));
        }

        public static Sonuc KaydetOnlineBasvuruParola(VeriErisim ve, OnlineBasvuruKaydi form)
        {
            Sonuc sonuc = new Sonuc();

            Random rnd = new Random();
            string yeniParola = rnd.Next(10000, 99999).ToString();

            //Parola alanını SHA1 ile şifrele
            //**************************************************
            form.parola = KYM.Yetki.Sifrele(yeniParola);
            //**************************************************

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.GuncelleOnlineBasvuruParola(form.kod, form.parola);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                form.parola = yeniParola;
                string sPosta = OnlineBasvuruParolaPostasiGonder(form);
                if (sPosta == "1")
                    sonuc.bilgiStr = form.ePostaAdresi + " adresine yeni parolanız gönderildi. Lütfen posta kutunuzu kontrol edin." + "<br>";
                else
                    sonuc.bilgiStr = "Posta gönderilemedi." + "<br>" + sPosta;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetOnlineBasvuru(VeriErisim ve, OnlineBasvuruKaydi form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = KaydetOnlineBasvuruKontrol(form);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parola alanını SHA1 ile şifrele
            //**************************************************
            form.parola = KYM.Yetki.Sifrele(form.parola);
            //**************************************************

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, "OnlineBasvuru");
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            if (string.IsNullOrEmpty(form.kod))
            {
                form.kod = Guid.NewGuid().ToString();
                sonuc = to.KaydetOnlineBasvuru(form);
            }
            else
            {
                sonuc = to.GuncelleOnlineBasvuru(form);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod;

                ve.Commit();

                if (form.postaGonder)
                {
                    string sPosta = OnlineBasvuruBilgiPostasiGonder(form);
                    sPosta = OnlineBasvuruAktivasyonPostasiGonder(form);
                    if (sPosta == "1")
                        sonuc.bilgiStr = form.ePostaAdresi + " adresine aktivasyon bilgisi gönderildi. Lütfen posta kutunuzu kontrol edin. Aktivasyon işlemini yaptıktan sonra sisteme giriş yapabilirsiniz." + "<br>";
                }
                else
                    sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı." + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string OnlineBasvuruAktivasyonPostasiGonder(OnlineBasvuruKaydi form)
        {
            if (string.IsNullOrEmpty(form.kod) || string.IsNullOrEmpty(form.ePostaAdresi))
                return "";

            string konu = "Hibe Başvurusu Giriş - Aktivasyon Bilgisi";
            string aktivasyonLinki = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimURL", "");
            aktivasyonLinki += "/Tarim/OnlineBasvuruAktivasyon.aspx?id=" + form.kod;
            string icerik = "Sayın yatırımcı,<br>Online hibe desteği üyelik başvurunuzu aktif etmek için <a href='" + aktivasyonLinki + "' target='_blank'>buraya tıklayın</a>.";
            icerik += "<br><br>Yatırımcı Kimlik No: " + form.kimlikNo + "<br>Yatırımcı Adı: " + form.adi + " " + form.soyadi + "<br><br>e-posta Adresi: " + form.ePostaAdresi;
            icerik += "<br><br><br>İyi çalışmalar dileriz.<br><br>Tarım ve Orman Bakanlığı - Kırsal Kalkınma Daire Başkanlığı";

            string yazilimMailAdresi = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
            return OrtakFonksiyonlar.MailAt(yazilimMailAdresi, form.ePostaAdresi, konu, icerik, true, false, null);
        }

        public static string OnlineBasvuruBilgiPostasiGonder(OnlineBasvuruKaydi form)
        {
            if (string.IsNullOrEmpty(form.kod) || string.IsNullOrEmpty(form.ePostaAdresi))
                return "";

            string konu = "Hibe Başvurusu Giriş - Kullanıcı Bilgileri";
            string icerik = "Sayın yatırımcı,<br>Online hibe desteği üyelik başvuru bilgileriniz başarıyla kayıt edilmiştir. Daha sonraki işlemleriniz için gerekli olabilecek bilgiler aşağıdadır. Lütfen bu postayı saklayınız.<br><br>Yatırımcı Kimlik No: " + form.kimlikNo + "<br>Yatırımcı Adı: " + form.adi + " " + form.soyadi + "<br><br>e-posta Adresi: " + form.ePostaAdresi;
            icerik += "<br><br><br>İyi çalışmalar dileriz.<br><br>Tarım ve Orman Bakanlığı - Kırsal Kalkınma Daire Başkanlığı";

            string yazilimMailAdresi = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
            return OrtakFonksiyonlar.MailAt(yazilimMailAdresi, form.ePostaAdresi, konu, icerik, true, false, null);
        }

        public static string OnlineBasvuruParolaPostasiGonder(OnlineBasvuruKaydi form)
        {
            if (string.IsNullOrEmpty(form.kod) || string.IsNullOrEmpty(form.ePostaAdresi))
                return "";

            string konu = "Hibe Başvurusu Giriş - Yeni Parola Talebi";
            string icerik = "Sayın yatırımcı,<br>Online hibe desteği üyeliği için yeni parolanız aşağıdadır.<br><br><br><br>Yatırımcı Kimlik No: " + form.kimlikNo + "<br>Yatırımcı Adı: " + form.adi + " " + form.soyadi + "<br><br>Yeni parolanız :" + form.parola;
            icerik += "<br><br><br>İyi çalışmalar dileriz.<br><br>Tarım ve Orman Bakanlığı - Kırsal Kalkınma Daire Başkanlığı";

            string yazilimMailAdresi = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
            return OrtakFonksiyonlar.MailAt(yazilimMailAdresi, form.ePostaAdresi, konu, icerik, true, false, null);
        }

        public static Sonuc KaydetOnlineBasvuruAktivasyon(VeriErisim ve, string kod, int aktif)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(kod))
            {
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.GuncelleOnlineBasvuruAktivasyon(kod, aktif);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                string durum = "";
                if (aktif == 1) durum = "aktif";
                else durum = "pasif";

                sonuc.bilgiStr = "Hesabınız başarıyla " + durum + " hale getirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static string KaydetOnlineBasvuruKontrol(OnlineBasvuruKaydi form)
        {
            string hata = string.Empty;

            if (form.kimlikNo.Trim() == "")
                hata += "Kimlik Kod bilgisi boş bırakılamaz." + "<br>";
            if (form.ePostaAdresi.Trim() == "")
                hata += "E-Posta adresi boş bırakılamaz." + "<br>";
            //if (form.ePostaAdresiTekrar.Trim() == "")
            //    hata += "E-Posta adresi (Tekrar) boş bırakılamaz." + "<br>";
            if (form.parola.Trim() == "")
                hata += "Parola bilgisi boş bırakılamaz." + "<br>";
            else if (form.parola.Trim().Length < 5)
                hata += "Parola bilgisi en az 5 karakterden oluşmalıdır." + "<br>";
            if (form.parolaTekrar.Trim() == "")
                hata += "Parola (Tekrar) bilgisi boş bırakılamaz." + "<br>";
            //if (form.guvenlikSorusu == 0)
            //    hata += "Güvenlik sorusu boş bırakılamaz." + "<br>";
            //if (form.guvenlikSorusuCevap.Trim() == "")
            //    hata += "Güvenlik Sorusu cevabı bilgisi boş bırakılamaz." + "<br>";
            if (form.kimlikNo.Trim().Length == 11)
            {
                if (form.adi.Trim() == "")
                    hata += "Ad bilgisi boş bırakılamaz." + "<br>";
                if (form.soyadi.Trim() == "")
                    hata += "Soyad bilgisi boş bırakılamaz." + "<br>";
                if (form.babaAdi.Trim() == "")
                    hata += "Baba Ad bilgisi boş bırakılamaz." + "<br>";
            }
            else if (form.kimlikNo.Trim().Length == 10)
            {
                if (form.adi.Trim() == "")
                    hata += "Firma Adı bilgisi boş bırakılamaz." + "<br>";
                if (form.vergiDairesi.Trim() == "")
                    hata += "Firma Vergi Dairesi bilgisi boş bırakılamaz." + "<br>";
            }
            else
            {
                hata += "Girmiş olduğunuz T.C Kimlik / Vergi Numarası hatalı. Lütfen tekrar giriş yapınız." + "<br>";
            }

            if (!string.IsNullOrEmpty(form.ePostaAdresi.Trim()))//&& !string.IsNullOrEmpty(form.ePostaAdresiTekrar.Trim())
            {
                //if (form.ePostaAdresi.Trim() != form.ePostaAdresiTekrar.Trim())
                //    hata += "Girdiğiniz e-posta adresi ile e-posta (Tekrar) adresi aynı değil" + "<br>";
                //else 
                if (!OrtakFonksiyonlar.MailAdresiDogrumu(form.ePostaAdresi.Trim()))
                    hata += "Girdiğiniz e-posta adresi hatalı" + "<br>";
            }
            if (!string.IsNullOrEmpty(form.parola.Trim()) && !string.IsNullOrEmpty(form.parolaTekrar.Trim()))
            {
                if (form.parola.Trim() != form.parolaTekrar.Trim())
                    hata += "Girdiğiniz parola bilgisi ile parola (Tekrar) bilgisi aynı değil" + "<br>";
            }

            if (string.IsNullOrEmpty(form.kod))
            {
                OnlineBasvuruKaydi o = new OnlineBasvuruKaydi();
                o.kimlikNo = form.kimlikNo;
                OnlineBasvuruKaydi[] olar = ListeleOnlineBasvuru(null, o);
                if (olar.Length > 0)
                {
                    hata = form.kimlikNo + " kimlik numarası ile daha önceden kayıt edilmiş üye bilgisi bulunmaktadır. Yeni üyelik kaydı yapılamaz. Kimlik No ve parolanız ile giriş yaptıktan sonra bilgilerinizi değiştirebilirsiniz.<br>";
                }
            }

            return hata;
        }

        public static Sonuc SilOnlineBasvuru(VeriErisim ve, OnlineBasvuruKaydi form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (form.kimlikNo.Trim() == "")
                hata += "Kimlik No boş bırakılamaz." + "<br>";
            if (form.turKod == 0)
                hata += "Tür Kod boş bırakılamaz." + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.SilOnlineBasvuru(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleOnlineBasvuruParola(VeriErisim ve, Kullanici kullanan, OnlineBasvuruKaydi form)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (form.kod.Trim() == "")
                hata += "Kod boş bırakılamaz." + "<br>";
            if (form.ePostaAdresi == "")//|| form.guvenlikSorusuCevap == "" || form.guvenlikSorusu == 0
                hata += "Tüm alanları doldurunuz.";
            if (!OrtakFonksiyonlar.MailAdresiDogrumu(form.ePostaAdresi))
                hata += "EPosta alanı hatalı.";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            sonuc = to.GuncelleOnlineBasvuruParola(form);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Güncelleme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Güncelleme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static OnlineBasvuruKaydi[] ListeleOnlineBasvuru(VeriErisim ve, OnlineBasvuruKaydi form)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, "OnlineBasvuru");
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            IDataReader r = to.ListeleOnlineBasvuru(form);

            ObjectArray bler = new ObjectArray();
            while (r.Read())
            {
                OnlineBasvuruKaydi b = new OnlineBasvuruKaydi();
                b.kod = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.KOD, "");
                b.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.KIMLIKNO, "");
                b.adi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.ADI, "");
                b.soyadi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.SOYADI, "");
                b.babaAdi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.BABAADI, "");
                b.vergiDairesi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.VERGIDAIRESI, "");
                b.ePostaAdresi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.EPOSTAADRESI, "");
                b.parola = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.PAROLA, "");
                b.guvenlikSorusu = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.GUVENLIKSORUSU, 0);
                b.guvenlikSorusuCevap = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.GUVENLIKCEVABI, "");
                b.ip = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.IP, "");
                b.tarih = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.TARIH);
                b.aktif = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.AKTIF, 0);
                b.uyeKimlikNo = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.UYEKIMLIKNO, "");
                b.uyeAdi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.UYEADI, "");
                b.uyeSoyadi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.UYESOYADI, "");
                b.uyeBabaAdi = VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.UYEBABAADI, "");
                b.dogumTarih.Yaz(VeriErisim.DegerAl(r, (int)TABHibeBasvuru.ENUMAlanlarOnline.DOGUMTARIH));

                bler.objeler.Add(b);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (OnlineBasvuruKaydi[])bler.objeler.ToArray(typeof(OnlineBasvuruKaydi));
        }

        public static Sonuc AktarSablon(Kullanici kullanan, int turKod, int eskiEtap, int yeniEtap)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (turKod == 0)
                hata += "Tür bilgisi boş bırakılamaz" + "<br>";

            if (eskiEtap == 0)
                hata += "Eski Etap No bilgisi boş bırakılamaz" + "<br>";

            if (yeniEtap == 0)
                hata += "Yeni Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuru to = new TABHibeBasvuru(ve);
            ve.BeginTransaction();

            BasvuruSablon bs = new BasvuruSablon();
            bs.etapNo = eskiEtap;
            bs.turKod = turKod;
            BasvuruSablon[] liste = ListeleSablon(ve, kullanan, bs);

            bs = new BasvuruSablon();
            bs.etapNo = yeniEtap;
            bs.turKod = turKod;
            sonuc = to.SilSablon(bs);

            foreach (BasvuruSablon bsYeni in liste)
            {
                bsYeni.kod = to.EnBuyukKodSablon();
                bsYeni.etapNo = yeniEtap;
                sonuc = to.KaydetSablon(bsYeni);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}