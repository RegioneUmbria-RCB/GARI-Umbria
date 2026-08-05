Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Reg_Impianti_XIndiciProduttivitaAI_R
    Inherits DataProvider

    Public Function Leggi(ByVal IDIndiciProduttivita As Integer,
                          ByVal piva As String,
                          ByVal sa_cod As Integer,
                          ByVal appezza As Integer,
                          ByVal id_reg As Integer,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As Date = AGRODATAFINE) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------

                    'Query per il prelievo dei dati                 ' #### CLASSE ####

                    StrSQL.Append(" SELECT Reg_Impianti_XIndiciProduttivitaAi.* ")
                    StrSQL.Append(" FROM   Reg_Impianti_XIndiciProduttivitaAi ")
                    StrSQL.Append(" WHERE  Reg_Impianti_XIndiciProduttivitaAi.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                    StrSQL.Append(" AND    Reg_Impianti_XIndiciProduttivitaAi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Fine) & " ")

                    If IDIndiciProduttivita <> 0 Then
                        StrSQL.Append(" AND IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
                    End If
                    If piva <> "" Then
                        StrSQL.Append(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    End If
                    If sa_cod <> 0 Then
                        StrSQL.Append(" AND sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
                    End If
                    If appezza <> 0 Then
                        StrSQL.Append(" AND appezza = " & Agro_SQL_SaveNum(appezza) & " ")
                    End If
                    If id_reg <> 0 Then
                        StrSQL.Append(" AND id_Reg = " & Agro_SQL_SaveNum(id_reg) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
                    '------------------------------

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class
Public Class Reg_Impianti_XIndiciProduttivitaAI_W
    Inherits DataProvider

    Public Function Scrivi(ByVal IDIndiciProduttivita As Integer,
                           ByVal piva As String,
                           ByVal sa_cod As Integer,
                           ByVal appezza As Integer,
                           ByVal id_reg As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Reg_Impianti_XIndiciProduttivitaAi( ")
            StrSQL.AppendLine("            IndiciProduttivitaAi_COD, piva, ")
            StrSQL.AppendLine("            sa_Cod, appezza, id_reg, ")

            StrSQL.AppendLine("            Inviato, DataInvio, ")
            StrSQL.AppendLine("            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("            Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("            ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("           " & Agro_SQL_SaveNum(IDIndiciProduttivita) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(piva) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(appezza) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(id_reg) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , NULL  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

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
    Public Function Modifica(ByVal IDIndiciProduttivita As Integer,
                             ByVal piva As String,
                             ByVal sa_cod As Integer,
                             ByVal appezza As Integer,
                             ByVal id_reg As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                             Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi__XReg_Impianti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Reg_Impianti_XIndiciProduttivitaAi SET ")
            StrSQL.AppendLine("   Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE 1=1 ")
            If IDIndiciProduttivita <> 0 Then
                StrSQL.AppendLine(" and IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
            End If
            If piva <> "" Then
                StrSQL.AppendLine(" And   piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If sa_cod <> 0 Then
                StrSQL.AppendLine(" AND   sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If
            If appezza <> 0 Then
                StrSQL.AppendLine(" AND   appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If
            If id_reg <> 0 Then
                StrSQL.AppendLine(" AND   id_reg = " & Agro_SQL_SaveNum(id_reg) & " ")
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
    Public Function Cancella(ByVal IDIndiciProduttivita As Integer,
                             ByVal piva As String,
                             ByVal sa_cod As Integer,
                             ByVal appezza As Integer,
                             ByVal id_reg As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi__XReg_Impianti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Reg_Impianti_XIndiciProduttivitaAi ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If piva <> "" Then
                StrSQL.AppendLine(" and piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If sa_cod <> 0 Then
                StrSQL.AppendLine(" AND sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If
            If appezza <> 0 Then
                StrSQL.AppendLine(" AND appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If
            If id_reg <> 0 Then
                StrSQL.AppendLine(" AND id_reg = " & Agro_SQL_SaveNum(id_reg) & " ")
            End If
            If IDIndiciProduttivita <> 0 Then
                StrSQL.AppendLine(" AND IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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



End Class
