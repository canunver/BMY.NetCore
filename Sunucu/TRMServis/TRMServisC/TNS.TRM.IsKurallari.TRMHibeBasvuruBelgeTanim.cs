using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMBasvuruBelgeTanim
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, BasvuruBelgeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");


            if (bilgi.etapNo == 0)
                sonuc.hataStr += "Etap No boş bırakılmış<br>";
            if (bilgi.malzemeGrubu.kod == 0 && bilgi.hibeTuru != "Sulama")
                sonuc.hataStr += "Grup bilgisi boş bırakılmış<br>";
            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = BelgeNoAlInt(ve);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = to.SilBelgeler(bilgi);
                if (sonuc.islemSonuc)
                {
                    if (bilgi.belgeAdlari != null)
                    {
                        if (bilgi.belgeAdlari != null)
                        {
                            //Siranosu boş olanlara atama yapmak için
                            int enBuyukSayi = 0;
                            foreach (BasvuruBelgeAdlari b in bilgi.belgeAdlari)
                            {
                                if (b != null)
                                {
                                    if (b.siraNo > enBuyukSayi) enBuyukSayi = b.siraNo;
                                }
                            }

                            foreach (BasvuruBelgeAdlari b in bilgi.belgeAdlari)
                            {
                                if (b != null)
                                {
                                    if (b.belgeKod == 0)
                                        b.belgeKod = bilgi.kod;

                                    if (b.siraNo == 0)
                                    {
                                        enBuyukSayi++;
                                        b.siraNo = enBuyukSayi;
                                    }
                                    if (sonuc.islemSonuc)
                                        sonuc = to.KaydetBelgeler(b);
                                }
                            }
                        }
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
        /// Arguman olarak verilen, ceza bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, BasvuruBelgeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);

            ve.BeginTransaction();

            sonuc = to.SilBelgeler(bilgi);

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
                sonuc.hataStr = "Başvuru belgeleri silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;

            ve.VeriErisimKapat();

            return sonuc;
        }

        ///// <summary>
        ///// Arguman olarak verilen, cezalı kişi bilgisini veritabanından siler 
        ///// ve işlem sonucunu Sonuc nesnesi olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public Sonuc SilBelge(Kullanici kullanan, BasvuruBelgeAdlari bilgi)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (bilgi.kod == 0)
        //        sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

        //    if (!string.IsNullOrEmpty(bilgi.cezaliKisiler[0].TCNO))
        //        sonuc.hataStr = "Silinecek kişi seçilmemiş" + "<br>";

        //    if (sonuc.hataStr != "")
        //    {
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
        //    TABHibeCeza to = new TABHibeCeza(ve);

        //    ve.BeginTransaction();

        //    sonuc = to.SilKisiler(bilgi);

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        ve.Rollback();
        //        sonuc.hataStr = "Cezalı kişi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, BasvuruBelgeTanim kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BasvuruBelgeTanim f = new BasvuruBelgeTanim();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.yil = VeriErisim.DegerAl(r, 1, 0);
                    f.malzemeGrubu = new MalzemeGrubu();
                    f.malzemeGrubu.kod = VeriErisim.DegerAl(r, 2, 0);
                    f.hibeTuru = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.etapNo = VeriErisim.DegerAl(r, 4, 0);
                    f.malzemeGrubu.ad = VeriErisim.DegerAl(r, 5, string.Empty);

                    if (f.malzemeGrubu.kod == 991)
                        f.malzemeGrubu.ad = "Yatırımlar";
                    else if (f.malzemeGrubu.kod == 992)
                        f.malzemeGrubu.ad = "Altyapı";
                    else if (f.malzemeGrubu.kod == 993)
                        f.malzemeGrubu.ad = "Altyapı A-Grubu";
                    else if (f.malzemeGrubu.kod == 994)
                        f.malzemeGrubu.ad = "Altyapı B-Grubu";

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public bool HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(Kullanici kullanan, BasvuruBelgeTanim kriter)
        {
            bool varMi = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);
            IDataReader r = to.HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                    varMi = true;
            }

            r.Close();

            ve.VeriErisimKapat();

            return varMi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme bilgisini veritabanından okur 
        /// ve BasvuruBelgeTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme listesini Malzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleBelgeler(Kullanici kullanan, BasvuruBelgeTanim kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);
            IDataReader r = to.ListeleBelgeler(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BasvuruBelgeTanim f = new BasvuruBelgeTanim();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.yil = VeriErisim.DegerAl(r, 1, 0);
                    f.malzemeGrubu = new MalzemeGrubu();
                    f.malzemeGrubu.kod = VeriErisim.DegerAl(r, 2, 0);
                    f.hibeTuru = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.etapNo = VeriErisim.DegerAl(r, 4, 0);
                    f.belgeAdlari = new BasvuruBelgeAdlari[1];
                    f.belgeAdlari[0] = new BasvuruBelgeAdlari();
                    f.belgeAdlari[0].belgeKod = VeriErisim.DegerAl(r, 5, 0);
                    f.belgeAdlari[0].siraNo = VeriErisim.DegerAl(r, 6, 0);
                    f.belgeAdlari[0].belgeAdi = VeriErisim.DegerAl(r, 7, string.Empty);
                    f.belgeAdlari[0].belgeKisaAdi = VeriErisim.DegerAl(r, 8, string.Empty);
                    f.belgeAdlari[0].gercekTuzel = VeriErisim.DegerAl(r, 9, string.Empty);
                    f.belgeAdlari[0].nitelik = VeriErisim.DegerAl(r, 10, string.Empty);
                    f.belgeAdlari[0].kosul = VeriErisim.DegerAl(r, 11, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, başvuru belge adlarını veritabanından okur 
        /// ve BasvuruBelgeAdlari nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, cezalı kişiler listesini BasvuruBelgeAdlari nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BasvuruBelgeAdlariniListele(Kullanici kullanan, int belgeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);
            IDataReader r = to.BasvuruBelgeAdlariniListele(belgeKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BasvuruBelgeAdlari f = new BasvuruBelgeAdlari();

                    f.belgeKod = VeriErisim.DegerAl(r, 0, 0);
                    f.siraNo = VeriErisim.DegerAl(r, 1, 0);
                    f.belgeAdi = VeriErisim.DegerAl(r, 2, string.Empty);
                    f.belgeKisaAdi = VeriErisim.DegerAl(r, 3, string.Empty);
                    f.gercekTuzel = VeriErisim.DegerAl(r, 4, string.Empty);
                    f.nitelik = VeriErisim.DegerAl(r, 5, string.Empty);
                    f.kosul = VeriErisim.DegerAl(r, 6, string.Empty);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, cezalı kişileri veritabanından okur 
        ///// ve CezaliKisiler nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <returns>İstenilen kriterdeki, cezalı kişiler listesini CezaliKisiler nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray ListeleCezaVeCezaliKisiler(Kullanici kullanan, int cezaKod)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

        //    TABHibeCeza to = new TABHibeCeza(ve);
        //    IDataReader r = to.ListeleCezaVeCezaliKisiler(cezaKod);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            Ceza f = new Ceza();

        //            f.aciklama = VeriErisim.DegerAl(r, 0, "");
        //            f.cezaliKisiler = new CezaliKisiler[1];
        //            f.cezaliKisiler[0] = new CezaliKisiler();
        //            f.cezaliKisiler[0].cezaKod = VeriErisim.DegerAl(r, 1, 0);
        //            f.cezaliKisiler[0].TCNO = VeriErisim.DegerAl(r, 2, string.Empty);
        //            f.cezaliKisiler[0].adSoyad = VeriErisim.DegerAl(r, 3, string.Empty);

        //            bilgi.objeler.Add(f);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}

        private int BelgeNoAlInt(VeriErisim ve)
        {
            TABGenel tg = new TABGenel(ve);

            string sayacKod = "HIBEBASVURUBELGETANIM";

            return tg.BelgeNoAlInt(sayacKod, "000000");
        }

        public Sonuc Aktar(Kullanici kullanan, int eskiEtap, int yeniEtap)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (eskiEtap == 0)
                hata += "Eski Etap No bilgisi boş bırakılamaz" + "<br>";

            if (yeniEtap == 0)
                hata += "Yeni Etap No bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            BasvuruBelgeTanim kriter = new BasvuruBelgeTanim();
            kriter.etapNo = eskiEtap;
            kriter.hibeTuru = "Ekonomik Yatırım";
            ObjectArray bilgi = Listele(kullanan, kriter);

            foreach (BasvuruBelgeTanim belge in bilgi.objeler)
            {
                ObjectArray belgeler = BasvuruBelgeAdlariniListele(kullanan, belge.kod);

                belge.belgeAdlari = new BasvuruBelgeAdlari[belgeler.objeler.Count];
                int sayac = 0;
                foreach (BasvuruBelgeAdlari bgt in belgeler.objeler)
                {
                    belge.belgeAdlari[sayac] = new BasvuruBelgeAdlari();
                    belge.belgeAdlari[sayac].belgeKod = bgt.belgeKod;
                    belge.belgeAdlari[sayac].belgeAdi = bgt.belgeAdi;
                    belge.belgeAdlari[sayac].siraNo = bgt.siraNo;
                    belge.belgeAdlari[sayac].belgeKisaAdi = bgt.belgeKisaAdi;
                    belge.belgeAdlari[sayac].gercekTuzel = bgt.gercekTuzel;
                    belge.belgeAdlari[sayac].nitelik = bgt.nitelik;
                    belge.belgeAdlari[sayac].kosul = bgt.kosul;
                    sayac++;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeBasvuruBelgeTanim to = new TABHibeBasvuruBelgeTanim(ve);

            ve.BeginTransaction();

            kriter = new BasvuruBelgeTanim();
            kriter.etapNo = yeniEtap;
            kriter.hibeTuru = "Ekonomik Yatırım";
            ObjectArray bilgiYeni = Listele(kullanan, kriter);//Yeni aktarılacak etapta daha önceden veri varsa çekilsin. Detayları silinecek

            foreach (BasvuruBelgeTanim belge in bilgiYeni.objeler)
            {
                BasvuruBelgeTanim kriterdt = new BasvuruBelgeTanim();
                kriterdt.kod = belge.kod;
                sonuc = to.SilBelgeler(kriterdt);
            }
            sonuc = to.Sil(kriter);

            foreach (BasvuruBelgeTanim belge in bilgi.objeler)
            {
                belge.kod = BelgeNoAlInt(ve);
                belge.etapNo = yeniEtap;
                belge.yil = System.DateTime.Now.Year;
                sonuc = to.Kaydet(belge);

                foreach (BasvuruBelgeAdlari b in belge.belgeAdlari)
                {
                    if (b != null)
                    {
                        b.belgeKod = belge.kod;
                        sonuc = to.KaydetBelgeler(b);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
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
    }
}
