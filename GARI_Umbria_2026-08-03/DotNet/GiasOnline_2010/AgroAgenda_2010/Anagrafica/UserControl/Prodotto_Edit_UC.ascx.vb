Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabBIZ.Prodotti_Costi_W
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Imports System.Web.Services
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.My.Resources

Public Class Prodotto_Edit_UC
    Inherits System.Web.UI.UserControl
    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Public Shared listModuliAttivi_anagrafe_log As New List(Of Integer)

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()

        Try
            listModuliAttivi_anagrafe_log = New List(Of Integer)

        Catch ex As Exception

            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " & Chr(13) & ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Public Shared Function Leggi_Specie() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecie = SpecieVegetali_R.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtSpecie, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim filtro_aggiuntivo As String = ""
            If Trim(filtro_specie) <> "" Then
                filtro_aggiuntivo = " Cultivar.Veg_Cod IN (" & filtro_specie & ") "
            End If

            Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim dtVarieta = Cultivar_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_aggiuntivo, "", objParametri_Server)

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtVarieta.Rows
                jArrayParamQual.Add(New JObject(New JProperty("Cul_Cod", dr.Item("Cul_Cod")), New JProperty("Cul_Des", dr.Item("Veg_Des") & " - " & dr.Item("Cul_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_Categorie_Commerciali(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objCC As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R
            Dim dtCategCommle = objCC.Leggi(piva, 0, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtCategCommle, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_Gruppi_Merce(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim GruppiMerceList As New List(Of Object)

            Dim objGM As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R
            Dim dtGruppiMerce = objGM.Leggi_con_Visibilita(piva, "", "", objParametriServer)


            For Each row As DataRow In dtGruppiMerce.Rows

                GruppiMerceList.Add(New With
                                {
                                     .Id_Gruppo_Merce = CInt(row("Id_Gruppo_Merce").ToString()),
                                     .Descrizione_Concatenata = row("Codice").ToString() & " " & row("Descrizione").ToString()
                                })

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(GruppiMerceList, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function EditProdotto(ByVal piva As String, ByVal mat_cod As Integer, ByVal elem_cod As Integer, ByVal duplica As Boolean, ByVal proprietario As Boolean, ByVal sa_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dtProdotto As DataTable
            Dim objMateria As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            dtProdotto = objMateria.LeggiMateriePrimexSuperUser(objParametriServer.PivaSuperUser,
                                                        piva,
                                                        CInt(elem_cod),
                                                        CInt(mat_cod),
                                                        0, "", True,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                        "", "", objParametriServer)

            'Faccio una lista con l'elenco delle tab e i loro rispettivi dati,per evitare dopo se sono in duplica di 
            'rifare la lettura.
            Dim tabdaDuplicare As New List(Of Object)

            'Se duplica è true allora mi carico anche tutti i dati relativi a quella materia_prima
            If duplica AndAlso dtProdotto.Rows.Count > 0 Then

                Dim DT As New DataTable


                '#####Lettura Costi Tab Storico Prezzi####
                Dim objCosti As New Prodotti_Costi_R
                Dim Piva_Proprietaria As String = piva

                If Not proprietario Then
                    Dim objParametriAgenda As New ParametriAgenda
                    Piva_Proprietaria = objParametriAgenda.Piva
                End If

                Dim FiltroperPiva As String = "Prodotti_Costi.piva = '" & Agro_SQL_SaveText(Piva_Proprietaria) & "' And Prodotti_Costi.Id_Budget = 0 "
                DT = objCosti.Leggi(Piva_Proprietaria, CInt(elem_cod), "", 0, CInt(mat_cod), 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, FiltroperPiva, "", objParametriServer)

                If DT.Rows.Count > 0 Then
                    tabdaDuplicare.Add(New With
                                    {
                                         .tab = "a_tab_prodotto_UC_storico_prezzi",
                                         .dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None),
                                         .datiparamqualcalibri = "",
                                         .datiparamqualindici = ""
                                    })
                End If
                DT.Reset()
                '#####Fine Lettura Costi Tab Storico Prezzi####

                '#####Lettura Tab Dati Contabilita####
                Dim objExtraPrivata As New Prodotti_Extra_Privata_R
                DT = objExtraPrivata.Leggi(Piva_Proprietaria, CInt(mat_cod), CInt(elem_cod), 0,
                                            Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                            "", "", "", Nothing, Nothing, Nothing, Nothing, Nothing,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objParametriServer)

                If DT.Rows.Count > 0 Then
                    tabdaDuplicare.Add(New With
                                        {
                                            .tab = "a_tab_prodotto_UC_dati_contabilita",
                                            .dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None),
                                            .datiparamqualcalibri = "",
                                            .datiparamqualindici = ""
                                        })
                End If
                DT.Reset()
                '#####Fine Lettura Tab Dati Contabilita####

                '#####Lettura Tab Traduzioni####
                Dim objMaterieXLingue As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                DT = objMaterieXLingue.Leggi_Materie_Prime_XLingue(piva,
                                                 CInt(elem_cod),
                                                 Nothing,
                                                 CInt(mat_cod),
                                                 0,
                                                 "",
                                                 "",
                                                 objParametriServer)

                If DT.Rows.Count > 0 Then
                    tabdaDuplicare.Add(New With
                                    {
                                        .tab = "a_tab_prodotto_UC_traduzioni",
                                        .dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None),
                                        .datiparamqualcalibri = "",
                                        .datiparamqualindici = ""
                                    })
                End If
                DT.Reset()
                '#####Fine Lettura Tab Traduzioni####

                '#####Lettura Tab Descrizioni Addizionali####
                If CInt(dtProdotto(0)("ChkAlias")) = 0 Then

                    Dim objAlias As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R


                    DT = objAlias.LeggiAliasxGrigliaAnagraficaProdotti(piva,
                                                                        mat_cod,
                                                                        0,
                                                                        sa_cod,
                                                                        elem_cod,
                                                                        "", "", objParametriServer)


                    If DT.Rows.Count > 0 Then
                        tabdaDuplicare.Add(New With
                                            {
                                                .tab = "a_tab_prodotto_UC_alias",
                                                .dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None),
                                                .datiparamqualcalibri = "",
                                                .datiparamqualindici = ""
                                            })
                    End If
                End If
                DT.Reset()
                '#####Fine Lettura Tab Descrizioni Addizionali####

                '#####Lettura Tab Parametri Qualitativi####
                If CInt(elem_cod) = SEMILAVORATI_VEGETALI Then
                    Dim objParametriQual As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R
                    Dim DTCalibri As New DataTable
                    DTCalibri = objParametriQual.Leggi_Calibri(piva,
                                                         sa_cod,
                                                         CInt(mat_cod),
                                                         0,
                                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "", "", objParametriServer)



                    Dim DTIndici As New DataTable
                    DTIndici = objParametriQual.Leggi_Indici(piva,
                                                             sa_cod,
                                                             CInt(mat_cod),
                                                             0,
                                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                             "", "", objParametriServer)

                    If DTCalibri.Rows.Count > 0 OrElse DTIndici.Rows.Count > 0 Then
                        tabdaDuplicare.Add(New With
                                            {
                                                .tab = "a_tab_prodotto_UC_parametri_qualitativi",
                                                .dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None),
                                                .datiparamqualcalibri = JsonConvert.SerializeObject(DTCalibri, Newtonsoft.Json.Formatting.None),
                                                .datiparamqualindici = JsonConvert.SerializeObject(DTIndici, Newtonsoft.Json.Formatting.None)
                                            })
                    End If

                End If
                '#####Fine Lettura Tab Parametri Qualitativi####

                '#####Lettura Tab Configurazione####
                If CInt(dtProdotto(0)("Udm_Cod_Extra")) <> 0 OrElse CInt(dtProdotto(0)("Flag_Variazione")) <> 0 OrElse
                     CInt(dtProdotto(0)("ChkEscludi_Magazzino")) <> 0 Then

                    Dim obj_OTB_R As New OTabelle_R
                    'Manca tabella_cod
                    Dim Filtro As String = "OTabelle_Parametri.Mat_Cod_Generazione_Link= " & Agro_SQL_SaveNum(CInt(mat_cod))

                    Dim Moduli_String = String.Empty

                    If listModuliAttivi_anagrafe_log IsNot Nothing AndAlso listModuliAttivi_anagrafe_log.Count > 0 Then
                        Moduli_String = String.Join(",", listModuliAttivi_anagrafe_log)
                    End If

                    DT = obj_OTB_R.LeggiParametri(piva, 0, Moduli_String, Filtro, objParametriServer)

                    Dim Dati As String = ""
                    If DT.Rows.Count > 0 Then
                        Dati = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None)
                    End If

                    tabdaDuplicare.Add(New With
                                            {
                                                .tab = "a_tab_prodotto_UC_configurazione",
                                                .dati = Dati,
                                                .datiparamqualcalibri = "",
                                                .datiparamqualindici = ""
                                             })
                End If

                '#####Fine Lettura Tab Configurazione####

            End If

            If tabdaDuplicare.Count > 0 Then
                r.ParametroDue_stringa = JsonConvert.SerializeObject(tabdaDuplicare, Newtonsoft.Json.Formatting.None)
                r.ParametroDue = True
            Else
                r.ParametroDue = False
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(dtProdotto, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function InfoProdotto(ByVal piva As String, ByVal mat_cod As Integer, ByVal elem_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dtProdotto As DataTable
            Dim objMateria As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            dtProdotto = objMateria.LeggiMateriePrimexSuperUser(objParametriServer.PivaSuperUser,
                                                        piva,
                                                        CInt(elem_cod),
                                                        CInt(mat_cod),
                                                        0, "", True,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                        "", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dtProdotto, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_XddlDittadiProvenienza() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim objDitte As New AgronicaCoreMetaSchemaDAL.Ditte_R
            dt = objDitte.Leggi(0, "", "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlSpecieAnimali() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Lista_Specie_Animali(List, True, "", 0, 0, "", "", objParametriServer)

            Dim AnimaliList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                AnimaliList.Add(New With
                                {
                                     .SPE_COD = List.Items(i).Value,
                                     .SPE_DES = List.Items(i).Text
                                })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(AnimaliList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlIndirizzoProduttivo(ByVal Gen_Cod As Integer, ByVal Spe_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Lista_IndirizziProd_Animali(List, True, "Tutti gli Indirizzi Produttivi", -1, Gen_Cod, Spe_Cod, "", "", objParametriServer)

            Dim IndirizziList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                IndirizziList.Add(New With
                                {
                                     .IPRO_COD = List.Items(i).Value,
                                     .IPRO_DES = List.Items(i).Text
                                })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(IndirizziList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Leggi_XddlTipologieSementi() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim objTipoSem As New AgronicaCoreMetaSchemaDAL.TipologieSementi_R
            dt = objTipoSem.Leggi(0, "", "", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function CaricaGrigliaStoricoPrezzi(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal pro_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dtProdotto As DataTable
            Dim obj As New Prodotti_Costi_R

            If piva <> "" Then
                Dim FiltroperPiva As String = "Prodotti_Costi.piva = '" & Agro_SQL_SaveText(piva) & "' And Prodotti_Costi.Id_Budget = 0 "

                dtProdotto = obj.Leggi(piva,
                                       elem_cod,
                                       "",
                                       pro_cod,
                                       mat_cod,
                                       0,
                                       0,
                                       0,
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       FiltroperPiva,
                                       "", objParametriServer)


                r.RispostaStringa = JsonConvert.SerializeObject(dtProdotto, Newtonsoft.Json.Formatting.None)
            End If


            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_GruppoVarietale(ByVal Veg_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim obj As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            dt = obj.Leggi(Veg_Cod,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           "", "",
                           objParametriServer)

            If Not IsNothing(dt) Then
                Dim xCod As Integer
                Dim xDes As String
                Dim udmList As New List(Of Object)
                For i = 0 To dt.Rows.Count - 1
                    'Non Ibrido
                    xCod = dt.Rows(i).Item("Grva_Cod")
                    xDes = CStr(dt.Rows(i).Item("Grva_Des"))

                    udmList.Add(New With
                             {
                                .Grva_Cod = xCod,
                                .Grva_Des = xDes
                            })

                    'Ibrido
                    xCod = -1 * dt.Rows(i).Item("Grva_Cod")
                    xDes = CStr(dt.Rows(i).Item("Grva_Des") & " -- Ibrido")

                    udmList.Add(New With
                                {
                                     .Grva_Cod = xCod,
                                     .Grva_Des = xDes
                                })

                Next

                r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
                r.RispostaOK = True

            End If


        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_VarietaColturale(ByVal piva As String, ByVal Veg_cod As Integer, ByVal Cul_cod_da_modificare As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim List As New DropDownList

            AgronicaCoreUtility.CaricaListControl.Cultivar(List, False, "", "", Veg_cod, 0, "", True, Cul_cod_da_modificare, 0, "", "", objParametriServer, objParametriUtenti)

            Dim udmList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                udmList.Add(New With
                                {
                                     .Cul_Cod = List.Items(i).Value,
                                     .Cul_Des = List.Items(i).Text
                                })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Leggi_Unita_Di_Misura(ByVal piva As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer, ByVal Sa_Cod As Integer) As RispostaStandard


        Dim cmbUdm As New DropDownList
        Dim Num_Totale As Integer = 0
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura
            AgronicaCoreUtility.CaricaListControl.Udm_Optimize(cmbUdm,
                                                                Num_Totale,
                                                                piva,
                                                                Sa_Cod,
                                                                0,
                                                                CAU_CARICO,
                                                                Elem_Cod,
                                                                False,
                                                                0,
                                                                Mat_Cod,
                                                                True, "", 0,
                                                                "", "", "",
                                                                objParametriServer, objParametriUtenti)

            Dim udmList As New List(Of Object)

            For Each i As ListItem In cmbUdm.Items

                udmList.Add(New With
                             {
                                .udm_cod = i.Value,
                                .udm_des = i.Text
                            })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function CaricaGrigliaCalibri(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim ObjPQ As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

            dt = ObjPQ.Leggi_Calibri(piva,
                                     sa_cod,
                                     mat_cod,
                                     0,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function



    Public Shared Function Leggi_CalibriFrutti() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
            Dim dt As DataTable
            dt = objCalibri.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function CaricaGrigliaIndici(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim ObjPQ As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

            dt = ObjPQ.Leggi_Indici(piva,
                                     sa_cod,
                                     mat_cod,
                                     0,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_IndiciMaturita(ByVal Veg_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Veg_Cod = 31
            Dim chk As New CheckBoxList
            AgronicaCoreUtility.CaricaListControl.IndiciMaturita(chk,
                                                                False, "", "",
                                                                Veg_Cod,
                                                                True,
                                                                "", "", objParametriServer)

            Dim udm As New List(Of Object)

            For Each i As ListItem In chk.Items
                Dim val As Integer = i.Value.Split("|")(0)
                udm.Add(New With
                             {
                                .IND_MAT_COD = val,
                                .IND_MAT_DES = i.Text
                              })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udm, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function PermessoModificaProdotto(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard

        Dim result As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim possoModificare As Boolean = False
        Dim strRes As String = ""
        If sa_cod <> -1 Then
            possoModificare = True
        Else
            If piva = objParametriAgenda.Piva Then
                possoModificare = True
            Else
                strRes = AgronicaAgenda_2010.ProdottoCreatoDaUnAltraImpresa
            End If
        End If

        result.RispostaOK = True
        result.RispostaConferma = possoModificare
        result.RispostaStringa = strRes

        Return result

    End Function

    Public Shared Function Leggi_XddlProdottoBase(ByVal piva As String, ByVal elem_cod As Integer, ByVal veg_cod As Integer, ByVal cul_cod As Integer, ByVal regolamento As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim ObjMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim DtMateriePrime As DataTable

            'Evito che vengano visualizzati tutti i prodotti OMNI che hanno un determinato veg_cod,
            'quando viene scelta una specie vegetale senza però una varietà.
            If veg_cod <> -1 AndAlso cul_cod = 0 Then
                cul_cod = -1
            End If

            'Creo il filtro su regolamento e chk_referenza
            Dim xFiltroAggiuntivo_MP As New System.Text.StringBuilder

            If regolamento <> 0 Then
                xFiltroAggiuntivo_MP.AppendLine(" Materie_Prime.Regolamento = " & Agro_SQL_SaveNum(regolamento) & " AND ")
            End If
            xFiltroAggiuntivo_MP.AppendLine(" Materie_Prime.ChkReferenza = 1")


            'Leggo le materie prime     
            DtMateriePrime = ObjMateriePrime.Leggi(piva,
                                                    0, elem_cod, 0, "", veg_cod, cul_cod, 0, 0, 0, 0, 0, "", 0, "",
                                                    True,
                                                    True,
                                                    "",
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    xFiltroAggiuntivo_MP.ToString,
                                                    "",
                                                    objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(DtMateriePrime, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Cambia_Codice_Articolo_Codice_Esterno_InBaseAFlag(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim DT As DataTable
            Dim obj As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim risp = "false"

            Dim xFiltro As String = "OGenerazioni_Anagrafe_Moduli.ChkReferenze = 1 AND OGenerazioni_Anagrafe_Moduli_Log.ChkCodiceEsterno = 1"


            Dim Moduli_String = String.Empty

            If listModuliAttivi_anagrafe_log IsNot Nothing AndAlso listModuliAttivi_anagrafe_log.Count > 0 Then
                Moduli_String = String.Join(",", listModuliAttivi_anagrafe_log)
            End If

            DT = obj.Leggi_Join_Con_OGenerazioni_Anagrafe_Moduli(objParametriServer.PivaSuperUser,
                                                                 piva,
                                                                 0,
                                                                 Moduli_String,
                                                                 xFiltro,
                                                                 "",
                                                                 objParametriServer)
            If DT.Rows.Count > 0 Then
                risp = "true"
            End If

            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function CaricaGrigliaTraduzioni(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim DT As DataTable
            Dim obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            DT = obj.Leggi_Materie_Prime_XLingue(piva,
                                                 elem_cod,
                                                 Nothing,
                                                 mat_cod,
                                                 0,
                                                 "",
                                                 "",
                                                 objParametriServer)

            Dim traduzioni As List(Of Object) = New List(Of Object)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                For Each dr As DataRow In DT.AsEnumerable

                    Dim pivakey As String = If(dr.Item("Piva") Is DBNull.Value, "", dr.Item("Piva"))
                    Dim elem_codkey As Integer = If(dr.Item("Elem_Cod") Is DBNull.Value, 0, dr.Item("Elem_Cod"))
                    Dim lingua_codkey As Integer = If(dr.Item("Lingua_Cod") Is DBNull.Value, 0, dr.Item("Lingua_Cod"))
                    Dim sa_codkey As Integer = If(dr.Item("Sa_Cod") Is DBNull.Value, 0, dr.Item("Sa_Cod"))
                    Dim mat_codkey As Integer = If(dr.Item("Mat_Cod") Is DBNull.Value, 0, dr.Item("Mat_Cod"))
                    traduzioni.Add(New With
                                   {
                                        .Lingua_Cod = lingua_codkey,
                                        .Piva = pivakey,
                                        .Elem_Cod = elem_codkey,
                                        .Sa_Cod = sa_codkey,
                                        .Mat_Cod = mat_codkey,
                                        .Mat_Des = If(dr.Item("Mat_Des") Is DBNull.Value, "", dr.Item("Mat_Des")),
                                        .DATA_AGG = If(dr.Item("DATA_AGG") Is DBNull.Value, "", dr.Item("DATA_AGG")),
                                        .Nome = If(dr.Item("Nome") Is DBNull.Value, "", dr.Item("Nome")),
                                        .Key_Traduzioni = lingua_codkey.ToString() & "." & pivakey & "." & elem_codkey.ToString() & "." & sa_codkey.ToString() & "." & mat_codkey.ToString()
                                   })
                Next
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(traduzioni, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlLingua() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Creo il filtro per escludere la lingua dell'utente
            Dim xFiltroAggiuntivo_Lingua As New System.Text.StringBuilder
            '01/06/2022 Casadei: modifica per far sì che si vedano le scelte tra inglese, francese ecc.
            'escludendo sempre l'italiano piuttosto che la lingua dell'utente
            xFiltroAggiuntivo_Lingua.AppendLine(" Lingue.Lingua_Cod <> 1") ' + Agro_SQL_SaveNum(objParametriUtenti.Lingua_Cod))

            Dim objLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = objLingua.Leggi_datoCODGIAS(0,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    xFiltroAggiuntivo_Lingua.ToString,
                                                                    "",
                                                                    objParametriUtenti)

            r.RispostaStringa = JsonConvert.SerializeObject(dtLingua, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Leggi_XddlIVACompensazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim obj As New AgronicaCoreMetaSchemaDAL.IVA_CodiciIvaProdottiAgricoli_R
            Dim dt As DataTable = obj.Leggi(0,
                                            "",
                                            "",
                                            "",
                                            objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_Prodotti_Extra_Privata(
                                                         ByVal piva As String,
                                                         ByVal elem_Cod As Integer,
                                                         ByVal mat_cod As Integer,
                                                         ByVal pro_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim obj As New Prodotti_Extra_Privata_R

            Dim dt As DataTable = obj.Leggi(piva, mat_cod, elem_Cod, pro_Cod,
                                            Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                            "", "", "", Nothing, Nothing, Nothing, Nothing, Nothing,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_XddlRazzaAnimale(ByVal Gen_Cod As Integer, ByVal Spe_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Lista_Razze_Animali(List, True, Gias.TutteLeRazze, -1, Gen_Cod, Spe_Cod, "", "", objParametriServer)

            Dim RazzeList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                RazzeList.Add(New With
                                {
                                     .RAZ_COD = List.Items(i).Value,
                                     .RAZ_DES = List.Items(i).Text
                                })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(RazzeList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_CategoriaRisorsa(ByVal piva As String, ByVal Tipo_Classe As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As DataTable
            Dim Obj As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R

            dt = Obj.Leggi_con_Filtro_Tipo_Classe(piva,
                                                  Tipo_Classe,
                                                  "",
                                                  objParametriServer)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim catRisLIst As New List(Of Object)
                For Each dr As DataRow In dt.Rows
                    catRisLIst.Add(New With {.Linea_Classe_Cod = dr.Item("Linea_Classe_Cod"), .Linea_Classe_Des = dr.Item("Linea_Classe_Des")})
                Next

                r.RispostaStringa = JsonConvert.SerializeObject(catRisLIst, Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Formatting.None)
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_LineaProduzione(ByVal piva As String, ByVal Linea_Classe_Cod As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreContabDAL.Linee_Produzioni_R

            If Linea_Classe_Cod <> 0 Then
                dt = Obj.Leggi_LineeProduzioni_Con_LineeProduzioni_Classi(piva,
                                                                          Linea_Classe_Cod,
                                                                          "",
                                                                          objParametriServer)
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_FinalitaProduttiva(ByVal GRFI_COD As Integer, ByVal VEG_COD As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R

            dt = Obj.Leggi_Con_GruppoFinalita_SpecieVegetali(GRFI_COD,
                                                            VEG_COD,
                                                            "",
                                                            objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_UnitaMisuraDefault(ByVal elem_cod As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R

            dt = Obj.Leggi_con_UnitaMisura(elem_cod,
                                           "",
                                          objParametriServer)


            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim udmLIst As New List(Of Object)
                For Each dr As DataRow In dt.Rows
                    udmLIst.Add(New With
                            {
                               .UDM_COD = dr.Item("Udm_Cod"),
                               .UDM_DES = dr.Item("Udm_Des")
                           })

                Next
                r.RispostaStringa = JsonConvert.SerializeObject(udmLIst, Newtonsoft.Json.Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_LastCod_Articolo(ByVal piva As String)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

            dt = Obj.Leggi_LastCod_Articolo(piva,
                                            "",
                                            objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_UnitaMisuraBene()
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

            dt = Obj.Leggi(0,
                           0,
                           "",
                           "",
                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                           "",
                           "",
                           objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_CodiciImballaggio()
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T010_TabellaCodiciImballaggio_R

            dt = Obj.Leggi("",
                           "",
                           objParametriServer)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim codiciList As New List(Of Object)
                For Each dr As DataRow In dt.Rows
                    codiciList.Add(New With
                            {
                               .Codice = dr.Item("Codice"),
                               .Descrizione = dr.Item("Descrizione italiana")
                           })

                Next
                r.RispostaStringa = JsonConvert.SerializeObject(codiciList, Newtonsoft.Json.Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
            End If
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_Modulo_Anagrafe(ByVal piva As String)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable

            ' Modulo di appartenenza
            Dim leggi_anagrafe_log As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim wModulo_Anagrafe_Log As Integer = 0
            Dim dtAnagrafeLog = leggi_anagrafe_log.Leggi(piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                               "", "", objParametriServer)


            For Each rows In dtAnagrafeLog.Rows
                listModuliAttivi_anagrafe_log.Add(CInt(rows.Item("Modulo_Generazione")))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(listModuliAttivi_anagrafe_log, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_Tipo_Default()
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_R

            dt = Obj.Leggi(0,
                           0,
                           "",
                           objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Carica_Default_ParametriQualitativi_Dati_Tecnici(ByVal cal_cod As Integer)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim Obj As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R

            If cal_cod <> 0 Then
                dt = Obj.Leggi(cal_cod,
                                "", 0, 0, 0, "", True,
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "",
               objParametriServer)

                r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function ParametroQualGestitiXCheckBoxBeniConf(ByVal piva As String)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Obj As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R

            Dim dtParamQual As DataTable = Obj.Leggi(piva, 0, False, "Tipo = 1", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dtParamQual, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Controlla_Se_Prodotto_Movimentato(ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod_alias As Integer)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Obj As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim dettagli_prodotto = Obj.Leggi_Dettagli_Prodotto("",
                                                             elem_cod,
                                                             pro_cod,
                                                             mat_cod,
                                                             "",
                                                             0,
                                                             "",
                                                             objParametriServer,
                                                             mat_cod_alias)

            r.RispostaStringa = JsonConvert.SerializeObject(dettagli_prodotto, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function ComponiMessaggioDiAvvisoCancellazioneProdotto(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal sa_cod As Integer, ByVal isalias As Boolean)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Msg = String.Empty

            If isalias Then

                'Controllo se l'Alias è associato a un Prodotto e mando un messaggio di conferma cancellazione
                Dim objMaterie_Prime_Alias As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R

                Dim Alias_Associato As Boolean = objMaterie_Prime_Alias.IsAlias(piva, mat_cod, objParametriServer)

                If Alias_Associato Then
                    Msg &= "-" & DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "TutteLeAssociazioniAltriProdotti"), String) & "<br>"
                End If

            ElseIf Not isalias Then

                'Se il Prodotto è pubblico avviso l'utente che verranno cancellati i Prezzi inseriti anche da altri Utenti
                If sa_cod = -1 Then
                    Dim obj As New Prodotti_Costi_R
                    Dim dt_Prodotti_Costi = obj.Leggi(0,
                                           elem_cod,
                                           "",
                                           0,
                                           mat_cod,
                                           0,
                                           0,
                                           0,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "", objParametriServer)

                    If dt_Prodotti_Costi IsNot Nothing AndAlso dt_Prodotti_Costi.Rows.Count > 0 Then
                        Msg &= "-" & DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "PrezziInseritiDaAltreAziende"), String) & "<br>"
                    End If

                End If

                'Controllo se ci sono dei Listini per quel Prodotto
                Dim objListiniPrezzixRisorse As New AgronicaCoreContabDAL.Listini_Prezzi_R
                Dim List_ListiniPrezzi = objListiniPrezzixRisorse.Leggi_Listino_Prodotti("", 0, elem_cod, 0, mat_cod,
                                                                                        objParametriServer)

                If List_ListiniPrezzi IsNot Nothing AndAlso List_ListiniPrezzi.Count > 0 Then
                    Msg &= "-" & DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "ListiniAssociati"), String) & "<br>"
                End If


                'Controllo se ci sono delle Griglie di Campionamento e Listini_CampionamentoConferito_Prodotti per quel Prodotto
                Dim objCampionamentoConferito As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R
                Dim List_CampionamentoConferito = objCampionamentoConferito.Leggi_TestataGriglia_Prodotti_FiltroMat_Cod("", mat_cod, "",
                                                                                                                        objParametriServer)

                If List_CampionamentoConferito IsNot Nothing AndAlso List_CampionamentoConferito.Count > 0 Then
                    Msg &= "-" & DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "TabelleCampionamentoEListiniLiquidazioneAssociati"), String) & "<br>"
                End If
            End If

            r.RispostaStringa = Msg

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function CaricaGrigliaAlias(ByVal piva As String, ByVal mat_cod As Integer, ByVal elem_cod As Integer, ByVal sa_cod As Integer)

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dtAlias As DataTable
            Dim objMateria As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R


            dtAlias = objMateria.LeggiAliasxGrigliaAnagraficaProdotti(piva,
                                                                        mat_cod,
                                                                        0,
                                                                        sa_cod,
                                                                        elem_cod,
                                                                        "", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dtAlias, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_XddlAlias(ByVal piva As String, ByVal elem_cod As Integer, ByVal sa_cod As Integer)

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

            Dim xFiltroAggiuntivo = "Materie_Prime.ChkAlias = 1 AND Materie_Prime.Elem_Cod = " & elem_cod

            Dim dtAlias = objMaterie_Prime.Leggi3(piva, 0, sa_cod, 0, "", False, xFiltroAggiuntivo, "",
                                                   objParametriServer)
            Dim jArrayAlias As New JArray()

            If dtAlias IsNot Nothing AndAlso dtAlias.Rows.Count > 0 Then

                For Each dr As DataRow In dtAlias.Rows
                    jArrayAlias.Add(New JObject(New JProperty("Mat_Cod_Alias", dr.Item("Mat_Cod")), New JProperty("Mat_Des_Alias", dr.Item("Mat_Des"))))
                Next

            End If


            r.RispostaStringa = JsonConvert.SerializeObject(jArrayAlias, Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Controlla_Se_Alias_Associato_a_Materia_Prima(ByVal piva As String, ByVal mat_cod_alias As Integer)

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objMaterie_Prime_Alias As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R

            Dim Alias_Associato = objMaterie_Prime_Alias.IsAlias(piva, mat_cod_alias,
                                                            objParametriServer)


            r.RispostaStringa = Alias_Associato

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    'Controlli Cancellazione Prodotto
    Public Shared Function Controlli_Cancella_Materia_Prima(ByVal elem_cod As Integer, ByVal mat_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            '############# 2. CONTROLLO LOG GIAS OMNI  #############################################################################
            'Controlli solo per Materie Prime

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Controllo_Log_Ok As Boolean = False
            Dim MsgErrore As String = String.Empty

            Dim Obj As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R

            Dim OrderBy As String = " Piva, Sa_Cod, ChkScollegamento, Tipo_Generazione, Codice_Generazione, Linea_Cod, Key1, Mat_Cod, Elem_Cod ASC"
            Dim FiltroAgg As String = "Linea_Cod <> 0 And ChkScollegamento = 0 And Mat_Cod = " & Agro_SQL_SaveNum(mat_Cod)
            Dim dtLog As DataTable = Obj.Leggi("", 0, 0, 0, 0, 0, objParametriServer.FinestraTemporaleInizio, objParametriServer.FinestraTemporaleFine,
                                                FiltroAgg, OrderBy, objParametriServer)


            If dtLog.Rows.Count > 0 Then
                Dim Modulo_Generazione As Integer = CInt(dtLog(0)("Modulo_Generazione").ToString)
                Dim objModuli As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim dtModuli As DataTable = objModuli.Leggi_Join_Con_OGenerazioni_Anagrafe_Moduli(objParametriServer.PivaSuperUser, "", 0, Modulo_Generazione, "", "", objParametriServer)

                If dtModuli.Rows.Count = 0 Then

                    'eccezione
                    MsgErrore += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "AnagraficaNonEliminabileInclusaModuliGiasContattaAmministratore"), String) '"L'anagrafica non può essere eliminata poichè incluso in moduli gias. Contattare l'amministratore."

                Else
                    Dim Modulo_Descrizione As String = dtModuli(0)("Modulo_Descrizione").ToString
                    MsgErrore += String.Format(DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "AnagraficaNonEliminabileInclusaModuloX"), String), Modulo_Descrizione) & "<br/>"
                End If

            Else
                Controllo_Log_Ok = True
            End If

            If Controllo_Log_Ok Then

                '############# 3. CONTROLLO REFERENZE GIAS OMNI  #######################################################################
                Dim ObjRef As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                Dim dtReferenze As DataTable = ObjRef.LeggiGenerazioniReferenze("", elem_cod, mat_Cod, 0, "", "", objParametriServer) 'Generate automaticamente da omni

                If dtReferenze.Rows.Count > 0 Then
                    Controllo_Log_Ok = False
                    MsgErrore += String.Format(DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "AnagraficaNonEliminabileRiferitaAziendaX"), String), dtReferenze(0)("Mat_Des").ToString) & "<br/>"

                End If

            End If

            If Controllo_Log_Ok Then
                r.RispostaOK = True
            Else
                r.RispostaStringa = MsgErrore
                r.RispostaOK = False
            End If


        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function RicercaFiltriBeniConfezVeg(ByVal piva As String, ByVal mat_Cod As Integer, ByVal tabella_Cod As Integer)
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim obj_OTB_R As New OTabelle_R
            Dim dt As DataTable
            Dim Filtro As String = "OTabelle_Parametri.Mat_Cod_Generazione_Link= " & Agro_SQL_SaveNum(mat_Cod)

            Dim Moduli_String = String.Empty

            If listModuliAttivi_anagrafe_log IsNot Nothing AndAlso listModuliAttivi_anagrafe_log.Count > 0 Then
                Moduli_String = String.Join(",", listModuliAttivi_anagrafe_log)
            End If

            dt = obj_OTB_R.LeggiParametri(piva, tabella_Cod, Moduli_String, Filtro, objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function CaricaCategorieMagazzinoXUtente() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim CategoriaDiDefault As String = String.Empty
            Dim Dt_Impost As DataTable
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim strElem_Cod As String = ""
            Dt_Impost = objImpost.Leggi2(1, objParametriUtenti.UtenteUsername, 0,
                                            "", "",
                                            objParametriUtenti)

            Dim DrFiltroElemCod() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)

            'E proporre già impostata quella che eventualmente l'utente ha di default
            Dim DrFiltro() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

            If DrFiltroElemCod IsNot Nothing AndAlso DrFiltroElemCod.Length > 0 Then
                strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                strElem_Cod = Replace(strElem_Cod, "|", ",")
            End If


            If strElem_Cod <> "" Then
                strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
            Else
                strElem_Cod = " Elem_Cod <> " & COADIUVANTI.ToString
            End If

            'Leggo le categorie di magazzino che l'utente puo vedere
            Dim objCategorie_Magazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim Dt_CategMag As DataTable
            Dim FiltroAggiuntivo As String = strElem_Cod

            Dt_CategMag = objCategorie_Magazzino.Leggi(0, "", False, FiltroAggiuntivo, "", objParametriServer)


            If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                CategoriaDiDefault = DrFiltro(0).Item("Impostazione_Valore_1")
                r.ParametroDue_stringa = CategoriaDiDefault
                r.ParametroDue = True
            Else
                r.ParametroDue_stringa = CategoriaDiDefault
                r.ParametroDue = False
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(Dt_CategMag, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Leggi_ImpostazioneUtente() As RispostaStandard
        Dim r As New RispostaStandard
        Dim AbilitaDDLCategoriaBeniConf As String = "false"
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
                r.Sessione = False
                Return r
            End If

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim mostraimballaggi As String = "false"
            Dim mostracontenitori As String = "false"

            Dim dtImpostazioni = leggiImpostazioni.Leggi(0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)

            If dtImpostazioni.Rows.Count > 0 Then
                'Verifico se è da abilitare o meno la DDL Categoria Beni Di Confezionamento
                Dim DrFiltroAbilitaDDL() As DataRow = dtImpostazioni.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_DAA)

                If DrFiltroAbilitaDDL IsNot Nothing AndAlso DrFiltroAbilitaDDL.Count > 0 Then
                    Dim valAbilitaDDL = DrFiltroAbilitaDDL(0).Item("Impostazione_Valore_1").ToString()

                    If valAbilitaDDL = "1" Then
                        AbilitaDDLCategoriaBeniConf = "true"
                    End If
                End If

                'Verifico se è da mostrare o meno il checkbox Imballaggio della tab Configurazione
                Dim DrFiltroImballaggio() As DataRow = dtImpostazioni.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.SUPERUSER_COD_IMPOSTAZIONE_IMBALLAGGI)

                If DrFiltroImballaggio IsNot Nothing AndAlso DrFiltroImballaggio.Count > 0 Then

                    Dim valImba = DrFiltroImballaggio(0).Item("Impostazione_Valore_1").ToString()

                    If valImba = "1" Then
                        mostraimballaggi = "true"
                    End If
                End If


                'Verifico se è da mostrare o meno il checkbox Contenitori della tab Configurazione
                Dim DrFiltroContenitori() As DataRow = dtImpostazioni.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.SUPERUSER_COD_IMPOSTAZIONE_CONTENITORI)

                If DrFiltroContenitori IsNot Nothing AndAlso DrFiltroContenitori.Count > 0 Then
                    Dim valCont = DrFiltroContenitori(0).Item("Impostazione_Valore_1").ToString()

                    If valCont = "1" Then
                        mostracontenitori = "true"
                    End If
                End If

            End If

            Dim FiltriImbaCont As New List(Of Object)

            FiltriImbaCont.Add(New With
                {
                     .MostraImballaggi = mostraimballaggi,
                     .MostraContenitori = mostracontenitori
                })

            r.ParametroDue_stringa = JsonConvert.SerializeObject(FiltriImbaCont, Newtonsoft.Json.Formatting.None)
            r.RispostaStringa = AbilitaDDLCategoriaBeniConf
            r.RispostaOK = True
            r.ParametroDue = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_Contatti_Clienti_Fornitori(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
                r.Sessione = False
                Return r
            End If

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim xFiltroAggiuntivo As String = "Rapporti_Contabili.Cliente = 1"

            Dim DT As DataTable = objContatti.Leggi_Contatti_ByCod_Rapporto(True, piva, "", 0, xFiltroAggiuntivo, "", objParametriServer)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For Each dr As DataRow In DT.Rows
                    dr("Rag_Soc") = dr("Rag_Soc") & " (" & dr("Rapporto_Des") & ")"
                Next

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Imposta_Default_UDM_GridStoricoPrezzi(ByVal elem_cod As Integer, Cod_BancheDati As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
                r.Sessione = False
                Return r
            End If

            Dim udmCod As Integer = 0

            Select Case elem_cod
                Case FERTILIZZANTI

                    Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS

                    Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input With {
                    .Codice = Cod_BancheDati,
                    .Descrizione = "",
                    .DataInizio = AGRODATAINIZIO,
                    .DataFine = AGRODATAFINE,
                    .Tipo = 0,
                    .IncludiApporti = True,
                    .IncludiTipologia = False,
                    .Regolamento = 0,
                    .strFiltro = ""
                    }

                    Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output = objFert_WS.Fertilizzanti(objParametriIngresso)

                    If objParametriUscita.ListaFertilizzanti IsNot Nothing AndAlso objParametriUscita.ListaFertilizzanti.Count = 1 Then
                        udmCod = objParametriUscita.ListaFertilizzanti(0).Udm_Cod
                    End If

                    If objParametriUscita.MessaggioErrore <> "" OrElse (udmCod = -1 OrElse udmCod = 0) Then
                        r.Errore = objParametriUscita.MessaggioErrore
                    End If

                Case FORMULATI
                    Dim strErr As String = ""
                    Dim Udm_Cod As Integer = 0
                    Dim Udm_Des As String = ""
                    Dim Udm_Sim As String = ""

                    Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
                    objDPILeggi.Recupera_UdM_da_FrCod(strErr,
                                                  Cod_BancheDati,
                                                  Udm_Cod,
                                                  Udm_Sim,
                                                  Udm_Des,
                                                  HttpContext.Current.Session)

                    If strErr = "" Then
                        If Udm_Des <> "" Then
                            udmCod = Udm_Cod
                        End If
                    End If

            End Select


            r.RispostaStringa = udmCod
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Redirect_Prodotto(ByVal tipooperazione As Integer,
                                                             ByVal piva As String,
                                                                ByVal mat_cod As String,
                                                                ByVal elem_cod As String,
                                                                ByVal prodotto_des As String,
                                                                ByVal pro_cod As String,
                                                                ByVal duplica As Boolean,
                                                                ByVal proprietario As Boolean,
                                                                ByVal sa_cod As String,
                                                                ByVal isalias As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim url_completo As String = String.Empty

        If tipooperazione = enum_TipoOperazioneDB.Scrittura Then

            url_completo = MenuBS_Anagrafica.NuovoProdotto(elem_cod, isalias)

        ElseIf tipooperazione = enum_TipoOperazioneDB.Modifica OrElse
            tipooperazione = enum_TipoOperazioneDB.Copia Then

            url_completo = MenuBS_Anagrafica.EditProdotto(piva, mat_cod, elem_cod, prodotto_des,
                                                          pro_cod, duplica, proprietario, sa_cod, isalias)

        End If

        Return New RispostaStandard With {.RispostaOK = True, .RispostaStringa = url_completo}

    End Function



    Public Shared Function ControlliPrimaDelSubmit(ByVal model As String, ByVal importato As Boolean) As RispostaStandard

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim parametri As SalvaProdottoModel = JsonConvert.DeserializeObject(Of SalvaProdottoModel)(model, settingLoc)

            Dim r As New RispostaStandard
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim errori As String = String.Empty

            'Controlli (GIAS ONLINE vecchia gestione, manca il GIAS LAN) in base all'elem_cod
            If parametri.DatiTecnici IsNot Nothing AndAlso Not importato Then
                If Not Controlli_XCategoriaProdotto(parametri, errori) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks sulle Configurazioni
            If parametri.Configurazione IsNot Nothing AndAlso Not importato Then
                If Not Controlli_ConfigurazioneProdotti(parametri.Configurazione, errori) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks sui ParametriQualitativi-Dati Tecnici
            If parametri.DatiTecnici IsNot Nothing AndAlso
                parametri.DatiTecnici.UDM_Cod IsNot Nothing AndAlso
                parametri.ParametriQualitativiDatiTecnici IsNot Nothing AndAlso
                parametri.ParametriQualitativiDatiTecnici.Campi IsNot Nothing AndAlso
                Not String.IsNullOrEmpty(parametri.ParametriQualitativiDatiTecnici.Campi) AndAlso
                Not importato Then

                If Not Controlli_ParametriQualitativiDatiTecnici(parametri.ParametriQualitativiDatiTecnici, parametri.DatiTecnici.UDM_Cod, errori) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks sulle traduzioni
            If parametri.Traduzioni IsNot Nothing Then
                If Not Controlli_SalvaMateriePrimeXLingue(parametri.Traduzioni, errori) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks sullo storico prezzi
            If parametri.StoricoPrezzi IsNot Nothing AndAlso Not importato Then
                If Not Controlli_SalvaProdottiCosti(parametri.StoricoPrezzi.RigheInserite,
                                                    parametri.StoricoPrezzi.RigheModificate,
                                                    parametri.StoricoPrezzi.RigheEliminate,
                                                    parametri.StoricoPrezzi.TutteLeRighe,
                                                    errori) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks su altri Dati
            If parametri.AltriDati IsNot Nothing Then
                If Not Controlli_SalvaProdottiExtraPrivata(parametri.AltriDati, parametri.Mat_Cod, errori, objParametri_Server) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

            ' Checks su alias
            If parametri.DescrizioniAlternative IsNot Nothing AndAlso
               parametri.ChkAlias = 0 AndAlso
                parametri.IsMateriaPrima Then
                If Not Controlli_SalvaProdottiAlias(parametri.DescrizioniAlternative, errori, objParametri_Server) Then
                    Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = errori}
                End If
            End If

        Catch ex As Exception
            Return New RispostaStandard With {.RispostaOK = False, .RispostaStringa = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}
        End Try

        Return New RispostaStandard With {.RispostaOK = True}

    End Function




    Public Shared Function Submit(ByVal model As String) As RispostaStandard

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim parametri As SalvaProdottoModel = JsonConvert.DeserializeObject(Of SalvaProdottoModel)(model)
        Dim parametriDatiTecnici As DatiTecniciModel = parametri.DatiTecnici
        Dim errori As String = String.Empty
        Dim nuovo_Mat_Cod As Integer = 0
        Dim ErroreSalvataggio As Boolean = False
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard
        Dim objParametriAgenda As ParametriAgenda = New ParametriAgenda

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'Salvataggio Prodotto
            If parametri.IsMateriaPrima Then
                If parametri.DatiTecnici IsNot Nothing Then
                    ErroreSalvataggio = SalvaMateriaPrima(parametri, nuovo_Mat_Cod, errori)
                    If ErroreSalvataggio Then
                        Throw New Exception(errori)
                    End If

                    If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Copia Then
                        parametri.Mat_Cod = nuovo_Mat_Cod
                    End If

                End If
            End If


            ' Salvataggio dati contabilita e altri dati (perché vengo salvati entrambi nella tabella Prodotti_Extra_Privata)
            ' Se flag proprietario = true passo parametri.Piva
            ' Se flag proprietario = false passo objParametriAgenda.Piva
            Dim Piva As String = String.Empty
            If parametri.Proprietario = True Then
                Piva = parametri.Piva
            Else
                Piva = objParametriAgenda.Piva
            End If

            If parametri.DatiContabilita IsNot Nothing OrElse parametri.AltriDati IsNot Nothing Then
                ErroreSalvataggio = SalvaDatiContabilitaEAltriDati(Piva, parametri.CategoriaProdotto,
                                                         parametri.Mat_Cod, parametri.Pro_Cod,
                                                         parametri.DatiContabilita, parametri.AltriDati,
                                                         parametri.Proprietario, parametri.IsMateriaPrima, errori)
                If ErroreSalvataggio Then
                    Throw New Exception(errori)
                End If
            End If


            ' Salvo le traduzioni
            If parametri.Traduzioni IsNot Nothing Then
                ErroreSalvataggio = SalvaMateriePrimeXLingue(
                                                                    parametri.Piva,
                                                                    parametri.CategoriaProdotto,
                                                                    parametri.Mat_Cod,
                                                                    parametri.DatiTecnici.Sa_Cod,
                                                                    parametri.Traduzioni,
                                                                    errori)
                If ErroreSalvataggio Then
                    Throw New Exception(errori)
                End If
            End If

            'Salvo Storico Prezzi
            ' TODO errore pro_cod nothing
            ' Situazione nuova - posso modificare parte di un prodotto anche se non ne sono proprietario
            ' Se flag proprietario = true passo parametri.Piva
            ' Se flag proprietario = false passo objParametriAgenda.Piva
            If parametri.StoricoPrezzi IsNot Nothing Then
                Dim salvaOk As Boolean = SalvaStoricoPrezzi(Piva,
                                                            parametri.CategoriaProdotto,
                                                            parametri.Mat_Cod,
                                                            parametri.Pro_Cod,
                                                            parametri.StoricoPrezzi.RigheInserite,
                                                            parametri.StoricoPrezzi.RigheModificate,
                                                            parametri.StoricoPrezzi.RigheEliminate,
                                                            parametri.StoricoPrezzi.TutteLeRighe,
                                                            errori,
                                                            objParametri_Server)
                If Not salvaOk Then
                    Throw New Exception(errori)
                End If
            End If


            'Salvo ParametriQualitativiCalibri
            If parametri.ParametriQualitativiCalibri IsNot Nothing Then
                ErroreSalvataggio = SalvaParametriQualitativiCalibri(parametri.Piva,
                                                                                      parametri.Mat_Cod,
                                                                                      parametri.DatiTecnici.Sa_Cod,
                                                                                      errori,
                                                                                      parametri.ParametriQualitativiCalibri)
                If ErroreSalvataggio Then
                    Return New RispostaStandard With {.RispostaOK = False, .Errore = errori}
                End If
            End If

            'Salvo ParametriQualitativiIndici
            If parametri.ParametriQualitativiIndici IsNot Nothing Then
                ErroreSalvataggio = SalvaParametriQualitativiIndici(parametri.Piva,
                                                                                      parametri.Mat_Cod,
                                                                                      parametri.DatiTecnici.Sa_Cod,
                                                                                      parametri.DatiTecnici.SpecieVegetali,
                                                                                      errori,
                                                                                      parametri.ParametriQualitativiIndici)
                If ErroreSalvataggio Then
                    Return New RispostaStandard With {.RispostaOK = False, .Errore = errori}
                End If
            End If

            ' Salvo ParametriQualitativi-Dati Tecnici
            If ((parametri.ParametriQualitativiDatiTecnici IsNot Nothing AndAlso
                parametri.ParametriQualitativiDatiTecnici.Campi IsNot Nothing AndAlso
                Not String.IsNullOrEmpty(parametri.ParametriQualitativiDatiTecnici.Campi)) OrElse
                (parametriDatiTecnici IsNot Nothing AndAlso
                parametriDatiTecnici.Elimina_Referenza_ParamQualDTec = True)) Then

                Dim Cod_Articolo As String = parametri.CodiceProdotto
                If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica AndAlso parametri.NuovoCodiceProdotto IsNot Nothing AndAlso
                     Not String.IsNullOrEmpty(parametri.NuovoCodiceProdotto) Then
                    Cod_Articolo = parametri.NuovoCodiceProdotto
                Else
                    Cod_Articolo = parametri.CodiceProdotto
                End If
                ErroreSalvataggio = SalvaParametriQualitativiDatiTecnici(parametri.Piva, parametri.CategoriaProdotto,
                                         parametri.Mat_Cod, parametri.Descrizione, parametri.CodiceProdotto,
                                         parametri.Cal_Cod, parametri.TipoOperazioneDB, parametriDatiTecnici.Sa_Cod,
                                         parametriDatiTecnici.UDM_Cod, parametriDatiTecnici.Elimina_Referenza_ParamQualDTec,
                                         parametri.ParametriQualitativiDatiTecnici, errori)
                If ErroreSalvataggio Then
                    Throw New Exception(errori)
                End If
            End If

            ' Salvo Alias
            If parametri.DescrizioniAlternative IsNot Nothing AndAlso
                parametri.ChkAlias = 0 AndAlso
                parametri.IsMateriaPrima Then

                ErroreSalvataggio = SalvaAlias(parametri.Piva, parametri.Mat_Cod, parametriDatiTecnici.Sa_Cod,
                                               parametri.DescrizioniAlternative, errori)
                If ErroreSalvataggio Then
                    Throw New Exception(errori)
                End If
            End If

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            r.RispostaOK = True

            Dim mat_cod_ref As Integer = 0

            r.RispostaStringa = parametri.Mat_Cod

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r.RispostaOK = False
            r.RispostaStringa = errori
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return r

    End Function

    Private Shared Function SalvaMateriePrimeXLingue(
       ByVal piva As String,
       ByVal elem_Cod As Integer,
       ByVal mat_Cod As Integer,
       ByVal sa_cod As Integer,
       ByVal traduzioni As TraduzioniModel,
       ByRef errori As String) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim righeInserite As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheInserite, settingLoc)
            Dim obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_W

            Dim righeEliminate As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheEliminate, settingLoc)
            If righeEliminate IsNot Nothing AndAlso righeEliminate.Any Then
                Dim Cancella_OK As Boolean = False
                For Each mpl As MateriaPrimaXLInguaModel In righeEliminate
                    Cancella_OK = obj.Cancella_Materie_Prime_XLingue(piva,
                                                                        elem_Cod,
                                                                        Nothing,
                                                                        mat_Cod,
                                                                        mpl.Lingua_Cod,
                                                                        mpl.Mat_Des,
                                                                        "",
                                                                        objParametriServer
                                                                        )
                    If Not Cancella_OK Then
                        Return True
                    End If
                Next
            End If


            If righeInserite IsNot Nothing AndAlso righeInserite.Any Then
                Dim Scrittura_OK As Boolean = False
                For Each mpl As MateriaPrimaXLInguaModel In righeInserite
                    Scrittura_OK = obj.Scrivi_Materie_Prime_XLingue(piva,
                                                     elem_Cod,
                                                     sa_cod,
                                                     mat_Cod,
                                                     mpl.Lingua_Cod,
                                                     mpl.Mat_Des,
                                                     AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    objParametriServer
                                                    )
                    If Not Scrittura_OK Then
                        Return True
                    End If

                Next
            End If

            Dim righeModificate As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheModificate, settingLoc)
            If righeModificate IsNot Nothing AndAlso righeModificate.Any Then
                Dim Modifica_OK As Boolean = False
                For Each mpl As MateriaPrimaXLInguaModel In righeModificate

                    Dim Lingua_Cod_Letta As Integer = mpl.Lingua_Cod

                    If mpl.Lingua_Cod_Letta IsNot Nothing Then
                        Lingua_Cod_Letta = mpl.Lingua_Cod_Letta
                    End If

                    Modifica_OK = obj.Modifica_Materie_Prime_XLingue(piva,
                                                                        elem_Cod,
                                                                        sa_cod,
                                                                        Nothing,
                                                                        mat_Cod,
                                                                        mpl.Lingua_Cod,
                                                                        Lingua_Cod_Letta,
                                                                        mpl.Mat_Des,
                                                                        "",
                                                                        objParametriServer
                                                                      )
                    If Not Modifica_OK Then
                        Return True
                    End If
                Next
            End If



        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function

    Private Shared Function SalvaParametriQualitativiCalibri(ByVal piva As String,
       ByVal mat_Cod As Integer,
       ByVal sa_cod As Integer,
       ByRef errori As String,
       ByVal calibri As ParametriQualitativiCalibriModel) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim RigheScelte As List(Of CalibriModel) = JsonConvert.DeserializeObject(Of List(Of CalibriModel))(calibri.RigheScelte, settingLoc)
            Dim obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

            'Prima Elimino i calibri scelti già presenti nella tabella Materie_Prime_ParametriQualitativi e po li reinserisco con i calibri nuovi
            'PS(Udm_Cod lo passo a 0 perché nella tabella non c'è nessuna riga con Udm_Cod diverso da 0)
            'mat_Cod = 3024
            If calibri.RigheScelte IsNot Nothing AndAlso calibri.RigheScelte.Any Then
                Dim Scrittura_OK As Boolean = False
                objParametriServer.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica
                For Each ca As CalibriModel In RigheScelte
                    Scrittura_OK = obj.Cancella(piva,
                                           sa_cod,
                                           mat_Cod,
                                           "calibro",
                                          ca.Tipo_Cod,
                                          0,
                                          "",
                                          objParametriServer)

                    If Not Scrittura_OK Then
                        Return True
                    Else
                        If ca.Selected Then
                            Scrittura_OK = obj.Scrivi(piva,
                                           sa_cod,
                                           mat_Cod,
                                          "calibro",
                                          ca.Tipo_Cod,
                                          0,
                                          "",
                                          0,
                                          0,
                                          0,
                                          0,
                                          AGRODATAINIZIO,
                                          AGRODATAFINE,
                                          objParametriServer)
                            If Not Scrittura_OK Then
                                Return True
                            End If
                        End If

                    End If
                Next
            End If

        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function

    Private Shared Function SalvaParametriQualitativiIndici(ByVal piva As String,
       ByVal mat_Cod As Integer,
       ByVal sa_cod As Integer,
       ByVal veg_cod As Integer,
       ByRef errori As String,
       ByVal indici As ParametriQualitativiIndiciModel) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim RigheScelte As List(Of IndiciModel) = JsonConvert.DeserializeObject(Of List(Of IndiciModel))(indici.RigheScelte, settingLoc)
            Dim obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

            'Prima Elimino gli indici scelti già presenti nella tabella Materie_Prime_ParametriQualitativi e po li reinserisco con gli indici nuovi
            'PS(Udm_Cod lo passo a 0 perché nella tabella non c'è nessuna riga con Udm_Cod diverso da 0)
            'mat_Cod = 3024
            If indici.RigheScelte IsNot Nothing AndAlso indici.RigheScelte.Any Then
                Dim Scrittura_OK As Boolean = False
                objParametriServer.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica
                For Each ind As IndiciModel In RigheScelte
                    Scrittura_OK = obj.Cancella(piva,
                                               sa_cod,
                                               mat_Cod,
                                               "indice",
                                              ind.IND_MAT_COD,
                                              0,
                                              "",
                                              objParametriServer)

                    If Not Scrittura_OK Then
                        Return True
                    Else
                        If ind.Selected Then
                            Scrittura_OK = obj.Scrivi(piva,
                                                       sa_cod,
                                                       mat_Cod,
                                                      "indice",
                                                      ind.IND_MAT_COD,
                                                      0,
                                                      "",
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      AGRODATAINIZIO,
                                                      AGRODATAFINE,
                                                      objParametriServer)
                            If Not Scrittura_OK Then
                                Return True
                            End If
                        End If

                    End If
                Next
            End If
        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function

    Private Shared Function SalvaAlias(ByVal piva As String,
                                       ByVal mat_cod As Integer,
                                       ByVal sa_cod As Integer,
                                        ByVal descrizioni_alternative As DescrizioniAlternativeModel,
                                        ByRef errori As String) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim righeInserite As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheInserite, settingLoc)
            Dim obj As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_W

            Dim righeEliminate As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheEliminate, settingLoc)
            If righeEliminate IsNot Nothing AndAlso righeEliminate.Any Then
                Dim Cancella_OK As Boolean = False
                For Each a As MateriePrimeAliasModel In righeEliminate
                    Cancella_OK = obj.Cancella(piva,
                                               sa_cod,
                                               mat_cod,
                                               a.Mat_Cod_Alias,
                                               "",
                                                objParametriServer
                                                    )
                    If Not Cancella_OK Then
                        Return True
                    End If
                Next
            End If

            If righeInserite IsNot Nothing AndAlso righeInserite.Any Then
                Dim Scrittura_OK As Boolean = False
                For Each a As MateriePrimeAliasModel In righeInserite
                    Scrittura_OK = obj.Scrivi(piva,
                                              sa_cod,
                                              mat_cod,
                                              a.Mat_Cod_Alias,
                                              AGRODATAINIZIO,
                                              AGRODATAFINE,
                                              a.Codice_Lingua,
                                              a.Filtro_Contatti_String,
                                              objParametriServer
                                              )

                    If Not Scrittura_OK Then
                        Return True
                    End If

                Next
            End If

            Dim righeModificate As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheModificate, settingLoc)
            If righeModificate IsNot Nothing AndAlso righeModificate.Any Then
                Dim Modifica_OK As Boolean = False
                For Each a As MateriePrimeAliasModel In righeModificate

                    Dim Mat_Cod_Alias_Old As Integer = a.Mat_Cod_Alias

                    If a.Mat_Cod_Alias_Old IsNot Nothing Then
                        Mat_Cod_Alias_Old = a.Mat_Cod_Alias_Old
                    End If

                    Modifica_OK = obj.Modifica(piva,
                                               sa_cod,
                                               mat_cod,
                                               Mat_Cod_Alias_Old,
                                               a.Mat_Cod_Alias,
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               a.Codice_Lingua,
                                               a.Filtro_Contatti_String,
                                               objParametriServer)

                    If Not Modifica_OK Then
                        Return True
                    End If
                Next
            End If

        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function

    Private Shared Function SalvaMateriaPrima(ByVal parametri As SalvaProdottoModel, ByRef new_mat_cod As Integer, ByRef errori As String) As Boolean

        'NOTA:
        'Nella Anagrafica Prodotti del LAN veniva scritta anche la Tabella Materie_Prime_Dettagli che è ora in disuso in questa versione dell'anagrafica Prodotti.
        'Materie_Prime_Dettagli ora viene scritta per esempio per i Prodotti con flag_importato=1 provenienti dall'import delle anagrafiche fruttagel da JDE. 
        'Per saperne di più su questa tabella consultare la documentazione:
        '"\\rubino2\DOCUMENTAZIONE\GIAS --- Anagrafica\Materie prime\Materie_Prime_Dettagli\Materie_Prime_Dettagli - struttura e campi utilizzati.docx"

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim obj As New Materie_Prime_W


        Dim Gen_Cod As Integer = 0
        Dim Spe_Cod As Integer = 0
        Dim IPro_Cod As Integer = 0
        Dim Raz_Cod As Integer = 0
        Dim Cat_Cod As Integer = 0
        Dim Flag_Biologico As Int16 = 0
        Dim Flag_Convenzionale As Int16 = 0
        Dim Flag_NonAgricolo As Int16 = 0
        Dim Flag_AusiliareFabbricazione As Int16 = 0
        Dim Trap_Dur As Integer = 0
        Dim Uso As Integer = 0
        Dim ClToss_Cod As String = "0"
        Dim NewClToss_Cod As String = "0"
        Dim Ditta_Cod As Integer = 0
        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim MgO As Decimal = 0
        Dim Note As String = ""
        Dim Regolamento As Integer = 0
        Dim Cal_Cod As Integer = 0
        Dim Cod_Articolo As String = ""
        Dim Prezzo_Unitario As Decimal = 0
        Dim Extra_Int As Integer = 0
        Dim Extra_Str As String = 0
        Dim Extra_Date As Date = AGRODATAFINE
        Dim Grva_Cod_Veg As Integer = 0
        Dim Flag_Extra As Int16 = 0
        Dim ChkImballaggio As Int16 = 0
        Dim Udm_Cod_Extra As Integer = 0
        Dim Qta_Extra As Decimal = 0
        Dim Taglio As Int16 = 0
        Dim Grfi_Cod As Integer = 0
        Dim ChkListino As Int16 = 0
        Dim Mat_Cod_Origine As Integer = 0
        Dim Piva_SuperUser_Origine As String = ""
        Dim Codice_Esterno As String = ""
        Dim Flag_Importato As Int32 = 0
        Dim Codice_Prodotto As String = ""
        Dim Colore As Int16 = 0
        Dim Codice_NC As String = ""
        Dim Manipolazioni As Integer = 0
        Dim Titolo_Alcol As Decimal = 0
        Dim ChkContenitore As Int16 = 0
        Dim Qta_Contenitore As Decimal = 0
        Dim Tipo_Peso As Int16 = 0
        Dim Tara As Decimal = 0
        Dim Udm_Cod As Integer = 0
        Dim Peso_Set As Int16 = 0
        Dim ChkEscludi_Magazzino As Int16 = 0
        Dim ChkEscludi_Preparazione As Int16 = 0
        Dim Id_Accisa_Cod As Integer = 0
        Dim Confezione_Cod As String = ""
        Dim Categoria_Vino_Cod As Int16 = 0
        Dim Tipo_Reg_Alcoli As String = 0
        Dim ChkAlias As Int16 = 0
        Dim Linea_Cod As Integer = 0
        Dim Tipo_Default As Integer = 0
        Dim ChkStampa_Dettagli As Int16 = 0
        Dim ChkReferenza As Int16 = 0
        Dim Mat_Cod_Referenza As Integer = 0
        Dim ID_DisciplinareAcquisti As Integer = 0
        Dim Lotto_Default As String = ""
        Dim OTabella_Cod_Base As Integer = 0
        Dim Provenienza_TR As String = ""
        Dim eBacchus As String = ""
        Dim Flag_Variazione As Int16 = 0
        Dim ELem_COd As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Sem_Cod As Integer = 0
        Dim CUl_Cod As Integer = 0
        Dim Veg_Cod As Integer = 0
        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE
        Dim Cod_TecnologiaSementi As Integer? = 0
        Dim Germinabilita As Double? = 0


        Dim CodiceProdotto As String = parametri.CodiceProdotto
        Dim ChkConfezione As Int16 = 0
        Dim config As ConfigurazioniModel = parametri.Configurazione
        Dim dTec As DatiTecniciModel = parametri.DatiTecnici
        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim Base, Top As Integer

        Try

            ELem_COd = parametri.CategoriaProdotto
            If dTec.SementiEMaterialiVivaisti IsNot Nothing AndAlso dTec.SementiEMaterialiVivaisti.HasValue Then
                Sem_Cod = dTec.SementiEMaterialiVivaisti
            End If
            If dTec.SpecieVegetali IsNot Nothing AndAlso dTec.SpecieVegetali.HasValue Then
                If dTec.SpecieVegetali = -1 Then
                    Veg_Cod = 0
                Else
                    Veg_Cod = dTec.SpecieVegetali
                End If
            End If
            If dTec.VarietaColturale IsNot Nothing AndAlso dTec.VarietaColturale.HasValue Then
                CUl_Cod = dTec.VarietaColturale
            End If
            If dTec.Regolamento IsNot Nothing AndAlso dTec.Regolamento.HasValue Then
                Regolamento = dTec.Regolamento
            End If
            If dTec.DittaDiProvenienza IsNot Nothing AndAlso dTec.DittaDiProvenienza.HasValue Then
                Ditta_Cod = dTec.DittaDiProvenienza
            End If
            If dTec.TipologiaVarietale IsNot Nothing AndAlso dTec.TipologiaVarietale.HasValue Then
                Grva_Cod_Veg = dTec.TipologiaVarietale
            End If
            If dTec.Referenza IsNot Nothing AndAlso dTec.Referenza.HasValue Then
                Mat_Cod_Referenza = dTec.Referenza
                ChkReferenza = 0
            End If
            If parametri.Categoria_Risorsa IsNot Nothing AndAlso parametri.Categoria_Risorsa.HasValue Then
                Cat_Cod = parametri.Categoria_Risorsa
            End If
            If parametri.Linea_Cod IsNot Nothing AndAlso parametri.Linea_Cod.HasValue Then
                Linea_Cod = parametri.Linea_Cod
            End If
            If parametri.ChkReferenza IsNot Nothing AndAlso parametri.ChkReferenza.HasValue Then
                ChkReferenza = parametri.ChkReferenza
            End If
            If dTec.Finalita_Produttiva IsNot Nothing AndAlso dTec.Finalita_Produttiva.HasValue Then
                Grfi_Cod = dTec.Finalita_Produttiva
            End If
            If dTec.UDM_Cod IsNot Nothing AndAlso dTec.UDM_Cod.HasValue Then
                Udm_Cod = dTec.UDM_Cod
            End If
            If Not IsNothing(dTec.Cod_TecnologiaSementi) AndAlso dTec.Cod_TecnologiaSementi.HasValue Then
                Cod_TecnologiaSementi = dTec.Cod_TecnologiaSementi
            End If
            If Not IsNothing(dTec.Germinabilita) AndAlso dTec.Germinabilita.HasValue Then
                Germinabilita = dTec.Germinabilita
            End If
            If dTec.SpecieAnimali IsNot Nothing Then
                If dTec.SpecieAnimali = "0" Then
                    Gen_Cod = 0
                    Spe_Cod = 0
                    IPro_Cod = 0
                    Raz_Cod = 0
                ElseIf dTec.SpecieAnimali <> "0" AndAlso dTec.SpecieAnimali.Length >= 2 Then
                    Gen_Cod = CInt(dTec.SpecieAnimali.Split("|")(0))
                    Spe_Cod = CInt(dTec.SpecieAnimali.Split("|")(1))
                    IPro_Cod = -1
                    Raz_Cod = -1
                End If
            End If
            If dTec.IndirizzoAnimale IsNot Nothing Then
                If dTec.IndirizzoAnimale = "-1" Then
                    IPro_Cod = CInt(dTec.IndirizzoAnimale)
                ElseIf dTec.IndirizzoAnimale <> "-1" AndAlso dTec.IndirizzoAnimale.Length >= 3 Then
                    IPro_Cod = CInt(dTec.IndirizzoAnimale.Split("|")(2))
                End If
            End If
            If dTec.RazzaAnimale IsNot Nothing Then
                If dTec.RazzaAnimale = "-1" Then
                    Raz_Cod = CInt(dTec.RazzaAnimale)
                ElseIf dTec.RazzaAnimale <> "-1" AndAlso dTec.RazzaAnimale.Length >= 3 Then
                    Raz_Cod = CInt(dTec.RazzaAnimale.Split("|")(2))
                End If

            End If
            If parametri.Cal_Cod IsNot Nothing Then
                Cal_Cod = CInt(parametri.Cal_Cod)
            End If
            If parametri.Codice_Esterno IsNot Nothing Then
                Codice_Esterno = parametri.Codice_Esterno
            End If
            If parametri.Flag_Importato IsNot Nothing Then
                Flag_Importato = CInt(parametri.Flag_Importato)
            End If

            Flag_Biologico = dTec.Flag_Biologico
            Flag_Convenzionale = dTec.Flag_Convenzionale
            Flag_AusiliareFabbricazione = dTec.Flag_Ausiliare_Fabbricazione
            Flag_NonAgricolo = dTec.Flag_Non_Agricolo

            Extra_Str = If(String.IsNullOrEmpty(dTec.Descrizione_Addizionale), "", dTec.Descrizione_Addizionale)
            Note = If(String.IsNullOrEmpty(dTec.Note), "", dTec.Note)

            If parametri.ChkAlias IsNot Nothing Then
                ChkAlias = CInt(parametri.ChkAlias)
            End If


            If config IsNot Nothing Then
                'Impostazione Configurazione in base al Flag_Udm_Cod_extra
                Imposta_Configurazione(config, ELem_COd)

                Flag_Extra = config.Flag_Extra
                ChkImballaggio = config.Flag_Imballaggio
                ChkContenitore = config.Flag_Contenitore
                ChkConfezione = config.Flag_Confezione
                ChkEscludi_Magazzino = config.Flag_EscludiMagazzino
                ChkEscludi_Preparazione = config.Flag_EscludiPreparazione
                Flag_Variazione = config.Flag_Variazione

                If config.Otabella_Cod_Base IsNot Nothing Then
                    OTabella_Cod_Base = CInt(config.Otabella_Cod_Base)
                End If
                If config.Peso_Set IsNot Nothing Then
                    Peso_Set = CInt(config.Peso_Set)
                End If
                If config.TipoPeso IsNot Nothing Then
                    Tipo_Peso = CInt(config.TipoPeso)
                End If
                If config.Tara IsNot Nothing Then
                    Tara = CDec(config.Tara)
                End If
                If config.Qta_Extra IsNot Nothing Then
                    Qta_Extra = CDec(config.Qta_Extra)
                End If
                If config.Udm_Cod_Extra IsNot Nothing Then
                    Udm_Cod_Extra = CInt(config.Udm_Cod_Extra)
                End If
                If config.Confezione_Cod IsNot Nothing Then
                    Confezione_Cod = config.Confezione_Cod
                End If
                If config.Contenitore IsNot Nothing Then
                    Qta_Contenitore = CDec(config.Contenitore)
                End If

                'se si tratta di BENI_CONFEZ_VEGETALE -> utilizzo Flag_CompLotto
                'altrimenti se ALTRE_MATERIE -> utilizzo Flag_UtilizzoCA
                Select Case ELem_COd
                    Case BENI_CONFEZ_VEGETALE
                        If config.Flag_CompLotto IsNot Nothing Then
                            Extra_Int = config.Flag_CompLotto
                        End If
                    Case ALTRE_MATERIE
                        If config.Flag_UtilizzoCA IsNot Nothing Then
                            Extra_Int = config.Flag_UtilizzoCA
                        End If
                End Select

            End If

            Select Case parametri.TipoOperazioneDB
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Copia

                    Calcola_BaseCode_TopCode(Base, Top,
                                             HttpContext.Current.Session("ASG_ProgressivoGIAS"))

                    Mat_Cod = ObjSequenze.NuovoId_Tabella("Materie_Prime", Base, Top, objParametriServer)
                    new_mat_cod = Mat_Cod

                    'Calcolo un nuovo Cal_Cod se è 0 e c'è da creare una referenza
                    If parametri.ParametriQualitativiDatiTecnici IsNot Nothing AndAlso
                       parametri.ParametriQualitativiDatiTecnici.Campi IsNot Nothing Then

                        Cal_Cod = -Math.Abs(ObjSequenze.NuovoId_Tabella("Materie_Prime_Campionature", 0, 2000000000, objParametriServer))
                        parametri.Cal_Cod = Cal_Cod
                    End If

                    'Se parametri.Componi_Cod_ArticoloDaCodice_Esterno = True Ricavo il Codice_Articolo da il Codice_Esterno_Mat_Cod
                    If Flag_Importato = 0 Then
                        If parametri.Componi_Cod_ArticoloDaCodice_Esterno AndAlso
                            Codice_Esterno IsNot Nothing AndAlso Codice_Esterno <> "" AndAlso
                             Mat_Cod <> 0 AndAlso CodiceProdotto = "" Then

                            CodiceProdotto = Codice_Esterno.ToString() & "_" & Mat_Cod.ToString()
                        End If
                    End If



                    obj.Scrivi_Completa(parametri.Piva, dTec.Sa_Cod, ELem_COd, Mat_Cod, CodiceProdotto,
                                        parametri.Descrizione, Sem_Cod, CUl_Cod, Veg_Cod, Trap_Dur, Uso,
                                        ClToss_Cod, NewClToss_Cod, Ditta_Cod, N, P2O5, K2O,
                                        MgO, Note, Regolamento, Cal_Cod, Prezzo_Unitario,
                                        Extra_Int, Extra_Str, Extra_Date, Grva_Cod_Veg, Gen_Cod,
                                        Spe_Cod, Raz_Cod, IPro_Cod, Cat_Cod, Flag_Biologico,
                                        Flag_Convenzionale, Flag_NonAgricolo, Flag_AusiliareFabbricazione, Udm_Cod_Extra,
                                        Flag_Extra, ChkImballaggio, Qta_Extra, Taglio, Mat_Cod_Origine,
                                        Piva_SuperUser_Origine, ChkListino, Grfi_Cod, Codice_Prodotto, Colore, Codice_NC,
                                        Manipolazioni, Titolo_Alcol, ChkContenitore, Qta_Contenitore, Tipo_Peso, Tara, Udm_Cod,
                                        Peso_Set, ChkEscludi_Magazzino, ChkEscludi_Preparazione, Id_Accisa_Cod, Confezione_Cod,
                                        Categoria_Vino_Cod, Tipo_Reg_Alcoli, ChkAlias, Linea_Cod, Tipo_Default,
                                        ChkStampa_Dettagli, ChkReferenza, Mat_Cod_Referenza, ID_DisciplinareAcquisti,
                                        Lotto_Default, OTabella_Cod_Base, Provenienza_TR, eBacchus, Flag_Variazione,
                                        Codice_Esterno, Flag_Importato, dTec.PrioritaCdG, Cod_TecnologiaSementi, Germinabilita, Validita_Inizio, Validita_Fine, objParametriServer)


                Case enum_TipoOperazioneDB.Modifica

                    If parametri.Proprietario = True Then
                        Mat_Cod = parametri.Mat_Cod

                        'Calcolo un nuovo Cal_Cod se è 0 e c'è da creare una referenza
                        If parametri.ParametriQualitativiDatiTecnici IsNot Nothing AndAlso
                            parametri.ParametriQualitativiDatiTecnici.Campi IsNot Nothing Then

                            If Cal_Cod = 0 Then
                                Cal_Cod = -Math.Abs(ObjSequenze.NuovoId_Tabella("Materie_Prime_Campionature", 0, 2000000000, objParametriServer))
                            End If

                            parametri.Cal_Cod = Cal_Cod

                        ElseIf dTec.Elimina_Referenza_ParamQualDTec IsNot Nothing AndAlso
                                dTec.Elimina_Referenza_ParamQualDTec = True Then

                            Cal_Cod = 0

                        End If

                        Dim NuovoCodiceProdotto As String = parametri.NuovoCodiceProdotto

                        'Se parametri.Componi_Cod_ArticoloDaCodice_Esterno = True Ricavo il Codice_Articolo da il Codice_Esterno_Mat_Cod
                        If Flag_Importato = 0 AndAlso CodiceProdotto = "" Then
                            If parametri.Componi_Cod_ArticoloDaCodice_Esterno AndAlso
                            Codice_Esterno IsNot Nothing AndAlso Codice_Esterno <> "" AndAlso
                             Mat_Cod <> 0 Then
                                NuovoCodiceProdotto = Codice_Esterno.ToString() & "_" & Mat_Cod.ToString()
                            End If
                        End If

                        obj.Modifica_Completa(parametri.Piva, dTec.Sa_Cod, ELem_COd, Mat_Cod, NuovoCodiceProdotto, CodiceProdotto,
                                            parametri.Descrizione, Sem_Cod, CUl_Cod, Veg_Cod, Trap_Dur, Uso,
                                            ClToss_Cod, NewClToss_Cod, Ditta_Cod, N, P2O5, K2O,
                                            MgO, Note, Regolamento, Cal_Cod, Prezzo_Unitario,
                                            Extra_Int, Extra_Str, Extra_Date, Grva_Cod_Veg, Gen_Cod,
                                            Spe_Cod, Raz_Cod, IPro_Cod, Cat_Cod, Flag_Biologico,
                                            Flag_Convenzionale, Flag_NonAgricolo, Flag_AusiliareFabbricazione, Udm_Cod_Extra,
                                            Flag_Extra, ChkImballaggio, Qta_Extra, Taglio, Mat_Cod_Origine,
                                            Piva_SuperUser_Origine, ChkListino, Grfi_Cod, Codice_Prodotto, Colore, Codice_NC,
                                            Manipolazioni, Titolo_Alcol, ChkContenitore, Qta_Contenitore, Tipo_Peso, Tara, Udm_Cod,
                                            Peso_Set, ChkEscludi_Magazzino, ChkEscludi_Preparazione, Id_Accisa_Cod, Confezione_Cod,
                                            Categoria_Vino_Cod, Tipo_Reg_Alcoli, ChkAlias, Linea_Cod, Tipo_Default,
                                            ChkStampa_Dettagli, ChkReferenza, Mat_Cod_Referenza, ID_DisciplinareAcquisti,
                                            Lotto_Default, OTabella_Cod_Base, Provenienza_TR, eBacchus, Flag_Variazione,
                                            Codice_Esterno, Flag_Importato, dTec.PrioritaCdG, Cod_TecnologiaSementi, Germinabilita, Validita_Inizio, Validita_Fine, objParametriServer)
                    End If

            End Select

            If (config IsNot Nothing AndAlso
               (listModuliAttivi_anagrafe_log IsNot Nothing AndAlso listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood))) Then

                'Scrivo anche su Otabelle_Parametri se sono un bene di confezionamento vegetale ed è stato scelto un imballaggio,
                'confezione o contenitore
                Dim tabella_cod As Integer = 0

                If ChkImballaggio <> 0 Then
                    tabella_cod = enum_OTabelle.Imballaggio
                End If

                If ChkConfezione <> 0 Then
                    tabella_cod = enum_OTabelle.Confezione
                End If

                If ChkContenitore <> 0 Then
                    tabella_cod = enum_OTabelle.Contenitore
                End If

                Dim obj_OTB_R As New OTabelle_R
                Dim obj_OTB_W As New OTabelle_W
                If tabella_cod <> 0 Then

                    Dim DT As DataTable
                    Dim Filtro As String = "OTabelle_Parametri.Mat_Cod_Generazione_Link= " & Agro_SQL_SaveNum(Mat_Cod)
                    DT = obj_OTB_R.LeggiParametri(parametri.Piva, 0, enum_Omni_Modulo_Generazione.FreshFood, Filtro, objParametriServer)

                    If DT.Rows.Count > 0 Then
                        Dim Par_cod As String = DT.Rows(0).Item("Tabella_Par_Cod").ToString()

                        If Par_cod IsNot Nothing Then
                            'Modifica
                            obj_OTB_W.ModificaParametro(parametri.Piva, tabella_cod, CInt(Par_cod), enum_Omni_Modulo_Generazione.FreshFood, parametri.Descrizione, "", "",
                                                config.Filtro_Veg_Cod, config.Filtro_Cul_Cod, Mat_Cod, "", objParametriServer)
                        End If
                    Else
                        Dim Tabella_Par_Cod = ObjSequenze.NuovoId_Tabella("omni_tabella_parametri", 0, 2000000000, objParametriServer)
                        obj_OTB_W.ScriviParametro(parametri.Piva, tabella_cod, Tabella_Par_Cod, enum_Omni_Modulo_Generazione.FreshFood, parametri.Descrizione, parametri.CodiceProdotto,
                                                  parametri.CodiceProdotto, config.Filtro_Veg_Cod, config.Filtro_Cul_Cod, Mat_Cod, objParametriServer)
                    End If
                Else
                    If config.ParamQualOriginaleDettagliBeniConf <> 0 AndAlso config.ParamQualOriginaleDettagliBeniConf <> tabella_cod Then
                        'Elimina
                        obj_OTB_W.CancellaParametro(parametri.Piva, config.ParamQualOriginaleDettagliBeniConf, 0,
                                                    enum_Omni_Modulo_Generazione.FreshFood, Mat_Cod, objParametriServer)
                    End If
                End If


            End If

        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function

    Private Shared Sub Imposta_Configurazione(ByRef config As ConfigurazioniModel, ByVal elem_cod As Integer)

        Select Case config.Flag_Udm_Cod_Extra

            'Flag_Udm_Cod_Extra è checkato
            Case 1

                'Definizione Codice Imballaggio
                If Not config.Flag_Imballaggio.HasValue OrElse Not config.Flag_Contenitore.HasValue OrElse config.Confezione_Cod Is Nothing Then
                    config.Confezione_Cod = ""
                End If

                If Not IsNumeric(config.Qta_Extra) Then
                    config.Qta_Extra = 0
                End If

                If Not IsNumeric(config.Tara) Then
                    config.Tara = 0
                End If

                '=============================================================================================================
                'Qta Contenitore
                '-------------------------------------------------------------------------------------------------------------
                Select Case elem_cod

                    Case BENI_CONFEZ_ANIMALE, BENI_CONFEZ_VEGETALE 'Beni Confezionamento

                        If Not IsNumeric(config.Contenitore) Then
                            config.Contenitore = 0
                        End If

                    Case Else
                        If config.Flag_Contenitore_Conf = 1 And IsNumeric(config.Contenitore_Conf) Then
                            config.Contenitore = config.Contenitore_Conf
                        Else
                            config.Contenitore = 0
                        End If

                End Select


            'Flag_Udm_Cod_Extra non è checkato
            Case 0
                'Se sono un BENE DI CONFEZIONAMENTO VEGETALE O ANIMALE azzero i parametri dei beni di confezionamento,
                'perchè non è checkato Flag_Udm_Cod_Extra
                config.Flag_Extra = 0
                config.Flag_Imballaggio = 0
                config.Contenitore = 0
                config.Confezione_Cod = ""
                config.Udm_Cod_Extra = 0
                config.Qta_Extra = 0
                config.Tara = 0
                config.TipoPeso = -1
                config.Peso_Set = 0
                config.Otabella_Cod_Base = 0
                config.Flag_Confezione = 0
                config.Flag_Contenitore = 0
        End Select

    End Sub

    Private Shared Function Controlli_ParametriQualitativiDatiTecnici(ByVal param As ParametriQualitativiDatiTecniciModel, ByVal UDM_Cod As Integer, ByRef errori As String) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim campi As List(Of ParametriQualitativi) = JsonConvert.DeserializeObject(Of List(Of ParametriQualitativi))(param.Campi, settingLoc)
        If campi IsNot Nothing Then
            Dim confezione = campi.FirstOrDefault(Function(f) (f.Valore_Des = "5"))
            Dim contenitore = campi.FirstOrDefault(Function(f) (f.Valore_Des = "8"))

            'Non fare questo controllo se l'unità di misura di default è numero
            If confezione IsNot Nothing AndAlso contenitore IsNot Nothing AndAlso UDM_Cod <> 38 Then

                If CInt(confezione.Val_Cod) < CInt(contenitore.Val_Cod) Then
                    errori &= DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NumeroConfezioniPerContenitoreIncoerente"), String)
                    Return False
                End If
            End If

        Else
            errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "ErroreParametriQualitativi"), String)
            Return False
        End If

        Return True

    End Function

    Private Shared Function Controlli_ConfigurazioneProdotti(ByVal config As ConfigurazioniModel, ByRef errori As String) As Boolean

        'Ora non più obbligatorio
        'If config.Flag_Contenitore = 1 Then

        '    If Agro_SQL_SaveNum(config.Contenitore) = 0 Then
        '        'i18N Numero Beni Contenuti Non Impostato da tradurre
        '        errori += "Numero Beni Contenuti Non Impostato Correttamente."
        '        Return False
        '    End If

        'End If

        '======================================================================================================================
        'Controllo impostazione peso/capacità nominale
        '----------------------------------------------------------------------------------------------------------------------
        If config.Flag_Udm_Cod_Extra = 1 And config.Peso_Set = 0 Then

            If Not IsNumeric(config.Qta_Extra) Then
                errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "PesoCapacitaNominaleNonImpostato"), String)
                Return False
            End If


            If CDec(config.Qta_Extra) = 0 And config.Flag_Imballaggio = 0 And config.Flag_Contenitore = 0 Then
                errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "PesoCapacitaNominaleNullo"), String)
                Return False
            End If

        End If

        Return True

    End Function


    Private Shared Function SalvaDatiContabilitaEAltriDati(ByVal piva As String,
        ByVal elem_Cod As Integer,
        ByVal mat_Cod As Integer,
        ByVal pro_Cod As Integer, ByVal dati_contab As DatiContabilitaModel, ByVal altri_dati As AltriDatiModel,
        ByVal Proprietario As Boolean, ByVal MateriaPrima As Boolean, ByRef errori As String) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim obj_W = New Prodotti_Extra_Privata_W


        Try
            Dim Salva As Boolean = False
            Dim TipoOperazione As Integer

            Dim Aliquota_Iva As Integer? = Nothing
            Dim Aliquota_Iva_Compensazione As Integer? = Nothing
            Dim Conto_Economico_Acquisto As Integer? = Nothing
            Dim Conto_Economico_Vendita As Integer? = Nothing
            Dim Conto_Patrimoniale_Acquisto As Integer? = Nothing
            Dim Conto_Patrimoniale_Vendita As Integer? = Nothing
            Dim Id_Gruppo_Merce As Integer? = Nothing

            Dim EAN As String = ""
            Dim Barcode As String = ""
            Dim Ingredienti As String = ""
            Dim Produzione_Propria As Integer? = Nothing
            Dim SalvaAllegatoImmagine As Integer = 0
            Dim ImmaginefileByteArray As Byte() = Nothing
            Dim Immagine_Estensione As String = ""
            Dim Immagine_NomeFile As String = ""
            Dim Peso_Netto As Integer? = Nothing
            Dim Peso_Sgocciolato As Decimal? = Nothing
            Dim Tara As Decimal? = Nothing
            Dim Peso_Egalizzato As Integer? = Nothing

            If dati_contab IsNot Nothing Then
                Aliquota_Iva = If(dati_contab.Aliquota_Iva = -1, Nothing, dati_contab.Aliquota_Iva)
                Aliquota_Iva_Compensazione = If(dati_contab.Aliquota_Iva_Compensazione = 0, Nothing, dati_contab.Aliquota_Iva_Compensazione)
                Conto_Economico_Acquisto = If(dati_contab.Conto_Economico_Acquisto = -1, Nothing, dati_contab.Conto_Economico_Acquisto)
                Conto_Economico_Vendita = If(dati_contab.Conto_Economico_Vendita = -1, Nothing, dati_contab.Conto_Economico_Vendita)
                Conto_Patrimoniale_Acquisto = If(dati_contab.Conto_Patrimoniale_Acquisto = -1, Nothing, dati_contab.Conto_Patrimoniale_Acquisto)
                Conto_Patrimoniale_Vendita = If(dati_contab.Conto_Patrimoniale_Vendita = -1, Nothing, dati_contab.Conto_Patrimoniale_Vendita)
                Id_Gruppo_Merce = If(dati_contab.Id_Gruppo_Merce = 0, Nothing, dati_contab.Id_Gruppo_Merce)

                TipoOperazione = dati_contab.TipoOperazioneDB
            End If

            If altri_dati IsNot Nothing Then
                EAN = altri_dati.EAN
                Barcode = altri_dati.Barcode
                Ingredienti = altri_dati.Ingredienti
                Produzione_Propria = altri_dati.Produzione_Propria

                If altri_dati.Immagine <> "" AndAlso
                    altri_dati.Immagine_Estensione <> "" AndAlso
                    altri_dati.Immagine_NomeFile <> "" Then

                    ' scrivo file allegato
                    SalvaAllegatoImmagine = 1
                    ImmaginefileByteArray = Convert.FromBase64String(altri_dati.Immagine)
                    Immagine_Estensione = altri_dati.Immagine_Estensione
                    Immagine_NomeFile = altri_dati.Immagine_NomeFile
                End If

                Peso_Netto = altri_dati.Peso_Netto
                Peso_Sgocciolato = altri_dati.Peso_Sgocciolato
                Tara = altri_dati.Tara
                Peso_Egalizzato = altri_dati.Peso_Egalizzato
                TipoOperazione = altri_dati.TipoOperazioneDB
            End If


            'Se sono tutti vuoti i dati contabilità e i gli altri dati non sto a scrivere (o modificare) una riga tutta vuota ma non la scrivo
            'oppure la cancello
            If Aliquota_Iva Is Nothing AndAlso Aliquota_Iva_Compensazione Is Nothing AndAlso
                Conto_Economico_Acquisto Is Nothing AndAlso Conto_Economico_Vendita Is Nothing AndAlso
                Conto_Patrimoniale_Acquisto Is Nothing AndAlso Conto_Patrimoniale_Vendita Is Nothing AndAlso
                  altri_dati IsNot Nothing AndAlso EAN = "" AndAlso Barcode = "" AndAlso Ingredienti = "" AndAlso
                  Produzione_Propria = 0 AndAlso SalvaAllegatoImmagine = 0 AndAlso (Peso_Netto Is Nothing OrElse Peso_Netto = 0) AndAlso
                  (Peso_Sgocciolato Is Nothing OrElse Peso_Sgocciolato = 0) AndAlso (Tara Is Nothing OrElse Tara = 0) AndAlso Peso_Egalizzato = -1 AndAlso
                  Id_Gruppo_Merce Is Nothing Then

                obj_W.Cancella("", piva, mat_Cod, elem_Cod, pro_Cod, objParametriServer)

            Else
                'Con Peso_Egalizzato -1 se altri campi non sono impostati non salvi il record; se invece c’è almeno un altro campo imposti in automatico 0
                If Peso_Egalizzato = -1 Then

                    If Aliquota_Iva Is Nothing AndAlso Aliquota_Iva_Compensazione Is Nothing AndAlso
                        Conto_Economico_Acquisto Is Nothing AndAlso Conto_Economico_Vendita Is Nothing AndAlso
                        Conto_Patrimoniale_Acquisto Is Nothing AndAlso Conto_Patrimoniale_Vendita Is Nothing AndAlso
                        EAN = "" AndAlso Barcode = "" AndAlso Ingredienti = "" AndAlso
                        Produzione_Propria = 0 AndAlso SalvaAllegatoImmagine = 0 AndAlso (Peso_Netto Is Nothing OrElse Peso_Netto = 0) AndAlso
                        (Peso_Sgocciolato Is Nothing OrElse Peso_Sgocciolato = 0) AndAlso (Tara Is Nothing OrElse Tara = 0) AndAlso
                        Id_Gruppo_Merce Is Nothing Then

                        'Non salvo il record
                        Salva = False
                    Else
                        Peso_Egalizzato = 0
                        Salva = True
                    End If

                Else
                    Salva = True
                End If

                If Salva Then

                    Select Case TipoOperazione

                        Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Copia
                            obj_W.Scrivi(piva, mat_Cod, elem_Cod, pro_Cod,
                                Aliquota_Iva, Aliquota_Iva_Compensazione,
                               Conto_Economico_Acquisto, Conto_Economico_Vendita,
                               Conto_Patrimoniale_Acquisto, Conto_Patrimoniale_Vendita,
                               AGRODATAINIZIO, AGRODATAFINE,
                               EAN, Barcode, Ingredienti, Produzione_Propria,
                               SalvaAllegatoImmagine, ImmaginefileByteArray, Immagine_Estensione,
                               Immagine_NomeFile, Peso_Netto, Peso_Sgocciolato,
                               Tara, Peso_Egalizzato, Id_Gruppo_Merce, objParametriServer)

                        Case enum_TipoOperazioneDB.Modifica
                            'Se il prodotto è Materia Prima ma non sono Proprietario non posso 
                            'modificare i Dati Contabili e i Dati di vendita ma sono in sola visualizzazione.
                            If Not MateriaPrima OrElse (MateriaPrima AndAlso Proprietario) Then

                                obj_W.Modifica(piva, mat_Cod, elem_Cod, pro_Cod,
                                                Aliquota_Iva, Aliquota_Iva_Compensazione,
                                               Conto_Economico_Acquisto, Conto_Economico_Vendita,
                                               Conto_Patrimoniale_Acquisto, Conto_Patrimoniale_Vendita,
                                               Nothing, Nothing,
                                               EAN, Barcode, Ingredienti, Produzione_Propria,
                                               SalvaAllegatoImmagine, ImmaginefileByteArray, Immagine_Estensione,
                                               Immagine_NomeFile, Peso_Netto, Peso_Sgocciolato,
                                               Tara, Peso_Egalizzato, Id_Gruppo_Merce,
                                               "", objParametriServer)
                            End If


                    End Select
                End If


            End If

        Catch ex As Exception

            errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return True
        End Try

        Return False

    End Function


    Private Shared Function SalvaParametriQualitativiDatiTecnici(ByVal piva As String,
        ByVal elem_Cod As Integer, ByVal mat_Cod As Integer, ByVal Mat_Des As String,
        ByVal Cod_Articolo As String, ByVal Cal_Cod As Integer, OperazioneDB As Integer, ByVal sa_Cod As Integer,
        ByVal udm_Cod As Integer, ByVal Elimina_Referenza As Boolean, ByVal param As ParametriQualitativiDatiTecniciModel, ByRef errori As String) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim obj_MPQ_W As New Materie_Prime_PQ_W
        Dim obj_MPQ_R As New Materie_Prime_PQ_R
        Dim obj_MCP_W As New Materie_Prime_Campion_W
        Dim obj_MCP_R As New Materie_Prime_Campionature_R

        Dim Risp As Boolean = True

        If listModuliAttivi_anagrafe_log IsNot Nothing AndAlso
           (listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
           listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Zoo)) Then

            Try
                If param.Campi IsNot Nothing AndAlso param.Campi.Any Then

                    Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

                    Dim campi As List(Of ParametriQualitativi) = JsonConvert.DeserializeObject(Of List(Of ParametriQualitativi))(param.Campi, settingLoc)

                    If Cal_Cod <> 0 Then

                        'Prima Elimino gli eventuali Parametri Qualitativi da Materie_Prime_Campionature e 
                        'da Materie_Prime_Parametri_Qualitativi, poi se il flag Elimina_Referenza=False allora
                        'scrivo i nuovi Parametri Qualitativi nelle due tabelle.

                        Dim DT_MPC As DataTable
                        DT_MPC = obj_MCP_R.Leggi(Cal_Cod, "", 0, 0, 0, "", False, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                 "", "", objParametriServer)

                        If DT_MPC.Rows.Count > 0 Then

                            Risp = obj_MCP_W.Cancella(Cal_Cod, "", 0, 0, False, "", objParametriServer)

                            If Not Risp Then
                                Return True
                            End If

                        End If

                        Dim DT_MPQ As DataTable
                        DT_MPQ = obj_MPQ_R.Leggi(piva, 0, mat_Cod, "", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", "", objParametriServer)

                        If DT_MPQ.Rows.Count > 0 Then

                            Risp = obj_MPQ_W.Cancella(piva, 0, mat_Cod, "", 0, 0, "", objParametriServer)

                            If Not Risp Then
                                Return True
                            End If

                        End If


                        For Each cmp As ParametriQualitativi In campi

                            'Salvataggio nella tabella Materie_Prime_Campionature
                            Risp = obj_MCP_W.Scrivi(Cal_Cod, cmp.Tipo_Campion, cmp.Tipo_Cod, udm_Cod, cmp.Val_Cod, "", 0,
                                                         0, "", AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

                            If Not Risp Then
                                Return True
                            End If

                            'Salvataggio nella tabella Materie_Prime_Parametri_Qualitativi
                            Risp = obj_MPQ_W.Scrivi(piva, sa_Cod, mat_Cod, cmp.Tipo_ParamQual, cmp.Tipo_Cod, udm_Cod, cmp.Valore_Des, -2000000000,
                                                    2000000000, 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

                            If Not Risp Then
                                Return True
                            End If

                        Next


                    End If

                    'Caso in cui si cambia specie e varietà di un Trasformato Vegetale che non hanno 
                    'parametri qualitativi associati e quindi bisogna cancellare quelli precedenti.
                ElseIf Elimina_Referenza Then

                    Dim DT_MPC As DataTable
                    DT_MPC = obj_MCP_R.Leggi(Cal_Cod, "", 0, 0, 0, "", False, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             "", "", objParametriServer)

                    If DT_MPC.Rows.Count > 0 Then

                        Risp = obj_MCP_W.Cancella(Cal_Cod, "", 0, 0, False, "", objParametriServer)

                        If Not Risp Then
                            Return True
                        End If

                    End If

                    Dim DT_MPQ As DataTable
                    DT_MPQ = obj_MPQ_R.Leggi(piva, 0, mat_Cod, "", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "", objParametriServer)

                    If DT_MPQ.Rows.Count > 0 Then

                        Risp = obj_MPQ_W.Cancella(piva, 0, mat_Cod, "", 0, 0, "", objParametriServer)

                        If Not Risp Then
                            Return True
                        End If

                    End If
                End If

            Catch ex As Exception

                errori = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                Return True
            End Try
        End If


        If Risp Then
            Return False
        Else
            Return True
        End If

    End Function


    Private Shared Function Controlli_XCategoriaProdotto(ByVal parametri As SalvaProdottoModel, ByRef errori As String) As Boolean

        Dim dtec As DatiTecniciModel = parametri.DatiTecnici

        If Not parametri.Componi_Cod_ArticoloDaCodice_Esterno Then
            ' Controlli su codice articolo
            If Not parametri.Opzioni.ConsentiCodiciDuplicati Then

                ' Check codarticolo immesso non è gia presente
                Dim objMateria As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
                Dim Mat_Cod As Integer

                ' se operazione scrittura faccio chiamata al check passando mat_cod nothing
                If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Copia Then
                    Mat_Cod = Nothing
                Else
                    ' altrimenti passo anche il mat_cod
                    Mat_Cod = parametri.Mat_Cod
                End If

                Dim duplicato As Boolean = False
                Dim CodiceProdotto As String = String.Empty

                ' se objMateria.Check_Cod_Articolo_Esistente() = true errore codice duplicato
                If parametri.NuovoCodiceProdotto IsNot Nothing AndAlso parametri.NuovoCodiceProdotto <> String.Empty Then
                    CodiceProdotto = parametri.NuovoCodiceProdotto
                Else
                    CodiceProdotto = parametri.CodiceProdotto
                End If

                duplicato = objMateria.Check_Cod_Articolo_Esistente(parametri.Piva,
                                                                            Mat_Cod,
                                                                            CodiceProdotto,
                                                                            "",
                                                                            objParametriServer)

                If duplicato Then
                    errori &= String.Format(DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "CodiceProdottoDuplicato"), String), CodiceProdotto) & "<br/>"
                    Return False
                End If
            End If

            If parametri.Opzioni.CodiceMaxLenght > 0 Then
                ' check che lunghezza codice immesso sia <=  parametri.Opzioni.CodiceMaxLenght altrimenti errore
                If parametri.CodiceProdotto.Length > parametri.Opzioni.CodiceMaxLenght Then

                    errori += String.Format(DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "CodiceProdottoMaxLength"), String), parametri.Opzioni.CodiceMaxLenght) & "<br/>"
                    Return False
                End If
            End If

            If parametri.Descrizione.Length < 3 Then
                errori += String.Format(DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "DescrizioneProdottoMinLength"), String), 3) & "<br/>"
                Return False
            End If

        ElseIf parametri.Componi_Cod_ArticoloDaCodice_Esterno Then

            'Controllo che sia stato inserito un Codice_Esterno valido
            If parametri.Codice_Esterno Is Nothing OrElse
                parametri.Codice_Esterno = "" Then
                errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "CodiceEsternoCorretto"), String) & "<br/>"
                Return False
            End If

        End If


        'Non deve essere più obbligatorio il collegamento a un prodotto OMNI
        'quindi per ora rimane commentato.
        '#####################################################################

        'Se dobbiamo salvare un TRASFORMATO VEGETALE, non OMNI (ChkReferenza=0) e siamo in un MODULO_FF è obbligatorio 
        'il prodotto base di riferimento
        'If parametri.CategoriaProdotto = TRASFORMATI_VEGETALI AndAlso parametri.IsModulo_FF = True AndAlso
        '    (Not parametri.ChkReferenza Is Nothing AndAlso parametri.ChkReferenza = 0) AndAlso
        '    (dtec.Referenza Is Nothing OrElse Not dtec.Referenza.HasValue OrElse
        '    dtec.Referenza = 0) Then
        '    'i18N Prodotto Base Di Riferimento da tradurre
        '    errori += "E' necessario specificare un Prodotto Base Di Riferimento! <br>"
        '    Return False
        'End If

        '#####################################################################

        'Se devo salvare un Alias salto i controlli che devono essere fatti per salvare una Categoria di Prodotto
        'perchè per ora posso inserire solo la CATEGORIA MAGAZZINO,CODICE PRODOTTO O CODICE ESTERNO, DESCRIZIONE,
        'VISIBILITA e TRADUZIONI

        If parametri.ChkAlias = 0 Then

            Select Case (parametri.CategoriaProdotto)
                Case SEMENTI

                    If dtec.SementiEMaterialiVivaisti <= 0 OrElse dtec.SementiEMaterialiVivaisti Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareTipologiaSemente"), String) & "<br/>"
                    End If

                    If dtec.SpecieVegetali < 0 OrElse dtec.SpecieVegetali Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareSpecieVegetale"), String) & "<br/>"
                    End If

                    If dtec.VarietaColturale <= 0 OrElse dtec.VarietaColturale Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareVarietàColturale"), String) & "<br/>"
                    End If

                Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI

                    If dtec.SpecieVegetali < 0 OrElse dtec.SpecieVegetali Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareSpecieVegetale"), String) & "<br/>"
                    End If

                    If dtec.VarietaColturale <= 0 OrElse dtec.VarietaColturale Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareVarietàColturale"), String) & "<br/>"
                    End If

                Case BENI_CONFEZ_VEGETALE

                Case MATERIE_VEGETALI

            'Case SEMILAVORATI_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI
                Case SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI

                    If dtec.SpecieAnimali = "" OrElse dtec.SpecieAnimali = "0" OrElse dtec.SpecieAnimali Is Nothing Then
                        errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareSpecieAnimale"), String) & "<br/>"
                    End If

                    If parametri.CategoriaProdotto = SEMILAVORATI_ANIMALI Then
                        If dtec.IndirizzoAnimale = "" OrElse dtec.IndirizzoAnimale = "0" OrElse dtec.IndirizzoAnimale = "-1" OrElse dtec.IndirizzoAnimale Is Nothing Then
                            errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareIndirizzoProduttivoCorretto"), String) & "<br/>"
                        End If

                        If dtec.RazzaAnimale = "" OrElse dtec.RazzaAnimale = "0" OrElse dtec.RazzaAnimale = "-1" OrElse dtec.RazzaAnimale Is Nothing Then
                            errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NecessarioSelezionareRazzaAnimaleCorretta"), String) & "<br/>"
                        End If
                    End If

                Case MATERIE_ANIMALI

                Case MANGIMI

                Case ALTRE_MATERIE

                Case FERTILIZZANTI

            End Select
        End If

        If errori <> String.Empty Then
            Return False
        End If


        Return True

    End Function

    Private Shared Function Controlli_SalvaMateriePrimeXLingue(ByVal traduzioni As TraduzioniModel, ByRef errori As String) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim righeInserite As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheInserite, settingLoc)
        Dim righeModificate As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheModificate, settingLoc)
        Dim righeEliminate As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.RigheEliminate, settingLoc)
        Dim tuttelerighe As List(Of MateriaPrimaXLInguaModel) = JsonConvert.DeserializeObject(Of List(Of MateriaPrimaXLInguaModel))(traduzioni.TutteLeRighe, settingLoc)
        Dim count As Integer = 0


        'Controllo che non ci sia due volte lo stesso Lingua_Cod
        If righeInserite IsNot Nothing Then
            For Each ins As MateriaPrimaXLInguaModel In righeInserite
                Dim lingua_ins = ins.Lingua_Cod
                Dim nome_lingua = ins.Nome
                Dim esistenti = tuttelerighe.Where(Function(f) f.Lingua_Cod = lingua_ins)

                If esistenti.Count > 1 Then
                    errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NonSpecificareDueTraduzioniConStessaLingua"), String) & " " & nome_lingua
                    Return False
                End If

            Next
        End If


        If righeModificate IsNot Nothing Then
            For Each modif As MateriaPrimaXLInguaModel In righeModificate
                Dim lingua_mod = modif.Lingua_Cod
                Dim nome_lingua = modif.Nome
                Dim esistenti = tuttelerighe.Where(Function(f) f.Lingua_Cod = lingua_mod)

                If esistenti.Count > 1 Then
                    errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "NonSpecificareDueTraduzioniConStessaLingua"), String) & " " & nome_lingua
                    Return False
                End If
            Next
        End If


        Return True

    End Function

    Private Shared Function Controlli_SalvaProdottiExtraPrivata(ByVal AltriDati As AltriDatiModel,
                                                                ByVal mat_cod As Integer,
                                                                ByRef errori As String, ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim Barcode = AltriDati.Barcode

        If Barcode <> "" Then
            Dim objExtraPrivata As New Prodotti_Extra_Privata_R

            Dim xfiltroAggiuntivo = "Mat_Cod <> " & mat_cod
            Dim DT = objExtraPrivata.Leggi("", 0, 0, 0, Nothing,
                                           Nothing, Nothing, Nothing, Nothing, Nothing,
                                        "", Barcode, "", Nothing, Nothing, Nothing, Nothing, Nothing,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta, xfiltroAggiuntivo, "", objParametri_Server)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                errori &= DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "BarcodeInternoDuplicato"), String)
                Return False
            End If

        End If

        Return True

    End Function


    Private Shared Function Controlli_SalvaProdottiAlias(ByVal descrizioni_alternative As DescrizioniAlternativeModel,
                                                         ByRef errori As String, ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        'Se sono una Materia Prima posso avere degli alias associati

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim righeInserite As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheInserite, settingLoc)
        Dim righeModificate As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheModificate, settingLoc)
        Dim righeEliminate As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.RigheEliminate, settingLoc)
        Dim tuttelerighe As List(Of MateriePrimeAliasModel) = JsonConvert.DeserializeObject(Of List(Of MateriePrimeAliasModel))(descrizioni_alternative.TutteLeRighe, settingLoc)
        Dim count As Integer = 0

        If righeInserite IsNot Nothing Then
            'Controllo che non ci siano righe inserite con lo stesso Mat_Cod
            For Each ins As MateriePrimeAliasModel In righeInserite
                Dim mat_cod_alias_ins = ins.Mat_Cod_Alias
                Dim mat_des_alias_ins = ins.Mat_Des_Alias

                Dim esistenti = tuttelerighe.Where(Function(f) f.Mat_Cod_Alias = mat_cod_alias_ins)

                If esistenti.Count > 1 Then
                    errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "InseriteDescrizioniAlternativeStessoProdotto"), String) & ": " & mat_des_alias_ins
                    Return False
                End If
            Next
        End If


        If righeModificate IsNot Nothing Then
            'Controllo che non ci siano righe modificate con lo stesso Mat_Cod
            For Each modi As MateriePrimeAliasModel In righeModificate
                Dim mat_cod_alias_modi = modi.Mat_Cod_Alias
                Dim mat_des_alias_modi = modi.Mat_Des_Alias

                Dim esistenti = tuttelerighe.Where(Function(f) f.Mat_Cod_Alias = mat_cod_alias_modi)

                If esistenti.Count > 1 Then
                    errori += DirectCast(HttpContext.GetLocalResourceObject("~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx", "ModificateDescrizioniAlternativeStessoProdotto"), String) & ": " & mat_des_alias_modi
                    Return False
                End If
            Next
        End If

        Return True

    End Function

    Public Shared Function Cancella_Prodotto(ByVal model As String, ByVal mat_cod_referenza As Integer, ByVal ParametroQual As Integer, ByVal Sa_Cod As Integer) As RispostaStandard
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim prodotto As SalvaProdottoModel = JsonConvert.DeserializeObject(Of SalvaProdottoModel)(model, settingLoc)

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objAgenda As New ParametriAgenda

        Dim errori As String = String.Empty

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            If (prodotto.TipoOperazioneDB = enum_TipoOperazioneDB.Cancellazione) Then

                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                If prodotto.IsMateriaPrima Then
                    'Cancellazione della Materia Prima
                    If prodotto.CategoriaProdotto <> 0 AndAlso prodotto.Mat_Cod <> 0 AndAlso prodotto.Piva <> "" Then

                        If prodotto.CodiceProdotto <> "" Then
                            'Cancellazione dalla tabella "Materie_Prime"
                            Dim obj_MP_W As New Materie_Prime_W

                            obj_MP_W.Cancella2(prodotto.CategoriaProdotto, prodotto.Mat_Cod, prodotto.Piva, prodotto.CodiceProdotto, "", objParametri_Server)
                        End If

                        'Cancellazione dati relativi alla tab "Traduzioni"
                        'Cancellazione dalla tabella "Materie_Prime_XLingue"
                        Dim obj_MPL_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_W
                        obj_MPL_W.Cancella_Materie_Prime_XLingue(prodotto.Piva, prodotto.CategoriaProdotto, Nothing, prodotto.Mat_Cod, 0, "", "", objParametri_Server)


                        'Cancellazione dati relativi ai Parametri Qualitativi della tab "Dati Tecnici"
                        If prodotto.Cal_Cod <> 0 Then

                            'Cancellazione dalla tabella "Materie_Prime_Campionature"
                            Dim obj_MCP_W As New Materie_Prime_Campion_W
                            obj_MCP_W.Cancella(prodotto.Cal_Cod, "", 0, 0, False, "", objParametri_Server)

                            'Cancellazione dalla tabella "Materie_Prime_ParametriQualitativi"
                            Dim obj_MPQ_W As New Materie_Prime_PQ_W
                            obj_MPQ_W.Cancella(prodotto.Piva, 0, prodotto.Mat_Cod, "", 0, 0, "", objParametri_Server)
                        End If

                        'Cancellazione dati relativi ai Parametri Qualitativi della tab "Configurazione"
                        If ((prodotto.CategoriaProdotto = BENI_CONFEZ_VEGETALE OrElse
                                prodotto.CategoriaProdotto = BENI_CONFEZ_ANIMALE) AndAlso
                                ParametroQual <> 0 AndAlso prodotto.Mat_Cod <> 0) Then
                            'Cancellazione dalla tabella "OTabelle_Parametri"

                            Dim obj_OTB_W As New OTabelle_W
                            obj_OTB_W.CancellaParametro(prodotto.Piva, ParametroQual, 0,
                                                                0, prodotto.Mat_Cod, objParametri_Server)

                        End If


                        'Cancellazione dati relativi ai Parametri Qualitativi della tab "Parametri Qualitativi"(Visibile solo con i SEMILAVORATI VEGETALI)
                        If prodotto.CategoriaProdotto = SEMILAVORATI_VEGETALI Then
                            'Cancellazione dalla tabella "Parametri_Qualitativi-calibro"

                            Dim obj_MPQ_W As New Materie_Prime_PQ_W
                            obj_MPQ_W.Cancella(prodotto.Piva, 0, prodotto.Mat_Cod, "calibro", 0, 0, "", objParametri_Server)

                            'Cancellazione dalla tabella "Parametri_Qualitativi-indice"
                            obj_MPQ_W.Cancella(prodotto.Piva, 0, prodotto.Mat_Cod, "indice", 0, 0, "", objParametri_Server)

                        End If

                        'Cancellazione dati relativi agli Alias della tab "Descrizioni Alternative"
                        Dim Mat_Cod_Non_Alias As Integer = prodotto.Mat_Cod
                        Dim Mat_Cod_Alias As Integer = 0

                        If prodotto.ChkAlias = 1 Then
                            Mat_Cod_Alias = Mat_Cod_Non_Alias
                            Mat_Cod_Non_Alias = 0
                        End If

                        'Cancellazione dalla tabella "Materie_Prime_Alias"
                        Dim obj_MPA_W As New Materie_Prime_Alias_W
                        obj_MPA_W.Cancella(prodotto.Piva, Nothing, Mat_Cod_Non_Alias, Mat_Cod_Alias, "", objParametri_Server)

                    End If


                    'Cancellazione dalla tabella "Listini_PrezzixRisorse" e "Listini_Prezzi_Dettagli"
                    Dim obj_ListiniPrezzi_W As New AgronicaCoreContabDAL.Listini_Prezzi_W
                    obj_ListiniPrezzi_W.Cancella_Prodotti("", 0, prodotto.CategoriaProdotto, 0, prodotto.Mat_Cod, objParametri_Server)

                    'Cancellazione dalla tabella "CampionamentoConferito_TestataGriglia_Prodotti",
                    '"Listini_CampionamentoConferito_Prodotti","Listini_CampionamentoConferito_Prodotti_Equivalenti" e
                    '"Listini_CampionamentoConferito_Dettagli"
                    Dim obj_CampionamentoConferito_Prodotti_W As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_W
                    obj_CampionamentoConferito_Prodotti_W.Cancella_CampionamentoConferito_TestataGriglia_e_Listini_Prodotti("", prodotto.Mat_Cod, objParametri_Server)
                End If

                'Cancellazione dati relativi alla tab "Storico Prezzi"
                'Cancellazione dalla tabella "Prodotti_Costi"
                Dim obj_PC_W As New AgronicaCoreContabDAL.Prodotti_Costi_W

                'Se devo cancellare gli "Storico Prezzi" di un Prodotto Materia Prima pubblico allora cancello
                'anche tutti gli altri "Storico Prezzi" inseriti dalle altre imprese per quel Prodotto.
                If prodotto.IsMateriaPrima Then

                    If Sa_Cod = -1 AndAlso prodotto.Piva = objAgenda.Piva Then
                        obj_PC_W.Cancella("", "", prodotto.CategoriaProdotto, prodotto.Pro_Cod, prodotto.Mat_Cod, 0, 0, "", objParametri_Server)
                    Else
                        obj_PC_W.Cancella(prodotto.Piva, "", prodotto.CategoriaProdotto, prodotto.Pro_Cod, prodotto.Mat_Cod, 0, 0, "", objParametri_Server)
                    End If


                ElseIf Not prodotto.IsMateriaPrima Then

                    obj_PC_W.Cancella(prodotto.Piva, "", prodotto.CategoriaProdotto, prodotto.Pro_Cod, prodotto.Mat_Cod, 0, 0, "", objParametri_Server)

                End If


                'Cancellazione dati relativi alla tab "Dati Contabilita" e "Dati Vendita Dettaglio"
                'Cancellazione dalla tabella "Prodotti_Extra_Privata"
                Dim obj_EP_W As New Prodotti_Extra_Privata_W
                obj_EP_W.Cancella("", prodotto.Piva, prodotto.Mat_Cod, prodotto.CategoriaProdotto, prodotto.Pro_Cod, objParametri_Server)

                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                r.RispostaOK = True

            End If

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r.RispostaOK = False
            r.RispostaStringa = ex.Message
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return r
    End Function

    Private Class SalvaProdottoModel
        Public CategoriaProdotto As Integer? = Nothing
        Public CodiceProdotto As String = ""
        Public Descrizione As String = ""
        Public NuovoCodiceProdotto As String = ""

        'cat_cod
        Public Categoria_Risorsa As Integer? = Nothing

        Public Linea_Cod As Integer? = Nothing
        Public IsMateriaPrima As Boolean
        Public Mat_Cod As Integer
        Public TipoOperazioneDB As Integer
        Public Piva As String
        Public DatiTecnici As DatiTecniciModel
        Public Configurazione As ConfigurazioniModel
        Public DatiContabilita As DatiContabilitaModel
        Public StoricoPrezzi As StoricoPrezziModel
        Public Traduzioni As TraduzioniModel
        Public ParametriQualitativiCalibri As ParametriQualitativiCalibriModel
        Public ParametriQualitativiIndici As ParametriQualitativiIndiciModel
        Public Opzioni As OpzioniMateriePrimeModel
        Public ParametriQualitativiDatiTecnici As ParametriQualitativiDatiTecniciModel
        Public AltriDati As AltriDatiModel
        Public DescrizioniAlternative As DescrizioniAlternativeModel
        Public Cal_Cod As Integer? = Nothing
        Public Proprietario As Boolean? = False
        Public Pro_Cod As Integer? = Nothing
        Public Codice_Esterno As String = ""
        Public Flag_Importato As Integer? = Nothing
        Public Componi_Cod_ArticoloDaCodice_Esterno As Boolean
        Public IsModulo_FF As Boolean
        Public ChkReferenza As Integer? = Nothing
        Public ChkAlias As Integer? = Nothing


    End Class

    Private Class OpzioniMateriePrimeModel
        Public ConsentiCodiciDuplicati As Boolean
        Public CodiceMaxLenght As Integer
    End Class
    Private Class GrigliaCheckBoxModel
        Public RigheScelte As String
    End Class

    Private Class CampiParametriModel
        Public Campi As String
    End Class

    Private Class ParametriQualitativiCalibriModel : Inherits GrigliaCheckBoxModel

    End Class

    Private Class ParametriQualitativiIndiciModel : Inherits GrigliaCheckBoxModel

    End Class

    Private Class ParametriQualitativiDatiTecniciModel : Inherits CampiParametriModel

    End Class

    Private Class DatiTecniciModel
        Public SementiEMaterialiVivaisti As Integer? = Nothing
        Public SpecieVegetali As Integer? = Nothing
        Public VarietaColturale As Integer? = Nothing
        Public TipologiaVarietale As Integer? = Nothing
        Public DittaDiProvenienza As Integer? = Nothing
        Public Regolamento As Integer? = Nothing
        Public Referenza As Integer? = Nothing
        Public Flag_Biologico As Int16? = Nothing
        Public Flag_Convenzionale As Int16? = Nothing
        Public Flag_Non_Agricolo As Int16? = Nothing
        Public Flag_Ausiliare_Fabbricazione As Int16? = Nothing
        Public Descrizione_Addizionale As String
        'Grfi COd
        Public Finalita_Produttiva As Integer? = Nothing
        Public UDM_Cod As Integer? = Nothing
        Public Note As String = ""
        Public Sa_Cod As Integer? = Nothing
        Public SpecieAnimali As String = ""
        Public IndirizzoAnimale As String = ""
        Public RazzaAnimale As String = ""
        Public Elimina_Referenza_ParamQualDTec As Boolean? = False
        Public PrioritaCdG As Integer = 0
        Public Cod_TecnologiaSementi As Integer? = Nothing
        Public Germinabilita As Double? = Nothing
    End Class

    Private Class StoricoPrezziModel : Inherits GrigliaModel

    End Class

    Private Class TraduzioniModel : Inherits GrigliaModel

    End Class

    Private Class DescrizioniAlternativeModel : Inherits GrigliaModel

    End Class

    Private Class ConfigurazioniModel
        Public Flag_EscludiMagazzino As Integer? = Nothing
        Public Flag_EscludiPreparazione As Integer? = Nothing
        Public Flag_Variazione As Integer? = Nothing
        Public Flag_Udm_Cod_Extra As Integer? = Nothing
        Public Flag_Extra As Integer? = Nothing
        Public Flag_Imballaggio As Integer? = Nothing
        Public Confezione_Cod As String = ""
        Public Udm_Cod_Extra As Integer? = Nothing
        Public Qta_Extra As Decimal? = Nothing
        Public Tara As Decimal? = Nothing
        Public Flag_Contenitore_Conf As Integer? = Nothing
        Public Flag_Contenitore As Integer? = Nothing
        Public Flag_Confezione As Integer? = Nothing
        Public Contenitore As Decimal? = Nothing
        Public Contenitore_Conf As Decimal? = Nothing
        Public TipoPeso As Integer? = Nothing
        Public Peso_Set As Integer? = Nothing
        Public Otabella_Cod_Base As Integer? = Nothing
        Public Filtro_Veg_Cod As String = ""
        Public Filtro_Cul_Cod As String = ""
        Public ParamQualOriginaleDettagliBeniConf As Integer? = Nothing
        Public Flag_CompLotto As Integer? = Nothing
        Public Flag_UtilizzoCA As Integer? = Nothing
    End Class

    Private Class ParametriQualitativi
        Public Progressivo As Integer? = Nothing
        Public Tipo_Campion As String = ""
        Public Tipo_ParamQual As String = ""
        Public Tipo_Cod As Integer? = Nothing
        Public Val_Cod As String = ""
        Public ChkTara_Campionatura As Integer? = Nothing
        Public Tara_Campionatura As Integer? = Nothing
        Public Valore_Des As String = ""
    End Class

    Private Class DatiContabilitaModel
        Public Aliquota_Iva As Integer? = Nothing
        Public Aliquota_Iva_Compensazione As Integer? = Nothing
        Public Conto_Economico_Acquisto As Integer? = Nothing
        Public Conto_Economico_Vendita As Integer? = Nothing
        Public Conto_Patrimoniale_Acquisto As Integer? = Nothing
        Public Conto_Patrimoniale_Vendita As Integer? = Nothing
        Public Id_Gruppo_Merce As Integer? = Nothing
        Public TipoOperazioneDB As Integer
    End Class

    Private Class AltriDatiModel
        Public EAN As String = ""
        Public Barcode As String = ""
        Public Ingredienti As String = ""
        Public Produzione_Propria As Integer? = Nothing
        Public Immagine_NomeFile As String = ""
        Public Immagine_Estensione As String = ""
        Public Immagine As String = ""
        Public Peso_Netto As Decimal? = Nothing
        Public Peso_Sgocciolato As Decimal? = Nothing
        Public Tara As Decimal? = Nothing
        Public Peso_Egalizzato As Integer? = Nothing
        Public TipoOperazioneDB As Integer
    End Class

    Private Class MateriePrimeAliasModel
        Public Mat_Cod_Alias As Integer? = Nothing
        Public Mat_Des_Alias As String = ""
        Public Mat_Cod_Alias_Old As Integer? = Nothing
        Public Mat_Des_Alias_Old As String = ""
        Public Codice_Lingua As String = ""
        Public Filtro_Contatti_String As String = ""
        Public Filtro_Contatti_Des_String As String = ""
    End Class

    Private Class GrigliaModel
        Public RigheInserite As String
        Public RigheModificate As String
        Public RigheEliminate As String
        Public TutteLeRighe As String
    End Class

    Private Class MateriaPrimaXLInguaModel
        Public Lingua_Cod As Integer? = Nothing
        Public Mat_Des As String = ""
        Public Nome As String = ""
        Public Lingua_Cod_Letta As Integer? = Nothing
        'Public DATA_AGG As Date? = Nothing
    End Class

    'Private Class ProdottiCostiModel
    '    Public ID As Integer? = Nothing
    '    Public Udm_Cod As Integer? = Nothing
    '    Public Udm_Des As String = ""
    '    Public Prezzo_Unitario As Decimal? = Nothing
    '    Public Validita_Inizio As Date = "1900/01/01"
    '    Public Validita_Fine As Date = "2100/12/31"
    '    Public Riferimento As String = ""
    '    Public Pro_Cod As Integer? = Nothing
    '    Public Mezzo As Integer? = Nothing
    '    Public Username_Creazione As String = ""
    '    Public Data_Creazione As Date = Date.Now
    'End Class
    Private Class CalibriModel
        Public Tipo_Cod As Integer? = Nothing
        Public Cal_Des As String = ""
        Public Selected As Boolean = False
    End Class

    Private Class IndiciModel
        Public IND_MAT_COD As Integer? = Nothing
        Public IND_MAT_DES As String = ""
        Public Selected As Boolean = False
    End Class

End Class
