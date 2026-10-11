using Newtonsoft.Json;
using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using TRMServisC.tr.gov.tarim.cezali;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeCeza
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeCeza to = new TABHibeCeza(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = BelgeNoAlInt(ve);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = to.SilKisiler(bilgi);
                if (sonuc.islemSonuc)
                {
                    if (bilgi.cezaliKisiler != null)
                    {
                        foreach (CezaliKisiler c in bilgi.cezaliKisiler)
                            if (c != null)
                            {
                                if (c.cezaKod == 0)
                                    c.cezaKod = bilgi.kod;

                                if (sonuc.islemSonuc)
                                    sonuc = to.KaydetKisiler(c);
                            }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod.ToString();
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ceza bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeCeza to = new TABHibeCeza(ve);

            ve.BeginTransaction();

            sonuc = to.SilKisiler(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = to.Sil(bilgi);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
                }
            }
            else
                sonuc.hataStr = "Cezalı kişiler silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, cezalı kişi bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SilKisi(Kullanici kullanan, Ceza bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(bilgi.cezaliKisiler[0].TCNO))
                sonuc.hataStr = "Silinecek kişi seçilmemiş" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeCeza to = new TABHibeCeza(ve);

            ve.BeginTransaction();

            sonuc = to.SilKisiler(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Cezalı kişi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Ceza kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeCeza to = new TABHibeCeza(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ceza f = new Ceza();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.baslangicTarihi = VeriErisim.DegerAl(r, 1);
                    f.bitisTarihi = VeriErisim.DegerAl(r, 2);
                    f.aciklama = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.kayitEden = VeriErisim.DegerAl(r, 4, string.Empty);
                    f.kayitTarihi = VeriErisim.DegerAl(r, 5);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, cezalı kişileri veritabanından okur 
        /// ve CezaliKisiler nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, cezalı kişiler listesini CezaliKisiler nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray CezaliKisileriListele(Kullanici kullanan, int cezaKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeCeza to = new TABHibeCeza(ve);
            IDataReader r = to.CezaliKisileriListele(cezaKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    CezaliKisiler f = new CezaliKisiler();

                    f.cezaKod = VeriErisim.DegerAl(r, 0, 0);
                    f.TCNO = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.adSoyad = VeriErisim.DegerAl(r, 2, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, cezalı kişileri veritabanından okur 
        /// ve CezaliKisiler nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, cezalı kişiler listesini CezaliKisiler nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleCezaVeCezaliKisiler(Kullanici kullanan, int cezaKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeCeza to = new TABHibeCeza(ve);
            IDataReader r = to.ListeleCezaVeCezaliKisiler(cezaKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ceza f = new Ceza();

                    f.aciklama = VeriErisim.DegerAl(r, 0, "");
                    f.cezaliKisiler = new CezaliKisiler[1];
                    f.cezaliKisiler[0] = new CezaliKisiler();
                    f.cezaliKisiler[0].cezaKod = VeriErisim.DegerAl(r, 1, 0);
                    f.cezaliKisiler[0].TCNO = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.cezaliKisiler[0].adSoyad = VeriErisim.DegerAl(r, 3, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBECEZA";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

        public Sonuc CezaliMi(string kimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeCeza c = new TABHibeCeza(ve);
            
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc.islemSonuc = c.CezaliMi(kimlikNo);
            if (sonuc.islemSonuc)
                sonuc.hataStr = "Kimlik Numarası " + kimlikNo + " olan kişi/firma, Ceza listesinde olduğu için işlem yapılamaz.";

            ve.VeriErisimKapat();
            return sonuc;

            string vno = "";
            if (kimlikNo.Length == 10)
            {
                vno = kimlikNo;
                kimlikNo = "";
            }

            try
            {
                TRMServisC.tr.gov.tarim.cezali.Service cezaServis = new TRMServisC.tr.gov.tarim.cezali.Service();
                TRMServisC.tr.gov.tarim.cezali.DestekDisiBilgileri ds = cezaServis.getDestekDisiBilgi(kimlikNo, vno);

                if (ds.bilgiler != null && ds.bilgiler.Length > 0)
                {
                    TNSDateTime bitTarih = new TNSDateTime(ds.bilgiler[0].BitisTarihi);
                    if (bitTarih.Oku() > DateTime.Now)
                    {
                        sonuc.hataStr = "Kimlik Numarası " + kimlikNo + " olan kişi/firma, Ceza listesinde olduğu için işlem yapılamaz.";
                        sonuc.islemSonuc = true;
                    }
                }
            }
            catch
            {
                sonuc.hataStr = "Yatırımcının Cezalı listesinden kontrolü sırasında, Tarım Bakanlığı Merkez Cezalı Veri tabanından kontrolü sırasında hata oluştu. Daha sonra tekrar deneyin.";
                sonuc.islemSonuc = true;
            }

            return sonuc;
        }

        //static string tokenAlinan = "";

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        ///// ve Malzeme nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray Listele(Kullanici kullanan, Ceza kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    string donenDeger = CezaliMi(kriter.kimlikNo, "ORG").anahtar;

        //    Ceza f = new Ceza();

        //    if (!string.IsNullOrWhiteSpace(donenDeger))
        //    {
        //        dynamic gelenCevap = JsonConvert.DeserializeObject<dynamic>(donenDeger);

        //        f.kimlikNo = gelenCevap["TCKimlik_VergiNo"];
        //        f.adi = gelenCevap["Adi"];
        //        f.soyadi = gelenCevap["Soyadi"];
        //        f.baslangicTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaBaslangicTarihi"]));
        //        f.bitisTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaBitisTarihi"]));
        //        f.kayitTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaKararTarihi"]));
        //        f.aciklama = gelenCevap["Aciklama"];
        //        if (f.kimlikNo.Length == 10) f.soyadi = "";
        //    }
        //    else
        //    {
        //        f.aciklama = "Cezalı kaydı bulunmamaktadır." + donenDeger;
        //    }

        //    bilgi.objeler.Add(f);

        //    return bilgi;
        //}

        //public Sonuc CezaliMi(string kimlikNo, string tur = "")
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    //localde cezalı kontrolü yapma
        //    string servisURL = System.Configuration.ConfigurationManager.AppSettings.Get("yazilimURL") + "";
        //    if (servisURL.IndexOf("localhost") > -1)
        //        return sonuc;

        //    if (string.IsNullOrWhiteSpace(kimlikNo))
        //        return sonuc;

        //    string donenDeger = getGeneralRequest(tokenAlinan, kimlikNo);
        //    if (!string.IsNullOrWhiteSpace(donenDeger))
        //    {
        //        if (tur == "ORG")
        //        {
        //            sonuc.anahtar = donenDeger;
        //            return sonuc;
        //        }

        //        dynamic gelenCevap = JsonConvert.DeserializeObject<dynamic>(donenDeger);

        //        Ceza f = new Ceza();
        //        f.kimlikNo = gelenCevap["TCKimlik_VergiNo"];
        //        f.adi = gelenCevap["Adi"];
        //        f.soyadi = gelenCevap["Soyadi"];
        //        f.baslangicTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaBaslangicTarihi"]));
        //        f.bitisTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaBitisTarihi"]));
        //        f.kayitTarihi = new TNSDateTime(OrtakFonksiyonlar.ConvertToStr(gelenCevap["CezaKararTarihi"]));
        //        f.aciklama = gelenCevap["Aciklama"];
        //        if (f.kimlikNo.Length == 10) f.soyadi = "";
        //        string kisiFirma = kimlikNo.Length == 10 ? "firma" : "şahıs";

        //        donenDeger = f.adi + " " + f.soyadi + " isimli " + kisiFirma + ", Ceza listesinde olduğu için işlem yapılamaz. Açıklama:" + f.aciklama;
        //    }

        //    if (!string.IsNullOrWhiteSpace(donenDeger))
        //    {
        //        sonuc.hataStr = "*" + donenDeger + "*";// "Kimlik Numarası " + kimlikNo + " olan kişi/firma, Ceza listesinde olduğu için işlem yapılamaz.";
        //        sonuc.islemSonuc = true;
        //    }
        //    return sonuc;
        //}

        //static string getGeneralRequest(string token, string kimlikNo)
        //{
        //    string resourceAddress = "https://apigateway.tarbil.gov.tr/api/v1/tbs/FarmerSupportOutTCTaxNo";
        //    string response = "";

        //    if (string.IsNullOrWhiteSpace(token))
        //    {
        //        tokenAlinan = getAuthCode();
        //        token = tokenAlinan;
        //    }

        //    var client = new WebClient();
        //    var dataContent = new { IdTaxNo = kimlikNo };
        //    var dataStringJSON = JsonConvert.SerializeObject(dataContent);

        //    client.Encoding = Encoding.UTF8;
        //    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        //    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);

        //    try
        //    {
        //        ServisLogYaz("resourceAddress:" + resourceAddress + " / dataStringJSON:" + dataStringJSON + " / token:" + token);
        //        response = client.UploadString(new Uri(resourceAddress), "POST", dataStringJSON);
        //    }
        //    catch (WebException we)
        //    {
        //        ServisLogYaz("Hata aldık:" + we.Message);

        //        if (we.Message.IndexOf("(401)") > -1)//"The remote server returned an error: (401) Unauthorized."
        //        {
        //            tokenAlinan = getAuthCode();
        //            return getGeneralRequest(tokenAlinan, kimlikNo);
        //        }
        //        else if (we.Message.IndexOf("(404)") > -1)//"The remote server returned an error: (401) Unauthorized."
        //        {
        //            return "";//Kişi cezaı değilse 404 dönüyor
        //        }
        //    }

        //    dynamic obj = JsonConvert.DeserializeObject<dynamic>(response);
        //    return bolun(obj);

        //}

        //static string getAuthCode()
        //{
        //    string kullaniciAdi = "stratek";
        //    string kullaniciSifre = "EVkGXzv5ZSnjZ5t3";
        //    string resourceAddress = "https://apigateway.tarbil.gov.tr/api/v1/auth";
        //    string response = "";

        //    var client = new WebClient();

        //    var dataContent = new { username = kullaniciAdi, password = kullaniciSifre };
        //    var dataStringJSON = JsonConvert.SerializeObject(dataContent);

        //    try
        //    {
        //        ServisLogYaz("resourceAddress (getAuthCode):" + resourceAddress);
        //        ServisLogYaz("dataStringJSON (getAuthCode):" + dataStringJSON);

        //        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        //        response = client.UploadString(new Uri(resourceAddress), "POST", dataStringJSON);
        //    }
        //    catch (WebException we)
        //    {
        //        ServisLogYaz("Hata aldık (getAuthCode):" + we.Message);
        //    }

        //    dynamic obj = JsonConvert.DeserializeObject<dynamic>(response);
        //    return bolun(obj);
        //}

        //static string bolun(dynamic obj)
        //{
        //    if (obj == null)
        //        return "";

        //    string tokenStr = "";
        //    int resultCode = OrtakFonksiyonlar.ConvertToInt(obj["resultCode"], 0);
        //    ServisLogYaz("resultCode: " + resultCode);

        //    if (resultCode == 1015)
        //    {
        //        ServisLogYaz("Kullanıcı adı veya şifre hatalı");
        //        return "Kullanıcı adı veya şifre hatalı";
        //    }
        //    else if (resultCode == 1014)
        //    {
        //        tokenStr = (string)obj["data"]["token"];
        //        ServisLogYaz("Token : " + tokenStr);
        //        return tokenStr;
        //    }
        //    else if (resultCode == 1001)
        //    {
        //        Newtonsoft.Json.Linq.JArray jArr = (Newtonsoft.Json.Linq.JArray)obj["data"];
        //        ServisLogYaz("Bilgialındı : resultCode 1001");
        //        return jArr[0].ToString();
        //    }
        //    else
        //    {
        //        ServisLogYaz("Diger Hata : " + (string)obj["statusMessage"]);
        //        return "Diger Hata : " + (string)obj["statusMessage"];
        //    }
        //}

        //private static void ServisLogYaz(string mesaj)
        //{
        //    mesaj = DateTime.Now.ToString() + "     " + mesaj;
        //    string dosyaAdi = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00");
        //    dosyaAdi += "CezaliServis";

        //    OrtakFonksiyonlar.HataDosyaYaz(dosyaAdi + ".txt", mesaj, false);
        //}

    }
}
