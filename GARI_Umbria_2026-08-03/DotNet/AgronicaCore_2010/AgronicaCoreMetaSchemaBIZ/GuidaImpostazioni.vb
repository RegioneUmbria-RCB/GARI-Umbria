Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.profilazione

Public Class GuidaImpostazioni

    Public Class SezioneImpostazione
        Public codice As String
        Public descrizione As String
        Public espandibile As Boolean
    End Class

    Public Sub AggiungiImpostazioneInTabellaGuida(list As Impostazione(), objParametri As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreMetaSchemaDAL.GuidaImpostazioniDAL

        For Each i In list
            objImpostazioni.AggiungiImpostazioneTabellaGuida(
                i.Impostazione_Cod, i.Impostazione_Des,
                i.Impostazione_SuperUser, 0, i.Impostazione_AziendaCentro, 0,
                i.Sezione_Cod, i.SottoSezione_Cod, i.Livello_Cod,
                objParametri
            )
        Next

    End Sub

    Public Function LeggiSezioniGuida(objParametri As AgronicaCoreParametri) As List(Of SezioneImpostazione)
        Dim objImpostazioni As New AgronicaCoreMetaSchemaDAL.GuidaImpostazioniDAL

        Return objImpostazioni.LeggiSezioniGuida("", "", objParametri).
            AsEnumerable().
            Select(Function(r) New SezioneImpostazione With {
                .codice = r.Item("Sezione_Cod"),
                .descrizione = r.Item("Label"),
                .espandibile = r.Item("Espandibile")
            }).ToList
    End Function

    ''' <summary>
    ''' Legge la tabella Guida_Impostazioni senza metterla in join con nessun'altra.
    ''' </summary>
    Public Function LeggiImpostazioni(
        leggiPerUtenti As Boolean, leggiPerSuperuser As Boolean,
        leggiPerImprese As Boolean, leggiPerCentri As Boolean,
        objParametri As AgronicaCoreParametri
    ) As DataTable
        Dim objImpostazioni As New AgronicaCoreMetaSchemaDAL.GuidaImpostazioniDAL

        Dim filtroSQL = ""
        Dim appendOrCondition = Function(str As String) If(filtroSQL = "", str, filtroSQL & " OR " & str & " ")

        If leggiPerUtenti Then
            filtroSQL = appendOrCondition("Impostazione_Utente = 1")
        End If
        If leggiPerSuperuser Then
            filtroSQL = appendOrCondition("Impostazione_SuperUser = 1")
        End If
        If leggiPerImprese Then
            filtroSQL = appendOrCondition("Impostazione_Azienda = 1")
        End If
        If leggiPerCentri Then
            filtroSQL = appendOrCondition("Impostazione_Azienda_Centro = 1")
        End If

        Return objImpostazioni.LeggiImpostazioni(filtroSQL, "", objParametri)
    End Function

End Class
