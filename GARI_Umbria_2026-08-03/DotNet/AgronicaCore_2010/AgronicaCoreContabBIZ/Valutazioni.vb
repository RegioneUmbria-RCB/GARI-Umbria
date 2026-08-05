Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.valutazioni
Imports AgronicaCoreUtility
Imports ClosedXML.Excel
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports AgronicaCoreDTOStd.InData.Valutazioni
Imports Excel = Microsoft.Office.Interop.Excel


Public Class ObjAnnoDaInserire

    Public Property Colonna As Integer
    Public Property Totale_Sezione As Decimal
    Public Property Totale_Gruppo As Decimal
    Public Property Totale_Gruppo_Padre As Decimal
    Public Property Totale As Decimal
    Public Property Totale_Ricavi As Decimal
    Public Property MOL As Decimal
    Public Property MON As Decimal

End Class


Public Class ObjTotaleDaInserire
    Public Property Totale As Decimal
End Class


Public Class Valutazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi_Valutazione(ByVal Piva As String,
                                      ByVal Id_Testata As Integer,
                                      ByVal Valutazione_Conto_Cod As Integer,
                                      ByVal Dettaglio_Key As String,
                                      ByVal Anno As Integer,
                                      ByVal bIncludi_Dettaglio_Specifico As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal xFiltroAggiuntivo As String = ""
                                      ) As List(Of Valutazione)


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazione_R.Leggi_Valutazione()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Dettaglio As New Valutazione_Dettaglio_R
        Dim ObjLeggi_Dettaglio_Specifico As New Valutazione_Dettaglio_Specifico_R
        Dim ObjLeggi_SpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim ObjLeggi_Cultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim ObjLeggi_Categorie As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim ObjLeggi_Regolamenti As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim objValutazioni As New List(Of Valutazione)

        Dim DT_Sp As DataTable = Nothing
        Dim DT_SpecieVegetali As DataTable = Nothing
        Dim DT_Cultivar As DataTable = Nothing
        Dim DT_Categorie As DataTable = Nothing
        Dim DT_Regolamenti As DataTable = Nothing


        Dim drSearch As DataRow()
        Dim drSearchKey As DataRow()

        Dim Arrayp As String()
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Reg_Cod As Integer
        Dim Elem_Cod As Integer
        Dim Descrizione As String

        Try

            'Lettura Dettagli
            Dim dt As DataTable = ObjLeggi_Dettaglio.Leggi(Piva, Id_Testata, Valutazione_Conto_Cod, Anno, Dettaglio_Key, objParametri, xFiltroAggiuntivo)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                'Lettura Preventiva x Filtri
                If bIncludi_Dettaglio_Specifico Then

                    'Lettura di tutti i dettagli specifici
                    DT_Sp = ObjLeggi_Dettaglio_Specifico.Leggi(Piva, Id_Testata, Valutazione_Conto_Cod, Anno, Dettaglio_Key, objParametri)

                    DT_SpecieVegetali = ObjLeggi_SpecieVegetali.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                    DT_Cultivar = ObjLeggi_Cultivar.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                    DT_Categorie = ObjLeggi_Categorie.Leggi(0, "", False, "", "", objParametri)
                    DT_Regolamenti = ObjLeggi_Regolamenti.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                End If



                For Each dr As DataRow In dt.Rows

                    Dim objValutazione As New Valutazione

                    objValutazione.Piva = CStr(dr.Item("Piva"))
                    objValutazione.Id_Testata = CInt(dr.Item("Id_Testata"))
                    objValutazione.Valutazione_Conto_Cod = CInt(dr.Item("Valutazione_Conto_Cod"))
                    objValutazione.Valutazione_Conto_Des = CStr(dr.Item("Valutazione_Conto_Des"))
                    objValutazione.Anno = CInt(dr.Item("Anno"))
                    objValutazione.Anno_Tipo = CInt(dr.Item("Anno_Tipo"))
                    objValutazione.Anno_Tipo_Des = CStr(dr.Item("Anno_Tipo_Des"))
                    objValutazione.Sezione_Ordine = CInt(dr.Item("Sezione_Ordine"))

                    If CInt(dr.Item("Ordine")) = 0 Then
                        'Considero il default di metaschema
                        objValutazione.Ordine = CInt(dr.Item("Ordine_Default"))
                    Else
                        objValutazione.Ordine = CInt(dr.Item("Ordine"))
                    End If

                    objValutazione.Valore = CDec(dr.Item("Valore"))
                    objValutazione.Valutazione_Sezione_Cod = CInt(dr.Item("Valutazione_Sezione_Cod"))
                    objValutazione.Valutazione_Sezione_Des = CStr(dr.Item("Valutazione_Sezione_Des"))
                    objValutazione.Valutazione_Sezione_Invisibile = CInt(dr.Item("Sezione_Invisibile"))
                    objValutazione.Valutazione_Gruppo_Cod = CInt(dr.Item("Valutazione_Gruppo_Cod"))
                    objValutazione.Valutazione_Gruppo_Des = CStr(dr.Item("Valutazione_Gruppo_Des"))
                    objValutazione.Valutazione_Gruppo_Padre = CInt(dr.Item("Valutazione_Gruppo_Padre"))
                    objValutazione.Valutazione_Gruppo_Padre_Des = CStr(dr.Item("Valutazione_Gruppo_Padre_Des"))
                    objValutazione.Pat_Eco = CInt(dr.Item("Pat_Eco"))
                    objValutazione.Attivo_Passivo = CInt(dr.Item("Attivo_Passivo"))
                    objValutazione.Valutazione_Conto_Padre = CInt(dr.Item("Valutazione_Conto_Padre"))
                    objValutazione.ChkBypassValore = CInt(dr.Item("ChkBypassValore"))

                    If bIncludi_Dettaglio_Specifico Then

                        If DT_Sp IsNot Nothing AndAlso DT_Sp.Rows.Count > 0 Then

                            drSearch = DT_Sp.Select("Piva = '" & CStr(dr.Item("Piva")) & "' And " &
                                                     "Id_Testata = " & CStr(dr.Item("Id_Testata")) & " And " &
                                                     "Valutazione_Conto_Cod = " & CStr(dr.Item("Valutazione_Conto_Cod")) & " And " &
                                                     "Anno = " & CStr(dr.Item("Anno")))


                            If drSearch.Length <> 0 Then

                                Dim ObjValutazione_Dettagli_Specifici As New List(Of Valutazione_Dettaglio_Specifico)

                                For Each dr_sp In drSearch

                                    Dim ObjValutazione_Dettaglio_Specifico As New Valutazione_Dettaglio_Specifico With {
                                        .Dettaglio_Key = CStr(dr_sp("Dettaglio_Key")),
                                        .Anno = CInt(dr_sp("Anno")),
                                        .Id_Testata = CInt(dr_sp("Id_Testata")),
                                        .Valore_Unitario = CDec(dr_sp("Valore_Unitario")),
                                        .Valore_Totale = CDec(dr_sp("Valore_Totale")),
                                        .Valore_Ha = CDec(dr_sp("Valore_Ha")),
                                        .Valore_Peso = CDec(dr_sp("Valore_Peso"))
                                    }

                                    'Definizione Descrizione
                                    Arrayp = Split(CStr(dr_sp("Dettaglio_Key")), "|")
                                    Veg_Cod = 0
                                    Cul_Cod = 0
                                    Reg_Cod = 0
                                    Elem_Cod = 0
                                    Descrizione = ""

                                    If UBound(Arrayp) > 0 Then
                                        Veg_Cod = CInt(Arrayp(0))
                                    End If
                                    If UBound(Arrayp) > 1 Then
                                        Cul_Cod = CInt(Arrayp(1))
                                    End If
                                    If UBound(Arrayp) > 2 Then
                                        Reg_Cod = CInt(Arrayp(2))
                                    End If
                                    If UBound(Arrayp) > 3 Then
                                        Elem_Cod = CInt(Arrayp(3))
                                    End If

                                    'Veg_Cod
                                    If Veg_Cod <> 0 AndAlso DT_SpecieVegetali IsNot Nothing Then
                                        drSearchKey = DT_SpecieVegetali.Select("Veg_Cod = " & Veg_Cod)
                                        If drSearchKey.Length <> 0 Then
                                            Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Veg_Des"))
                                        End If
                                    End If
                                    'Cul_Cod
                                    If Cul_Cod <> 0 AndAlso DT_Cultivar IsNot Nothing Then
                                        drSearchKey = DT_Cultivar.Select("Cul_Cod = " & Cul_Cod)
                                        If drSearchKey.Length <> 0 Then
                                            Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Cul_Des"))
                                        End If
                                    End If
                                    'Reg_Cod
                                    If Reg_Cod <> 0 AndAlso DT_Regolamenti IsNot Nothing Then
                                        drSearchKey = DT_Regolamenti.Select("Reg_Cod = " & Reg_Cod)
                                        If drSearchKey.Length <> 0 Then
                                            Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Reg_Des"))
                                        End If
                                    End If
                                    'Elem_Cod
                                    If Elem_Cod <> 0 AndAlso DT_Categorie IsNot Nothing Then
                                        drSearchKey = DT_Categorie.Select("Elem_Cod = " & CStr(dr.Item("Elem_Cod")))
                                        If drSearchKey.Length <> 0 Then
                                            Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("NomeComune"))
                                        End If
                                    End If

                                    'Impostazione Descrizione
                                    ObjValutazione_Dettaglio_Specifico.Descrizione = Descrizione

                                    ObjValutazione_Dettagli_Specifici.Add(ObjValutazione_Dettaglio_Specifico)

                                Next

                                objValutazione.Valutazione_Dettaglio_Specifico = ObjValutazione_Dettagli_Specifici

                            End If

                        End If


                    End If

                    objValutazioni.Add(objValutazione)

                Next


            End If



        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return objValutazioni



    End Function





    '============================================================================
    Public Function Leggi_Piano_Conti(ByVal Piva As String,
                                      ByVal Valutazione_Piano_Cod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal xFiltroAggiuntivo As String = "",
                                      Optional ByVal xOrderBy As String = ""
                                      ) As List(Of Valutazione_Piano_Conti)


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Piano_Conti()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Piano_Conti As New Valutazione_Piano_Conti_R
        Dim objValutazioni_Piano_Conti As New List(Of Valutazione_Piano_Conti)

        Try

            'Lettura Piano Conti
            Dim dt As DataTable = ObjLeggi_Piano_Conti.Leggi(Piva, Valutazione_Piano_Cod, objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    Dim objValutazione_Piano_Conto As New Valutazione_Piano_Conti With {
                        .primaryKey = New Valutazione_Piano_Conti.PK(CStr(dr.Item("Piva")), CInt(dr.Item("Valutazione_Piano_Cod"))),
                        .Valutazione_Piano_Des = CStr(dr.Item("Valutazione_Piano_Des"))
                    }

                    objValutazioni_Piano_Conti.Add(objValutazione_Piano_Conto)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return objValutazioni_Piano_Conti

    End Function

    '============================================================================
    Public Function Leggi_Piano_Conti_x_Griglia(ByVal Piva As String,
                                                ByVal Valutazione_Piano_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByVal xFiltroAggiuntivo As String = "",
                                                Optional ByVal xOrderBy As String = ""
                                                ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Piano_Conti_x_Griglia()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Piano_Conti As New Valutazione_Piano_Conti_R

        Dim dt As DataTable

        Try

            'Lettura Piano Conti
            dt = ObjLeggi_Piano_Conti.Leggi_x_Griglia(Piva, Valutazione_Piano_Cod, objParametri, xFiltroAggiuntivo, xOrderBy)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '============================================================================
    Public Function Leggi_Piano_ContixConti_Tree(ByVal Piva As String,
                                                 ByVal Valutazione_Piano_Cod As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 Optional ByVal xFiltroAggiuntivo As String = "",
                                                 Optional ByVal xOrderBy As String = ""
                                                 ) As List(Of TreeValutazionePianoContixConti)


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Piano_ContixConti_Tree()"
        Dim messaggioErrore As String = ""
        Dim ID As String = ""
        Dim ID_Start As String = ""
        Dim Indice_Tipo As Integer = 0
        Dim Indice_Attivo_Passivo As Integer = 0

        Dim ObjLeggi_Piano_Conti As New Valutazione_Piano_Conti_R
        Dim ObjLeggi_Piano_ContixConti As New Valutazione_Piano_ContixConti_R
        Dim ObjLeggi_Sezione As New Valutazione_Sezione_R
        Dim ObjLeggi_Gruppo As New Valutazione_Gruppo_R
        Dim ObjLeggi_Testata As New Valutazione_Testata_R
        Dim ObjLeggi_Conto As New Valutazione_Conto_R
        Dim ObjLeggi_Dettaglio As New Valutazione_Dettaglio_R

        Dim objValutazioni_Piano_Conti As New List(Of TreeValutazionePianoContixConti)

        Dim DTPianoContixConti As DataTable = Nothing
        Dim DTDettagli As DataTable = Nothing

        Dim dr_search_gruppo As DataRow()
        Dim dr_search_sezione As DataRow()
        Dim dr_search_conto As DataRow()
        Dim dr_search_pianocontoxconto As DataRow()
        Dim dr_search_dettaglio As DataRow()

        Dim objValutazione_Piano_Conto As New TreeValutazionePianoContixConti
        Dim bEmpty As Boolean = True
        Dim Valutazione_Piano_Des As String = ""
        Dim Flag_Collegato As Boolean = False

        Try


            If Valutazione_Piano_Cod <> 0 Then

                'Lettura Piano Conti
                Dim dt As DataTable = ObjLeggi_Piano_Conti.Leggi(Piva, Valutazione_Piano_Cod, objParametri, xFiltroAggiuntivo, xOrderBy)

                'Lettura PianoContixConti
                DTPianoContixConti = ObjLeggi_Piano_ContixConti.Leggi(Piva, Valutazione_Piano_Cod, 0, objParametri)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                    'Lettura Testate
                    Dim DT_Testate As DataTable = ObjLeggi_Testata.Leggi(Piva, 0, dt(0).Item("Valutazione_Piano_Cod"), objParametri, xFiltroAggiuntivo, xOrderBy)

                    Valutazione_Piano_Des = CStr(dt(0).Item("Valutazione_Piano_Des"))
                    Flag_Collegato = If(DT_Testate.Rows.Count > 0, True, False)

                    bEmpty = False

                End If

            End If

            Select Case bEmpty

                Case False

                    'Lettura Dettagli Associati al Piano Conti
                    DTDettagli = ObjLeggi_Dettaglio.Leggi_Da_Piano_Cod(Piva, Valutazione_Piano_Cod, objParametri)

                Case True

            End Select



            ID_Start = "Piano_" & Valutazione_Piano_Cod

            objValutazione_Piano_Conto.piva = Piva
            objValutazione_Piano_Conto.codice = Valutazione_Piano_Cod
            objValutazione_Piano_Conto.id = ID_Start
            objValutazione_Piano_Conto.tipo = "PianoConti"
            objValutazione_Piano_Conto.descrizione = Valutazione_Piano_Des
            objValutazione_Piano_Conto.flag_collegato = Flag_Collegato


            'Lettura Conti
            Dim DTConti As DataTable = ObjLeggi_Conto.Leggi(0, 0, objParametri)

            'Lettura Gruppi
            Dim DTGruppi As DataTable = ObjLeggi_Gruppo.Leggi(0, objParametri)

            'Lettura Sezioni
            Dim DTSezioni As DataTable = ObjLeggi_Sezione.Leggi(0, 0, objParametri)

            Dim ObjValutazione_Tipi As New List(Of TreeValutazione_Item)

            For Indice_Tipo = 1 To 2

                ID_Start = "Piano_" & Valutazione_Piano_Cod
                Dim ObjValutazione_Tipo As New TreeValutazione_Item

                Dim ObjValutazione_Attivi_Passivi As New List(Of TreeValutazione_Item)

                Select Case Indice_Tipo

                    Case 1 'Patrimoniale

                        ID_Start = ID_Start & "|Tipo_PAT"

                        ObjValutazione_Tipo.id = ID_Start
                        ObjValutazione_Tipo.tipo = "PAT"
                        ObjValutazione_Tipo.codice = 1
                        ObjValutazione_Tipo.descrizione = "Stato Patrimoniale"


                    Case 2 'Economico

                        ID_Start = ID_Start & "|Tipo_ECO"

                        ObjValutazione_Tipo.id = ID_Start
                        ObjValutazione_Tipo.tipo = "ECO"
                        ObjValutazione_Tipo.codice = 2
                        ObjValutazione_Tipo.descrizione = "Conto Economico"

                End Select


                For Indice_Attivo_Passivo = 1 To 2

                    ID = ID_Start

                    Dim ObjValutazione_Attivo_Passivo As New TreeValutazione_Item

                    Select Case Indice_Attivo_Passivo

                        Case 1 'Attivo

                            ID = ID & "|ATTIVO"

                            ObjValutazione_Attivo_Passivo.id = ID
                            ObjValutazione_Attivo_Passivo.tipo = "ATTIVO"
                            ObjValutazione_Attivo_Passivo.codice = 1
                            ObjValutazione_Attivo_Passivo.descrizione = "Attivo"


                        Case 2 'Passivo

                            ID = ID & "|PASSIVO"

                            ObjValutazione_Attivo_Passivo.id = ID
                            ObjValutazione_Attivo_Passivo.tipo = "PASSIVO"
                            ObjValutazione_Attivo_Passivo.codice = 2
                            ObjValutazione_Attivo_Passivo.descrizione = "Passivo"


                    End Select


                    'Filtro i Gruppi

                    'Inserimento Gruppo 0

                    'Aggiungo nuova riga
                    Dim dr_gruppo0 As DataRow
                    dr_gruppo0 = DTGruppi.NewRow

                    dr_gruppo0.Item("Valutazione_Gruppo_Cod") = 0
                    dr_gruppo0.Item("Valutazione_Gruppo_Des") = ""
                    dr_gruppo0.Item("Pat_Eco") = Indice_Tipo
                    dr_gruppo0.Item("Attivo_Passivo") = Indice_Attivo_Passivo

                    DTGruppi.Rows.Add(dr_gruppo0)

                    'Filtro i Gruppi
                    dr_search_gruppo = DTGruppi.Select("Pat_Eco = " & Indice_Tipo & " And " &
                                                        "Attivo_Passivo = " & Indice_Attivo_Passivo)


                    Dim ObjValutazione_Gruppi As New List(Of TreeValutazione_Item)

                    For Each dr_gruppo In dr_search_gruppo

                        'Inserimento Gruppo 
                        Dim ObjValutazione_Gruppo As New TreeValutazione_Item With {
                                                                .id = ID & "|GRU_" & dr_gruppo("Valutazione_Gruppo_Cod"),
                                                                .tipo = "GRUPPO",
                                                                .codice = dr_gruppo("Valutazione_Gruppo_Cod"),
                                                                .descrizione = dr_gruppo("Valutazione_Gruppo_Des"),
                                                                .ordine = 0
                                                                }


                        'Filtro le Sezioni
                        dr_search_sezione = DTSezioni.Select("Valutazione_Gruppo_Cod = " & dr_gruppo("Valutazione_Gruppo_Cod") & " And " &
                                                             "Pat_Eco = " & Indice_Tipo & " And " &
                                                             "Attivo_Passivo = " & Indice_Attivo_Passivo)

                        If dr_search_sezione.Length > 0 Then

                            Dim ObjValutazione_Sezioni As New List(Of TreeValutazione_Item)

                            For Each dr_sezione In dr_search_sezione

                                'Inserimento Sezione 
                                Dim ObjValutazione_Sezione As New TreeValutazione_Item With {
                                                                .id = ID & "|GRU_" & dr_gruppo("Valutazione_Gruppo_Cod") & "|SEZ_" & dr_sezione("Valutazione_Sezione_Cod"),
                                                                .tipo = "SEZIONE",
                                                                .codice = dr_sezione("Valutazione_Sezione_Cod"),
                                                                .descrizione = dr_sezione("Valutazione_Sezione_Des"),
                                                                .ordine = dr_sezione("Ordine")
                                                                }

                                'Filtro i Conti
                                dr_search_conto = DTConti.Select("Valutazione_Sezione_Cod = " & dr_sezione("Valutazione_Sezione_Cod"))

                                If dr_search_conto.Length > 0 Then

                                    Dim ObjValutazione_Conti As New List(Of TreeValutazione_Item)

                                    For Each dr_conto In dr_search_conto

                                        'Inserimento Conto 
                                        Dim ObjValutazione_Conto As New TreeValutazione_Item With {
                                                                        .id = ID & "|GRU_" & dr_gruppo("Valutazione_Gruppo_Cod") & "|SEZ_" & dr_sezione("Valutazione_Sezione_Cod") & "|CONTO_" & dr_conto("Valutazione_Conto_Cod"),
                                                                        .tipo = "CONTO",
                                                                        .codice = dr_conto("Valutazione_Conto_Cod"),
                                                                        .descrizione = dr_conto("Valutazione_Conto_Des"),
                                                                        .ordine = dr_conto("Ordine_Default"),
                                                                        .flag_selezionato = False,
                                                                        .flag_collegato = False
                                                                        }



                                        'Verifica Esistenza in PianoContixConti
                                        If DTPianoContixConti IsNot Nothing AndAlso DTPianoContixConti.Rows.Count > 0 Then

                                            dr_search_pianocontoxconto = DTPianoContixConti.Select("Valutazione_Conto_Cod = " & dr_conto("Valutazione_Conto_Cod"))

                                            If dr_search_pianocontoxconto.Length > 0 Then

                                                ObjValutazione_Conto.flag_selezionato = True
                                                ObjValutazione_Conto.ordine = dr_search_pianocontoxconto(0)("Ordine_PC")

                                            End If

                                        End If


                                        'Verifica Esistenza in Valutazione_Dettaglio
                                        If DTDettagli IsNot Nothing AndAlso DTDettagli.Rows.Count > 0 Then

                                            dr_search_dettaglio = DTDettagli.Select("Valutazione_Conto_Cod = " & dr_conto("Valutazione_Conto_Cod"))

                                            If dr_search_dettaglio.Length > 0 Then

                                                ObjValutazione_Conto.flag_collegato = True

                                            End If

                                        End If

                                        ObjValutazione_Conti.Add(ObjValutazione_Conto)

                                    Next

                                    ObjValutazione_Sezione.children = ObjValutazione_Conti.OrderBy(Function(x) x.ordine).ToList()
                                    'ObjValutazione_Sezione.setSelectedByChildren()
                                    ObjValutazione_Sezioni.Add(ObjValutazione_Sezione)

                                End If

                            Next

                            ObjValutazione_Gruppo.children = ObjValutazione_Sezioni
                            'ObjValutazione_Gruppo.setSelectedByChildren()
                            ObjValutazione_Gruppi.Add(ObjValutazione_Gruppo)

                        End If

                    Next


                    ObjValutazione_Attivo_Passivo.children = ObjValutazione_Gruppi
                    'ObjValutazione_Attivo_Passivo.setSelectedByChildren()
                    ObjValutazione_Attivi_Passivi.Add(ObjValutazione_Attivo_Passivo)


                Next

                ObjValutazione_Tipo.children = ObjValutazione_Attivi_Passivi
                'ObjValutazione_Tipo.setSelectedByChildren()
                ObjValutazione_Tipi.Add(ObjValutazione_Tipo)

            Next

            objValutazione_Piano_Conto.TreeValutazione = ObjValutazione_Tipi
            objValutazioni_Piano_Conti.Add(objValutazione_Piano_Conto)


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return objValutazioni_Piano_Conti

    End Function



    '============================================================================
    Public Function Leggi_Piano_ContixConti(ByVal Piva As String,
                                            ByVal Valutazione_Piano_Cod As Integer,
                                            ByVal Valutazione_Conto_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal xFiltroAggiuntivo As String = "",
                                            Optional ByVal xOrderBy As String = ""
                                            ) As DataTable


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Piano_ContixConti()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Piano_ContixConti As New Valutazione_Piano_ContixConti_R
        Dim objValutazioni_Piano_ContixConti As New List(Of Valutazione_Piano_ContixConti)
        Dim DTPianoContixConti As New DataTable
        Try

            'Lettura Piano Conti
            Dim dt As DataTable = ObjLeggi_Piano_ContixConti.Leggi(Piva, Valutazione_Piano_Cod, Valutazione_Conto_Cod, objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows

                    Dim objValutazione_Piano_ContoxConto As New Valutazione_Piano_ContixConti With {
                        .primaryKey = New Valutazione_Piano_ContixConti.PK(CStr(dr.Item("Piva")), CInt(dr.Item("Valutazione_Piano_Cod")), CInt(dr.Item("Valutazione_Conto_Cod")))
                    }

                    objValutazioni_Piano_ContixConti.Add(objValutazione_Piano_ContoxConto)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DTPianoContixConti

    End Function



    '============================================================================
    Public Function Leggi_Testata(ByVal Piva As String,
                                  ByVal Id_Testata As Integer,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByVal bIncludi_Anno As Boolean,
                                  Optional ByVal xFiltroAggiuntivo As String = "",
                                  Optional ByVal xOrderBy As String = ""
                                  ) As List(Of Valutazione_Testata)


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Testata()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Testata As New Valutazione_Testata_R
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R

        Dim objValutazioni_Testata As New List(Of Valutazione_Testata)

        Dim DT_Anno As DataTable = Nothing
        Dim dr_search As DataRow()

        Try

            'Lettura Testate
            Dim dt As DataTable = ObjLeggi_Testata.Leggi(Piva, Id_Testata, 0, objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt.Rows.Count > 0 Then

                If bIncludi_Anno Then

                    'Lettura di tutte le annualità
                    DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, 0, objParametri)

                End If

                For Each dr As DataRow In dt.Rows

                    Dim objValutazione_Testata As New Valutazione_Testata With {
                        .primaryKey = New Valutazione_Testata.PK(CStr(dr.Item("Piva")), CInt(dr.Item("Id_Testata"))),
                        .Ragione_Sociale = CStr(dr.Item("Rag_Soc")),
                        .Valutazione_Piano_Cod = CInt(dr.Item("Valutazione_Piano_Cod")),
                        .Valutazione_Piano_Des = CStr(dr.Item("Valutazione_Piano_Des")),
                        .Data_Redazione = dr.Item("Data_Redazione"),
                        .Note = dr.Item("Note")
                    }


                    If bIncludi_Anno Then

                        If DT_Anno IsNot Nothing AndAlso DT_Anno.Rows.Count > 0 Then

                            dr_search = DT_Anno.Select("Piva = '" & CStr(dr.Item("Piva")) & "' And " &
                                                   "Id_Testata = " & CStr(dr.Item("Id_Testata")))


                            If dr_search.Length <> 0 Then

                                Dim ObjValutazione_TestataxAnni As New List(Of Valutazione_TestataxAnno)

                                For Each dr_anno In dr_search

                                    Dim ObjValutazione_TestataxAnno As New Valutazione_TestataxAnno With {
                                        .primaryKey = New Valutazione_TestataxAnno.PK(CInt(dr_anno("Anno")), objValutazione_Testata.primaryKey),
                                        .Anno_Tipo = CInt(dr_anno("Anno_Tipo")),
                                        .Anno_Tipo_Des = CStr(dr_anno("Anno_Tipo_Des"))
                                    }

                                    ObjValutazione_TestataxAnni.Add(ObjValutazione_TestataxAnno)

                                Next

                                objValutazione_Testata.Valutazione_TestataxAnno = ObjValutazione_TestataxAnni

                            End If

                        End If


                    End If

                    objValutazioni_Testata.Add(objValutazione_Testata)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return objValutazioni_Testata

    End Function

    Public Function Leggi_Testata_x_Griglia(ByVal Piva As String,
                                            ByVal Id_Testata As Integer,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal xFiltroAggiuntivo As String = "",
                                            Optional ByVal xOrderBy As String = ""
                                            ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Testata_x_Griglia()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Testata As New Valutazione_Testata_R
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R

        Dim dt As DataTable

        Dim DT_Anno As DataTable = Nothing
        Dim dr_search As DataRow()

        Try

            'Lettura Testate
            dt = ObjLeggi_Testata.Leggi(Piva, Id_Testata, 0, objParametri, xFiltroAggiuntivo, xOrderBy)

            Dim contatoreAnni As Integer = 0

            'Aggiungo di default i primi 3 anni
            While contatoreAnni < 3
                contatoreAnni += 1
                AggiungiColumnAnni(dt, contatoreAnni)
            End While

            If dt.Rows.Count > 0 Then

                'Lettura di tutte le annualità
                DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, 0, objParametri)

                For Each dr As DataRow In dt.Rows

                    If DT_Anno IsNot Nothing AndAlso DT_Anno.Rows.Count > 0 Then

                        dr_search = DT_Anno.Select("Piva = '" & CStr(dr.Item("Piva")) & "' And " &
                                                   "Id_Testata = " & CStr(dr.Item("Id_Testata")))

                        If dr_search.Length <> 0 Then

                            Dim contatoreAnniDiTestata As Integer = 0

                            For Each dr_anno In dr_search

                                contatoreAnniDiTestata += 1

                                If contatoreAnniDiTestata > contatoreAnni Then
                                    'Mi ritrovo un anno in più, quindi devo aggiungere un altro set di colonne
                                    contatoreAnni += 1
                                    AggiungiColumnAnni(dt, contatoreAnni)
                                End If

                                Dim nomeColAnnoTesta As String = String.Format("Anno{0}", CStr(contatoreAnniDiTestata))
                                Dim nomeColAnnoTipoTesta As String = String.Format("Anno{0}_Tipo", CStr(contatoreAnniDiTestata))
                                Dim nomeColAnnoTipoDesTesta As String = String.Format("Anno{0}_Tipo_Des", CStr(contatoreAnniDiTestata))

                                dr.Item(nomeColAnnoTesta) = CInt(dr_anno("Anno"))
                                dr.Item(nomeColAnnoTipoTesta) = CInt(dr_anno("Anno_Tipo"))
                                dr.Item(nomeColAnnoTipoDesTesta) = CStr(dr_anno("Anno_Tipo_Des"))

                            Next

                        End If

                    End If

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    Public Function Leggi_Dettaglio_x_Griglia(ByVal Piva As String,
                                              ByVal Id_Testata As Integer,
                                              ByVal Valutazione_Conto_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByVal xFiltroAggiuntivo As String = "",
                                              Optional ByVal xOrderBy As String = ""
                                              ) As DataTable


        Const nomeRoutine = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Dettaglio_x_Griglia()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Dettaglio As New Valutazione_Dettaglio_R
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R

        Dim dt As DataTable
        Dim DT_Dettaglio As New DataTable
        Dim DT_Dettaglio_Specifico As New DataTable
        Dim DT_Anno As DataTable = Nothing
        Dim dr_search As DataRow()
        Dim drAnno_search As DataRow()
        Dim dr_dettaglio As DataRow
        Dim dettaglio_specifico As String = ""
        Dim bPresente As Boolean = False

        Try

            'Lettura Dettagli
            dt = ObjLeggi_Dettaglio.Leggi(Piva, Id_Testata, Valutazione_Conto_Cod, 0, "", objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt.Rows.Count > 0 Then

                'Lettura di tutte le annualità della testata
                DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, 0, objParametri)

                If DT_Anno.Rows.Count > 0 Then

                    'Definizione DT_Dettaglio
                    DT_Dettaglio.Columns.Add(New DataColumn("Piva_SuperUser", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Piva", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Id_Testata", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Conto_Cod", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Conto_Des", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Sezione_Cod", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Sezione_Des", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Gruppo_Cod", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Valutazione_Gruppo_Des", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Pat_Eco", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Pat_Eco_Des", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Attivo_Passivo", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Attivo_Passivo_Des", Type.GetType("System.String")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Ordine", Type.GetType("System.Int32")))
                    DT_Dettaglio.Columns.Add(New DataColumn("Dettaglio_Key", Type.GetType("System.String")))


                    For Each dr_anno As DataRow In DT_Anno.Rows
                        'Inserisco le colonne Anno
                        DT_Dettaglio.Columns.Add(New DataColumn(dr_anno("Anno").ToString, Type.GetType("System.Decimal")))
                    Next

                    DT_Dettaglio.Columns.Add(New DataColumn("Dettaglio_Specifico", Type.GetType("System.String")))


                    For Each dr As DataRow In dt.Rows

                        'Controllo che l'anno sia effettivamente presente
                        drAnno_search = DT_Anno.Select("Anno = " & dr.Item("Anno"))

                        If drAnno_search.Length <> 0 Then

                            bPresente = False

                            If DT_Dettaglio IsNot Nothing AndAlso DT_Dettaglio.Rows.Count > 0 Then

                                dr_search = DT_Dettaglio.Select("Valutazione_Conto_Cod = " & dr.Item("Valutazione_Conto_Cod"))

                                If dr_search.Length <> 0 Then
                                    bPresente = True
                                End If

                            End If

                            If Not bPresente Then

                                'Aggiungo nuova riga
                                dr_dettaglio = DT_Dettaglio.NewRow

                                dr_dettaglio.Item("Piva_SuperUser") = dr("Piva_SuperUser")
                                dr_dettaglio.Item("Piva") = dr("Piva")
                                dr_dettaglio.Item("Id_Testata") = dr("Id_Testata")
                                dr_dettaglio.Item("Valutazione_Conto_Cod") = dr("Valutazione_Conto_Cod")
                                dr_dettaglio.Item("Valutazione_Conto_Des") = dr("Valutazione_Conto_Des")
                                dr_dettaglio.Item("Valutazione_Sezione_Cod") = dr("Valutazione_Sezione_Cod")
                                dr_dettaglio.Item("Valutazione_Sezione_Des") = dr("Valutazione_Sezione_Des")
                                dr_dettaglio.Item("Valutazione_Gruppo_Cod") = dr("Valutazione_Gruppo_Cod")
                                dr_dettaglio.Item("Valutazione_Gruppo_Des") = dr("Valutazione_Gruppo_Des")
                                dr_dettaglio.Item("Pat_Eco") = dr("Pat_Eco")

                                If dr("Pat_Eco") = 1 Then
                                    dr_dettaglio.Item("Pat_Eco_Des") = "Patrimoniale"
                                Else
                                    dr_dettaglio.Item("Pat_Eco_Des") = "Economico"
                                End If

                                dr_dettaglio.Item("Attivo_Passivo") = dr("Attivo_Passivo")

                                If dr("Attivo_Passivo") = 1 Then
                                    dr_dettaglio.Item("Attivo_Passivo_Des") = "Attivo"
                                Else
                                    dr_dettaglio.Item("Attivo_Passivo_Des") = "Passivo"
                                End If

                                dr_dettaglio.Item("Ordine") = dr("Ordine")
                                dr_dettaglio.Item("Dettaglio_Key") = dr("Id_Testata") & "_" & dr("Valutazione_Conto_Cod")
                                dr_dettaglio.Item(CStr(dr("Anno"))) = dr("Valore")



                                'Inserimento Dettaglio Specifico
                                DT_Dettaglio_Specifico = Leggi_Dettaglio_Specifico_x_Griglia(dr("Piva"), dr("Id_Testata"), dr("Valutazione_Conto_Cod"), objParametri)
                                dettaglio_specifico = JsonConvert.SerializeObject(DT_Dettaglio_Specifico)

                                dr_dettaglio.Item("Dettaglio_Specifico") = dettaglio_specifico

                                DT_Dettaglio.Rows.Add(dr_dettaglio)


                            Else

                                dr_search(0)(CStr(dr("Anno"))) = dr("Valore")

                            End If


                        End If

                    Next

                End If

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] :   " & messaggioErrore)

        End Try

        Return DT_Dettaglio

    End Function




    Public Function Leggi_Dettaglio_Specifico_x_Griglia(ByVal Piva As String,
                                                        ByVal Id_Testata As Integer,
                                                        ByVal Valutazione_Conto_Cod As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        Optional ByVal xFiltroAggiuntivo As String = "",
                                                        Optional ByVal xOrderBy As String = ""
                                                        ) As DataTable


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Dettaglio_Specifico_x_Griglia()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Dettaglio_Specifico As New Valutazione_Dettaglio_Specifico_R
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R
        Dim ObjLeggi_SpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim ObjLeggi_Cultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim ObjLeggi_Categorie As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim ObjLeggi_Regolamenti As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

        Dim dt As DataTable
        Dim DT_Dettaglio_Specifico As New DataTable
        Dim DT_Anno As DataTable = Nothing
        Dim DT_SpecieVegetali As DataTable = Nothing
        Dim DT_Cultivar As DataTable = Nothing
        Dim DT_Categorie As DataTable = Nothing
        Dim DT_Regolamenti As DataTable = Nothing

        Dim drSearch As DataRow()
        Dim drSearchKey As DataRow()
        Dim drDettaglio As DataRow
        Dim drAnno_search As DataRow()

        Dim Arrayp As String()
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Reg_Cod As Integer
        Dim Elem_Cod As Integer
        Dim Descrizione As String

        Dim bPresente As Boolean = False

        Try

            'Lettura Dettagli Specifici
            dt = ObjLeggi_Dettaglio_Specifico.Leggi(Piva, Id_Testata, Valutazione_Conto_Cod, 0, "", objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt.Rows.Count > 0 Then

                DT_SpecieVegetali = ObjLeggi_SpecieVegetali.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                DT_Cultivar = ObjLeggi_Cultivar.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                DT_Categorie = ObjLeggi_Categorie.Leggi(0, "", False, "", "", objParametri)
                DT_Regolamenti = ObjLeggi_Regolamenti.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                'Lettura di tutte le annualità della testata
                DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, 0, objParametri)

                If DT_Anno.Rows.Count > 0 Then

                    'Definizione DT_Dettaglio_Specifico
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Piva_SuperUser", Type.GetType("System.String")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Piva", Type.GetType("System.String")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Id_Testata", Type.GetType("System.Int32")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Valutazione_Conto_Cod", Type.GetType("System.Int32")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Valutazione_Conto_Des", Type.GetType("System.String")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Dettaglio_Key", Type.GetType("System.String")))
                    DT_Dettaglio_Specifico.Columns.Add(New DataColumn("Descrizione", Type.GetType("System.String")))


                    For Each dr_anno As DataRow In DT_Anno.Rows
                        'Inserisco le colonne Anno
                        DT_Dettaglio_Specifico.Columns.Add(New DataColumn(dr_anno("Anno").ToString & "_Valore_Unitario", Type.GetType("System.Decimal")))
                        DT_Dettaglio_Specifico.Columns.Add(New DataColumn(dr_anno("Anno").ToString & "_Valore_Totale", Type.GetType("System.Decimal")))
                        DT_Dettaglio_Specifico.Columns.Add(New DataColumn(dr_anno("Anno").ToString & "_Valore_Ha", Type.GetType("System.Decimal")))
                        DT_Dettaglio_Specifico.Columns.Add(New DataColumn(dr_anno("Anno").ToString & "_Valore_Peso", Type.GetType("System.Decimal")))
                    Next

                    For Each dr As DataRow In dt.Rows

                        'Controllo che l'anno sia effettivamente presente
                        drAnno_search = DT_Anno.Select("Anno = " & dr.Item("Anno"))

                        If drAnno_search.Length <> 0 Then

                            bPresente = False

                            If DT_Dettaglio_Specifico IsNot Nothing AndAlso DT_Dettaglio_Specifico.Rows.Count > 0 Then

                                drSearch = DT_Dettaglio_Specifico.Select("Piva = '" & CStr(dr.Item("Piva")) & "' And " &
                                                                          "Id_Testata = " & CStr(dr.Item("Id_Testata")) & " And " &
                                                                          "Valutazione_Conto_Cod = " & CStr(dr.Item("Valutazione_Conto_Cod")) & " And " &
                                                                          "Dettaglio_Key = '" & CStr(dr.Item("Dettaglio_Key")) & "'")

                                If drSearch.Length <> 0 Then
                                    bPresente = True
                                End If

                            End If

                            If Not bPresente Then

                                'Aggiungo nuova riga
                                drDettaglio = DT_Dettaglio_Specifico.NewRow


                                drDettaglio.Item("Piva_SuperUser") = dr("Piva_SuperUser")
                                drDettaglio.Item("Piva") = dr("Piva")
                                drDettaglio.Item("Id_Testata") = dr("Id_Testata")
                                drDettaglio.Item("Valutazione_Conto_Cod") = dr("Valutazione_Conto_Cod")
                                drDettaglio.Item("Valutazione_Conto_Des") = dr("Valutazione_Conto_Des")
                                drDettaglio.Item("Dettaglio_Key") = dr("Dettaglio_Key")


                                'Definizione Descrizione
                                Arrayp = Split(CStr(dr("Dettaglio_Key")), "|")
                                Veg_Cod = 0
                                Cul_Cod = 0
                                Reg_Cod = 0
                                Elem_Cod = 0
                                Descrizione = ""

                                If UBound(Arrayp) > 0 Then
                                    Veg_Cod = CInt(Arrayp(0))
                                End If
                                If UBound(Arrayp) > 1 Then
                                    Cul_Cod = CInt(Arrayp(1))
                                End If
                                If UBound(Arrayp) > 2 Then
                                    Reg_Cod = CInt(Arrayp(2))
                                End If
                                If UBound(Arrayp) > 3 Then
                                    Elem_Cod = CInt(Arrayp(3))
                                End If

                                'Veg_Cod
                                If Veg_Cod <> 0 AndAlso DT_SpecieVegetali IsNot Nothing Then
                                    drSearchKey = DT_SpecieVegetali.Select("Veg_Cod = " & Veg_Cod)
                                    If drSearchKey.Length <> 0 Then
                                        Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Veg_Des"))
                                    End If
                                End If
                                'Cul_Cod
                                If Cul_Cod <> 0 AndAlso DT_Cultivar IsNot Nothing Then
                                    drSearchKey = DT_Cultivar.Select("Cul_Cod = " & Cul_Cod)
                                    If drSearchKey.Length <> 0 Then
                                        Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Cul_Des"))
                                    End If
                                End If
                                'Reg_Cod
                                If Reg_Cod <> 0 AndAlso DT_Regolamenti IsNot Nothing Then
                                    drSearchKey = DT_Regolamenti.Select("Reg_Cod = " & Reg_Cod)
                                    If drSearchKey.Length <> 0 Then
                                        Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("Reg_Des"))
                                    End If
                                End If
                                'Elem_Cod
                                If Elem_Cod <> 0 AndAlso DT_Categorie IsNot Nothing Then
                                    drSearchKey = DT_Categorie.Select("Elem_Cod = " & CStr(dr.Item("Elem_Cod")))
                                    If drSearchKey.Length <> 0 Then
                                        Descrizione = Descrizione & If(Descrizione <> "", " - ", "") & CStr(drSearchKey(0)("NomeComune"))
                                    End If
                                End If

                                'Impostazione Descrizione
                                drDettaglio.Item("Descrizione") = Descrizione

                                drDettaglio.Item(CStr(dr("Anno")) & "_Valore_Unitario") = CDec(dr("Valore_Unitario"))
                                drDettaglio.Item(CStr(dr("Anno")) & "_Valore_Totale") = CDec(dr("Valore_Totale"))
                                drDettaglio.Item(CStr(dr("Anno")) & "_Valore_Ha") = CDec(dr("Valore_Ha"))
                                drDettaglio.Item(CStr(dr("Anno")) & "_Valore_Peso") = CDec(dr("Valore_Peso"))

                                DT_Dettaglio_Specifico.Rows.Add(drDettaglio)

                            Else


                                drSearch(0).Item(CStr(dr("Anno")) & "_Valore_Unitario") = CDec(dr("Valore_Unitario"))
                                drSearch(0).Item(CStr(dr("Anno")) & "_Valore_Totale") = CDec(dr("Valore_Totale"))
                                drSearch(0).Item(CStr(dr("Anno")) & "_Valore_Ha") = CDec(dr("Valore_Ha"))
                                drSearch(0).Item(CStr(dr("Anno")) & "_Valore_Peso") = CDec(dr("Valore_Peso"))


                            End If


                        End If

                    Next

                End If

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT_Dettaglio_Specifico

    End Function


    '============================================================================
    Public Function Leggi_Conto(ByVal Valutazione_Conto_Cod As Integer,
                                ByVal Valutazione_Sezione_Cod As Integer,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByVal xFiltroAggiuntivo As String = "",
                                Optional ByVal xOrderBy As String = ""
                                ) As List(Of Valutazione_Conto)


        Const nomeRoutine As String = "AgronicaCoreContab_Biz.Valutazioni_R.Leggi_Conto()"
        Dim messaggioErrore As String = ""

        Dim ObjLeggi_Conto As New Valutazione_Conto_R
        Dim objValutazioni_Conti As New List(Of Valutazione_Conto)

        Try

            'Lettura Conti
            Dim dt As DataTable = ObjLeggi_Conto.Leggi(Valutazione_Conto_Cod, Valutazione_Sezione_Cod, objParametri, xFiltroAggiuntivo, xOrderBy)

            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows

                    Dim objValutazione_Conto As New Valutazione_Conto With {
                        .primaryKey = New Valutazione_Conto.PK(CInt(dr.Item("Valutazione_Conto_Cod"))),
                        .Valutazione_Conto_Des = CStr(dr.Item("Valutazione_Conto_Des")),
                        .Valutazione_Sezione_Cod = CInt(dr.Item("Valutazione_Sezione_Cod")),
                        .Ordine_Default = CInt(dr.Item("Ordine_Default"))
                    }

                    objValutazioni_Conti.Add(objValutazione_Conto)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return objValutazioni_Conti

    End Function



    Private Sub AggiungiColumnAnni(ByRef dt As DataTable, ByVal contatoreAnni As Integer)
        Dim nomeColAnno As String = String.Format("Anno{0}", CStr(contatoreAnni))
        Dim nomeColAnnoTipo As String = String.Format("Anno{0}_Tipo", CStr(contatoreAnni))
        Dim nomeColAnnoTipoDes As String = String.Format("Anno{0}_Tipo_Des", CStr(contatoreAnni))

        dt.Columns.Add(New DataColumn(nomeColAnno, Type.GetType("System.Int32")))
        dt.Columns.Add(New DataColumn(nomeColAnnoTipo, Type.GetType("System.Int32")))
        dt.Columns.Add(New DataColumn(nomeColAnnoTipoDes, Type.GetType("System.String")))
    End Sub


End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Valutazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function ScriviModifica_Piano_Conti(ByVal objValutazione_Piano_Conti As Valutazione_Piano_Conti,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Integer

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ScriviModifica_Piano_Conti()"

        Dim ObjSequenze As Agro_Sequenze
        Dim ObjValutazione_Piano_Conti_W As New AgronicaCoreContabDAL.Valutazione_Piano_Conti_W
        Dim ObjValutazione_Piano_ContixConti_W As New AgronicaCoreContabDAL.Valutazione_Piano_ContixConti_W

        Dim Dummy As Boolean

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Integer = 0
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            If objValutazione_Piano_Conti.flag_cancellazione = False Then

                If objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod = 0 Then

                    'Non ho Valutazione_Piano_Cod ==> Inserimento

                    If objValutazione_Piano_Conti.primaryKey.Piva = "" Then
                        Throw New Exception("Partita Iva non specificata")
                    End If

                    ObjSequenze = New Agro_Sequenze

                    'Richiedo un nuovo codice
                    objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod = ObjSequenze.NuovoId_Tabella("Valutazione_Piano_Conti",
                                                                                                              0,
                                                                                                              2000000000,
                                                                                                              objParametri)

                    ObjSequenze = Nothing

                    Dummy = ObjValutazione_Piano_Conti_W.Scrivi(objValutazione_Piano_Conti, objParametri)

                Else

                    'Ho Valutazione_Piano_Cod ==> Modifica
                    ObjValutazione_Piano_Conti_W.Modifica(objValutazione_Piano_Conti, objParametri)
                    Dummy = True

                End If

            Else
                'Cancellazione
                Dim objValutazione_Piano_ContoxConto As New Valutazione_Piano_ContixConti With {
                .primaryKey = New Valutazione_Piano_ContixConti.PK(objValutazione_Piano_Conti.primaryKey.Piva, objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod, 0)
                }

                Dummy = ObjValutazione_Piano_ContixConti_W.Cancella(objValutazione_Piano_ContoxConto, objParametri)

                If Dummy Then
                    Dummy = ObjValutazione_Piano_Conti_W.Cancella(objValutazione_Piano_Conti, objParametri)
                End If

            End If

            'Restituisco un valore Dummy
            Select Case Dummy
                Case True
                    xRisp = objValutazione_Piano_Conti.primaryKey.Valutazione_Piano_Cod
                Case False
                    xRisp = -1
            End Select

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = -1
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


    '============================================================================
    Public Function ScriviModifica_Testata(ByVal objValutazione_Testata As Valutazione_Testata,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ScriviModifica_Testata()"

        Dim ObjSequenze As Agro_Sequenze
        Dim ObjValutazione_Testata_W As New AgronicaCoreContabDAL.Valutazione_Testata_W
        Dim ObjValutazione_TestataxAnno_R As New AgronicaCoreContabDAL.Valutazione_TestataxAnno_R
        Dim ObjValutazione_TestataxAnno_W As New AgronicaCoreContabDAL.Valutazione_TestataxAnno_W
        Dim ObjValutazione_Dettaglio_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_W
        Dim ObjValutazione_Conto_R As New AgronicaCoreContabDAL.Valutazione_Conto_R
        Dim ObjValutazione_PianoContixConti_R As New AgronicaCoreContabDAL.Valutazione_Piano_ContixConti_R
        Dim ObjValutazione_Dettagli_R As New AgronicaCoreContabDAL.Valutazione_Dettaglio_R

        Dim DTTestata As New DataTable
        Dim DTConti As New DataTable
        Dim DTAnni As New DataTable
        Dim DTDettagli As DataTable
        Dim drsearch_dettagli As DataRow()
        Dim Dummy As Boolean

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        Dim bPresente As Boolean
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            If objValutazione_Testata.flag_cancellazione = False Then

                'Lettura di tutti i conti associati al piano conti
                DTConti = ObjValutazione_PianoContixConti_R.Leggi(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.Valutazione_Piano_Cod, 0, objParametri)


                If objValutazione_Testata.primaryKey.Id_Testata = 0 Then

                    'Non ho Id_Testata ==> Inserimento

                    If objValutazione_Testata.primaryKey.Piva = "" Then
                        Throw New Exception("Partita Iva non specificata")
                    End If

                    ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                    'Richiedo un nuovo codice rapporto
                    objValutazione_Testata.primaryKey.Id_Testata = ObjSequenze.NuovoId_Tabella("Valutazione_Testata",
                                                                                               0, 2000000000, objParametri)

                    ObjSequenze = Nothing

                    Dummy = ObjValutazione_Testata_W.Scrivi(objValutazione_Testata, objParametri)

                    If objValutazione_Testata.Valutazione_TestataxAnno.Count > 0 Then

                        For Each objValutazione_TestataxAnno As Valutazione_TestataxAnno In objValutazione_Testata.Valutazione_TestataxAnno

                            objValutazione_TestataxAnno.primaryKey.valutazioneTestataPK = objValutazione_Testata.primaryKey

                            If objValutazione_TestataxAnno.flag_cancellazione = False Then

                                'Modifica / inserimento                                
                                DTAnni = ObjValutazione_TestataxAnno_R.LeggixAnno(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, 0, objValutazione_TestataxAnno.primaryKey.Anno, objParametri)

                                If DTAnni.Rows.Count = 0 Then

                                    Dummy = ObjValutazione_TestataxAnno_W.Scrivi(objValutazione_TestataxAnno, objParametri)

                                    '===================================================================================================
                                    'Inserimento Dettagli Associati all'Anno
                                    '---------------------------------------------------------------------------------------------------
                                    If DTConti.Rows.Count > 0 Then

                                        For Each drConti In DTConti.Rows

                                            Dim ObjValutazione_Dettaglio As New Valutazione_Dettaglio

                                            ObjValutazione_Dettaglio.Piva = objValutazione_Testata.primaryKey.Piva
                                            ObjValutazione_Dettaglio.Id_Testata = objValutazione_Testata.primaryKey.Id_Testata
                                            ObjValutazione_Dettaglio.Valutazione_Conto_Cod = drConti("Valutazione_Conto_Cod")
                                            ObjValutazione_Dettaglio.Anno = objValutazione_TestataxAnno.primaryKey.Anno
                                            ObjValutazione_Dettaglio.Ordine = drConti("Ordine_PC")
                                            ObjValutazione_Dettaglio.Valore = 0

                                            Dummy = ObjValutazione_Dettaglio_W.Scrivi(ObjValutazione_Dettaglio, objParametri)

                                        Next

                                    End If
                                    '===================================================================================================


                                End If

                            Else
                                'CANCELLAZIONE
                                Dummy = ObjValutazione_TestataxAnno_W.Cancella(objValutazione_TestataxAnno, objParametri)
                            End If

                        Next

                    End If

                Else


                    ''Ho Id_Testata ==> Modifica

                    If objValutazione_Testata.primaryKey.Piva = "" Then
                        Throw New Exception("Partita Iva non specificata")
                    End If

                    'Campi Modificabili: Data_Redazione, Note, Anno_Tipo
                    Dummy = ObjValutazione_Testata_W.Modifica(objValutazione_Testata, objParametri)

                    'Lettura di tutti i dettagli associati alla testata
                    DTDettagli = ObjValutazione_Dettagli_R.Leggi(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, 0, 0, "", objParametri)

                    If objValutazione_Testata.Valutazione_TestataxAnno.Count > 0 Then

                        For Each objValutazione_TestataxAnno As Valutazione_TestataxAnno In objValutazione_Testata.Valutazione_TestataxAnno

                            objValutazione_TestataxAnno.primaryKey.valutazioneTestataPK = objValutazione_Testata.primaryKey

                            If objValutazione_TestataxAnno.flag_cancellazione = False Then

                                'Verifico se l'anno è già presente
                                DTAnni = ObjValutazione_TestataxAnno_R.LeggixAnno(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, 0, objValutazione_TestataxAnno.primaryKey.Anno, objParametri)

                                If DTAnni.Rows.Count = 0 Then
                                    'Inserimento
                                    Dummy = ObjValutazione_TestataxAnno_W.Scrivi(objValutazione_TestataxAnno, objParametri)
                                Else
                                    'Modifica
                                    ObjValutazione_TestataxAnno_W.Modifica(objValutazione_TestataxAnno, objParametri)
                                End If

                                '===================================================================================================
                                'Inserimento Dettagli Associati all'Anno
                                '---------------------------------------------------------------------------------------------------
                                If DTConti.Rows.Count > 0 Then

                                    For Each drConti In DTConti.Rows

                                        bPresente = False

                                        If DTDettagli.Rows.Count > 0 Then

                                            'Controllo se il dettaglio è presente
                                            drsearch_dettagli = DTDettagli.Select("Valutazione_Conto_Cod = " & drConti("Valutazione_Conto_Cod") & " And Anno = " & objValutazione_TestataxAnno.primaryKey.Anno)

                                            If drsearch_dettagli.Length <> 0 Then
                                                bPresente = True
                                            End If

                                        End If

                                        If Not bPresente Then

                                            Dim ObjValutazione_Dettaglio As New Valutazione_Dettaglio

                                            ObjValutazione_Dettaglio.Piva = objValutazione_Testata.primaryKey.Piva
                                            ObjValutazione_Dettaglio.Id_Testata = objValutazione_Testata.primaryKey.Id_Testata
                                            ObjValutazione_Dettaglio.Valutazione_Conto_Cod = drConti("Valutazione_Conto_Cod")
                                            ObjValutazione_Dettaglio.Anno = objValutazione_TestataxAnno.primaryKey.Anno
                                            ObjValutazione_Dettaglio.Ordine = drConti("Ordine_PC")
                                            ObjValutazione_Dettaglio.Valore = 0

                                            Dummy = ObjValutazione_Dettaglio_W.Scrivi(ObjValutazione_Dettaglio, objParametri)

                                        End If

                                    Next

                                End If
                                '===================================================================================================

                                'In caso di nuovo Anno
                                If DTAnni.Rows.Count = 0 Then

                                    'Aggiornamento Dettaglio
                                    Aggiorna_Dettaglio(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, objParametri,, objValutazione_TestataxAnno.primaryKey.Anno)


                                    'Aggiornamento Dettaglio Specifico
                                    Aggiorna_Dettaglio_Specifico(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, objParametri,, objValutazione_TestataxAnno.primaryKey.Anno)

                                Else

                                    'Modifica --> nessun aggiornamento altrimenti perderei i valori già presenti

                                End If



                            Else

                                'CANCELLAZIONE
                                Dummy = ObjValutazione_Dettaglio_W.Cancella2(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, 0, objValutazione_TestataxAnno.primaryKey.Anno, objParametri)
                                Dummy = ObjValutazione_TestataxAnno_W.Cancella(objValutazione_TestataxAnno, objParametri)
                            End If

                        Next

                    End If

                End If




            Else

                Dummy = ObjValutazione_Dettaglio_W.Cancella2(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, 0, 0, objParametri)
                Dummy = ObjValutazione_TestataxAnno_W.Cancella2(objValutazione_Testata.primaryKey.Piva, objValutazione_Testata.primaryKey.Id_Testata, objParametri)
                Dummy = ObjValutazione_Testata_W.Cancella(objValutazione_Testata, objParametri)

            End If

            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


    '============================================================================
    Public Function ScriviModifica_Piano_ContixConti_Tree(ByVal objValutazioni_Piano_Conti As TreeValutazionePianoContixConti,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ScriviModifica_Piano_ContixConti()"

        Dim DTPianoContixConti As DataTable
        Dim DTTestate As DataTable
        Dim ObjValutazione_Piani_ContixConti_R As New AgronicaCoreContabDAL.Valutazione_Piano_ContixConti_R
        Dim ObjValutazione_Piani_ContixConti_W As New AgronicaCoreContabDAL.Valutazione_Piano_ContixConti_W
        Dim ObjValutazione_Dettaglio_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_W
        Dim ObjLeggi_Testata As New Valutazione_Testata_R

        Dim Valutazione_Piano_Cod As Integer
        Dim Dummy As Boolean
        Dim bPresente As Boolean = False
        Dim drSearch As DataRow()

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)


            'Scrittura/Modifica Piano Conti
            If String.IsNullOrEmpty(objValutazioni_Piano_Conti.piva) Then
                Throw New Exception("Partita Iva non specificata")
            End If

            Dim objValutazione_Piano_Conti As New Valutazione_Piano_Conti

            objValutazione_Piano_Conti.primaryKey = New Valutazione_Piano_Conti.PK(objValutazioni_Piano_Conti.piva, objValutazioni_Piano_Conti.codice)
            objValutazione_Piano_Conti.Valutazione_Piano_Des = objValutazioni_Piano_Conti.descrizione
            objValutazione_Piano_Conti.flag_cancellazione = False

            Valutazione_Piano_Cod = ScriviModifica_Piano_Conti(objValutazione_Piano_Conti, objParametri)
            objValutazioni_Piano_Conti.codice = Valutazione_Piano_Cod


            If Valutazione_Piano_Cod > 0 Then

                'Lettura Testate
                DTTestate = ObjLeggi_Testata.Leggi(objValutazioni_Piano_Conti.piva, 0, Valutazione_Piano_Cod, objParametri)

                'Lettura PianoContixConti
                DTPianoContixConti = ObjValutazione_Piani_ContixConti_R.Leggi(objValutazioni_Piano_Conti.piva,
                                                                         Valutazione_Piano_Cod,
                                                                         0,
                                                                         objParametri)


                For Each objTipo In objValutazioni_Piano_Conti.TreeValutazione

                    For Each objAttivo_Passivo In objTipo.children

                        For Each objGruppo In objAttivo_Passivo.children

                            For Each objSezione In objGruppo.children

                                For Each objConto In objSezione.children

                                    Dim objValutazione_Piano_ContoxConto As New Valutazione_Piano_ContixConti With {
                                        .primaryKey = New Valutazione_Piano_ContixConti.PK(objValutazioni_Piano_Conti.piva,
                                                                                           Valutazione_Piano_Cod,
                                                                                           objConto.codice),
                                        .ordine_pc = objConto.ordine,
                                        .flag_cancellazione = Not objConto.flag_selezionato
                                    }

                                    'Controllo Esistenza
                                    bPresente = False
                                    If DTPianoContixConti IsNot Nothing AndAlso DTPianoContixConti.Rows.Count > 0 Then

                                        'Controllo se il dettaglio specifico è presente
                                        drSearch = DTPianoContixConti.Select("Valutazione_Conto_Cod = " & objValutazione_Piano_ContoxConto.primaryKey.Valutazione_Conto_Cod)

                                        If drSearch.Length <> 0 Then

                                            bPresente = True
                                        End If

                                    End If


                                    Select Case objValutazione_Piano_ContoxConto.flag_cancellazione

                                        Case False


                                            If Not bPresente Then

                                                Dummy = ObjValutazione_Piani_ContixConti_W.Scrivi(objValutazione_Piano_ContoxConto, objParametri)


                                                '===================================================================================================
                                                'Inserimento in Tabella Valutazione Dettaglio per tutte le testate presenti
                                                '---------------------------------------------------------------------------------------------------
                                                If DTTestate.Rows.Count > 0 Then

                                                    For Each drtestate In DTTestate.Rows

                                                        Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
                                                        Dim listItems As List(Of Valutazione_Testata) = objR.Leggi_Testata(drtestate("Piva"), drtestate("Id_Testata"),
                                                                                                         objParametri,
                                                                                                         True,
                                                                                                         "", "")

                                                        For Each objValutazione_Testata As Valutazione_Testata In listItems

                                                            ScriviModifica_Testata(objValutazione_Testata, objParametri)
                                                        Next

                                                    Next

                                                End If
                                                '===================================================================================================

                                            Else

                                                Dummy = ObjValutazione_Piani_ContixConti_W.Modifica(objValutazione_Piano_ContoxConto, objParametri)

                                            End If

                                        Case True

                                            'Cancellazione
                                            If bPresente Then

                                                Dummy = ObjValutazione_Piani_ContixConti_W.Cancella(objValutazione_Piano_ContoxConto, objParametri)

                                                If Dummy Then

                                                    If DTTestate.Rows.Count > 0 Then

                                                        For Each drtestate In DTTestate.Rows

                                                            Dummy = ObjValutazione_Dettaglio_W.Cancella2(objValutazione_Piano_ContoxConto.primaryKey.Piva, drtestate("Id_Testata"), objValutazione_Piano_ContoxConto.primaryKey.Valutazione_Conto_Cod, 0, objParametri)

                                                        Next

                                                    End If

                                                End If

                                            End If

                                    End Select

                                Next

                            Next

                        Next

                    Next

                Next

            End If

            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function



    '============================================================================
    Public Function ScriviModifica_Dettaglio(ByVal objValutazione_Dettaglio As Valutazione_Dettaglio,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ScriviModifica_Dettaglio()"

        Dim ObjValutazione_Dettaglio_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_W
        Dim ObjValutazione_Dettaglio_Specifico_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_Specifico_W

        Dim Dummy As Boolean

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            'Verifico l'operazione richiesta
            Select Case objValutazione_Dettaglio.TipoOperazioneDB

                Case enum_TipoOperazioneDB.Scrittura    'SALVA -------------------------------------------------------

                    Dummy = ObjValutazione_Dettaglio_W.Scrivi(objValutazione_Dettaglio, objParametri)

                    If objValutazione_Dettaglio.Valutazione_Dettaglio_Specifico.Count > 0 Then

                        For Each objValutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico In objValutazione_Dettaglio.Valutazione_Dettaglio_Specifico

                            Select Case objValutazione_Dettaglio_Specifico.TipoOperazioneDB

                                Case enum_TipoOperazioneDB.Scrittura    'SALVA -------------------------------------------------------

                                    Dummy = ObjValutazione_Dettaglio_Specifico_W.Scrivi(objValutazione_Dettaglio_Specifico, objParametri)

                                Case enum_TipoOperazioneDB.Cancellazione 'CANCELLAZIONE

                                    Dummy = ObjValutazione_Dettaglio_Specifico_W.Cancella(objValutazione_Dettaglio_Specifico, objParametri)


                            End Select


                        Next

                    End If


                Case enum_TipoOperazioneDB.Modifica    'MODIFICA -------------------------------------------------------

                    Dummy = ObjValutazione_Dettaglio_W.Modifica(objValutazione_Dettaglio, objParametri)


                Case enum_TipoOperazioneDB.Cancellazione 'CANCELLAZIONE


                    Dummy = ObjValutazione_Dettaglio_W.Cancella(objValutazione_Dettaglio, objParametri)



            End Select


            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


    Public Function ModificaDettaglio_x_Griglia(ByVal jsonGriglia As String,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ModificaDettaglio_x_Griglia()"

        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R
        Dim ObjValutazione_Dettaglio_W As New Valutazione_Dettaglio_W

        Dim DT_Anno As DataTable = Nothing
        Dim Dummy As Boolean = True

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim Id_Testata As Integer = 0
        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            'Scompatto il JSON           
            Dim objJson As JArray = JArray.Parse(jsonGriglia)

            For Each obj As JObject In objJson

                If Id_Testata = 0 Then

                    'Lettura di tutte le annualità della testata
                    Id_Testata = obj("Id_Testata")

                    DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(obj("Piva"), Id_Testata, 0, 0, objParametri)

                End If

                If DT_Anno.Rows.Count > 0 Then

                    For Each dr_anno In DT_Anno.Rows

                        'Aggiornamento Dettaglio
                        Dim ObjValutazione_Dettaglio As New Valutazione_Dettaglio

                        ObjValutazione_Dettaglio.Piva = CStr(obj("Piva"))
                        ObjValutazione_Dettaglio.Id_Testata = Id_Testata
                        ObjValutazione_Dettaglio.Valutazione_Conto_Cod = CInt(obj("Valutazione_Conto_Cod"))
                        ObjValutazione_Dettaglio.Anno = dr_anno("Anno")

                        If Not IsNumeric(obj(CStr(dr_anno("Anno")))) Then
                            ObjValutazione_Dettaglio.Valore = 0
                        Else
                            ObjValutazione_Dettaglio.Valore = obj(CStr(dr_anno("Anno")))
                        End If


                        Dummy = ObjValutazione_Dettaglio_W.Modifica(ObjValutazione_Dettaglio, objParametri)

                    Next

                End If

            Next


            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


    Public Function ModificaDettaglio_Specifico_x_Griglia(ByVal jsonGriglia As String,
                                                          ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ModificaDettaglio_Specifico_x_Griglia()"

        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R
        Dim ObjValutazione_Dettaglio_Specifico_W As New Valutazione_Dettaglio_Specifico_W

        Dim DT_Anno As DataTable = Nothing
        Dim Dummy As Boolean
        Dim Id_Testata As Integer = 0

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)


            'Scompatto il JSON
            Dim objJson As JArray = JArray.Parse(jsonGriglia)

            For Each obj As JObject In objJson

                If Id_Testata = 0 Then

                    'Lettura di tutte le annualità della testata
                    Id_Testata = obj("Id_Testata")

                    DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(obj("Piva"), Id_Testata, 0, 0, objParametri)

                End If

                If DT_Anno.Rows.Count > 0 Then

                    For Each dr_anno In DT_Anno.Rows

                        'Aggiornamento Dettaglio
                        Dim ObjValutazione_Dettaglio_Specifico As New Valutazione_Dettaglio_Specifico

                        ObjValutazione_Dettaglio_Specifico.Piva = CStr(obj("Piva"))
                        ObjValutazione_Dettaglio_Specifico.Id_Testata = Id_Testata
                        ObjValutazione_Dettaglio_Specifico.Valutazione_Conto_Cod = CInt(obj("Valutazione_Conto_Cod"))
                        ObjValutazione_Dettaglio_Specifico.Anno = dr_anno("Anno")

                        If Not IsNumeric(obj(CStr(dr_anno("Anno")) & "_Valore_Unitario")) Then
                            ObjValutazione_Dettaglio_Specifico.Valore_Unitario = 0
                        Else
                            ObjValutazione_Dettaglio_Specifico.Valore_Unitario = obj(CStr(dr_anno("Anno")) & "_Valore_Unitario")
                        End If

                        If Not IsNumeric(obj(CStr(dr_anno("Anno")) & "_Valore_Totale")) Then
                            ObjValutazione_Dettaglio_Specifico.Valore_Totale = 0
                        Else
                            ObjValutazione_Dettaglio_Specifico.Valore_Totale = obj(CStr(dr_anno("Anno")) & "_Valore_Totale")
                        End If


                        ObjValutazione_Dettaglio_Specifico.Dettaglio_Key = CStr(obj("Dettaglio_Key"))

                        Dummy = ObjValutazione_Dettaglio_Specifico_W.Modifica(ObjValutazione_Dettaglio_Specifico, False, objParametri)

                    Next

                End If

            Next



            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


    '============================================================================
    Public Function ScriviModifica_Dettaglio_Specifico(ByVal objValutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico,
                                                       ByVal bAggiornamento As Boolean,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.ScriviModifica_Dettaglio_Specifico()"

        Dim ObjValutazione_Dettaglio_Specifico_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_Specifico_W

        Dim Dummy As Boolean

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            'Verifico l'operazione richiesta
            Select Case objValutazione_Dettaglio_Specifico.TipoOperazioneDB

                Case enum_TipoOperazioneDB.Scrittura    'SALVA -------------------------------------------------------

                    Dummy = ObjValutazione_Dettaglio_Specifico_W.Scrivi(objValutazione_Dettaglio_Specifico, objParametri)

                Case enum_TipoOperazioneDB.Modifica    'MODIFICA -------------------------------------------------------

                    Dummy = ObjValutazione_Dettaglio_Specifico_W.Modifica(objValutazione_Dettaglio_Specifico, bAggiornamento, objParametri)


                Case enum_TipoOperazioneDB.Cancellazione 'CANCELLAZIONE


                    Dummy = ObjValutazione_Dettaglio_Specifico_W.Cancella(objValutazione_Dettaglio_Specifico, objParametri)



            End Select


            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function



    '============================================================================
    Public Function Aggiorna_Dettaglio(ByVal Piva As String,
                                       ByVal Id_Testata As Integer,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal Conto_Cod As Integer = 0,
                                       Optional ByVal Anno As Integer = 0
                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.Aggiorna_Dettaglio()"

        Dim ObjValutazione_Dettaglio_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_W

        Dim Dummy As Boolean = True

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim ObjLeggi_Dettaglio As New Valutazione_Dettaglio_R
        Dim ObjLeggi_Dettaglio_W As New Valutazione_Dettaglio_W
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R
        Dim ObjLeggi_Attivita As New AgronicaCoreContabDAL.Attivita_R

        Dim dt As DataTable
        Dim DT_Dettaglio As New DataTable
        Dim DT_Attivita As New DataTable
        Dim DT_Anno As DataTable
        'Dim dr_search() As DataRow
        'Dim drAnno_search() As DataRow
        'Dim drAttivita_search() As DataRow        
        Dim Dettaglio_Key As String = ""
        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE
        Dim Nome_Colonna As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim Totale_CDG As Decimal
        'Dim TipoOperazioneDB As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            'Lettura di tutti i dettagli della testata           
            dt = ObjLeggi_Dettaglio.Leggi(Piva, Id_Testata, Conto_Cod, 0, "", objParametri)

            ''Lettura di tutte le attivita         
            'DT_Attivita = ObjLeggi_Attivita.LeggiAttivitaXGrigliaCDG(0, Piva, "", "", objParametri)

            If dt.Rows.Count > 0 Then

                'Lettura di tutte le annualità della testata
                DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, Anno, objParametri)

                If DT_Anno.Rows.Count > 0 Then

                    'Scorro i dettagli per verificare il conto
                    For Each dr As DataRow In dt.Rows

                        For Each dr_Anno As DataRow In DT_Anno.Rows

                            'Costruzione Filtro Validità
                            Validita_Inizio = "01/01/" & dr_Anno("Anno")
                            Validita_Fine = "31/12/" & dr_Anno("Anno")

                            Select Case dr("Attivo_Passivo")
                                Case "1"
                                    Nome_Colonna = "Valutazione_Conto_Cod_Attivo"
                                    xFiltroAggiuntivo = "Costi_Ricavi = 1"
                                Case "2"
                                    Nome_Colonna = "Valutazione_Conto_Cod_Passivo"
                                    xFiltroAggiuntivo = "Costi_Ricavi = 0"
                            End Select

                            Totale_CDG = ObjLeggi_Dettaglio.Leggi_Da_Id_Attivita(Piva, dr("Valutazione_Conto_Cod"), Nome_Colonna, Validita_Inizio, Validita_Fine, objParametri, xFiltroAggiuntivo)

                            If Totale_CDG <> -1 Then
                                'Aggiornamento Valore
                                ObjLeggi_Dettaglio_W.Modifica_Valore(Piva, Id_Testata, dr("Valutazione_Conto_Cod"), dr_Anno("Anno"), Totale_CDG, objParametri)
                            End If

                        Next

                    Next

                End If

            End If


            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function




    '============================================================================
    Public Function Aggiorna_Dettaglio_Specifico(ByVal Piva As String,
                                                 ByVal Id_Testata As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 Optional ByVal Conto_Cod As Integer = 0,
                                                 Optional ByVal Anno As Integer = 0
                                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Valutazioni_W.Aggiorna_Dettaglio_Specifico()"

        Dim ObjValutazione_Dettaglio_Specifico_W As New AgronicaCoreContabDAL.Valutazione_Dettaglio_Specifico_W

        Dim Dummy As Boolean

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True


        Dim ObjLeggi_Dettaglio As New Valutazione_Dettaglio_R
        Dim ObjLeggi_Dettaglio_Specifico As New Valutazione_Dettaglio_Specifico_R
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R

        Dim objValutazione_Dettaglio_Specifico As Valutazione_Dettaglio_Specifico

        Dim dt As DataTable
        Dim DT_Dettaglio As New DataTable
        Dim DT_Dettaglio_Specifico As New DataTable
        Dim DT_Anno As DataTable
        Dim DT_Impianti As DataTable = Nothing
        Dim dr_search As DataRow()
        Dim Dettaglio_Key As String = ""
        Dim TipoOperazioneDB As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
        '------------------------------

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessioneLocale, flagTransazioneLocale)

            'Lettura di tutti i dettagli della testata           
            dt = ObjLeggi_Dettaglio.Leggi(Piva, Id_Testata, Conto_Cod, 0, "", objParametri)

            If dt.Rows.Count > 0 Then

                'Lettura di tutte le annualità della testata
                DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, Anno, objParametri)

                If DT_Anno.Rows.Count > 0 Then


                    'Lettura di tutti i dettagli specifici della testata           
                    DT_Dettaglio_Specifico = ObjLeggi_Dettaglio_Specifico.Leggi(Piva, Id_Testata, Conto_Cod, 0, "", objParametri)



                    'Scorro i dettagli per verificare il conto
                    For Each dr As DataRow In dt.Rows

                        Select Case dr.Item("Valutazione_Conto_Cod")

                            Case 30 'Ricavi Attività Caratteristica

                                'Ricerca di tutte le cultivar presenti nell'anno
                                DT_Impianti = ObjLeggi_Dettaglio_Specifico.LeggiImpianti(Piva, Id_Testata,
                                                                                         CInt(dr.Item("Anno")), objParametri)

                                If DT_Impianti.Rows.Count > 0 Then

                                    For Each dr_Impianto As DataRow In DT_Impianti.Rows

                                        TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                                        Dettaglio_Key = dr_Impianto("Veg_Cod") & "|" & dr_Impianto("Cul_Cod") & "|" & dr_Impianto("Regolamento") & "|0"

                                        If DT_Dettaglio_Specifico IsNot Nothing AndAlso DT_Dettaglio_Specifico.Rows.Count > 0 Then

                                            'Controllo se il dettaglio specifico è presente
                                            dr_search = DT_Dettaglio_Specifico.Select("Dettaglio_Key = '" & Dettaglio_Key & "' And " &
                                                                                       "Anno = " & CStr(dr.Item("Anno")))


                                            If dr_search.Length <> 0 Then
                                                TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                                            End If

                                        End If

                                        'Costruzione Oggetto
                                        objValutazione_Dettaglio_Specifico = New Valutazione_Dettaglio_Specifico

                                        objValutazione_Dettaglio_Specifico.TipoOperazioneDB = TipoOperazioneDB
                                        objValutazione_Dettaglio_Specifico.Piva = Piva
                                        objValutazione_Dettaglio_Specifico.Id_Testata = Id_Testata
                                        objValutazione_Dettaglio_Specifico.Valutazione_Conto_Cod = dr.Item("Valutazione_Conto_Cod")
                                        objValutazione_Dettaglio_Specifico.Anno = dr_Impianto("Anno")
                                        objValutazione_Dettaglio_Specifico.Dettaglio_Key = Dettaglio_Key
                                        objValutazione_Dettaglio_Specifico.Valore_Unitario = 0 'Tabella Metaschema
                                        objValutazione_Dettaglio_Specifico.Valore_Totale = 0
                                        objValutazione_Dettaglio_Specifico.Valore_Ha = CDec(dr_Impianto("Sup_Specie")) 'Update
                                        objValutazione_Dettaglio_Specifico.Valore_Peso = CDec(dr_Impianto("Media_Produzione")) 'Update

                                        Dummy = ScriviModifica_Dettaglio_Specifico(objValutazione_Dettaglio_Specifico, True, objParametri)

                                    Next

                                End If


                            Case Else

                                '.......


                        End Select

                    Next


                End If

            End If


            'Restituisco un valore Dummy
            xRisp = Dummy

            Utility.VerificaChiudiTransazione(objParametri, flagTransazioneLocale)
            '----------------------------------------------------------------------------

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri, flagTransazioneLocale)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            Utility.VerificaChiudiConnessione(objParametri, flagConnessioneLocale)

        End Try

        Return xRisp

    End Function


#Region "Esportazione Excel"

    Public Function creaReportExcel(ByVal Piva As String,
                                    ByVal Id_Testata As Integer,
                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                    ByRef aziendeVuote As List(Of String),
                                    ByRef erroreDataTable As String
                                    ) As List(Of String)

        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        'Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)

        'Dim path As String = "C:\GiasLAN\File_Temporanei\"

        'Lettura Report
        Dim Valutazioni As New List(Of Valutazione)
        Dim Valutazione As New Valutazione

        Dim ObjValutazione As New Valutazioni_R

        erroreDataTable = "["

        Dim listaXLSX As New List(Of String)

        Dim workbook = New XLWorkbook()

        Dim Riga As Integer = 0
        Dim RigaBase As Integer = 0
        Dim Colonna As Integer = 0
        Dim Colonna_End As Integer = 3
        Dim xFiltroAggiuntivo As String = ""
        Dim ObjLeggi_TestataxAnno As New Valutazione_TestataxAnno_R
        Dim DT_Anno As DataTable = Nothing
        Dim DT_Impresa As DataTable = Nothing
        Dim myKey As String = ""
        Dim myKeyConto As String = ""
        Dim myKeySezione As String = ""
        Dim myKeyGruppo As String = ""
        Dim myKeyTotale As String = ""

        Dim Tipo_Des As String = ""
        Dim Rag_Soc_Path As String

        Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim LeggiImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim path As String =
                    LeggiCFGDatiIniziali.Leggi_Valore(0, "PathFileTemporanei", "", "", objParametri_Server)
        If path = "" Then
            Throw New Exception("Configurazione file temporanei non trovata. impossibile proseguire")
        End If

        'Chiusura Path
        If Right(path, 1) <> "\" Then
            path = path & "\"
        End If


        'Lettura Azienda
        DT_Impresa = LeggiImpresa.Leggi(Piva, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        Rag_Soc_Path = Trim(Left(Replace(DT_Impresa(0)("Rag_Soc"), ".", "") & Space(20), 20))

        'Lettura Annate x Testata        
        DT_Anno = ObjLeggi_TestataxAnno.LeggixAnno(Piva, Id_Testata, 0, 0, objParametri_Server)

        If DT_Anno.Rows.Count > 0 Then

            Colonna_End = Colonna_End + DT_Anno.Rows.Count

            Dim htAnno As New Hashtable
            Dim htConto As New Hashtable
            Dim htSezione As New Hashtable
            Dim htGruppo As New Hashtable
            Dim htTotale As New Hashtable


            For Tipo = 1 To 2

                Select Case Tipo
                    Case 1
                        Tipo_Des = "SITUAZIONE PATRIMONIALE"
                    Case 2
                        Tipo_Des = "CONTO ECONOMICO"
                End Select

                Dim nomeFoglio = Tipo_Des
                workbook.Worksheets.Add(nomeFoglio)
                Dim worksheet = workbook.Worksheet(nomeFoglio)


                '=================================================================================================================================
                'Intestazione
                '---------------------------------------------------------------------------------------------------------------------------------
                Dim RigaIntestazioneBase As Integer = 1
                Dim ColonnaIntestazioneBase As Integer = 2
                RigaBase = 3

                worksheet.Cell(RigaIntestazioneBase, ColonnaIntestazioneBase).SetValue(Tipo_Des & " Dichiarato dal Cliente")
                worksheet.Cell(RigaIntestazioneBase, ColonnaIntestazioneBase).Style.Font.Bold = True
                worksheet.Column(ColonnaIntestazioneBase).Width = 60

                applicaStile(worksheet, RigaIntestazioneBase, ColonnaIntestazioneBase, "green", XLBorderStyleValues.Thick, True, True, True, False)

                For Colonna = ColonnaIntestazioneBase + 1 To Colonna_End - 1
                    applicaStile(worksheet, RigaIntestazioneBase, Colonna, "green", XLBorderStyleValues.Thick, True, True, False, False)
                Next
                applicaStile(worksheet, RigaIntestazioneBase, Colonna_End, "green", XLBorderStyleValues.Thick, True, True, False, True)
                '=================================================================================================================================

                For Attivo_Passivo = 1 To 2

                    '=================================================================================================================================
                    'Attivo-Passivo
                    '---------------------------------------------------------------------------------------------------------------------------------                

                    Dim ColonnaAttivoPassivoBase As Integer = 2
                    Dim ColonnaAnnoBase As Integer = 4
                    Dim Attivo_Passivo_Des As String = ""

                    Select Case Tipo

                        Case 1

                            Select Case Attivo_Passivo
                                Case 1
                                    Attivo_Passivo_Des = "ATTIVO"
                                Case 2
                                    Attivo_Passivo_Des = "PASSIVO"
                            End Select

                        Case 2

                            Select Case Attivo_Passivo
                                Case 1
                                    Attivo_Passivo_Des = "RICAVI"
                                Case 2
                                    Attivo_Passivo_Des = "COSTI"
                            End Select


                    End Select


                    worksheet.Cell(RigaBase, ColonnaAttivoPassivoBase).SetValue(Attivo_Passivo_Des)
                    worksheet.Cell(RigaBase, ColonnaAttivoPassivoBase).Style.Font.Bold = True
                    applicaStile(worksheet, RigaBase, ColonnaAttivoPassivoBase, "orange", XLBorderStyleValues.Thick, True, True, True, False)

                    'Anno
                    worksheet.Cell(RigaBase, ColonnaAttivoPassivoBase + 1).SetValue("Anno")
                    worksheet.Cell(RigaBase, ColonnaAttivoPassivoBase + 1).Style.Font.Italic = True
                    applicaStile(worksheet, RigaBase, ColonnaAttivoPassivoBase + 1, "white", XLBorderStyleValues.Thin, True, True, True, True)





                    For Each dr In DT_Anno.Rows

                        worksheet.Cell(RigaBase, ColonnaAnnoBase).SetValue(dr("Anno"))
                        worksheet.Cell(RigaBase, ColonnaAnnoBase).Style.Font.Bold = True
                        applicaStile(worksheet, RigaBase, ColonnaAnnoBase, "orange", XLBorderStyleValues.Thick, True, True, True, True)

                        'Euro/1000
                        worksheet.Cell(RigaBase + 1, ColonnaAnnoBase).SetValue("Euro/1000")
                        worksheet.Cell(RigaBase + 1, ColonnaAnnoBase).Style.Font.Italic = True

                        'Salvo la Colonna Anno
                        myKey = dr("Anno")

                        If Tipo = 1 AndAlso Attivo_Passivo = 1 Then

                            Dim myObj As New ObjAnnoDaInserire
                            myObj.Totale_Sezione = 0
                            myObj.Totale_Gruppo = 0
                            myObj.Totale_Gruppo_Padre = 0
                            myObj.Totale = 0
                            myObj.Colonna = ColonnaAnnoBase
                            myObj.MOL = 0
                            myObj.MON = 0

                            If Not htAnno.ContainsKey(myKey) Then
                                htAnno.Add(myKey, myObj)
                            End If

                        Else

                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione = 0
                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo = 0
                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo_Padre = 0
                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale = 0
                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna = ColonnaAnnoBase


                        End If

                        ColonnaAnnoBase = ColonnaAnnoBase + 1

                    Next


                    RigaBase = RigaBase + 2

                    '=================================================================================================================================
                    'Lettura Dettagli
                    '---------------------------------------------------------------------------------------------------------------------------------
                    xFiltroAggiuntivo = "Pat_Eco = " & Tipo & " And Attivo_Passivo = " & Attivo_Passivo

                    Valutazioni = ObjValutazione.Leggi_Valutazione(Piva, Id_Testata, 0, "", 0, False, objParametri_Server, xFiltroAggiuntivo)

                    If Valutazioni.Count > 0 Then


                        Dim Valutazione_Count As Integer = 1
                        Dim htDettaglio As New Hashtable
                        Dim Colonna_Conto As Integer = 2
                        Dim iDettaglio As Integer = 0

                        Dim bBorderTop As Boolean = True
                        Dim bBorderBottom As Boolean = False


                        For Each Valutazione In Valutazioni

                            '===============================================================================================
                            'Controllo Inserimento Sezioni, Gruppi e Totali
                            '-----------------------------------------------------------------------------------------------
                            Dim bInsert_Sezione As Boolean = False

                            If iDettaglio < Valutazioni.Count - 1 Then
                                If Valutazione.Valutazione_Sezione_Cod <> Valutazioni(iDettaglio + 1).Valutazione_Sezione_Cod Then
                                    bInsert_Sezione = True
                                End If
                            Else
                                bInsert_Sezione = True
                            End If

                            Dim bInsert_Gruppo As Boolean = False

                            If iDettaglio < Valutazioni.Count - 1 Then
                                If Valutazione.Valutazione_Gruppo_Cod <> Valutazioni(iDettaglio + 1).Valutazione_Gruppo_Cod AndAlso
                                    Valutazioni(iDettaglio).Valutazione_Gruppo_Cod <> 0 Then
                                    bInsert_Gruppo = True
                                End If
                            Else
                                If Valutazione.Valutazione_Gruppo_Cod <> 0 Then
                                    bInsert_Gruppo = True
                                End If

                            End If

                            Dim bInsert_Gruppo_Padre As Boolean = False

                            If iDettaglio < Valutazioni.Count - 1 Then
                                If Valutazione.Valutazione_Gruppo_Padre <> Valutazioni(iDettaglio + 1).Valutazione_Gruppo_Padre AndAlso
                                    Valutazioni(iDettaglio).Valutazione_Gruppo_Padre <> 0 Then
                                    bInsert_Gruppo_Padre = True
                                End If
                            Else
                                If Valutazione.Valutazione_Gruppo_Padre <> 0 Then
                                    bInsert_Gruppo_Padre = True
                                End If

                            End If

                            Dim bInsert_Totale As Boolean = False

                            If iDettaglio = Valutazioni.Count - 1 Then
                                bInsert_Totale = True
                            End If
                            '===============================================================================================

                            myKey = Valutazione.Valutazione_Conto_Cod

                            If Not htDettaglio.ContainsKey(myKey) Then

                                htDettaglio.Add(myKey, RigaBase)

                                'Inserimento Dettaglio                                
                                worksheet.Cell(RigaBase, 2).SetValue(Valutazione.Valutazione_Conto_Des)

                                bBorderBottom = bInsert_Sezione

                                applicaStile(worksheet, RigaBase, 2, "white", XLBorderStyleValues.Thick, bBorderTop, bBorderBottom, True, True)

                                Riga = RigaBase
                                RigaBase = RigaBase + 1
                                Valutazione_Count = Valutazione_Count + 1

                            Else
                                Riga = htDettaglio(myKey)
                            End If


                            myKey = Valutazione.Anno

                            If htAnno.ContainsKey(myKey) Then

                                Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna

                                'Controllo Inversione Segno per Conti Particolari (al momento non c'è tempo per la parametrizzazione ed avviene quindi qui da codice)
                                Dim Valore_Segno As Decimal = 0

                                Select Case Valutazione.Valutazione_Conto_Cod
                                    Case 47 'Proventi Finanaziati
                                        Valore_Segno = -Valutazione.Valore
                                    Case Else
                                        Valore_Segno = Valutazione.Valore
                                End Select


                                worksheet.Cell(Riga, Colonna).SetValue(Valutazione.Valore / 1000)
                                worksheet.Cell(Riga, Colonna).Style.NumberFormat.Format = "0.000"

                                'Aggiornamento Valori in caso di non sottocontoincluso (di cui) 
                                If Valutazione.ChkBypassValore <> 1 Then

                                    DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione + (Valore_Segno / 1000)
                                    DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo + (Valore_Segno / 1000)
                                    DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo_Padre = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo_Padre + (Valore_Segno / 1000)
                                    DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale + (Valore_Segno / 1000)

                                    '=========================================================================================================================================
                                    'Salvo il totale conto
                                    '-----------------------------------------------------------------------------------------------------------------------------------------
                                    myKeyConto = Valutazione.Anno & "_" & Valutazione.Valutazione_Conto_Cod

                                    Dim myObjConto As New ObjTotaleDaInserire
                                    myObjConto.Totale = Valutazione.Valore / 1000

                                    If Not htConto.ContainsKey(myKeyConto) Then
                                        htConto.Add(myKeyConto, myObjConto)
                                    Else
                                        DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale = myObjConto.Totale
                                    End If
                                    '=========================================================================================================================================

                                    'MOL e MON
                                    If Valutazione.Valutazione_Sezione_Cod = 10 Then
                                        If Not IsNumeric(DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL) Then
                                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL = 0
                                        End If
                                        If Not IsNumeric(DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON) Then
                                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = 0
                                        End If
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL + (Valutazione.Valore / 1000)
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON + (Valutazione.Valore / 1000)
                                    End If
                                    If Valutazione.Valutazione_Gruppo_Cod = 7 Then
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL - (Valutazione.Valore / 1000)
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON - (Valutazione.Valore / 1000)
                                    End If

                                    If Valutazione.Valutazione_Sezione_Cod = 13 Then
                                        If Not IsNumeric(DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON) Then
                                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = 0
                                        End If
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON - (Valutazione.Valore / 1000)
                                    End If

                                Else

                                    '=========================================================================================================================================
                                    'Salvo il totale conto
                                    '-----------------------------------------------------------------------------------------------------------------------------------------
                                    myKeyConto = Valutazione.Anno & "_" & Valutazione.Valutazione_Conto_Cod

                                    Dim myObjConto As New ObjTotaleDaInserire
                                    myObjConto.Totale = Valutazione.Valore / 1000

                                    If Not htConto.ContainsKey(myKeyConto) Then
                                        htConto.Add(myKeyConto, myObjConto)
                                    Else
                                        DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale = myObjConto.Totale
                                    End If
                                    '=========================================================================================================================================



                                End If

                                'Mantengo una copia per salvare il totale dei ricavi
                                If Tipo = 2 AndAlso Attivo_Passivo = 1 Then
                                    If Not IsNumeric(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi) Then
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi = 0
                                    End If
                                    If Valutazione.ChkBypassValore <> 1 Then
                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi + (Valutazione.Valore / 1000)
                                    End If

                                End If

                                applicaStile(worksheet, Riga, Colonna, "white", XLBorderStyleValues.Thick, True, True, True, True)

                            End If


                            bBorderTop = False

                            iDettaglio = iDettaglio + 1


                            If bInsert_Sezione Then

                                bBorderTop = True

                                'Riga Vuota
                                applicaStile(worksheet, RigaBase, 2, "white", XLBorderStyleValues.Thick, False, False, True, True)
                                RigaBase = RigaBase + 1

                                If Valutazione.Valutazione_Sezione_Invisibile = 0 Then

                                    '==============================================================================================================
                                    'Inserimento Sezione
                                    '--------------------------------------------------------------------------------------------------------------
                                    worksheet.Cell(RigaBase, Colonna_Conto).SetValue("Totale " & Valutazione.Valutazione_Sezione_Des)
                                    applicaStile(worksheet, RigaBase, Colonna_Conto, "green", XLBorderStyleValues.Thick, True, True, True, True)

                                    For Each dr In DT_Anno.Rows

                                        myKey = dr("Anno")

                                        If htAnno.ContainsKey(myKey) Then

                                            Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                            worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione)
                                            worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"


                                            '=========================================================================================================================================
                                            'Salvo il totale sezione per rendiconto finanziario
                                            '-----------------------------------------------------------------------------------------------------------------------------------------
                                            myKeySezione = myKey & "_" & Valutazione.Valutazione_Sezione_Cod

                                            Dim myObjSezione As New ObjTotaleDaInserire
                                            myObjSezione.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione

                                            If Not htSezione.ContainsKey(myKeySezione) Then
                                                htSezione.Add(myKeySezione, myObjSezione)
                                            Else
                                                DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale = myObjSezione.Totale
                                            End If
                                            '=========================================================================================================================================



                                            DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Sezione = 0

                                            applicaStile(worksheet, RigaBase, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)

                                        End If



                                    Next

                                    RigaBase = RigaBase + 2
                                    bBorderTop = True

                                End If

                            End If


                            If bInsert_Gruppo Then
                                '==============================================================================================================
                                'Inserimento Gruppo
                                '--------------------------------------------------------------------------------------------------------------                                
                                worksheet.Cell(RigaBase, Colonna_Conto).SetValue("Totale " & Valutazione.Valutazione_Gruppo_Des)
                                applicaStile(worksheet, RigaBase, Colonna_Conto, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                For Each dr In DT_Anno.Rows

                                    myKey = dr("Anno")

                                    If htAnno.ContainsKey(myKey) Then

                                        Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                        worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo)
                                        worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"



                                        '=========================================================================================================================================
                                        'Salvo il totale gruppo per rendiconto finanziario
                                        '-----------------------------------------------------------------------------------------------------------------------------------------
                                        myKeyGruppo = myKey & "_" & Valutazione.Valutazione_Gruppo_Cod

                                        Dim myObjGruppo As New ObjTotaleDaInserire
                                        myObjGruppo.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo

                                        If Not htGruppo.ContainsKey(myKeyGruppo) Then
                                            htGruppo.Add(myKeyGruppo, myObjGruppo)
                                        Else
                                            DirectCast(htGruppo(myKeyGruppo), ObjTotaleDaInserire).Totale = myObjGruppo.Totale
                                        End If
                                        '=========================================================================================================================================

                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo = 0

                                        applicaStile(worksheet, RigaBase, Colonna, "yellow", XLBorderStyleValues.Thick, True, True, True, True)


                                    End If

                                Next

                                RigaBase = RigaBase + 2


                            End If


                            If bInsert_Gruppo_Padre Then
                                '==============================================================================================================
                                'Inserimento Gruppo
                                '--------------------------------------------------------------------------------------------------------------
                                worksheet.Cell(RigaBase, Colonna_Conto).SetValue("Totale " & Valutazione.Valutazione_Gruppo_Padre_Des)
                                applicaStile(worksheet, RigaBase, Colonna_Conto, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                For Each dr In DT_Anno.Rows

                                    myKey = dr("Anno")

                                    If htAnno.ContainsKey(myKey) Then

                                        Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                        worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo_Padre)
                                        worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"

                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Gruppo_Padre = 0

                                        applicaStile(worksheet, RigaBase, Colonna, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                    End If



                                Next

                                RigaBase = RigaBase + 2


                            End If


                            'In caso di Costi della Produzione --> inserisco il MOL (Margine Operativo Lordo)
                            If bInsert_Gruppo And Valutazione.Valutazione_Gruppo_Cod = 7 Then

                                '==============================================================================================================
                                'Inserimento MOL = Totali ricavi attività agricole e complementari - costi della produzione
                                '--------------------------------------------------------------------------------------------------------------
                                worksheet.Cell(RigaBase, Colonna_Conto).SetValue("M.O.L. - margine operativo lordo")
                                applicaStile(worksheet, RigaBase, Colonna_Conto, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                For Each dr In DT_Anno.Rows

                                    myKey = dr("Anno")

                                    If htAnno.ContainsKey(myKey) Then

                                        Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                        worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL)
                                        worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"


                                        '=========================================================================================================================================
                                        'Salvo il MOL per Indici
                                        '-----------------------------------------------------------------------------------------------------------------------------------------
                                        myKeyConto = myKey & "_MOL"

                                        Dim myObjConto As New ObjTotaleDaInserire
                                        myObjConto.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL

                                        If Not htConto.ContainsKey(myKeyConto) Then
                                            htConto.Add(myKeyConto, myObjConto)
                                        Else
                                            DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale = myObjConto.Totale
                                        End If
                                        '=========================================================================================================================================



                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MOL = 0
                                        applicaStile(worksheet, RigaBase, Colonna, "yellow", XLBorderStyleValues.Thick, True, True, True, True)






                                    End If



                                Next

                                RigaBase = RigaBase + 2

                            End If



                            'In caso di Ammortamenti --> inserisco il MON (Margine Operativo Netto)
                            If bInsert_Sezione And Valutazione.Valutazione_Sezione_Cod = 13 Then

                                '==============================================================================================================
                                'Inserimento MON = MOL - Ammortamenti e Svalutazioni
                                '--------------------------------------------------------------------------------------------------------------
                                worksheet.Cell(RigaBase, Colonna_Conto).SetValue("M.O.N. - margine operativo netto")
                                applicaStile(worksheet, RigaBase, Colonna_Conto, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                For Each dr In DT_Anno.Rows

                                    myKey = dr("Anno")

                                    If htAnno.ContainsKey(myKey) Then

                                        Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                        worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON)
                                        worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"


                                        '=========================================================================================================================================
                                        'Salvo il MON per Indici
                                        '-----------------------------------------------------------------------------------------------------------------------------------------
                                        myKeyConto = myKey & "_MON"

                                        Dim myObjConto As New ObjTotaleDaInserire
                                        myObjConto.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON

                                        If Not htConto.ContainsKey(myKeyConto) Then
                                            htConto.Add(myKeyConto, myObjConto)
                                        Else
                                            DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale = myObjConto.Totale
                                        End If
                                        '=========================================================================================================================================


                                        DirectCast(htAnno(myKey), ObjAnnoDaInserire).MON = 0

                                        applicaStile(worksheet, RigaBase, Colonna, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                                    End If



                                Next

                                RigaBase = RigaBase + 2

                            End If





                            If bInsert_Totale Then

                                Select Case Tipo

                                    Case 1 'Stato Patrimoniale

                                        '=======================================================================================
                                        'Inserimento Totale Attivo_Passivo
                                        '--------------------------------------------------------------------------------------------------------------
                                        worksheet.Cell(RigaBase, Colonna_Conto).SetValue("Totale " & Attivo_Passivo_Des)
                                        applicaStile(worksheet, RigaBase, Colonna_Conto, "orange", XLBorderStyleValues.Thick, True, True, True, False)
                                        applicaStile(worksheet, RigaBase, Colonna_Conto + 1, "orange", XLBorderStyleValues.Thick, True, True, False, True)

                                        For Each dr In DT_Anno.Rows

                                            myKey = dr("Anno")

                                            If htAnno.ContainsKey(myKey) Then

                                                Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                                worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale)
                                                worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"
                                                applicaStile(worksheet, RigaBase, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)

                                                '=========================================================================================================================================
                                                'Salvo il totale attivo/passivo per indici
                                                '-----------------------------------------------------------------------------------------------------------------------------------------
                                                myKeyTotale = myKey & "_" & Attivo_Passivo

                                                Dim myObjTotale As New ObjTotaleDaInserire
                                                myObjTotale.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale

                                                If Not htTotale.ContainsKey(myKeyTotale) Then
                                                    htTotale.Add(myKeyTotale, myObjTotale)
                                                Else
                                                    DirectCast(htTotale(myKeyTotale), ObjTotaleDaInserire).Totale = myObjTotale.Totale
                                                End If
                                                '=========================================================================================================================================


                                            End If

                                        Next


                                    Case 2 'Conto Economico

                                        Select Case Attivo_Passivo

                                            Case 1

                                                DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale = 0

                                            Case 2

                                                '=======================================================================================
                                                'Inserimento UTILE ESERCIZIO
                                                '--------------------------------------------------------------------------------------------------------------
                                                worksheet.Cell(RigaBase, Colonna_Conto).SetValue("UTILE ESERCIZIO")
                                                applicaStile(worksheet, RigaBase, Colonna_Conto, "orange", XLBorderStyleValues.Thick, True, True, True, False)
                                                applicaStile(worksheet, RigaBase, Colonna_Conto + 1, "orange", XLBorderStyleValues.Thick, True, True, False, True)

                                                For Each dr In DT_Anno.Rows

                                                    myKey = dr("Anno")

                                                    If htAnno.ContainsKey(myKey) Then

                                                        Colonna = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Colonna
                                                        worksheet.Cell(RigaBase, Colonna).SetValue(DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi - DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale)
                                                        worksheet.Cell(RigaBase, Colonna).Style.NumberFormat.Format = "0.000"
                                                        applicaStile(worksheet, RigaBase, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)


                                                        '=========================================================================================================================================
                                                        'Salvo il totale Utile
                                                        '-----------------------------------------------------------------------------------------------------------------------------------------
                                                        myKeyConto = myKey & "_UTILE"

                                                        Dim myObjConto As New ObjTotaleDaInserire
                                                        myObjConto.Totale = DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale_Ricavi - DirectCast(htAnno(myKey), ObjAnnoDaInserire).Totale

                                                        If Not htConto.ContainsKey(myKeyConto) Then
                                                            htConto.Add(myKeyConto, myObjConto)
                                                        Else
                                                            DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale = myObjConto.Totale
                                                        End If
                                                        '=========================================================================================================================================

                                                    End If

                                                Next

                                        End Select


                                End Select

                                '====================================================================================================

                            End If

                        Next

                    End If

                    RigaBase = RigaBase + 4

                Next

            Next




            '############################################################################################################################################
            '##############################################  RENDICONTO FINANZIARIO #####################################################################
            '############################################################################################################################################

            Tipo_Des = "RENDICONTO FINANZIARIO"
            Dim nomeFoglioR = Tipo_Des
            workbook.Worksheets.Add(nomeFoglioR)
            Dim worksheetR = workbook.Worksheet(nomeFoglioR)

            '=================================================================================================================================
            'Intestazione
            '---------------------------------------------------------------------------------------------------------------------------------
            Dim RigaIntestazioneBaseR As Integer = 1
            Dim ColonnaIntestazioneBaseR As Integer = 2
            Colonna_End = Colonna_End - 1

            worksheetR.Cell(RigaIntestazioneBaseR, ColonnaIntestazioneBaseR).SetValue("RENDICONTO FINANZIARIO (METODO IND.)")
            worksheetR.Cell(RigaIntestazioneBaseR, ColonnaIntestazioneBaseR).Style.Font.Bold = True
            worksheetR.Column(ColonnaIntestazioneBaseR).Width = 60

            applicaStile(worksheetR, RigaIntestazioneBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, False)

            For Colonna = ColonnaIntestazioneBaseR + 1 To Colonna_End
                applicaStile(worksheetR, RigaIntestazioneBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, False, False)
            Next
            'applicaStile(worksheetR, RigaIntestazioneBaseR, Colonna_End, "green", XLBorderStyleValues.Thick, True, True, False, True)

            Dim bFirst As Boolean = True
            Dim RigaBaseR As Integer = RigaIntestazioneBaseR
            Dim ColonnaBaseR = 2

            For Each dr In DT_Anno.Rows

                ColonnaBaseR = ColonnaBaseR + 1
                RigaBaseR = RigaIntestazioneBaseR

                worksheetR.Cell(RigaIntestazioneBaseR, ColonnaBaseR).SetValue(Space(15) & dr("Anno"))
                worksheetR.Cell(RigaIntestazioneBaseR, ColonnaBaseR).Style.Font.Bold = True
                applicaStile(worksheetR, RigaIntestazioneBaseR, ColonnaBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)

                'Impostazione Totale Attivo
                Dim Totale_Attivo As Decimal = 0
                myKeyTotale = dr("Anno") & "_1"
                If htTotale.ContainsKey(myKeyTotale) Then
                    Totale_Attivo = DirectCast(htTotale(myKeyTotale), ObjTotaleDaInserire).Totale
                End If


                '============================================================================================================================================
                'Riga Vuota
                RigaBaseR = RigaBaseR + 1

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(Space(10) & "Euro/1000")
                worksheetR.Cell(RigaBaseR, ColonnaBaseR).Style.Font.Italic = True
                applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)



                '============================================================================================================================================
                'A) Flussi finanziari att. operativa
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("A) Flussi finanziari att. operativa")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                End If
                'Colorazione altre celle
                For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                    applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                Next

                'Utile (perdita) esercizio
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Utile (perdita) esercizio")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                Dim Totale As Decimal = 0
                Dim Totale1 As Decimal = 0
                Dim Totale2 As Decimal = 0
                Dim Totale3 As Decimal = 0
                Dim Totale6 As Decimal = 0
                Dim Parziale As Decimal = 0

                If Not bFirst And Totale_Attivo <> 0 Then

                    myKeyConto = dr("Anno") & "_UTILE"
                    If htConto.ContainsKey(myKeyConto) Then
                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale))
                        'worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                        Totale = Totale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                End If


                'Imposte sul Reddito
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Imposte sul Reddito")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    myKeyConto = dr("Anno") & "_49"
                    If htConto.ContainsKey(myKeyConto) Then
                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                        Totale = Totale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                End If


                'Interessi passivi/(attivi)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Interessi passivi/(attivi)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                'Oneri Finanaziari (48) - Proventi Finanziari (47)
                If Not bFirst And Totale_Attivo <> 0 Then
                    Parziale = 0
                    myKeyConto = dr("Anno") & "_48"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    myKeyConto = dr("Anno") & "_47"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If

                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    Totale = Totale + Parziale
                End If

                '1) Utile ante imposte, interessi, dividendi
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("1) Utile ante imposte, interessi, dividendi")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next

                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                    Totale1 = Totale
                End If

                'Accantonamento a fondi
                RigaBaseR = RigaBaseR + 1
                Totale = 0
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Accantonamento a fondi")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                '57+46+45
                If Not bFirst And Totale_Attivo <> 0 Then
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_57"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_46"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_45"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                    End If

                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    Totale = Totale + Parziale
                End If



                'Ammortamenti immobilizzazioni
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Ammortamenti immobilizzazioni")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                '55+56
                If Not bFirst And Totale_Attivo <> 0 Then
                    Parziale = 0
                    myKeyConto = dr("Anno") & "_55"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    myKeyConto = dr("Anno") & "_56"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If

                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    Totale = Totale + Parziale
                End If


                'Tot.rett.elementi monetari senza contropartita nel capitale circolante netto
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Tot.rett.elementi monetari senza contropartita nel capitale circolante netto")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                '2) Flusso finanziario prima delle variazioni del capitale circolante netto
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("2) Flusso finanziario prima delle variazioni del capitale circolante netto")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    Totale2 = Totale1 + Totale
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale2))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                Totale = 0

                'Decremento/ (Incremento) rimanenze
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Decremento/ (Incremento) rimanenze")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    '4+5+6 rapportato all'anno precedente
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_4"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_5"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_6"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        myKeyConto = dr("Anno") - 1 & "_4"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_5"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_6"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If


                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        Totale = Totale + Parziale

                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    End If
                End If




                'Decremento/ (Incremento) crediti v/clienti
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Decremento/ (Incremento) crediti v/clienti")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'crediti (2) rapportato con anno precedente + svalutazioni (46) anno in corso
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_2"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_46"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_2"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If


                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale

                    End If

                End If



                'Incremento/(Decremento) debiti v/fornitori
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Incremento/(Decremento) debiti v/fornitori")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'debiti vs fonitori (19) rapportato con anno precedente 
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_19"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_19"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If


                'Altri decrementi / (Altri incrementi) cap. circ. netto
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Altri decrementi / (Altri incrementi) cap. circ. netto")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Altri debiti (20) rapportato con anno precedente - Altri crediti (IVA, ecc.) (3) rapportato con anno precedente
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_20"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_20"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_3"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_3"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If


                'Totale variazioni del capitale circolante netto
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Totale variazioni del capitale circolante netto")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                '3) Flusso finanziario dopo le variazioni del capitale circolante netto
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("3) Flusso finanziario dopo le variazioni del capitale circolante netto")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    Totale3 = Totale2 + Totale
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale3))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                End If



                Totale = 0

                'Interessi incassati / (pagati)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Interessi incassati / (pagati)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                'Proventi (47) - Oneri (48)
                If Not bFirst And Totale_Attivo <> 0 Then
                    Parziale = 0
                    myKeyConto = dr("Anno") & "_47"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    myKeyConto = dr("Anno") & "_48"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If

                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    Totale = Totale + Parziale
                End If


                '(Imposte sul reddito pagate)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("(Imposte sul reddito pagate)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                'Imposte e tasse (49)
                If Not bFirst And Totale_Attivo <> 0 Then
                    Parziale = 0
                    myKeyConto = dr("Anno") & "_49"
                    If htConto.ContainsKey(myKeyConto) Then
                        Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If

                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                    Totale = Totale + Parziale

                End If




                '(Utilizzo dei fondi)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("(Utilizzo dei fondi)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    '21  FondiTFR (2 anni) - 57 (Spese annuali per il personale - di cui TFR) + 22 altri fondi e rischi (2 anni) - accontanemti 45
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_21"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_21"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_57"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_22"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_22"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_45"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale

                    End If
                End If


                'Totale altre rettifiche
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Totale altre rettifiche")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If



                'Flusso finanziario dell'attività operativa (A)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Flusso finanziario dell'attività operativa (A)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                Dim Flusso_Finanziario_AO As Decimal = Totale3 + Totale
                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Flusso_Finanziario_AO))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                Totale = 0

                'Riga Vuota
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("")
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                '============================================================================================================================================
                'B) Flussi finanziari attività investimento
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("B) Flussi finanziari attività investimento")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thin, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next

                End If



                '(Investimenti)/Disinvestimenti immob.materiali
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("(Investimenti)/Disinvestimenti immob.materiali")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Imm.Tecniche(sezione 2) rapportato in 2 anni + ammortamenti materiali (56)
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeySezione = dr("Anno") & "_2"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale - DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeySezione = dr("Anno") - 1 & "_2"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale + DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_56"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If

                '(Investimenti)/Disinvestimenti immob.immateriali
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("((Investimenti)/Disinvestimenti immob.immateriali")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Imm.Immatieriali(sezione 4) rapportato in2 anni + ammortamenti immateriali (55)
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeySezione = dr("Anno") & "_4"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale - DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeySezione = dr("Anno") - 1 & "_4"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale + DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_55"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If


                '(Investimenti)/Disinvestimenti immob.finanziarie
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("(Investimenti)/Disinvestimenti immob.finanziarie")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Imm.finanziarie(sezione 5) rapportato in 2 anni
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeySezione = dr("Anno") & "_5"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale - DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeySezione = dr("Anno") - 1 & "_5"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale + DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If

                'Flusso finanziario dell'attività d'investimento (B)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Flusso finanziario dell'attività d'investimento (B)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                Dim Totale5 As Decimal = Totale
                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale5))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                Totale = 0


                'C) Flussi finanziari attività finanziamento
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("C) Flussi finanziari attività finanziamento")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thin, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If




                'Var.debiti v / banche a bt
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Var.debiti v / banche a bt")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Debiti bancari agrari (17) rapp +  Altri debiti bancari (18) rapp.
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_17"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_17"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_18"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_18"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If





                'Var.debiti v / banche a mlt
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Var.debiti v / banche a mlt")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Finanziamenti bancari a m-l termine (23) rapp
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_23"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_23"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If




                'Var.debiti finanziari v/terzi a bt
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Var.debiti finanziari v/terzi a bt")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then

                    'Altri finanziamenti a breve termine (51)  rapp.
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_51"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_51"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If



                'Var. debiti finanziari vterzi a mlt
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Var. debiti finanziari vterzi a mlt")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then

                    ' Altri finanziamenti a m-l termine (24) rapp.
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_24"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") - 1 & "_24"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.Font.Bold = True
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale

                    End If
                End If





                'Increm.mezzi propri / (Dividendi o riserve distrib.)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Increm.mezzi propri / (Dividendi o riserve distrib.)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Mezzi Propri(sezione 9) rapportato in 2 anni - Risultato d'esercizio (27)
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeySezione = dr("Anno") & "_9"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale + DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeySezione = dr("Anno") - 1 & "_9"
                        If htSezione.ContainsKey(myKeySezione) Then
                            Parziale = Parziale - DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_27"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale - DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If


                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If



                'Flusso finanziario dell'attività di finanziamento (C)
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Flusso finanziario dell'attività di finanziamento (C)")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "orange", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If



                'Increm./(Decrem.) delle disponibilità liquide
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Increm./(Decrem.) delle disponibilità liquide")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    Totale6 = Flusso_Finanziario_AO + Totale5 + Totale
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale6))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                End If


                Totale = 0

                '============================================================================================================================================
                'VERIFICA
                '--------------------------------------------------------------------------------------------------------------------------------------------
                'Riga Vuota
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("")
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("VERIFICA")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Bold = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "yellow", XLBorderStyleValues.Thick, True, True, True, True)

                End If

                '============================================================================================================================================


                'Depositi bancari / cassa inizio esercizio
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Depositi bancari / cassa inizio esercizio")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Liquidità Anno -1
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") - 1 & "_1"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale - Parziale
                    End If

                End If

                'Depositi bancari / cassa fine esercizio
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Depositi bancari / cassa fine esercizio")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If


                If Not bFirst And Totale_Attivo <> 0 Then
                    'Liquidità 
                    Parziale = 0
                    If Totale_Attivo > 0 Then
                        myKeyConto = dr("Anno") & "_1"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Parziale))
                        worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                        applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)

                        Totale = Totale + Parziale
                    End If

                End If


                'Var.depositi bancari / cassa
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Var. depositi bancari / cassa")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, True, True, False)
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneRendiconto(Totale))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.Font.Bold = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If


                '============================================================================================================================================
                'Verifica
                '--------------------------------------------------------------------------------------------------------------------------------------------
                'Riga Vuota
                RigaBaseR = RigaBaseR + 1
                Dim Verifica As Decimal = Totale6 - Totale
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("")
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(Space(15) & IIf(Math.Abs(Verifica) < 10, "VERO", "FALSO"))
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).Style.Font.Bold = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                    'Colorazione altre celle
                    For Colonna = ColonnaIntestazioneBaseR + 1 To Colonna_End
                        applicaStile(worksheetR, RigaBaseR, Colonna, "green", XLBorderStyleValues.Thick, True, True, True, True)
                    Next
                End If
                '============================================================================================================================================

                Totale = 0


                '############################################################################################################################################
                '#######################################################  INDICI  ###########################################################################
                '############################################################################################################################################

                'Riga Vuota
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("")
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thin, True, True, True, True)
                End If

                'Indici
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("INDICI")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Bold = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "green", XLBorderStyleValues.Thick, True, True, True, True)
                End If



                'Qualità dei Ricavi
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Qualità dei Ricavi")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If

                'Flusso Finanziario Attività Operativa / (Ricavi Attività Carattetistica (30) + Premi Comunitari (31)))
                Dim Ricavi_Premi As Decimal = 0
                myKeyConto = dr("Anno") & "_30"
                    If htConto.ContainsKey(myKeyConto) Then
                        Ricavi_Premi = Ricavi_Premi + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    myKeyConto = dr("Anno") & "_31"
                    If htConto.ContainsKey(myKeyConto) Then
                        Ricavi_Premi = Ricavi_Premi + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    Dim Qualita_Ricavi As Decimal = 0
                    If Ricavi_Premi <> 0 Then
                        Qualita_Ricavi = Flusso_Finanziario_AO / (Ricavi_Premi)
                    End If
                If Not bFirst And Totale_Attivo <> 0 Then
                    worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Qualita_Ricavi))
                    worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                    applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)
                End If

                'Aliquota di circolante
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Aliquota di circolante")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If

                '(Totale Cap. Circ. Lordo (sez.1) - Deb. Breve Termine (sez.6) + Deb. ban. agrari (17) + Altri deb. ban (18) + Altri fin. bt (51)) / Ricavi_Premi
                Parziale = 0
                myKeySezione = dr("Anno") & "_1"
                If htSezione.ContainsKey(myKeySezione) Then
                    Parziale = Parziale + DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                End If
                myKeySezione = dr("Anno") & "_6"
                If htSezione.ContainsKey(myKeySezione) Then
                    Parziale = Parziale - DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale
                End If
                myKeyConto = dr("Anno") & "_17"
                If htConto.ContainsKey(myKeyConto) Then
                    Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                End If
                myKeyConto = dr("Anno") & "_18"
                If htConto.ContainsKey(myKeyConto) Then
                    Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                End If
                myKeyConto = dr("Anno") & "_51"
                If htConto.ContainsKey(myKeyConto) Then
                    Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                End If
                Dim Aliquota_Circolante As Decimal = 0
                If Ricavi_Premi <> 0 Then
                    Aliquota_Circolante = Parziale / (Ricavi_Premi)
                End If

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Aliquota_Circolante))
                worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)




                'Autonomia Finanziaria
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Autonomia Finanziaria")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If

                'Mezzi Propri (sez.9) / Totale Passivo
                Dim Mezzi_Propri As Decimal = 0
                myKeySezione = dr("Anno") & "_9"
                If htSezione.ContainsKey(myKeySezione) Then
                    myKeyTotale = dr("Anno") & "_2"
                    If htTotale.ContainsKey(myKeyTotale) Then
                        If CDec(DirectCast(htTotale(myKeyTotale), ObjTotaleDaInserire).Totale) > 0 Then
                            Mezzi_Propri = DirectCast(htSezione(myKeySezione), ObjTotaleDaInserire).Totale / DirectCast(htTotale(myKeyTotale), ObjTotaleDaInserire).Totale
                        End If
                    End If
                End If

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Mezzi_Propri))
                worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)





                'Costo del Debito
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Costo del Debito")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If

                'Oneri Finanziari (48) / (Deb. ban. agrari (17) + Altri deb. ban (18) + Altri fin. bt (51) + Fin. ban. mlt (23) + Altri fin. mlt (24)
                Dim Costo_Debito As Decimal = 0
                Dim Oneri_Finanziari As Decimal = 0
                Parziale = 0
                myKeyConto = dr("Anno") & "_48"
                If htConto.ContainsKey(myKeyConto) Then
                    Oneri_Finanziari = DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    If Oneri_Finanziari <> 0 Then
                        myKeyConto = dr("Anno") & "_17"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_18"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_51"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_23"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If
                        myKeyConto = dr("Anno") & "_24"
                        If htConto.ContainsKey(myKeyConto) Then
                            Parziale = Parziale + DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                        End If

                        If Parziale > 0 Then
                            Costo_Debito = Oneri_Finanziari / Parziale
                        End If

                    End If
                End If

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Costo_Debito))
                worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)





                'Oneri finanziari su M.O.L
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Oneri finanziari su M.O.L.")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, False, True, False)
                End If

                'Oneri Finanziari (48) / MOL
                Dim Oneri_MOL As Decimal = 0
                Dim MOL As Decimal = 0
                If Oneri_Finanziari <> 0 Then
                    myKeyConto = dr("Anno") & "_MOL"
                    If htConto.ContainsKey(myKeyConto) Then
                        MOL = DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    If MOL > 0 Then
                        Oneri_MOL = Oneri_Finanziari / MOL
                    End If
                End If

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Oneri_MOL))
                worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)



                'Oneri finanziari su M.O.N
                RigaBaseR = RigaBaseR + 1
                If bFirst Then
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).SetValue("Oneri finanziari su M.O.N.")
                    worksheetR.Cell(RigaBaseR, ColonnaIntestazioneBaseR).Style.Font.Italic = True
                    applicaStile(worksheetR, RigaBaseR, ColonnaIntestazioneBaseR, "white", XLBorderStyleValues.Thick, False, True, True, False)
                End If

                'Oneri Finanziari (48) / MON
                Dim Oneri_MON As Decimal = 0
                Dim MON As Decimal = 0
                If Oneri_Finanziari <> 0 Then
                    myKeyConto = dr("Anno") & "_MON"
                    If htConto.ContainsKey(myKeyConto) Then
                        MON = DirectCast(htConto(myKeyConto), ObjTotaleDaInserire).Totale
                    End If
                    If MON > 0 Then
                        Oneri_MON = Oneri_Finanziari / MON
                    End If
                End If

                worksheetR.Cell(RigaBaseR, ColonnaBaseR).SetValue(FormattazioneIndice(Oneri_MON))
                worksheetR.Cell(RigaBase, ColonnaBaseR).Style.NumberFormat.Format = "0.000"
                applicaStile(worksheetR, RigaBaseR, ColonnaBaseR, "white", XLBorderStyleValues.Thick, True, True, True, True)




                bFirst = False

            Next


            RigaIntestazioneBaseR = RigaIntestazioneBaseR + 1

            '=================================================================================================================================


            If (workbook.Worksheets.Count > 0) Then
                listaXLSX.Add(salvaExcel(Rag_Soc_Path & "__", path, workbook))
            End If

            If erroreDataTable <> "[" Then
                erroreDataTable = erroreDataTable.Substring(0, erroreDataTable.Length - 1)
            End If
            erroreDataTable &= "]"

        End If

        Return listaXLSX

    End Function

    Private Function FormattazioneRendiconto(ByVal Valore As Decimal) As String

        If Valore >= 0 Then
            FormattazioneRendiconto = Valore
        Else
            FormattazioneRendiconto = "(" & -Valore & ")"
        End If

        FormattazioneRendiconto = Left(Space(20), 20 - Len(FormattazioneRendiconto)) & FormattazioneRendiconto

        Return FormattazioneRendiconto

    End Function


    Private Function FormattazioneIndice(ByVal Valore As Decimal) As String

        FormattazioneIndice = Format(Valore, "0.000##")

        FormattazioneIndice = Left(Space(20), 20 - Len(FormattazioneIndice)) & FormattazioneIndice

        Return FormattazioneIndice

    End Function

    Public Function salvaExcel(nome As String, path As String, ByRef workbook As XLWorkbook) As String
        Dim pathFileXlsx As String
        Dim nomeReport As String
        nomeReport = nome & "__" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "")
        pathFileXlsx = path & nomeReport & ".xlsx"
        workbook.SaveAs(pathFileXlsx)

        Return pathFileXlsx
    End Function

    'Public Function creaFileZip(nomeReport As String, listaFileXLSX As List(Of String)) As String
    '    Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '    Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)


    '    Dim pathFileZip As String = path + nomeReport + Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") + "_" + Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") + ".zip"

    '    Dim fileManager As New FileManager(path, "b")

    '    fileManager.CreaFileZip(pathFileZip, listaFileXLSX)
    '    Return pathFileZip

    'End Function

    'Public Function eliminaFiles(listaFiles As List(Of String))
    '    Try
    '        For Each percorsoFile In listaFiles
    '            IO.File.Delete(percorsoFile)
    '        Next
    '    Catch ex As Exception
    '        Throw ex
    '    End Try

    'End Function

