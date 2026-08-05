Imports AgronicaCoreDataProvider
Imports System.Xml.Serialization
Imports AgronicaCoreUtility

Public Class VisSoggSiRPV

    Public Function sVisSoggSiRPVInput(ByVal Username As String, ByVal Password As String, _
                                         ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VisSoggSiRPV)

        Dim visSogg As New VisSoggSiRPVInput
        visSogg.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

        Dim req = XMLUtility.getBodyRequest(visSogg, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sVisSoggSiRPVOutput(ByVal response As String) As VisSoggSiRPVOutput
        Dim visSogg As New VisSoggSiRPVOutput()
        visSogg = XMLUtility.getObjectFromResponse(response, visSogg, Utility.getSoapenv)
        Return visSogg
    End Function

End Class
