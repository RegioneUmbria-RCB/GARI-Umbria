Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class xDBProdottiSiRPV_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Public Function getProdotto(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String, codiceIcqrf As String, mat_cod As String, lotto As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getProdotto(" & mat_cod & "," & lotto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE 1 = 1" & vbCrLf)
            If codOper <> "" Then
                Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If
            If codiceIcqrf <> "" Then
                Stb.Append(" AND CodIcqrf=" & Agro_SQL_SaveText_NULL(codiceIcqrf) & " " & vbCrLf)
            End If
            If mat_cod <> "" Then
                Stb.Append(" AND Mat_Cod=" & Agro_SQL_SaveNum_NULL(CInt(mat_cod)) & " " & vbCrLf)
            End If
            If lotto <> "" Then
                Stb.Append(" AND Lotto=" & Agro_SQL_SaveText_NULL(lotto) & " " & vbCrLf)
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

    Function getListaCodOper(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
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
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaCod.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCod
    End Function

    Function getListaCodIcqrf(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getListaCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCod As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT distinct(CodIcqrf) " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaCod.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCod
    End Function

    Function getCodOperFromCodIcqrf(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codIcqrf As String) As String
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
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                codOper = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codOper
    End Function

    Function personaFisica(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String) As Boolean
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.personaFisica()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim persFisica As Boolean = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT CodOper_Fisiche_Giuridiche " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
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
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return persFisica
    End Function

    Function getDBProddottiPerCodIcqrf(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codIcqrf As String, codOper As String) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getDBProddottiPerCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaProd As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT Mat_Cod, Lotto " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" WHERE CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta in ('A','I') " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                For Each row As DataRow In DT.Rows
                    listaProd.Add(CStr(row.Item(0)) & "|" & CStr(row.Item(1)))
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaProd
    End Function

    Function LeggiProdottiPerRichiesta(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       statoGIAS As Integer,
                                       tipoRichiesta As String,
                                       codOper As String,
                                       codIcqrf As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiProdottiPerRichiesta()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" WHERE CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" AND TipoRichiesta=" & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            Stb.Append(" AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND GIAS_Stato=" & CStr(statoGIAS) & " " & vbCrLf)

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

    Function getCodIcqrfFromLogInvioID(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.getCodOperFromCodIcqrf()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codIcqrf As String = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT p.codIcqrf " & vbCrLf)
            Stb.Append(" FROM ws_regVino_LogInvio_DettaglioProdotti dp " & vbCrLf)
            Stb.Append(" INNER JOIN ws_RegVino_Prodotti p ON dp.Mat_Cod=p.Mat_Cod AND dp.Lotto=p.Lotto" & vbCrLf)
            Stb.Append(" WHERE dp.ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                codIcqrf = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codIcqrf
    End Function

    Function listaProdottiDaAggiornare(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, logInvioID As Integer) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.listaProdottiDaAggiornare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaVasi As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT Mat_Cod, Lotto " & vbCrLf)
            Stb.Append(" FROM ws_RegVino_LogINvio_DettaglioProdotti " & vbCrLf)
            Stb.Append(" WHERE ws_RegVino_LogInvio_Cod=" & CStr(logInvioID) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaVasi.Add(CStr(row.Item(0)) & "|" & CStr(row.Item(1)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaVasi
    End Function

    Function getOperazioniDaControllare(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, operazione As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCore_DAL.getOperazioniDaControllare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append(" Select distinct l.ws_RegVino_LogInvio_Cod, l.idTrasmissione_SIAN " & vbCrLf)
            Stb.Append(" FROM  ws_RegVino_LogInvio l " & vbCrLf)
            Stb.Append(" INNER JOIN ws_RegVino_LogInvio_DettaglioProdotti ls ON l.ws_RegVino_LogInvio_Cod = ls.ws_RegVino_LogInvio_Cod " & vbCrLf)
            Stb.Append(" WHERE l.Operazione=" & Agro_SQL_SaveText_NULL(operazione) & " AND Controllata='False'" & vbCrLf)

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

    Public Function prodottiRead(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, codOper As String, codIcqrf As String, tipoRichiesta As String, Mat_cod As Integer, lotto As String, modificato As Integer?, GIAS_Stato As Integer?) As DataTable
        Dim NomeRoutine As String = "AgronicaCore_DAL.getOperazioniDaControllare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try
            Stb.Append("Select * " & vbCrLf)
            Stb.Append("   FROM [dbo].[ws_RegVino_Prodotti] " & vbCrLf)
            Stb.Append("   WHERE 1=1 " & vbCrLf)
            If codOper <> "" Then
                Stb.Append("   AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If
            If codIcqrf <> "" Then
                Stb.Append("   AND CodIcqrf= " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            End If
            If tipoRichiesta <> "" Then
                Stb.Append("   AND TipoRichiesta = " & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            End If
            If Mat_cod <> 0 Then
                Stb.Append("   AND Mat_Cod = " & Agro_SQL_SaveNum_NULL(Mat_cod) & " " & vbCrLf)
            End If
            If lotto <> "" Then
                Stb.Append("   AND Lotto = " & Agro_SQL_SaveText_NULL(lotto) & " " & vbCrLf)
            End If
            If modificato IsNot Nothing Then
                Stb.Append("   AND modificato = " & Agro_SQL_SaveNum_NULL(modificato) & " " & vbCrLf)
            End If
            If GIAS_Stato IsNot Nothing Then
                Stb.Append("   AND GIAS_Stato = " & Agro_SQL_SaveNum_NULL(GIAS_Stato) & " " & vbCrLf)
            End If

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


End Class

Public Class xDBProdottiSiRPV_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim utils As DBUtilityComuni

    Public Sub New()
        utils = New DBUtilityComuni()
    End Sub

    Public Function aggiornaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiceProdotto As String, _
                                  ByVal codIcqrf As String, _
                                  ByVal codOper As String, _
                                  ByVal statoGIAS As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & codiceProdotto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim codicePrimario = ""
            Dim codiceSecondario = ""
            If codiceProdotto <> "" Then
                Dim codici = codiceProdotto.Split("|")
                codicePrimario = codici(0)
                codiceSecondario = codici(1)
            End If
            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET GIAS_Stato = " & Agro_SQL_SaveNum(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)

            If codOper <> "" Then
                Stb.Append(" AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            If codIcqrf <> "" Then
                Stb.Append(" AND CodIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            End If

            If codicePrimario <> "" Then
                Stb.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum_NULL(CInt(codicePrimario)) & " " & vbCrLf)
            End If

            If codiceSecondario <> "" Then
                Stb.Append(" AND Lotto = " & Agro_SQL_SaveText_NULL(codiceSecondario) & "")
            End If


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

    Public Function aggiornaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByVal codIcqrf As String,
                                  ByVal codOper As String,
                                  ByVal mat_Cod As Integer,
                                  ByVal lotto As String,
                                  ByVal statoGIAS As Integer) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & mat_Cod & "-" & lotto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET GIAS_Stato = " & Agro_SQL_SaveNum(statoGIAS) & " " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)

            If codOper <> "" Then
                Stb.Append(" AND CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            If codIcqrf <> "" Then
                Stb.Append(" AND CodIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            End If

            If mat_Cod <> 0 Then
                Stb.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum_NULL(CInt(mat_Cod)) & " " & vbCrLf)
            End If

            If lotto <> "" Then
                Stb.Append(" AND Lotto = " & Agro_SQL_SaveText_NULL(lotto) & "")
            End If


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

    Public Function aggiornaModificato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                  ByVal codiceProdotto As String, _
                                  ByVal codIcqrf As String, _
                                  ByVal codOper As String, _
                                  ByVal modificato As Integer,
                                  ByVal tipoRichiesta As String) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaStato(" & codiceProdotto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim codici = codiceProdotto.Split("|")
            Dim codicePrimario = codici(0)
            Dim codiceSecondario = codici(1)
            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET modificato = " & Agro_SQL_SaveNum(modificato) & " " & vbCrLf)
            Stb.Append(" , TipoRichiesta = " & Agro_SQL_SaveText_NULL(tipoRichiesta) & " " & vbCrLf)
            Stb.Append(" WHERE " & vbCrLf)
            Stb.Append(" CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND CodIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum_NULL(CInt(codicePrimario)) & " " & vbCrLf)
            Stb.Append(" AND Lotto = " & Agro_SQL_SaveText_NULL(codiceSecondario) & "")

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
                                  ByVal codiciProdotti As List(Of String), _
                                  ByVal codIcqrf As String,
                                  ByVal codOper As String, _
                                  ByVal statoGIAS As Integer) As Boolean

        For Each codiceProdotto As String In codiciProdotti
            aggiornaStato(objParametri, codiceProdotto, codIcqrf, codOper, statoGIAS)
        Next

        Return True
    End Function

    Public Function AggiornaCodiciSIAN(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal codOper As String,
                                       ByVal codIcqrf As String,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Lotto As String,
                                       ByVal CodicePrimario As String,
                                       ByVal CodiceSecondario As String) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.AggiornaCodiciSIAN(" & CStr(Mat_Cod) & ", " & Lotto & ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET CodPrimario = " & Agro_SQL_SaveText_NULL(CodicePrimario) & " " & vbCrLf)
            Stb.Append(" ,CodSecondario = " & Agro_SQL_SaveText_NULL(CodiceSecondario) & " " & vbCrLf)
            Stb.Append(" WHERE " & vbCrLf)
            Stb.Append(" CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            Stb.Append(" AND CodIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            Stb.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum_NULL(Mat_Cod) & " " & vbCrLf)
            Stb.Append(" AND Lotto = " & Agro_SQL_SaveText_NULL(Lotto) & "")

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

    Function inserisciLogInvio(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               listaProdottiValidi As List(Of String),
                               codIcqrf As String,
                               LogInvio_Des As String,
                               esitoSIAN As String,
                               Operazione As String,
                               Controllata As Boolean,
                               idTrasmissione As String,
                               codOper As String,
                                Optional ByVal dataInvio As Nullable(Of DateTime) = Nothing) As Integer
        Dim id = utils.NuovoID_ws_RegVino_LogInvio(ObjParametri)
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
            StbLogInvio.Append("" & Agro_SQL_SaveStringToXML(LogInvio_Des) & " , " & vbCrLf)
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
            xRispLovInvio = EseguiQuery_Scrittura(ObjParametri, StbLogInvio.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRispLovInvio = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


        'INSERISCO LOGINVIO_DETTAGLIOPRODOTTI
        Dim StbSoggetti As New System.Text.StringBuilder
        Dim xRispSoggetti As Boolean = False
        Try
            For Each prodottoCodice As String In listaProdottiValidi

                '---------------------------------------------
                StbSoggetti.Length = 0
                StbSoggetti.Append(" INSERT into ws_RegVino_LogInvio_DettaglioProdotti " & vbCrLf)
                StbSoggetti.Append(" (ws_RegVino_LogInvio_Cod, Mat_Cod, Lotto, codIcqrf, codOper) " & vbCrLf)
                StbSoggetti.Append(" VALUES ( " & vbCrLf)
                StbSoggetti.Append("" & CStr(id) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveNum_NULL(CInt(prodottoCodice.Split("|")(0))) & " , " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(prodottoCodice.Split("|")(1)) & ", " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(codIcqrf) & ", " & vbCrLf)
                StbSoggetti.Append("" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                StbSoggetti.Append(" ) " & vbCrLf)

                '--------------------------------------------------------------------------
                xRispSoggetti = EseguiQuery_Scrittura(ObjParametri, StbSoggetti.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Next

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRispSoggetti = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
        Return id
    End Function

    Public Function ResettaCodiciSIAN(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreRegVino_DAL.ResettaCodiciSIAN"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.Append(" UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET CodPrimario = NULL " & vbCrLf)
            Stb.Append(" ,CodSecondario = NULL " & vbCrLf)

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

End Class