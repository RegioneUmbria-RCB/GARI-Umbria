Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CDX_PAP_Zeta_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PAP_Piva As String, _
                            ByVal ID_PAPzoo As Integer, _                             
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CDX_PAPZeta ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND PAP_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PAP_Piva <> "" Then
                StrSQL.Append(" AND PAP_Piva = '" & Agro_SQL_SaveText(PAP_Piva) & "' ")
            End If

            If ID_PAPzoo <> 0 Then
                StrSQL.Append(" AND ID_PAPzoo = " & Agro_SQL_SaveNum(ID_PAPzoo) & "  ")
            End If

            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

Public Class CDX_PAPZeta_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PAP_Piva As String, _
                            ByVal ID_PAPzoo As Integer, _
                            ByVal Anno As Integer, _
                            ByVal Organismo_Sigla As String, _
                            ByVal Sede_Regionale_Cod As String, _
                            ByVal Regione_Cod As String, _
                            ByVal Protocollo As String, _
                            ByVal Protocollo_Data As Date, _
                            ByVal Flag_Prima_Variazione As String, _
                            ByVal CUAA As String, _
                            ByVal Codice_Operatore As String, _
                            ByVal Data_Firma_Doc As Date, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal Validazione As Integer, _
                            ByVal Data_Validazione As Date, _
                            ByVal UserName_Validazione As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO CDX_PAPZeta " + vbCrLf)

            StrSQL.Append("             (PAP_SuperUser, PAP_Piva, ID_PAPzoo,  " + vbCrLf)
            StrSQL.Append("              Anno, Organismo_Sigla, Sede_Regionale_Cod, Regione_Cod, " + vbCrLf)
            StrSQL.Append("              Protocollo, Protocollo_Data, Flag_Prima_Variazione, " + vbCrLf)
            StrSQL.Append("               CUAA, Codice_Operatore, Data_Firma_Doc, " + vbCrLf)
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione, DataLock" + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(PAP_Piva)) + "' " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(ID_PAPzoo)) + " " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Anno)) + " " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Organismo_Sigla)) + "' " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Sede_Regionale_Cod)) + "' " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Regione_Cod)) + "' " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Protocollo)) + "' " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveDate(Trim(Protocollo_Data)) + " " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Flag_Prima_Variazione)) + "' " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(CUAA)) + "' " + vbCrLf)
            StrSQL.Append(" ,'" + Agro_SQL_SaveText(Trim(Codice_Operatore)) + "' " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveDate(Trim(Data_Firma_Doc)) + " " + vbCrLf)

            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " + vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " + vbCrLf)
            StrSQL.Append("         , " + Agro_SQL_SaveNum(Trim(Validazione)) + "  " + vbCrLf)
            StrSQL.Append("         , " + Agro_SQL_SaveNum(Trim(Data_Validazione)) + " " + vbCrLf)
            StrSQL.Append("         , '" + Agro_SQL_SaveText(Trim(UserName_Validazione)) + "' " + vbCrLf)
            StrSQL.Append("         , 0  ")
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







End Class
