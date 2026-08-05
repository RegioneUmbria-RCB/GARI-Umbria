Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Cespiti_Categorie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                      Optional ByVal IdCodCategoria As Long = 0, _
                      Optional ByVal xFiltroAggiuntivo As String = "", _
                      Optional ByVal xOrderBy As String = "", _
                      Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                      Optional ByRef strSQLOutput As String = "") As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.Cespiti_Categorie_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Stb.Length = 0


            Stb.Append("      SELECT   * " & vbCrLf)
            Stb.Append("      FROM     Cespiti_Categorie " & vbCrLf)
            Stb.Append("      WHERE 1=1 " & vbCrLf)

            '" WHERE    Cespiti_Categorie.Validita_inizio < " & Agro_SQL_SaveDate(AgroSet_DBMS_Tipo, FinestraTemp_Fine) & " " & _
            '" AND      Cespiti_Categorie.Validita_Fine > " & Agro_SQL_SaveDate(AgroSet_DBMS_Tipo, FinestraTemp_Inizio) & " "



            If IdCodCategoria <> 0 Then
                Stb.Append(" AND  Cespiti_Categorie.id_cod_categoria = " & IdCodCategoria & " " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Cespiti_Categorie.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Cespiti_Categorie.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
           
            End If

            If ChiamataDaGiasLan Then
                strSQLOutput = Stb.ToString
            Else
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function
End Class

Public Class Cespiti_Categorie_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                       ByVal IdCodCategoria As Long, _
                       ByVal DesCategoria As String, _
                       Optional ByVal PrcAmmort As Double = 0, _
                       Optional ByVal TipoBene As Integer = 0, _
                       Optional ByVal DimezzaPrimoAnno As Integer = 0, _
                 Optional ByVal FisPrcAmmor As Double = 0, _
                 Optional ByVal FisDimezzaPrimoAnno As Integer = 0, _
                 Optional ByVal FisLimMaxDed As Double = 0,
                 Optional ByVal FisPercDed As Double = 0,
                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                              Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                              Optional ByVal Data_creazione As Date = #2/1/1900#, _
                              Optional ByVal Data_modifica As Date = #2/1/1900#, _
                              Optional ByVal username_creazione As String = "", _
                              Optional ByVal username_modifica As String = "") As Boolean



        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
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
            Stb.Length = 0
            Stb.Append(" INSERT INTO Cespiti_Categorie        ( " & vbCrLf)

            Stb.Append("        id_cod_categoria,             " & vbCrLf)
            Stb.Append("        des_categoria,             " & vbCrLf)
            Stb.Append("        prc_ammor,                    " & vbCrLf)
            Stb.Append("        ind_tipo_bene,                " & vbCrLf)
            Stb.Append("        flg_dimezza_primo_anno,         " & vbCrLf)

            Stb.Append("        fis_prc_ammor,              " & vbCrLf)
            Stb.Append("        fis_flg_dimezza_primo_anno, " & vbCrLf)
            Stb.Append("        fis_lim_max_ded,            " & vbCrLf)
            Stb.Append("        fis_perc_ded,               " & vbCrLf)


            Stb.Append(" Inviato,            DataInvio, " & vbCrLf)
            Stb.Append(" Data_Creazione,     Data_Modifica, " & vbCrLf)
            Stb.Append(" UserName_Creazione, UserName_Modifica, " & vbCrLf)
            Stb.Append(" Validita_Inizio,    Validita_Fine " & vbCrLf)

            Stb.Append(" )" & vbCrLf)


            Stb.Append("  VALUES ( " & vbCrLf)

            Stb.Append("          " & Agro_SQL_SaveNum(IdCodCategoria) & "  " & vbCrLf)
            Stb.Append("         ,'" & Agro_SQL_SaveText(DesCategoria) & "' " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(PrcAmmort) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(TipoBene) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(DimezzaPrimoAnno) & "  " & vbCrLf)

            Stb.Append("         , " & Agro_SQL_SaveNum(FisPrcAmmor) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(FisDimezzaPrimoAnno) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(FisLimMaxDed) & "  " & vbCrLf)
            Stb.Append("         , " & Agro_SQL_SaveNum(FisPercDed) & "  " & vbCrLf)

            Stb.Append("         , 0  ")
            Stb.Append("         , Null  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(" ) " & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function



    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                        ByVal IdCodCategoria As Long, _
                        ByVal DesCategoria As String, _
                        Optional ByVal PrcAmmort As Double = 0, _
                        Optional ByVal TipoBene As Integer = 0, _
                        Optional ByVal DimezzaPrimoAnno As Integer = 0, _
                 Optional ByVal FisPrcAmmor As Double = 0, _
                 Optional ByVal FisDimezzaPrimoAnno As Integer = 0, _
                 Optional ByVal FisLimMaxDed As Double = 0,
                 Optional ByVal FisPercDed As Double = 0,
                         Optional ByVal Validita_Inizio As Date = #2/1/1900#, _
                         Optional ByVal Validita_Fine As Date = #2/1/1900#, _
                         Optional ByVal Data_creazione As Date = #2/1/1900#, _
                         Optional ByVal Data_modifica As Date = #2/1/1900#, _
                         Optional ByVal username_creazione As String = "", _
                         Optional ByVal username_modifica As String = "") As Boolean



        Dim NomeRoutine As String = "Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
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
            Stb.Length = 0

            Stb.Append(" UPDATE Cespiti_Categorie SET " & vbCrLf)
            Stb.Append(" des_categoria = '" & Agro_SQL_SaveText(DesCategoria) & "',  " & vbCrLf)
            Stb.Append(" prc_ammor = " & Agro_SQL_SaveNum(PrcAmmort) & ",  " & vbCrLf)
            Stb.Append(" ind_tipo_bene = " & Agro_SQL_SaveNum(TipoBene) & ",  " & vbCrLf)
            Stb.Append(" flg_dimezza_primo_anno = " & Agro_SQL_SaveNum(DimezzaPrimoAnno) & ",  " & vbCrLf)

            Stb.Append(" fis_prc_ammor = " & Agro_SQL_SaveNum(FisPrcAmmor) & ",  " & vbCrLf)
            Stb.Append(" fis_flg_dimezza_primo_anno = " & Agro_SQL_SaveNum(FisDimezzaPrimoAnno) & ",  " & vbCrLf)
            Stb.Append(" fis_lim_max_ded = " & Agro_SQL_SaveNum(FisLimMaxDed) & ",  " & vbCrLf)
            Stb.Append(" fis_perc_ded = " & Agro_SQL_SaveNum(FisPercDed) & "  " & vbCrLf)

            Stb.Append("   ,Inviato           = 0 ")
            Stb.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & "  ")
            Stb.Append("   ,DataInvio         = Null ")

            Stb.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            Stb.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            Stb.Append("  WHERE id_cod_categoria =  " & Agro_SQL_SaveNum(IdCodCategoria) & "    ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal IdCodCategoria As Long, _
                             Optional ByVal xFiltroAggiuntivo As String = "" _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE Cespiti_Categorie ")
                Stb.Append(" SET ... ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM Cespiti_Categorie ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If IdCodCategoria <> 0 Then
                Stb.Append(" AND id_cod_categoria = " & Agro_SQL_SaveNum(IdCodCategoria) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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