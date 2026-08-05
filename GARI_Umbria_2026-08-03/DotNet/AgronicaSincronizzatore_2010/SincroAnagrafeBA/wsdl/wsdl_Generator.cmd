
rem parte client
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\SvcUtil.exe" /out:FascicoloBAb.vb /language:VB http://web_collesb.coldiretti.it/FascicoloMMWeb/sca/FascicoloWS/wsdl/FAscicoloMM_FascicoloWS.wsdl 

rem parte server
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64\SvcUtil.exe" /out:iFascicoloBAb.vb /language:VB /serverInterface http://web_collesb.coldiretti.it/FascicoloMMWeb/sca/FascicoloWS/wsdl/FAscicoloMM_FascicoloWS.wsdl 

pause