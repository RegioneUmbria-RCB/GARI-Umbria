Imports System.Web
Imports AgronicaCoreDataProvider

Public Class PDC_Campioni

#Region "private"
    Private _PDC_Campione_Des As String
#End Region

    Sub New()
        AnalisiAssociate = New List(Of PDC_Analisi)
    End Sub

    Public Property Tecnico_Campione As String
    Public Property ID_PuntoDiPrelievo As String
    Public Property Descrizione_PuntoDiPrelievo As String
    Public Property PuntoPrelievo As String
    Public Property ID_PDC_Stato_Campione_Des As String
    Public Property Id_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
    Public Property ID_PDC_Campione As Integer
    Public Property ID_PDC_Stato_Campione As Integer
    Public Property Data_Campionamento As Date
    Public Property Codice_Campione As String
    Public Property AnalisiAssociate As List(Of PDC_Analisi)
    Public Property Lat_Campione As String
    Public Property Long_Campione As String
    Public Property Note_Campione As String
    Public Property Tipo_Campione As Integer
    Public Property ID_Motivo_Campione As Integer
    Public Property Motivo_Campione As String
    Public Property Num_Prodotti As Integer

End Class

'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################

Public Class PDC_Campioni_Helper

    Public Shared Sub CaricaLista(ByVal Modalita_Nero As Boolean, ByVal Id_Testata As Integer, ByVal ID_PDC_Dettagli As Integer,
                                  ByVal ID_PDC_Campione As Integer,
                                  ByRef oggetto As List(Of PDC_Campioni), ByVal objParametri As AgronicaCoreParametri)

        oggetto = New List(Of PDC_Campioni)

        Dim filtroAggiuntivo As String = ""

        If Modalita_Nero = True Then
            filtroAggiuntivo = " PDC_Campioni.ID_PDC_Campione NOT IN ( " &
                         " Select PDC_Analisi.ID_PDC_Campione " &
                         " FROM         Analisi_Conformita_Capitolato_Cliente INNER JOIN " &
                         " PDC_Analisi ON Analisi_Conformita_Capitolato_Cliente.Analisi_Testata_Cod = PDC_Analisi.Analisi_Testata_Cod " &
                         " WHERE     (Analisi_Conformita_Capitolato_Cliente.CapitolatoCliente_Cod = 0) AND (Analisi_Conformita_Capitolato_Cliente.Esito <> - 1) " &
                         " AND ID_PDC_Testata=" & Id_Testata &
                         " AND ID_PDC_Dettagli=" & ID_PDC_Dettagli &
                         ")"

            filtroAggiuntivo &= " AND PDC_Campioni.ID_PDC_Campione NOT IN (  " &
                         " Select PDC_Analisi.ID_PDC_Campione " &
                         " FROM         PDC_Analisi " &
                         " WHERE     PDC_Analisi.mostra_in_stampe <>-1 " &
                         " AND ID_PDC_Testata=" & Id_Testata &
                         " AND ID_PDC_Dettagli=" & ID_PDC_Dettagli &
                         ")"

        End If

        Dim objPDC_Campioni As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_R
        Dim dt As DataTable = objPDC_Campioni.Leggi(Id_Testata, ID_PDC_Dettagli, ID_PDC_Campione, filtroAggiuntivo, "", objParametri)

        For Each dr As DataRow In dt.Rows

            Dim obj As New PDC_Campioni With {
                .Id_PDC_Testata = dr.Item("Id_PDC_Testata"),
                .ID_PDC_Dettagli = dr.Item("ID_PDC_Dettagli"),
                .ID_PDC_Campione = dr.Item("ID_PDC_Campione"),
                .Codice_Campione = dr.Item("Codice_Campione"),
                .Lat_Campione = If(IsDBNull(dr.Item("X")), "0", dr.Item("X")),
                .Long_Campione = If(IsDBNull(dr.Item("Y")), "0", dr.Item("Y")),
                .Note_Campione = If(IsDBNull(dr.Item("Note_Campione")), "", dr.Item("Note_Campione")),
                .Tecnico_Campione = If(IsDBNull(dr.Item("Tecnico_Campione")), "", dr.Item("Tecnico_Campione")),
                .Tipo_Campione = If(IsNumeric(dr.Item("Tipo_Campione")), dr.Item("Tipo_Campione"), 0),
                .Data_Campionamento = dr.Item("Data_Campionamento"),
                .ID_PDC_Stato_Campione = dr.Item("ID_PDC_Stato_Campione"),
                .Descrizione_PuntoDiPrelievo = dr.Item("Descrizione_PuntoDiPrelievo"),
                .ID_PuntoDiPrelievo = dr.Item("ID_PuntoDiPrelievo"),
                .ID_Motivo_Campione = dr.Item("ID_Motivo_Campione"),
                .Motivo_Campione = dr.Item("Descrizione_Motivo_Campione"),
                .Num_Prodotti = If(IsNumeric(dr.Item("Num_Prodotti")), dr.Item("Num_Prodotti"), 0)
            }

            'carico la lista delle analisi
            PDC_Analisi_Helper.Carica(obj.Id_PDC_Testata, obj.ID_PDC_Dettagli, obj.ID_PDC_Campione, obj.AnalisiAssociate, objParametri)

            'aggiungo l'oggetto alla lista
            oggetto.Add(obj)
        Next


    End Sub

    Public Shared Function Salva(ByVal oggetto As PDC_Testata, ByVal objParametri As AgronicaCoreParametri) As String
        Dim xRisp As String = ""
        Dim flagConnessione, flagTransazione As Boolean
        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'salvo il campione

            'salvo le analisi associate

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function Scrivi(ByVal oggetto As List(Of PDC_Campioni), ByVal ID_PDC_Testata As Integer, ByVal ID_PDC_Dettagli As Integer, ByVal objParametri As AgronicaCoreParametri, Optional ByVal baseCode As Integer = 0, Optional ByVal topCode As Integer = 0)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            ' uso valori in session se definiti
            If HttpContext.Current.Session IsNot Nothing Then
                baseCode = HttpContext.Current.Session("BaseCode")
                topCode = HttpContext.Current.Session("TopCode")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'sono in fase di aggiunta 
            Dim objPdcCamp As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_W
            Dim objPDCCampione As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_R

            For i As Integer = 0 To oggetto.Count - 1

                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                oggetto(i).ID_PDC_Campione = objSeq.NuovoId_Tabella("PDC_Campioni", baseCode, topCode, objParametri)
                oggetto(i).Id_PDC_Testata = ID_PDC_Testata
                oggetto(i).ID_PDC_Dettagli = ID_PDC_Dettagli

                Dim Codice_Anno As Integer = oggetto(i).Data_Campionamento.Year
                Dim Codice_progressivo As Integer = objPDCCampione.Progressivo_Inizio_Anno(Codice_Anno, objParametri)
                Dim Codice_Campione As String = oggetto(i).Codice_Campione
                If String.IsNullOrEmpty(Codice_Campione) Then
                    Codice_Campione = Codice_progressivo & "/" & Right(Codice_Anno.ToString, 2)
                End If

                objPdcCamp.Scrivi(ID_PDC_Testata,
                                   ID_PDC_Dettagli,
                                   oggetto(i).ID_PDC_Campione,
                                   oggetto(i).Descrizione_PuntoDiPrelievo,
                                   oggetto(i).ID_PDC_Stato_Campione,
                                   oggetto(i).Data_Campionamento,
                                   Codice_Campione,
                                   oggetto(i).PuntoPrelievo,
                                   oggetto(i).Note_Campione,
                                   oggetto(i).Tecnico_Campione,
                                   Codice_progressivo,
                                   Codice_Anno,
                                   oggetto(i).Tipo_Campione,
                                   objParametri)

            Next

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function Cancella(ByVal Id_Testata As Integer,
                                    ByVal Id_Dettagli As Integer,
                                    ByVal Id_Campione As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal CancellaAncheBlocchi As Boolean = False,
                                    Optional ByVal CancellaAncheMarketAccess As Boolean = False
                                    ) As String

        Dim xRisp As String = ""
        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


            'identifico le analisi da eliminare
            Dim listAnalisi As List(Of PDC_Analisi) = Nothing
            PDC_Analisi_Helper.Carica(Id_Testata, Id_Dettagli, Id_Campione, listAnalisi, objParametri)

            'per ogni analisi elimino anche i dati effettivi dell'analisi (Analisi_Testata, Analisi_Dettagli)
            Dim objAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            For i As Integer = 0 To listAnalisi.Count - 1

                'Elimino le analisi 
                xRisp = PDC_Analisi_Helper.Cancella(Id_Testata, Id_Dettagli, Id_Campione, listAnalisi(i).Analisi_Testata_Cod,
                                                    objParametri, CancellaAncheBlocchi, CancellaAncheMarketAccess, "Cancella Campione ed Analisi PDC ")
                If xRisp <> "" Then
                    Throw New Exception(xRisp)
                End If

                ''Elimino l'allegato
                Dim nomeFile As String
                Dim objAllegatiEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                Dim dt As DataTable
                Dim Allegati_Documenti_Cod As Integer
                dt = objAllegatiEntita.Leggi_Analisi(listAnalisi(i).Analisi_Testata_Cod,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri)


                If dt.Rows.Count > 0 Then
                    Allegati_Documenti_Cod = dt.Rows(0).Item("Allegati_Documenti_Cod")
                    nomeFile = dt.Rows(0).Item("Allegati_Documenti_NomeFile")

                    Dim pathCompleto As String
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    pathCompleto = objAgroWeb.GestioneAllegati_Repository & "Analisi\"

                    If IO.Directory.Exists(pathCompleto) = True Then
                        If IO.File.Exists(pathCompleto & nomeFile) Then
                            IO.File.Delete(pathCompleto & nomeFile)
                        End If
                    End If

                    'elimino i record
                    Dim objAllegatiW As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                    Dim objAllegatiEntitaW As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                    objAllegatiW.Cancella(Allegati_Documenti_Cod, "", objParametri)
                    objAllegatiEntitaW.Cancella_Analisi(listAnalisi(i).Analisi_Testata_Cod, "", objParametri)
                End If
            Next

            'Elimino il Campione
            Dim objPdc_CampioneW As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_W
            objPdc_CampioneW.Cancella(Id_Testata, Id_Dettagli, Id_Campione, objParametri)
            
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function CaricaCampioni(ByVal Id_Testata As Integer, ByVal ID_PDC_Dettagli As Integer, dtCampioni As DataTable, dtAnalisi As DataTable) As List(Of PDC_Campioni)

        Dim listaCampioni As New List(Of PDC_Campioni)

        Dim rows = dtCampioni.Select("Id_PDC_Testata = " & UtilityProvider.Agro_SQL_SaveNum(Id_Testata) & " AND ID_PDC_Dettagli = " & UtilityProvider.Agro_SQL_SaveNum(ID_PDC_Dettagli) & "").ToList()

        For Each dr As DataRow In rows

            Dim obj As New PDC_Campioni With {
                .Id_PDC_Testata = dr.Item("Id_PDC_Testata"),
                .ID_PDC_Dettagli = dr.Item("ID_PDC_Dettagli"),
                .ID_PDC_Campione = dr.Item("ID_PDC_Campione"),
                .Codice_Campione = dr.Item("Codice_Campione"),
                .Note_Campione = If(IsDBNull(dr.Item("Note_Campione")), "", dr.Item("Note_Campione")),
                .Tecnico_Campione = If(IsDBNull(dr.Item("Tecnico_Campione")), "", dr.Item("Tecnico_Campione")),
                .Tipo_Campione = If(IsNumeric(dr.Item("Tipo_Campione")), dr.Item("Tipo_Campione"), 0),
                .Data_Campionamento = dr.Item("Data_Campionamento"),
                .ID_PDC_Stato_Campione = dr.Item("ID_PDC_Stato_Campione"),
                .Descrizione_PuntoDiPrelievo = dr.Item("Descrizione_PuntoDiPrelievo"),
                .ID_PuntoDiPrelievo = dr.Item("ID_PuntoDiPrelievo"),
                .ID_Motivo_Campione = dr.Item("ID_Motivo_Campione"),
                .Motivo_Campione = dr.Item("Descrizione_Motivo_Campione"),
                .Num_Prodotti = If(IsNumeric(dr.Item("Num_Prodotti")), dr.Item("Num_Prodotti"), 0),
                .Lat_Campione = If(IsDBNull(dr.Item("X")), "0", dr.Item("X")),
                .Long_Campione = If(IsDBNull(dr.Item("Y")), "0", dr.Item("Y"))
            }

            If dtAnalisi.Rows.Count > 0 Then
                'carico la lista delle analisi
                obj.AnalisiAssociate = PDC_Analisi_Helper.CaricaAnalisi(obj.Id_PDC_Testata, obj.ID_PDC_Dettagli, obj.ID_PDC_Campione, dtAnalisi)
            End If

            'aggiungo l'oggetto alla lista
            listaCampioni.Add(obj)
        Next

        Return listaCampioni

    End Function
End Class
