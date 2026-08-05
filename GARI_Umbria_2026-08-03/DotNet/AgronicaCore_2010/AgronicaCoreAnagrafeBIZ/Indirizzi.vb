Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.exceptions

Public Class Indirizzi_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function CodiceAslDatoContatto(ByVal piva As String,
                                          ByVal cod_contatto As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Indirizzi_R.CodiceAslDatoContatto(()"

        Dim contatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
        'LeggiCodiceAuslDatoContatto
        Dim dtContatto = contatti_R.Leggi(piva, cod_contatto, 0, 0, True, True, 0, 0, False, 0, -99, 0, "", False, False, False, False, False, False, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri)
        Try
            If dtContatto.Rows.Count > 0 Then
                Dim dtIndirizziValidi = dtContatto.Select(" pro_cod_istat <> '000' AND com_cod_istat <> '000' AND pro_cod_istat <> '' AND com_cod_istat <> '' ").CopyToDataTable
                If dtIndirizziValidi.Rows.Count > 0 Then
                    Return CodiceAslDatoCodIndirizzo(dtIndirizziValidi.Rows(0)("Cod_Indirizzo"), objParametri)
                End If
            Else
                Dim impresexindirizzi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                Dim dtIndirizzi = impresexindirizzi.Leggi(cod_contatto, 0, 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)
                Dim dtIndirizziValidi = dtIndirizzi.Select(" pro_cod_istat <> '000' AND com_cod_istat <> '000' AND pro_cod_istat <> '' AND com_cod_istat <> '' ").CopyToDataTable
                If dtIndirizziValidi.Rows.Count > 0 Then
                    Return CodiceAslDatoCodIndirizzo(dtIndirizziValidi.Rows(0)("Cod_Indirizzo"), objParametri)
                End If
            End If
        Catch ex As Exception

        End Try

        Return ""
    End Function

    Public Function CodiceAslDatoCodIndirizzo(ByVal codice As Integer,
                                                ByRef objParametri As AgronicaCoreParametri) As String


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Indirizzi_R.CodiceAuslDatoCodIndirizzo(()"

        Dim indirizzi_R As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
        'LeggiCodiceAuslDatoContatto

        Dim DT = indirizzi_R.LeggiCodiceAslDatoCodIndirizzo(codice, objParametri)

        If (DT.Rows.Count = 0) Then
            Throw New Exception("Nessun Codice Asl con codice " & codice & " trovato")
        End If

        Return DT.Rows(0)("codice")
    End Function

    Public Function Leggi_Indirizzo(codice As Integer,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Indirizzo
        Dim ind As New AgronicaCoreModelsSTD.anagrafiche.Indirizzo(codice)
        Dim indirizzi_R As New AgronicaCoreAnagrafeDAL.Indirizzi_Read

        Dim DT = indirizzi_R.Leggi(codice, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If (DT.Rows.Count = 0) Then
            Throw New Exception("Indirizzo con codice " & codice & " non trovato nella tabella Indirizzi")
        End If

        ind.codice = DT.Rows(0)("cod_indirizzo")
        ind.cap = DT.Rows(0)("CAP")
        ind.frazione = If(DT.Rows(0)("frz_des") IsNot DBNull.Value, DT.Rows(0)("frz_des"), "")
        ind.note = If(DT.Rows(0)("note") IsNot DBNull.Value, DT.Rows(0)("note"), "")
        ind.via = If(DT.Rows(0)("ind_des") IsNot DBNull.Value, DT.Rows(0)("ind_des"), "")
        Dim stato_cod = CStr(DT.Rows(0)("stato"))
        If New List(Of String)({"", "IT", "ITA", "ITALIA", "italia", "Italia"}).Contains(stato_cod) Then
            Dim stato As New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166("IT")
            stato.descrizione = "Italia"
            ind.stato = stato
            Dim readStati As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            Dim newstato = readStati.Leggi("IT", "", "", objParametri_Server)
            If newstato.Rows.Count > 0 Then
                ind.stato.gestioneGerarchia = newstato.Rows(0)("Gestione_Gerarchia_Geografica")
            End If
        Else
            Dim objStati_R As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            Dim dt_stato = objStati_R.Leggi(stato_cod, "", "", objParametri_Server)
            Dim stato As New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166(stato_cod)
            stato.descrizione = ""
            If dt_stato.Rows.Count > 0 Then
                stato.descrizione = dt_stato.Rows(0)("Descrizione")
            End If
            ind.stato = stato
            Dim readStati As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            Dim newstato = readStati.Leggi(stato_cod, "", "", objParametri_Server)
            If newstato.Rows.Count > 0 Then
                ind.stato.gestioneGerarchia = newstato.Rows(0)("Gestione_Gerarchia_Geografica")
            End If
        End If
        Dim istat As New AgronicaCoreModelsSTD.metaschema.Istat
        istat.cap = DT.Rows(0)("CAP")
        istat.codiceBelfiore = ""
        istat.prov = If(DT.Rows(0)("pro_cod_istat") IsNot DBNull.Value, DT.Rows(0)("pro_cod_istat"), "")
        istat.com = If(DT.Rows(0)("com_cod_istat") IsNot DBNull.Value, DT.Rows(0)("com_cod_istat"), "")
        istat.comuni_prov = If(DT.Rows(0)("pro_des") IsNot DBNull.Value, DT.Rows(0)("pro_des"), "")
        istat.localita = If(DT.Rows(0)("com_des") IsNot DBNull.Value, DT.Rows(0)("com_des"), "")
        istat.reg = DT.Rows(0)("reg")

        ind.istatComune = istat

        Return ind
    End Function

    Private Function Leggi_Indirizzi_Associati(Piva As String,
                                               Sa_Cod As Integer,
                                               Appezza As Integer,
                                               Cod_Contatto As String,
                                               Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                                    Optional Id_Budget As Integer = 0,
                                                    Optional DescrizioniComplete As Boolean = False) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)

        Dim ind_ass_list As New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        Dim dt As DataTable
        Select Case Elemento_Anagrafico
            Case enum_EntitaAlberoImprese.Impresa
                Dim indirizzixImprese As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                dt = indirizzixImprese.Leggi(Piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Centro
                Dim centrixIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                dt = centrixIndirizzi.Leggi(Piva, Sa_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Contatto
                Dim contattixIndirizzi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
                dt = contattixIndirizzi.Leggi(Piva, Cod_Contatto, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Case enum_EntitaAlberoImprese.Appezzamento
                If Id_Budget <> 0 Then
                    Dim appezzamentixIndirizzi As New AgronicaCoreBudgetDAL.Budget_AppezzamentixIndirizzi_R
                    dt = appezzamentixIndirizzi.LeggixAppezzamento(Id_Budget, Piva, Sa_Cod, Appezza, 0, "", "", objParametri_Server)
                Else
                    Dim appezzamentixIndirizzi As New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read
                    dt = appezzamentixIndirizzi.Leggi(Piva, Sa_Cod, Appezza, 0, "", "", objParametri_Server)
                End If
            Case Else
                Throw New Exception("Lettura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non gestita. ")
        End Select

        If dt IsNot Nothing Then
            If (dt.Rows.Count = 0) Then
                Return ind_ass_list
            End If
            For Each row In dt.Rows
                Dim ind_ass As New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato
                ind_ass.tipo_Indirizzo = row("Tipo_Indirizzo")
                ind_ass.indirizzo = Leggi_Indirizzo(row("Cod_Indirizzo"), objParametri_Server)

                If DescrizioniComplete Then
                    Dim objIndirizzi As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
                    Dim dtIndirizzoCompleto = objIndirizzi.Leggi_Indirizzo_Completo(row("cod_indirizzo"), objParametri_Server)
                    If dtIndirizzoCompleto.Rows.Count > 0 Then
                        Dim IndirizzoCompleto = dtIndirizzoCompleto.Rows(0)
                        ind_ass.indirizzo.via = IndirizzoCompleto("Indirizzo")
                        ind_ass.indirizzo.frazione = IndirizzoCompleto("Frazione")
                        ind_ass.indirizzo.cap = IndirizzoCompleto("CAP")
                        ind_ass.indirizzo.stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166 With {.descrizione = IndirizzoCompleto("Stato")}
                        ind_ass.indirizzo.istatComune = New AgronicaCoreModelsSTD.metaschema.Istat With {.localita = IndirizzoCompleto("Comune"), .comuni_prov = IndirizzoCompleto("Provincia")}
                    End If
                End If

                ind_ass_list.Add(ind_ass)
            Next
        End If

        Return ind_ass_list
    End Function

    Public Function Leggi_Indirizzi_Associati_Impresa(Piva As String,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        Return Leggi_Indirizzi_Associati(Piva, 0, 0, "", enum_EntitaAlberoImprese.Impresa, objParametri_Server)
    End Function

    Public Function Leggi_Indirizzi_Associati_Contatto(Piva As String,
                                                       Cod_Contatto As String,
                                                       ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        Return Leggi_Indirizzi_Associati(Piva, 0, 0, Cod_Contatto, enum_EntitaAlberoImprese.Contatto, objParametri_Server)
    End Function

    Public Function Leggi_Indirizzi_Associati_Centro(Piva As String,
                                                     Sa_Cod As Integer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     Optional DescrizioniComplete As Boolean = False) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        Return Leggi_Indirizzi_Associati(Piva, Sa_Cod, 0, "", enum_EntitaAlberoImprese.Centro, objParametri_Server, DescrizioniComplete:=DescrizioniComplete)
    End Function

    Public Function Leggi_Indirizzi_Associati_Appezzamento(Piva As String,
                                                           Sa_Cod As Integer,
                                                           Appezza As Integer,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                                Optional Id_Budget As Integer = 0) As List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        Return Leggi_Indirizzi_Associati(Piva, Sa_Cod, Appezza, "", enum_EntitaAlberoImprese.Appezzamento, objParametri_Server, Id_Budget)
    End Function
End Class

'################################################################################################################################################################################
'################################################################################################################################################################################
'################################################################################################################################################################################

Public Class Indirizzi_W
    Inherits AgronicaCoreDataProvider.LogProvider

    Private Function Scrivi_Indirizzi_Associati(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                                ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                                Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                Optional ByVal NewTransaction As Boolean = True)

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Indirizzi_W.Scrivi_Indirizzi_Associati()"
        Dim messaggioErrore As String = ""

        Dim Cod_Indirizzo As Integer 'corrisponde al codice nell'oggetto Indirizzo
        Dim indirizzoEF As AgronicaCoreEntityFramework_POCO.Indirizzi

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)

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
            CapValidator(indirizzoAssociato) ' check if the CAP has valid format

            If indirizzoAssociato.indirizzo.codice = 0 Then

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa
                        indirizzoEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateIndirizzoImpresa(indirizzoAssociato.indirizzo, GiasContext, objParametri_Server, Impresa, indirizzoAssociato.tipo_Indirizzo, objParametri_Utenti.UsernameOperazione)
                    Case enum_EntitaAlberoImprese.Centro
                        indirizzoEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateIndirizzoCentro(indirizzoAssociato.indirizzo, GiasContext, objParametri_Server, Centro, indirizzoAssociato.tipo_Indirizzo, objParametri_Utenti.UsernameOperazione)
                    Case enum_EntitaAlberoImprese.Contatto
                        indirizzoEF = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(indirizzoAssociato.indirizzo, GiasContext, objParametri_Server, Contatto.Piva, Contatto.Sa_Cod, Contatto, indirizzoAssociato.tipo_Indirizzo, objParametri_Utenti.UsernameOperazione)
                    Case enum_EntitaAlberoImprese.Appezzamento
                        indirizzoEF = AgronicaCoreAnagrafeDAL.EFAppezzamento.CreateIndirizzoAppezzamento(indirizzoAssociato.indirizzo, objParametri_Server, Appezzamento, indirizzoAssociato.tipo_Indirizzo, username, GiasContext, NewTransaction)
                    Case Else
                        Throw New Exception("Scrittura Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                End Select

                Cod_Indirizzo = indirizzoEF.cod_indirizzo

            Else

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Appezzamento
                        AgronicaCoreAnagrafeDAL.EFAppezzamento.ModificaIndirizzoAppezzamento(indirizzoAssociato.indirizzo, objParametri_Server, Appezzamento, indirizzoAssociato.tipo_Indirizzo, username, GiasContext, NewTransaction)
                    Case Else
                        Cod_Indirizzo = indirizzoAssociato.indirizzo.codice
                        indirizzoEF = (From ind In GiasContext.Indirizzi
                                       Where ind.cod_indirizzo = Cod_Indirizzo).FirstOrDefault()

                        '########### elemxIndirizzi ###########
                        Select Case Elemento_Anagrafico

                            Case enum_EntitaAlberoImprese.Impresa

                                Dim ImpresexIndirizziEF As AgronicaCoreEntityFramework_POCO.ImpresexIndirizzi
                                Dim PIVA = Impresa.PIVA

                                ImpresexIndirizziEF = (From ixi In GiasContext.ImpresexIndirizzi
                                                       Where ixi.PIVA = PIVA AndAlso
                                                              ixi.cod_indirizzo = Cod_Indirizzo).FirstOrDefault()

                                If ImpresexIndirizziEF Is Nothing Then
                                    Throw New GiasException("Indirizzo non trovato.")
                                End If

                                If ImpresexIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                                    'se il tipo_indirizzo è uguale, non faccio nulla
                                Else
                                    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                                    GiasContext.ImpresexIndirizzi.Attach(ImpresexIndirizziEF)
                                    GiasContext.ImpresexIndirizzi.Remove(ImpresexIndirizziEF)

                                    ImpresexIndirizziEF.PIVA = PIVA

                                    ImpresexIndirizziEF.cod_indirizzo = Cod_Indirizzo
                                    ImpresexIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                                    ImpresexIndirizziEF.inviato = 0
                                    ImpresexIndirizziEF.datainvio = DateTime.Now

                                    ImpresexIndirizziEF.Data_Creazione = DateTime.Now
                                    ImpresexIndirizziEF.Data_Modifica = DateTime.Now

                                    ImpresexIndirizziEF.Username_Creazione = username
                                    ImpresexIndirizziEF.Username_Modifica = username

                                    ImpresexIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                                    ImpresexIndirizziEF.Validita_Fine = AGRODATAFINE

                                    ImpresexIndirizziEF.Validazione = 0
                                    ImpresexIndirizziEF.Data_Validazione = DateTime.Now
                                    ImpresexIndirizziEF.UserName_Validazione = ""

                                End If

                            Case enum_EntitaAlberoImprese.Centro

                                Dim CentrixIndirizziEF As AgronicaCoreEntityFramework_POCO.CentrixIndirizzi
                                Dim PIVA = Centro.PIVA
                                Dim SA_COD = Centro.sa_cod

                                CentrixIndirizziEF = (From cenxi In GiasContext.CentrixIndirizzi Where
                                                                                     cenxi.PIVA = PIVA AndAlso
                                                                                     cenxi.sa_cod = SA_COD AndAlso
                                                                                     cenxi.cod_indirizzo = Cod_Indirizzo).FirstOrDefault()

                                If CentrixIndirizziEF Is Nothing OrElse CentrixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                                    'se il tipo_indirizzo è uguale, non faccio nulla
                                Else
                                    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                                    GiasContext.CentrixIndirizzi.Attach(CentrixIndirizziEF)
                                    GiasContext.CentrixIndirizzi.Remove(CentrixIndirizziEF)

                                    CentrixIndirizziEF.PIVA = PIVA
                                    CentrixIndirizziEF.sa_cod = SA_COD

                                    CentrixIndirizziEF.cod_indirizzo = Cod_Indirizzo
                                    CentrixIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                                    CentrixIndirizziEF.inviato = 0
                                    CentrixIndirizziEF.datainvio = DateTime.Now

                                    CentrixIndirizziEF.Data_Creazione = DateTime.Now
                                    CentrixIndirizziEF.Data_Modifica = DateTime.Now

                                    CentrixIndirizziEF.Username_Creazione = username
                                    CentrixIndirizziEF.Username_Modifica = username

                                    CentrixIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                                    CentrixIndirizziEF.Validita_Fine = AGRODATAFINE

                                    CentrixIndirizziEF.Validazione = 0
                                    CentrixIndirizziEF.Data_Validazione = DateTime.Now
                                    CentrixIndirizziEF.UserName_Validazione = ""

                                End If

                            Case enum_EntitaAlberoImprese.Contatto

                                Dim ContattiXIndirizziEF As AgronicaCoreEntityFramework_POCO.ContattiXIndirizzi
                                Dim PIVA = Contatto.Piva
                                Dim Cod_Contatto = Contatto.Cod_Contatto
                                Dim SA_COD = Contatto.Sa_Cod

                                ContattiXIndirizziEF = (From cxi In GiasContext.ContattiXIndirizzi
                                                        Where cxi.Piva = PIVA AndAlso
                                                              cxi.Cod_Contatto = Cod_Contatto AndAlso
                                                              cxi.Cod_Indirizzo = Cod_Indirizzo).FirstOrDefault()

                                If ContattiXIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo Then
                                    'se il tipo_indirizzo è uguale, non faccio nulla
                                Else
                                    ' se il tipo indirizzo è diverso, cancello il record e lo riscrivo
                                    GiasContext.ContattiXIndirizzi.Attach(ContattiXIndirizziEF)
                                    GiasContext.ContattiXIndirizzi.Remove(ContattiXIndirizziEF)

                                    ContattiXIndirizziEF.Piva = PIVA
                                    ContattiXIndirizziEF.Sa_Cod = SA_COD
                                    ContattiXIndirizziEF.Cod_Contatto = Cod_Contatto

                                    ContattiXIndirizziEF.Cod_Indirizzo = Cod_Indirizzo
                                    ContattiXIndirizziEF.Tipo_Indirizzo = indirizzoAssociato.tipo_Indirizzo

                                    ContattiXIndirizziEF.Inviato = 0
                                    ContattiXIndirizziEF.DataInvio = DateTime.Now

                                    ContattiXIndirizziEF.Data_Creazione = DateTime.Now
                                    ContattiXIndirizziEF.Data_Modifica = DateTime.Now

                                    ContattiXIndirizziEF.Username_Creazione = username
                                    ContattiXIndirizziEF.Username_Modifica = username

                                    ContattiXIndirizziEF.Validita_Inizio = AGRODATAINIZIO
                                    ContattiXIndirizziEF.Validita_Fine = AGRODATAFINE

                                End If

                            Case Else
                                Throw New Exception("Modifica Indirizzo Associato su elemento anagrafico " & Elemento_Anagrafico & " non riuscita. ")
                        End Select


                        If indirizzoEF Is Nothing Then
                            Throw New GiasException("Indirizzo non trovato.")
                        End If

                        '###  INDIRIZZO  ###
                        indirizzoEF.ind_des = indirizzoAssociato.indirizzo.via
                        indirizzoEF.frz_des = indirizzoAssociato.indirizzo.frazione

                        '###  ISTAT  ###
                        indirizzoEF.pro_cod_istat = indirizzoAssociato.indirizzo.istatComune.prov
                        indirizzoEF.com_cod_istat = indirizzoAssociato.indirizzo.istatComune.com
                        indirizzoEF.CAP = indirizzoAssociato.indirizzo.cap

                        indirizzoEF.com_des = ""
                        indirizzoEF.pro_cod = ""

                        If indirizzoAssociato.indirizzo.istatComune.localita IsNot Nothing AndAlso indirizzoAssociato.indirizzo.istatComune.localita <> "" Then
                            indirizzoEF.com_des = indirizzoAssociato.indirizzo.istatComune.localita
                        End If

                        If indirizzoAssociato.indirizzo.istatComune.comuni_prov IsNot Nothing AndAlso
                                indirizzoAssociato.indirizzo.istatComune.comuni_prov <> "" Then
                            Dim provincia = (From prov In GiasContext.Lista_Province Where prov.PROV = indirizzoAssociato.indirizzo.istatComune.prov).FirstOrDefault()
                            If provincia IsNot Nothing Then
                                indirizzoEF.pro_cod = provincia.SIGLA
                            Else
                                indirizzoEF.pro_cod = ""
                            End If

                        End If

                        indirizzoEF.stato = indirizzoAssociato.indirizzo.stato.codice

                        indirizzoEF.note = indirizzoAssociato.indirizzo.note

                        GiasContext.SaveChanges()
                End Select
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
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

        Return Cod_Indirizzo
    End Function

    Public Function Scrivi_Indirizzo_Fabbricato(indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Indirizzi_W.Scrivi_Indirizzi_Associati()"
        Dim messaggioErrore As String = ""

        Dim Cod_Indirizzo As Integer 'corrisponde al codice nell'oggetto Indirizzo
        Dim indirizzoEF As AgronicaCoreEntityFramework_POCO.Indirizzi

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)

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
            If indirizzo.codice = 0 Then

                indirizzoEF = AgronicaCoreAnagrafeDAL.EFIndirizzi.CreateIndirizziEF(GiasContext, objParametri_Server, objParametri_Utenti.UsernameOperazione)

                Cod_Indirizzo = indirizzoEF.cod_indirizzo

            Else

                Cod_Indirizzo = indirizzo.codice

                indirizzoEF = (From ind In GiasContext.Indirizzi Where
                                    ind.cod_indirizzo = Cod_Indirizzo).FirstOrDefault()

            End If

            If indirizzoEF Is Nothing Then
                Throw New GiasException("Indirizzo non trovato.")
            End If

            '###  INDIRIZZO  ###
            indirizzoEF.ind_des = indirizzo.via
            indirizzoEF.frz_des = indirizzo.frazione

            '###  ISTAT  ###
            indirizzoEF.pro_cod_istat = indirizzo.istatComune.prov
            indirizzoEF.com_cod_istat = indirizzo.istatComune.com
            indirizzoEF.CAP = indirizzo.cap
            indirizzoEF.com_des = indirizzo.istatComune.localita
            indirizzoEF.pro_cod = indirizzo.istatComune.comuni_prov

            indirizzoEF.stato = indirizzo.stato.codice

            indirizzoEF.note = indirizzo.note

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
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

        Return Cod_Indirizzo
    End Function

    Public Function Scrivi_Indirizzi_Associati_Impresa(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                       ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Indirizzi_Associati(indirizzoAssociato, GiasContext, Impresa, Nothing, Nothing, Nothing, enum_EntitaAlberoImprese.Impresa, objParametri_Server, objParametri_Utenti, NewTransaction)

    End Function
    'Return Scrivi_Indirizzi_Associati()
    Public Function Scrivi_Indirizzi_Associati_Centro(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                       ByRef Centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByVal NewTransaction As Boolean = True)
        Return Scrivi_Indirizzi_Associati(indirizzoAssociato, GiasContext, Nothing, Centro, Nothing, Nothing, enum_EntitaAlberoImprese.Centro, objParametri_Server, objParametri_Utenti, NewTransaction)
    End Function
    Public Function Scrivi_Indirizzi_Associati_Contatto(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                       ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Indirizzi_Associati(indirizzoAssociato, GiasContext, Nothing, Nothing, Contatto, Nothing, enum_EntitaAlberoImprese.Contatto, objParametri_Server, objParametri_Utenti, NewTransaction)

    End Function
    Public Function Scrivi_Indirizzi_Associati_Appezzamento(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                       ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Appezzamento,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByVal NewTransaction As Boolean = True)
        Return Scrivi_Indirizzi_Associati(indirizzoAssociato, GiasContext, Nothing, Nothing, Nothing, Appezzamento, enum_EntitaAlberoImprese.Appezzamento, objParametri_Server, objParametri_Utenti, NewTransaction)
    End Function

    Private Sub CapValidator(indirizzoAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
        If indirizzoAssociato.indirizzo.stato.codice = "IT" AndAlso indirizzoAssociato.indirizzo.cap.Count() > 5 Then
            Throw New GiasException("CAP length exceeds maximum length for italian address CAP")
        End If
    End Sub
End Class