using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using TNS.KYM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Sunucuda bulunan hata dosyaları ile ilgili işlemleri tutan sınıf.
    /// </summary>
    public class HataDosya
    {
        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasöründeki dosya bilgilerini ';' karakteri ile birleştirip döndüren yordam.
        /// Dosya bilgisi ise '|' karekteri ile ayrılmış değiştirilme zamanı, dosya büyüklüğü ve dosya adı bilgilerinden oluşmaktadır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <returns>"HataDosyasi" klasöründeki dosya bilgileri ';' ayracı ile döndürülür.</returns>
        public ObjectArray HataDosyaListele(Kullanici kullanan)
        {
            ObjectArray donenDeger = new ObjectArray();
            string filePath = ConfigurationManager.AppSettings.Get("HataDosyasi").Trim();
            filePath = Path.GetDirectoryName(filePath);//hatadosyası yolu

            string[] dosyalar = Directory.GetFiles(filePath);
            for (int i = 0; i < dosyalar.Length; i++)
            {
                string dosya = dosyalar[i];
                if (System.IO.File.Exists(dosya))
                {
                    EkliDosyalar ed = new EkliDosyalar();
                    FileInfo fi = new FileInfo(dosya);
                    ed.buyukluk = fi.Length;
                    ed.yuklemeTarihiDT = File.GetLastWriteTime(dosya);
                    ed.dosyaAdi = Path.GetFileName(dosya);
                    donenDeger.Ekle(ed);
                }
            }

            return donenDeger;
        }

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="dosyaAd">Silinecek dosyanın ismi.</param>
        public void HataDosyaSil(Kullanici kullanan, string dosyaAd)
        {
            string filePath = ConfigurationManager.AppSettings.Get("HataDosyasi").Trim();
            filePath = Path.GetDirectoryName(filePath);
            if (!filePath.EndsWith("\\"))
                filePath = filePath + "\\";
            dosyaAd = filePath + "\\" + dosyaAd;

            if (System.IO.File.Exists(dosyaAd))
                System.IO.File.Delete(dosyaAd);
        }

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı okuyan ve byte dizisi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="dosyaAd">İçeriği döndürülecek dosyanın ismi.</param>
        /// <returns>Verilen dosyanın içeriği byte dizisi olarak döndürülür.</returns>
        public byte[] HataDosyaVer(Kullanici kullanan, string dosyaAd)
        {
            byte[] donenDeger = { };
            string filePath = ConfigurationManager.AppSettings.Get("HataDosyasi").Trim();
            filePath = Path.GetDirectoryName(filePath);
            if (!filePath.EndsWith("\\"))
                filePath = filePath + "\\";
            dosyaAd = filePath + "\\" + dosyaAd;

            if (System.IO.File.Exists(dosyaAd))
            {
                FileInfo fInfo = new FileInfo(dosyaAd);
                long numBytes = fInfo.Length;
                double dLen = Convert.ToDouble(fInfo.Length / 1000000);
                if (dLen < 4)
                {
                    FileStream fStream = new FileStream(dosyaAd, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fStream);

                    donenDeger = br.ReadBytes((int)numBytes);
                    br.Close();
                    fStream.Close();
                    fStream.Dispose();
                }
            }

            return donenDeger;
        }
    }
}
