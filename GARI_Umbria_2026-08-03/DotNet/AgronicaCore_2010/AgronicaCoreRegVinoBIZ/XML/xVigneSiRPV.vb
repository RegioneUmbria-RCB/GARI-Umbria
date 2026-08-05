Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class xVigneSiRPV

    Public Shared Function sVigneSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal codiciVigne As List(Of String), _
                                      ByVal tipoRichiesta As Integer, _
                                      ByVal codIcqrf As String, _
                                      ByVal codOperCF As String, _
                                      ByVal codOperPersonaFisica As Boolean, _
                                      ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VigneSiRPV)

        Dim vigneSiRPV As New VigneSiRPVInput

        vigneSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        vigneSiRPV.CodiceIcqrf = codIcqrf
        vigneSiRPV.TipoRichiesta = tipoRichiesta
        vigneSiRPV.Vigne = getVigne(codiciVigne)

        Dim req = XMLUtility.getBodyRequest(vigneSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Shared Function sGetVigneSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String, ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetVigneSiRPV)

        Dim getVigneSiRPV As New GetVigneSiRPVInput

        getVigneSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getVigneSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Shared Function sCancVigneSiRPV(ByVal Username As String, ByVal Password As String, _
                                          ByVal codIcqrf As String, _
                                          ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean, _
                                          ByVal codiciVigne As List(Of String), ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim xml = Utility.getIntestazione(Username, Password, Utility.CancVigneSiRPV)

        Dim cancVigneSiRPV As New CancVigneSiRPVInput

        cancVigneSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        cancVigneSiRPV.CodiceIcqrf = codIcqrf
        cancVigneSiRPV.VignaElimina = getVigneElimina(codiciVigne)

        Dim req = XMLUtility.getBodyRequest(cancVigneSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString

    End Function

    Public Shared Function sGetCancVigneSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String, ByVal objParametri_server As AgronicaCoreParametri) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetCancVigneSiRPV)

        Dim getCancVigneSiRPV As New GetCancVasiSiRPVInput

        getCancVigneSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getCancVigneSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

#Region "Supporto alle Operazioni"

    Public Shared Function getVigne(ByVal codiciVigne As List(Of String)) As Vigna()
        Dim vigne As New List(Of Vigna)
        For Each codiceVigna As String In codiciVigne
            vigne.Add(getVigna(codiceVigna))
        Next
        Return vigne.ToArray
    End Function

    Private Shared Function getVigna(codiceVigna As String) As Vigna
        Dim vigna As New Vigna
        vigna.CodVigna = codiceVigna
        vigna.Descrizione = "descrizione Vigna"
        Return vigna
    End Function

    Private Shared Function getVigneElimina(codiciVigne As List(Of String)) As VignaElimina()
        Dim vigneEl As New List(Of VignaElimina)
        For Each codiceVigna As String In codiciVigne
            vigneEl.Add(getVignaElimina(codiceVigna))
        Next
        Return vigneEl.ToArray
    End Function

    Private Shared Function getVignaElimina(codiceVigna As String) As VignaElimina
        Dim vignaEl As New VignaElimina
        vignaEl.CodVigna = codiceVigna
        Return vignaEl
    End Function

#End Region

End Class
