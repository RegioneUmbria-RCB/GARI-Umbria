Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class xDBVasiSiRPV_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Public Function getVaso(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiceVaso As String, codiceIcqrf As String, codOper As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getVaso()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codiceIcqrf) & " " & vbCrLf)
            Stb.Append(" AND CodVaso=" & Agro_SQL_SaveText_NULL(codiceVaso) & " " & vbCrLf)
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

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

    Public Function getListaCodOper(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCod As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT distinct(CodOper) " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaCod.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCod
    End Function

    Function getListaCodIcqrf(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCod As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT distinct(CodiceIcqrf) " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaCod.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCod
    End Function

    Function getCodOperFromCodIcqrf(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codIcqrf As String) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codOper As String = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT CodOper " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                codOper = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codOper
    End Function

    Function personaFisica(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As Boolean
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim persFisica As Boolean = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT CodOper_Fisiche_Giuridiche " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

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

    Function LeggiVasiPerRichiesta(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, statoGIAS As Integer, tipoRichiesta As String, codIcqrf As String, codOper As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiVasiPerRichiesta()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta=" & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            Stb.Append(" AND GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)

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

    Function listaVasiDaAggiornare(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.listaVasiDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaVasi As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT ws_RegVino_CodVaso " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_LogINvio_DettaglioVasi " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaVasi.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaVasi
    End Function

    Function getCodIcqrfFromLogInvioID(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codIcqrf As String = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT ws_RegVino_CodiceIcqrf " & vbCrLf)
            Stb.Append(" FROM ws_regVino_LogInvio_DettaglioVasi " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                codIcqrf = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codIcqrf
    End Function

    Function getCodOperFromLogInvioID(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codIcqrf As String = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT codOper " & vbCrLf)
            Stb.Append(" FROM ws_regVino_LogInvio_DettaglioVasi " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                codIcqrf = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codIcqrf
    End Function

    Function getDBVasiPerCodIcqrf(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codIcqrf As String, codOper As String) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getDBSoggettiPerCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaVasi As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT CodVaso " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" And CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta in ('A','I') " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                For Each row As DataRow In DT.Rows
                    listaVasi.Add(CStr(row.Item(0)))
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaVasi
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

            Stb.Append(" Select distinct l.ws_RegVino_LogInvio_Cod, l.idTrasmissione_SIAN, ls.ws_RegVino_CodiceIcqrf " & vbCrLf)
            Stb.Append(" FROM  ws_RegVino_LogInvio l " & vbCrLf)
            Stb.Append(" INNER JOIN ws_RegVino_LogInvio_DettaglioVasi ls ON l.ws_RegVino_LogInvio_Cod = ls.ws_RegVino_LogInvio_Cod " & vbCrLf)
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

    Public Function LeggiWs_RegVino_Vasi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                         Optional ByVal codOper As String = "", _
                                         Optional ByVal codOperFisicheGiuridiche As String = "", _
                                         Optional ByVal CodiceIcqrf As String = "", _
                                         Optional ByVal CodVaso As String = "", _
                                         Optional ByVal TipoVaso As String = "", _
                                         Optional ByVal Descrizione As String = "", _
                                         Optional ByVal Volume As String = "", _
                                         Optional ByVal TipoRichiesta As String = "", _
                                         Optional ByVal GIAS_Stato As Integer = 0) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiWs_RegVino_Vasi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("SELECT  " & vbCrLf)
            Stb.Append(" CodOper, " & vbCrLf)
            Stb.Append(" CodOper_Fisiche_Giuridiche, " & vbCrLf)
            Stb.Append(" CodiceIcqrf, " & vbCrLf)
            Stb.Append(" CodVaso, " & vbCrLf)
            Stb.Append(" TipoVaso, " & vbCrLf)
            Stb.Append(" Descrizione, " & vbCrLf)
            Stb.Append(" Volume, " & vbCrLf)
            Stb.Append(" TipoRichiesta, " & vbCrLf)
            Stb.Append(" Gias_Stato " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE 1=1")

            If codOper <> "" Then
                Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            If codOperFisicheGiuridiche <> "" Then
                Stb.Append(" AND CodOper_Fisiche_Giuridiche=" & codOperFisicheGiuridiche & " " & vbCrLf)
            End If

            If CodiceIcqrf <> "" Then
                Stb.Append(" AND CodiceIcqrf=" & Agro_SQL_SaveText_NULL(CodiceIcqrf) & " " & vbCrLf)
            End If

            If CodVaso <> "" Then
                Stb.Append(" AND CodVaso=" & Agro_SQL_SaveText_NULL(CodVaso) & " " & vbCrLf)
            End If

            If TipoVaso <> "" Then
                Stb.Append(" AND TipoVaso=" & Agro_SQL_SaveText_NULL(TipoVaso) & " " & vbCrLf)
            End If

            If Descrizione <> "" Then
                Stb.Append(" AND Descrizione=" & Agro_SQL_SaveText_NULL(Descrizione) & " " & vbCrLf)
            End If

            If Volume <> "" Then
                Stb.Append(" AND Volume=" & Agro_SQL_SaveNum_NULL(Volume) & " " & vbCrLf)
            End If

            If TipoRichiesta <> "" Then
                Stb.Append(" AND TipoRichiesta=" & Agro_SQL_SaveText_NULL(TipoRichiesta) & " " & vbCrLf)
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

Public Class xDBVasiSiRPV_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Sub aggiornaStato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiceVaso As String, codiceIcqrf As String, statoGIAS As Integer, codOper As String)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & codiceVaso & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" SET GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE CodVaso='" & Agro_SQL_SaveText(codiceVaso) & "' " & vbCrLf)
            Stb.Append(" AND CodiceIcqrf='" & Agro_SQL_SaveText(codiceIcqrf) & "' " & vbCrLf)
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
    End Sub

    Sub aggiornaStato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiciVasi As List(Of String), codiceIcqrf As String, statoGIAS As Integer, codOper As String)
        For Each codiceVaso As String In codiciVasi
            aggiornaStato(objParametri, codiceVaso, codiceIcqrf, statoGIAS, codOper)
        Next
    End Sub

    Function inserisciLogInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByVal ws_RegVino_Vasi_Codici As List(Of String), _
                                      ByVal codIcqrf As String, _
                                      ByVal logInvio_Des As String, _
                                      ByVal esitoSIAN As String, _
                                      ByVal Operazione As String, _
                                      ByVal Controllata As Boolean, _
                                      ByVal idTrasmissione As String, _
                                      ByVal codOper As String, _
                                      Optional ByVal dataInvio As Nullable(Of DateTime) = Nothing) As Integer
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
            For Each vasoCodice As String In ws_RegVino_Vasi_Codici

                '---------------------------------------------
                StbSoggetti.Length = 0
                StbSoggetti.Append(" INSERT into ws_RegVino_LogInvio_DettaglioVasi " & vbCrLf)
                StbSoggetti.Append(" (ws_RegVino_LogInvio_Cod, ws_RegVino_CodVaso, ws_RegVino_CodiceIcqrf, codOper) " & vbCrLf)
                StbSoggetti.Append(" VALUES ( " & vbCrLf)
                StbSoggetti.Append("" & CStr(id) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(vasoCodice) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(codIcqrf) & ", " & vbCrLf)
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


    Sub eliminaVaso(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiceVaso As String, codiceIcqrf As String, codOper As String)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.eliminaVaso(" & codiceVaso & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append("DELETE FROM [dbo].[ws_RegVino_LogInvio_DettaglioVasi] " & vbCrLf)
            Stb.Append(" WHERE [CodOper]=" & Agro_SQL_SaveText_NULL(codOper) & " AND " & vbCrLf)
            Stb.Append(" [ws_RegVino_CodiceIcqrf] = " & Agro_SQL_SaveText_NULL(codiceIcqrf) & " AND " & vbCrLf)
            Stb.Append(" [ws_RegVino_CodVaso]= " & Agro_SQL_SaveText_NULL(codiceVaso) & " ")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0
            Stb1.Append("DELETE FROM [dbo].[ws_RegVino_Vasi] " & vbCrLf)
            Stb1.Append(" WHERE [CodOper]=" & Agro_SQL_SaveText_NULL(codOper) & " AND " & vbCrLf)
            Stb1.Append(" [CodiceIcqrf]= " & Agro_SQL_SaveText_NULL(codiceIcqrf) & " AND " & vbCrLf)
            Stb1.Append(" [CodVaso]= " & Agro_SQL_SaveText_NULL(codiceVaso) & " ")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)
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
    End Sub

    Public Function InserisciAggiornaWs_RegVino_Vasi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                     codOper As String, _
                                                     codOper_FisicheGiuridiche As String, _
                                                     codIcqrf As String, _
                                                     codVaso As String, _
                                                     TipoVaso As String, _
                                                     Descrizione As String, _
                                                     Volume As Double, _
                                                     TipoRichiesta As String, _
                                                     Gias_Stato As Integer)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.InserisciAggiornaWs_RegVino_Vasi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append("SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" WHERE  " & vbCrLf)
            Stb.Append(" CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & "" & vbCrLf)
            Stb.Append(" AND CodVaso=" & Agro_SQL_SaveText_NULL(codVaso) & " ")
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0
            If DT.Rows.Count > 0 Then
                'UPDATE
                Stb1.Append("UPDATE [dbo].[ws_RegVino_Vasi] " & vbCrLf)
                Stb1.Append("    SET [CodOper] =" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                Stb1.Append("       ,[CodOper_Fisiche_Giuridiche] =" & Agro_SQL_SaveText_NULL(codOper_FisicheGiuridiche) & " " & vbCrLf)
                Stb1.Append("       ,[CodiceIcqrf] =" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                Stb1.Append("       ,[CodVaso] =" & Agro_SQL_SaveText_NULL(codVaso) & " " & vbCrLf)
                Stb1.Append("       ,[TipoVaso] =" & Agro_SQL_SaveText_NULL(TipoVaso) & " " & vbCrLf)
                Stb1.Append("       ,[Descrizione] =" & Agro_SQL_SaveText_NULL(Descrizione) & " " & vbCrLf)
                Stb1.Append("       ,[Volume] =" & Agro_SQL_SaveNum(Volume) & " " & vbCrLf)
                Stb1.Append("       ,[TipoRichiesta] =" & Agro_SQL_SaveText_NULL(TipoRichiesta) & " " & vbCrLf)
                Stb1.Append("       ,[Gias_Stato] =" & Agro_SQL_SaveNum(Gias_Stato) & " " & vbCrLf)
                Stb1.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                Stb1.Append(" AND CodVaso=" & Agro_SQL_SaveText_NULL(codVaso) & " ")
                Stb1.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " ")
            Else
                'INSERT
                Stb1.Append("INSERT INTO [dbo].[ws_RegVino_Vasi] " & vbCrLf)
                Stb1.Append("            ([CodOper],[CodOper_Fisiche_Giuridiche],[CodiceIcqrf],[CodVaso],[TipoVaso],[Descrizione],[Volume],[TipoRichiesta],[Gias_Stato]) " & vbCrLf)
                Stb1.Append("                 VALUES " & vbCrLf)
                Stb1.Append("            (" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(codOper_FisicheGiuridiche) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(codVaso) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(TipoVaso) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(Descrizione) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveNum(Volume) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveText_NULL(TipoRichiesta) & " " & vbCrLf)
                Stb1.Append("            ," & Agro_SQL_SaveNum(Gias_Stato) & " )")
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

    Public Function EliminaWs_RegVino_Vasi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                           codOper As String, _
                                           codIcqrf As String, _
                                           codVaso As String) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.EliminaWs_RegVino_Vasi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim ret As Boolean = False
        Try

            Stb.Append("UPDATE ws_RegVino_Vasi " & vbCrLf)
            Stb.Append(" SET " & vbCrLf)
            Stb.Append(" TipoRichiesta = 'E', " & vbCrLf)
            Stb.Append(" GIAS_Stato = 18000001 " & vbCrLf)
            Stb.Append(" WHERE CodVaso = " & Agro_SQL_SaveText_NULL(codVaso) & " " & vbCrLf)
            Stb.Append(" AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND CodiceIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " ")

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

