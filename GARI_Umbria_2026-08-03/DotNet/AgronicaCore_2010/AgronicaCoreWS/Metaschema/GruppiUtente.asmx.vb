Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class GruppiUtente
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviGruppoUtente(ByVal InData As CoreWS_Generic(Of ScriviGruppoUtente)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objGruppi_W As New AgronicaCoreUtentiDAL.Gruppi_Utente_W

            Select Case InData.InData.Operazione
                Case enum_TipoOperazioneDB.Scrittura
                    Dim codice = CalcolaNuovoCodice(objParametri_Utenti)
                    objGruppi_W.Scrivi(
                        codice,
                        InData.InData.Gruppo.descrizione,
                        InData.InData.Gruppo.Identificativo,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        objParametri_Utenti
                    )

                Case enum_TipoOperazioneDB.Modifica

                    objGruppi_W.Modifica(
                        InData.InData.Gruppo.codice,
                        InData.InData.Gruppo.descrizione,
                        InData.InData.Gruppo.Identificativo,
                        "",
                        objParametri_Utenti
                    )

                Case enum_TipoOperazioneDB.Cancellazione

                    If (InData.InData.Gruppo.codice = 0) Then
                        Throw New Exception("Il codice gruppo è 0 / non definito.")
                    End If

                    objGruppi_W.Cancella(
                        InData.InData.Gruppo.codice,
                        "",
                        objParametri_Utenti
                    )

            End Select

            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    Private Function CalcolaNuovoCodice(objP_Utenti As AgronicaCoreParametri) As Integer
        Dim objRead As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim tuttiGruppi = 0
        Dim gruppoDefault = 99999999
        Dim codiceDecr = " Gruppi_Utente_cod desc "
        Dim eccettoDefault = " Gruppi_Utente_cod != " & gruppoDefault & " "
        Dim maxItem = objRead.Leggi(tuttiGruppi, eccettoDefault, codiceDecr, objP_Utenti).
            AsEnumerable.FirstOrDefault()
        If IsNothing(maxItem) Then
            Return 1
        ElseIf maxItem.Item("Gruppi_Utente_cod") = (gruppoDefault - 1) Then
            Return gruppoDefault + 2
        Else
            Return maxItem.Item("Gruppi_Utente_cod") + 1
        End If
    End Function

End Class