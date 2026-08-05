Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Linq

Public Class RisorseUmane_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Decodifica(Piva As String, cod_RisUm As Integer, objParametri_Server As AgronicaCoreParametri, verbose As Boolean)

        Dim risorsa_umana As New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(cod_RisUm)
        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim dt = objRisorse_Umane.Leggi4(Piva, "", cod_RisUm, 0, "", False, False, False, False, False, False, False, "", "", objParametri_Server)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim row As DataRow = dt.Rows(0)

        risorsa_umana.rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(CInt(row("Cod_Rapporto")))

        If row("Cliente") = 1 Then
            risorsa_umana.rapportoContabile.cliente = True
        End If

        If row("Fornitore") = 1 Then
            risorsa_umana.rapportoContabile.fornitore = True
        End If

        If row("Dipendente") = 1 Then
            risorsa_umana.rapportoContabile.dipendente = True
        End If

        If row("Terzista") = 1 Then
            risorsa_umana.rapportoContabile.terzista = True
        End If

        If row("Legale") = 1 Then
            risorsa_umana.rapportoContabile.legale = True
        End If

        If row("Agente") = 1 Then
            risorsa_umana.rapportoContabile.agente = True
        End If

        If row("Consulente") = 1 Then
            risorsa_umana.rapportoContabile.consulente = True
        End If

        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dtContatti As DataTable = objContattiR.Contatti_Contatto_Leggi(CStr(row("Piva")),
                                                    CStr(row("Cod_Contatto")),
                                                    cod_RisUm,
                                                    CInt(row("Cod_Rapporto")),
                                                    True,
                                                    False,
                                                    0,
                                                    0,
                                                    False,
                                                    0,
                                                    ID_CF_NOFILTRO,
                                                    0,
                                                    "", True, 0, 0, 0, 0, 0,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "", "", objParametri_Server)

        risorsa_umana.contatto = (From rowContatto As DataRow In dtContatti.Rows
                                  Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                            .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(CStr(rowContatto("Piva")), CStr(rowContatto("Cod_Contatto"))),
                                            .fisico_Giuridico = rowContatto("Id_CF")
                              }).FirstOrDefault

        'If verbose Then
        risorsa_umana.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(row("Validita_Inizio")), CDate(row("Validita_Fine")))
            risorsa_umana.settore = CStr(row("Settore_Des"))
            risorsa_umana.attivita = CStr(row("Attivita_Des"))
            risorsa_umana.rapportoContabile.descrizione = CStr(row("Rapporto_Des"))

            risorsa_umana.contatto = (From rowContatto As DataRow In dtContatti.Rows
                                      Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                        .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(CStr(rowContatto("Piva")), CStr(rowContatto("Cod_Contatto"))),
                                        .nome = CStr(rowContatto("Nome")),
                                        .cognome = CStr(rowContatto("Cognome")),
                                        .codiceFiscale = CStr(rowContatto("Codice_Fiscale")),
                                        .ragione_Sociale = IIf(CStr(rowContatto("Rag_Soc")) = "", CStr(rowContatto("Cognome")) & " " & CStr(rowContatto("Nome")), CStr(rowContatto("Rag_Soc"))),
                                        .risorseUmane = New List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)({New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(CInt(rowContatto("Cod_RisUm"))) With {
                                            .attivita = CStr(rowContatto("Attivita_Des")),
                                            .settore = CStr(rowContatto("Settore_Des")),
                                            .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(rowContatto("Validita_Inizio")), CDate(rowContatto("Validita_Fine"))),
                                            .rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(CInt(rowContatto("Cod_Rapporto"))) With {
                                            .descrizione = CStr(rowContatto("Rapporto_Des"))}
                                        }}),
                                        .fisico_Giuridico = rowContatto("Id_CF")
                                        }).FirstOrDefault
        'End If

        Return risorsa_umana

    End Function

    Public Function Leggi(codice As Integer,
                          Leggi_Contatto As Boolean,
                          ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane
        Dim risorsa_umana = New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(codice)

        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

        Dim dt = objRisorse_Umane.Leggi("", "", codice, 0, 0, "", True, True, "", "", objParametri_Server)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim row = dt.Rows(0)

        risorsa_umana.rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(CInt(row("Cod_Rapporto"))) With {.descrizione = CStr(row("Rapporto_Des"))}
        risorsa_umana.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(row("Validita_Inizio")), CDate(row("Validita_Fine")))

        risorsa_umana.settore = CStr(row("Settore_Des"))
        risorsa_umana.attivita = CStr(row("Attivita_Des"))

        If row("Cliente") = 1 Then
            risorsa_umana.rapportoContabile.cliente = True
        End If

        If row("Fornitore") = 1 Then
            risorsa_umana.rapportoContabile.fornitore = True
        End If

        If row("Dipendente") = 1 Then
            risorsa_umana.rapportoContabile.dipendente = True
        End If

        If row("Terzista") = 1 Then
            risorsa_umana.rapportoContabile.terzista = True
        End If

        If row("Legale") = 1 Then
            risorsa_umana.rapportoContabile.legale = True
        End If

        If row("Agente") = 1 Then
            risorsa_umana.rapportoContabile.agente = True
        End If

        If row("Consulente") = 1 Then
            risorsa_umana.rapportoContabile.consulente = True
        End If

        Return risorsa_umana
    End Function

    Public Function Leggi_Tecnici(Piva As String, Cod_Contatto As String, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)
        Dim TecniciList As New List(Of AgronicaCoreModelsSTD.anagrafiche.Contatto)

        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dt As DataTable = objContattiR.Contatti_Contatto_Leggi(CStr(Piva),
                                                  Cod_Contatto,
                                                  0,
                                                  -6,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "", objParametri_Server)

        TecniciList = (From row As DataRow In dt.Rows
                       Select New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                            .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(CStr(row("Piva")), CStr(row("Cod_Contatto"))),
                            .nome = CStr(row("Nome")),
                            .cognome = CStr(row("Cognome")),
                            .codiceFiscale = CStr(row("Codice_Fiscale")),
                            .ragione_Sociale = IIf(CStr(row("Rag_Soc")) = "", CStr(row("Cognome")) & " " & CStr(row("Nome")), CStr(row("Rag_Soc"))),
                            .risorseUmane = New List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)({New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(CInt(row("Cod_RisUm"))) With {
                                .attivita = CStr(row("Attivita_Des")),
                                .settore = CStr(row("Settore_Des")),
                                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(row("Validita_Inizio")), CDate(row("Validita_Fine"))),
                                .rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(CInt(row("Cod_Rapporto"))) With {
                                .descrizione = CStr(row("Rapporto_Des"))}
                            }})
                          }).ToList

        Return TecniciList
    End Function

    Public Function Leggi_RisorseUmane_ByCod_Rapporto(Piva As String, Cod_Rapporto As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)
        Dim ru_list As New List(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)

        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dt As DataTable = objContattiR.Leggi_Contatti_ByCod_Rapporto(True, Piva, "", Cod_Rapporto, "", "", objParametri_Server)

        ru_list = (From row As DataRow In dt.Rows
                   Select New AgronicaCoreModelsSTD.anagrafiche.RisorseUmane(CInt(row("Cod_RisUm"))) With {
                          .attivita = CStr(row("Attivita_Des")),
                          .settore = CStr(row("Settore_Des")),
                          .rapportoContabile = New AgronicaCoreModelsSTD.anagrafiche.RapportoContabile(CInt(row("Cod_Rapporto"))) With {
                                .descrizione = CStr(row("Rapporto_Des"))
                          },
                          .contatto = New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(CStr(row("Piva")), CStr(row("Cod_Contatto"))),
                                .nome = CStr(row("Nome")),
                                .cognome = CStr(row("Cognome")),
                                .ragione_Sociale = IIf(CStr(row("Rag_Soc")) = "", CStr(row("Cognome")) & " " & CStr(row("Nome")), CStr(row("Rag_Soc")))
                          },
                          .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(CDate(row("Validita_Inizio")), CDate(row("Validita_Fine")))
                          }).ToList

        Return ru_list
    End Function

