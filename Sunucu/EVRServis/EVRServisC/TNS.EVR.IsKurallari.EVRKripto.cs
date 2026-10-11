using System.Diagnostics;
using System.IO;
using EVRServisI;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using StratekCrypto;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak sisteminde bulunan dosyaların Kripto yordamlarını tutan sınıf.
    /// </summary>
    public class EVRKripto
    {
        /// <summary>
        /// Kripto işleminden geçirilecek olan dosyayı işler
        /// </summary>
        /// <returns>string değerinde sonuc bilgisi</returns>
        public static string Kriptola()
        {
            string hataMsg = "";
            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();

            EvrakDosya form = new EvrakDosya();
            form.kriptoYapildi = (int)ENUMEvrakDosyaKriptoIslem.YAPILACAK;
            form.ocrYapildi = (int)ENUMEvrakDosyaOCRIslem.ISLEMIBITENLER;

            EvrakDosya[] dosyaListe = servisEVR.EvrakDosyaListele(new Kullanici(), form, 5000, true);
            if (dosyaListe != null && dosyaListe.Length > 0)
            {
                //15 dk çalıştıktan sonra dursun
                Stopwatch watch = new Stopwatch();
                watch.Start();

                foreach (EvrakDosya dosya in dosyaListe)
                {
                    if (watch.Elapsed.Minutes >= 15)
                        break;

                    form = new EvrakDosya();
                    string islemYapilacakDosya = dosya.dosyaYolu + "\\" + dosya.dosyaAdi;
                    FileInfo fi = new FileInfo(islemYapilacakDosya);
                    if (fi.Exists)
                    {
                        try
                        {
                            Facade f = new Facade();
                            f.Password = "28bdb479-f804-4ea7-b0bc-531d4ece6c85";
                            f.EncryptFileByInternalPublicKey(islemYapilacakDosya, islemYapilacakDosya);
                        }
                        catch (Exception)
                        {
                            OrtakFonksiyonlar.HataStrYaz("Kripto işleminde hata oluştu.EvrEvrakno:" + dosya.evrEvrakNo + "_" + dosya.siraNo + "_" + islemYapilacakDosya);
                        }

                        form.kriptoYapildi = (int)ENUMEvrakDosyaKriptoIslem.YAPILDI;
                        form.evrEvrakNo = dosya.evrEvrakNo;
                        form.siraNo = dosya.siraNo;

                        Sonuc sonuc = servisEVR.EvrakDosyaOCRGuncelle(new Kullanici(), form);
                        if (!sonuc.islemSonuc)
                        {
                            hataMsg = sonuc.hataStr;
                            break;
                        }
                    }
                }
                watch.Stop();
            }
            return hataMsg;
        }


        /// <summary>
        /// İçerik alma işleminden geçirilecek olan dosyayı işler
        /// </summary>
        /// <returns>string değerinde sonuc bilgisi</returns>
        public static string DosyaIcerikKaydet()
        {
            string hataMsg = "";
            IEVRServis servisEVR = TNS.EVR.Arac.Tanimla();

            EvrakDosya form = new EvrakDosya();
            form.icerikYapildi = (int)ENUMEvrakDosyaIcerikIslem.YAPILACAK;

            EvrakDosya[] dosyaListe = servisEVR.EvrakDosyaListele(new Kullanici(), form, 20000, true);

            if (dosyaListe != null && dosyaListe.Length > 0)
            {
                new Aspose.Words.License().SetLicense("Aspose.Total.lic");
                new Aspose.Cells.License().SetLicense("Aspose.Total.lic");
                new Aspose.Pdf.License().SetLicense("Aspose.Total.lic");

                //15 dk çalıştıktan sonra dursun
                Stopwatch watch = new Stopwatch();
                watch.Start();

                foreach (EvrakDosya dosya in dosyaListe)
                {
                    if (watch.Elapsed.Minutes >= 15)
                        break;

                    form = new EvrakDosya();
                    string islemYapilacakDosya = Path.Combine(dosya.dosyaYolu, dosya.dosyaAdi);


                    FileInfo fi = null;

                    try
                    {
                        fi = new FileInfo(islemYapilacakDosya);
                    }
                    catch (Exception ex)
                    {
                        fi = null;
                        OrtakFonksiyonlar.HataStrYaz("{CEA3BE29-B54B-4998-BF82-8A208CD712XY} File Info Hatası Dosya Adı: " + islemYapilacakDosya + " Hata:" + ex.Message + " StackTrace: " + ex.StackTrace);
                    }

                    if (fi == null) continue;

                    bool islemYapilacak = false;
                    if (fi.Exists)
                    {
                        string tmpFileName = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

                        try
                        {
                            StratekCrypto.Facade f = new StratekCrypto.Facade();
                            f.Password = "28bdb479-f804-4ea7-b0bc-531d4ece6c85";
                            f.DecryptFileByInternalPrivateKey(fi.FullName, tmpFileName);
                        }
                        catch (Exception)
                        {
                            OrtakFonksiyonlar.HataStrYaz("İçerik Alma işleminde hata oluştu.EvrEvrakno:" + dosya.evrEvrakNo + "_" + dosya.siraNo + "_" + fi.FullName);
                        }

                        form.icerik = "";

                        if (fi.Extension.ToLower().StartsWith(".doc"))
                        {
                            try
                            {
                                Aspose.Words.Document doc = new Aspose.Words.Document(tmpFileName);
                                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                                builder.InsertField("MERGEFIELD Field");
                                form.icerik = doc.ToString(Aspose.Words.SaveFormat.Text);
                            }
                            catch (Exception e)
                            {
                                OrtakFonksiyonlar.HataStrYaz("Doc içerik alma işleminde hata oluştu.EvrEvrakno:" + form.evrEvrakNo + " ** " + e.Message + e.StackTrace);
                                form.icerik = "";
                            }

                            islemYapilacak = true;
                        }
                        else if (fi.Extension.ToLower().StartsWith(".pdf"))
                        {
                            try
                            {

                                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(tmpFileName);
                                Aspose.Pdf.Text.TextAbsorber textAbsorber = new Aspose.Pdf.Text.TextAbsorber();
                                pdfDocument.Pages.Accept(textAbsorber);
                                form.icerik = textAbsorber.Text;
                                pdfDocument.Dispose();
                            }
                            catch (Exception e)
                            {
                                OrtakFonksiyonlar.HataStrYaz("PDF içerik alma işleminde hata oluştu.EvrEvrakno:" + form.evrEvrakNo + " ** " + e.Message + e.StackTrace);
                                form.icerik = "";
                            }

                            islemYapilacak = true;
                        }
                        else if (fi.Extension.ToLower().StartsWith(".xls"))
                        {
                            islemYapilacak = true;
                        }
                        else if (fi.Extension.ToLower().StartsWith(".ppt"))
                        {
                            islemYapilacak = true;
                        }
                        else if (fi.Extension.ToLower().StartsWith(".txt"))
                        {
                            islemYapilacak = true;
                        }

                        try
                        {
                            File.Delete(tmpFileName);
                        }
                        catch (Exception e)
                        {
                            OrtakFonksiyonlar.HataStrYaz("Dosya silme işleminde hata oluştu.EvrEvrakno:" + e.Message);
                        }
                    }
                    Sonuc sonuc = new Sonuc();

                    form.icerikYapildi = (int)ENUMEvrakDosyaIcerikIslem.YAPILDI;
                    form.icerikTarihi = new TNSDateTime(DateTime.Now);
                    form.icerikGuid = dosya.icerikGuid;
                    form.evrEvrakNo = dosya.evrEvrakNo;
                    form.siraNo = dosya.siraNo;
                    form.dosyaAdi = dosya.dosyaAdi;
                    form.dosyaYolu = dosya.dosyaYolu;

                    if (islemYapilacak)
                    {
                        //icerik tablosuna yaz
                        sonuc = EVROCR.EvrakDosyaIcerikKaydet(null, new Kullanici(), form);

                        if (sonuc.islemSonuc)
                        {
                            form.icerikGuid = sonuc.anahtar;
                            sonuc = EVROCR.EvrakDosyaGuncelle(null, new Kullanici(), form);
                        }
                    }
                    else
                    {
                        form.icerikYapildi = (int)ENUMEvrakDosyaIcerikIslem.YAPILMAYACAK;
                        form.icerikGuid = "";
                        sonuc = EVROCR.EvrakDosyaGuncelle(null, new Kullanici(), form);
                    }

                    if (!sonuc.islemSonuc)
                    {
                        hataMsg = sonuc.hataStr;
                        OrtakFonksiyonlar.HataStrYaz("Gunceleme Hatası " + sonuc.hataStr);
                        break;
                    }
                }

                watch.Stop();
            }
            return hataMsg;
        }
    }
}
