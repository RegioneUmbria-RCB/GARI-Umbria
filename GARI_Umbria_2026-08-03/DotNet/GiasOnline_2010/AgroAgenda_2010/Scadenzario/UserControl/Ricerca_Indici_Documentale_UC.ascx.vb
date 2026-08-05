
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility


Public Class Ricerca_Indici_Documentale_UC
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

            'Impostazione Possibilità di modificare l'indice (solo se Utente = SuperUser)
            If objParametri_Utenti.UtenteCodFiscale = objParametri_Utenti.PivaSuperUser Then
                hdRiservato.Value = 1
            Else
                hdRiservato.Value = 0
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

        '---
    End Sub


    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function


End Class
