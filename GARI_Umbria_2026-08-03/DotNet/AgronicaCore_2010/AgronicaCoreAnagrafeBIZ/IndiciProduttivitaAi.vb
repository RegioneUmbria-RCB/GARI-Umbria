Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreModelsSTD.analisi.correzioni

Public Class IndiciProduttivitaAi_R
    Inherits DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal sa_cod As Integer,
                          ByVal appezza As Integer,
                          ByVal id_reg As Integer,
                          ByVal IDIndiciProduttivita As Integer,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As Date = AGRODATAFINE) As AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_R.Leggi()"
        Dim MessaggioErrore As String = ""

        Dim hDal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_R
        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_R

        Dim ret As AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI = Nothing
        Try

            Dim dtH = hDal.Leggi(IDIndiciProduttivita, piva, sa_cod, appezza, id_reg, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri, Validita_Inizio, Validita_Fine)
            If dtH.Rows.Count > 0 Then
                ret = New AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI
                ret.indici = New List(Of AgronicaCoreModelsSTD.Indici_rischio.IndiciProduttivitaAI)

                For Each row In dtH.Rows
                    Dim dtD = dDal.Leggi(row("IndiciProduttivitaAi_COD"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, Validita_Inizio, Validita_Fine)
                    For Each rowd In dtD.Rows
                        ret.indici.Add(New AgronicaCoreModelsSTD.Indici_rischio.IndiciProduttivitaAI() With {
                                        .id = rowd("IndiciProduttivitaAi_COD"),
                                        .Produttivita = rowd("Produttivita"),
                                        .plv = rowd("plv"),
                                        .IndiceCO2 = rowd("IndiceCO2"),
                                        .IndiceErosione = rowd("IndiceErosione"),
                                        .IndiceRischioAllagamento = rowd("IndiceRischioAllagamento"),
                                        .IndiceRischioGelata = rowd("IndiceRischioGelata"),
                                        .IndiceRischioGrandine = rowd("IndiceRischioGrandine"),
                                        .IndiceRischioMeteoAggregato = rowd("IndiceRischioMeteoAggregato"),
                                        .IndiceRischioVentoForte = rowd("IndiceRischioVentoForte"),
                                        .IndiceRischioSiccita = rowd("IndiceRischioSiccita"),
                                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CType(rowd("Validita_Inizio"), Date), CType(rowd("Validita_Fine"), Date))
                                       })
                    Next
                Next


                ret.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CType(dtH.Rows(0)("Validita_Inizio"), Date), CType(dtH.Rows(0)("Validita_Fine"), Date))
                ret.impiantoPK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(dtH.Rows(0)("id_Reg"),
                                                                                   New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(dtH.Rows(0)("Appezza"),
                                                                                                                                         New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(dtH.Rows(0)("sa_cod"),
                                                                                                                                                                                                  dtH.Rows(0)("piva"))
                                                                                                                                        ))
            End If


        Catch ex As Exception
            ret = Nothing

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return ret

    End Function


