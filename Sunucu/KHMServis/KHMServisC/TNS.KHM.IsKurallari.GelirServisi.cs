using System.Data;
using TNS.BKD;
using TNS.KHM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Gelir ile ilgili fonksiyonları yöneten sınıf
    /// </summary>
    public class KHMGelirServisi
    {
        private void GelirBilgileriHesapla(Kullanici kullanan, VeriErisim veriErisim, int yil, int ay, string[] kodlar, string ekKosul, out double tahmin, out double artik, out double yiliTahakkuk, out double netTahakkuk, out double devirArtik, out double rediade, out double netTahsilat)
        {
            double yiliTahakkuk1 = 0.0;
            double yiliTahakkuk2 = 0.0;
            tahmin = 0.0;
            artik = 0.0;
            yiliTahakkuk = 0.0;
            rediade = 0.0;
            netTahakkuk = 0.0;
            netTahsilat = 0.0;
            devirArtik = 0.0;

            string heskodlar = "", hesapKosul = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGelir tabGelir = new TABGelir(ve);
            TABGelirTahmini tabGelirTahmini = new TABGelirTahmini(ve);
            tahmin = tabGelirTahmini.GelirDegerBul(yil, 4, kodlar, ekKosul);
            TahakkukRedIadeBul(ve, yil, ay, kodlar, ekKosul, out yiliTahakkuk1, out yiliTahakkuk2, out rediade);
            heskodlar = "120;121;122;222";
            hesapKosul = GenelKosul.HesapKoduEkKosulOlustur(heskodlar, "HESKOD");
            if (ekKosul != "")
                ekKosul = ekKosul + " AND " + hesapKosul;
            artik = tabGelir.KHGelirGerceklesmesiDegerBul(yil - 1, 12, kodlar, ekKosul);
            yiliTahakkuk = yiliTahakkuk1 + yiliTahakkuk2 - artik;
            netTahakkuk = yiliTahakkuk - rediade;
            netTahsilat = yiliTahakkuk2 - rediade;

            ve.VeriErisimKapat();
        }

        private void TahakkukRedIadeBul(VeriErisim veriErisim, int yil, int ay, string[] kodlar, string ekKosul, out double yiliTahakkuk1, out double yiliTahakkuk2, out double rediade)
        {
            yiliTahakkuk1 = 0.0;
            yiliTahakkuk2 = 0.0;
            rediade = 0.0;
            double deger;
            string heskod = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            TABGelir islGelir = new TABGelir(ve);
            IDataReader dr = islGelir.KHGelirGerceklesmesiListeDegerBul(yil, ay, kodlar, ekKosul);
            if (dr != null)
            {
                while (dr.Read())
                {
                    heskod = (string)VeriErisim.DegerAl(dr, 0, "");
                    deger = (double)VeriErisim.DegerAl(dr, 1, (double)0);

                    if (heskod.StartsWith("120") || heskod.StartsWith("121") || heskod.StartsWith("122") || heskod.StartsWith("222"))
                         
                        {
                            if (deger > 0)
                                yiliTahakkuk1 = yiliTahakkuk1 + deger;
                            else
                                yiliTahakkuk1 = yiliTahakkuk1 - deger;                               
                        }
                     if (heskod.StartsWith("800"))                      
                    
                        if (yil > 2005)
                        {
                            if (deger > 0)
                                yiliTahakkuk2 = yiliTahakkuk2 + deger;
                            else
                                yiliTahakkuk2 = yiliTahakkuk2 - deger;
                        }
                
                  if (heskod.StartsWith("810"))    
                     
                        if (yil > 2005)
                        {
                            if (deger > 0)
                                rediade = rediade + deger;
                            else
                                rediade = rediade - deger;
                         
                        }
                    
                    if (heskod.StartsWith("602"))    
                
                        if (yil <= 2005)
                        {
                            if (deger > 0)
                                yiliTahakkuk2 = yiliTahakkuk2 + deger;
                            else
                                yiliTahakkuk2 = yiliTahakkuk2 - deger;
                        }
                         
                       
                    if (heskod.StartsWith("603"))   
                   
                        if (yil <= 2005)
                        {
                            if (deger > 0)
                                rediade = rediade + deger;
                            else
                                rediade = rediade - deger;
                          
                        } 
                     
                }
                dr.Close();
            }


            ve.VeriErisimKapat();


        }

        TertipGelirBilgi DegerBulStatic(VeriErisim veriErisim, int yil, int ay, string[] kodlar, string ekKosul)
        {
            TertipGelirBilgi tBilgi = new TertipGelirBilgi();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            TABGelir islGelir = new TABGelir(ve);
            IDataReader dr = islGelir.GelirDegerBul(yil, ay, kodlar, ekKosul);
            if (dr != null)
            {
                while (dr.Read())
                {
                    tBilgi.tahmin = (double)VeriErisim.DegerAl(dr, 0, (double)0);
                    tBilgi.artik = (double)VeriErisim.DegerAl(dr, 1, (double)0);
                    tBilgi.yiliTahakkuk = (double)VeriErisim.DegerAl(dr, 2, (double)0);
                    tBilgi.rediade = (double)VeriErisim.DegerAl(dr, 3, (double)0);
                    tBilgi.nettahakkuk = (double)VeriErisim.DegerAl(dr, 4, (double)0);
                    tBilgi.devirArtik = (double)VeriErisim.DegerAl(dr, 5, (double)0);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            return tBilgi;
        }

        TertipGelirBilgi DegerBulDinamik(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, string[] kodlar, string ekKosul)
        {
            TertipGelirBilgi tBilgi = new TertipGelirBilgi();
            GelirBilgileriHesapla(kullanan, veriErisim, yil, ay, kodlar, ekKosul, out tBilgi.tahmin, out tBilgi.artik, out tBilgi.yiliTahakkuk, out tBilgi.nettahakkuk, out tBilgi.devirArtik, out tBilgi.rediade, out tBilgi.netTahsilat);
            return tBilgi;
        }

        OdenekBilgileriVAN GelirGerceklesmesiListele(VeriErisim veriErisim, int yil, int ay, Tertip kistasTertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(null));
            OdenekBilgileriVAN obv = new OdenekBilgileriVAN();
            obv.sonuc.islemSonuc = false;
            kistasTertip.NoktalariAt();
            kistasTertip.DiziKontrol();
            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kistasTertip.kodlar[0], kistasTertip.Kirilimlar(0)).Replace(".", "");

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
        /// Gelir bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        public Sonuc KesinHesapGelirBilgileriHesapla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip)
        {

            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim veKesinHesap = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (!(yil > 0))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Yıl bilgisi boş olamaz.";
                return sonuc;
            }
            if (!(ay > 0))
                ay = 12;
            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                sonuc.hataStr = "Kurum kodu boş olamaz!";
            else
            {
                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.KesinHesapOlusturmaYetkisiVarMi(kullanan, yil, kurum))
                {
                    TABGelir tabGelir = new TABGelir(veKesinHesap);
                    veKesinHesap.BeginTransaction();
                    sonuc = tabGelir.GelirBilgileriSil(yil, ay, kurum);
                    if (sonuc.islemSonuc)
                    {

                        OdenekBilgileriVAN gerler = GelirGerceklesmesiListele(veKesinHesap, yil, -1, tertip);

                        if (gerler.sonuc.islemSonuc && sonuc.islemSonuc)
                        {
                            TertipGelirBilgi tertipBilgi = new TertipGelirBilgi();
                            foreach (OdenekBilgi gl in gerler.detaylar)
                            {
                                GelirBilgileriHesapla(kullanan, veKesinHesap, yil, ay, gl.tertip.kodlar, "", out tertipBilgi.tahmin, out tertipBilgi.artik, out tertipBilgi.yiliTahakkuk, out tertipBilgi.nettahakkuk, out tertipBilgi.devirArtik, out tertipBilgi.rediade, out tertipBilgi.netTahsilat);

                                if (sonuc.islemSonuc)
                                    sonuc = tabGelir.GelirBilgileriKaydet(yil, ay, gl.tertip.kodlar, tertipBilgi.tahmin, tertipBilgi.artik, tertipBilgi.yiliTahakkuk, tertipBilgi.rediade, tertipBilgi.nettahakkuk, tertipBilgi.netTahsilat, tertipBilgi.devirArtik);
                                else
                                    break;
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }


                    if (sonuc.islemSonuc)
                        veKesinHesap.Commit();
                    else
                        veKesinHesap.Rollback();
                }
                else
                    sonuc.hataStr = "Bu işlemi yapmaya yetkiniz bulunmamaktadır.";
            }


            veKesinHesap.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Gelir cetveli hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <param name="kacYil">Kaç yıllık oluşturulacak.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapGelirCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi, int kacYil)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            //donusDeger.Add(new Sonuc(false, "Gelir Rakamlarının  aktarılmasında sorun olduğundan daha sonra tekrar deneyiniz.!", ""));
            //return donusDeger;
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            int kirilimSay = 5;

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {

                    TABGelir tabGelir = new TABGelir(veListe);
                    TABGelir tabGelirDeger = new TABGelir(ve);
                    IDataReader dr = tabGelir.EkoKodListele(yil, tertip.kodlar, "");

                    if (dr != null)
                    {
                        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
                        string [] kistaslar =  new string [2];
                        string[] eskiDegerler = new string[kirilimSay];
                        string[] yeniDegerler = new string[kirilimSay];

                        string ekKosul = "";
                        ekKosul = GenelKosul.EkKosulOlusturGelir(tertip.kodlar);

                        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";
                        string tut;

                        while (dr.Read())
                        {
                            int yer = 0;
                            Tertip tertipListe = new Tertip(yil,ENUMTabloKod.TABGelir);
                            tertipListe.kodlar[0]="";
                            tertipListe.kodlar[1] = VeriErisim.DegerAl(dr, 0, "");

                            for (int k = 0; k < 2; k++)
                                kistaslar [k] = "";

                            tut = OrtakFonksiyonlar.KodNoktaKoy((string)tertipListe.Kirilimlar(1), (string)tertipListe.kodlar[1]);
                            kistaslar[0]="";
                            kistaslar[1]="";
                            ObjectArray d = new ObjectArray();
                            OrtakFonksiyonlar.StringiParcala(tut, ".", d);
                            for (int j = 0; j < d.objeler.Count; j++)
                            {
                                yeniDegerler[yer++] = (string)d.objeler[j];

                            }


                            int kodyer = 0;
                            string aciklamaKod = "";
                            for (int j = 0; j < yer; j++)
                            {

                                kistaslar[kodyer + 1] = (string)kistaslar[kodyer + 1] + (string)yeniDegerler[j];

                                if (eskiDegerler[j] != yeniDegerler[j])
                                {
                                    UcgenSatir us = new UcgenSatir();
                                    us.yer = j;
                                    aciklamaKod = (string)kistaslar[kodyer + 1];
                                    us.aciklama = tabGelirDeger.AciklamaBul(yil, aciklamaKod);
                                    us.kodParca = yeniDegerler[j];
                                    if (dinamikMi)
                                        for (int i = kacYil; i >= 0; i--)
                                            us.ekDegerler.Add(DegerBulDinamik(ve, kullanan, yil - i, ay, kistaslar, ekKosul));
                                    else
                                        us.ekDegerler.Add(DegerBulStatic(ve, yil, ay, kistaslar, ekKosul));
                                    donusDeger.Add(us);

                                    for (int k = j + 1; k < yer; k++)
                                        eskiDegerler[k] = "";
                                }
                            }
                            for (int j = 0; j < yer; j++)
                            {
                                eskiDegerler[j] = yeniDegerler[j];
                            }
                        }

                        dr.Close();

                    }

                }
                else
                    donusDeger.Add(new Sonuc(false, "Bu bilgileri görme yetkiniz yok", ""));

            }

            veListe.VeriErisimKapat();
            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Gelir cetveli (çok yıllı) raporunu hazirlayan yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        public TNSCollection KesinHesapCokYilliGelirCetveliHazirla(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi, int kacYil)
        {
            TNSCollection donusDeger = new TNSCollection();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKHM", TNS.KYM.Arac.KimlikBul(kullanan));
            //donusDeger.Add(new Sonuc(false, "Gelir Rakamlarının  aktarılmasında sorun olduğundan daha sonra tekrar deneyiniz.!", ""));
            //return donusDeger;
            if (!(yil > 0))
            {
                donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""));
                return donusDeger;
            }

            int kirilimSay = 2;

            tertip.NoktalariAt();

            if (tertip.kodlar.Length == 0 || (tertip.kodlar.Length > 0 && (string)tertip.kodlar[0] == ""))
                donusDeger.Add(new Sonuc(false, "Kurum kodu boş olamaz!", ""));

            else
            {
                if (!(ay > 0))
                    ay = 12;

                string kurum = (string)tertip.kodlar[0];
                //kurum = Tertip.KurumsaldanKuruma(kurum);

                if (KHMYetkiServisi.GormeYetkisiVarMi(kullanan, kurum))
                {

                    TABGelir tabGelir = new TABGelir(veListe);
                    TABGelir tabGelirDeger = new TABGelir(ve);

                    IDataReader dr = tabGelir.EkoKodListeleSeviye(yil, tertip.kodlar, "", 3);

                    if (dr != null)
                    {
                        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
                        string[] kistaslar = new string[2];
                        string[] eskiDegerler = new string[kirilimSay];
                        string[] yeniDegerler = new string[kirilimSay];

                        string ekKosul = "";
                        ekKosul = GenelKosul.EkKosulOlusturGelir(tertip.kodlar);

                        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";
                        string tut;

                        while (dr.Read())
                        {
                            int yer = 0;
                            Tertip tertipListe = new Tertip(yil,ENUMTabloKod.TABGelir);
                            tertipListe.kodlar[0]="";
                            tertipListe.kodlar[1]=VeriErisim.DegerAl(dr, 0, "");
                            for (int k = 0; k < 2; k++)
                                kistaslar[k] = "";
                           


                            tut = OrtakFonksiyonlar.KodNoktaKoy((string)tertipListe.Kirilimlar(1), (string)tertipListe.kodlar[1]);
                            kistaslar[0]=("");
                            kistaslar[1] = ("");
                            ObjectArray d = new ObjectArray();
                            OrtakFonksiyonlar.StringiParcala(tut, ".", d);
                            for (int j = 0; j < d.objeler.Count; j++)
                            {
                                yeniDegerler[yer++] = (string)d.objeler[j];

                            }


                            int kodyer = 0;
                            string aciklamaKod = "";
                            for (int j = 0; j < yer; j++)
                            {

                                kistaslar[kodyer + 1] = (string)kistaslar[kodyer + 1] + (string)yeniDegerler[j];

                                if (eskiDegerler[j] != yeniDegerler[j])
                                {
                                    UcgenSatir us = new UcgenSatir();
                                    us.yer = j;
                                    aciklamaKod = (string)kistaslar[kodyer + 1];
                                    us.aciklama = tabGelirDeger.AciklamaBul(yil, aciklamaKod);
                                    us.kodParca = yeniDegerler[j];
                                    if (dinamikMi)
                                        for (int i = kacYil; i >= 0; i--)
                                            us.ekDegerler.Add(DegerBulDinamik(ve, kullanan, yil - i, ay, kistaslar, ekKosul));
                                    else
                                        us.ekDegerler.Add(DegerBulStatic(ve, yil, ay, kistaslar, ekKosul));
                                    donusDeger.Add(us);

                                    for (int k = j + 1; k < yer; k++)
                                        eskiDegerler[k] = "";
                                }
                            }
                            for (int j = 0; j < yer; j++)
                            {
                                eskiDegerler[j] = yeniDegerler[j];
                            }
                        }

                        dr.Close();

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
