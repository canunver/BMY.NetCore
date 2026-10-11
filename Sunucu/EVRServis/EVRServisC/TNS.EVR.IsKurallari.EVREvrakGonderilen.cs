using System.Data;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Gönderilen evrak bilgileriyle ilgili yordamları tutan sınıf.
    /// </summary>
    public class EVREvrakGonderilen
    {
        /// <summary>
        /// Parametre olarak verilen evrak nesne koleksiyonuna göre, giden evrak dağıtım listesini gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="listeEvrak">EvrakSinif nesne koleksiyonu tipinde evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray listeEvrak)
        {
            Sonuc sonuc = new Sonuc(false, "1", "");

            ObjectArray gonderilenEvrakListe = new ObjectArray();
            EvrakGonderilen eGonderilen;

            ObjectArray dagitimListe;
            //EvrakSinif evrak;
            string dosya;
            //string EvrakBirimKokGonderen;
            string EvrakBirimKokGonderilen;
            bool listeyeEkle;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            foreach (EvrakSinif evrak in listeEvrak.objeler)
            {
                //ObjectArray evrakList = EVREvrak.Listele(kullanan, e);
                //evrak = (EvrakSinif)evrakList[0];

                if (evrak.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                {
                    //EvrakBirimKokGonderen = EVRBirim.EvrakKokBirimGetir(evrak.evrakBirimKodu);
                    dosya = EvrakEkDosyaYolu(evrak.evrakBirimKodu, evrak.evrakOlusturmaTarihi.Yil + evrak.evrakOlusturmaTarihi.Ay.ToString("00"), evrak.evrEvrakNo);

                    foreach (EvrakDagitim eDagitim in evrak.dagitim)
                    {

                        //Tanımlanmış dağıtım seilmiş ise
                        if (OrtakFonksiyonlar.ConvertToInt(eDagitim.dagitimKod, 0) > 0)
                        {
                            dagitimListe = EVRDagitim.Listele(ve, kullanan, eDagitim.dagitimKod, null, true);

                            foreach (Dagitim d in dagitimListe.objeler)
                            {
                                foreach (DagitimAlan da in d.alanlar)
                                {
                                    EvrakBirimKokGonderilen = EVRBirim.EvrakKokBirimGetir(ve, da.evrakBrimKodu);

                                    listeyeEkle = true;
                                    foreach (EvrakGonderilen eg in gonderilenEvrakListe.objeler)
                                    {
                                        if (string.IsNullOrEmpty(EvrakBirimKokGonderilen) || (eg.prEvrakNo == evrak.prEvrakNo && eg.gonderilenBirimKodu == EvrakBirimKokGonderilen))
                                        {
                                            listeyeEkle = false;
                                            break;
                                        }
                                    }

                                    if (listeyeEkle)
                                    {
                                        eGonderilen = new EvrakGonderilen();
                                        eGonderilen.prEvrakNo = evrak.prEvrakNo;
                                        eGonderilen.gonderilenBirimKodu = EvrakBirimKokGonderilen;
                                        eGonderilen.gonderenBirimKodu = evrak.evrakBirimKodu;
                                        eGonderilen.konu = evrak.konu;
                                        eGonderilen.evrakTarih = evrak.evrakTarih;
                                        eGonderilen.evrakNo = evrak.evrakNo;
                                        eGonderilen.gizlilik = evrak.gizlilik;
                                        eGonderilen.yaziDurum = evrak.yaziDurum;
                                        eGonderilen.evrakDosyaKodu = ((DosyaPlani)evrak.dosyaKodlari[0]).kod;
                                        eGonderilen.dosya = dosya;
                                        eGonderilen.alindimi = "";
                                        gonderilenEvrakListe.Ekle(eGonderilen);
                                    }
                                }

                            }

                        }


                    }

                }
            }

            sonuc = Kaydet(ve, gonderilenEvrakListe);
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakGonderilen nesne koleksiyonuna göre, gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">EvrakGonderilen nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (liste.objeler.Count > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                ve.BeginTransaction();
                try
                {
                    TABEvrakGonderilen to = new TABEvrakGonderilen(ve);
                    sonuc = to.Kaydet(liste);
                    if (sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{E4CC20A4-EEEC-4001-A659-46C6720AE29D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

                ve.VeriErisimKapat();
            }
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, Evrak gönderilen bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="gonderilenBirimKodu">Gönderilen birim kodunu içeren nesne</param>
        /// <param name="alindimi">Alındı bilgisini içeren nesne</param>
        /// <returns>EvrakGonderilen nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, string gonderilenBirimKodu, int alindimi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakGonderilen to = new TABEvrakGonderilen(ve);
            IDataReader r = to.Listele(gonderilenBirimKodu, alindimi);

            if (to.Hata() == "")
            {
                EvrakGonderilen form;
                while (r.Read())
                {
                    form = new EvrakGonderilen();
                    form.prEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    form.gonderilenBirimKodu = VeriErisim.DegerAl(r, 1, "");
                    form.gonderenBirimKodu = VeriErisim.DegerAl(r, 2, "");
                    form.konu = VeriErisim.DegerAl(r, 3, "");
                    form.evrakTarih = VeriErisim.DegerAl(r, 4);
                    form.evrakNo = VeriErisim.DegerAl(r, 5, 0);
                    form.gizlilik = VeriErisim.DegerAl(r, 6, 0);
                    form.yaziDurum = VeriErisim.DegerAl(r, 7, 0);
                    form.evrakDosyaKodu = VeriErisim.DegerAl(r, 8, "");
                    form.dosya = VeriErisim.DegerAl(r, 9, "");
                    form.alindimi = VeriErisim.DegerAl(r, 10, "");

                    hVAN.objeler.Add(form);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return hVAN;
        }

        /// <summary>
        /// Evrak'ın dosya yolunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrak birim kodunu içeren nesne</param>
        /// <param name="evrakTarih">Evrak tarihini içeren nesne</param>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo içeren nesne</param>
        /// <returns></returns>
        private static string EvrakEkDosyaYolu(string evrakBirim, string evrakTarih, string evrEvrakNo)
        {
            char dirSepChar = System.IO.Path.DirectorySeparatorChar;
            //string evrakDosyaYol = System.Configuration.ConfigurationManager.AppSettings.Get("EvrakDosyaYol") + "";
            //if (!evrakDosyaYol.EndsWith(dirSepChar.ToString()))
            //    evrakDosyaYol += dirSepChar.ToString();
            string evrakDosyaYol = evrakBirim + dirSepChar + evrakTarih + dirSepChar + evrEvrakNo + dirSepChar.ToString();
            return evrakDosyaYol;
        }

    }
}
