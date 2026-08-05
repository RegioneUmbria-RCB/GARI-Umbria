Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreDTOStd.InData.Profilazione
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Utenti_Visibilita
    Inherits System.Web.Services.WebService

    Private Function GetObjParams(Of T)(InData As CoreWS_Generic(Of T))
        Return New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        }
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ModificaVisibilitaUtenti(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtenti)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Try
            objUtentiBIZ.ModificaVisibilitaAziendaUtenti(
                InData.InData.Utenti, InData.InData.AziendeVisibili,
                InData.InData.Sovrascrivi, False,
                obj_Server, obj_Utenti
            )
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function RimuoviVisibilitaUtenti(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtenti)) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Dim objGisBIZ As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W

        Try
            objUtentiBIZ.RimuoviImpreseDaVisibilitaUtenti(InData.InData.Utenti, InData.InData.AziendeVisibili,
                                                         obj_Server, obj_Utenti)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CopiaVisibilitaUtenti(InData As CoreWS_Generic(Of CopiaVisibilitaObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Try
            objUtentiBIZ.CopiaVisibilitaUtentiEstesa(InData.InData.Template, InData.InData.Base,
                InData.InData.CopyHierarchy, InData.InData.CopyProcedures, obj_Utenti)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CreaUtenteConVisibilita(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtente)) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Dim objGisBIZ As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W

        Try

            objUtentiBIZ.ImpostaVisibilitaAzienda(InData.InData.Utente, InData.InData.AziendeVisibili,
                                                  True, True, obj_Server, obj_Utenti)
            objGisBIZ.ImpostaLayerGisSePermessiCartografia(InData.InData.Utente.UserName, obj_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiAziendeVisibilita(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtenti)) 'As rispostaStandard(Of List(Of ImpresexUtentiVisibilita))
        Dim res As New RispostaStandard '(Of List(Of ImpresexUtentiVisibilita))

        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita

        Try

            Dim result = objUtentiBIZ.LeggiAziendeVisibilita(
                InData.InData.Utenti,
                InData.InData.AziendeVisibili,
                obj_Server, obj_Utenti
            )

            res.RispostaStringa = JsonConvert.SerializeObject(result)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiVisibilitaUtenti(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtenti)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim params = GetObjParams(InData)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita

        Try
            Dim imprese = Nothing
            Dim t1 = DateTime.Now

            Dim visibilita = objUtentiBIZ.LeggiVisibilitaUtenti(
                InData.InData.Utenti, InData.InData.AziendeVisibili,
                params.objparametri_server, params.objparametri_Utenti,
                leggiDettagli:=InData.InData.Sovrascrivi
            )
            Dim t2 = DateTime.Now

            If InData.InData.LeggiDatiImprese Then
                imprese = objUtentiBIZ.GetImpreseFromVisibilita(visibilita, params)
            End If
            Dim t3 = DateTime.Now

            Dim result As New With {
                .visibilita = visibilita,
                .imprese = imprese
            }
            res.RispostaStringa = JsonConvert.SerializeObject(result)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiVisibilitaGruppi(InData As CoreWS_Generic(Of LeggiScriviVisibilitaUtenti)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim params = GetObjParams(InData)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita

        Try
            Dim result = Nothing
            Dim imprese = Nothing
            Dim visibilita = Nothing

            Dim groups = InData.InData.Utenti.Select(Function(u) CInt(u.UserName)).ToList
            Dim gropupsReader As New AgronicaCoreUtentiBIZ.Gruppi_UtenteBiz(params.ObjParametri_Server, params.ObjParametri_Utenti)
            Dim usernames = gropupsReader.GetUtentiFromGruppoUtenti(groups).Select.AsParallel.
                Select(Function(r) If(IsDBNull(r("UserName")), "", CStr(r("UserName")))).
                Where(Function(username) Not String.IsNullOrWhiteSpace(username))

            Dim vis = objUtentiBIZ.HannoVisibilitaTotale(usernames, params.objParametri_Utenti)
            Dim fullVis = vis.AsParallel.
                Where(Function(u) u.VisibilitaTotale).
                Select(Function(u) New UtenteDTO With {.UserName = u.Username}).ToList
            Dim reducedVis = vis.AsParallel.
                Where(Function(u) Not u.VisibilitaTotale).
                Select(Function(u) New UtenteDTO With {.UserName = u.Username}).ToList

            If reducedVis.Count > 10_000 Then
                result = New With {
                    .visibilitaTotale = fullVis,
                    .visibilita = reducedVis,
                    .imprese = imprese,
                    .fullResult = False
                }
            Else
                visibilita = objUtentiBIZ.LeggiVisibilitaUtenti(
                    reducedVis, InData.InData.AziendeVisibili,
                    params.ObjParametri_Server, params.ObjParametri_Utenti, True
                )
                If InData.InData.LeggiDatiImprese Then
                    imprese = objUtentiBIZ.GetImpreseFromVisibilita(visibilita, params)
                End If
                result = New With {
                   .visibilitaTotale = fullVis,
                   .visibilita = visibilita,
                   .imprese = imprese,
                   .fullResult = True
                }
            End If

            res.RispostaStringa = JsonConvert.SerializeObject(result)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function HaVisibilitaTotale(InData As CoreWS_Generic(Of List(Of String))) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Try
            Dim result = objUtentiBIZ.HannoVisibilitaTotale(InData.InData, obj_Utenti)
            res.RispostaStringa = JsonConvert.SerializeObject(result)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ControllaStessaVisibilita(InData As CoreWS_Generic(Of List(Of String))) As RispostaStandard
        Dim r As New RispostaStandard
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        Try
            Dim objUtentiVisibilita As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
            'r.RispostaStringa = JsonConvert.SerializeObject(objUtentiVisibilita.ControllaStessaVisibilita(InData.InData, objParametri_Server, objParametri_Utenti))
            r.RispostaStringa = JsonConvert.SerializeObject(objUtentiVisibilita.ControllaStessaVisibilitaLite(InData.InData, GetObjParams(InData)))
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ConfrontaVisibilitaUtenti(InData As CoreWS_Generic(Of ConfrontaVisibilitaUtenti)) As RispostaStandard

        Dim risp As Boolean = False

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objUtentiVisibilita As New AgronicaCoreUtentiBIZ.Utenti_Visibilita

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dim risposta = objUtentiVisibilita.ConfrontaVisibilitaUtenti(InData.InData.utente, InData.InData.utenteConfronto, objParametri_Server, objParametri_Utenti)
            r.RispostaStringa = JsonConvert.SerializeObject(risposta, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CheckFathersInList(InData As CoreWS_Generic(Of ImpresaDto())) As RispostaStandard
        Dim r As New RispostaStandard
        Dim params = GetObjParams(InData)
        Try
            Dim visReader As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
            Dim risposta = visReader.CheckIfAnyHasSons(InData.InData, params)
            r.RispostaStringa = JsonConvert.SerializeObject(risposta)
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

End Class