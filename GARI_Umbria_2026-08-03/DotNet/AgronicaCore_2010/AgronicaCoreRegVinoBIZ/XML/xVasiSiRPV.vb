Imports AgronicaCoreDataProvider
Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreUtility

Public Class xVasiSiRPV

    Dim reader As xDBVasiSiRPV_R
    Dim writer As xDBVasiSiRPV_W

    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        ObjParametri_Server = server
        ObjParametri_Utenti = utenti
        reader = New AgronicaCoreRegVinoDAL.xDBVasiSiRPV_R()
        writer = New AgronicaCoreRegVinoDAL.xDBVasiSiRPV_W()
    End Sub

    Public Function sVasiSiRPV(ByVal Username As String, ByVal Password As String, _
                                      ByVal codiciVasi As List(Of String), _
                                      ByVal tipoRichiesta As Integer, _
                                      ByVal codIcqrf As String, _
                                      ByVal codOperCF As String, _
                                      ByVal codOperPersonaFisica As Boolean) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.VasiSiRPV)

        Dim vasiSiRPV As VasiSiRPVInput = New VasiSiRPVInput

        vasiSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        vasiSiRPV.CodiceIcqrf = codIcqrf
        vasiSiRPV.TipoRichiesta = tipoRichiesta
        vasiSiRPV.Vasi = getVasi(codiciVasi, codIcqrf, codOperCF)

        Dim req = XMLUtility.getBodyRequest(vasiSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sGetVasiSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetVasiSiRPV)

        Dim getvasiSiRPV As New GetVasiSiRPVInput

        getvasiSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getvasiSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sCancVasiSiRPV(ByVal Username As String, ByVal Password As String, _
                                          ByVal codIcqrf As String, _
                                          ByVal codOperCF As String, _
                                          ByVal codOperPersonaFisica As Boolean, _
                                          ByVal codiciVasi As List(Of String)) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.CancVasiSiRPV)

        Dim cancVasiSiRPV As New CancVasiSiRPVInput

        cancVasiSiRPV.CodOper = Utility.getCodOper(codOperCF, codOperPersonaFisica)
        cancVasiSiRPV.CodiceIcqrf = codIcqrf
        cancVasiSiRPV.VasoElimina = getVasiElimina(codiciVasi, codIcqrf, codOperCF)
        Dim req = XMLUtility.getBodyRequest(cancVasiSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

    Public Function sGetCancVasiSiRPV(ByVal Username As String, ByVal Password As String, ByVal idTrasmissione As String) As String
        Dim xml = Utility.getIntestazione(Username, Password, Utility.GetCancVasiSiRPV)

        Dim getcancvasiSiRPV As New GetCancVasiSiRPVInput

        getcancvasiSiRPV.IdTrasmissione = idTrasmissione

        Dim req = XMLUtility.getBodyRequest(getcancvasiSiRPV, "wsm", "http://cooperazione.sian.it/schema/wsmrga/")

        XMLUtility.addBody(xml, req, Utility.getSoapenv)
        Utility.cleanXmlSend(xml)
        Return xml.ToString
    End Function

#Region "Supporto alle Operazioni"

    Public Function getVaso(ByVal codiceVaso As String, ByVal codiceIcqrf As String, codOper As String) As VasoVinario
        Dim vaso As New VasoVinario
        Try
            Dim dt = reader.getVaso(ObjParametri_Server, codiceVaso, codiceIcqrf, codOper)
            Dim row As DataRow = dt.Rows(0)
            vaso.CodVaso = row.Item("CodVaso")
            vaso.Descrizione = row.Item("Descrizione")
            vaso.TipoVaso = row.Item("TipoVaso")
            vaso.Volume = CInt(row.Item("Volume"))
            writer.aggiornaStato(ObjParametri_Server, codiceVaso, codiceIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
        Catch ex As Exception
            writer.aggiornaStato(ObjParametri_Server, codiceVaso, codiceIcqrf, Utility.StatoGIAS.non_valida_per_invio, codOper)
        End Try
        Return vaso

    End Function

    Public Function getVasi(ByVal codiciVasi As List(Of String), ByVal codiceIcqrf As String, codOper As String) As VasoVinario()
        Dim vasi As New List(Of VasoVinario)
        For Each codiceVaso As String In codiciVasi
            vasi.Add(getVaso(codiceVaso, codiceIcqrf, codOper))
        Next
        Return vasi.ToArray
    End Function

    Private Function getVasiElimina(codiciVasi As List(Of String), codiceIcqrf As String, codOper As String) As VasoElimina()
        Dim vasiElimina As New List(Of VasoElimina)
        For Each codiceVaso As String In codiciVasi
            vasiElimina.Add(getVasoElimina(codiceVaso, codiceIcqrf, codOper))
        Next
        Return vasiElimina.ToArray
    End Function

    Private Function getVasoElimina(codiceVaso As String, codiceIcqrf As String, codOper As String) As VasoElimina
        Dim vaso As New VasoElimina
        Try
            vaso.CodVaso = codiceVaso
            writer.aggiornaStato(ObjParametri_Server, codiceVaso, codiceIcqrf, Utility.StatoGIAS.valida_per_invio, codOper)
        Catch ex As Exception
            writer.aggiornaStato(ObjParametri_Server, codiceVaso, codiceIcqrf, Utility.StatoGIAS.non_valida_per_invio, codOper)
        End Try
        Return vaso
    End Function

#End Region

End Class
