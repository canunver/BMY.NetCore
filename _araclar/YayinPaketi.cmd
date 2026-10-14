@echo off
rem Yayin paketi: cmd icinden calistirmak icin. Ornek: _araclar\YayinPaketi.cmd -Tarih "14.10.26 17:45"   ya da   -Hepsi
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0YayinPaketi.ps1" %*
