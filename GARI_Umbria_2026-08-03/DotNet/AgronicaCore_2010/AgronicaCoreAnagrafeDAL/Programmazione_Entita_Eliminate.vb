Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class Programmazione_Entita_Eliminate_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Campo_Cod As Integer,
                           ByVal Programmazione_Cod As Integer,
                           ByVal Programmazione_Entita_Cod As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal Operazione_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            strSql.Append(" INSERT INTO Programmazione_Entita_Eliminate ( ")
            strSql.Append("             Piva_SuperUser,     Piva,                       Sa_Cod, ")
            strSql.Append("             Appezza,            Id_Reg,                     Campo_Cod, ")
            strSql.Append("             Programmazione_Cod, Programmazione_Entita_Cod,  Progetto_Cod, Operazione_Cod, ")

            strSql.Append("             Inviato,                ")
            strSql.Append("             Data_Creazione,         Data_Modifica, ")
            strSql.Append("             UserName_Creazione,     UserName_Modifica, ")
            strSql.Append("             Validita_Inizio,        Validita_Fine ")
            strSql.Append(" ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser.ToString) & "' ")
            strSql.Append("        ,'" & Agro_SQL_SaveText(Piva.ToString) & "' ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Appezza.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Id_Reg.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Campo_Cod.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Progetto_Cod.ToString) & " ")
            strSql.Append("         ," & Agro_SQL_SaveNum(Operazione_Cod.ToString) & " ")
            strSql.Append("         , 0  ")

            strSql.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva">OBBLIGATORIO</param>
    ''' <param name="Sa_Cod">OBBLIGATORIO</param>
    ''' <param name="Appezza">OBBLIGATORIO</param>
    ''' <param name="Id_Reg">OBBLIGATORIO</param>
    ''' <param name="Campo_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Programmazione_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Programmazione_Entita_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Progetto_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Validita_Inizio">DataDefault_per_MODIFICA</param>
    ''' <param name="Validita_Fine">DataDefault_per_MODIFICA</param>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal Programmazione_Cod As Integer,
                             ByVal Programmazione_Entita_Cod As Integer,
                             ByVal Progetto_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'controllo i campi obbligatori
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If
            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If
            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Append(" UPDATE Programmazione_Entita_Eliminate SET  ")

            If Campo_Cod <> IntDefault_per_MODIFICA Then
                strSql.Append("    Campo_Cod     = " & Agro_SQL_SaveNum(Campo_Cod) & "  ,")
            End If

            If Programmazione_Cod <> IntDefault_per_MODIFICA Then
                strSql.Append("    Programmazione_Cod     = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ,")
            End If

            If Programmazione_Entita_Cod <> IntDefault_per_MODIFICA Then
                strSql.Append("    Programmazione_Entita_Cod     = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ,")
            End If

            If Progetto_Cod <> IntDefault_per_MODIFICA Then
                strSql.Append("    Progetto_Cod     = " & Agro_SQL_SaveNum(Progetto_Cod) & "  ,")
            End If

            If Validita_Inizio <> DataDefault_per_MODIFICA Then
                strSql.Append("    Validita_Inizio     = " & Agro_SQL_SaveText(Validita_Inizio) & "  ,")
            End If

            If Validita_Fine <> DataDefault_per_MODIFICA Then
                strSql.Append("    Validita_Fine     = " & Agro_SQL_SaveText(Validita_Fine) & " ,")
            End If

            'rimuovo l ultima virgola
            strSql.Remove(strSql.Length - 1, 1)

            strSql.Append(" WHERE Piva     = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.Append(" AND   Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.Append(" AND   Appezza =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            strSql.Append(" AND   Id_Reg =  " & Agro_SQL_SaveText(Id_Reg) & "   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Programmazione_Cod As Integer,
                             ByVal Programmazione_Entita_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""         => Vengono cancellate tutte le entità del db
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '   Programmazione_Entita_Cod=0 => Vengono cancellate tutte le entità di una programmazione
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Programmazione_Entita_Eliminate ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM     Programmazione_Entita_Eliminate ")
                strSql.Append(" WHERE   1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append("  AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                strSql.Append("  AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                strSql.Append("  AND   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
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

Public Class Programmazione_Entita_Eliminate_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function Leggi(ByVal Programmazione_Cod As Integer,
                          ByRef MessaggioErrore As String,
                          ByVal Programmazione_Entita_Cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New Text.StringBuilder

        Try

            strSql.Length = 0

            strSql.Append(" SELECT * ")
            strSql.Append(" FROM    Programmazione_Entita_Eliminate ")
            strSql.Append(" WHERE   Programmazione_Entita_Eliminate.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser.ToString) & "' ")
            strSql.Append(" AND     Programmazione_Entita_Eliminate.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.Append(" AND     Programmazione_Entita_Eliminate.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Programmazione_Cod <> 0 Then
                strSql.Append(" AND Programmazione_Entita_Eliminate.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                strSql.Append(" AND Programmazione_Entita_Eliminate.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Programmazione_Entita_Eliminate.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Programmazione_Entita_Eliminate.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

End Class
