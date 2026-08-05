Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Parametri_Indice_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal ID_Indice As Integer,
                          ByVal TipoIndice As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal xSelezioneVariabile As enumSelezioneVariabile? = enumSelezioneVariabile.Selezione_TabellaCompleta
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Parametri_Indice_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Distinct Parametri_Indice.* ")
                    StrSQL.Append(" FROM  Parametri_Indice ")
                    StrSQL.Append(" WHERE Parametri_Indice.PivaSuperUser = '" & pivaSuperUser & "' ")

                    If piva <> "" Then
                        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                            StrSQL.Append(" And Parametri_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                        Else
                            If DataProviderFactory.Instance.ParametrizzaQuery Then
                                StrSQL.Append(" And Parametri_Indice.piva In (" & Agro_SQL_Save_Clausola_IN(piva, True) & ", '') ")
                            Else
                                StrSQL.Append(" And Parametri_Indice.piva In ('" & Agro_SQL_Save_Clausola_IN(piva, True) & "', '') ")
                            End If

                        End If
                    End If

                    If ID_Indice <> 0 Then
                        StrSQL.Append(" AND Parametri_Indice.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    End If

                    If Trim(TipoIndice) <> "" Then
                        StrSQL.Append(" AND Upper(Parametri_Indice.TipoIndice) = '" & Agro_SQL_SaveText(TipoIndice) & "' ")
                    End If

            End Select
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiDettagli(ByVal Piva As String,
                                  ByVal ID_Indice As Integer,
                                  ByVal TipoCampo As Integer,
                                  ByVal Elenco_Tipo As Integer,
                                  ByVal Elenco_Cod As Integer,
                                  ByVal Data_Riferimento As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Parametri_Indice_R.LeggiDettagli()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As New DataTable
        Dim pivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0

            Select Case TipoCampo

                Case 0 'Scelta Libera

                Case 1 'Valori

                    StrSQL.Append(" SELECT ID_Indice_Det as Valore_Cod, Valore as Valore_Des ")
                    StrSQL.Append(" FROM  Parametri_Indice_Dettagli ")
                    'StrSQL.Append(" Inner Join Parametri_Indice_Dettagli On ( Parametri_Indice_Dettagli.Piva = Alert_Indice.Piva And Parametri_Indice_Dettagli.Id_Indice = Alert_Indice.Id_Indice ) ")
                    StrSQL.Append(" WHERE Parametri_Indice_Dettagli.ID_Indice = " & Agro_SQL_SaveNum(ID_Indice) & " ")
                    StrSQL.Append(" And Parametri_Indice_Dettagli.PivaSuperUser = '" & pivaSuperUser & "' ")
                    'Controllo validità

                    If IsDate(Data_Riferimento) Then
                        StrSQL.Append(" And Parametri_Indice_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                        StrSQL.Append(" And Parametri_Indice_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                    End If

                    StrSQL.Append(" Order by Valore_Des ")

                Case 2 'Elenco

                    Select Case Elenco_Tipo

                        Case 1 'Unità Misura

                            StrSQL.Append(" SELECT Distinct Udm_Cod as Valore_Cod, Udm_Des as Valore_Des ")
                            StrSQL.Append(" FROM UnitaMisura" & vbCrLf)

                            If xFiltroAggiuntivo <> "" Then
                                StrSQL.Append(" Where " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                            End If

                            StrSQL.Append(" Order by Valore_Des ")

                        Case Else

                            '......

                    End Select

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class



Public Class Parametri_Indice_W
    Inherits AgronicaCoreDataProvider.DataProvider


    'Public Function Scrivi(ByVal piva As String,
    '                       ByVal id_indice As Integer,
    '                       ByVal titoloindice As String,
    '                       ByVal chkobbligatorio As Integer,
    '                       ByVal chkindice_speciale As Integer,
    '                       ByVal tipocampo As Integer,
    '                       ByVal tipodato As String,
    '                       ByVal elenco_tipo As Integer,
    '                       ByVal elenco_cod As Integer,
    '                       ByVal elenco_val As String,
    '                       ByVal validita_inizio As Date,
    '                       ByVal validita_fine As Date,
    '                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                       ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Parametri_Indice_W.Scrivi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False
    '    Dim PivaSuperUser = objParametri.PivaSuperUser

    '    Try

    '        piva = "" 'Non gestita

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("INSERT INTO Parametri_Indice ")
    '        StrSQL.Append("  (PivaSuperUser, Piva, Id_Indice, TitoloIndice, ChkObbligatorio, ChkIndice_Speciale, TipoCampo, TipoDato, Elenco_Tipo, Elenco_Cod, Elenco_Val, ")
    '        StrSQL.Append("   Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, validita_inizio, validita_fine   ")


    '        StrSQL.Append(" ) ")

    '        StrSQL.Append("VALUES (")

    '        StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
    '        StrSQL.Append("         ," & Agro_vb_SaveNum(id_indice) & " ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(titoloindice) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(chkobbligatorio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(chkindice_speciale) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(tipocampo) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(tipodato) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(elenco_tipo) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(elenco_cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(elenco_val) & "' ")
    '        StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
    '        StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
    '        StrSQL.Append("         , 0 ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_inizio) & " ")
    '        StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_fine) & " ")


    '        StrSQL.Append(" )")
    '        '---------------------------------------------


    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

End Class
