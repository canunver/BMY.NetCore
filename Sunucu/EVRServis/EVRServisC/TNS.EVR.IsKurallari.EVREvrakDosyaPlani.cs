using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using EVRServisI;
using System.Threading;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak dosya planı bilgileriyle ilgili, kaydetme ve okuma yordamlarını tutan sınıf. 
    /// </summary>
    public class EVREvrakDosyaPlani
    {
        /// <summary>
        /// Paramete olarak verilen DosyaPlani nesne koleksiyonunu, kullanıcının evrak dosya kodu bilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo, ArrayList liste, bool eskiKodlariSil, string gerekce)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            int sayac = 0;//Burayı çağıran metoda kaç tane dosya koduna kaldırılıldı göndermek için

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriEVR");

            TABEvrakDosyaPlani to = new TABEvrakDosyaPlani(ve);
            string hata = "";

            ve.BeginTransaction();
            try
            {
                TarihceyeYaz(ve, kullanan, prEvrakNo, liste, to, gerekce);

                if (eskiKodlariSil)
                    sonuc = to.Sil(prEvrakNo);

                foreach (DosyaPlani dosya in liste)
                {
                    if (!string.IsNullOrEmpty(dosya.kod))
                    {
                        DosyaPlani aranan = EVRDosyaPlani.Oku(ve, dosya);
                        //string dosyaKontrol = EVRGenel.DosyaPlaniAdGetir(dosya.kod.Replace("-", ""));//EVRServis.UzayYaziDegerGetir("EVRDOSYAPLANI", dosya.kod.Replace("-", ""), true, "");
                        if (aranan.ad != "" && aranan.kullanilmiyor == 1)
                        {
                            if (eskiKodlariSil)
                                sonuc = to.Kaydet(prEvrakNo, dosya);
                            else
                            {
                                bool kontrol = to.EvrakNoVeKodBul(prEvrakNo, dosya.kod);//Aynı evrak ve koddan varsa işlem yapmaması için kontrol
                                if (!kontrol)
                                {
                                    //sonuc = to.Guncelle(prEvrakNo, dosya);
                                    //if (!sonuc.islemSonuc)
                                    sonuc = to.Kaydet(prEvrakNo, dosya);
                                }
                            }
                            if (sonuc.islemSonuc)
                                sayac++;
                        }
                        else if (aranan.kullanilmiyor == 2)
                        {
                            sonuc.hataStr = string.Format("<b>{0}</b> dosya kodu kullanıma kapatılmıştır.", dosya.kod);
                            sonuc.islemSonuc = false;
                        }
                        else
                        {
                            sonuc.hataStr = string.Format(EvrakI.EVRI082, dosya.kod);
                            sonuc.islemSonuc = false;
                        }
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

                sonuc.anahtar = sayac.ToString();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{64783D18-8ADE-4278-A026-72BB64841305} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Tarihceyes the yaz.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="prEvrakNo">The pr evrak no.</param>
        /// <param name="liste">The liste.</param>
        /// <param name="to">To.</param>
        public static void TarihceyeYaz(VeriErisim ve, Kullanici kullanan, string prEvrakNo, ArrayList liste, TABEvrakDosyaPlani to, string gerekce)
        {
            TABEvrak toEvrak = new TABEvrak(ve);
            string evrEvrakNo = toEvrak.EvrEvrakNoGetir(prEvrakNo);

            ArrayList oncedenKayitli = Oku(ve, kullanan, prEvrakNo);
            foreach (DosyaPlani dd in oncedenKayitli)
            {
                bool bulundu = false;
                foreach (DosyaPlani dosya in liste)
                {
                    if (dd.kod == dosya.kod)
                    {
                        bulundu = true;
                        dd.seviye = 97;//daha önce vardı
                        break;
                    }
                }
                if (!bulundu)
                    dd.seviye = 99;//silindi
            }
            foreach (DosyaPlani dosya in liste)
            {
                bool bulundu = false;
                foreach (DosyaPlani dd in oncedenKayitli)
                {
                    if (dd.kod == dosya.kod)
                    {
                        bulundu = true;
                        dd.kod = "";
                        break;
                    }
                }
                if (!bulundu && dosya.kod != "")
                {
                    DosyaPlani dd = new DosyaPlani();
                    dd.kod = dosya.kod;
                    dd.seviye = 98;//yeni eklendi
                    oncedenKayitli.Add(dd);
                }
            }

            Sonuc sonuc = new Sonuc();
            string aciklama = "";

            foreach (DosyaPlani dd in oncedenKayitli)
            {
                if (dd.seviye == 98)//yeni
                    aciklama = dd.kod + " nolu dosya plan kodu eklenmiştir. " + gerekce;
                else if (dd.seviye == 99)//silindi
                    aciklama = dd.kod + " nolu dosya plan kodu silinmiştir. " + gerekce;
                else
                    continue;

                sonuc = to.TopluDegistirTarihce(evrEvrakNo, kullanan.mernis, kullanan.ad + " " + kullanan.soyad, "", "", null, null, aciklama);
            }
        }


        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak dosya kod bilgisini DosyaPlani nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak'ın prEvrakNo bilgisini içeren nesne</param>
        /// <returns>
        /// DosyaPlani nesne koleksiyonunu olarak döndürülür.
        /// </returns>
        public static ArrayList Oku(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriEVR");
            TABEvrakDosyaPlani to = new TABEvrakDosyaPlani(ve);

            ArrayList liste = new ArrayList();

            IDataReader dr = to.Listele(prEvrakNo);

            while (dr.Read())
            {
                DosyaPlani detay = new DosyaPlani();
                detay.kod = VeriErisim.DegerAl(dr, 1, "");
                detay.ad = VeriErisim.DegerAl(dr, 2, "");
                liste.Add(detay);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        public static DosyaPlani[] Listele(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakDosyaPlani tab = new TABEvrakDosyaPlani(ve);

            IDataReader dr = tab.Listele2(prEvrakNo);
            if (dr != null)
            {
                while (dr.Read())
                {
                    DosyaPlani d = new DosyaPlani();
                    d.kod = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.KOD, "");
                    d.ad = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.AD, "");
                    d.seviye = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.SEVIYE, 0);
                    d.detay = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.DETAY, 0) == 0;
                    d.kullanilmiyor = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.KULLANILMIYOR, 0);
                    d.saklamaSuresi = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.SAKLAMASURESI, 0);
                    d.saklamaKriteri = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.SAKLAMAKRITERI, 0);
                    d.islemKod = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.ISLEMKOD, 0);
                    d.sureBaslangicTipi = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.SUREBASLANGICTIPI, 0);
                    d.tasfiyePlanTipi = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.TASFIYEPLANTIPI, 0);
                    d.gerekce = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.GEREKCE, "");
                    d.aciklama = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.ACIKLAMA, "");
                    //d.ACILISTARIHI = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.ACILISTARIHI, "");
                    //d.KAPANISTARIHI = VeriErisim.DegerAl(dr, (int)TABDosyaPlani.ENUMAlanlar.KAPANISTARIHI, "");

                    liste.objeler.Add(d);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            DosyaPlani[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (DosyaPlani[])liste.objeler.ToArray(typeof(DosyaPlani));

            return arr;
        }

        /// <summary>
        /// Toplus the degisiklik.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="eskiKod">The eski kod.</param>
        /// <param name="yeniKod">The yeni kod.</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <param name="aciklama">The aciklama.</param>
        /// <returns></returns>
        public static Sonuc TopluDegisiklik(VeriErisim veriErisim, Kullanici kullanan, string eskiKod, string yeniKod, TNSDateTime tarih1, TNSDateTime tarih2, string aciklama, string bakimdakiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string hata = "";
            if (tarih1.isNull || tarih2.isNull)
                hata += "Tarih bilgileri boş bırakılamaz";
            if (eskiKod == yeniKod)
                hata += "Eski ve yeni bilgiler aynı dosya tasnif koduna ait olamaz<br>";
            if (tarih2.Oku() < tarih1.Oku())
                hata += "Son tarih, ilk tarihten küçük olamaz<br>";
            if (string.IsNullOrEmpty(aciklama.Trim()))
                hata += "Açıklama bilgisi giriniz<br>";

            DosyaPlani dp = new DosyaPlani();
            dp.kod = yeniKod;
            ObjectArray liste = EVRDosyaPlani.Listele(null, dp);
            if (liste.objeler.Count == 0)
                hata += yeniKod + " dosya kodu bulunamadı<br>";
            bool varmi = true;
            bool yetkiVar = true;
            foreach (DosyaPlani dosya in liste.objeler)
            {
                if (yeniKod == dosya.kod && dosya.kullanilmiyor == 2)//kapalıysa
                {
                    string kodlar = bakimdakiler;
                    string[] listeStr = kodlar.Split(',');
                    for (int i = 0; i < listeStr.Length; i++)
                    {
                        if (listeStr[i].Trim() == dosya.kod)
                        {//bakımdaysa
                            varmi = true;
                            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI))
                                yetkiVar = false;
                        }
                    }

                }
                else if (yeniKod == dosya.kod && dosya.kullanilmiyor == 1)
                    varmi = true;
            }
            if (varmi == false)
            {
                hata += yeniKod + " dosya kapali olduğundan aktarım yapamazsınız<br>";
                sonuc.islemSonuc = false;
            }

            if (hata != "")
            {
                sonuc.hataStr = "Toplu değişiklik işlemi yapılamadı<br>" + hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (yetkiVar == false)
            {
                hata += yeniKod + " taşıma işlemi yetkiniz bulunmamaktadır<br>";
                sonuc.islemSonuc = false;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriEVR");
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriEVR");
            TABEvrakDosyaPlani to = new TABEvrakDosyaPlani(ve);
            TABEvrakDosyaPlani to2 = new TABEvrakDosyaPlani(ve2);

            ve2.BeginTransaction();

            IDataReader dr = to.TopluDegistirListele(eskiKod, tarih1, tarih2);
            DosyaPlani dp2 = new DosyaPlani();
            int sayac = 0;
            string oncekiEvrEvrakNo = "";

            if (aciklama.Length > 150) aciklama = aciklama.Substring(0, 150);
            while (dr.Read())
            {
                string prEvrakNo = VeriErisim.DegerAl(dr, 0, "");
                string evrEvrakNo = VeriErisim.DegerAl(dr, 1, "");

                sonuc = to2.TopluDegistirHazirla(prEvrakNo, eskiKod, yeniKod);
                if (sonuc.islemSonuc)
                {
                    sonuc = to2.TopluDegistirTarihce(evrEvrakNo, kullanan.mernis, kullanan.ad + " " + kullanan.soyad, eskiKod, yeniKod, tarih1, tarih2, aciklama);
                }
                if (sonuc.islemSonuc)
                {
                    dp2.kod = yeniKod;
                    sonuc = to2.Kaydet(prEvrakNo, dp2);
                }
                else
                    break;

                oncekiEvrEvrakNo = evrEvrakNo;
                sayac++;
            }

            sonuc.anahtar = sayac.ToString();


            dr.Close();

            if (sonuc.islemSonuc)
                ve2.Commit();
            else
                ve2.Rollback();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

    }
}
