Imports <xmlns="http://www.agronica.it/grafica/">


Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json.Linq

Public Class GIS_Entita_R



    ''' <summary>
    ''' Legge i dati cartografici di un impresa, restituendo un xml
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="ForDelete"></param>
    ''' <param name="AllAttributes"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiPerImpresa(
              ByVal PivaSuperUser As String _
            , ByVal Piva As String _
            , ByVal ForDelete As Boolean _
            , ByVal AllAttributes As Boolean _
            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim sXmlRval As String = ""




        Return sXmlRval

    End Function


    ''' <summary>
    ''' Legge i dati cartografici di un impresa e di tutti i suoi figli nella gerarchia GIAS
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="ForDelete"></param>
    ''' <param name="AllAttributes"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Leggi(
              ByVal PivaSuperUser As String _
            , ByVal Piva As String _
            , ByVal ForDelete As Boolean _
            , ByVal AllAttributes As Boolean _
            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim sXmlRval As String = ""




        Return sXmlRval


    End Function

    Public Function LeggiGUIDDaEntita_Cod(ByVal entita_cod As Int32,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT = xRead.LeggiCompleto_Entita_cod(entita_cod.ToString, "", "", objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Entità non trovata.")
        End If

        If DT.Rows(0)("Entita_GUID") Is Nothing Then
            Return ""
        End If

        Return DT.Rows(0)("Entita_GUID").ToString

    End Function

    Public Function esisteGisEntita_cancellazioneElementoAnagrafico(Piva As String,
                                                                    Sa_Cod As Integer,
                                                                    Appezza As Integer,
                                                                    Campo_Cod As Integer,
                                                                    Id_Imp As Integer,
                                                                    TipoEntita_Cod As enum_GIS2012_TipoEntita,
                                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Boolean
        'Controllo se esiste un Entità GIS
        Dim objGIS As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim DT_GIS As DataTable

        Dim xFiltroAggiuntivo As String = If(TipoEntita_Cod = enum_GIS2012_TipoEntita.APPEZZAMENTI, "Id_Imp = 0", "")

        DT_GIS = objGIS.Leggi("", 0, TipoEntita_Cod,
                              Piva, Sa_Cod, Appezza,
                              Campo_Cod, Id_Imp,
                              AGRODATAINIZIO, AGRODATAFINE,
                              xFiltroAggiuntivo, "", objParametri_Server)

        If DT_GIS.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function



    Public Function LeggiPathAllegatoDaEntita(ByVal entita_cod As Int32,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim xRead As New AgronicaCoreGisDAL.GIS_Allegati_R

        Dim DT As DataTable = xRead.LeggiAllegatoDaEntita(entita_cod, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Throw New Exception("Nessun allegato trovato.")
        End If

        Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim basePath = LeggiCFGDatiIniziali.Leggi_Valore(0, "GestioneAllegati_Repository", "", "", objParametri_Server)

        Dim subfolderPathJSON = JObject.Parse(LeggiCFGDatiIniziali.Leggi_Valore(0, "pathFileRaster", "", "", objParametri_Server))

        Dim subfolder = subfolderPathJSON("BasePathAllegatiRaster").ToString

        basePath = System.IO.Path.Combine(basePath, subfolder)

        Dim workDir = System.IO.Path.Combine(basePath, DT.Rows(0)("Allegati_Documenti_NomeFile").ToString.Split(".")(0))

        Dim fullPath = System.IO.Path.Combine(workDir, DT.Rows(0)("Allegati_Documenti_NomeFile").ToString)

        If Not System.IO.File.Exists(fullPath) Then
            Throw New Exception("Il file allegato non esiste.")
        End If

        Return fullPath
    End Function

    Public Function LeggiParametriVisualizzazioneDaEntita_Cod(ByVal entita_cod As Int32,
                                                              ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT = xRead.LeggiParametriVisualizzazione(entita_cod, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Entità non trovata.")
        End If

        If DT.Rows(0)("ParametriVisualizzazioneLayer") Is Nothing Then
            Return ""
        End If

        Return DT.Rows(0)("ParametriVisualizzazioneLayer").ToString
    End Function

    Public Function LeggiValiditaImpiantoDaEntitaGUID(ByVal guidEntita1 As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT As DataTable = xRead.LeggiValiditaImpiantoDaEntitaGUID(guidEntita1, objParametri_Server)

        Return DT
    End Function
    
    Public Function LeggiValiditaImpiantoDaEntitaCod(ByVal entitaCod As Int32,
                                                      ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT As DataTable = xRead.LeggiValiditaImpiantoDaEntitaCod(entitaCod, objParametri_Server)

        Return DT
    End Function

    Public Function LeggiValiditaEntitaDaEntitaGUID(ByVal guidEntita1 As String,
                                                    ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT As DataTable = xRead.LeggiValiditaEntitaDaEntitaGUID(guidEntita1, objParametri_Server)

        Return DT
    End Function

    Public Function LeggiValiditaEntitaDaEntitaCod(ByVal guidEntita1 As String,
                                                    ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim DT As DataTable = xRead.LeggiValiditaEntitaDaEntitaCod(guidEntita1, objParametri_Server)

        Return DT
    End Function
End Class


Public Class GIS_Entita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Scrive una singola entita spacchettando l'xml
    ''' </summary>
    ''' <param name="xmlEntita">dati della singola entita</param>
    ''' <param name="OUTPUT_EntitaCod">codice di entità così come restituito da database</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function scrivi(
                          ByRef xmlEntita As String,
                          ByRef OUTPUT_EntitaCod As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As Boolean

        Const NomeRoutine As String = "GIS_Entita_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            'spacchetto l'xml e salvo gli elementi
            Dim DocumentoSalva As XDocument = XDocument.Parse(xmlEntita)
            Dim salvataggioCorretto As Boolean = True

            Dim ScriviEntita As New AgronicaCoreGisDAL.GIS_Entita_W
            Dim scriviEntitaGrafica As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

            Dim nodoEntita = DocumentoSalva.<Entita>
            Dim entitaGias = nodoEntita.<EntitaGIAS>.<DatoGias>.FirstOrDefault

            Dim TipoOperazioneDB As Integer = nodoEntita.@TipoOperazioneDB



            Select Case TipoOperazioneDB
                Case 1

                    Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    OUTPUT_EntitaCod = AgroSequenze.NuovoId_Tabella("GIS_Entita", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                    'OUTPUT_EntitaCod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                    '    "GIS_Entita",
                    '    objParametri
                    ')


                    Dim entitaGias_data_creazione As Date = #2/1/1900#
                    Dim entitaGias_data_Modifica As Date = #2/1/1900#
                    Dim entitaGias_username_creazione As String = ""
                    Dim entitaGias_username_modifica As String = ""


                    If Not String.IsNullOrEmpty(entitaGias.<Data_Creazione>.Value) Then
                        entitaGias_data_creazione = entitaGias.<Data_Creazione>.Value
                    End If

                    If Not String.IsNullOrEmpty(entitaGias.<Data_Modifica>.Value) Then
                        entitaGias_data_Modifica = entitaGias.<Data_Modifica>.Value
                    End If

                    If Not String.IsNullOrEmpty(entitaGias.<Username_Creazione>.Value) Then
                        entitaGias_username_creazione = entitaGias.<Username_Creazione>.Value
                    End If

                    If Not String.IsNullOrEmpty(entitaGias.<Username_Modifica>.Value) Then
                        entitaGias_username_modifica = entitaGias.<Username_Modifica>.Value
                    End If



                    ScriviEntita.Scrivi(
                        entitaGias.<PivaSuperUser>.Value,
                        OUTPUT_EntitaCod,
                        entitaGias.<TipoEntita_Cod>.Value,
                        entitaGias.<Piva>.Value,
                        entitaGias.<Sa_Cod>.Value,
                        entitaGias.<Appezza>.Value,
                        entitaGias.<Campo_Cod>.Value,
                        entitaGias.<Id_Imp>.Value,
                        entitaGias.<PROV>.Value,
                        entitaGias.<COM>.Value,
                        entitaGias.<SEZIONE>.Value,
                        entitaGias.<FOGLIO>.Value,
                        entitaGias.<NUMERO>.Value,
                        entitaGias.<SUBALTERNO>.Value,
                        entitaGias.<Programmazione_Cod>.Value,
                        entitaGias.<Programmazione_Entita_Cod>.Value,
                        entitaGias.<Id_Agenda>.Value,
                        entitaGias.<id_mov_det>.Value,
                        entitaGias.<Ricetta_Operazione_Cod>.Value,
                        entitaGias.<analisi_campione_cod>.Value,
                        entitaGias.<OLDGrafica_ID>.Value,
                        entitaGias.<inviato>.Value,
                        entitaGias.<Validita_Inizio>.Value,
                        entitaGias.<Validita_Fine>.Value,
                        objParametri,
                        entitaGias_data_creazione,
                        entitaGias_data_Modifica,
                        entitaGias_username_creazione,
                        entitaGias_username_modifica
                    )


                    Dim OUTPUT_ElementoGraficoCod As Integer

                    entitaGias.<Entita_Cod>.Value = OUTPUT_EntitaCod

                    'verificare agg.to
                    xRisp = scriviEntitaGrafica.Scrivi(
                        nodoEntita.FirstOrDefault.ToString,
                        OUTPUT_ElementoGraficoCod,
                        objParametri
                    )

                Case 2


                    Dim entitaGias_data_Modifica As Date = #2/1/1900#
                    Dim entitaGias_username_modifica As String = ""



                    If Not String.IsNullOrEmpty(entitaGias.<Data_Modifica>.Value) Then
                        entitaGias_data_Modifica = entitaGias.<Data_Modifica>.Value
                    End If


                    If Not String.IsNullOrEmpty(entitaGias.<Username_Modifica>.Value) Then
                        entitaGias_username_modifica = entitaGias.<Username_Modifica>.Value
                    End If

                    ScriviEntita.Modifica(
                        entitaGias.<PivaSuperUser>.Value,
                        entitaGias.<Entita_Cod>.Value,
                        entitaGias.<TipoEntita_Cod>.Value,
                        entitaGias.<Piva>.Value,
                        entitaGias.<Sa_Cod>.Value,
                        entitaGias.<Appezza>.Value,
                        entitaGias.<Campo_Cod>.Value,
                        entitaGias.<Id_Imp>.Value,
                        entitaGias.<PROV>.Value,
                        entitaGias.<COM>.Value,
                        entitaGias.<SEZIONE>.Value,
                        entitaGias.<FOGLIO>.Value,
                        entitaGias.<NUMERO>.Value,
                        entitaGias.<SUBALTERNO>.Value,
                        entitaGias.<Programmazione_Cod>.Value,
                        entitaGias.<Programmazione_Entita_Cod>.Value,
                        entitaGias.<Id_Agenda>.Value,
                        entitaGias.<Ricetta_Operazione_Cod>.Value,
                        entitaGias.<Validita_Inizio>.Value,
                        entitaGias.<Validita_Fine>.Value,
                        "",
                        objParametri,
                        Data_modifica:=entitaGias_data_Modifica,
                        username_modifica:=entitaGias_username_modifica
                    )

                    Dim idle As Integer
                    'verificare agg.to
                    xRisp = scriviEntitaGrafica.Scrivi(
                        nodoEntita.FirstOrDefault.ToString,
                        idle,
                        objParametri
                    )

                Case 3

                    Dim idle As Integer
                    'verificare cancellazione..
                    scriviEntitaGrafica.Scrivi(
                        nodoEntita.FirstOrDefault.ToString,
                        idle,
                        objParametri
                    )

                    ScriviEntita.Cancella(
                        entitaGias.<PivaSuperUser>.Value,
                        entitaGias.<Entita_Cod>.Value,
                        "",
                        objParametri
                    )


                Case Else

            End Select

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Entita_cod=" & OUTPUT_EntitaCod & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        Return xRisp


    End Function

    Public Function SalvaParametriVisualizzazioneEntita(ByVal entita_cod As Int32,
                                                        ByVal parametri_visualizzazione As String,
                                                        ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W

        resp = xWrite.SalvaParametriVisualizzazioneEntita(entita_cod, parametri_visualizzazione, objParametri)

        Return resp
    End Function

    Public Function EliminaDatiPrecisionFarmingFuoriImpianto(
                                ByVal PivaSuperUser As String,
                                ByVal Entita_Cod As Int32,
                                ByVal ricetta_operazione_cod As Integer,
                                ByVal ElencoLayer As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean


        Dim xEntita As New AgronicaCoreGisDAL.GIS_Entita_W
        xEntita.EliminaDatiPrecisionFarmingFuoriImpianto(PivaSuperUser, Entita_Cod, ricetta_operazione_cod, ElencoLayer, xFiltroAggiuntivo, objParametri)

        Return True

    End Function

    Public Function EliminaDatiPrecisionFarmingDatoImpianto(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "EliminaDatiPrecisionFarmingDatoImpianto"

        Dim xEntita As New AgronicaCoreGisDAL.GIS_Entita_W
        Dim xPoligono As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

        Dim messaggioErrore As String
        Dim xRisp As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try


            xPoligono.EliminaDatiPrecisionFarmingDatoImpianto(PivaSuperUser, Entita_Cod, ElencoLayer, xFiltroAggiuntivo, objParametri)
            xEntita.EliminaDatiPrecisionFarmingDatoImpianto(PivaSuperUser, Entita_Cod, ElencoLayer, xFiltroAggiuntivo, objParametri)

            xPoligono.EliminaDatiPrecisionFarmingDatoPlanning(PivaSuperUser, Entita_Cod, ElencoLayer, xFiltroAggiuntivo, objParametri)
            xEntita.EliminaDatiPrecisionFarmingDatoPlanning(PivaSuperUser, Entita_Cod, ElencoLayer, xFiltroAggiuntivo, objParametri)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = "(Entita_cod=" & Entita_Cod & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return True

    End Function


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function EliminaImpiantoPuntiScomposti(
                                                 ByVal piva As String,
                                                 ByVal sa_cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""

        Dim xRisp As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim LeggiXElimina As New AgronicaCoreGisDAL.GIS_Entita_R
            Dim EliminatoreElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
            Dim EliminatoreEntita As New AgronicaCoreGisDAL.GIS_Entita_W


            Dim dtLeggiXElimina As DataTable
            dtLeggiXElimina =
                LeggiXElimina.LeggixEliminaImpiantoPuntiScomposti(piva, sa_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)






            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            For Each dRigaLeggiXElimina As DataRow In dtLeggiXElimina.Rows

                EliminatoreElementiGrafici.Cancella(objParametri.PivaSuperUser, dRigaLeggiXElimina("ElementoGrafico_cod"), "", objParametri)

                EliminatoreEntita.Cancella(objParametri.PivaSuperUser, dRigaLeggiXElimina("Entita_cod"), "", objParametri)

            Next


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        Return xRisp

    End Function

    Public Function scriviEntitaBase(ByVal entita_cod As Int32,
                                     ByVal tipo_entita As Int32,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByVal Optional Inizio_Validita As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                     ByVal Optional Fine_Validita As Date = CostantiPersonalizzate.AGRODATAFINE,
                                     ByVal Optional allegato_cod As Int32 = 0
                                     ) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W
        Dim resp As Boolean

        resp = xWrite.Scrivi(objParametri_Utenti.PivaSuperUser,
                             entita_cod,
                             tipo_entita,
                             "", 0, 0, 0, 0, "", "", "", 0, 0,
                             "", 0, 0, 0, 0, 0, 0, "", 0,
                             Inizio_Validita, Fine_Validita,
                             objParametri_Server,
                             Now, Now,
                             objParametri_Utenti.UsernameOperazione,
                             objParametri_Utenti.UsernameOperazione,
                             allegato_cod)

        If Not resp Then
            Throw New Exception("Errore nel salvataggio dell'Entità GIS")
        End If

        Return resp
    End Function

    Public Function SalvaGUIDEntita(ByVal entita_cod As Int32,
                                    ByVal GUID_entita As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W

        resp = xWrite.SalvaGUIDEntita(entita_cod, GUID_entita, objParametri_Server)

        Return resp
    End Function

    Public Function salvaAllegatoEstrazione(ByVal zipFileFullPath As String,
                                            ByVal descrizione_esportazione As String,
                                            ByVal Inizio_Validita As Date,
                                            ByVal Fine_Validita As Date,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            Optional ByVal override_username As String = "") As Int32

        Dim newId As Int32 = 0

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Allegati_W

        Dim usernameOperazione = objParametri_Server.UsernameOperazione

        If Not override_username.Equals("") Then
            usernameOperazione = override_username
        End If

        Dim resp = xWrite.Scrivi(0,
                                 descrizione_esportazione,
                                 System.IO.Path.GetFileName(zipFileFullPath),
                                 System.IO.Path.GetExtension(zipFileFullPath),
                                 Inizio_Validita,
                                 Fine_Validita,
                                 newId,
                                 objParametri_Server,
                                 usernameOperazione,
                                 usernameOperazione)

        If Not resp Then
            Throw New Exception("Errore nel salvataggio delle informazioni dell'allegato.")
        End If

        Return newId
    End Function

    Public Function salvaAllegatoLayer(ByVal newAllegatoID As Int32,
                                       ByVal layerElementiGrafici_Cod As Int32,
                                       ByVal tipologiaLayer_Cod As Int32,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       Optional ByVal override_username As String = "") As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Allegati_W

        resp = xWrite.ScriviAllegatoLayer(newAllegatoID, layerElementiGrafici_Cod, tipologiaLayer_Cod, objParametri_Server, override_username)

        Return resp
    End Function

    ''' <summary>
    ''' Saves the entities identificators in EntitaXEsportazioni table and returns corresponding IdEsp
    ''' </summary>
    ''' <param name="elencoEntita"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function SaveExportEntities(ByVal elencoEntita As List(Of Int32),
                                       ByRef objParametri As AgronicaCoreParametri) As Int32
        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W
        Dim idEsp As Int32

        Dim sequenze As New Agro_Sequenze
        idEsp = sequenze.NuovoId_Tabella(
            "EntitaXEsportazioni",
            0,
            Int32.MaxValue,
            objParametri,
            True
            )

        xWrite.SaveExportEntities(idEsp, elencoEntita, objParametri) ' salvo le entita_cod da esportare

        Return idEsp
    End Function

    Public Function CleanCompletedExportEntitiesRecords(ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W

        Return xWrite.CleanCompletedExportEntitiesRecords(objParametri) ' pulisco i record della tabella EntitaXEsportazioni che sono già stati processati
    End Function
End Class
