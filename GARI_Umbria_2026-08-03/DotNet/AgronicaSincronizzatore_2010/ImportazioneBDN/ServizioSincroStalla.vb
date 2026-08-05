Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json

Public Class ServizioSincroStalla
    Private TIPO_SINCRO_VETINFO As Integer = 1

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
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private Parametri_Extra As String
    Private GiorniConfigurati As Integer = 7

    ''Parametri fissi impostati dalla classe
    Private ProgressivoGIAS As String
    Private LinkWSImportaGIAS As String

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
        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni
        DirectoryFileEsportazioni = _Configurazione_Servizio.DirectoryFileEsportazioni
        LinkWSImportaGIAS = Configurazione_Siti_R.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline, "Sincro_LinkWSImportaGIAS", "", "", ObjParametri_Server)

        If IsNumeric(Parametri_Extra) Then
            GiorniConfigurati = Parametri_Extra
        End If

    End Sub

    Public Function Avvia() As String
        Dim retStr = ""

        Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
        Dim dtStalle = objStalla_R.Leggi("", 0, 0,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         " BDN_Codice_Azienda <> '' ", "",
                                         ObjParametri_Server)

        Dim sincronizzaAzienda As New SincroBDNAzienda(ObjParametri_Server, ObjParametri_Utenti)
        Dim sincronizzaAllevamento As New SincroBDNAllevamento(ObjParametri_Server, ObjParametri_Utenti, "", "", "")

        Dim listResult As New List(Of List(Of SincroBDN_Allevamento_Response))
        If dtStalle IsNot Nothing AndAlso dtStalle.Rows.Count > 0 Then
            For Each rowStalla In dtStalle.Rows

                Try
                    Dim Piva As String = rowStalla("Piva")
                    Dim Sa_Cod As String = rowStalla("Sa_Cod")
                    Dim Fabb_Cod As String = rowStalla("STA_NUM")
                    Dim BDN_Codice_Azienda As String = rowStalla("BDN_Codice_Azienda")

                    Dim objFabbxCod_R As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
                    Dim dtStallexCodici As DataTable = objFabbxCod_R.Leggi(Piva, Sa_Cod, Fabb_Cod, 201, 1,
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                           "", "", ObjParametri_Server)

                    If Not IsNothing(dtStallexCodici) AndAlso dtStallexCodici.Rows.Count > 0 Then
                        Dim genCod As String = rowStalla("GEN_COD")
                        Dim speCod As String = rowStalla("SPE_COD")

                        Dim objCodificaSpecie As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                        Dim dtCodifica = objCodificaSpecie.leggi(ObjParametri_Server, "", "",
                                                                 genCod, speCod, "3")
                        If dtCodifica.Rows.Count = 0 Then
                            'Throw New GiasException("Stalla " & BDN_Codice_Azienda & " non configurata correttamente")
                            Continue For
                        End If

                        Dim spe_codice As String = dtCodifica.Rows(0)("CODICE")
                        spe_codice = CInt(spe_codice).ToString("D4")

                        listResult.Add(sincronizzaAllevamento.SincronizzaStallaGIAS(Piva, Sa_Cod, Fabb_Cod,
                                                                                         BDN_Codice_Azienda, spe_codice))
                        Try
                            SincronizzaStallaVetInfoxUtenti(Piva, Sa_Cod, Fabb_Cod, GiorniConfigurati)
                        Catch ex As Exception
                            Dim customLOGParams As New CustomLOGParams With {
                                .LogDescrizioneUtente = ObjParametri_Server.LogDescrizioneUtente,
                                .LogDirectory = LogDirectory,
                                .LogFileName = LogFileName
                            }


                            objLog.Scrivi_LOG(ObjParametri_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore su SincronizzaStallaVetInfoxUtenti: " & BDN_Codice_Azienda & " - " & ex.Message,
                              CustomLOGParams:=customLOGParams)
                        End Try
                    End If

                Catch ex As Exception

                End Try

            Next
        End If

        'SincronizzaVetInfoxUtenti(GiorniConfigurati)
        retStr = JsonConvert.SerializeObject(listResult)

        Return retStr

    End Function


    Private Function SincronizzaVetInfoxUtenti(Giorni As Integer)
        Dim objUtenti_Token_Spid_R As New AgronicaCoreUtentiDAL.Utenti_Token_Spid_R
        Dim objUtentixFabbricati_R As New AgronicaCoreAnagrafeDAL.UtentixFabbricati_R

        Dim dtUtentiToken = objUtenti_Token_Spid_R.Leggi(0, "", 1, "", "", ObjParametri_Utenti)

        For Each tokenRow In dtUtentiToken.Rows
            If (Not IsDBNull(tokenRow("Refresh_Token")) AndAlso CStr(tokenRow("Refresh_Token")) <> "") Then
                Dim username = tokenRow("Username")
                Dim dtStalleVisibili = objUtentixFabbricati_R.Leggi(username, "", 0, 0, "", "", ObjParametri_Server)

                If dtStalleVisibili IsNot Nothing AndAlso dtStalleVisibili.Rows.Count > 0 Then
                    For Each stallaVisibileRow In dtStalleVisibili.Rows
                        Dim piva As String = stallaVisibileRow("Piva")
                        Dim Sa_Cod As Integer = stallaVisibileRow("Piva")
                        Dim Fabbricato_Cod As Integer = stallaVisibileRow("Piva")

                        SincronizzaVetInfo(username, piva, Sa_Cod, Fabbricato_Cod, Date.Now, Date.Now.AddDays(-Giorni))

                    Next
                End If

            End If
        Next

    End Function

    Private Function SincronizzaStallaVetInfoxUtenti(Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, Giorni As Integer)
        Dim objUtenti_Token_Spid_R As New AgronicaCoreUtentiDAL.Utenti_Token_Spid_R
        Dim objUtentixFabbricati_R As New AgronicaCoreAnagrafeDAL.UtentixFabbricati_R

        Dim dtUtentiToken = objUtenti_Token_Spid_R.Leggi(0, "", 1, "", "", ObjParametri_Utenti)

        For Each tokenRow In dtUtentiToken.Rows
            If (Not IsDBNull(tokenRow("Refresh_Token")) AndAlso CStr(tokenRow("Refresh_Token")) <> "") Then
                Dim username = tokenRow("Username")
                Dim dtStalleVisibili = objUtentixFabbricati_R.Leggi(username, Piva, Sa_Cod, Fabbricato_Cod, "", "", ObjParametri_Server)

                If dtStalleVisibili IsNot Nothing AndAlso dtStalleVisibili.Rows.Count > 0 Then
                    For Each stallaVisibileRow In dtStalleVisibili.Rows

                        SincronizzaVetInfo(username, Piva, Sa_Cod, Fabbricato_Cod, Date.Now.AddDays(-Giorni), Date.Now)
                    Next
                End If

            End If
        Next

    End Function

    Private Function SincronizzaVetInfo(Username As String, Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, Data_Inizio As Date, Data_Fine As Date)
        Dim objimportazioneREV As New SincroREV(ObjParametri_Server, ObjParametri_Utenti, Username)
        Dim objStalle As New AgronicaCoreAnagrafeDAL.Stalla_R
        Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim DTStalla = objStalle.Leggi(Piva, Sa_Cod, Fabbricato_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametri_Server)
        Dim Cuaa = objImprese_Codici.Leggi_CUAA(Piva, ObjParametri_Server)
        Dim Codice_Stalla = DTStalla.Rows(0)("BDN_Codice_Azienda")

        Dim obj_ZooAnimali_R As New Zoo_Animali
        'Dim dt_CapiPresenti_Stalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Fabbricato_Cod,
        '                                                                              0, 0, Date.Now,
        '                                                                              ObjParametri_Server, filtraGiacenze1:=False)

        'Dim listaProprietari = From cp In dt_CapiPresenti_Stalla
        '                       Where CStr(cp.Item("CF_PROPRIETARIO")) <> ""
        '                       Select CStr(cp.Item("CF_PROPRIETARIO"))
        '                       Distinct.ToList()

        'Dim objResp As New List(Of Sincronizza_REV_VetInfo_Response)
        'For Each proprietari In listaProprietari
        '    Try
        '        objResp.Add(objimportazioneREV.Sincronizza_REV_VetInfo(Piva,
        '                                                               Sa_Cod,
        '                                                               Fabbricato_Cod,
        '                                                               Codice_Stalla,
        '                                                               Cuaa,
        '                                                               proprietari,
        '                                                               Data_Inizio,
        '                                                               Data_Fine,
        '                                                               True,
        '                                                               False,
        '                                                               False,
        '                                                               False))
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'Next
    End Function

End Class
