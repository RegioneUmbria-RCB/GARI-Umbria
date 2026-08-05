Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RedirectGestione

    #Region "AgronicaDomandaIrrigua"
    Public Shared Function IndirizzoCompleto_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByRef ParametriDomandaIrrigua As ParametriDomandaIrrigua,
        Optional ByVal IDSezione As Integer = -1
    )
        Dim Lingua As String = ""
        ParametriDomandaIrrigua.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByRef ParametriDomandaIrrigua As ParametriDomandaIrrigua
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(SitoOrigine, ParametriDomandaIrrigua)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "DomandaIrrigua")
        Return script
    End Function
    #End Region

    #Region "AgronicaAuditSicurezzaGlobalCoop"
    Public Shared Function ApriPopUp_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditPuaTipo,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer,
        ByVal LinkAgronicaAgenda2010 As String
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(SitoOrigine, Tipo_Audit, username_codfisc, piva, programmazione_cod, LinkAgronicaAgenda2010)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "Pua")
        Return script
    End Function

    Public Shared Function IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditPuaTipo,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer,
        ByVal linkagronicaagenda2010 As String
    ) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA = New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010 = linkagronicaagenda2010
        ParametriAgronicaAuditPUA.Salva()

        Dim SitoDestinazione As Enum_SiteRedirector
        Select Case Tipo_Audit
            Case enum_AuditPuaTipo.Audit_Condizionalita
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case enum_AuditPuaTipo.Audit_COOP
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SicurezzaLavoro
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case enum_AuditPuaTipo.Audit_GlobalGap
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaGlobalGAP
            Case enum_AuditPuaTipo.Audit_SchedaTecnicaTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SchedaControlliTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_PannelloDiControllo
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case Else
                Throw New Exception("enum_AuditPuaTipo non trovato")
        End Select

        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    'a diff della precedente :
    '1. parametro tipo_operazione
    '2. utilizzato enum_AuditTipi
    Public Shared Function IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditTipi,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer,
        ByVal tipo_operazione As Integer,
        ByVal Regolamento_Cod As Integer,
        ByVal Audit_Cod As Integer,
        ByVal linkagronicaagenda2010 As String
    ) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA = New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.Tipo_Operazione = tipo_operazione
        ParametriAgronicaAuditPUA.sito_origine = SitoOrigine
        ParametriAgronicaAuditPUA.Regolamento_Cod = Regolamento_Cod
        ParametriAgronicaAuditPUA.Audit_Cod = Audit_Cod
        ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010 = linkagronicaagenda2010

        ParametriAgronicaAuditPUA.Salva()

        Dim SitoDestinazione As Enum_SiteRedirector
        Select Case Tipo_Audit
            Case enum_AuditTipi.AuditTipi_Condizionalita
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case enum_AuditTipi.AuditTipi_CheckListCOOP
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditTipi.AuditTipi_SicurezzaLavoro
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case enum_AuditTipi.AuditTipi_GlobalGap
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaGlobalGAP
            Case enum_AuditTipi.AuditTipi_SchedaTecnicaTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditTipi.AuditTipi_SchedaControlliTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditTipi.AuditTipi_PraticheEcologicheAPOT
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case enum_AuditTipi.AuditTipi_Formazione
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case Else
                Throw New Exception("enum_AuditTipi non trovato")
        End Select

        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    #End Region

    #Region "AgronicaPUA"
    Public Shared Function ApriPopUp_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditPuaTipo,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaPUA_PassandoDirettamente_Parametri(SitoOrigine, Tipo_Audit, username_codfisc, piva, programmazione_cod)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "Pua")
        Return script
    End Function

    Public Shared Function ApriPopUp_Sito_AgronicaPUA_PassandoDirettamente_ParametriBS(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditPuaTipo,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaPUA_PassandoDirettamente_Parametri(SitoOrigine, Tipo_Audit, username_codfisc, piva, programmazione_cod)
        Dim script As String = PreparaScripPerPopupBS(IndirizzoSito_ConQueryString, "Pua")
        Return script
    End Function

    Public Shared Function IndirizzoCompleto_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal Tipo_Audit As enum_AuditPuaTipo,
        ByVal username_codfisc As String,
        ByVal piva As String,
        ByVal programmazione_cod As Integer
    ) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA = New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.sito_origine = SitoOrigine
        ParametriAgronicaAuditPUA.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPUA
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function
    #End Region

    #Region "AgronicaBio"
    Public Shared Function ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal PaginaSitoRichiesta As enum_CodificaPagBio,
        ByVal username_codfisc As String,
        ByVal piva As String
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(SitoOrigine, PaginaSitoRichiesta, username_codfisc, piva)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "AgronicaBio")
        Return script
    End Function

    Public Shared Function ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_ParametriBS(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal PaginaSitoRichiesta As enum_CodificaPagBio,
        ByVal username_codfisc As String,
        ByVal piva As String
    ) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(SitoOrigine, PaginaSitoRichiesta, username_codfisc, piva)
        Dim script As String = PreparaScripPerPopupBS(IndirizzoSito_ConQueryString, "AgronicaBio")
        Return script
    End Function


    Public Shared Function IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByVal PaginaSitoRichiesta As enum_CodificaPagBio,
        ByVal username_codfisc As String,
        ByVal piva As String
    ) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaBio = New ParametriAgronicaBio
        ParametriAgronicaBio.Piva = piva
        ParametriAgronicaBio.Pagina_Richiesta = PaginaSitoRichiesta
        ParametriAgronicaBio.username_codfisc = username_codfisc
        ParametriAgronicaBio.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaBio
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function
    #End Region

    #Region "AgronicaSementi"
    Public Shared Function IndirizzoCompleto_Sito_AgronicaSementi_PassandoDirettamente_ParametriSementieri(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriSementieri As ParametriSementieri)
        Dim Lingua As String = ""
        ParametriSementieri.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaSementi
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function
    #End Region

    #Region "AgronicaPlanning"
    Public Shared Function IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriPlanning As ParametriPlanning,
                                 Optional ByVal IDSezione As Integer = -1)
        Dim Lingua As String = ""
        ParametriPlanning.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPlanning
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function


    Public Shared Function ApriPopUp_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                       ByVal SitoOrigine As Enum_SiteRedirector,
                                       ByRef ParametriPlanning As ParametriPlanning) As String
        'Dim Lingua As String = ""
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(SitoOrigine, ParametriPlanning)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "Planning")
        Return script
    End Function
    #End Region

    #Region "AgronicaLabCQ"
    Public Shared Function IndirizzoCompleto_Sito_AgronicaLabCQ_PassandoDirettamente_ParametriLabCQ(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriLabCQ As ParametriLabCQ)
        Dim Lingua As String = ""
        ParametriLabCQ.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaLabQualita
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function


    Public Shared Function ApriPopUp_Sito_AgronicaLabCQ_PassandoDirettamente_ParametriLabCQ(
                                       ByVal SitoOrigine As Enum_SiteRedirector,
                                       ByRef ParametriLabCQ As ParametriLabCQ) As String
        'Dim Lingua As String = ""
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_Sito_AgronicaLabCQ_PassandoDirettamente_ParametriLabCQ(SitoOrigine, ParametriLabCQ)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "LabCQ")
        Return script
    End Function
    #End Region

    #Region "PianoConcimazione"
    '@deprecated
    <Obsolete("Deprecata, usa quella con i ParametriConcimazione_2008: 'IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2008' ")>
    Public Shared Function IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriAnalisiCosti_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAnalisiCosti_2010 As ParametriAnalisiCosti_2010)
        Dim Lingua As String = ""
        ParametriAnalisiCosti_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_PianoConcimazione
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_Sito_AgronicaAudit_PassandoDirettamente_Parametri(
                              ByVal SitoOrigine As Enum_SiteRedirector,
                              ByVal Tipo_Audit As enum_AuditPuaTipo,
                              ByVal username_codfisc As String,
                              ByVal piva As String,
                              ByVal programmazione_cod As Integer,
                              Optional ByVal IDSezione As Integer = -1) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAudit
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                              ByVal SitoOrigine As Enum_SiteRedirector,
                                ByVal Tipo_Audit As enum_AuditPuaTipo,
                               ByVal username_codfisc As String,
                               ByVal piva As String,
                               ByVal programmazione_cod As Integer) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.Salva()

        Dim SitoDestinazione As Enum_SiteRedirector
        Select Case Tipo_Audit
            Case enum_AuditPuaTipo.Audit_Condizionalita
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case enum_AuditPuaTipo.Audit_COOP
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SicurezzaLavoro
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case enum_AuditPuaTipo.Audit_GlobalGap
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaGlobalGAP
            Case enum_AuditPuaTipo.Audit_SchedaTecnicaTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SchedaControlliTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_PannelloDiControllo
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SQNPI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case Else
                Throw New Exception("enum_AuditPuaTipo non trovato")
        End Select

        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_Parametri_NG(
                              ByVal SitoOrigine As Enum_SiteRedirector,
                              ByVal Tipo_Audit As enum_AuditPuaTipo,
                              ByVal username_codfisc As String,
                              ByVal piva As String,
                              ByVal programmazione_cod As Integer,
                              Optional ByVal IDSezione As Integer = -1) As String
        Dim Lingua As String = ""
        Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
        ParametriAgronicaAuditPUA.Piva = piva
        ParametriAgronicaAuditPUA.Tipo_Audit = Tipo_Audit
        ParametriAgronicaAuditPUA.username_codfisc = username_codfisc
        ParametriAgronicaAuditPUA.programmazione_cod = programmazione_cod
        ParametriAgronicaAuditPUA.Salva()

        'Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAudit

        Dim SitoDestinazione As Enum_SiteRedirector
        Select Case Tipo_Audit
            Case enum_AuditPuaTipo.Audit_Condizionalita
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case enum_AuditPuaTipo.Audit_COOP
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SicurezzaLavoro
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
            Case enum_AuditPuaTipo.Audit_GlobalGap
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaGlobalGAP
            Case enum_AuditPuaTipo.Audit_SchedaTecnicaTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SchedaControlliTTI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_PannelloDiControllo
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaCheckCOOP
            Case enum_AuditPuaTipo.Audit_SQNPI
                SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAudit
            Case Else
                Throw New Exception("enum_AuditPuaTipo non trovato")
        End Select

        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriConcimazione_2017 As ParametriConcimazione_2017,
                                 Optional ByVal IDSezione As Integer = -1)
        Dim Lingua As String = ""
        ParametriConcimazione_2017.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_PianoConcimazione_2017
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function
    #End Region

    #Region "Analisi_2010"
    Public Shared Function IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(
        ByVal SitoOrigine As Enum_SiteRedirector,
        ByRef ParametriAnalisi_2010 As ParametriAnalisi_2010,
        Optional ByVal IDSezione As Integer = -1
    )
        Dim Lingua As String = ""
        ParametriAnalisi_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_SitoAnalisi_2010_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal TipoAnalisi As TipiEnumerativi.enum_AnalisiTipo) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriAnalisi_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          SitoOrigine,
                                          pagina_provenienza,
                                          Piva,
                                          TipoAnalisi)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal TipoAnalisi As TipiEnumerativi.enum_AnalisiTipo) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriAnalisi_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          SitoOrigine,
                                          pagina_provenienza,
                                          Piva,
                                          TipoAnalisi)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriAnalisi_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal piva As String,
                                         ByVal TipoAnalisi As TipiEnumerativi.enum_AnalisiTipo) As Enum_SiteRedirector
        Dim sito As Enum_SiteRedirector


        Dim ParametriAnalisi_2010_2010 As New ParametriAnalisi_2010
        ParametriAnalisi_2010_2010.Pagina_Richiesta = Pagina_Richiesta
        ParametriAnalisi_2010_2010.SitoOrigine = SitoOrigine
        ParametriAnalisi_2010_2010.Pagina_SitoOrigine = pagina_provenienza
        ParametriAnalisi_2010_2010.Tipo_Analisi = TipoAnalisi
        ParametriAnalisi_2010_2010.Piva = piva
        ParametriAnalisi_2010_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
        Return sito
    End Function
    #End region

    #Region "Profilazione_2010"
    Public Shared Function IndirizzoCompleto_SitoProfilazione_PassandoDirettamente_ParametriProfilazione_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriProfilazione_2010 As ParametriProfilazione_2010)
        Dim Lingua As String = ""
        ParametriProfilazione_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaProfilazione
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                               Optional ByVal Username As String = "") As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriProfilazione_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          Piva,
                                          Username)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoProfilazione_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriProfilazione_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          Piva)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriProfilazione_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         Optional ByVal Username As String = "") As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector


        Dim ParametriProfilazione_2010 As New ParametriProfilazione_2010
        ParametriProfilazione_2010.Piva = Piva
        ParametriProfilazione_2010.Pagina_Richiesta = Pagina_Richiesta
        ParametriProfilazione_2010.Username = Username
        'ParametriProfilazione_2010.Globale = Globale
        'ParametriProfilazione_2010.SitoRichiesto = SitoRichiesto
        'ParametriProfilazione_2010.Sql_Permessi = Sql_Permessi
        'ParametriProfilazione_2010.Stringa_Parametri = Stringa_Parametri
        'ParametriProfilazione_2010.Xml_Permessi = Xml_Permessi
        ParametriProfilazione_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaProfilazione
        Return sito
    End Function
    #End Region

    #Region "Agenda"
    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
                               ByVal SitoOrigine As Enum_SiteRedirector,
                               ByRef objParametriFILTRONE_2010 As ParametriFILTRONE_2010) As String
        Dim Lingua As String = ""
        objParametriFILTRONE_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriFILTRONE_2010 = objParametriFILTRONE_2010
        Dim ParametriConcimazione_2017 As New ParametriConcimazione_2017
        ParametriConcimazione_2017.Leggi()
        objScrivi.ParametriConcimazione_2017 = ParametriConcimazione_2017

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAnalisiCosti_2010(
                           ByVal SitoOrigine As Enum_SiteRedirector,
                           ByRef ParametriAnalisiCosti_2010 As ParametriAnalisiCosti_2010) As String
        Dim Lingua As String = ""
        ParametriAnalisiCosti_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriAnalisiCosti_2010 = ParametriAnalisiCosti_2010

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(
                             ByVal SitoOrigine As Enum_SiteRedirector,
                             ByRef ParametriRicette_2010 As ParametriRicette_2010) As String
        Dim Lingua As String = ""
        ParametriRicette_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriRicette_2010 = ParametriRicette_2010

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010(
                             ByVal SitoOrigine As Enum_SiteRedirector,
                             ByRef ParametriVerificaDisciplinare2010 As ParametriVerificaDisciplinare2010) As String
        Dim Lingua As String = ""
        ParametriVerificaDisciplinare2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriVerificaDisciplinare2010 = ParametriVerificaDisciplinare2010

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriNonConformita(
                         ByVal SitoOrigine As Enum_SiteRedirector,
                         ByRef ParametriNonConformita As ParametriNonConformita) As String
        Dim Lingua As String = ""
        ParametriNonConformita.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriNonConformita = ParametriNonConformita

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010_senza_blocco_script(
                         ByVal SitoOrigine As Enum_SiteRedirector,
                         ByRef ParametriVerificaDisciplinare2010 As ParametriVerificaDisciplinare2010) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010(SitoOrigine, ParametriVerificaDisciplinare2010)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010(
                          ByVal SitoOrigine As Enum_SiteRedirector,
                          ByRef ParametriVerificaDisciplinare2010 As ParametriVerificaDisciplinare2010) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010(SitoOrigine, ParametriVerificaDisciplinare2010)
        Dim script As String = PreparaScripPerPopup(IndirizzoSito_ConQueryString, "VerificaDisciplinare")
        Return script
    End Function

    Public Shared Function ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriNonConformita(
                      ByVal SitoOrigine As Enum_SiteRedirector,
                      ByRef ParametriNonConformita As ParametriNonConformita) As String
        Dim IndirizzoSito_ConQueryString As String = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriNonConformita(SitoOrigine, ParametriNonConformita)
        'ParametriNonConformita.NC_Str
        Dim script As String = "apriFormDialog('" & IndirizzoSito_ConQueryString & "',$(window).width()*90/100, $(window).height()*90/100);"
        'Dim script As String = PreparaScripPerIframe(IndirizzoSito_ConQueryString)
        Return script
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAgenda_2010 As ParametriAgenda_2010,
                                 Optional ByVal lingua As String = "", Optional ByVal IDSezione As Integer = -1)
        'Dim Lingua As String = ""
        ParametriAgenda_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    'ATTENZIONE: Questa funzione è la versione di IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010()
    'ma senza utilizzo della Session (e un nome meno complicato)
    Public Shared Function IndirizzoSitoAgenda_daParamsAgenda2010(ByVal SitoOrigine As Enum_SiteRedirector,
                                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                  ByRef objParametri_SuperServer As AgronicaCoreParametri,
                                                                  ByRef objParams_Agenda_2010 As ParametriAgenda_2010,
                                                                  ByVal objParams_AuditPua As ParametriAgronicaAuditPUA,
                                                                  ByVal objParams_Concimazione2017 As ParametriConcimazione_2017,
                                                                  Optional ByVal lingua As String = "") As String
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        Dim url As String = PreparaPassaggioSito(SitoOrigine,
                                                 SitoDestinazione,
                                                 lingua,
                                                 objParametri_Server,
                                                 objParametri_Utenti,
                                                 objParametri_SuperServer,
                                                 objParams_Agenda_2010,
                                                 objParams_AuditPua,
                                                 objParams_Concimazione2017)
        Return url
    End Function

    Public Shared Function ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                  ByVal SitoOrigine As Enum_SiteRedirector,
                                  ByRef ParametriAgenda_2010 As ParametriAgenda_2010) As String
        Dim Lingua As String = ""
        ParametriAgenda_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAgenda As Parametri_ObjParametriAgenda_NG,
                                 Optional ByVal lingua As String = "")
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.GiasNG
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgronicaUMA_PassandoDirettamente_ParametriAgenda_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAgenda As ParametriAgenda_2010,
                                 Optional ByVal lingua As String = "",
                                 Optional ByVal IDSezione As Integer = -1)
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaUma
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgronicaDomandaIrrigua_PassandoDirettamente_ParametriAgenda_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAgenda As ParametriAgenda_2010,
                                 Optional ByVal lingua As String = "",
                                 Optional ByVal IDSezione As Integer = -1)
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, lingua, "", IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function ApriPopUp_SitoLabQualita(
                              ByVal SitoOrigine As Enum_SiteRedirector,
                              ByVal piva As String) As String
        Dim Lingua As String = ""
        Dim ParametriLabCQ As New AgronicaCoreGestioneRichieste.ParametriLabCQ
        ParametriLabCQ.Leggi()
        ParametriLabCQ.Piva = piva
        ParametriLabCQ.PaginaProvenienza = SitoOrigine
        ParametriLabCQ.PaginaRichiesta = Enum_SiteRedirector.Sito_AgronicaLabQualita
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaLabQualita
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Sub ApriNellaStessaPagina_ConRedirect_PassandoDirettamente_ParametriAgenda_2010(
                                     ByVal SitoOrigine As Enum_SiteRedirector,
                                     ByRef ParametriAgenda_2010 As ParametriAgenda_2010,
                                     ByRef objPage As System.Web.UI.Page)
        Dim IndirizzoSito As String = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(SitoOrigine, ParametriAgenda_2010)
        objPage.Response.Redirect(IndirizzoSito)
    End Sub
    #End Region

    #Region "Sincronizzatore_2010"
    Public Shared Function ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          Piva,
                                          Id_Cod_Cliente)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal ParametriQueryString As String,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          ParametriQueryString,
                                          Piva,
                                          Id_Cod_Cliente)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function
    Public Shared Function ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametriBS(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          Piva,
                                          Id_Cod_Cliente)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)

        Dim strJSON As String = "{ ""UrlWindow"":""" & PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua) & """ ,"
        strJSON &= " ""WindowName"":""" & descrizioneFromSito(SitoDestinazione) & """ }"

        Return strJSON
    End Function
    #End Region

    #Region "Sincro 2010"
    Public Shared Function IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriSincro_2010 As ParametriSincronizzatore_2010,
                                 Optional ByVal IdSezione As Integer = -1)
        Dim Lingua As String = ""
        ParametriSincro_2010.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaSincronizzatore
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IdSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_Sitosincronizzatore_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          Piva,
                                          Id_Cod_Cliente)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector


        Dim Parametrisincronizzatore_2010 As New ParametriSincronizzatore_2010
        Parametrisincronizzatore_2010.Piva = Piva
        Parametrisincronizzatore_2010.Pagina_Richiesta = Pagina_Richiesta
        'Parametrisincronizzatore_2010.pagina_provenienza = pagina_provenienza
        Parametrisincronizzatore_2010.Id_Cod_Cliente = Id_Cod_Cliente
        Parametrisincronizzatore_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaSincronizzatore
        Return sito
    End Function

    Private Shared Function Prepara_Parametrisincronizzatore_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PagineAgronicaSincro,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal parametriQueryString As String,
                                         ByVal Piva As String,
                                         ByVal Id_Cod_Cliente As String) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector
        Dim Parametrisincronizzatore_2010 As New ParametriSincronizzatore_2010
        Parametrisincronizzatore_2010.Piva = Piva
        Parametrisincronizzatore_2010.Pagina_Richiesta = Pagina_Richiesta
        'Parametrisincronizzatore_2010.pagina_provenienza = pagina_provenienza
        Parametrisincronizzatore_2010.Id_Cod_Cliente = Id_Cod_Cliente
        Parametrisincronizzatore_2010.ParametriQueryString = parametriQueryString
        Parametrisincronizzatore_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaSincronizzatore
        Return sito
    End Function
    #End Region

    #Region "PianiSemina"
    Public Shared Function ApriPopUp_SitoPianiSemina_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As Integer,
                                         ByVal pagina_provenienza As Integer) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriPianiSemina_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoPianiSemina_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As Integer,
                                         ByVal pagina_provenienza As Integer) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriPianiSemina_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriPianiSemina_In_Sessione(
                                         ByVal Pagina_Richiesta As Integer,
                                         ByVal pagina_provenienza As Integer) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector
        'NON HA PARAMETRI
        'Dim ParametriPianiSemina As New ParametriPianiSemina
        'ParametriPianiSemina.PaginaRichiesta = Pagina_Richiesta
        'ParametriPianiSemina.ListaImpianti = ListaImpianti
        ' ParametriSincronizzatore_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaPianiSemina
        Return sito
    End Function
    #End Region

    #Region "PianiCampionamento_2010"
    Public Shared Function ApriPopUp_SitoPianiCampionamento_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PaginePianiCampionamento,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal ListaImpianti As Impianto(),
                                         Optional ByVal Id_PDC_Testata As Integer = 0,
                                         Optional ByVal Piva As String = "") As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriPianidiCampionamento_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          ListaImpianti,
                                          Id_PDC_Testata,
                                          Piva)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoPianiCampionamento_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PaginePianiCampionamento,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal ListaImpianti As Impianto(),
                                         Optional ByVal Id_PDC_Testata As Integer = 0,
                                         Optional ByVal Piva As String = "",
                                         Optional ByVal IdSezione As Integer = -1) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriPianidiCampionamento_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          ListaImpianti,
                                          Id_PDC_Testata,
                                          Piva)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, "", IdSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriPianidiCampionamento_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PaginePianiCampionamento,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal ListaImpianti As Impianto(),
                                         ByVal Id_PDC_Testata As Integer,
                                         ByVal Piva As String) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector
        Dim ParametriPianidiCampionamento_2010 As New ParametriPianidiCampionamento_2010
        ParametriPianidiCampionamento_2010.PaginaRichiesta = Pagina_Richiesta
        ParametriPianidiCampionamento_2010.ListaImpianti = ListaImpianti
        ParametriPianidiCampionamento_2010.Id_PDC_Testata = Id_PDC_Testata
        ParametriPianidiCampionamento_2010.Piva = Piva
        ParametriPianidiCampionamento_2010.Salva()
        sito = Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
        Return sito
    End Function
    #End Region

    #Region "STAMPE"
    Public Shared Function ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriAgronicaStampe As ParametriAgronicaStampe) As String
        Dim strJS As String = ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(SitoOrigine,
                                                                                  ParametriAgronicaStampe.report,
                                                                                  ParametriAgronicaStampe.username,
                                                                                  ParametriAgronicaStampe.user_profilo,
                                                                                  ParametriAgronicaStampe.Xml_Generico.ToString,
                                                                                  ParametriAgronicaStampe.UserProfilo_CodFisc,
                                                                                  ParametriAgronicaStampe.Sql_Filtro,
                                                                                  ParametriAgronicaStampe.Xml_Filtro,
                                                                                  ParametriAgronicaStampe.username,
                                                                                  ParametriAgronicaStampe.utente_codfiscale,
                                                                                  ParametriAgronicaStampe.superuser_username,
                                                                                  ParametriAgronicaStampe.superuser_codfiscale,
                                                                                  ParametriAgronicaStampe.data_stampa,
                                                                                  ParametriAgronicaStampe.centro_costo,
                                                                                  ParametriAgronicaStampe.superficie,
                                                                                  ParametriAgronicaStampe.ricavi,
                                                                                  ParametriAgronicaStampe.costi,
                                                                                  ParametriAgronicaStampe.differenza,
                                                                                  ParametriAgronicaStampe.ricavi_ha,
                                                                                  ParametriAgronicaStampe.costi_ha,
                                                                                  ParametriAgronicaStampe.visualizzazione,
                                                                                  ParametriAgronicaStampe.differenza_ha,
                                                                                  0,
                                                                                  ParametriAgronicaStampe.JSon_Generico.ToString)
        Return strJS
    End Function

    Public Shared Function ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(ByVal SitoOrigine As Enum_SiteRedirector,
                                             ByVal ReportSelezionato As enum_CodificaStampe,
                                             ByVal Username As String,
                                             ByVal User_Profilo As String,
                                             ByVal StrNodiVariabili As String,
                                             ByVal UserProfilo_CodFisc As String,
                                             ByVal Sql_Filtro As String,
                                             ByVal Xml_Filtro As String,
                                             ByVal username_codfisc As String,
                                             ByVal utente_codfiscale As String,
                                             ByVal superuser_username As String,
                                             ByVal superuser_codfiscale As String,
                                             Optional ByVal data_stampa As String = "",
                                             Optional ByVal centro_costo As String = "",
                                             Optional ByVal superficie As String = "",
                                             Optional ByVal ricavi As String = "",
                                             Optional ByVal costi As String = "",
                                             Optional ByVal differenza As String = "",
                                             Optional ByVal ricavi_ha As String = "",
                                             Optional ByVal costi_ha As String = "",
                                             Optional ByVal visualizzazione As String = "",
                                             Optional ByVal differenza_ha As String = "",
                                             Optional ByVal id_agenda As Integer = 0,
                                                  Optional ByVal JSon_Generico As String = "") As String
        'nuova che gestisce stampe 2010 e forse superserver e la stringa con tutti i parametri come gli altri siti
        'inizializzo objWebConfig se non presente
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriAgronicaStampe_In_Sessione(ReportSelezionato, Username, User_Profilo, StrNodiVariabili, UserProfilo_CodFisc, Sql_Filtro, Xml_Filtro, username_codfisc, utente_codfiscale, superuser_username, superuser_codfiscale, data_stampa,
                                                                                                  centro_costo, superficie, ricavi, costi, differenza, ricavi_ha, costi_ha, visualizzazione, differenza_ha, id_agenda, JSon_Generico)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua, JSon_Generico)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal ReportSelezionato As enum_CodificaStampe,
                                         ByVal ASG_Utente_Username As String,
                                         ByVal ASG_ProgressivoGIAS As String,
                                         ByVal StrNodiVariabili As String,
                                         ByVal UserProfilo_CodFisc As String,
                                         ByVal Sql_Filtro As String,
                                         ByVal Xml_Filtro As String,
                                         ByVal username_codfisc As String,
                                            ByVal utente_codfiscale As String,
                                            ByVal superuser_username As String,
                                            ByVal superuser_codfiscale As String,
                                            Optional ByVal data_stampa As String = "",
                                            Optional ByVal centro_costo As String = "",
                                             Optional ByVal superficie As String = "",
                                             Optional ByVal ricavi As String = "",
                                             Optional ByVal costi As String = "",
                                             Optional ByVal differenza As String = "",
                                             Optional ByVal ricavi_ha As String = "",
                                             Optional ByVal costi_ha As String = "",
                                             Optional ByVal visualizzazione As String = "",
                                             Optional ByVal differenza_ha As String = "",
                                             Optional ByVal id_agenda As Integer = 0,
                                                  Optional ByVal JSon_Generico As String = "") As String
        'nuova che gestisce stampe 2010 e forse superserver e la stringa con tutti i parametri come gli altri siti
        'inizializzo objWebConfig se non presente
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriAgronicaStampe_In_Sessione(ReportSelezionato, ASG_Utente_Username, ASG_ProgressivoGIAS, StrNodiVariabili, UserProfilo_CodFisc, Sql_Filtro, Xml_Filtro, username_codfisc, utente_codfiscale, superuser_username, superuser_codfiscale, data_stampa,
                                                                                                  centro_costo, superficie, ricavi, costi, differenza, ricavi_ha, costi_ha, visualizzazione, differenza_ha, id_agenda, JSon_Generico)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, JSon_Generico)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                             ByVal SitoOrigine As Enum_SiteRedirector,
                             ByRef ParametriAgronicaStampe As ParametriAgronicaStampe) As String
        Dim strJS As String = ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(SitoOrigine,
                                                                                  ParametriAgronicaStampe.report,
                                                                                  ParametriAgronicaStampe.username,
                                                                                  ParametriAgronicaStampe.user_profilo,
                                                                                  ParametriAgronicaStampe.Xml_Generico.ToString,
                                                                                  ParametriAgronicaStampe.UserProfilo_CodFisc,
                                                                                  ParametriAgronicaStampe.Sql_Filtro,
                                                                                  ParametriAgronicaStampe.Xml_Filtro,
                                                                                  ParametriAgronicaStampe.username,
                                                                                  ParametriAgronicaStampe.utente_codfiscale,
                                                                                  ParametriAgronicaStampe.superuser_username,
                                                                                  ParametriAgronicaStampe.superuser_codfiscale)
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriAgronicaStampe_In_Sessione(ParametriAgronicaStampe.report, ParametriAgronicaStampe.username, ParametriAgronicaStampe.user_profilo, ParametriAgronicaStampe.Xml_Generico.ToString, ParametriAgronicaStampe.UserProfilo_CodFisc, ParametriAgronicaStampe.Sql_Filtro, ParametriAgronicaStampe.Xml_Filtro, ParametriAgronicaStampe.username, ParametriAgronicaStampe.utente_codfiscale, ParametriAgronicaStampe.superuser_username, ParametriAgronicaStampe.superuser_codfiscale, ParametriAgronicaStampe.data_stampa,
                                                                                                  ParametriAgronicaStampe.centro_costo, ParametriAgronicaStampe.superficie, ParametriAgronicaStampe.ricavi, ParametriAgronicaStampe.costi, ParametriAgronicaStampe.differenza, ParametriAgronicaStampe.ricavi_ha, ParametriAgronicaStampe.costi_ha, ParametriAgronicaStampe.visualizzazione, ParametriAgronicaStampe.differenza_ha, ParametriAgronicaStampe.Id_Agenda, ParametriAgronicaStampe.JSon_Generico.ToString)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, ParametriAgronicaStampe.JSon_Generico.ToString, ParametriAgronicaStampe.Id_Sezione)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriAgronicaStampe_In_Sessione(ByVal ReportSelezionato As enum_CodificaStampe,
                                  ByVal username As String,
                                  ByVal user_profilo As String,
                                  ByVal StrNodiVariabili As String,
                                  ByVal UserProfilo_CodFisc As String,
                                  ByVal Sql_Filtro As String,
                                  ByVal Xml_Filtro As String,
                                  ByVal username_codfisc As String,
                                    ByVal utente_codfiscale As String,
                                    ByVal superuser_username As String,
                                    ByVal superuser_codfiscale As String,
                                    ByVal data_stampa As String,
                                     ByVal centro_costo As String,
                                     ByVal superficie As String,
                                     ByVal ricavi As String,
                                     ByVal costi As String,
                                     ByVal differenza As String,
                                     ByVal ricavi_ha As String,
                                     ByVal costi_ha As String,
                                     ByVal visualizzazione As String,
                                     ByVal differenza_ha As String,
                                     ByVal id_agenda As Integer,
                                       ByVal JSon_Generico As String) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector
        Dim strReport() As String = objWebConfig.enumStampe2010.Split(",")
        Dim permessoReportStampe2010 As Boolean = False
        'se non c'è la chiave lascio il permesso
        If (strReport.Length = 0) Then
            permessoReportStampe2010 = True
        Else
            Dim i As Integer = 0
            For i = 0 To strReport.Length - 1
                If Trim(strReport(i)) <> "" AndAlso IsNumeric(Trim(strReport(i))) andalso CInt(Trim(strReport(i))) = CInt(ReportSelezionato)  Then
                        permessoReportStampe2010 = True
                        Exit For
                End If
            Next
        End If

        If (objWebConfig.Stampe2010 = "False") OrElse Not permessoReportStampe2010 Then
            Dim objAgronicaStampe As New ParametriAgronicaStampe
            objAgronicaStampe.report = ReportSelezionato
            objAgronicaStampe.username = username
            objAgronicaStampe.user_profilo = user_profilo
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
            objAgronicaStampe.UserProfilo_CodFisc = UserProfilo_CodFisc
            objAgronicaStampe.Sql_Filtro = Sql_Filtro
            objAgronicaStampe.Xml_Filtro = Xml_Filtro
            objAgronicaStampe.username_codfisc = username_codfisc
            objAgronicaStampe.utente_codfiscale = utente_codfiscale
            objAgronicaStampe.superuser_codfiscale = superuser_codfiscale
            objAgronicaStampe.superuser_username = superuser_username
            objAgronicaStampe.data_stampa = data_stampa
            objAgronicaStampe.centro_costo = centro_costo
            objAgronicaStampe.superficie = superficie
            objAgronicaStampe.ricavi = ricavi
            objAgronicaStampe.costi = costi
            objAgronicaStampe.differenza = differenza
            objAgronicaStampe.ricavi_ha = ricavi_ha
            objAgronicaStampe.costi_ha = costi_ha
            objAgronicaStampe.visualizzazione = visualizzazione
            objAgronicaStampe.differenza_ha = differenza_ha
            objAgronicaStampe.Id_Agenda = id_agenda
            objAgronicaStampe.JSon_Generico.Length = 0
            objAgronicaStampe.JSon_Generico.Append(JSon_Generico)
            objAgronicaStampe.Salva()

            sito = Enum_SiteRedirector.Sito_AgronicaStampe
        Else
            Dim objAgronicaStampe_2010 As New ParametriAgronicaStampe_2010
            objAgronicaStampe_2010.report = ReportSelezionato
            objAgronicaStampe_2010.username = username
            objAgronicaStampe_2010.user_profilo = user_profilo
            objAgronicaStampe_2010.Xml_Generico.Length = 0
            objAgronicaStampe_2010.Xml_Generico.Append(StrNodiVariabili)
            objAgronicaStampe_2010.UserProfilo_CodFisc = UserProfilo_CodFisc
            objAgronicaStampe_2010.Sql_Filtro = Sql_Filtro
            objAgronicaStampe_2010.Xml_Filtro = Xml_Filtro
            objAgronicaStampe_2010.username_codfisc = username_codfisc
            objAgronicaStampe_2010.data_stampa = data_stampa
            objAgronicaStampe_2010.id_agenda = id_agenda
            objAgronicaStampe_2010.JSon_Generico.Length = 0
            objAgronicaStampe_2010.JSon_Generico.Append(JSon_Generico)
            objAgronicaStampe_2010.Salva()

            sito = Enum_SiteRedirector.Sito_AgronicaStampe_2010
        End If

        Return sito
    End Function

    'creata per sostituire xmlparametriforsiteredirector
    'che in base al report imposta i parametri
    'obbligatorio per gestione del filtrino altrimenti incasinata
    Public Shared Function Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                ByVal ReportSelezionato As enum_CodificaStampe,
                                ByVal Piva As String,
                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                Optional ByVal Rag_Soc As String = "",
                                Optional ByVal Sa_Cod As Integer = 0,
                                Optional ByVal Fabbricato_Cod As Integer = 0,
                                Optional ByVal Elem_Cod As Integer = 0,
                                Optional ByVal Pro_Cod As Integer = 0,
                                Optional ByVal Mat_Cod As Integer = 0,
                                Optional ByVal Data_Stampa As Date = AGRODATAINIZIO,
                                Optional ByVal Data_Inizio As Date = AGRODATAINIZIO,
                                Optional ByVal Data_Fine As Date = AGRODATAFINE,
                                Optional ByVal Anno As Integer = 0
                                ) As ParametriAgronicaStampe
        Dim objAgronicaStampe As New ParametriAgronicaStampe
        objAgronicaStampe.report = ReportSelezionato
        Dim username As String = ""
        Dim user_profilo As String = ""
        Dim user_profilo_codfiscale As String = ""
        Dim Sql_Filtro As String = ""
        Dim XML_Filtro As String = ""
        Dim username_codfisc As String = ""
        Dim utente_codfiscale As String = ""
        Dim superuser_username As String = ""
        Dim superuser_codfiscale As String = ""
        Dim StrNodiVariabili As String = ""
        Dim StrNodo As String = ""
        Dim user_profilo_codgias As String = ""
        Dim user_profilo_codfisc As String = ""
        Dim objStampeXml As New AgronicaCoreXML.XML_Stampe

        Select Case ReportSelezionato
            ' da FiltroElaboratiContabili_PreparaParametri
            Case enum_CodificaStampe.Registro_FattureVendita,
                enum_CodificaStampe.Registro_FattureAcquisto,
                enum_CodificaStampe.Bilancio_Civilistico,
                enum_CodificaStampe.PianoDeiConti,
                enum_CodificaStampe.Mastrino,
                enum_CodificaStampe.Bilanci_DiVerifica_Confronto,
                enum_CodificaStampe.LiquidazionePeriodica_IVA,
                enum_CodificaStampe.Lista_InsolutiClienti,
                enum_CodificaStampe.Lista_InsolutiFornitori,
                enum_CodificaStampe.RiBa_Report_Presentazione,
                enum_CodificaStampe.Registro_Corrispettivi

                username = CStr(objSession("ASG_Utente_Username"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                user_profilo_codgias = CStr(objSession("ASG_ProgressivoGIAS"))
                user_profilo = CStr(objSession("ASG_SuperUser_Username"))
                user_profilo_codfisc = CStr(objSession("ASG_SuperUser_CodFiscale"))

                StrNodiVariabili = objStampeXml.StrXmlParametri_FiltroElaboratiContabili(ReportSelezionato,
                                                                                         Piva,
                                                                                         Rag_Soc,
                                                                                         Anno,
                                                                                         Data_Inizio,
                                                                                         Data_Fine)

                'stampe newmode
            Case enum_CodificaStampe.Produzioni_XLS

                'Suppongo
                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                enum_CodificaStampe.SchedaMagazzinoMovimenti,
                enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                StrNodiVariabili = objStampeXml.StrXmlParametri_Schede_Magazzino(ReportSelezionato,
                                                                                 Piva,
                                                                                 Sa_Cod,
                                                                                 Fabbricato_Cod,
                                                                                 Elem_Cod,
                                                                                 Pro_Cod,
                                                                                 Mat_Cod,
                                                                                 AGRODATAINIZIO,
                                                                                 AGRODATAINIZIO,
                                                                                 AGRODATAFINE)
                'Suppongo
                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

            Case enum_CodificaStampe.GestioneEtichette_Trasformati

                StrNodiVariabili = objStampeXml.StrXmlParametri_GestioneEtichette_Trasformati(ReportSelezionato,
                                                                                              Piva,
                                                                                              Mat_Cod)
                'Suppongo
                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
                enum_CodificaStampe.SchedaVendite_Biologico,
                enum_CodificaStampe.SchedaPreparati_Biologico

                StrNodiVariabili = objStampeXml.StrXmlParametri_GestioneSchedeBio(ReportSelezionato,
                                                                                  Piva)
                'Suppongo
                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

            Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea


                StrNodiVariabili = objStampeXml.StrXmlParametri_GestioneReportIncongruenze(ReportSelezionato,
                                                                                           Piva)
                'Suppongo
                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))
                'fine stampe newmode

                '#####  Gestione Ricette Lista #############################
            Case enum_CodificaStampe.GestioneRicette_Lista

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = objSession("ASG_Utente_Username")
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                user_profilo_codfiscale = objSession("ASG_SuperUser_CodFiscale")

                '#####  Gestione Ricette Edit #############################
            Case enum_CodificaStampe.GestioneRicette_Edit

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo
                username = objSession("ASG_Utente_Username")
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                user_profilo_codfiscale = objSession("ASG_SuperUser_CodFiscale")

                '#####  Gestione Allegati  #############################
            Case enum_CodificaStampe.GestioneAllegati

                Dim xChiave As String = ""
                Dim vVarStampe(3) As ElementoStampe
                vVarStampe(0).Nome = "Agro_UserName_Crypt"
                vVarStampe(0).Valore = objSession("ASG_Utente_Username_Crypt")
                vVarStampe(1).Nome = "Agro_UserPassword_Crypt"
                vVarStampe(1).Valore = objSession("ASG_Utente_Password_Crypt")

                AgronicaCoreDataProvider.Albero.ChiaveAlbero_Codifica(xChiave, enum_TipoNodo.Impresa, Piva)

                vVarStampe(2).Nome = "NodoChiave"
                vVarStampe(2).Valore = CStr(Stringa_Codifica(xChiave, AgroKey_EncoderDecoder))
                vVarStampe(3).Nome = "piva"
                vVarStampe(3).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = objSession("ASG_Utente_Username")
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                user_profilo = objSession("ASG_SuperUser_Username")
                superuser_codfiscale = objSession("ASG_SuperUser_CodFiscale")

                '#####  Gestione Registri Stalle  #############################
            Case enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento,
                enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

                '#####  Esporta_Conferimenti_JDEdwards #############################
            Case enum_CodificaStampe.Esporta_Conferimenti_JDEdwards

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

                '#####  Report Accettazione da Diversi #############################
            Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi,
                enum_CodificaStampe.Importazione_DDT_PianoColturale

                Dim vVarStampe(1) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva
                vVarStampe(1).Nome = "rag_soc"
                Dim obj As New AgronicaCoreAnagrafeDAL.Imprese_Read
                If Rag_Soc = "" Then
                    Rag_Soc = obj.RagSoc_from_Piva(Piva, objParametri_Server)
                End If
                vVarStampe(1).Valore = Rag_Soc

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                ''utente_usr = CStr(objSession("ASG_Utente_Username"))
                ''utente_cf = CStr(objSession("ASG_Utente_CodFiscale"))
                ''superuser_usr = CStr(objSession("ASG_SuperUser_Username"))
                ''superuser_cf = CStr(objSession("ASG_SuperUser_CodFiscale"))
                ''progressivo_gias = CStr(objSession("ASG_ProgressivoGIAS"))

                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

                '#####  Registri di cantina #############################
            Case enum_CodificaStampe.Registri_Preparazioni

                Dim vVarStampe(1) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva
                vVarStampe(1).Nome = "sa_cod"
                vVarStampe(1).Valore = CStr(0)

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                'Inserisco l'XML nella stringa complessiva
                StrNodiVariabili = StrNodiVariabili & StrNodo
                username = CStr(objSession("ASG_Utente_Username"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

                '#####  Importazione Contatti XLS 2 Gias #############################
            Case enum_CodificaStampe.ImportaContatti_XLS2GIAS

                username = CStr(objSession("ASG_Utente_Username"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

                '#####  Monitoraggio Corpi Estranei #############################
            Case enum_CodificaStampe.ExportExcel_MonitoraggioCE,
                enum_CodificaStampe.ExportExcel_MonitoraggioCE_Aggregata

                Dim vVarStampe(13) As ElementoStampe

                vVarStampe(0).Nome = "dal"
                vVarStampe(0).Valore = objSession("dal")

                vVarStampe(1).Nome = "al"
                vVarStampe(1).Valore = objSession("al")

                vVarStampe(2).Nome = "piva_produttore"
                vVarStampe(2).Valore = objSession("piva_produttore")

                vVarStampe(3).Nome = "veg_cod"
                vVarStampe(3).Valore = objSession("veg_cod")

                vVarStampe(4).Nome = "mat_cod"
                vVarStampe(4).Valore = objSession("mat_cod")

                vVarStampe(5).Nome = "flag_appezza"
                vVarStampe(5).Valore = objSession("flag_appezza")

                vVarStampe(6).Nome = "sa_cod"
                vVarStampe(6).Valore = objSession("sa_cod")

                vVarStampe(7).Nome = "appezza"
                vVarStampe(7).Valore = objSession("appezza")

                vVarStampe(8).Nome = "id_reg"
                vVarStampe(8).Valore = objSession("id_reg")

                vVarStampe(9).Nome = "tipologia"
                vVarStampe(9).Valore = objSession("tipologia")

                vVarStampe(10).Nome = "pericolosita"
                vVarStampe(10).Valore = objSession("pericolosita")

                vVarStampe(11).Nome = "regolamento"
                vVarStampe(11).Valore = objSession("regolamento")

                vVarStampe(12).Nome = "sa_cod_fabbr"
                vVarStampe(12).Valore = objSession("sa_cod_fabbr")

                vVarStampe(13).Nome = "fabbr_cod"
                vVarStampe(13).Valore = objSession("fabbr_cod")

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = objSession("ASG_Utente_Username")
                username_codfisc = objSession("ASG_Utente_CodFiscale")
                user_profilo = objSession("ASG_SuperUser_Username")
                superuser_codfiscale = objSession("ASG_SuperUser_CodFiscale")

                'metto a nothing gli oggetti di sessione
                objSession("dal") = Nothing
                objSession("al") = Nothing
                objSession("piva_produttore") = Nothing
                objSession("veg_cod") = Nothing
                objSession("mat_cod") = Nothing
                objSession("flag_appezza") = Nothing
                objSession("sa_cod") = Nothing
                objSession("appezza") = Nothing
                objSession("id_reg") = Nothing
                objSession("tipologia") = Nothing
                objSession("pericolosita") = Nothing
                objSession("regolamento") = Nothing

                '#####  Altre Stampe  #############################
                'Al momento passano qui le stampe del pacchetto igiene clienti/fornitori,
                'l'esportazione anagrafica prodotti, l'esportazione anagrafica contatti,
                'l'importazione raccolte e conferimenti da Rintraccio
                '#####  Gestione Allegati  #############################
            Case Else

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                StrNodo = objStampeXml.XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                username = CStr(objSession("ASG_Utente_Username"))
                utente_codfiscale = CStr(objSession("ASG_Utente_CodFiscale"))
                superuser_username = CStr(objSession("ASG_SuperUser_Username"))
                superuser_codfiscale = CStr(objSession("ASG_SuperUser_CodFiscale"))
                user_profilo = CStr(objSession("ASG_ProgressivoGIAS"))

        End Select

        objAgronicaStampe.username = username
        objAgronicaStampe.user_profilo = user_profilo
        objAgronicaStampe.Xml_Generico.Length = 0
        objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
        objAgronicaStampe.UserProfilo_CodFisc = superuser_codfiscale
        objAgronicaStampe.Sql_Filtro = Sql_Filtro
        objAgronicaStampe.Xml_Filtro = XML_Filtro
        objAgronicaStampe.username_codfisc = username_codfisc
        objAgronicaStampe.utente_codfiscale = utente_codfiscale
        objAgronicaStampe.superuser_codfiscale = superuser_codfiscale
        objAgronicaStampe.superuser_username = superuser_username
        objAgronicaStampe.user_profilo_codgias = user_profilo_codgias
        objAgronicaStampe.user_profilo_codfisc = user_profilo_codfisc

        Return objAgronicaStampe
    End Function
    #end Region

    #Region "ONLINE2010"
    Public Shared Function ApriIFrame_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(
                                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                                         ByRef ParametriScadenziario As ParametriScadenziario) As String
        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(SitoOrigine, ParametriScadenziario)
        Return PreparaScripPerIframe(link)
    End Function

    Public Shared Function IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriScadenziario(
                                                     ByVal SitoOrigine As Enum_SiteRedirector,
                                                     ByRef ParametriScadenziario As ParametriScadenziario) As String
        Dim Lingua As String = ""
        ParametriScadenziario.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        
        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkAgronicaAgenda2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriScadenziario = ParametriScadenziario

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(
                                                     ByVal SitoOrigine As Enum_SiteRedirector,
                                                     ByRef ParametriScadenziario As ParametriScadenziario) As String
        Dim Lingua As String = ""
        ParametriScadenziario.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline_2010

        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim IndirizzoSito As String = objWebConfig.LinkGiasOnline_2010

        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess
        objScrivi.ParametriScadenziario = ParametriScadenziario

        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi)
        Return IndirizzoSito_ConQueryString
    End Function
    Public Shared Function ApriPopUp_SitoOnline2010_Conttatti(
                                                             ByVal Sito_Provenienza As Enum_SiteRedirector,
                                                             ByVal piva As String,
                                                             ByVal cod_contatto_piva As String,
                                                             ByVal cod_contatto As String,
                                                             ByVal cod_contatto_sa_cod As String,
                                                             Optional ByVal Tipo_Rapporto_Default As String = "")
        Return ApriPopUp_SitoOnline2010_PassandoDirettamenteIParametri(Sito_Provenienza,
                                                                enum_PagineGiasOnline_2010.Contatto,
                                                                0, piva, cod_contatto, cod_contatto_piva, cod_contatto_sa_cod, Tipo_Rapporto_Default)
    End Function

    Public Shared Function ApriPopUp_SitoOnline2010_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal piva As String,
                                         ByVal Cod_Contatto As String,
                                         ByVal Cod_Contatto_Piva As String,
                                         ByVal Cod_Contatto_Sa_Cod As Integer,
                                         ByVal Rapporto_Contabile As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriGiasOnline_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          piva,
                                          Cod_Contatto,
                                          Cod_Contatto_Piva,
                                          Cod_Contatto_Sa_Cod,
                                          Rapporto_Contabile)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal piva As String,
                                         ByVal Cod_Contatto As String,
                                         ByVal Cod_Contatto_Piva As String,
                                         ByVal Cod_Contatto_Sa_Cod As Integer,
                                         ByVal Rapporto_Contabile As String) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriGiasOnline_2010_In_Sessione(
                                          Pagina_Richiesta,
                                          pagina_provenienza,
                                          piva,
                                          Cod_Contatto,
                                          Cod_Contatto_Piva,
                                          Cod_Contatto_Sa_Cod,
                                          Rapporto_Contabile)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriGiasOnline_2010_In_Sessione(
                                         ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                         ByVal pagina_provenienza As Integer,
                                         ByVal piva As String,
                                         ByVal Cod_Contatto As String,
                                         ByVal Cod_Contatto_Piva As String,
                                         ByVal Cod_Contatto_Sa_Cod As Integer,
                                         ByVal Rapporto_Contabile As String) As Enum_SiteRedirector
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector
        Dim objGiasOnline_2010 As New ParametriGiasOnline_2010
        objGiasOnline_2010.Pagina_Richiesta = Pagina_Richiesta
        objGiasOnline_2010.pagina_provenienza = pagina_provenienza
        objGiasOnline_2010.piva = piva
        objGiasOnline_2010.Cod_Contatto = Cod_Contatto
        objGiasOnline_2010.Cod_Contatto_Piva = Cod_Contatto_Piva
        objGiasOnline_2010.Cod_Contatto_Sa_Cod = Cod_Contatto_Sa_Cod
        objGiasOnline_2010.Rapporto_Contabile = Rapporto_Contabile
        objGiasOnline_2010.Salva()
        sito = Enum_SiteRedirector.Sito_GiasOnline_2010
        Return sito
    End Function
    #end region

    #Region "ONLINE2003"
    Public Shared Function ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                 ByRef ParametriGiasOnline As ParametriGiasOnline) As String
        Dim Lingua As String = ""
        ParametriGiasOnline.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function ApriPopUp_SitoOnline_PassandoDirettamenteIParametri(
                                          ByVal SitoOrigine As Enum_SiteRedirector,
                                          ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                          ByVal Piva As String,
                                          ByVal Cod_Contatto As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Rag_Soc As String,
                                          ByVal Sa_Nome As String,
                                          ByVal DataSelezionata As Date,
                                          ByVal Veg_Cod As Integer,
                                          ByVal Cul_Cod As Integer,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Operazione As Integer,
                                          ByVal Lavorazione As Integer,
                                          ByVal DPI_Cod As String,
                                          ByVal IdRcdpi As String,
                                          ByVal Mode As String,
                                          ByVal Causale As String,
                                          ByVal xChiave As String,
                                          ByVal ElemCod As String,
                                          ByVal Id_Reg As String,
                                          ByVal AmaxX As String,
                                          ByVal AmaxY As String,
                                          ByVal AminX As String,
                                          ByVal AminY As String,
                                          ByVal Xml_Generico As Text.StringBuilder) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriGiasOnline_In_Sessione(
                                           Pagina_Richiesta,
                                           Piva,
                                           Cod_Contatto,
                                           Sa_Cod,
                                           Rag_Soc,
                                           Sa_Nome,
                                           DataSelezionata,
                                           Veg_Cod,
                                           Cul_Cod,
                                           Id_Agenda,
                                           Operazione,
                                           Lavorazione,
                                           DPI_Cod,
                                           IdRcdpi,
                                           Mode,
                                           Causale,
                                           xChiave,
                                           ElemCod,
                                           Id_Reg,
                                           AmaxX,
                                           AmaxY,
                                           AminX,
                                           AminY,
                                           Xml_Generico)
        Dim strJS As String = ApriPopUp_SitoGenerico_ConParametriSitoInSessione(SitoOrigine, SitoDestinazione, Lingua)
        Return strJS
    End Function

    Public Shared Function IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                ByVal SitoOrigine As Enum_SiteRedirector,
                                ByRef ParametriGiasOnline As ParametriGiasOnline) As String
        Dim Lingua As String = ""
        ParametriGiasOnline.Salva()
        Dim SitoDestinazione As Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Public Shared Function IndirizzoCompleto_SitoOnline_PassandoDirettamenteIParametri(
                                         ByVal SitoOrigine As Enum_SiteRedirector,
                                         ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                         ByVal Piva As String,
                                          ByVal Cod_Contatto As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Rag_Soc As String,
                                          ByVal Sa_Nome As String,
                                          ByVal DataSelezionata As Date,
                                          ByVal Veg_Cod As Integer,
                                          ByVal Cul_Cod As Integer,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Operazione As Integer,
                                          ByVal Lavorazione As Integer,
                                          ByVal DPI_Cod As String,
                                          ByVal IdRcdpi As String,
                                          ByVal Mode As String,
                                          ByVal Causale As String,
                                          ByVal xChiave As String,
                                          ByVal ElemCod As String,
                                          ByVal Id_Reg As String,
                                          ByVal AmaxX As String,
                                          ByVal AmaxY As String,
                                          ByVal AminX As String,
                                          ByVal AminY As String,
                                          ByVal Xml_Generico As Text.StringBuilder) As String
        Dim Lingua As String = ""
        Dim SitoDestinazione As Enum_SiteRedirector = Prepara_ParametriGiasOnline_In_Sessione(
                                           Pagina_Richiesta,
                                           Piva,
                                           Cod_Contatto,
                                           Sa_Cod,
                                           Rag_Soc,
                                           Sa_Nome,
                                           DataSelezionata,
                                           Veg_Cod,
                                           Cul_Cod,
                                           Id_Agenda,
                                           Operazione,
                                           Lavorazione,
                                           DPI_Cod,
                                           IdRcdpi,
                                           Mode,
                                           Causale,
                                           xChiave,
                                           ElemCod,
                                           Id_Reg,
                                           AmaxX,
                                           AmaxY,
                                           AminX,
                                           AminY,
                                           Xml_Generico)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Return IndirizzoSito_ConQueryString
    End Function

    Private Shared Function Prepara_ParametriGiasOnline_In_Sessione(
                                          ByVal Pagina_Richiesta As enum_PagineGiasOnline_2010,
                                          ByVal Piva As String,
                                          ByVal Cod_Contatto As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Rag_Soc As String,
                                          ByVal Sa_Nome As String,
                                          ByVal DataSelezionata As Date,
                                          ByVal Veg_Cod As Integer,
                                          ByVal Cul_Cod As Integer,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Operazione As Integer,
                                          ByVal Lavorazione As Integer,
                                          ByVal DPI_Cod As String,
                                          ByVal IdRcdpi As String,
                                          ByVal Mode As String,
                                          ByVal Causale As String,
                                          ByVal xChiave As String,
                                          ByVal ElemCod As String,
                                          ByVal Id_Reg As String,
                                          ByVal AmaxX As String,
                                          ByVal AmaxY As String,
                                          ByVal AminX As String,
                                          ByVal AminY As String,
                                          ByVal Xml_Generico As Text.StringBuilder) As Enum_SiteRedirector
        If IsNothing(Xml_Generico) Then
            Xml_Generico = New Text.StringBuilder
        End If
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim sito As Enum_SiteRedirector

        Dim objGiasOnline As New ParametriGiasOnline
        objGiasOnline.PaginaRichiesta = Pagina_Richiesta
        objGiasOnline.Piva = Piva
        objGiasOnline.Cod_Contatto = Cod_Contatto
        objGiasOnline.Sa_Cod = Sa_Cod
        objGiasOnline.Rag_Soc = Rag_Soc
        objGiasOnline.Sa_Nome = Sa_Nome
        objGiasOnline.DataSelezionata = DataSelezionata
        objGiasOnline.Veg_Cod = Veg_Cod
        objGiasOnline.Cul_Cod = Cul_Cod
        objGiasOnline.Id_Agenda = Id_Agenda
        objGiasOnline.Operazione = Operazione
        objGiasOnline.Lavorazione = Lavorazione
        objGiasOnline.DPI_Cod = DPI_Cod
        objGiasOnline.IdRcdpi = IdRcdpi
        objGiasOnline.Mode = Mode
        objGiasOnline.Causale = Causale
        objGiasOnline.xChiave = xChiave
        objGiasOnline.ElemCod = ElemCod
        objGiasOnline.Id_Reg = Id_Reg
        objGiasOnline.AmaxX = AmaxX
        objGiasOnline.AmaxY = AmaxY
        objGiasOnline.AminX = AminX
        objGiasOnline.AminY = AminY
        objGiasOnline.Xml_Generico = Xml_Generico
        objGiasOnline.Salva()

        sito = Enum_SiteRedirector.Sito_GiasOnline
        Return sito
    End Function
    #End Region

    #Region "GENERICHE"
    Private Shared Function ApriIframe_SitoGenerico_ConParametriSitoInSessione(ByVal SitoOrigine As Enum_SiteRedirector, ByVal SitoDestinazione As Enum_SiteRedirector, ByVal Lingua As String) As String
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        Dim script As String = PreparaScripPerIframe(IndirizzoSito_ConQueryString)
        Return script
    End Function


    Private Shared Function ApriPopUp_SitoGenerico_ConParametriSitoInSessione(ByVal SitoOrigine As Enum_SiteRedirector, ByVal SitoDestinazione As Enum_SiteRedirector, ByVal Lingua As String, Optional ByVal JSon_Generico As String = "") As String
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua, JSon_Generico)
        Dim NomeFinestra As String = descrizioneFromSito(SitoDestinazione)
        Dim script As String = PreparaScripPerPopupFull(IndirizzoSito_ConQueryString, NomeFinestra)
        Return script
    End Function

    Private Shared Sub ApriNellaStessaPagina_ConRedirect_ConParametriSitoInSessione(
                                                 ByVal SitoOrigine As Enum_SiteRedirector,
                                                 ByVal SitoDestinazione As Enum_SiteRedirector,
                                                 ByRef objPage As System.Web.UI.Page,
                                                 ByVal Lingua As String)
        Dim IndirizzoSito_ConQueryString As String = PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(SitoOrigine, SitoDestinazione, Lingua)
        objPage.Response.Redirect(IndirizzoSito_ConQueryString)
    End Sub

    Public Shared Function PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(ByVal SitoOrigine As Enum_SiteRedirector,
                                                                                                                 ByVal SitoDestinazione As Enum_SiteRedirector,
                                                                                                                 ByVal Lingua As String,
                                                                                                                 Optional ByVal JSON_Generico As String = "",
                                                                                                                 Optional ByVal IDSezione As Integer = -1) As String
        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        'passo objScrivi per inserire le variabili sito specifico
        Dim IndirizzoSito As String = PreparaVariabiliEParametriSitoSuScriviXml_e_OttineniIndirizzo(SitoDestinazione, objScrivi)
        'passo objScrivi per salvare xml completo su db
        Dim IndirizzoSito_ConQueryString As String = SalvaParametriPassaggioSuDB_e_Allega_QueryString(IndirizzoSito, SitoOrigine, SitoDestinazione, Lingua, objScrivi, JSON_Generico, IDSezione)
        Return IndirizzoSito_ConQueryString
    End Function

    'ATTENZIONE: Questa funzione è la versione di PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString()
    'ma senza utilizzo della Session (e un nome meno complicato); inoltre ParametriAgenda_2010 viene passato ByRef perchè deve ritornarne
    'i valori aggiornati
    Public Shared Function PreparaPassaggioSito(ByVal SitoOrigine As Enum_SiteRedirector,
                                                 ByVal SitoDestinazione As Enum_SiteRedirector,
                                                 ByVal lingua As String,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                 ByRef objParametri_SuperServer As AgronicaCoreParametri,
                                                 Optional ByRef objParametri_Agenda2010 As ParametriAgenda_2010 = Nothing,
                                                 Optional ByVal objParametri_AuditPUA As ParametriAgronicaAuditPUA = Nothing,
                                                 Optional ByVal objParametri_Concimazione2017 As ParametriConcimazione_2017 = Nothing,
                                                 Optional ByRef objParametri_Analisi2010 As ParametriAnalisi_2010 = Nothing,
                                                 Optional ByVal JSON_Generico As String = "") As String
        Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml(False,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     objParametri_SuperServer)

        'Passo objScrivi per inserire le vars del sito specifico
        Dim IndirizzoSito As String = CaricaVarSito_GetIndirizzo(SitoDestinazione,
                                                                 objScrivi,
                                                                 objParametri_Agenda2010,
                                                                 objParametri_AuditPUA,
                                                                 objParametri_Concimazione2017,
                                                                 objParametri_Analisi2010)

        'Passo objScrivi per salvare XML completo su DB
        Dim url As String = SalvaParams_ReturnUrl_Indirizzo(IndirizzoSito,
                                                            SitoOrigine,
                                                            SitoDestinazione,
                                                            lingua,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            objParametri_SuperServer,
                                                            objScrivi,
                                                            JSON_Generico)
        Return url
    End Function

    Private Shared Function PreparaVariabiliEParametriSitoSuScriviXml_e_OttineniIndirizzo(ByVal SitoDestinazione As Enum_SiteRedirector,
                                                                                          ByRef objScrivi As AgronicaCoreGestioneRichieste.ScriviXml) As String
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        'viene caricato in automatico dal costruttore
        Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        'Scrittura XML

        objScrivi.AgroWebConfig = objWebConfig
        objScrivi.VariabiliSessione = objVarSess

        Dim hrefsito As String = ""

        Select Case SitoDestinazione
            Case Enum_SiteRedirector.Sito_GiasOnline
                Dim ParametriGiasOnline As New ParametriGiasOnline
                ParametriGiasOnline.Leggi()
                objScrivi.ParametriGiasOnline = ParametriGiasOnline
                hrefsito = objWebConfig.LinkGiasOnline

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                Dim ParametriAgenda_2010 As New ParametriAgenda_2010
                ParametriAgenda_2010.Leggi()
                objScrivi.ParametriAgenda_2010 = ParametriAgenda_2010
                hrefsito = objWebConfig.LinkAgronicaAgenda2010

                Dim ParametriAgronicaAuditPUA = New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                If ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010 <> "" Then
                    hrefsito = ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010
                End If

                Dim ParametriConcimazione_2017 As New ParametriConcimazione_2017
                ParametriConcimazione_2017.Leggi()
                objScrivi.ParametriConcimazione_2017 = ParametriConcimazione_2017
                'hrefsito = objWebConfig.LinkPianoConcimazione_2017

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
                Dim ParametriAnalisi_2010 As New ParametriAnalisi_2010
                ParametriAnalisi_2010.Leggi()
                objScrivi.ParametriAnalisi_2010 = ParametriAnalisi_2010
                hrefsito = objWebConfig.LinkAgronicaAnalisi_2010

            Case Enum_SiteRedirector.Sito_AgronicaBio
                Dim ParametriAgronicaBio As New ParametriAgronicaBio
                ParametriAgronicaBio.Leggi()
                objScrivi.ParametriAgronicaBio = ParametriAgronicaBio
                hrefsito = objWebConfig.LinkAgronicaBio

            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                Dim ParametriPianidiCampionamento_2010 As New AgronicaCoreGestioneRichieste.ParametriPianidiCampionamento_2010
                ParametriPianidiCampionamento_2010.Leggi()
                objScrivi.ParametriPianidiCampionamento_2010 = ParametriPianidiCampionamento_2010
                hrefsito = objWebConfig.LinkAgronicaPianiCampionamento

            Case Enum_SiteRedirector.Sito_AgronicaPianiSemina
                'NON HA PARAMETRI PECIFICI PER ORA
                'Dim ParametriPianiSemina As New AgronicaCoreGestioneRichieste.ParametriPianiSemina
                'ParametriPianiSemina.Leggi()
                'objScrivi.ParametriPianiSemina = ParametriPianiSemina
                hrefsito = objWebConfig.LinkAgronicaPianiSemina

            Case Enum_SiteRedirector.Sito_AgronicaPlanning
                Dim ParametriPlanning As New AgronicaCoreGestioneRichieste.ParametriPlanning
                ParametriPlanning.Leggi()
                objScrivi.ParametriPlanning = ParametriPlanning
                hrefsito = objWebConfig.LinkAgronicaPlanning

            Case Enum_SiteRedirector.Sito_AgronicaLabQualita
                Dim ParametriLabCQ As New AgronicaCoreGestioneRichieste.ParametriLabCQ
                ParametriLabCQ.Leggi()
                objScrivi.ParametriLabCQ = ParametriLabCQ
                hrefsito = objWebConfig.LinkAgronicaLabControlloQualita

            Case Enum_SiteRedirector.Sito_AgronicaSementi
                Dim ParametriSementieri As New AgronicaCoreGestioneRichieste.ParametriSementieri
                ParametriSementieri.Leggi()
                objScrivi.ParametriSementieri = ParametriSementieri
                hrefsito = objWebConfig.LinkSementieri

            Case Enum_SiteRedirector.Sito_AgronicaProfilazione
                Dim ParametriProfilazione_2010 As New AgronicaCoreGestioneRichieste.ParametriProfilazione_2010
                ParametriProfilazione_2010.Leggi()
                objScrivi.ParametriProfilazione_2010 = ParametriProfilazione_2010
                hrefsito = objWebConfig.LinkAgronicaProfilazione

            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore
                Dim Parametrisincronizzatore_2010 As New ParametriSincronizzatore_2010
                Parametrisincronizzatore_2010.Leggi()
                objScrivi.ParametriSincronizzatore_2010 = Parametrisincronizzatore_2010
                hrefsito = objWebConfig.LinkAgronicaSincronizzatore

            Case Enum_SiteRedirector.Sito_AgronicaStampe, Enum_SiteRedirector.Sito_AgronicaStampe_NewMode
                Dim ParametriAgronicaStampe As New ParametriAgronicaStampe
                ParametriAgronicaStampe.Leggi()
                objScrivi.ParametriAgronicaStampe = ParametriAgronicaStampe
                hrefsito = objWebConfig.LinkAgronicaStampe

            Case Enum_SiteRedirector.Sito_GiasOnline_2010
                Dim ParametriGiasOnline_2010 As New ParametriGiasOnline_2010
                ParametriGiasOnline_2010.Leggi()
                objScrivi.ParametriGiasOnline_2010 = ParametriGiasOnline_2010
                hrefsito = objWebConfig.LinkGiasOnline_2010

            Case Enum_SiteRedirector.Sito_AgronicaStampe_2010
                Dim ParametriAgronicaStampe_2010 As New ParametriAgronicaStampe_2010
                ParametriAgronicaStampe_2010.Leggi()
                objScrivi.ParametriAgronicaStampe_2010 = ParametriAgronicaStampe_2010
                hrefsito = objWebConfig.LinkAgronicaStampe_2010

            Case Enum_SiteRedirector.Sito_PianoConcimazione
                Dim ParametriAnalisiCosti_2010 As New ParametriAnalisiCosti_2010
                ParametriAnalisiCosti_2010.Leggi()
                objScrivi.ParametriAnalisiCosti_2010 = ParametriAnalisiCosti_2010
                hrefsito = objWebConfig.LinkPianoConcimazione


            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017
                Dim ParametriConcimazione_2017 As New ParametriConcimazione_2017
                ParametriConcimazione_2017.Leggi()
                objScrivi.ParametriConcimazione_2017 = ParametriConcimazione_2017
                hrefsito = objWebConfig.LinkPianoConcimazione_2017

            Case Enum_SiteRedirector.Sito_AgronicaPUA
                Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaPua

            Case Enum_SiteRedirector.Sito_AgronicaAudit
                Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaAudit

            Case Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
                Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaSicurezzaLavoro

            Case Enum_SiteRedirector.Sito_AgronicaCheckCOOP
                Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaCheckCOOP

            Case Enum_SiteRedirector.Sito_AgronicaGlobalGAP
                Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                ParametriAgronicaAuditPUA.Leggi()
                objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaGlobalGap

            Case Enum_SiteRedirector.Sito_AgronicaLabQualita
                'Dim ParametriAgronicaAuditPUA As New ParametriAgronicaAuditPUA
                'ParametriAgronicaAuditPUA.Leggi()
                'objScrivi.ParametriAgronicaAuditPUA = ParametriAgronicaAuditPUA
                hrefsito = objWebConfig.LinkAgronicaLabControlloQualita

            Case Enum_SiteRedirector.Sito_AgronicaManutenzione
                'NON GESTITO

            Case Enum_SiteRedirector.Sito_AgronicaMeteo
                'NON GESTITO

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi
                'NON GESTITO
            Case Enum_SiteRedirector.Sito_AgronicaView
                'NON GESTITO

            Case Enum_SiteRedirector.GiasNG
                Dim Parametri_ObjParametriAgenda_NG As New Parametri_ObjParametriAgenda_NG
                Parametri_ObjParametriAgenda_NG.Leggi()
                objScrivi.Parametri_ObjParametriAgenda_NG = Parametri_ObjParametriAgenda_NG
                hrefsito = objWebConfig.LinkAgronicaGiasNG

            Case Enum_SiteRedirector.Sito_AgronicaUma
                Dim Parametri_ObjParametriAgenda_2010 As New ParametriAgenda_2010
                Parametri_ObjParametriAgenda_2010.Leggi()
                objScrivi.ParametriAgenda_2010 = Parametri_ObjParametriAgenda_2010
                hrefsito = objWebConfig.LinkAgronicaUMA

            Case Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua
                Dim Parametri_ObjParametriDomandaIrrigua As New ParametriDomandaIrrigua
                Parametri_ObjParametriDomandaIrrigua.Leggi()
                objScrivi.ParametriDomandaIrrigua = Parametri_ObjParametriDomandaIrrigua

                hrefsito = objWebConfig.LinkAgronicaDomandaIrrigua

        End Select

        Return hrefsito
    End Function

    ''' <remarks>
    ''' ATTENZIONE: Questa funzione è la versione di PreparaVariabiliEParametriSitoSuScriviXml_e_OttineniIndirizzo()
    ''' ma senza utilizzo della Session (e un nome meno complicato); inoltre ParametriAgenda_2010 viene passato ByRef perchè deve ritornarne
    ''' i valori aggiornati
    ''' </remarks>
    Private Shared Function CaricaVarSito_GetIndirizzo(ByVal SitoDestinazione As Enum_SiteRedirector,
                                                       ByRef objScrivi As AgronicaCoreGestioneRichieste.ScriviXml,
                                                       ByRef ParametriAgenda_2010 As ParametriAgenda_2010,
                                                       ByVal ParametriAgronicaAuditPUA As ParametriAgronicaAuditPUA,
                                                       ByVal ParametriConcimazione_2017 As ParametriConcimazione_2017,
                                                       ByRef ParametriAnalisi_2010 As ParametriAnalisi_2010) As String
        Dim hrefSito As String = ""

        'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(False)
        ''viene caricato in automatico dal costruttore
        'Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione(False)
        ''Scrittura XML

        'objScrivi.AgroWebConfig = objWebConfig
        'objScrivi.VariabiliSessione = objVarSess

        'Per il momento sono gestiti solo questi casi di reindirizzamento;
        'nel caso aggiungerne e gestire anche gli obj di ritorno
        Select Case SitoDestinazione

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objScrivi.ParametriAgenda_2010 = ParametriAgenda_2010
                hrefSito = objScrivi.AgroWebConfig.LinkAgronicaAgenda2010

                If ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010 <> "" Then
                    hrefSito = ParametriAgronicaAuditPUA.LinkAgronicaAgenda2010
                End If

                objScrivi.ParametriConcimazione_2017 = ParametriConcimazione_2017
                'hrefsito = objScrivi.AgroWebConfig.LinkPianoConcimazione_2017

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
                objScrivi.ParametriAnalisi_2010 = ParametriAnalisi_2010
                hrefSito = objScrivi.AgroWebConfig.LinkAgronicaAnalisi_2010

            Case Else

        End Select

        Return hrefSito
    End Function

    Public Shared Function PreparaScripPerPopup(ByVal IndirizzoSito As String, ByVal NomeFinestra As String) As String
        Dim Script As String
        Script = "<script language='javascript'>" &
                    "           window.open('" & IndirizzoSito & "' ," &
                    "           '" & NomeFinestra & "'," &
                    "           'height=700," &
                    "           width=1000," &
                    "           menubar=yes," &
                    "           resizable=yes," &
                    "           scrollbars=yes," &
                    "           top=0,left=0');" &
                 " </script> "
        Return Script
    End Function

    Public Shared Function PreparaScripPerPopupBS(ByVal IndirizzoSito As String, ByVal NomeFinestra As String) As String
        Dim Script As String
        Script = "           window.open('" & IndirizzoSito & "' ," &
                 "           '" & NomeFinestra & "'," &
                 "           'height=700," &
                 "           width=1000," &
                 "           menubar=yes," &
                 "           resizable=yes," &
                 "           scrollbars=yes," &
                 "           top=0,left=0');"
        Return Script
    End Function

    Public Shared Function PreparaScripPerPopupFull(ByVal IndirizzoSito As String, ByVal NomeFinestra As String) As String
        'Dim Script As String
        'Script = "<script language='javascript'>" & _
        '            "           window.open('" & IndirizzoSito & "' ," & _
        '            "           '" & NomeFinestra & "'," & _
        '            "           'height=' + screen.availHeight + '," & _
        '            "           width=' + screen.availWidth  + '," & _
        '            "           menubar=yes," & _
        '            "           resizable=yes," & _
        '            "           scrollbars=yes," & _
        '            "           top=0,left=0');" & _
        '         " </script> "
        Dim Script As String
        Script = "<script language='javascript'>" &
                    "           var win = window.open('" & IndirizzoSito & "' ," &
                    "           '" & NomeFinestra & "');" &
                    "           win.focus(); " &
                 " </script> "
        Return Script
    End Function

    Public Shared Function PreparaScripPerIframe(ByVal IndirizzoSito As String) As String
        Dim Script As String
        Script = "<script>$(document).ready(function() {" &
                "            $('#LinkxIframe').fancybox({ " &
                "                'width': '105%'," &
                "                'height': '105%'," &
                "                'autoScale': true," &
                "                'transitionIn': 'none'," &
                "                'transitionOut': 'none'," &
                "                'type': 'iframe'," &
                "                'scrolling' :'yes' " &
                "    });" &
                "            $('#LinkxIframe').attr('href', '" & IndirizzoSito & "');" &
                "            $('#LinkxIframe').click(); " &
                "        });" &
                " </script> "
        Return Script
    End Function

    Private Shared Function SalvaParametriPassaggioSuDB_e_Allega_QueryString(
        IndirizzoSito As String, 
        SitoOrigine As Int32,
        SitoDestinazione As Int32,
        Lingua As String,
        ByRef objScrivi As AgronicaCoreGestioneRichieste.ScriviXml,
        Optional JSON_Generico As String = "",
        Optional IDSezione As Integer = -1
    ) As String
        Dim stringaLingua As String = If(Lingua <> "", "&ln=" & Lingua & "", "")
        Dim UnID As String = objScrivi.ScriviXml(SitoOrigine, SitoDestinazione, JSON_Generico)
        Dim cn As String = HttpContext.Current.Session("ASG_Connessione_Server")
        Dim objCodifica As New AgronicaCoreDataProvider.Sicurezza
        If SitoDestinazione <> Enum_SiteRedirector.GiasNG Then
            UnID = Stringa_Codifica(UnID, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))
        End If
        cn = Stringa_Codifica(cn, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))

        'modifica per superserver, occorre passare la stringa connessione
        'non essendoci piu il file ini e dovendo leggere
        'la connessione al server dalla tabella del superserver usando l'id numerico cn
        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            Dim StrConSup As String = ""
            'TODO: Verificare che nel sito di arrivo sia configurato il super server
            If Sicurezza.ExistStringaConnessione(Sicurezza.ID_DB_Super_Server) Then
                StrConSup = Sicurezza.ID_DB_Super_Server
            Else
                Dim ASG_objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
                StrConSup = ASG_objParametri_Super_Server.StringaConnessione
            End If
            StrConSup = Stringa_Codifica(StrConSup, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))

            Dim indirizzoCompleto As String
            If SitoDestinazione <> Enum_SiteRedirector.GiasNG Then
                indirizzoCompleto = IndirizzoSito & "?unid=" & UnID & "&cn=" & cn & "&StrConSup=" & StrConSup & "&FlagNew=1" & stringaLingua
                If IDSezione <> -1 Then
                    indirizzoCompleto &= "&idBC=" & Stringa_Codifica(IDSezione, AgroKey_EncoderDecoder)
                End If
            Else
                Dim objParametri_Super_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
                Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
                Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                'Dim LinkCoreWS = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
                'Dim LinkCoreWSB64 = AgroZip.CompressioneBase64(0, LinkCoreWS)
                Dim LinkGiasBaseWSB64 = AgroZip.CompressioneBase64(0, objParametri_Server.LinkGiasBase)

                Dim LinkCoreAPI = ConfSitiFromServerOrSuperServer("GiasOnline_Core_API", objParametri_Server, objParametri_Super_Server)
                Dim LinkNetCore = ConfSitiFromServerOrSuperServer("GiasOnline_NetCore_API", objParametri_Server, objParametri_Super_Server)
                Dim LinkQdCACompliance = ConfSitiFromServerOrSuperServer("GiasOnline_QdCACompliance_API", objParametri_Server, objParametri_Super_Server)

                'Dim LinkCoreAPINet6 = objConfSiti.Leggi_Valore(0, "GiasOnline_Core_API", "", "", objParametri_Server)
                'If LinkCoreAPINet6 = "" Then
                '    LinkCoreAPINet6 = objConfSiti.Leggi_Valore(0, "GiasOnline_Core_API", "", "", objParametri_Super_Server)
                'End If

                Dim LinkCoreAPIB64 = AgroZip.CompressioneBase64(0, LinkCoreAPI)
                Dim LinkNetCoreAPIB64 = AgroZip.CompressioneBase64(0, LinkNetCore)
                Dim LinkQdCAComplianceB64 = AgroZip.CompressioneBase64(0, LinkQdCACompliance)
                'Dim LinkCoreAPINet6B64 = AgroZip.CompressioneBase64(0, LinkCoreAPINet6)

                Dim timeZonInfoLocal = AgroZip.CompressioneBase64(0, TimeZoneInfo.Local.Id)
                If Not IsNothing(IndirizzoSito) AndAlso IndirizzoSito(IndirizzoSito.Length - 1) = "/" Then
                    IndirizzoSito = IndirizzoSito.Substring(0, IndirizzoSito.Length - 1)
                End If

                Dim connessioni As New AgronicaCoreVarieDAL.ConnessioneWS
                indirizzoCompleto = IndirizzoSito &
                    "?unid=" & UnID &
                    "&lapi=" & LinkCoreAPIB64 &
                    "&lgb=" & LinkGiasBaseWSB64 &
                    "&tz=" & timeZonInfoLocal &
                    "&lnca=" & LinkNetCoreAPIB64 &
                    "&lqdcc=" & LinkQdCAComplianceB64

            End If
            Return indirizzoCompleto

        Else
            Dim indirizzoCompleto As String
            indirizzoCompleto = IndirizzoSito & "?unid=" & UnID & "&cn=" & cn & "&FlagNew=1" & stringaLingua
            If IDSezione <> -1 Then
                indirizzoCompleto &= "&idBC=" & Stringa_Codifica(IDSezione, AgroKey_EncoderDecoder)
            End If
            Return indirizzoCompleto

        End If
    End Function

    ''' <summary>
    ''' Retrieves a configuration value from either server or super-server parameters
    ''' </summary>
    ''' <param name="key">Configuration key to retrieve</param>
    ''' <param name="objParametri_Server">Server parameters object</param>
    ''' <param name="objParametri_Super_Server">Super-server parameters object</param>
    ''' <returns>Configuration value from either server or super-server</returns>
    Private shared Function ConfSitiFromServerOrSuperServer(key As string, objParametri_Server As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri)
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim value = objConfSiti.Leggi_Valore(0, key, "", "", objParametri_Server)
        If value = "" Then
            value = objConfSiti.Leggi_Valore(0, key, "", "", objParametri_Super_Server)
        End If
        Return value
    End Function

    ''' <remarks>
    ''' ATTENZIONE: Questa funzione è la versione di SalvaParametriPassaggioSuDB_e_Allega_QueryString()
    ''' ma senza utilizzo della Session (e un nome meno complicato)
    ''' </remarks>
    Private Shared Function SalvaParams_ReturnUrl_Indirizzo(ByVal IndirizzoSito As String,
                                                            ByVal SitoOrigine As Integer,
                                                            ByVal SitoDestinazione As Integer,
                                                            ByVal Lingua As String,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                            ByRef objParametri_SuperServer As AgronicaCoreParametri,
                                                            ByRef objScrivi As AgronicaCoreGestioneRichieste.ScriviXml,
                                                            Optional ByVal JSON_Generico As String = "") As String
        Dim IndirizzoCompleto As String
        Dim cn As String = ""
        Dim stringaLingua As String = ""
        If Lingua <> "" Then
            stringaLingua = "&ln=" & Lingua & ""
        End If
        Dim UnID As String = objScrivi.ScriviXml_NoSession(SitoOrigine,
                                                           SitoDestinazione,
                                                           objParametri_Server,
                                                           JSON_Generico)
        Dim strConn As String = objParametri_Server.StringaConnessione
        Dim objCodifica As New AgronicaCoreDataProvider.Sicurezza
        If SitoDestinazione <> Enum_SiteRedirector.GiasNG Then
            UnID = Stringa_Codifica(UnID, AgroKey_EncoderDecoder, objParametri_Server)
        End If
        cn = Stringa_Codifica(cn, AgroKey_EncoderDecoder, objParametri_Server)

        'modifica per superserver, occorre passare la stringa connessione
        'non essendoci piu il file ini e dovendo leggere
        'la connessione al server dalla tabella del superserver usando l'id numerico cn
        If Not IsNothing(objParametri_SuperServer) Then
            Dim StrConSup As String = ""
            'TODO: Verificare che nel sito di arrivo sia configurato il super server
            If Sicurezza.ExistStringaConnessione(Sicurezza.ID_DB_Super_Server) Then
                StrConSup = Sicurezza.ID_DB_Super_Server
            Else
                StrConSup = objParametri_SuperServer.StringaConnessione
            End If
            StrConSup = Stringa_Codifica(StrConSup, AgroKey_EncoderDecoder, objParametri_Server)

            If SitoDestinazione <> Enum_SiteRedirector.GiasNG Then
                IndirizzoCompleto = IndirizzoSito & "?unid=" & UnID & "&cn=" & cn & "&StrConSup=" & StrConSup & "&FlagNew=1" & stringaLingua

            Else
                Dim objParametri_Server_String As String = Utility.convertOBJparametritoString(objParametri_Server)
                Dim objParametri_Utenti_String As String = Utility.convertOBJparametritoString(objParametri_Utenti)
                Dim objparametri_SuperServer_String As String = Utility.convertOBJparametritoString(objParametri_SuperServer)

                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim LinkCoreWS = objConfSiti.Leggi_Valore(0,
                                                          "GiasOnline_WS_Core_AgroWS_Core",
                                                          "",
                                                          "",
                                                          objParametri_Server)

                Dim LinkCoreWSB64 = AgroZip.CompressioneBase64(0, LinkCoreWS)

                Dim LinkCoreAPI = objConfSiti.Leggi_Valore(0,
                                                           "GiasOnline_Core_API",
                                                           "",
                                                           "",
                                                           objParametri_Server)

                If LinkCoreAPI = "" Then
                    LinkCoreAPI = objConfSiti.Leggi_Valore(0,
                                                           "GiasOnline_Core_API",
                                                           "",
                                                           "",
                                                           objParametri_SuperServer)
                End If

                Dim LinkCoreAPIB64 = AgroZip.CompressioneBase64(0, LinkCoreAPI)

                If IndirizzoSito(IndirizzoSito.Length - 1) = "/" Then
                    IndirizzoSito = IndirizzoSito.Substring(0, IndirizzoSito.Length - 1)

                End If

                'indirizzoCompleto = IndirizzoSito & "/" &
                '    objparametri_super_server_string & "/" &
                '    objParametri_server_String & "/" &
                '    objParametri_utenti_String & "/" &
                '    UnID & "/" &
                '    LinkCoreWSB64

                Dim connessioni As New AgronicaCoreVarieDAL.ConnessioneWS

                IndirizzoCompleto = IndirizzoSito _
                                    & "?objss=" & objparametri_SuperServer_String _
                                    & "&objs=" & objParametri_Server_String _
                                    & "&obju=" & objParametri_Utenti_String _
                                    & "&unid=" & UnID _
                                    & "&lcore=" & LinkCoreWSB64 _
                                    & "&lapi=" & LinkCoreAPIB64

                'indirizzoCompleto = IndirizzoSito & "/" &
                '    objparametri_super_server_string & "/" &
                '    objParametri_server_String & "/" &
                '    objParametri_utenti_String & "/" &
                '    UnID

            End If

        Else
            IndirizzoCompleto = IndirizzoSito & "?unid=" & UnID & "&cn=" & cn & "&FlagNew=1" & stringaLingua

        End If

        Return IndirizzoCompleto
    End Function

    Private Shared Function descrizioneFromSito(ByVal SitoDestinazione As Enum_SiteRedirector) As String
        Dim nome As String = "Agronica"
        Select Case SitoDestinazione
            Case Enum_SiteRedirector.Sito_GiasOnline
                nome = "AgronicaGiasOnLine"

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                nome = "AgronicaAgenda"

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi
                nome = "AgronicaAnalisi"

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
                nome = "AgronicaAnalisi"

            Case Enum_SiteRedirector.Sito_AgronicaAudit
                nome = "AgronicaAUDIT"

            Case Enum_SiteRedirector.Sito_AgronicaBio
                nome = "AgronicaBIO"

            Case Enum_SiteRedirector.Sito_AgronicaCheckCOOP
                nome = "AgronicaCheckilstCOOP"

            Case Enum_SiteRedirector.Sito_AgronicaGlobalGAP
                nome = "AgronicaCheckilstGlobalGAP"

            Case Enum_SiteRedirector.Sito_AgronicaManutenzione
                nome = "AgronicaManutenzione"

            Case Enum_SiteRedirector.Sito_AgronicaMeteo
                nome = "AgronicaMeteo"

            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                nome = "AgronicaPianiCampionamento"

            Case Enum_SiteRedirector.Sito_AgronicaPianiSemina
                nome = "AgronicaPianiSemina"

            Case Enum_SiteRedirector.Sito_AgronicaPlanning
                nome = "AgronicaPlanning"

            Case Enum_SiteRedirector.Sito_AgronicaProfilazione
                nome = "AgronicaProfilazione"

            Case Enum_SiteRedirector.Sito_AgronicaPUA
                nome = "AgronicaPUA"

            Case Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
                nome = "AgronicaSicurezzaLavoro"

            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore
                nome = "AgronicaSincronizzatore"

            Case Enum_SiteRedirector.Sito_AgronicaStampe, Enum_SiteRedirector.Sito_AgronicaStampe_NewMode
                nome = "AgronicaStampe"

            Case Enum_SiteRedirector.Sito_AgronicaView
                nome = "AgronicaView"

            Case Enum_SiteRedirector.Sito_GiasOnline_2010
                nome = "AgronicaGiasSMART"

            Case Enum_SiteRedirector.Sito_PianoConcimazione
                nome = "AgronicaPianoConcimazione"

            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017
                nome = "AgronicaPianoConcimazione_2017"

            Case Enum_SiteRedirector.Sito_AgronicaStampe_2010
                nome = "AgronicaStampe"

            Case Enum_SiteRedirector.Sito_AgronicaLabQualita
                nome = "AgronicaLabControlloQualita"

        End Select
        'non usare i trattini o non apre con ie la pagina
        Return nome.Replace(" ", "").Replace("-", "")
    End Function


    Public Shared Function ControllaNumeroImprese_per_SiteRedirector(
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    ) As String
        Dim Piva As String = ""
        Dim Rag_Soc As String = ""
        Dim Num_Imprese As Integer = 0
        Dim i As Integer

        'leggo il numero di imprese sotto il super user
        Dim obj As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Num_Imprese = obj.Numero_Imprese_from_SuperUser(objParametri_Server.PivaSuperUser, Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", objParametri_Server)

        if Num_Imprese = 1 then'c'è una sola impresa
            Return Piva
        Else
            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim UtenteFiltro As String = ""
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim DtImpreseVisibili As DataTable
            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

            If DtImpreseVisibili IsNot Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteFiltro &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteFiltro <> "" Then
                    UtenteFiltro = " Imprese.piva IN (" & Left(UtenteFiltro, UtenteFiltro.Length - 1) & ") "
                End If
            End If

            'ci sono più imprese
            Dim ClassJoin As New JoinFiltrone
            If UtenteFiltro <> "" Then
                Dim objutil As New AgronicaCoreUtility.Filtrone
                objutil.ImpostaVariabiliJOIN_xFiltroUtente(UtenteFiltro, ClassJoin)

                'Per il momento imposto sempre a true perché può capitare
                'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
                '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
                'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
                ClassJoin.bGerarchiaImprese = True

                '------------------------------------------------------------------------
                '----- Filtro gli elementi usando il filtro associato all'utente 
                '------------------------------------------------------------------------
                Dim dtImprese = objutil.CreaDTFiltrone(objParametri_Server, UtenteFiltro,
                                                        enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                        "", ClassJoin)

                If dtImprese.Rows.Count = 1 Then
                    Piva = dtImprese.Rows(0).Item("Piva")
                    Rag_Soc = dtImprese.Rows(0).Item("Rag_Soc")
                    'Salvo la Partita IVA nella variabile di sessione
                    Return Piva
                End If
            End If
        End if

        Return ""
    End Function

    Public Shared Function GetLinkFiltrinoOnline(ByVal StringaSitoOrigine As String,
                                          ByVal StringaSitoDestinazione As String,
                                          ByVal SitoRichiesto As Integer,
                                          ByVal PaginaSitoRichiesta As Integer) As String
        Dim TargetURL As String
        Dim Origine As String
        Dim Destinazione As String
        Dim Sito As String
        Dim Pagina As String

        'Costruisco il link
        Origine = Stringa_Codifica(StringaSitoOrigine, AgroKey_EncoderDecoder)
        Destinazione = Stringa_Codifica(StringaSitoDestinazione, AgroKey_EncoderDecoder)
        Sito = Stringa_Codifica(SitoRichiesto, AgroKey_EncoderDecoder)
        Pagina = Stringa_Codifica(PaginaSitoRichiesta, AgroKey_EncoderDecoder)
        TargetURL = "../Utility/Filtrino.aspx" &
                      "?o=" & Origine &
                      "&d=" & Destinazione &
                      "&sito=" & Sito &
                      "&pagina=" & Pagina
        Return TargetURL
    End Function

    Public Shared Function GetLinkFiltrinoAgenda(ByVal StringaSitoOrigine As String,
                                          ByVal StringaSitoDestinazione As String,
                                          ByVal SitoRichiesto As Integer,
                                          ByVal PaginaSitoRichiesta As Integer) As String
        Dim TargetURL As String
        Dim Origine As String
        Dim Destinazione As String
        Dim Sito As String
        Dim Pagina As String

        'Costruisco il link
        Origine = Stringa_Codifica(StringaSitoOrigine, AgroKey_EncoderDecoder)
        Destinazione = Stringa_Codifica(StringaSitoDestinazione, AgroKey_EncoderDecoder)
        Sito = Stringa_Codifica(SitoRichiesto, AgroKey_EncoderDecoder)
        Pagina = Stringa_Codifica(PaginaSitoRichiesta, AgroKey_EncoderDecoder)

        TargetURL = "../Filtrino/FiltrinoImprese.aspx" &
                      "?o=" & Origine &
                      "&d=" & Destinazione &
                      "&sito=" & Sito &
                      "&pagina=" & Pagina

        Return TargetURL
    End Function

    Public Shared Function GetLinkGestioneAllegati(ByVal SitoOrigine As Enum_SiteRedirector,
                                                   ByVal Piva As String,
                                                   ByVal Cod_Contatto As String,
                                                   ByVal Tipologia As Integer,
                                                   Optional ByVal Analisi_Testata_Cod As Integer = 0,
                                                   Optional ByVal Data_Scadenza As Date = AGRODATAFINE,
                                                   Optional ByVal Data_Rilascio As Date = AGRODATAINIZIO) As String
        Dim Area As Integer = If(Cod_Contatto <> "", enum_ID_Area_Alert.Contatti, 0)
        Dim TargetURL As String

        If SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
            TargetURL = "../GestioneAllegati/GestioneAllegati.aspx" &
                        "?piva=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder) &
                        "&a=" & Stringa_Codifica(Area, AgroKey_EncoderDecoder) &
                        "&t=" & Stringa_Codifica(Tipologia, AgroKey_EncoderDecoder) &
                        "&cod=" & Stringa_Codifica(Analisi_Testata_Cod, AgroKey_EncoderDecoder) &
                        "&c_contatto=" & Stringa_Codifica(Cod_Contatto, AgroKey_EncoderDecoder) &
                        "&ds=" & Stringa_Codifica(Data_Scadenza.ToShortDateString, AgroKey_EncoderDecoder) &
                        "&dr=" & Stringa_Codifica(Data_Rilascio.ToShortDateString, AgroKey_EncoderDecoder)

        Else
            Dim ParametriScadenziario As New AgronicaCoreGestioneRichieste.ParametriScadenziario
            ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_GestioneAllegati
            ParametriScadenziario.Piva = Piva
            ParametriScadenziario.Id_Area = Area
            ParametriScadenziario.Id_Tipologia = Tipologia
            ParametriScadenziario.Cod_Contatto = Cod_Contatto
            ParametriScadenziario.Analisi_Testata_Cod = Analisi_Testata_Cod
            ParametriScadenziario.Data_Scadenza = Data_Scadenza

            TargetURL = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriScadenziario(SitoOrigine, ParametriScadenziario)

        End If

        Return TargetURL
    End Function

    Public Shared Function GetLinkPassaggioDiStato(ByVal SitoOrigine As Enum_SiteRedirector,
                                                   ByVal Pratica_Cod As String,
                                                   ByVal Passaggio_Di_Stato_Cod As Integer) As String
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim gestione_servizi_agenda = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Gestione_Servizi_NEW,
                                             enum_Security_Operazione.Modifica,
                                             Date.Now,
                                             "",
                                             HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim gestione_servizi_profilazione = False
        If Not gestione_servizi_agenda Then
            gestione_servizi_profilazione = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Gestione_Servizi,
                                             enum_Security_Operazione.Modifica,
                                             Date.Now,
                                             "",
                                             HttpContext.Current.Session("ASG_objParametri_Utenti"))
        End If

        If (gestione_servizi_agenda OrElse gestione_servizi_profilazione) AndAlso (SitoOrigine = Enum_SiteRedirector.Sito_AgronicaProfilazione OrElse SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010) Then
            Return "../Servizi/PassaggioDiStato.aspx" &
                        "?pratica=" & Pratica_Cod &
                        "&passaggiodistato=" & Passaggio_Di_Stato_Cod
        End If

        If gestione_servizi_agenda Then
            Dim ParametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010 With {
                .PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Passaggio_Stato,
                .QueryStringFiltrino = "?pratica=" & Pratica_Cod & "&passaggiodistato=" & Passaggio_Di_Stato_Cod
            }

            Return IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(SitoOrigine, ParametriAgenda)
        ElseIf gestione_servizi_profilazione Then
            Dim ParametriProfilatore As New ParametriProfilazione_2010 With {
                .Pagina_Richiesta = enum_PagineProfilazione_2010.Pagina_PassaggioDiStato,
                .Stringa_Parametri = "pratica=" & Pratica_Cod & "&passaggiodistato=" & Passaggio_Di_Stato_Cod
            }

            Return IndirizzoCompleto_SitoProfilazione_PassandoDirettamente_ParametriProfilazione_2010(SitoOrigine, ParametriProfilatore)
        Else
            Return "../Custom500.aspx?UtenteAbilitato_Lettura=0"
        End If

        'If SitoOrigine = Enum_SiteRedirector.Sito_AgronicaProfilazione Then
        '    TargetURL = "../Servizi/PassaggioDiStato.aspx" &
        '                "?pratica=" & Pratica_Cod &
        '                "&passaggiodistato=" & Passaggio_Di_Stato_Cod
        'Else
        '    Dim ParametriProfilatore As New AgronicaCoreGestioneRichieste.ParametriProfilazione_2010
        '    ParametriProfilatore.Pagina_Richiesta = enum_PagineProfilazione_2010.Pagina_PassaggioDiStato
        '    ParametriProfilatore.Stringa_Parametri = "pratica=" & Pratica_Cod & "&passaggiodistato=" & Passaggio_Di_Stato_Cod
        '    TargetURL = IndirizzoCompleto_SitoProfilazione_PassandoDirettamente_ParametriProfilazione_2010(SitoOrigine, ParametriProfilatore)
        'End If
        'Return TargetURL
    End Function
    #End Region

    ''' <summary>
    ''' Restituisce il link per effettuare il redirec verso il filtrino imprese o il filtrone ng, dopo aver letto il valore dell'impostazione
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="sitoOrigine"></param>
    ''' <param name="paginaProvenienza"></param>
    ''' <param name="sitoDestinazioneDopoIlRedirect"></param>
    ''' <param name="paginaDestinazioneDopoIlRedirect"></param>
    ''' <param name="link"></param>
    ''' <param name="queryString"></param>
    ''' <param name="chiave"></param>
    ''' <returns></returns>
    Public Shared Sub GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(ByVal piva As String,
                                                                        ByVal sitoOrigine As Enum_SiteRedirector,
                                                                        ByVal paginaProvenienza As Integer,
                                                                        ByVal sitoDestinazioneDopoIlRedirect As Enum_SiteRedirector,
                                                                        ByVal paginaDestinazioneDopoIlRedirect As Integer,
                                                                        ByRef link As String)

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim handleImpostazioniSuperuser As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim impostazionePagRicerca = handleImpostazioniSuperuser.ImpostazioneValore1_from_ImpostazioneCod(
            enum_Impostazioni_Utenti.SUPERUSER_Mod_Ricerca_Impresa, objParametri_Utenti, 2)

        Select Case impostazionePagRicerca
            Case "1"
                Dim objFiltrino As New ParametriAgenda_2010
                objFiltrino.SitoDestinazioneFiltrino = CInt(sitoDestinazioneDopoIlRedirect)
                objFiltrino.PaginaDestinazioneFiltrino = paginaDestinazioneDopoIlRedirect
                objFiltrino.PaginaProvenienza = paginaProvenienza
                objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                link = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(sitoOrigine,
                               objFiltrino)
            Case Else
                Dim rowsModalitaFiltroRicerca = handleImpostazioniSuperuser.LeggiImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_Mod_Filtro_Ricerca, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                Dim modalitaFiltroRicerca = rowsModalitaFiltroRicerca _
                    .Where(Function(item) Not IsDBNull(item("Impostazione_Valore_1"))) _
                    .Select(Function(item) item("Impostazione_Valore_1").ToString()) _
                    .FirstOrDefault()

                Select Case modalitaFiltroRicerca
                    Case "1"
                        Dim parametriAgenda As New ParametriAgenda_2010
                        parametriAgenda.QueryStringFiltrino = "?s_o=" & Stringa_Codifica(sitoOrigine, AgroKey_EncoderDecoder) &
                            "&p_o=" & Stringa_Codifica(paginaProvenienza, AgroKey_EncoderDecoder) &
                            "&s_d=" & Stringa_Codifica(sitoDestinazioneDopoIlRedirect, AgroKey_EncoderDecoder) &
                            "&p_d=" & Stringa_Codifica(paginaDestinazioneDopoIlRedirect, AgroKey_EncoderDecoder) &
                            "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.OperazioniMultiAziendali, AgroKey_EncoderDecoder) &
                            "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                            "&cat=" & Stringa_Codifica("azienda", AgroKey_EncoderDecoder) &
                            "&f=" & Stringa_Codifica("1", AgroKey_EncoderDecoder)

                        parametriAgenda.PaginaProvenienza = paginaProvenienza
                        parametriAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Filtrone

                        link = IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(sitoOrigine, parametriAgenda)
                    Case Else
                        Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                            .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Aziende},
                            .SitoDestinazioneDopoIlRedirect = CInt(sitoDestinazioneDopoIlRedirect),
                            .PaginaDestinazioneDopoIlRedirect = paginaDestinazioneDopoIlRedirect,
                            .PaginaProvenienza = paginaProvenienza,
                            .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda,
                            .SitoOrigine = sitoOrigine
                        }

                        Dim parametriFiltroRicercaNGAsJson = JsonConvert.SerializeObject(parametriFiltroRicercaNG)
                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("GenericObj_string") = parametriFiltroRicercaNGAsJson

                        MenuBS_2017_RedirectGestione.RedirectGenerico(piva,
                        Enum_SiteRedirector.GiasNG,
                        enum_PagineGiasNG.Pagina_Filtro_Ricerca,
                        link,
                        objParametri_Server,
                        Parametri_Aggiuntivi,
                        sitoOrigine)
                End Select
        End Select
    End Sub

End Class
