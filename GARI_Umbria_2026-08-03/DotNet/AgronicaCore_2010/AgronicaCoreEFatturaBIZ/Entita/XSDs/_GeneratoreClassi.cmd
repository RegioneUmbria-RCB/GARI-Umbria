"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\xsd.exe" Schema_VFSM10.xsd xmldsig-core-schema.xsd /C /Language:vb /namespace:Entita.Semplificata
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\xsd.exe" Schema_VFPR12.xsd xmldsig-core-schema.xsd /C /Language:vb /namespace:Entita.FatturaPa

copy Schema_VFSM10_xmldsig-core-schema.vb ..\FatturaSemplificata.vb /Y
copy Schema_VFPR12_xmldsig-core-schema.vb ..\FatturaPA.vb /Y

pause
