using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Öğrenci maaş bordro bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCOgrenciMaasBordro
    {
        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="maasHesaplalist">Kaydedilecek bilgileri PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray maasHesaplalist, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            int satir = 0;
            int yil = 0;
            string surecNo = "";

            if (maasHesaplalist.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
                ///////////////////////////////////////////////////////////////////////////
                bool tamamlandi = true;

                yil = ((PSLOgrenciMaasBordro)maasHesaplalist.objeler[0]).yil;
                surecNo = ((PSLOgrenciMaasBordro)maasHesaplalist.objeler[0]).surecNo;
                tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
                if (tamamlandi)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                    return sonuc;
                }
                ///////////////////////////////////////////////////////////////////////////

                foreach (PSLOgrenciMaasBordro ida in maasHesaplalist.objeler)
                {
                    satir++;

                    if (ida.yil == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00153, satir) + "<br>";
                        break;
                    }
                    if (ida.muhasebe == 0)
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00154, satir) + "<br>";
                        break;
                    }
                    if (ida.birim.Trim() == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00155, satir) + "<br>";
                        break;
                    }
                    if (ida.surecNo == "")
                    {
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00156, satir) + "<br>";
                        break;
                    }
                    if (ida.tCKimlikNo == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00214, satir) + "<br>";
                    if (ida.maasYil == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00215, satir) + "<br>";
                    if (ida.maasAy == 0)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00216, satir) + "<br>";
                }
            }
            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //**************** ******* ** *** MAAŞ HESAPLAMA ************ ** * * * * * ** **********
            PSLOgrenciMaasBordro bordro = (PSLOgrenciMaasBordro)maasHesaplalist.objeler[0];
            TNS.HRC.PSLMaasTemelBilgisi kriter = new TNS.HRC.PSLMaasTemelBilgisi();
            kriter.ay = bordro.maasAy;
            kriter.yil = bordro.maasYil;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            //1. ADIM Net Asgari ücreti genel tanımlamalardan al
            ObjectArray maasTemelBilgiList = (new HRCMaasTemelBilgisi()).Listele(kullanan, kriter);

            if (maasTemelBilgiList.ObjeSayisi == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00218 + "<br>";

            decimal netAsgariUcret = 0;
            if (hataStr == "")
                netAsgariUcret = ((PSLMaasTemelBilgisi)maasTemelBilgiList[0]).netAsgariUcret;

            //2. ADIM Listeden Gelen Öğrencilerin Maaş Hesabını yapalım
            decimal damgaVergisiOran = (decimal)6 / 1000;
            decimal ogrMaasi = Math.Round(netAsgariUcret / 3, 2);
            foreach (PSLOgrenciMaasBordro perMaas in maasHesaplalist.objeler)
            {
                perMaas.tahakkukEden = Math.Round((ogrMaasi / 30) * perMaas.gun, 2);
                perMaas.damgaVergisi = Math.Round(perMaas.tahakkukEden * damgaVergisiOran, 2);
                perMaas.ucret = perMaas.tahakkukEden - perMaas.damgaVergisi;
            }

            TABOgrenciMaasBordro to = new TABOgrenciMaasBordro(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(bordro);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(maasHesaplalist);

            if (sonuc.islemSonuc)
            {
                //Süreç devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                ///////////////////////////////////////////////////////////////////////////

                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş bordro bilgisini veritabanından okur 
        /// ve PSLOgrenciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, öğrenci maaş bordro listesini PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, PSLOgrenciMaasBordro kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOgrenciMaasBordro to = new TABOgrenciMaasBordro(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    PSLOgrenciMaasBordro bilgi = new PSLOgrenciMaasBordro();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tCKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    bilgi.maasYil = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.maasAy = VeriErisim.DegerAl(r, 6, 0);
                    bilgi.siraNo = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.gun = VeriErisim.DegerAl(r, 8, 0);
                    bilgi.tahakkukEden = (decimal)VeriErisim.DegerAl(r, 9, 0.0);
                    bilgi.damgaVergisi = (decimal)VeriErisim.DegerAl(r, 10, 0.0);
                    bilgi.ucret = (decimal)VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.adiSoyadi = VeriErisim.DegerAl(r, 12, "");

                    liste.objeler.Add(bilgi);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MIFNoKaydet(Kullanici kullanici, PSLOgrenciMaasBordro f, string belgeNo)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";
            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanici));
            ve.BeginTransaction();
            TABOgrenciMaasBordro tab = new TABOgrenciMaasBordro(ve);
            sonuc = tab.MIFNoKaydet(f, belgeNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="kullanici">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns> 
        public Sonuc MIFNoListele(Kullanici kullanici, PSLOgrenciMaasBordro f)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = false;

            hata = Kontrol(f);
            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABOgrenciMaasBordro tab = new TABOgrenciMaasBordro(ve);
            IDataReader r = null;
            try
            {
                r = tab.MIFNoListele(f);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (r.Read())
            {
                sonuc.anahtar = VeriErisim.DegerAl(r, 4, "");
                sonuc.islemSonuc = true;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private string Kontrol(PSLOgrenciMaasBordro f)
        {
            string hata = "";

            if (f.yil == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003;
            }

            if (f.muhasebe == 0)
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004;
            }
            if (f.birim == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005;
            }
            if (f.surecNo == "")
            {
                if (hata != "") hata += "<br>";
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067;
            }
            if (hata == "")
            {
                string ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", f.muhasebe.ToString(), true, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00234, f.muhasebe);
                }

                ad = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", f.birim.Trim() + "-" + f.muhasebe, false, "");
                if (ad == "")
                {
                    if (hata != "") hata += "<br>";
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00235, f.birim);
                }
            }
            return hata;
        }
    }
}
