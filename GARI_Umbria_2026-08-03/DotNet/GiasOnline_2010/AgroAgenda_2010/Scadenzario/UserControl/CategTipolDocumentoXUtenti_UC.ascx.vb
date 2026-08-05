Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CategTipolDocumentoXUtenti_UC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objparametri_server_string As String

    Public Rag_Soc_Azienda As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            inizializzoObjParametri()

            'Recupero la ragione sociale dell'azienda con cui sono entrato
            Dim QS_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                              AgroKey_EncoderDecoder,
                                              Server)

            If Not String.IsNullOrEmpty(QS_Piva) Then
                Dim obj As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Rag_Soc_Azienda = obj.RagSoc_from_Piva(QS_Piva, objParametri_Server)
                hdRag_Soc_Azienda.Value = Rag_Soc_Azienda
            End If

        Catch ex As Exception

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore & Chr(13) & ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function


    Public Shared Function Carica_Griglia_Utenti() As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim obj As New AgronicaCoreUtentiDAL.Utenti_Read


            'Escludo l'utente SuperUser perchè ha le autorizzazioni su tutte le Categorie e Tipologie
            DT = obj.LeggiUtenteConGruppo(True,
                                          "",
                                          "",
                                          objParametri_Utenti)

            Dim UtentiList As New List(Of Object)
            For i = 0 To DT.Rows.Count - 1

                UtentiList.Add(New With
                                {
                                   .NOME = CStr(DT.Rows(i).Item("Nome")),
                                   .COGNOME = CStr(DT.Rows(i).Item("Cognome")),
                                   .USER = CStr(DT.Rows(i).Item("UserName")),
                                   .GRUPPI_UTENTE_DES = CStr(DT.Rows(i).Item("Gruppi_Utente_des")),
                                   .rowId = Guid.NewGuid
                                })

            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UtentiList, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Carica_Griglia_Categoria_Tipologia() As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim obj As New AgronicaCoreScadenziario.Alert_Tipologia_R
            DT = obj.Leggi_Tipologia_UNION_Categoria(0, 0,
                                                     objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Carica_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim obj As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

            Dim XFiltroAggiuntivo As String = "CategTipologiaDocumentiXUtenti.Autorizzato IN (" & Tipo_Permesso_Documentale.Gestione_Completa & "," & Tipo_Permesso_Documentale.Lettura & ")"

            DT = obj.Leggi("",
                           0, 0,
                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                           objParametri_Server,
                           objParametri_Utenti,
                           FiltroAggiuntivo:=XFiltroAggiuntivo,
                           FiltroImpreseVisibiliXUtente:=True)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Carico_Autorizzazione_UtentixCategoriaTipologia(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim obj As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

            DT = obj.Leggi(piva,
                           0, Nothing,
                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                           objParametri_Server, Nothing)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Scrivi_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelutenti As String, ByVal modelcategoriatipologia As String, ByVal Autorizzato As Integer)
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim parametriUtenti As List(Of ModelUtenti) = JsonConvert.DeserializeObject(Of List(Of ModelUtenti))(modelutenti, settingLoc)
        Dim parametriCategoriaTipologia As List(Of ModelCategoriaTipologia) = JsonConvert.DeserializeObject(Of List(Of ModelCategoriaTipologia))(modelcategoriatipologia, settingLoc)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objW As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_W
        Dim objR As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R
        Dim r As New RispostaStandard
        Dim DT As DataTable


        If Not parametriUtenti Is Nothing AndAlso Not parametriCategoriaTipologia Is Nothing Then
            Try

                For Each ut As ModelUtenti In parametriUtenti
                    For Each categtipo As ModelCategoriaTipologia In parametriCategoriaTipologia
                        DT = objR.Leggi(piva, categtipo.ID_Area, categtipo.ID_Tipologia,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, objParametri_Utenti,, ut.USER)
                        If DT.Rows.Count = 0 Then
                            objW.Scrivi(piva, ut.USER, categtipo.ID_Area, categtipo.ID_Tipologia, Autorizzato, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                        Else
                            objW.Modifica(piva, ut.USER, categtipo.ID_Area, categtipo.ID_Tipologia, Autorizzato, objParametri_Server)
                        End If
                    Next
                Next

                r.RispostaOK = True

            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                Return r
            End Try
        End If


        Return r

    End Function


    Public Shared Function Modifica_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelutenti As String, ByVal modelcategoriatipologia As String, ByVal modelcategoriatipologiaxutenti As String) As RispostaStandard

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objW As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_W
        Dim objR As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R
        Dim r As New RispostaStandard
        Dim DT As DataTable

        r.RispostaOK = True


        If String.IsNullOrEmpty(modelcategoriatipologiaxutenti) Then

            Dim parametriUtenti As List(Of ModelUtenti) = JsonConvert.DeserializeObject(Of List(Of ModelUtenti))(modelutenti, settingLoc)
                Dim parametriCategoriaTipologia As List(Of ModelCategoriaTipologia) = JsonConvert.DeserializeObject(Of List(Of ModelCategoriaTipologia))(modelcategoriatipologia, settingLoc)

            If Not parametriUtenti Is Nothing AndAlso Not parametriCategoriaTipologia Is Nothing Then
                Try

                    For Each ut As ModelUtenti In parametriUtenti
                        For Each categtipo As ModelCategoriaTipologia In parametriCategoriaTipologia
                            DT = objR.Leggi(piva, categtipo.ID_Area, categtipo.ID_Tipologia,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, objParametri_Utenti,, ut.USER)
                            If DT.Rows.Count = 0 Then
                                'Ora non lo scrivo più il record se non sono autorizzato
                                'objW.Scrivi(piva, ut.USER, categtipo.ID_Area, categtipo.ID_Tipologia, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                            Else
                                'Ora non lo modifico più il record se non sono autorizzato
                                'objW.Modifica(piva, ut.USER, categtipo.ID_Area, categtipo.ID_Tipologia, 0, objParametri_Server)

                                'Se prima era autorizzato ed ora non lo è più cancello il record
                                objW.Cancella(piva, ut.USER, categtipo.ID_Area, categtipo.ID_Tipologia, objParametri_Server)

                            End If

                        Next
                    Next

                    r.RispostaOK = True

                Catch ex As Exception
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                    Return r
                End Try
            End If

        Else
            Dim parametriCategoriaTipologiaUtenti As List(Of ModelCategoriaTipologiaXUtenti) = JsonConvert.DeserializeObject(Of List(Of ModelCategoriaTipologiaXUtenti))(modelcategoriatipologiaxutenti, settingLoc)
            Try
                For Each ct As ModelCategoriaTipologiaXUtenti In parametriCategoriaTipologiaUtenti
                    objW.Modifica(piva, ct.Username, ct.ID_Area, ct.ID_Tipologia, ct.Autorizzato, objParametri_Server)
                Next

            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                Return r
            End Try
        End If

        Return r

    End Function


    Public Shared Function Cancella_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelcategoriatipologiaxutenti As String) As RispostaStandard

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim parametriCategoriaTipologiaUtenti As List(Of ModelCategoriaTipologiaXUtenti) = JsonConvert.DeserializeObject(Of List(Of ModelCategoriaTipologiaXUtenti))(modelcategoriatipologiaxutenti, settingLoc)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objW As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_W
        Dim r As New RispostaStandard

        r.RispostaOK = True
        If Not parametriCategoriaTipologiaUtenti Is Nothing Then
            Try
                For Each ct As ModelCategoriaTipologiaXUtenti In parametriCategoriaTipologiaUtenti

                    Dim FiltroAggiuntivo As String = "ID_Tipologia = " & ct.ID_Tipologia & " AND Autorizzato = " & ct.Autorizzato

                    objW.Cancella(ct.Piva, ct.Username, ct.ID_Area, 0, objParametri_Server, FiltroAggiuntivo)
                Next

            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                Return r
            End Try
        End If

        Return r

    End Function

    Private Class ModelUtenti
        Public USER As String = ""
        Public GRUPPI_UTENTE_DES As String = ""
    End Class

    Private Class ModelCategoriaTipologia
        Public ID_Area As Integer? = Nothing
        Public ID_Tipologia As Integer? = Nothing
    End Class

    Private Class ModelCategoriaTipologiaXUtenti
        Public Piva As String = ""
        Public Rag_Soc As String = ""
        Public Username As String = ""
        Public Gruppo_Utente As String = ""
        Public ID_Area As Integer? = Nothing
        Public ID_Tipologia As Integer? = Nothing
        Public Autorizzato As Integer? = Nothing
    End Class

End Class
