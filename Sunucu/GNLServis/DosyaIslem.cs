using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Renci.SshNet.Common;

namespace TNS
{
    public class FTPIslem
    {
        public enum DI_TUR : int
        {
            FTPYOK = 99,
            SFTP = 2,
            FTP = 1,
            DIRECTORY = 0
        }

        public DI_TUR tur = DI_TUR.DIRECTORY;
        string ftpYol;
        string dosyaYol;
        string user;
        string password;
        bool ssl = false;
        int portNo = 22;
        string yazDir = "";
        string okuDir = "";

        public FTPIslem(string ftpYol)
        {
            this.ftpYol = ftpYol;

            string dosyaTur = OrtakFonksiyonlar.WebConfigOku(ftpYol + ".tur", "0");
            if (dosyaTur == "99") tur = DI_TUR.FTPYOK;
            else if (dosyaTur == "1") tur = DI_TUR.FTP;
            else if (dosyaTur == "2") tur = DI_TUR.SFTP;
            else tur = DI_TUR.DIRECTORY;

            if (tur != DI_TUR.FTPYOK)
            {
                dosyaYol = OrtakFonksiyonlar.WebConfigOku(ftpYol, "");
                if (string.IsNullOrEmpty(dosyaYol))
                    throw new Exception(string.Format("Dosya yol ismi bulunamadı:{0}", ftpYol));
                if (tur == DI_TUR.FTP || tur == DI_TUR.SFTP)
                {
                    user = OrtakFonksiyonlar.WebConfigOku(ftpYol + ".kullanici", "can");
                    password = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(OrtakFonksiyonlar.WebConfigOku(ftpYol + ".parola", "1"));
                    ssl = OrtakFonksiyonlar.WebConfigOku(ftpYol + ".ssl", "0") == "1";
                    yazDir = OrtakFonksiyonlar.WebConfigOku(ftpYol + ".yaz", "");
                    okuDir = OrtakFonksiyonlar.WebConfigOku(ftpYol + ".oku", "");
                    portNo = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.WebConfigOku(ftpYol + ".port", "22"), 22);
                }
            }
        }

        public Sonuc FTPdenDosyaya(string ftpDosyaAd, string hedefDosyaAd)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            try
            {
                if (tur == DI_TUR.DIRECTORY)
                {
                    string dosyaAd = dosyaYol + "\\" + ftpDosyaAd;
                    System.IO.File.Copy(dosyaAd, hedefDosyaAd, true);
                }
                else if (tur == DI_TUR.FTP)
                {
                    // Get the object used to communicate with the server.
                    FtpWebRequest ftpReq = (FtpWebRequest)FtpWebRequest.Create(dosyaYol + "/" + ftpDosyaAd);
                    ftpReq.Method = WebRequestMethods.Ftp.DownloadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    ftpReq.Credentials = new NetworkCredential(user, password);

                    FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();

                    System.IO.Stream responseStream = response.GetResponseStream();
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(responseStream);

                    //reader.ReadToEnd();

                    reader.Close();
                    response.Close();
                }
                else if (tur == DI_TUR.SFTP)
                {
                    Renci.SshNet.SftpClient sftp = new Renci.SshNet.SftpClient(dosyaYol, portNo, user, password);
                    sftp.Connect();
                    try
                    {
                        sftp.ChangeDirectory(okuDir);
                    }
                    catch { }
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(hedefDosyaAd, false, Encoding.UTF8);
                    bool download = false;
                    try
                    {
                        sftp.DownloadFile(ftpDosyaAd, sw.BaseStream);
                        download = true;
                    }
                    catch (Exception ex) { sonuc.DegerAta(ex.Message); }
                    sw.Flush();
                    sw.Close();
                    if (!download)
                    {
                        if (sonuc.islemSonuc)
                        {
                            sonuc.DegerAta("Dosya oluşturulamadı!");
                        }
                        System.IO.File.Delete(hedefDosyaAd);
                    }
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                string hata = dosyaYol + "::" + portNo.ToString() + "::" + user + "::" + ex.Message + ex.StackTrace + ex.Source;
                OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", hata, false);
                sonuc.DegerAta(ex.Message);
            }
            return sonuc;
        }

        public void DosyadanFTPye(string kaynakDosyaAd, string ftpDosyaAd)
        {
            DosyadanFTPye(kaynakDosyaAd, ftpDosyaAd, false);
        }

