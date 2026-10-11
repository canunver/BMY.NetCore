using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    class HRCFazlaMesaiPuantajUnvan
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve FazlaMesaiPuantajUnvanlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islemi yapan.</param>
        /// <param name="f">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini FazlaMesaiPuantajUnvan nesnesi olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici islemYapan)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(islemYapan));

            TABFazlaMesaiPuantajUnvan to = new TABFazlaMesaiPuantajUnvan(ve);

            IDataReader r = null;

            try
            {
                r = to.Listele();
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                r.Close();
                ve.VeriErisimKapat();

                return liste;
            }

            while (r.Read())
            {
                FazlaMesaiPuantajUnvan fm = new FazlaMesaiPuantajUnvan();
                fm.unvanKod = VeriErisim.DegerAl(r, 0, 0);
                fm.toplamSaat = VeriErisim.DegerAl(r, 1, 0.0);
                fm.kesilenSaat = VeriErisim.DegerAl(r, 2, 0.0);
                fm.unvanAd = VeriErisim.DegerAl(r, 3, string.Empty);

                liste.objeler.Add(fm);
            }

            r.Close();

            if (liste.ObjeSayisi == 0)
            {
                IDataReader r1 = null;
                try
                {
                    r1 = to.ListeleUnvan();

                }
                catch (Exception e)
                {
                    liste.sonuc.islemSonuc = false;
                    liste.sonuc.hataStr = e.Message;

                    r1.Close();
                    ve.VeriErisimKapat();

                    return liste;
                }

                while (r1.Read())
                {
                    FazlaMesaiPuantajUnvan fm = new FazlaMesaiPuantajUnvan();
                    fm.unvanKod = VeriErisim.DegerAl(r1, 0, 0);
                    fm.unvanAd = VeriErisim.DegerAl(r1, 1, string.Empty);

                    liste.objeler.Add(fm);
                }
                r1.Close();
            }
            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="liste">The liste.</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABFazlaMesaiPuantajUnvan tab = new TABFazlaMesaiPuantajUnvan(ve);

            if (liste.ObjeSayisi > 0)
            {
                sonuc = tab.Sil();
                if (sonuc.islemSonuc)
                    sonuc = tab.Kaydet(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                }
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

    }
}