End Class

Public Class RisorseUmane_W
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Property Provider As Globalization.CultureInfo
    Public Property Format As String
    Public Property ValiditaInizio As Date
    Public Property ValiditaFine As Date

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

    Public Function Aggiorna_RisorseUmane(piva As String, ByRef righeInseriteArray As JArray, ByRef righeModificateArray As JArray, ByRef righeCancellateArray As JArray, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.RisorseUmane_W.Aggiorna_RisorseUmane()"

        Try

            Dim ru_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

            Dim curRisorse_Umane As New Risorse_Umane

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)
            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeModificateArray)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray

                curRisorse_Umane = New Risorse_Umane

                curRisorse_Umane.Piva = If(Not String.IsNullOrEmpty(obj("Piva")), CStr(obj("Piva")), piva)
                curRisorse_Umane.Sa_Cod = If(Not String.IsNullOrEmpty(obj("Sa_Cod")), CInt(obj("Sa_Cod")), 0)
                'curRisorse_Umane.Cod_RisUm = If(Not String.IsNullOrEmpty(obj("Cod_RisUm")), CInt(obj("Cod_RisUm")), 0)
                curRisorse_Umane.Cod_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Contatto")), CStr(obj("Cod_Contatto")), "")
                curRisorse_Umane.Cod_Rapporto = If(Not String.IsNullOrEmpty(obj("Cod_Rapporto")), CInt(obj("Cod_Rapporto")), 0)
                curRisorse_Umane.Settore_Des = If(Not String.IsNullOrEmpty(obj("Settore_Des")), CStr(obj("Settore_Des")), "")
                curRisorse_Umane.Attivita_Des = If(Not String.IsNullOrEmpty(obj("Attivita_Des")), CStr(obj("Attivita_Des")), "")
                curRisorse_Umane.Corrispettivo_Mensile = If(Not String.IsNullOrEmpty(obj("Corrispettivo_Mensile")), CDbl(obj("Corrispettivo_Mensile")), 0)
                curRisorse_Umane.Corrispettivo_Orario = If(Not String.IsNullOrEmpty(obj("Corrispettivo_Orario")), CDbl(obj("Corrispettivo_Orario")), 0)
                curRisorse_Umane.Occasionale = If(Not String.IsNullOrEmpty(obj("Occasionale")), CInt(obj("Occasionale")), 0)
                curRisorse_Umane.Ore_Settimanali = If(Not String.IsNullOrEmpty(obj("Ore_Settimanali")), CDbl(obj("Ore_Settimanali")), 0)
                curRisorse_Umane.Giorni_Ferie = If(Not String.IsNullOrEmpty(obj("Giorni_Ferie")), CInt(obj("Giorni_Ferie")), 0)
                curRisorse_Umane.Ferie_Godute = If(Not String.IsNullOrEmpty(obj("Ferie_Godute")), CInt(obj("Ferie_Godute")), 0)
                curRisorse_Umane.Giorni_Malattia = If(Not String.IsNullOrEmpty(obj("Giorni_Malattia")), CInt(obj("Giorni_Malattia")), 0)
                curRisorse_Umane.Patentino = "" 'If(Not String.IsNullOrEmpty(obj("Patentino")), CStr(obj("Patentino")), "")
                curRisorse_Umane.Data_Rilascio_Patentino = AGRODATAINIZIO 'If(Not String.IsNullOrEmpty(Trim(obj("Data_Rilascio_Patentino"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Rilascio_Patentino")), AGRODATAINIZIO)
                curRisorse_Umane.Data_Scadenza_Patentino = AGRODATAFINE 'If(Not String.IsNullOrEmpty(Trim(obj("Data_Scadenza_Patentino"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Scadenza_Patentino")), AGRODATAFINE)
                curRisorse_Umane.Cod_RisUm_Origine = If(Not String.IsNullOrEmpty(obj("Cod_RisUm_Origine")), CInt(obj("Cod_RisUm_Origine")), 0)
                curRisorse_Umane.Piva_SuperUser_Origine = If(Not String.IsNullOrEmpty(obj("Piva_SuperUser_Origine")), CStr(obj("Piva_SuperUser_Origine")), "")
                curRisorse_Umane.Ente_di_rilascio = "" 'If(Not String.IsNullOrEmpty(obj("Ente_di_rilascio")), CStr(obj("Ente_di_rilascio")), "")
                curRisorse_Umane.Saldo_Iniziale_Crediti = If(Not String.IsNullOrEmpty(obj("Saldo_Iniziale_Crediti")), CDbl(obj("Saldo_Iniziale_Crediti")), 0)
                curRisorse_Umane.Saldo_Iniziale_Debiti = If(Not String.IsNullOrEmpty(obj("Saldo_Iniziale_Debiti")), CDbl(obj("Saldo_Iniziale_Debiti")), 0)
                curRisorse_Umane.ChkSpesometro = If(Not String.IsNullOrEmpty(obj("ChkSpesometro")), CInt(obj("ChkSpesometro")), 0)
                curRisorse_Umane.ChkBlocco = If(Not String.IsNullOrEmpty(obj("ChkBlocco")), CInt(obj("ChkBlocco")), 0)
                curRisorse_Umane.Blocco_Des = If(Not String.IsNullOrEmpty(obj("Blocco_Des")), CStr(obj("Blocco_Des")), "")
                curRisorse_Umane.Classificazione_Cod = If(Not String.IsNullOrEmpty(obj("Classificazione_Cod")), CInt(obj("Classificazione_Cod")), 0)
                curRisorse_Umane.Qualifica_Cod = If(Not String.IsNullOrEmpty(obj("Qualifica_Cod")), CInt(obj("Qualifica_Cod")), 0)
                curRisorse_Umane.Mansione_Cod = If(Not String.IsNullOrEmpty(obj("Mansione_Cod")), CInt(obj("Mansione_Cod")), 0)
                curRisorse_Umane.Info_Famiglia = If(Not String.IsNullOrEmpty(obj("Info_Famiglia")), CStr(obj("Info_Famiglia")), "")
                curRisorse_Umane.codRuolo = If(Not String.IsNullOrEmpty(obj("codRuolo")), CStr(obj("codRuolo")), curRisorse_Umane.codRuolo)
                curRisorse_Umane.ruoloDescr = If(Not String.IsNullOrEmpty(obj("ruoloDescr")), CStr(obj("ruoloDescr")), curRisorse_Umane.ruoloDescr)
                curRisorse_Umane.dtVariazioneRuolo = If(Not String.IsNullOrEmpty(obj("dtVariazioneRuolo")), Date.ParseExact(obj("dtVariazioneRuolo"), Format, Provider), curRisorse_Umane.dtVariazioneRuolo)
                curRisorse_Umane.fonte = If(Not String.IsNullOrEmpty(obj("fonte")), CStr(obj("fonte")), curRisorse_Umane.fonte)
                curRisorse_Umane.fonteDescr = If(Not String.IsNullOrEmpty(obj("fonteDescr")), CStr(obj("fonteDescr")), curRisorse_Umane.fonteDescr)
                curRisorse_Umane.Ra_Cod = If(Not String.IsNullOrEmpty(obj("Ra_Cod")), CStr(obj("Ra_Cod")), curRisorse_Umane.Ra_Cod)

                curRisorse_Umane.Data_Creazione = DateTime.Now
                curRisorse_Umane.Username_Creazione = objParametri_Server.UsernameOperazione
                curRisorse_Umane.Data_Modifica = DateTime.Now
                curRisorse_Umane.Username_Modifica = objParametri_Server.UsernameOperazione
                curRisorse_Umane.Inviato = 0
                curRisorse_Umane.Validita_Inizio = If(Not String.IsNullOrEmpty(obj("Validita_Inizio")), CDate(obj("Validita_Inizio")), AGRODATAINIZIO)
                curRisorse_Umane.Validita_Fine = If(Not String.IsNullOrEmpty(obj("Validita_Fine")), CDate(obj("Validita_Fine")), AGRODATAFINE)

                EFArrayToInsert.Add(curRisorse_Umane)
            Next
            For Each obj As JObject In righeModificateArray
                curRisorse_Umane = ru_R.Leggi_RisorseUmane(obj("Cod_RisUm"), objParametri_Server)
                If curRisorse_Umane Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    'curRisorse_Umane.Piva = If(Not String.IsNullOrEmpty(obj("Piva")), CStr(obj("Piva")), curRisorse_Umane.Piva)
                    'curRisorse_Umane.Sa_Cod = If(Not String.IsNullOrEmpty(obj("Sa_Cod")), CInt(obj("Sa_Cod")), curRisorse_Umane.Sa_Cod)
                    'curRisorse_Umane.Cod_RisUm = If(Not String.IsNullOrEmpty(obj("Cod_RisUm")), CInt(obj("Cod_RisUm")), curRisorse_Umane.Cod_RisUm)
                    'curRisorse_Umane.Cod_Contatto = If(Not String.IsNullOrEmpty(obj("Cod_Contatto")), CStr(obj("Cod_Contatto")), curRisorse_Umane.Cod_Contatto)
                    curRisorse_Umane.Cod_Rapporto = If(Not String.IsNullOrEmpty(obj("Cod_Rapporto")), CInt(obj("Cod_Rapporto")), curRisorse_Umane.Cod_Rapporto)
                    curRisorse_Umane.Settore_Des = If(Not String.IsNullOrEmpty(obj("Settore_Des")), CStr(obj("Settore_Des")), curRisorse_Umane.Settore_Des)
                    curRisorse_Umane.Attivita_Des = If(Not String.IsNullOrEmpty(obj("Attivita_Des")), CStr(obj("Attivita_Des")), curRisorse_Umane.Attivita_Des)
                    curRisorse_Umane.Corrispettivo_Mensile = If(Not String.IsNullOrEmpty(obj("Corrispettivo_Mensile")), CDbl(obj("Corrispettivo_Mensile")), curRisorse_Umane.Corrispettivo_Mensile)
                    curRisorse_Umane.Corrispettivo_Orario = If(Not String.IsNullOrEmpty(obj("Corrispettivo_Orario")), CDbl(obj("Corrispettivo_Orario")), curRisorse_Umane.Corrispettivo_Orario)
                    curRisorse_Umane.Occasionale = If(Not String.IsNullOrEmpty(obj("Occasionale")), CInt(obj("Occasionale")), curRisorse_Umane.Occasionale)
                    curRisorse_Umane.Ore_Settimanali = If(Not String.IsNullOrEmpty(obj("Ore_Settimanali")), CDbl(obj("Ore_Settimanali")), curRisorse_Umane.Ore_Settimanali)
                    curRisorse_Umane.Giorni_Ferie = If(Not String.IsNullOrEmpty(obj("Giorni_Ferie")), CInt(obj("Giorni_Ferie")), curRisorse_Umane.Giorni_Ferie)
                    curRisorse_Umane.Ferie_Godute = If(Not String.IsNullOrEmpty(obj("Ferie_Godute")), CInt(obj("Ferie_Godute")), curRisorse_Umane.Ferie_Godute)
                    curRisorse_Umane.Giorni_Malattia = If(Not String.IsNullOrEmpty(obj("Giorni_Malattia")), CInt(obj("Giorni_Malattia")), curRisorse_Umane.Giorni_Malattia)
                    curRisorse_Umane.Patentino = "" 'If(Not String.IsNullOrEmpty(obj("Patentino")), CStr(obj("Patentino")), curRisorse_Umane.Patentino)
                    curRisorse_Umane.Data_Rilascio_Patentino = AGRODATAINIZIO 'If(Not String.IsNullOrEmpty(Trim(obj("Data_Rilascio_Patentino"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Rilascio_Patentino")), curRisorse_Umane.Data_Rilascio_Patentino)
                    curRisorse_Umane.Data_Scadenza_Patentino = AGRODATAFINE 'If(Not String.IsNullOrEmpty(Trim(obj("Data_Scadenza_Patentino"))), AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Scadenza_Patentino")), curRisorse_Umane.Data_Scadenza_Patentino)
                    curRisorse_Umane.Cod_RisUm_Origine = If(Not String.IsNullOrEmpty(obj("Cod_RisUm_Origine")), CInt(obj("Cod_RisUm_Origine")), curRisorse_Umane.Cod_RisUm_Origine)
                    curRisorse_Umane.Piva_SuperUser_Origine = If(Not String.IsNullOrEmpty(obj("Piva_SuperUser_Origine")), CStr(obj("Piva_SuperUser_Origine")), curRisorse_Umane.Piva_SuperUser_Origine)
                    curRisorse_Umane.Ente_di_rilascio = "" 'If(Not String.IsNullOrEmpty(obj("Ente_di_rilascio")), CStr(obj("Ente_di_rilascio")), curRisorse_Umane.Ente_di_rilascio)
                    curRisorse_Umane.Saldo_Iniziale_Crediti = If(Not String.IsNullOrEmpty(obj("Saldo_Iniziale_Crediti")), CDbl(obj("Saldo_Iniziale_Crediti")), curRisorse_Umane.Saldo_Iniziale_Crediti)
                    curRisorse_Umane.Saldo_Iniziale_Debiti = If(Not String.IsNullOrEmpty(obj("Saldo_Iniziale_Debiti")), CDbl(obj("Saldo_Iniziale_Debiti")), curRisorse_Umane.Saldo_Iniziale_Debiti)
                    curRisorse_Umane.ChkSpesometro = If(Not String.IsNullOrEmpty(obj("ChkSpesometro")), CInt(obj("ChkSpesometro")), curRisorse_Umane.ChkSpesometro)
                    curRisorse_Umane.ChkBlocco = If(Not String.IsNullOrEmpty(obj("ChkBlocco")), CInt(obj("ChkBlocco")), curRisorse_Umane.ChkBlocco)
                    curRisorse_Umane.Blocco_Des = If(Not String.IsNullOrEmpty(obj("Blocco_Des")), CStr(obj("Blocco_Des")), curRisorse_Umane.Blocco_Des)
                    curRisorse_Umane.Classificazione_Cod = If(Not String.IsNullOrEmpty(obj("Classificazione_Cod")), CInt(obj("Classificazione_Cod")), curRisorse_Umane.Classificazione_Cod)
                    curRisorse_Umane.Qualifica_Cod = If(Not String.IsNullOrEmpty(obj("Qualifica_Cod")), CInt(obj("Qualifica_Cod")), curRisorse_Umane.Qualifica_Cod)
                    curRisorse_Umane.Mansione_Cod = If(Not String.IsNullOrEmpty(obj("Mansione_Cod")), CInt(obj("Mansione_Cod")), curRisorse_Umane.Mansione_Cod)
                    curRisorse_Umane.Info_Famiglia = If(Not String.IsNullOrEmpty(obj("Info_Famiglia")), CStr(obj("Info_Famiglia")), curRisorse_Umane.Info_Famiglia)
                    curRisorse_Umane.codRuolo = If(Not String.IsNullOrEmpty(obj("codRuolo")), CStr(obj("codRuolo")), curRisorse_Umane.codRuolo)
                    curRisorse_Umane.ruoloDescr = If(Not String.IsNullOrEmpty(obj("ruoloDescr")), CStr(obj("ruoloDescr")), curRisorse_Umane.ruoloDescr)
                    curRisorse_Umane.dtVariazioneRuolo = If(Not String.IsNullOrEmpty(obj("dtVariazioneRuolo")), Date.ParseExact(obj("dtVariazioneRuolo"), Format, Provider), curRisorse_Umane.dtVariazioneRuolo)
                    curRisorse_Umane.fonte = If(Not String.IsNullOrEmpty(obj("fonte")), CStr(obj("fonte")), curRisorse_Umane.fonte)
                    curRisorse_Umane.fonteDescr = If(Not String.IsNullOrEmpty(obj("fonteDescr")), CStr(obj("fonteDescr")), curRisorse_Umane.fonteDescr)
                    curRisorse_Umane.Ra_Cod = If(Not String.IsNullOrEmpty(obj("Ra_Cod")), CStr(obj("Ra_Cod")), curRisorse_Umane.Ra_Cod)

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curRisorse_Umane.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curRisorse_Umane.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If

                    curRisorse_Umane.Data_Modifica = Date.Now
                    curRisorse_Umane.Username_Modifica = objParametri_Server.UsernameOperazione
                    EFArrayToUpdate.Add(curRisorse_Umane)

                End If
            Next

            For Each obj As JObject In righeCancellateArray
                curRisorse_Umane = New Risorse_Umane With {
                    .Cod_RisUm = CInt(obj("Cod_RisUm"))
                }
                EFArrayToDelete.Add(curRisorse_Umane)
            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim ru_W As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W

                MessaggioErrore = ru_W.Aggiorna_RisorseUmane(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri_Server
                 )

                For i As Integer = 0 To righeInseriteArray.Count - 1
                    If Not IsNothing(righeInseriteArray(i).Item("Cod_RisUm")) Then
                        righeInseriteArray(i).Item("Cod_RisUm") = CType(EFArrayToInsert(i), Risorse_Umane).Cod_RisUm
                    Else
                        CType(righeInseriteArray(i), JObject).Add("Cod_RisUm", CType(EFArrayToInsert(i), Risorse_Umane).Cod_RisUm)
                    End If

                Next

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore

    End Function

End Class
