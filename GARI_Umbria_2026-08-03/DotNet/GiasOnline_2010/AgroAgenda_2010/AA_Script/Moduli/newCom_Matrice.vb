Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Module newCom_Matrice

    Public Function NewCom_LeggiTabella_da_CategorieMagazzino(ByRef objParametri_Server As AgronicaCoreParametri, _
                                                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                                ByRef objPage As System.Web.UI.Page, _
                                                                ByVal NomeTabella As String, _
                                                                ByVal NomeCodice As String, _
                                                                ByVal NomeDescrizione As String, _
                                                                Optional ByVal TestoRicerca As String = "", _
                                                                Optional ByVal Codice As Integer = 0, _
                                                                Optional ByVal FiltroAggiuntivo As String = "")


        '----- Descrizione
        Dim DescrizioneFunzione As String = "CategorieMagazzino_Leggi"

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim MessaggioErrore As String
        Dim DT As New DataTable
        'Dim i As Integer = 0

        StrSQL = ""
        'StrSQL += " SELECT  " & NomeCodice & ", " & NomeDescrizione
        StrSQL += " SELECT  * "
        StrSQL += " FROM " & NomeTabella
        StrSQL += " WHERE " & NomeDescrizione & " LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%' "
        If NomeCodice <> "" And Codice <> 0 Then
            StrSQL += " AND " & NomeCodice & " = " & Agro_SQL_SaveNum(Codice) & " "
        End If
        If FiltroAggiuntivo <> "" Then
            StrSQL += FiltroAggiuntivo
        End If
        StrSQL += " ORDER BY " & NomeDescrizione


        'If Elem_Cod <> 0 Then
        '    If i = 0 Then
        '        StrSQL += " WHERE   (Elem_Cod = " & SQL_SaveNum(Elem_Cod) & ")   "
        '        i = i + 1
        '    Else
        '        StrSQL += " AND   (Elem_Cod = " & SQL_SaveNum(Elem_Cod) & ")   "
        '    End If
        'End If


        '----------------------------------------------------
        '--- Recupero il datatable --------------------------
        '----------------------------------------------------

        'Recupero il datatable
        DT = objSQL.EseguiQuery_Lettura( _
                objParametri_Server, _
                StrSQL, _
                DescrizioneFunzione _
        )

        'Elimino l'oggetto


        'Verifico la presenza di errori
        If Not IsNothing(MessaggioErrore) Then
            Throw New Exception("Modulo NewCom : " & DescrizioneFunzione & " : " & MessaggioErrore)
            Return Nothing
        Else
            Return DT
        End If



    End Function



End Module
