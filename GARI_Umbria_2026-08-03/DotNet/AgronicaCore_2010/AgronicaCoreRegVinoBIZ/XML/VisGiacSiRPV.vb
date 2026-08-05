Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Public Class VisGiacSiRPV
    Public Function sVisGiacSiRPVInput(ByVal Username As String, ByVal Password As String,
                                                ByVal codIcqrf As String,
                                                ByVal codOperCF As String,
                                                ByVal codOperPersonaFisica As Boolean,
                                                ByVal dataRiferimento As Date) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VisGiacSiRPV)

        Dim visGiacenze As New VisGiacSiRPVInput

        visGiacenze.CodiceIcqrf = codIcqrf
        visGiacenze.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        visGiacenze.DataGiacenza = dataRiferimento

        Dim req = XMLUtility.getBodyRequest(visGiacenze, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sVisGiacSiRPVOutput(ByVal response As String) As VisGiacSiRPVOutput
        Dim visGiac As New VisGiacSiRPVOutput
        visGiac = XMLUtility.getObjectFromResponse(response, visGiac, Utility.getSoapenv)
        Return visGiac
    End Function
End Class
