using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Öğrenci Eğitmen bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// TanimOgrenciEgitmen formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCOgrenciEgitmen
    {
        /// <summary>
        /// Arguman olarak verilen, öğrenci eğitmen bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, OgrenciEgitmen bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            double kontrolMuhasebe = 0;
            string kontrolBirim = "";

            if (bilgi.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            else if (kontrolMuhasebe == 0)
            {
                kontrolMuhasebe = HRCServis.UzaySayiDegerGetir("MUHMUHASEBE", bilgi.muhasebe.ToString(), true, "MUHASEBEKOD");
                if (Convert.ToInt32(kontrolMuhasebe) != bilgi.muhasebe)
                    hata += "<br>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00234, bilgi.muhasebe);
            }

            if (bilgi.birim == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            else if (kontrolBirim == "")
            {
                kontrolBirim = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", bilgi.birim + "-" + bilgi.muhasebe.ToString(), false, "BIRIMKOD");
                if (kontrolBirim != bilgi.birim)
                    hata += "<br>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00235, bilgi.birim);
            }

            if (bilgi.tcKimlikNo.Trim() == "")
                hata += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00236;
            if (bilgi.adiSoyadi.Trim() == "")
                hata += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00237;
            if (bilgi.tip == 0)
                hata += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00238;
            if (bilgi.durum == 0)
                hata += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00239;

            PersonelBilgi pb = new HRCPersonel().Listele(null, kullanan, bilgi.tcKimlikNo, 0, "");
            if (pb != null && pb.adSoyad != "")
                hata += "<br>" + string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00240, bilgi.tcKimlikNo);

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOgrenciEgitmen to = new TABOgrenciEgitmen(ve);


            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
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
        /// Arguman olarak verilen kriterlere göre Öğrenci ve Eğitmen bilgilerini veritabanından okur ve OgrenciEgitmen nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren OgrenciEgitmen nesnesi</param>
        /// <returns>İstenilen kriterdeki Öğrenci ve Eğitmen bilgileri listesini OgrenciEgitmen nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, OgrenciEgitmen kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOgrenciEgitmen to = new TABOgrenciEgitmen(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OgrenciEgitmen abilgi = new OgrenciEgitmen();

                    abilgi.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    abilgi.birim = VeriErisim.DegerAl(r, 1, "");
                    abilgi.tcKimlikNo = VeriErisim.DegerAl(r, 2, "");
                    abilgi.adiSoyadi = VeriErisim.DegerAl(r, 3, "");
                    abilgi.gorev = VeriErisim.DegerAl(r, 4, "");
                    abilgi.tip = VeriErisim.DegerAl(r, 5, 0);
                    abilgi.durum = VeriErisim.DegerAl(r, 6, 0);
                    abilgi.bankaKodu = VeriErisim.DegerAl(r, 7, "");
                    abilgi.bankaSubeKodu = VeriErisim.DegerAl(r, 8, "");
                    abilgi.bankaHesapNo = VeriErisim.DegerAl(r, 9, "");

                    bilgi.objeler.Add(abilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
