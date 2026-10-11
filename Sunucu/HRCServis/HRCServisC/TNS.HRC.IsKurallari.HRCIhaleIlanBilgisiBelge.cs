using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale ilan bilgisine ait belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleIlanBilgisiBelge
    {
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (((IhaleIlanBilgisiBelge)bilgi.objeler[0]).belgeTur == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00184 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = ((IhaleIlanBilgisiBelge)bilgi.objeler[0]).yil;
            string surecNo = ((IhaleIlanBilgisiBelge)bilgi.objeler[0]).surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);
            ve.BeginTransaction();

            //sonuc = to.Sil((IhaleIlanBilgisiBelge)bilgi.objeler[0]);

            //if (sonuc.islemSonuc)
            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
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
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (belge.belgeTur == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00184 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = belge.yil;
            string surecNo = belge.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(belge);

            if (sonuc.islemSonuc)
            {
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisine ait belge bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisiBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisine ait belge bilgisi listesini IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleIlanBilgisiBelge kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);
            IDataReader r = to.Listele(kriter);

            ObjectArray bilgi = new ObjectArray();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleIlanBilgisiBelge belge = new IhaleIlanBilgisiBelge();
                    belge.yil = VeriErisim.DegerAl(r, 0, 0);
                    belge.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    belge.birim = VeriErisim.DegerAl(r, 2, "");
                    belge.surecNo = VeriErisim.DegerAl(r, 3, "");
                    belge.belgeTur = VeriErisim.DegerAl(r, 4, 0);
                    belge.siraNo = VeriErisim.DegerAl(r, 5, 0);
                    belge.adi = VeriErisim.DegerAl(r, 6, "");
                    belge.aciklama = VeriErisim.DegerAl(r, 7, "");
                    bilgi.objeler.Add(belge);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = belge.yil;
            string surecNo = belge.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(belge);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00185, belge.adi) + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.objeler.Count == 0)
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00186;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = ((IhaleIlanBilgisiBelge)bilgi.objeler[0]).yil;
            string surecNo = ((IhaleIlanBilgisiBelge)bilgi.objeler[0]).surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);

            ve.BeginTransaction();

            foreach (IhaleIlanBilgisiBelge belge in bilgi.objeler)
            {
                sonuc = to.Sil(belge);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018 + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir üst sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YukariTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = belge.yil;
            string surecNo = belge.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);

            ve.BeginTransaction();

            int usttekiBelgeSiraNo = to.UsttekiBelgeSiraNo(belge);
            if (usttekiBelgeSiraNo > 0)
            {
                int belgeSiraNo = belge.siraNo;

                //Üstteki belgenin bilgilerini getir
                belge.siraNo = usttekiBelgeSiraNo;
                ObjectArray belgeler = Listele(kullanan, belge);

                IhaleIlanBilgisiBelge usttekiBelge = new IhaleIlanBilgisiBelge();
                if (belgeler.objeler.Count > 0)
                {
                    usttekiBelge = (IhaleIlanBilgisiBelge)belgeler.objeler[0];

                    ObjectArray yerDegistirecekBelgeler = new ObjectArray();
                    //Her iki belgenin siraNo larını birbirleri ile değiştir
                    usttekiBelge.siraNo = belgeSiraNo;
                    belge.siraNo = usttekiBelgeSiraNo;

                    //Her iki belgeyi de yeni halleri ile listeye ekle
                    yerDegistirecekBelgeler.objeler.Add(belge);
                    yerDegistirecekBelgeler.objeler.Add(usttekiBelge);

                    sonuc = Kaydet(kullanan, yerDegistirecekBelgeler, islemKod, parcaKod);
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00187;
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00187;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00188 + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00189 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir alt sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AsagiTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = belge.yil;
            string surecNo = belge.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisiBelge to = new TABIhaleIlanBilgisiBelge(ve);

            ve.BeginTransaction();

            int alttakiBelgeSiraNo = to.AlttakiBelgeSiraNo(belge);
            if (alttakiBelgeSiraNo > 0)
            {
                int belgeSiraNo = belge.siraNo;

                //Üstteki belgenin bilgilerini getir
                belge.siraNo = alttakiBelgeSiraNo;
                ObjectArray belgeler = Listele(kullanan, belge);

                IhaleIlanBilgisiBelge alttakiBelge = new IhaleIlanBilgisiBelge();
                if (belgeler.objeler.Count > 0)
                {
                    alttakiBelge = (IhaleIlanBilgisiBelge)belgeler.objeler[0];

                    ObjectArray yerDegistirecekBelgeler = new ObjectArray();
                    //Her iki belgenin siraNo larını birbirleri ile değiştir
                    alttakiBelge.siraNo = belgeSiraNo;
                    belge.siraNo = alttakiBelgeSiraNo;

                    //Her iki belgeyi de yeni halleri ile listeye ekle
                    yerDegistirecekBelgeler.objeler.Add(belge);
                    yerDegistirecekBelgeler.objeler.Add(alttakiBelge);

                    sonuc = Kaydet(kullanan, yerDegistirecekBelgeler, islemKod, parcaKod);
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00190;
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00190;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00191 + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00192 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
