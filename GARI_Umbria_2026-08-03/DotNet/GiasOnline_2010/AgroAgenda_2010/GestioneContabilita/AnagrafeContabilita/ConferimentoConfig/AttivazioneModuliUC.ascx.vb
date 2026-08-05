
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEFatturaDAL
Imports AgGateway.ADAPT.ApplicationDataModel.Documents

Public Class AttivazioneModuli
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Shared Function LeggiStatoAttivazioni(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim stato As List(Of KeyValuePair(Of String, Boolean)) = LeggiStatoAttivazioni(piva, objParametri_Server)
            stato.AddRange(ConsentiDisabilitazioneModuli(piva, objParametri_Server))
            Dim obj As New Dictionary(Of String, Boolean)
            stato.ForEach(Sub(x) obj.Add(x.Key, x.Value))

            r.RispostaStringa = JsonConvert.SerializeObject(obj)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function SalvaAttivazioneModuli(ByVal piva As String, trasformazioniVegetali As Boolean, trasformazioniAnimali As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            ScriviStatoAttivazioni(piva, trasformazioniVegetali, trasformazioniAnimali, objParametri_Server)
            Dim obj = LeggiStatoAttivazioni(piva)

            r.RispostaStringa = JsonConvert.SerializeObject(obj)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Private Shared Sub ScriviStatoAttivazioni(piva As String, trasformazioniVegetali As Boolean, trasformazioniAnimali As Boolean,
                                              objParametri_Server As AgronicaCoreParametri)
        Dim scriviAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_W

        If trasformazioniVegetali Then
            scriviAnagrafeLog.Scrivi_AttivazioneModuloFreshAndFood(piva, objParametri_Server)
        Else
            scriviAnagrafeLog.Cancella_AttivazioneModuloFreshAndFood(piva, objParametri_Server)
        End If

        If trasformazioniAnimali Then
            scriviAnagrafeLog.Scrivi_AttivazioneModuloZoo(piva, objParametri_Server)
        Else
            scriviAnagrafeLog.Cancella_AttivazioneModuloZoo(piva, objParametri_Server)
        End If

    End Sub

    Private Shared Function LeggiStatoAttivazioni(piva As String, objParametri_Server As AgronicaCoreParametri) As List(Of KeyValuePair(Of String, Boolean))
        Dim result As New List(Of KeyValuePair(Of String, Boolean))


        Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
        Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                   "(Modulo_Generazione = 2 OR Modulo_Generazione = 5)", "",
                                                   objParametri_Server)

        result.Add(ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.FreshFood, "trasformazioniVegetali"))
        result.Add(ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.Zoo, "trasformazioniAnimali"))

        Return result
    End Function

    Private Shared Function ModuloAttivo(dtAnagrafeLog As DataTable, codiceModuloGenerazione As enum_Omni_Modulo_Generazione,
                                    definizioneCampo As String) As KeyValuePair(Of String, Boolean)
        Dim attivo As Boolean = False

        If Not IsNothing(dtAnagrafeLog) Then
            attivo = dtAnagrafeLog.AsEnumerable() _
            .Any(Function(x) IsNumeric(x.Item("Modulo_Generazione")) AndAlso
                x.Item("Modulo_Generazione") = CInt(codiceModuloGenerazione))
        End If

        Return (New KeyValuePair(Of String, Boolean)(definizioneCampo, attivo))
    End Function

    Private Shared Function ConsentiDisabilitazioneModuli(piva As String, objParametri_Server As AgronicaCoreParametri) As List(Of KeyValuePair(Of String, Boolean))
        Dim result As New List(Of KeyValuePair(Of String, Boolean))
        Try
            Dim objOModuli_Referenze_Config_Testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R
            Dim DT = objOModuli_Referenze_Config_Testata.LeggiModuliConferimento(piva, objParametri_Server)

            If Not IsNothing(DT) Then
                Dim disattivaTrasformazioniVegetali As Boolean = DT.AsEnumerable() _
                    .Any(Function(x) IsNumeric(x.Item("Modulo_Generazione")) AndAlso
                        x.Item("Modulo_Generazione") = CInt(enum_Omni_Modulo_Generazione.FreshFood))

                Dim disattivaTrasformazioniAnimali As Boolean = DT.AsEnumerable() _
                    .Any(Function(x) IsNumeric(x.Item("Modulo_Generazione")) AndAlso
                        x.Item("Modulo_Generazione") = CInt(enum_Omni_Modulo_Generazione.Zoo))

                result.Add(New KeyValuePair(Of String, Boolean)("disattivaTrasformazioniVegetali", disattivaTrasformazioniVegetali))
                result.Add(New KeyValuePair(Of String, Boolean)("disattivaTrasformazioniAnimali", disattivaTrasformazioniAnimali))
            Else
                result.Add(New KeyValuePair(Of String, Boolean)("disattivaTrasformazioniVegetali", False))
                result.Add(New KeyValuePair(Of String, Boolean)("disattivaTrasformazioniAnimali", False))
            End If


        Catch ex As Exception
            Throw
        End Try

        Return result
    End Function
End Class