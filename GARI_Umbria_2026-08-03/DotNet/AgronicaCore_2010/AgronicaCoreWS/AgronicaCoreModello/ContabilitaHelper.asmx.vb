Imports System.Runtime.Serialization
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class ContabilitaHelper
    Inherits System.Web.Services.WebService

    'Funzioni spostate internamente al sito dell'agenda (GestioneContabilita\DocContabile_WS.aspx)

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function LeggiTestataDocumento(ByVal objP_server As String,
    '                                      ByVal objP_utenti As String,
    '                                      ByVal piva As String,
    '                                      ByVal saCod As Integer,
    '                                      ByVal idAgenda As Integer,
    '                                      ByVal lavCod As Integer
    '                                      ) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim xRisp As Contabilita_Testata
    '    Dim msgError As String = ""

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    ElseIf objP_utenti = "" Then
    '        r.Errore = "objP_utenti non valorizzato"
    '        Return r
    '    End If

    '    Try

    '        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
    '        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

    '        Dim objContabHelper As New AgronicaCoreModello.ContabilitaHelper(objParametriServer, objParametriUtenti, True)
    '        xRisp = objContabHelper.LeggiTestataDocumento(piva, saCod, idAgenda, lavCod, msgError)

    '        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

    '        r.RispostaStringa = JsonConvert.SerializeObject(xRisp, settingLoc)
    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False)
    '    End Try

    '    Return r

    'End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function ModificaTestataDocumento(ByVal objP_server As String,
    '                                         ByVal objP_utenti As String,
    '                                         ByVal piva As String,
    '                                         ByVal contabTestata As String,
    '                                         ByVal messaggioDettagliato As Boolean
    '                                         ) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim xRisp As Boolean
    '    Dim msgError As String = ""

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    ElseIf objP_utenti = "" Then
    '        r.Errore = "objP_utenti non valorizzato"
    '        Return r
    '    End If

    '    Try
            
    '        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
    '        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

    '        'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
    '        '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
    '        '       e
    '        '   #10/30/2017 12:00:00 AM#

    '        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

    '        'La data è corretta se arriva da griglia Kendo (verificare in caso di altri metodi se mi arriva sempre uguale)
    '        Dim agroContabT As New Contabilita_Testata 'With { .Piva = piva }

    '        Dim objContabT As Contabilita_Testata = JsonConvert.DeserializeObject(contabTestata, agroContabT.GetType(), settingLoc)

    '        objContabT.Piva = piva

    '        Dim objContabHelper As New AgronicaCoreModello.ContabilitaHelper(objParametriServer, objParametriUtenti, True)
    '        xRisp = objContabHelper.AggiornaTestataDocumento(objContabT, messaggioDettagliato, msgError)

    '        r.RispostaOK = xRisp
    '        r.RispostaStringa = If(xRisp = True, "OK", msgError)

    '    Catch ex As Exception

    '        r.RispostaOK = False
    '        r.RispostaStringa = If(msgError <> "", msgError, "Errore Modifica")
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return r

    'End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function ScriviTestataDocumento(ByVal objP_server As String,
    '                                       ByVal objP_utenti As String,
    '                                       ByVal piva As String,
    '                                       ByVal contabTestata As String,
    '                                       ByVal messaggioDettagliato As Boolean
    '                                       ) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim xRisp As Boolean
    '    Dim msgError As String = ""

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    ElseIf objP_utenti = "" Then
    '        r.Errore = "objP_utenti non valorizzato"
    '        Return r
    '    End If

    '    Try
            
    '        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
    '        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

    '        'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
    '        '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
    '        '       e
    '        '   #10/30/2017 12:00:00 AM#

    '        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

    '        'La data è corretta se arriva da griglia Kendo (verificare in caso di altri metodi se mi arriva sempre uguale)
    '        Dim agroContabT As New Contabilita_Testata 'With { .Piva = piva }

    '        Dim objContabT As Contabilita_Testata = JsonConvert.DeserializeObject(contabTestata, agroContabT.GetType(), settingLoc)

    '        objContabT.Piva = piva
            
    '        Dim objContabHelper As New AgronicaCoreModello.ContabilitaHelper(objParametriServer, objParametriUtenti, True)
    '        xRisp = objContabHelper.ScriviTestataDocumento(objContabT, messaggioDettagliato, msgError)

    '        r.RispostaOK = xRisp
    '        r.RispostaStringa = If(xRisp = True, "OK", msgError)

    '    Catch ex As Exception

    '        r.RispostaOK = False
    '        r.RispostaStringa = If(msgError <> "", msgError, "Errore Scrittura")
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return r

    'End Function

End Class