        public bool DosyaVarMi(string ftpDosyaAd)
        {
            try
            {
                if (tur == DI_TUR.DIRECTORY)
                {
                    string dosyaAd = dosyaYol + "\\" + ftpDosyaAd;
                    bool sonuc = System.IO.File.Exists(dosyaAd);
                    return sonuc;
                }
                else if (tur == DI_TUR.FTP)
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    FtpWebRequest ftpReq = (FtpWebRequest)FtpWebRequest.Create(dosyaYol + "/" + ftpDosyaAd);
                    if (ssl)
                        ftpReq.EnableSsl = true;

                    ftpReq.Credentials = new NetworkCredential(user, password);

                    ftpReq.KeepAlive = true;
                    ftpReq.UsePassive = false;

                    ftpReq.Method = WebRequestMethods.Ftp.GetFileSize;
                    ftpReq.UseBinary = true;

                    //SSL Certificate
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                    bool sonuc = true;
                    try
                    {
                        FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();
                        response.Close();
                        sonuc = true;
                    }
                    catch (WebException ex)
                    {
                        FtpWebResponse response = (FtpWebResponse)ex.Response;
                        if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            sonuc = false;
                        }
                        else
                        {
                            sonuc = false;
                        }
                    }
                    return sonuc;
                }
                else if (tur == DI_TUR.SFTP)
                {
                    return false; //sftp.Exists(ftpDosyaAd) metodu Dosya olmamasına rağmen true döndürüyor.
                    //bool sonuc = true;
                    //Renci.SshNet.SftpClient sftp = null;
                    //Exception ex = null;
                    //try
                    //{
                    //    sftp = new Renci.SshNet.SftpClient(dosyaYol, portNo, user, password);
                    //    sftp.Connect();
                    //    sftp.ChangeDirectory(yazDir);
                    //    sonuc = sftp.Exists(ftpDosyaAd);//Dosya yok ama true dönüyor
                    //    //OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", "6::" + sonuc.ToString() + "::" + ftpDosyaAd, false);
                    //}
                    //catch (Exception exIc)
                    //{
                    //    sonuc = false;
                    //    ex = exIc;
                    //}
                    //if (sftp != null)
                    //{
                    //    sftp.Disconnect();
                    //}
                    //if (ex != null) throw ex;
                    //return sonuc;
                }
            }
            catch (Exception ex)
            {
                string hata = dosyaYol + "::" + portNo.ToString() + "::" + DateTime.Now.ToShortDateString() + "::" + DateTime.Now.ToShortTimeString() + "::" + user + "::" + ex.Message + ex.StackTrace + ex.Source;
                OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", hata, false);
                throw ex;
            }
            return false;
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void DosyadanFTPye(string kaynakDosyaAd, string ftpDosyaAd, bool dosyaSil)
        {
            try
            {
                if (tur == DI_TUR.DIRECTORY)
                {
                    string dosyaAd = dosyaYol + "\\" + ftpDosyaAd;
                    System.IO.File.Copy(kaynakDosyaAd, dosyaAd, true);
                }
                else if (tur == DI_TUR.FTP)
                {
                    //System.Net.ServicePointManager.Expect100Continue = false;
                    FtpWebRequest ftpReq = (FtpWebRequest)FtpWebRequest.Create(dosyaYol + "/" + ftpDosyaAd);
                    if (ssl)
                        ftpReq.EnableSsl = true;

                    ftpReq.Credentials = new NetworkCredential(user, password);

                    ftpReq.KeepAlive = true;
                    ftpReq.UsePassive = false;

                    ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpReq.UseBinary = true;

                    //SSL Certificate
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                    System.IO.StreamReader sourceStream = new System.IO.StreamReader(kaynakDosyaAd);
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    ftpReq.ContentLength = fileContents.Length;

                    System.IO.Stream requestStream = ftpReq.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    FtpWebResponse response = (FtpWebResponse)ftpReq.GetResponse();
                    response.Close();
                }
                else if (tur == DI_TUR.SFTP)
                {
                    System.IO.StreamReader sourceStream = new System.IO.StreamReader(kaynakDosyaAd);
                    Renci.SshNet.SftpClient sftp = null;

                    try
                    {
                        //Renci.SshNet.PrivateKeyFile ObjPrivateKey = new Renci.SshNet.PrivateKeyFile(keyStream);
                        //if (this.ftpYol == "halkbankftp")
                        //{
                        //Renci.SshNet.KeyboardInteractiveAuthenticationMethod kauth = new Renci.SshNet.KeyboardInteractiveAuthenticationMethod(user);
                        //kauth.AuthenticationPrompt += (sender, e) => HandleKeyEvent(sender, e, password);
                        //Renci.SshNet.PasswordAuthenticationMethod pauth = new Renci.SshNet.PasswordAuthenticationMethod(user, password);

                        //Renci.SshNet.ConnectionInfo connectionInfo = new Renci.SshNet.ConnectionInfo(dosyaYol, portNo, user, p); //, pauth, kauth
                        //connectionInfo.RetryAttempts = 25;
                        //sftp = new Renci.SshNet.SftpClient(connectionInfo);

                        //}
                        //foreach (var d in sftp.ConnectionInfo.Encryptions.Where(p => p.Key != "aes128-cbc").ToList())
                        //{
                        //    sftp.ConnectionInfo.Encryptions.Remove(d.Key);
                        //}

                        sftp = new Renci.SshNet.SftpClient(dosyaYol, portNo, user, password);
                        sftp.HostKeyReceived += (sender, eventArgs) =>
                        {
                            eventArgs.CanTrust = true;
                        };

                        sftp.Connect();
                        OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", "4" + yazDir, false);
                        sftp.ChangeDirectory(yazDir);
                        OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", "5", false);
                        sftp.UploadFile(sourceStream.BaseStream, ftpDosyaAd, true);
                        OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", "6", false);
                        sftp.Disconnect();
                        OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", "7", false);
                    }
                    catch (Exception exIc)
                    {
                        if (sftp != null) sftp.Disconnect();
                        sourceStream.Close();
                        throw new Exception(this.ftpYol + " (" + this.tur + ") bağlantısına dosya (" + kaynakDosyaAd + ") gönderilirlen" + " hata oldu. Oluşan hata: " + exIc.Message);
                    }
                    sourceStream.Close();
                }
            }
            catch (Exception ex)
            {
                string hata = dosyaYol + "::" + portNo.ToString() + "::" + user + "::" + ex.Message + ex.StackTrace + ex.Source;
                OrtakFonksiyonlar.HataDosyaYaz("ftpexc.txt", hata + " (" + portNo + "::" + user + ":" + password + ")", false);
                throw ex;
            }
            if (dosyaSil)
                System.IO.File.Delete(kaynakDosyaAd);
        }

        private static void HandleKeyEvent(Object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e, string password)
        {
            foreach (Renci.SshNet.Common.AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = password;
                }
            }
        }

        public TNSDateTime FTPDosyaZamani(string ftpDosyaAd)
        {
            try
            {
                if (tur == DI_TUR.DIRECTORY)
                {
                    string dosyaAd = dosyaYol + "\\" + ftpDosyaAd;
                    if (System.IO.File.Exists(dosyaAd))
                        return new TNS.TNSDateTime(System.IO.File.GetCreationTime(dosyaAd));
                }
            }
            catch
            {
            }
            return null;
        }
    }

    public class TxtDosyaAlanTanim
    {
        public int alanYer;
        public string defDeger = null;
    }

    public class TxtDosyaSatir
    {
        public string[] parcalar { get; set; }
        public long dosyaSatirNo;
        private System.Collections.Hashtable alanTanimlari = null;

        public int ParcaInt(string parcaAd)
        {
            return OrtakFonksiyonlar.ConvertToInt(ParcaStr(parcaAd), 0);
        }

        public string ParcaStr(string parcaAd)
        {
            TxtDosyaAlanTanim at = (TxtDosyaAlanTanim)alanTanimlari[parcaAd];
            if (at == null) return null;
            if (at.alanYer >= 0 && at.alanYer < parcalar.Length) return NullYokEt(parcalar[at.alanYer]);
            return at.defDeger;
        }

        public string ParcaStr(int parcaNo)
        {
            if (parcaNo >= 0 && parcaNo < parcalar.Length) return NullYokEt(parcalar[parcaNo]);
            return null;
        }

        private string NullYokEt(string p)
        {
            if (p == "NULL") return null;
            return p;
        }

        public double ParcaDbl(string parcaAd)
        {
            string dlbS = ParcaStr(parcaAd);
            if (!string.IsNullOrWhiteSpace(dlbS)) dlbS = dlbS.Replace(".", ",");
            return OrtakFonksiyonlar.ConvertToDbl(dlbS);
        }

        public TNSDateTime ParcaDT(string parcaAd)
        {
            return new TNSDateTime(ParcaStr(parcaAd));
        }

        public decimal ParcaDec(string parcaAd)
        {
            return OrtakFonksiyonlar.ConvertToDecimal(ParcaStr(parcaAd));
        }

        public TxtDosyaSatir(System.Collections.Hashtable alanTanimlari)
        {
            this.alanTanimlari = alanTanimlari;
        }
    }

    public class TxtDosya
    {
        System.IO.FileInfo f = null;
        System.IO.StreamReader kaynakDosya = null;
        public long okunanBuyukluk = 0;
        public long dosyaSatirNo = 0;
        char ayrac;
        public long DosyaBuyukluk { get { return f.Length; } }
        private System.Collections.Hashtable alanTanimlari;

        public TxtDosya(string dosyaYol, char ayrac, System.Collections.Hashtable alanTanimlari)
        {
            f = new System.IO.FileInfo(dosyaYol);
            kaynakDosya = new System.IO.StreamReader(dosyaYol, Encoding.Default, false, 1000);
            this.ayrac = ayrac;
            this.alanTanimlari = alanTanimlari;
        }

        public TxtDosyaSatir SatirOku()
        {
            string s = kaynakDosya.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            okunanBuyukluk += (s.Length - 1);
            TxtDosyaSatir sat = new TxtDosyaSatir(alanTanimlari);
            sat.parcalar = s.Split(ayrac);
            sat.dosyaSatirNo = dosyaSatirNo++;
            return sat;
        }

        public void Kapat()
        {
            kaynakDosya.Close();
        }
    }
}
