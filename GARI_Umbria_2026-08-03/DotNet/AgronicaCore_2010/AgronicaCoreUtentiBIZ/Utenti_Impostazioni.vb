Imports AgronicaCoreContabHLP
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreAnagrafeDAL
Imports System.Net.Http

Public Class Utenti_Impostazioni_R

    Private Class Impostazione_DB
        Public UserName As String = ""
        Public Impostazione_Cod As Integer
        Public Valore1 As String = ""
        Public Valore2 As String = ""
        Public Valore3 As String = ""
        Public Valore4 As String = ""
    End Class


    Public Function RipartoCatastoConsideraRossiSoglia(objParametri_Utenti As AgronicaCoreParametri) As Decimal
        Dim lSogliaRossi As Decimal


        Dim leggiParametroRossi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtImp As DataTable = leggiParametroRossi.Leggi(
            AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_CATASTO_PercentoSogliaDifferenzeEvidenziate,
            1,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri_Utenti
        )

        If dtImp.Rows.Count > 0 Then
            Try
                lSogliaRossi = CStr(dtImp.Rows(0)("Impostazione_Valore_1")).Replace(",", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            Catch ex As Exception
                lSogliaRossi = 5.0
            End Try

        Else
            lSogliaRossi = 5.0
        End If

        Return lSogliaRossi
    End Function

    ''' <summary>
    ''' se esiste l'impostazione e vale 1 allora l'utente è tenuto a configurare e profilare l'azienda
    ''' </summary>
    ''' <param name="user"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function UTENTE_Attiva_Configurazione_Pratica_FlagAttivo(user As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean


        'se esiste l'impostazione e vale 1 allora l'utente è tenuto a configurare e profilare l'azienda. 

        Dim rval As Boolean = False

        Dim leggiImpostazioniUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtImpostazioneUtente As DataTable =
            leggiImpostazioniUtente.Leggi2(1, user, enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica, "", "", objParametri_Utenti)

        If dtImpostazioneUtente.Rows.Count > 0 AndAlso (dtImpostazioneUtente(0)("Impostazione_Valore_1") = "1") Then

            rval = True

        End If

        Return rval

    End Function

    '###############################################################################################
    Public Function LeggiOpzioni_DocContabili(ByVal piva As String, ByRef objParametriServer As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As Dictionary(Of String, Object)

        Const nomeRoutine = "AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R.LeggiOpzioni_DocContabili()"

        Dim dtUtente As DataTable
        Dim dtSuperUser As DataTable
        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim impreseImpostazioniR As New Imprese_Impostazioni_R

        'la chiave è il toString dell'enum (es:"SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA") e il valore è il valore di quell'impostazione
        Dim impostazioniDict As New Dictionary(Of String, Object)

        Try
            Dim enumType As Type = GetType(enum_Impostazioni_Utenti)
            Dim codImp As enum_Impostazioni_Utenti
            Dim impVal As String = ""

            Dim defImpImpresa As New Dictionary(Of enum_Impostazioni_Utenti, Object) From {
                {enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto, ""},
                {enum_Impostazioni_Utenti.GruppoMerce_Controllo, False},
                {enum_Impostazioni_Utenti.CdC_Wbs_Controllo, False},
                {enum_Impostazioni_Utenti.IB_RisorseUmane_SettoreDes_Controllo, False},
                {enum_Impostazioni_Utenti.Collega_Solo_Ordini_Inviati, False},
                {enum_Impostazioni_Utenti.ImpedisciCreazioneCarichiMultiriga, False},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA, ""},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_NOTE_DEFAULT_FATTURA_EMESSA, ""},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_MODALITA_TRASPORTO_DEFAULT, enum_TrasportoACaricoDi.Cedente},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_Aspetto_Beni_Default, ""},
                {enum_Impostazioni_Utenti.Degrado_Visibilita_Obbligatorieta, 1},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_AGENTI, False},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_SEZIONALI, False},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_TIPO_DOC_ACCETTAZIONE_DEFAULT, LAVCOD_ACCETTAZIONE_DIVERSI},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_CHKCOGE_MANUALE_DEFAULT, False},
                {enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_CAPOAREA, False},
                {enum_Impostazioni_Utenti.SUPERUSER_SoloLottiDisponibiliInOrdineVendita, False},
                {enum_Impostazioni_Utenti.SUPERUSER_Applica_Listini_Non_Associati, True},
                {enum_Impostazioni_Utenti.SUPERUSER_EDIT_LOTTO, False},
                {enum_Impostazioni_Utenti.SUPERUSER_OPERATORE_ACCETTAZIONE, False},
                {enum_Impostazioni_Utenti.SuperUser_CessionariAggiuntivi, False},
                {enum_Impostazioni_Utenti.SuperUser_RifDoc_EnteConsorzio, False},
                {enum_Impostazioni_Utenti.SuperUser_PesiColli_Riscontrati, False},
                {enum_Impostazioni_Utenti.SUPERUSER_LIVELLO_GESTIONE_CONTABILITA, enum_Livello_Gestione_Contabilita.NonGestita},
                {enum_Impostazioni_Utenti.SUPERUSER_ACCETTAZIONE_CON_GERARCHIA, False},
                {enum_Impostazioni_Utenti.SUPERUSER_DocContabili_SceltaImputazione, False},
                {enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO, False},
                {enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, ""},
                {enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, ""},
                {enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT, 0},
                {enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT, 0},
                {enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT, 0},
                {enum_Impostazioni_Utenti.RICERCA_CON_FILTRO_CESSIONARIO_DOC_CONT, False},
                {enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco, ""}
            }

            For Each elemDictImp In defImpImpresa
                impVal = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                  elemDictImp.Key,
                                                                                  elemDictImp.Value,
                                                                                  objParametriUtenti,
                                                                                  objParametriServer)

                'Sfrutto il valore di default utilizzato nel dictionary per capire di che tipo deve essere l'impostazione
                Select Case Type.GetTypeCode(elemDictImp.Value.GetType())
                    Case TypeCode.Boolean
                        impostazioniDict.Add([Enum].GetName(enumType, elemDictImp.Key), CBool(impVal))

                    Case TypeCode.Int32
                        impostazioniDict.Add([Enum].GetName(enumType, elemDictImp.Key), CInt(impVal))

                    Case Else
                        impostazioniDict.Add([Enum].GetName(enumType, elemDictImp.Key), impVal)

                End Select

                'impostazioniDict.Add(
                '    [Enum].GetName(enumType, elemDictImp.Key),
                '    Convert.ChangeType(impVal, Type.GetTypeCode(elemDictImp.Value.GetType()))
                ')
                'In teoria sarebbe possibile usare Convert.ChangeType al posto del select-case, ma la conversione da string "0" a boolean restituisce eccezione,
                'mentre con il CBool restituisce, come vorrei: "false".
                'Di conseguenza non avendo un'altra funzione di conversione che prende dinamicamente il tipo di una variabile e che usa CBool, devo usare il
                'select-case. CType non posso usarla perché ha bisogno del tipo definito a compile-time,
                'quindi occorre indicargli direttamente le keyword (Integer, String ecc...)
            Next

            codImp = enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE
            impVal = impostazioniDict([Enum].GetName(enumType, codImp))
            If Not String.IsNullOrEmpty(impVal) Then

                Dim listImpGiac As New List(Of Object)

                Dim tempSplit As String() = impVal.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
                For Each item In tempSplit
                    Dim elemGiac As String() = item.Split("_")
                    listImpGiac.Add(New With {.Elem_Cod = CInt(elemGiac(0)), .Impostazione_Valore = CInt(elemGiac(1))})
                Next

                impostazioniDict([Enum].GetName(enumType, codImp)) = listImpGiac

            Else
                impostazioniDict([Enum].GetName(enumType, codImp)) = New List(Of Object)
            End If


            codImp = enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI
            impVal = impostazioniDict([Enum].GetName(enumType, codImp))
            If Not String.IsNullOrEmpty(impVal) Then

                Dim listImpLotti As New List(Of Object)

                Dim tempSplit As String() = impVal.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
                For Each item In tempSplit
                    Dim elemGiac As String() = item.Split("_")
                    listImpLotti.Add(New With {.Elem_Cod = CInt(elemGiac(0)), .Impostazione_Valore = CInt(elemGiac(1))})
                Next

                impostazioniDict([Enum].GetName(enumType, codImp)) = listImpLotti
            Else
                impostazioniDict([Enum].GetName(enumType, codImp)) = New List(Of Object)
            End If


            codImp = enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco
            impVal = impostazioniDict([Enum].GetName(enumType, codImp))
            If Not String.IsNullOrEmpty(impVal) Then

                Dim listImpDataScadLotto As New List(Of Object)

                Dim tempSplit As String() = impVal.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
                For Each item In tempSplit
                    Dim elemGiac As String() = item.Split("_")
                    listImpDataScadLotto.Add(New With {.Elem_Cod = CInt(elemGiac(0)), .Impostazione_Valore = CInt(elemGiac(1))})
                Next

                impostazioniDict([Enum].GetName(enumType, codImp)) = listImpDataScadLotto
            Else
                impostazioniDict([Enum].GetName(enumType, codImp)) = New List(Of Object)
            End If


            'Visto che l'impostazione del layout è sparsa su 3 opzioni diverse, astraggo tutto e creo un'unica "opzione",
            ' anche perché anche l'UI la gestisce in astratto senza sapere che sono 3 campi diversi 

            codImp = enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT
            Dim valImpLayoutPeso As Integer = impostazioniDict([Enum].GetName(enumType, codImp))

            codImp = enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT
            Dim valImpLayoutPrezzo As Integer = impostazioniDict([Enum].GetName(enumType, codImp))

            codImp = enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT
            Dim valImpLayoutRiscontrato As Integer = impostazioniDict([Enum].GetName(enumType, codImp))

            Dim impFormatiStampa = Contabilita.GetLayoutFormatiStampa(valImpLayoutPeso,
                                                                          valImpLayoutPrezzo,
                                                                          valImpLayoutRiscontrato)

            impostazioniDict.Add("SuperUser_LayOut_FormatiStampa_DDT", impFormatiStampa)

            '------------------------------------------------------------

            dtUtente = objImpR.Leggi(0, 1,
                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "", objParametriUtenti)

            If Not IsNothing(dtUtente) AndAlso dtUtente.Rows.Count > 0 Then
                codImp = enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtUtente, codImp, "")))

                codImp = enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CBool(GetImpostazioneDt(dtUtente, codImp, True)))

                codImp = enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CBool(GetImpostazioneDt(dtUtente, codImp, False)))

                codImp = enum_Impostazioni_Utenti.UTENTE_COD_TIPO_ALLERTA_PREZZO_0_VENDITA
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CInt(GetImpostazioneDt(dtUtente, codImp, 0)))

                'UTENTE_COD_TIPO_ALLERTA_PREZZO_0_VENDITA:
                'Nessuna = 0
                'Allerta Video = 1
                'Solo Suono = 2
                'Allerta Video + Suono = 3

                codImp = enum_Impostazioni_Utenti.UTENTE_COD_TIPO_FATTURA_DEFAULT
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CInt(GetImpostazioneDt(dtUtente, codImp, enum_FatturaTipo.Differita)))

            Else
                'Devo impostare tutte le righe con il default (caso rarissimo che non c'è nessuna impostazione)
                impostazioniDict.Add(enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO.ToString(), "")
                impostazioniDict.Add(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE.ToString(), True)
                impostazioniDict.Add(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI.ToString(), False)
                impostazioniDict.Add(enum_Impostazioni_Utenti.UTENTE_COD_TIPO_ALLERTA_PREZZO_0_VENDITA.ToString(), 0)
                impostazioniDict.Add(enum_Impostazioni_Utenti.UTENTE_COD_TIPO_FATTURA_DEFAULT.ToString(), CInt(enum_FatturaTipo.Differita))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return impostazioniDict

    End Function

    '###############################################################################################
    Public Function LeggiOpzioni_Contatti(ByRef objParametri As AgronicaCoreParametri) As Dictionary(Of String, Object)

        Const nomeRoutine = "AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R.LeggiOpzioni_Contatti()"

        Dim dtUtente As DataTable
        Dim dtSuperUser As DataTable
        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'la chiave è il toString dell'enum (es:"SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA") e il valore è il valore di quell'impostazione
        Dim impostazioniDict As New Dictionary(Of String, Object)

        Try
            Dim enumType As Type = GetType(enum_Impostazioni_Utenti)
            Dim codImp As enum_Impostazioni_Utenti

            dtSuperUser = objImpR.Leggi(0, 2,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri)

            If Not IsNothing(dtSuperUser) AndAlso dtSuperUser.Rows.Count > 0 Then
                codImp = enum_Impostazioni_Utenti.SUPERUSER_Consenti_ContattoCod_Duplicato
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

                codImp = enum_Impostazioni_Utenti.SUPERUSER_ContattoCod_Max_Lenght
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

                codImp = enum_Impostazioni_Utenti.SUPERUSER_LIVELLO_GESTIONE_CONTABILITA
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

                codImp = enum_Impostazioni_Utenti.SUPERUSER_IMPEDISCI_ELIMINAZIONE_CONTATTI_E_RISORSE_UMANE
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

            End If

            '------------------------------------------------------------

            dtUtente = objImpR.Leggi(0, 1,
                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "", objParametri)

            If Not IsNothing(dtUtente) AndAlso dtUtente.Rows.Count > 0 Then
                codImp = enum_Impostazioni_Utenti.UTENTE_DAA
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtUtente, codImp, "")))
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return impostazioniDict

    End Function

    '###############################################################################################
    Public Function LeggiOpzioni_MateriePrime(ByRef objParametri As AgronicaCoreParametri) As Dictionary(Of String, Object)

        Const nomeRoutine = "AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R.LeggiOpzioni_Contatti()"

        Dim dtSuperUser As DataTable
        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'la chiave è il toString dell'enum (es:"SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA") e il valore è il valore di quell'impostazione
        Dim impostazioniDict As New Dictionary(Of String, Object)

        Try
            Dim enumType As Type = GetType(enum_Impostazioni_Utenti)
            Dim codImp As enum_Impostazioni_Utenti

            dtSuperUser = objImpR.Leggi(0, 2,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri)

            If Not IsNothing(dtSuperUser) AndAlso dtSuperUser.Rows.Count > 0 Then
                codImp = enum_Impostazioni_Utenti.SUPERUSER_Consenti_ProdottoCod_Duplicato
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

                codImp = enum_Impostazioni_Utenti.SUPERUSER_PRODOTTOCOD_MAX_LENGTH
                impostazioniDict.Add([Enum].GetName(enumType, codImp), CStr(GetImpostazioneDt(dtSuperUser, codImp, "")))

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return impostazioniDict

    End Function

    Private Function GetImpostazioneDt(ByRef dt As DataTable,
                                       ByVal impostazione As enum_Impostazioni_Utenti,
                                       ByVal valDefault As Object
                                       ) As Object

        Dim valImp = (From dr As DataRow In dt.AsEnumerable()
                      Where dr.Field(Of Integer)("Impostazione_Cod") = impostazione
                      Select dr.Field(Of String)("Impostazione_Valore_1")).FirstOrDefault()

        If Not valImp Is Nothing Then
            Return valImp
        Else
            Return valDefault
        End If

    End Function

    '###############################################################################################

    Private Function ShouldLoadAdvancedSettings(obj_Utenti As AgronicaCoreParametri) As Boolean
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim canViewAgendaBlocks = objPermessi.Controlla_Permessi_Utente(
            obj_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Gest_UtentiAgendaBlocchi, enum_Security_Operazione.Lettura,
            Now, String.Empty, obj_Utenti
        )
        Dim canViewAdvancedSettings = objPermessi.Controlla_Permessi_Utente(
            obj_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Gest_UtentiImpostazioni_Avanzate, enum_Security_Operazione.Lettura,
            Now, String.Empty, obj_Utenti
        )
        Return canViewAdvancedSettings AndAlso canViewAgendaBlocks
    End Function

    Public Function CaricaSezioni_Impostazioni_Utente(flagLetturaSuperUser As Integer, obj_Utenti As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim xFiltroAggiuntivo As String = String.Empty
        Dim xOrderBy As String = " Guida_Impostazioni.Sezione, Ordine, Impostazione_Cod "
        Select Case (flagLetturaSuperUser)
            Case 0 ' Legge solo le impostazioni utente
                xFiltroAggiuntivo = " Impostazione_Utente = 1 AND Impostazione_SuperUser = 0 "
            Case 1 ' Legge solo le impostazioni superuser
                xFiltroAggiuntivo = " Impostazione_Utente = 0 AND Impostazione_SuperUser = 1 "
            Case 2 ' Legge sia le impostazioni utente che quelle superuser
                xFiltroAggiuntivo = " Impostazione_Utente = 1 OR Impostazione_SuperUser = 1 "
        End Select

        Dim DT_Impostazioni As DataTable = objImpostazioni.LeggiSezioniImpostazioni(xFiltroAggiuntivo, xOrderBy, obj_Utenti)
        Dim rows = If(ShouldLoadAdvancedSettings(obj_Utenti), DT_Impostazioni.Select, DT_Impostazioni.Select(" SottoSezioneCod <> 5 "))

        Dim settingsList = rows.Select(Function(row) New With {
                            .Impostazione_Cod = row("Impostazione_Cod"),
                            .Impostazione_Des = row("Label"),
                            .Sezione_Des = row("SezioneDes"),
                            .Sezione_Cod = row("SezioneCod"),
                            .SottoSezione_Des = row("SottoSezioneDes"),
                            .SottoSezione_Cod = row("SottoSezioneCod"),
                            .SottoSezione_Espandibile = row("SottoSezioneEspandibile"),
                            .Livello_Des = row("LivelloDes"),
                            .Livello_Cod = row("LivelloCod"),
                            .Livello_Espandibile = row("LivelloEspandibile"),
                            .Ordine = row("Ordine"),
                            .Tipo_Campo = row("Tipo_Campo"),
                            .Valore_Default = row("Valore_Default"),
                            .Note = row("Note"),
                            .Impostazione_Utente = row("Impostazione_Utente"),
                            .Impostazione_SuperUser = row("Impostazione_SuperUser"),
                            .Impostazione_Azienda = row("Impostazione_Azienda"),
                            .Impostazione_Azienda_Centro = row("Impostazione_Azienda_Centro"),
                            .Impostazione_Azienda_Centro_Specie = row("Impostazione_Azienda_Centro_Specie")
                        })
        Return settingsList
    End Function

    ''' <summary>
    ''' Carica i dati relativi a un'impostazione utente tra quelli inseriti nella tabella <tt>Guida_Impostazioni_Valori</tt>.
    ''' </summary>
    ''' <param name="impCod"></param>
    ''' <param name="obj_Utenti"></param>
    ''' <returns>Una lista contentente il codice dell'impostazione e i relativi dati.</returns>
    Public Function Carica_Impostazioni2(impCod As Integer, obj_Utenti As AgronicaCoreParametri) As IEnumerable(Of profilazione.ImpostazioneBase)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim data = objImpostazioni.LeggiDatiImpostazione(
            impCod, "",
            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            obj_Utenti
        ).AsEnumerable.
        Select(Function(row) New profilazione.ImpostazioneBase With {
            .codice = row("codice"),
            .descrizione = row("descrizione").ToString,
            .Valore = row("Valore_Default").ToString,
            .TipoCampo = row("tipo_campo").ToString,
            .Note = row("note").ToString
        })
        Return data
    End Function

    ''' <summary>
    ''' Legge i valori scelti dall'utente per le impostazioni specificate.
    ''' La lettura considera i valori impostati a livello di utente, tipologia, superuser, default
    ''' </summary>
    Public Function LeggiImpostazioniScalare(
        username As String,
        impostazioni As IEnumerable(Of Integer),
        objPUtenti As AgronicaCoreParametri
    ) As Dictionary(Of Integer, String)
        Dim settingsMng As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim settingsMngFm As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim withFiltroMono = impostazioni.Where(Function(k) Utenti_Impostazioni_W.haFiltroMono(k))
        Dim withoutFiltroMono = impostazioni.Where(Function(k) Not Utenti_Impostazioni_W.haFiltroMono(k))
        Dim settingsFiltroMono = settingsMngFm.LeggiScalare(username, withFiltroMono, objPUtenti).
            Select(Function(item) New KeyValuePair(Of Integer, String)(item.Impostazione_Cod, item.Valore))
        Dim settings = settingsMng.LeggiScalare(username, withoutFiltroMono, objPUtenti).
            Select(Function(item) New KeyValuePair(Of Integer, String)(item.Impostazione_Cod, item.Valore))
        Return KeyValuesToDictOrEmpty(settings.Union(settingsFiltroMono))
    End Function

    ''' <summary>
    ''' Legge i valori scelti dall'utente per le impostazioni specificate.
    ''' La lettura considera i valori impostati a livello di utente, tipologia, superuser, default
    ''' </summary>
    ''' <param name="impostazioni">Ognichiave deve essere associata a una sequenza non vuota.</param>
    ''' <returns>Una copia del Dictionary di impostazioni con i valori attuali correttamente valorizzati</returns>
    ''' <seealso cref="LeggiImpostazioniScalare(String, IEnumerable(Of Integer), AgronicaCoreParametri)"/>
    Public Function LeggiDatiImpostazioniUtenteScalare(
        username As String,
        impostazioni As Dictionary(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)),
        objPUtenti As AgronicaCoreParametri
    ) As Dictionary(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase))
        Dim settings = LeggiDatiImpostazioniUtenteScalareNoFiltroMono(username, impostazioni, objPUtenti)
        Dim settingsFiltroMono = LeggiDatiImpostazioniUtenteScalareConFiltroMono(username, impostazioni, objPUtenti)
        Return KeyValuesToDictOrEmpty(settings.Union(settingsFiltroMono))
    End Function

    Private Function KeyValuesToDictOrEmpty(Of K, V)(ienum As IEnumerable(Of KeyValuePair(Of K, V))) As Dictionary(Of K, V)
        If ienum.Count > 0 Then
            Return ienum.ToDictionary(Function(x As KeyValuePair(Of K, V)) x.Key, Function(x As KeyValuePair(Of K, V)) x.Value)
        Else
            Return New Dictionary(Of K, V)
        End If
    End Function

    ''' <summary>
    ''' Legge i valori scelti dall'utente per le impostazioni specificate.
    ''' La lettura considera i valori impostati a livello di utente, tipologia, superuser, default
    ''' </summary>
    ''' <returns>Una copia del Dictionary di impostazioni con i valori attuali correttamente valorizzati</returns>
    Private Function LeggiDatiImpostazioniUtenteScalareNoFiltroMono(
        username As String,
        impostazioni As Dictionary(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)),
        objPUtenti As AgronicaCoreParametri
    ) As List(Of KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)))
        Dim settingsMng As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim withoutFiltroMono = impostazioni.Keys.Where(Function(k) Not Utenti_Impostazioni_W.haFiltroMono(k))

        ' contiene i valori validi per scalarita delle impostazioni senza filtro mono per l'utente
        Dim settings As Dictionary(Of Integer, utente.Utente_Impostazioni) = settingsMng.LeggiScalare(username, withoutFiltroMono, objPUtenti).
            ToDictionary(Function(x) x.Impostazione_Cod, Function(x) x)

        Dim personalized = impostazioni.Where(Function(item) settings.ContainsKey(item.Key))
        personalized = personalized.AsParallel.
            Select(Function(item) New With {
                .Key = item.Key,
                .Value = GetSettingsValueList(item, settings),
                .Read = settings(item.Key).Valore
            }).Select(Function(item) New KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase))(
                item.Key,
                patchValue(item.Value, item.Read)
            )).ToList()
        Return personalized
    End Function

    ''' <summary>
    ''' Restituisce la lista di valori disponibili per ogni impostazione caricata.
    ''' Se la lista di valori accettabili per l'impostazione è vuota (mancano i record nella tabella Guida_Impostazioni_Valori)
    ''' viene restituita una lista con un solo elemento che contiene il valore corrente letto per l'utente.
    ''' </summary>
    ''' <param name="item">Record che definisce l'impostazione e i suoi valori disponibili</param>
    ''' <param name="settings">Dictionary che associa ad ogni impostazione il valore letto per l'utente.</param>
    ''' <returns>Lista di profilazione.ImpostazioneBase contente i valori disponibili per l'impostazione indicata</returns>
    Private Function GetSettingsValueList(
        item As KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)), 
        settings As Dictionary(Of Integer, utente.Utente_Impostazioni)
    ) As  List(Of profilazione.ImpostazioneBase)
        If (item.Value.Any) Then
            Dim list = item.Value.ToList()
            list.ForEach(Sub(value) value.Username = settings(item.Key).Username)
            Return list

        Else
            Return New List(Of profilazione.ImpostazioneBase) From {
                New profilazione.ImpostazioneBase With {
                    .Valore = settings(item.Key).Valore,
                    .codice = item.Key,
                    .Username = settings(item.Key).Username
                }
            }
        End If
    End Function

    ''' <summary>
    ''' Legge i valori scelti dall'utente per le impostazioni specificate.
    ''' La lettura considera i valori impostati a livello di utente, tipologia, superuser, default
    ''' </summary>
    ''' <returns>Una copia del Dictionary di impostazioni con i valori attuali correttamente valorizzati</returns>
    Private Function LeggiDatiImpostazioniUtenteScalareConFiltroMono(
        username As String,
        impostazioni As Dictionary(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)),
        objPUtenti As AgronicaCoreParametri
    ) As List(Of KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)))
        Dim settingsMng As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim withFiltroMono = impostazioni.Keys.Where(Function(k) Utenti_Impostazioni_W.haFiltroMono(k))

        Dim valuesWithUsernaem = Function(setting As IEnumerable(Of profilazione.ImpostazioneBase), user As string)
                              Dim list = setting.ToList()
                              list.ForEach(Function(x) x.Username = user)
                              Return list
                          End Function

        Dim settings = settingsMng.LeggiScalare(username, withFiltroMono, objPUtenti).
            ToDictionary(Function(x) x.Impostazione_Cod, Function(x) x)
        'Dim personalized = New List(Of KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase)))

        Dim personalized = impostazioni.Where(Function(item) settings.ContainsKey(item.Key))

        '    Select(Function(item) setUsernaem(item, settings(item.Key).Username))
        'impostazioni = KeyValuesToDictOrEmpty(impostazioni.Except(personalized))

        personalized = personalized.AsParallel.
            Where(Function(item) item.Value.Any()).
            Select(Function(item) New With {
                .Key = item.Key,
                .Value = valuesWithUsernaem(item.Value, settings(item.Key).Username),
                .Read = settings(item.Key).Valore
            }).Select(Function(item) New KeyValuePair(Of Integer, IEnumerable(Of profilazione.ImpostazioneBase))(
                item.Key,
                patchValue(item.Value, item.Read)
            )).ToList()

        'impostazioni = KeyValuesToDictOrEmpty(impostazioni.Union(personalized))
        'Return impostazioni
        Return personalized
    End Function

    ''' <summary>
    ''' Legge il valore dell'impostazione specificata come stringa. La lettura avviene concatenando gli ID_0 letti
    ''' nella tabella <tt>Utenti_Impostazioni_FiltroMono</tt> se l'impostazione ha valori in filtroMono, o eseguendo
    ''' una lettura prima su utente, poi su superuser se l'impostazione è semplicemente salvata nella tabella
    ''' <tt>Utenti_Impostazioni</tt>.
    ''' </summary>
    ''' <param name="username">Utente per cui si vuole leggere l'impostazione</param>
    ''' <param name="codice">Codice dell'impostazione da leggere</param>
    ''' <param name="objUtenti">Parametri DB per l'esecuzione della query</param>
    ''' <returns></returns>
    Private Function leggiValoreStringaImpostazione(username As String, codice As Integer, objUtenti As AgronicaCoreParametri) As String
        If Utenti_Impostazioni_W.haFiltroMono(codice) Then
            Return leggiFiltroMonoScalare(username, codice, objUtenti)
        Else
            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim getStringOrDefault = Function(dr As DataRow, field As String) If(IsDBNull(dr(field)), String.Empty, dr(field))
            Return objImpostazioni.LeggiImpostazioneScalare(codice, username, objUtenti).
                Select(Function(dr) getStringOrDefault(dr, "Impostazione_Valore_1")).
                DefaultIfEmpty(String.Empty).First
        End If
    End Function

    ''' <summary>
    ''' Legge il valore nella tabella Utenti_Impostazioni_FiltroMono associato
    ''' all'impostazione specificata. Esegue la lettura scalando su utente,
    ''' profilo, superuser.
    ''' </summary>
    ''' <param name="username"></param>
    ''' <param name="codice"></param>
    ''' <param name="objUtenti"></param>
    ''' <returns>Stringa contenente i valori ID_0 associati all'impostazione
    ''' specificata concatenati tramite pipe.</returns>
    Private Function leggiFiltroMonoScalare(username As String, codice As Integer, objUtenti As AgronicaCoreParametri) As String
        Dim objImpostazioniFiltroMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim objUtentiDal As New AgronicaCoreUtentiDAL.Utenti_Read

        If Utenti_Impostazioni_W.haFiltroMono(codice) Then
            Dim dtfm = objImpostazioniFiltroMono.Leggi2(
                1, username,
                codice, 0,
                "", "", objUtenti
            )
            If IsNothing(dtfm) OrElse dtfm.Rows.Count = 0 Then
                Dim xFiltro = " username like '" & username & "' "
                Dim tipologia = objUtentiDal.Leggi(
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    xFiltro, xOrderBy:=String.Empty, objUtenti
                ).Select("Tipologia_Cod not is null").
                Select(Function(row) CStr(row("Tipologia_Cod"))).
                SingleOrDefault()
                If Not String.IsNullOrEmpty(tipologia) Then
                    dtfm = objImpostazioniFiltroMono.Leggi2(
                        Username_1Utente_o_2SuperUser:=1, tipologia,
                        codice, ID_0:=0,
                        xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objUtenti
                    )
                End If
            End If
            If IsNothing(dtfm) OrElse dtfm.Rows.Count = 0 Then
                dtfm = objImpostazioniFiltroMono.Leggi2(
                    Username_1Utente_o_2SuperUser:=2, username,
                    codice, ID_0:=0,
                    xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objUtenti
                )
            End If
            If dtfm.Rows.Count > 0 Then
                Return dtfm.Select.
                    Select(Function(row) row("ID_0")).
                    Aggregate(Function(acc, x) acc & "|" & x)
            End If
        End If
        Return String.Empty
    End Function

    Private Function patchValue(valori As List(Of profilazione.ImpostazioneBase), valueStr As String) As List(Of profilazione.ImpostazioneBase)
        Select Case valori.First.TipoCampo
            Case enum_TipoControllo.CASELLA_SPUNTA
                If valori.Count = 1 Then
                    valori.First.Valore = valueStr
                Else
                    Dim selezionati As New List(Of String)
                    If valueStr.Contains(",") Then
                        selezionati.AddRange(valueStr.Split(","))
                    ElseIf valueStr.Contains("|") Then
                        selezionati.AddRange(valueStr.Split("|"))
                    End If
                    valori.ForEach(Sub(x) x.Valore = If(selezionati.Contains(x.codice), "1", "0"))
                End If

            Case enum_TipoControllo.NUMERO_DECIMALE,
                 enum_TipoControllo.NUMERO_INTERO,
                 enum_TipoControllo.CASELLA_TESTO,
                 enum_TipoControllo.AREA_TESTO

                If valori.Count = 1 Then
                    valori.First.Valore = valueStr
                Else
                    Throw New InvalidOperationException("More than one control seems to be associated to this setting")
                End If

            Case enum_TipoControllo.CALENDARIO
                'Dim yyyymmdd2date = Function(str)
                '                        Dim yyyy = Integer.Parse(valueStr.Substring(0, 4))
                '                        Dim mm = Integer.Parse(valueStr.Substring(4, 2))
                '                        Dim dd = Integer.Parse(valueStr.Substring(6, 2))
                '                        Return New Date(yyyy, mm, dd)
                '                    End Function
                'valori.ForEach(Sub(ddlItem) ddlItem.Valore = yyyymmdd2date(valueStr))
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

            Case enum_TipoControllo.MENU_DISCESA,
                 enum_TipoControllo.PULSANTE_SCELTA
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

            Case Else
                valori.ForEach(Sub(ddlItem) ddlItem.Valore = valueStr)

        End Select
        Return valori
    End Function

    Public Function Carica_DDL(
        impCod As Integer, utenteSelezionato As String,
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri
    ) As IEnumerable(Of Object)
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impost As DataTable = objImpost.Leggi2(1, utenteSelezionato, 0, "", "", objParametri_Utenti)

        Select Case impCod
            Case enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE,
                enum_Impostazioni_Utenti.UTENTE_Operazione_Predefinita_Da_Impianto

                Return leggiOperazioniXDdl(impCod, utenteSelezionato, objParametri_Utenti, objParametri_Server)

            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI
                'TODO

            Case enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime,
                 enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI
                Return leggiDatiDaFiltroMono(impCod, utenteSelezionato, objParametri_Utenti)

            Case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA
                Dim objCul As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                Dim listObj As New List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                Dim varieta = leggiDatiDaFiltroMono(impCod, utenteSelezionato, objParametri_Utenti).GetEnumerator
                While (varieta.MoveNext)
                    Dim dt = objCul.Leggi(
                        varieta.Current.codice, 0, "",
                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                        "", "", objParametri_Utenti
                    )
                    listObj.AddRange(dt.AsEnumerable.
                                     Select(Function(row) New metaschema.utilizzi.Varieta With {
                                        .classType = "Varieta",
                                        .codice = row.Item("Cul_Cod"),
                                        .descrizione = row.Item("Cul_Des"),
                                        .specie = New metaschema.utilizzi.Specie With {
                                            .codice = row.Item("Veg_Cod"),
                                            .descrizione = row.Item("Veg_des")
                                        }
                                    })
                    )
                End While
                Return listObj

            Case enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT
                Dim list As New List(Of BaseCodeDescr)
                Dim collegate = {
                    enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT,
                    enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT,
                    enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT
                }.GetEnumerator
                While (collegate.MoveNext)
                    Dim code = collegate.Current
                    Dim value = objImpost.LeggiImpostazioneScalare(code, utenteSelezionato, objParametri_Utenti).FirstOrDefault
                    If value IsNot Nothing Then
                        list.Add(New BaseCodeDescr With {
                        .codice = code,
                        .descrizione = value.Item("Impostazione_Valore_1")
                    })
                    End If
                End While
                Return list
        End Select

        Return New List(Of BaseCodeDescr)
    End Function

    Private Function leggiOperazioniXDdl(
        impostazioneCod As Integer, username As String,
        objUtenti As AgronicaCoreParametri, objServer As AgronicaCoreParametri
    ) As IEnumerable(Of BaseCodeDescr)
        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim filtroUtente As String = ""
        Dim tipoOperazione As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim xOrderBy As String = ""

        Select Case impostazioneCod
            Case enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE
                filtroUtente = generaFiltroUtente(objUtenti)
                xFiltroAggiuntivo = STR_OP_NON_GESTITE & " AND  GruppoOperazioni.Tipo IN ('C','E','V')" & filtroUtente
                xOrderBy = " Operazioni.Lav_Des "

            Case enum_Impostazioni_Utenti.UTENTE_Operazione_Predefinita_Da_Impianto
                tipoOperazione = "C"

        End Select

        Return objOperazioniLeggi.Leggi(
                    0, 0, 0, tipoOperazione, 0, "", "",
                    False, False, False, False,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    xFiltroAggiuntivo, xOrderBy, objServer
                ).AsEnumerable.
                Select(Function(row) New BaseCodeDescr With {
                    .descrizione = row.Item("LAV_DES"),
                    .codice = row.Item("LAV_COD")
                })
    End Function

    Function leggiDatiDaFiltroMono(impostazioneCod As Integer, username As String, objUtenti As AgronicaCoreParametri) As IEnumerable(Of Object)
        Dim objFiltroMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim Dt_FiltroMono As DataTable = objFiltroMono.Leggi2(1, username, 0, 0, "", "", objUtenti)
        Dim filtroAggiuntivo = ""
        Dim xOrderBy = ""

        Select Case impostazioneCod
            Case Else
                Return objFiltroMono.Leggi2(
                    1, username, impostazioneCod,
                    0, filtroAggiuntivo, xOrderBy, objUtenti
                ).AsEnumerable().
                Select(Function(row) New With {
                    .descrizione = "",
                    .codice = row.Item("ID_0"),
                    .valore = row.Item("Str_0")
                })

        End Select
        Return New List(Of BaseCodeDescr)
    End Function

    Private Function generaFiltroUtente(objParametri_Utenti As AgronicaCoreParametri) As String
        Dim filtroUtente As String = ""
        Dim dt_FiltroUtente As DataTable
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, 1,
                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                          "", "", objParametri_Utenti)
        If dt_FiltroUtente.Rows.Count > 0 Then
            filtroUtente = " AND Operazioni.Lav_Cod in ("
            Dim j As Integer = 0
            For j = 0 To dt_FiltroUtente.Rows.Count - 1

                If j <> 0 Then
                    filtroUtente = filtroUtente & " ,"
                End If
                filtroUtente = filtroUtente & dt_FiltroUtente.Rows(j).Item("ID_0")
            Next
            filtroUtente = filtroUtente & " )  "
        End If

        Return filtroUtente
    End Function

    '###############################################################################################

    Public Function Carica_AreeGIAS(obj_Server As AgronicaCoreParametri)

        Dim objImpostazioi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim listaAree = objImpostazioi.CaricaAreeGIAS(obj_Server)

        Return listaAree

    End Function

    '###############################################################################################

    Public Function LeggiStampePreferite(obj_Utenti As AgronicaCoreParametri, obj_Server As AgronicaCoreParametri) As List(Of BaseCodeDescr)
        Dim risposta As String = ""
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim DT = objImpostazioni.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE,
                                                   1, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "", "", obj_Utenti)

        Dim listaStampe As New List(Of BaseCodeDescr)
        If DT.Rows.Count > 0 AndAlso Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then

            Dim codstr() As String = DT.Rows(0).Item("Impostazione_Valore_1").Split("|")

            For Each stmp In codstr
                If IsNumeric(stmp) Then
                    Dim stampacod As String = "Stampa-" & stmp
                    Dim stampades As String = New AgronicaCoreMetaSchemaDAL.StampeReport().LeggiDescrizione(CInt(stmp), "", obj_Server)
                    Dim item = New BaseCodeDescr()
                    item.codice = stmp
                    item.descrizione = stampades

                    listaStampe.Add(item)
                    'listaStampe.Add(" "" " & stampacod & " "": "" " & stampades & " "" ")
                End If
            Next

            'risposta = "{" & String.Join(",", listaStampe) & "}"
        End If

        Return listaStampe

    End Function

    '###############################################################################################

    ''' <summary>
    '''     Funzione che legge i valori delle impostazioni dato un codice tipologia.
    ''' </summary>
    ''' <param name="TipoUtente_1Utente_o_2SuperUser">Tipologia utente - 1 = utente, 2 = super utente</param>
    ''' <param name="codiceTipologia">Codice della tipologia</param>
    ''' <param name="Impostazione_Cod">Il codice identificativo dell'impostazione.</param>
    ''' <param name="xFiltroAggiuntivo">Filtro sulle impostazioni. Vuoto se non specificato.</param>
    ''' <param name="xOrderBy">Vuoto se non specificato.</param>
    ''' <param name="objParametri">Server su cui eseguire la query.</param>
    ''' <returns>Valori delle impostazioni dell'utente specificato.</returns>
    Public Function leggiImpostazioniDaCodiceTipologia(ByVal TipoUtente_1Utente_o_2SuperUser As Integer,
                                                       ByVal codiceTipologia As String,
                                                       ByVal Impostazione_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable
        Dim DT As DataTable
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        DT = objImpostazioni.Leggi2(TipoUtente_1Utente_o_2SuperUser,
                                    codiceTipologia,
                                    Impostazione_Cod,
                                    xFiltroAggiuntivo,
                                    xOrderBy,
                                    objParametri)

        Return DT
    End Function

End Class

Public Class Utenti_Impostazioni_W

    ' filtro per considerare solo le impostazioni gestite da interfaccia
    Public Sub SetFiltroImpostazioni(ByRef FiltroImpostazioni As String, ByRef FiltroImpostazioniFiltroMono As String)

        FiltroImpostazioni = " (Impostazione_Cod IN (" &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_QTA_PRODOTTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_QTA_ACQUA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_AVVERSITA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_INFESTANTI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_TUTTI_I_CENTRI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_CHILI_LITRI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NONCONFORME & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_DISCIPLINARE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_ProdottiTossiciPatentinoMovimenti & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO & "," &
                            enum_Impostazioni_Utenti.UTENTE_GESTIONE_MAGAZZINO_2 & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI_RICETTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_COSTI_DEFAULT & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_NumAppezza_Progr_Modalita & "," &
                            enum_Impostazioni_Utenti.UTENTE_MultiModificaImpianti_FiltroProprieta & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_UTILIZZA_SUP_APP_AGENDA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_PROXY & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_USERNAME_PROXY & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_PASSWORD_PROXY & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_HOST_PROXY & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_ARROTONDA_ACQUA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_NUMERO_RICETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_MACCHINE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_OPERATORI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_TECNICO_AUTORIZZANTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_FIRMA_AGRICOLTORE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_DATA_ULTIMA_MANUTENZIONE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_PIANOCONCIMAZIONE_CAMPAGNA_DATA_FIRMA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_ARROTONDA_ACQUA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_NUMERO_RICETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_MACCHINE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_OPERATORI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_TECNICO_AUTORIZZANTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DATA_FIRMA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DATA_ULTIMA_MANUTENZIONE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FASE_EPOCA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FILTRA_FASCICOLO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_CODICE_PRODUTTORE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_TECNICO_RIFERIMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DOSE_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_NASCONDI_CAMPO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_VISUALCAMPO_SOLOFRONTESPIZIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_SCHEDA_CAMPAGNA_NUM_APPEZZAMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_ARROTONDA_ACQUA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_NUMERO_RICETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_MACCHINE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_OPERATORI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_TECNICO_AUTORIZZANTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DATA_FIRMA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_FASE_EPOCA_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DOSE_ETICHETTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_NASCONDI_CAMPO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_MACCHINE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_OPERATORI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_TECNICO_AUTORIZZANTE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_FIRMA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_ULTIMA_MANUTENZIONE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_CAMPO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO_BIO & "," &
                            enum_Impostazioni_Utenti.Utente_SchedaColtBio_VisualizzaLottoSemine & "," &
                            enum_Impostazioni_Utenti.Utente_SchedaColtBio_VisualizzaLottoRaccolte & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_SCHEDA_CAMPAGNA_BIO_NUM_APPEZZAMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_CODICE_PRODUTTORE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_TECNICO_RIFERIMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_Tipo_Operazioni & "," &
                            enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_GruppoOperazioni & "," &
                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA & "," &
                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE & "," &
                            enum_Impostazioni_Utenti.UTENTE_AlberoAnagrafica_visualizzaRiferimentoAlfanumericoImpianto & "," &
                            enum_Impostazioni_Utenti.UTENTE_AlberoAnagrafica_ordinaDataUltimoImpianto & "," &
                            enum_Impostazioni_Utenti.UTENTE_Nitrati_PC_Analisi & "," &
                            enum_Impostazioni_Utenti.UTENTE_Nitrati_RegolamentoPC_Default & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda & "," &
                            enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria & "," &
                            enum_Impostazioni_Utenti.UTENTE_Planning_Date & "," &
                            enum_Impostazioni_Utenti.UTENTE_Planning_NValidazioneNome & "," &
                            enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica & "," &
                            enum_Impostazioni_Utenti.UTENTE_PRATICHE_DA_ATTIVARE_SCARICO_FASCICOLO & "," &
                            enum_Impostazioni_Utenti.UTENTE_WS_SCARICO_FASCICOLO & "," &
                            enum_Impostazioni_Utenti.UTENTE_WS_GESIONE_AZIENDE & "," &
                            enum_Impostazioni_Utenti.UTENTE_GESTORE_AZIENDE & "," &
                            enum_Impostazioni_Utenti.UTENTE_CONTROLLO_SCARICO_CDG & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SEMINA_SE_SENZA_QTA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPOLOGIA_PRODOTTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_Ribaltamento_CreaCampi & "," &
                            enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE & "," &
                            enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017 & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_MODALITA_STAMPA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DATA_ULTIMA_MANUTENZIONE & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_SEMINA_TIPO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FINALITA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_REGOLAMENTO & "," &
                            enum_Impostazioni_Utenti.UTENTE_NUOVA_PARTICELLA & "," &
                            enum_Impostazioni_Utenti.UTENTE_Operazione_Predefinita_Da_Impianto & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_AZIENDA_PREDEFINITA_ALL_AVVIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO & "))"

        FiltroImpostazioniFiltroMono = " (Impostazione_Cod IN (" &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI & "," &
                            enum_Impostazioni_Utenti.UTENTE_COD_UTILIZZA_SUP_APP_AGENDA & "," &
                            enum_Impostazioni_Utenti.UTENTE_MultiModificaImpianti_FiltroProprieta & "))"
    End Sub

    Public Sub ScriviModifica_Impostazione(ByVal Impostazione_Cod As Integer,
                                           ByVal Impostazione_Valore_1 As String,
                                           ByVal Impostazione_Valore_2 As String,
                                           ByVal Impostazione_Valore_3 As String,
                                           ByVal Impostazione_Valore_4 As String,
                                           ByVal Validita_Inizio As Date,
                                           ByVal Validita_Fine As Date,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Dim dt = objImpostazioni_R.Leggi(Impostazione_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        If dt.Rows.Count > 0 Then
            objImpostazioni_W.Modifica(Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Validita_Inizio, Validita_Fine, objParametri)
        Else
            objImpostazioni_W.Scrivi(Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Validita_Inizio, Validita_Fine, objParametri)
        End If

    End Sub

    Public Sub ScriviModificaImpostazioneUtente(username As String, impostazioneCod As Integer, objParametri_Utenti As AgronicaCoreParametri,
                                                valore1 As String,
                                                Optional valore2 As String = "", Optional valore3 As String = "", Optional valore4 As String = "",
                                                Optional validitaInizio As Date = AGRODATAINIZIO, Optional validitaFine As Date = AGRODATAFINE
    )
        Dim noFiltro = ""
        Dim objImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Dim dt = objImpostazioni_R.Leggi2(1, username, impostazioneCod,
                                          noFiltro, noFiltro, objParametri_Utenti)
        If dt.Rows.Count > 0 Then
            objImpostazioni_W.Modifica2(username, impostazioneCod,
                valore1, valore2, valore3, valore4,
                validitaInizio, validitaFine, objParametri_Utenti
            )
        Else
            objImpostazioni_W.Scrivi2(username, impostazioneCod,
                valore1, valore2, valore3, valore4,
                validitaInizio, validitaFine, objParametri_Utenti
            )
        End If
    End Sub

    Public Sub SalvaInfoAggiuntive(
        username As String, impostazioneCod As Integer,
        ByRef valoreStr As String,
        objParametri_Utenti As AgronicaCoreParametri
    )
        If haFiltroMono(impostazioneCod) Then
            SalvaFiltroMono(username, impostazioneCod, valoreStr, objParametri_Utenti)
        ElseIf impostazioneCod = enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT Then
            gestioneImpostazioneFormatiStampe(username, valoreStr, objParametri_Utenti)
        End If
    End Sub

    ''' <summary>
    ''' Sovrascrive le impostazione dell'utente <c>base</c> impostando i valori delle impostazioni dell'utente <c>template</c>.
    ''' </summary>
    ''' <remarks>
    ''' Tutte le impostazione dell'utente <c>base</c> vengono prima eliminate,
    ''' poi vengono riscritte identiche a quelle dell'utente <c>template</c>.
    ''' </remarks>
    ''' <param name="base">Username dell'utente a cui si vogliono cambiare le impostazioni</param>
    ''' <param name="template">Username dell'utente dal quale si vogliono copiare le impostazioni</param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <param name="usaSingolaTransizione">Se impostato a `True` esegue l'operazione in mod atomico (default)</param>
    Public Sub CopiaImpostazioni(base As IEnumerable(Of String),
                                 template As String,
                                 objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri,
                                 Optional usaSingolaTransizione As Boolean = True)
        Dim objImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim basi = base.GetEnumerator

        Try
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)
            End If

            While basi.MoveNext
                ' Cancello le precedenti impostizazioni dell'utente
                Dim enu = objImpostazioni_R.Leggi2(1, basi.Current, 0, "", "", objParametri_Utenti).Rows.GetEnumerator
                While enu.MoveNext
                    objImpostazioni_W.Cancella2(basi.Current, enu.Current.item("Impostazione_Cod"), "", objParametri_Utenti)
                End While
                ' Copio le impostazioni come dall'altro utente
                enu = objImpostazioni_R.Leggi2(1, template, 0,"", "", objParametri_Utenti).Rows.GetEnumerator
                While enu.MoveNext
                    objImpostazioni_W.Scrivi2(
                        basi.Current, enu.Current.item("Impostazione_Cod"),
                        enu.Current.item("Impostazione_Valore_1"), enu.Current.item("Impostazione_Valore_2"),
                        enu.Current.item("Impostazione_Valore_3"), enu.Current.item("Impostazione_Valore_4"),
                        enu.Current.item("Validita_Inizio"), enu.Current.item("Validita_Fine"),
                        objParametri_Utenti
                    )
                End While
            End While

            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCacheImpostazioni(client, objParametri_Server)

        Catch ex As Exception
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If
            Throw ex
        End Try

    End Sub

    Public Sub CancellaImpostazioni(utentiImpostazioni As IEnumerable(Of utente.Utente),
                                    objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   Optional usaSingolaTransizione As Boolean = True
                                   )
        Dim objImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim utenti = utentiImpostazioni.GetEnumerator

        Try
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)
            End If

            While (utenti.MoveNext)
                Dim impostazioni = utenti.Current.Impostazioni.GetEnumerator
                While (impostazioni.MoveNext)
                    objImpostazioni_W.Cancella2(
                        utenti.Current.Username, impostazioni.Current.Impostazione_Cod,
                        "", objParametri_Utenti
                    )
                    CancellaFiltroMono(
                        utenti.Current.Username,
                        impostazioni.Current.Impostazione_Cod,
                        objParametri_Utenti
                    )
                End While
            End While

            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCacheImpostazioni(client, objParametri_Server)

        Catch ex As Exception
            If usaSingolaTransizione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Indica se un'impostazione ha valori salvati sulla tabella `Utenti_Impostazioni_FiltroMono`.
    ''' 
    ''' Per ora fa riferimento solo all'impostazione 187.
    ''' <br></br>
    ''' <br></br>
    ''' <strong>LISTA DA AGGIORNARE</strong>
    ''' </summary>
    ''' <param name="impostazioneCod"></param>
    ''' <returns>True se l'impostazione è collegata alla tabella FiltoMono, False altrimenti</returns>
    Public Shared Function haFiltroMono(impostazioneCod As Integer) As Boolean
        Dim conFiltroMono As New List(Of Integer) From {
            enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI
        }
        Return conFiltroMono.Contains(impostazioneCod)
    End Function