End Class
Public Class IndiciProduttivitaAi_W
    Inherits DataProvider

    Public Function Scrivi(ByVal data As AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim hDal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_W
        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W
        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)


            For Each indice In data.indici

                If indice.id <= 0 Then

                    '    Dim NewSeq = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                    '    "IndiciProduttivitaAi",
                    '    objParametri
                    ')
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    Dim NewSeq = AgroSequenze.NuovoId_Tabella("IndiciProduttivitaAi", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)


                    If NewSeq = -1 Then
                        Throw New Exception("Errore in stack contatore IndiciProduttivitaAi")
                    Else
                        indice.id = NewSeq
                    End If

                    Dim retH = hDal.Scrivi(indice.id,
                            data.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                            data.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                            data.impiantoPK.appezzamentoPK.codice,
                            data.impiantoPK.codice,
                            objParametri,
                            data.validita.inizio,
                            data.validita.fine)

                    Dim retD = dDal.Scrivi(indice.id,
                            indice.Produttivita,
                            indice.plv,
                            indice.IndiceErosione,
                            indice.IndiceCO2,
                            indice.IndiceRischioMeteoAggregato,
                            indice.IndiceRischioGelata,
                            indice.IndiceRischioVentoForte,
                            indice.IndiceRischioSiccita,
                            indice.IndiceRischioGrandine,
                            indice.IndiceRischioAllagamento,
                            indice.port_Produttivita,
                            indice.port_plv,
                            indice.port_IndiceErosione,
                            indice.port_IndiceCO2,
                            indice.port_IndiceRischioMeteoAggregato,
                            indice.port_IndiceRischioGelata,
                            indice.port_IndiceRischioVentoForte,
                            indice.port_IndiceRischioSiccita,
                            indice.port_IndiceRischioGrandine,
                            indice.port_IndiceRischioAllagamento,
                            indice.reg_Produttivita,
                            indice.reg_plv,
                            indice.reg_IndiceErosione,
                            indice.reg_IndiceCO2,
                            indice.reg_IndiceRischioMeteoAggregato,
                            indice.reg_IndiceRischioGelata,
                            indice.reg_IndiceRischioVentoForte,
                            indice.reg_IndiceRischioSiccita,
                            indice.reg_IndiceRischioGrandine,
                            indice.reg_IndiceRischioAllagamento,
                            objParametri,
                            indice.validita.inizio,
                            indice.validita.fine
                            )
                Else
                    Dim retH = hDal.Modifica(indice.id,
                                data.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                data.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                data.impiantoPK.appezzamentoPK.codice,
                                data.impiantoPK.codice,
                                objParametri,
                                data.validita.inizio,
                                data.validita.fine)

                    Dim retD = dDal.Modifica(indice.id,
                                indice.Produttivita,
                                indice.plv,
                                indice.IndiceErosione,
                                indice.IndiceCO2,
                                indice.IndiceRischioMeteoAggregato,
                                indice.IndiceRischioGelata,
                                indice.IndiceRischioVentoForte,
                                indice.IndiceRischioSiccita,
                                indice.IndiceRischioGrandine,
                                indice.IndiceRischioAllagamento,
                                indice.port_Produttivita,
                                indice.port_plv,
                                indice.port_IndiceErosione,
                                indice.port_IndiceCO2,
                                indice.port_IndiceRischioMeteoAggregato,
                                indice.port_IndiceRischioGelata,
                                indice.port_IndiceRischioVentoForte,
                                indice.port_IndiceRischioSiccita,
                                indice.port_IndiceRischioGrandine,
                                indice.port_IndiceRischioAllagamento,
                                indice.reg_Produttivita,
                                indice.reg_plv,
                                indice.reg_IndiceErosione,
                                indice.reg_IndiceCO2,
                                indice.reg_IndiceRischioMeteoAggregato,
                                indice.reg_IndiceRischioGelata,
                                indice.reg_IndiceRischioVentoForte,
                                indice.reg_IndiceRischioSiccita,
                                indice.reg_IndiceRischioGrandine,
                                indice.reg_IndiceRischioAllagamento,
                                objParametri,
                                indice.validita.inizio,
                                indice.validita.fine
                                )
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            xRisp = True

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        objParametri)
        End Try
        Return xRisp
    End Function

    Public Function Modifica(ByVal data As AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.Modifica()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim hDal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_W
        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W
        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)


            For Each indice In data.indici
                If indice.flag_cancellazione = True Then
                    Dim retH = hDal.Cancella(indice.id,
                                             data.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                             data.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                             data.impiantoPK.appezzamentoPK.codice,
                                             data.impiantoPK.codice,
                                             "",
                                             objParametri
                        )

                    Dim retD = dDal.Cancella(indice.id,
                                             "",
                                             objParametri
                        )
                Else
                    Dim retD = dDal.Modifica(indice.id,
                        indice.Produttivita,
                        indice.plv,
                        indice.IndiceErosione,
                        indice.IndiceCO2,
                        indice.IndiceRischioMeteoAggregato,
                        indice.IndiceRischioGelata,
                        indice.IndiceRischioVentoForte,
                        indice.IndiceRischioSiccita,
                        indice.IndiceRischioGrandine,
                        indice.IndiceRischioAllagamento,
                        indice.port_Produttivita,
                        indice.port_plv,
                        indice.port_IndiceErosione,
                        indice.port_IndiceCO2,
                        indice.port_IndiceRischioMeteoAggregato,
                        indice.port_IndiceRischioGelata,
                        indice.port_IndiceRischioVentoForte,
                        indice.port_IndiceRischioSiccita,
                        indice.port_IndiceRischioGrandine,
                        indice.port_IndiceRischioAllagamento,
                        indice.reg_Produttivita,
                        indice.reg_plv,
                        indice.reg_IndiceErosione,
                        indice.reg_IndiceCO2,
                        indice.reg_IndiceRischioMeteoAggregato,
                        indice.reg_IndiceRischioGelata,
                        indice.reg_IndiceRischioVentoForte,
                        indice.reg_IndiceRischioSiccita,
                        indice.reg_IndiceRischioGrandine,
                        indice.reg_IndiceRischioAllagamento,
                        objParametri,
                        indice.validita.inizio,
                        indice.validita.fine
                        )
                End If

            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            xRisp = True

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        objParametri)
        End Try
        Return xRisp
    End Function

    Public Function Cancella(ByVal data As AgronicaCoreModelsSTD.Indici_rischio.Reg_Impianti_XIndiciProduttivitaAI,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.Cancella()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim hDal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_XIndiciProduttivitaAI_W
        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W
        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)


            For Each indice In data.indici
                If indice.flag_cancellazione = True Then
                    Dim retH = hDal.Cancella(indice.id,
                                             data.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                             data.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                             data.impiantoPK.appezzamentoPK.codice,
                                             data.impiantoPK.codice,
                                             "",
                                             objParametri
                        )

                    Dim retD = dDal.Cancella(indice.id,
                                             "",
                                             objParametri
                        )
                End If

            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            xRisp = True

        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        objParametri)
        End Try
        Return xRisp
    End Function

    Public Function RicalcolaMediaRegionale(ByVal CodRegione As String,
                                            ByVal ValiditaInizio As DateTime,
                                            ByVal ValiditaFine As DateTime,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.RicalcolaMediaRegionale()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W

        Try
            xRisp = dDal.RicalcolaMediaRegionale(CodRegione, ValiditaInizio, ValiditaFine, objParametri)
        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function

    Public Function RicalcolaMinMax(ByVal ValiditaInizio As DateTime,
                                    ByVal ValiditaFine As DateTime,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.RicalcolaMinMax()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W
        Try
            xRisp = dDal.RicalcolaMinMax(ValiditaInizio, ValiditaFine, objParametri)
        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function

    Public Function RicalcolaMediaGenerale(ByVal ValiditaInizio As DateTime,
                                ByVal ValiditaFine As DateTime,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.IndiciProduttivitaAi_W.RicalcolaMediaGenerale()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim dDal As New AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W
        Try
            xRisp = dDal.RicalcolaMediaGenerale(ValiditaInizio, ValiditaFine, objParametri)
        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function

End Class