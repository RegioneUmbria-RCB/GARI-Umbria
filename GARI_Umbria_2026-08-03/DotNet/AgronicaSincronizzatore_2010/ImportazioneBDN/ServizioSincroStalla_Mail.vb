Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports System
Imports System.Text

Public Class ServizioSincroStalla_Mail
    Private Const CODICE_ANAGRAFE_SINCRO_CAPI_INVIO_MAIL = 202
    Private Const CODICE_ANAGRAFE_INDIRIZZO_MAIL_RITARDI_REGISTRAZIONI = 203

    Private ID_Tabella_PivaSuperuser As String
    Private ID_Tabella_Id_Servizio As enum_Id_Servizio
    Private ID_Tabella_Tipo_Sincro As enum_Tipi_Servizi_Background
    Private ID_Tabella_Id_Riga As Integer

    Private objAgroSeq As AgronicaCoreDataProvider.Agro_Sequenze

    Private _ConfigurazioneServizio As Configurazione_Servizio
    Private ObjParametri_SuperServer As AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreParametri

    ''Oggetti utilizzati
    Private Configurazione_Siti_R As AgronicaCoreVarieDAL.Configurazione_Siti_R
    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    ''Da ricavare da Configurazione_Servizio
    Private SuperUserUsername As String
    Private SuperUserPassword As String
    Private SuperUserPiva As String
    Private Codice_Chiave_Cliente As Integer
    Private Piva_Padre As String
    Private Mittente As String
    Private Destinatari As String
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private Parametri_Extra As String

    ''Parametri fissi impostati dalla classe
    Private ProgressivoGIAS As String
    Private LinkWSImportaGIAS As String

    Private GiorniConfigurati As Integer = 2

    Public Sub New(ByVal _Configurazione_Servizio As Configurazione_Servizio,
            ByVal _ObjParametri_SuperServer As AgronicaCoreParametri,
            ByVal _ObjParametri_Server As AgronicaCoreParametri,
            ByVal _ObjParametri_Utenti As AgronicaCoreParametri)

        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra
        'Configurazione_Servizio = _Configurazione_Servizio leggo solo i parametri che mi servono 
        ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

    End Sub

    Private Sub InizializzoOggettiCore()
        objAgroSeq = New AgronicaCoreDataProvider.Agro_Sequenze
        Configurazione_Siti_R = New AgronicaCoreVarieDAL.Configurazione_Siti_R
        objLog = New AgronicaCoreDataProvider.LogProvider
        Configurazione_Servizi_R = New AgronicaCoreVarieDAL.Configurazione_Servizi_R
    End Sub

    Private Sub ImpostoGliAltriParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        ID_Tabella_PivaSuperuser = _Configurazione_Servizio.PivaSuperuser
        ID_Tabella_Id_Servizio = _Configurazione_Servizio.Id_Servizio
        ID_Tabella_Tipo_Sincro = _Configurazione_Servizio.Tipo_Sincro
        ID_Tabella_Id_Riga = _Configurazione_Servizio.Id_Riga

        SuperUserUsername = ObjParametri_Server.SuperUserUsername
        SuperUserPiva = ObjParametri_Server.PivaSuperUser
        Codice_Chiave_Cliente = _Configurazione_Servizio.Id_Cod_Cliente
        Piva_Padre = _Configurazione_Servizio.Piva_Padre
        Mittente = _Configurazione_Servizio.Mittente
        Destinatari = _Configurazione_Servizio.Destinatari
        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni
        DirectoryFileEsportazioni = _Configurazione_Servizio.DirectoryFileEsportazioni
        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra
        LinkWSImportaGIAS = Configurazione_Siti_R.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline, "Sincro_LinkWSImportaGIAS", "", "", ObjParametri_Server)

        If IsNumeric(Parametri_Extra) Then
            GiorniConfigurati = Parametri_Extra
        End If

    End Sub

    Public Function Avvia() As String
        Dim retStr = ""
        Dim message As New StringBuilder
        message.AppendLine($"I seguenti capi caricati non sono stati ancora sincronizzati:")
        message.AppendLine("")

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(ObjParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
        Dim dtStalle = objStalla_R.Leggi("", 0, 0,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         " BDN_Codice_Azienda <> '' ", "",
                                         ObjParametri_Server)

        For Each dr In dtStalle.Rows


            Dim Piva As String = dr("Piva")
            Dim Sa_Cod As Integer = dr("Sa_Cod")
            Dim Sta_Num As Integer = dr("STA_NUM")
            Dim Sta_Des As String = dr("STA_DES")
            Dim BDN_Codice_Azienda As String = dr("BDN_Codice_Azienda")

            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim Rag_Soc As String = objImprese.RagSoc_from_Piva(Piva, ObjParametri_Server)
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim Sa_Nome As String = objCentri.SaNome_from_SaCod(Piva, Sa_Cod, ObjParametri_Server)
            Try
                Dim objFabbxCod_R As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
                Dim dtStallexCodici As DataTable = objFabbxCod_R.Leggi(Piva, Sa_Cod, Sta_Num, CODICE_ANAGRAFE_SINCRO_CAPI_INVIO_MAIL,
                                                                       1, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                       "", "", ObjParametri_Server)

                'solo le stalle che hanno il servizio attivo
                If Not IsNothing(dtStallexCodici) AndAlso dtStallexCodici.Rows.Count > 0 Then
                    Dim obj_ZooAnimali_R As New Zoo_Animali
                    Dim dtMovCarico_NoSincro = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0,
                                                                                                             AGRODATAINIZIO, Date.Now.AddDays(-GiorniConfigurati),
                                                                                                             False, Nothing,
                                                                                                             ObjParametri_Server)

                    If dtMovCarico_NoSincro.Rows.Count > 0 Then
                        message.AppendLine("Impresa " & Rag_Soc & " Centro " & Sa_Nome & " Stalla " & Sta_Des & " (Codice Azienda BDN:" & BDN_Codice_Azienda & ")")

                        For Each capo In dtMovCarico_NoSincro.Rows
                            message.AppendLine(" - " & capo("Matricola") & " [" & capo("RAZ_DES") & "] in data " & capo("Data_Movimento") & " ;")
                        Next

                        message.AppendLine("")
                        message.AppendLine($"===========================================================================")

                        'invio mail per segnalazione carichi non sincronizzati
                        dtStallexCodici = objFabbxCod_R.Leggi(Piva, Sa_Cod, Sta_Num, CODICE_ANAGRAFE_INDIRIZZO_MAIL_RITARDI_REGISTRAZIONI,
                                                              "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              "", "", ObjParametri_Server)

                        Dim objMail As New Mail
                        Dim mailCentro_list As New List(Of String)
                        If Not IsNothing(dtStallexCodici) AndAlso dtStallexCodici.Rows.Count > 0 Then
                            For Each row In dtStallexCodici.Rows
                                mailCentro_list.Add(row("val_cod"))
                            Next
                        End If

                        'Dim mailCentro_list As List(Of String) = (From r In GiasContext.CentrixRubrica
                        '                                          Join rubrica In GiasContext.Rubrica On r.cod_rubrica Equals rubrica.cod_rubrica
                        '                                          Where r.PIVA = Piva AndAlso r.sa_cod = Sa_Cod AndAlso rubrica.descr = "Email"
                        '                                          Select rubrica.numero).ToList

                        Dim oggettoMail As String = $"Carichi capi non sincronizzati con BDN - {Now.ToShortDateString()} {Now.ToShortTimeString()}"

                        Dim respMail = ""
                        For Each mailCentro In mailCentro_list
                            Dim singolaMail = mailCentro.Split(";")
                            For Each mail In singolaMail
                                respMail &= objMail.invia(ObjParametri_Server,
                                                     Mittente, mail.Trim,
                                                     Nothing, Nothing,
                                                     oggettoMail, message.ToString,
                                                     False, Nothing, enablessl:=True)
                            Next
                        Next

                        'invio mail verso mail nel servizio
                        Dim listDestinatari = Destinatari.Split(";").ToList
                        For Each dest In listDestinatari
                            respMail &= objMail.invia(ObjParametri_Server,
                                                     Mittente, dest.Trim,
                                                     Nothing, Nothing,
                                                     oggettoMail, message.ToString,
                                                     False, Nothing, enablessl:=True)
                        Next

                        message = New StringBuilder
                    End If

                End If

            Catch ex As Exception
                Throw New Exception(System.Reflection.MethodBase.GetCurrentMethod.Name & "->" & ex.Message)
            End Try

        Next

        GiasContext.Dispose()

        Return retStr

    End Function

End Class
