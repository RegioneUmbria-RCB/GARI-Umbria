Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports Agronica.Helpers.Graphs
Imports System.Web.UI.WebControls

Public Class Fitofarmaci

    Inherits AgronicaCoreDataProvider.DataProvider



    '################################################################################
    Public Function LeggiXClassificazione_PA_Dosi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal TestoRicerca As String = "",
                                                Optional ByVal TipoRichiesto As Integer = 0,
                                                Optional ByVal Veg_Cod As Integer = 0,
                                                Optional ByVal strPA As String = "",
                                                Optional ByVal Data As Date = #12/31/2100#,
                                                Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                                                Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                                                Optional ByVal Av_Gru() As Integer = Nothing,
                                                Optional ByVal Av_Cod() As Integer = Nothing,
                                                Optional ByVal strFiltro As String = "",
                                                Optional ByVal strSort As String = "",
                                                Optional ByVal Grfi_cod As Integer = 0,
                                                Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                                    Optional ByVal Stato_Cod As String = "IT"
                                        ) _
                                        As DataTable


        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi()"

        Dim MessaggioErrore As String = ""
        Dim i, j, x As Integer

        Dim Dt_Formulati As DataTable
        Dim Dt_FormulatiTmp As DataTable
        Dim FrCod(0) As String
        Dim FrCod1() As String
        Dim FrCod2() As String
        Dim FormulatoPresente As Boolean

        Try


            Select Case Opt_Singola_Gruppo

                Case 0

                    If Not Av_Cod Is Nothing Then

                        For i = 0 To UBound(Av_Cod)

                            Dt_FormulatiTmp = LeggiXClassificazione_PA_Dosi_Avversita(objParametri,
                                                                              TestoRicerca,
                                                                              TipoRichiesto,
                                                                              Veg_Cod,
                                                                              strPA,
                                                                              Data,
                                                                              Opt_Avversita_Infestanti,
                                                                              Opt_Singola_Gruppo,
                                                                              0,
                                                                              Av_Cod(i),
                                                                              strFiltro,
                                                                              strSort,
                                                                              Grfi_cod,
                                                                              FlagVisualizza_Commercio_Tutti_Revocati,
                                                                                    Stato_Cod)

                            If i = 0 Then
                                Dt_Formulati = Dt_FormulatiTmp.Copy
                            Else
                                Dim objUtility As New AgronicaCoreUtility.DatatableUtility
                                FrCod1 = objUtility.SelectDistinct(Dt_Formulati, "Fr_Cod")
                                FrCod2 = objUtility.SelectDistinct(Dt_FormulatiTmp, "Fr_Cod")

                                If Not FrCod1 Is Nothing And Not FrCod2 Is Nothing Then
                                    Dim objCore As New AgronicaCoreUtility.VettoriUtility
                                    objCore.Interseca_Vettori(FrCod1, FrCod2, FrCod)
                                    If Not FrCod Is Nothing AndAlso FrCod.Length > 0 Then
                                        For j = 0 To Dt_Formulati.Rows.Count - 1
                                            FormulatoPresente = False
                                            For x = 0 To UBound(FrCod)
                                                If Dt_Formulati.Rows(j).Item("Fr_Cod") = FrCod(x) Then
                                                    FormulatoPresente = True
                                                    Exit For
                                                End If
                                            Next
                                            If FormulatoPresente = False Then
                                                Dt_Formulati.Rows(j).Delete()
                                                'Dt_Formulati.AcceptChanges()
                                            End If
                                        Next
                                        Dt_Formulati.AcceptChanges()
                                    Else
                                        Dt_Formulati.Clear()
                                    End If
                                End If

                            End If


                        Next

                    End If
                Case 1
                    If Not Av_Gru Is Nothing Then

                        For i = 0 To UBound(Av_Gru)

                            Dt_FormulatiTmp = LeggiXClassificazione_PA_Dosi_Avversita(objParametri,
                                                                              TestoRicerca,
                                                                              TipoRichiesto,
                                                                              Veg_Cod,
                                                                              strPA,
                                                                              Data,
                                                                              Opt_Avversita_Infestanti,
                                                                              Opt_Singola_Gruppo,
                                                                              Av_Gru(i),
                                                                              0,
                                                                              strFiltro,
                                                                              strSort,
                                                                              Grfi_cod,
                                                                              FlagVisualizza_Commercio_Tutti_Revocati,
                                                                                    Stato_Cod)

                            If i = 0 Then
                                Dt_Formulati = Dt_FormulatiTmp.Copy
                            Else
                                Dim objUtility As New AgronicaCoreUtility.DatatableUtility
                                FrCod1 = objUtility.SelectDistinct(Dt_Formulati, "Fr_Cod")
                                FrCod2 = objUtility.SelectDistinct(Dt_FormulatiTmp, "Fr_Cod")

                                If Not FrCod1 Is Nothing And Not FrCod2 Is Nothing Then
                                    Dim objCore As New AgronicaCoreUtility.VettoriUtility
                                    objCore.Interseca_Vettori(FrCod1, FrCod2, FrCod)
                                    If Not FrCod Is Nothing AndAlso FrCod.Length > 0 Then
                                        For j = 0 To Dt_Formulati.Rows.Count - 1
                                            FormulatoPresente = False
                                            For x = 0 To UBound(FrCod)
                                                If Dt_Formulati.Rows(j).Item("Fr_Cod") = FrCod(x) Then
                                                    FormulatoPresente = True
                                                    Exit For
                                                End If
                                            Next
                                            If FormulatoPresente = False Then
                                                Dt_Formulati.Rows(j).Delete()
                                                'Dt_Formulati.AcceptChanges()
                                            End If
                                        Next
                                        Dt_Formulati.AcceptChanges()
                                    Else
                                        Dt_Formulati.Clear()
                                    End If
                                End If

                            End If


                        Next

                    End If

                Case Else

                    Dt_Formulati = LeggiXClassificazione_PA_Dosi_Avversita(objParametri,
                                                  TestoRicerca,
                                                  TipoRichiesto,
                                                  Veg_Cod,
                                                  strPA,
                                                  Data,
                                                  Opt_Avversita_Infestanti,
                                                  Opt_Singola_Gruppo,
                                                  0,
                                                  0,
                                                  strFiltro,
                                                  strSort,
                                                  Grfi_cod,
                                                  FlagVisualizza_Commercio_Tutti_Revocati,
                                                                                    Stato_Cod)
            End Select


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Dt_Formulati


    End Function

    Public Function LeggiXClassificazione_PA_Dosi_Storico(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal TestoRicerca As String = "",
                                                Optional ByVal TipoRichiesto As Integer = 0,
                                                Optional ByVal Veg_Cod As Integer = 0,
                                                Optional ByVal strPA As String = "",
                                                Optional ByVal Data As Date = #12/31/2100#,
                                                Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                                                Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                                                Optional ByVal Av_Gru() As Integer = Nothing,
                                                Optional ByVal Av_Cod() As Integer = Nothing,
                                                Optional ByVal strFiltro As String = "",
                                                Optional ByVal strSort As String = "",
                                                Optional ByVal Grfi_cod As Integer = 0,
                                                Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                                Optional ByVal Copertura As String = "",
                                                Optional ByVal Stato_Cod As String = "IT",
                                                Optional ByVal Fr_Cod As String = ""
                                        ) _
                                        As DataTable


        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi_Avversita_Storico()"

        Dim MessaggioErrore As String = ""
        Dim Dt_Formulati As New DataTable

        Try

            Select Case Opt_Singola_Gruppo

                Case 0

                    If Not Av_Cod Is Nothing AndAlso Av_Cod.Length > 0 Then

                        'eliminato vettore
                        'l'avversità nella nuova agenda è una DDL
                        Dt_Formulati = LeggiXClassificazione_PA_Dosi_Avversita_Storico(objParametri,
                                                                              TestoRicerca,
                                                                              TipoRichiesto,
                                                                              Veg_Cod,
                                                                              strPA,
                                                                              Data,
                                                                              Opt_Avversita_Infestanti,
                                                                              Opt_Singola_Gruppo,
                                                                              0,
                                                                              Av_Cod(0),
                                                                              strFiltro,
                                                                              strSort,
                                                                              Grfi_cod,
                                                                              FlagVisualizza_Commercio_Tutti_Revocati,
                                                                              Copertura,
                                                                              Stato_Cod,
                                                                              Fr_Cod)


                    End If

                Case 1

                    If Not Av_Gru Is Nothing AndAlso Av_Gru.Length > 0 Then

                        'eliminato vettore
                        'l'avversità nella nuova agenda è una DDL

                        Dt_Formulati = LeggiXClassificazione_PA_Dosi_Avversita_Storico(objParametri,
                                                                              TestoRicerca,
                                                                              TipoRichiesto,
                                                                              Veg_Cod,
                                                                              strPA,
                                                                              Data,
                                                                              Opt_Avversita_Infestanti,
                                                                              Opt_Singola_Gruppo,
                                                                              Av_Gru(0),
                                                                              0,
                                                                              strFiltro,
                                                                              strSort,
                                                                              Grfi_cod,
                                                                              FlagVisualizza_Commercio_Tutti_Revocati,
                                                                              Copertura,
                                                                              Stato_Cod,
                                                                              Fr_Cod)

                    End If

                Case Else

                    Dt_Formulati = LeggiXClassificazione_PA_Dosi_Avversita_Storico(objParametri,
                                                  TestoRicerca,
                                                  TipoRichiesto,
                                                  Veg_Cod,
                                                  strPA,
                                                  Data,
                                                  Opt_Avversita_Infestanti,
                                                  Opt_Singola_Gruppo,
                                                  0,
                                                  0,
                                                  strFiltro,
                                                  strSort,
                                                  Grfi_cod,
                                                  FlagVisualizza_Commercio_Tutti_Revocati,
                                                  Copertura,
                                                  Stato_Cod,
                                                  Fr_Cod)


            End Select


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
        End Try

        Return Dt_Formulati


    End Function

    'sostituite le TABELLE FormulatixSpecieVegetali, FormulatixSpeciexAvversita, FormulatixSpeciexInfestanti, FormulatixSpeciexAvversitaxDosi, FormulatixSpeciexInfestantixDosi
    'con le VISTE FormulatixSpecieVegetalixNormative, FormulatixSpeciexAvversitaxNormative, FormulatixSpeciexInfestantixNormative, FormulatixSpeciexAvversitaxDosixNormative, FormulatixSpeciexInfestantixDosixNormative
    Public Function LeggiXClassificazione_PA_Dosi_Avversita_Storico(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal TestoRicerca As String = "",
                               Optional ByVal TipoRichiesto As Integer = 0,
                               Optional ByVal Veg_Cod As Integer = 0,
                               Optional ByVal strPA As String = "",
                               Optional ByVal Data As Date = AGRODATAFINE,
                               Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                               Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                               Optional ByVal Av_Gru As Integer = 0,
                               Optional ByVal Av_Cod As Integer = 0,
                               Optional ByVal strFiltro As String = "",
                               Optional ByVal strSort As String = "",
                               Optional ByVal Grfi_cod As Integer = 0,
                               Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                               Optional ByVal Copertura As String = "",
                               Optional ByVal Stato_Cod As String = "IT",
                               Optional ByVal Fr_Cod As String = ""
                            ) _
                            As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi_Avversita_Storico()"

        Dim MessaggioErrore As String = ""
        Dim Dt_App As New DataTable
        Dim DT As DataTable
        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbCteQuery As New System.Text.StringBuilder
        Dim stbSelectQuery As New System.Text.StringBuilder
        Dim stbFromQuery1 As New System.Text.StringBuilder
        Dim stbFromQuery2 As New System.Text.StringBuilder
        Dim stbVariabileQuery As New System.Text.StringBuilder
        Dim stbFiltroQuery As New System.Text.StringBuilder
        Dim i, n As Integer
        Dim AvGruTmp(0) As String

        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim Priorita As Boolean = True
        Dim _objLog As New AgronicaCoreDataProvider.LogProvider

        Try

            '--------------------------------------------------------------------------
            '------ SELECT LIST COMUNE 
            '--------------------------------------------------------------------------
            stbCteQuery.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            '''
            '''DRUDI 20/06/2024 con CTE
            '''           

            'Creazione tabella Temporanea ordinamento String_AGG
            stbCteQuery.Append("  Create Table #Ord (Fr_Cod Int, Pa_Cod Int, Pa_Des nvarchar(255), Titolo float, Peso float) ")
            stbCteQuery.Append(" Insert  #Ord (Fr_Cod, Pa_Cod, Pa_Des, Titolo, Peso) ")
            stbCteQuery.Append(" Select  FPA.fr_cod, FPA.Pa_Cod,  PA.Pa_Des, Titolo, Peso ")
            stbCteQuery.Append(" From PrincipiAttivi PA inner Join FormulatixPrincipiAttivi FPA ")
            stbCteQuery.Append(" On FPA.Pa_Cod = PA.Pa_Cod ")
            stbCteQuery.Append(" Order by FPA.Fr_Cod, Titolo Desc ")

            stbCteQuery.AppendLine(" 	 SELECT outside.Fr_Cod, MAX(FPTitolo.Pa_Cod) as Pa_Cod, MAX(FPTitolo.Pa_Des) as Pa_Des, MAX(FPTitolo.Titolo) as Titolo ")
            stbCteQuery.AppendLine("     INTO #FormulatixTop1PrincipiAttivi ")
            stbCteQuery.AppendLine(" 	 FROM FormulatixPrincipiAttivi outside ")
            stbCteQuery.AppendLine(" 	 CROSS APPLY ( SELECT TOP 1 inside.Fr_Cod, inside.Pa_Cod, PrincipiAttivi.Pa_Des, inside.Titolo ")
            stbCteQuery.AppendLine(" 					FROM FormulatixPrincipiAttivi inside  ")
            stbCteQuery.AppendLine(" 					JOIN PrincipiAttivi ON inside.Pa_Cod = PrincipiAttivi.Pa_Cod ")
            stbCteQuery.AppendLine(" 					WHERE outside.fr_cod = inside.fr_cod ORDER BY Titolo DESC ) as FPTitolo ")
            stbCteQuery.AppendLine(" 	 GROUP BY outside.Fr_Cod ")
            stbCteQuery.AppendLine(" 	Select Fr_Cod,   ")
            stbCteQuery.AppendLine("    String_AGG(Pa_cod, '|') as [strPA_COD], ")
            stbCteQuery.AppendLine("    string_agg(Pa_Des, '|') as [strPA_DES],   ")
            stbCteQuery.AppendLine("    string_agg(Titolo, '|') as [strTITOLI],  ")
            stbCteQuery.AppendLine("    string_agg(Peso, '|') as [strPESI]  ")
            stbCteQuery.AppendLine("    INTO #PrincipiCodici ")
            stbCteQuery.AppendLine("    FROM ( Select #Ord.Fr_Cod, #Ord.Pa_Cod, #ord.Pa_Des, #Ord.Titolo, #Ord.Peso From #Ord ")
            stbCteQuery.AppendLine("    	 ) [MainPrincipiCodici] Group By Fr_Cod  ")

            stbSelectQuery.Append(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.Data_Reg, " & vbCrLf)
            stbSelectQuery.Append(" FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Revocato, Formulati.Data_Revo, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Sospeso, Formulati.Data_Sosp, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Termine, Formulati.Data_Term, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Verificato_Flag, " & vbCrLf)

            '(21/11/2017 fede) aggiunti dati fine scorte, metodo impiego, copertura
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.For_Veg_Cod, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Fr_Des_Prec,'') AS Fr_Des_Prec, " & vbCrLf)
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.FormulatiXAllegatiNormative_IDRiga,0) AS FormulatiXAllegatiNormative_IDRiga, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Mdi_Cod,0) AS Mdi_Cod, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Flag_Protetto,0) AS Flag_Protetto, " & vbCrLf)

            'Gestione Prodotto Secco
            stbSelectQuery.Append(" Case When Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') Then 1 Else 0 End AS Polverulento, " & vbCrLf)


            '(22/11/2017 fede) TO DO
            'quando ci saranno i dati nella tabella FormulatixSpecieVegetalixNormative la data verrà letta da qui
            'per ora lascio l'indicazione dell'ultimo decreto
            '(12/04/2018 fede) ora i dati sono in FormulatixSpecieVegetalixNormative
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.DataAttoNormativo AS DataAttoNormativo, " & vbCrLf)
            'stbSelectQuery.Append(" (SELECT  TOP 1 DataAttoNormativo " & vbCrLf)
            'stbSelectQuery.Append(" FROM FormulatiXAllegatiNormative  " & vbCrLf)
            'stbSelectQuery.Append(" Where FormulatiXAllegatiNormative.FOR_COD = FORMULATI.FR_COD " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY DataAttoNormativo DESC) as DataAttoNormativo, " & vbCrLf)




            'stbSelectQuery.Append("(SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" FROM     dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("        dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Cod, " & vbCrLf)

            'stbSelectQuery.Append("ISNULL((SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Des " & vbCrLf)
            'stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC),'') as Pa_Des, " & vbCrLf)

            'stbSelectQuery.Append("(SELECT  TOP 1 dbo.FormulatixPrincipiAttivi.Titolo " & vbCrLf)
            'stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Titolo, " & vbCrLf)
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.pa_cod, 0) As Pa_Cod,  ")
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.pa_des, '') As Pa_Des,  ")
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.Titolo, '') As Titolo,  ")


            'stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_COD,'') AS strCLTOSS_COD, " & vbCrLf)
            'stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_Grado,'') AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" '' AS strCLTOSS_COD, " & vbCrLf)
            stbSelectQuery.Append(" '' AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)
            '(10/07/2018) fede aggiunto peso sostanza
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPESI,'') AS strPESI, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Epoca_Cod,0) AS Epoca_Cod, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Min,0) AS BufferZone_Min, ")
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Max,0) AS BufferZone_Max, ")

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.TempoCarenza,0) AS TempoCarenza,  " & vbCrLf)


            stbSelectQuery.Append(" ISNULL(Formulati.DurataFeromone,0) AS DurataFeromone " & vbCrLf)


            '--------------------------------------------------------------------------
            '------ FROM COMUNE 
            '--------------------------------------------------------------------------

            stbFromQuery1.Append(" FROM  Formulati INNER JOIN " & vbCrLf)

            If Stato_Cod <> "" Then
                stbFromQuery1.Append("   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' INNER JOIN " & vbCrLf)
            End If

            stbFromQuery1.Append("   FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   FormulatixSpecieVegetalixNormative ON Formulati.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod  " & vbCrLf)

            If Trim(strPA) <> "" Then
                stbFromQuery1.Append(" INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN " & vbCrLf)
                stbFromQuery1.Append("   PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  " & vbCrLf)
            End If
            stbFromQuery2.Append("   LEFT JOIN #FormulatixTop1PrincipiAttivi FormulatixTop1PrincipiAttivi ON Formulati.Fr_Cod = FormulatixTop1PrincipiAttivi.Fr_Cod    " & vbCrLf)
            stbFromQuery2.Append("   LEFT JOIN #PrincipiCodici PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)
            stbFromQuery2.Append("   Left Outer Join FormulatiXFormulazioni on (FormulatiXFormulazioni.FR_COD=Formulati.FR_COD )   " & vbCrLf)

            '------------------------------------


            stbFromQuery2.Append(" WHERE   1=1" & vbCrLf)

            If Not String.IsNullOrEmpty(TestoRicerca) Then
                stbFromQuery2.Append(" AND     (Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%') " & vbCrLf)
            End If

            If Not String.IsNullOrEmpty(Fr_Cod) Then
                stbFromQuery2.Append(" AND    Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ")  " & vbCrLf)
            End If

            stbFromQuery2.Append(" AND      Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Data) & "  " & vbCrLf)


            '--------------------------------------------------------------------------
            '------ FILTRO COMUNE 
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case enum_TipoFormulato.Tutti

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  " & vbCrLf)


                Case enum_TipoFormulato.Coadiuvanti

                    stbFiltroQuery.Append("  And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  " & vbCrLf)

                Case enum_TipoFormulato.Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 608  " & vbCrLf)

                Case enum_TipoFormulato.Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (603, 609)   " & vbCrLf)

                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestantixNormative.Av_Cod=0 " & vbCrLf)
                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestantixNormative.Av_Gru=0 " & vbCrLf)


                Case enum_TipoFormulato.Geodisinfestanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 607  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) " & vbCrLf)

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603,609) " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) " & vbCrLf)

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607, 606,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    stbFiltroQuery.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)


            End Select

            Dim tabAvInf As String = ""
            If Opt_Singola_Gruppo >= 0 Then
                tabAvInf = "FormulatixSpeciexAvversitaxDosixNormative"
                If Opt_Avversita_Infestanti > 0 Then
                    tabAvInf = "FormulatixSpeciexInfestantixDosixNormative"
                End If
            End If

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If Grfi_cod <> 0 Then

                stbFiltroQuery.Append("  and ( FormulatixSpecieVegetalixNormative.grfi_cod = 0 " & vbCrLf)
                stbFiltroQuery.Append("  OR (FormulatixSpecieVegetalixNormative.grfi_cod <> 0 and  " & vbCrLf)
                stbFiltroQuery.Append("     FormulatixSpecieVegetalixNormative.grfi_cod = " & Grfi_cod & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)

                If Opt_Singola_Gruppo >= 0 Then

                    stbFiltroQuery.Append("  and ( " & tabAvInf & ".grfi_cod = 0 " & vbCrLf)
                    stbFiltroQuery.Append("  OR (" & tabAvInf & ".grfi_cod <> 0 and  " & vbCrLf)
                    stbFiltroQuery.Append("     " & tabAvInf & ".grfi_cod = " & Grfi_cod & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)

                End If

            End If



            '(10/01/2018 fede) aggiunto filtro copertura
            If Copertura <> "" Then

                Select Case Copertura

                    Case "0" 'solo fuori campo (fuori campo + non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 1 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto <> 1 " & vbCrLf)
                        End If

                    Case "1" 'solo serra (serra + non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 2 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto <> 2 " & vbCrLf)
                        End If

                    Case "0,1", "1,0" 'entrambi (non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto = 0 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto = 0 " & vbCrLf)
                        End If

                End Select


            End If


            If Veg_Cod <> 0 Then

                Select Case TipoRichiesto
                    Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci,
                        enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                        stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetalixNormative.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                        stbFiltroQuery.Append("      OR  (FormulatixSpecieVegetalixNormative.Veg_Cod = 0 AND FormulatixSpecieVegetalixNormative.Grsp_Cod = 0 )" & vbCrLf)
                        stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetalixNormative.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                        stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
                    Case Else
                        stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetalixNormative.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                        'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
                        If TipoRichiesto = enum_TipoFormulato.Geodisinfestanti Then
                            stbFiltroQuery.Append("      OR  FormulatixSpecieVegetalixNormative.Veg_Cod = 5000336 " & vbCrLf)
                        End If
                        stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetalixNormative.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                        stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
                End Select
            End If

            If Trim(strPA) <> "" Then
                stbFiltroQuery.Append("  AND  PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "  " & vbCrLf)
                stbFiltroQuery.Append("  AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)" & vbCrLf)
            End If


            'stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            'stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)

            '(21/11/2017 fede) aggiunti dati fine scorte
            stbFiltroQuery.Append("  AND (")
            stbFiltroQuery.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
            stbFiltroQuery.Append("  OR ")
            stbFiltroQuery.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
            stbFiltroQuery.Append("      )")


            If Opt_Singola_Gruppo >= 0 Then

                tabAvInf = "FormulatixSpeciexAvversitaxNormative"
                Dim tabAvInfDosi As String = "FormulatixSpeciexAvversitaxDosixNormative"
                If Opt_Avversita_Infestanti > 0 Then

                    tabAvInfDosi = "FormulatixSpeciexInfestantixDosixNormative"
                    tabAvInf = "FormulatixSpeciexInfestantixNormative"
                End If

                '(21/11/2017 fede) aggiunti dati fine scorte
                stbFiltroQuery.Append("  AND (")
                stbFiltroQuery.Append("  (  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("     AND  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("  OR ")
                stbFiltroQuery.Append("  (  " & tabAvInf & ".DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("      )")

                stbFiltroQuery.Append("  AND (")
                stbFiltroQuery.Append("  (  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("     AND  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("  OR ")
                stbFiltroQuery.Append("  (  " & tabAvInfDosi & ".DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("      )")

                'stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            End If



            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                Case 1 'prodotti commercio + revocati
                Case 2 'prodotti revocati
                    stbFiltroQuery.Append("  And   FORMULATI.Revocato= 1 " & vbCrLf)
            End Select

            If strFiltro <> "" Then
                stbFiltroQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri))
            End If

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------



            '-----------------------------------------------------------------------------------
            '--- PRODOTTI REGISTRATI ESATTAMENTE SULL'AVVERSITA / GRUPPO SELEZIONATO 
            '-----------------------------------------------------------------------------------
            stbQuery.Append(stbCteQuery)
            stbQuery.Append(stbSelectQuery)

            Select Case Opt_Avversita_Infestanti

                '------------------------------------------------------------------------------------
                '------ AVVERSITA -------------------------------------------------------------------
                '------------------------------------------------------------------------------------
                Case 0

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) As Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) As Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) As UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)



                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------

                    Select Case Opt_Singola_Gruppo

                        Case 0

                            Select Case TipoRichiesto
                                Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori,
                                     enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0 )" & vbCrLf)
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0 )" & vbCrLf)
                                Case Else
                                    If Av_Cod <> 0 Then
                                        stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                                    End If
                            End Select

                        Case 1

                            Select Case TipoRichiesto
                                Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori,
                                     enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0 )" & vbCrLf)
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0 )" & vbCrLf)
                                Case Else
                                    If Av_Gru <> 0 Then
                                        stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                                    End If
                            End Select

                    End Select



                        '------------------------------------------------------------------------------------
                        '------ INFESTANTI ------------------------------------------------------------------
                        '------------------------------------------------------------------------------------

                Case 1

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)
                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1
                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------------------------------------

                    Select Case Opt_Singola_Gruppo

                        Case 0
                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestantixNormative.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestantixNormative.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select


                Case Else

                    stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                    stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)


                    stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                    stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery1)

                    stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery2)

                    '(23/05/2016 fede) modifica x filtrare dosaggio senza avversita/gruppo nei prodotti che hanno anche altra classificazione
                    Select Case TipoRichiesto

                        Case 3, 4, 11 'Coadiuvanti, Fitoregolatori, Fisiofarmaci e Corroboranti

                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0)" & vbCrLf)
                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0)" & vbCrLf)

                    End Select

            End Select

            stbQuery.Append(stbVariabileQuery)

            stbQuery.Append(stbFiltroQuery)

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case 3, 4 'Coadiuvanti, Fitoregolatori

                Case 6 'Disseccanti

                Case 11  'Fisiofarmaci e Corroboranti

                Case Else

                    If Opt_Singola_Gruppo <> -1 Then

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SULL'AVVERSITA FIGLIA / GRUPPO PADRE 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        ' stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)


                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "" & vbCrLf)
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SUI GRUPPI DELLA GERARCHIA DEI GRUPPI 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        'stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then

                                            '''' 295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986
                                            '''Dim strGruppi As String = "295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986"
                                            '''AvGruTot.Clear()
                                            '''Dim ArrayGru() As String = strGruppi.Split(",")
                                            '''For g = 0 To ArrayGru.Length - 1
                                            '''    If Not AvGruTot.ContainsKey(ArrayGru(g)) Then
                                            '''        AvGruTot.Add(ArrayGru(g), "" & vbCrLf)
                                            '''    End If
                                            '''Next

                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)


                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)
                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                    End If

            End Select

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES ASC" & vbCrLf)
            End If

            If Priorita = True Then
                stbQuery.Append(" ,priorita" & vbCrLf)
            End If

            stbQuery.Append(" ,DataAttoNormativo DESC" & vbCrLf)

            stbQuery.Append(" DROP Table #FormulatixTop1PrincipiAttivi ")
            stbQuery.Append(" DROP TABLE #PrincipiCodici ")
            stbQuery.Append(" DROP Table #Ord ")
            '_objLog.Scrivi_LOG(objParametri, NomeRoutine, stbQuery.ToOrigin)

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    '(29/11/2019 fede) temporaneamente NON filtro per prodotti ESTERI
                    If Stato_Cod = "IT" Then
                        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                        DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                    Else
                        DT = Dt_App
                    End If
                Case 1 'prodotti commercio + revocati
                    DT = Dt_App
                Case 2 'prodotti revocati
                    DT = Dt_App
            End Select

            'Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
            'DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)

            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    If DT.Rows(i).Item("Priorita") = 3 And DT.Rows(i).Item("Av_Cod") = 0 And DT.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If DT.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                DT.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If

