Imports Importazione_Agea_DAL
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Fascicolo_W
    Inherits AgronicaCoreDataProvider.DataProvider2010
    Public Function SalvaInCacheFascicolo(
            ByVal EnteValidatore_Cod As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByVal Validazione_Data As DateTime,
            ByVal Fascicolo As String,
            ByVal Validita_Inizio As DateTime,
            ByVal Validita_Fine As DateTime,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        'Verifica Esistenza..
        Try
            Dim SalvaCache As New AgeaAnagrafe_W
            SalvaCache.ScriviCache(EnteValidatore_Cod, cuaa, Validazione_Numero, Validazione_Data, Fascicolo, Validita_Inizio, Validita_Fine, objParametri)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Function AggiornaInCacheFascicolo(
            ByVal EnteValidatore_Cod As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByVal Validazione_Data As DateTime,
            ByVal Fascicolo As String,
            ByVal Validita_Inizio As DateTime,
            ByVal Validita_Fine As DateTime,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional Parametri_Extra As String = "",
            Optional ByVal Istat_provincia As String = "",
            Optional ByVal Istat_comune As String = "",
            Optional ByVal Stato As String = ""
        ) As Boolean

        'Verifica Esistenza..
        Try
            Dim SalvaCache As New AgeaAnagrafe_W
            SalvaCache.AggiornaCache(EnteValidatore_Cod, cuaa, Validazione_Numero, Validazione_Data, Fascicolo, Validita_Inizio, Validita_Fine, objParametri, , , , , Parametri_Extra, Istat_provincia, Istat_comune, Stato)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Function AggiornaDataValidazione(
            ByVal EnteValidatore_Cod As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByVal Validazione_Data As DateTime,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        'Verifica Esistenza..
        Try
            Dim SalvaCache As New AgeaAnagrafe_W
            SalvaCache.AggiornaDataValidazione(EnteValidatore_Cod, cuaa, Validazione_Numero, Validazione_Data, objParametri)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Function ScriviImportaFascicoli(ByVal data As Date,
                                           ByVal enteValidatore As Integer,
                                           ByVal numeroFascicoliCaricati As Integer,
                                           ByVal numeroErrori As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Try
            Dim ageaAnagrafe As New AgeaAnagrafe_W
            ageaAnagrafe.scriviImportaFascicoli(data, enteValidatore, numeroFascicoliCaricati, numeroErrori, objParametri)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Function scriviFascicoliFormatoComune(ByVal EnteValidatore As Integer,
                                           ByVal CUAA As String,
                                           ByVal Validazione_Numero As String,
                                           ByVal Validazione_Data As DateTime,
                                           ByVal xml As String,
                                           ByVal inviato As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean
        Try
            Dim writer As New AgeaAnagrafe_W
            writer.scriviFascicoloComune(EnteValidatore, CUAA, Validazione_Numero, Validazione_Data, xml, inviato, objParametri)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ''gloria
    Public Function ModificaInCacheFascicoloAllineamento(
            ByVal EnteValidatore_Cod As Integer,
            ByVal cuaa As String,
            ByVal Validazione_Numero As String,
            ByVal Validazione_Data As DateTime,
            ByVal Istat_Provincia As String,
            ByVal Istat_Comune As String,
            ByVal Stato As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
           ) As Boolean

        'Verifica Esistenza..
        Try
            Dim SalvaCache As New AgeaAnagrafe_W
            SalvaCache.ModificaPerAllineamentoFascicoliCache(EnteValidatore_Cod, cuaa, Validazione_Numero, Validazione_Data, Istat_Provincia, Istat_Comune, Stato, objParametri)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function
    ''fine gloria

    Function scriviErrore(Tabella As String, Cuaa As String, importato As Integer, ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Try
            Dim writer As New AgeaAnagrafe_W
            writer.scriviErrore(Tabella, Cuaa, importato, ObjParametri)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Function FascicoloImportato(cuaa As String, importato As Integer, ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.scriviErrore()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE __T_FascicoliDaCaricare " + vbCrLf)
            Stb.Append(" SET importato=" + CStr(importato) + " " + vbCrLf)
            Stb.Append(" WHERE CUAA='" + Agro_SQL_SaveText(cuaa) + "' " + vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Function ScriviAggiornaFascicoli(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     enteValidatore_Cod As Integer,
                                     dataRiferimento As Date,
                                     dataRichiesta As Date,
                                     cuaa As String,
                                     importato As Integer,
                                     Optional Parametri_Extra As String = "",
                                     Optional Utenza As Integer = 0)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.ScriviAggiornaFascicoli()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try

            Dim objFascicoli_R As New Fascicolo_R
            Dim dtFa = objFascicoli_R.LeggiAggiornaFascicoli(ObjParametri, enteValidatore_Cod, cuaa, dataRiferimento, dataRichiesta, importato, Parametri_Extra, Utenza)
            If dtFa.Rows.Count > 0 Then

            Else
                '---------------------------------------------
                Stb.Length = 0
                Stb.Append("INSERT INTO [dbo].[AggiornaFascicoli] " & vbCrLf)
                Stb.Append("            ([EnteValidatore_Cod] " & vbCrLf)
                Stb.Append("            ,[DataRiferimento] " & vbCrLf)
                Stb.Append("            ,[DataRichiesta] " & vbCrLf)
                Stb.Append("            ,[Cuaa] " & vbCrLf)
                Stb.Append("            ,[importato] " & vbCrLf)
                Stb.Append("            ,[Parametri_Extra] " & vbCrLf)
                Stb.Append("            ,[Utenza]) " & vbCrLf)
                Stb.Append("             VALUES " & vbCrLf)
                Stb.Append("            (" + Agro_SQL_SaveNum(enteValidatore_Cod) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(dataRiferimento) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(dataRichiesta) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveNum_NULL(importato) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveNum_NULL(Utenza) + " )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Function UpdateAggiornaFascicoli(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, EnteValidatore_cod As Integer, Cuaa As String, importato As Integer, Optional Parametri_Extra As String = "")
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.ScriviAggiornaFascicoli()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("UPDATE AggiornaFascicoli " & vbCrLf)
            Stb.Append(" SET importato = " + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
            Stb.Append(" WHERE Cuaa=" + Agro_SQL_SaveText_NULL(Cuaa) + " " & vbCrLf)
            Stb.Append(" AND EnteValidatore_Cod = " + Agro_SQL_SaveNum(EnteValidatore_cod) + " ")
            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_extra = " + Agro_SQL_SaveText_NULL(Parametri_Extra) + " ")
            End If



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Function eliminaAggiornaFascicoli(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      EnteValidatore_Cod As Integer,
                                      importato As Integer,
                                      dataMax As DateTime,
                                      Optional Parametri_Extra As String = "",
                                      Optional Utenza As Integer = 0
                                      )
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.eliminaAggiornaFascicoli()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("DELETE FROM AggiornaFascicoli " & vbCrLf)
            Stb.Append(" WHERE importato=" + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
            Stb.Append(" AND EnteValidatore_Cod = " + Agro_SQL_SaveNum(EnteValidatore_Cod) + " ")
            Stb.Append(" AND dataRichiesta <> " + Agro_SQL_SaveDateTime_NULL(dataMax) + " ")
            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra = " + Agro_SQL_SaveText_NULL(Parametri_Extra) + " ")
            End If
            If Utenza <> 0 Then
                Stb.Append(" AND Utenza = " + Agro_SQL_SaveNum(Utenza) + " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Function aggiornaPianoColturale2016Massivo(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, cuaa As String, Importato As Integer)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.scriviErrore()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE __T_PianiColturali2016 " + vbCrLf)
            Stb.Append(" SET importato=" + CStr(Importato) + " " + vbCrLf)
            Stb.Append(" WHERE CUAA='" + Agro_SQL_SaveText(cuaa) + "' " + vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Sub resettaCuaaDaImportare(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, importato As Integer, EnteValidatore_Cod As Integer)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.resettaCuaaDaImportare()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE __T_FascicoliDaCaricare " & vbCrLf)
            Stb.Append(" SET importato = 0 " & vbCrLf)
            Stb.Append(" WHERE 1=1 ")


            If importato <> 0 Then
                Stb.Append(" AND importato = " & CStr(importato) & " ")
            End If

            If EnteValidatore_Cod <> 0 Then
                Stb.Append(" AND EnteValidatore_Cod = " & CStr(EnteValidatore_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub

    Sub InserisciFascicoloDaCaricare(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Cuaa As String, importato As Integer, EnteValidatore_Cod As Integer, Optional ByVal Parametri_Extra As String = "")
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.resettaCuaaDaImportare()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("SELECT *  " & vbCrLf)
            Stb.Append(" FROM __T_FascicoliDaCaricare " & vbCrLf)
            Stb.Append(" WHERE CUAA= " + Agro_SQL_SaveText_NULL(Cuaa) + " ")

            If EnteValidatore_Cod <> 0 Then
                Stb.Append(" AND EnteValidatore_Cod= " + Agro_SQL_SaveNum(EnteValidatore_Cod) + " ")
            End If

            If Parametri_Extra <> "" Then
                Stb.Append(" AND Parametri_Extra= " + Agro_SQL_SaveText_NULL(Parametri_Extra) + " ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                Stb1.Append("INSERT INTO [dbo].[__T_FascicoliDaCaricare] " & vbCrLf)
                Stb1.Append("            ([ordine] " & vbCrLf)
                Stb1.Append("            ,[cuaa] " & vbCrLf)
                Stb1.Append("            ,[importato] " & vbCrLf)
                Stb1.Append("            ,[Parametri_Extra] " & vbCrLf)
                Stb1.Append("            ,[EnteValidatore_Cod]) " & vbCrLf)
                Stb1.Append("                 VALUES " & vbCrLf)
                Stb1.Append("            (0 " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(Cuaa) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(Parametri_Extra) + " ")
                Stb1.Append("            ," + Agro_SQL_SaveNum(EnteValidatore_Cod) + ") " & vbCrLf)

                xRisp = EseguiQuery_Scrittura(ObjParametri, Stb1.ToString, NomeRoutine)

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub

    Sub EliminaFascicoloDaCaricare(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Cuaa As String, EnteValidatore_Cod As Integer, Optional ByVal Parametri_Extra As String = "")
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.resettaCuaaDaImportare()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" DELETE  " & vbCrLf)
            Stb.Append(" FROM __T_FascicoliDaCaricare " & vbCrLf)
            Stb.Append(" WHERE CUAA= " + Agro_SQL_SaveText_NULL(Cuaa) + " ")

            If EnteValidatore_Cod <> 0 Then
                Stb.Append(" AND EnteValidatore_Cod= " + Agro_SQL_SaveNum(EnteValidatore_Cod) + " ")
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub


    Sub InserisciFascicoloDaImportare_AGREA(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Cuaa As String, importato As Integer, Anno As Integer, id_Caa As Integer, CAA_Des As String, DetentoreAGEA As String)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.InserisciFascicoloDaImportare_AGREA()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try

            If dt.Rows.Count = 0 Then
                Stb1.Append("INSERT INTO [dbo].[FascicoliDaImportare_AGREA] " & vbCrLf)
                Stb1.Append("            ([Ordine] " & vbCrLf)
                Stb1.Append("            ,[CUAA] " & vbCrLf)
                Stb1.Append("            ,[importato] " & vbCrLf)
                Stb1.Append("            ,[Anno] " & vbCrLf)
                Stb1.Append("            ,[ID_CAA] " & vbCrLf)
                Stb1.Append("            ,[CAA_Des] " & vbCrLf)
                Stb1.Append("            ,[DetentoreAGEA]) " & vbCrLf)
                Stb1.Append("                 VALUES " & vbCrLf)
                Stb1.Append("            (0 " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(Cuaa) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(Anno) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(id_Caa) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(CAA_Des) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(DetentoreAGEA) + ") " & vbCrLf)

                xRisp = EseguiQuery_Scrittura(ObjParametri, Stb1.ToString, NomeRoutine)

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub

    Sub InserisciAggiornaFascicoloDaImportare_AGREA(ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Cuaa As String, importato As Integer, Anno As Integer, id_Caa As Integer, CAA_Des As String, DetentoreAGEA As String)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.InserisciFascicoloDaImportare_AGREA()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try

            Dim fascicolo_r As New Fascicolo_R
            dt = fascicolo_r.Leggi_FascicoliDaImportare_AGREA(Cuaa, Nothing, Anno, 0, ObjParametri)

            If dt.Rows.Count = 0 Then
                Stb1.Append("INSERT INTO [dbo].[FascicoliDaImportare_AGREA] " & vbCrLf)
                Stb1.Append("            ([Ordine] " & vbCrLf)
                Stb1.Append("            ,[CUAA] " & vbCrLf)
                Stb1.Append("            ,[importato] " & vbCrLf)
                Stb1.Append("            ,[Anno] " & vbCrLf)
                Stb1.Append("            ,[ID_CAA] " & vbCrLf)
                Stb1.Append("            ,[CAA_Des] " & vbCrLf)
                Stb1.Append("            ,[DetentoreAGEA]) " & vbCrLf)
                Stb1.Append("                 VALUES " & vbCrLf)
                Stb1.Append("            (0 " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(Cuaa) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(importato) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(Anno) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveNum(id_Caa) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(CAA_Des) + " " & vbCrLf)
                Stb1.Append("            ," + Agro_SQL_SaveText_NULL(DetentoreAGEA) + ") " & vbCrLf)

                xRisp = EseguiQuery_Scrittura(ObjParametri, Stb1.ToString, NomeRoutine)

            Else

                Stb.AppendLine("UPDATE [dbo].[FascicoliDaImportare_AGREA] ")
                Stb.AppendLine("    SET [ID_CAA] = " + Agro_SQL_SaveNum(id_Caa) + " ")
                Stb.AppendLine("       ,[CAA_Des] = " + Agro_SQL_SaveText_NULL(CAA_Des) + " ")
                Stb.AppendLine("       ,[DetentoreAGEA] = " + Agro_SQL_SaveText_NULL(DetentoreAGEA) + " ")
                Stb.AppendLine("  WHERE CUAA = " + Agro_SQL_SaveText_NULL(Cuaa) + " ")
                Stb.AppendLine("  AND Anno = " + Agro_SQL_SaveNum(Anno) + " ")

                xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub

    Sub InserisciErroreAGREA(Cuaa As String, Parametri_Extra As String, IDCaa As Integer, Data As Date, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.InserisciErroreAGREA()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False

        Try

            Stb.AppendLine(" INSERT INTO [dbo].[ErroriFascicoliAGREA] ")
            Stb.AppendLine("            ([Cuaa] ")
            Stb.AppendLine("            ,[Parametri_Extra] ")
            Stb.AppendLine("            ,[IDCaa] ")
            Stb.AppendLine("            ,[DataRichiesta]) ")
            Stb.AppendLine(" VALUES ")
            Stb.AppendLine("            (" & Agro_SQL_SaveText_NULL(Cuaa) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Parametri_Extra) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(IDCaa) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDate(Data) & " )")


            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)



        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub

End Class
