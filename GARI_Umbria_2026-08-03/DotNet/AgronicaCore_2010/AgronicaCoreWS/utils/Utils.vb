Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Utility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module Utils
    Function ProvideRispostaStandardFrom(Of T)(risposta As T) As RispostaStandard
        If TypeOf risposta Is RispostaStandard Then
            Return TryCast(risposta, RispostaStandard)
        End If

        Dim r As New RispostaStandard
        r.RispostaOK = True
        If TypeOf risposta Is String And isValidJSON(TryCast(risposta, String)) Then
            r.RispostaStringa = TryCast(risposta, String)
        Else
            r.RispostaStringa = JsonConvert.SerializeObject(risposta)
        End If
        Return r
    End Function

    Function RispostaStandardOnError(errorCode As Integer)
        Dim r As New RispostaStandard
        r.RispostaOK = True
        r.Errore = errorCode
        Return r
    End Function

    Function MessaggioErroreFrom(ex As Exception, Optional reportEntireExceptionStack As Boolean = True) As AgronicaCoreVarieBIZ.RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = False
        If reportEntireExceptionStack Then
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        Else
            r.Errore = ex.Message
        End If
        Return r
    End Function

    Private Function isValidJSON(strInput As String) As Boolean
        If String.IsNullOrWhiteSpace(strInput) Then
            Return False
        End If

        strInput = strInput.Trim()

        If (strInput.StartsWith("{") AndAlso strInput.EndsWith("}")) OrElse (strInput.StartsWith("[") AndAlso strInput.EndsWith("]")) Then

            Try
                Dim obj = JToken.Parse(strInput)
                Return True
            Catch jex As JsonReaderException
                Console.WriteLine(jex.Message)
                Return False
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Public Function GetASG_Utente_Username(paramServer As AgronicaCoreParametri)
        Return paramServer.UtenteUsername
    End Function

    Public Function GetUtenteCodFiscale(x_UserName_Utente As String, objParametri_Utenti As AgronicaCoreParametri) As String

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim Id_Operazione = 1
        Dim Id_Attivita = 0
        Dim xFiltroAggiuntivo = ""
        Dim DT = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(x_UserName_Utente, "",
                                                    "",
                                                    Id_Operazione,
                                                    Id_Attivita,
                                                    Date.Now,
                                                    CType(Now.Hour, Short),
                                                    True,
                                                    5, xFiltroAggiuntivo, "", objParametri_Utenti)

        Dim x_Utente_CodFiscale = DT.Rows(0).Item("CodFisc")

        Return x_Utente_CodFiscale
    End Function


    Public Function GetParametriObjParametriAgendaNG(ParamAgenda As Parametri_ObjParametriAgenda_NG_GestioneRichieste) As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

        Dim objAgendaNG As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG

        objAgendaNG.TipoOperazioneDB = ParamAgenda.TipoOperazioneDB
        objAgendaNG.Piva = ParamAgenda.Piva
        objAgendaNG.Sa_Cod = ParamAgenda.Sa_Cod
        objAgendaNG.Fabbricato = ParamAgenda.Fabbricato
        objAgendaNG.Campo_Cod = ParamAgenda.Campo_Cod
        objAgendaNG.Appezza = ParamAgenda.Appezza
        objAgendaNG.Id_Reg = ParamAgenda.Id_Reg
        objAgendaNG.Progetto_Cod = ParamAgenda.Progetto_Cod
        objAgendaNG.Validita_Inizio = ParamAgenda.Validita_Inizio
        objAgendaNG.Validita_Fine = ParamAgenda.Validita_Fine
        objAgendaNG.Veg_Cod = ParamAgenda.Veg_Cod
        objAgendaNG.Veg_Des = ParamAgenda.Veg_Des
        objAgendaNG.Id_Cod = ParamAgenda.Id_Cod
        objAgendaNG.Id_Des = ParamAgenda.Id_Des
        objAgendaNG.Cau_Mov = ParamAgenda.Cau_Mov
        objAgendaNG.Mac_Cod = ParamAgenda.Mac_Cod
        objAgendaNG.Lav_Des = ParamAgenda.Lav_Des
        objAgendaNG.Lav_Cod = ParamAgenda.Lav_Cod
        objAgendaNG.SaNome = ParamAgenda.SaNome
        objAgendaNG.RagSoc = ParamAgenda.RagSoc
        objAgendaNG.Pagina_Provenienza = ParamAgenda.Pagina_Provenienza
        objAgendaNG.Pagina_Richiesta = ParamAgenda.Pagina_Richiesta
        objAgendaNG.Data = ParamAgenda.Data
        objAgendaNG.Cod_Contatto = ParamAgenda.Cod_Contatto
        objAgendaNG.QueryStringFiltrino = ParamAgenda.QueryStringFiltrino

        Dim ImpiantiAgendaNG As New AgronicaCoreGestioneRichieste.ImpiantiAgendaNG
        For Each item In ParamAgenda.Impianti
            ImpiantiAgendaNG = New AgronicaCoreGestioneRichieste.ImpiantiAgendaNG
            ImpiantiAgendaNG.Piva = item.Piva
            ImpiantiAgendaNG.Sa_Cod = item.Sa_Cod
            ImpiantiAgendaNG.Appezza = item.Appezza
            ImpiantiAgendaNG.Id_Reg = item.Id_Reg
            ImpiantiAgendaNG.Progetto_Cod = item.Progetto_Cod
            ImpiantiAgendaNG.Veg_Cod = item.Veg_Cod
            ImpiantiAgendaNG.Id_Cod = item.Id_Cod
            ImpiantiAgendaNG.Sup_Imp = item.Sup_Imp
            objAgendaNG.Impianti.Add(ImpiantiAgendaNG)
        Next

        objAgendaNG.Id_Agenda = ParamAgenda.Id_Agenda
        objAgendaNG.TipoOperazioneAgenda = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
        objAgendaNG.TipoRicetta = ParamAgenda.TipoRicetta
        objAgendaNG.Stato = ParamAgenda.Stato
        objAgendaNG.Ricetta_Operazione_Cod = ParamAgenda.Ricetta_Operazione_Cod
        objAgendaNG.Ricetta_Cod = ParamAgenda.Ricetta_Cod
        objAgendaNG.GenericObj_string = ParamAgenda.GenericObj_string
        objAgendaNG.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objAgendaNG.Programmazione_Cod = ParamAgenda.Programmazione_Cod
        objAgendaNG.RedirectUrl = ParamAgenda.RedirectUrl
        objAgendaNG.IdSezione = ParamAgenda.IdSezione

        Return objAgendaNG
    End Function

    Function GetParametriAggiuntiviNG(InParametriAggiunti As List(Of AgronicaCoreDTOStd.InData.Utility.ParametriAggiuntivi_QueryString)) As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)

        Dim ParametriAggiuntivi As New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
        Dim ParametriAggiuntivi_QueryString As New AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString

        For Each item In InParametriAggiunti
            ParametriAggiuntivi_QueryString = New AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString
            ParametriAggiuntivi_QueryString.key = item.key
            ParametriAggiuntivi_QueryString.value = item.value
            ParametriAggiuntivi_QueryString.codifica = item.codifica
            ParametriAggiuntivi.Add(ParametriAggiuntivi_QueryString)
        Next

        Return ParametriAggiuntivi
    End Function

End Module
