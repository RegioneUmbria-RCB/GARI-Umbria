Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions


Public Class SincroBDNConsorzi
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri
    Dim codice_consorzio As String
    Dim wsGestioneAssConsorzi As ChiamawsGestioneAssConsorzi
    Dim AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)

        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig
        Me.codice_consorzio = "INA"
    End Sub

    Public Function GetAziende() As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dt As DataTable = objFabbricati.LeggiStalle_x_anagrafica("", 0, 0, " Stalla.BDN_Codice_Azienda <> '' ", "", objParametriServer, True, True)
        dt.Columns.Add("InGias", GetType(String))
        dt.Columns.Add("InBDN", GetType(String))
        'DTAziendeGias.Columns.Add("Sa_Cod", GetType(Integer))
        'DTAziendeGias.Columns.Add("STA_NUM", GetType(Integer))
        'DTAziendeGias.Columns.Add("Rag_Soc", GetType(Integer))
        'DTAziendeGias.Columns.Add("sa_nome", GetType(String))
        'DTAziendeGias.Columns.Add("STA_DES", GetType(String))
        'DTAziendeGias.Columns.Add("BDN_Codice_Azienda", GetType(String))

        Dim aziende As New List(Of String)
        Dim aziendeToAdd As New List(Of String)
        Dim dtAziende = Me.wsGestioneAssConsorzi.Get_Aziende_Consorzio("", codice_consorzio)
        For Each row In dtAziende.Rows
            aziende.Add(row("AZIENDA_CODICE"))
            aziendeToAdd.Add(row("AZIENDA_CODICE"))
        Next


        For Each row In dt.Rows
            Dim codice_Azienda = row("BDN_Codice_Azienda")
            row("InGias") = "SI"
            row("InBDN") = "NO"
            If aziende.Contains(codice_Azienda) Then
                row("InBDN") = "SI"
                aziendeToAdd.Remove(codice_Azienda)
            End If
        Next

        For Each aziendaToAdd In aziendeToAdd
            Dim row = dt.NewRow
            row("BDN_Codice_Azienda") = aziendaToAdd
            row("chiave") = aziendaToAdd
            row("InGias") = "NO"
            row("InBDN") = "SI"
            dt.Rows.Add(row)
        Next

        Return dt
    End Function

    Public Function InsertAzienda(codice_azienda As String) As String
        Try
            Dim response As DataTable = Me.wsGestioneAssConsorzi.Insert_Azienda_Consorzio(0, codice_azienda, codice_consorzio)
            If response.Rows.Count > 0 Then
                Return "Azienda Attivata correttamente"
            End If
            Return ""
        Catch ex As BDNException
            Return ex.Message
        Catch ex As GiasException
            Return ex.Message
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

End Class
