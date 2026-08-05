Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports System.Drawing
Imports AgronicaCoreVarieBIZ

Public Class SchedaRilieviWS
    Inherits System.Web.UI.Page



    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca(ByVal DataDa As String, ByVal DataA As String, ByVal IncludiSoloDatiRilevati As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..
            'Inserire il codice QUI..

            Dim dt As DataTable
            Dim BISrl As New AgronicaCoreStampeDAL.BI_SchedaRilievi_R
            dt = BISrl.CaricaDSRilAvvAus(
                DataDa,
                DataA,
                IncludiSoloDatiRilevati,
                "",
                "",
                objParametri_Server,
                objParametri_Utenti
            )

            dt.Columns.Remove("AV_DES")
            dt.Columns.Remove("Ordine")
            dt.Columns.Remove("Qta")
            dt.Columns.Remove("Sup_Ha")
            dt.Columns.Remove("Sup_Are")
            'dt.Columns.Remove("id_agenda")
            'dt.Columns.Remove("veg_des")
            dt.Columns.Remove("DescrizioneOperazione")

            dt.Columns.Remove("Appezzamento_Descrizione")
            dt.Columns.Remove("Appezzamento_Indirizzo")

            'Dim pivotatore As New Pivot(dt)

            'Dim dtPivotata As DataTable = _
            '    pivotatore.Pivot(dt, dt.Columns("UDM_DES"), dt.Columns("qta_Descrittiva"))

            Dim str_RISPOSTA As String = DTToJson(dt)

            r.RispostaOK = True
            r.RispostaStringa = str_RISPOSTA

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function



    Private Shared Function DTToJson(ByVal dt As DataTable) As String
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim l As New List(Of ColonneNome)


        l.Add(New ColonneNome("Latitudine", "Latitudine", "string"))
        l.Add(New ColonneNome("Longitudine", "Longitudine", "string"))
        l.Add(New ColonneNome("DataOperazione", "Data", "string"))

        l.Add(New ColonneNome("RagioneSociale", "RagioneSociale", "string"))
        l.Add(New ColonneNome("sa_nome", "Centro Aziendale", "string"))
        l.Add(New ColonneNome("DescrizionePunto", "Descrizione Punto", "string"))
        l.Add(New ColonneNome("Veg_Des", "Specie", "string"))
        l.Add(New ColonneNome("Cul_Des", "Cultivar", "string"))

        'l.Add(New ColonneNome("Progetto", "Progetto", "string"))

        l.Add(New ColonneNome("NoteGenerali", "NoteGenerali", "string"))

        l.Add(New ColonneNome("FF_DES", "Fase Fenologica Foglia", "string"))
        l.Add(New ColonneNome("FF_DES2", "Fase Fenologica Grappolo", "string"))

        'pivotare su colonna
        l.Add(New ColonneNome("AV_DES", "Descrizione Avversità", "string"))
        l.Add(New ColonneNome("Utente", "Tecnico Referente", "string"))
        'dati pivotati
        l.Add(New ColonneNome("Qta", "Qta", "string"))
        l.Add(New ColonneNome("UDM_DES", "Rilievo", "string"))
        l.Add(New ColonneNome("qta_Descrittiva", "Qta Descrittiva", "string"))


        'Dim ToExclude As String = "PIVA,RagioneSociale,ff_Des2,id_agenda"
        'For Each c As DataColumn In dt.Columns
        '    Dim cName As String = c.ColumnName
        '    If (From i In l Where i._Nome_colonna_DT = cName).Count = 0 AndAlso _
        '        Not ToExclude.Contains(cName) Then

        '        l.Add(New ColonneNome(cName, cName, "string"))

        '    End If



        'Next


        Return js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipiDato:=True)

        'Return js.JSON_DataTable(dt, l, AssegnaAutomaticamenteTipiDato:=True)



    End Function


End Class