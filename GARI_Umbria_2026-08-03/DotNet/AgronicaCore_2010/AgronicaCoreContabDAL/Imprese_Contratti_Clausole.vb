Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider



Public Class Imprese_Contratti_Clausole_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PIVA As String,
                          ByVal Clausola_Cod As Integer,
                          ByVal Cau_Contratto As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_Contratti_Clausole_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Imprese_Contratti_Clausole.*, (Clausola_Numero + ' - ' + Clausola_Nome) as Clausola_Nome_Esteso, ")
                    StrSQL.Append(" CASE WHEN Cau_Contratto = '9300' THEN 'Contratto Conferimento Colturale' ELSE '' END AS Cau_Contratto_Des ")
                    StrSQL.Append(" FROM  Imprese_Contratti_Clausole  ")
                    StrSQL.Append(" WHERE Imprese_Contratti_Clausole.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Imprese_Contratti_Clausole.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Imprese_Contratti_Clausole.Piva In ('', '" & Agro_SQL_SaveText(PIVA) & "')   ")
                    End If

                    If Clausola_Cod <> 0 Then
                        StrSQL.Append(" AND Imprese_Contratti_Clausole.Clausola_Cod = " & Agro_SQL_SaveNum(Clausola_Cod) & "   ")
                    End If

                    If Cau_Contratto <> "" Then
                        StrSQL.Append(" AND Imprese_Contratti_Clausole.Cau_Contratto  = '" & Agro_SQL_SaveText(Cau_Contratto) & "'   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Contratti_Clausole.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Contratti_Clausole.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Piva ASC, Clausola_Numero ASC, Clausola_Nome ASC")
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
    
End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Imprese_Contratti_Clausole_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Clausola_Cod As Long,
                           ByVal Clausola_Numero As String,
                           ByVal Clausola_Nome As String,
                           ByVal Clausola_Des As String,
                           ByVal Cau_Contratto As String,
                           ByVal Clausola_Cod_Alternativo As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Imprese_Contratti_Clausole_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Imprese_Contratti_Clausole ")
            StrSQL.Append("  (Piva_SuperUser, Piva,          Clausola_Cod,      Clausola_Numero,  Clausola_Des,    Clausola_Nome, Cau_Contratto,     Clausola_Cod_Alternativo, ")
            StrSQL.Append("   Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, validita_inizio, validita_fine   ")


            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(Clausola_Cod) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Clausola_Numero) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Clausola_Des) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Clausola_Nome) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cau_Contratto) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Clausola_Cod_Alternativo) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Fine) & " ")
            
            StrSQL.Append(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal piva As String,
                             ByVal Clausola_Cod As Long,
                             ByVal Clausola_Numero As String,
                             ByVal Clausola_Nome As String,
                             ByVal Clausola_Des As String,
                             ByVal Cau_Contratto As String,
                             ByVal Clausola_Cod_Alternativo As String,
                             ByVal validita_inizio As Date,
                             ByVal validita_fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Imprese_Contratti_Clausole_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Imprese_Contratti_Clausole SET ")
            StrSQL.Append("  piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append(" ,Clausola_Numero = '" & Agro_SQL_SaveText(Clausola_Numero) & "' ")
            StrSQL.Append(" ,Clausola_Nome = '" & Agro_SQL_SaveText(Clausola_Nome) & "' ")
            StrSQL.Append(" ,Clausola_Des = '" & Agro_SQL_SaveText(Clausola_Des) & "' ")
            StrSQL.Append(" ,Cau_Contratto = '" & Agro_SQL_SaveText(Cau_Contratto) & "' ")
            StrSQL.Append(" ,Clausola_Cod_Alternativo = '" & Agro_SQL_SaveText(Clausola_Cod_Alternativo) & "' ")
            StrSQL.Append(" ,Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & " ")
            StrSQL.Append(" ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append(" ,validita_inizio = " & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append(" ,validita_fine = " & Agro_SQL_SaveDate(validita_fine) & " ")

            StrSQL.Append(" WHERE Clausola_Cod = " & Agro_SQL_SaveNum(Clausola_Cod) & " ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND Piva_SuperUser = '" & PivaSuperUser & "'")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal Piva As String,
                             ByVal Clausola_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Imprese_Contratti_Clausole_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            'Cancellazione preliminare ContrattoxClausola
            xRisp = CancellaContrattoxClausola(Piva, 0, Clausola_Cod, "", objParametri)

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Imprese_Contratti_Clausole ")
            StrSQL.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Clausola_Cod = " & Agro_SQL_SaveNum(Clausola_Cod) & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ScriviContrattoxClausola(ByVal Piva As String,
                                             ByVal Contratto_Cod As Integer,
                                             ByVal Clausola_Cod As Integer,
                                             ByVal Fase_Cod As Integer,
                                             ByVal Data As Date,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Imprese_Contratti_Clausole_W.ScriviContrattoxClausola()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Imprese_ContrattixClausole ")
            StrSQL.Append("  (Piva_SuperUser, Piva,          Contratto_Cod,      Clausola_Cod,  Fase_Cod,    Data, ")
            StrSQL.Append("   Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, validita_inizio, validita_fine   ")

            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(Contratto_Cod) & " ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(Clausola_Cod) & " ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(Fase_Cod) & " ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Data) & "  ")

            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Fine) & " ")
            
            StrSQL.Append(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaContrattoxClausola(ByVal Piva As String,
                                               ByVal Contratto_Cod As Integer,
                                               ByVal Clausola_Cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreContab_DAL.Imprese_Contratti_Clausole_W.CancellaContrattoxClausola()"

        '====================================================================================
        'Parametri opzionali :

        '- Contratto_Cod
        '- Clausola_Cod
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Imprese_ContrattixClausole ")
            StrSQL.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Contratto_Cod <> 0 Then
                StrSQL.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            End If

            If Clausola_Cod <> 0 Then
                StrSQL.Append(" AND Clausola_Cod = " & Agro_SQL_SaveNum(Clausola_Cod) & "   ")
            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
