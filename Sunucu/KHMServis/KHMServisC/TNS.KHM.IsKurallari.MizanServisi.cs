using System;
using System.Data;
using TNS.BKD;
using TNS.KHM.IsKurallari.Veri;
using TNS.KYM;
using TNS.UZY;
using TNS.Veri;
using TNS.BHMv2;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Mizan ile ilgili fonksiyonları yöneten sınıf
    /// </summary>
    public class KHMMizanServisi
    {
        string MizanEkKosulOlustur(string hesapKodu, out string aciklama, ref int aktif)
        {

            string kosul = "";
            aciklama = "";
            if (Convert.ToInt32(hesapKodu) >= 100 && Convert.ToInt32(hesapKodu) < 300 && aktif != 1)
            {
                aktif = 1;
                kosul = "HESKOD>=100 AND HESKOD<300";
                aciklama = "AKTİF HESAPLAR";
            }
            else if (Convert.ToInt32(hesapKodu) >= 300 && Convert.ToInt32(hesapKodu) < 600 && aktif != 2)
            {
                aktif = 2;
                kosul = "HESKOD>=300 AND hesapKodu<600";
                aciklama = "PASİF HESAPLAR";
            }
            else if (Convert.ToInt32(hesapKodu) >= 600 && aktif != 3)
            {
                aktif = 3;
                kosul = "HESKOD>=600";
                aciklama = "FAALİYET VE BÜTÇE HESAPLARI";
            }
            return kosul;
        }

        bool KonsolideAl(string hesapKodu, int aktif)
        {

            if (Convert.ToInt32(hesapKodu) >= 100 && Convert.ToInt32(hesapKodu) < 300)
                if (aktif != 1)
                    return true;
                else
                    return false;


            else if (Convert.ToInt32(hesapKodu) >= 300 && Convert.ToInt32(hesapKodu) < 600)
            {
                if (aktif != 2)
                    return true;
                else
                    return false;
            }
            else if (Convert.ToInt32(hesapKodu) >= 600)
            {
                if (aktif != 3)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        MizanBilgi DegerBul(VeriErisim veriErisim, int yil, string kurum, string heskod,bool gecici,string ekKosul)
        {
            MizanBilgi mizan = new MizanBilgi(); 
            double kalan=0.0;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            TABMizan islMizan = new TABMizan(ve);
            IDataReader dr = islMizan.RakamGetir(yil,kurum,heskod,gecici,ekKosul);
            if (dr != null)
            {
                while (dr.Read())
                {
                    mizan.borc = (double)VeriErisim.DegerAl(dr, 0,(double)0);
                    mizan.alacak = (double)VeriErisim.DegerAl(dr, 1, (double)0);
                    //mizan.borc_artik = (double)VeriErisim.DegerAl(dr, 2, (double)0);
                    //mizan.alacak_artik = (double)VeriErisim.DegerAl(dr, 3, (double)0);
                 
                    kalan=mizan.borc - mizan.alacak;
                    if (kalan > 0)
                        mizan.borc_artik = mizan.borc_artik +kalan ;
                    else
                        mizan.alacak_artik = mizan.alacak_artik - kalan;
                 
                               
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            return mizan;
        }

        OdenekBilgileriVAN GelirGerceklesmesiListele(VeriErisim veriErisim, int yil, int ay, Tertip kistasTertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            OdenekBilgileriVAN obv = new OdenekBilgileriVAN();
            obv.sonuc.islemSonuc = false;
            kistasTertip.NoktalariAt();
            kistasTertip.DiziKontrol();
            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma((string)kistasTertip.kodlar[0], kistasTertip.Kirilimlar(0)).Replace(".", "");

            TABGelir to = new TABGelir(ve);
            if (ay <= 0)
                ay = 12;
            IDataReader r = to.TertipListeleGerceklesme(yil, ay, kistasTertip.kodlar, "");
            if (to.Hata() == "")
            {
                OdenekBilgi ob = null;
                while (r.Read())
                {
                    ob = new OdenekBilgi(yil, ENUMTabloKod.TABGelirGerceklesmesi);
                    ob.tertip.kodlar[0] = VeriErisim.DegerAl(r, 0, "");
                    ob.tertip.kodlar[1] = VeriErisim.DegerAl(r, 1, "");

                    obv.detaylar.Add(ob);
                }
                r.Close();
                obv.sonuc.islemSonuc = true;
            }
            else
                obv.sonuc.hataStr = to.Hata();

            ve.VeriErisimKapat();

            return obv;
        }
        
        /// <summary>
        /// Kesin hesap mizan raporu hazırlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="gecici">Eğer <c>true</c> geçici Mizan alınır.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapMizanHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, bool gecici)
        {
            TNSCollection donusDeger=new TNSCollection();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM");
            int kirilimSay = 3;
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }
           
            if (kurum == "")
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));
                  
            else
            {
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kurum, tUz.Kirilimlar(0));

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan,kurum))
                {

                    TABMizan tabListe = new TABMizan(veListe);
                    TABMizan tabMizan = new TABMizan(ve);
                    IUZYServis uzS = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
                    IDataReader dr = tabListe.MizanListele(yil,kurum,gecici);
                  
                    if (dr != null)
                    {
                        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
                        TNSCollection kistaslar = new TNSCollection();
                        string[] eskiDegerler = new string[kirilimSay];
                        string[] yeniDegerler = new string[kirilimSay];
                       
                        string ekKosul = "";
                        string hesapKod = "";
                        int aktif = 0;                       
                        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";
                                           

                         while (dr.Read())
                        {
                            int yer = 0;                           
                            Tertip tertipListe=new Tertip(yil,ENUMTabloKod.TABGelir);                                                   
                            tertipListe.kodlar[0]="";
                            hesapKod=(VeriErisim.DegerAl(dr, 0, ""));
                           
                            for (int j = 1; j <= hesapKod.Length ; j++)
                            {
                                yeniDegerler[yer++] = hesapKod.Substring(0, j); ;
                               
                            }                            
                            
                            //int kodyer=0;
                           
                            string aciklamaKod = "";
                          
                            UcgenSatir us1 = new UcgenSatir();
                            if (KonsolideAl(hesapKod, aktif))
                            {
                                us1.ekDegerler.Add(DegerBul(ve, yil, kurum, "", gecici, MizanEkKosulOlustur(hesapKod, out us1.aciklama, ref aktif)));
                                donusDeger.Add(us1);
                            }

                            for (int j = 0; j < yer; j++)
                            {                                
                                //kistaslar[kodyer + 1] = (string)kistaslar[kodyer + 1] + (string)yeniDegerler[j];
                               
                                if (eskiDegerler[j] != yeniDegerler[j])
                                {
                                    UcgenSatir us = new UcgenSatir();
                                    us.yer = j;
                                    aciklamaKod = yeniDegerler[j];
                                    us.kodParca = yeniDegerler[j];
                                    us.ekDegerler.Add(DegerBul(ve, yil, kurum, aciklamaKod, gecici, ekKosul));
                                    us.aciklama = uzS.UzayDegeriStr(kullanan, "MUHHESAPPLANI", yil.ToString() + "-" + aciklamaKod, true, "ACIKLAMA");
                        
                                    donusDeger.Add(us);

                                    for (int k = j+1; k < yer; k++)
                                        eskiDegerler[k] = "";
                                }
                            }
                            for (int j = 0; j < yer; j++)
                            {
                                eskiDegerler[j] = yeniDegerler[j];
                            }                                
                         }                                                                             
                        
                        dr.Close();
                        UcgenSatir us2 = new UcgenSatir();
                        us2.aciklama = "GENEL TOPLAM";
                        us2.ekDegerler.Add(DegerBul(ve, yil, kurum, "", gecici, ""));
                        donusDeger.Add(us2);
                        //uzS.UzayKapat();

                    }
                 
                  }
                else
                   donusDeger.Add(new Sonuc(false, "Bu bilgileri görme yetkiniz yok", ""));
                  
            }

            veListe.VeriErisimKapat();
            ve.VeriErisimKapat();

            return donusDeger;
        }
    }
}
