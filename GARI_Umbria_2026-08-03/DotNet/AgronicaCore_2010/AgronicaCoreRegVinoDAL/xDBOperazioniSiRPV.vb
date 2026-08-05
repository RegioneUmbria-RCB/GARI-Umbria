Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class xDBOperazioniSiRPV_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Public Function LeggiOperazione(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal OperazioneCod As Integer
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & CStr(OperazioneCod) & " " & vbCrLf)

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

    Public Function LeggiOperazioniModificate(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal Modificato As Integer
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazioniModificate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE Modificato=" & CStr(Modificato) & " " & vbCrLf)

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

    Function leggiProdotti(
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef codiceOperazione As Integer,
                          Optional ByRef elementoXml As String = "") As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiProdotti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni_Prodotti " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & Agro_SQL_SaveNum_NULL(codiceOperazione) & " " & vbCrLf)
            If elementoXml <> "" Then
                Stb.Append(" AND ElementoXML=" & Agro_SQL_SaveText_NULL(elementoXml) & " " & vbCrLf)
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

    Function getListaCodOper(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaCodOper()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCod As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT distinct(CodOper) " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)

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
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
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
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
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
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
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

    Function LeggiOperazioniPerRichiesta(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, statoGIAS As Integer, tipoRichiesta As String, codIcqrf As String, codOper As String, orderBy As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazioniPerRichiesta()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE CodiceIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta=" & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            Stb.Append(" AND GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)
            Stb.Append(" AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            If orderBy <> "" Then
                Stb.Append(Agro_SQL_Save_xOrderBy(orderBy, objParametri) & vbCrLf)
            Else
                Stb.Append(" ORDER BY DataOperazione, NumOperazione, ID_Agenda " & vbCrLf)
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

    Function getCodiceOperazioneFromControlloEsito(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, dataOperazione As Date, NumOperazione As Integer, codOperazione As String) As Integer
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodiceOperazioneFromControlloEsito()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Dim dataOpStart As DateTime = New Date(dataOperazione.Year, dataOperazione.Month, dataOperazione.Day, 0, 0, 0)
            Dim dataOpEnd As DateTime = New Date(dataOperazione.Year, dataOperazione.Month, dataOperazione.Day, 23, 59, 59)
            Stb.Append(" SELECT ws_RegVino_Operazione_cod " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE DataOperazione>=" & Agro_SQL_SaveDateTime_NULL(dataOpStart) & " " & vbCrLf)
            Stb.Append(" AND DataOperazione<=" & Agro_SQL_SaveDateTime_NULL(dataOpEnd) & " " & vbCrLf)
            Stb.Append(" AND NumOperazione=" & CStr(NumOperazione) & " " & vbCrLf)
            Stb.Append(" AND CodiceOperazione=" & Agro_SQL_SaveText_NULL(codOperazione) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        If DT.Rows.Count > 0 Then
            Return CInt(DT.Rows(0).Item("ws_RegVino_Operazione_cod"))
        Else
            Return 0
        End If
    End Function

    Function getOperazioniDaControllare(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, oper As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCore_DAL.getOperazioniDaControllare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append(" Select distinct l.ws_RegVino_LogInvio_Cod, l.idTrasmissione_SIAN " & vbCrLf)
            Stb.Append(" FROM  ws_RegVino_LogInvio l " & vbCrLf)
            Stb.Append(" WHERE l.Operazione=" & Agro_SQL_SaveText_NULL(oper) & " AND Controllata='False'" & vbCrLf)

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

    Function listaOperazioniDaAggiornare(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.listaOperazioniDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaOperazioni As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT ws_RegVino_Operazione_cod " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_LogInvio_Dettaglio " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaOperazioni.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaOperazioni
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

            Stb.Append("Select lo.CodiceIcqrf " & vbCrLf)
            Stb.Append(" FROM ws_regVino_LogInvio_Dettaglio l " & vbCrLf)
            Stb.Append(" INNER JOIN ws_RegVino_Operazioni lo ON l.ws_RegVino_Operazione_cod=lo.ws_RegVino_Operazione_cod  " & vbCrLf)
            Stb.Append(" WHERE l.ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " ")

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

    Function toSendProdotto(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            operationCode As String,
                            prodottoCode As String,
                            dbCode As String,
                            sendAll As Boolean) As Boolean
        Dim NomeRoutine As String = "AgronicaCore_DAL.toSendProdotto(" & operationCode & ", " & prodottoCode & ", " & dbCode & ")"
        If sendAll Then
            Return True
        Else
            '----- Variabili
            Dim MessaggioErrore As String = ""
            Dim Stb As New System.Text.StringBuilder
            Dim Stb1 As New System.Text.StringBuilder
            Dim DT As DataTable
            Dim DT1 As DataTable
            Dim toSend = False
            Try

                Stb.Length = 0

                Stb.Append("Select Tipo " & vbCrLf)
                Stb.Append(" FROM TeleRegistri_Nodi  " & vbCrLf)
                Stb.Append(" WHERE Operazione_Cod = " & Agro_SQL_SaveText_NULL(operationCode) & "  " & vbCrLf)
                Stb.Append(" AND Elemento_Xml = " & Agro_SQL_SaveText_NULL(prodottoCode) & "")
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    Dim tipo As Integer = CInt(DT.Rows(0).Item(0))

                    Stb1.Length = 0

                    Stb1.Append("SELECT ChkAttributo_Obbligatorio  " & vbCrLf)
                    Stb1.Append(" FROM TeleRegistri_Attributi " & vbCrLf)
                    Stb1.Append(" WHERE Operazione_Cod = " & Agro_SQL_SaveText_NULL(operationCode) & " " & vbCrLf)
                    Stb1.Append(" AND Tipo=" & Agro_SQL_SaveNum_NULL(tipo) & " " & vbCrLf)
                    Stb1.Append(" AND Attributo_Cod like " & Agro_SQL_SaveText_NULL(dbCode) & " ")

                    '--------------------------------------------------------------------------
                    DT1 = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If DT1.Rows.Count > 0 Then
                        Dim obb = CInt(DT1.Rows(0).Item(0))
                        If obb = 1 Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return True
                    End If
                Else
                    Return True
                End If
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                DT = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            Return toSend
        End If
    End Function

    Function toSendProdotto(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            operationCode As String,
                            dbCode As String,
                            sendAll As Boolean) As Boolean
        Dim NomeRoutine As String = "AgronicaCore_DAL.toSendProdotto(" & operationCode & ", -1000 , " & dbCode & ")"
        If sendAll Then
            Return True
        Else
            '----- Variabili
            Dim MessaggioErrore As String = ""
            Dim Stb As New System.Text.StringBuilder
            Dim Stb1 As New System.Text.StringBuilder
            Dim DT As DataTable
            Dim DT1 As DataTable
            Dim toSend = False
            Try

                Stb.Length = 0

                Stb.Append("Select Tipo " & vbCrLf)
                Stb.Append(" FROM TeleRegistri_Nodi  " & vbCrLf)
                Stb.Append(" WHERE Operazione_Cod = " & Agro_SQL_SaveText_NULL(operationCode) & "  " & vbCrLf)
                Stb.Append(" AND Tipo =-1000")
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    Dim tipo As Integer = CInt(DT.Rows(0).Item(0))

                    Stb1.Length = 0

                    Stb1.Append("SELECT ChkAttributo_Obbligatorio  " & vbCrLf)
                    Stb1.Append("FROM TeleRegistri_Attributi " & vbCrLf)
                    Stb1.Append("WHERE Operazione_Cod = " & Agro_SQL_SaveText_NULL(operationCode) & " " & vbCrLf)
                    Stb1.Append("AND Tipo=" & Agro_SQL_SaveNum_NULL(tipo) & " " & vbCrLf)
                    Stb1.Append("AND Attributo_Cod like " & Agro_SQL_SaveText_NULL(dbCode) & " ")

                    '--------------------------------------------------------------------------
                    DT1 = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If DT1.Rows.Count > 0 Then
                        Dim obb = CInt(DT1.Rows(0).Item(0))
                        If obb = 1 Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return True
                    End If
                Else
                    Return True
                End If
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                DT = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            Return toSend
        End If
    End Function

    Function getAttributiProdotto(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, operationCode As String, prodottoCode As String, locoXml As String) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getAttributiProdotto(" & operationCode & "," & prodottoCode & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DT1 As DataTable
        Dim listaAttributi As New List(Of String)
        Try
            Dim tipo As Integer
            Stb1.Length = 0
            If prodottoCode = "" Then
                tipo = -1000
            Else
                Stb.Length = 0

                Stb.Append(" SELECT Tipo " & vbCrLf)
                Stb.Append(" FROM TeleRegistri_Nodi " & vbCrLf)
                Stb.Append(" WHERE Elemento_XML=" & Agro_SQL_SaveText_NULL(prodottoCode) & " " & vbCrLf)
                Stb.Append(" AND Operazione_Cod=" & Agro_SQL_SaveText_NULL(operationCode) & " " & vbCrLf)
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    tipo = CInt(DT.Rows(0).Item(0))
                Else
                    Return listaAttributi
                End If
            End If

            Stb1.Length = 0

            Stb1.Append(" Select attributo_cod " & vbCrLf)
            Stb1.Append(" FROM TeleRegistri_Attributi a " & vbCrLf)
            Stb1.Append(" INNER JOIN TeleRegistri_Dettagli d ON a.Attributo_Cod = d.Codice " & vbCrLf)
            Stb1.Append(" WHERE a.tipo=" & Agro_SQL_SaveNum_NULL(tipo) & " " & vbCrLf)
            Stb1.Append(" AND a.Operazione_Cod=" & Agro_SQL_SaveText_NULL(operationCode) & " " & vbCrLf)
            Stb1.Append(" AND d.Tipo like '%" & CStr(locoXml) & "%'")
            Stb1.Append(" ORDER BY ORDINE")

            '--------------------------------------------------------------------------
            DT1 = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT1.Rows
                listaAttributi.Add(CStr(row.Item(0)))
            Next


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaAttributi
    End Function

    Function leggiCodificaAttributo(objParametri As AgronicaCoreParametri, attributo As String, Optional locoXml As String = Nothing) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.leggiCodificaAttributo(" & attributo & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM TeleRegistri_Dettagli " & vbCrLf)
            Stb.Append(" WHERE Tabella_Cod='attributo' " & vbCrLf)
            Stb.Append(" AND Codice=" & Agro_SQL_SaveText_NULL(attributo) & " " & vbCrLf)
            If locoXml IsNot Nothing Then
                Stb.Append(" AND tipo like '%" & locoXml & "%' " & vbCrLf)
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

    Function LeggiWs_RegVino_Operazioni(objParametri As AgronicaCoreParametri, id As String) As Object
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiWs_RegVino_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("SELECT *  " & vbCrLf)
            Stb.Append(" from ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & Agro_SQL_SaveNum_NULL(CInt(id)) & " ")


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

    Function LeggiOperazioniEliminate(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, eliminata As Integer) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazioniModificate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE Elimina=" & CStr(eliminata) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Function leggiOperazioniStato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, stato As Integer) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazioniModificate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE GIAS_Stato=" & CStr(stato) & " " & vbCrLf)

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

    Function leggiOperazioniDaEliminare(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiOperazioniModificate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE GIAS_Stato=" & CStr(18000008) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta='E' " & vbCrLf)

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

    Function leggiOperazioniSenzaID_Agenda(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.leggiOperazioniSenzaID_Agenda()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select * ")
            Stb.AppendLine(" From WS_regVino_Operazioni o ")
            Stb.AppendLine(" Left Join Agenda a ON o.Id_Agenda = a.Id_Agenda ")
            Stb.AppendLine(" WHERE a.Id_Agenda Is null ")
            Stb.AppendLine(" And GIAS_Stato <> 18000009 ")


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

    Function leggiOperazioniDaAggiornare(OperazioneCod As Integer, ByRef StatoGIAS As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.leggiOperazioniDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE GIAS_Stato=" & CStr(StatoGIAS) & " " & vbCrLf)
            Stb.Append(" AND NumOperazione IN (0," & CStr(OperazioneCod) & ") " & vbCrLf)
            Stb.Append(" ORDER BY DataOperazione, NumOperazione, ID_Agenda ASC " & vbCrLf)

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

    Function leggiMaxProgressivo(ByRef dataOperazione As Date,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal startOperationIndex As Integer = 1) As Integer
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.leggiOperazioniDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim returnValue As Integer

        Try

            Stb.Length = 0
            Dim dataOpStart As DateTime = New Date(dataOperazione.Year, dataOperazione.Month, dataOperazione.Day, 0, 0, 0)
            Dim dataOpEnd As DateTime = New Date(dataOperazione.Year, dataOperazione.Month, dataOperazione.Day, 23, 59, 59)
            Stb.Append(" SELECT Max(NumOperazione) " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE DataOperazione >=" & Agro_SQL_SaveDateTime_NULL(dataOpStart) & " " & vbCrLf)
            Stb.Append(" AND DataOperazione <=" & Agro_SQL_SaveDateTime_NULL(dataOpEnd) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            If DT.Rows.Count > 0 Then
                If CInt(DT.Rows(0).Item(0)) > startOperationIndex Then
                    returnValue = CInt(DT.Rows(0).Item(0))
                Else
                    returnValue = startOperationIndex
                End If
            Else
                returnValue = startOperationIndex
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            returnValue = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return returnValue

    End Function

End Class

Public Class xDBOperazioniSiRPV_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Sub aggiornaStato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiceOperazione As Integer, statoGIAS As Integer)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & CStr(codiceOperazione) & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & CStr(codiceOperazione) & " " & vbCrLf)

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

    Public Sub impostaProgressivo(ws_RegVino_Operazione_Cod As Integer,
                                  Progressivo As Integer,
                                  objParametri As AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.impostaProgressivo(" & CStr(Progressivo) & ")"
        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET NumOperazione=" & Agro_SQL_SaveNum(Progressivo) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_Cod=" & Agro_SQL_SaveNum(ws_RegVino_Operazione_Cod) & " " & vbCrLf)

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

    Sub aggiornaStato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codiciOperazioni As List(Of String), statoGIAS As Integer)
        For Each codiceOperazione As String In codiciOperazioni
            aggiornaStato(objParametri, codiceOperazione, statoGIAS)
        Next
    End Sub

    Function inserisciLogInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                      ByVal ws_RegVino_Operazioni_Codici As List(Of String), _
                                      ByVal logInvio_Des As String, _
                                      ByVal esitoSIAN As String, _
                                      ByVal Operazione As String, _
                                      ByVal Controllata As Boolean, _
                                      ByVal idTrasmissione As String, _
                                      Optional ByVal dataInvio As Nullable(Of Date) = Nothing) As Integer
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
            For Each operazioneCodice As String In ws_RegVino_Operazioni_Codici

                '---------------------------------------------
                StbSoggetti.Length = 0
                StbSoggetti.Append(" INSERT into ws_RegVino_LogInvio_Dettaglio " & vbCrLf)
                StbSoggetti.Append(" (ws_RegVino_LogInvio_Cod, ws_RegVino_Operazione_cod) " & vbCrLf)
                StbSoggetti.Append(" VALUES ( " & vbCrLf)
                StbSoggetti.Append("" & CStr(id) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(operazioneCodice) & " " & vbCrLf)
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

    Function InserisciAggiornaWs_RegVino_Operazioni(objParametri As AgronicaCoreParametri, id As String, tipoRichiesta As String, stato As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.InserisciAggiornaWs_RegVino_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim ret As Boolean = False
        Try

            Stb.Append("UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET " & vbCrLf)
            Stb.Append(" TipoRichiesta = " & Agro_SQL_SaveText_NULL(tipoRichiesta) & ", " & vbCrLf)
            Stb.Append(" GIAS_Stato = " & Agro_SQL_SaveNum_NULL(stato) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod = " & Agro_SQL_SaveNum_NULL(id) & " ")

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret
    End Function

    Function EliminaWs_RegVino_Operazioni(objParametri As AgronicaCoreParametri, id As String) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVinoDAL.xDBOperazioniSiRPV_W.EliminaWs_RegVino_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim ret As Boolean = False
        Try

            Stb.Append("UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET " & vbCrLf)
            Stb.Append(" TipoRichiesta = 'E', " & vbCrLf)
            Stb.Append(" GIAS_Stato = 18000001 " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod = " & Agro_SQL_SaveNum_NULL(id) & " ")

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret
    End Function

    Function aggiornaModificato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, id As Integer, modificato As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.aggiornaModificato(" & CStr(id) & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim ret As Boolean = False

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET Modificato=" & Agro_SQL_SaveNum_NULL(modificato) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & Agro_SQL_SaveNum_NULL(id) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'non ci segnamo lo storico dei passaggi di stato
            '
            '

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret

    End Function

    Sub aggiornaEliminato(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, id As String, elimina As Integer)
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.aggiornaModificato(" & CStr(id) & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" SET elimina=" & Agro_SQL_SaveNum_NULL(elimina) & " " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod=" & Agro_SQL_SaveNum_NULL(id) & " " & vbCrLf)

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

    Sub eliminaOperazione(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, id As String)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.eliminaOperazione()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim ret As Boolean = False
        Try

            Stb.Append("DELETE FROM ws_RegVino_Operazioni " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod = " & Agro_SQL_SaveNum_NULL(id) & " ")

            Stb.Append("DELETE FROM ws_RegVino_Operazioni_Prodotti " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_Operazione_cod = " & Agro_SQL_SaveNum_NULL(id) & " ")

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Sub

End Class