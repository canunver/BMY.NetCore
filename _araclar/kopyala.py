# -*- coding: utf-8 -*-
"""Sgb.NETv2/Sunucu altindaki secili projeleri (ve ProjectReference kapanisini) Sgb.NETCore/Sunucu altina kopyalar,
eski csproj yerine .NET 8 SDK-stil csproj uretir. Kaynak klasore yazmaz.
Kullanim: python3 kopyala.py <eskiSunucu> <yeniSunucu> <baslangic.csproj> [...]"""
import os,re,sys,shutil,json
SRC,DST=sys.argv[1],sys.argv[2]
start=[os.path.normpath(a) for a in sys.argv[3:]]
SKIPD={'bin','obj','.vs','packages'}
SKIPE={'.user','.suo','.csproj','.cd','.dll','.pdb','.cache','.vspscc','.scc'}
PKG={ # eski Reference adi -> NuGet paketi
 'Renci.SshNet':('SSH.NET','2024.2.0'),'Newtonsoft.Json':('Newtonsoft.Json','13.0.3'),
 'System.Data.OracleClient':('Oracle.ManagedDataAccess.Core','23.6.1'),
 'System.DirectoryServices':('System.DirectoryServices','8.0.0'),
 'System.DirectoryServices.Protocols':('System.DirectoryServices.Protocols','8.0.0'),
 'System.ServiceModel':('System.ServiceModel.Http','8.0.0'),
 'System.Drawing':('System.Drawing.Common','8.0.8'),
}
def rd(p): return open(p,encoding='utf-8-sig',errors='ignore').read()
seen={};q=list(start)
while q:
    p=q.pop()
    if p in seen: continue
    s=rd(os.path.join(SRC,p)); d=os.path.dirname(p)
    prefs=[os.path.normpath(os.path.join(d,r.replace('\\','/'))) for r in re.findall(r'ProjectReference Include="([^"]+)"',s)]
    seen[p]=dict(s=s,prefs=prefs); q+=prefs
rapor=[]
for p,info in sorted(seen.items()):
    s=info['s']; d=os.path.dirname(p)
    if os.path.exists(os.path.join(DST,p)):
        print('atlandi (zaten tasinmis):',p); continue
    asm=re.search(r'<AssemblyName>([^<]+)',s).group(1); rns=re.search(r'<RootNamespace>([^<]+)',s).group(1)
    comp=set(os.path.normpath(c.replace('\\','/')) for c in re.findall(r'<Compile Include="([^"]+)"',s))
    refs=[r.split(',')[0] for r in re.findall(r'<Reference Include="([^"]+)"',s)]
    # dosyalari kopyala
    ondisk=set(); n=0
    for dp,dn,fn in os.walk(os.path.join(SRC,d)):
        dn[:]=[x for x in dn if x not in SKIPD]
        # alt klasorde baska bir csproj varsa (ic ice proje) atla
        if dp!=os.path.join(SRC,d) and any(f.endswith('.csproj') for f in fn): dn[:]=[]; continue
        for f in fn:
            if os.path.splitext(f)[1].lower() in SKIPE: continue
            rel=os.path.relpath(os.path.join(dp,f),os.path.join(SRC,d))
            out=os.path.join(DST,d,rel); os.makedirs(os.path.dirname(out),exist_ok=True)
            shutil.copyfile(os.path.join(dp,f),out); n+=1
            if f.endswith('.cs'): ondisk.add(os.path.normpath(rel))
    disarda=sorted(ondisk-comp)          # diskte var ama eski projede derlenmiyor
    eksik=sorted(c for c in comp-ondisk) # projede var diskte yok / proje disi link
    pk=[]; desteksiz=[]
    for r in refs:
        if r in PKG and PKG[r] not in pk: pk.append(PKG[r])
        elif r in('System.Web','System.Web.Services','System.EnterpriseServices','System.Runtime.Remoting','Microsoft.Web.Services2','Microsoft.IdentityModel','System.IdentityModel'): desteksiz.append(r)
    x=['<Project Sdk="Microsoft.NET.Sdk">','','  <PropertyGroup>',f'    <AssemblyName>{asm}</AssemblyName>',f'    <RootNamespace>{rns}</RootNamespace>','  </PropertyGroup>','']
    if disarda:
        x.append('  <!-- Eski projede derlemeye dahil olmayan dosyalar -->'); x.append('  <ItemGroup>')
        x+= [f'    <Compile Remove="{c.replace("/",chr(92))}" />' for c in disarda]; x+=['  </ItemGroup>','']
    link=[c for c in eksik if c.startswith('..')]
    if link:
        x.append('  <ItemGroup>'); x+=[f'    <Compile Include="{c.replace("/",chr(92))}" Link="{os.path.basename(c)}" />' for c in link]; x+=['  </ItemGroup>','']
    if pk:
        x.append('  <ItemGroup>'); x+=[f'    <PackageReference Include="{a}" Version="{v}" />' for a,v in pk]; x+=['  </ItemGroup>','']
    if info['prefs']:
        x.append('  <ItemGroup>')
        for r in info['prefs']:
            x.append('    <ProjectReference Include="%s" />'%os.path.relpath(r,d).replace('/','\\'))
        x+=['  </ItemGroup>','']
    x.append('</Project>')
    open(os.path.join(DST,p),'w',encoding='utf-8',newline='\r\n').write('\n'.join(x)+'\n')
    rapor.append(dict(proje=p,asm=asm,dosya=n,derlemeDisi=disarda,eksik=eksik,desteksizReferans=desteksiz,paket=[a for a,_ in pk]))
json.dump(rapor,open(os.path.join(os.path.dirname(os.path.abspath(__file__)),'kopya_rapor.json'),'w',encoding='utf-8'),ensure_ascii=False,indent=1)
for r in rapor: print(r['proje'],r['dosya'],'disi:',len(r['derlemeDisi']),'eksik:',r['eksik'],'desteksiz:',r['desteksizReferans'],'paket:',r['paket'])
