Imports System.Configuration
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Impostazioni_PDC

    Public Const PivaZESPRI As String = "02125421202"
    Public Const PivaFRUTTAGEL As String = "01271980391"

    Public Property FIXED_MENU As Boolean = False
    Public Property EXTRA_CONFIG As String = ""
    Public Property DEFAULT_LABEL As Boolean = True
    Public Property DEFAULT_NOTE_CAMPIONI As String
    Public Property Codice_Campione_Progressivo_TERREMERESE As Boolean
    Public Property Campione_Progressivo_Campione_ZANI As Boolean
    Public Property Campione_Progressivo_Inizio_Anno As Boolean
    Public Property Campione_Progressivo_ZESPRI As Boolean
    Public Property Campione_progressivo_ZESPRI_Prefisso As String
    Public Property Campione_progressivo_ZESPRI_Lunghezza As Integer
    Public Property Controllo_Quaderno_ZESPRI As String
    Public Property Stampa_Rapida As Boolean
    Public Property Stampa_Etichetta As Boolean
    Public Property Stampa_Rapporto As Boolean
    Public Property Label_Codice_Campione As String
    Public Property Label_Codice_Analisi As String
    Public Property Label_Codice_Griglia As String
    Public Property Gestione_LFO As Boolean
    Public Property Gestione_Stabilimento As Boolean
    Public Property Richiesta_Unica As Boolean
    Public Property Tipologie_Analisi As String
    Public Property Visibilita_Punti_Prelievo As String
    Public Property Visibilita_Tipo_Analisi As String
    Public Property Aggiorna_Sblocchi As Integer


    Public Sub New()
        DEFAULT_NOTE_CAMPIONI = ""
        Campione_Progressivo_Inizio_Anno = False
        Campione_Progressivo_Campione_ZANI = False
        Codice_Campione_Progressivo_TERREMERESE = False
        Campione_Progressivo_ZESPRI = False
        Campione_progressivo_ZESPRI_Prefisso = ""
        Campione_progressivo_ZESPRI_Lunghezza = 0
        Controllo_Quaderno_ZESPRI = ""
        Stampa_Rapida = False
        Stampa_Etichetta = True
        Stampa_Rapporto = False
        Label_Codice_Campione = ""
        Label_Codice_Analisi = ""
        Label_Codice_Griglia = ""
        Gestione_LFO = True
        Gestione_Stabilimento = False
        Richiesta_Unica = False
        Tipologie_Analisi = "8"
        Visibilita_Punti_Prelievo = ""
        Visibilita_Tipo_Analisi = ""
        Aggiorna_Sblocchi = 0
    End Sub

    Public Sub New(ByVal objParametri_Utenti As AgronicaCoreParametri)

        DEFAULT_NOTE_CAMPIONI = ""
        Campione_Progressivo_Inizio_Anno = False
        Campione_Progressivo_Campione_ZANI = False
        Codice_Campione_Progressivo_TERREMERESE = False
        Campione_Progressivo_ZESPRI = False
        Campione_progressivo_ZESPRI_Prefisso = ""
        Campione_progressivo_ZESPRI_Lunghezza = 0
        Controllo_Quaderno_ZESPRI = ""
        Stampa_Rapida = False
        Stampa_Etichetta = True
        Stampa_Rapporto = objParametri_Utenti.PivaSuperUser = PivaFRUTTAGEL
        Label_Codice_Campione = ""
        Label_Codice_Analisi = ""
        Label_Codice_Griglia = ""
        Gestione_LFO = True
        Richiesta_Unica = False
        Tipologie_Analisi = "8"
        Visibilita_Punti_Prelievo = ""
        Visibilita_Tipo_Analisi = ""
        Aggiorna_Sblocchi = 0

        ' gestione stabilimento x apofruit
        Dim ConfApo = ConfigurationManager.AppSettings("ConfApo")
        Gestione_Stabilimento = Not IsNothing(ConfApo) AndAlso ConfApo = "true"

        ' nuove impostazioni
        If objParametri_Utenti.PivaSuperUser = PivaZESPRI Then
            Campione_Progressivo_ZESPRI = True
            Campione_progressivo_ZESPRI_Prefisso = "Z"
            Campione_progressivo_ZESPRI_Lunghezza = 4
            'Controllo_Quaderno_ZESPRI = "1"
            Gestione_LFO = False
            Richiesta_Unica = True
            Tipologie_Analisi = "8,11"
            Aggiorna_Sblocchi = 1
        End If

        Dim objDefault As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT_Impostazioni As DataTable
        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_DEFAULT_NOTE_CAMPIONI, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            DEFAULT_NOTE_CAMPIONI = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Analisi, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Label_Codice_Analisi = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Griglia, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Label_Codice_Griglia = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Progressivo_Inizio_Anno, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Campione_Progressivo_Inizio_Anno = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Zani, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Campione_Progressivo_Campione_ZANI = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Stampa_Rapida_WorkFlow, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Stampa_Rapida = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Campione, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Label_Codice_Campione = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_TERREMERSE, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Codice_Campione_Progressivo_TERREMERESE = DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1")
        End If

        '----- LEGGO NUOVE IMPOSTAZIONI PDC

        DT_Impostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SuperUser_Impostazioni_PDC, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If DT_Impostazioni.Rows.Count > 0 Then
            Dim objImpostazioniPDC As JObject = JsonConvert.DeserializeObject(DT_Impostazioni.Rows(0).Item("Impostazione_Valore_1"))
            If Not IsNothing(objImpostazioniPDC.GetValue("Campione_Progressivo_ZESPRI")) Then
                Campione_Progressivo_ZESPRI = objImpostazioniPDC.GetValue("Campione_Progressivo_ZESPRI")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Campione_progressivo_ZESPRI_Prefisso")) Then
                Campione_progressivo_ZESPRI_Prefisso = objImpostazioniPDC.GetValue("Campione_progressivo_ZESPRI_Prefisso")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Campione_progressivo_ZESPRI_Lunghezza")) Then
                Campione_progressivo_ZESPRI_Lunghezza = objImpostazioniPDC.GetValue("Campione_progressivo_ZESPRI_Lunghezza")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Controllo_Quaderno_ZESPRI")) Then
                Controllo_Quaderno_ZESPRI = objImpostazioniPDC.GetValue("Controllo_Quaderno_ZESPRI")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Stampa_Etichetta")) Then
                Stampa_Etichetta = objImpostazioniPDC.GetValue("Stampa_Etichetta")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Gestione_LFO")) Then
                Gestione_LFO = objImpostazioniPDC.GetValue("Gestione_LFO")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Richiesta_Unica")) Then
                Richiesta_Unica = objImpostazioniPDC.GetValue("Richiesta_Unica")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Tipologie_Analisi")) Then
                Tipologie_Analisi = objImpostazioniPDC.GetValue("Tipologie_Analisi")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Visibilita_Punti_Prelievo")) Then
                Visibilita_Punti_Prelievo = objImpostazioniPDC.GetValue("Visibilita_Punti_Prelievo")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Visibilita_Tipo_Analisi")) Then
                Visibilita_Tipo_Analisi = objImpostazioniPDC.GetValue("Visibilita_Tipo_Analisi")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Default_Label")) Then
                DEFAULT_LABEL = objImpostazioniPDC.GetValue("Default_Label")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Extra")) Then
                EXTRA_CONFIG = objImpostazioniPDC.GetValue("Extra")
            End If
            If Not IsNothing(objImpostazioniPDC.GetValue("Aggiorna_Sblocchi")) Then
                Aggiorna_Sblocchi = objImpostazioniPDC.GetValue("Aggiorna_Sblocchi")
            End If
        End If

        SetDefaultLabel()

    End Sub

    ' imposta le label standard se non impostate
    Public Sub SetDefaultLabel(Optional ByVal Default_Label As Boolean = False)
        If Default_Label OrElse String.IsNullOrEmpty(Label_Codice_Campione) Then
            Label_Codice_Campione = Gias.CodiceCampione
        End If
        If Default_Label OrElse String.IsNullOrEmpty(Label_Codice_Analisi) Then
            Label_Codice_Analisi = Gias.CodiceAnalisi
        End If
        If Default_Label OrElse String.IsNullOrEmpty(Label_Codice_Griglia) Then
            Label_Codice_Griglia = Gias.SchemaAnalisi
        End If
    End Sub

    Public Sub Salva(ByVal objParametri_Utenti As AgronicaCoreParametri)
        Dim objDefault As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        '----- CANCELLO

        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_DEFAULT_NOTE_CAMPIONI, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Progressivo_Inizio_Anno, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Stampa_Rapida_WorkFlow, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Campione, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Analisi, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Griglia, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Zani, "", objParametri_Utenti)
        objDefault.Cancella(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_TERREMERSE, "", objParametri_Utenti)

        '----- SCRIVO

        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_DEFAULT_NOTE_CAMPIONI, DEFAULT_NOTE_CAMPIONI, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Campione, Label_Codice_Campione, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Progressivo_Inizio_Anno, Campione_Progressivo_Inizio_Anno, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Stampa_Rapida_WorkFlow, Stampa_Rapida, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Analisi, Label_Codice_Analisi, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Label_Codice_Griglia, Label_Codice_Griglia, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_Zani, Campione_Progressivo_Campione_ZANI, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SUPERUSER_Codice_Campione_TERREMERSE, Codice_Campione_Progressivo_TERREMERESE, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

        '----- SCRIVO NUOVE IMPOSTAZIONI PDC

        Dim objImpostazioniPDC As New JObject
        objImpostazioniPDC.Add("Campione_Progressivo_ZESPRI", Campione_Progressivo_ZESPRI)
        objImpostazioniPDC.Add("Campione_progressivo_ZESPRI_Prefisso", Campione_progressivo_ZESPRI_Prefisso)
        objImpostazioniPDC.Add("Campione_progressivo_ZESPRI_Lunghezza", Campione_progressivo_ZESPRI_Lunghezza)
        objImpostazioniPDC.Add("Controllo_Quaderno_ZESPRI", Controllo_Quaderno_ZESPRI)
        objImpostazioniPDC.Add("Stampa_Etichetta", Stampa_Etichetta)
        objImpostazioniPDC.Add("Gestione_LFO", Gestione_LFO)
        objImpostazioniPDC.Add("Richiesta_Unica", Richiesta_Unica)
        objImpostazioniPDC.Add("Tipologie_Analisi", Tipologie_Analisi)
        objImpostazioniPDC.Add("Visibilita_Punti_Prelievo", Visibilita_Punti_Prelievo)
        objImpostazioniPDC.Add("Visibilita_Tipo_Analisi", Visibilita_Tipo_Analisi)
        objImpostazioniPDC.Add("Aggiorna_Sblocchi", Aggiorna_Sblocchi)
        objImpostazioniPDC.Add("Default_Label", DEFAULT_LABEL)
        objImpostazioniPDC.Add("Extra", EXTRA_CONFIG)
        Dim Impostazioni_PDC As String = JsonConvert.SerializeObject(objImpostazioniPDC)
        objDefault.Cancella(enum_Impostazioni_Utenti.SuperUser_Impostazioni_PDC, "", objParametri_Utenti)
        objDefault.Scrivi(enum_Impostazioni_Utenti.SuperUser_Impostazioni_PDC, Impostazioni_PDC, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

    End Sub

End Class
