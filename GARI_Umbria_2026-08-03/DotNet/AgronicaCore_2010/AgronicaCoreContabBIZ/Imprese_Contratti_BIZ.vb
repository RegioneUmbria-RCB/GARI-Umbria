Imports System.Data
Imports System.Data.OleDb
Imports System.Collections
Imports System.Collections.Generic
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreContabDAL
Imports AgronicaCoreProfilazioneDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports System.Reflection
Imports AgronicaCoreEntityFramework

Public Class Imprese_Contratti_BIZ_R

    Private Function getRequisitiStabilimentoDataTable() As DataTable
        Dim DT As DataTable = New DataTable("GridRequisitiStabilimento")

        DT.Columns.Add("PIVA", Type.GetType("System.String"))
        DT.Columns.Add("rag_soc", Type.GetType("System.String"))
        DT.Columns.Add("cuaa", Type.GetType("System.String"))
        DT.Columns.Add("Piva_Padre", Type.GetType("System.String"))
        DT.Columns.Add("Rag_Soc_Padre", Type.GetType("System.String"))
        DT.Columns.Add("GruppoRaccolta_Cod", Type.GetType("System.Int32"))
        DT.Columns.Add("GruppoRaccolta_Des", Type.GetType("System.String"))
        DT.Columns.Add("Mat_Cod", Type.GetType("System.Int32"))
        DT.Columns.Add("Mat_Des", Type.GetType("System.String"))
        DT.Columns.Add("Superficie", Type.GetType("System.Decimal"))
        DT.Columns.Add("ResaPrevista", Type.GetType("System.Decimal"))
        DT.Columns.Add("QtaPrevista", Type.GetType("System.Decimal"))
        DT.Columns.Add("TipoTrasporto_Cod", Type.GetType("System.Int32"))
        DT.Columns.Add("ImpiantiSuperficie", Type.GetType("System.Decimal"))
        DT.Columns.Add("ImpiantiResaPrevista", Type.GetType("System.Decimal"))
        DT.Columns.Add("ImpiantiResaMedia", Type.GetType("System.Decimal"))

        Return DT
    End Function

    Public Function getDynamicColumnsTitle(ByVal DTContracts As DataTable) As List(Of String)
        Dim list As New List(Of String)
        For Each row In DTContracts.Rows
            Dim prefix As String = row("Contratto_Cod") & "_" & row("Fase_Cod") & "_"
            list.Add(prefix & "Superfice")
            list.Add(prefix & "Quantita")
            list.Add(prefix & "TipoTrasportoDes")
            list.Add(prefix & "TipoTrasportoCod")
            list.Add(prefix & "ContrattoCodFiglio")
            list.Add(prefix & "FaseCodFiglio")
        Next
        Return list
    End Function

    Private Sub addDynamicColumns(ByRef DT As DataTable, ByVal DTContracts As DataTable)
        For Each row In DTContracts.Rows
            Dim prefix As String = row("Contratto_Cod") & "_" & row("Fase_Cod") & "_"
            DT.Columns.Add(prefix & "Superfice", Type.GetType("System.Double"))
            DT.Columns.Add(prefix & "Quantita", Type.GetType("System.Double"))
            DT.Columns.Add(prefix & "TipoTrasportoDes", Type.GetType("System.String"))
            DT.Columns.Add(prefix & "TipoTrasportoCod", Type.GetType("System.Int32"))
            DT.Columns.Add(prefix & "ContrattoCodFiglio", Type.GetType("System.Int32"))
            DT.Columns.Add(prefix & "FaseCodFiglio", Type.GetType("System.Int32"))
        Next
    End Sub

    Private Function compatRequisitiStabilimentoDT(ByVal DT As DataTable) As DataTable
        Dim compatDT =
            From row In DT
            Group row By dateGroup = New With {
                Key .PIVA = row.Field(Of String)("PIVA"),
                Key .rag_soc = row.Field(Of String)("rag_soc"),
                Key .cuaa = If(IsDBNull(row("cuaa")), "", row("cuaa")),
                Key .Piva_Padre = row.Field(Of String)("Piva_Padre"),
                Key .Rag_Soc_Padre = row.Field(Of String)("Rag_Soc_Padre"),
                Key .GruppoRaccolta_Cod = row.Field(Of Integer)("GruppoRaccolta_Cod"),
                Key .GruppoRaccolta_Des = row.Field(Of String)("GruppoRaccolta_Des"),
                Key .Mat_Cod = row.Field(Of Integer)("Mat_Cod"),
                Key .Mat_Des = row.Field(Of String)("Mat_Des")
                } Into Group
            Select New With {
                .PIVA = dateGroup.PIVA,
                .rag_soc = dateGroup.rag_soc,
                .cuaa = dateGroup.cuaa,
                .Piva_Padre = dateGroup.Piva_Padre,
                .Rag_Soc_Padre = dateGroup.Rag_Soc_Padre,
                .GruppoRaccolta_Cod = dateGroup.GruppoRaccolta_Cod,
                .GruppoRaccolta_Des = dateGroup.GruppoRaccolta_Des,
                .Mat_Cod = dateGroup.Mat_Cod,
                .Mat_Des = dateGroup.Mat_Des,
                .TipoTrasportoCod = 0,
                .Superficie = Group.First()("ImpiantiSuperficie"),
                .ResaPrevista = Group.First()("ImpiantiResaPrevista"),
                .QtaPrevista = Group.First()("ImpiantiResaPrevista"),
                .ImpiantiResaMedia = Group.First()("ImpiantiResaMedia")
                }

        Dim ut As New Gias_EF_Utility
        DT = ut.ObjectQueryToDataTable(compatDT.AsQueryable.ToList())

        Return DT
    End Function

    Private Function extractDinamicColumnsData(
                                              ByVal DTPianoColturale As DataTable,
                                              ByVal DTContracts As DataTable
                                              ) As DataTable
        Dim ut As New Gias_EF_Utility

        Dim dtt =
            From rpc In DTPianoColturale, contr In DTContracts
            Where rpc("Contratto_Cod_Padre") = contr("Contratto_Cod") AndAlso
                rpc("Fase_Cod_Padre") = contr("Fase_Cod")
            Select New With {
                .PIVA = rpc("PIVA"),
                .Superficie = rpc("Superficie"),
                .ResaPrevista = rpc("ResaPrevista"),
                .ImpiantiResaMedia = rpc("ImpiantiResaMedia"),
                .QtaPrevista = rpc("QtaPrevista"),
                .Sup = rpc("Superficie"),
                .Contratto_Cod_Padre = contr("Contratto_Cod"),
                .Fase_Cod_Padre = contr("Fase_Cod"),
                .Contratto_Cod = rpc("Contratto_Cod"),
                .Fase_Cod = rpc("Fase_Cod"),
                .TipoTrasportoCod = rpc("TipoTrasporto_Cod")
                }

        Return ut.ObjectQueryToDataTable(dtt.AsQueryable.ToList())
    End Function

    Public Function readRequisitiStabilimento(
                                      ByVal risUm As Integer,
                                      ByVal dataInizio As Date,
                                      ByVal dataFine As Date,
                                      ByVal matCod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal idBudget As Integer = Nothing
                                      ) As DataTable

        Dim objImpreseContrattoFasiDAL As New Imprese_Contratto_Fasi_R
        Dim DTPianoColturale = objImpreseContrattoFasiDAL.readPianoColturale(risUm, dataInizio, dataFine, matCod, objParametri, idBudget)
        Dim DTContracts = objImpreseContrattoFasiDAL.readContracts(risUm, dataInizio, dataFine, matCod, False, objParametri)
        Dim DT As DataTable = getRequisitiStabilimentoDataTable()

        DT.Merge(DTPianoColturale, False, MissingSchemaAction.Ignore)

        DT = compatRequisitiStabilimentoDT(DT)

        addDynamicColumns(DT, DTContracts)

        Dim dtt = extractDinamicColumnsData(DTPianoColturale, DTContracts)

        DT.PrimaryKey = {DT.Columns("PIVA")}
        For Each row In dtt.Rows
            Dim r = DT.Rows.Find(row("PIVA"))
            If r IsNot Nothing Then
                Dim prefix As String = row("Contratto_Cod_Padre") & "_" & row("Fase_Cod_Padre") & "_"
                r(prefix & "Superfice") = row("Superficie")
                r(prefix & "Quantita") = row("QtaPrevista")
                Dim tipotrasporto_des = ""
                Select Case row("TipoTrasportoCod")
                    Case 0
                        tipotrasporto_des = ""
                    Case 1
                        tipotrasporto_des = "Franco Fabbrica"
                    Case 2
                        tipotrasporto_des = "Trasportato"
                    Case 3
                        tipotrasporto_des = "Misto"
                End Select
                r(prefix & "TipoTrasportoDes") = tipotrasporto_des
                r(prefix & "TipoTrasportoCod") = row("TipoTrasportoCod")
                r(prefix & "ContrattoCodFiglio") = row("Contratto_Cod")
                r(prefix & "FaseCodFiglio") = row("Fase_Cod")
            End If
        Next

        Return DT

    End Function

    Public Function readPianoColturale(
                                      ByVal risUm As Integer,
                                      ByVal dataInizio As Date,
                                      ByVal dataFine As Date,
                                      ByVal matCod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal idBudget As Integer = Nothing
                                      ) As DataTable

        Dim objImpreseContrattoFasiDAL As New Imprese_Contratto_Fasi_R
        Dim DT = objImpreseContrattoFasiDAL.readPianoColturale(risUm, dataInizio, dataFine, matCod, objParametri, idBudget)

        Return DT

    End Function

    Public Function readContracts(
                                 ByVal risUm As Integer,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByVal matCod As Integer,
                                 ByVal mostraAssegnazioni As Boolean,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 ByVal idBudget As Integer,
                                 Optional codContattoConferene As String = ""
                                 ) As DataTable

        Dim objImpreseContrattoFasiDAL As New Imprese_Contratto_Fasi_R
        Dim DT = objImpreseContrattoFasiDAL.readContracts(risUm, dataInizio, dataFine, matCod, mostraAssegnazioni, objParametri, codContattoConferene)

        Return DT

    End Function

    Public Function readSurfaceForProducts(
                                 ByVal risUm As Integer,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByVal matCod As Integer,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal idBudget As Integer = Nothing
                                 ) As DataTable

        Dim objImpreseContrattoFasiDAL As New Imprese_Contratto_Fasi_R
        Dim DT = objImpreseContrattoFasiDAL.readSurfaceForProducts(risUm, dataInizio, dataFine, matCod, objParametri, idBudget)

        Return DT

    End Function

    Public Function readDettaglioAziendale(
                                 ByVal piva As String,
                                 ByVal matCod As Integer,
                                 ByVal idBudget As Integer,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Dim objImpreseContrattoFasiDAL As New Imprese_Contratto_Fasi_R
        Dim DT = objImpreseContrattoFasiDAL.readDettaglioAziendale(piva, matCod, idBudget, dataInizio, dataFine, objParametri)

        Return DT

    End Function

    Public Shared Function ConvertToDataTable(ByVal enumerator As IEnumerator, ByVal fieldCount As Integer) As DataTable
        Dim table As New DataTable()

        For index = 1 To fieldCount
            enumerator.MoveNext()
            enumerator.Current.GetType()
        Next

        Return table
    End Function

End Class

Public Class Imprese_Contratti_BIZ_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiornaContrattoPomodoro(ByVal Piva As String,
                                          ByVal Contratto_Cod As Integer,
                                          ByVal Riferimento As String,
                                          ByVal Contratto_Nome As String,
                                          ByVal Contratto_Des As String,
                                          ByVal Contratto_Numero As String,
                                          ByVal Superficie_Prevista As Double,
                                          ByVal Resa_Prevista As Double,
                                          ByVal Ricavi_Previsti As Double,
                                          ByVal Cau_Contratto As String,
                                          ByVal Cod_Conto As Integer,
                                          ByVal Giudizio As String,
                                          ByVal Data_Inizio_Prevista As Date,
                                          ByVal Data_Fine_Prevista As Date,
                                          ByVal Descrizione_1 As String,
                                          ByVal Descrizione_2 As String,
                                          ByVal Cod_RisUm As Integer,
                                          ByVal Stato As Integer,
                                          ByVal ChkStato_Automatico As Integer,
                                          ByVal Cau_Pagamento As Integer,
                                          ByVal Data_Stipulazione As Date,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal righeGrid_Fasi_Inserite As String, ByVal righeGrid_Fasi_Modificate As String, ByVal righeGrid_Fasi_Cancellate As String,
                                          ByVal righeGrid_Clausole As String,
                                          ByVal Listino_Cod As Integer,
                                          ByVal Valore1 As Double, ByVal Valore2 As Double, ByVal Valore3 As Double, ByVal Valore4 As Double, ByVal Valore5 As Double,
                                          ByVal Valore6 As Double, ByVal Valore7 As Double, ByVal Valore8 As Double, ByVal Valore9 As Double,
                                          ByVal Valore1_2 As Double, ByVal Valore2_2 As Double, ByVal Valore3_2 As Double,
                                          ByVal Data_Inizio_Prevista2 As Date,
                                          ByVal Data_Fine_Prevista2 As Date,
                                          ByVal sa_cod As Integer,
                                          ByVal Fabbricato_Cod As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W.AggiornaContrattoPomodoro()"
        Dim strErr As String = ""

        Dim Fase_Cod As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim bOk As Boolean = True
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim PivaSuperUser = objParametri.PivaSuperUser

            Dim objImprese_Contratti As New AgronicaCoreContabDAL.Imprese_Contratti_W
            Dim objImprese_Contratti_Fasi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_W
            Dim objClausola As New AgronicaCoreContabDAL.Imprese_Contratti_Clausole_W
            Dim objTrasformazioni As New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

            ' forzati
            Dim Base, Top As Integer

            Base = 0
            Top = 2000000000

            'Controllo Salvataggio o Modifica
            Select Case Contratto_Cod

                Case 0

                    'Inserimento Contratto
                    Contratto_Cod = objSeq.NuovoId_Tabella("impresa_contratto", Base, Top, objParametri)

                    bOk = objImprese_Contratti.Scrivi(
                        Piva,
                        Contratto_Cod,
                        Riferimento,
                        Contratto_Nome,
                        Contratto_Des,
                        Contratto_Numero,
                        Superficie_Prevista,
                        Resa_Prevista,
                        Ricavi_Previsti,
                        Cau_Contratto,
                        Cod_Conto,
                        Giudizio,
                        Data_Inizio_Prevista,
                        Data_Fine_Prevista,
                        Descrizione_1,
                        Descrizione_2,
                        Cod_RisUm,
                        Stato,
                        ChkStato_Automatico,
                        Cau_Pagamento,
                        Data_Stipulazione,
                        Validita_Inizio,
                        Validita_Fine,
                        sa_cod,
                        Fabbricato_Cod,
                        objParametri
                    )

                Case Else

                    bOk = objImprese_Contratti.Modifica(
                        Piva,
                        Contratto_Cod,
                        Riferimento,
                        Contratto_Nome,
                        Contratto_Des,
                        Contratto_Numero,
                        Superficie_Prevista,
                        Resa_Prevista,
                        Ricavi_Previsti,
                        Cau_Contratto,
                        Cod_Conto,
                        Giudizio,
                        Data_Inizio_Prevista,
                        Data_Fine_Prevista,
                        Descrizione_1,
                        Descrizione_2,
                        Cod_RisUm,
                        Stato,
                        ChkStato_Automatico,
                        Cau_Pagamento,
                        Data_Stipulazione,
                        Validita_Inizio,
                        Validita_Fine,
                        sa_cod,
                        Fabbricato_Cod,
                        objParametri
                    )

            End Select

            If Not bOk Then
                Throw New Exception
            End If

            '====================================================================================================================================================
            'FASI
            '----------------------------------------------------------------------------------------------------------------------------------------------------
            If righeGrid_Fasi_Inserite <> "" And righeGrid_Fasi_Inserite <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeGrid_Fasi_Inserite)

                    'Inserimento Fase
                    Fase_Cod = objSeq.NuovoId_Tabella("impresa_contratto_fasi", Base, Top, objParametri)

                    'Controllo Maggiorazione
                    If IsNumeric(obj("Valore9")) Then
                        Valore9 = obj("Valore9")
                    Else
                        Valore9 = 0
                    End If

                    Dim Superficie As Decimal
                    If IsNumeric(obj("Superficie")) Then
                        Superficie = obj("Superficie")
                    Else
                        Superficie = 0
                    End If

                    Dim QtaPrevista As Decimal
                    If IsNumeric(obj("QtaPrevista")) Then
                        QtaPrevista = obj("QtaPrevista")
                    Else
                        QtaPrevista = 0
                    End If

                    Dim ResaPrevista As Decimal
                    If IsNumeric(obj("ResaPrevista")) Then
                        ResaPrevista = obj("ResaPrevista")
                    Else
                        ResaPrevista = 0
                    End If

                    bOk = objImprese_Contratti_Fasi.Scrivi(Piva, Contratto_Cod, Fase_Cod, "", Data_Inizio_Prevista, Data_Fine_Prevista, "", 0, 210,
                                                     0, CInt(obj("Mat_Cod")), 0, 0, 0, "", 0, Listino_Cod, Valore1, Valore2, Valore3, Valore4,
                                                     Valore5, Valore6, Valore7, Valore8, Valore9,
                                                     Valore1_2, Valore2_2, Valore3_2,
                                                     Data_Inizio_Prevista2, Data_Fine_Prevista2,
                                                     Validita_Inizio, Validita_Fine,
                                                     Superficie, QtaPrevista, ResaPrevista,
                                                     objParametri)

                    If Not bOk Then
                        Throw New Exception
                    End If
                Next

            End If


            If righeGrid_Fasi_Modificate <> "" And righeGrid_Fasi_Modificate <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeGrid_Fasi_Modificate)

                    'Controllo Maggiorazione
                    If IsNumeric(obj("Valore9")) Then
                        Valore9 = obj("Valore9")
                    Else
                        Valore9 = 0
                    End If

                    Dim Superficie As Decimal
                    If IsNumeric(obj("Superficie")) Then
                        Superficie = obj("Superficie")
                    Else
                        Superficie = 0
                    End If

                    Dim QtaPrevista As Decimal
                    If IsNumeric(obj("QtaPrevista")) Then
                        QtaPrevista = obj("QtaPrevista")
                    Else
                        QtaPrevista = 0
                    End If

                    Dim ResaPrevista As Decimal
                    If IsNumeric(obj("ResaPrevista")) Then
                        ResaPrevista = obj("ResaPrevista")
                    Else
                        ResaPrevista = 0
                    End If

                    'Modifica Fase                    
                    bOk = objImprese_Contratti_Fasi.Modifica(Piva, Contratto_Cod, CInt(obj("Fase_Cod")), "", Data_Inizio_Prevista, Data_Fine_Prevista, "", 0, 210,
                                                     0, CInt(obj("Mat_Cod")), 0, 0, 0, "", 0, Listino_Cod, Valore1, Valore2, Valore3, Valore4,
                                                     Valore5, Valore6, Valore7, Valore8, Valore9,
                                                     Valore1_2, Valore2_2, Valore3_2,
                                                     Data_Inizio_Prevista2, Data_Fine_Prevista2,
                                                     Validita_Inizio, Validita_Fine,
                                                     Superficie, QtaPrevista, ResaPrevista,
                                                     objParametri)

                    If Not bOk Then
                        Throw New Exception
                    End If

                Next

            End If


            If righeGrid_Fasi_Cancellate <> "" And righeGrid_Fasi_Cancellate <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeGrid_Fasi_Cancellate)

                    objImprese_Contratti_Fasi.Cancella(Piva, Contratto_Cod, CInt(obj("Fase_Cod")), "", objParametri)

                Next

            End If



            '====================================================================================================================================================
            'CLAUSOLE
            '----------------------------------------------------------------------------------------------------------------------------------------------------

            'Cancellazione Preventiva
            objClausola.CancellaContrattoxClausola(Piva, Contratto_Cod, 0, "", objParametri)

            If righeGrid_Clausole <> "" And righeGrid_Clausole <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeGrid_Clausole)

                    'Inserimento Clausola                    
                    bOk = objClausola.ScriviContrattoxClausola(Piva, Contratto_Cod, CInt(obj("Clausola_Cod")), 0, CDate(obj("Data")), Validita_Inizio, Validita_Fine, objParametri)

                    If Not bOk Then
                        Throw New Exception
                    End If

                Next

            End If



            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, strErr)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Contratto_Cod

    End Function

    Public Function AggiornaClausole(ByVal piva As String,
                                     ByVal validita_inizio As Date,
                                     ByVal validita_fine As Date,
                                     ByVal righeInseriteGrid_Clausole As String,
                                     ByVal righeModificateGrid_Clausole As String,
                                     ByVal righeCancellateGrid_Clausole As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W.AggiornaClausole()"
        Dim strErr As String = ""
        Dim Clausola_Cod As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim PivaSuperUser = objParametri.PivaSuperUser
            Dim objClausola As New AgronicaCoreContabDAL.Imprese_Contratti_Clausole_W
            Dim Validita_Inizio_Clausole As Date = AGRODATAINIZIO
            Dim Validita_Fine_Clausole As Date = AGRODATAFINE


            ' forzati
            Dim Base, Top As Integer

            Base = 0
            Top = 2000000000

            piva = "" 'Non Gestito


            If righeInseriteGrid_Clausole <> "" And righeInseriteGrid_Clausole <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeInseriteGrid_Clausole)

                    Clausola_Cod = objSeq.NuovoId_Tabella("imprese_contratti_clausole", Base, Top, objParametri)

                    'If IsDate(obj("Validita_Inizio").ToString) Then
                    'Validita_Inizio_Clausole = CDate(obj("Validita_Inizio").ToString)
                    'Else
                    'Validita_Inizio_Clausole = AGRODATAINIZIO
                    'End If

                    'If IsDate(obj("Validita_Fine").ToString) Then
                    'Validita_Fine_Clausole = CDate(obj("Validita_Fine").ToString)
                    'Else
                    'Validita_Fine_Clausole = AGRODATAFINE
                    'End If

                    objClausola.Scrivi(piva, Clausola_Cod, CStr(obj("Clausola_Numero")), CStr(obj("Clausola_Nome")),
                                       CStr(obj("Clausola_Des")), CStr(obj("Cau_Contratto")), CStr(obj("Clausola_Cod_Alternativo")),
                                       Validita_Inizio_Clausole, Validita_Fine_Clausole, objParametri)

                Next

            End If


            If righeModificateGrid_Clausole <> "" And righeModificateGrid_Clausole <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeModificateGrid_Clausole)

                    If IsDate(obj("Validita_Inizio").ToString) Then
                        Validita_Inizio_Clausole = CDate(obj("Validita_Inizio").ToString)
                    Else
                        Validita_Inizio_Clausole = AGRODATAINIZIO
                    End If

                    If IsDate(obj("Validita_Fine").ToString) Then
                        Validita_Fine_Clausole = CDate(obj("Validita_Fine").ToString)
                    Else
                        Validita_Fine_Clausole = AGRODATAFINE
                    End If

                    objClausola.Modifica(CStr(obj("Piva")), CInt(obj("Clausola_Cod")), CStr(obj("Clausola_Numero")), CStr(obj("Clausola_Nome")),
                                       CStr(obj("Clausola_Des")), CStr(obj("Cau_Contratto")), CStr(obj("Clausola_Cod_Alternativo")),
                                       Validita_Inizio_Clausole, Validita_Fine_Clausole, objParametri)

                Next

            End If


            If righeCancellateGrid_Clausole <> "" And righeCancellateGrid_Clausole <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeCancellateGrid_Clausole)

                    objClausola.Cancella(CStr(obj("Piva")), CInt(obj("Clausola_Cod")), "", objParametri)

                Next

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, strErr)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Clausola_Cod

    End Function

    Public Function Cancella(ByVal Piva As String,
                             ByVal Contratto_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W.Cancella()"
        Dim strErr As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim objImprese_Contratti As New AgronicaCoreContabDAL.Imprese_Contratti_W
            Dim objImprese_Contratti_Fasi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_W
            Dim objClausola As New AgronicaCoreContabDAL.Imprese_Contratti_Clausole_W
            Dim objTrasformazioni As New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

            'Controllo Coerenza
            If Contratto_Cod <> 0 Then

                objTrasformazioni.Cancella(Piva, Contratto_Cod, 0, 0, 0, "", objParametri)
                objClausola.CancellaContrattoxClausola(Piva, Contratto_Cod, 0, "", objParametri)
                objImprese_Contratti_Fasi.Cancella(Piva, Contratto_Cod, 0, "", objParametri)
                objImprese_Contratti.Cancella(Piva, Contratto_Cod, "", objParametri)

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, strErr)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Contratto_Cod

    End Function

    Public Function saveRequisitiStabilimento(
                                             ByVal rows As Object(),
                                             ByVal objParametri As AgronicaCoreParametri
                                             )

        Dim objImpreseContrattiBIZR As New Imprese_Contratti_BIZ_R
        For Each row In rows.Cast(Of Dictionary(Of String, Object))
            Dim prefix As String
            Dim values As New Dictionary(Of String, Object)
            Dim contrattoCodPadre As Integer
            Dim faseCodPadre As Integer

            For Each key In row.Keys.Where(Function(s As String) Char.IsDigit(s(0)))
                Dim prefixTemp = String.Join("_", key.Split("_").Take(2))

                If (prefix Is Nothing) Then
                    prefix = prefixTemp
                    contrattoCodPadre = key.Split("_").Take(2).First
                    faseCodPadre = key.Split("_").Take(2).Last
                ElseIf prefix <> prefixTemp Then
                    If values.Count <> 0 Then
                        saveRowRequisitiStabilimento(row("PIVA"), contrattoCodPadre, faseCodPadre, values, objParametri)
                    End If
                    contrattoCodPadre = key.Split("_").Take(2).First
                    faseCodPadre = key.Split("_").Take(2).Last
                    prefix = prefixTemp
                    values.Clear()
                End If

                If key.StartsWith(prefix) Then
                    values.Add(key, row(key))
                End If
            Next
            If values.Count <> 0 Then
                saveRowRequisitiStabilimento(row("PIVA"), contrattoCodPadre, faseCodPadre, values, objParametri)
            End If
        Next
    End Function

    Private Function saveRowRequisitiStabilimento(
                                                 ByVal piva As String,
                                                 ByVal contrattoCodPadre As Integer,
                                                 ByVal faseCodPadre As Integer,
                                                 ByVal values As Dictionary(Of String, Object),
                                                 ByVal objParametri As AgronicaCoreParametri
                                                 )
        Dim sup As Decimal
        Dim qta As Decimal
        Dim trDes As String
        Dim trCod As Integer
        Dim contrattoCodFiglio As Integer
        Dim faseCodFiglio As Integer
        Dim objImpreseContrattiW As New Imprese_Contratti_W
        Dim objImpreseContrattoFasiW As New Imprese_Contratto_Fasi_W

        For Each value In values
            If value.Key.Contains("Superfice") Then
                sup = value.Value
            End If
            If value.Key.Contains("Quantita") Then
                qta = value.Value
            End If
            If value.Key.Contains("TrasportoDes") Then
                trDes = value.Value
            End If
            If value.Key.Contains("TrasportoCod") Then
                trCod = value.Value
            End If
            If value.Key.Contains("ContrattoCod") Then
                contrattoCodFiglio = If(value.Value Is Nothing, 0, value.Value)
            End If
            If value.Key.Contains("FaseCod") Then
                faseCodFiglio = If(value.Value Is Nothing, 0, value.Value)
            End If
        Next

        If sup > 0 AndAlso qta > 0 Then
            'TODO Scrittura / Modifica
            updateContractRequisitiStabilimento(piva, sup, qta, trDes, trCod, contrattoCodFiglio, faseCodFiglio, contrattoCodPadre, faseCodPadre, objParametri)
        ElseIf qta = 0 AndAlso sup = 0 AndAlso contrattoCodFiglio <> 0 AndAlso faseCodFiglio <> 0 Then
            'TODO Cancellazione
            If objImpreseContrattoFasiW.deleteRequisitiStabilimento(contrattoCodFiglio, faseCodFiglio, contrattoCodPadre, faseCodPadre, objParametri) Then
                objImpreseContrattiW.deleteContractFromRequisitiStabilimento(piva, contrattoCodFiglio, faseCodFiglio, contrattoCodPadre, faseCodPadre, objParametri)
            End If
        End If

    End Function

    Public Function updateContractRequisitiStabilimento(
                                                       ByVal piva As String,
                                                       ByVal sup As Decimal,
                                                       ByVal qta As Decimal,
                                                       ByVal trDes As String,
                                                       ByVal trCod As Integer,
                                                       ByVal contrattoCodFiglio As Integer,
                                                       ByVal faseCodFiglio As Integer,
                                                       ByVal contrattoCodPadre As Integer,
                                                       ByVal faseCodPadre As Integer,
                                                       ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       )

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W.updateContractRequisitiStabilimento()"
        Dim strErr As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametriServer)
            Dim objSeq As New Agro_Sequenze
            Dim PivaSuperUser = objParametriServer.PivaSuperUser
            Dim objImprese_Contratti As New Imprese_Contratti_W
            Dim objImpreseContrattoFasiW As New Imprese_Contratto_Fasi_W

            ' forzati
            Dim Base, Top As Integer
            Base = 0
            Top = 2000000000

            'Controllo Salvataggio o Modifica
            Select Case contrattoCodFiglio
                Case 0
                    'Inserimento Contratto
                    contrattoCodFiglio = objSeq.NuovoId_Tabella("impresa_contratto", Base, Top, objParametriServer)
                    faseCodFiglio = objSeq.NuovoId_Tabella("impresa_contratto_fasi", Base, Top, objParametriServer)

                    If objImprese_Contratti.writeContractFromRequisitiStabilimento(piva, sup, qta, trDes, trCod, contrattoCodFiglio, contrattoCodPadre, faseCodPadre, objParametriServer) Then
                        objImpreseContrattoFasiW.writeRequisitiStabilimento(piva, sup, qta, trDes, trCod, contrattoCodPadre, faseCodPadre, contrattoCodFiglio, faseCodFiglio, objParametriServer)
                    End If
                Case Else
                    objImpreseContrattoFasiW.editRequisitiStabilimento(sup, qta, trDes, trCod, contrattoCodFiglio, faseCodFiglio, contrattoCodPadre, faseCodPadre, objParametriServer)
                    'objImprese_Contratti.editContractFromRequisitiStabilimento(piva, sup, qta, trDes, trCod, contrattoCodFiglio, faseCodFiglio, contrattoCodPadre, faseCodPadre, objParametriServer)
            End Select

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)
        Catch ex As Exception
            If Not objParametriServer.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
            strErr = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, strErr)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametriServer)
        End Try

        Return contrattoCodFiglio

    End Function

End Class

