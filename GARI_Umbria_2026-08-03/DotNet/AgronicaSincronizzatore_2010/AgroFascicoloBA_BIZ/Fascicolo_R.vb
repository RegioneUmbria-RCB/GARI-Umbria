Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports SincroAnagrafeBA
Imports AgroFascicoloBA_SIGPA_DAL
Imports System.Xml
Imports AgronicaCoreUtility

Public Class Fascicolo_R
    Inherits AgronicaCoreDataProvider.DataProvider2010


#Region "Lettura dalle varie origini Dati"

    ''' <summary>
    ''' Lettura dalle varie origini Dati
    ''' </summary>
    ''' <param name="EnteValidatore_COD"></param>
    ''' <param name="cuaa"></param>
    ''' <param name="Validazione_Numero"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="StringaConnessioneAlternativa">Stringa connessione ad altra origine dati (es.: oracle di AVEPA)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSingoloFascicoloAgea( _
            ByVal EnteValidatore_COD As enum_EnteValidadore, _
            ByVal cuaa As String, _
            ByVal Validazione_Numero As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
            Optional ByVal StringaConnessioneAlternativa As String = "" _
    ) As String

        Dim objA As New getSchedeFascicoloResponse
        Dim f As Fascicolo

        'gestione della creazione del fascicolo AGEA, partendo Da un'origine scelta in base alla variabile EnteValidatore_COD
        Dim Validazione_Data As DateTime


        'Cambio la connection string sulla connection string di ORACLE.
        Dim oldConnectionString As String = objParametri.StringaConnessione
        If StringaConnessioneAlternativa <> "" Then
            objParametri.StringaConnessione = StringaConnessioneAlternativa
        End If


        Select Case EnteValidatore_COD

            Case enum_EnteValidadore.Agea
                'TODO: Realizzare chiamata a web service BA


            Case enum_EnteValidadore.Avepa


                Dim avepa As New AgroFascicoloBA_AVEPA_BIZ.Fascicolo_R
                f = avepa.GetSingoloFascicoloAgea( _
                    EnteValidatore_COD, _
                    cuaa, _
                    Validazione_Numero, _
                    objParametri _
                )


            Case enum_EnteValidadore.RegioneUmbria_Sigpa
                Dim sigpa As New AgroFascicoloBA_SIGPA_BIZ.Fascicolo_R
                f = sigpa.GetSingoloFascicoloAgea( _
                    EnteValidatore_COD, _
                    cuaa, _
                    Validazione_Numero, _
                    objParametri _
                )


        End Select


        'una volta letto il fascicolo da fonte esterna ripristino la connection string originale
        If StringaConnessioneAlternativa <> "" Then
            objParametri.StringaConnessione = oldConnectionString
        End If


        'finalizzo..
        Dim aSerial As String
        aSerial = AgroSerializer.SerializzaQuesto(Of getSchedeFascicoloResponse)(objA)

        Dim scriviCache As New Fascicolo_W

        'scrivo in cache il fascicolo laddove non esiste..
        scriviCache.SalvaInCacheFascicolo(EnteValidatore_COD, cuaa, Validazione_Numero, Validazione_Data, aSerial, AGRODATAINIZIO, AGRODATAFINE, objParametri)


    End Function

    Public Function getSchedeFascicolo(ByVal EnteValidatore_cod As Integer, ByVal cuaa As String, ByVal DataDa1 As Date, ByVal DataA1 As Date, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of SchedaValidazione)

        Dim lRval As New List(Of SchedaValidazione)


        Select Case EnteValidatore_cod

            Case enum_EnteValidadore.Agea
                'TODO: Realizzare chiamata a web service BA


            Case enum_EnteValidadore.Avepa
                Dim avepa As New AgroFascicoloBA_AVEPA_BIZ.SchedaValidazione_R
                lRval = avepa.SchedeFascicolo( _
                     EnteValidatore_cod _
                   , cuaa _
                   , DataDa1 _
                   , DataA1 _
                   , "" _
                   , "" _
                   , objParametri_Server _
               )

            Case enum_EnteValidadore.RegioneUmbria_Sigpa
                Dim sigpa As New AgroFascicoloBA_SIGPA_BIZ.SchedaValidazione_R
                lRval = sigpa.SchedeFascicolo( _
                     EnteValidatore_cod _
                   , cuaa _
                   , DataDa1 _
                   , DataA1 _
                   , "" _
                   , "" _
                   , objParametri_Server _
               )


        End Select


        Return lRval

    End Function


#End Region



