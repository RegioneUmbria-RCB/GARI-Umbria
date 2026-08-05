
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Pratiche_Stati_Attuali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function LeggiListaPerImpresa(
        ByVal piva As String,
        ByVal WWorkflow_Cod As Integer,
        ByVal Servizio_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


            stb.Length = 0

            stb.AppendLine(" Select ")
            stb.AppendLine("    cast(p.Pratica_Cod As varchar(50)) + '_' +  ")
            stb.AppendLine("    cast(origine.WAnagraficaStati_Cod As varchar(50)) As Codice ")
            stb.AppendLine("     ")
            stb.AppendLine("  , case when Not tes.Programmazione_Des Is null then tes.programmazione_des + ' - ' else '' end + 'Servizio: ' + s.Servizio_Des + ' - ' +  ")
            stb.AppendLine("    p.Numero + ' - ' +  ")
            stb.AppendLine("    'Stato Pratica: ' + origine.wanagraficaStati_Des + ' (Avanzato da: ' + dU.Nome + ' ' + dU.Cognome  +  ")
            stb.AppendLine("    ', il ' + CONVERT(varchar(10),  statAtt.Validita_Inizio, 103) +  ")
            stb.AppendLine("    Case when statAtt.note <> '' then ', note: ' + statAtt.note else '' end + ')' ")
            stb.AppendLine("  as Descrizione")

            stb.AppendLine("   ")
            stb.AppendLine("     , origine.Colore ")

            stb.AppendLine(" From Pratiche p ")
            stb.AppendLine("  inner Join Imprese i  ")
            stb.AppendLine("         On p.Piva = i.PIVA ")
            stb.AppendLine("  inner Join Pratiche_Stati_Attuali statAtt ")
            stb.AppendLine("         On p.Pratica_Cod = statAtt.Pratica_Cod ")
            stb.AppendLine("  inner Join WAnagraficaStati origine ")
            stb.AppendLine("         On origine.WAnagraficaStati_Cod = statAtt.Stato_Cod ")
            stb.AppendLine("  inner Join Servizi s ")
            stb.AppendLine("         On s.Servizio_Cod = p.Servizio_Cod ")
            stb.AppendLine("  Left Join Programmazione_Testata tes ")
            stb.AppendLine("         On tes.Pratica_Cod Like '%' + cast(p.Pratica_Cod as varchar(50)) + '%'")
            stb.AppendLine("  Left Join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli dU ")
            stb.AppendLine("         On dU.CodFisc = statAtt.Username_Creazione ")
            stb.AppendLine(" inner Join WWorkFlow w ")
            stb.AppendLine("         On w.WorkFlow_Cod = origine.WWorkFlow_cod ")
            stb.AppendLine("       ")
            stb.AppendLine(" where W.WorkFlow_Cod = " & WWorkflow_Cod)
            stb.AppendLine(" And p.Piva ='" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine(" And p.Servizio_Cod = " & Servizio_Cod)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   p.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                stb.Append(" order by p.Pratica_Cod desc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    '##############################################################################################
    Public Function Leggi( _
                         ByVal Pratica_cod As Integer, _
                         ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT * " + vbCrLf)
            strSQL.Append(" from Pratiche_Stati_Attuali  " + vbCrLf)
            strSQL.Append(" Where Pratica_cod = " & Agro_SQL_SaveNum(Pratica_cod) + vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function LeggiConDescrizioneStato(
                         ByVal Pratica_cod As Integer,
                         ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiConDescrizioneStato()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT psa.*, wa.WAnagraficaStati_Des " + vbCrLf)
            strSQL.Append(" from Pratiche_Stati_Attuali psa " + vbCrLf)
            strSQL.Append(" JOIN WAnagraficaStati wa on wa.WAnagraficaStati_cod = psa.stato_cod " + vbCrLf)
            strSQL.Append(" Where Pratica_cod = " & Agro_SQL_SaveNum(Pratica_cod) + vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   psa.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   psa.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class Pratiche_Stati_Attuali_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByVal Pratica_Cod As Int32,
                            ByVal Stato_Cod As Int32,
                            ByVal note As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Data_creazione As Date,
                            ByVal Data_modifica As Date,
                            ByVal username_creazione As String,
                            ByVal username_modifica As String
                            ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT Pratiche_Stati_Attuali   " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("              Piva_SuperUser, Pratica_Cod, Stato_Cod, note, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pratica_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Stato_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(note) & "' ")

            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Validita_Fine) & "  ")



            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '#################################################################
    Public Function Modifica( _
                ByVal Pratica_Cod As Int32, _
                ByVal Stato_Cod As Int32, _
                ByVal Stato_Cod_PerRicerca As Int32, _
                ByVal note As String, _
                ByVal Validita_Inizio As Date, _
                ByVal Validita_Fine As Date, _
                ByVal xFiltroAggiuntivo As String, _
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.Append(" UPDATE Pratiche_Stati_Attuali  ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.Append("         ,validita_inizio= " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            StrSQL.Append("         ,stato_cod= " & Agro_SQL_SaveNum(Stato_Cod) & "")
            StrSQL.Append("         ,Note= '" & Agro_SQL_SaveText(note) & "'")

            StrSQL.Append(" WHERE   Pratica_cod = " & Pratica_Cod)
            StrSQL.Append(" And Stato_cod = " & Stato_Cod_PerRicerca)
            StrSQL.Append(" And piva_SuperUSer = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.Append(" UPDATE Pratiche  ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.Append(" WHERE   Pratica_cod = " & Pratica_Cod)
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function ModificaNote(
                ByVal Pratica_Cod As Int32,
                ByVal note As String,
                ByVal xFiltroAggiuntivo As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.Append(" UPDATE Pratiche_Stati_Attuali  ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         Note= '" & Agro_SQL_SaveText(note) & "'")
            StrSQL.Append(" WHERE   Pratica_cod = " & Pratica_Cod)
            StrSQL.Append(" And piva_SuperUSer = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    '#################################################################
    Public Function Cancella( _
                            ByVal Pratica_cod As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM Pratiche_Stati_Attuali ")
                StrSQL.Append(" WHERE 1=1 ")
                StrSQL.Append(" and pratica_cod = " & Pratica_cod & vbCrLf)


            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class


