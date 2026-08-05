Imports AgronicaCoreDataProvider

Public Class GDPR

    Public Function VerificaStatoAccettazione(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objparametri_Utenti As AgronicaCoreParametri) As Integer
        Dim informativa As New GDPRmodel
        OttieniInformativaCorrente(objParametri_Server, objparametri_Utenti, informativa)

        'non ho informative da accettare..
        If informativa.GDPR_Cod <> -1 Then

            Dim dt As DataTable
            OttieniGDPR(informativa.GDPR_Cod, objparametri_Utenti, dt)

            Dim gScrivi As New AgronicaCoreUtentiDAL.GDPR_W

            If dt.Rows.Count = 0 Then
                'memorizzo l'informativa in locale
                gScrivi.Scrivi(
                    informativa.GDPR_Cod,
                    informativa.TestoHtmlBreve,
                    informativa.TestoHtmlCompleto,
                    informativa.Validita_Inizio,
                    informativa.Validita_Fine,
                    objparametri_Utenti
                )
            ElseIf informativa.Validita_Fine > dt.Rows(0)("validita_fine") Then
                'aggiorno l'informativa in locale, soltanto se la scadenza sul server è maggiore
                gScrivi.modifica(
                    informativa.GDPR_Cod,
                    informativa.TestoHtmlBreve,
                    informativa.TestoHtmlCompleto,
                    informativa.Validita_Inizio,
                    informativa.Validita_Fine,
                    "",
                    objparametri_Utenti
                )
            End If
        End If

        'procedo con verifica
        Dim xListaGDPRAccettate As String = ""
        OttieniListaGDPR(objparametri_Utenti, xListaGDPRAccettate)
        Dim xVerifica As New AgronicaCoreUtentiDAL.GDPR_R
        Dim dtVerifica As DataTable =
            xVerifica.OttieniGDPRValido(xListaGDPRAccettate, "", "", objparametri_Utenti)

        If dtVerifica.Rows.Count > 0 Then
            Return dtVerifica.Rows(0)("GDPR_COD")
        Else
            Return -1
        End If
    End Function

    Public Function VerificaAccettazione(
        users As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of AgronicaCoreModelsSTD.profilazione.Utente_DettagliGDPR)
        Dim objGDPR As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_R
        Dim DT_GDPR As DataTable = objGDPR.LeggiPer(users, String.Empty, String.Empty, objParametri_Utenti)
        Dim getString = Function(row, field) If(IsDBNull(row(field)), String.Empty, CStr(row(field)))
        Return DT_GDPR.Select.AsParallel.
            Select(Function(row) New AgronicaCoreModelsSTD.profilazione.Utente_DettagliGDPR With {
                .UserName = getString(row, "username"),
                .Cognome = getString(row, "cognome"),
                .Nome = getString(row, "nome"),
                .Data_Accettazione = row("accettazione_dataora")
            }).ToList
    End Function

    Public Function VerificaAccettazioneTutti(
        ByRef objParametri_Utenti As AgronicaCoreParametri,
        Optional xFiltroAggiuntivo As String = ""
    ) As List(Of AgronicaCoreModelsSTD.profilazione.Utente_DettagliGDPR)
        Dim objGDPR As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_R
        Dim xOrderBy As String = ""

        Dim DT_GDPR As DataTable = objGDPR.LeggiTutti(xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
        Dim getString = Function(row, field) If(IsDBNull(row(field)), String.Empty, CStr(row(field)))

        Return DT_GDPR.Select.AsParallel.
            Select(Function(row) New AgronicaCoreModelsSTD.profilazione.Utente_DettagliGDPR With {
                .UserName = row(0),
                .Cognome = row(1),
                .Nome = row(2),
                .Data_Accettazione = row(3)
            }).ToList
    End Function

    Public Sub OttieniGDPRAccettati(ByVal GDPR_Cod As Integer, ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef dtLettura As DataTable)
        Dim Lettura As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_R
        dtLettura = Lettura.Leggi(GDPR_Cod, "", "", objParametri_Utenti)
    End Sub

    Public Sub OttieniGDPR(ByVal GDPR_Cod As Integer, ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef dtLettura As DataTable)
        Dim Lettura As New AgronicaCoreUtentiDAL.GDPR_R
        dtLettura = Lettura.Leggi(GDPR_Cod, "", "", objParametri_Utenti)
    End Sub

    Public Sub OttieniListaGDPR(ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef strDati As String)
        Dim dt As DataTable
        OttieniGDPRAccettati(0, objParametri_Utenti, dt)

        Dim lDati As List(Of String) = (
            From dd In dt.AsEnumerable
            Distinct Select CStr(dd("GDPR_Cod"))
        ).ToList

        strDati = String.Join(",", lDati.ToArray)

    End Sub



    Private Sub OttieniInformativaCorrente(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef informativa As GDPRmodel)
        Dim urlWS As String

        Try

            Dim xLeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt As DataTable =
                xLeggiConfSiti.Leggi(0, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server)

            If dt.Rows.Count > 0 Then
                urlWS = dt.Rows(0)("valore")
            Else
                urlWS = "https://ws.netagronica.it/AgronicaWebService/Gias_service.asmx"
            End If


            'urlWS = vDal.Leggi_Valore(6, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server)
        Catch ex As Exception
            'in errore, procedo con quanto già memorizzato ..
            informativa.GDPR_Cod = -1
            'Throw New Exception("errore in lettura configurazione", ex)
        End Try


        Dim xWS As New ws_ppt.WS_Ppt
        If urlWS <> "" Then
            xWS.Url = urlWS.Replace("Gias_service.asmx", "Ws_Ppt.asmx")
        End If

        xWS.Timeout = 4000


        Dim GDPR_Verifica As String = ""

        'ottengo comunque l'ultima valida .. commento riga 
        'OttieniListaGDPR(objParametri_Utenti, GDPR_Verifica)

        Try

            Dim informativa1 As ws_ppt.GDPRmodel =
            xWS.GDPR(GDPR_Verifica)

            informativa.GDPR_Cod = informativa1.GDPR_Cod
            informativa.TestoHtmlBreve = informativa1.TestoHtmlBreve
            informativa.TestoHtmlCompleto = informativa1.TestoHtmlCompleto
            informativa.Validita_Inizio = informativa1.Validita_Inizio
            informativa.Validita_Fine = informativa1.Validita_Fine

        Catch ex As Exception
            'in errore, procedo con quanto già memorizzato ..
            informativa.GDPR_Cod = -1
            'Throw New Exception("errore in chiamata ws ", ex)

        End Try
    End Sub

    Public Function CheckGDPRAcceptation(ByVal CodiceUtente As String,
                                         ByRef ObjParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim xGDPRr As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_R

        Dim dt = xGDPRr.LeggiXUtente(1, CodiceUtente, "", "", ObjParametri_Utenti)
        If dt.Rows.Count <= 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function ForceGDPRAcceptation(ByVal username As String,
                                         ByRef ObjParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim xGDPRw As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_W

        Return xGDPRw.ScriviXutente(username, 1, 1, Date.Now, ObjParametri_Utenti)
    End Function
End Class
