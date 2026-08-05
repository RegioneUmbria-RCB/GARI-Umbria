
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class FF_Opta_Track
    Inherits AgronicaCoreDataProvider.DataProvider







    Public Function Lotto_Da_Barcode( _
    ByVal Associazione As String, _
    ByVal Barcode As String, _
    ByVal xFiltroAggiuntivo As String, _
    ByVal xOrderBy As String, _
    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
) As DataTable





        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0

            stb.Append(" select  " & vbCrLf)
            stb.Append("    cast((Anno-2000) as varchar(1000)) + '/" & Associazione & "' + '/' + " & vbCrLf)
            stb.Append("    piva + '/' +  " & vbCrLf)
            stb.Append("    right ('00000' + cast(collo as varchar(100)) , 5) " & vbCrLf)
            stb.Append(" as lotto " & vbCrLf)
            stb.Append(" from __rintraccio_ritiro RR " & vbCrLf)
            stb.Append("    inner join imprese_codici ic " & vbCrLf)
            stb.Append("        on RR.CODICE_CUAA = ic.val_cod " & vbCrLf)
            stb.Append("        and ic.id_cod = 1010 " & vbCrLf)
            stb.Append(" where barcode = '" & Agro_SQL_SaveText(Barcode) & "' " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function




    Public Function Barcode_Da_Lotto( _
        ByVal Lotto As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim Associazione As String = ""
            Dim lLottoSplit As String() = Lotto.Split("/")
            If lLottoSplit.Length > 1 Then
                Associazione = lLottoSplit(2)
            End If



            Stb.Length = 0

            Stb.Append(" Select barcode " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" from __rintraccio_ritiro RR " & vbCrLf)
            Stb.Append("    inner join imprese_codici ic " & vbCrLf)
            Stb.Append("        on RR.CODICE_CUAA = ic.val_cod " & vbCrLf)
            Stb.Append("        and ic.id_cod = 1010 " & vbCrLf)
            Stb.Append(" where 'LU/' + cast((Anno-2000) as varchar(1000)) + '/" & Associazione & "' + '/' + " & vbCrLf)
            Stb.Append("    piva + '/' +  " & vbCrLf)
            Stb.Append("    right ('00000' + cast(collo as varchar(100)) , 5) = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
            Stb.Append(" ")


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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



    '##############################################################################################
    Public Function LeggiOPAgendaSuImpiantiDatoBarcode( _
            ByVal Barcode As String, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append("    Select distinct " & vbCrLf)
            Stb.Append("      D.piva " & vbCrLf)
            Stb.Append("    , D.sa_cod   " & vbCrLf)
            Stb.Append("    , D.Id_Agenda    " & vbCrLf)
            Stb.Append("    , A.Lav_Cod " & vbCrLf)
            Stb.Append("    , A.des_lib " & vbCrLf)
            Stb.Append("    , A.Validita_inizio as Data_Movimento " & vbCrLf)
            Stb.Append("    , D.Id_Mov   " & vbCrLf)
            Stb.Append("    , D.Id_Mov_Det " & vbCrLf)
            Stb.Append("    , Det.elem_cod " & vbCrLf)
            Stb.Append("    , Det.pro_cod " & vbCrLf)
            Stb.Append("    , Det.Mat_Cod " & vbCrLf)
            Stb.Append("    , Det.Udm_Cod " & vbCrLf)
            Stb.Append("    , Det.Cal_Cod " & vbCrLf)
            Stb.Append("    , Det.Lotto " & vbCrLf)
            Stb.Append("    , Det.Qta " & vbCrLf)
            Stb.Append("    , 1 as percentuale_contributo " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" from __rintraccio_ritiro RR " & vbCrLf)
            Stb.Append("    inner join __Rintraccio_RitiroXImpiantiRaccolti RRIP " & vbCrLf)
            Stb.Append("        on RR.ID = RRIP.id " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join mov_Destinazioni D " & vbCrLf)
            Stb.Append("        on RRIP.PIVA = D.piva " & vbCrLf)
            Stb.Append("        and RRIP.SA_COD = D.sa_cod " & vbCrLf)
            Stb.Append("        and RRIP.APPEZZA = D.Appezza " & vbCrLf)
            Stb.Append("        and RRIP.ID_REG = D.id_destinazione " & vbCrLf)
            Stb.Append("        and D.tipo_destinazione =  0 " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join Movimenti_dettagli Det " & vbCrLf)
            Stb.Append("        on Det.piva = D.piva  " & vbCrLf)
            Stb.Append("        and Det.Sa_cod = D.sa_cod  " & vbCrLf)
            Stb.Append("        and Det.Id_Mov = D.Id_Mov " & vbCrLf)
            Stb.Append("        and Det.Id_Mov_Det = D.Id_Mov_Det " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join movimenti M " & vbCrLf)
            Stb.Append("        on Det.piva = M.piva  " & vbCrLf)
            Stb.Append("        and Det.Sa_cod = M.sa_cod  " & vbCrLf)
            Stb.Append("        and Det.Id_Mov = M.Id_Mov " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join Agenda A " & vbCrLf)
            Stb.Append("        on A.Id_Agenda = M.Id_Agenda " & vbCrLf)

            Stb.Append(" where barcode = '" & Agro_SQL_SaveText(Barcode) & "' " & vbCrLf)



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class DAL_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


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
            Stb.Append(" INSERT ... " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine, ")
            Stb.Append("              Validazione, Data_Validazione, UserName_Validazione " + vbCrLf)
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")



            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            Stb.Append(") ")

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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

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


