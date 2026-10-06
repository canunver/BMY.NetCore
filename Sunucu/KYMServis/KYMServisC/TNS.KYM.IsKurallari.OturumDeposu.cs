using System;
using System.Data;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Web uygulamasının oturum bilgilerini KYMOTURUM tablosunda saklar.
    /// Eski sistemdeki ASP.NET State Server'ın yerini alır: oturum uygulamanın belleğinde değil veri tabanında
    /// durduğu için uygulama yeniden başlasa da (ör. yazılım güncellemesi) kullanıcılar oturumda kalır.
    /// </summary>
    public class OturumDeposu
    {
        private const string BAGLANTI = "BaglantiSatiriKYM";

        /// <summary>Oturumu yazar (varsa günceller) ve süresi dolmuş oturumları temizler.</summary>
        public static Sonuc Yaz(string oturumNo, string kullaniciKodu, string veri, int sureDakika)
        {
            if (string.IsNullOrEmpty(oturumNo)) return new Sonuc("Oturum numarası boş olamaz.");

            DateTime simdi = DateTime.Now;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BAGLANTI, null);
            try
            {
                ve.hata = "";
                ve.param.Add(new VeriPara("@SIMDI", DbType.DateTime, simdi));
                ve.ExecuteNonquery("DELETE FROM KYMOTURUM WHERE BITIS < @SIMDI");

                ve.hata = "";
                ve.param.Add(new VeriPara("@OTURUMNO", DbType.AnsiString, oturumNo));
                ve.param.Add(new VeriPara("@KULLANICIKODU", DbType.AnsiString, kullaniciKodu ?? ""));
                ve.param.Add(new VeriPara("@VERI", DbType.String, veri ?? ""));
                ve.param.Add(new VeriPara("@SIMDI", DbType.DateTime, simdi));
                ve.param.Add(new VeriPara("@BITIS", DbType.DateTime, simdi.AddMinutes(sureDakika)));
                ve.ExecuteNonquery(@"UPDATE KYMOTURUM SET KULLANICIKODU = @KULLANICIKODU, VERI = @VERI, SONERISIM = @SIMDI, BITIS = @BITIS WHERE OTURUMNO = @OTURUMNO;
                    IF @@ROWCOUNT = 0
                        INSERT INTO KYMOTURUM (OTURUMNO, KULLANICIKODU, VERI, OLUSTURMA, SONERISIM, BITIS)
                        VALUES (@OTURUMNO, @KULLANICIKODU, @VERI, @SIMDI, @SIMDI, @BITIS)");

                return new Sonuc(ve.hata ?? "");
            }
            finally
            {
                ve.VeriErisimKapat();
            }
        }

        /// <summary>
        /// Oturum verisini okur ve bitiş zamanını şimdiden itibaren sureDakika kadar ileri alır.
        /// Oturum yoksa ya da süresi dolmuşsa null döner.
        /// </summary>
        public static string Oku(string oturumNo, int sureDakika)
        {
            if (string.IsNullOrEmpty(oturumNo)) return null;

            DateTime simdi = DateTime.Now;
            string veri = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BAGLANTI, null);
            try
            {
                ve.hata = "";
                ve.param.Add(new VeriPara("@OTURUMNO", DbType.AnsiString, oturumNo));
                ve.param.Add(new VeriPara("@SIMDI", DbType.DateTime, simdi));
                IDataReader r = ve.DataReaderDondur("SELECT VERI FROM KYMOTURUM WHERE OTURUMNO = @OTURUMNO AND BITIS >= @SIMDI");
                if (r != null)
                {
                    if (r.Read()) veri = VeriErisim.DegerAl(r, 0, "");
                    r.Close();
                }

                if (veri == null) return null;

                ve.hata = "";
                ve.param.Add(new VeriPara("@OTURUMNO", DbType.AnsiString, oturumNo));
                ve.param.Add(new VeriPara("@SIMDI", DbType.DateTime, simdi));
                ve.param.Add(new VeriPara("@BITIS", DbType.DateTime, simdi.AddMinutes(sureDakika)));
                ve.ExecuteNonquery("UPDATE KYMOTURUM SET SONERISIM = @SIMDI, BITIS = @BITIS WHERE OTURUMNO = @OTURUMNO");

                return veri;
            }
            finally
            {
                ve.VeriErisimKapat();
            }
        }

        public static Sonuc Sil(string oturumNo)
        {
            if (string.IsNullOrEmpty(oturumNo)) return new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BAGLANTI, null);
            try
            {
                ve.hata = "";
                ve.param.Add(new VeriPara("@OTURUMNO", DbType.AnsiString, oturumNo));
                ve.ExecuteNonquery("DELETE FROM KYMOTURUM WHERE OTURUMNO = @OTURUMNO");
                return new Sonuc(ve.hata ?? "");
            }
            finally
            {
                ve.VeriErisimKapat();
            }
        }
    }
}
