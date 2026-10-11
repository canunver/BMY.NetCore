using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

//using MUHServisC.ebutce.Muhasebe;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    public class MUHMuhasebatEntegrasyon
    {
        private static long YevmiyeNoAl(int yil)
        {
            MUHServisC.MuhasebatMaliyeYevmiyeWS.SiradakiYevmiyeNoSorgulaPortClient client = new MUHServisC.MuhasebatMaliyeYevmiyeWS.SiradakiYevmiyeNoSorgulaPortClient();

            MUHServisC.MuhasebatMaliyeYevmiyeWS.siradakiYevmiyeNoSorgulaRequest req = new MUHServisC.MuhasebatMaliyeYevmiyeWS.siradakiYevmiyeNoSorgulaRequest();
            req.defterNo = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimDefterNo", ""));
            req.istenilenYevmiyeAdedi = 1;
            req.maliYil = yil;

            var soapInspector = new SoapInspectorBehavior();
            client.Endpoint.EndpointBehaviors.Add(soapInspector);

            MUHServisC.MuhasebatMaliyeYevmiyeWS.siradakiYevmiyeNoSorgulaResponse res = client.siradakiYevmiyeNoSorgula(req);

            long[] liste = res.siradakiDefterYevmiyeNoList;

            if (liste != null && liste.Length > 0)
                return liste[0];

            return 0;
        }

        public static string SonUcHaneyiDuzenle(string deger)
        {
            if (string.IsNullOrWhiteSpace(deger))
                return deger;

            string[] parcalar = deger.Split('.');

            if (parcalar.Length != 9)
                return deger;

            for (int i = parcalar.Length - 3; i < parcalar.Length; i++)
            {
                if (parcalar[i].Length == 1)
                    parcalar[i] = "0" + parcalar[i];
            }

            return string.Join(".", parcalar);
        }

        public Sonuc Gonder(Kullanici kullanan, ObjectArray oebListe)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");
            Hashtable htBirim = new Hashtable();
            Hashtable htSaymanlik = new Hashtable();
            Hashtable htHesap = new Hashtable();

            MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlariEkleRequest mker = new MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlariEkleRequest();

            MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlari mk = new MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlari();
            MUHServisC.MuhasebatMaliyeWS.dokumanBilgileri db = new MUHServisC.MuhasebatMaliyeWS.dokumanBilgileri();
            MUHServisC.MuhasebatMaliyeWS.kamuIdaresiBilgileri kib = new MUHServisC.MuhasebatMaliyeWS.kamuIdaresiBilgileri();
            List<MUHServisC.MuhasebatMaliyeWS.kayitBilgileri> kbListe = new List<MUHServisC.MuhasebatMaliyeWS.kayitBilgileri>();

            //Dokuman bilgilerinin oluşturan bilgisi
            MUHServisC.MuhasebatMaliyeWS.olusturanBilgisi db_ob = new MUHServisC.MuhasebatMaliyeWS.olusturanBilgisi();
            db_ob.sahis = new MUHServisC.MuhasebatMaliyeWS.sahis();
            db_ob.sahis.ad = kullanan.ad;
            db_ob.sahis.soyad = kullanan.soyad;
            db_ob.sahis.unvan = kullanan.unvan;
            db_ob.sahis.uyruk = MUHServisC.MuhasebatMaliyeWS.uyruk.YERLI;
            db_ob.sahis.vatandaslikNumarasi = kullanan.mernis;

            //Doküman bilgileri
            db.aciklama = "";
            //db.dokumanTarihi = DateTime.Now;
            //db.donemBaslangici = new DateTime(DateTime.Now.Year, 1, 1);
            //db.donemSonu = new DateTime(DateTime.Now.Year, 12, 31);
            db.ETTN = Guid.NewGuid().ToString();
            db.kayitTurKodu = MUHServisC.MuhasebatMaliyeWS.kayitTurKodlari.YEVMIYE;
            db.olusturanBilgisi = db_ob;

            //Kamu idaresi bilgisi
            kib.defterAdi = OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimDefterAdi", "");
            kib.defterNumarasi = OrtakFonksiyonlar.ConvertToLong(OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimDefterNo", ""));
            kib.kamuIdaresiAdi = OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimKamuIdaresiAdi", "");
            kib.kamuIdaresiVergiKimlikNumarasi = OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimKamuIdaresiVKN", "");
            if (string.IsNullOrWhiteSpace(kib.kamuIdaresiVergiKimlikNumarasi))
                kib.kamuIdaresiVergiKimlikNumarasi = "2160080374";

            //kib.maliYil = DateTime.Now.Year;

            //Kayıt alt detay bilgileri listesi
            List<MUHServisC.MuhasebatMaliyeWS.kayitAltDetayBilgileri> kadbListe = new List<MUHServisC.MuhasebatMaliyeWS.kayitAltDetayBilgileri>();
            List<MUHServisC.MuhasebatMaliyeWS.kayitDetayBilgileri> kdbListe = new List<MUHServisC.MuhasebatMaliyeWS.kayitDetayBilgileri>();

            //SGB Belge kayıtları
            foreach (OdemeEmriMIFForm oeb in oebListe.objeler)
            {
                db.dokumanTarihi = oeb.islemTarih.Oku();
                db.donemBaslangici = new DateTime(oeb.islemTarih.Yil, 1, 1);
                db.donemSonu = new DateTime(oeb.islemTarih.Yil, 12, 31);
                kib.maliYil = oeb.islemTarih.Yil;

                HarcamaBirimListesiDoldur(oeb.muhasebe, oeb.birim, ref htBirim);

                SaymanlikListesiDoldur(oeb.muhasebe, ref htSaymanlik);

                long toplamBorc = 0;
                long toplamAlacak = 0;
                //Belgelerin içindeki satırlar
                foreach (OdemeEmriMIFDetay oed in oeb.detay.objeler)
                {
                    HarcamaBirimListesiDoldur(oeb.muhasebe, oed.detayBirim, ref htBirim);
                    HesapPlaniListesiDoldur(oed.hesapKod.Substring(0, 3), ref htHesap);

                    //Kayıt alt detay bilgileri
                    MUHServisC.MuhasebatMaliyeWS.kayitAltDetayBilgileri kadb = new MUHServisC.MuhasebatMaliyeWS.kayitAltDetayBilgileri();
                    kadb.aciklama = "";
                    kadb.borcAlacakKodu = oed.borc > 0 ? MUHServisC.MuhasebatMaliyeWS.borcAlacakTuru.BORC : MUHServisC.MuhasebatMaliyeWS.borcAlacakTuru.ALACAK;

                    kadb.butceTertibi = SonUcHaneyiDuzenle(oed.sorguTertip);
                    if (kadb.butceTertibi.IndexOf("---") > -1) kadb.butceTertibi = "";

                    kadb.hesap = new MUHServisC.MuhasebatMaliyeWS.hesap();
                    kadb.hesap.anaHesapKodu = oed.hesapKod.Substring(0, 3);//ana hesap adı alınacak
                    kadb.hesap.anaHesapAdi = HesapPlanBilgisiVer(kadb.hesap.anaHesapKodu, htHesap, "ADI");//ana hesap alınacak
                    kadb.hesap.yardimciHesapKodu = oed.hesapKod;
                    kadb.hesap.yardimciHesapAdi = HesapPlanBilgisiVer(kadb.hesap.yardimciHesapKodu, htHesap, "ADI");

                    string[] kurlar = oed.kur.Split('.');
                    string gKur = "";
                    foreach (string item in kurlar)
                    {
                        int kk = OrtakFonksiyonlar.ConvertToInt(item, 0);
                        if (kk == 0) continue;
                        if (gKur != "") gKur += ".";
                        gKur += kk;
                    }

                    kadb.kurumsalKod = gKur;
                    kadb.siraNumarasi = oed.satirNo.ToString();
                    kadb.tutar = new MUHServisC.MuhasebatMaliyeWS.kurusCinsindenTL();
                    kadb.tutar.kurus = oed.borc > 0 ? OrtakFonksiyonlar.ConvertToLong(oed.borc * 100) : OrtakFonksiyonlar.ConvertToLong(oed.alacak * 100);
                    kadbListe.Add(kadb);

                    if (kadb.borcAlacakKodu == MUHServisC.MuhasebatMaliyeWS.borcAlacakTuru.BORC)
                        toplamBorc += kadb.tutar.kurus;
                    else
                        toplamAlacak += kadb.tutar.kurus;
                }

                //Eğer fiş açılış ise her zaman yevmiye no 1 olacak, diğer durumlarda sıradaki yevmiye numarası sorulsun 12.06.2023 Melih Gelen mail üzerine yapıldı
                //long yevNo = 0;
                //if (oeb.fisTur[0] == ENUMFISTUR.ACILIS)
                //    yevNo = 1;
                //else
                //    yevNo = 0; // YevmiyeNoAl(oeb.yil);

                //Kayıt detay bilgileri
                MUHServisC.MuhasebatMaliyeWS.kayitDetayBilgileri kdb = new MUHServisC.MuhasebatMaliyeWS.kayitDetayBilgileri();

                kdb.aciklama = "";
                kdb.harcamaBirimi = new MUHServisC.MuhasebatMaliyeWS.harcamaBirimi();
                kdb.harcamaBirimi.harcamaBirimiAdi = HarcamaBirimBilgisiVer(oeb.muhasebe, oeb.birim, htBirim, "ADI");
                kdb.harcamaBirimi.harcamaBirimiVergiKimlikNumarasi = HarcamaBirimBilgisiVer(oeb.muhasebe, oeb.birim, htBirim, "VERGINO");
                if (string.IsNullOrWhiteSpace(kdb.harcamaBirimi.harcamaBirimiVergiKimlikNumarasi))
                    kdb.harcamaBirimi.harcamaBirimiVergiKimlikNumarasi = "2160080374";
                kdb.iliskiliKayitNumarasi = 0;
                kdb.iliskiliKayitNumarasiSpecified = true;
                kdb.iliskiliKayitTuru = MUHServisC.MuhasebatMaliyeWS.muhasebeKaydiIliskiliKayitTuru.BUTCE;
                kdb.iliskiliKayitTuruSpecified = true;
                kdb.kayitAltDetayBilgileri = kadbListe.ToArray();
                kdbListe.Add(kdb);

                //Kaydı oluşturan bilgisi
                MUHServisC.MuhasebatMaliyeWS.olusturanBilgisi dk_ob = new MUHServisC.MuhasebatMaliyeWS.olusturanBilgisi();
                dk_ob.sahis = new MUHServisC.MuhasebatMaliyeWS.sahis();
                dk_ob.sahis.ad = AdveyaSoyadVer(oeb.islemYapanAd, "AD");
                dk_ob.sahis.soyad = AdveyaSoyadVer(oeb.islemYapanAd, "SOYAD");
                dk_ob.sahis.unvan = ".";
                dk_ob.sahis.uyruk = MUHServisC.MuhasebatMaliyeWS.uyruk.YERLI;
                dk_ob.sahis.vatandaslikNumarasi = ".";
                if (string.IsNullOrWhiteSpace(dk_ob.sahis.ad)) dk_ob = db_ob;

                MUHServisC.MuhasebatMaliyeWS.kayitBilgileri kb = new MUHServisC.MuhasebatMaliyeWS.kayitBilgileri();
                kb.aciklama = "";
                kb.adinaIslemYapilanBirim = "";
                kb.disSistemNumarasi = oeb.belgeNo;
                //kb.dokuman
                kb.kayitAdedi = kdbListe.Count;
                kb.kayitDetayBilgileri = kdbListe.ToArray();

                if (oeb.fisTur[0] == ENUMFISTUR.ACILIS)
                {
                    kb.kayitNumarasi = 1;
                    kb.kayitNumarasiSpecified = true;
                }
                else
                {
                    kb.kayitNumarasi = 0;
                    kb.kayitNumarasiSpecified = false;
                }

                kb.kayitTarihi = oeb.islemTarih.Oku();
                kb.muhasebeBirimi = new MUHServisC.MuhasebatMaliyeWS.muhasebeBirimi();
                kb.muhasebeBirimi.muhasebeBirimiAdi = oeb.muhasebeAd;
                kb.muhasebeBirimi.muhasebeBirimiVergiKimlikNumarasi = SaymanlikBilgisiVer(oeb.muhasebe, htSaymanlik, "VERGINO");
                if (string.IsNullOrWhiteSpace(kb.muhasebeBirimi.muhasebeBirimiVergiKimlikNumarasi))
                    kb.muhasebeBirimi.muhasebeBirimiVergiKimlikNumarasi = "2160080374";

                if (oeb.fisTur[0] == ENUMFISTUR.ACILIS) kb.muhasebeIslemFisiTurKodu = MUHServisC.MuhasebatMaliyeWS.yevmiyeTuru.ACILIS;
                else if (oeb.fisTur[0] == ENUMFISTUR.KAPANIS) kb.muhasebeIslemFisiTurKodu = MUHServisC.MuhasebatMaliyeWS.yevmiyeTuru.KAPANIS;
                else kb.muhasebeIslemFisiTurKodu = MUHServisC.MuhasebatMaliyeWS.yevmiyeTuru.DONEM_ICI_ISLEM;

                kb.olusturanBilgisi = dk_ob;
                kb.raporTarihi = oeb.islemTarih.Oku();
                kb.raporTarihiSpecified = true;
                kb.toplamAlacak = new MUHServisC.MuhasebatMaliyeWS.kurusCinsindenTL();
                kb.toplamBorc = new MUHServisC.MuhasebatMaliyeWS.kurusCinsindenTL();
                kb.toplamAlacak.kurus = toplamAlacak;
                kb.toplamBorc.kurus = toplamBorc;
                kbListe.Add(kb);
            }
            mk.dokumanBilgileri = db;
            mk.kamuIdaresiBilgileri = kib;
            mk.kayitBilgileri = kbListe.ToArray();

            mker.muhasebeKayitlari = mk;
            string kullaniciKodu = TNS.DEG.Arac.DegiskenDegerBul(0, "MuhasebatAktarimKul");
            string sifre = TNS.DEG.Arac.DegiskenDegerBul(0, "MuhasebatAktarimSifre");
            string yontem = TNS.DEG.Arac.DegiskenDegerBul(0, "MuhasebatAktarimYontem");

            string dbgText = $"Yöntem:{yontem} - Kullanıcı:{OrtakFonksiyonlar.Maskele(kullaniciKodu, 2, 2)} Şifre:{OrtakFonksiyonlar.Maskele(sifre, 2, 2)}";

            MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlariEkleResponse mkec = new MUHServisC.MuhasebatMaliyeWS.muhasebeKayitlariEkleResponse();
            try
            {
                string muhasebatMaliyeWSURL = OrtakFonksiyonlar.WebConfigOku("MuhasebatAktarimURL", "");

                MUHServisC.MuhasebatMaliyeWS.MuhasebeKayitlariEklePortClient mkeps = new MUHServisC.MuhasebatMaliyeWS.MuhasebeKayitlariEklePortClient();

                var soapInspector = new SoapInspectorBehavior();
                mkeps.Endpoint.EndpointBehaviors.Add(soapInspector);


                if (yontem == "2")
                {
                    string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(kullaniciKodu + ":" + sifre));

                    using (new OperationContextScope(mkeps.InnerChannel))
                    {
                        var requestMessage = new HttpRequestMessageProperty();
                        requestMessage.Headers["Authorization"] = "Basic " + auth;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                        mkec = mkeps.muhasebeKayitlariEkle(mker);
                    }
                }
                else
                {
                    if (yontem == "1")
                    {
                        mkeps.ClientCredentials.UserName.UserName = kullaniciKodu;
                        mkeps.ClientCredentials.UserName.Password = sifre;
                    }
                    mkec = mkeps.muhasebeKayitlariEkle(mker);
                }
            }
            catch (FaultException excp)
            {
                string yol = OrtakFonksiyonlar.WebConfigOku("HataDosyasi", "");
                yol = yol.Replace(@"\Hata.txt", "");
                yol = Path.Combine(yol, "res" + DateTime.Now.ToString("yyyyMM") + ".txt");

                sonuc.hataStr = dbgText + File.ReadAllText(yol);
                sonuc.islemSonuc = false;

                return sonuc;
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = dbgText  + "Exception:" + ex.Message + "\n" + ex.StackTrace;
                return sonuc;
            }

            sonuc.islemSonuc = false;
            if (mkec.sonuc == MUHServisC.MuhasebatMaliyeWS.sonuc.BASARILI)
            {
                sonuc.islemSonuc = true;
                sonuc.anahtar = mkec.ETTN;

                ObjectArray mifeListe = new ObjectArray();
                foreach (OdemeEmriMIFForm oeb in oebListe.objeler)
                {
                    MIFEntegrasyon mife = new MIFEntegrasyon();
                    mife.yil = oeb.yil;
                    mife.belgeNo = oeb.belgeNo;
                    mife.ettn = sonuc.anahtar;
                    mife.islemYapan = kullanan.mernis;
                    mife.tarih = new TNSDateTime(DateTime.Now);
                    mife.durum = 1;
                    mifeListe.Ekle(mife);
                }

                MUHOdemeEmriMIFServis.KaydetEntegraston(null, kullanan, mifeListe);
            }

            return sonuc;
        }

        public class SoapMessageInspector : IClientMessageInspector
        {
            public string LastRequestXml { get; private set; }
            public string LastResponseXml { get; private set; }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                LastRequestXml = request.ToString();

                string yol = OrtakFonksiyonlar.WebConfigOku("HataDosyasi", "");
                yol = yol.Replace(@"\Hata.txt", "");
                yol = Path.Combine(yol, "req" + DateTime.Now.ToString("yyyyMM") + ".txt");
                File.WriteAllText(yol, LastRequestXml);
                return request;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                LastResponseXml = reply.ToString();

                string yol = OrtakFonksiyonlar.WebConfigOku("HataDosyasi", "");
                yol = yol.Replace(@"\Hata.txt", "");
                yol = Path.Combine(yol, "res" + DateTime.Now.ToString("yyyyMM") + ".txt");
                File.WriteAllText(yol, LastResponseXml);
            }
        }

        public class SoapInspectorBehavior : IEndpointBehavior
        {
            private readonly SoapMessageInspector inspector_ = new SoapMessageInspector();

            public string LastRequestXml => inspector_.LastRequestXml;
            public string LastResponseXml => inspector_.LastResponseXml;

            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.ClientMessageInspectors.Add(inspector_);
            }
        }


        private void SaymanlikListesiDoldur(int saymanlikKod, ref Hashtable htSaymanlik)
        {
            if (htSaymanlik[saymanlikKod] != null) return;

            ObjectArray muhListe = MUHMuhasebeServis.Listele(null, new MuhasebeBilgisi { kod = saymanlikKod });
            foreach (MuhasebeBilgisi m in muhListe.objeler)
            {
                htSaymanlik[m.kod] = m;
            }
        }

        private void HarcamaBirimListesiDoldur(int saymanlikKod, string birimKod, ref Hashtable htBirim)
        {
            if (htBirim[saymanlikKod + "_" + birimKod] != null) return;

            ObjectArray birimListe = new MUHMuhasebeBirimServis().Listele(new MuhasebeBirim { muhasebe = saymanlikKod, birimKod = birimKod });
            foreach (MuhasebeBirim m in birimListe.objeler)
            {
                htBirim[m.muhasebe + "_" + m.birimKod] = m;
            }
        }

        private string HarcamaBirimBilgisiVer(int saymanlikKod, string birimKod, Hashtable htBirim, string tur)
        {
            if (htBirim[saymanlikKod + "_" + birimKod] == null) return "";

            string donenDeger = "";

            MuhasebeBirim birim = (MuhasebeBirim)htBirim[saymanlikKod + "_" + birimKod];
            if (tur == "ADI")
                return birim.birimAd;
            else if (tur == "KURKOD")
                return birim.kurum;
            else if (tur == "VERGINO")
                return birim.vno;

            return donenDeger;
        }

        private string SaymanlikBilgisiVer(int saymanlikKod, Hashtable htSaymanlik, string tur)
        {
            string donenDeger = "";

            MuhasebeBilgisi saymanlik = (MuhasebeBilgisi)htSaymanlik[saymanlikKod];
            if (tur == "ADI")
                return saymanlik.ad;
            else if (tur == "VERGINO")
                return saymanlik.vergiNo;

            return donenDeger;
        }

        private string AdveyaSoyadVer(string adSoyad, string tur)
        {
            string donenDeger = "";
            try
            {
                int index = adSoyad.LastIndexOf(" ");
                if (tur == "SOYAD")
                    donenDeger = adSoyad.Substring(index).Trim();
                else if (tur == "AD")
                    donenDeger = adSoyad.Substring(0, index).Trim();
            }
            catch { }

            return donenDeger;
        }

        private void HesapPlaniListesiDoldur(string hesapKod, ref Hashtable htHesap)
        {
            if (htHesap[hesapKod] != null) return;

            ObjectArray hesapListe = new MUHHesapPlaniServis().Listele(null, new HesapPlaniSatir { yil = DateTime.Now.Year, hesapKod = hesapKod + ";" });
            foreach (HesapPlaniSatir m in hesapListe.objeler)
            {
                htHesap[m.hesapKod.Replace(".", "")] = m;
            }
        }

        private string HesapPlanBilgisiVer(string hesapKod, Hashtable htHesap, string tur)
        {
            hesapKod = hesapKod.Replace(".", "");

            if (htHesap[hesapKod] == null) return "";

            string donenDeger = "";

            HesapPlaniSatir hesap = (HesapPlaniSatir)htHesap[hesapKod];
            if (tur == "ADI")
                return hesap.aciklama;

            return donenDeger;
        }


    }
}