#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            DT.TableName = "Prodotti"
            Dv.Table = DT
            Dv.Sort = "FR_DES , Priorita, Ordine "
            DT = Dv.ToTable
#Else
            'Codice per versioni < 2010
#End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function LeggiXClassificazione_PA_Dosi_Avversita_Storico_XML_Path(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal TestoRicerca As String = "",
                               Optional ByVal TipoRichiesto As Integer = 0,
                               Optional ByVal Veg_Cod As Integer = 0,
                               Optional ByVal strPA As String = "",
                               Optional ByVal Data As Date = AGRODATAFINE,
                               Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                               Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                               Optional ByVal Av_Gru As Integer = 0,
                               Optional ByVal Av_Cod As Integer = 0,
                               Optional ByVal strFiltro As String = "",
                               Optional ByVal strSort As String = "",
                               Optional ByVal Grfi_cod As Integer = 0,
                               Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                               Optional ByVal Copertura As String = "",
                               Optional ByVal Stato_Cod As String = "IT",
                               Optional ByVal Fr_Cod As String = ""
                            ) _
                            As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi_Avversita_Storico()"

        Dim MessaggioErrore As String = ""
        Dim Dt_App As New DataTable
        Dim DT As DataTable
        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbCteQuery As New System.Text.StringBuilder
        Dim stbSelectQuery As New System.Text.StringBuilder
        Dim stbFromQuery1 As New System.Text.StringBuilder
        Dim stbFromQuery2 As New System.Text.StringBuilder
        Dim stbVariabileQuery As New System.Text.StringBuilder
        Dim stbFiltroQuery As New System.Text.StringBuilder
        Dim i, n As Integer
        Dim AvGruTmp(0) As String

        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim Priorita As Boolean = True
        Dim _objLog As New AgronicaCoreDataProvider.LogProvider

        Try

            '--------------------------------------------------------------------------
            '------ SELECT LIST COMUNE 
            '--------------------------------------------------------------------------
            stbCteQuery.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            '''
            '''DRUDI 20/06/2024 con CTE
            '''
            stbCteQuery.AppendLine(" WITH FormulatixTop1PrincipiAttivi as ( ")
            stbCteQuery.AppendLine(" 	 SELECT outside.Fr_Cod, MAX(FPTitolo.Pa_Cod) as Pa_Cod, MAX(FPTitolo.Pa_Des) as Pa_Des, MAX(FPTitolo.Titolo) as Titolo ")
            stbCteQuery.AppendLine(" 	 FROM FormulatixPrincipiAttivi outside ")
            stbCteQuery.AppendLine(" 	 CROSS APPLY ( SELECT TOP 1 inside.Fr_Cod, inside.Pa_Cod, PrincipiAttivi.Pa_Des, inside.Titolo ")
            stbCteQuery.AppendLine(" 					FROM FormulatixPrincipiAttivi inside  ")
            stbCteQuery.AppendLine(" 					JOIN PrincipiAttivi ON inside.Pa_Cod = PrincipiAttivi.Pa_Cod ")
            stbCteQuery.AppendLine(" 					WHERE outside.fr_cod = inside.fr_cod ORDER BY Titolo DESC ) as FPTitolo ")
            stbCteQuery.AppendLine(" 	 GROUP BY outside.Fr_Cod ")
            stbCteQuery.AppendLine("  ), ")
            stbCteQuery.AppendLine("  PrincipiCodici as ( ")
            stbCteQuery.AppendLine(" 	SELECT  MainPrincipiCodici.fr_cod,   ")
            stbCteQuery.AppendLine("    Left(MainPrincipiCodici.strPA_COD,Len(MainPrincipiCodici.strPA_COD)-1) As strPA_COD,   ")
            stbCteQuery.AppendLine("    Left(MainPrincipiCodici.strPA_DES,Len(MainPrincipiCodici.strPA_DES)-1) As strPA_DES,   ")
            stbCteQuery.AppendLine("    Left(MainPrincipiCodici.strTITOLI,Len(MainPrincipiCodici.strTITOLI)-1) As strTITOLI,  ")
            stbCteQuery.AppendLine("    Left(MainPrincipiCodici.strPESI,Len(MainPrincipiCodici.strPESI)-1) As strPESI  ")
            stbCteQuery.AppendLine("    FROM	(              Select distinct FPA2.FR_COD,   ")
            stbCteQuery.AppendLine("    		(Select CONVERT (nvarchar, FPA1.PA_COD)  + '|' AS [text()]    ")
            stbCteQuery.AppendLine("    	 From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA               ")
            stbCteQuery.AppendLine("    	 Where FPA1.FR_COD = FPA2.FR_COD                ")
            stbCteQuery.AppendLine("    	 and FPA1.PA_COD=PA.PA_COD                   ")
            stbCteQuery.AppendLine("    	 ORDER BY FPA1.FR_COD, titolo desc                   ")
            stbCteQuery.AppendLine("    	 For XML PATH ('')                 ) [strPA_COD],              ")
            stbCteQuery.AppendLine("    		 (Select CONVERT (nvarchar, PA.PA_DES)  + '|' AS [text()]    ")
            stbCteQuery.AppendLine("    	  From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA   ")
            stbCteQuery.AppendLine("    	  Where FPA1.FR_COD = FPA2.FR_COD          ")
            stbCteQuery.AppendLine("    	  and FPA1.PA_COD=PA.PA_COD             ")
            stbCteQuery.AppendLine("    	  ORDER BY FPA1.FR_COD, titolo desc             ")
            stbCteQuery.AppendLine("    	  For XML PATH ('')                 ) [strPA_DES],    ")
            stbCteQuery.AppendLine("    		 (Select CONVERT (nvarchar, FPA1.Titolo)  + '|' AS [text()]      ")
            stbCteQuery.AppendLine("    	  From FormulatixPrincipiAttivi FPA1  ")
            stbCteQuery.AppendLine("    	  Where FPA1.FR_COD = FPA2.FR_COD      ")
            stbCteQuery.AppendLine("    	  ORDER BY FPA1.FR_COD , titolo desc  ")
            stbCteQuery.AppendLine("    	  For XML PATH ('')                 ) [strTITOLI],	  ")
            stbCteQuery.AppendLine("    		 (Select CONVERT (nvarchar, FPA1.Peso)  + '|' AS [text()]      ")
            stbCteQuery.AppendLine("    	  From FormulatixPrincipiAttivi FPA1  ")
            stbCteQuery.AppendLine("    	  Where FPA1.FR_COD = FPA2.FR_COD      ")
            stbCteQuery.AppendLine("    	  ORDER BY FPA1.FR_COD , titolo desc  ")
            stbCteQuery.AppendLine("    	  For XML PATH ('')                 ) [strPESI]	  ")
            stbCteQuery.AppendLine("    	  From FormulatixPrincipiAttivi FPA2         ")
            stbCteQuery.AppendLine("    	 ) [MainPrincipiCodici] ")
            stbCteQuery.AppendLine("  ) ")

            stbSelectQuery.Append(" SELECT Formulati.FR_COD, Formulati.FR_DES, Formulati.Data_Reg, " & vbCrLf)
            stbSelectQuery.Append(" FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Revocato, Formulati.Data_Revo, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Sospeso, Formulati.Data_Sosp, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Termine, Formulati.Data_Term, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Verificato_Flag, " & vbCrLf)

            '(21/11/2017 fede) aggiunti dati fine scorte, metodo impiego, copertura
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.For_Veg_Cod, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Fr_Des_Prec,'') AS Fr_Des_Prec, " & vbCrLf)
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.FormulatiXAllegatiNormative_IDRiga,0) AS FormulatiXAllegatiNormative_IDRiga, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Mdi_Cod,0) AS Mdi_Cod, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Flag_Protetto,0) AS Flag_Protetto, " & vbCrLf)

            'Gestione Prodotto Secco
            stbSelectQuery.Append(" Case When Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') Then 1 Else 0 End AS Polverulento, " & vbCrLf)


            '(22/11/2017 fede) TO DO
            'quando ci saranno i dati nella tabella FormulatixSpecieVegetalixNormative la data verrà letta da qui
            'per ora lascio l'indicazione dell'ultimo decreto
            '(12/04/2018 fede) ora i dati sono in FormulatixSpecieVegetalixNormative
            stbSelectQuery.Append(" FormulatixSpecieVegetalixNormative.DataAttoNormativo AS DataAttoNormativo, " & vbCrLf)
            'stbSelectQuery.Append(" (SELECT  TOP 1 DataAttoNormativo " & vbCrLf)
            'stbSelectQuery.Append(" FROM FormulatiXAllegatiNormative  " & vbCrLf)
            'stbSelectQuery.Append(" Where FormulatiXAllegatiNormative.FOR_COD = FORMULATI.FR_COD " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY DataAttoNormativo DESC) as DataAttoNormativo, " & vbCrLf)




            'stbSelectQuery.Append("(SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" FROM     dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("        dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Cod, " & vbCrLf)

            'stbSelectQuery.Append("ISNULL((SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Des " & vbCrLf)
            'stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC),'') as Pa_Des, " & vbCrLf)

            'stbSelectQuery.Append("(SELECT  TOP 1 dbo.FormulatixPrincipiAttivi.Titolo " & vbCrLf)
            'stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            'stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            'stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            'stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Titolo, " & vbCrLf)
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.pa_cod, 0) As Pa_Cod,  ")
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.pa_des, '') As Pa_Des,  ")
            stbSelectQuery.AppendLine("    	  COALESCE(FormulatixTop1PrincipiAttivi.Titolo, '') As Titolo,  ")


            'stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_COD,'') AS strCLTOSS_COD, " & vbCrLf)
            'stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_Grado,'') AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" '' AS strCLTOSS_COD, " & vbCrLf)
            stbSelectQuery.Append(" '' AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)
            '(10/07/2018) fede aggiunto peso sostanza
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPESI,'') AS strPESI, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Epoca_Cod,0) AS Epoca_Cod, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Min,0) AS BufferZone_Min, ")
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Max,0) AS BufferZone_Max, ")

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetalixNormative.TempoCarenza,0) AS TempoCarenza,  " & vbCrLf)


            stbSelectQuery.Append(" ISNULL(Formulati.DurataFeromone,0) AS DurataFeromone " & vbCrLf)


            '--------------------------------------------------------------------------
            '------ FROM COMUNE 
            '--------------------------------------------------------------------------

            stbFromQuery1.Append(" FROM  Formulati INNER JOIN " & vbCrLf)

            If Stato_Cod <> "" Then
                stbFromQuery1.Append("   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' INNER JOIN " & vbCrLf)
            End If

            stbFromQuery1.Append("   FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   FormulatixSpecieVegetalixNormative ON Formulati.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod  " & vbCrLf)

            If Trim(strPA) <> "" Then
                stbFromQuery1.Append(" INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN " & vbCrLf)
                stbFromQuery1.Append("   PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  " & vbCrLf)
            End If
            stbFromQuery2.Append("   LEFT JOIN FormulatixTop1PrincipiAttivi ON Formulati.Fr_Cod = FormulatixTop1PrincipiAttivi.Fr_Cod    " & vbCrLf)
            '--------------------------------------------------------------------------

            'stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)
            'stbFromQuery2.Append("      SELECT  MainClassiCodici.for_cod, Left(MainClassiCodici.strCLTOSS_COD,Len(MainClassiCodici.strCLTOSS_COD)-1) As strCLTOSS_COD, Left(MainClassiCodici.strCLTOSS_Grado,Len(MainClassiCodici.strCLTOSS_Grado)-1) As strCLTOSS_Grado " & vbCrLf)
            'stbFromQuery2.Append("      FROM (  " & vbCrLf)
            'stbFromQuery2.Append("          Select distinct FCT2.FOR_COD,            " & vbCrLf)

            'stbFromQuery2.Append("              (Select CONVERT (nvarchar, FCT1.CLTOSS_COD)  + ',' AS [text()]" & vbCrLf)
            'stbFromQuery2.Append("               From FormulatiXClassiTossicologiche FCT1, ClasseTossicologica CT" & vbCrLf)
            'stbFromQuery2.Append("               Where FCT1.FOR_COD = FCT2.FOR_COD" & vbCrLf)
            'stbFromQuery2.Append("               and FCT1.CLTOSS_COD=CT.CLTOSS_COD" & vbCrLf)
            'stbFromQuery2.Append("               ORDER BY FCT1.FOR_COD, GradoTossicita desc" & vbCrLf)
            'stbFromQuery2.Append("               For XML PATH ('')" & vbCrLf)
            'stbFromQuery2.Append("               ) [strCLTOSS_COD]," & vbCrLf)

            'stbFromQuery2.Append("              (Select CONVERT (nvarchar, CT.GradoTossicita)  + ',' AS [text()]" & vbCrLf)
            'stbFromQuery2.Append("               From FormulatiXClassiTossicologiche FCT1, ClasseTossicologica CT" & vbCrLf)
            'stbFromQuery2.Append("               Where FCT1.FOR_COD = FCT2.FOR_COD" & vbCrLf)
            'stbFromQuery2.Append("               and FCT1.CLTOSS_COD=CT.CLTOSS_COD" & vbCrLf)
            'stbFromQuery2.Append("               ORDER BY FCT1.FOR_COD, GradoTossicita desc" & vbCrLf)
            'stbFromQuery2.Append("               For XML PATH ('')" & vbCrLf)
            'stbFromQuery2.Append("               ) [strCLTOSS_Grado]" & vbCrLf)

            'stbFromQuery2.Append("        From FormulatiXClassiTossicologiche FCT2" & vbCrLf)
            'stbFromQuery2.Append("        ) [MainClassiCodici] " & vbCrLf)
            'stbFromQuery2.Append("     )" & vbCrLf)
            'stbFromQuery2.Append("   ClassiCodici on ClassiCodici.FOR_COD=Formulati.FR_COD  " & vbCrLf)


            ''------------------------------------
            'stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)

            'stbFromQuery2.Append("   SELECT  MainPrincipiCodici.fr_cod,  " & vbCrLf)
            'stbFromQuery2.Append("   Left(MainPrincipiCodici.strPA_COD,Len(MainPrincipiCodici.strPA_COD)-1) As strPA_COD,  " & vbCrLf)
            'stbFromQuery2.Append("   Left(MainPrincipiCodici.strPA_DES,Len(MainPrincipiCodici.strPA_DES)-1) As strPA_DES,  " & vbCrLf)
            'stbFromQuery2.Append("   Left(MainPrincipiCodici.strTITOLI,Len(MainPrincipiCodici.strTITOLI)-1) As strTITOLI, " & vbCrLf)
            ''(10/07/2018) fede aggiunto peso sostanza
            'stbFromQuery2.Append("   Left(MainPrincipiCodici.strPESI,Len(MainPrincipiCodici.strPESI)-1) As strPESI " & vbCrLf)

            'stbFromQuery2.Append("   FROM	(              Select distinct FPA2.FR_COD,  " & vbCrLf)
            'stbFromQuery2.Append("   		(Select CONVERT (nvarchar, FPA1.PA_COD)  + '|' AS [text()]   " & vbCrLf)
            'stbFromQuery2.Append("   	 From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA              " & vbCrLf)
            'stbFromQuery2.Append("   	 Where FPA1.FR_COD = FPA2.FR_COD                  " & vbCrLf)
            'stbFromQuery2.Append("   	 and FPA1.PA_COD=PA.PA_COD                  " & vbCrLf)
            'stbFromQuery2.Append("   	 ORDER BY FPA1.FR_COD, titolo desc                  " & vbCrLf)
            'stbFromQuery2.Append("   	 For XML PATH ('')                 ) [strPA_COD],             " & vbCrLf)

            'stbFromQuery2.Append("   		 (Select CONVERT (nvarchar, PA.PA_DES)  + '|' AS [text()]   " & vbCrLf)
            'stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA            " & vbCrLf)
            'stbFromQuery2.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD         " & vbCrLf)
            'stbFromQuery2.Append("   	  and FPA1.PA_COD=PA.PA_COD            " & vbCrLf)
            'stbFromQuery2.Append("   	  ORDER BY FPA1.FR_COD, titolo desc            " & vbCrLf)
            'stbFromQuery2.Append("   	  For XML PATH ('')                 ) [strPA_DES],   " & vbCrLf)

            'stbFromQuery2.Append("   		 (Select CONVERT (nvarchar, FPA1.Titolo)  + '|' AS [text()]     " & vbCrLf)
            'stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA1 " & vbCrLf)
            'stbFromQuery2.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD     " & vbCrLf)
            'stbFromQuery2.Append("   	  ORDER BY FPA1.FR_COD , titolo desc " & vbCrLf)
            'stbFromQuery2.Append("   	  For XML PATH ('')                 ) [strTITOLI],	 " & vbCrLf)

            ''(10/07/2018) fede aggiunto peso sostanza
            'stbFromQuery2.Append("   		 (Select CONVERT (nvarchar, FPA1.Peso)  + '|' AS [text()]     " & vbCrLf)
            'stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA1 " & vbCrLf)
            'stbFromQuery2.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD     " & vbCrLf)
            'stbFromQuery2.Append("   	  ORDER BY FPA1.FR_COD , titolo desc " & vbCrLf)
            'stbFromQuery2.Append("   	  For XML PATH ('')                 ) [strPESI]	 " & vbCrLf)

            'stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA2        " & vbCrLf)

            'stbFromQuery2.Append("   	 ) [MainPrincipiCodici]          " & vbCrLf)
            'stbFromQuery2.Append("   )      " & vbCrLf)
            'stbFromQuery2.Append("   PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)
            stbFromQuery2.Append("   LEFT JOIN PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)

            stbFromQuery2.Append("   Left Outer Join FormulatiXFormulazioni on (FormulatiXFormulazioni.FR_COD=Formulati.FR_COD )   " & vbCrLf)

            '------------------------------------


            stbFromQuery2.Append(" WHERE   1=1" & vbCrLf)

            If Not String.IsNullOrEmpty(TestoRicerca) Then
                stbFromQuery2.Append(" AND     (Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%') " & vbCrLf)
            End If

            If Not String.IsNullOrEmpty(Fr_Cod) Then
                stbFromQuery2.Append(" AND    Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ")  " & vbCrLf)
            End If

            stbFromQuery2.Append(" AND      Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Data) & "  " & vbCrLf)


            '--------------------------------------------------------------------------
            '------ FILTRO COMUNE 
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case enum_TipoFormulato.Tutti

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  " & vbCrLf)


                Case enum_TipoFormulato.Coadiuvanti

                    stbFiltroQuery.Append("  And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  " & vbCrLf)

                Case enum_TipoFormulato.Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 608  " & vbCrLf)

                Case enum_TipoFormulato.Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (603, 609)   " & vbCrLf)

                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestantixNormative.Av_Cod=0 " & vbCrLf)
                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestantixNormative.Av_Gru=0 " & vbCrLf)


                Case enum_TipoFormulato.Geodisinfestanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 607  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) " & vbCrLf)

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603,609) " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) " & vbCrLf)

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607, 606,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    stbFiltroQuery.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)


            End Select

            Dim tabAvInf As String = ""
            If Opt_Singola_Gruppo >= 0 Then
                tabAvInf = "FormulatixSpeciexAvversitaxDosixNormative"
                If Opt_Avversita_Infestanti > 0 Then
                    tabAvInf = "FormulatixSpeciexInfestantixDosixNormative"
                End If
            End If

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If Grfi_cod <> 0 Then

                stbFiltroQuery.Append("  and ( FormulatixSpecieVegetalixNormative.grfi_cod = 0 " & vbCrLf)
                stbFiltroQuery.Append("  OR (FormulatixSpecieVegetalixNormative.grfi_cod <> 0 and  " & vbCrLf)
                stbFiltroQuery.Append("     FormulatixSpecieVegetalixNormative.grfi_cod = " & Grfi_cod & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)

                If Opt_Singola_Gruppo >= 0 Then

                    stbFiltroQuery.Append("  and ( " & tabAvInf & ".grfi_cod = 0 " & vbCrLf)
                    stbFiltroQuery.Append("  OR (" & tabAvInf & ".grfi_cod <> 0 and  " & vbCrLf)
                    stbFiltroQuery.Append("     " & tabAvInf & ".grfi_cod = " & Grfi_cod & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)

                End If

            End If



            '(10/01/2018 fede) aggiunto filtro copertura
            If Copertura <> "" Then

                Select Case Copertura

                    Case "0" 'solo fuori campo (fuori campo + non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 1 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto <> 1 " & vbCrLf)
                        End If

                    Case "1" 'solo serra (serra + non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 2 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto <> 2 " & vbCrLf)
                        End If

                    Case "0,1", "1,0" 'entrambi (non specificato)

                        stbFiltroQuery.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto = 0 " & vbCrLf)

                        If Opt_Singola_Gruppo >= 0 Then
                            stbFiltroQuery.Append("  and  " & tabAvInf & ".Flag_Protetto = 0 " & vbCrLf)
                        End If

                End Select


            End If


            If Veg_Cod <> 0 Then

                Select Case TipoRichiesto
                    Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci
                        stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetalixNormative.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                        stbFiltroQuery.Append("      OR  (FormulatixSpecieVegetalixNormative.Veg_Cod = 0 AND FormulatixSpecieVegetalixNormative.Grsp_Cod = 0 )" & vbCrLf)
                        stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetalixNormative.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                        stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
                    Case Else
                        stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetalixNormative.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                        'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
                        If TipoRichiesto = enum_TipoFormulato.Geodisinfestanti Then
                            stbFiltroQuery.Append("      OR  FormulatixSpecieVegetalixNormative.Veg_Cod = 5000336 " & vbCrLf)
                        End If
                        stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetalixNormative.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                        stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
                End Select
            End If

            If Trim(strPA) <> "" Then
                stbFiltroQuery.Append("  AND  PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "  " & vbCrLf)
                stbFiltroQuery.Append("  AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)" & vbCrLf)
            End If


            'stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            'stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)

            '(21/11/2017 fede) aggiunti dati fine scorte
            stbFiltroQuery.Append("  AND (")
            stbFiltroQuery.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
            stbFiltroQuery.Append("  OR ")
            stbFiltroQuery.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
            stbFiltroQuery.Append("      )")


            If Opt_Singola_Gruppo >= 0 Then

                tabAvInf = "FormulatixSpeciexAvversitaxNormative"
                Dim tabAvInfDosi As String = "FormulatixSpeciexAvversitaxDosixNormative"
                If Opt_Avversita_Infestanti > 0 Then

                    tabAvInfDosi = "FormulatixSpeciexInfestantixDosixNormative"
                    tabAvInf = "FormulatixSpeciexInfestantixNormative"
                End If

                '(21/11/2017 fede) aggiunti dati fine scorte
                stbFiltroQuery.Append("  AND (")
                stbFiltroQuery.Append("  (  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("     AND  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("  OR ")
                stbFiltroQuery.Append("  (  " & tabAvInf & ".DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("      )")

                stbFiltroQuery.Append("  AND (")
                stbFiltroQuery.Append("  (  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("     AND  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("  OR ")
                stbFiltroQuery.Append("  (  " & tabAvInfDosi & ".DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbFiltroQuery.Append("      )")

                'stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                'stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            End If



            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                Case 1 'prodotti commercio + revocati
                Case 2 'prodotti revocati
                    stbFiltroQuery.Append("  And   FORMULATI.Revocato= 1 " & vbCrLf)
            End Select

            If strFiltro <> "" Then
                stbFiltroQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri))
            End If

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------



            '-----------------------------------------------------------------------------------
            '--- PRODOTTI REGISTRATI ESATTAMENTE SULL'AVVERSITA / GRUPPO SELEZIONATO 
            '-----------------------------------------------------------------------------------
            stbQuery.Append(stbCteQuery)
            stbQuery.Append(stbSelectQuery)

            Select Case Opt_Avversita_Infestanti

                '------------------------------------------------------------------------------------
                '------ AVVERSITA -------------------------------------------------------------------
                '------------------------------------------------------------------------------------
                Case 0

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) As Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) As Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) As UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)



                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------

                    Select Case Opt_Singola_Gruppo

                        Case 0

                            Select Case TipoRichiesto
                                Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0 )" & vbCrLf)
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0 )" & vbCrLf)
                                Case Else
                                    If Av_Cod <> 0 Then
                                        stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                                    End If
                            End Select

                        Case 1

                            Select Case TipoRichiesto
                                Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0 )" & vbCrLf)
                                    stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0 )" & vbCrLf)
                                Case Else
                                    If Av_Gru <> 0 Then
                                        stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                                    End If
                            End Select

                    End Select



                        '------------------------------------------------------------------------------------
                        '------ INFESTANTI ------------------------------------------------------------------
                        '------------------------------------------------------------------------------------

                Case 1

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)
                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1
                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------------------------------------

                    Select Case Opt_Singola_Gruppo

                        Case 0
                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestantixNormative.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestantixNormative.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select


                Case Else

                    stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                    stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)


                    stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                    stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery1)

                    stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery2)

                    '(23/05/2016 fede) modifica x filtrare dosaggio senza avversita/gruppo nei prodotti che hanno anche altra classificazione
                    Select Case TipoRichiesto

                        Case 3, 4, 11 'Coadiuvanti, Fitoregolatori, Fisiofarmaci e Corroboranti

                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Cod = 0)" & vbCrLf)
                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversitaxNormative.Av_Gru = 0)" & vbCrLf)

                    End Select

            End Select

            stbQuery.Append(stbVariabileQuery)

            stbQuery.Append(stbFiltroQuery)

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case 3, 4 'Coadiuvanti, Fitoregolatori

                Case 6 'Disseccanti

                Case 11  'Fisiofarmaci e Corroboranti

                Case Else

                    If Opt_Singola_Gruppo <> -1 Then

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SULL'AVVERSITA FIGLIA / GRUPPO PADRE 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        ' stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)


                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "" & vbCrLf)
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SUI GRUPPI DELLA GERARCHIA DEI GRUPPI 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        'stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then

                                            '''' 295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986
                                            '''Dim strGruppi As String = "295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986"
                                            '''AvGruTot.Clear()
                                            '''Dim ArrayGru() As String = strGruppi.Split(",")
                                            '''For g = 0 To ArrayGru.Length - 1
                                            '''    If Not AvGruTot.ContainsKey(ArrayGru(g)) Then
                                            '''        AvGruTot.Add(ArrayGru(g), "" & vbCrLf)
                                            '''    End If
                                            '''Next

                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)


                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestantixNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexInfestantixNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexInfestantixNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexInfestantixNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)
                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversitaxNormative ON FormulatixSpecieVegetalixNormative.Fr_Cod = FormulatixSpeciexAvversitaxNormative.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Veg_Cod = FormulatixSpeciexAvversitaxNormative.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetalixNormative.Grsp_Cod = FormulatixSpeciexAvversitaxNormative.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosixNormative ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosixNormative.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosixNormative.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                    End If

            End Select

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES ASC" & vbCrLf)
            End If

            If Priorita = True Then
                stbQuery.Append(" ,priorita" & vbCrLf)
            End If

            stbQuery.Append(" ,DataAttoNormativo DESC" & vbCrLf)


            '_objLog.Scrivi_LOG(objParametri, NomeRoutine, stbQuery.ToOrigin)

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    '(29/11/2019 fede) temporaneamente NON filtro per prodotti ESTERI
                    If Stato_Cod = "IT" Then
                        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                        DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                    Else
                        DT = Dt_App
                    End If
                Case 1 'prodotti commercio + revocati
                    DT = Dt_App
                Case 2 'prodotti revocati
                    DT = Dt_App
            End Select

            'Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
            'DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)

            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    If DT.Rows(i).Item("Priorita") = 3 And DT.Rows(i).Item("Av_Cod") = 0 And DT.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If DT.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                DT.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If

