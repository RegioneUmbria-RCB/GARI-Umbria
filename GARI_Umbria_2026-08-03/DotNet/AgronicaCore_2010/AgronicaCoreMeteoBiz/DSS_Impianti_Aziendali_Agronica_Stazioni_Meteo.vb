Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreMeteoDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo

    Private _cfg As ModelloParametriGSB

    Public Sub New(cfg As ModelloParametriGSB)
        _cfg = New ModelloParametriGSB
        _cfg = cfg
    End Sub


    Public Function AccodaImpiantiConDatoGisSenzaOrigineAssociata(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri, ByVal objParametri_Super_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Try
            rval.RispostaOK = True

            Dim letturaImpiantiDaAccodare As New DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_R
            Dim dtImpiantiLetti As DataTable =
                letturaImpiantiDaAccodare.LeggiImpiantiConDatoGisSenzaOrigineAssociata(_cfg.ListaSpecieFiltro, _cfg.Validita_Inizio_impianti, objParametri_Server)

            Dim scritturaImpianto As New DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo_W

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Dim objMeteo As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Utenti)

            If String.IsNullOrEmpty(_cfg.GiasOnline_WS_Meteo_Meteo) Then
                Dim leggicfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                _cfg.GiasOnline_WS_Meteo_Meteo = leggicfg.Leggi_Valore(0, "GiasOnline_WS_Meteo_Meteo", "", "", objParametri_Server)
            End If

            CompletaLetturaParametriCfg(objParametri_Utenti)

            For Each impianto As DataRow In dtImpiantiLetti.Rows

                'leggo da agronicameteosuite i dati ...

                If LetturaStazioneAssociataViaWS(objParametri_Server, objMeteo, impianto) Then

                    scritturaImpianto.Scrivi(
                        impianto("piva"),
                        impianto("sa_cod"),
                        impianto("appezza"),
                        impianto("id_reg"),
                        impianto("Tipo_Sorgente"),
                        impianto("Stazione_Cod"),
                        "",
                        objParametri_Server
                    )

                End If

            Next

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    Private Function CompletaLetturaParametriCfg(objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreParametri
        If _cfg.ParametriChiamataWsMeteo Is Nothing OrElse String.IsNullOrEmpty(_cfg.ParametriChiamataWsMeteo.ASG_Utente_Password_Crypt) Then

            If _cfg.ParametriChiamataWsMeteo Is Nothing Then
                _cfg.ParametriChiamataWsMeteo = New ParametriChiamataWsMeteo
            End If

            Dim ObjUtente_R As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim Dt_Utente As New DataTable

            Dt_Utente = ObjUtente_R.Leggi_DatiUtente_e_DatiSuperUser(
                _cfg.ParametriChiamataWsMeteo.ASG_Utente_Username,
                "",
                "",
                -999,
                0,
                Nothing,
                Nothing,
                False,
                0,
                "",
                "",
                objParametri_Utenti
             )


            If Not IsNothing(Dt_Utente) AndAlso Dt_Utente.Rows.Count > 0 Then


                _cfg.ParametriChiamataWsMeteo.ASG_Utente_Password_Crypt = Stringa_Codifica_LANCompatibile(Dt_Utente.Rows(0).Item("Password_SuperUser"), CostantiPersonalizzate.AgroKey_EncoderDecoder)
                _cfg.ParametriChiamataWsMeteo.ASG_Utente_Username_Crypt = Stringa_Codifica_LANCompatibile(_cfg.ParametriChiamataWsMeteo.ASG_Utente_Username, CostantiPersonalizzate.AgroKey_EncoderDecoder)

            End If

        End If

        Return objParametri_Utenti
    End Function

    Private Function LetturaStazioneAssociataViaWS(objParametri_Server As AgronicaCoreParametri, objMeteo As InterfacciaMeteoSuite, impianto As DataRow) As Boolean

        Dim DT_Stazioni As DataTable

        Dim sorgenti As Integer() = {
            TipiEnumerativi.enum_Meteo_Tiposorgente.Aziendali,
            TipiEnumerativi.enum_Meteo_Tiposorgente.RetiPartner,
            TipiEnumerativi.enum_Meteo_Tiposorgente.Gias_RER_Quadranti
        }

        Dim last_dist As Integer = Integer.MaxValue
        Dim result As Boolean = False

        For Each sorg In sorgenti

            DT_Stazioni = LetturaStazioneChiamaWS(sorg, objParametri_Server, objMeteo, impianto)

            If DT_Stazioni IsNot Nothing AndAlso DT_Stazioni.Rows.Count > 0 Then

                If Not IsDBNull(DT_Stazioni.Rows(0)("Distanza")) Then

                    Dim dist As Integer = CInt(DT_Stazioni.Rows(0)("Distanza"))

                    If dist > -1 AndAlso dist < last_dist Then

                        last_dist = dist

                        impianto("Tipo_Sorgente") = sorg
                        impianto("Stazione_Cod") = DT_Stazioni(0)("Id")

                        result = True
                    End If
                End If
            End If
        Next

        Return result
    End Function

    Private Function LetturaStazioneChiamaWS(
        TipoSorgente As Integer,
        objParametri_Server As AgronicaCoreParametri, objMeteo As InterfacciaMeteoSuite, impianto As DataRow) As DataTable

        Return objMeteo.ElencoStazioniConDistanza(
                    objParametri_Server.PivaSuperUser,
                    GetDoorkey(),
                    _cfg.ParametriChiamataWsMeteo.ASG_Utente_Username,
                    _cfg.ParametriChiamataWsMeteo.ASG_Utente_Username_Crypt,
                    _cfg.ParametriChiamataWsMeteo.ASG_Utente_Password_Crypt,
                    impianto("piva"), 'objParametri_Server,
                    impianto("Latitudine"),
                    impianto("Longitudine"),
                    TipoSorgente,
                    _cfg.GiasOnline_WS_Meteo_Meteo
                )

    End Function

    Private Shared Function GetDoorkey() As String
        Dim Doorkey As String = "Y4h8u3B5w2"
        Return Doorkey
    End Function

End Class
