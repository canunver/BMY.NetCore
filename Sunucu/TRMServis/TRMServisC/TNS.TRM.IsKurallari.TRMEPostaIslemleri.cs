using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using Aspose.Email.Pop3;
using System;
using System.IO;
using Aspose.Email.Mail;
using Aspose.Email.Imap;

namespace TNS.TRM.IsKurallari
{
    public class TRMEPostaIslemleri
    {
        //Pop3Client clientPOP3 = null;
        ImapClient clientIMAP = null;

        #region IMAP
        public void IMAPMailleriListele()
        {
            OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": Mailler kontrol ediliyor", false);
            try
            {
                ImapClient client = IMAPClientOlustur();
                client.SelectFolder(ImapFolderInfo.InBox);

                ImapQueryBuilder builder = new ImapQueryBuilder();
                builder.HasNoFlags(ImapMessageFlags.IsRead);
                Aspose.Email.MailQuery query = builder.GetQuery();

                ImapMessageInfoCollection msgCollection = client.ListMessages(query);

                for (int i = 0; i < msgCollection.Count; i++)
                {
                    string uniqueId = msgCollection[i].UniqueId;
                    try
                    {
                        OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": " + uniqueId + " nolu mail alınıyor", false);
                        IMAPMailIcerik(client, uniqueId, msgCollection[i]);
                        OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": " + uniqueId + " nolu mail alındı", false);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = DateTime.Now.ToString() + " - " + "Mail alma bağlantı hatası - " + uniqueId;
                        errorMessage += "\r\n" + "Message: " + ex.Message.ToString();
                        errorMessage += "\r\n" + "Stack Trace: " + ex.StackTrace.ToString();
                        errorMessage += "\r\n";

                        OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": " + errorMessage, false);
                    }
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                string errorMessage = DateTime.Now.ToString() + " - " + "Mail bağlantı hatası";
                errorMessage += "\r\n" + "Message: " + ex.Message.ToString();
                errorMessage += "\r\n" + "Stack Trace: " + ex.StackTrace.ToString();

                OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": " + errorMessage, false);
            }

            OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", DateTime.Now.ToLongTimeString() + ": Mailler kontrol edildi", false);
        }

        public void IMAPMailIcerik(ImapClient client, string uniqueId, ImapMessageInfo mailInfo)
        {
            string kimdenAdres = mailInfo.From.Address;
            string kimden = mailInfo.From.DisplayName + " (" + kimdenAdres + ")";

            string yatirimProjeNo = YatirimProjeNumarasiBul(kimdenAdres);
            if (yatirimProjeNo == "") return;

            MailMessage msg = client.FetchMessage(uniqueId);
            string tarih = new TNSDateTime(msg.Date).ToLongString();
            //string kimden = msg.From[0].DisplayName + " (" + msg.From[0].Address + ")";
            //string kimdenAdres = msg.From[0].Address;

            //Subject
            string konu = "";
            if (msg.Subject != null) konu = msg.Subject;

            //Body
            string icerik = "";
            if (msg.Body != null) icerik = msg.HtmlBody;

            //To
            string kime = "";
            foreach (MailAddress address in msg.To)
            {
                if (kime != "") kime += ", ";
                kime += "<b>" + address.DisplayName + "</b>" + " (" + address.Address + ")";
            }
            //CC
            string cc = "";
            foreach (MailAddress address in msg.CC)
            {
                if (cc != "") cc += ", ";
                cc += "<b>" + address.DisplayName + "</b>" + " (" + address.Address + ")";
            }

            string hedefKlasor = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaDosyaYol");
            hedefKlasor = Path.Combine(hedefKlasor, yatirimProjeNo);

            for (int i = 0; i < msg.Attachments.Count; i++)
            {
                string uzanti = Path.GetExtension(msg.Attachments[i].Name.ToString());
                string dosyaAdi = Guid.NewGuid().ToString() + uzanti; //msg.Attachments[i].Name.ToString();
                if (!Directory.Exists(hedefKlasor))
                    Directory.CreateDirectory(hedefKlasor);

                msg.Attachments[i].Save(Path.Combine(hedefKlasor, dosyaAdi));
                File.SetCreationTime(Path.Combine(hedefKlasor, dosyaAdi), msg.Date);
            }

            string hedefKlasorMail = hedefKlasor + "\\mail";
            if (!Directory.Exists(hedefKlasorMail))
                Directory.CreateDirectory(hedefKlasorMail);
            client.SaveMessage(uniqueId, Path.Combine(hedefKlasorMail, uniqueId));


            client.ChangeMessageFlags(uniqueId, Aspose.Email.Imap.ImapMessageFlags.IsRead);
        }

        private ImapClient IMAPClientOlustur()
        {
            if (clientIMAP != null)
                return clientIMAP;

            clientIMAP = new ImapClient();
            clientIMAP.Host = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuURL");
            clientIMAP.Port = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuPort"), 0);
            clientIMAP.Username = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuKullanici");
            clientIMAP.Password = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuParola");
            if (System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuSSL") == "1")
            {
                clientIMAP.EnableSsl = true;
                clientIMAP.SecurityMode = ImapSslSecurityMode.Implicit;
            }
            else
                clientIMAP.EnableSsl = false;

            return clientIMAP;


        }

        private string YatirimProjeNumarasiBul(string mailAdresi)
        {
            Goruntu goruntu = new Goruntu();
            goruntu.ePosta = mailAdresi;
            if (string.IsNullOrEmpty(goruntu.ePosta)) return "";

            ObjectArray liste = TRMHibeYatirim.ListeleGoruntu(null, goruntu);
            foreach (Goruntu g in liste.objeler)
            {
                return g.basvuruNo;
            }

            return "";
        }
        #endregion

        /*
        #region POP3
        public void Pop3MailleriListele()
        {
            try
            {
                Pop3Client client = Pop3ClientOlustur();

                Pop3MessageInfoCollection msgCollection = client.ListMessages();

                for (int i = 0; i < msgCollection.Count; i++)
                {
                    string uniqueId = msgCollection[i].UniqueId;
                    try
                    {
                        Pop3MailIcerik(client, uniqueId);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = DateTime.Now.ToString() + " - " + "Mail alma bağlantı hatası - " + uniqueId;
                        errorMessage += "\r\n" + "Message: " + ex.Message.ToString();
                        errorMessage += "\r\n" + "Stack Trace: " + ex.StackTrace.ToString();
                        errorMessage += "\r\n";

                        OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", errorMessage, false);
                    }
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                string errorMessage = DateTime.Now.ToString() + " - " + "Mail bağlantı hatası";
                errorMessage += "\r\n" + "Message: " + ex.Message.ToString();
                errorMessage += "\r\n" + "Stack Trace: " + ex.StackTrace.ToString();

                OrtakFonksiyonlar.HataDosyaYaz("YatirimEPostaIslemi.txt", errorMessage, false);
            }
        }

        public void Pop3MailIcerik(Pop3Client client, string uniqueId)
        {
            MailMessage msg = client.FetchMessage(uniqueId);

            string tarih = new TNSDateTime(msg.Date).ToLongString();
            string kimden = msg.From[0].DisplayName + " (" + msg.From[0].Address + ")";
            string kimdenAdres = msg.From[0].Address;

            //yatırımcı bulunacak
            if (kimdenAdres != "bilgi@guncelguvenlik.com.tr") return;

            //Subject
            string konu = "";
            if (msg.Subject != null) konu = msg.Subject;

            //Body
            string icerik = "";
            if (msg.Body != null) icerik = msg.HtmlBody;

            //To
            string kime = "";
            foreach (MailAddress address in msg.To)
            {
                if (kime != "") kime += ", ";
                kime += "<b>" + address.DisplayName + "</b>" + " (" + address.Address + ")";
            }
            //CC
            string cc = "";
            foreach (MailAddress address in msg.CC)
            {
                if (cc != "") cc += ", ";
                cc += "<b>" + address.DisplayName + "</b>" + " (" + address.Address + ")";
            }

            string yatirimProjeNo = YatirimProjeNumarasiBul(kimden);
            string hedefKlasor = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaDosyaYol");
            hedefKlasor = Path.Combine(hedefKlasor, yatirimProjeNo);

            for (int i = 0; i < msg.Attachments.Count; i++)
            {
                string uzanti = Path.GetExtension(msg.Attachments[i].Name.ToString());
                string dosyaAdi = Guid.NewGuid().ToString() + uzanti; //msg.Attachments[i].Name.ToString();
                if (!Directory.Exists(hedefKlasor))
                    Directory.CreateDirectory(hedefKlasor);

                msg.Attachments[i].Save(Path.Combine(hedefKlasor, dosyaAdi));
            }

            msg.Save(Path.Combine(hedefKlasor, uniqueId));
        }

        private Pop3Client Pop3ClientOlustur()
        {
            if (clientPOP3 != null)
                return clientPOP3;

            clientPOP3 = new Pop3Client();
            clientPOP3.Host = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuURL");
            clientPOP3.Port = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuPort"), 0);
            clientPOP3.Username = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuKullanici");
            clientPOP3.Password = System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuParola");
            if (System.Configuration.ConfigurationManager.AppSettings.Get("HibeDestekPostaSunucusuSSL") == "1")
            {
                clientPOP3.EnableSsl = true;
                clientPOP3.SecurityMode = Pop3SslSecurityMode.Implicit;
            }
            else
                clientPOP3.EnableSsl = false;

            return clientPOP3;
        }
        #endregion
        */
    }
}
