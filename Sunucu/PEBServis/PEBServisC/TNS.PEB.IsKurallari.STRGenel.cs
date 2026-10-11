using System;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Collections.Generic;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Stratejik plan modülü iş kuralları sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class STRGenel
    {
        /// <summary>
        /// Verilen misyonun sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Misyon varsa true, yoksa false döndürülür.</returns>
        public static bool MisyonVarMi(VeriErisim ve, int donemBYil, int dilKod, string kurumKod)
        {
            Misyon misyon = new Misyon();
            misyon.donemBYil = donemBYil;
            misyon.dil.kod = dilKod;
            misyon.kurumKod = kurumKod;

            return MisyonVarMi(ve, misyon);
        }

        /// <summary>
        /// Verilen misyonun sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Misyon varsa true, yoksa false döndürülür.</returns>
        public static bool MisyonVarMi(VeriErisim ve, Misyon misyon)
        {
            StratejikYazi yazi = new StratejikYazi(misyon);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen vizyonun sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Vizyon varsa true, yoksa false döndürülür.</returns>
        public static bool VizyonVarMi(VeriErisim ve, Vizyon vizyon)
        {
            StratejikYazi yazi = new StratejikYazi(vizyon);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen temanın sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Tema varsa true, yoksa false döndürülür.</returns>
        public static bool TemaVarMi(VeriErisim ve, Tema tema)
        {
            StratejikYazi yazi = new StratejikYazi(tema);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        /// <summary>
        /// Verilen stratejik amacın sistemde kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Stratejik amaç varsa true, yoksa false döndürülür.</returns>
        public static bool StratejikAmacVarMi(VeriErisim ve, StratejikAmac amac)
        {
            StratejikYazi yazi = new StratejikYazi(amac);
            TABYazi tYazi = new TABYazi(ve);
            return tYazi.YaziVarMi(yazi);
        }

        public static Sonuc AsamaKontrolBaslat(VeriErisim ve, Kullanici kullanan, int donemNo, int orgBirimKod, ENUMSurec konu)
        {
            AsamaKontrol askon = new AsamaKontrol();
            askon.donem = new Donem();
            askon.donem.donemNo = donemNo;
            askon.orgBirimKod = orgBirimKod;
            askon.konuNo = (int)konu;

            Sonuc sonuc = new Sonuc();
            if (!AsamaKontrolVarMi(ve, askon))
                sonuc = STRAsamaKontrol.Kaydet(ve, kullanan, askon, ENUMAsamaKontrolIslemTur.Kaydet, false);
            return sonuc;
        }

        private static bool AsamaKontrolVarMi(VeriErisim ve, AsamaKontrol askon)
        {
            TABAsamaKontrol tabAK = new TABAsamaKontrol(ve);
            return tabAK.AsamaKontrolKayitVarmi(askon);
        }

        public static bool AsamaKontrolDetayOnayliMi(VeriErisim ve, int donemNo, int orgBirimKod, ENUMSurec konu, string formId)
        {
            AsamaKontrolDetay askonDetay = new AsamaKontrolDetay();
            askonDetay.donem = new Donem();
            askonDetay.donem.donemNo = donemNo;
            askonDetay.orgBirimKod = orgBirimKod;
            askonDetay.konuNo = (int)konu;
            askonDetay.kontrolListesi = formId;

            TABAsamaKontrol tabAS = new TABAsamaKontrol(ve);
            return tabAS.DetayOnayliMi(askonDetay);
        }
    }
}