using System;
using System.Data;
using System.Collections;
using System.Text;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.MUH;
using TNS.Veri;
using System.Threading;


namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// AboneFatura formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCAboneFatura
    {
        /// <summary>
        /// Bütçe Muhasebe Servisi
        /// </summary>
        public static IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// AboneFatura nesnesini veri tabanına yazar ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek AboneFatura nesne koleksiyonu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, ""); //

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00002;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (liste.objeler.Count == 0)
                return new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00007, "");


            AboneFatura aboneFatura = (AboneFatura)liste.objeler[0];
            string hata = "";
            if (aboneFatura.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (string.IsNullOrWhiteSpace(aboneFatura.surecNo))
                hata += "Hatalı süreç numarası geldi!" + "<br>";
            if (aboneFatura.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (aboneFatura.birim == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAboneFatura to = new TABAboneFatura(ve);
            int aboneTip = aboneFatura.aboneTip;
            for (int jj = 0; jj < liste.objeler.Count; jj++)
            {
                AboneFatura aboneFaturaIc = (AboneFatura)liste.objeler[jj];
                if (aboneFaturaIc.aboneNo == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00006, jj + 1) + "<br>";
                if (aboneFaturaIc.faturaNo == "")
                    hata += string.Format("{0}. satırda fatura numarası girilmemiş", jj + 1) + "<br>";
                if (aboneFaturaIc.aboneTip != aboneTip)
                    hata += string.Format("{0}. satırda faturanın tipi tutarsız!", jj + 1) + "<br>";
                for (int kk = jj + 1; kk < liste.objeler.Count; kk++)
                {
                    AboneFatura aboneFaturaKontrol = (AboneFatura)liste.objeler[kk];
                    if (aboneFaturaKontrol.faturaNo == aboneFaturaIc.faturaNo)
                        hata += string.Format("{0}. satırdaki fatura ile {1}. satırdaki fatura no aynı olamaz!", jj + 1, kk + 1) + "<br>";
                }
                string eskiSurecNo = to.BaskaSurecteFaturaVarMi(aboneFaturaIc.faturaNo, aboneFatura.surecNo, aboneFatura.muhasebe);
                if (!string.IsNullOrWhiteSpace(eskiSurecNo))
                {
                    hata += string.Format("{0}. satırdaki {1} numaralı fatura, {2} numaralı süreçte de bulunuyor!", jj + 1, aboneFaturaIc.faturaNo, eskiSurecNo) + "<br>";
                }
            }

            if (hata != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            string ayKontroluYap = TNS.DEG.Arac.DegiskenDegerBul(0, "/ABONEFATURAAYKONTROLUYAPMA");
            ve.BeginTransaction();
            if (aboneTip != (int)ENUMAboneTip.ELEKTRIK && string.IsNullOrWhiteSpace(ayKontroluYap))
            {
                IDataReader r = to.OncedenKayitliMi(liste);
                ObjectArray oncedenKayitliOlanlar = new ObjectArray();
                string sonucBilgi = "";

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        AboneFatura afbilgi = new AboneFatura();

                        afbilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        afbilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        afbilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        afbilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                        afbilgi.faturaYil = VeriErisim.DegerAl(r, 4, 0);
                        afbilgi.faturaAy = VeriErisim.DegerAl(r, 5, 0);
                        afbilgi.aboneNo = VeriErisim.DegerAl(r, 6, "");

                        afbilgi.faturaTutar = VeriErisim.DegerAl(r, 8, 0.0);
                        afbilgi.faizMiktari = VeriErisim.DegerAl(r, 9, 0.0);

                        oncedenKayitliOlanlar.objeler.Add(afbilgi);
                    }
                }
                r.Close();

                if (oncedenKayitliOlanlar.ObjeSayisi > 0)
                {
                    foreach (AboneFatura fatura in oncedenKayitliOlanlar.objeler)
                    {
                        foreach (AboneFatura abone in liste.objeler)
                        {
                            if (fatura.aboneNo == abone.aboneNo)
                            {
                                if (fatura.faturaTutar > 0 && abone.faturaTutar > 0)
                                    sonucBilgi += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00008, fatura.aboneNo, fatura.faturaYil, fatura.faturaAy, fatura.surecNo) + "<br>";
                                if (fatura.faizMiktari > 0 && abone.faizMiktari > 0)
                                    sonucBilgi += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00009, fatura.aboneNo, fatura.faturaYil, fatura.faturaAy, fatura.surecNo) + "<br>";
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(sonucBilgi))
                    {
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = sonucBilgi;
                        return sonuc;
                    }
                }
            }

            sonuc = to.Sil((AboneFatura)liste.objeler[0]);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// AboneFatura nesnesini veri tabanından okur ve AboneFatura nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren AboneFatura nesnesi</param>
        /// <param name="kontrolTarih">Verilen tarihte oluşturulmuş olan sürece ait faturaları listelemek için kullanılır(özellikle veri aktarımı yapılan elektrik faturalarını listelemek için)</param>
        /// <returns>İstenilen kriterdeki fatura listesini AboneFatura nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, AboneFatura kriter, TNSDateTime kontrolTarih, bool esitle)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            IDataReader r = to.Listele(kriter, esitle);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AboneFatura afbilgi = new AboneFatura();

                    afbilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    afbilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    afbilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    afbilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    afbilgi.faturaYil = VeriErisim.DegerAl(r, 4, 0);
                    afbilgi.faturaAy = VeriErisim.DegerAl(r, 5, 0);
                    afbilgi.aboneNo = VeriErisim.DegerAl(r, 6, "");
                    afbilgi.aboneTip = VeriErisim.DegerAl(r, 7, 0);
                    afbilgi.faturaTutar = VeriErisim.DegerAl(r, 8, 0.0);
                    afbilgi.faizMiktari = VeriErisim.DegerAl(r, 9, 0.0);
                    afbilgi.ozelKullanim = VeriErisim.DegerAl(r, 10, 0.0);
                    afbilgi.odemeTarihi = VeriErisim.DegerAl(r, 11);
                    afbilgi.odemeEmriNo = VeriErisim.DegerAl(r, 12, "");
                    afbilgi.faturaNo = VeriErisim.DegerAl(r, 13, "");
                    afbilgi.isletmeKodu = VeriErisim.DegerAl(r, 14, "");
                    afbilgi.odemeEmriYil = VeriErisim.DegerAl(r, 15, 0);
                    afbilgi.damgaVergisi = VeriErisim.DegerAl(r, 16, 0.0);
                    TNSDateTime tarih = VeriErisim.DegerAl(r, 17);
                    afbilgi.yevmiyeTarih = VeriErisim.DegerAl(r, 18);
                    afbilgi.yevmiyeNo = VeriErisim.DegerAl(r, 19, 0);
                    afbilgi.mifDurum = VeriErisim.DegerAl(r, 20, 0);

                    if (!kontrolTarih.isNull)
                    {
                        if (!tarih.isNull && tarih.ToString() == kontrolTarih.ToString())
                            bilgi.objeler.Add(afbilgi);
                    }
                    else
                    {
                        if (!tarih.isNull)
                            bilgi.objeler.Add(afbilgi);
                    }
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere göre Ödeme Emri Belge Numarasını okur ve AboneFatura nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">Ödeme Emri Belge Numarasına ait Süreç Yıl bilgisi</param>
        /// <param name="surecNo">Ödeme Emri Belge Numarasına ait Süreç No bilgisi</param>
        /// <returns>İstenilen kriterdeki fatura listesini AboneFatura nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);
            TABAboneFatura toAboneFatura = new TABAboneFatura(ve);

            IDataReader dr = toAboneFatura.ListeleOdemeEmri(yil, surecNo);

            while (dr.Read())
            {
                AboneFatura sd = new AboneFatura();
                sd.yil = VeriErisim.DegerAl(dr, 0, 0);
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 1, "");
                sd.odemeEmriYil = VeriErisim.DegerAl(dr, 2, 0);

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00012;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Verilen AboneFatura nesnesi için Ödeme Emri Belge Numarasını kaydeder ve Sonuc nesnesi döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Ödeme Emri Belge Numarasının kaydedileceği AboneFatura nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GsmBilgiKaydet(Kullanici kullanan, AboneFatura kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            ve.BeginTransaction();

            sonuc = to.GsmBilgiKaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen AboneFatura nesnesi için Ödeme Emri Belge Numarasını kaydeder ve Sonuc nesnesi döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Ödeme Emri Belge Numarasının kaydedileceği AboneFatura nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, AboneFatura kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            ve.BeginTransaction();

            sonuc = to.OdemeEmriNoKaydet(kriter);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Ödenek Gönderme Belgesi gönderilecek AboneOdenekGondermeMail nesnesini veritabanından okur ve 
        /// AboneOdenekGondermeMail nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="mail">Listelemede kriter olarak kullanılacak değerleri içeren AboneOdenekGondermeMail nesnesi</param>
        /// <returns>İstenilen kriterdeki fatura listesini AboneOdenekGondermeMail nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OdenekGondermeMailListele(AboneOdenekGondermeMail mail)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABAboneFatura to = new TABAboneFatura(ve);
            IDataReader r = to.OdenekGondermeMailListele(mail);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AboneOdenekGondermeMail abilgi = new AboneOdenekGondermeMail();

                    abilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    abilgi.ePosta = VeriErisim.DegerAl(r, 1, "");
                    abilgi.adSoyad = VeriErisim.DegerAl(r, 2, "");
                    abilgi.kurKod = VeriErisim.DegerAl(r, 3, "");

                    bilgi.objeler.Add(abilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Ödenek Gönderme Mail bilgisini veritabanına yazar ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mail">Kaydedilecek AboneOdenekGondermeMail nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdenekGondermeMailKaydet(Kullanici kullanan, AboneOdenekGondermeMail mail)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = "";
            if (string.IsNullOrEmpty(mail.ePosta))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00014 + "<br>";
            else if (!OrtakFonksiyonlar.MailAdresiDogrumu(mail.ePosta))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00015;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            ve.BeginTransaction();

            if (mail.kod > 0)
                sonuc = to.OdenekGondermeMailGuncelle(mail);
            else
            {
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "HRCFATURAODENEKGONDERMEMAIL";
                mail.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                if (mail.kod < 1)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00016 + "<br>";
                }
                else
                {
                    sonuc = to.OdenekGondermeMailKaydet(mail);
                    sonuc.anahtar = mail.kod.ToString();
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010 + "<br>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Ödenek Gönderme Mail bilgisini veritabanından siler ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mail">Silinecek AboneOdenekGondermeMail nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdenekGondermeMailSil(Kullanici kullanan, AboneOdenekGondermeMail mail)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (mail.kod == 0)
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00017 + "<br>");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            ve.BeginTransaction();

            sonuc = to.OdenekGondermeMailSil(mail);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018 + "<br>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Ödenek Gönderme Belgelerini verilen kriterlere göre listeler ve bağlantılı adreslere mail olarak gönderir ve
        /// işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl</param>
        /// <param name="ay">İşlem yapılacak ay</param>
        /// <param name="aboneTip">İşlem yapılacak abone tipi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdenekGondermeMailGonder(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            Sonuc donusDeger = new Sonuc();
            string odenekAciklama = "";
            string konu = "";
            string ekoKod2 = "";
            string kimden = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPKullanici").Trim();

            if (aboneTip == (int)ENUMAboneTip.TELEFON)
            {
                ekoKod2 = "03.5";
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00020, ay.ToString("00"), yil.ToString());//Değiştirmeyin
                konu = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00021, ay.ToString("00"), yil.ToString());
            }
            else if (aboneTip == (int)ENUMAboneTip.ELEKTRIK)
            {
                ekoKod2 = "03.2";
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00022, ay.ToString("00"), yil.ToString());//Değiştirmeyin
                konu = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00023, ay.ToString("00"), yil.ToString());
            }
            else if (aboneTip == (int)ENUMAboneTip.SU)
            {
                ekoKod2 = "03.2";
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00024, ay.ToString("00"), yil.ToString());//Değiştirmeyin
                konu = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00025, ay.ToString("00"), yil.ToString());
            }

            ObjectArray mailListe = OdenekGondermeMailListele(new AboneOdenekGondermeMail());
            ObjectArray fler = new ObjectArray();
            string eskiKurKod = "";
            foreach (AboneOdenekGondermeMail m in mailListe.objeler)
            {
                if (eskiKurKod != m.kurKod)
                {
                    TNS.MUH.GondermeTenkisForm form = new TNS.MUH.GondermeTenkisForm();
                    form.yil = yil;
                    form.aciklama = odenekAciklama;
                    form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YENI;
                    form.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
                    TNS.MUH.GondermeTenkisDetay detay = new TNS.MUH.GondermeTenkisDetay();
                    detay.tertip.kodlar[0] = m.kurKod;
                    detay.tertip.kodlar[1] = "";
                    detay.tertip.kodlar[2] = "";
                    detay.tertip.kodlar[3] = ekoKod2;
                    form.detay.objeler.Add(detay);
                    fler = servisMUH.GondermeTenkisListele(kullanan, form, 0);

                    if (fler.ObjeSayisi == 0)
                        continue;
                }
                eskiKurKod = m.kurKod;

                StringBuilder liste = new StringBuilder();
                liste.Append("<table>");
                liste.Append("<tr>");
                liste.Append("<td><b>" + TNS.HRC.HarcamaSurecleriResI.HRC00026 + "</b></td>");
                liste.Append("<td><b>" + TNS.HRC.HarcamaSurecleriResI.HRC00027 + "</b></td>");
                liste.Append("<td><b>" + TNS.HRC.HarcamaSurecleriResI.HRC00028 + "</b></td>");
                liste.Append("<td><b>" + TNS.HRC.HarcamaSurecleriResI.HRC00029 + "</b></td>");
                liste.Append("</tr>");

                foreach (GondermeTenkisForm g in fler.objeler)
                {
                    liste.Append("<tr>");
                    liste.Append("<td>");
                    liste.Append(g.belgeNo);
                    liste.Append("</td>");
                    liste.Append("<td>");
                    liste.Append(g.muhasebe + "-" + g.muhasebeAd);
                    liste.Append("</td>");
                    liste.Append("<td>");
                    liste.Append(g.birim + "-" + g.birimAd);
                    liste.Append("</td>");
                    liste.Append("<td>");
                    liste.Append(g.belgeToplami);
                    liste.Append("</td>");
                    liste.Append("</tr>");
                }
                liste.Append("</table>");

                string bilgi = "";

                if (aboneTip == (int)ENUMAboneTip.TELEFON)
                    bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00030, ay.ToString("00"), yil.ToString(), liste.ToString());
                else if (aboneTip == (int)ENUMAboneTip.ELEKTRIK)
                    bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00031, ay.ToString("00"), yil.ToString(), liste.ToString());
                else if (aboneTip == (int)ENUMAboneTip.SU)
                    bilgi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00032, ay.ToString("00"), yil.ToString(), liste.ToString());

                if (m.ePosta != "" && m.kurKod != "")
                {
                    string mail = m.ePosta + ";sgbnetBilgi@sgb.gov.tr";

                    donusDeger.bilgiStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00033, m.ePosta);
                    OrtakFonksiyonlar.MailAt(kimden, mail, konu, bilgi, true, false, new TNSCollection());
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere bağlı olarak elde edilen abone fatura bilgilerine ait toplam bilgilerini okur ve
        /// AboneFaturaIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren AboneFaturaIstatistikSinif nesnesi</param>
        /// <returns></returns>
        public ObjectArray ListeleAboneFaturaIstatistik(Kullanici kullanan, AboneFaturaIstatistikSinif kriter)
        {
            ObjectArray donusList = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAboneFatura to = new TABAboneFatura(ve);
            IDataReader r = to.ListeleIstatistik(kriter);

            AboneFaturaIstatistikSinif t = null;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    t = new AboneFaturaIstatistikSinif();

                    t.aboneTip = VeriErisim.DegerAl(r, 0, 0);
                    t.toplamFaturaTutar = (decimal)VeriErisim.DegerAl(r, 1, 0.0);
                    t.faturaAySayisi = VeriErisim.DegerAl(r, 2, 0);
                    t.ortalama = (decimal)VeriErisim.DegerAl(r, 3, 0.0);

                    donusList.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donusList;
        }
    }
}
