using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using System.Configuration;
using System.IO;


namespace TNS.GEL
{
    /// <summary>
    /// Harcama Yönetimi sınıfı için; istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Bu sunucu sınıfının nesnesini, konfigürasyon dosyasından okuduğu
        /// remoting dizgisini kullanarak, .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne</returns>
        public static IGELServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("GELServisURL") + "";
            if (string.IsNullOrEmpty(sUrl))
                return null;
            IGELServis servis = TNS.Api.ServisIstemci.Olustur<IGELServis>(sUrl);
            return servis;
        }

        public static string GetDescription<T>(int value)
        {
            object tmpItem = null;
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (OrtakFonksiyonlar.ConvertToInt(item, 0) == value)
                {
                    tmpItem = item;
                }
            }
            if (tmpItem != null)
            {
                System.Reflection.FieldInfo field = tmpItem.GetType().GetField(tmpItem.ToString());
                object[] attribs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                if (attribs.Length > 0)
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs[0]).Description;
                }
                else
                {
                    return Enum.GetName(typeof(T), value);
                }
            }
            return "";
        }

        public static string GetDescription(Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
            if (attribs.Length > 0)
            {
                return ((System.ComponentModel.DescriptionAttribute)attribs[0]).Description;
            }
            else
            {
                return Enum.GetName(value.GetType(), value);
            }
        }
 
    }

    /// <summary>
    /// Yetki kontrollerini düzenleyen fonksiyonları içeren sınıf
    /// </summary>
    [Serializable]
    public class Yetki
    {

        /// <summary>
        /// Arguman olarak verilen kriterler için yetki kontrolü yapar ve bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="ogy">Yetki kontrolü yapılacak okuma görme yazma bilgisi.</param>
        /// <param name="belgeTur">Yetki kontrolü yapılacak belge tür bilgisi.</param>
        /// <returns>Yetki kontrolü yapar ve bool olarak döner</returns>
        public static bool YetkisiVarMi(Kullanici kullanan, int ogy)//, int belgeTur
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.GELIRYONETIMIKULLANICI))
                return true;

            string yetkiTipiYetkiDegeri = "";
            //if (belgeTur == (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)TNS.MUH.ENUMBelgeTuru.MIF)
            //    yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Yetkisi");

            yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
            int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
            if (ogy == (int)ENUMOGY.Gorme && yetkiTipiYetkiDegeriInt > 0)
                return true;
            if (ogy == (int)ENUMOGY.Yazma && yetkiTipiYetkiDegeriInt > 1)
                return true;
            if (ogy == (int)ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                return true;
            if (ogy == (int)ENUMOGY.OnayKaldir && yetkiTipiYetkiDegeriInt == 5)
                return true;

            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Yazma);
        }

        /// <summary>
        /// Kullanıcının yönetici olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının yönetici olup olmadığını bool olarak döner</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }
    }

    [Serializable]
    public class DosyaIslemleri
    {
        public static Sonuc DosyaSil(string dosyaYolu)
        {
            Sonuc sonuc = new Sonuc();
            if (File.Exists(dosyaYolu))
            {
                try
                {
                    File.Delete(dosyaYolu);
                    sonuc.islemSonuc = true;
                }
                catch (Exception ex)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ex.Message.ToString();

                }

            }
            return sonuc;
        }

        public static bool KlasorVarmi(string klasorYolu)
        {
            bool klasorVar = false;
            if (Directory.Exists(klasorYolu))
            {
                klasorVar = true;
            }
            return klasorVar;
        }


    }

    public interface IGELServis
    {
        ObjectArray TasinmazTalepListele(Kullanici kullanan, TasinmazTalep bilgi);
        Sonuc TasinmazTalepKaydet(Kullanici kullanan, TasinmazTalep bilgi);
        Sonuc TasinmazTalepSil(Kullanici kullanan, TasinmazTalep bilgi, TasinmazTalepEki[] talepEkleri);
        ObjectArray TasinmazTalepEkListele(Kullanici kullanan, TasinmazTalepEki bilgi);
        Sonuc TasinmazTalepEkiKaydet(Kullanici kullanan, TasinmazTalepEki bilgi);
        Sonuc TasinmazTalepEkiSil(Kullanici kullanan, TasinmazTalepEki bilgi, bool ekNoKontrol = true);


        ObjectArray IhaleTurListele(KYM.Kullanici kullanan, IhaleTur kriter);
        Sonuc IhaleTurKaydet(Kullanici kullanan, IhaleTur bilgi);
        Sonuc IhaleTurSil(Kullanici kullanan, IhaleTur bilgi);

        ObjectArray IhaleTurMaddeListele(KYM.Kullanici kullanan, int ihaleKod, int tur);
        Sonuc IhaleTurMaddeKaydet(Kullanici kullanan, IhaleTurMadde bilgi);
        Sonuc IhaleTurMaddeSil(Kullanici kullanan, IhaleTurMadde bilgi);

        ObjectArray IhaleIlanSekliListele(Kullanici kullanan, int ihaleKod, int ihaleMaddeSira, int tur);
        Sonuc IhaleIlanSekliKaydet(Kullanici kullanan, IhaleIlanTanim bilgi);
        Sonuc IhaleIlanSekliSil(Kullanici kullanan, IhaleIlanTanim bilgi);


        Sonuc SurecKaydet(Kullanici kullanan, Surec bilgi);
        Sonuc SurecGuncelle(Kullanici kullanan, Surec bilgi);
        Sonuc SurecDurumGuncelle(Kullanici kullanan, Surec bilgi);
        Sonuc SurecParcaDurumGuncelle(Kullanici kullanan, SurecParcaDurum bilgi);
        Sonuc SurecSifreKaydet(Kullanici kullanan, SurecSifre bilgi);
        Sonuc SurecSifreKaldir(Kullanici kullanan, SurecSifre bilgi);
        ObjectArray SurecSifreListele(Kullanici kullanan, SurecSifre bilgi);
        Sonuc SurecTeklifTipiGetir(Kullanici kullanan, Surec bilgi);

        ObjectArray SurecListele(Kullanici kullanan, Surec bilgi);
        ObjectArray SurecParcaListele(Kullanici kullanan, int yil, string surecno);
        ObjectArray SurecParcaDurumListele(Kullanici kullanan, int yil, string surecno);

        ObjectArray OnayBilgisiListele(Kullanici kullanan, OnayForm bilgi);
        Sonuc OnayBilgisiKaydet(Kullanici kullanan, OnayForm bilgi, int islemkod, int parcakod);


        Sonuc KomisyonUyesiKaydet(Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod);
        Sonuc KomisyonUyesiSil(Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod);
        ObjectArray KomisyonUyesiListele(Kullanici kullanan, KomisyonUye bilgi);

        ObjectArray ParaBirimiListele(KYM.Kullanici kullanan, TNS.BKD.ParaBirimi paraBirimi);

        ObjectArray MuhammenBedelListele(KYM.Kullanici kullanan, MuhammenBedel bilgi);
        Sonuc MuhammenBedelKaydet(KYM.Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod);
        Sonuc MuhammenBedelSil(KYM.Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod);



        Sonuc TeklifFirmaKaydet(KYM.Kullanici kullanan, TeklifFirma[] bilgi, int islemkod, int parcakod);
        Sonuc TeklifFirmaSil(KYM.Kullanici kullanan, TeklifFirma bilgi, int islemkod, int parcakod);
        ObjectArray TeklifFirmaListele(KYM.Kullanici kullanan, TeklifFirma bilgi);


        Sonuc TeklifTasinmazKaydet(KYM.Kullanici kullanan, TeklifTasinmaz[] bilgi, int islemkod, int parcakod);
        Sonuc TeklifTasinmazSil(KYM.Kullanici kullanan, TeklifTasinmaz bilgi, int islemkod, int parcakod);
        ObjectArray TeklifTasinmazListele(KYM.Kullanici kullanan, TeklifTasinmaz bilgi);

        ObjectArray TeklifListele(KYM.Kullanici kullanan, Teklif bilgi);
        Sonuc TeklifSil(KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod);
        Sonuc TeklifKaydet(KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod);
        Sonuc TeklifKaydiVarmi(KYM.Kullanici kullanan, Teklif bilgi);


        ObjectArray KomisyonKarariListele(KYM.Kullanici kullanan, KomisyonKarar bilgi);
        Sonuc KomisyonKarariSil(KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod);
        Sonuc KomisyonKarariKaydet(KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod);

        ObjectArray SonTeklifGetir(KYM.Kullanici kullanan, Teklif bilgi);

        Sonuc HakedisKaydet(KYM.Kullanici kullanan, Tahsilat bilgi, int islemkod, int parcakod);
        ObjectArray HakedisListele(KYM.Kullanici kullanan, Tahsilat bilgi);
        Sonuc HakedisTaksitSil(KYM.Kullanici kullanan, TasinmazOdemePlan[] bilgi, int islemkod, int parcakod);

        Sonuc MakbuzFormKaydet(Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod);
        ObjectArray MakbuzFormListele(Kullanici kullanan, Makbuz bilgi);
        Sonuc MakbuzFormSil(Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod);
        Sonuc MakbuzFormDetaySil(Kullanici kullanan, MakbuzDetay bilgi, int islemkod, int parcakod);

        Sonuc DosyaKaydet(Kullanici kullanan, Dosya bilgi);
        Sonuc DosyaSil(Kullanici kullanan, Dosya bilgi);
        ObjectArray DosyaListele(Kullanici kullanan, Dosya bilgi);

        Sonuc MuhasebeIslemFormKaydet(Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod);
        Sonuc MuhasebeIslemFormSil(Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod);
        ObjectArray MuhasebeIslemFormListele(Kullanici kullanan, MuhasebeIslemForm bilgi);

        Sonuc IsKaydet(Kullanici kullanan, Is bilgi);
        Sonuc IsSil(Kullanici kullanan, Is bilgi);
        ObjectArray IsListele(Kullanici kullanan, Is bilgi);

        Sonuc KomisyonKarariVarmi(Kullanici kullanan, KomisyonKarar bilgi);


        Sonuc ParcaKaydet(Kullanici kullanan, TNS.GEL.Parca bilgi);
        Sonuc ParcaSil(Kullanici kullanan, TNS.GEL.Parca bilgi);
        ObjectArray ParcaListele(Kullanici kullanan, TNS.GEL.Parca bilgi);


        Sonuc IsParcaKaydet(Kullanici kullanan, IsParca bilgi);
        ObjectArray IsParcaListele(Kullanici kullanan, int isKod);

        Sonuc OrnekPersonelKaydet(Kullanici kullanan, OrnekPersonel bilgi);
        // ObjectArray OrnekListele(Kullanici kullanan, OrnekPersonel bilgi);


        Sonuc TasinmazCinsKaydet(Kullanici kullanan, TasinmazCins cins);
        Sonuc TasinmazCinsKaydet(Kullanici kullanan, TasinmazCins[] cinsler);
        Sonuc TasinmazCinsSil(Kullanici kullanan, string kod);
        ObjectArray TasinmazCinsListele(Kullanici kullanan, TasinmazCins cins);

        Sonuc TasinmazKaydet(Kullanici kullanan, Tasinmaz bilgi);
        Sonuc TasinmazSil(Kullanici kullanan, Tasinmaz bilgi);
        ObjectArray TasinmazListele(Kullanici kullanan, Tasinmaz tasinmaz);
        ObjectArray TasinmazIcmalListele(Kullanici kullanan, TasinmazIcmal icmal);


        Sonuc OdemePlaniKaydet(Kullanici kullanan, TasinmazOdemePlan[] bilgiler);
        Sonuc OdemePalniSil(Kullanici kullanan, TasinmazOdemePlan bilgi);
        ObjectArray OdemePalniListele(Kullanici kullanan, TasinmazOdemePlan bilgi);
        ObjectArray OdemePlaniFirmaKisiListele(Kullanici kullanan, string tasinmazSistemKod, int tasinmazYili);

        Sonuc TasinmazBolgeKaydet(Kullanici kullanan,Bolge bilgi);
        ObjectArray TasinmazBolgeListele(Kullanici kullanan, Bolge bilgi);
        Sonuc TasinmazBolgeSil(Kullanici kullanan, TNSCollection bilgiler);

        Sonuc TasinmazBolgeIlKaydet(Kullanici kullanan, Bolge bolge);
        Sonuc TasinmazBolgeIlSil(Kullanici kullanan, Bolge bolge);
        ObjectArray TasinmazBolgeIlListele(Kullanici kullanan, Bolge bolge);


        Sonuc TasinmazEkDosyaKaydet(Kullanici kullanan, TasinmazEkDosya dosya);
        Sonuc TasinmazEkDosyaSil(Kullanici kullanan, TasinmazEkDosya dosya);
        ObjectArray TasinmazEkDosyaListele(Kullanici kullanan, TasinmazEkDosya dosya);
    }
}
