using System;
using System.Configuration;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.EVR.IsKurallari.Veri;
//using tr.com.ega.eimza.api;
//using tr.com.ega.eimza.api.araclar;
using WebServisler.MobilImzaEvrak;
using System.IO;
using EVRServisI;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Mobil imza bilgileriyle ilgili yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRMobilImza
    {
        /// <summary>
        /// Mobil imza için, sertfikanın gerçerliliğini kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <param name="tarih">ontrol edilecek tarih nesnesi</param>
        /// <returns>İşlem sonucu metin olarak döndürülür.</returns>
        public string SertifikaGecerliMi(byte[] data, TNSDateTime tarih)
        {
            //ImzaliVeri iv = new ImzaliVeri(data);
            //string bilgi = string.Empty;

            //if (iv.ImzaliMi)
            //{
            //    KontrolBilgisi kb = Dogrulayici.SertifikaGecerliliginiKontrolEt(iv.Imzalar[0].ImzaciSertifikasi, tarih.Oku(), SertifikaTipi.ImzaciSertifika);
            //    // KontrolBilgisi kb = iv.Imzalar[0].ImzaciSertifikasi.GecerliliginiKontrolEt(tarih.Oku());
            //    bilgi = kb.Aciklama;
            //}
            //else
            //{
            //    bilgi = EvrakI.EVRI099;
            //}
            //return bilgi;
            return "";
        }

        /// <summary>
        /// Mobil imza için, belgenin imza durumunu kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İmzalı ise doğru, yoksa yanlış döndürülür.</returns>
        public bool ImzaliMi(byte[] data)
        {
            //ImzaliVeri iv = new ImzaliVeri(data);
            //return iv.ImzaliMi;

            return false;
        }

        /// <summary>
        /// Mobil imza için, gönderilen veri dizisindeki imzaları döndüren yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İşlem sonucu imza bilgileri nesne koleksiyonu türünde döndürülür.</returns>
        public ObjectArray BelgedekiImzalar(byte[] data)
        {
            ObjectArray liste = new ObjectArray();
            //ImzaliVeri iv = new ImzaliVeri(data);
            //if (iv.ImzaliMi)
            //{
            //    foreach (tr.com.ega.eimza.api.Imza imza in iv.Imzalar)
            //    {
            //        TNS.EVR.Imza imz = new Imza();
            //        ImzaNesneDoldur(imza, imz);

            //        liste.objeler.Add(imz);
            //    }
            //}

            return liste;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, "imza" nesnesindeki bilgileri "imz" nesnesindeki bilgilere aktaran yordam.
        /// </summary>
        /// <param name="imza">imza bilgilerini içeren nesne</param>
        /// <param name="imz">imz bilgilerini içeren nesne</param>
        //private static void ImzaNesneDoldur(tr.com.ega.eimza.api.Imza imza, TNS.EVR.Imza imz)
        //{
        //    imz.beyanEdilenImzalamaTarihi = new TNSDateTime(imza.BeyanEdilenImzalamaTarihi);
        //    imz.imzalamaZamani = new TNSDateTime(imza.ImzalamaZamani);
        //    imz.sertifika = new ImzaSertifika();
        //    imz.sertifika.baslangicTarihi = new TNSDateTime(imza.ImzaciSertifikasi.BaslangicTarihi);
        //    imz.sertifika.bitisTarihi = new TNSDateTime(imza.ImzaciSertifikasi.BitisTarihi);
        //    imz.sertifika.ePosta = imza.ImzaciSertifikasi.EPosta;
        //    imz.sertifika.kimlikNo = imza.ImzaciSertifikasi.KimlikNo;
        //    imz.sertifika.sahibi = imza.ImzaciSertifikasi.Sahibi;
        //}

        /// <summary>
        /// Mobil imza için, veri dizisini imazaya gönderen yordam.
        /// </summary>
        /// <param name="data">Veri dizisi içeren nesne</param>
        /// <param name="gsmOperator">GSM operatörlerini içeren nesne</param>
        /// <param name="telNo">Telefon numarasını içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        public ImzaSonuc ImzayaGonder(byte[] data, ENUMGSMOperator gsmOperator, string telNo)
        {
            //ApiAyarlari.VarsayilanAyarlar.LisansDosyasi = ConfigurationManager.AppSettings.Get("MobilImzaLisansDosyasi") + "";//@"D:\Belgelerim\STRATEK\Sgb.NET\Istemci\EgaApi\maliye_elektronik_imza.lic";

            //Sonuc sonuc = new Sonuc();
            //ImzaSonuc imzaSonuc = new ImzaSonuc();

            //WebServisler.MobilImzaEvrak.MSS_Sender sender = new WebServisler.MobilImzaEvrak.MSS_Sender();
            //sender.Url = "http://88.247.125.212/MSS_ServiceTest/MSS_Sender.asmx";
            //ImzaliVeri iv = new ImzaliVeri(data);
            //WebServisler.MobilImzaEvrak.HashResult hres = null;
            //string imzaTipi = string.Empty;
            //if (!iv.ImzaliMi)
            //{
            //    imzaTipi = "Normal";
            //    hres = sender.GetHashForGsmOperator(data, telNo, (int)gsmOperator);

            //    if (hres.ResultCode == 0)
            //        hres.Message = "Normal";
            //}
            //else
            //{
            //    imzaTipi = "Normal";// "Paralel";
            //    hres = sender.GetHashForSignedDataForGsmOperator(data, telNo, (int)gsmOperator);

            //    if (hres.ResultCode == 0)
            //        hres.Message = "Paralel";
            //}

            //imzaSonuc.apTransId = hres.ApTransId;
            //imzaSonuc.isSuccessful = hres.IsSuccessful;
            //imzaSonuc.message = hres.Message;
            //imzaSonuc.parmakIzi = hres.ParmakIzi;
            //imzaSonuc.resultCode = hres.ResultCode;

            //return imzaSonuc;

            return null;
        }

        /// <summary>
        /// Mobil imza için, imzalama işlemini yapan yordam. 
        /// </summary>
        /// <param name="imzaSonuc">İmzalanacak bilgiyi içeren nesne</param>
        /// <param name="cepMesaj">İmzalamak için cep telefonuna gelen mesajı içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        public ImzaSonuc Imzala(ImzaSonuc imzaSonuc, string cepMesaj)
        {

            //WebServisler.MobilImzaEvrak.MSS_Sender sender = new WebServisler.MobilImzaEvrak.MSS_Sender();
            //sender.Url = "http://88.247.125.212/MSS_ServiceTest/MSS_Sender.asmx";

            //// sender.Url = "http://";

            //// Turkce karakter iceremez, max 160
            ////string mesaj = "Telefonda gozukecek bilgi mesaji";

            //// bu metodu cagirinca kisinin cep telefonuna mesaj gidiyor
            //// kisi telefonda imzalama yapana kadar bekleniyor.

            //SignatureResult sr = null;
            //if (imzaSonuc.message == "Normal")
            //{
            //    sr = sender.GetSignature(cepMesaj, imzaSonuc.apTransId);
            //}
            //else if (imzaSonuc.message == "Paralel")
            //{
            //    sr = sender.AddSignature(cepMesaj, imzaSonuc.apTransId);
            //}

            //switch (sr.ResultCode)
            //{
            //    case 0:
            //        // başarılı

            //        byte[] imzali_dosya = Convert.FromBase64String(sr.SignedData);

            //        //ImzaliVeri iv = new ImzaliVeri(imzali_dosya);

            //        //ZamanDamgasiParametreleri zdp = new ZamanDamgasiParametreleri();
            //        //// kullanıcı adı parola bilgileri tubitaktan verilecek
            //        //zdp.KullaniciAdi = ConfigurationManager.AppSettings.Get("ZamanDamgasiKullaniciAdi") + "";
            //        //zdp.Parola = ConfigurationManager.AppSettings.Get("ZamanDamgasiParola") + "";
            //        //zdp.SunucuAdresi = ConfigurationManager.AppSettings.Get("ZamanDamgasiSunucuAdres") + "";

            //        //foreach (Imza imza in iv.Imzalar)
            //        //{
            //        //    if (imza.ImzaZamanDamgalari == null || imza.ImzaZamanDamgalari.Length == 0)
            //        //    {
            //        //        ZamanDamgaci.ImzaZamanDamgasiEkle(imza, zdp);
            //        //    }
            //        //}
            //        imzaSonuc.imzaliDosya = imzali_dosya;
            //        imzaSonuc.sonuc.islemSonuc = true;
            //        imzaSonuc.sonuc.bilgiStr = sr.Message;
            //        File.WriteAllBytes("D:\\imzali.p7b", imzali_dosya);
            //        break;
            //    case 1:
            //        imzaSonuc.sonuc.islemSonuc = false;
            //        imzaSonuc.sonuc.hataStr = EvrakI.EVRI100 + "<br>" + sr.Message;
            //        break;
            //    case 2:
            //        imzaSonuc.sonuc.islemSonuc = false;
            //        imzaSonuc.sonuc.hataStr = EvrakI.EVRI101 + "<br>" + sr.Message;
            //        break;
            //    case 3:
            //        imzaSonuc.sonuc.islemSonuc = false;
            //        imzaSonuc.sonuc.hataStr = EvrakI.EVRI102 + "<br>" + sr.Message;
            //        break;
            //}

            
            //return imzaSonuc;

            return null;
        }
    }
}
