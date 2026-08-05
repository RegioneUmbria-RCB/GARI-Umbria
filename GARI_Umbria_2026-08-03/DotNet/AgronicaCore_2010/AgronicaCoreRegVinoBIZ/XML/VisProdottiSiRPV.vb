Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class VisProdottiSiRPV
    Public Function sVisProdottiSiRPVInput(ByVal Username As String, ByVal Password As String, _
                                                ByVal codIcqrf As String, _
                                                ByVal codOperCF As String, _
                                                ByVal codOperPersonaFisica As Boolean) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VisProdSiRPV)

        Dim visProdotti As New VisProdSiRPVInput
        visProdotti.CodiceIcqrf = codIcqrf
        visProdotti.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

        Dim req = XMLUtility.getBodyRequest(visProdotti, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sVisProdSiRPVOutput(ByVal response As String) As VisProdSiRPVOutput
        Dim visProd As New VisProdSiRPVOutput
        visProd = XMLUtility.getObjectFromResponse(response, visProd, Utility.getSoapenv)
        Return visProd
    End Function
End Class
