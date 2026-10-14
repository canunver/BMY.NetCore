using KYMServisI.IcisleriKullaniciServis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TNS.KPM;
using TNS.KYM.IsKurallari.Veri;
using TNS.MUH;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Sistem kullanıcılarıyla ilgili sunucu yordamlarını tutan sınıf.
    /// </summary>
    public class KullaniciServis
    {
        private Sonuc KontrolEt(Kullanici kullanici)
        {
            string hataStr = "";
            string webServisKontrol = System.Configuration.ConfigurationManager.AppSettings.Get("KullaniciWebServis") + "";

            if (webServisKontrol != "")
            {
                if (kullanici.mernis == "")
                    kullanici.mernis = kullanici.kullaniciKodu;
            }

            if (kullanici.kullaniciKodu == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL037;
            if (kullanici.mernis == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL038;
            if (kullanici.ad == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL039;
            if (kullanici.soyad == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL040;
            if (kullanici.calistigiKurum == "" && webServisKontrol == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL041;
            if (kullanici.unvan == "" && webServisKontrol == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL042;
            if (kullanici.telefon == "" && webServisKontrol == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL043;

            if (kullanici.emailAdresi == "" && webServisKontrol == "")
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL044;
            else if ((!OrtakFonksiyonlar.MailAdresiDogrumu(kullanici.emailAdresi, true)) && webServisKontrol == "")
            {
                if (string.IsNullOrWhiteSpace(kullanici.emailAdresi))
                    hataStr = hataStr + "<br>" + string.Format(KullaniciResI.KYMKUL045, kullanici.emailAdresi);
                if (string.IsNullOrWhiteSpace(OrtakFonksiyonlar.gecerliDomainlerStr))
                    hataStr = hataStr + "<br>" + string.Format("Girilen mail adresi ({0}) geçerli değil!", kullanici.emailAdresi);
                else
                    hataStr = hataStr + "<br>" + string.Format("Girilen mail adresi ({0}) geçerli değil veya izin verilen adreslerden ({1}) değil!", kullanici.emailAdresi, OrtakFonksiyonlar.gecerliDomainlerStr);
            }

            if (kullanici.kullaniciTipi == null)
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL046;
            else if (kullanici.kullaniciTipi == null || kullanici.kullaniciTipi.Length == 0)
                hataStr = hataStr + "<br>" + KullaniciResI.KYMKUL047;

            if (hataStr != "") hataStr = hataStr.Substring(4);

            return new Sonuc(hataStr == "", hataStr, "");
        }

        /// <summary>
        /// Datareaderdaki aktif satırdaki bilgileri, verilen kullanıcı nesnesinin alanlarına kopyalayan yordam.
        /// </summary>
        /// <param name="r">Verilerin okunacağı data reader nesnesi.</param>
        /// <param name="k">Verilerin kopyalanacağı kullanıcı nesnesi.</param>
        private void DatareaderdenKullaniciya(IDataReader r, Kullanici k)
        {
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.KULLANICIKODU))
                k.kullaniciKodu = r.GetString((int)TABKullanici.ENUMAlanlar.KULLANICIKODU);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.MERNIS))
                k.mernis = r.GetString((int)TABKullanici.ENUMAlanlar.MERNIS);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.AD))
                k.ad = r.GetString((int)TABKullanici.ENUMAlanlar.AD);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.SOYAD))
                k.soyad = r.GetString((int)TABKullanici.ENUMAlanlar.SOYAD);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.PAROLA))
                k.parola = r.GetString((int)TABKullanici.ENUMAlanlar.PAROLA);

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.CALISTIGIKURUM))
            {
                k.calistigiKurum = r.GetString((int)TABKullanici.ENUMAlanlar.CALISTIGIKURUM);
            }
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.DOGUMTARIHI))
                k.dogumTarihi = new TNSDateTime(r.GetDateTime((int)TABKullanici.ENUMAlanlar.DOGUMTARIHI));
            else
                k.dogumTarihi = new TNSDateTime();

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.EMAIL))
                k.emailAdresi = r.GetString((int)TABKullanici.ENUMAlanlar.EMAIL);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.TELEFON))
                k.telefon = r.GetString((int)TABKullanici.ENUMAlanlar.TELEFON);

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.SONGIRISTARIHI))
                k.sonGirisTarihi = new TNSDateTime(r.GetDateTime((int)TABKullanici.ENUMAlanlar.SONGIRISTARIHI));
            else
                k.sonGirisTarihi = new TNSDateTime();

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.AKTIFMI))
                k.aktifMi = (ENUMAktiflik)VeriErisim.DegerAl(r, (int)TABKullanici.ENUMAlanlar.AKTIFMI, 0);

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.CALISTIGIBIRIM))
                k.calistigiBirim = r.GetString((int)TABKullanici.ENUMAlanlar.CALISTIGIBIRIM);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.SICILNO))
                k.sicilNo = r.GetString((int)TABKullanici.ENUMAlanlar.SICILNO);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.UNVAN))
                k.unvan = r.GetString((int)TABKullanici.ENUMAlanlar.UNVAN);

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.PAROLABITISTARIHI))
                k.parolaBitisTarihi = new TNSDateTime(r.GetDateTime((int)TABKullanici.ENUMAlanlar.PAROLABITISTARIHI));
            else
                k.parolaBitisTarihi = new TNSDateTime();

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.ARAMADAGOSTERME))
                k.aramadaGosterme = (int)VeriErisim.DegerAl(r, (int)TABKullanici.ENUMAlanlar.ARAMADAGOSTERME, 0);

            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.AKADEMIKUNVAN))
                k.akademikUnvan = r.GetString((int)TABKullanici.ENUMAlanlar.AKADEMIKUNVAN);
            if (!r.IsDBNull((int)TABKullanici.ENUMAlanlar.RUMUZ))
                k.rumuz = r.GetString((int)TABKullanici.ENUMAlanlar.RUMUZ);

        }

        /// <summary>
        /// Veri tabanından çekilen ve data readerda bulunan kayıtları, kullanıcı nesnesine çevirip, verilen listeye dolduran yordam.
        /// </summary>
        /// <param name="klv">Veri tabanından okunan ve kullanıcı nesnesine çevrilen kayıtların doldurulacağı liste.</param>
        /// <param name="r">Veri tabanından çekilen kayıtların durduğu data reader nesnesi.</param>
        /// <param name="basla">Hangi kayıttan itibaren kopyalamanın başlayacağı.</param>
        /// <param name="say">Kaç adet kaydın kopyalanacağı. 0 ise başlangıçtan itibaren tüm kayıtlar kopyalanır.</param>
        /// <returns>Veri tabanından gelen toplam kayıt sayısı döndürülür.</returns>
        private int DatareaderdenListeAl(ObjectArray klv, IDataReader r, int basla, int say)
        {
            bool okundu, devam;
            int eklenen, kayitSay;
            kayitSay = 0;
            while ((okundu = r.Read()) && basla > 1)
            {
                kayitSay++;
                basla--;
            }

            if (basla <= 1 && okundu)
            {
                eklenen = 0;
                devam = true;
                kayitSay++;
                while (devam)
                {
                    if (eklenen < say || say == 0)
                    {
                        Kullanici k = new Kullanici();
                        DatareaderdenKullaniciya(r, k);
                        klv.objeler.Add(k);
                        eklenen++;
                        okundu = r.Read();
                        devam = okundu;
                        if (okundu)
                            kayitSay++;
                    }
                    else
                        devam = false;
                }
            }
            if (okundu)
                while (r.Read())
                    kayitSay++;
            return kayitSay;
        }

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// TABKullanicinin ListeleKullanici metodunu çağırır.
        /// TABKullanıcının döndürdüğü RecordSet'in basla parametresi ile verilen kayıtından başlayarak, 
        /// say parametresi ile verilen kadar kayıt döndürür. 
        /// 
        /// Sonuc yapısının kayıt sayısı alanına ise recorssetteki kayıt sayısını yazar.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <param name="basla">Sonuç kümesinde bu parametre ile belirtilen sıradaki kayıttan itibaren olanlar alınır.</param>
        /// <param name="say">Sonuç kümesinde bu parametre ile verilen sayıdaki kadar eleman yer alır. 0 ise tüm elemanlar sonuç kümesinde yer alır.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        public ObjectArray KullaniciAra(VeriErisim veriErisim, Kullanici aramaBilgisi, int basla, int say)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            ObjectArray klv = new ObjectArray();
            klv.sonuc = new Sonuc(true, "", "");
            TABKullanici tabKullanici = new TABKullanici(ve);
            if (aramaBilgisi.calistigiKurum != null && aramaBilgisi.calistigiKurum != "")
                aramaBilgisi.calistigiKurum = aramaBilgisi.calistigiKurum.Replace(".", "");
            IDataReader r = tabKullanici.ListeleKullanici(aramaBilgisi);
            klv.sonuc.hataStr = tabKullanici.Hata();
            if (klv.sonuc.hataStr == "")
            {
                klv.sonuc.kayitSay = DatareaderdenListeAl(klv, r, basla, say);
                r.Close();
            }
            else
                klv.sonuc.islemSonuc = false;

            ve.VeriErisimKapat();

            return klv;
        }

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// TABKullanicinin ListeleKullanici metodunu çağırır.
        /// TABKullanıcının döndürdüğü RecordSet'in basla parametresi ile verilen kayıtından başlayarak, 
        /// say parametresi ile verilen kadar kayıt döndürür. 
        /// 
        /// Sonuc yapısının kayıt sayısı alanına ise recorssetteki kayıt sayısını yazar.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        public ObjectArray KullaniciRaporla(VeriErisim veriErisim, Kullanici aramaBilgisi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            ObjectArray klv = new ObjectArray();
            klv.sonuc = new Sonuc(true, "", "");
            TABKullanici tabKullanici = new TABKullanici(ve);
            if (aramaBilgisi.calistigiKurum != null && aramaBilgisi.calistigiKurum != "")
                aramaBilgisi.calistigiKurum = aramaBilgisi.calistigiKurum.Replace(".", "");
            IDataReader r = tabKullanici.ListeleKullanici(aramaBilgisi, true);
            Hashtable yetkiTipleri = new Hashtable();
            if (r != null)
            {
                int sonAlan = (int)TABKullanici.ENUMAlanlar.RUMUZ;
                Kullanici kOnc = null;
                while (r.Read())
                {
                    Kullanici k = new Kullanici();
                    DatareaderdenKullaniciya(r, k);
                    if (kOnc == null || kOnc.kullaniciKodu != k.kullaniciKodu)
                    {
                        klv.objeler.Add(k);
                        kOnc = k;
                    }
                    int ytKod = VeriErisim.DegerAl(r, sonAlan + 1, 0);
                    string ytDeger = VeriErisim.DegerAl(r, sonAlan + 2, "");
                    if (ytKod > 0 && !string.IsNullOrWhiteSpace(ytDeger))
                    {
                        YetkiTipi yt;
                        try
                        {
                            if (yetkiTipleri.ContainsKey(ytKod))
                            {
                                yt = (YetkiTipi)yetkiTipleri[ytKod];
                            }
                            else
                            {
                                yt = TNS.KYM.Arac.YetkiTipiBul(ytKod);
                                yetkiTipleri[ytKod] = yt;
                            }
                            kOnc.yetkiTipiDegerleri[yt.ad, 0] = ytDeger;
                        }
                        catch { }
                    }
                }
                r.Close();
                klv.sonuc.ekNesne = yetkiTipleri;
            }
            else
                klv.sonuc.DegerAta(tabKullanici.Hata());
            ve.VeriErisimKapat();

            return klv;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcıların aynı olup olmadığına bakar. (Kullanıcı kodları aynı ise aynıdırlar.)
        /// </summary>
        /// <param name="kullanan">Aynı olduğu kontrol edilecek birinci kullanıcı parametresi.</param>
        /// <param name="kullanici">Aynı olduğu kontrol edilecek ikinci kullanıcı parametresi.</param>
        /// <returns>Verilen iki kullanıcının kullanıcı kodları aynı ise Doğru döndürülür, değilse Yanlış döndürülür.</returns>
        private bool KendisiMi(Kullanici kullanan, Kullanici kullanici)
        {
            if (kullanan.kullaniciKodu == kullanici.kullaniciKodu || kullanan.kullaniciKodu == kullanici.mernis)
                return true;
            else
                return false;
        }

        /// <summary>
        /// "kullanan" parametresi ile verilen kullanıcının, "kullanici" parametresi ile verilen kullanıcıyı yönetip yönetemediğini bulup, döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yönetme hakkına sahip mi diye kontrol edilecek kullanıcı parametresi.</param>
        /// <param name="kullanici">Yönetilip yönetilmediği bulunacak kullanıcı parametresi.</param>
        /// <returns>"kullanan", "kullanici"yi yönetiyorsa Doğru, değilse Yanlış döndürülür.</returns>
        public bool YonetiyorMu(Kullanici kullanan, Kullanici kullanici)
        {
            string girilebilecekKurumlar = "";

            //Tek kurum üzerinde çalıştığı için kuruma göre yönetir
            //return true;
            if (kullanici.aktifMi == ENUMAktiflik.PASIF)
                return true;
            else
            {
                string kopruKod = kullanan.YetkiTipiDegeriBul("KopruKod");
                if (!string.IsNullOrWhiteSpace(kopruKod))
                {
                    TNS.ORG.OrganizasyonBirim ob = KullaniciOrgBirimBul(kullanan, kullanici);
                    string gelenKoprukod = "";
                    if (ob != null)
                        gelenKoprukod = ob.kopruKod;
                    else
                        return true;
                    string[] kopruKodlar = kopruKod.Split(',', ';');
                    foreach (string s in kopruKodlar)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (gelenKoprukod.StartsWith(s))
                                return true;
                        }
                    }
                    return false;
                }
                else
                {
                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                    {
                        if (kullanan.calistigiKurum.EndsWith("00") || kullanan.calistigiKurum.EndsWith("50"))//İldeki kullanıcı yöneticisi ise
                            girilebilecekKurumlar = kullanan.calistigiKurum + ";" + kullanan.calistigiKurum.Substring(0, 2);
                        else
                            girilebilecekKurumlar = kullanan.calistigiKurum;
                    }
                    else
                        return true;
                    if (girilebilecekKurumlar == "")
                        return false;

                    if (!KullananBuDegeriGirebilirMi(kullanan.calistigiKurum, girilebilecekKurumlar, kullanici.calistigiKurum, true))
                        return false;
                    else
                        return true;
                }
            }
        }

        private ORG.OrganizasyonBirim KullaniciOrgBirimBul(Kullanici kullanan, Kullanici kullanici)
        {
            if (string.IsNullOrEmpty(kullanici.mernis)) return null;
            PER.IPERServis pers = TNS.PER.Arac.Tanimla();
            PER.Personel p = new PER.Personel();
            p.tcKimlikNo = kullanici.mernis;
            ObjectArray oa = pers.PersonelListele(kullanan, p);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                p = (PER.Personel)oa.objeler[0];
                if (p.muhasebeKodu > 0 && !string.IsNullOrWhiteSpace(p.birimKodu))
                {
                    TNS.ORG.IORGServis orgs = TNS.ORG.Arac.Tanimla();
                    ORG.OrganizasyonBirim ob = new ORG.OrganizasyonBirim();
                    ob.muhasebekodu = p.muhasebeKodu.ToString();
                    ob.muhasebebirim = p.birimKodu;
                    ObjectArray birimler = orgs.OrganizasyonBirimListele(ob, false, false);
                    if (birimler.sonuc.islemSonuc && birimler.ObjeSayisi > 0)
                    {
                        return (ORG.OrganizasyonBirim)birimler[0];
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///  Kendisine parametre olarak geçirilen "Kullanan" kullanıcısının, "Kullanıci" kullanıcısının bilgilerini görüp göremeyeceğini kontrol eder. 
        ///  Eğer görebiliyorsa true, değilse false döndürür.
        ///
        ///  Kontrol:
        ///     Herkes kendisini görür,
        ///     Sistem yöneticisi herkesi görür
        ///     Kurum Kullanıcı yöneticileri, kendi kurumlarındaki kullanıcıları görür.
        /// </summary>
        /// <param name="kullanan">Bilgileri görecek kullanıcı.</param>
        /// <param name="kullanici">Bilgileri görülecek kullanıcı</param>
        /// <returns>Görebiliyorsa Doğru, değilse Yanlış döndürülür.</returns>
        private bool BilgileriGorurMu(Kullanici kullanan, Kullanici kullanici)
        {
            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc)
                return true;
            if (KendisiMi(kullanan, kullanici))
                return true;
            if (TNS.KYM.Yetki.SistemveyaKullaniciYoneticisiMi(kullanan))
                return YonetiyorMu(kullanan, kullanici);
            if (Yetki.DestekYoneticisiMi(kullanan).islemSonuc)
                return true;
            return false;
        }

        /// <summary>
        /// Kullanıcı kodu veya TC Kimlik numarası veya Session numarası verilen kullanıcının bilgilerini okuyan ve döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Eğer 'sessionMi' parametresi Yanlış ise bu parametre kullanıcı kodu veya TC Kimlik numarasıdır. 'sessionMi' parametresi Doğru ise verilen değer session değeridir.</param>
        /// <param name="sessionMi">Verilen 'kullaniciKodu' değerininin kullanıcı kodu veya TC kimlik numarası yoksa session mı olduğı bilgisi. </param>
        /// <returns>Verilen anahtara göre okunan kullanıcı veri aktarım nesnesi döndürülür.</returns>
        private KullaniciVAN Oku(VeriErisim veriErisim, string kullaniciKodu, bool sessionMi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            KullaniciVAN kv = new KullaniciVAN();
            kv.sonuc.islemSonuc = true;
            TABKullanici tabKullanici = new TABKullanici(ve);
            IDataReader r = tabKullanici.Oku(kullaniciKodu, sessionMi);
            kv.sonuc.hataStr = tabKullanici.Hata();

            if (kv.sonuc.hataStr == "")
            {
                kv.sonuc.kayitSay = 1;
                if (r.Read())
                {
                    DatareaderdenKullaniciya(r, kv.kullanici);
                    kv.sonuc.anahtar = kv.kullanici.kullaniciKodu;
                }
                else
                {
                    kv.sonuc.islemSonuc = false;
                    kv.sonuc.hataStr = KullaniciResI.KYMKUL001;
                }
                r.Close();

                //Kullanıcı bulunamadı ise rumuz alanına göre arama yapılıyor
                if (!kv.sonuc.islemSonuc && !string.IsNullOrWhiteSpace(kullaniciKodu))
                {
                    r = tabKullanici.OkuRumuz(kullaniciKodu);
                    kv.sonuc.hataStr = tabKullanici.Hata();

                    if (kv.sonuc.hataStr == "")
                    {
                        kv.sonuc.kayitSay = 1;
                        if (r.Read())
                        {
                            DatareaderdenKullaniciya(r, kv.kullanici);
                            kv.sonuc.islemSonuc = true;
                            kv.sonuc.anahtar = kv.kullanici.kullaniciKodu;
                        }
                        else
                        {
                            kv.sonuc.islemSonuc = false;
                            kv.sonuc.hataStr = KullaniciResI.KYMKUL001;
                        }
                        r.Close();
                    }
                }

                Kullanici perKullanan = null;
                try
                {
                    TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();

                    if (kv.sonuc.islemSonuc)
                        perKullanan = orgServis.PersonelGorevYetkiBul(kv.kullanici.mernis);
                }
                catch
                {
                    perKullanan = null;
                }

                if (perKullanan != null)
                {

                    //*******************Merkez bankasın da pasif kullanıcıların giriş yapmaması için kaldırıldı. 08.08.2018  Hakan

                    //string kurum = TNS.DEG.Arac.VarsayilanKurumBul();

                    //if (kurum == "30")//Kullanıcı yetki ekranından pasif yapılan kullanıcı sisteme giriş yapabiliyor. TS13298 için bu kontrol kaldırıldı. En azından test işlemleri bitene kadar. 07.07.2014 Melih

                    //    kv.kullanici.aktifMi = ENUMAktiflik.AKTIF;

                    //*******************Merkez bankasın da pasif kullanıcıların giriş yapmaması için kaldırıldı. 08.08.2018  Hakan


                    if (!string.IsNullOrWhiteSpace(perKullanan.calistigiKurum))
                        kv.kullanici.calistigiKurum = perKullanan.calistigiKurum;
                    kv.kullanici.yetkiTipiDegerleri = perKullanan.yetkiTipiDegerleri;
                    kv.kullanici.kullaniciTipi = perKullanan.kullaniciTipi;

                    ve.VeriErisimKapat();
                    return kv;
                }

                if (kv.sonuc.islemSonuc)
                {
                    TABKullanicininTipleri tabKullanicininTipleri = new TABKullanicininTipleri(ve);
                    r = tabKullanicininTipleri.Listele(kv.kullanici.kullaniciKodu);
                    kv.sonuc.hataStr = tabKullanici.Hata();

                    if (kv.sonuc.hataStr == "")
                    {
                        TNSCollection c = new TNSCollection();
                        while (r.Read())
                        {
                            c.Add((ENUMKullaniciTipi)VeriErisim.DegerAl(r, 0, 0));
                        }
                        r.Close();

                        kv.kullanici.kullaniciTipi = new ENUMKullaniciTipi[c.Count];
                        for (int i = 0; i < c.Count; i++)
                        {
                            kv.kullanici.kullaniciTipi[i] = (ENUMKullaniciTipi)c[i];
                        }
                    }
                    else
                        kv.sonuc.islemSonuc = false;
                    r.Close();
                }

                if (kv.sonuc.islemSonuc)
                {
                    r = tabKullanici.YTOku(kv.kullanici.kullaniciKodu);

                    kv.sonuc.hataStr = tabKullanici.Hata();

                    if (kv.sonuc.hataStr == "")
                    {
                        kv.kullanici.yetkiTipiDegerleri.Clear();
                        while (r.Read())
                        {
                            try
                            {
                                int kod = VeriErisim.DegerAl(r, 0, -9999);
                                string ad = VeriErisim.DegerAl(r, 1, "");
                                if (kod == -1)
                                {
                                    kv.kullanici.sertNo = ad;
                                }
                                else if (kod >= 0)
                                    kv.kullanici.yetkiTipiDegerleri[TNS.KYM.Arac.YetkiTipiBul(kod).ad, 0] = ad;
                            }
                            catch { }
                        }
                        r.Close();
                        string kurumKod = kv.kullanici.yetkiTipiDegerleri["Kurum"];
                        if (kurumKod != kv.kullanici.calistigiKurum)
                            kv.kullanici.yetkiTipiDegerleri["Kurum", 0] = kv.kullanici.calistigiKurum;
                    }
                    else
                        kv.sonuc.islemSonuc = false;
                }
            }
            else
                kv.sonuc.islemSonuc = false;

            ve.VeriErisimKapat();
            return kv;
        }

        /// <summary>
        /// Kullanıcı kodu veya TC Kimlik numarası verilen kullanıcının bilgilerini okuyan ve döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Kullanıcı kodu veya TC Kimlik numarası parametresi.</param>
        /// <returns>Verilen anahtara göre okunan kullanıcı veri aktarım nesnesi döndürülür.</returns>
        public KullaniciVAN Oku(VeriErisim veriErisim, string kullaniciKodu)
        {
            return Oku(veriErisim, kullaniciKodu, false);
        }

        /// <summary>
        /// Kullanıcı kodu veya TC Kimlik numarası verilen kullanıcının bilgilerini okuyan ve kullanan okunan kullanıcıyı görebiliyorsa döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kullaniciKodu">Kullanıcı kodu veya TC Kimlik numarası parametresi.</param>
        /// <returns>Verilen anahtara göre okunan kullanıcı veri aktarım nesnesi döndürülür.</returns>
        public KullaniciVAN Oku(VeriErisim veriErisim, Kullanici kullanan, string kullaniciKodu)
        {
            KullaniciVAN kv = Oku(veriErisim, kullaniciKodu);
            if (kv.sonuc.islemSonuc)
            {
                if (!BilgileriGorurMu(kullanan, kv.kullanici))
                {
                    kv.sonuc.islemSonuc = false;
                    kv.sonuc.hataStr = string.Format(KullaniciResI.KYMKUL002, kullanan.kullaniciKodu, kullaniciKodu);
                    kv.kullanici = new Kullanici();
                }
                kv.sonuc.anahtar = kv.kullanici.kullaniciKodu;
            }
            return kv;
        }

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parola ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcı sisteme girişine izin verilir ve bilgileri döndürülür.
        /// Eğer kullanıcı bulunmuşsa, Veri tabanından gelen şifreli parola, ile gönderilen parolanın şifrelenmiş hali karşılaştırılır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN GirisYap(VeriErisim veriErisim, string kullaniciKodu, string parola)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            KullaniciVAN kv = Oku(ve, kullaniciKodu);
            //OrtakFonksiyonlar.HataStrYaz("kv.kullanici" + kv.kullanici.kullaniciKodu);
            //OrtakFonksiyonlar.HataStrYaz("kv.sonuc" + kv.sonuc.bilgiStr + "*" + kv.sonuc.hataStr + "*" + kv.sonuc.vtHatasi + "*" + kv.sonuc.islemSonuc);
            //Bazı alanlar Kapatıldı. Tarihçeden bakılabilir. TCCB kilitlenme araştırması yapılıyor. Şimdilik kapatıldı 14.12.2020 melih. İleride açılabilir

            bool girisYapabilir = true;
            //bool aktifHaleGeliyor = false;
            if (kv.sonuc.islemSonuc)
            {

                bool ldapSifreDogru = LDAPSifresiDogru(kullaniciKodu, parola, kv.kullanici);
                bool sadeceLDAP = OrtakFonksiyonlar.ConvertToInt(TNS.OrtakFonksiyonlar.WebConfigOku("SadeceLDAP", ""), 0) > 0;

                //Stratek kullanıcısı normal giriş yapabilsin
                if (kullaniciKodu == "stratek") sadeceLDAP = false;

                bool parolaDogru = false;
                if (kv.kullanici.parola == KYM.Yetki.Sifrele(parola) || kv.kullanici.parola == parola)//Manas tek noktadan giriş için veri tabanındaki parola geliyor
                    parolaDogru = true;

                if (sadeceLDAP)//Sadece LDAP ise normal şifre ile giriş yapamasın
                {
                    if (!ldapSifreDogru)
                    {
                        kv.sonuc.islemSonuc = false;
                        kv.sonuc.hataStr = KullaniciResI.KYMKUL003;
                        kv.kullanici = new Kullanici();
                        girisYapabilir = false;
                    }
                }
                else
                {
                    if (!ldapSifreDogru && !parolaDogru)
                    {
                        kv.sonuc.islemSonuc = false;
                        kv.sonuc.hataStr = KullaniciResI.KYMKUL003;
                        kv.kullanici = new Kullanici();
                        girisYapabilir = false;
                    }
                    else if (kv.kullanici.aktifMi != ENUMAktiflik.AKTIF)
                    {
                        //if (kv.kullanici.parolaBitisTarihi.Oku() > new TNSDateTime("01.01.1980").Oku() && kv.kullanici.parolaBitisTarihi.Oku() < new TNSDateTime(DateTime.Now.ToShortDateString()).Oku())
                        //{
                        //    girisYapabilir = true;
                        //    aktifHaleGeliyor = true;
                        //}
                        //else
                        {
                            kv.sonuc.islemSonuc = false;
                            kv.sonuc.hataStr = KullaniciResI.KYMKUL004;
                            girisYapabilir = false;
                        }
                    }
                }
                if (girisYapabilir)
                {
                    ve.BeginTransaction();
                    TABKullanici tab = new TABKullanici(ve);
                    //if (aktifHaleGeliyor)
                    //tab.TarihceKaydet(kv.kullanici.kullaniciKodu, kv.kullanici.kullaniciKodu, (int)ENUMTarihceIslem.AKTIF);

                    //tab.TarihceKaydet(kv.kullanici.kullaniciKodu, kv.kullanici.kullaniciKodu, (int)ENUMTarihceIslem.BASARILIGIRIS);

                    //string girisSayisiKontrol = TNS.DEG.Arac.DegiskenDegerBul(0, "GirisDenemeSayiTAB") + "";
                    //if (girisSayisiKontrol != "")
                    //tab.KullaniciGirisSayisiGuncelle(kv.kullanici.kullaniciKodu, 0);

                    ve.hata = "";
                    tab.GuncelleSonGiris(kv.kullanici.kullaniciKodu, new TNSDateTime(DateTime.Now));
                    if (tab.Hata() != "")
                    {
                        ve.Rollback();
                        kv.sonuc.islemSonuc = false;
                        kv.sonuc.hataStr = KullaniciResI.KYMKUL005 + " (" + tab.Hata() + ")";
                    }
                    else
                        ve.Commit();
                }
                else
                {
                    TABKullanici tab = new TABKullanici(ve);
                    ve.hata = "";
                    //tab.TarihceKaydet(kullaniciKodu, kullaniciKodu, (int)ENUMTarihceIslem.BASARISIZGIRIS);
                }

                try
                {
                    if (kv.sonuc.islemSonuc == false)
                    {
                        //Kapatıldı. TCCB kilitlenme araştırması yapılıyor. Şimdilik kapatıldı 14.12.2020 melih. İleride açılabilir
                        //kv = GirisDenemeKontrolu(ve, kv, kullaniciKodu);
                    }
                }
                catch { }
            }
            else
            {
                kv.sonuc.hataStr = KullaniciResI.KYMKUL006;
            }


            ve.VeriErisimKapat();
            return kv;
        }

        private KullaniciVAN GirisDenemeKontrolu(VeriErisim ve, KullaniciVAN kv, string kullaniciKodu)
        {
            //GirisDenemeSayiTAB değeri kadar arka arkaya hatalı giriş yapılırsa kullanıcı pasif yapılır
            //GirisDenemeSayiTAB değeri yoksa işlem yapılmaz

            ve.hata = "";
            int girisSayisiKontrol = TNS.OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "GirisDenemeSayiTAB") + "", 0);
            if (girisSayisiKontrol == 0) return kv;

            TABKullanici tab = new TABKullanici(ve);
            string kontrolKullaniciKodu = "";
            int sayac = 0;

            IDataReader r = tab.KullaniciGirisSayisiListele(kullaniciKodu);
            if (r != null)
            {
                while (r.Read())
                {
                    kontrolKullaniciKodu = VeriErisim.DegerAl(r, 0, "");
                    //DStarih = VeriErisim.DegerAl(r, 1);
                    sayac = VeriErisim.DegerAl(r, 2, 0);
                }
                r.Close();

                if (!string.IsNullOrEmpty(kontrolKullaniciKodu))
                {
                    if (sayac < girisSayisiKontrol)
                        sayac += 1;
                    else if (sayac >= girisSayisiKontrol)
                    {
                        sayac = 0;
                        kv.kullanici.kullaniciKodu = kontrolKullaniciKodu;
                        kv.kullanici.parolaBitisTarihi = new TNSDateTime(DateTime.Now.AddDays(7));
                        AktifPasifYap(ve, kv.kullanici, kv.kullanici, ENUMAktiflik.PASIF);
                        kv.sonuc.hataStr = KullaniciResI.KYMKUL051;

                        string yazilimAd = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimAd", "");
                        string from = TNS.OrtakFonksiyonlar.WebConfigOku("yazilimMailAdresi", "");
                        string baslik = "Kullanıcı pasif yapıldı[" + kontrolKullaniciKodu + "]";
                        string icerik = "Hatalı giriş nedeniyle kullanıcı pasif yapıldı.<br><br>Tarih:" + DateTime.Now.ToString() + "<br>Kullanıcı Adı:" + kontrolKullaniciKodu + "<br><br>İyi çalışmalar.<br>" + yazilimAd;
                        string to = TNS.DEG.Arac.DegiskenDegerBul(0, "GirisDenemeSayiEPostaAdres") + "";
                        if (to != "")
                            TNS.OrtakFonksiyonlar.MailAt(from, to, baslik, icerik, true, false, null);

                    }
                    tab.KullaniciGirisSayisiGuncelle(kontrolKullaniciKodu, sayac);
                }
                else
                {
                    kontrolKullaniciKodu = kullaniciKodu;
                    sayac += 1;
                    tab.KullaniciGirisSayisiKaydet(kontrolKullaniciKodu, sayac);
                }
            }

            return kv;
        }

        static string TarimAPIGrant = "https://api.tarimorman.gov.tr/auth/jwt";
        static string TarimAPIADKontrol = "https://api.tarimorman.gov.tr/YetkiNet/ADKontrol";
        static string TarimbearerToken = "eyJraWQiOiJiNmQyMmY1Ni04NzRmLTQ3MTEtYTc3OC1kNWNjNWJmMTg2NzgiLCJhbGciOiJSUzI1NiJ9.eyJYLUV4cGlyZXNBdCI6IjIwMjMtMTItMTlUMTQ6NTE6NTguNTM2WiIsImF1ZCI6IiNHVyMxM2QzMmMyYy04MzdiLTQ1ZWEtYTEwYi02YjZmMDQ0M2Q2NmIiLCJYLUlzc3VlZEF0IjoiMjAyMy0xMi0xOVQxMzo1MTo1OC41MzZaIiwiWC1BcHBsaWNhdGlvbk5hbWUiOiJZZXRraU5ldEFEIiwic2NvcGUiOiIiLCJpc3MiOiJzZ2JOZXRVc2VyIiwiZXhwIjoxNzAyOTk3NTE4LCJpYXQiOjE3MDI5OTM5MTgsImp0aSI6IjYxOWVmZmNjLTRhNTYtNGY5Yi05Y2MyLTkwZWJmMGIzNzQ1MyIsIlgtRXhwaXJlc0luTWlsbGlzIjozNjAwMDAwfQ.iv82SFM9z2asrzyjFIMexHuZIbb-keL87fnQZPRlL-HdhjY9gRH2iBS257PN1wp_lNQGBd7iVlHZD_-nO8InpadZElc5igIBBFDBMNhae00Mx0F6dhehmPQOeAdMnNJBLTda0q6qZMx8Lv5pg3dl5vCpk2sPwpx5eSuyi6IcG7QqUx6FI1QTmPT9W8yYf6tKtDYp-NZjfRCiBhXIvbqeAHS_Fie3s5cblpaIH7LAXI19p-qf9se4ooXWKQO3Hefh6tP29y45tmdig4KvELdwCAtxBqPVbmdImoZhY1_bCrSB3WuCelnj-2AD6A3lulrK9dwgWD_Rf3CIJJRrMLn3DA";
        static string grantType = "password";
        static string username = "sgbNetUser";

        static string password = "cid34h&9xH5o";
        static string client_id = "13d32c2c-837b-45ea-a10b-6b6f0443d66b";
        // Güvenlik sertifikası doğrulama işlevi
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Özel doğrulama mantığı buraya eklenir
            // Bu örnekte her sertifikayı onaylamış gibi davranır (geçici çözüm)

            //OrtakFonksiyonlar.HataDosyaYaz("LDAP.txt", $"Sertifika Konu Adı: {certificate.Subject}", false);
            //OrtakFonksiyonlar.HataDosyaYaz("LDAP.txt", $"Sertifika Sorunları: {sslPolicyErrors}", false);

            // Geçici olarak tüm sertifikaları kabul et
            return true;
        }

        private static string PostDataYap(string key, string value, string ayrac, bool enterEkle)
        {
            return key + ayrac + WebUtility.UrlEncode(value) + "&";
            //return key + ayrac + value + "\r\n";
        }

        static string postDataTarim = "";
        static KullaniciServis()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("grant_type", grantType);
            map.Add("username", username);
            map.Add("password", password);
            map.Add("client_id", client_id);
            map.Add("client_secret", "-");

            int say = 1;
            postDataTarim = "";
            foreach (KeyValuePair<string, string> item in map)
            {
                postDataTarim += PostDataYap(item.Key, item.Value, "=", say != map.Count);
                say++;
            }
        }

        public static ADTokenClass gelenVeri = null;

        private bool TarimYetkiAl()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            WebRequest request = WebRequest.Create(TarimAPIGrant);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", $"Bearer {TarimbearerToken}");

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postDataTarim);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();
                        gelenVeri = TNS.JsonIslem.Oku<ADTokenClass>(result);
                    }
                }
            }
            catch (WebException ex)
            {
                OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", $"Hata Kodu: {ex.Status}, {ex.Message}", false);
            }
            return false;
        }

        private bool TarimLDAP(string kullaniciKodu, string parola, Kullanici kullanici)
        {
            string kulKod = "", domainAd = "";
            OrtakFonksiyonlar.StringiIkiyeBol(kullanici.EPOSTA, "@", ref kulKod, ref domainAd);
            if (string.IsNullOrWhiteSpace(kulKod) || string.IsNullOrWhiteSpace(domainAd))
                return false;

            if (domainAd.IndexOf("tarimorman.gov.tr") < 0)
            {
                return false;
            }
            if (gelenVeri == null || gelenVeri.SuresiDoldu())
            {
                TarimYetkiAl();
            }
            if (gelenVeri == null || gelenVeri.SuresiDoldu())
                return false;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;

            WebRequest request = WebRequest.Create(TarimAPIADKontrol);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", $"Bearer {gelenVeri.access_token}");

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                string sent = TNS.JsonIslem.Yaz(new ADRequest(kulKod, parola));
                writer.Write(sent);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();

                        ADResponse cevap = TNS.JsonIslem.Oku<ADResponse>(result);
                        if (cevap != null && cevap.ADKontrolResponse != null && cevap.ADKontrolResponse.ADKontrolResult != null && string.IsNullOrWhiteSpace(cevap.ADKontrolResponse.ADKontrolResult.Hata))
                            return true;
                        OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", result, false);
                    }
                }
            }
            catch (WebException ex)
            {
                OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", $"Hata Kodu: {ex.Status}, {ex.Message}", false);
            }
            return false;
        }

        private bool LDAPSifresiDogru(string kullaniciKodu, string parola, Kullanici kullanici)
        {
            string ldapAdres = TNS.DEG.Arac.DegiskenDegerBul(0, "LDAP");//"ldap.forumsys.com:389";
            string ldapSecure = TNS.DEG.Arac.DegiskenDegerBul(0, "LDAPSSL") + "";
            string ldapEk = TNS.DEG.Arac.DegiskenDegerBul(0, "LDAPEK") + "";

            if (string.IsNullOrWhiteSpace(ldapAdres))
                return false;
            if (ldapAdres == "TARIM")
                return TarimLDAP(kullaniciKodu, parola, kullanici);
            OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "ldap kontrolü girildi. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu + "*LDAPEk:" + ldapEk + "*Adres:" + ldapAdres + "*SSL:" + ldapSecure, false);
            OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "ldap adres kontrol. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu, false);

            if (!string.IsNullOrEmpty(ldapEk))
            {
                ldapEk = string.Format(ldapEk, kullaniciKodu);//uid={0},dc=tcmb,dc=gov,dc=tr

                try
                {
                    using (LdapConnection connection = new LdapConnection(ldapAdres))
                    {
                        connection.Timeout = new TimeSpan(0, 0, 10);
                        connection.AuthType = AuthType.Basic;
                        connection.SessionOptions.ProtocolVersion = 3; // Set protocol to LDAPv3

                        if (ldapSecure == "1")
                            connection.SessionOptions.SecureSocketLayer = true;

                        var credential = new NetworkCredential(ldapEk, parola);
                        connection.Bind(credential);
                        connection.Dispose();
                    }

                    // If the bind succeeds, the credentials are valid
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "ldap giriş yaptı. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu, false);
                    return true;
                }
                catch (LdapException ldapEx)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu + "*Hata Kodu:" + ldapEx.ErrorCode + "*Mesaj:" + ldapEx.Message + "*LDAPEk:" + ldapEk + "*Adres:" + ldapAdres + "*SSL:" + ldapSecure, false);

                    //49, Hatalı kullanıcı veya parola
                    //81, The LDAP server is unavailable
                    if (ldapEx.ErrorCode.Equals(49))
                    {
                        return false;
                    }

                    throw;
                }
            }
            else
            {
                //Eski yöntem ile bağlantı
                OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "findone ldap kontrolü. Tarih:" + DateTime.Now.ToString() + "*LDAPEk:" + ldapEk + "*Adres:" + ldapAdres + "*SSL:" + ldapSecure, false);

                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(ldapAdres, kullaniciKodu, parola);

                System.DirectoryServices.DirectorySearcher dsearch = new System.DirectoryServices.DirectorySearcher(de);
                try
                {
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "FindOne yapılıyor. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu, false);

                    System.DirectoryServices.SearchResult sr = dsearch.FindOne();
                    if (sr != null)
                    {
                        OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "Başarılı. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu, false);
                        return true;
                    }
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "Başarılı değil. Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu, false);
                }
                catch (LdapException ldapEx)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "LDAPHata:Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu + "*Hata Kodu:" + ldapEx.ErrorCode + "*Mesaj:" + ldapEx.Message, false);
                }
                catch (Exception ldapEx)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("LDAPHATA.txt", "Hata:Tarih:" + DateTime.Now.ToString() + "*Kullanici:" + kullaniciKodu + "*Hata Kodu:" + ldapEx.Message + "*Mesaj:" + ldapEx.StackTrace, false);
                }
            }

            return false;
        }

        /// <summary>
        /// İçişleri bakanlığı entegrasyonu ile sisteme giriş yapılmış ise, giriş yapan kullanıcının son giriş tarihini güncellemek için çağrılan yordam.
        /// Ayrıca kullanıcı pasif ise, hata döndürülür ve giriş yapılmasına izin verilmez.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanici">Son giriş tarihi güncellenecek ve aktifliği kontrol edilecek kullanıcı parametresi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GirisYapSession(VeriErisim veriErisim, Kullanici kullanici)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanici.KimlikBul());
            if (kullanici.aktifMi != ENUMAktiflik.AKTIF)
            {
                s.DegerAta(KullaniciResI.KYMKUL004);
            }
            else
            {
                ve.BeginTransaction();
                TABKullanici tab = new TABKullanici(ve);
                tab.GuncelleSonGiris(kullanici.kullaniciKodu, new TNSDateTime(DateTime.Now));
                if (tab.Hata() != "")
                {
                    ve.Rollback();
                    s.DegerAta(KullaniciResI.KYMKUL005 + " (" + tab.Hata() + ")");
                }
                else
                    ve.Commit();
            }

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// İçişleri bakanlığı ile entegrasyon için eklenen yordam. Giriş ekranına iç işleri uygulaması tarafından verilen session numarası kullanılarak içişleri bakanlığı web servisi çağıran ve servisten gelen kullanıcı bilgilerini döndürüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kullaniciKodu">Bilgileri döndürülecek kullanıcının sisteme giriş kodu.</param>
        /// <param name="sessionID">Giriş ekranına iç işleri uygulaması tarafından verilen oturum (session) numarası</param>
        /// <param name="giris">Doğru ise yordam sisteme giriş yapılırken çağrılıyordur. Yanlış ise kullanıcı bilgilerini görüntülemek için çağrılıyordur.</param>
        /// <returns>Bilgileri verilen kullanıcı okunmuşsa aktarım nesnesinin kullanıcı alanında döndürülür. Okunamamışsa aktarım nesnesinin sonuc alanında okuyamama nedeni döndürülür.</returns>
        public KullaniciVAN OkuWebServisten(Kullanici kullanan, string kullaniciKodu, string sessionID, bool giris)
        {
            KullaniciVAN kv = new KullaniciVAN();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", kullanan.KimlikBul());
            TABKullanici tabKullanici = new TABKullanici(ve);
            SGBKullanici sgbKullanici = new SGBKullanici();
            string islemBilgi = "";
            try
            {
                KYMServisI.IcisleriKullaniciServis.YetkiSGB servisKullanici = new KYMServisI.IcisleriKullaniciServis.YetkiSGB();
                servisKullanici.Url = System.Configuration.ConfigurationManager.AppSettings.Get("KullaniciWebServis") + "";

                islemBilgi = "<br>" + string.Format(KullaniciResI.KYMKUL007, servisKullanici.Url);
                if (giris)
                {
                    islemBilgi += "<br><br>" + string.Format(KullaniciResI.KYMKUL008, kullaniciKodu, sessionID);
                    sgbKullanici = servisKullanici.GirisYap(kullaniciKodu, sessionID);
                }
                else
                {
                    islemBilgi += "<br><br>" + string.Format(KullaniciResI.KYMKUL009, kullaniciKodu);
                    sgbKullanici = servisKullanici.KullaniciOku(kullaniciKodu);
                }
            }
            catch (ApplicationException aex)
            {
                kv.sonuc.DegerAta(". " + aex.Message + "<br>" + islemBilgi);
            }
            catch (Exception ex)
            {
                kv.sonuc.DegerAta(".. " + ex.Message + "<br>" + islemBilgi);
            }

            if (kv == null || kv.sonuc.islemSonuc == false)
            {
                if (kv == null)
                {
                    kv = new KullaniciVAN();
                    kv.sonuc.DegerAta(islemBilgi + "<br>" + string.Format(KullaniciResI.KYMKUL010));
                }
                else
                    kv.sonuc.DegerAta(islemBilgi + "<br>" + string.Format(KullaniciResI.KYMKUL011, kv.sonuc.hataStr));
                return kv;
            }

            if (sgbKullanici != null)
            {
                if (sgbKullanici.GirisSonuc)
                {
                    islemBilgi += "<br>" + KullaniciResI.KYMKUL012;
                    try
                    {
                        //Kullanıcı daha önce sgb.net sistemine kayıt edilmiş mi?
                        kv = Oku(null, kullanan, sgbKullanici.KimlikNumarasi);
                    }
                    catch (Exception e)
                    {
                        kv = new KullaniciVAN();
                        kv.sonuc.DegerAta(kv.sonuc.hataStr + e.Message + islemBilgi);
                        return kv;
                    }

                    islemBilgi += "<br>" + KullaniciResI.KYMKUL013;
                    if (kv != null && kv.sonuc.islemSonuc)
                    {
                        bool kaydet = false;
                        islemBilgi += "<br>" + KullaniciResI.KYMKUL014;

                        if (kv.kullanici.mernis != sgbKullanici.KimlikNumarasi + "" ||
                            kv.kullanici.ad.ToUpper() != sgbKullanici.Ad.ToUpper() + "" ||
                            kv.kullanici.soyad.ToUpper() != sgbKullanici.Soyad.ToUpper() + "")
                            kaydet = true;

                        if (kaydet)
                        {
                            KullaniciEsitle(ref kv.kullanici, sgbKullanici, false);
                            kv.sonuc = Kaydet(null, kullanan, kv.kullanici, false);
                        }
                    }
                    else
                    {
                        if (sgbKullanici.KimlikNumarasi.Trim() == "")
                            islemBilgi += "<br>" + KullaniciResI.KYMKUL015;

                        islemBilgi += "<br>" + string.Format(KullaniciResI.KYMKUL016, sgbKullanici.KimlikNumarasi, sgbKullanici.Ad, sgbKullanici.KullaniciKodu);

                        string araHata = kv.sonuc.hataStr;
                        KullaniciEsitle(ref kv.kullanici, sgbKullanici, true);
                        kv.sonuc = Kaydet(null, kullanan, kv.kullanici, true);

                        if (!kv.sonuc.islemSonuc)
                            kv.sonuc.hataStr += islemBilgi + "<br>" + araHata;
                    }
                }
                else
                    kv.sonuc.DegerAta(KullaniciResI.KYMKUL017);
            }
            else
                kv.sonuc.DegerAta(KullaniciResI.KYMKUL018);

            if (!giris)
            {
                if (!BilgileriGorurMu(kullanan, kv.kullanici))
                {
                    kv.sonuc.islemSonuc = false;
                    kv.sonuc.hataStr = KullaniciResI.KYMKUL019;
                    kv.kullanici = new Kullanici();
                }
                kv.sonuc.anahtar = kv.kullanici.kullaniciKodu;
            }
            ve.VeriErisimKapat();
            return kv;
        }

        /// <summary>
        /// İçişleri bakanlığından web servisle alınan Sgbkullanıcı nesnesinin bilgilerin, verilen kullanıcı nesnesine kopyalayan yordam.
        /// </summary>
        /// <param name="kullanici">Verilen kopyalanacağı kullanıcı nesnesi.</param>
        /// <param name="sgbKullanici">İçişleri kullanıcı sisteminden çekilen bilgileri tutan kullanıcı nesnesi.</param>
        /// <param name="kullaniciTipi">Doğru ise kullanıcı tipi bilgileri kopyalanır, Yanlış ise kopyalanmaz.</param>
        private void KullaniciEsitle(ref Kullanici kullanici, SGBKullanici sgbKullanici, bool kullaniciTipi)
        {
            kullanici.kullaniciKodu = sgbKullanici.KullaniciKodu + "";
            kullanici.mernis = sgbKullanici.KimlikNumarasi + "";
            kullanici.ad = sgbKullanici.Ad + "";
            kullanici.soyad = sgbKullanici.Soyad + "";
            kullanici.calistigiKurum = sgbKullanici.CalistigiKurumKodu + "";
            kullanici.dogumTarihi = new TNSDateTime(sgbKullanici.DogumTarihi);
            kullanici.emailAdresi = sgbKullanici.EmailAdresi + "";
            kullanici.telefon = sgbKullanici.Telefon + "";
            kullanici.calistigiBirim = sgbKullanici.CalistigiBirimAdi + "";
            kullanici.sicilNo = sgbKullanici.SicilNo + "";
            kullanici.unvan = sgbKullanici.Unvan + "";
            kullanici.sessionId = sgbKullanici.SessionId + "";
            //kullanici.parola = sgbKullanici.Parola;// TNS.KYM.Yetki.Sifrele(sgbKullanici.Parola + "");
            if (sgbKullanici.AktifMi == 1)
                kullanici.aktifMi = ENUMAktiflik.AKTIF;
            else
                kullanici.aktifMi = ENUMAktiflik.PASIF;

            //Yetki tipi değerleri kontrolu
            ArrayList al = new ArrayList();
            if (kullaniciTipi)
            {
                bool bulundu = false;
                if (sgbKullanici.kullaniciTipi != null && sgbKullanici.kullaniciTipi.Length > 0)
                {
                    for (int i = 0; i < sgbKullanici.kullaniciTipi.Length; i++)
                    {
                        bulundu = false;
                        if (kullanici.kullaniciTipi != null && kullanici.kullaniciTipi.Length > 0)
                        {
                            for (int j = 0; j < kullanici.kullaniciTipi.Length; j++)
                            {
                                if (sgbKullanici.kullaniciTipi[i].kod == (int)kullanici.kullaniciTipi[j])
                                {
                                    al.Add(kullanici.kullaniciTipi[j]);
                                    bulundu = true;
                                    break;
                                }
                            }
                        }
                        if (!bulundu && sgbKullanici.kullaniciTipi[i].kod > 0)
                        {
                            al.Add((ENUMKullaniciTipi)sgbKullanici.kullaniciTipi[i].kod);
                        }
                    }
                }

                kullanici.kullaniciTipi = new ENUMKullaniciTipi[al.Count];
                kullanici.kullaniciTipi = (ENUMKullaniciTipi[])al.ToArray(typeof(ENUMKullaniciTipi));
            }
            if (kullanici.kullaniciTipi.Length == 0)
            {
                kullanici.kullaniciTipi = new TNS.KYM.ENUMKullaniciTipi[1];
                kullanici.kullaniciTipi[0] = TNS.KYM.ENUMKullaniciTipi.ODEMEEMRI_MIF;
            }
        }

        /// <summary>
        /// Verilen parola doğruysa, yeni parolalar aynıysa ve yeni parolaların uzunluğu 4 karakterden fazla ise, kullanıcının parolası değiştirilir ve yeni parola değiştirme zamanı atanır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Parolası değiştirilecek kullanıcı.</param>
        /// <param name="eskiParola">Kullanıcının şu andaki parolası.</param>
        /// <param name="yeniParola1">Kullanıcının olması istenen parolası.</param>
        /// <param name="yeniParola2">Kullanıcının olması istenen parolası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParolaDegistir(VeriErisim veriErisim, Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            Sonuc sonuc = new Sonuc(false, "", "");
            yeniParola1 = yeniParola1.Trim();
            yeniParola2 = yeniParola2.Trim();
            if (ParolaGecerliMi(yeniParola1, yeniParola2))
            {
                KullaniciVAN kv = Oku(ve, kullanan.kullaniciKodu);
                if (kv.sonuc.islemSonuc)
                {
                    if (kv.kullanici.parola == KYM.Yetki.Sifrele(eskiParola))
                    {
                        ve.BeginTransaction();
                        TABKullanici tabKullanici = new TABKullanici(ve);
                        TNSDateTime zaman = new TNSDateTime(DateTime.Now.AddDays(60));

                        sonuc = tabKullanici.GuncelleParola(kullanan.kullaniciKodu, KYM.Yetki.Sifrele(yeniParola1), zaman);
                        if (sonuc.hataStr != "")
                        {
                            ve.Rollback();
                        }
                        else
                            ve.Commit();
                    }
                    else
                    {
                        sonuc.hataStr = KullaniciResI.KYMKUL003;
                    }
                }
                else
                {
                    sonuc.hataStr = KullaniciResI.KYMKUL022;
                }

            }
            else
                sonuc.hataStr = "Girilen iki parola birbirine eşit değil veya parola güvenlik gereksinimlerini (" + ParolaGuvenlikGereksinimleri + ") karşılamıyor";
            ve.VeriErisimKapat();
            if (sonuc.islemSonuc)
                IzlemeKaydet(kullanan.ipAddress, kullanan.kullaniciKodu, ENUMIzleme.PAROLADEGISIM, "KYM", "ParolaDegistir", null);

            return sonuc;
        }

        public static int ParolaUzunluk = 8;
        public string ParolaGuvenlikGereksinimleri { get { return $"En az {ParolaUzunluk} karakter, en az 1 büyük harf, 1 küçük harf, 1 noktalama işareti ve 1 rakam içermeli"; } }

        private bool ParolaGecerliMi(string yeniParola1, string yeniParola2)
        {
            if (yeniParola1 != yeniParola2) return false;
            if (yeniParola1.Length < ParolaUzunluk) return false;
            if (!OzelKarakterVarMi(yeniParola1, 1)) return false;
            if (!OzelKarakterVarMi(yeniParola1, 2)) return false;
            if (!OzelKarakterVarMi(yeniParola1, 3)) return false;
            if (!OzelKarakterVarMi(yeniParola1, 4)) return false;
            return true;
        }

        string noktalamaKarakterleri = ",.:;'\"?!…-—()[]{}\\/@" + "#%&+-=*^_|";
        private bool OzelKarakterVarMi(string parola, int B1K2N3R4)
        {
            foreach (char ch in parola)
            {
                if (B1K2N3R4 == 1 && Char.IsUpper(ch)) return true;
                if (B1K2N3R4 == 2 && Char.IsLower(ch)) return true;
                if (B1K2N3R4 == 4 && Char.IsDigit(ch)) return true;
                if (B1K2N3R4 == 3 && noktalamaKarakterleri.IndexOf(ch) >= 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 8 harfli rastgele bir dizgi üreten yordam.
        /// </summary>
        /// <returns>8 harfli rastgele üretilen dizgi döndürülür.</returns>
        public string ParolaUret()
        {
            DateTime suan = DateTime.Now;
            Random r = new Random((int)(suan.Ticks % 30000));
            string s = "";
            for (int i = 0; i < ParolaUzunluk; i++)
            {
                char ch;
                if (i == 0) ch = (char)(r.Next(25) + 65); //Büyük harf
                else if (i == 4) ch = (char)(r.Next(25) + 97); //Küçük harf
                else if (i == 7) ch = noktalamaKarakterleri[r.Next(noktalamaKarakterleri.Length)]; //Noktalama işareti
                else ch = (char)(r.Next(10) + 48); //Rakam
                s = s + (char)(ch);
            }
            return s;
        }

        /// <summary>
        /// Verilen değerin (tekDeğer), verilen dizide yer alıp almadığı kontrol edilir. Kontrol başlıyor mu şeklinde yapılır.
        /// </summary>
        /// <param name="kurumListesi">Verilen değerin aranacağı dizi.</param>
        /// <param name="tekDeger">Aranacak değer.</param>
        /// <returns>Verilen değer dizide yer alıyorsa (başlıyor şekliyle) Doğru, değilse Yanlış döndürülür.</returns>
        private bool DegerGirilebilecekKurumlarArasindaMi(string[] kurumListesi, string tekDeger)
        {
            string kurumsal = "", diger = "";
            OrtakFonksiyonlar.StringiIkiyeBol(tekDeger, "-", ref kurumsal, ref diger);
            if (kurumsal == "")
                return false;
            kurumsal = kurumsal.Replace(".", "");

            foreach (string s in kurumListesi)
            {
                string tut = s.Replace(".", "");
                if (!string.IsNullOrEmpty(tut))
                    if (kurumsal.StartsWith(tut))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Bir kullanıcı, başka bir kullanıcının yetki değerlerini giriyorsa/değiştiriyorsa, yeni girdiği değerlerini kendi çalıştığı birim sınırları içinde olması gerekir. Bu yordam girilen değerlerin bu kullanıcı tarafından girilip girilemeyeceğini kontrol eder.
        /// </summary>
        /// <param name="calistigiKurum">Veri giren kullanıcının çalıştığı birim/kurum.</param>
        /// <param name="girilebilecekKurumlar">Veri giren kullanıcının girebileceği değerler dizisi.</param>
        /// <param name="deger">Veri giren kullanıcının girdiği değer.</param>
        /// <param name="zorunlu">Girelen yetki tipinin zorunlu olup olmadığı.</param>
        /// <returns>Girilen değer, girilebilecek değerler arasında ise ve zorunlu olanlar için doluysa doğru döndürülür, aksi halde yanlış döndürülür.</returns>
        private bool KullananBuDegeriGirebilirMi(string calistigiKurum, string girilebilecekKurumlar, string deger, bool zorunlu)
        {
            if ((deger == null || deger == "") && !zorunlu)
                return true;

            if (calistigiKurum == deger)
                return true;
            else
            {
                string[] kurumListesi = girilebilecekKurumlar.Split(';');

                string[] d = deger.Split(';');
                int parcaSay = d.Length;
                if (parcaSay == 0)
                    return false;
                for (int i = 0; i < parcaSay; i++)
                {
                    if (!DegerGirilebilecekKurumlarArasindaMi(kurumListesi, d[i]))
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Bir kullanıcı, başka bir kullanıcının yetki değerlerini giriyorsa/değiştiriyorsa, yeni girdiği değerlerini kendi çalıştığı birim sınırları içinde olması gerekir. Bu yordam girilen değerlerin bu kullanıcı tarafından girilip girilemeyeceğini kontrol eder.
        /// </summary>
        /// <param name="kullanan">Değeri giren kullanıcının bilgilerini tutan nesne.</param>
        /// <param name="kullanici">Girilen değerlerin tutulduğu kullanıcı nesnesi.</param>
        /// <returns>Girilen değer, girilebilecek değerler arasında ise ve zorunlu olanlar için doluysa doğru döndürülür, aksi halde yanlış döndürülür.</returns>
        public bool KurumDegerleriMi(Kullanici kullanan, Kullanici kullanici)
        {
            string girilebilecekKurumlar = "";
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                girilebilecekKurumlar = kullanan.calistigiKurum;
            else
                return true;
            if (girilebilecekKurumlar == "")
                return false;

            if (!KullananBuDegeriGirebilirMi(kullanan.calistigiKurum, girilebilecekKurumlar, kullanici.calistigiKurum, true))
                return false;

            foreach (System.Collections.DictionaryEntry de in kullanici.yetkiTipiDegerleri)
            {
                string ytAd = (string)de.Key;
                if (ytAd.Equals("Kurum") || ytAd.Equals("Bütçe Hazırlık Tertip Şablonu") || ytAd.Equals("Tertip"))
                {
                    if (!KullananBuDegeriGirebilirMi(kullanan.calistigiKurum, girilebilecekKurumlar, HashTableArrayList.ArrayListToStr(de.Value), false))
                        return false;
                }
            }
            return true;
        }



        /// <summary>
        /// Web.Config dosyası MernisKontrolYapma değeri 0'dan büyükse Yanlış, değilse doğru döndüren yordam. Bu sayede Mernise bağlanılıp bağlanılmayacağına karar verilir.
        /// </summary>
        /// <returns>Mernise bağlanılacaksa Doğru, bağlanılmayacaksa Yanlış döndürülür.</returns>
        public static bool MerniseBagliMi()
        {
            string mernisKontrol = System.Configuration.ConfigurationManager.AppSettings.Get("MernisKontrolYapma") + "";

            if (OrtakFonksiyonlar.ConvertToInt(mernisKontrol, 0) > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen TC kimlik numarasının geçerliliğini kontrol eden yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Kontrol edilecek TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası geçerli ise Doğru, geçersiz ise Yanlış döndürülür.</returns>
        public bool TCKimlikNoKontrolu(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih, Sonuc s)
        {
            if (!MerniseBagliMi())
                return true;

            KullaniciVAN kMernisVAN = MernisBilgiOku(kullanan, tcKimlikNo, dogumTarih);
            if (kMernisVAN.sonuc.islemSonuc)
            {
                if (kMernisVAN.kullanici.mernis != tcKimlikNo)
                {
                    s.DegerAta($"Verilen TCden {tcKimlikNo} farklı bir TC {kMernisVAN.kullanici.mernis} döndü!");
                    return false;
                }
                else
                    return true;
            }
            else
            {
                s.DegerAta(kMernisVAN.sonuc.hataStr);
                return false;
            }
        }

        /// <summary>
        /// İşlemi yapan kullanıcının, bilgilerini değiştirmek istediği kullanıcı üzerinde işlem hakkının olup olmadığını kontrol eden yordam. 
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcı bilgilerini tutan nesne.</param>
        /// <param name="kullanici">İşlem yapılan kullanıcı bilgilerini tutan nesne.</param>
        /// <param name="yeniMi">Doğru ise işlem yapılan kullanıcı yeni eklenen bir kullanıcıdır. Yanlış ise var olan bir kullanıcı üzerinde işlem yapılmaktadır.</param>
        /// <param name="mailKontrolu">Mail adresi doğru mu kontrol edilir.</param>
        /// <returns>İşlem hakkı varsa Doğru, yoksa Yanlış döndürür.</returns>
        public bool KayitKontrolu(Kullanici kullanan, Kullanici kullanici, bool yeniMi)
        {
            if (TNS.KYM.Yetki.YoneticiMi(kullanan).islemSonuc)
                return true;
            else if (TNS.KYM.Yetki.SistemveyaKullaniciYoneticisiMi(kullanan))
            {
                return true;
            }
            if (KendisiMi(kullanan, kullanici) && !yeniMi)
                return true;
            if (Yetki.DestekYoneticisiMi(kullanan).islemSonuc)
            {
                //31.07.2013 destek sistem yöneticisi kullanıcı bilgilerini (destek kullanıcısı olmayan) değiştirsin.
                //foreach (ENUMKullaniciTipi kt in kullanici.kullaniciTipi)
                //{
                //    if (TNS.KYM.Arac.DestekKullaniciTipindenMi((int)kt))
                //        return true;

                //}

                return true;
            }

            return false;
        }

        /// <summary>
        /// Kullanıcının parolası, sistem tarafından değiştirilmişse, kullanıcıya yeni parolası posta yoluyla bildirilir. Bu postayı hazırlayan ve gönderen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcı bilgilerini tutan nesne.</param>
        /// <param name="kullanici">Parolası talep üzerine ve yeni oluşturulduğu için sistem tarafından atanan kullanıcı nesnesi.</param>
        /// <param name="yeniMi">Doğru ise işlem yapılan kullanıcı yeni eklenen bir kullanıcıdır. Yanlış ise var olan bir kullanıcı üzerinde işlem yapılmaktadır.</param>
        public void YeniParolaMailAt(Kullanici kullanan, Kullanici kullanici, bool yeniMi)
        {
            string yazilimAd = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimAd");
            string yazilimMailAdresi = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");
            string yazilimURL = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimURL");

            string yeniEski;
            if (yeniMi)
                yeniEski = KullaniciResI.KYMKUL025;
            else
                yeniEski = KullaniciResI.KYMKUL026;
            string mesaj = string.Format(KullaniciResI.KYMKUL027, kullanici.ad, kullanici.soyad, yazilimAd, yeniEski, kullanici.kullaniciKodu, kullanici.parola, yazilimURL, yazilimMailAdresi).Replace("<br>", "\r\n");

            try
            {
                OrtakFonksiyonlar.MailAt(yazilimMailAdresi, kullanici.emailAdresi, string.Format(KullaniciResI.KYMKUL028, yazilimAd), mesaj);
            }
            catch
            {
            }
        }

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcı bilgilerini veri tabanına kaydeden yordam. İşlemi yapanın bu bilgileri kaydetme hakkı varsa işlem yapılır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kullanici">Kaydedilecek kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yeniMi">Doğru ise daha önce sistemde kayıtlı olmayan bir kullanıcının verilerinin kaydedildiği yordama söylenmiş olur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Kullanici kullanici, bool yeniMi)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            bool mailAt = false;
            bool sertYaz = false;
            string kullaniciBilgi = "";

            if (TCKimlikNoKontrolu(kullanan, kullanici.mernis, kullanici.dogumTarihi, sonuc))
            {
                kullaniciBilgi = GuncellemeDegisiklikleriBul(kullanici, yeniMi);
                string kurumKod = kullanici.yetkiTipiDegerleri["Kurum"];
                if (kurumKod != kullanici.calistigiKurum.Replace(".", ""))
                    kullanici.yetkiTipiDegerleri["Kurum", 0] = kullanici.calistigiKurum.Replace(".", "");

                if (KayitKontrolu(kullanan, kullanici, yeniMi))
                {
                    kullanici.calistigiKurum = kullanici.calistigiKurum.Replace(".", "");

                    TABKullanici tabKullanici = new TABKullanici(ve);
                    ve.BeginTransaction();

                    sonuc = KontrolEt(kullanici);
                    if (yeniMi && sonuc.islemSonuc)
                    {
                        kullanici.parola = ParolaUret();

                        int kullananSay = tabKullanici.KullaniciKoduSayiBul(kullanici.kullaniciKodu);
                        if (kullananSay != 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = string.Format(KullaniciResI.KYMKUL048, kullanici.kullaniciKodu);
                        }

                        kullananSay = tabKullanici.MernisSayiBul(kullanici.mernis);
                        if (kullananSay != 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = sonuc.hataStr + string.Format(KullaniciResI.KYMKUL049, kullanici.mernis);
                        }

                        if (sonuc.islemSonuc)
                        {
                            string tut = kullanici.parola;
                            kullanici.parola = KYM.Yetki.Sifrele(kullanici.parola);
                            sonuc = tabKullanici.Ekle(kullanici);
                            kullanici.parola = tut;
                        }
                        sertYaz = true;

                        if (sonuc.islemSonuc)
                        {
                            tabKullanici.TarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMTarihceIslem.YENI);
                            mailAt = true;
                        }
                    }
                    else if (sonuc.islemSonuc)
                    {
                        if (TNS.KYM.Yetki.SistemveyaKullaniciYoneticisiMi(kullanan))
                        {
                            sonuc = tabKullanici.Guncelle(kullanici);
                            sertYaz = true;
                        }
                        else
                            sonuc = tabKullanici.KisiselBilgileriKaydet(kullanici);

                        if (sonuc.islemSonuc)
                        {
                            tabKullanici.TarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMTarihceIslem.GUNCELLEME);
                            sonuc.anahtar = kullanici.kullaniciKodu;

                            //Kullanıcı üzerinde kullanıcı yöneticisi dışında HibeDestek , Görev ve Evrak Yöneticileride değişiklik yapabiliyorlar. Ama sertyaz olursa yetkitip değişikliği yapamıyorlar.24.01.2014 Melih
                            sertYaz = true;
                        }
                    }

                    string kullaniciGruplariTarihce = "";
                    string yetkiTipleriTarihce = "";
                    if (sonuc.islemSonuc && sertYaz)
                    {
                        //Yetkiler organizasyon görevden okunuyorsa yetki kaydı yapma
                        //******************************************************************
                        bool yetkiGorevdenGeliyor = false;

                        TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();
                        Kullanici perKullanan = null;
                        try
                        {
                            perKullanan = orgServis.PersonelGorevYetkiBul(kullanici.mernis);
                        }
                        catch
                        {
                            perKullanan = null;
                        }
                        if (perKullanan != null && perKullanan.kullaniciTipi.Length > 0)
                            yetkiGorevdenGeliyor = true;
                        //******************************************************************

                        if (!yetkiGorevdenGeliyor)
                        {
                            TABKullanicininTipleri tabKullanicininTipleri = new TABKullanicininTipleri(ve);
                            sonuc = tabKullanicininTipleri.Sil(kullanici.kullaniciKodu);
                            for (int j = 0; sonuc.islemSonuc && j < kullanici.kullaniciTipi.Length; j++)
                            {
                                if (kullanici.kullaniciTipi[j] != ENUMKullaniciTipi.TANIMSIZ)
                                {
                                    sonuc = tabKullanicininTipleri.Kaydet(kullanici.kullaniciKodu, (int)kullanici.kullaniciTipi[j]);
                                    if (kullaniciGruplariTarihce != "") kullaniciGruplariTarihce += "; ";
                                    kullaniciGruplariTarihce += kullanici.kullaniciTipi[j];
                                }
                            }

                            if (sonuc.islemSonuc)
                                sonuc = tabKullanici.YetkiTipiDegerleriniSil(kullanici);

                            if (sonuc.islemSonuc)
                            {
                                foreach (System.Collections.DictionaryEntry de in kullanici.yetkiTipiDegerleri)
                                {
                                    string yetkiTipiAd = (string)de.Key;
                                    string yetkiTipiDeger = HashTableArrayList.ArrayListToStr(de.Value);
                                    YetkiTipi yt = TNS.KYM.Arac.YetkiTipiBul(yetkiTipiAd);
                                    if (yt != null)
                                    {
                                        if (yetkiTipleriTarihce != "") yetkiTipleriTarihce += "; ";
                                        yetkiTipleriTarihce += yetkiTipiAd + "=" + yetkiTipiDeger;

                                        if (yt.zorunluMu == ENUMZorunluluk.ZORUNLU && String.IsNullOrEmpty(yetkiTipiDeger))
                                            sonuc.DegerAta(string.Format(KullaniciResI.KYMKUL050, yt.ad));
                                        else
                                            sonuc = tabKullanici.YetkiTipiDegeriKaydet(kullanici.kullaniciKodu, yt.kod, yetkiTipiDeger);

                                        if (!sonuc.islemSonuc) break;
                                    }
                                }
                            }
                            if (sonuc.islemSonuc)
                                sonuc = SertifikaKaydet(tabKullanici, kullanici);
                        }

                        if (sonuc.islemSonuc)
                        {
                            try
                            {
                                tabKullanici.YetkiTarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMYetkiTarihceTur.KULLANICITIPI, kullaniciGruplariTarihce);
                                tabKullanici.YetkiTarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMYetkiTarihceTur.YETKITIPI, yetkiTipleriTarihce);
                                tabKullanici.KullaniciDegisiklikTarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMYetkiTarihceTur.YETKIISLEMTIPI, kullaniciBilgi);
                            }
                            catch { }

                            ve.Commit();
                        }
                        else
                            ve.Rollback();
                    }
                }
                else
                    sonuc = new Sonuc(false, KullaniciResI.KYMKUL030, "");

                if (mailAt)
                    YeniParolaMailAt(kullanan, kullanici, true);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        private Sonuc SertifikaKaydet(TABKullanici tabKullanici, Kullanici kullanici)
        {
            return tabKullanici.YetkiTipiDegeriKaydet(kullanici.kullaniciKodu, -1, kullanici.sertNo);
        }

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcıyı, ENUMAktiflik parametresine göre aktif yada pasif yapan yordam. Kullanıcıyı aktif/pasif yapan kullanıcı, aktif/pasif yapılan kullanıcıyı yönetebilmelidir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kullanici">Aktif/Pasif yapılacak kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="aktifPasif">Kullanıcının aktif mi, pasif mi yapılacağı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AktifPasifYap(VeriErisim veriErisim, Kullanici kullanan, Kullanici kullanici, ENUMAktiflik aktifPasif)
        {
            if (string.IsNullOrWhiteSpace(kullanan.kullaniciKodu))
                return new Sonuc(KullaniciResI.KYMKUL037);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            Sonuc sonuc;

            //if (aktifPasif == ENUMAktiflik.AKTIF)
            //{
            //    sonuc = KontrolEt(kullanici);
            //    if (!sonuc.islemSonuc)
            //        return sonuc;
            //}

            if (KayitKontrolu(kullanan, kullanici, false))
            {
                TABKullanici tabKullanici = new TABKullanici(ve);

                if (kullanici.parolaBitisTarihi == null || kullanici.parolaBitisTarihi.isNull || kullanici.parolaBitisTarihi.Oku() < new TNSDateTime(DateTime.Now.ToShortDateString()).Oku())
                    kullanici.parolaBitisTarihi = new TNSDateTime("01.01.1980");//süresiz kapatıldı

                sonuc = tabKullanici.GuncelleAktifPasif(kullanici, aktifPasif);
                if (sonuc.islemSonuc)
                {

                    int islem;
                    if (aktifPasif == ENUMAktiflik.AKTIF)
                        islem = (int)ENUMTarihceIslem.AKTIF;
                    else
                        islem = (int)ENUMTarihceIslem.PASIF;

                    tabKullanici.TarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, islem);

                    if (aktifPasif == ENUMAktiflik.PASIF)
                    {
                        string islemAciklama = "";
                        if (kullanici.parolaBitisTarihi.Oku() == new TNSDateTime("01.01.1980").Oku())
                            islemAciklama = "Süresiz pasif hale getirildi.";
                        else
                            islemAciklama = kullanici.parolaBitisTarihi.ToString() + " tarihine kadar pasif hale getirildi.";

                        tabKullanici.YetkiTarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMTarihceIslem.PASIF, islemAciklama);
                    }
                    sonuc.anahtar = kullanici.kullaniciKodu;
                }
            }
            else
            {
                sonuc = new Sonuc(false, KullaniciResI.KYMKUL030, "");
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kullanıcı için, yetki kontrolleri yapıldıktan sonra yeni parola üreten ve kullanıcıya mail atan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kullanici">Sistem tarafından üretilmiş parola atanacak kullanıcı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParolaAta(VeriErisim veriErisim, Kullanici kullanan, Kullanici kullanici)
        {
            Sonuc sonuc;
            string parola;
            sonuc = KontrolEt(kullanici);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());

            if (KayitKontrolu(kullanan, kullanici, false))
            {
                ve.BeginTransaction();
                TABKullanici tabKullanici = new TABKullanici(ve);
                TNSDateTime zaman = new TNSDateTime(DateTime.Now);
                sonuc = tabKullanici.GuncelleParola(kullanici.kullaniciKodu, KYM.Yetki.Sifrele(parola = ParolaUret()), zaman);
                if (sonuc.islemSonuc)
                {
                    tabKullanici.TarihceKaydet(kullanan.kullaniciKodu, kullanici.kullaniciKodu, (int)ENUMTarihceIslem.SIFREATA);

                    ve.Commit();
                    kullanici.parola = parola;
                    YeniParolaMailAt(kullanan, kullanici, false);
                    sonuc.anahtar = kullanici.kullaniciKodu;
                }
                else
                    ve.Rollback();
            }
            else
            {
                sonuc = new Sonuc(false, KullaniciResI.KYMKUL030, "");
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Kullanıcı nesnesinde bilgileri verilen kullanıcı için ad ve soyad alanlarını kullanarak, olası kullancı adları üreten ve döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Kullanıcı adı üretilecek kullanıcıya ait bilgileri tutan parametre.</param>
        /// <returns>Üretilen kullanıcı adları liste olarak döndürülür.</returns>
        public ObjectArray KullaniciAdOner(Kullanici kullanici)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            ObjectArray kullaniciOneri = new ObjectArray();

            string kKod = "";
            int sayac = 1;
            string ad = kullanici.ad.ToLower();
            string soyad = kullanici.soyad.ToLower();

            if (ad == "" || soyad == "")
                return kullaniciOneri;

            string[] a = ad.Split(' ');
            ad = a[0];

            while (true)
            {
                Kullanici kul = new Kullanici();

                switch (sayac)
                {
                    case 1:
                        kKod = ad;
                        break;
                    case 2:
                        kKod = soyad;
                        break;
                    case 3:
                        kKod = ad + "" + soyad;
                        break;
                    case 4:
                        kKod = ad.Substring(0, 1) + "" + soyad;
                        break;
                    case 5:
                        kKod = ad + "" + soyad.Substring(0, 1);
                        break;
                    case 6:
                        kKod = ad + "." + soyad;
                        break;
                    case 7:
                        kKod = ad.Substring(0, 1) + "." + soyad;
                        break;
                    case 8:
                        kKod = ad + "_" + soyad;
                        break;
                    case 9:
                        kKod = ad.Substring(0, 1) + "_" + soyad;
                        break;
                    default:
                        kKod = ad.Substring(0, 1) + "" + soyad + (sayac - 9).ToString();
                        break;
                }

                kul.kullaniciKodu = kKod;
                ObjectArray sonuc = KullaniciAra(ve, kul, 1, 1);
                if (sonuc.sonuc.kayitSay == 0)
                    kullaniciOneri.objeler.Add(kul);

                if (kullaniciOneri.objeler.Count > 5)
                    break;

                sayac++;
            }
            ve.VeriErisimKapat();

            return kullaniciOneri;
        }

        /// <summary>
        /// Kullanıcılar her sayfa talep ettiğinde, veri tabanına işlemle ilgili bilgileri yazan yordam. Bu sayede sistem kullanımıyla ilgili grafikler alınabilir.
        /// </summary>
        /// <param name="kullaniciKodu">Sayfayı çağıran kullanıcının sisteme giriş kodu.</param>
        /// <param name="tarih">Sayfanın çağrıldığı tarih.</param>
        /// <param name="hostName">Talebin yapıldığı yerin ip adresi.</param>
        /// <param name="sayfaAd">İşlem yapılan sayfanın adresi.</param>
        /// <param name="bilgi1">İşlemi yapan kullanıcının adı ve soyadı.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SessionAta(string kullaniciKodu, TNSDateTime tarih, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);
            Sonuc sonuc = tabKullanici.KaydetSession(kullaniciKodu, tarih, hostName, sayfaAd, bilgi1, bilgi2, bilgi3, bilgi4, bilgi5);
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kıstaslara göre sistemde yapılan son işlemleri listeleyen yordam. 
        /// </summary>
        /// <param name="kullaniciKodu">Verilen kullanıcının işlemleri listelenir.</param>
        /// <param name="sure">Şu andaki zamandan geriye doğru kaç dakikalık işlemlerin listeleneceği bilgisi.</param>
        /// <param name="hostName">Listelenecek kayıtların ip adresi kıstası.</param>
        /// <param name="sayfaAd">Listelenecek kayıtların sayfa adı kıstası.</param>
        /// <param name="bilgi1">Listelenecek kayıtların ad soyad kıstası.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>Verilen kıstaslara uyan site erişim bilgilerinin listesi döndürülür.</returns>
        public ObjectArray SessionListele(string kullaniciKodu, int sure, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);

            TNSDateTime tarih = new TNSDateTime(DateTime.Now.AddMinutes(sure * -1));

            IDataReader r = tabKullanici.SessionListele(kullaniciKodu, tarih, hostName, sayfaAd, bilgi1, bilgi2, bilgi3, bilgi4, bilgi5);

            while (r.Read())
            {
                bool ekle = true;
                KullaniciIslemYapan k = new KullaniciIslemYapan();

                k.kullaniciAd = VeriErisim.DegerAl(r, 0, "");
                k.islemTarih = VeriErisim.DegerAl(r, 1);
                k.islemYeri = VeriErisim.DegerAl(r, 2, "");
                k.islemSayfa = VeriErisim.DegerAl(r, 3, "");
                k.bilgi1 = VeriErisim.DegerAl(r, 4, "");
                k.bilgi2 = VeriErisim.DegerAl(r, 5, "");
                k.bilgi3 = VeriErisim.DegerAl(r, 6, "");
                k.bilgi4 = VeriErisim.DegerAl(r, 7, "");
                k.bilgi5 = VeriErisim.DegerAl(r, 8, "");

                foreach (KullaniciIslemYapan t in donusDeger.objeler)
                {
                    if (t.kullaniciAd == k.kullaniciAd)
                        ekle = false;
                }
                if (ekle)
                    donusDeger.objeler.Add(k);
            }

            r.Close();
            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Sisteme erişim bilgilerinin verilen zaman aralığındaki sayılarını, verilen tarih aralığında listeleyen yordam. 
        /// </summary>
        /// <param name="kullaniciKodu">Verile bir kullanıcıya ait veriler listelenir.</param>
        /// <param name="grafikZamanAraligi">Erişim sayılarının hangi sıklıkta raporlanacağı</param>
        /// <param name="tarih1">Erişim sayılarının raporlanacağı zaman diliminin başlangıcı.</param>
        /// <param name="tarih2">Erişim sayılarının raporlanacağı zaman diliminin bitişi.</param>
        /// <param name="sayfaAd">Raporlanacak sayfaların içerdiği dizgi.</param>
        /// <returns>Verilen zaman aralığındaki her bir zaman için oluşan erişim sayısı listesi KullaniciIslemYapan sınıfının islem tarih ve adet alanlarında döndürülür.</returns>
        public ObjectArray SessionGrafik(string kullaniciKodu, ENUMKullanimBilgiZaman grafikZamanAraligi, TNSDateTime tarih1, TNSDateTime tarih2, string sayfaAd)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);

            IDataReader r = tabKullanici.SessionGrafik(kullaniciKodu, grafikZamanAraligi, tarih1, tarih2, sayfaAd);

            while (r.Read())
            {
                KullaniciIslemYapan k = new KullaniciIslemYapan();

                k.islemTarih = VeriErisim.DegerAl(r, 0);
                k.adet = VeriErisim.DegerAl(r, 1, 0);

                donusDeger.objeler.Add(k);
            }

            r.Close();
            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcı değişkenininin değerleri, verilen değerler yapan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Değişken değerleri saklanacak kullanıcının kodu.</param>
        /// <param name="degisken">Değerleri saklanacak değişkenin ismi.</param>
        /// <param name="degerler">Saklanacak değişkenin değerleri.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DegiskenKaydet(VeriErisim veriErisim, string kullaniciKodu, string degisken, TNSCollection degerler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(kullaniciKodu)) return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);
            ve.BeginTransaction();
            sonuc = tabKullanici.DegiskenleriSil(kullaniciKodu, degisken);
            for (int i = 0; i < degerler.Count && sonuc.islemSonuc; i++)
            {
                string deger = (string)degerler[i];
                if (deger != null && deger != "")
                    sonuc = tabKullanici.DegiskenDegerEkle(kullaniciKodu, degisken, i, deger);
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Bir kullanıcının, bir değişkeninin değerlerini döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Değişken değerleri bulunacak kullanıcının sisteme giriş kodu.</param>
        /// <param name="degisken">Kullanıcının değerleri bulunacak değişkeninin ismi.</param>
        /// <returns>Kullanıcı değişkeninin değerleri liste olarak döndürülür.</returns>
        public TNSCollection DegiskenleriListele(VeriErisim veriErisim, string kullaniciKodu, string degisken)
        {
            TNSCollection degerler = new TNSCollection();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);
            IDataReader r = tabKullanici.DegiskenleriListele(kullaniciKodu, degisken);
            if (r != null)
            {
                while (r.Read())
                {
                    degerler.Add(VeriErisim.DegerAl(r, 3, ""));
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return degerler;
        }

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcının bilgileri mernis bağlantısı ile alan ve kullanıcı veri aktarım nesnesi ile döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Mernis bağlantısı ile bilgileri bulunacak kişinin TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası geçerli ise  aktarım nesnesinin kullanıcı alanına kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN MernisBilgiOku(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih)
        {
            KullaniciVAN kVAN = new KullaniciVAN();

            if (!MerniseBagliMi())
            {
                kVAN.sonuc.islemSonuc = false;
                kVAN.sonuc.hataStr = KullaniciResI.KYMKUL031;
                return kVAN;
            }

            try
            {
                TNS.KPM.IKPMServis serKPM = TNS.KPM.Arac.Tanimla();

                TNS.KPM.Istek istek = new TNS.KPM.Istek();
                istek.dogumTarih.Yaz(dogumTarih);
                istek.istekTur = ((int)ENUMIstekTur.NUFUSCUZDANORNEK);
                istek.tcKimlikNo = tcKimlikNo;
                istek.islemYapan = "Sistem";
                istek.entegrasyon = true;

                ObjectArray nufusList = serKPM.NufusKayitOrnekRaporla(kullanan, istek);

                if (!nufusList.sonuc.islemSonuc)
                {
                    kVAN.sonuc.DegerAta(nufusList.sonuc.hataStr);
                }
                else if (nufusList.ObjeSayisi == 0)
                    kVAN.sonuc.DegerAta("Kayıt dönmedi:" + tcKimlikNo + "," + istek.dogumTarih.ToString());
                else
                {
                    TNS.KPM.Kisi nufusOrnek = (TNS.KPM.Kisi)nufusList.objeler[0];

                    kVAN.kullanici.ad = nufusOrnek.ad;
                    kVAN.kullanici.soyad = nufusOrnek.soyad;
                    kVAN.kullanici.mernis = nufusOrnek.tcKimlikNo;
                    kVAN.kullanici.dogumTarihi = new TNSDateTime(nufusOrnek.dogumTarihi.Oku());
                    kVAN.kullanici.anaAdi = nufusOrnek.anaAd;
                    kVAN.kullanici.babaAdi = nufusOrnek.babaAd;
                    kVAN.kullanici.cinsiyet = nufusOrnek.cinsiyetAciklama;
                }
            }
            catch (Exception ex)
            {
                kVAN.sonuc.DegerAta(ex.Message);
            }

            return kVAN;
        }

        /// <summary>
        /// T.C. Kimlik numarasına göre Borçlunun adres bilgisini Mernis Kimlik Paylaşım Sistemine bağlanarak veren fonksiyon
        /// </summary>
        /// <param name="TCKimlikNo">Borçluun TC Kimlik Numarası</param>
        /// <returns>Kisi nesnesi içinde adres ve kimlik bilgileri bulunur. Eğer TC Kimlik hatalı ise boş döner </returns>
        public static TNS.KPM.Kisi AdresVer(string TCKimlikNo)
        {
            IKPMServis servisKPM = TNS.KPM.Arac.Tanimla();
            TNS.KPM.Istek istek = new TNS.KPM.Istek();
            istek.istekTur = (int)ENUMIstekTur.SADECEADRES;
            istek.tcKimlikNo = TCKimlikNo;
            istek.islemYapan = "KullaniciIslem";
            istek.islemTarih.Yaz(DateTime.Now);

            Kullanici k = new Kullanici();
            k.kullaniciTipi = new ENUMKullaniciTipi[1];
            k.kullaniciTipi[0] = ENUMKullaniciTipi.KIMLIKPAYLASIMI;

            ObjectArray ikametgahList = servisKPM.IkametgahRaporla(k, istek);
            TNS.KPM.Kisi ikametgah = new TNS.KPM.Kisi();

            if (ikametgahList.ObjeSayisi > 0)
                ikametgah = (TNS.KPM.Kisi)ikametgahList.objeler[0];

            return ikametgah;
        }

        /// <summary>
        /// Mernis sistemine kayıtlı olan ilçe bilgilerini alma işlemini yapar
        /// </summary>
        //public void MernisIlIlceOku()
        //{
        //    try
        //    {
        //        KPS service = Mernis.KPS.Types.KPSServis.Create();
        //        service.Proxy = OrtakFonksiyonlar.ProxyAyarla();

        //        IlceBilgisi[] sonuc = service.IlceListesiGetir();

        //        if (sonuc.Length > 0)
        //        {
        //            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
        //            yazYol = Path.GetDirectoryName(yazYol);//hatadosyası yolu
        //            string dosyaAd = yazYol + "\\" + "MernisIlceListesi.txt";

        //            StreamWriter sw = new StreamWriter(dosyaAd, false);

        //            for (int i = 0; i < sonuc.Length; i++)
        //            {
        //                int ilKod = sonuc[i].IlKod;
        //                int ilceKod = sonuc[i].Kod;
        //                string ilceAd = sonuc[i].Ad;
        //                sw.WriteLine(ilKod + "\t" + ilceKod + "\t" + ilceAd);
        //            }

        //            sw.Close();
        //        }
        //    }
        //    catch { }
        //}

        /// <summary>
        /// TC kimlik numarası verilen kullanıcı varsa adı ve soyadı, yoksa boş dizgi döndüren yordam. 
        /// </summary>
        /// <param name="tcKimlikNo">Kullanıcı listesinde var olup olmadığı aranacak kullanıcının TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası verilen kişi kayıtlı kullanıcı ise, adı ve soyadı dizgi olarak yoksa boş dizgi döndürülür..</returns>
        public string KullaniciVarMi(string tcKimlikNo)
        {
            Kullanici kullanici = new Kullanici();
            if (tcKimlikNo.IndexOf('@') > -1)
                kullanici.emailAdresi = tcKimlikNo;
            else
                kullanici.mernis = tcKimlikNo;

            ObjectArray uV = KullaniciAra(null, kullanici, 1, 1);

            if (uV.sonuc.islemSonuc && uV.objeler.Count > 0)
                return ((Kullanici)uV.objeler[0]).ad + " " + ((Kullanici)uV.objeler[0]).soyad + "-" + ((Kullanici)uV.objeler[0]).mernis;
            else
                return "";
        }

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcı bulunur ve Parola ata yöntemi çağrılarak, kullanıcıya yeni üretilmiş parola atınır.
        /// </summary>
        /// <param name="tcKimlikNo">Yeni üretilmiş parola atanacak kullanıcının TC kimlik numarası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciyaYeniParolaAta(string tcKimlikNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            Kullanici kAra = new Kullanici();
            kAra.mernis = tcKimlikNo;
            ObjectArray uListeVAN = KullaniciAra(null, kAra, 1, 1);

            KullaniciVAN kkV = new KullaniciVAN();

            if (uListeVAN.sonuc.islemSonuc)
            {
                Kullanici kullanici = (Kullanici)uListeVAN.objeler[0];
                kullanici.kullaniciTipi = new ENUMKullaniciTipi[1];

                Kullanici kullanan = new Kullanici();
                kullanan.kullaniciTipi = new ENUMKullaniciTipi[1];
                kullanan.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

                kkV.sonuc = ParolaAta(null, kullanan, kullanici);
                if (!kkV.sonuc.islemSonuc)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = kkV.sonuc.hataStr;
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = kkV.sonuc.hataStr;
            }

            return sonuc;
        }

        /// <summary>
        /// Muhasebe listesi döndüren yordam.
        /// </summary>
        /// <returns>Muhasebe listesi döndürülür.</returns>
        public static ListeSatir[] MuhasebeListesi()
        {
            ArrayList lv = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABKullanici tabKullanici = new TABKullanici(ve);
            IDataReader r = tabKullanici.MuhasebeListele();

            if (tabKullanici.Hata() == "")
            {
                while (r.Read())
                {
                    ListeSatir s = new ListeSatir(VeriErisim.DegerAl(r, 0, ""), VeriErisim.DegerAl(r, 1, ""));

                    lv.Add(s);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return (ListeSatir[])lv.ToArray(typeof(ListeSatir));
        }

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parolası ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcının bilgileri döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <param name="sifreli">Gönderilen parola açık mı şifreli mi bilgisi.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN GirisYapabilirMi(VeriErisim veriErisim, string kullaniciKodu, string parola, bool sifreli)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            KullaniciVAN kv = Oku(ve, kullaniciKodu);
            if (kv.sonuc.islemSonuc)
            {
                bool ldapSifreDogru = LDAPSifresiDogru(kullaniciKodu, parola, kv.kullanici);

                // Isakis yonetim sistemi sadece kullanici kodu ve hash'li kullanici kodunu gondererek dogrulama
                // isteyebilir, bu durumda isakisi tarafindan kullanici kodu, hash'li kullanici kodu (parola olarak) ve sifreli flag true
                // olarak gelecektir. Ayni kullanici kodu SGB tarafinda hash'lenerek gelen ile kontrol edilir.
                if ((!sifreli && !ldapSifreDogru && kv.kullanici.parola != KYM.Yetki.Sifrele(parola))
                       || string.IsNullOrEmpty(parola)
                       || (sifreli && (
                            (parola.Substring(0, 1) == "#" && parola != ParolasizGirisParolasiUret(kullaniciKodu)) ||
                            parola.Substring(0, 1) != "#" && kv.kullanici.parola != parola
                                      )
                           )
                   )
                {
                    kv.sonuc.islemSonuc = false;
                    kv.sonuc.hataStr = KullaniciResI.KYMKUL003;
                }
                else if (kv.kullanici.aktifMi != ENUMAktiflik.AKTIF)
                {
                    kv.sonuc.islemSonuc = false;
                    kv.sonuc.hataStr = KullaniciResI.KYMKUL004;
                }
            }
            else
                kv.sonuc.hataStr = KullaniciResI.KYMKUL006;
            ve.VeriErisimKapat();
            return kv;
        }

        public static Kullanici[] KullaniciYetkiTipleriOku(VeriErisim veriErisim, string[] kullaniciKod, int yetkiTipKod)
        {
            ArrayList liste = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", null);
            TABKullanici tab = new TABKullanici(ve);
            IDataReader dr = tab.YTOku(kullaniciKod, yetkiTipKod);

            if (dr != null)
            {
                Kullanici k = null;

                while (dr.Read())
                {
                    string kKod = VeriErisim.DegerAl(dr, (int)TABKullanici.ENUMAlanlarKULLANICIYT.KULLANICIKODU, "");
                    if (k == null || k.kullaniciKodu != kKod)
                    {
                        if (k != null)
                            liste.Add(k);
                        k = new Kullanici();
                        k.kullaniciKodu = kKod;
                    }
                    int kod = VeriErisim.DegerAl(dr, (int)TABKullanici.ENUMAlanlarKULLANICIYT.YTKOD, -9999);
                    string deger = VeriErisim.DegerAl(dr, (int)TABKullanici.ENUMAlanlarKULLANICIYT.DEGER, "");
                    if (kod == -1)
                        k.sertNo = deger;
                    else if (kod >= 0)
                        k.yetkiTipiDegerleri[TNS.KYM.Arac.YetkiTipiBul(kod).ad, 0] = deger;
                }

                if (k != null)
                    liste.Add(k);

                dr.Close();
            }

            ve.VeriErisimKapat();

            Kullanici[] arr = null;
            if (liste.Count > 0)
                arr = (Kullanici[])liste.ToArray(typeof(Kullanici));

            return arr;
        }

        public string GuncellemeDegisiklikleriBul(Kullanici kullanici, bool yenimi)
        {
            string degisiklikler = "";
            string mernis = "mernis: ";
            string ad = "ad: ";
            string soyad = "soyad: ";
            string calistigiBirim = "calistigiBirim: ";
            string sicilNo = "sicilNo: ";
            string unvan = "unvan: ";
            string akademikUnvan = "akademikUnvan: ";
            string email = "email: ";
            string telefon = "telefon: ";
            string kullaniciKodu = "kullaniciKodu: ";
            string calistigiKurum = "calisitigiKurum: ";
            string aramadaGosterme = "aramadaGosterme: ";
            string sertNo = "sertNo: ";
            string rumuz = "rumuz: ";
            string virgul = ", ";

            if (yenimi)
            {
                degisiklikler = mernis + kullanici.mernis.ToString() + virgul +
                                 ad + kullanici.ad + virgul +
                                 soyad + kullanici.soyad + "," +
                                 calistigiBirim + kullanici.calistigiBirim + virgul +
                                 sicilNo + kullanici.sicilNo + virgul +
                                 unvan + kullanici.unvan + virgul +
                                 akademikUnvan + kullanici.akademikUnvan + virgul +
                                 email + kullanici.emailAdresi + virgul +
                                 telefon + kullanici.telefon + virgul +
                                 kullaniciKodu + kullanici.kullaniciKodu + virgul +
                                 calistigiKurum + kullanici.calistigiKurum + virgul +
                                 aramadaGosterme + kullanici.aramadaGosterme + virgul +
                                 sertNo + kullanici.sertNo + virgul +
                                 rumuz + kullanici.rumuz;
            }
            else
            {
                KullaniciVAN k = Oku(null, kullanici.KullaniciKodu);

                if (kullanici.mernis != k.kullanici.mernis)
                    degisiklikler += mernis + "(YENİ:" + kullanici.mernis.ToString() + ") (ESKİ:" + k.kullanici.mernis + ")" + virgul;
                if (kullanici.ad != k.kullanici.ad)
                    degisiklikler += ad + "(YENİ:" + kullanici.ad.ToString() + ") (ESKİ:" + k.kullanici.ad + ")" + virgul;
                if (kullanici.soyad != k.kullanici.soyad)
                    degisiklikler += soyad + "(YENİ:" + kullanici.soyad.ToString() + ") (ESKİ:" + k.kullanici.soyad + ")" + virgul;
                if (kullanici.calistigiBirim != k.kullanici.calistigiBirim)
                    degisiklikler += calistigiBirim + "(YENİ:" + kullanici.calistigiBirim.ToString() + ") (ESKİ:" + k.kullanici.calistigiBirim + ")" + virgul;
                if (kullanici.sicilNo != k.kullanici.sicilNo)
                    degisiklikler += sicilNo + "(YENİ:" + kullanici.sicilNo.ToString() + ") (ESKİ:" + k.kullanici.sicilNo + ")" + virgul;
                if (kullanici.unvan != k.kullanici.unvan)
                    degisiklikler += unvan + "(YENİ:" + kullanici.unvan.ToString() + ") (ESKİ:" + k.kullanici.unvan + ")" + virgul;
                if (kullanici.akademikUnvan != k.kullanici.akademikUnvan)
                    degisiklikler += akademikUnvan + "(YENİ:" + kullanici.akademikUnvan.ToString() + ") (ESKİ:" + k.kullanici.akademikUnvan + ")" + virgul;
                if (kullanici.emailAdresi != k.kullanici.emailAdresi)
                    degisiklikler += email + "(YENİ:" + kullanici.emailAdresi.ToString() + ") (ESKİ:" + k.kullanici.emailAdresi + ")" + virgul;
                if (kullanici.telefon != k.kullanici.telefon)
                    degisiklikler += telefon + "(YENİ:" + kullanici.telefon.ToString() + ") (ESKİ:" + k.kullanici.telefon + ")" + virgul;
                if (kullanici.kullaniciKodu != k.kullanici.kullaniciKodu)
                    degisiklikler += kullaniciKodu + "(YENİ:" + kullanici.kullaniciKodu.ToString() + ") (ESKİ:" + k.kullanici.kullaniciKodu + ")" + virgul;
                if (kullanici.calistigiKurum != k.kullanici.calistigiKurum)
                    degisiklikler += calistigiKurum + "(YENİ:" + kullanici.calistigiKurum.ToString() + ") (ESKİ:" + k.kullanici.calistigiKurum + ")" + virgul;
                if (kullanici.aramadaGosterme != k.kullanici.aramadaGosterme)
                    degisiklikler += aramadaGosterme + "(YENİ:" + kullanici.aramadaGosterme.ToString() + ") (ESKİ:" + k.kullanici.aramadaGosterme + ")" + virgul;
                if (kullanici.sertNo != k.kullanici.sertNo)
                    degisiklikler += sertNo + "(YENİ:" + kullanici.sertNo.ToString() + ") (ESKİ:" + k.kullanici.sertNo + ")" + virgul;
                if (kullanici.rumuz != k.kullanici.rumuz)
                    degisiklikler += rumuz + "(YENİ:" + kullanici.rumuz.ToString() + ") (ESKİ:" + k.kullanici.rumuz + ")";

            }
            return degisiklikler.TrimEnd(',', ' ');
        }

        internal static void IzlemeKaydet(string ipAddress, string kullaniciKodu, ENUMIzleme izlemeKod, string modulKod, string sayfaAd, string ekBilgi)
        {
            try
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", "IZLEME");
                try
                {
                    ve.ExecuteNonquery("INSERT INTO IZLEMETABLOSU(TARIHSAAT, IP, KULLANICIKODU, IZLEMEKOD, MODULKOD, SAYFAAD, EKBILGI) VALUES("
                        + $"{VeriErisim.DegerYap(new TNSDateTime(DateTime.Now))}, {VeriErisim.DegerYap(ipAddress)}, {VeriErisim.DegerYap(kullaniciKodu)}, {VeriErisim.DegerYap(izlemeKod.ToString())}, {VeriErisim.DegerYap(modulKod)}, {VeriErisim.DegerYap(sayfaAd)}, {VeriErisim.DegerYap(ekBilgi)}"
                        + ")");
                }
                catch
                {
                }
                ve.VeriErisimKapat();

            }
            catch
            {
            }
        }

        public ObjectArray IslemTarihceRapor(Kullanici kullanan, TNSDateTime tarih1, TNSDateTime tarih2, string islemYapilanTCNo, string islemYapilanAd, string islemiYapanKullaniciAdi, int kullaniciTipi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKullanici tabKullanici = new TABKullanici(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKullanici.IslemTarihceRapor(tarih1, tarih2, islemYapilanTCNo, islemYapilanAd, islemiYapanKullaniciAdi, kullaniciTipi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                Kullanici kullanici = new Kullanici();
                kullanici.kullaniciKodu = VeriErisim.DegerAl(dr, 0, "");
                kullanici.dogumTarihi = VeriErisim.DegerAl(dr, 1);
                kullanici.anaAdi = VeriErisim.DegerAl(dr, 2, "");
                kullanici.telefon = VeriErisim.DegerAl(dr, 3, 0).ToString();//TARIHCEISLEM
                kullanici.unvan = VeriErisim.DegerAl(dr, 4, "");//YETKIISLEM 
                kullanici.aramadaGosterme = VeriErisim.DegerAl(dr, 5, 0);//TUR
                kullanici.ad = VeriErisim.DegerAl(dr, 6, "");
                kullanici.mernis = VeriErisim.DegerAl(dr, 7, "");

                if (kullanici.aramadaGosterme == 0) kullanici.rumuz = "Kullanıcı";
                else if (kullanici.aramadaGosterme == 1) kullanici.rumuz = "Kullanıcı Grubu";
                else if (kullanici.aramadaGosterme == 2) kullanici.rumuz = "Yetki Grubu";
                else if (kullanici.aramadaGosterme == 4) kullanici.rumuz = "Çoklu Hatalı Giriş";

                if (kullanici.telefon == "0") kullanici.telefon = "";

                if (kullanici.telefon == "1") kullanici.telefon = "Yeni Kayıt edildi";
                else if (kullanici.telefon == "2") kullanici.telefon = "Değişiklik Yapıldı";
                else if (kullanici.telefon == "3") kullanici.telefon = "Aktif edildi";
                else if (kullanici.telefon == "4") kullanici.telefon = "Pasif yapıldı";
                else if (kullanici.telefon == "5") kullanici.telefon = "Yeni Şifre Gönderildi";
                else if (kullanici.telefon == "6") kullanici.telefon = "Başarılı Sisteme Giriş";
                else if (kullanici.telefon == "7") kullanici.telefon = "Başarısız Sisteme Giriş";

                liste.objeler.Add(kullanici);
            }

            liste.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return liste;
        }
        private void DebugYaz(StringBuilder sb, string v, bool brEkle = true)
        {
            sb.AppendLine(TNS.DEG.Arac.MS() + ": " + v + (brEkle ? "<br />" : ""));
        }

        internal string Ping(Kullanici kullanan, int tur)
        {
            StringBuilder sb = new StringBuilder();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) && tur != 6)
            {
                DebugYaz(sb, "İşlem Yapma Yetkiniz Yok!");
            }
            else
            {
                DebugYaz(sb, "VT Bağlantısı alınacak!");
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
                TNSDateTime tarih = new TNSDateTime(DateTime.Now);
                if (tur == 3 || tur == 5 || tur == 6)
                {
                    DebugYaz(sb, "100 kayıt döndüren Sorgu çalıştırılacak");
                    IDataReader dr = ve.DataReaderDondur("SELECT TOP 100 * FROM KULLANICI_BILGI");
                    DebugYaz(sb, "Sorgu çalıştı sonuç: " + ve.hata);
                    if (dr != null)
                    {
                        DebugYaz(sb, "Kayıtlar okunuyor: ");
                        while (dr.Read())
                        {

                        }
                        DebugYaz(sb, "Datareader kapatılacak: ");
                        dr.Close();
                    }
                }
                if (tur == 4 || tur == 5 || tur == 6)
                {
                    DebugYaz(sb, "Transaction açılacak!");
                    ve.BeginTransaction();
                    DebugYaz(sb, "Veri tabanına kayıt eklenecek!");
                    TABKullanici tabKullanici = new TABKullanici(ve);
                    Sonuc sonuc = tabKullanici.KaydetSession(kullanan.kullaniciKodu, tarih, "", "Ping.aspx", "", "", "", "", "");
                    DebugYaz(sb, "Veri tabanı commit/rollback işlemi: " + sonuc.hataStr);
                    if (sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }

                DebugYaz(sb, "VT Bağlantısı kapatılacak!", false);
                ve.VeriErisimKapat();
            }

            return sb.ToString();
        }

        internal Sonuc IISLogYaz(Kullanici kullanan, int sayac, TNSCollection loglar)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("İşlem Yapma Yetkiniz Yok!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABKullanici tabKullanici = new TABKullanici(ve);
            ve.BeginTransaction();
            foreach (IISLogSatir logSatir in loglar)
            {
                s = tabKullanici.LogYaz(sayac++, logSatir);
                if (!s.islemSonuc)
                    break;
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// KDSs the rol listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] KDSRolListele()
        {
            ArrayList lv = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABKullanici to = new TABKullanici(ve);
            IDataReader r = to.KDSRolListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ListeSatir s = new ListeSatir(VeriErisim.DegerAl(r, 0, 0).ToString(), VeriErisim.DegerAl(r, 1, ""));
                    lv.Add(s);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return (ListeSatir[])lv.ToArray(typeof(ListeSatir));
        }

        /// <summary>
        /// KDSs the birim listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] KDSBirimListele()
        {
            ArrayList lv = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABKullanici to = new TABKullanici(ve);
            IDataReader r = to.KDSBirimListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ListeSatir s = new ListeSatir(VeriErisim.DegerAl(r, 0, 0).ToString(), VeriErisim.DegerAl(r, 1, ""));
                    lv.Add(s);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return (ListeSatir[])lv.ToArray(typeof(ListeSatir));
        }

        public static ListeSatir[] GITRolListele()
        {
            ArrayList lv = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABKullanici to = new TABKullanici(ve);
            IDataReader r = to.GITRolListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ListeSatir s = new ListeSatir(VeriErisim.DegerAl(r, 0, 0).ToString(), VeriErisim.DegerAl(r, 1, ""));
                    lv.Add(s);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return (ListeSatir[])lv.ToArray(typeof(ListeSatir));
        }

        public static ListeSatir[] GITBirimListele()
        {
            ArrayList lv = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABKullanici to = new TABKullanici(ve);
            IDataReader r = to.GITBirimListele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ListeSatir s = new ListeSatir(VeriErisim.DegerAl(r, 0, ""), VeriErisim.DegerAl(r, 1, ""));
                    lv.Add(s);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            return (ListeSatir[])lv.ToArray(typeof(ListeSatir));
        }

        private static string ParolasizGirisParolasiUret(string kullaniciKodu)
        {
            Stack<byte> stack = new Stack<byte>();
            int kullaniciKoduUzunluk = kullaniciKodu.Length;
            for (int i = 0; i < kullaniciKoduUzunluk; i++)
            {
                stack.Push(anahtar[255 - i]);
                stack.Push(anahtar[15 + i]);
                stack.Push((byte)kullaniciKodu[i]);
                stack.Push(anahtar[kullaniciKoduUzunluk + i]);
            }

            System.Security.Cryptography.SHA256Managed hashstring = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = hashstring.ComputeHash(stack.ToArray());
            return "#" + Convert.ToBase64String(hash);
        }

        private static readonly byte[] anahtar = new byte[256] {
        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x16, 0x0A, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
        0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0xA6, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
        0x30, 0x51, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
        0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
        0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
        0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
        0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
        0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,
        0x50, 0x51, 0x52, 0x53, 0xe4, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
        0x41, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
        0x60, 0x61, 0xA2, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x6B, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
        0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
        0x80, 0x81, 0x82, 0x83, 0x84, 0xf5, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
        0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9c, 0x9c, 0x9d, 0x9e, 0x9f,
        0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xaa, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
        0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf
        };


        public class ADRequest
        {
            public ADKontrolClass ADKontrol { get; set; }

            public ADRequest(string kul, string par)
            {
                ADKontrol = new ADKontrolClass();
                ADKontrol.sParola = par;
                ADKontrol.sKullaniciAdi = kul;
            }
        }
        public class ADKontrolClass
        {
            public string sKullaniciAdi { get; set; }
            public string sParola { get; set; }
        }


        public class ADResponse
        {
            public ADKontrolResponse ADKontrolResponse { get; set; }

        }

        public class ADKontrolResponse
        {
            public ADKontrolResult ADKontrolResult { get; set; }
        }

        public class ADKontrolResult
        {
            public long TcKimlikNo { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public int birimId { get; set; }
            public string birimAdi { get; set; }
            public string Unvan { get; set; }
            public string Hata { get; set; }
        }

        public class ADTokenClass
        {
            public string access_token { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("X-ExpiresAt")]
            public DateTime X_ExpiresAt { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("X-IssuedAt")]
            public DateTime X_IssuedAt { get; set; }
            public string X_ApplicationName { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }

            internal bool SuresiDoldu()
            {
                if (DateTime.Now >= X_ExpiresAt.ToLocalTime()) return true;
                return false;
            }
        }
    }
}
