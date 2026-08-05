Imports System.Data
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL

Public Class Appezzamento_Piccolo
    Public Sa_Cod, Appezza, Id_reg, Veg_Cod, Cul_Cod, finalita_impianto, Tipologia, Progetto_Cod, campo_cod, N_piante As Integer
    Public Superficie_Appezzamento As Decimal

    Public via_stringa As String

    Public Superficie_IMPIANTO As Decimal

    Public data_inizio, data_fine, Data_semina, Data_Raccolta, Data_fioritura As Date
    Public Piva, app_nome, Progetto_Nome, Disciplinare_Cod, codice_Fiscale_Tecnico, OrganismoReferente As String
    Public Flag_Creato_conSmart As Integer
    Public TipoOperazioneDB_Impianto As enum_TipoOperazioneDB
    Public TipoOperazioneDB_Appezzamento As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura

    Public ImpostaPerScritturaNuovoImpiantoSuAppezzaEsistente As Boolean

    Public SalvaSoloNuovoAppezzamento As Boolean

    Public data_distinta_inizio, data_distinta_fine, codice_appezamento As String

    Public coltura_anno_1, coltura_anno_2, coltura_anno_3, coltura_anno_4 As String

    Public Copertura As Integer
    Public blk_flag As Integer

    Public N As String
    Public P As String
    Public K As String

    Public Regolamento_Concimazioni_Cod As Integer
    Public StatoImpianto As Integer

    '@Paolo: Aggiungo il campo codice terreno per Terreno Nudo
    Public codice_terreno As Integer

    '16/11/2016 x CCCI fede
    Public PianoSemina As String
    Public CodiceContratto As String

    Sub New()
        Superficie_IMPIANTO = 0
        finalita_impianto = 0
        Disciplinare_Cod = 0
        Tipologia = 0
        Data_semina = AGRODATAINIZIO
        Data_Raccolta = AGRODATAFINE
        Progetto_Nome = ""
        blk_flag = 0
        Progetto_Cod = 0
        Appezza = 0
        Id_reg = 0
        ''default nessuna
        Copertura = 0
        codice_appezamento = ""
        coltura_anno_1 = ""
        coltura_anno_2 = ""
        coltura_anno_3 = ""
        coltura_anno_4 = ""
        codice_Fiscale_Tecnico = ""
        via_stringa = ""
        N = ""
        P = ""
        K = ""
        Regolamento_Concimazioni_Cod = 0
        StatoImpianto = 0

        '@Paolo
        codice_terreno = 0
        Data_fioritura = AGRODATAINIZIO
        N_piante = 0
        campo_cod = 0

        PianoSemina = ""
        CodiceContratto = ""

    End Sub


    Public Sub ModificaAppezzamento(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        '_________________ APPEZZAMENTO
        Dim objModificaAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        If Superficie_Appezzamento <> 0 Then
            objModificaAppezzamento.Modifica_Parametrizzata(
                Piva,
                Sa_Cod,
                Appezza,
                "Sup_App",
                Superficie_Appezzamento,
                "",
                objParametri_Server
            )
        End If



        objModificaAppezzamento.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            "via_stringa",
            via_stringa,
            "",
            objParametri_Server
        )



        If app_nome <> "" Then
            objModificaAppezzamento.Modifica_Parametrizzata(
                        Piva,
                        Sa_Cod,
                        Appezza,
                        "App_nome",
                        app_nome,
                        "",
                        objParametri_Server
                    )
        End If


    End Sub


    Public Sub Modifica()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objModificaImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
        objModificaImpreseProgetti.Modifica_Parametrizzata(Piva, Sa_Cod, Appezza, Id_reg, 0, "Progetto_nome",
                                                             Progetto_Nome, " progetto_cod = (select max(progetto_cod) progetto_cod from imprese_progetti i where i.piva = imprese_progetti.piva  and i.sa_cod = imprese_progetti.sa_cod and i.Appezza = imprese_progetti.Appezza and i.id_reg = imprese_progetti.id_reg ) ", objParametri_Server)




        Dim objModificaImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        If Superficie_IMPIANTO <> 0 Then
            objModificaImpianto.Modifica_Parametrizzata(
                Piva,
                Sa_Cod,
                Appezza,
                Id_reg,
                "Sup_Imp",
                Superficie_IMPIANTO,
                "",
                objParametri_Server
            )
        End If

        objModificaImpianto.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            Id_reg,
            "GRVA_Cod_VEG",
            Tipologia,
            "",
            objParametri_Server
        )

        '***** GABRIELE 2020 05 26 *****
        'Destinazione d'uso gestita dopo...
        'If Cul_Cod <> 0 Then

        '    objModificaImpianto.Modifica_Parametrizzata(
        '    Piva,
        '    Sa_Cod,
        '    Appezza,
        '    Id_reg,
        '    "GRFI_COD",
        '    finalita_impianto,
        '    "",
        '    objParametri_Server
        ')

        '    'se è stato impostato un Cul_cod allora occorre rimuovere i dati dell'id_cod da reg_impianti_codici
        '    Dim xDeleteRegImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        '    Dim bDeleteRegImpiantiCodici As Boolean =
        '        xDeleteRegImpiantiCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, 0, " Progetto_Cod = 0 AND  ID_Cod >= 3000 and id_cod < 4000 ", objParametri_Server)

        '    If Not bDeleteRegImpiantiCodici Then
        '        Throw New Exception()
        '    End If

        'End If

        objModificaImpianto.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            Id_reg,
            "GRFI_COD",
            finalita_impianto,
            "",
            objParametri_Server
        )


        objModificaImpianto.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            Id_reg,
            "validita_inizio",
            data_inizio,
            "",
            objParametri_Server
        )

        objModificaImpianto.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            Id_reg,
            "validita_fine",
            data_fine,
            "",
            objParametri_Server
        )


        'cul_cod
        objModificaImpianto.Modifica_Parametrizzata(
                    Piva,
                    Sa_Cod,
                    Appezza,
                    Id_reg,
                    "Cul_cod",
                    Cul_Cod,
                    "",
                    objParametri_Server
                )

        '
        '
        '


        'Valorizzo Codice_Fiscale_Tecnico
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)

        '
        '
        '

        objModificaImpianto.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            Id_reg,
            "CODICE_FISCALE_TECNICO",
            codice_Fiscale_Tecnico,
            "",
            objParametri_Server
        )


        '_________________ APPEZZAMENTO
        Dim objModificaAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        If Superficie_Appezzamento <> 0 Then
            objModificaAppezzamento.Modifica_Parametrizzata(
                Piva,
                Sa_Cod,
                Appezza,
                "Sup_App",
                Superficie_Appezzamento,
                "",
                objParametri_Server
            )
        End If

        objModificaAppezzamento.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            "validita_inizio",
            data_inizio,
            "",
            objParametri_Server
        )

        objModificaAppezzamento.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            "validita_fine",
            data_fine,
            "",
            objParametri_Server
        )


        objModificaAppezzamento.Modifica_Parametrizzata(
            Piva,
            Sa_Cod,
            Appezza,
            "via_stringa",
            via_stringa,
            "",
            objParametri_Server
        )



        If app_nome <> "" Then
            objModificaAppezzamento.Modifica_Parametrizzata(
                        Piva,
                        Sa_Cod,
                        Appezza,
                        "App_nome",
                        app_nome,
                        "",
                        objParametri_Server
                    )
        End If


        'codici anagrafe aggiuntivi .. 

        Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objCodiciAppezz As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W

        'elimino e riscrivo ..

        objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Organismo_Referente,
                      "", objParametri_Server)

        objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_PianoSemina,
                       "", objParametri_Server)

        'cancello destinazione d'uso...
        objCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, 0, " Progetto_Cod = 0 AND Id_Cod >= 3000 AND Id_Cod < 4000", objParametri_Server)

        objCodici.ScrivixProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Organismo_Referente,
                                  OrganismoReferente, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)


        objCodici.ScrivixProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_PianoSemina,
                                  PianoSemina, data_inizio, AGRODATAFINE, objParametri_Server)

        If codice_appezamento <> "" Then
            objCodiciAppezz.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                        "", objParametri_Server)

            objCodiciAppezz.Scrivi(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                    codice_appezamento, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

        End If

        If codice_terreno > 0 Then
            'scrivo destinazione d'uso...
            objCodici.ScrivixProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, codice_terreno,
                                  "", data_inizio, AGRODATAFINE, objParametri_Server)
        End If
    End Sub


    Public Function Salva(ByRef OUTPUT_Piva As String,
                          ByRef OUTPUT_Sa_Cod As Integer,
                          ByRef OUTPUT_Appezza As Integer)


        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim Base, Top, Progressivo As Integer
        CodiciProgressivi.Trova_Base_e_Top(Progressivo, Base, Top, objParametri_Utenti, objParametri_Server)

        Dim strErr As String = ""
        Dim xCentro As XmlElement
        Dim objxml As New AgronicaCoreXML.XML_Anagrafe

        Dim tipoOperazione_Codici As enum_TipoOperazioneDB
        Dim tipoOperazione_codici_Appezzamento As enum_TipoOperazioneDB

        Dim modifica As Boolean = False
        'controllo se sono in modifica 
        If TipoOperazioneDB_Impianto = enum_TipoOperazioneDB.Cancellazione OrElse tipoOperazione_Codici = enum_TipoOperazioneDB.Cancellazione Then
            tipoOperazione_Codici = enum_TipoOperazioneDB.Cancellazione
            TipoOperazioneDB_Impianto = enum_TipoOperazioneDB.Cancellazione
        Else
            If Progetto_Cod <> 0 Then
                modifica = True
                '(18/11/2016 fede) in ogni caso imposto scrittura perché in seguito vengono cancellati e poi dovranno essere riscritti
                'tipoOperazione_Codici = enum_TipoOperazioneDB.Lettura
                tipoOperazione_Codici = enum_TipoOperazioneDB.Scrittura
                TipoOperazioneDB_Impianto = enum_TipoOperazioneDB.Modifica
            Else
                tipoOperazione_Codici = enum_TipoOperazioneDB.Scrittura
                TipoOperazioneDB_Impianto = enum_TipoOperazioneDB.Scrittura
            End If
        End If


        ' VAnni: 15/2/2018: verifico cosa fare sull'appezzamento, se non impostata esternamente imposto la stessa dell'impianto.
        If Not ImpostaPerScritturaNuovoImpiantoSuAppezzaEsistente Then
            TipoOperazioneDB_Appezzamento = TipoOperazioneDB_Impianto
            tipoOperazione_codici_Appezzamento = tipoOperazione_Codici
        Else
            TipoOperazioneDB_Appezzamento = enum_TipoOperazioneDB.Lettura
            tipoOperazione_codici_Appezzamento = enum_TipoOperazioneDB.Lettura
        End If

        If SalvaSoloNuovoAppezzamento Then
            TipoOperazioneDB_Impianto = enum_TipoOperazioneDB.Lettura
            TipoOperazioneDB_Appezzamento = enum_TipoOperazioneDB.Scrittura
            tipoOperazione_codici_Appezzamento = enum_TipoOperazioneDB.Scrittura
            tipoOperazione_Codici = enum_TipoOperazioneDB.Lettura
        End If


        If OrganismoReferente = "" Then
            OrganismoReferente = objParametri_Server.PivaSuperUser
        End If


        Dim VarGlob As New AgronicaCoreXML.XML_Utility

        Dim Dt_Codici_Appezzamento As DataTable = VarGlob.CaricaGriglia_CodiciAppezzamento_for_XML
        Dim Dt_Codici_Impianto As DataTable = VarGlob.CaricaGriglia_CodiciImpianto_for_XML
        Dim Dt_Codici_Progetto As DataTable = VarGlob.CaricaGriglia_CodiciProgetto_for_XML


        '-----------------------
        ' APPEZZAMENTO
        '-----------------------

        'TITOLOPOSSESSO
        VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(
                                        Dt_Codici_Appezzamento,
                                        tipoOperazione_codici_Appezzamento,
                                        Piva,
                                        Sa_Cod,
                                        0,
                                        enum_CodiciAnagrafe.TitoloPossesso,
                                        enum_TitoloPossesso.Proprieta,
                                        AGRODATAINIZIO,
                                        AGRODATAFINE)

        If Flag_Creato_conSmart = True Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(
                                        Dt_Codici_Appezzamento,
                                        tipoOperazione_codici_Appezzamento,
                                        Piva,
                                        Sa_Cod,
                                        0,
                                        enum_CodiciAnagrafe.Flag_Smart_Appezzamento,
                                        True,
                                        AGRODATAINIZIO,
                                        AGRODATAFINE)
        End If

        'MetodoDiProduzione
        VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(
                                       Dt_Codici_Appezzamento,
                                       tipoOperazione_codici_Appezzamento,
                                       Piva,
                                       Sa_Cod,
                                       0,
                                       enum_CodiciAnagrafe.MetodoDiProduzione,
                                       "1",
                                       AGRODATAINIZIO,
                                       AGRODATAFINE)

        If CodiceContratto <> "" Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(
                               Dt_Codici_Appezzamento,
                               tipoOperazione_codici_Appezzamento,
                               Piva,
                               Sa_Cod,
                               0,
                               enum_CodiciAnagrafe.Appezzamento_CodiceContratto,
                               CodiceContratto,
                               AGRODATAINIZIO,
                               AGRODATAFINE)
        End If



        If coltura_anno_1 <> "" Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(Dt_Codici_Appezzamento, enum_TipoOperazioneDB.Scrittura, Piva, Sa_Cod, Appezza,
                                  enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                                  coltura_anno_1,
                               data_inizio, AGRODATAFINE)
        End If
        If coltura_anno_2 <> "" Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(Dt_Codici_Appezzamento, enum_TipoOperazioneDB.Scrittura, Piva, Sa_Cod, Appezza,
                                   enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                                   coltura_anno_2,
                                   data_inizio, AGRODATAFINE)
        End If
        If coltura_anno_3 <> "" Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(Dt_Codici_Appezzamento, enum_TipoOperazioneDB.Scrittura, Piva, Sa_Cod, Appezza,
                                   enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                                   coltura_anno_3,
                                   data_inizio, AGRODATAFINE)
        End If
        If coltura_anno_4 <> "" Then
            VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(Dt_Codici_Appezzamento, enum_TipoOperazioneDB.Scrittura, Piva, Sa_Cod, Appezza,
                                   enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                                   coltura_anno_4,
                                   data_inizio, AGRODATAFINE)
        End If

        'codice_appezzamento
        VarGlob.Inserisci_Riga_Dt_CodiciAppezzamento_for_XML(
                                       Dt_Codici_Appezzamento,
                                       tipoOperazione_codici_Appezzamento,
                                       Piva,
                                       Sa_Cod,
                                       Appezza,
                                       enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                       codice_appezamento,
                                       AGRODATAINIZIO,
                                       AGRODATAFINE)



        If TipoOperazioneDB_Impianto <> enum_TipoOperazioneDB.Lettura Then

            '-----------------------
            ' IMPIANTO
            '-----------------------

            'Impianto_Ibrido
            VarGlob.Inserisci_Riga_Dt_CodiciImpianto_for_XML(
                                       Dt_Codici_Impianto,
                                       tipoOperazione_Codici,
                                       Piva,
                                       Sa_Cod,
                                       0,
                                       0,
                                       enum_CodiciAnagrafe.Impianto_Ibrido,
                                       "0",
                                       data_inizio,
                                       AGRODATAFINE)

            If Flag_Creato_conSmart = True Then
                VarGlob.Inserisci_Riga_Dt_CodiciImpianto_for_XML(
                                          Dt_Codici_Impianto,
                                          tipoOperazione_Codici,
                                          Piva,
                                          Sa_Cod,
                                          0,
                                          0,
                                          enum_CodiciAnagrafe.Flag_Smart_Impianto,
                                          True,
                                          data_inizio,
                                          AGRODATAFINE)
            End If

            'Impianto_Germinabilita
            VarGlob.Inserisci_Riga_Dt_CodiciImpianto_for_XML(
                                        Dt_Codici_Impianto,
                                        tipoOperazione_Codici,
                                        Piva,
                                        Sa_Cod,
                                        0,
                                        0,
                                        enum_CodiciAnagrafe.Impianto_Germinabilita,
                                        "100",
                                        data_inizio,
                                        AGRODATAFINE)


            If IsNumeric(codice_terreno) AndAlso codice_terreno <> 0 Then
                VarGlob.Inserisci_Riga_Dt_CodiciImpianto_for_XML(Dt_Codici_Impianto, enum_TipoOperazioneDB.Scrittura, Piva, Sa_Cod, Appezza, Id_reg,
                                       codice_terreno,
                                       "",
                                       data_inizio, AGRODATAFINE)
            End If


            '-----------------------
            ' ESERCIZIO
            '-----------------------
            'lo scrivo sempre
            'Impianto_Cooperativa
            VarGlob.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                        Dt_Codici_Progetto,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Piva,
                                        Sa_Cod,
                                        Appezza,
                                        Id_reg,
                                        Progetto_Cod,
                                        enum_CodiciAnagrafe.Organismo_Referente,
                                        OrganismoReferente,
                                        data_inizio,
                                        AGRODATAFINE)




            If N <> "" Then
                VarGlob.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                     Dt_Codici_Progetto,
                                     enum_TipoOperazioneDB.Scrittura,
                                     Piva,
                                     Sa_Cod,
                                     Appezza,
                                     Id_reg,
                                     Progetto_Cod,
                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                     N,
                                     AGRODATAINIZIO,
                                     AGRODATAFINE)
            End If

            If P <> "" Then
                VarGlob.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                     Dt_Codici_Progetto,
                                     enum_TipoOperazioneDB.Scrittura,
                                     Piva,
                                     Sa_Cod,
                                     Appezza,
                                     Id_reg,
                                     Progetto_Cod,
                                     enum_CodiciAnagrafe.Impianto_LimiteP,
                                     P,
                                     AGRODATAINIZIO,
                                     AGRODATAFINE)
            End If

            If K <> "" Then
                VarGlob.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                     Dt_Codici_Progetto,
                                     enum_TipoOperazioneDB.Scrittura,
                                     Piva,
                                     Sa_Cod,
                                     Appezza,
                                     Id_reg,
                                     Progetto_Cod,
                                     enum_CodiciAnagrafe.Impianto_LimiteK,
                                     K,
                                     AGRODATAINIZIO,
                                     AGRODATAFINE)
            End If

            If PianoSemina <> "" Then
                VarGlob.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                                             Dt_Codici_Progetto,
                                                             enum_TipoOperazioneDB.Scrittura,
                                                             Piva,
                                                             Sa_Cod,
                                                             Appezza,
                                                             Id_reg,
                                                             Progetto_Cod,
                                                             enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                             PianoSemina,
                                                             AGRODATAINIZIO,
                                                             AGRODATAFINE)
            End If
        End If


        If modifica Then
            'cancello 


            Dim objCodiciAppe As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.TitoloPossesso,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Flag_Smart_Appezzamento,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.MetodoDiProduzione,
                                 "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                                    "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Appezzamento_CodiceContratto,
                                  "", objParametri_Server)
            objCodiciAppe.Cancella(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                   "", objParametri_Server)

            Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

            objCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, 0,
                                  " Reg_Impianti_Codici.id_cod >= 3000 AND Reg_Impianti_Codici.id_cod < 4000 ", objParametri_Server)

            objCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, enum_CodiciAnagrafe.Impianto_Ibrido,
                                  "", objParametri_Server)
            objCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, enum_CodiciAnagrafe.Flag_Smart_Impianto,
                                  "", objParametri_Server)
            objCodici.Cancella(Piva, Sa_Cod, Appezza, Id_reg, enum_CodiciAnagrafe.Impianto_Germinabilita,
                                    "", objParametri_Server)

            objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Organismo_Referente,
                      "", objParametri_Server)

            objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_LimiteN,
                                       "", objParametri_Server)

            objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_LimiteP,
                                       "", objParametri_Server)

            objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_LimiteK,
                                       "", objParametri_Server)

            objCodici.CancellaxProgetto(Piva, Sa_Cod, Appezza, Id_reg, Progetto_Cod, enum_CodiciAnagrafe.Impianto_PianoSemina,
                           "", objParametri_Server)

        End If

        If data_distinta_inizio = "" Then
            data_distinta_inizio = data_inizio
        End If
        If data_distinta_fine = "" Then
            data_distinta_fine = data_fine
        Else
            If data_distinta_fine = AGRODATAFINE Then
                data_distinta_fine = "30/10/" & (CDate(data_distinta_inizio).Year + 1)
            End If
        End If
        'identifico la copertura
        Dim objspec As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim dt_spe As DataTable
        dt_spe = objspec.Leggi(Veg_Cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim objcop As New AgronicaCoreMetaSchemaDAL.Copertura_R
        dt_spe = objcop.Leggi(dt_spe.Rows(0).Item("Gru_Cod"), 0, "Nessuna", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        'copertura
        If Copertura = 0 Then
            Copertura = dt_spe.Rows(0).Item("Cop_cod")
        End If

        If IsNothing(Progetto_Nome) Then
            Progetto_Nome = ""
        End If
        If Progetto_Nome = "" Then
            Progetto_Nome = "lotto " & CDate(data_distinta_inizio).Year & " - " & CDate(data_distinta_fine).Year
        End If


        Dim Progetto_Des As String = "Distinta di produzione agricola: " & Progetto_Nome

        'controllo dpicod
        Dim privato_pubblico As Integer = 0
        Dim Disciplinare_Cod_Split As String() = Disciplinare_Cod.Split("/")

        If Disciplinare_Cod.Length > 1 Then
            If Disciplinare_Cod_Split.Length = 2 Then
                privato_pubblico = Disciplinare_Cod.Split("/")(1)
                Disciplinare_Cod = Disciplinare_Cod.Split("/")(0)
            Else
                If Not String.IsNullOrEmpty(Disciplinare_Cod) AndAlso Disciplinare_Cod <> 0 Then
                    privato_pubblico = 1
                End If
            End If
        End If

        'If Disciplinare_Cod.Length > 1 Then
        '    privato_pubblico = Disciplinare_Cod.Split("/")(1)
        '    Disciplinare_Cod = Disciplinare_Cod.Split("/")(0)
        'Else
        '    Disciplinare_Cod = 0
        'End If

        'If Disciplinare_Cod <> 0 Then
        '    privato_pubblico = 1
        'End If


        'Valorizzo Codice_Fiscale_Tecnico
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)


        xCentro = objxml.XML_Appezzamenti(
                Log_Errori:=strErr,
             XmlDoc:=Nothing,
             BaseCode:=Base,
             TopCode:=Top,
             Flag_CreaImpianto:=True,
             DT_Codici_Appezzamento:=Dt_Codici_Appezzamento,
             DT_Codici_Impianto:=Dt_Codici_Impianto,
             DT_Codici_Progetto:=Dt_Codici_Progetto,
             TipoOperazioneDB_Appezzamento:=TipoOperazioneDB_Appezzamento,
             TipoOperazioneDB_Impianto:=TipoOperazioneDB_Impianto,
             TipoOperazioneDB_Progetto:=TipoOperazioneDB_Impianto,
             Piva_SuperUser:=objParametri_Server.PivaSuperUser,
             Piva:=Piva,
             Sa_Cod:=Sa_Cod,
             Appezza:=Appezza,
             Sup_App:=Superficie_Appezzamento,
             App_Nome:=app_nome,
             Campo_Cod:=campo_cod,
             Validita_Inizio_Appezza:=data_inizio,
             Validita_Fine_Appezza:=data_fine,
             Id_Reg:=Id_reg,
             Id_Consociazione:=0,
             Sup_Imp:=Superficie_IMPIANTO,
             Cul_Cod:=Cul_Cod,
             Validita_Inizio_Impianto:=data_inizio,
             Validita_Fine_Impianto:=data_fine,
             Data:=data_inizio,
             Progetto_Cod:=Progetto_Cod,
             Progetto_Nome:=Progetto_Nome,
             Progetto_Des:=Progetto_Des,
             Cau_Progetto:=CAU_PROGETTO_PRODUZIONE,
             Validita_Inizio_Progetto:=data_distinta_inizio,
             Validita_Fine_Progetto:=data_distinta_fine,
             Data_App:="0",
             Data_Inizio:="0",
             Data_Fine:="0",
             Ep_Camp:="0",
             X:=0,
             Y:=0,
             Zslm:=0,
             Esposiz:="...",
             Pende:=0,
             Ubicazione:="...",
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
             K2OAss_Data:="0",
             MatOrg:=0,
             MatOrg_Data:="0",
             NOtot_Data:="0",
             NOtot:=0,
             P2O5Ass_Data:="0",
             Suolo_CodAttri:="",
             Campo_Spia:=0,
             Campo_Spia_Area:=0,
             Cs_SIPI:="",
             Prossimo:=0,
             Blk_Flag:=blk_flag,
             Blk_Inizio_Data:=Date.Now,
             Blk_Inizio_Username:=objParametri_Server.UtenteCodFiscale,
             Blk_Inizio_Note:="",
             Blk_Fine_Data:=AGRODATAFINE,
             Blk_Fine_Username:="",
             Blk_Fine_Note:="",
             Grva_Cod_Veg:=Tipologia,
             Grfi_Cod:=finalita_impianto,
             Cod_Resp:=0,
             Cod_Ente:=0,
             Campo_Spia_Impianto:=0,
             Data_Raccolta:="0",
             Produzione:=0,
             ResaPrevista:=0,
             ResaEffettiva:=0,
             Scarto:=0,
             Ind_Mat_Cod:=0,
             Ind_Mat_Ril:="0",
             Sta_Ter:="",
             Cop_DI:="0",
             Cop_DF:="0",
             Tra_Fila:=0,
             Su_Fila:=0,
             P_HA:=N_piante,
             Foral_Cod:=-1,
             Setup_Cod:="-1",
             Port_Cod:=-1,
             Imp_Cod:=-1,
             Stru_Prot:=0,
             Pro_Pag:=0,
             Seme_Q:=0,
             Seme_T:=0,
             Seme_P:=0,
             Seme_D:=0,
             Stato_Residui:="",
             Tecn_Cod:=-1,
             Denitrificazione:=0,
             Volatilizzazione:=0,
             ProfonditaLav:=0,
             Id_Campo:=0,
             Su_Cod:=-1,
             Cop_Cod:=Copertura,
             Cover:=0,
             Monitorato:=0,
             Codice_Ficale_Tecnico:=codice_Fiscale_Tecnico,
             Regolamento:=enum_Cod_Regolamento.Regolamento_Nessuno,
             Finanziamento:=0,
             Data_Conversione:="0",
             ProvenienzaSeme:=0,
             Cod_Contratto:=0,
             Cod_Conto:=0,
             Ricavi_Previsti:=0,
             Produzione_Prevista:=0,
             Giudizio:="",
             Veg_Cod:=0,
             Grfi_Cod_Progetto:=0,
             CSProgetto_Cod:=0,
             Stato_Impianto:=StatoImpianto,
             Regolamento_Cod:=enum_Cod_Regolamento.Regolamento_Nessuno,
             Disciplinare_Cod:=Disciplinare_Cod,
             Disciplinare_PrivatoPubblico:=privato_pubblico,
             Regolamento_Concimazioni_Cod:=Regolamento_Concimazioni_Cod,
             Data_Inizio_Prevista:=Data_semina,
             Data_Fine_Prevista:=Data_Raccolta,
             via_stringa:=via_stringa,
             Data_Fioritura_Prevista:=Data_fioritura)






        Dim objApp As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        'Dim out_Piva, out_Sa_Cod, out_Appezza As String

        Dim risp As Boolean

        risp = objApp.Appezzamento_Scrivi(xCentro.OuterXml, OUTPUT_Piva, OUTPUT_Sa_Cod, OUTPUT_Appezza, objParametri_Server, objParametri_Utenti)


        ' VAnni: 13/7/2018: se si imposta una finestra temporale che impedisce il recupero dell'impianto allora non viene memorizzato il dato dell'id impianto
        ' la query restituisce valore vuoto .. escludo il filtro temporale e lo riscrivo a lettura avvenuta .. 
        ' VAnni: 8/1/2020: bug fix nel caso in cui si stia inserendo un nuovo impianto su appezza esistente, 
        '   se ci sono più impianti sarà ovviamente l'ultimo scritto quindi imposto ordinamento " id_reg desc ".

        Dim tempFiltroDataDa As Date
        Dim tempFiltroDataA As Date

        tempFiltroDataDa = objParametri_Server.FinestraTemporaleInizio
        tempFiltroDataA = objParametri_Server.FinestraTemporaleFine

        objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
        objParametri_Server.FinestraTemporaleFine = AGRODATAFINE


        Piva = OUTPUT_Piva
        Sa_Cod = OUTPUT_Sa_Cod
        Appezza = OUTPUT_Appezza
        Dim objRegImpiant As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dt As DataTable
        dt = objRegImpiant.Leggi(Piva, Sa_Cod, Appezza, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", " id_reg desc ", objParametri_Server)
        If dt.Rows.Count > 0 Then
            Id_reg = dt.Rows(0).Item("Id_Reg")
        End If


        objParametri_Server.FinestraTemporaleInizio = tempFiltroDataDa
        objParametri_Server.FinestraTemporaleFine = tempFiltroDataA

        Return risp

    End Function

End Class