#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            DT.TableName = "Prodotti"
            Dv.Table = DT
            Dv.Sort = "FR_DES , Priorita, Ordine "
            DT = Dv.ToTable
#Else
            'Codice per versioni < 2010
#End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiXClassificazione_PA_Dosi_Avversita(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal TestoRicerca As String = "",
                               Optional ByVal TipoRichiesto As Integer = 0,
                               Optional ByVal Veg_Cod As Integer = 0,
                               Optional ByVal strPA As String = "",
                               Optional ByVal Data As Date = AGRODATAFINE,
                               Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                               Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                               Optional ByVal Av_Gru As Integer = 0,
                               Optional ByVal Av_Cod As Integer = 0,
                               Optional ByVal strFiltro As String = "",
                               Optional ByVal strSort As String = "",
                               Optional ByVal Grfi_cod As Integer = 0,
                               Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                     Optional ByVal Stato_Cod As String = "IT"
                            ) _
                            As DataTable



        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti
        '  11 = Fisiofarmaci e Corroboranti


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi_Avversita()"

        Dim MessaggioErrore As String = ""
        Dim Dt_App As New DataTable
        Dim DT As DataTable
        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbCteQuery As New System.Text.StringBuilder
        Dim stbSelectQuery As New System.Text.StringBuilder
        Dim stbFromQuery1 As New System.Text.StringBuilder
        Dim stbFromQuery2 As New System.Text.StringBuilder
        Dim stbVariabileQuery As New System.Text.StringBuilder
        Dim stbFiltroQuery As New System.Text.StringBuilder
        Dim i, n As Integer
        Dim AvGruTmp(0) As String

        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim Priorita As Boolean = True



        Try

            'Creazione tabelle Temporanea ordinamento String_AGG
            stbCteQuery.Append("  Create Table #Ord (Fr_Cod Int, Pa_Cod Int, Pa_Des nvarchar(255), Titolo float, Peso float) " & vbCrLf)
            stbCteQuery.Append(" Insert  #Ord (Fr_Cod, Pa_Cod, Pa_Des, Titolo, Peso) " & vbCrLf)
            stbCteQuery.Append(" Select  FPA.fr_cod, FPA.Pa_Cod,  PA.Pa_Des, Titolo, Peso " & vbCrLf)
            stbCteQuery.Append(" From PrincipiAttivi PA inner Join FormulatixPrincipiAttivi FPA " & vbCrLf)
            stbCteQuery.Append(" On FPA.Pa_Cod = PA.Pa_Cod " & vbCrLf)
            stbCteQuery.Append(" Order by FPA.Fr_Cod, Titolo Desc " & vbCrLf)

            stbCteQuery.Append("  Create Table #Ord1 (for_cod Int, CLTOSS_COD nvarchar(50), GradoTossicita nvarchar(50)) " & vbCrLf)
            stbCteQuery.Append(" Insert  #Ord1 (for_cod, CLTOSS_COD, GradoTossicita) " & vbCrLf)
            stbCteQuery.Append(" Select  for_cod,   CT.CLTOSS_COD, CT.GradoTossicita From  FormulatiXClassiTossicologiche FCT1 " & vbCrLf)
            stbCteQuery.Append(" Inner Join ClasseTossicologica CT On " & vbCrLf)
            stbCteQuery.Append(" (FCT1.CLTOSS_COD=CT.CLTOSS_COD) " & vbCrLf)
            stbCteQuery.Append(" ORDER BY FCT1.FOR_COD, GradoTossicita desc " & vbCrLf)






            '--------------------------------------------------------------------------
            '------ SELECT LIST COMUNE 
            '--------------------------------------------------------------------------

            stbSelectQuery.Append(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.Data_Reg, Formulati.Data_Modifica ," & vbCrLf)
            stbSelectQuery.Append(" FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Revocato, Formulati.Data_Revo, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Sospeso, Formulati.Data_Sosp, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Termine, Formulati.Data_Term, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " & vbCrLf)

            stbSelectQuery.Append(" Formulati.Verificato_Flag, " & vbCrLf)

            stbSelectQuery.Append(" (SELECT  TOP 1 DataAttoNormativo " & vbCrLf)
            stbSelectQuery.Append(" FROM FormulatiXAllegatiNormative  " & vbCrLf)
            stbSelectQuery.Append(" Where FormulatiXAllegatiNormative.FOR_COD = FORMULATI.FR_COD " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY DataAttoNormativo DESC) as DataAttoNormativo, " & vbCrLf)

            stbSelectQuery.Append("(SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" FROM     dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("        dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Cod, " & vbCrLf)

            stbSelectQuery.Append("ISNULL((SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Des " & vbCrLf)
            stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC),'') as Pa_Des, " & vbCrLf)

            stbSelectQuery.Append("(SELECT  TOP 1 dbo.FormulatixPrincipiAttivi.Titolo " & vbCrLf)
            stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Titolo, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_COD,'') AS strCLTOSS_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_Grado,'') AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.Epoca_Cod,0) AS Epoca_Cod, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.BufferZone_Min,0) AS BufferZone_Min, ")
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.BufferZone_Max,0) AS BufferZone_Max, ")

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.TempoCarenza,0) AS TempoCarenza  " & vbCrLf)

            '--------------------------------------------------------------------------
            '------ FROM COMUNE 
            '--------------------------------------------------------------------------

            stbFromQuery1.Append(" FROM  Formulati INNER JOIN " & vbCrLf)
            If Stato_Cod <> "" Then
                stbFromQuery1.Append("   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' INNER JOIN " & vbCrLf)
            End If
            stbFromQuery1.Append("   FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   FormulatixSpecieVegetali ON Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod  " & vbCrLf)

            If Trim(strPA) <> "" Then
                stbFromQuery1.Append(" INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN " & vbCrLf)
                stbFromQuery1.Append("   PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------

            stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)
            stbFromQuery2.Append("      Select For_Cod, " & vbCrLf)
            stbFromQuery2.AppendLine("    String_AGG(CLTOSS_COD, ',') as [strCLTOSS_COD]," & vbCrLf)
            stbFromQuery2.AppendLine("    String_AGG(GradoTossicita, ',') as [strCLTOSS_Grado] ")
            stbFromQuery2.AppendLine("    FROM ( Select #Ord1.For_Cod, #Ord1.CLTOSS_COD, #ord1.GradoTossicita From #Ord1  " & vbCrLf)
            stbFromQuery2.AppendLine("    	       ) [MainPrincipiCodici] Group By For_Cod   " & vbCrLf)
            stbFromQuery2.Append("     )" & vbCrLf)
            stbFromQuery2.Append("   ClassiCodici on ClassiCodici.FOR_COD=Formulati.FR_COD  " & vbCrLf)


            '------------------------------------
            stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)

            stbFromQuery2.AppendLine(" 	Select Fr_Cod,      ")
            stbFromQuery2.AppendLine("  String_AGG(Pa_cod, '|') as [strPA_COD],    ")
            stbFromQuery2.AppendLine("  String_AGG(Pa_Des, '|') as [strPA_DES],     ")
            stbFromQuery2.AppendLine("  String_AGG(Titolo, '|') as [strTITOLI],    ")
            stbFromQuery2.AppendLine("  String_AGG(Peso, '|') as [strPESI]     ")
            stbFromQuery2.AppendLine("  FROM ( Select #Ord.Fr_Cod, #Ord.Pa_Cod, #ord.Pa_Des, #Ord.Titolo, #Ord.Peso From #Ord " & vbCrLf)
            stbFromQuery2.AppendLine("    	 ) [MainPrincipiCodici] Group By Fr_Cod " & vbCrLf)
            stbFromQuery2.Append("   )      " & vbCrLf)
            stbFromQuery2.Append("   PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)
            '------------------------------------


            stbFromQuery2.Append(" WHERE   1 = 1 " & vbCrLf)

            If TestoRicerca <> "" Then
                stbFromQuery2.Append(" AND (Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%') " & vbCrLf)
            End If

            stbFromQuery2.Append(" AND      Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Data) & "  " & vbCrLf)




            '--------------------------------------------------------------------------
            '------ FILTRO COMUNE 
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case enum_TipoFormulato.Tutti

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  " & vbCrLf)


                Case enum_TipoFormulato.Coadiuvanti

                    stbFiltroQuery.Append("  And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  " & vbCrLf)

                Case enum_TipoFormulato.Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 608  " & vbCrLf)

                Case enum_TipoFormulato.Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (603, 609)   " & vbCrLf)

                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestanti.Av_Cod=0 " & vbCrLf)
                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestanti.Av_Gru=0 " & vbCrLf)


                Case enum_TipoFormulato.Geodisinfestanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 607  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) " & vbCrLf)

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603,609) " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) " & vbCrLf)

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607,  606, 400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale
                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    stbFiltroQuery.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)

            End Select

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If Grfi_cod <> 0 Then
                stbFiltroQuery.Append("  and ( FormulatixSpecieVegetali.grfi_cod = 0 " & vbCrLf)
                stbFiltroQuery.Append("  OR (FormulatixSpecieVegetali.grfi_cod <> 0 and  " & vbCrLf)
                stbFiltroQuery.Append("     FormulatixSpecieVegetali.grfi_cod = " & Grfi_cod & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)

                If Opt_Singola_Gruppo >= 0 Then

                    Dim tabAvInf As String = "FormulatixSpeciexAvversitaxDosi"
                    If Opt_Avversita_Infestanti > 0 Then
                        tabAvInf = "FormulatixSpeciexInfestantixDosi"
                    End If

                    stbFiltroQuery.Append("  and ( " & tabAvInf & ".grfi_cod = 0 " & vbCrLf)
                    stbFiltroQuery.Append("  OR (" & tabAvInf & ".grfi_cod <> 0 and  " & vbCrLf)
                    stbFiltroQuery.Append("     " & tabAvInf & ".grfi_cod = " & Grfi_cod & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)

                End If

            End If

            If Veg_Cod <> 0 Then
                stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
                If TipoRichiesto = 7 Then
                    stbFiltroQuery.Append("      OR  FormulatixSpecieVegetali.Veg_Cod = 5000336 " & vbCrLf)
                End If
                stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetali.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
            End If

            If Trim(strPA) <> "" Then
                stbFiltroQuery.Append("  AND  PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "  " & vbCrLf)
                stbFiltroQuery.Append("  AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)" & vbCrLf)
            End If

            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            If Opt_Singola_Gruppo >= 0 Then


                Dim tabAvInf As String = "FormulatixSpeciexAvversita"
                Dim tabAvInfDosi As String = "FormulatixSpeciexAvversitaxDosi"
                If Opt_Avversita_Infestanti > 0 Then
                    tabAvInfDosi = "FormulatixSpeciexInfestantixDosi"
                    tabAvInf = "FormulatixSpeciexInfestanti"
                End If

                stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            End If



            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                Case 1 'prodotti commercio + revocati
                Case 2 'prodotti revocati
                    stbFiltroQuery.Append("  And   FORMULATI.Revocato= 1 " & vbCrLf)
            End Select

            If strFiltro <> "" Then
                stbFiltroQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri))
            End If

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------



            '-----------------------------------------------------------------------------------
            '--- PRODOTTI REGISTRATI ESATTAMENTE SULL'AVVERSITA / GRUPPO SELEZIONATO 
            '-----------------------------------------------------------------------------------
            stbQuery.Append(stbCteQuery)

            stbQuery.Append(stbSelectQuery)

            Select Case Opt_Avversita_Infestanti

                '------------------------------------------------------------------------------------
                '------ AVVERSITA -------------------------------------------------------------------
                '------------------------------------------------------------------------------------
                Case 0

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) As Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) As Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) As UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)



                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------

                    Select Case Opt_Singola_Gruppo

                        Case 0

                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select



                    '------------------------------------------------------------------------------------
                    '------ INFESTANTI ------------------------------------------------------------------
                    '------------------------------------------------------------------------------------

                Case 1

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)
                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1
                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------------------------------------

                    Select Case Opt_Singola_Gruppo

                        Case 0
                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestanti.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestanti.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select


                Case Else

                    stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                    stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)


                    stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                    stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery1)

                    stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery2)

                    '(23/05/2016 fede) modifica x filtrare dosaggio senza avversita/gruppo nei prodotti che hanno anche altra classificazione
                    Select Case TipoRichiesto

                        Case 3, 4, 11 'Coadiuvanti, Fitoregolatori, Fisiofarmaci e Corroboranti

                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Cod = 0)" & vbCrLf)
                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Gru = 0)" & vbCrLf)

                    End Select

            End Select

            stbQuery.Append(stbVariabileQuery)

            stbQuery.Append(stbFiltroQuery)

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case 3, 4 'Coadiuvanti, Fitoregolatori

                Case 6 'Disseccanti

                Case 11  'Fisiofarmaci e Corroboranti

                Case Else

                    If Opt_Singola_Gruppo <> -1 Then

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SULL'AVVERSITA FIGLIA / GRUPPO PADRE 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        ' stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)


                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "" & vbCrLf)
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SUI GRUPPI DELLA GERARCHIA DEI GRUPPI 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        'stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then

                                            '''' 295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986
                                            '''Dim strGruppi As String = "295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986"
                                            '''AvGruTot.Clear()
                                            '''Dim ArrayGru() As String = strGruppi.Split(",")
                                            '''For g = 0 To ArrayGru.Length - 1
                                            '''    If Not AvGruTot.ContainsKey(ArrayGru(g)) Then
                                            '''        AvGruTot.Add(ArrayGru(g), "" & vbCrLf)
                                            '''    End If
                                            '''Next

                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)


                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)
                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                    End If

            End Select

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES ASC" & vbCrLf)
            End If

            If Priorita = True Then
                stbQuery.Append(" ,priorita" & vbCrLf)
            End If

            stbQuery.Append(" DROP Table #Ord" & vbCrLf)
            stbQuery.Append(" DROP Table #Ord1 ")

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Case 1 'prodotti commercio + revocati
                    DT = Dt_App
                Case 2 'prodotti revocati
                    DT = Dt_App
            End Select


            'Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
            'DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)

            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    If DT.Rows(i).Item("Priorita") = 3 And DT.Rows(i).Item("Av_Cod") = 0 And DT.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If DT.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                DT.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If


#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            DT.TableName = "Prodotti"
            Dv.Table = DT
            Dv.Sort = "FR_DES , Priorita, Ordine "
            DT = Dv.ToTable
#Else
            'Codice per versioni < 2010
#End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    Public Function LeggiXClassificazione_PA_Dosi_Avversita_XML_Path(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal TestoRicerca As String = "",
                               Optional ByVal TipoRichiesto As Integer = 0,
                               Optional ByVal Veg_Cod As Integer = 0,
                               Optional ByVal strPA As String = "",
                               Optional ByVal Data As Date = AGRODATAFINE,
                               Optional ByVal Opt_Avversita_Infestanti As Integer = 0,
                               Optional ByVal Opt_Singola_Gruppo As Integer = 0,
                               Optional ByVal Av_Gru As Integer = 0,
                               Optional ByVal Av_Cod As Integer = 0,
                               Optional ByVal strFiltro As String = "",
                               Optional ByVal strSort As String = "",
                               Optional ByVal Grfi_cod As Integer = 0,
                               Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                     Optional ByVal Stato_Cod As String = "IT"
                            ) _
                            As DataTable



        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti
        '  11 = Fisiofarmaci e Corroboranti


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.LeggiXClassificazione_PA_Dosi_Avversita()"

        Dim MessaggioErrore As String = ""
        Dim Dt_App As New DataTable
        Dim DT As DataTable
        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbSelectQuery As New System.Text.StringBuilder
        Dim stbFromQuery1 As New System.Text.StringBuilder
        Dim stbFromQuery2 As New System.Text.StringBuilder
        Dim stbVariabileQuery As New System.Text.StringBuilder
        Dim stbFiltroQuery As New System.Text.StringBuilder
        Dim i, n As Integer
        Dim AvGruTmp(0) As String

        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim Priorita As Boolean = True



        Try

            '--------------------------------------------------------------------------
            '------ SELECT LIST COMUNE 
            '--------------------------------------------------------------------------

            stbSelectQuery.Append(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.Data_Reg, Formulati.Data_Modifica ," & vbCrLf)
            stbSelectQuery.Append(" FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Revocato, Formulati.Data_Revo, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Sospeso, Formulati.Data_Sosp, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Termine, Formulati.Data_Term, " & vbCrLf)
            stbSelectQuery.Append(" Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " & vbCrLf)

            stbSelectQuery.Append(" Formulati.Verificato_Flag, " & vbCrLf)

            stbSelectQuery.Append(" (SELECT  TOP 1 DataAttoNormativo " & vbCrLf)
            stbSelectQuery.Append(" FROM FormulatiXAllegatiNormative  " & vbCrLf)
            stbSelectQuery.Append(" Where FormulatiXAllegatiNormative.FOR_COD = FORMULATI.FR_COD " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY DataAttoNormativo DESC) as DataAttoNormativo, " & vbCrLf)

            stbSelectQuery.Append("(SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" FROM     dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("        dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Cod, " & vbCrLf)

            stbSelectQuery.Append("ISNULL((SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Des " & vbCrLf)
            stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC),'') as Pa_Des, " & vbCrLf)

            stbSelectQuery.Append("(SELECT  TOP 1 dbo.FormulatixPrincipiAttivi.Titolo " & vbCrLf)
            stbSelectQuery.Append(" FROM    dbo.FormulatixPrincipiAttivi INNER JOIN " & vbCrLf)
            stbSelectQuery.Append("         dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " & vbCrLf)
            stbSelectQuery.Append(" Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " & vbCrLf)
            stbSelectQuery.Append(" ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Titolo, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_COD,'') AS strCLTOSS_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(ClassiCodici.strCLTOSS_Grado,'') AS strCLTOSS_Grado, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
            stbSelectQuery.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.Epoca_Cod,0) AS Epoca_Cod, " & vbCrLf)

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.BufferZone_Min,0) AS BufferZone_Min, ")
            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.BufferZone_Max,0) AS BufferZone_Max, ")

            stbSelectQuery.Append(" ISNULL(FormulatixSpecieVegetali.TempoCarenza,0) AS TempoCarenza  " & vbCrLf)

            '--------------------------------------------------------------------------
            '------ FROM COMUNE 
            '--------------------------------------------------------------------------

            stbFromQuery1.Append(" FROM  Formulati INNER JOIN " & vbCrLf)
            If Stato_Cod <> "" Then
                stbFromQuery1.Append("   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' INNER JOIN " & vbCrLf)
            End If
            stbFromQuery1.Append("   FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod INNER JOIN " & vbCrLf)
            stbFromQuery1.Append("   FormulatixSpecieVegetali ON Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod  " & vbCrLf)

            If Trim(strPA) <> "" Then
                stbFromQuery1.Append(" INNER JOIN FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN " & vbCrLf)
                stbFromQuery1.Append("   PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------

            stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)
            stbFromQuery2.Append("      SELECT  MainClassiCodici.for_cod, Left(MainClassiCodici.strCLTOSS_COD,Len(MainClassiCodici.strCLTOSS_COD)-1) As strCLTOSS_COD, Left(MainClassiCodici.strCLTOSS_Grado,Len(MainClassiCodici.strCLTOSS_Grado)-1) As strCLTOSS_Grado " & vbCrLf)
            stbFromQuery2.Append("      FROM (  " & vbCrLf)
            stbFromQuery2.Append("          Select distinct FCT2.FOR_COD,            " & vbCrLf)

            stbFromQuery2.Append("              (Select CONVERT (nvarchar, FCT1.CLTOSS_COD)  + ',' AS [text()]" & vbCrLf)
            stbFromQuery2.Append("               From FormulatiXClassiTossicologiche FCT1, ClasseTossicologica CT" & vbCrLf)
            stbFromQuery2.Append("               Where FCT1.FOR_COD = FCT2.FOR_COD" & vbCrLf)
            stbFromQuery2.Append("               and FCT1.CLTOSS_COD=CT.CLTOSS_COD" & vbCrLf)
            stbFromQuery2.Append("               ORDER BY FCT1.FOR_COD, GradoTossicita desc" & vbCrLf)
            stbFromQuery2.Append("               For XML PATH ('')" & vbCrLf)
            stbFromQuery2.Append("               ) [strCLTOSS_COD]," & vbCrLf)

            stbFromQuery2.Append("              (Select CONVERT (nvarchar, CT.GradoTossicita)  + ',' AS [text()]" & vbCrLf)
            stbFromQuery2.Append("               From FormulatiXClassiTossicologiche FCT1, ClasseTossicologica CT" & vbCrLf)
            stbFromQuery2.Append("               Where FCT1.FOR_COD = FCT2.FOR_COD" & vbCrLf)
            stbFromQuery2.Append("               and FCT1.CLTOSS_COD=CT.CLTOSS_COD" & vbCrLf)
            stbFromQuery2.Append("               ORDER BY FCT1.FOR_COD, GradoTossicita desc" & vbCrLf)
            stbFromQuery2.Append("               For XML PATH ('')" & vbCrLf)
            stbFromQuery2.Append("               ) [strCLTOSS_Grado]" & vbCrLf)

            stbFromQuery2.Append("        From FormulatiXClassiTossicologiche FCT2" & vbCrLf)
            stbFromQuery2.Append("        ) [MainClassiCodici] " & vbCrLf)
            stbFromQuery2.Append("     )" & vbCrLf)
            stbFromQuery2.Append("   ClassiCodici on ClassiCodici.FOR_COD=Formulati.FR_COD  " & vbCrLf)


            '------------------------------------
            stbFromQuery2.Append("   LEFT JOIN ( " & vbCrLf)

            stbFromQuery2.Append("   SELECT  MainPrincipiCodici.fr_cod,  " & vbCrLf)
            stbFromQuery2.Append("   Left(MainPrincipiCodici.strPA_COD,Len(MainPrincipiCodici.strPA_COD)-1) As strPA_COD,  " & vbCrLf)
            stbFromQuery2.Append("   Left(MainPrincipiCodici.strPA_DES,Len(MainPrincipiCodici.strPA_DES)-1) As strPA_DES,  " & vbCrLf)
            stbFromQuery2.Append("   Left(MainPrincipiCodici.strTITOLI,Len(MainPrincipiCodici.strTITOLI)-1) As strTITOLI " & vbCrLf)

            stbFromQuery2.Append("   FROM	(              Select distinct FPA2.FR_COD,  " & vbCrLf)
            stbFromQuery2.Append("   		(Select CONVERT (nvarchar, FPA1.PA_COD)  + '|' AS [text()]   " & vbCrLf)
            stbFromQuery2.Append("   	 From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA              " & vbCrLf)
            stbFromQuery2.Append("   	 Where FPA1.FR_COD = FPA2.FR_COD                  " & vbCrLf)
            stbFromQuery2.Append("   	 and FPA1.PA_COD=PA.PA_COD                  " & vbCrLf)
            stbFromQuery2.Append("   	 ORDER BY FPA1.FR_COD, titolo desc                  " & vbCrLf)
            stbFromQuery2.Append("   	 For XML PATH ('')                 ) [strPA_COD],             " & vbCrLf)

            stbFromQuery2.Append("   		 (Select CONVERT (nvarchar, PA.PA_DES)  + '|' AS [text()]   " & vbCrLf)
            stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA            " & vbCrLf)
            stbFromQuery2.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD         " & vbCrLf)
            stbFromQuery2.Append("   	  and FPA1.PA_COD=PA.PA_COD            " & vbCrLf)
            stbFromQuery2.Append("   	  ORDER BY FPA1.FR_COD, titolo desc            " & vbCrLf)
            stbFromQuery2.Append("   	  For XML PATH ('')                 ) [strPA_DES],   " & vbCrLf)

            stbFromQuery2.Append("   		 (Select CONVERT (nvarchar, FPA1.Titolo)  + '|' AS [text()]     " & vbCrLf)
            stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA1 " & vbCrLf)
            stbFromQuery2.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD     " & vbCrLf)
            stbFromQuery2.Append("   	  ORDER BY FPA1.FR_COD , titolo desc " & vbCrLf)
            stbFromQuery2.Append("   	  For XML PATH ('')                 ) [strTITOLI]	 " & vbCrLf)

            stbFromQuery2.Append("   	  From FormulatixPrincipiAttivi FPA2        " & vbCrLf)

            stbFromQuery2.Append("   	 ) [MainPrincipiCodici]          " & vbCrLf)
            stbFromQuery2.Append("   )      " & vbCrLf)
            stbFromQuery2.Append("   PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)
            '------------------------------------


            stbFromQuery2.Append(" WHERE     ")
            If TestoRicerca <> "" Then
                stbFromQuery2.Append(" (Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%') AND " & vbCrLf)
            End If
            stbFromQuery2.Append("  Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Data) & "  " & vbCrLf)




            '--------------------------------------------------------------------------
            '------ FILTRO COMUNE 
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case enum_TipoFormulato.Tutti

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) " & vbCrLf)

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  " & vbCrLf)


                Case enum_TipoFormulato.Coadiuvanti

                    stbFiltroQuery.Append("  And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  " & vbCrLf)

                Case enum_TipoFormulato.Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 608  " & vbCrLf)

                Case enum_TipoFormulato.Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (603, 609)   " & vbCrLf)

                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestanti.Av_Cod=0 " & vbCrLf)
                    stbFiltroQuery.Append("  And  FormulatixSpeciexInfestanti.Av_Gru=0 " & vbCrLf)


                Case enum_TipoFormulato.Geodisinfestanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 607  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) " & vbCrLf)

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603,609) " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) " & vbCrLf)

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607,  606, 400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    stbFiltroQuery.Append("  And  ClassificazioniFormulati.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale
                    stbFiltroQuery.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    stbFiltroQuery.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)

            End Select

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If Grfi_cod <> 0 Then
                stbFiltroQuery.Append("  and ( FormulatixSpecieVegetali.grfi_cod = 0 " & vbCrLf)
                stbFiltroQuery.Append("  OR (FormulatixSpecieVegetali.grfi_cod <> 0 and  " & vbCrLf)
                stbFiltroQuery.Append("     FormulatixSpecieVegetali.grfi_cod = " & Grfi_cod & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)
                stbFiltroQuery.Append(" ) " & vbCrLf)

                If Opt_Singola_Gruppo >= 0 Then

                    Dim tabAvInf As String = "FormulatixSpeciexAvversitaxDosi"
                    If Opt_Avversita_Infestanti > 0 Then
                        tabAvInf = "FormulatixSpeciexInfestantixDosi"
                    End If

                    stbFiltroQuery.Append("  and ( " & tabAvInf & ".grfi_cod = 0 " & vbCrLf)
                    stbFiltroQuery.Append("  OR (" & tabAvInf & ".grfi_cod <> 0 and  " & vbCrLf)
                    stbFiltroQuery.Append("     " & tabAvInf & ".grfi_cod = " & Grfi_cod & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)
                    stbFiltroQuery.Append(" ) " & vbCrLf)

                End If

            End If

            If Veg_Cod <> 0 Then
                stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
                If TipoRichiesto = 7 Then
                    stbFiltroQuery.Append("      OR  FormulatixSpecieVegetali.Veg_Cod = 5000336 " & vbCrLf)
                End If
                stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetali.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
            End If

            If Trim(strPA) <> "" Then
                stbFiltroQuery.Append("  AND  PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & "  " & vbCrLf)
                stbFiltroQuery.Append("  AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA, False) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)" & vbCrLf)
            End If

            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            stbFiltroQuery.Append("  AND  FormulatixSpecieVegetali.validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            If Opt_Singola_Gruppo >= 0 Then


                Dim tabAvInf As String = "FormulatixSpeciexAvversita"
                Dim tabAvInfDosi As String = "FormulatixSpeciexAvversitaxDosi"
                If Opt_Avversita_Infestanti > 0 Then
                    tabAvInfDosi = "FormulatixSpeciexInfestantixDosi"
                    tabAvInf = "FormulatixSpeciexInfestanti"
                End If

                stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInf & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbFiltroQuery.Append("  And  " & tabAvInfDosi & ".validita_fine>=" & Agro_SQL_SaveDate(Data) & vbCrLf)


            End If



            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                Case 1 'prodotti commercio + revocati
                Case 2 'prodotti revocati
                    stbFiltroQuery.Append("  And   FORMULATI.Revocato= 1 " & vbCrLf)
            End Select

            If strFiltro <> "" Then
                stbFiltroQuery.Append(Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri))
            End If

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------



            '-----------------------------------------------------------------------------------
            '--- PRODOTTI REGISTRATI ESATTAMENTE SULL'AVVERSITA / GRUPPO SELEZIONATO 
            '-----------------------------------------------------------------------------------
            stbQuery.Append(stbSelectQuery)

            Select Case Opt_Avversita_Infestanti

                '------------------------------------------------------------------------------------
                '------ AVVERSITA -------------------------------------------------------------------
                '------------------------------------------------------------------------------------
                Case 0

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) As Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(Decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) As Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) As UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)



                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------

                    Select Case Opt_Singola_Gruppo

                        Case 0

                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select



                    '------------------------------------------------------------------------------------
                    '------ INFESTANTI ------------------------------------------------------------------
                    '------------------------------------------------------------------------------------

                Case 1

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1

                            stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                        Case Else

                            stbVariabileQuery.Append(", 0 AS Dose_Min, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS Dose_Max, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD, " & vbCrLf)
                            stbVariabileQuery.Append(" 'N' AS UDM_SIM, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS ff_cod, 0 AS da_ff_cod, 0 AS a_ff_cod, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS da_epoca_1, 0 AS a_epoca_1, 0 AS da_epoca_2, 0 AS a_epoca_2, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS acqua_min, 0 AS acqua_max, 0 AS acqua_udm_cod, 'N' AS acqua_udm_sim, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 AS LimiteInterventi, " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS UDM_COD_Limite, 'N' AS udm_sim_Limite, " & vbCrLf)

                            stbVariabileQuery.Append(" ROW_NUMBER() OVER(ORDER BY Formulati.FR_des ASC) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                            stbVariabileQuery.Append(" 0 as Av_Cod, 0 as Av_gru, " & vbCrLf)
                            stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Min , " & vbCrLf)
                            stbVariabileQuery.Append(" 0 AS IntervalloTrattamenti_Max  " & vbCrLf)
                    End Select

                    stbVariabileQuery.Append(stbFromQuery1)

                    Select Case Opt_Singola_Gruppo

                        Case 0, 1
                            stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                            stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                            stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    End Select

                    stbVariabileQuery.Append(stbFromQuery2)

                    '---------------------------------------

                    Select Case Opt_Singola_Gruppo

                        Case 0
                            If Av_Cod <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestanti.Av_Cod= " & Av_Cod & "  )" & vbCrLf)
                            End If
                        Case 1
                            If Av_Gru <> 0 Then
                                stbVariabileQuery.Append(" AND (FormulatixSpeciexInfestanti.Av_Gru= " & Av_Gru & "  )" & vbCrLf)
                            End If
                    End Select


                Case Else

                    stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                    stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                    stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)


                    stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                    stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)


                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                    stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery1)

                    stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                    stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                    stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                    stbVariabileQuery.Append(stbFromQuery2)

                    '(23/05/2016 fede) modifica x filtrare dosaggio senza avversita/gruppo nei prodotti che hanno anche altra classificazione
                    Select Case TipoRichiesto

                        Case 3, 4, 11 'Coadiuvanti, Fitoregolatori, Fisiofarmaci e Corroboranti

                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Cod = 0)" & vbCrLf)
                            stbVariabileQuery.Append(" AND (FormulatixSpeciexAvversita.Av_Gru = 0)" & vbCrLf)

                    End Select

            End Select

            stbQuery.Append(stbVariabileQuery)

            stbQuery.Append(stbFiltroQuery)

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            Select Case TipoRichiesto

                Case 3, 4 'Coadiuvanti, Fitoregolatori

                Case 6 'Disseccanti

                Case 11  'Fisiofarmaci e Corroboranti

                Case Else

                    If Opt_Singola_Gruppo <> -1 Then

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SULL'AVVERSITA FIGLIA / GRUPPO PADRE 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        ' stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & "  )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 2 as priorita, 0 AS Ordine, " & vbCrLf)


                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvGru As DataTable
                                            DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvGru.Rows.Count - 1
                                                If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                                                    AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "" & vbCrLf)
                                                    strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                                                End If
                                            Next
                                            If strAvGru <> "" Then
                                                strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                            Dim DtAvCod As DataTable
                                            DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri)
                                            For i = 0 To DtAvCod.Rows.Count - 1
                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                                                    AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "" & vbCrLf)
                                                    strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                                                End If
                                            Next
                                            If strAvCod <> "" Then
                                                strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & " )" & vbCrLf)
                                            Else
                                                stbVariabileQuery.Length = 0
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------
                        '--------------------------------------------------------------------------

                        'stbQuery.Append(" UNION " & vbCrLf)

                        '-----------------------------------------------------------------------------------
                        '--- PRODOTTI REGISTRATI SUI GRUPPI DELLA GERARCHIA DEI GRUPPI 
                        '--- DELL'AVVERSITA / GRUPPO SELEZIONATO 
                        '-----------------------------------------------------------------------------------

                        'stbQuery.Append(stbSelectQuery)

                        stbVariabileQuery.Length = 0

                        Select Case Opt_Avversita_Infestanti

                            '------------------------------------------------------------------------------------
                            '------ AVVERSITA -------------------------------------------------------------------
                            '------------------------------------------------------------------------------------
                            Case 0

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then

                                            '''' 295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986
                                            '''Dim strGruppi As String = "295,299,304,320,327,365,365,381,383,396,458,463,463,468,475,496,496,537,537,556,577,577,600,618,618,618,652,652,668,681,685,720,981,981,982,982,986,986"
                                            '''AvGruTot.Clear()
                                            '''Dim ArrayGru() As String = strGruppi.Split(",")
                                            '''For g = 0 To ArrayGru.Length - 1
                                            '''    If Not AvGruTot.ContainsKey(ArrayGru(g)) Then
                                            '''        AvGruTot.Add(ArrayGru(g), "" & vbCrLf)
                                            '''    End If
                                            '''Next

                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)


                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexAvversita.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select


                                '------------------------------------------------------------------------------------
                                '------ INFESTANTI ------------------------------------------------------------------
                                '------------------------------------------------------------------------------------

                            Case 1

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexInfestanti.Av_Cod, FormulatixSpeciexInfestanti.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 3 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexInfestantixDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexInfestanti.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexInfestanti.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexInfestanti.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestantixDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexInfestantixDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexInfestantixDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexInfestantixDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)
                                '---------------------------------------

                                Select Case Opt_Singola_Gruppo

                                    Case 0
                                        If Av_Cod <> 0 Then
                                            If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                                                Dim ciclo As Integer
                                                For Each ciclo In AvGruTot.Keys
                                                    If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                                                        AvGruTot1.Add(ciclo.ToString, "" & vbCrLf)
                                                    End If
                                                    ReDim strGerarchiaAvGru(0)
                                                    ReDim strGerarchiaAvGruOrdine(0)
                                                    GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                        For n = 0 To strGerarchiaAvGru.Length - 1
                                                            If strGerarchiaAvGru(n) <> 0 Then
                                                                If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                                                    AvGruTot1.Add(strGerarchiaAvGru(n), "" & vbCrLf)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                                Dim ciclo1 As Integer
                                                If strAvGru <> "" Then
                                                    strAvGru &= ","
                                                End If
                                                For Each ciclo1 In AvGruTot1.Keys
                                                    strAvGru &= ciclo1 & ","
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                    Case 1
                                        If Av_Gru <> 0 Then
                                            ReDim strGerarchiaAvGru(0)
                                            ReDim strGerarchiaAvGruOrdine(0)
                                            GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                                            If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                                                For n = 0 To strGerarchiaAvGru.Length - 1
                                                    If strGerarchiaAvGru(n) <> 0 Then
                                                        strAvGru &= strGerarchiaAvGru(n) & ","
                                                    End If
                                                Next
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                    stbVariabileQuery.Append(" AND FormulatixSpeciexInfestanti.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & " )" & vbCrLf)
                                                Else
                                                    stbVariabileQuery.Length = 0
                                                End If
                                            End If
                                        End If
                                End Select

                            Case Else

                                stbVariabileQuery.Append(", convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Min,0)) AS Dose_Min, " & vbCrLf)
                                stbVariabileQuery.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosi.Dose_Max,0)) AS Dose_Max, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD,0) AS UDM_COD, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(ff_cod,0) AS ff_cod, ISNULL(da_ff_cod,0) AS da_ff_cod, ISNULL(a_ff_cod,0) AS a_ff_cod, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(da_epoca_1,0) AS da_epoca_1, ISNULL(a_epoca_1,0) AS a_epoca_1, ISNULL(da_epoca_2,0) AS da_epoca_2, ISNULL(a_epoca_2,0) AS a_epoca_2, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(acqua_min,0) AS acqua_min, ISNULL(acqua_max,0) AS acqua_max, ISNULL(acqua_udm_cod,0) AS acqua_udm_cod, ISNULL(UnitaMisura1.UDM_sim ,'') AS acqua_udm_sim, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.LimiteInterventi,0) AS LimiteInterventi, " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ISNULL(UnitaMisura2.UDM_sim ,'') AS udm_sim_Limite, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(For_Veg_Av_Dos_Cod,0) AS For_Veg_Av_Dos_Cod, " & vbCrLf)

                                stbVariabileQuery.Append(" FormulatixSpeciexAvversita.Av_Cod, FormulatixSpeciexAvversita.Av_gru, " & vbCrLf)
                                stbVariabileQuery.Append(" 1 as priorita, 0 AS Ordine, " & vbCrLf)

                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , " & vbCrLf)
                                stbVariabileQuery.Append(" ISNULL(FormulatixSpeciexAvversitaxDosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max  " & vbCrLf)


                                stbVariabileQuery.Append(stbFromQuery1)

                                stbVariabileQuery.Append("   INNER JOIN FormulatixSpeciexAvversita ON FormulatixSpecieVegetali.Fr_Cod = FormulatixSpeciexAvversita.Fr_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Veg_Cod = FormulatixSpeciexAvversita.Veg_Cod AND  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpecieVegetali.Grsp_Cod = FormulatixSpeciexAvversita.Grsp_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura RIGHT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversitaxDosi ON UnitaMisura.UDM_COD = FormulatixSpeciexAvversitaxDosi.UDM_COD ON  " & vbCrLf)
                                stbVariabileQuery.Append("   FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura1 ON FormulatixSpeciexAvversitaxDosi.acqua_udm_cod = UnitaMisura1.UDM_COD LEFT OUTER JOIN " & vbCrLf)
                                stbVariabileQuery.Append("   UnitaMisura AS UnitaMisura2 ON FormulatixSpeciexAvversitaxDosi.UDM_COD_Limite = UnitaMisura2.UDM_COD " & vbCrLf)

                                stbVariabileQuery.Append(stbFromQuery2)


                        End Select

                        '(01/04/2015 fede modifica x non fare la union se non esistono figli/padri diretti)
                        If stbVariabileQuery.Length <> 0 Then
                            stbQuery.Append(" UNION " & vbCrLf)
                            stbQuery.Append(stbSelectQuery)
                            stbQuery.Append(stbVariabileQuery)
                            stbQuery.Append(stbFiltroQuery)
                        End If

                        'stbQuery.Append(stbVariabileQuery)
                        'stbQuery.Append(stbFiltroQuery)

                    End If

            End Select

            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES ASC" & vbCrLf)
            End If

            If Priorita = True Then
                stbQuery.Append(" ,priorita" & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '(12/11/2015) aggiunto filtro revoche
            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Case 1 'prodotti commercio + revocati
                    DT = Dt_App
                Case 2 'prodotti revocati
                    DT = Dt_App
            End Select


            'Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
            'DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)

            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    If DT.Rows(i).Item("Priorita") = 3 And DT.Rows(i).Item("Av_Cod") = 0 And DT.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If DT.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                DT.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If


#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            DT.TableName = "Prodotti"
            Dv.Table = DT
            Dv.Sort = "FR_DES , Priorita, Ordine "
            DT = Dv.ToTable
#Else
            'Codice per versioni < 2010
#End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    Public Sub GerarchiaGruppiAvversita(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal Av_Gru As Integer,
                                             ByRef strAv_Gru() As String,
                                             ByRef strAv_Gru_Des() As String,
                                             ByRef strAv_Gru_Des_Lat() As String,
                                             ByRef strAv_Gru_Ordine() As String)

        ' TODO: aggiungere qui la logica del test
        Dim grafo As New Agronica.Helpers.Graphs.grafo
        Dim N As Integer = 0

        grafo.ConnectionString = objParametri.StringaConnessione 'ConnectionString 'ConfigurationManager.ConnectionStrings("cn").ConnectionString
        grafo.QueryVertici = "SELECT av_GRU as id, Av_GRU_DES as descrizione,Av_Gru_Des_Lat as descrizione_Latina, LivelloDiStopDiscesa FROM GruppoAvversita"
        grafo.QueryArchi = "SELECT av_gru_da AS vertice1, av_gru_a AS vertice2, Livello AS Peso FROM GruppoAvversitaXGruppoAvversita"
        'grafo.GrafoOrientato = False

        Dim Rval As String = ""


        'Debug.Print("input: " & 2)
        For Each a As Vertice In
            grafo.CalcolaPercorsi(Av_Gru)
            ReDim Preserve strAv_Gru(N)
            ReDim Preserve strAv_Gru_Des(N)
            ReDim Preserve strAv_Gru_Des_Lat(N)
            ReDim Preserve strAv_Gru_Ordine(N)
            strAv_Gru(N) = a.Name
            strAv_Gru_Des(N) = a.Descrizione
            strAv_Gru_Des_Lat(N) = a.Descrizione_Latina
            strAv_Gru_Ordine(N) = N
            N += 1
            'strAv_Gru &= a.Name & ","
            'strAv_Gru_Des &= a.Descrizione & ","
            'strAv_Gru_Des_Lat &= a.Descrizione_Latina & ","
        Next

        'strAv_Gru = Left(strAv_Gru, strAv_Gru.Length - 1)
        'strAv_Gru_Des = Left(strAv_Gru_Des, strAv_Gru_Des.Length - 1)
        'strAv_Gru_Des_Lat = Left(strAv_Gru_Des_Lat, strAv_Gru_Des_Lat.Length - 1)


    End Sub


    ''###############################################################################################
    'Public Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita( _
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByRef Dt As DataTable, _
    '                                    ByRef MessaggioErrore As String, _
    '                                    ByVal Fr_Cod As String, _
    '                                    ByVal Veg_Cod As String, _
    '                                    ByVal Grsp_Cod As String, _
    '                                    ByVal Av_Cod As String, _
    '                                    ByVal Av_Gru As String, _
    '                                    ByVal For_Veg_Av_Cod As String)


    '    Dim StrSQL As New System.Text.StringBuilder


    '    Try

    '        StrSQL.Length = 0

    '        Select Case Fr_Cod

    '            Case 0

    '                StrSQL.Length = 0
    '                StrSQL.Append(" Select DISTINCT ")
    '                StrSQL.Append("         Avversita.Av_Cod, GruppoAvversita.Av_Gru, ")
    '                StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
    '                StrSQL.Append("         ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

    '                StrSQL.Append(" FROM         FormulatixSpeciexAvversita LEFT OUTER JOIN")
    '                StrSQL.Append("              GruppoAvversita ON FormulatixSpeciexAvversita.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN")
    '                StrSQL.Append("              Avversita ON FormulatixSpeciexAvversita.Av_Cod = Avversita.Av_Cod  ")

    '                StrSQL.Append(" WHERE   (FormulatixSpeciexAvversita.Fr_Cod <> -1)  ")

    '                If Fr_Cod <> "0" Then
    '                    StrSQL.Append(" AND   (FormulatixSpeciexAvversita.Fr_Cod IN (" & Agro_SQL_SaveText(Fr_Cod) & "))  ")
    '                End If

    '                If Veg_Cod <> "0" Then
    '                    StrSQL.Append(" AND (FormulatixSpeciexAvversita.VEG_COD IN  (" & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
    '                    StrSQL.Append("      OR  FormulatixSpeciexAvversita.Veg_Cod = 5000336 ")
    '                    StrSQL.Append("      OR FormulatixSpeciexAvversita.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
    '                    StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
    '                    StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_SaveNum(Veg_Cod) & ")))  ")
    '                End If

    '                If Grsp_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Grsp_Cod IN (" & Agro_SQL_SaveText(Grsp_Cod) & "))  ")
    '                End If

    '                If Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Av_Cod IN (" & Agro_SQL_SaveText(Av_Cod) & "))  ")
    '                End If

    '                If Av_Gru <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Av_Gru IN (" & Agro_SQL_SaveText(Av_Gru) & "))")
    '                End If

    '                'Escludo le infestanti
    '                StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Cod not in (SELECT Av_Cod FROM InfestantiAttive) ")
    '                StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_gru not in (SELECT Av_gru FROM GruppoAvversitaAttive) ")

    '                'Escludo non usare e #
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

    '                If For_Veg_Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.For_Veg_Av_Cod = " & Agro_SQL_SaveText(For_Veg_Av_Cod) & ")")
    '                End If

    '                '--------------------------------------------------------------------------
    '                Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita")
    '                '--------------------------------------------------------------------------


    '            Case Else

    '                Dim Dt_Tmp As DataTable
    '                Dim Dr As DataRow
    '                Dim DrTmp() As DataRow
    '                Dim strGerarchiaAvGru(0) As String
    '                Dim strGerarchiaAvGruDes(0) As String
    '                Dim strGerarchiaAvGruDesLat(0) As String

    '                Dim i, j, n, x As Integer
    '                Dim AvGruPresente As Boolean
    '                Dim N_AvGru As Integer

    '                Dim AvGruTot As New Hashtable
    '                Dim AvCodTot As New Hashtable

    '                Dim objUtility As New AgronicaCoreUtility.DatatableUtility

    '                StrSQL.Append(" Select DISTINCT ")
    '                StrSQL.Append("         Avversita.Av_Cod, GruppoAvversita.Av_Gru, ")
    '                StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
    '                StrSQL.Append("         ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

    '                StrSQL.Append(" FROM         FormulatixSpeciexAvversita LEFT OUTER JOIN")
    '                StrSQL.Append("              GruppoAvversita ON FormulatixSpeciexAvversita.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN")
    '                StrSQL.Append("              Avversita ON FormulatixSpeciexAvversita.Av_Cod = Avversita.Av_Cod  ")

    '                StrSQL.Append(" WHERE   (FormulatixSpeciexAvversita.Fr_Cod <> -1)  ")

    '                If Fr_Cod <> "0" Then
    '                    StrSQL.Append(" AND   (FormulatixSpeciexAvversita.Fr_Cod IN (" & Agro_SQL_SaveText(Fr_Cod) & "))  ")
    '                End If

    '                If Veg_Cod <> "0" Then
    '                    StrSQL.Append(" AND (FormulatixSpeciexAvversita.VEG_COD IN  (" & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
    '                    StrSQL.Append("      OR  FormulatixSpeciexAvversita.Veg_Cod = 5000336 ")
    '                    StrSQL.Append("      OR FormulatixSpeciexAvversita.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
    '                    StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
    '                    StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_SaveNum(Veg_Cod) & ")))  ")

    '                End If

    '                If Grsp_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Grsp_Cod IN (" & Agro_SQL_SaveText(Grsp_Cod) & "))  ")
    '                End If

    '                If Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Av_Cod IN (" & Agro_SQL_SaveText(Av_Cod) & "))  ")
    '                End If

    '                If Av_Gru <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.Av_Gru IN (" & Agro_SQL_SaveText(Av_Gru) & "))")
    '                End If

    '                'Escludo le infestanti
    '                StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Cod not in (SELECT Av_Cod FROM InfestantiAttive) ")
    '                StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_gru not in (SELECT Av_gru FROM GruppoAvversitaAttive) ")

    '                'Escludo non usare e #
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

    '                If For_Veg_Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexAvversita.For_Veg_Av_Cod = " & Agro_SQL_SaveText(For_Veg_Av_Cod) & ")")
    '                End If

    '                '--------------------------------------------------------------------------
    '                Dt_Tmp = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita")
    '                '--------------------------------------------------------------------------

    '                If Not Dt_Tmp Is Nothing Then

    '                    Dt = Dt_Tmp.Clone

    '                    Dim strAvCod() As String = objUtility.SelectDistinct(Dt_Tmp, "Av_Cod")
    '                    Dim strAvGru() As String = objUtility.SelectDistinct(Dt_Tmp, "Av_Gru")
    '                    N_AvGru = strAvGru.Length

    '                    'Per ogni singola selezionata 
    '                    'ricavo il gruppo padre con livello più alto nella gerarchia
    '                    'e creco i gruppi a cui tale gruppo è legato nella gerarchia
    '                    If Not strAvCod Is Nothing Then

    '                        For i = 0 To strAvCod.Length - 1

    '                            If strAvCod(i) <> "0" Then

    '                                Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
    '                                Dim DtAvGru As DataTable
    '                                DtAvGru = objAvGru.Leggi3(0, strAvCod(i), 0, 0, AGRODATAINIZIO, AGRODATAFINE, _
    '                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                            "", "", _
    '                                                            objParametri)
    '                                For j = 0 To DtAvGru.Rows.Count - 1
    '                                    AvGruPresente = False
    '                                    For x = 0 To UBound(strAvGru)
    '                                        If DtAvGru.Rows(j).Item("av_gru") = strAvGru(x) Then
    '                                            AvGruPresente = True
    '                                            Exit For
    '                                        End If
    '                                    Next
    '                                    If AvGruPresente = False Then
    '                                        ReDim Preserve strAvGru(N_AvGru)
    '                                        strAvGru(N_AvGru) = DtAvGru.Rows(j).Item("av_gru")
    '                                        N_AvGru += 1
    '                                    End If
    '                                Next

    '                                'Aggiungo la singola nel Dt finale
    '                                DrTmp = Dt_Tmp.Select("Av_Cod=" & strAvCod(i).ToString)

    '                                If Not DrTmp Is Nothing AndAlso DrTmp.Length > 0 Then

    '                                    If Not AvCodTot.ContainsKey(CInt(strAvCod(i))) Then
    '                                        AvCodTot.Add(CInt(strAvCod(i)), DrTmp(0).Item("Av_Des_Vol") & "|" & DrTmp(0).Item("Av_Des_Lat"))
    '                                    End If

    '                                End If

    '                            End If

    '                        Next
    '                    End If



    '                    If Not strAvGru Is Nothing Then
    '                        For i = 0 To strAvGru.Length - 1
    '                            If strAvGru(i) <> "0" Then
    '                                ReDim strGerarchiaAvGru(0)
    '                                ReDim strGerarchiaAvGruDes(0)
    '                                ReDim strGerarchiaAvGruDesLat(0)
    '                                GerarchiaGruppiAvversita(objParametri, strAvGru(i), strGerarchiaAvGru, strGerarchiaAvGruDes, strGerarchiaAvGruDesLat)
    '                                If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
    '                                    For n = 0 To strGerarchiaAvGru.Length - 1
    '                                        If strGerarchiaAvGru(n) <> 0 Then
    '                                            If Not AvGruTot.ContainsKey(CInt(strGerarchiaAvGru(n))) Then

    '                                                AvGruTot.Add(CInt(strGerarchiaAvGru(n)), strGerarchiaAvGruDes(n) & "|" & strGerarchiaAvGruDesLat(n))

    '                                                'aggiungo le singole legate al gruppo
    '                                                Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
    '                                                Dim DtAvCod As DataTable
    '                                                DtAvCod = objAvCod.Leggi(0, 0, strGerarchiaAvGru(n), AGRODATAINIZIO, AGRODATAFINE, _
    '                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                            " Livello is not null ", "", _
    '                                                                            objParametri)
    '                                                If Not DtAvCod Is Nothing Then
    '                                                    For j = 0 To DtAvCod.Rows.Count - 1
    '                                                        If Not AvCodTot.ContainsKey(CInt(DtAvCod.Rows(j).Item("Av_Cod"))) Then
    '                                                            AvCodTot.Add(CInt(DtAvCod.Rows(j).Item("Av_Cod")), DtAvCod.Rows(j).Item("Av_Des_Vol") & "|" & DtAvCod.Rows(j).Item("Av_Des_Lat"))
    '                                                        End If
    '                                                    Next
    '                                                End If

    '                                            End If
    '                                        End If
    '                                    Next
    '                                End If
    '                            End If
    '                        Next
    '                    End If

    '                    Dim ciclo As Integer

    '                    For Each ciclo In AvGruTot.Keys

    '                        Dr = Dt.NewRow

    '                        Dr.Item("Av_Gru") = ciclo
    '                        Dr.Item("Av_Gru_Des") = Split(AvGruTot(ciclo), "|")(0)
    '                        Dr.Item("Av_Gru_Des_Lat") = Split(AvGruTot(ciclo), "|")(1)

    '                        Dr.Item("Av_Cod") = 0
    '                        Dr.Item("Av_Des_Vol") = ""
    '                        Dr.Item("Av_Des_Lat") = ""

    '                        Dt.Rows.Add(Dr)

    '                    Next

    '                    For Each ciclo In AvCodTot.Keys

    '                        Dr = Dt.NewRow

    '                        Dr.Item("Av_Gru") = 0
    '                        Dr.Item("Av_Gru_Des") = ""
    '                        Dr.Item("Av_Gru_Des_Lat") = ""

    '                        Dr.Item("Av_Cod") = ciclo
    '                        Dr.Item("Av_Des_Vol") = Split(AvCodTot(ciclo), "|")(0)
    '                        Dr.Item("Av_Des_Lat") = Split(AvCodTot(ciclo), "|")(1)

    '                        Dt.Rows.Add(Dr)

    '                    Next

    '                End If


    '        End Select

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Dt = Nothing

    '    End Try


    'End Sub


    Private Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita_2_getQuery(ByVal Fr_Cod As String, ByVal Veg_Cod As String, ByVal Grsp_Cod As String, ByVal Av_Cod As String, ByVal Av_Gru As String, ByVal For_Veg_Av_Cod As String, ByVal StrSQL As System.Text.StringBuilder, ByVal Solo_Registrati As String, ByVal Avversita_1_Infestanti_2 As Integer, ByVal Testo_Ricerca As String, ByVal Data As String, Optional ByVal TornaCodice As Boolean = False, Optional ByVal Storico As Boolean = False,
                                                                                       Optional ByVal FormulatiXAllegatiNormative_IDRiga As Integer = 0, Optional ByVal TipoFormulato As Integer = 0)

        Dim TabellaFormulatixSpeciexDanno As String
        If Avversita_1_Infestanti_2 = 1 Then
            If Storico = False Then
                TabellaFormulatixSpeciexDanno = "FormulatixSpeciexAvversita"
            Else
                TabellaFormulatixSpeciexDanno = "FormulatixSpeciexAvversitaxNormative"
            End If
        Else
            If Storico = False Then
                TabellaFormulatixSpeciexDanno = "FormulatixSpeciexInfestanti"
            Else
                TabellaFormulatixSpeciexDanno = "FormulatixSpeciexInfestantixNormative"
            End If

        End If

        StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
        StrSQL.Append(" Select DISTINCT ")

        StrSQL.Append("         Avversita.Av_Cod, GruppoAvversita.Av_Gru, ")
        StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
        StrSQL.Append("         ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

        If TornaCodice = True Then
            StrSQL.Append("    ,dann.For_Veg_Av_Cod ")
        End If

        '(21/11/2017 fede) aggiunti dati fine scorte
        If Storico = True Then
            StrSQL.Append(" , ISNULL(dann.Fr_Des_Prec,'') AS Fr_Des_Prec " & vbCrLf)
            StrSQL.Append(" , dann.DataSmaltimentoScorte " & vbCrLf)
            StrSQL.Append(" , dann.FormulatiXAllegatiNormative_IDRiga " & vbCrLf)
        Else
            StrSQL.Append(" , '' AS Fr_Des_Prec " & vbCrLf)
            StrSQL.Append(" , '' AS DataSmaltimentoScorte " & vbCrLf)
            StrSQL.Append(" , 0 AS FormulatiXAllegatiNormative_IDRiga " & vbCrLf)
        End If

        StrSQL.Append(" FROM         " & TabellaFormulatixSpeciexDanno & " dann LEFT OUTER JOIN")
        StrSQL.Append("              GruppoAvversita ON dann.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN")
        StrSQL.Append("              Avversita ON dann.Av_Cod = Avversita.Av_Cod  ")

        StrSQL.Append(" WHERE   (dann.Fr_Cod <> -1)  ")

        If Fr_Cod <> "0" Then
            StrSQL.Append(" AND   (dann.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
        End If

        If TipoFormulato = enum_TipoFormulato.InstallazioneTrappoleCattureMassa Then
            If Veg_Cod <> "0" Then
                StrSQL.Append(" AND (dann.VEG_COD IN  (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & ")  ")
                StrSQL.Append("      OR  dann.Veg_Cod = 5000336 ")
                StrSQL.Append("     OR dann.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
                StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
                StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & "))  ")
                StrSQL.Append("    OR  (dann.Veg_Cod = 0 AND  dann.Grsp_Cod = 0))  ")
            End If

            If Grsp_Cod <> "0" Then
                StrSQL.Append(" AND     (dann.Grsp_Cod IN (" & Agro_SQL_Save_Clausola_IN(Grsp_Cod, False) & ")  ")
                StrSQL.Append("    OR  (dann.Veg_Cod = 0 AND  dann.Grsp_Cod = 0))  ")
            End If
        Else
            If Veg_Cod <> "0" Then
                StrSQL.Append(" AND (dann.VEG_COD IN  (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & ")  ")
                StrSQL.Append("      OR  dann.Veg_Cod = 5000336 ")
                StrSQL.Append("     OR dann.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
                StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
                StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & ")))  ")
            End If

            If Grsp_Cod <> "0" Then
                StrSQL.Append(" AND     (dann.Grsp_Cod IN (" & Agro_SQL_Save_Clausola_IN(Grsp_Cod, False) & "))  ")
            End If
        End If

        If Av_Cod <> "0" Then
            StrSQL.Append(" AND     (dann.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(Av_Cod, False) & "))  ")
        End If

        If Av_Gru <> "0" Then
            StrSQL.Append(" AND     (dann.Av_Gru IN (" & Agro_SQL_Save_Clausola_IN(Av_Gru, False) & "))")
        End If

        'Escludo le infestanti
        If Avversita_1_Infestanti_2 = 1 Then
            StrSQL.Append(" AND dann.Av_Cod not in (SELECT Av_Cod FROM InfestantiAttive) ")
            StrSQL.Append(" AND dann.Av_gru not in (SELECT Av_gru FROM GruppoAvversitaAttive) ")
        End If

        'Escludo non usare e #
        StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%non usare%'  ")
        StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")
        StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'  ")
        StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

        If For_Veg_Av_Cod <> "0" Then
            StrSQL.Append(" AND     (dann.For_Veg_Av_Cod = " & Agro_SQL_SaveText(For_Veg_Av_Cod) & ")")
        End If

        If Testo_Ricerca <> "" Then
            StrSQL.Append(" AND ((Avversita.Av_Des_Vol LIKE '%" & Agro_SQL_SaveText(Testo_Ricerca) & "%')  ")
            StrSQL.Append(" OR (GruppoAvversita.Av_Gru_Des LIKE '%" & Agro_SQL_SaveText(Testo_Ricerca) & "%'))  ")
        End If

        '(21/11/2017 fede) aggiunti dati fine scorte
        If Data <> "" Then

            If Storico = False Then
                StrSQL.Append(" AND     (dann.validita_inizio <=" & Agro_SQL_SaveDate(Data) & ")")
                StrSQL.Append(" AND     (dann.validita_fine >=" & Agro_SQL_SaveDate(Data) & ")")
            Else
                StrSQL.Append("  AND (")
                StrSQL.Append("  (  dann.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                StrSQL.Append("  AND  dann.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                StrSQL.Append("  OR ")
                StrSQL.Append("  (  dann.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                StrSQL.Append("      )")

                'avversita legate al decreto scelto col prodotto
                'If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '    StrSQL.Append(" AND     (dann.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                'Else
                '    StrSQL.Append(" AND     (dann.FormulatiXAllegatiNormative_IDRiga is null )")
                'End If
            End If

        Else

            If Storico = False Then
                StrSQL.Append(" AND     (dann.validita_inizio <=" & Agro_SQL_SaveDate(Today) & ")")
                StrSQL.Append(" AND     (dann.validita_fine >=" & Agro_SQL_SaveDate(Today) & ")")
            Else
                StrSQL.Append("  AND (")
                StrSQL.Append("  (  dann.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                StrSQL.Append("  AND  dann.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                StrSQL.Append("  OR ")
                StrSQL.Append("  (  dann.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                StrSQL.Append("      )")

                'avversita legate al decreto scelto col prodotto
                'If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '    StrSQL.Append(" AND     (dann.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                'Else
                '    StrSQL.Append(" AND     (dann.FormulatiXAllegatiNormative_IDRiga is null )")
                'End If
            End If

        End If


    End Sub

    Private Function HashTableToString(ByVal H As Hashtable, ByVal key_1_Value_2 As Integer) As String

        Dim sArray As String()

        Dim i As Integer = 0
        If key_1_Value_2 = 1 Then
            For Each key As String In H.Keys
                i += 1
                ReDim sArray(i)
                sArray(i - 1) = key
            Next
        Else
            For Each val As String In H.Values
                i += 1
                ReDim sArray(i)
                sArray(i - 1) = val
            Next
        End If


        Return String.Join(",", sArray)

    End Function

    Private Function GetSoloAvGruConDose(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Fr_Cod As String, ByVal Veg_Cod As String, ByVal Grsp_Cod As String, ByVal StrSQL As System.Text.StringBuilder, ByVal AvCodTot As Hashtable, ByVal Avversita_1_Infestanti_2 As Integer, ByVal Data As Date, Optional ByVal TornaCodice As Boolean = False) As DataTable
        Return GetSoloAvGruConDose(objParametri, Fr_Cod, Veg_Cod, Grsp_Cod, StrSQL, HashTableToString(AvCodTot, 1), Avversita_1_Infestanti_2, Data)
    End Function

    Private Function GetSoloAvGruConDose(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Fr_Cod As String, ByVal Veg_Cod As String, ByVal Grsp_Cod As String, ByVal StrSQL As System.Text.StringBuilder, ByVal AvGruTot As String, ByVal Avversita_1_Infestanti_2 As Integer, ByVal Data As Date, Optional ByVal TornaCodice As Boolean = False, Optional ByVal Storico As Boolean = False, Optional ByVal FormulatiXAllegatiNormative_IDRiga As Integer = 0) As DataTable
        Dim dtSoloAvConDose As DataTable
        StrSQL.Length = 0

        AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita_2_getQuery(Fr_Cod, Veg_Cod, Grsp_Cod, "0", AvGruTot, "0", StrSQL, "0", Avversita_1_Infestanti_2, "", Data, TornaCodice, Storico, FormulatiXAllegatiNormative_IDRiga)
        dtSoloAvConDose = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita")
        Return dtSoloAvConDose
    End Function

    '###############################################################################################
    'A differenza della precedente scelgo se visualizzare solo le avversita/gruppi registrati o tutto quelli della gerarchia
    Public Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_AvversitaInfestanti_2(
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef Dt As DataTable,
                        ByRef MessaggioErrore As String,
                        ByVal Fr_Cod As String,
                        ByVal Veg_Cod As String,
                        ByVal Grsp_Cod As String,
                        ByVal Av_Cod As String,
                        ByVal Av_Gru As String,
                        ByVal For_Veg_Av_Cod As String,
                        ByVal Solo_Registrati As String,
                        ByVal Avversita_1_Infestanti_2 As Integer,
                        ByVal Testo_Ricerca As String,
                        ByVal Data As String,
                         Optional ByVal TornaCodice As Boolean = False,
                         Optional ByVal Storico As Boolean = False,
                         Optional ByVal FormulatiXAllegatiNormative_IDRiga As Integer = 0,
                        Optional ByVal TipoFormulato As Integer = 0
                    )


        Dim StrSQL As New System.Text.StringBuilder


        Try

            StrSQL.Length = 0

            Select Case Solo_Registrati

                Case "1"

                    AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita_2_getQuery(Fr_Cod, Veg_Cod, Grsp_Cod, Av_Cod, Av_Gru, For_Veg_Av_Cod, StrSQL, Solo_Registrati, Avversita_1_Infestanti_2, Testo_Ricerca, Data, TornaCodice, Storico, FormulatiXAllegatiNormative_IDRiga, TipoFormulato)

                    '--------------------------------------------------------------------------
                    Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita")
                    '--------------------------------------------------------------------------


                Case Else

                    Dim WS_VerificaDosiSuGrafoGruppi As Boolean = False
                    Try
                        WS_VerificaDosiSuGrafoGruppi = CBool(Configuration.ConfigurationManager.AppSettings("WS_VerificaDosiSuGrafoGruppi"))
                    Catch ex As Exception

                    End Try

                    Dim DtRegistratiDirettamente As DataTable
                    Dim Dr As DataRow
                    Dim DrTmp() As DataRow
                    Dim strGerarchiaAvGru(0) As String
                    Dim strGerarchiaAvGruDes(0) As String
                    Dim strGerarchiaAvGruDesLat(0) As String

                    Dim i, j, n, x As Integer
                    Dim AvGruPresente As Boolean
                    Dim N_AvGruRegistratiDirettamente As Integer

                    Dim AvGruTot As New Hashtable
                    Dim AvCodTot As New Hashtable

                    Dim Codice As String

                    Dim Fr_Des_Prec As String
                    Dim DataSmaltimentoScorte As String


                    Dim objUtility As New AgronicaCoreUtility.DatatableUtility

                    AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita_2_getQuery(Fr_Cod, Veg_Cod, Grsp_Cod, Av_Cod, Av_Gru, For_Veg_Av_Cod, StrSQL, Solo_Registrati, Avversita_1_Infestanti_2, Testo_Ricerca, Data, TornaCodice, Storico, FormulatiXAllegatiNormative_IDRiga)

                    '--------------------------------------------------------------------------
                    DtRegistratiDirettamente = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita")
                    '--------------------------------------------------------------------------

                    If Not DtRegistratiDirettamente Is Nothing Then

                        Dt = DtRegistratiDirettamente.Clone
                        Dt.Columns(8).DataType = GetType(String)


                        Dim strAvCodRegistratiDirettamente() As String = objUtility.SelectDistinct(DtRegistratiDirettamente, "Av_Cod")
                        Dim strAvGruRegistratiDirettamente() As String = objUtility.SelectDistinct(DtRegistratiDirettamente, "Av_Gru")

                        If Not strAvGruRegistratiDirettamente Is Nothing Then
                            N_AvGruRegistratiDirettamente = strAvGruRegistratiDirettamente.Length
                        End If

                        'Per ogni singola selezionata 
                        'ricavo il gruppo padre con livello più alto nella gerarchia
                        'e creco i gruppi a cui tale gruppo è legato nella gerarchia
                        If Not strAvCodRegistratiDirettamente Is Nothing Then

                            For i = 0 To strAvCodRegistratiDirettamente.Length - 1

                                Codice = ""

                                If strAvCodRegistratiDirettamente(i) <> "0" Then

                                    DrTmp = DtRegistratiDirettamente.Select("Av_Cod=" & strAvCodRegistratiDirettamente(i).ToString)

                                    Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                    Dim DtAvGru As DataTable
                                    DtAvGru = objAvGru.Leggi3(0, strAvCodRegistratiDirettamente(i), 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "",
                                                                objParametri)
                                    For j = 0 To DtAvGru.Rows.Count - 1
                                        AvGruPresente = False
                                        For x = 0 To UBound(strAvGruRegistratiDirettamente)
                                            If DtAvGru.Rows(j).Item("av_gru") = strAvGruRegistratiDirettamente(x) Then
                                                AvGruPresente = True
                                                Exit For
                                            End If
                                        Next
                                        If AvGruPresente = False Then
                                            ReDim Preserve strAvGruRegistratiDirettamente(N_AvGruRegistratiDirettamente)
                                            strAvGruRegistratiDirettamente(N_AvGruRegistratiDirettamente) = DtAvGru.Rows(j).Item("av_gru")
                                            N_AvGruRegistratiDirettamente += 1
                                        End If
                                    Next

                                    'Aggiungo la singola nel Dt finale
                                    If Not DrTmp Is Nothing AndAlso DrTmp.Length > 0 Then

                                        '(03/12/2020 fede) aggiunte le avv doppie per fine scorta
                                        For Each drAT As DataRow In DrTmp

                                            Fr_Des_Prec = ""
                                            DataSmaltimentoScorte = ""

                                            If Storico = True Then
                                                If Not IsDBNull(drAT.Item("Fr_Des_Prec")) Then
                                                    Fr_Des_Prec = drAT.Item("Fr_Des_Prec")
                                                End If
                                                If Not IsDBNull(drAT.Item("DataSmaltimentoScorte")) Then
                                                    DataSmaltimentoScorte = drAT.Item("DataSmaltimentoScorte")
                                                End If
                                            End If

                                            If TornaCodice = True Then
                                                If Not IsDBNull(drAT.Item("For_Veg_Av_Cod")) Then
                                                    Codice = drAT.Item("For_Veg_Av_Cod")
                                                End If
                                                If Not AvCodTot.ContainsKey(strAvCodRegistratiDirettamente(i) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte) Then
                                                    AvCodTot.Add(strAvCodRegistratiDirettamente(i) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte, drAT.Item("Av_Des_Vol") & "|" & drAT.Item("Av_Des_Lat"))
                                                End If
                                            Else
                                                If Not AvCodTot.ContainsKey(strAvCodRegistratiDirettamente(i)) Then
                                                    AvCodTot.Add(strAvCodRegistratiDirettamente(i), drAT.Item("Av_Des_Vol") & "|" & drAT.Item("Av_Des_Lat"))
                                                End If
                                            End If
                                        Next

                                        '    Fr_Des_Prec = ""
                                        '    DataSmaltimentoScorte = ""

                                        '    If Storico = True Then
                                        '        If Not IsDBNull(DrTmp(0).Item("Fr_Des_Prec")) Then
                                        '            Fr_Des_Prec = DrTmp(0).Item("Fr_Des_Prec")
                                        '        End If
                                        '        If Not IsDBNull(DrTmp(0).Item("DataSmaltimentoScorte")) Then
                                        '            DataSmaltimentoScorte = DrTmp(0).Item("DataSmaltimentoScorte")
                                        '        End If
                                        '    End If

                                        '    If TornaCodice = True Then
                                        '        Dim DrCod() As DataRow = DtRegistratiDirettamente.Select("av_cod=" & strAvCodRegistratiDirettamente(i))
                                        '        If Not DrCod Is Nothing AndAlso DrCod.Length > 0 AndAlso Not IsDBNull(DrCod(0).Item("For_Veg_Av_Cod")) Then
                                        '            Codice = DrCod(0).Item("For_Veg_Av_Cod")
                                        '        End If
                                        '        If Not AvCodTot.ContainsKey(strAvCodRegistratiDirettamente(i) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte) Then
                                        '            AvCodTot.Add(strAvCodRegistratiDirettamente(i) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte, DrTmp(0).Item("Av_Des_Vol") & "|" & DrTmp(0).Item("Av_Des_Lat"))
                                        '        End If
                                        '    Else
                                        '        If Not AvCodTot.ContainsKey(strAvCodRegistratiDirettamente(i)) Then
                                        '            AvCodTot.Add(strAvCodRegistratiDirettamente(i), DrTmp(0).Item("Av_Des_Vol") & "|" & DrTmp(0).Item("Av_Des_Lat"))
                                        '        End If
                                        '    End If


                                    End If

                                End If

                            Next

                        End If



                        If Not strAvGruRegistratiDirettamente Is Nothing Then

                            For i = 0 To strAvGruRegistratiDirettamente.Length - 1

                                Codice = ""

                                If strAvGruRegistratiDirettamente(i) <> "0" Then

                                    DrTmp = DtRegistratiDirettamente.Select("av_gru=" & strAvGruRegistratiDirettamente(i))

                                    Fr_Des_Prec = ""
                                    DataSmaltimentoScorte = ""

                                    If Not DrTmp Is Nothing AndAlso DrTmp.Length > 0 Then

                                        If Storico = True Then
                                            If Not IsDBNull(DrTmp(0).Item("Fr_Des_Prec")) Then
                                                Fr_Des_Prec = DrTmp(0).Item("Fr_Des_Prec")
                                            End If
                                            If Not IsDBNull(DrTmp(0).Item("DataSmaltimentoScorte")) Then
                                                DataSmaltimentoScorte = DrTmp(0).Item("DataSmaltimentoScorte")
                                            End If
                                        End If

                                    End If

                                    ReDim strGerarchiaAvGru(0)
                                    ReDim strGerarchiaAvGruDes(0)
                                    ReDim strGerarchiaAvGruDesLat(0)
                                    GerarchiaGruppiAvversita(objParametri, strAvGruRegistratiDirettamente(i), strGerarchiaAvGru, strGerarchiaAvGruDes, strGerarchiaAvGruDesLat, Nothing)

                                    'vanni, 12/04/2013: dopo aver trovato gruppi e singole anche attraverso il grafo, filtro solo le avversità che hanno effettivamente una dose
                                    Dim strAvCodGruConDose As String()
                                    If WS_VerificaDosiSuGrafoGruppi Then

                                        Dim DtGerarchieConDose As DataTable =
                                            GetSoloAvGruConDose(objParametri, Fr_Cod, Veg_Cod, Grsp_Cod, StrSQL, String.Join(",", strGerarchiaAvGru), Avversita_1_Infestanti_2, Data, TornaCodice, Storico, FormulatiXAllegatiNormative_IDRiga)


                                        strAvCodGruConDose = objUtility.SelectDistinct(DtGerarchieConDose, "Av_Gru")
                                    Else
                                        strAvCodGruConDose = strGerarchiaAvGru
                                    End If

                                    Fr_Des_Prec = ""
                                    DataSmaltimentoScorte = ""


                                    If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then

                                        For n = 0 To strGerarchiaAvGru.Length - 1

                                            If strGerarchiaAvGru(n) <> 0 Then

                                                If TornaCodice = True Then

                                                    Dim DrCod() As DataRow = DtRegistratiDirettamente.Select("av_gru=" & strAvGruRegistratiDirettamente(i))

                                                    If Not DrCod Is Nothing AndAlso DrCod.Length > 0 AndAlso Not IsDBNull(DrCod(0).Item("For_Veg_Av_Cod")) Then
                                                        Codice = DrCod(0).Item("For_Veg_Av_Cod")
                                                    End If

                                                    'vanni , 12/04/2013
                                                    If Not AvGruTot.ContainsKey(strGerarchiaAvGru(n) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte) AndAlso
                                                        (Not strAvCodGruConDose Is Nothing AndAlso
                                                         Array.IndexOf(strAvCodGruConDose, strGerarchiaAvGru(n)) > -1
                                                    ) Then

                                                        AvGruTot.Add(strGerarchiaAvGru(n) & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte, strGerarchiaAvGruDes(n) & "|" & strGerarchiaAvGruDesLat(n))

                                                        'aggiungo le singole legate al gruppo
                                                        Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                                        Dim DtAvCod As DataTable

                                                        'vanni , 12/04/2013
                                                        Dim lCondizioneRicercaAggiuntiva As String = ""
                                                        If Not WS_VerificaDosiSuGrafoGruppi Then
                                                            lCondizioneRicercaAggiuntiva = " Livello is not null "
                                                        End If

                                                        DtAvCod = objAvCod.Leggi(0, 0, strGerarchiaAvGru(n), AGRODATAINIZIO, AGRODATAFINE,
                                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                    lCondizioneRicercaAggiuntiva, "",
                                                                                    objParametri)


                                                        If Not DtAvCod Is Nothing Then
                                                            For j = 0 To DtAvCod.Rows.Count - 1
                                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(j).Item("Av_Cod") & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte) Then
                                                                    AvCodTot.Add(DtAvCod.Rows(j).Item("Av_Cod") & "|" & Codice & "|" & Fr_Des_Prec & "|" & DataSmaltimentoScorte, DtAvCod.Rows(j).Item("Av_Des_Vol") & "|" & DtAvCod.Rows(j).Item("Av_Des_Lat"))
                                                                End If
                                                            Next
                                                        End If

                                                    End If

                                                Else

                                                    'vanni , 12/04/2013
                                                    If Not AvGruTot.ContainsKey(strGerarchiaAvGru(n)) AndAlso
                                                        (Not strAvCodGruConDose Is Nothing AndAlso
                                                         Array.IndexOf(strAvCodGruConDose, strGerarchiaAvGru(n)) > -1
                                                    ) Then

                                                        AvGruTot.Add(strGerarchiaAvGru(n), strGerarchiaAvGruDes(n) & "|" & strGerarchiaAvGruDesLat(n))

                                                        'aggiungo le singole legate al gruppo
                                                        Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                                                        Dim DtAvCod As DataTable

                                                        'vanni , 12/04/2013
                                                        Dim lCondizioneRicercaAggiuntiva As String = ""
                                                        If Not WS_VerificaDosiSuGrafoGruppi Then
                                                            lCondizioneRicercaAggiuntiva = " Livello is not null "
                                                        End If

                                                        DtAvCod = objAvCod.Leggi(0, 0, strGerarchiaAvGru(n), AGRODATAINIZIO, AGRODATAFINE,
                                                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                    lCondizioneRicercaAggiuntiva, "",
                                                                                    objParametri)


                                                        If Not DtAvCod Is Nothing Then
                                                            For j = 0 To DtAvCod.Rows.Count - 1
                                                                If Not AvCodTot.ContainsKey(DtAvCod.Rows(j).Item("Av_Cod").ToString) Then
                                                                    AvCodTot.Add(DtAvCod.Rows(j).Item("Av_Cod").ToString, DtAvCod.Rows(j).Item("Av_Des_Vol") & "|" & DtAvCod.Rows(j).Item("Av_Des_Lat"))
                                                                End If
                                                            Next
                                                        End If

                                                    End If

                                                End If


                                            End If
                                        Next
                                    End If


                                End If
                            Next
                        End If


                        Dim ciclo As String

                        For Each ciclo In AvGruTot.Keys

                            'If strSoloDoseAvGru.Contains(ciclo) Then
                            Dr = Dt.NewRow

                            If TornaCodice = True Then
                                Dr.Item("Av_Gru") = Split(ciclo, "|")(0)
                                If IsNumeric(Split(ciclo, "|")(1)) Then
                                    Dr.Item("For_Veg_Av_Cod") = Split(ciclo, "|")(1)
                                Else
                                    Dr.Item("For_Veg_Av_Cod") = 0
                                End If

                                Dr.Item("Fr_Des_Prec") = Split(ciclo, "|")(2)
                                Dr.Item("DataSmaltimentoScorte") = Split(ciclo, "|")(3)

                            Else
                                Dr.Item("Av_Gru") = ciclo
                                Dr.Item("Fr_Des_Prec") = ""
                                Dr.Item("DataSmaltimentoScorte") = ""
                            End If

                            'Dr.Item("Av_Gru") = ciclo
                            Dr.Item("Av_Gru_Des") = Split(AvGruTot(ciclo), "|")(0)
                            Dr.Item("Av_Gru_Des_Lat") = Split(AvGruTot(ciclo), "|")(1)

                            Dr.Item("Av_Cod") = 0
                            Dr.Item("Av_Des_Vol") = ""
                            Dr.Item("Av_Des_Lat") = ""

                            Dt.Rows.Add(Dr)
                            'End If

                        Next

                        For Each ciclo In AvCodTot.Keys

                            Dr = Dt.NewRow
                            'If strSoloDoseAvCod.Contains(ciclo) Then
                            Dr.Item("Av_Gru") = 0
                            Dr.Item("Av_Gru_Des") = ""
                            Dr.Item("Av_Gru_Des_Lat") = ""

                            If TornaCodice = True Then
                                Dr.Item("Av_Cod") = Split(ciclo, "|")(0)
                                If IsNumeric(Split(ciclo, "|")(1)) Then
                                    Dr.Item("For_Veg_Av_Cod") = Split(ciclo, "|")(1)
                                Else
                                    Dr.Item("For_Veg_Av_Cod") = 0
                                End If
                                Dr.Item("Fr_Des_Prec") = Split(ciclo, "|")(2)
                                Dr.Item("DataSmaltimentoScorte") = Split(ciclo, "|")(3)
                            Else
                                Dr.Item("Av_Cod") = ciclo
                                Dr.Item("Fr_Des_Prec") = ""
                                Dr.Item("DataSmaltimentoScorte") = ""
                            End If


                            'Dr.Item("Av_Cod") = ciclo
                            Dr.Item("Av_Des_Vol") = Split(AvCodTot(ciclo), "|")(0)
                            Dr.Item("Av_Des_Lat") = Split(AvCodTot(ciclo), "|")(1)

                            Dt.Rows.Add(Dr)
                            'End If

                        Next

                    End If

            End Select

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try



    End Sub



    '###############################################################################################
    'A differenza della precedente scelgo se visualizzare solo le avversita/gruppi registrati o tutto quelli della gerarchia
    Public Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Avversita_2(
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef Dt As DataTable,
                                        ByRef MessaggioErrore As String,
                                        ByVal Fr_Cod As String,
                                        ByVal Veg_Cod As String,
                                        ByVal Grsp_Cod As String,
                                        ByVal Av_Cod As String,
                                        ByVal Av_Gru As String,
                                        ByVal For_Veg_Av_Cod As String,
                                        ByVal Solo_Registrati As String,
                                        ByVal Testo_Ricerca As String,
                                        ByVal Data As String,
                                            Optional ByVal TornaCodice As Boolean = False,
                                                Optional ByVal Storico As Boolean = False)


        AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_AvversitaInfestanti_2(
            objParametri, Dt, MessaggioErrore, Fr_Cod, Veg_Cod, Grsp_Cod, Av_Cod, Av_Gru, For_Veg_Av_Cod, Solo_Registrati, 1, Testo_Ricerca, Data, TornaCodice, Storico)


    End Sub


    ''###############################################################################################
    'Public Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Infestanti( _
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByRef Dt As DataTable, _
    '                                    ByRef MessaggioErrore As String, _
    '                                    ByVal Fr_Cod As String, _
    '                                    ByVal Veg_Cod As String, _
    '                                    ByVal Grsp_Cod As String, _
    '                                    ByVal Av_Cod As String, _
    '                                    ByVal Av_Gru As String, _
    '                                    ByVal For_Veg_Av_Cod As String)


    '    Dim StrSQL As New System.Text.StringBuilder


    '    Try

    '        StrSQL.Length = 0

    '        Select Case Fr_Cod

    '            Case 0

    '                StrSQL.Length = 0
    '                StrSQL.Append(" Select DISTINCT ")
    '                StrSQL.Append("         Avversita.Av_Cod, GruppoAvversita.Av_Gru, ")
    '                StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
    '                StrSQL.Append("         ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

    '                StrSQL.Append(" FROM         FormulatixSpeciexInfestanti LEFT OUTER JOIN")
    '                StrSQL.Append("              GruppoAvversita ON FormulatixSpeciexInfestanti.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN")
    '                StrSQL.Append("              Avversita ON FormulatixSpeciexInfestanti.Av_Cod = Avversita.Av_Cod  ")

    '                StrSQL.Append(" WHERE   (FormulatixSpeciexInfestanti.Fr_Cod <> -1)  ")

    '                If Fr_Cod <> "0" Then
    '                    StrSQL.Append(" AND   (FormulatixSpeciexInfestanti.Fr_Cod IN (" & Agro_SQL_SaveText(Fr_Cod) & "))  ")
    '                End If

    '                If Veg_Cod <> "0" Then
    '                    StrSQL.Append(" AND (FormulatixSpeciexInfestanti.VEG_COD IN  (" & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
    '                    StrSQL.Append("      OR FormulatixSpeciexInfestanti.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
    '                    StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
    '                    StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_SaveNum(Veg_Cod) & ")))  ")
    '                End If

    '                If Grsp_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Grsp_Cod IN (" & Agro_SQL_SaveText(Grsp_Cod) & "))  ")
    '                End If

    '                If Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Av_Cod IN (" & Agro_SQL_SaveText(Av_Cod) & "))  ")
    '                End If

    '                If Av_Gru <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Av_Gru IN (" & Agro_SQL_SaveText(Av_Gru) & "))")
    '                End If

    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")

    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

    '                If For_Veg_Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.For_Veg_Av_Cod = " & Agro_SQL_SaveText(For_Veg_Av_Cod) & ")")
    '                End If

    '                '--------------------------------------------------------------------------
    '                Dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Infestanti")
    '                '--------------------------------------------------------------------------


    '            Case Else

    '                Dim Dt_Tmp As DataTable
    '                Dim Dr As DataRow
    '                Dim DrTmp() As DataRow
    '                Dim strGerarchiaAvGru(0) As String
    '                Dim strGerarchiaAvGruDes(0) As String
    '                Dim strGerarchiaAvGruDesLat(0) As String

    '                Dim i, j, n, x As Integer
    '                Dim AvGruPresente As Boolean
    '                Dim N_AvGru As Integer

    '                Dim AvGruTot As New Hashtable
    '                Dim AvCodTot As New Hashtable

    '                Dim objUtility As New AgronicaCoreUtility.DatatableUtility

    '                StrSQL.Append(" Select DISTINCT ")
    '                StrSQL.Append("         Avversita.Av_Cod, GruppoAvversita.Av_Gru, ")
    '                StrSQL.Append("         Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des, ")
    '                StrSQL.Append("         ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

    '                StrSQL.Append(" FROM         FormulatixSpeciexInfestanti LEFT OUTER JOIN")
    '                StrSQL.Append("              GruppoAvversita ON FormulatixSpeciexInfestanti.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN")
    '                StrSQL.Append("              Avversita ON FormulatixSpeciexInfestanti.Av_Cod = Avversita.Av_Cod  ")

    '                StrSQL.Append(" WHERE   (FormulatixSpeciexInfestanti.Fr_Cod <> -1)  ")

    '                If Fr_Cod <> "0" Then
    '                    StrSQL.Append(" AND   (FormulatixSpeciexInfestanti.Fr_Cod IN (" & Agro_SQL_SaveText(Fr_Cod) & "))  ")
    '                End If

    '                If Veg_Cod <> "0" Then
    '                    StrSQL.Append(" AND (FormulatixSpeciexInfestanti.VEG_COD IN  (" & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
    '                    StrSQL.Append("      OR FormulatixSpeciexInfestanti.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
    '                    StrSQL.Append("                                                     FROM   GruppoColturaleXSpecieVegetali          ")
    '                    StrSQL.Append("                                                     WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_SaveNum(Veg_Cod) & ")))  ")
    '                End If

    '                If Grsp_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Grsp_Cod IN (" & Agro_SQL_SaveText(Grsp_Cod) & "))  ")
    '                End If

    '                If Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Av_Cod IN (" & Agro_SQL_SaveText(Av_Cod) & "))  ")
    '                End If

    '                If Av_Gru <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.Av_Gru IN (" & Agro_SQL_SaveText(Av_Gru) & "))")
    '                End If

    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")

    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%'  ")
    '                StrSQL.Append(" AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

    '                If For_Veg_Av_Cod <> "0" Then
    '                    StrSQL.Append(" AND     (FormulatixSpeciexInfestanti.For_Veg_Av_Cod = " & Agro_SQL_SaveText(For_Veg_Av_Cod) & ")")
    '                End If

    '                '--------------------------------------------------------------------------
    '                Dt_Tmp = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Infestanti")
    '                '--------------------------------------------------------------------------

    '                If Not Dt_Tmp Is Nothing Then

    '                    Dt = Dt_Tmp.Clone

    '                    Dim strAvCod() As String = objUtility.SelectDistinct(Dt_Tmp, "Av_Cod")
    '                    Dim strAvGru() As String = objUtility.SelectDistinct(Dt_Tmp, "Av_Gru")
    '                    N_AvGru = strAvGru.Length

    '                    'Per ogni singola selezionata 
    '                    'ricavo il gruppo padre con livello più alto nella gerarchia
    '                    'e creco i gruppi a cui tale gruppo è legato nella gerarchia
    '                    If Not strAvCod Is Nothing Then

    '                        For i = 0 To strAvCod.Length - 1

    '                            If strAvCod(i) <> "0" Then

    '                                Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
    '                                Dim DtAvGru As DataTable
    '                                DtAvGru = objAvGru.Leggi3(0, strAvCod(i), 0, 0, AGRODATAINIZIO, AGRODATAFINE, _
    '                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                            "", "", _
    '                                                            objParametri)
    '                                For j = 0 To DtAvGru.Rows.Count - 1
    '                                    AvGruPresente = False
    '                                    For x = 0 To UBound(strAvGru)
    '                                        If DtAvGru.Rows(j).Item("av_gru") = strAvGru(x) Then
    '                                            AvGruPresente = True
    '                                            Exit For
    '                                        End If
    '                                    Next
    '                                    If AvGruPresente = False Then
    '                                        ReDim Preserve strAvGru(N_AvGru)
    '                                        strAvGru(N_AvGru) = DtAvGru.Rows(j).Item("av_gru")
    '                                        N_AvGru += 1
    '                                    End If
    '                                Next

    '                                'Aggiungo la singola nel Dt finale
    '                                DrTmp = Dt_Tmp.Select("Av_Cod=" & strAvCod(i).ToString)

    '                                If Not DrTmp Is Nothing AndAlso DrTmp.Length > 0 Then

    '                                    If Not AvCodTot.ContainsKey(CInt(strAvCod(i))) Then
    '                                        AvCodTot.Add(CInt(strAvCod(i)), DrTmp(0).Item("Av_Des_Vol") & "|" & DrTmp(0).Item("Av_Des_Lat"))
    '                                    End If

    '                                End If

    '                            End If

    '                        Next
    '                    End If



    '                    If Not strAvGru Is Nothing Then
    '                        For i = 0 To strAvGru.Length - 1
    '                            If strAvGru(i) <> "0" Then
    '                                ReDim strGerarchiaAvGru(0)
    '                                ReDim strGerarchiaAvGruDes(0)
    '                                ReDim strGerarchiaAvGruDesLat(0)
    '                                GerarchiaGruppiAvversita(objParametri, strAvGru(i), strGerarchiaAvGru, strGerarchiaAvGruDes, strGerarchiaAvGruDesLat)
    '                                If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
    '                                    For n = 0 To strGerarchiaAvGru.Length - 1
    '                                        If strGerarchiaAvGru(n) <> 0 Then
    '                                            If Not AvGruTot.ContainsKey(CInt(strGerarchiaAvGru(n))) Then

    '                                                AvGruTot.Add(CInt(strGerarchiaAvGru(n)), strGerarchiaAvGruDes(n) & "|" & strGerarchiaAvGruDesLat(n))

    '                                                'aggiungo le singole legate al gruppo
    '                                                Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
    '                                                Dim DtAvCod As DataTable
    '                                                DtAvCod = objAvCod.Leggi(0, 0, strGerarchiaAvGru(n), AGRODATAINIZIO, AGRODATAFINE, _
    '                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                            " Livello is not null ", "", _
    '                                                                            objParametri)
    '                                                If Not DtAvCod Is Nothing Then
    '                                                    For j = 0 To DtAvCod.Rows.Count - 1
    '                                                        If Not AvCodTot.ContainsKey(CInt(DtAvCod.Rows(j).Item("Av_Cod"))) Then
    '                                                            AvCodTot.Add(CInt(DtAvCod.Rows(j).Item("Av_Cod")), DtAvCod.Rows(j).Item("Av_Des_Vol") & "|" & DtAvCod.Rows(j).Item("Av_Des_Lat"))
    '                                                        End If
    '                                                    Next
    '                                                End If

    '                                            End If
    '                                        End If
    '                                    Next
    '                                End If
    '                            End If
    '                        Next
    '                    End If

    '                    Dim ciclo As Integer

    '                    For Each ciclo In AvGruTot.Keys

    '                        Dr = Dt.NewRow

    '                        Dr.Item("Av_Gru") = ciclo
    '                        Dr.Item("Av_Gru_Des") = Split(AvGruTot(ciclo), "|")(0)
    '                        Dr.Item("Av_Gru_Des_Lat") = Split(AvGruTot(ciclo), "|")(1)

    '                        Dr.Item("Av_Cod") = 0
    '                        Dr.Item("Av_Des_Vol") = ""
    '                        Dr.Item("Av_Des_Lat") = ""

    '                        Dt.Rows.Add(Dr)

    '                    Next

    '                    For Each ciclo In AvCodTot.Keys

    '                        Dr = Dt.NewRow

    '                        Dr.Item("Av_Gru") = 0
    '                        Dr.Item("Av_Gru_Des") = ""
    '                        Dr.Item("Av_Gru_Des_Lat") = ""

    '                        Dr.Item("Av_Cod") = ciclo
    '                        Dr.Item("Av_Des_Vol") = Split(AvCodTot(ciclo), "|")(0)
    '                        Dr.Item("Av_Des_Lat") = Split(AvCodTot(ciclo), "|")(1)

    '                        Dt.Rows.Add(Dr)

    '                    Next

    '                End If


    '        End Select

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Dt = Nothing

    '    End Try


    'End Sub

    '###############################################################################################
    'A differenza della precedente scelgo se visualizzare solo le avversita/gruppi registrati o tutto quelli della gerarchia
    Public Sub AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_Infestanti_2(
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef Dt As DataTable,
                                        ByRef MessaggioErrore As String,
                                        ByVal Fr_Cod As String,
                                        ByVal Veg_Cod As String,
                                        ByVal Grsp_Cod As String,
                                        ByVal Av_Cod As String,
                                        ByVal Av_Gru As String,
                                        ByVal For_Veg_Av_Cod As String,
                                        ByVal Solo_Registrati As String,
                                        ByVal Testo_Ricerca As String,
                                        ByVal Data As String,
                                            Optional ByVal TornaCodice As Boolean = False,
                                                Optional ByVal Storico As Boolean = False)


        AgroWS_InfoReadAvversita_Formulati_SpecieVegetali_AvversitaInfestanti_2(
            objParametri, Dt, MessaggioErrore, Fr_Cod, Veg_Cod, Grsp_Cod, Av_Cod, Av_Gru, For_Veg_Av_Cod, Solo_Registrati, 2, Testo_Ricerca, Data, TornaCodice, Storico)


    End Sub

    '###############################################################################################
    Public Sub AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef Dt As DataTable,
                                    ByRef MessaggioErrore As String,
                                    ByVal Fr_Cod As String,
                                    ByVal Veg_Cod As String,
                                    ByVal Av_Cod As String,
                                    ByVal Av_Gru As String,
                                    ByVal grfi_cod As Integer,
                                    ByVal For_Veg_Av_Dos_Cod As String,
                                    ByVal Data As String,
                                    Optional ByVal FormulatiXAllegatiNormative_IDRiga As Integer = -1,
                                    Optional ByVal Copertura As String = "",
                                    Optional ByVal TipoRichiesto As Integer = 0,
                                    Optional ByVal Gruppo_Dosaggi As Integer = 0
                            )

        Dim stbQuerySelect As New System.Text.StringBuilder
        Dim stbQueryComune As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim stbQuery2 As New System.Text.StringBuilder
        Dim stbQuery3 As New System.Text.StringBuilder
        Dim stbQuery As New System.Text.StringBuilder

        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim i, n As Integer
        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        '--- Creo la Query SQL

        Try


            stbQuery.Length = 0

            stbQuerySelect.Append(" Select DISTINCT ")
            stbQuerySelect.Append(" FormulatixSpecieVegetalixNormative.Fr_Cod, ")
            stbQuerySelect.Append(" FormulatixSpecieVegetalixNormative.Veg_Cod, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Epoca_Cod,0) AS Epoca_Cod, ")

            'stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Min,0) AS BufferZone_Min, ")
            'stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Max,0) AS BufferZone_Max, ")

            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Fr_Des_Prec,'') AS Fr_Des_Prec, " & vbCrLf)
            stbQuerySelect.Append("  FormulatixSpeciexAvversitaxDosixNormative.DataSmaltimentoScorte, " & vbCrLf)

            '============================================================================================================================
            'Marco: Aggiunti Campi Controllo Dose Max Annuale
            '----------------------------------------------------------------------------------------------------------------------------
            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DoseMaxAnno,0) AS DoseMaxAnno, " & vbCrLf)
            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DoseMaxAnno_UDM,0) AS DoseMaxAnno_UDM, " & vbCrLf)

            stbQuerySelect.Append("  ISNULL(FormulatixSpecieVegetalixNormative.DoseMaxAnno,0) AS DoseMaxAnno_Veg_Cod, " & vbCrLf)
            stbQuerySelect.Append("  ISNULL(FormulatixSpecieVegetalixNormative.DoseMaxAnno_UDM,0) AS DoseMaxAnno_UDM_Veg_Cod, " & vbCrLf)
            '============================================================================================================================

            stbQuerySelect.Append("  FormulatixSpeciexAvversitaxDosixNormative.FormulatiXAllegatiNormative_IDRiga, " & vbCrLf)

            stbQuerySelect.Append(" FormulatixSpeciexAvversitaxNormative.Av_Cod, FormulatixSpeciexAvversitaxNormative.Av_Gru, ")

            stbQuerySelect.Append(" FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Dos_Cod, ")
            stbQuerySelect.Append(" FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Min,0)) AS Dose_Min, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose_Max,0)) AS Dose_Max, ")


            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.FF_COD,0) AS FF_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD,0) AS UDM_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.MDI_COD,0) AS MDI_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DA_FF_COD,0) AS DA_FF_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.A_FF_COD,0) AS A_FF_COD, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Flag_Protetto,0) AS Flag_Protetto, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DA_Data, '01/01/1900') as da_data, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.A_Data, '31/12/2100') as a_data, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DA_Epoca_1,0) AS DA_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.A_Epoca_1,0) AS A_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.DA_Epoca_2,0) AS DA_Epoca_2, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.A_Epoca_2,0) AS A_Epoca_2, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Dose,'') AS Dose, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Udm_Des,'') AS Udm_Des, ")
            stbQuerySelect.Append(" '' AS Note, ")
            'stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Note,'') AS Note, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Verificato,0) AS Verificato, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.LimiteInterventi,0) AS LimiteInterventi, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Acqua_min,0) AS Acqua_min, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Acqua_max,0) AS Acqua_max, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Acqua_UDM_COD,0) AS Acqua_UDM_COD, ")

            stbQuerySelect.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max,  ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Gruppo_Dosaggi,0) AS Gruppo_Dosaggi , ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexAvversitaxDosixNormative.Num_Max_Interventi_Globali,0) AS Num_Max_Interventi_Globali  ")

            stbQueryComune.Append(" FROM    FormulatixSpeciexAvversitaxDosixNormative INNER JOIN ")
            stbQueryComune.Append("    FormulatixSpeciexAvversitaxNormative ON FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxNormative.For_Veg_Av_Cod INNER JOIN ")
            stbQueryComune.Append("    UnitaMisura ON FormulatixSpeciexAvversitaxDosixNormative.UDM_COD = UnitaMisura.UDM_COD INNER JOIN ")
            stbQueryComune.Append("    FormulatixSpecieVegetalixNormative ON FormulatixSpeciexAvversitaxNormative.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod AND ")
            stbQueryComune.Append("    FormulatixSpeciexAvversitaxNormative.Veg_Cod = FormulatixSpecieVegetalixNormative.Veg_Cod AND ")
            stbQueryComune.Append("    FormulatixSpeciexAvversitaxNormative.Grsp_Cod = FormulatixSpecieVegetalixNormative.Grsp_Cod ")


            Dim whereOrAnd As String = " WHERE "
            If grfi_cod <> "0" Then
                whereOrAnd = " AND "
            End If

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If grfi_cod <> 0 Then
                stbQueryComune.Append(" WHERE ( FormulatixSpeciexAvversitaxDosixNormative.grfi_cod = 0 " & vbCrLf)
                stbQueryComune.Append("       OR (FormulatixSpeciexAvversitaxDosixNormative.grfi_cod <> 0 and  " & vbCrLf)
                stbQueryComune.Append("           FormulatixSpeciexAvversitaxDosixNormative.grfi_cod = " & grfi_cod & vbCrLf)
                stbQueryComune.Append("       )        " & vbCrLf)
                stbQueryComune.Append("   ) " & vbCrLf)
            End If

            stbQueryComune.Append(whereOrAnd & "    (FormulatixSpecieVegetalixNormative.Fr_Cod <> -1)  ")


            If Fr_Cod <> "0" Then
                stbQueryComune.Append("   AND   (FormulatixSpecieVegetalixNormative.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If Veg_Cod <> "0" Then

                stbQueryComune.Append(" AND (FormulatixSpecieVegetalixNormative.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")

                Select Case TipoRichiesto
                     'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
                    Case enum_TipoFormulato.Geodisinfestanti
                        stbQueryComune.Append("      OR  FormulatixSpecieVegetalixNormative.Veg_Cod = 5000336 ")
                        'Per i coadiuvanti aggiungo i prodotti registrati su 'nessuna specie'
                    Case enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.InstallazioneTrappoleCattureMassa
                        stbQueryComune.Append("      OR  (FormulatixSpecieVegetalixNormative.Veg_Cod = 0 AND FormulatixSpecieVegetalixNormative.Grsp_Cod = 0 )")
                End Select

                stbQueryComune.Append("      OR FormulatixSpecieVegetalixNormative.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
                stbQueryComune.Append("                                                 FROM   GruppoColturaleXSpecieVegetali          ")
                stbQueryComune.Append("                                                 WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "))  ")

            End If


            If For_Veg_Av_Dos_Cod <> "" AndAlso For_Veg_Av_Dos_Cod <> "0" Then
                stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.For_Veg_Av_Dos_Cod IN (" & Agro_SQL_Save_Clausola_IN(For_Veg_Av_Dos_Cod, True) & ")) ")
            End If
            If Gruppo_Dosaggi <> 0 Then
                stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.Gruppo_Dosaggi IN (" & Agro_SQL_Save_Clausola_IN(Gruppo_Dosaggi, False) & ")) ")
            End If

            '(10/01/2018 fede) aggiunto filtro copertura
            If Copertura <> "" Then

                Select Case Copertura

                    Case "0" 'solo fuori campo (fuori campo + non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexAvversitaxDosixNormative.Flag_Protetto <> 1 " & vbCrLf)

                    Case "1" 'solo serra (serra + non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexAvversitaxDosixNormative.Flag_Protetto <> 2 " & vbCrLf)

                    Case "0,1", "1,0" 'entrambi (non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexAvversitaxDosixNormative.Flag_Protetto = 0 " & vbCrLf)

                End Select

            End If


            '-------------------------------------------------------------------------------------------------
            '(21/11/2017 fede) aggiunti dati fine scorte
            If Data <> "" Then

                'If Storico = False Then

                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosi.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosi.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversita.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversita.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")

                'Else

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexAvversitaxNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxDosixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexAvversitaxDosixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxDosixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                'If FormulatiXAllegatiNormative_IDRiga <> -1 Then
                '    'avversita legate al decreto scelto col prodotto
                '    If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                '    Else
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.FormulatiXAllegatiNormative_IDRiga is null )")
                '    End If
                'End If

            Else

                'If Storico = False Then

                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosi.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosi.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversita.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpeciexAvversita.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                '    stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")

                'Else

                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexAvversitaxNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxDosixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexAvversitaxDosixNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexAvversitaxDosixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                'avversita legate al decreto scelto col prodotto
                'If FormulatiXAllegatiNormative_IDRiga <> -1 Then
                '    If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                '    Else
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.FormulatiXAllegatiNormative_IDRiga is null )")
                '    End If
                'End If

            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery1.Length = 0

            'If Av_Cod <> "0" Then
            '    stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Cod =" & Agro_SQL_SaveNum(Av_Cod) & " ")
            'End If

            'If Av_Gru <> "0" Then
            '    stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & " ")
            'End If

            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) Then
                Select Case CInt(Av_Cod)
                    Case Is > 0
                        stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Cod =" & Agro_SQL_SaveNum(Av_Cod) & " ")
                    Case Is < 0
                        stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Cod = 0 ")
                End Select
            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) Then
                Select Case CInt(Av_Gru)
                    Case Is > 0
                        stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & " ")
                    Case Is < 0
                        stbQuery1.Append(" AND   FormulatixSpeciexAvversitaxNormative.Av_Gru = 0 ")
                End Select
            End If


            '-------------------------------------------------------------------------------------------------
            stbQuery2.Length = 0

            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) AndAlso CInt(Av_Cod) > 0 Then
                Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvGru As DataTable
                DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvGru.Rows.Count - 1
                    If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                        AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                        strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                    End If
                Next
                If strAvGru <> "" Then
                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                    stbQuery2.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                End If

            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) AndAlso CInt(Av_Gru) > 0 Then
                Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvCod As DataTable
                DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvCod.Rows.Count - 1
                    If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                        AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "")
                        strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                    End If
                Next
                If strAvCod <> "" Then
                    strAvCod = Left(strAvCod, strAvCod.Length - 1)
                    stbQuery2.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & ")")
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery3.Length = 0

            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) AndAlso CInt(Av_Cod) > 0 Then
                If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                    Dim ciclo As Integer
                    For Each ciclo In AvGruTot.Keys
                        If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                            AvGruTot1.Add(ciclo.ToString, "")
                        End If
                        ReDim strGerarchiaAvGru(0)
                        ReDim strGerarchiaAvGruOrdine(0)
                        GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                        If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                            For n = 0 To strGerarchiaAvGru.Length - 1
                                If strGerarchiaAvGru(n) <> 0 Then
                                    If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                        AvGruTot1.Add(strGerarchiaAvGru(n), "")
                                    End If
                                End If
                            Next
                        End If
                    Next
                    Dim ciclo1 As Integer
                    If strAvGru <> "" Then
                        strAvGru &= ","
                    End If
                    For Each ciclo1 In AvGruTot1.Keys
                        strAvGru &= ciclo1 & ","
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) AndAlso CInt(Av_Gru) > 0 Then
                ReDim strGerarchiaAvGru(0)
                ReDim strGerarchiaAvGruOrdine(0)
                GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                    For n = 0 To strGerarchiaAvGru.Length - 1
                        If strGerarchiaAvGru(n) <> 0 Then
                            strAvGru &= strGerarchiaAvGru(n) & ","
                        End If
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND FormulatixSpeciexAvversitaxNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------



            stbQuery.Length = 0
            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 1 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery1)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 2 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery2)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 3 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery3)

            stbQuery.Append(" ORDER BY FormulatixSpecieVegetalixNormative.Fr_Cod, FormulatixSpecieVegetalixNormative.Veg_Cod, Priorita ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi")
            '--------------------------------------------------------------------------
            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To Dt.Rows.Count - 1
                    If Dt.Rows(i).Item("Priorita") = 3 And Dt.Rows(i).Item("Av_Cod") = 0 And Dt.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If Dt.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                Dt.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If

