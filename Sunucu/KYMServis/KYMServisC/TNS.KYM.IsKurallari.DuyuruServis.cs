using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;
using System.Threading;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Kullanıcılara sisteme girdiklerinde, giriş bilgi ekranında gösterilen duyurularla ilgili ve kullanıcılara toplu elektronik posta atılmasıyla ilgili yordamları gerçekleştiren sınıf.
    /// </summary>
    public class DuyuruServis
    {
        /// <summary>
        /// Parametre olarak verilen datareader'dan aldığı değerlerle duyuru nesnesi oluşturan yordam.
        /// </summary>
        /// <param name="r">Verilerin alınacağı datareader nesnesi. Datareader'ın aktif satırı kullanılır.</param>
        /// <returns>Datareader'dan alınan değerlerle oluşturulan duyuru nesnesi döndürülür.</returns>
        private Duyuru DataReaderdanAktar(IDataReader r)
        {
            Duyuru duy = new Duyuru();
            duy.kod = VeriErisim.DegerAl(r, 0, 0);
            duy.onem = VeriErisim.DegerAl(r, 1, 0);
            duy.tarih = VeriErisim.DegerAl(r, 2);
            duy.duyuru = VeriErisim.DegerAl(r, 3, "");
            duy.gizle = (ENUMGizle)VeriErisim.DegerAl(r, 4, 0);
            duy.KullaniciTipleriOl(VeriErisim.DegerAl(r, 5, ""));
            duy.pencerede = (ENUMPencerede)VeriErisim.DegerAl(r, 6, 0);
            duy.konu = VeriErisim.DegerAl(r, 7, "");

            if (duy.duyuru.StartsWith(Duyuru.penceredeGoster))
            {
                duy.duyuru = duy.duyuru.Replace(Duyuru.penceredeGoster, "");//Pencerede gösterme kaydı;
                duy.pencerede = ENUMPencerede.GORUNUR;
            }

            return duy;
        }

        /// <summary>
        /// Kullanıcı giriş yaptığında, giriş sayfasında gösterilecek olan dururuların listesini verilen kıstaslara göre döndüren yordam.
        ///   Duyuru tablosundan record set olarak listeyi alır, 
        ///   dönen kayıt kümesini veri aktarım nesnesine dönüştürüp, döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="aramaBilgisi">Döndürülecek duyuruların uyacağı kriter bilgisini tutan duyuru nesnesi.</param>
        /// <returns>Verilen kriterlere uyan duyurular döndürülür.</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Duyuru aramaBilgisi, bool modulDuyurulari)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            ObjectArray dyv = new ObjectArray();
            TABDuyuru tab = new TABDuyuru(ve);
            IDataReader r = tab.Ara(aramaBilgisi);
            if (r != null)
            {
                while (r.Read())
                {
                    dyv.Ekle(DataReaderdanAktar(r));
                }
                dyv.sonuc.islemSonuc = true;
                r.Close();
                if (modulDuyurulari)
                    DigerModulDuyurulariEkle(kullanan, aramaBilgisi, dyv);

                if (TNS.KYM.Arac.ModulKullaniliyorMu("GIT"))
                {
                    GorevYonetimiGorevleriListele(kullanan, aramaBilgisi, dyv);
                }
                dyv.sonuc.kayitSay = dyv.ObjeSayisi;
            }
            else
            {
                dyv.sonuc.DegerAta(tab.Hata());
            }
            ve.VeriErisimKapat();
            return dyv;
        }

        private void GorevYonetimiGorevleriListele(Kullanici kullanan, Duyuru aramaBilgisi, ObjectArray dyv)
        {
            ObjectArray oa = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABDuyuru tab = new TABDuyuru(ve);
            IDataReader dr = tab.GorevleriOku(kullanan.mernis);

            if (dr != null)
            {
                while (dr.Read())
                {
                    int durum = VeriErisim.DegerAl(dr, 0, 0);
                    string konu = VeriErisim.DegerAl(dr, 1, "");
                    string atayanKod = VeriErisim.DegerAl(dr, 2, "");
                    string atananKod = VeriErisim.DegerAl(dr, 3, "");
                    TNSDateTime atamaTarihi = VeriErisim.DegerAl(dr, 4);
                    string atananAd = VeriErisim.DegerAl(dr, 5, "");
                    string atayanAd = VeriErisim.DegerAl(dr, 6, "");
                    int okunma = VeriErisim.DegerAl(dr, 7, 0);

                    if (string.IsNullOrEmpty(atayanAd)) continue;

                    string dmetni = atayanAd + " tarafından <b>" + konu + "</b> konusunda görevlendirildiniz.";
                    if (atamaTarihi.isNull) atamaTarihi = new TNSDateTime(DateTime.Now);
                    if (okunma == 0) dmetni = "<img src=\"../app_themes/images/yeniBilgiVar.gif\"/>&nbsp;" + dmetni;


                    Duyuru duy = new Duyuru();
                    duy.kod = -1;
                    duy.onem = 1;
                    duy.tarih = atamaTarihi;
                    duy.gizle = ENUMGizle.GORUNUR;
                    duy.pencerede = ENUMPencerede.GORUNUR;
                    duy.duyuru = dmetni;
                    oa.Ekle(duy);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            if (oa.objeler.Count > 0)
                dyv.objeler.AddRange(oa.objeler);
        }

        public class DigerModulDuyuru
        {
            public string modulKod;
            public string dllAd;
            public string nesneAd;

            public DigerModulDuyuru(string modulKod, string pDLLAd, string pNesneAd)
            {
                this.modulKod = modulKod;
                this.dllAd = pDLLAd;
                this.nesneAd = pNesneAd;
            }
        }

        public static DigerModulDuyuru[] digerModulDuyurulari = { 
            new DigerModulDuyuru("ATM", "ATMServisC", "TNS.ATM.IsKurallari.ATMServis"),
            new DigerModulDuyuru("IKY", "IKYServisC", "TNS.IKY.IsKurallari.IKYServis")
        };
        private void DigerModulDuyurulariEkle(Kullanici kullanan, Duyuru aramaBilgisi, ObjectArray dyv)
        {
            foreach (DigerModulDuyuru item in digerModulDuyurulari)
            {
                if (TNS.KYM.Arac.ModulKullaniliyorMu(item.modulKod))
                {
                    try
                    {
                        System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(item.dllAd, item.nesneAd);
                        IDuyuruListesi rr = (IDuyuruListesi)oh.Unwrap();

                        dyv.objeler.AddRange(rr.DuyuruListesiAl(kullanan, aramaBilgisi.KullaniciTipleriStr).objeler);
                    }
                    catch (Exception ex)
                    {
                        OrtakFonksiyonlar.HataDosyaYaz("DuyuruCagir.txt", ex.Message, false);
                    }
                }
            }
        }

        /// <summary>
        /// Kodu verilen duyurunun bilgilerini döndüren yordam.
        /// Duyuru tablosundan dönen tek kayıt duyuru nesnesine döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="duyuruKodu">Döndürülecek duyurunun kodu.</param>
        /// <returns>Kodu verilen duyuru döndürülür.</returns>
        public Duyuru Oku(VeriErisim veriErisim, int duyuruKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABDuyuru tab = new TABDuyuru(ve);
            IDataReader r = tab.Oku(duyuruKodu);
            Duyuru duyuru = new Duyuru();
            string hataStr = string.Empty;
            if (tab.Hata() == "")
            {
                if (r.Read())
                    duyuru = DataReaderdanAktar(r);
                else
                    hataStr = KullaniciResI.KYMDUY001;

                r.Close();
            }
            else
                hataStr = tab.Hata();

            ve.VeriErisimKapat();

            if (hataStr != "")
                throw new Exception(hataStr);

            return duyuru;
        }

        private string KontrolEtDuyuru(Duyuru duyuru)
        {
            string hataStr = "";
            if (string.IsNullOrEmpty(duyuru.duyuru))
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL035;
            if (duyuru.tarih.isNull)
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL036;
            if (hataStr != "") hataStr = hataStr.Substring(4);
            return hataStr;
        }

        /// <summary>
        /// Bilgileri verilen duyuru nesnesini veri tabanına kaydeden yordam.
        /// Duyuru zaten var ise güncelleme işlemi yapılır. 
        /// Duyuru yeni ise, yeni kayıt için duyuru kodu üretilir ve sonra ekleme işlemi çalıştırılır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="duyuru">Duyuru bilgilerini tutan ve kaydedilecek nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Duyuru duyuru)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            Sonuc sonuc = new Sonuc(true, "", "");
            sonuc.DegerAta(KontrolEtDuyuru(duyuru));
            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc ||
                TNS.KYM.Yetki.DestekYoneticisiMi(kullanan).islemSonuc)
            {
                TABDuyuru tab = new TABDuyuru(ve);

                if (duyuru.kod == 0)
                {
                    duyuru.kod = tab.DuyuruKoduUret();
                    if (tab.Hata() == "")
                        sonuc = tab.Ekle(duyuru);
                    else
                        sonuc.hataStr = KullaniciResI.KYMDUY003 + " (" + tab.Hata() + ")";
                }
                else
                    sonuc = tab.Guncelle(duyuru);

                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = Convert.ToString(duyuru.kod);
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kodu verilen duyuru bilgilerini veri tabanından silen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="duyuruKodu">Silinecek duyurunun kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int duyuruKodu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            Sonuc sonuc = new Sonuc(false, KullaniciResI.KYMDUY002, "");

            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc)
            {
                TABDuyuru tab = new TABDuyuru(ve);
                sonuc = tab.Sil(duyuruKodu);
                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = Convert.ToString(duyuruKodu);
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Bir kullanıcı tipindeki kullanıcılara, email atmak için kullanılan yordam.
        /// Bu yordamı sadece sistem yöneticileri kullanabilir. Gönderilecek mesaj ve mesajın gönderileceği kişiler veri tabanına yazılır.
        /// Gönderme işlemi için bir thread başlatılır. Bu thread Her kullanıcıya emaili göndermeye çalışır. 
        /// Gönderme işlemi sonucu email için adı kaydedilmiş olan kullanıcının karşısına yazılır.
        /// Bu sayede mesajın gönderilemediği kullanıcılara (thread'in kesilmesi, eposta sunucusunun kabul etmemesi gibi sebeplerle) tekrar mesaj gönderilebilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="konu">Gönderilecek elektronik postanın konusu.</param>
        /// <param name="mesaj">Gönderilecek elektronik postanın metni.</param>
        /// <param name="ktKod">Elektronik postanın hangi kullanıcı tipindeki kullanıcılar gönderileceği</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc EPostaKaydet(Kullanici kullanan, string konu, string mesaj, int ktKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", kullanan.KimlikBul());
            Sonuc sonuc = new Sonuc(false, KullaniciResI.KYMDUY002, "");
            int kod = 0;

            if (!(TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc ||
                Yetki.DestekYoneticisiMi(kullanan).islemSonuc))
            {
                sonuc.DegerAta(KullaniciResI.KYMDUY002);
                return sonuc;
            }

            TABDuyuru tab = new TABDuyuru(ve);
            ve.BeginTransaction();

            if (kod == 0)
            {
                kod = tab.EPostaKoduUret();
                if (tab.Hata() == "")
                {
                    sonuc = tab.EPostaKaydet(kod, kullanan.emailAdresi, konu, mesaj, ktKod);
                    if (sonuc.islemSonuc)
                    {
                        Kullanici kul = new Kullanici();
                        kul.kullaniciTipi = new ENUMKullaniciTipi[1];
                        kul.aktifMi = ENUMAktiflik.AKTIF;

                        if (ktKod != 0)
                            kul.kullaniciTipi[0] = (ENUMKullaniciTipi)ktKod;
                        else
                            kul.kullaniciTipi[0] = ENUMKullaniciTipi.TANIMSIZ;

                        ObjectArray klv = new KYMServis().KullaniciAra(kul, 0, 0);
                        //TABDuyuru tab = new TABDuyuru(ve);
                        if (klv.sonuc.islemSonuc)
                        {
                            //Mail atılacak kullanıcıları kaydet
                            foreach (Kullanici k in klv.objeler)
                            {
                                sonuc = tab.EPostaGonderKaydet(kod, k.kullaniciKodu, new TNSDateTime(System.DateTime.Now), k.emailAdresi, KullaniciResI.KYMDUY009, 0);
                                if (!sonuc.islemSonuc) break;
                            }
                        }
                    }
                }
                else
                    sonuc.hataStr = KullaniciResI.KYMDUY004 + " (" + tab.Hata() + ")";
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = KullaniciResI.KYMDUY005;
                sonuc.anahtar = Convert.ToString(kod);
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = KullaniciResI.KYMDUY006 + " (" + sonuc.hataStr + ")";
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            EPostaGonderTekrar(kod);

            return sonuc;
        }

        /// <summary>
        /// Bir elektronik posta için, postanın gönderilmediği kullanıcıların listesini alan ve o kullanıcılara mail atan yordam.
        /// Bu yordam bir thread içinde çalışır.
        /// </summary>
        /// <param name="nesneMesajID">Kullanıcılara gönderilecek elektronik postanın numarası.</param>
        public void EPostaGonderKullanici(Object nesneMesajID)
        {
            ObjectArray postalar = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABDuyuru to = new TABDuyuru(ve);
            int mesajID = OrtakFonksiyonlar.ConvertToInt(nesneMesajID, 0);

            IDataReader r = to.EPostaGonderListele(mesajID, 0);

            string mailDurum = string.Empty;
            string mailHata = string.Empty;

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Posta p = new Posta();
                    p.mesajID = VeriErisim.DegerAl(r, 0, 0);
                    p.konu = VeriErisim.DegerAl(r, 1, "");
                    p.mesaj = VeriErisim.DegerAl(r, 2, "");
                    p.ktKod = (ENUMKullaniciTipi)VeriErisim.DegerAl(r, 3, 0);
                    p.gonderen = VeriErisim.DegerAl(r, 4, "");
                    p.kullaniciKodu = VeriErisim.DegerAl(r, 5, "");
                    p.email = VeriErisim.DegerAl(r, 6, "");

                    postalar.objeler.Add(p);
                }
                r.Close();
            }

            int sayac = 1;

            foreach (Posta pp in postalar.objeler)
            {
                sayac++;

                try
                {
                    mailDurum = OrtakFonksiyonlar.MailAt(pp.gonderen, pp.email, pp.konu, pp.mesaj, true, false, null);
                }
                catch { mailDurum = KullaniciResI.KYMDUY007; }

                if (OrtakFonksiyonlar.ConvertToInt(mailDurum, 0) == 1)
                    mailHata = KullaniciResI.KYMDUY008;
                else
                {
                    mailHata = mailDurum;
                    mailDurum = "0";
                }
                Sonuc ss = to.EPostaGonderKaydet(pp.mesajID, pp.kullaniciKodu, new TNSDateTime(System.DateTime.Now), pp.email, mailHata, OrtakFonksiyonlar.ConvertToInt(mailDurum, 0));
                if (!ss.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ss.hataStr);
                }
            }

            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Sisteme kaydedilmiş mesajları ve bu mesajların kaç kişiye gönderileceğini ve şu ana kadar kaç kişiye gönderildiğini listeleyen yordam.
        /// </summary>
        /// <returns>Sisteme kaydedilmiş mesajlar ve bu mesajların kaç kişiye gönderileceği ve şu ana kadar kaç kişiye gönderildiği bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        public ObjectArray EPostaGonderOncekiGonderiler()
        {
            ObjectArray postalar = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABDuyuru to = new TABDuyuru(ve);

            IDataReader r = to.EPostaGonderOncekiGonderiler();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Posta p = new Posta();
                    p.mesajID = VeriErisim.DegerAl(r, 0, 0);
                    p.konu = VeriErisim.DegerAl(r, 1, "");
                    p.gonderen = VeriErisim.DegerAl(r, 2, "");
                    p.tarih = VeriErisim.DegerAl(r, 3);
                    p.toplamGonderilen = VeriErisim.DegerAl(r, 4, 0);
                    p.toplamBasarili = VeriErisim.DegerAl(r, 5, 0);
                    postalar.objeler.Add(p);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return postalar;
        }

        /// <summary>
        /// "gonderildiMi" parametresinin değerine göre bir mesajla ilgili tüm kullanıcıları, gönderilenleri veya gönderilmeyenleri listeleyen yordam. 
        /// </summary>
        /// <param name="mesajId">Kullanıcıları döndürülecek elektronik postanın numarası.</param>
        /// <param name="gonderildiMi">-1 ise tüm kullanıcılar, 1 ise gönderilenler, 0 ise gönderilmeyenler listenir.</param>
        /// <returns>Bir mesajın gönderildiği/gönderilmediği/ilişkili tüm kullanıcıların bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        public ObjectArray EPostaGonderilenlerListele(int mesajId, int gonderildiMi)
        {
            ObjectArray postalar = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABDuyuru to = new TABDuyuru(ve);

            IDataReader r = to.EPostaGonderilenlerListele(mesajId, gonderildiMi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Posta p = new Posta();
                    p.mesajID = VeriErisim.DegerAl(r, 0, 0);
                    p.kullaniciKodu = VeriErisim.DegerAl(r, 1, "");
                    p.tarih = VeriErisim.DegerAl(r, 2);
                    p.email = VeriErisim.DegerAl(r, 3, "");
                    p.hataMesaji = VeriErisim.DegerAl(r, 4, "");
                    p.gonderildiMi = VeriErisim.DegerAl(r, 5, 0);
                    postalar.objeler.Add(p);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return postalar;
        }

        /// <summary>
        /// Bir elektronik postayı kullanıcılara göndermek için başlatılmış thread, tüm kullanıcılara mail atamadan sonlanmışsa, 
        /// verilen elektronik posta numarası için thread'i tekrar başlatan yordam. Daha önceki çalışmada posta atılamayan kullanıcılara posta atılır.
        /// </summary>
        /// <param name="mesajId">Posta atma işlemi tekrar başlatılacak yordam.</param>
        public void EPostaGonderTekrar(int mesajId)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(EPostaGonderKullanici);
            Thread postaThread = new Thread(pts);
            postaThread.Start(mesajId);
        }

        /// <summary>
        /// Istemci tarafından gönderilmek istenilen bir e-posta gönderilmesini sağlar. Istemci web.config dosyasında SMTP ayarları olmadığı için bu metod kullanılabilir.
        /// </summary>
        /// <param name="kimden">The kimden.</param>
        /// <param name="kime">The kime.</param>
        /// <param name="konu">The konu.</param>
        /// <param name="icerik">The icerik.</param>
        /// <param name="html">if set to <c>true</c> [HTML].</param>
        /// <param name="reply">if set to <c>true</c> [reply].</param>
        /// <param name="ekDosyaIsimleri">The ek dosya isimleri.</param>
        public static string EPostaGonder(string kimden, string kime, string konu, string icerik, bool html, bool reply, TNS.TNSCollection ekDosyaIsimleri)
        {
            return TNS.OrtakFonksiyonlar.MailAt(kimden, kime, konu, icerik, html, reply, ekDosyaIsimleri);
        }
    }
}
