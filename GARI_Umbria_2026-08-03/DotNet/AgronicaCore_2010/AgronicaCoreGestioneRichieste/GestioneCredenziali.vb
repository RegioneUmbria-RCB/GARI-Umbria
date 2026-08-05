
Imports System.Xml
Imports System.Data
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Configuration


Public Class GestioneCredenziali


    '###########################################################################
    Public Sub GestioneCredenziali(ByVal Str_XML_Credenziali_Criptata As String, _
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Username_Gias As String = ""
        Dim Password_Gias As String = ""
        'Dim Connessione_Server As String = ""
        'Dim Connessione_Utenti As String = ""
        'Dim PercorsoConnessioni As String = ""
        Dim PathDirFileLog As String = ""
        Dim NomeFileLog As String = ""
        Dim PathDirFileXmlPrivato As String = ""
        Dim FlagSalvaFileXmlPrivato As Boolean = False
        Dim SuperUser_Username As String = ""
        Dim SuperUser_CodFiscale As String = ""
        Dim SuperUser_Password As String = ""
        Dim Utente_CodFiscale As String = ""
        Dim ProgressivoGIAS As Integer = 0
        Dim Id_Servizio As Integer = 0

        Dim Super_Server As Boolean = False
        Dim Super_Server_Provider As String = ""
        Dim Super_Server_Server As String = ""
        Dim Super_Server_DB As String = ""
        Dim Super_Server_UserId As String = ""
        Dim Super_Server_Password As String = ""
        Dim Super_Server_FiltroAgg As String = ""
        Dim PivaSuperUser_xLetturaSuperServer As String = ""
        Dim StrConnessioneServer As String = ""
        Dim StrConnessioneUtenti As String = ""

        'RecuperaCredenziali_OLD(Str_XML_Credenziali_Criptata, _
        '                    Username_Gias, _
        '                    Password_Gias, _
        '                    Connessione_Server, _
        '                    Connessione_Utenti)

        RecuperaCredenziali(Str_XML_Credenziali_Criptata, _
                            Username_Gias, _
                            Password_Gias, _
                            PivaSuperUser_xLetturaSuperServer, _
                            Super_Server, _
                            Super_Server_Provider, _
                            Super_Server_Server, _
                            Super_Server_DB, _
                            Super_Server_UserId, _
                            Super_Server_Password, _
                            Super_Server_FiltroAgg, _
                            StrConnessioneServer, _
                            StrConnessioneUtenti)

        'PercorsoConnessioni
        Try
            RecuperaWebConfig(PathDirFileLog, _
                                        NomeFileLog, _
                                        PathDirFileXmlPrivato, _
                                        FlagSalvaFileXmlPrivato)
        Catch ex As Exception

        End Try


        GestioneSuperServer(objParametri_Server, _
                            objParametri_Utenti, _
                            Super_Server, _
                            Super_Server_Provider, _
                            Super_Server_Server, _
                            Super_Server_DB, _
                            Super_Server_UserId, _
                            Super_Server_Password, _
                            Super_Server_FiltroAgg, _
                            StrConnessioneServer, _
                            StrConnessioneUtenti, _
                            PathDirFileLog, _
                           NomeFileLog, _
                           Username_Gias, _
                            PivaSuperUser_xLetturaSuperServer)



        VerificaUtente1(Username_Gias, _
                        Password_Gias, _
                        objParametri_Utenti, _
                         SuperUser_Username, _
                         SuperUser_Password, _
                        SuperUser_CodFiscale, _
                        Utente_CodFiscale, _
                        ProgressivoGIAS, _
                        Id_Servizio)

        'visto che quando ho creato l'objparametri non c'erano i dati completi di utente e superuser
        'dopo aver fatto la lettura dei dati, aggiorno le info sull'objparametri

        objParametri_Server.UsernameOperazione = Utente_CodFiscale
        objParametri_Server.UtenteCodFiscale = Utente_CodFiscale
        objParametri_Server.SuperUserUsername = SuperUser_Username
        objParametri_Server.PivaSuperUser = SuperUser_CodFiscale

        objParametri_Utenti.UsernameOperazione = Utente_CodFiscale
        objParametri_Utenti.UtenteCodFiscale = Utente_CodFiscale
        objParametri_Utenti.SuperUserUsername = SuperUser_Username
        objParametri_Utenti.PivaSuperUser = SuperUser_CodFiscale

        ''query sull'utente
        'VerificaUtente(Username_Gias, _
        '                Password_Gias, _
        '                PathDirFileLog, _
        '                NomeFileLog, _
        '                PercorsoConnessioni, _
        '                Connessione_Utenti, _
        '                 SuperUser_Username, _
        '                SuperUser_CodFiscale, _
        '                Utente_CodFiscale, _
        '                ProgressivoGIAS, _
        '                Id_Servizio)

        'Crea_ObjParametri(objParametri_Server, _
        '                    objParametri_Utenti, _
        '                    PercorsoConnessioni, _
        '                    Connessione_Server, _
        '                    Connessione_Utenti, _
        '                    PathDirFileLog, _
        '                    NomeFileLog, _
        '                    SuperUser_Username, _
        '                    SuperUser_CodFiscale, _
        '                    Username_Gias, _
        '                    Utente_CodFiscale)


        Dim Piva_CAC_Codifica_ProdottiAziendali As String
        Piva_CAC_Codifica_ProdottiAziendali = objParametri_Server.PivaSuperUser

        ''query di lettura per recupero di questi dati
        'Dim Rag_Soc_Fornitore As String = ""
        'Dim Cod_Risum_Fornitore As Integer = 0
        'Dim Cod_IndirizzoRisum_Fornitore As Integer = 0
        'Dim objContSU As New AgronicaCoreContabDAL.Contabilita_R

        'objContSU.DatiSuperUserFornitore(Rag_Soc_Fornitore, _
        '                                    Cod_Risum_Fornitore, _
        '                                    Cod_IndirizzoRisum_Fornitore, _
        '                                    objParametri_Server)

        'objDati = New RaccoglitoreDati(Piva_CAC_Codifica_ProdottiAziendali, _
        '                               ProgressivoGIAS, _
        '                               Rag_Soc_Fornitore, _
        '                               Cod_Risum_Fornitore, _
        '                                Cod_IndirizzoRisum_Fornitore, _
        '                                Id_Servizio, _
        '                                PathDirFileXmlPrivato, _
        '                                FlagSalvaFileXmlPrivato)

        'objDati = New RaccoglitoreDati(ProgressivoGIAS, _
        '                                Id_Servizio, _
        '                                PathDirFileXmlPrivato, _
        '                                FlagSalvaFileXmlPrivato)



    End Sub


    '#############################################################
    Private Sub GestioneSuperServer(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByVal Super_Server As Boolean, _
                                    ByVal Super_Server_Provider As String, _
                                    ByVal Super_Server_Server As String, _
                                    ByVal Super_Server_DB As String, _
                                    ByVal Super_Server_UserId As String, _
                                    ByVal Super_Server_Password As String, _
                                    ByVal Super_Server_FiltroAgg As String, _
                                    ByVal StrConnessioneServer As String, _
                                    ByVal StrConnessioneUtenti As String, _
                                    ByVal PathDirFileLog As String, _
                                    ByVal NomeFileLog As String, _
                                    ByVal Username_Gias As String, _
                                    ByVal PivaSuperUser_xLetturaSuperServer As String)

        If StrConnessioneServer <> "" And StrConnessioneUtenti <> "" Then

            'creo objparametri con i dati dell'utente e le stringhe di connessione passate
            Crea_ObjParametri(objParametri_Server, _
                                objParametri_Utenti, _
                                StrConnessioneServer, _
                                StrConnessioneUtenti, _
                                PathDirFileLog, _
                                NomeFileLog, _
                                "", _
                                "", _
                                Username_Gias, _
                                "")

        ElseIf Not IsNothing(Super_Server) And Super_Server_Provider <> "" And _
            Super_Server_Server <> "" And Super_Server_DB <> "" And _
            Super_Server_UserId <> "" And Super_Server_Password <> "" Then

            'creo gli objparametri a partire da i dati del super server

            Dim objConn As New AgronicaCoreGestioneRichieste.Inizializzatore
            Dim objParametriSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
            objConn.AvviamentoConSuperServer(1, _
                                            Super_Server, _
                                            Super_Server_Provider, _
                                            Super_Server_Server, _
                                            Super_Server_DB, _
                                            Super_Server_UserId, _
                                            Super_Server_Password, _
                                             PathDirFileLog, _
                                             NomeFileLog, _
                                             PivaSuperUser_xLetturaSuperServer, _
                                               "", _
                                               "", _
                                               Username_Gias, _
                                               "", _
                                               Super_Server_FiltroAgg, _
                                               " Progressivo DESC ", _
                                                objParametriSuperServer, _
                                               objParametri_Server, _
                                               objParametri_Utenti)

        Else
            Throw New Exception("Dati mancanti epr creazione objparametri.")
        End If

    End Sub


    '###########################################################################
    Private Sub RecuperaCredenziali(ByVal Str_XML_Credenziali_Criptata As String, _
                                    ByRef Username_Gias As String, _
                                    ByRef Password_Gias As String, _
                                    ByRef PivaSuperUser_xLetturaSuperServer As String, _
                                    ByRef Super_Server As Boolean, _
                                    ByRef Super_Server_Provider As String, _
                                    ByRef Super_Server_Server As String, _
                                    ByRef Super_Server_DB As String, _
                                    ByRef Super_Server_UserId As String, _
                                    ByRef Super_Server_Password As String, _
                                    ByRef Super_Server_FiltroAgg As String, _
                                    ByRef StrConnessioneServer As String, _
                                    ByRef StrConnessioneUtenti As String)

        'tutto minuscolo!!!!!!!!!!!!
        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <PivaSuperUser></PivaSuperUser>
        '           <Super_Server></Super_Server>
        '           <Super_Server_Provider></Super_Server_Provider>
        '           <Super_Server_Server></Super_Server_Server>
        '           <Super_Server_DB></Super_Server_DB>
        '           <Super_Server_UserId></Super_Server_UserId>
        '           <Super_Server_Password></Super_Server_Password>
        '           <StrConnessioneServer></Super_Server_Password>
        '           <StrConnessioneUtenti></Super_Server_Password>
        '   </CREDENZIALI>

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
        Dim Str_XML_Credenziali As String

        Str_XML_Credenziali = objSecurity.Stringa_Decodifica_WebService(Str_XML_Credenziali_Criptata)

        Dim XmlDoc As New XmlDocument
        Dim XML_Credenziali As XmlElement
        XmlDoc.LoadXml(Str_XML_Credenziali)

        XML_Credenziali = XmlDoc.SelectSingleNode("credenziali")

        If Not IsNothing(XML_Credenziali.SelectSingleNode("user".ToLower)) Then
            Username_Gias = XML_Credenziali.SelectSingleNode("user".ToLower).InnerText
        Else
            Username_Gias = ""
            Throw New Exception("Credenziali: Username non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("pwd".ToLower)) Then
            Password_Gias = XML_Credenziali.SelectSingleNode("pwd".ToLower).InnerText
        Else
            Password_Gias = ""
            Throw New Exception("Credenziali: Password non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("PivaSuperUser".ToLower)) Then
            PivaSuperUser_xLetturaSuperServer = XML_Credenziali.SelectSingleNode("PivaSuperUser".ToLower).InnerText
        Else
            PivaSuperUser_xLetturaSuperServer = ""
            Throw New Exception("Credenziali: PivaSuperUser non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server".ToLower)) Then
            Super_Server = XML_Credenziali.SelectSingleNode("Super_Server".ToLower).InnerText
        Else
            Super_Server = False
            Throw New Exception("Credenziali: Super_Server non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_Provider".ToLower)) Then
            Super_Server_Provider = XML_Credenziali.SelectSingleNode("Super_Server_Provider".ToLower).InnerText
        Else
            Super_Server_Provider = ""
            Throw New Exception("Credenziali: Super_Server_Provider non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_Server".ToLower)) Then
            Super_Server_Server = XML_Credenziali.SelectSingleNode("Super_Server_Server".ToLower).InnerText
        Else
            Super_Server_Server = ""
            Throw New Exception("Credenziali: Super_Server_Server non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_DB".ToLower)) Then
            Super_Server_DB = XML_Credenziali.SelectSingleNode("Super_Server_DB".ToLower).InnerText
        Else
            Super_Server_DB = ""
            Throw New Exception("Credenziali: Super_Server_DB non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_UserId".ToLower)) Then
            Super_Server_UserId = XML_Credenziali.SelectSingleNode("Super_Server_UserId".ToLower).InnerText
        Else
            Super_Server_UserId = ""
            Throw New Exception("Credenziali: Super_Server_UserId non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_Password".ToLower)) Then
            Super_Server_Password = XML_Credenziali.SelectSingleNode("Super_Server_Password".ToLower).InnerText
        Else
            Super_Server_Password = ""
            Throw New Exception("Credenziali: Super_Server_Password non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("Super_Server_FiltroAgg".ToLower)) Then
            Super_Server_FiltroAgg = XML_Credenziali.SelectSingleNode("Super_Server_FiltroAgg".ToLower).InnerText
        Else
            Super_Server_FiltroAgg = ""
            Throw New Exception("Credenziali: Super_Server_FiltroAgg non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("StrConnessioneServer".ToLower)) Then
            StrConnessioneServer = XML_Credenziali.SelectSingleNode("StrConnessioneServer".ToLower).InnerText
        Else
            StrConnessioneServer = ""
            Throw New Exception("Credenziali: StrConnessioneServer non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("StrConnessioneUtenti".ToLower)) Then
            StrConnessioneUtenti = XML_Credenziali.SelectSingleNode("StrConnessioneUtenti".ToLower).InnerText
        Else
            StrConnessioneUtenti = ""
            Throw New Exception("Credenziali: StrConnessioneUtenti non inviata!")
        End If


    End Sub


    '###########################################################################
    Private Sub RecuperaCredenziali_OLD(ByVal Str_XML_Credenziali_Criptata As String, _
                                        ByRef Username_Gias As String, _
                                        ByRef Password_Gias As String, _
                                        ByRef Connessione_Server As String, _
                                        ByRef Connessione_Utenti As String)

        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <CN_SERVER></CN_SERVER>
        '           <CN_UTENTI></CN_UTENTI>
        '   </CREDENZIALI>

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
        Dim Str_XML_Credenziali As String

        Str_XML_Credenziali = objSecurity.Stringa_Decodifica_WebService(Str_XML_Credenziali_Criptata)

        Dim XmlDoc As New XmlDocument
        Dim XML_Credenziali As XmlElement
        XmlDoc.LoadXml(Str_XML_Credenziali)

        XML_Credenziali = XmlDoc.SelectSingleNode("credenziali")

        If Not IsNothing(XML_Credenziali.SelectSingleNode("user".ToLower)) Then
            Username_Gias = XML_Credenziali.SelectSingleNode("user".ToLower).InnerText
        Else
            Username_Gias = ""
            Throw New Exception("Credenziali: Username non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("pwd".ToLower)) Then
            Password_Gias = XML_Credenziali.SelectSingleNode("pwd".ToLower).InnerText
        Else
            Password_Gias = ""
            Throw New Exception("Credenziali: Password non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("cn_server".ToLower)) Then
            Connessione_Server = XML_Credenziali.SelectSingleNode("cn_server".ToLower).InnerText
        Else
            Connessione_Server = ""
            Throw New Exception("Credenziali: Connessione_Server non inviata!")
        End If

        If Not IsNothing(XML_Credenziali.SelectSingleNode("cn_utenti".ToLower)) Then
            Connessione_Utenti = XML_Credenziali.SelectSingleNode("cn_utenti".ToLower).InnerText
        Else
            Connessione_Utenti = ""
            Throw New Exception("Credenziali: Connessione_Utenti non inviata!")
        End If


    End Sub

    '###########################################################################
    Private Sub RecuperaWebConfig(ByRef PathDirFileLog As String, _
                                   ByRef NomeFileLog As String, _
                                   ByRef PathDirFileXmlPrivato As String, _
                                   ByRef FlagSalvaFileXmlPrivato As Boolean)

        If Not IsNothing(ConfigurationManager.AppSettings("DirectoryLOG")) AndAlso
           ConfigurationManager.AppSettings("DirectoryLOG") <> "" Then
            PathDirFileLog = ConfigurationManager.AppSettings("DirectoryLOG")

            If PathDirFileLog.EndsWith("\") = False Then
                PathDirFileLog += "\"
            End If
        Else
            Throw New Exception("Recupera configurazione web.config: Percorso file log non valorizzato.")
        End If

        If Not IsNothing(ConfigurationManager.AppSettings("FileLOG")) AndAlso
           ConfigurationManager.AppSettings("FileLOG") <> "" Then
            NomeFileLog = ConfigurationManager.AppSettings("FileLOG")
        Else
            Throw New Exception("Recupera configurazione web.config: nome file log non valorizzato.")
        End If

        If Not IsNothing(ConfigurationManager.AppSettings("FlagSalvaFileXmlPrivato")) AndAlso
           ConfigurationManager.AppSettings("FlagSalvaFileXmlPrivato") <> "" Then
            FlagSalvaFileXmlPrivato = ConfigurationManager.AppSettings("FlagSalvaFileXmlPrivato")

            If FlagSalvaFileXmlPrivato = True Then
                If Not IsNothing(ConfigurationManager.AppSettings("PathDirFileXmlPrivato")) AndAlso
                   ConfigurationManager.AppSettings("PathDirFileXmlPrivato") <> "" Then
                    PathDirFileXmlPrivato = ConfigurationManager.AppSettings("PathDirFileXmlPrivato")

                    If PathDirFileXmlPrivato.EndsWith("\") = False Then
                        PathDirFileXmlPrivato += "\"
                    End If
                Else
                    Throw New Exception("Recupera configurazione web.config: Percorso file xml privato non valorizzato.")
                End If
            Else
                PathDirFileXmlPrivato = ""
            End If
        Else
            Throw New Exception("Recupera configurazione web.config: flag salvataggio file xml privato non valorizzato.")
        End If

    End Sub

    '###########################################################################
    Private Sub Crea_ObjParametri(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByVal Stringa_Connessione_Server As String, _
                                    ByVal Stringa_Connessione_Utenti As String, _
                                    ByVal PathDirFileLog As String, _
                                    ByVal NomeFileLog As String, _
                                    ByVal SuperUser_Username As String, _
                                    ByVal SuperUser_CodFiscale As String, _
                                    ByVal Import_Username As String, _
                                    ByVal Import_CodFiscale As String _
                                    )

        Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri( _
                                                        AGRODATAINIZIO, _
                                                        AGRODATAFINE, _
                                                        enumCancellazioneLogica.CancellazioneFisica, _
                                                        enumVisibilita.Visibilita_SoloNonCancellati, _
                                                        PathDirFileLog, _
                                                        NomeFileLog, _
                                                        SuperUser_Username, _
                                                        SuperUser_CodFiscale, _
                                                        Import_Username, _
                                                        Import_CodFiscale, _
                                                        Stringa_Connessione_Server)

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri( _
                                                            AGRODATAINIZIO, _
                                                            AGRODATAFINE, _
                                                            enumCancellazioneLogica.CancellazioneFisica, _
                                                            enumVisibilita.Visibilita_SoloNonCancellati, _
                                                            PathDirFileLog, _
                                                            NomeFileLog, _
                                                            SuperUser_Username, _
                                                            SuperUser_CodFiscale, _
                                                            Import_Username, _
                                                            Import_CodFiscale, _
                                                            Stringa_Connessione_Utenti)

    End Sub

    ''###########################################################################
    'Private Sub Crea_ObjParametri_OLD(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByVal PercorsoConnessioni As String, _
    '                                    ByVal Connessione_Server As String, _
    '                                    ByVal Connessione_Utenti As String, _
    '                                    ByVal PathDirFileLog As String, _
    '                                    ByVal NomeFileLog As String, _
    '                                    ByVal SuperUser_Username As String, _
    '                                    ByVal SuperUser_CodFiscale As String, _
    '                                    ByVal Import_Username As String, _
    '                                    ByVal Import_CodFiscale As String _
    '                                    )

    '    Dim Stringa_Connessione_Server As String
    '    Dim Stringa_Connessione_Utenti As String
    '    Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider

    '    Stringa_Connessione_Server = objAgronicaCore.FindIniConnessioni(PercorsoConnessioni, Connessione_Server)
    '    Stringa_Connessione_Utenti = objAgronicaCore.FindIniConnessioni(PercorsoConnessioni, Connessione_Utenti)

    '    'Dim objParametriHLP As New AgronicaCoreDataProvider.AgronicaCoreParametri_Helper

    '    objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri( _
    '                                                    AGRODATAINIZIO, _
    '                                                    AGRODATAFINE, _
    '                                                    enumCancellazioneLogica.CancellazioneFisica, _
    '                                                    enumVisibilita.Visibilita_SoloNonCancellati, _
    '                                                    PathDirFileLog, _
    '                                                    NomeFileLog, _
    '                                                    SuperUser_Username, _
    '                                                    SuperUser_CodFiscale, _
    '                                                    Import_Username, _
    '                                                    Import_CodFiscale, _
    '                                                    Stringa_Connessione_Server)

    '    objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri( _
    '                                                        AGRODATAINIZIO, _
    '                                                        AGRODATAFINE, _
    '                                                        enumCancellazioneLogica.CancellazioneFisica, _
    '                                                        enumVisibilita.Visibilita_SoloNonCancellati, _
    '                                                        PathDirFileLog, _
    '                                                        NomeFileLog, _
    '                                                        SuperUser_Username, _
    '                                                        SuperUser_CodFiscale, _
    '                                                        Import_Username, _
    '                                                        Import_CodFiscale, _
    '                                                        Stringa_Connessione_Utenti)

    'End Sub



    '##########################################################
    Private Sub VerificaUtente(ByVal Utente_User As String, _
                               ByVal Utente_Pwd As String, _
                                ByVal PathDirFileLog As String, _
                                ByVal NomeFileLog As String, _
                                ByVal PercorsoConnessioni As String, _
                                ByVal Connessione_Utenti As String, _
                                ByRef SuperUser_Username As String, _
                                ByRef SuperUser_CodFiscale As String, _
                                ByRef Utente_CodFiscale As String, _
                                ByRef ProgressivoGIAS As Integer, _
                                ByRef id_Servizio As Integer)


        Dim ObjUtente_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim Dt_Utente As New DataTable
        Dim Stringa_cn_Utenti As String
        Dim objDP As New AgronicaCoreDataProvider.DataProvider

        Stringa_cn_Utenti = objDP.FindIniConnessioni(PathFileINI, Connessione_Utenti)

        Dt_Utente = ObjUtente_R.Leggi_DatiUtente_e_DatiSuperUser(Utente_User, _
                                                              "", _
                                                              Utente_Pwd, _
                                                              Nothing, _
                                                              Nothing, _
                                                              Nothing, _
                                                              Nothing, _
                                                              False, _
                                                              0, _
                                                              "", _
                                                             Nothing, _
                                                             Stringa_cn_Utenti, _
                                                             enumVisibilita.Visibilita_SoloNonCancellati, _
                                                             PathDirFileLog, _
                                                            NomeFileLog, _
                                                             Utente_User)

        If Not IsNothing(Dt_Utente) AndAlso Dt_Utente.Rows.Count > 0 Then
            SuperUser_Username = Dt_Utente.Rows(0).Item("Username_SuperUser")
            If SuperUser_Username = "" Then
                Throw New Exception("Verifica utente: Username superuser non trovata!")
            End If
            SuperUser_CodFiscale = Dt_Utente.Rows(0).Item("Piva_SuperUser")
            If SuperUser_CodFiscale = "" Then
                Throw New Exception("Verifica utente: piva superuser non trovata!")
            End If
            Utente_CodFiscale = Dt_Utente.Rows(0).Item("CodFisc")
            If Utente_CodFiscale = "" Then
                Throw New Exception("Verifica utente: codice fiscale utente non trovato!")
            End If
            ProgressivoGIAS = Dt_Utente.Rows(0).Item("ProgressivoGIAS")
            If ProgressivoGIAS = 0 Then
                Throw New Exception("Verifica utente: Progressivo GIAS non trovato!")
            End If
            id_Servizio = Dt_Utente.Rows(0).Item("id_Servizio")
            If id_Servizio = 0 Then
                Throw New Exception("Verifica utente: id_Servizio non trovato!")
            End If
        Else
            Throw New Exception("Le credenziali inviate non sono valide!")
        End If

    End Sub

    '##########################################################
    Private Sub VerificaUtente1(ByVal Utente_User As String, _
                               ByVal Utente_Pwd As String, _
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByRef SuperUser_Username As String, _
                                ByRef SuperUser_Password As String, _
                                ByRef SuperUser_CodFiscale As String, _
                                ByRef Utente_CodFiscale As String, _
                                ByRef ProgressivoGIAS As Integer, _
                                ByRef id_Servizio As Integer)


        Dim ObjUtente_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim Dt_Utente As New DataTable

        Dt_Utente = ObjUtente_R.Leggi_DatiUtente_e_DatiSuperUser(Utente_User, _
                                                                  "", _
                                                                  Utente_Pwd, _
                                                                    -999, _
                                                                  0, _
                                                                  Nothing, _
                                                                  Nothing, _
                                                                   False, _
                                                                  0, _
                                                                  "", _
                                                                  "", _
                                                                 objParametri_Utenti)

        If Not IsNothing(Dt_Utente) AndAlso Dt_Utente.Rows.Count > 0 Then
            SuperUser_Username = Dt_Utente.Rows(0).Item("Username_SuperUser")
            If SuperUser_Username = "" Then
                Throw New Exception("Verifica utente: Username superuser non trovata!")
            End If
            SuperUser_Password = Dt_Utente.Rows(0).Item("Password_SuperUser")
            If SuperUser_Password = "" Then
                Throw New Exception("Verifica utente: Password superuser non trovata!")
            End If
            SuperUser_CodFiscale = Dt_Utente.Rows(0).Item("Piva_SuperUser")
            If SuperUser_CodFiscale = "" Then
                SuperUser_CodFiscale = Dt_Utente.Rows(0).Item("CodFisc_SuperUser")
                If SuperUser_CodFiscale = "" Then
                    Throw New Exception("Verifica utente: piva superuser non trovata!")
                End If
            End If
            Utente_CodFiscale = Dt_Utente.Rows(0).Item("CodFisc")
            If Utente_CodFiscale = "" Then
                Throw New Exception("Verifica utente: codice fiscale utente non trovato!")
            End If
            ProgressivoGIAS = Dt_Utente.Rows(0).Item("ProgressivoGIAS")
            If ProgressivoGIAS = 0 Then
                Throw New Exception("Verifica utente: Progressivo GIAS non trovato!")
            End If
            id_Servizio = Dt_Utente.Rows(0).Item("id_Servizio")
            If id_Servizio = 0 Then
                Throw New Exception("Verifica utente: id_Servizio non trovato!")
            End If
        Else
            Throw New Exception("Le credenziali inviate non sono valide!")
        End If

    End Sub





End Class
