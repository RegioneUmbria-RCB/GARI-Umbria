Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Sequenza_Progressivi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '############################################################################
    Public Function Nuovo_Progressivo(ByVal Piva As String,
                                      ByVal Anno As Integer,
                                      ByVal Tipo_Progressivo As Integer,
                                      ByVal Doc_Numero_Sin As String,
                                      ByVal Doc_Numero_Des As String,
                                      ByVal Sezionale_Cod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Nuovo_Progressivo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim xRisp As Boolean = False
        Dim nuovoValore As Integer

        Try

            dt = Leggi(Piva,
                       Anno,
                       Tipo_Progressivo,
                       Doc_Numero_Sin,
                       Doc_Numero_Des,
                       Sezionale_Cod,
                       "",
                       "",
                       objParametri)

            If dt.Rows.Count <> 0 Then

                nuovoValore = CInt(dt.Rows(0).Item("Doc_Numero_UltimoValore")) + 1

                ''Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                'If nuovoValore > dt.Rows(0).Item("End") Then

                ''Imposto un valore dummy
                'nuovoValore = -1

                ''Genero un errore
                'Throw New Exception("L'indice ha raggiunto il limite superiore")

                'End If

            Else 'Non esiste il record nella tabella

                'Imposto un valore dummy
                nuovoValore = -1

                messaggioErrore = "Impossibile recuperare il nuovo progressivo: Sequenza_Progressivi non configurata."

                'Genero un errore
                Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

                '    NuovoValore = Base + 1

                '    strSql.Length = 0
                '    strSql.AppendLine(" INSERT INTO Sequenza_Tabelle ( ")
                '    strSql.AppendLine(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
                '    strSql.AppendLine(" )")
                '    strSql.AppendLine(" VALUES( ")
                '    strSql.AppendLine(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
                '    strSql.AppendLine(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                '    strSql.AppendLine(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
                '    strSql.AppendLine(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
                '    strSql.AppendLine(" ," & Agro_SQL_SaveNum(Base) & " ")
                '    strSql.AppendLine(" ," & Agro_SQL_SaveNum(Fine) & " ")
                '    strSql.AppendLine(" )")

                '    xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, strSql.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

                '    If Not xRisp Then

                '        'Imposto un valore dummy
                '        nuovoValore = -1

                '        'Genero un errore
                '        Throw New Exception("Query di inserimento non riuscita.")

                '    End If

            End If

            dt = Nothing

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return nuovoValore

    End Function


    '############################################################################
    Public Function Nuovo_Progressivo_UpdateImmediato(ByVal Piva As String,
                                                      ByVal Anno As Integer,
                                                      ByVal Tipo_Progressivo As Integer,
                                                      ByVal Doc_Numero_Sin As String,
                                                      ByVal Doc_Numero_Des As String,
                                                      ByVal Sezionale_Cod As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Nuovo_Progressivo_UpdateImmediato()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim xRisp As Boolean = False
        Dim nuovoValore As Integer

        Try

            dt = Leggi(Piva,
                       Anno,
                       Tipo_Progressivo,
                       Doc_Numero_Sin,
                       Doc_Numero_Des,
                       Sezionale_Cod,
                       "",
                       "",
                       objParametri)

            If dt.Rows.Count <> 0 Then

                nuovoValore = CInt(dt.Rows(0).Item("Doc_Numero_UltimoValore")) + 1

                ''Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                'If nuovoValore > dt.Rows(0).Item("End") Then

                ''Imposto un valore dummy
                'nuovoValore = -1

                ''Genero un errore
                'Throw New Exception("L'indice ha raggiunto il limite superiore")

                'Else

                Dim objSeqProgr_W As New AgronicaCoreDataProvider.Sequenza_Progressivi_W

                xRisp = objSeqProgr_W.Modifica_Base(Piva,
                                                    Anno,
                                                    Tipo_Progressivo,
                                                    Doc_Numero_Sin,
                                                    Doc_Numero_Des,
                                                    Sezionale_Cod,
                                                    nuovoValore,
                                                    "",
                                                    objParametri)

                If xRisp = False Then

                    'Imposto un valore dummy
                    nuovoValore = -1

                    messaggioErrore = "Errore durante la chiamata a Modifica_Base."

                    'Genero un errore
                    Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

                End If

                'End If

            Else 'Non esiste il record nella tabella

                '  Giulia, 13/02/2017 15:58:21: se non è presente la voce (perché è il primo documento di quel tipo in quell'anno,
                '               provo ad inserire la voce, utilizzando dei valori di default per alcuni campi

                Dim objSeqProgr_W As New AgronicaCoreDataProvider.Sequenza_Progressivi_W
                xRisp = objSeqProgr_W.Scrivi(Piva, Anno, Tipo_Progressivo,
                                             Doc_Numero_Sin, Doc_Numero_Des,
                                             Sezionale_Cod,
                                             1, 1, 0, 7, 0, "0",
                                             objParametri)

                If xRisp = False Then

                    'Imposto un valore dummy
                    nuovoValore = -1

                    messaggioErrore = "Impossibile recuperare il nuovo progressivo: Sequenza_Progressivi non configurata."

                    'Genero un errore
                    Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

                End If

                nuovoValore = 1

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return nuovoValore

    End Function

    '############################################################################
    ''' <summary>
    ''' Legge il progressivo e se non trova la riga, anziché generare errore, restituisce il default iniziale passato + 1
    ''' (utile in caso di primo progressivo di quell'anno)
    ''' </summary>
    Public Function Nuovo_ProgressivoValBase(ByVal Piva As String,
                                             ByVal Anno As Integer,
                                             ByVal Tipo_Progressivo As Integer,
                                             ByVal Doc_Numero_Sin As String,
                                             ByVal Doc_Numero_Des As String,
                                             ByVal Sezionale_Cod As Integer,
                                             ByVal Valore_Iniziale_Default As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Nuovo_Progressivo()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim nuovoValore As Integer

        Try

            dt = Leggi(Piva,
                       Anno,
                       Tipo_Progressivo,
                       Doc_Numero_Sin,
                       Doc_Numero_Des,
                       Sezionale_Cod,
                       "",
                       "",
                       objParametri)

            If dt.Rows.Count <> 0 Then

                nuovoValore = CInt(dt.Rows(0).Item("Doc_Numero_UltimoValore")) + 1

            Else 'Non esiste il record nella tabella

                nuovoValore = Valore_Iniziale_Default + 1

            End If

            dt = Nothing

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return nuovoValore

    End Function

    '#############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Anno As Integer,
                          ByVal Tipo_Progressivo As Integer,
                          ByVal Doc_Numero_Sin As String,
                          ByVal Doc_Numero_Des As String,
                          ByVal Sezionale_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT   Sequenza_Progressivi.*  ")
            strSql.AppendLine(" FROM     Sequenza_Progressivi ")

            strSql.AppendLine(" WHERE   Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'")
            'aggiunto con il migra 296
            strSql.AppendLine(" AND     Sequenza_Progressivi.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Sequenza_Progressivi.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Sequenza_Progressivi.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Sequenza_Progressivi.Doc_Numero_Iniziale ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#############################################################################################
    Public Function Leggi2(ByVal Piva As String,
                          ByVal Anno As Integer,
                          ByVal Tipo_Progressivo As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT   Sequenza_Progressivi.*  ")
            strSql.AppendLine(" FROM     Sequenza_Progressivi ")

            strSql.AppendLine(" WHERE   Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            strSql.AppendLine(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Sequenza_Progressivi.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   (Sequenza_Progressivi.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Sequenza_Progressivi.Doc_Numero_Iniziale ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '############################################################################
    Public Sub Formattazione_from_Tipo(ByVal Piva As String,
                                       ByVal Anno As Integer,
                                       ByVal Tipo_Progressivo As Integer,
                                       ByVal Doc_Numero_Sin As String,
                                       ByVal Doc_Numero_Des As String,
                                       ByVal Sezionale_Cod As Integer,
                                       ByRef Lunghezza_Sin As Integer,
                                       ByRef Lunghezza_Centro As Integer,
                                       ByRef Lunghezza_Des As Integer,
                                       ByRef CarattereFormattazione As String,
                                       ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_R.Formattazione_from_Tipo()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Lunghezza_Sin = 0
            Lunghezza_Centro = 0
            Lunghezza_Des = 0
            CarattereFormattazione = ""

            dt = Leggi(Piva,
                       Anno,
                       Tipo_Progressivo,
                       Doc_Numero_Sin,
                       Doc_Numero_Des,
                       Sezionale_Cod,
                       "",
                       "",
                       objParametri)

            If dt.Rows.Count <> 0 Then
                Lunghezza_Sin = CInt(dt.Rows(0).Item("Lunghezza_Sin"))
                Lunghezza_Centro = CInt(dt.Rows(0).Item("Lunghezza_Centro"))
                Lunghezza_Des = CInt(dt.Rows(0).Item("Lunghezza_Des"))
                CarattereFormattazione = CStr(dt.Rows(0).Item("CarattereFormattazione"))
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Sequenza_Progressivi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Anno As Integer,
                           ByVal Tipo_Progressivo As Integer,
                           ByVal Doc_Numero_Sin As String,
                           ByVal Doc_Numero_Des As String,
                           ByVal Sezionale_Cod As Integer,
                           ByVal Doc_Numero_Iniziale As Integer,
                           ByVal Doc_Numero_UltimoValore As Integer,
                           ByVal Lunghezza_Sin As Integer,
                           ByVal Lunghezza_Centro As Integer,
                           ByVal Lunghezza_Des As Integer,
                           ByVal CarattereFormattazione As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Sequenza_Progressivi ( ")
            strSql.AppendLine("             Piva_SuperUser, Piva, Anno, Tipo_Progressivo, Doc_Numero_Sin, Doc_Numero_Des, Sezionale_Cod, ")
            strSql.AppendLine("             Doc_Numero_Iniziale, Doc_Numero_UltimoValore, ")
            strSql.AppendLine("             Lunghezza_Sin, Lunghezza_Centro, Lunghezza_Des, CarattereFormattazione, ")
            strSql.AppendLine("             inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica ")
            strSql.AppendLine("             ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Anno) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Progressivo) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Doc_Numero_Sin) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Doc_Numero_Des) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sezionale_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Doc_Numero_Iniziale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Doc_Numero_UltimoValore) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Lunghezza_Sin) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Lunghezza_Centro) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Lunghezza_Des) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CarattereFormattazione) & "' ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine(")")

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


    '####################################################################
    Public Function Modifica_Base(ByVal Piva As String,
                                  ByVal Anno As Integer,
                                  ByVal Tipo_Progressivo As Integer,
                                  ByVal Doc_Numero_Sin As String,
                                  ByVal Doc_Numero_Des As String,
                                  ByVal Sezionale_Cod As Integer,
                                  ByVal Doc_Numero_UltimoValore As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Sequenza_Progressivi SET ")

            strSql.AppendLine("     Doc_Numero_UltimoValore     = " & Agro_SQL_SaveNum(Doc_Numero_UltimoValore) & " ")
            strSql.AppendLine("     ,Inviato                     = 0 ")
            strSql.AppendLine("     ,DataInvio                   = Null ")
            strSql.AppendLine("     ,Data_Modifica               = GETDATE() ")
            strSql.AppendLine("     ,UserName_Modifica           = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND     Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            strSql.AppendLine(" AND     Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")
            strSql.AppendLine(" AND     Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'")
            strSql.AppendLine(" AND     Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'")
            strSql.AppendLine(" AND     Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    '###############################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Anno As Integer,
                             ByVal Tipo_Progressivo As Integer,
                             ByVal Doc_Numero_Sin As String,
                             ByVal Doc_Numero_Des As String,
                             ByVal Sezionale_Cod As Integer,
                             ByVal Doc_Numero_Iniziale As Integer,
                             ByVal Doc_Numero_UltimoValore As Integer,
                             ByVal Lunghezza_Sin As Integer,
                             ByVal Lunghezza_Centro As Integer,
                             ByVal Lunghezza_Des As Integer,
                             ByVal CarattereFormattazione As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreDataProvider.Sequenza_Progressivi_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Sequenza_Progressivi SET ")

            strSql.AppendLine("     Doc_Numero_Iniziale          = " & Agro_SQL_SaveNum(Doc_Numero_Iniziale) & " ")
            strSql.AppendLine("     ,Doc_Numero_UltimoValore     = " & Agro_SQL_SaveNum(Doc_Numero_UltimoValore) & " ")
            strSql.AppendLine("     ,Lunghezza_Sin               = " & Agro_SQL_SaveNum(Lunghezza_Sin) & " ")
            strSql.AppendLine("     ,Lunghezza_Centro            = " & Agro_SQL_SaveNum(Lunghezza_Centro) & " ")
            strSql.AppendLine("     ,Lunghezza_Des               = " & Agro_SQL_SaveNum(Lunghezza_Des) & " ")
            strSql.AppendLine("     ,CarattereFormattazione      = '" & Agro_SQL_SaveText(CarattereFormattazione) & "' ")

            strSql.AppendLine("     ,Inviato                     = 0 ")
            strSql.AppendLine("     ,DataInvio                   = Null ")
            strSql.AppendLine("     ,Data_Modifica               = GETDATE() ")
            strSql.AppendLine("     ,UserName_Modifica           = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND     Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            strSql.AppendLine(" AND     Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")
            strSql.AppendLine(" AND     Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'")
            strSql.AppendLine(" AND     Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'")
            strSql.AppendLine(" AND     Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    '#################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Anno As Integer,
                             ByVal Tipo_Progressivo As Integer,
                             ByVal Doc_Numero_Sin As String,
                             ByVal Doc_Numero_Des As String,
                             ByVal Sezionale_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Sequenza_Progressivi_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE  Sequenza_Progressivi ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("         ,Inviato = -1 ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                strSql.AppendLine(" AND     Anno = " & Agro_SQL_SaveNum(Anno) & " ")
                strSql.AppendLine(" AND     Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")
                strSql.AppendLine(" AND     Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'")
                strSql.AppendLine(" AND     Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'")
                strSql.AppendLine(" AND     Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")
                strSql.AppendLine(" AND     Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    Sequenza_Progressivi ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                strSql.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                strSql.AppendLine(" AND     Anno = " & Agro_SQL_SaveNum(Anno) & " ")
                strSql.AppendLine(" AND     Tipo_Progressivo = " & Agro_SQL_SaveNum(Tipo_Progressivo) & " ")
                strSql.AppendLine(" AND     Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'")
                strSql.AppendLine(" AND     Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'")
                strSql.AppendLine(" AND     Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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
