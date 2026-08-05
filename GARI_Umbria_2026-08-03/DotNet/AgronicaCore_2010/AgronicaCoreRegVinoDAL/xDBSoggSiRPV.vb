Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class xDBSoggSiRPV_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    '##############################################################################################
    Public Function LeggiSoggetti(ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
        ByVal codOper As String, _
        Optional ByVal CodiceSoggetto As String = Nothing, _
        Optional ByVal GIAS_Stato As Nullable(Of Integer) = Nothing, _
        Optional ByVal tipoRichiesta As String = Nothing _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_soggetti rvs " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            Stb.Append(" AND rvs.codOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            If CodiceSoggetto IsNot Nothing Then
                Stb.Append(" AND rvs.CodiceSoggetto='" & Agro_SQL_SaveText(CodiceSoggetto) & "' " & vbCrLf)
            End If
            If GIAS_Stato IsNot Nothing Then
                Stb.Append(" AND rvs.GIAS_Stato=" & CStr(GIAS_Stato) & " " & vbCrLf)
            End If
            If tipoRichiesta IsNot Nothing Then
                Stb.Append(" AND rvs.TipoRichiesta='" & Agro_SQL_SaveText(tipoRichiesta) & "' " & vbCrLf)
            End If
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function getListaCodOper(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of String)

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaCodOper()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCodOper As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" Select distinct(CodOper) FROM [dbo].[ws_RegVino_Soggetti] " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            For Each row As DataRow In DT.Rows
                listaCodOper.Add(CStr(row.Item("CodOper")))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCodOper
    End Function

    Function personaFisica(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As Boolean
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.PersonaFisica()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim persFisica As Boolean
        Try

            Stb.Length = 0

            Stb.Append(" Select Top 1 CodOper_Fisiche FROM [dbo].[ws_RegVino_Soggetti] WHERE CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                If (CStr(DT.Rows(0).Item(0)) = "F") Then
                    persFisica = True
                Else
                    persFisica = False
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return persFisica
    End Function

    Function listaSoggettiDaAggiornare(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, LogInvioID As Integer) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaSoggettiDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaSogg As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" Select ws_RegVino_Soggetto_Cod FROM ws_RegVino_LogInvio_DettaglioSoggetti WHERE  ws_RegVino_LogInvio_Cod=" & CStr(LogInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            For Each row As DataRow In DT.Rows
                listaSogg.Add(CStr(row.Item("ws_RegVino_Soggetto_Cod")))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaSogg
    End Function

    Function getCodOperFromLogInvioID(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, LogInvioID As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromLogInvioID()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codOper As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" Select CodOper FROM ws_RegVino_LogInvio_DettaglioSoggetti WHERE  ws_RegVino_LogInvio_Cod=" & CStr(LogInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            codOper = DT.Rows(0).Item("CodOper")

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codOper
    End Function

    Function getDBSoggettiPerCodOper(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCore_DAL.getDBSoggettiPerCodOper()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append("Select CodiceSoggetto " & vbCrLf)
            Stb.Append(" from ws_RegVino_Soggetti  " & vbCrLf)
            Stb.Append(" WHERE CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta in ('A','I') " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function getOperazioniDaControllare(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                               ByRef operazione As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCore_DAL.getOperazioniDaControllare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append(" Select distinct l.ws_RegVino_LogInvio_Cod, l.idTrasmissione_SIAN, ls.CodOper " & vbCrLf)
            Stb.Append(" FROM  ws_RegVino_LogInvio l " & vbCrLf)
            Stb.Append(" INNER JOIN ws_RegVino_LogInvio_DettaglioSoggetti ls ON l.ws_RegVino_LogInvio_Cod = ls.ws_RegVino_LogInvio_Cod " & vbCrLf)
            Stb.Append(" WHERE l.Operazione=" & Agro_SQL_SaveText_NULL(operazione) & " AND Controllata='False'" & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Function leggiSoggettiDaOperazioni(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                       ByRef CodOper As String) As List(Of String)
        Dim NomeRoutine As String = "AgronicaCore_DAL.getOperazioniDaControllare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim listaSoggetti As New List(Of String)
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append("SELECT Distinct CodCommittente as CodiceSoggetto FROM [dbo].[ws_RegVino_Operazioni] WHERE TipoRichiesta IN ('I','A') AND CodOper = " & Agro_SQL_SaveText_NULL(CodOper) & " AND CodCommittente <> '' " & vbCrLf)
            Stb.Append(" UNION  " & vbCrLf)
            Stb.Append(" SELECT Distinct CodFornitore as CodiceSoggetto FROM [dbo].[ws_RegVino_Operazioni] WHERE TipoRichiesta IN ('I','A') AND CodOper = " & Agro_SQL_SaveText_NULL(CodOper) & " AND CodFornitore <> '' " & vbCrLf)
            Stb.Append(" UNION   " & vbCrLf)
            Stb.Append(" SELECT Distinct CodDestinatario as CodiceSoggetto FROM [dbo].[ws_RegVino_Operazioni] WHERE TipoRichiesta IN ('I','A') AND CodOper = " & Agro_SQL_SaveText_NULL(CodOper) & " AND CodDestinatario <> ''")



                        '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                        '--------------------------------------------------------------------------
            For Each row In DT.Rows
                listaSoggetti.Add(row.item("CodiceSoggetto"))
            Next


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaSoggetti
    End Function

    Public Function Leggiws_RegVino_Soggetti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                         Optional ByVal CodSoggetto As String = "", _
                                         Optional ByVal CodOper As String = "", _
                                         Optional ByVal codOperFisiche_Giuridiche As String = "", _
                                         Optional ByVal TipoRichiesta As String = "", _
                                         Optional ByVal CUAA As String = "", _
                                         Optional ByVal CUAA_Fisiche As String = "", _
                                         Optional ByVal TipoSoggetto As String = "", _
                                         Optional ByVal Nome As String = "", _
                                         Optional ByVal Cognome As String = "", _
                                         Optional ByVal Ragione_Sociale As String = "", _
                                         Optional ByVal cap As String = "", _
                                         Optional ByVal indirizzo As String = "", _
                                         Optional ByVal comune As String = "", _
                                         Optional ByVal provincia As String = "", _
                                         Optional ByVal stato As String = "", _
                                         Optional ByVal GIAS_Stato As Integer = 0) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.Leggiws_RegVino_Soggetti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("SELECT " & vbCrLf)
            Stb.Append(" CodiceSoggetto, " & vbCrLf)
            Stb.Append(" CodOper, " & vbCrLf)
            Stb.Append(" CodOper_Fisiche, " & vbCrLf)
            Stb.Append(" TipoRichiesta, " & vbCrLf)
            Stb.Append(" CUAA, " & vbCrLf)
            Stb.Append(" CUAA_Fisiche, " & vbCrLf)
            Stb.Append(" TipoSoggetto, " & vbCrLf)
            Stb.Append(" Nome, " & vbCrLf)
            Stb.Append(" Cognome, " & vbCrLf)
            Stb.Append(" Ragione_Sociale, " & vbCrLf)
            Stb.Append(" Indirizzo_CAP, " & vbCrLf)
            Stb.Append(" Indirizzo_Indirizzo, " & vbCrLf)
            Stb.Append(" Indirizzo_Comune, " & vbCrLf)
            Stb.Append(" Indirizzo_Provincia, " & vbCrLf)
            Stb.Append(" Indirizzo_Stato, " & vbCrLf)
            Stb.Append(" GIAS_Stato " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Soggetti " & vbCrLf)
            Stb.Append(" WHERE 1=1")

            If CodSoggetto <> "" Then
                Stb.Append(" AND CodiceSoggetto=" & Agro_SQL_SaveText_NULL(CodSoggetto) & " " & vbCrLf)
            End If

            If CodOper <> "" Then
                Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(CodOper) & " " & vbCrLf)
            End If

            If codOperFisiche_Giuridiche <> "" Then
                Stb.Append(" AND CodOper_Fisiche=" & Agro_SQL_SaveText_NULL(codOperFisiche_Giuridiche) & " " & vbCrLf)
            End If

            If TipoRichiesta <> "" Then
                Stb.Append(" AND TipoRichiesta=" & Agro_SQL_SaveText_NULL(TipoRichiesta) & " " & vbCrLf)
            End If

            If CUAA <> "" Then
                Stb.Append(" AND CUAA=" & Agro_SQL_SaveText_NULL(CUAA) & " " & vbCrLf)
            End If

            If CUAA_Fisiche <> "" Then
                Stb.Append(" AND CUAA_Fisiche=" & Agro_SQL_SaveText_NULL(CUAA_Fisiche) & " " & vbCrLf)
            End If

            If TipoSoggetto <> "" Then
                Stb.Append(" AND TipoSoggetto=" & Agro_SQL_SaveText_NULL(TipoSoggetto) & " " & vbCrLf)
            End If

            If Nome <> "" Then
                Stb.Append(" AND Nome=" & Agro_SQL_SaveText_NULL(Nome) & " " & vbCrLf)
            End If

            If Cognome <> "" Then
                Stb.Append(" AND Cognome=" & Agro_SQL_SaveText_NULL(Cognome) & " " & vbCrLf)
            End If

            If Ragione_Sociale <> "" Then
                Stb.Append(" AND Ragione_Sociale=" & Agro_SQL_SaveText_NULL(Ragione_Sociale) & " " & vbCrLf)
            End If

            If cap <> "" Then
                Stb.Append(" AND Indirizzo_CAP=" & Agro_SQL_SaveText_NULL(cap) & " " & vbCrLf)
            End If

            If indirizzo <> "" Then
                Stb.Append(" AND Indirizzo_Indirizzo=" & Agro_SQL_SaveText_NULL(indirizzo) & " " & vbCrLf)
            End If

            If comune <> "" Then
                Stb.Append(" AND Indirizzo_Comune=" & Agro_SQL_SaveText_NULL(comune) & " " & vbCrLf)
            End If

            If provincia <> "" Then
                Stb.Append(" AND Indirizzo_Provincia=" & Agro_SQL_SaveText_NULL(provincia) & " " & vbCrLf)
            End If

            If stato <> "" Then
                Stb.Append(" AND Indirizzo_Stato=" & Agro_SQL_SaveText_NULL(stato) & " " & vbCrLf)
            End If

            If GIAS_Stato <> 0 Then
                Stb.Append(" AND Gias_Stato=" & Agro_SQL_SaveNum_NULL(GIAS_Stato) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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

Public Class xDBSoggSiRPV_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Public Function inserisciLogInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByVal ws_RegVino_Soggetto_Codici As List(Of String), _
                                      ByVal codOper As String, _
                                      ByVal logInvio_Des As String, _
                                      ByVal esitoSIAN As String, _
                                      ByVal Operazione As String, _
                                      ByVal Controllata As Boolean, _
                                      ByVal idTrasmissione As String, _
                                      Optional ByVal dataInvio As Nullable(Of DateTime) = Nothing) As Integer
        'INSERISCO LOGINVIO
        Dim id = utils.NuovoID_ws_RegVino_LogInvio(objParametri)
        Dim NomeRoutine As String = "inserisciLogInvio()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StbLogInvio As New System.Text.StringBuilder
        Dim xRispLovInvio As Boolean = False
        Try

            '---------------------------------------------
            StbLogInvio.Length = 0
            StbLogInvio.Append(" INSERT into ws_RegVino_LogInvio " & vbCrLf)
            StbLogInvio.Append(" (ws_RegVino_LogInvio_Cod, ws_RegVino_LogInvio_Des, DataInvio, Esito_SIAN, Operazione, Controllata, IDTrasmissione_Sian) " & vbCrLf)
            StbLogInvio.Append(" VALUES ( " & vbCrLf)
            StbLogInvio.Append("" & Agro_SQL_SaveNum(id) & " , " & vbCrLf)
            StbLogInvio.Append("" & Agro_SQL_SaveStringToXML(logInvio_Des) & " , " & vbCrLf)
            If dataInvio Is Nothing Then
                StbLogInvio.Append("" & Agro_SQL_SaveDateTime(DateTime.Now) & " , " & vbCrLf)
            Else
                StbLogInvio.Append("" & Agro_SQL_SaveDateTime(dataInvio) & " , " & vbCrLf)
            End If
            StbLogInvio.Append("" & Agro_SQL_SaveText_NULL(esitoSIAN) & " , " & vbCrLf)
            StbLogInvio.Append("" & Agro_SQL_SaveText_NULL(Operazione) & " , " & vbCrLf)
            StbLogInvio.Append("" & Agro_SQL_SaveBoolStrToInt(CStr(Controllata)) & " , " & vbCrLf)
            StbLogInvio.Append("" & Agro_SQL_SaveText_NULL(idTrasmissione) & " " & vbCrLf)
            StbLogInvio.Append(" ) " & vbCrLf)
            '--------------------------------------------------------------------------
            xRispLovInvio = EseguiQuery_Scrittura(objParametri, StbLogInvio.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRispLovInvio = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


        'INSERISCO LOGINVIO_DETTAGLIOSOGGETTI
        Dim StbSoggetti As New System.Text.StringBuilder
        Dim xRispSoggetti As Boolean = False
        Try
            For Each SoggettoCodice As String In ws_RegVino_Soggetto_Codici

                '---------------------------------------------
                StbSoggetti.Length = 0
                StbSoggetti.Append(" INSERT into ws_RegVino_LogInvio_DettaglioSoggetti " & vbCrLf)
                StbSoggetti.Append(" (ws_RegVino_LogInvio_Cod, ws_RegVino_Soggetto_Cod, CodOper) " & vbCrLf)
                StbSoggetti.Append(" VALUES ( " & vbCrLf)
                StbSoggetti.Append("" & CStr(id) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(SoggettoCodice) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                StbSoggetti.Append(" ) " & vbCrLf)

                '--------------------------------------------------------------------------
                xRispSoggetti = EseguiQuery_Scrittura(objParametri, StbSoggetti.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRispSoggetti = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return id

    End Function

    Public Function aggiornaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiceSoggetto As String, _
                                  ByVal codOper As String, _
                                  ByVal statoGIAS As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & codiceSoggetto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_soggetti " & vbCrLf)
            Stb.Append(" SET GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            Stb.Append(" AND CodiceSoggetto='" & Agro_SQL_SaveText(codiceSoggetto) & "' " & vbCrLf)
            Stb.Append(" AND codOper='" & Agro_SQL_SaveText(codOper) & "' " & vbCrLf)
            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'non ci segnamo lo storico dei passaggi di stato
            '
            '

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function aggiornaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiciSoggetti As List(Of String), _
                                  ByVal codOper As String, _
                                  ByVal statoGIAS As Integer) As Boolean

        For Each codiceSoggetto As String In codiciSoggetti
            aggiornaStato(objParametri, codiceSoggetto, codOper, statoGIAS)
        Next

        Return True
    End Function

    Public Function inserisciSoggetti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiceSoggetto As String, _
                                  ByVal codOper As String, _
                                  ByVal codOperFG As String, _
                                  ByVal tipoRichiesta As String, _
                                  ByVal Cuaa As String, _
                                  ByVal Cuaa_Fisiche As Integer, _
                                  ByVal TipoSoggetto As String, _
                                  ByVal Nome As String, _
                                  ByVal Cognome As String, _
                                  ByVal Ragione_Sociale As String, _
                                  ByVal Indirizzo_CAP As String, _
                                  ByVal Indirizzo_Indirizzo As String, _
                                  ByVal Indirizzo_Comune As String, _
                                  ByVal Indirizzo_Provincia As String, _
                                  ByVal Indirizzo_Stato As String, _
                                  ByVal statoGIAS As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.inserisciSoggetti(" & codiceSoggetto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append("INSERT INTO [dbo].[ws_RegVino_Soggetti]([CodiceSoggetto],[CodOper],[CodOper_Fisiche],[TipoRichiesta],[CUAA],[CUAA_Fisiche],[TipoSoggetto],[Nome],[Cognome],[Ragione_Sociale],[Indirizzo_CAP],[Indirizzo_Indirizzo],[Indirizzo_Comune],[Indirizzo_Provincia],[Indirizzo_Stato],[GIAS_Stato]) " & vbCrLf)
            Stb.Append("      VALUES(" & Agro_SQL_SaveText_NULL(codiceSoggetto) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(codOperFG) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Cuaa) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveNum_NULL(Cuaa_Fisiche) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(TipoSoggetto) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Nome) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Cognome) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Ragione_Sociale) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Indirizzo_CAP) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Indirizzo_Indirizzo) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Indirizzo_Comune) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Indirizzo_Provincia) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveText_NULL(Indirizzo_Stato) & " " & vbCrLf)
            Stb.Append("            ," & Agro_SQL_SaveNum_NULL(statoGIAS) & " )")


            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'non ci segnamo lo storico dei passaggi di stato
            '
            '

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function eliminaSoggetto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiceSoggetto As String, _
                                  ByVal codOper As String)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.inserisciSoggetti(" & codiceSoggetto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim stb1 As New System.Text.StringBuilder

        Try

            Stb.Length = 0
            Stb.Append("DELETE FROM ws_RegVino_LogInvio_DettaglioSoggetti " & vbCrLf  ) 
            Stb.Append(" where [CodOper]= " & Agro_SQL_SaveText_NULL(codOper) & " AND [ws_RegVino_Soggetto_Cod]= " & Agro_SQL_SaveText_NULL(codiceSoggetto) & "")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            stb1.Length = 0
            stb1.Append("DELETE FROM ws_RegVino_Soggetti  " & vbCrLf)
            stb1.Append(" WHERE CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " and CodiceSoggetto = " & Agro_SQL_SaveText_NULL(codiceSoggetto) & "")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, stb1.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'non ci segnamo lo storico dei passaggi di stato
            '
            '

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Sub aggiornaSoggetto(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String, codiceSoggetto As String, tipoRichiesta As String, statoGIAS As Integer)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.inserisciSoggetti(" & codiceSoggetto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder

        Try

            Stb.Length = 0
            Stb.Append("UPDATE ws_RegVino_Soggetti " & vbCrLf)
            Stb.Append(" SET TipoRichiesta = " & Agro_SQL_SaveText_NULL(tipoRichiesta) & " , " & vbCrLf)
            Stb.Append("     GIAS_Stato = " & Agro_SQL_SaveNum_NULL(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE " & vbCrLf)
            Stb.Append("    CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append("    AND CodiceSoggetto = " & Agro_SQL_SaveText_NULL(codiceSoggetto) & " ")


            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Sub

    Public Function InserisciAggiornaWs_RegVino_Soggetti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                     codiceSoggetto As String,
                                                     codOper As String,
                                                     codOperFisiche As String,
                                                     tiporichiesta As String,
                                                     cuaa As String,
                                                     cuaa_Fisiche As String,
                                                     tipoSoggetto As String,
                                                     nome As String,
                                                     cognome As String,
                                                     ragione_sociale As String,
                                                     cap As String,
                                                     indirizzo As String,
                                                     comune As String,
                                                     provincia As String,
                                                     stato As String,
                                                     Gias_Stato As Integer)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.InserisciAggiornaWs_RegVino_Soggetti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append("SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Soggetti " & vbCrLf)
            Stb.Append(" WHERE  " & vbCrLf)
            Stb.Append(" CodiceSoggetto=" & Agro_SQL_SaveText_NULL(codiceSoggetto) & "" & vbCrLf)
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0
            If DT.Rows.Count > 0 Then
                'UPDATE
                Stb1.Append("UPDATE [dbo].[ws_RegVino_Soggetti] " & vbCrLf)
                Stb1.Append("    SET [CodOper] =" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                Stb1.Append("       ,[CodOper_Fisiche] =" & Agro_SQL_SaveText_NULL(codOperFisiche) & " " & vbCrLf)
                Stb1.Append("       ,[TipoRichiesta] =" & Agro_SQL_SaveText_NULL(tiporichiesta) & " " & vbCrLf)
                Stb1.Append("       ,[CUAA] =" & Agro_SQL_SaveText_NULL(cuaa) & " " & vbCrLf)
                Stb1.Append("       ,[CUAA_Fisiche] =" & Agro_SQL_SaveText_NULL(cuaa_Fisiche) & " " & vbCrLf)
                Stb1.Append("       ,[TipoSoggetto] =" & Agro_SQL_SaveText_NULL(tipoSoggetto) & " " & vbCrLf)
                Stb1.Append("       ,[Nome] =" & Agro_SQL_SaveText_NULL(nome) & " " & vbCrLf)
                Stb1.Append("       ,[Cognome] =" & Agro_SQL_SaveText_NULL(cognome) & " " & vbCrLf)
                Stb1.Append("       ,[Ragione_Sociale] =" & Agro_SQL_SaveText_NULL(ragione_sociale) & " " & vbCrLf)
                Stb1.Append("       ,[Indirizzo_CAP] =" & Agro_SQL_SaveText_NULL(cap) & " " & vbCrLf)
                Stb1.Append("       ,[Indirizzo_Indirizzo] =" & Agro_SQL_SaveText_NULL(indirizzo) & " " & vbCrLf)
                Stb1.Append("       ,[Indirizzo_Comune] =" & Agro_SQL_SaveText_NULL(comune) & " " & vbCrLf)
                Stb1.Append("       ,[Indirizzo_Provincia] =" & Agro_SQL_SaveText_NULL(provincia) & " " & vbCrLf)
                Stb1.Append("       ,[Indirizzo_Stato] =" & Agro_SQL_SaveText_NULL(stato) & " " & vbCrLf)
                Stb1.Append("       ,[Gias_Stato] =" & Agro_SQL_SaveNum(Gias_Stato) & " " & vbCrLf)
                Stb1.Append(" WHERE CodiceSoggetto=" & Agro_SQL_SaveText_NULL(codiceSoggetto) & " " & vbCrLf)
                Stb1.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Else
                'INSERT
                Stb1.Append("INSERT INTO [dbo].[ws_RegVino_Soggetti] " & vbCrLf)
                Stb1.Append("            ([CodiceSoggetto] " & vbCrLf)
                Stb1.Append("            ,[CodOper] " & vbCrLf)
                Stb1.Append("            ,[CodOper_Fisiche] " & vbCrLf)
                Stb1.Append("            ,[TipoRichiesta] " & vbCrLf)
                Stb1.Append("            ,[CUAA] " & vbCrLf)
                Stb1.Append("            ,[CUAA_Fisiche] " & vbCrLf)
                Stb1.Append("            ,[TipoSoggetto] " & vbCrLf)
                Stb1.Append("            ,[Nome] " & vbCrLf)
                Stb1.Append("            ,[Cognome] " & vbCrLf)
                Stb1.Append("            ,[Ragione_Sociale] " & vbCrLf)
                Stb1.Append("            ,[Indirizzo_CAP] " & vbCrLf)
                Stb1.Append("            ,[Indirizzo_Indirizzo] " & vbCrLf)
                Stb1.Append("            ,[Indirizzo_Comune] " & vbCrLf)
                Stb1.Append("            ,[Indirizzo_Provincia] " & vbCrLf)
                Stb1.Append("            ,[Indirizzo_Stato] " & vbCrLf)
                Stb1.Append("            ,[GIAS_Stato]) " & vbCrLf)
                Stb1.Append("                 VALUES " & vbCrLf)
                Stb1.Append("            (" & Agro_SQL_SaveText_NULL(codiceSoggetto) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(codOperFisiche) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(tiporichiesta) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(cuaa) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(cuaa_Fisiche) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(tipoSoggetto) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(nome) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(cognome) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(ragione_sociale) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(cap) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(indirizzo) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(comune) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(provincia) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(stato) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveNum_NULL(Gias_Stato) & ") ")
            End If

            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function InserisciAggiornaRichiestaWs_RegVino_Soggetti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                     codiceSoggetto As String,
                                                     codOper As String,
                                                     tiporichiesta As String,
                                                     Gias_Stato As Integer)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.InserisciAggiornaWs_RegVino_Soggetti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append("SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Soggetti " & vbCrLf)
            Stb.Append(" WHERE  " & vbCrLf)
            Stb.Append(" CodiceSoggetto=" & Agro_SQL_SaveText_NULL(codiceSoggetto) & "" & vbCrLf)
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0
            If DT.Rows.Count > 0 Then
                'UPDATE
                Stb1.Append("UPDATE [dbo].[ws_RegVino_Soggetti] " & vbCrLf)
                Stb1.Append("    SET " & vbCrLf)
                Stb1.Append("       [TipoRichiesta] =" & Agro_SQL_SaveText_NULL(tiporichiesta) & " " & vbCrLf)
                Stb1.Append("       ,[Gias_Stato] =" & Agro_SQL_SaveNum(Gias_Stato) & " " & vbCrLf)
                Stb1.Append(" WHERE CodiceSoggetto=" & Agro_SQL_SaveText_NULL(codiceSoggetto) & " " & vbCrLf)
                Stb1.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function EliminaWs_RegVino_Soggetti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                           codOper As String, _
                                           codiceSoggetto As String) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.EliminaWs_RegVino_Soggetti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim ret As Boolean = False
        Try

            Stb.Append("UPDATE ws_RegVino_Soggetti " & vbCrLf)
            Stb.Append(" SET " & vbCrLf)
            Stb.Append(" TipoRichiesta = 'E', " & vbCrLf)
            Stb.Append(" GIAS_Stato = 18000001 " & vbCrLf)
            Stb.Append(" WHERE CodiceSoggetto = " & Agro_SQL_SaveText_NULL(codiceSoggetto) & " " & vbCrLf)
            Stb.Append(" AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " ")

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret
    End Function

End Class
