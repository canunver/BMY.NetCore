using System;
using System.Text;

namespace TNS
{
    public enum VariableType : int
    {
        Money = 1,
        Integer = 2,
        String = 3,
        Date = 4,
        Bilinmiyor = 0
    }

    public static class VariableFunctions
    {
        public static string GetTypeName(VariableType vt)
        {
            if (vt == VariableType.Integer) return "int";
            if (vt == VariableType.Money) return "double";
            if (vt == VariableType.String) return "string";
            if (vt == VariableType.Date) return "DateTime";
            return "";
        }

        public static string GetSQLTypeName(VariableType vt)
        {
            return GetSQLTypeName(vt, 0);
        }

        public static string GetSQLTypeName(VariableType vt, int uzunluk)
        {
            if (uzunluk == 0)
            {
                if (vt == VariableType.String) uzunluk = 200;
                else uzunluk = 6;
            }

            if (vt == VariableType.Integer) return "INT";
            if (vt == VariableType.Money) return "DECIMAL(21, " + uzunluk + ")";
            if (vt == VariableType.String) return "VARCHAR(" + uzunluk + ")";
            if (vt == VariableType.Date) return "DATE";
            return "";
        }

        public static string GetTypeUserName(VariableType vt)
        {
            if (vt == VariableType.Integer) return "Tamsayı";
            if (vt == VariableType.Money) return "Para";
            if (vt == VariableType.String) return "Yazı";
            if (vt == VariableType.Date) return "Tarih";
            return "";
        }

        public static VariableType GetType(string value)
        {
            if (GetTypeUserName(VariableType.Integer) == value) return VariableType.Integer;
            else if (GetTypeUserName(VariableType.Money) == value) return VariableType.Money;
            else if (GetTypeUserName(VariableType.String) == value) return VariableType.String;
            else if (GetTypeUserName(VariableType.Date) == value) return VariableType.Date;
            else return VariableType.Bilinmiyor;
        }
    }

    public class Variable
    {
        public string name { get; set; }
        public VariableType type { get; set; }

        public Variable(string name, VariableType type)
        {
            this.name = name;
            this.type = type;
        }

        internal string GetTypeName()
        {
            return VariableFunctions.GetTypeName(this.type);
        }
    }

    public class Formula : Variable
    {
        public string formula { get; set; }

        public Formula(string name, VariableType type, string formula)
            : base(name, type)
        {
            this.formula = formula;
        }
    }

    public class Calculator
    {
        Variable[] variables;
        Formula[] formulas;
        object obj;
        string nameSpace;
        string className;
        string anaClassName;
        string functions = null;

        public string PrepareCalculator(string nameSpace, string className, string functions, Variable[] variables, Formula[] formulas)
        {
            return PrepareCalculator(nameSpace, className, "", functions, variables, formulas, null);
        }

        public string GetSource()
        {
            return GenerateClass();
        }

        public string PrepareCalculator(string nameSpace, string className, string anaClassName, string functions, Variable[] variables, Formula[] formulas, string[] referencedAssemblies)
        {
            this.variables = variables;
            this.formulas = formulas;
            this.nameSpace = nameSpace;
            this.className = className;
            this.anaClassName = anaClassName;
            this.functions = functions;

            // .NET 8: CSharpCodeProvider (CodeDom) derleyemez; kod Roslyn ile bellekte derlenip ayrı bir yükleme bağlamına
            // yüklenir (TNS.KodDerleyici). Çalışan uygulamanın kitaplıkları (GNLServis, HRCServisI ...) kendiliğinden görülür;
            // referencedAssemblies'teki eski "bin\X.dll" yolları uygulama klasöründe aranır.
            string hata;
            byte[] ikili = KodDerleyici.Derle(GenerateClass(), nameSpace + "_" + className + "_" + Guid.NewGuid().ToString("N"), referencedAssemblies, out hata);
            if (ikili == null) return hata;

            obj = KodDerleyici.Yukle(ikili, nameSpace + "." + className).CreateInstance(nameSpace + "." + className);
            if (obj == null) throw new Exception("Verilen kod derlenemedi!");
            return "";
        }

        public void ExecuteCalc(System.Collections.Hashtable htIn, System.Collections.Hashtable htOut)
        {
            Type t = obj.GetType();
            foreach (System.Collections.DictionaryEntry item in htIn)
            {
                System.Reflection.PropertyInfo pi = t.GetProperty((string)item.Key);
                pi.SetValue(obj, item.Value, null);
            }

            System.Collections.Generic.List<string> l = new System.Collections.Generic.List<string>();

            foreach (string ad in htOut.Keys)
            {
                l.Add(ad);
            }

            foreach (string ad in l)
            {
                System.Reflection.PropertyInfo pi = t.GetProperty((string)ad);
                htOut[ad] = pi.GetValue(obj, null);
            }
        }

        public string GenerateClass()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;"); //Namespace aç
            sb.AppendLine("namespace " + this.nameSpace + "{"); //Namespace aç
            sb.AppendLine("public class " + this.className + anaClassName + "{"); //Class aç
            sb.AppendLine(functions);
            sb.AppendLine();
            if (variables != null && variables.Length > 0)
            {
                for (int i = 0; i < variables.Length; i++)
                {
                    if (i == 0)
                    {
                        sb.AppendLine("private " + variables[i].GetTypeName() + " _" + variables[i].name + ";");
                        sb.AppendLine(string.Format("public {0} {1} {{ get {{ return _{1}; }} set {{ _{1} = value; InitOutputs(); }} }}",
                            variables[i].GetTypeName(), variables[i].name));
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine("public " + variables[i].GetTypeName() + " " + variables[i].name + "{ get; set;}");
                    }
                }
            }
            sb.AppendLine();
            string initFunc = "private void InitOutputs() {\r\n";
            if (formulas != null && formulas.Length > 0)
            {
                for (int i = 0; i < formulas.Length; i++)
                {
                    initFunc += "_" + formulas[i].name + " = null;\r\n";
                    bool nullable = false;
                    if (formulas[i].type == VariableType.String)
                        nullable = true;

                    sb.AppendLine("private " + formulas[i].GetTypeName() + (nullable ? "" : "?") + " _" + formulas[i].name + "=null;");
                    sb.AppendLine("private " + formulas[i].GetTypeName() + " __" + formulas[i].name + " { get {" + formulas[i].formula + " } }");
                    sb.AppendLine("public " + formulas[i].GetTypeName() + " " + formulas[i].name + " { get { " + IcFonksiyon(nullable, formulas[i].name) + " } }");
                    sb.AppendLine();
                }
            }
            initFunc += "}";
            sb.AppendLine(initFunc);
            sb.AppendLine("}"); //Class kapat
            sb.AppendLine("}"); //Namespace kapat
            return sb.ToString();
        }

        private string IcFonksiyon(bool nullable, string degiskenAd)
        {
            string tut = string.Format("if (_{0} == null) _{0} = __{0};", degiskenAd);
            tut += "return _" + degiskenAd + (nullable ? "" : ".Value") + ";";
            return tut;
        }
    }
}
