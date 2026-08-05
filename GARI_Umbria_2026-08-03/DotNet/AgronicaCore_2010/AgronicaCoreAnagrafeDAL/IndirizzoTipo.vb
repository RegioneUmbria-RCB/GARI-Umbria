Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class IndirizzoTipo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal TipoIndirizzo_Cod As Integer,
                          ByVal Cod_Contatto As String,
                          ByVal Contatto_Tipo As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NumIndirizzoTipo As Integer = 0

        Try

            If String.IsNullOrEmpty(Piva) Then
                Throw New Exception("PIVA è obbligatoria nel filtro")
            End If


            stb.AppendLine(" Select ")
            stb.AppendLine(" * ")
            stb.AppendLine(" From IndirizzoTipo ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine(" piva = '" & Agro_SQL_SaveText(Piva) & "'")

            If TipoIndirizzo_Cod <> -1 Then
                stb.AppendLine(" AND IndirizzoTipo_Cod = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod))
            End If

            If Contatto_Tipo <> -1 Then
                stb.AppendLine(" AND Contatto_Tipo = " & Agro_SQL_SaveNum(Contatto_Tipo))
            End If

            '' TODO
            If Cod_Contatto <> "-99" Then
                If Not String.IsNullOrEmpty(Cod_Contatto) Then
                    stb.AppendLine(" AND (Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' Or Cod_Contatto = '') ")
                Else
                    stb.AppendLine(" AND Cod_Contatto = ''")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY  Descrizione ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function InUso(ByVal TipoIndirizzo_Cod As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_R.Leggi<<<<estesa()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risposta As Boolean = False

        Try

            If TipoIndirizzo_Cod = 0 Then
                Throw New Exception("TipoIndirizzo_Cod è obbligatoria nel filtro")
            End If

            stb.AppendLine(" Select top 1 * ")
            stb.AppendLine(" from movimenti ")
            stb.AppendLine(" where ")
            stb.AppendLine(" cod_indirizzoRisum = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod) & " ")
            stb.AppendLine(" OR con_indirizzoDestinazione = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod) & " ")
            stb.AppendLine(" OR cod_IndirizzoVettore = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod) & " ")
            stb.AppendLine(" OR cod_Indirizzo_Aggiuntivo = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

            risposta = (Not DT Is Nothing AndAlso DT.Rows.Count > 0)
            Return risposta


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    Public Function LeggiEstesa(ByVal Piva As String,
                          ByVal TipoIndirizzo_Cod As Integer,
                          ByVal Cod_Contatto As String,
                          ByVal Contatto_Tipo As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_R.Leggi<<<<estesa()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NumIndirizzoTipo As Integer = 0

        Try

            If String.IsNullOrEmpty(Piva) Then
                Throw New Exception("PIVA è obbligatoria nel filtro")
            End If

            stb.AppendLine(" Select distinct ")
            stb.AppendLine(" it.*, c.Nome, c.Cognome, c.Rag_Soc ")
            stb.AppendLine(" From IndirizzoTipo it ")
            stb.AppendLine(" Left join Contatti c ")
            stb.AppendLine(" on it.Cod_Contatto = c.Cod_Contatto ")
            'stb.AppendLine(" and it.piva = c.piva ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine(" it.piva = '" & Agro_SQL_SaveText(Piva) & "'")

            If TipoIndirizzo_Cod <> -1 Then
                stb.AppendLine(" AND IndirizzoTipo_Cod = " & Agro_SQL_SaveNum(TipoIndirizzo_Cod))
            End If

            If Contatto_Tipo <> -1 Then
                stb.AppendLine(" AND Contatto_Tipo = " & Agro_SQL_SaveNum(Contatto_Tipo))
            End If

            '' TODO
            If Cod_Contatto <> "-99" Then
                If Not String.IsNullOrEmpty(Cod_Contatto) Then
                    stb.AppendLine(" AND (Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' Or Cod_Contatto = '') ")
                Else
                    stb.AppendLine(" AND Cod_Contatto = ''")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY  Descrizione ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class IndirizzoTipo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal piva As String,
                           ByVal cod_Contatto As String,
                           ByVal descrizione As String,
                           ByVal contatto_Tipo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Dim sequenza = New Agro_Sequenze
        Dim Id As Integer = sequenza.NuovoId_Tabella("IndirizzoTipo", 2000, 2000000000, objParametri)

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO IndirizzoTipo( ")
            strSql.AppendLine("     IndirizzoTipo_Cod,   cod_contatto,      piva, ")
            strSql.AppendLine("     Descrizione,         Contatto_tipo, ")
            strSql.AppendLine("     Inviato,            DataInvio, ")
            strSql.AppendLine("     Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("     UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("     Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                     ) ")


            strSql.AppendLine("VALUES ( ")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(Id) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(cod_Contatto) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(descrizione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(contatto_Tipo) & "  ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(" )")

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

    Public Function Modifica(ByVal piva As String,
                            ByVal indirizzoTipo_Cod As Integer,
                            ByVal cod_Contatto As String,
                            ByVal descrizione As String,
                            ByVal contatto_Tipo As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            If String.IsNullOrEmpty(piva) Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If indirizzoTipo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (indirizzoTipo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("UPDATE IndirizzoTipo SET ")
            strSql.AppendLine("    Descrizione           = '" & Agro_SQL_SaveText(descrizione) & "'")
            strSql.AppendLine("   ,Contatto_Tipo           = " & Agro_SQL_SaveNum(contatto_Tipo) & "")
            strSql.AppendLine("   ,Cod_Contatto           = '" & Agro_SQL_SaveText(cod_Contatto) & "'")
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")


            strSql.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND IndirizzoTipo_Cod = '" & Agro_SQL_SaveText(indirizzoTipo_Cod) & "' ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function Cancella(ByVal piva As String,
                             ByVal indirizzoTipo_Cod As Long,
                             ByVal cod_Contatto As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndirizzoTipo_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If String.IsNullOrEmpty(piva) Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If indirizzoTipo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (indirizzoTipo_Cod obbligatorio)")
            End If

            strSql.Length = 0
            strSql.AppendLine(" DELETE ")
            strSql.AppendLine(" FROM  IndirizzoTipo ")
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" And indirizzoTipo_Cod = " & Agro_SQL_SaveNum(indirizzoTipo_Cod) & " ")
            strSql.AppendLine(" And Cod_Contatto = '" & Agro_SQL_SaveText(cod_Contatto) & "' ")
            strSql.AppendLine(" AND Inviato = 0")


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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