#End Region



#Region "Applica stili Excel"



    Private Shared Sub applicaStile(ByRef worksheet As IXLWorksheet, Riga As Integer, Colonna As Integer, Backgroundcolor As String, BorderStyle As XLBorderStyleValues, BorderTop As Boolean, BorderBottom As Boolean, BorderLeft As Boolean, BorderRight As Boolean)

        Select Case LCase(Backgroundcolor)
            Case "white"
            Case "orange"
                worksheet.Cell(Riga, Colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 204, 153)
            Case "yellow"
                worksheet.Cell(Riga, Colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0)
            Case "green"
                worksheet.Cell(Riga, Colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(207, 226, 243)
        End Select


        If BorderTop Then
            worksheet.Cell(Riga, Colonna).Style.Border.SetTopBorder(BorderStyle)
        Else
            worksheet.Cell(Riga, Colonna).Style.Border.SetTopBorder(XLBorderStyleValues.None)
        End If
        If BorderBottom Then
            worksheet.Cell(Riga, Colonna).Style.Border.SetBottomBorder(BorderStyle)
        Else
            worksheet.Cell(Riga, Colonna).Style.Border.SetBottomBorder(XLBorderStyleValues.None)
        End If
        If BorderLeft Then
            worksheet.Cell(Riga, Colonna).Style.Border.SetLeftBorder(BorderStyle)
        Else
            worksheet.Cell(Riga, Colonna).Style.Border.SetLeftBorder(XLBorderStyleValues.None)
        End If
        If BorderRight Then
            worksheet.Cell(Riga, Colonna).Style.Border.SetRightBorder(BorderStyle)
        Else
            worksheet.Cell(Riga, Colonna).Style.Border.SetRightBorder(XLBorderStyleValues.None)
        End If

    End Sub



#End Region


End Class