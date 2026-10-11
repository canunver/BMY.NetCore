using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans bütçe modülü raporlarıyla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// raporları üreten fonksiyonları tutan sınıf
    /// </summary>
    public class PEBRaporlar
    {
        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        private static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Verilen stratejik hedefe ait performans hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public static ObjectArray StrHedefRaporla(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int strHedefKod, string senaryoKod)
        {
            ObjectArray perHedefVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.StrHedefRaporla(donemBYil, strHedefKod, senaryoKod); //???
            while (dr.Read())
            {
                PerformansHedef gelen = new PerformansHedef();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.odenek = VeriErisim.DegerAl(dr, 7, (double)0);

                perHedefVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (perHedefVAN.objeler.Count == 0)
                perHedefVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return perHedefVAN;
        }

        /// <summary>
        /// Verilen stratejik amaca ait stratejik hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StrAmacRapor nesnesi döndürülür.</returns>
        public static ObjectArray StrAmacRaporla(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int strAmacKod, string senaryoKod)
        {
            ObjectArray odenekVAN = new ObjectArray();
            StrAmacRapor strAmacRapor = new StrAmacRapor();

            //Stratejik Hedefleri alıyor
            StratejikHedef hedefKriter = new StratejikHedef();
            hedefKriter.donemBYil = donemBYil;
            hedefKriter.dil = new Dil(1, "");
            hedefKriter.strAmac = new StratejikAmac();
            hedefKriter.strAmac.strAmacKod = strAmacKod;
            ObjectArray oaHedef = StratejikHedefListele(kullanan, hedefKriter, true);
            if (!oaHedef.sonuc.islemSonuc) return oaHedef;
            strAmacRapor.strHedefler = oaHedef.objeler;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            foreach (StratejikHedef hedef in strAmacRapor.strHedefler)
            {
                IDataReader dr = tRapor.StrAmacRaporla(hedef.donemBYil, hedef.strHedefKod, senaryoKod);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        StrAmacRaporOdenek gelen = new StrAmacRaporOdenek();
                        gelen.donemBYil = hedef.donemBYil;
                        gelen.strHedefKod = hedef.strHedefKod;
                        gelen.strHedefAd = hedef.strHedef;
                        gelen.strHedefKisa = hedef.strHedefKisa;
                        gelen.kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                        gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                        gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", DateTime.Today.Year.ToString() + "-" + gelen.kurKod, true, "");
                        gelen.odenek = VeriErisim.DegerAl(dr, 1, (double)0);

                        strAmacRapor.odenekler.Add(gelen);
                    }
                    dr.Close();
                }
                else
                    odenekVAN.sonuc.DegerAta(ve.hata);
            }

            if (strAmacRapor.strHedefler.Count > 0 || strAmacRapor.odenekler.Count > 0)
                odenekVAN.objeler.Add(strAmacRapor);

            if (odenekVAN.objeler.Count == 0)
                odenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return odenekVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 4. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        public static ObjectArray OdenekDurumRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, string odenekSenaryo)
        {
            return OdenekDurumRaporla(veriErisim, kullanan, yil, kurKod, odenekSenaryo, (int)ENUMAbsKodSinif.TERTIP, 0);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 2. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        public static ObjectArray OdenekDurumRaporla2Duzey(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, string odenekSenaryo)
        {
            return OdenekDurumRaporla(veriErisim, kullanan, yil, kurKod, odenekSenaryo, (int)ENUMAbsKodSinif.TERTIP, 3);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ekonomik kod uzunluğu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        private static ObjectArray OdenekDurumRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, string odenekSenaryo, int absKodSinif, int uzunluk)
        {
            OdenekDurumListesi odenekDurumRaporu = new OdenekDurumListesi();
            odenekDurumRaporu.yil = yil;
            odenekDurumRaporu.kurKod = kurKod;
            odenekDurumRaporu.odenekSenaryo = odenekSenaryo;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            //Faaliyet ve Proje Ödenekleri toplanıyor
            Faaliyet faaliyet = null;
            Proje proje = null;
            int eskiKod = 0;
            IDataReader dr = tRapor.FaaliyetProjeOdenekleri(yil, kurKod, odenekSenaryo, absKodSinif, uzunluk);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int projeMi = VeriErisim.DegerAl(dr, 3, 0);
                    if (eskiKod == 0 || eskiKod != VeriErisim.DegerAl(dr, 1, 0))
                    {
                        if ((int)ENUMFaaliyetProje.Faaliyet == projeMi)
                        {
                            faaliyet = new Faaliyet();
                            faaliyet.kod = VeriErisim.DegerAl(dr, 1, 0);
                            faaliyet.ad = VeriErisim.DegerAl(dr, 2, string.Empty);
                            faaliyet.tur = VeriErisim.DegerAl(dr, 4, 0);
                            odenekDurumRaporu.faaliyetler.Add(faaliyet);
                            eskiKod = faaliyet.kod;
                        }
                        else if ((int)ENUMFaaliyetProje.Proje == projeMi)
                        {
                            proje = new Proje();
                            proje.kod = VeriErisim.DegerAl(dr, 1, 0);
                            proje.ad = VeriErisim.DegerAl(dr, 2, string.Empty);
                            proje.tur = VeriErisim.DegerAl(dr, 4, 0);
                            odenekDurumRaporu.projeler.Add(proje);
                            eskiKod = proje.kod;
                        }
                    }

                    TertipOdenek odenek = new TertipOdenek(yil);

                    if (absKodSinif == (int)ENUMAbsKodSinif.TERTIP)
                    {
                        odenek.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 5, string.Empty);
                        odenek.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 6, string.Empty);
                        odenek.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 7, string.Empty);
                        odenek.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 8, string.Empty);
                        odenek.tertip.Noktala();
                        odenek.odenek = VeriErisim.DegerAl(dr, 9, (double)0);
                    }
                    else
                    {
                        odenek.tertip.kodlar[absKodSinif - 1] = VeriErisim.DegerAl(dr, 5, string.Empty);
                        odenek.tertip.Noktala();
                        odenek.odenek = VeriErisim.DegerAl(dr, 6, (double)0);
                    }

                    if ((int)ENUMFaaliyetProje.Faaliyet == projeMi)
                        faaliyet.odenekler.Add(odenek);
                    else if ((int)ENUMFaaliyetProje.Proje == projeMi)
                        proje.odenekler.Add(odenek);
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            //Birim Sabit Maliyet Ödenekleri toplanıyor
            OdenekDurumBirimSMTopla(tRapor, dr, odenekDurumRaporu.birimSMOdenekler, yil, kurKod, odenekSenaryo, absKodSinif, uzunluk);

            //Faaliyetlere Yansıtılanlar toplanıyor
            OdenekDurumYansitilanlariTopla(tRapor, dr, odenekDurumRaporu.faaliyetler, yil, kurKod, odenekSenaryo);

            //Toplanan bilgiler ObjectArray' e dolduruluyor
            ObjectArray rapor = new ObjectArray();
            if (odenekDurumRaporu.faaliyetler.Count != 0 || odenekDurumRaporu.projeler.Count != 0 || odenekDurumRaporu.birimSMOdenekler.Count != 0)
                rapor.objeler.Add(odenekDurumRaporu);
            if (rapor.objeler.Count == 0)
                rapor.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return rapor;
        }

        /// <summary>
        /// Birim türündeki sabit maliyetlerin ödeneklerini bulup birimSM değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="birimSM">Ödenek bilgilerinin ekleneceği değişken</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ekonomik kod uzunluğu</param>
        private static void OdenekDurumBirimSMTopla(TABRaporlar tRapor, IDataReader dr, TNSCollection birimSM, int yil, string kurKod, string odenekSenaryo, int absKodSinif, int uzunluk)
        {
            //Birim Sabit Maliyet Ödenekleri toplanıyor
            dr = tRapor.SabitMaliyetOdenekleri(yil, kurKod, odenekSenaryo, 0, (int)ENUMSabitMaliyetTur.Birim, absKodSinif, uzunluk);
            while (dr.Read())
            {
                TertipOdenek odenek = new TertipOdenek(yil);

                if (absKodSinif == (int)ENUMAbsKodSinif.TERTIP)
                {
                    odenek.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 3, string.Empty);
                    odenek.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 4, string.Empty);
                    odenek.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 5, string.Empty);
                    odenek.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 6, string.Empty);
                    odenek.tertip.Noktala();
                    odenek.odenek = VeriErisim.DegerAl(dr, 7, (double)0);
                }
                else
                {
                    odenek.tertip.kodlar[absKodSinif - 1] = VeriErisim.DegerAl(dr, 3, string.Empty);
                    odenek.tertip.Noktala();
                    odenek.odenek = VeriErisim.DegerAl(dr, 4, (double)0);
                }

                birimSM.Add(odenek);
            }
            dr.Close();
        }

        /// <summary>
        /// Sabit maliyetlerden faaliyetlere yansıtılan ödenekleri
        /// bulup faaliyetler değişkenindeki ilgili nesnelere ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="faaliyetler">Faaliyet nesneleri listesini tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        private static void OdenekDurumYansitilanlariTopla(TABRaporlar tRapor, IDataReader dr, TNSCollection faaliyetler, int yil, string kurKod, string odenekSenaryo)
        {
            //Faaliyetlere Yansıtılanlar toplanıyor
            Faaliyet faaliyet = null;
            int fpKod = 0;
            dr = tRapor.FaaliyetYansitilanlar(yil, kurKod, odenekSenaryo);
            while (dr.Read())
            {
                //İlgili Faaliyet bulunuyor
                if (fpKod == 0 || fpKod != VeriErisim.DegerAl(dr, 1, 0))
                {
                    fpKod = VeriErisim.DegerAl(dr, 1, 0);
                    foreach (Faaliyet f in faaliyetler)
                    {
                        if (fpKod == f.kod)
                        {
                            faaliyet = f;
                            break;
                        }
                    }
                }

                int smTur = VeriErisim.DegerAl(dr, 4, 0);
                double odenek = VeriErisim.DegerAl(dr, 5, (double)0);
                if (smTur == (int)ENUMSabitMaliyetTur.Birim)
                    faaliyet.yansitilanlar[(int)ENUMSabitMaliyetTur.Birim - 1] = odenek;
                else if (smTur == (int)ENUMSabitMaliyetTur.Merkez)
                    faaliyet.yansitilanlar[(int)ENUMSabitMaliyetTur.Merkez - 1] = odenek;
                else if (smTur == (int)ENUMSabitMaliyetTur.Tasra)
                    faaliyet.yansitilanlar[(int)ENUMSabitMaliyetTur.Tasra - 1] = odenek;
                else if (smTur == (int)ENUMSabitMaliyetTur.MerkezTasra)
                    faaliyet.yansitilanlar[(int)ENUMSabitMaliyetTur.MerkezTasra - 1] = odenek;
            }
            dr.Close();
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri, ödeneklerini ve sabit maliyetlerden
        /// yansıyan ödenekleri toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceFaaliyetDagilim nesneleri listesi döndürülür.</returns>
        public static ObjectArray ButceFaaliyetDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string odenekSenaryo)
        {
            ObjectArray butceFaaliyetDagilimVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            //Faaliyet ve Proje Ödenekleri toplanıyor
            IDataReader dr = tRapor.FaaliyetProjeOdenekleri(yil, odenekSenaryo, false);
            while (dr.Read())
            {
                ButceFaaliyetDagilim gelen = new ButceFaaliyetDagilim();
                gelen.fp.yil = yil;
                gelen.fp.kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.fp.kurKod = PEBGenel.KurKodNoktala(gelen.fp.kurKod);
                gelen.fp.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.fp.yil.ToString() + "-" + gelen.fp.kurKod, true, "");
                gelen.fp.fpKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.fp.fpAd = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.fp.projeMi = VeriErisim.DegerAl(dr, 3, 0);
                gelen.fp.tur = VeriErisim.DegerAl(dr, 4, 0);
                gelen.odenek = VeriErisim.DegerAl(dr, 5, (double)0);

                butceFaaliyetDagilimVAN.objeler.Add(gelen);
            }
            dr.Close();

            //Faaliyet Personel Giderleri toplanıyor
            ButceFaaliyetDagilimPersonelGiderleriTopla(tRapor, dr, butceFaaliyetDagilimVAN.objeler, yil, odenekSenaryo);

            //Faaliyetlere Yansıtılanlar toplanıyor
            ButceFaaliyetDagilimYansitilanlariTopla(tRapor, dr, butceFaaliyetDagilimVAN.objeler, yil, odenekSenaryo);

            if (butceFaaliyetDagilimVAN.objeler.Count == 0)
                butceFaaliyetDagilimVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return butceFaaliyetDagilimVAN;
        }

        /// <summary>
        /// Faaliyetlerin personel giderleri ödeneklerini bulup objeler değişkenindeki ilgili nesnelere ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="objeler">ButceFaaliyetDagilim nesneleri listesini tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        private static void ButceFaaliyetDagilimPersonelGiderleriTopla(TABRaporlar tRapor, IDataReader dr, TNSCollection objeler, int yil, string odenekSenaryo)
        {
            //Faaliyet Personel Giderleri toplanıyor
            ButceFaaliyetDagilim bfDagilim = null;
            string kurKod = string.Empty;
            int fpKod = 0;
            dr = tRapor.FaaliyetProjeOdenekleri(yil, odenekSenaryo, true);
            while (dr.Read())
            {
                //İlgili Faaliyet bulunuyor
                if ((string.IsNullOrEmpty(kurKod) && fpKod == 0) || kurKod != VeriErisim.DegerAl(dr, 0, string.Empty) || fpKod != VeriErisim.DegerAl(dr, 1, 0))
                {
                    kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    kurKod = PEBGenel.KurKodNoktala(kurKod);
                    fpKod = VeriErisim.DegerAl(dr, 1, 0);
                    bfDagilim = ButceFaaliyetDagilimBul(objeler, kurKod, fpKod);
                }

                if (bfDagilim != null)
                    bfDagilim.personelGider = VeriErisim.DegerAl(dr, 5, (double)0);
            }
            dr.Close();
        }

        /// <summary>
        /// Sabit maliyetlerden faaliyetlere yansıtılan ödenekleri
        /// bulup objeler değişkenindeki ilgili nesnelere ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="dr">DataReader nesnesi</param>
        /// <param name="objeler">ButceFaaliyetDagilim nesneleri listesini tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        private static void ButceFaaliyetDagilimYansitilanlariTopla(TABRaporlar tRapor, IDataReader dr, TNSCollection objeler, int yil, string odenekSenaryo)
        {
            //Faaliyetlere Yansıtılanlar toplanıyor
            ButceFaaliyetDagilim bfDagilim = null;
            string kurKod = string.Empty;
            int fpKod = 0;
            dr = tRapor.FaaliyetYansitilanlar(yil, odenekSenaryo);
            while (dr.Read())
            {
                //İlgili Faaliyet bulunuyor
                if ((string.IsNullOrEmpty(kurKod) && fpKod == 0) || kurKod != VeriErisim.DegerAl(dr, 0, string.Empty) || fpKod != VeriErisim.DegerAl(dr, 1, 0))
                {
                    kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    kurKod = PEBGenel.KurKodNoktala(kurKod);
                    fpKod = VeriErisim.DegerAl(dr, 1, 0);
                    bfDagilim = ButceFaaliyetDagilimBul(objeler, kurKod, fpKod);
                }

                if (bfDagilim != null)
                {
                    int smTur = VeriErisim.DegerAl(dr, 4, 0);
                    double odenek = VeriErisim.DegerAl(dr, 5, (double)0);
                    if (smTur == (int)ENUMSabitMaliyetTur.Birim)
                        bfDagilim.birimSM = odenek;
                    else if (smTur == (int)ENUMSabitMaliyetTur.Merkez)
                        bfDagilim.merkezSM = odenek;
                    else if (smTur == (int)ENUMSabitMaliyetTur.Tasra)
                        bfDagilim.tasraSM = odenek;
                    else if (smTur == (int)ENUMSabitMaliyetTur.MerkezTasra)
                        bfDagilim.merkezTasraSM = odenek;
                }
            }
            dr.Close();
        }

        /// <summary>
        /// ButceFaaliyetDagilim nesneleri listesinden aranan nesneyi bulan yordam
        /// </summary>
        /// <param name="objeler">ButceFaaliyetDagilim nesneleri listesini tutan nesne</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <returns>İlgili ButceFaaliyetDagilim nesnesi bulunursa nesne, bulunamazsa null döndürülür.</returns>
        private static ButceFaaliyetDagilim ButceFaaliyetDagilimBul(TNSCollection objeler, string kurKod, int fpKod)
        {
            //İlgili ButceFaaliyetDagilim bulunuyor
            foreach (ButceFaaliyetDagilim bfd in objeler)
                if (kurKod == bfd.fp.kurKod && fpKod == bfd.fp.fpKod)
                    return bfd;

            return null;
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılına ait temaları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Temalardan oluşan liste döndürülür.</returns>
        private static TNSCollection TemaListele(Kullanici kullanan, int donemBYil)
        {
            Tema tema = new Tema();
            tema.donemBYil = donemBYil;
            tema.dil = new Dil(1, "");
            ObjectArray temalar = STRTema.Listele(null, kullanan, tema);
            return temalar.objeler;
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılına ait stratejik amaçları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Stratejik amaçlardan oluşan liste döndürülür.</returns>
        private static TNSCollection StratejikAmacListele(Kullanici kullanan, int donemBYil)
        {
            StratejikAmac sAmac = new StratejikAmac();
            sAmac.donemBYil = donemBYil;
            sAmac.dil = new Dil(1, "");
            ObjectArray amaclar = STRAmac.Listele(null, kullanan, sAmac);
            return amaclar.objeler;
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılına ait stratejik hedefleri listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Stratejik hedeflerden oluşan liste döndürülür.</returns>
        private static ObjectArray StratejikHedefListele(Kullanici kullanan, int donemBYil, bool hedefler)
        {
            StratejikHedef sHedef = new StratejikHedef();
            sHedef.donemBYil = donemBYil;
            sHedef.dil = new Dil(1, "");
            return StratejikHedefListele(kullanan, sHedef, hedefler);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedefleri listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Stratejik hedeflerden oluşan liste döndürülür.</returns>
        private static ObjectArray StratejikHedefListele(Kullanici kullanan, StratejikHedef sHedef, bool hedefler)
        {
            return STRHedef.Listele(null, kullanan, sHedef, hedefler);
        }

        /// <summary>
        /// Faaliyet/projelere ait kimlik, ağırlık ve ödenek bilgilerini bulup gönderilen objeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">Faaliyet/proje nesnelerinin ekleneceği nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phkurKod">Birim kodu</param>
        /// <param name="phKod">Performans hedefin programatik kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        private static void PerformansHedefDagilimFaaliyetProjeleriAl(TABRaporlar tRapor, TNSCollection objeler, int yil, string phkurKod, int phKod, string odenekSenaryo, int fpTur, int yansiyan)
        {
            FaaliyetProjeToplamlariAl(tRapor, objeler, yil, phkurKod, phKod, 0, fpTur, odenekSenaryo, yansiyan);
        }

        /// <summary>
        /// Faaliyet/projelere ait kimlik, ağırlık ve ödenek bilgilerini bulup gönderilen objeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">Faaliyet/proje nesnelerinin ekleneceği nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phkurKod">Birim kodu</param>
        /// <param name="phKod">Performans hedefin programatik kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="fpTur">Faaliyet/projenin türü, ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır ya da 0 olabilir</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        private static void FaaliyetProjeToplamlariAl(TABRaporlar tRapor, TNSCollection objeler, int yil, string phkurKod, int phKod, int fpKod, int fpTur, string odenekSenaryo, int yansiyan)
        {
            IDataReader dr = tRapor.FaaliyetProjeToplam(yil, phkurKod, odenekSenaryo, phKod, fpKod, fpTur, yansiyan); //???
            if (dr != null)
            {
                while (dr.Read())
                {
                    FaaliyetProje gelen = new FaaliyetProje();
                    gelen.yil = yil;
                    gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                    gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil + "-" + gelen.kurKod, true, "");
                    gelen.fpKod = VeriErisim.DegerAl(dr, 3, 0);
                    gelen.fpAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    gelen.projeMi = VeriErisim.DegerAl(dr, 5, 0);
                    gelen.tur = VeriErisim.DegerAl(dr, 6, 0);

                    if (fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumIci && fpTur != (int)ENUMFaaliyetProjeTur.DigerKurumDisi)
                    {
                        //Performans Hedefin ağırlığı
                        FaaliyetProjeAgirlik fpa = new FaaliyetProjeAgirlik();
                        fpa.perHedefKod = VeriErisim.DegerAl(dr, 7, 0);
                        fpa.agirlik = VeriErisim.DegerAl(dr, 8, (double)0);
                        gelen.fpAgirlik.Add(fpa);

                        //Faaliyet Proje Ödenek
                        FaaliyetProjeOdenek fpo = new FaaliyetProjeOdenek();
                        fpo.odenek = VeriErisim.DegerAl(dr, 9, (double)0);
                        gelen.odenekler.Add(fpo);
                    }
                    else
                    {
                        //Faaliyet Proje Ödenek
                        FaaliyetProjeOdenek fpo = new FaaliyetProjeOdenek();
                        fpo.odenek = VeriErisim.DegerAl(dr, 7, (double)0);
                        gelen.odenekler.Add(fpo);
                    }

                    objeler.Add(gelen);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Performans hedeflerine ait kimlik, ağırlık ve ödenek bilgilerini bulup gönderilen objeler değişkenine ekleyen yordam
        /// Parametre olarak gönderilen stratejik hedefin ödeneğini de hesaplar ve döndürür.
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="tRapor2">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">Performans hedef nesnelerinin ekleneceği nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Parametre olarak gönderilen stratejik hedefin ödeneği döndürülür.</returns>
        private static double PerformansHedefDagilimPerformansHedefleriAl(TABRaporlar tRapor, TABRaporlar tRapor2, TNSCollection objeler, int donemBYil, int strHedefKod, int yil, string kurKod, string odenekSenaryo)
        {
            double toplam = 0;
            IDataReader dr = tRapor.PerformansHedefToplam(donemBYil, strHedefKod, yil, odenekSenaryo, kurKod);
            while (dr.Read())
            {
                PerformansHedef gelen = new PerformansHedef();
                gelen.yil = yil;
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil + "-" + gelen.kurKod, true, "");
                gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);

                //Stratejik Hedefin ağırlığı
                PerformansHedefAgirlik pha = new PerformansHedefAgirlik();
                pha.strHedef.donemBYil = VeriErisim.DegerAl(dr, 4, 0);
                pha.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 5, 0);
                pha.agirlik = VeriErisim.DegerAl(dr, 6, (double)0);
                gelen.perAgirlik.Add(pha);

                gelen.odenek = VeriErisim.DegerAl(dr, 7, (double)0);

                //Faaliyet Projeler alınıyor
                PerformansHedefDagilimFaaliyetProjeleriAl(tRapor2, gelen.fpler, gelen.yil, gelen.kurKod, gelen.perHedefKod, odenekSenaryo, 0, -1);

                objeler.Add(gelen);

                //Stratejik Hedefin toplamı hesaplanıyor
                toplam += gelen.odenek * pha.agirlik / 100;
            }
            dr.Close();
            return toplam;
        }

        /// <summary>
        /// Stratejik hedefleri ve ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>StratejikHedefOdenek nesneleri listesi döndürülür.</returns>
        private static TNSCollection PerformansHedefDagilimStrHedefleriAl(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod)
        {
            ObjectArray oaHedef = StratejikHedefListele(kullanan, donemBYil, true);
            if (!oaHedef.sonuc.islemSonuc) throw new Exception(oaHedef.sonuc.hataStr);
            //Stratejik Hedefleri alıyor
            TNSCollection strHedefler = oaHedef.objeler;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);
            TABRaporlar tRapor2 = new TABRaporlar(ve2);

            //Stratejik Hedeflerin Ödeneklerini alıyor
            TNSCollection sholar = new TNSCollection();
            foreach (StratejikHedef hedef in strHedefler)
            {
                StratejikHedefOdenek sho = new StratejikHedefOdenek();
                sho.sHedef = hedef;

                //Performans Hedefler alınıyor ve stratejik hedefin toplamı dönüyor
                sho.odenek = PerformansHedefDagilimPerformansHedefleriAl(tRapor, tRapor2, sho.pHedefler, hedef.donemBYil, hedef.strHedefKod, yil, kurKod, senaryoKod);

                sholar.Add(sho);
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sholar;
        }

        /// <summary>
        /// Stratejik amaçları ve ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>StratejikAmacOdenek nesneleri listesi döndürülür.</returns>
        private static TNSCollection PerformansHedefDagilimStrAmaclariAl(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod)
        {
            //Stratejik Amaçları alıyor
            TNSCollection strAmaclar = StratejikAmacListele(kullanan, donemBYil);

            //Stratejik Hedefler alınıyor
            TNSCollection sholar = PerformansHedefDagilimStrHedefleriAl(veriErisim, kullanan, donemBYil, yil, senaryoKod, kurKod);

            //Stratejik Amaçların Ödeneklerini alıyor
            TNSCollection saolar = new TNSCollection();
            foreach (StratejikAmac amac in strAmaclar)
            {
                StratejikAmacOdenek sao = new StratejikAmacOdenek();
                sao.sAmac = amac;

                //Stratejik Hedefler Stratejik Amaç'a ekleniyor ve stratejik amacın toplamı hesaplanıyor
                foreach (StratejikHedefOdenek sho in sholar)
                {
                    if (sho.sHedef.strAmac.strAmacKod == sao.sAmac.strAmacKod)
                    {
                        sao.odenek += sho.odenek;
                        sao.sHedefler.Add(sho);
                    }
                }

                saolar.Add(sao);
            }

            return saolar;
        }

        /// <summary>
        /// Temaları ve ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>TemaOdenek nesneleri listesi döndürülür.</returns>
        private static TNSCollection PerformansHedefDagilimTemalariAl(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod)
        {
            //Temaları alıyor
            TNSCollection temalar = TemaListele(kullanan, donemBYil);

            //Stratejik Amaçlar alınıyor
            TNSCollection saolar = PerformansHedefDagilimStrAmaclariAl(veriErisim, kullanan, donemBYil, yil, senaryoKod, kurKod);

            //Temaların Ödeneklerini alıyor
            TNSCollection tolar = new TNSCollection();
            foreach (Tema tema in temalar)
            {
                TemaOdenek to = new TemaOdenek();
                to.tema = tema;

                //Stratejik Amaçlar Tema'ya ekleniyor ve temanın toplamı hesaplanıyor
                foreach (StratejikAmacOdenek sao in saolar)
                {
                    if (sao.sAmac.tema.temaKod == to.tema.temaKod)
                    {
                        to.odenek += sao.odenek;
                        to.sAmaclar.Add(sao);
                    }
                }

                tolar.Add(to);
            }

            return tolar;
        }

        /// <summary>
        /// Verilen kriterlere uyan temaları, stratejik amaçları, stratejik hedefleri,
        /// performans hedeflerini, faaliyet/projeleri ve ödeneklerini toparlayıp döndüren yordam
        /// Ödeneklerin toplamı ve birim bazında dağılımı gösteriliyor ayrı ayrı.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TemaOdenek nesneleri listesi döndürülür.</returns>
        public static ObjectArray PerformansHedefDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod)
        {
            ObjectArray phdVAN = new ObjectArray();

            phdVAN.objeler = PerformansHedefDagilimTemalariAl(veriErisim, kullanan, donemBYil, yil, senaryoKod, kurKod);
            if (phdVAN.objeler.Count == 0)
                phdVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            return phdVAN;
        }

        /// <summary>
        /// Verilen performans hedefin bilgilerini, destekleyen faaliyet/proje ağırlıklarını
        /// ve ödeneklerini, bağlı olan performans göstergelerini ve değerlerini,
        /// bağlı olduğu stratejik hedefleri ve ağırlıklarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedefi bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public static ObjectArray PerformansHedefKartiListele(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef hedef, string odenekSenaryo, int gercekYil, int fpTur, int yansiyan)
        {
            ObjectArray hedefVAN = new ObjectArray();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(hedef.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (hedef.perHedefKod <= 0)
                hata += PerformansButceResI.PEB093 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                hedefVAN.sonuc.islemSonuc = false;
                hedefVAN.sonuc.hataStr = hata;
                return hedefVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);
            TABRaporlar tRapor2 = new TABRaporlar(ve2);

            IDataReader dr = tRapor.PerformansHedefKartiListele(hedef);
            PerformansHedef gelen = null;
            string eskiKurKod = string.Empty;
            int eskiPHKod = 0;
            if (dr != null)
            {
                while (dr.Read())
                {
                    if (gelen == null || eskiKurKod != VeriErisim.DegerAl(dr, 1, string.Empty) || eskiPHKod != VeriErisim.DegerAl(dr, 2, 0))
                    {
                        gelen = new PerformansHedef();
                        gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                        gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                        gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                        gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                        gelen.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                        gelen.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
                        gelen.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);
                        gelen.perHedefGorKod = VeriErisim.DegerAl(dr, 8, string.Empty);

                        eskiKurKod = gelen.kurKod;
                        eskiPHKod = gelen.perHedefKod;

                        //Faaliyet Proje toplamı ve ağırlıklar ekleniyor
                        PerformansHedefDagilimFaaliyetProjeleriAl(tRapor2, gelen.fpler, gelen.yil, gelen.kurKod, gelen.perHedefKod, odenekSenaryo, fpTur, yansiyan);

                        hedefVAN.objeler.Add(gelen);
                    }

                    //Metrik Hedeflenen Değerleri alınıyor
                    PerformansGostergeHedeflenen hedeflenen = new PerformansGostergeHedeflenen();
                    hedeflenen.yil = gelen.yil;
                    hedeflenen.phKurKod = gelen.kurKod;
                    hedeflenen.ghKurKod = gelen.kurKod;
                    hedeflenen.perHedefKod = gelen.perHedefKod;
                    string senaryoIki;
                    if (odenekSenaryo.Length > 2)
                        senaryoIki = odenekSenaryo.Substring(0, 2);
                    else
                        senaryoIki = odenekSenaryo;

                    hedeflenen.asama =  OrtakFonksiyonlar.ConvertToInt(senaryoIki, 6);
                    ObjectArray gostergeHedef = PEBGostergeHedef.Listele(ve2, kullanan, hedeflenen);
                    if (!gostergeHedef.sonuc.islemSonuc)
                    {
                        hedefVAN.sonuc = gostergeHedef.sonuc;
                        break;
                    }
                    else
                        gelen.perGostergeHedeflenen = gostergeHedef.objeler;

                    //Stratejik Hedef ve ağırlıklar ekleniyor
                    PerformansHedefAgirlik pha = new PerformansHedefAgirlik();
                    pha.strHedef.donemBYil = VeriErisim.DegerAl(dr, 5, 0);
                    pha.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 6, 0);
                    pha.strHedef.dil = new Dil(1, "");
                    pha.strHedef.kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
                    //Stratejik Amaç bilgileri de eklensin diye
                    ObjectArray oaHedef = StratejikHedefListele(kullanan, pha.strHedef, true);
                    if (!oaHedef.sonuc.islemSonuc) return oaHedef;

                    TNSCollection sHedefler = oaHedef.objeler;
                    if (sHedefler != null && sHedefler.Count > 0)
                        pha.strHedef = (StratejikHedef)sHedefler[0];
                    pha.agirlik = VeriErisim.DegerAl(dr, 7, (double)0);

                    gelen.perAgirlik.Add(pha);
                }
                dr.Close();
                if (hedefVAN.objeler.Count == 0)
                    hedefVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;
            }
            else
                hedefVAN.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return hedefVAN;
        }

        /// <summary>
        /// Verilen faaliyet/projenin bilgilerini, bağlı olduğu performans hedeflerini
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public static ObjectArray FaaliyetProjeKartiListele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProje fp, string odenekSenaryo)
        {
            ObjectArray fpVAN = new ObjectArray();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(fp.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (fp.fpKod <= 0)
                hata += PerformansButceResI.PEB037 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                fpVAN.sonuc.islemSonuc = false;
                fpVAN.sonuc.hataStr = hata;
                return fpVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
            TABRaporlar tRapor = new TABRaporlar(ve2);

            IDataReader dr = tFP.Listele(fp, 0);
            while (dr.Read())
            {
                FaaliyetProje gelen = new FaaliyetProje();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.fpKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.fpAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.projeMi = VeriErisim.DegerAl(dr, 4, 0);
                gelen.sorumlu = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.seviye = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.tur = VeriErisim.DegerAl(dr, 7, 0);
                gelen.ebutceKod = VeriErisim.DegerAl(dr, 8, string.Empty);

                ObjectArray fpAgirlik = PEBFaaliyetProjeAgirlik.Listele(ve2, kullanan, gelen.yil, gelen.kurKod, gelen.fpKod);
                if (!fpAgirlik.sonuc.islemSonuc)
                {
                    fpVAN.sonuc = fpAgirlik.sonuc;
                    break;
                }
                else
                    gelen.fpAgirlik = fpAgirlik.objeler;

                FaaliyetProjeKartiOdenekleriAl(tRapor, gelen.odenekler, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.fpKod, (int)ENUMAbsKodSinif.EKOKOD, 0);//???
                FaaliyetProjeKartiOdenekleriAl(tRapor, gelen.odenekler, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.fpKod, (int)ENUMAbsKodSinif.FONKOD, 0);//
                FaaliyetProjeKartiOdenekleriAl(tRapor, gelen.odenekler, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.fpKod, (int)ENUMAbsKodSinif.FINKOD, 0);//

                fpVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (fpVAN.objeler.Count == 0)
                fpVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return fpVAN;
        }

        /// <summary>
        /// İstenen faaliyet/projenin ödeneklerini bulup objeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">TertipOdenek nesnelerinin ekleneceği nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ENUMAbsKodSinif türünün uzunluğu</param>
        private static void FaaliyetProjeKartiOdenekleriAl(TABRaporlar tRapor, TNSCollection objeler, int yil, string kurKod, string odenekSenaryo, int fpKod, int absKodSinif, int uzunluk)
        {
            IDataReader dr = tRapor.FaaliyetProjeOdenekleri(yil, kurKod, odenekSenaryo, fpKod, absKodSinif, uzunluk);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipOdenek gelen = new TertipOdenek(yil);
                    gelen.tertip.kodlar[absKodSinif - 1] = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.tertip.Noktala();
                    gelen.tertip.AdlariBul(servisUZY);
                    gelen.odenek = VeriErisim.DegerAl(dr, 6, (double)0);

                    objeler.Add(gelen);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Verilen sabit maliyetin bilgilerini, ödeneklerinin yansıtıldığı faaliyet/projeleri
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        public static ObjectArray SabitMaliyetKartiListele(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyet sm, string odenekSenaryo)
        {
            ObjectArray smVAN = new ObjectArray();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sm.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (sm.smKod <= 0)
                hata += PerformansButceResI.PEB098 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smVAN.sonuc.islemSonuc = false;
                smVAN.sonuc.hataStr = hata;
                return smVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
            TABRaporlar tRapor = new TABRaporlar(ve2);

            IDataReader dr = tSM.Listele(sm);
            while (dr.Read())
            {
                SabitMaliyet gelen = new SabitMaliyet();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.smKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.smAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.tur = VeriErisim.DegerAl(dr, 4, 0);
                gelen.perHedef.kurKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.perHedef.perHedefKod = VeriErisim.DegerAl(dr, 6, 0);
                gelen.perHedef.perHedef = VeriErisim.DegerAl(dr, 7, string.Empty);

                //Ağırlıklar alınıyor
                SabitMaliyetAgirlik sma = new SabitMaliyetAgirlik();
                sma.yil = gelen.yil;
                sma.smKurKod = gelen.kurKod;
                sma.smKod = gelen.smKod;
                ObjectArray smAgirlik = PEBSabitMaliyetAgirlik.Listele(ve2, kullanan, sma);
                if (!smAgirlik.sonuc.islemSonuc)
                {
                    smVAN.sonuc = smAgirlik.sonuc;
                    break;
                }
                else
                    gelen.smAgirlik = smAgirlik.objeler;

                SabitMaliyetKartiOdenekleriAl(tRapor, gelen.smOdenek, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.smKod, (int)ENUMAbsKodSinif.EKOKOD, 0);
                SabitMaliyetKartiOdenekleriAl(tRapor, gelen.smOdenek, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.smKod, (int)ENUMAbsKodSinif.FONKOD, 0);
                SabitMaliyetKartiOdenekleriAl(tRapor, gelen.smOdenek, gelen.yil, gelen.kurKod, odenekSenaryo, gelen.smKod, (int)ENUMAbsKodSinif.FINKOD, 0);

                smVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (smVAN.objeler.Count == 0)
                smVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return smVAN;
        }

        /// <summary>
        /// İstenen sabit maliyetin ödeneklerini bulup objeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">TertipOdenek nesnelerinin ekleneceği nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ENUMAbsKodSinif türünün uzunluğu</param>
        private static void SabitMaliyetKartiOdenekleriAl(TABRaporlar tRapor, TNSCollection objeler, int yil, string kurKod, string odenekSenaryo, int smKod, int absKodSinif, int uzunluk)
        {
            IDataReader dr = tRapor.SabitMaliyetOdenekleri(yil, kurKod, odenekSenaryo, smKod, 0, absKodSinif, uzunluk);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipOdenek gelen = new TertipOdenek(yil);
                    gelen.tertip.kodlar[absKodSinif - 1] = VeriErisim.DegerAl(dr, 3, string.Empty);
                    gelen.tertip.Noktala();
                    gelen.tertip.AdlariBul(servisUZY);
                    gelen.odenek = VeriErisim.DegerAl(dr, 4, (double)0);

                    objeler.Add(gelen);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amacı veya amaçları ve ödeneklerini bulup döndüren yordam
        /// Performans bütçede yer almayan ödenekler de bulunur ve listeye eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PEBOzetRapor nesnesi döndürülür.</returns>
        public static ObjectArray PEBOzetRaporla(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int strAmacKod, int yil, string odenekSenaryo)
        {
            ObjectArray odenekVAN = new ObjectArray();
            PEBOzetRapor ozetRapor = new PEBOzetRapor();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            //Stratejik Hedefleri alıyor
            StratejikHedef hedefKriter = new StratejikHedef();
            hedefKriter.donemBYil = donemBYil;
            hedefKriter.dil.kod = 1;
            hedefKriter.strAmac.strAmacKod = strAmacKod;
            ObjectArray oaHedef = StratejikHedefListele(kullanan, hedefKriter, true);
            if (!oaHedef.sonuc.islemSonuc) return oaHedef;

            TNSCollection strHedefler = oaHedef.objeler;

            StratejikAmacOdenek sao = null;
            foreach (StratejikHedef hedef in strHedefler)
            {
                if (sao == null || sao.sAmac.strAmacKod != hedef.strAmac.strAmacKod)
                {
                    sao = new StratejikAmacOdenek();
                    sao.sAmac = hedef.strAmac;
                    ozetRapor.sAmaclar.Add(sao);
                }
                sao.odenek += tRapor.PEBOzetRaporla(hedef.donemBYil, hedef.strHedefKod, yil, odenekSenaryo);
            }

            //PEBOzetFaaliyetProjeleriAl(tRapor, ozetRapor.fpler, yil, asama);
            ozetRapor.digerKurumIci = tRapor.DigerFaaliyetProjeToplam(yil, odenekSenaryo, (int)ENUMFaaliyetProjeTur.DigerKurumIci);
            ozetRapor.digerKurumDisi = tRapor.DigerFaaliyetProjeToplam(yil, odenekSenaryo, (int)ENUMFaaliyetProjeTur.DigerKurumDisi);

            if (ozetRapor.sAmaclar.Count > 0 || ozetRapor.digerKurumIci > 0 || ozetRapor.digerKurumDisi > 0)
                odenekVAN.objeler.Add(ozetRapor);

            if (odenekVAN.objeler.Count == 0)
                odenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return odenekVAN;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        public static ObjectArray StrHedefOdenekEkoDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            return StrHedefOdenekDagilimRaporla(veriErisim, kullanan, yil, senaryoKod, (int)ENUMAbsKodSinif.EKOKOD, 2);
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        public static ObjectArray StrHedefOdenekFonDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            return StrHedefOdenekDagilimRaporla(veriErisim, kullanan, yil, senaryoKod, (int)ENUMAbsKodSinif.FONKOD, 2);
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ENUMAbsKodSinif türünün uzunluğu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        private static ObjectArray StrHedefOdenekDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int absKodSinif, int uzunluk)
        {
            ObjectArray odenekVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            StrHedefOdenekDagilimRaporla(tRapor, odenekVAN.objeler, yil, senaryoKod, false, absKodSinif, uzunluk);
            StrHedefOdenekDagilimRaporla(tRapor, odenekVAN.objeler, yil, senaryoKod, true, absKodSinif, uzunluk);

            if (odenekVAN.objeler.Count == 0)
                odenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return odenekVAN;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve ödeneklerini bulup gönderilen objeler değişkeninde
        /// varolan bir stratejik hedefe ait bilgiler ise ilgili nesneye ekleyen, değilse yeni nesne oluşturup
        /// bilgileri yeni nesneye ekleyen ve yeni nesneyi de objeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="tRapor">Raporlarla ilgili sql işlemlerini yapan yordamları barındıran nesne</param>
        /// <param name="objeler">StratejikHedefOdenekDagilim nesnelerinin ekleneceği nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="yansitilan">İstenilen ödeneğin yansıtılan ödenek mi yoksa kendisine ait ödenek mi olduğu bilgisi</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ENUMAbsKodSinif türünün uzunluğu</param>
        private static void StrHedefOdenekDagilimRaporla(TABRaporlar tRapor, TNSCollection objeler, int yil, string senaryoKod, bool yansitilan, int absKodSinif, int uzunluk)
        {
            int arrayIndex = -1;
            if (absKodSinif == (int)ENUMAbsKodSinif.FONKOD)
                arrayIndex = 1;
            else if (absKodSinif == (int)ENUMAbsKodSinif.EKOKOD)
                arrayIndex = 3;

            IDataReader dr = tRapor.StrHedefOdenekDagilimRaporla(yil, senaryoKod, yansitilan, absKodSinif, uzunluk);

            StratejikHedefOdenekDagilim gelen = null;
            while (dr.Read())
            {
                if (objeler.Count > 0)
                    gelen = StratejikHedefBul(VeriErisim.DegerAl(dr, 0, 0), VeriErisim.DegerAl(dr, 1, 0), objeler);

                if (gelen == null)
                {
                    gelen = new StratejikHedefOdenekDagilim();
                    gelen.sHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.sHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                    gelen.sHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.sHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + gelen.sHedef.strHedefKod.ToString() + "-" + "1", true, "");

                    objeler.Add(gelen);
                }

                TertipOdenek to = new TertipOdenek(yil);

                if (yansitilan)
                    to.odenek = VeriErisim.DegerAl(dr, 2, (double)0);
                else
                {
                    to.tertip.kodlar[arrayIndex] = VeriErisim.DegerAl(dr, 2, string.Empty);
                    to.odenek = VeriErisim.DegerAl(dr, 3, (double)0);
                }

                gelen.odenekler.Add(to);
            }

            dr.Close();
        }

        /// <summary>
        /// StratejikHedefOdenekDagilim nesneleri listesinden aranan nesneyi bulan yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="objeler">StratejikHedefOdenekDagilim nesneleri listesini tutan nesne</param>
        /// <returns>İlgili StratejikHedefOdenekDagilim nesnesi bulunursa nesne, bulunamazsa null döndürülür.</returns>
        private static StratejikHedefOdenekDagilim StratejikHedefBul(int donemBYil, int strHedefKod, TNSCollection objeler)
        {
            foreach (StratejikHedefOdenekDagilim sho in objeler)
                if (donemBYil == sho.sHedef.donemBYil && strHedefKod == sho.sHedef.strHedefKod)
                    return sho;

            return null;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        public static ObjectArray StrAmacOdenekEkoDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            return StrAmacOdenekDagilimRaporla(veriErisim, kullanan, yil, senaryoKod, (int)ENUMAbsKodSinif.EKOKOD, 2);
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        public static ObjectArray StrAmacOdenekFonDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod)
        {
            return StrAmacOdenekDagilimRaporla(veriErisim, kullanan, yil, senaryoKod, (int)ENUMAbsKodSinif.FONKOD, 2);
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="absKodSinif">ENUMAbsKodSinif listesindeki türlerden biri olmalıdır.</param>
        /// <param name="uzunluk">İstenen ENUMAbsKodSinif türünün uzunluğu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        private static ObjectArray StrAmacOdenekDagilimRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int absKodSinif, int uzunluk)
        {
            ObjectArray odenekVAN = new ObjectArray();

            ObjectArray strHedefOdenekleri = null;
            if (absKodSinif == (int)ENUMAbsKodSinif.EKOKOD)
                strHedefOdenekleri = StrHedefOdenekEkoDagilimRaporla(veriErisim, kullanan, yil, senaryoKod);
            else if (absKodSinif == (int)ENUMAbsKodSinif.FONKOD)
                strHedefOdenekleri = StrHedefOdenekFonDagilimRaporla(veriErisim, kullanan, yil, senaryoKod);

            if (strHedefOdenekleri.sonuc.islemSonuc && strHedefOdenekleri.objeler.Count > 0)
            {
                int donemBYil = ((StratejikHedefOdenekDagilim)strHedefOdenekleri.objeler[0]).sHedef.donemBYil;
                ObjectArray oaHedef = StratejikHedefListele(kullanan, donemBYil, true);
                if (!oaHedef.sonuc.islemSonuc) return oaHedef;

                TNSCollection sHedefler = oaHedef.objeler;

                StratejikAmacOdenekDagilim strAmacOdenek = null;
                foreach (StratejikHedefOdenekDagilim sho in strHedefOdenekleri.objeler)
                {
                    StratejikHedef sh = null;
                    foreach (StratejikHedef sHedef in sHedefler)
                    {
                        if (sHedef.donemBYil == sho.sHedef.donemBYil && sHedef.strHedefKod == sho.sHedef.strHedefKod)
                        {
                            sh = sHedef;
                            break;
                        }
                    }

                    if (sh != null)
                    {
                        if (odenekVAN.objeler.Count > 0)
                            strAmacOdenek = StratejikAmacBul(sh.donemBYil, sh.strAmac.strAmacKod, odenekVAN.objeler);

                        if (strAmacOdenek == null)
                        {
                            strAmacOdenek = new StratejikAmacOdenekDagilim();
                            strAmacOdenek.sAmac = sh.strAmac;
                            strAmacOdenek.sAmac.donemBYil = donemBYil;
                            odenekVAN.objeler.Add(strAmacOdenek);
                        }

                        TertipOdenek tOdenek = null;
                        foreach (TertipOdenek to in sho.odenekler)
                        {
                            if (strAmacOdenek.odenekler.Count > 0)
                                tOdenek = StratejikAmacTertipOdenekBul(to, strAmacOdenek.odenekler);

                            if (tOdenek == null)
                            {
                                tOdenek = to;
                                strAmacOdenek.odenekler.Add(tOdenek);
                            }
                            else
                                tOdenek.odenek += to.odenek;
                        }
                    }
                }
            }

            if (odenekVAN.objeler.Count == 0)
                odenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            return odenekVAN;
        }

        /// <summary>
        /// StratejikAmacOdenekDagilim nesneleri listesinden aranan nesneyi bulan yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amacın programatik kodu</param>
        /// <param name="objeler">StratejikAmacOdenekDagilim nesneleri listesini tutan nesne</param>
        /// <returns>İlgili StratejikAmacOdenekDagilim nesnesi bulunursa nesne, bulunamazsa null döndürülür.</returns>
        private static StratejikAmacOdenekDagilim StratejikAmacBul(int donemBYil, int strAmacKod, TNSCollection objeler)
        {
            foreach (StratejikAmacOdenekDagilim sao in objeler)
                if (donemBYil == sao.sAmac.donemBYil && strAmacKod == sao.sAmac.strAmacKod)
                    return sao;

            return null;
        }

        /// <summary>
        /// TertipOdenek nesneleri listesinden aranan nesneyi bulan yordam
        /// </summary>
        /// <param name="tOdenek">Aranan tertip ödenek bilgilerini tutan nesne</param>
        /// <param name="objeler">TertipOdenek nesneleri listesini tutan nesne</param>
        /// <returns>İlgili TertipOdenek nesnesi bulunursa nesne, bulunamazsa null döndürülür.</returns>
        private static TertipOdenek StratejikAmacTertipOdenekBul(TertipOdenek tOdenek, TNSCollection objeler)
        {
            foreach (TertipOdenek to in objeler)
                if (to.tertip.TertipStrOl() == tOdenek.tertip.TertipStrOl())
                    return to;

            return null;
        }

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini ve ödeneklerini toplayıp döndüren yordam
        /// Bu raporda, performans göstergeleri hedeflenen değerler giriş ekranında sadece ilk sıraya girilen
        /// performans göstergeleri listeleniyor ve çıktıda performans hedefleri olarak görüntüleniyor.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public static ObjectArray PerformansGostergeOdenekRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kurKod)
        {
            ObjectArray perGostergeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.PerformansGostergeOdenekToplam(yil, senaryoKod, kurKod);
            while (dr.Read())
            {
                PerformansGosterge gelen = new PerformansGosterge();
                gelen.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.gostergeAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.olcuBirim.kod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.olcuBirim.ad = PEBServis.UzayYaziDegerGetir("PEBOLCUBIRIM", gelen.olcuBirim.kod.ToString(), true, "");
                gelen.tur = (ENUMGostergeTur)VeriErisim.DegerAl(dr, 3, 0);
                gelen.ondalikSayisi = VeriErisim.DegerAl(dr, 4, 0);
                gelen.seviye = VeriErisim.DegerAl(dr, 5, 0);
                gelen.odenek = VeriErisim.DegerAl(dr, 6, (double)0);

                perGostergeVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (perGostergeVAN.objeler.Count == 0)
                perGostergeVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return perGostergeVAN;
        }

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        /// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        public static ObjectArray BirimGostergeleriRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        {
            ObjectArray bgVAN = new ObjectArray();

            string hata = string.Empty;
            if (string.IsNullOrEmpty(kurKod))
                hata = PerformansButceResI.PEB005 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                bgVAN.sonuc.islemSonuc = false;
                bgVAN.sonuc.hataStr = hata;
                return bgVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.BirimGostergeleriRaporla(yil, kurKod, gostergeSeviye);
            BirimGostergeleriRaporu gelen = null;
            PerformansHedef ph = null;
            PerformansGostergeHedeflenenDegerler pghd = null;
            while (dr.Read())
            {
                //Stratejik Hedef değişince yeni obje yaratılıyor
                if (gelen == null || gelen.strHedef.donemBYil != VeriErisim.DegerAl(dr, 0, 0) || gelen.strHedef.strHedefKod != VeriErisim.DegerAl(dr, 1, 0))
                {
                    gelen = new BirimGostergeleriRaporu();
                    gelen.strHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);

                    gelen.strHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + gelen.strHedef.strHedefKod.ToString() + "-" + "1", true, "");
                    gelen.strHedef.strAmac.strAmacKod = (int)PEBServis.UzaySayiDegerGetir("STRHEDEFAMACKOD", gelen.strHedef.donemBYil.ToString() + "-" + gelen.strHedef.strHedefKod.ToString(), true, "");
                    gelen.strHedef.strAmac.strAmac = PEBServis.UzayYaziDegerGetir("STRYAZI", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + gelen.strHedef.strAmac.strAmacKod.ToString() + "-" + "1", true, "");

                    bgVAN.objeler.Add(gelen);

                    ph = null;
                }

                //Performans Hedefi değişince yeni obje yaratılıyor
                if (ph == null || ph.perHedefKod != VeriErisim.DegerAl(dr, 2, 0))
                {
                    ph = new PerformansHedef();
                    ph.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
                    ph.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
                    ph.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);

                    gelen.perHedefler.Add(ph);

                    pghd = null;
                }

                //Performans Göstergesi değişince yeni obje yaratılıyor
                if (pghd == null || pghd.gosterge.gostergeKod != VeriErisim.DegerAl(dr, 5, 0))
                {
                    pghd = new PerformansGostergeHedeflenenDegerler();
                    pghd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 5, 0);
                    pghd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    pghd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 7, 0);
                    pghd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 8, string.Empty);
                    pghd.miktar[0] = VeriErisim.DegerAl(dr, 9, (double)0);
                    pghd.siraNo = VeriErisim.DegerAl(dr, 10, 0);

                    ph.perGostergeHedeflenen.Add(pghd);
                }

                if (VeriErisim.DegerAl(dr, 11, 0) != 0)
                {
                    PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
                    PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
                    pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 5, 0);
                    pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
                    pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 7, 0);
                    pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 8, string.Empty);
                    pggd.miktar = VeriErisim.DegerAl(dr, 12, (double)0);

                    pgg.ay = VeriErisim.DegerAl(dr, 11, 0);
                    pgg.degerler.Add(pggd);

                    ph.perGostergeGerceklesen.Add(pgg);
                }
            }
            dr.Close();

            if (bgVAN.objeler.Count == 0)
                bgVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return bgVAN;
        }

        /// <summary>
        /// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        /// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        public static ObjectArray PerformansGostergeDetayRaporla(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge)
        {
            ObjectArray phVAN = new ObjectArray();

            string hata = string.Empty;
            if (pHedef.yil <= 0)
                hata = PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(pHedef.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (pHedef.perHedefKod <= 0)
                hata += PerformansButceResI.PEB069 + "<br>";
            if (pGosterge.gostergeKod <= 0)
                hata += PerformansButceResI.PEB099 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                phVAN.sonuc.islemSonuc = false;
                phVAN.sonuc.hataStr = hata;
                return phVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.PerformansGostergeDetayRaporla(pHedef, pGosterge);
            PerformansHedef ph = new PerformansHedef();
            PerformansGostergeHedeflenenDegerler pghd = new PerformansGostergeHedeflenenDegerler();
            PerformansHedefAgirlik pha = null;
            while (dr.Read())
            {
                //Stratejik Hedef değişince yeni obje yaratılıyor
                if (pha == null || pha.strHedef.donemBYil != VeriErisim.DegerAl(dr, 0, 0) || pha.strHedef.strHedefKod != VeriErisim.DegerAl(dr, 1, 0))
                {
                    pha = new PerformansHedefAgirlik();
                    pha.strHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                    pha.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                    pha.agirlik = VeriErisim.DegerAl(dr, 2, (double)0);

                    pha.strHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZI", pha.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + pha.strHedef.strHedefKod.ToString() + "-" + "1", true, "");
                    pha.strHedef.strAmac.strAmacKod = (int)PEBServis.UzaySayiDegerGetir("STRHEDEFAMACKOD", pha.strHedef.donemBYil.ToString() + "-" + pha.strHedef.strHedefKod.ToString(), true, "");
                    pha.strHedef.strAmac.strAmac = PEBServis.UzayYaziDegerGetir("STRYAZI", pha.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + pha.strHedef.strAmac.strAmacKod.ToString() + "-" + "1", true, "");

                    ph.perAgirlik.Add(pha);
                }

                ph.perHedefKod = VeriErisim.DegerAl(dr, 3, 0);
                ph.perHedef = VeriErisim.DegerAl(dr, 4, string.Empty);
                ph.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);

                pghd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 6, 0);
                pghd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                pghd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 8, 0);
                pghd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 9, string.Empty);
                pghd.miktar[0] = VeriErisim.DegerAl(dr, 10, (double)0);
                pghd.siraNo = VeriErisim.DegerAl(dr, 11, 0);
                pghd.aciklama[0] = VeriErisim.DegerAl(dr, 12, string.Empty);
                pghd.gosterge.ilerlemeYonu = (ENUMGostergeIlerlemeYonu)VeriErisim.DegerAl(dr, 19, 0);

                if (VeriErisim.DegerAl(dr, 13, 0) != 0)
                {
                    PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
                    PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
                    pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 6, 0);
                    pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 7, string.Empty);
                    pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 8, 0);
                    pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 9, string.Empty);
                    pggd.miktar = VeriErisim.DegerAl(dr, 14, (double)0);
                    pggd.kaynak = VeriErisim.DegerAl(dr, 15, string.Empty);
                    pggd.sonuc = VeriErisim.DegerAl(dr, 16, string.Empty);
                    pggd.sapma = VeriErisim.DegerAl(dr, 17, string.Empty);
                    pggd.onlem = VeriErisim.DegerAl(dr, 18, string.Empty);

                    pgg.ay = VeriErisim.DegerAl(dr, 13, 0);
                    pgg.degerler.Add(pggd);

                    ph.perGostergeGerceklesen.Add(pgg);
                }
            }
            dr.Close();

            if (pghd.gosterge.gostergeKod > 0)
            {
                ph.perGostergeHedeflenen.Add(pghd);
                phVAN.objeler.Add(ph);
            }

            if (phVAN.objeler.Count == 0)
                phVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return phVAN;
        }

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="perHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        public static ObjectArray PerformansGostergeRaporla(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef perHedef, int[] gostergeSeviye)
        {
            ObjectArray pgVAN = new ObjectArray();

            string hata = string.Empty;
            if (perHedef.yil <= 0)
                hata = PerformansButceResI.PEB039 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                pgVAN.sonuc.islemSonuc = false;
                pgVAN.sonuc.hataStr = hata;
                return pgVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.PerformansGostergeRaporla(perHedef, gostergeSeviye);
            while (dr.Read())
            {
                PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
                PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
                pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
                pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 2, 0);
                pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 3, string.Empty);
                pggd.gosterge.ilerlemeYonu = (ENUMGostergeIlerlemeYonu)VeriErisim.DegerAl(dr, 15, 0);

                pgg.phKurKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                pgg.perHedefKod = VeriErisim.DegerAl(dr, 5, 0);
                pgg.ay = VeriErisim.DegerAl(dr, 9, 0);
                pgg.ggKurKod = VeriErisim.DegerAl(dr, 16, string.Empty);

                pggd.hedefMiktar[0] = VeriErisim.DegerAl(dr, 6, (double)0);
                pggd.miktar = VeriErisim.DegerAl(dr, 10, (double)0);
                pggd.veriYok = dr.IsDBNull(10);
                pggd.kaynak = VeriErisim.DegerAl(dr, 11, string.Empty);
                pggd.sonuc = VeriErisim.DegerAl(dr, 12, string.Empty);
                pggd.sapma = VeriErisim.DegerAl(dr, 13, string.Empty);
                pggd.onlem = VeriErisim.DegerAl(dr, 14, string.Empty);

                pgg.degerler.Add(pggd);
                pgVAN.objeler.Add(pgg);
            }
            dr.Close();

            if (pgVAN.objeler.Count == 0)
                pgVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return pgVAN;
        }

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        public static ObjectArray PerformansGostergeRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        {
            PerformansHedef perHedef = new PerformansHedef();
            perHedef.yil = yil;

            string[] parcali = kurKod.Split('-');
            if (!string.IsNullOrEmpty(parcali[0]))
                perHedef.kurKod = parcali[0];
            if (parcali.Length == 2 && !string.IsNullOrEmpty(parcali[1]))
                perHedef.perHedefKod = OrtakFonksiyonlar.ConvertToInt(parcali[1], 0);

            return PerformansGostergeRaporla(veriErisim, kullanan, perHedef, gostergeSeviye);
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve bağlı olan I. ve II. seviye performans göstergelerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="asama">Aşama kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public static ObjectArray StratejikGostergeRaporla(VeriErisim veriErisim, Kullanici kullanan, int donemBYil, int yil, int asama)
        {
            ObjectArray sgVAN = new ObjectArray();

            string hata = string.Empty;
            if (donemBYil <= 0)
                hata = PerformansButceResI.PEB100 + "<br>";
            if (yil <= 0)
                hata = PerformansButceResI.PEB039 + "<br>";
            if (asama < 0)
                hata = PerformansButceResI.PEB045 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sgVAN.sonuc.islemSonuc = false;
                sgVAN.sonuc.hataStr = hata;
                return sgVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);

            IDataReader dr = tRapor.StratejikGostergeRaporla(donemBYil, yil, asama);
            while (dr.Read())
            {
                PerformansGosterge gelen = new PerformansGosterge();
                gelen.strHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.gostergeKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.gostergeAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.olcuBirim.kod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.olcuBirim.ad = VeriErisim.DegerAl(dr, 5, string.Empty);

                sgVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (sgVAN.objeler.Count == 0)
                sgVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return sgVAN;
        }

        ///// <summary>
        ///// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        ///// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        //public static ObjectArray BirimGostergeleriRaporlaEskiDB(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        //{
        //    ObjectArray bgVAN = new ObjectArray();

        //    string hata = string.Empty;
        //    if (string.IsNullOrEmpty(kurKod))
        //        hata = PerformansButceResI.PEB005 + "<br>";
        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        bgVAN.sonuc.islemSonuc = false;
        //        bgVAN.sonuc.hataStr = hata;
        //        return bgVAN;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY");
        //    TABRaporlar tRapor = new TABRaporlar(ve);

        //    IDataReader dr = tRapor.BirimGostergeleriRaporla(yil, kurKod, gostergeSeviye);
        //    BirimGostergeleriRaporu gelen = null;
        //    PerformansHedef ph = null;
        //    PerformansGostergeHedeflenenDegerler pghd = null;
        //    while (dr.Read())
        //    {
        //        //Stratejik Hedef değişince yeni obje yaratılıyor
        //        if (gelen == null || gelen.strHedef.donemBYil != VeriErisim.DegerAl(dr, 0, 0) || gelen.strHedef.strHedefKod != VeriErisim.DegerAl(dr, 1, 0))
        //        {
        //            gelen = new BirimGostergeleriRaporu();
        //            gelen.strHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
        //            gelen.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);

        //            gelen.strHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZIY", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + gelen.strHedef.strHedefKod.ToString() + "-" + "1", true, "");
        //            gelen.strHedef.strAmac.strAmacKod = (int)PEBServis.UzaySayiDegerGetir("STRHEDEFAMACKODY", gelen.strHedef.donemBYil.ToString() + "-" + gelen.strHedef.strHedefKod.ToString(), true, "");
        //            gelen.strHedef.strAmac.strAmac = PEBServis.UzayYaziDegerGetir("STRYAZIY", gelen.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + gelen.strHedef.strAmac.strAmacKod.ToString() + "-" + "1", true, "");

        //            bgVAN.objeler.Add(gelen);

        //            ph = null;
        //        }

        //        //Performans Hedefi değişince yeni obje yaratılıyor
        //        if (ph == null || ph.perHedefKod != VeriErisim.DegerAl(dr, 2, 0))
        //        {
        //            ph = new PerformansHedef();
        //            ph.perHedefKod = VeriErisim.DegerAl(dr, 2, 0);
        //            ph.perHedef = VeriErisim.DegerAl(dr, 3, string.Empty);
        //            ph.aciklama = VeriErisim.DegerAl(dr, 4, string.Empty);

        //            gelen.perHedefler.Add(ph);

        //            pghd = null;
        //        }

        //        //Performans Göstergesi değişince yeni obje yaratılıyor
        //        if (pghd == null || pghd.gosterge.gostergeKod != VeriErisim.DegerAl(dr, 5, 0))
        //        {
        //            pghd = new PerformansGostergeHedeflenenDegerler();
        //            pghd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 5, 0);
        //            pghd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
        //            pghd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 7, 0);
        //            pghd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 8, string.Empty);
        //            pghd.miktar[0] = VeriErisim.DegerAl(dr, 9, (double)0);
        //            pghd.siraNo = VeriErisim.DegerAl(dr, 10, 0);

        //            ph.perGostergeHedeflenen.Add(pghd);
        //        }

        //        if (VeriErisim.DegerAl(dr, 11, 0) != 0)
        //        {
        //            PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
        //            PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
        //            pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 5, 0);
        //            pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 6, string.Empty);
        //            pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 7, 0);
        //            pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 8, string.Empty);
        //            pggd.miktar = VeriErisim.DegerAl(dr, 12, (double)0);

        //            pgg.ay = VeriErisim.DegerAl(dr, 11, 0);
        //            pgg.degerler.Add(pggd);

        //            ph.perGostergeGerceklesen.Add(pgg);
        //        }
        //    }
        //    dr.Close();

        //    if (bgVAN.objeler.Count == 0)
        //        bgVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return bgVAN;
        //}

        ///// <summary>
        ///// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        ///// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        ///// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        //public static ObjectArray PerformansGostergeDetayRaporlaEskiDB(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge)
        //{
        //    ObjectArray phVAN = new ObjectArray();

        //    string hata = string.Empty;
        //    if (pHedef.yil <= 0)
        //        hata = PerformansButceResI.PEB039 + "<br>";
        //    if (string.IsNullOrEmpty(pHedef.kurKod))
        //        hata += PerformansButceResI.PEB005 + "<br>";
        //    if (pHedef.perHedefKod <= 0)
        //        hata += PerformansButceResI.PEB069 + "<br>";
        //    if (pGosterge.gostergeKod <= 0)
        //        hata += PerformansButceResI.PEB099 + "<br>";
        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        phVAN.sonuc.islemSonuc = false;
        //        phVAN.sonuc.hataStr = hata;
        //        return phVAN;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY");
        //    TABRaporlar tRapor = new TABRaporlar(ve);

        //    IDataReader dr = tRapor.PerformansGostergeDetayRaporla(pHedef, pGosterge);
        //    PerformansHedef ph = new PerformansHedef();
        //    PerformansGostergeHedeflenenDegerler pghd = new PerformansGostergeHedeflenenDegerler();
        //    PerformansHedefAgirlik pha = null;
        //    while (dr.Read())
        //    {
        //        //Stratejik Hedef değişince yeni obje yaratılıyor
        //        if (pha == null || pha.strHedef.donemBYil != VeriErisim.DegerAl(dr, 0, 0) || pha.strHedef.strHedefKod != VeriErisim.DegerAl(dr, 1, 0))
        //        {
        //            pha = new PerformansHedefAgirlik();
        //            pha.strHedef.donemBYil = VeriErisim.DegerAl(dr, 0, 0);
        //            pha.strHedef.strHedefKod = VeriErisim.DegerAl(dr, 1, 0);
        //            pha.agirlik = VeriErisim.DegerAl(dr, 2, (double)0);

        //            pha.strHedef.strHedef = PEBServis.UzayYaziDegerGetir("STRYAZIY", pha.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikHedef).ToString() + "-" + pha.strHedef.strHedefKod.ToString() + "-" + "1", true, "");
        //            pha.strHedef.strAmac.strAmacKod = (int)PEBServis.UzaySayiDegerGetir("STRHEDEFAMACKODY", pha.strHedef.donemBYil.ToString() + "-" + pha.strHedef.strHedefKod.ToString(), true, "");
        //            pha.strHedef.strAmac.strAmac = PEBServis.UzayYaziDegerGetir("STRYAZIY", pha.strHedef.donemBYil.ToString() + "-" + ((int)ENUMStratejikPlan.StratejikAmac).ToString() + "-" + pha.strHedef.strAmac.strAmacKod.ToString() + "-" + "1", true, "");

        //            ph.perAgirlik.Add(pha);
        //        }

        //        ph.perHedefKod = VeriErisim.DegerAl(dr, 3, 0);
        //        ph.perHedef = VeriErisim.DegerAl(dr, 4, string.Empty);
        //        ph.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);

        //        pghd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 6, 0);
        //        pghd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 7, string.Empty);
        //        pghd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 8, 0);
        //        pghd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 9, string.Empty);
        //        pghd.miktar[0] = VeriErisim.DegerAl(dr, 10, (double)0);
        //        pghd.siraNo = VeriErisim.DegerAl(dr, 11, 0);
        //        pghd.aciklama[0] = VeriErisim.DegerAl(dr, 12, string.Empty);
        //        pghd.gosterge.ilerlemeYonu = VeriErisim.DegerAl(dr, 19, 0);

        //        if (VeriErisim.DegerAl(dr, 13, 0) != 0)
        //        {
        //            PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
        //            PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
        //            pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 6, 0);
        //            pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 7, string.Empty);
        //            pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 8, 0);
        //            pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 9, string.Empty);
        //            pggd.miktar = VeriErisim.DegerAl(dr, 14, (double)0);
        //            pggd.kaynak = VeriErisim.DegerAl(dr, 15, string.Empty);
        //            pggd.sonuc = VeriErisim.DegerAl(dr, 16, string.Empty);
        //            pggd.sapma = VeriErisim.DegerAl(dr, 17, string.Empty);
        //            pggd.onlem = VeriErisim.DegerAl(dr, 18, string.Empty);

        //            pgg.ay = VeriErisim.DegerAl(dr, 13, 0);
        //            pgg.degerler.Add(pggd);

        //            ph.perGostergeGerceklesen.Add(pgg);
        //        }
        //    }
        //    dr.Close();

        //    if (pghd.gosterge.gostergeKod > 0)
        //    {
        //        ph.perGostergeHedeflenen.Add(pghd);
        //        phVAN.objeler.Add(ph);
        //    }

        //    if (phVAN.objeler.Count == 0)
        //        phVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return phVAN;
        //}

        ///// <summary>
        ///// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="perHedef">Performans hedef bilgilerini tutan nesne</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        //private static ObjectArray PerformansGostergeRaporlaEskiDB(VeriErisim veriErisim, Kullanici kullanan, PerformansHedef perHedef, int[] gostergeSeviye)
        //{
        //    ObjectArray pgVAN = new ObjectArray();

        //    string hata = string.Empty;
        //    if (perHedef.yil <= 0)
        //        hata = PerformansButceResI.PEB039 + "<br>";
        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        pgVAN.sonuc.islemSonuc = false;
        //        pgVAN.sonuc.hataStr = hata;
        //        return pgVAN;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEBY");
        //    TABRaporlar tRapor = new TABRaporlar(ve);

        //    IDataReader dr = tRapor.PerformansGostergeRaporla(perHedef, gostergeSeviye);
        //    while (dr.Read())
        //    {
        //        PerformansGostergeGerceklesen pgg = new PerformansGostergeGerceklesen();
        //        PerformansGostergeGerceklesenDeger pggd = new PerformansGostergeGerceklesenDeger();
        //        pggd.gosterge.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
        //        pggd.gosterge.gostergeAd = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        pggd.gosterge.olcuBirim.kod = VeriErisim.DegerAl(dr, 2, 0);
        //        pggd.gosterge.olcuBirim.ad = VeriErisim.DegerAl(dr, 3, string.Empty);
        //        pggd.gosterge.ilerlemeYonu = VeriErisim.DegerAl(dr, 15, 0);

        //        pgg.phKurKod = VeriErisim.DegerAl(dr, 4, string.Empty);
        //        pgg.perHedefKod = VeriErisim.DegerAl(dr, 5, 0);
        //        pgg.ay = VeriErisim.DegerAl(dr, 9, 0);

        //        pggd.hedefMiktar[0] = VeriErisim.DegerAl(dr, 6, (double)0);
        //        pggd.miktar = VeriErisim.DegerAl(dr, 10, (double)0);
        //        pggd.kaynak = VeriErisim.DegerAl(dr, 11, string.Empty);
        //        pggd.sonuc = VeriErisim.DegerAl(dr, 12, string.Empty);
        //        pggd.sapma = VeriErisim.DegerAl(dr, 13, string.Empty);
        //        pggd.onlem = VeriErisim.DegerAl(dr, 14, string.Empty);

        //        pgg.degerler.Add(pggd);
        //        pgVAN.objeler.Add(pgg);
        //    }
        //    dr.Close();

        //    if (pgVAN.objeler.Count == 0)
        //        pgVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return pgVAN;
        //}

        ///// <summary>
        ///// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        //public static ObjectArray PerformansGostergeRaporlaEskiDB(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        //{
        //    PerformansHedef perHedef = new PerformansHedef();
        //    perHedef.yil = yil;
        //    perHedef.kurKod = kurKod;

        //    return PerformansGostergeRaporlaEskiDB(veriErisim, kullanan, perHedef, gostergeSeviye);
        //}

        public static ObjectArray GostergeGerceklesenHedefGosterge(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod, int ay)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tRapor = new TABRaporlar(ve);
            TNSCollection c = null;
            TNSCollection cg = null;
            string eskiPHKURKOD = null;
            int eskiPERHEDEFKOD = -1;
            int eskiGOSTERGEKOD = -1;

            IDataReader dr = tRapor.GostergeGerceklesenHedefGosterge(yil, kurKod, ay);
            while (dr.Read())
            {
                string PHKURKOD = VeriErisim.DegerAl(dr, 0, "");
                int PERHEDEFKOD = VeriErisim.DegerAl(dr, 1, 0);
                string HEDEF = VeriErisim.DegerAl(dr, 2, "");
                string HEDEF_ACIKLAMA = VeriErisim.DegerAl(dr, 3, "");

                if (eskiPHKURKOD != PHKURKOD || eskiPERHEDEFKOD != PERHEDEFKOD)
                {
                    eskiPHKURKOD = PHKURKOD; 
                    eskiPERHEDEFKOD = PERHEDEFKOD;
                    c = new TNSCollection();
                    o.Ekle(c);
                    c.Add(PHKURKOD);
                    c.Add(PERHEDEFKOD);
                    c.Add(HEDEF);
                    c.Add(HEDEF_ACIKLAMA);
                    eskiGOSTERGEKOD = -1;
                }
                int GOSTERGEKOD = VeriErisim.DegerAl(dr, 4, 0);

                if (eskiGOSTERGEKOD != GOSTERGEKOD)
                {
                    cg = new TNSCollection();
                    cg.Add(GOSTERGEKOD);

                    string GOSTERGEAD = VeriErisim.DegerAl(dr, 9, "");
                    cg.Add(GOSTERGEAD);

                    string OLCUBIRIMAD = VeriErisim.DegerAl(dr, 10, "");
                    cg.Add(OLCUBIRIMAD);

                    int ILERLEMEYONU = VeriErisim.DegerAl(dr, 11, 0);
                    cg.Add(ILERLEMEYONU);
                    c.Add(cg);
                }

                TNSCollection cic = new TNSCollection();
                string GHKURKOD = VeriErisim.DegerAl(dr, 5, "");
                cic.Add(GHKURKOD);

                double HEDEF_MIKTAR = VeriErisim.DegerAl(dr, 6, 0);
                cic.Add(HEDEF_MIKTAR);

                int SIRANO = VeriErisim.DegerAl(dr, 7, 0);
                cic.Add(SIRANO);

                string GOSTERGEHEDEF_ACIKLAMA = VeriErisim.DegerAl(dr, 8, "");
                cic.Add(GOSTERGEHEDEF_ACIKLAMA);

                double GERCEK_MIKTAR = VeriErisim.DegerAl(dr, 12, 0.0);
                cic.Add(GERCEK_MIKTAR);

                string KAYNAK = VeriErisim.DegerAl(dr, 13, "");
                cic.Add(KAYNAK);

                string SONUC = VeriErisim.DegerAl(dr, 14, "");
                cic.Add(SONUC);

                string SAPMA = VeriErisim.DegerAl(dr, 15, "");
                cic.Add(SAPMA);

                string ONLEM = VeriErisim.DegerAl(dr, 16, "");
                cic.Add(ONLEM);

                cg.Add(cic);
            }

            return o;
        }
    }
}