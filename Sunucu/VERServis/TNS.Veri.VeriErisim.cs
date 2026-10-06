using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace TNS.Veri
{
    public class XMLStringDataReader : IDataReader
    {
        //System.Xml.XmlReader xmlReader;
        //System.IO.StringReader stReader;
        System.Xml.XmlDocument xd;
        System.Xml.XmlNode anaNode;
        System.Xml.XmlNode aktifRec;
        public XMLStringDataReader(string xml)
        {
            xd = new System.Xml.XmlDocument();
            xd.LoadXml("<root>" + xml + "</root>");
            anaNode = xd.FirstChild;
            aktifRec = null;
            //stReader = new System.IO.StringReader(xml);

            //System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            //settings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;

            //xmlReader = System.Xml.XmlReader.Create(stReader, settings);
        }

        public void Close()
        {
        }

        public int Depth
        {
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public bool IsClosed
        {
            get { return false; }
        }

        public bool NextResult()
        {
            return Read();
        }

        public bool Read()
        {
            if (!anaNode.HasChildNodes) return false;
            if (aktifRec == null) aktifRec = anaNode.FirstChild;
            else aktifRec = aktifRec.NextSibling;
            return aktifRec != null;
            //while (true)
            //{
            //    bool oku = xmlReader.Read();
            //    if (oku)
            //    {
            //        if (xmlReader.HasAttributes) return true;
            //    }
            //    else
            //        return false;
            //}
        }

        public int RecordsAffected
        {
            get { return 0; }
        }

        public void Dispose()
        {
            this.Dispose();
        }

        public int FieldCount
        {
            get { return aktifRec.ChildNodes.Count; }
        }

        public bool GetBoolean(int i)
        {
            try
            {
                return Convert.ToBoolean(xmlReader_GetAttribute(i));
            }
            catch { return false; }
        }

        public byte GetByte(int i)
        {
            try
            {
                return Convert.ToByte(xmlReader_GetAttribute(i));
            }
            catch { return 0; }
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public char GetChar(int i)
        {
            try
            {
                return Convert.ToChar(xmlReader_GetAttribute(i));
            }
            catch { return ' '; }
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public IDataReader GetData(int i)
        {
            return null;
        }

        public string GetDataTypeName(int i)
        {
            return "string";
        }

        public DateTime GetDateTime(int i)
        {
            try
            {
                return Convert.ToDateTime(xmlReader_GetAttribute(i));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public decimal GetDecimal(int i)
        {
            try
            {
                return Convert.ToDecimal(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        public double GetDouble(int i)
        {
            try
            {
                return Convert.ToDouble(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        public Type GetFieldType(int i)
        {
            return typeof(string);
        }

        public float GetFloat(int i)
        {
            try
            {
                return (float)Convert.ToDouble(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        public Guid GetGuid(int i)
        {
            return new Guid(xmlReader_GetAttribute(i));
        }

        public short GetInt16(int i)
        {
            try
            {
                return Convert.ToInt16(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        public int GetInt32(int i)
        {
            try
            {
                return Convert.ToInt32(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        public long GetInt64(int i)
        {
            try
            {
                return Convert.ToInt64(NoktadanVirgule(xmlReader_GetAttribute(i)));
            }
            catch { return 0; }
        }

        private string NoktadanVirgule(string p)
        {
            if (p == null) return p;
            return p.Replace('.', ',');
        }

        public string GetName(int i)
        {
            try
            {
                System.Xml.XmlNode x = xmlReader_GetNode(i);
                return x.Name;
            }
            catch { }
            return "";
        }

        public int GetOrdinal(string name)
        {
            for (int i = 0; i < this.FieldCount; i++)
            {
                if (GetName(i) == name) return i;
            }
            return -1;
        }

        public string GetString(int i)
        {
            return xmlReader_GetAttribute(i);
        }

        public object GetValue(int i)
        {
            return xmlReader_GetAttribute(i);
        }

        public int GetValues(object[] values)
        {
            return 0;
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public object this[string name]
        {
            get { return null; }
        }

        public object this[int i]
        {
            get { return xmlReader_GetAttribute(i); }
        }

        private string xmlReader_GetAttribute(int i)
        {
            System.Xml.XmlNode n = xmlReader_GetNode(i);
            if (n == null) return null;
            if (n.Name.StartsWith("XML_"))
            {
                //return "<row xmlns: xsi = \"http://www.w3.org/2001/XMLSchema-instance\" >" + n.InnerXml + "</row>";
                return n.InnerXml;
            }
            return n.InnerText;
        }

        private System.Xml.XmlNode xmlReader_GetNode(int i)
        {
            if (aktifRec == null) return null;
            return aktifRec.ChildNodes[i];
        }
    }

    public class VeriPara
    {
        public int size;
        public string paraName;
        public DbType dbType;
        public Object value;

        public VeriPara(string pParaName, DbType pDbType, int pSize, Object pValue)
        {
            paraName = pParaName;
            size = pSize;
            dbType = pDbType;
            value = pValue;
        }
        public VeriPara(string pParaName, DbType pDbType, Object pValue)
        {
            size = 0;
            paraName = pParaName;
            dbType = pDbType;
            value = pValue;
            if (pDbType == DbType.String || pDbType == DbType.AnsiString)
                if (pValue != null)
                    size = pValue.ToString().Length;
        }
    }

    class VeriErisimOdbc : VeriErisim, IDisposable
    {
        public VeriErisimOdbc(string cnStr)
        {
            cn = new OdbcConnection(cnStr);
            cn.Open();
        }
        private void ParamEkle(OdbcCommand cmd)
        {
            //**Parameter için***************
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++)
                {
                    VeriPara vp = (VeriPara)param[i];
                    OdbcParameter p = new OdbcParameter();

                    p.DbType = vp.dbType;
                    p.Value = vp.value;
                    p.Size = vp.size;
                    p.ParameterName = vp.paraName;
                    cmd.Parameters.Add(p);
                }
            }
            //*****************
        }

        public override IDataReader DataReaderDondur(String sql)
        {
            Logla(sql, 2);
            OdbcCommand cmd;
            IDataReader dr;

            dr = null;
            try
            {
                if (transactionSay > 0)
                    cmd = new OdbcCommand(sql, (OdbcConnection)cn, (OdbcTransaction)tran);
                else
                    cmd = new OdbcCommand(sql, (OdbcConnection)cn);
                this.ParamEkle(cmd);
                cmd.CommandTimeout = this.sureAsimi;
                dr = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                HataEkle(e.Message, sql);
            }
            param.Clear();
            return dr;
        }

        public override int ExecuteNonquery(string sql, int timeOut)
        {
            Logla(sql, 1);
            OdbcCommand cmd;

            int EtkilenenSatir;

            EtkilenenSatir = -1;

            try
            {
                if (transactionSay > 0)
                    cmd = new OdbcCommand(sql, (OdbcConnection)cn, (OdbcTransaction)tran);
                else
                    cmd = new OdbcCommand(sql, (OdbcConnection)cn, (OdbcTransaction)tran);
                this.ParamEkle(cmd);
                cmd.CommandTimeout = timeOut;
                EtkilenenSatir = cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                HataEkle(Ex.Message, sql);
            }
            param.Clear();
            return EtkilenenSatir;
        }
    }

    class VeriErisimSql : VeriErisim, IDisposable
    {
        public VeriErisimSql(string cnStr)
        {
            cn = new SqlConnection(cnStr);
            this.sureAsimi = cn.ConnectionTimeout;
            cn.Open();
        }

        private void ParamEkle(SqlCommand cmd)
        {
            //**Parameter için***************
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++)
                {
                    VeriPara vp = (VeriPara)param[i];
                    SqlParameter p = new SqlParameter();

                    p.DbType = vp.dbType;
                    p.Value = vp.value;
                    p.Size = vp.size;
                    p.ParameterName = vp.paraName;

                    cmd.Parameters.Add(p);
                }
            }
            //*****************
        }

        /// <summary>
        /// Datas the reader dondur.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">VeriPara nesnesi</param>
        /// <returns></returns>
        public override IDataReader DataReaderDondur(String sql)
        {
            Logla(sql, 2);
            SqlCommand cmd;
            IDataReader dr;

            dr = null;
            try
            {
                if (transactionSay > 0)
                    cmd = new SqlCommand(sql, (SqlConnection)cn, (SqlTransaction)tran);
                else
                    cmd = new SqlCommand(sql, (SqlConnection)cn);
                this.ParamEkle(cmd);
                cmd.CommandTimeout = this.sureAsimi;
                dr = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                HataEkle(e.Message, sql);
            }
            param.Clear();
            return dr;
        }

        public override int ExecuteNonquery(string sql, int timeOut)
        {
            Logla(sql, 1);

            SqlCommand cmd;

            int EtkilenenSatir;

            EtkilenenSatir = -1;

            try
            {
                if (transactionSay > 0)
                    cmd = new SqlCommand(sql, (SqlConnection)cn, (SqlTransaction)tran);
                else
                    cmd = new SqlCommand(sql, (SqlConnection)cn);

                this.ParamEkle(cmd);
                cmd.CommandTimeout = timeOut;
                cmd.CommandType = this.komutTuru;

                EtkilenenSatir = cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                HataEkle(Ex.Message, sql);
            }
            param.Clear();
            return EtkilenenSatir;
        }
    }

    class VeriErisimOleDb : VeriErisim, IDisposable
    {
        public VeriErisimOleDb(string cnStr)
        {
            cn = new OleDbConnection(cnStr);
            cn.Open();
        }

        private void ParamEkle(OleDbCommand cmd)
        {
            //**Parameter için***************
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++)
                {
                    VeriPara vp = (VeriPara)param[i];
                    OleDbParameter p = new OleDbParameter();
                    p.DbType = vp.dbType;
                    p.Value = vp.value;
                    p.Size = vp.size;
                    p.ParameterName = vp.paraName;
                    cmd.Parameters.Add(p);
                }
            }
            //*****************
        }
        public override IDataReader DataReaderDondur(String sql)
        {
            Logla(sql, 2);

            OleDbCommand cmd;
            IDataReader dr;

            dr = null;
            try
            {
                if (transactionSay > 0)
                    cmd = new OleDbCommand(sql, (OleDbConnection)cn, (OleDbTransaction)tran);
                else
                    cmd = new OleDbCommand(sql, (OleDbConnection)cn);
                dr = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                HataEkle(e.Message, sql);
            }
            param.Clear();
            return dr;
        }

        public override int ExecuteNonquery(string sql, int timeOut)
        {
            Logla(sql, 1);

            OleDbCommand cmd;
            int EtkilenenSatir;

            EtkilenenSatir = -1;
            //cn=new SqlConnection(cnStr);

            try
            {
                if (transactionSay > 0)
                    cmd = new OleDbCommand(sql, (OleDbConnection)cn, (OleDbTransaction)tran);
                else
                    cmd = new OleDbCommand(sql, (OleDbConnection)cn);
                //**Parameter için***************
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        VeriPara vp = (VeriPara)param[i];
                        SqlParameter p = new SqlParameter();

                        p.DbType = vp.dbType;
                        p.Value = vp.value;
                        p.Size = vp.size;
                        p.ParameterName = vp.paraName;
                        cmd.Parameters.Add(p);
                    }
                }
                //*****************
                cmd.CommandTimeout = timeOut;
                EtkilenenSatir = cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                HataEkle(Ex.Message, sql);
            }
            param.Clear();
            return EtkilenenSatir;
        }
    }

    public class VeriErisim : IDisposable
    {
        protected string cnStr;
        protected System.Data.IDbConnection cn;
        protected System.Data.IDbTransaction tran;
        protected int transactionSay;
        protected int veriErisimSay;
        public string hata;
        public string baglantiAd;
        public string baglantiTip;
        /// <summary>
        /// Komutu yürütmek için beklenecek saniye cinsinden süre.
        /// </summary>
        public int sureAsimi;
        public string yazYol;

        public string logDurum;
        public string logKullaniciBilgi;
        public TNSCollection param;
        protected System.Data.CommandBehavior drCommandBehavior = System.Data.CommandBehavior.Default; //.De .SingleRow
        public CommandType komutTuru = CommandType.Text;

        public VeriErisim()
        {
            tran = null;
            transactionSay = 0;
            veriErisimSay = 0;
            hata = "";
            sureAsimi = 0;
            param = new TNSCollection();
        }

        public void YeniErisimIstegi()
        {
            veriErisimSay++;
        }

        protected void HataEkle(string h, string sql)
        {
            if (h.IndexOf("truncated") > 0)
                h = "Gönderilen data, ayrılan alandan daha büyük olduğundan kayıt işlemi iptal edildi!";

            if (hata == "")
                hata = h;
            else
                hata = hata + "\r\n" + h;
            Logla(sql, 3, h);
        }

        public virtual DataSet DataSetDondur(string sql)
        {
            return null;
        }

        public virtual IDataReader DataReaderDondur(String sql)
        {
            return null;
        }

        public virtual int ExecuteNonquery(string sql, int timeOut)
        {
            return -1;
        }
        public virtual int ExecuteNonquery(string sql, ObjectArray gelenParam)
        {
            param.Clear();
            param.AddRange(gelenParam.objeler);
            return ExecuteNonquery(sql);
        }

        public int ExecuteNonquery(string sql)
        {
            return ExecuteNonquery(sql, this.sureAsimi);
        }

        public void Logla(string sql, int komutTur, string hata = null)
        {
            if (komutTur == 1 && logDurum == "1" || logDurum == "2" || komutTur == 3)
            {
                DateTime dt = DateTime.Now;
                OrtakFonksiyonlar.DosyayaYaz(yazYol + "\\" + DosyaAdBul(sql, komutTur) + "_" + komutTur + "_" + dt.Year.ToString("0000") + "_" + dt.Month.ToString("00") + ".txt", logKullaniciBilgi + "\t" + dt.ToLongDateString() + "\t" + dt.ToLongTimeString() + "\t" + sql + hata, false);
            }
        }

        static string[] komutlar = new string[] { "SELECT ", "INSERT INTO ", "UPDATE ", "DELETE FROM " };
        private string DosyaAdBul(string sql, int komutTur)
        {
            string dosyaAd = "";
            sql = sql.TrimStart();
            int sql_Length = sql.Length;
            for (int i = 0; i < komutlar.Length; i++)
            {
                if (sql.StartsWith(komutlar[i], StringComparison.OrdinalIgnoreCase))
                {
                    int yer = 0;
                    if (i == 0)
                    {
                        yer = sql.IndexOf(" FROM ", 0, sql_Length - 6, StringComparison.CurrentCultureIgnoreCase) + 6;
                    }
                    else yer = komutlar[i].Length;
                    if (yer > 0)
                    {
                        while (true)
                        {
                            if (AtlanacakChar(sql[yer]))
                            {
                                yer++;
                                if (yer >= sql_Length)
                                    break;
                            }
                            else
                                break;
                        }
                        if (yer < sql_Length)
                        {
                            while (true)
                            {
                                dosyaAd += sql[yer];
                                yer++;
                                if (yer >= sql_Length)
                                    break;
                                if (AtlanacakChar(sql[yer]))
                                    break;
                            }
                        }
                    }
                    break;
                }
            }
            if (dosyaAd == "") dosyaAd = "ADYOK";
            return dosyaAd;
        }

        private static bool AtlanacakChar(char p)
        {
            if (p == ' ' || p == '\r' || p == '\n' || p == '\t' || p == '(') return true;
            return false;
        }

        protected void CloseConnection()
        {
            cn.Close();
        }

        public void BeginTransaction()
        {
            if (transactionSay++ == 0)
            {
                tran = cn.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (--transactionSay == 0)
            {
                try
                {
                    tran.Commit();
                }
                catch (Exception) { }
                tran = null;
            }
        }

        public void Rollback()
        {
            try
            {
                if (transactionSay > 0)
                    tran.Rollback();
            }
            catch (Exception) { }
            transactionSay = 0;
            tran = null;
        }

        public void VeriErisimKapat()
        {
            if (veriErisimSay == 0)
            {
                try
                {
                    cn.Close();
                    Logla(2, null);
                }
                catch (Exception ex)
                {
                    Logla(3, ex.Message);
                }
            }
            else
                veriErisimSay--;
        }

        public void VeriErisimKapat(bool sayactan)
        {
            if (!sayactan)
            {
                veriErisimSay = 0;
                try
                {
                    cn.Close();
                    Logla(2, null);
                }
                catch (Exception) { }
            }
            else
                VeriErisimKapat();
        }

        private object TekKolonSQLObject(string sql, int typeNo)
        {
            object donusDeger;
            if (typeNo == 1)
                donusDeger = "";
            else if (typeNo == 2)
                donusDeger = (double)0;
            else if (typeNo == 3)
                donusDeger = new TNSDateTime();
            else if (typeNo == 4)
                donusDeger = (int)0;
            else if (typeNo == 5)
                donusDeger = (decimal)0;
            else
                return null;
            if (hata == "")
            {
                drCommandBehavior = System.Data.CommandBehavior.SingleRow | System.Data.CommandBehavior.SequentialAccess;
                IDataReader r = DataReaderDondur(sql);
                drCommandBehavior = System.Data.CommandBehavior.Default;
                if (hata == "")
                {
                    if (r.Read())
                        if (typeNo == 1)
                            donusDeger = DegerAl(r, 0, "");
                        else if (typeNo == 2)
                            donusDeger = DegerAl(r, 0, (double)0);
                        else if (typeNo == 3)
                            donusDeger = DegerAl(r, 0);
                        else if (typeNo == 4)
                            donusDeger = DegerAl(r, 0, 0);
                        else if (typeNo == 5)
                            donusDeger = DegerAl(r, 0, (decimal)0);
                    r.Close();
                }
            }
            return donusDeger;
        }

        public string TekKolonSQLStr(string sql)
        {
            return (string)TekKolonSQLObject(sql, 1);
        }

        public double TekKolonSQLDbl(string sql)
        {
            return (double)TekKolonSQLObject(sql, 2);
        }

        public TNSDateTime TekKolonSQLDate(string sql)
        {
            return (TNSDateTime)TekKolonSQLObject(sql, 3);
        }

        public int TekKolonSQLInt(string sql)
        {
            return (int)TekKolonSQLObject(sql, 4);
        }

        public decimal TekKolonSQLDecimal(string sql)
        {
            return (decimal)TekKolonSQLObject(sql, 5);
        }

        enum ENUMVTAlanTip { VTDecimal, VTFloat, VTDouble, VTInt64, VTInt32, VTInt16, VTYok }
        private static ENUMVTAlanTip VTAlanTipBul(string alanTip)
        {
            if (alanTip == "money" || alanTip == "DBTYPE_CY" || alanTip == "DECIMAL" || alanTip == "NUMBER")
                return ENUMVTAlanTip.VTDecimal;
            else if (alanTip == "float")
                return ENUMVTAlanTip.VTFloat;
            else if (alanTip == "DOUBLE PRECISION" || alanTip == "DBTYPE_R8")
                return ENUMVTAlanTip.VTDouble;
            else if (alanTip == "bigint")
                return ENUMVTAlanTip.VTInt64;
            else if (alanTip == "int" || alanTip == "DBTYPE_I4")
                return ENUMVTAlanTip.VTInt32;
            else if (alanTip == "DBTYPE_I2" || alanTip == "smallint")
                return ENUMVTAlanTip.VTInt16;
            else
            {
                return ENUMVTAlanTip.VTYok;
                //throw new Exception(alanTip + " alanının tipi bulunamadı!");
                //return ENUMVTAlanTip.VTYok;
            }
        }

        public static double DegerAl(IDataReader r, int kolonNo, double hataDeger)
        {
            if (r.IsDBNull(kolonNo))
                return hataDeger;
            else
            {
                ENUMVTAlanTip alanTip = VTAlanTipBul(r.GetDataTypeName(kolonNo));

                double donusDeger = hataDeger;

                try
                {
                    if (alanTip == ENUMVTAlanTip.VTFloat)
                        donusDeger = r.GetDouble(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTDouble)
                        donusDeger = r.GetDouble(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt64)
                        donusDeger = (double)r.GetInt64(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt32)
                        donusDeger = (double)r.GetInt32(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt16)
                        donusDeger = (double)r.GetInt16(kolonNo);
                    else
                    {
                        donusDeger = (double)GetDecimalOra(r, kolonNo);
                    }
                }
                catch (Exception ex) { throw ex; }

                return donusDeger;
            }
        }

        public static decimal GetDecimalOra(IDataReader r, int kolonNo)
        {
            try
            {
                return r.GetDecimal(kolonNo);
            }
            catch (Exception ex)
            {
                object o = r.GetValue(kolonNo);
                throw new Exception(o.GetType() + ex.Message);
            }
        }

        public static decimal DegerAl(IDataReader r, int kolonNo, decimal hataDeger)
        {
            if (r.IsDBNull(kolonNo))
                return hataDeger;
            else
            {
                ENUMVTAlanTip alanTip = VTAlanTipBul(r.GetDataTypeName(kolonNo));

                decimal donusDeger = hataDeger;

                try
                {
                    if (alanTip == ENUMVTAlanTip.VTFloat)
                        donusDeger = (decimal)r.GetFloat(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt32)
                        donusDeger = (decimal)r.GetInt32(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt16)
                        donusDeger = (decimal)r.GetInt16(kolonNo);
                    else
                    {
                        donusDeger = GetDecimalOra(r, kolonNo);
                    }
                }
                catch (Exception ex) { throw ex; }

                return donusDeger;
            }
        }

        public static int DegerAl(IDataReader r, int kolonNo, int hataDeger)
        {
            if (r.IsDBNull(kolonNo))
                return hataDeger;
            else
            {
                ENUMVTAlanTip alanTip = VTAlanTipBul(r.GetDataTypeName(kolonNo));
                int donusDeger = hataDeger;
                try
                {
                    if (alanTip == ENUMVTAlanTip.VTDecimal)
                        donusDeger = (int)GetDecimalOra(r, kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTFloat)
                        donusDeger = (int)(r.GetFloat(kolonNo));
                    else if (alanTip == ENUMVTAlanTip.VTDouble)
                        donusDeger = (int)r.GetDouble(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt64)
                        donusDeger = (int)r.GetInt64(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt32)
                        donusDeger = r.GetInt32(kolonNo);
                    else if (alanTip == ENUMVTAlanTip.VTInt16)
                        donusDeger = r.GetInt16(kolonNo);
                    else
                        donusDeger = (int)r.GetInt32(kolonNo);
                }
                catch { }

                return donusDeger;
            }
        }

        public static string DegerAl(IDataReader r, int kolonNo, string hataDeger)
        {
            try
            {
                if (r.IsDBNull(kolonNo))
                    return hataDeger;
                else
                {
                    string conData = System.Configuration.ConfigurationManager.AppSettings.Get("ConnectionString.Data") + "";

                    string tut = r.GetString(kolonNo);
                    if (string.IsNullOrEmpty(tut))
                        return tut;
                    if (tut.Equals(" ") && conData.Equals("ORACLE"))
                        tut = string.Empty;
                    return tut;
                }
            }
            catch { }
            return hataDeger;
        }

        public static TNSDateTime DegerAl(IDataReader r, int kolonNo)
        {
            try
            {
                string alanTip = r.GetDataTypeName(kolonNo);

                if (!r.IsDBNull(kolonNo))
                    return new TNSDateTime(r.GetDateTime(kolonNo));
            }
            catch { }
            return new TNSDateTime();
        }

        [Obsolete]
        public static VeriErisim VeriErisimBul(VeriErisim veriErisim, string p_baglantiAd)
        {
            return VeriErisimBul(veriErisim, p_baglantiAd, null);
        }

        public static VeriErisim VeriErisimBul(VeriErisim veriErisim, string p_baglantiAd, string kullaniciBilgi)
        {
            string cnStr;
            if (veriErisim != null)
            {
                if (veriErisim.baglantiAd == p_baglantiAd)
                {
                    veriErisim.YeniErisimIstegi();
                    return veriErisim;
                }
            }

            cnStr = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd);

            if (string.IsNullOrEmpty(cnStr))
            {
                p_baglantiAd = "ConnectionString";
                cnStr = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd) + "";
            }

            string p_log = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + ".Log") + "";

            //Connection string crypto ise
            //************************************
            string p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiri.Sifrele") + "";
            if (p_sifrele == "")
                p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + ".Sifre") + "";

            if (OrtakFonksiyonlar.ConvertToInt(p_sifrele, 0) > 0 && !string.IsNullOrEmpty(cnStr))
            {
                if (p_sifrele == "2")
                    cnStr = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(cnStr);
                else
                    cnStr = OrtakFonksiyonlar.SifreCoz(cnStr);
            }
            //************************************

            VeriErisim donusDeger = null;

            string p_tip = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + ".Data") + "";
            if (string.IsNullOrEmpty(p_tip))
                p_tip = "SQLCLIENT";

            p_tip = p_tip.ToUpper();
            try
            {
                if (p_tip.Equals("OLEDB"))
                {
                    donusDeger = new VeriErisimOleDb(cnStr);
                }
                else if (p_tip.Equals("ORACLE"))
                {
                    throw new NotSupportedException("Oracle desteği .NET 8 sürümünden çıkarıldı (ConnectionString.Data = ORACLE).");
                }
                else if (p_tip.Equals("ODBC"))
                {
                    donusDeger = new VeriErisimOdbc(cnStr);
                }
                else //SQLCLIENT
                {
                    donusDeger = new VeriErisimSql(cnStr);
                }
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataDosyaYaz("VeriErisim.txt", $"{p_baglantiAd}: {ex.Message}");
                donusDeger = null;
            }
            if (donusDeger != null)
            {
                donusDeger.baglantiAd = p_baglantiAd;
                donusDeger.baglantiTip = p_tip;
                donusDeger.logDurum = p_log;
                donusDeger.yazYol = YolOl(p_baglantiAd);
                donusDeger.logKullaniciBilgi = kullaniciBilgi;
                if (!string.IsNullOrEmpty(donusDeger.logDurum))
                {
                    try
                    {
                        var m = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod();
                        donusDeger.modulAd = m.ReflectedType.FullName + "." + m.Name;
                    }
                    catch
                    {
                        donusDeger.modulAd = "bulunamadı!";
                    }
                }
                else
                    donusDeger.modulAd = "";
                donusDeger.Logla(1, null);
            }
            return donusDeger;
        }

        public void Logla(int acilis1Kapanis2Hata3, string hata)
        {
            if (!string.IsNullOrEmpty(modulAd))
            {
                DateTime dt = DateTime.Now;
                string ekle;
                if (acilis1Kapanis2Hata3 == 1)
                    ekle = "açıldı";
                else if (acilis1Kapanis2Hata3 == 2)
                    ekle = "kapandı";
                else
                    ekle = "hata:" + hata;
                OrtakFonksiyonlar.HataDosyaYaz(yazYol + "\\" + this.baglantiAd + "_" + dt.Year.ToString("0000") + "_" + dt.Month.ToString("00") + "_" + dt.Day.ToString("00") + ".txt", modulAd + "\t" + ekle + "\t" + dt.ToLongTimeString() + "." + dt.Millisecond.ToString("000"));
            }
        }

        static System.Collections.Hashtable yollar = new System.Collections.Hashtable();
        private static string YolOl(string p_baglantiAd)
        {
            object yolAd = yollar[p_baglantiAd];
            if (yolAd == null)
            {
                string yolIcAd = System.Configuration.ConfigurationManager.AppSettings.Get("VTLogYol");

                if (!yolIcAd.EndsWith("\\"))
                    yolIcAd += "\\";
                yolIcAd += p_baglantiAd;
                yollar[p_baglantiAd] = yolIcAd;
                OrtakFonksiyonlar.YolOlustur(yolIcAd);
                return yolIcAd;
            }
            return (string)yolAd;
        }

        public static string VeriErisimDegerleri(string p_baglantiAd)
        {
            if (p_baglantiAd == "")
                return "";

            string cnStr = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd);

            if (string.IsNullOrEmpty(cnStr))
            {
                p_baglantiAd = "ConnectionString";
                cnStr = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd) + "";
            }

            //Connection string crypto ise
            //************************************
            string p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiri.Sifrele") + "";
            if (p_sifrele == "")
                p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + ".Sifre") + "";

            if (OrtakFonksiyonlar.ConvertToInt(p_sifrele, 0) > 0 && !string.IsNullOrEmpty(cnStr))
            {
                if (p_sifrele == "2")
                    cnStr = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(cnStr);
                else
                    cnStr = OrtakFonksiyonlar.SifreCoz(cnStr);
            }
            //************************************

            return cnStr;
        }

        public static VeriErisim VeriErisimBulBaglantiYoluIle(VeriErisim veriErisim, string p_baglantiYolu, string p_tip)
        {
            string cnStr;
            if (veriErisim != null)
            {
                if (veriErisim.cnStr == p_baglantiYolu)
                {
                    veriErisim.YeniErisimIstegi();
                    return veriErisim;
                }
            }

            cnStr = p_baglantiYolu;
            VeriErisim donusDeger = null;

            if (p_tip == null || p_tip == "") p_tip = "SQLCLIENT";
            p_tip = p_tip.ToUpper();

            if (p_tip.Equals("OLEDB"))
            {
                donusDeger = new VeriErisimOleDb(cnStr);
            }
            else if (p_tip.Equals("ORACLE"))
            {
                throw new NotSupportedException("Oracle desteği .NET 8 sürümünden çıkarıldı (ConnectionString.Data = ORACLE).");
            }
            else if (p_tip.Equals("ODBC"))
            {
                donusDeger = new VeriErisimOdbc(cnStr);
            }
            else //SQLCLIENT
            {
                donusDeger = new VeriErisimSql(cnStr);
            }
            if (donusDeger != null)
            {
                donusDeger.baglantiAd = "BAGLANTIADYOK";
                donusDeger.baglantiTip = p_tip;
            }
            return donusDeger;
        }

        public static string IslecBul(string islec, string p_baglantiAd)
        {
            string islec_ad = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + "." + islec);
            if (string.IsNullOrEmpty(islec_ad))
                return islec;
            else
                return islec_ad;
        }

        public static string IslecBul(string islec)
        {
            return IslecBul(islec, "ConnectionString");
        }

        public static string unicode = null;
        public static string conData = null;
        private string modulAd;

        public static string DegerYap(string pAlanDeger)
        {
            if (pAlanDeger == null)
                return "NULL";
            else
            {
                if (conData == null)
                    conData = System.Configuration.ConfigurationManager.AppSettings.Get("ConnectionString.Data") + "";

                if (unicode == null)
                    unicode = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiri.Unicode") + "";

                if (string.IsNullOrEmpty(pAlanDeger) && conData.Equals("ORACLE"))
                    pAlanDeger = " ";

                if (string.IsNullOrEmpty(unicode))
                    return "'" + pAlanDeger.Replace("'", "''") + "'";
                else
                    return "N'" + pAlanDeger.Replace("'", "''") + "'";

                //#if(UNICODE)
                //                return "N'" + pAlanDeger.Replace("'", "''") + "'";
                //#else
                //                return "'" + pAlanDeger.Replace("'", "''") + "'";
                //#endif
            }
        }

        public static string DegerYap(int pAlanDeger)
        {
            return Convert.ToString(pAlanDeger);
        }

        public static string DegerYap(long pAlanDeger)
        {
            return Convert.ToString(pAlanDeger);
        }

        public static string DegerYap(double pAlanDeger)
        {
            string tut = Convert.ToString(pAlanDeger);
            tut = tut.Replace(",", ".");
            return tut;
        }

        public static string DegerYap(decimal pAlanDeger)
        {
            string tut = Convert.ToString(pAlanDeger);
            tut = tut.Replace(",", ".");
            return tut;
        }

        public static string DegerYap(TNSDateTime pAlanDeger, string p_baglantiAd)
        {
            string tarih_tip = System.Configuration.ConfigurationManager.AppSettings.Get(p_baglantiAd + ".TarihFormat") + "";
            if (pAlanDeger == null || pAlanDeger.isNull)
                return "null";
            else if (string.IsNullOrEmpty(tarih_tip) || tarih_tip.ToUpper() == "TS")
                return pAlanDeger.ToTimeStamp();
            else if (tarih_tip.ToUpper() == "TO")
            {
                string t = pAlanDeger.ToTimeStamp();
                t = t.Replace("{ts", "");
                t = t.Replace("}", "");
                return "timestamp" + t + "";
            }
            else
                return "'" + pAlanDeger.ToLongString() + "'";
        }

        public static string DegerYap(TNSDateTime pAlanDeger)
        {
            return DegerYap(pAlanDeger, "ConnectionString");
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, int pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + pKosulIslec + DegerYap(pAlanDeger);
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, double pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + pKosulIslec + DegerYap(pAlanDeger);
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, decimal pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + pKosulIslec + DegerYap(pAlanDeger);
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, long pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + pKosulIslec + DegerYap(pAlanDeger);
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, string pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + " " + pKosulIslec + " " + DegerYap(pAlanDeger);
        }

        public static string KosulOl(string pTabloAd, string pAlanAd, string pKosulIslec, TNSDateTime pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            return donenDeger + pAlanAd + " " + pKosulIslec + " " + DegerYap(pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, string pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, int pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, double pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, decimal pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, long pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public static string AtamaOl(string pTabloAd, string pAlanAd, TNSDateTime pAlanDeger)
        {
            return KosulOl(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        private static string AdBul(TNSCollection param)
        {
            return "@p" + param.Count;
        }

        public string DegerYapPRM(string pAlanDeger)
        {
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger != null) o = pAlanDeger;
            param.Add(new VeriPara(adOl, DbType.String, o));
            return adOl;
        }

        public string DegerYapPRM(int? pAlanDeger)
        {
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Int32, o));
            return adOl;
        }

        public string DegerYapPRM(long? pAlanDeger)
        {
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Int64, o));
            return adOl;
        }

        public string DegerYapPRM(double? pAlanDeger)
        {
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Double, o));
            return adOl;
        }

        public string DegerYapPRM(decimal? pAlanDeger)
        {
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Decimal, o));
            return adOl;
        }

        public string DegerYapPRM(TNSDateTime pAlanDeger, string p_baglantiAd)
        {
            string adOl = AdBul(param);
            object o = null;
            if (!TNSDateTime.IsNull(pAlanDeger)) o = pAlanDeger.Oku();
            param.Add(new VeriPara(adOl, DbType.DateTime, o));
            return adOl;
        }

        public string DegerYapPRM(TNSDateTime pAlanDeger)
        {
            return DegerYapPRM(pAlanDeger, "ConnectionString");
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, int? pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";

            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Int32, o));
            return donenDeger + pAlanAd + pKosulIslec + adOl;
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, double? pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Double, o));
            return donenDeger + pAlanAd + pKosulIslec + adOl;
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, decimal? pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Decimal, o));
            return donenDeger + pAlanAd + pKosulIslec + adOl;
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, long? pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger.HasValue) o = pAlanDeger.Value;
            param.Add(new VeriPara(adOl, DbType.Int64, o));
            return donenDeger + pAlanAd + pKosulIslec + adOl;
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, string pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            string adOl = AdBul(param);
            object o = null;
            if (pAlanDeger != null) o = pAlanDeger;
            param.Add(new VeriPara(adOl, DbType.String, o));
            return donenDeger + pAlanAd + " " + pKosulIslec + " " + adOl;
        }

        public string KosulOlPRM(string pTabloAd, string pAlanAd, string pKosulIslec, TNSDateTime pAlanDeger)
        {
            string donenDeger;
            if (pTabloAd != "")
                donenDeger = pTabloAd + ".";
            else
                donenDeger = "";
            string adOl = AdBul(param);
            object o = null;
            if (!TNSDateTime.IsNull(pAlanDeger)) o = pAlanDeger.Oku();
            param.Add(new VeriPara(adOl, DbType.DateTime, o));
            return donenDeger + pAlanAd + pKosulIslec + adOl;
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, string pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, int? pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, double? pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, decimal? pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, long? pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        public string AtamaOlPRM(string pTabloAd, string pAlanAd, TNSDateTime pAlanDeger)
        {
            return KosulOlPRM(pTabloAd, pAlanAd, "=", pAlanDeger);
        }

        /// <summary>
        /// Kırılımlı bir alan için, sql cümleciği where kısmına eklenen koşulu hazırlayıp döndüren yordam.
        /// Verilen değer içinde virgül veya noktalı virgül varsa, değer parçalanır ve her bir parça OR anahtar kelimesi ile koşula eklenir.
        /// Eğer koşul oluşturulmuşsa başına her zaman AND anahtar kelimesi eklenir.
        /// </summary>
        /// <param name="tabloAd">Koşulda alan adının başına eklenilmesi isteniyorsa verilen tablo adı</param>
        /// <param name="alanAd">Koşulun hazırlnacağı alan ad</param>
        /// <param name="alanDeger">Koşulun kıstas parametresi. Virgül veya noktalı virgül ile ayrılmışsa her bir değer OR anahtar kelimesi ile koşula eklenir.</param>
        /// <returns></returns>
        public static string AlanKosulOl(string tabloAd, string alanAd, string alanDeger, string onEk)
        {
            if (string.IsNullOrEmpty(alanDeger))
                return "";

            int pos = alanDeger.IndexOf(',');
            char ayrac;
            if (pos > 0)
                ayrac = ',';
            else
                ayrac = ';';

            string[] kistasStrler = alanDeger.Split(ayrac);
            string icKosul = "";
            for (int i = 0; i < kistasStrler.Length; i++)
            {
                if (!string.IsNullOrEmpty(kistasStrler[i]))
                {
                    if (!string.IsNullOrEmpty(icKosul)) icKosul += " OR ";
                    icKosul += VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kistasStrler[i].Replace(".", "") + "%");
                }
            }
            if (!string.IsNullOrEmpty(icKosul))
                return onEk + "(" + icKosul + ") ";
            else
                return "";
        }

        public static string TumAlanlar(string pTabloAd, string[] pAlanlar)
        {
            return TumAlanlarEksik(pTabloAd, pAlanlar, null);
        }

        public static string TumAlanlarEksik(string pTabloAd, string[] pAlanlar, int[] eksikAlannolar)
        {
            string donenDeger = "";
            string tabloEk = "";
            if (pTabloAd != "") tabloEk = pTabloAd + ".";
            for (int i = 0; i < pAlanlar.Length; i++)
            {
                if (!EksinAlanMi(i, eksikAlannolar))
                {
                    string a = pAlanlar[i];
                    if (donenDeger != "") donenDeger = donenDeger + ", ";
                    donenDeger = donenDeger + tabloEk + a;
                }
            }
            return donenDeger;
        }

        private static bool EksinAlanMi(int i, int[] eksikAlannolar)
        {
            if (eksikAlannolar == null || eksikAlannolar.Length == 0) return false;
            foreach (int item in eksikAlannolar)
            {
                if (item == i) return true;
            }
            return false;
        }

        public static string KosulEkle(string kosul, string baglac, string ekKosul)
        {
            if (string.IsNullOrEmpty(ekKosul)) return kosul;
            if (kosul != "") return kosul + " " + baglac + " " + ekKosul;
            else return ekKosul;
        }

        private static string JoinTek(string pTabloAd1, string pAlanAd1, string pTabloAd2, string pAlanAd2)
        {
            if (!string.IsNullOrWhiteSpace(pTabloAd1)) pTabloAd1 = pTabloAd1 + ".";
            if (!string.IsNullOrWhiteSpace(pTabloAd2)) pTabloAd2 = pTabloAd2 + ".";

            return pTabloAd1 + pAlanAd1 + " = " + pTabloAd2 + pAlanAd2;
        }

        public static string Join(string pTabloAd1, string pTabloAd2, string[] pAlanAdlar1, string[] pAlanAdlar2)
        {
            string joinStr = "";
            for (int i = 0; i < Math.Min(pAlanAdlar1.Length, pAlanAdlar2.Length); i++)
            {
                if (i > 0) joinStr += " AND ";
                joinStr += JoinTek(pTabloAd1, pAlanAdlar1[i], pTabloAd2, pAlanAdlar2[i]);
            }
            return joinStr;
        }

        public static string Join(string pTabloAd1, string pTabloAd2, string[] pAlanAdlar)
        {
            return Join(pTabloAd1, pTabloAd2, pAlanAdlar, pAlanAdlar);
        }

        public static string IN_KosulOl(string baglac, string alan, string parametreler, bool tirnakla, bool noktaAt)
        {
            if (!string.IsNullOrWhiteSpace(parametreler))
            {
                return IN_KosulOl(baglac, alan, parametreler.Split(',', ';'), tirnakla, noktaAt);
            }
            return "";
        }

        public static string IN_KosulOl(string baglac, string alan, string[] parametreler, bool tirnakla, bool noktaAt)
        {
            string donusDeger = "";
            if (parametreler != null && parametreler.Length > 0)
            {
                foreach (string item in parametreler)
                {
                    if (!tirnakla && (OrtakFonksiyonlar.ConvertToInt(item, 0) == 0)) continue;

                    if (donusDeger == "")
                        donusDeger = baglac + " " + alan + " IN (";
                    else
                        donusDeger += ", ";

                    donusDeger += Tirnakla(item, tirnakla, noktaAt);
                }
                if (donusDeger != "")
                {
                    donusDeger += ")";
                }
            }
            return donusDeger;
        }

        public static string OR_KosulOl(string alan, string parametreler, bool tirnakla, bool noktaAt, int strEsitOnLikeArkaLikeOnArkaLiske)
        {
            if (!string.IsNullOrWhiteSpace(parametreler))
                return OR_KosulOl(alan, parametreler.Split(',', ';'), tirnakla, noktaAt, strEsitOnLikeArkaLikeOnArkaLiske);
            else
                return "";
        }

        public static string OR_KosulOl(string alan, string[] parametreler, bool tirnakla, bool noktaAt, int strEsitOnLikeArkaLikeOnArkaLiske)
        {
            string donusDeger = "";
            if (parametreler != null && parametreler.Length > 0)
            {
                foreach (string item in parametreler)
                {
                    if (!tirnakla && (OrtakFonksiyonlar.ConvertToInt(item, 0) == 0)) continue;
                    if (tirnakla && string.IsNullOrWhiteSpace(item)) continue;
                    string on = "";
                    string son = "";
                    string islec = "=";
                    if (tirnakla && strEsitOnLikeArkaLikeOnArkaLiske > 0)
                    {
                        islec = "LIKE";
                        if (strEsitOnLikeArkaLikeOnArkaLiske == 1 || strEsitOnLikeArkaLikeOnArkaLiske == 3) on = "%";
                        if (strEsitOnLikeArkaLikeOnArkaLiske == 2 || strEsitOnLikeArkaLikeOnArkaLiske == 3) son = "%";
                    }
                    if (donusDeger != "") donusDeger += " OR ";
                    donusDeger += alan + " " + islec + " " + Tirnakla(item, tirnakla, noktaAt, on, son);
                }
                if (donusDeger != "")
                {
                    donusDeger = "(" + donusDeger + ")";
                }
            }
            return donusDeger;
        }

        private static string Tirnakla(string deger, bool tirnakla, bool noktaAt, string on = "", string son = "")
        {
            if (tirnakla)
            {
                if (noktaAt) deger = deger.Replace(".", "");
                return VeriErisim.DegerYap(on + deger + son);
            }
            else
                return deger;
        }

        public void Dispose()
        {
            if (this.veriErisimSay > 0)
            {
                OrtakFonksiyonlar.HataDosyaYaz("VEDispose.txt", "Veri Erişim Dispose:" + this.baglantiAd + "::" + this.modulAd + " (" + this.hata + ")", false);
                VeriErisimKapat(false);
            }
        }

        ~VeriErisim()
        {
            Dispose();
        }
    }
}
