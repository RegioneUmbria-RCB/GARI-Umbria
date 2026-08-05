Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class VisVasiSiRPV

    Public Function sVisVasiSiRPVInput(ByVal Username As String, ByVal Password As String, _
                                                ByVal codIcqrf As String, _
                                                ByVal codOperCF As String, _
                                                ByVal codOperPersonaFisica As Boolean) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VisVasiSiRPV)

        Dim visVasi As New VisVasiSiRPVInput
        visVasi.CodiceIcqrf = codIcqrf
        visVasi.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

        Dim req = XMLUtility.getBodyRequest(visVasi, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sVisVasiSiRPVOutput(ByVal response As String) As VisVasiSiRPVOutput
        Dim visVasi As New VisVasiSiRPVOutput
        visVasi = XMLUtility.getObjectFromResponse(response, visVasi, Utility.getSoapenv)
        Return visVasi
    End Function

End Class
