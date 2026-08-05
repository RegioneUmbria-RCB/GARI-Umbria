Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class VisOperSiRPV

    Public Shared Function sVisOperSiRPVInput(ByVal Username As String, ByVal Password As String, _
                                         ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean, _
                                          ByVal codIcqrf As String, _
                                          ByVal startDate As Date, _
                                          ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.SoggSiRPV)

        Dim visOper As New VisOperSiRPVInput
        visOper.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)

        visOper.CodiceIcqrf = codIcqrf
        visOper.DataOperIni = startDate  'Data Operazione da cui inizia il range di ricerca

        Dim req = XMLUtility.getBodyRequest(visOper, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Shared Function sVisOperSiRPVOutput(ByVal response As String) As VisOperSiRPVOutput
        Dim visOper As New VisOperSiRPVOutput
        visOper = XMLUtility.getObjectFromResponse(response, visOper, Utility.getSoapenv)
        Return visOper
    End Function

End Class