#Region "Lettura Da Cache"


    Public Function getSchedeFascicolo_CACHE(ByVal EnteValidatore_cod As Integer, ByVal cuaa As String, ByVal DataDa1 As Date, ByVal DataA1 As Date, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of SchedaValidazione)

        Dim lRval As New List(Of SchedaValidazione)
        Dim leggi As New Importazione_Agea_DAL.AgeaAnagrafe_R

        Dim dtFA As DataTable = _
        leggi.SchedeFascicoloCache(EnteValidatore_cod, _
                                   DataDa1, _
                                   DataA1, _
                                   cuaa, _
                                   "", _
                                   "", _
                                   objParametri_Server)

        For Each rFA As DataRow In dtFA.Rows
            lRval.Add(New SchedaValidazione With { _
                       .numeroScheda = rFA("Validazione_Numero"), _
                       .dataScheda = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(rFA("Validazione_Data")), _
                       .dataSchedaSpecified = True _
                   })
        Next

        Return lRval

    End Function

    Public Function getFascicolidaCaricare(ByVal top As Integer, ByVal objParametri_Server As AgronicaCoreParametri, Optional EnteValidatore As Integer = 0, Optional ValidazioneNumero As String = "") As List(Of String)
        Dim lCuaa As New List(Of String)
        Dim leggi As New Importazione_Agea_DAL.AgeaAnagrafe_R

        Dim dtFA As DataTable = leggi.FascicoliDaCaricare(top, objParametri_Server, EnteValidatore, ValidazioneNumero)


        For Each rFA As DataRow In dtFA.Rows
            lCuaa.Add(CStr(rFA.Item("Cuaa")))
        Next

        Return lCuaa
    End Function

    Public Function getFascicolidaCaricare_1(ByVal top As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of String)
        Dim lCuaa As New List(Of String)
        Dim leggi As New Importazione_Agea_DAL.AgeaAnagrafe_R

        Dim dtFA As DataTable = leggi.FascicoliDaCaricare_1(top, objParametri_Server)


        For Each rFA As DataRow In dtFA.Rows
            lCuaa.Add(CStr(rFA.Item("Cuaa")))
        Next

        Return lCuaa
    End Function

    Public Function GetSingoloFascicoloAgea_CACHE(
            ByVal EnteValidatore_COD As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByRef OrderBy As String,
            ByRef parametriExtra As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional Data_Validazione As Date = AGRODATAINIZIO
    ) As String

        'Dim objFascicolo As New String

        Dim LettoreCache As New Importazione_Agea_DAL.AgeaAnagrafe_R
        Dim xml As String = LettoreCache.LeggiCache(
            EnteValidatore_COD,
            cuaa,
            Validazione_Numero,
            "",
            OrderBy,
            parametriExtra,
            objParametri,
            Data_Validazione
        )

        'AgronicaCoreUtility.XMLUtility.getObjectFromXml(xml, objFascicolo)
        Return xml
    End Function

    Public Function GetSingoloFascicoloAgea_CACHE_DT(
            ByVal EnteValidatore_COD As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByRef OrderBy As String,
            ByRef parametriExtra As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional Data_Validazione As Date = AGRODATAINIZIO
    ) As DataTable

        'Dim objFascicolo As New String

        Dim LettoreCache As New Importazione_Agea_DAL.AgeaAnagrafe_R
        Dim DT As DataTable
        DT = LettoreCache.LeggiCacheDT(
            EnteValidatore_COD,
            cuaa,
            Validazione_Numero,
            "",
            OrderBy,
            parametriExtra,
            objParametri,
            Data_Validazione
        )

        'AgronicaCoreUtility.XMLUtility.getObjectFromXml(xml, objFascicolo)
        Return DT
    End Function


    'GLORIA
    Public Function GetCuaaModificata_CACHE_DT(
            ByVal EnteValidatore_COD As Integer,
            ByVal DataVerifica As Date,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        'Dim objFascicolo As New String

        Dim LettoreCache As New Importazione_Agea_DAL.AgeaAnagrafe_R
        Dim DT As DataTable
        DT = LettoreCache.GetCuaaModificato(
            EnteValidatore_COD,
            DataVerifica,
            objParametri
            )

        'AgronicaCoreUtility.XMLUtility.getObjectFromXml(xml, objFascicolo)
        Return DT
    End Function
    'fine GLORIA

#End Region

    Function leggiImportaFascicoli(data As Date, enteValidatore As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT TOP 1 * " & vbCrLf)
            Stb.Append(" FROM ImportaFascicoli " & vbCrLf)
            Stb.Append(" WHERE Data=" + Agro_SQL_SaveDate(data) + " " & vbCrLf)
            Stb.Append(" AND EnteValidatore=" + Agro_SQL_SaveNum(enteValidatore) + " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    Function leggiCuaaImportazioneMassiva(ObjParametri_Server As AgronicaCoreParametri, ByVal top As Integer, ByVal importato As Integer, EnteValidatore_Cod As Integer, Optional ByVal Parametri_Extra As String = "") As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.leggiCuaaImportazioneMassiva()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim cuaas As New List(Of String)
        Try

            Stb.Length = 0
            If top <> 0 Then
                Stb.Append(" SELECT TOP " + CStr(top) + " * " & vbCrLf)
            Else
                Stb.Append(" SELECT * " & vbCrLf)
            End If
            Stb.Append(" FROM __T_FascicoliDaCaricare " & vbCrLf)
            Stb.Append(" WHERE importato = 0 " & vbCrLf)

            If EnteValidatore_Cod <> 0 Then
                Stb.Append(" AND EnteValidatore_Cod= " + Agro_SQL_SaveNum(EnteValidatore_Cod) + " " & vbCrLf)
            End If

            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra= " + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
            End If

            Stb.Append(" AND Ordine <> -1 " & vbCrLf)

            Stb.Append(" ORDER BY Ordine " & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return DT
    End Function


    Function leggiCuaaDaConvertireAGEA(top As Integer, _
                                       EnteValidatore As Integer, _
                                       inviato As Integer, _
                                       ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.leggiCuaaDaConvertireAGEA()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim cuaas As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT top " + Agro_SQL_SaveNum_NULL(top) + " * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(EnteValidatore) + " " & vbCrLf)
            Stb.Append(" AND inviato=" + Agro_SQL_SaveNum_NULL(inviato) + " " & vbCrLf)
            Stb.Append(" AND Fascicolo is not null " & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return DT
    End Function

    Function leggiFascicolo(EnteValidatore As Integer, cuaa As String, ObjParametri_Server As AgronicaCoreParametri) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.legggiFascicolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(EnteValidatore) + " " & vbCrLf)
            Stb.Append(" AND CUAA=" + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                xml = CStr(DT.Rows(0).Item("Fascicolo"))
            End If

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return xml
    End Function

    Function legggiAziendaFascicolo(EnteValidatore As Integer, cuaa As String, validazioneNumero As String, ObjParametri_Server As AgronicaCoreParametri, Optional validazioneData As DateTime = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO) As DataRow
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.legggiFascicolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DR As DataRow
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(EnteValidatore) + " " & vbCrLf)
            Stb.Append(" AND CUAA=" + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)
            If validazioneNumero <> "" Then
                Stb.Append(" AND Validazione_Numero=" + Agro_SQL_SaveText_NULL(validazioneNumero) + " " & vbCrLf)
            End If
            If validazioneData <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                Stb.Append(" AND Validazione_Data=" + Agro_SQL_SaveDateTime_NULL(validazioneData) + " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                DR = DT.Rows(0)
            End If
        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DR
    End Function

    Function LeggiAggiornaFascicoli(ObjParametri_Server As AgronicaCoreParametri,
                                    enteValidatore_cod As Integer,
                                    Optional cuaa As String = "",
                                    Optional dataRiferimento As DateTime = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                    Optional dataRichiesta As DateTime = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                    Optional importato As Integer = -1000,
                                    Optional Parametri_Extra As String = "",
                                    Optional Utenza As Integer = 0) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.LeggiAggiornaFascicoli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM AggiornaFascicoli " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            If enteValidatore_cod <> 0 Then
                Stb.Append(" AND EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(enteValidatore_cod) + " " & vbCrLf)
            End If
            If cuaa <> "" Then
                Stb.Append(" AND CUAA=" + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)
            End If
            If importato <> -1000 Then
                Stb.Append(" AND Importato=" + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
            End If
            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra=" + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
            End If
            If dataRiferimento <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                Stb.Append(" AND DataRiferimento=" + Agro_SQL_SaveDateTime_NULL(dataRiferimento) + " " & vbCrLf)
            End If
            If dataRichiesta <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                Stb.Append(" AND DataRichiesta=" + Agro_SQL_SaveDateTime_NULL(dataRichiesta) + " " & vbCrLf)
            End If
            If Utenza <> 0 Then
                Stb.Append(" AND Utenza=" + Agro_SQL_SaveNum(Utenza) + " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    Function maxDataAggiornaFascicoli(ObjParametri_Server As AgronicaCoreParametri,
                                    enteValidatore_cod As Integer,
                                    Optional Parametri_Extra As String = "",
                                    Optional Utenza As Integer = 0) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.LeggiAggiornaFascicoli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" SELECT max(DataRichiesta) " & vbCrLf)
            Stb.Append(" FROM AggiornaFascicoli " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            If enteValidatore_cod <> 0 Then
                Stb.Append(" AND EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(enteValidatore_cod) + " " & vbCrLf)
            End If
            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra=" + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
            End If
            If Utenza <> 0 Then
                Stb.Append(" AND Utenza=" + Agro_SQL_SaveNum(Utenza) + " " & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    Function leggiPianoColturaleMassivo2016(ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgeaAnagrafe_R.leggiPianoColturaleMassivo2016()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim cuaas As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM __T_PianiColturali2016 " & vbCrLf)
            Stb.Append(" WHERE importato =0 " & vbCrLf)
            Stb.Append(" ORDER BY Ordine " & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return DT
    End Function

    Function GetCuaaModificati(EnteValidatore_COD As Integer, data_Riferimento As Date?, parametri_Extra As String, objParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.legggiFascicolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append("SELECT distinct(cuaa)  " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD = " + Agro_SQL_SaveNum(EnteValidatore_COD) + " " & vbCrLf)
            Stb.Append(" AND Validazione_Data>=" + Agro_SQL_SaveDate(data_Riferimento) + " " & vbCrLf)
            If parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra=" + Agro_SQL_SaveText_NULL(parametri_Extra) + " " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    Function autenticaUtente(objParametri_Server As AgronicaCoreParametri, username As String, password As String) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.legggiFascicolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append("SELECT *  " & vbCrLf)
            Stb.Append(" FROM Utenti u " & vbCrLf)
            Stb.Append(" WHERE Username = " + Agro_SQL_SaveText_NULL(username) + " " & vbCrLf)
            Stb.Append(" AND PASSWORD= " + Agro_SQL_SaveText_NULL(password) + " ")



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    Function Leggi_FascicoliDaImportare_AGREA(CUAA As String, Importato As Integer?, Anno As Integer, ID_Caa As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "Fascicolo_R.Leggi_FascicoliDaImportare_AGREA()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append("SELECT *  " & vbCrLf)
            Stb.Append(" FROM FascicoliDaImportare_AGREA " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            If CUAA <> "" Then
                Stb.Append(" AND CUAA = " & Agro_SQL_SaveText_NULL(CUAA) & " " & vbCrLf)
            End If

            If Importato IsNot Nothing Then
                Stb.Append(" AND Importato = " & Agro_SQL_SaveNum(Importato) & " " & vbCrLf)
            End If

            If Anno <> 0 Then
                Stb.Append(" AND Anno = " & Agro_SQL_SaveNum(Anno) & " " & vbCrLf)
            End If

            If ID_Caa <> 0 Then
                Stb.Append(" AND ID_Caa = " & Agro_SQL_SaveNum(ID_Caa) & " " & vbCrLf)
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT

    End Function

    Function richiestaGiaEffettuata(Cuaa As String,
                                    Parametri_Extra As String,
                                    idCaa As enum_CAA,
                                    dataRichiesta As Date,
                                    day As Integer,
                                    ObjParametri_Server As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Fascicolo_R.richiestaGiaEffettuata()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Dim result As Boolean = False
        Try

            Stb.Length = 0

            Stb.Append("SELECT *  " & vbCrLf)
            Stb.Append(" FROM ErroriFascicoliAGREA " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            If Cuaa <> "" Then
                Stb.Append(" AND CUAA = " & Agro_SQL_SaveText_NULL(Cuaa) & " " & vbCrLf)
            End If

            If Parametri_Extra IsNot Nothing Then
                Stb.Append(" AND Parametri_Extra = " & Agro_SQL_SaveText_NULL(Parametri_Extra) & " " & vbCrLf)
            End If

            If idCaa <> 0 Then
                Stb.Append(" AND idCaa = " & Agro_SQL_SaveNum(idCaa) & " " & vbCrLf)
            End If

            Stb.Append(" AND DataRichiesta >= " & Agro_SQL_SaveDate(dataRichiesta.AddDays(-day)) & " AND DataRichiesta <= " & Agro_SQL_SaveDate(dataRichiesta) & " " & vbCrLf)



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Return True
            End If

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return result

    End Function

    '' INIZIO Gloria
    Function leggiFascicoliDaAggiornareCampiProvicia(EnteValidatore As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.leggiFascicoliDaAggiornareCampiProvicia()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xml As String = ""
        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(EnteValidatore) + " " & vbCrLf)
            Stb.Append(" AND ( Istat_Provincia is null OR Istat_Provincia = '') ")
            Stb.Append(" AND ( Istat_Comune is null  OR Istat_Comune = '') ")
            Stb.Append(" AND ( Stato is null   OR Stato = '') ")
            'Stb.Append(" AND CUAA=" + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'If DT.Rows.Count > 0 Then
            '    xml = CStr(DT.Rows(0).Item("Fascicolo"))
            'End If

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT
    End Function

    ''FINE




End Class
