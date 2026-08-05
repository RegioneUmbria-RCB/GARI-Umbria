
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Scad_CategorieTipologie_UC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            ' Nothing

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
        '---
    End Sub


    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    Public Shared Function ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Msg As String = ""

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            'Questa lettura la faccio solamente nel caso si voglia eliminare una Tipologia, perchè gli Indici
            'non si associano a delle Aree.
            If ID_Tipologia <> 0 Then
                Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
                Dim DT_Alert_IndicexTipologia = leggi.LeggiIndicixTipologia("", ID_Area, ID_Tipologia,
                                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                            0, objParametri_Server)

                If DT_Alert_IndicexTipologia IsNot Nothing AndAlso DT_Alert_IndicexTipologia.Rows.Count > 0 Then
                    Msg += "-Tutti gli Indici Associati. <br>"
                End If
            End If

            'Se ID_Tipologia è 0 passo Nothing alla funzione di lettura CategTipologiaDocumentiXUtenti
            Dim ID_TipologiaxCategTipologiaDocumentiXUtenti As Integer? = ID_Tipologia

            If ID_TipologiaxCategTipologiaDocumentiXUtenti = 0 Then
                ID_TipologiaxCategTipologiaDocumentiXUtenti = Nothing
            End If

            Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

            Dim DT_CategTipologiaDocumentiXUtenti = obj_CategTipologiaDocumentiXUtenti.Leggi("",
                                                                                               ID_Area,
                                                                                               ID_TipologiaxCategTipologiaDocumentiXUtenti,
                                                                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                               objParametri_Server,
                                                                                                objParametri_Utenti)

            If DT_CategTipologiaDocumentiXUtenti IsNot Nothing AndAlso DT_CategTipologiaDocumentiXUtenti.Rows.Count > 0 Then
                Msg += "-Tutte le Autorizzazioni date agli Utenti. <br>"
            End If

            r.RispostaStringa = Msg

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class
