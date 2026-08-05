Imports System.Data.Entity
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Transactions
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaConversioneCartografiaGias
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.InData.FiltroRicerca
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreGisBIZ
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.anagrafiche.Appezzamento
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json

Public Class Appezzamento_R
    Inherits AgronicaCoreDataProvider.LogProvider

    ''' <summary>
    ''' Per l'appezzamento passato come parametro legge le precessioni colturali attraverso elementi sovrapposti (distinte, altri appezzamenti con lo stesso riparto del catasto)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="coltura_Precedente_1">Descrizione della coltura precedente</param>
    ''' <param name="coltura_Precedente_2">Descrizione della coltura precedente</param>
    ''' <param name="coltura_Precedente_3">Descrizione della coltura precedente</param>
    ''' <param name="coltura_Precedente_4">Descrizione della coltura precedente</param>
    ''' <param name="objparametri_server"></param>
    Public Sub LetturaPrecessioniDaElementiCollegati(
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Appezza As Integer,
        ByRef coltura_Precedente_1 As String,
        ByRef coltura_Precedente_2 As String,
        ByRef coltura_Precedente_3 As String,
        ByRef coltura_Precedente_4 As String,
        ByVal objparametri_server As AgronicaCoreParametri,
        ByVal objparametri_Utenti As AgronicaCoreParametri
    )

        coltura_Precedente_1 = ""
        coltura_Precedente_2 = ""
        coltura_Precedente_3 = ""
        coltura_Precedente_4 = ""

        Dim objLeggiAppezzamentoDal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim xDtcolduraPrec As DataTable =
            objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto(Piva, Sa_Cod, Appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objparametri_server)

        If Not IsNothing(xDtcolduraPrec) AndAlso xDtcolduraPrec.Rows.Count > 0 Then

            Dim utentiImpostazioniLeggi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim AnnataAgrariariferimanto_da As Date
            Dim AnnataAgrariariferimanto_a As Date

            Dim DataRiferimentoDistintaMax As Date
            Dim drowDataRif As DataRow() = xDtcolduraPrec.Select("Risultato = 'Questo Appezzamento'", " Validita_Inizio DESC ")

            If Not IsNothing(drowDataRif) AndAlso drowDataRif.Length > 0 Then

                DataRiferimentoDistintaMax = drowDataRif(0)("Validita_Inizio")

                utentiImpostazioniLeggi.AnnataAgraria(DataRiferimentoDistintaMax, AnnataAgrariariferimanto_da, AnnataAgrariariferimanto_a, objparametri_Utenti)

                Dim prec As New List(Of String)
                Dim aggiungi As Boolean = False

                For annoCorrente = 0 To 3

                    Dim DataDa As String = AnnataAgrariariferimanto_da.AddYears(-(annoCorrente + 1)).ToString("MM/dd/yyyy")
                    Dim DataA As String = AnnataAgrariariferimanto_a.AddYears(-(annoCorrente + 1)).ToString("MM/dd/yyyy")
                    Dim listaColturaDrow As DataRow() = xDtcolduraPrec.Select(" validita_inizio >=#" & DataDa & "# and validita_fine <= #" & DataA & "#")
                    Dim listaColtPrec As New List(Of String)

                    For Each rowColturaPrec In listaColturaDrow
                        aggiungi = True
                        If Not listaColtPrec.Contains(rowColturaPrec("Utilizzo")) Then
                            listaColtPrec.Add(rowColturaPrec("Utilizzo"))
                        End If
                    Next
                    If aggiungi Then
                        prec.Add(String.Join(",", listaColtPrec))
                    End If

                Next

                Dim i As Integer = 1
                For Each cp In prec
                    Select Case i
                        Case 1
                            coltura_Precedente_1 = cp
                        Case 2
                            coltura_Precedente_2 = cp
                        Case 3
                            coltura_Precedente_3 = cp
                        Case 4
                            coltura_Precedente_4 = cp
                    End Select
                    i += 1
                Next

            End If

        End If

    End Sub

    Public Sub LetturaVincoliAgronomiciDaElementiCollegati(
        ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer,
        ByVal puaCod As String, ByVal regolamentoCod As String,
        ByVal dataInizio As Date, ByVal dataFine As Date,
        ByRef Veg_Cod As Integer,
        ByRef Analisi_Testata_Cod As Integer,
        ByRef Ubicazione_Cod As Integer,
        ByRef TipoAcqua_Cod As Integer,
        ByVal objparametri_server As AgronicaCoreParametri
    )

        Veg_Cod = 0
        Analisi_Testata_Cod = 0
        Ubicazione_Cod = 0
        TipoAcqua_Cod = 0

        Dim objLeggiAppezzamentoDal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read


        Dim xDtcolduraPrec As DataTable =
            objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto_VincoliAgronomici(Piva, Sa_Cod, Appezza, puaCod, regolamentoCod, "", "", objparametri_server)

        If Not IsNothing(xDtcolduraPrec) AndAlso xDtcolduraPrec.Rows.Count > 0 Then

            If Not IsDBNull(xDtcolduraPrec.Rows(0).Item("veg_cod")) Then
                Veg_Cod = xDtcolduraPrec.Rows(0).Item("veg_cod")
            End If

            'considero l'analisi solo se non scaduta
            If Not IsDBNull(xDtcolduraPrec.Rows(0).Item("Analisi_Testata_Cod")) Then
                If Not IsDBNull(xDtcolduraPrec.Rows(0).Item("Analisi_Testata_Data_Fine")) Then
                    Dim Analisi_Testata_Data_Fine As Date = xDtcolduraPrec.Rows(0).Item("Analisi_Testata_Data_Fine")
                    If Analisi_Testata_Data_Fine >= dataInizio Then
                        Analisi_Testata_Cod = xDtcolduraPrec.Rows(0).Item("Analisi_Testata_Cod")
                    End If
                End If
            End If

            If Not IsDBNull(xDtcolduraPrec.Rows(0).Item("Ubicazione_Cod")) Then
                Ubicazione_Cod = xDtcolduraPrec.Rows(0).Item("Ubicazione_Cod")
            End If
            If Not IsDBNull(xDtcolduraPrec.Rows(0).Item("TipoAcqua_Cod")) Then
                TipoAcqua_Cod = xDtcolduraPrec.Rows(0).Item("TipoAcqua_Cod")
            End If

        End If

    End Sub

    Public Sub LetturaPUALetamazioniPrecedentiDaElementiCollegati(
        ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
        ByVal puaCod As String, ByVal regolamentoCod As String,
        ByVal dataInizio As Date, ByVal dataFine As Date,
        ByVal objEFOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output,
        ByRef N_FertilizzazioniPrecedenti As Decimal,
        ByVal objparametri_server As AgronicaCoreParametri
    )

        N_FertilizzazioniPrecedenti = 0

        Dim Id_Fre As Integer
        Dim N_Riduzione As Decimal


        Dim objLeggiAppezzamentoDal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objPUA_LetPrec_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

        Dim bRet As Boolean

        Dim xDtcolduraPrec As DataTable =
            objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto_PUALetamazioniPrecedenti(Piva, Sa_Cod, Appezza, puaCod, regolamentoCod, "", "", objparametri_server)

        If Not IsNothing(xDtcolduraPrec) AndAlso xDtcolduraPrec.Rows.Count > 0 Then

            For Each Dr As DataRow In xDtcolduraPrec.Rows

                Select Case Dr.Item("id_fre")

                    Case 1, 2 'ANNO CORRENTE,ANNO PRECEDENTE

                        'aggiorno la frequenza slittandola indietro
                        Id_Fre = Dr.Item("id_fre") + 1

                        N_Riduzione = 0

                        Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                        Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Eff_Cod = Dr("eff_cod") AndAlso x.Id_Fre = Id_Fre)(0)
                        If Effluente IsNot Nothing Then
                            N_Riduzione = Effluente.N
                        End If

                        If Not IsDBNull(Dr("n_titolo")) AndAlso Not IsDBNull(Dr("qta")) Then
                            N_FertilizzazioniPrecedenti += Math.Round(CDec(Dr("qta")) * CDec(Dr("n_titolo")) * N_Riduzione, 2)
                        End If

                        Dim Id As Integer = 0
                        bRet = objPUA_LetPrec_W.Scrivi(Id, regolamentoCod, puaCod,
                               Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                               Dr("eff_cod"), Id_Fre, Dr("udm_cod"),
                               If(IsNumeric(Dr("qta")), Dr("qta"), 0), If(IsNumeric(Dr("n_titolo")), Dr("n_titolo"), 0),
                                AGRODATAINIZIO, AGRODATAFINE, objparametri_server)

                        If bRet Then
                            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W
                            objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "PUA_LetamazioniPrecedenti", puaCod, regolamentoCod, Id, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Dr("eff_cod"), Id_Fre, "", objparametri_server)
                        End If


                End Select

            Next

        End If

    End Sub

    Public Sub LetturaLetamazioniPrecedentiDaElementiCollegati(
        ByVal regolamentoCod As Integer, ByVal puaCod As Integer,
        ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
        ByVal dataInizio As Date, ByVal dataFine As Date,
        ByVal objEFOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output,
        ByVal str_FerCod_Letami As String,
        ByRef N_FertilizzazioniPrecedenti As Decimal,
        ByVal objparametri_server As AgronicaCoreParametri
    )

        N_FertilizzazioniPrecedenti = 0

        Dim Anno_Precedente_Inizio As Date = DateAdd(DateInterval.Year, -1, dataInizio)
        Dim Anno_Precedente_Fine As Date = DateAdd(DateInterval.Year, -1, dataFine)
        Dim Anno_2Precedente_Inizio As Date = DateAdd(DateInterval.Year, -2, dataInizio)
        Dim Anno_2Precedente_Fine As Date = DateAdd(DateInterval.Year, -2, dataFine)
        Dim Anno_3Precedente_Inizio As Date = DateAdd(DateInterval.Year, -3, dataInizio)
        Dim Anno_3Precedente_Fine As Date = DateAdd(DateInterval.Year, -3, dataFine)

        Dim objLeggiAppezzamentoDal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objPUA_LetPrec_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W
        Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

        Dim bRet As Boolean

        Dim FiltroMovimenti As String = " m.cau_mov ='2300' and m.data_movimento < " & Agro_SQL_SaveDate(dataInizio)
        Dim FiltroMovimentiDettagli As String = " md.elem_cod=3 and md.pro_cod in " & str_FerCod_Letami

        Dim xDtcolduraPrec As DataTable =
            objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto_LetamazioniPrecedenti(Piva, Sa_Cod, Appezza, FiltroMovimenti, FiltroMovimentiDettagli, "", objparametri_server)

        If Not IsNothing(xDtcolduraPrec) AndAlso xDtcolduraPrec.Rows.Count > 0 Then

            Dim DR_Anno_Precedente As DataRow() '--> anno corrente
            Dim DR_2Anni_Precedenti As DataRow() '--> anno precedente
            Dim DR_3Anni_Precedenti As DataRow() '--> 2 anno precedente

            Dim Titolo As Decimal
            Dim ExtraInt As Integer
            Dim Qta As Decimal
            Dim QtaEff As Decimal

            'anno corrente --> conteggio il letame distribuito lo scorso anno sui terreni nudi
            DR_Anno_Precedente = xDtcolduraPrec.Select("cul_cod=0 and data_movimento<=#" & Anno_Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_Precedente_Inizio.ToString("MM/dd/yyyy") & "#", "pro_cod")

            If DR_Anno_Precedente IsNot Nothing AndAlso DR_Anno_Precedente.Length > 0 Then

                Dim HashEffCod As New Hashtable
                Dim HashIdAgenda As New Hashtable
                'chiave effcod_udmcod_titolo_idfre_riduz
                'valore qta

                Dim Id_Fre As Integer = 1
                QtaEff = 0

                For Each Dr As DataRow In DR_Anno_Precedente

                    Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                    Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Fer_Cod = Dr("pro_cod") And x.Id_Fre = Id_Fre)(0)

                    '(07/10/2020 fede) per la stessa operazione e lo stesso effluente conteggio solo una volta la qta
                    If Effluente IsNot Nothing AndAlso Not HashIdAgenda.ContainsKey(Dr("id_agenda") & "|" & Dr("pro_cod")) Then

                        Titolo = Dr.Item("n")
                        ExtraInt = Dr.Item("extra_int")
                        Qta = Dr.Item("qta")

                        If ExtraInt <> Effluente.Udm_Cod Then
                            Qta = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(ExtraInt, Qta, Effluente.Udm_Cod)
                        End If

                        If Not HashEffCod.ContainsKey(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) Then
                            HashEffCod.Add(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N, Qta)
                        Else
                            HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) = HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) + Qta
                        End If

                        HashIdAgenda.Add(Dr("id_agenda") & "|" & Dr("pro_cod"), "")

                    End If


                Next

                For Each eff In HashEffCod.Keys

                    Dim Id As Integer = 0
                    bRet = objPUA_LetPrec_W.Scrivi(Id, regolamentoCod, puaCod,
                                      Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                      Split(eff, "_")(0), Id_Fre, Split(eff, "_")(1),
                                      If(IsNumeric(HashEffCod(eff)), HashEffCod(eff), 0), If(IsNumeric(Split(eff, "_")(2)), Split(eff, "_")(2), 0),
                                       AGRODATAINIZIO, AGRODATAFINE, objparametri_server)

                    N_FertilizzazioniPrecedenti += HashEffCod(eff) * Split(eff, "_")(2) * Split(eff, "_")(4)

                    If bRet Then
                        objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "PUA_LetamazioniPrecedenti", puaCod, regolamentoCod, Id, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Split(eff, "_")(0), Id_Fre, "", objparametri_server)
                    End If

                Next

            End If

            'anno precedente --> conteggio il letame distribuito lo scorso anno sulle colture + quello distribuito sui terreni nudi 2 anni prima
            DR_2Anni_Precedenti = xDtcolduraPrec.Select("(cul_cod=0 and data_movimento<=#" & Anno_2Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_2Precedente_Inizio.ToString("MM/dd/yyyy") & "#) OR (cul_cod<>0 and data_movimento<=#" & Anno_Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_Precedente_Inizio.ToString("MM/dd/yyyy") & "#)", "pro_cod")

            If DR_2Anni_Precedenti IsNot Nothing AndAlso DR_2Anni_Precedenti.Length > 0 Then

                Dim HashEffCod As New Hashtable
                Dim HashIdAgenda As New Hashtable
                'chiave effcod_udmcod_titolo_idfre_riduz
                'valore qta

                Dim Id_Fre As Integer = 2
                QtaEff = 0

                For Each Dr As DataRow In DR_2Anni_Precedenti

                    Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                    Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Fer_Cod = Dr("pro_cod") And x.Id_Fre = Id_Fre)(0)

                    If Effluente IsNot Nothing AndAlso Not HashIdAgenda.ContainsKey(Dr("id_agenda") & "|" & Dr("pro_cod")) Then

                        Titolo = Dr.Item("n")
                        ExtraInt = Dr.Item("extra_int")
                        Qta = Dr.Item("qta")

                        If ExtraInt <> Effluente.Udm_Cod Then
                            Qta = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(ExtraInt, Qta, Effluente.Udm_Cod)
                        End If

                        If Not HashEffCod.ContainsKey(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) Then
                            HashEffCod.Add(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N, Qta)
                        Else
                            HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) = HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) + Qta
                        End If

                        HashIdAgenda.Add(Dr("id_agenda") & "|" & Dr("pro_cod"), "")

                    End If


                Next

                For Each eff In HashEffCod.Keys

                    Dim Id As Integer = 0
                    bRet = objPUA_LetPrec_W.Scrivi(Id, regolamentoCod, puaCod,
                                      Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                      Split(eff, "_")(0), Id_Fre, Split(eff, "_")(1),
                                      If(IsNumeric(HashEffCod(eff)), HashEffCod(eff), 0), If(IsNumeric(Split(eff, "_")(2)), Split(eff, "_")(2), 0),
                                       AGRODATAINIZIO, AGRODATAFINE, objparametri_server)

                    N_FertilizzazioniPrecedenti += HashEffCod(eff) * Split(eff, "_")(2) * Split(eff, "_")(4)

                    If bRet Then
                        objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "PUA_LetamazioniPrecedenti", puaCod, regolamentoCod, Id, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Split(eff, "_")(0), Id_Fre, "", objparametri_server)
                    End If
                Next

            End If



            '2 anni precedenti --> conteggio il letame distribuito nei 2 anni precedenti sulle colture + quello distribuito sui terreni nudi 3 anni prima
            DR_3Anni_Precedenti = xDtcolduraPrec.Select("(cul_cod=0 and data_movimento<=#" & Anno_3Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_3Precedente_Inizio.ToString("MM/dd/yyyy") & "#) OR (cul_cod<>0 and data_movimento<=#" & Anno_2Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_2Precedente_Inizio.ToString("MM/dd/yyyy") & "#)", "pro_cod")

            If DR_3Anni_Precedenti IsNot Nothing AndAlso DR_3Anni_Precedenti.Length > 0 Then

                Dim HashEffCod As New Hashtable
                Dim HashIdAgenda As New Hashtable
                'chiave effcod_udmcod_titolo_idfre_riduz
                'valore qta

                Dim Id_Fre As Integer = 3
                QtaEff = 0

                For Each Dr As DataRow In DR_3Anni_Precedenti

                    Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                    Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Fer_Cod = Dr("pro_cod") And x.Id_Fre = Id_Fre)(0)

                    If Effluente IsNot Nothing AndAlso Not HashIdAgenda.ContainsKey(Dr("id_agenda") & "|" & Dr("pro_cod")) Then

                        Titolo = Dr.Item("n")
                        ExtraInt = Dr.Item("extra_int")
                        Qta = Dr.Item("qta")

                        If ExtraInt <> Effluente.Udm_Cod Then
                            Qta = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(ExtraInt, Qta, Effluente.Udm_Cod)
                        End If

                        If Not HashEffCod.ContainsKey(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) Then
                            HashEffCod.Add(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N, Qta)
                        Else
                            HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) = HashEffCod(Effluente.Eff_Cod & "_" & Effluente.Udm_Cod & "_" & Titolo & "_" & Id_Fre & "_" & Effluente.N) + Qta
                        End If

                        HashIdAgenda.Add(Dr("id_agenda") & "|" & Dr("pro_cod"), "")

                    End If


                Next

                For Each eff In HashEffCod.Keys

                    Dim Id As Integer = 0
                    bRet = objPUA_LetPrec_W.Scrivi(Id, regolamentoCod, puaCod,
                                      Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                      Split(eff, "_")(0), Id_Fre, Split(eff, "_")(1),
                                      If(IsNumeric(HashEffCod(eff)), HashEffCod(eff), 0), If(IsNumeric(Split(eff, "_")(2)), Split(eff, "_")(2), 0),
                                       AGRODATAINIZIO, AGRODATAFINE, objparametri_server)

                    N_FertilizzazioniPrecedenti += HashEffCod(eff) * Split(eff, "_")(2) * Split(eff, "_")(4)

                    If bRet Then
                        objPUALog.Scrivi(enum_TipoOperazioneDB.Scrittura, "PUA_LetamazioniPrecedenti", puaCod, regolamentoCod, Id, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Split(eff, "_")(0), Id_Fre, "", objparametri_server)
                    End If
                Next

            End If

        End If

    End Sub

    Public Function Appezzamento_Leggi(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Campo_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal ForDelete As Boolean,
                                       ByVal AllAttributes As Boolean,
                                       ByVal SoloNonBloccati As Boolean,
                                       ByVal Filtro As Boolean,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal TipoG2G As Integer = 0,
                                       Optional ByVal LeggiCodiciAnagrafe_ConDescrizioni As Boolean = False
                                       ) As String

        Const nomeRoutine = "AnagrafeBIZ.Appezzamento_R.Appezzamento_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim i As Integer
        Dim j As Integer
        Dim x As Integer
        Dim y As Integer

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument


        Dim XmlDatiAppezzamenti As XmlElement
        Dim XmlAppezzamento As XmlElement
        Dim XmlDatiCodici As XmlElement
        Dim XmlCodice As XmlElement
        Dim XmlDatiAppezzaxParticelle As XmlElement
        Dim XmlAppezzaxParticelle As XmlElement
        Dim XmlDatiAppezzaxParticellexMacrousi As XmlElement
        Dim XmlDatiAppezzaxParticellexMacrousixUtilizzi As XmlElement
        Dim XmlAppezzaxParticellexMacrousi As XmlElement
        Dim XmlAppezzaxParticellexMacrousixUtilizzo As XmlElement
        Dim XmlDatiAppezzamento_Storico As XmlElement
        Dim XmlAppezzamento_Storico As XmlElement = Nothing
        Dim XmlDatiAppezzaxIndirizzi As XmlElement
        Dim XmlAppezzaxIndirizzi As XmlElement


        Dim objAppezzaxParticelle As AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim objAppezzaxParticellexMacrousi As AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R
        Dim objAppezzaxParticellexMacrousixUtilizzo As AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R
        Dim objAppezzaxIndirizzi As AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read
        Dim objIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Read
        Dim objUtentixAppezzamenti As AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_R
        Dim objAppezzaxCodici As AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
        Dim ObjAppezzamento_Storico As AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R

        Dim DtUtentixAppezzamenti As DataTable
        Dim DrUtentixAppezzamenti As DataRow()
        Dim DtAppezzaxParticelle As DataTable
        Dim DtAppezzaxParticellexMacrousi As DataTable
        Dim DtAppezzaxParticellexMacrousixUtilizzo As DataTable
        Dim DtAppezzaxCodici As DataTable
        Dim DtAppezzaxIndirizzi As DataTable
        Dim DtAppezzamento_Storico As DataTable = Nothing

        Dim Parent As Integer
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------


            'Mi procuro un elenco degli Appezzzamenti associati al Campo
            'all'interno della finestra temporale selezionata

            'Creo l'oggetto COM+

            objUtentixAppezzamenti = New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_R

            'If SoloNonBloccati Then

            'Mi procuro il recordset richiesto, prendo solo gli appezzamenti che non sono bloccati
            DtUtentixAppezzamenti = objUtentixAppezzamenti.Leggi(CStr(Piva),
                                                                 CInt(Sa_Cod),
                                                                 CInt(Campo_Cod),
                                                                 CInt(Appezza),
                                                                 SoloNonBloccati,
                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                 "",
                                                                 "",
                                                                 objParametri,
                                                                 TipoG2G)

            '-----------------------------

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtUtentixAppezzamenti.Rows.Count > 0 Then

                '--------------------------------------------------------------------
                If Filtro Then
                    DrUtentixAppezzamenti = DtUtentixAppezzamenti.Select("Validita_Fine > #" & Format(Now, "dd/mm/yyyy") & "#")
                Else
                    DrUtentixAppezzamenti = DtUtentixAppezzamenti.Select("")
                End If

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiAppezzamenti = XmlDoc.CreateElement("DatiAppezzamenti")


                If DrUtentixAppezzamenti IsNot Nothing AndAlso DrUtentixAppezzamenti.Length > 0 Then


                    'Effettuo un ciclo sugli Appezzamenti
                    For i = 0 To DrUtentixAppezzamenti.Length - 1

                        '----- < APPEZZAMENTO > -----
                        XmlAppezzamento = XmlDoc.CreateElement("Appezzamento")

                        '  Marco Grilli, 25/06/2014 15:33:27: ho messo _1 in quanto sono 2 tabelle in join ed alcuni campi sono presenti in entrambe le tabelle
                        With XmlAppezzamento

                            .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            .SetAttribute("piva", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("PIVA")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Sa_Cod")))
                            .SetAttribute("appezza", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezza")))
                            .SetAttribute("sup_app", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Sup_App")))
                            .SetAttribute("x", IIf(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("X")) <> 0, Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("X")), ""))
                            .SetAttribute("y", IIf(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Y")) <> 0, Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Y")), ""))
                            .SetAttribute("zslm", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Zslm")))
                            .SetAttribute("pende", IIf(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Pende")) <> 0, Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Pende")), ""))
                            .SetAttribute("ubicazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Ubicazione")))
                            .SetAttribute("sabbia", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Sabbia")))
                            .SetAttribute("limo", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Limo")))
                            .SetAttribute("argilla", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Argilla")))
                            '.SetAttribute("user", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("User")))
                            .SetAttribute("user", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("User1")))
                            .SetAttribute("app_nome", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("App_Nome")))
                            .SetAttribute("prossimo", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Prossimo")))
                            .SetAttribute("campo_spia", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Campo_Spia")))
                            .SetAttribute("campo_spia_area", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Campo_Spia_Area")))
                            .SetAttribute("cs_sipi", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Cs_Sipi")))
                            .SetAttribute("campo_cod", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Campo_Cod")))
                            .SetAttribute("inviato", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Inviato")))
                            .SetAttribute("datainvio", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("DataInvio")))
                            .SetAttribute("data_creazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Data_Creazione")))
                            .SetAttribute("data_modifica", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Data_Modifica")))
                            .SetAttribute("username_creazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Username_Creazione")))
                            .SetAttribute("username_modifica", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Username_Modifica")))
                            '.SetAttribute("validita_inizio", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Validita_Inizio")))
                            '.SetAttribute("validita_fine", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Validita_Fine")))
                            .SetAttribute("validita_inizio", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Validita_Inizio1")))
                            .SetAttribute("validita_fine", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Validita_Fine1")))
                            .SetAttribute("blk_flag", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_flag")))
                            .SetAttribute("blk_inizio_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_inizio_data")))
                            .SetAttribute("blk_inizio_username", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_inizio_username")))
                            .SetAttribute("blk_inizio_note", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_inizio_note")))
                            .SetAttribute("blk_fine_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_fine_data")))
                            .SetAttribute("blk_fine_username", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_fine_username")))
                            .SetAttribute("blk_fine_note", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("blk_fine_note")))

                            .SetAttribute("data_creazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezzamento_data_creazione")))
                            .SetAttribute("data_modifica", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezzamento_data_modifica")))
                            .SetAttribute("username_creazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezzamento_username_creazione")))
                            .SetAttribute("username_modifica", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezzamento_username_modifica")))

                            .SetAttribute("data_validazione", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("data_validazione1")))

                        End With

                        '-------------------------------------------------------------------------------------------------------
                        ''' Agro_Personalizza.Agro_XmlSetting(XmlAppezzamento, DatiSetting)
                        '-------------------------------------------------------------------------------------------------------

                        'Se voglio tutti gli attributi
                        If AllAttributes Then

                            With XmlAppezzamento
                                .SetAttribute("data_app", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Data_App")))
                                .SetAttribute("ep_camp", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Ep_Camp")))
                                .SetAttribute("esposiz", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Esposiz")))
                                .SetAttribute("num_del", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Num_del")))
                                .SetAttribute("clas", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Clas")))
                                .SetAttribute("ph", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Ph")))
                                .SetAttribute("caltot", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Caltot")))
                                .SetAttribute("calatt", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Calatt")))
                                .SetAttribute("sostorg", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Sostorg")))
                                .SetAttribute("k2oass", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("K2oAss")))
                                .SetAttribute("p2o5ass", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("P2o5Ass")))
                                .SetAttribute("mg", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Mg")))
                                .SetAttribute("ntot", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Ntot")))
                                .SetAttribute("um_s", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Um_s")))
                                .SetAttribute("cl_dren", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Cl_Dren")))
                                .SetAttribute("falda", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Falda")))
                                .SetAttribute("csc", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Csc")))
                                .SetAttribute("k2oass_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("K2oAss_Data")))
                                .SetAttribute("matorg", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("MatOrg")))
                                .SetAttribute("matorg_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("MatOrg_Data")))
                                .SetAttribute("notot_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("NoTot_Data")))
                                .SetAttribute("notot", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("NoTot")))
                                .SetAttribute("p2o5ass_data", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("P2o5Ass_Data")))
                                .SetAttribute("suolo_codattri", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Suolo_CodAttri")))
                                .SetAttribute("data_inizio", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Data_Inizio")))
                                .SetAttribute("data_fine", Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Data_Fine")))

                            End With

                        End If



                        '-------------------------------------------------------------
                        ' APPEZZAMENTO STORICO
                        '-------------------------------------------------------------

                        '----- < Documento XML > -----
                        ' XmlDoc = New XmlDocument

                        XmlDatiAppezzamento_Storico = XmlDoc.CreateElement("DatiAppezzamenti_Storico")

                        'Mi procuro un elenco dei padri dell'appezzamento

                        'Creo l'oggetto COM+
                        ObjAppezzamento_Storico = New AgronicaCoreAnagrafeDAL.Appezzamento_Storico_R

                        For Parent = 0 To 1

                            Select Case Parent

                                Case 0 'LETTURA DA PADRE
                                    'Mi procuro il recordset richiesto
                                    DtAppezzamento_Storico = ObjAppezzamento_Storico.Leggi_Da_Padre(
                                                                           CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                                           CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                                           CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                                           "",
                                                                           0,
                                                                           0,
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "",
                                                                            "",
                                                                            objParametri)

                                Case 1 'LETTURA DA FIGLIO
                                    'Mi procuro il recordset richiesto
                                    DtAppezzamento_Storico = ObjAppezzamento_Storico.Leggi_Da_Figlio(
                                                                           CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                                           CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                                           CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                                           "",
                                                                           0,
                                                                           0,
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "",
                                                                            "",
                                                                            objParametri)


                            End Select

                            '-----------------------------

                            For j = 0 To DtAppezzamento_Storico.Rows.Count - 1

                                '----- < APPEZZAMENTO_STORICO > -----
                                Select Case Parent
                                    Case 0 : XmlAppezzamento_Storico = XmlDoc.CreateElement("Appezzamento_Storico_Figlio")
                                    Case 1 : XmlAppezzamento_Storico = XmlDoc.CreateElement("Appezzamento_Storico_Padre")
                                End Select

                                With XmlAppezzamento_Storico
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("padre_piva", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Padre_Piva")))
                                    .SetAttribute("padre_sa_cod", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Padre_Sa_Cod")))
                                    .SetAttribute("padre_appezza", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Padre_Appezza")))
                                    .SetAttribute("figlio_piva", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Figlio_Piva")))
                                    .SetAttribute("figlio_sa_cod", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Figlio_Sa_Cod")))
                                    .SetAttribute("figlio_appezza", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Figlio_Appezza")))
                                    .SetAttribute("trasferimento_superficie", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Trasferimento_Superficie")))
                                    .SetAttribute("trasferimento_data", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Trasferimento_Data")))
                                    .SetAttribute("inviato", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Inviato")))
                                    .SetAttribute("datainvio", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("DataInvio")))
                                    .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Data_Creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Data_Modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Username_Creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Username_Modifica")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Validita_Inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzamento_Storico.Rows(j).Item("Validita_Fine")))
                                End With

                                XmlDatiAppezzamento_Storico.AppendChild(XmlAppezzamento_Storico)
                                XmlAppezzamento_Storico = Nothing
                            Next

                            DtAppezzamento_Storico.Dispose()
                            DtAppezzamento_Storico = Nothing

                        Next Parent

                        ' distruggo l'oggetto
                        ObjAppezzamento_Storico = Nothing

                        XmlAppezzamento.AppendChild(XmlDatiAppezzamento_Storico)

                        '----- < / Appezzamento_Storico > -----


                        '#################################
                        '#### PARTICELLE CATASTALI  ######
                        '#################################


                        '------------------------------

                        'Mi procuro un elenco delle Particelle associate all'Appezzamento
                        'all'interno della finestra temporale selezionata


                        'Creo l'oggetto COM+
                        objAppezzaxParticelle = New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

                        'Mi procuro il recordset richiesto
                        DtAppezzaxParticelle = objAppezzaxParticelle.LeggiParticelle_Da_Appezzamento(
                                                        CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                        CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                        CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "",
                                                        "",
                                                        objParametri)
                        '-----------------------------

                        'Se ottengo almeno un risultato, creo la struttura XML
                        If DtAppezzaxParticelle.Rows.Count > 0 Then

                            '----- < Documento XML > -----
                            'XmlDoc = New XmlDocument

                            XmlDatiAppezzaxParticelle = XmlDoc.CreateElement("DatiParticelle")

                            'Effettuo un ciclo sulle AppezzAxParticelle
                            For j = 0 To DtAppezzaxParticelle.Rows.Count - 1

                                '----- < APPEZZAXPARTICELLA > -----
                                XmlAppezzaxParticelle = XmlDoc.CreateElement("Particella")

                                With XmlAppezzaxParticelle
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("piva", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Piva")))
                                    .SetAttribute("sa_cod", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Sa_Cod")))
                                    .SetAttribute("appezza", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Appezza")))
                                    .SetAttribute("part_cod", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Part_Cod")))
                                    .SetAttribute("prov", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Prov")))
                                    .SetAttribute("com", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Com")))
                                    .SetAttribute("sezione", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Sezione")))
                                    .SetAttribute("foglio", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Foglio")))
                                    .SetAttribute("numero", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Numero")))
                                    .SetAttribute("subalterno", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Subalterno")))
                                    .SetAttribute("area", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Area")))
                                    .SetAttribute("partita_catastale", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Partita_Catastale")))
                                    .SetAttribute("ettari", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Ettari")))
                                    .SetAttribute("are", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Are")))
                                    .SetAttribute("centiare", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Centiare")))
                                    .SetAttribute("qualita_cod", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Qualita_Cod")))
                                    .SetAttribute("classe", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Classe")))
                                    .SetAttribute("reddito_dominicale", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Reddito_Dominicale")))
                                    .SetAttribute("reddito_agrario", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Reddito_Agrario")))
                                    .SetAttribute("titolopossesso", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("TitoloPossesso")))
                                    .SetAttribute("sau_convenz_ettari", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convenz_Ettari")))
                                    .SetAttribute("sau_convenz_are", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convenz_Are")))
                                    .SetAttribute("sau_convenz_centiare", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convenz_Centiare")))
                                    .SetAttribute("sau_convers_ettari", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convers_Ettari")))
                                    .SetAttribute("sau_convers_are", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convers_Are")))
                                    .SetAttribute("sau_convers_centiare", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Convers_Centiare")))
                                    .SetAttribute("sau_bio_ettari", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Bio_Ettari")))
                                    .SetAttribute("sau_bio_are", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Bio_Are")))
                                    .SetAttribute("sau_bio_centiare", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("SAU_Bio_Centiare")))
                                    .SetAttribute("inviato", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Inviato")))
                                    .SetAttribute("datainvio", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("DataInvio")))
                                    .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Data_Creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Data_Modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Username_Creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Username_Modifica")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Validita_Inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzaxParticelle.Rows(j).Item("Validita_Fine")))


                                    '#################################
                                    '#### MACROUSI  ##################
                                    '#################################

                                    objAppezzaxParticellexMacrousi = New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R

                                    DtAppezzaxParticellexMacrousi = objAppezzaxParticellexMacrousi.Leggi(
                                                                    CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                                    CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                                    CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                                    DtAppezzaxParticelle.Rows(j).Item("Prov"),
                                                                    DtAppezzaxParticelle.Rows(j).Item("Com"),
                                                                    DtAppezzaxParticelle.Rows(j).Item("sezione"),
                                                                    DtAppezzaxParticelle.Rows(j).Item("foglio"),
                                                                    DtAppezzaxParticelle.Rows(j).Item("numero"),
                                                                    DtAppezzaxParticelle.Rows(j).Item("subalterno"),
                                                                    "",
                                                                    "",
                                                                    "",
                                                                    objParametri)


                                    'Se ottengo almeno un risultato, creo la struttura XML
                                    If DtAppezzaxParticellexMacrousi.Rows.Count > 0 Then

                                        '----- < Documento XML > -----
                                        XmlDatiAppezzaxParticellexMacrousi = XmlDoc.CreateElement("DatiMacrousi")

                                        'Effettuo un ciclo sulle AppezzAxParticelle
                                        For x = 0 To DtAppezzaxParticellexMacrousi.Rows.Count - 1

                                            '----- < APPEZZAXPARTICELLAXMACROUSI > -----
                                            XmlAppezzaxParticellexMacrousi = XmlDoc.CreateElement("Macrouso")

                                            With XmlAppezzaxParticellexMacrousi
                                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                                .SetAttribute("piva", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Piva")))
                                                .SetAttribute("sa_cod", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Sa_Cod")))
                                                .SetAttribute("appezza", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Appezza")))
                                                .SetAttribute("prov", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Prov")))
                                                .SetAttribute("com", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Com")))
                                                .SetAttribute("sezione", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Sezione")))
                                                .SetAttribute("foglio", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Foglio")))
                                                .SetAttribute("numero", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Numero")))
                                                .SetAttribute("subalterno", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Subalterno")))
                                                .SetAttribute("macrouso_cod", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("macrouso_cod")))
                                                .SetAttribute("macrouso_des", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("macrouso_des")))
                                                .SetAttribute("superficie", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("superficie")))
                                                .SetAttribute("inviato", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Inviato")))
                                                .SetAttribute("datainvio", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("DataInvio")))
                                                .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Data_Creazione")))
                                                .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Data_Modifica")))
                                                .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Username_Creazione")))
                                                .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Username_Modifica")))
                                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Validita_Inizio")))
                                                .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzaxParticellexMacrousi.Rows(x).Item("Validita_Fine")))
                                            End With

                                            '#################################
                                            '#### UTILIZZI  ##################
                                            '#################################

                                            objAppezzaxParticellexMacrousixUtilizzo = New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R

                                            DtAppezzaxParticellexMacrousixUtilizzo = objAppezzaxParticellexMacrousixUtilizzo.Leggi(
                                                                            CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                                            CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                                            CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                                            DtAppezzaxParticelle.Rows(j).Item("Prov"),
                                                                            DtAppezzaxParticelle.Rows(j).Item("Com"),
                                                                            DtAppezzaxParticelle.Rows(j).Item("sezione"),
                                                                            DtAppezzaxParticelle.Rows(j).Item("foglio"),
                                                                            DtAppezzaxParticelle.Rows(j).Item("numero"),
                                                                            DtAppezzaxParticelle.Rows(j).Item("subalterno"),
                                                                            DtAppezzaxParticellexMacrousi.Rows(x).Item("macrouso_cod"),
                                                                            "",
                                                                            "",
                                                                            "",
                                                                            "",
                                                                            objParametri)


                                            'Se ottengo almeno un risultato, creo la struttura XML
                                            If DtAppezzaxParticellexMacrousixUtilizzo.Rows.Count > 0 Then

                                                XmlDatiAppezzaxParticellexMacrousixUtilizzi = XmlDoc.CreateElement("DatiUtilizzi")

                                                For y = 0 To DtAppezzaxParticellexMacrousixUtilizzo.Rows.Count - 1

                                                    XmlAppezzaxParticellexMacrousixUtilizzo = XmlDoc.CreateElement("Utilizzo")

                                                    With XmlAppezzaxParticellexMacrousixUtilizzo
                                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                                        .SetAttribute("piva", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Piva")))
                                                        .SetAttribute("sa_cod", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Sa_Cod")))
                                                        .SetAttribute("appezza", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Appezza")))
                                                        .SetAttribute("prov", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Prov")))
                                                        .SetAttribute("com", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Com")))
                                                        .SetAttribute("sezione", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Sezione")))
                                                        .SetAttribute("foglio", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Foglio")))
                                                        .SetAttribute("numero", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Numero")))
                                                        .SetAttribute("subalterno", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Subalterno")))
                                                        .SetAttribute("macrouso_cod", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("macrouso_cod")))
                                                        .SetAttribute("veg_cod_agea", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("veg_cod_agea")))
                                                        .SetAttribute("cul_cod_agea", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("cul_cod_agea")))
                                                        .SetAttribute("superficie", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("superficie")))
                                                        .SetAttribute("inviato", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Inviato")))
                                                        .SetAttribute("datainvio", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("DataInvio")))
                                                        .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Data_Creazione")))
                                                        .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Data_Modifica")))
                                                        .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Username_Creazione")))
                                                        .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Username_Modifica")))
                                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Validita_Inizio")))
                                                        .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzaxParticellexMacrousixUtilizzo.Rows(y).Item("Validita_Fine")))
                                                    End With

                                                    XmlDatiAppezzaxParticellexMacrousixUtilizzi.AppendChild(XmlAppezzaxParticellexMacrousixUtilizzo)
                                                    XmlAppezzaxParticellexMacrousixUtilizzo = Nothing

                                                Next

                                                DtAppezzaxParticellexMacrousixUtilizzo.Dispose()
                                                DtAppezzaxParticellexMacrousixUtilizzo = Nothing
                                                objAppezzaxParticellexMacrousixUtilizzo = Nothing

                                                XmlAppezzaxParticellexMacrousi.AppendChild(XmlDatiAppezzaxParticellexMacrousixUtilizzi)

                                            End If

                                            XmlDatiAppezzaxParticellexMacrousi.AppendChild(XmlAppezzaxParticellexMacrousi)
                                            XmlAppezzaxParticellexMacrousi = Nothing

                                        Next

                                        DtAppezzaxParticellexMacrousi.Dispose()
                                        DtAppezzaxParticellexMacrousi = Nothing
                                        objAppezzaxParticellexMacrousi = Nothing

                                        XmlAppezzaxParticelle.AppendChild(XmlDatiAppezzaxParticellexMacrousi)

                                    End If

                                End With

                                '-------------------------------------------------------------------------------------------------------
                                'Agro_Personalizza.Agro_XmlSetting(XmlAppezzaxParticelle, DatiSetting)
                                '-------------------------------------------------------------------------------------------------------

                                XmlDatiAppezzaxParticelle.AppendChild(XmlAppezzaxParticelle)
                                XmlAppezzaxParticelle = Nothing

                            Next


                            DtAppezzaxParticelle.Dispose()
                            DtAppezzaxParticelle = Nothing
                            objAppezzaxParticelle = Nothing

                            XmlAppezzamento.AppendChild(XmlDatiAppezzaxParticelle)

                            '----- < / AppezzaxParticelle > -----

                        End If


                        '#################################
                        '##########  CODICI  #############
                        '#################################

                        'Mi procuro un elenco dei codici dell'appezzamento

                        'Creo l'oggetto COM+
                        objAppezzaxCodici = New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R

                        'Mi procuro il recordset richiesto
                        Dim tipoSelezione As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_TabellaCompleta
                        If LeggiCodiciAnagrafe_ConDescrizioni Then
                            tipoSelezione = enumSelezioneVariabile.Selezione_JoinCompleta
                        End If

                        DtAppezzaxCodici = objAppezzaxCodici.Leggi(
                                                CStr(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("PIVA"))),
                                                CInt(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Sa_Cod"))),
                                                CInt(Agro_SQL_Load(DrUtentixAppezzamenti(i).Item("Appezza"))),
                                                0,
                                                "",
                                                tipoSelezione,
                                                "",
                                                "",
                                                objParametri)

                        Dim val_cod_2 As String = ""

                        'Se ottengo almeno un risultato, creo la struttura XML
                        If DtAppezzaxCodici.Rows.Count > 0 Then

                            XmlDatiCodici = XmlDoc.CreateElement("DatiCodici")

                            'Effettuo un ciclo sui codici
                            For j = 0 To DtAppezzaxCodici.Rows.Count - 1

                                '----- < CODICE > -----
                                XmlCodice = XmlDoc.CreateElement("CodiceAppezzamento")

                                If LeggiCodiciAnagrafe_ConDescrizioni Then
                                    val_cod_2 = Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("val_cod_2"))
                                    XmlCodice.SetAttribute("val_cod_2", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("val_cod_2")))
                                End If

                                With XmlCodice
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("id_cod", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("id_cod")))
                                    .SetAttribute("val_cod", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("val_cod")))
                                    .SetAttribute("descrizione", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("descrizione")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("validita_inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("validita_fine")))
                                    .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("Appezzamento_Codici_data_creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("Appezzamento_Codici_data_modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("Appezzamento_Codici_username_creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzaxCodici.Rows(j).Item("Appezzamento_Codici_username_modifica")))
                                End With

                                '-------------------------------------------------------------------------------------------------------
                                'Agro_Personalizza.Agro_XmlSetting(XmlCodice, DatiSetting)
                                '-------------------------------------------------------------------------------------------------------


                                XmlDatiCodici.AppendChild(XmlCodice)
                                XmlCodice = Nothing

                                '----- < / CODICE > -----
                            Next

                            DtAppezzaxCodici.Dispose()
                            DtAppezzaxCodici = Nothing

                            objAppezzaxCodici = Nothing

                            XmlAppezzamento.AppendChild(XmlDatiCodici)

                        End If

                        XmlDatiCodici = Nothing

                        '#################################
                        '#######    INDIRIZZI   ##########
                        '#################################

                        objAppezzaxIndirizzi = New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read
                        objIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Read

                        DtAppezzaxIndirizzi = objAppezzaxIndirizzi.Leggi(CStr(DrUtentixAppezzamenti(i).Item("Piva")),
                                                        CInt(DrUtentixAppezzamenti(i).Item("Sa_Cod")),
                                                        CInt(DrUtentixAppezzamenti(i).Item("Appezza")),
                                                        0,
                                                        "",
                                                        "",
                                                        objParametri)

                        If DtAppezzaxIndirizzi.Rows.Count > 0 Then

                            XmlDatiAppezzaxIndirizzi = XmlDoc.CreateElement("Indirizzi")

                            'Effettuo un ciclo sulle AppezzAxParticelle
                            For j = 0 To DtAppezzaxIndirizzi.Rows.Count - 1

                                '----- < APPEZZAXPARTICELLA > -----
                                XmlAppezzaxIndirizzi = XmlDoc.CreateElement("Indirizzo")

                                With XmlAppezzaxIndirizzi
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("piva", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Piva")))
                                    .SetAttribute("sa_cod", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Sa_Cod")))
                                    .SetAttribute("appezza", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Appezza")))
                                    .SetAttribute("cod_indirizzo", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("cod_indirizzo")))
                                    .SetAttribute("tipo_indirizzo", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("tipo_indirizzo")))
                                    .SetAttribute("inviato", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Inviato")))
                                    .SetAttribute("datainvio", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("DataInvio")))
                                    .SetAttribute("data_creazione", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Data_Creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Data_Modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Username_Creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Username_Modifica")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Validita_Inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(DtAppezzaxIndirizzi.Rows(j).Item("Validita_Fine")))

                                    Dim dtIndirizzi = objIndirizzi.Leggi(DtAppezzaxIndirizzi.Rows(j).Item("cod_indirizzo"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                                    If dtIndirizzi.Rows.Count > 0 Then

                                        .SetAttribute("ind_des", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("ind_des")))
                                        .SetAttribute("frz_des", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("frz_des")))
                                        .SetAttribute("CAP", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("CAP")))
                                        .SetAttribute("stato", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("stato")))
                                        .SetAttribute("note", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("note")))
                                        .SetAttribute("pro_cod_istat", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("pro_cod_istat")))
                                        .SetAttribute("com_cod_istat", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("com_cod_istat")))
                                        .SetAttribute("com_des", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("com_des")))
                                        .SetAttribute("pro_cod", Agro_SQL_Load(dtIndirizzi.Rows(0).Item("pro_cod")))

                                    End If

                                End With

                                XmlDatiAppezzaxIndirizzi.AppendChild(XmlAppezzaxIndirizzi)
                                XmlAppezzaxIndirizzi = Nothing

                            Next

                            DtAppezzaxIndirizzi.Dispose()
                            DtAppezzaxIndirizzi = Nothing

                            objAppezzaxIndirizzi = Nothing
                            objIndirizzi = Nothing

                            XmlAppezzamento.AppendChild(XmlDatiAppezzaxIndirizzi)

                        End If




                        XmlDatiAppezzamenti.AppendChild(XmlAppezzamento)

                        XmlAppezzamento = Nothing
                    Next

                End If

                DtUtentixAppezzamenti.Dispose()

                '#################################
                '#################################
                '#################################


                '----- < / Appezzamenti > -----

                XmlDoc.AppendChild(XmlDatiAppezzamenti)

                RisultatoFunzione = XmlDoc.InnerXml
                '----- < / Documento XML > -----


                XmlDatiAppezzamenti = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun appezzamento ...
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtUtentixAppezzamenti = Nothing
            objUtentixAppezzamenti = Nothing

        Catch ex As Exception

            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Pulizia
            'objUtentixImprese = Nothing
            'objImpresexCodici = Nothing
            'objImpresexIndirizzi = Nothing

            '------------------------------
            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return RisultatoFunzione

    End Function

    Public Function Leggi_AppezzamentoGlobal_Anagrafica(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Appezza As Integer,
                                                        ByVal Leggi_Impresa As Boolean,
                                                        ByVal Leggi_Centro As Boolean,
                                                        ByVal Leggi_Campo As Boolean,
                                                        ByVal Leggi_Impianti As Boolean,
                                                        ByVal Leggi_Distinte As Boolean,
                                                        ByRef objParametri_Server As AgronicaCoreParametri
                                                        ) As AnagrafeNG.AppezzamentoGlobal

        Dim appezzamento_G As New AnagrafeNG.AppezzamentoGlobal

        If Piva = "" OrElse Sa_Cod = 0 OrElse Appezza = 0 Then
            If Piva = "" Then
                Throw New Exception("Piva Obbligatorio")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Sa_Cod Obbligatorio")
            End If

            If Appezza = 0 Then
                Throw New Exception("Appezza Obbligatorio")
            End If

        End If

        If Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 Then

            If Leggi_Impresa Then
                'Dim Impresa_R As New Impresa_R
                'appezzamento_G.Impresa = Impresa_R.Impresa_Leggi_Anagrafica(Piva,
                '                                                            Leggi_Indirizzo:=True,
                '                                                            Leggi_Padri:=False,
                '                                                            Leggi_Contatti:=False,
                '                                                            Leggi_Contatto_Superuser:=False,
                '                                                            Leggi_Codici:=False,
                '                                                            objParametri_Server)
            End If

            If Leggi_Centro Then

            End If

            If Leggi_Campo Then

            End If

            'appezzamento_G.Appezzamento = Leggi_Appezzamento_Anagrafica(Piva, Sa_Cod, Appezza, objParametri_Server)

            'If Leggi_Impianti Then
            '    Dim impianti_R As New Reg_Impianto_R
            '    appezzamento_G.Impianti = impianti_R.Leggi_Impianti_Anagrafica(Piva,
            '                                                                   Sa_Cod,
            '                                                                   Appezza,
            '                                                                   Leggi_Distinte,
            '                                                                   objParametri_Server)
            'End If

        End If

        Return appezzamento_G
    End Function

    Public Function Leggi_Appezzamento_UtilizzoTerreno(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno


        Dim utilizzoTerreno = Nothing
        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DtRegImpianti As DataTable = objRegImpianti.Leggi(Piva, Sa_Cod, Appezza, Id_Reg, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

        If DtRegImpianti IsNot Nothing AndAlso DtRegImpianti.Rows.Count > 0 Then

            If DtRegImpianti.Rows(0).Item("Cul_cod") <> 0 Then
                utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(DtRegImpianti.Rows(0).Item("Cul_cod")) With {
                    .descrizione = DtRegImpianti.Rows(0).Item("Cul_Des"),
                    .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(DtRegImpianti.Rows(0).Item("Veg_Cod")) With {
                        .descrizione = DtRegImpianti.Rows(0).Item("Veg_Des")
                    }
                }
            Else
                Dim FiltroLetturaCodici As String = ""

                FiltroLetturaCodici += " ( " &
                " Reg_Impianti_Codici.Id_Cod NOT IN ( " &
                CStr(enum_CodiciAnagrafe.Codice_Specie_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_1) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_2) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_3) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_4) & " " &
                "   ) " &
                " )"

                FiltroLetturaCodici += " AND Reg_Impianti_Codici.progetto_cod = 0  " &
                                " AND (Reg_Impianti_Codici.id_cod < 2000 OR Reg_Impianti_Codici.id_cod >= 3000 ) "

                Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                Dim DtCodici = objCodici.Leggi(Piva,
                                    Sa_Cod,
                                    Appezza,
                                    Id_Reg,
                                    "",
                                    0,
                                    "",
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    FiltroLetturaCodici,
                                    "",
                                    objParametri_Server)

                If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

                    For i = 0 To DtCodici.Rows.Count - 1

                        If (Not IsDBNull(DtCodici.Rows(i).Item("val_cod"))) Then

                            Dim Id_Cod = DtCodici.Rows(i).Item("id_cod")
                            Dim CodiceAnagrafeDes = DtCodici.Rows(i).Item("descrizione")

                            Select Case Id_Cod

                                Case 3000 To 3999

                                    utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(Id_Cod) With {
                                        .descrizione = CodiceAnagrafeDes
                                    }

                            End Select
                        End If
                    Next

                Else

                    'Caso Terreno nudo
                    utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(0) With {
                        .descrizione = Gias.TerrenoNudo
                    }

                End If
            End If
        End If

        Return utilizzoTerreno
    End Function

    Public Function Leggi_Appezzamento_GUID(ByVal guid As String,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim appezzamento As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento()
        Dim DTAppezzamenti As DataTable

        DTAppezzamenti = objAppezzamenti.Leggi_x_GUID(guid,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "",
                                                      "",
                                                      objParametri_Server)
        If DTAppezzamenti.Rows.Count > 0 Then

            If Not IsDBNull(DTAppezzamenti.Rows(0).Item("PIVA")) AndAlso Not IsDBNull(DTAppezzamenti.Rows(0).Item("SA_Cod")) AndAlso Not IsDBNull(DTAppezzamenti.Rows(0).Item("APPEZZA")) Then
                Dim piva = DTAppezzamenti.Rows(0).Item("PIVA")
                Dim sa_cod = CInt(DTAppezzamenti.Rows(0).Item("SA_Cod"))
                Dim appezza = CInt(DTAppezzamenti.Rows(0).Item("APPEZZA"))

                appezzamento = Leggi_Appezzamento_Anagrafica(
                        piva,
                        sa_cod,
                        appezza,
                        0,
                        True,
                        True,
                        True,
                        AGRODATAINIZIO,
                        False,
                        True,
                        True,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti)

            End If

        Else
            appezzamento = Nothing
        End If

        Return appezzamento
    End Function

    Public Function Leggi_Appezzamento_Anagrafica(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal IdReg As Integer,
                                                  ByVal Leggi_Impianti As Boolean,
                                                  ByVal Leggi_Indirizzi As Boolean,
                                                  ByVal Leggi_Catasto As Boolean,
                                                  ByVal data As Date,
                                                  ByVal filtroData As Boolean,
                                                  ByVal Leggi_Distinte As Boolean,
                                                  ByVal Leggi_Cartografia As Boolean,
                                                  ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim appezzamento As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento(
            New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(Appezza,
                                                                  New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)))


        Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
        Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

        If filtroData Then
            objParametri_Server.FinestraTemporaleInizio = data
            objParametri_Server.FinestraTemporaleFine = data
        Else
            objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
            objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
        End If

        If Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 Then

            Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim DTAppezzamenti As DataTable


            Dim tipoSelezione As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_TabellaCompleta
            Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

            If jSonStaticMapCFG <> "" Then
                Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                If StaticMapCFG.StaticMapAttive Then
                    tipoSelezione = enumSelezioneVariabile.Selezione_JoinCompleta
                End If
            End If

            objParametri_Server.FinestraTemporaleInizio = appInizio
            objParametri_Server.FinestraTemporaleFine = appFine

            DTAppezzamenti = objAppezzamenti.Leggi(
                CStr(Piva),
                CInt(Sa_Cod),
                CInt(Appezza),
                tipoSelezione,
                "",
                "",
                objParametri_Server,
                Leggi_Cartografia:=Leggi_Cartografia
                )

            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE


            If DTAppezzamenti.Rows.Count > 0 Then

                appezzamento.campoPK = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(DTAppezzamenti.Rows(0).Item("Campo_Cod"),
                                                                                      New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva))

                appezzamento.descrizione = If(IsDBNull(DTAppezzamenti.Rows(0).Item("App_Nome")), "", DTAppezzamenti.Rows(0).Item("App_Nome"))
                appezzamento.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DTAppezzamenti.Rows(0).Item("Validita_Inizio"),
                                                                                                  DTAppezzamenti.Rows(0).Item("Validita_Fine"))
                appezzamento.superficie = DTAppezzamenti.Rows(0).Item("sup_app")


                appezzamento.pendenza = DTAppezzamenti.Rows(0).Item("pende")
                appezzamento.esposizione = New BaseCodeDescrStr(DTAppezzamenti.Rows(0).Item("esposiz"), DTAppezzamenti.Rows(0).Item("esposiz"))
                appezzamento.ubicazione = New BaseCodeDescrStr(DTAppezzamenti.Rows(0).Item("ubicazione"), DTAppezzamenti.Rows(0).Item("ubicazione"))

                appezzamento.supBZ_Riduzione = DTAppezzamenti.Rows(0).Item("SupBZ_Riduzione")
                appezzamento.distBZ_CorpiIdrici = DTAppezzamenti.Rows(0).Item("DistBZ_CorpiIdrici")
                appezzamento.distBZ_AreeResPub = DTAppezzamenti.Rows(0).Item("DistBZ_AreeResPub")
                appezzamento.distBZ_Allevamenti = DTAppezzamenti.Rows(0).Item("DistBZ_Allevamenti")
                appezzamento.distBZ_VegNatNonColt = DTAppezzamenti.Rows(0).Item("DistBZ_VegNatNonColt")

                appezzamento.lat = DTAppezzamenti.Rows(0).Item("x")
                appezzamento.lng = DTAppezzamenti.Rows(0).Item("y")
                appezzamento.altitudine = DTAppezzamenti.Rows(0).Item("zslm")

                If Leggi_Catasto Then
                    appezzamento.catastoAppezzamento = Leggi_AppezzamentoCatasto(Piva, Sa_Cod, Appezza, objParametri_Server)
                End If

                If tipoSelezione = enumSelezioneVariabile.Selezione_JoinCompleta Then
                    appezzamento.immagineBase64 = DTAppezzamenti.Rows(0).Item("StaticMapBase64String")
                End If

                appezzamento.n_App_Bio = ""
                appezzamento.confini_A_Rischio = ""
                appezzamento.utilizzo_Terreno = New List(Of BaseCodeDescr)
                appezzamento.fine_Impiego_Prod_Non_Conformi = AGRODATAINIZIO

                'lavez - 21/03/2024 - chiavi nuovo tracciato agea
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_idSchedaValidazione")) Then
                    appezzamento.Agea_idSchedaValidazione = DTAppezzamenti.Rows(0).Item("Agea_idSchedaValidazione")
                End If
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_identificativoPianoColtivazione")) Then
                    appezzamento.Agea_identificativoPianoColtivazione = DTAppezzamenti.Rows(0).Item("Agea_identificativoPianoColtivazione")
                End If
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_identificativoIsola")) Then
                    appezzamento.Agea_identificativoIsola = DTAppezzamenti.Rows(0).Item("Agea_identificativoIsola")
                End If
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_identificativoAppezzamento")) Then
                    appezzamento.Agea_identificativoAppezzamento = DTAppezzamenti.Rows(0).Item("Agea_identificativoAppezzamento")
                End If
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_codiBarrScheVali")) Then
                    appezzamento.Agea_codiBarrScheVali = DTAppezzamenti.Rows(0).Item("Agea_codiBarrScheVali")
                End If
                If Not IsDBNull(DTAppezzamenti.Rows(0).Item("Agea_idAppezzamentoOrig")) Then
                    appezzamento.Agea_idAppezzamentoOrig = DTAppezzamenti.Rows(0).Item("Agea_idAppezzamentoOrig")
                End If

                Dim objMetaschema As New AgronicaCoreMetaSchemaBIZ.ClassiTessitura

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Sabbia")) Then
                    appezzamento.sabbia = Nothing
                Else
                    appezzamento.sabbia = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Sabbia"))
                End If

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Limo")) Then
                    appezzamento.limo = Nothing
                Else
                    appezzamento.limo = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Limo"))
                End If

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Argilla")) Then
                    appezzamento.argilla = Nothing
                Else
                    appezzamento.argilla = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Argilla"))
                End If

                Dim id_classetessitura As Integer = If(IsDBNull(DTAppezzamenti.Rows(0).Item("CLAS")) OrElse DTAppezzamenti.Rows(0).Item("CLAS") = "", 0, CInt(DTAppezzamenti.Rows(0).Item("CLAS")))
                appezzamento.classeTessitura = New ClasseTessitura(id_classetessitura) With {
                    .descrizione = Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(objMetaschema.ClasseTessituraDes_from_ClasseTessituraCod(id_classetessitura, 0, objParametri_Server).Trim())
                }

                Dim DTCodici As DataTable

                Dim objCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R

                Dim Id_Cod As Integer = 0
                Dim Val_Cod As String = ""

                DTCodici = objCodici.Leggi(
                    Piva,
                    Sa_Cod,
                    Appezza,
                    Id_Cod,
                    Val_Cod,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "(id_cod < 2000 OR id_cod >= 3000)",
                    "",
                    objParametri_Server
                    )

                Dim objSpecie_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

                appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato) With {.descrizione = Gias.Integrato}

                Dim altriCodici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

                If DTCodici.Rows.Count > 0 Then

                    For i = 0 To DTCodici.Rows.Count - 1

                        If (Not IsDBNull(DTCodici.Rows(i).Item("val_cod"))) Then

                            Id_Cod = DTCodici.Rows(i).Item("id_cod")
                            Val_Cod = DTCodici.Rows(i).Item("val_cod")

                            Select Case DTCodici.Rows(i).Item("id_cod")

                                Case enum_CodiciAnagrafe.MetodoDiProduzione
                                    Dim objMetodoProduzione As New AgronicaCoreMetaSchemaBIZ.MetodoProduzione
                                    Select Case DTCodici.Rows(i).Item("val_cod")
                                        Case enum_MetodoProduzione.Integrato, enum_MetodoProduzione.InConversione, enum_MetodoProduzione.Biologico
                                            appezzamento.metodo_Produzione = objMetodoProduzione.Leggi(DTCodici.Rows(i).Item("val_cod"), objParametri_Server)
                                        Case Else
                                            appezzamento.metodo_Produzione = objMetodoProduzione.Leggi(enum_MetodoProduzione.Integrato, objParametri_Server)
                                    End Select

                                Case enum_CodiciAnagrafe.DataFineImpiegoPNC

                                    If IsDate(DTCodici.Rows(i).Item("val_cod")) Then
                                        appezzamento.fine_Impiego_Prod_Non_Conformi = CDate(DTCodici.Rows(i).Item("val_cod"))
                                    End If
                                Case enum_CodiciAnagrafe.Appezzamento_ConfiniRischio
                                    appezzamento.confini_A_Rischio = DTCodici.Rows(i).Item("val_cod")

                                Case enum_CodiciAnagrafe.OrientamentoProduttivo
                                    If Not IsDBNull(DTCodici.Rows(i).Item("val_cod")) Then
                                        appezzamento.utilizzo_Terreno = Leggi_Orientamento_Produttivo(DTCodici.Rows(i).Item("val_cod"), objParametri_Server)
                                    End If
                                Case enum_CodiciAnagrafe.Terreno_Inutilizzato
                                    appezzamento.terrenoInutilizzato = CInt(DTCodici.Rows(i).Item("val_cod"))
                                Case enum_CodiciAnagrafe.Terreno_Degradato
                                    appezzamento.terrenoDegradato = CInt(DTCodici.Rows(i).Item("val_cod"))
                                Case enum_CodiciAnagrafe.Low_ILUC
                                    appezzamento.lowILUC = CInt(DTCodici.Rows(i).Item("val_cod"))
                                Case enum_CodiciAnagrafe.Codice_Appezza_Biologico
                                    appezzamento.n_App_Bio = DTCodici.Rows(i).Item("val_cod")
                                Case enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento
                                    appezzamento.rif_Appezzamento = DTCodici.Rows(i).Item("val_cod")
                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                          0, "", "",
                                                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                          "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_1_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                                dtSpecie.Rows(0)("Veg_Cod")) With {
                                                    .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                                }
                                    End If
                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                          0, "", "",
                                                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                          "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_2_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                                dtSpecie.Rows(0)("Veg_Cod")) With {
                                                    .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                                }
                                    End If
                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                          0, "", "",
                                                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                          "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_3_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                                dtSpecie.Rows(0)("Veg_Cod")) With {
                                                    .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                                }
                                    End If
                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                          0, "", "",
                                                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                          "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_4_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                                dtSpecie.Rows(0)("Veg_Cod")) With {
                                                    .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                                }
                                    End If
                                Case enum_CodiciAnagrafe.Isola
                                    appezzamento.isola = DTCodici.Rows(i).Item("val_cod")
                                Case enum_CodiciAnagrafe.TitoloPossesso
                                    'DO NOTHING
                                Case Else
                                    Dim codice = New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
                                    codice.valore = DTCodici.Rows(i).Item("val_cod")
                                    codice.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DTCodici.Rows(i).Item("Validita_Inizio"), DTCodici.Rows(i).Item("Validita_Fine"))
                                    Dim cod = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe
                                    cod.codice = DTCodici.Rows(i).Item("id_Cod")
                                    cod.descrizione = DTCodici.Rows(i).Item("descrizione")
                                    codice.codiceAnagrafe = cod
                                    altriCodici.Add(codice)
                            End Select

                        End If
                    Next
                End If

                appezzamento.codici = altriCodici

                If Leggi_Cartografia AndAlso DTAppezzamenti.Columns.Contains("cartografia") Then
                    appezzamento.cartografia = DTAppezzamenti.Rows(0)("cartografia")
                End If

                If Leggi_Cartografia AndAlso DTAppezzamenti.Columns.Contains("SuperficieGis") Then
                    appezzamento.superficieGis = DTAppezzamenti.Rows(0)("SuperficieGis")
                End If

                If Leggi_Impianti Then
                    Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                    appezzamento.impianti = objImpianti_R.Leggi_Impianti_Anagrafica(
                        Piva,
                        Sa_Cod,
                        Appezza,
                        IdReg,
                        Leggi_Distinte,
                        data,
                        filtroData,
                        Leggi_Cartografia,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti
                        )
                End If

                If Leggi_Indirizzi Then
                    Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                    appezzamento.indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Appezzamento(Piva, Sa_Cod, Appezza, objParametri_Server)
                End If

            End If
        Else
            appezzamento = Nothing
        End If

        objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
        objParametri_Server.FinestraTemporaleFine = CDate(appFine)

        Return appezzamento
    End Function

    Public Function ReadAgriculturalPlotLight(piva As String,
                                              saCod As Int32,
                                              appezza As Int32,
                                              objParametriServer As AgronicaCoreParametri,
                                              objParametriUtenti As AgronicaCoreParametri,
                                              objParametriSuperServer As AgronicaCoreParametri
                                              ) As AgronicaCoreModelsSTD.anagrafiche.AppezzamentoJoinDescrizioni

        If String.IsNullOrEmpty(piva) Then
            Throw New Exception("Parametro [piva] non valido")
        End If

        If saCod = 0 Then
            Throw New Exception("Parametro [saCod] non valido")
        End If

        If appezza = 0 Then
            Throw New Exception("Parametro [appezza] non valido")
        End If

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim leggiMacchineAssociate As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametriUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametriUtenti
            )

        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim dt = objAppezzamenti.Leggi(
            CStr(piva),
            CInt(saCod),
            CInt(appezza),
            enumSelezioneVariabile.Selezione_JoinDescrizioni,
            "",
            "",
            objParametriServer
            )

        Dim appezzamento As New AppezzamentoJoinDescrizioni(
            New PK(appezza, New CentroAziendale.PK(saCod, piva))
            )

        If dt.Rows.Count = 1 Then
            appezzamento.ragSoc = dt.Rows(0).Item("Rag_Soc")
            appezzamento.saNome = dt.Rows(0).Item("Sa_Nome")

            appezzamento.campoPK = New Campo.PK(
                dt.Rows(0).Item("Campo_Cod"),
                New CentroAziendale.PK(saCod, piva)
                )

            appezzamento.descrizione = If(IsDBNull(dt.Rows(0).Item("App_Nome")), "", dt.Rows(0).Item("App_Nome"))
            appezzamento.validita = New IntervalloTemporale(
                dt.Rows(0).Item("Validita_Inizio"),
                dt.Rows(0).Item("Validita_Fine")
                )

            appezzamento.superficie = dt.Rows(0).Item("sup_app")

            If leggiMacchineAssociate Then
                Dim objAxP As New AppezzamentiXParcoMacchine_R
                appezzamento.linkedMachines = objAxP.ReadJoinDescriptions(objParametriServer, objParametriUtenti, piva, saCod, appezza)
            End If

            Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            appezzamento.impianti = objImpianti_R.Leggi_Impianti_Anagrafica(
                piva,
                saCod,
                appezza,
                0,
                False,
                New Date(),
                False,
                False,
                objParametriSuperServer,
                objParametriServer,
                objParametriUtenti
                )
        ElseIf dt.Rows.Count > 1 Then
            Throw New Exception("Violato vincolo chiave in lettura AppezzamentoJoinDescrizioni")
        Else
            Throw New Exception("Errore durante la lettura dell'appezzamento: appezzamento non trovato")
        End If

        Return appezzamento

    End Function

    Public Function Leggi_Orientamento_Produttivo(valoriStr As String, objParametriServer As AgronicaCoreParametri) As List(Of BaseCodeDescr)
        Dim list As New List(Of BaseCodeDescr)
        Dim objOrientamentroProduttivo As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R
        If valoriStr <> "" Then
            Dim ValoriArr = valoriStr.Split(",")
            For Each valore In ValoriArr
                valore = valore.Trim
                If IsNumeric(valore) Then
                    Dim dt = objOrientamentroProduttivo.Leggi(CInt(valore), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)
                    If dt.Rows.Count > 0 Then
                        list.Add(New BaseCodeDescr() With {
                            .codice = CInt(valore),
                            .descrizione = dt.Rows(0)("Orientamento_Des")
                        })
                    End If
                End If
            Next
        End If

        Return list
    End Function

    Public Function Leggi_Appezzamenti_Anagrafica(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal Campo_Cod As Integer,
                                                  ByVal Data_Filtro As Date,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim dt As DataTable

        Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
        Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

        If Data_Filtro <> AGRODATAINIZIO Then
            objParametri_Server.FinestraTemporaleInizio = Data_Filtro
            objParametri_Server.FinestraTemporaleFine = Data_Filtro
        End If

        dt = objAppezzamento.Leggi_x_anagrafica(Piva, Sa_Cod, Campo_Cod, 0, "", "", objParametri_Server)


        Dim DTImpianti = objImpianti.Leggi_x_anagrafica2(Piva,
                                                         Sa_Cod,
                                                         Appezza,
                                                         0,
                                                         Campo_Cod,
                                                        "",
                                                        "",
                                                        objParametri_Server,
                                                        Date.Now)

        Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inDataxAnagrafica(Piva,
                                                                                   Sa_Cod,
                                                                                   Appezza,
                                                                                   0,
                                                                                   Data_Filtro,
                                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                  "",
                                                                                  " Imprese_Progetti.Validita_Inizio DESC ",
                                                                                  objParametri_Server)

        Dim dataRiferimento = AGRODATAINIZIO

        If Data_Filtro <> AGRODATAINIZIO Then
            dataRiferimento = Data_Filtro
        End If

        dt.Columns.Add(New DataColumn("utilizzo"))
        dt.Columns.Add(New DataColumn("cod_kpin"))
        dt.Columns.Add(New DataColumn("cod_block"))
        dt.Columns.Add(New DataColumn("Blk_Flag_Des"))

        For Each row In dt.Rows

            Dim rowsImpianti = DTImpianti.Select(" sa_Cod = " & row("sa_cod") & " AND Appezza = " & row("Appezza") & " ", "Validita_Inizio")

            If rowsImpianti IsNot Nothing AndAlso rowsImpianti.Length > 0 Then
                Dim listUtilizzo As New List(Of String)
                For Each rowImpianti In rowsImpianti
                    If rowImpianti("cul_cod") <> 0 Then
                        listUtilizzo.Add($"{rowImpianti("veg_des")} - { rowImpianti("cul_des")}")
                    Else
                        listUtilizzo.Add(rowImpianti("utilizzo"))
                    End If

                Next
                row("utilizzo") = String.Join(", ", listUtilizzo.Distinct())
            End If

            'row("utilizzo") = Utilizzi_Da_Appezzamento(row("piva"), row("sa_cod"), row("appezza"), objParametri_Server)

            Dim rowDist = dtDistinta.Select(" sa_Cod = " & row("sa_cod") &
                                                " AND Appezza = " & row("Appezza") & " ")


            If rowDist IsNot Nothing AndAlso rowDist.Length > 0 Then
                row("cod_kpin") = rowDist(0)("cod_kpin")
                row("cod_block") = rowDist(0)("cod_block")
            End If

            row("Blk_Flag_Des") = ""

            If CInt(row("Blk_Flag")) = 0 Then
                row("Blk_Flag_Des") = "No"
            ElseIf CInt(row("Blk_Flag")) = -1 Then
                row("Blk_Flag_Des") = "Si"
            End If

        Next

        Return dt

    End Function

    Private Function CalcolaPiante(ByVal dist_su As Double, ByVal dist_tra As Double, ByVal interb As Double, ByVal germin As Double, ByVal superficie As Double, ByRef data As Object) As Boolean
        If dist_su > 0 AndAlso dist_tra > 0 AndAlso germin > 0 Then
            ' Dim denominatore = If(interb > 0, (interb / 2) * dist_su, dist_su * dist_tra)
            Dim denominatore = If(interb > 0, Math.Abs(dist_tra - interb) * dist_su, dist_su * dist_tra)
            Dim PianteHa = 10000 / denominatore * (germin / 100)
            Dim PianteImpianto = PianteHa * superficie
            data("piante_ha") = Int(PianteHa)
            data("piante_impianto") = Int(PianteImpianto)
            Return True
        End If
        Return False
    End Function

    Public Function LeggiDatiCatastali(
                                      ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal Campo_Cod As Integer,
                                      ByVal flag_Macrousi As Boolean,
                                      ByVal flag_Utilizzi As Boolean,
                                      ByVal flag_Varieta As Boolean,
                                      ByVal ValiditaInizio As Date,
                                      ByVal ValiditaFine As Date,
                                      ByVal Tipo_Operazione As Integer,
                                      ByRef SuperficieIntersezioneTotale As Double,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      Optional ByVal isBudget As Boolean = False,
                                      Optional ByVal idBudget As Integer = 0
                                      ) As DataTable

        'modifico la data dell oggetto objparametri
        Dim appDInizio As New Date
        appDInizio = objParametri_Server.FinestraTemporaleInizio
        Dim appDFine As New Date
        appDFine = objParametri_Server.FinestraTemporaleFine

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(ValiditaInizio, ValiditaFine)

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Progressivo As Integer

        Dim SuperficieTotaleUsata As Double

        Dim Sup_Condotta_Min As Double
        Dim SuperficieIntersezione As Double
        Dim SuperficieDisponibile As Double

        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String

        Dim ZVN As String

        Dim Macrouso_Cod As String
        Dim Macrouso_Des As String
        Dim Sup_Macrouso As Double

        Dim Veg_Cod_Agea As String
        Dim Veg_Des_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Cul_Des_Agea As String
        Dim Sup_Utilizzo As Double

        Dim i, j As Integer

        Dim Squadro As Boolean = False

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
        Dt.Columns.Add(New DataColumn("ChkSelezionaParticella", GetType(Boolean)))
        Dt.Columns.Add(New DataColumn("CodiceIstat_Provincia", GetType(String)))
        Dt.Columns.Add(New DataColumn("CodiceIstat_Comune", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Progressivo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Provincia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Comune", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieLorda", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(Double)))
        Dt.Columns.Add(New DataColumn("SuperficieCondottaDisponibile", GetType(Double)))
        Dt.Columns.Add(New DataColumn("SuperficieImpiegata", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(Double)))
        Dt.Columns.Add(New DataColumn("SuperficieMacrousoDisponibile", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Utilizzo", GetType(Double)))
        Dt.Columns.Add(New DataColumn("SuperficieUtilizzoDisponibile", GetType(Double)))


        Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))
        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(10) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Provincia")
        DtKeys(1) = Dt.Columns("Comune")
        DtKeys(2) = Dt.Columns("Sezione")
        DtKeys(3) = Dt.Columns("Foglio")
        DtKeys(4) = Dt.Columns("Numero")
        DtKeys(5) = Dt.Columns("Subalterno")
        DtKeys(6) = Dt.Columns("Macrouso_Cod")
        DtKeys(7) = Dt.Columns("Sup_Macrouso")
        DtKeys(8) = Dt.Columns("Veg_Cod_Agea")
        DtKeys(9) = Dt.Columns("Cul_Cod_Agea")
        DtKeys(10) = Dt.Columns("Sup_Utilizzo")


        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Verifico quali particelle devo caricare
        Dim Dt_Particelle As New DataTable
        Dim DrParticella As DataRow()


        ' MACROUSI
        'Leggo i record della tabella AppezzamentoxParticellexMacrousi:
        '   - se contiene dei record metto il check sui macrousi e carico la combo;
        '   - se non contiene alcun record tolgo il check dai macrousi.

        Dim objAppxPartxMacr As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R

        ' UTILIZZI
        'Leggo i record della tabella AppezzamentoxParticellexMacrousixUtilizzo:
        '   - se contiene dei record metto il check utilizzi e carico le combo;
        '   - se non contiene alcun record tolgo il check utilizzi.

        Dim objAppxPartxMacrxUtil As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R

        'NOTA
        'Se l'appezzamento appartiene ad un campo allora verifico se il campo ha delle 
        'relazioni CAMPIxPARTICELLE valorizzate.
        'In caso affermativo ho una gestione del terreno a SQUADRI e presento solo le 
        'particelle che sono state relazionate con il campo.

        'In caso negativo il campo e' gestito solo come AGGREGATO di APPEZZAMENTI allora
        'visualizzo tutte le particelle del Centro Aziendale.

        'Se l'appezzamento non appartiene ad un campo, allora opero come nel caso dello
        'aggregato e visualizzo tutte le particelle del Centro Aziendale.

        'NOTA
        'Se APPEZZA <> 0 allora ricarico anche i valori attuali

        If Campo_Cod = 0 Then

            Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

            If flag_Macrousi Then
                If Not flag_Utilizzi Then
                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                Else
                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                End If
            Else
                If Not flag_Utilizzi Then
                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                Else
                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                End If
            End If

        Else

            Dim objCampixParticelleR As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            If objCampixParticelleR.Campo_Definito_Come_Squadro(Piva, Sa_Cod, Campo_Cod, objParametri_Server, isBudget:=isBudget, idBudget:=idBudget) Then

                'Imposto la variabile Squadro 
                Squadro = True

                Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.CampixParticelle_R


                If flag_Macrousi Then
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(Piva, Sa_Cod, Campo_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget:=isBudget, idBudget:=idBudget)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(Piva, Sa_Cod, Campo_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget:=isBudget, idBudget:=idBudget)
                    End If
                Else
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(Piva, Sa_Cod, Campo_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget:=isBudget, idBudget:=idBudget)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(Piva, Sa_Cod, Campo_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget:=isBudget, idBudget:=idBudget)
                    End If
                End If
            Else

                Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                If flag_Macrousi Then
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                    End If
                Else
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, ValiditaInizio, ValiditaFine, objParametri_Server, isBudget, idBudget)
                    End If
                End If

            End If

        End If

        '--------------------------------------------------------------------
        '--- HO TUTTE LE PARTICELLE --> CALCOLO X OGNUNA LA SUP.IMPIEGATA ---
        '--------------------------------------------------------------------

        Dim chiave_hash As String
        Dim table_hash As New Hashtable

        'Cmb_Macrousi.Items.Clear()
        'Cmb_Utilizzo1.Items.Clear()

        'Se il recordset esiste ...
        If Not IsNothing(Dt_Particelle) Then

            Progressivo = 1

            For i = 0 To Dt_Particelle.Rows.Count - 1

                Prov = Dt_Particelle.Rows(i).Item("prov")
                Com = Dt_Particelle.Rows(i).Item("com")

                If Dt_Particelle.Rows(i).Item("sezione") = "0" Then
                    Sezione = ""
                Else
                    Sezione = Trim(Dt_Particelle.Rows(i).Item("sezione"))
                End If

                Foglio = Dt_Particelle.Rows(i).Item("foglio")
                Numero = Dt_Particelle.Rows(i).Item("numero")

                If Dt_Particelle.Rows(i).Item("Subalterno") = "0" Then
                    Subalterno = ""
                Else
                    Subalterno = Trim(Dt_Particelle.Rows(i).Item("subalterno"))
                End If

                If (Dt_Particelle.Columns.Contains("ZVN")) Then
                    ZVN = Dt_Particelle.Rows(i).Item("ZVN")
                Else
                    ZVN = ""
                End If

                'RIEMPIO LA COMBO MACROUSI CON QUELLI GESTITI
                If flag_Macrousi Then

                    Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")
                    Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")
                    Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")

                Else
                    Macrouso_Cod = ""
                    Macrouso_Des = ""
                    Sup_Macrouso = 0
                End If


                'RIEMPIO LE COMBO UTILIZZI
                If flag_Utilizzi Then

                    'Dim cmb_index_u As Integer
                    'Dim cmb_flag_u As Boolean

                    Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")
                    Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")
                    Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")

                    Veg_Cod_Agea = Dt_Particelle.Rows(i).Item("Veg_Cod_agea")
                    Veg_Des_Agea = Dt_Particelle.Rows(i).Item("Veg_Des_agea")
                    Cul_Cod_Agea = Dt_Particelle.Rows(i).Item("Cul_Cod_agea")
                    Cul_Des_Agea = Dt_Particelle.Rows(i).Item("Cul_Des_agea")
                    Sup_Utilizzo = Dt_Particelle.Rows(i).Item("Sup_Utilizzo")

                Else
                    Veg_Cod_Agea = ""
                    Veg_Des_Agea = ""
                    Cul_Cod_Agea = ""
                    Cul_Des_Agea = ""
                    Sup_Utilizzo = 0
                End If

                'creo la stringa CHIAVE
                chiave_hash = Prov & "§" & Com & "§" & Sezione & "§" & Foglio.ToString & "§" & Numero.ToString & "§" & Subalterno & "§" & Macrouso_Cod & "§" & Veg_Cod_Agea & "§" & Cul_Cod_Agea

                If Not table_hash.ContainsKey(chiave_hash) Then

                    table_hash.Add(chiave_hash, "")
                    DrParticella = Dt_Particelle.Select("prov='" & Dt_Particelle.Rows(i).Item("prov").ToString &
                                    "' AND com='" & Dt_Particelle.Rows(i).Item("com").ToString &
                                    "' AND sezione='" & Dt_Particelle.Rows(i).Item("sezione").ToString &
                                    "' AND foglio='" & Dt_Particelle.Rows(i).Item("foglio").ToString &
                                    "' AND numero='" & Dt_Particelle.Rows(i).Item("numero").ToString &
                                    "' AND subalterno='" & Dt_Particelle.Rows(i).Item("subalterno").ToString & "'")

                    Sup_Condotta_Min = 0

                    For j = 0 To DrParticella.Length - 1
                        If j = 0 Then
                            Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
                        Else
                            If CDbl(DrParticella(j).Item("sup_condotta")) <> 0 Then
                                If Sup_Condotta_Min > CDbl(DrParticella(j).Item("sup_condotta")) Then
                                    Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
                                    ' SuperficieTotaleUsata = CDbl(DrParticella(j).Item("sup_condotta"))
                                End If
                            End If
                        End If
                    Next

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    'Definisco i valori

                    '---
                    Dr.Item("chiave") = chiave_hash
                    Dr.Item("CodiceIstat_Provincia") = Dt_Particelle.Rows(i).Item("prov")
                    Dr.Item("CodiceIstat_Comune") = Dt_Particelle.Rows(i).Item("com")
                    Dr.Item("Part_Cod") = Dt_Particelle.Rows(i).Item("part_cod")

                    Dr.Item("Progressivo") = Progressivo

                    '-----

                    Dr.Item("Provincia") = Dt_Particelle.Rows(i).Item("COMUNI_PROV")
                    Dr.Item("Comune") = Dt_Particelle.Rows(i).Item("LOCALITA")

                    '-----

                    Dr.Item("Sezione") = If(Dt_Particelle.Rows(i).Item("Sezione") = "0", "", Dt_Particelle.Rows(i).Item("Sezione"))
                    Dr.Item("Foglio") = Dt_Particelle.Rows(i).Item("Foglio")
                    Dr.Item("Numero") = Dt_Particelle.Rows(i).Item("Numero")
                    Dr.Item("Subalterno") = If(Dt_Particelle.Rows(i).Item("Subalterno") = "0", "", Dt_Particelle.Rows(i).Item("Subalterno"))

                    Dr.Item("ZVN") = ZVN

                    '-----

                    Dr.Item("Superficie") = Format(Sup_Condotta_Min, "0.0000")

                    Dr.Item("SuperficieLorda") = Format(UtilityProvider.Ettari_from_EttariAreCentiare(
                                                        CDbl(Dt_Particelle.Rows(i).Item("particella_ettari")),
                                                        CDbl(Dt_Particelle.Rows(i).Item("particella_are")),
                                                        CDbl(Dt_Particelle.Rows(i).Item("particella_centiare"))),
                                                        "0.0000")


                    '--------------------------------------------------------------------------
                    ' Se l'Appezzamento appartiene ad un campo SQUADRO
                    ' l'area della particella utilizzata è
                    ' la somma delle intersezioni degli altri appezzamenti aggregati al campo!
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    ' Se l'Appezzamento appartiene ad un campo NON SQUADRO
                    ' o NON appartiene a campi
                    ' l'area della particella utilizzata è
                    ' la somma delle intersezioni con gli altri campi squadri + 
                    ' l'area di intersezione con gli appezzamenti non aggregati in campo!
                    '--------------------------------------------------------------------------

                    Dim sezioneMacr As String
                    Dim subMacr As String

                    If Dt_Particelle.Rows(i).Item("sezione").ToString = "" Then
                        sezioneMacr = "0"
                    Else
                        sezioneMacr = Dt_Particelle.Rows(i).Item("sezione").ToString
                    End If

                    If Dt_Particelle.Rows(i).Item("subalterno").ToString = "" Then
                        subMacr = "0"
                    Else
                        subMacr = Dt_Particelle.Rows(i).Item("subalterno").ToString
                    End If

                    If Macrouso_Cod <> "" Then

                        Dim DtSuperficie As DataTable

                        DtSuperficie = objAppxPartxMacr.Leggi_SuperficieMacrousoUtilizzata(Piva, Sa_Cod, 0,
                                                                                           Dt_Particelle.Rows(i).Item("PROV"),
                                                                                           Dt_Particelle.Rows(i).Item("COM"),
                                                                                           sezioneMacr,
                                                                                           CInt(Dt_Particelle.Rows(i).Item("Foglio")),
                                                                                           CInt(Dt_Particelle.Rows(i).Item("Numero")),
                                                                                           subMacr,
                                                                                           Macrouso_Cod,
                                                                                           "", "", objParametri_Server)

                        SuperficieDisponibile = Sup_Macrouso

                        If Not IsDBNull(DtSuperficie.Rows(0).Item("Superficie")) Then
                            SuperficieDisponibile -= DtSuperficie.Rows(0).Item("Superficie")
                        End If

                        Dr.Item("SuperficieMacrousoDisponibile") = Format(SuperficieDisponibile, "0.0000")

                    End If

                    If Veg_Des_Agea <> "" Then

                        Dim DtSuperficieUt As DataTable

                        DtSuperficieUt = objAppxPartxMacrxUtil.Leggi_SuperficieMacrousoxUtilizzo(Piva, Sa_Cod, 0,
                                                                                            Dt_Particelle.Rows(i).Item("PROV"),
                                                                                            Dt_Particelle.Rows(i).Item("COM"),
                                                                                            sezioneMacr,
                                                                                            CInt(Dt_Particelle.Rows(i).Item("Foglio")),
                                                                                            CInt(Dt_Particelle.Rows(i).Item("Numero")),
                                                                                            subMacr,
                                                                                            Macrouso_Cod,
                                                                                            Veg_Cod_Agea,
                                                                                            Cul_Cod_Agea,
                                                                                            "", "", objParametri_Server)

                        SuperficieDisponibile = Sup_Utilizzo

                        If Not IsDBNull(DtSuperficieUt.Rows(0).Item("Superficie")) Then
                            SuperficieDisponibile -= DtSuperficieUt.Rows(0).Item("Superficie")
                        End If

                        Dr.Item("SuperficieUtilizzoDisponibile") = Format(SuperficieDisponibile, "0.0000")

                    Else
                        Dr.Item("SuperficieUtilizzoDisponibile") = "0"
                    End If

                    If Squadro AndAlso Campo_Cod <> 0 Then

                        'Appezzamenti Aggregati allo squadro
                        SuperficieTotaleUsata = CDbl(Dt_Particelle.Rows(i).Item("SuperficieAppSquadro"))

                        'Calcolo quella disponibile
                        SuperficieDisponibile = CDbl(Sup_Condotta_Min) - SuperficieTotaleUsata

                        Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")

                    Else

                        SuperficieDisponibile = CDbl(Dt_Particelle.Rows(i).Item("SuperficieDisponibile"))

                        Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")

                    End If


                    'End If


                    '--------------------------------------------------------
                    '--------------------------------------------------------
                    '--------------------------------------------------------
                    '--------------------------------------------------------
                    '--------------------------------------------------------

                    Dr.Item("SuperficieImpiegata") = 0

                    Dr.Item("Macrouso_Cod") = Macrouso_Cod
                    Dr.Item("Macrouso_Des") = Macrouso_Des
                    Dr.Item("Sup_Macrouso") = Sup_Macrouso

                    Dr.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                    If Cul_Des_Agea = "" Then
                        Dr.Item("Veg_Des_Agea") = Veg_Des_Agea
                    Else
                        Dr.Item("Veg_Des_Agea") = Veg_Des_Agea & " - " & Cul_Des_Agea
                    End If

                    Dr.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                    Dr.Item("Cul_Des_Agea") = Cul_Des_Agea
                    Dr.Item("Sup_Utilizzo") = Sup_Utilizzo


                    '-----

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                    'Aggiorno il contatore
                    Progressivo += 1

                End If

            Next

        End If




        'In questo momento la "SuperficieDisponibile" è solo parziale
        Dim Dt_SuperfInter As DataTable = Dt

        '//////////////////////////////////////////////////////////////////////////////////////////////
        ' MODIFICA
        '----- Per ciascuna particella ripristino i check e le sup di intersezione
        '//////////////////////////////////////////////////////////////////////////////////////////////

        If CInt(Appezza) <> 0 Then

            Dim objCOMAP As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R 'Object New Agro_Anagrafe_AD.AppezzaxParticelle_R
            Dim DTRsAP As DataTable
            Dim xy As Integer
            Dim HTAppezzamenti As New Hashtable()

            Dim DtMacr As New DataTable
            DtMacr = objAppxPartxMacr.Leggi(Piva, Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", objParametri_Server)

            Dim DtUtil As New DataTable
            DtUtil = objAppxPartxMacrxUtil.Leggi(Piva, Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", "", "", objParametri_Server)

            'Cerco le intersezioni eventuali
            DTRsAP = objCOMAP.LeggiParticelle_Da_Appezzamento(
                                                    CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    CInt(Appezza),
                                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "",
                                                    "", objParametri_Server)

            objCOMAP = Nothing

            'SE CI SONO UTILIZZI
            If flag_Utilizzi AndAlso DtUtil.Rows.Count > 0 Then

                For xy = 0 To DtUtil.Rows.Count - 1

                    For i = 0 To Dt_SuperfInter.Rows.Count - 1

                        'Recupero la chiave
                        Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                        Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                        Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                        Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                        Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                        Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")
                        Macrouso_Cod = Dt_SuperfInter.Rows(i).Item("Macrouso_Cod")
                        Veg_Cod_Agea = Dt_SuperfInter.Rows(i).Item("Veg_Cod_agea")
                        Cul_Cod_Agea = Dt_SuperfInter.Rows(i).Item("Cul_Cod_agea")

                        If Sezione = "&nbsp;" OrElse Sezione = "" Then
                            Sezione = "0"
                        End If

                        If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                            Subalterno = "0"
                        End If

                        If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea))) AndAlso
                               Prov = DtUtil.Rows(xy).Item("prov") AndAlso
                               Com = DtUtil.Rows(xy).Item("com") AndAlso
                               Sezione = DtUtil.Rows(xy).Item("sezione") AndAlso
                               Foglio = DtUtil.Rows(xy).Item("foglio") AndAlso
                               Numero = DtUtil.Rows(xy).Item("numero") AndAlso
                               Subalterno = DtUtil.Rows(xy).Item("Subalterno") AndAlso
                               Macrouso_Cod = DtUtil.Rows(xy).Item("Macrouso_Cod") AndAlso
                               Veg_Cod_Agea = DtUtil.Rows(xy).Item("Veg_Cod_Agea") AndAlso
                               Cul_Cod_Agea = DtUtil.Rows(xy).Item("Cul_Cod_Agea") Then

                            SuperficieIntersezione = DtUtil.Rows(xy).Item("Superficie")
                            'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

                            'TO VERIFY colonna del check
                            Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                            'TO VERIFY colonna intersezione
                            Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                            'essendo in modifica risommo la sup del macrouso assegnata a questo campo

                            Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                            Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile")) + SuperficieIntersezione, "0.0000")
                            Dt_SuperfInter.Rows(i).Item("SuperficieUtilizzoDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieUtilizzoDisponibile")) + SuperficieIntersezione, "0.0000")

                            HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea), 1)

                        End If

                    Next


                Next

            Else

                'SE CI SONO MACROUSI
                If flag_Macrousi AndAlso DtMacr.Rows.Count > 0 Then

                    For xy = 0 To DtMacr.Rows.Count - 1

                        For i = 0 To Dt_SuperfInter.Rows.Count - 1

                            'Recupero la chiave
                            Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                            Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                            Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                            Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                            Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                            Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")
                            Macrouso_Cod = Dt_SuperfInter.Rows(i).Item("Macrouso_Cod")

                            If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                Sezione = "0"
                            End If

                            If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                Subalterno = "0"
                            End If

                            If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod))) AndAlso
                                   Prov = DtMacr.Rows(xy).Item("prov") AndAlso
                                   Com = DtMacr.Rows(xy).Item("com") AndAlso
                                   Sezione = DtMacr.Rows(xy).Item("sezione") AndAlso
                                   Foglio = DtMacr.Rows(xy).Item("foglio") AndAlso
                                   Numero = DtMacr.Rows(xy).Item("numero") AndAlso
                                   Subalterno = DtMacr.Rows(xy).Item("Subalterno") AndAlso
                                   Macrouso_Cod = DtMacr.Rows(xy).Item("Macrouso_Cod") Then


                                SuperficieIntersezione = DtMacr.Rows(xy).Item("Superficie")

                                'colonna del check
                                Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                                'colonna intersezione
                                Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")


                                'essendo in modifica risommo la sup del macrouso assegnata a questo campo
                                DtMacr.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(DtMacr.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                DtMacr.Rows(i).Item("SuperficieMacrousoDisponibile") = Format(CDbl(DtMacr.Rows(i).Item("SuperficieMacrousoDisponibile")) + SuperficieIntersezione, "0.0000")

                                HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod), 1)

                            End If

                        Next

                    Next

                Else

                    For xy = 0 To DTRsAP.Rows.Count - 1

                        For i = 0 To Dt_SuperfInter.Rows.Count - 1

                            'Recupero la chiave
                            Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                            Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                            Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                            Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                            Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                            Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")

                            If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                Sezione = "0"
                            End If

                            If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                Subalterno = "0"
                            End If

                            If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno))) AndAlso
                                   Prov = DTRsAP.Rows(xy).Item("prov") AndAlso
                                   Com = DTRsAP.Rows(xy).Item("com") AndAlso
                                   Sezione = DTRsAP.Rows(xy).Item("sezione") AndAlso
                                   Foglio = DTRsAP.Rows(xy).Item("foglio") AndAlso
                                   Numero = DTRsAP.Rows(xy).Item("numero") AndAlso
                                   Subalterno = DTRsAP.Rows(xy).Item("Subalterno") Then

                                SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

                                If (Tipo_Operazione <> enum_TipoOperazioneDB.Scrittura) Then
                                    'colonna del check
                                    Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                                    'colonna intersezione
                                    Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                                    'essendo in modifica risommo la sup assegnata a questo campo
                                    Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                Else
                                    'colonna del check
                                    Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = False

                                    'colonna intersezione
                                    Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                                    'essendo in modifica risommo la sup assegnata a questo campo
                                    Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                End If

                                Exit For

                            End If

                        Next

                        SuperficieIntersezioneTotale += SuperficieIntersezione

                    Next

                End If

            End If

        End If

        Return Dt_SuperfInter

    End Function

    Public Function Leggi_AppezzamentoCatasto(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)

        Dim catastoApp As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
        Dim objAppxPart As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim DtAppxPart As DataTable

        DtAppxPart = objAppxPart.LeggiParticelle_Da_Appezzamento(Piva, Sa_Cod, Appezza,
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objParametri_Server)

        If DtAppxPart.Rows.Count > 0 Then

            For Each r As DataRow In DtAppxPart.Rows

                Dim prov = r("Prov")
                Dim com = r("Com")
                Dim sezione = r("Sezione")
                Dim foglio = r("Foglio")
                Dim numero = r("Numero")
                Dim subalterno = r("subalterno")

                If sezione = "0" Then
                    sezione = ""
                End If

                If subalterno = "0" Then
                    subalterno = ""
                End If

                Dim Catasto_Appezzamento As New AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento
                Dim particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                    prov, com, sezione, foglio, numero, subalterno
                )

                Catasto_Appezzamento.area = r("AREA")
                Catasto_Appezzamento.particella = particella

                catastoApp.Add(Catasto_Appezzamento)
            Next


        End If
        Return catastoApp
    End Function

    Public Function LeggiMaxData(Piva As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As DateTime
        Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

            Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            Dim DateList As New List(Of DateTime)

            Dim maxDataAppezza As DateTime = objAppezzamenti.Leggi_Max_DataModifica(Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim maxDataImpianti As DateTime = objImpianti.Leggi_Max_DataModifica(Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim objEsercizi As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

            Dim maxDataEsercizi As DateTime = objEsercizi.Leggi_Max_DataModifica(Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim listTipo As New List(Of String) From {"Appezzamento", "Reg_Impianti", "Imprese_Progetti"}

            Dim maxLog = AGRODATAINIZIO
            Dim maxLogEls = (From al In GiasContext.Agronica_Log_Anagrafe
                             Where al.Param1 = Piva AndAlso listTipo.Contains(al.Tipo)
                             Select al.Data_Ora_RegistrazioneLog).ToList()
            If maxLogEls.Count > 0 Then
                maxLog = maxLogEls.Max()
            End If

            DateList.Add(maxDataAppezza)
            DateList.Add(maxLog)
            DateList.Add(maxDataImpianti)
            DateList.Add(maxDataEsercizi)

            Return DateList.Max()

        End Using
    End Function

    Public Function Leggi_Appezzamenti_APP(piva As String,
                                           data As Date,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri
                                           ) As List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim StaticMapCFG As New GeneraMappaStaticaInData

        Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)
        Dim leggiStaticMap As Boolean = False
        Dim GeneraStaticMapDaSincroAPP As Boolean = False
        If jSonStaticMapCFG <> "" Then
            StaticMapCFG = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
            If StaticMapCFG.StaticMapAttive Then
                leggiStaticMap = True
            End If
            If StaticMapCFG.GeneraStaticMapDaSincroAPP Then
                GeneraStaticMapDaSincroAPP = True
            End If
        End If

        Dim objImpiantiBIZ_R As New Reg_Impianto_R
        Dim objImpiantiBIZ_W As New Reg_Impianto_W
        Dim LeggiAncheBloccati As Boolean = objImpiantiBIZ_R.BlockedImplantsReadSetting(objParametri_Utenti)

        Dim listImpianti As New List(Of String)
        Dim appezzamenti As New List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)
        Dim repository As New Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim dtImpianti = objImpianti.Leggi_Impianti_APP(
            piva,
            0,
            0,
            0,
            "",
            data,
            leggiStaticMap OrElse GeneraStaticMapDaSincroAPP,
            "",
            "",
            objParametri_Server,
            leggiAncheBloccati:=LeggiAncheBloccati,
            False
            )

        Dim objCodiciAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
        Dim objCodiciImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        'Troviamo tutti gli impianti che non hanno la StaticMap e la generiamo
        If GeneraStaticMapDaSincroAPP Then
            objImpiantiBIZ_W.GeneraStaticMapDaSincroAPP(dtImpianti, StaticMapCFG, objParametri_Server)
        End If

        For Each row In dtImpianti.Rows

            Dim chiave_appezzamento As String = row.Item("Piva") & "_" & row.Item("Sa_Cod") & "_" & row.Item("appezza")
            Dim chiave_impianto As String = chiave_appezzamento & "_" & row.Item("Id_Reg")

            Dim appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento
            Dim impianto As New AgronicaCoreModelsSTD.anagrafiche.Impianto

            If Not repository.ContainsKey(chiave_appezzamento) Then

                Dim indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Appezzamento(row.Item("Piva"), row.Item("Sa_Cod"), row.Item("appezza"), objParametri_Server)

                Dim dtCodici = objCodiciAppezzamento.Leggi(row.Item("Piva"), row.Item("Sa_Cod"), row.Item("appezza"), 0, "",
                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                               "(id_cod < 2000 OR id_cod >= 3000)", "", objParametri_Server)

                Dim codici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

                For Each drCodici In dtCodici.Rows
                    codici.Add(New CodiciAnagrafeValori With {
                        .valore = drCodici.Item("val_cod"),
                        .validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE),
                        .codiceAnagrafe = New CodiceAnagrafe With {
                            .codice = drCodici.Item("id_Cod"),
                            .descrizione = drCodici.Item("descrizione")
                        }
                    })
                Next

                Dim blkAppezzamento As New BlockAppezzamento With {
                    .blkFlag = row.Item("Blk_Flag") <> 0,
                    .blkInizioData = row.Item("Blk_Inizio_Data"),
                    .blkFineData = row.Item("Blk_Fine_Data"),
                    .blkInizioUsername = row.Item("Blk_Inizio_Username"),
                    .blkFineUsername = row.Item("Blk_Fine_Username"),
                    .blkInizioNote = row.Item("Blk_Inizio_Note"),
                    .blkFineNote = row.Item("Blk_Fine_Note")
                    }

                appezzamento = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento() With {
                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK() With {
                        .codice = row("appezza"),
                        .centroAziendalePK = New CentroAziendale.PK(row.Item("Sa_Cod"), row.Item("Piva"))
                    },
                    .campoPK = New Campo.PK(row("campo_cod"), New CentroAziendale.PK(row.Item("Sa_Cod"), row.Item("Piva"))),
                    .descrizione = row("app_nome"),
                    .superficie = row("sup_app"),
                    .rif_Appezzamento = row("codici_anagrafe_appezzamento"),
                    .indirizzi = indirizzi,
                    .impianti = New List(Of Impianto),
                    .codici = codici,
                    .validita = New IntervalloTemporale(row("Validita_Inizio_Appezza"), row("Validita_Fine_Appezza")),
                    .blkAppezzamento = blkAppezzamento
                }

                appezzamenti.Add(appezzamento)

                repository.Add(chiave_appezzamento, appezzamento)
            Else

                appezzamento = repository(chiave_appezzamento)

            End If

            If Not listImpianti.Contains(chiave_impianto) Then

                Dim utilizzo As UtilizzoTerreno

                If row("veg_cod") = 0 Then
                    utilizzo = New DestinazioneUso(row("id_cod")) With {.descrizione = row("codici_anagrafe_des")}
                Else
                    utilizzo = New Varieta(row("cul_cod")) With {
                        .descrizione = row("cul_des"),
                        .specie = New Specie(row("veg_cod")) With {.descrizione = row("veg_des")}
                    }
                End If

                Dim dtCodici = objCodiciImpianto.Leggi(row.Item("Piva"), row.Item("Sa_Cod"), row.Item("appezza"), row.Item("id_reg"), "", TipiEnumerativi.enum_CodiciAnagrafe.CodiceCatalogoAgeaDemetra, "",
                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                               "", "", objParametri_Server)

                Dim codici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

                For Each drCodici In dtCodici.Rows
                    codici.Add(New CodiciAnagrafeValori With {
                        .valore = drCodici.Item("val_cod"),
                        .validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE),
                        .codiceAnagrafe = New CodiceAnagrafe With {
                            .codice = drCodici.Item("id_Cod")
                        }
                    })
                Next

                impianto = New AgronicaCoreModelsSTD.anagrafiche.Impianto() With {
                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK() With {
                        .codice = row("id_reg"),
                        .appezzamentoPK = appezzamento.primaryKey
                    },
                    .descrizione = row("imp_des"),
                    .superficie = row("sup_imp"),
                    .utilizzoTerreno = utilizzo,
                    .gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(row("grfi_cod")) With {.descrizione = row("grfi_des")},
                    .validita = New IntervalloTemporale(row("Validita_Inizio_Impianto"), row("Validita_Fine_Impianto")),
                    .esercizi = New List(Of Esercizio),
                    .cover_Crops = row("cover") = 1,
                    .codici = codici
                }

                impianto.cartografia = row("cartografia")
                impianto.codiceImpianto = row("Codici_Anagrafe_Impianto")
                impianto.Agea_idColt = row("Agea_idColt")

                If leggiStaticMap OrElse GeneraStaticMapDaSincroAPP Then
                    impianto.immagineBase64 = row("StaticMapBase64String")
                End If

                appezzamento.impianti.Add(impianto)

                listImpianti.Add(chiave_impianto)

            Else

                For Each imp In appezzamento.impianti
                    If imp.primaryKey.codice = row("id_reg") Then
                        impianto = imp
                    End If
                Next

            End If

            If impianto IsNot Nothing Then

                impianto.esercizi.Add(New Esercizio() With {
                    .codice = row("progetto_cod"),
                    .impiantoPK = impianto.primaryKey,
                    .descrizione = row("progetto"),
                    .resa_prevista = row("produzione_prevista"),
                    .validita = New IntervalloTemporale(row("Validita_Inizio_Distinta"), row("Validita_Fine_Distinta"))
                    })

            End If

        Next

        Return appezzamenti

    End Function

    Public Shared Function VerificaEsistenzaEntitaAnagrafiche(ByVal PivaSuperUser As String,
                                                         ByVal Piva As String,
                                                         ByVal Sa_Cod As Int32,
                                                         ByVal Appezza As Int32,
                                                         ByVal Id_Reg As Int32,
                                                         ByRef objParametri_Server As AgronicaCoreParametri
                                                        ) As VerificaEsistenzaEntitaAnagrafiche

        Dim ret As New VerificaEsistenzaEntitaAnagrafiche

        If Id_Reg > 0 Then
            Dim leggiImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim dtImpianto As DataTable = leggiImpianto.Leggi(Piva, Sa_Cod, Appezza, Id_Reg,
                                                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                              "", "",
                                                              objParametri_Server)

            If dtImpianto IsNot Nothing AndAlso dtImpianto.Rows.Count > 0 Then
                ret.EsisteImpianto = True
            End If
        End If

        Dim leggiAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim dtAppezzamento As DataTable = leggiAppezzamento.Leggi(Piva,
                                                                  Sa_Cod,
                                                                  Appezza,
                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                  "",
                                                                  "",
                                                                  objParametri_Server)

        If dtAppezzamento IsNot Nothing AndAlso dtAppezzamento.Rows.Count > 0 Then
            ret.EsisteAppezzamento = True
        End If

        Return ret

    End Function

    Public Shared Function VerificaEsistenzaAppezzamentoDaCodice(ByVal piva As String,
                                                                 ByVal sa_cod As Integer,
                                                                 ByVal val_cod As String,
                                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                                 Optional ByVal leggiValCodPerLike As Boolean = True
                                                                 ) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK

        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK = Nothing
        Dim xAppCodR As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R

        Try


            Dim r = xAppCodR.Leggi(piva, sa_cod, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, val_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server, leggiValCodPerLike)
            If r.Rows.Count > 0 Then
                ret = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK
                ret.codice = r.Rows(0)("Appezza")
                ret.centroAziendalePK = New CentroAziendale.PK(sa_cod, piva)
            End If

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ReadAppezzamentoCampoCod(piva As String,
                                             saCod As Integer,
                                             appezza As Integer,
                                             objParametriServer As AgronicaCoreParametri) As Integer

        Dim dal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim dt = dal.ReadAppezzamentoCampoCod(piva, saCod, appezza, objParametriServer)

        If dt.Rows.Count = 0 Then
            Throw New GiasException("Appezzamento not found")
        Else
            Return If(IsDBNull(dt.Rows(0).Item("Campo_Cod")), 0, dt.Rows(0).Item("Campo_Cod"))
        End If
    End Function

    ''' <summary>
    ''' Legge i valori dei codici di un appezzamento per gli identificativi specificati.
    ''' </summary>
    ''' <param name="piva">Partita IVA dell'impresa</param>
    ''' <param name="saCod">Codice del centro aziendale</param>
    ''' <param name="appezza">Codice dell'appezzamento</param>
    ''' <param name="idCods">Lista degli identificativi dei codici da leggere</param>
    ''' <param name="objParametri">Parametri di connessione e contesto</param>
    Public Function LeggiCodiciPerIdCods(piva As String,
                                          saCod As Integer,
                                          appezza As Integer,
                                          idCods As List(Of enum_CodiciAnagrafe),
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        
        If IdCods Is Nothing OrElse IdCods.Count = 0 Then
            Throw New Exception("IdCods è un parametro obbligatorio e non può essere vuoto")
        End If

        Dim cods = idCods.Select(Function(cod) CInt(cod)).ToList()
        
        Dim dal As New Appezzamento_Codici_R
        Return dal.LeggiPerIdCods(piva, saCod, appezza, cods, objParametri)

    End Function

End Class

'##########################################################################################
'##########################################################################################
'##########################################################################################
'##########################################################################################

Public Class Appezzamento_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella(ByVal xpiva As String,
                             ByVal xsa_Cod As Integer,
                             ByVal xappezza As Integer,
                             ByVal Utente_Username As String,
                             ByVal idServizio As Integer,
                             ByVal objParametri_Server As AgronicaCoreParametri,
                             ByVal objParametri_Utenti As AgronicaCoreParametri
                             ) As RispostaStandard

        Dim result As New RispostaStandard

        If Not (xpiva <> "" AndAlso xsa_Cod <> 0 AndAlso xappezza <> 0) Then
            result.RispostaOK = False
            result.Errore = "La funzione di cancellazione è stata chiamata con parametri non validi."
            Return result
        End If


        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                    Utente_Username,
                                    idServizio,
                                    enum_Security_Attivita.Anagrafica_Appezzamento,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        If Not UtenteAbilitato Then
            result.RispostaOK = False
            result.Errore = "Non si dispone dei permessi per cancellare l'appezzamento."
            Return result
        End If


        'controllo che non ci siano registrazioni di agenda riferite all'impianto

        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DtAgenda As DataTable
        Dim DtRicette As DataTable
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        DtAgenda = ObjAgenda.Leggi(CStr(xpiva),
                                 CInt(xsa_Cod),
                                 0,
                                 0,
                                 0,
                                 CInt(xappezza),
                                 0,
                                 0,
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "",
                                 "",
                                 objParametri_Server)

        DtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                             CStr(xpiva),
                                             CInt(xsa_Cod),
                                             CInt(xappezza),
                                             0,
                                             AGRODATAINIZIO,
                                             AGRODATAFINE,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri_Server)



        If DtAgenda.Rows.Count <> 0 OrElse DtRicette.Rows.Count <> 0 Then
            'Throw New Exception("Impossibile eliminare l'appezzamento poiché esistono delle registrazioni ad esso associate!" & Chr(13) & _
            '            " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!")

            result.RispostaOK = False
            result.Errore = "Impossibile eliminare l'appezzamento, perché esistono delle registrazioni associate ad un impianto!" & Chr(13) &
                        " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!"
            Return result
        End If

        'Prima di eliminare controllo Movimenti, Ricette e PUA
        Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, CStr(xpiva), CInt(xsa_Cod), CInt(xappezza), 0, 0, objParametri_Server)
        If controllo.errore Then
            result.RispostaOK = False
            result.Errore = "Impossibile eliminare l'appezzamento, perché esistono dei Costi di Gestione associati ad un esercizio!" & Chr(13) &
                    " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Menu CdG'!"
            Return result
        End If

        'controllo se l'appezzamento è bloccato
        Dim objAppezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim DT_Appezza As DataTable
        DT_Appezza = objAppezza_R.Leggi(xpiva, xsa_Cod, xappezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If DT_Appezza.Rows(0).Item("blk_Flag") = -1 Then
            'Throw New Exception("Impossibile eliminare l'impianto poiché è stato bloccato da smart!")

            result.RispostaOK = False
            result.Errore = "Impossibile eliminare l'impianto poiché è stato bloccato da smart!"
            Return result

        End If


        'CANCELLIAMO I POLIGONI IN Appezzamento_Scrivi()
        'controllo se l'Appezzamento è collegato ad un Entità  GIS
        'Dim objGIS As New AgronicaCoreGisBIZ.GIS_Entita_R
        'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(xpiva, xsa_Cod, xappezza, 0, 0, enum_GIS2012_TipoEntita.APPEZZAMENTI, objParametri_Server)) Then
        '    result.RispostaOK = False
        '    result.Errore = Gias.ImpossibileCancellareAppezzamentoCollegatoPoligono
        '    Return result
        'End If

        ''controllo se esistono impianti collegati ad un Entità  GIS
        'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(xpiva, xsa_Cod, xappezza, 0, 0, enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA, objParametri_Server)) Then
        '    result.RispostaOK = False
        '    result.Errore = Gias.ImpossibileCancellareImpiantoCollegatoPoligono
        '    Return result
        'End If

        'controllo se è uguale a 1 e chiedo conferma
        'If DT_Appezza.Rows(0).Item("blk_Flag") = 1 And SI_NO.Value = 0 Then
        '    Dim Testo As String
        '    Testo = "E' stata fatta una misura tramite Palmare, si desidera comunque cancellare?"
        '    Testo = HttpContext.Current.Server.UrlEncode(Testo)
        '    Dim Stringa As String = "<script language='javascript'> " & _
        '                        " a = window.showModalDialog(" & Chr(34) & "../AA_Script/Controlli/AgroSiNo/AgroSiNo.aspx?des=" & Testo & Chr(34) & "," & Chr(34) & Chr(34) & "," & Chr(34) & "dialogWidth:260px;dialogHeight:350px;status:no; center:yes;edge:raised;" & Chr(34) & ") " & _
        '                        vbCrLf & " document.all(" & Chr(34) & "SI_NO" & Chr(34) & ").value = a" & _
        '                        vbCrLf & " rispostaSiNo();" & _
        '                        "</script>"
        '    Me.FindControl("Form1").Controls.Add(New LiteralControl(Stringa))
        '    Exit Function
        'End If

        'SI_NO.Value = 0


        'cancello l'appezzamento
        Dim ObjAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

        Dim Dati As String = ObjAppezzamentoR.Appezzamento_Leggi(
                                CStr(xpiva),
                                CInt(xsa_Cod),
                                0,
                                CInt(xappezza),
                                CBool(True),
                                CBool(False),
                                CBool(True),
                                CBool(False),
                                objParametri_Server)

        ObjAppezzamentoR = Nothing

        If Dati <> "" Then

            'Cancello l'elemento
            Dim rvalAppezzamentoScrivi As Boolean = Appezzamento_Scrivi(
                CStr(Dati),
                Nothing,
                Nothing,
                Nothing,
                objParametri_Server,
                objParametri_Utenti)

            If Not rvalAppezzamentoScrivi Then
                result.RispostaOK = False
                result.RispostaStringa = "Si è verificato un errore in fase di scrittura. Ripetere l'operazione"
                Return result
            End If

        End If


        result.RispostaOK = True
        result.RispostaStringa = "Appezzamento correttamente cancellato"


        Return result

    End Function

    Public Function Appezzamento_Scrivi(ByVal DatiAppezzamento As String,
                                        ByRef OUTPUT_Piva As String,
                                        ByRef OUTPUT_Sa_Cod As Integer,
                                        ByRef OUTPUT_Appezza As Integer,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Veg_Des As String = "",
                                        Optional ByVal TipoG2G As Integer = 0,
                                        Optional ByVal Date_Modifiche_Impianti_Verificate As Boolean = False,
                                        Optional NoteLog As String = ""
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Appezzamento_Scrivi()"

        'Dim NomeConnessione As String
        Dim XmlDoc As XmlDocument

        Dim objSequenze As AgronicaCoreDataProvider.Agro_Sequenze
        Dim objUtentixAppezzamenti As AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
        Dim ObjAppezzamento_Storico As AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W
        Dim objAppezzamenti As AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objReg_Impianti As AgronicaCoreAnagrafeDAL.Reg_Impianti_Write    ' Basso livello
        Dim objAppezzaxParticelle As AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim objAppezzaxParticellexMacrousi As AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
        Dim objAppezzaxParticellexMacrousixUtilizzi As AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W
        Dim objAppezzaxCodici As AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
        Dim objAppezzaxIndirizzi As AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write
        Dim objIndirizzi As AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim objGrafica As AgronicaCoreGraficaBIZ.Grafica_Write
        Dim objGrafica_AD As AgronicaCoreGraficaDAL.Grafica_Write

        Dim GraphicKey As String

        'Dim objGrafica                As Object

        'Figlio Maggiore = Reg_Impianto
        Dim objReg_ImpiantiLeggi As AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
        Dim objReg_ImpiantiScrivi As AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

        Dim XmlReg_Impianti As String

        Dim Dummy As Long
        Dim Cod_Appezzamento As Long

        Dim BaseCode As Long

        Dim xDatiAppezzamenti As XmlNodeList
        Dim xDatiAppezzamento As XmlElement
        Dim xAppezzamenti As XmlNodeList
        Dim xAppezzamento As XmlElement
        Dim xParticelle As XmlNodeList
        Dim xParticella As XmlElement
        Dim xMacrousi As XmlNodeList
        Dim xMacrouso As XmlElement
        Dim xUtilizzi As XmlNodeList
        Dim xUtilizzo As XmlElement
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement
        Dim xIndirizzi As XmlNodeList
        Dim xIndirizzo As XmlElement
        Dim xEntitaGrafiche As XmlNodeList
        Dim xDatiImpianti As XmlNodeList
        Dim xDatoImpianti As XmlElement
        Dim xImpianti As XmlNodeList
        Dim xImpianto As XmlElement
        Dim xDatiAppezzamenti_Storico As XmlNodeList
        Dim xDatiAppezzamento_Storico As XmlElement
        Dim xAppezzamenti_Storico As XmlNodeList
        Dim xAppezzamento_Storico As XmlElement

        Dim i_DatiAppezzamento As Integer
        Dim i_Appezzamento As Integer
        Dim i_Particella As Integer
        Dim i_Macrouso As Integer
        Dim i_Utilizzo As Integer
        Dim i_Codice As Integer
        Dim i_Indirizzo As Integer
        Dim i_Impianti As Integer
        Dim i_Appezzamenti_Storico As Integer

        Dim OpeDB_Appezzamento As String
        Dim OpeDB_Particella As String
        Dim OpeDB_Macrouso As String
        Dim OpeDB_Utilizzo As String
        Dim OpeDB_Codice As String
        Dim OpeDB_Appezzamento_Storico As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Dim wkt As String
        Dim wkt_georiferimento_cod As String

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            '------------------------------

            XmlDoc = New Xml.XmlDocument
            XmlDoc.LoadXml(DatiAppezzamento)

            '------------------------------

            xDatiAppezzamenti = XmlDoc.GetElementsByTagName("DatiAppezzamenti")

            i_DatiAppezzamento = 0

            Do While i_DatiAppezzamento < xDatiAppezzamenti.Count

                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realtà ne esiste uno solo)
                xDatiAppezzamento = xDatiAppezzamenti.Item(i_DatiAppezzamento)

                '------------------------------

                xAppezzamenti = xDatiAppezzamento.GetElementsByTagName("Appezzamento")

                i_Appezzamento = 0

                Do While i_Appezzamento < xAppezzamenti.Count

                    'Prelevo l' i-esimo Appezzamento
                    xAppezzamento = xAppezzamenti.Item(i_Appezzamento)

                    'Prelevo gli attributi dell'appezzamento selezionato
                    OpeDB_Appezzamento = xAppezzamento.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    objAppezzamenti = New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                    objUtentixAppezzamenti = New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
                    objAppezzaxParticelle = New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
                    'Inizializzo Preventivamente il Cod_appezzamento
                    Cod_Appezzamento = CInt(xAppezzamento.GetAttribute("appezza"))

                    'Definizione BaseCode x Esportazione
                    If Not IsNothing(xAppezzamento.GetAttribute("basecode")) AndAlso xAppezzamento.GetAttribute("basecode") <> "" Then
                        BaseCode = xAppezzamento.GetAttribute("basecode")
                    Else
                        BaseCode = 0
                    End If


                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Appezzamento

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xAppezzamento.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xAppezzamento.GetAttribute("sa_cod"))
                            OUTPUT_Appezza = CInt(xAppezzamento.GetAttribute("appezza"))

                        Case "1"    'SALVA -------------------------------------------------------

                            If Cod_Appezzamento <= 0 Then

                                'Nota Cod_Appezzamento = 0 -> Nuovo Appezzamento in remoto/locale
                                '     Cod_Appezzamento < 0 -> Importazione in remoto da versione StandAlone

                                objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                Cod_Appezzamento = objSequenze.NuovoId_Appezzamento(CStr(xAppezzamento.GetAttribute("piva")),
                                                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                                    CInt(xAppezzamento.GetAttribute("basecode")),
                                                                                    CInt(xAppezzamento.GetAttribute("topcode")),
                                                                                    objParametri)

                                objSequenze = Nothing

                                OUTPUT_Piva = CStr(xAppezzamento.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xAppezzamento.GetAttribute("sa_cod"))
                                OUTPUT_Appezza = Cod_Appezzamento

                            Else

                                'Esportazione in Locale dell'Appezzamento

                                OUTPUT_Piva = CStr(xAppezzamento.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xAppezzamento.GetAttribute("sa_cod"))
                                OUTPUT_Appezza = CInt(xAppezzamento.GetAttribute("appezza"))

                            End If

                            Dim app_nome As String = ""

                            If TipoG2G <> 0 Then
                                'Non devo assolutamente andare a guardare le impostazioni,
                                'ma scrivere il nome dell'appezzamento così come mi è arrivato
                                app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000"))
                            Else

                                Try

                                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim ImpostazioneValore1 As String

                                    ImpostazioneValore1 = objImpost.ImpostazioneValore1_from_ImpostazioneCod(
                                                            enum_Impostazioni_Utenti.UTENTE_NumAppezza_Progr_Modalita,
                                                            objParametri_Utenti,
                                                            1)

                                    Select Case ImpostazioneValore1
                                        Case "1"
                                            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                            Dim Numero_Appezzamenti As Integer
                                            Numero_Appezzamenti = objAppezza.Numero_Appezzamenti(CStr(xAppezzamento.GetAttribute("piva")), 0, "", objParametri)
                                            Numero_Appezzamenti += 1
                                            app_nome = "App. " & Format(Numero_Appezzamenti, "000")
                                        Case Else

                                            If (ImpostazioneValore1 = "2" OrElse ImpostazioneValore1 = "3") AndAlso Veg_Cod <> 0 Then

                                                Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                                Dim Numero_Appezzamenti As Integer

                                                Dim sacod As Integer = 0
                                                If ImpostazioneValore1 = "2" Then
                                                    sacod = 0
                                                Else
                                                    sacod = OUTPUT_Sa_Cod
                                                End If

                                                Numero_Appezzamenti = objAppezza.Numero_Appezzamenti_X_Specie(CStr(xAppezzamento.GetAttribute("piva")), sacod, Veg_Cod, "", objParametri)
                                                Numero_Appezzamenti += 1
                                                If Veg_Des = "" Then
                                                    Veg_Des = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R().VegDes_from_VegCod(Veg_Cod, objParametri)
                                                End If
                                                app_nome = Veg_Des & " " & Format(Numero_Appezzamenti, "000")

                                            Else

                                                app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000"))

                                            End If

                                    End Select

                                Catch ex As Exception
                                    app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000"))
                                End Try

                            End If


                            Dummy = objAppezzamenti.Scrivi(CStr(xAppezzamento.GetAttribute("piva")),
                                                           CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                           Cod_Appezzamento,
                                                           CDbl(xAppezzamento.GetAttribute("sup_app")),
                                                           Agro_XML_GetDate(xAppezzamento, "data_app", AGRODATAINIZIO),
                                                           Agro_XML_GetDate(xAppezzamento, "ep_camp", AGRODATAINIZIO),
                                                           Agro_XML_GetDecimal(xAppezzamento, "x", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "y", 0),
                                                           CDbl(xAppezzamento.GetAttribute("zslm")),
                                                           CStr(xAppezzamento.GetAttribute("esposiz")),
                                                           Agro_XML_GetDecimal(xAppezzamento, "pende", 0),
                                                           CStr(xAppezzamento.GetAttribute("ubicazione")),
                                                           Agro_XML_GetInteger(xAppezzamento, "num_del", 0),
                                                           Agro_XML_GetString(xAppezzamento, "clas", ""),
                                                           CDbl(xAppezzamento.GetAttribute("sabbia")),
                                                           CDbl(xAppezzamento.GetAttribute("limo")),
                                                           CDbl(xAppezzamento.GetAttribute("argilla")),
                                                           CDbl(xAppezzamento.GetAttribute("ph")),
                                                           Agro_XML_GetDecimal(xAppezzamento, "caltot", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "calatt", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "sostorg", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "k2oass", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "p2o5ass", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "mg", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "ntot", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "um_s", 0),
                                                           Agro_XML_GetString(xAppezzamento, "cl_dren", ""),
                                                           Agro_XML_GetInteger(xAppezzamento, "falda", 0),
                                                           Agro_XML_GetDecimal(xAppezzamento, "csc", 0),
                                                           Agro_XML_GetDate(xAppezzamento, "k2oass_data", AGRODATAINIZIO),
                                                           Agro_XML_GetDecimal(xAppezzamento, "matorg", 0),
                                                           Agro_XML_GetDate(xAppezzamento, "matorg_data", AGRODATAINIZIO),
                                                           Agro_XML_GetDecimal(xAppezzamento, "notot", 0),
                                                           Agro_XML_GetDate(xAppezzamento, "notot_data", AGRODATAINIZIO),
                                                           Agro_XML_GetDate(xAppezzamento, "p2o5ass_data", AGRODATAINIZIO),
                                                           Agro_XML_GetString(xAppezzamento, "suolo_codattri", ""),
                                                           CStr(objParametri.PivaSuperUser),
                                                           app_nome,
                                                           CInt(xAppezzamento.GetAttribute("campo_spia")),
                                                           CDbl(xAppezzamento.GetAttribute("campo_spia_area")),
                                                           CStr(xAppezzamento.GetAttribute("cs_sipi")),
                                                           CInt(xAppezzamento.GetAttribute("campo_cod")),
                                                           Agro_XML_GetDate(xAppezzamento, "data_inizio", AGRODATAINIZIO),
                                                           Agro_XML_GetDate(xAppezzamento, "data_fine", AGRODATAFINE),
                                                           CInt(xAppezzamento.GetAttribute("prossimo")),
                                                           CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                           CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                           objParametri,
                                                           Data_creazione:=Agro_XML_GetDate(xAppezzamento, "data_creazione", #2/1/1900#),
                                                           Data_modifica:=Agro_XML_GetDate(xAppezzamento, "data_modifica", #2/1/1900#),
                                                           username_creazione:=Agro_XML_GetString(xAppezzamento, "username_creazione", ""),
                                                           username_modifica:=Agro_XML_GetString(xAppezzamento, "username_modifica", ""),
                                                           blk_flag:=Agro_XML_GetInteger(xAppezzamento, "blk_flag", 0),
                                                           blk_inizio_data:=Agro_XML_GetDate(xAppezzamento, "blk_inizio_data", AGRODATAINIZIO),
                                                           blk_inizio_username:=Agro_XML_GetString(xAppezzamento, "blk_inizio_username", ""),
                                                           blk_inizio_note:=Agro_XML_GetString(xAppezzamento, "blk_inizio_note", ""),
                                                           blk_fine_data:=Agro_XML_GetDate(xAppezzamento, "blk_fine_data", AGRODATAFINE),
                                                           blk_fine_username:=Agro_XML_GetString(xAppezzamento, "blk_fine_username", ""),
                                                           blk_fine_note:=Agro_XML_GetString(xAppezzamento, "blk_fine_note", ""),
                                                           via_stringa:=CStr(xAppezzamento.GetAttribute("via_stringa")),
                                                           BZ_CorpiIdrici:=Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_CorpiIdrici"), 0),
                                                           BZ_AreeResPub:=Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_AreeResPub"), 0),
                                                           BZ_Allevamenti:=Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_Allevamenti"), 0),
                                                           BZ_VegNatNonColt:=Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_VegNatNonColt"), 0),
                                                           BZ_SupRiduzione:=Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_SupRiduzione"), 0)
                                                           )


                            Dummy = objUtentixAppezzamenti.Scrivi(CStr(objParametri.PivaSuperUser),
                                                                  CStr(xAppezzamento.GetAttribute("piva")),
                                                                  CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                  Cod_Appezzamento,
                                                                  CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                                  CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                                  objParametri,
                                                                  Data_creazione:=Agro_XML_GetDate(xAppezzamento, "data_creazione", #2/1/1900#),
                                                                  Data_modifica:=Agro_XML_GetDate(xAppezzamento, "data_modifica", #2/1/1900#),
                                                                  username_creazione:=Agro_XML_GetString(xAppezzamento, "username_creazione", ""),
                                                                  username_modifica:=Agro_XML_GetString(xAppezzamento, "username_modifica", "")
                                                                  )


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************
                            objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Appezzamento),
                                                           enum_TipoEntita_Des.Appezza,
                                                           CStr(xAppezzamento.GetAttribute("piva")),
                                                           CStr(xAppezzamento.GetAttribute("sa_cod")),
                                                           CStr(xAppezzamento.GetAttribute("appezza")),
                                                           Nothing, Nothing, Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri)
                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************


                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xAppezzamento.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xAppezzamento.GetAttribute("sa_cod"))
                            OUTPUT_Appezza = CInt(xAppezzamento.GetAttribute("appezza"))


                            Dim app_nome As String = ""

                            If TipoG2G <> 0 Then
                                'Non devo assolutamente andare a guardare le impostazioni,
                                'ma scrivere il nome dell'appezzamento così come mi è arrivato
                                app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000"))
                            Else

                                Try

                                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim ImpostazioneValore1 As String
                                    Dim app_nome_default As String = "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000")

                                    ImpostazioneValore1 = objImpost.ImpostazioneValore1_from_ImpostazioneCod(
                                                            enum_Impostazioni_Utenti.UTENTE_NumAppezza_Progr_Modalita,
                                                            objParametri_Utenti,
                                                            1)

                                    Select Case ImpostazioneValore1
                                        Case "1"
                                            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                            Dim Numero_Appezzamenti As Integer
                                            Numero_Appezzamenti = objAppezza.Numero_Appezzamenti(CStr(xAppezzamento.GetAttribute("piva")), 0, "", objParametri)
                                            Numero_Appezzamenti += 1
                                            app_nome_default = "App. " & Format(Numero_Appezzamenti, "000")
                                        Case "2"

                                            If (ImpostazioneValore1 = "2" OrElse ImpostazioneValore1 = "3") AndAlso Veg_Cod <> 0 Then

                                                Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                                Dim Numero_Appezzamenti As Integer
                                                Dim sacod As Integer = 0
                                                If ImpostazioneValore1 = "2" Then
                                                    sacod = 0
                                                Else
                                                    sacod = OUTPUT_Sa_Cod
                                                End If

                                                Numero_Appezzamenti = objAppezza.Numero_Appezzamenti_X_Specie(CStr(xAppezzamento.GetAttribute("piva")), sacod, Veg_Cod, "", objParametri)
                                                Numero_Appezzamenti += 1
                                                If Veg_Des = "" Then
                                                    Veg_Des = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R().VegDes_from_VegCod(Veg_Cod, objParametri)
                                                End If
                                                app_nome = Veg_Des & " " & Format(Numero_Appezzamenti, "000")

                                            End If
                                    End Select

                                    app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", app_nome_default)

                                Catch ex As Exception
                                    app_nome = Agro_XML_GetString_NoVuota(xAppezzamento, "app_nome", "App. " & Format((Cod_Appezzamento - BaseCode) Mod 100, "000"))
                                End Try

                            End If

                            Dummy = objAppezzamenti.Modifica(CStr(xAppezzamento.GetAttribute("piva")),
                                                             CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                             CInt(xAppezzamento.GetAttribute("appezza")),
                                                             CDbl(xAppezzamento.GetAttribute("sup_app")),
                                                             Agro_XML_GetDate(xAppezzamento, "data_app", AGRODATAINIZIO),
                                                             Agro_XML_GetDate(xAppezzamento, "ep_camp", AGRODATAINIZIO),
                                                             Agro_XML_GetDecimal(xAppezzamento, "x", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "y", 0),
                                                             CDbl(xAppezzamento.GetAttribute("zslm")),
                                                             CStr(xAppezzamento.GetAttribute("esposiz")),
                                                             Agro_XML_GetDecimal(xAppezzamento, "pende", 0),
                                                             CStr(xAppezzamento.GetAttribute("ubicazione")),
                                                             Agro_XML_GetInteger(xAppezzamento, "num_del", 0),
                                                             Agro_XML_GetString(xAppezzamento, "clas", ""),
                                                             CDbl(xAppezzamento.GetAttribute("sabbia")),
                                                             CDbl(xAppezzamento.GetAttribute("limo")),
                                                             CDbl(xAppezzamento.GetAttribute("argilla")),
                                                             CDbl(xAppezzamento.GetAttribute("ph")),
                                                             Agro_XML_GetDecimal(xAppezzamento, "caltot", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "calatt", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "sostorg", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "k2oass", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "p2o5ass", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "mg", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "ntot", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "um_s", 0),
                                                             Agro_XML_GetString(xAppezzamento, "cl_dren", ""),
                                                             Agro_XML_GetInteger(xAppezzamento, "falda", 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, "csc", 0),
                                                             Agro_XML_GetDate(xAppezzamento, "k2oass_data", AGRODATAINIZIO),
                                                             Agro_XML_GetDecimal(xAppezzamento, "matorg", 0),
                                                             Agro_XML_GetDate(xAppezzamento, "matorg_data", AGRODATAINIZIO),
                                                             Agro_XML_GetDecimal(xAppezzamento, "notot", 0),
                                                             Agro_XML_GetDate(xAppezzamento, "notot_data", AGRODATAINIZIO),
                                                             Agro_XML_GetDate(xAppezzamento, "p2o5ass_data", AGRODATAINIZIO),
                                                             Agro_XML_GetString(xAppezzamento, "suolo_codattri", ""),
                                                             app_nome,
                                                             CInt(xAppezzamento.GetAttribute("campo_spia")),
                                                             CDbl(xAppezzamento.GetAttribute("campo_spia_area")),
                                                             CStr(xAppezzamento.GetAttribute("cs_sipi")),
                                                             CInt(xAppezzamento.GetAttribute("campo_cod")),
                                                             Agro_XML_GetDate(xAppezzamento, "data_inizio", AGRODATAINIZIO),
                                                             Agro_XML_GetDate(xAppezzamento, "data_fine", AGRODATAFINE),
                                                             CInt(xAppezzamento.GetAttribute("prossimo")),
                                                             Agro_XML_GetString(xAppezzamento, "via_stringa", ""),
                                                             CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                             CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                             "",
                                                             objParametri,
                                                             Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_CorpiIdrici"), 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_AreeResPub"), 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_Allevamenti"), 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_VegNatNonColt"), 0),
                                                             Agro_XML_GetDecimal(xAppezzamento, LCase("BZ_SupRiduzione"), 0)
                                                             )


                            'Aggiorno le Validità inizio e fine

                            objUtentixAppezzamenti.AggiornaValiditaInizio(CStr(xAppezzamento.GetAttribute("piva")),
                                                                          CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                          CInt(xAppezzamento.GetAttribute("campo_cod")),
                                                                          CInt(xAppezzamento.GetAttribute("appezza")),
                                                                          CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                                          "",
                                                                          objParametri)

                            objUtentixAppezzamenti.AggiornaValiditaFine(CStr(xAppezzamento.GetAttribute("piva")),
                                                                        CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                        CInt(xAppezzamento.GetAttribute("campo_cod")),
                                                                        CInt(xAppezzamento.GetAttribute("appezza")),
                                                                        CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                                        "",
                                                                        objParametri)


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************
                            objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Appezzamento),
                                                           enum_TipoEntita_Des.Appezza,
                                                           CStr(xAppezzamento.GetAttribute("piva")),
                                                           CStr(xAppezzamento.GetAttribute("sa_cod")),
                                                           CStr(xAppezzamento.GetAttribute("appezza")),
                                                           Nothing, Nothing, Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                    End Select

                    '-------------------------------------------------------------
                    ' ENTITA GRAFICHE
                    '-------------------------------------------------------------
                    xEntitaGrafiche = xAppezzamento.GetElementsByTagName("DatiEntita")
                    If xEntitaGrafiche.Count = 1 Then
                        objGrafica = New AgronicaCoreGraficaBIZ.Grafica_Write
                        objGrafica.Grafica_Scrivi2005(xEntitaGrafiche.Item(0).OuterXml,
                                                      CStr(xAppezzamento.GetAttribute("piva")),
                                                      CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                      Cod_Appezzamento,
                                                      True,
                                                      objParametri)
                    End If

                    '-------------------------------------------------------------
                    ' ENTITA IMPIANTI
                    '-------------------------------------------------------------
                    xDatiImpianti = xAppezzamento.GetElementsByTagName("DatiReg_Impianti")

                    'controllo che ci sia il TAG DatiReg_Impianti

                    If xDatiImpianti.Count = 1 Then

                        'Leggo il nodo DatiReg_Impianti
                        xDatoImpianti = xDatiImpianti.Item(0)

                        'Cerco gli impianti
                        xImpianti = xDatoImpianti.GetElementsByTagName("Reg_Impianto")

                        'Modifico l'attributo appezza con il nuovo codice appezza
                        For i_Impianti = 0 To xImpianti.Count - 1

                            xImpianto = xImpianti.Item(i_Impianti)

                            xImpianto.SetAttribute("appezza", Cod_Appezzamento)

                        Next

                        'Scrivo gli impianti
                        objReg_ImpiantiScrivi = New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

                        objReg_ImpiantiScrivi.Reg_Impianto_Scrivi(xDatoImpianti.OuterXml,
                                                                  Nothing,
                                                                  Nothing,
                                                                  Nothing,
                                                                  Nothing,
                                                                  "",
                                                                  objParametri)

                        objReg_ImpiantiScrivi = Nothing

                    End If


                    '-------------------------------------------------------------
                    ' APPEZZAMENTO STORICO
                    '-------------------------------------------------------------

                    xDatiAppezzamenti_Storico = xAppezzamento.GetElementsByTagName("DatiAppezzamenti_Storico")

                    'Controllo che ci sia il TAG DatiAppezzamenti_Storico

                    If xDatiAppezzamenti_Storico.Count = 1 Then

                        ObjAppezzamento_Storico = New AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W

                        'Leggo il nodo DatiReg_Impianti
                        xDatiAppezzamento_Storico = xDatiAppezzamenti_Storico.Item(0)

                        'Cerco i padri dell'appezzamento
                        xAppezzamenti_Storico = xDatiAppezzamento_Storico.GetElementsByTagName("Appezzamento_Storico_Padre")

                        'Modifico l'attributo appezza con il nuovo codice appezza
                        For i_Appezzamenti_Storico = 0 To xAppezzamenti_Storico.Count - 1

                            xAppezzamento_Storico = xAppezzamenti_Storico.Item(i_Appezzamenti_Storico)

                            'Prelevo gli attributi dell'appezzamento selezionato
                            OpeDB_Appezzamento_Storico = xAppezzamento_Storico.GetAttribute("TipoOperazioneDB")

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Appezzamento

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = ObjAppezzamento_Storico.Scrivi(CStr(xAppezzamento_Storico.GetAttribute("padre_piva")),
                                                                           CInt(xAppezzamento_Storico.GetAttribute("padre_sa_cod")),
                                                                           CInt(xAppezzamento_Storico.GetAttribute("padre_appezza")),
                                                                           CStr(xAppezzamento_Storico.GetAttribute("figlio_piva")),
                                                                           CInt(xAppezzamento_Storico.GetAttribute("figlio_sa_cod")),
                                                                           Cod_Appezzamento,
                                                                           CDbl(xAppezzamento_Storico.GetAttribute("trasferimento_superficie")),
                                                                           CDate(xAppezzamento_Storico.GetAttribute("trasferimento_data")),
                                                                           CDate(xAppezzamento_Storico.GetAttribute("validita_inizio")),
                                                                           CDate(xAppezzamento_Storico.GetAttribute("validita_fine")),
                                                                           objParametri,
                                                                           Data_creazione:=Agro_XML_GetDate(xAppezzamento_Storico, "data_creazione", #2/1/1900#),
                                                                           Data_modifica:=Agro_XML_GetDate(xAppezzamento_Storico, "data_modifica", #2/1/1900#),
                                                                           username_creazione:=Agro_XML_GetString(xAppezzamento_Storico, "username_creazione", ""),
                                                                           username_modifica:=Agro_XML_GetString(xAppezzamento_Storico, "username_modifica", "")
                                                                           )

                                Case "2"    'MODIFICA ----------------------------------------------------

                                    ObjAppezzamento_Storico.Modifica(CStr(xAppezzamento_Storico.GetAttribute("padre_piva")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("padre_sa_cod")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("padre_appezza")),
                                                                     CStr(xAppezzamento_Storico.GetAttribute("figlio_piva")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("figlio_sa_cod")),
                                                                     Cod_Appezzamento,
                                                                     CDbl(xAppezzamento_Storico.GetAttribute("trasferimento_superficie")),
                                                                     CDate(xAppezzamento_Storico.GetAttribute("trasferimento_data")),
                                                                     CDate(xAppezzamento_Storico.GetAttribute("validita_inizio")),
                                                                     CDate(xAppezzamento_Storico.GetAttribute("validita_fine")),
                                                                     "",
                                                                     objParametri)

                                Case "3"    'CANCELLA ----------------------------------------------------

                                    ObjAppezzamento_Storico.Cancella(CStr(xAppezzamento_Storico.GetAttribute("padre_piva")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("padre_sa_cod")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("padre_appezza")),
                                                                     CStr(xAppezzamento_Storico.GetAttribute("figlio_piva")),
                                                                     CInt(xAppezzamento_Storico.GetAttribute("figlio_sa_cod")),
                                                                     Cod_Appezzamento,
                                                                     "",
                                                                     objParametri)

                            End Select

                        Next i_Appezzamenti_Storico

                        ObjAppezzamento_Storico = Nothing

                    End If



                    '-------------------------------------------------------------
                    ' PARTICELLE CATASTALI
                    '-------------------------------------------------------------

                    'Prelevo l'elenco delle particelle soggette ad operazioni significative
                    xParticelle = xAppezzamento.GetElementsByTagName("Particella")

                    i_Particella = 0

                    Do While i_Particella < xParticelle.Count

                        'Prelevo l'i-esima Particella Catastale associate all'appezzamento
                        xParticella = xParticelle.Item(i_Particella)

                        'Prelevo gli attributi della particella selezionata
                        OpeDB_Particella = xParticella.GetAttribute("TipoOperazioneDB")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Particella

                            Case "1"    'SALVA -------------------------------------------------------

                                Dummy = objAppezzaxParticelle.Scrivi(
                                            CStr(xParticella.GetAttribute("piva")),
                                            CInt(xParticella.GetAttribute("sa_cod")),
                                            Cod_Appezzamento,
                                            CStr(xParticella.GetAttribute("prov")),
                                            CStr(xParticella.GetAttribute("com")),
                                            CStr(xParticella.GetAttribute("sezione")),
                                            IIf(CStr(xParticella.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("foglio"), False)), 0),
                                            IIf(CStr(xParticella.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("numero"), False)), 0),
                                            CStr(xParticella.GetAttribute("subalterno")),
                                            CDbl(xParticella.GetAttribute("area")),
                                            CDbl(xParticella.GetAttribute("sau_convenz_ettari")),
                                            CInt(xParticella.GetAttribute("sau_convenz_are")),
                                            CInt(xParticella.GetAttribute("sau_convenz_centiare")),
                                            CDbl(xParticella.GetAttribute("sau_convers_ettari")),
                                            CInt(xParticella.GetAttribute("sau_convers_are")),
                                            CInt(xParticella.GetAttribute("sau_convers_centiare")),
                                            CDbl(xParticella.GetAttribute("sau_bio_ettari")),
                                            CInt(xParticella.GetAttribute("sau_bio_are")),
                                            CInt(xParticella.GetAttribute("sau_bio_centiare")),
                                            CDate(xParticella.GetAttribute("validita_inizio")),
                                            CDate(xParticella.GetAttribute("validita_fine")),
                                            objParametri,
                                            Data_creazione:=Agro_XML_GetDate(xParticella, "data_creazione", #2/1/1900#),
                                            Data_modifica:=Agro_XML_GetDate(xParticella, "data_modifica", #2/1/1900#),
                                            username_creazione:=Agro_XML_GetString(xParticella, "username_creazione", ""),
                                            username_modifica:=Agro_XML_GetString(xParticella, "username_modifica", "")
                                            )

                            Case "2", "0"    'LETTURA MODIFICA -------------------------------------------------------
                                'ATTENZIONE: Occorre mantenere sempre le validità delle particelle associate pari a
                                'quelle dell'appezzamento, pertanto la lettura equivale ad una modifica!.

                                '06/03/2020 maga: per il G2G era stata sostituita (il 19/06/19 da Carlo) la Modifica con la Aggiungi_Aggiorna
                                'ma sul sincro harvard veniva generato errore di chiave duplicata perché la query di lettura dentro all'Aggiungi_Aggiorna non trovava il record 
                                '(colpa del paletto "storico" su sezione e subalterno che dovevano essere salvati ="0" quando erano ="") e faceva la Scrivi anziché la Modifica
                                '-> quindi introdotto controllo sul TipoG2G:
                                'TipoG2G = 0 -> chiama la Modifica come ha sempre fatto
                                'TipoG2G <> 0 -> chiama la Aggiungi_Aggiorna 

                                If TipoG2G = 0 Then
                                    objAppezzaxParticelle.Modifica(
                                            CStr(xParticella.GetAttribute("piva")),
                                            CInt(xParticella.GetAttribute("sa_cod")),
                                            Cod_Appezzamento,
                                            CStr(xParticella.GetAttribute("prov")),
                                            CStr(xParticella.GetAttribute("com")),
                                            CStr(xParticella.GetAttribute("sezione")),
                                            IIf(CStr(xParticella.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("foglio"), False)), 0),
                                            IIf(CStr(xParticella.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("numero"), False)), 0),
                                            CStr(xParticella.GetAttribute("subalterno")),
                                            CDbl(xParticella.GetAttribute("area")),
                                            CDbl(xParticella.GetAttribute("sau_convenz_ettari")),
                                            CInt(xParticella.GetAttribute("sau_convenz_are")),
                                            CInt(xParticella.GetAttribute("sau_convenz_centiare")),
                                            CDbl(xParticella.GetAttribute("sau_convers_ettari")),
                                            CInt(xParticella.GetAttribute("sau_convers_are")),
                                            CInt(xParticella.GetAttribute("sau_convers_centiare")),
                                            CDbl(xParticella.GetAttribute("sau_bio_ettari")),
                                            CInt(xParticella.GetAttribute("sau_bio_are")),
                                            CInt(xParticella.GetAttribute("sau_bio_centiare")),
                                            CDate(xParticella.GetAttribute("validita_inizio")),
                                            CDate(xParticella.GetAttribute("validita_fine")),
                                            "",
                                            objParametri)
                                Else
                                    objAppezzaxParticelle.Aggiungi_Aggiorna(
                                      CStr(xParticella.GetAttribute("piva")),
                                      CInt(xParticella.GetAttribute("sa_cod")),
                                      Cod_Appezzamento,
                                      CStr(xParticella.GetAttribute("prov")),
                                      CStr(xParticella.GetAttribute("com")),
                                      CStr(xParticella.GetAttribute("sezione")),
                                      If(CStr(xParticella.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("foglio"), False)), 0),
                                      If(CStr(xParticella.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("numero"), False)), 0),
                                      CStr(xParticella.GetAttribute("subalterno")),
                                      CDbl(xParticella.GetAttribute("area")),
                                      CDbl(xParticella.GetAttribute("sau_convenz_ettari")),
                                      CInt(xParticella.GetAttribute("sau_convenz_are")),
                                      CInt(xParticella.GetAttribute("sau_convenz_centiare")),
                                      CDbl(xParticella.GetAttribute("sau_convers_ettari")),
                                      CInt(xParticella.GetAttribute("sau_convers_are")),
                                      CInt(xParticella.GetAttribute("sau_convers_centiare")),
                                      CDbl(xParticella.GetAttribute("sau_bio_ettari")),
                                      CInt(xParticella.GetAttribute("sau_bio_are")),
                                      CInt(xParticella.GetAttribute("sau_bio_centiare")),
                                      CDate(xParticella.GetAttribute("validita_inizio")),
                                      CDate(xParticella.GetAttribute("validita_fine")),
                                      objParametri)
                                End If

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objAppezzaxParticelle.Cancella(
                                           CStr(xParticella.GetAttribute("piva")),
                                           CInt(xParticella.GetAttribute("sa_cod")),
                                           CInt(xParticella.GetAttribute("appezza")),
                                           CStr(xParticella.GetAttribute("prov")),
                                           CStr(xParticella.GetAttribute("com")),
                                           CStr(xParticella.GetAttribute("sezione")),
                                           If(CStr(xParticella.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("foglio"), False)), 0),
                                           If(CStr(xParticella.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xParticella.GetAttribute("numero"), False)), 0),
                                           CStr(xParticella.GetAttribute("subalterno")),
                                           "",
                                           objParametri)


                        End Select

                        '-------------------------------------------------------------
                        ' MACROUSI
                        '-------------------------------------------------------------

                        'Prelevo l'elenco dei macrousi 
                        xMacrousi = xParticella.GetElementsByTagName("Macrouso")

                        i_Macrouso = 0

                        If xMacrousi IsNot Nothing Then

                            objAppezzaxParticellexMacrousi = New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W

                            Do While i_Macrouso < xMacrousi.Count

                                xMacrouso = xMacrousi.Item(i_Macrouso)

                                OpeDB_Macrouso = xMacrouso.GetAttribute("TipoOperazioneDB")

                                'Verifico l'operazione richiesta
                                Select Case OpeDB_Macrouso

                                    Case "1"    'SALVA -------------------------------------------------------

                                        Dummy = objAppezzaxParticellexMacrousi.Scrivi(
                                                    CStr(xMacrouso.GetAttribute("piva")),
                                                    CInt(xMacrouso.GetAttribute("sa_cod")),
                                                    Cod_Appezzamento,
                                                    CStr(xMacrouso.GetAttribute("prov")),
                                                    CStr(xMacrouso.GetAttribute("com")),
                                                    CStr(xMacrouso.GetAttribute("sezione")),
                                                    IIf(CStr(xMacrouso.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("foglio"), False)), 0),
                                                    IIf(CStr(xMacrouso.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("numero"), False)), 0),
                                                    CStr(xMacrouso.GetAttribute("subalterno")),
                                                    CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                    CDbl(xMacrouso.GetAttribute("superficie")),
                                                    CDate(xMacrouso.GetAttribute("validita_inizio")),
                                                    CDate(xMacrouso.GetAttribute("validita_fine")),
                                                    objParametri,
                                                    Data_creazione:=Agro_XML_GetDate(xMacrouso, "data_creazione", #2/1/1900#),
                                                    Data_modifica:=Agro_XML_GetDate(xMacrouso, "data_modifica", #2/1/1900#),
                                                    username_creazione:=Agro_XML_GetString(xMacrouso, "username_creazione", ""),
                                                    username_modifica:=Agro_XML_GetString(xMacrouso, "username_modifica", "")
                                                    )

                                    Case "2", "0"    'LETTURA MODIFICA -------------------------------------------------------
                                        'ATTENZIONE: Occorre mantenere sempre le validità delle particelle associate pari a
                                        'quelle dell'appezzamento, pertanto la lettura equivale ad una modifica!.

                                        Dummy = objAppezzaxParticellexMacrousi.Modifica(
                                                                    CStr(xMacrouso.GetAttribute("piva")),
                                                                    CInt(xMacrouso.GetAttribute("sa_cod")),
                                                                    Cod_Appezzamento,
                                                                    CStr(xMacrouso.GetAttribute("prov")),
                                                                    CStr(xMacrouso.GetAttribute("com")),
                                                                    CStr(xMacrouso.GetAttribute("sezione")),
                                                                    IIf(CStr(xMacrouso.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("foglio"), False)), 0),
                                                                    IIf(CStr(xMacrouso.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("numero"), False)), 0),
                                                                    CStr(xMacrouso.GetAttribute("subalterno")),
                                                                    CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                                    CDbl(xMacrouso.GetAttribute("superficie")),
                                                                    CDate(xMacrouso.GetAttribute("validita_inizio")),
                                                                    CDate(xMacrouso.GetAttribute("validita_fine")),
                                                                    "",
                                                                    objParametri)

                                    Case "3"    'ELIMINA -------------------------------------------------------

                                        objAppezzaxParticellexMacrousi.Cancella(
                                                   CStr(xMacrouso.GetAttribute("piva")),
                                                   CInt(xMacrouso.GetAttribute("sa_cod")),
                                                   CInt(xMacrouso.GetAttribute("appezza")),
                                                   CStr(xMacrouso.GetAttribute("prov")),
                                                   CStr(xMacrouso.GetAttribute("com")),
                                                   CStr(xMacrouso.GetAttribute("sezione")),
                                                   IIf(CStr(xMacrouso.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("foglio"), False)), 0),
                                                   IIf(CStr(xMacrouso.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xMacrouso.GetAttribute("numero"), False)), 0),
                                                   CStr(xMacrouso.GetAttribute("subalterno")),
                                                   CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                   "",
                                                   objParametri)
                                End Select


                                '-------------------------------------------------------------
                                ' UTILIZZI
                                '-------------------------------------------------------------

                                'Prelevo l'elenco dei macrousi 
                                xUtilizzi = xMacrouso.GetElementsByTagName("Utilizzo")

                                i_Utilizzo = 0

                                If xUtilizzi IsNot Nothing Then

                                    objAppezzaxParticellexMacrousixUtilizzi = New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W

                                    Do While i_Utilizzo < xUtilizzi.Count

                                        xUtilizzo = xUtilizzi.Item(i_Utilizzo)

                                        OpeDB_Utilizzo = xUtilizzo.GetAttribute("TipoOperazioneDB")

                                        'Verifico l'operazione richiesta
                                        Select Case OpeDB_Utilizzo

                                            Case "1"    'SALVA -------------------------------------------------------

                                                Dummy = objAppezzaxParticellexMacrousixUtilizzi.Scrivi(
                                                            CStr(xUtilizzo.GetAttribute("piva")),
                                                            CInt(xUtilizzo.GetAttribute("sa_cod")),
                                                            Cod_Appezzamento,
                                                            CStr(xUtilizzo.GetAttribute("prov")),
                                                            CStr(xUtilizzo.GetAttribute("com")),
                                                            CStr(xUtilizzo.GetAttribute("sezione")),
                                                            IIf(CStr(xUtilizzo.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("foglio"), False)), 0),
                                                            IIf(CStr(xUtilizzo.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("numero"), False)), 0),
                                                            CStr(xUtilizzo.GetAttribute("subalterno")),
                                                            CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                            CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                            CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                            CDbl(xUtilizzo.GetAttribute("superficie")),
                                                            CDate(xUtilizzo.GetAttribute("validita_inizio")),
                                                            CDate(xUtilizzo.GetAttribute("validita_fine")),
                                                            objParametri,
                                                            Data_creazione:=Agro_XML_GetDate(xUtilizzo, "data_creazione", #2/1/1900#),
                                                            Data_modifica:=Agro_XML_GetDate(xUtilizzo, "data_modifica", #2/1/1900#),
                                                            username_creazione:=Agro_XML_GetString(xUtilizzo, "username_creazione", ""),
                                                            username_modifica:=Agro_XML_GetString(xUtilizzo, "username_modifica", "")
                                                            )

                                            Case "2", "0"    'LETTURA MODIFICA -------------------------------------------------------
                                                'ATTENZIONE: Occorre mantenere sempre le validità delle particelle associate pari a
                                                'quelle dell'appezzamento, pertanto la lettura equivale ad una modifica!.

                                                Dummy = objAppezzaxParticellexMacrousixUtilizzi.Modifica(
                                                                            CStr(xUtilizzo.GetAttribute("piva")),
                                                                            CInt(xUtilizzo.GetAttribute("sa_cod")),
                                                                            Cod_Appezzamento,
                                                                            CStr(xUtilizzo.GetAttribute("prov")),
                                                                            CStr(xUtilizzo.GetAttribute("com")),
                                                                            CStr(xUtilizzo.GetAttribute("sezione")),
                                                                            If(CStr(xUtilizzo.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("foglio"), False)), 0),
                                                                            If(CStr(xUtilizzo.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("numero"), False)), 0),
                                                                            CStr(xUtilizzo.GetAttribute("subalterno")),
                                                                            CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                                            CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                                            CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                                            CDbl(xUtilizzo.GetAttribute("superficie")),
                                                                            CDate(xUtilizzo.GetAttribute("validita_inizio")),
                                                                            CDate(xUtilizzo.GetAttribute("validita_fine")),
                                                                            "",
                                                                            objParametri)

                                            Case "3"    'ELIMINA -------------------------------------------------------

                                                objAppezzaxParticellexMacrousixUtilizzi.Cancella(
                                                           CStr(xUtilizzo.GetAttribute("piva")),
                                                           CInt(xUtilizzo.GetAttribute("sa_cod")),
                                                           CInt(xUtilizzo.GetAttribute("appezza")),
                                                           CStr(xUtilizzo.GetAttribute("prov")),
                                                           CStr(xUtilizzo.GetAttribute("com")),
                                                           CStr(xUtilizzo.GetAttribute("sezione")),
                                                           If(CStr(xUtilizzo.GetAttribute("foglio")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("foglio"), False)), 0),
                                                           If(CStr(xUtilizzo.GetAttribute("numero")) <> "", Val(Agro_SQL_SaveNum(xUtilizzo.GetAttribute("numero"), False)), 0),
                                                           CStr(xUtilizzo.GetAttribute("subalterno")),
                                                           CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                           CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                           CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                           "",
                                                           objParametri)
                                        End Select

                                        i_Utilizzo += 1

                                    Loop

                                End If

                                i_Macrouso += 1

                            Loop

                        End If

                        'Incremento l'indice
                        i_Particella += 1

                    Loop
                    '-------------------------------------------------------------
                    ' CODICI
                    '-------------------------------------------------------------

                    'il codice alfanumerico è inserito nell'xml appezza
                    Dim codice_alfanumerico As String = ""
                    If xAppezzamento.HasAttribute("codice_alfanumerico") Then
                        If Not IsNothing(xAppezzamento.GetAttribute("codice_alfanumerico")) AndAlso
                           CStr(xAppezzamento.GetAttribute("codice_alfanumerico")) <> "#" Then
                            codice_alfanumerico = CStr(xAppezzamento.GetAttribute("codice_alfanumerico"))
                        End If
                    End If
                    codice_alfanumerico = codice_alfanumerico.Trim
                    If codice_alfanumerico <> "" AndAlso codice_alfanumerico <> "#" Then
                        Dim codimod As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                        Select Case OpeDB_Appezzamento
                            Case "0"
                            Case "1"
                                codimod.aggiorna(CStr(xAppezzamento.GetAttribute("piva")),
                                                 CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                 Cod_Appezzamento,
                                                 enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                 codice_alfanumerico, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                            Case "2"
                                codimod.aggiorna(CStr(xAppezzamento.GetAttribute("piva")),
                                                 CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                 CInt(xAppezzamento.GetAttribute("appezza")),
                                                 enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                 codice_alfanumerico, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                            Case "3"
                                codimod.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                 CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                 CInt(xAppezzamento.GetAttribute("appezza")),
                                                 enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                 "", objParametri)
                        End Select
                    End If

                    objAppezzaxCodici = New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_Appezzamento = 2 Then

                        Dim filtro1104 As String = ""
                        If codice_alfanumerico <> "" AndAlso codice_alfanumerico <> "#" Then
                            filtro1104 = " Id_Cod Not In (1104) "
                        End If

                        objAppezzaxCodici.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                   CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                   CInt(xAppezzamento.GetAttribute("appezza")),
                                                   0, filtro1104, objParametri)
                    End If

                    'Prelevo l'elenco dei codici
                    xCodici = xAppezzamento.GetElementsByTagName("CodiceAppezzamento")

                    i_Codice = 0

                    Do While i_Codice < xCodici.Count

                        'Prelevo l'i-esimo codice
                        xCodice = xCodici.Item(i_Codice)

                        'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                        OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Se entro in modifica e inserisco un nuovo codice ->
                                'l'appezza si trova nella stringa Xml
                                If OpeDB_Appezzamento = 2 Then
                                    Cod_Appezzamento = CInt(xAppezzamento.GetAttribute("appezza"))
                                End If

                                Dummy = objAppezzaxCodici.Scrivi(
                                            CStr(xAppezzamento.GetAttribute("piva")),
                                            CInt(xAppezzamento.GetAttribute("sa_cod")),
                                            Cod_Appezzamento,
                                            CInt(xCodice.GetAttribute("id_cod")),
                                            CStr(xCodice.GetAttribute("val_cod")),
                                            CDate(xCodice.GetAttribute("validita_inizio")),
                                            CDate(xCodice.GetAttribute("validita_fine")),
                                            objParametri,
                                            Data_creazione:=Agro_XML_GetDate(xCodice, "data_creazione", #2/1/1900#),
                                            Data_modifica:=Agro_XML_GetDate(xCodice, "data_modifica", #2/1/1900#),
                                            username_creazione:=Agro_XML_GetString(xCodice, "username_creazione", ""),
                                            username_modifica:=Agro_XML_GetString(xCodice, "username_modifica", "")
                                            )

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objAppezzaxCodici.Modifica(CStr(xAppezzamento.GetAttribute("piva")),
                                                           CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                           CInt(xAppezzamento.GetAttribute("appezza")),
                                                           CInt(xCodice.GetAttribute("id_cod")),
                                                           CStr(xCodice.GetAttribute("val_cod")),
                                                           CDate(xCodice.GetAttribute("validita_inizio")),
                                                           CDate(xCodice.GetAttribute("validita_fine")),
                                                           "",
                                                           objParametri)

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objAppezzaxCodici.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                           CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                           CInt(xAppezzamento.GetAttribute("appezza")),
                                                           CInt(xCodice.GetAttribute("id_cod")),
                                                           "",
                                                           objParametri)

                        End Select

                        'Incremento l'indice
                        i_Codice += 1

                    Loop


                    'Prelevo l'elenco dei codici
                    xIndirizzi = xAppezzamento.GetElementsByTagName("Indirizzo")

                    i_Indirizzo = 0

                    Do While i_Indirizzo < xIndirizzi.Count

                        'Prelevo l'i-esimo codice
                        xIndirizzo = xIndirizzi.Item(i_Indirizzo)

                        'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                        OpeDB_Codice = If(TipoG2G = 0, xIndirizzo.GetAttribute("TipoOperazioneDB"), "1")

                        objIndirizzi = New AgronicaCoreAnagrafeDAL.Indirizzi_Write
                        objAppezzaxIndirizzi = New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write
                        objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Se entro in modifica e inserisco un nuovo codice ->
                                'l'appezza si trova nella stringa Xml
                                If OpeDB_Appezzamento = 2 Then
                                    Cod_Appezzamento = CInt(xAppezzamento.GetAttribute("appezza"))
                                End If

                                Dim Cod_Indirizzo = CInt(xIndirizzo.GetAttribute("cod_indirizzo"))

                                If Cod_Indirizzo = 0 Then

                                    'Richiedo un nuovo codice indirizzo
                                    Cod_Indirizzo = objSequenze.NuovoId_Tabella("Indirizzi",
                                                                                    CInt(xIndirizzo.GetAttribute("basecode")),
                                                                                    CInt(xIndirizzo.GetAttribute("topcode")),
                                                                                    objParametri)

                                End If

                                'Salvo l'indirizzo
                                Dummy = objIndirizzi.Scrivi(Cod_Indirizzo,
                                                                CStr(xIndirizzo.GetAttribute("ind_des")),
                                                                CStr(xIndirizzo.GetAttribute("frz_des")),
                                                                CStr(xIndirizzo.GetAttribute("cap")),
                                                                CStr(xIndirizzo.GetAttribute("com_des")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                                CStr(xIndirizzo.GetAttribute("stato")),
                                                                CStr(xIndirizzo.GetAttribute("note")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                                CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                                CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                                CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                                objParametri)


                                Dummy = objAppezzaxIndirizzi.Scrivi(
                                            CStr(xAppezzamento.GetAttribute("piva")),
                                            CInt(xAppezzamento.GetAttribute("sa_cod")),
                                            Cod_Appezzamento,
                                            Cod_Indirizzo,
                                            CStr(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                            CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                            CDate(xIndirizzo.GetAttribute("validita_fine")),
                                            objParametri,
                                            Data_creazione:=Agro_XML_GetDate(xIndirizzo, "data_creazione", #2/1/1900#),
                                            Data_modifica:=Agro_XML_GetDate(xIndirizzo, "data_modifica", #2/1/1900#),
                                            username_creazione:=Agro_XML_GetString(xIndirizzo, "username_creazione", ""),
                                            username_modifica:=Agro_XML_GetString(xIndirizzo, "username_modifica", "")
                                            )

                            Case "2"    'MODIFICA -------------------------------------------------------

                                Dim Cod_Indirizzo = CInt(xIndirizzo.GetAttribute("cod_indirizzo"))

                                If Cod_Indirizzo = 0 Then

                                    Throw New Exception("Cod_Indirizzo = 0 in modifica")

                                End If

                                'Salvo l'indirizzo
                                Dummy = objIndirizzi.Modifica(Cod_Indirizzo,
                                                                CStr(xIndirizzo.GetAttribute("ind_des")),
                                                                CStr(xIndirizzo.GetAttribute("frz_des")),
                                                                CStr(xIndirizzo.GetAttribute("cap")),
                                                                CStr(xIndirizzo.GetAttribute("com_des")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod")),
                                                                CStr(xIndirizzo.GetAttribute("stato")),
                                                                CStr(xIndirizzo.GetAttribute("note")),
                                                                CStr(xIndirizzo.GetAttribute("pro_cod_istat")),
                                                                CStr(xIndirizzo.GetAttribute("com_cod_istat")),
                                                                CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                                                CDate(xIndirizzo.GetAttribute("validita_fine")),
                                                                "",
                                                                objParametri)


                                Dummy = objAppezzaxIndirizzi.Modifica(
                                            CStr(xAppezzamento.GetAttribute("piva")),
                                            CInt(xAppezzamento.GetAttribute("sa_cod")),
                                            Cod_Appezzamento,
                                            Cod_Indirizzo,
                                            CStr(xIndirizzo.GetAttribute("tipo_indirizzo")),
                                            CDate(xIndirizzo.GetAttribute("validita_inizio")),
                                            CDate(xIndirizzo.GetAttribute("validita_fine")),
                                            "",
                                            objParametri
                                            )

                            Case "3"    'ELIMINA -------------------------------------------------------

                                objIndirizzi.Cancella(xIndirizzo.GetAttribute("cod_indirizzo"), "", objParametri)
                                objAppezzaxIndirizzi.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                              CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                              xAppezzamento.GetAttribute("appezza"),
                                                              xIndirizzo.GetAttribute("cod_indirizzo"),
                                                              "", objParametri)

                        End Select

                        'Incremento l'indice
                        i_Indirizzo += 1

                    Loop


                    ''
                    ''  APPEZZAMENTO GRAFICO
                    ''
                    If xAppezzamento.HasAttribute("wkt") Then
                        If xAppezzamento.GetAttribute("wkt") = "" Then
                            wkt = ""
                            wkt_georiferimento_cod = "-1"
                        Else
                            wkt = xAppezzamento.GetAttribute("wkt")
                            wkt_georiferimento_cod = xAppezzamento.GetAttribute("wkt_georiferimento_cod")
                        End If
                    Else
                        wkt = ""
                        wkt_georiferimento_cod = "-1"
                    End If


                    If Not String.IsNullOrEmpty(wkt) Then


                        AppezzamentoScriviDatoCartografico(CStr(xAppezzamento.GetAttribute("piva")),
                                                           CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                           Cod_Appezzamento,
                                                           wkt,
                                                           wkt_georiferimento_cod,
                                                           ScriviElementiGrafici,
                                                           objParametri)


                    End If
                    ' esiste un wkt ... 


                    Select Case OpeDB_Appezzamento

                        Case "2"

                            '-------------------------------------------------
                            'Modifica delle finestre temporali impianti

                            objReg_Impianti = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
                            objReg_Impianti.AggiornaValiditaInizio(CStr(xAppezzamento.GetAttribute("piva")),
                                                                   CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                   CInt(xAppezzamento.GetAttribute("appezza")),
                                                                   0,
                                                                   0,
                                                                   CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                                   objParametri)
                            objReg_Impianti.AggiornaValiditaFine(CStr(xAppezzamento.GetAttribute("piva")),
                                                                 CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                 CInt(xAppezzamento.GetAttribute("appezza")),
                                                                 0,
                                                                 0,
                                                                 CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                                 objParametri)

                            objReg_Impianti = Nothing


                            '-------------------------------------------------
                            '-------------INIZIO MODIFICHE PER CHIUSURA DISTINTE-----------
                            '-------------------------------------------------

                            objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

                            '-------------------------------------------------
                            'Modifica delle finestre temporali progetti
                            'chiudo la fine del progetto 
                            Dim objimpreseprogr As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                            Dim dt As DataTable = objimpreseprogr.LeggiDistinta(CStr(xAppezzamento.GetAttribute("piva")),
                                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                    CInt(xAppezzamento.GetAttribute("appezza")),
                                                                    0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    "", " Imprese_Progetti.Validita_Inizio desc  ", objParametri)

                            If dt.Rows.Count > 0 Then
                                If Date.Compare(CDate(dt.Rows(0).Item("Validita_Inizio")), CDate(xAppezzamento.GetAttribute("validita_fine"))) > 0 Then

                                    'c'è una distinta aperta in una data successiva a quella di chiusura impianto, genero errore
                                    Throw New Exception("<br><br> ATTENZIONE! <br> C'è una distinta aperta in una data successiva a quella di chiusura impianto, verificare le distinte!")
                                Else
                                    'modifico la data distinta, se non ci sono operazioni successive registrate
                                    ''''''''''''''''''

                                    If Not Date_Modifiche_Impianti_Verificate AndAlso True Then
                                        'devo controllare anche che sia solo una la distinta successiva...
                                        Dim dt2 As DataTable = objimpreseprogr.LeggiDistinta(CStr(xAppezzamento.GetAttribute("piva")),
                                                                        CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                        CInt(xAppezzamento.GetAttribute("appezza")),
                                                                        0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                        " Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(CDate(xAppezzamento.GetAttribute("validita_fine")), False), "", objParametri)
                                        If dt2.Rows.Count > 1 Then
                                            Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono " & dt2.Rows.Count & " distinte aperta in una data successiva a quella di chiusura impianto, verificare le distinte!")
                                        End If
                                    End If

                                    If Not Date_Modifiche_Impianti_Verificate AndAlso True Then
                                        'devo controllare anche che non ci siano distinte precedenti
                                        Dim dt2 As DataTable = objimpreseprogr.LeggiDistinta(CStr(xAppezzamento.GetAttribute("piva")),
                                                                        CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                        CInt(xAppezzamento.GetAttribute("appezza")),
                                                                        0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                        " Imprese_Progetti.Validita_Inizio < " & Agro_SQL_SaveDate(CDate(xAppezzamento.GetAttribute("validita_inizio")), False), "", objParametri)
                                        If dt2.Rows.Count > 0 Then
                                            Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono " & dt2.Rows.Count & " distinte aperta in una data precedente a quella di apertura impianto, verificare le distinte!")
                                        End If
                                    End If


                                    '--------------------------------------------------------------------------------------------------------------------------------------------
                                    'Controllo, se sono già state registrate Operazioni d'Agenda
                                    'che le date delle operazioni nn siano esterne alle date scelte x l'impianto 
                                    '--------------------------------------------------------------------------------------------------------------------------------------------


                                    Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                    Dim DTAgenda As DataTable

                                    'ricavo il recordset dei movimenti di produzione associati all'impianto
                                    DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(CStr(xAppezzamento.GetAttribute("piva")),
                                                                                  CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                                  CInt(xAppezzamento.GetAttribute("appezza")),
                                                                                  0,
                                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                  " Data_Movimento > " & Agro_SQL_SaveDate(CDate(xAppezzamento.GetAttribute("validita_fine")), False) & " ",
                                                                                  "  Data_Movimento desc ",
                                                                                  objParametri)


                                    If DTAgenda.Rows.Count > 0 Then
                                        'ci sono operazioni fatte successivamente alla chiusura
                                        Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono una o più operazioni di agenda create successivamente alla data di chiusura impianto, verificare le operazioni sull'impianto!")
                                    End If


                                    ''''''''''''''''''

                                    Dim objimpreseprog As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W

                                    objimpreseprog.AggiornaValiditaFine(CStr(xAppezzamento.GetAttribute("piva")),
                                                                        CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                        CInt(xAppezzamento.GetAttribute("appezza")),
                                                                        dt.Rows(0).Item("id_reg"),
                                                                        dt.Rows(0).Item("progetto_cod"),
                                                                        0,
                                                                        CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                                         "", objParametri)

                                End If
                            End If



                            objParametri.ResettaFinestra()

                            '-------------------------------------------------
                            '-------------FINE MODIFICHE---------------
                            '-------------------------------------------------
                            '-------------------------------------------------
                            'Modifica delle finestre temporali daltri figli

                            'Aggiorno le date della grafica
                            GraphicKey = "A" & Right(New String("0", 8) & Hex(CInt(xAppezzamento.GetAttribute("appezza"))), 8)
                            objGrafica_AD = New AgronicaCoreGraficaDAL.Grafica_Write
                            objGrafica_AD.AggiornaValiditaInizio(CStr(xAppezzamento.GetAttribute("piva")),
                                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                    GraphicKey,
                                                                     CDate(xAppezzamento.GetAttribute("validita_inizio")),
                                                                     "",
                                                                     objParametri)
                            objGrafica_AD.AggiornaValiditaFine(CStr(xAppezzamento.GetAttribute("piva")),
                                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                    GraphicKey,
                                                                     CDate(xAppezzamento.GetAttribute("validita_fine")),
                                                                     "",
                                                                     objParametri)
                            objGrafica_AD = Nothing


                        Case "3" 'CANCELLAZIONE APPEZZAMENTO

                            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

                            'DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
                            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
                            Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
                            Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0,
                                                                                          CStr(xAppezzamento.GetAttribute("piva")),
                                                                                          CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                                          CInt(xAppezzamento.GetAttribute("appezza")),
                                                                                          0, 0, 0, 0, 0, "", "", objParametri)

                            For Each vincolo In dtAnagrafe_Vincoli.Rows
                                If vincolo("ID_Reg") = 0 AndAlso vincolo("Progetto_Cod") = 0 Then
                                    objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
                                End If
                            Next


                            Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
                            Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

                            Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0,
                                                                                         0,
                                                                                         CStr(xAppezzamento.GetAttribute("piva")),
                                                                                         CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                                         CInt(xAppezzamento.GetAttribute("appezza")),
                                                                                         0, 0, "", "", objParametri)

                            For Each pualet In dtPUA_Letamazioni.Rows
                                If pualet("ID_Reg") = 0 AndAlso pualet("Progetto_Cod") = 0 Then
                                    objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
                                End If
                            Next


                            '--------------------------------------------------------------------------------------------------
                            'Cancellazione Poligoni
                            Dim objGISEntita_R As New AgronicaCoreGisDAL.GIS_Entita_R
                            Dim objGISEntita_W As New AgronicaCoreGisDAL.GIS_Entita_W
                            Dim objGISElementiGrafici_R As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
                            Dim objGISElementiGrafici_W As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

                            Dim DT_GISEntita As DataTable
                            Dim DT_GISElementiGrafici As DataTable

                            DT_GISEntita = objGISEntita_R.Leggi("", 0, enum_GIS2012_TipoEntita.APPEZZAMENTI,
                                                  CStr(xAppezzamento.GetAttribute("piva")),
                                                  CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                  CInt(xAppezzamento.GetAttribute("appezza")),
                                                  0, 0,
                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                  "", "", objParametri)

                            For Each entita In DT_GISEntita.Rows
                                DT_GISElementiGrafici = objGISElementiGrafici_R.Leggi(entita("PivaSuperUser"), 0, entita("Entita_Cod"),
                                                                                      0, 0,
                                                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                      "", "", objParametri)

                                For Each elemento In DT_GISElementiGrafici.Rows
                                    objGISElementiGrafici_W.Cancella(entita("PivaSuperUser"), elemento("ElementoGrafico_Cod"), "", objParametri)
                                Next

                                objGISEntita_W.Cancella(entita("PivaSuperUser"), entita("Entita_Cod"), "", objParametri)
                            Next


                            '--------------------------------------------------------------------------------------------------
                            'Cancellazione Sequenze 

                            objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                            objSequenze.CancellaSeqReg_Impianti(CStr(xAppezzamento.GetAttribute("piva")),
                                                                CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                CInt(xAppezzamento.GetAttribute("appezza")),
                                                                 objParametri)

                            objSequenze = Nothing

                            '--------------------------------------------------------------------------------------------------
                            'Cancello i figli Maggiori dell'Appezzamento

                            'Cancello TUTTI gli IMPIANTI
                            objReg_ImpiantiLeggi = New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                            XmlReg_Impianti = objReg_ImpiantiLeggi.Reg_Impianto_Leggi(CStr(xAppezzamento.GetAttribute("piva")),
                                                                CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                                CInt(xAppezzamento.GetAttribute("appezza")),
                                                                0,
                                                                True,
                                                                True,
                                                                objParametri)
                            objReg_ImpiantiLeggi = Nothing
                            If XmlReg_Impianti <> "" Then
                                objReg_ImpiantiScrivi = New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                                objReg_ImpiantiScrivi.Reg_Impianto_Scrivi(XmlReg_Impianti,
                                                                          Nothing,
                                                                          Nothing,
                                                                          Nothing,
                                                                          Nothing,
                                                                          "",
                                                                          objParametri)
                                objReg_ImpiantiScrivi = Nothing
                            End If

                            'Cancella i dati grafici
                            GraphicKey = "A" & Right(New String("0", 8) & Hex(CInt(xAppezzamento.GetAttribute("appezza"))), 8)
                            objGrafica_AD = New AgronicaCoreGraficaDAL.Grafica_Write
                            objGrafica_AD.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                    "",
                                                    GraphicKey, "",
                                                      "",
                                                        objParametri)
                            GraphicKey = "E" & Right(New String("0", 8) & Hex(CInt(xAppezzamento.GetAttribute("appezza"))), 8)
                            objGrafica_AD.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                    CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                    "", GraphicKey, "",
                                                       "",
                                                        objParametri)
                            objGrafica_AD = Nothing

                            '------------------------------------------------------
                            objAppezzaxCodici = New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                            objAppezzaxCodici.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                       CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                       CInt(xAppezzamento.GetAttribute("appezza")),
                                                       0,
                                                       "",
                                                       objParametri)
                            '------------------------------------------------------
                            ObjAppezzamento_Storico = New AgronicaCoreAnagrafeDAL.Appezzamento_Storico_W
                            'Cancellazione delle Storico dell'Appezzamento da Figlio
                            ObjAppezzamento_Storico.Cancella("",
                                                             0,
                                                             0,
                                                             CStr(xAppezzamento.GetAttribute("piva")),
                                                             CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                             CInt(xAppezzamento.GetAttribute("appezza")),
                                                             "",
                                                             objParametri)

                            'Cancellazione delle Storico dell'Appezzamento da Padre
                            ObjAppezzamento_Storico.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                             CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                             CInt(xAppezzamento.GetAttribute("appezza")),
                                                             "",
                                                             0,
                                                             0,
                                                             "",
                                                             objParametri)

                            '------------------------------------------------------

                            objUtentixAppezzamenti.Cancella(CStr(objParametri.PivaSuperUser),
                                                            CStr(xAppezzamento.GetAttribute("piva")),
                                                            CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                            CInt(xAppezzamento.GetAttribute("appezza")),
                                                            "",
                                                            objParametri)

                            objAppezzaxParticelle.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                           CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                           CInt(xAppezzamento.GetAttribute("appezza")),
                                                           "",
                                                           "",
                                                           "",
                                                           0,
                                                           0,
                                                           "",
                                                           "",
                                                           objParametri)

                            objAppezzamenti.Cancella(CStr(xAppezzamento.GetAttribute("piva")),
                                                     CInt(xAppezzamento.GetAttribute("sa_cod")),
                                                     CInt(xAppezzamento.GetAttribute("appezza")),
                                                     "",
                                                     objParametri)

                    End Select

                    'Elimino l'oggetto
                    objAppezzamenti = Nothing
                    objAppezzaxParticelle = Nothing
                    objUtentixAppezzamenti = Nothing
                    objAppezzaxCodici = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Appezzamento += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiAppezzamento += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti Xml utilizzati
            xCodice = Nothing
            xCodici = Nothing
            xAppezzamento = Nothing
            xDatiAppezzamento = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            Appezzamento_Scrivi = Cod_Appezzamento

            'Esco dalla funzione
            'Exit Function

            'Restituisco un valore Dummy
            xRisp = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Sa_Cod=" & CStr(OUTPUT_Sa_Cod) & ")" &
                              "(Appezza=" & CStr(OUTPUT_Appezza) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////


            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    Private Sub AppezzamentoScriviDatoCartografico(Piva As String,
                                                   Sa_Cod As Integer,
                                                   Appezza As Integer,
                                                   wkt As String,
                                                   wkt_georiferimento_cod As String,
                                                   ScriviElementiGrafici As AgronicaCoreGisBIZ.GIS_Entita_W,
                                                   objParametri_Server As AgronicaCoreParametri)

        Dim ParametriCartografici As ParametriCoordinateConverter = Nothing

        If wkt_georiferimento_cod <> "-1" Then


            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            ParametriCartografici = New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }


        End If

        Dim sNodeDoc As XDocument

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim cconverter As New Agronica.CoordinateConverter

        Dim FinalDoc As New XDocument

        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
        FinalDoc.Add(ElemFinalXdoc)

        If ParametriCartografici Is Nothing Then

            Dim DatiCartograficiOriginali_WGS84 As List(Of xyz) =
                    wktHelp.CreaCoordinateDaWkt(wkt, False)

            sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    wktHelp.CreaPoligonoDaCoordinate(
                        DatiCartograficiOriginali_WGS84,
                        (DatiCartograficiOriginali_WGS84.Count = 1)
                    ),
                    True,
                    False,
                    True,
                    0)
            )
            sNodeDoc.Root.@Flag_GPS = 0
        Else

            sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici),
                    False,
                    False,
                    True,
                    0)
            )

            sNodeDoc.Root.@Flag_GPS = 0

        End If

        Dim objGisEntitaR = New AgronicaCoreGisDAL.GIS_Entita_R

        Dim dtEntitaCod = objGisEntitaR.LeggiDB(objParametri_Server.PivaSuperUser, 0, 0, Piva, Sa_Cod, Appezza, 0, 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        'Dim dtEntitaCod = objGisEntitaR.LeggiSmallAppezza(Piva, Sa_Cod, Appezza, 0, "", "", objParametri_Server)
        Dim newEntitaElement As XElement = Nothing
        Dim entitaCod As Integer = 0
        Dim TipoOperazione As Integer = 1

        If dtEntitaCod.Rows.Count > 0 Then
            entitaCod = dtEntitaCod.Rows(0)("Entita_Cod")
            TipoOperazione = 2
            sNodeDoc.Root.@ElementoGrafico_Cod = entitaCod
        End If
        newEntitaElement = <Entita TipoOperazioneDB=<%= TipoOperazione.ToString() %> recno="" deleted="" section="E" id="1" descr="" ecolor="256" eline="256" rad="15" text="" gps="0" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                               <layers>
                                   <layer tipologia_layer="1"><%= CInt(enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI) %></layer>
                               </layers>
                               <EntitaGIAS>
                                   <DatoGias>
                                       <PivaSuperUser><%= objParametri_Server.PivaSuperUser %></PivaSuperUser>
                                       <Entita_Cod><%= entitaCod %></Entita_Cod>
                                       <TipoEntita_Cod><%= CInt(enum_GIS2012_TipoEntita.APPEZZAMENTI) %></TipoEntita_Cod>
                                       <Piva><%= Piva %></Piva>
                                       <Sa_Cod><%= Sa_Cod %></Sa_Cod>
                                       <Appezza><%= Appezza %></Appezza>
                                       <Campo_Cod>0</Campo_Cod>
                                       <Id_Imp>0</Id_Imp>
                                       <PROV>0</PROV>
                                       <COM>0</COM>
                                       <SEZIONE>-1</SEZIONE>
                                       <FOGLIO>-1</FOGLIO>
                                       <NUMERO>-1</NUMERO>
                                       <SUBALTERNO>-1</SUBALTERNO>
                                       <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                       <Programmazione_Cod>0</Programmazione_Cod>
                                       <Id_Agenda>0</Id_Agenda>
                                       <id_mov_det>0</id_mov_det>
                                       <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                       <OLDGrafica_ID></OLDGrafica_ID>
                                       <analisi_campione_cod>0</analisi_campione_cod>
                                       <inviato>0</inviato>
                                       <Data_Creazione><%= GetData_Creazione(Now) %></Data_Creazione>
                                       <Data_Modifica><%= GetData_Creazione(Now) %></Data_Modifica>
                                       <Username_Creazione>agronica</Username_Creazione>
                                       <Username_Modifica>agronica</Username_Modifica>
                                       <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                       <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                   </DatoGias>
                               </EntitaGIAS>
                           </Entita>



        newEntitaElement.Add(sNodeDoc.FirstNode)
        FinalDoc.Root.Add(newEntitaElement)

        Dim ns1 As XNamespace = "http://www.agronica.it/grafica/"

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim sFinalDoc1 As String = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
        Dim FinalDoc1 As XDocument = XDocument.Parse(sFinalDoc1)

        For Each elemento In (
            From a In FinalDoc1.Elements(ns1 + "DatiEntita").Elements(ns1 + "Entita")
            Select a).ToList()



            Try

                Dim idle As Integer
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_Server)


            Catch ex As Exception


                'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

            End Try


        Next

    End Sub

    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function

    Private Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dataCreazione)
    End Function

    ''' <summary>
    ''' Apre un nuovo appezzamento le cui chiavi vengono passate alla funzione
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="OUTPUT_Appezza"></param>    
    ''' <param name="BaseCode"></param>
    ''' <param name="TopCode"></param>
    ''' <param name="Errore"></param>
    ''' <returns></returns>
    Public Function Apertura_Appezzamento(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Campo_Cod As Integer,
                                          ByRef OUTPUT_Appezza As Integer,
                                          ByVal APP_Nome As String,
                                          ByVal SupApp As Decimal,
                                          ByVal BaseCode As Integer,
                                          ByVal TopCode As Integer,
                                          ByRef Errore As String,
                                          ByVal Validita_inizio_App As Date,
                                          ByVal Validita_fine_App As Date,
                                          ByVal objparametri_server As AgronicaCoreParametri,
                                          ByVal objparametri_utenti As AgronicaCoreParametri
                                          ) As Boolean

        Dim DesChiaveImpianto As String = ""
        Dim objXML As New AgronicaCoreXML.XML_Anagrafe
        Dim StringaXmlCreazione As String
        Dim flag_insert = True
        Dim OUT_Piva As String = ""
        Dim XmlDoc As XmlDocument

        Dim data_fine_prec As Date

        'nuovi dati distinta
        Dim data_inizio As Date
        Dim data_fine As Date
        Dim progetto_nome As String
        Dim progetto_des As String


        'credo le nuove date 

        'imposto la data inizio come la data fine della distinta precedente piu un giorno
        data_inizio = DateAdd(DateInterval.Day, 1, data_fine_prec)

        If data_inizio > Validita_fine_App Then
            'errore!!!! non dovrebbe mai finire qui
            Errore &= DesChiaveImpianto & "errore, la nuova data di fine inizio supera la data di fine appezzamento!" & vbCrLf
            Return False
        End If

        'la edit impianto fa: imposto la data fine come la data fine della distinta precedente + 1 anno
        'che è uguale a fare la data inizio + 1 anno - 1 giorno
        data_fine = DateAdd(DateInterval.Year, 1, data_fine_prec)

        If data_fine > Validita_fine_App Then
            'la data fine della distinta non può superare la data fine dell'impianto
            'quindi la imposto uguale alla data fine impianto
            data_fine = Validita_fine_App
        End If


        'lotto
        If data_inizio.Year <> data_fine.Year Then
            progetto_nome = "Lotto " & CStr(data_inizio.Year) & "/" & CStr(data_fine.Year)
        Else
            progetto_nome = "Lotto " & CStr(data_inizio.Year)
        End If

        progetto_des = progetto_nome

        XmlDoc = New XmlDocument

        Dim StrAppezzamento As String

        'Creo il nodo "DatiReg_Impianti"
        Dim XmlDatiReg_Appezzamenti As XmlElement
        XmlDatiReg_Appezzamenti = XmlDoc.CreateElement("DatiAppezzamenti")



        StrAppezzamento = objXML.XML_Appezzamento(
            TipoOperazioneDB:=enum_TipoOperazioneDB.Scrittura,
            Piva:=Piva,
            Sa_Cod:=Sa_Cod,
            Appezza:=0,
            Sup_App:=SupApp,
            Data_App:=AGRODATAINIZIO.ToShortDateString,
            Ep_Camp:=AGRODATAINIZIO.ToShortDateString,
            X:=0,
            Y:=0,
            Zslm:=0,
            Esposiz:=".",
            Pende:=0,
            Ubicazione:=".",
            Num_Del:=0,
            Clas:="",
            Sabbia:=0,
            Limo:=0,
            Argilla:=0,
            pH:=0,
            CalTot:=0,
            CalAtt:=0,
            SostOrg:=0,
            K2OAss:=0,
            P2O5Ass:=0,
            Mg:=0,
            Ntot:=0,
            Um_S:=0,
            Cl_Dren:="0",
            Falda:=0,
            CsC:=0,
            K2OAss_Data:=AGRODATAINIZIO.ToShortDateString,
            MatOrg:=0,
            MatOrg_Data:=AGRODATAINIZIO.ToShortDateString,
            NOtot_Data:=AGRODATAINIZIO.ToShortDateString,
            NOtot:=0,
            P2O5Ass_Data:=AGRODATAINIZIO.ToShortDateString,
            Suolo_CodAttri:="",
            App_Nome:=APP_Nome,
            Campo_Spia:=0,
            Campo_Spia_Area:=0,
            Cs_SIPI:="",
            Campo_Cod:=Campo_Cod,
            Prossimo:=0,
            Data_Inizio:=Validita_inizio_App.ToShortDateString,
            Data_Fine:=Validita_fine_App.ToShortDateString,
            Validita_Inizio:=Validita_inizio_App,
            Validita_Fine:=Validita_fine_App,
            BaseCode:=BaseCode,
            TopCode:=TopCode
        )

        XmlDatiReg_Appezzamenti.InnerXml = StrAppezzamento

        If Errore <> "" Then
            Return False
        End If

        StringaXmlCreazione = XmlDatiReg_Appezzamenti.OuterXml()

        If StringaXmlCreazione <> "" Then


            Try

                '-----------------------------------------------------
                '--------------- SCRITTURA  --------------------

                flag_insert = Appezzamento_Scrivi(StringaXmlCreazione,
                                                        OUT_Piva, 0, OUTPUT_Appezza,
                                                         objparametri_server, objparametri_utenti)
                '-----------------------------------------------------

            Catch ex As Exception
                Errore &= ex.Message & vbCrLf
                Return False
            End Try

        Else
            Errore &= DesChiaveImpianto & "errore durante la creazione dei dati del nuovo impianto." & vbCrLf
            Return False
        End If




        If Errore <> "" Then
            Return False
        End If


        Return True

    End Function

    Public Function Appezzamento_ScriviModifica_From_List(ByVal DatiAppezzamenti As List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento),
                              ByRef OUTPUT_Appezza As Integer,
                              ByRef objParametri As AgronicaCoreParametri,
                              ByRef objParametri_Utenti As AgronicaCoreParametri
                              ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Scrivi_Global_EF()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False

        Try
            For Each itm As AgronicaCoreModelsSTD.anagrafiche.Appezzamento In DatiAppezzamenti
                Appezzamento_ScriviModifica(itm,
                                            objParametri,
                                            objParametri_Utenti)
            Next
            ret = True
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ret
    End Function

    Public Function Appezzamento_ScriviModifica(ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                Optional ByVal OpenNewTransaction As Boolean = True,
                                                Optional isFromOperazioneAgenda As Boolean = False, 'Aggiunto per evitare questo controllo quando modifico l'appezzamneto dall'operazione di semina con frazionamento                                       
                                                Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                                Optional AggiornaSoloValidita As Boolean = False,
                                                Optional ScriviLog As Boolean = True,
                                                Optional CopiaSposta_preserveDataUsernameCreazioneOriginali As CopiaSposta_preserveDataUsernameCreazioneOriginali = Nothing,
                                                Optional ByVal LogVerbose As Boolean = False,
                                                Optional defaultAlgoritmoCodifica As String = Nothing,
                                                Optional defaultCrea_MateriaPrima As Integer = -1
                                                ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Appezzamento_ScriviModifica()"

        Dim sw As Stopwatch = Stopwatch.StartNew()

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Amine - Leggo la configurazione per vedere se la feature di clustering è attiva
            Dim configClustering As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
            Dim clusteringFeatureString As String = configClustering.Leggi_Valore(0, "Gis_Clustering", "", "", objParametri_Server)
            Dim clusteringFeature As Boolean = False 'di default la feature è spenta

            'clusteringFeature contiene sempre il valore corretto anche nel caso in cui non riesca a convertire la stringa
            Boolean.TryParse(clusteringFeatureString, clusteringFeature)


            'lavez - 29/11/2023 - Recupero configurazione GIS_StaticMapCFG per determinare se ho delle limitazioni sui layer per cui si vuole generare l'anteprima
            Dim xCfgSitiR As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = xCfgSitiR.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)
            Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)

            Dim parametrizzazione As SementieriParametrizzazione = Nothing

            If DatiAppezzamento.dati_sementieri IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(DatiAppezzamento.dati_sementieri.Sementi) Then
                parametrizzazione = New SementieriParametrizzazione With {
                        .Sementi = DatiAppezzamento.dati_sementieri.Sementi,
                        .DatiPassaggio = DatiAppezzamento.dati_sementieri.DatiPassaggio,
                        .SementiMappaturaLibera = DatiAppezzamento.dati_sementieri.SementiMappaturaLibera
                    }
            End If

            If Not DatiAppezzamento.flag_cancellazione Then

                '20230406 MF: Passare un oggetto AgronicaCoreModelsSTD.Gis.SementieriParametrizzazione come ultimo parametro
                '             Per la modalità sementieri

                Internal_Appezzamento_ScriviModifica(
                    DatiAppezzamento,
                    GiasContext,
                    isFromOperazioneAgenda,
                    NoteLog,
                    StaticMapCFG,
                    objParametri_Server,
                    objParametri_Utenti,
                    ParametriSementieri:=parametrizzazione,
                    AggiornaSoloValidita:=AggiornaSoloValidita,
                    ScriviLog:=ScriviLog,
                    CopiaSposta_preserveDataUsernameCreazioneOriginali:=CopiaSposta_preserveDataUsernameCreazioneOriginali,
                    LogVerbose:=LogVerbose,
                    defaultAlgoritmoCodifica:=defaultAlgoritmoCodifica,
                    defaultCrea_MateriaPrima:=defaultCrea_MateriaPrima,
                    clusteringFeature:=clusteringFeature
                    )


            Else

                Internal_Appezzamento_Cancellazione(
                    DatiAppezzamento,
                    GiasContext,
                    NoteLog,
                    objParametri_Server,
                    objParametri_Utenti,
                    parametrizzazione,
                    ScriviLog:=ScriviLog,
                    clusteringFeature:=clusteringFeature
                    )

            End If

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException

            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex

        Catch ex As Exception

            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If


        sw.Stop()
        If LogVerbose Then
            Scrivi_LOG(objParametri_Server, nomeRoutine, "Tempo di esecuzione TOTALE: " & sw.Elapsed.TotalMilliseconds & " ms", False)
        End If


        Return True

    End Function

    Private Sub Internal_Appezzamento_Cancellazione(
        ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
        ByRef GiasContext As Gias_DeveloperServer_Entities,
        ByVal NoteLog As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri,
        Optional ByVal ParametriSementieri As AgronicaCoreModelsSTD.Gis.SementieriParametrizzazione = Nothing,
        Optional ScriviLog As Boolean = True,
        Optional ByVal clusteringFeature As Boolean = False)

        Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
        Dim Sa_Cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
        Dim Appezza = DatiAppezzamento.primaryKey.codice
        'Dim Campo_Cod = DatiAppezzamento.campoPK.codice

        If Piva = "" Then
            Throw New GiasException("Partita Iva non impostata correttamente")
        End If

        If Sa_Cod = 0 Then
            Throw New GiasException("Sa_Cod non impostato correttamente")
        End If

        If Appezza = 0 Then
            Throw New GiasException("Appezza non impostato correttamente")
        End If

        Dim appezzamentoDB = (From a In GiasContext.Appezzamento
                              Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza).SingleOrDefault()

        If appezzamentoDB IsNot Nothing Then

            If appezzamentoDB.Blk_Flag = -1 Then
                Throw New GiasException(String.Format(Gias.ErroreEliminazioneImpiantoBloccato, appezzamentoDB.APP_NOME))
            End If

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim leggiMacchineAssociate As Boolean = objPermessi.Controlla_Permessi_Utente(
                objParametri_Utenti.UtenteUsername,
                enum_Id_Servizio.GiasOnline,
                enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                objParametri_Utenti
                )

            If leggiMacchineAssociate Then
                Dim objAxPR As New AppezzamentiXParcoMacchine_R
                Dim dtLetture = objAxPR.ReadJoinLettureContatori(
                    objParametri_Server,
                    objParametri_Utenti,
                    New List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento) From {DatiAppezzamento}
                    )

                If dtLetture.Rows.Count > 0 Then
                    Throw New GiasException(My.Resources.AgronicaCoreAnagrafeBIZ.ErroreCancellazioneAppezzamentoPresentiLettureContatoriAssociati)
                End If

                Dim objAxPW As New AppezzamentiXParcoMacchine_W
                objAxPW.Delete(objParametri_Server, objParametri_Utenti, Piva, Sa_Cod, Appezza, giasContext:=GiasContext, openNewTransaction:=False)
            End If

            'ORA CANCELLIAMO I POLIGONI
            ''controllo se l'Appezzamento è collegato ad un Entità GIS
            'Dim objGIS As New AgronicaCoreGisBIZ.GIS_Entita_R
            'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(Piva, Sa_Cod, Appezza, 0, 0, enum_GIS2012_TipoEntita.APPEZZAMENTI, objParametri_Server)) Then
            '    Throw New GiasException(Gias.ImpossibileCancellareAppezzamentoCollegatoPoligono)
            'End If

            Dim impiantiDaEliminare = (From i In GiasContext.Reg_Impianti
                                       Where i.PIVA = Piva AndAlso i.SA_COD = Sa_Cod AndAlso i.APPEZZA = Appezza).ToList

            Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            For Each impiantoDaEliminare In impiantiDaEliminare

                Dim id_Reg = impiantoDaEliminare.ID_REG

                Dim DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto

                If DatiAppezzamento.impianti IsNot Nothing Then

                    DatiImpianto = (From ia In DatiAppezzamento.impianti
                                    Where ia.primaryKey.codice = id_Reg).FirstOrDefault

                Else

                    DatiImpianto = objImpianti_R.Leggi_Impianto_Anagrafica(
                        impiantoDaEliminare.PIVA,
                        impiantoDaEliminare.SA_COD,
                        impiantoDaEliminare.APPEZZA,
                        impiantoDaEliminare.ID_REG,
                        True,
                        False,
                        AGRODATAINIZIO,
                        False,
                        False,
                        objParametri_Server,
                        objParametri_Server,
                        objParametri_Utenti)

                End If

                'Prima di poter eliminare controllo CdG e Movimenti

                Dim descrizione_imp As String = "[dal " & impiantoDaEliminare.Validita_Inizio & " al " & impiantoDaEliminare.Validita_Fine & "]"

                Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                Dim controlloCdG = objControlloCdG.controllo_CdGxEliminazione(Nothing, Piva, Sa_Cod, Appezza, id_Reg, 0, objParametri_Server)

                Dim objControlli As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                If objControlli.controllo_MovimentiRicettexEliminazione(Nothing, Piva, Sa_Cod, Appezza, id_Reg, objParametri_Server) Then

                    Dim MessaggioErroreAgenda As String = String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoOperazioniRegistrate, descrizione_imp)

                    Throw New GiasException(MessaggioErroreAgenda)

                ElseIf controlloCdG.errore Then

                    Dim MessaggioErroreCdG As String = ""

                    If Not controlloCdG.messaggioSpecifico Then

                        MessaggioErroreCdG = String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoCdGEsercizio, descrizione_imp)

                    Else

                        MessaggioErroreCdG = String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoCdGEsercizioInData, descrizione_imp, controlloCdG.dataCdG.ToShortDateString())

                    End If

                    Throw New GiasException(MessaggioErroreCdG)

                ElseIf objControlli.controllo_UMA_RichiestexEliminazione(Piva, Sa_Cod, Appezza, id_Reg, objParametri_Server) Then
                    Dim msg As String = String.Format(Gias.ImpossibileEliminareImpiantoXPraticheUMA, descrizione_imp)
                    Throw New GiasException(msg)
                Else

                    If Not IsNothing(ParametriSementieri) AndAlso Not IsNothing(ParametriSementieri.DatiPassaggio) Then

                        Dim GISentita = (From entita In GiasContext.GIS_Entita
                                         Where entita.Piva = impiantoDaEliminare.PIVA AndAlso
                                           entita.Sa_Cod = impiantoDaEliminare.SA_COD AndAlso
                                           entita.Appezza = impiantoDaEliminare.APPEZZA AndAlso
                                           entita.Id_Imp = impiantoDaEliminare.ID_REG).ToList.FirstOrDefault

                        If GISentita Is Nothing OrElse String.IsNullOrEmpty(GISentita.Entita_Cod) OrElse GISentita.Entita_Cod = 0 Then
                            Throw New GiasException("Impianto non trovato")
                        End If

                        Internal_Elimina_Interferenze_Sementieri(ParametriSementieri,
                                                             GISentita.Entita_Cod,
                                                             objParametri_Server,
                                                             objParametri_Utenti)
                    End If

                    eliminaImpianto(
                        impiantoDaEliminare,
                        DatiImpianto,
                        objParametri_Server,
                        objParametri_Utenti,
                        GiasContext,
                        NoteLog:=NoteLog,
                        ScriviLog:=ScriviLog,
                        clusteringFeature:=clusteringFeature)

                End If

            Next

            Dim appezzamentixIndirizziDB = (From a In GiasContext.AppezzamentixIndirizzi
                                            Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList

            Dim appezzamentiXParticelleDB = (From a In GiasContext.AppezzamentiXParticelle
                                             Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza).ToList

            Dim appezzamentiXParticellexMacrousiDB = (From a In GiasContext.AppezzamentiXParticellexMacrousi
                                                      Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList

            Dim appezzamentiXParticellexMacrousixUtilizzoDB = (From a In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo
                                                               Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList

            Dim appezzamentixRubricaDB = (From a In GiasContext.AppezzamentixRubrica
                                          Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList

            Dim appezzamento_CodiciDB = (From a In GiasContext.Appezzamento_Codici
                                         Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList

            'Dim appezzamento_StoricoDB = (From a In GiasContext.Appezzamento_Storico
            '                              Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList

            Dim utentiXAppezzamentiDB = (From a In GiasContext.UtentiXAppezzamenti
                                         Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.Appezza = Appezza).ToList

            GiasContext.Appezzamento.Remove(appezzamentoDB)

            GiasContext.AppezzamentixIndirizzi.RemoveRange(appezzamentixIndirizziDB)

            GiasContext.AppezzamentiXParticelle.RemoveRange(appezzamentiXParticelleDB)

            GiasContext.AppezzamentiXParticellexMacrousi.RemoveRange(appezzamentiXParticellexMacrousiDB)

            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.RemoveRange(appezzamentiXParticellexMacrousixUtilizzoDB)

            GiasContext.AppezzamentixRubrica.RemoveRange(appezzamentixRubricaDB)

            GiasContext.Appezzamento_Codici.RemoveRange(appezzamento_CodiciDB)

            'GiasContext.Appezzamento_Storico.RemoveRange(appezzamento_StoricoDB)

            GiasContext.UtentiXAppezzamenti.RemoveRange(utentiXAppezzamentiDB)

            'CANCELLAZIONE POLIGONI

            Dim GIS_Entita_DaEliminare = (From a In GiasContext.GIS_Entita Where a.Piva = Piva AndAlso a.Sa_Cod = Sa_Cod AndAlso a.Appezza = Appezza AndAlso a.TipoEntita_Cod = enum_GIS2012_TipoEntita.APPEZZAMENTI).ToList()
            For Each entita_DaEliminare In GIS_Entita_DaEliminare
                Dim GIS_ElementiGrafici_DaEliminare = (From a In GiasContext.GIS_ElementiGrafici Where a.Entita_Cod = entita_DaEliminare.Entita_Cod).ToList()
                GiasContext.GIS_Entita.Remove(entita_DaEliminare)
                GiasContext.GIS_ElementiGrafici.RemoveRange(GIS_ElementiGrafici_DaEliminare)

                ''eliminazione record di clustering se modulo attivo
                If clusteringFeature Then
                    For Each ele In GIS_ElementiGrafici_DaEliminare
                        Dim GIS_ElementiGraficiCluster_DaEliminare = (From a In GiasContext.GIS_ElementiGrafici_Clustering Where a.ElementoGrafico_Cod = ele.ElementoGrafico_Cod).ToList()
                        If GIS_ElementiGraficiCluster_DaEliminare IsNot Nothing AndAlso GIS_ElementiGraficiCluster_DaEliminare.Count > 0 Then
                            GiasContext.GIS_ElementiGrafici_Clustering.RemoveRange(GIS_ElementiGraficiCluster_DaEliminare)
                        End If
                    Next
                End If
            Next

            GiasContext.SaveChanges()   'commit modifiche

            If ScriviLog Then
                Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim DatiAppezzamentoStr = JsonConvert.SerializeObject(DatiAppezzamento, tzh)

                'Scrittura tabella Agronica_Log_Anagrafe

                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                enum_TipoEntita_Des.Appezza,
                CStr(Piva), CStr(Sa_Cod),
                CStr(Appezza), Nothing,
                Nothing, Nothing,
                enum_TipoOperazioneDB.Cancellazione,
                objParametri_Server, enum_Id_Servizio.GiasOnline,
                NoteLog, DatiAppezzamentoStr)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()
            End If


        End If

    End Sub

    Private Sub Internal_Elimina_Interferenze_Sementieri(ByVal parametriSementieri As SementieriParametrizzazione,
                                                         ByVal Entita_Cod As Int32,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri)

        Interferenze.GeneraDescrizione_Casella_Conflitto(Entita_Cod, objParametri_Server, objParametri_Utenti)

        'Log operazione

        LoggaOperazioneDB(objParametri_Server,
                          Entita_Cod,
                          3,
                          parametriSementieri.DatiPassaggio,
                          parametriSementieri.Sementi)

        'Metto in cache tutti i log relativi all'entità

        Dim leggiDati As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_R
        Dim dt As DataTable = leggiDati.Leggi(0, Entita_Cod, objParametri_Server, objParametri_Utenti)

        Dim aggiorna As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_W

        For Each riga As DataRow In dt.Rows

            aggiorna.ScriviInCache(CInt(riga("Sementieri_Sportello_LogOperazioni_COD")),
                                   0,
                                   riga,
                                   objParametri_Server)

        Next

    End Sub

    Private Sub Internal_Appezzamento_ScriviModifica(ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                     ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                     ByVal isFromOperazioneAgenda As Boolean,
                                                     ByVal NoteLog As String,
                                                     ByVal StaticMapCFG As GeneraMappaStaticaInData,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                     Optional ByVal ParametriSementieri As SementieriParametrizzazione = Nothing,
                                                     Optional AggiornaSoloValidita As Boolean = False,
                                                     Optional ScriviLog As Boolean = True,
                                                     Optional CopiaSposta_preserveDataUsernameCreazioneOriginali As CopiaSposta_preserveDataUsernameCreazioneOriginali = Nothing,
                                                     Optional ByVal LogVerbose As Boolean = False,
                                                     Optional defaultAlgoritmoCodifica As String = Nothing,
                                                     Optional defaultCrea_MateriaPrima As Integer = -1,
                                                     Optional ByVal clusteringFeature As Boolean = False
                                                     )
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Appezzamento_ScriviModifica()"

        Dim swAppezzamento As Stopwatch = Stopwatch.StartNew()

        Dim appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento = Nothing

        Dim username As String = objParametri_Server.UsernameOperazione

        Dim EntitaCodxImg As New List(Of Integer)

        Dim objImpostazioni_Utenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim sw As Stopwatch
        sw = Stopwatch.StartNew()

        If Not String.IsNullOrEmpty(DatiAppezzamento.rif_Appezzamento) Then

            Dim DTRiferimentoAppezzamento = objImpostazioni_Utenti.Leggi(
            enum_Impostazioni_Utenti.Codice_Univoco_Appezzamento,
            2,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri_Utenti
            )

            If DTRiferimentoAppezzamento.Rows.Count > 0 AndAlso DTRiferimentoAppezzamento.Rows(0)("Impostazione_Valore_1").trim = "1" Then

                Dim objAppezzamentoCodici As New Appezzamento_Codici_R

                Dim filtro As String = "(Appezzamento_Codici.Sa_Cod <> " & DatiAppezzamento.primaryKey.centroAziendalePK.codice & " OR Appezzamento_Codici.Appezza <> " & DatiAppezzamento.primaryKey.codice & ")"

                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "Prima di Controllo riferimento alfanumerico appezzamento", False)
                End If
                Dim dtCodAppezzamento = objAppezzamentoCodici.Leggi(
                    DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                    0,
                    0,
                    enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                    DatiAppezzamento.rif_Appezzamento,
                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    filtro,
                    "",
                    objParametri_Server
                    )
                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "Dopo di Controllo riferimento alfanumerico appezzamento", False)
                End If

                If dtCodAppezzamento.Rows.Count > 0 Then
                    Throw New GiasException("Riferimento Appezzamento Già Utilizzato Su Un Altro")
                End If

            End If

        End If

        If LogVerbose Then
            sw.Stop()
            Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione DTRiferimentoAppezzamento: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
            sw.Reset()
            sw = Stopwatch.StartNew()
        End If


        If DatiAppezzamento.campoPK IsNot Nothing AndAlso DatiAppezzamento.campoPK.codice > 0 AndAlso Not AggiornaSoloValidita Then
            'Se arriviamo da modifica campo il flag è true
            ControlloDateCampo(DatiAppezzamento, objParametri_Server)

        End If

        Dim tipoOperazioneAppezzamento As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura

        If DatiAppezzamento.primaryKey.codice = 0 Then
            tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura
            Dim usernameCreazioneOriginale_xToolCopiaSposta As String = ""
            Dim dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO

            If CopiaSposta_preserveDataUsernameCreazioneOriginali IsNot Nothing Then
                usernameCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.Username_Creazione_Appezzamento
                dataCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.Data_Creazione_Appezzamento
            End If


            appezzamento = Internal_Scrivi_Appezzamento(
                DatiAppezzamento,
                objParametri_Server,
                objParametri_Utenti,
                username,
                GiasContext,
                False,
                NoteLog:=NoteLog,
                ScriviLog:=ScriviLog,
                usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta,
                LogVerbose
                )

            If appezzamento IsNot Nothing Then
                DatiAppezzamento.primaryKey.codice = appezzamento.APPEZZA
                DatiAppezzamento.primaryKey.centroAziendalePK.codice = appezzamento.SA_COD
            End If

        Else
            tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Modifica

            appezzamento = Internal_Modifica_Appezzamento(
                DatiAppezzamento,
                objParametri_Server,
                username,
                GiasContext,
                False,
                NoteLog:=NoteLog,
                AggiornaSoloValidita:=AggiornaSoloValidita,
                ScriviLog:=ScriviLog,
                LogVerbose
                )

        End If

        If LogVerbose Then
            sw.Stop()
            Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione tabAppezzamento: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
            sw.Reset()
            sw = Stopwatch.StartNew()
        End If

        If Not AggiornaSoloValidita Then
            If appezzamento Is Nothing Then

            Else

                Internal_ScriviModificaElimina_AppezzamentoCodici(
                    tipoOperazioneAppezzamento,
                    DatiAppezzamento,
                    objParametri_Server,
                    username,
                    GiasContext,
                    False
                    )

                If LogVerbose Then
                    sw.Stop()
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AppezzamentoCodici: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
                    sw.Reset()
                    sw = Stopwatch.StartNew()
                End If


                If (DatiAppezzamento.indirizzi IsNot Nothing) Then
                    If Not (DatiAppezzamento.indirizzi.Count = 0 AndAlso tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura) Then
                        Internal_Aggiorna_IndirizziAppezzamento(
                        appezzamento,
                        objParametri_Server,
                        DatiAppezzamento.indirizzi,
                        username,
                        GiasContext,
                        False
                        )
                    End If
                End If

                If LogVerbose Then
                    sw.Stop()
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AppezzamentoIndirizzi: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
                    sw.Reset()
                    sw = Stopwatch.StartNew()
                End If

                If (DatiAppezzamento.catastoAppezzamento IsNot Nothing) Then

                    If Not (DatiAppezzamento.catastoAppezzamento.Count = 0 AndAlso tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura) Then
                        Internal_Scrivi_CatastoAppezzamento(
                        DatiAppezzamento,
                        objParametri_Server,
                        username,
                        GiasContext,
                        False
                        )
                    End If

                End If

                If LogVerbose Then
                    sw.Stop()
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AppezzamentoCatasto: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
                    sw.Reset()
                    sw = Stopwatch.StartNew()
                End If


                If DatiAppezzamento.campoPK Is Nothing Then
                    DatiAppezzamento.campoPK = New Campo.PK(0, DatiAppezzamento.primaryKey.centroAziendalePK)
                End If

                If DatiAppezzamento.cartografia IsNot Nothing AndAlso DatiAppezzamento.cartografia <> "" Then

                    Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento(
                        tipoOperazioneAppezzamento,
                        DatiAppezzamento,
                        objParametri_Server,
                        username,
                        StaticMapCFG,
                        EntitaCodxImg,
                        GiasContext,
                        False,
                        LogVerbose,
                        clusteringFeature:=clusteringFeature
                        )

                    If IsScritturaRipartoCatasto(DatiAppezzamento, objParametri_Server, objParametri_Utenti) Then
                        Internal_Scrivi_CatastoAppezzamento_DaEntita(
                            EntitaCodxImg,
                            DatiAppezzamento,
                            GiasContext,
                            username,
                            objParametri_Server
                            )
                    End If
                End If

                If LogVerbose Then
                    sw.Stop()
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AppezzamentoCartografia: " & sw.Elapsed.TotalMilliseconds & " ms tempoAppezzamentoTotale:" & swAppezzamento.Elapsed.Milliseconds & "ms", False)
                    sw.Reset()
                    sw = Stopwatch.StartNew()
                End If

            End If
        End If

        swAppezzamento.Stop()
        If LogVerbose Then
            Scrivi_LOG(objParametri_Server, nomeRoutine, "Tempo di esecuzione Appezzamento: " & swAppezzamento.Elapsed.TotalMilliseconds & " ms", False)
        End If

        Dim swImpianto As Stopwatch = Stopwatch.StartNew()

        If (DatiAppezzamento.impianti IsNot Nothing) Then

            ' CANCELLAZIONE IMPIANTI DA CONTROLLO DATE

            If tipoOperazioneAppezzamento <> enum_TipoOperazioneDB.Scrittura Then

                DatiAppezzamento = eliminaImpianti(
                DatiAppezzamento,
                GiasContext,
                objParametri_Server,
                objParametri_Utenti,
                NoteLog:=NoteLog,
                ScriviLog:=ScriviLog
                )

            End If

            If LogVerbose Then
                sw.Stop()
                Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione EliminaImpianti: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                sw.Reset()
                sw = Stopwatch.StartNew()
            End If


            For Each imp As AgronicaCoreModelsSTD.anagrafiche.Impianto In DatiAppezzamento.impianti

                'CONTROLLO SALVATAGGIO CODICE IMPIANTO
                If Not AggiornaSoloValidita Then
                    ' se nuovo impianto e se non valorizzato, cerco di generare il codice impianto in base alle impostazioni del DB


                    If imp.primaryKey.codice = 0 AndAlso (String.IsNullOrEmpty(imp.codiceImpianto) OrElse String.IsNullOrWhiteSpace(imp.codiceImpianto)) Then
                        Dim piva As String = imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                        Dim year As Integer = imp.validita.inizio.Year

                        Dim algoritmoCodifica As String = ""
                        If defaultAlgoritmoCodifica Is Nothing Then
                            algoritmoCodifica = Replica_GIAS.LeggiAlgoritmoCodifica(piva, objParametri_Server)
                        End If

                        Dim codiceImpianto As String = Replica_GIAS.LeggiCodiceProgressivo(piva, algoritmoCodifica, enum_SequenzaProgressiviTipi.CodiciProgettoAgricoli, year, objParametri_Server)
                        imp.codiceImpianto = codiceImpianto
                    End If

                    If LogVerbose Then
                        sw.Stop()
                        Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AlgoritmoCodifica: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                        sw.Reset()
                        sw = Stopwatch.StartNew()
                    End If

                    If Not String.IsNullOrEmpty(imp.codiceImpianto) Then

                        Dim DTRiferimentoImpianto = objImpostazioni_Utenti.Leggi(
                        enum_Impostazioni_Utenti.Codice_Univoco_Impianto,
                        2,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri_Utenti
                        )

                        If DTRiferimentoImpianto.Rows.Count > 0 AndAlso DTRiferimentoImpianto.Rows(0)("Impostazione_Valore_1").trim = "1" Then


                            Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                            Dim filtro As String = "(Reg_Impianti_Codici.Sa_Cod <> " & imp.primaryKey.appezzamentoPK.centroAziendalePK.codice &
                                        " OR Reg_Impianti_Codici.Appezza <> " & imp.primaryKey.appezzamentoPK.codice &
                                        " OR Reg_Impianti_Codici.ID_Reg <> " & imp.primaryKey.codice &
                                        ") AND Progetto_Cod = 0 "

                            'Lavez - 27/05/2025 - Log verboso
                            If LogVerbose Then
                                Scrivi_LOG(objParametri_Server, nomeRoutine, "Prima di lettura check codice impianto già utilizzato", False)
                            End If

                            Dim dtCodImpianto = objImpiantoCodici.Leggi(
                                imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                0,
                                0,
                                0,
                                "",
                                enum_CodiciAnagrafe.Codice_Impianto,
                                imp.codiceImpianto,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                filtro,
                                "",
                                objParametri_Server
                                )

                            If dtCodImpianto.Rows.Count > 0 Then
                                Throw New GiasException("Codice Impianto già utilizzato")
                            End If

                        End If

                    End If

                    If LogVerbose Then
                        sw.Stop()
                        Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione DTRiferimentoImpianto: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                        sw.Reset()
                        sw = Stopwatch.StartNew()
                    End If

                End If


                Dim impianto As AgronicaCoreEntityFramework_POCO.Reg_Impianti = Nothing
                Dim tipoOperazioneImpianto As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura
                sw.Stop()
                sw.Reset()
                'Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione AppezzamentoCartografia: " & swAppezzamento.Elapsed.TotalMilliseconds & " ms", False)
                sw = Stopwatch.StartNew()
                If imp.primaryKey.codice = 0 Then
                    tipoOperazioneImpianto = enum_TipoOperazioneDB.Scrittura
                    Dim usernameCreazioneOriginale_xToolCopiaSposta As String = ""
                    Dim dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO

                    If CopiaSposta_preserveDataUsernameCreazioneOriginali IsNot Nothing AndAlso CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexImp IsNot Nothing Then
                        Try
                            usernameCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexImp((imp.validita.inizio, imp.validita.fine)).Item1
                            dataCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexImp((imp.validita.inizio, imp.validita.fine)).Item2
                        Catch ex As Exception
                            usernameCreazioneOriginale_xToolCopiaSposta = ""
                            dataCreazioneOriginale_xToolCopiaSposta = AGRODATAINIZIO
                        End Try
                    End If

                    imp.primaryKey.appezzamentoPK = DatiAppezzamento.primaryKey

                    impianto = Internal_Scrivi_Impianti_da_Appezzamento(
                        imp,
                        objParametri_Server,
                        objParametri_Utenti,
                        username,
                        GiasContext,
                        False,
                        NoteLog:=NoteLog,
                        ScriviLog:=ScriviLog,
                        usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                        dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta,
                        LogVerbose
                        )

                    If impianto IsNot Nothing Then

                        imp.primaryKey.codice = impianto.ID_REG

                    End If

                Else
                    tipoOperazioneImpianto = enum_TipoOperazioneDB.Modifica
                    impianto = Internal_Modifica_Impianti_da_Appezzamento(
                        imp,
                        objParametri_Server,
                        objParametri_Utenti,
                        username,
                        GiasContext,
                        False,
                        isFromOperazioneAgenda:=isFromOperazioneAgenda,
                        NoteLog:=NoteLog,
                        AggiornaSoloValidita:=AggiornaSoloValidita,
                        ScriviLog:=ScriviLog,
                        LogVerbose
                        )


                End If

                If LogVerbose Then
                    sw.Stop()
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione tabImpianto: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                    sw.Reset()
                    sw = Stopwatch.StartNew()
                End If

                If Not AggiornaSoloValidita Then
                    If impianto IsNot Nothing Then

                        Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento(
                            tipoOperazioneImpianto,
                            imp,
                            objParametri_Server,
                            username,
                            GiasContext,
                            False
                            )

                        If LogVerbose Then
                            sw.Stop()
                            Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione ImpiantoCodici: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                            sw.Reset()
                            sw = Stopwatch.StartNew()
                        End If

                        If imp.cartografia IsNot Nothing AndAlso imp.cartografia <> "" Then

                            Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento(
                                tipoOperazioneImpianto,
                                imp,
                                objParametri_Server,
                                username,
                                StaticMapCFG,
                                EntitaCodxImg,
                                GiasContext,
                                False,
                                clusteringFeature:=clusteringFeature
                                )

                        End If

                        If LogVerbose Then
                            sw.Stop()
                            Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione ImpiantoCartografia: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                            sw.Reset()
                            sw = Stopwatch.StartNew()
                        End If

                        ' aggiorna associazione impianto-macchina
                        If imp.macchineIrrigazione IsNot Nothing Then
                            EFReg_Impianti.ScriviModificaEliminaMacchinaxImpianto(tipoOperazioneImpianto,
                                imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                imp.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                imp.primaryKey.appezzamentoPK.codice,
                                imp.primaryKey.codice,
                                imp.macchineIrrigazione,
                                username,
                                objParametri_Server,
                                GiasContext,
                                False,
                                False)
                        End If

                    End If

                    If Not IsNothing(ParametriSementieri) AndAlso Not IsNothing(ParametriSementieri.DatiPassaggio) Then

                        Dim Cultivar = (From specieVegetale In GiasContext.Cultivar
                                        Where specieVegetale.Cul_Cod = imp.utilizzoTerreno.codice).ToList.FirstOrDefault

                        If Cultivar Is Nothing OrElse String.IsNullOrEmpty(Cultivar.Veg_Cod) OrElse Cultivar.Veg_Cod = 0 Then
                            Throw New GiasException("Specie vegetale non trovata")
                        End If

                        Dim GISentita = (From entita In GiasContext.GIS_Entita
                                         Where entita.Piva = impianto.PIVA AndAlso
                                           entita.Sa_Cod = impianto.SA_COD AndAlso
                                           entita.Appezza = impianto.APPEZZA AndAlso
                                           entita.Id_Imp = impianto.ID_REG).ToList.FirstOrDefault

                        If GISentita Is Nothing OrElse String.IsNullOrEmpty(GISentita.Entita_Cod) OrElse GISentita.Entita_Cod = 0 Then
                            Throw New GiasException("Impianto non trovato")
                        End If

                        Internal_ScriviModifica_Interferenze_Sementieri(
                            ParametriSementieri,
                            impianto,
                            imp.cartografia,
                            (tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura),
                            Cultivar.Veg_Cod,
                            GISentita.Entita_Cod,
                            objParametri_Server,
                            objParametri_Utenti,
                            GiasContext,
                            False
                            )

                    End If

                    If LogVerbose Then
                        sw.Stop()
                        Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione ImpiantoSementieri: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                        sw.Reset()
                        sw = Stopwatch.StartNew()
                    End If

                    If Not IsNothing(imp.utilizzoTerreno) AndAlso imp.utilizzoTerreno.classType = "Varieta" Then

                        'CREAZIONE SEMENTI E TRASFORMATI VEGETALI DA VARIETA

                        Dim cultivar = CType(imp.utilizzoTerreno, Varieta)
                        Dim objGias As New Importa_GIAS

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            Scrivi_LOG(objParametri_Server, nomeRoutine, "Prima di Crea_MateriaPrima_Specie_Varieta_Regolamento", False)
                        End If

                        objGias.Crea_MateriaPrima_Specie_Varieta_Regolamento(
                            cultivar,
                            2,
                            objParametri_Server,
                            objParametri_Utenti,
                            True,
                            True,
                            defaultCrea_MateriaPrima
                            )
                    End If

                    If LogVerbose Then
                        sw.Stop()
                        Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione MateriePrime: " & sw.Elapsed.TotalMilliseconds & " ms tempoImpiantoTotale:" & swImpianto.Elapsed.Milliseconds & "ms", False)
                        sw.Reset()
                        sw = Stopwatch.StartNew()
                    End If

                End If

                swImpianto.Stop()

                If LogVerbose Then
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "Tempo di esecuzione Impianto: " & swImpianto.Elapsed.TotalMilliseconds & " ms", False)
                End If

                sw.Stop()
                sw.Reset()
                sw = Stopwatch.StartNew()
                Dim swEsercizio As Stopwatch = Stopwatch.StartNew()

                If (imp.esercizi IsNot Nothing) Then

                    'CANCELLAZIONE ESERCIZI DA CONTROLLO DATE

                    If tipoOperazioneImpianto <> enum_TipoOperazioneDB.Scrittura Then
                        imp = eliminaEsercizi(
                        imp,
                        objParametri_Server,
                        objParametri_Utenti,
                        GiasContext,
                        NoteLog:=NoteLog,
                        ScriviLog:=ScriviLog
                        )
                    End If

                    If LogVerbose Then
                        sw.Stop()
                        Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione eliminaEsercizi: " & sw.Elapsed.TotalMilliseconds & " ms tempoEsercizioTotale:" & swEsercizio.Elapsed.Milliseconds & "ms", False)
                        sw.Reset()
                        sw = Stopwatch.StartNew()
                    End If

                    For Each ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio In imp.esercizi


                        ese.impiantoPK = imp.primaryKey

                        Dim esercizio As AgronicaCoreEntityFramework_POCO.Imprese_Progetti = Nothing
                        If Not AggiornaSoloValidita Then

                            If Not String.IsNullOrEmpty(ese.lotto) Then

                                Dim DTRiferimentoProgetto = objImpostazioni_Utenti.Leggi(
                                                                                        enum_Impostazioni_Utenti.Codice_Univoco_Progetto,
                                                                                        2,
                                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                        "",
                                                                                        "",
                                                                                        objParametri_Utenti)

                                If DTRiferimentoProgetto.Rows.Count > 0 AndAlso DTRiferimentoProgetto.Rows(0)("Impostazione_Valore_1").trim = "1" Then

                                    Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

                                    Dim filtro As String = "(Imprese_Progetti.Sa_Cod <> " & imp.primaryKey.appezzamentoPK.centroAziendalePK.codice &
                                    " OR Imprese_Progetti.Appezza <> " & imp.primaryKey.appezzamentoPK.codice &
                                    " OR Imprese_Progetti.ID_Reg <> " & imp.primaryKey.codice &
                                    " OR Imprese_Progetti.Progetto_Cod <> " & ese.codice &
                                    ") AND Progetto_Nome = '" & ese.lotto & "' "

                                    Dim dtCodImpianto = objImprese_Progetti.Leggi(
                                    imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                    0,
                                    "",
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    filtro,
                                    "",
                                    objParametri_Server)

                                    If dtCodImpianto.Rows.Count > 0 Then
                                        Throw New GiasException("Lotto già utilizzato")
                                    End If



                                End If

                            End If

                            If LogVerbose Then
                                sw.Stop()
                                Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione DTRiferimentoProgetto: " & sw.Elapsed.TotalMilliseconds & " ms tempoEsercizioTotale:" & swEsercizio.Elapsed.Milliseconds & "ms", False)
                                sw.Reset()
                                sw = Stopwatch.StartNew()
                            End If


                        End If

                        Dim tipoOperazioneEsercizio As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura
                        If ese.codice = 0 Then
                            tipoOperazioneEsercizio = enum_TipoOperazioneDB.Scrittura
                            Dim usernameCreazioneOriginale_xToolCopiaSposta As String = ""
                            Dim dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO

                            If CopiaSposta_preserveDataUsernameCreazioneOriginali IsNot Nothing AndAlso CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexEse IsNot Nothing Then
                                Try
                                    usernameCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexEse((ese.validita.inizio, ese.validita.fine)).Item1
                                    dataCreazioneOriginale_xToolCopiaSposta = CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexEse((ese.validita.inizio, ese.validita.fine)).Item2
                                Catch ex As Exception
                                    usernameCreazioneOriginale_xToolCopiaSposta = ""
                                    dataCreazioneOriginale_xToolCopiaSposta = AGRODATAINIZIO
                                End Try
                            End If

                            'Creazione esercizio
                            esercizio = Internal_Scrivi_Esercizi_da_Impianto_Appezzamento(
                                ese,
                                objParametri_Server,
                                objParametri_Utenti,
                                username,
                                GiasContext,
                                False,
                                NoteLog:=NoteLog,
                                ScriviLog:=ScriviLog,
                                usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                                dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta
                                )

                            If esercizio IsNot Nothing Then
                                ese.codice = esercizio.Progetto_Cod
                            End If

                        Else
                            tipoOperazioneEsercizio = enum_TipoOperazioneDB.Modifica
                            'Modifica esercizio
                            esercizio = Internal_Modifica_Esercizi_da_Impianto_Appezzamento(
                                ese,
                                objParametri_Server,
                                username,
                                GiasContext,
                                False,
                                NoteLog:=NoteLog,
                                AggiornaSoloValidita:=AggiornaSoloValidita,
                                ScriviLog:=ScriviLog,
                                imp.esercizi)
                        End If

                        If LogVerbose Then
                            sw.Stop()
                            Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione tabEsercizio: " & sw.Elapsed.TotalMilliseconds & " ms tempoEsercizioTotale:" & swEsercizio.Elapsed.Milliseconds & "ms", False)
                            sw.Reset()
                            sw = Stopwatch.StartNew()
                        End If


                        If Not AggiornaSoloValidita Then
                            If esercizio Is Nothing Then

                            Else

                                ese.codice = esercizio.Progetto_Cod

                                'Scrittura\aggiornamento\eliminazione codici
                                Internal_ScriviModificaElimina_EserciziCodici_da_Impianto_Appezzamento(tipoOperazioneEsercizio,
                                    ese,
                                    objParametri_Server,
                                    username,
                                    GiasContext,
                                    False
                                    )
                                If LogVerbose Then
                                    sw.Stop()
                                    Scrivi_LOG(objParametri_Server, nomeRoutine, "      Tempo di esecuzione EsercizioCodici: " & sw.Elapsed.TotalMilliseconds & " ms tempoEsercizioTotale:" & swEsercizio.Elapsed.Milliseconds & "ms", False)
                                    sw.Reset()
                                    sw = Stopwatch.StartNew()
                                End If

                                Me.UpdateLinkedContributes(objParametri_Server, objParametri_Utenti, ese, esercizio, GiasContext)
                            End If
                        End If

                    Next

                End If

                swEsercizio.Stop()
                If LogVerbose Then
                    Scrivi_LOG(objParametri_Server, nomeRoutine, "Tempo di esecuzione Esercizio: " & swEsercizio.Elapsed.TotalMilliseconds & " ms", False)
                End If


            Next

        End If

        If Not AggiornaSoloValidita Then
            For Each entita In EntitaCodxImg
                GeneraGMapJPG(entita, objParametri_Server)
            Next
        End If

    End Sub

    Private Sub ControlloDateCampo(appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento, objParametri_Server As AgronicaCoreParametri)
        Dim campo_R As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim campoDT = campo_R.Leggi(appezzamento.primaryKey.centroAziendalePK.partitaIva, appezzamento.primaryKey.centroAziendalePK.codice, appezzamento.campoPK.codice, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If campoDT.Rows.Count > 0 Then
            Dim validita_inizio_Campo As Date = campoDT(0)("Validita_Inizio")
            Dim validita_fine_Campo As Date = campoDT(0)("Validita_Fine")
            If appezzamento.validita.inizio < validita_inizio_Campo OrElse
                    appezzamento.validita.inizio > validita_fine_Campo OrElse
                    appezzamento.validita.fine > validita_fine_Campo OrElse
                    appezzamento.validita.fine < validita_inizio_Campo Then
                Throw New GiasException("Le date di validità dell'appezzamento non sono coerenti con le date di validità del campo ")
            End If
        End If


    End Sub

    Private Sub Internal_ScriviModifica_Interferenze_Sementieri(ByVal ParametriSementieri As SementieriParametrizzazione,
                                                                ByVal impianto As Reg_Impianti,
                                                                ByVal poligono As String,
                                                                ByVal insert As Boolean,
                                                                ByVal veg_cod As Int32,
                                                                ByVal entita_cod As Int32,
                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                Optional ByVal giasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                Optional ByVal NewTransaction As Boolean = False)

        If insert Then

            Internal_Scrivi_Interferenze_Sementieri(ParametriSementieri,
                                                    impianto,
                                                    poligono,
                                                    veg_cod,
                                                    entita_cod,
                                                    objParametri,
                                                    objParametri_Utenti,
                                                    giasContext,
                                                    NewTransaction)

        Else

            Internal_Modifica_Interferenze_Sementieri(ParametriSementieri,
                                                      impianto,
                                                      poligono,
                                                      veg_cod,
                                                      entita_cod,
                                                      objParametri,
                                                      objParametri_Utenti,
                                                      giasContext,
                                                      NewTransaction)

        End If

    End Sub

    Private Sub Internal_Modifica_Interferenze_Sementieri(ByVal ParametriSementieri As SementieriParametrizzazione,
                                                          ByVal impianto As Reg_Impianti,
                                                          ByVal poligono As String,
                                                          ByVal veg_cod As Int32,
                                                          ByVal entita_cod As Int32,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                          Optional ByVal giasContext As Gias_DeveloperServer_Entities = Nothing,
                                                          Optional ByVal newTransaction As Boolean = False)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Modifica_Interferenze_Sementieri()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If giasContext Is Nothing Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If newTransaction Then
            scope = New TransactionScope()
        End If

        Try

            LoggaOperazioneDB(objParametri, entita_cod, 2, ParametriSementieri.DatiPassaggio, ParametriSementieri.Sementi)

            'metto tutte le interferenze a stato 

            If ParametriSementieri.DatiPassaggio.ToString.Split("|")(5) = 1 Then

                Dim objInter_W As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W
                Dim objIntR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_R

                Interferenze.GeneraDescrizione_Casella_Conflitto(entita_cod, objParametri, objParametri_Utenti)

                Dim dtInterferenze As DataTable
                Dim numeroRecordDaRestituire As Integer = -1
                Dim moltiplicatoreDistanze As Integer = 1

                dtInterferenze = dtInterferenze_from_poligono(ParametriSementieri,
                                                              impianto.CODICE_FISCALE_TECNICO,
                                                              veg_cod,
                                                              impianto.GRVA_Cod_VEG,
                                                              impianto.Validita_Inizio,
                                                              impianto.Validita_Fine,
                                                              moltiplicatoreDistanze,
                                                              False,
                                                              numeroRecordDaRestituire,
                                                              poligono,
                                                              objParametri,
                                                              objParametri_Utenti)

                If dtInterferenze.Rows.Count > 0 Then
                    'Ho ancora interferenze???
                    For Each drowRisp As DataRow In dtInterferenze.Rows
                        Dim entita_cod_proprietario As Integer = drowRisp("Entita_Cod")

                        'se è già stata confermata 
                        Dim dt_gia_Inseriti As DataTable
                        dt_gia_Inseriti = objIntR.Leggi_da_Propietario_Interferente(entita_cod_proprietario,
                                                                                    entita_cod,
                                                                                    objParametri)

                        If dt_gia_Inseriti.Rows.Count = 0 Then
                            Dim Interferenze_Cod As Integer

                            Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                            'Interferenze_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Sementieri_Sportello_InterferenzePerConferma",
                            '                                                                 objParametri)
                            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                            Interferenze_Cod = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_InterferenzePerConferma", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)

                            objInter_W.Scrivi(Interferenze_Cod,
                                              entita_cod_proprietario,
                                              entita_cod,
                                              True,
                                              drowRisp("DistanzaEffettiva"),
                                              objParametri)
                        Else
                            'modifico e imposto a true 
                            objInter_W.Modifica_Flag(dt_gia_Inseriti.Rows(0).Item("Interferenze_Cod"),
                                                     True,
                                                     True,
                                                     objParametri)
                        End If
                    Next
                End If

            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            giasContext.Dispose()
        End If
    End Sub

    Private Shared Function dtInterferenze_from_poligono(ByVal ParametriSementieri As SementieriParametrizzazione,
                                                         ByVal pivaSementieroReferente As String,
                                                         ByVal veg_cod As Integer,
                                                         ByVal grva_cod As Integer,
                                                         ByVal data_inizio As Date,
                                                         ByVal data_fine As Date,
                                                         ByVal moltiplicatoreDistanze As Integer,
                                                         ByVal Flag_InterferenzeStessoReferente As Boolean,
                                                         ByVal numeroRecordDaRestituire As Integer,
                                                         ByVal wkt As String,
                                                         ByVal objParametri_Server As AgronicaCoreParametri,
                                                         ByVal objParametri_Utenti As AgronicaCoreParametri
                                                         ) As DataTable
        Dim gmlPol As String

        Dim converter As New wkt_gml
        gmlPol = converter.Trasforma(wkt, True, False, False, 0)

        'gmlPol = GetGmlPol(hiddenPunti_Nuovo)
        Dim gp As New GisPurpose(ParametriSementieri.Sementi, ParametriSementieri.SementiMappaturaLibera)
        Dim FlagFinalita As Boolean = gp.Mode() = GisPurpose.enum_GisPurpose.SementiSportello

        Dim verificatore As New AgronicaCoreGisDAL.GIS_OperazioniCartograficheDB
        Dim dtInterferenze As DataTable = verificatore.ElencoImpiantiInterferenzaSementi(
            pivaSementieroReferente,
            gmlPol,
            veg_cod,
            grva_cod,
            Math.Abs(CInt(grva_cod < 0)),
            data_inizio,
            data_fine,
            Flag_InterferenzeStessoReferente,
            moltiplicatoreDistanze,
            numeroRecordDaRestituire,
            FlagFinalita,
            objParametri_Server,
            objParametri_Utenti)

        Return dtInterferenze
    End Function

    Private Sub Internal_Scrivi_Interferenze_Sementieri(ByVal ParametriSementieri As SementieriParametrizzazione,
                                                        ByVal impianto As Reg_Impianti,
                                                        ByVal poligono As String,
                                                        ByVal veg_cod As Int32,
                                                        ByVal entita_cod As Int32,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                        Optional ByVal giasContext As Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal newTransaction As Boolean = False)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Scrivi_Interferenze_Sementieri()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If giasContext Is Nothing Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If newTransaction Then
            scope = New TransactionScope()
        End If

        Try

            LoggaOperazioneDB(objParametri, entita_cod, 1, ParametriSementieri.DatiPassaggio, ParametriSementieri.Sementi)

            'verifico se devo forzare l'evento di check interferenze 
            Dim VerificaInterferenze As Boolean = False
            If ParametriSementieri.DatiPassaggio.ToString.Split("|")(5) = 1 Then

                VerificaInterferenze = True
                Dim dtInterferenze As DataTable
                Dim numeroRecordDaRestituire As Integer = -1
                Dim moltiplicatoreDistanze As Integer = 1

                dtInterferenze = dtInterferenze_from_poligono(ParametriSementieri,
                                                              impianto.CODICE_FISCALE_TECNICO,
                                                              veg_cod,
                                                              impianto.GRVA_Cod_VEG,
                                                              impianto.Validita_Inizio,
                                                              impianto.Validita_Fine,
                                                              moltiplicatoreDistanze,
                                                              False,
                                                              numeroRecordDaRestituire,
                                                              poligono,
                                                              objParametri,
                                                              objParametri_Utenti)

                If dtInterferenze.Rows.Count > 0 Then
                    For Each drowRisp As DataRow In dtInterferenze.Rows
                        Dim entita_cod_proprietario As Integer
                        entita_cod_proprietario = drowRisp("Entita_Cod")
                        Dim Interferenze_Cod As Integer

                        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                        'Interferenze_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                        '                    "Sementieri_Sportello_InterferenzePerConferma",
                        '                    objParametri)
                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        Interferenze_Cod = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_InterferenzePerConferma", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)


                        Dim objInter As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W
                        objInter.Scrivi(Interferenze_Cod,
                                        entita_cod_proprietario,
                                        entita_cod,
                                        True,
                                        drowRisp("DistanzaEffettiva"),
                                        objParametri)
                    Next
                End If

            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            giasContext.Dispose()
        End If
    End Sub

    Private Shared Sub LoggaOperazioneDB(ByVal objParametri_Server As AgronicaCoreParametri,
                                         ByVal Ultimo_Entita_Cod As Integer,
                                         ByVal tipoOperazioneDB As Integer,
                                         ByVal DatiPassaggio As String,
                                         ByVal Sementi As String)

        If Not String.IsNullOrEmpty(DatiPassaggio) Then
            If DatiPassaggio.ToString.Split("|")(6) = 1 Then

                Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'Dim LogCod As Integer = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Sementieri_Sportello_LogOperazioni", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim LogCod As Integer = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_LogOperazioni", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                Dim SportelloCod As Integer = Sementi.ToString.Split("|")(4)

                Dim LoggaOperazioni As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_W
                LoggaOperazioni.Scrivi(LogCod, SportelloCod, tipoOperazioneDB, Ultimo_Entita_Cod, objParametri_Server)

            End If
        End If
    End Sub

    Private Function IsScritturaRipartoCatasto(
        datiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
        ) As Boolean

        If Not IsNothing(datiAppezzamento.catastoAppezzamento) AndAlso
           datiAppezzamento.catastoAppezzamento.Count > 0 Then
            Return False
        End If

        Const scriviRipartoCatastoAbilitato = "1"
        Const scriviRipartoCatastoDisattivo = "0"

        Dim objLetturaImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim listaCentriAziendali As New List(Of Integer)

        listaCentriAziendali.Add(datiAppezzamento.primaryKey.centroAziendalePK.codice)

        Dim impostazioneScriviRipartoCatasto =
            objLetturaImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
            datiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            listaCentriAziendali,
            enum_Impostazioni_Utenti.ScriviAppezza_RipartoCatastoDaEntita,
            scriviRipartoCatastoDisattivo,
            objParametri_Utenti,
            objParametri_Server)

        Return impostazioneScriviRipartoCatasto = scriviRipartoCatastoAbilitato

    End Function

    Private Sub Internal_Scrivi_CatastoAppezzamento_DaEntita(
        ByVal listaEntitaCodiceNumero As List(Of Integer),
        ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
        ByRef GiasContext As Gias_DeveloperServer_Entities,
        ByVal username As String,
        ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objLeggiAppezzaParticelle = New AppezzaxParticelle_R

        Dim dtAppezzaParticelle = objLeggiAppezzaParticelle.LeggiParticelle_Da_Appezzamento(
            DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            DatiAppezzamento.primaryKey.centroAziendalePK.codice,
            DatiAppezzamento.primaryKey.codice,
            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            "",
            "",
            objParametri_Server)

        'Se non esiste il riparto catasto per l'appezzamento, provvedo ad inserirlo

        If Not IsNothing(dtAppezzaParticelle) AndAlso dtAppezzaParticelle.Rows.Count = 0 Then

            Dim objRipartoCatasto = New AgronicaCoreGisBIZ.RipartoCatasto

            Dim listaEntitaCodiceStringa = New List(Of String)
            For Each entitaCodiceNumero In listaEntitaCodiceNumero
                listaEntitaCodiceStringa.Add(entitaCodiceNumero.ToString())
            Next

            Dim ripartoCatastoEsito = objRipartoCatasto.GetRipartoCatastoElencoEntita(
                listaEntitaCodiceStringa.ToArray(),
                objParametri_Server)

            If ripartoCatastoEsito.TipoErrore = enumTipoErroreRipartoCatasto.Nessuno AndAlso
               Not IsNothing(ripartoCatastoEsito.TabellaRipartoCatasto) Then

                Dim objLeggiImpreseParticelle = New ImpresexParticelle2_R
                Dim objScriviImpreseParticelle = New ImpresexParticelle2_W

                For Each rigaRipartoCatasto As DataRow In ripartoCatastoEsito.TabellaRipartoCatasto.Rows

                    Dim datiCatasto = New CatastoAppezzamento()

                    datiCatasto.particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK()

                    datiCatasto.particella.Prov = rigaRipartoCatasto.Item("Prov")
                    datiCatasto.particella.Com = rigaRipartoCatasto.Item("Com")
                    datiCatasto.particella.Sezione = rigaRipartoCatasto.Item("Sezione")
                    datiCatasto.particella.Foglio = rigaRipartoCatasto.Item("Foglio")
                    datiCatasto.particella.Numero = rigaRipartoCatasto.Item("Numero")
                    datiCatasto.particella.Subalterno = rigaRipartoCatasto.Item("Subalterno")

                    Dim areaIntersezione = rigaRipartoCatasto.Item("inter_ettari_are_centiare")

                    datiCatasto.area = UtilityProvider.Ettari_from_StringaEttariAreCentiare(areaIntersezione)

                    EFAppezzamento.Create_CatastoAppezzamento(
                        datiCatasto,
                        objParametri_Server,
                        DatiAppezzamento,
                        username,
                        GiasContext,
                        False)

                    ScritturaImpreseParticelleMancanti(
                        datiCatasto,
                        DatiAppezzamento,
                        objLeggiImpreseParticelle,
                        objScriviImpreseParticelle,
                        objParametri_Server)

                Next

            End If

        End If

    End Sub

    Private Sub ScritturaImpreseParticelleMancanti(
        ByVal datiCatasto As CatastoAppezzamento,
        ByVal datiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
        ByRef objLeggiImpreseParticelle As ImpresexParticelle2_R,
        ByRef objScriviImpreseParticelle As ImpresexParticelle2_W,
        ByRef objParametri_Server As AgronicaCoreParametri)

        Dim dtImpreseParticelle As DataTable = objLeggiImpreseParticelle.Leggi(
            0,
            datiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            datiAppezzamento.primaryKey.centroAziendalePK.codice,
            0,
            datiCatasto.particella.Prov,
            datiCatasto.particella.Com,
            datiCatasto.particella.Sezione,
            datiCatasto.particella.Foglio,
            datiCatasto.particella.Numero,
            datiCatasto.particella.Subalterno,
            enumSelezioneVariabile.Selezione_JoinDescrizioni,
            "",
            "",
            objParametri_Server)

        If dtImpreseParticelle.Rows.Count = 0 Then

            objScriviImpreseParticelle.Scrivi_2(
                datiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                datiAppezzamento.primaryKey.centroAziendalePK.codice,
                datiCatasto.particella.Prov,
                datiCatasto.particella.Com,
                datiCatasto.particella.Sezione,
                datiCatasto.particella.Foglio,
                datiCatasto.particella.Numero,
                datiCatasto.particella.Subalterno,
                "",
                enum_TitoloPossesso.Proprieta,
                datiCatasto.area,
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server)

        End If

    End Sub

    Private Sub Internal_Aggiorna_IndirizziAppezzamento(Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                        indirizzi As List(Of IndirizzoAssociato),
                                                        ByVal username As String,
                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal NewTransaction As Boolean = True)

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim list_indirizzi_cod = (From i In indirizzi Select i.indirizzo.codice).ToList

            Dim Piva = Appezzamento.PIVA
            Dim Sa_Cod = Appezzamento.SA_COD
            Dim Appezza = Appezzamento.APPEZZA

            Dim appezzaxIndirizzi_Del = (From axi In GiasContext.AppezzamentixIndirizzi
                                         Where axi.PIVA = Piva AndAlso
                                             axi.sa_cod = Sa_Cod AndAlso
                                             axi.appezza = Appezza AndAlso
                                             Not list_indirizzi_cod.Contains(axi.cod_indirizzo)).ToList

            Dim indirizzi_cod_del = (From a In appezzaxIndirizzi_Del Select a.cod_indirizzo).ToList()

            Dim indirizzi_del = (From i In GiasContext.Indirizzi Where indirizzi_cod_del.Contains(i.cod_indirizzo))

            GiasContext.Indirizzi.RemoveRange(indirizzi_del)
            GiasContext.AppezzamentixIndirizzi.RemoveRange(appezzaxIndirizzi_Del)

            GiasContext.SaveChanges()

            For Each add As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato In indirizzi
                If (add.indirizzo.istatComune.com IsNot Nothing) AndAlso (add.indirizzo.istatComune.prov IsNot Nothing) Then
                    Internal_ScriviModifica_IndirizzoAssociatoAppezzamento(
                        add,
                        objParametri_Server,
                        Appezzamento,
                        add.tipo_Indirizzo,
                        username,
                        GiasContext,
                        False
                        )
                End If
            Next

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Private Sub Internal_Scrivi_CatastoAppezzamento(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If appezzamento.primaryKey.codice <> 0 Then
                Dim Piva = appezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim Sa_Cod = appezzamento.primaryKey.centroAziendalePK.codice
                Dim Appezza = appezzamento.primaryKey.codice
                Dim catastoOld = (From c In GiasContext.AppezzamentiXParticelle
                                  Where c.PIVA = Piva AndAlso c.SA_COD = Sa_Cod AndAlso c.APPEZZA = Appezza).ToList

                GiasContext.AppezzamentiXParticelle.RemoveRange(catastoOld)
                GiasContext.SaveChanges()

                For Each catasto In appezzamento.catastoAppezzamento
                    AgronicaCoreAnagrafeDAL.EFAppezzamento.Create_CatastoAppezzamento(catasto, objParametri, appezzamento, username, GiasContext, False)
                Next


            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub

    Private Function Internal_Scrivi_Appezzamento(ByRef app As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  ByRef objParametri_utenti As AgronicaCoreParametri,
                                                  ByVal username As String,
                                                  Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                  Optional ByVal NewTransaction As Boolean = True,
                                                  Optional NoteLog As String = "",
                                                  Optional ScriviLog As Boolean = True,
                                                  Optional usernameCreazioneOriginale_xToolCopiaSposta As String = "",
                                                  Optional dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO,
                                                  Optional ByVal LogVerbose As Boolean = False
                                                  ) As AgronicaCoreEntityFramework_POCO.Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Scrivi_Appezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.Appezzamento = Nothing

        'Verifica_ValiditaInizioFine(app, objParametri, objParametri_utenti)
        'Verifica_Utilizzo(app, objParametri)
        Try

            Verifica_ControlliAppezzamento(app, objParametri, objParametri_utenti, True)

            Verifica_PrimaryKeysImpianto(app, objParametri)

            ret = AgronicaCoreAnagrafeDAL.EFAppezzamento.Appezzamento_Scrivi_EF(app,
                                                                                objParametri,
                                                                                objParametri_utenti,
                                                                                username,
                                                                                GiasContext,
                                                                                NewTransaction,
                                                                                NoteLog:=NoteLog,
                                                                                ScriviLog:=ScriviLog,
                                                                                usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                                                                                dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta, LogVerbose)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret
    End Function

    Private Function Internal_Modifica_Appezzamento(ByRef app As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True,
                                                    Optional NoteLog As String = "",
                                                    Optional AggiornaSoloValidita As Boolean = False,
                                                    Optional ScriviLog As Boolean = True,
                                                    Optional ByVal LogVerbose As Boolean = False
                                                    ) As AgronicaCoreEntityFramework_POCO.Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Modifica_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.Appezzamento = Nothing

        Try
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Prima di Verifica_ControlliAppezzamento", False)
            End If
            Verifica_ControlliAppezzamento(app, objParametri, objParametri)
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di Verifica_ControlliAppezzamento", False)
            End If
            ret = AgronicaCoreAnagrafeDAL.EFAppezzamento.Appezzamento_Modifica_EF(app,
                                                                                objParametri,
                                                                                username,
                                                                                GiasContext,
                                                                                NewTransaction,
                                                                                NoteLog:=NoteLog,
                                                                                AggiornaSoloValidita:=AggiornaSoloValidita,
                                                                                ScriviLog:=ScriviLog)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
        Return ret
    End Function

    Private Sub Internal_ScriviModifica_IndirizzoAssociatoAppezzamento(ByVal indAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                                       ByRef objParametri As AgronicaCoreParametri,
                                                                       ByRef appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                                                       ByRef tipoIndirizzo As Integer,
                                                                       ByRef username As String,
                                                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                       Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try
            If indAssociato.indirizzo.codice = 0 Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.CreateIndirizzoAppezzamento(
                    indAssociato.indirizzo,
                    objParametri,
                    appezzamento,
                    indAssociato.tipo_Indirizzo,
                    username,
                    GiasContext,
                    NewTransaction
                    )
            Else
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ModificaIndirizzoAppezzamento(
                    indAssociato.indirizzo,
                    objParametri,
                    appezzamento,
                    indAssociato.tipo_Indirizzo,
                    username,
                    GiasContext,
                    NewTransaction
                    )
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Private Function Internal_Scrivi_Impianti_da_Appezzamento(ByRef imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                              ByRef objParametri As AgronicaCoreParametri,
                                                              ByRef objParametriUtenti As AgronicaCoreParametri,
                                                              ByVal username As String,
                                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                              Optional ByVal NewTransaction As Boolean = True,
                                                              Optional NoteLog As String = "",
                                                              Optional ScriviLog As Boolean = True,
                                                              Optional usernameCreazioneOriginale_xToolCopiaSposta As String = "",
                                                              Optional dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO,
                                                              Optional ByVal LogVerbose As Boolean = False
                                                              ) As AgronicaCoreEntityFramework_POCO.Reg_Impianti
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Scrivi_Impianti_da_Appezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.Reg_Impianti = Nothing
        Dim verificaImpianto = New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

        Try
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Prima di Verifica_ControlliImpianto", False)
            End If
            verificaImpianto.Verifica_ControlliImpianto(imp, objParametri, objParametriUtenti, GiasContext, NewTransaction)

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di Verifica_ControlliImpianto", False)
            End If

            'LL - 28/12/2021 non servono.....
            'verificaImpianto.Verifica_CodiceImpianto(imp, objParametri, objParametri)

            ret = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Impianto_Scrivi_EF(imp,
                                                                            objParametri,
                                                                            objParametriUtenti,
                                                                            username,
                                                                            GiasContext,
                                                                            NewTransaction,
                                                                            NoteLog:=NoteLog,
                                                                            ScriviLog:=ScriviLog,
                                                                            usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                                                                            dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta,
                                                                            LogVerbose)

            If NewTransaction AndAlso scope IsNot Nothing Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function

    ''' <param name="isFromOperazioneAgenda"></param> 
    ''' Nel caso la Appezzamento_ScriviModifica venga richiamata dalla Operazioni NG (caso frazionamento), 
    ''' NON deve essere impostato il blocco_flag = 2 al cambio di superficie in (Verifica_Superficie)
    ''' NON deve essere fatto il Verifica_Utilizzo
    Private Function Internal_Modifica_Impianti_da_Appezzamento(ByRef imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                ByRef objParametriUtenti As AgronicaCoreParametri,
                                                                ByVal username As String,
                                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                Optional ByRef NewTransaction As Boolean = True,
                                                                Optional isFromOperazioneAgenda As Boolean = False,
                                                                Optional NoteLog As String = "",
                                                                Optional AggiornaSoloValidita As Boolean = False,
                                                                Optional ScriviLog As Boolean = True,
                                                                Optional ByVal LogVerbose As Boolean = False
                                                              ) As AgronicaCoreEntityFramework_POCO.Reg_Impianti

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Modifica_Impianti_da_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.Reg_Impianti = Nothing
        Dim verificaImpianto = New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

        Try
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Prima di Verifica_ControlliImpianto", False)
            End If

            verificaImpianto.Verifica_ControlliImpianto(imp, objParametri, objParametriUtenti, GiasContext, NewTransaction, isFromOperazioneAgenda)

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di Verifica_ControlliImpianto", False)
            End If

            ret = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Impianto_Modifica_EF(imp,
                                                                            objParametri,
                                                                            objParametriUtenti,
                                                                            username,
                                                                            GiasContext,
                                                                            NewTransaction,
                                                                            NoteLog:=NoteLog,
                                                                            AggiornaSoloValidita:=AggiornaSoloValidita,
                                                                            ScriviLog:=ScriviLog)
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di Impianto_Modifica_EF", False)
            End If

            If NewTransaction AndAlso scope IsNot Nothing Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function

    Private Function Internal_Scrivi_Esercizi_da_Impianto_Appezzamento(ByRef ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                       ByRef objParametri_server As AgronicaCoreParametri,
                                                                       ByRef objParametri_utente As AgronicaCoreParametri,
                                                                       ByVal username As String,
                                                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                       Optional ByVal NewTransaction As Boolean = True,
                                                                       Optional NoteLog As String = "",
                                                                       Optional ScriviLog As Boolean = True,
                                                                       Optional usernameCreazioneOriginale_xToolCopiaSposta As String = "",
                                                                       Optional dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO
                                                                       ) As AgronicaCoreEntityFramework_POCO.Imprese_Progetti
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Scrivi_EserciziCodici_da_Impianto_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim algoritmo_codifica = objImpreseCodici.Leggi_Codice_from_Imprese_Codici(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                                   enum_CodiciAnagrafe.Algoritmo_Codifica,
                                                                                                   objParametri_server)

        Dim ret As AgronicaCoreEntityFramework_POCO.Imprese_Progetti = Nothing
        Dim verificaEsercizio = New AgronicaCoreAnagrafeBIZ.Progetto_W

        Try
            verificaEsercizio.Verifica_ValiditaInizioFine(ese, objParametri_server)

            If algoritmo_codifica <> "" AndAlso String.IsNullOrEmpty(ese.lotto) AndAlso ese.codice = 0 Then
                ese.lotto = Replica_GIAS.LeggiCodiceProgressivo(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                               enum_SequenzaProgressiviTipi.CodiciOPAgriZoo,
                                                               ese.validita.inizio.Year,
                                                               objParametri_server)
            End If

            ret = AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Scrivi_EF(ese,
                                                                        objParametri_server,
                                                                        username,
                                                                        GiasContext,
                                                                        NewTransaction,
                                                                        NoteLog:=NoteLog,
                                                                        ScriviLog:=ScriviLog,
                                                                        usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                                                                        dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta)
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function

    Public Function Internal_Modifica_Esercizi_da_Impianto_Appezzamento(ByRef ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByVal username As String,
                                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                        Optional ByVal NewTransaction As Boolean = True,
                                                                        Optional NoteLog As String = "",
                                                                        Optional AggiornaSoloValidita As Boolean = False,
                                                                        Optional ScriviLog As Boolean = True,
                                                                        Optional ByVal allEsercizi_ControlliMovimenti As List(Of Esercizio) = Nothing
                                                                      ) As AgronicaCoreEntityFramework_POCO.Imprese_Progetti

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Modifica_Esercizi_da_Impianto_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.Imprese_Progetti = Nothing
        Dim verificaEsercizio = New AgronicaCoreAnagrafeBIZ.Progetto_W

        Try

            verificaEsercizio.Verifica_ValiditaInizioFine(ese, objParametri, allEsercizi_ControlliMovimenti:=allEsercizi_ControlliMovimenti)
            ret = AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Modifica_EF(ese,
                                                                        objParametri,
                                                                        username,
                                                                        GiasContext,
                                                                        NewTransaction,
                                                                        NoteLog:=NoteLog,
                                                                        AggiornaSoloValidita:=AggiornaSoloValidita,
                                                                        ScriviLog:=ScriviLog)
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function

    Private Sub Internal_ScriviModificaElimina_AppezzamentoCodici(ByRef TipoOperazioneAppezzamento As enum_TipoOperazioneDB,
                                                                  ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                  ByRef objParametri As AgronicaCoreParametri,
                                                                  ByVal username As String,
                                                                  ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                                  ByVal NewTransaction As Boolean)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModificaElimina_AppezzamentoCodici()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If


        Try
            'scrivi appezzamento_codici
            If (DatiAppezzamento.coltura_Precedente_1_Anno IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                              DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                                              DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                                              DatiAppezzamento.primaryKey.codice,
                                                                                              enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                                                                                              DatiAppezzamento.coltura_Precedente_1_Anno.codice,
                                                                                              False,
                                                                                              username,
                                                                                              objParametri,
                                                                                              GiasContext,
                                                                                              NewTransaction,
                                                                                              False)
            End If

            If (DatiAppezzamento.coltura_Precedente_2_Anno IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                                                DatiAppezzamento.primaryKey.codice,
                                                                                                enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                                                                                                DatiAppezzamento.coltura_Precedente_2_Anno.codice,
                                                                                                False,
                                                                                                username,
                                                                                                objParametri,
                                                                                                GiasContext,
                                                                                                NewTransaction,
                                                                                                False)

            End If

            If (DatiAppezzamento.coltura_Precedente_3_Anno IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                                                DatiAppezzamento.primaryKey.codice,
                                                                                                enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                                                                                                DatiAppezzamento.coltura_Precedente_3_Anno.codice,
                                                                                                False,
                                                                                                username,
                                                                                                objParametri,
                                                                                                GiasContext,
                                                                                                NewTransaction,
                                                                                                False)
            End If

            If (DatiAppezzamento.coltura_Precedente_4_Anno IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                                               DatiAppezzamento.primaryKey.codice,
                                                                                               enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                                                                                               DatiAppezzamento.coltura_Precedente_4_Anno.codice,
                                                                                               False,
                                                                                               username,
                                                                                               objParametri,
                                                                                               GiasContext,
                                                                                               NewTransaction,
                                                                                               False)
            End If

            If (DatiAppezzamento.confini_A_Rischio IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                                                DatiAppezzamento.primaryKey.codice,
                                                                                                enum_CodiciAnagrafe.Appezzamento_ConfiniRischio,
                                                                                                DatiAppezzamento.confini_A_Rischio,
                                                                                                False,
                                                                                                username,
                                                                                                objParametri,
                                                                                                GiasContext,
                                                                                                NewTransaction,
                                                                                                False)
            End If

            If (DatiAppezzamento.rif_Appezzamento IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                                DatiAppezzamento.rif_Appezzamento,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            If (DatiAppezzamento.isola IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Isola,
                                                                DatiAppezzamento.isola,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            If (DatiAppezzamento.n_App_Bio IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                                                                DatiAppezzamento.n_App_Bio,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            If (DatiAppezzamento.metodo_Produzione Is Nothing) Then
                'AF 04/25: Se proveniamo da giri <> interfaccia Angular il metodo produzione potrebbe non essere valorizzato
                'Ex: APP (da dove è emerso il problema)
                'Per scegliere dinamicamente il valore ci basiamo sul valore 'Disciplinare Aziendale Predefinito' impostato sull'Azienda
                'Se BIO --> Metodo Produzione BIO
                'Else Metodo Produzione Integrato
                Dim objAzienda As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim codiceDisciplinare = objAzienda.Leggi_Codice_from_Imprese_Codici(CStr(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva), enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri)

                If codiceDisciplinare <> "" AndAlso codiceDisciplinare = CInt(enum_Cod_Regolamento.Regolamento_bio).ToString() Then
                    DatiAppezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Biologico)
                Else
                    DatiAppezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
                End If
            End If
            If (DatiAppezzamento.metodo_Produzione IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.MetodoDiProduzione,
                                                                DatiAppezzamento.metodo_Produzione.codice,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            If DatiAppezzamento.fine_Impiego_Prod_Non_Conformi >= AGRODATAINIZIO Then
                Dim strImpiegoProdottiNonConformi = ""
                If DatiAppezzamento.fine_Impiego_Prod_Non_Conformi <> AGRODATAINIZIO AndAlso DatiAppezzamento.fine_Impiego_Prod_Non_Conformi <> AGRODATAFINE Then
                    strImpiegoProdottiNonConformi = DatiAppezzamento.fine_Impiego_Prod_Non_Conformi.ToShortDateString
                End If
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.DataFineImpiegoPNC,
                                                                strImpiegoProdottiNonConformi,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            If (DatiAppezzamento.utilizzo_Terreno IsNot Nothing) Then
                Dim valStr = ""
                If DatiAppezzamento.utilizzo_Terreno.Count > 0 Then
                    For Each utlizzoApp In DatiAppezzamento.utilizzo_Terreno
                        valStr &= utlizzoApp.codice & ","
                    Next
                    valStr = valStr.Substring(0, valStr.Length - 1)
                End If
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.OrientamentoProduttivo,
                                                                valStr,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)
            End If

            AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                           DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Terreno_Inutilizzato,
                                                                If(DatiAppezzamento.terrenoInutilizzato, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                           DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Terreno_Degradato,
                                                                If(DatiAppezzamento.terrenoDegradato, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(TipoOperazioneAppezzamento,
                                                                                           DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Low_ILUC,
                                                                If(DatiAppezzamento.lowILUC, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction, False)

            GiasContext.SaveChanges()

            'TUTTI GLI ALTRI CODICI to do....
            If (DatiAppezzamento.codici IsNot Nothing) Then

                If Not (TipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura AndAlso DatiAppezzamento.codici.Count = 0) Then
                    Dim list_id_cod As New List(Of Integer) From {
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                    enum_CodiciAnagrafe.Appezzamento_ConfiniRischio,
                    enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                    enum_CodiciAnagrafe.Isola,
                    enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                    enum_CodiciAnagrafe.MetodoDiProduzione,
                    enum_CodiciAnagrafe.DataFineImpiegoPNC,
                    enum_CodiciAnagrafe.OrientamentoProduttivo,
                    enum_CodiciAnagrafe.Low_ILUC,
                    enum_CodiciAnagrafe.Terreno_Degradato,
                    enum_CodiciAnagrafe.Terreno_Inutilizzato
                }

                    Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
                    Dim sa_cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                    Dim appezza = DatiAppezzamento.primaryKey.codice

                    Dim appezza_codici_del = (From ac In GiasContext.Appezzamento_Codici Where ac.PIVA = Piva AndAlso
                                                                                             ac.sa_cod = sa_cod AndAlso
                                                                                             ac.appezza = appezza AndAlso
                                                                                             (Not list_id_cod.Contains(ac.id_cod)) AndAlso
                                                                                             (ac.id_cod < 2000 OrElse ac.id_cod > 3000)).ToList

                    GiasContext.Appezzamento_Codici.RemoveRange(appezza_codici_del)
                    GiasContext.SaveChanges()

                    For Each attr As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori In DatiAppezzamento.codici

                        If attr.validita Is Nothing Then
                            attr.validita = New IntervalloTemporale()
                        Else
                            If attr.validita.inizio < AGRODATAINIZIO Then
                                attr.validita.inizio = AGRODATAINIZIO
                            End If

                            If attr.validita.fine < AGRODATAINIZIO Then
                                attr.validita.fine = AGRODATAFINE
                            End If

                        End If

                        Dim appezzamento_Codici_W As New Appezzamento_Codici_W
                        appezzamento_Codici_W.Scrivi(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                        DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                        DatiAppezzamento.primaryKey.codice,
                                                                        attr.codiceAnagrafe.codice,
                                                                        attr.valore,
                                                                        attr.validita.inizio,
                                                                        attr.validita.fine,
                                                                        objParametri)
                    Next

                End If

            End If

            'If (DatiAppezzamento.impianti.Count > 0) Then
            '    If (DatiAppezzamento.impianti(0).gruppoVarietale.codice >= 0) Then
            '        AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            '                                                        DatiAppezzamento.primaryKey.centroAziendalePK.codice,
            '                                                        DatiAppezzamento.primaryKey.codice,
            '                                                        enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                        "0",
            '                                                        True,
            '                                                        username,
            '                                                        objParametri,
            '                                                        GiasContext,
            '                                                        NewTransaction)
            '    Else
            '        AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            '                                                        DatiAppezzamento.primaryKey.centroAziendalePK.codice,
            '                                                        DatiAppezzamento.primaryKey.codice,
            '                                                        enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                        "1",
            '                                                        True,
            '                                                        username,
            '                                                        objParametri,
            '                                                        GiasContext,
            '                                                        NewTransaction)
            '    End If
            'End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Private Sub Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento(tipoOperazioneImpianto As enum_TipoOperazioneDB,
                                                                              ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                              ByRef objParametri As AgronicaCoreParametri,
                                                                              ByVal username As String,
                                                                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                                              ByVal NewTransaction As Boolean)
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If


        Try
            'registrazione destinazione uso
            If impianto.utilizzoTerreno IsNot Nothing Then
                If (impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaDestinazioneUsoxImpianto(tipoOperazioneImpianto,
                                                                                                         impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                                                          impianto.primaryKey.codice,
                                                                                                          impianto.utilizzoTerreno.codice,
                                                                                                          "",
                                                                                                          False,
                                                                                                          username,
                                                                                                          objParametri,
                                                                                                          GiasContext,
                                                                                                          NewTransaction,
                                                                                                          False)
                Else
                    'se ho una varieta\specie i codici da 3000 a 3999 devono essere cancellati
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaDestinazioneUsoxImpianto(tipoOperazioneImpianto,
                                                                                                         impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          impianto.utilizzoTerreno.codice,
                                                                          "",
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False)
                End If
            End If

            'reg_impianti_codici

            'Interbina
            'If (impianto.interbina <> 0) Then
            '    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
            '                                                              impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
            '                                                              impianto.primaryKey.appezzamentoPK.codice,
            '                                                              impianto.primaryKey.codice,
            '                                                              enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                              "1",
            '                                                              True,
            '                                                              username,
            '                                                              objParametri,
            '                                                              GiasContext,
            '                                                              NewTransaction)
            'Else
            '    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
            '                                                              impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
            '                                                              impianto.primaryKey.appezzamentoPK.codice,
            '                                                              impianto.primaryKey.codice,
            '                                                              enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                              "0",
            '                                                              True,
            '                                                              username,
            '                                                              objParametri,
            '                                                              GiasContext,
            '                                                              NewTransaction)
            'End If

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_CodiceB_Maschio,
                                                                          impianto.codBMBDBT_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_CodiceB_Femmina,
                                                                          impianto.codBMBDBT_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)


            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Genetica_Maschio,
                                                                          impianto.genetica_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Genetica_Femmina,
                                                                          impianto.genetica_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_OffType_Maschio,
                                                                          impianto.offType_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_OffType_Femmina,
                                                                          impianto.offType_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                          impianto.tra_Fila_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_TraFila_Femmina,
                                                                          impianto.distanzaTraFila_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                          impianto.su_Fila_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_SuFila_Femmina,
                                                                          impianto.distanzaSuFila_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Interbina,
                                                                          impianto.interbina,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Germinabilita,
                                                                          impianto.germinabilita,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            If (impianto.codiceZona IsNot Nothing) Then

                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                      impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.CodiceZona,
                                                                          impianto.codiceZona.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            End If

            If (impianto.tagliatoIntero IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                      impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate,
                                                                          impianto.tagliatoIntero.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)
            End If

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate,
                                                                          impianto.partiTuberi,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            If (impianto.dettaglio_varieta_personalizzato IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                      impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato,
                                                                          impianto.dettaglio_varieta_personalizzato.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)
            End If

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                  impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Codice_Impianto,
                                                                          impianto.codiceImpianto,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction, False, False)

            'lavez - 29/01/2024 - Verificato con Drudi, la data_inizio_portinnesto ha già il suo campo dedicato e viene già popolato.
            'AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
            '                                                              impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
            '                                                              impianto.primaryKey.appezzamentoPK.codice,
            '                                                              impianto.primaryKey.codice,
            '                                                              enum_CodiciAnagrafe.Data_Inizio_Portinnesto,
            '                                                              CStr(impianto.data_Inizio_Portinnesto),
            '                                                              True,
            '                                                              username,
            '                                                              objParametri,
            '                                                              GiasContext,
            '                                                              NewTransaction)

            If (impianto.gruppoVarietale IsNot Nothing AndAlso impianto.gruppoVarietale.codice >= 0) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                      impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                    enum_CodiciAnagrafe.Impianto_Ibrido,
                                                                    "0",
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    True, False)
            Else
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                      impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                    enum_CodiciAnagrafe.Impianto_Ibrido,
                                                                    "1",
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    True, False)
            End If

            'codici personalizzati impianto - solo scrittura\modifica
            If impianto.codici IsNot Nothing Then
                For Each codice In impianto.codici
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(tipoOperazioneImpianto,
                                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                    codice.codiceAnagrafe.codice,
                                                                    codice.valore,
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    True, False)
                Next
            End If

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Private Sub Internal_ScriviModificaElimina_EserciziCodici_da_Impianto_Appezzamento(tipoOperazioneEsercizio As enum_TipoOperazioneDB,
                                                                                       ByRef ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                                       ByRef objParametri As AgronicaCoreParametri,
                                                                                       ByVal username As String,
                                                                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                                                       ByVal NewTransaction As Boolean)
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Scrivi_EserciziCodici_da_Impianto_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim operazioneDB = -1
            If tipoOperazioneEsercizio = enum_TipoOperazioneDB.Scrittura Then
                operazioneDB = 1
            End If


            If (ese.apportiMassimiMacroelementi IsNot Nothing) Then
                If (ese.apportiMassimiMacroelementi.tipologia IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                                              ese.apportiMassimiMacroelementi.tipologia.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If

                If (ese.apportiMassimiMacroelementi.n IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                              ese.apportiMassimiMacroelementi.n,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If

                If (ese.apportiMassimiMacroelementi.p2o5 IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                              ese.apportiMassimiMacroelementi.p2o5,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If

                If (ese.apportiMassimiMacroelementi.k2o IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                              ese.apportiMassimiMacroelementi.k2o,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If

                If (ese.apportiMassimiMacroelementi.mgo IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_LimiteMg,
                                                                              ese.apportiMassimiMacroelementi.mgo,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If

            End If

            If (ese.organismo_Referente IsNot Nothing) Then
                If (ese.organismo_Referente.primaryKey IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Organismo_Referente,
                                                                              ese.organismo_Referente.primaryKey.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If
            End If

            If (ese.modalita_liquidazione IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Modalita_Liquidazione,
                                                                              ese.modalita_liquidazione.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.origine_prodotto IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Origine_Prodotto,
                                                                              ese.origine_prodotto.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.riferimento_Trasferimento_Dati IsNot Nothing) Then
                If (ese.riferimento_Trasferimento_Dati.primaryKey IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati,
                                                                              ese.riferimento_Trasferimento_Dati.primaryKey.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If
            End If

            If (ese.magazzino_Conferimento IsNot Nothing) Then
                If (ese.magazzino_Conferimento.primaryKey IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Magazzino_Conferimento,
                                                                              ese.magazzino_Conferimento.primaryKey.codice & "|" & ese.magazzino_Conferimento.primaryKey.centroAziendalePK.codice & "|" & ese.magazzino_Conferimento.primaryKey.centroAziendalePK.partitaIva,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If
            End If

            If (ese.tecnico IsNot Nothing AndAlso ese.tecnico.Count <> 0) Then
                Dim tecnico_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.tecnico.Count - 1
                If ese.tecnico.Count > 1 Then
                    For Each tecnico_codice In ese.tecnico
                        tecnico_string = sb.Append(tecnico_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    tecnico_string = ese.tecnico(0).codice
                End If
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Tecnico,
                    tecnico_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            Else
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Tecnico,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.residuo IsNot Nothing) Then
                If (ese.residuo.codice IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Residuo,
                    ese.residuo.codice,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
                End If
            End If

            If (ese.certificazioneAziendale IsNot Nothing AndAlso ese.certificazioneAziendale.Count <> 0) Then
                Dim certificazione_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.certificazioneAziendale.Count - 1
                If ese.certificazioneAziendale.Count > 1 Then
                    For Each cert_codice In ese.certificazioneAziendale
                        certificazione_string = sb.Append(cert_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    certificazione_string = ese.certificazioneAziendale(0).codice
                End If
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Certificazione,
                    certificazione_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            Else
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Certificazione,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.contributi IsNot Nothing AndAlso ese.contributi.Count <> 0) Then
                Dim contributi_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.contributi.Count - 1
                If ese.contributi.Count > 1 Then
                    For Each contrib_codice In ese.contributi
                        contributi_string = sb.Append(contrib_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    contributi_string = ese.contributi(0).codice
                End If
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Contributi,
                    contributi_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            Else
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Contributi,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.certificazioneProdotto IsNot Nothing) Then
                If (ese.certificazioneProdotto.codice IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(
                        ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Codice_Certificazione_Prodotto,
                                                                              ese.certificazioneProdotto.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False, True)

                End If
            End If

            If (ese.capitolato_Privato IsNot Nothing) Then
                If (ese.capitolato_Privato.codice IsNot Nothing) Then
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Capitolato_Privato,
                                                                              ese.capitolato_Privato.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
                End If
            End If

            If (ese.licenza_Coltivazione IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Zespri_Fasi_Fase,
                                                                              ese.licenza_Coltivazione.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.piano_Semina IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                              ese.piano_Semina.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.lavorazione IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Lavorazione,
                                                                              ese.lavorazione.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            If (ese.specifica IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Specifica,
                                                                              ese.specifica.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Distinta_Chiusa,
                                                                              If(ese.esercizio_Chiuso, 1, 0),
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)

            If (ese.iaf IsNot Nothing) Then
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxProgetto(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi,
                                                                              String.Join("|", ese.iaf),
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction, operazioneDB, Nothing, False)
            End If

            GiasContext.SaveChanges()

            'TUTTI GLI ALTRI CODICI to do......
            If (ese.codici IsNot Nothing) Then

                If Not (tipoOperazioneEsercizio = enum_TipoOperazioneDB.Scrittura AndAlso ese.codici.Count = 0) Then
                    VerificaSovrapposizioneCodici(ese.codici, objParametri)

                    Dim listField As New List(Of Integer) From {enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                                enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                enum_CodiciAnagrafe.Impianto_LimiteMg,
                                                                enum_CodiciAnagrafe.Organismo_Referente,
                                                                enum_CodiciAnagrafe.Modalita_Liquidazione,
                                                                enum_CodiciAnagrafe.Origine_Prodotto,
                                                                enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati,
                                                                enum_CodiciAnagrafe.Magazzino_Conferimento,
                                                                enum_CodiciAnagrafe.Capitolato_Privato,
                                                                enum_CodiciAnagrafe.Codice_Residuo,
                                                                enum_CodiciAnagrafe.Codice_Certificazione,
                                                                enum_CodiciAnagrafe.Codice_Certificazione_Prodotto,
                                                                enum_CodiciAnagrafe.Contributi,
                                                                enum_CodiciAnagrafe.Tecnico,
                                                                enum_CodiciAnagrafe.Lavorazione,
                                                                enum_CodiciAnagrafe.Specifica,
                                                                enum_CodiciAnagrafe.Zespri_Fasi_Fase,
                                                                enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                enum_CodiciAnagrafe.Distinta_Chiusa,
                                                                enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi}

                    Dim codiciEse = (From c In ese.codici Select c.codiceAnagrafe.codice).ToList
                    Dim Piva = ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                    Dim Sa_Cod = ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                    Dim Appezza = ese.impiantoPK.appezzamentoPK.codice
                    Dim id_Reg = ese.impiantoPK.codice
                    Dim Progetto_Cod = ese.codice

                    Dim imprese_progetti_codici_del = (From ipc In GiasContext.Reg_Impianti_Codici Where ipc.PIVA = Piva AndAlso
                                                                                                       ipc.sa_cod = Sa_Cod AndAlso
                                                                                                       ipc.appezza = Appezza AndAlso
                                                                                                       ipc.Id_Reg = id_Reg AndAlso
                                                                                                       ipc.Progetto_Cod = Progetto_Cod AndAlso
                                                                                                       (ipc.id_cod < 2000 OrElse ipc.id_cod > 3000) AndAlso
                                                                                                        Not listField.Contains(ipc.id_cod)).ToList

                    If imprese_progetti_codici_del.Count > 0 Then
                        GiasContext.Reg_Impianti_Codici.RemoveRange(imprese_progetti_codici_del)
                        GiasContext.SaveChanges()
                    End If

                    For Each attr As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori In ese.codici

                        If attr.validita Is Nothing Then
                            attr.validita = New IntervalloTemporale()
                        Else
                            If attr.validita.inizio < AGRODATAINIZIO Then
                                attr.validita.inizio = AGRODATAINIZIO
                            End If

                            If attr.validita.fine < AGRODATAINIZIO Then
                                attr.validita.fine = AGRODATAFINE
                            End If

                        End If

                        Dim Reg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
                        Reg_Impianti_Codici_W.ScrivixProgetto(
                            ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                            ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                            ese.impiantoPK.appezzamentoPK.codice,
                            ese.impiantoPK.codice,
                            ese.codice,
                            attr.codiceAnagrafe.codice,
                            attr.valore,
                            attr.validita.inizio,
                            attr.validita.fine,
                            objParametri,
                            DateTime.Now,
                            DateTime.Now
                            )
                    Next

                End If

            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub

    Private Sub UpdateLinkedContributes(objServer As AgronicaCoreParametri,
                                        objUtenti As AgronicaCoreParametri,
                                        exercice As Esercizio,
                                        project As Imprese_Progetti,
                                        giasContext As Gias_DeveloperServer_Entities)

        Dim objProgettiXContributi As New ProgettiXContributi_W
        Dim contributes As New List(Of LinkedContribute(Of KeyValuePair(Of Int32, String)))

        If Not IsNothing(exercice.acaContributes) Then
            contributes = contributes.Concat(
                exercice.acaContributes.Select(
                Function(l)
                    l.linkedItemPK = New KeyValuePair(Of Int32, String)(exercice.codice, project.Piva)
                    Return l
                End Function
            ).DefaultIfEmpty(
            New AgronicaCoreModelsSTD.metaschema.LinkedContribute(Of KeyValuePair(Of Int32, String))(
            0, ContributeType.ACA, New KeyValuePair(Of Int32, String)(exercice.codice, project.Piva)
            )
            )
            ).ToList

            objProgettiXContributi.Update(
            objServer,
            objUtenti,
            contributes,
            giasContext,
            False
            )
        End If

    End Sub

    Function ScriviModifica_GIS_Entita_ElementiGrafici(
        ByVal SalvaEntitaConAttributiIn As AgronicaCoreDTOStd.InData.Gis.SalvaEntitaConAttributi_In,
        ByRef objParametri_Server As AgronicaCoreParametri
        ) As AgronicaCoreModelsSTD.Gis.SalvaEntitaConAttributi_Out

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.ScriviModifica_GIS_Entita_ElementiGrafici()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        Dim GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
        bCloseContext = True

        scope = New TransactionScope()

        Dim pivaSuperUser = objParametri_Server.PivaSuperUser
        Dim username = objParametri_Server.UsernameOperazione

        Dim datiInseriti = New AgronicaCoreModelsSTD.Gis.SalvaEntitaConAttributi_Out()
        datiInseriti.ListaEntitaInserite = New List(Of Integer)
        datiInseriti.ListaElementiGraficiInseriti = New List(Of Integer)

        Try

            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici

            Dim entitaLista = From entita In GiasContext.GIS_Entita
                              Where entita.PivaSuperUser = pivaSuperUser AndAlso
                                    entita.Entita_Cod = SalvaEntitaConAttributiIn.EntitaCod
                              Select entita

            Dim entitaSingola = entitaLista.FirstOrDefault

            If entitaSingola Is Nothing AndAlso SalvaEntitaConAttributiIn.EntitaCod <> 0 Then
                Throw New GiasException("Entità da modificare non trovata")
            End If

            Dim tipoEntitaLayer = 0

            Const limiteCodiciLayerStandard = 1000000
            Const tipoEntitaLayerPersonalizzati = 11

            If SalvaEntitaConAttributiIn.LayerElementiGraficiCod > limiteCodiciLayerStandard Then
                tipoEntitaLayer = tipoEntitaLayerPersonalizzati
            Else
                Dim datiTipoEntitaLayerLista = From tipoEntita In GiasContext.GIS_TipoEntita
                                               Where tipoEntita.LayerElementiGrafici_Cod = SalvaEntitaConAttributiIn.LayerElementiGraficiCod
                                               Select tipoEntita.TipoEntita_Cod
                tipoEntitaLayer = datiTipoEntitaLayerLista.FirstOrDefault
            End If

            If entitaSingola Is Nothing Then

                'Inserimento entità + elemento grafico

                modelEntita.Piva = SalvaEntitaConAttributiIn.Piva
                modelEntita.Sa_Cod = SalvaEntitaConAttributiIn.SaCod
                modelEntita.TipoEntita = tipoEntitaLayer
                If Not IsNothing(SalvaEntitaConAttributiIn.AnalisiCampioneCod) Then
                    modelEntita.Analisi_Campione_Cod = SalvaEntitaConAttributiIn.AnalisiCampioneCod
                End If
                If Not IsNothing(SalvaEntitaConAttributiIn.FabbricatoCod) Then
                    modelEntita.Fabbricato_Cod = SalvaEntitaConAttributiIn.FabbricatoCod
                End If

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(
                    modelEntita,
                    objParametri_Server,
                    username,
                    GiasContext,
                    False)

                datiInseriti.ListaEntitaInserite.Add(entita.Entita_Cod)

                InserisciElementoGrafico(
                    SalvaEntitaConAttributiIn,
                    objParametri_Server,
                    modelElementoGrafico,
                    entita,
                    username,
                    GiasContext,
                    datiInseriti)

            Else

                'Modifica entità

                modelEntita.PivaSuperUser = entitaSingola.PivaSuperUser
                modelEntita.EntitaCod = entitaSingola.Entita_Cod
                modelEntita.Piva = SalvaEntitaConAttributiIn.Piva
                modelEntita.Sa_Cod = SalvaEntitaConAttributiIn.SaCod

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(
                    modelEntita,
                    objParametri_Server,
                    username,
                    GiasContext,
                    False)

                Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                          Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                                elem.Entita_Cod = modelEntita.EntitaCod
                                          Select elem

                Dim elementoGrafico = elementiGraficiList.FirstOrDefault

                If elementoGrafico Is Nothing Then

                    'Inserimento elemento grafico

                    InserisciElementoGrafico(
                    SalvaEntitaConAttributiIn,
                    objParametri_Server,
                    modelElementoGrafico,
                    entita,
                    username,
                    GiasContext,
                    datiInseriti)

                Else

                    'Modifica elemento grafico

                    modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                    modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                    modelElementoGrafico.ElementoGraficoDes = SalvaEntitaConAttributiIn.ElementoGraficoDes
                    modelElementoGrafico.Cartography = SalvaEntitaConAttributiIn.Cartografia
                    modelElementoGrafico.Flag_GPS = SalvaEntitaConAttributiIn.FlagGps

                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(
                        modelElementoGrafico,
                        objParametri_Server,
                        username,
                        GiasContext,
                        False)

                End If

            End If

            scope.Complete()
            scope.Dispose()

        Catch ex As GiasException

            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex

        Catch ex As Exception

            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return datiInseriti

    End Function

    Private Sub InserisciElementoGrafico(
             ByRef SalvaEntitaConAttributiIn As AgronicaCoreDTOStd.InData.Gis.SalvaEntitaConAttributi_In,
             ByRef objParametri_Server As AgronicaCoreParametri,
             ByRef modelElementoGrafico As AgronicaCoreGisDAL.ElementiGrafici,
             ByRef entita As GIS_Entita,
             ByVal username As String,
             ByRef GiasContext As Gias_DeveloperServer_Entities,
             ByRef datiInseriti As AgronicaCoreModelsSTD.Gis.SalvaEntitaConAttributi_Out
        )

        modelElementoGrafico.EntitaCod = entita.Entita_Cod
        modelElementoGrafico.Layer = SalvaEntitaConAttributiIn.LayerElementiGraficiCod
        modelElementoGrafico.ElementoGraficoDes = SalvaEntitaConAttributiIn.ElementoGraficoDes
        modelElementoGrafico.Cartography = SalvaEntitaConAttributiIn.Cartografia
        modelElementoGrafico.Flag_GPS = SalvaEntitaConAttributiIn.FlagGps

        Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(
            modelElementoGrafico,
            objParametri_Server,
            username,
            GiasContext,
            False)

        datiInseriti.ListaElementiGraficiInseriti.Add(elementoGrafico.ElementoGrafico_Cod)

    End Sub

    Private Sub Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento(tipoOperazioneAppezzamento As enum_TipoOperazioneDB,
                                                                                ByVal dati_appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                                ByVal username As String,
                                                                                ByVal StaticMapCFG As GeneraMappaStaticaInData,
                                                                                ByRef EntitaCodXImg As List(Of Integer),
                                                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                                Optional ByVal NewTransaction As Boolean = True,
                                                                                Optional ByVal LogVerbose As Boolean = False,
                                                                                Optional ByVal clusteringFeature As Boolean = False)
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If
        Try
            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Prima di lettura entita appezzamento", False)
            End If
            Dim entitaAppezzamento As GIS_Entita = Nothing

            If tipoOperazioneAppezzamento <> enum_TipoOperazioneDB.Scrittura Then

                Dim entitaAppezzamentoList = From entApp In GiasContext.GIS_Entita
                                             Where entApp.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva AndAlso
                                             entApp.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice AndAlso
                                             entApp.Appezza = dati_appezzamento.primaryKey.codice AndAlso
                                             entApp.Id_Imp = 0 AndAlso
                                             entApp.Validita_Inizio <= DateTime.Now AndAlso
                                             entApp.Validita_Fine >= DateTime.Now
                                             Select entApp

                entitaAppezzamento = entitaAppezzamentoList.FirstOrDefault

            End If
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di lettura entita appezzamento", False)
            End If
            If entitaAppezzamento Is Nothing Then
                'creazione
                modelEntita.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice
                modelEntita.Appezza = dati_appezzamento.primaryKey.codice
                modelEntita.Campo_cod = 0
                modelEntita.TipoEntita = enum_GIS2012_TipoEntita.APPEZZAMENTI

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(modelEntita, objParametri, username, GiasContext, False, LogVerbose)
                modelElementoGrafico.EntitaCod = entita.Entita_Cod
                modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps

                Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                If clusteringFeature Then
                    ClusteringRecord(elementoGrafico, username, GiasContext)
                End If

                'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                    If StaticMapCFG.LayerAbilitati.Contains(1) Then
                        If (modelElementoGrafico.Cartography <> "") Then
                            EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                        End If
                    End If
                End If
            Else
                modelEntita.PivaSuperUser = entitaAppezzamento.PivaSuperUser
                modelEntita.EntitaCod = entitaAppezzamento.Entita_Cod
                modelEntita.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice
                modelEntita.Appezza = dati_appezzamento.primaryKey.codice
                modelEntita.Campo_cod = 0

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(modelEntita, objParametri, username, GiasContext, False, LogVerbose)

                Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                          Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                             elem.Entita_Cod = modelEntita.EntitaCod
                                          Select elem

                Dim elementoGrafico = elementiGraficiList.FirstOrDefault
                If elementoGrafico Is Nothing Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                    modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps
                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                    ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                    If clusteringFeature Then
                        ClusteringRecord(ele, username, GiasContext)
                    End If

                    'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                    If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                        If StaticMapCFG.LayerAbilitati.Contains(1) Then
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                            End If
                        End If
                    End If
                Else
                    modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                    modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                    modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps

                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                    ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                    If clusteringFeature Then
                        ClusteringRecord(ele, username, GiasContext)
                    End If

                    'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                    If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                        If StaticMapCFG.LayerAbilitati.Contains(1) Then
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(elementoGrafico.Entita_Cod)
                            End If
                        End If
                    End If
                End If
            End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub

    Private Sub Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento(
        ByVal tipoOperazioneImpianto As enum_TipoOperazioneDB,
        ByVal dati_impianto As Impianto,
        ByRef objParametri As AgronicaCoreParametri,
        ByVal username As String,
        ByVal StaticMapCFG As GeneraMappaStaticaInData,
        ByRef EntitaCodXImg As List(Of Integer),
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
        Optional ByVal NewTransaction As Boolean = True,
        Optional ByVal LogVerbose As Boolean = False,
        Optional ByVal clusteringFeature As Boolean = False)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If
        Try
            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici
            Dim xMetaschema As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R

            Dim TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_GENERICO

            If dati_impianto.utilizzoTerreno.GetType() = GetType(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta) Then
                Dim Cul_cod As Integer = CType(dati_impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).codice
                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    Scrivi_LOG(objParametri, nomeRoutine, "Prima di GruCod_from_Cul_Cod", False)
                End If
                Dim gru_cod As Integer = xMetaschema.GruCod_from_Cul_Cod(Cul_cod, objParametri)

                Select Case gru_cod
                    Case 1
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_ARBOREA
                    Case 2
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTI_ERBACEA
                    Case 3
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA
                    Case Else
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_GENERICO
                End Select
                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    Scrivi_LOG(objParametri, nomeRoutine, "Dopo di GruCod_from_Cul_Cod", False)
                End If
            End If

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Prima di lettura entita impianto", False)
            End If
            Dim entitaImpianto As GIS_Entita
            'tipoOperazioneImpianto
            If tipoOperazioneImpianto <> enum_TipoOperazioneDB.Scrittura Then
                Dim entitaImpiantoList = From entImp In GiasContext.GIS_Entita
                                         Where entImp.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                             entImp.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice AndAlso
                                             entImp.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice AndAlso
                                             entImp.Id_Imp = dati_impianto.primaryKey.codice AndAlso
                                             entImp.Validita_Inizio <= DateTime.Now AndAlso
                                             entImp.Validita_Fine >= DateTime.Now
                                         Select entImp

                entitaImpianto = entitaImpiantoList.FirstOrDefault
            End If

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                Scrivi_LOG(objParametri, nomeRoutine, "Dopo di lettura entita impianto", False)
            End If

            If entitaImpianto Is Nothing Then
                'creazione
                modelEntita.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                modelEntita.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice
                modelEntita.Id_Imp = dati_impianto.primaryKey.codice
                modelEntita.Campo_cod = 0
                modelEntita.TipoEntita = TipoEntita

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(modelEntita, objParametri, username, GiasContext, False, LogVerbose)
                If (entita IsNot Nothing) Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.IMPIANTI
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps

                    Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                    ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                    If clusteringFeature Then
                        ClusteringRecord(elementoGrafico, username, GiasContext)
                    End If

                    'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                    If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                        If StaticMapCFG.LayerAbilitati.Contains(19) Then
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                            End If
                        End If
                    End If
                End If
            Else
                modelEntita.PivaSuperUser = entitaImpianto.PivaSuperUser
                modelEntita.EntitaCod = entitaImpianto.Entita_Cod
                modelEntita.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                modelEntita.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice
                modelEntita.Id_Imp = dati_impianto.primaryKey.codice
                modelEntita.TipoEntita = TipoEntita
                modelEntita.Campo_cod = 0

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(modelEntita,
                                                                                     objParametri,
                                                                                     username,
                                                                                     GiasContext,
                                                                                     False,
                                                                                     True,
                                                                                     LogVerbose)

                Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                          Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                             elem.Entita_Cod = modelEntita.EntitaCod
                                          Select elem

                Dim elementoGrafico = elementiGraficiList.FirstOrDefault
                If elementoGrafico Is Nothing Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.IMPIANTI
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps
                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                    ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                    If clusteringFeature Then
                        ClusteringRecord(ele, username, GiasContext)
                    End If

                    'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                    If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                        If StaticMapCFG.LayerAbilitati.Contains(19) Then
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                            End If
                        End If
                    End If
                Else
                    modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                    modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps

                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(modelElementoGrafico, objParametri, username, GiasContext, False, LogVerbose)

                    ' Esegue questo blocco solo se l'interruttore della feature di clustering è acceso.
                    If clusteringFeature Then
                        ClusteringRecord(ele, username, GiasContext)
                    End If

                    'lavez - 29/11/2023 - messo sotto parametro rif mail di Vanni R: dati demetra in collaudo
                    If StaticMapCFG.LayerAbilitati IsNot Nothing Then
                        If StaticMapCFG.LayerAbilitati.Contains(19) Then
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(elementoGrafico.Entita_Cod)
                            End If
                        End If
                    End If
                End If
            End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub

    Public Shared Sub GeneraGMapJPG(ByVal EntitaCod As Integer, ByRef objParametri As AgronicaCoreParametri)
        Dim leggiCFGStaticMap As New Configurazione_Siti_R
        Dim jSonStaticMapCFG As String = leggiCFGStaticMap.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri)
        Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
        StaticMapCFG.EntitaCod = EntitaCod

        Dim gestioneStaticMaps As New GoogleStaticMaps

        StaticMapCFG.objParametri_Server = objParametri
        gestioneStaticMaps.AggiornaElementoGraficoConMappaStatica(StaticMapCFG)
    End Sub

    Private Sub Verifica_ControlliAppezzamento(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               Optional ByVal InserimentoAppezzamento As Boolean = False)
        Verifica_Sa_Cod(appezzamento, objParametri_Server)
        Verifica_Superficie(appezzamento, objParametri_Server)
        Verifica_CampoPerCentro(appezzamento, objParametri_Server)
        Verifica_MetodoProduzione(appezzamento, objParametri_Server)
        'Lavez - 29/05/2025 - controllo per non verificare le date di inizio e fine se si tratta di un inserimento (In coldiretti può solo che peggiorare le performance)
        If Not InserimentoAppezzamento Then
            Verifica_ValiditaInizioFine(True, appezzamento, objParametri_Server, objParametri_Utenti)
        End If
    End Sub

    Private Sub Verifica_Superficie(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                         ByRef objParametri_Server As AgronicaCoreParametri)

        Dim MessaggioErrore As String

        If appezzamento.superficie = "0" OrElse appezzamento.superficie = 0 Then
            MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.LaSUPERFICIENonPuoEssereNulla
            Throw New GiasException(MessaggioErrore)
        ElseIf Not IsNumeric(appezzamento.superficie) Then
            MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.IlValoreDellaSUPERFICIEDeveEssereNumerico
            Throw New GiasException(MessaggioErrore)
        End If

    End Sub

    Private Sub Verifica_Sa_Cod(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                        ByRef objParametri_Server As AgronicaCoreParametri)

        Dim MessaggioErrore As String

        If appezzamento.primaryKey.centroAziendalePK.codice = "0" OrElse appezzamento.primaryKey.centroAziendalePK.codice = 0 Then
            MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoSaCodValorizzato
            Throw New GiasException(MessaggioErrore)
        ElseIf Not IsNumeric(appezzamento.primaryKey.centroAziendalePK.codice) Then
            MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoSaCodNumerico
            Throw New GiasException(MessaggioErrore)
        End If

    End Sub

    Private Sub Verifica_CampoPerCentro(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                    ByRef objParametri As AgronicaCoreParametri)

        Try
            'Lavez - 16/09/2024 - CampoPK non è obbligatorio che sia valorizzato... potrebbe essere null
            If appezzamento.campoPK IsNot Nothing AndAlso appezzamento.campoPK.codice <> 0 AndAlso
                appezzamento.campoPK.centroAziendalePK.codice <> appezzamento.primaryKey.centroAziendalePK.codice Then
                Dim MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoNonSottostaAlCentro
                Throw New GiasException(MessaggioErrore)
            End If
        Catch ex As GiasException
            Throw ex
        End Try

    End Sub

    Private Sub VerificaSovrapposizioneCodici(ByRef codici As List(Of CodiciAnagrafeValori),
                                              ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Try
            For Each codice In codici
                Dim codiciUguali = codici.Where(Function(t) t.codiceAnagrafe.codice = codice.codiceAnagrafe.codice).ToList()
                codiciUguali.Remove(codice)
                For Each codConfronto In codiciUguali
                    If (codConfronto.validita.inizio <= codice.validita.fine AndAlso codice.validita.inizio <= codConfronto.validita.fine) Then
                        MessaggioErrore = ("Il codice " & codice.codiceAnagrafe.descrizione & " (" & codice.valore & ") è in sovrapposizione di validità con il codice " & codConfronto.codiceAnagrafe.descrizione & " (" & codConfronto.valore & ").")
                        Throw New GiasException(MessaggioErrore)
                    End If
                Next
            Next

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        End Try
    End Sub

    Private Sub Verifica_MetodoProduzione(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                          ByRef objParametri_Server As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""

        If appezzamento.metodo_Produzione IsNot Nothing AndAlso
            (appezzamento.metodo_Produzione.codice = enum_MetodoProduzione.Biologico OrElse appezzamento.metodo_Produzione.codice = enum_MetodoProduzione.InConversione) AndAlso
            appezzamento.impianti IsNot Nothing Then
            For Each impianto In appezzamento.impianti
                If impianto.utilizzoTerreno.classType <> "DestinazioneUso" Then
                    For Each esercizio In impianto.esercizi
                        If esercizio.vincolo IsNot Nothing Then
                            If esercizio.vincolo.regolamento IsNot Nothing AndAlso esercizio.vincolo.regolamento.codice <> enum_Cod_Regolamento.Regolamento_bio Then
                                MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.MetodoProduzioneBiologico
                                Throw New GiasException(MessaggioErrore)
                            End If
                        Else
                            If esercizio.regolamento IsNot Nothing AndAlso esercizio.regolamento.codice <> enum_Cod_Regolamento.Regolamento_bio Then
                                MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.MetodoProduzioneBiologico
                                Throw New GiasException(MessaggioErrore)
                            End If
                        End If
                    Next
                End If
            Next
        End If
    End Sub

    Private Sub Verifica_PrimaryKeysImpianto(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                         ByRef objParametri_Server As AgronicaCoreParametri)

        For Each impianto In appezzamento.impianti
            impianto.primaryKey.appezzamentoPK = appezzamento.primaryKey
        Next

    End Sub

    Private Sub Verifica_ValiditaInizioFine(ByRef internal As Boolean,
                                                 ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento.Verifica_ValiditaInizioFine()"
        Dim Validita_Fine = appezzamento.validita.fine
        Dim Validita_Inizio = appezzamento.validita.inizio

        Dim sa_cod = appezzamento.primaryKey.centroAziendalePK.codice
        Dim piva = appezzamento.primaryKey.centroAziendalePK.partitaIva
        Dim appezza = appezzamento.primaryKey.codice
        Dim descrizione = appezzamento.descrizione

        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim centro = objCentriR.Leggi(piva, sa_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objAppR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim app = objAppR.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim imp = objImpiantoR.LeggiImpianto_ControlliAnagrafica(piva, sa_cod, appezza, 0, objParametri_Server)

        Dim nomeCurrent As String = ""
        If app.Rows.Count = 1 Then
            nomeCurrent = $"[{Gias.AppezzamentoAbbr} { app.Rows(0).Item("App_Nome")}]: "
        End If

        Try

            If app.Rows.Count > 0 AndAlso appezza <> 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Impianto
                If Validita_Inizio <> app.Rows(0)("Validita_Inizio") Or Validita_Fine <> app.Rows(0)("Validita_Fine") Then
#Region "Non ci entrerà mai, lo teniamo perchè potrebbe tornare utile?"
                    If False Then
                        If imp.Rows.Count > 0 Then
                            For Each i In imp.Rows


                                Dim id_Reg = i("id_reg")

                                'Ripristino le date dopo ogni iterazione
                                Dim Validita_InizioNew = Validita_Inizio
                                Dim Validita_FineNew = Validita_Fine

                                'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                If i("Validita_Inizio") > Validita_InizioNew Then
                                    Validita_InizioNew = i("Validita_Inizio")
                                End If
                                If Validita_FineNew > i("Validita_Fine") Then
                                    Validita_FineNew = i("Validita_Fine")
                                End If

                                If i("Validita_Inizio") <> Validita_InizioNew Or Validita_FineNew <> i("Validita_Fine") Then
                                    Dim objImpiantiR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                                    Dim impR = objImpiantiR.Leggi_Appezzamento_Anagrafica(Piva:=piva,
                                                                                          Sa_Cod:=sa_cod,
                                                                                          Appezza:=appezza,
                                                                                          IdReg:=id_Reg,
                                                                                          Leggi_Impianti:=True,
                                                                                          Leggi_Indirizzi:=True,
                                                                                          Leggi_Catasto:=True,
                                                                                          data:=AGRODATAINIZIO,
                                                                                          filtroData:=False,
                                                                                          Leggi_Distinte:=True,
                                                                                          Leggi_Cartografia:=False,
                                                                                          objParametri_Server,
                                                                                          objParametri_Server,
                                                                                          objParametri_Utenti
                                                                                          )
                                    'lavez - 15/02/2022 - se non trova nulla, non deve andare in crash...
                                    If impR IsNot Nothing Then

                                        '------------------------
                                        'CONTROLLO DATE IMPIANTI
                                        '------------------------
                                        Dim impiantiDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)
                                        Dim eserciziDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

                                        For Each impianto In impR.impianti

                                            'Ripristino le date dopo ogni iterazione
                                            Dim Validita_Inizio_Impianto = Validita_InizioNew
                                            Dim Validita_Fine_Impianto = Validita_FineNew

                                            'se la data di inizio dell'impianto è SUCCESSIVA alla FINE dell'Appezzamento, elimino l'Impianto
                                            'se la data di fine dell'impianto è PRECEDENTE all'INIZIO dell'Appezzamento, elimino l'Impianto
                                            If impianto.validita.inizio > Validita_FineNew OrElse impianto.validita.fine < Validita_InizioNew Then
                                                impiantiDaEliminare.Add(impianto)

                                                'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                            ElseIf impianto.validita.inizio >= Validita_InizioNew AndAlso impianto.validita.fine <= Validita_FineNew Then
                                                If impianto.validita.inizio > Validita_InizioNew Then
                                                    Validita_Inizio_Impianto = impianto.validita.inizio
                                                End If
                                                If impianto.validita.fine < Validita_FineNew Then
                                                    Validita_Fine_Impianto = impianto.validita.fine
                                                End If
                                            End If

                                            'aggiorno le validità solo se l'Impianto non è stato cancellato
                                            If Not impiantiDaEliminare.Contains(impianto) Then
                                                impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Impianto, Validita_Fine_Impianto)
                                            End If


                                            '------------------------
                                            'CONTROLLO DATE ESERCIZI
                                            '------------------------
                                            For Each esercizio In impianto.esercizi

                                                'Ripristino le date dopo ogni iterazione
                                                Dim Validita_Inizio_Esercizio = Validita_Inizio_Impianto
                                                Dim Validita_Fine_Esercizio = Validita_Fine_Impianto

                                                'se la data di inizio dell'Esercizio è SUCCESSIVA alla FINE dell'Impianto, elimino l'Esercizio
                                                'se la data di fine dell'Esercizio è PRECEDENTE all'INIZIO dell'Impianto, elimino l'Esercizio
                                                If esercizio.validita.inizio > Validita_Fine_Impianto OrElse esercizio.validita.fine < Validita_Inizio_Impianto Then
                                                    eserciziDaEliminare.Add(esercizio)


                                                    'Controllo se le date dell'Esercizio rientrano nell'intervallo temporale dell'Impianto, in questo caso rimangono invariate
                                                ElseIf esercizio.validita.inizio >= Validita_Inizio_Impianto AndAlso esercizio.validita.fine <= Validita_Fine_Impianto Then
                                                    If esercizio.validita.inizio > Validita_Inizio_Impianto Then
                                                        Validita_Inizio_Esercizio = esercizio.validita.inizio
                                                    End If
                                                    If esercizio.validita.fine < Validita_Fine_Impianto Then
                                                        Validita_Fine_Esercizio = esercizio.validita.fine
                                                    End If
                                                End If

                                                'aggiorno le validità solo se l'Esercizio non è stato cancellato
                                                If Not eserciziDaEliminare.Contains(esercizio) Then
                                                    esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Esercizio, Validita_Fine_Esercizio)
                                                End If
                                            Next

                                            For Each esercizio In eserciziDaEliminare
                                                impianto.esercizi.Remove(esercizio)
                                            Next
                                        Next


                                        For Each impianto In impiantiDaEliminare
                                            impR.impianti.Remove(impianto)
                                        Next

                                        Dim objAppsW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                                        If Not internal Then
                                            Dim appW = objAppsW.Appezzamento_ScriviModifica(impR, objParametri_Server, objParametri_Utenti)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End If
#End Region

                    If Validita_Inizio < CDate(centro.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.InizioAppezzamentoPrecedenteCreazioneCentroAziendale & " (" & CDate(centro.Rows(0)("Validita_Inizio")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(centro.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.FineAppezzamentoSuccessivaCessazioneCentroAziendale & " (" & CDate(centro.Rows(0)("Validita_Fine")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If


                    'controllo MOVIMENTI su qualsiasi Impianto collegato
                    Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                    objControlloAgenda.controllo_MovimentiRicettePua(Nothing, piva, sa_cod, appezza, 0, Validita_Inizio, Validita_Fine, objParametri_Server)

                    'controllo COSTI DI GESTIONE su qualsiasi Esercizio collegato
                    Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controlloCdG = objControlloCdG.controllo_CdG(Nothing, piva, sa_cod, appezza, 0, 0, Validita_Inizio, Validita_Fine, objParametri_Server)
                    If controlloCdG.errore Then
                        Dim MessaggioErroreCdG As String = ""

                        If Not controlloCdG.messaggioSpecifico Then
                            MessaggioErroreCdG &= nomeCurrent & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareDataXAppezzamentoCostiGestioneAssociati, controlloCdG.inizio_fine)
                        Else
                            MessaggioErroreCdG &= nomeCurrent & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareDataXAppezzamentoCostiGestioneAssociatiInDataX, controlloCdG.inizio_fine, controlloCdG.dataCdG)
                        End If

                        Throw New GiasException(MessaggioErroreCdG)
                    End If
                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.FineAppezzamentoPrecedenteCreazione
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] :   " & MessaggioErrore)
        End Try

    End Sub

    Private Function eliminaImpianti(ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                     GiasContext As Gias_DeveloperServer_Entities,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                     Optional NoteLog As String = "",
                                     Optional ScriviLog As Boolean = True
                                     ) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "eliminaImpianti"
        '-----------------------------------------
        ' CANCELLAZIONE IMPIANTI DA CONTROLLO DATE
        '-----------------------------------------
        Dim id_reg As New List(Of Integer)
        Dim impiantiDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti)

        Try
            For Each impianto In DatiAppezzamento.impianti
                id_reg.Add(impianto.primaryKey.codice)
            Next

            If id_reg.Count > 0 Then
                Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim sa_cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                Dim appezza = DatiAppezzamento.primaryKey.codice

                impiantiDaEliminare = (From impianti In GiasContext.Reg_Impianti
                                       Where impianti.PIVA = Piva AndAlso
                                              impianti.SA_COD = sa_cod AndAlso
                                              impianti.APPEZZA = appezza AndAlso
                                              Not id_reg.Contains(impianti.ID_REG)
                                       Select impianti).ToList()

                If impiantiDaEliminare.Count > 0 Then
                    For Each impiantoDaEliminare In impiantiDaEliminare
                        Dim codiceImpianto = impiantoDaEliminare.ID_REG
                        Dim DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto

                        If DatiAppezzamento.impianti IsNot Nothing Then
                            DatiImpianto = (From ia In DatiAppezzamento.impianti Where ia.primaryKey.codice = codiceImpianto).FirstOrDefault
                        Else
                            Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                            DatiImpianto = objImpianti_R.Leggi_Impianto_Anagrafica(impiantoDaEliminare.PIVA,
                                                                                   impiantoDaEliminare.SA_COD,
                                                                                   impiantoDaEliminare.APPEZZA,
                                                                                   impiantoDaEliminare.ID_REG,
                                                                                   True,
                                                                                   False,
                                                                                   AGRODATAINIZIO,
                                                                                   False,
                                                                                   False,
                                                                                   objParametri_Server,
                                                                                   objParametri_Server,
                                                                                   objParametri_Utenti)
                        End If

                        eliminaImpianto(impiantoDaEliminare, DatiImpianto, objParametri_Server, objParametri_Utenti, GiasContext, NoteLog:=NoteLog, ScriviLog:=ScriviLog)
                    Next
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DatiAppezzamento
    End Function

    Private Sub eliminaImpianto(impiantoDaEliminare As AgronicaCoreEntityFramework_POCO.Reg_Impianti,
                                DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                objParametri_Server As AgronicaCoreParametri,
                                objParametri_Utenti As AgronicaCoreParametri,
                                GiasContext As Gias_DeveloperServer_Entities,
                                Optional NoteLog As String = "",
                                Optional ScriviLog As Boolean = True,
                                Optional ByVal clusteringFeature As Boolean = False)

        Dim Piva = impiantoDaEliminare.PIVA
        Dim Sa_Cod = impiantoDaEliminare.SA_COD
        Dim Appezza = impiantoDaEliminare.APPEZZA
        Dim Id_Reg = impiantoDaEliminare.ID_REG


        'Prima di eliminare controllo Movimenti, Ricette e PUA
        Dim controllo As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
        'controllo.controllo_MovimentiRicettePua(Piva, Sa_Cod, Appezza, Id_Reg, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
        If controllo.controllo_MovimentiRicettexEliminazione(DatiImpianto, Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server) Then
            Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim imp = objImpiantoR.LeggiImpianto_ControlliAnagrafica(Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server)
            Dim nomeCurrent As String = ""
            If imp.Rows.Count = 1 Then
                Dim objProgettoBIZ As New AgronicaCoreAnagrafeBIZ.Progetto_W
                nomeCurrent = objProgettoBIZ.getCurrentDescrizione(imp.Rows(0), "", imp.Rows(0).Item("validita_inizio"), imp.Rows(0).Item("validita_fine"))
            End If
            Dim MessaggioErrore = nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileEliminareImpiantoRegistrazioniAssociate
            Throw New GiasException(MessaggioErrore)
        ElseIf controllo.controllo_UMA_RichiestexEliminazione(Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server) Then
            Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim imp = objImpiantoR.LeggiImpianto_ControlliAnagrafica(Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server)
            Dim nomeCurrent As String = ""
            If imp.Rows.Count = 1 Then
                Dim objProgettoBIZ As New AgronicaCoreAnagrafeBIZ.Progetto_W
                nomeCurrent = objProgettoBIZ.getCurrentDescrizione(imp.Rows(0), "", imp.Rows(0).Item("validita_inizio"), imp.Rows(0).Item("validita_fine"))
            End If
            Dim msg As String = String.Format(Gias.ImpossibileEliminareImpiantoXPraticheUMA, nomeCurrent)
            Throw New GiasException(msg)
        End If

        controllo.controllo_CancellazioneVincoliLetamazioniPUAxEliminazione(DatiImpianto, Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server)

        'ORA CANCELLIAMO I POLIGONI (SOTTO)
        'Dim objGIS As New AgronicaCoreGisBIZ.GIS_Entita_R
        'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(Piva, Sa_Cod, Appezza, 0, Id_Reg, enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA, objParametri_Server)) Then
        '    Throw New GiasException(Gias.ImpossibileCancellareImpiantoCollegatoPoligono)
        'End If

        Dim impiantiCodiciDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici)

        impiantiCodiciDaEliminare = (From impianti In GiasContext.Reg_Impianti_Codici
                                     Where impianti.PIVA = Piva AndAlso
                                          impianti.sa_cod = Sa_Cod AndAlso
                                          impianti.appezza = Appezza AndAlso
                                          impianti.Id_Reg = Id_Reg AndAlso
                                          impianti.Progetto_Cod = 0
                                     Select impianti).ToList()

        If impiantiCodiciDaEliminare.Count > 0 Then
            GiasContext.Reg_Impianti_Codici.RemoveRange(impiantiCodiciDaEliminare)
            GiasContext.SaveChanges()
        End If

        Dim PianoConcimazione_EntitaxTestata_DaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.PianoConcimazione_EntitaxTestata)
        PianoConcimazione_EntitaxTestata_DaEliminare = (From pc In GiasContext.PianoConcimazione_EntitaxTestata Where
                                                          pc.Piva = Piva _
                                                          AndAlso pc.Sa_Cod = Sa_Cod _
                                                          AndAlso pc.Appezza = Appezza _
                                                          AndAlso pc.Id_Imp = Id_Reg
                                                        Select pc).ToList()

        If PianoConcimazione_EntitaxTestata_DaEliminare.Count > 0 Then
            GiasContext.PianoConcimazione_EntitaxTestata.RemoveRange(PianoConcimazione_EntitaxTestata_DaEliminare)
            GiasContext.SaveChanges()
        End If

        ' cancella associazione impianto-macchina
        EFReg_Impianti.ScriviModificaEliminaMacchinaxImpianto(enum_TipoOperazioneDB.Cancellazione,
                                                              Piva, Sa_Cod, Appezza, Id_Reg, Nothing,
                                                              objParametri_Server.UsernameOperazione,
                                                              objParametri_Server, GiasContext, False, True)

        '-- CANCELLAZIONE POLIGONI
        Dim EntitaImpianto As New List(Of Integer) From
            {enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA, enum_GIS2012_TipoEntita.IMPIANTI_ERBACEA,
            enum_GIS2012_TipoEntita.IMPIANTO_NUDO, enum_GIS2012_TipoEntita.IMPIANTO_ARBOREA,
            enum_GIS2012_TipoEntita.IMPIANTO_GENERICO}
        Dim GIS_Entita_DaEliminare = (From a In GiasContext.GIS_Entita Where a.Piva = Piva AndAlso a.Sa_Cod = Sa_Cod AndAlso a.Appezza = Appezza AndAlso a.Id_Imp = Id_Reg AndAlso EntitaImpianto.Contains(a.TipoEntita_Cod)).ToList()
        For Each entita_DaEliminare In GIS_Entita_DaEliminare
            Dim GIS_ElementiGrafici_DaEliminare = (From a In GiasContext.GIS_ElementiGrafici Where a.Entita_Cod = entita_DaEliminare.Entita_Cod).ToList()
            GiasContext.GIS_Entita.Remove(entita_DaEliminare)
            GiasContext.GIS_ElementiGrafici.RemoveRange(GIS_ElementiGrafici_DaEliminare)

            ''eliminazione record di clustering se modulo attivo
            If clusteringFeature Then
                For Each ele In GIS_ElementiGrafici_DaEliminare
                    Dim GIS_ElementiGraficiCluster_DaEliminare = (From a In GiasContext.GIS_ElementiGrafici_Clustering Where a.ElementoGrafico_Cod = ele.ElementoGrafico_Cod).ToList()
                    If GIS_ElementiGraficiCluster_DaEliminare IsNot Nothing AndAlso GIS_ElementiGraficiCluster_DaEliminare.Count > 0 Then
                        GiasContext.GIS_ElementiGrafici_Clustering.RemoveRange(GIS_ElementiGraficiCluster_DaEliminare)
                    End If
                Next
            End If
        Next

        'Cancello anche le fasi fenologiche restituite dall'engine associate all'impianto
        Dim Impianto_Fasi_Fenologiche_Engine_DaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Impianto_Fasi_Fenologiche_Engine)
        Impianto_Fasi_Fenologiche_Engine_DaEliminare = (From ip In GiasContext.Impianto_Fasi_Fenologiche_Engine Where
                                                          ip.PIVA = Piva _
                                                          AndAlso ip.SA_COD = Sa_Cod _
                                                          AndAlso ip.APPEZZA = Appezza _
                                                          AndAlso ip.ID_REG = Id_Reg
                                                        Select ip).ToList()

        If Impianto_Fasi_Fenologiche_Engine_DaEliminare.Count > 0 Then
            GiasContext.Impianto_Fasi_Fenologiche_Engine.RemoveRange(Impianto_Fasi_Fenologiche_Engine_DaEliminare)
            GiasContext.SaveChanges()
        End If


        Dim eserciziDaEliminare = (From ip In GiasContext.Imprese_Progetti Where ip.Piva = Piva AndAlso
                                                                               ip.Sa_Cod = Sa_Cod AndAlso
                                                                               ip.Appezza = Appezza AndAlso
                                                                               ip.Id_Reg = Id_Reg).ToList

        For Each esercizioDaEliminare In eserciziDaEliminare
            Dim Progetto_Cod = esercizioDaEliminare.Progetto_Cod
            Dim objProg As New AgronicaCoreAnagrafeBIZ.Progetto_R
            Dim DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio = objProg.Leggi_Esercizio_Anagrafica(Nothing, Progetto_Cod, Nothing, objParametri_Server, objParametri_Utenti)

            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controlloCdG = objControllo.controllo_CdGxEliminazione(DatiEsercizio, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, objParametri_Server)
            If controlloCdG.errore Then
                Dim MessaggioErrore As String = ""

                Dim descrizione_imp As String = "[dal " & impiantoDaEliminare.Validita_Inizio & " al " & impiantoDaEliminare.Validita_Fine & "]"

                Dim descrizione_ese As String = esercizioDaEliminare.Progetto_Nome
                If descrizione_ese = "" Then
                    descrizione_ese = "[dal " & esercizioDaEliminare.Validita_Inizio & " al " & esercizioDaEliminare.Validita_Fine & "]"
                End If

                If Not controlloCdG.messaggioSpecifico Then
                    MessaggioErrore = ("Non è possibile eliminare l'impianto " & descrizione_imp & ", perché esistono costi di gestione associati all'esercizio " & descrizione_ese)
                Else
                    MessaggioErrore = ("Non è possibile eliminare l'impianto " & descrizione_imp & ", perché esistono costi di gestione associati all'esercizio " & descrizione_ese & " in data " & controlloCdG.dataCdG)
                End If

                Throw New GiasException(MessaggioErrore)
            Else
                EliminaEsercizio(esercizioDaEliminare, DatiEsercizio, objParametri_Server, GiasContext, NoteLog:=NoteLog, ScriviLog:=ScriviLog)
            End If
        Next

        Dim DatiImpiantoStr = ""

        If ScriviLog Then
            If DatiImpianto IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                DatiImpiantoStr = JsonConvert.SerializeObject(DatiImpianto, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                                                    CStr(Piva), CStr(Sa_Cod),
                                                                                    CStr(Appezza), CStr(Id_Reg),
                                                                                    Nothing, Nothing,
                                                                                    enum_TipoOperazioneDB.Cancellazione,
                                                                                    objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                    NoteLog, DatiImpiantoStr)

            ' funzione per cancellare il record in Imprese_Progetti
            GiasContext.Agronica_Log_Anagrafe.Add(log)
        End If
        GiasContext.Reg_Impianti.Remove(impiantoDaEliminare)
        GiasContext.SaveChanges()

    End Sub

    Private Function eliminaEsercizi(imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                     GiasContext As Gias_DeveloperServer_Entities,
                                     Optional NoteLog As String = "",
                                     Optional ScriviLog As Boolean = True
                                     ) As AgronicaCoreModelsSTD.anagrafiche.Impianto

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Appezzamento.eliminaEsercizi()"
        '-----------------------------------------
        ' CANCELLAZIONE ESERCIZI DA CONTROLLO DATE
        '-----------------------------------------
        Dim cod_progetti As New List(Of Integer)
        Dim eserciziDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Imprese_Progetti)
        Dim eserciziCodiciDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici)

        Try
            For Each ese In imp.esercizi
                cod_progetti.Add(ese.codice)
            Next

            If cod_progetti.Count > 0 Then
                eserciziDaEliminare = (From ese In GiasContext.Imprese_Progetti
                                       Where ese.Piva = imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                              ese.Sa_Cod = imp.primaryKey.appezzamentoPK.centroAziendalePK.codice AndAlso
                                              ese.Appezza = imp.primaryKey.appezzamentoPK.codice AndAlso
                                              ese.Id_Reg = imp.primaryKey.codice AndAlso
                                              Not cod_progetti.Contains(ese.Progetto_Cod)
                                       Select ese).ToList()

                If eserciziDaEliminare.Count > 0 Then
                    For Each esercizioDaEliminare In eserciziDaEliminare
                        Dim Progetto_Cod = esercizioDaEliminare.Progetto_Cod
                        Dim DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio = Nothing
                        If imp IsNot Nothing AndAlso imp.esercizi IsNot Nothing Then
                            Dim objProg As New AgronicaCoreAnagrafeBIZ.Progetto_R
                            DatiEsercizio = objProg.Leggi_Esercizio_Anagrafica(Nothing, Progetto_Cod, Nothing, objParametri_Server, objParametri_Utenti)
                        End If
                        EliminaEsercizio(esercizioDaEliminare, DatiEsercizio, objParametri_Server, GiasContext, NoteLog:=NoteLog, ScriviLog:=ScriviLog)
                    Next
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return imp
    End Function

    Private Sub EliminaEsercizio(esercizioDaEliminare As AgronicaCoreEntityFramework_POCO.Imprese_Progetti,
                                 DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                 objParametri_Server As AgronicaCoreParametri,
                                 GiasContext As Gias_DeveloperServer_Entities,
                                 Optional NoteLog As String = "",
                                 Optional ScriviLog As Boolean = True)

        Dim piva = esercizioDaEliminare.Piva
        Dim sa_cod = esercizioDaEliminare.Sa_Cod
        Dim appezza = esercizioDaEliminare.Appezza
        Dim id_reg = esercizioDaEliminare.Id_Reg
        Dim progetto_cod = esercizioDaEliminare.Progetto_Cod

        Dim objEsercizioR As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim esercizioDB = objEsercizioR.LeggiDistinta_ControlliAnagrafica(piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)
        Dim nomeCurrent As String = ""
        If esercizioDB.Rows.Count = 1 Then
            Dim objProgettoBIZ As New AgronicaCoreAnagrafeBIZ.Progetto_W
            nomeCurrent = objProgettoBIZ.getCurrentDescrizione(esercizioDB.Rows(0), esercizioDB.Rows(0).Item("Lotto"), esercizioDB.Rows(0).Item("validita_inizio"), esercizioDB.Rows(0).Item("validita_fine"))
        End If

        'Prima di eliminare controllo Movimenti, Ricette e PUA
        Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim controllo = objControllo.controllo_CdGxEliminazione(DatiEsercizio, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)
        'controllo.controllo_MovimentiRicettePua(Piva, Sa_Cod, Appezza, Id_Reg, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
        If controllo.errore Then
            Dim MessaggioErrore As String = ""
            If Not controllo.messaggioSpecifico Then
                MessaggioErrore = nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileEliminareEsercizioCdGRegistrati
            Else
                MessaggioErrore = nomeCurrent & String.Format(My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileEliminareEsercizioCdGRegistratiInDataX, controllo.dataCdG)
            End If
            Throw New GiasException(MessaggioErrore)
        End If



        Dim eserciziCodiciDaEliminare = (From ese In GiasContext.Reg_Impianti_Codici Where
                                                   ese.PIVA = piva _
                                                   And ese.sa_cod = sa_cod _
                                                   And ese.appezza = appezza _
                                                   And ese.Id_Reg = id_reg _
                                                   And ese.Progetto_Cod = progetto_cod
                                         Select ese).ToList()

        If eserciziCodiciDaEliminare.Count > 0 Then
            GiasContext.Reg_Impianti_Codici.RemoveRange(eserciziCodiciDaEliminare)
            GiasContext.SaveChanges()
        End If

        Dim DatiEsercizioStr = ""

        If ScriviLog Then
            If DatiEsercizio IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                DatiEsercizioStr = JsonConvert.SerializeObject(DatiEsercizio, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                                                 CStr(piva), CStr(progetto_cod), CStr(sa_cod),
                                                                                 CStr(appezza), CStr(id_reg), Nothing,
                                                                                 enum_TipoOperazioneDB.Cancellazione,
                                                                                 objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                 NoteLog, DatiEsercizioStr)

            ' funzione per cancellare il record in Imprese_Progetti
            GiasContext.Agronica_Log_Anagrafe.Add(log)
        End If
        GiasContext.Imprese_Progetti.Remove(esercizioDaEliminare)
        GiasContext.SaveChanges()
    End Sub

    Public Sub BloccaSbloccaAppezzamenti(appezzamenti As List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento),
                                         blocca As Boolean,
                                         ByRef objParametri_Server As AgronicaCoreParametri)

        Using scope As New TransactionScope()
            Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                For Each objAppezzamento In appezzamenti
                    Dim Piva = objAppezzamento.primaryKey.centroAziendalePK.partitaIva
                    Dim Sa_Cod = objAppezzamento.primaryKey.centroAziendalePK.codice
                    Dim Appezza = objAppezzamento.primaryKey.codice
                    Dim appezzamento = (From a In GiasContext.Appezzamento
                                        Where a.PIVA = Piva AndAlso
                                              a.SA_COD = Sa_Cod AndAlso
                                              a.APPEZZA = Appezza).FirstOrDefault

                    If appezzamento IsNot Nothing Then
                        appezzamento.Username_Modifica = objParametri_Server.UsernameOperazione
                        appezzamento.Data_Modifica = DateTime.Now()
                        If blocca Then
                            appezzamento.Blk_Flag = -1
                            appezzamento.Blk_Inizio_Data = Date.Now
                            appezzamento.Blk_Inizio_Username = objParametri_Server.UsernameOperazione
                            appezzamento.Blk_Inizio_Note = ""
                            appezzamento.Blk_Fine_Data = AGRODATAFINE
                            appezzamento.Blk_Fine_Username = objParametri_Server.UsernameOperazione
                            appezzamento.Blk_Fine_Note = ""
                        Else
                            appezzamento.Blk_Flag = 0
                            appezzamento.Blk_Inizio_Data = AGRODATAINIZIO
                            appezzamento.Blk_Inizio_Username = ""
                            appezzamento.Blk_Inizio_Note = ""
                            appezzamento.Blk_Fine_Data = AGRODATAFINE
                            appezzamento.Blk_Fine_Username = ""
                            appezzamento.Blk_Fine_Note = ""
                        End If
                        GiasContext.Appezzamento.Attach(appezzamento)
                        GiasContext.Entry(appezzamento).State = EntityState.Modified

                    End If

                Next

                GiasContext.SaveChanges()
                scope.Complete()
                scope.Dispose()

            End Using
        End Using

    End Sub

    Public Function Scrivi_Appezzamento_APP(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            Optional NoteLog As String = "Operazione Registrata da APP"
                                            ) As Boolean

        If appezzamento.validita Is Nothing Then
            appezzamento.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
        End If

        Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim objAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

        Dim esito = objAppezzamentoW.Appezzamento_ScriviModifica(
                appezzamento,
                objParametri_Server,
                objParametri_Utenti,
                NoteLog:=NoteLog)

        If esito Then

            If Not appezzamento.flag_cancellazione Then

                appezzamento = objAppezzamentoR.Leggi_Appezzamento_Anagrafica(
                        appezzamento.primaryKey.centroAziendalePK.partitaIva,
                        appezzamento.primaryKey.centroAziendalePK.codice,
                        appezzamento.primaryKey.codice,
                        0,
                        True,
                        True,
                        True,
                        AGRODATAINIZIO,
                        False,
                        True,
                        True,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti)

                'Valorizzazione_Geojson_NodeInfo_Appezzamento(
                '    obj_Appezzamento,
                '    iData,
                '    objParametri_Server,
                '    objParametri_Utenti)

            End If

        End If

        Return esito

    End Function

    Public Function CopiaSpostaAppezzamenti(ListaAppezzamenti As List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento),
                                            NuovoCentro As CentroAziendale,
                                            SpostaEdElimina As Boolean,
                                            CopiaCatasto As Boolean,
                                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As String

        Dim res As String = ""

        Dim objAppezzamento_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

        Dim msgImpossibileSpostarexAgenda As New List(Of String)
        Dim msgImpossibileSpostarexCDG As New List(Of String)
        Dim msgErrore As New List(Of String)

        Dim NoteLog = "CopiaSpostaAppezzamenti " & If(SpostaEdElimina, "[SPOSTA]", "[COPIA]")

        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim scope As TransactionScope = Nothing

        Dim desApp As String

        ''' <summary>
        ''' key: PIVA-SA_COD-APPEZZA
        ''' value: username, data
        ''' </summary>
        Dim datiCreazioneOrignalexApp As New Dictionary(Of (String, Integer, Integer), (String, Date))

        ''' <summary>
        ''' key1: PIVA-SA_COD-APPEZZA,
        ''' key2: validita_inizio-validita_fine
        ''' value: username, data
        ''' </summary>
        Dim userCreazioneOrignalexImp As New Dictionary(Of ((String, Integer, Integer), (Date, Date)), (String, Date))

        ''' <summary>
        ''' key1: PIVA-SA_COD-APPEZZA,
        ''' key2: validita_inizio-validita_fine
        ''' value: username, data
        ''' </summary>
        Dim userCreazioneOrignalexEse As New Dictionary(Of ((String, Integer, Integer), (Date, Date)), (String, Date))

        For Each originalApp In ListaAppezzamenti

            desApp = ""

            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            scope = New TransactionScope()

            Try
                If ListaAppezzamenti.IndexOf(originalApp) = 0 Then
                    Dim listPiva = ListaAppezzamenti.Select(Function(x) x.primaryKey.centroAziendalePK.partitaIva).Distinct.ToList()
                    Dim listSaCod = ListaAppezzamenti.Select(Function(x) x.primaryKey.centroAziendalePK.codice).Distinct.ToList()
                    Dim listAppezza = ListaAppezzamenti.Select(Function(x) x.primaryKey.codice).Distinct.ToList()
                    'Estraggo tutti gli utenti Creazione Originali
                    Dim dummyList = (From appezzamenti In GiasContext.Appezzamento
                                     Join impianti In GiasContext.Reg_Impianti On
                                         impianti.PIVA Equals appezzamenti.PIVA And
                                         impianti.SA_COD Equals appezzamenti.SA_COD And
                                         impianti.APPEZZA Equals appezzamenti.APPEZZA
                                     Join esercizi In GiasContext.Imprese_Progetti On
                                         esercizi.Piva Equals impianti.PIVA And
                                         esercizi.Sa_Cod Equals impianti.SA_COD And
                                         esercizi.Appezza Equals impianti.APPEZZA And
                                         esercizi.Id_Reg Equals impianti.ID_REG
                                     Where listPiva.Contains(appezzamenti.PIVA) AndAlso
                                         listSaCod.Contains(appezzamenti.SA_COD) AndAlso
                                         listAppezza.Contains(appezzamenti.APPEZZA)
                                     Select New With {
                                         appezzamenti.PIVA, appezzamenti.SA_COD, appezzamenti.APPEZZA, .appUsername = appezzamenti.Username_Creazione, .appData = appezzamenti.Data_Creazione,
                                         .impInizio = impianti.Validita_Inizio, .impFine = impianti.Validita_Fine, .impUsername = impianti.Username_Creazione, .impData = impianti.Data_Creazione,
                                         .eseInizio = esercizi.Validita_Inizio, .eseFine = esercizi.Validita_Fine, .eseUsername = esercizi.Username_Creazione, .eseData = esercizi.Data_Creazione
                                         }).ToList()

                    For Each dummy In dummyList
                        If Not datiCreazioneOrignalexApp.ContainsKey((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA)) Then
                            datiCreazioneOrignalexApp.Add((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA), (dummy.appUsername, dummy.appData))
                        End If
                        If Not userCreazioneOrignalexImp.ContainsKey(((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA), (dummy.impInizio, dummy.impFine))) Then
                            userCreazioneOrignalexImp.Add(((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA), (dummy.impInizio, dummy.impFine)), (dummy.impUsername, dummy.impData))
                        End If
                        If Not userCreazioneOrignalexEse.ContainsKey(((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA), (dummy.impInizio, dummy.impFine))) Then
                            userCreazioneOrignalexEse.Add(((dummy.PIVA, dummy.SA_COD, dummy.APPEZZA), (dummy.eseInizio, dummy.eseFine)), (dummy.eseUsername, dummy.eseData))
                        End If
                    Next
                End If

                '-------------------------------
                'LEGGO TUTTI I DATI DELL'APPEZZAMENTO DA COPIARE 
                '-------------------------------
                Dim objApp = objAppezzamento_R.Leggi_Appezzamento_Anagrafica(
                            originalApp.primaryKey.centroAziendalePK.partitaIva,
                            originalApp.primaryKey.centroAziendalePK.codice,
                            originalApp.primaryKey.codice, 0,
                            True, True,
                            CopiaCatasto,
                            AGRODATAINIZIO, False,
                            True, True,
                            objParametri_Super_Server,
                            objParametri_Server,
                            objParametri_Utenti)

                desApp = objApp.descrizione

                '-------------------------------
                'RIPORTO LA CHIAVE DEL CAMPO SE ESISTE,
                'altrimenti si schianta in cancellazione
                '-------------------------------
                originalApp.campoPK = objApp.campoPK


                If objApp IsNot Nothing AndAlso objApp.impianti IsNot Nothing Then
                    Dim procediCopia = True

                    If SpostaEdElimina Then
                        If objApp.impianti IsNot Nothing Then

                            '-------------------------------------
                            ' CONTROLLI SU OPERAZIONI E CDG X OGNI IMPIANTO DA SPOSTARE
                            ' Se ne esistono di collegati:
                            ' - Non copio l'appezzamento
                            ' - Avverto l'utente
                            '-------------------------------------
                            For Each objImp In objApp.impianti
                                Dim PIVA = objImp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                                Dim SA_COD = objImp.primaryKey.appezzamentoPK.centroAziendalePK.codice
                                Dim APPEZZA = objImp.primaryKey.appezzamentoPK.codice
                                Dim ID_REG = objImp.primaryKey.codice

                                '-------------------------------------
                                ' CONTROLLI OPERAZIONI AGENDA
                                '-------------------------------------
                                Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                                If objControlloAgenda.controllo_MovimentiRicettexEliminazione(Nothing, PIVA, SA_COD, APPEZZA, ID_REG, objParametri_Server) Then
                                    msgImpossibileSpostarexAgenda.Add(objApp.descrizione)

                                    procediCopia = False
                                    Continue For
                                End If

                                '-------------------------------------
                                ' CONTROLLI CGD
                                '-------------------------------------
                                Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                                Dim controlloCdG = objControlloCdG.controllo_CdGxEliminazione(Nothing, PIVA, SA_COD, APPEZZA, ID_REG, 0, objParametri_Server)

                                If controlloCdG.errore Then
                                    msgImpossibileSpostarexCDG.Add(objApp.descrizione)

                                    procediCopia = False
                                    Continue For
                                End If
                            Next
                        End If
                    End If

                    If procediCopia Then
                        '-------------------------------------
                        ' REIMPOSTO TUTTE LE CHIAVI
                        ' APPEZZAMENTO - CAMPO - IMPIANTO - ESERCIZIO
                        '-------------------------------------
                        objApp.primaryKey.centroAziendalePK = NuovoCentro.primaryKey
                        objApp.campoPK.codice = 0
                        objApp.primaryKey.codice = 0

                        For Each objImp In objApp.impianti
                            objImp.primaryKey.appezzamentoPK.centroAziendalePK = NuovoCentro.primaryKey
                            objImp.primaryKey.codice = 0

                            For Each objEse In objImp.esercizi
                                objEse.impiantoPK.appezzamentoPK.centroAziendalePK = NuovoCentro.primaryKey
                                objEse.codice = 0
                            Next
                        Next

                        Dim CopiaSposta_preserveDataUsernameCreazioneOriginali As New CopiaSposta_preserveDataUsernameCreazioneOriginali With {
                            .Username_Creazione_Appezzamento = datiCreazioneOrignalexApp((originalApp.primaryKey.centroAziendalePK.partitaIva, originalApp.primaryKey.centroAziendalePK.codice, originalApp.primaryKey.codice)).Item1,
                            .Data_Creazione_Appezzamento = datiCreazioneOrignalexApp((originalApp.primaryKey.centroAziendalePK.partitaIva, originalApp.primaryKey.centroAziendalePK.codice, originalApp.primaryKey.codice)).Item2
                        }

                        For Each imp In userCreazioneOrignalexImp
                            If imp.Key.Item1.Item1 = originalApp.primaryKey.centroAziendalePK.partitaIva AndAlso
                                imp.Key.Item1.Item2 = originalApp.primaryKey.centroAziendalePK.codice AndAlso
                                imp.Key.Item1.Item3 = originalApp.primaryKey.codice AndAlso Not CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexImp.ContainsKey((imp.Key.Item2.Item1, imp.Key.Item2.Item2)) Then

                                CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexImp.Add((imp.Key.Item2.Item1, imp.Key.Item2.Item2), (imp.Value.Item1, imp.Value.Item2))
                            End If
                        Next
                        For Each ese In userCreazioneOrignalexEse
                            If ese.Key.Item1.Item1 = originalApp.primaryKey.centroAziendalePK.partitaIva AndAlso
                                ese.Key.Item1.Item2 = originalApp.primaryKey.centroAziendalePK.codice AndAlso
                                ese.Key.Item1.Item3 = originalApp.primaryKey.codice AndAlso Not CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexEse.ContainsKey((ese.Key.Item2.Item1, ese.Key.Item2.Item2)) Then

                                CopiaSposta_preserveDataUsernameCreazioneOriginali.datiCreazioneOrignalexEse.Add((ese.Key.Item2.Item1, ese.Key.Item2.Item2), (ese.Value.Item1, ese.Value.Item2))
                            End If
                        Next

                        objAppezzamento_W.Appezzamento_ScriviModifica(objApp,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti,
                                                                      GiasContext, OpenNewTransaction:=False,
                                                                      NoteLog:=NoteLog,
                                                                      CopiaSposta_preserveDataUsernameCreazioneOriginali:=CopiaSposta_preserveDataUsernameCreazioneOriginali)

                        If CopiaCatasto Then
                            '-------------------------------------
                            ' CONTROLLO SE ESISTE PARTICELLA NEL NUOVO CENTRO
                            '-------------------------------------
                            If objApp.catastoAppezzamento IsNot Nothing Then
                                For Each p In objApp.catastoAppezzamento
                                    Dim PROV = p.particella.Prov
                                    Dim COM = p.particella.Com
                                    Dim SEZIONE = p.particella.Sezione
                                    Dim FOGLIO = p.particella.Foglio
                                    Dim NUMERO = p.particella.Numero
                                    Dim SUBALTERNO = p.particella.Subalterno

                                    Dim impreseXParticelle = (From x In GiasContext.ImpreseXParticelle
                                                              Where x.PIVA = NuovoCentro.primaryKey.partitaIva AndAlso
                                                                      x.sa_cod = NuovoCentro.primaryKey.codice AndAlso
                                                                      x.PROV = PROV AndAlso x.COM = COM AndAlso
                                                                      x.SEZIONE = SEZIONE AndAlso x.FOGLIO = FOGLIO AndAlso
                                                                      x.NUMERO = NUMERO AndAlso x.SUBALTERNO = SUBALTERNO
                                                              Select x).FirstOrDefault()

                                    '-------------------------------------
                                    ' SE NON ESISTE COPIO IL COLLEGAMENTO
                                    '-------------------------------------
                                    If IsNothing(impreseXParticelle) Then
                                        '-------------------------------------
                                        ' LEGGO LA PARTCELLA DA COPIARE
                                        '-------------------------------------
                                        Dim objParticella_R As New AgronicaCoreAnagrafeBIZ.Particella_R
                                        Dim particella = objParticella_R.Leggi_Particella_Anagrafica(
                                                                        originalApp.primaryKey.centroAziendalePK.partitaIva,
                                                                        originalApp.primaryKey.centroAziendalePK.codice,
                                                                        Prov:=PROV,
                                                                        Com:=COM,
                                                                        Sezione:=SEZIONE,
                                                                        Foglio:=FOGLIO,
                                                                        Numero:=NUMERO,
                                                                        Subalterno:=SUBALTERNO,
                                                                        Leggi_Metodi_Produzione:=True,
                                                                        Leggi_Macrousi:=True,
                                                                        Leggi_Zone:=True,
                                                                        Leggi_Classamento:=True,
                                                                        objParametri_Server)

                                        '-------------------------------------
                                        ' REIMPOSTO LE CHIAVI DEL CENTRO                                        ' 
                                        '-------------------------------------
                                        particella.centro = NuovoCentro.primaryKey
                                        For Each possesso In particella.possessiParticella
                                            possesso.codice = 0
                                        Next
                                        '-------------------------------------
                                        ' SCRIVO LA PARTICELLA 
                                        '-------------------------------------
                                        Dim objParticella_W As New AgronicaCoreAnagrafeBIZ.Particella_W
                                        objParticella_W.ParticellaCatasto_Scrivi(particella, Nothing,
                                                                                     objParametri_Server, objParametri_Utenti,
                                                                                     False, NoteLog:=NoteLog)
                                    End If
                                Next
                            End If
                        End If

                        If SpostaEdElimina Then

                            originalApp.flag_cancellazione = True
                            objAppezzamento_W.Appezzamento_ScriviModifica(originalApp,
                                                                              objParametri_Server,
                                                                              objParametri_Utenti,
                                                                              GiasContext, OpenNewTransaction:=False,
                                                                              NoteLog:=NoteLog)
                        End If

                    End If
                Else
                    'Non dovrebbe mai entrare qui
                    Throw New Exception("Appezzamento o impianto non trovato.")
                End If

                GiasContext.SaveChanges()
                scope.Complete()

            Catch ex As GiasException
                If desApp <> "" Then
                    msgErrore.Add($"{desApp} : {ex.Message}")
                End If
            Finally
                scope.Dispose()
                GiasContext.Dispose()
            End Try
        Next

        Dim msg As String = ""
        If msgImpossibileSpostarexAgenda.Count > 0 Then
            res &= Gias.NonPossibileSpostareSeguentiAppezzamentiEsistonoOperazioniRegistrateSugliImpianti & ":" & NEWLINE
            res &= "- " & String.Join(NEWLINE & "- ", msgImpossibileSpostarexAgenda) & NEWLINE
        End If

        If msgImpossibileSpostarexCDG.Count > 0 Then
            res &= Gias.NonPossibileSpostareSeguentiAppezzamentiEsistonoCostiGestioneAssociatiAgliEsercizi & ":" & NEWLINE
            res &= "- " & String.Join(NEWLINE & "- ", msgImpossibileSpostarexCDG) & NEWLINE
        End If

        If msgErrore.Count > 0 Then
            res &= My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileCopiaSpostaApezzamentiErroriNonGestiti & ":" & NEWLINE
            res &= "- " & String.Join(NEWLINE & "- ", msgErrore)
        End If

        Return res

    End Function

    Public Shared Function Internal_Delete_Appezzamento_Reale_Da_Ribaltamento(Ribaltamento As Ribaltamento_Appezzamento,
                                                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                                  Optional GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                                  Optional OpenNewTransaction As Boolean = True,
                                                                                  Optional NoteLog As String = "") As String

        Dim msgErrore As String = ""

        If NoteLog = "" Then
            NoteLog = "Cancellato da Eliminazione Impianto Budget (" & Ribaltamento.Budget_Piva & "-" & Ribaltamento.Budget_Sa_Cod & "-" & Ribaltamento.Budget_Appezza & "), Id_Budget: " & Ribaltamento.Budget_Id_Testata
        End If

        Dim appezzamento As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento
        appezzamento.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(Ribaltamento.Reale_Appezza, New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Ribaltamento.Reale_Sa_Cod, Ribaltamento.Reale_Piva))
        appezzamento.flag_cancellazione = True

        Try
            Dim objAppezzamentoReale_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            objAppezzamentoReale_W.Appezzamento_ScriviModifica(appezzamento,
                                                               objParametri_Server, objParametri_Utenti,
                                                               GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                               NoteLog:=NoteLog)

        Catch ex As GiasException
            msgErrore = Gias.ImpossibileCancellareImpiantoRibaltatoPianoColturaleEffettivoEsistonoMovimentiAssociati
        Catch ex As Exception
            msgErrore = Gias.ImpossibileCancellareImpiantoRibaltatoPianoColturaleEffettivo & ": " & ex.Message
        End Try

        Return msgErrore

    End Function


#Region "PlotWeaving"
    Public Function UpdateWeaving(plots As List(Of anagrafiche.Appezzamento),
                                  objServer As AgronicaCoreParametri,
                                  objUtenti As AgronicaCoreParametri,
                                  Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                                  Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.UpdateWeaving()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAppezzamentiTessitura,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            objUtenti
            )

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objServer.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            For Each plt In plots
                objDAL.UpdateWeaving(
                    plt.primaryKey.centroAziendalePK.partitaIva,
                    plt.primaryKey.centroAziendalePK.codice,
                    plt.primaryKey.codice,
                    plt.argilla,
                    plt.sabbia,
                    plt.limo,
                    CStr(plt.classeTessitura.codice),
                    objServer
                    )

                Dim objTessitura = New With {
                    .argilla = plt.argilla,
                    .sabbia = plt.sabbia,
                    .limo = plt.limo,
                    .codice = CStr(plt.classeTessitura.codice)
                }

                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Appezza,
                                                                                     CStr(plt.primaryKey.centroAziendalePK.partitaIva),
                                                                                     CStr(plt.primaryKey.centroAziendalePK.codice),
                                                                                     CStr(plt.primaryKey.codice), Nothing,
                                                                                     Nothing, Nothing,
                                                                                     enum_TipoOperazioneDB.Modifica,
                                                                                     objServer,
                                                                                     enum_Id_Servizio.GiasOnline,
                                                                                     "Update Tessitura Terreno: " & CStr(JsonConvert.SerializeObject(objTessitura)),
                                                                                     JsonConvert.SerializeObject(plt))

                Dim reg_impianti = (From ri In giasContext.Reg_Impianti
                                    Where ri.PIVA = plt.primaryKey.centroAziendalePK.partitaIva AndAlso
                                        ri.SA_COD = plt.primaryKey.centroAziendalePK.codice AndAlso
                                        ri.APPEZZA = plt.primaryKey.codice).ToList()

                For Each imp In reg_impianti
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.AggiornaDataModificaImpianto(
                                                            imp.PIVA,
                                                            imp.SA_COD,
                                                            imp.APPEZZA,
                                                            imp.ID_REG,
                                                            objServer,
                                                            giasContext,
                                                            openNewTransaction,
                                                            False)
                Next

                Dim esercizi = (From ri In giasContext.Imprese_Progetti
                                Where ri.Piva = plt.primaryKey.centroAziendalePK.partitaIva AndAlso
                                        ri.Sa_Cod = plt.primaryKey.centroAziendalePK.codice AndAlso
                                        ri.Appezza = plt.primaryKey.codice).ToList()

                For Each ese In esercizi
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.AggiornaDataModificaEsercizio(
                                                            ese.Piva,
                                                            ese.Progetto_Cod,
                                                            objServer,
                                                            giasContext,
                                                            openNewTransaction,
                                                            False)
                Next

                giasContext.Agronica_Log_Anagrafe.Add(log)

            Next

            giasContext.SaveChanges()

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function
#End Region

#Region "PlotSlope"
    Public Function UpdateSlope(plots As List(Of anagrafiche.Appezzamento),
                                objServer As AgronicaCoreParametri,
                                objUtenti As AgronicaCoreParametri,
                                Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                                Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.UpdateSlope()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAppezzamentiPendenza,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            objUtenti
            )

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objServer.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            For Each plt In plots
                objDAL.UpdateSlope(
                    plt.primaryKey.centroAziendalePK.partitaIva,
                    plt.primaryKey.centroAziendalePK.codice,
                    plt.primaryKey.codice,
                    plt.pendenza,
                    objServer
                    )

                Dim reg_impianti = (From ri In giasContext.Reg_Impianti
                                    Where ri.PIVA = plt.primaryKey.centroAziendalePK.partitaIva AndAlso
                                        ri.SA_COD = plt.primaryKey.centroAziendalePK.codice AndAlso
                                        ri.APPEZZA = plt.primaryKey.codice).ToList()

                For Each imp In reg_impianti
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.AggiornaDataModificaImpianto(
                                                            imp.PIVA,
                                                            imp.SA_COD,
                                                            imp.APPEZZA,
                                                            imp.ID_REG,
                                                            objServer,
                                                            giasContext,
                                                            openNewTransaction,
                                                            False)
                Next

                Dim esercizi = (From ri In giasContext.Imprese_Progetti
                                Where ri.Piva = plt.primaryKey.centroAziendalePK.partitaIva AndAlso
                                        ri.Sa_Cod = plt.primaryKey.centroAziendalePK.codice AndAlso
                                        ri.Appezza = plt.primaryKey.codice).ToList()

                For Each ese In esercizi
                    AgronicaCoreAnagrafeDAL.EFReg_Impianti.AggiornaDataModificaEsercizio(
                                                            ese.Piva,
                                                            ese.Progetto_Cod,
                                                            objServer,
                                                            giasContext,
                                                            openNewTransaction,
                                                            False)
                Next

                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Appezza,
                                                                                     CStr(plt.primaryKey.centroAziendalePK.partitaIva),
                                                                                     CStr(plt.primaryKey.centroAziendalePK.codice),
                                                                                     CStr(plt.primaryKey.codice), Nothing,
                                                                                     Nothing, Nothing,
                                                                                     enum_TipoOperazioneDB.Modifica,
                                                                                     objServer,
                                                                                     enum_Id_Servizio.GiasOnline,
                                                                                     "Update Pendenza Terreno: " & CStr(plt.pendenza),
                                                                                     JsonConvert.SerializeObject(plt))

                giasContext.Agronica_Log_Anagrafe.Add(log)

            Next

            giasContext.SaveChanges()

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function
#End Region

#Region "PlotConstrain"
    Public Function UpdateConstrain(plots As List(Of anagrafiche.Appezzamento),
                                    serverParams As AgronicaCoreParametri,
                                    userParams As AgronicaCoreParametri,
                                    Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                                    Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.Appezzamento_W.UpdateConstrain()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            userParams.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneEserciziVincoli,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            userParams
            )

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(serverParams.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            For Each plt In plots
                ' 1 - Check data validity
                Verifica_MetodoProduzione(plt, serverParams)

                ' 2 - Save plot use of land
                AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(
                    enum_TipoOperazioneDB.Modifica,
                    plt.primaryKey.centroAziendalePK.partitaIva,
                    plt.primaryKey.centroAziendalePK.codice,
                    plt.primaryKey.codice,
                    enum_CodiciAnagrafe.MetodoDiProduzione,
                    plt.metodo_Produzione.codice,
                    False,
                    serverParams.UtenteUsername,
                    serverParams,
                    giasContext,
                    openNewTransaction,
                    False)

                AgronicaCoreAnagrafeDAL.EFAppezzamento.AggiornaDataModificaAppezzamento(
                    plt.primaryKey.centroAziendalePK.partitaIva,
                    plt.primaryKey.centroAziendalePK.codice,
                    plt.primaryKey.codice,
                    serverParams,
                    giasContext,
                    openNewTransaction,
                    False)

                Dim logAppezza As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Appezza,
                                                                     CStr(plt.primaryKey.centroAziendalePK.partitaIva),
                                                                     CStr(plt.primaryKey.centroAziendalePK.codice),
                                                                     CStr(plt.primaryKey.codice), Nothing,
                                                                     Nothing, Nothing,
                                                                     enum_TipoOperazioneDB.Modifica,
                                                                     serverParams,
                                                                     enum_Id_Servizio.GiasOnline,
                                                                     "Aggiorna Vincoli: " & CStr(plt.metodo_Produzione.codice),
                                                                     JsonConvert.SerializeObject(plt))

                giasContext.Agronica_Log_Anagrafe.Add(logAppezza)

                ' 3 - Save exercise constrain
                For Each plant In plt.impianti
                    If plant.utilizzoTerreno.classType <> "DestinazioneUso" Then


                        For Each ex In plant.esercizi
                            Dim regolamentoCod As Integer
                            Dim disciplinareCod As Integer
                            Dim disciplinarePubblicoPrivato As Integer
                            Dim regolamentoConcimazioniCod As Integer

                            If ex.vincolo IsNot Nothing Then
                                ex.disciplinare = ex.vincolo.disciplinare
                                ex.regolamento = ex.vincolo.regolamento
                                ex.apportiMassimiMacroelementi.pianoConcimazione = ex.vincolo.disciplinare.regolamentoConcimazione
                            End If

                            If ex.disciplinare Is Nothing Then
                                disciplinareCod = 0
                            ElseIf IsNumeric(ex.disciplinare.codice) Then
                                disciplinareCod = Integer.Parse(ex.disciplinare.codice)
                            ElseIf ex.disciplinare.codice.Split("/").Length = 4 Then
                                disciplinareCod = Integer.Parse(ex.disciplinare.codice.Split("/")(0))
                            End If

                            disciplinarePubblicoPrivato = If(ex.disciplinare Is Nothing, 0, ex.disciplinare.disciplinarePubblicoPrivato)

                            If ex.apportiMassimiMacroelementi IsNot Nothing Then
                                regolamentoConcimazioniCod = If(ex.apportiMassimiMacroelementi.pianoConcimazione Is Nothing, 0, ex.apportiMassimiMacroelementi.pianoConcimazione.codice)
                            End If

                            If (ex.regolamento IsNot Nothing AndAlso ex.regolamento.codice > 0) Then
                                regolamentoCod = ex.regolamento.codice
                            Else
                                regolamentoCod = 1
                            End If

                            objDAL.UpdateConstrain(
                                regolamentoCod,
                                disciplinareCod,
                                disciplinarePubblicoPrivato,
                                regolamentoConcimazioniCod,
                                ex.impiantoPK.piva,
                                ex.codice,
                                serverParams
                                )

                            AgronicaCoreAnagrafeDAL.EFReg_Impianti.AggiornaDataModificaImpianto(
                                                            ex.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                            ex.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                            ex.impiantoPK.appezzamentoPK.codice,
                                                            ex.impiantoPK.codice,
                                                            serverParams,
                                                            giasContext,
                                                            openNewTransaction,
                                                            False)

                            Dim logImpianto = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                            ex.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva, ex.codice,
                                            ex.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                            ex.impiantoPK.appezzamentoPK.codice,
                                            ex.impiantoPK.codice,
                                            Nothing,
                                            enum_TipoOperazioneDB.Modifica,
                                            serverParams, enum_Id_Servizio.GiasOnline,
                                            "Aggiorna Vincoli",
                                            JsonConvert.SerializeObject(ex))

                            giasContext.Agronica_Log_Anagrafe.Add(logImpianto)

                        Next

                    End If
                Next
            Next

            giasContext.SaveChanges()

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function
#End Region

    'Esegue un'operazione di Update o Insert sulla tabella GIS_ElementiGrafici_Clustering.
    'Viene chiamata solo se la feature di clustering è attiva.
    Private Sub ClusteringRecord(
        ByVal elementoGrafico As GIS_ElementiGrafici,
        ByVal username As String,
        ByRef GiasContext As Gias_DeveloperServer_Entities
    )
        If elementoGrafico Is Nothing Then Return

        Dim clusteringRecord = GiasContext.GIS_ElementiGrafici_Clustering.Find(
            elementoGrafico.PivaSuperUser,
            elementoGrafico.LayerElementiGrafici_Cod,
            elementoGrafico.ElementoGrafico_Cod
        )

        If clusteringRecord Is Nothing Then
            clusteringRecord = New GIS_ElementiGrafici_Clustering() With {
                .PivaSuperUser = elementoGrafico.PivaSuperUser,
                .LayerElementoGrafico_Cod = elementoGrafico.LayerElementiGrafici_Cod,
                .ElementoGrafico_Cod = elementoGrafico.ElementoGrafico_Cod,
                .Username_Creazione = username,
                .Data_Creazione = DateTime.Now
        }
            clusteringRecord.Username_Modifica = username
            clusteringRecord.Data_Modifica = DateTime.Now
            clusteringRecord.Stato = 0
            clusteringRecord.Centroide_GeoEntity_WKT = Nothing
            clusteringRecord.Centroide_GeoEntity = Nothing
            GiasContext.GIS_ElementiGrafici_Clustering.Add(clusteringRecord)
        Else
            clusteringRecord.Username_Modifica = username
            clusteringRecord.Data_Modifica = DateTime.Now
            clusteringRecord.Stato = 0
            GiasContext.GIS_ElementiGrafici_Clustering.Attach(clusteringRecord)
            GiasContext.Entry(clusteringRecord).State = Entity.EntityState.Modified
        End If



    End Sub
End Class

Public Class CopiaSposta_preserveDataUsernameCreazioneOriginali
    Public Property Username_Creazione_Appezzamento As String = ""
    Public Property Data_Creazione_Appezzamento As Date

    ''' <summary>
    ''' key: validita_inizio-validita_fine
    ''' value: username, data
    ''' </summary>
    Public Property datiCreazioneOrignalexImp As New Dictionary(Of (Date, Date), (String, Date))

    ''' <summary>
    ''' key: validita_inizio-validita_fine
    ''' value: username, data
    ''' </summary>
    Public Property datiCreazioneOrignalexEse As New Dictionary(Of (Date, Date), (String, Date))
End Class