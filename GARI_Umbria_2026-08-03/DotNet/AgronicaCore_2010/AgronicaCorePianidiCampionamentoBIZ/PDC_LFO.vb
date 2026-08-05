Imports System.Web
Imports AgronicaCoreDataProvider

Public Class PDC_LFO

    Public Property PivaSuperUser As String
    Public Property Id_PDC_Testata As Integer
    Public Property ID_LFO As Integer
    Public Property LFO_Des As String

End Class


'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################

Public Class PDC_LFO_Helper
    Public Shared Sub Carica(ByVal Id_Testata As Integer, ByVal ID_LFO As Integer, ByRef oggetto As PDC_LFO, ByVal objParametri As AgronicaCoreParametri)
        
        Dim objLFO As New AgronicaCorePianidiCampionamentoDAL.LFO_R
        Dim dt As DataTable = objLFO.Leggi(Id_Testata, ID_LFO, "", objParametri)

        If dt.Rows.Count > 0 Then
            oggetto.ID_LFO = dt.Rows(0).Item("ID_LFO")
            oggetto.Id_PDC_Testata = dt.Rows(0).Item("ID_PDC_Testata")
            oggetto.LFO_Des = dt.Rows(0).Item("LFO_Des")
        End If
    End Sub

    Public Shared Sub CaricaLista(ByVal Id_Testata As Integer, ByRef ListaOggetto As List(Of PDC_LFO), ByVal objParametri As AgronicaCoreParametri)
        
        Dim objLFO As New AgronicaCorePianidiCampionamentoDAL.LFO_R
        Dim dt As DataTable = objLFO.Leggi(Id_Testata, 0, "", objParametri)

        ListaOggetto.Clear()

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim oggetto As New PDC_LFO With {
                .ID_LFO = dt.Rows(i).Item("ID_LFO"),
                .Id_PDC_Testata = dt.Rows(i).Item("ID_PDC_Testata"),
                .LFO_Des = dt.Rows(i).Item("LFO_Des")
            }
            ListaOggetto.Add(oggetto)
        Next

    End Sub

    Public Shared Function Salva(ByRef oggetto As PDC_LFO, ByVal objParametri As AgronicaCoreParametri) As String
        Dim objW As New AgronicaCorePianidiCampionamentoDAL.LFO_W
        Dim xRisp As String = ""
        'controllo se ho già un id_lfo 
        If oggetto.ID_LFO > 0 Then
            'sono in MODIFICA
            objW.Modifica(oggetto.Id_PDC_Testata, oggetto.ID_LFO, oggetto.LFO_Des, objParametri)
            Return ""
        Else
            'sono in INSERISCI
            Dim objSq As New AgronicaCoreDataProvider.Agro_Sequenze
            'TODO: REFACTOR = Togli uso Session!!!
            oggetto.ID_LFO = objSq.NuovoId_Tabella("PDC_LFO", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
            objW.Scrivi(oggetto.Id_PDC_Testata, oggetto.ID_LFO, oggetto.LFO_Des, objParametri)
            Return ""
        End If
    End Function
    
    Public Shared Function Cancella(ByVal Id_Testata As Integer, ByVal Id_LFO As Integer, ByRef objParametri As AgronicaCoreParametri) As String
        Dim xRisp As String = ""
        Dim flagConnessione, flagTransazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            Dim objLFOW As New AgronicaCorePianidiCampionamentoDAL.LFO_W
            objLFOW.Cancella(Id_Testata, Id_LFO, objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function
    
    Public Shared Function GetDataTableLFO(ByVal Lista_Oggetti As List(Of PDC_LFO)) As DataTable

        Dim dt As New DataTable
        GeneraStrutturaDT(dt)

        Dim dr As DataRow
        For i As Integer  = 0 To Lista_Oggetti.Count - 1
            dr = dt.NewRow
            dr.Item("Id_PDC_Testata") = Lista_Oggetti(i).Id_PDC_Testata
            dr.Item("ID_LFO") = Lista_Oggetti(i).ID_LFO
            dr.Item("LFO_Des") = Lista_Oggetti(i).LFO_Des.Replace("'", " ")

            dt.Rows.Add(dr)
        Next

        dt.DefaultView.Sort = "LFO_Des"
        
        Return dt

    End Function

    Private Shared Sub GeneraStrutturaDT(ByRef DT As DataTable)

        Dim Id_PDC_Testata As New DataColumn("Id_PDC_Testata") With {
            .DataType = Type.GetType("System.Int32")
        }
        DT.Columns.Add(Id_PDC_Testata)

        Dim ID_LFO As New DataColumn("ID_LFO") With {
            .DataType = Type.GetType("System.Int32")
        }
        DT.Columns.Add(ID_LFO)

        Dim LFO_Des As New DataColumn("LFO_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(LFO_Des)
    End Sub

End Class
