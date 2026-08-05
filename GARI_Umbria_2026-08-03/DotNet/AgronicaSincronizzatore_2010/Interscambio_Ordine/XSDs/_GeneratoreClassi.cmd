"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\xsd.exe" StandardBusinessDocumentHeader.xsd SharedCommon.xsd eComCommon.xsd Order.xsd OrderResponse.xsd /C /Language:vb

copy StandardBusinessDocumentHeader_SharedCommon_eComCommon_Order_OrderResponse.vb ..\OrderMessageType.vb /Y

pause