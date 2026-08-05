Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreModello
Imports AgronicaCoreScadenziario_BIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Ricette
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RicettaLeggi(ByVal objP_server As String,
                                 ByVal Ricetta_Cod As Int32,
                                 ByVal Piva As String,
                                 ByVal Sa_Cod As Int32,
                                 ByVal Tipo_Ricetta As Int32,
                                 ByVal Veg_Cod As Int32,
                                 ByVal ForDelete As Boolean,
                                 ByVal FiltriXlettura_vuoti As Boolean,
                                 ByVal RicettaDettagliOrderBy As String
                                 ) As String

        Dim objRic_R As New AgronicaCoreContabBIZ.Ricette_R
        Dim result As String
        Try

            'Prlevo l'obj Parametri
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Leggo la ricetta passando i parametri
            result = objRic_R.Ricetta_Leggi(Ricetta_Cod, Piva, Sa_Cod, Tipo_Ricetta, Veg_Cod, ForDelete, objParametri_Server, FiltriXlettura_vuoti, RicettaDettagliOrderBy)

        Catch ex As Exception

            result = "<DatiRicetta><Errore>Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False) & "</Errore></DatiRicetta>"

        End Try

        Return result
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RicettaLeggi_APP(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal Piva As String,
        ByVal DataDa As String,
        ByVal DataA As String
        ) As RispostaStandard

        Dim objRic_R As New AgronicaCoreContabBIZ.Ricette_R
        Dim result As New RispostaStandard


        Try

            'Prlevo l'obj Parametri
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            ''Leggo la ricetta passando i parametri
            'result = objRic_R.Ricetta_Leggi(Ricetta_Cod, Piva, Sa_Cod, Tipo_Ricetta, Veg_Cod, ForDelete, objParametri_Server, FiltriXlettura_vuoti, RicettaDettagliOrderBy)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            'Dim r1 As List(Of AgronicaCoreEntityFramework_POCO.Ricette) = objRic_R.Ricetta_Leggi_APP(Piva, objParametri_Server)

            Dim Dt_Ricette As DataTable
            Dim Dt_RicetteXNote As DataTable
            Dim Dt_Operazioni As DataTable
            Dim Dt_Dettagli As DataTable
            Dim Dt_DettaglioTecnico As DataTable
            Dim Dt_Destinazioni As DataTable


            Dim letturaRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_R

            'capire il filtro per date...
            DataDa = CostantiPersonalizzate.AGRODATAINIZIO
            DataA = CostantiPersonalizzate.AGRODATAFINE


            Dim ErrMsg As String = ""
            Dim rvalLetturaRicette As Integer =
            letturaRicette.Ricette_Operazioni_LeggiPerAPP(
                ErrMsg,
                Piva,
                0,
                0,
                0,
                0,
                DataDa,
                DataA,
                "",
                Dt_Ricette,
                Dt_RicetteXNote,
                Dt_Operazioni,
                Dt_Dettagli,
                Dt_DettaglioTecnico,
                Dt_Destinazioni,
                objParametri_Server,
                objParametri_Utenti
            )

            If rvalLetturaRicette = 0 OrElse ErrMsg <> "" Then
                result.RispostaOK = False
                result.RispostaStringa = ""
                result.Errore = ErrMsg
                Return result
            End If

            Dt_Ricette.TableName = "Ricette"
            Dt_RicetteXNote.TableName = "RicettexNote"
            Dt_Operazioni.TableName = "RicetteOperazioni"
            Dt_Dettagli.TableName = "RicetteDettagli"
            Dt_DettaglioTecnico.TableName = "RicetteDettaglioTecnico"
            Dt_Destinazioni.TableName = "RicetteDestinazioni"

            'aggiusto i tipi di dato
            Dim Dt_Destinazioni_Cloned As DataTable = Dt_Destinazioni.Clone()
            Dt_Destinazioni_Cloned.Columns("Tipo_Destinazione").DataType = GetType(Int32)

            For Each row As DataRow In Dt_Destinazioni.Rows
                Dt_Destinazioni_Cloned.ImportRow(row)
            Next

            Dim dataSet As DataSet = New DataSet("RicettePerScarico")
            dataSet.[Namespace] = "NetFrameWork"

            dataSet.Tables.Add(Dt_Ricette)
            dataSet.Tables.Add(Dt_RicetteXNote)
            dataSet.Tables.Add(Dt_Operazioni)
            dataSet.Tables.Add(Dt_Dettagli)
            dataSet.Tables.Add(Dt_DettaglioTecnico)
            dataSet.Tables.Add(Dt_Destinazioni_Cloned)

            result.RispostaOK = True
            result.RispostaStringa = JsonConvert.SerializeObject(value:=dataSet, formatting:=Formatting.None, settings:=serializerSettings)

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function


    <WebMethod()>
    Public Function RicettaScrivi_APP(ByVal objP_super_server As String,
                                     ByVal objP_server As String,
                                     ByVal objP_utenti As String,
                                     ByVal RicetteDaMemorizzare As RicettePerScarico) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            'Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim unid As String = RicetteDaMemorizzare.guid
            Dim cancellazione As Boolean = False
            Dim importazione As Boolean = False

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                cancellazione = True
            End If

            Dim scriviRicetteApp As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            scriviRicetteApp.Ricetta_Operazione_ScriviPerAPP(
                unid,
                RicetteDaMemorizzare.Ricette,
                RicetteDaMemorizzare.RicetteOperazioni,
                RicetteDaMemorizzare.RicetteDettagli,
                RicetteDaMemorizzare.RicetteDettaglioTecnico,
                RicetteDaMemorizzare.RicetteXNote,
                RicetteDaMemorizzare.RicetteDestinazioni,
                RicetteDaMemorizzare.Attivita,
                RicetteDaMemorizzare.AttivitaMovimenti,
                RicetteDaMemorizzare.AttivitaOperazioni,
                objParametri_Server,
                cancellazione
            )

            If Not RicetteDaMemorizzare.Documenti Is Nothing Then

                Dim scriviAllegati As New Allegati
                For Each Documento In RicetteDaMemorizzare.Documenti
                    Dim resultAllegati As RispostaStandard = scriviAllegati.DocumentiScrivi_APP(Documento, unid, objParametri_Server, objParametri_Utenti)
                Next
            End If


            ' importazione dati app
            If importazione Then
                Dim msgFinale As String = ""
                Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
                Dim esito As Boolean = objRic_W.ImportaRicetteDaTabelleAPP(objParametri_Server, msgFinale, "", unid)
            End If

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RicettaScrivi(ByVal objP_server As String,
                                  ByVal DatiRicetta As String
                                  ) As String 'As Boolean

        Dim result As String
        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W

        Try

            'Variabile con il codice della nuova ricetta
            Dim OUTPUT_Ricetta_Cod As Int32

            'Prlevo l'obj Parametri
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'scrivo la ricetta passando i parametri
            Dim esito As Boolean = objRic_W.Ricetta_Scrivi(DatiRicetta, OUTPUT_Ricetta_Cod, objParametri_Server)

            If esito Then
                result = "<DatiRicetta><Ricetta_Cod>" & OUTPUT_Ricetta_Cod & "</Ricetta_Cod></DatiRicetta>"
            Else
                result = "<DatiRicetta><Errore>Impossibile salvare la ricetta.</Errore></DatiRicetta>"
            End If

        Catch ex As Exception

            result = "<DatiRicetta><Errore>Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False) & "</Errore></DatiRicetta>"

        End Try

        Return result
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaRicetteDaTabelleAPP(ByVal objP_server As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgFinale As String = ""

        Dim res As New RispostaStandard()

        If Application("ImportaRicetteDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaRicetteDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaRicetteDaTabelleAPP(objParametri_Server, msgFinale)

        Application("ImportaRicetteDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaRicetteAziendaDaTabelleAPP(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgFinale As String = ""

        Dim res As New RispostaStandard()

        If Application("ImportaRicetteDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaRicetteDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaRicetteDaTabelleAPP(objParametri_Server, msgFinale, piva)

        Application("ImportaRicetteDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaRicetteAziendaDaTabelleAPP_NG(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim msgFinale As String = ""
        Dim piva As String = InData.InData
        Dim res As New RispostaStandard()

        If Application("ImportaRicetteDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaRicetteDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaRicetteDaTabelleAPP(objParametri_Server, msgFinale, piva)

        Application("ImportaRicetteDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaAgendaAziendaDaTabelleAPP(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgFinale As String = ""

        Dim res As New RispostaStandard()

        If Application("ImportaAgendaDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaAgendaDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaAgendDaTabelleAPP(objParametri_Server, msgFinale, piva)

        Application("ImportaAgendaDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaAgendaAziendaDaTabelleAPP_NG(InData As Object) As RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim msgFinale As String = ""
        Dim piva As String = InData.InData
        Dim res As New RispostaStandard()

        If Application("ImportaAgendaDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaAgendaDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaAgendDaTabelleAPP(objParametri_Server, msgFinale, piva)

        Application("ImportaAgendaDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaAgendaDaTabelleAPP(ByVal objP_server As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgFinale As String = ""

        Dim res As New RispostaStandard()

        If Application("ImportaAgendaDaTabelleAPP") = "1" Then
            res.RispostaOK = False
            res.Errore = "Procedura già in esecuzione"
            Return res
        End If

        Application("ImportaAgendaDaTabelleAPP") = "1"

        Dim objRic_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esitoFinale As Boolean = objRic_W.ImportaAgendDaTabelleAPP(objParametri_Server, msgFinale)

        Application("ImportaAgendaDaTabelleAPP") = "0"

        res.RispostaOK = esitoFinale

        If esitoFinale = True Then
            res.RispostaStringa = msgFinale
        Else
            res.Errore = msgFinale
        End If

        Return res

    End Function

End Class