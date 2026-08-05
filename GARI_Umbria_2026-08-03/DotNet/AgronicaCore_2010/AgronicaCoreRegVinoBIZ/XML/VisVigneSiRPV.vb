Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class VisVigneSiRPV

    Public Shared Function sVisVigneSiRPVInput(ByVal Username As String, ByVal Password As String, _
                                        ByVal codOperCF As String, _
                                         ByVal codOperPersonaFisica As Boolean, _
                                         ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VisVasiSiRPV)

        Dim visVigne As New VisVigneSiRPVInput
        visVigne.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

        Dim req = XMLUtility.getBodyRequest(visVigne, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Shared Function sVisVigneSiRPVOutput(ByVal response As String) As VisVigneSiRPVOutput
        Dim visVigne As New VisVigneSiRPVOutput
        visVigne = XMLUtility.getObjectFromResponse(response, visVigne, Utility.getSoapenv)
        Return visVigne
    End Function

End Class
