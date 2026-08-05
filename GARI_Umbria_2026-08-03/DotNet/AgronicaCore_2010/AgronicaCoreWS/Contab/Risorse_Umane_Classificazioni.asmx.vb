Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
'Imports AgronicaCoreRegVinoDAL

'<System.Web.Script.Services.ScriptService()> _

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Risorse_Umane_Classificazioni
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="id_risum_cl"></param>
    ''' <param name="codice"></param>
    ''' <param name="descrizione"></param>
    ''' <param name="descr_breve"></param>
    ''' <param name="visibilita"></param>
    ''' <param name="chkDefault"></param>
    ''' <param name="Flag_Aggiungi_Visibilita_Tutti"></param>
    ''' <param name="Flag_Aggiungi_Col_x_VB6"></param>
    ''' <param name="objP_server"></param>
    ''' <returns></returns>
    ''' <remarks>Utilizzato dal LAN in FormContatti e FormRu_Classificazioni --> RICORDARSI DI CAMBIARLE</remarks>
    <WebMethod()> _
    Public Function WS_RU_Classificazioni_Leggi(ByVal piva As String, _
                                                ByVal id_risum_cl As Integer, _
                                                ByVal codice As String, _
                                                ByVal descrizione As String, _
                                                ByVal descr_breve As String, _
                                                ByVal visibilita As Integer, _
                                                ByVal chkDefault As Integer, _
                                                ByVal Flag_Aggiungi_Visibilita_Tutti As Boolean, _
                                                ByVal Flag_Aggiungi_Col_x_VB6 As Boolean, _
                                                ByVal objP_server As String
                                                ) As String
        Dim r As New rispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objRisUmClass As New AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_R

            'Dim DT As DataTable = objRisUmClass.Leggi_SQL(piva, id_risum_cl, codice, _
            '                                              descrizione, descr_breve, visibilita, chkDefault, _
            '                                              Flag_Aggiungi_Visibilita_Tutti, Flag_Aggiungi_Col_x_VB6, _
            '                                              "", "", objParametri_Server)


            Dim EFstr As String = objRisUmClass.Leggi_EF_JSON(piva, id_risum_cl, codice, _
                                                              descrizione, descr_breve, visibilita, chkDefault, _
                                                              Flag_Aggiungi_Visibilita_Tutti, Flag_Aggiungi_Col_x_VB6, _
                                                              objParametri_Server)

            r.RispostaOK = True

            'r.RispostaStringa = DammiWaTableClassificazioni(DT, Flag_Aggiungi_Col_x_VB6)
            r.RispostaStringa = DammiWaTableClassificazioniJson(EFstr, Flag_Aggiungi_Col_x_VB6)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function


    ''' <summary>
    ''' Permette l'aggiornamento dell'entità, il suo aggiornamento o la cancellazione
    ''' </summary>
    ''' <param name="piva">Partita Iva</param>
    ''' <param name="id_risum_cl"></param>
    ''' <param name="codice">Codice Alfanumerico</param>
    ''' <param name="descrizione"></param>
    ''' <param name="descr_breve"></param>
    ''' <param name="visibilita">Visibilità sui Rapporti contabili che deve avere la classificazione (0 = tutti)</param>
    ''' <param name="chkDefault">0 = NO, 1 = Sì</param>
    ''' <param name="objP_server"></param>
    ''' <param name="OperazioneDB">1 = insert, 2 = update, 3 = delete, 0 = non specificata(tenta insert)</param>
    ''' <returns></returns>
    ''' <remarks>Utilizzato dal LAN in FormRu_Classificazioni --> RICORDARSI DI CAMBIARLE</remarks>
    <WebMethod()> _
    Public Function WS_RU_Classificazioni_AddUpdDel(ByVal piva As String, _
                                                    ByVal id_risum_cl As Integer, _
                                                    ByVal codice As String, _
                                                    ByVal descrizione As String, _
                                                    ByVal descr_breve As String, _
                                                    ByVal visibilita As Integer, _
                                                    ByVal chkDefault As Integer, _
                                                    ByVal OperazioneDB As Integer, _
                                                    ByVal objP_server As String _
                                                    ) As String

        '   OperazioneDB = 0 ==> non viene specificato

        Dim r As New rispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objRisUmClass As New AgronicaCoreAnagrafeDAL.Risorse_Umane_Classificazioni_W

            strRisposta = objRisUmClass.Aggiorna_EF_IUD(piva, id_risum_cl, codice, _
                                                        descrizione, descr_breve, visibilita, chkDefault, _
                                                        OperazioneDB, _
                                                        objParametri_Server, Nothing)

            r.RispostaStringa = strRisposta

            If strRisposta = "INSERT: OK" Or strRisposta = "UPDATE: OK" Or strRisposta = "DELETE: OK" Then
                r.RispostaOK = True
            Else
                r.RispostaOK = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function


    '############################################################################ 

    'Metodi normali
    Public Function WaTable_From_JSON_Data(json As String, l As List(Of ColonneNome)) As String
        Dim JsonString As New StringBuilder()

        JsonString.Append("{ ")
        JsonString.Append(" ""cols"": " & JSON_DataTable.Create_cols(l))
        JsonString.Append(" ,""rows"": " & json)
        JsonString.Append("} ")

        Return JsonString.ToString()
    End Function

    Function DammiWaTableClassificazioniJson(ByVal json As String, ByVal Flag_Aggiungi_Col_x_VB6 As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        If Flag_Aggiungi_Col_x_VB6 = True Then
            c = New ColonneNome("Select_Col", "Select_Col", "int")
            c._placeHolder = "..."
            c._FormatoParticolare = "250"
            l.Add(c)
        End If

        c = New ColonneNome("Piva", "Piva", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Id_RisUm_CL", "Id_RisUm_CL", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Codice", "Codice", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Descrizione", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4700"
        l.Add(c)


        c = New ColonneNome("Descr_Breve", "Descr_Breve", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        c = New ColonneNome("Visibilita", "Visibilita", "int")
        c._placeHolder = "..."

        If Flag_Aggiungi_Col_x_VB6 = True Then
            c._FormatoParticolare = "0"
        Else
            c._FormatoParticolare = "1000"
        End If

        l.Add(c)

        c = New ColonneNome("ChkDefault", "ChkDefault", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        If Flag_Aggiungi_Col_x_VB6 = True Then

            c = New ColonneNome("Visibilita_Des", "Visibilita_Des", "string")
            c._placeHolder = "..."
            c._FormatoParticolare = "3500"
            l.Add(c)

            c = New ColonneNome("OperazioneDB", "Operazione", "int")
            c._placeHolder = "..."
            c._FormatoParticolare = "0"
            l.Add(c)

            c = New ColonneNome("$id", "$id", "int")
            c._placeHolder = "..."
            c._FormatoParticolare = "0"
            l.Add(c)

        End If

        Dim risp As String = WaTable_From_JSON_Data(json, l)

        Return risp
    End Function

    Function DammiWaTableClassificazioni(ByVal dt As DataTable, ByVal Flag_Aggiungi_Col_x_VB6 As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        If Flag_Aggiungi_Col_x_VB6 = True Then
            c = New ColonneNome("Select_Col", "Select_Col", "int")
            c._placeHolder = "..."
            c._FormatoParticolare = "250"
            l.Add(c)
        End If

        c = New ColonneNome("Piva", "Piva", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Id_RisUm_Cl", "Id_RisUm_Cl", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Codice", "Codice", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Descrizione", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4700"
        l.Add(c)


        c = New ColonneNome("Descr_Breve", "Descrizione Breve", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        c = New ColonneNome("Visibilita", "Visibilita_Cod", "int")
        c._placeHolder = "..."

        'If Flag_Aggiungi_Col_x_VB6 = True Then
        'c._FormatoParticolare = "0"
        'Else
        c._FormatoParticolare = "1000"
        'End If

        l.Add(c)

        c = New ColonneNome("ChkDefault", "ChkDefault", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "250"
        l.Add(c)

        If Flag_Aggiungi_Col_x_VB6 = True Then
            c = New ColonneNome("Visibilita_Des", "Visibilita", "string")
            c._placeHolder = "..."
            c._FormatoParticolare = "3000"
            l.Add(c)

            c = New ColonneNome("OperazioneDB", "Operazione", "int")
            c._placeHolder = "..."
            c._FormatoParticolare = "250"
            l.Add(c)
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiComboClassificazioni(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Id_RisUm_Cl", "Id_RisUm_Cl", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Descrizione", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

End Class
