Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DBCalibrature_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(ByRef objPar As AgronicaCoreDataProvider.AgronicaCoreParametri)
        objParametri = objPar
    End Sub

    Public Function leggiTipoImportatore(ByVal codMacchinaLav As String) As Integer

        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiTipoImportatore()"

        '----- Variabili
        Dim MessaggioErrore As String = ""

        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim result As Integer = 0

        Try

            Stb.Length = 0
            Stb.Append("Select TipoImportatore from Linee_Macchine_Lavorazione where codice = '" + Agro_SQL_SaveText_NULL(codMacchinaLav) + "' ")
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            result = CInt(DT.Rows(0).Item("TipoImportatore"))

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function leggiCalibratura(id As Integer) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibratura(" + CStr(id) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0
            Stb.Append("SELECT " & vbCrLf)
            Stb.Append("    '' As Sel, " & vbCrLf)
            Stb.Append("    ID, " & vbCrLf)
            Stb.Append("    Conferitore_codice, " & vbCrLf)
            Stb.Append("    Conferitore_nome,  " & vbCrLf)
            Stb.Append("    iif(Contatto_GIAS is null,'',con.Rag_Soc) as contattoGIAS, " & vbCrLf)
            Stb.Append("    Data_Inizio, " & vbCrLf)
            Stb.Append("    Data_Fine, " & vbCrLf)
            Stb.Append("    Lotto, " & vbCrLf)
            Stb.Append("    Varieta, " & vbCrLf)
            Stb.Append("    iif(Veg_cod_GIAS is null,'',sv.Veg_des) as SpecieVegetale_GIAS, " & vbCrLf)
            Stb.Append("    iif(Cul_cod_GIAS is null,'',cv.Cul_Des) as Varieta_GIAS, " & vbCrLf)
            Stb.Append("    iif(c.Stato is null,'',wan.[WAnagraficaStati_Des]) as Stato,  " & vbCrLf)
            Stb.Append("    ru.Cod_RisUm as Cod_ContattoGIAS,  " & vbCrLf)
            Stb.Append("    sv.Veg_Cod as Cod_SpecieVegetaleGIAS,  " & vbCrLf)
            Stb.Append("    cv.Cul_Cod as Cod_VarietaGIAS " & vbCrLf)
            Stb.Append(" FROM cbl_Calibrature c   " & vbCrLf)
            Stb.Append(" LEFT JOIN SpecieVegetali sv ON c.Veg_cod_GIAS = sv.Veg_Cod  " & vbCrLf)
            Stb.Append(" LEFT JOIN Cultivar cv ON c.Cul_cod_GIAS = cv.Cul_Cod " & vbCrLf)
            Stb.Append(" LEFT JOIN Risorse_Umane ru ON c.Contatto_GIAS = ru.Cod_RisUm " & vbCrLf)
            Stb.Append(" LEFT JOIN Contatti con ON ru.Cod_Contatto = con.Cod_Contatto " & vbCrLf)
            Stb.Append(" LEFT JOIN WAnagraficaStati wan ON c.stato = wan.[WAnagraficaStati_Cod] " & vbCrLf)
            Stb.Append(" WHERE c.ID=" + Agro_SQL_SaveNum_NULL(id) + " ")

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

    Public Function leggiCalibrature() As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibrature()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append("SELECT " & vbCrLf)
            Stb.Append("    '' As Sel, " & vbCrLf)
            Stb.Append("    ID, " & vbCrLf)
            Stb.Append("    Conferitore_codice, " & vbCrLf)
            Stb.Append("    Conferitore_nome,  " & vbCrLf)
            Stb.Append("    iif(Contatto_GIAS is null,'',con.Rag_Soc) as SpecieVegetale_GIAS, " & vbCrLf)
            Stb.Append("    Data_Inizio, " & vbCrLf)
            Stb.Append("    Data_Fine, " & vbCrLf)
            Stb.Append("    Lotto, " & vbCrLf)
            Stb.Append("    Varieta, " & vbCrLf)
            Stb.Append("    iif(Veg_cod_GIAS is null,'',sv.Veg_des) as SpecieVegetale_GIAS, " & vbCrLf)
            Stb.Append("    iif(Cul_cod_GIAS is null,'',cv.Cul_Des) as Varieta_GIAS, " & vbCrLf)
            Stb.Append("    iif(c.Stato is null,'',wan.[WAnagraficaStati_Des]) as Stato,  " & vbCrLf)
            Stb.Append("    ru.Cod_RisUm as Cod_ContattoGIAS,  " & vbCrLf)
            Stb.Append("    sv.Veg_Cod as Cod_SpecieVegetaleGIAS,  " & vbCrLf)
            Stb.Append("    cv.Cul_Cod as Cod_VarietaGIAS, " & vbCrLf)
            Stb.Append("    c.stato as Cod_Stato " & vbCrLf)
            Stb.Append(" FROM cbl_Calibrature c " & vbCrLf)
            Stb.Append(" LEFT JOIN SpecieVegetali sv ON c.Veg_cod_GIAS = sv.Veg_Cod  " & vbCrLf)
            Stb.Append(" LEFT JOIN Cultivar cv ON c.Cul_cod_GIAS = cv.Cul_Cod " & vbCrLf)
            Stb.Append(" LEFT JOIN Risorse_Umane ru ON c.Contatto_GIAS = ru.Cod_RisUm " & vbCrLf)
            Stb.Append(" LEFT JOIN Contatti con ON ru.Cod_Contatto = con.Cod_Contatto " & vbCrLf)
            Stb.Append(" LEFT JOIN WAnagraficaStati wan ON c.stato = wan.[WAnagraficaStati_Cod] ")

            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function leggiCalibriXCalibratura(id As Integer) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibriXCalibrature(" + CStr(id) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0

            Stb.Append("SELECT IDCalibro, " & vbCrLf)
            Stb.Append("    Nome, " & vbCrLf)
            Stb.Append("    IIF(Calibro_par_cod_GIAS is null,'',tp.Descrizione) AS Calibro_GIAS, " & vbCrLf)
            Stb.Append("    Numero, " & vbCrLf)
            Stb.Append("    Peso, " & vbCrLf)
            Stb.Append("    tp.Tabella_Par_Cod as Cod_CalibroGIAS " & vbCrLf)
            Stb.Append(" FROM [dbo].[cbl_CalibratureXCalibri] c " & vbCrLf)
            Stb.Append(" LEFT JOIN OTabelle_Parametri tp ON c.Calibro_par_cod_GIAS = tp.Tabella_Par_Cod AND tp.Tabella_Cod=1 " & vbCrLf)
            Stb.Append(" WHERE IDCalibro = " + Agro_SQL_SaveNum_NULL(id) + " ")

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

    Public Function leggiFileImportati() As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibrature()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaFile As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" Select nomeFile FROM cbl_LogImportazioni" & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaFile.Add(CStr(row.Item(0)))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaFile
    End Function

    Public Function leggiCalibriDataSpecie(ByRef veg_cod As Integer) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibriDataSpecie(" + CStr(veg_cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaCalibri As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append(" SELECT *  " & vbCrLf)
            Stb.Append(" FROM OTabelle_Parametri  " & vbCrLf)
            Stb.Append(" WHERE Tabella_Cod=1 " & vbCrLf)
            Stb.Append(" AND (OFiltro_Veg_Cod like '%0%' OR OFiltro_Veg_Cod like '% " + CStr(veg_cod) + ",%' )" & vbCrLf)
            Stb.Append(" ORDER BY Tabella_Par_Cod")

                '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaCalibri.Add(CStr(row.Item("Tabella_Par_Cod")) + " | " + CStr(row.Item("Descrizione")))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaCalibri
    End Function

    Public Function leggiSpecie( _
                                Optional ByRef Veg_Cod As Nullable(Of Integer) = Nothing) As List(Of String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiSpecie()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaSpecie As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append("SELECT Veg_Cod, Veg_Des  " & vbCrLf)
            Stb.Append(" FROM SpecieVegetali  " & vbCrLf)
            If Veg_Cod IsNot Nothing Then
                Stb.Append(" WHERE veg_cod=" + Agro_SQL_SaveNum_NULL(Veg_Cod) + " " & vbCrLf)
            End If
            Stb.Append(" ORDER BY veg_Cod")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaSpecie.Add(CStr(row.Item("Veg_Cod")) + " | " + CStr(row.Item("Veg_Des")))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaSpecie
    End Function

    Public Function leggiVarieta(ByVal veg_cod As Integer, _
                                 Optional cul_cod As Nullable(Of Integer) = Nothing) As List(Of String)
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiVarieta(" + CStr(veg_cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listaVarieta As New List(Of String)
        Try

            Stb.Length = 0

            Stb.Append("SELECT Cul_Cod, Cul_Des" & vbCrLf)
            Stb.Append(" FROM Cultivar " & vbCrLf)
            Stb.Append(" WHERE Veg_Cod=" + Agro_SQL_SaveNum(veg_cod) + " ")
            If cul_cod IsNot Nothing Then
                Stb.Append(" AND Cul_Cod=" + Agro_SQL_SaveNum(cul_cod) + " ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                listaVarieta.Add(CStr(row.Item("cul_cod")) + " | " + CStr(row.Item("cul_Des")))
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaVarieta
    End Function

    Public Function leggiSpecieEVarietaDataStringa(ByRef varieta As String) As List(Of String)
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiSpecieEVarietaDataStringa(" + varieta + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim specVar As New List(Of String)
        Try

            Stb.Length = 0


            Stb.Append("select top 1 * from cbl_transcodificaSpecieXCultivar Where varietaCal = " + Agro_SQL_SaveText_NULL(varieta) + " ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                specVar.Add(leggiSpecie(CInt(DT.Rows(0).Item("Veg_Cod")))(0))
                specVar.Add(leggiVarieta(CInt(DT.Rows(0).Item("Veg_Cod")), CInt(DT.Rows(0).Item("Cul_Cod")))(0))
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return specVar
    End Function

    Public Function leggiCalibroDataStreVegCod(ByRef calibro As String, ByRef veg_cod As Integer) As String
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiCalibroDataStreVegCod(" + calibro + ", " + CStr(veg_cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim specVar As String = Nothing
        Try

            Stb.Length = 0


            Stb.Append("select top 1 * from cbl_transcodificaCalibri Where DescrizioneCbl = " + Agro_SQL_SaveText_NULL(calibro) + " AND Veg_Cod=" + Agro_SQL_SaveNum_NULL(veg_cod) + " ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                specVar = CStr(DT.Rows(0).Item("Tabella_Par_Cod")) + " | " + CStr(DT.Rows(0).Item("DescrizioneGIAS"))
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return specVar
    End Function

    Function leggiContattoDataStr(contattoCalibratura As String) As Object
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggiContattoDataStr(" + contattoCalibratura + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim contatto As String = Nothing
        Try

            Stb.Length = 0


            Stb.Append("SELECT iif(c.cod_contatto is null, '', concat(ru.cod_risUm, ' | ',c.Rag_soc))  as Conferitore " & vbCrLf  ) 
            Stb.Append(" FROM cbl_transcodificaConferitori tc " & vbCrLf)
            Stb.Append(" INNER JOIN [dbo].[Risorse_Umane] ru ON tc.cod_contatto=ru.Cod_RisUm  " & vbCrLf)
            Stb.Append(" INNER JOIN [dbo].[Contatti] c ON ru.cod_contatto=c.cod_contatto " & vbCrLf)
            Stb.Append(" WHERE tc.conferitoreCal = " + Agro_SQL_SaveText_NULL(contattoCalibratura) + "  ")


                '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                contatto = CStr(DT.Rows(0).Item(0))
            Else
                Return Nothing
                End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return contatto
    End Function

    Function getStatoCalibratura(id As Integer) As Integer
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.getStatoCalibratura(" + CStr(id) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim stato As Integer = 0
        Try

            Stb.Length = 0


            Stb.Append("SELECT Stato " & vbCrLf)
            Stb.Append("FROM cbl_calibrature c " & vbCrLf)
            Stb.Append("WHERE c.ID = " + Agro_SQL_SaveNum_NULL(id) + "  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                stato = CInt(DT.Rows(0).Item(0))
            Else
                Return 0
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return stato
    End Function

    Function getTranscodificaConferitore(nomeConferitore As String) As Integer
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.getTranscodificaConferitore(" + nomeConferitore + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim contatto As Integer = 0
        Try

            Stb.Length = 0


            Stb.Append("SELECT cod_contatto " & vbCrLf)
            Stb.Append("FROM cbl_transcodificaConferitori " & vbCrLf)
            Stb.Append("WHERE conferitoreCal = " + Agro_SQL_SaveText_NULL(nomeConferitore) + "  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                contatto = CInt(DT.Rows(0).Item(0))
            Else
                Return 0
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return contatto
    End Function

    Function getTranscodificaVarieta(nomeSpecieVar As String) As List(Of Integer)
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.getTranscodificaVarieta(" + nomeSpecieVar + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim codici As New List(Of Integer)
        Try

            Stb.Length = 0


            Stb.Append("SELECT * " & vbCrLf)
            Stb.Append("FROM cbl_transcodificaSpecieXCultivar " & vbCrLf)
            Stb.Append("WHERE varietaCal = " + Agro_SQL_SaveText_NULL(nomeSpecieVar) + "  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                If Not IsDBNull(DT.Rows(0).Item("Veg_cod")) Then
                    codici.Add(CInt(DT.Rows(0).Item("Veg_cod")))
                Else
                    codici.Add(0)
                End If

                If Not IsDBNull(DT.Rows(0).Item("Cul_Cod")) Then
                    codici.Add(CInt(DT.Rows(0).Item("Cul_Cod")))
                Else
                    codici.Add(0)
                End If

            Else
                codici.Add(0)
                codici.Add(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codici
    End Function

    Function getTranscodificaCalibro(nomeCalibro As String) As Integer
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.getTranscodificaCalibro(" + nomeCalibro + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim par_cod As Integer = 0
        Try

            Stb.Length = 0


            Stb.Append("SELECT Tabella_Par_Cod " & vbCrLf)
            Stb.Append("FROM cbl_transcodificaCalibri " & vbCrLf)
            Stb.Append("WHERE DescrizioneCbl = " + Agro_SQL_SaveText_NULL(nomeCalibro) + "  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT.Rows.Count > 0 Then
                par_cod = CInt(DT.Rows(0).Item(0))
            Else
                Return 0
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return par_cod
    End Function

    Public Function leggi_ID_da_filename(ByVal filename As String) As Integer
        Dim id As Integer = 0

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggi_ID_da_filename()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim log_filename As String
        Dim log_id As Integer

        Try

            Stb.Length = 0

            Stb.Append(" Select idCalibratura, nomeFile FROM cbl_LogImportazioni" & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For Each row As DataRow In DT.Rows
                log_filename = CStr(row.Item(1))
                log_id = CInt(row.Item(0))
                If (String.Compare(filename, Right(log_filename, filename.Length), True) = 0) Then
                    id = log_id
                    Exit For
                End If
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return id
    End Function

    Public Function leggi_Lotto(ByVal id As Integer) As String
        Dim lotto As String = String.Empty

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.leggi_Lotto(" + CStr(id) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.Length = 0
            Stb.Append("SELECT Lotto FROM cbl_Calibrature WHERE id = " + Agro_SQL_SaveNum_NULL(id))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If (DT.Rows.Count > 0) Then
                If (IsDBNull(DT.Rows(0).Item(0))) Then
                    lotto = ""
                Else
                    lotto = CStr(DT.Rows(0).Item(0))
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return lotto
    End Function

End Class

Public Class DBCalibrature_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(ByRef objPar As AgronicaCoreDataProvider.AgronicaCoreParametri)
        objParametri = objPar
    End Sub

    Private Class SQL_Insert
        Private StbFields As System.Text.StringBuilder
        Private StbVals As System.Text.StringBuilder
        Private Cnt As Integer

        Public ReadOnly Property sql As String
            Get
                Dim StbSQL As New System.Text.StringBuilder
                StbSQL.Length = 0
                StbSQL.Append(StbFields.ToString + ")")
                StbSQL.Append(StbVals.ToString + ")")
                Return StbSQL.ToString
            End Get
        End Property

        Public Sub New(ByVal tablename As String)
            StbFields = New System.Text.StringBuilder
            StbVals = New System.Text.StringBuilder
            StbFields.Length = 0
            StbVals.Length = 0
            StbFields.Append("INSERT INTO " + tablename + " (")
            StbVals.Append(" VALUES (")
            Cnt = 0
        End Sub
        Public Sub setField(ByVal field As String, ByVal val As String)
            If Cnt > 0 Then
                StbFields.Append(", ")
                StbVals.Append(", ")
            End If
            StbFields.Append(field)
            StbVals.Append(val)
            Cnt += 1
        End Sub
        Public Sub setStringField(ByVal field As String, ByVal val As String)
            If Not String.IsNullOrEmpty(val) Then
                setField(field, AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText_NULL(val))
            End If
        End Sub
        Public Sub setIntegerField(ByVal field As String, ByVal val As Nullable(Of Integer))
            If val.HasValue Then
                setField(field, Agro_SQL_SaveNum_NULL(val.Value))
            End If
        End Sub
        Public Sub setDecimalField(ByVal field As String, ByVal val As Nullable(Of Decimal))
            If val.HasValue Then
                setField(field, Agro_SQL_SaveNum_NULL(val.Value))
            End If
        End Sub
        Public Sub setDateField(ByVal field As String, ByVal val As Nullable(Of DateTime))
            If val IsNot Nothing And val.HasValue Then
                setField(field, AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDateTime(val.Value))
            End If
        End Sub
    End Class

    Private Function nuovoId(ByVal tablename As String) As Integer

        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.nuovoId()"

        '----- Variabili
        Dim MessaggioErrore As String = ""

        Dim StbCount As New System.Text.StringBuilder
        Dim DTCount As DataTable

        Dim StbId As New System.Text.StringBuilder
        Dim DTId As DataTable

        Dim id As Integer = 1

        Try

            StbCount.Length = 0
            StbCount.Append("Select count(*) as totale from ")
            StbCount.Append(tablename)
            DTCount = EseguiQuery_Lettura(objParametri, StbCount.ToString, NomeRoutine)

            Dim totale As Integer = CInt(DTCount.Rows(0).Item("totale"))

            If totale > 0 Then
                StbId.Length = 0
                StbId.Append(" Select Max(id) as maxID from ")
                StbId.Append(tablename)
                DTId = EseguiQuery_Lettura(objParametri, StbId.ToString, NomeRoutine)
                id = CInt(DTId.Rows(0).Item("maxID")) + 1
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DTCount = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return id

    End Function

    Public Function InserisciCalibratura(ByVal conferitoreCodice As String,
                                      ByVal conferitoreNome As String,
                                      dataInizio As Nullable(Of DateTime),
                                      dataFine As Nullable(Of DateTime),
                                      lotto As String,
                                      varieta As String,
                                      programma As String,
                                      nrBolla As String,
                                      rifBolla As String,
                                      pesoTot As Nullable(Of Decimal),
                                      numeroTot As Nullable(Of Integer),
                                      durata As Nullable(Of Integer),
                                      scarti As Nullable(Of Decimal),
                                      note As String,
                                      codMacchinaLav As String,
                                      stato As Integer) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.InserisciCalibratura()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As SQL_Insert
        Dim newID As Integer = 0

        Try

            newID = nuovoId("cbl_Calibrature")

            sql = New SQL_Insert("cbl_Calibrature")
            sql.setField("id", Agro_SQL_SaveNum_NULL(newID))
            sql.setStringField("conferitore_codice", conferitoreCodice)
            sql.setStringField("conferitore_nome", conferitoreNome)
            sql.setDateField("Data_Inizio", dataInizio)
            sql.setDateField("Data_Fine", dataFine)
            sql.setStringField("Lotto", lotto)
            sql.setStringField("Varieta", varieta)
            sql.setStringField("Programma", programma)
            sql.setStringField("NrBolla", nrBolla)
            sql.setStringField("RifBolla", rifBolla)
            sql.setIntegerField("NumeroTot", numeroTot)
            sql.setDecimalField("PesoTot", pesoTot)
            sql.setIntegerField("Durata", durata)
            sql.setDecimalField("Scarti", scarti)
            sql.setStringField("Note", note)
            sql.setField("Cod_Macchina_Lav", Agro_SQL_SaveText_NULL(codMacchinaLav))
            sql.setField("Stato", Agro_SQL_SaveNum_NULL(stato))

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, sql.sql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return newID

    End Function

    Public Function InserisciCalibriXCalibrature(ByVal idCalibratura As Integer, _
                                              ByVal quality As String, _
                                              ByVal nome As String, _
                                              ByVal peso As Decimal, _
                                              perc As Nullable(Of Decimal), _
                                              num As Nullable(Of Integer)) As Boolean
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.InserisciCalibriXCalibrature()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As SQL_Insert

        Try

            Dim newID As Integer = nuovoId("cbl_CalibratureXCalibri")

            sql = New SQL_Insert("cbl_CalibratureXCalibri")
            sql.setField("id", Agro_SQL_SaveNum_NULL(newID))
            sql.setField("idCalibro", Agro_SQL_SaveNum_NULL(idCalibratura))
            sql.setStringField("Qualita", quality)
            sql.setField("Nome", Agro_SQL_SaveText_NULL(nome))
            sql.setField("Peso", Agro_SQL_SaveNum_NULL(peso))
            sql.setDecimalField("Perc", perc)
            sql.setIntegerField("Num", num)

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, sql.sql, NomeRoutine)
            '--------------------------------------------------------------------------

            newID += 1

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Sub inserisciLogImportazione(idCalibratura As Integer, data As Nullable(Of DateTime), nomeFile As String)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.inserisciLogImportazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As SQL_Insert

        Try

            Dim newID As Integer = nuovoId("cbl_LogImportazioni")

            Dim dataLog As DateTime = TimeOfDay
            If data IsNot Nothing And data.HasValue Then
                dataLog = data.Value
            End If

            sql = New SQL_Insert("cbl_LogImportazioni")
            sql.setField("id", Agro_SQL_SaveNum_NULL(newID))
            sql.setField("idCalibratura", Agro_SQL_SaveNum_NULL(idCalibratura))
            sql.setField("Data", Agro_SQL_SaveDateTime(dataLog))
            sql.setField("nomeFile", Agro_SQL_SaveText_NULL(nomeFile))

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, sql.sql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Function inseriscriTranscodificaSpecieXCultivar(ByRef varietaCal As String, veg_cod As Integer, Cul_Cod As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.inseriscriTranscodificaSpecieXCultivar(" + varietaCal + ", " + CStr(veg_cod) + ", " + CStr(Cul_Cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DTLeggi As DataTable
        Try
            Stb.Length = 0

            Stb.Append(" SELECT *  " & vbCrLf)
            Stb.Append(" FROM [dbo].[cbl_transcodificaSpecieXCultivar]  " & vbCrLf)
            Stb.Append(" WHERE varietaCal=" + Agro_SQL_SaveText_NULL(varietaCal) + " ")

            '--------------------------------------------------------------------------
            DTLeggi = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0

            If DTLeggi.Rows.Count > 0 Then
                Stb1.Append(" UPDATE [dbo].[cbl_transcodificaSpecieXCultivar] " & vbCrLf)
                Stb1.Append(" SET Veg_cod=" + Agro_SQL_SaveNum_NULL(veg_cod) + ", Cul_Cod=" + Agro_SQL_SaveNum_NULL(Cul_Cod) + "  " & vbCrLf)
                Stb1.Append(" WHERE varietaCal=" + Agro_SQL_SaveText_NULL(varietaCal) + " ")
            Else
                Stb1.Append("INSERT INTO [dbo].[cbl_transcodificaSpecieXCultivar]  " & vbCrLf)
                Stb1.Append(" (VarietaCal, Veg_Cod, Cul_Cod) " & vbCrLf)
                Stb1.Append(" VALUES (" + Agro_SQL_SaveText_NULL(varietaCal) + ", " + Agro_SQL_SaveNum_NULL(veg_cod) + ", " + Agro_SQL_SaveNum_NULL(Cul_Cod) + ")")
            End If

            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function inserisciTranscodificaCalibri(ByRef calibroCal As String, tabella_par_cod As Integer, veg_cod As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.inseriscriTranscodificaCalibri(" + calibroCal + ", " + CStr(tabella_par_cod) + ", " + CStr(veg_cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DTLeggi As DataTable
        Try
            Stb.Length = 0

            Stb.Append(" SELECT *  " & vbCrLf)
            Stb.Append(" FROM cbl_transcodificaCalibri  " & vbCrLf)
            Stb.Append(" WHERE DescrizioneCbl=" + Agro_SQL_SaveText_NULL(calibroCal) + " ")

            '--------------------------------------------------------------------------
            DTLeggi = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0

            Dim calibroGIAS As String
            If DTLeggi.Rows.Count > 0 Then
                calibroGIAS = getCalibroGIAS(tabella_par_cod)
                If calibroGIAS IsNot Nothing Then
                    Stb1.Append(" UPDATE cbl_transcodificaCalibri " & vbCrLf)
                    Stb1.Append(" SET Tabella_Par_Cod=" + Agro_SQL_SaveNum_NULL(tabella_par_cod) + ", Veg_Cod=" + Agro_SQL_SaveNum_NULL(veg_cod) + ", DescrizioneGIAS=" + Agro_SQL_SaveText_NULL(calibroGIAS) + " " & vbCrLf)
                    Stb1.Append(" WHERE DescrizioneCbl=" + Agro_SQL_SaveText_NULL(calibroCal) + " ")
                End If
            Else
                calibroGIAS = getCalibroGIAS(tabella_par_cod)
                If calibroGIAS IsNot Nothing Then
                    Stb1.Append("INSERT INTO cbl_transcodificaCalibri  " & vbCrLf)
                    Stb1.Append(" (DescrizioneCbl, DescrizioneGIAS, Tabella_par_cod, Veg_Cod) " & vbCrLf)
                    Stb1.Append(" VALUES (" + Agro_SQL_SaveText_NULL(calibroCal) + " , " + Agro_SQL_SaveText_NULL(calibroGIAS) + " , " + Agro_SQL_SaveNum_NULL(tabella_par_cod) + ", " + Agro_SQL_SaveNum_NULL(veg_cod) + ")")
                End If
            End If

            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function inserisciTranscodificaContatto(contatto As String, contattoCod As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.inserisciTranscodificaContatto(" + contatto + ", " + CStr(contattoCod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim DTLeggi As DataTable
        Try
            Stb.Length = 0

            Stb.Append(" SELECT *  " & vbCrLf)
            Stb.Append(" FROM [dbo].[cbl_transcodificaConferitori]  " & vbCrLf)
            Stb.Append(" WHERE conferitoreCal =" + Agro_SQL_SaveText_NULL(contatto) + " ")

            '--------------------------------------------------------------------------
            DTLeggi = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb1.Length = 0

            If DTLeggi.Rows.Count > 0 Then
                Stb1.Append(" UPDATE [dbo].[cbl_transcodificaConferitori] " & vbCrLf)
                Stb1.Append("SET cod_Contatto=" + Agro_SQL_SaveNum_NULL(contattoCod) + " " & vbCrLf)
                Stb1.Append("WHERE conferitoreCal =" + Agro_SQL_SaveText_NULL(contatto) + " ")
            Else
                Stb1.Append("INSERT INTO [dbo].[cbl_transcodificaConferitori]  " & vbCrLf)
                Stb1.Append(" (conferitoreCal, cod_Contatto) " & vbCrLf)
                Stb1.Append(" VALUES (" + Agro_SQL_SaveText_NULL(contatto) + ", " + Agro_SQL_SaveNum_NULL(contattoCod) + ")")
            End If
            EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Private Function getCalibroGIAS(tabella_par_cod As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.getCalibroGIAS()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim calibroNome As String = Nothing
        Try

            Stb.Length = 0

            Stb.Append(" SELECT *  " & vbCrLf)
            Stb.Append(" FROM OTabelle_Parametri " & vbCrLf)
            Stb.Append(" WHERE Tabella_Cod=1 " & vbCrLf)
            Stb.Append(" AND Tabella_Par_Cod=" + Agro_SQL_SaveNum_NULL(tabella_par_cod) + "  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                calibroNome = CStr(DT.Rows(0).Item("Descrizione"))
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return calibroNome
    End Function

    Public Function SalvaSpecieVarietaPerCalibratura(idCal As Integer, specie As Integer, varieta As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.SalvaSpecieVarietaPerCalibratura(" + CStr(idCal) + ", " + CStr(specie) + ", " + CStr(varieta) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_Calibrature] " & vbCrLf)
            Stb.Append("SET Veg_cod_GIAS=" + Agro_SQL_SaveNum_NULL(specie) + ", Cul_cod_GIAS =" + Agro_SQL_SaveNum_NULL(varieta) + " " & vbCrLf)
            Stb.Append("WHERE ID = " + Agro_SQL_SaveNum_NULL(idCal))

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function SalvaConferitorePerCalibratura(idCal As Integer, contatto As Integer) As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.SalvaSpecieVarietaPerCalibratura(" + CStr(idCal) + ", " + CStr(contatto) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_Calibrature] " & vbCrLf)
            Stb.Append("SET Contatto_GIAS=" + Agro_SQL_SaveNum_NULL(contatto) + " " & vbCrLf)
            Stb.Append("WHERE ID = " + Agro_SQL_SaveNum_NULL(idCal))

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function SalvaCalibroPerCalibroGIAS(idCal As Integer, nome As String, par_cod_GIAS As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.SalvaCalibroPerCalibroGIAS(" + CStr(idCal) + ", " + CStr(nome) + ", " + CStr(par_cod_GIAS) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_CalibratureXCalibri] " & vbCrLf)
            Stb.Append(" SET Calibro_par_cod_GIAS =" + Agro_SQL_SaveNum_NULL(par_cod_GIAS) + " " & vbCrLf)
            Stb.Append(" WHERE IDCalibro=" + Agro_SQL_SaveNum_NULL(idCal) + " AND Nome=" + Agro_SQL_SaveText_NULL(nome) + "")

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function aggiornaStatoCalibratura(id As Integer, statoImportazione As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.aggiornaStatoCalibratura(" + CStr(id) + ", " + CStr(statoImportazione) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_Calibrature] " & vbCrLf)
            Stb.Append(" SET stato =" + Agro_SQL_SaveNum_NULL(statoImportazione) + " " & vbCrLf)
            Stb.Append(" WHERE ID=" + Agro_SQL_SaveNum_NULL(id) + " ")

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function scriviCodContatto(id As Integer, idContatto As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.scriviCodContatto(" + CStr(id) + ", " + CStr(idContatto) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_Calibrature] " & vbCrLf)
            Stb.Append(" SET contatto_GIAS =" + Agro_SQL_SaveNum_NULL(idContatto) + " " & vbCrLf)
            Stb.Append(" WHERE ID=" + Agro_SQL_SaveNum_NULL(id) + " ")

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function scriviSpecieVarieta(id As Integer, specie As Integer, varieta As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.scriviSpecieVarieta(" + CStr(id) + ", " + CStr(specie) + ", " + CStr(varieta) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_Calibrature] " & vbCrLf)
            Stb.Append(" SET veg_cod_GIAS =" + Agro_SQL_SaveNum_NULL(specie) + ", cul_cod_GIAS=" + Agro_SQL_SaveNum_NULL(varieta) + " " & vbCrLf)
            Stb.Append(" WHERE ID=" + Agro_SQL_SaveNum_NULL(id) + " ")

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Function scriviCalibroParCod(id As Integer, nomeCalibro As String, calibro_par_cod As Integer)
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.scriviCalibroParCod(" + CStr(id) + ", " + nomeCalibro + ", " + CStr(calibro_par_cod) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("UPDATE [dbo].[cbl_CalibratureXCalibri] " & vbCrLf)
            Stb.Append(" SET Calibro_par_cod_GIAS =" + Agro_SQL_SaveNum_NULL(calibro_par_cod) + " " & vbCrLf)
            Stb.Append(" WHERE IDCalibro=" + Agro_SQL_SaveNum_NULL(id) + " AND Nome=" + Agro_SQL_SaveText_NULL(nomeCalibro) + " ")

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return True
    End Function

    Public Sub elimina(ByVal id As Integer)

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreWinsort_DAL.elimina(" + CStr(id) + ")"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Try
            Stb.Length = 0

            Stb.Append("DELETE FROM cbl_LogImportazioni WHERE IDCalibratura = " & Agro_SQL_SaveNum_NULL(id) & " " & vbCrLf)
            Stb.Append("DELETE FROM cbl_CalibratureXCalibri WHERE IDCalibro = " & Agro_SQL_SaveNum_NULL(id) & " " & vbCrLf)
            Stb.Append("DELETE FROM cbl_Calibrature WHERE ID = " & Agro_SQL_SaveNum_NULL(id) & " " & vbCrLf)

            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

End Class