#Region "Private functions"

    ''' <summary>
    ''' Gestisce il salvatagio della versione concatenata delle impostazioni di formato per le stampe dei documenti
    ''' contabili. Considera i valori delle impostazioni di peso, prezzo e riscontro come concatenati tramite pipe
    ''' ("|"). Salva i valori per le impostazioni SuperUser_LayOut_Prezzo_DDT e SuperUser_LayOut_Riscontrato_DDT,
    ''' lasciando alla funzione chiamate il compito di scrivere il valore per l'impostazione SuperUser_LayOut_Peso_DDT.
    ''' </summary>
    ''' <param name="username">Username dell'utente per cui salvare l'impostazione</param>
    ''' <param name="value">Valore concatenato delle impostazioni, nell'ordine: peso, prezzo, riscontro</param>
    Private Sub gestioneImpostazioneFormatiStampe(username As String, ByRef value As String, objUtenti As AgronicaCoreParametri)
        Dim values = value.Split("|")
        value = If(values.ElementAtOrDefault(0) Is Nothing, 0, values.ElementAt(0))
        If values.ElementAtOrDefault(1) IsNot Nothing Then
            ScriviModificaImpostazioneUtente(
                username, enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT,
                objUtenti, values.ElementAt(1)
            )
        End If
        If values.ElementAtOrDefault(2) IsNot Nothing Then
            ScriviModificaImpostazioneUtente(
                username, enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT,
                objUtenti, values.ElementAt(2)
            )
        End If
    End Sub

    ''' <summary>
    ''' Per le impostazioni che lo richiedono, salva i corrispettivi record
    ''' sulla tabella <code>Utenti_Impostazioni_FiltroMono</code> e aggiorna il
    ''' valore di conseguenza.
    ''' </summary>
    ''' <param name="username"></param>
    ''' <param name="impostazioneCod"></param>
    ''' <param name="valoreStr"></param>
    Private Sub SalvaFiltroMono(
        username As String, impostazioneCod As Integer,
        ByRef valoreStr As String,
        objParametri_Utenti As AgronicaCoreParametri
    )
        If Not haFiltroMono(impostazioneCod) Then
            Return
        End If

        Dim objFiltroMono = New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
        Dim valori As New List(Of BaseCodeDescr)
        Dim noFiltro = ""

        If Not String.IsNullOrWhiteSpace(valoreStr) Then
            Select Case impostazioneCod
                Case enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime
                    ' (18/12/2023) Per evitare che il possibile uso di caratteri
                    ' particolari nelle query provochi problemi, si è deciso lato
                    ' client di usare come separatori ♪ (Alt+13) e ♫ (Alt+14)
                    valoreStr.Split("♫").ToList().
                        ForEach(Sub(i) valori.Add(New BaseCodeDescr With {
                            .codice = i.Split("♪")(0),
                            .descrizione = i.Split("♪")(1)
                        }))
                    ' Su Utenti_Impostazioni salvo sempre stringa vuota
                    valoreStr = ""

                Case Else
                    valoreStr.Split("|").ToList().
                    ForEach(Sub(i) valori.Add(New BaseCodeDescr With {
                        .codice = i.Split("_").ElementAtOrDefault(0),
                        .descrizione = If(i.Split("_").ElementAtOrDefault(1) Is Nothing, "", i.Split("_")(1))
                    }))
                    valoreStr = ""
            End Select
        End If

        objFiltroMono.Cancella(username, impostazioneCod, noFiltro, objParametri_Utenti)

        valori.ForEach(Sub(record) objFiltroMono.Scrivi(
            username, impostazioneCod,
            record.codice, record.descrizione,
            AGRODATAINIZIO, AGRODATAFINE,
            objParametri_Utenti
        ))
    End Sub

    Private Sub CancellaFiltroMono(username As String, impostazioneCod As Integer, objParametri_Utenti As AgronicaCoreParametri)
        Dim objFiltroMono = New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
        Dim noFiltro = ""

        If impostazioneCod <> 0 AndAlso Not haFiltroMono(impostazioneCod) Then
            Return
        End If

        objFiltroMono.Cancella(username, impostazioneCod, noFiltro, objParametri_Utenti)
    End Sub

#End Region

End Class
