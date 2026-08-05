Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PDC_Sblocca
    
    Public Property PivaSuperUser As String
    Public Property ID_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Appezza As Integer
    Public Property ID_Reg As Integer
    Public Property CapitolatoCliente_Cod As Integer
    Public Property Esito As Integer
    Public Property EsitoBool As Boolean
    Public Property Note As String
    Public Property Data_Sblocco As DateTime
    Public Property Analisi_Tipologia As Integer
    Public Property Analisi_Associata As Integer?

End Class

'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################

Public Class PDC_Sblocca_Helper

    Public Shared Sub Carica(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Analisi_Tipologia As Integer,
                             ByVal Analisi_Associata As Integer,
                             ByRef oggetto As List(Of PDC_Sblocca),
                             ByRef objParametri As AgronicaCoreParametri)

        oggetto = New List(Of PDC_Sblocca)
        
        Dim objPDC_Sblocca As New AgronicaCorePianidiCampionamentoDAL.PDC_Sblocca_R
        Dim dt As DataTable = objPDC_Sblocca.LeggiJoinDettagli(ID_PDC_Testata, ID_PDC_Dettagli, Piva, Sa_Cod, Appezza, Id_reg,
                                                               0, Analisi_Tipologia, Analisi_Associata,
                                                               "", "", objParametri)

        For Each dr In dt.Rows

            Dim obj As New PDC_Sblocca With {
                .ID_PDC_Testata = dr.Item("ID_PDC_Testata"),
                .ID_PDC_Dettagli = dr.Item("ID_PDC_Dettagli"),
                .Piva = dr.Item("Piva"),
                .Sa_Cod = dr.Item("Sa_Cod"),
                .Appezza = dr.Item("Appezza"),
                .ID_Reg = dr.Item("Id_Reg"),
                .CapitolatoCliente_Cod = dr.Item("CapitolatoCliente_Cod"),
                .Esito = dr.Item("Esito"),
                .EsitoBool = If(dr.Item("Esito") = -1, True, False),
                .Note = dr.Item("note"),
                .Data_Sblocco = dr.Item("data_sblocco"),
                .Analisi_Tipologia = dr.Item("Analisi_Tipologia"),
                .Analisi_Associata = If(IsDbNull(dr.Item("Analisi_Associata")), Nothing, dr.Item("Analisi_Associata"))
            }

            'aggiungo l'oggetto alla lista
            oggetto.Add(obj)
        Next

    End Sub

    Public Shared Function Cancella(ByVal ID_PDC_Testata As Integer,
                                    ByVal ID_PDC_Dettagli As Integer,
                                    ByVal Analisi_Testata_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal CancellaAncheMarketAccess As Boolean = False
                                    ) As String

        Dim xRisp As String = ""
        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)
            
            'TODO: eventualmente fare versione che arriva a cancellare più nel dettaglio per i specifici capitolati

            If CancellaAncheMarketAccess = True Then

                'devo prima leggerli, perché sennò non saprei quali record di market access eventualmente cancellare
                Dim listPDCSblocca As New List(Of PDC_Sblocca)
                PDC_Sblocca_Helper.Carica(ID_PDC_Testata, ID_PDC_Dettagli,
                                          "", 0, 0, 0,
                                          0, Analisi_Testata_Cod,
                                          listPDCSblocca, objParametri)

                Dim listDettagli = (From s In listPDCSblocca Select New With { s.ID_PDC_Testata, s.ID_PDC_Dettagli } Distinct)

                For Each dettaglio In listDettagli

                    'devo cancellare anche il market access derivanti dai blocchi dovuti a questa analisi (solo se non bloccato)
                    Dim objPdc_Market As New AgronicaCorePianidiCampionamentoDAL.PDC_MarketAccess_W
                    objPdc_Market.Cancella(dettaglio.ID_PDC_Testata, dettaglio.ID_PDC_Dettagli,
                                           "", 0, 0, 0, objParametri, EscludiBloccati:=True)

                Next

            End If

            Dim objPdc_Sblocca As New AgronicaCorePianidiCampionamentoDAL.PDC_Sblocca_W
            objPdc_Sblocca.CancellaGenericoJoin(ID_PDC_Testata, ID_PDC_Dettagli, Analisi_Testata_Cod, -99, objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

End Class
