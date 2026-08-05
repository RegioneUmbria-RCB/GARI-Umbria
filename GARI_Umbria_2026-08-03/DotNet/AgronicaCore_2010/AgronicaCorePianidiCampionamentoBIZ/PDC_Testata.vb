Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtentiDAL

Public Class PDC_Testata

#Region "private"
    Private _Validita_Fine As String
#End Region

    Sub New()
        PDC_Dettagli = New List(Of PDC_Dettagli)
        LFOAssociati = New List(Of PDC_LFO)
    End Sub

    Public Property CodiceStabilimento As String
    Public Property Da_Campagna As Integer
    Public Property PivaSuperUser As String
    Public Property Id_PDC_Testata As Integer
    Public Property PDC_Testata_Des As String
    Public Property PDC_Data_Istantanea As Date
    Public Property Validita_Inizio As Date
    Public Property PivaOwner As String
    Public Property Sa_CodOwner As Integer
    Public Property Fabbricato_CodOwner As Integer
    Public Property Da_Zoo As Integer

    Public Property Validita_Fine As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property

    Public Property PDC_Stato As Integer
    Public Property LFOAssociati As List(Of PDC_LFO)
    Public Property PDC_Dettagli As List(Of PDC_Dettagli)

End Class




'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################

Public Class PDC_Testata_Helper

    Public Shared Sub Carica(ByVal Modalita_Nero As Boolean, ByRef Id_Testata As Integer, ByVal CodiceStabilimento As String,
                             ByRef oggetto As PDC_Testata, ByVal objParametri As AgronicaCoreParametri)

        'Leggo tutto e creo l oggetto
        oggetto = New PDC_Testata

        Dim objTestata As New AgronicaCorePianidiCampionamentoDAL.PDC_R
        Dim dt As DataTable = objTestata.Leggi(Id_Testata, "", 0, -1, CodiceStabilimento,
                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "", "", objParametri)

        If dt.Rows.Count > 0 Then
            oggetto.Id_PDC_Testata = dt.Rows(0).Item("ID_PDC_Testata")
            If Not IsDBNull(dt.Rows(0).Item("PDC_Data_Istantanea")) Then
                oggetto.PDC_Data_Istantanea = dt.Rows(0).Item("PDC_Data_Istantanea")
            Else
                oggetto.PDC_Data_Istantanea = Date.Today
            End If
            oggetto.PDC_Testata_Des = dt.Rows(0).Item("PDC_Testata_Des")
            If IsDate(dt.Rows(0).Item("Validita_Fine")) Then
                oggetto.Validita_Fine = dt.Rows(0).Item("Validita_Fine")
            End If
            If IsDate(dt.Rows(0).Item("Validita_Inizio")) Then
                oggetto.Validita_Inizio = dt.Rows(0).Item("Validita_Inizio")
            End If
            oggetto.CodiceStabilimento = dt.Rows(0).Item("CodiceStabilimento")
            oggetto.Da_Campagna = If(IsDBNull(dt.Rows(0).Item("Da_Campagna")), 0, dt.Rows(0).Item("Da_Campagna"))

            If dt.Rows(0).Item("Da_Zoo") = 1 Then
                oggetto.PivaOwner = dt.Rows(0).Item("PivaOwner")
                oggetto.Sa_CodOwner = dt.Rows(0).Item("Sa_CodOwner")
                oggetto.Fabbricato_CodOwner = dt.Rows(0).Item("Fabbricato_CodOwner")
            End If
        End If

        If dt.Rows(0).Item("Da_Zoo") = 1 Then
            PDC_Dettagli_Helper.CaricaListaMassivo(Modalita_Nero,
                                                   oggetto.Id_PDC_Testata, oggetto.PDC_Data_Istantanea,
                                                   oggetto.PDC_Dettagli, objParametri)
        Else
            'carico gli LFO Associati
            PDC_LFO_Helper.CaricaLista(oggetto.Id_PDC_Testata, oggetto.LFOAssociati, objParametri)

            'carico i dettagli
            PDC_Dettagli_Helper.CaricaLista(Modalita_Nero,
                                        oggetto.Id_PDC_Testata, oggetto.PDC_Data_Istantanea,
                                        oggetto.PDC_Dettagli, objParametri)
        End If

    End Sub

    Public Shared Function Salva(ByVal oggetto As PDC_Testata, ByRef objParametri As AgronicaCoreParametri, Optional ByVal baseCode As Integer = 0, Optional ByVal topCode As Integer = 0) As String

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            ' uso valori in session se definiti
            If HttpContext.Current.Session IsNot Nothing Then
                baseCode = HttpContext.Current.Session("BaseCode")
                topCode = HttpContext.Current.Session("TopCode")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'controllo se sono in modifica o scrittura
            Dim objPdcTestata As New AgronicaCorePianidiCampionamentoDAL.PDC_R
            Dim dt As DataTable = objPdcTestata.Leggi(oggetto.Id_PDC_Testata,
                                                      "", 0, oggetto.Da_Campagna, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "", "", objParametri)

            Dim objPdcW As New AgronicaCorePianidiCampionamentoDAL.PDC_W
            If dt.Rows.Count = 0 Or oggetto.Id_PDC_Testata = 0 Then

                'sono in scrittura di un nuovo elemento 
                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                oggetto.Id_PDC_Testata = objSeq.NuovoId_Tabella("PDC_Testata", baseCode, topCode, objParametri)

                objPdcW.Scrivi(oggetto.Id_PDC_Testata,
                               oggetto.PDC_Testata_Des,
                               oggetto.PDC_Data_Istantanea,
                               oggetto.PDC_Stato,
                               oggetto.Da_Campagna,
                               oggetto.CodiceStabilimento,
                               oggetto.Validita_Inizio,
                               oggetto.Validita_Fine,
                               objParametri,
                               PivaOwner:=oggetto.PivaOwner,
                               Sa_CodOwner:=oggetto.Sa_CodOwner,
                               Fabbricato_CodOwner:=oggetto.Fabbricato_CodOwner,
                               Da_Zoo:=oggetto.Da_Zoo)

                PDC_Dettagli_Helper.Scrivi(oggetto.PDC_Dettagli, oggetto.Id_PDC_Testata, objParametri, baseCode, topCode, oggetto.Da_Zoo)

            Else
                'sono in modifica
                objPdcW.Modifica(oggetto.Id_PDC_Testata,
                                 oggetto.PDC_Testata_Des,
                                 oggetto.PDC_Data_Istantanea,
                                 oggetto.PDC_Stato,
                                 oggetto.CodiceStabilimento,
                                 oggetto.Validita_Inizio,
                                 oggetto.Validita_Fine,
                                 objParametri,
                                 PivaOwner:=oggetto.PivaOwner,
                                 Sa_CodOwner:=oggetto.Sa_CodOwner,
                                 Fabbricato_CodOwner:=oggetto.Fabbricato_CodOwner,
                                 Da_Zoo:=oggetto.Da_Zoo)

                PDC_Dettagli_Helper.Modifica(oggetto.PDC_Dettagli, oggetto.Id_PDC_Testata, objParametri, baseCode, topCode, oggetto.Da_Zoo)
            End If

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function Cancella(ByVal ID_Testata As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal CancellaAncheBlocchi As Boolean = False,
                                    Optional ByVal CancellaAncheMarketAccess As Boolean = False
                                    ) As String

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


            'Cancello TUTTI i Dettagli
            xRisp = PDC_Dettagli_Helper.Cancella(ID_Testata, 0, objParametri,
                                                 CancellaAncheBlocchi, CancellaAncheMarketAccess)
            If xRisp <> "" Then
                Throw New Exception(xRisp)
            End If


            'Cancella TUTTI gli LFO
            xRisp = PDC_LFO_Helper.Cancella(ID_Testata, 0, objParametri)
            If xRisp <> "" Then
                Throw New Exception(xRisp)
            End If

            'procedo con la cancellazione Fisica della Testata
            Dim objPDCW As New AgronicaCorePianidiCampionamentoDAL.PDC_W
            objPDCW.Cancella(ID_Testata, objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function GetInfo(ByRef oggetto As PDC_Testata,
                                   Optional Zoo As Boolean = False) As String

        Dim str As String

        If Zoo Then
            Dim n_pdc As Integer = 0

            For i As Integer = 0 To oggetto.PDC_Dettagli.Count - 1
                If oggetto.PDC_Dettagli(i).Flag_PDC <> 0 Then
                    n_pdc += 1
                End If
            Next

            str = String.Format(
                Gias.PDC_Testata_Helper_GetInfoZooAnimali,
                oggetto.PDC_Dettagli.Count, n_pdc)

        Else

            Dim sup_pp As Decimal = 0
            Dim sup_pdc As Decimal = 0
            Dim n_pdc As Integer = 0
            Dim n_LFO As Integer = 0

            Dim list_LFO As New Hashtable

            For i As Integer = 0 To oggetto.PDC_Dettagli.Count - 1
                If IsNumeric(oggetto.PDC_Dettagli(i).Impianto.Sup_Imp) Then
                    sup_pp += oggetto.PDC_Dettagli(i).Impianto.Sup_Imp
                    If Not IsNothing(oggetto.PDC_Dettagli(i).LFO) Then
                        If list_LFO.ContainsKey(oggetto.PDC_Dettagli(i).LFO.ID_LFO) = False Then
                            list_LFO.Add(oggetto.PDC_Dettagli(i).LFO.ID_LFO, oggetto.PDC_Dettagli(i).LFO.ID_LFO)
                            n_LFO += 1
                        End If
                    End If

                    If oggetto.PDC_Dettagli(i).Flag_PDC <> 0 Then
                        n_pdc += 1
                        sup_pdc += oggetto.PDC_Dettagli(i).Impianto.Sup_Imp
                    End If
                End If
            Next

            'str = "<b>N. Impianti Piano Produttivo:</b> " & oggetto.PDC_Dettagli.Count &
            '      " <b>con Sup. Tot. Ha:</b> " & sup_pp & "<br>" &
            '      "<b>N. Impianti PDC:</b> " & n_pdc &
            '      " <b>con Sup. Tot. Ha:</b> " & sup_pdc &
            '      " <br><b>Totale N. LFO:</b> " & n_LFO
            str = String.Format(
                Gias.PDC_Testata_Helper_GetInfo,
                oggetto.PDC_Dettagli.Count, sup_pp, n_pdc, sup_pdc, n_LFO
            )

        End If

        Return str

    End Function

    Public Shared Function CancellaLFO(ByRef oggetto As PDC_Testata, ByRef ID_LFO As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'cancello l'LFO
            PDC_LFO_Helper.Cancella(oggetto.Id_PDC_Testata, ID_LFO, objParametri)

            'aggiorno la lista degli lfo
            Dim i As Integer
            For i = oggetto.LFOAssociati.Count - 1 To 0 Step -1
                If oggetto.LFOAssociati(i).ID_LFO = ID_LFO Then
                    oggetto.LFOAssociati.RemoveAt(i)
                End If
            Next

            'procedo con la cancellazione di quell'id dai dettagli
            Dim objDettagliW As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
            objDettagliW.Modifica_Flag_OldLFO_NewLFO(oggetto.Id_PDC_Testata, ID_LFO, 0, objParametri)

            'aggiorno l'oggetto dettagli
            For i = 0 To oggetto.PDC_Dettagli.Count - 1
                If oggetto.PDC_Dettagli(i).LFO.ID_LFO = ID_LFO Then
                    oggetto.PDC_Dettagli(i).LFO = Nothing
                End If
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

    Public Shared Sub CaricaListaProdotti(ByVal Oggetto As PDC_Testata,
                                          ByRef ListaTotale As String,
                                          ByRef DataInizio As Date,
                                          ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable
        Dim i As Integer

        Dim objProdotti As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        'lista

        dt = objProdotti.LeggiProdottiUtilizzatiFromID_PDC_Testata(True, Oggetto.Id_PDC_Testata,
                                                                   Oggetto.PDC_Data_Istantanea,
                                                                   AGRODATAINIZIO,
                                                                   "", "", objParametri)


        ListaTotale = ""
        For i = 0 To dt.Rows.Count - 1
            ListaTotale += dt.Rows(i).Item("Pro_Cod") & " ,"
        Next
        If ListaTotale.Length > 0 Then
            ListaTotale = ListaTotale.Substring(0, ListaTotale.Length - 2)
        End If


        dt = objProdotti.LeggiProdottiUtilizzatiFromID_PDC_Testata(False, Oggetto.Id_PDC_Testata,
                                                                   Oggetto.PDC_Data_Istantanea,
                                                                   AGRODATAINIZIO,
                                                                   "", "", objParametri)


        Dim righe() As DataRow
        Dim piva, sacod, appezza, idreg As String

        For i = 0 To Oggetto.PDC_Dettagli.Count - 1
            piva = Oggetto.PDC_Dettagli(i).Impianto.Piva
            sacod = Oggetto.PDC_Dettagli(i).Impianto.Sa_Cod
            appezza = Oggetto.PDC_Dettagli(i).Impianto.Appezza
            idreg = Oggetto.PDC_Dettagli(i).Impianto.Id_Reg
            righe = dt.Select(" Piva= '" & piva & "' and Sa_Cod = " & sacod & " AND Appezza = " & appezza & " AND ID_Reg =" & idreg)
            Oggetto.PDC_Dettagli(i).ProdottiUtilizzati = New List(Of ProdottiQuantita)

            For j As Integer = 0 To righe.Length - 1
                Dim obj As New ProdottiQuantita With {
                    .Data = righe(j).Item("Data_Movimento"),
                    .FR_Cod = righe(j).Item("Pro_Cod"),
                    .FR_Des = righe(j).Item("FR_Des"),
                    .Qta = righe(j).Item("Qta")
                }
                Oggetto.PDC_Dettagli(i).ProdottiUtilizzati.Add(obj)
            Next
        Next


    End Sub

    Public Shared Sub AggiungiInformazioniProdotti(ByVal ListaPA_NO As List(Of Integer),
                                                   ByVal Data As Date,
                                                   ByVal Oggetto As PDC_Testata,
                                                   ByVal HashProdotti As Hashtable)

        For i As Integer = 0 To Oggetto.PDC_Dettagli.Count - 1
            For j As Integer = Oggetto.PDC_Dettagli(i).ProdottiUtilizzati.Count - 1 To 0 Step -1
                If Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).Data >= Data Then
                    Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).ListaPA =
                            CType(HashProdotti(Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).FR_Cod), Formulato).ListaPA
                    'elimino i formulati che non devo considerare
                    Dim jj, kk As Integer
                    For jj = Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).ListaPA.Count - 1 To 0 Step -1
                        For kk = 0 To ListaPA_NO.Count - 1
                            If Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).ListaPA(jj).Pa_Cod = ListaPA_NO(kk) Then
                                Oggetto.PDC_Dettagli(i).ProdottiUtilizzati(j).ListaPA.RemoveAt(jj)
                                Exit For
                            End If
                        Next
                    Next
                Else
                    Oggetto.PDC_Dettagli(i).ProdottiUtilizzati.RemoveAt(j)
                End If

            Next
        Next

    End Sub

    Public Shared Function AggiungiImpianti(ByRef ID_PianoCampionamento As Integer, ByVal Oggetto As PDC_Testata, ByRef objParametri As AgronicaCoreParametri, ByVal Sincronizza As Boolean, ByVal LFO_Unico As Boolean) As String

        Dim risp = AggiungiListaImpianti(Oggetto, objParametri, Sincronizza, LFO_Unico) = ""

        'genero un LFO unico e lo associo a tutti gli impianti
        If LFO_Unico Then

            Dim objLFOR As New AgronicaCorePianidiCampionamentoDAL.LFO_R
            Dim DT_LFO As DataTable = objLFOR.Leggi(ID_PianoCampionamento, 0, "LFO_UNICO", objParametri)

            Dim new_id_lfo As Integer
            If DT_LFO.Rows.Count = 0 Then
                Dim Agroseq As New AgronicaCoreDataProvider.Agro_Sequenze
                new_id_lfo = Agroseq.NuovoId_Tabella("PDC_LFO", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
                Dim objLFOW As New AgronicaCorePianidiCampionamentoDAL.LFO_W
                objLFOW.Scrivi(ID_PianoCampionamento, new_id_lfo, "LFO_UNICO", objParametri)
            Else
                new_id_lfo = DT_LFO.Rows(0).Item("ID_LFO")
            End If

            'lo associo a chi non ha l'LFO impostato
            Dim objDettagli_W As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
            objDettagli_W.LFO_Unico(ID_PianoCampionamento, new_id_lfo, objParametri)

        End If

        Return risp

    End Function

    Public Shared Function AggiungiListaImpianti(ByVal Oggetto As PDC_Testata, ByRef objParametri As AgronicaCoreParametri, Optional ByVal Sincronizza As Boolean = False, Optional ByVal LFO_Unico As Boolean = True) As String

        Dim i As Integer
        Dim xRisp As String = ""

        Dim objParametriPianidiCampionamento_2010 As New AgronicaCoreGestioneRichieste.ParametriPianidiCampionamento_2010
        objParametriPianidiCampionamento_2010.Leggi()

        Dim ListaImpinati As New List(Of AgronicaCoreGestioneRichieste.Impianto)
        Dim ListaDettagli As New List(Of Integer)

        For i = objParametriPianidiCampionamento_2010.ListaImpianti.Length - 1 To 0 Step -1
            Dim trovato As Boolean = False
            For j = 0 To Oggetto.PDC_Dettagli.Count - 1
                If ConfrontaImpianti(objParametriPianidiCampionamento_2010.ListaImpianti(i), Oggetto.PDC_Dettagli(j).Impianto) = True Then
                    trovato = True
                    Exit For
                End If
            Next
            If trovato = False Then
                ListaImpinati.Add(objParametriPianidiCampionamento_2010.ListaImpianti(i))
            End If
        Next

        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


            For i = 0 To ListaImpinati.Count - 1

                'genero un nuovo id 
                Dim Dettaglio As New PDC_Dettagli With {
                    .Id_PDC_Testata = Oggetto.Id_PDC_Testata,
                    .ID_PDC_Dettagli = 0,
                    .PDC_Dettagli_Des = ""
                }

                Dettaglio.Impianto = New Impianto With {
                    .App_Nome = ListaImpinati(i).App_Nome,
                    .Appezza = ListaImpinati(i).Appezza,
                    .Cul_Cod = ListaImpinati(i).Cul_Cod,
                    .Cul_Des = ListaImpinati(i).Cul_Des,
                    .Data_Raccolta = ListaImpinati(i).Data_Raccolta,
                    .Id_Reg = ListaImpinati(i).Id_Reg,
                    .Piva = ListaImpinati(i).Piva,
                    .Rag_Soc = ListaImpinati(i).Rag_Soc,
                    .Sa_Nome = ListaImpinati(i).Sa_Nome,
                    .Sa_Cod = ListaImpinati(i).Sa_Cod,
                    .Sup_Imp = ListaImpinati(i).Sup_Imp,
                    .Veg_Cod = ListaImpinati(i).Veg_Cod,
                    .Veg_Des = ListaImpinati(i).Veg_Des
                }

                ' genero un LFO per ogni impianto
                If Not LFO_Unico Then
                    Dim Agroseq As New AgronicaCoreDataProvider.Agro_Sequenze
                    Dim objLFOW As New AgronicaCorePianidiCampionamentoDAL.LFO_W
                    Dim ID_LFO = Agroseq.NuovoId_Tabella("PDC_LFO", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
                    objLFOW.Scrivi(Oggetto.Id_PDC_Testata, ID_LFO, "LFO_" & ID_LFO, objParametri)
                    Dettaglio.LFO.ID_LFO = ID_LFO
                End If

                PDC_Dettagli_Helper.ScriviSingolo(Dettaglio, Oggetto.Id_PDC_Testata, objParametri)

                Oggetto.PDC_Dettagli.Add(Dettaglio)

                ListaDettagli.Add(Dettaglio.ID_PDC_Dettagli)

            Next

            ' sincronizzazione automatica su aggiunta impianti
            If Sincronizza AndAlso ListaDettagli.Count > 0 Then
                Dim objw As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
                objw.Update_da_Anagrafica(Oggetto.Id_PDC_Testata, objParametri, String.Join(",", ListaDettagli.ToArray))
            End If

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Private Shared Function ConfrontaImpianti(ByVal impianto1 As AgronicaCoreGestioneRichieste.Impianto, ByVal impianto2 As Impianto) As Boolean
        If impianto1.Piva <> impianto2.Piva Then
            Return False
        End If

        If impianto1.Sa_Cod <> impianto2.Sa_Cod Then
            Return False
        End If

        If impianto1.Appezza <> impianto2.Appezza Then
            Return False
        End If

        If impianto1.Id_Reg <> impianto2.Id_Reg Then
            Return False
        End If

        Return True

    End Function

    Public Shared Function Leggi_PDC_APP(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Fabbricato_Cod As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim objPDCR As New AgronicaCorePianidiCampionamentoDAL.PDC_R

        Return objPDCR.Leggi_PDC_ZooApp(Piva, Sa_Cod, Fabbricato_Cod, "", "", objParametri_Server)

    End Function

    Public Shared Function AggiungiListaCapiAnimale(Oggetto As PDC_Testata,
                                                    ByVal listaDettagli As List(Of PDC_Dettagli),
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim i As Integer
        Dim xRisp As String = ""

        Dim objParametriPianidiCampionamento_2010 As New AgronicaCoreGestioneRichieste.ParametriPianidiCampionamento_2010
        objParametriPianidiCampionamento_2010.Leggi()

        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


            For Each dettaglio In listaDettagli

                PDC_Dettagli_Helper.ScriviSingoloCapoAnimale(dettaglio, Oggetto.Id_PDC_Testata, objParametri)

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

    Public Shared Sub Aggiungi_Impianti_FiltroRicercaNG(Id_PDC_Testata As Integer, LFOUnico As Boolean, chiavi As List(Of String), objParametri_Server As AgronicaCoreParametri, Optional rileggiDettagli As Boolean = False)
        Dim objParametriPianiCampionamento As New ParametriPianidiCampionamento_2010

        Dim filtroProgetti As New List(Of Integer)
        For Each chiave In chiavi
            filtroProgetti.Add(chiave.Split("_")(5))
        Next
        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dtimpianti = objRegImpianti.Leggi_x_ParametriAgenda(filtroProgetti, objParametri_Server)

        For Each chiave In chiavi
            Dim objImpianto As New AgronicaCoreGestioneRichieste.Impianto

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As String = chiave.Split("_")(1)
            Dim appezza As String = chiave.Split("_")(2)
            Dim id_reg As String = chiave.Split("_")(3)
            Dim progetto_cod As String = chiave.Split("_")(5)

            objImpianto.Piva = piva
            objImpianto.Sa_Cod = sa_cod
            objImpianto.Appezza = appezza
            objImpianto.Id_Reg = id_reg

            Dim dr() As DataRow = dtimpianti.Select("progetto_cod = " & progetto_cod)

            If dr.Length > 0 Then
                objImpianto.Rag_Soc = CStr(dr(0).Item("rag_soc"))
                objImpianto.Sa_Nome = CStr(dr(0).Item("sa_nome"))
                objImpianto.App_Nome = CStr(dr(0).Item("app_nome"))

                If IsDBNull(dr(0).Item("veg_cod")) Then
                    objImpianto.Veg_Cod = 0
                Else
                    objImpianto.Veg_Cod = CStr(dr(0).Item("veg_cod"))
                End If
                If IsDBNull(dr(0).Item("veg_des")) Then
                    objImpianto.Veg_Des = ""
                Else
                    objImpianto.Veg_Des = CStr(dr(0).Item("veg_des"))
                End If
                If IsDBNull(dr(0).Item("cul_cod")) Then
                    objImpianto.Cul_Cod = 0
                Else
                    objImpianto.Cul_Cod = CStr(dr(0).Item("cul_cod"))
                End If
                If IsDBNull(dr(0).Item("cul_des")) Then
                    objImpianto.Cul_Des = ""
                Else
                    objImpianto.Cul_Des = CStr(dr(0).Item("cul_des"))
                End If

                objImpianto.Sup_Imp = CStr(dr(0).Item("sup_imp"))
            End If

            objParametriPianiCampionamento.AddList(objImpianto)
        Next

        objParametriPianiCampionamento.Id_PDC_Testata = Id_PDC_Testata

        AggiungiImpianti(Id_PDC_Testata, True, LFOUnico, "", objParametri_Server, rileggiDettagli)

    End Sub

    Public Shared Sub AggiungiImpianti(Id_PDC_Testata As Integer,
                                        Sincronizza As Boolean,
                                        LFO_Unico As Boolean,
                                        FiltroStabilimento As String,
                                        objParametri_Server As AgronicaCoreParametri,
                                            Optional Nero As Boolean = False,
                                            Optional rileggiDettagli As Boolean = False)

        Try

            Dim objPDCTestata As New PDC_Testata

            Carica(Nero, Id_PDC_Testata, FiltroStabilimento, objPDCTestata, objParametri_Server)
            AggiungiImpianti(Id_PDC_Testata, objPDCTestata, objParametri_Server, Sincronizza, LFO_Unico)

            If rileggiDettagli Then 'Ricarico i dettagli degli impianti che non sono salvati sull'objPianiCampionamento
                Carica(Nero, Id_PDC_Testata, FiltroStabilimento, objPDCTestata, objParametri_Server)
            End If

            HttpContext.Current.Session("_PDC_Testata") = objPDCTestata

        Catch ex As Exception
            Throw New Exception("Errore nell'aggiunta impianti")
        End Try

    End Sub

    ' inizializza piani campionamento per nuova gestione
    Public Shared Sub InitPDC(ByRef objParametri_Server As AgronicaCoreParametri)

        ' inizializza progressivi anno per i codici analisi fruttagel
        Dim objWebConfig As New AgroWebConfig
        If objWebConfig.Codici_Analisi_PDC_x_Fruttagel = True Then
            PDC_Analisi_Helper.InitCodiciAnalisiFruttagel(objParametri_Server)
        End If

        ' importa gli utenti dalla vecchia alla nuova gestione laboratori
        PDC_Analisi_Helper.InitUtentiLaboratori(objParametri_Server)

    End Sub

End Class