#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            Dt.TableName = "Prodotti"
            Dv.Table = Dt
            Dv.Sort = "Fr_Cod, Priorita, Ordine"
            Dt = Dv.ToTable
#Else
            'Codice per versioni < 2010

#End If
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    Public Sub AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef Dt As DataTable,
                                    ByRef MessaggioErrore As String,
                                    ByVal Fr_Cod As String,
                                    ByVal Veg_Cod As String,
                                    ByVal Av_Cod As String,
                                    ByVal Av_Gru As String,
                                    ByVal grfi_cod As Integer,
                                    ByVal For_Veg_Av_Dos_Cod As String,
                                    ByVal Data As String,
                                    Optional ByVal FormulatiXAllegatiNormative_IDRiga As Integer = -1,
                                    Optional ByVal Copertura As String = "",
                                    Optional ByVal TipoRichiesto As Integer = 0,
                                    Optional ByVal Gruppo_Dosaggi As Integer = 0
                                    )

        Dim stbQuerySelect As New System.Text.StringBuilder
        Dim stbQueryComune As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim stbQuery2 As New System.Text.StringBuilder
        Dim stbQuery3 As New System.Text.StringBuilder
        Dim stbQuery As New System.Text.StringBuilder

        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim i, n As Integer
        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        '--- Creo la Query SQL

        Try


            stbQuery.Length = 0

            stbQuerySelect.Append(" Select DISTINCT ")
            stbQuerySelect.Append(" FormulatixSpecieVegetalixNormative.Fr_Cod, ")
            stbQuerySelect.Append(" FormulatixSpecieVegetalixNormative.Veg_Cod, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.Epoca_Cod,0) AS Epoca_Cod, ")

            'stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Min,0) AS BufferZone_Min, ")
            'stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetalixNormative.BufferZone_Max,0) AS BufferZone_Max, ")

            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexInfestantixDosixNormative.Fr_Des_Prec,'') AS Fr_Des_Prec, " & vbCrLf)
            stbQuerySelect.Append("  FormulatixSpeciexInfestantixDosixNormative.DataSmaltimentoScorte, " & vbCrLf)
            stbQuerySelect.Append("  FormulatixSpeciexInfestantixDosixNormative.FormulatiXAllegatiNormative_IDRiga, " & vbCrLf)

            stbQuerySelect.Append(" FormulatixSpeciexInfestantixNormative.Av_Cod, FormulatixSpeciexInfestantixNormative.Av_Gru, ")

            stbQuerySelect.Append(" FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Dos_Cod, ")
            stbQuerySelect.Append(" FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Min,0)) AS Dose_Min, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose_Max,0)) AS Dose_Max, ")

            '============================================================================================================================
            'Marco: Aggiunti Campi Controllo Dose Max Annuale
            '----------------------------------------------------------------------------------------------------------------------------
            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexInfestantixDosixNormative.DoseMaxAnno,0) AS DoseMaxAnno, " & vbCrLf)
            stbQuerySelect.Append("  ISNULL(FormulatixSpeciexInfestantixDosixNormative.DoseMaxAnno_UDM,0) AS DoseMaxAnno_UDM, " & vbCrLf)

            stbQuerySelect.Append("  ISNULL(FormulatixSpecieVegetalixNormative.DoseMaxAnno,0) AS DoseMaxAnno_Veg_Cod, " & vbCrLf)
            stbQuerySelect.Append("  ISNULL(FormulatixSpecieVegetalixNormative.DoseMaxAnno_UDM,0) AS DoseMaxAnno_UDM_Veg_Cod, " & vbCrLf)
            '============================================================================================================================


            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.FF_COD,0) AS FF_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD,0) AS UDM_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.MDI_COD,0) AS MDI_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.DA_FF_COD,0) AS DA_FF_COD, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.A_FF_COD,0) AS A_FF_COD, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Flag_Protetto,0) AS Flag_Protetto, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.DA_Data, '01/01/1900') as da_data, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.A_Data, '31/12/2100') as a_data, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.DA_Epoca_1,0) AS DA_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.A_Epoca_1,0) AS A_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.DA_Epoca_2,0) AS DA_Epoca_2, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.A_Epoca_2,0) AS A_Epoca_2, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Dose,'') AS Dose, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Udm_Des,'') AS Udm_Des, ")
            stbQuerySelect.Append(" '' AS Note, ")
            'stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Note,'') AS Note, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Verificato,0) AS Verificato, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.LimiteInterventi,0) AS LimiteInterventi, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.UDM_COD_Limite,0) AS UDM_COD_Limite, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Acqua_min,0) AS Acqua_min, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Acqua_max,0) AS Acqua_max, ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Acqua_UDM_COD,0) AS Acqua_UDM_COD, ")

            stbQuerySelect.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max , ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Gruppo_Dosaggi,0) AS Gruppo_Dosaggi , ")
            stbQuerySelect.Append(" ISNULL(FormulatixSpeciexInfestantixDosixNormative.Num_Max_Interventi_Globali,0) AS Num_Max_Interventi_Globali  ")

            stbQueryComune.Append(" FROM    FormulatixSpeciexInfestantixDosixNormative INNER JOIN ")
            stbQueryComune.Append("    FormulatixSpeciexInfestantixNormative ON FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Cod = FormulatixSpeciexInfestantixNormative.For_Veg_Av_Cod INNER JOIN ")
            stbQueryComune.Append("    UnitaMisura ON FormulatixSpeciexInfestantixDosixNormative.UDM_COD = UnitaMisura.UDM_COD INNER JOIN ")
            stbQueryComune.Append("    FormulatixSpecieVegetalixNormative ON FormulatixSpeciexInfestantixNormative.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod AND ")
            stbQueryComune.Append("    FormulatixSpeciexInfestantixNormative.Veg_Cod = FormulatixSpecieVegetalixNormative.Veg_Cod AND ")
            stbQueryComune.Append("    FormulatixSpeciexInfestantixNormative.Grsp_Cod = FormulatixSpecieVegetalixNormative.Grsp_Cod ")

            Dim whereOrAnd As String = " WHERE "
            If grfi_cod <> "0" Then
                whereOrAnd = " AND "
            End If

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If grfi_cod <> 0 Then
                stbQueryComune.Append(" WHERE ( FormulatixSpeciexInfestantixDosixNormative.grfi_cod = 0 " & vbCrLf)
                stbQueryComune.Append("       OR (FormulatixSpeciexInfestantixDosixNormative.grfi_cod <> 0 and  " & vbCrLf)
                stbQueryComune.Append("           FormulatixSpeciexInfestantixDosixNormative.grfi_cod = " & grfi_cod & vbCrLf)
                stbQueryComune.Append("       )        " & vbCrLf)
                stbQueryComune.Append("   ) " & vbCrLf)
            End If

            stbQueryComune.Append(whereOrAnd & "    (FormulatixSpecieVegetalixNormative.Fr_Cod <> -1)  ")



            If Fr_Cod <> "0" Then
                stbQueryComune.Append("   AND   (FormulatixSpecieVegetalixNormative.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If Veg_Cod <> "0" Then
                stbQueryComune.Append(" AND (FormulatixSpecieVegetalixNormative.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                stbQueryComune.Append("     OR FormulatixSpecieVegetalixNormative.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
                stbQueryComune.Append("                                                 FROM   GruppoColturaleXSpecieVegetali          ")
                stbQueryComune.Append("                                                 WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "))  ")
            End If


            If For_Veg_Av_Dos_Cod <> "" AndAlso For_Veg_Av_Dos_Cod <> "0" Then
                stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosixNormative.For_Veg_Av_Dos_Cod IN (" & Agro_SQL_Save_Clausola_IN(For_Veg_Av_Dos_Cod, True) & ")) ")
            End If
            If Gruppo_Dosaggi <> 0 Then
                stbQueryComune.Append(" AND     (FormulatixSpeciexAvversitaxDosixNormative.Gruppo_Dosaggi IN (" & Agro_SQL_Save_Clausola_IN(Gruppo_Dosaggi, False) & ")) ")
            End If

            '(10/01/2018 fede) aggiunto filtro copertura
            If Copertura <> "" Then

                Select Case Copertura

                    Case "0" 'solo fuori campo (fuori campo + non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexInfestantixDosixNormative.Flag_Protetto <> 1 " & vbCrLf)

                    Case "1" 'solo serra (serra + non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexInfestantixDosixNormative.Flag_Protetto <> 2 " & vbCrLf)

                    Case "0,1", "1,0" 'entrambi (non specificato)
                        stbQueryComune.Append("  and  FormulatixSpeciexInfestantixDosixNormative.Flag_Protetto = 0 " & vbCrLf)

                End Select

            End If


            '(21/11/2017 fede) aggiunti dati fine scorte
            '-------------------------------------------------------------------------------------------------
            If Data <> "" Then

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexInfestantixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixDosixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexInfestantixDosixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixDosixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                'avversita legate al decreto scelto col prodotto
                'If FormulatiXAllegatiNormative_IDRiga <> -1 Then
                '    If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosixNormative.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                '    Else
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosixNormative.FormulatiXAllegatiNormative_IDRiga is null )")
                '    End If
                'End If

                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosi.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosi.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestanti.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestanti.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
            Else

                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexInfestantixNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                stbQueryComune.Append("  AND (")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixDosixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Today) & vbCrLf)
                stbQueryComune.Append("  AND  FormulatixSpeciexInfestantixDosixNormative.validita_fine>=" & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("  OR ")
                stbQueryComune.Append("  (  FormulatixSpeciexInfestantixDosixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Today) & ")" & vbCrLf)
                stbQueryComune.Append("      )")

                'If FormulatiXAllegatiNormative_IDRiga <> -1 Then
                '    If FormulatiXAllegatiNormative_IDRiga <> 0 Then
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosixNormative.FormulatiXAllegatiNormative_IDRiga =" & Agro_SQL_SaveNum(FormulatiXAllegatiNormative_IDRiga) & ")")
                '    Else
                '        stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosixNormative.FormulatiXAllegatiNormative_IDRiga is null )")
                '    End If
                'End If

                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosi.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestantixDosi.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestanti.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpeciexInfestanti.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                'stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
            End If


            '-------------------------------------------------------------------------------------------------
            stbQuery1.Length = 0


            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) Then
                Select Case CInt(Av_Cod)
                    Case Is > 0
                        stbQuery1.Append(" AND   FormulatixSpeciexInfestantixNormative.Av_Cod =" & Agro_SQL_SaveNum(Av_Cod) & " ")
                    Case Is < 0
                        stbQuery1.Append(" AND   FormulatixSpeciexInfestantixNormative.Av_Cod = 0 ")
                End Select
            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) Then
                Select Case CInt(Av_Gru)
                    Case Is > 0
                        stbQuery1.Append(" AND   FormulatixSpeciexInfestantixNormative.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & " ")
                    Case Is < 0
                        stbQuery1.Append(" AND   FormulatixSpeciexInfestantixNormative.Av_Gru = 0 ")
                End Select
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery2.Length = 0

            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) AndAlso CInt(Av_Cod) > 0 Then
                Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvGru As DataTable
                DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvGru.Rows.Count - 1
                    If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                        AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                        strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                    End If
                Next
                If strAvGru <> "" Then
                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                    stbQuery2.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                End If
            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) AndAlso CInt(Av_Gru) > 0 Then
                Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvCod As DataTable
                DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvCod.Rows.Count - 1
                    If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                        AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "")
                        strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                    End If
                Next
                If strAvCod <> "" Then
                    strAvCod = Left(strAvCod, strAvCod.Length - 1)
                    stbQuery2.Append(" AND FormulatixSpeciexInfestantixNormative.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & ")")
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery3.Length = 0

            If Av_Cod <> "0" AndAlso IsNumeric(Av_Cod) AndAlso CInt(Av_Cod) > 0 Then
                If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                    Dim ciclo As Integer
                    For Each ciclo In AvGruTot.Keys
                        If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                            AvGruTot1.Add(ciclo.ToString, "")
                        End If
                        ReDim strGerarchiaAvGru(0)
                        ReDim strGerarchiaAvGruOrdine(0)
                        GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                        If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                            For n = 0 To strGerarchiaAvGru.Length - 1
                                If strGerarchiaAvGru(n) <> 0 Then
                                    If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                        AvGruTot1.Add(strGerarchiaAvGru(n), "")
                                    End If
                                End If
                            Next
                        End If
                    Next
                    Dim ciclo1 As Integer
                    If strAvGru <> "" Then
                        strAvGru &= ","
                    End If
                    For Each ciclo1 In AvGruTot1.Keys
                        strAvGru &= ciclo1 & ","
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            If Av_Gru <> "0" AndAlso IsNumeric(Av_Gru) AndAlso CInt(Av_Gru) > 0 Then
                ReDim strGerarchiaAvGru(0)
                ReDim strGerarchiaAvGruOrdine(0)
                GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                    For n = 0 To strGerarchiaAvGru.Length - 1
                        If strGerarchiaAvGru(n) <> 0 Then
                            strAvGru &= strGerarchiaAvGru(n) & ","
                        End If
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND FormulatixSpeciexInfestantixNormative.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------

            stbQuery.Length = 0
            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 1 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery1)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 2 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery2)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 3 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery3)

            stbQuery.Append(" ORDER BY FormulatixSpecieVegetalixNormative.Fr_Cod, FormulatixSpecieVegetalixNormative.Veg_Cod, Priorita ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi")
            '--------------------------------------------------------------------------
            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To Dt.Rows.Count - 1
                    If Dt.Rows(i).Item("Priorita") = 3 And Dt.Rows(i).Item("Av_Cod") = 0 And Dt.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If Dt.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                Dt.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If

