www.ismaillowkey.net

this example windows service can run external winform application 

-- add configuration.ini 
[App]
AppNameRunning = App
PathRunExe = D:\IsmailLowkey\App.exe

-- for installing windows service
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe yourService.exe

-- for uninstalling windows service
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u yourService.exe