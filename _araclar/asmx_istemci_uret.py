# -*- coding: utf-8 -*-
"""Visual Studio'nun urettigi eski ASMX istemcisini (Web References/.../Reference.cs; SoapHttpClientProtocol veya
WSE WebServicesClientProtocol tabanli) .NET 8'de derlenen, TNS.Soap.SoapIstemci tabanli surume cevirir.
Veri siniflari aynen korunur; proxy sinifi yeniden yazilir; Async metotlar ve olay siniflari atilir.
Kullanim: python3 asmx_istemci_uret.py <Reference.cs>   (dosya yerinde degistirilir, eskisi Reference.cs.eski olarak kalir)"""
import re,sys,os,shutil
p=sys.argv[1]
raw=open(p,'rb').read().decode('utf-8-sig'); s=raw.replace('\r\n','\n')
ns=re.search(r'^namespace ([\w.]+) \{',s,re.M).group(1)
m=re.search(r'((?:    \[[^\n]*\]\n)*)    public partial class (\w+) : (?:System\.Web\.Services\.Protocols\.SoapHttpClientProtocol|Microsoft\.Web\.Services\d\.WebServicesClientProtocol)\s*\{',s)
sinif=m.group(2); ekler=re.findall(r'XmlIncludeAttribute\(typeof\((\w+)\)\)',m.group(1))
# proxy sinifinin sonu
d=0;i=m.end()-1
while True:
    if s[i]=='{':d+=1
    elif s[i]=='}':
        d-=1
        if d==0:break
    i+=1
proxy=s[m.start():i+1]; sonra=s[i+1:]
ayar=re.search(r'this\.Url = ([^;]+);',proxy)
# veri siniflari: ilk delegate'e kadar
k=sonra.find('public delegate')
if k>=0:
    k=sonra.rfind('\n',0,k)
    L=sonra[:k].split('\n')
    while L and L[-1].strip().startswith(('[','///')) or (L and not L[-1].strip()): L.pop()
    tipler='\n'.join(L)
    kalan=sonra[k:]
    geride=[x for x in re.findall(r'public (?:partial class|enum) (\w+)',kalan) if not x.endswith('CompletedEventArgs')]
    assert not geride,geride
else: tipler=sonra[:sonra.rstrip().rfind('}')]
assert 'System.Web' not in tipler and 'Microsoft.Web' not in tipler
pat=re.compile(r'\[System\.Web\.Services\.Protocols\.SoapDocumentMethodAttribute\("([^"]*)"([^\n]*)\)\]\n((?:\s*\[[^\n]*\]\n)*)\s*public ([\w.\[\]<>]+) (\w+)\(([^)]*)\) \{')
met=[]
for mm in pat.finditer(proxy):
    action,ozel,ekattr,ret,ad,par=mm.groups()
    rq=re.search(r'RequestNamespace="([^"]*)"',ozel); rs=re.search(r'ResponseNamespace="([^"]*)"',ozel)
    assert rq and rs and rq.group(1)==rs.group(1),ad
    assert 'RequestElementName' not in ozel and 'ResponseElementName' not in ozel and 'Bare' not in ozel,(ad,ozel)
    sonuc=ad+'Result'
    r2=re.search(r'\[return: System\.Xml\.Serialization\.Xml(?:Element|Array)Attribute\("(\w+)"',ekattr)
    if r2: sonuc=r2.group(1)
    ps=[]
    for x in [x.strip() for x in par.split(',') if x.strip()]:
        assert not x.startswith(('ref ','out ')),(ad,x)
        x=re.sub(r'^\[[^\]]*\]\s*','',x)
        t,n=x.rsplit(' ',1); ps.append((t,n))
    met.append((action,rq.group(1),ret,ad,ps,sonuc))
assert len(met)==proxy.count('SoapDocumentMethodAttribute('),(len(met),proxy.count('SoapDocumentMethodAttribute('))
o=[]
o.append('''//------------------------------------------------------------------------------
// %s web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace %s {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class %s : SoapIstemci {
'''%(sinif,ns,sinif))
if ekler:
    o.append('        private static readonly Type[] ekTipler = new Type[] { %s };\n\n        protected override Type[] EkTipler {\n            get { return ekTipler; }\n        }\n'%', '.join('typeof(%s)'%e for e in ekler))
o.append('        public %s() {\n%s        }\n'%(sinif,('            this.Url = %s;\n'%ayar.group(1)) if ayar else ''))
for action,adalani,ret,ad,ps,sonuc in met:
    imza=', '.join('%s %s'%x for x in ps)
    args=''.join(',\n                new SoapParametre("%s", %s, typeof(%s))'%(n,n,t) for t,n in ps)
    if ret=='void':
        o.append('        public void %s(%s) {\n            Cagir("%s", "%s", "%s", null, null%s);\n        }\n'%(ad,imza,action,ad,adalani,args))
    else:
        o.append('        public %s %s(%s) {\n            return (%s)Cagir("%s", "%s", "%s", typeof(%s), "%s"%s);\n        }\n'%(ret,ad,imza,ret,action,ad,adalani,ret,sonuc,args))
o.append('    }\n')
yeni='\n'.join(o)+tipler.rstrip()+'\n}\n\n#pragma warning restore 1591\n'
if not os.path.exists(p+'.eski'): shutil.copyfile(p,p+'.eski')
open(p,'w',encoding='utf-8-sig',newline='\r\n').write(yeni)
print(sinif,'metot:',len(met),'ek tip:',ekler,'ayar:',ayar.group(1) if ayar else None)
