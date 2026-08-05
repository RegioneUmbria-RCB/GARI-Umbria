
Imports System.Text

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ

Public Class Servizi2017


    Public Function ProvisioningDSSResponseElaboraRispostaStandard(s As String) As RispostaStandard

        Dim o As ProvisioningDSSResponse = JsonConvert.DeserializeObject(Of ProvisioningDSSResponse)(s)

        Dim r As New RispostaStandard
        r.RispostaOK = True
        r.RispostaStringa = ""

        Select Case o.statusCode
            Case 200
                'esito ok

            Case Else
                r.RispostaOK = False
                r.RispostaStringa = "Errore Globale"
                r.Errore = o.message
        End Select

        Return r


    End Function

    Public Function ProfileManagerResponseElaboraRispostaStandard(s As String) As RispostaStandard

        Dim o As ProfileManagerResponse = JsonConvert.DeserializeObject(Of ProfileManagerResponse)(s)

        Return ProfileManagerResponseElaboraRispostaStandard(o)

    End Function

    ''' <summary>
    ''' Elabora la risposta del metodo profileManager su rispostaStandard.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <returns></returns>
    Public Function ProfileManagerResponseElaboraRispostaStandard(ByVal o As ProfileManagerResponse) As RispostaStandard

        Dim r As New RispostaStandard
        r.RispostaOK = True
        r.RispostaStringa = ""

        Select Case o.statusCode
            Case 200
                'esito ok


                Dim risultatoSuImpresa As AgronicaCoreProfilazioneBIZ.ImpreseProfilateResponse =
                    o.dettaglioEsito.impreseProfilateResponse.FirstOrDefault

                If Not risultatoSuImpresa Is Nothing Then

                    For Each curServizioAttivato In risultatoSuImpresa.serviziResponse

                        If Not curServizioAttivato.esitoBool Then
                            r.RispostaOK = False
                            r.RispostaStringa &= curServizioAttivato.servizio_id & " - " & curServizioAttivato.esito
                        End If

                    Next


                End If

            Case Else
                r.RispostaOK = False
                r.RispostaStringa = "Errore Globale"
                r.Errore = o.message
        End Select

        Return r

    End Function


    ''' <summary>
    ''' Servizi attivi
    ''' </summary>
    ''' <param name="p">p.iva impresa</param>
    ''' <returns>oggetto json con lista di servizi attivi</returns>
    Public Function ServiziAttiviDataImpresa(p As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim verificaProfilazione As New AgronicaCoreProfilazioneBIZ.Pratiche_R

        Dim esitoVerificaProfilazione As RispostaStandard =
            verificaProfilazione.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, p, 0, "", objParametri_Server, objParametri_Utenti)


        Return esitoVerificaProfilazione
    End Function

    ''' <summary>
    ''' Genera la stringa JSON a partire da CUAA, Piva, lista di servizi da attivare (sempre in stato "Attivo, Pagante"
    ''' </summary>
    ''' <param name="cuaa"></param>
    ''' <param name="profiloCommerciale">es.: 1002,1003</param>
    ''' <param name="Piva"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function ProfileManagerClientGeneraRichiesta(cuaa As String, profiloCommerciale As String, Piva As String, UtenteUsername As String) As String
        Dim xRichiestaWS_Post As String
        Dim stb As New StringBuilder

        Dim serviziRequest As String = ServiziRequestOttieni(profiloCommerciale)

        stb.AppendLine("             { ")
        stb.AppendLine("  ")
        stb.AppendLine("               ""username"": """ & UtenteUsername & """, ")
        stb.AppendLine("               ""impreseProfilateRequest"": [ ")
        stb.AppendLine("                 { ")
        stb.AppendLine("                   ""impresa"": """ & cuaa & """, ")
        stb.AppendLine("                   ""impresa_piva"": """ & Piva & """, ")
        stb.AppendLine("                   ""serviziRequest"":    [ ")

        stb.Append(serviziRequest)

        stb.AppendLine("                   ] ")
        stb.AppendLine("  ")
        stb.AppendLine("                 } ")
        stb.AppendLine("  ")
        stb.AppendLine("               ] ")
        stb.AppendLine("             }")


        xRichiestaWS_Post = stb.ToString
        Return xRichiestaWS_Post
    End Function

    ''' <summary>
    ''' Genera la stringa JSON a partire dai parametri passati
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="piva_superUser"></param>
    ''' <param name="PacchettoCommerciale_Cod"></param>
    ''' <param name="scadenza"></param>
    ''' <returns></returns>
    Public Function ProvisioningDSSClientGeneraRichiesta(ByVal piva As String, ByVal piva_superUser As String, ByVal PacchettoCommerciale_Cod As Integer, scadenza As DateTime) As String
        Dim xRichiestaWS_Post As String
        Dim stb As New StringBuilder

        Dim sScadenza As String = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(scadenza, DateTimeKind.Local)

        stb.AppendLine("             { ")
        stb.AppendLine("               ""username"": """", ")
        stb.AppendLine("               ""password"": """", ")
        stb.AppendLine("               ""piva"": """ & piva & """, ")
        stb.AppendLine("               ""piva_SuperUser"": """ & piva_superUser & """, ")
        stb.AppendLine("               ""scadenza"": """ & sScadenza & """, ")
        stb.AppendLine("               ""pacchettoCommercialeCod"": " & PacchettoCommerciale_Cod & " ")
        stb.AppendLine("             }")


        xRichiestaWS_Post = stb.ToString
        Return xRichiestaWS_Post
    End Function

    Private Shared Function ServiziRequestOttieni(profiloCommerciale As String) As String
        Dim vProfiloCommerciale As String() = profiloCommerciale.Split(",")
        Dim listaProfili As New List(Of String)
        Dim stb1 As New StringBuilder
        For Each pCom In vProfiloCommerciale
            stb1.Length = 0
            stb1.AppendLine("                     { ")
            stb1.AppendLine("                       ""servizio_id"": " & pCom & ", ")
            stb1.AppendLine("                       ""stato_id"": 1002 ")
            stb1.Append("                     } ")
            listaProfili.Add(stb1.ToString)
        Next

        Dim serviziRequest As String =
            String.Join(",", listaProfili)
        Return serviziRequest
    End Function

End Class