#If VBC_VER >= 9 Then
            Dim Dv As New DataView
            Dt.TableName = "Prodotti"
            Dv.Table = Dt
            Dv.Sort = "Fr_Cod, Priorita, Ordine"
            Dt = Dv.ToTable

#Else
            'Codice per versioni < 2010
#End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    'a differenza della precedente ha in più i parametri:
    '- For_Veg_Av_Cod
    '- Avversita1_Infestanti_2 che discrimina dove leggere
    Public Sub AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi_2(
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef Dt As DataTable,
                                 ByRef MessaggioErrore As String,
                                 ByVal Fr_Cod As String,
                                 ByVal Veg_Cod As String,
                                 ByVal Avversita_1_Infestanti_2 As String,
                                 ByVal Av_Cod As String,
                                 ByVal Av_Gru As String,
                                 ByVal grfi_cod As Integer,
                                 ByVal For_Veg_Av_Cod As String,
                                 ByVal For_Veg_Av_Dos_Cod As String,
                                 ByVal Data As String
                         )

        Dim stbQuerySelect As New System.Text.StringBuilder
        Dim stbQueryComune As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim stbQuery2 As New System.Text.StringBuilder
        Dim stbQuery3 As New System.Text.StringBuilder
        Dim stbQuery As New System.Text.StringBuilder

        Dim strAvGru As String = ""
        Dim strAvCod As String = ""
        Dim AvGruTot As New Hashtable
        Dim AvGruTot1 As New Hashtable
        Dim AvCodTot As New Hashtable
        Dim i, n As Integer
        Dim strGerarchiaAvGru(0) As String
        Dim strGerarchiaAvGruOrdine(0) As String

        Try

            Dim TabellaFormulatixSpeciexAvv As String
            Dim TabellaFormulatixSpeciexAvvxDosi As String
            If Avversita_1_Infestanti_2 = "1" Then
                TabellaFormulatixSpeciexAvv = "FormulatixSpeciexAvversita"
                TabellaFormulatixSpeciexAvvxDosi = "FormulatixSpeciexAvversitaxDosi"
            Else
                TabellaFormulatixSpeciexAvv = "FormulatixSpeciexInfestanti"
                TabellaFormulatixSpeciexAvvxDosi = "FormulatixSpeciexInfestantixDosi"
            End If

            stbQuery.Length = 0

            stbQuerySelect.Append(" Select DISTINCT ")
            stbQuerySelect.Append(" FormulatixSpecieVegetali.Fr_Cod, ")
            stbQuerySelect.Append(" FormulatixSpecieVegetali.Veg_Cod, ")

            stbQuerySelect.Append(" ISNULL(FormulatixSpecieVegetali.Epoca_Cod,0) AS Epoca_Cod, ")

            stbQuerySelect.Append(" avv.Av_Cod, avv.Av_Gru, ")

            stbQuerySelect.Append(" dosi.For_Veg_Av_Dos_Cod, ")
            stbQuerySelect.Append(" dosi.For_Veg_Av_Cod, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(dosi.Dose_Min,0)) AS Dose_Min, ")
            stbQuerySelect.Append(" convert(decimal(10,3),ISNULL(dosi.Dose_Max,0)) AS Dose_Max, ")

            stbQuerySelect.Append(" ISNULL(dosi.FF_COD,0) AS FF_COD, ")
            stbQuerySelect.Append(" ISNULL(dosi.UDM_COD,0) AS UDM_COD, ")
            stbQuerySelect.Append(" ISNULL(dosi.MDI_COD,0) AS MDI_COD, ")
            stbQuerySelect.Append(" ISNULL(dosi.DA_FF_COD,0) AS DA_FF_COD, ")
            stbQuerySelect.Append(" ISNULL(dosi.A_FF_COD,0) AS A_FF_COD, ")

            stbQuerySelect.Append(" ISNULL(dosi.Flag_Protetto,0) AS Flag_Protetto, ")

            stbQuerySelect.Append(" ISNULL(dosi.DA_Data, '01/01/1900') as da_data, ")
            stbQuerySelect.Append(" ISNULL(dosi.A_Data, '31/12/2100') as a_data, ")

            stbQuerySelect.Append(" ISNULL(dosi.DA_Epoca_1,0) AS DA_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(dosi.A_Epoca_1,0) AS A_Epoca_1, ")
            stbQuerySelect.Append(" ISNULL(dosi.DA_Epoca_2,0) AS DA_Epoca_2, ")
            stbQuerySelect.Append(" ISNULL(dosi.A_Epoca_2,0) AS A_Epoca_2, ")

            stbQuerySelect.Append(" ISNULL(EpocheDa.Descrizione,'') AS DA_Epoca_Des, ")
            stbQuerySelect.Append(" ISNULL(EpocheA.Descrizione,'') AS A_Epoca_Des, ")

            stbQuerySelect.Append(" ISNULL(dosi.Dose,'') AS Dose, ")
            stbQuerySelect.Append(" ISNULL(dosi.Udm_Des,'') AS Udm_Des, ")
            stbQuerySelect.Append(" '' AS Note, ")

            stbQuerySelect.Append(" ISNULL(dosi.Verificato,0) AS Verificato, ")
            stbQuerySelect.Append(" ISNULL(dosi.LimiteInterventi,0) AS LimiteInterventi, ")
            stbQuerySelect.Append(" ISNULL(dosi.UDM_COD_Limite,0) AS UDM_COD_Limite, ")

            stbQuerySelect.Append(" ISNULL(dosi.Acqua_min,0) AS Acqua_min, ")
            stbQuerySelect.Append(" ISNULL(dosi.Acqua_max,0) AS Acqua_max, ")
            stbQuerySelect.Append(" ISNULL(dosi.Acqua_UDM_COD,0) AS Acqua_UDM_COD, ")
            stbQuerySelect.Append(" ISNULL(UnitaMisura1.UDM_SIM,'') AS Acqua_UDM_SIM, ")

            stbQuerySelect.Append(" ISNULL(UnitaMisura.UDM_SIM,'') AS UDM_SIM, ")

            stbQuerySelect.Append(" ISNULL(dosi.IntervalloTrattamenti_Min,0) AS IntervalloTrattamenti_Min , ")
            stbQuerySelect.Append(" ISNULL(dosi.IntervalloTrattamenti_Max,0) AS IntervalloTrattamenti_Max , ")

            stbQuerySelect.Append(" ISNULL(dosi.Gruppo_Dosaggi,0) AS Gruppo_Dosaggi , ")
            stbQuerySelect.Append(" ISNULL(dosi.Num_Max_Interventi_Globali,0) AS Num_Max_Interventi_Globali  ")

            stbQueryComune.Append(" FROM   " & TabellaFormulatixSpeciexAvvxDosi & " dosi INNER JOIN ")
            stbQueryComune.Append("   " & TabellaFormulatixSpeciexAvv & " avv ON ")
            stbQueryComune.Append("    dosi.For_Veg_Av_Cod = avv.For_Veg_Av_Cod INNER JOIN ")
            stbQueryComune.Append("    UnitaMisura ON dosi.UDM_COD = UnitaMisura.UDM_COD INNER JOIN ")
            stbQueryComune.Append("    FormulatixSpecieVegetali ON avv.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod AND ")
            stbQueryComune.Append("    avv.Veg_Cod = FormulatixSpecieVegetali.Veg_Cod AND ")
            stbQueryComune.Append("    avv.Grsp_Cod = FormulatixSpecieVegetali.Grsp_Cod ")

            stbQueryComune.Append("    LEFT OUTER JOIN UnitaMisura AS UnitaMisura1 ON dosi.Acqua_UDM_COD = UnitaMisura1.UDM_COD ")

            stbQueryComune.Append("    LEFT OUTER JOIN Epoche AS EpocheDa ON dosi.DA_Epoca_1 = EpocheDa.Ep_Cod ")
            stbQueryComune.Append("    LEFT OUTER JOIN Epoche AS EpocheA ON dosi.A_Epoca_1 = EpocheA.Ep_Cod ")


            Dim whereOrAnd As String = " WHERE "
            If grfi_cod <> "0" Then
                whereOrAnd = " AND "
            End If

            'vanni, 09/04/2013 - aggiunta parte grfi_cod
            If grfi_cod <> 0 Then
                stbQueryComune.Append(" WHERE ( dosi.grfi_cod = 0 " & vbCrLf)
                stbQueryComune.Append("       OR (dosi.grfi_cod <> 0 and  " & vbCrLf)
                stbQueryComune.Append("           dosi.grfi_cod = " & grfi_cod & vbCrLf)
                stbQueryComune.Append("       )        " & vbCrLf)
                stbQueryComune.Append("   ) " & vbCrLf)
            End If

            stbQueryComune.Append(whereOrAnd & "    (FormulatixSpecieVegetali.Fr_Cod <> -1)  ")


            If Fr_Cod <> "0" Then
                stbQueryComune.Append("   AND   (FormulatixSpecieVegetali.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If Veg_Cod <> "0" Then
                stbQueryComune.Append(" AND (FormulatixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                stbQueryComune.Append("      OR  avv.Veg_Cod = 5000336 ")
                stbQueryComune.Append("      OR FormulatixSpecieVegetali.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod ")
                stbQueryComune.Append("                                                 FROM   GruppoColturaleXSpecieVegetali          ")
                stbQueryComune.Append("                                                 WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "))  ")
            End If

            If For_Veg_Av_Cod <> "" AndAlso For_Veg_Av_Cod <> "0" Then
                stbQueryComune.Append(" AND     (dosi.For_Veg_Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(For_Veg_Av_Cod, False) & ")) ")
            End If

            If For_Veg_Av_Dos_Cod <> "" AndAlso For_Veg_Av_Dos_Cod <> "0" Then
                stbQueryComune.Append(" AND     (dosi.For_Veg_Av_Dos_Cod IN (" & Agro_SQL_Save_Clausola_IN(For_Veg_Av_Dos_Cod, False) & ")) ")
            End If

            '-------------------------------------------------------------------------------------------------

            If Data <> "" Then
                stbQueryComune.Append(" AND     (dosi.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                stbQueryComune.Append(" AND     (dosi.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                stbQueryComune.Append(" AND     (avv.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                stbQueryComune.Append(" AND     (avv.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
            Else
                stbQueryComune.Append(" AND     (dosi.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                stbQueryComune.Append(" AND     (dosi.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                stbQueryComune.Append(" AND     (avv.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                stbQueryComune.Append(" AND     (avv.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                stbQueryComune.Append(" AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery1.Length = 0

            If Av_Cod <> "0" Then
                stbQuery1.Append(" AND   avv.Av_Cod =" & Agro_SQL_SaveNum(Av_Cod) & " ")
            End If

            If Av_Gru <> "0" Then
                stbQuery1.Append(" AND   avv.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & " ")
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery2.Length = 0

            If Av_Cod <> "0" Then
                Dim objAvGru As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvGru As DataTable
                DtAvGru = objAvGru.Leggi3(0, Av_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvGru.Rows.Count - 1
                    If Not AvGruTot.ContainsKey(DtAvGru.Rows(i).Item("av_gru").ToString) Then
                        AvGruTot.Add(DtAvGru.Rows(i).Item("av_gru").ToString, "")
                        strAvGru &= DtAvGru.Rows(i).Item("av_gru").ToString & ","
                    End If
                Next
                If strAvGru <> "" Then
                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                    stbQuery2.Append(" AND avv.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                End If

            End If

            If Av_Gru <> "0" Then
                Dim objAvCod As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
                Dim DtAvCod As DataTable
                DtAvCod = objAvCod.Leggi3(0, 0, Av_Gru, 0, AGRODATAINIZIO, AGRODATAFINE,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)
                For i = 0 To DtAvCod.Rows.Count - 1
                    If Not AvCodTot.ContainsKey(DtAvCod.Rows(i).Item("av_Cod").ToString) Then
                        AvCodTot.Add(DtAvCod.Rows(i).Item("av_Cod").ToString, "")
                        strAvCod &= DtAvCod.Rows(i).Item("av_Cod").ToString & ","
                    End If
                Next
                If strAvCod <> "" Then
                    strAvCod = Left(strAvCod, strAvCod.Length - 1)
                    stbQuery2.Append(" AND avv.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvCod) & ")")
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            stbQuery3.Length = 0

            If Av_Cod <> "0" Then
                If Not AvGruTot Is Nothing AndAlso AvGruTot.Count > 0 Then
                    Dim ciclo As Integer
                    For Each ciclo In AvGruTot.Keys
                        If Not AvGruTot1.ContainsKey(ciclo.ToString) Then
                            AvGruTot1.Add(ciclo.ToString, "")
                        End If
                        ReDim strGerarchiaAvGru(0)
                        ReDim strGerarchiaAvGruOrdine(0)
                        GerarchiaGruppiAvversita(objParametri, ciclo, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                        If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                            For n = 0 To strGerarchiaAvGru.Length - 1
                                If strGerarchiaAvGru(n) <> 0 Then
                                    If Not AvGruTot1.ContainsKey(strGerarchiaAvGru(n)) Then
                                        AvGruTot1.Add(strGerarchiaAvGru(n), "")
                                    End If
                                End If
                            Next
                        End If
                    Next
                    Dim ciclo1 As Integer
                    If strAvGru <> "" Then
                        strAvGru &= ","
                    End If
                    For Each ciclo1 In AvGruTot1.Keys
                        strAvGru &= ciclo1 & ","
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND avv.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            If Av_Gru <> "0" Then
                ReDim strGerarchiaAvGru(0)
                ReDim strGerarchiaAvGruOrdine(0)
                GerarchiaGruppiAvversita(objParametri, Av_Gru, strGerarchiaAvGru, Nothing, Nothing, strGerarchiaAvGruOrdine)
                If Not strGerarchiaAvGru Is Nothing AndAlso strGerarchiaAvGru.Length > 0 Then
                    For n = 0 To strGerarchiaAvGru.Length - 1
                        If strGerarchiaAvGru(n) <> 0 Then
                            strAvGru &= strGerarchiaAvGru(n) & ","
                        End If
                    Next
                    If strAvGru <> "" Then
                        strAvGru = Left(strAvGru, strAvGru.Length - 1)
                        stbQuery3.Append(" AND avv.Av_gru IN (" & Agro_SQL_Save_Clausola_IN(strAvGru) & ")")
                    End If
                End If
            End If

            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------
            '-------------------------------------------------------------------------------------------------



            stbQuery.Length = 0
            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 1 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery1)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 2 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery2)

            stbQuery.Append(" UNION ")

            stbQuery.Append(stbQuerySelect)
            stbQuery.Append(" , 3 AS Priorita, 0 AS Ordine ")
            stbQuery.Append(stbQueryComune)
            stbQuery.Append(stbQuery3)

            stbQuery.Append(" ORDER BY FormulatixSpecieVegetali.Fr_Cod, FormulatixSpecieVegetali.Veg_Cod, Priorita ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi")
            '--------------------------------------------------------------------------
            Dim j As Integer
            '16/10/2014 Fede : modifica per ordinare le dosi a priorita 3 (quelle registrate sui gruppi trovati navigando la gerarchia dei gruppi) 
            'in base alla navigazione della gerarchia
            If Not strGerarchiaAvGruOrdine Is Nothing AndAlso strGerarchiaAvGruOrdine.Length > 0 Then
                For i = 0 To Dt.Rows.Count - 1
                    If Dt.Rows(i).Item("Priorita") = 3 And Dt.Rows(i).Item("Av_Cod") = 0 And Dt.Rows(i).Item("Av_Gru") <> 0 Then
                        For j = 0 To strGerarchiaAvGruOrdine.Length - 1
                            If Dt.Rows(i).Item("Av_Gru") = strGerarchiaAvGru(j) Then
                                Dt.Rows(i).Item("Ordine") = strGerarchiaAvGruOrdine(j)
                            End If
                        Next
                    End If
                Next
            End If

#If VBC_VER >= 9 Then
            'Codice per VS 2010 e successivi
            Dim Dv As New DataView
            Dt.TableName = "Prodotti"
            Dv.Table = Dt
            Dv.Sort = "Fr_Cod, Veg_Cod, Priorita, Ordine"
            Dt = Dv.ToTable
#Else
            'Codice per versioni < 2010

#End If
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    '###############################################################################################
    Public Sub AgroWS_InfoRead_Formulati_PrincipiAttivi(
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef Dt As DataTable,
                                     ByRef MessaggioErrore As String,
                                     ByVal Fr_Cod As String, ByVal Stato_Cod As String)

        Dim stbQuery As New System.Text.StringBuilder

        '--- Creo la Query SQL

        Try


            stbQuery.Length = 0
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.NewCLTOSS_COD, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod,  PrincipiAttivi.Pa_Des,  FormulatixPrincipiAttivi.Titolo,  FormulatixPrincipiAttivi.Peso ")

            stbQuery.Append("         FROM    Formulati INNER JOIN ")
            If Stato_Cod <> "" Then
                stbQuery.Append("         FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' INNER JOIN " & vbCrLf)
            End If
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  ")
            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            stbQuery.Append("         ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_PrincipiAttivi")
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub


    '###############################################################################################
    'Le informazioni sulla classe tossicologica di un formulato non si trovano piu' nella tabella 'Formulati'.
    'Modifico la query per ricavare tale informazione dalle nuove tabelle.
    'Introduco anche l'OuterJoin per i formulati senza valori associati ...
    'Nota Bene : ora un formulato puo' avere piu' classi tossicologiche !!

    Public Sub AgroWS_InfoRead_Formulati_PrincipiAttivi_2(
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef Dt As DataTable,
                                     ByRef MessaggioErrore As String,
                                     ByVal Fr_Cod As String, ByVal Stato_Cod As String)

        Dim stbQuery As New System.Text.StringBuilder

        '--- Creo la Query SQL

        Try
            'stbQuery.Length = 0
            'stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.NewCLTOSS_COD, ")
            'stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            'stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            'stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            'stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            'stbQuery.Append("         PrincipiAttivi.Pa_Cod,  PrincipiAttivi.Pa_Des,  FormulatixPrincipiAttivi.Titolo ")

            'stbQuery.Append("         FROM    Formulati INNER JOIN ")
            'stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            'stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  ")

            'stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            'If Fr_Cod <> "" Then
            '    stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_SaveText(Fr_Cod) & "))  ")
            'End If

            'stbQuery.Append("         ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")


            stbQuery.Length = 0

            stbQuery.Append(" SELECT ")
            stbQuery.Append("       Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.NewCLTOSS_COD,  ")
            stbQuery.Append("       Formulati.Revocato, Formulati.Data_Revo, Formulati.Sospeso, Formulati.Data_Sosp, ")
            stbQuery.Append("       Formulati.Termine, Formulati.Data_Term, Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte,  ")
            stbQuery.Append("       isnull(PrincipiAttivi.Pa_Cod,0) as Pa_Cod ,  ")
            stbQuery.Append("       isnull(PrincipiAttivi.Pa_Des,'') as Pa_Des ,  ")
            stbQuery.Append("       isnull(FormulatixPrincipiAttivi.Titolo, 0) as Titolo, isnull(FormulatixPrincipiAttivi.Peso, 0) as Peso,  ")
            stbQuery.Append("       isnull(ClasseTossicologica.CLTOSS_COD,'') as CLTOSS_COD,  ")
            stbQuery.Append("       isnull(ClasseTossicologica.CLTOSS_DES,'') as CLTOSS_DES ")
            stbQuery.Append(" FROM  ClasseTossicologica INNER JOIN ")
            stbQuery.Append("       FormulatiXClassiTossicologiche ON ClasseTossicologica.CLTOSS_COD = FormulatiXClassiTossicologiche.CLTOSS_COD RIGHT OUTER JOIN")
            stbQuery.Append("       PrincipiAttivi INNER JOIN")
            stbQuery.Append("       FormulatixPrincipiAttivi ON PrincipiAttivi.Pa_Cod = FormulatixPrincipiAttivi.Pa_Cod RIGHT OUTER JOIN ")
            stbQuery.Append("       Formulati ON FormulatixPrincipiAttivi.Fr_Cod = Formulati.Fr_Cod ON FormulatiXClassiTossicologiche.FOR_COD = Formulati.Fr_Cod ")
            If Stato_Cod <> "" Then
                stbQuery.Append("    INNER JOIN   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
            End If
            stbQuery.Append(" WHERE (Formulati.Fr_Cod <> 0)  ")

            If Fr_Cod <> "" Then
                stbQuery.Append(" AND   (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            'stbQuery.Append(" ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")
            stbQuery.Append(" ORDER BY Formulati.Fr_Des, FormulatixPrincipiAttivi.Titolo DESC ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_PrincipiAttivi_2")
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    Public Sub AgroWS_InfoRead_Formulati_PrincipiAttivi_FamigliePA(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef Dt As DataTable,
        ByRef MessaggioErrore As String,
        ByVal Fr_Cod As String,
        ByVal pa_cod As String,
        ByVal ListaFamigliePrincipiAttivi_Contesto_Cod As String
    )

        Dim stb As New System.Text.StringBuilder

        '--- Creo la Query SQL

        Try


            stb.Length = 0
            stb.AppendLine("  Select  ")
            stb.AppendLine("    FORMULATI.Fr_Cod ")
            stb.AppendLine("  , Formulati.Fr_Des ")
            stb.AppendLine("  , Formulati.NewCLTOSS_COD ")
            stb.AppendLine("  , Formulati.Revocato ")
            stb.AppendLine("  , Formulati.Data_Revo ")
            stb.AppendLine("  , Formulati.Sospeso ")
            stb.AppendLine("  , Formulati.Data_Sosp ")
            stb.AppendLine("  , Formulati.Termine ")
            stb.AppendLine("  , Formulati.Data_Term ")
            stb.AppendLine("  , Formulati.Data_Fine_Comm ")
            stb.AppendLine("  , Formulati.Data_Fine_UsoScorte ")
            stb.AppendLine("  , PrincipiAttivi.Pa_Cod ")
            stb.AppendLine("  , PrincipiAttivi.Pa_Des ")
            stb.AppendLine("  , FormulatixPrincipiAttivi.Titolo ")
            stb.AppendLine("  , isnull(FamigliePrincipiAttivi.Fam_Cod,'') as Fam_Cod ")
            stb.AppendLine("  , isnull( FamigliePrincipiAttivi.Descrizione,'') as Descrizione           ")
            stb.AppendLine("  From Formulati  ")
            stb.AppendLine("     INNER Join          FormulatixPrincipiAttivi  ")
            stb.AppendLine("         On Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod  ")
            stb.AppendLine("  INNER Join          PrincipiAttivi  ")
            stb.AppendLine("         On FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod   ")
            stb.AppendLine("  Left Join          PrincipiAttiviXFamigliePrincipiAttivi pafp ")
            stb.AppendLine("         On pafp.pa_cod = PrincipiAttivi.pa_cod ")

            If ListaFamigliePrincipiAttivi_Contesto_Cod <> "" Then
                stb.AppendLine("and pafp.FamigliePrincipiAttivi_Contesto_COD in (" & Agro_SQL_Save_Clausola_IN(ListaFamigliePrincipiAttivi_Contesto_Cod, False) & ")     ")
            End If

            stb.AppendLine("  Left Join         FamigliePrincipiAttivi  ")
            stb.AppendLine("         On pafp.Fam_Cod = FamigliePrincipiAttivi.Fam_Cod           ")


            stb.AppendLine("  WHERE(FORMULATI.Fr_Cod <> 0)")

            If Fr_Cod <> "" Then
                stb.AppendLine("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If Not String.IsNullOrEmpty(pa_cod) Then

                stb.AppendLine(" union ")
                stb.AppendLine("  Select  ")
                stb.AppendLine("     0  as Fr_Cod  ")
                stb.AppendLine("   , '' as Fr_Des  ")
                stb.AppendLine("   , '' as NewCLTOSS_COD  ")
                stb.AppendLine("   , 0  as Revocato  ")
                stb.AppendLine("   , null as Data_Revo  ")
                stb.AppendLine("   , 0 as Sospeso  ")
                stb.AppendLine("   , null as Data_Sosp  ")
                stb.AppendLine("   , 0 as Termine  ")
                stb.AppendLine("   , null as Data_Term  ")
                stb.AppendLine("   , null as Data_Fine_Comm  ")
                stb.AppendLine("   , null as Data_Fine_UsoScorte  ")
                stb.AppendLine("   , PrincipiAttivi.Pa_Cod  ")
                stb.AppendLine("   , PrincipiAttivi.Pa_Des  ")
                stb.AppendLine("   , 0 as Titolo  ")
                stb.AppendLine("   , isnull(FamigliePrincipiAttivi.Fam_Cod,'') as Fam_Cod  ")
                stb.AppendLine("   , isnull( FamigliePrincipiAttivi.Descrizione,'') as Descrizione     ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine("   From principiAttivi        ")
                stb.AppendLine("   Left Join          PrincipiAttiviXFamigliePrincipiAttivi pafp  ")
                stb.AppendLine("          On pafp.pa_cod = PrincipiAttivi.pa_cod  ")

                If ListaFamigliePrincipiAttivi_Contesto_Cod <> "" Then
                    stb.AppendLine("and pafp.FamigliePrincipiAttivi_Contesto_COD in (" & ListaFamigliePrincipiAttivi_Contesto_Cod & ")     ")
                End If

                stb.AppendLine("   Left Join         FamigliePrincipiAttivi   ")
                stb.AppendLine("          On pafp.Fam_Cod = FamigliePrincipiAttivi.Fam_Cod       ")
                stb.AppendLine("  ")
                stb.AppendLine(" where principiAttivi.pa_cod in (" & Agro_SQL_Save_Clausola_IN(pa_cod) & ")")

            End If

            stb.AppendLine("         ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stb.ToString, "AgroWS_InfoRead_Formulati_PrincipiAttivi")
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub


    '###############################################################################################
    Public Sub AgroWS_InfoRead_Formulati_PrincipiAttivi_FamigliePA(
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef Dt As DataTable,
                                     ByRef MessaggioErrore As String,
                                     ByVal Fr_Cod As String)

        Dim stbQuery As New System.Text.StringBuilder

        '--- Creo la Query SQL

        Try


            stbQuery.Length = 0
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.NewCLTOSS_COD, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod,  PrincipiAttivi.Pa_Des,  FormulatixPrincipiAttivi.Titolo, ")
            stbQuery.Append("         isnull(FamigliePrincipiAttivi.Fam_Cod,'') as Fam_Cod, isnull( FamigliePrincipiAttivi.Descrizione,'') as Descrizione ")

            stbQuery.Append("         FROM    Formulati INNER JOIN ")
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  LEFT JOIN")
            stbQuery.Append("         FamigliePrincipiAttivi ON PrincipiAttivi.Fam_Cod = FamigliePrincipiAttivi.Fam_Cod ")

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")


            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            stbQuery.Append("         ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_PrincipiAttivi")
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    '###############################################################################################
    Public Sub AgroWS_InfoRead_Formulati_PrincipiAttiviGruppiPrincipiAttivi(
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef Dt As DataTable,
                                    ByRef MessaggioErrore As String,
                                    ByVal Fr_Cod As String, ByVal Stato_Cod As String,
                                    Optional ByVal SoloPrincipiConTitolo As Boolean = False,
                                    Optional ByVal Escludi_Pa_Co_Trattamento_Valorizzati As Boolean = False)

        Dim stbQuery As New System.Text.StringBuilder

        Try

            '--- Creo la Query SQL

            stbQuery.Length = 0
            stbQuery.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.NewCLTOSS_COD, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod,  PrincipiAttivi.Pa_Des,  FormulatixPrincipiAttivi.Titolo, FormulatixPrincipiAttivi.Peso, ")
            stbQuery.Append("         ISNULL(GruppiPrincipiAttivi.Gpa_Cod,0) AS Gpa_Cod, ISNULL(GruppiPrincipiAttivi.Gpa_Des,'') AS Gpa_Des ")

            stbQuery.Append("         FROM         GruppiPrincipiAttivi INNER JOIN")
            stbQuery.Append("         GruppiPrincipiAttiviXPrincipiAttivi ON GruppiPrincipiAttivi.GPA_COD = GruppiPrincipiAttiviXPrincipiAttivi.GPA_COD RIGHT OUTER JOIN ")
            stbQuery.Append("         Formulati INNER JOIN ")
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod = FormulatixPrincipiAttivi.Fr_Cod INNER JOIN")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod ON GruppiPrincipiAttiviXPrincipiAttivi.PA_COD = PrincipiAttivi.Pa_Cod ")

            If Stato_Cod <> "" Then
                stbQuery.Append("     INNER JOIN   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
            End If

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If SoloPrincipiConTitolo = True Then
                stbQuery.Append("         AND     ((FormulatixPrincipiAttivi.Titolo <> 0) or (FormulatixPrincipiAttivi.Peso <> 0))  ")
            End If

            If Escludi_Pa_Co_Trattamento_Valorizzati = True Then
                stbQuery.Append("         AND     (PrincipiAttivi.Pa_Co_Trattamento <> 1)  ")
            End If

            stbQuery.Append("          ORDER BY Formulati.Fr_Des, PrincipiAttivi.Pa_Des ")

            '--------------------------------------------------------------------------
            Dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_PrincipiAttiviGruppiPrincipiAttivi")
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

    '################################################################################
    Public Function Leggi_UdM_from_FrCod(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Fr_Cod As Integer, _
                                ByRef Udm_Des As String, _
                                ByRef Udm_Sim As String) _
                                As Integer

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Fitofarmaci.Leggi_UdM()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim stbQuery As New System.Text.StringBuilder
        Dim Udm_Cod As Integer = 0

        Try

            '----- Genero la query SQL 

            stbQuery.Append(" SELECT     TOP 1 FormulatixSpeciexAvversita.Fr_Cod,FormulatixSpeciexAvversitaxDosi.UDM_COD, UnitaMisura.UDM_SIM, UnitaMisura.UDM_DES ")
            stbQuery.Append(" FROM       FormulatixSpeciexAvversita INNER JOIN ")
            stbQuery.Append("            FormulatixSpeciexAvversitaxDosi ON FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod INNER JOIN ")
            stbQuery.Append("            UnitaMisura ON FormulatixSpeciexAvversitaxDosi.UDM_COD = UnitaMisura.UDM_COD")
            stbQuery.Append(" WHERE      FormulatixSpeciexAvversita.Fr_Cod=" & Agro_SQL_SaveNum(Fr_Cod))

            stbQuery.Append(" UNION ")

            stbQuery.Append(" SELECT     TOP 1 FormulatixSpeciexInfestanti.Fr_Cod,FormulatixSpeciexInfestantixDosi.UDM_COD, UnitaMisura.UDM_SIM, UnitaMisura.UDM_DES ")
            stbQuery.Append(" FROM       FormulatixSpeciexInfestanti INNER JOIN ")
            stbQuery.Append("            FormulatixSpeciexInfestantixDosi ON FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod INNER JOIN ")
            stbQuery.Append("            UnitaMisura ON FormulatixSpeciexInfestantixDosi.UDM_COD = UnitaMisura.UDM_COD")
            stbQuery.Append(" WHERE      FormulatixSpeciexInfestanti.Fr_Cod=" & Agro_SQL_SaveNum(Fr_Cod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Dim objCore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                Udm_Cod = objCore.Converti_Kg_L_from_UdmCod(DT.Rows(0).Item("UDM_COD"), Udm_Sim, Udm_Des)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Udm_Cod
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Udm_Cod


    End Function


    '###############################################################################################
    Public Sub AgroWS_InfoRead_Formulati_Classificazioni(
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef Dt As DataTable,
                                     ByRef MessaggioErrore As String,
                                     ByVal Fr_Cod As String, ByVal Stato_Cod As String,
                                     Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                     Optional ByVal Data As Date = AGRODATAINIZIO,
                                     Optional ByVal strFiltro As String = "",
                                     Optional ByVal strSort As String = ""
                                    )

        Dim stbQuery As New System.Text.StringBuilder
        Dim Dt_App As New DataTable

        Try

            stbQuery.Length = 0
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.data_reg, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des ")

            stbQuery.Append("         FROM    Formulati INNER JOIN ")
            stbQuery.Append("         FormulatixClassificazioni ON Formulati.Fr_Cod =  FormulatixClassificazioni.For_Cod INNER JOIN ")
            stbQuery.Append("         ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  ")

            If Stato_Cod <> "" Then
                stbQuery.Append("     INNER JOIN   FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "'  " & vbCrLf)
            End If

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If

            If strFiltro <> "" Then
                stbQuery.Append(strFiltro)
            End If

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES , ClassificazioniFormulati.Class_Des " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_Classificazioni")
            '--------------------------------------------------------------------------

            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    Dt = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Case 1 'prodotti commercio + revocati
                    Dt = Dt_App
                Case 2 'prodotti revocati
                    Dt = Dt_App
            End Select

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub


    Public Sub AgroWS_InfoRead_Formulati_Classificazioni_PrincipiAttivi(
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef Dt As DataTable,
                                  ByRef MessaggioErrore As String,
                                  ByVal Fr_Des As String,
                                  ByVal Fr_Cod As String,
                                  Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                  Optional ByVal Data As Date = AGRODATAINIZIO,
                                  Optional ByVal strFiltro As String = "",
                                  Optional ByVal strSort As String = ""
                                 )

        Dim stbQuery As New System.Text.StringBuilder
        Dim Dt_App As New DataTable

        Try

            stbQuery.Length = 0
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.data_reg, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         ClassificazioniFormulati.Class_Cod, ClassificazioniFormulati.Class_Des, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod, PrincipiAttivi.Pa_Des, ")
            stbQuery.Append("         FormulatiXPeriodoSospensione.DataSospensioneDA, FormulatiXPeriodoSospensione.DataSospensioneA ")

            stbQuery.Append("         FROM    Formulati INNER JOIN ")

            stbQuery.Append("         FormulatixClassificazioni ON Formulati.Fr_Cod =  FormulatixClassificazioni.For_Cod INNER JOIN ")
            stbQuery.Append("         ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  INNER JOIN ")
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod =  FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  LEFT OUTER JOIN ")
            stbQuery.Append("         FormulatiXPeriodoSospensione ON Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If
            If Fr_Des <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Des Like '%" & Agro_SQL_SaveText(Fr_Des) & "%')  ")
            End If

            If strFiltro <> "" Then
                stbQuery.Append(strFiltro)
            End If

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES , ClassificazioniFormulati.Class_Des " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_Formulati_Classificazioni_PrincipiAttivi")
            '--------------------------------------------------------------------------

            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    Dt = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Case 1 'prodotti commercio + revocati
                    Dt = Dt_App
                Case 2 'prodotti revocati
                    Dt = Dt_App
            End Select

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub


    ''' <summary>
    ''' Se Veg_Cod è stringa vuota salto il JOIN e il filtro della FormulatixSpecieVegetali
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="Dt"></param>
    ''' <param name="MessaggioErrore"></param>
    ''' <param name="Fr_Des"></param>
    ''' <param name="Fr_Cod"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="FlagVisualizza_Commercio_Tutti_Revocati"></param>
    ''' <param name="Data"></param>
    ''' <param name="strFiltro"></param>
    ''' <param name="strSort"></param>
    ''' <param name="strPaCod"></param>
    ''' <param name="Stato_Cod"></param>
    Public Sub AgroWS_InfoRead_FormulatixSpecieVegetali_Classificazioni_PrincipiAttivi(
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef Dt As DataTable,
                                  ByRef MessaggioErrore As String,
                                  ByVal Fr_Des As String,
                                  ByVal Fr_Cod As String,
                                  ByVal Veg_Cod As String,
                                  Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                  Optional ByVal Data As Date = AGRODATAINIZIO,
                                  Optional ByVal strFiltro As String = "",
                                  Optional ByVal strSort As String = "",
                                  Optional ByVal strPaCod As String = "",
                                  Optional ByVal Stato_Cod As String = "",
                                  Optional ByVal TipiFormulatiRichiesti As String = "0"
                                 )

        Dim stbQuery As New System.Text.StringBuilder
        Dim Dt_App As New DataTable
        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R

        Try

            stbQuery.Length = 0
            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.data_reg, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         ClassificazioniFormulati.Class_Cod, ClassificazioniFormulati.Class_Des, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod, PrincipiAttivi.Pa_Des, ")
            stbQuery.Append("         FormulatiXPeriodoSospensione.DataSospensioneDA, FormulatiXPeriodoSospensione.DataSospensioneA, ")

            If Veg_Cod <> "" Then
                stbQuery.Append("         FormulatixSpecieVegetali.TempoCarenza,  ")
            Else
                stbQuery.Append("         0 AS TempoCarenza,  ")
            End If

            stbQuery.Append("         FormulatixPrincipiAttivi.Titolo, Ditte.Ditta_Cod, Ditte.Ditta_Des ")

            stbQuery.Append("         FROM    Formulati")

            If Veg_Cod <> "" Then
                stbQuery.Append("         INNER JOIN FormulatixSpecieVegetali ON Formulati.Fr_Cod =  FormulatixSpecieVegetali.Fr_Cod ")
            End If

            stbQuery.Append("         INNER JOIN FormulatixClassificazioni ON Formulati.Fr_Cod =  FormulatixClassificazioni.For_Cod INNER JOIN ")
            stbQuery.Append("         ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  INNER JOIN ")
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod =  FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  LEFT OUTER JOIN ")
            stbQuery.Append("         FormulatiXPeriodoSospensione ON Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")
            stbQuery.Append("         LEFT OUTER JOIN FormulatiXDitte ON FormulatiXDitte.Fr_Cod = Formulati.Fr_Cod AND FormulatiXDitte.TIPODITTA_COD = 1")
            stbQuery.Append("         LEFT OUTER JOIN Ditte ON Ditte.Ditta_Cod = FormulatiXDitte.Ditta_Cod")

            If Stato_Cod <> "" Then
                stbQuery.Append(" INNER JOIN FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "'")
            End If

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If
            If Fr_Des <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Des Like '%" & Agro_SQL_SaveText(Fr_Des) & "%')  ")
            End If

            If Veg_Cod <> "" Then
                stbQuery.Append("         AND     (FormulatixSpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & ")  ")
                stbQuery.Append("               OR  ( FormulatixSpecieVegetali.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
                stbQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Veg_Cod, False) & ") ) ) ) " & vbCrLf)
            End If

            'If Veg_Cod <> 0 Then
            '    stbFiltroQuery.Append("  AND ( FormulatixSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            '    'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
            '    If TipoRichiesto = 7 Then
            '        stbFiltroQuery.Append("      OR  FormulatixSpecieVegetali.Veg_Cod = 5000336 " & vbCrLf)
            '    End If
            '    stbFiltroQuery.Append("      OR  ( FormulatixSpecieVegetali.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
            '    stbFiltroQuery.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
            'End If


            If strPaCod <> "" Then
                stbQuery.Append("         AND     (PrincipiAttivi.Pa_Cod IN (" & Agro_SQL_Save_Clausola_IN(strPaCod, False) & "))  ")
                stbQuery.Append("  AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN (" & Agro_SQL_Save_Clausola_IN(strPaCod, False) & ") and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)" & vbCrLf)
            End If

            If Veg_Cod <> "" Then
                If Data <> AGRODATAINIZIO Then
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                Else
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                End If
            End If

            objFiltro.Filtro_Formulato_Classificazione(TipiFormulatiRichiesti, stbQuery)

            If strFiltro <> "" Then
                stbQuery.Append(strFiltro)
            End If

            'COADIUVANTI (non hanno indicata la specie)
            stbQuery.Append(" UNION " & vbCrLf)

            stbQuery.Append(" SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.data_reg, ")
            stbQuery.Append("         Formulati.Revocato,  Formulati.Data_Revo, ")
            stbQuery.Append("         Formulati.Sospeso,  Formulati.Data_Sosp, ")
            stbQuery.Append("         Formulati.Termine,  Formulati.Data_Term, ")
            stbQuery.Append("         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
            stbQuery.Append("         ClassificazioniFormulati.Class_Cod, ClassificazioniFormulati.Class_Des, ")
            stbQuery.Append("         PrincipiAttivi.Pa_Cod, PrincipiAttivi.Pa_Des, ")
            stbQuery.Append("         FormulatiXPeriodoSospensione.DataSospensioneDA, FormulatiXPeriodoSospensione.DataSospensioneA, ")

            If Veg_Cod <> "" Then
                stbQuery.Append("         FormulatixSpecieVegetali.TempoCarenza,")
            Else
                stbQuery.Append("         0 AS TempoCarenza,")
            End If

            stbQuery.Append("         FormulatixPrincipiAttivi.Titolo, Ditte.Ditta_Cod, Ditte.Ditta_Des ")

            stbQuery.Append("         FROM    Formulati")

            If Veg_Cod <> "" Then
                stbQuery.Append("         INNER JOIN FormulatixSpecieVegetali ON Formulati.Fr_Cod =  FormulatixSpecieVegetali.Fr_Cod ")
            End If

            stbQuery.Append("         INNER JOIN  FormulatixClassificazioni ON Formulati.Fr_Cod =  FormulatixClassificazioni.For_Cod INNER JOIN ")
            stbQuery.Append("         ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  INNER JOIN ")
            stbQuery.Append("         FormulatixPrincipiAttivi ON Formulati.Fr_Cod =  FormulatixPrincipiAttivi.Fr_Cod INNER JOIN ")
            stbQuery.Append("         PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod  LEFT OUTER JOIN ")
            stbQuery.Append("         FormulatiXPeriodoSospensione ON Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")
            stbQuery.Append("         LEFT OUTER JOIN FormulatiXDitte ON FormulatiXDitte.Fr_Cod = Formulati.Fr_Cod AND FormulatiXDitte.TIPODITTA_COD = 1")
            stbQuery.Append("         LEFT OUTER JOIN Ditte ON Ditte.Ditta_Cod = FormulatiXDitte.Ditta_Cod")

            If Stato_Cod <> "" Then
                stbQuery.Append(" INNER JOIN FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "'")
            End If

            stbQuery.Append("         WHERE   (Formulati.Fr_Cod <> 0) ")

            If Veg_Cod <> "" Then
                stbQuery.Append("         AND     (FormulatixSpecieVegetali.Veg_Cod = 0 )  ")
            End If

            stbQuery.Append("         AND     (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506 )  ")

            If Fr_Cod <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fr_Cod, False) & "))  ")
            End If
            If Fr_Des <> "" Then
                stbQuery.Append("         AND     (Formulati.Fr_Des Like '%" & Agro_SQL_SaveText(Fr_Des) & "%')  ")
            End If

            If strPaCod <> "" Then
                stbQuery.Append("         AND     (PrincipiAttivi.Pa_Cod IN (" & Agro_SQL_Save_Clausola_IN(strPaCod, False) & "))  ")
            End If

            If Veg_Cod <> "" Then
                If Data <> AGRODATAINIZIO Then
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Data) & ") ")
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                Else
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_inizio <= " & Agro_SQL_SaveDate(Today) & ") ")
                    stbQuery.Append("         AND     (FormulatixSpecieVegetali.validita_fine >= " & Agro_SQL_SaveDate(Today) & ") ")
                End If
            End If

            objFiltro.Filtro_Formulato_Classificazione(TipiFormulatiRichiesti, stbQuery)

            If strFiltro <> "" Then
                stbQuery.Append(strFiltro)
            End If

            If strSort <> "" Then
                stbQuery.Append(strSort)
            Else
                stbQuery.Append(" ORDER BY Formulati.FR_DES , ClassificazioniFormulati.Class_Des " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, stbQuery.ToString, "AgroWS_InfoRead_FormulatixSpecieVegetali_Classificazioni_PrincipiAttivi")
            '--------------------------------------------------------------------------

            Select Case FlagVisualizza_Commercio_Tutti_Revocati
                Case 0 'solo prodotti in commercio
                    Dt = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Case 1 'prodotti commercio + revocati
                    Dt = Dt_App
                Case 2 'prodotti revocati
                    Dt = Dt_App
            End Select

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Dt = Nothing

        End Try

    End Sub

End Class
