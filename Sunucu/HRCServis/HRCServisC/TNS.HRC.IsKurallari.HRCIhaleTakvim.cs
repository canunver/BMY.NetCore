using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale takvim bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleTakvimi
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale takvim bilgisini veritabanından okur 
        /// ve IhaleTakvim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale takvim bilgisi listesini IhaleTakvim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Surec kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisi tabIlanBilgisi = new TABIhaleIlanBilgisi(ve);
            ObjectArray bilgi = new ObjectArray();
            IhaleTakvim t = new IhaleTakvim();

            /*1 İhalenin ilan edilmesi*/
            OnayForm onayForm = new OnayForm();
            onayForm.birim = kriter.birim;
            onayForm.yil = kriter.yil;
            onayForm.muhasebe = kriter.muhasebe;
            onayForm.surecNo = kriter.surecNo;

            ObjectArray onayFormListe = new HRCOnayForm().Listele(kullanan, onayForm);

            if (onayFormListe.ObjeSayisi == 1)
            {
                onayForm = (OnayForm)onayFormListe.objeler[0];

                if (onayForm.ihaleUsulu == (int)ENUMIhaleUsul.DOGRUDANTEMIN)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }

                IhaleIlanTanim ilanSekli = new HRCIhaleIlanTanim().IhaleIlanSekliAl(kullanan, onayForm.ihaleUsulu, onayForm.yaklasikMaliyet, kriter.islem.tur);

                IhaleIlanBilgisi ilanBilgisi = new IhaleIlanBilgisi();
                ilanBilgisi.birim = kriter.birim;
                ilanBilgisi.yil = kriter.yil;
                ilanBilgisi.muhasebe = kriter.muhasebe;
                ilanBilgisi.surecNo = kriter.surecNo;

                ilanBilgisi = new HRCIhaleIlanBilgisi().Listele(kullanan, ilanBilgisi);

                if (ilanBilgisi.ihaleTarihi.isNull == false)
                {
                    t = new IhaleTakvim();
                    t.sure = new TNSDateTime(ilanBilgisi.ihaleTarihi.ToLongString());
                    t.sure.AddDays(-ilanSekli.zaman);
                    if (onayForm.ilanVerilecekmi == (int)ENUMEvetHayir.EVET)
                        t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00203;
                    else
                        t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00204;

                    bilgi.Ekle(t);

                    /*3Teklif Mektuplarının gönderilmesi*/
                    t = new IhaleTakvim();
                    t.sure = new TNSDateTime(ilanBilgisi.ihaleTarihi.ToLongString());
                    t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00205;

                    bilgi.Ekle(t);
                    /************************************/

                    /*4 Teklif geçerlilik süresi*/
                    t = new IhaleTakvim();
                    t.sure = new TNSDateTime(ilanBilgisi.ihaleTarihi.ToLongString());
                    t.sure.AddDays(ilanBilgisi.teklifSuresi);
                    t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00206;

                    bilgi.Ekle(t);
                    /****************************/
                }
            }
            else
            {
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }
            /******************************************/


            /*2 İhale işlem dosyasının komisyona gönderilmesi*/
            IhaleIlanBilgisi iib = new IhaleIlanBilgisi();
            iib.birim = kriter.birim;
            iib.yil = kriter.yil;
            iib.muhasebe = kriter.muhasebe;
            iib.surecNo = kriter.surecNo;

            iib = new HRCIhaleIlanBilgisi().Listele(kullanan, iib);

            if (iib.tarih.isNull == false)
            {
                t = new IhaleTakvim();
                t.sure = new TNSDateTime(iib.tarih.ToLongString());
                t.sure.AddDays(3);
                t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00207;
                bilgi.Ekle(t);
            }
            else
            {
                FirmaTeklifDaveti firmaDavet = new FirmaTeklifDaveti();
                firmaDavet.birim = kriter.birim;
                firmaDavet.yil = kriter.yil;
                firmaDavet.muhasebe = kriter.muhasebe;
                firmaDavet.surecNo = kriter.surecNo;

                ObjectArray oa = new HRCIhaleFirmaTeklifDaveti().Listele(kullanan, firmaDavet);

                foreach (FirmaTeklifDaveti davet in oa.objeler)
                {
                    if (davet.belgeGondermeTarihi.Ticks > firmaDavet.belgeGondermeTarihi.Ticks)
                        firmaDavet = davet;
                }
                if (firmaDavet.belgeGondermeTarihi.isNull == false)
                {
                    t = new IhaleTakvim();
                    t.sure = new TNSDateTime(firmaDavet.belgeGondermeTarihi.ToLongString());
                    t.sure.AddDays(3);
                    t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00207;
                    bilgi.Ekle(t);
                }
            }

            /*******************************************/

            /*5	İhale komisyon kararının onaylanması*/
            IhaleKarari ihaleKarari = new IhaleKarari();
            ihaleKarari.birim = kriter.birim;
            ihaleKarari.yil = kriter.yil;
            ihaleKarari.muhasebe = kriter.muhasebe;
            ihaleKarari.surecNo = kriter.surecNo;

            ihaleKarari = new HRCIhaleKarari().Listele(kullanan, ihaleKarari);

            if (ihaleKarari.tarih.isNull == false)
            {
                t = new IhaleTakvim();
                t.sure = new TNSDateTime(ihaleKarari.tarih.ToLongString());
                t.sure.AddDays(5);
                t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00208;

                bilgi.Ekle(t);
                /****************************************/
            }


            /*6	Kesinleşen ihale kararının bildirilmesi*/
            /****************************************/
            IhaleSonucu ihaleSonucu = new IhaleSonucu();
            ihaleSonucu.birim = kriter.birim;
            ihaleSonucu.yil = kriter.yil;
            ihaleSonucu.muhasebe = kriter.muhasebe;
            ihaleSonucu.surecNo = kriter.surecNo;
       
            ihaleSonucu = new HRCIhaleSonucu().Listele(kullanan, ihaleSonucu);
            if (ihaleSonucu.tarih.isNull == false)
            {
                t = new IhaleTakvim();
                t.sure = new TNSDateTime(ihaleSonucu.tarih.ToLongString());
                t.sure.AddDays(3);
                t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00209;
                bilgi.Ekle(t);

                t = new IhaleTakvim();
                t.sure = new TNSDateTime(ihaleSonucu.tarih.ToLongString());
                t.sure.AddDays(10);
                t.ihaleSurec = TNS.HRC.HarcamaSurecleriResI.HRC00210;
                bilgi.Ekle(t);


            }
            /*7	Sözleşmeye davet yazısı*/


            /***************************/


            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}
