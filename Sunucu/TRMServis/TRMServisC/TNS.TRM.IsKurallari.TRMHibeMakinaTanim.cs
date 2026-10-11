using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakinaTanim
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string hata = "";

            bool yetki = Yetki.YetkiVarMiOkumaya(kullanan, bilgi.kayitTur.ToString(), ENUMHibeTur.MAKINE);

            if (!yetki)
                hata += "Malzeme kayıt yetkiniz bulunmamaktadır" + "<br>";
            if (bilgi.marka == "")
                hata += "Marka bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.model == "")
                hata += "Model bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.ad == "")
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";
            if (bilgi.kod > 0 && MalzemeBasvurdaKullanilmisMi(null, bilgi.kod))
                hata += "Bu malzeme bir başvuruda kullanıldığı için değişiklik yapılamaz." + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = BelgeNoAlInt(ve);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = to.SilTanimOzellik(bilgi.kod);
                if (sonuc.islemSonuc)
                {
                    foreach (MakinaTanimOzellik m in bilgi.tanimOzellik)
                        if (m != null)
                        {
                            if (m.malzemeKod == 0)
                                m.malzemeKod = bilgi.kod;

                            if (sonuc.islemSonuc)
                                sonuc = to.KaydetTanimOzellik(m);
                        }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod.ToString();
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MalzemeAnaKodGuncelle(Kullanici kullanan, int malzemeKod, int anaMalzemeKod, int kendiAnaKodu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            ve.BeginTransaction();

            if (kendiAnaKodu > 0)
            {
                if (anaMalzemeKod != kendiAnaKodu)
                {
                    Malzeme bilgi = new Malzeme();
                    bilgi.anaKod = kendiAnaKodu;
                    ObjectArray arr = Listele(kullanan, bilgi);
                    if (arr.ObjeSayisi > 0)
                    {
                        foreach (Malzeme n in arr.objeler)
                        {
                            MalzemeAnaKodGuncelle(kullanan, n.kod, anaMalzemeKod, 0);
                        }
                    }
                }
            }

            sonuc = to.MalzemeAnaKodGuncelle(malzemeKod, anaMalzemeKod);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = " Kayıt işlemi başarılı";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Malzeme bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";
            else if (MalzemeBasvurdaKullanilmisMi(null, bilgi.kod))
                sonuc.hataStr += "Bu malzeme bir başvuruda kullanıldığı için silinemez." + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);

            ve.BeginTransaction();

            sonuc = to.SilTanimOzellik(bilgi.kod);

            if (sonuc.islemSonuc)
            {
                sonuc = to.Sil(bilgi);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
                }
            }
            else
                sonuc.hataStr = "Makine özellikleri silinrken bir hata oluştu" + "<br>" + sonuc.hataStr;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Malzeme kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Malzeme f = new Malzeme();
                    f.malzemeGrubu = new MalzemeGrubu();
                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.malzemeGrubu.kod = VeriErisim.DegerAl(r, 2, 0);
                    f.kayitTur = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.referansFiyat = VeriErisim.DegerAl(r, 4, decimal.Zero);
                    f.marka = VeriErisim.DegerAl(r, 5, string.Empty);
                    f.model = VeriErisim.DegerAl(r, 6, string.Empty);
                    f.anaKod = VeriErisim.DegerAl(r, 7, 0);
                    f.kullanimda = VeriErisim.DegerAl(r, 8, 0);
                    f.malzemeGrubu.ad = VeriErisim.DegerAl(r, 9, string.Empty);
                    f.malzemeGrubu.hibeOran = VeriErisim.DegerAl(r, 10, decimal.Zero);
                    f.il = VeriErisim.DegerAl(r, 11, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public bool AnaMalzemeKodVarMi(Kullanici kullanan, int anaMalzemeKod)
        {
            bool var = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            IDataReader r = to.AnaMalzemeKodVarMi(anaMalzemeKod);

            if (to.Hata() == "")
            {
                if (r.Read())
                    var = true;
            }

            r.Close();

            ve.VeriErisimKapat();

            return var;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleMakinaTanimOzellik(Kullanici kullanan, int malzemeGrupKod, int malzemeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            IDataReader r = to.ListeleMakinaTanimOzellik(malzemeGrupKod, malzemeKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Malzeme f = new Malzeme();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.malzemeGrubu.kod = VeriErisim.DegerAl(r, 2, 0);
                    f.siraNo = VeriErisim.DegerAl(r, 3, 0);
                    f.cokluSecim = VeriErisim.DegerAl(r, 4, 0);
                    f.cokluSecimIcerik = VeriErisim.DegerAl(r, 5, "");
                    f.tanimOzellik = new MakinaTanimOzellik[1];
                    f.tanimOzellik[0] = new MakinaTanimOzellik();

                    if (malzemeKod > 0)
                    {
                        f.tanimOzellik = new MakinaTanimOzellik[1];
                        f.tanimOzellik[0] = new MakinaTanimOzellik();
                        f.tanimOzellik[0].malzemeKod = VeriErisim.DegerAl(r, 6, 0);
                        f.tanimOzellik[0].siraNo = VeriErisim.DegerAl(r, 7, 0);
                        f.tanimOzellik[0].ozellikKod = VeriErisim.DegerAl(r, 8, 0);
                        f.tanimOzellik[0].ad = VeriErisim.DegerAl(r, 9, string.Empty);
                    }
                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleMakinaTanimVeOzellikleri(Kullanici kullanan, Malzeme m)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            IDataReader r = to.ListeleMakinaTanimVeOzellikleri(m);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Malzeme f = new Malzeme();

                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.cokluSecim = VeriErisim.DegerAl(r, 4, 0);
                    f.cokluSecimIcerik = VeriErisim.DegerAl(r, 5, "");
                    f.genelAd = VeriErisim.DegerAl(r, 11, string.Empty);
                    f.referansFiyat = VeriErisim.DegerAl(r, 14, decimal.Zero);
                    f.marka = VeriErisim.DegerAl(r, 15, "");
                    f.model = VeriErisim.DegerAl(r, 16, "");

                    f.tanimOzellik = new MakinaTanimOzellik[1];
                    f.tanimOzellik[0] = new MakinaTanimOzellik();
                    f.tanimOzellik[0].malzemeKod = VeriErisim.DegerAl(r, 6, 0);
                    f.tanimOzellik[0].ad = VeriErisim.DegerAl(r, 9, string.Empty);

                    f.malzemeGrubu = new MalzemeGrubu();
                    f.malzemeGrubu.ad = VeriErisim.DegerAl(r, 17, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static decimal ReferansFiyatListele(int etapNo, int malzemeKod, string ilKod)
        {
            decimal bilgi = decimal.Zero;

            //Merkez tarafından belirlenmiş olan referans fiyat
            //HibeMakineRefFiyat kriter = new HibeMakineRefFiyat();
            //kriter.etapNo = etapNo;
            //kriter.malzemeKod = malzemeKod;
            //kriter.ilKod = ilKod;
            //ObjectArray liste = new TRMHibeMakineRefFiyat().Listele(null, kriter);
            //foreach (HibeMakineRefFiyat rf in liste.objeler)
            //{
            //    bilgi = OrtakFonksiyonlar.ConvertToDecimal(rf.fiyat);
            //    break;
            //}

            //Malzeme bazında belirlenen referans fiyat
            Malzeme kriter = new Malzeme();
            kriter.kod = malzemeKod;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = VeriErisim.DegerAl(r, 4, decimal.Zero);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        private bool MalzemeBasvurdaKullanilmisMi(VeriErisim ve, int malzemeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABHibeMakinaTanim to = new TABHibeMakinaTanim(ve);

            bool donusDeger = to.MalzemeBasvurdaKullanilmisMi(malzemeKod);

            ve.VeriErisimKapat();

            return donusDeger;

        }

        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBEMAKINETANIM";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

    }
}
