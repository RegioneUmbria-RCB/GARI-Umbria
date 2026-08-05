Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class BIO_Notifica_SezE_Particelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer, _
                            ByVal NumeroOrdine As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  BIO_Notifica_SezE_Particelle ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If NumeroOrdine <> 0 Then
                StrSQL.Append(" AND NumeroOrdine = " & Agro_SQL_SaveNum(NumeroOrdine) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezE_Particelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezE_Particelle.Inviato =-1 ")
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

    '##############################################################################################
    Public Function LeggiConTitoloPossesso(ByVal Notifica_ID As Integer,
                                                ByVal NumeroOrdine As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_R.LeggiConTitoloPossesso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_SezE_Particelle.*, TitoloPossesso_Sigla, TitoloPossesso_Des, ")
            StrSQL.Append(" TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des ")
            StrSQL.Append(" FROM  BIO_Notifica_SezE_Particelle ")
            StrSQL.Append(" INNER JOIN BIO_Dati_TitoloPossesso  ")
            StrSQL.Append(" ON BIO_Notifica_SezE_Particelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod  ")

            StrSQL.Append(" WHERE BIO_Notifica_SezE_Particelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   BIO_Notifica_SezE_Particelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND BIO_Notifica_SezE_Particelle.Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezE_Particelle.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If NumeroOrdine <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezE_Particelle.NumeroOrdine = " & Agro_SQL_SaveNum(NumeroOrdine) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezE_Particelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezE_Particelle.Inviato =-1 ")
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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class BIO_Notifica_SezE_Particelle_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                            ByVal NumeroOrdine As Integer,
                            ByVal CodIstat_Provincia As String,
                            ByVal CodIstat_Comune As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Integer,
                            ByVal Numero As Integer,
                            ByVal Subalterno As String,
                            ByVal TitoloPossesso As Integer,
                            ByVal Sup_Catastale As Decimal,
                            ByVal Sup_Convenzionale As Decimal,
                            ByVal Sup_Conversione As Decimal,
                            ByVal Sup_Biologico As Decimal,
                            ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezE_Particelle " + vbCrLf)

            StrSQL.Append("             (Notifica_SuperUser, " + vbCrLf)
            StrSQL.Append("             Notifica_ID, " + vbCrLf)
            StrSQL.Append("             NumeroOrdine, " + vbCrLf)
            StrSQL.Append("             CodIstat_Provincia, " + vbCrLf)
            StrSQL.Append("             CodIstat_Comune, " + vbCrLf)
            StrSQL.Append("             Sezione, " + vbCrLf)
            StrSQL.Append("             Foglio, " + vbCrLf)
            StrSQL.Append("             Numero, " + vbCrLf)
            StrSQL.Append("             Subalterno, " + vbCrLf)
            StrSQL.Append("             TitoloPossesso, " + vbCrLf)
            StrSQL.Append("             Sup_Catastale, " + vbCrLf)
            StrSQL.Append("             Sup_Convenzionale, " + vbCrLf)
            StrSQL.Append("             Sup_Conversione, " + vbCrLf)
            StrSQL.Append("             Sup_Biologico, " + vbCrLf)
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Notifica_ID)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(NumeroOrdine)) + "" + vbCrLf)
            StrSQL.Append(" , '" + Agro_SQL_SaveText(Trim(CodIstat_Provincia)) + "' " + vbCrLf)
            StrSQL.Append(" , '" + Agro_SQL_SaveText(Trim(CodIstat_Comune)) + "' " + vbCrLf)
            StrSQL.Append(" , '" + Agro_SQL_SaveText(Trim(Sezione)) + "' " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Foglio)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Numero)) + "" + vbCrLf)
            StrSQL.Append(" , '" + Agro_SQL_SaveText(Trim(Subalterno)) + "' " + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(TitoloPossesso)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Sup_Catastale)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Sup_Convenzionale)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Sup_Conversione)) + "" + vbCrLf)
            StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Sup_Biologico)) + "" + vbCrLf)
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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


    '##############################################################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                              ByVal NumeroOrdine As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE BIO_Notifica_SezE_Particelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM BIO_Notifica_SezE_Particelle ")
                StrSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If
            '---------------------------------------------

            If NumeroOrdine <> 0 Then
                StrSQL.Append(" AND NumeroOrdine = " & Agro_SQL_SaveNum(NumeroOrdine) & "  ")
            End If

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